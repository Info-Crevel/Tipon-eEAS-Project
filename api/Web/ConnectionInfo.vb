Public Class ConnectionInfo
   Friend Sub New(provider As DataProvider, connectionString As String)
      MyBase.New

      Me.Provider = provider
      Me.ConnectionString = connectionString
      Me.ProviderFactory = Me.GetProviderFactory

   End Sub

   Friend ReadOnly Property Provider As Sys.DataProvider
   'Friend ReadOnly Property ConnectionString As String
   Public ReadOnly Property ConnectionString As String
   Friend ReadOnly Property ProviderFactory As DbProviderFactory

   Friend Function CreateConnection() As DbConnection

      Dim _connection As DbConnection

      _connection = ProviderFactory.CreateConnection()
      _connection.ConnectionString = ConnectionString

      Return _connection

   End Function

   Private Function GetProviderFactory() As DbProviderFactory

      Select Case Me.Provider
         Case DataProvider.Sql
            Return DbProviderFactories.GetFactory("System.Data.SqlClient")

         Case DataProvider.Odbc
            Return DbProviderFactories.GetFactory("System.Data.Odbc")

         Case Else
            Return DbProviderFactories.GetFactory("System.Data.OleDb")

      End Select

   End Function

End Class
