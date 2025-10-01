<RoutePrefix("api")>
Public Class HRS0060_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetEmploymentStatusMemberLog")>
   <Route("employment-status-members/{employmentStatusActionId}/{memberId}/log")>
   <HttpGet>
   Public Function GetEmploymentStatusMemberLog(employmentStatusActionId As Integer, MemberId As Integer) As IHttpActionResult

      If employmentStatusActionId <= 0 Then
         Throw New ArgumentException("Employment Status Action ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("Web.HRS0060_Log")
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


   <SymAuthorization("GetEmploymentStatusMemberActions")>
   <Route("employment-status-member-actions")>
   <HttpGet>
   Public Function GetEmploymentStatusMemberActions() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.HRS0060_All")
            With _direct
               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetEmploymentStatusMemberAction")>
   <Route("employment-status-member-actions/{employmentStatusMemberId}")>
   <HttpGet>
   Public Function GetEmploymentStatusMemberAction(employmentStatusMemberId As Integer) As IHttpActionResult

      If employmentStatusMemberId <= 0 Then
         Throw New ArgumentException("Employment Status Member ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.HRS0060")
            With _direct
               .AddParameter("employmentStatusMemberId", employmentStatusMemberId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyEmploymentStatusMemberAction")>
   <Route("employment-status-member-actions/{employmentStatusMemberId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyEmploymentStatusMemberAction(employmentStatusMemberId As Integer, currentUserId As Integer, <FromBody> action As HrsEmploymentStatusActionMemberBody) As IHttpActionResult

      If employmentStatusMemberId <= 0 Then
         Throw New ArgumentException("Employment Status Member ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _hrsEmploymentStatusActionMember As New HrsEmploymentStatusActionMember

         Me.LoadHrsEmploymentStatusActionMember(action, _hrsEmploymentStatusActionMember)


         '
         ' Load old values from DB
         '
         Dim _hrsEmploymentStatusActionMemberOld As New HrsEmploymentStatusActionMember

         'Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetEmploymentStatusMemberAction(employmentStatusMemberId), Results.OkNegotiatedContentResult(Of DataSet))

         Dim _resultLog As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEmploymentStatusMemberAction(employmentStatusMemberId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _resultLog.Content
            'With _dataTable.Rows(0)
            Me.LoadHrsEmploymentStatusActionMember(_dataTable.Rows(0), _hrsEmploymentStatusActionMemberOld)

            'End With
         End Using


         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' HrsApplicant
         '

         Dim _hrsEmploymentStatusMemberLogDetailList As New SysLogDetailList

         With _hrsEmploymentStatusActionMemberOld


            'If .EmploymentStatusId <> _hrsEmploymentStatusActionMember.EmploymentStatusId Then
            '   Dim _employmentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
            '   AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.EmploymentStatusId, String.Empty, .EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _employmentStatusName)
            'End If

            If .StatusActionId <> _hrsEmploymentStatusActionMember.StatusActionId Then
               Select Case _hrsEmploymentStatusActionMember.StatusActionId
                  Case 1 'Pending
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.StatusActionId, .StatusActionId.ToString, _hrsEmploymentStatusActionMember.StatusActionId.ToString, .StatusActionId.ToString + "=" + "Pending; " + _hrsEmploymentStatusActionMember.StatusActionId.ToString + "=" + "Pending")
                  Case 2 'Approved
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.StatusActionId, .StatusActionId.ToString, _hrsEmploymentStatusActionMember.StatusActionId.ToString, .StatusActionId.ToString + "=" + "Pending; " + _hrsEmploymentStatusActionMember.StatusActionId.ToString + "=" + "Approved")
                  Case 3 'Declined
                     AppLib.AddLogDetail(_hrsEmploymentStatusMemberLogDetailList, _id, LogColumnId.StatusActionId, .StatusActionId.ToString, _hrsEmploymentStatusActionMember.StatusActionId.ToString, .StatusActionId.ToString + "=" + "Pending; " + _hrsEmploymentStatusActionMember.StatusActionId.ToString + "=" + "Declined")
               End Select

            End If

         End With




         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateHrsEmploymentStatusActionMember(_hrsEmploymentStatusActionMember)

            If _hrsEmploymentStatusMemberLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("EmploymentStatusActionId", _hrsEmploymentStatusActionMemberOld.EmploymentStatusActionId)
                  .Add("MemberId", _hrsEmploymentStatusActionMemberOld.MemberId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsEmploymentStatusMemberLog", _logKeyList, LogActionId.Add, currentUserId)
               AppLib.AssignLogHeaderId(_id, _hrsEmploymentStatusMemberLogDetailList)
               AppLib.CreateLogDetails(_hrsEmploymentStatusMemberLogDetailList, "HrsEmploymentStatusMemberLogDetail")

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEmploymentStatusMemberAction(employmentStatusMemberId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception

         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   Private Sub LoadHrsEmploymentStatusActionMember(action As HrsEmploymentStatusActionMemberBody, HrsEmploymentStatusActionMember As HrsEmploymentStatusActionMember)

      DataLib.ScatterValues(action, HrsEmploymentStatusActionMember)

   End Sub

   Private Sub LoadHrsEmploymentStatusActionMember(row As DataRow, hrsEmploymentStatusActionMember As HrsEmploymentStatusActionMember)

      With hrsEmploymentStatusActionMember
         .EmploymentStatusMemberId = row.ToInt32("EmploymentStatusMemberId")
         .StatusActionId = row.ToInt32("StatusActionId")
         .MemberId = row.ToInt32("MemberId")
         .EmploymentStatusActionId = row.ToInt32("EmploymentStatusActionId")
      End With

   End Sub

   Private Sub UpdateHrsEmploymentStatusActionMember(action As HrsEmploymentStatusActionMember)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("EmploymentStatusMemberId")
         .Add("LockId")
      End With


      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberId")
         .Add("EmploymentStatusActionId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsEmploymentStatusMember", action, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsEmploymentStatusActionMember(action)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(action.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsHrsEmploymentStatusActionMember(action As HrsEmploymentStatusActionMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@EmploymentStatusMemberId", action.EmploymentStatusMemberId)
         .AddWithValue("@StatusActionId", action.StatusActionId)

      End With

   End Sub

End Class

Public Class HrsEmploymentStatusActionMemberBody
   Inherits HrsEmploymentStatusActionMember

End Class
