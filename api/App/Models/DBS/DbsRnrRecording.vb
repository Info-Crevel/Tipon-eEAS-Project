Public Class DbsRnrRecording
    Public Property RnrRecordingId As Integer
    Public Property RnrRecordingName As String
    Public Property SortSeq As Integer
    Public Property LockId As String

End Class

Public Class QDbsRnrRecording
    Inherits DataSource(Of DbsRnrRecording)

End Class