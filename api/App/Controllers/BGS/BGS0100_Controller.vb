<RoutePrefix("api")>
Public Class BGS0100_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   '<SymAuthorization("GetActivity")>
   <Route("paps/{activityId}")>
   <HttpGet>
   Public Function GetActivity(activityId As Integer) As IHttpActionResult

      If activityId <= 0 Then
         Throw New ArgumentException("Activity ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0100")
            With _direct
               .AddParameter("ActivityId", activityId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GeActivityLog")>
   <Route("paps/{activityId}/log")>
   <HttpGet>
   Public Function GeActivityLog(activityId As Integer) As IHttpActionResult

      If activityId <= 0 Then
         Throw New ArgumentException("Activity ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0100_Log")
            With _direct
               .AddParameter("ActivityId", activityId)

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

   '<SymAuthorization("CreateActivity")>
   '<Route("paps")>
   '<HttpPost>
   'Public Function CreateActivity(<FromBody> activity As ActivityBody) As IHttpActionResult

   '   If activity.ActivityId <= 0 Then
   '      Throw New ArgumentException("Activity ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '
   '      Dim _bgsActivity As New BgsActivity

   '      Me.LoadBgsActivity(activity, _bgsActivity)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.InsertBgsActivity(_bgsActivity)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("CreateActivity")>
   <Route("paps/{currentUserId}")>
   <HttpPost>
   Public Function CreateActivity(currentUserId As Integer, <FromBody> activity As ActivityBody) As IHttpActionResult

      If activity.ActivityId <= 0 Then
         Throw New ArgumentException("Activity ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsActivity As New BgsActivity

         Me.LoadBgsActivity(activity, _bgsActivity)

         '
         ' Log addition, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsActivityLogDetailList As New SysLogDetailList

         With _bgsActivity
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityName, String.Empty, .ActivityName)
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityShortName, String.Empty, .ActivityShortName)
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.PapCode, String.Empty, .PapCode)

            If .HeaderActivityId > 0 Then
               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.HeaderActivityId, String.Empty, .HeaderActivityId.ToString)
            End If

            Dim _flag As String
            If .ExcludeFlag Then
               _flag = "Yes"
            Else
               _flag = "No"
            End If

            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ExcludeFlag, String.Empty, _flag)

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

            Me.InsertBgsActivity(_bgsActivity)

            If _bgsActivityLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ActivityId", _bgsActivity.ActivityId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsActivityLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsActivityLogDetailList)

               AppLib.CreateLogDetails(_bgsActivityLogDetailList, "BgsActivityLogDetail")

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

   '<SymAuthorization("ModifyActivity")>
   '<Route("paps/{activityId}")>
   '<HttpPut>
   'Public Function ModifyActivity(activityId As Integer, <FromBody> activity As ActivityBody) As IHttpActionResult

   '   If activityId <= 0 Then
   '      Throw New ArgumentException("Activity ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '
   '      Dim _bgsActivity As New BgsActivity

   '      Me.LoadBgsActivity(activity, _bgsActivity)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.UpdateBgsActivity(_bgsActivity)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function

   <SymAuthorization("ModifyActivity")>
   <Route("paps/{activityId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyActivity(activityId As Integer, currentUserId As Integer, <FromBody> activity As ActivityBody) As IHttpActionResult

      If activityId <= 0 Then
         Throw New ArgumentException("Activity ID is required.")
      End If

      Try
         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsActivity As New BgsActivity

         Me.LoadBgsActivity(activity, _bgsActivity)

         '
         ' Load old values from DB
         '
         Dim _bgsActivityOld As New BgsActivity

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetActivity(activityId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsActivity(_row, _bgsActivityOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsActivityLogDetailList As New SysLogDetailList

         With _bgsActivityOld
            If .ActivityName <> _bgsActivity.ActivityName Then
               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityName, .ActivityName, _bgsActivity.ActivityName)
            End If

            If .ActivityShortName <> _bgsActivity.ActivityShortName Then
               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityShortName, .ActivityShortName, _bgsActivity.ActivityShortName)
            End If

            If .PapCode <> _bgsActivity.PapCode Then
               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.PapCode, .PapCode, _bgsActivity.PapCode)
            End If

            If .HeaderActivityId <> _bgsActivity.HeaderActivityId Then
               Dim _oldId As String
               Dim _newId As String

               If .HeaderActivityId = 0 Then
                  _oldId = String.Empty
               Else
                  _oldId = .HeaderActivityId.ToString
               End If

               If _bgsActivity.HeaderActivityId = 0 Then
                  _newId = String.Empty
               Else
                  _newId = _bgsActivity.HeaderActivityId.ToString
               End If

               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.HeaderActivityId, _oldId, _newId)
            End If

            If .ExcludeFlag <> _bgsActivity.ExcludeFlag Then
               Dim _oldFlag As String
               Dim _newFlag As String

               If .ExcludeFlag Then
                  _oldFlag = "Yes"
               Else
                  _oldFlag = "No"
               End If

               If _bgsActivity.ExcludeFlag Then
                  _newFlag = "Yes"
               Else
                  _newFlag = "No"
               End If

               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ExcludeFlag, _oldFlag, _newFlag)
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

            Me.UpdateBgsActivity(_bgsActivity)

            If _bgsActivityLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ActivityId", _bgsActivity.ActivityId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsActivityLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsActivityLogDetailList)

               AppLib.CreateLogDetails(_bgsActivityLogDetailList, "BgsActivityLogDetail")

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

   '<SymAuthorization("RemoveActivity")>
   '<Route("paps/{activityId}/{lockId}")>
   '<HttpDelete>
   'Public Function RemoveActivity(activityId As Integer, lockId As String) As IHttpActionResult

   '   If activityId <= 0 Then
   '      Throw New ArgumentException("Activity ID is required.")
   '   End If

   '   Try
   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.DeleteBgsActivity(activityId, lockId)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("RemoveActivity")>
   <Route("paps/{activityId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveActivity(activityId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If activityId <= 0 Then
         Throw New ArgumentException("Activity ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Check for existing record
         '
         Dim _filter As String = "ActivityId=" + activityId.ToString
         If DataLib.GetRecordCount("dbo.BgsActivity", _filter, _databaseId) = 0 Then
            Throw New ArgumentException("Activity ID not found.")
         End If

         '
         ' Load current values from DB
         '
         Dim _bgsActivity As New BgsActivity

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetActivity(activityId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsActivity(_row, _bgsActivity)
         End Using

         '
         ' Log deletion, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsActivityLogDetailList As New SysLogDetailList

         With _bgsActivity
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityName, .ActivityName, String.Empty)
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ActivityShortName, .ActivityShortName, String.Empty)
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.PapCode, .PapCode, String.Empty)

            If .HeaderActivityId > 0 Then
               AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.HeaderActivityId, .HeaderActivityId.ToString, String.Empty)
            End If

            Dim _flag As String
            If .ExcludeFlag Then
               _flag = "Yes"
            Else
               _flag = "No"
            End If
            AppLib.AddLogDetail(_bgsActivityLogDetailList, 0, LogColumnId.ExcludeFlag, _flag, String.Empty)

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

            Me.DeleteBgsActivity(activityId, lockId)

            If _bgsActivityLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ActivityId", _bgsActivity.ActivityId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsActivityLog", _logKeyList, LogActionId.Delete, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsActivityLogDetailList)

               AppLib.CreateLogDetails(_bgsActivityLogDetailList, "BgsActivityLogDetail")

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

   Private Sub LoadBgsActivity(activity As ActivityBody, bgsActivity As BgsActivity)

      DataLib.ScatterValues(activity, bgsActivity)

   End Sub

   Private Sub LoadBgsActivity(row As DataRow, bgsActivity As BgsActivity)

      With bgsActivity
         .ActivityId = row.ToInt32("ActivityId")
         .ActivityName = row.ToString("ActivityName")
         .ActivityShortName = row.ToString("ActivityShortName")
         .ActivityTitle = row.ToString("ActivityTitle")
         .PapCode = row.ToString("PapCode")
         .HeaderActivityId = row.ToInt32("HeaderActivityId")
         .ExcludeFlag = row.ToBoolean("ExcludeFlag")
      End With

   End Sub

   Private Sub InsertBgsActivity(activity As BgsActivity)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsActivity", activity, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsActivity(activity)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsActivity(activity As BgsActivity)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ActivityId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsActivity", activity, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsActivity(activity)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(activity.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsActivity(activity As BgsActivity)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ActivityId", activity.ActivityId)
         .AddWithValue("@ActivityName", activity.ActivityName)
         .AddWithValue("@ActivityShortName", activity.ActivityShortName)
         .AddWithValue("@ActivityTitle", activity.ActivityTitle.ToNullable)
         .AddWithValue("@PapCode", activity.PapCode)
         .AddWithValue("@HeaderActivityId", activity.HeaderActivityId.ToNullable)
         .AddWithValue("@ExcludeFlag", activity.ExcludeFlag)

      End With

   End Sub

   Private Sub DeleteBgsActivity(activityId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ActivityId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsActivity", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ActivityId", activityId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ActivityBody
   Inherits BgsActivity

End Class
