Public Class DbsDayType
    Public Property DayTypeId As Integer
    Public Property DayTypeCode As String
    Public Property DayTypeName As String
    Public Property PremiumPercentage As Decimal
    Public Property NightDifferentialFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsbsDayType
    Inherits DataSource(Of DbsDayType)
End Class