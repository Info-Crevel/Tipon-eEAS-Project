Public Class DbsNCIIQualificationTitle
    Public Property NCIIQualificationTitleId As Integer
    Public Property NCIIQualificationTitleName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsNCIIQualificationTitle
    Inherits DataSource(Of DbsNCIIQualificationTitle)

End Class