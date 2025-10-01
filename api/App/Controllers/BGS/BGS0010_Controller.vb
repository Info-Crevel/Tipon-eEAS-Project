<RoutePrefix("api")>
Public Class BGS0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_BGS0010")>
   <Route("references/bgs0010")>
   <HttpGet>
   Public Function GetReferences_BGS0010() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.BGS0010_References")
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

   <SymAuthorization("GetAppropriation")>
   <Route("appropriations/{appropriationId}")>
   <HttpGet>
   Public Function GetAppropriation(appropriationId As Integer) As IHttpActionResult

      If appropriationId <= 0 Then
         Throw New ArgumentException("Appropriation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0010")
            With _direct
               .AddParameter("AppropriationId", appropriationId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "appro"
                     .Tables(1).TableName = "approActivities"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAppropriationLog")>
   <Route("appropriations/{appropriationId}/log")>
   <HttpGet>
   Public Function GetAppropriationLog(appropriationId As Integer) As IHttpActionResult

      If appropriationId <= 0 Then
         Throw New ArgumentException("Appropriation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0010_Log")
            With _direct
               .AddParameter("AppropriationId", appropriationId)

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

   <SymAuthorization("CreateAppropriation")>
   <Route("appropriations/{currentUserId}")>
   <HttpPost>
   Public Function CreateAppropriation(currentUserId As Integer, <FromBody> appro As AppropriationBody) As IHttpActionResult

      If appro.AppropriationId <> -1 Then
         Throw New ArgumentException("Appropriation ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _appropriationId As Integer = SysLib.GetNextSequence("AppropriationId")

         appro.AppropriationId = _appropriationId

         '
         ' Load proposed values from payload
         '
         Dim _bgsAppropriation As New BgsAppropriation
         Dim _bgsAppropriationActivityList As New BgsAppropriationActivityList

         Me.LoadBgsAppropriation(appro, _bgsAppropriation)

         Dim _approActId As Integer = SysLib.GetNextSequence("ApproActId", appro.Activities.Count)

         For Each _appropriationActivity As BgsAppropriationActivity In appro.Activities
            _appropriationActivity.AppropriationId = _appropriationId
            _appropriationActivity.ApproActId = _approActId
            _bgsAppropriationActivityList.Add(_appropriationActivity)

            _approActId = _approActId + 1
         Next

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAppropriationLogDetailList As New SysLogDetailList
         Dim _bgsAppropriationActivityLogDetailList As New SysLogDetailList

         With _bgsAppropriation
            AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.AppropriationName, String.Empty, .AppropriationName)
            AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.PostDate, String.Empty, .PostDate.ToDisplayFormat)
            AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.ReferenceDate, String.Empty, .ReferenceDate.ToDisplayFormat)
            AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.ReferenceDocument, String.Empty, .ReferenceDocument)

            Dim _budgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
            AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.BudgetClassId, String.Empty, .BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _budgetClassName)

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

            Me.InsertBgsAppropriation(_bgsAppropriation)
            Me.InsertBgsAppropriationActivities(_bgsAppropriationActivityList)

            If _bgsAppropriationLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AppropriationId", _bgsAppropriation.AppropriationId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAppropriationLogDetailList)
               AppLib.CreateLogDetails(_bgsAppropriationLogDetailList, "BgsAppropriationLogDetail")

            End If

            For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityList

               With _logKeyList
                  .Clear()
                  .Add("AppropriationId", _aaNew.AppropriationId)
                  .Add("ActivityId", _aaNew.ActivityId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationActivityLog", _logKeyList, LogActionId.Add, _currentUserId)

               _bgsAppropriationActivityLogDetailList.Clear()

               With _aaNew
                  If .PSRAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.PSRAmount, "", .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .RLIPAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.RLIPAmount, "", .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .MOOEAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.MOOEAmount, "", .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .FEAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.FEAmount, "", .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If
                  If .COAmount > 0 Then
                     AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.COAmount, "", .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                  End If

                  AppLib.CreateLogDetails(_bgsAppropriationActivityLogDetailList, "BgsAppropriationActivityLogDetail")

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
         Return Me.Ok(appro.AppropriationId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   '<SymAuthorization("ModifyAppropriation")>
   '<Route("appropriations/{appropriationId}")>
   '<HttpPut>
   'Public Function ModifyAppropriation(appropriationId As Integer, <FromBody> appro As AppropriationBody) As IHttpActionResult

   '   If appropriationId <= 0 Then
   '      Throw New ArgumentException("Appropriation ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '
   '      Dim _bgsAppropriation As New BgsAppropriation

   '      Me.LoadBgsAppropriation(appro, _bgsAppropriation)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.UpdateBgsAppropriation(_bgsAppropriation)

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

   <SymAuthorization("ModifyAppropriation")>
   <Route("appropriations/{appropriationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAppropriation(appropriationId As Integer, currentUserId As Integer, <FromBody> appro As AppropriationBody) As IHttpActionResult

      If appropriationId <= 0 Then
         Throw New ArgumentException("Appropriation ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsAppropriation As New BgsAppropriation
         Dim _bgsAppropriationActivityList As New BgsAppropriationActivityList

         Me.LoadBgsAppropriation(appro, _bgsAppropriation)

         For Each _approActivity As BgsAppropriationActivity In appro.Activities
            _bgsAppropriationActivityList.Add(_approActivity)
         Next

         '
         ' Load old values from DB
         '
         Dim _bgsAppropriationOld As New BgsAppropriation
         Dim _bgsAppropriationActivityListOld As New BgsAppropriationActivityList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetAppropriation(appropriationId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("appro").Rows(0)
            Me.LoadBgsAppropriation(_row, _bgsAppropriationOld)
            Me.LoadBgsAppropriationActivityList(_dataSet.Tables("approActivities").Rows, _bgsAppropriationActivityListOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' BgsAppropriation
         '

         Dim _bgsAppropriationLogDetailList As New SysLogDetailList

         With _bgsAppropriationOld
            If .AppropriationName <> _bgsAppropriation.AppropriationName Then
               AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.AppropriationName, .AppropriationName, _bgsAppropriation.AppropriationName)
            End If

            If .PostDate <> _bgsAppropriation.PostDate Then
               AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.PostDate, .PostDate.ToDisplayFormat, _bgsAppropriation.PostDate.ToDisplayFormat)
            End If

            If .ReferenceDate <> _bgsAppropriation.ReferenceDate Then
               AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.ReferenceDate, .ReferenceDate.ToDisplayFormat, _bgsAppropriation.ReferenceDate.ToDisplayFormat)
            End If

            If .ReferenceDocument <> _bgsAppropriation.ReferenceDocument Then
               AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.ReferenceDocument, .ReferenceDocument, _bgsAppropriation.ReferenceDocument)
            End If

            If .BudgetClassId <> _bgsAppropriation.BudgetClassId Then
               Dim _oldBudgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = .BudgetClassId).BudgetClassName
               Dim _newBudgetClassName As String = FrsSession.DbsBudgetClass.Rows.Find(Function(m) m.BudgetClassId = _bgsAppropriation.BudgetClassId).BudgetClassName

               AppLib.AddLogDetail(_bgsAppropriationLogDetailList, 0, LogColumnId.BudgetClassId, .BudgetClassId.ToString, _bgsAppropriation.BudgetClassId.ToString, .BudgetClassId.ToString + "=" + _oldBudgetClassName + "; " + _bgsAppropriation.BudgetClassId.ToString + "=" + _newBudgetClassName)
            End If
         End With

         '
         ' BgsAppropriationActivity
         '

         Dim _bgsAppropriationActivityLogDetailList As New SysLogDetailList
         Dim _removeApproActivityCount As Integer
         Dim _addApproActivityCount As Integer
         Dim _editApproActivityCount As Integer

         ' Mark activities for deletion if not found in new list

         For Each _aaOld As BgsAppropriationActivity In _bgsAppropriationActivityListOld
            _aaOld.LogActionId = LogActionId.Delete
            For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityList
               If _aaNew.ActivityId = _aaOld.ActivityId Then
                  _aaOld.LogActionId = 0  ' retain
               End If
            Next

            If _aaOld.LogActionId = LogActionId.Delete Then
               _removeApproActivityCount = _removeApproActivityCount + 1
            End If
         Next

         ' Mark activities for addition if not found in old list;
         ' Mark activities for modification if found in old list but with at least 1 property mismatch

         For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityList
            _aaNew.LogActionId = LogActionId.Add
            For Each _aaOld As BgsAppropriationActivity In _bgsAppropriationActivityListOld
               If _aaNew.ApproActId = _aaOld.ApproActId Then
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
               _addApproActivityCount = _addApproActivityCount + 1
            End If

            If _aaNew.LogActionId = LogActionId.Edit Then
               _editApproActivityCount = _editApproActivityCount + 1
            End If

         Next

         Dim _bgsAppropriationActivityListNew As New BgsAppropriationActivityList     ' for adding new Appro. Activities

         If _addApproActivityCount > 0 Then
            Dim _approActId As Integer = SysLib.GetNextSequence("ApproActId", _addApproActivityCount)
            Dim _bgsAppropriationActivity As BgsAppropriationActivity

            For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityList
               If _aaNew.LogActionId = LogActionId.Add Then
                  _bgsAppropriationActivity = New BgsAppropriationActivity
                  _bgsAppropriationActivityListNew.Add(_bgsAppropriationActivity)
                  DataLib.ScatterValues(_aaNew, _bgsAppropriationActivity)

                  _bgsAppropriationActivity.ApproActId = _approActId
                  _approActId = _approActId + 1
               End If
            Next

            'Me.InsertBgsAppropriationActivities(_bgsAppropriationActivityListNew, _approActId)
         End If

         Dim _isBgsAppropriationChanged As Boolean = Me.HasBgsAppropriationChanges(_bgsAppropriationOld, _bgsAppropriation)

         If Not _isBgsAppropriationChanged AndAlso _addApproActivityCount = 0 AndAlso _removeApproActivityCount = 0 AndAlso _editApproActivityCount = 0 Then
            '
            ' No changes; just return current appropriation
            ' 
            Return Me.GetAppropriation(appropriationId)
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

            If _isBgsAppropriationChanged Then
               Me.UpdateBgsAppropriation(_bgsAppropriation)

               With _logKeyList
                  .Clear()
                  .Add("AppropriationId", _bgsAppropriation.AppropriationId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAppropriationLogDetailList)

               AppLib.CreateLogDetails(_bgsAppropriationLogDetailList, "BgsAppropriationLogDetail")

            End If

            '
            ' BgsAppropriationActivity
            '
            If _removeApproActivityCount > 0 Then
               Me.DeleteBgsAppropriationActivities(_bgsAppropriationActivityListOld)

               For Each _aaOld As BgsAppropriationActivity In _bgsAppropriationActivityListOld
                  If _aaOld.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("AppropriationId", _aaOld.AppropriationId)
                        .Add("ActivityId", _aaOld.ActivityId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsAppropriationActivityLog", _logKeyList, LogActionId.Delete, _currentUserId)

                     _bgsAppropriationActivityLogDetailList.Clear()

                     With _aaOld
                        If .PSRAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.PSRAmount, .PSRAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .RLIPAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.RLIPAmount, .RLIPAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .MOOEAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.MOOEAmount, .MOOEAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .FEAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.FEAmount, .FEAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                        If .COAmount > 0 Then
                           AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.COAmount, .COAmount.ToString("N"), "", "ActivityId=" + .ActivityId.ToString)
                        End If
                     End With

                     AppLib.CreateLogDetails(_bgsAppropriationActivityLogDetailList, "BgsAppropriationActivityLogDetail")

                  End If
               Next

            End If

            If _addApproActivityCount > 0 Then
               'Me.InsertBgsAppropriationActivities(_bgsAppropriationActivityListNew, _approActId)
               Me.InsertBgsAppropriationActivities(_bgsAppropriationActivityListNew)

               For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityListNew

                  With _logKeyList
                     .Clear()
                     .Add("AppropriationId", _aaNew.AppropriationId)
                     .Add("ActivityId", _aaNew.ActivityId)
                  End With

                  _id = AppLib.CreateLogHeader("InsBgsAppropriationActivityLog", _logKeyList, LogActionId.Add, _currentUserId)

                  _bgsAppropriationActivityLogDetailList.Clear()

                  With _aaNew
                     If .PSRAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.PSRAmount, "", .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .RLIPAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.RLIPAmount, "", .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .MOOEAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.MOOEAmount, "", .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .FEAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.FEAmount, "", .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If
                     If .COAmount > 0 Then
                        AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.COAmount, "", .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                     End If

                     AppLib.CreateLogDetails(_bgsAppropriationActivityLogDetailList, "BgsAppropriationActivityLogDetail")

                  End With
               Next

            End If

            If _editApproActivityCount > 0 Then
               Me.UpdateBgsAppropriationActivities(_bgsAppropriationActivityList)

               For Each _aaNew As BgsAppropriationActivity In _bgsAppropriationActivityList
                  If _aaNew.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("AppropriationId", _aaNew.AppropriationId)
                        .Add("ActivityId", _aaNew.ActivityId)
                     End With

                     _id = AppLib.CreateLogHeader("InsBgsAppropriationActivityLog", _logKeyList, LogActionId.Edit, _currentUserId)

                     _bgsAppropriationActivityLogDetailList.Clear()

                     For Each _aaOld As BgsAppropriationActivity In _bgsAppropriationActivityListOld
                        If _aaNew.ApproActId = _aaOld.ApproActId Then
                           With _aaNew
                              If .PSRAmount <> _aaOld.PSRAmount Then
                                 AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.PSRAmount, _aaOld.PSRAmount.ToString("N"), .PSRAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .RLIPAmount <> _aaOld.RLIPAmount Then
                                 AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.RLIPAmount, _aaOld.RLIPAmount.ToString("N"), .RLIPAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .MOOEAmount <> _aaOld.MOOEAmount Then
                                 AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.MOOEAmount, _aaOld.MOOEAmount.ToString("N"), .MOOEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .FEAmount <> _aaOld.FEAmount Then
                                 AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.FEAmount, _aaOld.FEAmount.ToString("N"), .FEAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If
                              If .COAmount <> _aaOld.COAmount Then
                                 AppLib.AddLogDetail(_bgsAppropriationActivityLogDetailList, _id, LogColumnId.COAmount, _aaOld.COAmount.ToString("N"), .COAmount.ToString("N"), "ActivityId=" + .ActivityId.ToString)
                              End If

                              AppLib.CreateLogDetails(_bgsAppropriationActivityLogDetailList, "BgsAppropriationActivityLogDetail")

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

   <SymAuthorization("RemoveAppropriation")>
   <Route("appropriations/{appropriationId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveAppropriation(appropriationId As Integer, lockId As String) As IHttpActionResult

      If appropriationId <= 0 Then
         Throw New ArgumentException("Appropriation ID is required.")
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

            Me.DeleteBgsAppropriation(appropriationId, lockId)

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

   Private Sub LoadBgsAppropriation(appro As AppropriationBody, bgsAppropriation As BgsAppropriation)

      DataLib.ScatterValues(appro, bgsAppropriation)

   End Sub

   Private Sub LoadBgsAppropriation(row As DataRow, bgsAppropriation As BgsAppropriation)

      With bgsAppropriation
         .AppropriationId = row.ToInt32("AppropriationId")
         .AppropriationName = row.ToString("AppropriationName")
         .PostDate = row.ToDate("PostDate")
         .ReferenceDate = row.ToDate("ReferenceDate")
         .ReferenceDocument = row.ToString("ReferenceDocument")
         .BudgetClassId = row.ToInt32("BudgetClassId")
      End With

   End Sub

   Private Sub LoadBgsAppropriationActivityList(rows As DataRowCollection, list As BgsAppropriationActivityList)

      Dim _bgsAppropriationActivity As BgsAppropriationActivity
      For Each _row As DataRow In rows
         _bgsAppropriationActivity = New BgsAppropriationActivity

         With _bgsAppropriationActivity
            .ApproActId = _row.ToInt32("ApproActId")
            .AppropriationId = _row.ToInt32("AppropriationId")
            .ActivityId = _row.ToInt32("ActivityId")
            .PSRAmount = _row.ToDecimal("PSRAmount")
            .RLIPAmount = _row.ToDecimal("RLIPAmount")
            .MOOEAmount = _row.ToDecimal("MOOEAmount")
            .FEAmount = _row.ToDecimal("FEAmount")
            .COAmount = _row.ToDecimal("COAmount")
         End With

         list.Add(_bgsAppropriationActivity)

      Next

   End Sub

   Private Sub InsertBgsAppropriation(appro As BgsAppropriation)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAppropriation", appro, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsAppropriation(appro)

         .ExecuteNonQuery()

      End With

   End Sub

   'Private Sub InsertBgsAppropriationActivities(list As BgsAppropriationActivityList, startIdSequence As Integer)

   '   With DataCore.Command
   '      .CommandText = "INSERT INTO dbo.BgsAppropriationActivity (ApproActId, AppropriationId, ActivityId, PSRAmount, RLIPAmount, MOOEAmount, FEAmount, COAmount) " +
   '                     "VALUES (@ApproActId, @AppropriationId, @ActivityId, @PSRAmount, @RLIPAmount, @MOOEAmount, @FEAmount, @COAmount)"

   '      .CommandType = CommandType.Text

   '      Dim _approActId As Integer = startIdSequence

   '      For Each _approActivity As BgsAppropriationActivity In list
   '         With .Parameters
   '            .Clear()

   '            _approActivity.ApproActId = _approActId

   '            .AddWithValue("@ApproActId", _approActivity.ApproActId)
   '            .AddWithValue("@AppropriationId", _approActivity.AppropriationId)
   '            .AddWithValue("@ActivityId", _approActivity.ActivityId)
   '            .AddWithValue("@PSRAmount", _approActivity.PSRAmount)
   '            .AddWithValue("@RLIPAmount", _approActivity.RLIPAmount)
   '            .AddWithValue("@MOOEAmount", _approActivity.MOOEAmount)
   '            .AddWithValue("@FEAmount", _approActivity.FEAmount)
   '            .AddWithValue("@COAmount", _approActivity.COAmount)

   '            _approActId = _approActId + 1
   '         End With

   '         DataCore.Command.ExecuteNonQuery()
   '      Next

   '   End With

   'End Sub

   'Private Sub InsertBgsAppropriationActivities(list As BgsAppropriationActivityList, startIdSequence As Integer)

   '   Dim _excludedFields As New List(Of String)

   '   With _excludedFields
   '      .Add("LogActionId")
   '      .Add("LockId")
   '   End With

   '   With DataCore.Command
   '      '.CommandText = "INSERT INTO dbo.BgsAppropriationActivity (ApproActId, AppropriationId, ActivityId, PSRAmount, RLIPAmount, MOOEAmount, FEAmount, COAmount) " +
   '      '               "VALUES (@ApproActId, @AppropriationId, @ActivityId, @PSRAmount, @RLIPAmount, @MOOEAmount, @FEAmount, @COAmount)"

   '      .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAppropriationActivity", list(0), _excludedFields)
   '      .CommandType = CommandType.Text

   '      Dim _approActId As Integer = startIdSequence

   '      For Each _approActivity As BgsAppropriationActivity In list
   '         _approActivity.ApproActId = _approActId
   '         Me.AddInsertUpdateParamsBgsAppropriationActivity(_approActivity)
   '         _approActId = _approActId + 1

   '         'With .Parameters
   '         '   .Clear()

   '         '   _approActivity.ApproActId = _approActId

   '         '   .AddWithValue("@ApproActId", _approActivity.ApproActId)
   '         '   .AddWithValue("@AppropriationId", _approActivity.AppropriationId)
   '         '   .AddWithValue("@ActivityId", _approActivity.ActivityId)
   '         '   .AddWithValue("@PSRAmount", _approActivity.PSRAmount)
   '         '   .AddWithValue("@RLIPAmount", _approActivity.RLIPAmount)
   '         '   .AddWithValue("@MOOEAmount", _approActivity.MOOEAmount)
   '         '   .AddWithValue("@FEAmount", _approActivity.FEAmount)
   '         '   .AddWithValue("@COAmount", _approActivity.COAmount)

   '         '   _approActId = _approActId + 1
   '         'End With

   '         DataCore.Command.ExecuteNonQuery()
   '      Next

   '   End With

   'End Sub

   Private Sub InsertBgsAppropriationActivities(list As BgsAppropriationActivityList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAppropriationActivity", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _approActivity As BgsAppropriationActivity In list
            Me.AddInsertUpdateParamsBgsAppropriationActivity(_approActivity)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateBgsAppropriation(appro As BgsAppropriation)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AppropriationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAppropriation", appro, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsBgsAppropriation(appro)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(appro.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsAppropriationActivities(list As BgsAppropriationActivityList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ApproActId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAppropriationActivity", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _approActivity As BgsAppropriationActivity In list
            If _approActivity.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsBgsAppropriationActivity(_approActivity)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsAppropriation(appro As BgsAppropriation)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AppropriationId", appro.AppropriationId)
         .AddWithValue("@AppropriationName", appro.AppropriationName)
         .AddWithValue("@PostDate", appro.PostDate)
         .AddWithValue("@ReferenceDate", appro.ReferenceDate)
         .AddWithValue("@ReferenceDocument", appro.ReferenceDocument)
         .AddWithValue("@BudgetClassId", appro.BudgetClassId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsBgsAppropriationActivity(approActivity As BgsAppropriationActivity)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApproActId", approActivity.ApproActId)
         .AddWithValue("@AppropriationId", approActivity.AppropriationId)
         .AddWithValue("@ActivityId", approActivity.ActivityId)
         .AddWithValue("@PSRAmount", approActivity.PSRAmount)
         .AddWithValue("@RLIPAmount", approActivity.RLIPAmount)
         .AddWithValue("@MOOEAmount", approActivity.MOOEAmount)
         .AddWithValue("@FEAmount", approActivity.FEAmount)
         .AddWithValue("@COAmount", approActivity.COAmount)
      End With

   End Sub

   Private Sub DeleteBgsAppropriation(appropriationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AppropriationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsAppropriation", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AppropriationId", appropriationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteBgsAppropriationActivities(list As BgsAppropriationActivityList)

      With DataCore.Command
         .CommandText = "DELETE dbo.BgsAppropriationActivity WHERE ApproActId=@ApproActId"
         .CommandType = CommandType.Text

         For Each _aaOld As BgsAppropriationActivity In list
            If _aaOld.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ApproActId", _aaOld.ApproActId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub


   'Private Sub DeleteSecUserRoles(userId As Integer)
   'Private Sub DeleteBgsAppropriationActivities(appropriationId As Integer)

   '   With DataCore.Command
   '      .CommandText = "DELETE dbo.BgsAppropriationActivity WHERE AppropriationId=@AppropriationId"
   '      .CommandType = CommandType.Text

   '      With .Parameters
   '         .Clear()
   '         .AddWithValue("@AppropriationId", appropriationId)
   '      End With

   '      .ExecuteNonQuery()

   '   End With

   'End Sub

   Private Function HasBgsAppropriationChanges(oldRecord As BgsAppropriation, newRecord As BgsAppropriation) As Boolean

      With oldRecord
         If .AppropriationName <> newRecord.AppropriationName Then Return True
         If .PostDate <> newRecord.PostDate Then Return True
         If .ReferenceDate <> newRecord.ReferenceDate Then Return True
         If .ReferenceDocument <> newRecord.ReferenceDocument Then Return True
         If .BudgetClassId <> newRecord.BudgetClassId Then Return True
      End With

      Return False

   End Function

End Class

Public Class AppropriationBody
   Inherits BgsAppropriation

   Public Property Activities As BgsAppropriationActivity()

End Class
