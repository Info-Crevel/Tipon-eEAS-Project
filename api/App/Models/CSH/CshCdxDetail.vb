Public Class CshCdxDetail
   Public Property CdxDetailId As Integer
   Public Property CdxId As Integer
   Public Property CdxTypeId As Integer
   Public Property ReferenceDate As Date
   Public Property Reference As String
   Public Property PayeeName As String
   Public Property AccountId As String
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class CshCdxDetailList
   Inherits List(Of CshCdxDetail)

End Class
