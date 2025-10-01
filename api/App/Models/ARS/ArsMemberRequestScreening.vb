Public Class ArsMemberRequestScreening
   Public Property ScreeningDetailId As Integer
   Public Property MemberRequestId As Integer
   Public Property ApplicantScreeningId As Integer
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class ArsMemberRequestScreeningList
   Inherits List(Of ArsMemberRequestScreening)

End Class

