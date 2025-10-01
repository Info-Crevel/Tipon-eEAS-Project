<RoutePrefix("api")>
Public Class GLS1000_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_GLS1000")>
   <Route("references/gls1000")>
   <HttpGet>
   Public Function GetReferences_GLS1000() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.GLS1000_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "types"         ' Asset, Liability, Equity, Reveneue, Expense
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFinAccounts")>
   <Route("fin-accounts-structure")>
   <HttpGet>
   Public Function GetFinAccounts(<FromUri> q As GLS1000_Query) As IHttpActionResult

      Dim _accountTypeId As Integer = q.AccountTypeId

      Try
         Using _direct As New SqlDirect("web.GLS1000")
            With _direct
               .AddParameter("AccountTypeId", _accountTypeId.ToNullable)

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

Public Class GLS1000_Query
   Public Property AccountTypeId As Integer

End Class
