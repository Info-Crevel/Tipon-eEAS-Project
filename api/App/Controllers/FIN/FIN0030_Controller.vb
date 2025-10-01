<RoutePrefix("api")>
Public Class FIN0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAcctBanks")>
   <Route("acct-banks")>
   <HttpGet>
   Public Function GetAcctBanks() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.FIN0030_All")
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

   <SymAuthorization("GetAcctBank")>
   <Route("acct-banks/{bankId}")>
   <HttpGet>
   Public Function GetAcctBank(bankId As Integer) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.FIN0030")
            With _direct
               .AddParameter("BankId", bankId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAcctBank")>
   <Route("acct-banks/{currentUserId}")>
   <HttpPost>
   Public Function CreateAcctBank(currentUserId As Integer, <FromBody> bank As FinBankBody) As IHttpActionResult

      If bank.BankId <> -1 Then
         Throw New ArgumentException("Bank ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _bankId As Integer = SysLib.GetNextSequence("BankId")

         bank.BankId = _bankId

         '
         ' Load proposed values from payload
         '
         Dim _finBank As New FinBank

         Me.LoadFinBank(bank, _finBank)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertFinBank(_finBank)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAcctBank(_bankId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAcctBank")>
   <Route("acct-banks/{bankId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAcctBank(bankId As Integer, currentUserId As Integer, <FromBody> bank As FinBankBody) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _finBank As New FinBank

         Me.LoadFinBank(bank, _finBank)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateFinBank(_finBank)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAcctBank(bankId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveAcctBank")>
   <Route("acct-banks/{bankId}")>
   <HttpDelete>
   Public Function RemoveAcctBank(bankId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
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
            Me.DeleteFinBank(bankId, q.LockId)

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
   Private Sub LoadFinBank(bank As FinBankBody, finBank As FinBank)

      DataLib.ScatterValues(bank, finBank)

   End Sub
   Private Sub LoadFinBank(row As DataRow, finBank As FinBank)

      With finBank
         .BankId = row.ToInt32("BankId")
         .BankName = row.ToString("BankName")
         .BankNextSeqId = row.ToInt32("BankNextSeqId")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub InsertFinBank(bank As FinBank)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinBank", bank, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinBank(bank)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateFinBank(bank As FinBank)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinBank", bank, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinBank(bank)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(bank.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinBank(bank As FinBank)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@BankId", bank.BankId)
         .AddWithValue("@BankName", bank.BankName)
         .AddWithValue("@BankNextSeqId", bank.BankNextSeqId)
         .AddWithValue("@AccountId", bank.AccountId)
      End With

   End Sub
   Private Sub DeleteFinBank(bankId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinBank", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@BankId", bankId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class FinBankBody
   Inherits FinBank

End Class
