Public Class ArsMemberRequestPosition
    Public Property MemberRequestPositionId As Integer
    Public Property MemberRequestPositionName As String
    Public Property MemberRequestPositionDescription As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class
Public Class QArsMemberRequestPosition
    Inherits DataSource(Of ArsMemberRequestPosition)
End Class