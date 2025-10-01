<RoutePrefix("api")>
Public Class GLS0200_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_GLS0200")>
   <Route("references/gls0200")>
   <HttpGet>
   Public Function GetReferences_GLS0200() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.GLS0200_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "coxTypes"         ' all defined collection types
                     .Tables(2).TableName = "banks"            ' all defined banks
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFinOpx")>
   <Route("ops/{opxId}")>
   <HttpGet>
   Public Function GetFinOpx(opxId As Integer) As IHttpActionResult

      If opxId <= 0 Then
         Throw New ArgumentException("OP ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.GLS0200")
            With _direct
               .AddParameter("OpxId", opxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "opx"
                     .Tables(1).TableName = "opxDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateFinOpx")>
   <Route("ops/{currentUserId}")>
   <HttpPost>
   Public Function CreateFinOpx(currentUserId As Integer, <FromBody> opx As FinOpxBody) As IHttpActionResult

      If opx.OpxId <> -1 Then
         Throw New ArgumentException("OP ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _opxId As Integer = SysLib.GetNextSequence("FinOpxId")

         opx.OpxId = _opxId

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If opx.DocumentId = "< NEW >" Then
            opx.DocumentId = AppLib.GetNextDocSequence(DocSequencerId.OP, opx.OpxDate)
         End If

         '
         ' Load proposed values from payload
         '
         Dim _finOpx As New FinOpx
         Dim _finOpxDetailList As New FinOpxDetailList

         Me.LoadFinOpx(opx, _finOpx)

         For Each _detail As FinOpxDetail In opx.Details
            _detail.OpxId = _opxId
            _finOpxDetailList.Add(_detail)
         Next

         '
         ' Log addition, save to DB
         '

         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         'Dim _cshCoxLogDetailList As New SysLogDetailList
         'Dim _cshCoxDetailLogDetailList As New SysLogDetailList

         'With _cshCox
         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)

         '   Dim _fundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.FundClusterId, String.Empty, .FundClusterId, .FundClusterId + "=" + _fundClusterName)

         '   Dim _statusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

         '   Dim _typeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = .CoxTypeId).CoxTypeName
         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CoxTypeId, String.Empty, .CoxTypeId.ToString, .CoxTypeId.ToString + "=" + _typeName)

         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.ReceiptNumber, String.Empty, .ReceiptNumber)

         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.PayorName, String.Empty, .PayorName)

         '   AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CollectorId, String.Empty, .CollectorId.ToString)

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

            Me.InsertFinOpx(_finOpx)
            Me.InsertFinOpxDetails(_finOpxDetailList)

            'If _cshCoxLogDetailList.Count > 0 Then
            '   With _logKeyList
            '      .Clear()
            '      .Add("CoxId", _cshCox.CoxId)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsCshCoxLog", _logKeyList, LogActionId.Add, _currentUserId)
            '   AppLib.AssignLogHeaderId(_id, _cshCoxLogDetailList)
            '   AppLib.CreateLogDetails(_cshCoxLogDetailList, "CshCoxLogDetail")

            'End If

            'For Each _new As CshCoxDetail In _cshCoxDetailList

            '   With _logKeyList
            '      .Clear()
            '      .Add("CoxId", _new.CoxId)
            '      .Add("TrxCode", _new.TrxCode)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

            '   _cshCoxDetailLogDetailList.Clear()

            '   With _new
            '      AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, "", .TrxCode.ToString, .TrxCode.ToString + "=" + .TrxName)

            '      AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)

            '      AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

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
         Return Me.Ok(opx.OpxId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyFinOpx")>
   <Route("ops/{opxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFinOpx(opxId As Integer, currentUserId As Integer, <FromBody> opx As FinOpxBody) As IHttpActionResult

      If opxId <= 0 Then
         Throw New ArgumentException("OP ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _finOpx As New FinOpx
         Dim _finOpxDetailList As New FinOpxDetailList

         Me.LoadFinOpx(opx, _finOpx)

         For Each _detail As FinOpxDetail In opx.Details
            _finOpxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _finOpxOld As New FinOpx
         Dim _finOpxDetailListOld As New FinOpxDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetFinOpx(opxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("opx").Rows(0)
            Me.LoadFinOpx(_row, _finOpxOld)
            Me.LoadFinOpxDetailList(_dataSet.Tables("opxDetails").Rows, _finOpxDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         '
         ' CshCox
         '

         'Dim _cshCoxLogDetailList As New SysLogDetailList

         'With _cshCoxOld
         '   If .TrxDate <> _cshCox.TrxDate Then
         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxDate, .TrxDate.ToDisplayFormat, _cshCox.TrxDate.ToDisplayFormat)
         '   End If

         '   If .FundClusterId <> _cshCox.FundClusterId Then
         '      Dim _oldFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = .FundClusterId).FundClusterName
         '      Dim _newFundClusterName As String = FrsSession.DbsFundCluster.Rows.Find(Function(m) m.FundClusterId = _cshCox.FundClusterId).FundClusterName

         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.FundClusterId, .FundClusterId, _cshCox.FundClusterId, .FundClusterId + "=" + _oldFundClusterName + "; " + _cshCox.FundClusterId + "=" + _newFundClusterName)
         '   End If

         '   If .TrxStatusId <> _cshCox.TrxStatusId Then
         '      Dim _oldStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
         '      Dim _newStatusName As String = FrsSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = _cshCox.TrxStatusId).TrxStatusName

         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.TrxStatusId, .TrxStatusId.ToString, _cshCox.TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _oldStatusName + "; " + _cshCox.TrxStatusId.ToString + "=" + _newStatusName)
         '   End If

         '   If .CoxTypeId <> _cshCox.CoxTypeId Then
         '      Dim _oldTypeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = .CoxTypeId).CoxTypeName
         '      Dim _newTypeName As String = FrsSession.DbsCoxType.Rows.Find(Function(m) m.CoxTypeId = _cshCox.CoxTypeId).CoxTypeName

         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CoxTypeId, .CoxTypeId.ToString, _cshCox.CoxTypeId.ToString, .CoxTypeId.ToString + "=" + _oldTypeName + "; " + _cshCox.CoxTypeId.ToString + "=" + _newTypeName)
         '   End If

         '   If .ReceiptNumber <> _cshCox.ReceiptNumber Then
         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.ReceiptNumber, .ReceiptNumber, _cshCox.ReceiptNumber)
         '   End If

         '   If .Particulars <> _cshCox.Particulars Then
         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.Particulars, .Particulars, _cshCox.Particulars)
         '   End If

         '   If .PayorName <> _cshCox.PayorName Then
         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.PayorName, .PayorName, _cshCox.PayorName)
         '   End If

         '   If .CollectorId <> _cshCox.CollectorId Then
         '      AppLib.AddLogDetail(_cshCoxLogDetailList, 0, LogColumnId.CollectorId, .CollectorId.ToString, _cshCox.CollectorId.ToString)
         '   End If

         'End With

         '
         ' FinOpxDetail
         '

         'Dim _cshCoxDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FinOpxDetail In _finOpxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FinOpxDetail In _finOpxDetailList
               If _new.OpxDetailId = _old.OpxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As FinOpxDetail In _finOpxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FinOpxDetail In _finOpxDetailListOld
               If _new.OpxDetailId = _old.OpxDetailId Then
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

         Dim _finOpxDetailListNew As New FinOpxDetailList      ' for adding new Opx Details

         If _addDetailCount > 0 Then
            Dim _finOpxDetail As FinOpxDetail

            For Each _new As FinOpxDetail In _finOpxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _finOpxDetail = New FinOpxDetail
                  _finOpxDetailListNew.Add(_finOpxDetail)
                  DataLib.ScatterValues(_new, _finOpxDetail)
                  _finOpxDetail.OpxId = _finOpx.OpxId
               End If
            Next

         End If

         Dim _isFinOpxChanged As Boolean = Me.HasFinOpxChanges(_finOpxOld, _finOpx)

         If Not _isFinOpxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetFinOpx(opxId)
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

            If _isFinOpxChanged Then
               Me.UpdateFinOpx(_finOpx)

               'With _logKeyList
               '   .Clear()
               '   .Add("CoxId", _cshCox.CoxId)
               'End With

               '_id = AppLib.CreateLogHeader("InsCshCoxLog", _logKeyList, LogActionId.Edit, _currentUserId)
               'AppLib.AssignLogHeaderId(_id, _cshCoxLogDetailList)

               'AppLib.CreateLogDetails(_cshCoxLogDetailList, "CshCoxLogDetail")

            End If

            '
            ' FinOpxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFinOpxDetails(_finOpxDetailListOld)

               'For Each _old As CshCoxDetail In _cshCoxDetailListOld
               '   If _old.LogActionId = LogActionId.Delete Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("CoxId", _old.CoxId)
               '         .Add("TrxCode", _old.TrxCode)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

               '      _cshCoxDetailLogDetailList.Clear()

               '      With _old
               '         AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, .TrxCode.ToString, "")

               '         AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, .Amount.ToString("N"), "", "TrxCode=" + .TrxCode.ToString)

               '      End With

               '      AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

               '   End If
               'Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertFinOpxDetails(_finOpxDetailListNew)

               'For Each _new As CshCoxDetail In _cshCoxDetailListNew

               '   With _logKeyList
               '      .Clear()
               '      .Add("CoxId", _new.CoxId)
               '      .Add("TrxCode", _new.TrxCode)
               '   End With

               '   _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               '   _cshCoxDetailLogDetailList.Clear()

               '   With _new
               '      AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, "", .TrxCode.ToString, .TrxCode.ToString + "=" + .TrxName)

               '      AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, "", .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)

               '      AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

               '   End With
               'Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateFinOpxDetails(_finOpxDetailList)

               'For Each _new As CshCoxDetail In _cshCoxDetailList
               '   If _new.LogActionId = LogActionId.Edit Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("CoxId", _new.CoxId)
               '         .Add("TrxCode", _new.TrxCode)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsCshCoxDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

               '      _cshCoxDetailLogDetailList.Clear()

               '      For Each _old As CshCoxDetail In _cshCoxDetailListOld
               '         If _new.CoxDetailId = _old.CoxDetailId Then
               '            With _new
               '               If .TrxCode <> _old.TrxCode Then
               '                  AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.TrxCode, _old.TrxCode.ToString, .TrxCode.ToString, _old.TrxCode.ToString + "=" + _old.TrxName + "; " + .TrxCode.ToString + "=" + .TrxName)
               '               End If

               '               If .Amount <> _old.Amount Then
               '                  AppLib.AddLogDetail(_cshCoxDetailLogDetailList, _id, LogColumnId.Amount, _old.Amount.ToString("N"), .Amount.ToString("N"), "TrxCode=" + .TrxCode.ToString)
               '               End If

               '               AppLib.CreateLogDetails(_cshCoxDetailLogDetailList, "CshCoxDetailLogDetail")

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

   <SymAuthorization("RemoveFinOpx")>
   <Route("ops/{opxId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveFinOpx(opxId As Integer, lockId As String) As IHttpActionResult

      If opxId <= 0 Then
         Throw New ArgumentException("OP ID is required.")
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

            Me.DeleteFinOpx(opxId, lockId)

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

   Private Sub LoadFinOpx(opx As FinOpxBody, finOpx As FinOpx)

      DataLib.ScatterValues(opx, finOpx)

   End Sub

   Private Sub LoadFinOpx(row As DataRow, opx As FinOpx)

      With opx
         .OpxId = row.ToInt32("OpxId")
         .OpxDate = row.ToDate("OpxDate")
         .DocumentId = row.ToString("DocumentId")
         .FundClusterId = row.ToString("FundClusterId")
         .BankId = row.ToInt32("BankId")
         .CoxTypeId = row.ToInt32("CoxTypeId")
         .BillNumber = row.ToString("BillNumber")
         .BillDate = row.ToDate("billDate")
         .Particulars = row.ToString("Particulars")
         .PayorName = row.ToString("PayorName")
         .PostUserId = row.ToInt32("PostUserId")
         .SignatoryId = row.ToInt32("SignatoryId")
      End With

   End Sub

   Private Sub LoadFinOpxDetailList(rows As DataRowCollection, list As FinOpxDetailList)

      Dim _detail As FinOpxDetail
      For Each _row As DataRow In rows
         _detail = New FinOpxDetail

         With _detail
            .OpxDetailId = _row.ToInt32("OpxDetailId")
            .OpxId = _row.ToInt32("OpxId")
            .TrxCode = _row.ToInt32("TrxCode")
            '.TrxName = _row.ToString("TrxName")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertFinOpx(opx As FinOpx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinOpx", opx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinOpx(opx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFinOpxDetails(list As FinOpxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("OpxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
         .Add("TrxName")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinOpxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinOpxDetail In list
            Me.AddInsertUpdateParamsFinOpxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateFinOpx(opx As FinOpx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("OpxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinOpx", opx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinOpx(opx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(opx.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFinOpxDetails(list As FinOpxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("OpxDetailId")
      End With

      With _excludedFields
         .Add("TrxName")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinOpxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinOpxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFinOpxDetail(_detail)
               .Parameters.AddWithValue("@OpxDetailId", _detail.OpxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinOpx(opx As FinOpx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@OpxId", opx.OpxId)
         .AddWithValue("@OpxDate", opx.OpxDate)
         .AddWithValue("@DocumentId", opx.DocumentId)
         .AddWithValue("@FundClusterId", opx.FundClusterId)
         .AddWithValue("@BankId", opx.BankId)
         .AddWithValue("@CoxTypeId", opx.CoxTypeId)
         .AddWithValue("@BillNumber", opx.BillNumber)
         .AddWithValue("@BillDate", opx.BillDate)
         .AddWithValue("@Particulars", opx.Particulars)
         .AddWithValue("@PayorName", opx.PayorName)
         .AddWithValue("@PostUserId", opx.PostUserId)
         .AddWithValue("@SignatoryId", opx.SignatoryId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinOpxDetail(dtl As FinOpxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@OpxId", dtl.OpxId)
         .AddWithValue("@TrxCode", dtl.TrxCode)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub

   Private Sub DeleteFinOpx(opxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("OpxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinOpx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@OpxId", opxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFinOpxDetails(list As FinOpxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FinOpxDetail WHERE OpxDetailId=@OpxDetailId"
         .CommandType = CommandType.Text

         For Each _old As FinOpxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@OpxDetailId", _old.OpxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasFinOpxChanges(oldRecord As FinOpx, newRecord As FinOpx) As Boolean

      With oldRecord
         If .OpxDate <> newRecord.OpxDate Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .BankId <> newRecord.BankId Then Return True
         If .CoxTypeId <> newRecord.CoxTypeId Then Return True
         If .BillNumber <> newRecord.BillNumber Then Return True
         If .BillDate <> newRecord.BillDate Then Return True
         If .Particulars <> newRecord.Particulars Then Return True
         If .PayorName <> newRecord.PayorName Then Return True
         If .SignatoryId <> newRecord.SignatoryId Then Return True
      End With

      Return False

   End Function

End Class

Public Class FinOpxBody
   Inherits FinOpx

   Public Property Details As FinOpxDetail()

End Class
