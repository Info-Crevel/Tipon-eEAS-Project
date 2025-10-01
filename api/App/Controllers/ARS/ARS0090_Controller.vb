
Imports ImageMagick

<RoutePrefix("api")>
Public Class ARS0090_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayOutProcess")>
   <Route("timekeeping-pay-out-process/{timekeepingSheetId}")>
   <HttpGet>
   Public Function GetPayOutProcess(timekeepingSheetId As Integer) As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0090_Sheet")
            With _direct
               .AddParameter("TimekeepingSheetId", timekeepingSheetId)

               Dim rateList As New PayTimekeepingPayOutMemberList()

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     ' Iterate through the dataset's rows
                     For Each _row As DataRow In .Tables(0).Rows
                        Dim _rate As New PayTimekeepingPayOutMember

                        With _rate
                           ' Populate the properties with the values from the data row
                           .MemberId = Convert.ToInt32(_row("MemberId").ToString)
                           .MemberName = _row("MemberName").ToString
                           .TotalWorkingHour = Convert.ToDecimal(_row("TotalWorkingHour"))
                           .TotalWorkingDay = Convert.ToDecimal(_row("TotalWorkingHour")) / 8 'Convert.ToDecimal(_row("TotalWorkingDay"))
                           .PayingDailyRate = Convert.ToDecimal(_row("PayingDailyRate"))
                           .BasicPayOutRate = .PayingDailyRate * .TotalWorkingDay
                           .AllowanceDaily = Convert.ToDecimal(_row("AllowanceDaily"))
                           .OTHour = Convert.ToDecimal(_row("OTHour"))
                           .OTRate = .OTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("OTPerc")) / 100
                           .OTPerc = Convert.ToDecimal(_row("OTPerc"))
                           'REST DAY
                           .RDHour = Convert.ToDecimal(_row("RDHour"))
                           .RDRate = (.PayingDailyRate / 8) * .RDHour * Convert.ToDecimal(_row("RDPerc")) / 100 '?100 and 30 how to break
                           'Pending Premium
                           .RDRate = .RDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("RDPerc")) / 100

                           .RDPerc = Convert.ToDecimal(_row("RDPerc"))
                           .RDOTHour = Convert.ToDecimal(_row("RDOTHour"))
                           .RDOTRate = .RDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("RDOTPerc")) / 100
                           .RDOTPerc = Convert.ToDecimal(_row("RDOTPerc"))
                           'Special Holiday			

                           .SHHour = Convert.ToDecimal(_row("SHHour"))
                           .SHRate = (.PayingDailyRate / 8) * .SHHour * Convert.ToDecimal(_row("SHPerc")) / 100 '?100 and 30 how to break
                           'Pending Premium
                           .SHRate = .SHHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHPerc")) / 100
                           .SHPerc = Convert.ToDecimal(_row("SHPerc"))
                           .SHOTHour = Convert.ToDecimal(_row("SHOTHour"))
                           .SHOTRate = .SHOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHOTPerc")) / 100
                           .SHOTPerc = Convert.ToDecimal(_row("SHOTPerc"))

                           'Legal Holiday Unworked			

                           .HU100Hour = Convert.ToDecimal(_row("HU100Hour"))
                           .HU100Rate = (.PayingDailyRate / 8) * .HU100Hour * Convert.ToDecimal(_row("HU100Perc")) / 100 '?100 and 30 how to break
                           .HU100Perc = Convert.ToDecimal(_row("HU100Perc"))
                           'Legal Holiday			

                           .LHHour = Convert.ToDecimal(_row("LHHour"))
                           .LHRate = (.PayingDailyRate / 8) * .LHHour * Convert.ToDecimal(_row("LHPerc")) / 100 '?100 and 30 how to break
                           'Pending Premium
                           .LHRate = .LHHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHPerc")) / 100
                           .LHPerc = Convert.ToDecimal(_row("LHPerc"))
                           .LHOTHour = Convert.ToDecimal(_row("LHOTHour"))
                           .LHOTRate = .LHOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHOTPerc")) / 100
                           .LHOTPerc = Convert.ToDecimal(_row("LHOTPerc"))


                           'Special Holiday Rest Day			

                           .SHRDHour = Convert.ToDecimal(_row("SHRDHour"))
                           .SHRDRate = (.PayingDailyRate / 8) * .SHRDHour * Convert.ToDecimal(_row("SHRDPerc")) / 100 '?100 and 30 how to break
                           'Pending Premium
                           .SHRDRate = .SHRDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHRDPerc")) / 100
                           .SHRDPerc = Convert.ToDecimal(_row("SHRDPerc"))
                           .SHRDOTHour = Convert.ToDecimal(_row("SHRDOTHour"))
                           .SHRDOTRate = .SHRDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHRDOTPerc")) / 100
                           .SHRDOTPerc = Convert.ToDecimal(_row("SHRDOTPerc"))
                           'Legal Holiday	Rest Day		

                           .LHRDHour = Convert.ToDecimal(_row("LHRDHour"))
                           .LHRDRate = (.PayingDailyRate / 8) * .LHRDHour * Convert.ToDecimal(_row("LHRDPerc")) / 100 '?100 and 30 how to break
                           'Pending Premium
                           .LHRDRate = .LHRDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHRDPerc")) / 100
                           .LHRDPerc = Convert.ToDecimal(_row("LHRDPerc"))
                           .LHRDOTHour = Convert.ToDecimal(_row("LHRDOTHour"))
                           .LHRDOTRate = .LHRDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHRDOTPerc")) / 100
                           .LHRDOTPerc = Convert.ToDecimal(_row("LHRDOTPerc"))
                           'Extended Work
                           .ExtendedWorkRate = .OTRate + .RDRate + .RDOTRate + .SHRate + .SHOTRate + .HU100Rate + .LHRate + .LHOTRate + .SHRDRate + .SHRDOTRate + .LHRDRate + .LHRDOTRate

                           'Night Differential
                           .NDHour = Convert.ToDecimal(_row("NDHour"))
                           .NDRate = .NDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("NDPerc")) / 100
                           .NDPerc = Convert.ToDecimal(_row("NDPerc"))
                           'Night Differential OT
                           .NDOTHour = Convert.ToDecimal(_row("NDOTHour"))
                           .NDOTRate = .NDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("NDOTPerc")) / 100
                           .NDOTPerc = Convert.ToDecimal(_row("NDOTPerc"))
                           'Night Differential RD
                           .RDNDHour = Convert.ToDecimal(_row("RDNDHour"))
                           .RDNDRate = .NDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("RDNDPerc")) / 100
                           .RDNDPerc = Convert.ToDecimal(_row("RDNDPerc"))
                           'Night Differential RD OT
                           .RDNDOTHour = Convert.ToDecimal(_row("RDNDOTHour"))
                           .RDNDOTRate = .RDNDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("RDNDOTPerc")) / 100
                           .RDNDOTPerc = Convert.ToDecimal(_row("RDNDOTPerc"))
                           'Night Differential SH
                           .SHNDHour = Convert.ToDecimal(_row("SHNDHour"))
                           .SHNDRate = .SHNDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHNDPerc")) / 100
                           .SHNDPerc = Convert.ToDecimal(_row("SHNDPerc"))
                           'Night Differential SH OT
                           .SHNDOTHour = Convert.ToDecimal(_row("SHNDOTHour"))
                           .SHNDOTRate = .SHNDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHNDOTPerc")) / 100
                           .SHNDOTPerc = Convert.ToDecimal(_row("SHNDOTPerc"))
                           'Night Differential SH RD
                           .SHRDNDHour = Convert.ToDecimal(_row("SHRDNDHour"))
                           .SHRDNDRate = .SHRDNDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHRDNDPerc")) / 100
                           .SHRDNDPerc = Convert.ToDecimal(_row("SHRDNDPerc"))
                           'Night Differential SH RD OT
                           .SHRDNDOTHour = Convert.ToDecimal(_row("SHRDNDOTHour"))
                           .SHRDNDOTRate = .SHRDNDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("SHRDNDOTPerc")) / 100
                           .SHRDNDOTPerc = Convert.ToDecimal(_row("SHRDNDOTPerc"))

                           'Night Differential LH
                           .LHNDHour = Convert.ToDecimal(_row("LHNDHour"))
                           .LHNDRate = .LHNDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHNDPerc")) / 100
                           .LHNDPerc = Convert.ToDecimal(_row("LHNDPerc"))
                           'Night Differential LH OT
                           .LHNDOTHour = Convert.ToDecimal(_row("LHNDOTHour"))
                           .LHNDOTRate = .LHNDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHNDOTPerc")) / 100
                           .LHNDOTPerc = Convert.ToDecimal(_row("LHNDOTPerc"))
                           'Night Differential LH RD
                           .LHRDNDHour = Convert.ToDecimal(_row("LHRDNDHour"))
                           .LHRDNDRate = .LHRDNDHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHRDNDPerc")) / 100
                           .LHRDNDPerc = Convert.ToDecimal(_row("LHRDNDPerc"))
                           'Night Differential LH RD OT
                           .LHRDNDOTHour = Convert.ToDecimal(_row("LHRDNDOTHour"))
                           .LHRDNDOTRate = .LHRDNDOTHour * (.PayingDailyRate / 8) * Convert.ToDecimal(_row("LHRDNDOTPerc")) / 100
                           .LHRDNDOTPerc = Convert.ToDecimal(_row("LHRDNDOTPerc"))
                           'Other Allowance   

                           .OtherAllowance = (.RDHour + .SHHour + .LHHour + .SHRDHour + .LHRDHour) / 8 * .AllowanceDaily

                           '.MemberName = _row("MemberName").ToString


                           .GrossPay = .BasicPayOutRate + .ExtendedWorkRate + .NDRate + .NDOTRate + .RDNDRate + .RDNDOTRate + .SHNDRate + .SHOTRate + .SHRDRate + .SHRDOTRate + .LHNDRate + .LHOTRate + .LHRDRate + .LHRDOTRate + .OtherAllowance

                           .SSSER = Convert.ToDecimal(_row("SSSER"))
                           .SSSEE = Convert.ToDecimal(_row("SSSEE"))
                           .SSSEC = Convert.ToDecimal(_row("SSSEC"))


                           .PFER = Convert.ToDecimal(_row("PFER"))
                           .PFEE = Convert.ToDecimal(_row("PFEE"))

                           .PBGER = Convert.ToDecimal(_row("PBGER"))
                           .PBGEE = Convert.ToDecimal(_row("PBGEE"))

                           .PHHER = Convert.ToDecimal(_row("PHHER"))
                           .PHHEE = Convert.ToDecimal(_row("PHHEE"))


                           .MPL = Convert.ToDecimal(_row("MPL"))
                           .SL4 = Convert.ToDecimal(_row("SL4"))
                           .SL1 = Convert.ToDecimal(_row("SL1"))
                           .SPL = Convert.ToDecimal(_row("SPL"))
                           .EL = Convert.ToDecimal(_row("EL"))
                           .SL3 = Convert.ToDecimal(_row("SL3"))
                           .SAL = Convert.ToDecimal(_row("SAL"))
                           .EP = Convert.ToDecimal(_row("EP"))
                           .CSC = Convert.ToDecimal(_row("CSC"))
                           .PML3 = Convert.ToDecimal(_row("PML3"))
                           .CMF = Convert.ToDecimal(_row("CMF"))
                           .PCL2 = Convert.ToDecimal(_row("PCL2"))
                           .CS = Convert.ToDecimal(_row("CS"))
                           .SSL = Convert.ToDecimal(_row("SSL"))
                           .SSL2 = Convert.ToDecimal(_row("SSL2"))
                           .SCL = Convert.ToDecimal(_row("SCL"))
                           .PML = Convert.ToDecimal(_row("PML"))
                           .PCL = Convert.ToDecimal(_row("PCL"))
                           .PML2 = Convert.ToDecimal(_row("PML2"))
                           .PAI = Convert.ToDecimal(_row("PAI"))
                           .CAN = Convert.ToDecimal(_row("CAN"))
                           .ACC = Convert.ToDecimal(_row("ACC"))
                           .SD = Convert.ToDecimal(_row("SD"))


                           .GrossShare = .GrossPay + .SSSER + .PFER + .SSSEC + .PBGER + .PHHER
                           .TotalShare = .GrossPay


                           .TotalDeduction = .SSSER + .SSSEE + .SSSEC + .PFER + .PFEE + .PBGER + .PBGEE + .PHHER + .PHHEE + .MPL + .SL4 + .SL1 + .SPL + .EL + .SL3 + .SAL + .EP + .CSC + .PML3 + .CMF + .PCL2 + .CS + .SSL + .SSL2 + .SCL + .PML + .PCL + .PML2 + .PAI + .CAN + .ACC + .SD
                           .NetShare = .GrossShare - .TotalDeduction



                           .SSSER = GetSssRate(.GrossShare, Today)
                           .SSSEC = GetEcRate(.GrossShare, Today)
                           .SSSEE = GetWispRate(.GrossShare, Today)


                           .PBGER = GetPbgRate(.GrossShare, Today)
                           .PHHER = GetPhhRate(.GrossShare, Today)

                           '.PayOutSheetFormula = _row("PayOutSheetFormula").ToString()
                           '.A = If(IsDBNull(_row("A")), 0D, Convert.ToDecimal(_row("A")))
                           '.B = If(IsDBNull(_row("B")), 0D, Convert.ToDecimal(_row("B")))
                           '.C = If(IsDBNull(_row("C")), 0D, Convert.ToDecimal(_row("C")))
                           '.D = If(IsDBNull(_row("D")), 0D, Convert.ToDecimal(_row("D")))
                           '.E = If(IsDBNull(_row("E")), 0D, Convert.ToDecimal(_row("E")))
                           '.F = If(IsDBNull(_row("F")), 0D, Convert.ToDecimal(_row("F")))
                           '.G = If(IsDBNull(_row("G")), 0D, Convert.ToDecimal(_row("G")))
                           '.X = If(IsDBNull(_row("X")), 0D, Convert.ToDecimal(_row("X")))
                           '.W = workingDays
                           '.FixedAmount = If(IsDBNull(_row("FixedAmount")), 0D, Convert.ToDecimal(_row("FixedAmount")))
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
                              'File.WriteAllText("e:\" + .PayTrxCode.ToString + ".txt", GetSssRate(result, deploymentDate).ToString)
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

   <SymAuthorization("GetTimekeepingSheet")>
   <Route("timekeeping-sheet-id/{timekeepingSheetId}")>
   <HttpGet>
   Public Function GetTimekeepingSheet(timekeepingSheetId As Integer) As IHttpActionResult


      If timekeepingSheetId <= 0 Then
         Throw New ArgumentException("Client Timekeeping Sheet ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ars0090_Sheet")
            With _direct
               .AddParameter("timekeepingSheetId", timekeepingSheetId)

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

   <SymAuthorization("GetReferences_ars0090")>
   <Route("references/ars0090")>
   <HttpGet>
   Public Function GetReferences_ars0090() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ars0090_References")
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

   <SymAuthorization("GetTimekeepingPayOut")>
   <Route("timekeeping-pay-out/{timekeepingPayOutId}")>
   <HttpGet>
   Public Function GetTimekeepingPayOut(timekeepingPayOutId As Integer) As IHttpActionResult

      If timekeepingPayOutId <= 0 Then
         Throw New ArgumentException("Timekeeping Pay Out ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ars0090")
            With _direct
               .AddParameter("timekeepingPayOutId", timekeepingPayOutId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "timekeeping"
                     .Tables(1).TableName = "schedule"
                     .Tables(2).TableName = "member"
                     .Tables(3).TableName = "payables"

                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateTimekeepingPayOut")>
   <Route("timekeeping-pay-out/{currentUserId}")>
   <HttpPost>
   Public Function CreateTimekeepingPayOut(currentUserId As Integer, <FromBody> timekeeping As TimekeepingPayOutBody) As IHttpActionResult

      If timekeeping.TimekeepingPayOutId <> -1 Then
         Throw New ArgumentException("Timekeeping Pay Out ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _timekeepingPayOutId As Integer = SysLib.GetNextSequence("TimekeepingPayOutId")

         timekeeping.TimekeepingPayOutId = _timekeepingPayOutId
         timekeeping.TimekeepingPayOutStatusId = 1 ' Submitted

         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingPayOut
         Dim _payTimekeepingPayOutMemberList As New PayTimekeepingPayOutMemberList

         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingPayOutMember In timekeeping.Schedules
            _schedule.TimekeepingPayOutId = _timekeepingPayOutId
            _payTimekeepingPayOutMemberList.Add(_schedule)
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


            If _payTimekeepingPayOutMemberList.Count > 0 Then

               Me.InsertPayTimekeepingPayOutMembers(_payTimekeepingPayOutMemberList)
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

         Return Me.Ok(timekeeping.TimekeepingPayOutId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("CreatePayOutPayable")>
   <Route("pay-out-payable/{timekeepingPayOutId}/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayOutPayable(timekeepingPayOutId As Integer, currentUserId As Integer, <FromBody> trx As PayablePayOutBody) As IHttpActionResult

      'If timekeepingPayOutId <> -1 Then
      '   Throw New ArgumentException("Timekeeping Pay Out ID not recognized.")
      'End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _trxId As Integer = SysLib.GetNextSequence("FinTrxId")

         trx.TrxId = _trxId
         'trx.CashReceiptTypeId = 1 'AR PAYMENT
         trx.TrxTypeId = 21 'Accounts Payable
         trx.TrxStatusId = 1 'Posted

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         'If trx.DocumentId = "< NEW >" Then
         trx.DocumentId = AppLib.GetNextTrxTypeSequence(NextSeqId.AccountsPayable)
         'End If

         '
         ' Load proposed values from payload
         '
         Dim _finTrx As New ApsFinTrx
         Dim _finTrxDetailList As New FinTrxDetailList
         'Dim _apsTrx As New ApsTrx
         Dim _apsTrx As New ApsTrx

         'Me.LoadApsFinTrx(trx, _finTrx)


         'Dim _trxDetailId As Integer = SysLib.GetNextSequence("TrxDetailId", trx.Details.Count)
         Dim _creditAmount As Decimal = 0
         Dim _details As String = ""

         For Each _detail As FinTrxDetail In trx.Payables
            _detail.TrxId = _trxId
            '_creditAmount = _detail.CreditAmount
            _details = _detail.Details
            If _detail.DebitAmount > 0 Then
               _detail.CreditAmount = 0
               _finTrxDetailList.Add(_detail)
            End If

         Next

         For Each _detail As FinTrxDetail In trx.Payables
            _detail.TrxId = _trxId
            _creditAmount = _detail.CreditAmount

            If _detail.CreditAmount > 0 Then
               _detail.DebitAmount = 0
               _finTrxDetailList.Add(_detail)
               Exit For
            End If

         Next

         With _apsTrx
            .TrxId = trx.TrxId
            .PayeeId = 1
            .PayableTypeId = 1
            .PayableTaxCode = 0

            .Amount = _creditAmount
         End With


         With _finTrx
            .TrxId = _trxId
            .TrxDate = Today
            .TrxTypeId = 21
            .TrxStatusId = 1
            .DocumentId = trx.DocumentId
            .Reference = timekeepingPayOutId.ToString
            .Particulars = _details
            .PostUserId = currentUserId
         End With


         'For Each _aps As ApsTrx In trx.PayableVoucher
         '   _aps.TrxId = _trxId
         '   _apsTrx.Add(_aps)
         'Next


         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _finTrxLogDetailList As New SysLogDetailList
         Dim _finTrxDetailLogDetailList As New SysLogDetailList

         With _finTrx
            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.TrxDate, String.Empty, .TrxDate.ToDisplayFormat)


            'Dim _statusName As String = EasSession.DbsTrxStatus.Rows.Find(Function(m) m.TrxStatusId = .TrxStatusId).TrxStatusName
            'AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.TrxStatusId, String.Empty, .TrxStatusId.ToString, .TrxStatusId.ToString + "=" + _statusName)

            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.DocumentId, String.Empty, .DocumentId)

            'Dim _costCenterName As String = FrsSession.DbsCostCenter.Rows.Find(Function(m) m.CostCenterId = .CostCenterId).CostCenterShortName
            'AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.CostCenterId, String.Empty, .CostCenterId.ToString, .CostCenterId.ToString + "=" + _costCenterName)

            AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Particulars, String.Empty, .Particulars)

            If Not String.IsNullOrEmpty(.Remarks) Then
               AppLib.AddLogDetail(_finTrxLogDetailList, 0, LogColumnId.Remarks, String.Empty, .Remarks)
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

            Me.InsertFinTrx(_finTrx)
            Me.InsertFinTrxDetails(_finTrxDetailList)
            Me.InsertApsTrx(_apsTrx)


            If _finTrxLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("TrxId", _finTrx.TrxId)
               End With

               _id = AppLib.CreateLogHeader("InsFinTrxLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _finTrxLogDetailList)
               AppLib.CreateLogDetails(_finTrxLogDetailList, "FinTrxLogDetail")

            End If

            For Each _new As FinTrxDetail In _finTrxDetailList

               With _logKeyList
                  .Clear()
                  .Add("TrxId", _new.TrxId)
                  .Add("AccountId", _new.AccountId)
               End With

               _id = AppLib.CreateLogHeader("InsFinTrxDetailLog", _logKeyList, LogActionId.Add, _currentUserId)

               _finTrxDetailLogDetailList.Clear()

               With _new
                  If .DebitAmount > 0 Then
                     AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.DebitAmount, "", .DebitAmount.ToString("N"), "AccountId=" + .AccountId)
                  End If
                  If .CreditAmount > 0 Then
                     AppLib.AddLogDetail(_finTrxDetailLogDetailList, _id, LogColumnId.CreditAmount, "", .CreditAmount.ToString("N"), "AccountId=" + .AccountId)
                  End If

                  AppLib.CreateLogDetails(_finTrxDetailLogDetailList, "FinTrxDetailLogDetail")

               End With
            Next

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

         'Return Me.Ok(True)
         Return Me.Ok(trx.TrxId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   Private Sub LoadApsFinTrx(trx As PayableVoucherTrxBody, finTrx As ApsFinTrx)

      DataLib.ScatterValues(trx, finTrx)

   End Sub

   Private Sub LoadApsFinTrx(row As DataRow, trx As ApsFinTrx)

      With trx
         .TrxId = row.ToInt32("TrxId")
         .TrxDate = row.ToDate("TrxDate")

         .TrxStatusId = row.ToInt32("TrxStatusId")
         .DocumentId = row.ToString("DocumentId")
         '.CostCenterId = row.ToInt32("CostCenterId")
         .Reference = row.ToString("Reference")
         .Particulars = row.ToString("Particulars")
         .BankId = row.ToInt32("BankId")
         .CashReceiptTypeId = row.ToInt32("CashReceiptTypeId")
         .TrxTypeId = row.ToInt32("TrxTypeId")
         .Remarks = row.ToString("Remarks")
         .PostUserId = row.ToInt32("PostUserId")


      End With

   End Sub

   Private Sub LoadFinTrxDetailList(rows As DataRowCollection, list As FinTrxDetailList)

      Dim _detail As FinTrxDetail
      For Each _row As DataRow In rows
         _detail = New FinTrxDetail

         With _detail
            .TrxDetailId = _row.ToInt32("TrxDetailId")
            .TrxId = _row.ToInt32("TrxId")
            .AccountId = _row.ToString("AccountId")
            .DebitAmount = _row.ToDecimal("DebitAmount")
            .CreditAmount = _row.ToDecimal("CreditAmount")

            .OrgId = _row.ToInt32("OrgId")
            .PlatformId = _row.ToInt32("PlatformId")
            .ClusterId = _row.ToInt32("ClusterId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .PayeeId = _row.ToInt32("PayeeId")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertFinTrx(trx As ApsFinTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
         .Add("PayeeId")
         .Add("PayableTypeId")
         .Add("PayableTaxCode")
         .Add("ATaxCode")
         .Add("DueDate")
         .Add("CheckDate")
         .Add("Amount")
         .Add("ApsLockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTrx", trx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsFinTrx(trx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertFinTrxDetails(list As FinTrxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TrxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("Details")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinTrxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As FinTrxDetail In list
            Me.AddInsertUpdateParamsFinTrxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinTrxDetail(dtl As FinTrxDetail)

      With DataCore.Command.Parameters
         .Clear()

         '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
         .AddWithValue("@TrxId", dtl.TrxId)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@DebitAmount", dtl.DebitAmount)
         .AddWithValue("@CreditAmount", dtl.CreditAmount)

         .AddWithValue("@OrgId", dtl.OrgId)
         .AddWithValue("@PlatformId", dtl.PlatformId)
         .AddWithValue("@ClusterId", dtl.ClusterId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@PayeeId", dtl.PayeeId)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsFinTrx(trx As ApsFinTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TrxId", trx.TrxId)
         .AddWithValue("@TrxDate", trx.TrxDate)
         .AddWithValue("@TrxStatusId", trx.TrxStatusId)
         .AddWithValue("@DocumentId", trx.DocumentId)
         .AddWithValue("@Reference", trx.Reference)
         .AddWithValue("@Particulars", trx.Particulars)
         .AddWithValue("@Remarks", trx.Remarks.ToNullable)
         .AddWithValue("@CashReceiptTypeId", trx.CashReceiptTypeId.ToNullable)
         .AddWithValue("@TrxTypeId", trx.TrxTypeId)
         .AddWithValue("@BankId", trx.BankId.ToNullable)
         .AddWithValue("@DisbursementTypeId", trx.DisbursementTypeId.ToNullable)
         .AddWithValue("@PostUserId", trx.PostUserId)

      End With

   End Sub

   Private Sub InsertApsTrx(aps As ApsTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")

      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsTrx", aps, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsTrx(aps)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsTrx(aps As ApsTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TrxId", aps.TrxId)
         .AddWithValue("@PayeeId", aps.PayeeId)
         .AddWithValue("@PayableTypeId", aps.PayableTypeId)
         .AddWithValue("@PayableTaxCode", aps.PayableTaxCode.ToNullable)
         .AddWithValue("@ATaxCode", aps.ATaxCode.ToNullable)
         .AddWithValue("@Details", aps.Details.ToNullable)
         .AddWithValue("@DueDate", aps.DueDate.ToNullable)
         .AddWithValue("@CheckDate", aps.CheckDate.ToNullable)
         .AddWithValue("@Amount", aps.Amount)
      End With

   End Sub


   <SymAuthorization("ModifyTimekeepingPayOut")>
   <Route("timekeeping-pay-out/{timekeepingPayOutId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyTimekeepingRecord(timekeepingPayOutId As Integer, currentUserId As Integer, <FromBody> timekeeping As TimekeepingPayOutBody) As IHttpActionResult

      If timekeepingPayOutId <= 0 Then
         Throw New ArgumentException("Timekeeping Pay Out ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '

         Dim _payTimekeeping As New PayTimekeepingPayOut
         Dim _payTimekeepingPayOutMemberList As New PayTimekeepingPayOutMemberList


         Me.LoadPayTimekeeping(timekeeping, _payTimekeeping)


         For Each _schedule As PayTimekeepingPayOutMember In timekeeping.Schedules
            _payTimekeepingPayOutMemberList.Add(_schedule)
         Next

         '
         ' Load old values from DB
         '
         Dim _payTimekeepingOld As New PayTimekeepingPayOut
         Dim _payTimekeepingPayOutMemberListOld As New PayTimekeepingPayOutMemberList


         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetTimekeepingPayOut(timekeepingPayOutId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("timekeeping").Rows(0)
            Me.LoadPayTimekeeping(_row, _payTimekeepingOld)
            Me.LoadPayTimekeepingPayOutMemberList(_dataSet.Tables("schedule").Rows, _payTimekeepingPayOutMemberListOld)
         End Using




#Region "PayTimekeepingPayOutMember"

         '
         ' Apply changes, save to DB
         '

         Dim _removePayOutMemberCount As Integer
         Dim _addPayOutMemberCount As Integer
         Dim _editPayOutMemberCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As PayTimekeepingPayOutMember In _payTimekeepingPayOutMemberListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As PayTimekeepingPayOutMember In _payTimekeepingPayOutMemberList
               If _new.PayOutMemberId = _old.PayOutMemberId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removePayOutMemberCount = _removePayOutMemberCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As PayTimekeepingPayOutMember In _payTimekeepingPayOutMemberList
            _new.LogActionId = LogActionId.Add
            For Each _old As PayTimekeepingPayOutMember In _payTimekeepingPayOutMemberListOld
               If _new.PayOutMemberId = _old.PayOutMemberId Then
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
               _addPayOutMemberCount = _addPayOutMemberCount + 1

            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editPayOutMemberCount = _editPayOutMemberCount + 1
            End If

         Next

         Dim _payTimekeepingPayOutMemberListNew As New PayTimekeepingPayOutMemberList      ' for adding new Barangays

         If _addPayOutMemberCount > 0 Then
            Dim _payTimekeepingPayOutMember As PayTimekeepingPayOutMember

            For Each _new As PayTimekeepingPayOutMember In _payTimekeepingPayOutMemberList
               If _new.LogActionId = LogActionId.Add Then
                  _payTimekeepingPayOutMember = New PayTimekeepingPayOutMember
                  _payTimekeepingPayOutMemberListNew.Add(_payTimekeepingPayOutMember)
                  DataLib.ScatterValues(_new, _payTimekeepingPayOutMember)
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

            If _removePayOutMemberCount > 0 Then
               'Me.DeletePayTimekeepingPayOutMembers(_payTimekeepingPayOutMemberListOld)
            End If

            If _addPayOutMemberCount > 0 Then
               Me.InsertPayTimekeepingPayOutMembers(_payTimekeepingPayOutMemberListNew)

            End If

            If _editPayOutMemberCount > 0 Then
               Me.UpdatePayTimekeepingPayOutMembers(_payTimekeepingPayOutMemberList)
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
   Private Sub LoadPayTimekeeping(timekeeping As TimekeepingPayOutBody, payTimekeeping As PayTimekeepingPayOut)

      DataLib.ScatterValues(timekeeping, payTimekeeping)

   End Sub
   Private Sub LoadPayTimekeeping(row As DataRow, timekeeping As PayTimekeepingPayOut)

      With timekeeping
         .TimekeepingPayOutId = row.ToInt32("TimekeepingPayOutId")
         .TimekeepingSheetId = row.ToInt32("TimekeepingSheetId")
         .TimekeepingPayOutStatusId = row.ToInt32("TimekeepingPayOutStatusId")
         .PayOutDate = row.ToDate("PayOutDate")
         .Remarks = row.ToString("Remarks")
         .TrxId = row.ToInt32("TrxId")
      End With

   End Sub
   Private Sub LoadPayTimekeepingPayOutMemberList(rows As DataRowCollection, list As PayTimekeepingPayOutMemberList)

      Dim _detail As PayTimekeepingPayOutMember
      For Each _row As DataRow In rows
         _detail = New PayTimekeepingPayOutMember

         With _detail
            .PayOutMemberId = _row.ToInt32("PayOutMemberId")
            .TimekeepingPayOutId = _row.ToInt32("TimekeepingPayOutId")
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
            .SSSER = _row.ToDecimal("SSSER")
            .SSSEE = _row.ToDecimal("SSSEE")
            .SSSEC = _row.ToDecimal("SSSEC")
            .PFER = _row.ToDecimal("PFER")
            .PFEE = _row.ToDecimal("PFEE")
            .PBGER = _row.ToDecimal("PBGER")
            .PBGEE = _row.ToDecimal("PBGEE")
            .PHHER = _row.ToDecimal("PHHER")
            .PHHEE = _row.ToDecimal("PHHEE")
            .GrossPay = _row.ToDecimal("GrossPay")
            .GrossShare = _row.ToDecimal("GrossShare")
            .TotalShare = _row.ToDecimal("TotalShare")
            .MPL = _row.ToDecimal("MPL")
            .SL4 = _row.ToDecimal("SL4")
            .SL1 = _row.ToDecimal("SL1")
            .SPL = _row.ToDecimal("SPL")
            .EL = _row.ToDecimal("EL")
            .SL3 = _row.ToDecimal("SL3")
            .SAL = _row.ToDecimal("SAL")
            .EP = _row.ToDecimal("EP")
            .CSC = _row.ToDecimal("CSC")
            .PML3 = _row.ToDecimal("PML3")
            .CMF = _row.ToDecimal("CMF")
            .PCL2 = _row.ToDecimal("PCL2")
            .CS = _row.ToDecimal("CS")
            .SSL = _row.ToDecimal("SSL")
            .SSL2 = _row.ToDecimal("SSL2")
            .SCL = _row.ToDecimal("SCL")
            .PML = _row.ToDecimal("PML")
            .PCL = _row.ToDecimal("PCL")
            .PML2 = _row.ToDecimal("PML2")
            .PAI = _row.ToDecimal("PAI")
            .CAN = _row.ToDecimal("CAN")
            .ACC = _row.ToDecimal("ACC")
            .SD = _row.ToDecimal("SD")
            .TotalDeduction = _row.ToDecimal("TotalDeduction")
            .NetShare = _row.ToDecimal("NetShare")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertPayTimekeeping(timekeeping As PayTimekeepingPayOut)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingPayOut", timekeeping, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertPayTimekeepingPayOutMembers(list As PayTimekeepingPayOutMemberList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("PayOutMemberId")
         .Add("BasicPayOutRate")
         .Add("MemberName")
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


         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.PayTimekeepingPayOutMember", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingPayOutMember In list
            Me.AddInsertUpdateParamsPayTimekeepingPayOutMember(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdatePayTimekeeping(timekeeping As PayTimekeepingPayOut)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("TimekeepingPayOutId")
         .Add("LockId")
      End With

      'With _excludedFields
      '   .Add("Age")
      '   '.Add("Address2")
      'End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingPayOut", timekeeping, _keyFields) ', _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayTimekeeping(timekeeping)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(timekeeping.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsPayTimekeeping(timekeeping As PayTimekeepingPayOut)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingPayOutId", timekeeping.TimekeepingPayOutId)
         .AddWithValue("@TimekeepingSheetId", timekeeping.TimekeepingSheetId)
         .AddWithValue("@PayOutDate", timekeeping.PayOutDate)
         .AddWithValue("@TimekeepingPayOutStatusId", timekeeping.TimekeepingPayOutStatusId)
         .AddWithValue("@Remarks", timekeeping.Remarks)
         .AddWithValue("@TrxId", timekeeping.TrxId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayTimekeepingPayOutMember(dtl As PayTimekeepingPayOutMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@TimekeepingPayOutId", dtl.TimekeepingPayOutId)
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

         .AddWithValue("@SSSER", dtl.SSSER)
         .AddWithValue("@SSSEE", dtl.SSSEE)
         .AddWithValue("@SSSEC", dtl.SSSEC)
         .AddWithValue("@PFER", dtl.PFER)
         .AddWithValue("@PFEE", dtl.PFEE)
         .AddWithValue("@PBGER", dtl.PBGER)
         .AddWithValue("@PBGEE", dtl.PBGEE)
         .AddWithValue("@PHHER", dtl.PHHER)
         .AddWithValue("@PHHEE", dtl.PHHEE)
         .AddWithValue("@GrossPay", dtl.GrossPay)
         .AddWithValue("@GrossShare", dtl.GrossShare)
         .AddWithValue("@TotalShare", dtl.TotalShare)
         .AddWithValue("@MPL", dtl.MPL)
         .AddWithValue("@SL4", dtl.SL4)
         .AddWithValue("@SL1", dtl.SL1)
         .AddWithValue("@SPL", dtl.SPL)
         .AddWithValue("@EL", dtl.EL)
         .AddWithValue("@SL3", dtl.SL3)
         .AddWithValue("@SAL", dtl.SAL)
         .AddWithValue("@EP", dtl.EP)
         .AddWithValue("@CSC", dtl.CSC)
         .AddWithValue("@PML3", dtl.PML3)
         .AddWithValue("@CMF", dtl.CMF)
         .AddWithValue("@PCL2", dtl.PCL2)
         .AddWithValue("@CS", dtl.CS)
         .AddWithValue("@SSL", dtl.SSL)
         .AddWithValue("@SSL2", dtl.SSL2)
         .AddWithValue("@SCL", dtl.SCL)
         .AddWithValue("@PML", dtl.PML)
         .AddWithValue("@PCL", dtl.PCL)
         .AddWithValue("@PML2", dtl.PML2)
         .AddWithValue("@PAI", dtl.PAI)
         .AddWithValue("@CAN", dtl.CAN)
         .AddWithValue("@ACC", dtl.ACC)
         .AddWithValue("@SD", dtl.SD)
         .AddWithValue("@TotalDeduction", dtl.TotalDeduction)
         .AddWithValue("@NetShare", dtl.NetShare)

      End With

   End Sub
   Private Sub UpdatePayTimekeepingPayOutMembers(list As PayTimekeepingPayOutMemberList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PayOutMemberId")
         .Add("MemberName")
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

      End With

      With _excludedFields
         '.Add("ScheduleTimeIn")
         '.Add("ScheduleTimeOut")

         .Add("BasicPayOutRate")

         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.PayTimekeepingPayOutMember", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As PayTimekeepingPayOutMember In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsPayTimekeepingPayOutMember(_detail)
               .Parameters.AddWithValue("@PayOutMemberId", _detail.PayOutMemberId)
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

   Friend Shared Function GetSssRate(rate As Decimal, sysDate As Date) As Decimal
      Try
         Using _direct As New SqlDirect("web.ARS0030_SssRate")
            With _direct
               .AddParameter("Rate", rate)
               .AddParameter("SysDate", sysDate)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  If _dataSet.Tables.Count > 0 AndAlso _dataSet.Tables(0).Rows.Count > 0 Then
                     Dim sSSEmployerAmount As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("SSEmployerAmount"))
                     Return sSSEmployerAmount
                  Else

                     Return 0
                  End If
               End Using
            End With
         End Using
      Catch ex As Exception
         Return 0
      End Try
   End Function
   Friend Shared Function GetPbgRate(rate As Decimal, sysDate As Date) As Decimal
      Try
         Using _direct As New SqlDirect("web.ARS0030_PbgRate")
            With _direct
               .AddParameter("Rate", rate)
               .AddParameter("SysDate", sysDate)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  If _dataSet.Tables.Count > 0 AndAlso _dataSet.Tables(0).Rows.Count > 0 Then
                     Dim employeeRate As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("EmployeeRate"))
                     Dim employerRate As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("EmployerRate"))
                     Dim maxRate As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("MaxRate"))
                     Dim pbgRate As Decimal = 0
                     If maxRate > 0 Then

                        pbgRate = maxRate * (employerRate / 100)

                     Else
                        pbgRate = rate * (employerRate / 100)
                     End If
                     Return pbgRate
                  Else

                     Return 0
                  End If
               End Using
            End With
         End Using
      Catch ex As Exception
         Return 0
      End Try
   End Function

   Friend Shared Function GetEcRate(rate As Decimal, sysDate As Date) As Decimal
      Try
         Using _direct As New SqlDirect("web.ARS0030_SssRate")
            With _direct
               .AddParameter("Rate", rate)
               .AddParameter("SysDate", sysDate)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  If _dataSet.Tables.Count > 0 AndAlso _dataSet.Tables(0).Rows.Count > 0 Then
                     Dim sSSEmployerAmount As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("EcEmployerAmount"))
                     Return sSSEmployerAmount
                  Else

                     Return 0
                  End If
               End Using
            End With
         End Using
      Catch ex As Exception
         Return 0
      End Try
   End Function
   Friend Shared Function GetWispRate(rate As Decimal, sysDate As Date) As Decimal
      Try
         Using _direct As New SqlDirect("web.ARS0030_WispRate")
            With _direct
               .AddParameter("Rate", rate)
               .AddParameter("SysDate", sysDate)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  If _dataSet.Tables.Count > 0 AndAlso _dataSet.Tables(0).Rows.Count > 0 Then
                     Dim wISPEmployerAmount As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("WISPEmployerAmount"))
                     Dim wISPEmployeeAmount As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("WISPEmployeeAmount"))
                     Return wISPEmployerAmount
                  Else

                     Return 0
                  End If
               End Using
            End With
         End Using
      Catch ex As Exception
         Return 0
      End Try
   End Function
   Friend Shared Function GetPhhRate(rate As Decimal, sysDate As Date) As Decimal
      Try
         Using _direct As New SqlDirect("web.ARS0030_PhhRate")
            With _direct
               .AddParameter("Rate", rate)
               .AddParameter("SysDate", sysDate)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  If _dataSet.Tables.Count > 0 AndAlso _dataSet.Tables(0).Rows.Count > 0 Then
                     Dim premiumRate As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("PremiumRate"))
                     Dim fixedAmount As Decimal = Convert.ToDecimal(_dataSet.Tables(0).Rows(0)("FixedAmount"))
                     Dim phhRate As Decimal = 0
                     If fixedAmount > 0 Then

                        phhRate = fixedAmount

                     Else
                        phhRate = rate * (premiumRate / 100)
                     End If
                     Return phhRate
                  Else

                     Return 0
                  End If
               End Using
            End With
         End Using
      Catch ex As Exception
         Return 0
      End Try
   End Function

End Class
Public Class TimekeepingPayOutBody
   Inherits PayTimekeepingPayOut

   Public Property Schedules As PayTimekeepingPayOutMember()

End Class

Public Class PayablePayOutBody
   Inherits FinTrx

   Public Property Payables As FinTrxDetail()

End Class

