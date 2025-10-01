Public Class DbList(Of T)

   Private _model As T

   Public Sub New()
      MyBase.New()

      _model = Activator.CreateInstance(Of T)()
   End Sub

   Public ReadOnly Property Rows As New ModelList(Of T)

   Public ReadOnly Property Config As New ListConfig

   Public ReadOnly Property FillInfo As New FillInfo

   Public Function IsEmpty() As Boolean
      Return Rows.Count = 0
   End Function

   Public Sub Clear()
      Rows.Clear()
   End Sub

   Public Function Fill() As Integer

      Me.Clear()

      Dim _selectCommandText As String = String.Empty
      Dim _columnList As String = String.Empty
      Dim _propertyInfo() As PropertyInfo = _model.GetType.GetProperties(BindingFlags.Public Or BindingFlags.IgnoreCase Or BindingFlags.Instance)
      Dim _whereClause As String = String.Empty

      If Me.Config.DataSourceName.ToUpperInvariant.Trim.StartsWith("SELECT ") Then
         _selectCommandText = Me.Config.DataSourceName
      ElseIf Me.FillInfo.DataSourceName.ToUpperInvariant.Trim.StartsWith("SELECT ") Then
         _selectCommandText = Me.FillInfo.DataSourceName
      Else
         If String.IsNullOrEmpty(_columnList) OrElse _propertyInfo Is Nothing Then
            For Each _info As PropertyInfo In _propertyInfo
               If _columnList Is String.Empty Then
                  _columnList = _info.Name
               Else
                  _columnList = _columnList + "," + _info.Name
               End If
            Next

            If String.IsNullOrEmpty(_columnList) Then
               Return 0
            End If
         End If

         Dim _builder As New StringBuilder

         If Me.FillInfo.Top > 0 Then
            _builder.Append("SELECT TOP " + Me.FillInfo.Top.ToString)
         Else
            _builder.Append("SELECT")
         End If

         Dim _dataSourceName As String

         If String.IsNullOrEmpty(Me.FillInfo.DataSourceName) Then
            _dataSourceName = Me.Config.DataSourceName
         Else
            _dataSourceName = Me.FillInfo.DataSourceName
         End If

         _builder.Append(" " + _columnList + " FROM " + _dataSourceName)

         If Me.Config.KeyColumns.Count > 0 Then
            For Each _columnName As String In Me.Config.KeyColumns.Keys
               If String.IsNullOrEmpty(_whereClause) Then
                  _whereClause = " WHERE " + _columnName + "=@" + _columnName
               Else
                  _whereClause = _whereClause + " AND " + _columnName + "=@" + _columnName
               End If
            Next
         End If

         If Not String.IsNullOrEmpty(Me.FillInfo.Filter) Then
            If String.IsNullOrEmpty(_whereClause) Then
               _builder.Append(" WHERE " + Me.FillInfo.Filter)
            Else
               _builder.Append(_whereClause + " AND " + Me.FillInfo.Filter)
            End If
         Else
            _builder.Append(_whereClause)
         End If

         If Not String.IsNullOrEmpty(Me.FillInfo.Sort) Then
            _builder.Append(" ORDER BY " + Me.FillInfo.Sort)
         End If

         _selectCommandText = _builder.ToString()
      End If

      Using _direct As New SqlDirect(_selectCommandText, CommandType.Text, Me.Config.DatabaseId)

         If Me.Config.KeyColumns.Count > 0 Then
            For Each _pair As KeyValuePair(Of String, Object) In Me.Config.KeyColumns
               With _direct.AddParameter(_pair.Key, DataLib.SystemToDbType(_pair.Value.GetType()))
                  .Value = _pair.Value
               End With
            Next
         End If

         Dim _reader As DbDataReader = _direct.ExecuteReader()

         If _reader Is Nothing Then
            Return 0
         End If

         Do While _reader.Read
            _model = DirectCast(Activator.CreateInstance(_model.GetType), T)

            For Each _info As PropertyInfo In _propertyInfo
               Select Case _info.PropertyType
                  Case DataLib.TypeBoolean : _info.SetValue(_model, _reader.ToBoolean(_info.Name))
                  Case DataLib.TypeDate : _info.SetValue(_model, _reader.ToDate(_info.Name))
                  Case DataLib.TypeDecimal : _info.SetValue(_model, _reader.ToDecimal(_info.Name))
                  Case DataLib.TypeDouble : _info.SetValue(_model, _reader.ToDouble(_info.Name))
                  Case DataLib.TypeInt32 : _info.SetValue(_model, _reader.ToInt32(_info.Name))
                  Case DataLib.TypeTime : _info.SetValue(_model, _reader.ToTime(_info.Name))
                  Case DataLib.TypeObject, DataLib.TypeBinary
                     _info.SetValue(_model, _reader.ToBase64(_info.Name))
                  Case Else
                     If _info.Name = "LockId" OrElse _info.Name.EndsWith("LockId") Then
                        _info.SetValue(_model, _reader.ToBase64(_info.Name))
                     Else
                        _info.SetValue(_model, _reader.ToString(_info.Name).Trim)
                     End If

               End Select
            Next

            Me.Rows.Add(_model)

         Loop

         _reader.Close()

      End Using

      Return Me.Rows.Count

   End Function

End Class
