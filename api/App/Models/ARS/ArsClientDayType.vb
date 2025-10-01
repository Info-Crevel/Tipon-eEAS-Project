Public Class ArsClientDayType
    Public Property ClientDayTypeDetailId As Integer
    Public Property ClientPayGroupId As Integer
    Public Property DayTypeId As Integer
   Public Property PremiumPercentage As Decimal
   Public Property AdminFee As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsClientDayTypeList
    Inherits List(Of ArsClientDayType)

End Class