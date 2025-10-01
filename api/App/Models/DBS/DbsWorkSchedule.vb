Public Class DbsWorkSchedule
    Public Property ScheduleId As Integer
    Public Property ScheduleCode As String
    Public Property ScheduleIn As String
    Public Property ScheduleOut As String
    Public Property WorkingHours As Integer
    Public Property UnpaidBreak As Integer
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsWorkSchedule
    Inherits DataSource(Of DbsWorkSchedule)
End Class