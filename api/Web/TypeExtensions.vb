<Extension()>
Public Module TypeExtensions

#Region " String "

   <Extension()>
   Public Function ToNullable(value As String) As Object
      If String.IsNullOrEmpty(value) Then
         Return DBNull.Value
      Else
         Return value
      End If
   End Function

   <Extension()>
   Public Function ToParameterName(parameterName As String, provider As DataProvider) As String

      If parameterName Is Nothing Then
         Throw New System.ArgumentNullException("parameterName")
      End If

      Dim _marker As Char = TypeExtensions.GetParameterMarker(provider)

      If parameterName.StartsWith(_marker, StringComparison.Ordinal) Then
         Return parameterName
      Else
         Return _marker + parameterName
      End If

   End Function

   <Extension()>
   Public Function Enclose(value As String) As String
      Return TypeExtensions.Enclose(value, EncloseCharacter.Parenthesis)
   End Function

   <Extension()>
   Public Function Enclose(value As String, character As EncloseCharacter) As String
      If value Is Nothing Then
         Throw New ArgumentNullException("value")
      End If

      If character < EncloseCharacter.Parenthesis OrElse character > EncloseCharacter.Asterisk Then
         Throw New ArgumentOutOfRangeException("character")    ' 01 Aug 2014 - EMT
      End If

      Select Case character
         Case EncloseCharacter.Parenthesis
            Return "(" + value + ")"

         Case EncloseCharacter.Brace
            Return "[" + value + "]"

         Case EncloseCharacter.CurlyBrace
            Return "{" + value + "}"

         Case EncloseCharacter.DoubleQuote
            Return """" + value + """"

         Case EncloseCharacter.LessGreaterThan
            Return "<" + value + ">"

         Case EncloseCharacter.Quote
            Return "'" + value + "'"

         Case EncloseCharacter.Space
            Return " " + value + " "

         Case EncloseCharacter.Asterisk
            Return "*" + value + "*"

         Case Else
            Return "[" + value + "]"

      End Select

   End Function

#End Region

#Region " Integer "

   <Extension()>
   Public Function ToNullable(value As Integer) As Object
      If value = 0 Then
         Return DBNull.Value
      Else
         Return value
      End If
   End Function

   <Extension()>
   Public Function Between(value As Integer, beginRange As Integer, endRange As Integer) As Boolean
      Return value >= beginRange AndAlso value <= endRange
   End Function

   <Extension()>
   Public Function Enclose(value As Integer) As String
      Return TypeExtensions.Enclose(value, EncloseCharacter.Parenthesis)
   End Function

   <Extension()>
   Public Function Enclose(value As Integer, character As EncloseCharacter) As String
      Return value.ToString(CultureInfo.InvariantCulture).Enclose(character)
   End Function

#End Region

#Region " Date "

   <Extension()>
   Public Function ToNullable(dateTime As Date) As Object
      If dateTime.IsEmpty Then
         Return DBNull.Value
      Else
         Return dateTime
      End If
   End Function

   <Extension()>
   Public Function Between(value As Date, beginRange As Date, endRange As Date) As Boolean
      Return TypeExtensions.Between(value, beginRange, endRange)
   End Function

   <Extension()>
   Public Function Between(value As Date, beginRange As Date, endRange As Date, includeTime As Boolean) As Boolean
      If includeTime Then
         Return value >= beginRange AndAlso value <= endRange
      Else
         Return value.Date >= beginRange.Date AndAlso value <= endRange.Date
      End If
   End Function

   <Extension()>
   Public Function ToIsoFormat(value As Date) As String
      Return TypeExtensions.ToIsoFormat(value, False)
   End Function

   <Extension()>
   Public Function ToIsoFormat(value As Date, enclosed As Boolean) As String
      If enclosed Then
         Return value.Date.ToString(Tags.DateFormatIso, CultureInfo.InvariantCulture).Enclose(EncloseCharacter.Quote)
      Else
         Return value.Date.ToString(Tags.DateFormatIso, CultureInfo.InvariantCulture)
      End If
   End Function

   <Extension()>
   Public Function ToIsoFormat(value As Date, separator As Char) As String
      Return value.ToString("yyyy" + separator + "MM" + separator + "dd", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToDisplayFormat(value As Date) As String
      Return value.Date.ToString(Tags.DateDisplayFormat)
   End Function

   <Extension()>
   Public Function ToPaddedDay(value As Date) As String
      Return value.ToString("dd", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToShortdDay(value As Date) As String
      Return value.ToString("ddd", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToLongDay(value As Date) As String
      Return value.ToString("dddd", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToPaddedMonth(value As Date) As String
      Return value.ToString("MM", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToShortMonth(value As Date) As String
      Return value.ToString("MMM", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function ToLongMonth(value As Date) As String
      Return value.ToString("MMMM", CultureInfo.InvariantCulture)
   End Function

   <Extension()>
   Public Function FirstDayOfMonth(value As Date) As Date
      Return New Date(value.Year, value.Month, 1)
   End Function

   <Extension()>
   Public Function LastDayOfMonth(value As Date) As Date
      Return New Date(value.Year, value.Month, Date.DaysInMonth(value.Year, value.Month))
   End Function

   <Extension()>
   Public Function IsEmpty(value As Date) As Boolean
      Return (value.ToUniversalTime = Tags.EmptyDate) OrElse (value.Date = Tags.EmptyDate)
   End Function

#End Region

#Region " Decimal "

   <Extension()>
   Public Function ToNullable(value As Decimal) As Object
      If value = 0 Then
         Return DBNull.Value
      Else
         Return value
      End If
   End Function

   <Extension()>
   Public Function Between(value As Decimal, beginRange As Decimal, endRange As Decimal) As Boolean
      Return value >= beginRange AndAlso value <= endRange
   End Function

   <Extension()>
   Public Function Enclose(value As Decimal) As String
      Return TypeExtensions.Enclose(value, EncloseCharacter.Parenthesis)
   End Function

   <Extension()>
   Public Function Enclose(value As Decimal, character As EncloseCharacter) As String
      Return value.ToString(CultureInfo.InvariantCulture).Enclose(character)
   End Function

#End Region

#Region " Timespan "

   <Extension()>
   Public Function ToNullable(value As TimeSpan) As Object
      If value = TimeSpan.Zero Then
         Return DBNull.Value
      Else
         Return value
      End If
   End Function

   <Extension()>
   Public Function ToDisplayFormat(value As TimeSpan) As String
      Return value.ToString(Tags.TimeDisplayFormat)
   End Function

   <Extension()>
   Public Function ToIsoFormat(value As TimeSpan) As String
      Return value.ToString()
   End Function

   <Extension()>
   Public Function ToCustomFormat(value As TimeSpan, format As String) As String
      Return New Date(value.Ticks).ToString(format)
   End Function

#End Region

#Region " DataTable "

   <Extension()>
   Friend Function ToXml(table As DataTable) As String
      Return TypeExtensions.ToXml(table, False)
   End Function

   <Extension()>
   Friend Function ToXml(table As DataTable, includeSchema As Boolean) As String

      If String.IsNullOrEmpty(table.TableName) Then
         table.TableName = "row"
      End If

      Dim _builder As New StringBuilder
      Dim _settings As New XmlWriterSettings
      _settings.OmitXmlDeclaration = True

      Using _writer As XmlWriter = XmlWriter.Create(_builder, _settings)
         If includeSchema Then
            table.WriteXml(_writer, XmlWriteMode.WriteSchema, False)
         Else
            table.WriteXml(_writer, XmlWriteMode.IgnoreSchema, False)
         End If

         Return _builder.ToString().Replace("DocumentElement", "table").Replace("NewDataSet", "data")

      End Using

   End Function

#End Region

#Region " DataSet "

   <Extension()>
   Public Function ToXml(dataSet As DataSet) As String
      Return TypeExtensions.ToXml(dataSet, False)
   End Function

   <Extension()>
   Public Function ToXml(dataSet As DataSet, includeSchema As Boolean) As String

      Dim _builder As New StringBuilder
      Dim _settings As New XmlWriterSettings
      _settings.OmitXmlDeclaration = True

      Using _writer As XmlWriter = XmlWriter.Create(_builder, _settings)
         If includeSchema Then
            dataSet.WriteXml(_writer, XmlWriteMode.WriteSchema)
         Else
            dataSet.WriteXml(_writer, XmlWriteMode.IgnoreSchema)
         End If

      End Using

      Return _builder.ToString().Replace("NewDataSet", "data")

   End Function

#End Region

#Region " DbDataReader "

   <Extension()>
   Public Function ToString(reader As DbDataReader, columnName As String) As String
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return String.Empty
      Else
         Return reader.Item(columnName).ToString()
      End If
   End Function

   <Extension()>
   Public Function ToInt32(reader As DbDataReader, columnName As String) As Integer
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return 0
      Else
         Return CInt(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToDecimal(reader As DbDataReader, columnName As String) As Decimal
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return 0.00D
      Else
         Return CDec(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToDouble(reader As DbDataReader, columnName As String) As Double
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return CDbl(0)
      Else
         Return CDbl(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToDate(reader As DbDataReader, columnName As String) As Date
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return Tags.EmptyDate
      Else
         Return CDate(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToDateTime(reader As DbDataReader, columnName As String) As DateTime
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return Tags.EmptyDate
      Else
         Return Convert.ToDateTime(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToBoolean(reader As DbDataReader, columnName As String) As Boolean
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return False
      Else
         Return CBool(reader.Item(columnName))
      End If
   End Function

   <Extension()>
   Public Function ToBase64(reader As DbDataReader, columnName As String) As String
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return String.Empty
      Else
         Return Convert.ToBase64String(DirectCast(reader.Item(columnName), Byte()))
      End If
   End Function

   <Extension()>
   Public Function ToTime(reader As DbDataReader, columnName As String) As TimeSpan
      If reader.Item(columnName) Is Nothing OrElse DBNull.Value.Equals(reader.Item(columnName)) Then
         Return TimeSpan.Zero
      Else
         Return CType(reader.Item(columnName), TimeSpan)
      End If
   End Function

#End Region

#Region " DataRow "

   <Extension()>
   Public Function ToBoolean(row As DataRow, columnName As String) As Boolean
      Return TypeExtensions.ToBoolean(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToBoolean(row As DataRow, columnName As String, version As DataRowVersion) As Boolean

      If row(columnName, version).Equals(DBNull.Value) Then
         Return False
      Else
         Return CBool(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToDate(row As DataRow, columnName As String) As Date
      Return TypeExtensions.ToDate(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToDate(row As DataRow, columnName As String, version As DataRowVersion) As Date
      Return TypeExtensions.ToDateTime(row, columnName, version).Date
   End Function

   <Extension()>
   Public Function ToDateTime(row As DataRow, columnName As String) As DateTime
      Return TypeExtensions.ToDateTime(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToDateTime(row As DataRow, columnName As String, version As DataRowVersion) As DateTime

      If row(columnName, version).Equals(DBNull.Value) Then
         Return Tags.EmptyDate
      Else
         Return Convert.ToDateTime(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToDecimal(row As DataRow, columnName As String) As Decimal
      Return TypeExtensions.ToDecimal(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToDecimal(row As DataRow, columnName As String, version As DataRowVersion) As Decimal

      If row(columnName, version).Equals(DBNull.Value) OrElse String.IsNullOrEmpty(row(columnName, version).ToString) Then
         Return CDec(0)
      Else
         Return CDec(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToDouble(row As DataRow, columnName As String) As Double
      Return TypeExtensions.ToDouble(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToDouble(row As DataRow, columnName As String, version As DataRowVersion) As Double

      If row(columnName, version).Equals(DBNull.Value) OrElse String.IsNullOrEmpty(row(columnName, version).ToString) Then
         Return CDbl(0)
      Else
         Return CDbl(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToInt32(row As DataRow, columnName As String) As Int32
      Return TypeExtensions.ToInt32(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToInt32(row As DataRow, columnName As String, version As DataRowVersion) As Int32

      If row(columnName, version).Equals(DBNull.Value) OrElse String.IsNullOrEmpty(row(columnName, version).ToString) Then
         Return 0
      Else
         Return CInt(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToInt64(row As DataRow, columnName As String) As Int64
      Return TypeExtensions.ToInt64(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToInt64(row As DataRow, columnName As String, version As DataRowVersion) As Int64

      If row(columnName, version).Equals(DBNull.Value) OrElse String.IsNullOrEmpty(row(columnName, version).ToString) Then
         Return 0
      Else
         Return CLng(row(columnName, version))
      End If

   End Function

   <Extension()>
   Public Function ToString(row As DataRow, columnName As String) As String
      Return TypeExtensions.ToString(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToString(row As DataRow, columnName As String, version As DataRowVersion) As String

      If row(columnName, version).Equals(DBNull.Value) Then
         Return String.Empty
      Else
         Return CStr(row(columnName, version)).Trim
      End If

   End Function

   <Extension()>
   Public Function ToTimeSpan(row As DataRow, columnName As String) As TimeSpan
      Return TypeExtensions.ToTimeSpan(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToTimeSpan(row As DataRow, columnName As String, version As DataRowVersion) As TimeSpan

      If row(columnName, version).Equals(DBNull.Value) Then
         Return TimeSpan.Zero
      Else
         Return CType(row(columnName, version), TimeSpan)
      End If

   End Function

   <Extension()>
   Public Function ToBytes(row As DataRow, columnName As String) As Byte()
      Return TypeExtensions.ToBytes(row, columnName, DataRowVersion.Current)
   End Function

   <Extension()>
   Public Function ToBytes(row As DataRow, columnName As String, version As DataRowVersion) As Byte()
      If row(columnName).Equals(DBNull.Value) Then
         Return Nothing
      Else
         Return DirectCast(row(columnName, version), Byte())
      End If
   End Function

#End Region

#Region " List(Of T) "

   <Extension()>
   Public Function ToInList(Of T)(list As List(Of T)) As String
      Return SysLib.ToInList(list)
   End Function

#End Region

#Region " Collection(Of T) "

   <Extension()>
   Public Function ToInList(Of T)(collection As Collection(Of T)) As String
      Return SysLib.ToInList(collection)
   End Function

#End Region

#Region " Helpers "

   Private Function GetParameterMarker(provider As DataProvider) As Char

      Select Case provider
         Case DataProvider.Sql
            Return "@"c

         Case Else
            Return "?"c

      End Select

   End Function

#End Region

End Module
