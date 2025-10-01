Public Class DbsAllowance
    Public Property AllowanceId As Integer
    Public Property AllowanceName As String
   Public Property PayTrxCode As String

    Public Property TaxFlag As Boolean
    Public Property PayHourly As Boolean
    Public Property PayMaxAmount As Integer

    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsAllowance
    Inherits DataSource(Of DbsAllowance)
End Class