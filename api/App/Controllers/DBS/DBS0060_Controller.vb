<RoutePrefix("api")>
Public Class DBS0060_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetProvince")>
   <Route("provinces/{provinceId}")>
   <HttpGet>
   Public Function GetProvince(provinceId As String) As IHttpActionResult

      If String.IsNullOrEmpty(provinceId) Then
         Throw New ArgumentException("Province ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0060")
            With _direct
               .AddParameter("ProvinceId", provinceId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "province"
                     .Tables(1).TableName = "provinceDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateProvince")>
   <Route("provinces/{currentUserId}")>
   <HttpPost>
   Public Function CreateProvince(currentUserId As Integer, <FromBody> province As DbsProvinceBody) As IHttpActionResult

      If String.IsNullOrEmpty(province.ProvinceId) Then
         Throw New ArgumentException("Province ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID
         '
         Dim _provinceId As String = province.ProvinceId

         '
         ' Load proposed values from payload
         '
         Dim _dbsProvince As New DbsProvince
         Dim _dbsProvinceDetailList As New DbsProvinceDetailList

         Me.LoadDbsProvince(province, _dbsProvince)

         For Each _detail As DbsProvinceDetail In province.Details
            '_detail.ProvinceId = _provinceId
            _dbsProvinceDetailList.Add(_detail)
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

            Me.InsertDbsProvince(_dbsProvince)
            Me.InsertDbsProvinceDetails(_dbsProvinceDetailList)

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

   <SymAuthorization("ModifyProvince")>
   <Route("provinces/{provinceId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyProvince(provinceId As String, currentUserId As Integer, <FromBody> province As DbsProvinceBody) As IHttpActionResult

      If String.IsNullOrEmpty(provinceId) Then
         Throw New ArgumentException("Province ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsProvince As New DbsProvince
         Dim _dbsProvinceDetailList As New DbsProvinceDetailList

         Me.LoadDbsProvince(province, _dbsProvince)

         For Each _detail As DbsProvinceDetail In province.Details
            _dbsProvinceDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsProvinceOld As New DbsProvince
         Dim _dbsProvinceDetailListOld As New DbsProvinceDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetProvince(provinceId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("province").Rows(0)
            Me.LoadDbsProvince(_row, _dbsProvinceOld)
            Me.LoadDbsProvinceDetailList(_dataSet.Tables("provinceDetails").Rows, _dbsProvinceDetailListOld)
         End Using

         '
         ' Apply changes, save to DB
         '

         '
         ' DbsProvinceDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As DbsProvinceDetail In _dbsProvinceDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsProvinceDetail In _dbsProvinceDetailList
               If _new.MunicipalityId = _old.MunicipalityId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsProvinceDetail In _dbsProvinceDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsProvinceDetail In _dbsProvinceDetailListOld
               If _new.MunicipalityId = _old.MunicipalityId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                            If .MunicipalityName <> _old.MunicipalityName OrElse .PostalCode <> _old.PostalCode Then
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

         Dim _dbsProvinceDetailListNew As New DbsProvinceDetailList      ' for adding new Municipalities

         If _addDetailCount > 0 Then
            Dim _dbsProvinceDetail As DbsProvinceDetail

            For Each _new As DbsProvinceDetail In _dbsProvinceDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsProvinceDetail = New DbsProvinceDetail
                  _dbsProvinceDetailListNew.Add(_dbsProvinceDetail)
                  DataLib.ScatterValues(_new, _dbsProvinceDetail)
                  _dbsProvinceDetail.ProvinceId = _dbsProvince.ProvinceId
               End If
            Next

         End If

         Dim _isDbsProvinceChanged As Boolean = Me.HasDbsProvinceChanges(_dbsProvinceOld, _dbsProvince)

         If Not _isDbsProvinceChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetProvince(provinceId)
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

            If _isDbsProvinceChanged Then
               Me.UpdateDbsProvince(_dbsProvince)
            End If

            '
            ' DbsProvinceDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteDbsProvinceDetails(_dbsProvinceDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsProvinceDetails(_dbsProvinceDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsProvinceDetails(_dbsProvinceDetailList)
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

   <SymAuthorization("RemoveProvince")>
   <Route("provinces/{provinceId}")>
   <HttpDelete>
   Public Function RemoveProvince(provinceId As String, <FromUri> q As DeleteQuery) As IHttpActionResult

      If String.IsNullOrEmpty(provinceId) Then
         Throw New ArgumentException("Province ID is required.")
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

            Me.DeleteDbsProvince(provinceId, q.LockId)

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

   Private Sub LoadDbsProvince(province As DbsProvinceBody, dbsProvince As DbsProvince)

      DataLib.ScatterValues(province, dbsProvince)

   End Sub

   Private Sub LoadDbsProvince(row As DataRow, province As DbsProvince)

      With province
         .ProvinceId = row.ToString("ProvinceId")
         .ProvinceName = row.ToString("ProvinceName")
      End With

   End Sub

   Private Sub LoadDbsProvinceDetailList(rows As DataRowCollection, list As DbsProvinceDetailList)

      Dim _detail As DbsProvinceDetail
      For Each _row As DataRow In rows
         _detail = New DbsProvinceDetail

         With _detail
            .MunicipalityId = _row.ToString("MunicipalityId")
            .MunicipalityName = _row.ToString("MunicipalityName")
            .PostalCode = _row.ToString("PostalCode")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsProvince(province As DbsProvince)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsProvince", province, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsProvince(province)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsProvinceDetails(list As DbsProvinceDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMunicipality", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsProvinceDetail In list
            Me.AddInsertUpdateParamsDbsProvinceDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsProvince(province As DbsProvince)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ProvinceId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsProvince", province, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsProvince(province)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(province.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsProvinceDetails(list As DbsProvinceDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MunicipalityId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMunicipality", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsProvinceDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsProvinceDetail(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsProvince(province As DbsProvince)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ProvinceId", province.ProvinceId)
         .AddWithValue("@ProvinceName", province.ProvinceName)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsProvinceDetail(dtl As DbsProvinceDetail)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MunicipalityId", dtl.MunicipalityId)
         .AddWithValue("@MunicipalityName", dtl.MunicipalityName)
         .AddWithValue("@PostalCode", dtl.PostalCode.ToNullable)
         .AddWithValue("@ProvinceId", dtl.ProvinceId)
      End With

   End Sub

   Private Sub DeleteDbsProvince(provinceId As String, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ProvinceId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsProvince", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ProvincceId", provinceId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsProvinceDetails(list As DbsProvinceDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsMunicipality WHERE MunicipalityId=@MunicipalityId"
         .CommandType = CommandType.Text

         For Each _old As DbsProvinceDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MunicipalityId", _old.MunicipalityId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasDbsProvinceChanges(oldRecord As DbsProvince, newRecord As DbsProvince) As Boolean

      With oldRecord
         If .ProvinceName <> newRecord.ProvinceName Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsProvinceBody
   Inherits DbsProvince

   Public Property Details As DbsProvinceDetail()

End Class
