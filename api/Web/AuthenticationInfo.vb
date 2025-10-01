Public Class AuthenticationInfo
   Private Sub New()
      MyBase.New
   End Sub

    'Friend Sub New(userId As Integer, userName As String, userShortName As String, photo As Byte(), imageUrl As String, pageId As String, pageName As String, pagePath As String, isSysAdmin As Boolean, canChangePassword As Boolean, pages As SysPageInfo(), restrictions As SysRestrictionInfo(), token As String, userCode As String, careCenterId As Integer, careCenterName As String, expiredFlag As Boolean)
    'Friend Sub New(userId As Integer, userName As String, userShortName As String, photo As Byte(), imageUrl As String, pageId As String, pageName As String, pagePath As String, isSysAdmin As Boolean, canChangePassword As Boolean, pages As SysPageInfo(), restrictions As SysRestrictionInfo(), modules As SysModuleInfo(), moduleId As String, token As String, userCode As String, expiredFlag As Boolean)
    Friend Sub New(userId As Integer, logOnId As String, userName As String, userShortName As String, photo As Byte(), imageUrl As String, pageId As String, pageName As String, pagePath As String, isSysAdmin As Boolean, workgroupName As String, canChangePassword As Boolean, webChangePassword As Boolean, pages As SysPageInfo(), restrictions As SysRestrictionInfo(), modules As SysModuleInfo(), orgs As SysOrgInfo(), platforms As SysPlatformInfo(), moduleId As String, token As String, userCode As String, expiredFlag As Boolean, userOrgId As Integer, userOrgShortName As String)
        MyBase.New()

        Me.UserId = userId
        Me.LogOnId = logOnId
        Me.UserName = userName
        Me.UserShortName = userShortName
        Me.UserOrgId = userOrgId
        Me.UserOrgShortName = userOrgShortName

        Me.Photo = photo
        Me.ImageUrl = imageUrl
        Me.PageId = pageId
        Me.PageName = pageName
        Me.PagePath = pagePath
        Me.IsSysAdmin = isSysAdmin
        Me.WorkgroupName = workgroupName
        Me.CanChangePassword = canChangePassword
        Me.WebChangePassword = webChangePassword
        Me.Pages = pages
        Me.Restrictions = restrictions
        Me.Modules = modules
        Me.Orgs = orgs
        Me.Platforms = platforms

        Me.ModuleId = moduleId
        Me.Token = token
        Me.UserCode = userCode
        'Me.CareCenterId = CareCenterId
        'Me.CareCenterName = CareCenterName
        Me.ExpiredFlag = expiredFlag

    End Sub

    Public ReadOnly Property UserId As Integer
   Public ReadOnly Property LogOnId As String
   Public ReadOnly Property UserName As String
    Public ReadOnly Property UserShortName As String

    Public ReadOnly Property UserOrgId As Integer
    Public ReadOnly Property UserOrgShortName As String

    Public ReadOnly Property Photo As Byte()
    Public ReadOnly Property ImageUrl As String
   Public ReadOnly Property PageId As String
   Public ReadOnly Property PageName As String
   Public ReadOnly Property PagePath As String
   Public ReadOnly Property IsSysAdmin As Boolean
   Public ReadOnly Property WorkgroupName As String      ' 25 Mar 2024 - EMT (eFRS)
   Public ReadOnly Property CanChangePassword As Boolean
   Public ReadOnly Property WebChangePassword As Boolean
   Public ReadOnly Property Pages As SysPageInfo()
   Public ReadOnly Property Restrictions As SysRestrictionInfo()
   Public ReadOnly Property Modules As SysModuleInfo()      ' 01 Aug 2022 - EMT (eFRS)
   Public ReadOnly Property Orgs As SysOrgInfo()      ' 01 Aug 2022 - EMT (eFRS)
   Public ReadOnly Property Platforms As SysPlatformInfo()      ' 01 Aug 2022 - EMT (eFRS)

   Public ReadOnly Property ModuleId As String              ' 01 Aug 2022 - EMT (eFRS)
   Public ReadOnly Property Token As String
   Public ReadOnly Property UserCode As String
   'Public ReadOnly Property CareCenterId As Integer
   'Public ReadOnly Property CareCenterName As String
   Public ReadOnly Property ExpiredFlag As Boolean

End Class
