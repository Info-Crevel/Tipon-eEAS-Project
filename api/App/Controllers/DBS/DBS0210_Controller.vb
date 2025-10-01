<RoutePrefix("api")>
Public Class DBS0210_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetNCIIQualificationTitles")>
   <Route("ncii-qualification-titles")>
   <HttpGet>
   Public Function GetNCIIQualificationTitles() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0210_All")
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

   <SymAuthorization("GetNCIIQualificationTitle")>
   <Route("ncii-qualification-titles/{nCIIQualificationTitleId}")>
   <HttpGet>
   Public Function GetNCIIQualificationTitle(nCIIQualificationTitleId As Integer) As IHttpActionResult

      If nCIIQualificationTitleId <= 0 Then
         Throw New ArgumentException("NCII Qualification Title ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0210")
            With _direct
               .AddParameter("NCIIQualificationTitleId", nCIIQualificationTitleId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateNCIIQualificationTitle")>
   <Route("ncii-qualification-titles/{currentUserId}")>
   <HttpPost>
   Public Function CreateNCIIQualificationTitle(currentUserId As Integer, <FromBody> nCIIQualificationTitle As DbsNCIIQualificationTitleBody) As IHttpActionResult

      If nCIIQualificationTitle.NCIIQualificationTitleId <> -1 Then
         Throw New ArgumentException("NCII Qualification Title ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _nCIIQualificationTitleId As Integer = SysLib.GetNextSequence("NCIIQualificationTitleId")

         nCIIQualificationTitle.NCIIQualificationTitleId = _nCIIQualificationTitleId

         '
         ' Load proposed values from payload
         '
         Dim _dbsNCIIQualificationTitle As New DbsNCIIQualificationTitle

         Me.LoadDbsNCIIQualificationTitle(nCIIQualificationTitle, _dbsNCIIQualificationTitle)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsNCIIQualificationTitle(_dbsNCIIQualificationTitle)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetNCIIQualificationTitle(_nCIIQualificationTitleId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(nCIIQualificationTitle.NCIIQualificationTitleId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyNCIIQualificationTitle")>
   <Route("ncii-qualification-titles/{nCIIQualificationTitleId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyNCIIQualificationTitle(nCIIQualificationTitleId As Integer, currentUserId As Integer, <FromBody> nCIIQualificationTitle As DbsNCIIQualificationTitleBody) As IHttpActionResult

      If nCIIQualificationTitleId <= 0 Then
         Throw New ArgumentException("NCII Qualification Title ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsNCIIQualificationTitle As New DbsNCIIQualificationTitle

         Me.LoadDbsNCIIQualificationTitle(nCIIQualificationTitle, _dbsNCIIQualificationTitle)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsNCIIQualificationTitle(_dbsNCIIQualificationTitle)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetNCIIQualificationTitle(nCIIQualificationTitleId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveNCIIQualificationTitle")>
   <Route("ncii-qualification-titles/{nCIIQualificationTitleId}")>
   <HttpDelete>
   Public Function RemoveNCIIQualificationTitle(nCIIQualificationTitleId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If nCIIQualificationTitleId <= 0 Then
         Throw New ArgumentException("NCII Qualification Title ID is required.")
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

            Me.DeleteDbsNCIIQualificationTitle(nCIIQualificationTitleId, q.LockId)

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

   Private Sub LoadDbsNCIIQualificationTitle(nCIIQualificationTitle As DbsNCIIQualificationTitleBody, dbsNCIIQualificationTitle As DbsNCIIQualificationTitle)

      DataLib.ScatterValues(nCIIQualificationTitle, dbsNCIIQualificationTitle)

   End Sub

   Private Sub LoadDbsNCIIQualificationTitle(row As DataRow, dbsNCIIQualificationTitle As DbsNCIIQualificationTitle)

      With dbsNCIIQualificationTitle
         .NCIIQualificationTitleId = row.ToInt32("NCIIQualificationTitleId")
         .NCIIQualificationTitleName = row.ToString("NCIIQualificationTitleName")
         .UploadRequiredFlag = row.ToBoolean("UploadRequiredFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsNCIIQualificationTitle(nCIIQualificationTitle As DbsNCIIQualificationTitle)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsNCIIQualificationTitle", nCIIQualificationTitle, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsNCIIQualificationTitle(nCIIQualificationTitle)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsNCIIQualificationTitle(nCIIQualificationTitle As DbsNCIIQualificationTitle)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("NCIIQualificationTitleId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsNCIIQualificationTitle", nCIIQualificationTitle, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsNCIIQualificationTitle(nCIIQualificationTitle)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(nCIIQualificationTitle.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsNCIIQualificationTitle(nCIIQualificationTitle As DbsNCIIQualificationTitle)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@NCIIQualificationTitleId", nCIIQualificationTitle.NCIIQualificationTitleId)
         .AddWithValue("@NCIIQualificationTitleName", nCIIQualificationTitle.NCIIQualificationTitleName)
         .AddWithValue("@UploadRequiredFlag", nCIIQualificationTitle.UploadRequiredFlag)
         .AddWithValue("@SortSeq", nCIIQualificationTitle.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsNCIIQualificationTitle(nCIIQualificationTitleId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("NCIIQualificationTitleId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsNCIIQualificationTitle", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@NCIIQualificationTitleId", nCIIQualificationTitleId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsNCIIQualificationTitleBody
   Inherits DbsNCIIQualificationTitle

End Class
