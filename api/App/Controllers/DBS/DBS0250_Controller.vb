<RoutePrefix("api")>
Public Class DBS0250_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetInsuranceCoverages")>
   <Route("insurance-coverages")>
   <HttpGet>
   Public Function GetInsuranceCoverages() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0250_All")
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

   <SymAuthorization("GetInsuranceCoverage")>
   <Route("insurance-coverages/{insuranceCoverageId}")>
   <HttpGet>
   Public Function GetInsuranceCoverage(insuranceCoverageId As Integer) As IHttpActionResult

      If insuranceCoverageId <= 0 Then
         Throw New ArgumentException("Insurance Coverage ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0250")
            With _direct
               .AddParameter("InsuranceCoverageId", insuranceCoverageId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateInsuranceCoverage")>
   <Route("insurance-coverages/{currentUserId}")>
   <HttpPost>
   Public Function CreateInsuranceCoverage(currentUserId As Integer, <FromBody> insuranceCoverage As DbsInsuranceCoverageBody) As IHttpActionResult

      If insuranceCoverage.InsuranceCoverageId <> -1 Then
         Throw New ArgumentException("Insurance Coverage ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _insuranceCoverageId As Integer = SysLib.GetNextSequence("InsuranceCoverageId")

         insuranceCoverage.InsuranceCoverageId = _insuranceCoverageId

         '
         ' Load proposed values from payload
         '
         Dim _dbsInsuranceCoverage As New DbsInsuranceCoverage

         Me.LoadDbsInsuranceCoverage(insuranceCoverage, _dbsInsuranceCoverage)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsInsuranceCoverage(_dbsInsuranceCoverage)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInsuranceCoverage(_insuranceCoverageId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(insuranceCoverage.InsuranceCoverageId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyInsuranceCoverage")>
   <Route("insurance-coverages/{insuranceCoverageId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyInsuranceCoverage(insuranceCoverageId As Integer, currentUserId As Integer, <FromBody> insuranceCoverage As DbsInsuranceCoverageBody) As IHttpActionResult

      If insuranceCoverageId <= 0 Then
         Throw New ArgumentException("Insurance Coverage ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsInsuranceCoverage As New DbsInsuranceCoverage

         Me.LoadDbsInsuranceCoverage(insuranceCoverage, _dbsInsuranceCoverage)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsInsuranceCoverage(_dbsInsuranceCoverage)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInsuranceCoverage(insuranceCoverageId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveInsuranceCoverage")>
   <Route("insurance-coverages/{insuranceCoverageId}")>
   <HttpDelete>
   Public Function RemoveInsuranceCoverage(insuranceCoverageId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If insuranceCoverageId <= 0 Then
         Throw New ArgumentException("Insurance Coverage ID is required.")
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

            Me.DeleteDbsInsuranceCoverage(insuranceCoverageId, q.LockId)

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

   Private Sub LoadDbsInsuranceCoverage(insuranceCoverage As DbsInsuranceCoverageBody, dbsInsuranceCoverage As DbsInsuranceCoverage)

      DataLib.ScatterValues(insuranceCoverage, dbsInsuranceCoverage)

   End Sub

   Private Sub LoadDbsInsuranceCoverage(row As DataRow, dbsInsuranceCoverage As DbsInsuranceCoverage)

      With dbsInsuranceCoverage
         .InsuranceCoverageId = row.ToInt32("InsuranceCoverageId")
         .InsuranceCoverageName = row.ToString("InsuranceCoverageName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsInsuranceCoverage(insuranceCoverage As DbsInsuranceCoverage)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsInsuranceCoverage", insuranceCoverage, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInsuranceCoverage(insuranceCoverage)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsInsuranceCoverage(insuranceCoverage As DbsInsuranceCoverage)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InsuranceCoverageId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsInsuranceCoverage", insuranceCoverage, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInsuranceCoverage(insuranceCoverage)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(insuranceCoverage.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsInsuranceCoverage(insuranceCoverage As DbsInsuranceCoverage)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@InsuranceCoverageId", insuranceCoverage.InsuranceCoverageId)
         .AddWithValue("@InsuranceCoverageName", insuranceCoverage.InsuranceCoverageName)
         .AddWithValue("@SortSeq", insuranceCoverage.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsInsuranceCoverage(insuranceCoverageId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InsuranceCoverageId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsInsuranceCoverage", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@InsuranceCoverageId", insuranceCoverageId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsInsuranceCoverageBody
   Inherits DbsInsuranceCoverage

End Class
