<RoutePrefix("api")>
Public Class CSH0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0010")>
   <Route("references/csh0010")>
   <HttpGet>
   Public Function GetReferences_CSH0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0010_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "coxTypes"         ' all defined collection types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCshCox")>
   <Route("collections/{coxId}")>
   <HttpGet>
   Public Function GetCshCox(coxId As Integer) As IHttpActionResult

      If coxId <= 0 Then
         Throw New ArgumentException("Collection ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0010")
            With _direct
               .AddParameter("CoxId", coxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "cox"
                     .Tables(1).TableName = "coxDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCshCoxLog")>
   <Route("collections/{coxId}/log")>
   <HttpGet>
   Public Function GetCshCoxLog(coxId As Integer) As IHttpActionResult

      If coxId <= 0 Then
         Throw New ArgumentException("Collection ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0010_Log")
            With _direct
               .AddParameter("CoxId", coxId)

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

   <SymAuthorization("CreateCshCox")>
   <Route("collections/{currentUserId}")>
   <HttpPost>
   Public Function CreateCshCox(currentUserId As Integer, <FromBody> cox As CshCoxBody) As IHttpActionResult

      If cox.CoxId <> -1 Then
         Throw New ArgumentException("Collection ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _coxId As Integer = SysLib.GetNextSequence("CshCoxId")

         cox.CoxId = _coxId

         '
         ' Load proposed values from payload
         '
         Dim _cshCox As New CshCox
         Dim _cshCoxDetailList As New CshCoxDetailList

         Me.LoadCshCox(cox, _cshCox)

         For Each _detail As CshCoxDetail In cox.Details
            _detail.CoxId = _coxId
            _cshCoxDetailList.Add(_detail)
         Next

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _cshCoxLogDetailList As New SysLogDetailList
         Dim _cshCoxDetailLogDetailList As New SysLogDetailList

         With _cshCox
            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)

            Dim _fundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.FundClusterId, String.Empty, .FundClusterId, .FundClusterId + "=" + _fundClusterName)

            Dim _statusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

            Dim _typeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = .CoxTypeId).CoxTypeName
            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CoxTypeId, String.Empty, .CoxTypeId.ToString, .CoxTypeId.ToString + "=" + _typeName)

            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.ReceiptNumber, String.Empty, .ReceiptNumber)

            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.PayorName, String.Empty, .PayorName)

            AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CollectorId, String.Empty, .CollectorId.ToString)

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

            Me.InsertCshCox(_cshCox)
            Me.InsertCshCoxDetails(_cshCoxDetailList)

            If _cshCoxLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("CoxId", _cshCox.CoxId)
               End With

               _id = AppLib.CreateLogHeader("InsCshCoxLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _cshCoxLogDetailList)
               AppLib.CreateLogDetails(_cshCoxLogDetailList, "CshCoxLogDetail")

            End If

            For Each _new As CshCoxDetail In _cshCoxDetailList

               With _logKeyList
                  .Clear()
                  .Add("CoxId", _new.CoxId)
                  .Add("TrxCode", _new.TrxCode)
               End With

               _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               _cshCoxDetailLogDetailList.Clear()

               With _new
                  AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, "", .TrxCode.ToString, .TrxCode.ToString + "=" + .TrxName)

                  AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)

                  AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

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
         Return Me.Ok(cox.CoxId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyCshCox")>
   <Route("collections/{coxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCshCox(coxId As Integer, currentUserId As Integer, <FromBody> cox As CshCoxBody) As IHttpActionResult

      If coxId <= 0 Then
         Throw New ArgumentException("Collection ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _cshCox As New CshCox
         Dim _cshCoxDetailList As New CshCoxDetailList

         Me.LoadCshCox(cox, _cshCox)

         For Each _detail As CshCoxDetail In cox.Details
            _cshCoxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _cshCoxOld As New CshCox
         Dim _cshCoxDetailListOld As New CshCoxDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetCshCox(coxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("cox").Rows(0)
            Me.LoadCshCox(_row, _cshCoxOld)
            Me.LoadCshCoxDetailList(_dataSet.Tables("coxDetails").Rows, _cshCoxDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' CshCox
         '

         Dim _cshCoxLogDetailList As New SysLogDetailList

         With _cshCoxOld
            If .TrxDate <> _cshCox.TrxDate Then
               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxDate, .TrxDate.ToDisplayFormat, _cshCox.TrxDate.ToDisplayFormat)
            End If

            If .FundClusterId <> _cshCox.FundClusterId Then
               Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
               Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _cshCox.FundClusterId).FundClusterName

               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _cshCox.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _cshCox.FundClusterId + "=" + _newFundClusterName)
            End If

            If .TrxStatusId <> _cshCox.TrxStatusId Then
               Dim _oldStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
               Dim _newStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = _cshCox.TrxStatusId).TrxStatusName

               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxStatusId, .TrxStatusId.ToString, _cshCox.TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _oldStatusName + "; " + _cshCox.TrxStatusId.ToString + "=" + _newStatusName)
            End If

            If .CoxTypeId <> _cshCox.CoxTypeId Then
               Dim _oldTypeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = .CoxTypeId).CoxTypeName
               Dim _newTypeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = _cshCox.CoxTypeId).CoxTypeName

               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CoxTypeId, .CoxTypeId.ToString, _cshCox.CoxTypeId.ToString, .CoxTypeId.ToString + "=" + _oldTypeName + "; " + _cshCox.CoxTypeId.ToString + "=" + _newTypeName)
            End If

            If .ReceiptNumber <> _cshCox.ReceiptNumber Then
               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.ReceiptNumber, .ReceiptNumber, _cshCox.ReceiptNumber)
            End If

            If .Particulars <> _cshCox.Particulars Then
               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _cshCox.Particulars)
            End If

            If .PayorName <> _cshCox.PayorName Then
               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.PayorName, .PayorName, _cshCox.PayorName)
            End If

            If .CollectorId <> _cshCox.CollectorId Then
               AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CollectorId, .CollectorId.ToString, _cshCox.CollectorId.ToString)
            End If

         End With

         '
         ' CshCoxDetail
         '

         Dim _cshCoxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As CshCoxDetail In _cshCoxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As CshCoxDetail In _cshCoxDetailList
               If _new.CoxDetailId = _old.CoxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As CshCoxDetail In _cshCoxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As CshCoxDetail In _cshCoxDetailListOld
               If _new.CoxDetailId = _old.CoxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .TrxCode <> _old.TrxCode OrElse .Amount <> _old.Amount Then
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

         Dim _cshCoxDetailListNew As New CshCoxDetailList      ' for adding new Cox Details

         If _addDetailCount > 0 Then
            Dim _cshCoxDetail As CshCoxDetail

            For Each _new As CshCoxDetail In _cshCoxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _cshCoxDetail = New CshCoxDetail
                  _cshCoxDetailListNew.Add(_cshCoxDetail)
                  DataLib.ScatterValues(_new, _cshCoxDetail)
                  _cshCoxDetail.CoxId = _cshCox.CoxId
               End If
            Next

         End If

         Dim _isCshCoxChanged As Boolean = Me.HasCshCoxChanges(_cshCoxOld, _cshCox)

         If Not _isCshCoxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetCshCox(coxId)
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

            If _isCshCoxChanged Then
               Me.UpdateCshCox(_cshCox)

               With _logKeyList
                  .Clear()
                  .Add("CoxId", _cshCox.CoxId)
               End With

               _id = AppLib.CreateLogHeader("InsCshCoxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _cshCoxLogDetailList)

               AppLib.CreateLogDetails(_cshCoxLogDetailList, "CshCoxLogDetail")

            End If

            '
            ' CshCoxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteCshCoxDetails(_cshCoxDetailListOld)

               For Each _old As CshCoxDetail In _cshCoxDetailListOld
                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("CoxId", _old.CoxId)
                        .Add("TrxCode", _old.TrxCode)
                     End With

                     _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _cshCoxDetailLogDetailList.Clear()

                     With _old
                        AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, .TrxCode.ToString, "")

                        AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "TrxCode=" + .TrxCode.ToString)

                     End With

                     AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

                  End If
               Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertCshCoxDetails(_cshCoxDetailListNew)

               For Each _new As CshCoxDetail In _cshCoxDetailListNew

                  With _logKeyList
                     .Clear()
                     .Add("CoxId", _new.CoxId)
                     .Add("TrxCode", _new.TrxCode)
                  End With

                  _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _cshCoxDetailLogDetailList.Clear()

                  With _new
                     AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, "", .TrxCode.ToString, .TrxCode.ToString + "=" + .TrxName)

                     AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)

                     AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

                  End With
               Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateCshCoxDetails(_cshCoxDetailList)

               For Each _new As CshCoxDetail In _cshCoxDetailList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("CoxId", _new.CoxId)
                        .Add("TrxCode", _new.TrxCode)
                     End With

                     _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _cshCoxDetailLogDetailList.Clear()

                     For Each _old As CshCoxDetail In _cshCoxDetailListOld
                        If _new.CoxDetailId = _old.CoxDetailId Then
                           With _new
                              If .TrxCode <> _old.TrxCode Then
                                 AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, _old.TrxCode.ToString, .TrxCode.ToString, _old.TrxCode.ToString + "=" + _old.TrxName + "; " + .TrxCode.ToString + "=" + .TrxName)
                              End If

                              If .Amount <> _old.Amount Then
                                 AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)
                              End If

                              AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

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

   <SymAuthorization("RemoveCshCox")>
   <Route("collections/{coxId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveCshCox(coxId As Integer, lockId As String) As IHttpActionResult

      If coxId <= 0 Then
         Throw New ArgumentException("Collection ID is required.")
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

            Me.DeleteCshCox(coxId, lockId)

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

   Private Sub LoadCshCox(cox As CshCoxBody, cshCox As CshCox)

      DataLib.ScatterValues(cox, cshCox)

   End Sub

   Private Sub LoadCshCox(row As DataRow, cox As CshCox)

      With cox
         .CoxId = row.ToInt32("CoxId")
         .TrxDate = row.ToDate("TrxDate")
         .FundClusterId = row.ToString("FundClusterId")
         .TrxStatusId = row.ToInt32("TrxStatusId")
         .CoxTypeId = row.ToInt32("CoxTypeId")
         .OpxId = row.ToInt32("OpxId")
         .ReceiptNumber = row.ToString("ReceiptNumber")
         .Particulars = row.ToString("Particulars")
         .Particulars2 = row.ToString("Particulars2")
         .PayorName = row.ToString("PayorName")
         .CollectorId = row.ToInt32("CollectorId")
         .PostUserId = row.ToInt32("PostUserId")
      End With

   End Sub

   Private Sub LoadCshCoxDetailList(rows As DataRowCollection, list As CshCoxDetailList)

      Dim _detail As CshCoxDetail
      For Each _row As DataRow In rows
         _detail = New CshCoxDetail

         With _detail
            .CoxDetailId = _row.ToInt32("CoxDetailId")
            .CoxId = _row.ToInt32("CoxId")
            .TrxCode = _row.ToInt32("TrxCode")
            .TrxName = _row.ToString("TrxName")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertCshCox(cox As CshCox)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshCox", cox, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshCox(cox)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertCshCoxDetails(list As CshCoxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("CoxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("TrxName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshCoxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshCoxDetail In list
            Me.AddInsertUpdateParamsCshCoxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateCshCox(cox As CshCox)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CoxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshCox", cox, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshCox(cox)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(cox.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateCshCoxDetails(list As CshCoxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CoxDetailId")
      End With

      With _excludedFields
         .Add("TrxName")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshCoxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshCoxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsCshCoxDetail(_detail)
               .Parameters.AddWithValue("@CoxDetailId", _detail.CoxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshCox(cox As CshCox)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CoxId", cox.CoxId)
         .AddWithValue("@TrxDate", cox.TrxDate)
         .AddWithValue("@FundClusterId", cox.FundClusterId)
         .AddWithValue("@TrxStatusId", cox.TrxStatusId)
         .AddWithValue("@CoxTypeId", cox.CoxTypeId)
         .AddWithValue("@OpxId", cox.OpxId.ToNullable)
         .AddWithValue("@ReceiptNumber", cox.ReceiptNumber)
         .AddWithValue("@Particulars", cox.Particulars)
         .AddWithValue("@Particulars2", cox.Particulars2)
         .AddWithValue("@PayorName", cox.PayorName)
         .AddWithValue("@CollectorId", cox.CollectorId)
         .AddWithValue("@PostUserId", cox.PostUserId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshCoxDetail(dtl As CshCoxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CoxId", dtl.CoxId)
         .AddWithValue("@TrxCode", dtl.TrxCode)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub

   Private Sub DeleteCshCox(coxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CoxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.CshCox", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CoxId", coxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteCshCoxDetails(list As CshCoxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.CshCoxDetail WHERE CoxDetailId=@CoxDetailId"
         .CommandType = CommandType.Text

         For Each _old As CshCoxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@CoxDetailId", _old.CoxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasCshCoxChanges(oldRecord As CshCox, newRecord As CshCox) As Boolean

      With oldRecord
         If .TrxDate <> newRecord.TrxDate Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .TrxStatusId <> newRecord.TrxStatusId Then Return True
         If .CoxTypeId <> newRecord.CoxTypeId Then Return True
         If .OpxId <> newRecord.OpxId Then Return True
         If .ReceiptNumber <> newRecord.ReceiptNumber Then Return True
         If .Particulars <> newRecord.Particulars Then Return True
         If .Particulars2 <> newRecord.Particulars2 Then Return True
         If .PayorName <> newRecord.PayorName Then Return True
         If .CollectorId <> newRecord.CollectorId Then Return True
      End With

      Return False

   End Function

End Class

Public Class CshCoxBody
   Inherits CshCox

   Public Property Details As CshCoxDetail()

End Class
