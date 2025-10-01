
Imports ImageMagick

<RoutePrefix("api")>
Public Class PAY0040_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetTimekeepingOutputClientPayGroup")>
   <Route("timekeeping-output-client-pay-group-id/{clientPayGroupId}")>
   <HttpGet>
   Public Function GetTimekeepingOutputClientPayGroup(clientPayGroupId As Integer) As IHttpActionResult


      If clientPayGroupId <= 0 Then
         Throw New ArgumentException("Client Paygroup ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.pay0040_Schedule")
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

   <SymAuthorization("GetReferences_pay0040")>
   <Route("references/pay0040")>
   <HttpGet>
   Public Function GetReferences_pay0040() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.pay0040_References")
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
                     .Tables(8).TableName = "noSchedules"

                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTimekeepingOutput")>
   <Route("timekeeping-outputs/{timekeepingSheetId}")>
   <HttpGet>
   Public Function GetTimekeepingOutput(timekeepingSheetId As Integer) As IHttpActionResult

      If timekeepingSheetId <= 0 Then
         Throw New ArgumentException("Timekeeping Sheet ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.pay0040")
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
                     .Tables(9).TableName = "lateMatrix"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTimekeepingOutput")>
   <Route("timekeeping-outputs/{currentUserId}")>
   <HttpPost>
   Public Function CreateTimekeepingOutput(currentUserId As Integer, <FromBody> timekeeping As TimekeepingSheetBody) As IHttpActionResult

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

   <SymAuthorization("ModifyTimekeepingOutput")>
   <Route("timekeeping-outputs/{timekeepingSheetId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeepingOutput(timekeepingSheetId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingSheetBody) As IHttpActionResult

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


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeepingOutput(timekeepingSheetId), Results.OkNegotiatedContentResult(Of DataSet))

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
                     If .MemberId <> _old.MemberId OrElse .PayableToMemberFlag <> _old.PayableToMemberFlag OrElse .BillableToClientFlag <> _old.BillableToClientFlag OrElse .ScheduleDate <> _old.ScheduleDate OrElse .TimekeepingScheduleId <> _old.TimekeepingScheduleId OrElse .TimekeepingScheduleIdA <> _old.TimekeepingScheduleIdA OrElse .TimekeepingScheduleIdB <> _old.TimekeepingScheduleIdB OrElse .TimekeepingScheduleIdC <> _old.TimekeepingScheduleIdC OrElse .TimekeepingScheduleIdD <> _old.TimekeepingScheduleIdD OrElse .ScheduleTimeIn <> _old.ScheduleTimeIn OrElse .ScheduleTimeOut <> _old.ScheduleTimeOut OrElse .ScheduleDateTimeOut <> _old.ScheduleDateTimeOut OrElse .TotalWorkingHour <> _old.TotalWorkingHour OrElse .ApprovedFormFlag <> _old.ApprovedFormFlag OrElse .PayableToMemberFlag <> _old.PayableToMemberFlag OrElse .BillableToClientFlag <> _old.BillableToClientFlag OrElse .Remarks <> _old.Remarks OrElse .regularHour <> _old.regularHour OrElse .overtimeHour <> _old.overtimeHour OrElse .regularWorkingDayHour <> _old.regularWorkingDayHour OrElse .regularOvertimeHour <> _old.regularOvertimeHour OrElse .lateMinute <> _old.lateMinute OrElse .undertimeMinute <> _old.undertimeMinute OrElse .OTRemarks <> _old.OTRemarks OrElse .OTFileName <> _old.OTFileName OrElse .OTGUID <> _old.OTGUID OrElse .regularHour <> _old.regularHour OrElse .overtimeHour <> _old.overtimeHour OrElse .ND <> _old.ND OrElse .NDOT <> _old.NDOT OrElse .lateMinute <> _old.lateMinute OrElse .undertimeMinute <> _old.undertimeMinute OrElse .RD <> _old.RD OrElse .RDOT <> _old.RDOT OrElse .RDND <> _old.RDND OrElse .RDNDOT <> _old.RDNDOT OrElse .SH <> _old.SH OrElse .SHOT <> _old.SHOT OrElse .SHND <> _old.SHND OrElse .SHNDOT <> _old.SHNDOT OrElse .SHRD <> _old.SHRD OrElse .SHRDOT <> _old.SHRDOT OrElse .SHRDND <> _old.SHRDND OrElse .SHRDNDOT <> _old.SHRDNDOT OrElse .LHRD <> _old.LHRD OrElse .LHRDOT <> _old.LHRDOT OrElse .LHRDND <> _old.LHRDND OrElse .LHRDNDOT <> _old.LHRDNDOT OrElse .LH <> _old.LH OrElse .LHOT <> _old.LHOT OrElse .LHND <> _old.LHND OrElse .LHNDOT <> _old.LHNDOT OrElse .LHSH <> _old.LHSH OrElse .LHSHRD <> _old.LHSHRD OrElse .HU100 <> _old.HU100 OrElse .DRH <> _old.DRH OrElse .DRHOT <> _old.DRHOT OrElse .DRHRD <> _old.DRHRD OrElse .DRHRDOT <> _old.DRHRDOT OrElse .regularHourP <> _old.regularHourP OrElse .overtimeHourP <> _old.overtimeHourP OrElse .lateMinuteP <> _old.lateMinuteP OrElse .undertimeMinuteP <> _old.undertimeMinuteP OrElse .NDP <> _old.NDP OrElse .NDOTP <> _old.NDOTP OrElse .RDP <> _old.RDP OrElse .RDOTP <> _old.RDOTP OrElse .RDNDP <> _old.RDNDP OrElse .RDNDOTP <> _old.RDNDOTP OrElse .SHP <> _old.SHP OrElse .SHOTP <> _old.SHOTP OrElse .SHNDP <> _old.SHNDP OrElse .SHNDOTP <> _old.SHNDOTP OrElse .SHRDP <> _old.SHRDP OrElse .SHRDOTP <> _old.SHRDOTP OrElse .SHRDNDP <> _old.SHRDNDP OrElse .SHRDNDOTP <> _old.SHRDNDOTP OrElse .LHRDP <> _old.LHRDP OrElse .LHRDOTP <> _old.LHRDOTP OrElse .LHRDNDP <> _old.LHRDNDP OrElse .LHRDNDOTP <> _old.LHRDNDOTP OrElse .LHP <> _old.LHP OrElse .LHOTP <> _old.LHOTP OrElse .LHNDP <> _old.LHNDP OrElse .LHNDOTP <> _old.LHNDOTP OrElse .LHSHP <> _old.LHSHP OrElse .LHSHOTP <> _old.LHSHOTP OrElse .LHSHNDP <> _old.LHSHNDP OrElse .LHSHNDOTP <> _old.LHSHNDOTP OrElse .LHSHRDP <> _old.LHSHRDP OrElse .LHSHRDOTP <> _old.LHSHRDOTP OrElse .LHSHRDNDP <> _old.LHSHRDNDP OrElse .LHSHRDNDOTP <> _old.LHSHRDNDOTP OrElse .HU100P <> _old.HU100P OrElse .DRHP <> _old.DRHP OrElse .DRHOTP <> _old.DRHOTP OrElse .DRHNDP <> _old.DRHNDP OrElse .DRHNDOTP <> _old.DRHNDOTP OrElse .DRHRDP <> _old.DRHRDP OrElse .DRHRDOTP <> _old.DRHRDOTP OrElse .DRHRDNDP <> _old.DRHRDNDP OrElse .DRHRDNDOTP <> _old.DRHRDNDOTP OrElse .NoScheduleId <> _old.NoScheduleId Then
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
                    'File.WriteAllText("e:\aaa.txt", "XX")
                    Me.UpdatePayTimekeepingSheetMembers(_payTimekeepingSheetMemberList)
                    'File.WriteAllText("e:\aaa1.txt", "XX")

                    For Each _new As PayTimekeepingSheetMember In _payTimekeepingSheetMemberList
                  If _new.LogActionId = LogActionId.Edit Then

                     'With _logKeyList
                     '   .Clear()
                     '   .Add("MemberId", _new.MemberId)
                     '   '.Add("AccountId", _new.AccountId)
                     'End With

                     '_id = AppLib.CreateLogHeader("InsHrsMemberDocTypeLog", _logKeyList, LogActionId.Edit, currentUserId)

                     '_hrsMemberDocTypeLogDetailList.Clear()

                     For Each _old As PayTimekeepingSheetMember In _payTimekeepingSheetMemberListOld
                        If _new.SheetMemberId = _old.SheetMemberId Then

                           With _new

                              'If .DocTypeId <> _old.DocTypeId Then
                              '   Dim _oldDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = _old.DocTypeId).DocTypeName
                              '   Dim _newDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName

                              '   AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeId, .DocTypeId.ToString, _old.DocTypeId.ToString, .DocTypeId.ToString + "=" + _oldDocTypeName + "; " + .DocTypeId.ToString + "=" + _newDocTypeName)
                              'End If


                              If .OTGUID <> _old.OTGUID Then

                                 'Upload File
                                 RemoveOTFile(.MemberId, _old.OTFileName, _old.OTGUID)
                                 UploadOTFile(.MemberId, .OTFileName, .OTGUID)
                                 'Upload File

                                 'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                              End If

                              'AppLib.CreateLogDetails(_hrsMemberDocTypeLogDetailList, "HrsMemberDocTypeLogDetail")

                           End With

                        End If
                     Next

                  End If

               Next



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

   <SymAuthorization("RemoveOutput")>
                                                                                                                                  <Route("timekeeping-outputs/{timekeepingId}/{lockId}/{currentUserId}")>
                                                                                                                                     <HttpDelete>
   Public Function RemoveOutput(timekeepingId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

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
         .ApplyOffSetFlag = row.ToBoolean("ApplyOffSetFlag")
         .DetailsFlag = row.ToBoolean("DetailsFlag")
         .ManualUploadFlag = row.ToBoolean("ManualUploadFlag")
         .ManualEditFlag = row.ToBoolean("ManualEditFlag")
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
            .NoScheduleId = _row.ToInt32("NoScheduleId")

            .TimekeepingScheduleId = _row.ToInt32("TimekeepingScheduleId")
            .TimekeepingScheduleIdA = _row.ToInt32("TimekeepingScheduleIdA")
            .TimekeepingScheduleIdB = _row.ToInt32("TimekeepingScheduleIdB")
            .TimekeepingScheduleIdC = _row.ToInt32("TimekeepingScheduleIdC")
            .TimekeepingScheduleIdD = _row.ToInt32("TimekeepingScheduleIdD")
            .ScheduleTimeIn = _row.ToTimeSpan("ScheduleTimeIn")
            .ScheduleTimeOut = _row.ToTimeSpan("ScheduleTimeOut")
            .ScheduleDateTimeOut = _row.ToDate("ScheduleDateTimeOut")
            .TotalWorkingHour = _row.ToDecimal("TotalWorkingHour")

            .ApprovedFormFlag = _row.ToBoolean("ApprovedFormFlag")
            .PayableToMemberFlag = _row.ToBoolean("PayableToMemberFlag")
            .BillableToClientFlag = _row.ToBoolean("BillableToClientFlag")
            .Remarks = _row.ToString("Remarks")
            '.RegularHour = _row.ToDecimal("RegularHour")
            '.OvertimeHour = _row.ToDecimal("OvertimeHour")
            '.RegularWorkingDayHour = _row.ToDecimal("RegularWorkingDayHour")
            '.RegularOvertimeHour = _row.ToDecimal("RegularOvertimeHour")
            '.LateMinute = _row.ToDecimal("LateMinute")
            '.UndertimeMinute = _row.ToDecimal("UndertimeMinute")
            .regularHour = _row.ToDecimal("RegularHour")
            .overtimeHour = _row.ToDecimal("OvertimeHour")
            .regularWorkingDayHour = _row.ToDecimal("RegularWorkingDayHour")
            .regularOvertimeHour = _row.ToDecimal("RegularOvertimeHour")
            .lateMinute = _row.ToDecimal("LateMinute")
            .undertimeMinute = _row.ToDecimal("UndertimeMinute")
            .nd = _row.ToDecimal("nd")
            .ndOt = _row.ToDecimal("ndOt")
            .rd = _row.ToDecimal("rd")
            .rdOT = _row.ToDecimal("rdOT")
            .rdND = _row.ToDecimal("rdND")
            .rdNDOT = _row.ToDecimal("rdNDOT")
            .sh = _row.ToDecimal("sh")
            .shOT = _row.ToDecimal("shOT")
            .shND = _row.ToDecimal("shND")
            .shNDOT = _row.ToDecimal("shNDOT")
            .shRD = _row.ToDecimal("shRD")
            .shRDOT = _row.ToDecimal("shRDOT")
            .shRDND = _row.ToDecimal("shRDND")
            .shRDNDOT = _row.ToDecimal("shRDNDOT")
            .lhRD = _row.ToDecimal("lhRD")
            .lhRDOT = _row.ToDecimal("lhRDOT")
            .lhRDND = _row.ToDecimal("lhRDND")
            .lhRDNDOT = _row.ToDecimal("lhRDNDOT")
            .lh = _row.ToDecimal("lh")
            .lhOT = _row.ToDecimal("lhOT")
            .lhND = _row.ToDecimal("lhND")
            .lhNDOT = _row.ToDecimal("lhNDOT")

            .LHSH = _row.ToDecimal("lhSH")
            .LHSHOT = _row.ToDecimal("lhSHOT")
            .LHSHND = _row.ToDecimal("lhSHND")
            .LHSHNDOT = _row.ToDecimal("lhSHNDOT")

            .LHSHRD = _row.ToDecimal("lhSHRD")
            .LHSHRDOT = _row.ToDecimal("lhSHRDOT")
            .LHSHRDND = _row.ToDecimal("lhSHRDND")
            .LHSHRDNDOT = _row.ToDecimal("lhSHRDNDOT")

            .hu100 = _row.ToDecimal("hu100")
            .dRH = _row.ToDecimal("dRH")
            .DRHOT = _row.ToDecimal("dRHOT")
            .DRHND = _row.ToDecimal("dRHND")
            .DRHNDOT = _row.ToDecimal("dRHNDOT")
            .dRHRD = _row.ToDecimal("dRHRD")
            .dRHRDOT = _row.ToDecimal("dRHRDOT")
            .DRHRDND = _row.ToDecimal("dRHRDND")
            .DRHRDNDOT = _row.ToDecimal("dRHRDNDOT")

            .regularHourP = _row.ToDecimal("RegularHourP")
            .overtimeHourP = _row.ToDecimal("OvertimeHourP")

            .lateMinuteP = _row.ToDecimal("LateMinuteP")
            .undertimeMinuteP = _row.ToDecimal("UndertimeMinuteP")
            .NDP = _row.ToDecimal("ndP")
            .NDOTP = _row.ToDecimal("ndOtP")
            .RDP = _row.ToDecimal("rdP")
            .RDOTP = _row.ToDecimal("rdOTP")
            .RDNDP = _row.ToDecimal("rdNDP")
            .RDNDOTP = _row.ToDecimal("rdNDOTP")
            .SHP = _row.ToDecimal("shP")
            .SHOTP = _row.ToDecimal("shOTP")
            .SHNDP = _row.ToDecimal("shNDP")
            .SHNDOTP = _row.ToDecimal("shNDOTP")
            .SHRDP = _row.ToDecimal("shRDP")
            .SHRDOTP = _row.ToDecimal("shRDOTP")
            .SHRDNDP = _row.ToDecimal("shRDNDP")
            .SHRDNDOTP = _row.ToDecimal("shRDNDOTP")
            .LHRDP = _row.ToDecimal("lhRDP")
            .LHRDOTP = _row.ToDecimal("lhRDOTP")
            .LHRDNDP = _row.ToDecimal("lhRDNDP")
            .LHRDNDOTP = _row.ToDecimal("lhRDNDOTP")
            .LHP = _row.ToDecimal("lhP")
            .LHOTP = _row.ToDecimal("lhOTP")
            .LHNDP = _row.ToDecimal("lhNDP")
            .LHNDOTP = _row.ToDecimal("lhNDOTP")
            .LHSHP = _row.ToDecimal("lhSHP")
            .LHSHOTP = _row.ToDecimal("lhSHOTP")
            .LHSHNDP = _row.ToDecimal("lhSHNDP")
            .LHSHNDOTP = _row.ToDecimal("lhSHNDOTP")


            .LHSHRDP = _row.ToDecimal("lhSHRDP")
            .LHSHRDOTP = _row.ToDecimal("lhSHRDOTP")
            .LHSHRDNDP = _row.ToDecimal("lhSHRDNDP")
            .LHSHRDNDOTP = _row.ToDecimal("lhSHRDNDOTP")

            .HU100P = _row.ToDecimal("hu100P")
            .DRHP = _row.ToDecimal("dRHP")
            .DRHOTP = _row.ToDecimal("dRHOTP")
            .DRHNDP = _row.ToDecimal("dRHNDP")
            .DRHNDOTP = _row.ToDecimal("dRHNDOTP")

            .DRHRDP = _row.ToDecimal("dRHRDP")
            .DRHRDOTP = _row.ToDecimal("dRHRDOTP")
            .DRHRDNDP = _row.ToDecimal("dRHRDNDP")
            .DRHRDNDOTP = _row.ToDecimal("dRHRDNDOTP")

            .OTRemarks = _row.ToString("OTRemarks")
            .OTFileName = _row.ToString("OTFileName")
            .OTGUID = _row.ToString("OTGUID")
            .FileExtension = _row.ToString("FileExtension")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertPayTimekeeping(timekeeping As PayTimekeepingSheet)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
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

         .Add("ScheduleTimeInA")
         .Add("ScheduleTimeOutA")

         .Add("ScheduleTimeInB")
         .Add("ScheduleTimeOutB")
         .Add("ScheduleTimeInC")
         .Add("ScheduleTimeOutC")

         .Add("ScheduleTimeInD")
         .Add("ScheduleTimeOutD")

            .Add("FileUrl")

            '.add("otremarks")
            '.add("otfilename")
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

      'With _excludedFields
      '   .Add("Age")
      '   '.Add("Address2")
      'End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingSheet", timekeeping, _keyFields) ', _excludedFields)
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
         .AddWithValue("@ApplyOffSetFlag", timekeeping.ApplyOffSetFlag)
         .AddWithValue("@DetailsFlag", timekeeping.DetailsFlag)
         .AddWithValue("@ManualUploadFlag", timekeeping.ManualUploadFlag)
         .AddWithValue("@ManualEditFlag", timekeeping.ManualEditFlag)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingSheetMember(dtl As PayTimekeepingSheetMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingSheetId", dtl.TimekeepingSheetId)
         .AddWithValue("@MemberId", dtl.MemberId)
         .AddWithValue("@ScheduleDate", dtl.ScheduleDate)
         .AddWithValue("@NoScheduleId", dtl.NoScheduleId)
         .AddWithValue("@TimekeepingScheduleId", dtl.TimekeepingScheduleId)
         .AddWithValue("@TimekeepingScheduleIdA", dtl.TimekeepingScheduleIdA)
         .AddWithValue("@TimekeepingScheduleIdB", dtl.TimekeepingScheduleIdB)
         .AddWithValue("@TimekeepingScheduleIdC", dtl.TimekeepingScheduleIdC)
         .AddWithValue("@TimekeepingScheduleIdD", dtl.TimekeepingScheduleIdD)
         .AddWithValue("@ScheduleTimeIn", dtl.ScheduleTimeIn)
         .AddWithValue("@ScheduleTimeOut", dtl.ScheduleTimeOut)
         .AddWithValue("@ScheduleDateTimeOut", dtl.ScheduleDateTimeOut.ToNullable)
         .AddWithValue("@TotalWorkingHour", dtl.TotalWorkingHour)


         .AddWithValue("@ApprovedFormFlag", dtl.ApprovedFormFlag)
         .AddWithValue("@PayableToMemberFlag", dtl.PayableToMemberFlag)
         .AddWithValue("@BillableToClientFlag", dtl.BillableToClientFlag)
         .AddWithValue("@Remarks", dtl.Remarks.ToNullable)
         '.AddWithValue("@RegularHour", dtl.RegularHour)
         '.AddWithValue("@OvertimeHour", dtl.OvertimeHour)
         '.AddWithValue("@RegularWorkingDayHour", dtl.RegularWorkingDayHour)
         '.AddWithValue("@RegularOvertimeHour", dtl.RegularOvertimeHour)
         '.AddWithValue("@LateMinute", dtl.LateMinute)
         '.AddWithValue("@UndertimeMinute", dtl.UndertimeMinute)

         .AddWithValue("@RegularHour", dtl.regularHour)
         .AddWithValue("@OvertimeHour", dtl.overtimeHour)
         .AddWithValue("@RegularWorkingDayHour", dtl.regularWorkingDayHour)
         .AddWithValue("@RegularOvertimeHour", dtl.regularOvertimeHour)
         .AddWithValue("@LateMinute", dtl.lateMinute)
         .AddWithValue("@UndertimeMinute", dtl.undertimeMinute)
         .AddWithValue("@nd", dtl.nd)
         .AddWithValue("@ndOt", dtl.ndOt)
         .AddWithValue("@rd", dtl.rd)
         .AddWithValue("@rdOT", dtl.rdOT)
         .AddWithValue("@rdND", dtl.rdND)
         .AddWithValue("@rdNDOT", dtl.rdNDOT)
         .AddWithValue("@sh", dtl.sh)
         .AddWithValue("@shOT", dtl.shOT)
         .AddWithValue("@shND", dtl.shND)
         .AddWithValue("@shNDOT", dtl.shNDOT)
         .AddWithValue("@shRD", dtl.shRD)
         .AddWithValue("@shRDOT", dtl.shRDOT)
         .AddWithValue("@shRDND", dtl.shRDND)
         .AddWithValue("@shRDNDOT", dtl.shRDNDOT)
         .AddWithValue("@lhRD", dtl.lhRD)
         .AddWithValue("@lhRDOT", dtl.lhRDOT)
         .AddWithValue("@lhRDND", dtl.lhRDND)
         .AddWithValue("@lhRDNDOT", dtl.lhRDNDOT)
         .AddWithValue("@lh", dtl.lh)
         .AddWithValue("@lhOT", dtl.lhOT)
         .AddWithValue("@lhND", dtl.lhND)
         .AddWithValue("@lhNDOT", dtl.lhNDOT)
         .AddWithValue("@lhSH", dtl.LHSH)
         .AddWithValue("@lhSHOT", dtl.LHSHOT)
         .AddWithValue("@lhSHND", dtl.LHSHND)
         .AddWithValue("@lhSHNDOT", dtl.LHSHNDOT)

         .AddWithValue("@lhSHRD", dtl.LHSHRD)
         .AddWithValue("@lhSHRDOT", dtl.LHSHRDOT)
         .AddWithValue("@lhSHRDND", dtl.LHSHRDND)
         .AddWithValue("@lhSHRDNDOT", dtl.LHSHRDNDOT)

         .AddWithValue("@hu100", dtl.hu100)
         .AddWithValue("@dRH", dtl.dRH)
         .AddWithValue("@dRHOT", dtl.DRHOT)
         .AddWithValue("@dRHND", dtl.DRHND)
         .AddWithValue("@dRHNDOT", dtl.DRHNDOT)

         .AddWithValue("@dRHRD", dtl.dRHRD)
         .AddWithValue("@dRHRDOT", dtl.dRHRDOT)
         .AddWithValue("@dRHRDND", dtl.DRHRDND)
         .AddWithValue("@dRHRDNDOT", dtl.DRHRDNDOT)

         .AddWithValue("@RegularHourP", dtl.regularHourP)
         .AddWithValue("@OvertimeHourP", dtl.overtimeHourP)

         .AddWithValue("@LateMinuteP", dtl.lateMinuteP)
         .AddWithValue("@UndertimeMinuteP", dtl.undertimeMinuteP)
         .AddWithValue("@ndP", dtl.NDP)
         .AddWithValue("@ndOtP", dtl.NDOTP)
         .AddWithValue("@rdP", dtl.RDP)
         .AddWithValue("@rdOTP", dtl.RDOTP)
         .AddWithValue("@rdNDP", dtl.RDNDP)
         .AddWithValue("@rdNDOTP", dtl.RDNDOTP)
         .AddWithValue("@shP", dtl.SHP)
         .AddWithValue("@shOTP", dtl.SHOTP)
         .AddWithValue("@shNDP", dtl.SHNDP)
         .AddWithValue("@shNDOTP", dtl.SHNDOTP)
         .AddWithValue("@shRDP", dtl.SHRDP)
         .AddWithValue("@shRDOTP", dtl.SHRDOTP)
         .AddWithValue("@shRDNDP", dtl.SHRDNDP)
         .AddWithValue("@shRDNDOTP", dtl.SHRDNDOTP)
         .AddWithValue("@lhRDP", dtl.LHRDP)
         .AddWithValue("@lhRDOTP", dtl.LHRDOTP)
         .AddWithValue("@lhRDNDP", dtl.LHRDNDP)
         .AddWithValue("@lhRDNDOTP", dtl.LHRDNDOTP)
         .AddWithValue("@lhP", dtl.LHP)
         .AddWithValue("@lhOTP", dtl.LHOTP)
         .AddWithValue("@lhNDP", dtl.LHNDP)
         .AddWithValue("@lhNDOTP", dtl.LHNDOTP)

         .AddWithValue("@lhSHP", dtl.LHSHP)
         .AddWithValue("@lhSHOTP", dtl.LHSHOTP)
         .AddWithValue("@lhSHNDP", dtl.LHSHNDP)
         .AddWithValue("@lhSHNDOTP", dtl.LHSHNDOTP)

         .AddWithValue("@lhSHRDP", dtl.LHSHRDP)
         .AddWithValue("@lhSHRDOTP", dtl.LHSHRDOTP)
         .AddWithValue("@lhSHRDNDP", dtl.LHSHRDNDP)
         .AddWithValue("@lhSHRDNDOTP", dtl.LHSHRDNDOTP)

         .AddWithValue("@hu100P", dtl.HU100P)
         .AddWithValue("@dRHP", dtl.DRHP)
         .AddWithValue("@dRHOTP", dtl.DRHOTP)
         .AddWithValue("@dRHNDP", dtl.DRHNDP)
         .AddWithValue("@dRHNDOTP", dtl.DRHNDOTP)

         .AddWithValue("@dRHRDP", dtl.DRHRDP)
         .AddWithValue("@dRHRDOTP", dtl.DRHRDOTP)
         .AddWithValue("@dRHRDNDP", dtl.DRHRDNDP)
         .AddWithValue("@dRHRDNDOTP", dtl.DRHRDNDOTP)


         .AddWithValue("@OTRemarks", dtl.OTRemarks.ToNullable)
         .AddWithValue("@OTFileName", dtl.OTFileName.ToNullable)
         .AddWithValue("@OTGUID", dtl.OTGUID.ToNullable)
         .AddWithValue("@FileExtension", dtl.FileExtension.ToNullable)

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

         .Add("ScheduleTimeInB")
         .Add("ScheduleTimeOutB")
         .Add("ScheduleTimeInC")
         .Add("ScheduleTimeOutC")

         .Add("ScheduleTimeInD")
         .Add("ScheduleTimeOutD")

         .Add("LogActionId")
         .Add("FileUrl")
         '.Add("OTRemarks")
         '.Add("OTFileName")
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

   <SymAuthorization("UploadOTFiles")>
   <Route("timekeeping-output-member/{memberId}/{currentUserId}/{guid}/files")>
   <HttpPost>
   Public Async Function UploadOTFiles(memberId As Integer, currentUserId As Integer, guid As String, <FromUri> q As UploadOTDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

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

   Friend Shared Function RemoveOTFile(memberId As Integer, fileName As String, guid As String) As Boolean

      Try

         Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
         Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant
         Dim _memberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())

         Dim _fileName As String = Path.Combine(_memberFolder, guid + _extension)

         If File.Exists(_fileName) Then
            File.Delete(_fileName)
            Return True
         End If

      Catch ex As Exception

      End Try

      Return False

   End Function
   Friend Shared Function UploadOTFile(memberId As Integer, fileName As String, guid As String) As Boolean

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
End Class
'Public Class TimekeepingSheetBody
'   Inherits PayTimekeepingSheet

'   Public Property Schedules As PayTimekeepingSheetMember()

'End Class

Public Class UploadOTDocumentsQuery
   'Public Property DocTypeId As Integer
   Public Property OTGUID As String
End Class
