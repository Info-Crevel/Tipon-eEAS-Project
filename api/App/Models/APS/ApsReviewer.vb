Public Class ApsReviewer
   Public Property ReviewerId As Integer
   Public Property MemberEmployeeId As String
   Public Property UserId As Integer
   'Public Property RecruiterName As String
   'Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsReviewer
   Inherits DataSource(Of ApsReviewer)

End Class

