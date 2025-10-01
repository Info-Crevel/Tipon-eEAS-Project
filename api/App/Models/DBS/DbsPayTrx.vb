Public Class DbsPayTrx
   Public Property PayTrxId As Integer
   Public Property PayTrxCode As String
   Public Property PayTrxName As String
   Public Property PayGroupFlag As Boolean
   Public Property MemberRequestFlag As Boolean
   Public Property AccountId As Integer
   Public Property TrxFormula As String
   Public Property SortSeq As Integer
   Public Property LockId As String

End Class

Public Class QDbsPayTrx
   Inherits DataSource(Of DbsPayTrx)
End Class