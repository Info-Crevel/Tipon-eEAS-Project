<RoutePrefix("api")>
Public Class ARS0010_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetReferences_ARS0010")>
    <Route("references/ars0010")>
    <HttpGet>
    Public Function GetReferences_ARS0010() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.ARS0010_References")
                With _direct

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "clientType"
                            .Tables(1).TableName = "creditStatus"
                        End With

                        Return Me.Ok(_dataSet)

                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetClient")>
    <Route("clients/{clientId}")>
    <HttpGet>
    Public Function GetClient(clientId As Integer) As IHttpActionResult

        If clientId <= 0 Then
            Throw New ArgumentException("Client ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.ARS0010")

                With _direct
                    .AddParameter("ClientId", clientId)

                    'Using _dataTable As DataTable = _direct.ExecuteDataTable()
                    '    Return Me.Ok(_dataTable)
                    'End Using

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "clients"
                            .Tables(1).TableName = "contacts"
                        End With

                        Return Me.Ok(_dataSet)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    '<SymAuthorization("GetArsClientLog")>
    '<Route("clients/{clientId}/log")>
    '<HttpGet>
    'Public Function GetArsClientLog(clientId As Integer) As IHttpActionResult

    '    If clientId <= 0 Then
    '        Throw New ArgumentException("Client ID is required.")
    '    End If

    '    Try
    '        Using _direct As New SqlDirect("web.ARS0010_Log")
    '            With _direct
    '                .AddParameter("clientId", clientId)

    '                ' Allow DateTime
    '                Dim _settings As New JsonSerializerSettings
    '                With _settings
    '                    .ContractResolver = New CamelCasePropertyNamesContractResolver
    '                    .DateFormatString = "yyyy-MM-ddTHH:mm"
    '                End With

    '                Using _dataTable As DataTable = _direct.ExecuteDataTable()
    '                    'Return Me.Ok(_dataTable)
    '                    Return Json(_dataTable, _settings)
    '                End Using

    '            End With
    '        End Using

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function

    <SymAuthorization("CreateClient")>
    <Route("clients/{currentUserId}")>
    <HttpPost>
    Public Function CreateClient(currentUserId As Integer, <FromBody> client As ClientBody) As IHttpActionResult

        If client.ClientId <> -1 Then
            Throw New ArgumentException("Client ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _clientId As Integer = SysLib.GetNextSequence("ClientId")

            client.ClientId = _clientId

            '
            ' Load proposed values from payload
            '
            Dim _arsClient As New ArsClient
            Dim _arsClientContactPersonList As New ArsClientContactPersonList

            Me.LoadArsClient(client, _arsClient)
            'File.WriteAllText("d:\2.txt", client.ToString)

            ' ArsClientContactPerson
            For Each _contactPerson As ArsClientContactPerson In client.Contacts
                _contactPerson.ClientId = _clientId
                _arsClientContactPersonList.Add(_contactPerson)
            Next

            '
            ' Log addition, save to DB
            '
            'Dim _logKeyList As New LogKeyList
            'Dim _id As Integer
            ''mod.ArsClient
            'Dim _arsClientLogDetailList As New SysLogDetailList

            'With _arsClient
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.ClientName, String.Empty, .ClientName)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.Address1, String.Empty, .Address1.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.Address2, String.Empty, .Address2.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.PostalCode, String.Empty, .PostalCode.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.TaxIdNumber, String.Empty, .TaxIdNumber.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.MobileNumber, String.Empty, .MobileNumber.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.PhoneNumber, String.Empty, .PhoneNumber.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.Email, String.Empty, .Email.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.FaxNumber, String.Empty, .FaxNumber.ToString)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.ContactPerson, String.Empty, .ContactPerson.ToString)

            '    Dim _clientTypeName As String = EasSession.ArsClientType.Rows.Find(Function(m) m.ClientTypeId = .ClientTypeId).ClientTypeName
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.ClientTypeId, String.Empty, .ClientTypeId.ToString, .ClientTypeId.ToString + "=" + _clientTypeName)

            '    'AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.AccountId, String.Empty, .AccountId)

            '    Dim _creditStatusName As String = EasSession.ArsCreditStatus.Rows.Find(Function(m) m.CreditStatusId = .CreditStatusId).CreditStatusName
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.CreditStatusId, String.Empty, .CreditStatusId.ToString, .CreditStatusId.ToString + "=" + _creditStatusName)

            '    If .CreditLimit > 0 Then
            '        AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.CreditLimit, String.Empty, .CreditLimit.ToString("N"))
            '    End If

            '    'AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.DueDays, String.Empty, .DueDays)
            '    AppLib.AddLogDetail(_arsClientLogDetailList, 0, LogColumnId.Remarks, String.Empty, .Remarks.ToString)

            '    File.WriteAllText("d:\3.txt", client.ToString)
            'End With

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertArsClient(_arsClient)
                ' File.WriteAllText("d:\3.txt", client.ToString)

                If _arsClientContactPersonList.Count > 0 Then
                    'File.WriteAllText("d:\4.txt", client.ToString)
                    Me.InsertArsClientContactPerson(_arsClientContactPersonList)

                End If

                Dim _logKeyList As New LogKeyList

                'If _arsClientContactPersonList.Count > 0 Then
                '    Me.InsertArsClientContactPerson(_arsClientContactPersonList)
                'End If
                'If _arsClientLogDetailList.Count > 0 Then
                '    With _logKeyList
                '        .Clear()
                '        .Add("ClientId", _arsClient.ClientId)
                '    End With

                '    _id = AppLib.CreateLogHeader("InsArsClientLog", _logKeyList, LogActionId.Add, currentUserId)
                '    AppLib.AssignLogHeaderId(_id, _arsClientLogDetailList)
                '    AppLib.CreateLogDetails(_arsClientLogDetailList, "ArsClientLogDetail")

                'End If

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

            'Return Me.Ok(True)
            Return Me.Ok(client.ClientId)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyClient")>
    <Route("clients/{clientId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyClient(clientId As Integer, currentUserId As Integer, <FromBody> client As ClientBody) As IHttpActionResult
        If clientId <= 0 Then
            Throw New ArgumentException("Client ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _arsClient As New ArsClient
            Dim _arsClientContactPersonList As New ArsClientContactPersonList
            Me.LoadArsClient(client, _arsClient)

            'ArsClientContactPerson
            For Each _contactPerson As ArsClientContactPerson In client.Contacts
                _arsClientContactPersonList.Add(_contactPerson)
            Next
            '
            ' Load old values from DB
            '
            Dim _arsClientOld As New ArsClient
            Dim _arsClientContactPersonListOld As New ArsClientContactPersonList



            'COPY FROM HRS0010
            Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetClient(clientId), Results.OkNegotiatedContentResult(Of DataSet))

            Using _dataSet As DataSet = _result.Content
                Dim _row As DataRow = _dataSet.Tables("clients").Rows(0)
                Me.LoadArsClient(_row, _arsClientOld)
                Me.LoadArsClientContactPersonList(_dataSet.Tables("contacts").Rows, _arsClientContactPersonListOld)

            End Using

            Dim _logKeyList As New LogKeyList
            ' Dim _id As Integer



#Region "ArsClientContactPerson"

            Dim _arsClientContactPersonLogDetailList As New SysLogDetailList

            Dim _removeContactPersonCount As Integer
            Dim _addContactPersonCount As Integer
            Dim _editContactPersonCount As Integer

            ' Mark details for deletion if not found in new list
            File.WriteAllText("d:\del.txt", client.ToString)
            For Each _old As ArsClientContactPerson In _arsClientContactPersonListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientContactPerson In _arsClientContactPersonList
                    If _new.ContactDetailId = _old.ContactDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    File.WriteAllText("d:\del1.txt", client.ToString)
                    _removeContactPersonCount = _removeContactPersonCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch
            For Each _new As ArsClientContactPerson In _arsClientContactPersonList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientContactPerson In _arsClientContactPersonListOld
                    If _new.ContactDetailId = _old.ContactDetailId Then

                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .ContactEmail <> _old.ContactEmail OrElse .ContactMobileNumber <> _old.ContactMobileNumber OrElse .ContactPerson <> _old.ContactPerson OrElse .ContactPosition <> _old.ContactPosition Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addContactPersonCount = _addContactPersonCount + 1

                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editContactPersonCount = _editContactPersonCount + 1
                End If

            Next

            Dim _ArsClientContactPersonListNew As New ArsClientContactPersonList      ' for adding new Trx Details


            If _addContactPersonCount > 0 Then
                Dim _ArsClientContactPerson As ArsClientContactPerson
                For Each _new As ArsClientContactPerson In _arsClientContactPersonList
                    If _new.LogActionId = LogActionId.Add Then
                        _ArsClientContactPerson = New ArsClientContactPerson
                        _ArsClientContactPersonListNew.Add(_ArsClientContactPerson)
                        DataLib.ScatterValues(_new, _ArsClientContactPerson)
                        _ArsClientContactPerson.ClientId = client.ClientId
                    End If
                Next

            End If

#End Region


            ' Apply and log changes, save to DB
            '

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If
                'File.WriteAllText("d:\7.txt", client.ToString)
                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection


                Me.UpdateArsClient(_arsClient)



                'ArsClientContactPerson

                If _addContactPersonCount > 0 Then
                    Me.InsertArsClientContactPerson(_arsClientContactPersonListNew)

                    For Each _new As ArsClientContactPerson In _arsClientContactPersonListNew

                        With _logKeyList
                            '.Clear()
                            .Add("ClientId", _new.ClientId)

                        End With

                    Next

                End If

                If _editContactPersonCount > 0 Then
                    Me.UpdateArsClientContactPerson(_arsClientContactPersonList)

                    For Each _new As ArsClientContactPerson In _arsClientContactPersonList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("ClientId", _new.ClientId)

                            End With
                        End If
                    Next
                End If

                If _removeContactPersonCount > 0 Then
                    Me.DeleteArsClientContactPersonDetails(_arsClientContactPersonListOld)
                    For Each _old As ArsClientContactPerson In _arsClientContactPersonListOld
                        'If _old.LogActionId = LogActionId.Delete Then
                        With _logKeyList
                            .Clear()
                            .Add("ClientId", clientId)
                            '.Add("AccountId", _old.AccountId)
                        End With

                        'End If
                    Next

                End If
                File.WriteAllText("d:\cleint9.txt", client.ToString)


                File.WriteAllText("d:\cleint10.txt", client.ToString)


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

    <SymAuthorization("RemoveClient")>
    <Route("clients/{clientId}/{lockId}/{currentUserId}")>
    <HttpDelete>
    Public Function RemoveClient(clientId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

        If clientId <= 0 Then
            Throw New ArgumentException("Client ID is required.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)
            If currentUserId > 0 Then
                _currentUserId = currentUserId
            End If

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.DeleteArsClient(clientId, lockId)

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
    Private Sub LoadArsClient(client As ClientBody, arsClient As ArsClient)

        DataLib.ScatterValues(client, arsClient)

    End Sub
    Private Sub LoadArsClient(row As DataRow, client As ArsClient)

        With client
            .ClientId = row.ToInt32("ClientId")
            .ClientName = row.ToString("ClientName")
            '.ClientTypeId = row.ToInt32("ClientTypeId")--pbn03272025
            .CreditStatusId = row.ToInt32("CreditStatusId")
            .Address1 = row.ToString("Address1")
            .Address2 = row.ToString("Address2")
            .PostalCode = row.ToString("PostalCode")
            .TaxIdNumber = row.ToString("TaxIdNumber")
            .PhoneNumber = row.ToString("PhoneNumber")
            .PhoneNumber = row.ToString("PhoneNumber")
            .MobileNumber = row.ToString("MobileNumber")
            .FaxNumber = row.ToString("FaxNumber")
            .Email = row.ToString("Email")
            .ContactPerson = row.ToString("ContactPerson")
            .Remarks = row.ToString("Remarks")
            .CreditLimit = row.ToDecimal("CreditLimit")
            .DueDays = row.ToInt32("DueDays")
            .AccountId = row.ToInt32("AccountId")
            '.Amount = row.ToDecimal("Amount")
        End With

    End Sub
    Private Sub LoadArsClientContactPersonList(rows As DataRowCollection, list As ArsClientContactPersonList)

        Dim _detail As ArsClientContactPerson

        For Each _row As DataRow In rows
            _detail = New ArsClientContactPerson

            With _detail
                .ContactDetailId = _row.ToInt32("ContactDetailId")

                .ContactPerson = _row.ToString("ContactPerson")
                .ContactMobileNumber = _row.ToString("ContactMobileNumber")
                .ContactEmail = _row.ToString("ContactEmail")
                .ContactPosition = _row.ToString("ContactPosition")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub InsertArsClient(client As ArsClient)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClient", client, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClient(client)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub InsertArsClientContactPerson(list As ArsClientContactPersonList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ContactDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientContactPerson", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientContactPerson In list
                Me.AddInsertUpdateParamsArsClientContactPerson(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub UpdateArsClient(client As ArsClient)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClient", client, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClient(client)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(client.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateArsClientContactPerson(list As ArsClientContactPersonList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ContactDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientContactPerson", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientContactPerson In list
                'Me.AddInsertUpdateParamsArsClientContactPerson(_detail)
                '.Parameters.AddWithValue("@ContactDetailId", _detail.ContactDetailId)
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientContactPerson(_detail)
                    .Parameters.AddWithValue("@ContactDetailId", _detail.ContactDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub AddInsertUpdateParamsArsClient(client As ArsClient)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientId", client.ClientId)
            .AddWithValue("@ClientName", client.ClientName)

            '.AddWithValue("@ClientTypeId", client.ClientTypeId)--pbn03272025
            .AddWithValue("@CreditStatusId", client.CreditStatusId)

            .AddWithValue("@Address1", client.Address1)
            .AddWithValue("@Address2", client.Address2.ToNullable)
            .AddWithValue("@PostalCode", client.PostalCode.ToNullable)
            .AddWithValue("@TaxIdNumber", client.TaxIdNumber.ToNullable)
            .AddWithValue("@PhoneNumber", client.PhoneNumber.ToNullable)
            .AddWithValue("@MobileNumber", client.MobileNumber.ToNullable)
            .AddWithValue("@FaxNumber", client.FaxNumber.ToNullable)
            .AddWithValue("@Email", client.Email.ToNullable)
            .AddWithValue("@ContactPerson", client.ContactPerson.ToNullable)
            .AddWithValue("@Remarks", client.Remarks.ToNullable)
            .AddWithValue("@CreditLimit", client.CreditLimit)
            .AddWithValue("@DueDays", client.DueDays)

            .AddWithValue("@AccountId", client.AccountId.ToNullable)
            .AddWithValue("@NoStatementFlag", client.NoStatementFlag)
            .AddWithValue("@PurgeFlag", client.PurgeFlag)

        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientContactPerson(contacts As ArsClientContactPerson)

        With DataCore.Command.Parameters
            .Clear()


            .AddWithValue("@ClientId", contacts.ClientId)
            .AddWithValue("@ContactPerson", contacts.ContactPerson)
            .AddWithValue("@ContactMobileNumber", contacts.ContactMobileNumber)
            .AddWithValue("@ContactEmail", contacts.ContactEmail)
            .AddWithValue("@ContactPosition", contacts.ContactPosition)
        End With

    End Sub

    Private Sub DeleteArsClient(clientId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsClient", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ClientId", clientId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With
    End Sub

    Private Sub DeleteArsClientContactPersonDetails(list As ArsClientContactPersonList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientContactPerson WHERE ContactDetailId=@ContactDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientContactPerson In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ContactDetailId", _old.ContactDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
End Class
Public Class ClientBody
    Inherits ArsClient

    Public Property Contacts As ArsClientContactPerson()

End Class
