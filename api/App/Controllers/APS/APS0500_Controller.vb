<RoutePrefix("api")>
Public Class APS0500_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReviewers")>
   <Route("reviewers")>
   <HttpGet>
   Public Function GetReviewers() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0500_All")
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

   <SymAuthorization("GetReviewer")>
   <Route("reviewers/{reviewerId}")>
   <HttpGet>
   Public Function GetReviewer(reviewerId As Integer) As IHttpActionResult

      If reviewerId <= 0 Then
         Throw New ArgumentException("Reviewer ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0500")
            With _direct
               .AddParameter("ReviewerId", reviewerId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateReviewer")>
   <Route("reviewers/{currentUserId}")>
   <HttpPost>
   Public Function CreateReviewer(currentUserId As Integer, <FromBody> reviewer As ApsReviewerBody) As IHttpActionResult

      If reviewer.ReviewerId <> -1 Then
         Throw New ArgumentException("Reviewer ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _reviewerId As Integer = SysLib.GetNextSequence("ReviewerId")

         reviewer.ReviewerId = _reviewerId

         '
         ' Load proposed values from payload
         '
         Dim _apsReviewer As New ApsReviewer

         Me.LoadApsReviewer(reviewer, _apsReviewer)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsReviewer(_apsReviewer)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetReviewer(_reviewerId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(reviewer.ReviewerId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyReviewer")>
   <Route("reviewers/{reviewerId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyReviewer(reviewerId As Integer, currentUserId As Integer, <FromBody> reviewer As ApsReviewerBody) As IHttpActionResult

      If reviewerId <= 0 Then
         Throw New ArgumentException("Reviewer ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsReviewer As New ApsReviewer

         Me.LoadApsReviewer(reviewer, _apsReviewer)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsReviewer(_apsReviewer)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetReviewer(reviewerId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveReviewer")>
   <Route("reviewers/{reviewerId}")>
   <HttpDelete>
   Public Function RemoveReviewer(reviewerId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If reviewerId <= 0 Then
         Throw New ArgumentException("Reviewer ID is required.")
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

            'Me.DeleteDbsReligion(religionId, lockId)
            Me.DeleteApsReviewer(reviewerId, q.LockId)

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

   Private Sub LoadApsReviewer(reviewer As ApsReviewerBody, apsReviewer As ApsReviewer)

      DataLib.ScatterValues(reviewer, apsReviewer)

   End Sub
   Private Sub LoadApsReviewer(row As DataRow, apsReviewer As ApsReviewer)

      With apsReviewer
         .ReviewerId = row.ToInt32("ReviewerId")
         .MemberEmployeeId = row.ToString("MemberEmployeeId")
         .UserId = row.ToInt32("UserId")
      End With

   End Sub

   Private Sub InsertApsReviewer(reviewer As ApsReviewer)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsReviewer", reviewer, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsReviewer(reviewer)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsReviewer(reviewer As ApsReviewer)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ReviewerId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsReviewer", reviewer, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsReviewer(reviewer)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(reviewer.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsReviewer(reviewer As ApsReviewer)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ReviewerId", reviewer.ReviewerId)
         .AddWithValue("@MemberEmployeeId", reviewer.MemberEmployeeId)
         .AddWithValue("@UserId", reviewer.UserId)
      End With

   End Sub
   Private Sub DeleteApsReviewer(reviewerId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ReviewerId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsReviewer", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ReviewerId", reviewerId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsReviewerBody
   Inherits ApsReviewer

End Class
