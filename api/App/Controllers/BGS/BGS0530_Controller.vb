<RoutePrefix("api")>
Public Class BGS0530_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetContinuingSummary")>
   <Route("budget-summary-continuing/{yearId}")>
   <HttpGet>
   Public Function GetContinuingSummary(yearId As Integer) As IHttpActionResult

      If yearId <= 0 Then
         Throw New ArgumentException("Year is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0530")
            With _direct
               .AddParameter("YearId", yearId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "gaa"
                     .Tables(1).TableName = "saa"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

End Class
