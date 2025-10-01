Public Class ArsClientBillingDayType
   Public Property BillingDayTypeId As Integer
   Public Property ClientBillingDetailId As Integer
   Public Property DayTypeId As Integer
   Public Property PremiumPercentage As Decimal
   Public Property AdminFee As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsClientBillingDayTypeList
   Inherits List(Of ArsClientBillingDayType)

End Class