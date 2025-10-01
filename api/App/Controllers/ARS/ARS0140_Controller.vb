<RoutePrefix("api")>
Public Class ARS0140_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_ARS0140")>
   <Route("references/ars0140")>
   <HttpGet>
   Public Function GetReferences_ARS0140() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0140_References")
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

   <SymAuthorization("GetMemberDeminimis")>
   <Route("member-deminimis/{memberDeminimisId}")>
   <HttpGet>
   Public Function GetMemberDeminimis(memberDeminimisId As Integer) As IHttpActionResult

      If memberDeminimisId <= 0 Then
         Throw New ArgumentException("Member Deminimis ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0140")
            With _direct
               .AddParameter("MemberDeminimisId", memberDeminimisId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "deminimis"
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

   <SymAuthorization("CreateMemberDeminimis")>
   <Route("member-deminimis/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberDeminimis(currentUserId As Integer, <FromBody> deminimis As ArsMemberDeminimisBody) As IHttpActionResult

      If deminimis.MemberDeminimisId <> -1 Then
         Throw New ArgumentException("Member Deminimis ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _memberDeminimisId As Integer = SysLib.GetNextSequence("MemberDeminimisId")

         deminimis.MemberDeminimisId = _memberDeminimisId

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberDeminimis As New ArsMemberDeminimis
         Dim _arsMemberDeminimisTrxList As New ArsMemberDeminimisTrxList

         Me.LoadArsMemberDeminimis(deminimis, _arsMemberDeminimis)

         For Each _detail As ArsMemberDeminimisTrx In deminimis.Trxs
            _detail.MemberDeminimisId = _memberDeminimisId
            _arsMemberDeminimisTrxList.Add(_detail)
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

            Me.InsertArsMemberDeminimis(_arsMemberDeminimis)

            If _arsMemberDeminimisTrxList.Count > 0 Then
               Me.InsertArsMemberDeminimisTrxs(_arsMemberDeminimisTrxList)
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
         Return Me.Ok(deminimis.MemberDeminimisId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMemberDeminimis")>
   <Route("member-deminimis/{memberDeminimisId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberDeminimis(memberDeminimisId As Integer, currentUserId As Integer, <FromBody> deminimis As ArsMemberDeminimisBody) As IHttpActionResult

      If memberDeminimisId <= 0 Then
         Throw New ArgumentException("Member Deminimis ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberDeminimis As New ArsMemberDeminimis
         Dim _arsMemberDeminimisTrxList As New ArsMemberDeminimisTrxList

         Me.LoadArsMemberDeminimis(deminimis, _arsMemberDeminimis)

         For Each _detail As ArsMemberDeminimisTrx In deminimis.Trxs
            _arsMemberDeminimisTrxList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _arsMemberDeminimisOld As New ArsMemberDeminimis
         Dim _arsMemberDeminimisTrxListOld As New ArsMemberDeminimisTrxList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMemberDeminimis(memberDeminimisId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("deminimis").Rows(0)
            Me.LoadArsMemberDeminimis(_row, _arsMemberDeminimisOld)
            Me.LoadArsMemberDeminimisTrxList(_dataSet.Tables("trxs").Rows, _arsMemberDeminimisTrxListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberDeminimisTrx In _arsMemberDeminimisTrxListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberDeminimisTrx In _arsMemberDeminimisTrxList
               If _new.MemberDeminimisTrxId = _old.MemberDeminimisTrxId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberDeminimisTrx In _arsMemberDeminimisTrxList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberDeminimisTrx In _arsMemberDeminimisTrxListOld
               If _new.MemberDeminimisTrxId = _old.MemberDeminimisTrxId Then
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

         Dim _arsMemberDeminimisTrxListNew As New ArsMemberDeminimisTrxList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _arsMemberDeminimisTrx As ArsMemberDeminimisTrx

            For Each _new As ArsMemberDeminimisTrx In _arsMemberDeminimisTrxList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberDeminimisTrx = New ArsMemberDeminimisTrx
                  _arsMemberDeminimisTrxListNew.Add(_arsMemberDeminimisTrx)
                  DataLib.ScatterValues(_new, _arsMemberDeminimisTrx)
                  _arsMemberDeminimisTrx.MemberDeminimisId = _arsMemberDeminimis.MemberDeminimisId
               End If
            Next

         End If

         Dim _isArsMemberDeminimisChanged As Boolean = Me.HasArsMemberDeminimisChanges(_arsMemberDeminimisOld, _arsMemberDeminimis)

         If Not _isArsMemberDeminimisChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetMemberDeminimis(memberDeminimisId)
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

            If _isArsMemberDeminimisChanged Then
               Me.UpdateArsMemberDeminimis(_arsMemberDeminimis)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteArsMemberDeminimisTrxs(_arsMemberDeminimisTrxListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertArsMemberDeminimisTrxs(_arsMemberDeminimisTrxListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateArsMemberDeminimisTrxs(_arsMemberDeminimisTrxList)

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

   <SymAuthorization("RemoveMemberDeminimis")>
   <Route("member-deminimis/{memberDeminimisId}")>
   <HttpDelete>
   Public Function RemoveMemberDeminimis(memberDeminimisId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberDeminimisId <= 0 Then
         Throw New ArgumentException("Member Deminimis  ID is required.")
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

            Me.DeleteArsMemberDeminimis(memberDeminimisId, q.LockId)

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
   Private Sub LoadArsMemberDeminimis(deminimis As ArsMemberDeminimisBody, ArsMemberDeminimis As ArsMemberDeminimis)

      DataLib.ScatterValues(deminimis, ArsMemberDeminimis)

   End Sub

   Private Sub LoadArsMemberDeminimis(row As DataRow, deminimis As ArsMemberDeminimis)

      With deminimis
         .MemberDeminimisId = row.ToInt32("MemberDeminimisId")
         .MemberId = row.ToInt32("MemberId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub
   Private Sub LoadArsMemberDeminimisTrxList(rows As DataRowCollection, list As ArsMemberDeminimisTrxList)

      Dim _detail As ArsMemberDeminimisTrx
      For Each _row As DataRow In rows
         _detail = New ArsMemberDeminimisTrx

         With _detail
            .MemberDeminimisTrxId = _row.ToInt32("MemberDeminimisTrxId")
            .MemberDeminimisId = _row.ToInt32("MemberDeminimisId")
            .PayTrxCode = _row.ToString("PayTrxCode")
            .DailyFlag = _row.ToBoolean("DailyFlag")
            .Amount = _row.ToDecimal("Amount")

         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertArsMemberDeminimis(deminimis As ArsMemberDeminimis)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberDeminimis", deminimis, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberDeminimis(deminimis)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertArsMemberDeminimisTrxs(list As ArsMemberDeminimisTrxList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberDeminimisTrxId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberDeminimisTrx", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberDeminimisTrx In list
            Me.AddInsertUpdateParamsArsMemberDeminimisTrx(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateArsMemberDeminimis(deminimis As ArsMemberDeminimis)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberDeminimisId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberDeminimis", deminimis, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberDeminimis(deminimis)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(deminimis.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateArsMemberDeminimisTrxs(list As ArsMemberDeminimisTrxList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberDeminimisTrxId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberDeminimisTrx", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberDeminimisTrx In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberDeminimisTrx(_detail)
               .Parameters.AddWithValue("@MemberDeminimisTrxId", _detail.MemberDeminimisTrxId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub
   Private Sub AddInsertUpdateParamsArsMemberDeminimis(deminimis As ArsMemberDeminimis)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberDeminimisId", deminimis.MemberDeminimisId)
         .AddWithValue("@MemberId", deminimis.MemberId)
         .AddWithValue("@MemberRequestId", deminimis.MemberRequestId)
         .AddWithValue("@Remarks", deminimis.Remarks)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberDeminimisTrx(dtl As ArsMemberDeminimisTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberDeminimisId", dtl.MemberDeminimisId)
         .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
         .AddWithValue("@DailyFlag", dtl.DailyFlag)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub
   Private Sub DeleteArsMemberDeminimis(memberDeminimisId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("memberDeminimisId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberDeminimis", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MemberDeminimisId", memberDeminimisId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub DeleteArsMemberDeminimisTrxs(list As ArsMemberDeminimisTrxList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberDeminimisTrx WHERE MemberDeminimisTrxId=@MemberDeminimisTrxId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberDeminimisTrx In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberDeminimisTrxId", _old.MemberDeminimisTrxId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasArsMemberDeminimisChanges(oldRecord As ArsMemberDeminimis, newRecord As ArsMemberDeminimis) As Boolean

      With oldRecord
         If .MemberId <> newRecord.MemberId Then Return True
         If .MemberRequestId <> newRecord.MemberRequestId Then Return True
         If .Remarks <> newRecord.Remarks Then Return True

      End With

      Return False

   End Function

End Class

Public Class ArsMemberDeminimisBody
   Inherits ArsMemberDeminimis
   Public Property Trxs As ArsMemberDeminimisTrx()

End Class
