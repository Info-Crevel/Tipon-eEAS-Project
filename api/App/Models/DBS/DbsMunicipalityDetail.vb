Public Class DbsMunicipalityDetail
   Public Property BarangayId As String
   Public Property BarangayName As String
    Public Property PostalCode As String
   Public Property MunicipalityId As String

   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsMunicipalityDetailList
   Inherits List(Of DbsMunicipalityDetail)

End Class

