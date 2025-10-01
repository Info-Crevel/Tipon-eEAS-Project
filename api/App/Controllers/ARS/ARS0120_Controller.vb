
Imports ImageMagick

<RoutePrefix("api")>
Public Class ARS0120_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_ARS0120")>
   <Route("references/ars0120")>
   <HttpGet>
   Public Function GetReferences_ARS0120() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0120_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "org"
                     '.Tables(1).TableName = "term"
                     '.Tables(2).TableName = "tax"
                     '.Tables(3).TableName = "atax"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetSourceRequest")>
   <Route("source-requests/{memberRequestId}")>
   <HttpGet>
   Public Function GetSourceRequest(memberRequestId As Integer) As IHttpActionResult

      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0120")
            With _direct
               .AddParameter("memberRequestId", memberRequestId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "sourceRequest"
                     .Tables(1).TableName = "sourceMember"

                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetTargetRequest")>
   <Route("target-requests/{memberRequestId}")>
   <HttpGet>
   Public Function GetTargetRequest(memberRequestId As Integer) As IHttpActionResult

      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0120")
            With _direct
               .AddParameter("memberRequestId", memberRequestId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "targetRequest"
                     .Tables(1).TableName = "targetMember"

                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   '<SymAuthorization("CreatePayee")>
   '<Route("payees/{currentUserId}")>
   '<HttpPost>
   'Public Function CreatePayee(currentUserId As Integer, <FromBody> payee As PayeeBody) As IHttpActionResult

   '   If payee.PayeeId <> -1 Then
   '      Throw New ArgumentException("Payee ID is unrecognized.")
   '   End If

   '   Try

   '      '
   '      ' Assign new ID from sequencer
   '      '
   '      Dim _payeeId As Integer = SysLib.GetNextSequence("PayeeId")

   '      payee.PayeeId = _payeeId
   '      payee.OrgId = 1 'Sample muna
   '      '
   '      ' Load proposed values from payload
   '      '

   '      Dim _apsPayee As New ApsPayee


   '      Me.LoadApsPayee(payee, _apsPayee)

   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.InsertApsPayee(_apsPayee)

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

   '      Return Me.Ok(payee.PayeeId)

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)

   '   End Try

   'End Function

   '<SymAuthorization("ModifyPayee")>
   '<Route("payees/{payeeId}/{currentUserId}")>
   '<HttpPut>
   'Public Function ModifyPayee(payeeId As Integer, currentUserId As Integer, <FromBody> payee As PayeeBody) As IHttpActionResult

   '   If payeeId <= 0 Then
   '      Throw New ArgumentException("Payee ID is required.")
   '   End If

   '   Try
   '      '
   '      ' Load proposed values from payload
   '      '

   '      Dim _apsPayee As New ApsPayee

   '      Me.LoadApsPayee(payee, _apsPayee)

   '      '
   '      ' Load old values from DB
   '      '
   '      Dim _apsPayeeOld As New ApsPayee


   '      Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetPayee(payeeId), Results.OkNegotiatedContentResult(Of DataSet))

   '      Using _dataSet As DataSet = _result.Content
   '         Dim _row As DataRow = _dataSet.Tables("payee").Rows(0)
   '         Me.LoadApsPayee(_row, _apsPayeeOld)

   '      End Using


   '      Dim _transaction As SqlTransaction = Nothing
   '      Dim _successFlag As Boolean

   '      Try
   '         If Not DataCore.Connect(_databaseId) Then
   '            ' TODO: Cannot connect
   '         End If

   '         _transaction = DataCore.Connection.BeginTransaction()
   '         DataCore.Command.Transaction = _transaction
   '         DataCore.Command.Connection = DataCore.Connection

   '         Me.UpdateApsPayee(_apsPayee)


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

   '<SymAuthorization("RemovePayee")>
   '<Route("payees/{payeeId}/{lockId}/{currentUserId}")>
   '<HttpDelete>
   'Public Function RemovePayee(payeeId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

   '   If payeeId <= 0 Then
   '      Throw New ArgumentException("Payee ID is required.")
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

   '         Me.DeleteApsPayee(payeeId, lockId)

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


   Private Sub LoadApsPayee(payee As PayeeBody, apsPayee As ApsPayee)

      DataLib.ScatterValues(payee, apsPayee)

   End Sub
   Private Sub LoadApsPayee(row As DataRow, payee As ApsPayee)

      With payee
         .PayeeId = row.ToInt32("PayeeId")
         .OrgId = row.ToInt32("OrgId")
         .PayeeName = row.ToString("PayeeName")
         .CheckName = row.ToString("CheckName")
         .PayeeTypeId = row.ToInt32("PayeeTypeId")
         .Address1 = row.ToString("Address1")
         .Address2 = row.ToString("Address2")
         .PostalCode = row.ToString("PostalCode")
         .PhoneNumber = row.ToString("PhoneNumber")
         .MobileNumber = row.ToString("MobileNumber")
         .FaxNumber = row.ToString("FaxNumber")
         .Email = row.ToString("Email")
         .ContactPerson = row.ToString("ContactPerson")
         .Remarks = row.ToString("Remarks")
         .VATFlag = row.ToBoolean("VATFlag")
         .TaxIdNumber = row.ToString("TaxIdNumber")
         .PayableTermId = row.ToInt32("PayableTermId")
         .PayableTaxCode = row.ToInt32("PayableTaxCode")
         .ATaxCode = row.ToString("ATaxCode")
         .WTaxRate = row.ToDecimal("WTaxRate")
         .EWTRate = row.ToDecimal("EWTRate")
         .AccountId = row.ToString("AccountId")
      End With

   End Sub
   Private Sub InsertApsPayee(payee As ApsPayee)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With
        File.WriteAllText("D:\payee1.txt", payee.ToString)
        With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ApsPayee", payee, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsPayee(payee)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateApsPayee(payee As ApsPayee)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("PayeeId")
         .Add("LockId")
      End With

      'With _excludedFields
      '   .Add("Age")
      '   '.Add("Address2")
      'End With
      File.WriteAllText("D:\payee1.txt", payee.ToString)
      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ApsPayee", payee, _keyFields) ', _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsApsPayee(payee)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(payee.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub AddInsertUpdateParamsApsPayee(payee As ApsPayee)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@PayeeId", payee.PayeeId)
         .AddWithValue("@OrgId", payee.OrgId)
         .AddWithValue("@PayeeName", payee.PayeeName)
         .AddWithValue("@CheckName", payee.CheckName)
         .AddWithValue("@PayeeTypeId", payee.PayeeTypeId)
         .AddWithValue("@Address1", payee.Address1)
         .AddWithValue("@Address2", payee.Address2)
         .AddWithValue("@PostalCode", payee.PostalCode)
         .AddWithValue("@PhoneNumber", payee.PhoneNumber)
         .AddWithValue("@MobileNumber", payee.MobileNumber)
         .AddWithValue("@FaxNumber", payee.FaxNumber)
         .AddWithValue("@Email", payee.Email)
         .AddWithValue("@ContactPerson", payee.ContactPerson)
         .AddWithValue("@Remarks", payee.Remarks)
         .AddWithValue("@VATFlag", payee.VATFlag)
         .AddWithValue("@TaxIdNumber", payee.TaxIdNumber)
         .AddWithValue("@PayableTermId", payee.PayableTermId)
         .AddWithValue("@PayableTaxCode", payee.PayableTaxCode)
         .AddWithValue("@ATaxCode", payee.ATaxCode)
         .AddWithValue("@WTaxRate", payee.WTaxRate)
         .AddWithValue("@EWTRate", payee.EWTRate)
         .AddWithValue("@AccountId", payee.AccountId)

      End With

   End Sub

   Private Sub DeleteApsPayee(payeeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("payeeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ApsPayee", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@PayeeId", payeeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class
'Public Class PayeeBody
'   Inherits ApsPayee
'End Class

