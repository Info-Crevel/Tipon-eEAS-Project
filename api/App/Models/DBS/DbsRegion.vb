Public Class DbsRegion
   Public Property RegionId As String
   Public Property RegionName As String
   Public Property LockId As String

End Class

Public Class QDbsRegion
    Inherits DataSource(Of DbsRegion)

End Class
