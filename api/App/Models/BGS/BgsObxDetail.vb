Public Class BgsObxDetail
   Public Property ObxDetailId As Integer
   Public Property ObxId As Integer
   Public Property CostCenterId As Integer
   'Public Property Particulars As String
   Public Property ActivityId As Integer
   Public Property BudgetClassId As Integer                    ' 04 Jul 2023 - EMT
   Public Property AllotmentId As Integer                      ' 12 Sep 2023 - EMT
   Public Property AccountId As String
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String
   Public Property CostCenterShortName As String              ' used by logs
   Public Property ActivityShortName As String                ' used by logs
   Public Property BudgetClassName As String                  ' used by logs

End Class

Public Class BgsObxDetailList
   Inherits List(Of BgsObxDetail)

End Class

