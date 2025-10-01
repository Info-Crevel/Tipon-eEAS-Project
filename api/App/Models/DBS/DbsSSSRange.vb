Public Class DbsSSSRange
    Public Property SSSRangeId As Integer
    Public Property StartAmount As Decimal
    Public Property EndAmount As Decimal
    Public Property RangeOperatorId As Integer
    Public Property SSEC As Decimal
    Public Property WISP As Decimal
    Public Property SSER As Decimal
    Public Property SSEE As Decimal
    Public Property ECER As Decimal
    Public Property ECEE As Decimal
    Public Property WISPER As Decimal
    Public Property WISPEE As Decimal
    Public Property SortSeq As Decimal
    Public Property LockId As String

End Class

Public Class QDbsSSSRange
    Inherits DataSource(Of DbsSSSRange)
End Class