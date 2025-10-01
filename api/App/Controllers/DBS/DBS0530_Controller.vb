<RoutePrefix("api")>
Public Class DBS0530_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0530")>
   <Route("references/dbs0530")>
   <HttpGet>
   Public Function GetReferences_DBS0530() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0530_References")
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

   <SymAuthorization("GetMatrixPbg")>
   <Route("matrix-pbg/{pbgId}")>
   <HttpGet>
   Public Function GetDbsMatrixPbg(pbgId As Integer) As IHttpActionResult

      If pbgId <= 0 Then
         Throw New ArgumentException("Matrix Pbg ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0530")
            With _direct
               .AddParameter("PbgId", pbgId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "matrixPbg"
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

   <SymAuthorization("CreateMatrixPbg")>
   <Route("matrix-pbg/{currentUserId}")>
   <HttpPost>
   Public Function CreateMatrixPbg(currentUserId As Integer, <FromBody> matrixPbg As DbsMatrixPbgBody) As IHttpActionResult

      If matrixPbg.PbgId <> -1 Then
         Throw New ArgumentException("Matrix Pbg ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _pbgId As Integer = SysLib.GetNextSequence("PbgId")

         matrixPbg.PbgId = _pbgId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPbg As New DbsMatrixPbg
         Dim _dbsMatrixPbgDetailList As New DbsMatrixPbgDetailList

         Me.LoadDbsMatrixPbg(matrixPbg, _dbsMatrixPbg)

         For Each _detail As DbsMatrixPbgDetail In matrixPbg.Details
            _detail.PbgId = _pbgId
            _dbsMatrixPbgDetailList.Add(_detail)
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

            Me.InsertDbsMatrixPbg(_dbsMatrixPbg)

            If _dbsMatrixPbgDetailList.Count > 0 Then
               Me.InsertDbsMatrixPbgDetails(_dbsMatrixPbgDetailList)
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
         Return Me.Ok(matrixPbg.PbgId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMatrixPbg")>
   <Route("matrix-pbg/{pbgId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMatrixPbg(pbgId As Integer, currentUserId As Integer, <FromBody> matrixPbg As DbsMatrixPbgBody) As IHttpActionResult

      If pbgId <= 0 Then
         Throw New ArgumentException("Matrix Pbg ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPbg As New DbsMatrixPbg
         Dim _dbsMatrixPbgDetailList As New DbsMatrixPbgDetailList

         Me.LoadDbsMatrixPbg(matrixPbg, _dbsMatrixPbg)

         For Each _detail As DbsMatrixPbgDetail In matrixPbg.Details
            _dbsMatrixPbgDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsMatrixPbgOld As New DbsMatrixPbg
         Dim _dbsMatrixPbgDetailListOld As New DbsMatrixPbgDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsMatrixPbg(pbgId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("matrixPbg").Rows(0)
            Me.LoadDbsMatrixPbg(_row, _dbsMatrixPbgOld)
            Me.LoadDbsMatrixPbgDetailList(_dataSet.Tables("details").Rows, _dbsMatrixPbgDetailListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsMatrixPbgDetail In _dbsMatrixPbgDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsMatrixPbgDetail In _dbsMatrixPbgDetailList
               If _new.PbgRangeId = _old.PbgRangeId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If

         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsMatrixPbgDetail In _dbsMatrixPbgDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsMatrixPbgDetail In _dbsMatrixPbgDetailListOld
               If _new.PbgRangeId = _old.PbgRangeId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MinAmount <> _old.MinAmount OrElse .MaxAmount <> _old.MaxAmount OrElse .MaxRate <> _old.MaxRate OrElse .EmployeeRate <> _old.EmployeeRate OrElse .EmployerRate <> _old.EmployerRate Then
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

         Dim _dbsMatrixPbgDetailListNew As New DbsMatrixPbgDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsMatrixPbgDetail As DbsMatrixPbgDetail

            For Each _new As DbsMatrixPbgDetail In _dbsMatrixPbgDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsMatrixPbgDetail = New DbsMatrixPbgDetail
                  _dbsMatrixPbgDetailListNew.Add(_dbsMatrixPbgDetail)
                  DataLib.ScatterValues(_new, _dbsMatrixPbgDetail)
                  _dbsMatrixPbgDetail.PbgId = _dbsMatrixPbg.PbgId
               End If
            Next

         End If

         Dim _isDbsMatrixPbgChanged As Boolean = Me.HasDbsMatrixPbgChanges(_dbsMatrixPbgOld, _dbsMatrixPbg)

         If Not _isDbsMatrixPbgChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsMatrixPbg(pbgId)
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

            If _isDbsMatrixPbgChanged Then
               Me.UpdateDbsMatrixPbg(_dbsMatrixPbg)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsMatrixPbgDetails(_dbsMatrixPbgDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsMatrixPbgDetails(_dbsMatrixPbgDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsMatrixPbgDetails(_dbsMatrixPbgDetailList)
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

   <SymAuthorization("RemoveMatrixPbg")>
   <Route("matrix-pbg/{pbgId}")>
   <HttpDelete>
   Public Function RemoveMatrixPbg(pbgId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If pbgId <= 0 Then
         Throw New ArgumentException("Matrix Pbg ID is required.")
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

            Me.DeleteDbsMatrixPbg(pbgId, q.LockId)

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

   Private Sub LoadDbsMatrixPbg(holiday As DbsMatrixPbgBody, dbsMatrixPbg As DbsMatrixPbg)

      DataLib.ScatterValues(holiday, dbsMatrixPbg)

   End Sub

   Private Sub LoadDbsMatrixPbg(row As DataRow, matrixPbg As DbsMatrixPbg)

      With matrixPbg
         .PbgId = row.ToInt32("PbgId")
         .PbgName = row.ToString("PbgName")
         .StartDate = row.ToDate("StartDate")
      End With

   End Sub

   Private Sub LoadDbsMatrixPbgDetailList(rows As DataRowCollection, list As DbsMatrixPbgDetailList)

      Dim _detail As DbsMatrixPbgDetail
      For Each _row As DataRow In rows
         _detail = New DbsMatrixPbgDetail

         With _detail
            .PbgId = _row.ToInt32("PbgId")
            .PbgRangeId = _row.ToInt32("PbgRangeId")
            .MinAmount = _row.ToDecimal("MinAmount")
            .MaxAmount = _row.ToDecimal("MaxAmount")
            .EmployeeRate = _row.ToDecimal("EmployeeRate")
            .EmployerRate = _row.ToDecimal("EmployerRate")
            .MaxRate = _row.ToDecimal("MaxRate")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertDbsMatrixPbg(matrixPbg As DbsMatrixPbg)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPbg", matrixPbg, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPbg(matrixPbg)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsMatrixPbgDetails(list As DbsMatrixPbgDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("PbgRangeId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPbgDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixPbgDetail In list
            Me.AddInsertUpdateParamsDbsMatrixPbgDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub UpdateDbsMatrixPbg(matrixPbg As DbsMatrixPbg)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PbgId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPbg", matrixPbg, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPbg(matrixPbg)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(matrixPbg.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixPbgDetails(list As DbsMatrixPbgDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PbgRangeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPbgDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixPbgDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsMatrixPbgDetail(_detail)
               .Parameters.AddWithValue("@PbgRangeId", _detail.PbgRangeId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPbg(matrixPbg As DbsMatrixPbg)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PbgId", matrixPbg.PbgId)
         .AddWithValue("@PbgName", matrixPbg.PbgName)
         .AddWithValue("@StartDate", matrixPbg.StartDate)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPbgDetail(dtl As DbsMatrixPbgDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PbgId", dtl.PbgId)
         .AddWithValue("@MinAmount", dtl.MinAmount)
         .AddWithValue("@MaxAmount", dtl.MaxAmount)
         .AddWithValue("@EmployeeRate", dtl.EmployeeRate)
         .AddWithValue("@EmployerRate", dtl.EmployerRate)
         .AddWithValue("@MaxRate", dtl.MaxRate)
      End With

   End Sub

   Private Sub DeleteDbsMatrixPbg(pbgId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PbgId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixPbg", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PbgId", pbgId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsMatrixPbgDetails(list As DbsMatrixPbgDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsMatrixPbgDetail WHERE PbgRangeId=@PbgRangeId"
         .CommandType = CommandType.Text

         For Each _old As DbsMatrixPbgDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@PbgRangeId", _old.PbgRangeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasDbsMatrixPbgChanges(oldRecord As DbsMatrixPbg, newRecord As DbsMatrixPbg) As Boolean

      With oldRecord
         If .PbgName <> newRecord.PbgName Then Return True
         If .StartDate <> newRecord.StartDate Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsMatrixPbgBody
   Inherits DbsMatrixPbg

   Public Property Details As DbsMatrixPbgDetail()

End Class
