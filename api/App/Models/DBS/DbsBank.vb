Public Class DbsBank
    Public Property BankId As Integer
    Public Property BankName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsBank
    Inherits DataSource(Of DbsBank)
End Class