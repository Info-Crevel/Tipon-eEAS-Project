<RoutePrefix("api")>
Public Class DBS0370_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCdaTrainings")>
   <Route("cda-trainings")>
   <HttpGet>
   Public Function GetCdaTrainings() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0370_All")
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

   <SymAuthorization("GetCdaTraining")>
   <Route("cdaTrainings/{cdaTrainingId}")>
   <HttpGet>
   Public Function GetCdaTraining(cdaTrainingId As Integer) As IHttpActionResult

      If cdaTrainingId <= 0 Then
         Throw New ArgumentException("CDA Training ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0370")
            With _direct
               .AddParameter("CdaTrainingId", cdaTrainingId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateCdaTraining")>
   <Route("cdaTrainings/{currentUserId}")>
   <HttpPost>
   Public Function CreateCdaTraining(currentUserId As Integer, <FromBody> cdaTraining As DbsCdaTrainingBody) As IHttpActionResult

      If cdaTraining.CDATrainingId <> -1 Then
         Throw New ArgumentException("CDA Training ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _cdaTrainingId As Integer = SysLib.GetNextSequence("CdaTrainingId")

         cdaTraining.CDATrainingId = _cdaTrainingId

         '
         ' Load proposed values from payload
         '
         Dim _dbsCdaTraining As New DbsCDATraining

         Me.LoadDbsCdaTraining(cdaTraining, _dbsCdaTraining)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsCdaTraining(_dbsCdaTraining)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCdaTraining(_cdaTrainingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(cdaTraining.CdaTrainingId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyCdaTraining")>
   <Route("cdaTrainings/{cdaTrainingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCdaTraining(cdaTrainingId As Integer, currentUserId As Integer, <FromBody> cdaTraining As DbsCdaTrainingBody) As IHttpActionResult

      If cdaTrainingId <= 0 Then
         Throw New ArgumentException("CDA Training ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsCdaTraining As New DbsCDATraining

         Me.LoadDbsCdaTraining(cdaTraining, _dbsCdaTraining)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsCdaTraining(_dbsCdaTraining)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCdaTraining(cdaTrainingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveCdaTraining")>
   <Route("cdaTrainings/{cdaTrainingId}")>
   <HttpDelete>
   Public Function RemoveCdaTraining(cdaTrainingId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If cdaTrainingId <= 0 Then
         Throw New ArgumentException("CDA Training ID is required.")
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

            Me.DeleteDbsCdaTraining(cdaTrainingId, q.LockId)

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

   Private Sub LoadDbsCdaTraining(cdaTraining As DbsCdaTrainingBody, dbsCdaTraining As DbsCDATraining)

      DataLib.ScatterValues(cdaTraining, dbsCdaTraining)

   End Sub

   Private Sub LoadDbsCdaTraining(row As DataRow, dbsCdaTraining As DbsCDATraining)

      With dbsCdaTraining
         .CDATrainingId = row.ToInt32("CdaTrainingId")
         .CDATrainingName = row.ToString("CdaTrainingName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsCdaTraining(cdaTraining As DbsCDATraining)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsCdaTraining", cdaTraining, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCdaTraining(cdaTraining)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsCdaTraining(cdaTraining As DbsCDATraining)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CdaTrainingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsCdaTraining", cdaTraining, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCdaTraining(cdaTraining)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(cdaTraining.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsCdaTraining(cdaTraining As DbsCDATraining)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CdaTrainingId", cdaTraining.CDATrainingId)
         .AddWithValue("@CdaTrainingName", cdaTraining.CDATrainingName)
         .AddWithValue("@UploadRequiredFlag", cdaTraining.UploadRequiredFlag)
         .AddWithValue("@SortSeq", cdaTraining.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsCdaTraining(cdaTrainingId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CdaTrainingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsCdaTraining", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CdaTrainingId", cdaTrainingId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsCdaTrainingBody
   Inherits DbsCDATraining

End Class
