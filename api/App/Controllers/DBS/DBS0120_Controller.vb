<RoutePrefix("api")>
Public Class DBS0120_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberType")>
   <Route("member-types/{memberTypeId}")>
   <HttpGet>
   Public Function GetMemberType(memberTypeId As Integer) As IHttpActionResult

      If memberTypeId <= 0 Then
         Throw New ArgumentException("Member Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0120")
            With _direct
               .AddParameter("MemberTypeId", memberTypeId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "memberType"
                     .Tables(1).TableName = "memberTypeDetails"
                     '.Tables(2).TableName = "memberTypeList"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateMemberType")>
   <Route("member-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberType(currentUserId As Integer, <FromBody> memberType As HrsMemberTypeBody) As IHttpActionResult

      If memberType.MemberTypeId <> -1 Then
         Throw New ArgumentException("Member Type ID is unrecognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID
         '
         Dim _memberTypeId As Integer = memberType.MemberTypeId

         '
         ' Load proposed values from payload
         '
         Dim _hrsMemberType As New HrsMemberType
         Dim _hrsMemberTypeQualificationList As New HrsMemberTypeQualificationList

         Me.LoadHrsMemberType(memberType, _hrsMemberType)

         For Each _detail As HrsMemberTypeQualification In memberType.Details
            '_detail.MunicipalityId = _municipalityId
            _hrsMemberTypeQualificationList.Add(_detail)
         Next

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertHrsMemberType(_hrsMemberType)
            Me.InsertHrsMemberTypeQualifications(_hrsMemberTypeQualificationList)

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
         'Return Me.Ok(municipality.MunicipalityId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyMemberType")>
   <Route("member-types/{memberTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyMemberType(memberTypeId As Integer, currentUserId As Integer, <FromBody> memberType As HrsMemberTypeBody) As IHttpActionResult

      If memberTypeId <= 0 Then
         Throw New ArgumentException("Member Type ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _hrsMemberType As New HrsMemberType
         Dim _hrsMemberTypeQualificationList As New HrsMemberTypeQualificationList

         Me.LoadHrsMemberType(memberType, _hrsMemberType)

         For Each _detail As HrsMemberTypeQualification In memberType.Details
            _hrsMemberTypeQualificationList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _hrsMemberTypeOld As New HrsMemberType
         Dim _hrsMemberTypeQualificationListOld As New HrsMemberTypeQualificationList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMemberType(memberTypeId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("memberType").Rows(0)
            Me.LoadHrsMemberType(_row, _hrsMemberTypeOld)
            Me.LoadHrsMemberTypeQualificationList(_dataSet.Tables("memberTypeDetails").Rows, _hrsMemberTypeQualificationListOld)
         End Using

         '
         ' Apply changes, save to DB
         '

         '
         ' DbsMunicipalityDetail
         '

         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As HrsMemberTypeQualification In _hrsMemberTypeQualificationListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As HrsMemberTypeQualification In _hrsMemberTypeQualificationList
               If _new.TypeQualificationDetailId = _old.TypeQualificationDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As HrsMemberTypeQualification In _hrsMemberTypeQualificationList
            _new.LogActionId = LogActionId.Add
            For Each _old As HrsMemberTypeQualification In _hrsMemberTypeQualificationListOld
               If _new.TypeQualificationDetailId = _old.TypeQualificationDetailId Then
                  _new.LogActionId = 0   ' don't add

                  'With _new
                  '    If .BarangayName <> _old.BarangayName Then
                  '        .LogActionId = LogActionId.Edit
                  '    End If
                  'End With

                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then
               _addDetailCount = _addDetailCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editDetailCount = _editDetailCount + 1
            End If

         Next

         Dim _hrsMemberTypeQualificationListNew As New HrsMemberTypeQualificationList      ' for adding new Barangays

         If _addDetailCount > 0 Then
            Dim _hrsMemberTypeQualification As HrsMemberTypeQualification

            For Each _new As HrsMemberTypeQualification In _hrsMemberTypeQualificationList
               If _new.LogActionId = LogActionId.Add Then
                  _hrsMemberTypeQualification = New HrsMemberTypeQualification
                  _hrsMemberTypeQualificationListNew.Add(_hrsMemberTypeQualification)
                  DataLib.ScatterValues(_new, _hrsMemberTypeQualification)
                  '_dbsMunicipalityDetail.MunicipalityId = _dbsMunicipality.MunicipalityId       ' not needed
               End If
            Next

         End If


         Dim _isHrsMemberTypeChanged As Boolean = Me.HasHrsMemberTypeChanges(_hrsMemberTypeOld, _hrsMemberType)


         If Not _isHrsMemberTypeChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetMemberType(memberTypeId)
         End If


         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean



         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection


            If _isHrsMemberTypeChanged Then

               Me.UpdateHrsMemberType(_hrsMemberType)

            End If

            '
            ' DbsMunicipalityDetail
            '
            If _removeDetailCount > 0 Then
               Me.DeleteHrsMemberTypeQualifications(_hrsMemberTypeQualificationListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertHrsMemberTypeQualifications(_hrsMemberTypeQualificationListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateHrsMemberTypeQualifications(_hrsMemberTypeQualificationList)
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

   <SymAuthorization("RemoveMemberType")>
   <Route("member-types/{memberTypeId}")>
   <HttpDelete>
   Public Function RemoveMemberType(memberTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If memberTypeId <= 0 Then
         Throw New ArgumentException("Member Type ID is required.")
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

            Me.DeleteHrsMemberType(memberTypeId, q.LockId)

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
   Private Sub LoadHrsMemberType(memberType As HrsMemberTypeBody, hrsMemberType As HrsMemberType)

      DataLib.ScatterValues(memberType, hrsMemberType)

   End Sub

   Private Sub LoadHrsMemberType(row As DataRow, memberType As HrsMemberType)

      With memberType
         .MemberTypeId = row.ToInt32("MemberTypeId")
         .MemberTypeName = row.ToString("MemberTypeName")
      End With

   End Sub

   Private Sub LoadHrsMemberTypeQualificationList(rows As DataRowCollection, list As HrsMemberTypeQualificationList)

      Dim _detail As HrsMemberTypeQualification
      For Each _row As DataRow In rows
         _detail = New HrsMemberTypeQualification

         With _detail
            .MemberTypeId = _row.ToInt32("MemberTypeId")
            .TypeQualificationName = _row.ToString("TypeQualificationName")
            .SortSeq = _row.ToInt32("SortSeq")
            '.BarangayName = _row.ToString("BarangayName")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertHrsMemberType(memberType As HrsMemberType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberType", memberType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsMemberType(memberType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertHrsMemberTypeQualifications(list As HrsMemberTypeQualificationList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("TypeQualificationDetailId")
         '.Add("MemberTypeId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberTypeQualification", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsMemberTypeQualification In list
            Me.AddInsertUpdateParamsHrsMemberTypeQualification(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateHrsMemberType(memberType As HrsMemberType)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberTypeId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberType", memberType, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsMemberType(memberType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateHrsMemberTypeQualifications(list As HrsMemberTypeQualificationList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberTypeId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberTypeQualification", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsMemberTypeQualification In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsHrsMemberTypeQualification(_detail)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsHrsMemberType(memberType As HrsMemberType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberTypeId", memberType.MemberTypeId)
         .AddWithValue("@MemberTypeName", memberType.MemberTypeName)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsHrsMemberTypeQualification(dtl As HrsMemberTypeQualification)

      With DataCore.Command.Parameters
         .Clear()

         '.AddWithValue("@TypeQualificationId", dtl.TypeQualificationId)
         .AddWithValue("@MemberTypeId", dtl.MemberTypeId)
         .AddWithValue("@TypeQualificationName", dtl.TypeQualificationName)
         .AddWithValue("@SortSeq", dtl.SortSeq)
         '.AddWithValue("@BarangayName", dtl.BarangayName)
      End With

   End Sub

   Private Sub DeleteHrsMemberType(memberTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsMemberType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@memberTypeId", memberTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteHrsMemberTypeQualifications(list As HrsMemberTypeQualificationList)

      With DataCore.Command
         .CommandText = "DELETE dbo.HrsMemberTypeQualification WHERE memberTypeId=@memberTypeId"
         .CommandType = CommandType.Text

         For Each _old As HrsMemberTypeQualification In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberTypeId", _old.MemberTypeId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasHrsMemberTypeChanges(oldRecord As HrsMemberType, newRecord As HrsMemberType) As Boolean

      With oldRecord
         If .MemberTypeName <> newRecord.MemberTypeName Then Return True
         If .MemberTypeId <> newRecord.MemberTypeId Then Return True
      End With

      Return False

   End Function

End Class

Public Class HrsMemberTypeBody
   Inherits HrsMemberType

   Public Property Details As HrsMemberTypeQualification()

End Class

