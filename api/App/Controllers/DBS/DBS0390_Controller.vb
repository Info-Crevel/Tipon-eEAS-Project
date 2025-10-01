<RoutePrefix("api")>
Public Class DBS0390_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetHolidayProvinces")>
   <Route("holiday-provinces/{regionId}")>
   <HttpGet>
   Public Function GetHolidayProvinces(regionId As Integer) As IHttpActionResult
      Dim _filter As String = "RegionId=" + regionId.ToString
      Dim _dataSource As String = "dbo.DbsProvince"
      Dim _fields As String = "ProvinceId, ProvinceName, RegionId"
      Dim _sort As String = "ProvinceName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetHolidayMunicipalities")>
   <Route("holiday-municipalities/{provinceId}")>
   <HttpGet>
   Public Function GetHolidayMunicipalities(provinceId As Integer) As IHttpActionResult

      Dim _filter As String = "ProvinceId=" + provinceId.ToString
      Dim _dataSource As String = "dbo.QDbsMunicipality"
      Dim _fields As String = "MunicipalityId, MunicipalityName, ProvinceId, RegionId"
      Dim _sort As String = "MunicipalityName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   <SymAuthorization("GetReferences_DBS0390")>
   <Route("references/dbs0390")>
   <HttpGet>
   Public Function GetReferences_DBS0390() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0390_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "type"     ' all defined Transaction Types
                     .Tables(1).TableName = "region"     ' all defined Transaction Types
                     .Tables(2).TableName = "religion"     ' all defined Transaction Types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetHoliday")>
   <Route("holidays/{holidayId}")>
   <HttpGet>
   Public Function GetDbsHoliday(holidayId As Integer) As IHttpActionResult

      If holidayId <= 0 Then
         Throw New ArgumentException("Holiday ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0390")
            With _direct
               .AddParameter("HolidayId", holidayId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "holiday"
                     .Tables(1).TableName = "locations"
                     .Tables(2).TableName = "religions"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateHoliday")>
   <Route("holidays/{currentUserId}")>
   <HttpPost>
   Public Function CreateHoliday(currentUserId As Integer, <FromBody> holiday As DbsHolidayBody) As IHttpActionResult

      If holiday.HolidayId <> -1 Then
         Throw New ArgumentException("Holiday ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _holidayId As Integer = SysLib.GetNextSequence("HolidayId")

         holiday.HolidayId = _holidayId

         '
         ' Load proposed values from payload
         '
         Dim _dbsHoliday As New DbsHoliday
         Dim _dbsHolidayLocationList As New DbsHolidayLocationList
         Dim _dbsHolidayReligionList As New DbsHolidayReligionList

         Me.LoadDbsHoliday(holiday, _dbsHoliday)

         For Each _detail As DbsHolidayLocation In holiday.Locations
            _detail.HolidayId = _holidayId
            _dbsHolidayLocationList.Add(_detail)
         Next


         For Each _detail As DbsHolidayReligion In holiday.Religions
            _detail.HolidayId = _holidayId
            _dbsHolidayReligionList.Add(_detail)
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

            Me.InsertDbsHoliday(_dbsHoliday)

            If _dbsHolidayLocationList.Count > 0 Then
               Me.InsertDbsHolidayLocations(_dbsHolidayLocationList)
            End If

            If _dbsHolidayReligionList.Count > 0 Then
               Me.InsertDbsHolidayReligions(_dbsHolidayReligionList)
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

         'Return Me.Ok(True)
         Return Me.Ok(holiday.HolidayId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyHoliday")>
   <Route("holidays/{holidayId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyHoliday(holidayId As Integer, currentUserId As Integer, <FromBody> holiday As DbsHolidayBody) As IHttpActionResult

      If holidayId <= 0 Then
         Throw New ArgumentException("Holiday ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsHoliday As New DbsHoliday
         Dim _dbsHolidayLocationList As New DbsHolidayLocationList
         Dim _dbsHolidayReligionList As New DbsHolidayReligionList

         Me.LoadDbsHoliday(holiday, _dbsHoliday)

         For Each _detail As DbsHolidayLocation In holiday.Locations
            _dbsHolidayLocationList.Add(_detail)
         Next

         For Each _detail As DbsHolidayReligion In holiday.Religions
            _dbsHolidayReligionList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsHolidayOld As New DbsHoliday
         Dim _dbsHolidayLocationListOld As New DbsHolidayLocationList
         Dim _dbsHolidayReligionListOld As New DbsHolidayReligionList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetDbsHoliday(holidayId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("holiday").Rows(0)
            Me.LoadDbsHoliday(_row, _dbsHolidayOld)
            Me.LoadDbsHolidayLocationList(_dataSet.Tables("locations").Rows, _dbsHolidayLocationListOld)
            Me.LoadDbsHolidayReligionList(_dataSet.Tables("religions").Rows, _dbsHolidayReligionListOld)

         End Using


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsHolidayLocation In _dbsHolidayLocationListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsHolidayLocation In _dbsHolidayLocationList
               If _new.HolidayLocationDetailId = _old.HolidayLocationDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsHolidayLocation In _dbsHolidayLocationList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsHolidayLocation In _dbsHolidayLocationListOld
               If _new.HolidayLocationDetailId = _old.HolidayLocationDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .RegionId <> _old.RegionId OrElse .ProvinceId <> _old.ProvinceId OrElse .MunicipalityId <> _old.MunicipalityId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



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

         Dim _dbsHolidayLocationListNew As New DbsHolidayLocationList      ' for adding new Template Details

         If _addDetailCount > 0 Then
            Dim _dbsHolidayLocation As DbsHolidayLocation

            For Each _new As DbsHolidayLocation In _dbsHolidayLocationList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsHolidayLocation = New DbsHolidayLocation
                  _dbsHolidayLocationListNew.Add(_dbsHolidayLocation)
                  DataLib.ScatterValues(_new, _dbsHolidayLocation)
                  _dbsHolidayLocation.HolidayId = _dbsHoliday.HolidayId
               End If
            Next

         End If

         'Religion

         Dim _removeReligionCount As Integer
         Dim _addReligionCount As Integer
         Dim _editReligionCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As DbsHolidayReligion In _dbsHolidayReligionListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsHolidayReligion In _dbsHolidayReligionList
               If _new.HolidayReligionDetailId = _old.HolidayReligionDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeReligionCount = _removeReligionCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsHolidayReligion In _dbsHolidayReligionList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsHolidayReligion In _dbsHolidayReligionListOld
               If _new.HolidayReligionDetailId = _old.HolidayReligionDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .ReligionId <> _old.ReligionId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addReligionCount = _addReligionCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editReligionCount = _editReligionCount + 1
            End If

         Next

         Dim _dbsHolidayReligionListNew As New DbsHolidayReligionList      ' for adding new Template Details

         If _addReligionCount > 0 Then
            Dim _dbsHolidayReligion As DbsHolidayReligion

            For Each _new As DbsHolidayReligion In _dbsHolidayReligionList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsHolidayReligion = New DbsHolidayReligion
                  _dbsHolidayReligionListNew.Add(_dbsHolidayReligion)
                  DataLib.ScatterValues(_new, _dbsHolidayReligion)
                  _dbsHolidayReligion.HolidayId = _dbsHoliday.HolidayId
               End If
            Next

         End If



         Dim _isDbsHolidayChanged As Boolean = Me.HasDbsHolidayChanges(_dbsHolidayOld, _dbsHoliday)

         If Not _isDbsHolidayChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 AndAlso _addReligionCount = 0 AndAlso _removeReligionCount = 0 AndAlso _editReligionCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetDbsHoliday(holidayId)
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

            If _isDbsHolidayChanged Then
               Me.UpdateDbsHoliday(_dbsHoliday)
            End If

            If _removeDetailCount > 0 Then
               Me.DeleteDbsHolidayLocations(_dbsHolidayLocationListOld)

            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsHolidayLocations(_dbsHolidayLocationListNew)

            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsHolidayLocations(_dbsHolidayLocationList)

            End If

            If _removeReligionCount > 0 Then
               Me.DeleteDbsHolidayReligions(_dbsHolidayReligionListOld)

            End If

            If _addReligionCount > 0 Then
               Me.InsertDbsHolidayReligions(_dbsHolidayReligionListNew)

            End If

            If _editReligionCount > 0 Then
               Me.UpdateDbsHolidayReligions(_dbsHolidayReligionList)

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

   <SymAuthorization("RemoveHoliday")>
   <Route("holidays/{holidayId}")>
   <HttpDelete>
   Public Function RemoveHoliday(holidayId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If holidayId <= 0 Then
         Throw New ArgumentException("Holiday ID is required.")
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

            Me.DeleteDbsHoliday(holidayId, q.LockId)

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

   Private Sub LoadDbsHoliday(holiday As DbsHolidayBody, dbsHoliday As DbsHoliday)

      DataLib.ScatterValues(holiday, dbsHoliday)

   End Sub

   Private Sub LoadDbsHoliday(row As DataRow, holiday As DbsHoliday)

      With holiday
         .HolidayId = row.ToInt32("HolidayId")
         .HolidayName = row.ToString("HolidayName")
         .DayTypeId = row.ToInt32("DayTypeId")
         .HolidayDate = row.ToDate("HolidayDate")
         .NationalFlag = row.ToBoolean("NationalFlag")
         .Remarks = row.ToString("Remarks")
      End With

   End Sub

   Private Sub LoadDbsHolidayLocationList(rows As DataRowCollection, list As DbsHolidayLocationList)

      Dim _detail As DbsHolidayLocation
      For Each _row As DataRow In rows
         _detail = New DbsHolidayLocation

         With _detail
            .HolidayLocationDetailId = _row.ToInt32("HolidayLocationDetailId")
            .HolidayId = _row.ToInt32("HolidayId")
            .RegionId = _row.ToString("RegionId")
            .ProvinceId = _row.ToString("ProvinceId")
            .MunicipalityId = _row.ToString("MunicipalityId")
            '.BarangayId = _row.ToString("BarangayId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadDbsHolidayReligionList(rows As DataRowCollection, list As DbsHolidayReligionList)

      Dim _detail As DbsHolidayReligion
      For Each _row As DataRow In rows
         _detail = New DbsHolidayReligion

         With _detail
            .HolidayReligionDetailId = _row.ToInt32("HolidayReligionDetailId")
            .HolidayId = _row.ToInt32("HolidayId")
            .ReligionId = _row.ToInt32("ReligionId")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertDbsHoliday(holiday As DbsHoliday)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsHoliday", holiday, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsHoliday(holiday)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub InsertDbsHolidayLocations(list As DbsHolidayLocationList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("HolidayLocationDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsHolidayLocation", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsHolidayLocation In list
            Me.AddInsertUpdateParamsDbsHolidayLocation(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub InsertDbsHolidayReligions(list As DbsHolidayReligionList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("HolidayReligionDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsHolidayReligion", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsHolidayReligion In list
            Me.AddInsertUpdateParamsDbsHolidayReligion(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateDbsHoliday(holiday As DbsHoliday)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("HolidayId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsHoliday", holiday, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsHoliday(holiday)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(holiday.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsHolidayLocations(list As DbsHolidayLocationList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("HolidayLocationDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsHolidayLocation", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsHolidayLocation In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsHolidayLocation(_detail)
               .Parameters.AddWithValue("@HolidayLocationDetailId", _detail.HolidayLocationDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub UpdateDbsHolidayReligions(list As DbsHolidayReligionList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("HolidayReligionDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsHolidayReligion", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As DbsHolidayReligion In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsDbsHolidayReligion(_detail)
               .Parameters.AddWithValue("@HolidayReligionDetailId", _detail.HolidayReligionDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With


   End Sub

   Private Sub AddInsertUpdateParamsDbsHoliday(holiday As DbsHoliday)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@HolidayId", holiday.HolidayId)
         .AddWithValue("@HolidayName", holiday.HolidayName)
         .AddWithValue("@DayTypeId", holiday.DayTypeId)
         .AddWithValue("@HolidayDate", holiday.HolidayDate)
         .AddWithValue("@NationalFlag", holiday.NationalFlag)
         .AddWithValue("@Remarks", holiday.Remarks)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsHolidayLocation(dtl As DbsHolidayLocation)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@HolidayId", dtl.HolidayId)
         .AddWithValue("@RegionId", dtl.RegionId.ToNullable)
         .AddWithValue("@ProvinceId", dtl.ProvinceId.ToNullable)
         .AddWithValue("@MunicipalityId", dtl.MunicipalityId.ToNullable)
         '.AddWithValue("@BarangayId", dtl.BarangayId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsHolidayReligion(dtl As DbsHolidayReligion)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@HolidayId", dtl.HolidayId)
         .AddWithValue("@ReligionId", dtl.ReligionId)
      End With

   End Sub

   Private Sub DeleteDbsHoliday(holidayId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("HolidayId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsHoliday", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@HolidayId", holidayId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub DeleteDbsHolidayLocations(list As DbsHolidayLocationList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsHolidayLocation WHERE HolidayLocationDetailId=@HolidayLocationDetailId"
         .CommandType = CommandType.Text

         For Each _old As DbsHolidayLocation In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@HolidayLocationDetailId", _old.HolidayLocationDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteDbsHolidayReligions(list As DbsHolidayReligionList)

      With DataCore.Command
         .CommandText = "DELETE dbo.DbsHolidayReligion WHERE HolidayReligionDetailId=@HolidayReligionDetailId"
         .CommandType = CommandType.Text

         For Each _old As DbsHolidayReligion In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@HolidayReligionDetailId", _old.HolidayReligionDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub


   Private Function HasDbsHolidayChanges(oldRecord As DbsHoliday, newRecord As DbsHoliday) As Boolean

      With oldRecord
         If .HolidayName <> newRecord.HolidayName Then Return True
         If .DayTypeId <> newRecord.DayTypeId Then Return True
         If .HolidayDate <> newRecord.HolidayDate Then Return True
         If .NationalFlag <> newRecord.NationalFlag Then Return True
         If .Remarks <> newRecord.Remarks Then Return True

      End With

      Return False

   End Function

End Class

Public Class DbsHolidayBody
   Inherits DbsHoliday

   Public Property Locations As DbsHolidayLocation()
   Public Property Religions As DbsHolidayReligion()

End Class
