Public Class DbsRnrNumber
    Public Property RnrNumberId As Integer
    Public Property RnrNumber As Decimal
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsRnrNumber
    Inherits DataSource(Of DbsRnrNumber)

End Class