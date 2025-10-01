Public NotInheritable Class SysLib

   Private Shared _crcTable() As Integer = {}

   Private Sub New()
      MyBase.New()
   End Sub

   'Friend Shared ReadOnly Property AuthenticatedTokens As New AuthenticatedTokenCollection
   Friend Shared ReadOnly Property AuthenticatedTokens As New TokenCollection
   'Friend Shared ReadOnly Property ValidActions As New SecuredActionsList
   Friend Shared ReadOnly Property ValidMethods As New SecuredMethodsList
   Friend Shared ReadOnly Property ApiTokens As New TokenCollection

   Public Shared Function GetNextSequence(sequencerId As String) As Integer
      Return SysLib.GetNextSequence(sequencerId, "sys")
   End Function

   Public Shared Function GetNextSequence(sequencerId As String, skipCount As Integer) As Integer
      Return SysLib.GetNextSequence(sequencerId, skipCount, "sys")
   End Function

   Public Shared Function GetNextSequence(sequencerId As String, databaseId As String) As Integer
      Return SysLib.GetNextSequence(sequencerId, 1, databaseId)
   End Function

   Public Shared Function GetNextSequence(sequencerId As String, skipCount As Integer, databaseId As String) As Integer

      With DataCore.Command
         .Connection = DataCore.Connection
         '.CommandText = "dbo.SysGetNextSequence"
         .CommandText = "dbo.ZSysGetNextSequence"
         .CommandType = CommandType.StoredProcedure
         .Parameters.Clear()

         Dim _nextSeqIdParam As SqlParameter
         Dim _nextSeqId As Integer

         With .Parameters
            .Add("@SequencerId", SqlDbType.VarChar).Value = sequencerId
            .Add("@SkipCount", SqlDbType.Int).Value = skipCount
            _nextSeqIdParam = .Add("@NextSeqId", SqlDbType.Int)
            _nextSeqIdParam.Direction = ParameterDirection.Output
         End With

         DataCore.Disconnect()
         DataCore.Connect(databaseId)
         .ExecuteScalar()
         _nextSeqId = CInt(_nextSeqIdParam.Value)
         DataCore.Disconnect()

         Return _nextSeqId

      End With

   End Function

   Public Shared Function Encrypt(rawText As String, key As String) As String

      Dim _provider As SymmetricAlgorithm = SysLib.CreateCryptoProvider(key)
      Dim _cipherText As String = Convert.ToBase64String(GetCryptoData(_provider.CreateEncryptor(), Encoding.Unicode.GetBytes(rawText)))
      _provider.Clear()
      _provider.Dispose()
      Return _cipherText

   End Function

   Public Shared Function EncryptHex(rawText As String, key As String, charCase As CharacterCase) As String

      If rawText Is Nothing Then
         Throw New ArgumentNullException("rawText")
      End If

      If key Is Nothing Then
         Throw New ArgumentNullException("key")
      End If

      If key.Trim.Length = 0 Then
         Return rawText
      End If

      Dim _cipherText As String
      Dim _seed As Byte() = System.Text.Encoding.Unicode.GetBytes(rawText + key)
      Dim _cryptoProvider As New System.Security.Cryptography.SHA1CryptoServiceProvider

      _cipherText = SysLib.HexFromBytes(_cryptoProvider.ComputeHash(_seed), charCase).Substring(0, 16)
      _cryptoProvider.Clear()

      Return _cipherText

   End Function

   Public Shared Function HexFromBytes(value As Byte(), charCase As CharacterCase) As String

      If value Is Nothing Then
         Throw New ArgumentNullException("value")
      End If

      Dim _stringBuilder As New StringBuilder
      Dim _toggleFlag As Boolean
      Dim _hexString As String
      Dim _hexCase As Char

      For Each _byte As Byte In value

         Select Case charCase
            Case CharacterCase.Lower
               _hexCase = "x"c
            Case CharacterCase.Upper
               _hexCase = "X"c

            Case Else
               If _toggleFlag Then
                  _hexCase = "x"c
               Else
                  _hexCase = "X"c
               End If
               _toggleFlag = Not _toggleFlag

         End Select

         _hexString = String.Format(CultureInfo.InvariantCulture, "{0:" + _hexCase + "2}", _byte)
         _stringBuilder.Append(_hexString)

      Next

      _hexString = _stringBuilder.ToString

      Return _hexString

   End Function

   Public Shared Function Decrypt(cipherText As String, key As String) As String

      Dim _rawText As String

      Try
         Dim _provider As SymmetricAlgorithm = SysLib.CreateCryptoProvider(key)
         _rawText = Encoding.Unicode.GetString(GetCryptoData(_provider.CreateDecryptor(), Convert.FromBase64String(cipherText)))
         _provider.Clear()
         _provider.Dispose()
      Catch When True
         _rawText = SysLib.Randomize(10)
      End Try

      Return _rawText

   End Function

   Private Shared Function GetCryptoData(transformer As ICryptoTransform, data As Byte()) As Byte()

      Dim _memoryStream As MemoryStream = Nothing
      Dim _cryptoStream As CryptoStream
      Dim _byteArray As Byte() = Nothing

      Try
         _memoryStream = New MemoryStream
         _cryptoStream = New CryptoStream(_memoryStream, transformer, CryptoStreamMode.Write)

         _cryptoStream.Write(data, 0, data.Length)
         _cryptoStream.FlushFinalBlock()

         _byteArray = _memoryStream.ToArray()
         transformer.Dispose()
      Catch When True

      Finally
         If _memoryStream IsNot Nothing Then
            _memoryStream.Dispose()
         End If
      End Try

      Return _byteArray

   End Function

   Friend Shared Function CreateCryptoProvider(key As String) As SymmetricAlgorithm

      Dim _ringCounter As String = "4017"           ' Crypto seed
      Dim _magicNumber As String = "10120618"       ' Crypto key

      'Dim _provider As New TripleDESCryptoServiceProvider
      '_provider.IV = Encoding.Unicode.GetBytes(_ringCounter.ToString(CultureInfo.InvariantCulture))

      Dim _provider As New TripleDESCryptoServiceProvider With {
        .IV = Encoding.Unicode.GetBytes(_ringCounter.ToString(CultureInfo.InvariantCulture))
      }

      If key Is Nothing Then
         _provider.Key = Encoding.Unicode.GetBytes(CStr(_magicNumber))
      Else
         If key.Length > 8 Then
            _provider.Key = Encoding.Unicode.GetBytes(key.Substring(0, 8))
         Else
            _provider.Key = Encoding.Unicode.GetBytes(key.PadRight(8, "@"c))
         End If
      End If

      Return _provider

   End Function

   Public Shared Function Checksum(value As Object) As Integer

      Dim _value As String = value.ToString
      Dim _encoding As New System.Text.UnicodeEncoding

      Dim _buffer() As Byte = _encoding.GetBytes(_value)
      Return SysLib.Checksum(New MemoryStream(_buffer))

   End Function

   Public Shared Function Checksum(stream As Stream) As Integer

      If _crcTable.Length = 0 Then
         Array.Resize(_crcTable, 256)

         Dim _polynomial As Integer = &HEDB88320    ' Official polynomial used by CRC32 in PKZip
         Dim _crc As Integer

         For _index As Integer = 0 To 255
            _crc = _index
            For _j As Integer = 1 To 8
               If (_crc And 1) = 1 Then
                  _crc = Convert.ToInt32(((_crc And &HFFFFFFFE) \ 2&) And &H7FFFFFFF)
                  _crc = _crc Xor _polynomial
               Else
                  _crc = Convert.ToInt32(((_crc And &HFFFFFFFE) \ 2&) And &H7FFFFFFF) ' (_crc And &HFFFFFFFE \ 2&) And &H7FFFFFFF
               End If
            Next
            _crcTable(_index) = _crc
         Next
      End If

      Dim _result As Integer = &HFFFFFFFF
      Dim _bufferSize As Integer = 1024
      Dim _buffer(_bufferSize) As Byte
      Dim _tableIndex As Integer

      Dim _count As Integer = stream.Read(_buffer, 0, _bufferSize)
      Do While _count > 0
         For _index As Integer = 0 To _count - 1
            _tableIndex = (_result And &HFF) Xor _buffer(_index)
            _result = ((_result And &HFFFFFF00) \ &H100) And &HFFFFFF
            _result = _result Xor _crcTable(_tableIndex)
         Next
         _count = stream.Read(_buffer, 0, _bufferSize)
      Loop

      Return Not _result

   End Function

   Public Shared Function Randomize(length As Integer) As String
      Return SysLib.Randomize(length, CharacterCase.Normal)
   End Function

   Public Shared Function Randomize(length As Integer, charCase As CharacterCase) As String

      If length <= 0 Then
         Throw New ArgumentOutOfRangeException("length")
      End If

      Dim _byteArray As Byte() = New Byte(length) {}
      Dim _provider As New RNGCryptoServiceProvider()

      _provider.GetBytes(_byteArray)            ' Randomizes the bytes in array

      Return SysLib.BytesToHex(_byteArray, charCase).Substring(0, length)

   End Function

   Public Shared Function BytesToHex(value As Byte(), charCase As CharacterCase) As String

      If value Is Nothing Then
         Throw New ArgumentNullException("value")
      End If

      Dim _stringBuilder As New StringBuilder
      Dim _toggleFlag As Boolean
      Dim _hexString As String
      Dim _hexCase As Char

      For Each _byte As Byte In value

         Select Case charCase
            Case CharacterCase.Lower
               _hexCase = "x"c
            Case CharacterCase.Upper
               _hexCase = "X"c

            Case Else
               If _toggleFlag Then
                  _hexCase = "x"c
               Else
                  _hexCase = "X"c
               End If
               _toggleFlag = Not _toggleFlag

         End Select

         _hexString = String.Format(CultureInfo.InvariantCulture, "{0:" + _hexCase + "2}", _byte)
         _stringBuilder.Append(_hexString)

      Next

      _hexString = _stringBuilder.ToString

      Return _hexString

   End Function

   Friend Shared Function ToInList(Of T)(ByVal list As IEnumerable(Of T)) As String

      If list.Count = 0 Then
         Return String.Empty
      End If

      Dim _builder As New StringBuilder
      Dim _inList As String
      Dim _item As String

      Dim _type As Type

      For Each _object As Object In list

         _type = _object.GetType

         If _type.Equals(DataLib.TypeString) Then
            _item = _object.ToString.Enclose(EncloseCharacter.Quote)
         ElseIf _type.Equals(DataLib.TypeDate) Then
            _item = CType(_object, Date).ToIsoFormat.Enclose(EncloseCharacter.Quote)
         ElseIf _type.Equals(DataLib.TypeBoolean) Then
            If CType(_object, Boolean) Then
               _item = "1"
            Else
               _item = "0"
            End If
         ElseIf _type.IsEnum Then
            _item = CStr(_object)
         Else
            _item = _object.ToString
         End If

         If _builder.Length = 0 Then
            _builder.Append("(" + _item)
         Else
            _builder.Append("," + _item)
         End If
      Next

      _inList = _builder.ToString + ")"

      Return _inList

   End Function

   Public Shared Function GetAuthenticationInfo(request As HttpRequestMessage) As AuthenticationInfo
      Return SysLib.GetAuthenticationInfo(request, "web.SysTaskLogon")
   End Function

   Public Shared Function GetAuthenticationInfo(request As HttpRequestMessage, scriptName As String) As AuthenticationInfo

      Dim _headerValue As AuthenticationHeaderValue = request.Headers.Authorization

      If _headerValue Is Nothing Then
         Return Nothing
      End If

      Dim _info As AuthenticationInfo = Nothing
      Dim _parameter As String = Nothing

      If _headerValue.Scheme = "Basic" Then
         _parameter = _headerValue.Parameter
      End If

      If String.IsNullOrWhiteSpace(_parameter) Then
         Return _info
      End If

      Try
         _parameter = Encoding.Default.GetString(Convert.FromBase64String(_parameter))
      Catch When True
         Return _info
      End Try

      Dim _credentials() As String = _parameter.Split(":"c)

      If _credentials.Length < 2 Then
         Return _info
      End If

      Dim _logonId As String = _credentials(0)
      Dim _password As String = _credentials(1)

      If String.IsNullOrWhiteSpace(_logonId) OrElse String.IsNullOrWhiteSpace(_password) Then
         Return _info
      End If

      Dim _token As String = Guid.NewGuid.ToString().Substring(24)   ' last 12 characters
      Dim _sessionId As Integer = SysLib.Checksum(_logonId + SysLib.Randomize(8))
      Dim _expiryDate As Date

      Using _direct As New SqlDirect(scriptName)

         With _direct
            .AddParameter("LogonId", _logonId)
            '.AddParameter("Password", SysLib.Encrypt(_password, _logonId))
            .AddParameter("Password", SysLib.Hash(_logonId + ";" + _password))
            .AddParameter("SessionId", _sessionId)
            .AddParameter("Token", _token)

            Dim _dataSet As DataSet = .ExecuteDataSet()
            Dim _table As DataTable = _dataSet.Tables(0)

            If _table.Rows.Count > 0 Then

               Dim _row As DataRow = _table.Rows(0)

               Dim _pages As New List(Of SysPageInfo)
               Dim _restrictions As New List(Of SysRestrictionInfo)
               Dim _modules As New List(Of SysModuleInfo)

               Dim _orgs As New List(Of SysOrgInfo)
               Dim _platforms As New List(Of SysPlatformInfo)
               'Dim _roles As String = String.Empty
               'Dim _roleCode As String = String.Empty
               'Dim _restrictionId As String = String.Empty
               'Dim _restrictionPassword As String = String.Empty

               ' 01 Mar 2025 - EMT
               Dim _actionId As String = String.Empty
               Dim _actionName As String = String.Empty
               Dim _actionPassword As String = String.Empty

               Dim _userId As Integer = _row.ToInt32("UserId")
               'Dim _Xxx As String = _row.ToString("LogOnId")
               Dim _userName As String = _row.ToString("UserName")
                    Dim _userShortName As String = _row.ToString("UserShortName")

                    Dim _userOrgId As Integer = _row.ToInt32("OrgId")
                    Dim _userOrgShortName As String = _row.ToString("OrgShortName")

                    Dim _photo As Byte() = _row.ToBytes("Photo")
               Dim _imageUrl As String = _row.ToString("ImageUrl")

               Dim _userPageId As String = _row.ToString("PageId")
               Dim _userPageName As String = _row.ToString("PageName")
               Dim _userPagePath As String = _row.ToString("PagePath")
               Dim _userModuleId As String = _row.ToString("ModuleId")

               Dim _pageId As String = String.Empty
               Dim _pageName As String = String.Empty
               Dim _pagePath As String = String.Empty

               ' 06 Feb 2025 - EMT
               Dim _pagePassword As String = String.Empty
               Dim _xa As Boolean
               Dim _xe As Boolean
               Dim _xd As Boolean

               Dim _moduleId As String = String.Empty
               Dim _moduleName As String = String.Empty
               Dim _moduleShortName As String = String.Empty

               Dim _orgId As Integer = 0
               Dim _orgName As String = String.Empty
               Dim _orgShortName As String = String.Empty

               Dim _platformId As Integer = 0
               Dim _platformName As String = String.Empty
               Dim _platformShortName As String = String.Empty

               Dim _isSysAdmin As Boolean = _row.ToBoolean("SysAdminFlag")
               Dim _workgroupName As String = _row.ToString("WorkgroupName")
               Dim _canChangePassword As Boolean = _row.ToBoolean("ChangePasswordFlag")
               Dim _webChangePassword As Boolean = _row.ToBoolean("WebChangePasswordFlag")
               Dim _userCode As String = _row.ToString("UserCode")
               'Dim _careCenterId As Integer = _row.ToInt32("CareCenterId")
               'Dim _careCenterName As String = _row.ToString("CareCenterName")
               Dim _expiredFlag As Boolean = _row.ToBoolean("ExpiredFlag")

               'For Each _dataRow As DataRow In _dataSet.Tables(1).Rows   ' Roles
               '   _roleCode = _dataRow.ToString("RoleCode").ToLowerInvariant

               '   If _roles Is String.Empty Then
               '      _roles = _roleCode
               '   Else
               '      _roles = _roles + "/" + _roleCode
               '   End If
               'Next

               'For Each _dataRow As DataRow In _dataSet.Tables(2).Rows   ' Pages
               '   _pageId = _dataRow.ToString("PageId")
               '   _pagePath = _dataRow.ToString("PagePath")
               '   _pages.Add(New SysPageInfo(_pageId, _pagePath))
               'Next

               ' 05 Feb 2025 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(2).Rows   ' Pages
                  _pageId = _dataRow.ToString("PageId")
                  _pagePath = _dataRow.ToString("PagePath")
                  _pageName = _dataRow.ToString("PageName")
                  _pagePassword = _dataRow.ToString("Password")
                  _xa = _dataRow.ToBoolean("XA")
                  _xe = _dataRow.ToBoolean("XE")
                  _xd = _dataRow.ToBoolean("XD")
                  _pages.Add(New SysPageInfo(_pageId, _pagePath, _pageName, _pagePassword, _xa, _xe, _xd))
               Next

               'For Each _dataRow As DataRow In _dataSet.Tables(3).Rows   ' Restrictions
               '   _restrictionId = _dataRow.ToString("RestrictionId").Trim.ToUpperInvariant
               '   _restrictionPassword = _dataRow.ToString("Password")
               '   _restrictions.Add(New SysRestrictionInfo(_restrictionId, _restrictionPassword))
               'Next

               ' 01 Mar 2025 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(3).Rows   ' Restrictions
                  _actionId = _dataRow.ToString("ActionId").Trim.ToUpperInvariant
                  _actionName = _dataRow.ToString("ActionDescription")
                  _actionPassword = _dataRow.ToString("Password")
                  _restrictions.Add(New SysRestrictionInfo(_actionId, _actionName, _actionPassword))
               Next

               For Each _dataRow As DataRow In _dataSet.Tables(4).Rows   ' Modules
                  _moduleId = _dataRow.ToString("ModuleId")
                  _moduleName = _dataRow.ToString("ModuleName")
                  _moduleShortName = _dataRow.ToString("ModuleShortName")
                  _modules.Add(New SysModuleInfo(_moduleId, _moduleName, _moduleShortName))
               Next

               ' 01 Mar 2025 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(6).Rows   ' Restrictions
                  _orgId = _dataRow.ToInt32("OrgId")
                  _orgName = _dataRow.ToString("OrgName")
                  _orgShortName = _dataRow.ToString("OrgShortName")
                  _orgs.Add(New SysOrgInfo(_orgId, _orgName, _orgShortName))
               Next

               ' 01 Mar 2025 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(7).Rows   ' Restrictions
                  _platformId = _dataRow.ToInt32("platformId")
                  _platformName = _dataRow.ToString("platformName")
                  _platformShortName = _dataRow.ToString("platformShortName")
                  _platforms.Add(New SysPlatformInfo(_platformId, _platformName, _platformShortName))
               Next


               '_expiryDate = _dataSet.Tables(4).Rows(0).ToDate("ExpiryDate")
               _expiryDate = _dataSet.Tables(5).Rows(0).ToDate("ExpiryDate")

               With SysLib.AuthenticatedTokens
                  If Not .ContainsKey(_token) Then
                     .Add(_token, _expiryDate)
                  End If
               End With

                    '_info = New AuthenticationInfo(_userId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _canChangePassword, _pages.ToArray, _restrictions.ToArray, _token, _userCode, _careCenterId, _careCenterName, _expiredFlag)
                    '_info = New AuthenticationInfo(_userId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _canChangePassword, _pages.ToArray, _restrictions.ToArray, _modules.ToArray, _userModuleId, _token, _userCode, _expiredFlag)
                    _info = New AuthenticationInfo(_userId, _logonId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _workgroupName, _canChangePassword, _webChangePassword, _pages.ToArray, _restrictions.ToArray, _modules.ToArray, _orgs.ToArray, _platforms.ToArray, _userModuleId, _token, _userCode, _expiredFlag, _userOrgId, _userOrgShortName)
                End If

            _dataSet.Dispose()

         End With

      End Using

      Return _info

   End Function

   Public Shared Function GetUserInfo(token As String, userCode As String) As AuthenticationInfo

      Dim _info As AuthenticationInfo = Nothing

      Using _direct As New SqlDirect("web.SysGetUserInfo")
         With _direct
            .AddParameter("Token", token)
            .AddParameter("UserCode", userCode.ToNullable)

            Dim _dataSet As DataSet = .ExecuteDataSet()
            Dim _table As DataTable = _dataSet.Tables(0)

            If _table.Rows.Count > 0 Then
               Dim _row As DataRow = _table.Rows(0)

               Dim _pages As New List(Of SysPageInfo)
               Dim _restrictions As New List(Of SysRestrictionInfo)
               Dim _modules As New List(Of SysModuleInfo)

               Dim _orgs As New List(Of SysOrgInfo)
               Dim _platforms As New List(Of SysPlatformInfo)

               'Dim _restrictionId As String = String.Empty
               'Dim _restrictionPassword As String = String.Empty

               ' 01 Mar 2025 - EMT
               Dim _actionId As String = String.Empty
               Dim _actionName As String = String.Empty
               Dim _actionPassword As String = String.Empty

               Dim _userId As Integer = _row.ToInt32("UserId")
               Dim _logOnId As String = _row.ToString("LogOnId")
               Dim _userName As String = _row.ToString("UserName")
                    Dim _userShortName As String = _row.ToString("UserShortName")
                    Dim _userOrgId As Integer = _row.ToInt32("OrgId")
                    Dim _userOrgShortName As String = _row.ToString("OrgShortName")



                    Dim _photo As Byte() = _row.ToBytes("Photo")
               Dim _imageUrl As String = _row.ToString("ImageUrl")

               Dim _userPageId As String = _row.ToString("PageId")
               Dim _userPageName As String = _row.ToString("PageName")
               Dim _userPagePath As String = _row.ToString("PagePath")
               Dim _userModuleId As String = _row.ToString("ModuleId")

               Dim _pageId As String = String.Empty
               Dim _pageName As String = String.Empty
               Dim _pagePath As String = String.Empty

               ' 06 Feb 2025 - EMT
               Dim _pagePassword As String = String.Empty
               Dim _xa As Boolean
               Dim _xe As Boolean
               Dim _xd As Boolean

               Dim _moduleId As String = String.Empty
               Dim _moduleName As String = String.Empty
               Dim _moduleShortName As String = String.Empty


               Dim _orgId As Integer = 0
               Dim _orgName As String = String.Empty
               Dim _orgShortName As String = String.Empty

               Dim _platformId As Integer = 0
               Dim _platformName As String = String.Empty
               Dim _platformShortName As String = String.Empty

               Dim _isSysAdmin As Boolean = _row.ToBoolean("SysAdminFlag")
               Dim _workgroupName As String = _row.ToString("WorkgroupName")
               Dim _canChangePassword As Boolean = _row.ToBoolean("ChangePasswordFlag")
               Dim _webChangePassword As Boolean = _row.ToBoolean("WebChangePasswordFlag")
               Dim _userCode As String = _row.ToString("UserCode")
               'Dim _careCenterId As Integer = _row.ToInt32("CareCenterId")
               'Dim _careCenterName As String = _row.ToString("CareCenterName")
               Dim _expiredFlag As Boolean = _row.ToBoolean("ExpiredFlag")

               'For Each _dataRow As DataRow In _dataSet.Tables(2).Rows   ' Pages
               '   _pageId = _dataRow.ToString("PageId")
               '   _pagePath = _dataRow.ToString("PagePath")
               '   _pages.Add(New SysPageInfo(_pageId, _pagePath))
               'Next

               ' 06 Feb 2015 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(2).Rows   ' Pages
                  _pageId = _dataRow.ToString("PageId")
                  _pagePath = _dataRow.ToString("PagePath")
                  _pageName = _dataRow.ToString("PageName")
                  _pagePassword = _dataRow.ToString("Password")
                  _xa = _dataRow.ToBoolean("XA")
                  _xe = _dataRow.ToBoolean("XE")
                  _xd = _dataRow.ToBoolean("XD")
                  _pages.Add(New SysPageInfo(_pageId, _pagePath, _pageName, _pagePassword, _xa, _xe, _xd))
               Next

               'For Each _dataRow As DataRow In _dataSet.Tables(3).Rows   ' Restrictions
               '   _restrictionId = _dataRow.ToString("RestrictionId").Trim.ToUpperInvariant
               '   _restrictionPassword = _dataRow.ToString("Password")
               '   _restrictions.Add(New SysRestrictionInfo(_restrictionId, _restrictionPassword))
               'Next

               ' 01 Mar 2025 - EMT
               For Each _dataRow As DataRow In _dataSet.Tables(3).Rows   ' Restrictions
                  _actionId = _dataRow.ToString("ActionId").Trim.ToUpperInvariant
                  _actionName = _dataRow.ToString("ActionDescription")
                  _actionPassword = _dataRow.ToString("Password")
                  _restrictions.Add(New SysRestrictionInfo(_actionId, _actionName, _actionPassword))
               Next

               For Each _dataRow As DataRow In _dataSet.Tables(4).Rows   ' Modules
                  _moduleId = _dataRow.ToString("ModuleId")
                  _moduleName = _dataRow.ToString("ModuleName")
                  _moduleShortName = _dataRow.ToString("ModuleShortName")
                  _modules.Add(New SysModuleInfo(_moduleId, _moduleName, _moduleShortName))
               Next

               For Each _dataRow As DataRow In _dataSet.Tables(5).Rows   ' Modules
                  _orgId = _dataRow.ToInt32("OrgId")
                  _orgName = _dataRow.ToString("OrgName")
                  _orgShortName = _dataRow.ToString("OrgShortName")
                  _orgs.Add(New SysOrgInfo(_orgId, _orgName, _orgShortName))
               Next

               For Each _dataRow As DataRow In _dataSet.Tables(6).Rows   ' Modules
                  _platformId = _dataRow.ToInt32("PlatformId")
                  _platformName = _dataRow.ToString("PlatformName")
                  _platformShortName = _dataRow.ToString("PlatformShortName")
                  _platforms.Add(New SysPlatformInfo(_platformId, _platformName, _platformShortName))
               Next


                    '_info = New AuthenticationInfo(_userId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _canChangePassword, _pages.ToArray, _restrictions.ToArray, token, _userCode, _careCenterId, _careCenterName, _expiredFlag)
                    '_info = New AuthenticationInfo(_userId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _canChangePassword, _pages.ToArray, _restrictions.ToArray, _modules.ToArray, _userModuleId, token, _userCode, _expiredFlag)
                    _info = New AuthenticationInfo(_userId, _logOnId, _userName, _userShortName, _photo, _imageUrl, _userPageId, _userPageName, _userPagePath, _isSysAdmin, _workgroupName, _canChangePassword, _webChangePassword, _pages.ToArray, _restrictions.ToArray, _modules.ToArray, _orgs.ToArray, _platforms.ToArray, _userModuleId, token, _userCode, _expiredFlag, _userOrgId, _userOrgShortName)

                End If

            _dataSet.Dispose()

         End With

      End Using

      Return _info

   End Function

   Public Shared Function LogOff(request As HttpRequestMessage) As Boolean

      Dim _values As IEnumerable(Of String) = Enumerable.Empty(Of String)
      If Not request.Headers.TryGetValues("Token", _values) Then
         Return False
      End If

      Dim _token As String = _values.FirstOrDefault

      Using _direct As New SqlDirect("web.SysTaskLogoff")
         With _direct
            .AddParameter("Token", _token)
            .ExecuteNonQuery()
         End With
      End Using

      With SysLib.AuthenticatedTokens
         .Remove(_token)
      End With

      Return True

   End Function

   Public Shared Function Round(value As Decimal) As Decimal
      Return Math.Round(value, 2, MidpointRounding.AwayFromZero)     ' same rounding as SQL Server/ VFP
   End Function

   Public Shared Function RandomInteger(minValue As Integer, maxValue As Integer) As Integer

      Dim _byteArray As Byte() = New Byte(3) {}
      Dim _provider As New RNGCryptoServiceProvider()
      _provider.GetBytes(_byteArray)            ' Randomizes the bytes in array

      Dim _seed As Integer = BitConverter.ToInt32(_byteArray, 0)
      Return New Random(_seed).Next(minValue, maxValue)

   End Function

   Public Shared Function Hash(rawText As String) As String

      Dim _bytes() As Byte = System.Text.Encoding.UTF8.GetBytes(rawText)
      Dim _hashOfBytes() As Byte = New System.Security.Cryptography.SHA1Managed().ComputeHash(_bytes)

      Return Convert.ToBase64String(_hashOfBytes)

   End Function

End Class
