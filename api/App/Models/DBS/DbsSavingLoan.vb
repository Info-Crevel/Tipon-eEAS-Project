Public Class DbsSavingLoan
    Public Property SavingLoanId As Integer
    Public Property SavingLoanName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsSavingLoan
    Inherits DataSource(Of DbsSavingLoan)
End Class