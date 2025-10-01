Public Class ArsMemberRequestPayOut
   Public Property PayOutDetailId As Integer
   Public Property MemberRequestId As Integer
   Public Property PayTrxCode As String
   Public Property DailyRate As Decimal
   Public Property MonthlyRate As Decimal
   Public Property FixedFlag As Boolean
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class
Public Class ArsMemberRequestPayOutList
   Inherits List(Of ArsMemberRequestPayOut)

End Class