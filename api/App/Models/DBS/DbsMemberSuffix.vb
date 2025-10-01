Public Class DbsMemberSuffix
    Public Property MemberSuffixId As Integer
    Public Property MemberSuffixName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsMemberSuffix
    Inherits DataSource(Of DbsMemberSuffix)

End Class