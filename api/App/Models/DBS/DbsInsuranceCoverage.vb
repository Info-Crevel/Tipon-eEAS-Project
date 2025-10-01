Public Class DbsInsuranceCoverage
    Public Property InsuranceCoverageId As Integer
    Public Property InsuranceCoverageName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsInsuranceCoverage
    Inherits DataSource(Of DbsInsuranceCoverage)
End Class