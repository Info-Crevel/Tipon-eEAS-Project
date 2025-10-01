Public Class CshDbx
   Public Property DbxId As Integer
   Public Property TrxDate As Date
   Public Property FundClusterId As String
   Public Property TrxStatusId As Integer
   Public Property DbxTypeId As Integer
   Public Property ObxId As Integer
   Public Property VoucherId As String          ' DV Number
   Public Property CheckNumber As String
   Public Property Particulars As String
   Public Property PayeeId As Integer
   Public Property PayeeBankName As String
   Public Property PayeeBankAccountNumber As String
   Public Property DisburserId As Integer
   Public Property PostUserId As Integer
   Public Property TotalAmount As Decimal
   Public Property TaxAmount As Decimal
   Public Property LockId As String
   Public Property ORSNumber As String              ' used by logs
   Public Property PayeeName As String              ' used by logs
   Public Property DisburserName As String          ' used by logs

End Class
