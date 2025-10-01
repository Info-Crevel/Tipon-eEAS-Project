<RoutePrefix("api")>
Public Class DBS0430_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetRevenueQualifications")>
   <Route("revenue-qualifications")>
   <HttpGet>
   Public Function GetRevenueQualifications() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0430_All")
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

   <SymAuthorization("GetRevenueQualification")>
   <Route("revenue-qualifications/{revenueQualificationId}")>
   <HttpGet>
   Public Function GetRevenueQualification(revenueQualificationId As Integer) As IHttpActionResult

      If revenueQualificationId <= 0 Then
         Throw New ArgumentException("Revenue Qualification ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0430")
            With _direct
               .AddParameter("RevenueQualificationId", revenueQualificationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateRevenueQualification")>
   <Route("revenue-qualifications/{currentUserId}")>
   <HttpPost>
   Public Function CreateRevenueQualification(currentUserId As Integer, <FromBody> revenueQualification As DbsRevenueQualificationBody) As IHttpActionResult

      If revenueQualification.RevenueQualificationId <> -1 Then
         Throw New ArgumentException("Revenue Qualification ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _revenueQualificationId As Integer = SysLib.GetNextSequence("RevenueQualificationId")

         revenueQualification.RevenueQualificationId = _revenueQualificationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsRevenueQualification As New DbsRevenueQualification

         Me.LoadDbsRevenueQualification(revenueQualification, _dbsRevenueQualification)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsRevenueQualification(_dbsRevenueQualification)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRevenueQualification(_revenueQualificationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyRevenueQualification")>
   <Route("revenue-qualifications/{revenueQualificationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyRevenueQualification(revenueQualificationId As Integer, currentUserId As Integer, <FromBody> revenueQualification As DbsRevenueQualificationBody) As IHttpActionResult

      If revenueQualificationId <= 0 Then
         Throw New ArgumentException("Revenue Qualification ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsRevenueQualification As New DbsRevenueQualification

         Me.LoadDbsRevenueQualification(revenueQualification, _dbsRevenueQualification)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsRevenueQualification(_dbsRevenueQualification)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetRevenueQualification(revenueQualificationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveRevenueQualification")>
   <Route("revenue-qualifications/{revenueQualificationId}")>
   <HttpDelete>
   Public Function RemoveRevenueQualification(revenueQualificationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If revenueQualificationId <= 0 Then
         Throw New ArgumentException("Revenue Qualification ID is required.")
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

            Me.DeleteDbsRevenueQualification(revenueQualificationId, q.LockId)

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

   Private Sub LoadDbsRevenueQualification(revenueQualification As DbsRevenueQualificationBody, dbsRevenueQualification As DbsRevenueQualification)

      DataLib.ScatterValues(revenueQualification, dbsRevenueQualification)

   End Sub

   Private Sub LoadDbsRevenueQualification(row As DataRow, dbsRevenueQualification As DbsRevenueQualification)

      With dbsRevenueQualification
         .RevenueQualificationId = row.ToInt32("RevenueQualificationId")
         .RevenueQualificationName = row.ToString("RevenueQualificationName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsRevenueQualification(revenueQualification As DbsRevenueQualification)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsRevenueQualification", revenueQualification, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRevenueQualification(revenueQualification)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsRevenueQualification(revenueQualification As DbsRevenueQualification)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RevenueQualificationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsRevenueQualification", revenueQualification, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsRevenueQualification(revenueQualification)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(revenueQualification.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsRevenueQualification(revenueQualification As DbsRevenueQualification)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@RevenueQualificationId", revenueQualification.RevenueQualificationId)
         .AddWithValue("@RevenueQualificationName", revenueQualification.RevenueQualificationName)
         .AddWithValue("@SortSeq", revenueQualification.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsRevenueQualification(revenueQualificationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("RevenueQualificationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsRevenueQualification", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@RevenueQualificationId", revenueQualificationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsRevenueQualificationBody
   Inherits DbsRevenueQualification

End Class
