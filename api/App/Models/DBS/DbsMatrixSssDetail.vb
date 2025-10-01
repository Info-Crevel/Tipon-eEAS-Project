Public Class DbsMatrixSssDetail
   Public Property SssRangeId As Integer
   Public Property SssId As Integer
   Public Property MinAmount As Decimal
   Public Property MaxAmount As Decimal
   Public Property SsEmployerAmount As Decimal
   Public Property SsEmployeeAmount As Decimal
   Public Property EcEmployerAmount As Decimal
   Public Property EcEmployeeAmount As Decimal
   Public Property WispEmployerAmount As Decimal
   Public Property WispEmployeeAmount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class
Public Class DbsMatrixSssDetailList
   Inherits List(Of DbsMatrixSssDetail)
End Class