Public Class DbsDocType
    Public Property DocTypeId As Integer
    Public Property DocTypeName As String
    Public Property DocTypeFormat As String
    Public Property DocTypeLength As Integer
    Public Property SortSeq As Integer
    Public Property ApplicantFlag As Boolean
    Public Property ContractFlag As Boolean
    Public Property TradeTestFlag As Boolean
    Public Property ExpirationFlag As Boolean
    Public Property MemberFlag As Boolean
   Public Property LockId As String

End Class

Public Class QDbsDocType
    Inherits DataSource(Of DbsDocType)
End Class