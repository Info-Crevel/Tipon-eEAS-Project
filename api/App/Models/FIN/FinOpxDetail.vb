Public Class FinOpxDetail
   Public Property OpxDetailId As Integer
   Public Property OpxId As Integer
   Public Property TrxCode As Integer
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FinOpxDetailList
   Inherits List(Of FinOpxDetail)

End Class

