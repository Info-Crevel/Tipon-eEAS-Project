Public Class DbsContractType
    Public Property ContractTypeId As Integer
    Public Property ContractTypeName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsContractType
    Inherits DataSource(Of DbsContractType)

End Class