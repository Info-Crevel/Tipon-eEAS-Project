<RoutePrefix("api")>
Public Class APS0510_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayableRequestTypes")>
   <Route("payable-request-types")>
   <HttpGet>
   Public Function GetPayableRequestTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0510_All")
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
   <Route("payable-request-types/{apsRequestTypeId}")>
   <HttpGet>
   Public Function GetPayableRequestType(apsRequestTypeId As Integer) As IHttpActionResult

      If apsRequestTypeId <= 0 Then
         Throw New ArgumentException("Request Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0510")
            With _direct
               .AddParameter("ApsRequestTypeId", apsRequestTypeId)

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
   <Route("payable-request-types/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableRequestType(currentUserId As Integer, <FromBody> requestType As ApsRequestTypeBody) As IHttpActionResult

      If requestType.ApsRequestTypeId <> -1 Then
         Throw New ArgumentException("Request Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _apsrequestTypeId As Integer = SysLib.GetNextSequence("ApsRequestTypeId")

         requestType.ApsRequestTypeId = _apsrequestTypeId

         '
         ' Load proposed values from payload
         '
         Dim _apsRequestType As New ApsRequestType

         Me.LoadApsRequestType(requestType, _apsRequestType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsRequestType(_apsRequestType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestType(_apsrequestTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(requestType.RequestTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayableRequestType")>
   <Route("payable-request-types/{apsRequestTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableRequestType(apsRequestTypeId As Integer, currentUserId As Integer, <FromBody> requestType As ApsRequestTypeBody) As IHttpActionResult

      If apsRequestTypeId <= 0 Then
         Throw New ArgumentException("Request Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsRequestType As New ApsRequestType

         Me.LoadApsRequestType(requestType, _apsRequestType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsRequestType(_apsRequestType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestType(apsRequestTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayableRequestType")>
   <Route("payable-request-types/{apsRequestTypeId}")>
   <HttpDelete>
   Public Function RemovePayableRequestType(apsRequestTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If apsRequestTypeId <= 0 Then
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
            Me.DeleteApsRequestType(apsRequestTypeId, q.LockId)

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

   Private Sub LoadApsRequestType(requestType As ApsRequestTypeBody, apsRequestType As ApsRequestType)

      DataLib.ScatterValues(requestType, apsRequestType)

   End Sub
   Private Sub LoadApsRequestType(row As DataRow, apsRequestType As ApsRequestType)

      With apsRequestType
         .ApsRequestTypeId = row.ToInt32("ApsRequestTypeId")
         .RequestTypeName = row.ToString("RequestTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertApsRequestType(requestType As ApsRequestType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsRequestType", requestType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestType(requestType)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsRequestType(requestType As ApsRequestType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApsRequestTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsRequestType", requestType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestType(requestType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(requestType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsRequestType(requestType As ApsRequestType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ApsRequestTypeId", requestType.ApsRequestTypeId)
         .AddWithValue("@RequestTypeName", requestType.RequestTypeName)
         .AddWithValue("@SortSeq", requestType.SortSeq)
      End With

   End Sub
   Private Sub DeleteApsRequestType(apsRequestTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApsRequestTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsRequestType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApsRequestTypeId", apsRequestTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsRequestTypeBody
   Inherits ApsRequestType

End Class
