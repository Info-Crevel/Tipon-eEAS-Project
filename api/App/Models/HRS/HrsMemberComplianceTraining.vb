Public Class HrsMemberComplianceTraining
    Public Property ComplianceTrainingDetailId As Integer
    Public Property MemberId As Integer
    Public Property CertificateNumber As String
    Public Property ComplianceTrainingId As Integer
    Public Property IssuanceDate As Date
    Public Property ValidityDate As Date
    Public Property TrainingInstitutionId As Integer
    Public Property AssessmentCenterId As Integer
    Public Property ComplianceFileName As String
    Public Property ComplianceGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberComplianceTrainingList
    Inherits List(Of HrsMemberComplianceTraining)

End Class

