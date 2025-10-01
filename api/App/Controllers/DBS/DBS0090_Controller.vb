<RoutePrefix("api")>
Public Class DBS0090_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetEducationLevels")>
   <Route("education-levels")>
   <HttpGet>
   Public Function GetEducationLevels() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0090_All")
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

   <SymAuthorization("GetEducationLevel")>
   <Route("education-levels/{educationLevelId}")>
   <HttpGet>
   Public Function GetEducationLevel(educationLevelId As Integer) As IHttpActionResult

      If educationLevelId <= 0 Then
         Throw New ArgumentException("Education Level ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0090")
            With _direct
               .AddParameter("EducationLevelId", educationLevelId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateEducationLevel")>
   <Route("education-levels/{currentUserId}")>
   <HttpPost>
   Public Function CreateEducationLevel(currentUserId As Integer, <FromBody> educationLevel As DbsEducationLevelBody) As IHttpActionResult

      If educationLevel.EducationLevelId <> -1 Then
         Throw New ArgumentException("Education Level ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _educationLevelId As Integer = SysLib.GetNextSequence("EducationLevelId")

         educationLevel.EducationLevelId = _educationLevelId

         '
         ' Load proposed values from payload
         '
         Dim _dbsEducationLevel As New DbsEducationLevel

         Me.LoadDbsEducationLevel(educationLevel, _dbsEducationLevel)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsEducationLevel(_dbsEducationLevel)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEducationLevel(_educationLevelId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyEducationLevel")>
   <Route("education-levels/{educationLevelId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyEducationLevel(educationLevelId As Integer, currentUserId As Integer, <FromBody> educationLevel As DbsEducationLevelBody) As IHttpActionResult

      If educationLevelId <= 0 Then
         Throw New ArgumentException("Education Level ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsEducationLevel As New DbsEducationLevel

         Me.LoadDbsEducationLevel(educationLevel, _dbsEducationLevel)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsEducationLevel(_dbsEducationLevel)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEducationLevel(educationLevelId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveEducationLevel")>
   <Route("education-levels/{educationLevelId}")>
   <HttpDelete>
   Public Function RemoveEducationLevel(educationLevelId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If educationLevelId <= 0 Then
         Throw New ArgumentException("Education Level ID is required.")
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

            Me.DeleteDbsEducationLevel(educationLevelId, q.LockId)

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

   Private Sub LoadDbsEducationLevel(educationLevel As DbsEducationLevelBody, dbsEducationLevel As DbsEducationLevel)

      DataLib.ScatterValues(educationLevel, dbsEducationLevel)

   End Sub

   Private Sub LoadDbsEducationLevel(row As DataRow, dbsEducationLevel As DbsEducationLevel)

      With dbsEducationLevel
         .EducationLevelId = row.ToInt32("EducationLevelId")
         .EducationLevelName = row.ToString("EducationLevelName")
         .CourseNameRequiredFlag = row.ToBoolean("CourseNameRequiredFlag")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsEducationLevel(educationLevel As DbsEducationLevel)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsEducationLevel", educationLevel, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsEducationLevel(educationLevel)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsEducationLevel(educationLevel As DbsEducationLevel)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("EducationLevelId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsEducationLevel", educationLevel, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsEducationLevel(educationLevel)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(educationLevel.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsEducationLevel(educationLevel As DbsEducationLevel)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@EducationLevelId", educationLevel.EducationLevelId)
         .AddWithValue("@EducationLevelName", educationLevel.EducationLevelName)
         .AddWithValue("@CourseNameRequiredFlag", educationLevel.CourseNameRequiredFlag)
         .AddWithValue("@UploadRequiredFlag", educationLevel.UploadRequiredFlag)

         .AddWithValue("@SortSeq", educationLevel.SortSeq)

      End With

   End Sub
   Private Sub DeleteDbsEducationLevel(educationLevelId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("EducationLevelId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsEducationLevel", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@EducationLevelId", educationLevelId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsEducationLevelBody
   Inherits DbsEducationLevel

End Class

