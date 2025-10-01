Public Class ArsMemberRequestLicenseProfession
    Public Property LicenseProfessionDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property LicenseProfessionId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestLicenseProfessionList
    Inherits List(Of ArsMemberRequestLicenseProfession)

End Class

