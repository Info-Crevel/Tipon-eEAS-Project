Public Class DbsMemberTransactionType
    Public Property MemberTransactionTypeId As Integer
    Public Property MemberTransactionTypeName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsMemberTransactionType
    Inherits DataSource(Of DbsMemberTransactionType)
End Class