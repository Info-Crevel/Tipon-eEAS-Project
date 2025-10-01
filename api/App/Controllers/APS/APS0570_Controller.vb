<RoutePrefix("api")>
Public Class APS0570_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApsOrg")>
   <Route("aps-orgs/{orgId}")>
   <HttpGet>
   Public Function GetApsOrg(orgId As Integer) As IHttpActionResult

      If orgId <= 0 Then
         Throw New ArgumentException("Org ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0570")
            With _direct
               .AddParameter("OrgId", orgId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "org"
                     .Tables(1).TableName = "orgDetails"
                     .Tables(2).TableName = "requestTrxTypes"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApsOrg")>
   <Route("aps-orgs/{currentUserId}")>
   <HttpPost>
   Public Function CreateApsOrg(currentUserId As Integer, <FromBody> org As ApsOrgBody) As IHttpActionResult

      If org.OrgId <> -1 Then
         Throw New ArgumentException("Org ID is unrecognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID
         '
         Dim _orgId As Integer = org.OrgId

         '
         ' Load proposed values from payload
         '
         Dim _secOrg As New SecOrg
         Dim _apsOrgTrxlList As New ApsOrgTrxList

         Me.LoadApsOrg(org, _secOrg)

         For Each _detail As ApsOrgTrx In org.Details
            '_detail.MunicipalityId = _municipalityId
            _apsOrgTrxlList.Add(_detail)
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

            Me.InsertApsOrg(_secOrg)
            Me.InsertApsOrgTrxs(_apsOrgTrxlList)

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

   <SymAuthorization("ModifyApsOrg")>
   <Route("aps-orgs/{orgId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyOrg(orgId As Integer, currentUserId As Integer, <FromBody> org As ApsOrgBody) As IHttpActionResult

      If orgId <= 0 Then
         Throw New ArgumentException("Org ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _secOrg As New SecOrg
         Dim _apsOrgTrxList As New ApsOrgTrxList

         Me.LoadApsOrg(org, _secOrg)

         For Each _detail As ApsOrgTrx In org.Details
            _apsOrgTrxList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _secOrgOld As New SecOrg
         Dim _apsOrgTrxListOld As New ApsOrgTrxList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetApsOrg(orgId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("org").Rows(0)
            Me.LoadApsOrg(_row, _secOrgOld)
            Me.LoadApsOrgTrxList(_dataSet.Tables("orgDetails").Rows, _apsOrgTrxListOld)
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

         For Each _old As ApsOrgTrx In _apsOrgTrxListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ApsOrgTrx In _apsOrgTrxList
               If _new.OrgTrxId = _old.OrgTrxId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ApsOrgTrx In _apsOrgTrxList
            _new.LogActionId = LogActionId.Add
            For Each _old As ApsOrgTrx In _apsOrgTrxListOld
               If _new.OrgTrxId = _old.OrgTrxId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .OrgId <> _old.OrgId OrElse .RequestTrxName <> _old.RequestTrxName OrElse .RequestTrxTypeId <> _old.RequestTrxTypeId OrElse .AccountId <> _old.AccountId Then
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

         Dim _apsOrgTrxListNew As New ApsOrgTrxList      ' for adding new Barangays

         If _addDetailCount > 0 Then
            Dim _apsOrgTrx As ApsOrgTrx

            For Each _new As ApsOrgTrx In _apsOrgTrxList
               If _new.LogActionId = LogActionId.Add Then
                  _apsOrgTrx = New ApsOrgTrx
                  _apsOrgTrxListNew.Add(_apsOrgTrx)
                  DataLib.ScatterValues(_new, _apsOrgTrx)
                  '_dbsMunicipalityDetail.MunicipalityId = _dbsMunicipality.MunicipalityId       ' not needed
               End If
            Next

         End If


         Dim _isSecOrgChanged As Boolean = Me.HasApsOrgChanges(_secOrgOld, _secOrg)


         If Not _isSecOrgChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetApsOrg(orgId)
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


            If _isSecOrgChanged Then

               Me.UpdateApsOrg(_secOrg)

            End If

            '
            ' DbsMunicipalityDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteApsOrgTrxs(_apsOrgTrxListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertApsOrgTrxs(_apsOrgTrxListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateApsOrgTrxs(_apsOrgTrxList)
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

   <SymAuthorization("RemoveApsOrg")>
   <Route("aps-orgs/{orgId}")>
   <HttpDelete>
   Public Function RemoveApsOrg(orgId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If orgId <= 0 Then
         Throw New ArgumentException("Org ID is required.")
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

            Me.DeleteApsOrg(orgId, q.LockId)

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
   Private Sub LoadApsOrg(org As ApsOrgBody, secOrg As SecOrg)

      DataLib.ScatterValues(org, secOrg)

   End Sub

   Private Sub LoadApsOrg(row As DataRow, org As SecOrg)

      With org
         .OrgId = row.ToInt32("OrgId")
         .OrgName = row.ToString("OrgName")
         .OrgShortName = row.ToString("OrgShortName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub LoadApsOrgTrxList(rows As DataRowCollection, list As ApsOrgTrxList)

      Dim _detail As ApsOrgTrx
      For Each _row As DataRow In rows
         _detail = New ApsOrgTrx

         With _detail
            .OrgTrxId = _row.ToInt32("OrgTrxId")
            .OrgId = _row.ToInt32("OrgId")
            .RequestTrxName = _row.ToString("RequestTrxName")
            .RequestTrxTypeId = _row.ToInt32("RequestTrxTypeId")
            .AccountId = _row.ToString("AccountId")

         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertApsOrg(org As SecOrg)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.SecOrg", org, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsOrg(org)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertApsOrgTrxs(list As ApsOrgTrxList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("OrgTrxId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsOrgTrx", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ApsOrgTrx In list
            Me.AddInsertUpdateParamsApsOrgTrx(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateApsOrg(org As SecOrg)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("OrgId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.SecOrg", org, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsOrg(org)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(org.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateApsOrgTrxs(list As ApsOrgTrxList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("OrgTrxId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsOrgTrx", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ApsOrgTrx In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsApsOrgTrx(_detail)
               .Parameters.AddWithValue("@OrgTrxId", _detail.OrgTrxId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsApsOrg(org As SecOrg)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@OrgId", org.OrgId)
         .AddWithValue("@OrgName", org.OrgName)
         .AddWithValue("@OrgShortName", org.OrgShortName)
         .AddWithValue("@SortSeq", org.SortSeq)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsOrgTrx(dtl As ApsOrgTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@OrgId", dtl.OrgId)
         .AddWithValue("@RequestTrxName", dtl.RequestTrxName)
         .AddWithValue("@RequestTrxTypeId", dtl.RequestTrxTypeId)
         .AddWithValue("@AccountId", dtl.AccountId)

      End With

   End Sub

   Private Sub DeleteApsOrg(orgId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("OrgId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.SecOrg", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@OrgId", orgId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteApsOrgTrxs(list As ApsOrgTrxList)

      With DataCore.Command
         .CommandText = "DELETE dbo. ApsOrgTrx WHERE OrgTrxId=@OrgTrxId"
         .CommandType = CommandType.Text

         For Each _old As ApsOrgTrx In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@OrgTrxId", _old.OrgTrxId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasApsOrgChanges(oldRecord As SecOrg, newRecord As SecOrg) As Boolean

      With oldRecord
         If .OrgName <> newRecord.OrgName Then Return True
         If .OrgId <> newRecord.OrgId Then Return True
         If .OrgShortName <> newRecord.OrgShortName Then Return True
      End With

      Return False

   End Function

End Class

Public Class ApsOrgBody
   Inherits SecOrg

   Public Property Details As ApsOrgTrx()

End Class

