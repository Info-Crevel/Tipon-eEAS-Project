<RoutePrefix("api")>
Public Class DBS0510_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0510")>
   <Route("references/dbs0510")>
   <HttpGet>
   Public Function GetReferences_DBS0510() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0510_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "payFrequencies"      ' Daily, Weekly, Semi-Monthly, Monthly
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMatrixWht")>
   <Route("matrix-whts/{whtId}")>
   <HttpGet>
   Public Function GetDbsMatrixWht(whtId As Integer) As IHttpActionResult

      If whtId <= 0 Then
         Throw New ArgumentException("Matrix Wht ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0510")
            With _direct
               .AddParameter("WhtId", whtId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "matrixWht"
                     .Tables(1).TableName = "details"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMatrixWht")>
   <Route("matrix-whts/{currentUserId}")>
   <HttpPost>
   Public Function CreateMatrixWht(currentUserId As Integer, <FromBody> matrixWht As DbsMatrixWhtBody) As IHttpActionResult

      If matrixWht.WhtId <> -1 Then
         Throw New ArgumentException("Matrix Wht ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _whtId As Integer = SysLib.GetNextSequence("WhtId")

         matrixWht.WhtId = _whtId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixWht As New DbsMatrixWht
         Dim _dbsMatrixWhtDetailList As New DbsMatrixWhtDetailList

         Me.LoadDbsMatrixWht(matrixWht, _dbsMatrixWht)

         For Each _detail As DbsMatrixWhtDetail In matrixWht.Details
            _detail.WhtId = _whtId
            _dbsMatrixWhtDetailList.Add(_detail)
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

            Me.InsertDbsMatrixWht(_dbsMatrixWht)

            If _dbsMatrixWhtDetailList.Count > 0 Then
               Me.InsertDbsMatrixWhtDetails(_dbsMatrixWhtDetailList)
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

         'Return Me.Ok(True)
         Return Me.Ok(matrixWht.WhtId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMatrixWht")>
   <Route("matrix-whts/{whtId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMatrixWht(whtId As Integer, currentUserId As Integer, <FromBody> matrixWht As DbsMatrixWhtBody) As IHttpActionResult

      If whtId <= 0 Then
         Throw New ArgumentException("Matrix Wht ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixWht As New DbsMatrixWht
         Dim _dbsMatrixWhtDetailList As New DbsMatrixWhtDetailList

         Me.LoadDbsMatrixWht(matrixWht, _dbsMatrixWht)

         For Each _detail As DbsMatrixWhtDetail In matrixWht.Details
            _dbsMatrixWhtDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsMatrixWhtOld As New DbsMatrixWht
         Dim _dbsMatrixWhtDetailListOld As New DbsMatrixWhtDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsMatrixWht(whtId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("matrixWht").Rows(0)
            Me.LoadDbsMatrixWht(_row, _dbsMatrixWhtOld)
            Me.LoadDbsMatrixWhtDetailList(_dataSet.Tables("details").Rows, _dbsMatrixWhtDetailListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsMatrixWhtDetail In _dbsMatrixWhtDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsMatrixWhtDetail In _dbsMatrixWhtDetailList
               If _new.WhtRangeId = _old.WhtRangeId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If

         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsMatrixWhtDetail In _dbsMatrixWhtDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsMatrixWhtDetail In _dbsMatrixWhtDetailListOld
               If _new.WhtRangeId = _old.WhtRangeId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .PayFreqId <> _old.PayFreqId OrElse .MinAmount <> _old.MinAmount OrElse .MaxAmount <> _old.MaxAmount OrElse .FixedTaxAmount <> _old.FixedTaxAmount OrElse .ExcessRate <> _old.ExcessRate Then
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

         Dim _dbsMatrixWhtDetailListNew As New DbsMatrixWhtDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsMatrixWhtDetail As DbsMatrixWhtDetail

            For Each _new As DbsMatrixWhtDetail In _dbsMatrixWhtDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsMatrixWhtDetail = New DbsMatrixWhtDetail
                  _dbsMatrixWhtDetailListNew.Add(_dbsMatrixWhtDetail)
                  DataLib.ScatterValues(_new, _dbsMatrixWhtDetail)
                  _dbsMatrixWhtDetail.WhtId = _dbsMatrixWht.WhtId
               End If
            Next

         End If

         Dim _isDbsMatrixWhtChanged As Boolean = Me.HasDbsMatrixWhtChanges(_dbsMatrixWhtOld, _dbsMatrixWht)

         If Not _isDbsMatrixWhtChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsMatrixWht(whtId)
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

            If _isDbsMatrixWhtChanged Then
               Me.UpdateDbsMatrixWht(_dbsMatrixWht)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsMatrixWhtDetails(_dbsMatrixWhtDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsMatrixWhtDetails(_dbsMatrixWhtDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsMatrixWhtDetails(_dbsMatrixWhtDetailList)

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

   <SymAuthorization("RemoveMatrixWht")>
    <Route("matrix-whts/{whtId}")>
    <HttpDelete>
   Public Function RemoveMatrixWht(whtId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult
        'File.WriteAllText("d:\1.txt", "1")
        If whtId <= 0 Then
         Throw New ArgumentException("Matrix Wht ID is required.")
      End If
        'File.WriteAllText("d:\2.txt", "1")
        Try
         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean
            'File.WriteAllText("d:\2.txt", "1")
            Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If
                'File.WriteAllText("d:\3.txt", "1")
                _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection
                'File.WriteAllText("d:\4.txt", "1")
                Me.DeleteDbsMatrixWht(whtId, q.LockId)
                'File.WriteAllText("d:\5.txt", "1")
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

   Private Sub LoadDbsMatrixWht(holiday As DbsMatrixWhtBody, dbsMatrixWht As DbsMatrixWht)

      DataLib.ScatterValues(holiday, dbsMatrixWht)

   End Sub

   Private Sub LoadDbsMatrixWht(row As DataRow, matrixWht As DbsMatrixWht)

      With matrixWht
         .WhtId = row.ToInt32("WhtId")
         .WhtName = row.ToString("WhtName")
         .StartDate = row.ToDate("StartDate")
      End With

   End Sub

   Private Sub LoadDbsMatrixWhtDetailList(rows As DataRowCollection, list As DbsMatrixWhtDetailList)

      Dim _detail As DbsMatrixWhtDetail
      For Each _row As DataRow In rows
         _detail = New DbsMatrixWhtDetail

         With _detail
            .WhtId = _row.ToInt32("WhtId")
            .WhtRangeId = _row.ToInt32("WhtRangeId")
            .PayFreqId = _row.ToInt32("PayFreqId")
            .MinAmount = _row.ToDecimal("MinAmount")
            .MaxAmount = _row.ToDecimal("MaxAmount")
            .FixedTaxAmount = _row.ToDecimal("FixedTaxAmount")
            .ExcessRate = _row.ToDecimal("ExcessRate")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertDbsMatrixWht(matrixWht As DbsMatrixWht)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixWht", matrixWht, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixWht(matrixWht)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsMatrixWhtDetails(list As DbsMatrixWhtDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("WhtRangeId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixWhtDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixWhtDetail In list
            Me.AddInsertUpdateParamsDbsMatrixWhtDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub UpdateDbsMatrixWht(matrixWht As DbsMatrixWht)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("WhtId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixWht", matrixWht, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixWht(matrixWht)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(matrixWht.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixWhtDetails(list As DbsMatrixWhtDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("WhtRangeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixWhtDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixWhtDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsMatrixWhtDetail(_detail)
               .Parameters.AddWithValue("@WhtRangeId", _detail.WhtRangeId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixWht(matrixWht As DbsMatrixWht)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@WhtId", matrixWht.WhtId)
         .AddWithValue("@WhtName", matrixWht.WhtName)
         .AddWithValue("@StartDate", matrixWht.StartDate)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixWhtDetail(dtl As DbsMatrixWhtDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@WhtId", dtl.WhtId)
         '.AddWithValue("@WhtRangeId", dtl.WhtRangeId)
         .AddWithValue("@PayFreqId", dtl.PayFreqId)
         .AddWithValue("@MinAmount", dtl.MinAmount)
         .AddWithValue("@MaxAmount", dtl.MaxAmount)
         .AddWithValue("@FixedTaxAmount", dtl.FixedTaxAmount)
         .AddWithValue("@ExcessRate", dtl.ExcessRate)

      End With

   End Sub

   Private Sub DeleteDbsMatrixWht(whtId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("WhtId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixWht", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@WhtId", whtId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsMatrixWhtDetails(list As DbsMatrixWhtDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsMatrixWhtDetail WHERE WhtRangeId=@WhtRangeId"
         .CommandType = CommandType.Text

         For Each _old As DbsMatrixWhtDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@WhtRangeId", _old.WhtRangeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasDbsMatrixWhtChanges(oldRecord As DbsMatrixWht, newRecord As DbsMatrixWht) As Boolean

      With oldRecord
         If .WhtName <> newRecord.WhtName Then Return True
         If .StartDate <> newRecord.StartDate Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsMatrixWhtBody
   Inherits DbsMatrixWht

   Public Property Details As DbsMatrixWhtDetail()

End Class
