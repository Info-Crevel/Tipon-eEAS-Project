Public Class DbsClusterOrgPlatform
   Public Property ClusterOrgPlatformId As Integer
   Public Property ClusterId As Integer
   Public Property OrgId As Integer
   Public Property PlatformId As Integer
   Public Property AccountId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsClusterOrgPlatformList
   Inherits List(Of DbsClusterOrgPlatform)

End Class
