Public Class DbsProvince
    Public Property ProvinceId As String
    Public Property ProvinceName As String
    'Public Property RegionId As String
    Public Property LockId As String

End Class


Public Class QDbsProvince
    Inherits DataSource(Of DbsProvince)

End Class

