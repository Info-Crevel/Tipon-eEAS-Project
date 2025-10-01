Public Class DbsTrainingInstitution
    Public Property TrainingInstitutionId As Integer
    Public Property TrainingInstitutionName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsTrainingInstitution
    Inherits DataSource(Of DbsTrainingInstitution)

End Class