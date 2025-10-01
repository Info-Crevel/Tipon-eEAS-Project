Public Class ApsRequestType
   Public Property ApsRequestTypeId As Integer
   Public Property RequestTypeName As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsRequestType
   Inherits DataSource(Of ApsRequestType)

End Class

