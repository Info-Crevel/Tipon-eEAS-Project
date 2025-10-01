Public Class DbsRevenueQualification
   Public Property RevenueQualificationId As Integer
   Public Property RevenueQualificationName As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QDbsRevenueQualification
   Inherits DataSource(Of DbsRevenueQualification)

End Class