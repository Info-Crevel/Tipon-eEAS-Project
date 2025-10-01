Public Class ApsRequestTrxDetail
   Public Property RequestTrxDetailId As Integer
   Public Property RequestTrxId As Integer
   Public Property OrgTrxId As Integer
   Public Property OrgId As Integer
   Public Property PlatformId As Integer
   Public Property ClusterId As Integer
   Public Property ClientPayGroupId As Integer
   Public Property ApsDocTypeId As Integer
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class ApsRequestTrxDetailList
   Inherits List(Of ApsRequestTrxDetail)

End Class
