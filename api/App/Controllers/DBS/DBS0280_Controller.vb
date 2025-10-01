<RoutePrefix("api")>
Public Class DBS0280_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCourses")>
   <Route("courses")>
   <HttpGet>
   Public Function GetCourses() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0280_All")
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

   <SymAuthorization("GetCourse")>
   <Route("courses/{courseId}")>
   <HttpGet>
   Public Function GetCourse(courseId As Integer) As IHttpActionResult

      If courseId <= 0 Then
         Throw New ArgumentException("Course ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0280")
            With _direct
               .AddParameter("CourseId", courseId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateCourse")>
   <Route("courses/{currentUserId}")>
   <HttpPost>
   Public Function CreateCourse(currentUserId As Integer, <FromBody> course As DbsCourseBody) As IHttpActionResult

      If course.CourseId <> -1 Then
         Throw New ArgumentException("Course ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _courseId As Integer = SysLib.GetNextSequence("CourseId")

         course.CourseId = _courseId

         '
         ' Load proposed values from payload
         '
         Dim _dbsCourse As New DbsCourse

         Me.LoadDbsCourse(course, _dbsCourse)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsCourse(_dbsCourse)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCourse(_courseId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(course.CourseId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyCourse")>
   <Route("courses/{courseId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCourse(courseId As Integer, currentUserId As Integer, <FromBody> course As DbsCourseBody) As IHttpActionResult

      If courseId <= 0 Then
         Throw New ArgumentException("Course ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsCourse As New DbsCourse

         Me.LoadDbsCourse(course, _dbsCourse)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsCourse(_dbsCourse)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCourse(courseId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveCourse")>
   <Route("courses/{courseId}")>
   <HttpDelete>
   Public Function RemoveCourse(courseId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If courseId <= 0 Then
         Throw New ArgumentException("Course ID is required.")
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

            Me.DeleteDbsCourse(courseId, q.LockId)

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

   Private Sub LoadDbsCourse(course As DbsCourseBody, dbsCourse As DbsCourse)

      DataLib.ScatterValues(course, dbsCourse)

   End Sub

   Private Sub LoadDbsCourse(row As DataRow, dbsCourse As DbsCourse)

      With dbsCourse
         .CourseId = row.ToInt32("CourseId")
         .CourseName = row.ToString("CourseName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsCourse(course As DbsCourse)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsCourse", course, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCourse(course)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsCourse(course As DbsCourse)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CourseId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsCourse", course, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCourse(course)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(course.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsCourse(course As DbsCourse)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CourseId", course.CourseId)
         .AddWithValue("@CourseName", course.CourseName)
         .AddWithValue("@SortSeq", course.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsCourse(courseId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CourseId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsCourse", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CourseId", courseId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsCourseBody
   Inherits DbsCourse

End Class
