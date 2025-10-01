<RoutePrefix("api")>
Public Class APS0010_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayeeTypes")>
   <Route("payee-types")>
   <HttpGet>
   Public Function GetPayeeTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0010_All")
            Using _dataSet As DataSet = _direct.ExecuteDataSet()

               With _dataSet
                  .Tables(0).TableName = "payeeTypes"
                  .Tables(1).TableName = "terms"
               End With

               Return Me.Ok(_dataSet)
            End Using

         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetPayeeType")>
   <Route("payee-types/{payeeTypeId}")>
   <HttpGet>
   Public Function GetPayeeType(payeeTypeId As Integer) As IHttpActionResult

      If payeeTypeId <= 0 Then
         Throw New ArgumentException("PayeeType ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0010")
            With _direct
               .AddParameter("PayeeTypeId", payeeTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePayeeType")>
   <Route("payee-types/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayeeType(currentUserId As Integer, <FromBody> payeeType As PayeeTypeBody) As IHttpActionResult

      If payeeType.PayeeTypeId <> -1 Then
         Throw New ArgumentException("PayeeType ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _payeeTypeId As Integer = SysLib.GetNextSequence("PayeeTypeId")

         payeeType.PayeeTypeId = _payeeTypeId

         '
         ' Load proposed values from payload
         '
         Dim _apsPayeeType As New ApsPayeeType

         Me.LoadApsPayeeType(payeeType, _apsPayeeType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertPayeeType(_apsPayeeType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayeeType(_payeeTypeId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(True)
         'Return Me.Ok(payTrx.PayTrxId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayeeType")>
   <Route("payee-types/{payeeTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayeeType(payeeTypeId As Integer, currentUserId As Integer, <FromBody> payeeType As PayeeTypeBody) As IHttpActionResult

      If payeeTypeId <= 0 Then
         Throw New ArgumentException("PayeeType ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsPayeeType As New ApsPayeeType

         Me.LoadApsPayeeType(payeeType, _apsPayeeType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsPayeeType(_apsPayeeType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayeeType(payeeTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayeeType")>
   <Route("payee-types/{payeeTypeId}")>
   <HttpDelete>
   Public Function RemovePayeeType(payeeTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payeeTypeId <= 0 Then
         Throw New ArgumentException("Payee Type ID is required.")
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
            Me.DeletePayeeType(payeeTypeId, q.LockId)

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
   Private Sub LoadApsPayeeType(payeeType As PayeeTypeBody, finPayeeType As ApsPayeeType)

      DataLib.ScatterValues(payeeType, finPayeeType)

   End Sub
   Private Sub LoadApsPayeeType(row As DataRow, apsPayeeType As ApsPayeeType)

      With apsPayeeType
         .PayeeTypeId = row.ToInt32("PayeeTypeId")
         .PayeeTypeName = row.ToString("PayeeTypeName")
         .PayableTermId = row.ToInt32("PayableTermId")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub

   Private Sub InsertPayeeType(payeeType As ApsPayeeType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsPayeeType", payeeType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayeeType(payeeType)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsPayeeType(payeeType As ApsPayeeType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayeeTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsPayeeType", payeeType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsPayeeType(payeeType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payeeType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsPayeeType(payeeType As ApsPayeeType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PayeeTypeId", payeeType.PayeeTypeId)
         .AddWithValue("@PayeeTypeName", payeeType.PayeeTypeName)
         .AddWithValue("@PayableTermId", payeeType.PayableTermId)
         .AddWithValue("@AccountId", payeeType.AccountId)
      End With

   End Sub
   Private Sub DeletePayeeType(payeeTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayeeTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsPayeeType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PayeeTypeId", payeeTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
Public Class PayeeTypeBody
   Inherits ApsPayeeType

End Class
