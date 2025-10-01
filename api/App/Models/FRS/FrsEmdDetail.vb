Public Class FrsEmdDetail
   Public Property DetailId As Integer
   Public Property EmdId As Integer
   Public Property DbxId As Integer
   Public Property AccountId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FrsEmdDetailList
   Inherits List(Of FrsEmdDetail)

End Class

