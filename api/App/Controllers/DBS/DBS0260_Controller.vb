<RoutePrefix("api")>
Public Class DBS0260_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetSavingLoans")>
   <Route("savings-loans")>
   <HttpGet>
   Public Function GetSavingLoans() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0260_All")
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

   <SymAuthorization("GetSavingLoan")>
   <Route("savings-loans/{savingLoanId}")>
   <HttpGet>
   Public Function GetSavingLoan(savingLoanId As Integer) As IHttpActionResult

      If savingLoanId <= 0 Then
         Throw New ArgumentException("Saving Loan ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0260")
            With _direct
               .AddParameter("SavingLoanId", savingLoanId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateSavingLoan")>
   <Route("savings-loans/{currentUserId}")>
   <HttpPost>
   Public Function CreateSavingLoan(currentUserId As Integer, <FromBody> savingLoan As DbsSavingLoanBody) As IHttpActionResult

      If savingLoan.SavingLoanId <> -1 Then
         Throw New ArgumentException("Saving Loan ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _savingLoanId As Integer = SysLib.GetNextSequence("SavingLoanId")

         savingLoan.SavingLoanId = _savingLoanId

         '
         ' Load proposed values from payload
         '
         Dim _dbsSavingLoan As New DbsSavingLoan

         Me.LoadDbsSavingLoan(savingLoan, _dbsSavingLoan)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsSavingLoan(_dbsSavingLoan)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSavingLoan(_savingLoanId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(savingLoan.SavingLoanId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifySavingLoan")>
   <Route("savings-loans/{savingLoanId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifySavingLoan(savingLoanId As Integer, currentUserId As Integer, <FromBody> savingLoan As DbsSavingLoanBody) As IHttpActionResult

      If savingLoanId <= 0 Then
         Throw New ArgumentException("Saving Loan ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsSavingLoan As New DbsSavingLoan

         Me.LoadDbsSavingLoan(savingLoan, _dbsSavingLoan)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsSavingLoan(_dbsSavingLoan)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSavingLoan(savingLoanId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveSavingLoan")>
   <Route("savings-loans/{savingLoanId}")>
   <HttpDelete>
   Public Function RemoveSavingLoan(savingLoanId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If savingLoanId <= 0 Then
         Throw New ArgumentException("Saving Loan ID is required.")
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

            Me.DeleteDbsSavingLoan(savingLoanId, q.LockId)

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

   Private Sub LoadDbsSavingLoan(savingLoan As DbsSavingLoanBody, dbsSavingLoan As DbsSavingLoan)

      DataLib.ScatterValues(savingLoan, dbsSavingLoan)

   End Sub

   Private Sub LoadDbsSavingLoan(row As DataRow, dbsSavingLoan As DbsSavingLoan)

      With dbsSavingLoan
         .SavingLoanId = row.ToInt32("SavingLoanId")
         .SavingLoanName = row.ToString("SavingLoanName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsSavingLoan(savingLoan As DbsSavingLoan)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSavingLoan", savingLoan, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSavingLoan(savingLoan)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsSavingLoan(savingLoan As DbsSavingLoan)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SavingLoanId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSavingLoan", savingLoan, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsSavingLoan(savingLoan)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(savingLoan.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsSavingLoan(savingLoan As DbsSavingLoan)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@SavingLoanId", savingLoan.SavingLoanId)
         .AddWithValue("@SavingLoanName", savingLoan.SavingLoanName)
         .AddWithValue("@SortSeq", savingLoan.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsSavingLoan(savingLoanId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("SavingLoanId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsSavingLoan", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@SavingLoanId", savingLoanId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsSavingLoanBody
   Inherits DbsSavingLoan

End Class
