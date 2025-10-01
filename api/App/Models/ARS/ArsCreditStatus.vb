Public Class ArsCreditStatus
    Public Property CreditStatusId As Integer
    Public Property CreditStatusName As String

End Class
Public Class QArsCreditStatus
    Inherits DataSource(Of ArsCreditStatus)

End Class