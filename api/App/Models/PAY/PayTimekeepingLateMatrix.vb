Public Class PayTimekeepingLateMatrix
   Public Property LateMatrixId As Integer
   Public Property TimekeepingId As Integer
   Public Property StartMinute As Integer
   Public Property EndMinute As Integer
   Public Property LateCount As Integer
   Public Property LogActionId As Integer
   Public Property LockId As String

End Class

Public Class PayTimekeepingLateMatrixList
   Inherits List(Of PayTimekeepingLateMatrix)

End Class

