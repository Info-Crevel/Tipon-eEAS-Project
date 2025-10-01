Public Class DbsSkill
    Public Property SkillId As Integer
    Public Property SkillName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsSkill
    Inherits DataSource(Of DbsSkill)
End Class