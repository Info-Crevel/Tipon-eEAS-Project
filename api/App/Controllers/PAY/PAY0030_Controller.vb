
Imports ImageMagick

<RoutePrefix("api")>
Public Class PAY0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetTimekeepingRecordClientPayGroup")>
   <Route("timekeeping-record-client-pay-group-id/{clientPayGroupId}")>
   <HttpGet>
   Public Function GetTimekeepingRecordClientPayGroup(clientPayGroupId As Integer) As IHttpActionResult


      If clientPayGroupId <= 0 Then
         Throw New ArgumentException("Client Paygroup ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.pay0030_Schedule")
            With _direct
               .AddParameter("clientPayGroupId", clientPayGroupId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "scheduleA"
                     .Tables(1).TableName = "scheduleB"
                     .Tables(2).TableName = "scheduleC"
                     .Tables(3).TableName = "scheduleD"
                     .Tables(4).TableName = "scheduleE"
                     .Tables(5).TableName = "holiday"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try
   End Function

   <SymAuthorization("GetReferences_pay0030")>
   <Route("references/pay0030")>
   <HttpGet>
   Public Function GetReferences_pay0030() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.pay0030_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "schedule"
                     .Tables(1).TableName = "scheduleA"
                     .Tables(2).TableName = "scheduleB"
                     .Tables(3).TableName = "scheduleC"
                     .Tables(4).TableName = "scheduleD"
                     .Tables(5).TableName = "payFreq"
                     .Tables(6).TableName = "period"
                     .Tables(7).TableName = "payrollPeriod"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTimekeepingRecord")>
   <Route("timekeeping-records/{timekeepingSheetId}")>
   <HttpGet>
   Public Function GetTimekeepingRecord(timekeepingSheetId As Integer) As IHttpActionResult

      If timekeepingSheetId <= 0 Then
         Throw New ArgumentException("Timekeeping Sheet ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.pay0030")
            With _direct
               .AddParameter("TimekeepingSheetId", timekeepingSheetId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "timekeeping"
                     .Tables(1).TableName = "schedule"
                     .Tables(2).TableName = "member"
                     .Tables(3).TableName = "scheduleList"
                     .Tables(4).TableName = "scheduleA"
                     .Tables(5).TableName = "scheduleB"
                     .Tables(6).TableName = "scheduleC"
                     .Tables(7).TableName = "scheduleD"
                     .Tables(8).TableName = "scheduleE"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTimekeepingRecord")>
   <Route("timekeeping-records/{currentUserId}")>
   <HttpPost>
   Public Function CreateTimekeepingRecord(currentUserId As Integer, <FromBody> timekeeping As TimekeepingSheetBody) As IHttpActionResult

      If timekeeping.TimekeepingSheetId <> -1 Then
         Throw New ArgumentException("Timekeeping Sheet ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _timekeepingSheetId As Integer = SysLib.GetNextSequence("TimekeepingSheetId")

         timekeeping.TimekeepingSheetId = _timekeepingSheetId

         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingSheet
         Dim _payTimekeepingSheetMemberList As New PayTimekeepingSheetMemberList

         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingSheetMember In timekeeping.Schedules
            _schedule.TimekeepingSheetId = _timekeepingSheetId
            _payTimekeepingSheetMemberList.Add(_schedule)
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


            If _payTimekeepingSheetMemberList.Count > 0 Then

               Me.InsertPayTimekeepingSheetMembers(_payTimekeepingSheetMemberList)
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

         Return Me.Ok(timekeeping.TimekeepingSheetId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyTimekeepingRecord")>
   <Route("timekeeping-records/{timekeepingSheetId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeepingRecord(timekeepingSheetId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingSheetBody) As IHttpActionResult

      If timekeepingSheetId <= 0 Then
         Throw New ArgumentException("Timekeeping Sheet ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingSheet
         Dim _payTimekeepingSheetMemberList As New PayTimekeepingSheetMemberList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingSheetMember In timekeeping.Schedules
            _payTimekeepingSheetMemberList.Add(_schedule)
         Next

         '
         ' Load old values from DB
         '
         Dim _payTimekeepingOld As New PayTimekeepingSheet
         Dim _payTimekeepingSheetMemberListOld As New PayTimekeepingSheetMemberList


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeepingRecord(timekeepingSheetId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("timekeeping").Rows(0)
            Me.LoadPayTimekeeping(_row, _payTimekeepingOld)
            Me.LoadPayTimekeepingSheetMemberList(_dataSet.Tables("schedule").Rows, _payTimekeepingSheetMemberListOld)
         End Using



#Region "PayTimekeepingSheetMember"

         '
         ' Apply changes, save to DB
         '

         Dim _removeSheetMemberCount As Integer
         Dim _addSheetMemberCount As Integer
         Dim _editSheetMemberCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingSheetMember In _payTimekeepingSheetMemberListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingSheetMember In _payTimekeepingSheetMemberList
               If _new.SheetMemberId = _old.SheetMemberId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeSheetMemberCount = _removeSheetMemberCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingSheetMember In _payTimekeepingSheetMemberList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingSheetMember In _payTimekeepingSheetMemberListOld
               If _new.SheetMemberId = _old.SheetMemberId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MemberId <> _old.MemberId OrElse .ScheduleDate <> _old.ScheduleDate OrElse .TimekeepingScheduleId <> _old.TimekeepingScheduleId OrElse .TimekeepingScheduleIdA <> _old.TimekeepingScheduleIdA OrElse .TimekeepingScheduleIdB <> _old.TimekeepingScheduleIdB OrElse .TimekeepingScheduleIdC <> _old.TimekeepingScheduleIdC OrElse .TimekeepingScheduleIdD <> _old.TimekeepingScheduleIdD OrElse .ScheduleTimeIn <> _old.ScheduleTimeIn OrElse .ScheduleTimeOut <> _old.ScheduleTimeOut OrElse .ScheduleDateTimeOut <> _old.ScheduleDateTimeOut OrElse .TotalWorkingHour <> _old.TotalWorkingHour Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addSheetMemberCount = _addSheetMemberCount + 1

            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editSheetMemberCount = _editSheetMemberCount + 1
            End If

         Next

         Dim _payTimekeepingSheetMemberListNew As New PayTimekeepingSheetMemberList      ' for adding new Barangays

         If _addSheetMemberCount > 0 Then
            Dim _payTimekeepingSheetMember As PayTimekeepingSheetMember

            For Each _new As PayTimekeepingSheetMember In _payTimekeepingSheetMemberList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingSheetMember = New PayTimekeepingSheetMember
                  _payTimekeepingSheetMemberListNew.Add(_payTimekeepingSheetMember)
                  DataLib.ScatterValues(_new, _payTimekeepingSheetMember)
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

            If _removeSheetMemberCount > 0 Then
               Me.DeletePayTimekeepingSheetMembers(_payTimekeepingSheetMemberListOld)
            End If

            If _addSheetMemberCount > 0 Then
               Me.InsertPayTimekeepingSheetMembers(_payTimekeepingSheetMemberListNew)

            End If

            If _editSheetMemberCount > 0 Then
               Me.UpdatePayTimekeepingSheetMembers(_payTimekeepingSheetMemberList)
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

   <SymAuthorization("RemoveRecord")>
   <Route("timekeeping-records/{timekeepingId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveRecord(timekeepingId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

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
   Private Sub LoadPayTimekeeping(timekeeping As TimekeepingSheetBody, payTimekeeping As PayTimekeepingSheet)

      DataLib.ScatterValues(timekeeping, payTimekeeping)

   End Sub
   Private Sub LoadPayTimekeeping(row As DataRow, timekeeping As PayTimekeepingSheet)

      With timekeeping
         .TimekeepingSheetId = row.ToInt32("TimekeepingSheetId")
         .ClientPayGroupId = row.ToInt32("ClientPayGroupId")
         .PayFreqId = row.ToInt32("PayFreqId")
         .PeriodId = row.ToInt32("PeriodId")
         .YearId = row.ToInt32("YearId")
         .PayrollPeriodId = row.ToInt32("PayrollPeriodId")
         .StartDate = row.ToDate("StartDate")
         .EndDate = row.ToDate("EndDate")
         .CutOffDate = row.ToDate("CutOffDate")
         .CutOffStartDate = row.ToDate("CutOffStartDate")
         .CutOffEndDate = row.ToDate("CutOffEndDate")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub
   Private Sub LoadPayTimekeepingSheetMemberList(rows As DataRowCollection, list As PayTimekeepingSheetMemberList)

      Dim _detail As PayTimekeepingSheetMember
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingSheetMember

         With _detail
            .SheetMemberId = _row.ToInt32("SheetMemberId")
            .TimekeepingSheetId = _row.ToInt32("TimekeepingSheetId")
            .MemberId = _row.ToInt32("MemberId")
            .ScheduleDate = _row.ToDate("ScheduleDate")
            .TimekeepingScheduleId = _row.ToInt32("TimekeepingScheduleId")
            .TimekeepingScheduleIdA = _row.ToInt32("TimekeepingScheduleIdA")
            .TimekeepingScheduleIdB = _row.ToInt32("TimekeepingScheduleIdB")
            .TimekeepingScheduleIdC = _row.ToInt32("TimekeepingScheduleIdC")
            .TimekeepingScheduleIdD = _row.ToInt32("TimekeepingScheduleIdD")
            .ScheduleTimeIn = _row.ToTimeSpan("ScheduleTimeIn")
            .ScheduleTimeOut = _row.ToTimeSpan("ScheduleTimeOut")
            .ScheduleDateTimeOut = _row.ToDate("ScheduleDateTimeOut")
            .TotalWorkingHour = _row.ToDecimal("TotalWorkingHour")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertPayTimekeeping(timekeeping As PayTimekeepingSheet)

      Dim _excludedFields As New List(Of String)

        With _excludedFields

         .Add("LockId")
         .Add("ApplyOffSetFlag")
         .Add("DetailsFlag")
         .Add("ManualUploadFlag")
         .Add("ManualEditFlag")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingSheet", timekeeping, _excludedFields)
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
   Private Sub InsertPayTimekeepingSheetMembers(list As PayTimekeepingSheetMemberList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("SheetMemberId")
            .Add("NoScheduleId")
            .Add("ScheduleTimeInA")
         .Add("ScheduleTimeOutA")

         .Add("ScheduleTimeInB")
         .Add("ScheduleTimeOutB")
         .Add("ScheduleTimeInC")
         .Add("ScheduleTimeOutC")

         .Add("ScheduleTimeInD")
         .Add("ScheduleTimeOutD")

         .Add("ApprovedFormFlag")
         .Add("PayableToMemberFlag")
         .Add("BillableToClientFlag")
         .Add("Remarks")
            .Add("regularHour")
            .Add("overtimeHour")
            .Add("regularWorkingDayHour")
            .Add("regularOvertimeHour")
            .Add("lateMinute")
            .Add("undertimeMinute")
            .Add("OTRemarks")
            .Add("OTFileName")
            .Add("OTGUID")
            .Add("FileExtension")
            .Add("FileUrl")
            .Add("ND")
            .Add("NDOT")
            .Add("RD")
            .Add("RDOT")
            .Add("RDND")
            .Add("RDNDOT")
            .Add("SH")
            .Add("SHOT")
            .Add("SHND")
            .Add("SHNDOT")
            .Add("SHRD")
            .Add("SHRDOT")
            .Add("SHRDND")
            .Add("SHRDNDOT")
            .Add("LHRD")
            .Add("LHRDOT")
            .Add("LHRDND")
            .Add("LHRDNDOT")
            .Add("LH")
            .Add("LHOT")
            .Add("LHND")
            .Add("LHNDOT")
         .Add("LHSH")
         .Add("LHSHOT")
         .Add("LHSHND")
         .Add("LHSHNDOT")

         .Add("LHSHRD")
         .Add("LHSHRDOT")
         .Add("LHSHRDND")
         .Add("LHSHRDNDOT")

         .Add("HU100")
         .Add("DRH")
         .Add("DRHOT")
         .Add("DRHND")
         .Add("DRHNDOT")

         .Add("DRHRD")
         .Add("DRHRDOT")
         .Add("DRHRDND")
         .Add("DRHRDNDOT")

         .Add("regularHourP")
            .Add("overtimeHourP")
            .Add("lateMinuteP")
            .Add("undertimeMinuteP")
            .Add("NDP")
            .Add("NDOTP")

         .Add("RDP")
         .Add("RDOTP")
         .Add("RDNDP")
         .Add("RDNDOTP")
         .Add("SHP")
         .Add("SHOTP")
         .Add("SHNDP")
         .Add("SHNDOTP")
         .Add("SHRDP")
         .Add("SHRDOTP")
         .Add("SHRDNDP")
         .Add("SHRDNDOTP")
         .Add("LHRDP")
         .Add("LHRDOTP")
         .Add("LHRDNDP")
         .Add("LHRDNDOTP")
         .Add("LHP")
         .Add("LHOTP")
         .Add("LHNDP")
         .Add("LHNDOTP")
         .Add("LHSHP")
         .Add("LHSHOTP")
         .Add("LHSHNDP")
         .Add("LHSHNDOTP")

         .Add("LHSHRDP")
         .Add("LHSHRDOTP")
         .Add("LHSHRDNDP")
         .Add("LHSHRDNDOTP")

         .Add("HU100P")
         .Add("DRHP")
         .Add("DRHOTP")
         .Add("DRHNDP")
         .Add("DRHNDOTP")

         .Add("DRHRDP")
         .Add("DRHRDOTP")
         .Add("DRHRDNDP")
         .Add("DRHRDNDOTP")


         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingSheetMember", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingSheetMember In list
            Me.AddInsertUpdateParamsPayTimekeepingSheetMember(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdatePayTimekeeping(timekeeping As PayTimekeepingSheet)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingSheetId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("ApplyOffSetFlag")
         .Add("DetailsFlag")
         .Add("ManualUploadFlag")
         .Add("ManualEditFlag")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingSheet", timekeeping, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(timekeeping.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsPayTimekeeping(timekeeping As PayTimekeepingSheet)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingSheetId", timekeeping.TimekeepingSheetId)
         .AddWithValue("@ClientPayGroupId", timekeeping.ClientPayGroupId)
         .AddWithValue("@PayFreqId", timekeeping.PayFreqId)
         .AddWithValue("@PeriodId", timekeeping.PeriodId)
         .AddWithValue("@YearId", timekeeping.YearId)
         .AddWithValue("@PayrollPeriodId", timekeeping.PayrollPeriodId)
         .AddWithValue("@StartDate", timekeeping.StartDate)
         .AddWithValue("@EndDate", timekeeping.EndDate)
         .AddWithValue("@CutOffDate", timekeeping.CutOffDate)
         .AddWithValue("@CutOffStartDate", timekeeping.CutOffStartDate)
         .AddWithValue("@CutOffEndDate", timekeeping.CutOffEndDate)

            .AddWithValue("@Remarks", timekeeping.Remarks.ToNullable)
        End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingSheetMember(dtl As PayTimekeepingSheetMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingSheetId", dtl.TimekeepingSheetId)
         .AddWithValue("@MemberId", dtl.MemberId)
         .AddWithValue("@ScheduleDate", dtl.ScheduleDate)
         .AddWithValue("@TimekeepingScheduleId", dtl.TimekeepingScheduleId)
         .AddWithValue("@TimekeepingScheduleIdA", dtl.TimekeepingScheduleIdA)
         .AddWithValue("@TimekeepingScheduleIdB", dtl.TimekeepingScheduleIdB)
         .AddWithValue("@TimekeepingScheduleIdC", dtl.TimekeepingScheduleIdC)
         .AddWithValue("@TimekeepingScheduleIdD", dtl.TimekeepingScheduleIdD)
         .AddWithValue("@ScheduleTimeIn", dtl.ScheduleTimeIn)
         .AddWithValue("@ScheduleTimeOut", dtl.ScheduleTimeOut)
         .AddWithValue("@TotalWorkingHour", dtl.TotalWorkingHour)
         .AddWithValue("@ScheduleDateTimeOut", dtl.ScheduleDateTimeOut.ToNullable)

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
   Private Sub UpdatePayTimekeepingSheetMembers(list As PayTimekeepingSheetMemberList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("SheetMemberId")
      End With

      With _excludedFields
         '.Add("ScheduleTimeIn")
         '.Add("ScheduleTimeOut")

         .Add("ScheduleTimeInA")
         .Add("ScheduleTimeOutA")
            .Add("NoScheduleId")
            .Add("ScheduleTimeInB")
         .Add("ScheduleTimeOutB")
         .Add("ScheduleTimeInC")
            .Add("ScheduleTimeOutC")
            .Add("ScheduleTimeInD")
            .Add("ScheduleTimeOutD")
            .Add("ApprovedFormFlag")
         .Add("PayableToMemberFlag")
         .Add("BillableToClientFlag")
         .Add("Remarks")
            .Add("regularHour")
            .Add("overtimeHour")
            .Add("regularWorkingDayHour")
            .Add("regularOvertimeHour")
            .Add("lateMinute")
            .Add("undertimeMinute")
            .Add("OTRemarks")
            .Add("OTFileName")
            .Add("OTGUID")
            .Add("FileExtension")
            .Add("ND")
            .Add("NDOT")
            .Add("RD")
            .Add("RDOT")
            .Add("RDND")
            .Add("RDNDOT")
            .Add("SH")
            .Add("SHOT")
            .Add("SHND")
            .Add("SHNDOT")
            .Add("SHRD")
            .Add("SHRDOT")
            .Add("SHRDND")
            .Add("SHRDNDOT")
            .Add("LHRD")
            .Add("LHRDOT")
            .Add("LHRDND")
            .Add("LHRDNDOT")
            .Add("LH")
            .Add("LHOT")
            .Add("LHND")
            .Add("LHNDOT")

         .Add("LHSH")
         .Add("LHSHOT")
         .Add("LHSHND")
         .Add("LHSHNDOT")

         .Add("LHSHRD")
         .Add("LHSHRDOT")
         .Add("LHSHRDND")
         .Add("LHSHRDNDOT")

         .Add("HU100")
            .Add("DRH")
            .Add("DRHOT")
         .Add("DRHND")
         .Add("DRHNDOT")

         .Add("DRHRD")
         .Add("DRHRDOT")
         .Add("DRHRDND")
         .Add("DRHRDNDOT")

         .Add("regularHourP")
            .Add("overtimeHourP")
            .Add("lateMinuteP")
            .Add("undertimeMinuteP")
         '.Add("NDP")
         '.Add("NDOTP")
         '.Add("RDP")
         '.Add("RDOTP")
         '.Add("RDNDP")
         '.Add("RDNDOTP")
         '.Add("SHP")
         '.Add("SHOTP")
         '.Add("SHNDP")
         '.Add("SHNDOTP")
         '.Add("SHRDP")
         '.Add("SHRDOTP")
         '.Add("SHRDNDP")
         '.Add("SHRDNDOTP")
         '.Add("LHRDP")
         '.Add("LHRDOTP")
         '.Add("LHRDNDP")
         '.Add("LHRDNDOTP")
         '.Add("LHSHP")
         '.Add("LHP")
         '.Add("LHOTP")
         '.Add("LHNDP")
         '.Add("LHNDOTP")
         '.Add("LHSHRDP")
         '.Add("HU100P")
         '.Add("DRHP")
         '.Add("DRHOTP")
         '.Add("DRHRDP")
         '.Add("DRHRDOTP")
         .Add("NDP")
         .Add("NDOTP")
         .Add("RDP")
         .Add("RDOTP")
         .Add("RDNDP")
         .Add("RDNDOTP")
         .Add("SHP")
         .Add("SHOTP")
         .Add("SHNDP")
         .Add("SHNDOTP")
         .Add("SHRDP")
         .Add("SHRDOTP")
         .Add("SHRDNDP")
         .Add("SHRDNDOTP")
         .Add("LHRDP")
         .Add("LHRDOTP")
         .Add("LHRDNDP")
         .Add("LHRDNDOTP")
         .Add("LHP")
         .Add("LHOTP")
         .Add("LHNDP")
         .Add("LHNDOTP")

         .Add("LHSHP")
         .Add("LHSHOTP")
         .Add("LHSHNDP")
         .Add("LHSHNDOTP")

         .Add("LHSHRDP")
         .Add("LHSHRDOTP")
         .Add("LHSHRDNDP")
         .Add("LHSHRDNDOTP")

         .Add("HU100P")
         .Add("DRHP")
         .Add("DRHOTP")
         .Add("DRHNDP")
         .Add("DRHNDOTP")

         .Add("DRHRDP")
         .Add("DRHRDOTP")
         .Add("DRHRDNDP")
         .Add("DRHRDNDOTP")


         .Add("FileUrl")

            .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingSheetMember", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingSheetMember In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingSheetMember(_detail)
               .Parameters.AddWithValue("@SheetMemberId", _detail.SheetMemberId)
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
   Private Sub DeletePayTimekeepingSheetMembers(list As PayTimekeepingSheetMemberList)

      With DataCore.Command
         .CommandText = "DELETE dbo.PayTimekeepingSheetMember WHERE SheetMemberId=@SheetMemberId"
         .CommandType = CommandType.Text

         For Each _old As PayTimekeepingSheetMember In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@SheetMemberId", _old.SheetMemberId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

End Class
'Public Class TimekeepingSheetBody
'   Inherits PayTimekeepingSheet

'   Public Property Schedules As PayTimekeepingSheetMember()

'End Class

