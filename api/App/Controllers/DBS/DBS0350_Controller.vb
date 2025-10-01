<RoutePrefix("api")>
Public Class DBS0350_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetDeminimis")>
   <Route("deminimis")>
   <HttpGet>
   Public Function GetDeminimis() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0350_All")
            Using _dataSet As DataSet = _direct.ExecuteDataSet()

               With _dataSet
                  .Tables(0).TableName = "deminimis"
                  .Tables(1).TableName = "payTrx"
               End With

               Return Me.Ok(_dataSet)
            End Using

         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetDeminimis")>
   <Route("deminimis/{deminimisId}")>
   <HttpGet>
   Public Function GetDeminimis(deminimisId As Integer) As IHttpActionResult

      If deminimisId <= 0 Then
         Throw New ArgumentException("Deminimis ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0350")
            With _direct
               .AddParameter("DeminimisId", deminimisId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateDeminimis")>
   <Route("deminimis/{currentUserId}")>
   <HttpPost>
   Public Function CreateDeminimis(currentUserId As Integer, <FromBody> deminimis As DbsDeminimisBody) As IHttpActionResult

      If deminimis.DeminimisId <> -1 Then
         Throw New ArgumentException("Deminimis ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _deminimisId As Integer = SysLib.GetNextSequence("DeminimisId")

         deminimis.DeminimisId = _deminimisId

         '
         ' Load proposed values from payload
         '
         Dim _dbsDeminimis As New DbsDeminimis

         Me.LoadDbsDeminimis(deminimis, _dbsDeminimis)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsDeminimis(_dbsDeminimis)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDeminimis(_deminimisId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(deminimis.DeminimisId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyDeminimis")>
   <Route("deminimis/{deminimisId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyDeminimis(deminimisId As Integer, currentUserId As Integer, <FromBody> deminimis As DbsDeminimisBody) As IHttpActionResult

      If deminimisId <= 0 Then
         Throw New ArgumentException("Deminimis ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsDeminimis As New DbsDeminimis

         Me.LoadDbsDeminimis(deminimis, _dbsDeminimis)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsDeminimis(_dbsDeminimis)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDeminimis(deminimisId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveDeminimis")>
   <Route("deminimis/{deminimisId}")>
   <HttpDelete>
   Public Function RemoveDeminimis(deminimisId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If deminimisId <= 0 Then
         Throw New ArgumentException("Deminimis ID is required.")
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

            Me.DeleteDbsDeminimis(deminimisId, q.LockId)

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

   Private Sub LoadDbsDeminimis(deminimis As DbsDeminimisBody, dbsDeminimis As DbsDeminimis)

      DataLib.ScatterValues(deminimis, dbsDeminimis)

   End Sub

   Private Sub LoadDbsDeminimis(row As DataRow, dbsDeminimis As DbsDeminimis)

      With dbsDeminimis
         .DeminimisId = row.ToInt32("DeminimisId")
         .DeminimisName = row.ToString("DeminimisName")
         .PayTrxCode = row.ToString("PayTrxCode")
            .TaxFlag = row.ToBoolean("TaxFlag")
            .PayHourly = row.ToBoolean("PayHourly")
            .PayMaxAmount = row.ToInt32("PayMaxAmount")
            .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsDeminimis(deminimis As DbsDeminimis)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsDeminimis", deminimis, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDeminimis(deminimis)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsDeminimis(deminimis As DbsDeminimis)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DeminimisId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsDeminimis", deminimis, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDeminimis(deminimis)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(deminimis.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsDeminimis(deminimis As DbsDeminimis)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DeminimisId", deminimis.DeminimisId)
         .AddWithValue("@DeminimisName", deminimis.DeminimisName)
         .AddWithValue("@PayTrxCode", deminimis.PayTrxCode)
            .AddWithValue("@TaxFlag", deminimis.TaxFlag)
            .AddWithValue("@PayHourly", deminimis.PayHourly)
            .AddWithValue("@PayMaxAmount", deminimis.PayMaxAmount)
            .AddWithValue("@SortSeq", deminimis.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsDeminimis(deminimisId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DeminimisId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsDeminimis", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DeminimisId", deminimisId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsDeminimisBody
   Inherits DbsDeminimis

End Class
