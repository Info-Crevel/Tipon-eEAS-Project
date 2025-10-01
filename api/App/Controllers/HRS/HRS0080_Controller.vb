<RoutePrefix("api")>
Public Class HRS0080_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberIdMonitoringInquiry")>
   <Route("member-id-monitoring/inquiry")>
   <HttpGet>
   Public Function GetMemberIdMonitoringInquiry(<FromUri> q As MemberIdInquiryQuery) As IHttpActionResult

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
         Using _direct As New SqlDirect("web.HRS0080_Inquiry", CommandType.StoredProcedure)
            With _direct
               .AddParameter("@MemberName", q.MemberName)
               .AddParameter("@SssFlag", q.SssFlag)
               .AddParameter("@PgbFlag", q.PgbFlag)
               .AddParameter("@PhhFlag", q.PhhFlag)
               .AddParameter("@WhtFlag", q.WhtFlag)

               .AddParameter("@MinIndex", _min)
               .AddParameter("@MaxIndex", _max)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberList"
                     .Tables(1).TableName = "memberReferenceList"
                     .Tables(2).TableName = "rowPerPage"
                  End With
                  Return Me.Ok(_dataSet)
               End Using
            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberIdMonitoringCount")>
   <Route("member-id-monitoring/count")>
   <HttpGet>
   Public Function GetMemberIdMonitoringCount(<FromUri> q As MemberIdCountQuery) As IHttpActionResult
      Dim _filter As String = ""

      If String.IsNullOrEmpty(q.MemberName) Or q.MemberName = "undefined" Then
         _filter = ""
      Else
         _filter = "MemberName LIKE '%" & q.MemberName & "%'"
      End If

      Dim _sssflag As String = "SssFlag=" & Convert.ToInt32(q.SssFlag).ToString
      Dim _pgbFlag As String = "PgbFlag=" & Convert.ToInt32(q.PgbFlag).ToString
      Dim _phhFlag As String = "PhhFlag=" & Convert.ToInt32(q.PhhFlag).ToString
      Dim _whtFlag As String = "WhtFlag=" & Convert.ToInt32(q.WhtFlag).ToString

      If Convert.ToInt32(q.SssFlag) = 0 Then
         _sssflag = "SssFlag in (0,1)"
      End If

      If Convert.ToInt32(q.PgbFlag) = 0 Then
         _pgbFlag = "PgbFlag in (0,1)"
      End If

      If Convert.ToInt32(q.PhhFlag) = 0 Then
         _phhFlag = "PhhFlag in (0,1)"
      End If

      If Convert.ToInt32(q.WhtFlag) = 0 Then
         _whtFlag = "WhtFlag in (0,1)"
      End If

      Dim _flag As String = _sssflag + " AND " + _pgbFlag + " AND " + _phhFlag + " AND " + _whtFlag

      If _filter <> "" Then
         _filter = " where " + _filter + " AND " + _flag
      Else
         _filter = " where " + _flag
      End If

      Dim _sql As String = "SELECT COUNT(*) FROM QHrsMemberGovernmentIdentification " + _filter
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

   <SymAuthorization("GetMemberIdMonitoring")>
   <Route("member-id-monitoring")>
   <HttpGet>
   Public Function GetMemberIdMonitoring() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.HRS0080_All")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberList"
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

Public Class MemberIdCountQuery
   Public Property MemberId As Integer
   Public Property MemberName As String
   Public Property SssFlag As Boolean
   Public Property PgbFlag As Boolean
   Public Property PhhFlag As Boolean
   Public Property WhtFlag As Boolean


End Class

Public Class MemberIdInquiryQuery

   Public Property MemberId As Integer
   Public Property MemberName As String
   Public Property SssFlag As Boolean
   Public Property PgbFlag As Boolean
   Public Property PhhFlag As Boolean
   Public Property WhtFlag As Boolean
   Public Property IPP As Integer      ' items per page
   Public Property Page As Integer

End Class