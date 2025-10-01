Public Class ApsRequestTypeParticulars
   Public Property ParticularsId As Integer
   Public Property ParticularsName As String
   Public Property ParticularsDescription As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QApsRequestTypeParticulars
   Inherits DataSource(Of ApsRequestTypeParticulars)

End Class

