Public Class ApsOrgTrx
   Public Property OrgTrxId As Integer
   Public Property OrgId As Integer
   Public Property RequestTrxName As String
   Public Property RequestTrxTypeId As Integer
   Public Property AccountId As String

   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ApsOrgTrxList
   Inherits List(Of ApsOrgTrx)

End Class

