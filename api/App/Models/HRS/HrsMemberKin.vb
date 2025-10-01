Public Class HrsMemberKin
    Public Property KinDetailId As Integer
    Public Property MemberId As Integer
   Public Property KinLastName As String
   Public Property KinFirstName As String
   Public Property KinMiddleName As String
   Public Property KinSuffixId As Integer
   Public Property EmergencyContactFlag As Boolean
   Public Property RelationId As Integer
    Public Property KinPhoneNumber As String
    Public Property KinOccupation As String
    Public Property KinEmail As String
    Public Property KinFileName As String
    Public Property KinGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberKinList
    Inherits List(Of HrsMemberKin)

End Class
