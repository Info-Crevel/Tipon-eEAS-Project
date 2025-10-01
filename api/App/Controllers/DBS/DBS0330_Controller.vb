<RoutePrefix("api")>
Public Class DBS0330_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetInventoryCharges")>
   <Route("inventory-charges")>
   <HttpGet>
   Public Function GetInventoryCharges() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0330_All")
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

   <SymAuthorization("GetInventoryCharge")>
   <Route("inventory-charges/{inventoryChargeId}")>
   <HttpGet>
   Public Function GetInventoryCharge(inventoryChargeId As Integer) As IHttpActionResult

      If inventoryChargeId <= 0 Then
         Throw New ArgumentException("Inventory Charge ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0330")
            With _direct
               .AddParameter("InventoryChargeId", inventoryChargeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateInventoryCharge")>
   <Route("inventory-charges/{currentUserId}")>
   <HttpPost>
   Public Function CreateInventoryCharge(currentUserId As Integer, <FromBody> inventoryCharge As DbsInventoryChargeBody) As IHttpActionResult

      If inventoryCharge.InventoryChargeId <> -1 Then
         Throw New ArgumentException("Inventory Charge ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _inventoryChargeId As Integer = SysLib.GetNextSequence("InventoryChargeId")

         inventoryCharge.InventoryChargeId = _inventoryChargeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsInventoryCharge As New DbsInventoryCharge

         Me.LoadDbsInventoryCharge(inventoryCharge, _dbsInventoryCharge)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsInventoryCharge(_dbsInventoryCharge)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInventoryCharge(_inventoryChargeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(inventoryCharge.InventoryChargeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyInventoryCharge")>
   <Route("inventory-charges/{inventoryChargeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyInventoryCharge(inventoryChargeId As Integer, currentUserId As Integer, <FromBody> inventoryCharge As DbsInventoryChargeBody) As IHttpActionResult

      If inventoryChargeId <= 0 Then
         Throw New ArgumentException("Inventory Charge ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsInventoryCharge As New DbsInventoryCharge

         Me.LoadDbsInventoryCharge(inventoryCharge, _dbsInventoryCharge)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsInventoryCharge(_dbsInventoryCharge)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInventoryCharge(inventoryChargeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveInventoryCharge")>
   <Route("inventory-charges/{inventoryChargeId}")>
   <HttpDelete>
   Public Function RemoveInventoryCharge(inventoryChargeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If inventoryChargeId <= 0 Then
         Throw New ArgumentException("Inventory Charge ID is required.")
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

            Me.DeleteDbsInventoryCharge(inventoryChargeId, q.LockId)

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

   Private Sub LoadDbsInventoryCharge(inventoryCharge As DbsInventoryChargeBody, dbsInventoryCharge As DbsInventoryCharge)

      DataLib.ScatterValues(inventoryCharge, dbsInventoryCharge)

   End Sub

   Private Sub LoadDbsInventoryCharge(row As DataRow, dbsInventoryCharge As DbsInventoryCharge)

      With dbsInventoryCharge
         .InventoryChargeId = row.ToInt32("InventoryChargeId")
         .InventoryChargeName = row.ToString("InventoryChargeName")
         .ClientFlag = row.ToBoolean("ClientFlag")
         .MemberFlag = row.ToBoolean("MemberFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsInventoryCharge(inventoryCharge As DbsInventoryCharge)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsInventoryCharge", inventoryCharge, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInventoryCharge(inventoryCharge)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsInventoryCharge(inventoryCharge As DbsInventoryCharge)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InventoryChargeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsInventoryCharge", inventoryCharge, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInventoryCharge(inventoryCharge)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(inventoryCharge.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsInventoryCharge(inventoryCharge As DbsInventoryCharge)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@InventoryChargeId", inventoryCharge.InventoryChargeId)
         .AddWithValue("@InventoryChargeName", inventoryCharge.InventoryChargeName)
         .AddWithValue("@ClientFlag", inventoryCharge.ClientFlag)
         .AddWithValue("@MemberFlag", inventoryCharge.MemberFlag)
         .AddWithValue("@SortSeq", inventoryCharge.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsInventoryCharge(inventoryChargeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InventoryChargeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsInventoryCharge", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@InventoryChargeId", inventoryChargeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsInventoryChargeBody
   Inherits DbsInventoryCharge

End Class
