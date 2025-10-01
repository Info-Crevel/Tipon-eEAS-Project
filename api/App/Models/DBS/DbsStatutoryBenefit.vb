Public Class DbsStatutoryBenefit
    Public Property StatutoryBenefitId As Integer
    Public Property StatutoryBenefitName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsStatutoryBenefit
    Inherits DataSource(Of DbsStatutoryBenefit)

End Class