<RoutePrefix("api")>
Public Class DBS0220_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetTrainingInstitutions")>
   <Route("training-institutions")>
   <HttpGet>
   Public Function GetTrainingInstitutions() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0220_All")
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

   <SymAuthorization("GetTrainingInstitution")>
   <Route("training-institutions/{trainingInstitutionId}")>
   <HttpGet>
   Public Function GetTrainingInstitution(trainingInstitutionId As Integer) As IHttpActionResult

      If trainingInstitutionId <= 0 Then
         Throw New ArgumentException("Training Institution ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0220")
            With _direct
               .AddParameter("TrainingInstitutionId", trainingInstitutionId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTrainingInstitution")>
   <Route("training-institutions/{currentUserId}")>
   <HttpPost>
   Public Function CreateTrainingInstitution(currentUserId As Integer, <FromBody> trainingInstitution As DbsTrainingInstitutionBody) As IHttpActionResult

      If trainingInstitution.TrainingInstitutionId <> -1 Then
         Throw New ArgumentException("Training Institution ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _trainingInstitutionId As Integer = SysLib.GetNextSequence("TrainingInstitutionId")

         trainingInstitution.TrainingInstitutionId = _trainingInstitutionId

         '
         ' Load proposed values from payload
         '
         Dim _dbsTrainingInstitution As New DbsTrainingInstitution

         Me.LoadDbsTrainingInstitution(trainingInstitution, _dbsTrainingInstitution)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsTrainingInstitution(_dbsTrainingInstitution)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetTrainingInstitution(_trainingInstitutionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(trainingInstitution.TrainingInstitutionId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyTrainingInstitution")>
   <Route("training-institutions/{trainingInstitutionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTrainingInstitution(trainingInstitutionId As Integer, currentUserId As Integer, <FromBody> trainingInstitution As DbsTrainingInstitutionBody) As IHttpActionResult

      If trainingInstitutionId <= 0 Then
         Throw New ArgumentException("Training Institution ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsTrainingInstitution As New DbsTrainingInstitution

         Me.LoadDbsTrainingInstitution(trainingInstitution, _dbsTrainingInstitution)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsTrainingInstitution(_dbsTrainingInstitution)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetTrainingInstitution(trainingInstitutionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveTrainingInstitution")>
   <Route("training-institutions/{trainingInstitutionId}")>
   <HttpDelete>
   Public Function RemoveTrainingInstitution(trainingInstitutionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If trainingInstitutionId <= 0 Then
         Throw New ArgumentException("Training Institution ID is required.")
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

            Me.DeleteDbsTrainingInstitution(trainingInstitutionId, q.LockId)

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

   Private Sub LoadDbsTrainingInstitution(trainingInstitution As DbsTrainingInstitutionBody, dbsTrainingInstitution As DbsTrainingInstitution)

        DataLib.ScatterValues(trainingInstitution, dbsTrainingInstitution)

    End Sub

    Private Sub LoadDbsTrainingInstitution(row As DataRow, dbsTrainingInstitution As DbsTrainingInstitution)

        With dbsTrainingInstitution
            .TrainingInstitutionId = row.ToInt32("TrainingInstitutionId")
            .TrainingInstitutionName = row.ToString("TrainingInstitutionName")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsTrainingInstitution(trainingInstitution As DbsTrainingInstitution)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsTrainingInstitution", trainingInstitution, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsTrainingInstitution(trainingInstitution)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateDbsTrainingInstitution(trainingInstitution As DbsTrainingInstitution)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("TrainingInstitutionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsTrainingInstitution", trainingInstitution, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsTrainingInstitution(trainingInstitution)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(trainingInstitution.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsTrainingInstitution(trainingInstitution As DbsTrainingInstitution)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@TrainingInstitutionId", trainingInstitution.TrainingInstitutionId)
            .AddWithValue("@TrainingInstitutionName", trainingInstitution.TrainingInstitutionName)
            .AddWithValue("@SortSeq", trainingInstitution.SortSeq)

        End With

    End Sub

    Private Sub DeleteDbsTrainingInstitution(trainingInstitutionId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("TrainingInstitutionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsTrainingInstitution", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@TrainingInstitutionId", trainingInstitutionId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsTrainingInstitutionBody
   Inherits DbsTrainingInstitution

End Class
