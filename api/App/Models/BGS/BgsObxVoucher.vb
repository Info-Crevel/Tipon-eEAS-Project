Public Class BgsObxVoucher
   Public Property ObxVoucherId As Integer
   Public Property ObxId As Integer
   Public Property VoucherId As String
   Public Property TotalAmount As Decimal
   Public Property TaxAmount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class BgsObxVoucherList
   Inherits List(Of BgsObxVoucher)

End Class

