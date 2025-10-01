Public Class HrsMemberNCIIQualificationTitle
    Public Property NCIIQualificationTitleDetailId As Integer
    Public Property MemberId As Integer
    Public Property CertificateNumber As String
    Public Property NCIIQualificationTitleId As Integer
    Public Property IssuanceDate As Date
    Public Property ValidityDate As Date
    Public Property TrainingInstitutionId As Integer
    Public Property AssessmentCenterId As Integer
    Public Property NCIIFileName As String
    Public Property NCIIGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberNCIIQualificationTitleList
    Inherits List(Of HrsMemberNCIIQualificationTitle)

End Class

