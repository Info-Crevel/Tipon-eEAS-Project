Public Class FinTemplateDetail
   Public Property TemplateDetailId As Integer
   Public Property TemplateId As Integer
   Public Property AccountId As String
   Public Property DebitAmount As Decimal
   Public Property CreditAmount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class FinTemplateDetailList
   Inherits List(Of FinTemplateDetail)

End Class
