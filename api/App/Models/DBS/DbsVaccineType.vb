Public Class DbsVaccineType
    Public Property VaccineTypeId As Integer
    Public Property VaccineTypeName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsVaccineType
    Inherits DataSource(Of DbsVaccineType)

End Class