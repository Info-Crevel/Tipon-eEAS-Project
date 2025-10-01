<RoutePrefix("api")>
Public Class DBS0110_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetDocTypes")>
   <Route("doc-types")>
   <HttpGet>
   Public Function GetDocTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0110_All")
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

   <SymAuthorization("GetDocType")>
   <Route("doc-types/{docTypeId}")>
   <HttpGet>
   Public Function GetDocType(docTypeId As Integer) As IHttpActionResult

      If docTypeId <= 0 Then
         Throw New ArgumentException("Doc Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0110")
            With _direct
               .AddParameter("DocTypeId", docTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateDocType")>
   <Route("doc-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateDocType(currentUserId As Integer, <FromBody> docType As DbsDocTypeBody) As IHttpActionResult

      If docType.DocTypeId <> -1 Then
         Throw New ArgumentException("Doc Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _docTypeId As Integer = SysLib.GetNextSequence("DocTypeId")

         docType.DocTypeId = _docTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsDocType As New DbsDocType

         Me.LoadDbsDocType(docType, _dbsDocType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsDocType(_dbsDocType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDocType(_docTypeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyDocType")>
   <Route("doc-types/{docTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyDocType(docTypeId As Integer, currentUserId As Integer, <FromBody> DocType As DbsDocTypeBody) As IHttpActionResult

      If docTypeId <= 0 Then
         Throw New ArgumentException("Doc Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsDocType As New DbsDocType

         Me.LoadDbsDocType(DocType, _dbsDocType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsDocType(_dbsDocType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDocType(docTypeId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveDocType")>
   <Route("doc-types/{docTypeId}")>
   <HttpDelete>
   Public Function RemoveDocType(docTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If docTypeId <= 0 Then
         Throw New ArgumentException("Doc Type ID is required.")
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

            Me.DeleteDbsDocType(docTypeId, q.LockId)

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

   Private Sub LoadDbsDocType(docType As DbsDocTypeBody, dbsDocType As DbsDocType)

      DataLib.ScatterValues(docType, dbsDocType)

   End Sub

   Private Sub LoadDbsDocType(row As DataRow, dbsDocType As DbsDocType)

      With dbsDocType
         .DocTypeId = row.ToInt32("DocTypeId")
         .DocTypeName = row.ToString("DocTypeName")
         .DocTypeLength = row.ToInt32("DocTypeLength")

         .SortSeq = row.ToInt32("SortSeq")
            .ApplicantFlag = row.ToBoolean("ApplicantFlag")
            .ContractFlag = row.ToBoolean("ContractFlag")
            .TradeTestFlag = row.ToBoolean("TradeTestFlag")
            .ExpirationFlag = row.ToBoolean("ExpirationFlag")
            .MemberFlag = row.ToBoolean("MemberFlag")
      End With

   End Sub

   Private Sub InsertDbsDocType(docType As DbsDocType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
         .Add("DocTypeFormat")
         '.Add("DocTypeLength")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsDocType", docType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDocType(docType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsDocType(DocType As DbsDocType)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("DocTypeId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("DocTypeFormat")
         '.Add("DocTypeLength")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsDocType", DocType, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDocType(DocType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(DocType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsDocType(docType As DbsDocType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DocTypeId", docType.DocTypeId)
         .AddWithValue("@DocTypeName", docType.DocTypeName)
         .AddWithValue("@DocTypeLength", docType.DocTypeLength.ToNullable)

         .AddWithValue("@SortSeq", docType.SortSeq)
            .AddWithValue("@ApplicantFlag", docType.ApplicantFlag)
            .AddWithValue("@ContractFlag", docType.ContractFlag)
            .AddWithValue("@TradeTestFlag", docType.TradeTestFlag)
            .AddWithValue("@ExpirationFlag", docType.ExpirationFlag)
            .AddWithValue("@MemberFlag", docType.MemberFlag)

      End With

   End Sub

   Private Sub DeleteDbsDocType(docTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DocTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsDocType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DocTypeId", docTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsDocTypeBody
   Inherits DbsDocType

End Class

