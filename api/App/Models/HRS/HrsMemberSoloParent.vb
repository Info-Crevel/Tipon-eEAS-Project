Public Class HrsMemberSoloParent
    Public Property MemberId As Integer
    Public Property SoloParentFlag As Boolean
    'Public Property SoloParentGUID As String
    'Public Property SoloParentImage As Byte
    'Public Property PageCount As Integer
    ''Public Property UploadFlag As Boolean
    'Public Property FileName As String
    'Public Property WorkStartDate As DateTime
    'Public Property WorkEndDate As DateTime
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberSoloParentList
    Inherits List(Of HrsMemberSoloParent)

End Class