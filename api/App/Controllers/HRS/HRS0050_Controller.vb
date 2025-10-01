<RoutePrefix("api")>
Public Class HRS0050_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetEmploymentStatusActionLog")>
   <Route("employment-status-actions/{employmentStatusActionId}/{memberId}/log")>
   <HttpGet>
   Public Function GetEmploymentStatusActionLog(employmentStatusActionId As Integer, MemberId As Integer) As IHttpActionResult

      If employmentStatusActionId <= 0 Then
         Throw New ArgumentException("Employment Status Action ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("Web.HRS0050_Log")
            With _direct
               .AddParameter("employmentStatusActionId", employmentStatusActionId)
               .AddParameter("MemberId", MemberId)

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


   <SymAuthorization("GetReferences_HRS0050")>
   <Route("references/hrs0050")>
   <HttpGet>
   Public Function GetReferences_DBS0390() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.HRS0050_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                            .Tables(0).TableName = "employmentStatus"
                            .Tables(1).TableName = "workForms" ' all defined Transaction Types
                        End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetEmploymentStatusAction")>
   <Route("employment-status-actions/{employmentStatusActionId}")>
   <HttpGet>
   Public Function GetEmploymentStatusAction(employmentStatusActionId As Integer) As IHttpActionResult

      If employmentStatusActionId <= 0 Then
         Throw New ArgumentException("Employment Status Action ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.HRS0050")
            With _direct
               .AddParameter("employmentStatusActionId", employmentStatusActionId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "action"
                     .Tables(1).TableName = "members"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateEmploymentStatusAction")>
   <Route("employment-status-actions/{currentUserId}")>
   <HttpPost>
   Public Function CreateEmploymentStatusAction(currentUserId As Integer, <FromBody> action As HrsEmploymentStatusActionBody) As IHttpActionResult
        File.WriteAllText("d:\act1.txt", action.ToString)
        If action.EmploymentStatusActionId <> -1 Then
            Throw New ArgumentException("Employment Status Action ID not recognized.")
        End If

        Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _employmentStatusActionId As Integer = SysLib.GetNextSequence("EmploymentStatusActionId")

         action.EmploymentStatusActionId = _employmentStatusActionId

         '
         ' Load proposed values from payload
         '
         Dim _hrsEmploymentStatusAction As New HrsEmploymentStatusAction
         Dim _hrsEmploymentStatusMemberList As New HrsEmploymentStatusMemberList

         Me.LoadHrsEmploymentStatusAction(action, _hrsEmploymentStatusAction)
            File.WriteAllText("d:\act2.txt", action.ToString)
            For Each _detail As HrsEmploymentStatusMember In action.Members
            _detail.EmploymentStatusActionId = _employmentStatusActionId
            _hrsEmploymentStatusMemberList.Add(_detail)
         Next


         Dim _logKeyList As New LogKeyList

         Dim _id As Integer
         Dim _hrsEmploymentStatusActionLogDetailList As New SysLogDetailList
         Dim _hrsEmploymentStatusMemberLogDetailList As New SysLogDetailList

         With _hrsEmploymentStatusAction

            AppLib.AddLogDetail(_hrsEmploymentStatusActionLogDetailList, 0, LogColumnId.EmploymentStatusActionId, String.Empty, .EmploymentStatusActionId.ToString)
            AppLib.AddLogDetail(_hrsEmploymentStatusActionLogDetailList, 0, LogColumnId.Remarks, String.Empty, .Remarks)

         End With


         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean
            File.WriteAllText("d:\act3.txt", action.ToString)
            Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertHrsEmploymentStatusAction(_hrsEmploymentStatusAction)
            File.WriteAllText("d:\act4.txt", action.ToString)
                If _hrsEmploymentStatusActionLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("EmploymentStatusActionId", _hrsEmploymentStatusAction.EmploymentStatusActionId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusActionLog", _logKeyList, LogActionId.Add, currentUserId)
               AppLib.AssignLogHeaderId(_id, _hrsEmploymentStatusActionLogDetailList)
               AppLib.CreateLogDetails(_hrsEmploymentStatusActionLogDetailList, "HrsEmploymentStatusActionLogDetail")

            End If

                File.WriteAllText("d:\act5.txt", action.ToString)
                If _hrsEmploymentStatusMemberList.Count > 0 Then
               Me.InsertHrsEmploymentStatusMembers(_hrsEmploymentStatusMemberList)
            End If

                Try

                    File.WriteAllText("d:\act6.txt", action.ToString)
                    For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList

                  With _logKeyList
                     .Clear()
                            .Add("EmploymentStatusActionId", _new.EmploymentStatusActionId)
                            .Add("WorkFormId", _new.WorkFormId)
                            .Add("MemberId", _new.MemberId)
                  End With
                  Try


                     _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Add, currentUserId)

                     _hrsEmploymentStatusMemberLogDetailList.Clear()
                  Catch ex As Exception

                  End Try
                  With _new

                     Dim _employmentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, String.Empty, .EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _employmentStatusName)

                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.MemberId, String.Empty, .MemberId.ToString, .MemberId.ToString)
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EffectivityDate, String.Empty, .EffectivityDate.ToString, .EffectivityDate.ToString)
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EndDate, String.Empty, .EndDate.ToString, .EndDate.ToString)

                     AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")
                  End With
               Next

            Catch ex As Exception

            End Try

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
            File.WriteAllText("d:\act7.txt", action.ToString)
            'Return Me.Ok(True)
            Return Me.Ok(action.EmploymentStatusActionId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyEmploymentStatusAction")>
   <Route("employment-status-actions/{employmentStatusActionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyEmploymentStatusAction(employmentStatusActionId As Integer, currentUserId As Integer, <FromBody> action As HrsEmploymentStatusActionBody) As IHttpActionResult
        File.WriteAllText("d:\act1.txt", action.ToString)
        If employmentStatusActionId <= 0 Then
         Throw New ArgumentException("Employment Status Action ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _hrsEmploymentStatusAction As New HrsEmploymentStatusAction
         Dim _hrsEmploymentStatusMemberList As New HrsEmploymentStatusMemberList

         Me.LoadHrsEmploymentStatusAction(action, _hrsEmploymentStatusAction)

         For Each _detail As HrsEmploymentStatusMember In action.Members
            _hrsEmploymentStatusMemberList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _hrsEmploymentStatusActionOld As New HrsEmploymentStatusAction
         Dim _hrsEmploymentStatusMemberListOld As New HrsEmploymentStatusMemberList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetEmploymentStatusAction(employmentStatusActionId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("action").Rows(0)
            Me.LoadHrsEmploymentStatusAction(_row, _hrsEmploymentStatusActionOld)
            Me.LoadHrsEmploymentStatusMemberList(_dataSet.Tables("members").Rows, _hrsEmploymentStatusMemberListOld)

         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer


         Dim _hrsEmploymentStatusActionLogDetailList As New SysLogDetailList

         With _hrsEmploymentStatusActionOld
                File.WriteAllText("d:\act2.txt", action.ToString)
                If .EmploymentStatusActionId <> _hrsEmploymentStatusAction.EmploymentStatusActionId Then
               AppLib.AddLogDetail(_hrsEmploymentStatusActionLogDetailList, 0, LogColumnId.EmploymentStatusActionId, .EmploymentStatusActionId.ToString, _hrsEmploymentStatusAction.EmploymentStatusActionId.ToString)
            End If

            If .Remarks <> _hrsEmploymentStatusAction.Remarks Then
               AppLib.AddLogDetail(_hrsEmploymentStatusActionLogDetailList, 0, LogColumnId.Remarks, .Remarks.ToString, _hrsEmploymentStatusAction.Remarks.ToString)
            End If

         End With

         'JENON

         Dim _hrsEmploymentStatusMemberLogDetailList As New SysLogDetailList


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList
               If _new.EmploymentStatusMemberId = _old.EmploymentStatusMemberId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList
            _new.LogActionId = LogActionId.Add
            For Each _old As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListOld
               If _new.EmploymentStatusMemberId = _old.EmploymentStatusMemberId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If
                     'If .MemberId <> _old.MemberId OrElse .EmploymentFileName <> _old.EmploymentFileName OrElse .EmploymentGUID <> _old.EmploymentGUID Then
                     '    .LogActionId = LogActionId.Edit
                     'End If
                     If .MemberId <> _old.MemberId OrElse .WorkFormId <> _old.WorkFormId OrElse .EmploymentStatusId <> _old.EmploymentStatusId OrElse .EffectivityDate <> _old.EffectivityDate OrElse .EndDate <> _old.EndDate OrElse .EmploymentFileName <> _old.EmploymentFileName OrElse .EmploymentGUID <> _old.EmploymentGUID Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next
                File.WriteAllText("d:\act3.txt", action.ToString)
                If _new.LogActionId = LogActionId.Add Then

               _addDetailCount = _addDetailCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editDetailCount = _editDetailCount + 1
            End If

         Next

         Dim _hrsEmploymentStatusMemberListNew As New HrsEmploymentStatusMemberList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _hrsEmploymentStatusMember As HrsEmploymentStatusMember

            For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList
               If _new.LogActionId = LogActionId.Add Then
                  _hrsEmploymentStatusMember = New HrsEmploymentStatusMember
                  _hrsEmploymentStatusMemberListNew.Add(_hrsEmploymentStatusMember)
                  DataLib.ScatterValues(_new, _hrsEmploymentStatusMember)
                  _hrsEmploymentStatusMember.EmploymentStatusActionId = _hrsEmploymentStatusAction.EmploymentStatusActionId
               End If
            Next

         End If

         Dim _isHrsEmploymentStatusActionChanged As Boolean = Me.HasHrsEmploymentStatusActionChanges(_hrsEmploymentStatusActionOld, _hrsEmploymentStatusAction)

         If Not _isHrsEmploymentStatusActionChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetEmploymentStatusAction(employmentStatusActionId)
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

            If _isHrsEmploymentStatusActionChanged Then
               Me.UpdateHrsEmploymentStatusAction(_hrsEmploymentStatusAction)
            End If


            If _hrsEmploymentStatusActionLogDetailList.Count > 0 Then

               With _logKeyList
                  .Clear()
                  .Add("EmploymentStatusActionId", _hrsEmploymentStatusAction.EmploymentStatusActionId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusActionLog", _logKeyList, LogActionId.Edit, currentUserId)
               AppLib.AssignLogHeaderId(_id, _hrsEmploymentStatusActionLogDetailList)
               AppLib.CreateLogDetails(_hrsEmploymentStatusActionLogDetailList, "HrsEmploymentStatusActionLogDetail")

            End If

            '

            '
            If _removeDetailCount > 0 Then
               Me.DeleteHrsEmploymentStatusMembers(_hrsEmploymentStatusMemberListOld)

               For Each _old As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListOld
                  If _old.LogActionId = LogActionId.Delete Then
                     With _logKeyList
                        .Clear()
                        .Add("EmploymentStatusActionId", employmentStatusActionId)
                        .Add("MemberId", _old.MemberId)
                     End With

                     _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Delete, currentUserId)
                     _hrsEmploymentStatusMemberLogDetailList.Clear()

                            With _old

                                Dim _employmentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                                AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, String.Empty, .EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _employmentStatusName)
                                AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EffectivityDate, String.Empty, .EffectivityDate.ToDisplayFormat, .EffectivityDate.ToDisplayFormat)
                                AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EndDate, String.Empty, .EndDate.ToDisplayFormat, .EndDate.ToDisplayFormat)

                            End With
                            'With _old
                            '    Dim _workFormName As String = EasSession.DbsWorkForms.Rows.Find(Function(m) m.WorkFormId = .WorkFormId).WorkFormName
                            '    AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.WorkFormId, String.Empty, .WorkFormId.ToString, .WorkFormId.ToString + "=" + _workFormName)

                            'End With

                            AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")

                  End If
               Next

            End If
                File.WriteAllText("d:\act4.txt", action.ToString)
                If _addDetailCount > 0 Then

               Me.InsertHrsEmploymentStatusMembers(_hrsEmploymentStatusMemberListNew)

               For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListNew

                  With _logKeyList
                     .Clear()
                            .Add("EmploymentStatusActionId", _new.EmploymentStatusActionId)
                            .Add("MemberId", _new.MemberId)
                  End With

                  _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Add, currentUserId)

                  _hrsEmploymentStatusMemberLogDetailList.Clear()

                        With _new
                            Dim _employmentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                            AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, String.Empty, .EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _employmentStatusName)
                            AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EffectivityDate, String.Empty, .EffectivityDate.ToDisplayFormat, .EffectivityDate.ToDisplayFormat)
                        End With
                        'With _new
                        '    Dim _workFormName As String = EasSession.DbsWorkForms.Rows.Find(Function(m) m.WorkFormId = .WorkFormId).WorkFormName
                        '    AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.WorkFormId, String.Empty, .WorkFormId.ToString, .WorkFormId.ToString + "=" + _workFormName)

                        'End With
                    Next

               AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")


            End If



            If _editDetailCount > 0 Then

               Me.UpdateHrsEmploymentStatusMembers(_hrsEmploymentStatusMemberList)

               For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList
                  If _new.LogActionId = LogActionId.Edit Then
                     With _logKeyList
                        .Clear()
                        .Add("EmploymentStatusActionId", _new.EmploymentStatusActionId)
                        .Add("MemberId", _new.MemberId)
                     End With

                     _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Add, currentUserId)

                     _hrsEmploymentStatusMemberLogDetailList.Clear()

                     For Each _old As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListOld
                        With _new
                           If .EmploymentStatusId <> _old.EmploymentStatusId Then
                              Dim _oldEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = _old.EmploymentStatusId).EmploymentStatusName
                              Dim _newEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName

                              AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, .EmploymentStatusId.ToString, _old.EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _oldEmploymentStatusName + "; " + .EmploymentStatusId.ToString + "=" + _newEmploymentStatusName)
                           End If

                           If .EmploymentGUID <> _old.EmploymentGUID Then

                              'Upload File
                              RemoveEmploymentActionMemberFile(.MemberId, _old.EmploymentFileName, _old.EmploymentGUID)
                              UploadEmploymentActionMemberFile(.MemberId, .EmploymentFileName, .EmploymentGUID)
                           End If

                           If .EmploymentStatusId <> _old.EmploymentStatusId Then
                              Dim _oldEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = _old.EmploymentStatusId).EmploymentStatusName
                              Dim _newEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName

                              AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, .EmploymentStatusId.ToString, _old.EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _oldEmploymentStatusName + "; " + .EmploymentStatusId.ToString + "=" + _newEmploymentStatusName)
                           End If

                           If .EffectivityDate <> _old.EffectivityDate Then

                              AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EffectivityDate, .EffectivityDate.ToDisplayFormat, _old.EffectivityDate.ToDisplayFormat, .EffectivityDate.ToDisplayFormat)
                           End If

                           If .EndDate <> _old.EndDate Then

                              AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EndDate, .EndDate.ToDisplayFormat, _old.EndDate.ToDisplayFormat, .EndDate.ToDisplayFormat)
                           End If

                           'If .WorkFormId <> _old.WorkFormId Then

                           '   AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.WorkFormId, .EndDate.ToDisplayFormat, _old.EndDate.ToDisplayFormat, .EndDate.ToDisplayFormat)
                           'End If

                        End With
                     Next

                     'With _new
                     '    Dim _workFormName As String = EasSession.DbsWorkForms.Rows.Find(Function(m) m.WorkFormId = .WorkFormId).WorkFormName
                     '    AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.WorkFormId, String.Empty, .WorkFormId.ToString, .WorkFormId.ToString + "=" + _workFormName)

                     'End With
                  End If
               Next

               AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")


            End If

            '    If _editDetailCount > 0 Then
            '   Me.UpdateHrsEmploymentStatusMembers(_hrsEmploymentStatusMemberList)
            '   File.WriteAllText("d:\act5.txt", action.ToString)
            '   For Each _new As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberList
            '      If _new.LogActionId = LogActionId.Edit Then
            '         File.WriteAllText("d:\act51.txt", action.ToString)

            '         With _logKeyList
            '            .Clear()
            '            .Add("EmploymentStatusActionId", _new.EmploymentStatusActionId)
            '            .Add("MemberId", _new.MemberId)
            '         End With
            '         File.WriteAllText("d:\act52.txt", action.ToString)

            '         Try
            '            File.WriteAllText("d:\act53.txt", action.ToString)

            '            _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Edit, currentUserId)
            '            File.WriteAllText("d:\act54.txt", action.ToString)

            '         Catch ex As Exception
            '            File.WriteAllText("d:\act1111.txt", ex.InnerException.ToString)
            '         End Try

            '         _hrsEmploymentStatusMemberLogDetailList.Clear()
            '         File.WriteAllText("d:\jyl1.txt", "")
            '         For Each _old As HrsEmploymentStatusMember In _hrsEmploymentStatusMemberListOld
            '            If _new.EmploymentStatusActionId = _old.EmploymentStatusActionId Then
            '               File.WriteAllText("d:\jyl2.txt", "")

            '               With _new
            '                  File.WriteAllText("d:\jyl3.txt", "")

            '                  If .EmploymentGUID <> _old.EmploymentGUID Then

            '                     'Upload File
            '                     RemoveEmploymentActionMemberFile(.MemberId, _old.EmploymentFileName, _old.EmploymentGUID)
            '                     'File.WriteAllText("d:\RemoveClientContractFile.txt", contract.ToString)
            '                     UploadEmploymentActionMemberFile(.MemberId, .EmploymentFileName, .EmploymentGUID)
            '                     'File.WriteAllText("d:\UploadClientContractFile.txt", contract.ToString)
            '                     'Upload File

            '                     'AppLib.AddLogDetail(_hrsApplicantKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
            '                  End If
            '                  If .EmploymentStatusId <> _old.EmploymentStatusId Then
            '                     Dim _oldEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = _old.EmploymentStatusId).EmploymentStatusName
            '                     Dim _newEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName

            '                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, .EmploymentStatusId.ToString, _old.EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _oldEmploymentStatusName + "; " + .EmploymentStatusId.ToString + "=" + _newEmploymentStatusName)
            '                  End If

            '                  If .EffectivityDate <> _old.EffectivityDate Then

            '                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EffectivityDate, .EffectivityDate.ToDisplayFormat, _old.EffectivityDate.ToDisplayFormat, .EffectivityDate.ToDisplayFormat)
            '                  End If

            '                  File.WriteAllText("d:\jyl4.txt", "")

            '                  If .EndDate <> _old.EndDate Then

            '                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EndDate, .EndDate.ToDisplayFormat, _old.EndDate.ToDisplayFormat, .EndDate.ToDisplayFormat)
            '                  End If
            '                  File.WriteAllText("d:\jyl5.txt", "")

            '                  Try
            '                     AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")
            '                  Catch ex As Exception
            '                     File.WriteAllText("d:\act1111.txt", ex.InnerException.ToString)
            '                  End Try


            '               End With

            '            End If
            '         Next

            '      End If

            '   Next


            'End If

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

   <SymAuthorization("RemoveEmploymentStatusAction")>
   <Route("employment-status-action/{employmentStatusActionId}")>
   <HttpDelete>
   Public Function RemoveEmploymentStatusAction(employmentStatusActionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If employmentStatusActionId <= 0 Then
         Throw New ArgumentException("Employment Status Action ID is required.")
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

            Me.DeleteHrsEmploymentStatusAction(employmentStatusActionId, q.LockId)

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

   Private Sub LoadHrsEmploymentStatusAction(action As HrsEmploymentStatusActionBody, hrsEmploymentStatusAction As HrsEmploymentStatusAction)

      DataLib.ScatterValues(action, hrsEmploymentStatusAction)

   End Sub

   Private Sub LoadHrsEmploymentStatusAction(row As DataRow, action As HrsEmploymentStatusAction)

      With action
         .EmploymentStatusActionId = row.ToInt32("EmploymentStatusActionId")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub

   Private Sub LoadHrsEmploymentStatusMemberList(rows As DataRowCollection, list As HrsEmploymentStatusMemberList)

      Dim _detail As HrsEmploymentStatusMember
      For Each _row As DataRow In rows
         _detail = New HrsEmploymentStatusMember

         With _detail
            .EmploymentStatusActionId = _row.ToInt32("EmploymentStatusActionId")
            .EmploymentStatusMemberId = _row.ToInt32("EmploymentStatusMemberId")
            .MemberId = _row.ToInt32("MemberId")
                .EmploymentStatusId = _row.ToInt32("EmploymentStatusId")
                '.CurrentEmploymentStatusName = _row.ToString("CurrentEmploymentStatusName")
                .EffectivityDate = _row.ToDate("EffectivityDate")
                .EndDate = _row.ToDate("EndDate")
                .StatusActionId = _row.ToInt32("StatusActionId")
                .WorkFormId = _row.ToInt32("WorkFormId")
                .EmploymentFileName = _row.ToString("EmploymentFileName")
                .EmploymentGUID = _row.ToString("EmploymentGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertHrsEmploymentStatusAction(action As HrsEmploymentStatusAction)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsEmploymentStatusAction", action, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsEmploymentStatusAction(action)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertHrsEmploymentStatusMembers(list As HrsEmploymentStatusMemberList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
            .Add("EmploymentStatusMemberId")
            .Add("FileUrl") ' auto-assigned by DB
            .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsEmploymentStatusMember", list(0), _excludedFields)
         .CommandType = CommandType.Text

            For Each _detail As HrsEmploymentStatusMember In list
                Try
                    UploadMemberFile(_detail.MemberId, _detail.EmploymentFileName, _detail.EmploymentGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsEmploymentStatusMember(_detail)
                .ExecuteNonQuery()
            Next

        End With

   End Sub

   Private Sub UpdateHrsEmploymentStatusAction(action As HrsEmploymentStatusAction)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("EmploymentStatusActionId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsEmploymentStatusAction", action, _keyFields, _excludedFields)
            .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsEmploymentStatusAction(action)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(action.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateHrsEmploymentStatusMembers(list As HrsEmploymentStatusMemberList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("EmploymentStatusMemberId")
      End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsEmploymentStatusMember", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsEmploymentStatusMember In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsHrsEmploymentStatusMember(_detail)
                    .Parameters.AddWithValue("@EmploymentStatusMemberId", _detail.EmploymentStatusMemberId)

                    .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsHrsEmploymentStatusAction(action As HrsEmploymentStatusAction)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@EmploymentStatusActionId", action.EmploymentStatusActionId)
         .AddWithValue("@Remarks", action.Remarks)
      End With

    End Sub

   Private Sub AddInsertUpdateParamsHrsEmploymentStatusMember(dtl As HrsEmploymentStatusMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@EmploymentStatusActionId", dtl.EmploymentStatusActionId)
         .AddWithValue("@MemberId", dtl.MemberId)
         .AddWithValue("@EmploymentStatusId", dtl.EmploymentStatusId)
         .AddWithValue("@EffectivityDate", dtl.EffectivityDate)
            .AddWithValue("@EndDate", dtl.EndDate)
            '.AddWithValue("@CurrentEmploymentStatusName", dtl.CurrentEmploymentStatusName)
            .AddWithValue("@StatusActionId", dtl.StatusActionId)
            .AddWithValue("@WorkFormId", dtl.WorkFormId)
            .AddWithValue("@EmploymentFileName", dtl.EmploymentFileName)
            .AddWithValue("@EmploymentGUID", dtl.EmploymentGUID)
            .AddWithValue("@FileExtension", Path.GetExtension(dtl.EmploymentFileName.Replace("""", "")).ToLowerInvariant)
        End With

   End Sub

   Private Sub DeleteHrsEmploymentStatusAction(employmentStatusActionId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("EmploymentStatusActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsEmploymentStatusAction", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@EmploymentStatusActionId", employmentStatusActionId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteHrsEmploymentStatusMembers(list As HrsEmploymentStatusMemberList)

      With DataCore.Command
         .CommandText = "DELETE dbo.HrsEmploymentStatusMember WHERE EmploymentStatusMemberId=@EmploymentStatusMemberId"
         .CommandType = CommandType.Text

         For Each _old As HrsEmploymentStatusMember In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@EmploymentStatusMemberId", _old.EmploymentStatusMemberId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasHrsEmploymentStatusActionChanges(oldRecord As HrsEmploymentStatusAction, newRecord As HrsEmploymentStatusAction) As Boolean

      With oldRecord
         If .Remarks <> newRecord.Remarks Then Return True
      End With

      Return False

   End Function

    Friend Shared Function UploadMemberFile(memberId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _memberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant


            Dim _fileName As String = Path.Combine(_rootPath, guid + _extension)

            If Not Directory.Exists(_uploadRootPath) Then
                Directory.CreateDirectory(_uploadRootPath)
            End If

            If Not Directory.Exists(_memberFolder) Then
                Directory.CreateDirectory(_memberFolder)
            End If

            If File.Exists(_fileName) Then
                File.Move(_fileName, Path.Combine(_memberFolder, Path.GetFileName(_fileName)))
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function

    Friend Shared Function RemoveEmploymentActionMemberFile(memberId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant
            Dim _EmploymentActionMemberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())

            Dim _fileName As String = Path.Combine(_EmploymentActionMemberFolder, guid + _extension)

            If File.Exists(_fileName) Then
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function
    Friend Shared Function UploadEmploymentActionMemberFile(memberId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _EmploymentActionMemberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant


            Dim _fileName As String = Path.Combine(_rootPath, guid + _extension)

            If Not Directory.Exists(_uploadRootPath) Then
                Directory.CreateDirectory(_uploadRootPath)
            End If

            If Not Directory.Exists(_EmploymentActionMemberFolder) Then
                Directory.CreateDirectory(_EmploymentActionMemberFolder)
            End If

            If File.Exists(_fileName) Then
                File.Move(_fileName, Path.Combine(_EmploymentActionMemberFolder, Path.GetFileName(_fileName)))
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function
    <SymAuthorization("UploadEmploymentActionMemberFile")>
    <Route("employment-status-actions/{memberId}/{currentUserId}/{guid}/files")>
    <HttpPost>
    Public Async Function UploadEmploymentActionMemberFile(memberId As Integer, currentUserId As Integer, guid As String, <FromUri> q As UploadDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

        If Not Me.Request.Content.IsMimeMultipartContent() Then
            Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
        End If
        Try
            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")
            Dim _provider As New MultipartFormDataStreamProvider(_rootPath)

            If Not Directory.Exists(_rootPath) Then
                Directory.CreateDirectory(_rootPath)
            End If

            Await Request.Content.ReadAsMultipartAsync(_provider)

            Dim _fileName As String
            Dim _extension As String
            Dim _targetFileName As String
            Dim _detail As FileUploadDetaii
            Dim _detailList As New FileUploadDetaiiList
            Dim _uploadResponse As New FileUploadResponse
            Dim _createdCount As Integer
            Dim _failedCount As Integer
            Dim _imageInfo As MagickImageInfo
            Dim _isFileValid As Boolean
            Dim _pageCount As Integer

            For Each _file As MultipartFileData In _provider.FileData
                _fileName = guid '_file.Headers.ContentDisposition.FileName.Replace("""", "")    ' removes double quotes in string
                _extension = Path.GetExtension(_file.Headers.ContentDisposition.FileName.Replace("""", "")).ToLowerInvariant
                '_targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_file.LocalFileName).Replace("BodyPart_", "") + _extension)
                _targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), _fileName + _extension)
                _isFileValid = False
                _pageCount = 0

                _detail = New FileUploadDetaii
                _detail.FileName = _file.Headers.ContentDisposition.FileName.Replace("""", "")
                _detailList.Add(_detail)

                _imageInfo = ImageLib.GetFileInfo(_file.LocalFileName)

                File.Move(_file.LocalFileName, _targetFileName)
                _isFileValid = True

                _detail.StatusCode = 201
                _detail.StatusText = "Uploaded"
                _createdCount = _createdCount + 1

            Next

            With _uploadResponse
                .CreatedCount = _createdCount
                .FailedCount = _failedCount
                .Details = _detailList
            End With

            Return Me.Ok(_uploadResponse)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function
End Class

Public Class HrsEmploymentStatusActionBody
   Inherits HrsEmploymentStatusAction

   Public Property Members As HrsEmploymentStatusMember()

End Class
