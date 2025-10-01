<RoutePrefix("api")>
Public Class BGS0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAppropriationDetail")>
   <Route("appro-details/{approDetailId}")>
   <HttpGet>
   Public Function GetAppropriationDetail(approDetailId As Integer) As IHttpActionResult

      If approDetailId <= 0 Then
         Throw New ArgumentException("Appropriation Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0020")
            With _direct
               .AddParameter("ApproDetailId", approDetailId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetAppropriationLog")>
   <Route("appro-details/{approDetailId}/log")>
   <HttpGet>
   Public Function GetAppropriationDetailLog(approDetailId As Integer) As IHttpActionResult

      If approDetailId <= 0 Then
         Throw New ArgumentException("Appropriation Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.BGS0020_Log")
            With _direct
               .AddParameter("ApproDetailId", approDetailId)

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

   <SymAuthorization("CreateAppropriationDetail")>
   <Route("appro-details/{currentUserId}")>
   <HttpPost>
   Public Function CreateAppropriationDetail(currentUserId As Integer, <FromBody> approDetail As AppropriationDetailBody) As IHttpActionResult

      If approDetail.ApproDetailId <> -1 Then
         Throw New ArgumentException("Appropriation Detail ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _approDetailId As Integer = SysLib.GetNextSequence("ApproDetailId")

         approDetail.ApproDetailId = _approDetailId

         '
         ' Load proposed values from payload
         '
         Dim _bgsAppropriationDetail As New BgsAppropriationDetail

         Me.LoadBgsAppropriationDetail(approDetail, _bgsAppropriationDetail)

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAppropriationDetailLogDetailList As New SysLogDetailList

         With _bgsAppropriationDetail
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.AppropriationName, String.Empty, .AppropriationName)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.ActivityName, String.Empty, .ActivityName)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.CostCenterName, String.Empty, .CostCenterName)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.AccountName, String.Empty, .AccountName, "AccountId=" + .AccountId)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.Amount, String.Empty, .Amount.ToString("N"))
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

            Me.InsertBgsAppropriationDetail(_bgsAppropriationDetail)

            If _bgsAppropriationDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ApproDetailId", _bgsAppropriationDetail.ApproDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationDetailLog", _logKeyList, LogActionId.Add, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAppropriationDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAppropriationDetailLogDetailList, "BgsAppropriationDetailLogDetail")

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
         'Return Me.GetAppropriationDetail(approDetail.ApproDetailId)
         Return Me.Ok(approDetail.ApproDetailId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAppropriationDetail")>
   <Route("appro-details/{approDetailId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAppropriationDetail(approDetailId As Integer, currentUserId As Integer, <FromBody> approDetail As AppropriationDetailBody) As IHttpActionResult

      If approDetailId <= 0 Then
         Throw New ArgumentException("Appropriation Detail ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _bgsAppropriationDetail As New BgsAppropriationDetail

         Me.LoadBgsAppropriationDetail(approDetail, _bgsAppropriationDetail)

         '
         ' Load old values from DB
         '
         Dim _bgsAppropriationDetailOld As New BgsAppropriationDetail
         '
         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAppropriationDetail(approDetailId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsAppropriationDetail(_row, _bgsAppropriationDetailOld)
         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAppropriationDetailLogDetailList As New SysLogDetailList

         With _bgsAppropriationDetailOld
            If .Amount <> _bgsAppropriationDetail.Amount Then
               AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.Amount, .Amount.ToString("N"), _bgsAppropriationDetail.Amount.ToString("N"))
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

            Me.UpdateBgsAppropriationDetail(_bgsAppropriationDetail)

            If _bgsAppropriationDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ApproDetailId", _bgsAppropriationDetail.ApproDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationDetailLog", _logKeyList, LogActionId.Edit, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAppropriationDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAppropriationDetailLogDetailList, "BgsAppropriationDetailLogDetail")

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

   <SymAuthorization("RemoveAppropriationDetail")>
   <Route("appro-details/{approDetailId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveAppropriationDetail(approDetailId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If approDetailId <= 0 Then
         Throw New ArgumentException("Appropriation Detail ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Check for existing record
         '
         Dim _filter As String = "ApproDetailId=" + approDetailId.ToString
         If DataLib.GetRecordCount("dbo.BgsAppropriationDetail", _filter, _databaseId) = 0 Then
            Throw New ArgumentException("Appropriation Detail ID not found.")
         End If

         '
         ' Load current values from DB
         '
         Dim _bgsAppropriationDetail As New BgsAppropriationDetail

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAppropriationDetail(approDetailId), Results.OkNegotiatedContentResult(Of DataTable))

         Using _dataTable As DataTable = _result.Content
            Dim _row As DataRow = _dataTable.Rows(0)
            Me.LoadBgsAppropriationDetail(_row, _bgsAppropriationDetail)
         End Using

         '
         ' Log deletion, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         Dim _bgsAppropriationDetailLogDetailList As New SysLogDetailList

         With _bgsAppropriationDetail
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.AppropriationName, .AppropriationName, String.Empty)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.ActivityName, .ActivityName, String.Empty)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.CostCenterName, .CostCenterName, String.Empty)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.AccountName, .AccountName, String.Empty, "AccountId=" + .AccountId)
            AppLib.AddLogDetail(_bgsAppropriationDetailLogDetailList, 0, LogColumnId.Amount, .Amount.ToString("N"), String.Empty)
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

            Me.DeleteBgsAppropriationDetail(approDetailId, lockId)

            If _bgsAppropriationDetailLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ApproDetailId", _bgsAppropriationDetail.ApproDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsBgsAppropriationDetailLog", _logKeyList, LogActionId.Delete, _currentUserId)
               AppLib.AssignLogHeaderId(_id, _bgsAppropriationDetailLogDetailList)

               AppLib.CreateLogDetails(_bgsAppropriationDetailLogDetailList, "BgsAppropriationDetailLogDetail")

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

   Private Sub LoadBgsAppropriationDetail(approDetail As AppropriationDetailBody, bgsAppropriationDetail As BgsAppropriationDetail)

      DataLib.ScatterValues(approDetail, bgsAppropriationDetail)

   End Sub

   Private Sub LoadBgsAppropriationDetail(row As DataRow, bgsAppropriationDetail As BgsAppropriationDetail)

      With bgsAppropriationDetail
         .ApproDetailId = row.ToInt32("ApproDetailId")
         .ApproActId = row.ToInt32("ApproActId")
         .CostCenterId = row.ToInt32("CostCenterId")
         .AccountId = row.ToString("AccountId")
         .Amount = row.ToDecimal("Amount")
         .AppropriationName = row.ToString("AppropriationName")
         .ActivityName = row.ToString("ActivityName")
         .CostCenterName = row.ToString("CostCenterName")
         .AccountName = row.ToString("AccountName")
      End With

   End Sub

   Private Sub InsertBgsAppropriationDetail(approDetail As BgsAppropriationDetail)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("AppropriationName")
         .Add("ActivityName")
         .Add("CostCenterName")
         .Add("AccountName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.BgsAppropriationDetail", approDetail, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertParamsBgsAppropriationDetail(approDetail)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateBgsAppropriationDetail(approDetail As BgsAppropriationDetail)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ApproDetailId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("ApproActId")
         .Add("CostCenterId")
         .Add("AccountId")
         .Add("AppropriationName")
         .Add("ActivityName")
         .Add("CostCenterName")
         .Add("AccountName")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.BgsAppropriationDetail", approDetail, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApproDetailId", approDetail.ApproDetailId)
            .AddWithValue("@Amount", approDetail.Amount)
            .AddWithValue("@LockId", Convert.FromBase64String(approDetail.LockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertParamsBgsAppropriationDetail(approDetail As BgsAppropriationDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApproDetailId", approDetail.ApproDetailId)
         .AddWithValue("@ApproActId", approDetail.ApproActId)
         .AddWithValue("@CostCenterId", approDetail.CostCenterId)
         .AddWithValue("@AccountId", approDetail.AccountId)
         .AddWithValue("@Amount", approDetail.Amount)
      End With

   End Sub

   Private Sub DeleteBgsAppropriationDetail(approDetailId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApproDetailId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.BgsAppropriationDetail", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApproDetailId", approDetailId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class AppropriationDetailBody
   Inherits BgsAppropriationDetail

End Class

