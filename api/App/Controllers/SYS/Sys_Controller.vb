<RoutePrefix("api")>
Public Class Sys_Controller
   Inherits ApiController

   <Route("config")>
   <HttpGet>
   Public Function GetConfig() As IHttpActionResult

      Try
         Return Me.Ok(FileSystemLib.GetJsonFile("Bin\AppConfig.json"))
      Catch _exception As FileNotFoundException
         Return Me.NotFound()
      Catch _exception As Exception
         Return Me.InternalServerError(_exception)
      End Try

   End Function

   <Route("sysinfo")>
   <HttpGet>
   Public Function GetSysInfo() As IHttpActionResult
      Return Me.Ok(Session.SYS)
   End Function

   <Route("refdates")>
   <HttpGet>
   Public Function GetReferenceDates() As IHttpActionResult
      Return Me.Ok(Session.GetReferenceDates)
   End Function

   <Route("logon")>
   <HttpPost>
   Public Function LogOn() As IHttpActionResult

      Dim _info As AuthenticationInfo = SysLib.GetAuthenticationInfo(Me.Request)

      If _info Is Nothing Then
         Return Me.Unauthorized()
      End If

      If Not String.IsNullOrEmpty(_info.UserName) Then
         Return Me.Ok(_info)
      Else
         Return Me.StatusCode(Net.HttpStatusCode.ServiceUnavailable)
      End If

   End Function

   <Route("logoff")>
   <HttpDelete>
   Public Function Logoff() As IHttpActionResult

      SysLib.LogOff(Me.Request)
      Return Me.Ok()

   End Function

   <Route("userinfo")>
   <HttpGet>
   Public Function GetUserInfo(<FromUri> q As UserInfoQueryParams) As IHttpActionResult

      Try
         'Dim _info As AuthenticationInfo = SysLib.GetUserInfo(q.Token, q.AccountId)
         Dim _info As AuthenticationInfo = SysLib.GetUserInfo(q.Token, q.UserCode)
         Return Me.Ok(_info)
      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <Route("count")>
   <HttpGet>
   Public Function GetCount(<FromUri> q As CountQueryParams) As IHttpActionResult

        If String.IsNullOrEmpty(q.DataSourceName) Then
         Return Me.BadRequest("'dataSourceName' is required.")
      End If

      Dim _filter As String = q.Filter
      Dim _databaseId As String = q.DatabaseId

      If String.IsNullOrEmpty(q.Filter) Then
         _filter = String.Empty
      End If

      If String.IsNullOrEmpty(q.DatabaseId) Then
         _databaseId = Tags.SystemId
      End If

      Try
         Dim _count As Integer = DataLib.GetRecordCount(q.DataSourceName, _filter, _databaseId)
         Return Me.Ok(_count)
      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetLookup")>
   <Route("lookups/{lookupId}")>
   <HttpGet>
   Public Function GetLookup(lookupId As String) As IHttpActionResult

      Try
         'Return Me.Ok(FileSystemLib.GetJsonFile("Bin\lookup\" + lookupId + ".json"))
         Return Me.Ok(AppLib.GetJsonFile(lookupId + ".json", "lookup"))
      Catch _exception As FileNotFoundException
         Return Me.NotFound()
      Catch _exception As Exception
         Return Me.InternalServerError(_exception)
      End Try

   End Function

   <SymAuthorization("GetList")>
   <Route("list")>
   <HttpGet>
   Public Function GetList(<FromUri> q As ListQueryParams) As IHttpActionResult

      If q Is Nothing Then
         Return Me.BadRequest("Query string is required.")
      End If

      If String.IsNullOrEmpty(q.DataSource) Then
         Return Me.BadRequest("'dataSource' is required.")
      End If

      If String.IsNullOrEmpty(q.Fields) Then
         Return Me.BadRequest("'fields' is required.")
      End If

      Dim _filter As String = q.Filter
      Dim _sort As String = q.Sort

      If String.IsNullOrEmpty(q.Filter) Then
         _filter = String.Empty
      End If

      If String.IsNullOrEmpty(q.Sort) Then
         _sort = String.Empty
      End If

      Try
         Using _table As DataTable = DataLib.GetList(q.DataSource, q.Fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   '<SymAuthorization("GetSafeDeleteFlag")>
   <Route("safedel/{tableName}/{id}")>
   <HttpGet>
   Public Function GetSafeDeleteFlag(tableName As String, id As String) As IHttpActionResult
      '
      ' Check for valid table name
      '
      Dim _count As Integer = DataLib.GetRecordCount("sys.tables", "name=" + tableName.Enclose(EncloseCharacter.Quote), Tags.SystemId)
      If _count = 0 Then
         Return Me.Ok(False)
      End If

      Dim _canDeleteFlag As Boolean

      'Using _direct As New SqlDirect("dbo.SysGetFKStr")
      Using _direct As New SqlDirect("web.SysGetFKStr")
         With _direct
            .AddParameter("TableName", tableName)
            .AddParameter("RowId", id)

            Using _table As DataTable = .ExecuteDataTable()
               _canDeleteFlag = _table.Rows.Count = 0
            End Using
         End With
      End Using

      Return Me.Ok(_canDeleteFlag)

   End Function

   <Route("recaptcha")>
   <HttpPost>
   Public Async Function VerifyRecaptcha(<FromBody> payload As RecaptchaBody) As Threading.Tasks.Task(Of RecaptchaResponse)

      Dim _apiResponse As New RecaptchaResponse

      Try
         Dim _values As New Dictionary(Of String, String)

         With _values
            .Add("secret", "6LcYBzQaAAAAACoLg7RTHro5iC_yEnRDJ2S7rxUO")
            .Add("response", payload.Response)
         End With

         Dim _content As New FormUrlEncodedContent(_values)

         Using _client As New HttpClient
            Dim _response As HttpResponseMessage = Await _client.PostAsync("https://www.google.com/recaptcha/api/siteverify", _content)
            Return Await _response.Content.ReadAsAsync(Of RecaptchaResponse)
         End Using

      Catch _exception As Exception
         With _apiResponse
            .Success = False
            .HostName = _exception.Message
         End With

         Return _apiResponse
      End Try

   End Function

   <Route("amount-text")>
   <HttpGet>
   Public Function GetAmountText(<FromUri> q As AmountTextQueryParams) As IHttpActionResult

      Try
         'Dim _text As String = AppLib.NumberToText(q.Amount, FractionStyle.Word)
         Dim _text As String = AppLib.NumberToText(q.Amount, FractionStyle.PerHundred)    ' 04 Jul 2023 - EMT
         Return Me.Ok(_text)
      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

End Class

Public Class UserInfoQueryParams
   Public Property Token As String
   'Public Property AccountId As Integer
   Public Property UserCode As String

End Class

Public Class CountQueryParams
   Public Property DataSourceName As String
   Public Property Filter As String
   Public Property DatabaseId As String

End Class

Public Class ListQueryParams
   Public Property DataSource As String
   Public Property Fields As String
   Public Property Filter As String
   Public Property Sort As String

End Class

Public Class RecaptchaBody
   Public Property Response As String

End Class

Public Class RecaptchaResponse
   Public Property Success As Boolean
   Public Property HostName As String

End Class

Public Class AmountTextQueryParams
   Public Property Amount As Decimal

End Class
