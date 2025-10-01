
Public Class DbsMatrixWhtDetail
   Public Property WhtId As Integer
   Public Property WhtRangeId As Integer
   Public Property PayFreqId As Integer
   Public Property MinAmount As Decimal
   Public Property MaxAmount As Decimal
   Public Property FixedTaxAmount As Decimal
   Public Property ExcessRate As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class
Public Class DbsMatrixWhtDetailList
   Inherits List(Of DbsMatrixWhtDetail)
End Class
