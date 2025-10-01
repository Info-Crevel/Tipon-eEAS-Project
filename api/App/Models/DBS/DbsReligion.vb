Public Class DbsReligion
    Public Property ReligionId As Integer
    Public Property ReligionName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsReligion
    Inherits DataSource(Of DbsReligion)

End Class