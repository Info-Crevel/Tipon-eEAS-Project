<RoutePrefix("api")>
Public Class BGS0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_BGS0030")>
   <Route("references/bgs0030")>
   <HttpGet>
   Public Function GetReferences_BGS0030() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.BGS0030_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "costCenters"      ' all defined cost centers
                     .Tables(2).TableName = "budgetClasses"    ' 1=Annual, 2=Continuing, 3=Supplemental, 4=Automatic, 5=Unprogrammed
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetBgsObx")>
   <Route("obligations/{obxId}")>
   <HttpGet>
   Public Function GetBgsObx(obxId As Integer) As IHttpActionResult

      If obxId <= 0 Then
         Throw New ArgumentException("Obligation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0030")
            With _direct
               .AddParameter("ObxId", obxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "obx"
                     .Tables(1).TableName = "obxDetails"
                     .Tables(2).TableName = "obxVouchers"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetBgsObxLog")>
   <Route("obligations/{obxId}/log")>
   <HttpGet>
   Public Function GetBgsObxLog(obxId As Integer) As IHttpActionResult

      If obxId <= 0 Then
         Throw New ArgumentException("Obligation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0030_Log")
            With _direct
               .AddParameter("ObxId", obxId)

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

   <SymAuthorization("CreateBgsObx")>
   <Route("obligations/{currentUserId}")>
   <HttpPost>
   Public Function CreateBgsObx(currentUserId As Integer, <FromBody> obx As BgsObxBody) As IHttpActionResult

      If obx.ObxId <> -1 Then
         Throw New ArgumentException("Obligation ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _obxId As Integer = SysLib.GetNextSequence("BgsObxId")

         obx.ObxId = _obxId

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If obx.DocumentId = "< NEW >" Then
            obx.DocumentId = AppLib.GetNextDocSequence(DocSequencerId.ORS, obx.ObxDate)
         End If

         '
         ' Assign next NORSA sequence if new adjustment / auto-set status to Approved
         '
         If obx.AdjustmentId = "< NEW >" Then
            obx.AdjustmentId = AppLib.GetNextDocSequence(DocSequencerId.NORSA, obx.ObxDate)
            obx.ObxStatusId = 2     ' Approved
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsObx As New BgsObx
         Dim _bgsObxDetailList As New BgsObxDetailList
         Dim _bgsObxVoucherList As New BgsObxVoucherList

         Me.LoadBgsObx(obx, _bgsObx)

         For Each _detail As BgsObxDetail In obx.Details
            _detail.ObxId = _obxId
            _bgsObxDetailList.Add(_detail)
         Next

         For Each _voucher As BgsObxVoucher In obx.Vouchers
            _voucher.ObxId = _obxId
            _bgsObxVoucherList.Add(_voucher)
         Next

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsObxLogDetailList As New SysLogDetailList
         Dim _bgsObxDetailLogDetailList As New SysLogDetailList
         Dim _bgsObxVoucherLogDetailList As New SysLogDetailList

         With _bgsObx
            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.ObxDate, String.Empty, .ObxDate.ToDisplayFormat)

            Dim _fundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.FundClusterId, String.Empty, .FundClusterId, .FundClusterId + "=" + _fundClusterName)

            Dim _statusName As String = FrsSession.DbsObxStatus.Rows.Find(Function(m) m.ObxStatusId = .ObxStatusId).ObxStatusName
            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.ObxStatusId, String.Empty, .ObxStatusId.ToString, .ObxStatusId.ToString + "=" + _statusName)

            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.DocumentId, String.Empty, .DocumentId)

            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeId, String.Empty, .PayeeId.ToString + "=" + .PayeeName)

            'AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeName, String.Empty, .PayeeName)

            'If Not String.IsNullOrEmpty(.PayeeOffice) Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeOffice, String.Empty, .PayeeOffice)
            'End If

            'If Not String.IsNullOrEmpty(.PayeeAddress) Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeAddress, String.Empty, .PayeeAddress)
            'End If

            AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

            If Not String.IsNullOrEmpty(.Remarks) Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Remarks, String.Empty, .Remarks)
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

            Me.InsertBgsObx(_bgsObx)
            Me.InsertBgsObxDetails(_bgsObxDetailList)

            If _bgsObxVoucherList.Count > 0 Then
               Me.InsertBgsObxVouchers(_bgsObxVoucherList)
            End If

            If _bgsObxLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ObxId", _bgsObx.ObxId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsObxLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsObxLogDetailList)
               AppLib.CreateLogDetails(_bgsObxLogDetailList, "BgsObxLogDetail")

            End If

            For Each _new As BgsObxDetail In _bgsObxDetailList

               With _logKeyList
                  .Clear()
                  .Add("ObxId", _new.ObxId)
                  .Add("AccountId", _new.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               _bgsObxDetailLogDetailList.Clear()

               With _new
                  Dim _costCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.CostCenterId, "", .CostCenterId.ToString, .CostCenterId.ToString + "=" + _costCenter)

                  'AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Particulars, "", .Particulars)

                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.ActivityId, "", .ActivityId.ToString, .ActivityId.ToString + "=" + .ActivityShortName)

                  Dim _budgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.BudgetClassId, "", .BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _budgetClass)

                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

                  AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "Account ID=" + .AccountId)

                  AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

               End With

            Next

            For Each _new As BgsObxVoucher In _bgsObxVoucherList

               With _logKeyList
                  .Clear()
                  .Add("ObxId", _new.ObxId)
                  .Add("VoucherId", _new.VoucherId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Add, _currentUserId)

               _bgsObxVoucherLogDetailList.Clear()

               With _new
                  AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, "", .VoucherId)

                  AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, "", .TotalAmount.ToString("N"), "VoucherId=" + .VoucherId)

                  If .TaxAmount > 0 Then
                     AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, "", .TaxAmount.ToString("N"), "VoucherId=" + .VoucherId)
                  End If

                  AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

               End With

            Next

            _successFlag = True

         Catch _exception As Exception
            'File.WriteAllText("d:\yyy1.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
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
         Return Me.Ok(obx.ObxId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz1.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyBgsObx")>
   <Route("obligations/{obxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyBgsObx(obxId As Integer, currentUserId As Integer, <FromBody> obx As BgsObxBody) As IHttpActionResult

      If obxId <= 0 Then
         Throw New ArgumentException("Obligation ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsObx As New BgsObx
         Dim _bgsObxDetailList As New BgsObxDetailList
         Dim _bgsObxVoucherList As New BgsObxVoucherList

         Me.LoadBgsObx(obx, _bgsObx)

         For Each _detail As BgsObxDetail In obx.Details
            _bgsObxDetailList.Add(_detail)
         Next

         For Each _voucher As BgsObxVoucher In obx.Vouchers
            _bgsObxVoucherList.Add(_voucher)
         Next

         '
         ' Load old values from DB
         '
         Dim _bgsObxOld As New BgsObx
         Dim _bgsObxDetailListOld As New BgsObxDetailList
         Dim _bgsObxVoucherListOld As New BgsObxVoucherList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetBgsObx(obxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("obx").Rows(0)
            Me.LoadBgsObx(_row, _bgsObxOld)
            Me.LoadBgsObxDetailList(_dataSet.Tables("obxDetails").Rows, _bgsObxDetailListOld)
            Me.LoadBgsObxVoucherList(_dataSet.Tables("obxVouchers").Rows, _bgsObxVoucherListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' BgsObx
         '

         Dim _bgsObxLogDetailList As New SysLogDetailList

         With _bgsObxOld
            If .ObxDate <> _bgsObx.ObxDate Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PostDate, .ObxDate.ToDisplayFormat, _bgsObx.ObxDate.ToDisplayFormat)
            End If

            If .FundClusterId <> _bgsObx.FundClusterId Then
               Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
               Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _bgsObx.FundClusterId).FundClusterName

               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _bgsObx.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _bgsObx.FundClusterId + "=" + _newFundClusterName)
            End If

            If .ObxStatusId <> _bgsObx.ObxStatusId Then
               Dim _oldStatusName As String = FrsSession.DbsObxStatus.Rows.Find(Function(m) m.ObxStatusId = .ObxStatusId).ObxStatusName
               Dim _newStatusName As String = FrsSession.DbsObxStatus.Rows.Find(Function(m) m.ObxStatusId = _bgsObx.ObxStatusId).ObxStatusName

               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.ObxStatusId, .ObxStatusId.ToString, _bgsObx.ObxStatusId.ToString, .ObxStatusId.ToString + "=" + _oldStatusName + "; " + _bgsObx.ObxStatusId.ToString + "=" + _newStatusName)
            End If

            If .DocumentId <> _bgsObx.DocumentId Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.DocumentId, .DocumentId, _bgsObx.DocumentId)
            End If

            If .PayeeId <> _bgsObx.PayeeId Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeId, .PayeeId.ToString + "=" + .PayeeName, _bgsObx.PayeeId.ToString + "=" + _bgsObx.PayeeName)
            End If

            'If .PayeeName <> _bgsObx.PayeeName Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeName, .PayeeName, _bgsObx.PayeeName)
            'End If

            'If .PayeeOffice <> _bgsObx.PayeeOffice Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeOffice, .PayeeOffice, _bgsObx.PayeeOffice)
            'End If

            'If .PayeeAddress <> _bgsObx.PayeeAddress Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.PayeeAddress, .PayeeAddress, _bgsObx.PayeeAddress)
            'End If

            If .Particulars <> _bgsObx.Particulars Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _bgsObx.Particulars)
            End If

            If .Remarks <> _bgsObx.Remarks Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.Remarks, .Remarks, _bgsObx.Remarks)
            End If

            'If .ApprovedFlag <> _bgsObx.ApprovedFlag Then
            '   AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.ApprovedFlag, .ApprovedFlag.ToString, _bgsObx.ApprovedFlag.ToString)
            'End If

            If .DisburseFlag <> _bgsObx.DisburseFlag Then
               AppLib.AddLogDetail(_bgsObxLogDetailList, 0, LogColumnId.DisburseFlag, .DisburseFlag.ToString, _bgsObx.DisburseFlag.ToString)
            End If

         End With

         '
         ' BgsObxDetail
         '

         Dim _bgsObxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As BgsObxDetail In _bgsObxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As BgsObxDetail In _bgsObxDetailList
               If _new.ObxDetailId = _old.ObxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As BgsObxDetail In _bgsObxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As BgsObxDetail In _bgsObxDetailListOld
               If _new.ObxDetailId = _old.ObxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .CostCenterId <> _old.CostCenterId OrElse .Particulars <> _old.Particulars OrElse .ActivityId <> _old.ActivityId Then
                     '   .LogActionId = LogActionId.Edit
                     'End If

                     If .CostCenterId <> _old.CostCenterId OrElse .ActivityId <> _old.ActivityId OrElse .BudgetClassId <> _old.BudgetClassId Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .AllotmentId <> _old.AllotmentId OrElse .AccountId <> _old.AccountId OrElse .Amount <> _old.Amount Then
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

         Dim _bgsObxDetailListNew As New BgsObxDetailList      ' for adding new Obx Details

         If _addDetailCount > 0 Then
            Dim _bgsObxDetail As BgsObxDetail

            For Each _new As BgsObxDetail In _bgsObxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _bgsObxDetail = New BgsObxDetail
                  _bgsObxDetailListNew.Add(_bgsObxDetail)
                  DataLib.ScatterValues(_new, _bgsObxDetail)
                  _bgsObxDetail.ObxId = _bgsObx.ObxId
               End If
            Next

         End If


         '
         ' BgsObxVoucher
         '

         Dim _bgsObxVoucherLogDetailList As New SysLogDetailList

         Dim _removeVoucherCount As Integer
         Dim _addVoucherCount As Integer
         Dim _editVoucherCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As BgsObxVoucher In _bgsObxVoucherList
               If _new.ObxVoucherId = _old.ObxVoucherId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeVoucherCount = _removeVoucherCount + 1
            End If
         Next

         ' Mark vouchers for addition if not found in old list;
         ' Mark vouchers for modification if found in old list but with at least 1 property mismatch

         For Each _new As BgsObxVoucher In _bgsObxVoucherList
            _new.LogActionId = LogActionId.Add
            For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
               If _new.ObxVoucherId = _old.ObxVoucherId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .VoucherId <> _old.VoucherId OrElse .TotalAmount <> _old.TotalAmount OrElse .TaxAmount <> _old.TaxAmount Then
                        .LogActionId = LogActionId.Edit
                     End If

                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addVoucherCount = _addVoucherCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editVoucherCount = _editVoucherCount + 1
            End If

         Next

         Dim _bgsObxVoucherListNew As New BgsObxVoucherList      ' for adding new Obx Vouchers

         If _addVoucherCount > 0 Then
            Dim _bgsObxVoucher As BgsObxVoucher

            For Each _new As BgsObxVoucher In _bgsObxVoucherList
               If _new.LogActionId = LogActionId.Add Then
                  _bgsObxVoucher = New BgsObxVoucher
                  _bgsObxVoucherListNew.Add(_bgsObxVoucher)
                  DataLib.ScatterValues(_new, _bgsObxVoucher)
                  _bgsObxVoucher.ObxId = _bgsObx.ObxId
               End If
            Next

         End If

         Dim _isBgsObxChanged As Boolean = Me.HasBgsObxChanges(_bgsObxOld, _bgsObx)

         If Not _isBgsObxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 AndAlso _addVoucherCount = 0 AndAlso _removeVoucherCount = 0 AndAlso _editVoucherCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetBgsObx(obxId)
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

            If _isBgsObxChanged Then
               Me.UpdateBgsObx(_bgsObx)

               With _logKeyList
                  .Clear()
                  .Add("ObxId", _bgsObx.ObxId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsObxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsObxLogDetailList)

               AppLib.CreateLogDetails(_bgsObxLogDetailList, "BgsObxLogDetail")

            End If

            '
            ' BgsObxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteBgsObxDetails(_bgsObxDetailListOld)

               For Each _old As BgsObxDetail In _bgsObxDetailListOld
                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("ObxId", _old.ObxId)
                        .Add("AccountId", _old.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _bgsObxDetailLogDetailList.Clear()

                     With _old
                        AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

                        AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "Account ID=" + .AccountId)

                     End With

                     AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

                  End If
               Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertBgsObxDetails(_bgsObxDetailListNew)

               For Each _new As BgsObxDetail In _bgsObxDetailListNew

                  With _logKeyList
                     .Clear()
                     .Add("ObxId", _new.ObxId)
                     .Add("AccountId", _new.AccountId)
                  End With

                  _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _bgsObxDetailLogDetailList.Clear()

                  With _new
                     Dim _costCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
                     AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.CostCenterId, "", .CostCenterId.ToString, .CostCenterId.ToString + "=" + _costCenter)

                     'AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Particulars, "", .Particulars)

                     AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.ActivityId, "", .ActivityId.ToString, .ActivityId.ToString + "=" + .ActivityShortName)

                     Dim _budgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
                     AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.BudgetClassId, "", .BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _budgetClass)

                     AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

                     AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "AccountId=" + .AccountId)

                     AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

                  End With
               Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateBgsObxDetails(_bgsObxDetailList)

               For Each _new As BgsObxDetail In _bgsObxDetailList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("ObxId", _new.ObxId)
                        .Add("AccountId", _new.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsObxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _bgsObxDetailLogDetailList.Clear()

                     For Each _old As BgsObxDetail In _bgsObxDetailListOld
                        If _new.ObxDetailId = _old.ObxDetailId Then
                           With _new
                              If .CostCenterId <> _old.CostCenterId Then
                                 Dim _oldCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = _old.CostCenterId).CostCenterShortName
                                 Dim _newCostCenter As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName

                                 AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.CostCenterId, _old.CostCenterId.ToString, .CostCenterId.ToString, _old.CostCenterId.ToString + "=" + _oldCostCenter + "; " + .CostCenterId.ToString + "=" + _newCostCenter)
                              End If

                              'If .Particulars <> _old.Particulars Then
                              '   AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Particulars, _old.Particulars, .Particulars)
                              'End If

                              If .ActivityId <> _old.ActivityId Then
                                 AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.ActivityId, _old.ActivityId.ToString, .ActivityId.ToString, _old.ActivityId.ToString + "=" + _old.ActivityShortName + "; " + .ActivityId.ToString + "=" + .ActivityShortName)
                              End If

                              If .BudgetClassId <> _old.BudgetClassId Then
                                 Dim _oldBudgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = _old.BudgetClassId).BudgetClassName
                                 Dim _newBudgetClass As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName

                                 AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.BudgetClassId, _old.BudgetClassId.ToString, .BudgetClassId.ToString, _old.BudgetClassId.ToString + "=" + _oldBudgetClass + "; " + .BudgetClassId.ToString + "=" + _newBudgetClass)
                              End If

                              If .AccountId <> _old.AccountId Then
                                 AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId)
                              End If

                              If .Amount <> _old.Amount Then
                                 AppLib.AddLogDetail(_bgsObxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "Account ID=" + .AccountId)
                              End If

                              AppLib.CreateLogDetails(_bgsObxDetailLogDetailList, "BgsObxDetailLogDetail")

                           End With

                        End If
                     Next

                  End If

               Next

            End If

            '
            ' BgsObxVoucher
            '
            If _removeVoucherCount > 0 Then
               Me.DeleteBgsObxVouchers(_bgsObxVoucherListOld)

               For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("ObxId", _old.ObxId)
                        .Add("VoucherId", _old.VoucherId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _bgsObxVoucherLogDetailList.Clear()

                     With _old
                        AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, .VoucherId, "")

                        AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, .TotalAmount.ToString("N"), "", "VoucherId=" + .VoucherId)

                        If .TaxAmount > 0 Then
                           AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, .TaxAmount.ToString("N"), "", "VoucherId=" + .VoucherId)
                        End If

                     End With

                     AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

                  End If
               Next

            End If

            If _addVoucherCount > 0 Then
               Me.InsertBgsObxVouchers(_bgsObxVoucherListNew)

               For Each _new As BgsObxVoucher In _bgsObxVoucherListNew

                  With _logKeyList
                     .Clear()
                     .Add("ObxId", _new.ObxId)
                     .Add("VoucherId", _new.VoucherId)
                  End With

                  _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _bgsObxVoucherLogDetailList.Clear()

                  With _new
                     AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, "", .VoucherId)

                     AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, "", .TotalAmount.ToString("N"), "VoucherId=" + .VoucherId)

                     If .TaxAmount > 0 Then
                        AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, "", .TaxAmount.ToString("N"), "VoucherId=" + .VoucherId)
                     End If

                     AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

                  End With
               Next

            End If

            If _editVoucherCount > 0 Then
               Me.UpdateBgsObxVouchers(_bgsObxVoucherList)

               For Each _new As BgsObxVoucher In _bgsObxVoucherList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("ObxId", _new.ObxId)
                        .Add("VoucherId", _new.VoucherId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsObxVoucherLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _bgsObxVoucherLogDetailList.Clear()

                     For Each _old As BgsObxVoucher In _bgsObxVoucherListOld
                        If _new.ObxVoucherId = _old.ObxVoucherId Then
                           With _new
                              If .VoucherId <> _old.VoucherId Then
                                 AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.VoucherId, _old.VoucherId, .VoucherId)
                              End If

                              If .TotalAmount <> _old.TotalAmount Then
                                 AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TotalAmount, _old.TotalAmount.ToString("N"), .TotalAmount.ToString("N"), "VoucherId=" + .VoucherId)
                              End If

                              If .TaxAmount <> _old.TaxAmount Then
                                 AppLib.AddLogDetail(_bgsObxVoucherLogDetailList, _id, LogColumnId.TaxAmount, _old.TaxAmount.ToString("N"), .TaxAmount.ToString("N"), "VoucherId=" + .VoucherId)
                              End If

                              AppLib.CreateLogDetails(_bgsObxVoucherLogDetailList, "BgsObxVoucherLogDetail")

                           End With

                        End If
                     Next

                  End If

               Next

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

   <SymAuthorization("RemoveBgsObx")>
   <Route("obligations/{obxId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveBgsObx(obxId As Integer, lockId As String) As IHttpActionResult

      If obxId <= 0 Then
         Throw New ArgumentException("Obligation ID is required.")
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

            Me.DeleteBgsObx(obxId, lockId)

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

   Private Sub LoadBgsObx(obx As BgsObxBody, bgsObx As BgsObx)

      DataLib.ScatterValues(obx, bgsObx)

   End Sub

   Private Sub LoadBgsObx(row As DataRow, obx As BgsObx)

      With obx
         .ObxId = row.ToInt32("ObxId")
         .ObxDate = row.ToDate("ObxDate")
         .FundClusterId = row.ToString("FundClusterId")
         .ObxStatusId = row.ToInt32("ObxStatusId")
         .DocumentId = row.ToString("DocumentId")
         .PayeeId = row.ToInt32("PayeeId")

         .PayeeName = row.ToString("PayeeName")
         '.PayeeOffice = row.ToString("PayeeOffice")
         '.PayeeAddress = row.ToString("PayeeAddress")
         '.PayeeBankName = row.ToString("PayeeBankName")
         '.PayeeBankAccountNumber = row.ToString("PayeeBankAccountNumber")

         .Particulars = row.ToString("Particulars")
         .Remarks = row.ToString("Remarks")
         .PostUserId = row.ToInt32("PostUserId")
         .RequestSignatoryId = row.ToInt32("RequestSignatoryId")
         .SignatoryId = row.ToInt32("SignatoryId")
         '.BudgetClassId = row.ToInt32("budgetClassId")
         '.ApprovedFlag = row.ToBoolean("ApprovedFlag")
         .DisburseFlag = row.ToBoolean("DisburseFlag")
         .AdjustmentId = row.ToString("AdjustmentId")
         .AdjustObxId = row.ToInt32("AdjustObxId")
      End With

   End Sub

   Private Sub LoadBgsObxDetailList(rows As DataRowCollection, list As BgsObxDetailList)

      Dim _detail As BgsObxDetail
      For Each _row As DataRow In rows
         _detail = New BgsObxDetail

         With _detail
            .ObxDetailId = _row.ToInt32("ObxDetailId")
            .ObxId = _row.ToInt32("ObxId")
            .CostCenterId = _row.ToInt32("CostCenterId")
            .CostCenterShortName = _row.ToString("CostCenterShortName")
            '.Particulars = _row.ToString("Particulars")
            .ActivityId = _row.ToInt32("ActivityId")
            .ActivityShortName = _row.ToString("ActivityShortName")
            .BudgetClassId = _row.ToInt32("BudgetClassId")
            .BudgetClassName = _row.ToString("BudgetClassName")
            .AccountId = _row.ToString("AccountId")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub LoadBgsObxVoucherList(rows As DataRowCollection, list As BgsObxVoucherList)

      Dim _voucher As BgsObxVoucher
      For Each _row As DataRow In rows
         _voucher = New BgsObxVoucher

         With _voucher
            .ObxVoucherId = _row.ToInt32("ObxVoucherId")
            .ObxId = _row.ToInt32("ObxId")
            .VoucherId = _row.ToString("VoucherId")
            .TotalAmount = _row.ToDecimal("TotalAmount")
            .TaxAmount = _row.ToDecimal("TaxAmount")
         End With

         list.Add(_voucher)

      Next

   End Sub

   Private Sub InsertBgsObx(obx As BgsObx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("PayeeName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsObx", obx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsObx(obx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertBgsObxDetails(list As BgsObxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ObxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("CostCenterShortName")
         .Add("ActivityShortName")
         .Add("BudgetClassName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsObxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As BgsObxDetail In list
            Me.AddInsertUpdateParamsBgsObxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub InsertBgsObxVouchers(list As BgsObxVoucherList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ObxVoucherId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsObxVoucher", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _voucher As BgsObxVoucher In list
            Me.AddInsertUpdateParamsBgsObxVoucher(_voucher)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateBgsObx(obx As BgsObx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ObxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("PayeeName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsObx", obx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsObx(obx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(obx.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsObxDetails(list As BgsObxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ObxDetailId")
      End With

      With _excludedFields
         .Add("CostCenterShortName")
         .Add("ActivityShortName")
         .Add("BudgetClassName")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsObxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As BgsObxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsObxDetail(_detail)
               .Parameters.AddWithValue("@ObxDetailId", _detail.ObxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub UpdateBgsObxVouchers(list As BgsObxVoucherList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ObxVoucherId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsObxVoucher", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _voucher As BgsObxVoucher In list
            If _voucher.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsObxVoucher(_voucher)
               .Parameters.AddWithValue("@ObxVoucherId", _voucher.ObxVoucherId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsObx(obx As BgsObx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ObxId", obx.ObxId)
         .AddWithValue("@ObxDate", obx.ObxDate)
         .AddWithValue("@FundClusterId", obx.FundClusterId)
         .AddWithValue("@ObxStatusId", obx.ObxStatusId)
         .AddWithValue("@DocumentId", obx.DocumentId)
         .AddWithValue("@PayeeId", obx.PayeeId)

         '.AddWithValue("@PayeeName", obx.PayeeName)
         '.AddWithValue("@PayeeOffice", obx.PayeeOffice.ToNullable)
         '.AddWithValue("@PayeeAddress", obx.PayeeAddress.ToNullable)
         '.AddWithValue("@PayeeBankName", obx.PayeeBankName.ToNullable)
         '.AddWithValue("@PayeeBankAccountNumber", obx.PayeeBankAccountNumber.ToNullable)

         .AddWithValue("@Particulars", obx.Particulars)
         .AddWithValue("@Remarks", obx.Remarks.ToNullable)
         .AddWithValue("@PostUserId", obx.PostUserId)
         .AddWithValue("@RequestSignatoryId", obx.RequestSignatoryId)
         .AddWithValue("@SignatoryId", obx.SignatoryId)
         '.AddWithValue("@BudgetClassId", obx.BudgetClassId)
         '.AddWithValue("@ApprovedFlag", obx.ApprovedFlag)
         .AddWithValue("@DisburseFlag", obx.DisburseFlag)
         .AddWithValue("@AdjustmentId", obx.AdjustmentId.ToNullable)
         .AddWithValue("@AdjustObxId", obx.AdjustObxId.ToNullable)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsObxDetail(dtl As BgsObxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ObxId", dtl.ObxId)
         .AddWithValue("@CostCenterId", dtl.CostCenterId)
         '.AddWithValue("@Particulars", dtl.Particulars)
         .AddWithValue("@ActivityId", dtl.ActivityId)
         .AddWithValue("@BudgetClassId", dtl.BudgetClassId)
         .AddWithValue("@AllotmentId", dtl.AllotmentId.ToNullable)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsObxVoucher(vch As BgsObxVoucher)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ObxId", vch.ObxId)
         .AddWithValue("@VoucherId", vch.VoucherId)
         .AddWithValue("@TotalAmount", vch.TotalAmount)
         .AddWithValue("@TaxAmount", vch.TaxAmount)
      End With

   End Sub

   Private Sub DeleteBgsObx(obxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ObxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsObx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ObxId", obxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteBgsObxDetails(list As BgsObxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsObxDetail WHERE ObxDetailId=@ObxDetailId"
         .CommandType = CommandType.Text

         For Each _old As BgsObxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ObxDetailId", _old.ObxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub DeleteBgsObxVouchers(list As BgsObxVoucherList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsObxVoucher WHERE ObxVoucherId=@ObxVoucherId"
         .CommandType = CommandType.Text

         For Each _old As BgsObxVoucher In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ObxVoucherId", _old.ObxVoucherId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasBgsObxChanges(oldRecord As BgsObx, newRecord As BgsObx) As Boolean

      With oldRecord
         If .ObxDate <> newRecord.ObxDate Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .ObxStatusId <> newRecord.ObxStatusId Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .PayeeId <> newRecord.PayeeId Then Return True

         'If .PayeeName <> newRecord.PayeeName Then Return True
         'If .PayeeOffice <> newRecord.PayeeOffice Then Return True
         'If .PayeeAddress <> newRecord.PayeeAddress Then Return True

         If .Particulars <> newRecord.Particulars Then Return True
         If .Remarks <> newRecord.Remarks Then Return True
         If .RequestSignatoryId <> newRecord.RequestSignatoryId Then Return True
         'If .BudgetClassId <> newRecord.BudgetClassId Then Return True
         'If .ApprovedFlag <> newRecord.ApprovedFlag Then Return True
         If .DisburseFlag <> newRecord.DisburseFlag Then Return True
         If .AdjustmentId <> newRecord.AdjustmentId Then Return True
         If .AdjustObxId <> newRecord.AdjustObxId Then Return True
      End With

      Return False

   End Function

End Class

Public Class BgsObxBody
   Inherits BgsObx

   Public Property Details As BgsObxDetail()
   Public Property Vouchers As BgsObxVoucher()

End Class

