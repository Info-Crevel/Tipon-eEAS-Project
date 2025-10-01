Public Class FillInfo

   Public Property DataSourceName As String = String.Empty
   Public Property Top As Integer
   Public Property Filter As String
   Public Property Sort As String

   Public Sub Clear()
      With Me
         .Top = 0
         .Filter = String.Empty
         .Sort = String.Empty
      End With
   End Sub

End Class
