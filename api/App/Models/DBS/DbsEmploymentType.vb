
Public Class DbsEmploymentType
    Public Property EmploymentTypeId As Integer
    Public Property EmploymentTypeName As String

End Class

Public Class QDbsEmploymentType
    Inherits DataSource(Of DbsEmploymentType)

End Class

