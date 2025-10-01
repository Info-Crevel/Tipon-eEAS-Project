Public Class ArsMemberAllowanceTrx
   Public Property MemberAllowanceTrxId As Integer
   Public Property MemberAllowanceId As Integer
   Public Property PayTrxCode As String
   Public Property DailyFlag As Boolean
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsMemberAllowanceTrxList
   Inherits List(Of ArsMemberAllowanceTrx)

End Class
