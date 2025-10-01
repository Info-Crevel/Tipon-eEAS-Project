<RoutePrefix("api")>
Public Class APS0600_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_APS0600")>
   <Route("references/aps0600")>
   <HttpGet>
   Public Function GetReferences_APS0600() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0600_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "orgs"
                     .Tables(1).TableName = "platforms"
                     .Tables(2).TableName = "requestTypes"
                     .Tables(3).TableName = "particulars"
                     .Tables(4).TableName = "clusters"
                     .Tables(5).TableName = "payGroups"
                     .Tables(6).TableName = "docTypes"
                     .Tables(7).TableName = "requestTrxs"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetApsRequestTrx")>
   <Route("aps-request-trxs/{requestTrxId}")>
   <HttpGet>
   Public Function GetApsRequestTrx(requestTrxId As Integer) As IHttpActionResult

      If requestTrxId <= 0 Then
         Throw New ArgumentException("Request Trx ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0600")
            With _direct
               .AddParameter("requestTrxId", requestTrxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "request"
                     .Tables(1).TableName = "details"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApsRequestTrx")>
   <Route("aps-request-trxs/{currentUserId}")>
   <HttpPost>
   Public Function CreateApsRequestTrx(currentUserId As Integer, <FromBody> request As ApsRequestTrxBody) As IHttpActionResult

      If request.RequestTrxId <> -1 Then
         Throw New ArgumentException("Request Trx ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _requestTrxId As Integer = SysLib.GetNextSequence("RequestTrxId")

         request.RequestTrxId = _requestTrxId

         '
         ' Load proposed values from payload
         '
         Dim _apsRequestTrx As New ApsRequestTrx
         Dim _apsRequestTrxDetailList As New ApsRequestTrxDetailList

         Me.LoadApsRequestTrx(request, _apsRequestTrx)

         For Each _detail As ApsRequestTrxDetail In request.Details
            _detail.RequestTrxId = _requestTrxId
            _apsRequestTrxDetailList.Add(_detail)
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

            Me.InsertApsRequestTrx(_apsRequestTrx)

            If _apsRequestTrxDetailList.Count > 0 Then
               Me.InsertApsRequestTrxDetails(_apsRequestTrxDetailList)
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
         Return Me.Ok(request.RequestTrxId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyApsRequestTrx")>
   <Route("aps-request-trxs/{requestTrxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCluster(requestTrxId As Integer, currentUserId As Integer, <FromBody> request As ApsRequestTrxBody) As IHttpActionResult

      If requestTrxId <= 0 Then
         Throw New ArgumentException("Request Trx ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _apsRequestTrx As New ApsRequestTrx
         Dim _apsRequestTrxDetailList As New ApsRequestTrxDetailList

         Me.LoadApsRequestTrx(request, _apsRequestTrx)

         For Each _detail As ApsRequestTrxDetail In request.Details
            _apsRequestTrxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _apsRequestTrxOld As New ApsRequestTrx
         Dim _apsRequestTrxDetailListOld As New ApsRequestTrxDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetApsRequestTrx(requestTrxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("request").Rows(0)
            Me.LoadApsRequestTrx(_row, _apsRequestTrxOld)
            Me.LoadApsRequestTrxDetailList(_dataSet.Tables("details").Rows, _apsRequestTrxDetailListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ApsRequestTrxDetail In _apsRequestTrxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ApsRequestTrxDetail In _apsRequestTrxDetailList
               If _new.RequestTrxDetailId = _old.RequestTrxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ApsRequestTrxDetail In _apsRequestTrxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As ApsRequestTrxDetail In _apsRequestTrxDetailListOld
               If _new.RequestTrxDetailId = _old.RequestTrxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .OrgTrxId <> _old.OrgTrxId OrElse .OrgId <> _old.OrgId OrElse .PlatformId <> _old.PlatformId OrElse .ClusterId <> _old.ClusterId OrElse .ClientPayGroupId <> _old.ClientPayGroupId OrElse .ApsDocTypeId <> _old.ApsDocTypeId OrElse .Amount <> _old.Amount Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With


               Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addDetailCount = _addDetailCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editDetailCount = _editDetailCount + 1
            End If

         Next

         Dim _apsRequestTrxDetailListNew As New ApsRequestTrxDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _apsRequestTrxDetail As ApsRequestTrxDetail

            For Each _new As ApsRequestTrxDetail In _apsRequestTrxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _apsRequestTrxDetail = New ApsRequestTrxDetail
                  _apsRequestTrxDetailListNew.Add(_apsRequestTrxDetail)
                  DataLib.ScatterValues(_new, _apsRequestTrxDetail)
                  _apsRequestTrxDetail.RequestTrxId = _apsRequestTrx.RequestTrxId
               End If
            Next

         End If


         Dim _isApsRequestTrxChanged As Boolean = Me.HasApsRequestTrxChanges(_apsRequestTrxOld, _apsRequestTrx)

         If Not _isApsRequestTrxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetApsRequestTrx(requestTrxId)
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

            If _isApsRequestTrxChanged Then
               Me.UpdateApsRequestTrx(_apsRequestTrx)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteApsRequestTrxDetails(_apsRequestTrxDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertApsRequestTrxDetails(_apsRequestTrxDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateApsRequestTrxDetails(_apsRequestTrxDetailList)

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

   <SymAuthorization("RemoveApsRequestTrx")>
   <Route("aps-request-trxs/{requestTrxId}")>
   <HttpDelete>
   Public Function RemoveApsRequestTrx(requestTrxId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If requestTrxId <= 0 Then
         Throw New ArgumentException("Request Trx ID is required.")
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

            Me.DeleteApsRequestTrx(requestTrxId, q.LockId)

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

   Private Sub LoadApsRequestTrx(request As ApsRequestTrxBody, apsRequestTrx As ApsRequestTrx)

      DataLib.ScatterValues(request, apsRequestTrx)

   End Sub

   Private Sub LoadApsRequestTrx(row As DataRow, request As ApsRequestTrx)

      With request
         .RequestTrxId = row.ToInt32("RequestTrxId")
         .TrxDate = row.ToDate("TrxDate")
         .MemberId = row.ToInt32("MemberId")
         .ReviewerId = row.ToInt32("ReviewerId")
         .RequestStatusId = row.ToInt32("RequestStatusId")
         .ApproverId = row.ToInt32("ApproverId")
         .ApsRequestTypeId = row.ToInt32("ApsRequestTypeId")
         .ParticularsId = row.ToInt32("ParticularsId")
         .Particulars = row.ToString("Particulars")
      End With

   End Sub

   Private Sub LoadApsRequestTrxDetailList(rows As DataRowCollection, list As ApsRequestTrxDetailList)

      Dim _detail As ApsRequestTrxDetail
      For Each _row As DataRow In rows
         _detail = New ApsRequestTrxDetail

         With _detail
            .RequestTrxDetailId = _row.ToInt32("RequestTrxDetailId")
            .RequestTrxId = _row.ToInt32("RequestTrxId")
            .OrgTrxId = _row.ToInt32("OrgTrxId")
            .OrgId = _row.ToInt32("OrgId")
            .PlatformId = _row.ToInt32("PlatformId")
            .ClusterId = _row.ToInt32("ClusterId")
            .ClientPayGroupId = _row.ToInt32("ClientPayGroupId")
            .ApsDocTypeId = _row.ToInt32("ApsDocTypeId")
            .ApsDocTypeId = _row.ToInt32("ApsDocTypeId")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertApsRequestTrx(request As ApsRequestTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsRequestTrx", request, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTrx(request)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertApsRequestTrxDetails(list As ApsRequestTrxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("RequestTrxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsRequestTrxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ApsRequestTrxDetail In list
            Me.AddInsertUpdateParamsApsRequestTrxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateApsRequestTrx(request As ApsRequestTrx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("RequestTrxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsRequestTrx", request, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTrx(request)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(request.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateApsRequestTrxDetails(list As ApsRequestTrxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("RequestTrxDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsRequestTrxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ApsRequestTrxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsApsRequestTrxDetail(_detail)
               .Parameters.AddWithValue("@RequestTrxDetailId", _detail.RequestTrxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub
   Private Sub AddInsertUpdateParamsApsRequestTrx(request As ApsRequestTrx)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@RequestTrxId", request.RequestTrxId)
         .AddWithValue("@TrxDate", request.TrxDate)
         .AddWithValue("@MemberId", request.MemberId)
         .AddWithValue("@ReviewerId", request.ReviewerId)
         .AddWithValue("@RequestStatusId", request.RequestStatusId)
         .AddWithValue("@ApproverId", request.ApproverId)
         .AddWithValue("@ApsRequestTypeId", request.ApsRequestTypeId)
         .AddWithValue("@ParticularsId", request.ParticularsId)
         .AddWithValue("@Particulars", request.Particulars)


      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsRequestTrxDetail(dtl As ApsRequestTrxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@RequestTrxId", dtl.RequestTrxId)
         .AddWithValue("@OrgTrxId", dtl.OrgTrxId)
         .AddWithValue("@OrgId", dtl.OrgId)
         .AddWithValue("@PlatformID", dtl.PlatformId)
         .AddWithValue("@ClusterId", dtl.ClusterId)
         .AddWithValue("@ClientPayGroupId", dtl.ClientPayGroupId)
         .AddWithValue("@ApsDocTypeId", dtl.ApsDocTypeId)
         .AddWithValue("@Amount", dtl.Amount)

      End With

   End Sub
   Private Sub DeleteApsRequestTrx(requestTrxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RequestTrxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsRequestTrx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RequestTrxId", requestTrxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteApsRequestTrxDetails(list As ApsRequestTrxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ApsRequestTrxDetail WHERE RequestTrxDetailId=@RequestTrxDetailId"
         .CommandType = CommandType.Text

         For Each _old As ApsRequestTrxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@RequestTrxDetailId", _old.RequestTrxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasApsRequestTrxChanges(oldRecord As ApsRequestTrx, newRecord As ApsRequestTrx) As Boolean

      With oldRecord
         If .TrxDate <> newRecord.TrxDate Then Return True
         If .MemberId <> newRecord.MemberId Then Return True
         If .ReviewerId <> newRecord.ReviewerId Then Return True
         If .RequestStatusId <> newRecord.RequestStatusId Then Return True
         If .ApproverId <> newRecord.ApproverId Then Return True
         If .ApsRequestTypeId <> newRecord.ApsRequestTypeId Then Return True
         If .ParticularsId <> newRecord.ParticularsId Then Return True
         If .Particulars <> newRecord.Particulars Then Return True

      End With

      Return False

   End Function

End Class

Public Class ApsRequestTrxBody
   Inherits ApsRequestTrx

   Public Property Details As ApsRequestTrxDetail()

End Class
