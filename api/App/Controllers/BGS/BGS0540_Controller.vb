<RoutePrefix("api")>
Public Class BGS0540_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCurrentDetail")>
   <Route("budget-detail-current")>
   <HttpGet>
   Public Function GetCurrentDetail(<FromUri> q As BGS0540_Query) As IHttpActionResult

      Dim _yearId As Integer = q.YearId
      Dim _activityId As Integer = q.ActivityId

      Try
         Using _direct As New SqlDirect("web.BGS0540")
            With _direct
               .AddParameter("YearId", _yearId)
               .AddParameter("ActivityId", _activityId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "stats"
                     .Tables(1).TableName = "activity"
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

Public Class BGS0540_Query
   Public Property YearId As Integer
   Public Property ActivityId As Integer

End Class
