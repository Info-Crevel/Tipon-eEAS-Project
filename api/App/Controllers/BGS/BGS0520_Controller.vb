<RoutePrefix("api")>
Public Class BGS0520_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCurrentSummary")>
   <Route("budget-summary-current/{yearId}")>
   <HttpGet>
   Public Function GetCurrentSummary(yearId As Integer) As IHttpActionResult

      If yearId <= 0 Then
         Throw New ArgumentException("Year is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0520")
            With _direct
               .AddParameter("YearId", yearId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "stats"
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
