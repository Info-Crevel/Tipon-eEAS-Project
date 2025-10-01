<RoutePrefix("api")>
Public Class DBS0100_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetDisabilities")>
   <Route("disabilities")>
   <HttpGet>
   Public Function GetDisabilities() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0100_All")
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

   <SymAuthorization("GetDisability")>
   <Route("disabilities/{disabilityId}")>
   <HttpGet>
   Public Function GetDisability(disabilityId As Integer) As IHttpActionResult

      If disabilityId <= 0 Then
         Throw New ArgumentException("Disability ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0100")
            With _direct
               .AddParameter("disabilityId", disabilityId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateDisability")>
   <Route("disabilities/{currentUserId}")>
   <HttpPost>
   Public Function CreateDisability(currentUserId As Integer, <FromBody> disability As DbsDisabilityBody) As IHttpActionResult

      If disability.DisabilityId <> -1 Then
         Throw New ArgumentException("Disability ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _disabilityId As Integer = SysLib.GetNextSequence("DisabilityId")

         disability.DisabilityId = _disabilityId

         '
         ' Load proposed values from payload
         '
         Dim _dbsDisability As New DbsDisability

         Me.LoadDbsDisability(disability, _dbsDisability)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsDisability(_dbsDisability)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDisability(_disabilityId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyDisability")>
   <Route("disabilities/{disabilityId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyDisability(disabilityId As Integer, currentUserId As Integer, <FromBody> Disability As DbsDisabilityBody) As IHttpActionResult

      If disabilityId <= 0 Then
         Throw New ArgumentException("Disability ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsDisability As New DbsDisability

         Me.LoadDbsDisability(Disability, _dbsDisability)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsDisability(_dbsDisability)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDisability(disabilityId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveDisability")>
   <Route("disabilities/{disabilityId}")>
   <HttpDelete>
   Public Function RemoveDisability(disabilityId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If disabilityId <= 0 Then
         Throw New ArgumentException("Disability ID is required.")
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

            Me.DeleteDbsDisability(disabilityId, q.LockId)

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

   Private Sub LoadDbsDisability(disability As DbsDisabilityBody, dbsDisability As DbsDisability)

      DataLib.ScatterValues(disability, dbsDisability)

   End Sub

   Private Sub LoadDbsDisability(row As DataRow, dbsDisability As DbsDisability)

      With dbsDisability
         .DisabilityId = row.ToInt32("DisabilityId")
         .DisabilityName = row.ToString("DisabilityName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsDisability(disability As DbsDisability)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsDisability", disability, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDisability(disability)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsDisability(disability As DbsDisability)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DisabilityId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsDisability", disability, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDisability(disability)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(disability.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsDisability(disability As DbsDisability)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DisabilityId", disability.DisabilityId)
         .AddWithValue("@DisabilityName", disability.DisabilityName)
         .AddWithValue("@SortSeq", disability.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsDisability(disabilityId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DisabilityId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsDisability", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DisabilityId", disabilityId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsDisabilityBody
   Inherits DbsDisability

End Class

