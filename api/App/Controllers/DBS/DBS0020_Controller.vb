<RoutePrefix("api")>
Public Class DBS0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPlatforms")>
   <Route("platforms")>
   <HttpGet>
   Public Function GetPlatforms() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0020_All")
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

   <SymAuthorization("GetPlatform")>
   <Route("platforms/{platformId}")>
   <HttpGet>
   Public Function GetPlatform(platformId As Integer) As IHttpActionResult

      If platformId <= 0 Then
         Throw New ArgumentException("Platform ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0020")
            With _direct
               .AddParameter("PlatformId", platformId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePlatform")>
   <Route("platforms/{currentUserId}")>
   <HttpPost>
   Public Function CreatePlatform(currentUserId As Integer, <FromBody> platform As DbsPlatformBody) As IHttpActionResult

      If platform.PlatformId <> -1 Then
         Throw New ArgumentException("Platform ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _platformId As Integer = SysLib.GetNextSequence("PlatformId")

         platform.PlatformId = _platformId

         '
         ' Load proposed values from payload
         '
         Dim _dbsPlatform As New DbsPlatform

         Me.LoadDbsPlatform(platform, _dbsPlatform)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsPlatform(_dbsPlatform)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPlatform(_platformId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPlatform")>
   <Route("platforms/{platformId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPlatform(platformId As Integer, currentUserId As Integer, <FromBody> platform As DbsPlatformBody) As IHttpActionResult

      If platformId <= 0 Then
         Throw New ArgumentException("Platform ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsPlatform As New DbsPlatform

         Me.LoadDbsPlatform(platform, _dbsPlatform)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsPlatform(_dbsPlatform)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPlatform(platformId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePlatform")>
   <Route("platforms/{platformId}")>
   <HttpDelete>
   Public Function RemovePlatform(platformId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If platformId <= 0 Then
         Throw New ArgumentException("Platform ID is required.")
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

            Me.DeleteDbsPlatform(platformId, q.LockId)

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

   Private Sub LoadDbsPlatform(platform As DbsPlatformBody, dbsPlatform As DbsPlatform)

      DataLib.ScatterValues(platform, dbsPlatform)

   End Sub

   Private Sub LoadDbsPlatform(row As DataRow, dbsPlatform As DbsPlatform)

      With dbsPlatform
         .PlatformId = row.ToInt32("PlatformId")
         .PlatformName = row.ToString("PlatformName")
         .PlatformShortName = row.ToString("PlatformShortName")
      End With

   End Sub

   Private Sub InsertDbsPlatform(platform As DbsPlatform)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsPlatform", platform, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsPlatform(platform)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsPlatform(platform As DbsPlatform)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PlatformId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsPlatform", platform, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsPlatform(platform)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(platform.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsPlatform(platform As DbsPlatform)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PlatformId", platform.PlatformId)
         .AddWithValue("@PlatformName", platform.PlatformName)
         .AddWithValue("@PlatformShortName", platform.PlatformShortName)

      End With

   End Sub

   Private Sub DeleteDbsPlatform(platformId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PlatformId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsPlatform", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PlatformId", platformId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsPlatformBody
   Inherits DbsPlatform

End Class
