<RoutePrefix("api")>
Public Class APS0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayableTaxes")>
   <Route("payable-taxes")>
   <HttpGet>
   Public Function GetPayableTaxes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0030_All")
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

   <SymAuthorization("GetPayableTax")>
   <Route("payable-taxes/{payabletaxCode}")>
   <HttpGet>
   Public Function GetPayableTax(payabletaxCode As Integer) As IHttpActionResult

      If payabletaxCode <= 0 Then
         Throw New ArgumentException("Payable Tax Code is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0030")
            With _direct
               .AddParameter("PayableTaxCode", payabletaxCode)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePayableTax")>
   <Route("payable-taxes/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableTax(currentUserId As Integer, <FromBody> payableTax As ApsPayableTaxBody) As IHttpActionResult

      If payableTax.PayableTaxCode <> -1 Then
         Throw New ArgumentException("Payable Tax Code is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _payableTaxCode As Integer = SysLib.GetNextSequence("PayableTaxCode")

         payableTax.PayableTaxCode = _payableTaxCode

         '
         ' Load proposed values from payload
         '
         Dim _apsPayableTax As New ApsPayableTax

         Me.LoadApsPayableTax(payableTax, _apsPayableTax)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsPayableTax(_apsPayableTax)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableTax(_payableTaxCode), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayableTax")>
   <Route("payable-taxes/{payableTaxCode}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableTax(payableTaxCode As Integer, currentUserId As Integer, <FromBody> atc As ApsPayableTaxBody) As IHttpActionResult

      If payableTaxCode <= 0 Then
         Throw New ArgumentException("Payable Tax Code is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsPayableTax As New ApsPayableTax

         Me.LoadApsPayableTax(atc, _apsPayableTax)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsPayableTax(_apsPayableTax)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableTax(payableTaxCode), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayableTax")>
   <Route("payable-taxes/{payableTaxCode}")>
   <HttpDelete>
   Public Function RemovePayableTax(payableTaxCode As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payableTaxCode <= 0 Then
         Throw New ArgumentException("Payable Tax Code is required.")
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
            Me.DeleteApsPayableTax(payableTaxCode, q.LockId)

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
   Private Sub LoadApsPayableTax(payableTax As ApsPayableTaxBody, apsPayableTax As ApsPayableTax)

      DataLib.ScatterValues(payableTax, apsPayableTax)

   End Sub
   Private Sub LoadApsPayableTax(row As DataRow, apsPayableTax As ApsPayableTax)

      With apsPayableTax
         .PayableTaxCode = row.ToInt32("PayableTaxCode")
         .PayableTaxName = row.ToString("PayableTaxName")
         .Percentage = row.ToDecimal("Percentage")
         .AccountId = row.ToString("AccountId")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertApsPayableTax(payableTax As ApsPayableTax)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsPayableTax", payableTax, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsPayableTax(payableTax)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsPayableTax(payableTax As ApsPayableTax)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayableTaxCode")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsPayableTax", payableTax, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsPayableTax(payableTax)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payableTax.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsPayableTax(payableTax As ApsPayableTax)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PayableTaxCode", payableTax.PayableTaxCode)
         .AddWithValue("@PayableTaxName", payableTax.PayableTaxName)
         .AddWithValue("@Percentage", payableTax.Percentage)
         .AddWithValue("@AccountId", payableTax.AccountId)
         .AddWithValue("@SortSeq", payableTax.SortSeq)
      End With

   End Sub
   Private Sub DeleteApsPayableTax(payableTaxCode As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayableTaxCode")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsPayableTax", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PayableTaxCode", payableTaxCode)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class ApsPayableTaxBody
   Inherits ApsPayableTax

End Class
