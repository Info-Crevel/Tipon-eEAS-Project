<RoutePrefix("api")>
Public Class BGS0110_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_BGS0110")>
   <Route("references/bgs0110")>
   <HttpGet>
   Public Function GetReferences_BGS0110() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.BGS0110_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "budgetClasses"       ' 1=Annual, 2=Continuing, 3=Supplemental, 4=Automatic, 5=Unprogrammed
                     .Tables(1).TableName = "activities"          ' all defined activities, header and detail
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAllotment")>
   <Route("allotments/{allotmentId}")>
   <HttpGet>
   Public Function GetAllotment(allotmentId As Integer) As IHttpActionResult

      If allotmentId <= 0 Then
         Throw New ArgumentException("Allotment ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0110")
            With _direct
               .AddParameter("AllotmentId", allotmentId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "allot"
                     .Tables(1).TableName = "allotActivities"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAllotmentLog")>
   <Route("allotments/{allotmentId}/log")>
   <HttpGet>
   Public Function GetAllotmentLog(allotmentId As Integer) As IHttpActionResult

      If allotmentId <= 0 Then
         Throw New ArgumentException("Allotment ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0110_Log")
            With _direct
               .AddParameter("AllotmentId", allotmentId)

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

   <SymAuthorization("CreateAllotment")>
   <Route("allotments/{currentUserId}")>
   <HttpPost>
   Public Function CreateAllotment(currentUserId As Integer, <FromBody> allot As AllotmentBody) As IHttpActionResult

      If allot.AllotmentId <> -1 Then
         Throw New ArgumentException("Allotment ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _allotmentId As Integer = SysLib.GetNextSequence("AllotmentId")

         allot.AllotmentId = _allotmentId

         '
         ' Load proposed values from payload
         '
         Dim _bgsAllotment As New BgsAllotment
         Dim _bgsAllotmentActivityList As New BgsAllotmentActivityList

         Me.LoadBgsAllotment(allot, _bgsAllotment)

         Dim _allotActId As Integer = SysLib.GetNextSequence("AllotActId", allot.Activities.Count)

         For Each _allotmentActivity As BgsAllotmentActivity In allot.Activities
            _allotmentActivity.AllotmentId = _allotmentId
            _allotmentActivity.AllotActId = _allotActId
            _bgsAllotmentActivityList.Add(_allotmentActivity)

            _allotActId = _allotActId + 1
         Next

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAllotmentLogDetailList As New SysLogDetailList
         Dim _bgsAllotmentActivityLogDetailList As New SysLogDetailList

         With _bgsAllotment
            AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.AllotmentName, String.Empty, .AllotmentName)
            AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.PostDate, String.Empty, .PostDate.ToDisplayFormat)
            AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.ReferenceDate, String.Empty, .ReferenceDate.ToDisplayFormat)
            AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.ReferenceDocument, String.Empty, .ReferenceDocument)

            Dim _budgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
            AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.BudgetClassId, String.Empty, .BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _budgetClassName)

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

            Me.InsertBgsAllotment(_bgsAllotment)
            Me.InsertBgsAllotmentActivities(_bgsAllotmentActivityList)

            If _bgsAllotmentLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AllotmentId", _bgsAllotment.AllotmentId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAllotmentLogDetailList)
               AppLib.CreateLogDetails(_bgsAllotmentLogDetailList, "BgsAllotmentLogDetail")

            End If

            For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityList

               With _logKeyList
                  .Clear()
                  .Add("AllotmentId", _aaNew.AllotmentId)
                  .Add("ActivityId", _aaNew.ActivityId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentActivityLog", _logKeyList, LogActionId.Add, _currentUserId)

               _bgsAllotmentActivityLogDetailList.Clear()

               With _aaNew
                  If .PSRAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.PSRAmount, "", .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .RLIPAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.RLIPAmount, "", .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .MOOEAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.MOOEAmount, "", .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .FEAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.FEAmount, "", .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .COAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.COAmount, "", .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If

                  AppLib.CreateLogDetails(_bgsAllotmentActivityLogDetailList, "BgsAllotmentActivityLogDetail")

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
         Return Me.Ok(allot.AllotmentId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function


   <SymAuthorization("ModifyAllotment")>
   <Route("allotments/{allotmentId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAllotment(allotmentId As Integer, currentUserId As Integer, <FromBody> allot As AllotmentBody) As IHttpActionResult

      If allotmentId <= 0 Then
         Throw New ArgumentException("Allotment ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsAllotment As New BgsAllotment
         Dim _bgsAllotmentActivityList As New BgsAllotmentActivityList

         Me.LoadBgsAllotment(allot, _bgsAllotment)

         For Each _allotActivity As BgsAllotmentActivity In allot.Activities
            _bgsAllotmentActivityList.Add(_allotActivity)
         Next

         '
         ' Load old values from DB
         '
         Dim _bgsAllotmentOld As New BgsAllotment
         Dim _bgsAllotmentActivityListOld As New BgsAllotmentActivityList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetAllotment(allotmentId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("allot").Rows(0)
            Me.LoadBgsAllotment(_row, _bgsAllotmentOld)
            Me.LoadBgsAllotmentActivityList(_dataSet.Tables("allotActivities").Rows, _bgsAllotmentActivityListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' BgsAllotment
         '

         Dim _bgsAllotmentLogDetailList As New SysLogDetailList

         With _bgsAllotmentOld
            If .AllotmentName <> _bgsAllotment.AllotmentName Then
               AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.AllotmentName, .AllotmentName, _bgsAllotment.AllotmentName)
            End If

            If .PostDate <> _bgsAllotment.PostDate Then
               AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.PostDate, .PostDate.ToDisplayFormat, _bgsAllotment.PostDate.ToDisplayFormat)
            End If

            If .ReferenceDate <> _bgsAllotment.ReferenceDate Then
               AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.ReferenceDate, .ReferenceDate.ToDisplayFormat, _bgsAllotment.ReferenceDate.ToDisplayFormat)
            End If

            If .ReferenceDocument <> _bgsAllotment.ReferenceDocument Then
               AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.ReferenceDocument, .ReferenceDocument, _bgsAllotment.ReferenceDocument)
            End If

            If .BudgetClassId <> _bgsAllotment.BudgetClassId Then
               Dim _oldBudgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
               Dim _newBudgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = _bgsAllotment.BudgetClassId).BudgetClassName

               AppLib.AddLogDetail(_bgsAllotmentLogDetailList, 0, LogColumnId.BudgetClassId, .BudgetClassId.ToString, _bgsAllotment.BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _oldBudgetClassName + "; " + _bgsAllotment.BudgetClassId.ToString + "=" + _newBudgetClassName)
            End If
         End With

         '
         ' BgsAllotmentActivity
         '

         Dim _bgsAllotmentActivityLogDetailList As New SysLogDetailList
         Dim _removeAllotActivityCount As Integer
         Dim _addAllotActivityCount As Integer
         Dim _editAllotActivityCount As Integer

         ' Mark activities for deletion if not found in new list

         For Each _aaOld As BgsAllotmentActivity In _bgsAllotmentActivityListOld
            _aaOld.LogActionId = LogActionId.Delete
            For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityList
               If _aaNew.ActivityId = _aaOld.ActivityId Then
                  _aaOld.LogActionId = 0  ' retain
               End If
            Next

            If _aaOld.LogActionId = LogActionId.Delete Then
               _removeAllotActivityCount = _removeAllotActivityCount + 1
            End If
         Next

         ' Mark activities for addition if not found in old list;
         ' Mark activities for modification if found in old list but with at least 1 property mismatch

         For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityList
            _aaNew.LogActionId = LogActionId.Add
            For Each _aaOld As BgsAllotmentActivity In _bgsAllotmentActivityListOld
               If _aaNew.AllotActId = _aaOld.AllotActId Then
                  _aaNew.LogActionId = 0   ' don't add

                  With _aaNew
                     If .PSRAmount <> _aaOld.PSRAmount OrElse .RLIPAmount <> _aaOld.RLIPAmount Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .MOOEAmount <> _aaOld.MOOEAmount OrElse .FEAmount <> _aaOld.FEAmount OrElse .COAmount <> _aaOld.COAmount Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With

                  Exit For
               End If
            Next

            If _aaNew.LogActionId = LogActionId.Add Then
               _addAllotActivityCount = _addAllotActivityCount + 1
            End If

            If _aaNew.LogActionId = LogActionId.Edit Then
               _editAllotActivityCount = _editAllotActivityCount + 1
            End If

         Next

         Dim _bgsAllotmentActivityListNew As New BgsAllotmentActivityList     ' for adding new Allot. Activities

         If _addAllotActivityCount > 0 Then
            Dim _allotActId As Integer = SysLib.GetNextSequence("AllotActId", _addAllotActivityCount)
            Dim _bgsAllotmentActivity As BgsAllotmentActivity

            For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityList
               If _aaNew.LogActionId = LogActionId.Add Then
                  _bgsAllotmentActivity = New BgsAllotmentActivity
                  _bgsAllotmentActivityListNew.Add(_bgsAllotmentActivity)
                  DataLib.ScatterValues(_aaNew, _bgsAllotmentActivity)

                  _bgsAllotmentActivity.AllotActId = _allotActId
                  _allotActId = _allotActId + 1
               End If
            Next

         End If

         Dim _isBgsAllotmentChanged As Boolean = Me.HasBgsAllotmentChanges(_bgsAllotmentOld, _bgsAllotment)

         If Not _isBgsAllotmentChanged AndAlso _addAllotActivityCount = 0 AndAlso _removeAllotActivityCount = 0 AndAlso _editAllotActivityCount = 0 Then
            '
            ' No changes; just return current allotment
            ' 
            Return Me.GetAllotment(allotmentId)
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

            If _isBgsAllotmentChanged Then
               Me.UpdateBgsAllotment(_bgsAllotment)

               With _logKeyList
                  .Clear()
                  .Add("AllotmentId", _bgsAllotment.AllotmentId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAllotmentLogDetailList)

               AppLib.CreateLogDetails(_bgsAllotmentLogDetailList, "BgsAllotmentLogDetail")

            End If

            '
            ' BgsAllotmentActivity
            '
            If _removeAllotActivityCount > 0 Then
               Me.DeleteBgsAllotmentActivities(_bgsAllotmentActivityListOld)

               For Each _aaOld As BgsAllotmentActivity In _bgsAllotmentActivityListOld
                  If _aaOld.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("AllotmentId", _aaOld.AllotmentId)
                        .Add("ActivityId", _aaOld.ActivityId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsAllotmentActivityLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _bgsAllotmentActivityLogDetailList.Clear()

                     With _aaOld
                        If .PSRAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.PSRAmount, .PSRAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .RLIPAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.RLIPAmount, .RLIPAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .MOOEAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.MOOEAmount, .MOOEAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .FEAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.FEAmount, .FEAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .COAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.COAmount, .COAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                     End With

                     AppLib.CreateLogDetails(_bgsAllotmentActivityLogDetailList, "BgsAllotmentActivityLogDetail")

                  End If
               Next

            End If

            If _addAllotActivityCount > 0 Then
               Me.InsertBgsAllotmentActivities(_bgsAllotmentActivityListNew)

               For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityListNew

                  With _logKeyList
                     .Clear()
                     .Add("AllotmentId", _aaNew.AllotmentId)
                     .Add("ActivityId", _aaNew.ActivityId)
                  End With

                  _id = AppLib.CreateLogHeader("InsBgsAllotmentActivityLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _bgsAllotmentActivityLogDetailList.Clear()

                  With _aaNew
                     If .PSRAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.PSRAmount, "", .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .RLIPAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.RLIPAmount, "", .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .MOOEAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.MOOEAmount, "", .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .FEAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.FEAmount, "", .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .COAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.COAmount, "", .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If

                     AppLib.CreateLogDetails(_bgsAllotmentActivityLogDetailList, "BgsAllotmentActivityLogDetail")

                  End With
               Next

            End If

            If _editAllotActivityCount > 0 Then
               Me.UpdateBgsAllotmentActivities(_bgsAllotmentActivityList)

               For Each _aaNew As BgsAllotmentActivity In _bgsAllotmentActivityList
                  If _aaNew.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("AllotmentId", _aaNew.AllotmentId)
                        .Add("ActivityId", _aaNew.ActivityId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsAllotmentActivityLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _bgsAllotmentActivityLogDetailList.Clear()

                     For Each _aaOld As BgsAllotmentActivity In _bgsAllotmentActivityListOld
                        If _aaNew.AllotActId = _aaOld.AllotActId Then
                           With _aaNew
                              If .PSRAmount <> _aaOld.PSRAmount Then
                                 AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.PSRAmount, _aaOld.PSRAmount.ToString("N"), .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .RLIPAmount <> _aaOld.RLIPAmount Then
                                 AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.RLIPAmount, _aaOld.RLIPAmount.ToString("N"), .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .MOOEAmount <> _aaOld.MOOEAmount Then
                                 AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.MOOEAmount, _aaOld.MOOEAmount.ToString("N"), .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .FEAmount <> _aaOld.FEAmount Then
                                 AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.FEAmount, _aaOld.FEAmount.ToString("N"), .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .COAmount <> _aaOld.COAmount Then
                                 AppLib.AddLogDetail(_bgsAllotmentActivityLogDetailList, _id, LogColumnId.COAmount, _aaOld.COAmount.ToString("N"), .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If

                              AppLib.CreateLogDetails(_bgsAllotmentActivityLogDetailList, "BgsAllotmentActivityLogDetail")

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

   <SymAuthorization("RemoveAllotment")>
   <Route("allotments/{allotmentId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveAllotment(allotmentId As Integer, lockId As String) As IHttpActionResult

      If allotmentId <= 0 Then
         Throw New ArgumentException("Allotment ID is required.")
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

            Me.DeleteBgsAllotment(allotmentId, lockId)

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

   Private Sub LoadBgsAllotment(allot As AllotmentBody, bgsAllotment As BgsAllotment)

      DataLib.ScatterValues(allot, bgsAllotment)

   End Sub

   Private Sub LoadBgsAllotment(row As DataRow, bgsAllotment As BgsAllotment)

      With bgsAllotment
         .AllotmentId = row.ToInt32("AllotmentId")
         .AllotmentName = row.ToString("AllotmentName")
         .PostDate = row.ToDate("PostDate")
         .ReferenceDate = row.ToDate("ReferenceDate")
         .ReferenceDocument = row.ToString("ReferenceDocument")
         .BudgetClassId = row.ToInt32("BudgetClassId")
      End With

   End Sub

   Private Sub LoadBgsAllotmentActivityList(rows As DataRowCollection, list As BgsAllotmentActivityList)

      Dim _bgsAllotmentActivity As BgsAllotmentActivity
      For Each _row As DataRow In rows
         _bgsAllotmentActivity = New BgsAllotmentActivity

         With _bgsAllotmentActivity
            .AllotActId = _row.ToInt32("AllotActId")
            .AllotmentId = _row.ToInt32("AllotmentId")
            .ActivityId = _row.ToInt32("ActivityId")
            .PSRAmount = _row.ToDecimal("PSRAmount")
            .RLIPAmount = _row.ToDecimal("RLIPAmount")
            .MOOEAmount = _row.ToDecimal("MOOEAmount")
            .FEAmount = _row.ToDecimal("FEAmount")
            .COAmount = _row.ToDecimal("COAmount")
         End With

         list.Add(_bgsAllotmentActivity)

      Next

   End Sub

   Private Sub InsertBgsAllotment(allot As BgsAllotment)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAllotment", allot, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsAllotment(allot)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertBgsAllotmentActivities(list As BgsAllotmentActivityList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAllotmentActivity", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _allotActivity As BgsAllotmentActivity In list
            Me.AddInsertUpdateParamsBgsAllotmentActivity(_allotActivity)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateBgsAllotment(allot As BgsAllotment)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AllotmentId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAllotment", allot, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsAllotment(allot)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(allot.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsAllotmentActivities(list As BgsAllotmentActivityList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("AllotActId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAllotmentActivity", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _allotActivity As BgsAllotmentActivity In list
            If _allotActivity.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsAllotmentActivity(_allotActivity)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsAllotment(allot As BgsAllotment)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AllotmentId", allot.AllotmentId)
         .AddWithValue("@AllotmentName", allot.AllotmentName)
         .AddWithValue("@PostDate", allot.PostDate)
         .AddWithValue("@ReferenceDate", allot.ReferenceDate)
         .AddWithValue("@ReferenceDocument", allot.ReferenceDocument)
         .AddWithValue("@BudgetClassId", allot.BudgetClassId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsAllotmentActivity(allotActivity As BgsAllotmentActivity)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AllotActId", allotActivity.AllotActId)
         .AddWithValue("@AllotmentId", allotActivity.AllotmentId)
         .AddWithValue("@ActivityId", allotActivity.ActivityId)
         .AddWithValue("@PSRAmount", allotActivity.PSRAmount)
         .AddWithValue("@RLIPAmount", allotActivity.RLIPAmount)
         .AddWithValue("@MOOEAmount", allotActivity.MOOEAmount)
         .AddWithValue("@FEAmount", allotActivity.FEAmount)
         .AddWithValue("@COAmount", allotActivity.COAmount)
      End With

   End Sub

   Private Sub DeleteBgsAllotment(allotmentId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AllotmentId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsAllotment", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AllotmentId", allotmentId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteBgsAllotmentActivities(list As BgsAllotmentActivityList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsAllotmentActivity WHERE AllotActId=@AllotActId"
         .CommandType = CommandType.Text

         For Each _aaOld As BgsAllotmentActivity In list
            If _aaOld.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@AllotActId", _aaOld.AllotActId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasBgsAllotmentChanges(oldRecord As BgsAllotment, newRecord As BgsAllotment) As Boolean

      With oldRecord
         If .AllotmentName <> newRecord.AllotmentName Then Return True
         If .PostDate <> newRecord.PostDate Then Return True
         If .ReferenceDate <> newRecord.ReferenceDate Then Return True
         If .ReferenceDocument <> newRecord.ReferenceDocument Then Return True
         If .BudgetClassId <> newRecord.BudgetClassId Then Return True
      End With

      Return False

   End Function

End Class

Public Class AllotmentBody
   Inherits BgsAllotment

   Public Property Activities As BgsAllotmentActivity()

End Class

