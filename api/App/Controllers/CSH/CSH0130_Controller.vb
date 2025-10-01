<RoutePrefix("api")>
Public Class CSH0130_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0130")>
   <Route("references/csh0130")>
   <HttpGet>
   Public Function GetReferences_CSH0130() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0130_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "banks"            ' all defined banks
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFrsEmd")>
   <Route("emd/{emdId}")>
   <HttpGet>
   Public Function GetFrsEmd(emdId As Integer) As IHttpActionResult

      If emdId <= 0 Then
         Throw New ArgumentException("EMDS ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0130")
            With _direct
               .AddParameter("EmdId", emdId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "emd"
                     .Tables(1).TableName = "emdDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateFrsEmd")>
   <Route("emd/{currentUserId}")>
   <HttpPost>
   Public Function CreateFrsEmd(currentUserId As Integer, <FromBody> emd As FrsEmdBody) As IHttpActionResult

      If emd.EmdId <> -1 Then
         Throw New ArgumentException("EMDS ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _emdId As Integer = SysLib.GetNextSequence("FrsEmdId")

         emd.EmdId = _emdId

         emd.AmountText = AppLib.NumberToText(emd.NetAmount, FractionStyle.PerHundred)

         '
         ' Load proposed values from payload
         '
         Dim _frsEmd As New FrsEmd
         Dim _frsEmdDetailList As New FrsEmdDetailList

         Me.LoadFrsEmd(emd, _frsEmd)

         For Each _detail As FrsEmdDetail In emd.Details
            _detail.EmdId = _emdId
            _frsEmdDetailList.Add(_detail)
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

            Me.InsertFrsEmd(_frsEmd)
            Me.InsertFrsEmdDetails(_frsEmdDetailList)

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
         Return Me.Ok(emd.EmdId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyFrsEmd")>
   <Route("emd/{emdId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFrsEmd(emdId As Integer, currentUserId As Integer, <FromBody> emd As FrsEmdBody) As IHttpActionResult

      If emdId <= 0 Then
         Throw New ArgumentException("EMDS ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _frsEmd As New FrsEmd
         Dim _frsEmdDetailList As New FrsEmdDetailList

         Me.LoadFrsEmd(emd, _frsEmd)

         For Each _detail As FrsEmdDetail In emd.Details
            _frsEmdDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _frsEmdOld As New FrsEmd
         Dim _frsEmdDetailListOld As New FrsEmdDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetFrsEmd(emdId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("emd").Rows(0)
            Me.LoadFrsEmd(_row, _frsEmdOld)
            Me.LoadFrsEmdDetailList(_dataSet.Tables("emdDetails").Rows, _frsEmdDetailListOld)
         End Using

         '
         ' FrsEmdDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FrsEmdDetail In _frsEmdDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FrsEmdDetail In _frsEmdDetailList
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

         For Each _new As FrsEmdDetail In _frsEmdDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FrsEmdDetail In _frsEmdDetailListOld
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

         Dim _frsEmdDetailListNew As New FrsEmdDetailList      ' for adding new EMD Details

         If _addDetailCount > 0 Then
            Dim _frsEmdDetail As FrsEmdDetail

            For Each _new As FrsEmdDetail In _frsEmdDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _frsEmdDetail = New FrsEmdDetail
                  _frsEmdDetailListNew.Add(_frsEmdDetail)
                  DataLib.ScatterValues(_new, _frsEmdDetail)
                  _frsEmdDetail.EmdId = _frsEmd.EmdId
               End If
            Next

         End If

         Dim _isFrsEmdChanged As Boolean = Me.HasFrsEmdChanges(_frsEmdOld, _frsEmd)

         If Not _isFrsEmdChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current record
            ' 
            Return Me.GetFrsEmd(emdId)
         End If

         If _frsEmdOld.NetAmount <> _frsEmd.NetAmount Then
            _frsEmd.AmountText = AppLib.NumberToText(emd.NetAmount, FractionStyle.PerHundred)
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

            If _isFrsEmdChanged Then
               Me.UpdateFrsEmd(_frsEmd)
            End If

            '
            ' FrsEmdDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFrsEmdDetails(_frsEmdDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertFrsEmdDetails(_frsEmdDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateFrsEmdDetails(_frsEmdDetailList)
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

   <SymAuthorization("RemoveFrsEmd")>
   <Route("emd/{emdId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveFrsEmd(emdId As Integer, lockId As String) As IHttpActionResult

      If emdId <= 0 Then
         Throw New ArgumentException("EMDS ID is required.")
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

            Me.DeleteFrsEmd(emdId, lockId)

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

   Private Sub LoadFrsEmd(emd As FrsEmdBody, frsEmd As FrsEmd)

      DataLib.ScatterValues(emd, frsEmd)

   End Sub

   Private Sub LoadFrsEmd(row As DataRow, emd As FrsEmd)

      With emd
         .EmdId = row.ToInt32("EmdId")
         .ReportDate = row.ToDate("ReportDate")
         .DocumentId = row.ToString("DocumentId")
         .DepartmentName = row.ToString("DepartmentName")
         .AgencyName = row.ToString("AgencyName")
         .OrgCode = row.ToString("OrgCode")
         .FundClusterId = row.ToString("FundClusterId")
         .BankId = row.ToInt32("BankId")
         .NcaNumber = row.ToString("NcaNumber")
         .ControlNumber = row.ToString("ControlNumber")
         .TotalAmount = row.ToDecimal("TotalAmount")
         .TaxAmount = row.ToDecimal("TaxAmount")
         .NetAmount = row.ToDecimal("NetAmount")
         .AmountText = row.ToString("AmountText")
         .PostUserId = row.ToInt32("PostUserId")
      End With

   End Sub

   Private Sub LoadFrsEmdDetailList(rows As DataRowCollection, list As FrsEmdDetailList)

      Dim _detail As FrsEmdDetail
      For Each _row As DataRow In rows
         _detail = New FrsEmdDetail

         With _detail
            .DetailId = _row.ToInt32("DetailId")
            .EmdId = _row.ToInt32("EmdId")
            .DbxId = _row.ToInt32("DbxId")
            .AccountId = _row.ToString("AccountId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertFrsEmd(emd As FrsEmd)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsEmd", emd, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsEmd(emd)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFrsEmdDetails(list As FrsEmdDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsEmdDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsEmdDetail In list
            Me.AddInsertUpdateParamsFrsEmdDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateFrsEmd(emd As FrsEmd)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("EmdId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsEmd", emd, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsEmd(emd)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(emd.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFrsEmdDetails(list As FrsEmdDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("DetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsEmdDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsEmdDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFrsEmdDetail(_detail)
               .Parameters.AddWithValue("@DetailId", _detail.DetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsEmd(emd As FrsEmd)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@EmdId", emd.EmdId)
         .AddWithValue("@ReportDate", emd.ReportDate)
         .AddWithValue("@DocumentId", emd.DocumentId)
         .AddWithValue("@DepartmentName", emd.DepartmentName)
         .AddWithValue("@AgencyName", emd.AgencyName)
         .AddWithValue("@OrgCode", emd.OrgCode)
         .AddWithValue("@FundClusterId", emd.FundClusterId)
         .AddWithValue("@BankId", emd.BankId)
         .AddWithValue("@NcaNumber", emd.NcaNumber)
         .AddWithValue("@ControlNumber", emd.ControlNumber)
         .AddWithValue("@TotalAmount", emd.TotalAmount)
         .AddWithValue("@TaxAmount", emd.TaxAmount)
         .AddWithValue("@AmountText", emd.AmountText)
         .AddWithValue("@PostUserId", emd.PostUserId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsEmdDetail(dtl As FrsEmdDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@EmdId", dtl.EmdId)
         .AddWithValue("@DbxId", dtl.DbxId)
         .AddWithValue("@AccountId", dtl.AccountId)
      End With

   End Sub

   Private Sub DeleteFrsEmd(emdId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("EmdId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FrsEmd", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@EmdId", emdId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFrsEmdDetails(list As FrsEmdDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FrsEmdDetail WHERE DetailId=@DetailId"
         .CommandType = CommandType.Text

         For Each _old As FrsEmdDetail In list
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

   Private Function HasFrsEmdChanges(oldRecord As FrsEmd, newRecord As FrsEmd) As Boolean

      With oldRecord
         If .ReportDate <> newRecord.ReportDate Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .BankId <> newRecord.BankId Then Return True
         If .NcaNumber <> newRecord.NcaNumber Then Return True
         If .ControlNumber <> newRecord.ControlNumber Then Return True
         If .TotalAmount <> newRecord.TotalAmount Then Return True
         If .TaxAmount <> newRecord.TaxAmount Then Return True
      End With

      Return False

   End Function

End Class

Public Class FrsEmdBody
   Inherits FrsEmd

   Public Property Details As FrsEmdDetail()

End Class
