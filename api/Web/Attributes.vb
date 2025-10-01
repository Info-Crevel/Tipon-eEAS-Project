Imports System.Web.Http.Controllers

<AttributeUsage(AttributeTargets.Method)>
Public Class SymAuthorizationAttribute
   Inherits System.Web.Http.AuthorizeAttribute

   Private ReadOnly _methodName As String

   Private Sub New()
      MyBase.New()
   End Sub

   Public Sub New(methodName As String)
      _methodName = methodName
   End Sub

   Protected Overrides Function IsAuthorized(actionContext As HttpActionContext) As Boolean

      Dim _tokens As IEnumerable(Of String) = Enumerable.Empty(Of String)
      If Not actionContext.Request.Headers.TryGetValues("Token", _tokens) Then
         Return False
      End If

      Dim _token As String = _tokens.FirstOrDefault

      If String.IsNullOrWhiteSpace(_token) Then
         Return False
      End If

      Dim _expiryDate As Date = Tags.EmptyDate

      If SysLib.AuthenticatedTokens.ContainsKey(_token) Then
         _expiryDate = SysLib.AuthenticatedTokens(_token)
      Else
         '
         ' Check for authenticated token from user session
         '
         Dim _result As Object
         Using _direct As New SqlDirect("SELECT ExpiryDate FROM dbo.SecUserSession WHERE Token=@Token", CommandType.Text)
            With _direct
               .AddParameter("Token", _token).DbType = DbType.String
               _result = .ExecuteScalar()
            End With
         End Using

         If Not (DBNull.Value.Equals(_result) OrElse _result Is Nothing) Then
            _expiryDate = CDate(_result)

            If Date.Now < _expiryDate Then
               With SysLib.AuthenticatedTokens
                  .Add(_token, _expiryDate)
               End With
            End If
         End If

      End If

      If _expiryDate = Tags.EmptyDate Then
         '
         ' Check against issued Web Tokens for API-only clients 
         '
         If Session.SYS.ActiveTokenCount > 0 Then

            Dim _tokenFound As Boolean

            With SysLib.ApiTokens
               If .ContainsKey(_token) Then
                  _expiryDate = .Item(_token)
                  _tokenFound = True
               End If
            End With

            If Not _tokenFound Then
               Using _direct As New SqlDirect("SELECT ExpiryDate FROM dbo.SecToken WHERE Token=" + _token.Enclose(EncloseCharacter.Quote) + " AND PurgeFlag=0", CommandType.Text)
                  Using _table As DataTable = _direct.ExecuteDataTable
                     With _table
                        If .Rows.Count = 0 Then
                           Return False
                        End If

                        _tokenFound = True
                        _expiryDate = .Rows(0).ToDate("ExpiryDate")
                     End With
                  End Using
               End Using

               If _tokenFound Then
                  '
                  ' Cache to list of api-only tokens to avoid hitting the DB each time
                  '
                  With SysLib.ApiTokens
                     .Add(_token, _expiryDate)
                  End With
               End If

            End If

            If Not _tokenFound Then
               Return False
            End If

            If Date.Now >= _expiryDate AndAlso _expiryDate <> Tags.EmptyDate Then
               '
               ' Expired token
               '
               Return False
            End If

            '
            ' Validate requested method against actions assigned to token
            '
            With SysLib.ValidMethods
               If .ContainsKey(_token) Then
                  With .Item(_token)
                     If .Contains(_methodName) Then
                        Return True
                     End If
                  End With
               End If
            End With

            Using _direct As New SqlDirect("SELECT COUNT(*) FROM web.QSecTokenMethod WHERE Token=" + _token.Enclose(EncloseCharacter.Quote) + " AND MethodName=" + _methodName.Enclose(EncloseCharacter.Quote), CommandType.Text)
               With _direct
                  Dim _count As Integer = CInt(.ExecuteScalar)
                  If _count > 0 Then
                     '
                     ' Cache to list of validated methods to avoid hitting the DB each time
                     '
                     With SysLib.ValidMethods
                        If Not .ContainsKey(_token) Then
                           .Add(_token, New MethodList)
                        End If
                        With .Item(_token)
                           If Not .Contains(_methodName) Then
                              .Add(_methodName)
                           End If
                        End With
                     End With

                  End If

               End With
            End Using

            Return True

         End If

         Return False

      Else
         If Date.Now >= _expiryDate Then
            '
            ' Expired token
            '
            Return False
         End If

         Return True

      End If

   End Function

   'Protected Overrides Function IsAuthorized(actionContext As HttpActionContext) As Boolean

   '   Dim _tokens As IEnumerable(Of String) = Enumerable.Empty(Of String)
   '   If Not actionContext.Request.Headers.TryGetValues("Token", _tokens) Then
   '      Return False
   '   End If

   '   Dim _token As String = _tokens.FirstOrDefault

   '   If String.IsNullOrWhiteSpace(_token) Then
   '      Return False
   '   End If

   '   Dim _expiryDate As Date = Tags.EmptyDate

   '   If SysLib.AuthenticatedTokens.ContainsKey(_token) Then
   '      _expiryDate = SysLib.AuthenticatedTokens(_token)
   '   Else
   '      '
   '      ' Check for authenticated token from user session
   '      '
   '      Dim _result As Object
   '      Using _direct As New SqlDirect("SELECT ExpiryDate FROM dbo.SecUserSession WHERE Token=@Token", CommandType.Text)
   '         With _direct
   '            .AddParameter("Token", _token).DbType = DbType.String
   '            _result = .ExecuteScalar()
   '         End With
   '      End Using

   '      If Not (DBNull.Value.Equals(_result) OrElse _result Is Nothing) Then
   '         _expiryDate = CDate(_result)

   '         If Date.Now < _expiryDate Then
   '            With SysLib.AuthenticatedTokens
   '               .Add(_token, _expiryDate)
   '            End With
   '         End If
   '      End If

   '   End If

   '   If _expiryDate = Tags.EmptyDate Then
   '      '
   '      ' Check against issued Web Tokens for API-only clients 
   '      '
   '      If Session.SYS.ActiveTokenCount > 0 Then

   '         Dim _tokenFound As Boolean

   '         With SysLib.ApiTokens
   '            If .ContainsKey(_token) Then
   '               _expiryDate = .Item(_token)
   '               _tokenFound = True
   '            End If
   '         End With

   '         If Not _tokenFound Then
   '            Using _direct As New SqlDirect("SELECT ExpiryDate FROM dbo.SecToken WHERE Token=" + _token.Enclose(EncloseCharacter.Quote) + " AND PurgeFlag=0", CommandType.Text)
   '               Using _table As DataTable = _direct.ExecuteDataTable
   '                  With _table
   '                     If .Rows.Count = 0 Then
   '                        Return False
   '                     End If

   '                     _tokenFound = True
   '                     _expiryDate = .Rows(0).ToDate("ExpiryDate")
   '                  End With
   '               End Using
   '            End Using

   '            If _tokenFound Then
   '               '
   '               ' Cache to list of api-only tokens to avoid hitting the DB each time
   '               '
   '               With SysLib.ApiTokens
   '                  .Add(_token, _expiryDate)
   '               End With
   '            End If

   '         End If

   '         If Not _tokenFound Then
   '            Return False
   '         End If

   '         If Date.Now >= _expiryDate AndAlso _expiryDate <> Tags.EmptyDate Then
   '            '
   '            ' Expired token
   '            '
   '            Return False
   '         End If

   '         '
   '         ' Validate requested method against actions assigned to token
   '         '
   '         With SysLib.ValidMethods
   '            If .ContainsKey(_token) Then
   '               With .Item(_token)
   '                  If .Contains(_methodName) Then
   '                     Return True
   '                  End If
   '               End With
   '            End If
   '         End With

   '         Using _direct As New SqlDirect("SELECT COUNT(*) FROM web.QSecTokenMethod WHERE Token=" + _token.Enclose(EncloseCharacter.Quote) + " AND MethodName=" + _methodName.Enclose(EncloseCharacter.Quote), CommandType.Text)
   '            With _direct
   '               Dim _count As Integer = CInt(.ExecuteScalar)
   '               If _count > 0 Then
   '                  '
   '                  ' Cache to list of validated methods to avoid hitting the DB each time
   '                  '
   '                  With SysLib.ValidMethods
   '                     If Not .ContainsKey(_token) Then
   '                        .Add(_token, New MethodList)
   '                     End If
   '                     With .Item(_token)
   '                        If Not .Contains(_methodName) Then
   '                           .Add(_methodName)
   '                        End If
   '                     End With
   '                  End With

   '                  Return True
   '               End If

   '            End With
   '         End Using

   '      End If

   '      Return False

   '   Else
   '      If Date.Now >= _expiryDate Then
   '         '
   '         ' Expired token
   '         '
   '         Return False
   '      End If

   '      Return True

   '   End If

   'End Function

End Class
