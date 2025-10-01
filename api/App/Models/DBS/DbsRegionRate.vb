Public Class DbsRegionRate
    Public Property RegionRateId As Integer
    Public Property RegionId As String
    Public Property StartDate As Date
    Public Property RateAmount As Decimal
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class DbsRegionRateList
    Inherits List(Of DbsRegionRate)

End Class

