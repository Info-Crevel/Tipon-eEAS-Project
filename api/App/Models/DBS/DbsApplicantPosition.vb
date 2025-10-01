Public Class DbsApplicantPosition
    Public Property ApplicantPositionId As Integer
    Public Property ApplicantPositionName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsApplicationPosition
    Inherits DataSource(Of DbsApplicantPosition)

End Class