Public Class ArsClientBilling
    Public Property ClientBillingDetailId As Integer
    Public Property ClientPayGroupId As Integer
    Public Property ChargingConsiderationId As Integer
    Public Property PayTrxCode As String
    Public Property BillingSheetFormula As String
    Public Property FixedAmount As Decimal
    Public Property BasicFlag As Boolean
    Public Property DeminimisFlag As Boolean
    Public Property AllowanceFlag As Boolean
    Public Property OvertimeFlag As Boolean
    Public Property NightDifferentialFlag As Boolean
    Public Property HolidayFlag As Boolean
    Public Property RestDayFlag As Boolean
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsClientBillingList
    Inherits List(Of ArsClientBilling)

End Class