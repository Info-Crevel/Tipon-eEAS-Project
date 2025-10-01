Public Class DbsPayGroupComputation
    Public Property PayGroupComputationId As Integer
    Public Property PayGroupComputationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsPayGroupComputation
    Inherits DataSource(Of DbsPayGroupComputation)

End Class