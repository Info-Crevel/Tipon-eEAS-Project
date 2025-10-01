Public Class ArsClientBillingDeminimis
   Public Property BillingDeminimisId As Integer
   Public Property ClientBillingDetailId As Integer
   Public Property DeminimisId As Integer
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsClientBillingDeminimisList
   Inherits List(Of ArsClientBillingDeminimis)

End Class

