<RoutePrefix("api")>
Public Class DBS0140_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApplicationSources")>
   <Route("application-sources")>
   <HttpGet>
   Public Function GetApplicationSources() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0140_All")
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

   <SymAuthorization("GetApplicationSource")>
   <Route("application-sources/{applicationSourceId}")>
   <HttpGet>
   Public Function GetApplicationSource(applicationSourceId As Integer) As IHttpActionResult

      If applicationSourceId <= 0 Then
         Throw New ArgumentException("Application Source ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0140")
            With _direct
               .AddParameter("ApplicationSourceId", applicationSourceId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApplicationSource")>
   <Route("application-sources/{currentUserId}")>
   <HttpPost>
   Public Function CreateApplicationSource(currentUserId As Integer, <FromBody> applicationSource As DbsApplicationSourceBody) As IHttpActionResult

      If applicationSource.ApplicationSourceId <> -1 Then
         Throw New ArgumentException("Application Source ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _applicationSourceId As Integer = SysLib.GetNextSequence("ApplicationSourceId")

         applicationSource.ApplicationSourceId = _applicationSourceId

         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicationSource As New DbsApplicationSource

         Me.LoadDbsApplicationSource(applicationSource, _dbsApplicationSource)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsApplicationSource(_dbsApplicationSource)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicationSource(_applicationSourceId), Results.OkNegotiatedContentResult(Of DataTable))

         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApplicationSource")>
   <Route("application-sources/{applicationSourceId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApplicationSource(applicationSourceId As Integer, currentUserId As Integer, <FromBody> applicationSource As DbsApplicationSourceBody) As IHttpActionResult

      If applicationSourceId <= 0 Then
         Throw New ArgumentException("Application Source ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicationSource As New DbsApplicationSource

         Me.LoadDbsApplicationSource(applicationSource, _dbsApplicationSource)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsApplicationSource(_dbsApplicationSource)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicationSource(applicationSourceId), Results.OkNegotiatedContentResult(Of DataTable))

         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveApplicationSource")>
   <Route("application-sources/{applicationSourceId}")>
   <HttpDelete>
   Public Function RemoveApplicationSourc(applicationSourceId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If applicationSourceId <= 0 Then
         Throw New ArgumentException("Application Source ID is required.")
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

            Me.DeleteDbsApplicationSource(applicationSourceId, q.LockId)

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

   Private Sub LoadDbsApplicationSource(applicationSource As DbsApplicationSourceBody, dbsApplicationSource As DbsApplicationSource)

      DataLib.ScatterValues(applicationSource, dbsApplicationSource)

   End Sub

   Private Sub LoadDbsApplicationSource(row As DataRow, dbsApplicationSource As DbsApplicationSource)

      With dbsApplicationSource
         .ApplicationSourceId = row.ToInt32("ApplicationSourceId")
         .ApplicationSourceName = row.ToString("ApplicationSourceName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsApplicationSource(applicationSource As DbsApplicationSource)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsApplicationSource", applicationSource, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicationSource(applicationSource)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsApplicationSource(applicationSource As DbsApplicationSource)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicationSourceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsApplicationSource", applicationSource, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicationSource(applicationSource)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(applicationSource.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsApplicationSource(applicationSource As DbsApplicationSource)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicationSourceId", applicationSource.ApplicationSourceId)
         .AddWithValue("@ApplicationSourceName", applicationSource.ApplicationSourceName)
         .AddWithValue("@SortSeq", applicationSource.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsApplicationSource(applicationSourceId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicationSourceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsApplicationSource", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApplicationSourceId", applicationSourceId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsApplicationSourceBody
   Inherits DbsApplicationSource

End Class
