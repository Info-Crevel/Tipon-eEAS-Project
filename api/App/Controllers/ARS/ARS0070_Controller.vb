<RoutePrefix("api")>
Public Class ARS0070_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberClientPayGroupInquiry")>
   <Route("member-client-pay-group-monitoring/inquiry")>
   <HttpGet>
   Public Function GetMemberClientPayGroupInquiry(<FromUri> q As MemberPayGroupInquiryQuery) As IHttpActionResult

      Dim _clientPayGroupName As String = q.ClientPayGroupName

      If String.IsNullOrEmpty(q.ClientPayGroupName) Or q.ClientPayGroupName = "undefined" Then
         _clientPayGroupName = ""
      End If

      Dim _table As String = ""

      Dim _ipp As Integer = 15
      Dim _page As Integer = 1

      With q
         If .IPP > 0 Then
            _ipp = .IPP
         End If

         If .Page > 0 Then
            _page = .Page
         End If
      End With

      Dim _min As Integer = ((_page - 1) * _ipp) + 1
      Dim _max As Integer = _page * _ipp

      Try
         Using _direct As New SqlDirect("web.ARS0070_Inquiry", CommandType.StoredProcedure)
            With _direct
               .AddParameter("@MemberName", q.MemberName)
               .AddParameter("@ClientPayGroupName", _clientPayGroupName)

               .AddParameter("@MinIndex", _min)
               .AddParameter("@MaxIndex", _max)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberList"
                     .Tables(1).TableName = "memberReferenceList"
                     .Tables(2).TableName = "payGroupList"
                     .Tables(3).TableName = "report"
                     .Tables(4).TableName = "rowPerPage"

                  End With
                  Return Me.Ok(_dataSet)
               End Using
            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberClientPayGroupMonitoringCount")>
   <Route("member-client-pay-group-monitoring/count")>
   <HttpGet>
   Public Function GetMemberClientPayGroupMonitoringCount(<FromUri> q As MemberPayGroupCountQuery) As IHttpActionResult
      Dim _filter As String = ""
      Dim _filterPayGroup As String = ""

      Dim _filterParts As New List(Of String)

      If Not String.IsNullOrEmpty(q.ClientPayGroupName) AndAlso q.ClientPayGroupName <> "undefined" Then
         _filterParts.Add("ClientPayGroupName LIKE '%" & q.ClientPayGroupName & "%'")
      End If

      If Not String.IsNullOrEmpty(q.MemberName) AndAlso q.MemberName <> "undefined" Then
         _filterParts.Add("MemberName LIKE '%" & q.MemberName & "%'")
      End If

      If _filterParts.Count > 0 Then
         _filter = " WHERE " & String.Join(" AND ", _filterParts)
      Else
         _filter = ""
      End If
      Dim _sql As String = "SELECT COUNT(*) FROM QArsClientPayGroupMember " + _filter
         Dim _count As Integer

         Try

            Using _direct As New SqlDirect(_sql, CommandType.Text)
               With _direct

                  _count = CInt(.ExecuteScalar())
                  Return Me.Ok(_count)

               End With
            End Using

         Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
         End Try

   End Function

End Class

Public Class MemberPayGroupCountQuery
   Public Property ClientPayGroupName As String
   Public Property MemberName As String

End Class

Public Class MemberPayGroupInquiryQuery

   Public Property ClientPayGroupName As String
   Public Property MemberName As String
   Public Property IPP As Integer      ' items per page
   Public Property Page As Integer

End Class