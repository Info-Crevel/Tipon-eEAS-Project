Public Class DbsInventoryCharge
    Public Property InventoryChargeId As Integer
    Public Property InventoryChargeName As String
    Public Property ClientFlag As Boolean
    Public Property MemberFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class


Public Class QDbsInventoryCharge
    Inherits DataSource(Of DbsInventoryCharge)
End Class