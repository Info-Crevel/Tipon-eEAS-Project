<RoutePrefix("api")>
Public Class DBS0320_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetExpenseChargings")>
   <Route("expense-chargings")>
   <HttpGet>
   Public Function GetExpenseChargings() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0320_All")
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

   <SymAuthorization("GetExpenseCharging")>
   <Route("expense-chargings/{expenseChargingId}")>
   <HttpGet>
   Public Function GetExpenseCharging(expenseChargingId As Integer) As IHttpActionResult

      If expenseChargingId <= 0 Then
         Throw New ArgumentException("Expense Charging ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0320")
            With _direct
               .AddParameter("ExpenseChargingId", expenseChargingId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateExpenseCharging")>
   <Route("expense-chargings/{currentUserId}")>
   <HttpPost>
   Public Function CreateExpenseCharging(currentUserId As Integer, <FromBody> expenseCharging As DbsExpenseChargingBody) As IHttpActionResult

      If expenseCharging.ExpenseChargingId <> -1 Then
         Throw New ArgumentException("Expense Charging ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _expenseChargingId As Integer = SysLib.GetNextSequence("ExpenseChargingId")

         expenseCharging.ExpenseChargingId = _expenseChargingId

         '
         ' Load proposed values from payload
         '
         Dim _dbsExpenseCharging As New DbsExpenseCharging

         Me.LoadDbsExpenseCharging(expenseCharging, _dbsExpenseCharging)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsExpenseCharging(_dbsExpenseCharging)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetExpenseCharging(_expenseChargingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(expenseCharging.ExpenseChargingId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyExpenseCharging")>
   <Route("expense-chargings/{expenseChargingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyExpenseCharging(expenseChargingId As Integer, currentUserId As Integer, <FromBody> expenseCharging As DbsExpenseChargingBody) As IHttpActionResult

      If expenseChargingId <= 0 Then
         Throw New ArgumentException("Expense Charging ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsExpenseCharging As New DbsExpenseCharging

         Me.LoadDbsExpenseCharging(expenseCharging, _dbsExpenseCharging)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsExpenseCharging(_dbsExpenseCharging)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetExpenseCharging(expenseChargingId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveExpenseCharging")>
   <Route("expense-chargings/{expenseChargingId}")>
   <HttpDelete>
   Public Function RemoveExpenseCharging(expenseChargingId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If expenseChargingId <= 0 Then
         Throw New ArgumentException("Expense Charging ID is required.")
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

            Me.DeleteDbsExpenseCharging(expenseChargingId, q.LockId)

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

   Private Sub LoadDbsExpenseCharging(expenseCharging As DbsExpenseChargingBody, dbsExpenseCharging As DbsExpenseCharging)

      DataLib.ScatterValues(expenseCharging, dbsExpenseCharging)

   End Sub

   Private Sub LoadDbsExpenseCharging(row As DataRow, dbsExpenseCharging As DbsExpenseCharging)

      With dbsExpenseCharging
         .ExpenseChargingId = row.ToInt32("ExpenseChargingId")
         .ExpenseChargingName = row.ToString("ExpenseChargingName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsExpenseCharging(expenseCharging As DbsExpenseCharging)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsExpenseCharging", expenseCharging, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsExpenseCharging(expenseCharging)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsExpenseCharging(expenseCharging As DbsExpenseCharging)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ExpenseChargingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsExpenseCharging", expenseCharging, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsExpenseCharging(expenseCharging)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(expenseCharging.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsExpenseCharging(expenseCharging As DbsExpenseCharging)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ExpenseChargingId", expenseCharging.ExpenseChargingId)
         .AddWithValue("@ExpenseChargingName", expenseCharging.ExpenseChargingName)
         .AddWithValue("@SortSeq", expenseCharging.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsExpenseCharging(expenseChargingId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ExpenseChargingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsExpenseCharging", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ExpenseChargingId", expenseChargingId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsExpenseChargingBody
   Inherits DbsExpenseCharging

End Class
