<RoutePrefix("api")>
Public Class WEB_Controller
    Inherits ApiController

    '<SymAuthorization("RequestPasswordReset")>
    <Route("passwords/request")>
    <HttpPut>
    Public Function RequestPasswordReset(<FromUri> q As RequestResetPasswordQuery) As IHttpActionResult

        If String.IsNullOrEmpty(q.AccessId) Then
            Throw New ArgumentException("Logon ID is required.")
        End If

        If String.IsNullOrEmpty(q.PhoneNumberEndDigits) Then
            Throw New ArgumentException("Phone Number ending digits required.")
        End If

        Dim _info As New PasswordResetInfo

        Try

            Dim _securityCode As String = SysLib.RandomInteger(1001, 9999).ToString()
            Dim _filter As String = "(Email=" + q.AccessId.Enclose(EncloseCharacter.Quote) + " OR LogonId=" + q.AccessId.Enclose(EncloseCharacter.Quote) + ") AND RIGHT(PhoneNumber, 2) = " + q.PhoneNumberEndDigits.Enclose(EncloseCharacter.Quote)
            Dim _fields As String = "LogOnId, Password, PhoneNumber"

            Dim _logonId As String = ""
            Dim _mobileNumber As String = ""

            Using _table As DataTable = DataLib.GetList("dbo.SecUser", _fields, _filter, String.Empty)

                With _table
                    If .Rows.Count > 0 Then
                        Dim _row As DataRow = _table.Rows(0)

                        _logonId = _row.ToString("LogOnId")
                        _mobileNumber = _row.ToString("PhoneNumber")

                        With _info
                            .Password = _row.ToString("Password")
                        End With

                    End If

                End With
            End Using

            If _logonId.Length > 0 Then
                '
                ' Queue SMS
                '

                Using _direct As New SqlDirect("web.SecSMSPasswordReset", CommandType.StoredProcedure)
                    With _direct
                        .AddParameter("LogOnId", _logonId).DbType = DbType.String
                        .AddParameter("VerificationCode", _securityCode)
                        .AddParameter("Message", "Your password reset PIN for eEAS-Tipon " + _securityCode + ". This is a system-generated message, please do not reply.")

                        .ExecuteNonQuery()

                    End With
                End Using


            End If


            Return Me.Ok(_info)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    '<SymAuthorization("ResetAccountPassword")>
    <Route("passwords/reset")>
    <HttpPut>
    Public Function ResetAccountPassword(<FromBody> q As ResetPasswordBody) As IHttpActionResult

        If String.IsNullOrEmpty(q.AccessId) Then
            Throw New ArgumentException("Access ID is required.")
        End If

        If String.IsNullOrEmpty(q.PhoneNumberEndDigits) Then
            Throw New ArgumentException("Phone Number ending digits required.")
        End If

        If String.IsNullOrEmpty(q.NewPassword) Then
            Throw New ArgumentException("New password is required.")
        End If

        Try

            Dim _filter As String = "(Email=" + q.AccessId.Enclose(EncloseCharacter.Quote) + " OR LogonId=" + q.AccessId.Enclose(EncloseCharacter.Quote) + ") AND RIGHT(PhoneNumber, 2) = " + q.PhoneNumberEndDigits.Enclose(EncloseCharacter.Quote)
            Dim _fields As String = "LogOnId, Password, SMSVerificationCode"

            Dim _logOnId As String = ""
            Dim _oldPassword As String = String.Empty
            Dim _smsVerificationCode As String = String.Empty

            Using _table As DataTable = DataLib.GetList("dbo.SecUser", _fields, _filter, String.Empty)

                With _table
                    If .Rows.Count > 0 Then
                        Dim _row As DataRow = _table.Rows(0)

                        _logOnId = _row.ToString("LogOnId")
                        _oldPassword = _row.ToString("Password")
                        _smsVerificationCode = _row.ToString("SMSVerificationCode")

                    End If

                End With

            End Using

            If _logOnId.Length > 0 AndAlso _oldPassword = q.OldPassword AndAlso _smsVerificationCode = q.SecurityCode Then

                Dim _sql As String = "UPDATE dbo.SecUser SET Password=@Password WHERE LogOnId=@LogOnId"
                Using _direct As New SqlDirect(_sql, CommandType.Text)
                    With _direct
                        .AddParameter("@LogOnId", _logOnId)
                        .AddParameter("@Password", SysLib.Hash(q.AccessId + ";" + q.NewPassword))

                        .ExecuteNonQuery()
                    End With
                End Using

                Return Me.Ok(True)
            Else
                Return Me.Ok(False)
            End If

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    '<SymAuthorization("ResetAccountPassword")>
    <Route("passwords/reset-default")>
    <HttpPut>
    Public Function ResetDefaultAccountPassword(<FromBody> q As ResetDefaultPasswordBody) As IHttpActionResult

        If String.IsNullOrEmpty(q.AccessId) Then
            Throw New ArgumentException("Access ID is required.")
        End If

        If String.IsNullOrEmpty(q.NewPassword) Then
            Throw New ArgumentException("New password is required.")
        End If

        Try
            Dim _filter As String = "LogonId=" + q.AccessId.Enclose(EncloseCharacter.Quote) + " AND Password = " + SysLib.Hash(q.AccessId + ";" + q.OldPassword).Enclose(EncloseCharacter.Quote)
            Dim _fields As String = "LogOnId, Password"
            Dim _logOnId As String = ""
            Dim _oldPassword As String = String.Empty
            'Dim _smsVerificationCode As String = String.Empty
            Using _table As DataTable = DataLib.GetList("dbo.SecUser", _fields, _filter, String.Empty)

                With _table
                    If .Rows.Count > 0 Then
                        Dim _row As DataRow = _table.Rows(0)

                        _logOnId = _row.ToString("LogOnId")
                        _oldPassword = _row.ToString("Password")

                    End If

                End With

            End Using
            '.AddParameter("Password", SysLib.Hash(_logOnId + ";" + _password))
            If _logOnId.Length > 0 Then

                Dim _sql As String = "UPDATE dbo.SecUser SET Password=@Password, WebChangePasswordFlag = 0 WHERE LogOnId=@LogOnId"
                Using _direct As New SqlDirect(_sql, CommandType.Text)
                    With _direct
                        .AddParameter("@LogOnId", _logOnId)
                        .AddParameter("@Password", SysLib.Hash(q.AccessId + ";" + q.NewPassword))

                        .ExecuteNonQuery()
                    End With
                End Using

                Return Me.Ok(True)
            Else
                Return Me.Ok(False)
            End If

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    '<SymAuthorization("RequestPasswordReset")>
    <Route("passwords/validate")>
    <HttpGet>
    Public Function ValidateAccount(<FromUri> q As ValidateAccountQuery) As IHttpActionResult

        If String.IsNullOrEmpty(q.AccessId) Then
            Throw New ArgumentException("Logon ID is required.")
        End If

        If String.IsNullOrEmpty(q.Password) Then
            Throw New ArgumentException("Old passsword is required.")
        End If

        Dim _info As New PasswordResetInfo

        Try

            Dim _result As Integer = 0

            Dim _filter As String = "LogonId=" + q.AccessId.Enclose(EncloseCharacter.Quote) + " AND Password = " + SysLib.Hash(q.AccessId + ";" + q.Password).Enclose(EncloseCharacter.Quote)
            Dim _fields As String = "LogOnId, Password"

            Dim _logonId As String = ""

            Using _table As DataTable = DataLib.GetList("dbo.SecUser", _fields, _filter, String.Empty)

                With _table
                    If .Rows.Count > 0 Then
                        _result = 1
                    End If

                End With
            End Using

            'If _logonId.Length > 0 Then
            '    '
            '    ' Queue SMS
            '    '

            '    Using _direct As New SqlDirect("web.SecSMSPasswordReset", CommandType.StoredProcedure)
            '        With _direct
            '            .AddParameter("LogOnId", _logonId).DbType = DbType.String
            '            .AddParameter("VerificationCode", _securityCode)
            '            .AddParameter("Message", "Your password reset PIN for eEAS-Tipon " + _securityCode + ". This is a system-generated message, please do not reply.")

            '            .ExecuteNonQuery()

            '        End With
            '    End Using


            'End If

            'File.WriteAllText("e:\OK.txt", _result.ToString)

            Return Me.Ok(_result)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("UpdateUserOrg")>
    <Route("user-org/{token}/{orgId}")>
    <HttpPut>
    Public Function UpdateUserOrg(token As String, orgId As Integer) As IHttpActionResult

        Try
            Dim _sql As String = "UPDATE dbo.SecUserSession SET OrgId=@OrgId WHERE Token=@Token"
            Using _direct As New SqlDirect(_sql, CommandType.Text)
                With _direct
                    .AddParameter("@Token", token)
                    .AddParameter("@OrgId", orgId)
                    .ExecuteNonQuery()
                End With
            End Using

            Return Me.Ok(True)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function
End Class

Public Class ResetPasswordBody
    Public Property AccessId As String        ' Email or LogonId
    Public Property PhoneNumberEndDigits As String
    Public Property OldPassword As String     ' hashed
    Public Property NewPassword As String     ' in plain text
    Public Property SecurityCode As String

End Class

Public Class RequestResetPasswordQuery
    Public Property AccessId As String        ' Email or LogonId
    Public Property PhoneNumberEndDigits As String

End Class

Public Class ValidateAccountQuery
    Public Property AccessId As String        ' Email or LogonId
    Public Property Password As String

End Class

Public Class PasswordResetInfo
    Public Property Password As String        ' old password 

End Class

Public Class ResetDefaultPasswordBody
    Public Property AccessId As String        ' Email or LogonId
    Public Property OldPassword As String     ' hashed
    Public Property NewPassword As String     ' in plain text

End Class
