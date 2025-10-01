Public Class DbsChargingConsideration
    Public Property ChargingConsiderationId As Integer
    Public Property ChargingConsiderationName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsChargingConsideration
    Inherits DataSource(Of DbsChargingConsideration)

End Class