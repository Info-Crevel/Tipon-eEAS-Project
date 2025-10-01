<RoutePrefix("api")>
Public Class DBS0360_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAffiliations")>
   <Route("affiliations")>
   <HttpGet>
   Public Function GetAffiliations() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0360_All")
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

   <SymAuthorization("GetAffiliation")>
   <Route("affiliations/{affiliationId}")>
   <HttpGet>
   Public Function GetAffiliation(affiliationId As Integer) As IHttpActionResult

      If affiliationId <= 0 Then
         Throw New ArgumentException("Affiliation ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0360")
            With _direct
               .AddParameter("AffiliationId", affiliationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAffiliation")>
   <Route("affiliations/{currentUserId}")>
   <HttpPost>
   Public Function CreateAffiliation(currentUserId As Integer, <FromBody> affiliation As DbsAffiliationBody) As IHttpActionResult

      If affiliation.AffiliationId <> -1 Then
         Throw New ArgumentException("Affiliation ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _affiliationId As Integer = SysLib.GetNextSequence("AffiliationId")

         affiliation.AffiliationId = _affiliationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsAffiliation As New DbsAffiliation

         Me.LoadDbsAffiliation(affiliation, _dbsAffiliation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsAffiliation(_dbsAffiliation)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAffiliation(_affiliationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(affiliation.AffiliationId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAffiliation")>
   <Route("affiliations/{affiliationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAffiliation(affiliationId As Integer, currentUserId As Integer, <FromBody> affiliation As DbsAffiliationBody) As IHttpActionResult

      If affiliationId <= 0 Then
         Throw New ArgumentException("Affiliation ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsAffiliation As New DbsAffiliation

         Me.LoadDbsAffiliation(affiliation, _dbsAffiliation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsAffiliation(_dbsAffiliation)

            _successFlag = True

         Catch _exception As Exception
            'File.WriteAllText("d:\yyy.txt", _exception.Message)
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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAffiliation(affiliationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveAffiliation")>
   <Route("affiliations/{affiliationId}")>
   <HttpDelete>
   Public Function RemoveAffiliation(affiliationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If affiliationId <= 0 Then
         Throw New ArgumentException("Affiliation ID is required.")
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

            Me.DeleteDbsAffiliation(affiliationId, q.LockId)

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

   Private Sub LoadDbsAffiliation(affiliation As DbsAffiliationBody, dbsAffiliation As DbsAffiliation)

      DataLib.ScatterValues(affiliation, dbsAffiliation)

   End Sub

   Private Sub LoadDbsAffiliation(row As DataRow, dbsAffiliation As DbsAffiliation)

      With dbsAffiliation
         .AffiliationId = row.ToInt32("AffiliationId")
         .AffiliationName = row.ToString("AffiliationName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsAffiliation(affiliation As DbsAffiliation)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsAffiliation", affiliation, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAffiliation(affiliation)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsAffiliation(affiliation As DbsAffiliation)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AffiliationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsAffiliation", affiliation, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsAffiliation(affiliation)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(affiliation.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsAffiliation(affiliation As DbsAffiliation)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@AffiliationId", affiliation.AffiliationId)
         .AddWithValue("@AffiliationName", affiliation.AffiliationName)
         .AddWithValue("@SortSeq", affiliation.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsAffiliation(affiliationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AffiliationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsAffiliation", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AffiliationId", affiliationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsAffiliationBody
   Inherits DbsAffiliation

End Class
