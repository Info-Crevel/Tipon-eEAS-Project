<RoutePrefix("api")>
Public Class DBS0230_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAssessmentCenters")>
   <Route("assessment-centers")>
   <HttpGet>
   Public Function GetAssessmentCenters() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0230_All")
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

   <SymAuthorization("GetAssessmentCenter")>
   <Route("assessment-centers/{assessmentCenterId}")>
   <HttpGet>
   Public Function GetAssessmentCenter(assessmentCenterId As Integer) As IHttpActionResult

      If assessmentCenterId <= 0 Then
         Throw New ArgumentException("Assessment Center ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0230")
            With _direct
               .AddParameter("AssessmentCenterId", assessmentCenterId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAssessmentCenter")>
   <Route("assessment-centers/{currentUserId}")>
   <HttpPost>
   Public Function CreateAssessmentCenter(currentUserId As Integer, <FromBody> assessmentCenter As DbsAssessmentCenterBody) As IHttpActionResult

      If assessmentCenter.AssessmentCenterId <> -1 Then
         Throw New ArgumentException("Assessment Center ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _assessmentCenterId As Integer = SysLib.GetNextSequence("AssessmentCenterId")

         assessmentCenter.AssessmentCenterId = _assessmentCenterId

         '
         ' Load proposed values from payload
         '
         Dim _dbsAssessmentCenter As New DbsAssessmentCenter

         Me.LoadDbsAssessmentCenter(assessmentCenter, _dbsAssessmentCenter)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsAssessmentCenter(_dbsAssessmentCenter)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAssessmentCenter(_assessmentCenterId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(assessmentCenter.AssessmentCenterId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAssessmentCenter")>
   <Route("assessment-centers/{assessmentCenterId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAssessmentCenter(assessmentCenterId As Integer, currentUserId As Integer, <FromBody> assessmentCenter As DbsAssessmentCenterBody) As IHttpActionResult

      If assessmentCenterId <= 0 Then
         Throw New ArgumentException("Assessment Center ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsAssessmentCenter As New DbsAssessmentCenter

         Me.LoadDbsAssessmentCenter(assessmentCenter, _dbsAssessmentCenter)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsAssessmentCenter(_dbsAssessmentCenter)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAssessmentCenter(assessmentCenterId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveAssessmentCenter")>
   <Route("assessment-centers/{assessmentCenterId}")>
   <HttpDelete>
   Public Function RemoveAssessmentCenter(assessmentCenterId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If assessmentCenterId <= 0 Then
         Throw New ArgumentException("Assessment Center ID is required.")
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

            Me.DeleteDbsAssessmentCenter(assessmentCenterId, q.LockId)

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

   Private Sub LoadDbsAssessmentCenter(assessmentCenter As DbsAssessmentCenterBody, dbsAssessmentCenter As DbsAssessmentCenter)

      DataLib.ScatterValues(assessmentCenter, dbsAssessmentCenter)

   End Sub

   Private Sub LoadDbsAssessmentCenter(row As DataRow, dbsAssessmentCenter As DbsAssessmentCenter)

      With dbsAssessmentCenter
         .AssessmentCenterId = row.ToInt32("AssessmentCenterId")
         .AssessmentCenterName = row.ToString("AssessmentCenterName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsAssessmentCenter(assessmentCenter As DbsAssessmentCenter)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsAssessmentCenter", assessmentCenter, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAssessmentCenter(assessmentCenter)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsAssessmentCenter(assessmentCenter As DbsAssessmentCenter)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AssessmentCenterId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsAssessmentCenter", assessmentCenter, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAssessmentCenter(assessmentCenter)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(assessmentCenter.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsAssessmentCenter(assessmentCenter As DbsAssessmentCenter)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AssessmentCenterId", assessmentCenter.AssessmentCenterId)
         .AddWithValue("@AssessmentCenterName", assessmentCenter.AssessmentCenterName)
         .AddWithValue("@SortSeq", assessmentCenter.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsAssessmentCenter(assessmentCenterId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AssessmentCenterId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsAssessmentCenter", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AssessmentCenterId", assessmentCenterId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsAssessmentCenterBody
   Inherits DbsAssessmentCenter

End Class
