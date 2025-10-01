Public Class DateOnlyConverter
   Inherits IsoDateTimeConverter

   Public Sub New()
      Me.DateTimeFormat = "yyyy-MM-dd"
   End Sub

End Class
