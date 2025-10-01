<RoutePrefix("api")>
Public Class DBS0520_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetSssRanges")>
   <Route("sss-ranges")>
   <HttpGet>
   Public Function GetSssRanges() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0520_All")
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

   <SymAuthorization("GetSssRange")>
   <Route("sss-ranges/{rangeId}")>
   <HttpGet>
   Public Function GetSssRange(rangeId As Integer) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0520")
            With _direct
               .AddParameter("SssRangeId", rangeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateSssRange")>
   <Route("sss-ranges/{currentUserId}")>
   <HttpPost>
   Public Function CreateSssRange(currentUserId As Integer, <FromBody> range As DbsMatrixSssBody) As IHttpActionResult

      If range.SssRangeId <> -1 Then
         Throw New ArgumentException("Range ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _rangeId As Integer = SysLib.GetNextSequence("SssRangeId")

         range.SssRangeId = _rangeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixSss As New DbsMatrixSss

         Me.LoadDbsMatrixSss(range, _dbsMatrixSss)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSssRange(_rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifySssRange")>
   <Route("sss-ranges/{rangeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifySssRange(rangeId As Integer, currentUserId As Integer, <FromBody> range As DbsMatrixSssBody) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixSss As New DbsMatrixSss

         Me.LoadDbsMatrixSss(range, _dbsMatrixSss)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsMatrixSss(_dbsMatrixSss)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSssRange(rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveSssRange")>
   <Route("sss-ranges/{rangeId}")>
   <HttpDelete>
   Public Function RemoveSssRange(rangeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

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

            Me.DeleteSssRange(rangeId, q.LockId)

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

   Private Sub LoadDbsMatrixSss(range As DbsMatrixSssBody, dbsMatrixSss As DbsMatrixSss)

      DataLib.ScatterValues(range, dbsMatrixSss)

   End Sub

   Private Sub LoadDbsMatrixSss(row As DataRow, dbsMatrixSss As DbsMatrixSss)

      With dbsMatrixSss
         .SssRangeId = row.ToInt32("SssRangeId")
         .MinAmount = row.ToDecimal("MinAmount")
         .MaxAmount = row.ToDecimal("MaxAmount")
         .SsEmployerAmount = row.ToInt32("SsEmployerAmount")
         .SsEmployeeAmount = row.ToInt32("SsEmployeeAmount")
         .EcEmployerAmount = row.ToInt32("EcEmployerAmount")
         .WispEmployerAmount = row.ToDecimal("WispEmployerAmount")
         .WispEmployeeAmount = row.ToDecimal("WispEmployeeAmount")
      End With

   End Sub

   Private Sub InsertDbsMatrixSss(range As DbsMatrixSss)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixSss", range, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixSss(range)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixSss(range As DbsMatrixSss)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SssRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixSss", range, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixSss(range)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(range.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixSss(range As DbsMatrixSss)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SssRangeId", range.SssRangeId)
         .AddWithValue("@MinAmount", range.MinAmount)
         .AddWithValue("@MaxAmount", range.MaxAmount)
         .AddWithValue("@SsEmployerAmount", range.SsEmployerAmount)
         .AddWithValue("@SsEmployeeAmount", range.SsEmployeeAmount)
         .AddWithValue("@EcEmployerAmount", range.EcEmployerAmount)
         .AddWithValue("@WispEmployerAmount", range.WispEmployerAmount)
         .AddWithValue("@WispEmployeeAmount", range.WispEmployeeAmount)

      End With

   End Sub

   Private Sub DeleteSssRange(rangeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SssRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixSss", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SssRangeId", rangeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsMatrixSssBody
   Inherits DbsMatrixSss

End Class

