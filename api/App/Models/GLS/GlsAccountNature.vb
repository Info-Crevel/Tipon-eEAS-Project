Public Class GlsAccountNature
   Public Property AccountNatureId As Integer
   Public Property AccountNatureName As String

End Class

Public Class QGlsAccountNature
   Inherits DataSource(Of GlsAccountNature)

End Class

