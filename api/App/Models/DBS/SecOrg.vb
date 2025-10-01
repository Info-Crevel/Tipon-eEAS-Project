Public Class SecOrg
    Public Property OrgId As Integer
    Public Property OrgName As String
    Public Property OrgShortName As String
    Public Property SortSeq As Integer
    'Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class QSecOrg
    Inherits DataSource(Of SecOrg)

End Class
