<RoutePrefix("api")>
Public Class DBS0190_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetInHouseBenefits")>
   <Route("in-house-benefits")>
   <HttpGet>
   Public Function GetInHouseBenefits() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0190_All")
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

   <SymAuthorization("GetInHouseBenefit")>
   <Route("in-house-benefits/{inHouseBenefitId}")>
   <HttpGet>
   Public Function GetInHouseBenefit(inHouseBenefitId As Integer) As IHttpActionResult

      If inHouseBenefitId <= 0 Then
         Throw New ArgumentException("In-House Benefit ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0190")
            With _direct
               .AddParameter("InHouseBenefitId", inHouseBenefitId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateInHouseBenefit")>
   <Route("in-house-benefits/{currentUserId}")>
   <HttpPost>
   Public Function CreateInHouseBenefit(currentUserId As Integer, <FromBody> inHouseBenefit As DbsInHouseBenefitBody) As IHttpActionResult

      If inHouseBenefit.InHouseBenefitId <> -1 Then
         Throw New ArgumentException("In-House Benefit ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _inHouseBenefitId As Integer = SysLib.GetNextSequence("InHouseBenefitId")

         inHouseBenefit.InHouseBenefitId = _inHouseBenefitId

         '
         ' Load proposed values from payload
         '
         Dim _dbsInHouseBenefit As New DbsInHouseBenefit

         Me.LoadDbsInHouseBenefit(inHouseBenefit, _dbsInHouseBenefit)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsInHouseBenefit(_dbsInHouseBenefit)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInHouseBenefit(_inHouseBenefitId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(InHouseBenefit.InHouseBenefitId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyInHouseBenefit")>
   <Route("in-house-benefits/{inHouseBenefitId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyInHouseBenefit(inHouseBenefitId As Integer, currentUserId As Integer, <FromBody> inHouseBenefit As DbsInHouseBenefitBody) As IHttpActionResult

      If inHouseBenefitId <= 0 Then
         Throw New ArgumentException("In-House Benefit ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsInHouseBenefit As New DbsInHouseBenefit

         Me.LoadDbsInHouseBenefit(inHouseBenefit, _dbsInHouseBenefit)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsInHouseBenefit(_dbsInHouseBenefit)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetInHouseBenefit(inHouseBenefitId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveInHouseBenefit")>
   <Route("in-house-benefits/{inHouseBenefitId}")>
   <HttpDelete>
   Public Function RemoveInHouseBenefit(inHouseBenefitId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If inHouseBenefitId <= 0 Then
         Throw New ArgumentException("In-House Benefit ID is required.")
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

            Me.DeleteDbsInHouseBenefit(inHouseBenefitId, q.LockId)

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

   Private Sub LoadDbsInHouseBenefit(inHouseBenefit As DbsInHouseBenefitBody, dbsInHouseBenefit As DbsInHouseBenefit)

      DataLib.ScatterValues(inHouseBenefit, dbsInHouseBenefit)

   End Sub

   Private Sub LoadDbsInHouseBenefit(row As DataRow, dbsInHouseBenefit As DbsInHouseBenefit)

      With dbsInHouseBenefit
         .InHouseBenefitId = row.ToInt32("InHouseBenefitId")
         .InHouseBenefitName = row.ToString("InHouseBenefitName")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsInHouseBenefit(inHouseBenefit As DbsInHouseBenefit)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsInHouseBenefit", inHouseBenefit, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInHouseBenefit(inHouseBenefit)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsInHouseBenefit(inHouseBenefit As DbsInHouseBenefit)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InHouseBenefitId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsInHouseBenefit", inHouseBenefit, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsInHouseBenefit(inHouseBenefit)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(inHouseBenefit.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsInHouseBenefit(inHouseBenefit As DbsInHouseBenefit)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@InHouseBenefitId", inHouseBenefit.InHouseBenefitId)
         .AddWithValue("@InHouseBenefitName", inHouseBenefit.InHouseBenefitName)
         .AddWithValue("@SortSeq", inHouseBenefit.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsInHouseBenefit(inHouseBenefitId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("InHouseBenefitId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsInHouseBenefit", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@InHouseBenefitId", inHouseBenefitId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsInHouseBenefitBody
   Inherits DbsInHouseBenefit

End Class
