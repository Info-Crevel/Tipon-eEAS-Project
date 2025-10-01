Public Class GlsAccountType
   Public Property AccountTypeId As Integer
   Public Property AccountTypeName As String

End Class

Public Class QGlsAccountType
   Inherits DataSource(Of GlsAccountType)

End Class
