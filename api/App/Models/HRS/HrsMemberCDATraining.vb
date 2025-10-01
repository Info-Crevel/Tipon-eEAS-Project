Public Class HrsMemberCDATraining
    Public Property CDATrainingDetailId As Integer
    Public Property MemberId As Integer
    Public Property CertificateNumber As String
    Public Property CDATrainingId As Integer
    Public Property TrainingDate As Date
    Public Property CDAFileName As String
    Public Property CDAGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberCDATrainingList
    Inherits List(Of HrsMemberCDATraining)

End Class

