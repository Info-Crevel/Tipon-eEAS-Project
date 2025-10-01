<RoutePrefix("api")>
Public Class DBS0460_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMedicalResultTypes")>
   <Route("medical-result-types")>
   <HttpGet>
   Public Function GetMedicalResultTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0460_All")
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

   <SymAuthorization("GetMedicalResultType")>
   <Route("medical-result-types/{medicalResultTypeId}")>
   <HttpGet>
   Public Function GetMedicalResultType(medicalResultTypeId As Integer) As IHttpActionResult

      If medicalResultTypeId <= 0 Then
         Throw New ArgumentException("Medical Result Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0460")
            With _direct
               .AddParameter("MedicalResultTypeId", medicalResultTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMedicalResultType")>
   <Route("medical-result-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateMedicalResultType(currentUserId As Integer, <FromBody> medicalResultType As DbsMedicalResultTypeBody) As IHttpActionResult

      If medicalResultType.MedicalResultTypeId <> -1 Then
         Throw New ArgumentException("Medical Result Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _medicalResultTypeId As Integer = SysLib.GetNextSequence("MedicalResultTypeId")

         medicalResultType.MedicalResultTypeId = _medicalResultTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMedicalResultType As New DbsMedicalResultType

         Me.LoadDbsMedicalResultType(medicalResultType, _dbsMedicalResultType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsMedicalResultType(_dbsMedicalResultType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMedicalResultType(_medicalResultTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(medicalResultType.MedicalResultTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyMedicalResultType")>
   <Route("medical-result-types/{medicalResultTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMedicalResultType(medicalResultTypeId As Integer, currentUserId As Integer, <FromBody> medicalResultType As DbsMedicalResultTypeBody) As IHttpActionResult

      If medicalResultTypeId <= 0 Then
         Throw New ArgumentException("Medical Result Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsMedicalResultType As New DbsMedicalResultType

         Me.LoadDbsMedicalResultType(medicalResultType, _dbsMedicalResultType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsMedicalResultType(_dbsMedicalResultType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMedicalResultType(medicalResultTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveMedicalResultType")>
   <Route("medical-result-types/{medicalResultTypeId}")>
   <HttpDelete>
   Public Function RemoveMedicalResultType(medicalResultTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If medicalResultTypeId <= 0 Then
         Throw New ArgumentException("Medical Result Type ID is required.")
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

            Me.DeleteDbsMedicalResultType(medicalResultTypeId, q.LockId)

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

   Private Sub LoadDbsMedicalResultType(medicalResultType As DbsMedicalResultTypeBody, dbsMedicalResultType As DbsMedicalResultType)

      DataLib.ScatterValues(medicalResultType, dbsMedicalResultType)

   End Sub

   Private Sub LoadDbsMedicalResultType(row As DataRow, dbsMedicalResultType As DbsMedicalResultType)

      With dbsMedicalResultType
         .MedicalResultTypeId = row.ToInt32("MedicalResultTypeId")
         .MedicalResultTypeCode = row.ToString("MedicalResultTypeCode")
         .MedicalResultTypeName = row.ToString("MedicalResultTypeName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsMedicalResultType(medicalResultType As DbsMedicalResultType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMedicalResultType", medicalResultType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMedicalResultType(medicalResultType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMedicalResultType(medicalResultType As DbsMedicalResultType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MedicalResultTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMedicalResultType", medicalResultType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMedicalResultType(medicalResultType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(medicalResultType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMedicalResultType(medicalResultType As DbsMedicalResultType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MedicalResultTypeId", medicalResultType.MedicalResultTypeId)
         .AddWithValue("@MedicalResultTypeCode", medicalResultType.MedicalResultTypeCode)
         .AddWithValue("@MedicalResultTypeName", medicalResultType.MedicalResultTypeName)
         .AddWithValue("@UploadRequiredFlag", medicalResultType.UploadRequiredFlag)
         .AddWithValue("@SortSeq", medicalResultType.SortSeq)
      End With

   End Sub

   Private Sub DeleteDbsMedicalResultType(medicalResultTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MedicalResultTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMedicalResultType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MedicalResultTypeId", medicalResultTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsMedicalResultTypeBody
   Inherits DbsMedicalResultType

End Class
