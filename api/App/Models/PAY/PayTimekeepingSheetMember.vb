Public Class PayTimekeepingSheetMember
   Public Property SheetMemberId As Integer
   Public Property TimekeepingSheetId As Integer
   Public Property MemberId As Integer
   Public Property ScheduleDate As Date
   Public Property NoScheduleId As Integer
   Public Property TimekeepingScheduleId As Integer
   Public Property TimekeepingScheduleIdA As Integer
   Public Property TimekeepingScheduleIdB As Integer
   Public Property TimekeepingScheduleIdC As Integer
   Public Property TimekeepingScheduleIdD As Integer
   Public Property ScheduleTimeIn As TimeSpan
   Public Property ScheduleTimeOut As TimeSpan
   Public Property ScheduleDateTimeOut As Date
   Public Property ScheduleTimeInA As TimeSpan
   Public Property ScheduleTimeOutA As TimeSpan
   Public Property ScheduleTimeInB As TimeSpan
   Public Property ScheduleTimeOutB As TimeSpan

   Public Property ScheduleTimeInC As TimeSpan
   Public Property ScheduleTimeOutC As TimeSpan

   Public Property ScheduleTimeInD As TimeSpan
   Public Property ScheduleTimeOutD As TimeSpan
   Public Property TotalWorkingHour As Decimal


   Public Property ApprovedFormFlag As Boolean
   Public Property PayableToMemberFlag As Boolean
   Public Property BillableToClientFlag As Boolean

   Public Property Remarks As String
   'Public Property RegularHour As Decimal
   'Public Property OvertimeHour As Decimal
   'Public Property RegularWorkingDayHour As Decimal
   'Public Property RegularOvertimeHour As Decimal
   'Public Property LateMinute As Decimal
   'Public Property UndertimeMinute As Decimal

   Public Property OTRemarks As String
   Public Property OTFileName As String
   Public Property OTGUID As String
   Public Property FileExtension As String
   Public Property FileUrl As String

   Public Property regularHour As Decimal
   Public Property overtimeHour As Decimal
   Public Property regularWorkingDayHour As Decimal
   Public Property regularOvertimeHour As Decimal
   Public Property lateMinute As Decimal
   Public Property undertimeMinute As Decimal
   Public Property ND As Decimal
   Public Property NDOT As Decimal
   Public Property RD As Decimal
   Public Property RDOT As Decimal
   Public Property RDND As Decimal
   Public Property RDNDOT As Decimal
   Public Property SH As Decimal
   Public Property SHOT As Decimal
   Public Property SHND As Decimal
   Public Property SHNDOT As Decimal
   Public Property SHRD As Decimal
   Public Property SHRDOT As Decimal
   Public Property SHRDND As Decimal
   Public Property SHRDNDOT As Decimal
   Public Property LHRD As Decimal
   Public Property LHRDOT As Decimal
   Public Property LHRDND As Decimal
   Public Property LHRDNDOT As Decimal
   Public Property LH As Decimal
   Public Property LHOT As Decimal
   Public Property LHND As Decimal
   Public Property LHNDOT As Decimal
   Public Property LHSH As Decimal
   Public Property LHSHOT As Decimal
   Public Property LHSHND As Decimal
   Public Property LHSHNDOT As Decimal

   Public Property LHSHRD As Decimal
   Public Property LHSHRDOT As Decimal
   Public Property LHSHRDND As Decimal
   Public Property LHSHRDNDOT As Decimal
   Public Property HU100 As Decimal
   Public Property DRH As Decimal
   Public Property DRHOT As Decimal
   Public Property DRHND As Decimal
   Public Property DRHNDOT As Decimal
   Public Property DRHRD As Decimal
   Public Property DRHRDOT As Decimal
   Public Property DRHRDND As Decimal
   Public Property DRHRDNDOT As Decimal

   Public Property regularHourP As Decimal
   Public Property overtimeHourP As Decimal
   Public Property lateMinuteP As Decimal
   Public Property undertimeMinuteP As Decimal
   Public Property NDP As Decimal
   Public Property NDOTP As Decimal
   Public Property RDP As Decimal
   Public Property RDOTP As Decimal
   Public Property RDNDP As Decimal
   Public Property RDNDOTP As Decimal
   Public Property SHP As Decimal
   Public Property SHOTP As Decimal
   Public Property SHNDP As Decimal
   Public Property SHNDOTP As Decimal
   Public Property SHRDP As Decimal
   Public Property SHRDOTP As Decimal
   Public Property SHRDNDP As Decimal
   Public Property SHRDNDOTP As Decimal
   Public Property LHRDP As Decimal
   Public Property LHRDOTP As Decimal
   Public Property LHRDNDP As Decimal
   Public Property LHRDNDOTP As Decimal
   Public Property LHP As Decimal
   Public Property LHOTP As Decimal
   Public Property LHNDP As Decimal
   Public Property LHNDOTP As Decimal
   Public Property LHSHP As Decimal
   Public Property LHSHOTP As Decimal
   Public Property LHSHNDP As Decimal
   Public Property LHSHNDOTP As Decimal
   Public Property LHSHRDP As Decimal
   Public Property LHSHRDOTP As Decimal
   Public Property LHSHRDNDP As Decimal
   Public Property LHSHRDNDOTP As Decimal
   Public Property HU100P As Decimal
   Public Property DRHP As Decimal
   Public Property DRHOTP As Decimal
   Public Property DRHNDP As Decimal
   Public Property DRHNDOTP As Decimal
   Public Property DRHRDP As Decimal
   Public Property DRHRDOTP As Decimal
   Public Property DRHRDNDP As Decimal
   Public Property DRHRDNDOTP As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String


End Class

Public Class PayTimekeepingSheetMemberList
   Inherits List(Of PayTimekeepingSheetMember)

End Class

