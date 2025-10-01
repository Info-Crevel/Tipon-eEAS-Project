Imports System.Web.Http
'Imports System.Web.Optimization

Public Class WebApiApplication
   Inherits System.Web.HttpApplication

   Sub Application_Start()
      'AreaRegistration.RegisterAllAreas()
      GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
      'FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
      'RouteConfig.RegisterRoutes(RouteTable.Routes)
      'BundleConfig.RegisterBundles(BundleTable.Bundles)

      ImageMagick.MagickAnyCPU.CacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin\cache")

      MagickNET.SetGhostscriptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin"))

      Sys.Session.Open()
      'Sys.FrsSession.Open()
      EasSession.Open()

   End Sub

   Protected Sub Application_BeginRequest()
      ' Rewrite URLs - EMT
      ' https://msdn.microsoft.com/en-US/library/sa5wkk6d.aspx?cs-save-lang=1&cs-lang=vb#code-snippet-2
      '
      Dim _path As String = HttpContext.Current.Request.Path.ToLower()

      If Not _path.StartsWith("/api") AndAlso _path <> "/" Then
         Context.RewritePath("/")
      End If

   End Sub

End Class
