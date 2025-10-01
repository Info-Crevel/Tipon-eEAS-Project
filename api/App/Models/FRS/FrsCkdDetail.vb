Public Class FrsCkdDetail
   Public Property DetailId As Integer
   Public Property CkdId As Integer
   Public Property DbxId As Integer
   Public Property AccountId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FrsCkdDetailList
   Inherits List(Of FrsCkdDetail)

End Class
