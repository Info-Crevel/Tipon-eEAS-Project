Public Class BgsObx
   Public Property ObxId As Integer
   Public Property ObxDate As Date
   Public Property FundClusterId As String
   Public Property ObxStatusId As Integer             ' 12 Oct 2023 - EMT
   Public Property DocumentId As String
   Public Property PayeeId As Integer
   'Public Property PayeeName As String
   'Public Property PayeeOffice As String
   'Public Property PayeeAddress As String
   'Public Property PayeeBankName As String
   'Public Property PayeeBankAccountNumber As String
   Public Property Particulars As String
   Public Property Remarks As String
   Public Property PostUserId As Integer
   Public Property RequestSignatoryId As Integer
   Public Property SignatoryId As Integer
   'Public Property BudgetClassId As Integer        ' 04 Jul 2023 - EMT (removed)
   'Public Property ApprovedFlag As Boolean         ' 12 Oct 2023 - EMT (removed)
   Public Property DisburseFlag As Boolean         ' 24 Mar 2024 - EMT
   Public Property AdjustmentId As String          ' 12 Oct 2023 - EMT
   Public Property AdjustObxId As Integer          ' 12 Oct 2023 - EMT
   Public Property LockId As String
   Public Property PayeeName As String             ' used by logs

End Class
