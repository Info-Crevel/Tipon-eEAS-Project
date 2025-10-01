Public Class DbsCourse
    Public Property CourseId As Integer
    Public Property CourseName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsCourse
    Inherits DataSource(Of DbsCourse)
End Class