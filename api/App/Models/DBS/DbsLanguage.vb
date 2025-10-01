Public Class DbsLanguage
    Public Property LanguageId As Integer
    Public Property LanguageName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsLanguage
    Inherits DataSource(Of DbsLanguage)

End Class