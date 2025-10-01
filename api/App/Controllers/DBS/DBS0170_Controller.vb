<RoutePrefix("api")>
Public Class DBS0170_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetContractTypes")>
   <Route("contract-types")>
   <HttpGet>
   Public Function GetContractTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0170_All")
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

   <SymAuthorization("GetContractType")>
   <Route("contract-types/{contractTypeId}")>
   <HttpGet>
   Public Function GetContractType(contractTypeId As Integer) As IHttpActionResult

      If contractTypeId <= 0 Then
         Throw New ArgumentException("Contract Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0170")
            With _direct
               .AddParameter("ContractTypeId", contractTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateContractType")>
   <Route("contract-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateContractType(currentUserId As Integer, <FromBody> contractType As DbsContractTypeBody) As IHttpActionResult

      If contractType.ContractTypeId <> -1 Then
         Throw New ArgumentException("Contract Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _ContractTypeId As Integer = SysLib.GetNextSequence("ContractTypeId")

         contractType.ContractTypeId = _ContractTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsContractType As New DbsContractType

         Me.LoadDbsContractType(contractType, _dbsContractType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsContractType(_dbsContractType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetContractType(_ContractTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(ContractType.ContractTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyContractType")>
   <Route("contract-types/{contractTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyContractType(contractTypeId As Integer, currentUserId As Integer, <FromBody> contractType As DbsContractTypeBody) As IHttpActionResult

      If contractTypeId <= 0 Then
         Throw New ArgumentException("Contract Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsContractType As New DbsContractType

         Me.LoadDbsContractType(contractType, _dbsContractType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsContractType(_dbsContractType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetContractType(contractTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveContractType")>
   <Route("contract-types/{contractTypeId}")>
   <HttpDelete>
   Public Function RemoveContractType(contractTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If contractTypeId <= 0 Then
         Throw New ArgumentException("Contract Type ID is required.")
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

            Me.DeleteDbsContractType(contractTypeId, q.LockId)

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

   Private Sub LoadDbsContractType(contractType As DbsContractTypeBody, dbsContractType As DbsContractType)

      DataLib.ScatterValues(contractType, dbsContractType)

   End Sub

   Private Sub LoadDbsContractType(row As DataRow, dbsContractType As DbsContractType)

      With dbsContractType
         .ContractTypeId = row.ToInt32("ContractTypeId")
         .ContractTypeName = row.ToString("ContractTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsContractType(contractType As DbsContractType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsContractType", contractType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsContractType(contractType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsContractType(contractType As DbsContractType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ContractTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsContractType", contractType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsContractType(contractType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(contractType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsContractType(contractType As DbsContractType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ContractTypeId", contractType.ContractTypeId)
         .AddWithValue("@ContractTypeName", contractType.ContractTypeName)
         .AddWithValue("@SortSeq", contractType.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsContractType(ContractTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ContractTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsContractType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ContractTypeId", ContractTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsContractTypeBody
   Inherits DbsContractType

End Class
