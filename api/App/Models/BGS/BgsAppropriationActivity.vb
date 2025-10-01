Public Class BgsAppropriationActivity
   Public Property ApproActId As Integer
   Public Property AppropriationId As Integer
   Public Property ActivityId As Integer
   Public Property PSRAmount As Decimal
   Public Property RLIPAmount As Decimal
   Public Property MOOEAmount As Decimal
   Public Property FEAmount As Decimal
   Public Property COAmount As Decimal
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class BgsAppropriationActivityList
   Inherits List(Of BgsAppropriationActivity)

End Class
