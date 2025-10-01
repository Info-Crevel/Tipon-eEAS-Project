Public Class DbsMatrixPbgDetail
   Public Property PbgRangeId As Integer
   Public Property PbgId As Integer
   Public Property MinAmount As Decimal
   Public Property MaxAmount As Decimal
   Public Property EmployeeRate As Decimal
   Public Property EmployerRate As Decimal
   Public Property MaxRate As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class DbsMatrixPbgDetailList
   Inherits List(Of DbsMatrixPbgDetail)
End Class
