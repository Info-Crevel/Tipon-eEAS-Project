Public Class DbsRelation
   Public Property RelationId As Integer
    Public Property RelationName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property OccupationRequiredFlag As Boolean
    Public Property EmailRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsRelation
    Inherits DataSource(Of DbsRelation)
End Class