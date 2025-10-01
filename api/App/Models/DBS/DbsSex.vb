Public Class DbsSex
    Public Property SexId As String
    Public Property SexName As String

End Class

Public Class QDbsSex
    Inherits DataSource(Of DbsSex)

End Class

