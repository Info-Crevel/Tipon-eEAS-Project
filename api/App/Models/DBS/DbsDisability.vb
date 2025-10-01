Public Class DbsDisability
    Public Property DisabilityId As Integer
    Public Property DisabilityName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsDisability
    Inherits DataSource(Of DbsDisability)
End Class