Public Class BgsTransferTarget
   Public Property TransferTargetId As Integer
   Public Property TransferId As Integer
   Public Property AllotActId As Integer
   Public Property CostCenterId As Integer
   Public Property AccountId As String
   Public Property Amount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String
   'Public Property AllotmentName As String                    ' used by logs
   Public Property ActivityName As String                     ' used by logs
   Public Property CostCenterName As String                   ' used by logs

End Class

Public Class BgsTransferTargetList
   Inherits List(Of BgsTransferTarget)

End Class
