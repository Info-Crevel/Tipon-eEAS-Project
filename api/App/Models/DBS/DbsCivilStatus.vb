Public Class DbsCivilStatus
    Public Property CivilStatusId As Integer
    Public Property CivilStatusName As String

End Class

Public Class QDbsCivilStatus
    Inherits DataSource(Of DbsCivilStatus)

End Class

