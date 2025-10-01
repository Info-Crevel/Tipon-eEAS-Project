<RoutePrefix("api")>
Public Class APS0560_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayableRequestTrxTypes")>
   <Route("payable-request-trx-types")>
   <HttpGet>
   Public Function GetPayableRequestTrxTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0560_All")
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

   <SymAuthorization("GetPayableRequestType")>
   <Route("payable-request-trx-types/{requestTrxTypeId}")>
   <HttpGet>
   Public Function GetPayableRequestType(requestTrxTypeId As Integer) As IHttpActionResult

      If requestTrxTypeId <= 0 Then
         Throw New ArgumentException("Request Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0560")
            With _direct
               .AddParameter("RequestTrxTypeId", requestTrxTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreatePayableRequestType")>
   <Route("payable-request-trx-types/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableRequestType(currentUserId As Integer, <FromBody> requestTrxType As ApsRequestTrxTypeBody) As IHttpActionResult

      If requestTrxType.RequestTrxTypeId <> -1 Then
         Throw New ArgumentException("Request Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _apsrequestTrxTypeId As Integer = SysLib.GetNextSequence("RequestTrxTypeId")

         requestTrxType.RequestTrxTypeId = _apsrequestTrxTypeId

         '
         ' Load proposed values from payload
         '
         Dim _apsRequestType As New ApsRequestTrxType

         Me.LoadApsRequestTrxType(requestTrxType, _apsRequestType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsRequestTrxType(_apsRequestType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestType(_apsrequestTrxTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(requestTrxType.RequestTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayableRequestType")>
   <Route("payable-request-trx-types/{requestTrxTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableRequestType(requestTrxTypeId As Integer, currentUserId As Integer, <FromBody> requestTrxType As ApsRequestTrxTypeBody) As IHttpActionResult

      If requestTrxTypeId <= 0 Then
         Throw New ArgumentException("Request Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsRequestType As New ApsRequestTrxType

         Me.LoadApsRequestTrxType(requestTrxType, _apsRequestType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsRequestTrxType(_apsRequestType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestType(requestTrxTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayableRequestType")>
   <Route("payable-request-trx-types/{requestTrxTypeId}")>
   <HttpDelete>
   Public Function RemovePayableRequestType(requestTrxTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If requestTrxTypeId <= 0 Then
         Throw New ArgumentException("RequestType ID is required.")
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
            Me.DeleteApsRequestTrxType(requestTrxTypeId, q.LockId)

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

   Private Sub LoadApsRequestTrxType(requestTrxType As ApsRequestTrxTypeBody, apsRequestType As ApsRequestTrxType)

      DataLib.ScatterValues(requestTrxType, apsRequestType)

   End Sub
   Private Sub LoadApsRequestTrxType(row As DataRow, apsRequestType As ApsRequestTrxType)

      With apsRequestType
         .RequestTrxTypeId = row.ToInt32("RequestTrxTypeId")
         .RequestTrxTypeName = row.ToString("RequestTrxTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertApsRequestTrxType(requestTrxType As ApsRequestTrxType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsRequestTrxType", requestTrxType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTrxType(requestTrxType)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsRequestTrxType(requestTrxType As ApsRequestTrxType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RequestTrxTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsRequestTrxType", requestTrxType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTrxType(requestTrxType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(requestTrxType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsRequestTrxType(requestTrxType As ApsRequestTrxType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@RequestTrxTypeId", requestTrxType.RequestTrxTypeId)
         .AddWithValue("@RequestTrxTypeName", requestTrxType.RequestTrxTypeName)
         .AddWithValue("@SortSeq", requestTrxType.SortSeq)
      End With

   End Sub
   Private Sub DeleteApsRequestTrxType(requestTrxTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RequestTrxTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsRequestTrxType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RequestTrxTypeId", requestTrxTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsRequestTrxTypeBody
   Inherits ApsRequestTrxType

End Class
