Public Class DbsBillingComputation
    Public Property BillingComputationId As Integer
    Public Property BillingComputationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsBillingComputation
    Inherits DataSource(Of DbsBillingComputation)

End Class