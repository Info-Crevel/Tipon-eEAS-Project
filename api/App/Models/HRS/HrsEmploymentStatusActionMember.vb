Public Class HrsEmploymentStatusActionMember
   Public Property EmploymentStatusMemberId As Integer
   Public Property EmploymentStatusActionId As Integer
   'Public Property EmploymentStatusId As Integer
   Public Property MemberId As Integer
   Public Property StatusActionId As Integer
   Public Property LockId As String

End Class

Public Class QHrsEmploymentStatusActionMember
   Inherits DataSource(Of HrsEmploymentStatusActionMember)

End Class

