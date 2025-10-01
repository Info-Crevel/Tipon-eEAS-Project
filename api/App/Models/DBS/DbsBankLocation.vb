Public Class DbsBankLocation
    Public Property BankLocationId As Integer
    Public Property BankLocationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsBankLocation
    Inherits DataSource(Of DbsBankLocation)
End Class