<RoutePrefix("api")>
Public Class APS0530_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayOutAccounts")>
   <Route("pay-out-accounts")>
   <HttpGet>
   Public Function GetPayOutAccounts() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0530_All")
            Using _dataSet As DataSet = _direct.ExecuteDataSet()

               With _dataSet
                  .Tables(0).TableName = "payOutAccounts"
               End With

               Return Me.Ok(_dataSet)
            End Using

         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetPayOutAccount")>
   <Route("pay-out-accounts/{payOutAccountId}")>
   <HttpGet>
   Public Function GetPayOutAccount(payOutAccountId As Integer) As IHttpActionResult

      If payOutAccountId <= 0 Then
         Throw New ArgumentException("PayOutAccount ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0530")
            With _direct
               .AddParameter("PayOutAccountId", payOutAccountId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePayOutAccount")>
   <Route("pay-out-accounts/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayOutAccount(currentUserId As Integer, <FromBody> payOutAccount As PayOutAccountBody) As IHttpActionResult

      If payOutAccount.PayOutAccountId <> -1 Then
         Throw New ArgumentException("PayOutAccount ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _payOutAccountId As Integer = SysLib.GetNextSequence("PayOutAccountId")

         payOutAccount.PayOutAccountId = _payOutAccountId

         '
         ' Load proposed values from payload
         '
         Dim _apsPayOutAccount As New ApsPayOutAccount

         Me.LoadApsPayOutAccount(payOutAccount, _apsPayOutAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertPayOutAccount(_apsPayOutAccount)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayOutAccount(_payOutAccountId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayOutAccount")>
   <Route("pay-out-accounts/{payOutAccountId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayOutAccount(payOutAccountId As Integer, currentUserId As Integer, <FromBody> payOutAccount As PayOutAccountBody) As IHttpActionResult

      If payOutAccountId <= 0 Then
         Throw New ArgumentException("PayOutAccount ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsPayOutAccount As New ApsPayOutAccount

         Me.LoadApsPayOutAccount(payOutAccount, _apsPayOutAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsPayOutAccount(_apsPayOutAccount)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayOutAccount(payOutAccountId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayOutAccount")>
   <Route("pay-out-accounts/{payOutAccountId}")>
   <HttpDelete>
   Public Function RemovePayOutAccount(payOutAccountId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payOutAccountId <= 0 Then
         Throw New ArgumentException("Payee Type ID is required.")
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

            'Me.DeleteDbsReligion(religionId, lockId)
            Me.DeletePayOutAccount(payOutAccountId, q.LockId)

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
   Private Sub LoadApsPayOutAccount(payOutAccount As PayOutAccountBody, finPayOutAccount As ApsPayOutAccount)

      DataLib.ScatterValues(payOutAccount, finPayOutAccount)

   End Sub
   Private Sub LoadApsPayOutAccount(row As DataRow, apsPayOutAccount As ApsPayOutAccount)

      With apsPayOutAccount
         .PayOutAccountId = row.ToInt32("PayOutAccountId")
         .PayOutTrxCode = row.ToString("PayOutTrxCode")
         .PayOutTrxName = row.ToString("PayOutTrxName")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub InsertPayOutAccount(payOutAccount As ApsPayOutAccount)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsPayOutAccount", payOutAccount, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayOutAccount(payOutAccount)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsPayOutAccount(payOutAccount As ApsPayOutAccount)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayOutAccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsPayOutAccount", payOutAccount, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayOutAccount(payOutAccount)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payOutAccount.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayOutAccount(payOutAccount As ApsPayOutAccount)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PayOutAccountId", payOutAccount.PayOutAccountId)
         .AddWithValue("@PayOutTrxCode", payOutAccount.PayOutTrxCode)
         .AddWithValue("@PayOutTrxName", payOutAccount.PayOutTrxName)
         .AddWithValue("@AccountId", payOutAccount.AccountId)
      End With

   End Sub
   Private Sub DeletePayOutAccount(payOutAccountId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayOutAccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsPayOutAccount", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PayOutAccountId", payOutAccountId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class PayOutAccountBody
   Inherits ApsPayOutAccount

End Class
