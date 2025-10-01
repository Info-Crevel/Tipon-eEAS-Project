Public Class HrsMemberSkillSet
    Public Property SkillSetDetailId As Integer
    Public Property MemberId As Integer
    Public Property SkillDetailId As Integer
    Public Property Remarks As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberSkillSetList
    Inherits List(Of HrsMemberSkillSet)

End Class
