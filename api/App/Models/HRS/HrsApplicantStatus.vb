Public Class HrsApplicantStatus
    Public Property ApplicantStatusId As Integer
    Public Property ApplicantStatusName As String


End Class

Public Class QHrsApplicantStatus
    Inherits DataSource(Of HrsApplicantStatus)

End Class

