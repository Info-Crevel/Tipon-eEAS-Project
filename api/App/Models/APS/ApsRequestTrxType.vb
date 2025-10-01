Public Class ApsRequestTrxType
   Public Property RequestTrxTypeId As Integer
   Public Property RequestTrxTypeName As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsRequestTrxType
   Inherits DataSource(Of ApsRequestTrxType)

End Class

