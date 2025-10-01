Public Class DbsAffiliation
    Public Property AffiliationId As Integer
    Public Property AffiliationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsAffiliation
    Inherits DataSource(Of DbsAffiliation)

End Class