<RoutePrefix("api")>
Public Class DBS0240_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetComplianceTrainings")>
   <Route("compliance-trainings")>
   <HttpGet>
   Public Function GetComplianceTrainings() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0240_All")
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

   <SymAuthorization("GetComplianceTraining")>
   <Route("compliance-trainings/{complianceTrainingId}")>
   <HttpGet>
   Public Function GetComplianceTraining(complianceTrainingId As Integer) As IHttpActionResult

      If complianceTrainingId <= 0 Then
         Throw New ArgumentException("Compliance Training ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0240")
            With _direct
               .AddParameter("ComplianceTrainingId", complianceTrainingId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateComplianceTraining")>
   <Route("compliance-trainings/{currentUserId}")>
   <HttpPost>
   Public Function CreateComplianceTraining(currentUserId As Integer, <FromBody> complianceTraining As DbsComplianceTrainingBody) As IHttpActionResult

      If complianceTraining.ComplianceTrainingId <> -1 Then
         Throw New ArgumentException("Compliance Training ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _complianceTrainingId As Integer = SysLib.GetNextSequence("ComplianceTrainingId")

         complianceTraining.ComplianceTrainingId = _complianceTrainingId

         '
         ' Load proposed values from payload
         '
         Dim _dbsComplianceTraining As New DbsComplianceTraining

         Me.LoadDbsComplianceTraining(complianceTraining, _dbsComplianceTraining)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsComplianceTraining(_dbsComplianceTraining)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetComplianceTraining(_complianceTrainingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(complianceTraining.ComplianceTrainingId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyComplianceTraining")>
   <Route("compliance-trainings/{complianceTrainingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyComplianceTraining(complianceTrainingId As Integer, currentUserId As Integer, <FromBody> complianceTraining As DbsComplianceTrainingBody) As IHttpActionResult

      If complianceTrainingId <= 0 Then
         Throw New ArgumentException("Compliance Training ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsComplianceTraining As New DbsComplianceTraining

         Me.LoadDbsComplianceTraining(complianceTraining, _dbsComplianceTraining)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsComplianceTraining(_dbsComplianceTraining)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetComplianceTraining(complianceTrainingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveComplianceTraining")>
   <Route("compliance-trainings/{complianceTrainingId}")>
   <HttpDelete>
   Public Function RemoveComplianceTraining(complianceTrainingId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If complianceTrainingId <= 0 Then
         Throw New ArgumentException("Compliance Training ID is required.")
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

            Me.DeleteDbsComplianceTraining(complianceTrainingId, q.LockId)

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

   Private Sub LoadDbsComplianceTraining(complianceTraining As DbsComplianceTrainingBody, dbsComplianceTraining As DbsComplianceTraining)

      DataLib.ScatterValues(complianceTraining, dbsComplianceTraining)

   End Sub

   Private Sub LoadDbsComplianceTraining(row As DataRow, dbsComplianceTraining As DbsComplianceTraining)

      With dbsComplianceTraining
         .ComplianceTrainingId = row.ToInt32("ComplianceTrainingId")
         .ComplianceTrainingName = row.ToString("ComplianceTrainingName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsComplianceTraining(complianceTraining As DbsComplianceTraining)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsComplianceTraining", complianceTraining, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsComplianceTraining(complianceTraining)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsComplianceTraining(complianceTraining As DbsComplianceTraining)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ComplianceTrainingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsComplianceTraining", complianceTraining, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsComplianceTraining(complianceTraining)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(complianceTraining.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsComplianceTraining(complianceTraining As DbsComplianceTraining)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ComplianceTrainingId", complianceTraining.ComplianceTrainingId)
         .AddWithValue("@ComplianceTrainingName", complianceTraining.ComplianceTrainingName)
         .AddWithValue("@UploadRequiredFlag", complianceTraining.UploadRequiredFlag)
         .AddWithValue("@SortSeq", complianceTraining.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsComplianceTraining(complianceTrainingId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ComplianceTrainingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsComplianceTraining", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ComplianceTrainingId", complianceTrainingId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsComplianceTrainingBody
   Inherits DbsComplianceTraining

End Class
