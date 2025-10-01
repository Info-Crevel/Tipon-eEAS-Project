Public Class ArsClientPayOutAllowance
    Public Property PayOutAllowanceId As Integer
    Public Property ClientPayOutDetailId As Integer
    Public Property AllowanceId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsClientPayOutAllowanceList
    Inherits List(Of ArsClientPayOutAllowance)

End Class

