<RoutePrefix("api")>
Public Class GLS0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   '<SymAuthorization("GetReferences_GLS0010")>
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
                     '.Tables(2).TableName = "classes"    ' Allotment Classes: PS, MOOE, FE, COs
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

   <SymAuthorization("GetFinAccountLog")>
   <Route("fin-accounts/{accountId}/log")>
   <HttpGet>
   Public Function GetFinAccountLog(accountId As String) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.GLS0010_Log")
            With _direct
               .AddParameter("AccountId", accountId)

               ' Allow DateTime
               Dim _settings As New JsonSerializerSettings
               With _settings
                  .ContractResolver = New CamelCasePropertyNamesContractResolver
                  .DateFormatString = "yyyy-MM-ddTHH:mm"
               End With

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  'Return Me.Ok(_dataTable)
                  Return Json(_dataTable, _settings)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   '<SymAuthorization("CreateFinAccount")>
   '<Route("fin-accounts")>
   '<HttpPost>
   'Public Function CreateFinAccount(<FromBody> account As FinAccountBody) As IHttpActionResult

   '   If String.IsNullOrEmpty(account.AccountId) Then
   '      Throw New ArgumentException("Account ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '
   '      Dim _glsAccount As New GlsAccount

   '      Me.LoadGlsChart(account, _glsAccount)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.InsertGlsChart(_glsAccount)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("CreateFinAccount")>
   <Route("fin-accounts/{currentUserId}")>
   <HttpPost>
   Public Function CreateFinAccount(currentUserId As Integer, <FromBody> account As FinAccountBody) As IHttpActionResult

      If String.IsNullOrEmpty(account.AccountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _glsAccount As New GlsAccount

         Me.LoadGlsChart(account, _glsAccount)

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _glsChartLogDetailList As New SysLogDetailList

         With _glsAccount
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountName, String.Empty, .AccountName)

            Dim _accountTypeName As String = FrsSession.GlsAccountType.Rows.Find(Function(m) m.AccountTypeId = .AccountTypeId).AccountTypeName
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountTypeId, String.Empty, .AccountTypeId.ToString, .AccountTypeId.ToString + "=" + _accountTypeName)

            Dim _accountNatureName As String = FrsSession.GlsAccountNature.Rows.Find(Function(m) m.AccountNatureId = .AccountNatureId).AccountNatureName
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountNatureId, String.Empty, .AccountNatureId.ToString, .AccountNatureId.ToString + "=" + _accountNatureName)

            If Not String.IsNullOrEmpty(.HeaderAccountId) Then
               AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.HeaderAccountId, String.Empty, .HeaderAccountId)
            End If
         End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertGlsChart(_glsAccount)

            If _glsChartLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AccountId", _glsAccount.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsGlsChartLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _glsChartLogDetailList)

               AppLib.CreateLogDetails(_glsChartLogDetailList, "GlsChartLogDetail")

            End If

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

   '<SymAuthorization("ModifyFinAccount")>
   '<Route("fin-accounts/{accountId}")>
   '<HttpPut>
   'Public Function ModifyFinAccount(accountId As String, <FromBody> account As FinAccountBody) As IHttpActionResult

   '   If String.IsNullOrEmpty(accountId) Then
   '      Throw New ArgumentException("Account ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '
   '      Dim _glsAccount As New GlsAccount

   '      Me.LoadGlsChart(account, _glsAccount)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.UpdateGlsChart(_glsAccount)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function

   <SymAuthorization("ModifyFinAccount")>
   <Route("fin-accounts/{accountId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFinAccount(accountId As String, currentUserId As Integer, <FromBody> account As FinAccountBody) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _glsAccount As New GlsAccount

         Me.LoadGlsChart(account, _glsAccount)

         '
         ' Load old values from DB
         '
         Dim _glsAccountOld As New GlsAccount

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetFinAccount(accountId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadGlsChart(_row, _glsAccountOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _glsChartLogDetailList As New SysLogDetailList

         With _glsAccountOld
            If .AccountName <> _glsAccount.AccountName Then
               AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountName, .AccountName, _glsAccount.AccountName)
            End If

            If .AccountTypeId <> _glsAccount.AccountTypeId Then
               Dim _oldAccountTypeName As String = FrsSession.GlsAccountType.Rows.Find(Function(m) m.AccountTypeId = .AccountTypeId).AccountTypeName
               Dim _newAccountTypeName As String = FrsSession.GlsAccountType.Rows.Find(Function(m) m.AccountTypeId = _glsAccount.AccountTypeId).AccountTypeName

               AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountTypeId, .AccountTypeId.ToString, _glsAccount.AccountTypeId.ToString, .AccountTypeId.ToString + "=" + _oldAccountTypeName + "; " + _glsAccount.AccountTypeId.ToString + "=" + _newAccountTypeName)
            End If

            If .AccountNatureId <> _glsAccount.AccountNatureId Then
               Dim _oldAccountNatureName As String = FrsSession.GlsAccountNature.Rows.Find(Function(m) m.AccountNatureId = .AccountNatureId).AccountNatureName
               Dim _newAccountNatureName As String = FrsSession.GlsAccountNature.Rows.Find(Function(m) m.AccountNatureId = _glsAccount.AccountNatureId).AccountNatureName

               AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountNatureId, .AccountNatureId.ToString, _glsAccount.AccountNatureId.ToString, .AccountNatureId.ToString + "=" + _oldAccountNatureName + "; " + _glsAccount.AccountNatureId.ToString + "=" + _newAccountNatureName)
            End If

            If .HeaderAccountId <> _glsAccount.HeaderAccountId Then
               AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.HeaderAccountId, .HeaderAccountId, _glsAccount.HeaderAccountId)
            End If
         End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateGlsChart(_glsAccount)

            If _glsChartLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AccountId", _glsAccount.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsGlsChartLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _glsChartLogDetailList)

               AppLib.CreateLogDetails(_glsChartLogDetailList, "GlsChartLogDetail")

            End If

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

   '<SymAuthorization("RemoveFinAccount")>
   '<Route("fin-accounts/{accountId}/{lockId}")>
   '<HttpDelete>
   'Public Function RemoveFinAccount(accountId As String, lockId As String) As IHttpActionResult

   '   If String.IsNullOrEmpty(accountId) Then
   '      Throw New ArgumentException("Account ID is required.")
   '   End If

   '   Try

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.DeleteGlsChart(accountId, lockId)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("RemoveFinAccount")>
   <Route("fin-accounts/{accountId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveFinAccount(accountId As String, lockId As String, currentUserId As Integer) As IHttpActionResult

      If String.IsNullOrEmpty(accountId) Then
         Throw New ArgumentException("Account ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Check for existing record
         '
         Dim _filter As String = "AccountId=" + accountId.ToString
         If DataLib.GetRecordCount("dbo.GlsChart", _filter, _databaseId) = 0 Then
            Throw New ArgumentException("Account ID not found.")
         End If

         '
         ' Load current values from DB
         '
         Dim _glsAccount As New GlsAccount

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetFinAccount(accountId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadGlsChart(_row, _glsAccount)
         End Using

         '
         ' Log deletion, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _glsChartLogDetailList As New SysLogDetailList

         With _glsAccount
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountName, .AccountName, String.Empty)

            Dim _accountTypeName As String = FrsSession.GlsAccountType.Rows.Find(Function(m) m.AccountTypeId = .AccountTypeId).AccountTypeName
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountTypeId, .AccountTypeId.ToString, String.Empty, .AccountTypeId.ToString + "=" + _accountTypeName)

            Dim _accountNatureName As String = FrsSession.GlsAccountNature.Rows.Find(Function(m) m.AccountNatureId = .AccountNatureId).AccountNatureName
            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.AccountNatureId, .AccountNatureId.ToString, String.Empty, .AccountNatureId.ToString + "=" + _accountNatureName)

            AppLib.AddLogDetail(_glsChartLogDetailList, 0, LogColumnId.HeaderAccountId, .HeaderAccountId, String.Empty)
         End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.DeleteGlsChart(accountId, lockId)

            If _glsChartLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AccountId", _glsAccount.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsGlsChartLog", _logKeyList, LogActionId.Delete, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _glsChartLogDetailList)

               AppLib.CreateLogDetails(_glsChartLogDetailList, "GlsChartLogDetail")

            End If

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

   Private Sub LoadGlsChart(account As FinAccountBody, glsAccount As GlsAccount)

      DataLib.ScatterValues(account, glsAccount)

   End Sub

   Private Sub LoadGlsChart(row As DataRow, glsAccount As GlsAccount)

      With glsAccount
         .AccountId = row.ToString("AccountId")
         .AccountName = row.ToString("AccountName")
         .AccountTypeId = row.ToInt32("AccountTypeId")
         .AccountNatureId = row.ToInt32("AccountNatureId")
         .HeaderAccountId = row.ToString("HeaderAccountId")
      End With

   End Sub

   Private Sub InsertGlsChart(account As GlsAccount)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.GlsChart", account, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsGlsChart(account)

         'With .Parameters
         '   .Clear()
         '   .AddWithValue("@AccountId", account.AccountId)
         '   .AddWithValue("@AccountName", account.AccountName)
         '   .AddWithValue("@AccountTypeId", account.AccountTypeId)
         '   .AddWithValue("@AccountNatureId", account.AccountNatureId)
         '   .AddWithValue("@HeaderAccountId", account.HeaderAccountId.ToNullable)
         'End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateGlsChart(account As GlsAccount)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.GlsChart", account, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsGlsChart(account)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(account.LockId)).DbType = DbType.Binary

         'With .Parameters
         '   .Clear()
         '   .AddWithValue("@AccountId", account.AccountId)
         '   .AddWithValue("@AccountName", account.AccountName)
         '   .AddWithValue("@AccountTypeId", account.AccountTypeId)
         '   .AddWithValue("@AccountNatureId", account.AccountNatureId)
         '   .AddWithValue("@HeaderAccountId", account.HeaderAccountId.ToNullable)
         '   .AddWithValue("@LockId", Convert.FromBase64String(account.LockId)).DbType = DbType.Binary
         'End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsGlsChart(account As GlsAccount)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AccountId", account.AccountId)
         .AddWithValue("@AccountName", account.AccountName)
         .AddWithValue("@AccountTypeId", account.AccountTypeId)
         .AddWithValue("@AccountNatureId", account.AccountNatureId)
         .AddWithValue("@HeaderAccountId", account.HeaderAccountId.ToNullable)

      End With

   End Sub

   Private Sub DeleteGlsChart(accountId As String, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AccountId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.GlsChart", _keyFields)
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
   Inherits GlsAccount

End Class
