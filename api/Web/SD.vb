Public NotInheritable Class SD

   Private Shared _direct As SqlDirect

#Region " Constructor "

   Private Sub New()
      MyBase.New()
   End Sub

#End Region

#Region " Open "

   Public Shared Sub Open(ByVal commandText As String)
      SD.Open(commandText, CommandType.StoredProcedure, Tags.SystemId)
   End Sub

   Public Shared Sub Open(commandText As String, commandType As CommandType)
      SD.Open(commandText, commandType, Tags.SystemId)
   End Sub

   Public Shared Sub Open(commandText As String, databaseId As String)
      SD.Open(commandText, CommandType.StoredProcedure, databaseId)
   End Sub

   Public Shared Sub Open(commandText As String, commandType As CommandType, databaseId As String)
      _direct = New SqlDirect(commandText, commandType, databaseId)
   End Sub

#End Region

#Region " Parameter-related "

   Public Shared Function GetParameterValue(parameterName As String) As Object
      Return _direct.GetParameterValue(parameterName)
   End Function

   Public Shared Sub SetParameterValue(parameterName As String, value As Object)
      _direct.SetParameterValue(parameterName, value)
   End Sub

   Public Shared Function AddParameter(parameterName As String) As DbParameter
      Return _direct.AddParameter(parameterName)
   End Function

   Public Shared Function AddParameter(parameterName As String, value As Object) As DbParameter
      Return _direct.AddParameter(parameterName, value)
   End Function

   Public Shared Function AddParameter(parameterName As String, size As Integer, direction As ParameterDirection) As DbParameter
      Return _direct.AddParameter(parameterName, size, direction)
   End Function

#End Region

#Region " ExecuteScalar "

   Public Shared Function ExecuteScalar() As Object
      Return _direct.ExecuteScalar()
   End Function

   Public Shared Function ExecuteScalar(timeout As Integer) As Object
      Return _direct.ExecuteScalar(timeout)
   End Function

#End Region

#Region " ExecuteNonQuery "

   Public Shared Function ExecuteNonQuery() As Integer
      Return _direct.ExecuteNonQuery()
   End Function

   Public Shared Function ExecuteNonQuery(timeout As Integer) As Integer
      Return _direct.ExecuteNonQuery(timeout)
   End Function

#End Region

#Region " ExecuteDataTable "

   Public Shared Function ExecuteDataTable() As DataTable
      Return _direct.ExecuteDataTable()
   End Function

   Public Shared Function ExecuteDataTable(timeout As Integer) As DataTable
      Return _direct.ExecuteDataTable(timeout)
   End Function

#End Region

#Region " ExecuteDataSet "

   Public Shared Function ExecuteDataSet() As DataSet
      Return _direct.ExecuteDataSet()
   End Function

   Public Shared Function ExecuteDataSet(timeout As Integer) As DataSet
      Return _direct.ExecuteDataSet(timeout)
   End Function

#End Region

#Region " HasRecords "

   Public Shared Function HasRecords(dataSourceName As String) As Boolean
      Return SD.HasRecords(dataSourceName, String.Empty)
   End Function

   Public Shared Function HasRecords(dataSourceName As String, filter As String) As Boolean
      Return SD.HasRecords(dataSourceName, filter, Tags.SystemId)
   End Function

   Public Shared Function HasRecords(dataSourceName As String, filter As String, databaseId As String) As Boolean
      Return (SD.GetRecordCount(dataSourceName, filter, databaseId) > 0)
   End Function

#End Region

#Region " GetRecordCount "

   Public Shared Function GetRecordCount(dataSourceName As String) As Integer
      Return SD.GetRecordCount(dataSourceName, String.Empty)
   End Function

   Public Shared Function GetRecordCount(dataSourceName As String, filter As String) As Integer
      Return SD.GetRecordCount(dataSourceName, filter, Tags.SystemId)
   End Function

   Public Shared Function GetRecordCount(dataSourceName As String, filter As String, databaseId As String) As Integer

      If dataSourceName Is Nothing Then
         Throw New ArgumentNullException("dataSourceName")
      End If

      Dim _filter As String = filter

      If String.IsNullOrEmpty(filter) Then
         _filter = String.Empty
      End If

      Dim _count As Integer

      Using _direct As New SqlDirect("dbo.SysGetRecordCount", databaseId)
         With _direct
            .AddParameter("DataSourceName", dataSourceName)
            .AddParameter("Filter", _filter)

            _count = CInt(.ExecuteScalar)
         End With
      End Using

      'SD.Open("dbo.SysGetRecordCount", databaseId)
      'SD.AddParameter("DataSourceName", dataSourceName)
      'SD.AddParameter("Filter", _filter)

      '_count = CInt(SD.ExecuteScalar)

      'SD.Close()

      Return _count

   End Function

#End Region

   Public Shared Sub Close()
      If _direct IsNot Nothing Then
         _direct.Dispose()
      End If
      _direct = Nothing
   End Sub

End Class
