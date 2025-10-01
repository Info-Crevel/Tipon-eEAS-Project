Public Class SysRole
   Public Property RoleId As Integer
   Public Property RoleName As String
   Public Property LockId As String

End Class

Public Class SysRolePage
   Public Property RoleId As Integer
   Public Property PageId As String

End Class

Public Class SysRolePageList
   Inherits List(Of SysRolePage)

End Class

