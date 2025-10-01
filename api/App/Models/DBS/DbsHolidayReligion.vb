Public Class DbsHolidayReligion
    Public Property HolidayReligionDetailId As Integer
    Public Property HolidayId As Integer
    Public Property ReligionId As Integer
    Public Property LogActionId As Integer
    Public Property LockId As String

End Class

Public Class DbsHolidayReligionList
    Inherits List(Of DbsHolidayReligion)

End Class
