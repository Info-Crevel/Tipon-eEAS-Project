<RoutePrefix("api")>
Public Class CSH0110_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0110")>
   <Route("references/csh0110")>
   <HttpGet>
   Public Function GetReferences_CSH0110() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0110_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "banks"            ' all defined banks
                     .Tables(2).TableName = "signatories"      ' all defined signatories
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFrsAda")>
   <Route("ada/{adaId}")>
   <HttpGet>
   Public Function GetFrsAda(adaId As Integer) As IHttpActionResult

      If adaId <= 0 Then
         Throw New ArgumentException("ADA ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0110")
            With _direct
               .AddParameter("AdaId", adaId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "ada"
                     .Tables(1).TableName = "adaDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateFrsAda")>
   <Route("ada/{currentUserId}")>
   <HttpPost>
   Public Function CreateFrsAda(currentUserId As Integer, <FromBody> ada As FrsAdaBody) As IHttpActionResult

      If ada.AdaId <> -1 Then
         Throw New ArgumentException("ADA ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _adaId As Integer = SysLib.GetNextSequence("FrsAdaId")

         ada.AdaId = _adaId

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If ada.DocumentId = "< NEW >" Then
            ada.DocumentId = AppLib.GetNextDocSequence(DocSequencerId.RADAI, ada.ReportDate)
         End If

         ada.AmountText = AppLib.NumberToText(ada.NetAmount, FractionStyle.PerHundred)

         '
         ' Load proposed values from payload
         '
         Dim _frsAda As New FrsAda
         Dim _frsAdaDetailList As New FrsAdaDetailList

         Me.LoadFrsAda(ada, _frsAda)

         For Each _detail As FrsAdaDetail In ada.Details
            _detail.AdaId = _adaId
            _frsAdaDetailList.Add(_detail)
         Next

         '
         ' Log addition, save to DB
         '

         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         'Dim _cshDbxLogDetailList As New SysLogDetailList
         'Dim _cshDbxDetailLogDetailList As New SysLogDetailList

         'With _cshDbx
         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)

         '   Dim _fundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.FundClusterId, String.Empty, .FundClusterId, .FundClusterId + "=" + _fundClusterName)

         '   Dim _statusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

         '   Dim _typeName As String = FrsSession.DbsDbxType.Rows.Find(Function(m) m.DbxTypeId = .DbxTypeId).DbxTypeName
         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DbxTypeId, String.Empty, .DbxTypeId.ToString, .DbxTypeId.ToString + "=" + _typeName)

         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.ObxId, String.Empty, .ObxId.ToString, "ORS Number=" + .ORSNumber)

         '   If Not String.IsNullOrEmpty(.CheckNumber) Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.CheckNumber, String.Empty, .CheckNumber)
         '   End If

         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

         '   AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.PayeeId, String.Empty, .PayeeId.ToString + "=" + .PayeeName)

         '   If .DisburserId > 0 Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, String.Empty, .DisburserId.ToString + "=" + .DisburserName)
         '   End If

         'End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertFrsAda(_frsAda)
            Me.InsertFrsAdaDetails(_frsAdaDetailList)

            'If _cshDbxLogDetailList.Count > 0 Then
            '   With _logKeyList
            '      .Clear()
            '      .Add("DbxId", _cshDbx.DbxId)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsCshDbxLog", _logKeyList, LogActionId.Add, _currentUserId)
            '   AppLib.AssignLogHeaderId(_id, _cshDbxLogDetailList)
            '   AppLib.CreateLogDetails(_cshDbxLogDetailList, "CshDbxLogDetail")

            'End If

            'For Each _new As CshDbxDetail In _cshDbxDetailList

            '   With _logKeyList
            '      .Clear()
            '      .Add("DbxId", _new.DbxId)
            '      .Add("AccountId", _new.AccountId)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

            '   _cshDbxDetailLogDetailList.Clear()

            '   With _new
            '      AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId, .AccountId + "=" + .AccountName)

            '      AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "Account ID=" + .AccountId)

            '      AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

            '   End With

            'Next

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
         Return Me.Ok(ada.AdaId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyFrsAda")>
   <Route("ada/{adaId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFrsAda(adaId As Integer, currentUserId As Integer, <FromBody> ada As FrsAdaBody) As IHttpActionResult

      If adaId <= 0 Then
         Throw New ArgumentException("ADA ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _frsAda As New FrsAda
         Dim _frsAdaDetailList As New FrsAdaDetailList

         Me.LoadFrsAda(ada, _frsAda)

         For Each _detail As FrsAdaDetail In ada.Details
            _frsAdaDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _frsAdaOld As New FrsAda
         Dim _frsAdaDetailListOld As New FrsAdaDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetFrsAda(adaId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("ada").Rows(0)
            Me.LoadFrsAda(_row, _frsAdaOld)
            Me.LoadFrsAdaDetailList(_dataSet.Tables("adaDetails").Rows, _frsAdaDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         '
         ' CshDbx
         '

         'Dim _cshDbxLogDetailList As New SysLogDetailList

         'With _cshDbxOld
         '   If .TrxDate <> _cshDbx.TrxDate Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxDate, .TrxDate.ToDisplayFormat, _cshDbx.TrxDate.ToDisplayFormat)
         '   End If

         '   If .FundClusterId <> _cshDbx.FundClusterId Then
         '      Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
         '      Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _cshDbx.FundClusterId).FundClusterName

         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _cshDbx.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _cshDbx.FundClusterId + "=" + _newFundClusterName)
         '   End If

         '   If .TrxStatusId <> _cshDbx.TrxStatusId Then
         '      Dim _oldStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
         '      Dim _newStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = _cshDbx.TrxStatusId).TrxStatusName

         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.TrxStatusId, .TrxStatusId.ToString, _cshDbx.TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _oldStatusName + "; " + _cshDbx.TrxStatusId.ToString + "=" + _newStatusName)
         '   End If

         '   If .ObxId <> _cshDbx.ObxId Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.ObxId, .ObxId.ToString, _cshDbx.ObxId.ToString)
         '   End If

         '   If .CheckNumber <> _cshDbx.CheckNumber Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.CheckNumber, .CheckNumber, _cshDbx.CheckNumber)
         '   End If

         '   If .Particulars <> _cshDbx.Particulars Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _cshDbx.Particulars)
         '   End If

         '   If .PayeeId <> _cshDbx.PayeeId Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.PayeeId, .PayeeId.ToString + "=" + .PayeeName, _cshDbx.PayeeId.ToString + "=" + _cshDbx.PayeeName)
         '   End If

         '   If .DisburserId <> _cshDbx.DisburserId Then
         '      AppLib.AddLogDetail(_cshDbxLogDetailList, 0, LogColumnId.DisburserId, .DisburserId.ToString + "=" + .DisburserName, _cshDbx.DisburserId.ToString + "=" + _cshDbx.DisburserName)
         '   End If

         'End With

         '
         ' FrsAdaDetail
         '

         'Dim _cshDbxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FrsAdaDetail In _frsAdaDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FrsAdaDetail In _frsAdaDetailList
               If _new.DetailId = _old.DetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As FrsAdaDetail In _frsAdaDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FrsAdaDetail In _frsAdaDetailListOld
               If _new.DetailId = _old.DetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .DbxId <> _old.DbxId OrElse .AccountId <> _old.AccountId Then
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

         Dim _frsAdaDetailListNew As New FrsAdaDetailList      ' for adding new ADA Details

         If _addDetailCount > 0 Then
            Dim _frsAdaDetail As FrsAdaDetail

            For Each _new As FrsAdaDetail In _frsAdaDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _frsAdaDetail = New FrsAdaDetail
                  _frsAdaDetailListNew.Add(_frsAdaDetail)
                  DataLib.ScatterValues(_new, _frsAdaDetail)
                  _frsAdaDetail.AdaId = _frsAda.AdaId
               End If
            Next

         End If

         Dim _isFrsAdaChanged As Boolean = Me.HasFrsAdaChanges(_frsAdaOld, _frsAda)

         If Not _isFrsAdaChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current record
            ' 
            Return Me.GetFrsAda(adaId)
         End If

         If _frsAdaOld.NetAmount <> _frsAda.NetAmount Then
            _frsAda.AmountText = AppLib.NumberToText(ada.NetAmount, FractionStyle.PerHundred)
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

            If _isFrsAdaChanged Then
               Me.UpdateFrsAda(_frsAda)

               'With _logKeyList
               '   .Clear()
               '   .Add("DbxId", _cshDbx.DbxId)
               'End With

               '_id = AppLib.CreateLogHeader("InsCshDbxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               'AppLib.AssignLogHeaderId(_id, _cshDbxLogDetailList)

               'AppLib.CreateLogDetails(_cshDbxLogDetailList, "CshDbxLogDetail")

            End If

            '
            ' FrsAdaDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFrsAdaDetails(_frsAdaDetailListOld)

               'For Each _old As CshDbxDetail In _cshDbxDetailListOld
               '   If _old.LogActionId = LogActionId.Delete Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("DbxId", _old.DbxId)
               '         .Add("AccountId", _old.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

               '      _cshDbxDetailLogDetailList.Clear()

               '      With _old
               '         AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

               '         AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "Account ID=" + .AccountId)

               '      End With

               '      AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

               '   End If
               'Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertFrsAdaDetails(_frsAdaDetailListNew)

               'For Each _new As CshDbxDetail In _cshDbxDetailListNew

               '   With _logKeyList
               '      .Clear()
               '      .Add("DbxId", _new.DbxId)
               '      .Add("AccountId", _new.AccountId)
               '   End With

               '   _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               '   _cshDbxDetailLogDetailList.Clear()

               '   With _new
               '      AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId, .AccountId + "=" + .AccountName)

               '      AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "Account ID=" + .AccountId)

               '      AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

               '   End With
               'Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateFrsAdaDetails(_frsAdaDetailList)

               'For Each _new As CshDbxDetail In _cshDbxDetailList
               '   If _new.LogActionId = LogActionId.Edit Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("DbxId", _new.DbxId)
               '         .Add("AccountId", _new.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsCshDbxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

               '      _cshDbxDetailLogDetailList.Clear()

               '      For Each _old As CshDbxDetail In _cshDbxDetailListOld
               '         If _new.DbxDetailId = _old.DbxDetailId Then
               '            With _new
               '               If .AccountId <> _old.AccountId Then
               '                  AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId, _old.AccountId + "=" + _old.AccountName + "; " + .AccountId + "=" + .AccountName)
               '               End If

               '               If .Amount <> _old.Amount Then
               '                  AppLib.AddLogDetail(_cshDbxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "Account ID=" + .AccountId)
               '               End If

               '               AppLib.CreateLogDetails(_cshDbxDetailLogDetailList, "CshDbxDetailLogDetail")

               '            End With

               '         End If
               '      Next

               '   End If

               'Next

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

   <SymAuthorization("RemoveFrsAda")>
   <Route("ada/{adaId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveFrsAda(adaId As Integer, lockId As String) As IHttpActionResult

      If adaId <= 0 Then
         Throw New ArgumentException("ADA ID is required.")
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

            Me.DeleteFrsAda(adaId, lockId)

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

   Private Sub LoadFrsAda(ada As FrsAdaBody, frsAda As FrsAda)

      DataLib.ScatterValues(ada, frsAda)

   End Sub

   Private Sub LoadFrsAda(row As DataRow, ada As FrsAda)

      With ada
         .AdaId = row.ToInt32("AdaId")
         .ReportDate = row.ToDate("ReportDate")
         .DocumentId = row.ToString("DocumentId")
         .DepartmentName = row.ToString("DepartmentName")
         .AgencyName = row.ToString("AgencyName")
         .OrgCode = row.ToString("OrgCode")
         .FundClusterId = row.ToString("FundClusterId")
         .BankId = row.ToInt32("BankId")
         .NcaNumber = row.ToString("NcaNumber")
         .ControlNumber = row.ToString("ControlNumber")
         .BatchName = row.ToString("BatchName")
         .TotalAmount = row.ToDecimal("TotalAmount")
         .TaxAmount = row.ToDecimal("TaxAmount")
         .NetAmount = row.ToDecimal("NetAmount")
         .AmountText = row.ToString("AmountText")
         .PostUserId = row.ToInt32("PostUserId")
         .AgencySignatoryId = row.ToInt32("AgencySignatoryId")
         .AccountingSignatoryId = row.ToInt32("AccountingSignatoryId")
         .CashSignatoryId = row.ToInt32("CashSignatoryId")
      End With

   End Sub

   Private Sub LoadFrsAdaDetailList(rows As DataRowCollection, list As FrsAdaDetailList)

      Dim _detail As FrsAdaDetail
      For Each _row As DataRow In rows
         _detail = New FrsAdaDetail

         With _detail
            .DetailId = _row.ToInt32("DetailId")
            .AdaId = _row.ToInt32("AdaId")
            .DbxId = _row.ToInt32("DbxId")
            .AccountId = _row.ToString("AccountId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertFrsAda(ada As FrsAda)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsAda", ada, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsAda(ada)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFrsAdaDetails(list As FrsAdaDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         '.Add("AccountName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsAdaDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsAdaDetail In list
            Me.AddInsertUpdateParamsFrsAdaDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateFrsAda(ada As FrsAda)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("AdaId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsAda", ada, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsAda(ada)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(ada.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFrsAdaDetails(list As FrsAdaDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("DetailId")
      End With

      With _excludedFields
         '.Add("AccountName")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsAdaDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsAdaDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFrsAdaDetail(_detail)
               .Parameters.AddWithValue("@DetailId", _detail.DetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsAda(ada As FrsAda)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AdaId", ada.AdaId)
         .AddWithValue("@ReportDate", ada.ReportDate)
         .AddWithValue("@DocumentId", ada.DocumentId)
         .AddWithValue("@DepartmentName", ada.DepartmentName)
         .AddWithValue("@AgencyName", ada.AgencyName)
         .AddWithValue("@OrgCode", ada.OrgCode)
         .AddWithValue("@FundClusterId", ada.FundClusterId)
         .AddWithValue("@BankId", ada.BankId)
         .AddWithValue("@NcaNumber", ada.NcaNumber)
         .AddWithValue("@ControlNumber", ada.ControlNumber)
         .AddWithValue("@BatchName", ada.BatchName)
         .AddWithValue("@TotalAmount", ada.TotalAmount)
         .AddWithValue("@TaxAmount", ada.TaxAmount)
         .AddWithValue("@AmountText", ada.AmountText)
         .AddWithValue("@PostUserId", ada.PostUserId)
         .AddWithValue("@AgencySignatoryId", ada.AgencySignatoryId)
         .AddWithValue("@AccountingSignatoryId", ada.AccountingSignatoryId)
         .AddWithValue("@CashSignatoryId", ada.CashSignatoryId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsAdaDetail(dtl As FrsAdaDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AdaId", dtl.AdaId)
         .AddWithValue("@DbxId", dtl.DbxId)
         .AddWithValue("@AccountId", dtl.AccountId)
      End With

   End Sub

   Private Sub DeleteFrsAda(adaId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AdaId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FrsAda", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AdaId", adaId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFrsAdaDetails(list As FrsAdaDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FrsAdaDetail WHERE DetailId=@DetailId"
         .CommandType = CommandType.Text

         For Each _old As FrsAdaDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@DetailId", _old.DetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasFrsAdaChanges(oldRecord As FrsAda, newRecord As FrsAda) As Boolean

      With oldRecord
         If .ReportDate <> newRecord.ReportDate Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .BankId <> newRecord.BankId Then Return True
         If .NcaNumber <> newRecord.NcaNumber Then Return True
         If .ControlNumber <> newRecord.ControlNumber Then Return True
         If .BatchName <> newRecord.BatchName Then Return True
         If .TotalAmount <> newRecord.TotalAmount Then Return True
         If .TaxAmount <> newRecord.TaxAmount Then Return True
         If .AgencySignatoryId <> newRecord.AgencySignatoryId Then Return True
         If .AccountingSignatoryId <> newRecord.AccountingSignatoryId Then Return True
         If .CashSignatoryId <> newRecord.CashSignatoryId Then Return True
      End With

      Return False

   End Function

End Class

Public Class FrsAdaBody
   Inherits FrsAda

   Public Property Details As FrsAdaDetail()

End Class
