Public Class ArsMemberRequestCDATraining
    Public Property CDATrainingDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property CDATrainingId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestCDATrainingList
    Inherits List(Of ArsMemberRequestCDATraining)

End Class

