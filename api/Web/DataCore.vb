Public NotInheritable Class DataCore

   Private Shared ReadOnly _connectionString As String = DataLib.GetConnectionInfo("sys").ConnectionString
   Public Shared ReadOnly Property Command As New SqlCommand
   Public Shared ReadOnly Property Connection As New SqlConnection(_connectionString)

   Public Shared Function Connect(databaseId As String) As Boolean

      Connection.ConnectionString = DataLib.GetConnectionInfo(databaseId).ConnectionString

      Try
         If Connection.State <> ConnectionState.Open Then
            Connection.Open()
         End If
      Catch When True
         Return False
      End Try

      Return Connection.State = ConnectionState.Open

   End Function

   Public Shared Sub Disconnect()

      'With Connection
      '   If .State = ConnectionState.Open Then
      '      .Close()
      '   End If
      'End With

      With Connection
         If .State <> ConnectionState.Closed Then
            SqlConnection.ClearPool(Connection)
            .Close()
         End If
      End With

   End Sub

End Class

