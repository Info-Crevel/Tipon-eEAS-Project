Public Class PayTimekeepingSheetMemberSchedule
   Public Property SheetMemberId As Integer
   Public Property NoScheduleId As Integer
   Public Property TimekeepingSheetId As Integer
   Public Property MemberId As Integer
   Public Property ScheduleDate As Date
   Public Property TimekeepingScheduleId As Integer
   Public Property TimekeepingScheduleIdA As Integer
   Public Property TimekeepingScheduleIdB As Integer
   Public Property TimekeepingScheduleIdC As Integer
   Public Property TimekeepingScheduleIdD As Integer
   Public Property ScheduleTimeIn As TimeSpan
   Public Property ScheduleTimeOut As TimeSpan
   Public Property ScheduleTimeInA As TimeSpan
   Public Property ScheduleTimeOutA As TimeSpan
   Public Property ScheduleTimeInB As TimeSpan
   Public Property ScheduleTimeOutB As TimeSpan

   Public Property ScheduleTimeInC As TimeSpan
   Public Property ScheduleTimeOutC As TimeSpan

   Public Property ScheduleTimeInD As TimeSpan
   Public Property ScheduleTimeOutD As TimeSpan
   Public Property TotalWorkingHour As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String


End Class

Public Class PayTimekeepingSheetMemberScheduleList
   Inherits List(Of PayTimekeepingSheetMemberSchedule)

End Class

