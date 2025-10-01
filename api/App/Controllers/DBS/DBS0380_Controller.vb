<RoutePrefix("api")>
Public Class DBS0380_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetDayTypes")>
   <Route("day-types")>
   <HttpGet>
   Public Function GetDayTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0380_All")
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

   <SymAuthorization("GetDayType")>
   <Route("day-types/{dayTypeId}")>
   <HttpGet>
   Public Function GetDayType(dayTypeId As Integer) As IHttpActionResult

      If dayTypeId <= 0 Then
         Throw New ArgumentException("Day Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0380")
            With _direct
               .AddParameter("DayTypeId", dayTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateDayType")>
   <Route("day-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateDayType(currentUserId As Integer, <FromBody> dayType As DbsDayTypeBody) As IHttpActionResult

      If dayType.DayTypeId <> -1 Then
         Throw New ArgumentException("Day Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _dayTypeId As Integer = SysLib.GetNextSequence("DayTypeId")

         dayType.DayTypeId = _dayTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsDayType As New DbsDayType

         Me.LoadDbsDayType(dayType, _dbsDayType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsDayType(_dbsDayType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDayType(_dayTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(dayType.DayTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyDayType")>
   <Route("day-types/{dayTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyDayType(dayTypeId As Integer, currentUserId As Integer, <FromBody> dayType As DbsDayTypeBody) As IHttpActionResult

      If dayTypeId <= 0 Then
         Throw New ArgumentException("Day Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsDayType As New DbsDayType

         Me.LoadDbsDayType(dayType, _dbsDayType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsDayType(_dbsDayType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetDayType(dayTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveDayType")>
   <Route("day-types/{dayTypeId}")>
   <HttpDelete>
   Public Function RemoveDayType(dayTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If dayTypeId <= 0 Then
         Throw New ArgumentException("Day Type ID is required.")
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

            Me.DeleteDbsDayType(dayTypeId, q.LockId)

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

   Private Sub LoadDbsDayType(dayType As DbsDayTypeBody, dbsDayType As DbsDayType)

      DataLib.ScatterValues(dayType, dbsDayType)

   End Sub

   Private Sub LoadDbsDayType(row As DataRow, dbsDayType As DbsDayType)

      With dbsDayType
         .DayTypeId = row.ToInt32("DayTypeId")
         .DayTypeCode = row.ToString("DayTypeCode")
         .DayTypeName = row.ToString("DayTypeName")
         .PremiumPercentage = row.ToDecimal("PremiumPercentage")
         .NightDifferentialFlag = row.ToBoolean("NightDifferentialFlag")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsDayType(dayType As DbsDayType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsDayType", dayType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDayType(dayType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsDayType(dayType As DbsDayType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DayTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsDayType", dayType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsDayType(dayType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(dayType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsDayType(dayType As DbsDayType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@DayTypeId", dayType.DayTypeId)
         .AddWithValue("@DayTypeCode", dayType.DayTypeCode)
         .AddWithValue("@DayTypeName", dayType.DayTypeName)
         .AddWithValue("@PremiumPercentage", dayType.PremiumPercentage)
         .AddWithValue("@NightDifferentialFlag", dayType.NightDifferentialFlag)
         .AddWithValue("@SortSeq", dayType.SortSeq)
      End With

   End Sub

   Private Sub DeleteDbsDayType(dayTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("DayTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsDayType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@DayTypeId", dayTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsDayTypeBody
   Inherits DbsDayType

End Class
