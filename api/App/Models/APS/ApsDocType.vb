Public Class ApsDocType
   Public Property ApsDocTypeId As Integer
   Public Property DocTypeName As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsDocType
   Inherits DataSource(Of ApsDocType)
End Class