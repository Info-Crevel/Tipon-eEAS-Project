Public Class DbsMedicalResultType
    Public Property MedicalResultTypeId As Integer
    Public Property MedicalResultTypeCode As String
    Public Property MedicalResultTypeName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsbsMedicalResultType
    Inherits DataSource(Of DbsMedicalResultType)
End Class