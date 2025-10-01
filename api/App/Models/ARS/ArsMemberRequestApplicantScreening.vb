Public Class ArsMemberRequestApplicantScreening
   Public Property ScreeningDetailId As Integer
   Public Property ApplicantDetailId As Integer

   Public Property ApplicantScreeningId As Integer
   Public Property ScreeningStatusId As Integer

   Public Property RequestDocTypeId As Integer
   'Public Property RequestDocTypeReference As String
   Public Property RequestDocTypeFileName As String
   Public Property RequestDocTypeGUID As String
   Public Property FileExtension As String
   Public Property FileUrl As String

   Public Property LockId As String

End Class

Public Class QArsMemberRequestApplicantScreening
   Inherits DataSource(Of ArsMemberRequestApplicantScreening)

End Class