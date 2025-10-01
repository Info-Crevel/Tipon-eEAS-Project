<RoutePrefix("api")>
Public Class APS0550_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetApprovers")>
   <Route("approvers")>
   <HttpGet>
   Public Function GetApprovers() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.APS0550_All")
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

   <SymAuthorization("GetApprover")>
   <Route("approvers/{approverId}")>
   <HttpGet>
   Public Function GetApprover(approverId As Integer) As IHttpActionResult

      If approverId <= 0 Then
         Throw New ArgumentException("Approver ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.APS0550")
            With _direct
               .AddParameter("ApproverId", approverId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateApprover")>
   <Route("approvers/{currentUserId}")>
   <HttpPost>
   Public Function CreateApprover(currentUserId As Integer, <FromBody> approver As ApsApproverBody) As IHttpActionResult

      If approver.ApproverId <> -1 Then
         Throw New ArgumentException("Approver ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _approverId As Integer = SysLib.GetNextSequence("ApproverId")

         approver.ApproverId = _approverId

         '
         ' Load proposed values from payload
         '
         Dim _apsApprover As New ApsApprover

         Me.LoadApsApprover(approver, _apsApprover)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertApsApprover(_apsApprover)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApprover(_approverId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(approver.ApproverId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApprover")>
   <Route("approvers/{approverId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApprover(approverId As Integer, currentUserId As Integer, <FromBody> approver As ApsApproverBody) As IHttpActionResult

      If approverId <= 0 Then
         Throw New ArgumentException("Approver ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _apsApprover As New ApsApprover

         Me.LoadApsApprover(approver, _apsApprover)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateApsApprover(_apsApprover)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetApprover(approverId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveApprover")>
   <Route("approvers/{approverId}")>
   <HttpDelete>
   Public Function RemoveApprover(approverId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If approverId <= 0 Then
         Throw New ArgumentException("Approver ID is required.")
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
            Me.DeleteApsApprover(approverId, q.LockId)

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

   Private Sub LoadApsApprover(approver As ApsApproverBody, apsApprover As ApsApprover)

      DataLib.ScatterValues(approver, apsApprover)

   End Sub
   Private Sub LoadApsApprover(row As DataRow, apsApprover As ApsApprover)

      With apsApprover
         .ApproverId = row.ToInt32("ApproverId")
         .MemberEmployeeId = row.ToString("MemberEmployeeId")
         .UserId = row.ToInt32("UserId")
      End With

   End Sub

   Private Sub InsertApsApprover(approver As ApsApprover)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsApprover", approver, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsApprover(approver)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsApprover(approver As ApsApprover)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApproverId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsApprover", approver, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsApprover(approver)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(approver.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsApsApprover(approver As ApsApprover)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@ApproverId", approver.ApproverId)
         .AddWithValue("@MemberEmployeeId", approver.MemberEmployeeId)
         .AddWithValue("@UserId", approver.UserId)
      End With

   End Sub
   Private Sub DeleteApsApprover(approverId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApproverId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsApprover", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApproverId", approverId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class ApsApproverBody
   Inherits ApsApprover

End Class
