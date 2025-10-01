Public Class DbsLicenseProfession
    Public Property LicenseProfessionId As Integer
    Public Property LicenseProfessionName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsLicenseProfession
    Inherits DataSource(Of DbsLicenseProfession)

End Class