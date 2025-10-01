Public Class DbsScreeningStatus
   Public Property ScreeningStatusId As Integer
   Public Property ScreeningStatusName As String
   Public Property LockId As String

End Class

Public Class QDbsScreeningStatus
   Inherits DataSource(Of DbsScreeningStatus)

End Class