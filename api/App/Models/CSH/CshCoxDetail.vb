Public Class CshCoxDetail
   Public Property CoxDetailId As Integer
   Public Property CoxId As Integer
   Public Property TrxCode As Integer
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String
   Public Property TrxName As String              ' used by logs

End Class

Public Class CshCoxDetailList
   Inherits List(Of CshCoxDetail)

End Class
