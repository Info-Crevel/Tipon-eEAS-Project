Public Class ArsClientPayOutBasicRate
    Public Property PayOutBasicRateId As Integer
    Public Property ClientPayOutDetailId As Integer
    Public Property PayTrxCode As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsClientPayOutBasicRateList
    Inherits List(Of ArsClientPayOutBasicRate)

End Class

