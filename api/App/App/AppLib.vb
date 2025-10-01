Public NotInheritable Class AppLib

   Public Shared Function GetJsonFile(filePath As String, folder As String) As Object

      Dim _mapPath As String = System.Web.Hosting.HostingEnvironment.MapPath("/" + folder)

        Dim _fileName As String = Path.Combine(_mapPath, filePath)
        If Not File.Exists(_fileName) Then
            Throw New FileNotFoundException(filePath)
        End If

        Dim _json As String = File.ReadAllText(_fileName)
      Return JsonConvert.DeserializeObject(_json)

   End Function

   Friend Shared Function GetUserSecurityInfo(userCode As String) As UserSecurityInfo

      Dim _info As New UserSecurityInfo

      Using _table As DataTable = DataLib.GetList("dbo.SecUser", "UserId, LogonId, Password", "UserCode=" + userCode.Enclose(EncloseCharacter.Quote), String.Empty)
         With _table
            If .Rows.Count > 0 Then
               Dim _row As DataRow = _table.Rows(0)

               With _info
                  .UserId = _row.ToInt32("UserId")
                  .LogonId = _row.ToString("LogonId")
                  .OldHashedPassword = _row.ToString("Password")
               End With

            End If
         End With
      End Using

      Return _info

   End Function

   Friend Shared Sub AddLogDetail(list As SysLogDetailList, id As Integer, columnId As LogColumnId, oldValue As String, newValue As String)
      AppLib.AddLogDetail(list, id, columnId, oldValue, newValue, "")
   End Sub

   Friend Shared Sub AddLogDetail(list As SysLogDetailList, id As Integer, columnId As LogColumnId, oldValue As String, newValue As String, logReference As String)

      Dim _logDetail As New SysLogDetail

      With _logDetail
         .Id = id
         .SeqId = list.Count + 1
         .ColumnId = columnId
         .OldValue = oldValue
         .NewValue = newValue
         .LogReference = logReference
      End With

      list.Add(_logDetail)

   End Sub

   Friend Shared Function CreateLogHeader(storedProcName As String, logKeyList As LogKeyList, logActionId As LogActionId, userId As Integer) As Integer
      Return AppLib.CreateLogHeader(storedProcName, logKeyList, logActionId, 0, userId)
   End Function

   Friend Shared Function CreateLogHeader(storedProcName As String, logKeyList As LogKeyList, logActionId As LogActionId, logSetId As Integer, userId As Integer) As Integer

      With DataCore.Command
         .CommandText = "web." + storedProcName
         .CommandType = CommandType.StoredProcedure

         Dim _idParam As SqlClient.SqlParameter

         With .Parameters
            .Clear()

            For Each _pair As KeyValuePair(Of String, Object) In logKeyList
               .AddWithValue("@" + _pair.Key, _pair.Value)
            Next

            .Add("@LogId", SqlDbType.Int).Value = logActionId
            .Add("@LogUserId", SqlDbType.Int).Value = userId

            If storedProcName = "InsFosAccountLog" Then
               .Add("@LogSetId", SqlDbType.Int).Value = logSetId
            End If

            _idParam = .Add("@Id", SqlDbType.Int)
            _idParam.Direction = ParameterDirection.Output
         End With

         .ExecuteNonQuery()

         Return CInt(_idParam.Value)

      End With

   End Function

   Friend Shared Sub CreateLogDetails(list As SysLogDetailList, tableName As String)

      With DataCore.Command
         .CommandText = "INSERT INTO mod." + tableName + " (Id, SeqId, ColumnId, OldValue, NewValue, LogReference) " +
                        "VALUES (@Id, @SeqId, @ColumnId, @OldValue, @NewValue, @LogReference)"

         .CommandType = CommandType.Text
      End With

      For Each _detail As SysLogDetail In list
         With DataCore.Command.Parameters
            .Clear()

            .Add("@Id", SqlDbType.Int).Value = _detail.Id
            .Add("@SeqId", SqlDbType.Int).Value = _detail.SeqId
            .Add("@ColumnId", SqlDbType.Int).Value = _detail.ColumnId
            '.Add("@OldValue", SqlDbType.NVarChar).Value = Datalib.ToNullableString(_detail.OldValue)
            '.Add("@NewValue", SqlDbType.NVarChar).Value = Datalib.ToNullableString(_detail.NewValue)
            '.Add("@LogReference", SqlDbType.NVarChar).Value = Datalib.ToNullableString(_detail.LogReference)

            .Add("@OldValue", SqlDbType.NVarChar).Value = _detail.OldValue.ToNullable()
            .Add("@NewValue", SqlDbType.NVarChar).Value = _detail.NewValue.ToNullable()
            .Add("@LogReference", SqlDbType.NVarChar).Value = _detail.LogReference.ToNullable()

         End With

         DataCore.Command.ExecuteNonQuery()

      Next

   End Sub

   Friend Shared Sub AssignLogHeaderId(id As Integer, detailList As SysLogDetailList)

      For Each _detail As SysLogDetail In detailList
         _detail.Id = id
      Next

   End Sub
   Public Shared Function GetNextTrxTypeSequence(nextSeqId As NextSeqId) As String

      With DataCore.Command
         .Connection = DataCore.Connection
         .CommandText = "web.SysGetNextTrxTypeSequence"
         .CommandType = CommandType.StoredProcedure
         .Parameters.Clear()

         Dim _docSeqIdParam As SqlParameter
         Dim _docSeqId As String

         With .Parameters
            .Add("@trxTypeId", SqlDbType.Int).Value = nextSeqId

            _docSeqIdParam = .Add("@NextSeqId", SqlDbType.VarChar, 15)
            _docSeqIdParam.Direction = ParameterDirection.Output
         End With

         DataCore.Disconnect()
         DataCore.Connect("sys")
         .ExecuteScalar()

         _docSeqId = CStr(_docSeqIdParam.Value)
         DataCore.Disconnect()

         Return _docSeqId

      End With

   End Function

   Public Shared Function GetNextDocSequence(docSequencerId As DocSequencerId, sysDate As Date) As String

      With DataCore.Command
         .Connection = DataCore.Connection
         .CommandText = "web.SysGetNextDocSequence"
         .CommandType = CommandType.StoredProcedure
         .Parameters.Clear()

         Dim _docSeqIdParam As SqlParameter
         Dim _docSeqId As String

         With .Parameters
            .Add("@DocSequencerId", SqlDbType.Int).Value = docSequencerId
            .Add("@SysDate", SqlDbType.Date).Value = sysDate.Date.ToIsoFormat("-"c)

            _docSeqIdParam = .Add("@DocSeqId", SqlDbType.VarChar, 15)
            _docSeqIdParam.Direction = ParameterDirection.Output
         End With

         DataCore.Disconnect()
         DataCore.Connect("sys")
         .ExecuteScalar()

         _docSeqId = CStr(_docSeqIdParam.Value)
         DataCore.Disconnect()

         Return _docSeqId

      End With

   End Function

   'Public Shared Function GetNextDocSequence(docSequencerId As DocSequencerId, sysDate As Date) As String
   '   Return AppLib.GetNextDocSequence(docSequencerId, sysDate, Nothing)
   'End Function

   'Public Shared Function GetNextDocSequence(docSequencerId As DocSequencerId, sysDate As Date, sequencerId As String) As String

   '   Dim _docSeqId As String

   '   If String.IsNullOrEmpty(sequencerId) Then
   '      With DataCore.Command
   '         .Connection = DataCore.Connection
   '         .CommandText = "web.SysGetNextDocSequence"
   '         .CommandType = CommandType.StoredProcedure
   '         .Parameters.Clear()

   '         Dim _docSeqIdParam As SqlParameter
   '         'Dim _docSeqId As String

   '         With .Parameters
   '            .Add("@DocSequencerId", SqlDbType.Int).Value = docSequencerId
   '            .Add("@SysDate", SqlDbType.Date).Value = sysDate.Date.ToIsoFormat("-"c)

   '            _docSeqIdParam = .Add("@DocSeqId", SqlDbType.VarChar, 15)
   '            _docSeqIdParam.Direction = ParameterDirection.Output
   '         End With

   '         DataCore.Disconnect()
   '         DataCore.Connect("sys")
   '         .ExecuteScalar()

   '         _docSeqId = CStr(_docSeqIdParam.Value)
   '         DataCore.Disconnect()

   '      End With

   '   Else

   '      Dim _seq As Integer = SysLib.GetNextSequence(sequencerId)
   '      _docSeqId = sysDate.ToIsoFormat("-"c).Substring(0, 8) + _seq.ToString("D4")

   '   End If

   '   Return _docSeqId

   'End Function

   Friend Shared Function NumberToText(value As Decimal) As String
      Return AppLib.NumberToText(value, FractionStyle.Word, "en-PH")
   End Function

   Friend Shared Function NumberToText(value As Decimal, fractionStyle As FractionStyle) As String
      Return AppLib.NumberToText(value, fractionStyle, "en-PH")
   End Function

   Private Shared Function NumberToText(value As Decimal, fractionStyle As FractionStyle, cultureName As String) As String

      ' Default culture is 'en-US'
      Dim _cardinalSingle As String = "Dollar"
      Dim _cardinalPlural As String = "Dollars"
      Dim _fractionSingle As String = "Cent"
      Dim _fractionPlural As String = "Cents"

      If EasSession.CurrencyWords.ContainsKey(cultureName) Then
         With EasSession.CurrencyWords(cultureName)
            If Not (String.IsNullOrEmpty(.CardinalSingle) OrElse String.IsNullOrEmpty(.CardinalPlural) OrElse String.IsNullOrEmpty(.FractionSingle) OrElse String.IsNullOrEmpty(.FractionPlural)) Then
               _cardinalSingle = .CardinalSingle
               _cardinalPlural = .CardinalPlural
               _fractionSingle = .FractionSingle
               _fractionPlural = .FractionPlural
            End If
         End With
      End If

      Dim _value As String = value.ToString("#################0.00").Trim
      Dim _decimalSeparator As String = EasSession.CurrencyDecimalSeparator
      Dim _decimalPosition As Integer = _value.IndexOf(_decimalSeparator, StringComparison.Ordinal)

      Dim _wholeNumber As String = _value.Substring(0, _decimalPosition)
      Dim _fraction As String = _value.Substring(_decimalPosition)
      Dim _wholePartValue As Long = CLng(_wholeNumber)

      If CSng(_fraction) > 0 Then
         Dim _decimalValue As Double = Math.Round(CSng(_fraction), 2) * 100
         _fraction = "and " + CStr(_decimalValue)

         Select Case fractionStyle
            Case FractionStyle.PerHundred
               _fraction = _fraction + "/100"
            Case Else      ' Word
               If _decimalValue = 1 Then
                  _fraction = _fraction + " " + _fractionSingle
               Else
                  _fraction = _fraction + " " + _fractionPlural
               End If
         End Select

      Else
         _fraction = String.Empty
      End If

      Dim _text As String

      If _wholePartValue = 1 Then
         _text = (AppLib.NumberToText(CLng(_wholeNumber)) + _cardinalSingle + " " + _fraction).Replace("  ", " ")
      Else
         _text = (AppLib.NumberToText(CLng(_wholeNumber)) + _cardinalPlural + " " + _fraction).Replace("  ", " ")
      End If

      Return _text + " Only"

   End Function

   Private Shared Function NumberToText(value As Long) As String

      Select Case value
         Case 0
            Return String.Empty

         Case Is <= 19
            Return New String() {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"}(CInt(value - 1)) + " "

         Case Is <= 99
            Return New String() {"Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"}(CInt(value \ 10 - 2)) + " " + AppLib.NumberToText(value Mod 10)

         Case Is <= 199
            Return "One Hundred " + AppLib.NumberToText(value Mod 100)

         Case Is <= 999
            Return AppLib.NumberToText(value \ 100) + " Hundred " + AppLib.NumberToText(value Mod 100)

         Case Is <= 1999
            Return "One Thousand " + AppLib.NumberToText(value Mod 1000)

         Case Is <= 999999
            Return AppLib.NumberToText(value \ 1000) + " Thousand " + AppLib.NumberToText(value Mod 1000)

         Case Is <= 1999999
            Return "One Million " + AppLib.NumberToText(value Mod 1000000)

         Case Is <= 999999999
            Return AppLib.NumberToText(value \ 1000000) + " Million " + AppLib.NumberToText(value Mod 1000000)

         Case Is <= 1999999999
            Return "One Billion " + AppLib.NumberToText(value Mod 1000000000)

         Case Else
            Return AppLib.NumberToText(value \ 1000000000) + "Billion " + AppLib.NumberToText(value Mod 1000000000)

      End Select

   End Function

End Class

Friend Class UserSecurityInfo
   Friend Property UserId As Integer
   Friend Property LogonId As String
   Friend Property OldHashedPassword As String

End Class

