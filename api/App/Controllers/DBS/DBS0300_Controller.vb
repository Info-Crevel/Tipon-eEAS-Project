<RoutePrefix("api")>
Public Class DBS0300_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetBankLocations")>
   <Route("bank-locations")>
   <HttpGet>
   Public Function GetBankLocations() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0300_All")
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

   <SymAuthorization("GetBankLocation")>
   <Route("bank-locations/{bankLocationId}")>
   <HttpGet>
   Public Function GetBankLocation(bankLocationId As Integer) As IHttpActionResult

      If bankLocationId <= 0 Then
         Throw New ArgumentException("Bank Location ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0300")
            With _direct
               .AddParameter("BankLocationId", bankLocationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateBankLocation")>
   <Route("bank-locations/{currentUserId}")>
   <HttpPost>
   Public Function CreateBankLocation(currentUserId As Integer, <FromBody> bankLocation As DbsBankLocationBody) As IHttpActionResult

      If bankLocation.BankLocationId <> -1 Then
         Throw New ArgumentException("Bank Location ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _bankLocationId As Integer = SysLib.GetNextSequence("BankLocationId")

         bankLocation.BankLocationId = _bankLocationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsBankLocation As New DbsBankLocation

         Me.LoadDbsBankLocation(bankLocation, _dbsBankLocation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsBankLocation(_dbsBankLocation)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetBankLocation(_bankLocationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(bankLocation.BankLocationId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyBankLocation")>
   <Route("bank-locations/{bankLocationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyBankLocation(bankLocationId As Integer, currentUserId As Integer, <FromBody> bankLocation As DbsBankLocationBody) As IHttpActionResult

      If bankLocationId <= 0 Then
         Throw New ArgumentException("Bank Location ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsBankLocation As New DbsBankLocation

         Me.LoadDbsBankLocation(bankLocation, _dbsBankLocation)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsBankLocation(_dbsBankLocation)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetBankLocation(bankLocationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveBankLocation")>
   <Route("bank-locations/{bankLocationId}")>
   <HttpDelete>
   Public Function RemoveBankLocation(bankLocationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If bankLocationId <= 0 Then
         Throw New ArgumentException("Bank Location ID is required.")
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

            Me.DeleteDbsBankLocation(bankLocationId, q.LockId)

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

   Private Sub LoadDbsBankLocation(bankLocation As DbsBankLocationBody, dbsBankLocation As DbsBankLocation)

      DataLib.ScatterValues(bankLocation, dbsBankLocation)

   End Sub

   Private Sub LoadDbsBankLocation(row As DataRow, dbsBankLocation As DbsBankLocation)

      With dbsBankLocation
         .BankLocationId = row.ToInt32("BankLocationId")
         .BankLocationName = row.ToString("BankLocationName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsBankLocation(bankLocation As DbsBankLocation)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsBankLocation", bankLocation, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsBankLocation(bankLocation)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsBankLocation(bankLocation As DbsBankLocation)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankLocationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsBankLocation", bankLocation, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsBankLocation(bankLocation)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(bankLocation.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsBankLocation(bankLocation As DbsBankLocation)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@BankLocationId", bankLocation.BankLocationId)
         .AddWithValue("@BankLocationName", bankLocation.BankLocationName)
         .AddWithValue("@SortSeq", bankLocation.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsBankLocation(bankLocationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("BankLocationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsBankLocation", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@BankLocationId", bankLocationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsBankLocationBody
   Inherits DbsBankLocation

End Class
