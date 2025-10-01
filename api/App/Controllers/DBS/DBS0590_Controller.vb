<RoutePrefix("api")>
Public Class DBS0590_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApplicantScreening")>
   <Route("applicant-screenings")>
   <HttpGet>
   Public Function GetRequestType() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0590_All")
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

   <SymAuthorization("GetApplicantScreening")>
   <Route("applicant-screenings/{applicantScreeningId}")>
   <HttpGet>
   Public Function GetApplicantScreening(applicantScreeningId As Integer) As IHttpActionResult

      If applicantScreeningId <= 0 Then
         Throw New ArgumentException("Applicant Screening ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0590")
            With _direct
               .AddParameter("ApplicantScreeningId", applicantScreeningId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApplicantScreening")>
   <Route("applicant-screenings/{currentUserId}")>
   <HttpPost>
   Public Function CreateApplicantScreening(currentUserId As Integer, <FromBody> screening As DbsApplicantScreeningBody) As IHttpActionResult

      If screening.ApplicantScreeningId <> -1 Then
         Throw New ArgumentException("Applicant Screening ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _applicantScreeningId As Integer = SysLib.GetNextSequence("ApplicantScreeningId")

         screening.ApplicantScreeningId = _applicantScreeningId

         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicantScreening As New DbsApplicantScreening

         Me.LoadDbsApplicantScreening(screening, _dbsApplicantScreening)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsApplicantScreening(_dbsApplicantScreening)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicantScreening(_applicantScreeningId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(requestType.RequestTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApplicantScreening")>
   <Route("applicant-screenings/{applicantScreeningId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApplicantScreening(applicantScreeningId As Integer, currentUserId As Integer, <FromBody> screening As DbsApplicantScreeningBody) As IHttpActionResult

      If applicantScreeningId <= 0 Then
         Throw New ArgumentException("Applicant Screening ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicantScreening As New DbsApplicantScreening

         Me.LoadDbsApplicantScreening(screening, _dbsApplicantScreening)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsApplicantScreening(_dbsApplicantScreening)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicantScreening(applicantScreeningId), Results.OkNegotiatedContentResult(Of DataTable))

         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveApplicantScreening")>
   <Route("applicant-screenings/{applicantScreeningId}")>
   <HttpDelete>
   Public Function RemoveApplicantScreening(applicantScreeningId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If applicantScreeningId <= 0 Then
         Throw New ArgumentException("Applicant Screening ID is required.")
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

            Me.DeleteDbsApplicantScreening(applicantScreeningId, q.LockId)

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

   Private Sub LoadDbsApplicantScreening(screening As DbsApplicantScreeningBody, dbsApplicantScreening As DbsApplicantScreening)

      DataLib.ScatterValues(screening, dbsApplicantScreening)

   End Sub

   Private Sub LoadDbsApplicantScreening(row As DataRow, dbsApplicantScreening As DbsApplicantScreening)

      With dbsApplicantScreening
         .ApplicantScreeningId = row.ToInt32("ApplicantScreeningId")
         .ApplicantScreeningName = row.ToString("ApplicantScreeningName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsApplicantScreening(screening As DbsApplicantScreening)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsApplicantScreening", screening, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicantScreening(screening)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsApplicantScreening(screening As DbsApplicantScreening)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantScreeningId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsApplicantScreening", screening, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicantScreening(screening)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(screening.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsApplicantScreening(screening As DbsApplicantScreening)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicantScreeningId", screening.ApplicantScreeningId)
         .AddWithValue("@ApplicantScreeningName", screening.ApplicantScreeningName)
         .AddWithValue("@UploadRequiredFlag", screening.UploadRequiredFlag)
         .AddWithValue("@SortSeq", screening.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsApplicantScreening(applicantScreeningId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantScreeningId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsApplicantScreening", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApplicantScreeningId", applicantScreeningId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsApplicantScreeningBody
   Inherits DbsApplicantScreening

End Class
