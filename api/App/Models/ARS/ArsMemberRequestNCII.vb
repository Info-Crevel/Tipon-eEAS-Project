Public Class ArsMemberRequestNCII
    Public Property NCIIDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property NCIIQualificationTitleId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestNCIIList
    Inherits List(Of ArsMemberRequestNCII)

End Class

