Public Class DbsEngagementType
    Public Property EngagementTypeId As Integer
    Public Property EngagementTypeCode As String
    Public Property EngagementTypeName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsEngagementType
    Inherits DataSource(Of DbsEngagementType)
End Class