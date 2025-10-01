'Public NotInheritable Class SysInfo

'   Private _siteId As Integer
'   Private _siteName As String
'   Private _siteShortName As String
'   Private _sessionTimeout As Integer
'   Private _currencyId As String
'   Private _pages As PageInfo()

'   Friend Sub New()
'      MyBase.New()
'   End Sub

'   Public Property SiteId As Integer
'      Get
'         Return _siteId
'      End Get
'      Friend Set(value As Integer)
'         _siteId = value
'      End Set
'   End Property

'   Public Property SiteName As String
'      Get
'         Return _siteName
'      End Get
'      Friend Set(value As String)
'         _siteName = value
'      End Set
'   End Property

'   Public Property SiteShortName As String
'      Get
'         Return _siteShortName
'      End Get
'      Friend Set(value As String)
'         _siteShortName = value
'      End Set
'   End Property

'   Public Property SessionTimeout As Integer
'      Get
'         Return _sessionTimeout
'      End Get
'      Friend Set(value As Integer)
'         _sessionTimeout = value
'      End Set
'   End Property

'   Public Property CurrencyId As String
'      Get
'         Return _currencyId
'      End Get
'      Friend Set(value As String)
'         _currencyId = value
'      End Set
'   End Property

'   Public ReadOnly Property ProductTitle As String
'      Get
'         Return "Symphony"
'      End Get
'   End Property

'   Public ReadOnly Property ProductDescription As String
'      Get
'         Return "ePMS 2.0"
'      End Get
'   End Property

'   Public ReadOnly Property ProductName As String
'      Get
'         Return Me.ProductTitle + " " + Me.ProductDescription
'      End Get
'   End Property

'   Public Property Pages As PageInfo()
'      Get
'         Return _pages
'      End Get
'      Friend Set(value As PageInfo())
'         _pages = value
'      End Set
'   End Property

'End Class

Public NotInheritable Class SysInfo

   'Private _siteId As Integer
   'Private _siteName As String
   'Private _siteShortName As String
   'Private _sessionTimeout As Integer
   'Private _currencyId As String
   'Private _pages As PageInfo()

   Friend Sub New()
      MyBase.New()
   End Sub

   Public Property SiteId As Integer
   Public Property SiteName As String
   Public Property SiteShortName As String
   Public Property SessionTimeout As Integer
   Public Property CurrencyId As String
   Public Property ActiveTokenCount As Integer
   Public Property YearId As Integer

   Public ReadOnly Property ProductTitle As String
      Get
         Return "Symphony"
      End Get
   End Property

   Public ReadOnly Property ProductDescription As String
      Get
         Return "eEAS 1.1"
      End Get
   End Property

   Public ReadOnly Property ProductName As String
      Get
         Return Me.ProductTitle + " " + Me.ProductDescription
      End Get
   End Property

   Public Property Pages As PageInfo()

End Class

'Public Structure PageInfo
'   Public Sub New(pageId As String, pageName As String, pagePath As String, webAppId As String)
'      Me.PageId = pageId
'      Me.PageName = pageName
'      Me.PagePath = pagePath
'      Me.WebAppId = webAppId
'   End Sub

'   Public ReadOnly Property PageId As String
'   Public ReadOnly Property PageName As String
'   Public ReadOnly Property PagePath As String
'   Public ReadOnly Property WebAppId As String

'End Structure

Public Structure PageInfo

   Public Sub New(pageId As String, pageName As String, pagePath As String)
      Me.PageId = pageId
      Me.PageName = pageName
      Me.PagePath = pagePath
   End Sub

   Public ReadOnly Property PageId As String
   Public ReadOnly Property PageName As String
   Public ReadOnly Property PagePath As String

End Structure

Public NotInheritable Class ReferenceDateInfo
   Public Property SystemDate As Date
   Public Property AuditDate As Date
   Public Property ServerDate As Date
   Public Property ServerTime As TimeSpan

End Class

Public NotInheritable Class ServerDateInfo
   Public Property ServerDate As Date
   Public Property ServerTIme As TimeSpan

End Class
