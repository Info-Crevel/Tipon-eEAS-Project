<RoutePrefix("api")>
Public Class HRS0040_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    '<SymAuthorization("GetReferences_HRS0040")>
    '<Route("references/hrs0040")>
    '<HttpGet>
    'Public Function GetReferences_HRS0040() As IHttpActionResult

    '    Try
    '        Using _direct As New SqlDirect("web.HRS0040_References")
    '            With _direct

    '                Using _dataSet As DataSet = _direct.ExecuteDataSet()
    '                    With _dataSet
    '                        .Tables(0).TableName = "member"
    '                        .Tables(1).TableName = "user"
    '                    End With

    '                    Return Me.Ok(_dataSet)

    '                End Using

    '            End With
    '        End Using

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function

    <SymAuthorization("GetRecruiters")>
    <Route("recruiters")>
    <HttpGet>
    Public Function GetRecruiters() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.HRS0040_All")
                With _direct
                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        Return Me.Ok(_dataTable)
                    End Using
                    'Using _dataSet As DataSet = _direct.ExecuteDataSet()
                    '    With _dataSet
                    '        .Tables(0).TableName = "recruiter"
                    '        .Tables(1).TableName = "member"
                    '        .Tables(2).TableName = "user"
                    '    End With

                    '    Return Me.Ok(_dataSet)
                    'End Using


                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetRecruiter")>
    <Route("recruiters/{recruiterId}")>
   <HttpGet>
   Public Function GetRecruiter(recruiterId As Integer) As IHttpActionResult

      If recruiterId <= 0 Then
         Throw New ArgumentException("Recruiter ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.HRS0040")
            With _direct
               .AddParameter("RecruiterId", recruiterId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

               'Using _dataSet As DataSet = _direct.ExecuteDataSet()
               '    With _dataSet
               '        .Tables(0).TableName = "recruiter"
               '        .Tables(1).TableName = "member"
               '        .Tables(2).TableName = "user"
               '    End With

               '    Return Me.Ok(_dataSet)
               'End Using



            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateRecruiter")>
    <Route("recruiters/{currentUserId}")>
    <HttpPost>
    Public Function CreateRecruiter(currentUserId As Integer, <FromBody> recruiter As HrsRecruiterBody) As IHttpActionResult

        If recruiter.RecruiterId <> -1 Then
            Throw New ArgumentException("Recruiter ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _recruiterId As Integer = SysLib.GetNextSequence("RecruiterId")

            recruiter.RecruiterId = _recruiterId

            '
            ' Load proposed values from payload
            '
            Dim _hrsRecruiter As New HrsRecruiter

            Me.LoadHrsRecruiter(recruiter, _hrsRecruiter)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertHrsRecruiter(_hrsRecruiter)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRecruiter(_recruiterId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            'Return Me.Ok(recruiter.RecruiterId)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyRecruiter")>
    <Route("recruiters/{recruiterId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyRecruiter(recruiterId As Integer, currentUserId As Integer, <FromBody> recruiter As HrsRecruiterBody) As IHttpActionResult

        If recruiterId <= 0 Then
            Throw New ArgumentException("Recruiter ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _hrsRecruiter As New HrsRecruiter

            Me.LoadHrsRecruiter(recruiter, _hrsRecruiter)

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateHrsRecruiter(_hrsRecruiter)

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

            Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRecruiter(recruiterId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(True)
            Return Me.Ok(_result.Content)

        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("RemoveRecruiter")>
   <Route("recruiters/{recruiterId}")>
   <HttpDelete>
   Public Function RemoveRecruiter(recruiterId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If recruiterId <= 0 Then
         Throw New ArgumentException("Recruiter ID is required.")
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
            Me.DeleteHrsRecruiter(recruiterId, q.LockId)

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

   '<SymAuthorization("RemoveRecruiter")>
   '<Route("recruiters/{recruiterId}/{lockId}/{currentUserId}")>
   '<HttpDelete>
   'Public Function RemoveRecruiter(recruiterId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

   '    If recruiterId <= 0 Then
   '        Throw New ArgumentException("Recruiter ID is required.")
   '    End If

   '    Try

   '        Dim _transaction As SqlTransaction = Nothing
   '        Dim _successFlag As Boolean

   '        Try
   '            If Not DataCore.Connect(_databaseId) Then
   '                ' TODO: Cannot connect
   '            End If

   '            _transaction = DataCore.Connection.BeginTransaction()
   '            DataCore.Command.Transaction = _transaction
   '            DataCore.Command.Connection = DataCore.Connection
   '            Me.DeleteHrsRecruiter(recruiterId, lockId)

   '            _successFlag = True

   '        Catch _exception As Exception
   '            Return Me.BadRequest(_exception.Message)
   '        Finally
   '            With _transaction
   '                If _successFlag Then
   '                    .Commit()
   '                Else
   '                    .Rollback()
   '                End If
   '                .Dispose()
   '            End With

   '            _transaction = Nothing
   '            DataCore.Disconnect()

   '        End Try

   '        Return Me.Ok(True)

   '    Catch _exception As Exception
   '        Return Me.BadRequest(_exception.Message)

   '    End Try

   'End Function
   Private Sub LoadHrsRecruiter(recruiter As HrsRecruiterBody, hrsRecruiter As HrsRecruiter)

        DataLib.ScatterValues(recruiter, hrsRecruiter)

    End Sub
    Private Sub LoadHrsRecruiter(row As DataRow, hrsRecruiter As HrsRecruiter)

        With hrsRecruiter
            .RecruiterId = row.ToInt32("RecruiterId")
            .MemberEmployeeId = row.ToString("MemberEmployeeId")
            .UserId = row.ToInt32("UserId")
            .RecruiterName = row.ToString("RecruiterName")
            '.SortSeq = row.ToInt32("SortSeq")
        End With

    End Sub

    Private Sub InsertHrsRecruiter(recruiter As HrsRecruiter)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsRecruiter", recruiter, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsHrsRecruiter(recruiter)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateHrsRecruiter(recruiter As HrsRecruiter)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("RecruiterId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsRecruiter", recruiter, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsHrsRecruiter(recruiter)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(recruiter.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsHrsRecruiter(recruiter As HrsRecruiter)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@RecruiterId", recruiter.RecruiterId)
            .AddWithValue("@MemberEmployeeId", recruiter.MemberEmployeeId)
            .AddWithValue("@RecruiterName", recruiter.RecruiterName)
            .AddWithValue("@UserId", recruiter.UserId)
        End With

    End Sub
    Private Sub DeleteHrsRecruiter(recruiterId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("RecruiterId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsRecruiter", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@RecruiterId", recruiterId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub

End Class

Public Class HrsRecruiterBody
    Inherits HrsRecruiter

End Class
