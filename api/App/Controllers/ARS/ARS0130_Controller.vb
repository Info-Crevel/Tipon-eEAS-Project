<RoutePrefix("api")>
Public Class ARS0130_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_ARS0130")>
   <Route("references/ars0130")>
   <HttpGet>
   Public Function GetReferences_ARS0130() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0130_References")
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

   <SymAuthorization("GetClientPayGroupBilling")>
   <Route("client-pay-group-billings/{clientBillingDetailId}")>
   <HttpGet>
   Public Function GetClientPayGroupBilling(clientBillingDetailId As Integer) As IHttpActionResult

      If clientBillingDetailId <= 0 Then
         Throw New ArgumentException("Client Billing Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0130")
            With _direct
               .AddParameter("ClientBillingDetailId", clientBillingDetailId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "billing"
                     .Tables(1).TableName = "billingDeminimis"
                     .Tables(2).TableName = "billingAllowances"
                     .Tables(3).TableName = "billingBasicRate"
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

   <SymAuthorization("CreateClientPayGroupBilling")>
   <Route("client-pay-group-billings/{currentUserId}")>
   <HttpPost>
   Public Function CreateClientPayGroupBilling(currentUserId As Integer, <FromBody> billing As ArsClientBillingBody) As IHttpActionResult

      If billing.ClientBillingDetailId <> -1 Then
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
         Dim _clientBillingDetailId As Integer = SysLib.GetNextSequence("ClientBillingDetailId")

         billing.ClientBillingDetailId = _clientBillingDetailId
         'billing.ClientPayGroupId = 42
         '
         ' Load proposed values from payload
         '
         Dim _arsClientBilling As New ArsClientPayGroupBilling
         Dim _arsClientBillingDeminimisList As New ArsClientBillingDeminimisList
         Dim _arsClientBillingAllowanceList As New ArsClientBillingAllowanceList
         Dim _arsClientBillingBasicRateList As New ArsClientBillingBasicRateList
         Dim _arsClientBillingDayTypeList As New ArsClientBillingDayTypeList

         Me.LoadArsClientBilling(billing, _arsClientBilling)

         For Each _deminimis As ArsClientBillingDeminimis In billing.Deminimis
            _deminimis.ClientBillingDetailId = _clientBillingDetailId
            _arsClientBillingDeminimisList.Add(_deminimis)
         Next

         For Each _allowances As ArsClientBillingAllowance In billing.Allowances
            _allowances.ClientBillingDetailId = _clientBillingDetailId
            _arsClientBillingAllowanceList.Add(_allowances)
         Next

         For Each _basicRate As ArsClientBillingBasicRate In billing.BasicRate
            _basicRate.ClientBillingDetailId = _clientBillingDetailId
            _arsClientBillingBasicRateList.Add(_basicRate)
         Next

         For Each _dayType As ArsClientBillingDayType In billing.DayTypes
            _dayType.ClientBillingDetailId = _clientBillingDetailId
            _arsClientBillingDayTypeList.Add(_dayType)
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

            Me.InsertArsClientBilling(_arsClientBilling)

            If _arsClientBillingDeminimisList.Count > 0 Then
               Me.InsertArsClientBillingDeminimis(_arsClientBillingDeminimisList)
            End If

            If _arsClientBillingAllowanceList.Count > 0 Then
               Me.InsertArsClientBillingAllowances(_arsClientBillingAllowanceList)
            End If

            If _arsClientBillingBasicRateList.Count > 0 Then
               Me.InsertArsClientBillingBasicRate(_arsClientBillingBasicRateList)
            End If

            If _arsClientBillingDayTypeList.Count > 0 Then
               Me.InsertArsClientDayTypes(_arsClientBillingDayTypeList)
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
         Return Me.Ok(billing.ClientBillingDetailId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyClientBillingDetail")>
   <Route("client-pay-group-billings/{clientBillingDetailId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCluster(clientBillingDetailId As Integer, currentUserId As Integer, <FromBody> billing As ArsClientBillingBody) As IHttpActionResult

      If clientBillingDetailId <= 0 Then
         Throw New ArgumentException("Client Billing Detail ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _arsClientBilling As New ArsClientPayGroupBilling
         Dim _arsClientBillingDeminimisList As New ArsClientBillingDeminimisList
         Dim _arsClientBillingAllowanceList As New ArsClientBillingAllowanceList
         Dim _arsClientBillingBasicRateList As New ArsClientBillingBasicRateList
         Dim _arsClientBillingDayTypeList As New ArsClientBillingDayTypeList

         Me.LoadArsClientBilling(billing, _arsClientBilling)

         For Each _deminimis As ArsClientBillingDeminimis In billing.Deminimis
            _arsClientBillingDeminimisList.Add(_deminimis)
         Next

         For Each _allowances As ArsClientBillingAllowance In billing.Allowances
            _arsClientBillingAllowanceList.Add(_allowances)
         Next

         For Each _basicRate As ArsClientBillingBasicRate In billing.BasicRate
            _arsClientBillingBasicRateList.Add(_basicRate)
         Next

         For Each _dayType As ArsClientBillingDayType In billing.DayTypes
            _dayType.ClientBillingDetailId = clientBillingDetailId
            _arsClientBillingDayTypeList.Add(_dayType)
         Next


         '
         ' Load old values from DB
         '
         Dim _arsClientBillingOld As New ArsClientPayGroupBilling
         Dim _arsClientBillingDeminimisListOld As New ArsClientBillingDeminimisList
         Dim _arsClientBillingAllowanceListOld As New ArsClientBillingAllowanceList
         Dim _arsClientBillingBasicRateListOld As New ArsClientBillingBasicRateList
         Dim _arsClientBillingDayTypeListOld As New ArsClientBillingDayTypeList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetClientPayGroupBilling(clientBillingDetailId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("billing").Rows(0)
            Me.LoadArsClientBilling(_row, _arsClientBillingOld)
            Me.LoadArsClientBillingDeminimisList(_dataSet.Tables("billingDeminimis").Rows, _arsClientBillingDeminimisListOld)
            Me.LoadArsClientBillingAllowanceList(_dataSet.Tables("billingAllowances").Rows, _arsClientBillingAllowanceListOld)
            Me.LoadArsClientBillingBasicRateList(_dataSet.Tables("billingBasicRate").Rows, _arsClientBillingBasicRateListOld)
            Me.LoadArsClientDayTypeList(_dataSet.Tables("dayTypes").Rows, _arsClientBillingDayTypeListOld)
         End Using


         Dim _removeDeminimisCount As Integer
         Dim _addDeminimisCount As Integer
         Dim _editDeminimisCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientBillingDeminimis In _arsClientBillingDeminimisListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientBillingDeminimis In _arsClientBillingDeminimisList
               If _new.BillingDeminimisId = _old.BillingDeminimisId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDeminimisCount = _removeDeminimisCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsClientBillingDeminimis In _arsClientBillingDeminimisList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientBillingDeminimis In _arsClientBillingDeminimisListOld
               If _new.BillingDeminimisId = _old.BillingDeminimisId Then
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

         Dim _arsClientBillingDeminimisListNew As New ArsClientBillingDeminimisList      ' for adding new Template Deminimiss

         If _addDeminimisCount > 0 Then
            Dim _arsClientBillingDeminimis As ArsClientBillingDeminimis

            For Each _new As ArsClientBillingDeminimis In _arsClientBillingDeminimisList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientBillingDeminimis = New ArsClientBillingDeminimis
                  _arsClientBillingDeminimisListNew.Add(_arsClientBillingDeminimis)
                  DataLib.ScatterValues(_new, _arsClientBillingDeminimis)
                  _arsClientBillingDeminimis.ClientBillingDetailId = _arsClientBilling.ClientBillingDetailId
               End If
            Next

         End If

         'Allowance

         Dim _removeAllowanceCount As Integer
         Dim _addAllowanceCount As Integer
         Dim _editAllowanceCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientBillingAllowance In _arsClientBillingAllowanceListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientBillingAllowance In _arsClientBillingAllowanceList
               If _new.BillingAllowanceId = _old.BillingAllowanceId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeAllowanceCount = _removeAllowanceCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsClientBillingAllowance In _arsClientBillingAllowanceList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientBillingAllowance In _arsClientBillingAllowanceListOld
               If _new.BillingAllowanceId = _old.BillingAllowanceId Then
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

         Dim _arsClientBillingAllowanceListNew As New ArsClientBillingAllowanceList      ' for adding new Template Allowances

         If _addAllowanceCount > 0 Then
            Dim _arsClientBillingAllowance As ArsClientBillingAllowance

            For Each _new As ArsClientBillingAllowance In _arsClientBillingAllowanceList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientBillingAllowance = New ArsClientBillingAllowance
                  _arsClientBillingAllowanceListNew.Add(_arsClientBillingAllowance)
                  DataLib.ScatterValues(_new, _arsClientBillingAllowance)
                  _arsClientBillingAllowance.ClientBillingDetailId = _arsClientBilling.ClientBillingDetailId
               End If
            Next

         End If

         'Basic Rate

         Dim _removeBasicRateCount As Integer
         Dim _addBasicRateCount As Integer
         Dim _editBasicRateCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientBillingBasicRate In _arsClientBillingBasicRateListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientBillingBasicRate In _arsClientBillingBasicRateList
               If _new.BillingBasicRateId = _old.BillingBasicRateId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeBasicRateCount = _removeBasicRateCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsClientBillingBasicRate In _arsClientBillingBasicRateList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientBillingBasicRate In _arsClientBillingBasicRateListOld
               If _new.BillingBasicRateId = _old.BillingBasicRateId Then
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

         Dim _arsClientBillingBasicRateListNew As New ArsClientBillingBasicRateList      ' for adding new Template BasicRates

         If _addBasicRateCount > 0 Then
            Dim _arsClientBillingBasicRate As ArsClientBillingBasicRate

            For Each _new As ArsClientBillingBasicRate In _arsClientBillingBasicRateList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientBillingBasicRate = New ArsClientBillingBasicRate
                  _arsClientBillingBasicRateListNew.Add(_arsClientBillingBasicRate)
                  DataLib.ScatterValues(_new, _arsClientBillingBasicRate)
                  _arsClientBillingBasicRate.ClientBillingDetailId = _arsClientBilling.ClientBillingDetailId
               End If
            Next

         End If


#Region "ArsClientBillingDayType"

         Dim _removeClientDayTypeCount As Integer
         Dim _addClientDayTypeCount As Integer
         Dim _editClientDayTypeCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsClientBillingDayType In _arsClientBillingDayTypeListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsClientBillingDayType In _arsClientBillingDayTypeList
               If _new.BillingDayTypeId = _old.BillingDayTypeId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeClientDayTypeCount = _removeClientDayTypeCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsClientBillingDayType In _arsClientBillingDayTypeList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsClientBillingDayType In _arsClientBillingDayTypeListOld
               If _new.BillingDayTypeId = _old.BillingDayTypeId Then
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

         Dim _arsClientBillingDayTypeListNew As New ArsClientBillingDayTypeList      ' for adding new Template Details

         If _addClientDayTypeCount > 0 Then
            Dim _arsClientBillingDayType As ArsClientBillingDayType

            For Each _new As ArsClientBillingDayType In _arsClientBillingDayTypeList
               If _new.LogActionId = LogActionId.Add Then
                  _arsClientBillingDayType = New ArsClientBillingDayType
                  _arsClientBillingDayTypeListNew.Add(_arsClientBillingDayType)
                  DataLib.ScatterValues(_new, _arsClientBillingDayType)
                  _arsClientBillingDayType.ClientBillingDetailId = _arsClientBillingDayType.ClientBillingDetailId
               End If
            Next

         End If

#End Region


         Dim _isArsClientBillingChanged As Boolean = Me.HasArsClientBillingChanges(_arsClientBillingOld, _arsClientBilling)

         If Not _isArsClientBillingChanged AndAlso _addDeminimisCount = 0 AndAlso _removeDeminimisCount = 0 AndAlso _editDeminimisCount = 0 AndAlso _addAllowanceCount = 0 AndAlso _removeAllowanceCount = 0 AndAlso _editAllowanceCount = 0 AndAlso _addBasicRateCount = 0 AndAlso _removeBasicRateCount = 0 AndAlso _editBasicRateCount = 0 AndAlso _addClientDayTypeCount = 0 AndAlso _removeClientDayTypeCount = 0 AndAlso _editClientDayTypeCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetClientPayGroupBilling(clientBillingDetailId)
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

            If _isArsClientBillingChanged Then
               Me.UpdateArsClientBilling(_arsClientBilling)
            End If

            If _removeDeminimisCount > 0 Then
               Me.DeleteArsClientBillingDeminimis(_arsClientBillingDeminimisListOld)

            End If

            If _addDeminimisCount > 0 Then
               Me.InsertArsClientBillingDeminimis(_arsClientBillingDeminimisListNew)

            End If

            If _editDeminimisCount > 0 Then
               Me.UpdateArsClientBillingDeminimis(_arsClientBillingDeminimisList)

            End If

            If _removeAllowanceCount > 0 Then
               Me.DeleteArsClientBillingAllowance(_arsClientBillingAllowanceListOld)

            End If

            If _addAllowanceCount > 0 Then
               Me.InsertArsClientBillingAllowances(_arsClientBillingAllowanceListNew)

            End If

            If _editAllowanceCount > 0 Then
               Me.UpdateArsClientBillingAllowance(_arsClientBillingAllowanceList)

            End If

            If _removeBasicRateCount > 0 Then
               Me.DeleteArsClientBillingBasicRate(_arsClientBillingBasicRateListOld)

            End If

            If _addBasicRateCount > 0 Then
               Me.InsertArsClientBillingBasicRate(_arsClientBillingBasicRateListNew)

            End If

            If _editBasicRateCount > 0 Then
               Me.UpdateArsClientBillingBasicRate(_arsClientBillingBasicRateList)

            End If

            If _removeClientDayTypeCount > 0 Then
               Me.DeleteArsClientDayTypes(_arsClientBillingDayTypeListOld)
            End If

            If _addClientDayTypeCount > 0 Then
               Me.InsertArsClientDayTypes(_arsClientBillingDayTypeListNew)
            End If

            If _editClientDayTypeCount > 0 Then
               Me.UpdateArsClientDayTypes(_arsClientBillingDayTypeList)
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

   <SymAuthorization("RemoveClientBillingDetail")>
   <Route("client-pay-group-billings/{clientBillingDetailId}")>
   <HttpDelete>
   Public Function RemoveClientBillingDetail(clientBillingDetailId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If clientBillingDetailId <= 0 Then
         Throw New ArgumentException("client Billing Detail ID is required.")
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

            Me.DeleteArsClientBilling(clientBillingDetailId, q.LockId)

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

   Private Sub LoadArsClientBilling(billing As ArsClientPayGroupBilling, ArsClientBilling As ArsClientPayGroupBilling)

      DataLib.ScatterValues(billing, ArsClientBilling)

   End Sub

   Private Sub LoadArsClientBilling(row As DataRow, billing As ArsClientPayGroupBilling)

      With billing
         .ClientBillingDetailId = row.ToInt32("ClientBillingDetailId")
         .ClientPayGroupId = row.ToInt32("ClientPayGroupId")
         .PayTrxCode = row.ToString("PayTrxCode")
         .BillingSheetFormula = row.ToString("BillingSheetFormula")
         .AdminFee = row.ToDecimal("AdminFee")
         .OrgFlag = row.ToBoolean("OrgFlag")
      End With

   End Sub

   Private Sub LoadArsClientBillingDeminimisList(rows As DataRowCollection, list As ArsClientBillingDeminimisList)

      Dim _detail As ArsClientBillingDeminimis

      For Each _row As DataRow In rows
         _detail = New ArsClientBillingDeminimis

         With _detail
            .BillingDeminimisId = _row.ToInt32("BillingDeminimisId")
            .ClientBillingDetailId = _row.ToInt32("ClientBillingDetailId")
            .DeminimisId = _row.ToInt32("DeminimisId")

         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub LoadArsClientBillingAllowanceList(rows As DataRowCollection, list As ArsClientBillingAllowanceList)

      Dim _detail As ArsClientBillingAllowance

      For Each _row As DataRow In rows
         _detail = New ArsClientBillingAllowance

         With _detail
            .BillingAllowanceId = _row.ToInt32("BillingAllowanceId")
            .ClientBillingDetailId = _row.ToInt32("ClientBillingDetailId")
            .AllowanceId = _row.ToInt32("AllowanceId")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsClientBillingBasicRateList(rows As DataRowCollection, list As ArsClientBillingBasicRateList)

      Dim _detail As ArsClientBillingBasicRate

      For Each _row As DataRow In rows
         _detail = New ArsClientBillingBasicRate

         With _detail
            .BillingBasicRateId = _row.ToInt32("BillingBasicRateId")
            .ClientBillingDetailId = _row.ToInt32("ClientBillingDetailId")
            .PayTrxCode = _row.ToString("PayTrxCode")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsClientDayTypeList(rows As DataRowCollection, list As ArsClientBillingDayTypeList)

      Dim _detail As ArsClientBillingDayType
      For Each _row As DataRow In rows
         _detail = New ArsClientBillingDayType

         With _detail
            .BillingDayTypeId = _row.ToInt32("BillingDayTypeId")
            .ClientBillingDetailId = _row.ToInt32("ClientBillingDetailId")
            .DayTypeId = _row.ToInt32("DayTypeId")
            .PremiumPercentage = _row.ToDecimal("PremiumPercentage")
            .AdminFee = _row.ToDecimal("AdminFee")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertArsClientBilling(billing As ArsClientPayGroupBilling)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBilling", billing, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsClientBilling(billing)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertArsClientBillingDeminimis(list As ArsClientBillingDeminimisList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("BillingDeminimisId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBillingDeminimis", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingDeminimis In list
            Me.AddInsertUpdateParamsArsClientBillingDeminimis(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub InsertArsClientBillingAllowances(list As ArsClientBillingAllowanceList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("BillingAllowanceId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBillingAllowance", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingAllowance In list
            Me.AddInsertUpdateParamsArsClientBillingAllowances(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsClientBillingBasicRate(list As ArsClientBillingBasicRateList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("BillingBasicRateId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBillingBasicRate", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingBasicRate In list
            Me.AddInsertUpdateParamsArsClientBillingBasicRate(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsClientDayTypes(list As ArsClientBillingDayTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
            .Add("BillingDayTypeId")     ' auto-assigned by DB

            .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientBillingDayType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingDayType In list
            Me.AddInsertUpdateParamsArsClientDayType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateArsClientBilling(billing As ArsClientPayGroupBilling)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClientBillingDetailId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBilling", billing, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsClientBilling(billing)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(billing.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateArsClientBillingDeminimis(list As ArsClientBillingDeminimisList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingDeminimisId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBillingDeminimis", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingDeminimis In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientBillingDeminimis(_detail)
               .Parameters.AddWithValue("@BillingDeminimisId", _detail.BillingDeminimisId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub UpdateArsClientBillingAllowance(list As ArsClientBillingAllowanceList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingAllowanceId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBillingAllowance", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingAllowance In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientBillingAllowances(_detail)
               .Parameters.AddWithValue("@BillingAllowanceId", _detail.BillingAllowanceId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub UpdateArsClientBillingBasicRate(list As ArsClientBillingBasicRateList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingBasicRateId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBillingBasicRate", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingBasicRate In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientBillingBasicRate(_detail)
               .Parameters.AddWithValue("@BillingBasicRateId", _detail.BillingBasicRateId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub UpdateArsClientDayTypes(list As ArsClientBillingDayTypeList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingDayTypeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientBillingDayType", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsClientBillingDayType In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsClientDayType(_detail)
               .Parameters.AddWithValue("@BillingDayTypeId", _detail.BillingDayTypeId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsClientBilling(billing As ArsClientPayGroupBilling)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ClientBillingDetailId", billing.ClientBillingDetailId)
         .AddWithValue("@ClientPayGroupId", billing.ClientPayGroupId)
         .AddWithValue("@PayTrxCode", billing.PayTrxCode)
         .AddWithValue("@BillingSheetFormula", billing.BillingSheetFormula)
         .AddWithValue("@AdminFee", billing.AdminFee)
         .AddWithValue("@OrgFlag", billing.OrgFlag)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsClientBillingDeminimis(dtl As ArsClientBillingDeminimis)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ClientBillingDetailId", dtl.ClientBillingDetailId)
         .AddWithValue("@DeminimisId", dtl.DeminimisId)

      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsClientBillingAllowances(dtl As ArsClientBillingAllowance)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ClientBillingDetailId", dtl.ClientBillingDetailId)
         .AddWithValue("@AllowanceId", dtl.AllowanceId)

      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsClientBillingBasicRate(dtl As ArsClientBillingBasicRate)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ClientBillingDetailId", dtl.ClientBillingDetailId)
         .AddWithValue("@PayTrxCode", dtl.PayTrxCode)

      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsClientDayType(dtl As ArsClientBillingDayType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ClientBillingDetailId", dtl.ClientBillingDetailId)
         .AddWithValue("@DayTypeId", dtl.DayTypeId)
         .AddWithValue("@PremiumPercentage", dtl.PremiumPercentage)
         .AddWithValue("@AdminFee", dtl.AdminFee)
      End With

   End Sub

   Private Sub DeleteArsClientBilling(clientBillingDetailId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ClientBillingDetailId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsClientBilling", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ClientBillingDetailId", clientBillingDetailId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteArsClientBillingDeminimis(list As ArsClientBillingDeminimisList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientBillingDeminimis WHERE BillingDeminimisId=@BillingDeminimisId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientBillingDeminimis In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BillingDeminimisId", _old.BillingDeminimisId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsClientBillingAllowance(list As ArsClientBillingAllowanceList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientBillingAllowance WHERE BillingAllowanceId=@BillingAllowanceId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientBillingAllowance In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BillingAllowanceId", _old.BillingAllowanceId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsClientBillingBasicRate(list As ArsClientBillingBasicRateList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientBillingBasicRate WHERE BillingBasicRateId=@BillingBasicRateId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientBillingBasicRate In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BillingBasicRateId", _old.BillingBasicRateId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsClientDayTypes(list As ArsClientBillingDayTypeList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsClientBillingDayType WHERE BillingDayTypeId=@BillingDayTypeId"
         .CommandType = CommandType.Text

         For Each _old As ArsClientBillingDayType In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BillingDayTypeId", _old.BillingDayTypeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasArsClientBillingChanges(oldRecord As ArsClientPayGroupBilling, newRecord As ArsClientPayGroupBilling) As Boolean

      With oldRecord
         If .PayTrxCode <> newRecord.PayTrxCode Then Return True
         If .ClientPayGroupId <> newRecord.ClientPayGroupId Then Return True
         If .PayTrxCode <> newRecord.PayTrxCode Then Return True
         If .AdminFee <> newRecord.AdminFee Then Return True
         If .OrgFlag <> newRecord.OrgFlag Then Return True
         If .BillingSheetFormula <> newRecord.BillingSheetFormula Then Return True

      End With

      Return False

   End Function

End Class

Public Class ArsClientBillingBody
   Inherits ArsClientPayGroupBilling

   Public Property Deminimis As ArsClientBillingDeminimis()
   Public Property Allowances As ArsClientBillingAllowance()
   Public Property BasicRate As ArsClientBillingBasicRate()
   Public Property DayTypes As ArsClientBillingDayType()
End Class
