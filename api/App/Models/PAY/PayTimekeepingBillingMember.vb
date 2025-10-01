Public Class PayTimekeepingBillingMember

   Public Property BillingMemberId As Integer
   Public Property TimekeepingBillingId As Integer
   Public Property MemberId As Integer
   Public Property TotalWorkingDay As Decimal
   Public Property AllowanceDaily As Decimal
   Public Property TotalWorkingHour As Decimal
   Public Property PayingDailyRate As Decimal
   Public Property BasicPayOutRate As Decimal
   Public Property RegularHourRate As Decimal
   Public Property OvertimeHourRate As Decimal
   Public Property LateMinuteRate As Decimal
   Public Property UndertimeMinuteRate As Decimal
   Public Property HU100Rate As Decimal
   Public Property OTRate As Decimal
   Public Property RDRate As Decimal
   Public Property RDOTRate As Decimal
   Public Property SHRate As Decimal
   Public Property SHOTRate As Decimal
   Public Property SHRDRate As Decimal
   Public Property SHRDOTRate As Decimal
   Public Property LHRate As Decimal
   Public Property LHOTRate As Decimal
   Public Property LHRDRate As Decimal
   Public Property LHRDOTRate As Decimal
   Public Property LHSHRate As Decimal
   Public Property LHSHRDRate As Decimal
   Public Property DRHRate As Decimal
   Public Property DRHOTRate As Decimal
   Public Property DRHRDRate As Decimal
   Public Property DRHRDOTRate As Decimal
   Public Property NDRate As Decimal
   Public Property NDOTRate As Decimal
   Public Property RDNDRate As Decimal
   Public Property RDNDOTRate As Decimal
   Public Property SHNDRate As Decimal
   Public Property SHNDOTRate As Decimal
   Public Property SHRDNDRate As Decimal
   Public Property SHRDNDOTRate As Decimal
   Public Property LHNDRate As Decimal
   Public Property LHNDOTRate As Decimal
   Public Property LHRDNDRate As Decimal
   Public Property LHRDNDOTRate As Decimal
   Public Property HU100Hour As Decimal
   Public Property OTHour As Decimal
   Public Property RDHour As Decimal
   Public Property RDOTHour As Decimal
   Public Property SHHour As Decimal
   Public Property SHOTHour As Decimal
   Public Property SHRDHour As Decimal
   Public Property SHRDOTHour As Decimal
   Public Property LHHour As Decimal
   Public Property LHOTHour As Decimal
   Public Property LHRDHour As Decimal
   Public Property LHRDOTHour As Decimal
   Public Property LHSHHour As Decimal
   Public Property LHSHRDHour As Decimal
   Public Property DRHHour As Decimal
   Public Property DRHOTHour As Decimal
   Public Property DRHRDHour As Decimal
   Public Property DRHRDOTHour As Decimal
   Public Property NDHour As Decimal
   Public Property NDOTHour As Decimal
   Public Property RDNDHour As Decimal
   Public Property RDNDOTHour As Decimal
   Public Property SHNDHour As Decimal
   Public Property SHNDOTHour As Decimal
   Public Property SHRDNDHour As Decimal
   Public Property SHRDNDOTHour As Decimal
   Public Property LHNDHour As Decimal
   Public Property LHNDOTHour As Decimal
   Public Property LHRDNDHour As Decimal
   Public Property LHRDNDOTHour As Decimal

   Public Property RDPremiumRate As Decimal
   Public Property SHPremiumRate As Decimal
   Public Property LHPremiumrRate As Decimal
   Public Property SHRDPremiumRate As Decimal
   Public Property LHRDPremiumRate As Decimal
   Public Property RDAmount As Decimal
   Public Property SHAmount As Decimal
   Public Property LHAmount As Decimal
   Public Property SHRDAmount As Decimal
   Public Property LHRDAmount As Decimal
   Public Property ExtendedWorkRate As Decimal
   Public Property OtherAllowance As Decimal

   Public Property HU100Perc As Decimal
   Public Property OTPerc As Decimal
   Public Property RDPerc As Decimal
   Public Property RDOTPerc As Decimal
   Public Property SHPerc As Decimal
   Public Property SHOTPerc As Decimal
   Public Property SHRDPerc As Decimal
   Public Property SHRDOTPerc As Decimal
   Public Property LHPerc As Decimal
   Public Property LHOTPerc As Decimal
   Public Property LHRDPerc As Decimal
   Public Property LHRDOTPerc As Decimal
   Public Property LHSHPerc As Decimal
   Public Property LHSHRDPerc As Decimal
   Public Property DRHPerc As Decimal
   Public Property DRHOTPerc As Decimal
   Public Property DRHRDPerc As Decimal
   Public Property DRHRDOTPerc As Decimal
   Public Property NDPerc As Decimal
   Public Property NDOTPerc As Decimal
   Public Property RDNDPerc As Decimal
   Public Property RDNDOTPerc As Decimal
   Public Property SHNDPerc As Decimal
   Public Property SHNDOTPerc As Decimal
   Public Property SHRDNDPerc As Decimal
   Public Property SHRDNDOTPerc As Decimal
   Public Property LHNDPerc As Decimal
   Public Property LHNDOTPerc As Decimal
   Public Property LHRDNDPerc As Decimal
   Public Property LHRDNDOTPerc As Decimal
   Public Property MemberName As String
   Public Property DepartmentName As String
   Public Property GrossPay As Decimal
   Public Property TotalGrossPay As Decimal
   Public Property TotalShare As Decimal
   Public Property AdminFee As Decimal
   Public Property NetBilling As Decimal
   Public Property YEI As Decimal
   Public Property TotalBasic As Decimal
   Public Property TotalGrossPayAF As Decimal

   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class PayTimekeepingBillingMemberList
   Inherits List(Of PayTimekeepingBillingMember)

End Class

