<RoutePrefix("api")>
Public Class DBS0470_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetEngagementTypes")>
    <Route("engagement-types")>
    <HttpGet>
    Public Function GetEngagementTypes() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0470_All")
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

    <SymAuthorization("GetEngagementType")>
    <Route("engagement-types/{engagementTypeId}")>
    <HttpGet>
    Public Function GetEngagementType(engagementTypeId As Integer) As IHttpActionResult

        If engagementTypeId <= 0 Then
            Throw New ArgumentException("Engagement Type ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0470")
                With _direct
                    .AddParameter("EngagementTypeId", engagementTypeId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateEngagementType")>
    <Route("engagement-types/{currentUserId}")>
    <HttpPost>
    Public Function CreateEngagementType(currentUserId As Integer, <FromBody> engagementType As DbsEngagementTypeBody) As IHttpActionResult

        If engagementType.EngagementTypeId <> -1 Then
            Throw New ArgumentException("Engagement Type ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _engagementTypeId As Integer = SysLib.GetNextSequence("EngagementTypeId")

            engagementType.EngagementTypeId = _engagementTypeId

            '
            ' Load proposed values from payload
            '
            Dim _dbsEngagementType As New DbsEngagementType

            Me.LoadDbsEngagementType(engagementType, _dbsEngagementType)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsEngagementType(_dbsEngagementType)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEngagementType(_engagementTypeId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(engagementType.EngagementTypeId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyEngagementType")>
    <Route("engagement-types/{engagementTypeId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyEngagementType(engagementTypeId As Integer, currentUserId As Integer, <FromBody> engagementType As DbsEngagementTypeBody) As IHttpActionResult

        If engagementTypeId <= 0 Then
            Throw New ArgumentException("Engagement Type ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsEngagementType As New DbsEngagementType

            Me.LoadDbsEngagementType(engagementType, _dbsEngagementType)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsEngagementType(_dbsEngagementType)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetEngagementType(engagementTypeId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("RemoveEngagementType")>
    <Route("engagement-types/{engagementTypeId}")>
    <HttpDelete>
    Public Function RemoveEngagementType(engagementTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

        If engagementTypeId <= 0 Then
            Throw New ArgumentException("Engagement Type ID is required.")
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

                Me.DeleteDbsEngagementType(engagementTypeId, q.LockId)

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
    Private Sub LoadDbsEngagementType(engagementType As DbsEngagementTypeBody, dbsEngagementType As DbsEngagementType)

        DataLib.ScatterValues(engagementType, dbsEngagementType)

    End Sub
    Private Sub LoadDbsEngagementType(row As DataRow, dbsEngagementType As DbsEngagementType)

        With dbsEngagementType
            .EngagementTypeId = row.ToInt32("EngagementTypeId")
            .EngagementTypeCode = row.ToString("EngagementTypeCode")
            .EngagementTypeName = row.ToString("EngagementTypeName")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

   Private Sub InsertDbsEngagementType(engagementType As DbsEngagementType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsEngagementType", engagementType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsEngagementType(engagementType)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateDbsEngagementType(engagementType As DbsEngagementType)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("EngagementTypeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsEngagementType", engagementType, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsEngagementType(engagementType)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(engagementType.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsEngagementType(engagementType As DbsEngagementType)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@EngagementTypeId", engagementType.EngagementTypeId)
            .AddWithValue("@EngagementTypeCode", engagementType.EngagementTypeCode)
            .AddWithValue("@EngagementTypeName", engagementType.EngagementTypeName)
            .AddWithValue("@SortSeq", engagementType.SortSeq)
        End With

    End Sub
    Private Sub DeleteDbsEngagementType(engagementTypeId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("EngagementTypeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsEngagementType", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@EngagementTypeId", engagementTypeId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class
Public Class DbsEngagementTypeBody
   Inherits DbsEngagementType

End Class
