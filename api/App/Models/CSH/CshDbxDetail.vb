Public Class CshDbxDetail
   Public Property DbxDetailId As Integer
   Public Property DbxId As Integer
   Public Property AccountId As String
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String
   Public Property AccountName As String              ' used by logs

End Class

Public Class CshDbxDetailList
   Inherits List(Of CshDbxDetail)

End Class

