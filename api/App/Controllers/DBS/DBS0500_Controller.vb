<RoutePrefix("api")>
Public Class DBS0500_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetSkills")>
   <Route("skills")>
   <HttpGet>
   Public Function GetSkills() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0500_All")
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

   <SymAuthorization("GetSkill")>
   <Route("skills/{skillId}")>
   <HttpGet>
   Public Function GetSkill(skillId As Integer) As IHttpActionResult

      If skillId <= 0 Then
         Throw New ArgumentException("Skill ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0500")
            With _direct
               .AddParameter("SkillId", skillId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateSkill")>
   <Route("skills/{currentUserId}")>
   <HttpPost>
   Public Function CreateSkill(currentUserId As Integer, <FromBody> skill As DbsSkillBody) As IHttpActionResult

      If skill.SkillId <> -1 Then
         Throw New ArgumentException("Skill ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _skillId As Integer = SysLib.GetNextSequence("SkillId")

         skill.SkillId = _skillId

         '
         ' Load proposed values from payload
         '
         Dim _dbsSkill As New DbsSkill

         Me.LoadDbsSkill(skill, _dbsSkill)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsSkill(_dbsSkill)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSkill(_skillId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifySkill")>
   <Route("skills/{skillId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifySkill(skillId As Integer, currentUserId As Integer, <FromBody> skill As DbsSkillBody) As IHttpActionResult

      If skillId <= 0 Then
         Throw New ArgumentException("Skill ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsSkill As New DbsSkill

         Me.LoadDbsSkill(skill, _dbsSkill)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsSkill(_dbsSkill)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSkill(skillId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveSkill")>
   <Route("skills/{skillId}")>
   <HttpDelete>
   Public Function RemoveSkill(skillId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If skillId <= 0 Then
         Throw New ArgumentException("Skill ID is required.")
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

            Me.DeleteDbsSkill(skillId, q.LockId)

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

   Private Sub LoadDbsSkill(skill As DbsSkillBody, dbsSkill As DbsSkill)

      DataLib.ScatterValues(skill, dbsSkill)

   End Sub

   Private Sub LoadDbsSkill(row As DataRow, dbsSkill As DbsSkill)

      With dbsSkill
         .SkillId = row.ToInt32("SkillId")
         .SkillName = row.ToString("SkillName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub
   Private Sub InsertDbsSkill(skill As DbsSkill)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSkill", skill, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSkill(skill)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsSkill(skill As DbsSkill)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SkillId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSkill", skill, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSkill(skill)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(skill.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsSkill(skill As DbsSkill)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SkillId", skill.SkillId)
         .AddWithValue("@SkillName", skill.SkillName)
         .AddWithValue("@SortSeq", skill.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsSkill(skillId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SkillId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsSkill", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SkillId", skillId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsSkillBody
   Inherits DbsSkill

End Class
