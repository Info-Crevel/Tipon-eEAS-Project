<RoutePrefix("api")>
Public Class DBS0480_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetMemberRequestPositions")>
    <Route("member-request-positions")>
    <HttpGet>
    Public Function GetMemberRequestPositions() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0480_All")
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

    <SymAuthorization("GetMemberRequestPosition")>
    <Route("member-request-positions/{memberRequestPositionId}")>
    <HttpGet>
    Public Function GetMemberRequestPosition(memberRequestPositionId As Integer) As IHttpActionResult

        If memberRequestPositionId <= 0 Then
            Throw New ArgumentException("Member Request Position ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0480")
                With _direct
                    .AddParameter("MemberRequestPositionId", memberRequestPositionId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateMemberRequestPosition")>
    <Route("member-request-positions/{currentUserId}")>
    <HttpPost>
    Public Function CreateMemberRequestPosition(currentUserId As Integer, <FromBody> memberRequestPosition As ArsMemberRequestPositionBody) As IHttpActionResult

        If memberRequestPosition.MemberRequestPositionId <> -1 Then
            Throw New ArgumentException("Member Request Position ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _memberRequestPositionId As Integer = SysLib.GetNextSequence("MemberRequestPositionId")

            memberRequestPosition.MemberRequestPositionId = _memberRequestPositionId

            '
            ' Load proposed values from payload
            '
            Dim _arsMemberRequestPosition As New ArsMemberRequestPosition

            Me.LoadArsMemberRequestPosition(memberRequestPosition, _arsMemberRequestPosition)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertArsMemberRequestPosition(_arsMemberRequestPosition)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestPosition(_memberRequestPositionId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(engagementType.MemberRequestPositionId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyMemberRequestPosition")>
    <Route("member-request-positions/{memberRequestPositionId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyMemberRequestPosition(memberRequestPositionId As Integer, currentUserId As Integer, <FromBody> memberRequestPosition As ArsMemberRequestPositionBody) As IHttpActionResult

        If memberRequestPositionId <= 0 Then
            Throw New ArgumentException("Member Request Position ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _arsMemberRequestPosition As New ArsMemberRequestPosition

            Me.LoadArsMemberRequestPosition(memberRequestPosition, _arsMemberRequestPosition)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateArsMemberRequestPosition(_arsMemberRequestPosition)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestPosition(memberRequestPositionId), Results.OkNegotiatedContentResult(Of DataTable))

            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemoveMemberRequestPosition")>
   <Route("member-request-positions/{memberRequestPositionId}")>
   <HttpDelete>
   Public Function RemoveMemberRequestPosition(memberRequestPositionId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberRequestPositionId <= 0 Then
         Throw New ArgumentException("Member Request Position ID is required.")
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

            Me.DeleteArsMemberRequestPosition(memberRequestPositionId, q.LockId)

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

   Private Sub LoadArsMemberRequestPosition(memberRequestPosition As ArsMemberRequestPositionBody, arsMemberRequestPosition As ArsMemberRequestPosition)

      DataLib.ScatterValues(memberRequestPosition, arsMemberRequestPosition)

   End Sub
   Private Sub LoadArsMemberRequestPosition(row As DataRow, arsMemberRequestPosition As ArsMemberRequestPosition)

        With arsMemberRequestPosition
            .MemberRequestPositionId = row.ToInt32("MemberRequestPositionId")
            .MemberRequestPositionName = row.ToString("MemberRequestPositionName")
            .MemberRequestPositionDescription = row.ToString("MemberRequestPositionDescription")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertArsMemberRequestPosition(engagementType As ArsMemberRequestPosition)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestPosition", engagementType, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsMemberRequestPosition(engagementType)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateArsMemberRequestPosition(memberRequestPosition As ArsMemberRequestPosition)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberRequestPositionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestPosition", memberRequestPosition, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsMemberRequestPosition(memberRequestPosition)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberRequestPosition.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsMemberRequestPosition(memberRequestPosition As ArsMemberRequestPosition)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@MemberRequestPositionId", memberRequestPosition.MemberRequestPositionId)
            .AddWithValue("@MemberRequestPositionName", memberRequestPosition.MemberRequestPositionName)
            .AddWithValue("@MemberRequestPositionDescription", memberRequestPosition.MemberRequestPositionDescription)
            .AddWithValue("@SortSeq", memberRequestPosition.SortSeq)
        End With

    End Sub
    Private Sub DeleteArsMemberRequestPosition(engagementTypeId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberRequestPositionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberRequestPosition", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@MemberRequestPositionId", engagementTypeId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class
Public Class ArsMemberRequestPositionBody
    Inherits ArsMemberRequestPosition
End Class
