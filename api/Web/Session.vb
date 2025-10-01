Public NotInheritable Class Session
   Private Sub New()
      MyBase.New()
   End Sub

   Public Shared Sub Open()

      DataLib.LoadConnectionDefinitions()

      Using _sqlDirect As New SqlDirect("web.SysGetSession")
         Using _dataSet As DataSet = _sqlDirect.ExecuteDataSet()
            Session.LoadSystemInfo(_dataSet)
         End Using
      End Using

   End Sub

   Public Shared Sub Close()
   End Sub

   Public Shared Function GetReferenceDates() As ReferenceDateInfo

      Dim _info As New ReferenceDateInfo

      Using _sqlDirect As New SqlDirect("web.SysGetReferenceDates")
         Dim _reader As DbDataReader = _sqlDirect.ExecuteReader()

         While _reader.Read
            _info.SystemDate = _reader.ToDate("SystemDate")
            _info.AuditDate = _reader.ToDate("AuditDate")
            _info.ServerDate = _reader.ToDate("ServerDateTime")
            _info.ServerTime = _reader.ToDateTime("ServerDateTime").TimeOfDay
         End While

         _reader.Close()
      End Using

      Return _info

   End Function

   Public Shared ReadOnly Property SYS As New SysInfo
   Public Shared ReadOnly Property Pages As New PageList

   Private Shared Sub LoadSystemInfo(dataSet As DataSet)

      With dataSet.Tables(0).Rows(0)
         Session.SYS.SiteId = .ToInt32("SiteId")
         Session.SYS.SiteName = .ToString("SiteName")
         Session.SYS.SiteShortName = .ToString("SiteShortName")
         Session.SYS.SessionTimeout = .ToInt32("SessionTimeout")
         Session.SYS.CurrencyId = .ToString("CurrencyId")
         Session.SYS.ActiveTokenCount = .ToInt32("ActiveTokenCount")
         Session.SYS.YearId = .ToInt32("YearId")
      End With

      Dim _pageList As New PageList

      For Each _row As DataRow In dataSet.Tables(1).Rows
         _pageList.Add(New PageInfo(_row.ToString("PageId"), _row.ToString("PageName"), _row.ToString("PagePath")))
      Next

      Session.SYS.Pages = _pageList.ToArray()

   End Sub

End Class
