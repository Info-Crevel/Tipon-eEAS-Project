Public Class DbsCDATraining
    Public Property CDATrainingId As Integer
    Public Property CDATrainingName As String
    Public Property UploadRequiredFlag As Boolean
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsCDATraining
    Inherits DataSource(Of DbsCDATraining)

End Class