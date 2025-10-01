<RoutePrefix("api")>
Public Class ARS5000_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetSourcingHiringDashBoard")>
   <Route("sourcing-hiring-dashboard/{orgId}")>
   <HttpGet>
   Public Function GetReligion(orgId As Integer) As IHttpActionResult

      If orgId <= 0 Then
         Throw New ArgumentException("Org ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS5000")
            With _direct
               .AddParameter("OrgId", orgId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function
    <SymAuthorization("GetReferences_ARS5000")>
    <Route("references/ars5000")>
    <HttpGet>
    Public Function GetReferences_AR5000() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.ARS5000_References")
                With _direct
                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "clientPayGroup"     ' all defined Transaction Types
                            .Tables(1).TableName = "memberRequest"     ' all defined Transaction Types

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
