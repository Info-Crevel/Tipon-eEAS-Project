Public Class DbsApplicantStatus
   Public Property ApplicantStatusId As Integer
   Public Property ApplicantStatusName As String

End Class

Public Class QDbsApplicantStatus
   Inherits DataSource(Of DbsApplicantStatus)

End Class