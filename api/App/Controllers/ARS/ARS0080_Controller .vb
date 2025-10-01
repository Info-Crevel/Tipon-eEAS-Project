
Imports ImageMagick

<RoutePrefix("api")>
Public Class ARS0080_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_ARS0080")>
   <Route("references/ars0080")>
   <HttpGet>
   Public Function GetReferences_ARS0080() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0080_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "deductionType"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberDeductionTrx")>
   <Route("member-deductions/{deductionTrxId}")>
   <HttpGet>
   Public Function GetMemberDeductionTrx(deductionTrxId As Integer) As IHttpActionResult

      If deductionTrxId <= 0 Then
         Throw New ArgumentException("Deduction Trx ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0080")
            With _direct
               .AddParameter("DeductionTrxId", deductionTrxId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "deduction"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   '<SymAuthorization("GetHrsApplicantLog")>
   '<Route("applicant/{logId}/{applicantId}/log")>
   '<HttpGet>
   'Public Function GetHrsApplicantLog(logId As Integer, applicantId As Integer) As IHttpActionResult

   '   If applicantId <= 0 Then
   '      Throw New ArgumentException("Applicant ID is required.")
   '   End If

   '   Dim _applicant As String = "web.HRS0030_Log"

   '   Select Case logId
   '      'Case LogTable.Applicant
   '      '   _applicant = "web.HRS0010_MemberLog"
   '      Case LogTable.DocType
   '         _applicant = "web.HRS0030_DocTypeLog"
   '   End Select

   '   Try
   '      Using _direct As New SqlDirect(_applicant)
   '         With _direct
   '            .AddParameter("applicantId", applicantId)

   '            ' Allow DateTime
   '            Dim _settings As New JsonSerializerSettings
   '            With _settings
   '               .ContractResolver = New CamelCasePropertyNamesContractResolver
   '               .DateFormatString = "yyyy-MM-ddTHH:mm"
   '            End With

   '            Using _dataTable As DataTable = _direct.ExecuteDataTable()
   '               'Return Me.Ok(_dataTable)
   '               Return Json(_dataTable, _settings)
   '            End Using

   '         End With
   '      End Using

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function

   <SymAuthorization("CreateMemberDeductionTrx")>
   <Route("member-deductions/{currentUserId}")>
   <HttpPost>
   Public Function CreateApplicant(currentUserId As Integer, <FromBody> deduction As DeductionBody) As IHttpActionResult

      If deduction.DeductionTrxId <> -1 Then
         Throw New ArgumentException("Deduction Trx ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _deductionTrxId As Integer = SysLib.GetNextSequence("DeductionTrxId")

         deduction.DeductionTrxId = _deductionTrxId

         deduction.DeductionTrxStatusId = 1 'Active
         '
         ' Load proposed values from payload
         '

         Dim _arsMemberDeductionTrx As New ArsMemberDeductionTrx


         Me.LoadArsMemberDeductionTrx(deduction, _arsMemberDeductionTrx)


         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertArsMemberDeductionTrx(_arsMemberDeductionTrx)

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

         Return Me.Ok(deduction.DeductionTrxId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyMemberDeductionTrx")>
   <Route("member-deductions/{deductionTrxId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberDeductionTrx(deductionTrxId As Integer, currentUserId As Integer, <FromBody> deduction As DeductionBody) As IHttpActionResult

      If deductionTrxId <= 0 Then
         Throw New ArgumentException("Deduction Trx ID is required.")
      End If


      Try
         '
         ' Load proposed values from payload
         '

         Dim _arsMemberDeductionTrx As New ArsMemberDeductionTrx

         Me.LoadArsMemberDeductionTrx(deduction, _arsMemberDeductionTrx)


         '
         ' Load old values from DB
         '
         Dim _arsMemberDeductionTrxOld As New ArsMemberDeductionTrx

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMemberDeductionTrx(deductionTrxId), Results.OkNegotiatedContentResult(Of DataSet))


         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("deduction").Rows(0)
            Me.LoadArsMemberDeductionTrx(_row, _arsMemberDeductionTrxOld)
            'Me.LoadHrsApplicantDocTypeList(_dataSet.Tables("docs").Rows, _hrsApplicantDocTypeListOld)

         End Using


         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         'Dim _id As Integer

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean


         Dim _isMemberDeductionTrxChanged As Boolean = Me.HasArsMemberDeductionTrxChanges(_arsMemberDeductionTrxOld, _arsMemberDeductionTrx)

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            If _isMemberDeductionTrxChanged Then
               Me.UpdateArsMemberDeductionTrx(_arsMemberDeductionTrx)
            End If

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

   '<SymAuthorization("RemoveApplicant")>
   '<Route("applicants/{applicantId}/{lockId}/{currentUserId}")>
   '<HttpDelete>
   'Public Function RemoveApplicant(applicantId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

   '   If applicantId <= 0 Then
   '      Throw New ArgumentException("Applicant ID is required.")
   '   End If

   '   Try

   '      Dim _currentUserId As Integer = 1      ' System (default)
   '      If currentUserId > 0 Then
   '         _currentUserId = currentUserId
   '      End If

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.DeleteHrsApplicant(applicantId, lockId)

   '         _successFlag = True

   '      Catch _exception As Exception
   '         Return Me.BadRequest(_exception.Message)
   '      Finally
   '         With _transaction
   '            If _successFlag Then
   '               .Commit()
   '            Else
   '               .Rollback()
   '            End If
   '            .Dispose()
   '         End With

   '         _transaction = Nothing
   '         DataCore.Disconnect()

   '      End Try

   '      Return Me.Ok(True)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function
   Private Sub LoadArsMemberDeductionTrx(deduction As DeductionBody, ArsMemberDeductionTrx As ArsMemberDeductionTrx)

      DataLib.ScatterValues(deduction, ArsMemberDeductionTrx)

   End Sub
   Private Sub LoadArsMemberDeductionTrx(row As DataRow, deduction As ArsMemberDeductionTrx)

      With deduction
         .DeductionTrxId = row.ToInt32("DeductionTrxId")
         .MemberId = row.ToInt32("MemberId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .MemberDeductionTypeId = row.ToInt32("MemberDeductionTypeId")
         .DeductionTrxStatusId = row.ToInt32("DeductionTrxStatusId")
         .TotalAmount = row.ToDecimal("TotalAmount")
         .DeductionAmount = row.ToDecimal("DeductionAmount")
         .StartDate = row.ToDate("StartDate")
         .TermsInMonth = row.ToInt32("TermsInMonth")
      End With

   End Sub
   Private Sub InsertArsMemberDeductionTrx(deduction As ArsMemberDeductionTrx)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberDeductionTrx", deduction, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberDeductionTrx(deduction)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateArsMemberDeductionTrx(deduction As ArsMemberDeductionTrx)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DeductionTrxId")
         .Add("LockId")
      End With

      'With _excludedFields
      '   '.Add("Address2")
      'End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberDeductionTrx", deduction, _keyFields) ', _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberDeductionTrx(deduction)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(deduction.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberDeductionTrx(deduction As ArsMemberDeductionTrx)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@DeductionTrxId", deduction.DeductionTrxId)
         .AddWithValue("@MemberId", deduction.MemberId)
         .AddWithValue("@MemberRequestId", deduction.MemberRequestId)
         .AddWithValue("@MemberDeductionTypeId", deduction.MemberDeductionTypeId)
         .AddWithValue("@DeductionTrxStatusId", deduction.DeductionTrxStatusId)
         .AddWithValue("@TotalAmount", deduction.TotalAmount)
         .AddWithValue("@DeductionAmount", deduction.DeductionAmount)
         .AddWithValue("@StartDate", deduction.StartDate)
         .AddWithValue("@TermsInMonth", deduction.TermsInMonth)

      End With

   End Sub
   Private Sub DeleteArs(deductionTrxId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DeductionTrxId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberDeductionTrx", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DeductionTrxId", deductionTrxId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Function HasArsMemberDeductionTrxChanges(oldRecord As ArsMemberDeductionTrx, newRecord As ArsMemberDeductionTrx) As Boolean

      With oldRecord
         If .MemberRequestId <> newRecord.MemberRequestId Then Return True
         If .MemberDeductionTypeId <> newRecord.MemberDeductionTypeId Then Return True
         If .MemberId <> newRecord.MemberId Then Return True
         If .TotalAmount <> newRecord.TotalAmount Then Return True
         If .DeductionAmount <> newRecord.DeductionAmount Then Return True
         If .StartDate <> newRecord.StartDate Then Return True
         If .TermsInMonth <> newRecord.TermsInMonth Then Return True
         If .DeductionTrxStatusId <> newRecord.DeductionTrxStatusId Then Return True
      End With

      Return False

   End Function

End Class

Public Class DeductionBody
   Inherits ArsMemberDeductionTrx
End Class

