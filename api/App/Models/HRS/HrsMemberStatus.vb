Public Class HrsMemberStatus
    Public Property MemberStatusId As Integer
    Public Property MemberStatusName As String
    'Public Property SortSeq As Integer
    'Public Property LockId As String

End Class

Public Class QHrsMemberStatus
    Inherits DataSource(Of HrsMemberStatus)

End Class

