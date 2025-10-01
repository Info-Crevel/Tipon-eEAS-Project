Public Class DbsRevenueClarification
    Public Property RevenueClarificationId As Integer
    Public Property RevenueClarificationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsRevenueClarification
    Inherits DataSource(Of DbsRevenueClarification)

End Class