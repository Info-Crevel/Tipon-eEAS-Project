<RoutePrefix("api")>
Public Class DB1000_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetSSSRanges")>
    <Route("sssRanges")>
    <HttpGet>
    Public Function GetSSSRanges() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DB1000_All")
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

    <SymAuthorization("GetSSSRange")>
    <Route("sssRanges/{sssRangeId}")>
    <HttpGet>
    Public Function GetSSSRange(sssRangeId As Integer) As IHttpActionResult

        If sssRangeId <= 0 Then
            Throw New ArgumentException("SSS Range ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS1000")
                With _direct
                    .AddParameter("SSSRangeId", sssRangeId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateSSSRange")>
    <Route("sssRanges/{currentUserId}")>
    <HttpPost>
    Public Function CreateSSSRange(currentUserId As Integer, <FromBody> sssrange As DbsSSSRangeBody) As IHttpActionResult

        If sssrange.sssRangeId <> -1 Then
            Throw New ArgumentException("SSS Range ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _sssRangeId As Integer = SysLib.GetNextSequence("SssRangeId")

            sssrange.sssRangeId = _sssRangeId

            '
            ' Load proposed values from payload
            '
            Dim _dbsSSSRange As New DbsSSSRange

            Me.LoadDbsSSSRange(sssrange, _dbsSSSRange)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsSSSRange(_dbsSSSRange)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSSSRange(_sssRangeId), Results.OkNegotiatedContentResult(Of DataTable))
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifySSSRange")>
    <Route("sssRanges/{sssRangeId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifySkill(sssRangeId As Integer, currentUserId As Integer, <FromBody> sssRange As DbsSSSRangeBody) As IHttpActionResult

        If sssRangeId <= 0 Then
            Throw New ArgumentException("SSS Range ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsSSSRange As New DbsSSSRange

            Me.LoadDbsSSSRange(sssRange, _dbsSSSRange)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsSSSRange(_dbsSSSRange)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSSSRange(sssRangeId), Results.OkNegotiatedContentResult(Of DataTable))
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("RemoveSSSRange")>
    <Route("sssRanges/{sssRangeId}/{lockId}/{currentUserId}")>
    <HttpDelete>
    Public Function RemoveSSSRange(sssRangeId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

        If sssRangeId <= 0 Then
            Throw New ArgumentException("SSS Range ID is required.")
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

                Me.DeleteDbsSSSRange(sssRangeId, lockId)

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

    Private Sub LoadDbsSSSRage(sssRange As DbsSSSRangeBody, dbsSSSRange As DbsSSSRange)

        DataLib.ScatterValues(sssRange, dbsSSSRange)

    End Sub
    Private Sub LoadDbsSSSRange(row As DataRow, DbsSSSRange As DbsSSSRange)

        With DbsSSSRange
            .SSSRangeId = row.ToInt32("SSSRangeId")
            .StartAmount = row.ToDecimal("StartAmount")
            .EndAmount = row.ToDecimal("EndAmount")
            .RangeOperatorId = row.ToInt32("RangeOperatorId")
            .SSEC = row.ToDecimal("SSEC")
            .WISP = row.ToDecimal("WISP")
            .SSER = row.ToDecimal("SSER")
            .SSEE = row.ToDecimal("SSEE")
            .ECER = row.ToDecimal("ECER")
            .ECEE = row.ToDecimal("ECEE")
            .WISPER = row.ToDecimal("WISPER")
            .WISPEE = row.ToDecimal("WISPEE")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsSSSRange(sssRange As DbsSSSRange)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsSSSRange", sssRange, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsSSSRange(sssRange)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateDbsSSSRange(sssRange As DbsSSSRange)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("SSSRangeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsSSSRange", sssRange, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsSSSRange(sssRange)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(sssRange.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsSSSRange(sssRange As DbsSSSRange)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@SSSRangeId", sssRange.SSSRangeId)
            .AddWithValue("@StartAmount", sssRange.StartAmount)
            .AddWithValue("@EndAmount", sssRange.EndAmount)
            .AddWithValue("@RangeOperatorId", sssRange.RangeOperatorId)
            .AddWithValue("@SSEC", sssRange.SSEC)
            .AddWithValue("@WISP", sssRange.WISP)
            .AddWithValue("@SSER", sssRange.SSER)
            .AddWithValue("@SSEE", sssRange.SSEE)
            .AddWithValue("@ECER", sssRange.ECER)
            .AddWithValue("@ECEE", sssRange.ECEE)
            .AddWithValue("@WISPER", sssRange.WISPER)
            .AddWithValue("@WISPEE", sssRange.WISPEE)
            .AddWithValue("@SortSeq", sssRange.SortSeq)

        End With

    End Sub

    Private Sub DeleteDbsSSSRange(sssRangeId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("SSSRangeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsSSSRange", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@SSSRangeId", sssRangeId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsSSSRangeBody
    Inherits DbsSSSRange

End Class

