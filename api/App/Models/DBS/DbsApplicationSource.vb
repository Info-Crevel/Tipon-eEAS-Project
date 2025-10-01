Public Class DbsApplicationSource
    Public Property ApplicationSourceId As Integer
    Public Property ApplicationSourceName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsApplicationSource
    Inherits DataSource(Of DbsApplicationSource)

End Class