Public Class QDbsSkillSetDetail
    Public Property SkillDetailId As Integer
    Public Property SkillId As Integer
    Public Property SkillSetId As Integer
    Public Property SkillDetailName As String
    Public Property SortSeq As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class QDbsSkillSetDetailLog
    Inherits DataSource(Of QDbsSkillSetDetail)

End Class
