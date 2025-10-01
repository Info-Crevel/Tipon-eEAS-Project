Public Class DbsCDAMemberType
    Public Property CDAMemberTypeId As Integer
   Public Property CDAMemberTypeName As String
   Public Property CDAMemberTypeAmount As Decimal
   Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsCDAMemberType
    Inherits DataSource(Of DbsCDAMemberType)

End Class