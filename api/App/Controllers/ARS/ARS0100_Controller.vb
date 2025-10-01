
Imports ImageMagick

<RoutePrefix("api")>
Public Class ARS0100_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetBillingProcess")>
   <Route("timekeeping-billing-process/{timekeepingPayOutId}")>
   <HttpGet>
   Public Function GetBillingProcess(timekeepingPayOutId As Integer) As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0100_Sheet")
            With _direct
               .AddParameter("TimekeepingPayOutId", timekeepingPayOutId)

               Dim rateList As New PayTimekeepingBillingMemberList()

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     ' Iterate through the dataset's rows
                     For Each _row As DataRow In .Tables(0).Rows
                        Dim _rate As New PayTimekeepingBillingMember

                        With _rate
                           ' Populate the properties with the values from the data row
                           .MemberId = Convert.ToInt32(_row("MemberId").ToString)
                           .MemberName = _row("MemberName").ToString
                           .DepartmentName = _row("DepartmentName").ToString
                           .TotalWorkingHour = Convert.ToDecimal(_row("TotalWorkingHour"))
                           .TotalWorkingDay = Convert.ToDecimal(_row("TotalWorkingHour")) / 8 'Convert.ToDecimal(_row("TotalWorkingDay"))
                           .PayingDailyRate = Convert.ToDecimal(_row("PayingDailyRate"))
                           .BasicPayOutRate = .PayingDailyRate * .TotalWorkingDay
                           .AllowanceDaily = Convert.ToDecimal(_row("AllowanceDaily"))
                           .OTHour = Convert.ToDecimal(_row("OTHour"))
                           .OTRate = Convert.ToDecimal(_row("OTRate"))

                           'REST DAY
                           .RDHour = Convert.ToDecimal(_row("RDHour"))
                           .RDRate = Convert.ToDecimal(_row("RDRate"))
                           'Pending Premium
                           '.RDRate = .RDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("RDPerc")) / 100

                           .RDOTHour = Convert.ToDecimal(_row("RDOTHour"))
                           .RDOTRate = Convert.ToDecimal(_row("RDOTRate"))

                           'Special Holiday			

                           .SHHour = Convert.ToDecimal(_row("SHHour"))
                           .SHRate = Convert.ToDecimal(_row("SHRate"))
                           'Pending Premium
                           .SHRate = Convert.ToDecimal(_row("SHRate"))

                           .SHOTHour = Convert.ToDecimal(_row("SHOTHour"))
                           .SHOTRate = Convert.ToDecimal(_row("SHOTRate"))


                           'Legal Holiday Unworked			

                           .HU100Hour = Convert.ToDecimal(_row("HU100Hour"))
                           .HU100Rate = Convert.ToDecimal(_row("HU100Rate"))

                           'Legal Holiday			

                           .LHHour = Convert.ToDecimal(_row("LHHour"))
                           .LHRate = Convert.ToDecimal(_row("LHRate"))
                           'Pending Premium
                           '.LHRate = Convert.ToDecimal(_row("LHRate"))

                           .LHOTHour = Convert.ToDecimal(_row("LHOTHour"))
                           .LHOTRate = Convert.ToDecimal(_row("LHOTRate"))



                           'Special Holiday Rest Day			

                           .SHRDHour = Convert.ToDecimal(_row("SHRDHour"))
                           .SHRDRate = Convert.ToDecimal(_row("SHRDRate"))
                           'Pending Premium
                           '.SHRDRate = Convert.ToDecimal(_row("SHRDRate"))

                           .SHRDOTHour = Convert.ToDecimal(_row("SHRDOTHour"))
                           .SHRDOTRate = Convert.ToDecimal(_row("SHRDOTRate"))

                           'Legal Holiday	Rest Day		

                           .LHRDHour = Convert.ToDecimal(_row("LHRDHour"))
                           .LHRDRate = Convert.ToDecimal(_row("LHRDRate"))
                           'Pending Premium
                           '.LHRDRate = .LHRDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHRDPerc")) / 100

                           .LHRDOTHour = Convert.ToDecimal(_row("LHRDOTHour"))
                           .LHRDOTRate = Convert.ToDecimal(_row("LHRDOTRate"))

                           'Extended Work
                           .ExtendedWorkRate = Convert.ToDecimal(_row("ExtendedWorkRate"))

                           'Night Differential
                           .NDHour = Convert.ToDecimal(_row("NDHour"))
                           .NDRate = Convert.ToDecimal(_row("NDRate"))

                           'Night Differential OT
                           .NDOTHour = Convert.ToDecimal(_row("NDOTHour"))
                           .NDOTRate = Convert.ToDecimal(_row("NDOTRate"))

                           'Night Differential RD
                           .RDNDHour = Convert.ToDecimal(_row("RDNDHour"))
                           .RDNDRate = Convert.ToDecimal(_row("RDNDRate"))

                           'Night Differential RD OT
                           .RDNDOTHour = Convert.ToDecimal(_row("RDNDOTHour"))
                           .RDNDOTRate = Convert.ToDecimal(_row("RDNDOTRate"))

                           'Night Differential SH
                           .SHNDHour = Convert.ToDecimal(_row("SHNDHour"))
                           .SHNDRate = Convert.ToDecimal(_row("SHNDRate"))

                           'Night Differential SH OT
                           .SHNDOTHour = Convert.ToDecimal(_row("SHNDOTHour"))
                           .SHNDOTRate = Convert.ToDecimal(_row("SHNDOTRate"))

                           'Night Differential SH RD
                           .SHRDNDHour = Convert.ToDecimal(_row("SHRDNDHour"))
                           .SHRDNDRate = Convert.ToDecimal(_row("SHRDNDRate"))

                           'Night Differential SH RD OT
                           .SHRDNDOTHour = Convert.ToDecimal(_row("SHRDNDOTHour"))
                           .SHRDNDOTRate = Convert.ToDecimal(_row("SHRDNDOTRate"))


                           'Night Differential LH
                           .LHNDHour = Convert.ToDecimal(_row("LHNDHour"))
                           .LHNDRate = Convert.ToDecimal(_row("LHNDRate"))

                           'Night Differential LH OT
                           .LHNDOTHour = Convert.ToDecimal(_row("LHNDOTHour"))
                           .LHNDOTRate = Convert.ToDecimal(_row("LHNDOTRate"))

                           'Night Differential LH RD
                           .LHRDNDHour = Convert.ToDecimal(_row("LHRDNDHour"))
                           .LHRDNDRate = Convert.ToDecimal(_row("LHRDNDRate"))

                           'Night Differential LH RD OT
                           .LHRDNDOTHour = Convert.ToDecimal(_row("LHRDNDOTHour"))
                           .LHRDNDOTRate = Convert.ToDecimal(_row("LHRDNDRate"))

                           'Other Allowance   

                           .OtherAllowance = (.RDHour + .SHHour + .LHHour + .SHRDHour + .LHRDHour) / 8 * .AllowanceDaily

                           .TotalBasic = .BasicPayOutRate + .NDRate
                           .GrossPay = .TotalBasic + .ExtendedWorkRate
                           .YEI = (.TotalWorkingHour + .SHHour + .LHHour + .SHRDHour + .LHRDHour) / 8 * Convert.ToDecimal(_row("YEI"))
                           .TotalGrossPay = .GrossPay + .YEI
                           .TotalGrossPayAF = .TotalGrossPay - .OTRate - .NDOTRate - .RDRate - .RDOTRate - .RDNDRate - .RDNDOTRate - .SHRate - .SHOTRate - .SHNDRate - .SHNDOTRate - .HU100Rate - .LHRate - .LHOTRate - .LHNDRate - .LHNDOTRate - .LHRDRate - .LHRDOTRate - .LHRDNDRate - .LHRDNDOTRate - .OtherAllowance
                           .AdminFee = .TotalGrossPayAF * Convert.ToDecimal(_row("AdminFee")) / 100
                           '.TotalGrossPay = .TotalGrossPayAF - .AdminFee

                           .NetBilling = .TotalGrossPay + .AdminFee
                           Try

                                        'Dim expression As String = .PayOutSheetFormula.Replace("A", .A.ToString()).Replace("B", .B.ToString()).Replace("C", .C.ToString()).Replace("D", .D.ToString()).Replace("E", .E.ToString()).Replace("F", .F.ToString()).Replace("G", .G.ToString()).Replace("W", .W.ToString()).Replace("X", .X.ToString())
                                        'Dim result As Decimal = Convert.ToDecimal(New DataTable().Compute(expression, String.Empty))
                                        'Dim fixedAmountResult As Decimal = Convert.ToDecimal(New DataTable().Compute(expression, String.Empty))
                                        '.DailyRate = result

                                        'If .PayTrxCode = "SSS" Then
                                        '   .DailyRate = GetSssRate(result, deploymentDate) * 12 / workingDays
                                        'End If

                                        'If .PayTrxCode = "PAGIB" Then
                                        '   .DailyRate = GetPbgRate(result, deploymentDate) * 12 / workingDays
                                        'End If

                                        'If .PayTrxCode = "EC" Then
                                        '   .DailyRate = GetEcRate(result, deploymentDate) * 12 / workingDays
                                        'End If

                                        'If .PayTrxCode = "PROV" Then
                                        '   .DailyRate = GetWispRate(result, deploymentDate) * 12 / workingDays
                                        'End If

                                        'If .PayTrxCode = "PHIL" Then
                                        '   .DailyRate = GetPhhRate(result, deploymentDate) * 12 / workingDays
                                        'End If

                                        'If .X > 0 Then
                                        '   .DailyRate = fixedAmountResult '* 12 / workingDays
                                        'End If

                                        '.DailyRate = Math.Round(.DailyRate, 2)
                                        '.MonthlyRate = Math.Round((.DailyRate * workingDays) / 12, 2)
                                        'Fil e.WriteAllText("e:\" + .PayTrxCode.ToString + ".txt", GetSssRate(result, deploymentDate).ToString)
                                    Catch ex As Exception
                              'File.WriteAllText("e:\error.txt", ex.Message)

                           End Try

                           '.PayTrxCode = "A+B+C" how To compute A + B+ C from column
                           '
                        End With


                        ' Add the rate object to the list
                        rateList.Add(_rate)
                     Next



                  End With

                  'Dim A As String = "400"
                  'Dim B As String = "450"
                  'Dim C As String = "460"


                  Return Me.Ok(rateList)
               End Using


            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try



   End Function

   <SymAuthorization("GetTimekeepingPayOut")>
   <Route("timekeeping-pay-out-id/{timekeepingPayOutId}")>
   <HttpGet>
   Public Function GetTimekeepingPayOut(timekeepingPayOutId As Integer) As IHttpActionResult


      If timekeepingPayOutId <= 0 Then
         Throw New ArgumentException("Pay Out ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ars0100_Sheet")
            With _direct
               .AddParameter("timekeepingPayOutId", timekeepingPayOutId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "sheet"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try
   End Function

   <SymAuthorization("GetReferences_ars0100")>
   <Route("references/ars0100")>
   <HttpGet>
   Public Function GetReferences_ars0100() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ars0100_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "deduction"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTimekeepingBilling")>
   <Route("timekeeping-billing/{timekeepingBillingId}")>
   <HttpGet>
   Public Function GetTimekeepingBilling(timekeepingBillingId As Integer) As IHttpActionResult

      If timekeepingBillingId <= 0 Then
         Throw New ArgumentException("Timekeeping Billing ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ars0100")
            With _direct
               .AddParameter("timekeepingBillingId", timekeepingBillingId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "timekeeping"
                     .Tables(1).TableName = "schedule"
                     .Tables(2).TableName = "member"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTimekeepingBilling")>
   <Route("timekeeping-billing/{currentUserId}")>
   <HttpPost>
   Public Function CreateTimekeepingBilling(currentUserId As Integer, <FromBody> timekeeping As TimekeepingBillingBody) As IHttpActionResult

      If timekeeping.TimekeepingBillingId <> -1 Then
         Throw New ArgumentException("Timekeeping Billing ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _timekeepingBillingId As Integer = SysLib.GetNextSequence("TimekeepingBillingId")

         timekeeping.TimekeepingBillingId = _timekeepingBillingId
         timekeeping.TimekeepingBillingStatusId = 1 ' Submitted

         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingBilling
         Dim _payTimekeepingBillingMemberList As New PayTimekeepingBillingMemberList

         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingBillingMember In timekeeping.Schedules
            _schedule.TimekeepingBillingId = _timekeepingBillingId
            _payTimekeepingBillingMemberList.Add(_schedule)
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


            If _payTimekeepingBillingMemberList.Count > 0 Then

               Me.InsertPayTimekeepingBillingMembers(_payTimekeepingBillingMemberList)
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

         Return Me.Ok(timekeeping.TimekeepingBillingId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyTimekeepingBilling")>
   <Route("timekeeping-billing/{timekeepingBillingId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeepingBilling(timekeepingBillingId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingBillingBody) As IHttpActionResult

      If timekeepingBillingId <= 0 Then
         Throw New ArgumentException("Timekeeping Billing ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingBilling
         Dim _payTimekeepingBillingMemberList As New PayTimekeepingBillingMemberList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingBillingMember In timekeeping.Schedules
            _payTimekeepingBillingMemberList.Add(_schedule)
         Next

         '
         ' Load old values from DB
         '
         Dim _payTimekeepingOld As New PayTimekeepingBilling
         Dim _payTimekeepingBillingMemberListOld As New PayTimekeepingBillingMemberList


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeepingBilling(timekeepingBillingId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("timekeeping").Rows(0)
            Me.LoadPayTimekeeping(_row, _payTimekeepingOld)
            Me.LoadPayTimekeepingBillingMemberList(_dataSet.Tables("schedule").Rows, _payTimekeepingBillingMemberListOld)
         End Using




#Region "PayTimekeepingBillingMember"

         '
         ' Apply changes, save to DB
         '

         Dim _removeBillingMemberCount As Integer
         Dim _addBillingMemberCount As Integer
         Dim _editBillingMemberCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingBillingMember In _payTimekeepingBillingMemberListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingBillingMember In _payTimekeepingBillingMemberList
               If _new.BillingMemberId = _old.BillingMemberId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeBillingMemberCount = _removeBillingMemberCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingBillingMember In _payTimekeepingBillingMemberList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingBillingMember In _payTimekeepingBillingMemberListOld
               If _new.BillingMemberId = _old.BillingMemberId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MemberId <> _old.MemberId Then 'OrElse .ScheduleDate <> _old.ScheduleDate OrElse .TimekeepingScheduleId <> _old.TimekeepingScheduleId OrElse .TimekeepingScheduleIdA <> _old.TimekeepingScheduleIdA OrElse .TimekeepingScheduleIdB <> _old.TimekeepingScheduleIdB OrElse .TimekeepingScheduleIdC <> _old.TimekeepingScheduleIdC OrElse .TimekeepingScheduleIdD <> _old.TimekeepingScheduleIdD OrElse .ScheduleTimeIn <> _old.ScheduleTimeIn OrElse .ScheduleTimeOut <> _old.ScheduleTimeOut OrElse .TotalWorkingHour <> _old.TotalWorkingHour Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addBillingMemberCount = _addBillingMemberCount + 1

            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editBillingMemberCount = _editBillingMemberCount + 1
            End If

         Next

         Dim _payTimekeepingBillingMemberListNew As New PayTimekeepingBillingMemberList      ' for adding new Barangays

         If _addBillingMemberCount > 0 Then
            Dim _payTimekeepingBillingMember As PayTimekeepingBillingMember

            For Each _new As PayTimekeepingBillingMember In _payTimekeepingBillingMemberList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingBillingMember = New PayTimekeepingBillingMember
                  _payTimekeepingBillingMemberListNew.Add(_payTimekeepingBillingMember)
                  DataLib.ScatterValues(_new, _payTimekeepingBillingMember)
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

            If _removeBillingMemberCount > 0 Then
               'Me.DeletePayTimekeepingPayOutMembers(_payTimekeepingPayOutMemberListOld)
            End If

            If _addBillingMemberCount > 0 Then
               Me.InsertPayTimekeepingBillingMembers(_payTimekeepingBillingMemberListNew)

            End If

            If _editBillingMemberCount > 0 Then
               Me.UpdatePayTimekeepingBillingMembers(_payTimekeepingBillingMemberList)
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

   '<SymAuthorization("RemoveRecord")>
   '<Route("timekeeping-pay-out/{timekeepingPayOutId}/{lockId}/{currentUserId}")>
   '<HttpDelete>
   'Public Function RemoveRecord(timekeepingPayOutId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

   '   If timekeepingId <= 0 Then
   '      Throw New ArgumentException("Timekeeping ID is required.")
   '   End If

   '   Try

   '      Dim _currentUserId As Integer = 1      ' System (default)
   '      If currentUserId > 0 Then
   '         _currentUserId = currentUserId
   '      End If

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.DeletePayTimekeeping(timekeepingId, lockId)

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
   Private Sub LoadPayTimekeeping(timekeeping As TimekeepingBillingBody, payTimekeeping As PayTimekeepingBilling)

      DataLib.ScatterValues(timekeeping, payTimekeeping)

   End Sub
   Private Sub LoadPayTimekeeping(row As DataRow, timekeeping As PayTimekeepingBilling)

      With timekeeping
         .TimekeepingBillingId = row.ToInt32("TimekeepingBillingId")
         .TimekeepingPayOutId = row.ToInt32("TimekeepingPayOutId")
         .TimekeepingBillingStatusId = row.ToInt32("TimekeepingBillingStatusId")
         .BillingDate = row.ToDate("BillingDate")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub
   Private Sub LoadPayTimekeepingBillingMemberList(rows As DataRowCollection, list As PayTimekeepingBillingMemberList)

      Dim _detail As PayTimekeepingBillingMember
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingBillingMember

         With _detail
            .BillingMemberId = _row.ToInt32("BillingMemberId")
            .TimekeepingBillingId = _row.ToInt32("TimekeepingBillingId")
            .MemberId = _row.ToInt32("MemberId")
            .TotalWorkingDay = _row.ToDecimal("TotalWorkingDay")
            .AllowanceDaily = _row.ToDecimal("AllowanceDaily")
            .TotalWorkingHour = _row.ToDecimal("TotalWorkingHour")
            .PayingDailyRate = _row.ToDecimal("PayingDailyRate")
            .BasicPayOutRate = _row.ToDecimal("BasicPayOutRate")
            .RegularHourRate = _row.ToDecimal("RegularHourRate")
            .OvertimeHourRate = _row.ToDecimal("OvertimeHourRate")
            .LateMinuteRate = _row.ToDecimal("LateMinuteRate")
            .UndertimeMinuteRate = _row.ToDecimal("UndertimeMinuteRate")
            .HU100Rate = _row.ToDecimal("HU100Rate")
            .OTRate = _row.ToDecimal("OTRate")
            .RDRate = _row.ToDecimal("RDRate")
            .RDOTRate = _row.ToDecimal("RDOTRate")
            .SHRate = _row.ToDecimal("SHRate")
            .SHOTRate = _row.ToDecimal("SHOTRate")
            .SHRDRate = _row.ToDecimal("SHRDRate")
            .SHRDOTRate = _row.ToDecimal("SHRDOTRate")
            .LHRate = _row.ToDecimal("LHRate")
            .LHOTRate = _row.ToDecimal("LHOTRate")
            .LHRDRate = _row.ToDecimal("LHRDRate")
            .LHRDOTRate = _row.ToDecimal("LHRDOTRate")
            .LHSHRate = _row.ToDecimal("LHSHRate")
            .LHSHRDRate = _row.ToDecimal("LHSHRDRate")
            .DRHRate = _row.ToDecimal("DRHRate")
            .DRHOTRate = _row.ToDecimal("DRHOTRate")
            .DRHRDRate = _row.ToDecimal("DRHRDRate")
            .DRHRDOTRate = _row.ToDecimal("DRHRDOTRate")
            .NDRate = _row.ToDecimal("NDRate")
            .NDOTRate = _row.ToDecimal("NDOTRate")
            .RDNDRate = _row.ToDecimal("RDNDRate")
            .RDNDOTRate = _row.ToDecimal("RDNDOTRate")
            .SHNDRate = _row.ToDecimal("SHNDRate")
            .SHNDOTRate = _row.ToDecimal("SHNDOTRate")
            .SHRDNDRate = _row.ToDecimal("SHRDNDRate")
            .SHRDNDOTRate = _row.ToDecimal("SHRDNDOTRate")
            .LHNDRate = _row.ToDecimal("LHNDRate")
            .LHNDOTRate = _row.ToDecimal("LHNDOTRate")
            .LHRDNDRate = _row.ToDecimal("LHRDNDRate")
            .LHRDNDOTRate = _row.ToDecimal("LHRDNDOTRate")

            .HU100Hour = _row.ToDecimal("HU100Hour")
            .OTHour = _row.ToDecimal("OTHour")
            .RDHour = _row.ToDecimal("RDHour")
            .RDOTHour = _row.ToDecimal("RDOTHour")
            .SHHour = _row.ToDecimal("SHHour")
            .SHOTHour = _row.ToDecimal("SHOTHour")
            .SHRDHour = _row.ToDecimal("SHRDHour")
            .SHRDOTHour = _row.ToDecimal("SHRDOTHour")
            .LHHour = _row.ToDecimal("LHHour")
            .LHOTHour = _row.ToDecimal("LHOTHour")
            .LHRDHour = _row.ToDecimal("LHRDHour")
            .LHRDOTHour = _row.ToDecimal("LHRDOTHour")
            .LHSHHour = _row.ToDecimal("LHSHHour")
            .LHSHRDHour = _row.ToDecimal("LHSHRDHour")
            .DRHHour = _row.ToDecimal("DRHHour")
            .DRHOTHour = _row.ToDecimal("DRHOTHour")
            .DRHRDHour = _row.ToDecimal("DRHRDHour")
            .DRHRDOTHour = _row.ToDecimal("DRHRDOTHour")
            .NDHour = _row.ToDecimal("NDHour")
            .NDOTHour = _row.ToDecimal("NDOTHour")
            .RDNDHour = _row.ToDecimal("RDNDHour")
            .RDNDOTHour = _row.ToDecimal("RDNDOTHour")
            .SHNDHour = _row.ToDecimal("SHNDHour")
            .SHNDOTHour = _row.ToDecimal("SHNDOTHour")
            .SHRDNDHour = _row.ToDecimal("SHRDNDHour")
            .SHRDNDOTHour = _row.ToDecimal("SHRDNDOTHour")
            .LHNDHour = _row.ToDecimal("LHNDHour")
            .LHNDOTHour = _row.ToDecimal("LHNDOTHour")
            .LHRDNDHour = _row.ToDecimal("LHRDNDHour")
            .LHRDNDOTHour = _row.ToDecimal("LHRDNDOTHour")


            .RDPremiumRate = _row.ToDecimal("RDPremiumRate")
            .SHPremiumRate = _row.ToDecimal("SHPremiumRate")
            .LHPremiumrRate = _row.ToDecimal("LHPremiumrRate")
            .SHRDPremiumRate = _row.ToDecimal("SHRDPremiumRate")
            .LHRDPremiumRate = _row.ToDecimal("LHRDPremiumRate")
            .RDAmount = _row.ToDecimal("RDAmount")
            .SHAmount = _row.ToDecimal("SHAmount")
            .LHAmount = _row.ToDecimal("LHAmount")
            .SHRDAmount = _row.ToDecimal("SHRDAmount")

            .LHRDAmount = _row.ToDecimal("LHRDAmount")
            .ExtendedWorkRate = _row.ToDecimal("ExtendedWorkRate")
            .OtherAllowance = _row.ToDecimal("OtherAllowance")

            .GrossPay = _row.ToDecimal("GrossPay")
            .TotalGrossPay = _row.ToDecimal("TotalGrossPay")
            .AdminFee = _row.ToDecimal("AdminFee")
            .NetBilling = _row.ToDecimal("NetBilling")
            .YEI = _row.ToDecimal("YEI")
            .TotalGrossPayAF = _row.ToDecimal("TotalGrossPayAF")
            .TotalBasic = _row.ToDecimal("TotalBasic")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertPayTimekeeping(timekeeping As PayTimekeepingBilling)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingBilling", timekeeping, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertPayTimekeepingBillingMembers(list As PayTimekeepingBillingMemberList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("BillingMemberId")
         .Add("BasicPayOutRate")
         .Add("MemberName")
         .Add("DepartmentName")
         .Add("HU100Perc")
         .Add("OTPerc")
         .Add("RDPerc")
         .Add("RDOTPerc")
         .Add("SHPerc")
         .Add("SHOTPerc")
         .Add("SHRDPerc")
         .Add("SHRDOTPerc")
         .Add("LHPerc")
         .Add("LHOTPerc")
         .Add("LHRDPerc")
         .Add("LHRDOTPerc")
         .Add("LHSHPerc")
         .Add("LHSHRDPerc")
         .Add("DRHPerc")
         .Add("DRHOTPerc")
         .Add("DRHRDPerc")
         .Add("DRHRDOTPerc")
         .Add("NDPerc")
         .Add("NDOTPerc")
         .Add("RDNDPerc")
         .Add("RDNDOTPerc")
         .Add("SHNDPerc")
         .Add("SHNDOTPerc")
         .Add("SHRDNDPerc")
         .Add("SHRDNDOTPerc")
         .Add("LHNDPerc")
         .Add("LHNDOTPerc")
         .Add("LHRDNDPerc")
         .Add("LHRDNDOTPerc")
         .Add("TotalShare")

         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingBillingMember", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingBillingMember In list
            Me.AddInsertUpdateParamsPayTimekeepingBillingMember(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdatePayTimekeeping(timekeeping As PayTimekeepingBilling)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingBillingId")
         .Add("LockId")
      End With

      'With _excludedFields
      '   .Add("Age")
      '   '.Add("Address2")
      'End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingBilling", timekeeping, _keyFields) ', _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(timekeeping.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsPayTimekeeping(timekeeping As PayTimekeepingBilling)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@TimekeepingBillingId", timekeeping.TimekeepingBillingId)
         .AddWithValue("@TimekeepingPayOutId", timekeeping.TimekeepingPayOutId)
         .AddWithValue("@BillingDate", timekeeping.BillingDate)
         .AddWithValue("@TimekeepingBillingStatusId", timekeeping.TimekeepingBillingStatusId)
            .AddWithValue("@Remarks", timekeeping.Remarks.ToNullable)
        End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingBillingMember(dtl As PayTimekeepingBillingMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingBillingId", dtl.TimekeepingBillingId)
         .AddWithValue("@MemberId", dtl.MemberId)

         .AddWithValue("@TotalWorkingDay", dtl.TotalWorkingDay)
         .AddWithValue("@AllowanceDaily", dtl.AllowanceDaily)
         .AddWithValue("@TotalWorkingHour", dtl.TotalWorkingHour)
         .AddWithValue("@PayingDailyRate", dtl.PayingDailyRate)
         .AddWithValue("@BasicPayOutRate", dtl.BasicPayOutRate)
         .AddWithValue("@RegularHourRate", dtl.RegularHourRate)
         .AddWithValue("@OvertimeHourRate", dtl.OvertimeHourRate)
         .AddWithValue("@LateMinuteRate", dtl.LateMinuteRate)
         .AddWithValue("@UndertimeMinuteRate", dtl.UndertimeMinuteRate)
         .AddWithValue("@HU100Rate", dtl.HU100Rate)
         .AddWithValue("@OTRate", dtl.OTRate)
         .AddWithValue("@RDRate", dtl.RDRate)
         .AddWithValue("@RDOTRate", dtl.RDOTRate)
         .AddWithValue("@SHRate", dtl.SHRate)
         .AddWithValue("@SHOTRate", dtl.SHOTRate)
         .AddWithValue("@SHRDRate", dtl.SHRDRate)
         .AddWithValue("@SHRDOTRate", dtl.SHRDOTRate)
         .AddWithValue("@LHRate", dtl.LHRate)
         .AddWithValue("@LHOTRate", dtl.LHOTRate)
         .AddWithValue("@LHRDRate", dtl.LHRDRate)
         .AddWithValue("@LHRDOTRate", dtl.LHRDOTRate)
         .AddWithValue("@LHSHRate", dtl.LHSHRate)
         .AddWithValue("@LHSHRDRate", dtl.LHSHRDRate)
         .AddWithValue("@DRHRate", dtl.DRHRate)
         .AddWithValue("@DRHOTRate", dtl.DRHOTRate)
         .AddWithValue("@DRHRDRate", dtl.DRHRDRate)
         .AddWithValue("@DRHRDOTRate", dtl.DRHRDOTRate)
         .AddWithValue("@NDRate", dtl.NDRate)
         .AddWithValue("@NDOTRate", dtl.NDOTRate)
         .AddWithValue("@RDNDRate", dtl.RDNDRate)
         .AddWithValue("@RDNDOTRate", dtl.RDNDOTRate)
         .AddWithValue("@SHNDRate", dtl.SHNDRate)
         .AddWithValue("@SHNDOTRate", dtl.SHNDOTRate)
         .AddWithValue("@SHRDNDRate", dtl.SHRDNDRate)
         .AddWithValue("@SHRDNDOTRate", dtl.SHRDNDOTRate)
         .AddWithValue("@LHNDRate", dtl.LHNDRate)
         .AddWithValue("@LHNDOTRate", dtl.LHNDOTRate)
         .AddWithValue("@LHRDNDRate", dtl.LHRDNDRate)
         .AddWithValue("@LHRDNDOTRate", dtl.LHRDNDOTRate)
         .AddWithValue("@OTHour", dtl.OTHour)
         .AddWithValue("@HU100Hour", dtl.HU100Hour)

         .AddWithValue("@RDHour", dtl.RDHour)
         .AddWithValue("@RDOTHour", dtl.RDOTHour)
         .AddWithValue("@SHHour", dtl.SHHour)
         .AddWithValue("@SHOTHour", dtl.SHOTHour)
         .AddWithValue("@SHRDHour", dtl.SHRDHour)
         .AddWithValue("@SHRDOTHour", dtl.SHRDOTHour)
         .AddWithValue("@LHHour", dtl.LHHour)
         .AddWithValue("@LHOTHour", dtl.LHOTHour)
         .AddWithValue("@LHRDHour", dtl.LHRDHour)
         .AddWithValue("@LHRDOTHour", dtl.LHRDOTHour)
         .AddWithValue("@LHSHHour", dtl.LHSHHour)
         .AddWithValue("@LHSHRDHour", dtl.LHSHRDHour)
         .AddWithValue("@DRHHour", dtl.DRHHour)
         .AddWithValue("@DRHOTHour", dtl.DRHOTHour)
         .AddWithValue("@DRHRDHour", dtl.DRHRDHour)
         .AddWithValue("@DRHRDOTHour", dtl.DRHRDOTHour)
         .AddWithValue("@NDHour", dtl.NDHour)
         .AddWithValue("@NDOTHour", dtl.NDOTHour)
         .AddWithValue("@RDNDHour", dtl.RDNDHour)
         .AddWithValue("@RDNDOTHour", dtl.RDNDOTHour)
         .AddWithValue("@SHNDHour", dtl.SHNDHour)
         .AddWithValue("@SHNDOTHour", dtl.SHNDOTHour)
         .AddWithValue("@SHRDNDHour", dtl.SHRDNDHour)
         .AddWithValue("@SHRDNDOTHour", dtl.SHRDNDOTHour)
         .AddWithValue("@LHNDHour", dtl.LHNDHour)
         .AddWithValue("@LHNDOTHour", dtl.LHNDOTHour)
         .AddWithValue("@LHRDNDHour", dtl.LHRDNDHour)
         .AddWithValue("@LHRDNDOTHour", dtl.LHRDNDOTHour)
         .AddWithValue("@RDPremiumRate", dtl.RDPremiumRate)
         .AddWithValue("@SHPremiumRate", dtl.SHPremiumRate)
         .AddWithValue("@LHPremiumrRate", dtl.LHPremiumrRate)
         .AddWithValue("@SHRDPremiumRate", dtl.SHRDPremiumRate)
         .AddWithValue("@LHRDPremiumRate", dtl.LHRDPremiumRate)
         .AddWithValue("@RDAmount", dtl.RDAmount)
         .AddWithValue("@SHAmount", dtl.SHAmount)
         .AddWithValue("@LHAmount", dtl.LHAmount)
         .AddWithValue("@SHRDAmount", dtl.SHRDAmount)
         .AddWithValue("@LHRDAmount", dtl.LHRDAmount)
         .AddWithValue("@ExtendedWorkRate", dtl.ExtendedWorkRate)
         .AddWithValue("@OtherAllowance", dtl.OtherAllowance)


         .AddWithValue("@GrossPay", dtl.GrossPay)
         .AddWithValue("@TotalGrossPay", dtl.TotalGrossPay)
         .AddWithValue("@AdminFee", dtl.AdminFee)
         .AddWithValue("@NetBilling", dtl.NetBilling)
         .AddWithValue("@YEI", dtl.YEI)
         .AddWithValue("@TotalGrossPayAF", dtl.TotalGrossPayAF)
         .AddWithValue("@TotalBasic", dtl.TotalBasic)

      End With

   End Sub
   Private Sub UpdatePayTimekeepingBillingMembers(list As PayTimekeepingBillingMemberList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingMemberId")
         .Add("MemberName")
         .Add("DepartmentName")
         .Add("HU100Perc")
         .Add("OTPerc")
         .Add("RDPerc")
         .Add("RDOTPerc")
         .Add("SHPerc")
         .Add("SHOTPerc")
         .Add("SHRDPerc")
         .Add("SHRDOTPerc")
         .Add("LHPerc")
         .Add("LHOTPerc")
         .Add("LHRDPerc")
         .Add("LHRDOTPerc")
         .Add("LHSHPerc")
         .Add("LHSHRDPerc")
         .Add("DRHPerc")
         .Add("DRHOTPerc")
         .Add("DRHRDPerc")
         .Add("DRHRDOTPerc")
         .Add("NDPerc")
         .Add("NDOTPerc")
         .Add("RDNDPerc")
         .Add("RDNDOTPerc")
         .Add("SHNDPerc")
         .Add("SHNDOTPerc")
         .Add("SHRDNDPerc")
         .Add("SHRDNDOTPerc")
         .Add("LHNDPerc")
         .Add("LHNDOTPerc")
         .Add("LHRDNDPerc")
         .Add("LHRDNDOTPerc")
         .Add("TotalShare")

      End With

      With _excludedFields
         '.Add("ScheduleTimeIn")
         '.Add("ScheduleTimeOut")

         .Add("BasicPayOutRate")

         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingBillingMember", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingBillingMember In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingBillingMember(_detail)
               .Parameters.AddWithValue("@BillingMemberId", _detail.BillingMemberId)
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
   'Private Sub DeletePayTimekeeping(timekeepingId As Integer, lockId As String)

   '   Dim _keyFields As New List(Of String)

   '   With _keyFields
   '      .Add("timekeepingId")
   '      .Add("LockId")
   '   End With

   '   With DataCore.Command
   '      .CommandText = DataLib.BuildDeleteCommandText("dbo.PayTimekeeping", _keyFields)
   '      .CommandType = CommandType.Text

   '      With .Parameters
   '         .Clear()
   '         .AddWithValue("@TimekeepingId", timekeepingId)
   '         .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
   '      End With

   '      .ExecuteNonQuery()

   '   End With

   'End Sub
   'Private Sub DeletePayTimekeepingSheetMembers(list As PayTimekeepingSheetMemberList)

   '   With DataCore.Command
   '      .CommandText = "DELETE dbo.PayTimekeepingSheetMember WHERE SheetMemberId=@SheetMemberId"
   '      .CommandType = CommandType.Text

   '      For Each _old As PayTimekeepingSheetMember In list
   '         If _old.LogActionId = LogActionId.Delete Then
   '            With .Parameters
   '               .Clear()
   '               .AddWithValue("@SheetMemberId", _old.SheetMemberId)
   '            End With

   '            .ExecuteNonQuery()
   '         End If
   '      Next

   '   End With

   'End Sub

End Class
Public Class TimekeepingBillingBody
   Inherits PayTimekeepingBilling

   Public Property Schedules As PayTimekeepingBillingMember()

End Class

