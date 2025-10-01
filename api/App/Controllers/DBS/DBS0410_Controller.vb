<RoutePrefix("api")>
Public Class DBS0410_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetLanguages")>
   <Route("languages")>
   <HttpGet>
   Public Function GetLanguages() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0410_All")
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

   <SymAuthorization("GetLanguage")>
   <Route("languages/{languageId}")>
   <HttpGet>
   Public Function GetLanguage(languageId As Integer) As IHttpActionResult

      If languageId <= 0 Then
         Throw New ArgumentException("Language ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0410")
            With _direct
               .AddParameter("LanguageId", languageId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateLanguage")>
   <Route("languages/{currentUserId}")>
   <HttpPost>
   Public Function CreateLanguage(currentUserId As Integer, <FromBody> language As DbsLanguageBody) As IHttpActionResult

      If language.LanguageId <> -1 Then
         Throw New ArgumentException("Language ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _languageId As Integer = SysLib.GetNextSequence("LanguageId")

         language.LanguageId = _languageId

         '
         ' Load proposed values from payload
         '
         Dim _dbsLanguage As New DbsLanguage

         Me.LoadDbsLanguage(language, _dbsLanguage)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsLanguage(_dbsLanguage)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetLanguage(_languageId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(language.LanguageId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyLanguage")>
   <Route("languages/{languageId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyLanguage(languageId As Integer, currentUserId As Integer, <FromBody> language As DbsLanguageBody) As IHttpActionResult

      If languageId <= 0 Then
         Throw New ArgumentException("Language ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsLanguage As New DbsLanguage

         Me.LoadDbsLanguage(language, _dbsLanguage)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsLanguage(_dbsLanguage)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetLanguage(languageId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveLanguage")>
   <Route("languages/{languageId}")>
   <HttpDelete>
   Public Function RemoveLanguage(languageId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If languageId <= 0 Then
         Throw New ArgumentException("Language ID is required.")
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

            Me.DeleteDbsLanguage(languageId, q.LockId)

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

   Private Sub LoadDbsLanguage(language As DbsLanguageBody, dbsLanguage As DbsLanguage)

      DataLib.ScatterValues(language, dbsLanguage)

   End Sub

   Private Sub LoadDbsLanguage(row As DataRow, dbsLanguage As DbsLanguage)

      With dbsLanguage
         .LanguageId = row.ToInt32("LanguageId")
         .LanguageName = row.ToString("LanguageName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsLanguage(language As DbsLanguage)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsLanguage", language, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsLanguage(language)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsLanguage(language As DbsLanguage)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("LanguageId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsLanguage", language, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsLanguage(language)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(language.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsLanguage(language As DbsLanguage)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@LanguageId", language.LanguageId)
         .AddWithValue("@LanguageName", language.LanguageName)
         .AddWithValue("@SortSeq", language.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsLanguage(languageId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("LanguageId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsLanguage", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@LanguageId", languageId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsLanguageBody
   Inherits DbsLanguage

End Class
