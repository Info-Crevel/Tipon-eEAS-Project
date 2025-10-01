Public Class ArsMemberRequestSex
    Public Property MemberRequestSexDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property SexId As String
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsMemberRequestSexList
    Inherits List(Of ArsMemberRequestSex)

End Class

