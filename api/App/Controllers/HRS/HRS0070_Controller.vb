<RoutePrefix("api")>
Public Class HRS0070_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberListFilter")>
   <Route("member-engagements")>
   <HttpPost>
   Public Function GetMemberListFilter(<FromBody> filterParam As FilterParam) As IHttpActionResult

      Dim _filter As String = filterParam.filter

      Dim _dataSource As String = "dbo.QHrsMemberEngagement"
        Dim _fields As String = "MemberId, MemberEmployeeId, MemberName,EmploymentStatusId, EmploymentStatusName, HiredDate, DeploymentDate, PlatformName, EndDate, MemberRequestName, MemberRequestId, MemberTypeId, MemberTypeName"
        Dim _sort As String = "MemberName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberEngagements")>
   <Route("member-engagements")>
   <HttpGet>
   Public Function GetMemberEngagements() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.HRS0070_All")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberList"
                     .Tables(1).TableName = "platform"
                     .Tables(2).TableName = "memberRequest"
                            .Tables(3).TableName = "employmentStatus"
                            .Tables(4).TableName = "memberType"

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

