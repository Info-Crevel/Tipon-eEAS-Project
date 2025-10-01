Public Class HrsMemberTypeQualification
    Public Property TypeQualificationDetailId As Integer
    Public Property TypeQualificationName As String
    Public Property MemberTypeId As Integer
    Public Property SortSeq As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberTypeQualificationList
    Inherits List(Of HrsMemberTypeQualification)

End Class

Public Class QHrsMemberTypeQualification
    Inherits DataSource(Of HrsMemberTypeQualification)

End Class
