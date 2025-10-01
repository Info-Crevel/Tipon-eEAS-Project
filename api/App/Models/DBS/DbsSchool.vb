Public Class DbsSchool
    Public Property SchoolId As Integer
    Public Property SchoolName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsSchool
    Inherits DataSource(Of DbsSchool)
End Class