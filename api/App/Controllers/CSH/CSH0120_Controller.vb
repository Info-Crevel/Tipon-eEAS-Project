<RoutePrefix("api")>
Public Class CSH0120_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0120")>
   <Route("references/csh0120")>
   <HttpGet>
   Public Function GetReferences_CSH0120() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0120_References")
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

   <SymAuthorization("GetFrsCkd")>
   <Route("ckd/{ckdId}")>
   <HttpGet>
   Public Function GetFrsCkd(ckdId As Integer) As IHttpActionResult

      If ckdId <= 0 Then
         Throw New ArgumentException("RCI ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0120")
            With _direct
               .AddParameter("CkdId", ckdId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "ckd"
                     .Tables(1).TableName = "ckdDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   <SymAuthorization("CreateFrsCkd")>
   <Route("ckd/{currentUserId}")>
   <HttpPost>
   Public Function CreateFrsCkd(currentUserId As Integer, <FromBody> ckd As FrsCkdBody) As IHttpActionResult

      If ckd.CkdId <> -1 Then
         Throw New ArgumentException("RCI ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _ckdId As Integer = SysLib.GetNextSequence("FrsCkdId")

         ckd.CkdId = _ckdId

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If ckd.DocumentId = "< NEW >" Then
            ckd.DocumentId = AppLib.GetNextDocSequence(DocSequencerId.RCI, ckd.ReportDate)
         End If

         '
         ' Load proposed values from payload
         '
         Dim _frsCkd As New FrsCkd
         Dim _frsCkdDetailList As New FrsCkdDetailList

         Me.LoadFrsCkd(ckd, _frsCkd)

         For Each _detail As FrsCkdDetail In ckd.Details
            _detail.CkdId = _ckdId
            _frsCkdDetailList.Add(_detail)
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

            Me.InsertFrsCkd(_frsCkd)
            Me.InsertFrsCkdDetails(_frsCkdDetailList)

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
         Return Me.Ok(ckd.CkdId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyFrsCkd")>
   <Route("ckd/{ckdId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFrsCkd(ckdId As Integer, currentUserId As Integer, <FromBody> ckd As FrsCkdBody) As IHttpActionResult

      If ckdId <= 0 Then
         Throw New ArgumentException("RCI ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _frsCkd As New FrsCkd
         Dim _frsCkdDetailList As New FrsCkdDetailList

         Me.LoadFrsCkd(ckd, _frsCkd)

         For Each _detail As FrsCkdDetail In ckd.Details
            _frsCkdDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _frsCkdOld As New FrsCkd
         Dim _frsCkdDetailListOld As New FrsCkdDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetFrsCkd(ckdId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("ckd").Rows(0)
            Me.LoadFrsCkd(_row, _frsCkdOld)
            Me.LoadFrsCkdDetailList(_dataSet.Tables("ckdDetails").Rows, _frsCkdDetailListOld)
         End Using

         '
         ' FrsCkdDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FrsCkdDetail In _frsCkdDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FrsCkdDetail In _frsCkdDetailList
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

         For Each _new As FrsCkdDetail In _frsCkdDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FrsCkdDetail In _frsCkdDetailListOld
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

         Dim _frsCkdDetailListNew As New FrsCkdDetailList      ' for adding new RCI Details

         If _addDetailCount > 0 Then
            Dim _frsCkdDetail As FrsCkdDetail

            For Each _new As FrsCkdDetail In _frsCkdDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _frsCkdDetail = New FrsCkdDetail
                  _frsCkdDetailListNew.Add(_frsCkdDetail)
                  DataLib.ScatterValues(_new, _frsCkdDetail)
                  _frsCkdDetail.CkdId = _frsCkd.CkdId
               End If
            Next

         End If

         Dim _isFrsCkdChanged As Boolean = Me.HasFrsCkdChanges(_frsCkdOld, _frsCkd)

         If Not _isFrsCkdChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current record
            ' 
            Return Me.GetFrsCkd(ckdId)
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

            If _isFrsCkdChanged Then
               Me.UpdateFrsCkd(_frsCkd)
            End If

            '
            ' FrsCkdDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFrsCkdDetails(_frsCkdDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertFrsCkdDetails(_frsCkdDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateFrsCkdDetails(_frsCkdDetailList)
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

   <SymAuthorization("RemoveFrsCkd")>
   <Route("ckd/{ckdId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveFrsCkd(ckdId As Integer, lockId As String) As IHttpActionResult

      If ckdId <= 0 Then
         Throw New ArgumentException("RCI ID is required.")
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

            Me.DeleteFrsCkd(ckdId, lockId)

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

   Private Sub LoadFrsCkd(ckd As FrsCkdBody, frsCkd As FrsCkd)

      DataLib.ScatterValues(ckd, frsCkd)

   End Sub

   Private Sub LoadFrsCkd(row As DataRow, ckd As FrsCkd)

      With ckd
         .CkdId = row.ToInt32("CkdId")
         .ReportDate = row.ToDate("ReportDate")
         .DocumentId = row.ToString("DocumentId")
         .AgencyName = row.ToString("AgencyName")
         .FundClusterId = row.ToString("FundClusterId")
         .BankId = row.ToInt32("BankId")
         .TotalAmount = row.ToDecimal("TotalAmount")
         .TaxAmount = row.ToDecimal("TaxAmount")
         .NetAmount = row.ToDecimal("NetAmount")
         .PostUserId = row.ToInt32("PostUserId")
         .CashSignatoryId = row.ToInt32("CashSignatoryId")
      End With

   End Sub

   Private Sub LoadFrsCkdDetailList(rows As DataRowCollection, list As FrsCkdDetailList)

      Dim _detail As FrsCkdDetail
      For Each _row As DataRow In rows
         _detail = New FrsCkdDetail

         With _detail
            .DetailId = _row.ToInt32("DetailId")
            .CkdId = _row.ToInt32("CkdId")
            .DbxId = _row.ToInt32("DbxId")
            .AccountId = _row.ToString("AccountId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertFrsCkd(ckd As FrsCkd)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsCkd", ckd, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsCkd(ckd)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFrsCkdDetails(list As FrsCkdDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FrsCkdDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsCkdDetail In list
            Me.AddInsertUpdateParamsFrsCkdDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateFrsCkd(ckd As FrsCkd)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CkdId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("NetAmount")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsCkd", ckd, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFrsCkd(ckd)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(ckd.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFrsCkdDetails(list As FrsCkdDetailList)

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
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FrsCkdDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FrsCkdDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFrsCkdDetail(_detail)
               .Parameters.AddWithValue("@DetailId", _detail.DetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsCkd(ckd As FrsCkd)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CkdId", ckd.CkdId)
         .AddWithValue("@ReportDate", ckd.ReportDate)
         .AddWithValue("@DocumentId", ckd.DocumentId)
         .AddWithValue("@AgencyName", ckd.AgencyName)
         .AddWithValue("@FundClusterId", ckd.FundClusterId)
         .AddWithValue("@BankId", ckd.BankId)
         .AddWithValue("@TotalAmount", ckd.TotalAmount)
         .AddWithValue("@TaxAmount", ckd.TaxAmount)
         .AddWithValue("@PostUserId", ckd.PostUserId)
         .AddWithValue("@CashSignatoryId", ckd.CashSignatoryId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFrsCkdDetail(dtl As FrsCkdDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CkdId", dtl.CkdId)
         .AddWithValue("@DbxId", dtl.DbxId)
         .AddWithValue("@AccountId", dtl.AccountId)
      End With

   End Sub

   Private Sub DeleteFrsCkd(ckdId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CkdId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FrsCkd", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CkdId", ckdId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFrsCkdDetails(list As FrsCkdDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FrsCkdDetail WHERE DetailId=@DetailId"
         .CommandType = CommandType.Text

         For Each _old As FrsCkdDetail In list
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

   Private Function HasFrsCkdChanges(oldRecord As FrsCkd, newRecord As FrsCkd) As Boolean

      With oldRecord
         If .ReportDate <> newRecord.ReportDate Then Return True
         If .DocumentId <> newRecord.DocumentId Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .BankId <> newRecord.BankId Then Return True
         If .TotalAmount <> newRecord.TotalAmount Then Return True
         If .TaxAmount <> newRecord.TaxAmount Then Return True
         If .CashSignatoryId <> newRecord.CashSignatoryId Then Return True
      End With

      Return False

   End Function

End Class

Public Class FrsCkdBody
   Inherits FrsCkd

   Public Property Details As FrsCkdDetail()

End Class
