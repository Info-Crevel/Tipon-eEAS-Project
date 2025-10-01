Public Class ArsClientPayOutDayType
    Public Property ClientPayOutDayTypeDetailId As Integer
   'Public Property PayOutDayTypeId As Integer
   Public Property ClientPayGroupId As Integer

    Public Property ClientPayOutDetailId As Integer
    Public Property DayTypeId As Integer
   Public Property PremiumPercentage As Decimal
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class ArsClientPayOutDayTypeList
   Inherits List(Of ArsClientPayOutDayType)

End Class