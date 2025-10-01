<RoutePrefix("api")>
Public Class DBS0550_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetPayVariables")>
    <Route("pay-variables")>
    <HttpGet>
    Public Function GetPayVariables() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0550_All")
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

    <SymAuthorization("GetPayVariable")>
    <Route("pay-variables/{payVariableId}")>
    <HttpGet>
    Public Function GetPayVariable(payVariableId As Integer) As IHttpActionResult

        If payVariableId <= 0 Then
            Throw New ArgumentException("Medical Result Type ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0550")
                With _direct
                    .AddParameter("PayVariableId", payVariableId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreatePayVariable")>
    <Route("pay-variables/{currentUserId}")>
    <HttpPost>
    Public Function CreatePayVariable(currentUserId As Integer, <FromBody> payVariable As DbsPayVariableBody) As IHttpActionResult

        If payVariable.PayVariableId <> -1 Then
            Throw New ArgumentException("PayVariable ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _payVariableId As Integer = SysLib.GetNextSequence("PayVariableId")

            payVariable.PayVariableId = _payVariableId

            '
            ' Load proposed values from payload
            '
            Dim _dbsPayVariable As New DbsPayVariable

            Me.LoadDbsPayVariable(payVariable, _dbsPayVariable)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsPayVariable(_dbsPayVariable)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayVariable(_payVariableId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(payVariable.PayVariableId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyPayVariable")>
    <Route("pay-variables/{payVariableId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyPayVariable(payVariableId As Integer, currentUserId As Integer, <FromBody> payVariable As DbsPayVariableBody) As IHttpActionResult

        If payVariableId <= 0 Then
            Throw New ArgumentException("PayVariable ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsPayVariable As New DbsPayVariable

            Me.LoadDbsPayVariable(payVariable, _dbsPayVariable)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsPayVariable(_dbsPayVariable)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayVariable(payVariableId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemovePayVariable")>
   <Route("pay-variables/{payVariableId}")>
   <HttpDelete>
   Public Function RemovePayVariable(payVariableId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payVariableId <= 0 Then
         Throw New ArgumentException("Pay Variable  ID is required.")
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
            Me.DeleteDbsPayVariable(payVariableId, q.LockId)

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
   Private Sub LoadDbsPayVariable(payVariable As DbsPayVariableBody, dbsPayVariable As DbsPayVariable)

      DataLib.ScatterValues(payVariable, dbsPayVariable)

   End Sub
   Private Sub LoadDbsPayVariable(row As DataRow, dbsPayVariable As DbsPayVariable)

        With dbsPayVariable
            .PayVariableId = row.ToInt32("PayVariableId")
            .PayVariableCode = row.ToString("PayVariableCode")
            .PayVariableName = row.ToString("PayVariableName")
            .SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertDbsPayVariable(payVariable As DbsPayVariable)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsPayVariable", payVariable, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsPayVariable(payVariable)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateDbsPayVariable(payVariable As DbsPayVariable)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("PayVariableId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsPayVariable", payVariable, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsPayVariable(payVariable)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payVariable.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsPayVariable(payVariable As DbsPayVariable)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@PayVariableId", payVariable.PayVariableId)
            .AddWithValue("@PayVariableCode", payVariable.PayVariableCode)
            .AddWithValue("@PayVariableName", payVariable.PayVariableName)
            .AddWithValue("@SortSeq", payVariable.SortSeq)
        End With

    End Sub
    Private Sub DeleteDbsPayVariable(payVariableId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("PayVariableId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsPayVariable", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@PayVariableId", payVariableId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsPayVariableBody
    Inherits DbsPayVariable

End Class
