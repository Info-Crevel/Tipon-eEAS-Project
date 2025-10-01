Public Class ArsClientBillingBasicRate
   Public Property BillingBasicRateId As Integer
   Public Property ClientBillingDetailId As Integer
   Public Property PayTrxCode As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsClientBillingBasicRateList
   Inherits List(Of ArsClientBillingBasicRate)

End Class

