Public Class ArsMemberRequestTypeQualification
    Public Property MemberRequestTypeQualificationDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property TypeQualificationDetailId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestTypeQualificationList
    Inherits List(Of ArsMemberRequestTypeQualification)

End Class

