<RoutePrefix("api")>
Public Class DBS0450_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetVaccineTypes")>
   <Route("vaccine-types")>
   <HttpGet>
   Public Function GetVaccineTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0450_All")
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

   <SymAuthorization("GetVaccineType")>
   <Route("vaccine-types/{vaccineTypeId}")>
   <HttpGet>
   Public Function GetVaccineType(vaccineTypeId As Integer) As IHttpActionResult

      If vaccineTypeId <= 0 Then
         Throw New ArgumentException("Vaccine Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0450")
            With _direct
               .AddParameter("VaccineTypeId", vaccineTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateVaccineType")>
   <Route("vaccine-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateVaccineType(currentUserId As Integer, <FromBody> vaccineType As DbsVaccineTypeBody) As IHttpActionResult

      If vaccineType.VaccineTypeId <> -1 Then
         Throw New ArgumentException("Vaccine Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _vaccineTypeId As Integer = SysLib.GetNextSequence("VaccineTypeId")

         vaccineType.VaccineTypeId = _vaccineTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsVaccineType As New DbsVaccineType

         Me.LoadDbsVaccineType(vaccineType, _dbsVaccineType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsVaccineType(_dbsVaccineType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetVaccineType(_vaccineTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(vaccineType.VaccineTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyVaccineType")>
   <Route("vaccine-types/{vaccineTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyVaccineType(vaccineTypeId As Integer, currentUserId As Integer, <FromBody> vaccineType As DbsVaccineTypeBody) As IHttpActionResult

      If vaccineTypeId <= 0 Then
         Throw New ArgumentException("Vaccine Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsVaccineType As New DbsVaccineType

         Me.LoadDbsVaccineType(vaccineType, _dbsVaccineType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsVaccineType(_dbsVaccineType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetVaccineType(vaccineTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveVaccineType")>
   <Route("vaccine-types/{vaccineTypeId}")>
   <HttpDelete>
   Public Function RemoveVaccineType(vaccineTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If vaccineTypeId <= 0 Then
         Throw New ArgumentException("Vaccine Type ID is required.")
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

            Me.DeleteDbsVaccineType(vaccineTypeId, q.LockId)

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

   Private Sub LoadDbsVaccineType(vaccineType As DbsVaccineTypeBody, dbsVaccineType As DbsVaccineType)

      DataLib.ScatterValues(vaccineType, dbsVaccineType)

   End Sub

   Private Sub LoadDbsVaccineType(row As DataRow, dbsVaccineType As DbsVaccineType)

      With dbsVaccineType
         .VaccineTypeId = row.ToInt32("VaccineTypeId")
         .VaccineTypeName = row.ToString("VaccineTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsVaccineType(vaccineType As DbsVaccineType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsVaccineType", vaccineType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsVaccineType(vaccineType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsVaccineType(vaccineType As DbsVaccineType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("VaccineTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsVaccineType", vaccineType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsVaccineType(vaccineType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(vaccineType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsVaccineType(vaccineType As DbsVaccineType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@VaccineTypeId", vaccineType.VaccineTypeId)
         .AddWithValue("@VaccineTypeName", vaccineType.VaccineTypeName)
         .AddWithValue("@SortSeq", vaccineType.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsVaccineType(vaccineTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("VaccineTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsVaccineType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@VaccineTypeId", vaccineTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsVaccineTypeBody
   Inherits DbsVaccineType

End Class
