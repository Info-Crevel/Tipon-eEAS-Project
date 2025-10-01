<RoutePrefix("api")>
Public Class DBS0530_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPbgRanges")>
   <Route("pbg-ranges")>
   <HttpGet>
   Public Function GetPbgRanges() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0530_All")
            With _direct
               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetPbgRange")>
   <Route("pbg-ranges/{rangeId}")>
   <HttpGet>
   Public Function GetPbgRange(rangeId As Integer) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0530")
            With _direct
               .AddParameter("PbgRangeId", rangeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePbgRange")>
   <Route("pbg-ranges/{currentUserId}")>
   <HttpPost>
   Public Function CreatePbgRange(currentUserId As Integer, <FromBody> range As DbsMatrixPbgBody) As IHttpActionResult

      If range.PbgRangeId <> -1 Then
         Throw New ArgumentException("Range ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _rangeId As Integer = SysLib.GetNextSequence("PbgRangeId")

         range.PbgRangeId = _rangeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPbg As New DbsMatrixPbg

         Me.LoadDbsMatrixPbg(range, _dbsMatrixPbg)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPbgRange(_rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPbgRange")>
   <Route("pbg-ranges/{rangeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPbgRange(rangeId As Integer, currentUserId As Integer, <FromBody> range As DbsMatrixPbgBody) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPbg As New DbsMatrixPbg

         Me.LoadDbsMatrixPbg(range, _dbsMatrixPbg)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsMatrixPbg(_dbsMatrixPbg)

            _successFlag = True

         Catch _exception As Exception
            'File.WriteAllText("d:\yyy.txt", _exception.Message)
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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPbgRange(rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePbgRange")>
   <Route("pbg-ranges/{rangeId}")>
   <HttpDelete>
   Public Function RemovePbgRange(rangeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
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

            Me.DeletePbgRange(rangeId, q.LockId)

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

   Private Sub LoadDbsMatrixPbg(range As DbsMatrixPbgBody, dbsMatrixPbg As DbsMatrixPbg)

      DataLib.ScatterValues(range, dbsMatrixPbg)

   End Sub

   Private Sub LoadDbsMatrixPbg(row As DataRow, dbsMatrixPbg As DbsMatrixPbg)

      With dbsMatrixPbg
         .PbgRangeId = row.ToInt32("PbgRangeId")
         .MinAmount = row.ToDecimal("MinAmount")
         .MaxAmount = row.ToDecimal("MaxAmount")
         .EmployeeRate = row.ToDecimal("EmployeeRate")
         .EmployerRate = row.ToDecimal("EmployerRate")
      End With

   End Sub

   Private Sub InsertDbsMatrixPbg(range As DbsMatrixPbg)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPbg", range, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPbg(range)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixPbg(range As DbsMatrixPbg)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PbgRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPbg", range, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPbg(range)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(range.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPbg(range As DbsMatrixPbg)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PbgRangeId", range.PbgRangeId)
         .AddWithValue("@MinAmount", range.MinAmount)
         .AddWithValue("@MaxAmount", range.MaxAmount)
         .AddWithValue("@EmployeeRate", range.EmployeeRate)
         .AddWithValue("@EmployerRate", range.EmployerRate)

      End With

   End Sub

   Private Sub DeletePbgRange(rangeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PbgRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixPbg", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PbgRangeId", rangeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsMatrixPbgBody
   Inherits DbsMatrixPbg

End Class
