Public Class ArsClientPayOutHoliday
   Public Property ClientPayOutHolidayDetailId As Integer
   Public Property ClientPayGroupId As Integer
   Public Property DayTypeId As Integer
   Public Property HolidayId As Integer
   Public Property HolidayDate As Date
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class ArsClientPayOutHolidayList
   Inherits List(Of ArsClientPayOutHoliday)

End Class