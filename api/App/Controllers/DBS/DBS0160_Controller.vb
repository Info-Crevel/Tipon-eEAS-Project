<RoutePrefix("api")>
Public Class DBS0160_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApplicantPositions")>
   <Route("applicant-positions")>
   <HttpGet>
   Public Function GetApplicantPositions() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0160_All")
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

   <SymAuthorization("GetApplicantPosition")>
   <Route("applicant-positions/{applicantPositionId}")>
   <HttpGet>
   Public Function GetApplicantPosition(applicantPositionId As Integer) As IHttpActionResult

      If applicantPositionId <= 0 Then
         Throw New ArgumentException("Applicant Position ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0160")
            With _direct
               .AddParameter("ApplicantPositionId", applicantPositionId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApplicantPosition")>
   <Route("applicant-positions/{currentUserId}")>
   <HttpPost>
   Public Function CreateApplicantPosition(currentUserId As Integer, <FromBody> applicantPosition As DbsApplicantPositionBody) As IHttpActionResult

      If applicantPosition.ApplicantPositionId <> -1 Then
         Throw New ArgumentException("Applicant Position ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _ApplicantPositionId As Integer = SysLib.GetNextSequence("ApplicantPositionId")

         applicantPosition.ApplicantPositionId = _ApplicantPositionId

         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicantPosition As New DbsApplicantPosition

         Me.LoadDbsApplicantPosition(applicantPosition, _dbsApplicantPosition)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsApplicantPosition(_dbsApplicantPosition)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicantPosition(_ApplicantPositionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(ApplicantPosition.ApplicantPositionId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApplicantPosition")>
   <Route("applicant-positions/{applicantPositionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApplicantPosition(applicantPositionId As Integer, currentUserId As Integer, <FromBody> applicantPosition As DbsApplicantPositionBody) As IHttpActionResult

      If applicantPositionId <= 0 Then
         Throw New ArgumentException("Applicant Position ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsApplicantPosition As New DbsApplicantPosition

         Me.LoadDbsApplicantPosition(applicantPosition, _dbsApplicantPosition)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsApplicantPosition(_dbsApplicantPosition)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApplicantPosition(applicantPositionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveApplicantPosition")>
   <Route("applicant-positions/{applicantPositionId}")>
   <HttpDelete>
   Public Function RemoveApplicantPosition(applicantPositionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If applicantPositionId <= 0 Then
         Throw New ArgumentException("Applicant Position ID is required.")
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

            Me.DeleteDbsApplicantPosition(applicantPositionId, q.LockId)

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
   Private Sub LoadDbsApplicantPosition(applicantPosition As DbsApplicantPositionBody, dbsApplicantPosition As DbsApplicantPosition)

      DataLib.ScatterValues(applicantPosition, dbsApplicantPosition)

   End Sub

   Private Sub LoadDbsApplicantPosition(row As DataRow, dbsApplicantPosition As DbsApplicantPosition)

      With dbsApplicantPosition
         .ApplicantPositionId = row.ToInt32("ApplicantPositionId")
         .ApplicantPositionName = row.ToString("ApplicantPositionName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsApplicantPosition(applicantPosition As DbsApplicantPosition)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsApplicantPosition", applicantPosition, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicantPosition(applicantPosition)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsApplicantPosition(applicantPosition As DbsApplicantPosition)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantPositionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsApplicantPosition", applicantPosition, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsApplicantPosition(applicantPosition)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(applicantPosition.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsApplicantPosition(applicantPosition As DbsApplicantPosition)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicantPositionId", applicantPosition.ApplicantPositionId)
         .AddWithValue("@ApplicantPositionName", applicantPosition.ApplicantPositionName)
         .AddWithValue("@SortSeq", applicantPosition.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsApplicantPosition(applicantPositionId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantPositionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsApplicantPosition", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApplicantPositionId", applicantPositionId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsApplicantPositionBody
   Inherits DbsApplicantPosition

End Class
