Public Class DbsBloodType
    Public Property BloodTypeId As Integer
    Public Property BloodTypeName As String

End Class

Public Class QDbsBloodTYpe
    Inherits DataSource(Of DbsBloodType)

End Class

