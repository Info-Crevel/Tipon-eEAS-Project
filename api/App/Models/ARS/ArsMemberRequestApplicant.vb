Public Class ArsMemberRequestApplicant
   Public Property ApplicantDetailId As Integer
   Public Property MemberRequestId As Integer
   Public Property MemberId As Integer
   Public Property ApplicantId As Integer
   Public Property ApplicantScreeningId As Integer
   Public Property ScreeningStatusId As Integer
   Public Property ApplicantStatusId As Integer
   Public Property Remarks As String
   Public Property HiredDate As Date
   Public Property DeploymentDate As Date
   Public Property EmploymentStatusId As Integer
    Public Property Name3 As String
    Public Property RequestDocTypeId As Integer
    'Public Property RequestDocTypeReference As String
    Public Property RequestDocTypeFileName As String
    Public Property RequestDocTypeGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class QArsMemberRequestApplicant
   Inherits DataSource(Of ArsMemberRequestApplicant)

End Class