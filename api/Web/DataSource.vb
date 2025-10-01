Public MustInherit Class DataSource(Of T)

   Private ReadOnly _dbList As DbList(Of T)

   Public Sub New()
      MyBase.New()
      _dbList = New DbList(Of T)
   End Sub

   Public ReadOnly Property FillInfo As FillInfo
      Get
         Return _dbList.FillInfo
      End Get
   End Property

   Public ReadOnly Property Config As ListConfig
      Get
         Return _dbList.Config
      End Get
   End Property

   Public ReadOnly Property Rows As List(Of T)
      Get
         Return _dbList.Rows
      End Get
   End Property

   Public Function Fill() As Integer
      Return _dbList.Fill()
   End Function

   Public Sub Clear()
      _dbList.Clear()
   End Sub

   Public Function IsEmpty() As Boolean
      Return _dbList.Rows.Count = 0
   End Function

End Class
