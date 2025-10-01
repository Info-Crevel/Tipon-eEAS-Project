Public Class DbsRequestType
    Public Property RequestTypeId As Integer
    Public Property RequestTypeName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsRequestType
    Inherits DataSource(Of DbsRequestType)

End Class