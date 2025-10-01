<RoutePrefix("api")>
Public Class DBS0420_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetRnrRecordings")>
   <Route("rnr-recordings")>
   <HttpGet>
   Public Function GetRnrRecordings() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0420_All")
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

   <SymAuthorization("GetRnrRecording")>
   <Route("rnr-recordings/{rnrRecordingId}")>
   <HttpGet>
   Public Function GetRnrRecording(rnrRecordingId As Integer) As IHttpActionResult

      If rnrRecordingId <= 0 Then
         Throw New ArgumentException("RNR Recording ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0420")
            With _direct
               .AddParameter("RnrRecordingId", rnrRecordingId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateRnrRecording")>
   <Route("rnr-recordings/{currentUserId}")>
   <HttpPost>
   Public Function CreateRnrRecording(currentUserId As Integer, <FromBody> rnrRecording As DbsRnrRecordingBody) As IHttpActionResult

      If rnrRecording.RnrRecordingId <> -1 Then
         Throw New ArgumentException("RNR Recording ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _rnrRecordingId As Integer = SysLib.GetNextSequence("RnrRecordingId")

         rnrRecording.RnrRecordingId = _rnrRecordingId

         '
         ' Load proposed values from payload
         '
         Dim _dbsRnrRecording As New DbsRnrRecording

         Me.LoadDbsRnrRecording(rnrRecording, _dbsRnrRecording)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsRnrRecording(_dbsRnrRecording)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRnrRecording(_rnrRecordingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(rnrRecording.RnrRecordingId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyRnrRecording")>
   <Route("rnr-recordings/{rnrRecordingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyRnrRecording(rnrRecordingId As Integer, currentUserId As Integer, <FromBody> rnrRecording As DbsRnrRecordingBody) As IHttpActionResult

      If rnrRecordingId <= 0 Then
         Throw New ArgumentException("RNR Recording ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsRnrRecording As New DbsRnrRecording

         Me.LoadDbsRnrRecording(rnrRecording, _dbsRnrRecording)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsRnrRecording(_dbsRnrRecording)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRnrRecording(rnrRecordingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveRnrRecording")>
   <Route("rnr-recordings/{rnrRecordingId}")>
   <HttpDelete>
   Public Function RemoveRnrRecording(rnrRecordingId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If rnrRecordingId <= 0 Then
         Throw New ArgumentException("RNR Recording ID is required.")
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

            Me.DeleteDbsRnrRecording(rnrRecordingId, q.LockId)

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

   Private Sub LoadDbsRnrRecording(rnrRecording As DbsRnrRecordingBody, dbsRnrRecording As DbsRnrRecording)

      DataLib.ScatterValues(rnrRecording, dbsRnrRecording)

   End Sub

   Private Sub LoadDbsRnrRecording(row As DataRow, dbsRnrRecording As DbsRnrRecording)

      With dbsRnrRecording
         .RnrRecordingId = row.ToInt32("RnrRecordingId")
         .RnrRecordingName = row.ToString("RnrRecordingName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsRnrRecording(rnrRecording As DbsRnrRecording)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRnrRecording", rnrRecording, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRnrRecording(rnrRecording)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsRnrRecording(rnrRecording As DbsRnrRecording)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RnrRecordingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRnrRecording", rnrRecording, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRnrRecording(rnrRecording)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(rnrRecording.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsRnrRecording(rnrRecording As DbsRnrRecording)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@RnrRecordingId", rnrRecording.RnrRecordingId)
         .AddWithValue("@RnrRecordingName", rnrRecording.RnrRecordingName)
         .AddWithValue("@SortSeq", rnrRecording.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsRnrRecording(rnrRecordingId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RnrRecordingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsRnrRecording", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RnrRecordingId", rnrRecordingId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsRnrRecordingBody
   Inherits DbsRnrRecording

End Class
