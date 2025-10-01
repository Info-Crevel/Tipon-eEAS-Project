Public Class DbsWorkForms
    Public Property WorkFormId As Integer
    Public Property WorkFormName As String

    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsWorkForms
    Inherits DataSource(Of DbsWorkForms)
End Class