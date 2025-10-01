Public Class QueryParameter

   Public Property ColumnName As String
   'Public CommandActions As CommandActions
   Public Property DbType As System.Data.DbType
   Public Property ParameterName As String
   Public Property Size As Integer
   Public Property Direction As ParameterDirection
   Public Property Value As Object
   Public Property TypeName As String        ' for SqlDbType.Structured

End Class

Friend Class QueryParameterCollection
   Inherits KeyedCollection(Of String, QueryParameter)

   Protected Overrides Function GetKeyForItem(item As QueryParameter) As String
      Return item.ColumnName
   End Function

End Class
