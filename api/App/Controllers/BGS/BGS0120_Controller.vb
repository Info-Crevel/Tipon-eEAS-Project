<RoutePrefix("api")>
Public Class BGS0120_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAllotmentDetail")>
   <Route("allot-details/{allotDetailId}")>
   <HttpGet>
   Public Function GetAllotmentDetail(allotDetailId As Integer) As IHttpActionResult

      If allotDetailId <= 0 Then
         Throw New ArgumentException("Allotment Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0120")
            With _direct
               .AddParameter("AllotDetailId", allotDetailId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAllotmentDetailLog")>
   <Route("allot-details/{allotDetailId}/log")>
   <HttpGet>
   Public Function GetAllotmentDetailLog(allotDetailId As Integer) As IHttpActionResult

      If allotDetailId <= 0 Then
         Throw New ArgumentException("Allotment Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0120_Log")
            With _direct
               .AddParameter("AllotDetailId", allotDetailId)

               ' Allow DateTime
               Dim _settings As New JsonSerializerSettings
               With _settings
                  .ContractResolver = New CamelCasePropertyNamesContractResolver
                  .DateFormatString = "yyyy-MM-ddTHH:mm"
               End With

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Json(_dataTable, _settings)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAllotmentDetail")>
   <Route("allot-details/{currentUserId}")>
   <HttpPost>
   Public Function CreateAllotmentDetail(currentUserId As Integer, <FromBody> allotDetail As AllotmentDetailBody) As IHttpActionResult

      If allotDetail.AllotDetailId <> -1 Then
         Throw New ArgumentException("Allotment Detail ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _allotDetailId As Integer = SysLib.GetNextSequence("AllotDetailId")

         allotDetail.AllotDetailId = _allotDetailId

         '
         ' Load proposed values from payload
         '
         Dim _bgsAllotmentDetail As New BgsAllotmentDetail

         Me.LoadBgsAllotmentDetail(allotDetail, _bgsAllotmentDetail)

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAllotmentDetailLogDetailList As New SysLogDetailList

         With _bgsAllotmentDetail
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.AllotmentName, String.Empty, .AllotmentName)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.ActivityName, String.Empty, .ActivityName)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.CostCenterName, String.Empty, .CostCenterName)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.AccountName, String.Empty, .AccountName, "AccountId=" + .AccountId)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.Amount, String.Empty, .Amount.ToString("N"))
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

            Me.InsertBgsAllotmentDetail(_bgsAllotmentDetail)

            If _bgsAllotmentDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AllotDetailId", _bgsAllotmentDetail.AllotDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentDetailLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAllotmentDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAllotmentDetailLogDetailList, "BgsAllotmentDetailLogDetail")

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
         Return Me.Ok(allotDetail.AllotDetailId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAllotmentDetail")>
   <Route("allot-details/{allotDetailId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAllotmentDetail(allotDetailId As Integer, currentUserId As Integer, <FromBody> allotDetail As AllotmentDetailBody) As IHttpActionResult

      If allotDetailId <= 0 Then
         Throw New ArgumentException("Allotment Detail ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsAllotmentDetail As New BgsAllotmentDetail

         Me.LoadBgsAllotmentDetail(allotDetail, _bgsAllotmentDetail)

         '
         ' Load old values from DB
         '
         Dim _bgsAllotmentDetailOld As New BgsAllotmentDetail
         '
         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAllotmentDetail(allotDetailId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsAllotmentDetail(_row, _bgsAllotmentDetailOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAllotmentDetailLogDetailList As New SysLogDetailList

         With _bgsAllotmentDetailOld
            If .Amount <> _bgsAllotmentDetail.Amount Then
               AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.Amount, .Amount.ToString("N"), _bgsAllotmentDetail.Amount.ToString("N"))
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

            Me.UpdateBgsAllotmentDetail(_bgsAllotmentDetail)

            If _bgsAllotmentDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AllotDetailId", _bgsAllotmentDetail.AllotDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAllotmentDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAllotmentDetailLogDetailList, "BgsAllotmentDetailLogDetail")

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

   <SymAuthorization("RemoveAllotmentDetail")>
   <Route("allot-details/{allotDetailId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveAllotmentDetail(allotDetailId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If allotDetailId <= 0 Then
         Throw New ArgumentException("Allotment Detail ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Check for existing record
         '
         Dim _filter As String = "AllotDetailId=" + allotDetailId.ToString
         If DataLib.GetRecordCount("dbo.BgsAllotmentDetail", _filter, _databaseId) = 0 Then
            Throw New ArgumentException("Allotment Detail ID not found.")
         End If

         '
         ' Load current values from DB
         '
         Dim _bgsAllotmentDetail As New BgsAllotmentDetail

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAllotmentDetail(allotDetailId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsAllotmentDetail(_row, _bgsAllotmentDetail)
         End Using

         '
         ' Log deletion, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAllotmentDetailLogDetailList As New SysLogDetailList

         With _bgsAllotmentDetail
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.AllotmentName, .AllotmentName, String.Empty)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.ActivityName, .ActivityName, String.Empty)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.CostCenterName, .CostCenterName, String.Empty)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.AccountName, .AccountName, String.Empty, "AccountId=" + .AccountId)
            AppLib.AddLogDetail(_bgsAllotmentDetailLogDetailList, 0, LogColumnId.Amount, .Amount.ToString("N"), String.Empty)
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

            Me.DeleteBgsAllotmentDetail(allotDetailId, lockId)

            If _bgsAllotmentDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("AllotDetailId", _bgsAllotmentDetail.AllotDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAllotmentDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAllotmentDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAllotmentDetailLogDetailList, "BgsAllotmentDetailLogDetail")

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

   Private Sub LoadBgsAllotmentDetail(allotDetail As AllotmentDetailBody, bgsAllotmentDetail As BgsAllotmentDetail)

      DataLib.ScatterValues(allotDetail, bgsAllotmentDetail)

   End Sub

   Private Sub LoadBgsAllotmentDetail(row As DataRow, bgsAllotmentDetail As BgsAllotmentDetail)

      With bgsAllotmentDetail
         .AllotDetailId = row.ToInt32("AllotDetailId")
         .AllotActId = row.ToInt32("AllotActId")
         .CostCenterId = row.ToInt32("CostCenterId")
         .AccountId = row.ToString("AccountId")
         .Amount = row.ToDecimal("Amount")
         .AllotmentName = row.ToString("AllotmentName")
         .ActivityName = row.ToString("ActivityName")
         .CostCenterName = row.ToString("CostCenterName")
         .AccountName = row.ToString("AccountName")
      End With

   End Sub

   Private Sub InsertBgsAllotmentDetail(allotDetail As BgsAllotmentDetail)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("AllotmentName")
         .Add("ActivityName")
         .Add("CostCenterName")
         .Add("AccountName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAllotmentDetail", allotDetail, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertParamsBgsAllotmentDetail(allotDetail)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsAllotmentDetail(allotDetail As BgsAllotmentDetail)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("AllotDetailId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("AllotActId")
         .Add("CostCenterId")
         .Add("AccountId")
         .Add("AllotmentName")
         .Add("ActivityName")
         .Add("CostCenterName")
         .Add("AccountName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAllotmentDetail", allotDetail, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AllotDetailId", allotDetail.AllotDetailId)
            .AddWithValue("@Amount", allotDetail.Amount)
            .AddWithValue("@LockId", Convert.FromBase64String(allotDetail.LockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertParamsBgsAllotmentDetail(allotDetail As BgsAllotmentDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AllotDetailId", allotDetail.AllotDetailId)
         .AddWithValue("@AllotActId", allotDetail.AllotActId)
         .AddWithValue("@CostCenterId", allotDetail.CostCenterId)
         .AddWithValue("@AccountId", allotDetail.AccountId)
         .AddWithValue("@Amount", allotDetail.Amount)
      End With

   End Sub

   Private Sub DeleteBgsAllotmentDetail(allotDetailId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AllotDetailId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsAllotmentDetail", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AllotDetailId", allotDetailId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class AllotmentDetailBody
   Inherits BgsAllotmentDetail

End Class

