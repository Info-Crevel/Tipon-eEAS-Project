<RoutePrefix("api")>
Public Class BGS0040_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_BGS0040")>
   <Route("references/bgs0040")>
   <HttpGet>
   Public Function GetReferences_BGS0040() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.BGS0040_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "transferTypes"
                     .Tables(1).TableName = "fundSources"
                     .Tables(2).TableName = "signatories"      ' all defined signatories
                     .Tables(3).TableName = "costCenters"      ' all defined cost centers
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetBgsTransfer")>
   <Route("transfers/{transferId}")>
   <HttpGet>
   Public Function GetBgsTransfer(transferId As Integer) As IHttpActionResult

      If transferId <= 0 Then
         Throw New ArgumentException("Transfer ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0040")
            With _direct
               .AddParameter("TransferId", transferId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "transfer"
                     .Tables(1).TableName = "transferSources"
                     .Tables(2).TableName = "transferTargets"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateBgsTransfer")>
   <Route("transfers/{currentUserId}")>
   <HttpPost>
   Public Function CreateBgsTransfer(currentUserId As Integer, <FromBody> xfr As BgsTransferBody) As IHttpActionResult

      If xfr.TransferId <> -1 Then
         Throw New ArgumentException("Transfer ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _transferId As Integer = SysLib.GetNextSequence("TransferId")

         xfr.TransferId = _transferId

         '
         ' Load proposed values from payload
         '
         Dim _bgsTransfer As New BgsTransfer
         Dim _bgsTransferSourceList As New BgsTransferSourceList
         Dim _bgsTransferTargetList As New BgsTransferTargetList

         Me.LoadBgsTransfer(xfr, _bgsTransfer)

         For Each _source As BgsTransferSource In xfr.Sources
            _source.TransferId = _transferId
            _bgsTransferSourceList.Add(_source)
         Next

         For Each _target As BgsTransferTarget In xfr.Targets
            _target.TransferId = _transferId
            _bgsTransferTargetList.Add(_target)
         Next

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertBgsTransfer(_bgsTransfer)
            Me.InsertBgsTransferSources(_bgsTransferSourceList)
            Me.InsertBgsTransferTargets(_bgsTransferTargetList)

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
         Return Me.Ok(xfr.TransferId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyBgsTransfer")>
   <Route("transfers/{transferId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyBgsTransfer(transferId As Integer, currentUserId As Integer, <FromBody> xfr As BgsTransferBody) As IHttpActionResult

      If transferId <= 0 Then
         Throw New ArgumentException("Transfer ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsTransfer As New BgsTransfer
         Dim _bgsTransferSourceList As New BgsTransferSourceList
         Dim _bgsTransferTargetList As New BgsTransferTargetList

         Me.LoadBgsTransfer(xfr, _bgsTransfer)

         For Each _source As BgsTransferSource In xfr.Sources
            _bgsTransferSourceList.Add(_source)
         Next

         For Each _target As BgsTransferTarget In xfr.Targets
            _bgsTransferTargetList.Add(_target)
         Next

         '
         ' Load old values from DB
         '
         Dim _bgsTransferOld As New BgsTransfer
         Dim _bgsTransferSourceListOld As New BgsTransferSourceList
         Dim _bgsTransferTargetListOld As New BgsTransferTargetList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetBgsTransfer(transferId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("transfer").Rows(0)
            Me.LoadBgsTransfer(_row, _bgsTransferOld)
            Me.LoadBgsTransferSourceList(_dataSet.Tables("transferSources").Rows, _bgsTransferSourceListOld)
            Me.LoadBgsTransferTargetList(_dataSet.Tables("transferTargets").Rows, _bgsTransferTargetListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         '
         ' BgsObx
         '

         'Dim _bgsObxLogDetailList As New SysLogDetailList

         'With _bgsObxOld
         '   If .ObxDate <> _bgsObx.ObxDate Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PostDate, .ObxDate.ToDisplayFormat, _bgsObx.ObxDate.ToDisplayFormat)
         '   End If

         '   If .FundClusterId <> _bgsObx.FundClusterId Then
         '      Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
         '      Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _bgsObx.FundClusterId).FundClusterName

         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _bgsObx.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _bgsObx.FundClusterId + "=" + _newFundClusterName)
         '   End If

         '   If .DocumentId <> _bgsObx.DocumentId Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.DocumentId, .DocumentId, _bgsObx.DocumentId)
         '   End If

         '   If .PayeeId <> _bgsObx.PayeeId Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeId, .PayeeId.ToString + "=" + .PayeeName, _bgsObx.PayeeId.ToString + "=" + _bgsObx.PayeeName)
         '   End If

         '   'If .PayeeName <> _bgsObx.PayeeName Then
         '   '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeName, .PayeeName, _bgsObx.PayeeName)
         '   'End If

         '   'If .PayeeOffice <> _bgsObx.PayeeOffice Then
         '   '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeOffice, .PayeeOffice, _bgsObx.PayeeOffice)
         '   'End If

         '   'If .PayeeAddress <> _bgsObx.PayeeAddress Then
         '   '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeAddress, .PayeeAddress, _bgsObx.PayeeAddress)
         '   'End If

         '   If .Particulars <> _bgsObx.Particulars Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _bgsObx.Particulars)
         '   End If

         '   If .Remarks <> _bgsObx.Remarks Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Remarks, .Remarks, _bgsObx.Remarks)
         '   End If

         '   If .ApprovedFlag <> _bgsObx.ApprovedFlag Then
         '      AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.ApprovedFlag, .ApprovedFlag.ToString, _bgsObx.ApprovedFlag.ToString)
         '   End If

         'End With

         '
         ' BgsTransferSource
         '

         'Dim _bgsObxDetailLogDetailList As New SysLogDetailList

         Dim _removeSourceCount As Integer
         Dim _addSourceCount As Integer
         Dim _editSourceCount As Integer

         ' Mark sources for deletion if not found in new list

         For Each _old As BgsTransferSource In _bgsTransferSourceListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As BgsTransferSource In _bgsTransferSourceList
               If _new.TransferSourceId = _old.TransferSourceId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeSourceCount = _removeSourceCount + 1
            End If
         Next

         ' Mark sources for addition if not found in old list;
         ' Mark sources for modification if found in old list but with at least 1 property mismatch

         For Each _new As BgsTransferSource In _bgsTransferSourceList
            _new.LogActionId = LogActionId.Add
            For Each _old As BgsTransferSource In _bgsTransferSourceListOld
               If _new.TransferSourceId = _old.TransferSourceId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .AllotActId <> _old.AllotActId OrElse .CostCenterId <> _old.CostCenterId OrElse .AccountId <> _old.AccountId OrElse .Amount <> _old.Amount Then
                        .LogActionId = LogActionId.Edit
                     End If

                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addSourceCount = _addSourceCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editSourceCount = _editSourceCount + 1
            End If

         Next

         Dim _bgsTransferSourceListNew As New BgsTransferSourceList      ' for adding new Transfer Sources

         If _addSourceCount > 0 Then
            Dim _bgsTransferSource As BgsTransferSource

            For Each _new As BgsTransferSource In _bgsTransferSourceList
               If _new.LogActionId = LogActionId.Add Then
                  _bgsTransferSource = New BgsTransferSource
                  _bgsTransferSourceListNew.Add(_bgsTransferSource)
                  DataLib.ScatterValues(_new, _bgsTransferSource)
                  _bgsTransferSource.TransferId = _bgsTransfer.TransferId
               End If
            Next

         End If


         '
         ' BgsTransferTarget
         '

         'Dim _bgsObxVoucherLogDetailList As New SysLogDetailList

         Dim _removeTargetCount As Integer
         Dim _addTargetCount As Integer
         Dim _editTargetCount As Integer

         ' Mark targets for deletion if not found in new list

         For Each _old As BgsTransferTarget In _bgsTransferTargetListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As BgsTransferTarget In _bgsTransferTargetList
               If _new.TransferTargetId = _old.TransferTargetId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeTargetCount = _removeTargetCount + 1
            End If
         Next

         ' Mark targets for addition if not found in old list;
         ' Mark targets for modification if found in old list but with at least 1 property mismatch

         For Each _new As BgsTransferTarget In _bgsTransferTargetList
            _new.LogActionId = LogActionId.Add
            For Each _old As BgsTransferTarget In _bgsTransferTargetListOld
               If _new.TransferTargetId = _old.TransferTargetId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .AllotActId <> _old.AllotActId OrElse .CostCenterId <> _old.CostCenterId OrElse .AccountId <> _old.AccountId OrElse .Amount <> _old.Amount Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addTargetCount = _addTargetCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editTargetCount = _editTargetCount + 1
            End If

         Next

         Dim _bgsTransferTargetListNew As New BgsTransferTargetList      ' for adding new Transfer Targets

         If _addTargetCount > 0 Then
            Dim _bgsTransferTarget As BgsTransferTarget

            For Each _new As BgsTransferTarget In _bgsTransferTargetList
               If _new.LogActionId = LogActionId.Add Then
                  _bgsTransferTarget = New BgsTransferTarget
                  _bgsTransferTargetListNew.Add(_bgsTransferTarget)
                  DataLib.ScatterValues(_new, _bgsTransferTarget)
                  _bgsTransferTarget.TransferId = _bgsTransfer.TransferId
               End If
            Next

         End If

         Dim _isBgsTransferChanged As Boolean = Me.HasBgsTransferChanges(_bgsTransferOld, _bgsTransfer)

         If Not _isBgsTransferChanged AndAlso _addSourceCount = 0 AndAlso _removeSourceCount = 0 AndAlso _editSourceCount = 0 AndAlso _addTargetCount = 0 AndAlso _removeTargetCount = 0 AndAlso _editTargetCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetBgsTransfer(transferId)
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

            If _isBgsTransferChanged Then
               Me.UpdateBgsTransfer(_bgsTransfer)

               'With _logKeyList
               '   .Clear()
               '   .Add("ObxId", _bgsObx.ObxId)
               'End With

               '_id = AppLib.CreateLogHeader("InsBgsObxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               'AppLib.AssignLogHeaderId(_id, _bgsObxLogDetailList)

               'AppLib.CreateLogDetails(_bgsObxLogDetailList, "BgsObxLogDetail")

            End If

            '
            ' BgsTransferSource
            '
            If _removeSourceCount > 0 Then
               Me.DeleteBgsTransferSources(_bgsTransferSourceListOld)

               'For Each _old As BgsObxDetail In _bgsObxDetailListOld
               '   If _old.LogActionId = LogActionId.Delete Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("ObxId", _old.ObxId)
               '         .Add("AccountId", _old.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

               '      _bgsObxDetailLogDetailList.Clear()

               '      With _old
               '         AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

               '         AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "Account ID=" + .AccountId)

               '      End With

               '      AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

               '   End If
               'Next

            End If

            If _addSourceCount > 0 Then
               Me.InsertBgsTransferSources(_bgsTransferSourceListNew)

               'For Each _new As BgsObxDetail In _bgsObxDetailListNew

               '   With _logKeyList
               '      .Clear()
               '      .Add("ObxId", _new.ObxId)
               '      .Add("AccountId", _new.AccountId)
               '   End With

               '   _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               '   _bgsObxDetailLogDetailList.Clear()

               '   With _new
               '      Dim _costCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
               '      AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.CostCenterId, "", .CostCenterId.ToString, .CostCenterId.ToString + "=" + _costCenter)

               '      'AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Particulars, "", .Particulars)

               '      AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.ActivityId, "", .ActivityId.ToString, .ActivityId.ToString + "=" + .ActivityShortName)

               '      Dim _budgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
               '      AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.BudgetClassId, "", .BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _budgetClass)

               '      AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

               '      AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "AccountId=" + .AccountId)

               '      AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

               '   End With
               'Next

            End If

            If _editSourceCount > 0 Then
               Me.UpdateBgsTransferSources(_bgsTransferSourceList)

               'For Each _new As BgsObxDetail In _bgsObxDetailList
               '   If _new.LogActionId = LogActionId.Edit Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("ObxId", _new.ObxId)
               '         .Add("AccountId", _new.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

               '      _bgsObxDetailLogDetailList.Clear()

               '      For Each _old As BgsObxDetail In _bgsObxDetailListOld
               '         If _new.ObxDetailId = _old.ObxDetailId Then
               '            With _new
               '               If .CostCenterId <> _old.CostCenterId Then
               '                  Dim _oldCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = _old.CostCenterId).CostCenterShortName
               '                  Dim _newCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName

               '                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.CostCenterId, _old.CostCenterId.ToString, .CostCenterId.ToString, _old.CostCenterId.ToString + "=" + _oldCostCenter + "; " + .CostCenterId.ToString + "=" + _newCostCenter)
               '               End If

               '               'If .Particulars <> _old.Particulars Then
               '               '   AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Particulars, _old.Particulars, .Particulars)
               '               'End If

               '               If .ActivityId <> _old.ActivityId Then
               '                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.ActivityId, _old.ActivityId.ToString, .ActivityId.ToString, _old.ActivityId.ToString + "=" + _old.ActivityShortName + "; " + .ActivityId.ToString + "=" + .ActivityShortName)
               '               End If

               '               If .BudgetClassId <> _old.BudgetClassId Then
               '                  Dim _oldBudgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = _old.BudgetClassId).BudgetClassName
               '                  Dim _newBudgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName

               '                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.BudgetClassId, _old.BudgetClassId.ToString, .BudgetClassId.ToString, _old.BudgetClassId.ToString + "=" + _oldBudgetClass + "; " + .BudgetClassId.ToString + "=" + _newBudgetClass)
               '               End If

               '               If .AccountId <> _old.AccountId Then
               '                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId)
               '               End If

               '               If .Amount <> _old.Amount Then
               '                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "Account ID=" + .AccountId)
               '               End If

               '               AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

               '            End With

               '         End If
               '      Next

               '   End If

               'Next

            End If

            '
            ' BgsTransferTarget
            '
            If _removeTargetCount > 0 Then
               Me.DeleteBgsTransferTargets(_bgsTransferTargetListOld)

               'For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
               '   If _old.LogActionId = LogActionId.Delete Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("ObxId", _old.ObxId)
               '         .Add("VoucherId", _old.VoucherId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Delete, _currentUserId)

               '      _bgsObxVoucherLogDetailList.Clear()

               '      With _old
               '         AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, .VoucherId, "")

               '         AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, .TotalAmount.ToString("N"), "", "VoucherId=" + .VoucherId)

               '         If .TaxAmount > 0 Then
               '            AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, .TaxAmount.ToString("N"), "", "VoucherId=" + .VoucherId)
               '         End If

               '      End With

               '      AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

               '   End If
               'Next

            End If

            If _addTargetCount > 0 Then
               Me.InsertBgsTransferTargets(_bgsTransferTargetListNew)

               'For Each _new As BgsObxVoucher In _bgsObxVoucherListNew

               '   With _logKeyList
               '      .Clear()
               '      .Add("ObxId", _new.ObxId)
               '      .Add("VoucherId", _new.VoucherId)
               '   End With

               '   _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Add, _currentUserId)

               '   _bgsObxVoucherLogDetailList.Clear()

               '   With _new
               '      AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, "", .VoucherId)

               '      AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, "", .TotalAmount.ToString("N"), "VoucherId=" + .VoucherId)

               '      If .TaxAmount > 0 Then
               '         AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, "", .TaxAmount.ToString("N"), "VoucherId=" + .VoucherId)
               '      End If

               '      AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

               '   End With
               'Next

            End If

            If _editTargetCount > 0 Then
               Me.UpdateBgsTransferTargets(_bgsTransferTargetList)

               'For Each _new As BgsObxVoucher In _bgsObxVoucherList
               '   If _new.LogActionId = LogActionId.Edit Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("ObxId", _new.ObxId)
               '         .Add("VoucherId", _new.VoucherId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Edit, _currentUserId)

               '      _bgsObxVoucherLogDetailList.Clear()

               '      For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
               '         If _new.ObxVoucherId = _old.ObxVoucherId Then
               '            With _new
               '               If .VoucherId <> _old.VoucherId Then
               '                  AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, _old.VoucherId, .VoucherId)
               '               End If

               '               If .TotalAmount <> _old.TotalAmount Then
               '                  AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, _old.TotalAmount.ToString("N"), .TotalAmount.ToString("N"), "VoucherId=" + .VoucherId)
               '               End If

               '               If .TaxAmount <> _old.TaxAmount Then
               '                  AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, _old.TaxAmount.ToString("N"), .TaxAmount.ToString("N"), "VoucherId=" + .VoucherId)
               '               End If

               '               AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

               '            End With

               '         End If
               '      Next

               '   End If

               'Next

            End If

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

         Return Me.Ok(True)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveBgsTransfer")>
   <Route("transfers/{transferId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveBgsTransfer(transferId As Integer, lockId As String) As IHttpActionResult

      If transferId <= 0 Then
         Throw New ArgumentException("Transfer ID is required.")
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

            Me.DeleteBgsTransfer(transferId, lockId)

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

   Private Sub LoadBgsTransfer(xfr As BgsTransferBody, bgsTransfer As BgsTransfer)

      DataLib.ScatterValues(xfr, bgsTransfer)

   End Sub

   Private Sub LoadBgsTransfer(row As DataRow, xfr As BgsTransfer)

      With xfr
         .TransferId = row.ToInt32("TransferId")
         .TransferDate = row.ToDate("TransferDate")
         .DocumentId = row.ToString("DocumentId")
         .TransferTypeId = row.ToInt32("TransferTypeId")
         .FundSourceId = row.ToInt32("FundSourceId")
         .LegalBasis = row.ToString("LegalBasis")
         .PrepareSignatoryId = row.ToInt32("PrepareSignatoryId")
         .RecommendSignatoryId = row.ToInt32("RecommendSignatoryId")
         .SignatoryId = row.ToInt32("SignatoryId")
         '.SourceAllotActId = row.ToInt32("SourceAllotActId")
         '.TargetAllotActId = row.ToInt32("TargetAllotActId")
         .PostUserId = row.ToInt32("PostUserId")
      End With

   End Sub

   Private Sub LoadBgsTransferSourceList(rows As DataRowCollection, list As BgsTransferSourceList)

      Dim _source As BgsTransferSource
      For Each _row As DataRow In rows
         _source = New BgsTransferSource

         With _source
            .TransferSourceId = _row.ToInt32("TransferSourceId")
            .TransferId = _row.ToInt32("TransferId")
            .AllotActId = _row.ToInt32("AllotActId")
            .ActivityName = _row.ToString("ActivityName")
            .CostCenterId = _row.ToInt32("CostCenterId")
            .CostCenterName = _row.ToString("CostCenterName")
            .AccountId = _row.ToString("AccountId")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_source)

      Next

   End Sub

   Private Sub LoadBgsTransferTargetList(rows As DataRowCollection, list As BgsTransferTargetList)

      Dim _target As BgsTransferTarget
      For Each _row As DataRow In rows
         _target = New BgsTransferTarget

         With _target
            .TransferTargetId = _row.ToInt32("TransferTargetId")
            .TransferId = _row.ToInt32("TransferId")
            .AllotActId = _row.ToInt32("AllotActId")
            .ActivityName = _row.ToString("ActivityName")
            .CostCenterId = _row.ToInt32("CostCenterId")
            .CostCenterName = _row.ToString("CostCenterName")
            .AccountId = _row.ToString("AccountId")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_target)

      Next

   End Sub

   Private Sub InsertBgsTransfer(xfr As BgsTransfer)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsTransfer", xfr, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsTransfer(xfr)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertBgsTransferSources(list As BgsTransferSourceList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TransferSourceId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("ActivityName")
         .Add("CostCenterName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsTransferSource", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _source As BgsTransferSource In list
            Me.AddInsertUpdateParamsBgsTransferSource(_source)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub InsertBgsTransferTargets(list As BgsTransferTargetList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TransferTargetId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("ActivityName")
         .Add("CostCenterName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsTransferTarget", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _target As BgsTransferTarget In list
            Me.AddInsertUpdateParamsBgsTransferTarget(_target)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateBgsTransfer(xfr As BgsTransfer)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TransferId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsTransfer", xfr, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsTransfer(xfr)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(xfr.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsTransferSources(list As BgsTransferSourceList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TransferSourceId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
         .Add("ActivityName")
         .Add("CostCenterName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsTransferSource", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _source As BgsTransferSource In list
            If _source.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsTransferSource(_source)
               .Parameters.AddWithValue("@TransferSourceId", _source.TransferSourceId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub UpdateBgsTransferTargets(list As BgsTransferTargetList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TransferTargetId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
         .Add("ActivityName")
         .Add("CostCenterName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsTransferTarget", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _target As BgsTransferTarget In list
            If _target.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsTransferTarget(_target)
               .Parameters.AddWithValue("@TransferTargetId", _target.TransferTargetId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsTransfer(xfr As BgsTransfer)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TransferId", xfr.TransferId)
         .AddWithValue("@TransferDate", xfr.TransferDate)
         .AddWithValue("@DocumentId", xfr.DocumentId)
         .AddWithValue("@TransferTypeId", xfr.TransferTypeId)
         .AddWithValue("@FundSourceId", xfr.FundSourceId)
         .AddWithValue("@LegalBasis", xfr.LegalBasis)
         .AddWithValue("@PrepareSignatoryId", xfr.PrepareSignatoryId)
         .AddWithValue("@RecommendSignatoryId", xfr.RecommendSignatoryId)
         .AddWithValue("@SignatoryId", xfr.SignatoryId)
         .AddWithValue("@PostUserId", xfr.PostUserId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsTransferSource(src As BgsTransferSource)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TransferId", src.TransferId)
         .AddWithValue("@AllotActId", src.AllotActId)
         .AddWithValue("@CostCenterId", src.CostCenterId)
         .AddWithValue("@AccountId", src.AccountId)
         .AddWithValue("@Amount", src.Amount)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsTransferTarget(tgt As BgsTransferTarget)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TransferId", tgt.TransferId)
         .AddWithValue("@AllotActId", tgt.AllotActId)
         .AddWithValue("@CostCenterId", tgt.CostCenterId)
         .AddWithValue("@AccountId", tgt.AccountId)
         .AddWithValue("@Amount", tgt.Amount)
      End With

   End Sub

   Private Sub DeleteBgsTransfer(transferId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TransferId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsTransfer", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@TransferId", transferId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteBgsTransferSources(list As BgsTransferSourceList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsTransferSource WHERE TransferSourceId=@TransferSourceId"
         .CommandType = CommandType.Text

         For Each _old As BgsTransferSource In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TransferSourceId", _old.TransferSourceId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub DeleteBgsTransferTargets(list As BgsTransferTargetList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsTransferTarget WHERE TransferTargetId=@TransferTargetId"
         .CommandType = CommandType.Text

         For Each _old As BgsTransferTarget In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TransferTargetId", _old.TransferTargetId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasBgsTransferChanges(oldRecord As BgsTransfer, newRecord As BgsTransfer) As Boolean

      With oldRecord
         If .TransferDate <> newRecord.TransferDate Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .TransferTypeId <> newRecord.TransferTypeId Then Return True
         If .FundSourceId <> newRecord.FundSourceId Then Return True
         If .LegalBasis <> newRecord.LegalBasis Then Return True
         If .PrepareSignatoryId <> newRecord.PrepareSignatoryId Then Return True
         If .RecommendSignatoryId <> newRecord.RecommendSignatoryId Then Return True
         If .SignatoryId <> newRecord.SignatoryId Then Return True

      End With

      Return False

   End Function

End Class

Public Class BgsTransferBody
   Inherits BgsTransfer

   Public Property Sources As BgsTransferSource()
   Public Property Targets As BgsTransferTarget()

End Class