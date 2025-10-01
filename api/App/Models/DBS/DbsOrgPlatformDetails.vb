Public Class DbsOrgPlatformDetail
    Public Property OrgPlatformId As Integer
    Public Property PlatformId As Integer
    Public Property OrgId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class DbsOrgPlatformDetailList
    Inherits List(Of DbsOrgPlatformDetail)

End Class

