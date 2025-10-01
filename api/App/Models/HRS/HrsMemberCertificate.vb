Public Class HrsMemberCertificate
    Public Property CertificateDetailId As Integer
    Public Property MemberId As Integer
    Public Property CertificateName As String
    Public Property Rating As String
    Public Property IssuedBy As String
    Public Property IssuedDate As Date
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberCertificateList
    Inherits List(Of HrsMemberCertificate)

End Class
