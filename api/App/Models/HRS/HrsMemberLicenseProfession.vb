Public Class HrsMemberLicenseProfession
    Public Property LicenseProfessionDetailId As Integer
    Public Property MemberId As Integer
    Public Property LicenseProfessionId As Integer
    Public Property PRCIdNo As String
    Public Property LicenseFileName As String
    Public Property LicenseGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberLicenseProfessionList
    Inherits List(Of HrsMemberLicenseProfession)

End Class
