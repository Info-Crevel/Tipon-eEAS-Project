<RoutePrefix("api")>
Public Class DBS0310_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberTransactionTypes")>
   <Route("member-transaction-types")>
   <HttpGet>
   Public Function GetMemberTransactionTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0310_All")
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

   <SymAuthorization("GetMemberTransactionType")>
   <Route("member-transaction-types/{memberTransactionTypeId}")>
   <HttpGet>
   Public Function GetMemberTransactionType(memberTransactionTypeId As Integer) As IHttpActionResult

      If memberTransactionTypeId <= 0 Then
         Throw New ArgumentException("Member Transaction Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0310")
            With _direct
               .AddParameter("MemberTransactionTypeId", memberTransactionTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMemberTransactionType")>
   <Route("member-transaction-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberTransactionType(currentUserId As Integer, <FromBody> memberTransactionType As DbsMemberTransactionTypeBody) As IHttpActionResult

      If memberTransactionType.MemberTransactionTypeId <> -1 Then
         Throw New ArgumentException("Member Transaction Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _memberTransactionTypeId As Integer = SysLib.GetNextSequence("MemberTransactionTypeId")

         memberTransactionType.MemberTransactionTypeId = _memberTransactionTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsMemberTransactionType As New DbsMemberTransactionType

         Me.LoadDbsMemberTransactionType(memberTransactionType, _dbsMemberTransactionType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsMemberTransactionType(_dbsMemberTransactionType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberTransactionType(_memberTransactionTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(memberTransactionType.MemberTransactionTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyMemberTransactionType")>
   <Route("member-transaction-types/{memberTransactionTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberTransactionType(memberTransactionTypeId As Integer, currentUserId As Integer, <FromBody> memberTransactionType As DbsMemberTransactionTypeBody) As IHttpActionResult

      If memberTransactionTypeId <= 0 Then
         Throw New ArgumentException("Member Transaction Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsMemberTransactionType As New DbsMemberTransactionType

         Me.LoadDbsMemberTransactionType(memberTransactionType, _dbsMemberTransactionType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsMemberTransactionType(_dbsMemberTransactionType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberTransactionType(memberTransactionTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveMemberTransactionType")>
   <Route("member-transaction-types/{memberTransactionTypeId}")>
   <HttpDelete>
   Public Function RemoveMemberTransactionType(memberTransactionTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberTransactionTypeId <= 0 Then
         Throw New ArgumentException("Member Transaction Type ID is required.")
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

            Me.DeleteDbsMemberTransactionType(memberTransactionTypeId, q.LockId)

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

   Private Sub LoadDbsMemberTransactionType(memberTransactionType As DbsMemberTransactionTypeBody, dbsMemberTransactionType As DbsMemberTransactionType)

      DataLib.ScatterValues(memberTransactionType, dbsMemberTransactionType)

   End Sub

   Private Sub LoadDbsMemberTransactionType(row As DataRow, dbsMemberTransactionType As DbsMemberTransactionType)

      With dbsMemberTransactionType
         .MemberTransactionTypeId = row.ToInt32("MemberTransactionTypeId")
         .MemberTransactionTypeName = row.ToString("MemberTransactionTypeName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsMemberTransactionType(memberTransactionType As DbsMemberTransactionType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMemberTransactionType", memberTransactionType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMemberTransactionType(memberTransactionType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsMemberTransactionType(memberTransactionType As DbsMemberTransactionType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberTransactionTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMemberTransactionType", memberTransactionType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsMemberTransactionType(memberTransactionType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberTransactionType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsMemberTransactionType(memberTransactionType As DbsMemberTransactionType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberTransactionTypeId", memberTransactionType.MemberTransactionTypeId)
         .AddWithValue("@MemberTransactionTypeName", memberTransactionType.MemberTransactionTypeName)
         .AddWithValue("@SortSeq", memberTransactionType.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsMemberTransactionType(memberTransactionTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberTransactionTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMemberTransactionType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MemberTransactionTypeId", memberTransactionTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsMemberTransactionTypeBody
   Inherits DbsMemberTransactionType

End Class
