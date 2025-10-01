<RoutePrefix("api")>
Public Class FIN0020_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetAtcs")>
   <Route("atcs")>
   <HttpGet>
   Public Function GetAtcs() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.FIN0020_All")
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

   <SymAuthorization("GetAtc")>
   <Route("atcs/{atcId}")>
   <HttpGet>
   Public Function GetAtc(atcId As Integer) As IHttpActionResult

      If atcId <= 0 Then
         Throw New ArgumentException("Atc ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.FIN0020")
            With _direct
               .AddParameter("AtcId", atcId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateAtc")>
   <Route("atcs/{currentUserId}")>
   <HttpPost>
   Public Function CreateAtc(currentUserId As Integer, <FromBody> atc As FinAtcBody) As IHttpActionResult

      If atc.AtcId <> -1 Then
         Throw New ArgumentException("Atc ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _atcId As Integer = SysLib.GetNextSequence("AtcId")

         atc.AtcId = _atcId

         '
         ' Load proposed values from payload
         '
         Dim _finAtc As New FinAtc

         Me.LoadFinAtc(atc, _finAtc)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertFinAtc(_finAtc)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAtc(_atcId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyAtc")>
   <Route("atcs/{atcId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyAtc(atcId As Integer, currentUserId As Integer, <FromBody> atc As FinAtcBody) As IHttpActionResult

      If atcId <= 0 Then
         Throw New ArgumentException("Atc ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _finAtc As New FinAtc

         Me.LoadFinAtc(atc, _finAtc)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateFinAtc(_finAtc)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetAtc(atcId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveAtc")>
   <Route("atcs/{atcId}")>
   <HttpDelete>
   Public Function RemoveAtc(atcId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If atcId <= 0 Then
         Throw New ArgumentException("Atc ID is required.")
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
            Me.DeleteFinAtc(atcId, q.LockId)

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
   Private Sub LoadFinAtc(atc As FinAtcBody, finAtc As FinAtc)

      DataLib.ScatterValues(atc, finAtc)

   End Sub
   Private Sub LoadFinAtc(row As DataRow, finAtc As FinAtc)

      With finAtc
         .AtcId = row.ToInt32("AtcId")
         .AtcName = row.ToString("AtcName")
         .AtcCode = row.ToString("AtcCode")
         .AtcDescription = row.ToString("AtcDescription")
         .Percentage = row.ToDecimal("Percentage")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub InsertFinAtc(atc As FinAtc)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.FinAtc", atc, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinAtc(atc)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateFinAtc(atc As FinAtc)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AtcId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.FinAtc", atc, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsFinAtc(atc)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(atc.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsFinAtc(atc As FinAtc)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@AtcId", atc.AtcId)
         .AddWithValue("@AtcCode", atc.AtcCode)
         .AddWithValue("@AtcName", atc.AtcName)
         .AddWithValue("@AtcDescription", atc.AtcDescription)
         .AddWithValue("@Percentage", atc.Percentage)
         .AddWithValue("@AccountId", atc.AccountId)
      End With

   End Sub
   Private Sub DeleteFinAtc(atcId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("AtcId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.FinAtc", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@AtcId", atcId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class FinAtcBody
   Inherits FinAtc

End Class
