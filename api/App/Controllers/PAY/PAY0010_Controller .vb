
Imports ImageMagick

<RoutePrefix("api")>
Public Class PAY0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_PAY0010")>
   <Route("references/pay0010")>
   <HttpGet>
   Public Function GetReferences_PAY0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.PAY0010_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "minimumRequiredOvertimeUnit"
                     .Tables(1).TableName = "overtimeDurationUnit"
                     .Tables(2).TableName = "overtimeCount"
                     .Tables(3).TableName = "maximumRegularHourUnit"
                     .Tables(4).TableName = "lateGracePeriodUnit"
                     .Tables(5).TableName = "lateUnit"
                     .Tables(6).TableName = "paidUpHoliday"
                     .Tables(7).TableName = "nightDifferentialCount"
                     .Tables(8).TableName = "restDay"
                     .Tables(9).TableName = "lateCount"
                     .Tables(10).TableName = "holidayCount"
                     .Tables(11).TableName = "undertimeCount"
                     '.Tables(8).TableName = "NightDifferentialCount"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTimekeeping")>
   <Route("timekeepings/{timekeepingId}")>
   <HttpGet>
   Public Function GetTimekeeping(timekeepingId As Integer) As IHttpActionResult

      If timekeepingId <= 0 Then
         Throw New ArgumentException("Timekeeping ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.PAY0010")
            With _direct
               .AddParameter("TimekeepingId", timekeepingId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "timekeeping"
                     .Tables(1).TableName = "restDayList"
                     .Tables(2).TableName = "schedule"
                     .Tables(3).TableName = "lateMatrix"

                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTimekeeping")>
   <Route("timekeepings/{currentUserId}")>
   <HttpPost>
   Public Function CreateTimekeeping(currentUserId As Integer, <FromBody> timekeeping As TimekeepingBody) As IHttpActionResult

      If timekeeping.TimekeepingId <> -1 Then
         Throw New ArgumentException("Timekeeping ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _timekeepingId As Integer = SysLib.GetNextSequence("TimekeepingId")

         timekeeping.TimekeepingId = _timekeepingId
         timekeeping.PolicyStatusId = 2 'In-Active
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeeping
         Dim _payTimekeepingRestDayList As New PayTimekeepingRestDayList
         Dim _payTimekeepingScheduleList As New PayTimekeepingScheduleList
         Dim _payTimekeepingLateMatrixList As New PayTimekeepingLateMatrixList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _restDay As PayTimekeepingRestDay In timekeeping.RestDayList
            _restDay.TimekeepingId = _timekeepingId
            _payTimekeepingRestDayList.Add(_restDay)
         Next

         For Each _schedule As PayTimekeepingSchedule In timekeeping.Schedules
            _schedule.TimekeepingId = _timekeepingId
            _payTimekeepingScheduleList.Add(_schedule)
         Next

         For Each _lateMatrix As PayTimekeepingLateMatrix In timekeeping.LateMatrix
            _lateMatrix.TimekeepingId = _timekeepingId
            _payTimekeepingLateMatrixList.Add(_lateMatrix)
         Next


         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertPayTimekeeping(_payTimekeeping)


            If _payTimekeepingRestDayList.Count > 0 Then
               Me.InsertPayTimekeepingRestDays(_payTimekeepingRestDayList)
            End If

            If _payTimekeepingScheduleList.Count > 0 Then
               Me.InsertPayTimekeepingSchedules(_payTimekeepingScheduleList)
            End If

            If _payTimekeepingLateMatrixList.Count > 0 Then
               Me.InsertPayTimekeepingLateMatrix(_payTimekeepingLateMatrixList)
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

         Return Me.Ok(timekeeping.TimekeepingId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyTimekeeping")>
   <Route("timekeepings/{timekeepingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeeping(timekeepingId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingBody) As IHttpActionResult

      If timekeepingId <= 0 Then
         Throw New ArgumentException("Timekeeping ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeeping
         Dim _payTimekeepingRestDayList As New PayTimekeepingRestDayList
         Dim _payTimekeepingScheduleList As New PayTimekeepingScheduleList
         Dim _payTimekeepingLateMatrixList As New PayTimekeepingLateMatrixList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)

         For Each _restDay As PayTimekeepingRestDay In timekeeping.RestDayList
            _payTimekeepingRestDayList.Add(_restDay)
         Next

         For Each _schedule As PayTimekeepingSchedule In timekeeping.Schedules
            _payTimekeepingScheduleList.Add(_schedule)
         Next


         For Each _lateMatrix As PayTimekeepingLateMatrix In timekeeping.LateMatrix
            _payTimekeepingLateMatrixList.Add(_lateMatrix)
         Next

         '
         ' Load old values from DB
         '
         Dim _payTimekeepingOld As New PayTimekeeping
         Dim _payTimekeepingRestDayListOld As New PayTimekeepingRestDayList
         Dim _payTimekeepingScheduleListOld As New PayTimekeepingScheduleList
         Dim _payTimekeepingLateMatrixListOld As New PayTimekeepingLateMatrixList


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeeping(timekeepingId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("timekeeping").Rows(0)
            Me.LoadPayTimekeeping(_row, _payTimekeepingOld)
            Me.LoadPayTimekeepingRestDayList(_dataSet.Tables("restDayList").Rows, _payTimekeepingRestDayListOld)
            Me.LoadPayTimekeepingScheduleList(_dataSet.Tables("schedule").Rows, _payTimekeepingScheduleListOld)
            Me.LoadPayTimekeepingLateMatrixList(_dataSet.Tables("lateMatrix").Rows, _payTimekeepingLateMatrixListOld)

         End Using


#Region "PayTimekeepingRestDay"

         Dim _removeRestDayCount As Integer
         Dim _addRestDayCount As Integer
         Dim _editRestDayCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingRestDay In _payTimekeepingRestDayListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingRestDay In _payTimekeepingRestDayList
               If _new.TimekeepingRestDayId = _old.TimekeepingRestDayId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeRestDayCount = _removeRestDayCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingRestDay In _payTimekeepingRestDayList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingRestDay In _payTimekeepingRestDayListOld
               If _new.TimekeepingRestDayId = _old.TimekeepingRestDayId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .RestDayId <> _old.RestDayId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addRestDayCount = _addRestDayCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editRestDayCount = _editRestDayCount + 1
            End If

         Next

         Dim _payTimekeepingRestDayListNew As New PayTimekeepingRestDayList      ' for adding new Template Details

         If _addRestDayCount > 0 Then
            Dim _payTimekeepingRestDay As PayTimekeepingRestDay

            For Each _new As PayTimekeepingRestDay In _PayTimekeepingRestDayList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingRestDay = New PayTimekeepingRestDay
                  _payTimekeepingRestDayListNew.Add(_payTimekeepingRestDay)
                  DataLib.ScatterValues(_new, _payTimekeepingRestDay)
                  _payTimekeepingRestDay.TimekeepingId = _payTimekeeping.TimekeepingId
               End If
            Next

         End If

#End Region

#Region "PayTimekeepingShchedule"

         '
         ' Apply changes, save to DB
         '

         Dim _removeScheduleCount As Integer
         Dim _addScheduleCount As Integer
         Dim _editScheduleCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingSchedule In _payTimekeepingScheduleListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingSchedule In _payTimekeepingScheduleList
               If _new.TimekeepingScheduleId = _old.TimekeepingScheduleId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeScheduleCount = _removeScheduleCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingSchedule In _payTimekeepingScheduleList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingSchedule In _payTimekeepingScheduleListOld
               If _new.TimekeepingScheduleId = _old.TimekeepingScheduleId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .TimeIn <> _old.TimeIn OrElse .TimeOut <> _old.TimeOut OrElse .WorkingHour <> _old.WorkingHour OrElse .UnpaidBreak <> _old.UnpaidBreak Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addScheduleCount = _addScheduleCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editScheduleCount = _editScheduleCount + 1
            End If

         Next

         Dim _payTimekeepingScheduleListNew As New PayTimekeepingScheduleList      ' for adding new Barangays

         If _addScheduleCount > 0 Then
            Dim _payTimekeepingSchedule As PayTimekeepingSchedule

            For Each _new As PayTimekeepingSchedule In _payTimekeepingScheduleList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingSchedule = New PayTimekeepingSchedule
                  _payTimekeepingScheduleListNew.Add(_payTimekeepingSchedule)
                  DataLib.ScatterValues(_new, _payTimekeepingSchedule)
               End If
            Next

         End If


#End Region

#Region "PayTimekeepingShchedule"

         '
         ' Apply changes, save to DB
         '

         Dim _removeLateMatrixCount As Integer
         Dim _addLateMatrixCount As Integer
         Dim _editLateMatrixCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingLateMatrix In _payTimekeepingLateMatrixListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingLateMatrix In _payTimekeepingLateMatrixList
               If _new.LateMatrixId = _old.LateMatrixId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeLateMatrixCount = _removeLateMatrixCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingLateMatrix In _payTimekeepingLateMatrixList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingLateMatrix In _payTimekeepingLateMatrixListOld
               If _new.LateMatrixId = _old.LateMatrixId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .StartMinute <> _old.StartMinute OrElse .EndMinute <> _old.EndMinute OrElse .LateCount <> _old.LateCount Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addLateMatrixCount = _addLateMatrixCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editLateMatrixCount = _editLateMatrixCount + 1
            End If

         Next

         Dim _payTimekeepingLateMatrixListNew As New PayTimekeepingLateMatrixList      ' for adding new Barangays

         If _addLateMatrixCount > 0 Then
            Dim _payTimekeepingLateMatrix As PayTimekeepingLateMatrix

            For Each _new As PayTimekeepingLateMatrix In _payTimekeepingLateMatrixList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingLateMatrix = New PayTimekeepingLateMatrix
                  _payTimekeepingLateMatrixListNew.Add(_payTimekeepingLateMatrix)
                  DataLib.ScatterValues(_new, _payTimekeepingLateMatrix)
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

            If _removeRestDayCount > 0 Then
               Me.DeletePayTimekeepingRestDays(_payTimekeepingRestDayListOld)
            End If

            If _addRestDayCount > 0 Then
               Me.InsertPayTimekeepingRestDays(_payTimekeepingRestDayListNew)
            End If

            If _editRestDayCount > 0 Then
               Me.UpdatePayTimekeepingRestDays(_payTimekeepingRestDayList)
            End If

            If _removeScheduleCount > 0 Then
               Me.DeletePayTimekeepingSchedules(_payTimekeepingScheduleListOld)
            End If

            If _addScheduleCount > 0 Then
               Me.InsertPayTimekeepingSchedules(_payTimekeepingScheduleListNew)
            End If

            If _editScheduleCount > 0 Then
               Me.UpdatePayTimekeepingSchedules(_payTimekeepingScheduleList)
            End If

            If _removeLateMatrixCount > 0 Then
               Me.DeletePayTimekeepingLateMatrix(_payTimekeepingLateMatrixListOld)
            End If

            If _addLateMatrixCount > 0 Then
               Me.InsertPayTimekeepingLateMatrix(_payTimekeepingLateMatrixListNew)
            End If

            If _editLateMatrixCount > 0 Then
               Me.UpdatePayTimekeepingLateMatrix(_payTimekeepingLateMatrixList)
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
   Private Sub LoadPayTimekeeping(timekeeping As TimekeepingBody, payTimekeeping As PayTimekeeping)

      DataLib.ScatterValues(timekeeping, payTimekeeping)

   End Sub
   Private Sub LoadPayTimekeeping(row As DataRow, timekeeping As PayTimekeeping)

      With timekeeping
         .TimekeepingId = row.ToInt32("TimekeepingId")
         .TimekeepingDescription = row.ToString("TimekeepingDescription")
         .PolicyStatusId = row.ToInt32("PolicyStatusId")
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
         .PaidUpHolidayId = row.ToInt32("PaidUpHolidayId")
         .RestDayOffsetFlag = row.ToBoolean("RestDayOffsetFlag")
         .NightDifferentialCountId = row.ToInt32("NightDifferentialCountId")
         .NDCount = row.ToInt32("NDCount")
         .RestDayConsideration = row.ToInt32("RestDayConsideration")
         .LateCountId = row.ToInt32("LateCountId")
         .UndertimeCountId = row.ToInt32("UndertimeCountId")
         .HolidayCountId = row.ToInt32("HolidayCountId")
         .RegularHourIntervalMinute = row.ToInt32("RegularHourIntervalMinute")
         .ConstructionFlag = row.ToBoolean("ConstructionFlag")
         .UndertimeDurationPerCount = row.ToInt32("UndertimeDurationPerCount")
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
   Private Sub LoadPayTimekeepingScheduleList(rows As DataRowCollection, list As PayTimekeepingScheduleList)

      Dim _detail As PayTimekeepingSchedule
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingSchedule

         With _detail
            .TimekeepingScheduleId = _row.ToInt32("TimekeepingScheduleId")
            .TimekeepingId = _row.ToInt32("TimekeepingId")
            .TimeIn = _row.ToTimeSpan("TimeIn")
            .TimeOut = _row.ToTimeSpan("TimeOut")
            .WorkingHour = _row.ToInt32("WorkingHour")
            .UnpaidBreak = _row.ToInt32("UnpaidBreak")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadPayTimekeepingLateMatrixList(rows As DataRowCollection, list As PayTimekeepingLateMatrixList)

      Dim _detail As PayTimekeepingLateMatrix
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingLateMatrix

         With _detail
            .LateMatrixId = _row.ToInt32("LateMatrixId")
            .TimekeepingId = _row.ToInt32("TimekeepingId")
            .StartMinute = _row.ToInt32("StartMinute")
            .EndMinute = _row.ToInt32("EndMinute")
            .LateCount = _row.ToInt32("LateCount")
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
   Private Sub InsertPayTimekeepingSchedules(list As PayTimekeepingScheduleList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TimekeepingScheduleId")
         .Add("WorkingHour")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingSchedule", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingSchedule In list
            Me.AddInsertUpdateParamsPayTimekeepingSchedule(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertPayTimekeepingLateMatrix(list As PayTimekeepingLateMatrixList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LateMatrixId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingLateMatrix", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingLateMatrix In list
            Me.AddInsertUpdateParamsPayTimekeepingLateMatrix(_detail)
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
         .AddWithValue("@TimekeepingDescription", timekeeping.TimekeepingDescription)

         .AddWithValue("@PolicyStatusId", timekeeping.PolicyStatusId)

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
         .AddWithValue("@RestDayOffsetFlag", timekeeping.RestDayOffsetFlag)
         .AddWithValue("@NightDifferentialCountId", timekeeping.NightDifferentialCountId)
         .AddWithValue("@NDCount", timekeeping.NDCount)
         .AddWithValue("@RestDayConsideration", timekeeping.RestDayConsideration)
         .AddWithValue("@LateCountId", timekeeping.LateCountId)
         .AddWithValue("@UndertimeCountId", timekeeping.UndertimeCountId)
         .AddWithValue("@HolidayCountId", timekeeping.HolidayCountId)
         .AddWithValue("@RegularHourIntervalMinute", timekeeping.RegularHourIntervalMinute)
         .AddWithValue("@ConstructionFlag", timekeeping.ConstructionFlag)
         .AddWithValue("@UndertimeDurationPerCount", timekeeping.UndertimeDurationPerCount)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingSchedule(dtl As PayTimekeepingSchedule)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingId", dtl.TimekeepingId)
         .AddWithValue("@TimeIn", dtl.TimeIn)
         .AddWithValue("@TimeOut", dtl.TimeOut)
         .AddWithValue("@WorkingHour", dtl.WorkingHour)
         .AddWithValue("@UnpaidBreak", dtl.UnpaidBreak)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingLateMatrix(dtl As PayTimekeepingLateMatrix)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingId", dtl.TimekeepingId)
         .AddWithValue("@StartMinute", dtl.StartMinute)
         .AddWithValue("@EndMinute", dtl.EndMinute)
         .AddWithValue("@LateCount", dtl.LateCount)

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
   Private Sub UpdatePayTimekeepingSchedules(list As PayTimekeepingScheduleList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingScheduleId")
         .Add("TimekeepingId")
      End With

      With _excludedFields
         .Add("WorkingHour")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingSchedule", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingSchedule In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingSchedule(_detail)
               .Parameters.AddWithValue("@TimekeepingScheduleId", _detail.TimekeepingScheduleId)
               '.Parameters.AddWithValue("@TimekeepingId", _detail.TimekeepingId)
               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdatePayTimekeepingLateMatrix(list As PayTimekeepingLateMatrixList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingLateMatrixId")
      End With

      With _excludedFields
         .Add("WorkingHour")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingLateMatrix", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingLateMatrix In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingLateMatrix(_detail)
               .Parameters.AddWithValue("@LateMatrixId", _detail.LateMatrixId)
               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

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
   Private Sub DeletePayTimekeepingSchedules(list As PayTimekeepingScheduleList)

      With DataCore.Command
         .CommandText = "DELETE dbo.PayTimekeepingSchedule WHERE TimekeepingScheduleId=@TimekeepingScheduleId"
         .CommandType = CommandType.Text

         For Each _old As PayTimekeepingSchedule In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@TimekeepingScheduleId", _old.TimekeepingScheduleId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeletePayTimekeepingLateMatrix(list As PayTimekeepingLateMatrixList)

      With DataCore.Command
         .CommandText = "DELETE dbo.PayTimekeepingLateMatrix WHERE LateMatrixId=@LateMatrixId"
         .CommandType = CommandType.Text

         For Each _old As PayTimekeepingLateMatrix In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@LateMatrixId", _old.LateMatrixId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

End Class
Public Class TimekeepingBody
   Inherits PayTimekeeping

   Public Property RestDayList As PayTimekeepingRestDay()
   Public Property Schedules As PayTimekeepingSchedule()
   Public Property LateMatrix As PayTimekeepingLateMatrix()

End Class

