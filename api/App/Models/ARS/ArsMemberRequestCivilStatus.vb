Public Class ArsMemberRequestCivilStatus
    Public Property MemberRequestCivilStatusDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property CivilStatusId As Integer
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsMemberRequestCivilStatusList
    Inherits List(Of ArsMemberRequestCivilStatus)

End Class

