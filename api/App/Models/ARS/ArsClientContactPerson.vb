Public Class ArsClientContactPerson
    Public Property ContactDetailId As Integer
    Public Property ClientId As Integer
    Public Property ContactPerson As String
    Public Property ContactMobileNumber As String

    Public Property ContactEmail As String

    Public Property ContactPosition As String
    Public Property LockId As String
    Public Property LogActionId As Integer

End Class

Public Class ArsClientContactPersonList
    Inherits List(Of ArsClientContactPerson)

End Class
