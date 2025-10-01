'Public Structure SysPageInfo
'   Public Sub New(pageid As String, pagePath As String)
'      Me.PageId = pageid
'      Me.PagePath = pagePath
'   End Sub

'   Public ReadOnly Property PageId As String
'   Public ReadOnly Property PagePath As String

'End Structure

' 05 Feb 2025 - EMT
Public Structure SysPageInfo
   Public Sub New(pageid As String, pagePath As String, pageName As String, password As String, xa As Boolean, xe As Boolean, xd As Boolean)
      Me.PageId = pageid
      Me.PagePath = pagePath
      Me.PageName = pageName
      Me.Password = password
      Me.XA = xa
      Me.XE = xe
      Me.XD = xd
   End Sub

   Public ReadOnly Property PageId As String
   Public ReadOnly Property PagePath As String
   Public ReadOnly Property PageName As String
   Public ReadOnly Property Password As String
   Public ReadOnly Property XA As Boolean
   Public ReadOnly Property XE As Boolean
   Public ReadOnly Property XD As Boolean

End Structure

'Public Structure SysRoleInfo
'   Public Sub New(roleId As String)
'      Me.RoleId = roleId
'   End Sub

'   Public ReadOnly Property RoleId As String

'End Structure

'Public Structure SysRestrictionInfo
'   Public Sub New(actionId As String)
'      Me.ActionId = actionId
'   End Sub

'   Public ReadOnly Property ActionId As String

'End Structure

'Public Structure SysRestrictionInfo
'   Public Sub New(restrictionId As String, password As String)
'      Me.RestrictionId = restrictionId
'      Me.Password = password
'   End Sub

'   Public ReadOnly Property RestrictionId As String
'   Public ReadOnly Property Password As String

'End Structure

' 01 Mar 2025 - EMT

Public Structure SysRestrictionInfo
   Public Sub New(actionId As String, actionName As String, password As String)
      Me.ActionId = actionId
      Me.ActionName = actionName
      Me.Password = password
   End Sub

   Public ReadOnly Property ActionId As String
   Public ReadOnly Property ActionName As String
   Public ReadOnly Property Password As String

End Structure


'Public Structure SysTokenInfo
'   Public Sub New(roles As String, expiryDate As Date)
'      Me.Roles = roles
'      Me.ExpiryDate = expiryDate
'   End Sub

'   Public ReadOnly Property Roles As String
'   Public ReadOnly Property ExpiryDate As Date

'End Structure

' 01 Aug 2022 - EMT (eFRS)
Public Structure SysModuleInfo
   Public Sub New(moduleId As String, moduleName As String, moduleShortName As String)
      Me.ModuleId = moduleId
      Me.ModuleName = moduleName
      Me.ModuleShortName = moduleShortName
   End Sub

   Public ReadOnly Property ModuleId As String
   Public ReadOnly Property ModuleName As String
   Public ReadOnly Property ModuleShortName As String

End Structure

' 05 Feb 2025 - EMT
Public Structure SysOrgInfo
   Public Sub New(OrgId As Integer, OrgName As String, OrgShortName As String)
      Me.OrgId = OrgId
      Me.OrgName = OrgName
      Me.OrgShortName = OrgShortName
   End Sub

   Public ReadOnly Property OrgId As Integer
   Public ReadOnly Property OrgName As String
   Public ReadOnly Property OrgShortName As String

End Structure

' 05 Feb 2025 - EMT
Public Structure SysPlatformInfo
   Public Sub New(PlatformId As Integer, PlatformName As String, PlatformShortName As String)
      Me.PlatformId = PlatformId
      Me.PlatformName = PlatformName
      Me.PlatformShortName = PlatformShortName
   End Sub

   Public ReadOnly Property PlatformId As Integer
   Public ReadOnly Property PlatformName As String
   Public ReadOnly Property PlatformShortName As String

End Structure
