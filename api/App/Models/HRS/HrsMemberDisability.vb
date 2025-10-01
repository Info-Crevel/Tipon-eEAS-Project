Public Class HrsMemberDisability
    Public Property DisabilityDetailId As Integer
    Public Property MemberId As Integer
    Public Property DisabilityId As Integer
    Public Property Remarks As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberDisabilityList
    Inherits List(Of HrsMemberDisability)

End Class
