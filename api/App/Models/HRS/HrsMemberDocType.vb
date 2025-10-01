Public Class HrsMemberDocType
    Public Property DocTypeDetailId As Integer
    Public Property DocTypeId As Integer
    Public Property ExpirationDate As Date
    Public Property MemberId As Integer
    Public Property DocTypeReference As String
    Public Property DocTypeFileName As String
    Public Property DocTypeGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberDocTypeList
    Inherits List(Of HrsMemberDocType)

End Class

