Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Http

Public Module WebApiConfig
   Public Sub Register(config As HttpConfiguration)
      ' Web API configuration and services

      ' Camel-casing of JSON 
      With config.Formatters.JsonFormatter.SerializerSettings
         .Formatting = Newtonsoft.Json.Formatting.Indented
         .ContractResolver = New CamelCasePropertyNamesContractResolver
         .DateFormatString = "yyyy-MM-dd"
      End With

      ' Allow viewing of JSON in browser
      config.Formatters.JsonFormatter.SupportedMediaTypes.Add(New Net.Http.Headers.MediaTypeHeaderValue("text/html"))

      ' Web API routes
      config.MapHttpAttributeRoutes()

      config.Routes.MapHttpRoute(
          name:="DefaultApi",
          routeTemplate:="api/{controller}/{id}",
          defaults:=New With {.id = RouteParameter.Optional}
      )

      ' Never show exception stack when an unhandled error is encountered
      config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always       ' development mode
      'config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never        ' production mode

   End Sub

End Module
