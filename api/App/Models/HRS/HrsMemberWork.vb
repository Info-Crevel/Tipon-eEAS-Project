Public Class HrsMemberWork
    Public Property WorkDetailId As Integer
    Public Property MemberId As Integer
    Public Property WorkPosition As String
    Public Property CompanyName As String
    Public Property MunicipalityId As String
    Public Property CompanyPhoneNumber As String
    Public Property WorkStartDate As DateTime
    Public Property WorkEndDate As DateTime
    Public Property ReasonForLeaving As String
    Public Property WorkFileName As String
    Public Property WorkGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberWorkList
    Inherits List(Of HrsMemberWork)

End Class

