<RoutePrefix("api")>
Public Class DBS0440_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetNonRevenueQualifications")>
   <Route("non-revenue-qualifications")>
   <HttpGet>
   Public Function GetNonRevenueQualifications() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0440_All")
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

   <SymAuthorization("GetNonRevenueQualification")>
   <Route("non-revenue-qualifications/{nonRevenueQualificationId}")>
   <HttpGet>
   Public Function GetNonRevenueQualification(nonRevenueQualificationId As Integer) As IHttpActionResult

      If nonRevenueQualificationId <= 0 Then
         Throw New ArgumentException("Non-Revenue Qualification ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0440")
            With _direct
               .AddParameter("NonRevenueQualificationId", nonRevenueQualificationId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateNonRevenueQualification")>
   <Route("non-revenue-qualifications/{currentUserId}")>
   <HttpPost>
   Public Function CreateNonRevenueQualification(currentUserId As Integer, <FromBody> nonRevenueQualification As DbsNonRevenueQualificationBody) As IHttpActionResult

      If nonRevenueQualification.NonRevenueQualificationId <> -1 Then
         Throw New ArgumentException("Non-Revenue Qualification ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _nonRevenueQualificationId As Integer = SysLib.GetNextSequence("NonRevenueQualificationId")

         nonRevenueQualification.NonRevenueQualificationId = _nonRevenueQualificationId

         '
         ' Load proposed values from payload
         '
         Dim _dbsNonRevenueQualification As New DbsNonRevenueQualification

         Me.LoadDbsNonRevenueQualification(nonRevenueQualification, _dbsNonRevenueQualification)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsNonRevenueQualification(_dbsNonRevenueQualification)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetNonRevenueQualification(_nonRevenueQualificationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyNonRevenueQualification")>
   <Route("non-revenue-qualifications/{nonRevenueQualificationId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyNonRevenueQualification(nonRevenueQualificationId As Integer, currentUserId As Integer, <FromBody> nonRevenueQualification As DbsNonRevenueQualificationBody) As IHttpActionResult

      If nonRevenueQualificationId <= 0 Then
         Throw New ArgumentException("Non-Revenue Qualification ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsNonRevenueQualification As New DbsNonRevenueQualification

         Me.LoadDbsNonRevenueQualification(nonRevenueQualification, _dbsNonRevenueQualification)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsNonRevenueQualification(_dbsNonRevenueQualification)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetNonRevenueQualification(nonRevenueQualificationId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveNonRevenueQualification")>
   <Route("non-revenue-qualifications/{nonRevenueQualificationId}")>
   <HttpDelete>
   Public Function RemoveNonRevenueQualification(nonRevenueQualificationId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If nonRevenueQualificationId <= 0 Then
         Throw New ArgumentException("Non-Revenue Qualification ID is required.")
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

            Me.DeleteDbsNonRevenueQualification(nonRevenueQualificationId, q.LockId)

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

   Private Sub LoadDbsNonRevenueQualification(nonRevenueQualification As DbsNonRevenueQualificationBody, dbsNonRevenueQualification As DbsNonRevenueQualification)

      DataLib.ScatterValues(nonRevenueQualification, dbsNonRevenueQualification)

   End Sub

   Private Sub LoadDbsNonRevenueQualification(row As DataRow, dbsNonRevenueQualification As DbsNonRevenueQualification)

      With dbsNonRevenueQualification
         .NonRevenueQualificationId = row.ToInt32("NonRevenueQualificationId")
         .NonRevenueQualificationName = row.ToString("NonRevenueQualificationName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsNonRevenueQualification(nonRevenueQualification As DbsNonRevenueQualification)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsNonRevenueQualification", nonRevenueQualification, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsNonRevenueQualification(nonRevenueQualification)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsNonRevenueQualification(nonRevenueQualification As DbsNonRevenueQualification)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("NonRevenueQualificationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsNonRevenueQualification", nonRevenueQualification, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsNonRevenueQualification(nonRevenueQualification)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(nonRevenueQualification.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsNonRevenueQualification(nonRevenueQualification As DbsNonRevenueQualification)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@NonRevenueQualificationId", nonRevenueQualification.NonRevenueQualificationId)
         .AddWithValue("@NonRevenueQualificationName", nonRevenueQualification.NonRevenueQualificationName)
         .AddWithValue("@SortSeq", nonRevenueQualification.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsNonRevenueQualification(nonRevenueQualificationId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("NonRevenueQualificationId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsNonRevenueQualification", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@NonRevenueQualificationId", nonRevenueQualificationId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsNonRevenueQualificationBody
   Inherits DbsNonRevenueQualification

End Class
