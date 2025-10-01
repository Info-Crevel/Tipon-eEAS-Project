<RoutePrefix("api")>
Public Class DBS0180_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetChargingConsiderations")>
   <Route("charging-considerations")>
   <HttpGet>
   Public Function GetChargingConsiderations() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0180_All")
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

   <SymAuthorization("GetChargingConsideration")>
   <Route("charging-considerations/{chargingConsiderationId}")>
   <HttpGet>
   Public Function GetChargingConsideration(chargingConsiderationId As Integer) As IHttpActionResult

      If chargingConsiderationId <= 0 Then
         Throw New ArgumentException("Charging Consideration ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0180")
            With _direct
               .AddParameter("ChargingConsiderationId", chargingConsiderationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateChargingConsideration")>
   <Route("charging-considerations/{currentUserId}")>
   <HttpPost>
   Public Function CreateChargingConsideration(currentUserId As Integer, <FromBody> chargingConsideration As DbsChargingConsiderationBody) As IHttpActionResult

      If chargingConsideration.ChargingConsiderationId <> -1 Then
         Throw New ArgumentException("Charging Consideration ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _chargingConsiderationId As Integer = SysLib.GetNextSequence("ChargingConsiderationId")

         chargingConsideration.ChargingConsiderationId = _chargingConsiderationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsChargingConsideration As New DbsChargingConsideration

         Me.LoadDbsChargingConsideration(chargingConsideration, _dbsChargingConsideration)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsChargingConsideration(_dbsChargingConsideration)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetChargingConsideration(_chargingConsiderationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(ChargingConsideration.ChargingConsiderationId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyChargingConsideration")>
   <Route("charging-considerations/{chargingConsiderationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyChargingConsideration(chargingConsiderationId As Integer, currentUserId As Integer, <FromBody> chargingConsideration As DbsChargingConsiderationBody) As IHttpActionResult

      If chargingConsiderationId <= 0 Then
         Throw New ArgumentException("Charging Consideration ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsChargingConsideration As New DbsChargingConsideration

         Me.LoadDbsChargingConsideration(chargingConsideration, _dbsChargingConsideration)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsChargingConsideration(_dbsChargingConsideration)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetChargingConsideration(chargingConsiderationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveChargingConsideration")>
   <Route("charging-considerations/{chargingConsiderationId}")>
   <HttpDelete>
   Public Function RemoveChargingConsideration(chargingConsiderationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If chargingConsiderationId <= 0 Then
         Throw New ArgumentException("Charging Consideration ID is required.")
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

            Me.DeleteDbsChargingConsideration(chargingConsiderationId, q.LockId)

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

   Private Sub LoadDbsChargingConsideration(chargingConsideration As DbsChargingConsiderationBody, dbsChargingConsideration As DbsChargingConsideration)

      DataLib.ScatterValues(chargingConsideration, dbsChargingConsideration)

   End Sub

   Private Sub LoadDbsChargingConsideration(row As DataRow, dbsChargingConsideration As DbsChargingConsideration)

      With dbsChargingConsideration
         .ChargingConsiderationId = row.ToInt32("ChargingConsiderationId")
         .ChargingConsiderationName = row.ToString("ChargingConsiderationName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsChargingConsideration(chargingConsideration As DbsChargingConsideration)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsChargingConsideration", chargingConsideration, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsChargingConsideration(chargingConsideration)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsChargingConsideration(chargingConsideration As DbsChargingConsideration)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ChargingConsiderationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsChargingConsideration", chargingConsideration, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsChargingConsideration(chargingConsideration)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(chargingConsideration.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsChargingConsideration(chargingConsideration As DbsChargingConsideration)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ChargingConsiderationId", chargingConsideration.ChargingConsiderationId)
         .AddWithValue("@ChargingConsiderationName", chargingConsideration.ChargingConsiderationName)
         .AddWithValue("@SortSeq", chargingConsideration.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsChargingConsideration(chargingConsiderationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ChargingConsiderationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsChargingConsideration", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ChargingConsiderationId", chargingConsiderationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsChargingConsiderationBody
   Inherits DbsChargingConsideration

End Class
