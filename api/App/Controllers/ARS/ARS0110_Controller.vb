<RoutePrefix("api")>
Public Class ARS0110_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_ARS0110")>
   <Route("references/ars0110")>
   <HttpGet>
   Public Function GetReferences_ARS0110() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0110_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "payTrx"     ' all defined Transaction Types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberAllowance")>
   <Route("member-allowances/{memberAllowanceId}")>
   <HttpGet>
   Public Function GetMemberAllowance(memberAllowanceId As Integer) As IHttpActionResult

      If memberAllowanceId <= 0 Then
         Throw New ArgumentException("Member Allowance ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0110")
            With _direct
               .AddParameter("MemberAllowanceId", memberAllowanceId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "allowance"
                     .Tables(1).TableName = "trxs"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMemberAllowance")>
   <Route("member-allowances/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberAllowance(currentUserId As Integer, <FromBody> allowance As ArsMemberAllowanceBody) As IHttpActionResult

      If allowance.MemberAllowanceId <> -1 Then
         Throw New ArgumentException("Member Allowance ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _memberAllowanceId As Integer = SysLib.GetNextSequence("MemberAllowanceId")

         allowance.MemberAllowanceId = _memberAllowanceId

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberAllowance As New ArsMemberAllowance
         Dim _arsMemberAllowanceTrxList As New ArsMemberAllowanceTrxList

         Me.LoadArsMemberAllowance(allowance, _arsMemberAllowance)

         For Each _detail As ArsMemberAllowanceTrx In allowance.Trxs
            _detail.MemberAllowanceId = _memberAllowanceId
            _arsMemberAllowanceTrxList.Add(_detail)
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

            Me.InsertArsMemberAllowance(_arsMemberAllowance)

            If _arsMemberAllowanceTrxList.Count > 0 Then
               Me.InsertArsMemberAllowanceTrxs(_arsMemberAllowanceTrxList)
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
         Return Me.Ok(allowance.MemberAllowanceId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMemberAllowance")>
   <Route("member-allowances/{memberAllowanceId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberAllowance(memberAllowanceId As Integer, currentUserId As Integer, <FromBody> allowance As ArsMemberAllowanceBody) As IHttpActionResult

      If memberAllowanceId <= 0 Then
         Throw New ArgumentException("Member Allowance ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberAllowance As New ArsMemberAllowance
         Dim _arsMemberAllowanceTrxList As New ArsMemberAllowanceTrxList

         Me.LoadArsMemberAllowance(allowance, _arsMemberAllowance)

         For Each _detail As ArsMemberAllowanceTrx In allowance.Trxs
            _arsMemberAllowanceTrxList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _arsMemberAllowanceOld As New ArsMemberAllowance
         Dim _arsMemberAllowanceTrxListOld As New ArsMemberAllowanceTrxList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMemberAllowance(memberAllowanceId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("allowance").Rows(0)
            Me.LoadArsMemberAllowance(_row, _arsMemberAllowanceOld)
            Me.LoadArsMemberAllowanceTrxList(_dataSet.Tables("trxs").Rows, _arsMemberAllowanceTrxListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberAllowanceTrx In _arsMemberAllowanceTrxListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberAllowanceTrx In _arsMemberAllowanceTrxList
               If _new.MemberAllowanceTrxId = _old.MemberAllowanceTrxId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberAllowanceTrx In _arsMemberAllowanceTrxList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberAllowanceTrx In _arsMemberAllowanceTrxListOld
               If _new.MemberAllowanceTrxId = _old.MemberAllowanceTrxId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .PayTrxCode <> _old.PayTrxCode OrElse .Amount <> _old.Amount OrElse .DailyFlag <> _old.DailyFlag Then
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

         Dim _arsMemberAllowanceTrxListNew As New ArsMemberAllowanceTrxList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _arsMemberAllowanceTrx As ArsMemberAllowanceTrx

            For Each _new As ArsMemberAllowanceTrx In _arsMemberAllowanceTrxList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberAllowanceTrx = New ArsMemberAllowanceTrx
                  _arsMemberAllowanceTrxListNew.Add(_arsMemberAllowanceTrx)
                  DataLib.ScatterValues(_new, _arsMemberAllowanceTrx)
                  _arsMemberAllowanceTrx.MemberAllowanceId = _arsMemberAllowance.MemberAllowanceId
               End If
            Next

         End If

         Dim _isArsMemberAllowanceChanged As Boolean = Me.HasArsMemberAllowanceChanges(_arsMemberAllowanceOld, _arsMemberAllowance)

         If Not _isArsMemberAllowanceChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetMemberAllowance(memberAllowanceId)
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

            If _isArsMemberAllowanceChanged Then
               Me.UpdateArsMemberAllowance(_arsMemberAllowance)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteArsMemberAllowanceTrxs(_arsMemberAllowanceTrxListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertArsMemberAllowanceTrxs(_arsMemberAllowanceTrxListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateArsMemberAllowanceTrxs(_arsMemberAllowanceTrxList)

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

   <SymAuthorization("RemoveMemberAllowance")>
   <Route("member-allowances/{memberAllowanceId}")>
   <HttpDelete>
   Public Function RemoveMemberAllowance(memberAllowanceId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberAllowanceId <= 0 Then
         Throw New ArgumentException("Member Allowance  ID is required.")
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

            Me.DeleteArsMemberAllowance(memberAllowanceId, q.LockId)

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
   Private Sub LoadArsMemberAllowance(allowance As ArsMemberAllowanceBody, ArsMemberAllowance As ArsMemberAllowance)

      DataLib.ScatterValues(allowance, ArsMemberAllowance)

   End Sub

   Private Sub LoadArsMemberAllowance(row As DataRow, allowance As ArsMemberAllowance)

      With allowance
         .MemberAllowanceId = row.ToInt32("MemberAllowanceId")
         .MemberId = row.ToInt32("MemberId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub
   Private Sub LoadArsMemberAllowanceTrxList(rows As DataRowCollection, list As ArsMemberAllowanceTrxList)

      Dim _detail As ArsMemberAllowanceTrx
      For Each _row As DataRow In rows
         _detail = New ArsMemberAllowanceTrx

         With _detail
            .MemberAllowanceTrxId = _row.ToInt32("MemberAllowanceTrxId")
            .MemberAllowanceId = _row.ToInt32("MemberAllowanceId")
            .PayTrxCode = _row.ToString("PayTrxCode")
            .DailyFlag = _row.ToBoolean("DailyFlag")
            .Amount = _row.ToDecimal("Amount")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertArsMemberAllowance(allowance As ArsMemberAllowance)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberAllowance", allowance, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberAllowance(allowance)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertArsMemberAllowanceTrxs(list As ArsMemberAllowanceTrxList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberAllowanceTrxId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberAllowanceTrx", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberAllowanceTrx In list
            Me.AddInsertUpdateParamsArsMemberAllowanceTrx(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateArsMemberAllowance(allowance As ArsMemberAllowance)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberAllowanceId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberAllowance", allowance, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberAllowance(allowance)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(allowance.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateArsMemberAllowanceTrxs(list As ArsMemberAllowanceTrxList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberAllowanceTrxId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberAllowanceTrx", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberAllowanceTrx In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberAllowanceTrx(_detail)
               .Parameters.AddWithValue("@MemberAllowanceTrxId", _detail.MemberAllowanceTrxId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub
   Private Sub AddInsertUpdateParamsArsMemberAllowance(allowance As ArsMemberAllowance)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberAllowanceId", allowance.MemberAllowanceId)
         .AddWithValue("@MemberId", allowance.MemberId)
         .AddWithValue("@MemberRequestId", allowance.MemberRequestId)
         .AddWithValue("@Remarks", allowance.Remarks)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberAllowanceTrx(dtl As ArsMemberAllowanceTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberAllowanceId", dtl.MemberAllowanceId)
         .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
         .AddWithValue("@DailyFlag", dtl.DailyFlag)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub
   Private Sub DeleteArsMemberAllowance(memberAllowanceId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("memberAllowanceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberAllowance", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MemberAllowanceId", memberAllowanceId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub DeleteArsMemberAllowanceTrxs(list As ArsMemberAllowanceTrxList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberAllowanceTrx WHERE MemberAllowanceTrxId=@MemberAllowanceTrxId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberAllowanceTrx In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberAllowanceTrxId", _old.MemberAllowanceTrxId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasArsMemberAllowanceChanges(oldRecord As ArsMemberAllowance, newRecord As ArsMemberAllowance) As Boolean

      With oldRecord
         If .MemberId <> newRecord.MemberId Then Return True
         If .MemberRequestId <> newRecord.MemberRequestId Then Return True
         If .Remarks <> newRecord.Remarks Then Return True

      End With

      Return False

   End Function

End Class

Public Class ArsMemberAllowanceBody
   Inherits ArsMemberAllowance
   Public Property Trxs As ArsMemberAllowanceTrx()

End Class
