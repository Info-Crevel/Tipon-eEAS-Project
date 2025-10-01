<RoutePrefix("api")>
Public Class CSH0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_CSH0030")>
   <Route("references/csh0030")>
   <HttpGet>
   Public Function GetReferences_CSH0030() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.CSH0030_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "fundClusters"     ' all defined Fund Clusters
                     .Tables(1).TableName = "cdxTypes"         ' all defined cash disbursement transaction types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCshCdx")>
   <Route("cds/{cdxId}")>
   <HttpGet>
   Public Function GetCshCdx(cdxId As Integer) As IHttpActionResult

      If cdxId <= 0 Then
         Throw New ArgumentException("CD ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0030")
            With _direct
               .AddParameter("CdxId", cdxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "cdx"
                     .Tables(1).TableName = "cdxDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateCshCdx")>
   <Route("cds/{currentUserId}")>
   <HttpPost>
   Public Function CreateCshCdx(currentUserId As Integer, <FromBody> cdx As CshCdxBody) As IHttpActionResult

      If cdx.CdxId <> -1 Then
         Throw New ArgumentException("CD ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _cdxId As Integer = SysLib.GetNextSequence("CshCdxId")

         cdx.CdxId = _cdxId

         '
         ' Load proposed values from payload
         '
         Dim _cshCdx As New CshCdx
         Dim _cshCdxDetailList As New CshCdxDetailList

         Me.LoadCshCdx(cdx, _cshCdx)

         For Each _detail As CshCdxDetail In cdx.Details
            _detail.CdxId = _cdxId
            _cshCdxDetailList.Add(_detail)
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

            Me.InsertCshCdx(_cshCdx)
            Me.InsertCshCdxDetails(_cshCdxDetailList)

            _successFlag = True

         Catch _exception As Exception
            File.WriteAllText("d:\yyy.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
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
         Return Me.Ok(cdx.CdxId)

      Catch _exception As Exception
         File.WriteAllText("d:\zzz.txt", _exception.Message + Tags.NewLine2 + _exception.StackTrace)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyCshCdx")>
   <Route("cds/{cdxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCshCdx(cdxId As Integer, currentUserId As Integer, <FromBody> cdx As CshCdxBody) As IHttpActionResult

      If cdxId <= 0 Then
         Throw New ArgumentException("CD ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _cshCdx As New CshCdx
         Dim _cshCdxDetailList As New CshCdxDetailList

         Me.LoadCshCdx(cdx, _cshCdx)

         For Each _detail As CshCdxDetail In cdx.Details
            _cshCdxDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _cshCdxOld As New CshCdx
         Dim _cshCdxDetailListOld As New CshCdxDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetCshCdx(cdxId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("cdx").Rows(0)
            Me.LoadCshCdx(_row, _cshCdxOld)
            Me.LoadCshCdxDetailList(_dataSet.Tables("cdxDetails").Rows, _cshCdxDetailListOld)
         End Using

         '
         ' CshCdxDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As CshCdxDetail In _cshCdxDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As CshCdxDetail In _cshCdxDetailList
               If _new.CdxDetailId = _old.CdxDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As CshCdxDetail In _cshCdxDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As CshCdxDetail In _cshCdxDetailListOld
               If _new.CdxDetailId = _old.CdxDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .CdxTypeId <> _old.CdxTypeId OrElse .ReferenceDate <> _old.ReferenceDate Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .Reference <> _old.Reference OrElse .PayeeName <> _old.PayeeName Then
                        .LogActionId = LogActionId.Edit
                     End If

                     If .AccountId <> _old.AccountId OrElse .Amount <> _old.Amount Then
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

         Dim _cshCdxDetailListNew As New CshCdxDetailList      ' for adding new Cdx Details

         If _addDetailCount > 0 Then
            Dim _cshCdxDetail As CshCdxDetail

            For Each _new As CshCdxDetail In _cshCdxDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _cshCdxDetail = New CshCdxDetail
                  _cshCdxDetailListNew.Add(_cshCdxDetail)
                  DataLib.ScatterValues(_new, _cshCdxDetail)
                  _cshCdxDetail.CdxId = _cshCdx.CdxId
               End If
            Next

         End If

         Dim _isCshCdxChanged As Boolean = Me.HasCshCdxChanges(_cshCdxOld, _cshCdx)

         If Not _isCshCdxChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetCshCdx(cdxId)
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

            If _isCshCdxChanged Then
               Me.UpdateCshCdx(_cshCdx)
            End If

            '
            ' CshCdxDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteCshCdxDetails(_cshCdxDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertCshCdxDetails(_cshCdxDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateCshCdxDetails(_cshCdxDetailList)
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

   <SymAuthorization("RemoveCshCdx")>
   <Route("cds/{cdxId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveCshCdx(cdxId As Integer, lockId As String) As IHttpActionResult

      If cdxId <= 0 Then
         Throw New ArgumentException("CD ID is required.")
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

            Me.DeleteCshCdx(cdxId, lockId)

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

   Private Sub LoadCshCdx(cdx As CshCdxBody, cshCdx As CshCdx)

      DataLib.ScatterValues(cdx, cshCdx)

   End Sub

   Private Sub LoadCshCdx(row As DataRow, cdx As CshCdx)

      With cdx
         .CdxId = row.ToInt32("CdxId")
         .TrxDate = row.ToDate("TrxDate")
         .FundClusterId = row.ToString("FundClusterId")
         .DbxId = row.ToInt32("DbxId")
         .PostUserId = row.ToInt32("PostUserId")
      End With

   End Sub

   Private Sub LoadCshCdxDetailList(rows As DataRowCollection, list As CshCdxDetailList)

      Dim _detail As CshCdxDetail
      For Each _row As DataRow In rows
         _detail = New CshCdxDetail

         With _detail
            .CdxDetailId = _row.ToInt32("CdxDetailId")
            .CdxId = _row.ToInt32("CdxId")
            .CdxTypeId = _row.ToInt32("CdxTypeId")
            .ReferenceDate = _row.ToDate("ReferenceDate")
            .Reference = _row.ToString("Reference")
            .PayeeName = _row.ToString("PayeeName")
            .AccountId = _row.ToString("AccountId")
            .Amount = _row.ToDecimal("Amount")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertCshCdx(cdx As CshCdx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshCdx", cdx, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshCdx(cdx)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertCshCdxDetails(list As CshCdxDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("CdxDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshCdxDetail", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshCdxDetail In list
            Me.AddInsertUpdateParamsCshCdxDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateCshCdx(cdx As CshCdx)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CdxId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshCdx", cdx, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshCdx(cdx)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(cdx.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateCshCdxDetails(list As CshCdxDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CdxDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshCdxDetail", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As CshCdxDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsCshCdxDetail(_detail)
               .Parameters.AddWithValue("@CdxDetailId", _detail.CdxDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshCdx(cdx As CshCdx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CdxId", cdx.CdxId)
         .AddWithValue("@TrxDate", cdx.TrxDate)
         .AddWithValue("@FundClusterId", cdx.FundClusterId)
         .AddWithValue("@DbxId", cdx.DbxId)
         .AddWithValue("@PostUserId", cdx.PostUserId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshCdxDetail(dtl As CshCdxDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CdxId", dtl.CdxId)
         .AddWithValue("@CdxTypeId", dtl.CdxTypeId)
         .AddWithValue("@ReferenceDate", dtl.ReferenceDate)
         .AddWithValue("@Reference", dtl.Reference)
         .AddWithValue("@PayeeName", dtl.PayeeName)
         .AddWithValue("@AccountId", dtl.AccountId)
         .AddWithValue("@Amount", dtl.Amount)
      End With

   End Sub

   Private Sub DeleteCshCdx(cdxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CdxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.CshCdx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CdxId", cdxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteCshCdxDetails(list As CshCdxDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.CshCdxDetail WHERE CdxDetailId=@CdxDetailId"
         .CommandType = CommandType.Text

         For Each _old As CshCdxDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@CdxDetailId", _old.CdxDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasCshCdxChanges(oldRecord As CshCdx, newRecord As CshCdx) As Boolean

      With oldRecord
         If .TrxDate <> newRecord.TrxDate Then Return True
         If .FundClusterId <> newRecord.FundClusterId Then Return True
         If .DbxId <> newRecord.DbxId Then Return True
      End With

      Return False

   End Function

End Class

Public Class CshCdxBody
   Inherits CshCdx

   Public Property Details As CshCdxDetail()

End Class