Public Class HrsMemberLicense
    Public Property LicenseDetailId As Integer
    Public Property MemberId As Integer
    Public Property LicenseTitle As String
    Public Property ExpiryDate As Date
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberLicenseList
    Inherits List(Of HrsMemberLicense)

End Class

