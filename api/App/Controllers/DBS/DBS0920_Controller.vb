<RoutePrefix("api")>
Public Class DBS0920_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0920")>
   <Route("references/dbs0920")>
   <HttpGet>
   Public Function GetReferences_DBS0920() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0920_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "orgs"
                     .Tables(1).TableName = "platforms"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetCluster")>
   <Route("clusters/{clusterId}")>
   <HttpGet>
   Public Function GetDbsCluster(clusterId As Integer) As IHttpActionResult

      If clusterId <= 0 Then
         Throw New ArgumentException("Cluster ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0920")
            With _direct
               .AddParameter("ClusterId", clusterId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "cluster"
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

   <SymAuthorization("CreateCluster")>
   <Route("clusters/{currentUserId}")>
   <HttpPost>
   Public Function CreateCluster(currentUserId As Integer, <FromBody> cluster As DbsClusterBody) As IHttpActionResult

      If cluster.ClusterId <> -1 Then
         Throw New ArgumentException("Cluster ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _clusterId As Integer = SysLib.GetNextSequence("ClusterId")

         cluster.ClusterId = _clusterId

         '
         ' Load proposed values from payload
         '
         Dim _dbsCluster As New DbsCluster
         Dim _dbsClusterOrgPlatformList As New DbsClusterOrgPlatformList

         Me.LoadDbsCluster(cluster, _dbsCluster)

         For Each _detail As DbsClusterOrgPlatform In cluster.Details
            _detail.ClusterId = _clusterId
            _dbsClusterOrgPlatformList.Add(_detail)
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

            Me.InsertDbsCluster(_dbsCluster)

            If _dbsClusterOrgPlatformList.Count > 0 Then
               Me.InsertDbsClusterOrgPlatforms(_dbsClusterOrgPlatformList)
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
         Return Me.Ok(cluster.ClusterId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyCluster")>
   <Route("clusters/{clusterId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCluster(clusterId As Integer, currentUserId As Integer, <FromBody> cluster As DbsClusterBody) As IHttpActionResult

      If clusterId <= 0 Then
         Throw New ArgumentException("Cluster ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsCluster As New DbsCluster
         Dim _dbsClusterOrgPlatformList As New DbsClusterOrgPlatformList

         Me.LoadDbsCluster(cluster, _dbsCluster)

         For Each _detail As DbsClusterOrgPlatform In cluster.Details
            _dbsClusterOrgPlatformList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsClusterOld As New DbsCluster
         Dim _dbsClusterOrgPlatformListOld As New DbsClusterOrgPlatformList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsCluster(clusterId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("cluster").Rows(0)
            Me.LoadDbsCluster(_row, _dbsClusterOld)
            Me.LoadDbsClusterOrgPlatformList(_dataSet.Tables("details").Rows, _dbsClusterOrgPlatformListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsClusterOrgPlatform In _dbsClusterOrgPlatformListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsClusterOrgPlatform In _dbsClusterOrgPlatformList
               If _new.ClusterOrgPlatformId = _old.ClusterOrgPlatformId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsClusterOrgPlatform In _dbsClusterOrgPlatformList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsClusterOrgPlatform In _dbsClusterOrgPlatformListOld
               If _new.ClusterOrgPlatformId = _old.ClusterOrgPlatformId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .OrgId <> _old.OrgId OrElse .PlatformId <> _old.PlatformId OrElse .AccountId <> _old.AccountId Then
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

         Dim _dbsClusterOrgPlatformListNew As New DbsClusterOrgPlatformList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsClusterOrgPlatform As DbsClusterOrgPlatform

            For Each _new As DbsClusterOrgPlatform In _dbsClusterOrgPlatformList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsClusterOrgPlatform = New DbsClusterOrgPlatform
                  _dbsClusterOrgPlatformListNew.Add(_dbsClusterOrgPlatform)
                  DataLib.ScatterValues(_new, _dbsClusterOrgPlatform)
                  _dbsClusterOrgPlatform.ClusterId = _dbsCluster.ClusterId
               End If
            Next

         End If


         Dim _isDbsClusterChanged As Boolean = Me.HasDbsClusterChanges(_dbsClusterOld, _dbsCluster)

         If Not _isDbsClusterChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsCluster(clusterId)
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

            If _isDbsClusterChanged Then
               Me.UpdateDbsCluster(_dbsCluster)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsClusterOrgPlatforms(_dbsClusterOrgPlatformListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsClusterOrgPlatforms(_dbsClusterOrgPlatformListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsClusterOrgPlatforms(_dbsClusterOrgPlatformList)

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

   <SymAuthorization("RemoveCluster")>
   <Route("clusters/{clusterId}")>
   <HttpDelete>
   Public Function RemoveCluster(clusterId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If clusterId <= 0 Then
         Throw New ArgumentException("Cluster ID is required.")
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

            Me.DeleteDbsCluster(clusterId, q.LockId)

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

   Private Sub LoadDbsCluster(cluster As DbsClusterBody, dbsCluster As DbsCluster)

      DataLib.ScatterValues(cluster, dbsCluster)

   End Sub

   Private Sub LoadDbsCluster(row As DataRow, cluster As DbsCluster)

      With cluster
         .ClusterId = row.ToInt32("ClusterId")
         .ClusterName = row.ToString("ClusterName")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub LoadDbsClusterOrgPlatformList(rows As DataRowCollection, list As DbsClusterOrgPlatformList)

      Dim _detail As DbsClusterOrgPlatform
      For Each _row As DataRow In rows
         _detail = New DbsClusterOrgPlatform

         With _detail
            .ClusterOrgPlatformId = _row.ToInt32("ClusterOrgPlatformId")
            .ClusterId = _row.ToInt32("ClusterId")
            .OrgId = _row.ToInt32("OrgId")
            .PlatformId = _row.ToInt32("PlatformId")
            .AccountId = _row.ToString("AccountId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsCluster(cluster As DbsCluster)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsCluster", cluster, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCluster(cluster)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsClusterOrgPlatforms(list As DbsClusterOrgPlatformList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ClusterOrgPlatformId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsClusterOrgPlatform", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsClusterOrgPlatform In list
            Me.AddInsertUpdateParamsDbsClusterOrgPlatform(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsCluster(cluster As DbsCluster)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClusterId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsCluster", cluster, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCluster(cluster)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(cluster.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsClusterOrgPlatforms(list As DbsClusterOrgPlatformList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ClusterOrgPlatformId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsClusterOrgPlatform", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsClusterOrgPlatform In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsClusterOrgPlatform(_detail)
               .Parameters.AddWithValue("@ClusterOrgPlatformId", _detail.ClusterOrgPlatformId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub
   Private Sub AddInsertUpdateParamsDbsCluster(cluster As DbsCluster)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ClusterId", cluster.ClusterId)
         .AddWithValue("@ClusterName", cluster.ClusterName)
         .AddWithValue("@AccountId", cluster.AccountId.ToNullable)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsClusterOrgPlatform(dtl As DbsClusterOrgPlatform)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ClusterId", dtl.ClusterId)
         .AddWithValue("@OrgId", dtl.OrgId)
         .AddWithValue("@PlatformId", dtl.PlatformId)
         .AddWithValue("@AccountId", dtl.AccountId.ToNullable)
      End With

   End Sub
   Private Sub DeleteDbsCluster(clusterId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ClusterId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsCluster", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ClusterId", clusterId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsClusterOrgPlatforms(list As DbsClusterOrgPlatformList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsClusterOrgPlatform WHERE ClusterOrgPlatformId=@ClusterOrgPlatformId"
         .CommandType = CommandType.Text

         For Each _old As DbsClusterOrgPlatform In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ClusterOrgPlatformId", _old.ClusterOrgPlatformId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Function HasDbsClusterChanges(oldRecord As DbsCluster, newRecord As DbsCluster) As Boolean

      With oldRecord
         If .ClusterName <> newRecord.ClusterName Then Return True
         If .AccountId <> newRecord.AccountId Then Return True
      End With

      Return False

   End Function

End Class

Public Class DbsClusterBody
   Inherits DbsCluster

   Public Property Details As DbsClusterOrgPlatform()

End Class
