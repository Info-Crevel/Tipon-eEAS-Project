Public Class PayTimekeepingScheduleMember
   Public Property ScheduleMemberId As Integer
   Public Property TimekeepingId As Integer
   Public Property MemberId As Integer
   Public Property ScheduleDate As Date
   Public Property TimekeepingScheduleId As Integer
   Public Property TimekeepingScheduleIdA As Integer
   Public Property TimekeepingScheduleIdB As Integer
   Public Property TimekeepingScheduleIdC As Integer
   Public Property TimekeepingScheduleIdD As Integer

   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class PayTimekeepingScheduleMemberList
   Inherits List(Of PayTimekeepingScheduleMember)

End Class

