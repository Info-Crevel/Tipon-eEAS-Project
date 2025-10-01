<RoutePrefix("api")>
Public Class DBS0520_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0520")>
   <Route("references/dbs0520")>
   <HttpGet>
   Public Function GetReferences_DBS0520() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0520_References")
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

   <SymAuthorization("GetMatrixSss")>
   <Route("matrix-sss/{sssId}")>
   <HttpGet>
   Public Function GetDbsMatrixSss(sssId As Integer) As IHttpActionResult

      If sssId <= 0 Then
         Throw New ArgumentException("Matrix Sss ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0520")
            With _direct
               .AddParameter("SssId", sssId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "matrixSss"
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

   <SymAuthorization("CreateMatrixSss")>
   <Route("matrix-sss/{currentUserId}")>
   <HttpPost>
   Public Function CreateMatrixSss(currentUserId As Integer, <FromBody> matrixSss As DbsMatrixSssBody) As IHttpActionResult

      If matrixSss.SssId <> -1 Then
         Throw New ArgumentException("Matrix Sss ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _sssId As Integer = SysLib.GetNextSequence("SssId")

         matrixSss.SssId = _sssId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixSss As New DbsMatrixSss
         Dim _dbsMatrixSssDetailList As New DbsMatrixSssDetailList

         Me.LoadDbsMatrixSss(matrixSss, _dbsMatrixSss)

         For Each _detail As DbsMatrixSssDetail In matrixSss.Details
            _detail.SssId = _sssId
            _dbsMatrixSssDetailList.Add(_detail)
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

            Me.InsertDbsMatrixSss(_dbsMatrixSss)

            If _dbsMatrixSssDetailList.Count > 0 Then
               Me.InsertDbsMatrixSssDetails(_dbsMatrixSssDetailList)
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
         Return Me.Ok(matrixSss.SssId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMatrixSss")>
   <Route("matrix-sss/{sssId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMatrixSss(sssId As Integer, currentUserId As Integer, <FromBody> matrixSss As DbsMatrixSssBody) As IHttpActionResult

      If sssId <= 0 Then
         Throw New ArgumentException("Matrix Sss ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixSss As New DbsMatrixSss
         Dim _dbsMatrixSssDetailList As New DbsMatrixSssDetailList

         Me.LoadDbsMatrixSss(matrixSss, _dbsMatrixSss)

         For Each _detail As DbsMatrixSssDetail In matrixSss.Details
            _dbsMatrixSssDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsMatrixSssOld As New DbsMatrixSss
         Dim _dbsMatrixSssDetailListOld As New DbsMatrixSssDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsMatrixSss(sssId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("matrixSss").Rows(0)
            Me.LoadDbsMatrixSss(_row, _dbsMatrixSssOld)
            Me.LoadDbsMatrixSssDetailList(_dataSet.Tables("details").Rows, _dbsMatrixSssDetailListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsMatrixSssDetail In _dbsMatrixSssDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsMatrixSssDetail In _dbsMatrixSssDetailList
               If _new.SssRangeId = _old.SssRangeId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If

         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsMatrixSssDetail In _dbsMatrixSssDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsMatrixSssDetail In _dbsMatrixSssDetailListOld
               If _new.SssRangeId = _old.SssRangeId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MinAmount <> _old.MinAmount OrElse .MaxAmount <> _old.MaxAmount OrElse .SsEmployerAmount <> _old.SsEmployerAmount OrElse .SsEmployeeAmount <> _old.SsEmployeeAmount OrElse .EcEmployerAmount <> _old.EcEmployerAmount OrElse .EcEmployeeAmount <> _old.EcEmployeeAmount OrElse .WispEmployerAmount <> _old.WispEmployerAmount OrElse .WispEmployeeAmount <> _old.WispEmployeeAmount Then
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

         Dim _dbsMatrixSssDetailListNew As New DbsMatrixSssDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsMatrixSssDetail As DbsMatrixSssDetail

            For Each _new As DbsMatrixSssDetail In _dbsMatrixSssDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsMatrixSssDetail = New DbsMatrixSssDetail
                  _dbsMatrixSssDetailListNew.Add(_dbsMatrixSssDetail)
                  DataLib.ScatterValues(_new, _dbsMatrixSssDetail)
                  _dbsMatrixSssDetail.SssId = _dbsMatrixSss.SssId
               End If
            Next

         End If

         Dim _isDbsMatrixSssChanged As Boolean = Me.HasDbsMatrixSssChanges(_dbsMatrixSssOld, _dbsMatrixSss)

         If Not _isDbsMatrixSssChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsMatrixSss(sssId)
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

            If _isDbsMatrixSssChanged Then
               Me.UpdateDbsMatrixSss(_dbsMatrixSss)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsMatrixSssDetails(_dbsMatrixSssDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsMatrixSssDetails(_dbsMatrixSssDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsMatrixSssDetails(_dbsMatrixSssDetailList)
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

   <SymAuthorization("RemoveMatrixSss")>
   <Route("matrix-sss/{sssId}")>
   <HttpDelete>
   Public Function RemoveMatrixSss(sssId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If sssId <= 0 Then
         Throw New ArgumentException("Matrix Sss ID is required.")
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

            Me.DeleteDbsMatrixSss(sssId, q.LockId)

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

   Private Sub LoadDbsMatrixSss(holiday As DbsMatrixSssBody, dbsMatrixSss As DbsMatrixSss)

      DataLib.ScatterValues(holiday, dbsMatrixSss)

   End Sub

   Private Sub LoadDbsMatrixSss(row As DataRow, matrixSss As DbsMatrixSss)

      With matrixSss
         .SssId = row.ToInt32("SssId")
         .SssName = row.ToString("SssName")
         .StartDate = row.ToDate("StartDate")
      End With

   End Sub

   Private Sub LoadDbsMatrixSssDetailList(rows As DataRowCollection, list As DbsMatrixSssDetailList)

      Dim _detail As DbsMatrixSssDetail
      For Each _row As DataRow In rows
         _detail = New DbsMatrixSssDetail

         With _detail
            .SssId = _row.ToInt32("SssId")
            .SssRangeId = _row.ToInt32("SssRangeId")
            .MinAmount = _row.ToDecimal("MinAmount")
            .MaxAmount = _row.ToDecimal("MaxAmount")
            .SsEmployerAmount = _row.ToInt32("SsEmployerAmount")
            .SsEmployeeAmount = _row.ToInt32("SsEmployeeAmount")
            .EcEmployerAmount = _row.ToInt32("EcEmployerAmount")
            .EcEmployeeAmount = _row.ToInt32("EcEmployeeAmount")
            .WispEmployerAmount = _row.ToDecimal("WispEmployerAmount")
            .WispEmployeeAmount = _row.ToDecimal("WispEmployeeAmount")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertDbsMatrixSss(matrixSss As DbsMatrixSss)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixSss", matrixSss, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixSss(matrixSss)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsMatrixSssDetails(list As DbsMatrixSssDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("SssRangeId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixSssDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixSssDetail In list
            Me.AddInsertUpdateParamsDbsMatrixSssDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub UpdateDbsMatrixSss(matrixSss As DbsMatrixSss)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("SssId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixSss", matrixSss, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixSss(matrixSss)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(matrixSss.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixSssDetails(list As DbsMatrixSssDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("SssRangeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixSssDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixSssDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsMatrixSssDetail(_detail)
               .Parameters.AddWithValue("@SssRangeId", _detail.SssRangeId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixSss(matrixSss As DbsMatrixSss)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@SssId", matrixSss.SssId)
         .AddWithValue("@SssName", matrixSss.SssName)
         .AddWithValue("@StartDate", matrixSss.StartDate)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixSssDetail(dtl As DbsMatrixSssDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SssId", dtl.SssId)
         .AddWithValue("@MinAmount", dtl.MinAmount)
         .AddWithValue("@MaxAmount", dtl.MaxAmount)
         .AddWithValue("@SsEmployerAmount", dtl.SsEmployerAmount)
         .AddWithValue("@SsEmployeeAmount", dtl.SsEmployeeAmount)
         .AddWithValue("@EcEmployerAmount", dtl.EcEmployerAmount)
         .AddWithValue("@EcEmployeeAmount", dtl.EcEmployeeAmount)
         .AddWithValue("@WispEmployerAmount", dtl.WispEmployerAmount)
         .AddWithValue("@WispEmployeeAmount", dtl.WispEmployeeAmount)

      End With

   End Sub

   Private Sub DeleteDbsMatrixSss(sssId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SssId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixSss", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SssId", sssId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsMatrixSssDetails(list As DbsMatrixSssDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsMatrixSssDetail WHERE SssRangeId=@SssRangeId"
         .CommandType = CommandType.Text

         For Each _old As DbsMatrixSssDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@SssRangeId", _old.SssRangeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasDbsMatrixSssChanges(oldRecord As DbsMatrixSss, newRecord As DbsMatrixSss) As Boolean

      With oldRecord
         If .SssName <> newRecord.SssName Then Return True
         If .StartDate <> newRecord.StartDate Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsMatrixSssBody
   Inherits DbsMatrixSss

   Public Property Details As DbsMatrixSssDetail()

End Class
