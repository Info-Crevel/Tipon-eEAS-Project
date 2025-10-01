Public Class DbsExpenseCharging
    Public Property ExpenseChargingId As Integer
    Public Property ExpenseChargingName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsExpenseCharging
    Inherits DataSource(Of DbsExpenseCharging)
End Class