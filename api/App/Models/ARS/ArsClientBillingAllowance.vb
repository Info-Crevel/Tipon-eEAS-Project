Public Class ArsClientBillingAllowance
   Public Property BillingAllowanceId As Integer
   Public Property ClientBillingDetailId As Integer
   Public Property AllowanceId As Integer
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsClientBillingAllowanceList
   Inherits List(Of ArsClientBillingAllowance)

End Class

