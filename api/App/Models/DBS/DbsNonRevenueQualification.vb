Public Class DbsNonRevenueQualification
   Public Property NonRevenueQualificationId As Integer
   Public Property NonRevenueQualificationName As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QDbsNonRevenueQualification
   Inherits DataSource(Of DbsNonRevenueQualification)

End Class