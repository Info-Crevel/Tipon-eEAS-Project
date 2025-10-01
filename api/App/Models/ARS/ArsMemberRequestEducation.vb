Public Class ArsMemberRequestEducation
    Public Property MemberRequestEducationDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property EducationLevelId As Integer
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsMemberRequestEducationList
    Inherits List(Of ArsMemberRequestEducation)

End Class

