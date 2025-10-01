<RoutePrefix("api")>
Public Class APS0520_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetPayableRequestTypesParticulars")>
   <Route("payable-request-type-particulars")>
   <HttpGet>
   Public Function GetPayableRequestTypesParticulars() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0520_All")
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

   <SymAuthorization("GetPayableRequestTypeParticulars")>
   <Route("payable-request-type-particulars/{particularsId}")>
   <HttpGet>
   Public Function GetPayableRequestTypeParticulars(particularsId As Integer) As IHttpActionResult

      If particularsId <= 0 Then
         Throw New ArgumentException("Particulars ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0520")
            With _direct
               .AddParameter("ParticularsId", particularsId)

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
   <Route("payable-request-type-particulars/{currentUserId}")>
   <HttpPost>
   Public Function CreatePayableRequestType(currentUserId As Integer, <FromBody> particulars As ApsRequestTypeParticularsBody) As IHttpActionResult

      If particulars.ParticularsId <> -1 Then
         Throw New ArgumentException("Particulars ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _particularsId As Integer = SysLib.GetNextSequence("ParticularsId")

         particulars.ParticularsId = _particularsId

         '
         ' Load proposed values from payload
         '
         Dim _apsRequestTypeParticulars As New ApsRequestTypeParticulars

         Me.LoadApsRequestTypeParticulars(particulars, _apsRequestTypeParticulars)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsRequestTypeParticulars(_apsRequestTypeParticulars)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestTypeParticulars(_particularsId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(requestType.RequestTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyPayableRequestType")>
   <Route("payable-request-type-particulars/{particularsId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyPayableRequestType(particularsId As Integer, currentUserId As Integer, <FromBody> particulars As ApsRequestTypeParticularsBody) As IHttpActionResult

      If particularsId <= 0 Then
         Throw New ArgumentException("Particulars ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsRequestTypeParticulars As New ApsRequestTypeParticulars

         Me.LoadApsRequestTypeParticulars(particulars, _apsRequestTypeParticulars)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsRequestTypeParticulars(_apsRequestTypeParticulars)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayableRequestTypeParticulars(particularsId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemovePayableRequestTypeParticulars")>
   <Route("payable-request-type-particulars/{particularsId}")>
   <HttpDelete>
   Public Function RemovePayableRequestTypeParticulars(particularsId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If particularsId <= 0 Then
         Throw New ArgumentException("Particulars ID is required.")
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
            Me.DeleteApsRequestTypeParticulars(particularsId, q.LockId)

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

   Private Sub LoadApsRequestTypeParticulars(requestTypeParticulars As ApsRequestTypeParticularsBody, apsRequestTypeParticulars As ApsRequestTypeParticulars)

      DataLib.ScatterValues(requestTypeParticulars, apsRequestTypeParticulars)

   End Sub
   Private Sub LoadApsRequestTypeParticulars(row As DataRow, apsRequestTypeParticulars As ApsRequestTypeParticulars)

      With apsRequestTypeParticulars
         .ParticularsId = row.ToInt32("ParticularsId")
         .ParticularsName = row.ToString("ParticularsName")
         .ParticularsDescription = row.ToString("ParticularsDescription")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertApsRequestTypeParticulars(requestTypeParticulars As ApsRequestTypeParticulars)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsRequestTypeParticulars", requestTypeParticulars, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTypeParticulars(requestTypeParticulars)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsRequestTypeParticulars(requestTypeParticulars As ApsRequestTypeParticulars)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ParticularsId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsRequestTypeParticulars", requestTypeParticulars, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsRequestTypeParticulars(requestTypeParticulars)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(requestTypeParticulars.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsRequestTypeParticulars(requestTypeParticulars As ApsRequestTypeParticulars)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ParticularsId", requestTypeParticulars.ParticularsId)
         .AddWithValue("@ParticularsName", requestTypeParticulars.ParticularsName)
         .AddWithValue("@ParticularsDescription", requestTypeParticulars.ParticularsDescription)
         .AddWithValue("@SortSeq", requestTypeParticulars.SortSeq)
      End With

   End Sub
   Private Sub DeleteApsRequestTypeParticulars(particularsId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ParticularsId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsRequestTypeParticulars", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ParticularsId", particularsId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsRequestTypeParticularsBody
   Inherits ApsRequestTypeParticulars

End Class
