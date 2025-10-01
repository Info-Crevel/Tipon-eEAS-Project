Public Class HrsMemberLanguage
    Public Property LanguageDetailId As Integer
    Public Property MemberId As Integer
    Public Property LanguageId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberLanguageList
    Inherits List(Of HrsMemberLanguage)

End Class
