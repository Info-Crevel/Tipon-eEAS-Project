<RoutePrefix("api")>
Public Class DBS0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReligions")>
   <Route("religions")>
   <HttpGet>
   Public Function GetReligions() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0010_All")
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

   <SymAuthorization("GetReligion")>
   <Route("religions/{religionId}")>
   <HttpGet>
   Public Function GetReligion(religionId As Integer) As IHttpActionResult

      If religionId <= 0 Then
         Throw New ArgumentException("Religion ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0010")
            With _direct
               .AddParameter("ReligionId", religionId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateReligion")>
   <Route("religions/{currentUserId}")>
   <HttpPost>
   Public Function CreateReligion(currentUserId As Integer, <FromBody> religion As DbsReligionBody) As IHttpActionResult

      If religion.ReligionId <> -1 Then
         Throw New ArgumentException("Religion ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _religionId As Integer = SysLib.GetNextSequence("ReligionId")

         religion.ReligionId = _religionId

         '
         ' Load proposed values from payload
         '
         Dim _dbsReligion As New DbsReligion

         Me.LoadDbsReligion(religion, _dbsReligion)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsReligion(_dbsReligion)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetReligion(_religionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(religion.ReligionId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyReligion")>
   <Route("religions/{religionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyReligion(religionId As Integer, currentUserId As Integer, <FromBody> religion As DbsReligionBody) As IHttpActionResult

      If religionId <= 0 Then
         Throw New ArgumentException("Religion ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsReligion As New DbsReligion

         Me.LoadDbsReligion(religion, _dbsReligion)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsReligion(_dbsReligion)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetReligion(religionId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   '<SymAuthorization("RemoveReligion")>
   '<Route("religions/{religionId}/{lockId}/{currentUserId}")>
   '<HttpDelete>
   'Public Function RemoveReligion(religionId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

   '   If religionId <= 0 Then
   '      Throw New ArgumentException("Religion ID is required.")
   '   End If

   '   Try

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.DeleteDbsReligion(religionId, lockId)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   <SymAuthorization("RemoveReligion")>
   <Route("religions/{religionId}")>
   <HttpDelete>
   Public Function RemoveReligion(religionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If religionId <= 0 Then
         Throw New ArgumentException("Religion ID is required.")
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

            'Me.DeleteDbsReligion(religionId, lockId)
            Me.DeleteDbsReligion(religionId, q.LockId)

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

   Private Sub LoadDbsReligion(religion As DbsReligionBody, dbsReligion As DbsReligion)

      DataLib.ScatterValues(religion, dbsReligion)

   End Sub

   Private Sub LoadDbsReligion(row As DataRow, dbsReligion As DbsReligion)

      With dbsReligion
         .ReligionId = row.ToInt32("ReligionId")
         .ReligionName = row.ToString("ReligionName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsReligion(religion As DbsReligion)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsReligion", religion, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsReligion(religion)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsReligion(religion As DbsReligion)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ReligionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsReligion", religion, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsReligion(religion)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(religion.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsReligion(religion As DbsReligion)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ReligionId", religion.ReligionId)
         .AddWithValue("@ReligionName", religion.ReligionName)
         .AddWithValue("@SortSeq", religion.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsReligion(religionId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ReligionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsReligion", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ReligionId", religionId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsReligionBody
   Inherits DbsReligion

End Class
