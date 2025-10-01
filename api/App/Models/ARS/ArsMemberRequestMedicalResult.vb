Public Class ArsMemberRequestMedicalResult
   Public Property MedicalResultDetailId As Integer
   Public Property MemberRequestId As Integer
   Public Property MedicalResultTypeId As Integer
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class ArsMemberRequestMedicalResultList
   Inherits List(Of ArsMemberRequestMedicalResult)

End Class

