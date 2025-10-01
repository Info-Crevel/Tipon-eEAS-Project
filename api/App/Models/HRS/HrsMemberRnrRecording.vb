Public Class HrsMemberRnrRecording
    Public Property RnrRecordingDetailId As Integer
    Public Property MemberId As Integer
    Public Property RnrRecordingId As Integer
    'Public Property RnrRecordingName As String
    Public Property RnrNumberId As Integer
    Public Property RnrNumber As Decimal
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberRnrRecordingList
    Inherits List(Of HrsMemberRnrRecording)

End Class
