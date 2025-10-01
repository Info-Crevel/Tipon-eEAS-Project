<RoutePrefix("api")>
Public Class ARS0150_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetReferences_ARS0150")>
    <Route("references/ars0150")>
    <HttpGet>
    Public Function GetReferences_ARS0150() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.ARS0150_References")
                With _direct

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "payTrxs"
                            .Tables(1).TableName = "deminimis"
                            .Tables(2).TableName = "allowances"
                            .Tables(3).TableName = "basicRate"
                            .Tables(4).TableName = "dayTypeSheet"     ' all defined Transaction Types
                        End With

                        Return Me.Ok(_dataSet)

                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetClientPayGroupPayOut")>
    <Route("client-pay-group-payOuts/{clientPayOutDetailId}")>
    <HttpGet>
    Public Function GetClientPayGroupPayOut(clientPayOutDetailId As Integer) As IHttpActionResult

        If clientPayOutDetailId <= 0 Then
            Throw New ArgumentException("Client Billing Detail ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.ARS0150")
                With _direct
                    .AddParameter("ClientPayOutDetailId", clientPayOutDetailId)

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "payOut"
                     .Tables(1).TableName = "payOutDeminimis"
                     .Tables(2).TableName = "payOutAllowances"
                     .Tables(3).TableName = "payOutBasicRate"
                     .Tables(4).TableName = "dayTypes"
                        End With

                        Return Me.Ok(_dataSet)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateClientPayGroupPayOut")>
    <Route("client-pay-group-payOuts/{currentUserId}")>
    <HttpPost>
    Public Function CreateClientPayGroupPayOut(currentUserId As Integer, <FromBody> payOut As ArsClientPayOutBody) As IHttpActionResult

        If payOut.ClientPayOutDetailId <> -1 Then
            Throw New ArgumentException("Client Billing Detail ID not recognized.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)
            If currentUserId > 0 Then
                _currentUserId = currentUserId
            End If

            '
            ' Assign new ID from sequencer
            '
            Dim _clientPayOutDetailId As Integer = SysLib.GetNextSequence("ClientPayOutDetailId")

            payOut.ClientPayOutDetailId = _clientPayOutDetailId

         'billing.ClientPayGroupId = 42
         '
         ' Load proposed values from payload
         '
         Dim _arsClientPayOut As New ArsClientPayGroupPayOut
            Dim _arsClientPayOutDeminimisList As New ArsClientPayOutDeminimisList
            Dim _arsClientPayOutAllowanceList As New ArsClientPayOutAllowanceList
            Dim _arsClientPayOutBasicRateList As New ArsClientPayOutBasicRateList
            Dim _arsClientPayOutDayTypeList As New ArsClientPayOutDayTypeList

            Me.LoadArsClientPayOut(payOut, _arsClientPayOut)

            For Each _deminimis As ArsClientPayOutDeminimis In payOut.Deminimis
                _deminimis.ClientPayOutDetailId = _clientPayOutDetailId
                _arsClientPayOutDeminimisList.Add(_deminimis)
            Next

            For Each _allowances As ArsClientPayOutAllowance In payOut.Allowances
                _allowances.ClientPayOutDetailId = _clientPayOutDetailId
                _arsClientPayOutAllowanceList.Add(_allowances)
            Next

            For Each _basicRate As ArsClientPayOutBasicRate In payOut.BasicRate
                _basicRate.ClientPayOutDetailId = _clientPayOutDetailId
                _arsClientPayOutBasicRateList.Add(_basicRate)
            Next

            For Each _dayType As ArsClientPayOutDayType In payOut.DayTypes
                _dayType.ClientPayOutDetailId = _clientPayOutDetailId
                _arsClientPayOutDayTypeList.Add(_dayType)
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

                Me.InsertArsClientPayOut(_arsClientPayOut)

                If _arsClientPayOutDeminimisList.Count > 0 Then
                    Me.InsertArsClientPayOutDeminimis(_arsClientPayOutDeminimisList)
                End If

                If _arsClientPayOutAllowanceList.Count > 0 Then
                    Me.InsertArsClientPayOutAllowances(_arsClientPayOutAllowanceList)
                End If

                If _arsClientPayOutBasicRateList.Count > 0 Then
                    Me.InsertArsClientPayOutBasicRate(_arsClientPayOutBasicRateList)
                End If

                If _arsClientPayOutDayTypeList.Count > 0 Then
                    Me.InsertArsClientDayTypes(_arsClientPayOutDayTypeList)
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

            'Return Me.Ok(True)
            Return Me.Ok(payOut.ClientPayOutDetailId)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("ModifyClientPayOutDetail")>
    <Route("client-pay-group-payOuts/{clientPayOutDetailId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyCluster(clientPayOutDetailId As Integer, currentUserId As Integer, <FromBody> payOut As ArsClientPayOutBody) As IHttpActionResult

        If clientPayOutDetailId <= 0 Then
            Throw New ArgumentException("Client PayOut Detail ID is required.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)
            If currentUserId > 0 Then
                _currentUserId = currentUserId
            End If

            '
            ' Load proposed values from payload
            '
            Dim _arsClientPayOut As New ArsClientPayGroupPayOut
            Dim _arsClientPayOutDeminimisList As New ArsClientPayOutDeminimisList
            Dim _arsClientPayOutAllowanceList As New ArsClientPayOutAllowanceList
            Dim _arsClientPayOutBasicRateList As New ArsClientPayOutBasicRateList
            Dim _arsClientPayOutDayTypeList As New ArsClientPayOutDayTypeList

            Me.LoadArsClientPayOut(payOut, _arsClientPayOut)

            For Each _deminimis As ArsClientPayOutDeminimis In payOut.Deminimis
                _arsClientPayOutDeminimisList.Add(_deminimis)
            Next

            For Each _allowances As ArsClientPayOutAllowance In payOut.Allowances
                _arsClientPayOutAllowanceList.Add(_allowances)
            Next

            For Each _basicRate As ArsClientPayOutBasicRate In payOut.BasicRate
                _arsClientPayOutBasicRateList.Add(_basicRate)
            Next

            For Each _dayType As ArsClientPayOutDayType In payOut.DayTypes
                _dayType.ClientPayOutDetailId = clientPayOutDetailId
                _arsClientPayOutDayTypeList.Add(_dayType)
            Next


            '
            ' Load old values from DB
            '
            Dim _arsClientPayOutOld As New ArsClientPayGroupPayOut
            Dim _arsClientPayOutDeminimisListOld As New ArsClientPayOutDeminimisList
            Dim _arsClientPayOutAllowanceListOld As New ArsClientPayOutAllowanceList
            Dim _arsClientPayOutBasicRateListOld As New ArsClientPayOutBasicRateList
            Dim _arsClientPayOutDayTypeListOld As New ArsClientPayOutDayTypeList

            Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetClientPayGroupPayOut(clientPayOutDetailId), Results.OkNegotiatedContentResult(Of DataSet))

            Using _dataSet As DataSet = _result.Content
                Dim _row As DataRow = _dataSet.Tables("payOut").Rows(0)
                Me.LoadArsClientPayOut(_row, _arsClientPayOutOld)
                Me.LoadArsClientPayOutDeminimisList(_dataSet.Tables("PayOutDeminimis").Rows, _arsClientPayOutDeminimisListOld)
                Me.LoadArsClientPayOutAllowanceList(_dataSet.Tables("PayOutAllowances").Rows, _arsClientPayOutAllowanceListOld)
                Me.LoadArsClientPayOutBasicRateList(_dataSet.Tables("PayOutBasicRate").Rows, _arsClientPayOutBasicRateListOld)
                Me.LoadArsClientDayTypeList(_dataSet.Tables("dayTypes").Rows, _arsClientPayOutDayTypeListOld)
            End Using


            Dim _removeDeminimisCount As Integer
            Dim _addDeminimisCount As Integer
            Dim _editDeminimisCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientPayOutDeminimis In _arsClientPayOutDeminimisListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientPayOutDeminimis In _arsClientPayOutDeminimisList
                    If _new.PayOutDeminimisId = _old.PayOutDeminimisId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeDeminimisCount = _removeDeminimisCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientPayOutDeminimis In _arsClientPayOutDeminimisList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientPayOutDeminimis In _arsClientPayOutDeminimisListOld
                    If _new.PayOutDeminimisId = _old.PayOutDeminimisId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                            'If .ed <> _old.AccountId Then
                            '    .LogActionId = LogActionId.Edit
                            'End If

                            If .DeminimisId <> _old.DeminimisId Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With



                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then

                    _addDeminimisCount = _addDeminimisCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editDeminimisCount = _editDeminimisCount + 1
                End If

            Next

            Dim _arsClientPayOutDeminimisListNew As New ArsClientPayOutDeminimisList      ' for adding new Template Deminimiss

            If _addDeminimisCount > 0 Then
                Dim _arsClientPayOutDeminimis As ArsClientPayOutDeminimis

                For Each _new As ArsClientPayOutDeminimis In _arsClientPayOutDeminimisList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientPayOutDeminimis = New ArsClientPayOutDeminimis
                        _arsClientPayOutDeminimisListNew.Add(_arsClientPayOutDeminimis)
                        DataLib.ScatterValues(_new, _arsClientPayOutDeminimis)
                        _arsClientPayOutDeminimis.ClientPayOutDetailId = _arsClientPayOut.ClientPayOutDetailId
                    End If
                Next

            End If

            'Allowance

            Dim _removeAllowanceCount As Integer
            Dim _addAllowanceCount As Integer
            Dim _editAllowanceCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientPayOutAllowance In _arsClientPayOutAllowanceListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientPayOutAllowance In _arsClientPayOutAllowanceList
                    If _new.PayOutAllowanceId = _old.PayOutAllowanceId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeAllowanceCount = _removeAllowanceCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientPayOutAllowance In _arsClientPayOutAllowanceList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientPayOutAllowance In _arsClientPayOutAllowanceListOld
                    If _new.PayOutAllowanceId = _old.PayOutAllowanceId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                            'If .ed <> _old.AccountId Then
                            '    .LogActionId = LogActionId.Edit
                            'End If

                            If .AllowanceId <> _old.AllowanceId Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With



                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then

                    _addAllowanceCount = _addAllowanceCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editAllowanceCount = _editAllowanceCount + 1
                End If

            Next

            Dim _arsClientPayOutAllowanceListNew As New ArsClientPayOutAllowanceList      ' for adding new Template Allowances

            If _addAllowanceCount > 0 Then
                Dim _arsClientPayOutAllowance As ArsClientPayOutAllowance

                For Each _new As ArsClientPayOutAllowance In _arsClientPayOutAllowanceList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientPayOutAllowance = New ArsClientPayOutAllowance
                        _arsClientPayOutAllowanceListNew.Add(_arsClientPayOutAllowance)
                        DataLib.ScatterValues(_new, _arsClientPayOutAllowance)
                        _arsClientPayOutAllowance.ClientPayOutDetailId = _arsClientPayOut.ClientPayOutDetailId
                    End If
                Next

            End If

            'Basic Rate

            Dim _removeBasicRateCount As Integer
            Dim _addBasicRateCount As Integer
            Dim _editBasicRateCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientPayOutBasicRate In _arsClientPayOutBasicRateListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientPayOutBasicRate In _arsClientPayOutBasicRateList
                    If _new.PayOutBasicRateId = _old.PayOutBasicRateId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeBasicRateCount = _removeBasicRateCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientPayOutBasicRate In _arsClientPayOutBasicRateList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientPayOutBasicRate In _arsClientPayOutBasicRateListOld
                    If _new.PayOutBasicRateId = _old.PayOutBasicRateId Then
                        _new.LogActionId = 0   ' don't add

                        With _new
                            'If .ed <> _old.AccountId Then
                            '    .LogActionId = LogActionId.Edit
                            'End If

                            If .PayTrxCode <> _old.PayTrxCode Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With



                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then

                    _addBasicRateCount = _addBasicRateCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editBasicRateCount = _editBasicRateCount + 1
                End If

            Next

            Dim _arsClientPayOutBasicRateListNew As New ArsClientPayOutBasicRateList      ' for adding new Template BasicRates

            If _addBasicRateCount > 0 Then
                Dim _arsClientPayOutBasicRate As ArsClientPayOutBasicRate

                For Each _new As ArsClientPayOutBasicRate In _arsClientPayOutBasicRateList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientPayOutBasicRate = New ArsClientPayOutBasicRate
                        _arsClientPayOutBasicRateListNew.Add(_arsClientPayOutBasicRate)
                        DataLib.ScatterValues(_new, _arsClientPayOutBasicRate)
                        _arsClientPayOutBasicRate.ClientPayOutDetailId = _arsClientPayOut.ClientPayOutDetailId
                    End If
                Next

            End If


#Region "ArsClientPayOutDayType"

            Dim _removeClientDayTypeCount As Integer
            Dim _addClientDayTypeCount As Integer
            Dim _editClientDayTypeCount As Integer
            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientPayOutDayType In _arsClientPayOutDayTypeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientPayOutDayType In _arsClientPayOutDayTypeList
               If _new.ClientPayOutDayTypeDetailId = _old.ClientPayOutDayTypeDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeClientDayTypeCount = _removeClientDayTypeCount + 1
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
                     If .DayTypeId <> _old.DayTypeId Then
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

            Dim _arsClientpayOutDayTypeListNew As New ArsClientPayOutDayTypeList      ' for adding new Template Details

            If _addClientDayTypeCount > 0 Then
                Dim _arsClientPayOutDayType As ArsClientPayOutDayType

                For Each _new As ArsClientPayOutDayType In _arsClientPayOutDayTypeList
                    If _new.LogActionId = LogActionId.Add Then
                        _arsClientPayOutDayType = New ArsClientPayOutDayType
                        _arsClientpayOutDayTypeListNew.Add(_arsClientPayOutDayType)
                        DataLib.ScatterValues(_new, _arsClientPayOutDayType)
                        _arsClientPayOutDayType.ClientPayOutDetailId = _arsClientPayOutDayType.ClientPayOutDetailId
                    End If
                Next

            End If

#End Region


            Dim _isArsClientPayOutChanged As Boolean = Me.HasArsClientPayOutChanges(_arsClientPayOutOld, _arsClientPayOut)

            If Not _isArsClientPayOutChanged AndAlso _addDeminimisCount = 0 AndAlso _removeDeminimisCount = 0 AndAlso _editDeminimisCount = 0 AndAlso _addAllowanceCount = 0 AndAlso _removeAllowanceCount = 0 AndAlso _editAllowanceCount = 0 AndAlso _addBasicRateCount = 0 AndAlso _removeBasicRateCount = 0 AndAlso _editBasicRateCount = 0 AndAlso _addClientDayTypeCount = 0 AndAlso _removeClientDayTypeCount = 0 AndAlso _editClientDayTypeCount = 0 Then
                '
                ' No changes; just return current transaction
                ' 
                Return Me.GetClientPayGroupPayOut(clientPayOutDetailId)
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

                If _isArsClientPayOutChanged Then
                    Me.UpdateArsClientPayOut(_arsClientPayOut)
                End If

                If _removeDeminimisCount > 0 Then
                    Me.DeleteArsClientPayOutDeminimis(_arsClientPayOutDeminimisListOld)

                End If

                If _addDeminimisCount > 0 Then
                    Me.InsertArsClientPayOutDeminimis(_arsClientPayOutDeminimisListNew)

                End If

                If _editDeminimisCount > 0 Then
                    Me.UpdateArsClientPayOutDeminimis(_arsClientPayOutDeminimisList)

                End If

                If _removeAllowanceCount > 0 Then
                    Me.DeleteArsClientPayOutAllowance(_arsClientPayOutAllowanceListOld)

                End If

                If _addAllowanceCount > 0 Then
                    Me.InsertArsClientPayOutAllowances(_arsClientPayOutAllowanceListNew)

                End If

                If _editAllowanceCount > 0 Then
                    Me.UpdateArsClientPayOutAllowance(_arsClientPayOutAllowanceList)

                End If

                If _removeBasicRateCount > 0 Then
                    Me.DeleteArsClientPayOutBasicRate(_arsClientPayOutBasicRateListOld)

                End If

                If _addBasicRateCount > 0 Then
                    Me.InsertArsClientPayOutBasicRate(_arsClientPayOutBasicRateListNew)

                End If

                If _editBasicRateCount > 0 Then
                    Me.UpdateArsClientPayOutBasicRate(_arsClientPayOutBasicRateList)

                End If

                If _removeClientDayTypeCount > 0 Then
                    Me.DeleteArsClientDayTypes(_arsClientPayOutDayTypeListOld)
                End If

                If _addClientDayTypeCount > 0 Then
                    Me.InsertArsClientDayTypes(_arsClientPayOutDayTypeListNew)
                End If

                If _editClientDayTypeCount > 0 Then
                    Me.UpdateArsClientDayTypes(_arsClientPayOutDayTypeList)
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

    <SymAuthorization("RemoveClientPayOutDetail")>
    <Route("client-pay-group-payOuts/{clientPayOutDetailId}")>
    <HttpDelete>
    Public Function RemoveClientPayOutDetail(clientPayOutDetailId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

        If clientPayOutDetailId <= 0 Then
            Throw New ArgumentException("client PayOut Detail ID is required.")
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

                Me.DeleteArsClientPayOut(clientPayOutDetailId, q.LockId)

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

    Private Sub LoadArsClientPayOut(payOut As ArsClientPayGroupPayOut, ArsClientPayOut As ArsClientPayGroupPayOut)

        DataLib.ScatterValues(payOut, ArsClientPayOut)

    End Sub

    Private Sub LoadArsClientPayOut(row As DataRow, payOut As ArsClientPayGroupPayOut)

        With payOut

            .ClientPayOutDetailId = row.ToInt32("ClientPayOutDetailId")
            .ClientPayGroupId = row.ToInt32("ClientPayGroupId")
            .PayTrxCode = row.ToString("PayTrxCode")
            .PayOutSheetFormula = row.ToString("PayOutSheetFormula")
            .AdminFee = row.ToDecimal("AdminFee")

        End With

    End Sub

    Private Sub LoadArsClientPayOutDeminimisList(rows As DataRowCollection, list As ArsClientPayOutDeminimisList)

        Dim _detail As ArsClientPayOutDeminimis

        For Each _row As DataRow In rows
            _detail = New ArsClientPayOutDeminimis

            With _detail
                .PayOutDeminimisId = _row.ToInt32("PayOutDeminimisId")
                .ClientPayOutDetailId = _row.ToInt32("ClientPayOutDetailId")
                .DeminimisId = _row.ToInt32("DeminimisId")

            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub LoadArsClientPayOutAllowanceList(rows As DataRowCollection, list As ArsClientPayOutAllowanceList)

        Dim _detail As ArsClientPayOutAllowance

        For Each _row As DataRow In rows
            _detail = New ArsClientPayOutAllowance

            With _detail
                .PayOutAllowanceId = _row.ToInt32("PayOutAllowanceId")
                .ClientPayOutDetailId = _row.ToInt32("ClientPayOutDetailId")
                .AllowanceId = _row.ToInt32("AllowanceId")

            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadArsClientPayOutBasicRateList(rows As DataRowCollection, list As ArsClientPayOutBasicRateList)

        Dim _detail As ArsClientPayOutBasicRate

        For Each _row As DataRow In rows
            _detail = New ArsClientPayOutBasicRate

            With _detail
                .PayOutBasicRateId = _row.ToInt32("PayOutBasicRateId")
                .ClientPayOutDetailId = _row.ToInt32("ClientPayOutDetailId")
                .PayTrxCode = _row.ToString("PayTrxCode")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadArsClientDayTypeList(rows As DataRowCollection, list As ArsClientPayOutDayTypeList)

        Dim _detail As ArsClientPayOutDayType
        For Each _row As DataRow In rows
            _detail = New ArsClientPayOutDayType

            With _detail
            '.PayOutDayTypeId = _row.ToInt32("PayOutDayTypeId")
            '.ClientPayOutDayTypeDetailId = _row.ToInt32("ClientPayOutDayTypeDetailId")
            .ClientPayOutDetailId = _row.ToInt32("ClientPayOutDetailId")
                .DayTypeId = _row.ToInt32("DayTypeId")
                .PremiumPercentage = _row.ToDecimal("PremiumPercentage")
                '.AdminFee = _row.ToDecimal("AdminFee")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub InsertArsClientPayOut(payOut As ArsClientPayGroupPayOut)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOut", payOut, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClientPayOut(payOut)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub InsertArsClientPayOutDeminimis(list As ArsClientPayOutDeminimisList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("PayOutDeminimisId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutDeminimis", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutDeminimis In list
                Me.AddInsertUpdateParamsArsClientPayOutDeminimis(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub InsertArsClientPayOutAllowances(list As ArsClientPayOutAllowanceList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("PayOutAllowanceId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutAllowance", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutAllowance In list
                Me.AddInsertUpdateParamsArsClientPayOutAllowances(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertArsClientPayOutBasicRate(list As ArsClientPayOutBasicRateList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("PayOutBasicRateId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutBasicRate", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutBasicRate In list
                Me.AddInsertUpdateParamsArsClientPayOutBasicRate(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertArsClientDayTypes(list As ArsClientPayOutDayTypeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("PayOutDayTypeId")     ' auto-assigned by DB
            .Add("ClientPayOutDayTypeDetailId")

            .Add("ClientPayGroupId")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientPayOutDayType", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutDayType In list
                Me.AddInsertUpdateParamsArsClientDayType(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub UpdateArsClientPayOut(payOut As ArsClientPayGroupPayOut)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ClientPayOutDetailId")
            .Add("LockId")
        End With

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOut", payOut, _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClientPayOut(payOut)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payOut.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateArsClientPayOutDeminimis(list As ArsClientPayOutDeminimisList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("PayOutDeminimisId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOutDeminimis", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutDeminimis In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientPayOutDeminimis(_detail)
                    .Parameters.AddWithValue("@PayOutDeminimisId", _detail.PayOutDeminimisId)

                    .ExecuteNonQuery()
                End If
            Next

        End With


    End Sub

    Private Sub UpdateArsClientPayOutAllowance(list As ArsClientPayOutAllowanceList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("PayOutAllowanceId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOutAllowance", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutAllowance In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientPayOutAllowances(_detail)
                    .Parameters.AddWithValue("@PayOutAllowanceId", _detail.PayOutAllowanceId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateArsClientPayOutBasicRate(list As ArsClientPayOutBasicRateList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("PayOutBasicRateId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientPayOutBasicRate", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientPayOutBasicRate In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientPayOutBasicRate(_detail)
                    .Parameters.AddWithValue("@PayOutBasicRateId", _detail.PayOutBasicRateId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateArsClientDayTypes(list As ArsClientPayOutDayTypeList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("PayOutDayTypeId")

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
                    Me.AddInsertUpdateParamsArsClientDayType(_detail)
               .Parameters.AddWithValue("@ClientPayOutDayTypeDetailId", _detail.ClientPayOutDayTypeDetailId)

               .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientPayOut(payOut As ArsClientPayGroupPayOut)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayOutDetailId", payOut.ClientPayOutDetailId)
            .AddWithValue("@ClientPayGroupId", payOut.ClientPayGroupId)
            .AddWithValue("@PayTrxCode", payOut.PayTrxCode)
            .AddWithValue("@PayOutSheetFormula", payOut.PayOutSheetFormula)
            .AddWithValue("@AdminFee", payOut.AdminFee)


        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientPayOutDeminimis(dtl As ArsClientPayOutDeminimis)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientPayOutDetailId", dtl.ClientPayOutDetailId)
            .AddWithValue("@DeminimisId", dtl.DeminimisId)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsArsClientPayOutAllowances(dtl As ArsClientPayOutAllowance)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientPayOutDetailId", dtl.ClientPayOutDetailId)
            .AddWithValue("@AllowanceId", dtl.AllowanceId)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsArsClientPayOutBasicRate(dtl As ArsClientPayOutBasicRate)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientPayOutDetailId", dtl.ClientPayOutDetailId)
            .AddWithValue("@PayTrxCode", dtl.PayTrxCode)

        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientDayType(dtl As ArsClientPayOutDayType)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@ClientPayOutDayTypeDetailId", dtl.ClientPayOutDayTypeDetailId)
            .AddWithValue("@ClientPayOutDetailId", dtl.ClientPayOutDetailId)
            .AddWithValue("@DayTypeId", dtl.DayTypeId)
            .AddWithValue("@PremiumPercentage", dtl.PremiumPercentage)
            '.AddWithValue("@AdminFee", dtl.AdminFee)
        End With

    End Sub

    Private Sub DeleteArsClientPayOut(clientPayOutDetailId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientPayOutDetailId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsClientPayOut", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ClientPayOutDetailId", clientPayOutDetailId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub DeleteArsClientPayOutDeminimis(list As ArsClientPayOutDeminimisList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientPayOutDeminimis WHERE PayOutDeminimisId=@PayOutDeminimisId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientPayOutDeminimis In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@PayOutDeminimisId", _old.PayOutDeminimisId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteArsClientPayOutAllowance(list As ArsClientPayOutAllowanceList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientPayOutAllowance WHERE PayOutAllowanceId=@PayOutAllowanceId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientPayOutAllowance In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@PayOutAllowanceId", _old.PayOutAllowanceId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteArsClientPayOutBasicRate(list As ArsClientPayOutBasicRateList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientPayOutBasicRate WHERE PayOutBasicRateId=@PayOutBasicRateId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientPayOutBasicRate In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@PayOutBasicRateId", _old.PayOutBasicRateId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteArsClientDayTypes(list As ArsClientPayOutDayTypeList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientPayOutDayType WHERE ClientPayOutDayTypeDetailId=@ClientPayOutDayTypeDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientPayOutDayType In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@PayOutDayTypeId", _old.ClientPayOutDayTypeDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Function HasArsClientPayOutChanges(oldRecord As ArsClientPayGroupPayOut, newRecord As ArsClientPayGroupPayOut) As Boolean

        With oldRecord
            If .PayTrxCode <> newRecord.PayTrxCode Then Return True
            If .ClientPayGroupId <> newRecord.ClientPayGroupId Then Return True
            If .PayTrxCode <> newRecord.PayTrxCode Then Return True
            If .AdminFee <> newRecord.AdminFee Then Return True

            If .PayOutSheetFormula <> newRecord.PayOutSheetFormula Then Return True

        End With

        Return False

    End Function

End Class

Public Class ArsClientPayOutBody
    Inherits ArsClientPayGroupPayOut

    Public Property Deminimis As ArsClientPayOutDeminimis()
    Public Property Allowances As ArsClientPayOutAllowance()
    Public Property BasicRate As ArsClientPayOutBasicRate()
    Public Property DayTypes As ArsClientPayOutDayType()
End Class
