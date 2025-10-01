Public Class DbsProvinceDetail
   Public Property MunicipalityId As String
    Public Property MunicipalityName As String
    Public Property PostalCode As String
    Public Property ProvinceId As String
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsProvinceDetailList
   Inherits List(Of DbsProvinceDetail)

End Class
