

Public Class HrsMemberVaccine
    Public Property VaccineDetailId As Integer
    Public Property MemberId As Integer
    Public Property VaccineTypeId As Integer
    Public Property VaccineName As String
    Public Property VaccineDate As DateTime
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberVaccineList
    Inherits List(Of HrsMemberVaccine)

End Class
