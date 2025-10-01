<RoutePrefix("api")>
Public Class BGS0550_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCurrentDetailSAA")>
   <Route("budget-detail-current-saa")>
   <HttpGet>
   Public Function GetCurrentDetailSAA(<FromUri> q As BGS0550_Query) As IHttpActionResult

      Dim _yearId As Integer = q.YearId
      Dim _allotmentId As Integer = q.AllotmentId

      Try
         Using _direct As New SqlDirect("web.BGS0550")
            With _direct
               .AddParameter("YearId", _yearId)
               .AddParameter("AllotmentId", _allotmentId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "stats"
                     .Tables(1).TableName = "allotment"
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

Public Class BGS0550_Query
   Public Property YearId As Integer
   Public Property AllotmentId As Integer

End Class
