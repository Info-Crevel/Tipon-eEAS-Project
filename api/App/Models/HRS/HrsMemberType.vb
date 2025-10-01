Public Class HrsMemberType
    Public Property MemberTypeId As Integer
    Public Property MemberTypeName As String
    'Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QHrsMemberType
    Inherits DataSource(Of HrsMemberType)

End Class

