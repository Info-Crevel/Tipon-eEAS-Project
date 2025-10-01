Public Class DbsMunicipality
   Public Property MunicipalityId As String
    Public Property MunicipalityName As String
    Public Property ProvinceId As String
    Public Property PostalCode As String
    Public Property LockId As String

End Class

Public Class QDbsMunicipality
    Inherits DataSource(Of DbsMunicipality)

End Class
