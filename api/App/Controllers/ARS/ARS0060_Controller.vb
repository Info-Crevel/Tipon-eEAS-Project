<RoutePrefix("api")>
Public Class ARS0060_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <Route("member-request/count")>
   <HttpGet>
   Public Function GetMemberRequestCount(<FromUri> query As MemberRequestSearchQuery) As IHttpActionResult



      Try
         Dim _memberRequestPositionName As String = ""

         With query
            _memberRequestPositionName = .MemberRequestPositionName
         End With

         Using _direct As New SqlDirect("web.GetStsMemberChitMatchCount")
            With _direct
               ' Adding the input parameter
               .AddParameter("MemberRequestPositionName", _memberRequestPositionName)

               ' Adding the output parameter (MatchCount) before executing the procedure
               'Dim _p = .AddParameter("MatchCount", DbType.Int32, ParameterDirection.Output)

               ' Execute the stored procedure
               .ExecuteScalar()

               ' Retrieve and return the output parameter value as an integer
               Return Me.Ok(CInt(10))
            End With
         End Using

         'Dim _p As DbParameter

         'Using _direct As New SqlDirect("web.GetStsMemberChitMatchCount")
         '   With _direct
         '      .AddParameter("MemberRequestPositionName", _memberRequestPositionName)
         '      _p = .AddParameter("MatchCount", DbType.Int32, ParameterDirection.Output)

         '      .ExecuteScalar()
         '   End With

         '   Return Me.Ok(CInt(_p.Value))
         'End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try


      'Dim _filter As String = query.FilterParam

      'Dim _dataSource As String = "dbo.QHrsMemberRequestEngagement"
      'Dim _fields As String = "MemberRequestId, MemberRequestName, PlatformId,PlatformName,ClientContractId,ClientContractName,MemberRequestPositionName,VacancyCount,DeploymentDate,MemberRequestStatusId,MemberRequestStatusName,WorkingDays,TotalHired,TotalVacancy,EndDate,EmploymentTypeId,EmploymentTypeName"
      'Dim _sort As String = "MemberRequestId"

      'Try
      '   Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
      '      Dim rowCount As Integer = _table.Rows.Count
      '      Return Me.Ok(rowCount) ' count the row in _table
      '   End Using

      'Catch _exception As Exception
      '   Return Me.BadRequest(_exception.Message)
      'End Try
   End Function


   <Route("member-request/search")>
   <HttpGet>
   Public Function SearchMemberChit(<FromUri> query As MemberRequestSearchQuery) As IHttpActionResult

      If query Is Nothing Then
         Return Me.BadRequest("Query string is required.")
      End If

      'Try
      '   Dim _builder As New StringBuilder

      '   Dim _filterParam As String = ""

      '   Dim _ipp As Integer = 20
      '   Dim _page As Integer = 1

      '   With query
      '      If Not String.IsNullOrEmpty(.FilterParam) Then
      '         _filterParam = .FilterParam.Replace("'", "''")
      '      End If

      '      If .IPP > 0 Then
      '         _ipp = .IPP
      '      End If

      '      If .Page > 0 Then
      '         _page = .Page
      '      End If

      '   End With

      '   Dim _min As Integer = ((_page - 1) * _ipp) + 1
      '   Dim _max As Integer = _page * _ipp

      '   '_builder.Append("SELECT OutletId, OutletName,TrxId,CheckNumber,TrxStatusName,StatusUserName,OpenDate,Amount,AccountId,SoldTo FROM web.StsMemberChitSearch(" + _memberRequestPositionName.Enclose(EncloseCharacter.Quote) + ")")
      '   _builder.Append("SELECT MemberRequestId, MemberRequestName, PlatformId,PlatformName,ClientContractId,ClientContractName,MemberRequestPositionName,VacancyCount,DeploymentDate,MemberRequestStatusId,MemberRequestStatusName,WorkingDays,TotalHired,TotalVacancy,EndDate,EmploymentTypeId,EmploymentTypeName FROM dbo.QHrsMemberRequestEngagement where " + _filterParam + " AND RowIndex BETWEEN " + _min.ToString + " AND " + _max.ToString)

      '   '_builder.Append(" WHERE RowIndex BETWEEN " + _min.ToString + " AND " + _max.ToString)
      '   _builder.Append(" ORDER BY MemberRequestId")

      '   Using _direct As New SqlDirect(_builder.ToString, CommandType.Text)
      '      Using _table As DataTable = _direct.ExecuteDataTable()
      Return Me.Ok(1)
      '      End Using
      '   End Using

      'Catch _exception As Exception
      '   Return Me.BadRequest(_exception.Message)
      'End Try

   End Function

   <SymAuthorization("GetMemberRequestFormListFilter")>
   <Route("member-request-forms")>
   <HttpPost>
   Public Function GetMemberRequestFormListFilter(<FromBody> filterParam As FilterParam) As IHttpActionResult

      Dim _filter As String = filterParam.filter

      Dim _dataSource As String = "dbo.QHrsMemberRequestEngagement"
        Dim _fields As String = "MemberRequestId, MemberRequestName, OrgId, OrgName, PlatformId,PlatformName,ClientPayGroupId,ClientPayGroupName, ClientContractId,ClientContractName,MemberRequestPositionName,VacancyCount,DeploymentDate,MemberRequestStatusId,MemberRequestStatusName,WorkingDays,TotalHired,TotalVacancy,EndDate,EmploymentTypeId,EmploymentTypeName"
        Dim _sort As String = "MemberRequestId"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberRequestForms")>
   <Route("member-request-forms")>
   <HttpGet>
   Public Function GetMemberRequestForms() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0060_All")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "requestList"
                            .Tables(1).TableName = "platforms"
                            .Tables(2).TableName = "contracts"
                            .Tables(3).TableName = "requestStatus"
                            .Tables(4).TableName = "employmentTypes"
                            .Tables(5).TableName = "orgs"
                            .Tables(6).TableName = "payGroups"

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
Public Class MemberRequestSearchQuery
   'Public Property FilterParam As String
   Public Property MemberRequestPositionName As String
   Public Property EmploymentStatusId As Integer
   Public Property MemberRequestStatusId As Integer
    Public Property ClientContractId As Integer
    Public Property OrgId As Integer
    Public Property PlatformId As Integer
   Public Property DeploymentDate As Date
   Public Property IPP As Integer      ' items per page
   Public Property Page As Integer

End Class

