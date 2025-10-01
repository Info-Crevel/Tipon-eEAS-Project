Public Class HrsMemberEducation
   Public Property EduDetailId As Integer
    Public Property MemberId As Integer
    Public Property EducationLevelId As Integer
    Public Property SchoolName As String
    Public Property CourseName As String

    Public Property EduStartYear As Integer
    Public Property EduEndYear As Integer
    Public Property EduFileName As String
    Public Property EduGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberEducationList
   Inherits List(Of HrsMemberEducation)

End Class

