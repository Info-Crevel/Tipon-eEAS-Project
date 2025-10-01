Public NotInheritable Class DataLib

#Region " Variables "

   Private Shared ReadOnly _connectionList As New Dictionary(Of String, ConnectionInfo)

#End Region

#Region " Constructor "

   Private Sub New()
      MyBase.New()
   End Sub

#End Region

#Region " Properties "

   Public Shared ReadOnly Property TypeString As Type = GetType(String)
   Public Shared ReadOnly Property TypeInt32 As Type = GetType(Int32)
   Public Shared ReadOnly Property TypeInt64 As Type = GetType(Int64)
   Public Shared ReadOnly Property TypeDecimal As Type = GetType(Decimal)
   Public Shared ReadOnly Property TypeDouble As Type = GetType(Double)
   Public Shared ReadOnly Property TypeDate As Type = GetType(Date)
   Public Shared ReadOnly Property TypeDateTime As Type = GetType(DateTime)
   Public Shared ReadOnly Property TypeBoolean As Type = GetType(Boolean)
   Public Shared ReadOnly Property TypeByte As Type = GetType(Byte)
   Public Shared ReadOnly Property TypeObject As Type = GetType(Object)
   Public Shared ReadOnly Property TypeTime As Type = GetType(TimeSpan)
   Public Shared ReadOnly Property TypeBinary As Type = GetType(Byte())

#End Region

#Region " Methods "

   Friend Shared Sub LoadConnectionDefinitions()

      Dim _xmlFileName As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin\Connection.xml")

      If Not File.Exists(_xmlFileName) Then
         Throw New FileNotFoundException("Connection setup file not found.", _xmlFileName)
      End If

      Dim _databaseId As String
      Dim _provider As DataProvider
      Dim _connectionString As String

      _connectionList.Clear()

      Using _dataSet As New DataSet
         _dataSet.ReadXml(_xmlFileName)
         For Each _row As DataRow In _dataSet.Tables(0).Rows
            _databaseId = _row.ToString("DatabaseId")

            Try
               _provider = CType(System.Enum.Parse(GetType(DataProvider), _row.ToString("ProviderId")), DataProvider)
            Catch _exception As Exception
               Throw _exception
            End Try

            '_connectionString = SysLib.Decrypt(_row.ToString("ConnectionString"), "Connection")
            _connectionString = _row.ToString("ConnectionString")

            _connectionList.Add(_databaseId, New ConnectionInfo(_provider, _connectionString))

         Next

      End Using

   End Sub

   'Friend Shared Function GetConnectionInfo(databaseId As String) As ConnectionInfo
   '   Return _connectionList(databaseId)
   'End Function

   Public Shared Function GetConnectionInfo(databaseId As String) As ConnectionInfo
      Return _connectionList(databaseId)
   End Function

   '
   ' Data type converters
   '
   Public Shared Function SystemToDbType(type As Type) As DbType

      Select Case type
         Case DataLib.TypeBoolean : Return DbType.Boolean
         Case DataLib.TypeDecimal : Return DbType.Decimal
         Case DataLib.TypeDouble : Return DbType.Double
         Case DataLib.TypeInt32 : Return DbType.Int32
         Case DataLib.TypeInt64 : Return DbType.Int64
         Case DataLib.TypeDate : Return DbType.Date
         Case DataLib.TypeDateTime : Return DbType.DateTime
         Case DataLib.TypeTime : Return DbType.Time
         Case Else : Return DbType.String
      End Select

   End Function

   '
   ' Utilities
   '
   Public Shared Function BuildUpdateCommandText(tableName As String, source As Object, keyFields As List(Of String)) As String
      Return DataLib.BuildUpdateCommandText(tableName, source, keyFields, New List(Of String))
   End Function

   Public Shared Function BuildUpdateCommandText(tableName As String, source As Object, keyFields As List(Of String), excludedFields As List(Of String)) As String

      Dim _propInfo() As PropertyInfo = source.GetType.GetProperties(BindingFlags.Public Or BindingFlags.IgnoreCase Or BindingFlags.Instance)
      Dim _builder As New Text.StringBuilder
      Dim _whereClause As String = String.Empty

      With _builder
         For Each _info As PropertyInfo In _propInfo
            If keyFields.Contains(_info.Name) Then
               If String.IsNullOrEmpty(_whereClause) Then
                  _whereClause = "WHERE " + _info.Name + "=@" + _info.Name
               Else
                  _whereClause = _whereClause + " AND " + _info.Name + "=@" + _info.Name
               End If
            Else
               If Not excludedFields.Contains(_info.Name) Then
                  If .Length = 0 Then
                     .Append(_info.Name + "=@" + _info.Name)
                  Else
                     .Append("," + _info.Name + "=@" + _info.Name)
                  End If
               End If
            End If
         Next

         .Insert(0, "UPDATE " + tableName + " SET ")
      End With

      If String.IsNullOrEmpty(_whereClause) Then
         _whereClause = "WHERE 1=0"
      End If

      Return _builder.ToString() + " " + _whereClause

   End Function

   Public Shared Function BuildInsertCommandText(tableName As String, source As Object, excludedFields As List(Of String)) As String

      Dim _propInfo() As PropertyInfo = source.GetType.GetProperties(BindingFlags.Public Or BindingFlags.IgnoreCase Or BindingFlags.Instance)
      Dim _columnList As String = String.Empty
      Dim _valueList As String = String.Empty

      For Each _info As PropertyInfo In _propInfo
         If Not excludedFields.Contains(_info.Name) Then
            If String.IsNullOrEmpty(_columnList) Then
               _columnList = _info.Name
               _valueList = "@" + _info.Name
            Else
               _columnList = _columnList + ", " + _info.Name
               _valueList = _valueList + ", @" + _info.Name
            End If
         End If

      Next

      Return "INSERT INTO " + tableName + " " + _columnList.Enclose() + " VALUES " + _valueList.Enclose

   End Function

   Public Shared Function BuildDeleteCommandText(tableName As String, keyFields As List(Of String)) As String

      Dim _whereClause As String = String.Empty

      For Each _columnName As String In keyFields
         If String.IsNullOrEmpty(_whereClause) Then
            _whereClause = "WHERE " + _columnName + "=@" + _columnName
         Else
            _whereClause = _whereClause + " AND " + _columnName + "=@" + _columnName
         End If
      Next

      If String.IsNullOrEmpty(_whereClause) Then
         _whereClause = "WHERE 1=0"
      End If

      Return "DELETE " + tableName + " " + _whereClause

   End Function

   Public Shared Sub ScatterValues(source As Object, target As Object)

      Dim _sourceInfo() As PropertyInfo = source.GetType.GetProperties(BindingFlags.Public Or BindingFlags.IgnoreCase Or BindingFlags.Instance)
      Dim _targetPropInfo As PropertyInfo

      For Each _info As PropertyInfo In _sourceInfo
         _targetPropInfo = target.GetType.GetProperty(_info.Name)
         If _targetPropInfo IsNot Nothing Then
            _targetPropInfo.SetValue(target, _info.GetValue(source, Nothing), Nothing)
         End If
      Next

   End Sub

   Public Shared Function CanDeleteRecord(tableName As String, id As String) As Boolean

      Dim _canDeleteFlag As Boolean

      Using _direct As New SqlDirect("web.SysGetFKStr")
         With _direct
            .AddParameter("TableName", tableName)
            .AddParameter("RowId", id)
            Using _table As DataTable = .ExecuteDataTable()
               _canDeleteFlag = _table.Rows.Count = 0
            End Using
         End With
      End Using

      Return _canDeleteFlag

   End Function

   Public Shared Function GetRecordCount(dataSourceName As String, filter As String, databaseId As String) As Integer

      Dim _count As Integer

      Using _direct As New SqlDirect("web.SysGetRecordCount", databaseId)
         With _direct
            .AddParameter("DataSourceName", dataSourceName)
            .AddParameter("Filter", filter)

            _count = CInt(.ExecuteScalar)
         End With
      End Using

      Return _count

   End Function

   Public Shared Function GetList(dataSourceName As String, fields As String, filter As String, sort As String) As DataTable

      Using _direct As New SqlDirect("web.SysGetList")
         With _direct
            .AddParameter("DataSource", dataSourceName)
            .AddParameter("Fields", fields)
            .AddParameter("Filter", filter.ToNullable)
            .AddParameter("Sort", sort.ToNullable)

            Return .ExecuteDataTable()
         End With
      End Using

   End Function

#End Region

End Class
