<RoutePrefix("api")>
Public Class APS0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayableAccounts")>
   <Route("payable-accounts")>
   <HttpGet>
   Public Function GetPayableAccounts() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0020_All")
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

   <SymAuthorization("GetPayableAccount")>
   <Route("payable-accounts/{payableAccountId}")>
   <HttpGet>
   Public Function GetPayableAccount(payableAccountId As Integer) As IHttpActionResult

      If payableAccountId <= 0 Then
         Throw New ArgumentException("PayableAccount ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0020")
            With _direct
               .AddParameter("PayableAccountId", payableAccountId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePayableAccount")>
   <Route("payable-accounts/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableAccount(currentUserId As Integer, <FromBody> payableAccount As PayableAccountBody) As IHttpActionResult

      If payableAccount.PayableAccountId <> -1 Then
         Throw New ArgumentException("PayableAccount ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _payableAccountId As Integer = SysLib.GetNextSequence("PayableAccountId")

         payableAccount.PayableAccountId = _payableAccountId

         '
         ' Load proposed values from payload
         '
         Dim _apsPayableAccount As New ApsPayableAccount

         Me.LoadApsPayableAccount(payableAccount, _apsPayableAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertPayableAccount(_apsPayableAccount)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableAccount(_payableAccountId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayableAccount")>
   <Route("payable-accounts/{payableAccountId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableAccount(payableAccountId As Integer, currentUserId As Integer, <FromBody> payableAccount As PayableAccountBody) As IHttpActionResult

      If payableAccountId <= 0 Then
         Throw New ArgumentException("PayableAccount ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsPayableAccount As New ApsPayableAccount

         Me.LoadApsPayableAccount(payableAccount, _apsPayableAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsPayableAccount(_apsPayableAccount)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableAccount(payableAccountId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayableAccount")>
   <Route("payable-accounts/{payableAccountId}")>
   <HttpDelete>
   Public Function RemovePayableAccount(payableAccountId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payableAccountId <= 0 Then
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
            Me.DeletePayableAccount(payableAccountId, q.LockId)

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
   Private Sub LoadApsPayableAccount(payableAccount As PayableAccountBody, finPayableAccount As ApsPayableAccount)

      DataLib.ScatterValues(payableAccount, finPayableAccount)

   End Sub
   Private Sub LoadApsPayableAccount(row As DataRow, apsPayableAccount As ApsPayableAccount)

      With apsPayableAccount
         .PayableAccountId = row.ToInt32("PayableAccountId")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub InsertPayableAccount(payableAccount As ApsPayableAccount)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsPayableAccount", payableAccount, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayableAccount(payableAccount)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsPayableAccount(payableAccount As ApsPayableAccount)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayableAccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsPayableAccount", payableAccount, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayableAccount(payableAccount)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payableAccount.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayableAccount(payableAccount As ApsPayableAccount)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PayableAccountId", payableAccount.PayableAccountId)
         .AddWithValue("@AccountId", payableAccount.AccountId)
      End With

   End Sub
   Private Sub DeletePayableAccount(payableAccountId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayableAccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsPayableAccount", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PayableAccountId", payableAccountId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class PayableAccountBody
   Inherits ApsPayableAccount

End Class
