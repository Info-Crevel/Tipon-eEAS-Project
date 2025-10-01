Public Class DbsHolidayLocation
    Public Property HolidayLocationDetailId As Integer
    Public Property HolidayId As Integer
    Public Property RegionId As String
    Public Property ProvinceId As String
    Public Property MunicipalityId As String
    'Public Property BarangayId As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class DbsHolidayLocationList
    Inherits List(Of DbsHolidayLocation)

End Class
