Public Class DbsAssessmentCenter
    Public Property AssessmentCenterId As Integer
    Public Property AssessmentCenterName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsAssessmentCenter
    Inherits DataSource(Of DbsAssessmentCenter)

End Class