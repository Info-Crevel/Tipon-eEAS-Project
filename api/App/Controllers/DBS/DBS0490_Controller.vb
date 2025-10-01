<RoutePrefix("api")>
Public Class DBS0490_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetPayTrxs")>
    <Route("pay-trxs")>
    <HttpGet>
    Public Function GetPayTrxs() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.DBS0490_All")
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

    <SymAuthorization("GetPayTrx")>
    <Route("pay-trxs/{payTrxId}")>
    <HttpGet>
    Public Function GetPayTrx(payTrxId As Integer) As IHttpActionResult

        If payTrxId <= 0 Then
            Throw New ArgumentException("Medical Result Type ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0490")
                With _direct
                    .AddParameter("PayTrxId", payTrxId)

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreatePayTrx")>
    <Route("pay-trxs/{currentUserId}")>
    <HttpPost>
    Public Function CreatePayTrx(currentUserId As Integer, <FromBody> payTrx As DbsPayTrxBody) As IHttpActionResult

        If payTrx.PayTrxId <> -1 Then
            Throw New ArgumentException("PayTrx ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _payTrxId As Integer = SysLib.GetNextSequence("PayTrxId")

            payTrx.PayTrxId = _payTrxId

            '
            ' Load proposed values from payload
            '
            Dim _dbsPayTrx As New DbsPayTrx

            Me.LoadDbsPayTrx(payTrx, _dbsPayTrx)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertDbsPayTrx(_dbsPayTrx)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayTrx(_payTrxId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(payTrx.PayTrxId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyPayTrx")>
    <Route("pay-trxs/{payTrxId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyPayTrx(payTrxId As Integer, currentUserId As Integer, <FromBody> payTrx As DbsPayTrxBody) As IHttpActionResult

        If payTrxId <= 0 Then
            Throw New ArgumentException("PayTrx ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _dbsPayTrx As New DbsPayTrx

            Me.LoadDbsPayTrx(payTrx, _dbsPayTrx)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateDbsPayTrx(_dbsPayTrx)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetPayTrx(payTrxId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemovePayTrx")>
   <Route("pay-trxs/{payTrxId}")>
   <HttpDelete>
   Public Function RemoveReligion(payTrxId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If payTrxId <= 0 Then
         Throw New ArgumentException("Pay Trx ID is required.")
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
            Me.DeleteDbsPayTrx(payTrxId, q.LockId)

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
   Private Sub LoadDbsPayTrx(payTrx As DbsPayTrxBody, dbsPayTrx As DbsPayTrx)

      DataLib.ScatterValues(payTrx, dbsPayTrx)

   End Sub
   Private Sub LoadDbsPayTrx(row As DataRow, dbsPayTrx As DbsPayTrx)

      With dbsPayTrx
         .PayTrxId = row.ToInt32("PayTrxId")
         .PayTrxCode = row.ToString("PayTrxCode")
         .PayTrxName = row.ToString("PayTrxName")
         .PayGroupFlag = row.ToBoolean("PayGroupFlag")
         .MemberRequestFlag = row.ToBoolean("MemberRequestFlag")
         .AccountId = row.ToInt32("AccountId")
         .TrxFormula = row.ToString("TrxFormula")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

    Private Sub InsertDbsPayTrx(payTrx As DbsPayTrx)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsPayTrx", payTrx, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsPayTrx(payTrx)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateDbsPayTrx(payTrx As DbsPayTrx)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("PayTrxId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsPayTrx", payTrx, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsPayTrx(payTrx)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payTrx.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsPayTrx(payTrx As DbsPayTrx)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@PayTrxId", payTrx.PayTrxId)
         .AddWithValue("@PayTrxCode", payTrx.PayTrxCode)
         .AddWithValue("@PayTrxName", payTrx.PayTrxName)
         .AddWithValue("@PayGroupFlag", payTrx.PayGroupFlag)
         .AddWithValue("@MemberRequestFlag", payTrx.MemberRequestFlag)
         .AddWithValue("@AccountId", payTrx.AccountId)
            .AddWithValue("@TrxFormula", payTrx.TrxFormula.ToNullable)
            .AddWithValue("@SortSeq", payTrx.SortSeq)
      End With

   End Sub
    Private Sub DeleteDbsPayTrx(payTrxId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("PayTrxId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsPayTrx", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@PayTrxId", payTrxId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class DbsPayTrxBody
    Inherits DbsPayTrx

End Class
