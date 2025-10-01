Public Class DbsSkillSetDetail
    Public Property SkillDetailId As Integer
    Public Property SkillId As Integer
    Public Property SkillSetId As Integer
    Public Property SortSeq As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class DbsSkillSetDetailList
    Inherits List(Of DbsSkillSetDetail)

End Class
