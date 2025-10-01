Public Class FrsAdaDetail
   Public Property DetailId As Integer
   Public Property AdaId As Integer
   Public Property DbxId As Integer
   Public Property AccountId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FrsAdaDetailList
   Inherits List(Of FrsAdaDetail)

End Class
