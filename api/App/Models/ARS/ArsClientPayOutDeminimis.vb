Public Class ArsClientPayOutDeminimis
    Public Property PayOutDeminimisId As Integer
    Public Property ClientPayOutDetailId As Integer
    Public Property DeminimisId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsClientPayOutDeminimisList
    Inherits List(Of ArsClientPayOutDeminimis)

End Class

