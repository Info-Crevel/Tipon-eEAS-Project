<RoutePrefix("api")>
Public Class DBS0050_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetRegion")>
   <Route("regions/{regionId}")>
   <HttpGet>
   Public Function GetRegion(regionId As String) As IHttpActionResult

      If String.IsNullOrEmpty(regionId) Then
         Throw New ArgumentException("Region ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0050")
            With _direct
               .AddParameter("RegionId", regionId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "region"
                     .Tables(1).TableName = "regionDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateRegion")>
   <Route("regions/{currentUserId}")>
   <HttpPost>
   Public Function CreateRegion(currentUserId As Integer, <FromBody> region As DbsRegionBody) As IHttpActionResult

        If String.IsNullOrEmpty(region.RegionId) Then
         Throw New ArgumentException("Region ID not recognized.")
      End If

      Try

            Dim _currentUserId As Integer = 1      ' System (default)
            If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID
         '
         Dim _regionId As String = region.RegionId

         '
         ' Load proposed values from payload
         '
         Dim _dbsRegion As New DbsRegion
         Dim _dbsRegionDetailList As New DbsRegionDetailList

         Me.LoadDbsRegion(region, _dbsRegion)

            For Each _detail As DbsRegionDetail In region.Details
            _detail.RegionId = _regionId
            _dbsRegionDetailList.Add(_detail)
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

                Me.InsertDbsRegion(_dbsRegion)

                If _dbsRegionDetailList.Count > 0 Then
                    Me.InsertDbsRegionDetails(_dbsRegionDetailList)
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
         'Return Me.Ok(region.RegionId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyRegion")>
   <Route("regions/{regionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyRegion(regionId As String, currentUserId As Integer, <FromBody> region As DbsRegionBody) As IHttpActionResult

      If String.IsNullOrEmpty(regionId) Then
         Throw New ArgumentException("Region ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsRegion As New DbsRegion
         Dim _dbsRegionDetailList As New DbsRegionDetailList

         Me.LoadDbsRegion(region, _dbsRegion)

         For Each _detail As DbsRegionDetail In region.Details
            _dbsRegionDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsRegionOld As New DbsRegion
         Dim _dbsRegionDetailListOld As New DbsRegionDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetRegion(regionId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("region").Rows(0)
            Me.LoadDbsRegion(_row, _dbsRegionOld)
            Me.LoadDbsRegionDetailList(_dataSet.Tables("regionDetails").Rows, _dbsRegionDetailListOld)
         End Using

         '
         ' Apply changes, save to DB
         '

         '
         ' DbsRegionDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As DbsRegionDetail In _dbsRegionDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsRegionDetail In _dbsRegionDetailList
               If _new.ProvinceId = _old.ProvinceId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsRegionDetail In _dbsRegionDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsRegionDetail In _dbsRegionDetailListOld
               If _new.ProvinceId = _old.ProvinceId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .ProvinceName <> _old.ProvinceName Then
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

         Dim _dbsRegionDetailListNew As New DbsRegionDetailList      ' for adding new Provinces

         If _addDetailCount > 0 Then
            Dim _dbsRegionDetail As DbsRegionDetail

            For Each _new As DbsRegionDetail In _dbsRegionDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsRegionDetail = New DbsRegionDetail
                  _dbsRegionDetailListNew.Add(_dbsRegionDetail)
                  DataLib.ScatterValues(_new, _dbsRegionDetail)
                  _dbsRegionDetail.RegionId = _dbsRegion.RegionId       ' not needed
               End If
            Next

         End If

         Dim _isDbsRegionChanged As Boolean = Me.HasDbsRegionChanges(_dbsRegionOld, _dbsRegion)

         If Not _isDbsRegionChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetRegion(regionId)
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

            If _isDbsRegionChanged Then
               Me.UpdateDbsRegion(_dbsRegion)
            End If

            '
            ' DbsRegionDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteDbsRegionDetails(_dbsRegionDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsRegionDetails(_dbsRegionDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsRegionDetails(_dbsRegionDetailList)
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

   <SymAuthorization("RemoveRegion")>
   <Route("regions/{regionId}")>
   <HttpDelete>
   Public Function RemoveRegion(regionId As String, <FromUri> q As DeleteQuery) As IHttpActionResult

      If String.IsNullOrEmpty(regionId) Then
         Throw New ArgumentException("Region ID is required.")
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

            Me.DeleteDbsRegion(regionId, q.LockId)

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

   Private Sub LoadDbsRegion(region As DbsRegionBody, dbsRegion As DbsRegion)

      DataLib.ScatterValues(region, dbsRegion)

   End Sub

   Private Sub LoadDbsRegion(row As DataRow, region As DbsRegion)

      With region
         .RegionId = row.ToString("RegionId")
         .RegionName = row.ToString("RegionName")
      End With

   End Sub

   Private Sub LoadDbsRegionDetailList(rows As DataRowCollection, list As DbsRegionDetailList)

      Dim _detail As DbsRegionDetail
      For Each _row As DataRow In rows
         _detail = New DbsRegionDetail

         With _detail
            .ProvinceId = _row.ToString("ProvinceId")
            .ProvinceName = _row.ToString("ProvinceName")
            .RegionId = _row.ToString("RegionId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsRegion(region As DbsRegion)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRegion", region, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRegion(region)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsRegionDetails(list As DbsRegionDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
            .Add("LogActionId")

            .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsProvince", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsRegionDetail In list
            Me.AddInsertUpdateParamsDbsRegionDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsRegion(region As DbsRegion)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("RegionId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRegion", region, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRegion(region)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(region.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsRegionDetails(list As DbsRegionDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ProvinceId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsProvince", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsRegionDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsRegionDetail(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsRegion(region As DbsRegion)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@RegionId", region.RegionId)
         .AddWithValue("@RegionName", region.RegionName)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsRegionDetail(dtl As DbsRegionDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ProvinceId", dtl.ProvinceId)
         .AddWithValue("@ProvinceName", dtl.ProvinceName)
         .AddWithValue("@RegionId", dtl.RegionId)

      End With

   End Sub

   Private Sub DeleteDbsRegion(regionId As String, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RegionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsRegion", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RegionId", regionId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsRegionDetails(list As DbsRegionDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsProvince WHERE ProvinceId=@ProvinceId"
         .CommandType = CommandType.Text

         For Each _old As DbsRegionDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ProvinceId", _old.ProvinceId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasDbsRegionChanges(oldRecord As DbsRegion, newRecord As DbsRegion) As Boolean

      With oldRecord
         If .RegionName <> newRecord.RegionName Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsRegionBody
   Inherits DbsRegion

   Public Property Details As DbsRegionDetail()

End Class

