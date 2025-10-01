Public Class ArsMemberDeminimisTrx
   Public Property MemberDeminimisTrxId As Integer
   Public Property MemberDeminimisId As Integer
   Public Property PayTrxCode As String
   Public Property DailyFlag As Boolean
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ArsMemberDeminimisTrxList
   Inherits List(Of ArsMemberDeminimisTrx)

End Class
