Public Class PayTimekeepingSchedule
   Public Property TimekeepingScheduleId As Integer
   Public Property TimekeepingId As Integer
   Public Property TimeIn As TimeSpan
   Public Property TimeOut As TimeSpan
   Public Property WorkingHour As Integer
   Public Property UnpaidBreak As Integer
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class PayTimekeepingScheduleList
   Inherits List(Of PayTimekeepingSchedule)

End Class

