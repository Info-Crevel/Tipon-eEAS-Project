<RoutePrefix("api")>
Public Class DBS0270_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetSchools")>
   <Route("schools")>
   <HttpGet>
   Public Function GetSchools() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0270_All")
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

   <SymAuthorization("GetSchool")>
   <Route("schools/{schoolId}")>
   <HttpGet>
   Public Function GetSchool(schoolId As Integer) As IHttpActionResult

      If schoolId <= 0 Then
         Throw New ArgumentException("School ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0270")
            With _direct
               .AddParameter("SchoolId", schoolId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateSchool")>
   <Route("schools/{currentUserId}")>
   <HttpPost>
   Public Function CreateSchool(currentUserId As Integer, <FromBody> school As DbsSchoolBody) As IHttpActionResult

      If school.SchoolId <> -1 Then
         Throw New ArgumentException("School ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _schoolId As Integer = SysLib.GetNextSequence("SchoolId")

         school.SchoolId = _schoolId

         '
         ' Load proposed values from payload
         '
         Dim _dbsSchool As New DbsSchool

         Me.LoadDbsSchool(school, _dbsSchool)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsSchool(_dbsSchool)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSchool(_schoolId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(school.SchoolId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifySchool")>
   <Route("schools/{schoolId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifySchool(schoolId As Integer, currentUserId As Integer, <FromBody> school As DbsSchoolBody) As IHttpActionResult

      If schoolId <= 0 Then
         Throw New ArgumentException("School ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsSchool As New DbsSchool

         Me.LoadDbsSchool(school, _dbsSchool)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsSchool(_dbsSchool)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSchool(schoolId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveSchool")>
   <Route("schools/{schoolId}")>
   <HttpDelete>
   Public Function RemoveSchool(schoolId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If schoolId <= 0 Then
         Throw New ArgumentException("School ID is required.")
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

            Me.DeleteDbsSchool(schoolId, q.LockId)

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

   Private Sub LoadDbsSchool(school As DbsSchoolBody, dbsSchool As DbsSchool)

      DataLib.ScatterValues(school, dbsSchool)

   End Sub

   Private Sub LoadDbsSchool(row As DataRow, dbsSchool As DbsSchool)

      With dbsSchool
         .SchoolId = row.ToInt32("SchoolId")
         .SchoolName = row.ToString("SchoolName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsSchool(school As DbsSchool)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSchool", school, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSchool(school)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsSchool(school As DbsSchool)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SchoolId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSchool", school, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSchool(school)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(school.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsSchool(school As DbsSchool)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SchoolId", school.SchoolId)
         .AddWithValue("@SchoolName", school.SchoolName)
         .AddWithValue("@SortSeq", school.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsSchool(schoolId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SchoolId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsSchool", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SchoolId", schoolId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsSchoolBody
   Inherits DbsSchool

End Class
