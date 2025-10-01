<RoutePrefix("api")>
Public Class ARS0040_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetClientPaygroupProvinces")>
   <Route("client-pay-group-provinces/{regionId}")>
   <HttpGet>
   Public Function GetClientPaygroupProvinces(regionId As Integer) As IHttpActionResult
      Dim _filter As String = "RegionId=" + regionId.ToString
      Dim _dataSource As String = "dbo.DbsProvince"
      Dim _fields As String = "ProvinceId, ProvinceName, RegionId"
      Dim _sort As String = "ProvinceName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetClientPaygroupMunicipalities")>
   <Route("client-pay-group-municipalities/{regionId}/{provinceId}")>
   <HttpGet>
   Public Function GetClientPaygroupMunicipalities(regionId As Integer, provinceId As Integer) As IHttpActionResult

      Dim _filter As String = "RegionId=" + regionId.ToString + " AND ProvinceId=" + provinceId.ToString
      Dim _dataSource As String = "dbo.QDbsMunicipality"
      Dim _fields As String = "MunicipalityId, MunicipalityName, ProvinceId, RegionId"
      Dim _sort As String = "MunicipalityName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   <SymAuthorization("GetReferences_ARS0040")>
    <Route("references/ars0040")>
    <HttpGet>
    Public Function GetReferences_ARS0030() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.ARS0040_References")
                With _direct

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "contract"     ' all defined Transaction Types
                            .Tables(1).TableName = "payOutComputation"     ' all defined Transaction Types
                            .Tables(2).TableName = "billingComputation"     ' all defined Transaction Types
                            .Tables(3).TableName = "payFreq"     ' all defined Transaction Types
                            .Tables(4).TableName = "payTrx"     ' all defined Transaction Types
                            .Tables(5).TableName = "payOutSheet"     ' all defined Transaction Types
                            .Tables(6).TableName = "billingSheet"     ' all defined Transaction Types
                            .Tables(7).TableName = "chargingConsideration"     ' all defined Transaction Types
                            .Tables(8).TableName = "overtimeSheet"     ' all defined Transaction Types
                            .Tables(9).TableName = "dayTypeSheet"     ' all defined Transaction Types
                     .Tables(10).TableName = "region"     ' all defined Transaction Types
                     .Tables(11).TableName = "holidays"     ' all defined Transaction Types
                  End With

                        Return Me.Ok(_dataSet)

                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetClientPayGroup")>
    <Route("client-pay-groups/{clientPayGroupId}")>
    <HttpGet>
    Public Function GetClientPayGroup(clientPayGroupId As Integer) As IHttpActionResult

        If clientPayGroupId <= 0 Then
            Throw New ArgumentException("Client Pay Group ID is required.")
        End If

      Try
         Using _direct As New SqlDirect("web.ARS0040")
            With _direct
               .AddParameter("ClientPayGroupId", clientPayGroupId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "payGroup"
                     .Tables(1).TableName = "payOuts"
                     .Tables(2).TableName = "billings"
                     .Tables(3).TableName = "overtimes"
                     .Tables(4).TableName = "dayTypes"
                     .Tables(5).TableName = "mrfs"
                     .Tables(6).TableName = "province"
                     .Tables(7).TableName = "municipality"
                     .Tables(8).TableName = "payOutDayTypes"
                     .Tables(9).TableName = "holidays"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

    <SymAuthorization("CreateClientPayGroup")>
    <Route("client-pay-groups/{currentUserId}")>
    <HttpPost>
    Public Function CreateClientPayGroup(currentUserId As Integer, <FromBody> clientPayGroup As ArsClientPayGroupBody) As IHttpActionResult

        If clientPayGroup.ClientPayGroupId <> -1 Then
            Throw New ArgumentException("Client Pay Group ID not recognized.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)

            If currentUserId > 0 Then
                _currentUserId = currentUserId
            End If

            '
            ' Assign new ID from sequencer
            '
            Dim _clientPayGroupId As Integer = SysLib.GetNextSequence("ClientPayGroupId")

            clientPayGroup.ClientPayGroupId = _clientPayGroupId
            clientPayGroup.ClientPayGroupStatusId = ClientPayGroupStatus.Active
            'memberRequest.MemberRequestStatusId = MemberRequestStatus.InActive

            '
            ' Load proposed values from payload
            '
            Dim _arsClientPayGroup As New ArsClientPayGroup
            Dim _arsClientPayOutList As New ArsClientPayOutList
         'Dim _arsClientBillingList As New ArsClientBillingList
         Dim _arsClientOvertimeList As New ArsClientOvertimeList
            Dim _arsClientDayTypeList As New ArsClientDayTypeList
         Dim _arsClientPayOutDayTypeList As New ArsClientPayOutDayTypeList
         Dim _arsClientPayOutHolidayList As New ArsClientPayOutHolidayList


         Me.LoadArsClientPayGroup(clientPayGroup, _arsClientPayGroup)

            For Each _payOut As ArsClientPayOut In clientPayGroup.PayOuts
                _payOut.ClientPayGroupId = _clientPayGroupId
                _arsClientPayOutList.Add(_payOut)
            Next

         'For Each _billing As ArsClientBilling In clientPayGroup.Billings
         '    _billing.ClientPayGroupId = _clientPayGroupId
         '    _arsClientBillingList.Add(_billing)
         'Next

         For Each _overTime As ArsClientOvertime In clientPayGroup.Overtimes
                _overTime.ClientPayGroupId = _clientPayGroupId
                _arsClientOvertimeList.Add(_overTime)
            Next

            For Each _dayType As ArsClientDayType In clientPayGroup.DayTypes
                _dayType.ClientPayGroupId = _clientPayGroupId
                _arsClientDayTypeList.Add(_dayType)
            Next

         For Each _payOutDayType As ArsClientPayOutDayType In clientPayGroup.PayOutDayTypes
            _payOutDayType.ClientPayGroupId = _clientPayGroupId
            _arsClientPayOutDayTypeList.Add(_payOutDayType)
         Next

         For Each _holiday As ArsClientPayOutHoliday In clientPayGroup.Holidays
            _holiday.ClientPayGroupId = _clientPayGroupId
            _arsClientPayOutHolidayList.Add(_holiday)
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

                Me.InsertArsClientPayGroup(_arsClientPayGroup)

                If _arsClientPayOutList.Count > 0 Then
                    Me.InsertArsClientPayOuts(_arsClientPayOutList)
                End If

            'If _arsClientBillingList.Count > 0 Then
            '    Me.InsertArsClientBillings(_arsClientBillingList)
            'End If

            If _arsClientOvertimeList.Count > 0 Then
                    Me.InsertArsClientOvertimes(_arsClientOvertimeList)
                End If

                If _arsClientDayTypeList.Count > 0 Then
                    Me.InsertArsClientDayTypes(_arsClientDayTypeList)
                End If

            If _arsClientPayOutDayTypeList.Count > 0 Then
               Me.InsertArsClientPayOutDayTypes(_arsClientPayOutDayTypeList)
            End If

            If _arsClientPayOutHolidayList.Count > 0 Then
               Me.InsertArsClientPayOutHolidays(_arsClientPayOutHolidayList)
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

            Return Me.Ok(clientPayGroup.ClientPayGroupId)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("ModifyArsClientPayGroup")>
    <Route("client-pay-groups/{clientPayGroupId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyArsMemberRequest(clientPayGroupId As Integer, currentUserId As Integer, <FromBody> clientPayGroup As ArsClientPayGroupBody) As IHttpActionResult

        If clientPayGroupId <= 0 Then
            Throw New ArgumentException("Client Pay Group ID is required.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)

            If currentUserId > 0 Then
                _currentUserId = currentUserId
            End If

         '
         ' Load proposed values from payload
         '
         Dim _arsClientPayGroup As New ArsClientPayGroup
         Dim _arsClientPayOutList As New ArsClientPayOutList
         'Dim _arsClientBillingList As New ArsClientBillingList
         Dim _arsClientOvertimeList As New ArsClientOvertimeList
         Dim _arsClientDayTypeList As New ArsClientDayTypeList
         Dim _arsClientPayOutDayTypeList As New ArsClientPayOutDayTypeList
         Dim _arsClientPayOutHolidayList As New ArsClientPayOutHolidayList

         Me.LoadArsClientPayGroup(clientPayGroup, _arsClientPayGroup)

            For Each _payOut As ArsClientPayOut In clientPayGroup.PayOuts
                _payOut.ClientPayGroupId = clientPayGroupId
                _arsClientPayOutList.Add(_payOut)
            Next

         'For Each _billing As ArsClientBilling In clientPayGroup.Billings
         '    _billing.ClientPayGroupId = clientPayGroupId
         '    _arsClientBillingList.Add(_billing)
         'Next

         For Each _overtime As ArsClientOvertime In clientPayGroup.Overtimes
                _overtime.ClientPayGroupId = clientPayGroupId
                _arsClientOvertimeList.Add(_overtime)
            Next

            For Each _dayType As ArsClientDayType In clientPayGroup.DayTypes
                _dayType.ClientPayGroupId = clientPayGroupId
                _arsClientDayTypeList.Add(_dayType)
            Next

         For Each _payOutDayType As ArsClientPayOutDayType In clientPayGroup.PayOutDayTypes
            _payOutDayType.ClientPayGroupId = clientPayGroupId
            _arsClientPayOutDayTypeList.Add(_payOutDayType)
         Next

         For Each _holiday As ArsClientPayOutHoliday In clientPayGroup.Holidays
            _holiday.ClientPayGroupId = clientPayGroupId
            _arsClientPayOutHolidayList.Add(_holiday)
         Next

         '
         ' Load old values from DB
         '
         Dim _arsclientPayGroupOld As New ArsClientPayGroup
            Dim _arsClientPayOutListOld As New ArsClientPayOutList
         'Dim _arsClientBillingListOld As New ArsClientBillingList
         Dim _arsClientOvertimeListOld As New ArsClientOvertimeList
            Dim _arsClientDayTypeListOld As New ArsClientDayTypeList
         Dim _arsClientPayOutDayTypeListOld As New ArsClientPayOutDayTypeList
         Dim _arsClientPayOutHolidayListOld As New ArsClientPayOutHolidayList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetClientPayGroup(clientPayGroupId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
                Dim _row As DataRow = _dataSet.Tables("payGroup").Rows(0)
                Me.LoadArsClientPayGroup(_row, _arsclientPayGroupOld)
                Me.LoadArsClientPayOutList(_dataSet.Tables("payOuts").Rows, _arsClientPayOutListOld)
            'Me.LoadArsClientBillingList(_dataSet.Tables("billings").Rows, _arsClientBillingListOld)
            Me.LoadArsClientOvertimeList(_dataSet.Tables("overtimes").Rows, _arsClientOvertimeListOld)
                Me.LoadArsClientDayTypeList(_dataSet.Tables("dayTypes").Rows, _arsClientDayTypeListOld)
            Me.LoadArsClientPayOutDayTypeList(_dataSet.Tables("payOutDayTypes").Rows, _arsClientPayOutDayTypeListOld)
            Me.LoadArsClientPayOutHolidayList(_dataSet.Tables("holidays").Rows, _arsClientPayOutHolidayListOld)

         End Using


#Region "ArsClientPayOut"

            Dim _removeClientPayOutCount As Integer
            Dim _addClientPayOutCount As Integer
            Dim _editClientPayOutCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientPayOut In _arsClientPayOutListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientPayOut In _arsClientPayOutList
                    If _new.ClientPayOutDetailId = _old.ClientPayOutDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeClientPayOutCount = _removeClientPayOutCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientPayOut In _arsClientPayOutList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientPayOut In _arsClientPayOutListOld
                    If _new.ClientPayOutDetailId = _old.ClientPayOutDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                            If .PayTrxCode <> _old.PayTrxCode OrElse .PayOutSheetFormula <> _old.PayOutSheetFormula OrElse .FixedAmount <> _old.FixedAmount OrElse .BasicFlag <> _old.BasicFlag OrElse .DeminimisFlag <> _old.DeminimisFlag OrElse .AllowanceFlag <> _old.AllowanceFlag OrElse .OvertimeFlag <> _old.OvertimeFlag OrElse .NightDifferentialFlag <> _old.NightDifferentialFlag OrElse .HolidayFlag <> _old.HolidayFlag OrElse .RestDayFlag <> _old.RestDayFlag Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With



                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    'File.WriteAllText("e:\JENON.txt", _new.PayTrxCode)
                    _addClientPayOutCount = _addClientPayOutCount + 1

                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editClientPayOutCount = _editClientPayOutCount + 1
                End If

            Next

            Dim _arsClientPayOutListNew As New ArsClientPayOutList      ' for adding new Template Details

            If _addClientPayOutCount > 0 Then
                Dim _arsClientPayOut As ArsClientPayOut

                For Each _new As ArsClientPayOut In _arsClientPayOutList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientPayOut = New ArsClientPayOut
                        _arsClientPayOutListNew.Add(_arsClientPayOut)
                        DataLib.ScatterValues(_new, _arsClientPayOut)
                        _arsClientPayOut.ClientPayGroupId = _arsClientPayOut.ClientPayGroupId
                    End If
                Next

            End If

#End Region

         '#Region "ArsClientBilling"

         '            Dim _removeClientBillingCount As Integer
         '            Dim _addClientBillingCount As Integer
         '            Dim _editClientBillingCount As Integer
         '            ' Mark details for deletion if not found in new list

         '            For Each _old As ArsClientBilling In _arsClientBillingListOld
         '                _old.LogActionId = LogActionId.Delete
         '                For Each _new As ArsClientBilling In _arsClientBillingList
         '                    If _new.ClientBillingDetailId = _old.ClientBillingDetailId Then
         '                        _old.LogActionId = 0  ' retain
         '                    End If
         '                Next

         '                If _old.LogActionId = LogActionId.Delete Then
         '                    _removeClientBillingCount = _removeClientBillingCount + 1
         '                End If
         '            Next

         '            ' Mark details for addition if not found in old list;
         '            ' Mark details for modification if found in old list but with at least 1 property mismatch

         '            For Each _new As ArsClientBilling In _arsClientBillingList
         '                _new.LogActionId = LogActionId.Add
         '                For Each _old As ArsClientBilling In _arsClientBillingListOld
         '                    If _new.ClientBillingDetailId = _old.ClientBillingDetailId Then
         '                        _new.LogActionId = 0   ' don't add

         '                        With _new
         '                            If .PayTrxCode <> _old.PayTrxCode OrElse .ChargingConsiderationId <> _old.ChargingConsiderationId OrElse .BillingSheetFormula <> _old.BillingSheetFormula OrElse .FixedAmount <> _old.FixedAmount OrElse .BasicFlag <> _old.BasicFlag OrElse .DeminimisFlag <> _old.DeminimisFlag OrElse .AllowanceFlag <> _old.AllowanceFlag OrElse .OvertimeFlag <> _old.OvertimeFlag OrElse .NightDifferentialFlag <> _old.NightDifferentialFlag OrElse .HolidayFlag <> _old.HolidayFlag OrElse .RestDayFlag <> _old.RestDayFlag Then
         '                                .LogActionId = LogActionId.Edit
         '                            End If
         '                        End With



         '                        Exit For
         '                    End If
         '                Next

         '                If _new.LogActionId = LogActionId.Add Then
         '                    'File.WriteAllText("e:\JENON.txt", _new.PayTrxCode)
         '                    _addClientBillingCount = _addClientBillingCount + 1

         '                End If

         '                If _new.LogActionId = LogActionId.Edit Then
         '                    _editClientBillingCount = _editClientBillingCount + 1
         '                End If

         '            Next

         '            Dim _arsClientBillingListNew As New ArsClientBillingList      ' for adding new Template Details

         '            If _addClientBillingCount > 0 Then
         '                Dim _arsClientBilling As ArsClientBilling

         '                For Each _new As ArsClientBilling In _arsClientBillingList
         '                    If _new.LogActionId = LogActionId.Add Then
         '                        _arsClientBilling = New ArsClientBilling
         '                        _arsClientBillingListNew.Add(_arsClientBilling)
         '                        DataLib.ScatterValues(_new, _arsClientBilling)
         '                        _arsClientBilling.ClientPayGroupId = _arsClientBilling.ClientPayGroupId
         '                    End If
         '                Next

         '            End If

         '#End Region

#Region "ArsClientOvertime"

         Dim _removeClientOvertimeCount As Integer
            Dim _addClientOvertimeCount As Integer
            Dim _editClientOvertimeCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientOvertime In _arsClientOvertimeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientOvertime In _arsClientOvertimeList
                    If _new.ClientOvertimeDetailId = _old.ClientOvertimeDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeClientOvertimeCount = _removeClientOvertimeCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientOvertime In _arsClientOvertimeList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientOvertime In _arsClientOvertimeListOld
                    If _new.ClientOvertimeDetailId = _old.ClientOvertimeDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                            If .PayTrxCode <> _old.PayTrxCode Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With



                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    'File.WriteAllText("e:\JENON.txt", _new.PayTrxCode)
                    _addClientOvertimeCount = _addClientOvertimeCount + 1

                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editClientOvertimeCount = _editClientOvertimeCount + 1
                End If

            Next

            Dim _arsClientOvertimeListNew As New ArsClientOvertimeList      ' for adding new Template Details

            If _addClientOvertimeCount > 0 Then
                Dim _arsClientOvertime As ArsClientOvertime

                For Each _new As ArsClientOvertime In _arsClientOvertimeList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientOvertime = New ArsClientOvertime
                        _arsClientOvertimeListNew.Add(_arsClientOvertime)
                        DataLib.ScatterValues(_new, _arsClientOvertime)
                        _arsClientOvertime.ClientPayGroupId = _arsClientOvertime.ClientPayGroupId
                    End If
                Next

            End If

#End Region


#Region "ArsClientDayType"

            Dim _removeClientDayTypeCount As Integer
            Dim _addClientDayTypeCount As Integer
            Dim _editClientDayTypeCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientDayType In _arsClientDayTypeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientDayType In _arsClientDayTypeList
                    If _new.ClientDayTypeDetailId = _old.ClientDayTypeDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeClientDayTypeCount = _removeClientDayTypeCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientDayType In _arsClientDayTypeList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientDayType In _arsClientDayTypeListOld
                    If _new.ClientDayTypeDetailId = _old.ClientDayTypeDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                     If .DayTypeId <> _old.DayTypeId OrElse .PremiumPercentage <> _old.PremiumPercentage OrElse .AdminFee <> _old.AdminFee Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    'File.WriteAllText("e:\JENON.txt", _new.PayTrxCode)
                    _addClientDayTypeCount = _addClientDayTypeCount + 1

                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editClientDayTypeCount = _editClientDayTypeCount + 1
                End If

            Next

            Dim _arsClientDayTypeListNew As New ArsClientDayTypeList      ' for adding new Template Details

            If _addClientDayTypeCount > 0 Then
                Dim _arsClientDayType As ArsClientDayType

                For Each _new As ArsClientDayType In _arsClientDayTypeList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientDayType = New ArsClientDayType
                        _arsClientDayTypeListNew.Add(_arsClientDayType)
                        DataLib.ScatterValues(_new, _arsClientDayType)
                        _arsClientDayType.ClientPayGroupId = _arsClientDayType.ClientPayGroupId
                    End If
                Next

            End If

#End Region

#Region "ArsClientPayOutDayType"

         Dim _removeClientPayOutDayTypeCount As Integer
         Dim _addClientPayOutDayTypeCount As Integer
         Dim _editClientPayOutDayTypeCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientPayOutDayType In _arsClientPayOutDayTypeListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientPayOutDayType In _arsClientPayOutDayTypeList
               If _new.ClientPayOutDayTypeDetailId = _old.ClientPayOutDayTypeDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeClientPayOutDayTypeCount = _removeClientPayOutDayTypeCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsClientPayOutDayType In _arsClientPayOutDayTypeList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientPayOutDayType In _arsClientPayOutDayTypeListOld
               If _new.ClientPayOutDayTypeDetailId = _old.ClientPayOutDayTypeDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .DayTypeId <> _old.DayTypeId OrElse .PremiumPercentage <> _old.PremiumPercentage Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               'File.WriteAllText("e:\JENON.txt", _new.PayTrxCode)
               _addClientPayOutDayTypeCount = _addClientPayOutDayTypeCount + 1

            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editClientPayOutDayTypeCount = _editClientPayOutDayTypeCount + 1
            End If

         Next

         Dim _arsClientPayOutDayTypeListNew As New ArsClientPayOutDayTypeList      ' for adding new Template Details

         If _addClientPayOutDayTypeCount > 0 Then
            Dim _arsClientPayOutDayType As ArsClientPayOutDayType

            For Each _new As ArsClientPayOutDayType In _arsClientPayOutDayTypeList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientPayOutDayType = New ArsClientPayOutDayType
                  _arsClientPayOutDayTypeListNew.Add(_arsClientPayOutDayType)
                  DataLib.ScatterValues(_new, _arsClientPayOutDayType)
                  _arsClientPayOutDayType.ClientPayGroupId = _arsClientPayOutDayType.ClientPayGroupId
               End If
            Next

         End If

#End Region


#Region "ArsClientPayOutHoliday"

         Dim _removeClientPayOutHolidayCount As Integer
         Dim _addClientPayOutHolidayCount As Integer
         Dim _editClientPayOutHolidayCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientPayOutHoliday In _arsClientPayOutHolidayListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientPayOutHoliday In _arsClientPayOutHolidayList
               If _new.ClientPayOutHolidayDetailId = _old.ClientPayOutHolidayDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeClientPayOutHolidayCount = _removeClientPayOutHolidayCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch
         For Each _new As ArsClientPayOutHoliday In _arsClientPayOutHolidayList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientPayOutHoliday In _arsClientPayOutHolidayListOld
               If _new.ClientPayOutHolidayDetailId = _old.ClientPayOutHolidayDetailId And _old.ClientPayOutHolidayDetailId <> 0 Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If (.HolidayId <> _old.HolidayId OrElse .DayTypeId <> _old.DayTypeId OrElse .HolidayDate <> _old.HolidayDate) Then

                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addClientPayOutHolidayCount = _addClientPayOutHolidayCount + 1

            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editClientPayOutHolidayCount = _editClientPayOutHolidayCount + 1

            End If

         Next

         Dim _arsClientPayOutHolidayListNew As New ArsClientPayOutHolidayList      ' for adding new Template Details

         If _addClientPayOutHolidayCount > 0 Then
            Dim _arsClientPayOutHoliday As ArsClientPayOutHoliday

            For Each _new As ArsClientPayOutHoliday In _arsClientPayOutHolidayList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientPayOutHoliday = New ArsClientPayOutHoliday
                  _arsClientPayOutHolidayListNew.Add(_arsClientPayOutHoliday)
                  DataLib.ScatterValues(_new, _arsClientPayOutHoliday)
                  _arsClientPayOutHoliday.ClientPayGroupId = _arsClientPayOutHoliday.ClientPayGroupId
               End If
            Next

         End If

#End Region


         Dim _isArsClientPayGroupChanged As Boolean = Me.HasArsClientPayGroupChanges(_arsclientPayGroupOld, _arsClientPayGroup)
         'AndAlso _addClientBillingCount = 0 AndAlso _removeClientBillingCount = 0 AndAlso _editClientBillingCount = 0 
         If Not _isArsClientPayGroupChanged AndAlso _addClientPayOutCount = 0 AndAlso _removeClientPayOutCount = 0 AndAlso _editClientPayOutCount = 0 AndAlso _addClientOvertimeCount = 0 AndAlso _removeClientOvertimeCount = 0 AndAlso _editClientOvertimeCount = 0 AndAlso _addClientDayTypeCount = 0 AndAlso _removeClientDayTypeCount = 0 AndAlso _editClientDayTypeCount = 0 AndAlso _addClientPayOutDayTypeCount = 0 AndAlso _removeClientPayOutDayTypeCount = 0 AndAlso _editClientPayOutDayTypeCount = 0 AndAlso _addClientPayOutHolidayCount = 0 AndAlso _removeClientPayOutHolidayCount = 0 AndAlso _editClientPayOutHolidayCount = 0 Then

            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetClientPayGroup(clientPayGroupId)
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

                If _isArsClientPayGroupChanged Then
                    Me.UpdateArsClientPayGroup(_arsClientPayGroup)
                End If

                If _removeClientPayOutCount > 0 Then
                    Me.DeleteArsClientPayOuts(_arsClientPayOutListOld)
                End If

                If _addClientPayOutCount > 0 Then
                    Me.InsertArsClientPayOuts(_arsClientPayOutListNew)
                End If

                If _editClientPayOutCount > 0 Then
                    Me.UpdateArsClientPayOuts(_arsClientPayOutList)
                End If

            'If _removeClientBillingCount > 0 Then
            '    Me.DeleteArsClientBillings(_arsClientBillingListOld)
            'End If

            'If _addClientBillingCount > 0 Then
            '    Me.InsertArsClientBillings(_arsClientBillingListNew)
            'End If

            'If _editClientBillingCount > 0 Then
            '    Me.UpdateArsClientBillings(_arsClientBillingList)
            'End If

            If _removeClientOvertimeCount > 0 Then
                    Me.DeleteArsClientOvertimes(_arsClientOvertimeListOld)
                End If

                If _addClientOvertimeCount > 0 Then
                    Me.InsertArsClientOvertimes(_arsClientOvertimeListNew)
                End If

                If _editClientOvertimeCount > 0 Then
                    Me.UpdateArsClientOvertimes(_arsClientOvertimeList)
                End If

                If _removeClientDayTypeCount > 0 Then
                    Me.DeleteArsClientDayTypes(_arsClientDayTypeListOld)
                End If

                If _addClientDayTypeCount > 0 Then
                    Me.InsertArsClientDayTypes(_arsClientDayTypeListNew)
                End If

                If _editClientDayTypeCount > 0 Then
                    Me.UpdateArsClientDayTypes(_arsClientDayTypeList)
                End If

            If _removeClientPayOutDayTypeCount > 0 Then
               Me.DeleteArsClientPayOutDayTypes(_arsClientPayOutDayTypeListOld)
            End If

            If _addClientPayOutDayTypeCount > 0 Then
               Me.InsertArsClientPayOutDayTypes(_arsClientPayOutDayTypeListNew)
            End If

            If _editClientPayOutDayTypeCount > 0 Then
               Me.UpdateArsClientPayOutDayTypes(_arsClientPayOutDayTypeList)
            End If

            If _removeClientPayOutHolidayCount > 0 Then
               Me.DeleteArsClientPayOutHolidays(_arsClientPayOutHolidayListOld)
            End If

            If _addClientPayOutHolidayCount > 0 Then
               Me.InsertArsClientPayOutHolidays(_arsClientPayOutHolidayListNew)
            End If

            If _editClientPayOutHolidayCount > 0 Then
               Me.UpdateArsClientPayOutHolidays(_arsClientPayOutHolidayList)
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

    <SymAuthorization("RemoveArsClientPayGroup")>
    <Route("client-pay-clients/{clientPayGroupId}/{lockId}")>
    <HttpDelete>
    Public Function RemoveArsclientPayGroup(clientPayGroupId As Integer, lockId As String) As IHttpActionResult

        If clientPayGroupId <= 0 Then
            Throw New ArgumentException("Client Pay Group ID is required.")
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

                Me.DeleteArsClientPayGroup(clientPayGroupId, lockId)

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
    Private Sub LoadArsClientPayGroup(clientPayGroup As ArsClientPayGroupBody, arsClientPayGroup As ArsClientPayGroup)

        DataLib.ScatterValues(clientPayGroup, arsClientPayGroup)

    End Sub

    Private Sub LoadArsClientPayGroup(row As DataRow, clientPayGroup As ArsClientPayGroup)

      With clientPayGroup
         .ClientPayGroupId = row.ToInt32("ClientPayGroupId")
         .ClientContractId = row.ToInt32("ClientContractId")
         .ClientPayGroupName = row.ToString("ClientPayGroupName")
         .PayOutComputationId = row.ToInt32("PayOutComputationId")
         .BillingComputationId = row.ToInt32("BillingComputationId")
         .AdminFee = row.ToDecimal("AdminFee")
         .PayFreqId = row.ToInt32("PayFreqId")
         .StartCutOffDate = row.ToDate("StartCutOffDate")
         .EndCutOffDate = row.ToDate("EndCutOffDate")
         .PayOutDate = row.ToDate("PayOutDate")
         .BillingDate = row.ToDate("BillingDate")
         .PayOutRemitFlag = row.ToBoolean("PayOutRemitFlag")
         .ClientPayGroupStatusId = row.ToInt32("ClientPayGroupStatusId")
         .RegionId = row.ToString("RegionId")
         .ProvinceId = row.ToString("ProvinceId")
         .MunicipalityId = row.ToString("MunicipalityId")
         .RateAmount = row.ToDecimal("RateAmount")
         .DeminimisAmount = row.ToDecimal("DeminimisAmount")
         .AllowanceAmount = row.ToDecimal("AllowanceAmount")
         .MinimumWageFlag = row.ToBoolean("MinimumWageFlag")
         .Name2 = row.ToString("Name2")
         .Name3 = row.ToString("Name3")
         .Name4 = row.ToString("Name4")
         .Name5 = row.ToString("Name5")
         .Name6 = row.ToString("Name6")
         .Name7 = row.ToString("Name7")
         .TimekeepingId = row.ToInt32("TimekeepingId")
      End With

   End Sub
    Private Sub LoadArsClientPayOutList(rows As DataRowCollection, list As ArsClientPayOutList)

        Dim _detail As ArsClientPayOut
        For Each _row As DataRow In rows
            _detail = New ArsClientPayOut

            With _detail
                .ClientPayOutDetailId = _row.ToInt32("ClientPayOutDetailId")
                .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
                .PayTrxCode = _row.ToString("PayTrxCode")
                .PayOutSheetFormula = _row.ToString("PayOutSheetFormula")
                .FixedAmount = _row.ToDecimal("FixedAmount")
                .BasicFlag = _row.ToBoolean("BasicFlag")
                .DeminimisFlag = _row.ToBoolean("DeminimisFlag")
                .AllowanceFlag = _row.ToBoolean("AllowanceFlag")
                .OvertimeFlag = _row.ToBoolean("OvertimeFlag")
                .NightDifferentialFlag = _row.ToBoolean("NightDifferentialFlag")
                .HolidayFlag = _row.ToBoolean("HolidayFlag")
                .RestDayFlag = _row.ToBoolean("RestDayFlag")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadArsClientBillingList(rows As DataRowCollection, list As ArsClientBillingList)

        Dim _detail As ArsClientBilling
        For Each _row As DataRow In rows
            _detail = New ArsClientBilling

            With _detail
                .ClientBillingDetailId = _row.ToInt32("ClientBillingDetailId")
                .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
                .ChargingConsiderationId = _row.ToInt32("ChargingConsiderationId")

                .PayTrxCode = _row.ToString("PayTrxCode")
                .BillingSheetFormula = _row.ToString("BillingSheetFormula")
                .FixedAmount = _row.ToDecimal("FixedAmount")
                .BasicFlag = _row.ToBoolean("BasicFlag")
                .DeminimisFlag = _row.ToBoolean("DeminimisFlag")
                .AllowanceFlag = _row.ToBoolean("AllowanceFlag")
                .OvertimeFlag = _row.ToBoolean("OvertimeFlag")
                .NightDifferentialFlag = _row.ToBoolean("NightDifferentialFlag")
                .HolidayFlag = _row.ToBoolean("HolidayFlag")
                .RestDayFlag = _row.ToBoolean("RestDayFlag")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadArsClientOvertimeList(rows As DataRowCollection, list As ArsClientOvertimeList)

        Dim _detail As ArsClientOvertime
        For Each _row As DataRow In rows
            _detail = New ArsClientOvertime

            With _detail
                .ClientOvertimeDetailId = _row.ToInt32("ClientOvertimeDetailId")
                .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
                .PayTrxCode = _row.ToString("PayTrxCode")
            End With

            list.Add(_detail)

        Next

    End Sub
   Private Sub LoadArsClientDayTypeList(rows As DataRowCollection, list As ArsClientDayTypeList)

      Dim _detail As ArsClientDayType
      For Each _row As DataRow In rows
         _detail = New ArsClientDayType

         With _detail
            .ClientDayTypeDetailId = _row.ToInt32("ClientDayTypeDetailId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .DayTypeId = _row.ToInt32("DayTypeId")
            .PremiumPercentage = _row.ToDecimal("PremiumPercentage")
            .AdminFee = _row.ToDecimal("AdminFee")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsClientPayOutDayTypeList(rows As DataRowCollection, list As ArsClientPayOutDayTypeList)

      Dim _detail As ArsClientPayOutDayType
      For Each _row As DataRow In rows
         _detail = New ArsClientPayOutDayType

         With _detail
            .ClientPayOutDayTypeDetailId = _row.ToInt32("ClientPayOutDayTypeDetailId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .DayTypeId = _row.ToInt32("DayTypeId")
            .PremiumPercentage = _row.ToDecimal("PremiumPercentage")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsClientPayOutHolidayList(rows As DataRowCollection, list As ArsClientPayOutHolidayList)

      Dim _detail As ArsClientPayOutHoliday
      For Each _row As DataRow In rows
         _detail = New ArsClientPayOutHoliday

         With _detail
            .ClientPayOutHolidayDetailId = _row.ToInt32("ClientPayOutHolidayDetailId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .DayTypeId = _row.ToInt32("DayTypeId")
            .HolidayId = _row.ToInt32("HolidayId")
            .HolidayDate = _row.ToDate("HolidayDate")

         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertArsClientPayGroup(clientPayGroup As ArsClientPayGroup)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayGroup", clientPayGroup, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsClientPayGroup(clientPayGroup)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertArsClientPayOuts(list As ArsClientPayOutList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ClientPayOutDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOut", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOut In list
                Me.AddInsertUpdateParamsArsClientPayOut(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertArsClientBillings(list As ArsClientBillingList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ClientBillingDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBilling", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientBilling In list
                Me.AddInsertUpdateParamsArsClientBilling(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertArsClientOvertimes(list As ArsClientOvertimeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ClientOvertimeDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientOvertime", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientOvertime In list
                Me.AddInsertUpdateParamsArsClientOvertime(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
   Private Sub InsertArsClientDayTypes(list As ArsClientDayTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ClientDayTypeDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientDayType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientDayType In list
            Me.AddInsertUpdateParamsArsClientDayType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsClientPayOutDayTypes(list As ArsClientPayOutDayTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ClientPayOutDetailId")     ' auto-assigned by DB
         .Add("ClientPayOutDayTypeDetailId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutDayType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientPayOutDayType In list
            Me.AddInsertUpdateParamsArsClientPayOutDayType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsClientPayOutHolidays(list As ArsClientPayOutHolidayList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ClientPayOutHolidayDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutHoliday", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientPayOutHoliday In list
            Me.AddInsertUpdateParamsArsClientPayOutHoliday(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub


   Private Sub UpdateArsClientPayGroup(ClientPayGroup As ArsClientPayGroup)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ClientPayGroupId")
            .Add("LockId")
        End With

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayGroup", ClientPayGroup, _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClientPayGroup(ClientPayGroup)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(ClientPayGroup.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateArsClientPayOuts(list As ArsClientPayOutList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ClientPayOutDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOut", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOut In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientPayOut(_detail)
                    .Parameters.AddWithValue("@ClientPayOutDetailId", _detail.ClientPayOutDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateArsClientBillings(list As ArsClientBillingList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ClientBillingDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBilling", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientBilling In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientBilling(_detail)
                    .Parameters.AddWithValue("@ClientBillingDetailId", _detail.ClientBillingDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
   Private Sub UpdateArsClientOvertimes(list As ArsClientOvertimeList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClientOvertimeDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientOvertime", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientOvertime In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientOvertime(_detail)
               .Parameters.AddWithValue("@ClientOvertimeDetailId", _detail.ClientOvertimeDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsClientDayTypes(list As ArsClientDayTypeList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ClientDayTypeDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientDayType", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientDayType In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientDayType(_detail)
                    .Parameters.AddWithValue("@ClientDayTypeDetailId", _detail.ClientDayTypeDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
   Private Sub UpdateArsClientPayOutDayTypes(list As ArsClientPayOutDayTypeList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClientPayOutDayTypeDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOutDayType", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientPayOutDayType In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientPayOutDayType(_detail)
               .Parameters.AddWithValue("@ClientPayOutDayTypeDetailId", _detail.ClientPayOutDayTypeDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsClientPayOutHolidays(list As ArsClientPayOutHolidayList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClientPayOutHolidayDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOutHoliday", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientPayOutHoliday In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientPayOutHoliday(_detail)
               .Parameters.AddWithValue("@ClientPayOutHolidayDetailId", _detail.ClientPayOutHolidayDetailId)

               .ExecuteNonQuery()
            End If
         Next


        End With

   End Sub

   Private Sub DeleteArsClientPayGroup(clientPayGroupId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientPayGroupId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsClientPayGroup", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ClientPayGroupId", clientPayGroupId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub DeleteArsClientPayOuts(list As ArsClientPayOutList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientPayOut WHERE ClientPayOutDetailId=@ClientPayOutDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientPayOut In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ClientPayOutDetailId", _old.ClientPayOutDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteArsClientBillings(list As ArsClientBillingList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientBilling WHERE ClientBillingDetailId=@ClientBillingDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientBilling In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ClientBillingDetailId", _old.ClientBillingDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteArsClientOvertimes(list As ArsClientOvertimeList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientOvertime WHERE ClientOvertimeDetailId=@ClientOvertimeDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientOvertime In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ClientOvertimeDetailId", _old.ClientOvertimeDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
   Private Sub DeleteArsClientDayTypes(list As ArsClientDayTypeList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientDayType WHERE ClientDayTypeDetailId=@ClientDayTypeDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientDayType In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ClientDayTypeDetailId", _old.ClientDayTypeDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsClientPayOutDayTypes(list As ArsClientPayOutDayTypeList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientPayOutDayType WHERE ClientPayOutDayTypeDetailId=@ClientPayOutDayTypeDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientPayOutDayType In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ClientPayOutDayTypeDetailId", _old.ClientPayOutDayTypeDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsClientPayOutHolidays(list As ArsClientPayOutHolidayList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientPayOutHoliday WHERE ClientPayOutHolidayDetailId=@ClientPayOutHolidayDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientPayOutHoliday In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ClientPayOutHolidayDetailId", _old.ClientPayOutHolidayDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub


   Private Sub AddInsertUpdateParamsArsClientPayGroup(ClientPayGroup As ArsClientPayGroup)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayGroupId", ClientPayGroup.ClientPayGroupId)
            .AddWithValue("@ClientContractId", ClientPayGroup.ClientContractId)
            .AddWithValue("@ClientPayGroupName", ClientPayGroup.ClientPayGroupName)
            .AddWithValue("@PayOutComputationId", ClientPayGroup.PayOutComputationId)
            .AddWithValue("@BillingComputationId", ClientPayGroup.BillingComputationId)

            .AddWithValue("@AdminFee", ClientPayGroup.AdminFee)
            .AddWithValue("@PayFreqId", ClientPayGroup.PayFreqId)
            .AddWithValue("@StartCutOffDate", ClientPayGroup.StartCutOffDate)
            .AddWithValue("@EndCutOffDate", ClientPayGroup.EndCutOffDate)

            .AddWithValue("@PayOutDate", ClientPayGroup.PayOutDate)
            .AddWithValue("@BillingDate", ClientPayGroup.BillingDate)
            .AddWithValue("@PayOutRemitFlag", ClientPayGroup.PayOutRemitFlag)
            .AddWithValue("@ClientPayGroupStatusId", ClientPayGroup.ClientPayGroupStatusId)


            .AddWithValue("@RegionId", ClientPayGroup.RegionId)
         .AddWithValue("@ProvinceId", ClientPayGroup.ProvinceId)
         .AddWithValue("@MunicipalityId", ClientPayGroup.MunicipalityId)

         .AddWithValue("@RateAmount", ClientPayGroup.RateAmount)
         .AddWithValue("@DeminimisAmount", ClientPayGroup.DeminimisAmount)
            .AddWithValue("@AllowanceAmount", ClientPayGroup.AllowanceAmount)
         .AddWithValue("@MinimumWageFlag", ClientPayGroup.MinimumWageFlag)
         .AddWithValue("@Name2", ClientPayGroup.Name2)
         .AddWithValue("@Name3", ClientPayGroup.Name3)
         .AddWithValue("@Name4", ClientPayGroup.Name4)
         .AddWithValue("@Name5", ClientPayGroup.Name5)
         .AddWithValue("@Name6", ClientPayGroup.Name6)
         .AddWithValue("@Name7", ClientPayGroup.Name7)
         .AddWithValue("@TimekeepingId", ClientPayGroup.TimekeepingId)

      End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientPayOut(dtl As ArsClientPayOut)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
            .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
            .AddWithValue("@PayOutSheetFormula", dtl.PayOutSheetFormula)
            .AddWithValue("@FixedAmount", dtl.FixedAmount.ToNullable)
            .AddWithValue("@BasicFlag", dtl.BasicFlag)
            .AddWithValue("@DeminimisFlag", dtl.DeminimisFlag)
            .AddWithValue("@AllowanceFlag", dtl.AllowanceFlag)
            .AddWithValue("@OvertimeFlag", dtl.OvertimeFlag)
            .AddWithValue("@NightDifferentialFlag", dtl.NightDifferentialFlag)
            .AddWithValue("@HolidayFlag", dtl.HolidayFlag)
            .AddWithValue("@RestDayFlag", dtl.RestDayFlag)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsArsClientBilling(dtl As ArsClientBilling)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
            .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
            .AddWithValue("@ChargingConsiderationId", dtl.ChargingConsiderationId)
            .AddWithValue("@BillingSheetFormula", dtl.BillingSheetFormula)
            .AddWithValue("@FixedAmount", dtl.FixedAmount.ToNullable)
            .AddWithValue("@BasicFlag", dtl.BasicFlag)
            .AddWithValue("@DeminimisFlag", dtl.DeminimisFlag)
            .AddWithValue("@AllowanceFlag", dtl.AllowanceFlag)
            .AddWithValue("@OvertimeFlag", dtl.OvertimeFlag)
            .AddWithValue("@NightDifferentialFlag", dtl.NightDifferentialFlag)
            .AddWithValue("@HolidayFlag", dtl.HolidayFlag)
            .AddWithValue("@RestDayFlag", dtl.RestDayFlag)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsArsClientOvertime(dtl As ArsClientOvertime)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
            .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
        End With

    End Sub
   Private Sub AddInsertUpdateParamsArsClientDayType(dtl As ArsClientDayType)

      With DataCore.Command.Parameters
         .Clear()
         '.AddWithValue("@ClientDayTypeDetailId", dtl.ClientDayTypeDetailId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@DayTypeId", dtl.DayTypeId)
         .AddWithValue("@PremiumPercentage", dtl.PremiumPercentage)
         .AddWithValue("@AdminFee", dtl.AdminFee)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsClientPayOutDayType(dtl As ArsClientPayOutDayType)

      With DataCore.Command.Parameters
         .Clear()
         '.AddWithValue("@ClientPayOutDayTypeDetailId", dtl.ClientPayOutDayTypeDetailId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@DayTypeId", dtl.DayTypeId)
         .AddWithValue("@PremiumPercentage", dtl.PremiumPercentage)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsClientPayOutHoliday(dtl As ArsClientPayOutHoliday)

      With DataCore.Command.Parameters
         .Clear()
         '.AddWithValue("@ClientPayOutDayTypeDetailId", dtl.ClientPayOutDayTypeDetailId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@DayTypeId", dtl.DayTypeId)
         .AddWithValue("@HolidayId", dtl.HolidayId)
         .AddWithValue("@HolidayDate", dtl.HolidayDate)

      End With

   End Sub


   'Private Sub AddInsertUpdateParamsArsClientBilling(dtl As ArsClientBilling)

   '    With DataCore.Command.Parameters
   '        .Clear()
   '        .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
   '        .AddWithValue("@ChargingConsiderationId", dtl.ChargingConsiderationId)
   '        .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
   '        .AddWithValue("@BillingSheetFormula", dtl.BillingSheetFormula)
   '        .AddWithValue("@FixedAmount", dtl.FixedAmount.ToNullable)
   '        .AddWithValue("@BasicFlag", dtl.BasicFlag)
   '        .AddWithValue("@DeminimisFlag", dtl.DeminimisFlag)
   '        .AddWithValue("@AllowanceFlag", dtl.AllowanceFlag)
   '        .AddWithValue("@OvertimeFlag", dtl.OvertimeFlag)
   '        .AddWithValue("@NightDifferentialFlag", dtl.NightDifferentialFlag)
   '        .AddWithValue("@HolidayFlag", dtl.HolidayFlag)
   '        .AddWithValue("@RestDayFlag", dtl.RestDayFlag)

   '    End With

   'End Sub

   Private Function HasArsClientPayGroupChanges(oldRecord As ArsClientPayGroup, newRecord As ArsClientPayGroup) As Boolean

      With oldRecord
         If .ClientPayGroupId <> newRecord.ClientPayGroupId Then Return True
         If .ClientContractId <> newRecord.ClientContractId Then Return True
         If .PayOutComputationId <> newRecord.PayOutComputationId Then Return True
         If .BillingComputationId <> newRecord.BillingComputationId Then Return True
         If .AdminFee <> newRecord.AdminFee Then Return True
         If .PayFreqId <> newRecord.PayFreqId Then Return True
         If .StartCutOffDate <> newRecord.StartCutOffDate Then Return True
         If .EndCutOffDate <> newRecord.EndCutOffDate Then Return True
         If .PayOutDate <> newRecord.PayOutDate Then Return True
         If .BillingDate <> newRecord.BillingDate Then Return True
         If .PayOutRemitFlag <> newRecord.PayOutRemitFlag Then Return True
         If .RegionId <> newRecord.RegionId Then Return True
         If .ProvinceId <> newRecord.ProvinceId Then Return True
         If .MunicipalityId <> newRecord.MunicipalityId Then Return True
         If .RateAmount <> newRecord.RateAmount Then Return True
         If .DeminimisAmount <> newRecord.DeminimisAmount Then Return True
         If .AllowanceAmount <> newRecord.AllowanceAmount Then Return True
         If .MinimumWageFlag <> newRecord.MinimumWageFlag Then Return True
         If .ClientPayGroupStatusId <> newRecord.ClientPayGroupStatusId Then Return True

      End With

      Return False

    End Function

End Class

Public Class ArsClientPayGroupBody
    Inherits ArsClientPayGroup
    Public Property PayOuts As ArsClientPayOut()
   'Public Property Billings As ArsClientBilling()
   Public Property Overtimes As ArsClientOvertime()
    Public Property DayTypes As ArsClientDayType()
   Public Property PayOutDayTypes As ArsClientPayOutDayType()
   Public Property Holidays As ArsClientPayOutHoliday()
End Class
