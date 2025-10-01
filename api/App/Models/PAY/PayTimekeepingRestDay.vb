Public Class PayTimekeepingRestDay
   Public Property TimekeepingRestDayId As Integer
   Public Property TimekeepingId As Integer
   Public Property RestDayId As Integer
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class PayTimekeepingRestDayList
   Inherits List(Of PayTimekeepingRestDay)

End Class

