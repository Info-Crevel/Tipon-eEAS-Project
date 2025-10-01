<RoutePrefix("api")>
Public Class DBS0580_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetRequestType")>
    <Route("request-types")>
    <HttpGet>
    Public Function GetRequestType() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0580_All")
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

    <SymAuthorization("GetRequestType")>
    <Route("request-types/{requestTypeId}")>
    <HttpGet>
    Public Function GetRequestType(requestTypeId As Integer) As IHttpActionResult

        If requestTypeId <= 0 Then
            Throw New ArgumentException("Request Type ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0580")
                With _direct
                    .AddParameter("RequestTypeId", requestTypeId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateRequestType")>
    <Route("request-types/{currentUserId}")>
    <HttpPost>
    Public Function CreateRequestType(currentUserId As Integer, <FromBody> requestType As DbsRequestTypeBody) As IHttpActionResult

        If requestType.RequestTypeId <> -1 Then
            Throw New ArgumentException("Request Type ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _requestTypeId As Integer = SysLib.GetNextSequence("RequestTypeId")

            requestType.RequestTypeId = _requestTypeId

            '
            ' Load proposed values from payload
            '
            Dim _dbsRequestType As New DbsRequestType

            Me.LoadDbsRequestType(requestType, _dbsRequestType)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsRequestType(_dbsRequestType)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRequestType(_requestTypeId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(requestType.RequestTypeId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyRequestType")>
    <Route("request-types/{requestTypeId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyRequestType(requestTypeId As Integer, currentUserId As Integer, <FromBody> requestType As DbsRequestTypeBody) As IHttpActionResult

        If requestTypeId <= 0 Then
            Throw New ArgumentException("Request Type ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsRequestType As New DbsRequestType

            Me.LoadDbsRequestType(requestType, _dbsRequestType)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsRequestType(_dbsRequestType)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRequestType(requestTypeId), Results.OkNegotiatedContentResult(Of DataTable))

            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemoveRequestType")>
   <Route("request-types/{requestTypeId}")>
   <HttpDelete>
   Public Function RemoveRequestType(requestTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If requestTypeId <= 0 Then
         Throw New ArgumentException("Request Type  ID is required.")
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

            Me.DeleteDbsRequestType(requestTypeId, q.LockId)

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

   Private Sub LoadDbsRequestType(requestType As DbsRequestTypeBody, dbsRequestType As DbsRequestType)

      DataLib.ScatterValues(requestType, dbsRequestType)

   End Sub

   Private Sub LoadDbsRequestType(row As DataRow, dbsRequestType As DbsRequestType)

        With dbsRequestType
            .RequestTypeId = row.ToInt32("RequestTypeId")
            .RequestTypeName = row.ToString("RequestTypeName")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsRequestType(requestType As DbsRequestType)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRequestType", requestType, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsRequestType(requestType)

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub UpdateDbsRequestType(requestType As DbsRequestType)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("RequestTypeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRequestType", requestType, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsRequestType(requestType)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(requestType.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsRequestType(requestType As DbsRequestType)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@RequestTypeId", requestType.RequestTypeId)
            .AddWithValue("@RequestTypeName", requestType.RequestTypeName)
            .AddWithValue("@SortSeq", requestType.SortSeq)

        End With

    End Sub

    Private Sub DeleteDbsRequestType(requestTypeId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("RequestTypeId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsRequestType", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@RequestTypeId", requestTypeId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsRequestTypeBody
    Inherits DbsRequestType

End Class
