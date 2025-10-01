Public Class ArsMemberRequestDocType
   Public Property MemberRequestDocTypeDetailId As Integer
   Public Property MemberRequestId As Integer
   Public Property DocTypeId As Integer
   Public Property LogActionId As Integer

   Public Property LockId As String

End Class

Public Class ArsMemberRequestDocTypeList
   Inherits List(Of ArsMemberRequestDocType)

End Class

