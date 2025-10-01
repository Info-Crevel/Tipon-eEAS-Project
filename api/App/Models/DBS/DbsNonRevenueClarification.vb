Public Class DbsNonRevenueClarification
    Public Property NonRevenueClarificationId As Integer
    Public Property NonRevenueClarificationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsNonRevenueClarification
    Inherits DataSource(Of DbsNonRevenueClarification)

End Class