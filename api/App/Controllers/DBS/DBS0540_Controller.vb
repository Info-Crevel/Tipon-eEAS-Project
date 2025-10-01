<RoutePrefix("api")>
Public Class DBS0540_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0540")>
   <Route("references/dbs0540")>
   <HttpGet>
   Public Function GetReferences_DBS0540() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0540_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "payFrequencies"      ' Daily, Weekly, Semi-Monthly, Monthly
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMatrixPhh")>
   <Route("matrix-phh/{phhId}")>
   <HttpGet>
   Public Function GetDbsMatrixPhh(phhId As Integer) As IHttpActionResult

      If phhId <= 0 Then
         Throw New ArgumentException("Matrix Phh ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0540")
            With _direct
               .AddParameter("PhhId", phhId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "matrixPhh"
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

   <SymAuthorization("CreateMatrixPhh")>
   <Route("matrix-phh/{currentUserId}")>
   <HttpPost>
   Public Function CreateMatrixPhh(currentUserId As Integer, <FromBody> matrixPhh As DbsMatrixPhhBody) As IHttpActionResult

      If matrixPhh.PhhId <> -1 Then
         Throw New ArgumentException("Matrix Phh ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _phhId As Integer = SysLib.GetNextSequence("PhhId")

         matrixPhh.PhhId = _phhId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPhh As New DbsMatrixPhh
         Dim _dbsMatrixPhhDetailList As New DbsMatrixPhhDetailList

         Me.LoadDbsMatrixPhh(matrixPhh, _dbsMatrixPhh)

         For Each _detail As DbsMatrixPhhDetail In matrixPhh.Details
            _detail.PhhId = _phhId
            _dbsMatrixPhhDetailList.Add(_detail)
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

            Me.InsertDbsMatrixPhh(_dbsMatrixPhh)

            If _dbsMatrixPhhDetailList.Count > 0 Then
               Me.InsertDbsMatrixPhhDetails(_dbsMatrixPhhDetailList)
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
         Return Me.Ok(matrixPhh.PhhId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMatrixPhh")>
   <Route("matrix-phh/{phhId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMatrixPhh(phhId As Integer, currentUserId As Integer, <FromBody> matrixPhh As DbsMatrixPhhBody) As IHttpActionResult

      If phhId <= 0 Then
         Throw New ArgumentException("Matrix Phh ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsMatrixPhh As New DbsMatrixPhh
         Dim _dbsMatrixPhhDetailList As New DbsMatrixPhhDetailList

         Me.LoadDbsMatrixPhh(matrixPhh, _dbsMatrixPhh)

         For Each _detail As DbsMatrixPhhDetail In matrixPhh.Details
            _dbsMatrixPhhDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsMatrixPhhOld As New DbsMatrixPhh
         Dim _dbsMatrixPhhDetailListOld As New DbsMatrixPhhDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsMatrixPhh(phhId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("matrixPhh").Rows(0)
            Me.LoadDbsMatrixPhh(_row, _dbsMatrixPhhOld)
            Me.LoadDbsMatrixPhhDetailList(_dataSet.Tables("details").Rows, _dbsMatrixPhhDetailListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsMatrixPhhDetail In _dbsMatrixPhhDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsMatrixPhhDetail In _dbsMatrixPhhDetailList
               If _new.PhhRangeId = _old.PhhRangeId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If

         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsMatrixPhhDetail In _dbsMatrixPhhDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsMatrixPhhDetail In _dbsMatrixPhhDetailListOld
               If _new.PhhRangeId = _old.PhhRangeId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .MinAmount <> _old.MinAmount OrElse .MaxAmount <> _old.MaxAmount OrElse .PremiumRate <> _old.PremiumRate OrElse .FixedAmount <> _old.FixedAmount Then
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

         Dim _dbsMatrixPhhDetailListNew As New DbsMatrixPhhDetailList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsMatrixPhhDetail As DbsMatrixPhhDetail

            For Each _new As DbsMatrixPhhDetail In _dbsMatrixPhhDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsMatrixPhhDetail = New DbsMatrixPhhDetail
                  _dbsMatrixPhhDetailListNew.Add(_dbsMatrixPhhDetail)
                  DataLib.ScatterValues(_new, _dbsMatrixPhhDetail)
                  _dbsMatrixPhhDetail.PhhId = _dbsMatrixPhh.PhhId
               End If
            Next

         End If

         Dim _isDbsMatrixPhhChanged As Boolean = Me.HasDbsMatrixPhhChanges(_dbsMatrixPhhOld, _dbsMatrixPhh)

         If Not _isDbsMatrixPhhChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsMatrixPhh(phhId)
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

            If _isDbsMatrixPhhChanged Then
               Me.UpdateDbsMatrixPhh(_dbsMatrixPhh)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsMatrixPhhDetails(_dbsMatrixPhhDetailListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsMatrixPhhDetails(_dbsMatrixPhhDetailListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsMatrixPhhDetails(_dbsMatrixPhhDetailList)
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

   <SymAuthorization("RemoveMatrixPhh")>
   <Route("matrix-phh/{phhId}")>
   <HttpDelete>
   Public Function RemoveMatrixPhh(phhId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If phhId <= 0 Then
         Throw New ArgumentException("Matrix Phh ID is required.")
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

            Me.DeleteDbsMatrixPhh(phhId, q.LockId)

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

   Private Sub LoadDbsMatrixPhh(holiday As DbsMatrixPhhBody, dbsMatrixPhh As DbsMatrixPhh)

      DataLib.ScatterValues(holiday, dbsMatrixPhh)

   End Sub

   Private Sub LoadDbsMatrixPhh(row As DataRow, matrixPhh As DbsMatrixPhh)

      With matrixPhh
         .PhhId = row.ToInt32("PhhId")
         .PhhName = row.ToString("PhhName")
         .StartDate = row.ToDate("StartDate")
      End With

   End Sub

   Private Sub LoadDbsMatrixPhhDetailList(rows As DataRowCollection, list As DbsMatrixPhhDetailList)

      Dim _detail As DbsMatrixPhhDetail
      For Each _row As DataRow In rows
         _detail = New DbsMatrixPhhDetail

         With _detail
            .PhhId = _row.ToInt32("PhhId")
            .PhhRangeId = _row.ToInt32("PhhRangeId")
            .MinAmount = _row.ToDecimal("MinAmount")
            .MaxAmount = _row.ToDecimal("MaxAmount")
            .PremiumRate = _row.ToDecimal("PremiumRate")
            .FixedAmount = _row.ToDecimal("FixedAmount")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertDbsMatrixPhh(matrixPhh As DbsMatrixPhh)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPhh", matrixPhh, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPhh(matrixPhh)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsMatrixPhhDetails(list As DbsMatrixPhhDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("PhhRangeId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMatrixPhhDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixPhhDetail In list
            Me.AddInsertUpdateParamsDbsMatrixPhhDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub UpdateDbsMatrixPhh(matrixPhh As DbsMatrixPhh)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PhhId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPhh", matrixPhh, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMatrixPhh(matrixPhh)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(matrixPhh.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMatrixPhhDetails(list As DbsMatrixPhhDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PhhRangeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMatrixPhhDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMatrixPhhDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsMatrixPhhDetail(_detail)
               .Parameters.AddWithValue("@PhhRangeId", _detail.PhhRangeId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPhh(matrixPhh As DbsMatrixPhh)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PhhId", matrixPhh.PhhId)
         .AddWithValue("@PhhName", matrixPhh.PhhName)
         .AddWithValue("@StartDate", matrixPhh.StartDate)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMatrixPhhDetail(dtl As DbsMatrixPhhDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PhhId", dtl.PhhId)
         .AddWithValue("@MinAmount", dtl.MinAmount)
         .AddWithValue("@MaxAmount", dtl.MaxAmount)
         .AddWithValue("@PremiumRate", dtl.PremiumRate)
         .AddWithValue("@FixedAmount", dtl.FixedAmount)

      End With

   End Sub

   Private Sub DeleteDbsMatrixPhh(phhId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PhhId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMatrixPhh", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PhhId", phhId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsMatrixPhhDetails(list As DbsMatrixPhhDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsMatrixPhhDetail WHERE PhhRangeId=@PhhRangeId"
         .CommandType = CommandType.Text

         For Each _old As DbsMatrixPhhDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@PhhRangeId", _old.PhhRangeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasDbsMatrixPhhChanges(oldRecord As DbsMatrixPhh, newRecord As DbsMatrixPhh) As Boolean

      With oldRecord
         If .PhhName <> newRecord.PhhName Then Return True
         If .StartDate <> newRecord.StartDate Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsMatrixPhhBody
   Inherits DbsMatrixPhh

   Public Property Details As DbsMatrixPhhDetail()

End Class
