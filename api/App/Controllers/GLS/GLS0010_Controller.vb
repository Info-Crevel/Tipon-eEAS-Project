<RoutePrefix("api")>
Public Class GLS0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_GLS0010")>
   <Route("references/gls0010")>
   <HttpGet>
   Public Function GetReferences_GLS0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.GLS0010_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "types"      ' Asset, Liability, Equity, Reveneue, Expense
                     .Tables(1).TableName = "natures"    ' Debit, Credit
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFinAccount")>
   <Route("fin-accounts/{accountId}")>
   <HttpGet>
   Public Function GetFinAccount(accountId As String) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Dim _accountId As String = accountId.Trim

      Try
         Using _direct As New SqlDirect("web.GLS0010")
            With _direct
               .AddParameter("AccountId", _accountId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateFinAccount")>
   <Route("fin-accounts")>
   <HttpPost>
   Public Function CreateFinAccount(<FromBody> account As FinAccountBody) As IHttpActionResult

      If String.IsNullOrEmpty(account.AccountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _finAccount As New FinAccount

         Me.LoadFinAccount(account, _finAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertFinAccount(_finAccount)

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

   <SymAuthorization("ModifyFinAccount")>
   <Route("fin-accounts/{accountId}")>
   <HttpPut>
   Public Function ModifyFinAccount(accountId As String, <FromBody> account As FinAccountBody) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _finAccount As New FinAccount

         Me.LoadFinAccount(account, _finAccount)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateFinAccount(_finAccount)

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

   <SymAuthorization("RemoveFinAccount")>
   <Route("fin-accounts/{accountId}")>
   <HttpDelete>
   Public Function RemoveFinAccount(accountId As String, <FromUri> q As DeleteQuery) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
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

            Me.DeleteFinAccount(accountId, q.LockId)

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

   Private Sub LoadFinAccount(account As FinAccountBody, finAccount As FinAccount)

      DataLib.ScatterValues(account, finAccount)

   End Sub

   Private Sub LoadFinAccount(row As DataRow, finAccount As FinAccount)

      With finAccount
         .AccountId = row.ToString("AccountId")
            .AccountName = row.ToString("AccountName")
            .AccountDescription = row.ToString("AccountDescription")
            .AccountTypeId = row.ToInt32("AccountTypeId")
            .AccountNatureId = row.ToInt32("AccountNatureId")
         .HeaderAccountId = row.ToString("HeaderAccountId")
      End With

   End Sub

   Private Sub InsertFinAccount(account As FinAccount)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinAccount", account, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinAccount(account)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFinAccount(account As FinAccount)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinAccount", account, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinAccount(account)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(account.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinAccount(account As FinAccount)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AccountId", account.AccountId)
            .AddWithValue("@AccountName", account.AccountName)
            .AddWithValue("@AccountDescription", account.AccountDescription)
            .AddWithValue("@AccountTypeId", account.AccountTypeId)
         .AddWithValue("@AccountNatureId", account.AccountNatureId)
         .AddWithValue("@HeaderAccountId", account.HeaderAccountId.ToNullable)

      End With

   End Sub

   Private Sub DeleteFinAccount(accountId As String, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinAccount", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AccountId", accountId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class FinAccountBody
   Inherits FinAccount

End Class
