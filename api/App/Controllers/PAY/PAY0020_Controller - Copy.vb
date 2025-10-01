
Imports ImageMagick

<RoutePrefix("api")>
Public Class PAY0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_PAY0020")>
   <Route("references/pay0020")>
   <HttpGet>
   Public Function GetReferences_PAY0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.PAY0010_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "schedule"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTimekeepingSchedule")>
   <Route("timekeeping-schedules/{timekeepingId}")>
   <HttpGet>
   Public Function GetTimekeepingSchedule(timekeepingId As Integer) As IHttpActionResult

      If timekeepingId <= 0 Then
         Throw New ArgumentException("Timekeeping ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.PAY0020")
            With _direct
               .AddParameter("TimekeepingId", timekeepingId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "timekeeping"
                     .Tables(1).TableName = "schedule"
                     .Tables(2).TableName = "member"
                     .Tables(3).TableName = "scheduleList"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   '<SymAuthorization("CreateTimekeepingSchedule")>
   '<Route("timekeeping-schedules/{currentUserId}")>
   '<HttpPost>
   'Public Function CreateTimekeepingSchedule(currentUserId As Integer, <FromBody> timekeeping As TimekeepingBody) As IHttpActionResult

   '   If timekeeping.TimekeepingId <> -1 Then
   '      Throw New ArgumentException("Timekeeping ID is unrecognized.")
   '   End If

   '   Try

   '      '
   '      ' Assign new ID from sequencer
   '      '
   '      Dim _timekeepingId As Integer = SysLib.GetNextSequence("TimekeepingId")

   '      timekeeping.TimekeepingId = _timekeepingId

   '      '
   '      ' Load proposed values from payload
   '      '

   '      Dim _payTimekeeping As New PayTimekeeping
   '      Dim _payTimekeepingRestDayList As New PayTimekeepingRestDayList


   '      Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


   '      For Each _restDay As PayTimekeepingRestDay In timekeeping.RestDayList
   '         _restDay.TimekeepingId = _timekeepingId
   '         _payTimekeepingRestDayList.Add(_restDay)
   '      Next


   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.InsertPayTimekeeping(_payTimekeeping)


   '         If _payTimekeepingRestDayList.Count > 0 Then
   '            Me.InsertPayTimekeepingRestDays(_payTimekeepingRestDayList)
   '         End If

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

   '      Return Me.Ok(timekeeping.TimekeepingId)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("ModifyTimekeepingSchedule")>
   <Route("timekeeping-schedules/{timekeepingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeepingSchedule(timekeepingId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingScheduleBody) As IHttpActionResult

      If timekeepingId <= 0 Then
         Throw New ArgumentException("Timekeeping ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeeping
         Dim _payTimekeepingScheduleMemberList As New PayTimekeepingScheduleMemberList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingScheduleMember In timekeeping.Schedules
            _payTimekeepingScheduleMemberList.Add(_schedule)
         Next

         '
         ' Load old values from DB
         '
         Dim _payTimekeepingOld As New PayTimekeeping
         Dim _payTimekeepingScheduleMemberListOld As New PayTimekeepingScheduleMemberList


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeepingSchedule(timekeepingId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("timekeeping").Rows(0)
            Me.LoadPayTimekeeping(_row, _payTimekeepingOld)
            Me.LoadPayTimekeepingScheduleMemberList(_dataSet.Tables("schedules").Rows, _payTimekeepingScheduleMemberListOld)
         End Using



#Region "PayTimekeepingScheduleMember"

         '
         ' Apply changes, save to DB
         '

         Dim _removeScheduleMemberCount As Integer
         Dim _addScheduleMemberCount As Integer
         Dim _editScheduleMemberCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingScheduleMember In _payTimekeepingScheduleMemberListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingScheduleMember In _payTimekeepingScheduleMemberList
               If _new.ScheduleMemberId = _old.ScheduleMemberId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeScheduleMemberCount = _removeScheduleMemberCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingScheduleMember In _payTimekeepingScheduleMemberList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingScheduleMember In _payTimekeepingScheduleMemberListOld
               If _new.ScheduleMemberId = _old.ScheduleMemberId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MemberId <> _old.MemberId OrElse .ScheduleDate <> _old.ScheduleDate OrElse .TimekeepingScheduleId <> _old.TimekeepingScheduleId OrElse .TimekeepingScheduleIdA <> _old.TimekeepingScheduleIdA OrElse .TimekeepingScheduleIdB <> _old.TimekeepingScheduleIdB OrElse .TimekeepingScheduleIdC <> _old.TimekeepingScheduleIdC OrElse .TimekeepingScheduleIdD <> _old.TimekeepingScheduleIdD Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addScheduleMemberCount = _addScheduleMemberCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editScheduleMemberCount = _editScheduleMemberCount + 1
            End If

         Next

         Dim _payTimekeepingScheduleMemberListNew As New PayTimekeepingScheduleMemberList      ' for adding new Barangays

         If _addScheduleMemberCount > 0 Then
            Dim _payTimekeepingScheduleMember As PayTimekeepingScheduleMember

            For Each _new As PayTimekeepingScheduleMember In _payTimekeepingScheduleMemberList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingScheduleMember = New PayTimekeepingScheduleMember
                  _payTimekeepingScheduleMemberListNew.Add(_payTimekeepingScheduleMember)
                  DataLib.ScatterValues(_new, _payTimekeepingScheduleMember)
               End If
            Next

         End If


#End Region




         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdatePayTimekeeping(_payTimekeeping)


            If _removeScheduleMemberCount > 0 Then
               Me.DeletePayTimekeepingScheduleMembers(_payTimekeepingScheduleMemberListOld)
            End If

            If _addScheduleMemberCount > 0 Then
               Me.InsertPayTimekeepingScheduleMembers(_payTimekeepingScheduleMemberListNew)
            End If

            If _editScheduleMemberCount > 0 Then
               Me.UpdatePayTimekeepingScheduleMembers(_payTimekeepingScheduleMemberList)
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

   <SymAuthorization("RemoveTimekeeping")>
   <Route("timekeepings/{timekeepingId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveTimekeeping(timekeepingId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If timekeepingId <= 0 Then
         Throw New ArgumentException("Timekeeping ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
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

            Me.DeletePayTimekeeping(timekeepingId, lockId)

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
   Private Sub LoadPayTimekeeping(timekeeping As TimekeepingScheduleBody, payTimekeeping As PayTimekeeping)

      DataLib.ScatterValues(timekeeping, payTimekeeping)

   End Sub
   Private Sub LoadPayTimekeeping(row As DataRow, timekeeping As PayTimekeeping)

      With timekeeping
         .TimekeepingId = row.ToInt32("TimekeepingId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .MinimumRequiredOvertimeCount = row.ToInt32("MinimumRequiredOvertimeCount")
         .MinimumRequiredOvertimeUnitId = row.ToInt32("MinimumRequiredOvertimeUnitId")
         .OvertimeDurationCount = row.ToInt32("OvertimeDurationCount")
         .OvertimeDurationUnitId = row.ToInt32("OvertimeDurationUnitId")
         .OvertimeCountId = row.ToInt32("OvertimeCountId")
         .OvertimeStartDate = row.ToDate("OvertimeStartDate")
         .OvertimeEndDate = row.ToDate("OvertimeEndDate")
         .MaximumRegularHourCount = row.ToInt32("MaximumRegularHourCount")
         .MaximumRegularHourUnitId = row.ToInt32("MaximumRegularHourUnitId")
         .LateGracePeriodCount = row.ToInt32("LateGracePeriodCount")
         .LateGracePeriodUnitId = row.ToInt32("LateGracePeriodUnitId")
         .LateCount = row.ToInt32("LateCount")
         .LateUnitId = row.ToInt32("LateUnitId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .PaidUpHolidayId = row.ToInt32("PaidUpHolidayId")
         .NightDifferentialCountId = row.ToInt32("NightDifferentialCountId")
      End With

   End Sub
   Private Sub LoadPayTimekeepingRestDayList(rows As DataRowCollection, list As PayTimekeepingRestDayList)

      Dim _detail As PayTimekeepingRestDay
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingRestDay

         With _detail
            .TimekeepingRestDayId = _row.ToInt32("TimekeepingRestDayId")
            .TimekeepingId = _row.ToInt32("TimekeepingId")
            .RestDayId = _row.ToInt32("RestDayId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadPayTimekeepingScheduleMemberList(rows As DataRowCollection, list As PayTimekeepingScheduleMemberList)

      Dim _detail As PayTimekeepingScheduleMember
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingScheduleMember

         With _detail
            .ScheduleMemberId = _row.ToInt32("ScheduleMemberId")
            .TimekeepingId = _row.ToInt32("TimekeepingId")
            .MemberId = _row.ToInt32("MemberId")
            .ScheduleDate = _row.ToDate("ScheduleDate")
            .TimekeepingScheduleId = _row.ToInt32("TimekeepingScheduleId")
            .TimekeepingScheduleIdA = _row.ToInt32("TimekeepingScheduleIdA")
            .TimekeepingScheduleIdB = _row.ToInt32("TimekeepingScheduleIdB")
            .TimekeepingScheduleIdC = _row.ToInt32("TimekeepingScheduleIdC")
            .TimekeepingScheduleIdD = _row.ToInt32("TimekeepingScheduleIdD")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertPayTimekeeping(timekeeping As PayTimekeeping)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeeping", timekeeping, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertPayTimekeepingRestDays(list As PayTimekeepingRestDayList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TimekeepingRestDayId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingRestDay", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingRestDay In list
            Me.AddInsertUpdateParamsPayTimekeepingRestDay(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertPayTimekeepingScheduleMembers(list As PayTimekeepingScheduleMemberList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ScheduleMemberId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingScheduleMember", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingScheduleMember In list
            Me.AddInsertUpdateParamsPayTimekeepingScheduleMember(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdatePayTimekeeping(timekeeping As PayTimekeeping)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingId")
         .Add("LockId")
      End With

      'With _excludedFields
      '   .Add("Age")
      '   '.Add("Address2")
      'End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeeping", timekeeping, _keyFields) ', _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(timekeeping.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsPayTimekeeping(timekeeping As PayTimekeeping)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingId", timekeeping.TimekeepingId)
         .AddWithValue("@MemberRequestId", timekeeping.MemberRequestId)

         .AddWithValue("@MinimumRequiredOvertimeCount", timekeeping.MinimumRequiredOvertimeCount)

         .AddWithValue("@MinimumRequiredOvertimeUnitId", timekeeping.MinimumRequiredOvertimeUnitId)
         .AddWithValue("@OvertimeDurationCount", timekeeping.OvertimeDurationCount)

         .AddWithValue("@OvertimeDurationUnitId", timekeeping.OvertimeDurationUnitId)
         .AddWithValue("@OvertimeCountId", timekeeping.OvertimeCountId)

         .AddWithValue("@OvertimeStartDate", timekeeping.OvertimeStartDate)
         .AddWithValue("@OvertimeEndDate", timekeeping.OvertimeEndDate)

         .AddWithValue("@MaximumRegularHourCount", timekeeping.MaximumRegularHourCount)
         .AddWithValue("@MaximumRegularHourUnitId", timekeeping.MaximumRegularHourUnitId)

         .AddWithValue("@LateGracePeriodCount", timekeeping.LateGracePeriodCount)
         .AddWithValue("@LateGracePeriodUnitId", timekeeping.LateGracePeriodUnitId)

         .AddWithValue("@LateCount", timekeeping.LateCount)
         .AddWithValue("@LateUnitId", timekeeping.LateUnitId)

         .AddWithValue("@PaidUpHolidayId", timekeeping.PaidUpHolidayId)
         .AddWithValue("@NightDifferentialCountId", timekeeping.NightDifferentialCountId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingScheduleMember(dtl As PayTimekeepingScheduleMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingId", dtl.TimekeepingId)
         .AddWithValue("@MemberId", dtl.MemberId)
         .AddWithValue("@ScheduleDate", dtl.ScheduleDate)
         .AddWithValue("@TimekeepingScheduleId", dtl.TimekeepingScheduleId)
         .AddWithValue("@TimekeepingScheduleIdA", dtl.TimekeepingScheduleIdA)
         .AddWithValue("@TimekeepingScheduleIdB", dtl.TimekeepingScheduleIdB)
         .AddWithValue("@TimekeepingScheduleIdC", dtl.TimekeepingScheduleIdC)
         .AddWithValue("@TimekeepingScheduleIdD", dtl.TimekeepingScheduleIdD)

      End With

   End Sub

   Private Sub UpdatePayTimekeepingRestDays(list As PayTimekeepingRestDayList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingRestDayId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingRestDay", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingRestDay In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingRestDay(_detail)
               .Parameters.AddWithValue("@TimekeepingRestDayId", _detail.TimekeepingRestDayId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdatePayTimekeepingScheduleMembers(list As PayTimekeepingScheduleMemberList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ScheduleMemberId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingScheduleMember", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingScheduleMember In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingScheduleMember(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   'Private Sub UpdatePayTimekeepingSchedules(list As PayTimekeepingScheduleList)

   '   Dim _keyFields As New List(Of String)
   '   Dim _excludedFields As New List(Of String)

   '   With _keyFields
   '      .Add("TimekeepingScheduleId")
   '   End With

   '   With _excludedFields
   '      .Add("LogActionId")
   '      .Add("LockId")
   '   End With

   '   With DataCore.Command
   '      .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingSchedule", list(0), _keyFields, _excludedFields)
   '      .CommandType = CommandType.Text

   '      For Each _detail As PayTimekeepingSchedule In list
   '         If _detail.LogActionId = LogActionId.Edit Then
   '            Me.AddInsertUpdateParamsPayTimekeepingSchedule(_detail)

   '            .ExecuteNonQuery()
   '         End If
   '      Next

   '   End With

   'End Sub
   Private Sub AddInsertUpdateParamsPayTimekeepingRestDay(dtl As PayTimekeepingRestDay)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@TimekeepingId", dtl.TimekeepingId)
         .AddWithValue("@RestDayId", dtl.RestDayId)
      End With

   End Sub

   Private Sub DeletePayTimekeeping(timekeepingId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("timekeepingId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.PayTimekeeping", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@TimekeepingId", timekeepingId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub DeletePayTimekeepingRestDays(list As PayTimekeepingRestDayList)

      With DataCore.Command
         .CommandText = "DELETE dbo.PayTimekeepingRestDay WHERE TimekeepingRestDayId=@TimekeepingRestDayId"
         .CommandType = CommandType.Text

         For Each _old As PayTimekeepingRestDay In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TimekeepingRestDayId", _old.TimekeepingRestDayId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeletePayTimekeepingScheduleMembers(list As PayTimekeepingScheduleMemberList)

      With DataCore.Command
         .CommandText = "DELETE dbo.PayTimekeepingScheduleMember WHERE ScheduleMemberId=@ScheduleMemberId"
         .CommandType = CommandType.Text

         For Each _old As PayTimekeepingScheduleMember In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ScheduleMemberId", _old.ScheduleMemberId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

End Class
Public Class TimekeepingScheduleBody
   Inherits PayTimekeeping

   Public Property Schedules As PayTimekeepingScheduleMember()

End Class

