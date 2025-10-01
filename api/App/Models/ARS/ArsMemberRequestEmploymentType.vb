Public Class ArsMemberRequestEmploymentType
    Public Property MemberRequestEmploymentTypeDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property EmploymentTypeId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestEmploymentTypeList
    Inherits List(Of ArsMemberRequestEmploymentType)

End Class

