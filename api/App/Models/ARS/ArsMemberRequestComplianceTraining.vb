Public Class ArsMemberRequestComplianceTraining
    Public Property ComplianceTrainingDetailId As Integer
    Public Property MemberRequestId As Integer
    Public Property ComplianceTrainingId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class ArsMemberRequestComplianceTrainingList
    Inherits List(Of ArsMemberRequestComplianceTraining)

End Class

