<RoutePrefix("api")>
Public Class DBS0910_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetWorkSchedules")>
    <Route("schedules")>
    <HttpGet>
    Public Function GetDisabilities() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0910_All")
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

    <SymAuthorization("GetWorkSchedules")>
    <Route("schedules/{scheduleId}")>
    <HttpGet>
    Public Function GetSchedules(scheduleId As Integer) As IHttpActionResult

        If scheduleId <= 0 Then
            Throw New ArgumentException("Schedule ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0910")
                With _direct
                    .AddParameter("scheduleId", scheduleId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateWorkSchedule")>
    <Route("schedules/{currentUserId}")>
    <HttpPost>
    Public Function CreateWorkSchedule(currentUserId As Integer, <FromBody> schedule As DbsWorkScheduleBody) As IHttpActionResult

        If schedule.ScheduleId <> -1 Then
            Throw New ArgumentException("Disability ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _scheduleId As Integer = SysLib.GetNextSequence("ScheduleId")

            schedule.ScheduleId = _scheduleId

            '
            ' Load proposed values from payload
            '
            Dim _dbsWorkSchedule As New DbsWorkSchedule

            Me.LoadDbsWorkSchedule(schedule, _dbsWorkSchedule)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsWorkSchedule(_dbsWorkSchedule)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSchedules(_scheduleId), Results.OkNegotiatedContentResult(Of DataTable))
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyWorkSchedule")>
    <Route("schedules/{scheduleId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyWorkSchedule(scheduleId As Integer, currentUserId As Integer, <FromBody> Schedule As DbsWorkScheduleBody) As IHttpActionResult
        File.WriteAllText("d:\1.txt", Schedule.ToString)
        If scheduleId <= 0 Then
            Throw New ArgumentException("Schedule ID is required.")
        End If
        File.WriteAllText("d:\2.txt", Schedule.ToString)
        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsWorkSchedule As New DbsWorkSchedule

            Me.LoadDbsWorkSchedule(Schedule, _dbsWorkSchedule)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean
            File.WriteAllText("d:\3.txt", Schedule.ToString)
            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsWorkSchedule(_dbsWorkSchedule)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetSchedules(scheduleId), Results.OkNegotiatedContentResult(Of DataTable))
            Return Me.Ok(_result.Content)
            File.WriteAllText("d:\4.txt", Schedule.ToString)
        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("RemoveWorkSchedule")>
    <Route("schedules/{scheduleId}")>
    <HttpDelete>
    Public Function RemoveWorkSchedule(scheduleId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

        If scheduleId <= 0 Then
            Throw New ArgumentException("Schedule ID is required.")
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

                Me.DeleteDbsWorkSchedule(scheduleId, q.LockId)

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

    Private Sub LoadDbsWorkSchedule(schedule As DbsWorkScheduleBody, dbsWorkSchedule As DbsWorkSchedule)

        DataLib.ScatterValues(schedule, dbsWorkSchedule)

    End Sub

    Private Sub LoadDbsWorkSchedule(row As DataRow, dbsWorkSchedule As DbsWorkSchedule)

        With dbsWorkSchedule
            .ScheduleId = row.ToInt32("ScheduleId")
            .ScheduleCode = row.ToString("ScheduleCode")
            .ScheduleIn = row.ToString("ScheduleIn")
            .ScheduleOut = row.ToString("ScheduleOut")
            .WorkingHours = row.ToInt32("WorkingHours")
            .UnpaidBreak = row.ToInt32("UnpaidBreak")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsWorkSchedule(schedule As DbsWorkSchedule)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsWorkSchedule", schedule, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsWorkSchedule(schedule)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateDbsWorkSchedule(schedule As DbsWorkSchedule)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ScheduleId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsWorkSchedule", schedule, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsWorkSchedule(schedule)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(schedule.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsWorkSchedule(schedule As DbsWorkSchedule)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ScheduleId", schedule.ScheduleId)
            .AddWithValue("@ScheduleCode", schedule.ScheduleCode)
            .AddWithValue("@ScheduleIn", schedule.ScheduleIn)
            .AddWithValue("@ScheduleOut", schedule.ScheduleOut)
            .AddWithValue("@WorkingHours", schedule.WorkingHours)
            .AddWithValue("@UnpaidBreak", schedule.UnpaidBreak)
            .AddWithValue("@SortSeq", schedule.SortSeq)

        End With

    End Sub

    Private Sub DeleteDbsWorkSchedule(scheduleId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ScheduleId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsWorkSchedule", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ScheduleId", scheduleId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsWorkScheduleBody
    Inherits DbsWorkSchedule

End Class

