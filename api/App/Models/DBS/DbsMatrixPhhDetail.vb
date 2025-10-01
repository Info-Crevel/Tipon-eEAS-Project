Public Class DbsMatrixPhhDetail
   Public Property PhhRangeId As Integer
   Public Property PhhId As Integer
   Public Property MinAmount As Decimal
   Public Property MaxAmount As Decimal
   Public Property PremiumRate As Decimal
   Public Property FixedAmount As Decimal

   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsMatrixPhhDetailList
   Inherits List(Of DbsMatrixPhhDetail)
End Class
