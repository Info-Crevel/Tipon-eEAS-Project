<RoutePrefix("api")>
Public Class DBS0900_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetReferences_DBS0900")>
   <Route("references/dbs0900")>
   <HttpGet>
   Public Function GetReferences_DBS0900() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0900_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "basic"
                     .Tables(1).TableName = "pbg"
                     .Tables(2).TableName = "phh"
                     .Tables(3).TableName = "sss"
                     .Tables(4).TableName = "wht"
                     .Tables(5).TableName = "deminimis"
                     .Tables(6).TableName = "allowance"
                     .Tables(7).TableName = "separation"
                     .Tables(8).TableName = "yearEnd"
                     .Tables(9).TableName = "sickLeave"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberControl")>
   <Route("member-control")>
   <HttpGet>
   Public Function GetMemberControl() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0900")
            With _direct
               '.AddParameter("MemberControlId", memberControlId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMemberControl")>
   <Route("member-control/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberControl(currentUserId As Integer, <FromBody> memberControl As HrsMemberControlBody) As IHttpActionResult

      'If memberControlId <= 0 Then
      '    Throw New ArgumentException("MemberControl ID is required.")
      'End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _hrsMemberControl As New HrsMemberControl

         Me.LoadHrsMemberControl(memberControl, _hrsMemberControl)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateHrsMemberControl(_hrsMemberControl)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberControl(), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   Private Sub LoadHrsMemberControl(memberControl As HrsMemberControlBody, hrsMemberControl As HrsMemberControl)

      DataLib.ScatterValues(memberControl, hrsMemberControl)

   End Sub

   Private Sub LoadHrsMemberControl(row As DataRow, hrsMemberControl As HrsMemberControl)

      With hrsMemberControl
         .MemberControlId = row.ToInt32("MemberControlId")
         .SssUploadRequiredFlag = row.ToBoolean("SssUploadRequiredFlag")
         .PbgUploadRequiredFlag = row.ToBoolean("PbgUploadRequiredFlag")
         .PhhUploadRequiredFlag = row.ToBoolean("PhhUploadRequiredFlag")
         .WhtUploadRequiredFlag = row.ToBoolean("WhtUploadRequiredFlag")
         .SoloParentUploadRequiredFlag = row.ToBoolean("SoloParentUploadRequiredFlag")
         .PwdUploadRequiredFlag = row.ToBoolean("PwdUploadRequiredFlag")
         .MinimumAge = row.ToInt32("MinimumAge")
         .PoolingValidationPeriod = row.ToInt32("PoolingValidationPeriod")
         .BasicPayTrxId = row.ToInt32("BasicPayTrxId")
         .PbgPayTrxId = row.ToInt32("PbgPayTrxId")
         .PhhPayTrxId = row.ToInt32("PhhPayTrxId")
         .SSSPayTrxId = row.ToInt32("SSSPayTrxId")
         .WhtPayTrxId = row.ToInt32("WhtPayTrxId")
         .DeminimisPayTrxId = row.ToInt32("DeminimisPayTrxId")
         .AllowancePayTrxId = row.ToInt32("AllowancePayTrxId")
         .SeparationPayTrxId = row.ToInt32("SeparationPayTrxId")
         .YearEndPayTrxId = row.ToInt32("YearEndPayTrxId")
         .SickLeavePayTrxId = row.ToInt32("SickLeavePayTrxId")
         .FallOutDays = row.ToInt32("FallOutDays")
         .BackOutDays = row.ToInt32("BackOutDays")
      End With

   End Sub

   'Private Sub InsertHrsMemberControl(memberControl As HrsMemberControl)

   '   Dim _excludedFields As New List(Of String)

   '   With _excludedFields
   '      .Add("LockId")
   '   End With

   '   With DataCore.Command
   '      .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberControl", memberControl, _excludedFields)
   '      .CommandType = CommandType.Text

   '      Me.AddInsertUpdateParamsHrsMemberControl(memberControl)

   '      .ExecuteNonQuery()

   '   End With

   'End Sub

   Private Sub UpdateHrsMemberControl(memberControl As HrsMemberControl)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberControlId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberControl", memberControl, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsMemberControl(memberControl)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberControl.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsHrsMemberControl(memberControl As HrsMemberControl)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberControlId", memberControl.MemberControlId)
         .AddWithValue("@SssUploadRequiredFlag", memberControl.SssUploadRequiredFlag)
         .AddWithValue("@PbgUploadRequiredFlag", memberControl.PbgUploadRequiredFlag)
         .AddWithValue("@PhhUploadRequiredFlag", memberControl.PhhUploadRequiredFlag)
         .AddWithValue("@WhtUploadRequiredFlag", memberControl.WhtUploadRequiredFlag)
         .AddWithValue("@SoloParentUploadRequiredFlag", memberControl.SoloParentUploadRequiredFlag)
         .AddWithValue("@PwdUploadRequiredFlag", memberControl.PwdUploadRequiredFlag)
         .AddWithValue("@MinimumAge", memberControl.MinimumAge)
         .AddWithValue("@PoolingValidationPeriod", memberControl.PoolingValidationPeriod)
         .AddWithValue("@BasicPayTrxId", memberControl.BasicPayTrxId)
         .AddWithValue("@PbgPayTrxId", memberControl.PbgPayTrxId)
         .AddWithValue("@PhhPayTrxId", memberControl.PhhPayTrxId)
         .AddWithValue("@SSSPayTrxId", memberControl.SSSPayTrxId)
         .AddWithValue("@WhtPayTrxId", memberControl.WhtPayTrxId)
         .AddWithValue("@DeminimisPayTrxId", memberControl.DeminimisPayTrxId)
         .AddWithValue("@AllowancePayTrxId", memberControl.AllowancePayTrxId)
         .AddWithValue("@SeparationPayTrxId", memberControl.SeparationPayTrxId)
         .AddWithValue("@YearEndPayTrxId", memberControl.YearEndPayTrxId)
         .AddWithValue("@SickLeavePayTrxId", memberControl.SickLeavePayTrxId)
         .AddWithValue("@FallOutDays", memberControl.FallOutDays)
         .AddWithValue("@BackOutDays", memberControl.BackOutDays)

      End With

   End Sub

   Private Sub DeleteHrsMemberControl(memberControlId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberControlId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsMemberControl", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MemberControlId", memberControlId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class HrsMemberControlBody
   Inherits HrsMemberControl

End Class
