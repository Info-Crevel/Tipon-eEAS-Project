
Public Class DbsEmploymentStatus
    Public Property EmploymentStatusId As Integer
    Public Property EmploymentStatusName As String

End Class

Public Class QDbsEmploymentStatus
    Inherits DataSource(Of DbsEmploymentStatus)

End Class

