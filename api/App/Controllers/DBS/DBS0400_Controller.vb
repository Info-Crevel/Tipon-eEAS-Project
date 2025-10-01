<RoutePrefix("api")>
Public Class DBS0400_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetCdaMemberTypes")>
   <Route("cda-member-types")>
   <HttpGet>
   Public Function GetCdaMemberTypes() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.DBS0400_All")
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

   <SymAuthorization("GetCdaMemberType")>
   <Route("cda-member-types/{cdaMemberTypeId}")>
   <HttpGet>
   Public Function GetCdaMemberType(cdaMemberTypeId As Integer) As IHttpActionResult

      If cdaMemberTypeId <= 0 Then
         Throw New ArgumentException("CDA Member Type ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.DBS0400")
            With _direct
               .AddParameter("CdaMemberTypeId", cdaMemberTypeId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("CreateCdaMemberType")>
   <Route("cda-member-types/{currentUserId}")>
   <HttpPost>
   Public Function CreateCdaMemberType(currentUserId As Integer, <FromBody> cdaMemberType As DbsCdaMemberTypeBody) As IHttpActionResult

      If cdaMemberType.CDAMemberTypeId <> -1 Then
         Throw New ArgumentException("CDA Member Type ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _cdaMemberTypeId As Integer = SysLib.GetNextSequence("CdaMemberTypeId")

         cdaMemberType.CDAMemberTypeId = _cdaMemberTypeId

         '
         ' Load proposed values from payload
         '
         Dim _dbsCdaMemberType As New DbsCDAMemberType

         Me.LoadDbsCdaMemberType(cdaMemberType, _dbsCdaMemberType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertDbsCdaMemberType(_dbsCdaMemberType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCdaMemberType(_cdaMemberTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         'Return Me.Ok(cdaMemberType.CdaMemberTypeId)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyCdaMemberType")>
   <Route("cda-member-types/{cdaMemberTypeId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyCdaMemberType(cdaMemberTypeId As Integer, currentUserId As Integer, <FromBody> cdaMemberType As DbsCdaMemberTypeBody) As IHttpActionResult

      If cdaMemberTypeId <= 0 Then
         Throw New ArgumentException("CDA Member Type ID is required.")
      End If

      Try
         '
         ' Load proposed values from payload
         '
         Dim _dbsCdaMemberType As New DbsCDAMemberType

         Me.LoadDbsCdaMemberType(cdaMemberType, _dbsCdaMemberType)

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.UpdateDbsCdaMemberType(_dbsCdaMemberType)

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

         Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetCdaMemberType(cdaMemberTypeId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(True)
         Return Me.Ok(_result.Content)

      Catch _exception As Exception
         'File.WriteAllText("d:\zzz.txt", _exception.Message)
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("RemoveCdaMemberType")>
   <Route("cda-member-types/{cdaMemberTypeId}")>
   <HttpDelete>
   Public Function RemoveCdaMemberType(cdaMemberTypeId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

      If cdaMemberTypeId <= 0 Then
         Throw New ArgumentException("CDA Member Type ID is required.")
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

            Me.DeleteDbsCdaMemberType(cdaMemberTypeId, q.LockId)

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

   Private Sub LoadDbsCdaMemberType(cdaMemberType As DbsCdaMemberTypeBody, dbsCdaMemberType As DbsCDAMemberType)

      DataLib.ScatterValues(cdaMemberType, dbsCdaMemberType)

   End Sub

   Private Sub LoadDbsCdaMemberType(row As DataRow, dbsCdaMemberType As DbsCDAMemberType)

      With dbsCdaMemberType
         .CDAMemberTypeId = row.ToInt32("CdaMemberTypeId")
         .CDAMemberTypeName = row.ToString("CdaMemberTypeName")
         .CDAMemberTypeAmount = row.ToDecimal("CDAMemberTypeAmount")
         .SortSeq = row.ToInt32("SortSeq")
      End With

   End Sub

   Private Sub InsertDbsCdaMemberType(cdaMemberType As DbsCDAMemberType)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.DbsCdaMemberType", cdaMemberType, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCdaMemberType(cdaMemberType)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub UpdateDbsCdaMemberType(cdaMemberType As DbsCDAMemberType)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CdaMemberTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.DbsCdaMemberType", cdaMemberType, _keyFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsDbsCdaMemberType(cdaMemberType)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(cdaMemberType.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsDbsCdaMemberType(cdaMemberType As DbsCDAMemberType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@CdaMemberTypeId", cdaMemberType.CDAMemberTypeId)
         .AddWithValue("@CdaMemberTypeName", cdaMemberType.CDAMemberTypeName)
         .AddWithValue("@CDAMemberTypeAmount", cdaMemberType.CDAMemberTypeAmount)

         .AddWithValue("@SortSeq", cdaMemberType.SortSeq)

      End With

   End Sub

   Private Sub DeleteDbsCdaMemberType(cdaMemberTypeId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("CdaMemberTypeId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.DbsCdaMemberType", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@CdaMemberTypeId", cdaMemberTypeId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub

End Class

Public Class DbsCdaMemberTypeBody
   Inherits DbsCDAMemberType

End Class
