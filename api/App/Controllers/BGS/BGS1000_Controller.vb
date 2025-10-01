<RoutePrefix("api")>
Public Class BGS1000_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetBgsActivities")>
   <Route("activities-structure")>
   <HttpGet>
   Public Function GetBgsActivities() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.BGS1000")
            With _direct
               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

End Class
