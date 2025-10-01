<RoutePrefix("api")>
Public Class GLS0100_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_GLS0100")>
   <Route("references/gls0100")>
   <HttpGet>
   Public Function GetReferences_GLS0100() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.GLS0100_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "trxTypes"     ' all defined Transaction Types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFinTemplate")>
   <Route("fin-templates/{templateId}")>
   <HttpGet>
   Public Function GetFinTemplate(templateId As Integer) As IHttpActionResult

      If templateId <= 0 Then
         Throw New ArgumentException("Template ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.GLS0100")
            With _direct
               .AddParameter("TemplateId", templateId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "template"
                     .Tables(1).TableName = "templateDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetFinTemplateLog")>
   <Route("fin-templates/{templateId}/log")>
   <HttpGet>
   Public Function GetFinTemplateLog(templateId As Integer) As IHttpActionResult

      If templateId <= 0 Then
         Throw New ArgumentException("Template ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.GLS0100_Log")
            With _direct
               .AddParameter("TemplateId", templateId)

               ' Allow DateTime
               Dim _settings As New JsonSerializerSettings
               With _settings
                  .ContractResolver = New CamelCasePropertyNamesContractResolver
                  .DateFormatString = "yyyy-MM-ddTHH:mm"
               End With

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  'Return Me.Ok(_dataTable)
                  Return Json(_dataTable, _settings)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateFinTemplate")>
   <Route("fin-templates/{currentUserId}")>
   <HttpPost>
   Public Function CreateFinTemplate(currentUserId As Integer, <FromBody> template As FinTemplateBody) As IHttpActionResult

      If template.TemplateId <> -1 Then
         Throw New ArgumentException("Template ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _templateId As Integer = SysLib.GetNextSequence("FinTemplateId")

         template.TemplateId = _templateId

         '
         ' Load proposed values from payload
         '
         Dim _finTemplate As New FinTemplate
         Dim _finTemplateDetailList As New FinTemplateDetailList

         Me.LoadFinTemplate(template, _finTemplate)

         For Each _detail As FinTemplateDetail In template.Details
            _detail.TemplateId = _templateId
            _finTemplateDetailList.Add(_detail)
         Next

         '
         ' Log addition, save to DB
         '

         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         'Dim _finTemplateLogDetailList As New SysLogDetailList
         'Dim _finTemplateDetailLogDetailList As New SysLogDetailList

         'With _finTemplate
         '   AppLib.AddLogDetail(_finTemplateLogDetailList, 0, LogColumnId.TemplateName, String.Empty, .TemplateName)

         '   Dim _journalName As String = FrsSession.DbsJournal.Rows.Find(Function(m) m.JournalId = .JournalId).JournalName
         '   AppLib.AddLogDetail(_finTemplateLogDetailList, 0, LogColumnId.JournalId, String.Empty, .JournalId.ToString, .JournalId.ToString + "=" + _journalName)

         'End With

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertFinTemplate(_finTemplate)
            Me.InsertFinTemplateDetails(_finTemplateDetailList)

            'If _finTemplateLogDetailList.Count > 0 Then
            '   With _logKeyList
            '      .Clear()
            '      .Add("TemplateId", _finTemplate.TemplateId)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsFinTemplateLog", _logKeyList, LogActionId.Add, _currentUserId)
            '   AppLib.AssignLogHeaderId(_id, _finTemplateLogDetailList)
            '   AppLib.CreateLogDetails(_finTemplateLogDetailList, "FinTemplateLogDetail")

            'End If

            'For Each _new As FinTemplateDetail In _finTemplateDetailList

            '   With _logKeyList
            '      .Clear()
            '      .Add("TemplateId", _new.TemplateId)
            '      .Add("AccountId", _new.AccountId)
            '   End With

            '   _id = AppLib.CreateLogHeader("InsFinTemplateDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

            '   _finTemplateDetailLogDetailList.Clear()

            '   With _new
            '      If .DebitAmount > 0 Then
            '         AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.DebitAmount, "", .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
            '      End If
            '      If .CreditAmount > 0 Then
            '         AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.CreditAmount, "", .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
            '      End If

            '      AppLib.CreateLogDetails(_finTemplateDetailLogDetailList, "FinTemplateDetailLogDetail")

            '   End With
            'Next

            _successFlag = True

         Catch _exception As Exception
            'File.WriteAllText("d:\yyy.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         'Return Me.Ok(True)
         Return Me.Ok(template.TemplateId)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyFinTemplate")>
   <Route("fin-templates/{templateId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyFinTemplate(templateId As Integer, currentUserId As Integer, <FromBody> template As FinTemplateBody) As IHttpActionResult

      If templateId <= 0 Then
         Throw New ArgumentException("Template ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _finTemplate As New FinTemplate
         Dim _finTemplateDetailList As New FinTemplateDetailList

         Me.LoadFinTemplate(template, _finTemplate)

         For Each _detail As FinTemplateDetail In template.Details
            _finTemplateDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _finTemplateOld As New FinTemplate
         Dim _finTemplateDetailListOld As New FinTemplateDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetFinTemplate(templateId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("template").Rows(0)
            Me.LoadFinTemplate(_row, _finTemplateOld)
            Me.LoadFinTemplateDetailList(_dataSet.Tables("templateDetails").Rows, _finTemplateDetailListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         'Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         '
         ' FinTemplate
         '

         'Dim _finTemplateLogDetailList As New SysLogDetailList

         'With _finTemplateOld
         '   If .TemplateName <> _finTemplate.TemplateName Then
         '      AppLib.AddLogDetail(_finTemplateLogDetailList, 0, LogColumnId.TemplateName, .TemplateName, _finTemplate.TemplateName)
         '   End If

         '   If .JournalId <> _finTemplate.JournalId Then
         '      Dim _oldJournalName As String = FrsSession.DbsJournal.Rows.Find(Function(m) m.JournalId = .JournalId).JournalName
         '      Dim _newJournalName As String = FrsSession.DbsJournal.Rows.Find(Function(m) m.JournalId = _finTemplate.JournalId).JournalName

         '      AppLib.AddLogDetail(_finTemplateLogDetailList, 0, LogColumnId.JournalId, .JournalId.ToString, _finTemplate.JournalId.ToString, .JournalId.ToString + "=" + _oldJournalName + "; " + _finTemplate.JournalId.ToString + "=" + _newJournalName)
         '   End If

         'End With

         '
         ' FinTemplateDetail
         '

         'Dim _finTemplateDetailLogDetailList As New SysLogDetailList

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As FinTemplateDetail In _finTemplateDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As FinTemplateDetail In _finTemplateDetailList
               If _new.TemplateDetailId = _old.TemplateDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As FinTemplateDetail In _finTemplateDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As FinTemplateDetail In _finTemplateDetailListOld
               If _new.TemplateDetailId = _old.TemplateDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .AccountId <> _old.AccountId Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .DebitAmount <> _old.DebitAmount OrElse .CreditAmount <> _old.CreditAmount Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addDetailCount = _addDetailCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editDetailCount = _editDetailCount + 1
            End If

         Next

         Dim _finTemplateDetailListNew As New FinTemplateDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _finTemplateDetail As FinTemplateDetail

            For Each _new As FinTemplateDetail In _finTemplateDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _finTemplateDetail = New FinTemplateDetail
                  _finTemplateDetailListNew.Add(_finTemplateDetail)
                  DataLib.ScatterValues(_new, _finTemplateDetail)
                  _finTemplateDetail.TemplateId = _finTemplate.TemplateId
               End If
            Next

         End If

         Dim _isFinTemplateChanged As Boolean = Me.HasFinTemplateChanges(_finTemplateOld, _finTemplate)

         If Not _isFinTemplateChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetFinTemplate(templateId)
         End If

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            If _isFinTemplateChanged Then
               Me.UpdateFinTemplate(_finTemplate)

               'With _logKeyList
               '   .Clear()
               '   .Add("TemplateId", _finTemplate.TemplateId)
               'End With

               '_id = AppLib.CreateLogHeader("InsFinTemplateLog", _logKeyList, LogActionId.Edit, _currentUserId)
               'AppLib.AssignLogHeaderId(_id, _finTemplateLogDetailList)

               'AppLib.CreateLogDetails(_finTemplateLogDetailList, "FinTemplateLogDetail")

            End If

            '
            ' FinTemplateDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteFinTemplateDetails(_finTemplateDetailListOld)

               'For Each _old As FinTemplateDetail In _finTemplateDetailListOld
               '   If _old.LogActionId = LogActionId.Delete Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("TemplateId", _old.TemplateId)
               '         .Add("AccountId", _old.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsFinTemplateDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)

               '      _finTemplateDetailLogDetailList.Clear()

               '      With _old
               '         AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

               '         If .DebitAmount > 0 Then
               '            AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.DebitAmount, .DebitAmount.ToString("N"), "", "AccountId=" + .AccountId)
               '         End If
               '         If .CreditAmount > 0 Then
               '            AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.CreditAmount, .CreditAmount.ToString("N"), "", "AccountId=" + .AccountId)
               '         End If
               '      End With

               '      AppLib.CreateLogDetails(_finTemplateDetailLogDetailList, "FinTemplateDetailLogDetail")

               '   End If
               'Next

            End If

            If _addDetailCount > 0 Then
               Me.InsertFinTemplateDetails(_finTemplateDetailListNew)

               'For Each _new As FinTemplateDetail In _finTemplateDetailListNew

               '   With _logKeyList
               '      .Clear()
               '      .Add("TemplateId", _new.TemplateId)
               '      .Add("AccountId", _new.AccountId)
               '   End With

               '   _id = AppLib.CreateLogHeader("InsFinTemplateDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               '   _finTemplateDetailLogDetailList.Clear()

               '   With _new
               '      AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

               '      If .DebitAmount > 0 Then
               '         AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.DebitAmount, "", .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
               '      End If
               '      If .CreditAmount > 0 Then
               '         AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.CreditAmount, "", .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
               '      End If

               '      AppLib.CreateLogDetails(_finTemplateDetailLogDetailList, "FinTemplateDetailLogDetail")

               '   End With
               'Next

            End If

            If _editDetailCount > 0 Then
               Me.UpdateFinTemplateDetails(_finTemplateDetailList)

               'For Each _new As FinTemplateDetail In _finTemplateDetailList
               '   If _new.LogActionId = LogActionId.Edit Then

               '      With _logKeyList
               '         .Clear()
               '         .Add("TemplateId", _new.TemplateId)
               '         .Add("AccountId", _new.AccountId)
               '      End With

               '      _id = AppLib.CreateLogHeader("InsFinTemplateDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)

               '      _finTemplateDetailLogDetailList.Clear()

               '      For Each _old As FinTemplateDetail In _finTemplateDetailListOld
               '         If _new.TemplateDetailId = _old.TemplateDetailId Then
               '            With _new
               '               If .AccountId <> _old.AccountId Then
               '                  AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.AccountId, _old.AccountId, .AccountId)
               '               End If
               '               If .DebitAmount <> _old.DebitAmount Then
               '                  AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.DebitAmount, _old.DebitAmount.ToString("N"), .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
               '               End If
               '               If .CreditAmount <> _old.CreditAmount Then
               '                  AppLib.AddLogDetail(_finTemplateDetailLogDetailList, _id, LogColumnId.CreditAmount, _old.CreditAmount.ToString("N"), .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
               '               End If

               '               AppLib.CreateLogDetails(_finTemplateDetailLogDetailList, "FinTemplateDetailLogDetail")

               '            End With

               '         End If
               '      Next

               '   End If

               'Next

            End If

            _successFlag = True

         Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         Return Me.Ok(True)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveFinTemplate")>
   <Route("fin-templates/{templateId}")>
   <HttpDelete>
   Public Function RemoveFinTemplate(templateId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If templateId <= 0 Then
         Throw New ArgumentException("Template ID is required.")
      End If

      Try
         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.DeleteFinTemplate(templateId, q.LockId)

            _successFlag = True

         Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
         Finally
            With _transaction
               If _successFlag Then
                  .Commit()
               Else
                  .Rollback()
               End If
               .Dispose()
            End With

            _transaction = Nothing
            DataCore.Disconnect()

         End Try

         Return Me.Ok(True)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   Private Sub LoadFinTemplate(template As FinTemplateBody, finTemplate As FinTemplate)

      DataLib.ScatterValues(template, finTemplate)

   End Sub

   Private Sub LoadFinTemplate(row As DataRow, template As FinTemplate)

      With template
         .TemplateId = row.ToInt32("TemplateId")
         .TemplateName = row.ToString("TemplateName")
         .TrxTypeId = row.ToInt32("TrxTypeId")
      End With

   End Sub

   Private Sub LoadFinTemplateDetailList(rows As DataRowCollection, list As FinTemplateDetailList)

      Dim _detail As FinTemplateDetail
      For Each _row As DataRow In rows
         _detail = New FinTemplateDetail

         With _detail
            .TemplateDetailId = _row.ToInt32("TemplateDetailId")
            .TemplateId = _row.ToInt32("TemplateId")
            .AccountId = _row.ToString("AccountId")
            .DebitAmount = _row.ToDecimal("DebitAmount")
            .CreditAmount = _row.ToDecimal("CreditAmount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertFinTemplate(template As FinTemplate)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTemplate", template, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinTemplate(template)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFinTemplateDetails(list As FinTemplateDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TemplateDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTemplateDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinTemplateDetail In list
            Me.AddInsertUpdateParamsFinTemplateDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateFinTemplate(template As FinTemplate)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TemplateId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinTemplate", template, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinTemplate(template)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(template.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateFinTemplateDetails(list As FinTemplateDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TemplateDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinTemplateDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinTemplateDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsFinTemplateDetail(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinTemplate(template As FinTemplate)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TemplateId", template.TemplateId)
         .AddWithValue("@TemplateName", template.TemplateName)
         .AddWithValue("@TrxTypeId", template.TrxTypeId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinTemplateDetail(dtl As FinTemplateDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TemplateId", dtl.TemplateId)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@DebitAmount", dtl.DebitAmount)
         .AddWithValue("@CreditAmount", dtl.CreditAmount)
      End With

   End Sub

   Private Sub DeleteFinTemplate(templateId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TemplateId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinTemplate", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@TemplateId", templateId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteFinTemplateDetails(list As FinTemplateDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.FinTemplateDetail WHERE TemplateDetailId=@TemplateDetailId"
         .CommandType = CommandType.Text

         For Each _old As FinTemplateDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TemplateDetailId", _old.TemplateDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasFinTemplateChanges(oldRecord As FinTemplate, newRecord As FinTemplate) As Boolean

      With oldRecord
         If .TemplateName <> newRecord.TemplateName Then Return True
         If .TrxTypeId <> newRecord.TrxTypeId Then Return True
      End With

      Return False

   End Function

End Class

Public Class FinTemplateBody
   Inherits FinTemplate

   Public Property Details As FinTemplateDetail()

End Class
