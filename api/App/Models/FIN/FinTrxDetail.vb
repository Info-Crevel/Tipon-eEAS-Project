Public Class FinTrxDetail
   Public Property TrxDetailId As Integer
   Public Property TrxId As Integer
   Public Property AccountId As String
   Public Property DebitAmount As Decimal
   Public Property CreditAmount As Decimal
   Public Property OrgId As Integer
   Public Property PlatformId As Integer
   Public Property ClusterId As Integer
   Public Property ClientPayGroupId As Integer
   Public Property PayeeId As Integer
   Public Property Details As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FinTrxDetailList
   Inherits List(Of FinTrxDetail)

End Class
