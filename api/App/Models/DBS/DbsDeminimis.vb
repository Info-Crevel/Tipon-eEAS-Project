Public Class DbsDeminimis
    Public Property DeminimisId As Integer
    Public Property DeminimisName As String
    Public Property PayTrxCode As String
    Public Property TaxFlag As Boolean
    Public Property PayHourly As Boolean
    Public Property PayMaxAmount As Integer
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsDeminimis
    Inherits DataSource(Of DbsDeminimis)
End Class