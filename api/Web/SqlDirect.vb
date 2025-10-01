Public NotInheritable Class SqlDirect
   Implements IDisposable

#Region " Variables "

   Private _dataLayer As DataAccessDirect

   Private ReadOnly _commandType As CommandType = CommandType.StoredProcedure
   Private ReadOnly _commandText As String
   Private ReadOnly _dbParameters As New DbParameterList
   Private ReadOnly _parameters As New QueryParameterCollection

   Private _isDisposed As Boolean

#End Region

#Region " Constructor "

   Public Sub New(commandText As String)
      Me.New(commandText, CommandType.StoredProcedure, Tags.SystemId)
   End Sub

   Public Sub New(commandText As String, commandType As CommandType)
      Me.New(commandText, commandType, Tags.SystemId)
   End Sub

   Public Sub New(commandText As String, databaseId As String)
      Me.New(commandText, CommandType.StoredProcedure, databaseId)
   End Sub

   Public Sub New(commandText As String, commandType As CommandType, databaseId As String)
      MyBase.New()

      _commandText = commandText
      _commandType = commandType

      _dataLayer = New DataAccessDirect(databaseId)
      Me.ClearParameters()

   End Sub

#End Region

#Region " Methods "

   Public Function GetParameterValue(parameterName As String) As Object
      Return Me.GetParameter(parameterName).Value
   End Function

   Friend Sub SetParameterValue(parameterName As String, value As Object)
      Me.GetParameter(parameterName).Value = value
   End Sub

   Public Function AddParameter(parameterName As String, value As Object) As DbParameter

      Dim _parameter As DbParameter = Me.AddParameter(parameterName)
      _parameter.Value = value

      Return _parameter

   End Function

   Public Function AddParameter(parameterName As String, dbType As DbType) As DbParameter
      Return Me.AddParameter(parameterName, dbType, ParameterDirection.Input)
   End Function

   Public Function AddParameter(parameterName As String, dbType As DbType, direction As ParameterDirection) As DbParameter

      If Not System.Enum.IsDefined(GetType(DbType), dbType) Then
         Throw New InvalidEnumArgumentException("dbType", dbType, GetType(DbType))
      End If

      Dim _parameter As DbParameter

      Select Case dbType
         Case System.Data.DbType.Time
            Dim sqlParameter As New SqlParameter("@" + parameterName, SqlDbType.Time)
            _parameter = sqlParameter
            _parameter.Direction = direction

            _dbParameters.Add(_parameter)

         Case Else
            _parameter = Me.AddParameter(parameterName)

            With _parameter
               .Direction = direction
               .DbType = dbType
            End With

      End Select

      Return _parameter

   End Function

   Public Function AddParameter(parameterName As String, size As Integer, direction As ParameterDirection) As DbParameter

      Dim _parameter As DbParameter = Me.AddParameter(parameterName)

      With _parameter
         .Size = size
         .Direction = direction
      End With

      Return _parameter

   End Function

   Public Function AddParameter(parameterName As String) As DbParameter

      Dim _parameter As DbParameter = Me.CreateParameter(parameterName)

      _parameter.DbType = DbType.String
      _dbParameters.Add(_parameter)

      Return _parameter

   End Function

#End Region

#Region " ExecuteScalar "

   Public Function ExecuteScalar() As Object
      Return Me.ExecuteScalar(Tags.CommandTimeout)
   End Function

   'Public Function ExecuteScalar(timeout As Integer) As Object

   '   Me.BuildParameters()
   '   Dim _result As Object = Me.Execute(ExecutionType.Scalar, timeout)
   '   Me.BuildReturnValue(_result)
   '   Return _result

   'End Function

   Public Function ExecuteScalar(timeout As Integer) As Object

      'Me.BuildParameters()
      'Dim _result As Object = Me.Execute(ExecutionType.Scalar, timeout)
      'Me.BuildReturnValue(_result)

      'If _result.GetType.IsAssignableFrom(GetType(DataTable)) Then
      '   _result = CType(_result, DataTable).Rows(0).Item(Tags.ParameterColumnValueAlias)
      'End If

      'Return _result

      Me.BuildParameters()
      Dim _result As Object = Me.Execute(ExecutionType.Scalar, timeout)
      Me.BuildReturnValue(_result)

      If _result IsNot Nothing AndAlso _result.GetType.IsAssignableFrom(GetType(DataTable)) Then
         _result = CType(_result, DataTable).Rows(0).Item(Tags.ParameterColumnValueAlias)
      End If

      Return _result

   End Function

#End Region

#Region " ExecuteNonQuery "

   Public Function ExecuteNonQuery() As Integer
      Return Me.ExecuteNonQuery(Tags.CommandTimeout)
   End Function

   'Public Function ExecuteNonQuery(timeout As Integer) As Integer

   '   Me.BuildParameters()
   '   Return CInt(Me.Execute(ExecutionType.NonQuery, timeout))

   'End Function

   Public Function ExecuteNonQuery(timeout As Integer) As Integer

      'Me.BuildParameters()
      'Dim _result As Object = Me.Execute(ExecutionType.NonQuery, timeout)
      'Me.BuildReturnValue(_result)

      'If _result.GetType.IsAssignableFrom(GetType(DataTable)) Then
      '   _result = CType(_result, DataTable).Rows(0).Item(Tags.ParameterColumnValueAlias)
      'End If

      'If _result Is Nothing Then
      '   Return 0
      'End If

      'Return CInt(_result)

      Me.BuildParameters()
      Dim _result As Object = Me.Execute(ExecutionType.NonQuery, timeout)
      Me.BuildReturnValue(_result)

      If _result Is Nothing Then
         Return 0
      End If

      If _result.GetType.IsAssignableFrom(GetType(DataTable)) Then
         _result = CType(_result, DataTable).Rows(0).Item(Tags.ParameterColumnValueAlias)
      End If

      Return CInt(_result)

   End Function

#End Region

#Region " ExecuteDataTable "

   Public Function ExecuteDataTable() As DataTable
      Return Me.ExecuteDataTable(Tags.CommandTimeout)
   End Function

   Public Function ExecuteDataTable(timeout As Integer) As DataTable

      Me.BuildParameters()
      Dim _dataTable As DataTable = CType(Me.Execute(ExecutionType.DataTable, timeout), DataTable)
      Return _dataTable

   End Function

#End Region

#Region " ExecuteDataSet "

   Public Function ExecuteDataSet() As DataSet
      Return Me.ExecuteDataSet(Tags.CommandTimeout)
   End Function

   Public Function ExecuteDataSet(timeout As Integer) As DataSet

      Me.BuildParameters()
      Return CType(Me.Execute(ExecutionType.DataSet, timeout), DataSet)

   End Function

#End Region

#Region " ExecuteReader "

   Public Function ExecuteReader() As DbDataReader
      Return Me.ExecuteReader(Tags.CommandTimeout)
   End Function

   Public Function ExecuteReader(timeout As Integer) As DbDataReader

      Me.BuildParameters()
      Return CType(Me.Execute(ExecutionType.DataReader, timeout), DbDataReader)

   End Function

#End Region

#Region " Helpers "

   Private Function Execute(type As ExecutionType, timeout As Integer) As Object

      Dim _result As Object = Nothing

      '
      ' Execute the command
      '
      Try
         Select Case type
            Case ExecutionType.Scalar
               _result = _dataLayer.ExecuteScalar(_commandType, _commandText, timeout, _parameters)

            Case ExecutionType.NonQuery
               _result = _dataLayer.ExecuteNonQuery(_commandType, _commandText, timeout, _parameters)

            Case ExecutionType.DataTable
               _result = _dataLayer.ExecuteDataTable(_commandType, _commandText, timeout, _parameters)

            Case ExecutionType.DataSet
               _result = _dataLayer.ExecuteDataSet(_commandType, _commandText, timeout, _parameters)

            Case ExecutionType.DataReader
               _result = _dataLayer.ExecuteReader(_commandType, _commandText, timeout, _parameters)

         End Select

      Catch _exception As Exception
         Throw
      End Try

      Return _result

   End Function

   Private Function CreateParameter(parameterName As String) As DbParameter

      Dim _parameter As DbParameter = _dataLayer.ProviderFactory.CreateParameter
      _parameter.ParameterName = parameterName.ToParameterName(_dataLayer.Provider)
      Return _parameter

   End Function

   Private Function GetParameter(parameterName As String) As DbParameter

      Dim _parameterName As String = parameterName.ToParameterName(_dataLayer.Provider)
      Dim _parameter As DbParameter = Me.GetParameterByName(_parameterName)

      If _parameter Is Nothing Then
         Throw New ArgumentException("Parameter not found.", _parameterName)
      End If

      Return _parameter

   End Function

   Private Function GetParameterByName(parameterName As String) As DbParameter

      For Each _parameter As DbParameter In _dbParameters
         If _parameter.ParameterName = parameterName Then
            Return _parameter
         End If
      Next
      Return Nothing

   End Function

   Private Sub ClearParameters()
      _dbParameters.Clear()
   End Sub

   Private Sub BuildParameters()

      If _dbParameters.Count = 0 Then
         Return
      End If

      _parameters.Clear()

      For Each _dbParameter As DbParameter In _dbParameters
         With _dbParameter
            If TypeOf _dbParameter Is SqlParameter Then
               Me.AddQueryParameter(.ParameterName, .DbType, .Size, .Direction, .Value, DirectCast(_dbParameter, SqlParameter).TypeName)
            Else
               Me.AddQueryParameter(.ParameterName, .DbType, .Size, .Direction, .Value, String.Empty)
            End If
         End With
      Next

   End Sub

   'Private Sub AddQueryParameter(parameterName As String, dbType As DbType, size As Integer, direction As ParameterDirection, value As Object)
   Private Sub AddQueryParameter(parameterName As String, dbType As DbType, size As Integer, direction As ParameterDirection, value As Object, typeName As String)

      If _parameters.Contains(parameterName) Then
         Return
      End If

      Dim _queryParameter As New QueryParameter

      With _queryParameter
         '.CommandActions = CommandActions.Select
         .ParameterName = parameterName
         .ColumnName = parameterName.Substring(1)
         .DbType = dbType
         .Size = size
         .Direction = direction
         .TypeName = typeName

         If TypeOf value Is Date Then
            .Value = CType(value, Date).ToIsoFormat()
         Else
            .Value = value
         End If
      End With

      _parameters.Add(_queryParameter)

   End Sub

   Private Sub BuildReturnValue(result As Object)

      If result Is Nothing Then
         Return
      End If

      '
      ' Test the type of returned object after execution; Data Layer may return
      ' a DataTable if underlying command object has output parameter(s)
      '
      If result.GetType.IsAssignableFrom(GetType(DataTable)) Then
         '
         ' Set the value(s) of the output parameter(s)
         '
         Dim _dataRow As DataRow
         Dim _outputParameterTable As DataTable = CType(result, DataTable)

         Dim _rowCount As Integer = _outputParameterTable.Rows.Count - 1

         For _index As Integer = 1 To _rowCount
            _dataRow = _outputParameterTable.Rows(_index)

            With _dataRow
               Me.SetParameterValue(.Item(Tags.ParameterColumnParameterNameAlias).ToString, .Item(Tags.ParameterColumnValueAlias))
            End With

         Next

         '
         ' Remap the result to the first row (placeholder for return value)
         '
         result = _outputParameterTable.Rows(0).Item(Tags.ParameterColumnValueAlias)

      End If

   End Sub

#End Region

#Region " IDisposable Support "

   Private Sub Dispose(disposing As Boolean)
      If Not _isDisposed Then
         If disposing Then
            Me.ClearParameters()

            If _dataLayer IsNot Nothing Then
               CType(_dataLayer, DataAccess).Dispose()
               _dataLayer = Nothing
            End If
         End If
      End If
      _isDisposed = True
   End Sub

   Public Sub Dispose() Implements IDisposable.Dispose
      Me.Dispose(True)
      GC.SuppressFinalize(Me)
   End Sub

#End Region

End Class
