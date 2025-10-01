<RoutePrefix("api")>
Public Class DBS0560_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetMemberSuffixes")>
    <Route("member-suffixes")>
    <HttpGet>
    Public Function GetMemberSuffixes() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0560_All")
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

    <SymAuthorization("GetMemberSuffix")>
    <Route("member-suffixes/{memberSuffixId}")>
    <HttpGet>
    Public Function GetMemberSuffix(memberSuffixId As Integer) As IHttpActionResult

        If memberSuffixId <= 0 Then
            Throw New ArgumentException("Member Suffix ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0560")
                With _direct
                    .AddParameter("MemberSuffixId", memberSuffixId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateMemberSuffix")>
    <Route("member-suffixes/{currentUserId}")>
    <HttpPost>
    Public Function CreateMemberSuffix(currentUserId As Integer, <FromBody> memberSuffix As DbsMemberSuffixBody) As IHttpActionResult

        If memberSuffix.MemberSuffixId <> -1 Then
            Throw New ArgumentException("Member Suffix ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _memberSuffixId As Integer = SysLib.GetNextSequence("MemberSuffixId")

            memberSuffix.MemberSuffixId = _memberSuffixId

            '
            ' Load proposed values from payload
            '
            Dim _dbsMemberSuffix As New DbsMemberSuffix

            Me.LoadDbsMemberSuffix(memberSuffix, _dbsMemberSuffix)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsMemberSuffix(_dbsMemberSuffix)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberSuffix(_memberSuffixId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(memberSuffix.MemberSuffixId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyMemberSuffix")>
    <Route("member-suffixes/{memberSuffixId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyMemberSuffix(memberSuffixId As Integer, currentUserId As Integer, <FromBody> memberSuffix As DbsMemberSuffixBody) As IHttpActionResult

        If memberSuffixId <= 0 Then
            Throw New ArgumentException("Member Suffix ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsMemberSuffix As New DbsMemberSuffix

            Me.LoadDbsMemberSuffix(memberSuffix, _dbsMemberSuffix)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsMemberSuffix(_dbsMemberSuffix)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberSuffix(memberSuffixId), Results.OkNegotiatedContentResult(Of DataTable))

            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemoveMemberSuffix")>
   <Route("member-suffixes/{memberSuffixId}")>
   <HttpDelete>
   Public Function RemoveMemberSuffix(memberSuffixId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberSuffixId <= 0 Then
         Throw New ArgumentException("Member Suffix ID is required.")
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

            Me.DeleteDbsMemberSuffix(memberSuffixId, q.LockId)

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

   Private Sub LoadDbsMemberSuffix(memberSuffix As DbsMemberSuffixBody, dbsMemberSuffix As DbsMemberSuffix)

        DataLib.ScatterValues(memberSuffix, dbsMemberSuffix)

    End Sub

    Private Sub LoadDbsMemberSuffix(row As DataRow, dbsMemberSuffix As DbsMemberSuffix)

        With dbsMemberSuffix
            .MemberSuffixId = row.ToInt32("MemberSuffixId")
            .MemberSuffixName = row.ToString("MemberSuffixName")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsMemberSuffix(memberSuffix As DbsMemberSuffix)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsMemberSuffix", memberSuffix, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsMemberSuffix(memberSuffix)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateDbsMemberSuffix(memberSuffix As DbsMemberSuffix)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberSuffixId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsMemberSuffix", memberSuffix, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsMemberSuffix(memberSuffix)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberSuffix.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsMemberSuffix(memberSuffix As DbsMemberSuffix)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@MemberSuffixId", memberSuffix.MemberSuffixId)
            .AddWithValue("@MemberSuffixName", memberSuffix.MemberSuffixName)
            .AddWithValue("@SortSeq", memberSuffix.SortSeq)

        End With

    End Sub

    Private Sub DeleteDbsMemberSuffix(memberSuffixId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberSuffixId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsMemberSuffix", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@MemberSuffixId", memberSuffixId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsMemberSuffixBody
    Inherits DbsMemberSuffix

End Class
