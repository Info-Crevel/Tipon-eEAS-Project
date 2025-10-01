<RoutePrefix("api")>
Public Class DBS0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetIndustries")>
   <Route("industries")>
   <HttpGet>
   Public Function GetIndustries() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0030_All")
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

   <SymAuthorization("GetIndustry")>
   <Route("industries/{industryId}")>
   <HttpGet>
   Public Function GetIndustry(industryId As Integer) As IHttpActionResult

      If industryId <= 0 Then
         Throw New ArgumentException("Industry ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0030")
            With _direct
               .AddParameter("IndustryId", industryId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateIndustry")>
   <Route("industries/{currentUserId}")>
   <HttpPost>
   Public Function CreateIndustry(currentUserId As Integer, <FromBody> industry As DbsIndustryBody) As IHttpActionResult

      If industry.IndustryId <> -1 Then
         Throw New ArgumentException("Industry ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _industryId As Integer = SysLib.GetNextSequence("IndustryId")

         industry.IndustryId = _industryId

         '
         ' Load proposed values from payload
         '
         Dim _dbsIndustry As New DbsIndustry

         Me.LoadDbsIndustry(industry, _dbsIndustry)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsIndustry(_dbsIndustry)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetIndustry(_industryId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyIndustry")>
   <Route("industries/{industryId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyIndustry(industryId As Integer, currentUserId As Integer, <FromBody> industry As DbsIndustryBody) As IHttpActionResult

      If industryId <= 0 Then
         Throw New ArgumentException("Industry ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsIndustry As New DbsIndustry

         Me.LoadDbsIndustry(industry, _dbsIndustry)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsIndustry(_dbsIndustry)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetIndustry(industryId), Results.OkNegotiatedContentResult(Of DataTable))
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveIndustry")>
   <Route("industries/{industryId}")>
   <HttpDelete>
   Public Function RemoveIndustry(industryId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If industryId <= 0 Then
         Throw New ArgumentException("Industry ID is required.")
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

            Me.DeleteDbsIndustry(industryId, q.LockId)

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

   Private Sub LoadDbsIndustry(industry As DbsIndustryBody, dbsIndustry As DbsIndustry)

      DataLib.ScatterValues(industry, dbsIndustry)

   End Sub

   Private Sub LoadDbsIndustry(row As DataRow, dbsIndustry As DbsIndustry)

      With dbsIndustry
         .IndustryId = row.ToInt32("IndustryId")
         .IndustryName = row.ToString("IndustryName")
      End With

   End Sub

   Private Sub InsertDbsIndustry(industry As DbsIndustry)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsIndustry", industry, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsIndustry(industry)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsIndustry(industry As DbsIndustry)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("IndustryId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsIndustry", industry, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsIndustry(industry)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(industry.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsIndustry(industry As DbsIndustry)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@IndustryId", industry.IndustryId)
         .AddWithValue("@IndustryName", industry.IndustryName)

      End With

   End Sub

   Private Sub DeleteDbsIndustry(industryId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("IndustryId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsIndustry", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@IndustryId", industryId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsIndustryBody
   Inherits DbsIndustry

End Class
