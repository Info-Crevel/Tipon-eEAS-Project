Public Class DbsEducationLevel
    Public Property EducationLevelId As Integer
    Public Property EducationLevelName As String
    Public Property CourseNameRequiredFlag As Boolean
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsEducationLevel
    Inherits DataSource(Of DbsEducationLevel)

End Class