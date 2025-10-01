<RoutePrefix("api")>
Public Class DBS0340_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAllowances")>
   <Route("allowances")>
   <HttpGet>
   Public Function GetAllowances() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0340_All")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()

                  With _dataSet
                     .Tables(0).TableName = "allowance"
                     .Tables(1).TableName = "payTrx"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAllowance")>
   <Route("allowances/{allowanceId}")>
   <HttpGet>
   Public Function GetAllowance(allowanceId As Integer) As IHttpActionResult

      If allowanceId <= 0 Then
         Throw New ArgumentException("Allowance ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0340")
            With _direct
               .AddParameter("AllowanceId", allowanceId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAllowance")>
   <Route("allowances/{currentUserId}")>
   <HttpPost>
   Public Function CreateAllowance(currentUserId As Integer, <FromBody> allowance As DbsAllowanceBody) As IHttpActionResult

      If allowance.AllowanceId <> -1 Then
         Throw New ArgumentException("Allowance ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _allowanceId As Integer = SysLib.GetNextSequence("AllowanceId")

         allowance.AllowanceId = _allowanceId

         '
         ' Load proposed values from payload
         '
         Dim _dbsAllowance As New DbsAllowance

         Me.LoadDbsAllowance(allowance, _dbsAllowance)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsAllowance(_dbsAllowance)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAllowance(_allowanceId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(allowance.AllowanceId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAllowance")>
   <Route("allowances/{allowanceId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAllowance(allowanceId As Integer, currentUserId As Integer, <FromBody> allowance As DbsAllowanceBody) As IHttpActionResult

      If allowanceId <= 0 Then
         Throw New ArgumentException("Allowance ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsAllowance As New DbsAllowance

         Me.LoadDbsAllowance(allowance, _dbsAllowance)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsAllowance(_dbsAllowance)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAllowance(allowanceId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveAllowance")>
   <Route("allowances/{allowanceId}")>
   <HttpDelete>
   Public Function RemoveAllowance(allowanceId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If allowanceId <= 0 Then
         Throw New ArgumentException("Allowance ID is required.")
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

            Me.DeleteDbsAllowance(allowanceId, q.LockId)

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

   Private Sub LoadDbsAllowance(allowance As DbsAllowanceBody, dbsAllowance As DbsAllowance)

      DataLib.ScatterValues(allowance, dbsAllowance)

   End Sub

   Private Sub LoadDbsAllowance(row As DataRow, dbsAllowance As DbsAllowance)

      With dbsAllowance
         .AllowanceId = row.ToInt32("AllowanceId")
         .AllowanceName = row.ToString("AllowanceName")
            .PayTrxCode = row.ToString("PayTrxCode")
            .TaxFlag = row.ToBoolean("TaxFlag")
            .PayHourly = row.ToBoolean("PayHourly")
            .PayMaxAmount = row.ToInt32("PayMaxAmount")
            .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsAllowance(allowance As DbsAllowance)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsAllowance", allowance, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAllowance(allowance)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsAllowance(allowance As DbsAllowance)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AllowanceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsAllowance", allowance, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAllowance(allowance)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(allowance.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsAllowance(allowance As DbsAllowance)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AllowanceId", allowance.AllowanceId)
         .AddWithValue("@AllowanceName", allowance.AllowanceName)
         .AddWithValue("@PayTrxCode", allowance.PayTrxCode)
            .AddWithValue("@TaxFlag", allowance.TaxFlag)
            .AddWithValue("@PayHourly", allowance.PayHourly)
            .AddWithValue("@PayMaxAmount", allowance.PayMaxAmount)
            .AddWithValue("@SortSeq", allowance.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsAllowance(allowanceId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AllowanceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsAllowance", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AllowanceId", allowanceId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsAllowanceBody
   Inherits DbsAllowance

End Class
