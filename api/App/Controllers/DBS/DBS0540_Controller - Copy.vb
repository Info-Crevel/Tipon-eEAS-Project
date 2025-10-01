<RoutePrefix("api")>
Public Class DBS0540_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPhhRanges")>
   <Route("phh-ranges")>
   <HttpGet>
   Public Function GetPhhRanges() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0540_All")
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

   <SymAuthorization("GetPhhRange")>
   <Route("phh-ranges/{rangeId}")>
   <HttpGet>
   Public Function GetPhhRange(rangeId As Integer) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0540")
            With _direct
               .AddParameter("PhhRangeId", rangeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePhhRange")>
   <Route("phh-ranges/{currentUserId}")>
   <HttpPost>
   Public Function CreatePhhRange(currentUserId As Integer, <FromBody> range As DbsMatrixPhhBody) As IHttpActionResult

      If range.PhhRangeId <> -1 Then
         Throw New ArgumentException("Range ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _rangeId As Integer = SysLib.GetNextSequence("PhhRangeId")

         range.PhhRangeId = _rangeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPhh As New DbsMatrixPhh

         Me.LoadDbsMatrixPhh(range, _dbsMatrixPhh)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsMatrixPhh(_dbsMatrixPhh)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPhhRange(_rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPhhRange")>
   <Route("phh-ranges/{rangeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPhhRange(rangeId As Integer, currentUserId As Integer, <FromBody> range As DbsMatrixPhhBody) As IHttpActionResult

      If rangeId <= 0 Then
         Throw New ArgumentException("Range ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPhh As New DbsMatrixPhh

         Me.LoadDbsMatrixPhh(range, _dbsMatrixPhh)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsMatrixPhh(_dbsMatrixPhh)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPhhRange(rangeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePhhRange")>
   <Route("phh-ranges/{rangeId}")>
   <HttpDelete>
   Public Function RemovePhhRange(rangeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

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

            Me.DeletePhhRange(rangeId, q.LockId)

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

   Private Sub LoadDbsMatrixPhh(range As DbsMatrixPhhBody, dbsMatrixPhh As DbsMatrixPhh)

      DataLib.ScatterValues(range, dbsMatrixPhh)

   End Sub

   Private Sub LoadDbsMatrixPhh(row As DataRow, dbsMatrixPhh As DbsMatrixPhh)

      With dbsMatrixPhh
         .PhhRangeId = row.ToInt32("PhhRangeId")
         .MinAmount = row.ToDecimal("MinAmount")
         .MaxAmount = row.ToDecimal("MaxAmount")
         .PremiumRate = row.ToDecimal("PremiumRate")
         .FixedAmount = row.ToDecimal("FixedAmount")
      End With

   End Sub

   Private Sub InsertDbsMatrixPhh(range As DbsMatrixPhh)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPhh", range, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPhh(range)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixPhh(range As DbsMatrixPhh)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PhhRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPhh", range, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPhh(range)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(range.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPhh(range As DbsMatrixPhh)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PhhRangeId", range.PhhRangeId)
         .AddWithValue("@MinAmount", range.MinAmount)
         .AddWithValue("@MaxAmount", range.MaxAmount)
         .AddWithValue("@PremiumRate", range.PremiumRate)
         .AddWithValue("@FixedAmount", range.FixedAmount)

      End With

   End Sub

   Private Sub DeletePhhRange(rangeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PhhRangeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixPhh", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PhhRangeId", rangeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsMatrixPhhBody
   Inherits DbsMatrixPhh

End Class
