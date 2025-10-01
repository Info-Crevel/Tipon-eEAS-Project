<RoutePrefix("api")>
Public Class DBS0150_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetLicenseProfessions")>
   <Route("license-professions")>
   <HttpGet>
   Public Function GetLicenseProfessions() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0150_All")
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

   <SymAuthorization("GetLicenseProfession")>
   <Route("license-professions/{licenseProfessionId}")>
   <HttpGet>
   Public Function GetLicenseProfession(licenseProfessionId As Integer) As IHttpActionResult

      If licenseProfessionId <= 0 Then
         Throw New ArgumentException("License Profession ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0150")
            With _direct
               .AddParameter("LicenseProfessionId", licenseProfessionId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateLicenseProfession")>
   <Route("license-professions/{currentUserId}")>
   <HttpPost>
   Public Function CreateLicenseProfession(currentUserId As Integer, <FromBody> licenseProfession As DbsLicenseProfessionBody) As IHttpActionResult

      If licenseProfession.LicenseProfessionId <> -1 Then
         Throw New ArgumentException("License Profession ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _LicenseProfessionId As Integer = SysLib.GetNextSequence("LicenseProfessionId")

         licenseProfession.LicenseProfessionId = _LicenseProfessionId

         '
         ' Load proposed values from payload
         '
         Dim _dbsLicenseProfession As New DbsLicenseProfession

         Me.LoadDbsLicenseProfession(licenseProfession, _dbsLicenseProfession)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsLicenseProfession(_dbsLicenseProfession)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetLicenseProfession(_LicenseProfessionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(LicenseProfession.LicenseProfessionId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyLicenseProfession")>
   <Route("license-professions/{licenseProfessionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyLicenseProfession(licenseProfessionId As Integer, currentUserId As Integer, <FromBody> licenseProfession As DbsLicenseProfessionBody) As IHttpActionResult

      If licenseProfessionId <= 0 Then
         Throw New ArgumentException("License Profession ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsLicenseProfession As New DbsLicenseProfession

         Me.LoadDbsLicenseProfession(licenseProfession, _dbsLicenseProfession)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsLicenseProfession(_dbsLicenseProfession)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetLicenseProfession(licenseProfessionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveLicenseProfession")>
   <Route("license-professions/{licenseProfessionId}")>
   <HttpDelete>
   Public Function RemoveLicenseProfession(licenseProfessionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If licenseProfessionId <= 0 Then
         Throw New ArgumentException("License Profession ID is required.")
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

            Me.DeleteDbsLicenseProfession(licenseProfessionId, q.LockId)

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

   Private Sub LoadDbsLicenseProfession(licenseProfession As DbsLicenseProfessionBody, dbsLicenseProfession As DbsLicenseProfession)

      DataLib.ScatterValues(licenseProfession, dbsLicenseProfession)

   End Sub

   Private Sub LoadDbsLicenseProfession(row As DataRow, dbsLicenseProfession As DbsLicenseProfession)

      With dbsLicenseProfession
         .LicenseProfessionId = row.ToInt32("LicenseProfessionId")
         .LicenseProfessionName = row.ToString("LicenseProfessionName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsLicenseProfession(licenseProfession As DbsLicenseProfession)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsLicenseProfession", licenseProfession, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsLicenseProfession(licenseProfession)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsLicenseProfession(licenseProfession As DbsLicenseProfession)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("LicenseProfessionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsLicenseProfession", licenseProfession, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsLicenseProfession(licenseProfession)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(licenseProfession.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsLicenseProfession(licenseProfession As DbsLicenseProfession)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@LicenseProfessionId", licenseProfession.LicenseProfessionId)
         .AddWithValue("@LicenseProfessionName", licenseProfession.LicenseProfessionName)
         .AddWithValue("@UploadRequiredFlag", licenseProfession.UploadRequiredFlag)
         .AddWithValue("@SortSeq", licenseProfession.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsLicenseProfession(licenseProfessionId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("LicenseProfessionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsLicenseProfession", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@LicenseProfessionId", licenseProfessionId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsLicenseProfessionBody
   Inherits DbsLicenseProfession

End Class
