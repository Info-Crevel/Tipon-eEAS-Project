Public Class ApsApprover
   Public Property ApproverId As Integer
   Public Property MemberEmployeeId As String
   Public Property UserId As Integer
   'Public Property RecruiterName As String
   'Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsApprover
   Inherits DataSource(Of ApsApprover)

End Class

