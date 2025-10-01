Public Class HrsRecruiter
   Public Property RecruiterId As Integer
   Public Property MemberEmployeeId As String
   Public Property UserId As Integer
   Public Property RecruiterName As String
   'Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QHrsRecruiter
   Inherits DataSource(Of HrsRecruiter)

End Class

