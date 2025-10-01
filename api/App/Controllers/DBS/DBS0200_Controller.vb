<RoutePrefix("api")>
Public Class DBS0200_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetStatutoryBenefits")>
   <Route("statutory-benefits")>
   <HttpGet>
   Public Function GetStatutoryBenefits() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0200_All")
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

   <SymAuthorization("GetStatutoryBenefit")>
   <Route("statutory-benefits/{statutoryBenefitId}")>
   <HttpGet>
   Public Function GetStatutoryBenefit(statutoryBenefitId As Integer) As IHttpActionResult

      If statutoryBenefitId <= 0 Then
         Throw New ArgumentException("Statutory Benefit ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0200")
            With _direct
               .AddParameter("StatutoryBenefitId", statutoryBenefitId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateStatutoryBenefit")>
   <Route("statutory-benefits/{currentUserId}")>
   <HttpPost>
   Public Function CreateStatutoryBenefit(currentUserId As Integer, <FromBody> statutoryBenefit As DbsStatutoryBenefitBody) As IHttpActionResult

      If statutoryBenefit.StatutoryBenefitId <> -1 Then
         Throw New ArgumentException("Statutory Benefit ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _StatutoryBenefitId As Integer = SysLib.GetNextSequence("StatutoryBenefitId")

         statutoryBenefit.StatutoryBenefitId = _StatutoryBenefitId

         '
         ' Load proposed values from payload
         '
         Dim _dbsStatutoryBenefit As New DbsStatutoryBenefit

         Me.LoadDbsStatutoryBenefit(statutoryBenefit, _dbsStatutoryBenefit)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsStatutoryBenefit(_dbsStatutoryBenefit)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetStatutoryBenefit(_StatutoryBenefitId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(StatutoryBenefit.StatutoryBenefitId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyStatutoryBenefit")>
   <Route("statutory-benefits/{statutoryBenefitId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyStatutoryBenefit(statutoryBenefitId As Integer, currentUserId As Integer, <FromBody> statutoryBenefit As DbsStatutoryBenefitBody) As IHttpActionResult

      If statutoryBenefitId <= 0 Then
         Throw New ArgumentException("Statutory Benefit ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsStatutoryBenefit As New DbsStatutoryBenefit

         Me.LoadDbsStatutoryBenefit(statutoryBenefit, _dbsStatutoryBenefit)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsStatutoryBenefit(_dbsStatutoryBenefit)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetStatutoryBenefit(statutoryBenefitId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveStatutoryBenefit")>
   <Route("statutory-benefits/{statutoryBenefitId}")>
   <HttpDelete>
   Public Function RemoveStatutoryBenefit(statutoryBenefitId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If statutoryBenefitId <= 0 Then
         Throw New ArgumentException("Statutory Benefit ID is required.")
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

            Me.DeleteDbsStatutoryBenefit(statutoryBenefitId, q.LockId)

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

   Private Sub LoadDbsStatutoryBenefit(statutoryBenefit As DbsStatutoryBenefitBody, dbsStatutoryBenefit As DbsStatutoryBenefit)

      DataLib.ScatterValues(statutoryBenefit, dbsStatutoryBenefit)

   End Sub

   Private Sub LoadDbsStatutoryBenefit(row As DataRow, dbsStatutoryBenefit As DbsStatutoryBenefit)

      With dbsStatutoryBenefit
         .StatutoryBenefitId = row.ToInt32("StatutoryBenefitId")
         .StatutoryBenefitName = row.ToString("StatutoryBenefitName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsStatutoryBenefit(statutoryBenefit As DbsStatutoryBenefit)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsStatutoryBenefit", statutoryBenefit, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsStatutoryBenefit(statutoryBenefit)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsStatutoryBenefit(statutoryBenefit As DbsStatutoryBenefit)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("StatutoryBenefitId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsStatutoryBenefit", statutoryBenefit, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsStatutoryBenefit(statutoryBenefit)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(statutoryBenefit.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsStatutoryBenefit(statutoryBenefit As DbsStatutoryBenefit)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@StatutoryBenefitId", statutoryBenefit.StatutoryBenefitId)
         .AddWithValue("@StatutoryBenefitName", statutoryBenefit.StatutoryBenefitName)
         .AddWithValue("@SortSeq", statutoryBenefit.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsStatutoryBenefit(statutoryBenefitId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("StatutoryBenefitId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsStatutoryBenefit", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@StatutoryBenefitId", statutoryBenefitId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsStatutoryBenefitBody
   Inherits DbsStatutoryBenefit

End Class
