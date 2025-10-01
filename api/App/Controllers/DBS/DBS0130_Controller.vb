<RoutePrefix("api")>
Public Class DBS0130_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0130")>
   <Route("references/dbs0130")>
   <HttpGet>
   Public Function GetReferences_DBS0130() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0130_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "skills"     ' all defined Transaction Types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetSkillSet")>
   <Route("skill-sets/{skillSetId}")>
   <HttpGet>
   Public Function GetSkillSet(skillSetId As Integer) As IHttpActionResult

      If skillSetId <= 0 Then
         Throw New ArgumentException("Skill Set ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0130")
            With _direct
               .AddParameter("SkillSetId", skillSetId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "skillSet"
                     .Tables(1).TableName = "skillSetDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateSkillSet")>
   <Route("skill-sets/{currentUserId}")>
   <HttpPost>
   Public Function CreateSkillSet(currentUserId As Integer, <FromBody> skillSet As DbsSkillSetBody) As IHttpActionResult

      If skillSet.SkillSetId <> -1 Then
         Throw New ArgumentException("Skill Set ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _skillSetId As Integer = SysLib.GetNextSequence("SkillSetId")

         skillSet.SkillSetId = _skillSetId

         '
         ' Load proposed values from payload
         '
         Dim _dbsSkillSet As New DbsSkillSet
         Dim _dbsSkillSetDetailList As New DbsSkillSetDetailList

         Me.LoadDbsSkillSet(skillSet, _dbsSkillSet)

         For Each _detail As DbsSkillSetDetail In skillSet.Details
            _detail.SkillSetId = _skillSetId
            _dbsSkillSetDetailList.Add(_detail)
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

            Me.InsertDbsSkillSet(_dbsSkillSet)
            Me.InsertDbsSkillSetDetails(_dbsSkillSetDetailList)

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
         Return Me.Ok(skillSet.SkillSetId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifySkillSet")>
   <Route("skill-sets/{skillSetId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifySkillSet(skillSetId As Integer, currentUserId As Integer, <FromBody> skillSet As DbsSkillSetBody) As IHttpActionResult

      If skillSetId <= 0 Then
         Throw New ArgumentException("Skill Set ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsSkillSet As New DbsSkillSet
         Dim _dbsSkillSetDetailList As New DbsSkillSetDetailList

         Me.LoadDbsSkillSet(skillSet, _dbsSkillSet)

         For Each _detail As DbsSkillSetDetail In skillSet.Details
            _dbsSkillSetDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsSkillSetOld As New DbsSkillSet
         Dim _dbsSkillSetDetailListOld As New DbsSkillSetDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetSkillSet(skillSetId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("skillSet").Rows(0)
            Me.LoadDbsSkillSet(_row, _dbsSkillSetOld)
            Me.LoadDbsSkillSetDetailList(_dataSet.Tables("skillSetDetails").Rows, _dbsSkillSetDetailListOld)
         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As DbsSkillSetDetail In _dbsSkillSetDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsSkillSetDetail In _dbsSkillSetDetailList
               If _new.SkillDetailId = _old.SkillDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsSkillSetDetail In _dbsSkillSetDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsSkillSetDetail In _dbsSkillSetDetailListOld
               If _new.SkillDetailId = _old.SkillDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .SkillId <> _old.SkillId OrElse .SkillSetId <> _old.SkillSetId OrElse .SortSeq <> _old.SortSeq Then
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

         Dim _dbsSkillSetDetailListNew As New DbsSkillSetDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsSkillSetDetail As DbsSkillSetDetail

            For Each _new As DbsSkillSetDetail In _dbsSkillSetDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsSkillSetDetail = New DbsSkillSetDetail
                  _dbsSkillSetDetailListNew.Add(_dbsSkillSetDetail)
                  DataLib.ScatterValues(_new, _dbsSkillSetDetail)
                  _dbsSkillSetDetail.SkillSetId = _dbsSkillSet.SkillSetId
               End If
            Next

         End If

         Dim _isDbsSkillSetChanged As Boolean = Me.HasDbsSkillSetChanges(_dbsSkillSetOld, _dbsSkillSet)

         If Not _isDbsSkillSetChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetSkillSet(skillSetId)
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

            If _isDbsSkillSetChanged Then
               Me.UpdateDbsSkillSet(_dbsSkillSet)

            End If

            '
            ' FinTemplateDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteDbsSkillSetDetails(_dbsSkillSetDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsSkillSetDetails(_dbsSkillSetDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsSkillSetDetails(_dbsSkillSetDetailList)

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

   <SymAuthorization("RemoveSkillSet")>
   <Route("skill-sets/{skillSetId}")>
   <HttpDelete>
   Public Function RemoveSkillSet(skillSetId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If skillSetId <= 0 Then
         Throw New ArgumentException("Skill Set ID is required.")
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

            Me.DeleteDbsSkillSet(skillSetId, q.LockId)

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

   Private Sub LoadDbsSkillSet(skillSet As DbsSkillSetBody, dbsSkillSet As DbsSkillSet)

      DataLib.ScatterValues(skillSet, dbsSkillSet)

   End Sub

   Private Sub LoadDbsSkillSet(row As DataRow, skillSet As DbsSkillSet)

      With skillSet
         .SkillSetId = row.ToInt32("SkillSetId")
         .SkillSetName = row.ToString("SkillSetName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub LoadDbsSkillSetDetailList(rows As DataRowCollection, list As DbsSkillSetDetailList)

      Dim _detail As DbsSkillSetDetail
      For Each _row As DataRow In rows
         _detail = New DbsSkillSetDetail

         With _detail
            .SkillDetailId = _row.ToInt32("SkillDetailId")
            .SkillId = _row.ToInt32("SkillId")
            .SortSeq = _row.ToInt32("SortSeq")
            '.CreditAmount = _row.ToDecimal("CreditAmount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsSkillSet(skillSet As DbsSkillSet)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSkillSet", skillSet, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSkillSet(skillSet)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsSkillSetDetails(list As DbsSkillSetDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("SkillDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSkillSetDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsSkillSetDetail In list
            Me.AddInsertUpdateParamsDbsSkillSetDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsSkillSet(skillSet As DbsSkillSet)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("SkillSetId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSkillSet", skillSet, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSkillSet(skillSet)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(skillSet.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsSkillSetDetails(list As DbsSkillSetDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("SkillDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSkillSetDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsSkillSetDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsSkillSetDetail(_detail)
               .Parameters.AddWithValue("@SkillDetailId", _detail.SkillDetailId)

               .ExecuteNonQuery()
            End If
         Next


      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsSkillSet(skillSet As DbsSkillSet)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SkillSetId", skillSet.SkillSetId)
         .AddWithValue("@SkillSetName", skillSet.SkillSetName)
         .AddWithValue("@SortSeq", skillSet.SortSeq)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsSkillSetDetail(dtl As DbsSkillSetDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SkillSetId", dtl.SkillSetId)
         .AddWithValue("@SkillId", dtl.SkillId)
         .AddWithValue("@SortSeq", dtl.SortSeq)
      End With

   End Sub

   Private Sub DeleteDbsSkillSet(skillSetId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SkillSetId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsSkillSet", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SkillSetId", skillSetId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsSkillSetDetails(list As DbsSkillSetDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsSkillSetDetail WHERE SkillDetailId=@SkillDetailId"
         .CommandType = CommandType.Text

         For Each _old As DbsSkillSetDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@SkillDetailId", _old.SkillDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasDbsSkillSetChanges(oldRecord As DbsSkillSet, newRecord As DbsSkillSet) As Boolean

      With oldRecord
         If .SkillSetName <> newRecord.SkillSetName Then Return True
         If .SortSeq <> newRecord.SortSeq Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsSkillSetBody
   Inherits DbsSkillSet

   Public Property Details As DbsSkillSetDetail()

End Class
