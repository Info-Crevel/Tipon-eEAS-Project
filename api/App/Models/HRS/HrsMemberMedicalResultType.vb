Public Class HrsMemberMedicalResultType
    Public Property MedicalResultTypeDetailId As Integer
    Public Property MemberId As Integer
    Public Property MedicalResultTypeId As Integer
    Public Property Remarks As String
    Public Property MedicalResultTypeFileName As String
    Public Property MedicalResultTypeGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberMedicalResultTypeList
    Inherits List(Of HrsMemberMedicalResultType)

End Class
