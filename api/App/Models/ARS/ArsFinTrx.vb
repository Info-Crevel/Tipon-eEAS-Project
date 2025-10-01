Public Class ArsFinTrx
   Public Property TrxId As Integer
   Public Property TrxDate As Date
   Public Property TrxStatusId As Integer
   Public Property DocumentId As String
   Public Property Reference As String
   Public Property Particulars As String
   Public Property Remarks As String               ' 01 Oct 2023 - EMT
   Public Property CashReceiptTypeId As Integer
   Public Property DisbursementTypeId As Integer
   Public Property BankId As Integer
   Public Property PostUserId As Integer
   Public Property TrxTypeId As Integer
   Public Property ClientId As Integer
   Public Property Amount As Decimal
   'Public Property PayableTypeId As Integer
   'Public Property PayableTaxCode As Integer
   'Public Property ATaxCode As String
   ''Public Property Details As String
   'Public Property DueDate As Date
   'Public Property CheckDate As Date
   'Public Property Amount As Decimal
   Public Property ArsLockId As String
   Public Property LockId As String

End Class
