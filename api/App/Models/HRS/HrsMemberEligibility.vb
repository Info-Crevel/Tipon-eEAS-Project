Public Class HrsMemberEligibility
    Public Property EligibilityDetailId As Integer
    Public Property MemberId As Integer
    Public Property EligibilityName As String
    Public Property YearTaken As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberEligibilityList
    Inherits List(Of HrsMemberEligibility)

End Class

