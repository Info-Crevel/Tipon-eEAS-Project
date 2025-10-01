Public Class DbsApplicantScreening
   Public Property ApplicantScreeningId As Integer
   Public Property ApplicantScreeningName As String

   Public Property UploadRequiredFlag As Boolean
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QDbsApplicantScreening
   Inherits DataSource(Of DbsApplicantScreening)

End Class