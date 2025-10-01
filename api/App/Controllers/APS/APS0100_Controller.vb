<RoutePrefix("api")>
Public Class APS0100_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_APS0100")>
   <Route("references/aps0100")>
   <HttpGet>
   Public Function GetReferences_FIN0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0100_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "types"     ' all defined Fund Clusters
                     .Tables(1).TableName = "terms"      ' all defined cost centers
                     .Tables(2).TableName = "banks"      ' all defined cost centers
                     .Tables(3).TableName = "taxes"      ' all defined cost centers
                     .Tables(4).TableName = "ataxes"      ' all defined cost centers
                     .Tables(5).TableName = "control"      ' all defined cost centers

                     .Tables(6).TableName = "orgs"      ' all defined cost centers
                     .Tables(7).TableName = "platforms"      ' all defined cost centers
                     .Tables(8).TableName = "clusters"      ' all defined cost centers
                     .Tables(9).TableName = "payGroups"      ' all defined cost centers
                     .Tables(10).TableName = "payees"      ' all defined cost centers

                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetPayableVoucherTrx")>
   <Route("trans-pv/{trxId}")>
   <HttpGet>
   Public Function GetPayableVoucherTrx(trxId As Integer) As IHttpActionResult

      If trxId <= 0 Then
         Throw New ArgumentException("Transaction ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0100")
            With _direct
               .AddParameter("TrxId", trxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "trx"
                     .Tables(1).TableName = "trxDetails"
                     '.Tables(2).TableName = "aps"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetPayableVoucherTrxLog")>
   <Route("trans-pv/{trxId}/log")>
   <HttpGet>
   Public Function GetPayableVoucherTrxLog(trxId As Integer) As IHttpActionResult

      If trxId <= 0 Then
         Throw New ArgumentException("Transaction ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0100_Log")
            With _direct
               .AddParameter("TrxId", trxId)

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

   <SymAuthorization("CreatePayableVoucherTrx")>
   <Route("trans-pv/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableVoucherTrx(currentUserId As Integer, <FromBody> trx As PayableVoucherTrxBody) As IHttpActionResult

      If trx.TrxId <> -1 Then
         Throw New ArgumentException("Transaction ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _trxId As Integer = SysLib.GetNextSequence("FinTrxId")

         trx.TrxId = _trxId
         'trx.CashReceiptTypeId = 1 'AR PAYMENT
         trx.TrxTypeId = 21 'Accounts Payable
         trx.TrxStatusId = 1 'Posted

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If trx.DocumentId = "< NEW >" Then
            trx.DocumentId = AppLib.GetNextTrxTypeSequence(NextSeqId.AccountsPayable)
         End If

         '
         ' Load proposed values from payload
         '
         Dim _finTrx As New ApsFinTrx
         Dim _finTrxDetailList As New FinTrxDetailList
         'Dim _apsTrx As New ApsTrx
         Dim _apsTrx As New ApsTrx


         With _apsTrx
            .TrxId = trx.TrxId
            .PayeeId = trx.PayeeId
            .PayableTypeId = trx.PayableTypeId
            .PayableTaxCode = trx.PayableTaxCode
            .ATaxCode = trx.ATaxCode
            .DueDate = trx.DueDate
            .CheckDate = trx.CheckDate
            .Amount = trx.Amount
         End With


         Me.LoadApsFinTrx(trx, _finTrx)

         'Dim _trxDetailId As Integer = SysLib.GetNextSequence("TrxDetailId", trx.Details.Count)

         For Each _detail As FinTrxDetail In trx.Details
            _detail.TrxId = _trxId
            _finTrxDetailList.Add(_detail)
         Next

         'For Each _aps As ApsTrx In trx.PayableVoucher
         '   _aps.TrxId = _trxId
         '   _apsTrx.Add(_aps)
         'Next


         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _finTrxLogDetailList As New SysLogDetailList
         Dim _finTrxDetailLogDetailList As New SysLogDetailList

         With _finTrx
            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)


            'Dim _statusName As String = EasSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
            'AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.DocumentId, String.Empty, .DocumentId)

            'Dim _costCenterName As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
            'AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.CostCenterId, String.Empty, .CostCenterId.ToString, .CostCenterId.ToString + "=" + _costCenterName)

            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

            If Not String.IsNullOrEmpty(.Remarks) Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Remarks, String.Empty, .Remarks)
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

            Me.InsertFinTrx(_finTrx)
            Me.InsertFinTrxDetails(_finTrxDetailList)
            Me.InsertApsTrx(_apsTrx)


            If _finTrxLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("TrxId", _finTrx.TrxId)
               End With

               _id = AppLib.CreateLogHeader("InsFinTrxLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _finTrxLogDetailList)
               AppLib.CreateLogDetails(_finTrxLogDetailList, "FinTrxLogDetail")

            End If

            For Each _new As FinTrxDetail In _finTrxDetailList

               With _logKeyList
                  .Clear()
                  .Add("TrxId", _new.TrxId)
                  .Add("AccountId", _new.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsFinTrxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               _finTrxDetailLogDetailList.Clear()

               With _new
                  If .DebitAmount > 0 Then
                     AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.DebitAmount, "", .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
                  End If
                  If .CreditAmount > 0 Then
                     AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.CreditAmount, "", .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
                  End If

                  AppLib.CreateLogDetails(_finTrxDetailLogDetailList, "FinTrxDetailLogDetail")

               End With
            Next

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

         'Return Me.Ok(True)
         Return Me.Ok(trx.TrxId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyPayableVoucherTrx")>
   <Route("trans-pv/{trxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableVoucherTrx(trxId As Integer, currentUserId As Integer, <FromBody> trx As PayableVoucherTrxBody) As IHttpActionResult

      If trxId <= 0 Then
         Throw New ArgumentException("Transaction ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)      
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _finTrx As New ApsFinTrx
         Dim _finTrxDetailList As New FinTrxDetailList
         Dim _apsTrx As New ApsTrx


         Me.LoadApsFinTrx(trx, _finTrx)

         With _apsTrx
            .TrxId = trx.TrxId
            .PayeeId = trx.PayeeId
            .PayableTypeId = trx.PayableTypeId
            .PayableTaxCode = trx.PayableTaxCode
            .ATaxCode = trx.ATaxCode
            .DueDate = trx.DueDate
            .CheckDate = trx.CheckDate
            .Amount = trx.Amount
            .LockId = trx.ApsLockId
         End With

         For Each _detail As FinTrxDetail In trx.Details
            _finTrxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _finTrxOld As New ApsFinTrx
         Dim _finTrxDetailListOld As New FinTrxDetailList
         Dim _apsTrxOld As New ApsTrx
         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetPayableVoucherTrx(trxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("trx").Rows(0)
            Me.LoadApsFinTrx(_row, _finTrxOld)




            Me.LoadFinTrxDetailList(_dataSet.Tables("trxDetails").Rows, _finTrxDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' FinTrx
         '

         Dim _finTrxLogDetailList As New SysLogDetailList


         With _apsTrxOld
            .TrxId = _finTrxOld.TrxId
            .PayeeId = _finTrxOld.PayeeId
            .PayableTypeId = _finTrxOld.PayableTypeId
            .PayableTaxCode = _finTrxOld.PayableTaxCode
            .ATaxCode = _finTrxOld.ATaxCode
            .DueDate = _finTrxOld.DueDate
            .CheckDate = _finTrxOld.CheckDate
            .Amount = _finTrxOld.Amount
            .LockId = _finTrxOld.ApsLockId

         End With

         With _finTrxOld
            If .TrxDate <> _finTrx.TrxDate Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.PostDate, .TrxDate.ToDisplayFormat, _finTrx.TrxDate.ToDisplayFormat)
            End If

            'If .FundClusterId <> _finTrx.FundClusterId Then
            '   Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
            '   Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _finTrx.FundClusterId).FundClusterName

            '   AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _finTrx.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _finTrx.FundClusterId + "=" + _newFundClusterName)
            'End If

            'If .TrxStatusId <> _finTrx.TrxStatusId Then
            '   Dim _oldStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
            '   Dim _newStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = _finTrx.TrxStatusId).TrxStatusName

            '   AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.TrxStatusId, .TrxStatusId.ToString, _finTrx.TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _oldStatusName + "; " + _finTrx.TrxStatusId.ToString + "=" + _newStatusName)
            'End If

            If .DocumentId <> _finTrx.DocumentId Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.DocumentId, .DocumentId, _finTrx.DocumentId)
            End If

            'If .CostCenterId <> _finTrx.CostCenterId Then
            '   Dim _oldCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
            '   Dim _newCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = _finTrx.CostCenterId).CostCenterShortName

            '   AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.CostCenterId, .CostCenterId.ToString, _finTrx.CostCenterId.ToString, .CostCenterId.ToString + "=" + _oldCostCenter + "; " + _finTrx.CostCenterId.ToString + "=" + _newCostCenter)
            'End If
            If .Reference <> _finTrx.Reference Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Reference, .Reference, _finTrx.Reference)
            End If


            If .Particulars <> _finTrx.Particulars Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _finTrx.Particulars)
            End If

            If .Remarks <> _finTrx.Remarks Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Remarks, .Remarks, _finTrx.Remarks)
            End If

         End With

         '
         ' FinTrxDetail
         '

         Dim _finTrxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FinTrxDetail In _finTrxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FinTrxDetail In _finTrxDetailList
               If _new.TrxDetailId = _old.TrxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As FinTrxDetail In _finTrxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FinTrxDetail In _finTrxDetailListOld
               If _new.TrxDetailId = _old.TrxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .AccountId <> _old.AccountId Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .DebitAmount <> _old.DebitAmount OrElse .CreditAmount <> _old.CreditAmount OrElse .OrgId <> _old.OrgId OrElse .PlatformId <> _old.PlatformId OrElse .ClientPayGroupId <> _old.ClientPayGroupId OrElse .ClusterId <> _old.ClusterId OrElse .PayeeId <> _old.PayeeId Then
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

         Dim _finTrxDetailListNew As New FinTrxDetailList      ' for adding new Trx Details

         If _addDetailCount > 0 Then
            'Dim _trxDetailId As Integer = SysLib.GetNextSequence("TrxDetailId", _addDetailCount)
            Dim _finTrxDetail As FinTrxDetail

            For Each _new As FinTrxDetail In _finTrxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _finTrxDetail = New FinTrxDetail
                  _finTrxDetailListNew.Add(_finTrxDetail)
                  DataLib.ScatterValues(_new, _finTrxDetail)
                  _finTrxDetail.TrxId = _finTrx.TrxId

                  '_finTrxDetail.TrxDetailId = _trxDetailId
                  '_trxDetailId = _trxDetailId + 1
               End If
            Next

         End If

         Dim _isFinTrxChanged As Boolean = Me.HasFinTrxChanges(_finTrxOld, _finTrx)
         Dim _isApsTrxChanged As Boolean = Me.HasApsTrxChanges(_apsTrxOld, _apsTrx)

         If Not _isFinTrxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 AndAlso Not _isApsTrxChanged Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetPayableVoucherTrx(trxId)
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

            If _isFinTrxChanged Then
               Me.UpdateFinTrx(_finTrx)

               With _logKeyList
                  .Clear()
                  .Add("TrxId", _finTrx.TrxId)
               End With

               _id = AppLib.CreateLogHeader("InsFinTrxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _finTrxLogDetailList)

               AppLib.CreateLogDetails(_finTrxLogDetailList, "FinTrxLogDetail")

            End If
            Try

               If _isApsTrxChanged Then
                  Me.UpdateApsTrx(_apsTrx)
               End If

            Catch ex As Exception
            End Try

            '
            ' FinTrxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFinTrxDetails(_finTrxDetailListOld)

               For Each _old As FinTrxDetail In _finTrxDetailListOld
                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("TrxId", _old.TrxId)
                        .Add("AccountId", _old.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsFinTrxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _finTrxDetailLogDetailList.Clear()

                     With _old
                        AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

                        If .DebitAmount > 0 Then
                           AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.DebitAmount, .DebitAmount.ToString("N"), "", "AccountId=" + .AccountId)
                        End If
                        If .CreditAmount > 0 Then
                           AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.CreditAmount, .CreditAmount.ToString("N"), "", "AccountId=" + .AccountId)
                        End If
                     End With

                     AppLib.CreateLogDetails(_finTrxDetailLogDetailList, "FinTrxDetailLogDetail")

                  End If
               Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertFinTrxDetails(_finTrxDetailListNew)

               For Each _new As FinTrxDetail In _finTrxDetailListNew

                  With _logKeyList
                     .Clear()
                     .Add("TrxId", _new.TrxId)
                     .Add("AccountId", _new.AccountId)
                  End With

                  _id = AppLib.CreateLogHeader("InsFinTrxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _finTrxDetailLogDetailList.Clear()

                  With _new
                     AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

                     If .DebitAmount > 0 Then
                        AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.DebitAmount, "", .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
                     End If
                     If .CreditAmount > 0 Then
                        AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.CreditAmount, "", .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
                     End If

                     AppLib.CreateLogDetails(_finTrxDetailLogDetailList, "FinTrxDetailLogDetail")

                  End With
               Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateFinTrxDetails(_finTrxDetailList)

               For Each _new As FinTrxDetail In _finTrxDetailList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("TrxId", _new.TrxId)
                        .Add("AccountId", _new.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsFinTrxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _finTrxDetailLogDetailList.Clear()

                     For Each _old As FinTrxDetail In _finTrxDetailListOld
                        If _new.TrxDetailId = _old.TrxDetailId Then
                           With _new
                              If .AccountId <> _old.AccountId Then
                                 AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId)
                              End If
                              If .DebitAmount <> _old.DebitAmount Then
                                 AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.DebitAmount, _old.DebitAmount.ToString("N"), .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
                              End If
                              If .CreditAmount <> _old.CreditAmount Then
                                 AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.CreditAmount, _old.CreditAmount.ToString("N"), .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
                              End If

                              AppLib.CreateLogDetails(_finTrxDetailLogDetailList, "FinTrxDetailLogDetail")

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

   <SymAuthorization("RemovePayableVoucherTrx")>
   <Route("trans-pv/{trxId}/{lockId}")>
   <HttpDelete>
   Public Function RemovePayableVoucherTrx(trxId As Integer, lockId As String) As IHttpActionResult

      If trxId <= 0 Then
         Throw New ArgumentException("Transaction ID is required.")
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

            Me.DeleteFinTrx(trxId, lockId)

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

   Private Sub LoadApsFinTrx(trx As PayableVoucherTrxBody, finTrx As ApsFinTrx)

      DataLib.ScatterValues(trx, finTrx)

   End Sub

   Private Sub LoadApsFinTrx(row As DataRow, trx As ApsFinTrx)

      With trx
         .TrxId = row.ToInt32("TrxId")
         .TrxDate = row.ToDate("TrxDate")

         .TrxStatusId = row.ToInt32("TrxStatusId")
         .DocumentId = row.ToString("DocumentId")
         '.CostCenterId = row.ToInt32("CostCenterId")
         .Reference = row.ToString("Reference")
         .Particulars = row.ToString("Particulars")
         .BankId = row.ToInt32("BankId")
         .CashReceiptTypeId = row.ToInt32("CashReceiptTypeId")
         .TrxTypeId = row.ToInt32("TrxTypeId")
         .Remarks = row.ToString("Remarks")
         .PostUserId = row.ToInt32("PostUserId")


      End With

   End Sub

   Private Sub LoadFinTrxDetailList(rows As DataRowCollection, list As FinTrxDetailList)

      Dim _detail As FinTrxDetail
      For Each _row As DataRow In rows
         _detail = New FinTrxDetail

         With _detail
            .TrxDetailId = _row.ToInt32("TrxDetailId")
            .TrxId = _row.ToInt32("TrxId")
            .AccountId = _row.ToString("AccountId")
            .DebitAmount = _row.ToDecimal("DebitAmount")
            .CreditAmount = _row.ToDecimal("CreditAmount")

            .OrgId = _row.ToInt32("OrgId")
            .PlatformId = _row.ToInt32("PlatformId")
            .ClusterId = _row.ToInt32("ClusterId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .PayeeId = _row.ToInt32("PayeeId")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertFinTrx(trx As ApsFinTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
         .Add("PayeeId")
         .Add("PayableTypeId")
         .Add("PayableTaxCode")
         .Add("ATaxCode")
         .Add("DueDate")
         .Add("CheckDate")
         .Add("Amount")
         .Add("ApsLockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTrx", trx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsFinTrx(trx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFinTrxDetails(list As FinTrxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TrxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("Details")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTrxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinTrxDetail In list
            Me.AddInsertUpdateParamsFinTrxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertApsTrx(aps As ApsTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")

      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsTrx", aps, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsTrx(aps)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFinTrx(trx As ApsFinTrx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TrxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
         .Add("PayeeId")
         .Add("PayableTypeId")
         .Add("PayableTaxCode")
         .Add("ATaxCode")
         .Add("DueDate")
         .Add("CheckDate")
         .Add("Amount")
         .Add("ApsLockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinTrx", trx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsFinTrx(trx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(trx.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFinTrxDetails(list As FinTrxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TrxDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("DisbursementTypeId")
         .Add("Details")

         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinTrxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinTrxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFinTrxDetail(_detail)
               .Parameters.AddWithValue("@TrxDetailId", _detail.TrxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateApsTrx(trx As ApsTrx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TrxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")

      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsTrx", trx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsTrx(trx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(trx.LockId)).DbType = DbType.Binary
         '.Parameters.AddWithValue("@TrxId", trx.TrxId)
         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsFinTrx(trx As ApsFinTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TrxId", trx.TrxId)
         .AddWithValue("@TrxDate", trx.TrxDate)
         .AddWithValue("@TrxStatusId", trx.TrxStatusId)
         .AddWithValue("@DocumentId", trx.DocumentId)
         .AddWithValue("@Reference", trx.Reference.ToNullable)
         .AddWithValue("@Particulars", trx.Particulars.ToNullable)
         .AddWithValue("@Remarks", trx.Remarks.ToNullable)
         .AddWithValue("@CashReceiptTypeId", trx.CashReceiptTypeId.ToNullable)
         .AddWithValue("@TrxTypeId", trx.TrxTypeId)
         .AddWithValue("@BankId", trx.BankId.ToNullable)
         .AddWithValue("@DisbursementTypeId", trx.DisbursementTypeId.ToNullable)
         .AddWithValue("@PostUserId", trx.PostUserId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinTrxDetail(dtl As FinTrxDetail)

      With DataCore.Command.Parameters
         .Clear()

         '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
         .AddWithValue("@TrxId", dtl.TrxId)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@DebitAmount", dtl.DebitAmount)
         .AddWithValue("@CreditAmount", dtl.CreditAmount)

         .AddWithValue("@OrgId", dtl.OrgId)
         .AddWithValue("@PlatformId", dtl.PlatformId)
         .AddWithValue("@ClusterId", dtl.ClusterId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@PayeeId", dtl.PayeeId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsTrx(aps As ApsTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TrxId", aps.TrxId)
         .AddWithValue("@PayeeId", aps.PayeeId)
         .AddWithValue("@PayableTypeId", aps.PayableTypeId)
         .AddWithValue("@PayableTaxCode", aps.PayableTaxCode.ToNullable)
         .AddWithValue("@ATaxCode", aps.ATaxCode.ToNullable)
         .AddWithValue("@Details", aps.Details.ToNullable)
         .AddWithValue("@DueDate", aps.DueDate.ToNullable)
         .AddWithValue("@CheckDate", aps.CheckDate.ToNullable)
         .AddWithValue("@Amount", aps.Amount)
      End With

   End Sub

   Private Sub DeleteFinTrx(trxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TrxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinTrx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@TrxId", trxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFinTrxDetails(list As FinTrxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FinTrxDetail WHERE TrxDetailId=@TrxDetailId"
         .CommandType = CommandType.Text

         For Each _old As FinTrxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TrxDetailId", _old.TrxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasFinTrxChanges(oldRecord As ApsFinTrx, newRecord As ApsFinTrx) As Boolean

      With oldRecord
         If .TrxDate <> newRecord.TrxDate Then Return True
         'If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .TrxStatusId <> newRecord.TrxStatusId Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         'If .CostCenterId <> newRecord.CostCenterId Then Return True
         If .Reference <> newRecord.Reference Then Return True
         If .Particulars <> newRecord.Particulars Then Return True
         If .Remarks <> newRecord.Remarks Then Return True
         'If .RequestSignatoryId <> newRecord.RequestSignatoryId Then Return True
      End With

      Return False

   End Function

   Private Function HasApsTrxChanges(oldRecord As ApsTrx, newRecord As ApsTrx) As Boolean

      With oldRecord
         If .PayeeId <> newRecord.PayeeId Then Return True
         'If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .PayableTypeId <> newRecord.PayableTypeId Then Return True
         If .PayableTaxCode <> newRecord.PayableTaxCode Then Return True
         'If .CostCenterId <> newRecord.CostCenterId Then Return True
         If .Amount <> newRecord.Amount Then Return True
         'If .RequestSignatoryId <> newRecord.RequestSignatoryId Then Return True
      End With

      Return False

   End Function

End Class
Public Class PayableVoucherTrxBody
   Inherits ApsFinTrx
   Public Property Details As FinTrxDetail()
   'Public Property PayableVoucher As ApsTrx()

End Class
