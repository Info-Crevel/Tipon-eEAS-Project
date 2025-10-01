Public Class HrsMemberAffiliation
    Public Property AffiliationDetailId As Integer
    Public Property MemberId As Integer
    Public Property AffiliationId As Integer
    Public Property AffiliationDate As Date
    Public Property AffiliationPosition As String
    Public Property AffiliationDescription As String
    Public Property AffiliationFileName As String
    Public Property AffiliationGUID As String
    Public Property FileExtension As String
    Public Property FileUrl As String
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class HrsMemberAffiliationList
    Inherits List(Of HrsMemberAffiliation)

End Class
