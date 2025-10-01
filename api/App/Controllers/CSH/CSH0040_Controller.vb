<RoutePrefix("api")>
Public Class CSH0040_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetDeposit")>
   <Route("deposits/{depositId}")>
   <HttpGet>
   Public Function GetDeposit(depositId As Integer) As IHttpActionResult

      If depositId <= 0 Then
         Throw New ArgumentException("Deposit ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.CSH0040")
            With _direct
               .AddParameter("DepositId", depositId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateDeposit")>
   <Route("deposits/{currentUserId}")>
   <HttpPost>
   Public Function CreateDeposit(currentUserId As Integer, <FromBody> dep As DepositBody) As IHttpActionResult

      If dep.DepositId <> -1 Then
         Throw New ArgumentException("Deposit ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _depositId As Integer = SysLib.GetNextSequence("DepositId")

         dep.DepositId = _depositId

         '
         ' Assign next Document sequence if 'new' token is indicated
         '
         If dep.LCNumber = "< NEW >" Then
            dep.LCNumber = AppLib.GetNextDocSequence(DocSequencerId.LC, dep.DepositDate)
         End If

         '
         ' Load proposed values from payload
         '
         Dim _cshDep As New CshDep

         Me.LoadCshDep(dep, _cshDep)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertCshDep(_cshDep)

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

         'Return Me.Ok(True)
         Return Me.Ok(dep.DepositId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyDeposit")>
   <Route("deposits/{depositId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyDeposit(depositId As Integer, currentUserId As Integer, <FromBody> dep As DepositBody) As IHttpActionResult

      If depositId <= 0 Then
         Throw New ArgumentException("Deposit ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _cshDep As New CshDep

         Me.LoadCshDep(dep, _cshDep)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateCshDep(_cshDep)

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

   <SymAuthorization("RemoveDeposit")>
   <Route("deposits/{depositId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveDeposit(depositId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If depositId <= 0 Then
         Throw New ArgumentException("Deposit ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.DeleteCshDep(depositId, lockId)

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

   Private Sub LoadCshDep(dep As DepositBody, cshDep As CshDep)

      DataLib.ScatterValues(dep, cshDep)

   End Sub

   Private Sub InsertCshDep(dep As CshDep)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.CshDep", dep, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshDep(dep)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateCshDep(dep As CshDep)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DepositId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.CshDep", dep, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsCshDep(dep)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(dep.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsCshDep(dep As CshDep)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DepositId", dep.DepositId)
         .AddWithValue("@DepositDate", dep.DepositDate)
         .AddWithValue("@LCNumber", dep.LCNumber)
         .AddWithValue("@StartReceiptNumber", dep.StartReceiptNumber)
         .AddWithValue("@EndReceiptNumber", dep.EndReceiptNumber)
         .AddWithValue("@Amount", dep.Amount)
         .AddWithValue("@PostUserId", dep.PostUserId)

      End With

   End Sub

   Private Sub DeleteCshDep(depositId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DepositId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.CshDep", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DepositId", depositId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DepositBody
   Inherits CshDep

End Class
