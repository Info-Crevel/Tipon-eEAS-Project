Public Class ArsClientType
    Public Property ClientTypeId As Integer
    Public Property ClientTypeName As String
    Public Property DueDays As Integer
    Public Property AccountId As Integer
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class
Public Class QArsClientType
    Inherits DataSource(Of ArsClientType)

End Class