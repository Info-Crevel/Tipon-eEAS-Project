Public Class HrsEmploymentStatusMember
   Public Property EmploymentStatusActionId As Integer
   Public Property EmploymentStatusMemberId As Integer
   Public Property MemberId As Integer
    Public Property EmploymentStatusId As Integer
    'Public Property CurrentEmploymentStatusName As String
    Public Property EffectivityDate As Date
    Public Property EndDate As Date
    Public Property StatusActionId As Integer
    Public Property WorkFormId As Integer

    Public Property EmploymentFileName As String
    Public Property EmploymentGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsEmploymentStatusMemberList
   Inherits List(Of HrsEmploymentStatusMember)

End Class
