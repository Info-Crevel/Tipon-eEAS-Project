<RoutePrefix("api")>
Public Class APS0540_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApsDocTypes")>
   <Route("aps-doc-types")>
   <HttpGet>
   Public Function GetApsDocTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0540_All")
            With _direct
               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetApsDocType")>
   <Route("aps-doc-types/{apsDocTypeId}")>
   <HttpGet>
   Public Function GetApsDocType(apsDocTypeId As Integer) As IHttpActionResult

      If apsDocTypeId <= 0 Then
         Throw New ArgumentException("ApsDoc Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0540")
            With _direct
               .AddParameter("ApsDocTypeId", apsDocTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApsDocType")>
   <Route("aps-doc-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateApsDocType(currentUserId As Integer, <FromBody> apsDocType As ApsDocTypeBody) As IHttpActionResult

      If apsDocType.ApsDocTypeId <> -1 Then
         Throw New ArgumentException("ApsDoc Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _apsDocTypeId As Integer = SysLib.GetNextSequence("ApsDocTypeId")

         apsDocType.ApsDocTypeId = _apsDocTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsApsDocType As New ApsDocType

         Me.LoadApsDocType(apsDocType, _dbsApsDocType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsDocType(_dbsApsDocType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApsDocType(_apsDocTypeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApsDocType")>
   <Route("aps-doc-types/{apsDocTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApsDocType(apsDocTypeId As Integer, currentUserId As Integer, <FromBody> ApsDocType As ApsDocTypeBody) As IHttpActionResult

      If apsDocTypeId <= 0 Then
         Throw New ArgumentException("Aps Doc Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsApsDocType As New ApsDocType

         Me.LoadApsDocType(ApsDocType, _dbsApsDocType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsDocType(_dbsApsDocType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApsDocType(apsDocTypeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveApsDocType")>
   <Route("aps-doc-types/{apsDocTypeId}")>
   <HttpDelete>
   Public Function RemoveApsDocType(apsDocTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If apsDocTypeId <= 0 Then
         Throw New ArgumentException("Aps Doc Type ID is required.")
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

            Me.DeleteApsDocType(apsDocTypeId, q.LockId)

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

   Private Sub LoadApsDocType(apsDocType As ApsDocTypeBody, dbsApsDocType As ApsDocType)

      DataLib.ScatterValues(apsDocType, dbsApsDocType)

   End Sub

   Private Sub LoadApsDocType(row As DataRow, dbsApsDocType As ApsDocType)

      With dbsApsDocType
         .ApsDocTypeId = row.ToInt32("ApsDocTypeId")
         .DocTypeName = row.ToString("ApsDocTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertApsDocType(apsDocType As ApsDocType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsDocType", apsDocType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsDocType(apsDocType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateApsDocType(ApsDocType As ApsDocType)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ApsDocTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsDocType", ApsDocType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsDocType(ApsDocType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(ApsDocType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsDocType(apsDocType As ApsDocType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApsDocTypeId", apsDocType.ApsDocTypeId)
         .AddWithValue("@DocTypeName", apsDocType.DocTypeName)
         .AddWithValue("@SortSeq", apsDocType.SortSeq)

      End With

   End Sub

   Private Sub DeleteApsDocType(apsDocTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApsDocTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsDocType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApsDocTypeId", apsDocTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsDocTypeBody
   Inherits ApsDocType

End Class

