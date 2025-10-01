Public Class DbsBarangay
   Public Property BarangayId As String
   Public Property BarangayName As String
   Public Property MunicipalityId As String
   Public Property PostalCode As String
   Public Property LockId As String

End Class

Public Class QDbsBarangay
    Inherits DataSource(Of DbsBarangay)

End Class
