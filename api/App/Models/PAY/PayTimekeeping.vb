Public Class PayTimekeeping
   Public Property TimekeepingId As Integer
   Public Property TimekeepingDescription As String
   Public Property PolicyStatusId As Integer

   'Public Property MemberRequestId As Integer
   Public Property MinimumRequiredOvertimeCount As Integer
   Public Property MinimumRequiredOvertimeUnitId As Integer
   Public Property OvertimeDurationCount As Integer
   Public Property OvertimeDurationUnitId As Integer
   Public Property OvertimeCountId As Integer
   Public Property OvertimeStartDate As Date
   Public Property OvertimeEndDate As Date
   Public Property MaximumRegularHourCount As Integer
   Public Property MaximumRegularHourUnitId As Integer
   Public Property LateGracePeriodCount As Integer
   Public Property LateGracePeriodUnitId As Integer
   'Public Property Address2 As String
   Public Property LateCount As Integer
   Public Property NDCount As Integer
   Public Property LateUnitId As Integer
   Public Property PaidUpHolidayId As Integer
   Public Property NightDifferentialCountId As Integer
   Public Property RestDayOffsetFlag As Boolean

   Public Property RestDayConsideration As Integer
   Public Property LateCountId As Integer
   Public Property HolidayCountId As Integer
   Public Property UndertimeCountId As Integer
   Public Property RegularHourIntervalMinute As Integer
   Public Property ConstructionFlag As Boolean
   Public Property UndertimeDurationPerCount As Integer
   Public Property LockId As String

End Class
