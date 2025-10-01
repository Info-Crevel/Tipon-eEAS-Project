Public Class ArsClientOvertime
    Public Property ClientOvertimeDetailId As Integer
    Public Property ClientPayGroupId As Integer
    Public Property PayTrxCode As String
    Public Property LogActionId As Integer

    Public Property LockId As String

End Class

Public Class ArsClientOvertimeList
    Inherits List(Of ArsClientOvertime)

End Class

