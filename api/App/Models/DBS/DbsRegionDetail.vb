Public Class DbsRegionDetail
   Public Property ProvinceId As String
   Public Property ProvinceName As String
   Public Property RegionId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsRegionDetailList
   Inherits List(Of DbsRegionDetail)

End Class
