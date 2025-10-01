<RoutePrefix("api")>
Public Class DBS0290_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetBanks")>
   <Route("banks")>
   <HttpGet>
   Public Function GetBanks() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0290_All")
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

   <SymAuthorization("GetBank")>
   <Route("banks/{bankId}")>
   <HttpGet>
   Public Function GetBank(bankId As Integer) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0290")
            With _direct
               .AddParameter("BankId", bankId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateBank")>
   <Route("banks/{currentUserId}")>
   <HttpPost>
   Public Function CreateBank(currentUserId As Integer, <FromBody> bank As DbsBankBody) As IHttpActionResult

      If bank.BankId <> -1 Then
         Throw New ArgumentException("Bank ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _bankId As Integer = SysLib.GetNextSequence("BankId")

         bank.BankId = _bankId

         '
         ' Load proposed values from payload
         '
         Dim _dbsBank As New DbsBank

         Me.LoadDbsBank(bank, _dbsBank)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsBank(_dbsBank)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetBank(_bankId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(bank.BankId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyBank")>
   <Route("banks/{bankId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyBank(bankId As Integer, currentUserId As Integer, <FromBody> bank As DbsBankBody) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsBank As New DbsBank

         Me.LoadDbsBank(bank, _dbsBank)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsBank(_dbsBank)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetBank(bankId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveBank")>
   <Route("banks/{bankId}")>
   <HttpDelete>
   Public Function RemoveBank(bankId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If bankId <= 0 Then
         Throw New ArgumentException("Bank ID is required.")
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

            Me.DeleteDbsBank(bankId, q.LockId)

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

   Private Sub LoadDbsBank(bank As DbsBankBody, dbsBank As DbsBank)

      DataLib.ScatterValues(bank, dbsBank)

   End Sub

   Private Sub LoadDbsBank(row As DataRow, dbsBank As DbsBank)

      With dbsBank
         .BankId = row.ToInt32("BankId")
         .BankName = row.ToString("BankName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsBank(bank As DbsBank)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsBank", bank, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsBank(bank)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsBank(bank As DbsBank)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsBank", bank, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsBank(bank)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(bank.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsBank(bank As DbsBank)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@BankId", bank.BankId)
         .AddWithValue("@BankName", bank.BankName)
         .AddWithValue("@SortSeq", bank.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsBank(bankId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsBank", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@BankId", bankId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsBankBody
   Inherits DbsBank

End Class
