<RoutePrefix("api")>
Public Class DBS0070_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMunicipality")>
   <Route("municipalities/{municipalityId}")>
   <HttpGet>
   Public Function GetMunicipality(municipalityId As String) As IHttpActionResult

      If String.IsNullOrEmpty(municipalityId) Then
         Throw New ArgumentException("Municipality ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0070")
            With _direct
               .AddParameter("MunicipalityId", municipalityId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "municipality"
                     .Tables(1).TableName = "municipalityDetails"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMunicipality")>
   <Route("municipalities/{currentUserId}")>
   <HttpPost>
   Public Function CreateMunicipality(currentUserId As Integer, <FromBody> municipality As DbsMunicipalityBody) As IHttpActionResult

      If String.IsNullOrEmpty(municipality.MunicipalityId) Then
         Throw New ArgumentException("Municipality ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID
         '
         Dim _municipalityId As String = municipality.MunicipalityId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMunicipality As New DbsMunicipality
         Dim _dbsMunicipalityDetailList As New DbsMunicipalityDetailList

         Me.LoadDbsMunicipality(municipality, _dbsMunicipality)

         For Each _detail As DbsMunicipalityDetail In municipality.Details
            '_detail.MunicipalityId = _municipalityId
            _dbsMunicipalityDetailList.Add(_detail)
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

            Me.InsertDbsMunicipality(_dbsMunicipality)
            Me.InsertDbsMunicipalityDetails(_dbsMunicipalityDetailList)

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
         'Return Me.Ok(municipality.MunicipalityId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMunicipality")>
   <Route("municipalities/{municipalityId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMunicipality(municipalityId As String, currentUserId As Integer, <FromBody> municipality As DbsMunicipalityBody) As IHttpActionResult

      If String.IsNullOrEmpty(municipalityId) Then
         Throw New ArgumentException("Municipality ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsMunicipality As New DbsMunicipality
         Dim _dbsMunicipalityDetailList As New DbsMunicipalityDetailList

         Me.LoadDbsMunicipality(municipality, _dbsMunicipality)

         For Each _detail As DbsMunicipalityDetail In municipality.Details
            _dbsMunicipalityDetailList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsMunicipalityOld As New DbsMunicipality
         Dim _dbsMunicipalityDetailListOld As New DbsMunicipalityDetailList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMunicipality(municipalityId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("municipality").Rows(0)
            Me.LoadDbsMunicipality(_row, _dbsMunicipalityOld)
            Me.LoadDbsMunicipalityDetailList(_dataSet.Tables("municipalityDetails").Rows, _dbsMunicipalityDetailListOld)
         End Using

         '
         ' Apply changes, save to DB
         '

         '
         ' DbsMunicipalityDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As DbsMunicipalityDetail In _dbsMunicipalityDetailListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsMunicipalityDetail In _dbsMunicipalityDetailList
               If _new.BarangayId = _old.BarangayId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsMunicipalityDetail In _dbsMunicipalityDetailList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsMunicipalityDetail In _dbsMunicipalityDetailListOld
               If _new.BarangayId = _old.BarangayId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .BarangayName <> _old.BarangayName OrElse .PostalCode <> _old.PostalCode Then
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

         Dim _dbsMunicipalityDetailListNew As New DbsMunicipalityDetailList      ' for adding new Barangays

         If _addDetailCount > 0 Then
            Dim _dbsMunicipalityDetail As DbsMunicipalityDetail

            For Each _new As DbsMunicipalityDetail In _dbsMunicipalityDetailList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsMunicipalityDetail = New DbsMunicipalityDetail
                  _dbsMunicipalityDetailListNew.Add(_dbsMunicipalityDetail)
                  DataLib.ScatterValues(_new, _dbsMunicipalityDetail)
                  _dbsMunicipalityDetail.MunicipalityId = _dbsMunicipality.MunicipalityId       ' not needed
               End If
            Next

         End If

         Dim _isDbsMunicipalityChanged As Boolean = Me.HasDbsMunicipalityChanges(_dbsMunicipalityOld, _dbsMunicipality)

         If Not _isDbsMunicipalityChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetMunicipality(municipalityId)
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

            If _isDbsMunicipalityChanged Then
               Me.UpdateDbsMunicipality(_dbsMunicipality)
            End If

            '
            ' DbsMunicipalityDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteDbsMunicipalityDetails(_dbsMunicipalityDetailListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsMunicipalityDetails(_dbsMunicipalityDetailListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsMunicipalityDetails(_dbsMunicipalityDetailList)
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

   <SymAuthorization("RemoveMunicipality")>
   <Route("municipalities/{municipalityId}")>
   <HttpDelete>
   Public Function RemoveMunicipality(municipalityId As String, <FromUri> q As DeleteQuery) As IHttpActionResult

      If String.IsNullOrEmpty(municipalityId) Then
         Throw New ArgumentException("Municipality ID is required.")
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

            Me.DeleteDbsMunicipality(municipalityId, q.LockId)

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

   Private Sub LoadDbsMunicipality(municipality As DbsMunicipalityBody, dbsMunicipality As DbsMunicipality)

      DataLib.ScatterValues(municipality, dbsMunicipality)

   End Sub

   Private Sub LoadDbsMunicipality(row As DataRow, municipality As DbsMunicipality)

      With municipality
         .MunicipalityId = row.ToString("MunicipalityId")
         .MunicipalityName = row.ToString("MunicipalityName")
         .ProvinceId = row.ToString("ProvinceId")
         .PostalCode = row.ToString("PostalCode")
      End With

   End Sub

   Private Sub LoadDbsMunicipalityDetailList(rows As DataRowCollection, list As DbsMunicipalityDetailList)

      Dim _detail As DbsMunicipalityDetail
      For Each _row As DataRow In rows
         _detail = New DbsMunicipalityDetail

         With _detail
            .BarangayId = _row.ToString("BarangayId")
            .BarangayName = _row.ToString("BarangayName")
            .PostalCode = _row.ToString("PostalCode")
            .MunicipalityId = _row.ToString("MunicipalityId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsMunicipality(municipality As DbsMunicipality)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMunicipality", municipality, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMunicipality(municipality)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsMunicipalityDetails(list As DbsMunicipalityDetailList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsBarangay", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMunicipalityDetail In list
            Me.AddInsertUpdateParamsDbsMunicipalityDetail(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsMunicipality(municipality As DbsMunicipality)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MunicipalityId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMunicipality", municipality, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMunicipality(municipality)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(municipality.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMunicipalityDetails(list As DbsMunicipalityDetailList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BarangayId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsBarangay", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsMunicipalityDetail In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsMunicipalityDetail(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMunicipality(municipality As DbsMunicipality)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MunicipalityId", municipality.MunicipalityId)
         .AddWithValue("@MunicipalityName", municipality.MunicipalityName)
         .AddWithValue("@ProvinceId", municipality.ProvinceId)
         .AddWithValue("@PostalCode", municipality.PostalCode.ToNullable)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMunicipalityDetail(dtl As DbsMunicipalityDetail)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@BarangayId", dtl.BarangayId)
         .AddWithValue("@BarangayName", dtl.BarangayName)
         .AddWithValue("@PostalCode", dtl.PostalCode.ToNullable)
         .AddWithValue("@MunicipalityId", dtl.MunicipalityId)
      End With

   End Sub

   Private Sub DeleteDbsMunicipality(municipalityId As String, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MunicipalityId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMunicipality", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MunicipalityId", municipalityId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsMunicipalityDetails(list As DbsMunicipalityDetailList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsBarangay WHERE BarangayId=@BarangayId"
         .CommandType = CommandType.Text

         For Each _old As DbsMunicipalityDetail In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BarangayId", _old.BarangayId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasDbsMunicipalityChanges(oldRecord As DbsMunicipality, newRecord As DbsMunicipality) As Boolean

      With oldRecord
         If .MunicipalityName <> newRecord.MunicipalityName Then Return True
            If .ProvinceId <> newRecord.ProvinceId Then Return True
         If .PostalCode <> newRecord.PostalCode Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsMunicipalityBody
   Inherits DbsMunicipality

   Public Property Details As DbsMunicipalityDetail()

End Class

