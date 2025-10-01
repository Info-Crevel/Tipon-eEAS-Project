Public Class DbsPayVariable
    Public Property PayVariableId As Integer
    Public Property PayVariableCode As String
    Public Property PayVariableName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsPayVariable
    Inherits DataSource(Of DbsPayVariable)
End Class