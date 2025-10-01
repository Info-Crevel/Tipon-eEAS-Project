Public Class HrsMemberControl
    Public Property MemberControlId As Integer
    Public Property SssUploadRequiredFlag As Boolean
    Public Property PbgUploadRequiredFlag As Boolean
    Public Property PhhUploadRequiredFlag As Boolean
    Public Property WhtUploadRequiredFlag As Boolean
    Public Property SoloParentUploadRequiredFlag As Boolean
    Public Property PwdUploadRequiredFlag As Boolean
   Public Property MinimumAge As Integer
   Public Property PoolingValidationPeriod As Integer
   Public Property BasicPayTrxId As Integer
   Public Property PbgPayTrxId As Integer
   Public Property PhhPayTrxId As Integer
   Public Property SSSPayTrxId As Integer
   Public Property WhtPayTrxId As Integer

   Public Property DeminimisPayTrxId As Integer
   Public Property AllowancePayTrxId As Integer
   Public Property SeparationPayTrxId As Integer
   Public Property YearEndPayTrxId As Integer
   Public Property SickLeavePayTrxId As Integer
   Public Property FallOutDays As Integer
   Public Property BackOutDays As Integer
   Public Property LockId As String

End Class


Public Class QHrsMemberControl
    Inherits DataSource(Of HrsMemberControl)
End Class