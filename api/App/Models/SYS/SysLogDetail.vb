Public Class SysLogDetail           ' represents a row in XXXLogDetail table

   Public Property Id As Integer
   Public Property SeqId As Integer
   Public Property ColumnId As LogColumnId
   Public Property OldValue As String
   Public Property NewValue As String
   Public Property LogReference As String

End Class

Public Class SysLogDetailList       ' collection of SysLogDetail rows
   Inherits List(Of SysLogDetail)

End Class
