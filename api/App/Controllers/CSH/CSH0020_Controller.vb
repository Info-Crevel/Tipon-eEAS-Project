<RoutePrefix("api")>
Public Class CSH0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0020")>
   <Route("references/csh0020")>
   <HttpGet>
   Public Function GetReferences_CSH0020() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0020_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "dbxTypes"         ' all defined disbursement types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCshDbx")>
   <Route("disbursements/{dbxId}")>
   <HttpGet>
   Public Function GetCshDbx(dbxId As Integer) As IHttpActionResult

      If dbxId <= 0 Then
         Throw New ArgumentException("Disbursement ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0020")
            With _direct
               .AddParameter("DbxId", dbxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "dbx"
                     .Tables(1).TableName = "dbxDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCshDbxLog")>
   <Route("disbursements/{dbxId}/log")>
   <HttpGet>
   Public Function GetCshDbxLog(dbxId As Integer) As IHttpActionResult

      If dbxId <= 0 Then
         Throw New ArgumentException("Disbursement ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0020_Log")
            With _direct
               .AddParameter("DbxId", dbxId)

               ' Allow DateTime
               Dim _settings As New JsonSerializerSettings
               With _settings
                  .ContractResolver = New CamelCasePropertyNamesContractResolver
                  .DateFormatString = "yyyy-MM-ddTHH:mm"
               End With

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  'Return Me.Ok(_dataTable)
                  Return Json(_dataTable, _settings)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateCshDbx")>
   <Route("disbursements/{currentUserId}")>
   <HttpPost>
   Public Function CreateCshDbx(currentUserId As Integer, <FromBody> dbx As CshDbxBody) As IHttpActionResult

      If dbx.DbxId <> -1 Then
         Throw New ArgumentException("Disbursement ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _dbxId As Integer = SysLib.GetNextSequence("CshDbxId")

         dbx.DbxId = _dbxId

         '
         ' Load proposed values from payload
         '
         Dim _cshDbx As New CshDbx
         Dim _cshDbxDetailList As New CshDbxDetailList

         Me.LoadCshDbx(dbx, _cshDbx)

         For Each _detail As CshDbxDetail In dbx.Details
            _detail.DbxId = _dbxId
            _cshDbxDetailList.Add(_detail)
         Next

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _cshDbxLogDetailList As New SysLogDetailList
         Dim _cshDbxDetailLogDetailList As New SysLogDetailList

         With _cshDbx
            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)

            Dim _fundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.FundClusterId, String.Empty, .FundClusterId, .FundClusterId + "=" + _fundClusterName)

            Dim _statusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

            Dim _typeName As String = FrsSession.DbsDbxType.Rows.Find(Function(m) m.DbxTypeId = .DbxTypeId).DbxTypeName
            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DbxTypeId, String.Empty, .DbxTypeId.ToString, .DbxTypeId.ToString + "=" + _typeName)

            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.ObxId, String.Empty, .ObxId.ToString, "ORS Number=" + .ORSNumber)

            If Not String.IsNullOrEmpty(.CheckNumber) Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.CheckNumber, String.Empty, .CheckNumber)
            End If

            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

            AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.PayeeId, String.Empty, .PayeeId.ToString + "=" + .PayeeName)

            If .DisburserId > 0 Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, String.Empty, .DisburserId.ToString + "=" + .DisburserName)
            End If

         End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertCshDbx(_cshDbx)
            Me.InsertCshDbxDetails(_cshDbxDetailList)

            If _cshDbxLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("DbxId", _cshDbx.DbxId)
               End With

               _id = AppLib.CreateLogHeader("InsCshDbxLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _cshDbxLogDetailList)
               AppLib.CreateLogDetails(_cshDbxLogDetailList, "CshDbxLogDetail")

            End If

            For Each _new As CshDbxDetail In _cshDbxDetailList

               With _logKeyList
                  .Clear()
                  .Add("DbxId", _new.DbxId)
                  .Add("AccountId", _new.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               _cshDbxDetailLogDetailList.Clear()

               With _new
                  AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId, .AccountId + "=" + .AccountName)

                  AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "Account ID=" + .AccountId)

                  AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

               End With

            Next

            _successFlag = True

         Catch _exception As Exception
            'File.WriteAllText("d:\yyy.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         'Return Me.Ok(True)
         Return Me.Ok(dbx.DbxId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyCshDbx")>
   <Route("disbursements/{dbxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCshDbx(dbxId As Integer, currentUserId As Integer, <FromBody> dbx As CshDbxBody) As IHttpActionResult

      If dbxId <= 0 Then
         Throw New ArgumentException("Disbursement ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _cshDbx As New CshDbx
         Dim _cshDbxDetailList As New CshDbxDetailList

         Me.LoadCshDbx(dbx, _cshDbx)

         For Each _detail As CshDbxDetail In dbx.Details
            _cshDbxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _cshDbxOld As New CshDbx
         Dim _cshDbxDetailListOld As New CshDbxDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetCshDbx(dbxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("dbx").Rows(0)
            Me.LoadCshDbx(_row, _cshDbxOld)
            Me.LoadCshDbxDetailList(_dataSet.Tables("dbxDetails").Rows, _cshDbxDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' CshDbx
         '

         Dim _cshDbxLogDetailList As New SysLogDetailList

         With _cshDbxOld
            If .TrxDate <> _cshDbx.TrxDate Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxDate, .TrxDate.ToDisplayFormat, _cshDbx.TrxDate.ToDisplayFormat)
            End If

            If .FundClusterId <> _cshDbx.FundClusterId Then
               Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
               Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _cshDbx.FundClusterId).FundClusterName

               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _cshDbx.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _cshDbx.FundClusterId + "=" + _newFundClusterName)
            End If

            If .TrxStatusId <> _cshDbx.TrxStatusId Then
               Dim _oldStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
               Dim _newStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = _cshDbx.TrxStatusId).TrxStatusName

               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxStatusId, .TrxStatusId.ToString, _cshDbx.TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _oldStatusName + "; " + _cshDbx.TrxStatusId.ToString + "=" + _newStatusName)
            End If

            If .ObxId <> _cshDbx.ObxId Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.ObxId, .ObxId.ToString, _cshDbx.ObxId.ToString)
            End If

            If .CheckNumber <> _cshDbx.CheckNumber Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.CheckNumber, .CheckNumber, _cshDbx.CheckNumber)
            End If

            If .Particulars <> _cshDbx.Particulars Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _cshDbx.Particulars)
            End If

            If .PayeeId <> _cshDbx.PayeeId Then
               AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.PayeeId, .PayeeId.ToString + "=" + .PayeeName, _cshDbx.PayeeId.ToString + "=" + _cshDbx.PayeeName)
            End If

            If .DisburserId <> _cshDbx.DisburserId Then
               'AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, .DisburserId.ToString + "=" + .DisburserName, _cshDbx.DisburserId.ToString + "=" + _cshDbx.DisburserName)
               If .DisburserId = 0 Then
                  AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, String.Empty, _cshDbx.DisburserId.ToString + "=" + _cshDbx.DisburserName)
               ElseIf _cshDbx.DisburserId = 0 Then
                  AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, .DisburserId.ToString + "=" + .DisburserName, String.Empty)
               Else
                  AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, .DisburserId.ToString + "=" + .DisburserName, _cshDbx.DisburserId.ToString + "=" + _cshDbx.DisburserName)
               End If
            End If

         End With

         '
         ' CshDbxDetail
         '

         Dim _cshDbxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As CshDbxDetail In _cshDbxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As CshDbxDetail In _cshDbxDetailList
               If _new.DbxDetailId = _old.DbxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As CshDbxDetail In _cshDbxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As CshDbxDetail In _cshDbxDetailListOld
               If _new.DbxDetailId = _old.DbxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .AccountId <> _old.AccountId OrElse .Amount <> _old.Amount Then
                        .LogActionId = LogActionId.Edit
                     End If

                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addDetailCount = _addDetailCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editDetailCount = _editDetailCount + 1
            End If

         Next

         Dim _cshDbxDetailListNew As New CshDbxDetailList      ' for adding new Dbx Details

         If _addDetailCount > 0 Then
            Dim _cshDbxDetail As CshDbxDetail

            For Each _new As CshDbxDetail In _cshDbxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _cshDbxDetail = New CshDbxDetail
                  _cshDbxDetailListNew.Add(_cshDbxDetail)
                  DataLib.ScatterValues(_new, _cshDbxDetail)
                  _cshDbxDetail.DbxId = _cshDbx.DbxId
               End If
            Next

         End If

         Dim _isCshDbxChanged As Boolean = Me.HasCshDbxChanges(_cshDbxOld, _cshDbx)

         If Not _isCshDbxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetCshDbx(dbxId)
         End If

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            If _isCshDbxChanged Then
               Me.UpdateCshDbx(_cshDbx)

               With _logKeyList
                  .Clear()
                  .Add("DbxId", _cshDbx.DbxId)
               End With

               _id = AppLib.CreateLogHeader("InsCshDbxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _cshDbxLogDetailList)

               AppLib.CreateLogDetails(_cshDbxLogDetailList, "CshDbxLogDetail")

            End If

            '
            ' CshDbxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteCshDbxDetails(_cshDbxDetailListOld)

               For Each _old As CshDbxDetail In _cshDbxDetailListOld
                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("DbxId", _old.DbxId)
                        .Add("AccountId", _old.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _cshDbxDetailLogDetailList.Clear()

                     With _old
                        AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

                        AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "Account ID=" + .AccountId)

                     End With

                     AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

                  End If
               Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertCshDbxDetails(_cshDbxDetailListNew)

               For Each _new As CshDbxDetail In _cshDbxDetailListNew

                  With _logKeyList
                     .Clear()
                     .Add("DbxId", _new.DbxId)
                     .Add("AccountId", _new.AccountId)
                  End With

                  _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _cshDbxDetailLogDetailList.Clear()

                  With _new
                     AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId, .AccountId + "=" + .AccountName)

                     AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "Account ID=" + .AccountId)

                     AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

                  End With
               Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateCshDbxDetails(_cshDbxDetailList)

               For Each _new As CshDbxDetail In _cshDbxDetailList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("DbxId", _new.DbxId)
                        .Add("AccountId", _new.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _cshDbxDetailLogDetailList.Clear()

                     For Each _old As CshDbxDetail In _cshDbxDetailListOld
                        If _new.DbxDetailId = _old.DbxDetailId Then
                           With _new
                              If .AccountId <> _old.AccountId Then
                                 AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId, _old.AccountId + "=" + _old.AccountName + "; " + .AccountId + "=" + .AccountName)
                              End If

                              If .Amount <> _old.Amount Then
                                 AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "Account ID=" + .AccountId)
                              End If

                              AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

                           End With

                        End If
                     Next

                  End If

               Next

            End If

            _successFlag = True

         Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         Return Me.Ok(True)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveCshDbx")>
   <Route("disbursements/{dbxId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveCshDbx(dbxId As Integer, lockId As String) As IHttpActionResult

      If dbxId <= 0 Then
         Throw New ArgumentException("Disbursement ID is required.")
      End If

      Try
         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.DeleteCshDbx(dbxId, lockId)

            _successFlag = True

         Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         Return Me.Ok(True)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   Private Sub LoadCshDbx(dbx As CshDbxBody, cshDbx As CshDbx)

      DataLib.ScatterValues(dbx, cshDbx)

   End Sub

   Private Sub LoadCshDbx(row As DataRow, dbx As CshDbx)

      With dbx
         .DbxId = row.ToInt32("DbxId")
         .TrxDate = row.ToDate("TrxDate")
         .FundClusterId = row.ToString("FundClusterId")
         .TrxStatusId = row.ToInt32("TrxStatusId")
         .DbxTypeId = row.ToInt32("DbxTypeId")
         .ObxId = row.ToInt32("ObxId")
         .VoucherId = row.ToString("VoucherId")
         .CheckNumber = row.ToString("CheckNumber")
         .Particulars = row.ToString("Particulars")
         .PayeeId = row.ToInt32("PayeeId")
         .PayeeBankName = row.ToString("PayeeBankName")
         .PayeeBankAccountNumber = row.ToString("PayeeBankAccountNumber")
         .DisburserId = row.ToInt32("DisburserId")
         .DisburserName = row.ToString("DisburserName")
         .PostUserId = row.ToInt32("PostUserId")
         .TotalAmount = row.ToDecimal("TotalAmount")
         .TaxAmount = row.ToDecimal("TaxAmount")
      End With

   End Sub

   Private Sub LoadCshDbxDetailList(rows As DataRowCollection, list As CshDbxDetailList)

      Dim _detail As CshDbxDetail
      For Each _row As DataRow In rows
         _detail = New CshDbxDetail

         With _detail
            .DbxDetailId = _row.ToInt32("DbxDetailId")
            .DbxId = _row.ToInt32("DbxId")
            .AccountId = _row.ToString("AccountId")
            .AccountName = _row.ToString("AccountName")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertCshDbx(dbx As CshDbx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ORSNumber")
         .Add("PayeeName")
         .Add("DisburserName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshDbx", dbx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshDbx(dbx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertCshDbxDetails(list As CshDbxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DbxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("AccountName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshDbxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshDbxDetail In list
            Me.AddInsertUpdateParamsCshDbxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateCshDbx(dbx As CshDbx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("DbxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("ORSNumber")
         .Add("PayeeName")
         .Add("DisburserName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshDbx", dbx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshDbx(dbx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(dbx.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateCshDbxDetails(list As CshDbxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("DbxDetailId")
      End With

      With _excludedFields
         .Add("AccountName")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshDbxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshDbxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsCshDbxDetail(_detail)
               .Parameters.AddWithValue("@DbxDetailId", _detail.DbxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshDbx(dbx As CshDbx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DbxId", dbx.DbxId)
         .AddWithValue("@TrxDate", dbx.TrxDate)
         .AddWithValue("@FundClusterId", dbx.FundClusterId)
         .AddWithValue("@TrxStatusId", dbx.TrxStatusId)
         .AddWithValue("@DbxTypeId", dbx.DbxTypeId)
         .AddWithValue("@ObxId", dbx.ObxId)
         .AddWithValue("@VoucherId", dbx.VoucherId)
         .AddWithValue("@CheckNumber", dbx.CheckNumber.ToNullable)
         .AddWithValue("@Particulars", dbx.Particulars)
         .AddWithValue("@PayeeId", dbx.PayeeId)
         .AddWithValue("@PayeeBankName", dbx.PayeeBankName.ToNullable)
         .AddWithValue("@PayeeBankAccountNumber", dbx.PayeeBankAccountNumber.ToNullable)
         .AddWithValue("@DisburserId", dbx.DisburserId.ToNullable)
         .AddWithValue("@PostUserId", dbx.PostUserId)
         .AddWithValue("@TotalAmount", dbx.TotalAmount)
         .AddWithValue("@TaxAmount", dbx.TaxAmount)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshDbxDetail(dtl As CshDbxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DbxId", dtl.DbxId)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub

   Private Sub DeleteCshDbx(dbxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DbxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.CshDbx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DbxId", dbxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteCshDbxDetails(list As CshDbxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.CshDbxDetail WHERE DbxDetailId=@DbxDetailId"
         .CommandType = CommandType.Text

         For Each _old As CshDbxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@DbxDetailId", _old.DbxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasCshDbxChanges(oldRecord As CshDbx, newRecord As CshDbx) As Boolean

      With oldRecord
         If .TrxDate <> newRecord.TrxDate Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .TrxStatusId <> newRecord.TrxStatusId Then Return True
         If .CheckNumber <> newRecord.CheckNumber Then Return True
         If .Particulars <> newRecord.Particulars Then Return True
         If .PayeeId <> newRecord.PayeeId Then Return True
         If .PayeeBankName <> newRecord.PayeeBankName Then Return True
         If .PayeeBankAccountNumber <> newRecord.PayeeBankAccountNumber Then Return True
         If .DisburserId <> newRecord.DisburserId Then Return True
         If .TotalAmount <> newRecord.TotalAmount Then Return True
         If .TaxAmount <> newRecord.TaxAmount Then Return True
      End With

      Return False

   End Function

End Class

Public Class CshDbxBody
   Inherits CshDbx

   Public Property Details As CshDbxDetail()

End Class
