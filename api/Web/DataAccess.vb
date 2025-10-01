Friend MustInherit Class DataAccess
   Implements IDisposable

#Region " Variables "

   Private _connection As DbConnection
   Private _isDisposed As Boolean

#End Region

#Region " Constructor "

   Friend Sub New(databaseId As String)
      MyBase.New()

      With DataLib.GetConnectionInfo(databaseId)
         Me.Provider = .Provider
         Me.ProviderFactory = .ProviderFactory
         _connection = .CreateConnection()
      End With

   End Sub

#End Region

#Region " Properties "

   Friend ReadOnly Property Provider As DataProvider

   Friend ReadOnly Property ProviderFactory As DbProviderFactory

#End Region

#Region " Methods "

   Protected Sub OpenConnection()

      Select Case _connection.State
         Case ConnectionState.Open
            '
            ' Do nothing
            '
         Case Else

            Try
               _connection.Open()
            Catch When Not _connection.State.Equals(ConnectionState.Open)
               Throw New Exception("Cannot open database connection. (" + _connection.DataSource + ")")
            End Try

      End Select

   End Sub

   Protected Sub CloseConnection()

      If Not _connection.State.Equals(ConnectionState.Closed) Then
         _connection.Close()
      End If

   End Sub

   Protected Function CreateCommand(commandType As CommandType, commandText As String) As DbCommand

      Dim _command As DbCommand = ProviderFactory.CreateCommand()

      With _command
         .CommandText = commandText
         .CommandType = commandType
         .Connection = _connection
      End With

      Return _command

   End Function

#End Region

#Region " IDisposable "

   Protected Overridable Sub Dispose(disposing As Boolean)
      If Not _isDisposed Then
         If disposing Then
            If _connection IsNot Nothing Then
               _connection.Close()
               _connection.Dispose()
               _connection = Nothing
            End If
         End If

      End If
      _isDisposed = True
   End Sub

   Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Me.Dispose(True)
      GC.SuppressFinalize(Me)
   End Sub

#End Region

End Class
