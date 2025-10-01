Public Class DbsInHouseBenefit
    Public Property InHouseBenefitId As Integer
    Public Property InHouseBenefitName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsInHouseBenefit
    Inherits DataSource(Of DbsInHouseBenefit)

End Class