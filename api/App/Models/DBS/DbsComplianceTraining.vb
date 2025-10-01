Public Class DbsComplianceTraining
    Public Property ComplianceTrainingId As Integer
    Public Property ComplianceTrainingName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsComplianceTraining
    Inherits DataSource(Of DbsComplianceTraining)

End Class