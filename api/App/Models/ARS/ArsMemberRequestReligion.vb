Public Class ArsMemberRequestReligion
    Public Property MemberRequestReligionDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property ReligionId As Integer
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsMemberRequestReligionList
    Inherits List(Of ArsMemberRequestReligion)

End Class

