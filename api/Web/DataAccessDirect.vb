Friend Class DataAccessDirect
   Inherits DataAccess

#Region " Variables "

   Private _command As DbCommand
   Private _outputParameterCount As Integer

#End Region

#Region " Constructor "

   Friend Sub New(databaseId As String)
      MyBase.New(databaseId)
   End Sub

#End Region

#Region " Methods "

   Friend Function ExecuteScalar(commandType As CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As Object

      Dim _returnValue As Object = Me.Execute(ExecutionType.Scalar, commandType, commandText, timeout, parameters)
      '
      ' Test for existing output parameters and marshal return value as a DataTable 
      ' for parsing by SqlDirect (SD); otherwise, simply use the default return value
      '
      With Me
         If _command.Parameters.Count > 0 AndAlso _outputParameterCount > 0 Then
            Dim _outputParameterTable As New DataTable

            '
            ' Add ParameterName and Value columns
            '
            With _outputParameterTable.Columns
               .Add(Tags.ParameterColumnParameterNameAlias, GetType(String))
               .Add(Tags.ParameterColumnValueAlias, GetType(Object))
            End With

            '
            ' Use first row as placeholder for default return value
            '
            Dim _dataRow As DataRow = _outputParameterTable.NewRow
            With _dataRow
               .Item(Tags.ParameterColumnParameterNameAlias) = "@RV"
               .Item(Tags.ParameterColumnValueAlias) = _returnValue
            End With

            _outputParameterTable.Rows.Add(_dataRow)

            For Each _parameter As DbParameter In _command.Parameters

               Select Case _parameter.Direction
                  Case ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue

                     _dataRow = _outputParameterTable.NewRow

                     With _dataRow
                        .Item(Tags.ParameterColumnParameterNameAlias) = _parameter.ParameterName
                        .Item(Tags.ParameterColumnValueAlias) = _parameter.Value
                     End With

                     _outputParameterTable.Rows.Add(_dataRow)

               End Select
            Next

            Return _outputParameterTable

         Else
            Return _returnValue
         End If

      End With

   End Function

   'Friend Function ExecuteNonQuery(commandType As System.Data.CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As Integer
   '   Return CInt(Me.Execute(ExecutionType.NonQuery, commandType, commandText, timeout, parameters))
   'End Function

   Friend Function ExecuteNonQuery(commandType As System.Data.CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As Object

      Dim _returnValue As Object = Me.Execute(ExecutionType.NonQuery, commandType, commandText, timeout, parameters)

      ' Test for existing output parameters and marshal return value as a DataTable 
      ' for parsing by SqlDirect (SD); otherwise, simply use the default return value
      '
      With Me
         If _command.Parameters.Count > 0 AndAlso _outputParameterCount > 0 Then
            Dim _outputParameterTable As New DataTable
            _outputParameterTable.TableName = "OutParams"
            '
            ' Add ParameterName and Value columns
            '
            With _outputParameterTable.Columns
               .Add(Tags.ParameterColumnParameterNameAlias, GetType(String))
               .Add(Tags.ParameterColumnValueAlias, GetType(Object))
            End With

            '
            ' Use first row as placeholder for default return value
            '
            Dim _dataRow As DataRow = _outputParameterTable.NewRow
            With _dataRow
               .Item(Tags.ParameterColumnParameterNameAlias) = "@RV"
               .Item(Tags.ParameterColumnValueAlias) = _returnValue
            End With

            _outputParameterTable.Rows.Add(_dataRow)

            For Each _parameter As DbParameter In _command.Parameters

               Select Case _parameter.Direction
                  Case ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue

                     _dataRow = _outputParameterTable.NewRow

                     With _dataRow
                        .Item(Tags.ParameterColumnParameterNameAlias) = _parameter.ParameterName
                        .Item(Tags.ParameterColumnValueAlias) = _parameter.Value
                     End With

                     _outputParameterTable.Rows.Add(_dataRow)

               End Select
            Next

            Return _outputParameterTable

         Else
            Return _returnValue
         End If

      End With

   End Function

   Friend Function ExecuteDataTable(commandType As System.Data.CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As System.Data.DataTable
      Return CType(Me.Execute(ExecutionType.DataTable, commandType, commandText, timeout, parameters), DataTable)
   End Function

   Friend Function ExecuteDataSet(commandType As System.Data.CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As System.Data.DataSet
      Return CType(Me.Execute(ExecutionType.DataSet, commandType, commandText, timeout, parameters), DataSet)
   End Function

   Friend Function ExecuteReader(commandType As System.Data.CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As DbDataReader
      Return CType(Me.Execute(ExecutionType.DataReader, commandType, commandText, timeout, parameters), DbDataReader)
   End Function

#End Region

#Region " Helpers "

   Private Function Execute(executionType As ExecutionType, commandType As CommandType, commandText As String, timeout As Integer, parameters As QueryParameterCollection) As Object

      Dim _result As Object = Nothing
      _command = Me.CreateCommand(commandType, commandText)

      Try
         _command.CommandTimeout = timeout
         _outputParameterCount = 0

         Me.AttachParameters(parameters)
         Me.OpenConnection()

         Try
            Select Case executionType
               Case ExecutionType.Scalar
                  _result = _command.ExecuteScalar()

               Case ExecutionType.NonQuery
                  _result = _command.ExecuteNonQuery()

               Case ExecutionType.DataTable

                  Dim _dataTable As New DataTable
                  Dim _dataAdapter As DbDataAdapter = Me.ProviderFactory.CreateDataAdapter()

                  With _dataAdapter
                     .SelectCommand = _command
                     .Fill(_dataTable)
                     .Dispose()
                  End With

                  _result = _dataTable

               Case ExecutionType.DataSet

                  Dim _dataSet As New DataSet
                  Dim _dataAdapter As DbDataAdapter = Me.ProviderFactory.CreateDataAdapter()

                  With _dataAdapter
                     .SelectCommand = _command
                     .Fill(_dataSet)
                     .Dispose()
                  End With

                  _result = _dataSet

               Case ExecutionType.DataReader
                  _result = _command.ExecuteReader(CommandBehavior.CloseConnection)

            End Select

         Catch _exception As DataException
            Throw New DataException(_exception.Message + Tags.NewLine2 + _command.CommandText)

         Catch _exception As DbException
            Throw New Exception(_exception.Message + Tags.NewLine2 + _command.CommandText)

         Finally
            If executionType <> ExecutionType.DataReader Then
               Me.CloseConnection()
            End If

         End Try

      Catch _exception As DbException
         Throw New Exception(_exception.Message + Tags.NewLine2 + _command.CommandText)

      End Try

      Return _result

   End Function

   Private Sub AttachParameters(parameters As QueryParameterCollection)

      For Each _parameter As QueryParameter In parameters
         With _parameter
            Select Case .Direction
               Case ParameterDirection.Input
               Case Else
                  '_outputParameterCount = _outputParameterCount + 1
                  _outputParameterCount += 1

            End Select

            Me.AddParameter(_parameter)

         End With
      Next

   End Sub

   'Private Sub AddParameter(p As QueryParameter)

   '   Dim _parameter As DbParameter

   '   Select Case p.DbType
   '      Case DbType.Time
   '         _parameter = New SqlClient.SqlParameter(p.ParameterName, SqlDbType.Time)

   '      Case Else
   '         _parameter = _command.CreateParameter
   '         _parameter.DbType = p.DbType

   '   End Select

   '   With _parameter
   '      .ParameterName = p.ParameterName
   '      .Size = p.Size
   '      .Direction = p.Direction
   '      .Value = p.Value
   '      .SourceColumn = p.ColumnName
   '   End With

   '   _command.Parameters.Add(_parameter)

   'End Sub

   Private Sub AddParameter(p As QueryParameter)

      Dim _parameter As DbParameter

      If String.IsNullOrEmpty(p.TypeName) Then
         Select Case p.DbType
            Case DbType.Time
               _parameter = New SqlClient.SqlParameter(p.ParameterName, SqlDbType.Time)

            Case Else
               _parameter = _command.CreateParameter
               _parameter.DbType = p.DbType

         End Select

         With _parameter
            .ParameterName = p.ParameterName
            .Size = p.Size
            .Direction = p.Direction
            .Value = p.Value
            .SourceColumn = p.ColumnName
         End With
      Else
         _parameter = New SqlParameter(p.ParameterName, SqlDbType.Structured)
         _parameter.Value = p.Value
         DirectCast(_parameter, SqlParameter).TypeName = p.TypeName
      End If

      _command.Parameters.Add(_parameter)

   End Sub

#End Region

End Class
