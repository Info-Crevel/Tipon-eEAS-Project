<RoutePrefix("api")>
Public Class DBS0570_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetRegionRate")>
    <Route("region-rates/{regionId}")>
    <HttpGet>
    Public Function GetRegionRate(regionId As String) As IHttpActionResult

        If String.IsNullOrEmpty(regionId) Then
            Throw New ArgumentException("Region ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.DBS0570")
                With _direct
                    .AddParameter("RegionId", regionId)

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "region"
                            .Tables(1).TableName = "rateDetails"
                        End With

                        Return Me.Ok(_dataSet)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("ModifyRegionRate")>
   <Route("region-rates/{regionId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyRegionRate(regionId As String, currentUserId As Integer, <FromBody> regionRate As DbsRegionRateBody) As IHttpActionResult

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _dbsRegion As New DbsRegion
         Dim _dbsRegionRateList As New DbsRegionRateList

         Me.LoadDbsRegion(regionRate, _dbsRegion)

         For Each _detail As DbsRegionRate In regionRate.Details
            _dbsRegionRateList.Add(_detail)
         Next

         '
         ' Load old values from DB
         '
         Dim _dbsRegionOld As New DbsRegion
         Dim _dbsRegionRateListOld As New DbsRegionRateList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetRegionRate(regionId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("region").Rows(0)
            Me.LoadDbsRegion(_row, _dbsRegionOld)
            Me.LoadDbsRegionRateList(_dataSet.Tables("rateDetails").Rows, _dbsRegionRateListOld)
         End Using

         '
         ' Apply changes, save to DB
         '


         Dim _removeDetailCount As Integer
         Dim _addDetailCount As Integer
         Dim _editDetailCount As Integer


         ' Mark details for deletion if not found in new list

         For Each _old As DbsRegionRate In _dbsRegionRateListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As DbsRegionRate In _dbsRegionRateList
               If _new.RegionRateId = _old.RegionRateId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeDetailCount = _removeDetailCount + 1
            End If
         Next


         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As DbsRegionRate In _dbsRegionRateList
            _new.LogActionId = LogActionId.Add
            For Each _old As DbsRegionRate In _dbsRegionRateListOld
               If _new.RegionRateId = _old.RegionRateId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     If .StartDate <> _old.StartDate OrElse .RateAmount <> _old.RateAmount Then
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

         Dim _dbsRegionRateListNew As New DbsRegionRateList      ' for adding new Barangays

         If _addDetailCount > 0 Then
            Dim _dbsRegionRate As DbsRegionRate

            For Each _new As DbsRegionRate In _dbsRegionRateList
               If _new.LogActionId = LogActionId.Add Then
                  _dbsRegionRate = New DbsRegionRate
                  _dbsRegionRateListNew.Add(_dbsRegionRate)
                  DataLib.ScatterValues(_new, _dbsRegionRate)
               End If
            Next

         End If


         Dim _isDbsRegionChanged As Boolean = Me.HasDbsRegionChanges(_dbsRegionOld, _dbsRegion)


         If Not _isDbsRegionChanged AndAlso _addDetailCount = 0 AndAlso _removeDetailCount = 0 AndAlso _editDetailCount = 0 Then
            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetRegionRate(regionId)
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

            If _removeDetailCount > 0 Then
               Me.DeleteDbsRegionRates(_dbsRegionRateListOld)
            End If

            If _addDetailCount > 0 Then
               Me.InsertDbsRegionRates(_dbsRegionRateListNew)
            End If

            If _editDetailCount > 0 Then
               Me.UpdateDbsRegionRates(_dbsRegionRateList)
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
   Private Sub LoadDbsRegion(region As DbsRegionRateBody, dbsRegion As DbsRegion)

      DataLib.ScatterValues(region, dbsRegion)

   End Sub

   Private Sub LoadDbsRegion(row As DataRow, region As DbsRegion)

        With region
            .RegionId = row.ToString("RegionId")
            .RegionName = row.ToString("RegionName")
        End With

    End Sub

    Private Sub LoadDbsRegionRateList(rows As DataRowCollection, list As DbsRegionRateList)

        Dim _detail As DbsRegionRate
        For Each _row As DataRow In rows
            _detail = New DbsRegionRate

            With _detail
                .RegionRateId = _row.ToInt32("RegionRateId")
                .RegionId = _row.ToString("RegionId")
                .StartDate = _row.ToDate("StartDate")
                .RateAmount = _row.ToDecimal("RateAmount")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub InsertDbsRegion(region As DbsRegion)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRegion", region, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsDbsRegion(region)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub InsertDbsRegionRates(list As DbsRegionRateList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("RegionRateId")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRegionRate", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As DbsRegionRate In list
                Me.AddInsertUpdateParamsDbsRegionRate(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub UpdateDbsRegionRates(list As DbsRegionRateList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("StartDate")
        End With

        With _excludedFields
            .Add("RegionRateId")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRegionRate", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As DbsRegionRate In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsDbsRegionRate(_detail)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub AddInsertUpdateParamsDbsRegion(region As DbsRegion)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@RegionId", region.RegionId)
            .AddWithValue("@RegionName", region.RegionName)
        End With

    End Sub

   Private Sub AddInsertUpdateParamsDbsRegionRate(dtl As DbsRegionRate)

      With DataCore.Command.Parameters
         .Clear()

         '.AddWithValue("@PlatformId", dtl.PlatformId)
         .AddWithValue("@RegionId", dtl.RegionId)
         .AddWithValue("@StartDate", dtl.StartDate)
         .AddWithValue("@RateAmount", dtl.RateAmount)
         '.AddWithValue("@BarangayName", dtl.BarangayName)
      End With

   End Sub
   Private Sub DeleteDbsRegionRates(list As DbsRegionRateList)

        With DataCore.Command
            .CommandText = "DELETE dbo.DbsRegionRate WHERE RegionRateId=@RegionRateId"
            .CommandType = CommandType.Text

            For Each _old As DbsRegionRate In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@RegionRateId", _old.RegionRateId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Function HasDbsRegionChanges(oldRecord As DbsRegion, newRecord As DbsRegion) As Boolean

        With oldRecord
            If .RegionName <> newRecord.RegionName Then Return True
        End With

        Return False

    End Function

End Class

Public Class DbsRegionRateBody
    Inherits DbsRegion

    Public Property Details As DbsRegionRate()

End Class

