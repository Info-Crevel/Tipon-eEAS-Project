<RoutePrefix("api")>
Public Class DBS0040_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetRelations")>
   <Route("relations")>
   <HttpGet>
   Public Function GetRelations() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0040_All")
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

   <SymAuthorization("GetRelation")>
   <Route("relations/{relationId}")>
   <HttpGet>
   Public Function GetRelation(relationId As Integer) As IHttpActionResult

      If relationId <= 0 Then
         Throw New ArgumentException("Relation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0040")
            With _direct
               .AddParameter("RelationId", relationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateRelation")>
   <Route("relations/{currentUserId}")>
   <HttpPost>
   Public Function CreateRelation(currentUserId As Integer, <FromBody> relation As DbsRelationBody) As IHttpActionResult

      If relation.RelationId <> -1 Then
         Throw New ArgumentException("Relation ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _relationId As Integer = SysLib.GetNextSequence("RelationId")

         relation.RelationId = _relationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsRelation As New DbsRelation

         Me.LoadDbsRelation(relation, _dbsRelation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsRelation(_dbsRelation)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRelation(_relationId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyRelation")>
   <Route("relations/{relationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyRelation(relationId As Integer, currentUserId As Integer, <FromBody> relation As DbsRelationBody) As IHttpActionResult

      If relationId <= 0 Then
         Throw New ArgumentException("Relation ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsRelation As New DbsRelation

         Me.LoadDbsRelation(relation, _dbsRelation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsRelation(_dbsRelation)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRelation(relationId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveRelation")>
   <Route("relations/{relationId}")>
   <HttpDelete>
   Public Function RemoveRelation(relationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If relationId <= 0 Then
         Throw New ArgumentException("Relation ID is required.")
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

            Me.DeleteDbsRelation(relationId, q.LockId)

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

   Private Sub LoadDbsRelation(relation As DbsRelationBody, dbsRelation As DbsRelation)

      DataLib.ScatterValues(relation, dbsRelation)

   End Sub

   Private Sub LoadDbsRelation(row As DataRow, dbsRelation As DbsRelation)

        With dbsRelation
            .RelationId = row.ToInt32("RelationId")
            .RelationName = row.ToString("RelationName")
            .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
            .OccupationRequiredFlag = row.ToBoolean("OccupationRequiredFlag")
            .EmailRequiredFlag = row.ToBoolean("EmailRequiredFlag")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

   Private Sub InsertDbsRelation(relation As DbsRelation)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRelation", relation, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRelation(relation)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsRelation(relation As DbsRelation)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RelationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRelation", relation, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRelation(relation)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(relation.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsRelation(relation As DbsRelation)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@RelationId", relation.RelationId)
            .AddWithValue("@RelationName", relation.RelationName)
            .AddWithValue("@UploadRequiredFlag", relation.UploadRequiredFlag)
            .AddWithValue("@OccupationRequiredFlag", relation.OccupationRequiredFlag)
            .AddWithValue("@EmailRequiredFlag", relation.EmailRequiredFlag)
            .AddWithValue("@SortSeq", relation.SortSeq)

        End With

   End Sub

   Private Sub DeleteDbsRelation(relationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RelationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsRelation", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RelationId", relationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsRelationBody
   Inherits DbsRelation

End Class

