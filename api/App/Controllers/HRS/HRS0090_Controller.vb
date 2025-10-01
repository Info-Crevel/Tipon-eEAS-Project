<RoutePrefix("api")>
Public Class HRS0090_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberRequestCandidateScreenings")>
   <Route("member-request-candidate-screenings/{applicantDetailId}/{memberRequestId}")>
   <HttpGet>
   Public Function GetMemberRequestCandidateScreenings(applicantDetailId As Integer, memberRequestId As Integer) As IHttpActionResult
      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.HRS0090_Screening")
            With _direct
               .AddParameter("applicantDetailId", applicantDetailId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "screening"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   <SymAuthorization("GetMemberRequestInquiry")>
   <Route("member-request-monitoring/inquiry")>
   <HttpGet>
   Public Function GetMemberRequestInquiry(<FromUri> q As MemberRequestInquiryQuery) As IHttpActionResult

      Dim _memberRequestName As String = q.MemberRequestName

      If String.IsNullOrEmpty(q.MemberRequestName) Or q.MemberRequestName = "undefined" Then
         _memberRequestName = ""
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
         Using _direct As New SqlDirect("web.HRS0090_Inquiry", CommandType.StoredProcedure)
            With _direct
               .AddParameter("@MemberName", q.MemberName)
               .AddParameter("@MemberRequestName", _memberRequestName)

               .AddParameter("@MinIndex", _min)
               .AddParameter("@MaxIndex", _max)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberList"
                     .Tables(1).TableName = "memberReferenceList"
                     .Tables(2).TableName = "memberRequestList"
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

   <SymAuthorization("GetMemberRequestMonitoringCount")>
   <Route("member-request-monitoring/count")>
   <HttpGet>
   Public Function GetMemberRequestMonitoringCount(<FromUri> q As MemberRequestCountQuery) As IHttpActionResult
      Dim _filter As String = ""
      Dim _filterMemberRequest As String = ""

      Dim _filterParts As New List(Of String)

      If Not String.IsNullOrEmpty(q.MemberRequestName) AndAlso q.MemberRequestName <> "undefined" Then
         _filterParts.Add("MemberRequestName LIKE '%" & q.MemberRequestName & "%'")
      End If

      If Not String.IsNullOrEmpty(q.MemberName) AndAlso q.MemberName <> "undefined" Then
         _filterParts.Add("MemberName LIKE '%" & q.MemberName & "%'")
      End If

      If _filterParts.Count > 0 Then
         _filter = " WHERE " & String.Join(" AND ", _filterParts)
      Else
         _filter = ""
      End If
      Dim _sql As String = "SELECT COUNT(*) FROM QArsMemberRequestMember " + _filter
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

Public Class MemberRequestCountQuery
   Public Property MemberRequestName As String
   Public Property MemberName As String

End Class

Public Class MemberRequestInquiryQuery

   Public Property MemberRequestName As String
   Public Property MemberName As String
   Public Property IPP As Integer      ' items per page
   Public Property Page As Integer

End Class