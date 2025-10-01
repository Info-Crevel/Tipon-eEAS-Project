Public NotInheritable Class FileSystemLib

   Private Sub New()
      MyBase.New()
   End Sub

   Public Shared Function GetFileNames(folder As String) As System.IO.FileInfo()
      Return FileSystemLib.GetFileNames(folder, "*.*")
   End Function

   Public Shared Function GetFileNames(folder As String, searchPattern As String) As System.IO.FileInfo()
      Return FileSystemLib.GetFileNames(folder, searchPattern, SearchOption.TopDirectoryOnly)
   End Function

   Public Shared Function GetFileNames(folder As String, searchPattern As String, searchOption As SearchOption) As FileInfo()

      If folder Is Nothing Then
         Throw New ArgumentNullException("folder")
      End If

      If searchPattern Is Nothing Then
         Throw New ArgumentNullException("searchPattern")
      End If

      If Not Directory.Exists(folder) Then
         Throw New DirectoryNotFoundException
      End If

      Dim _folderInfo As DirectoryInfo = New DirectoryInfo(folder)

      If searchPattern Is String.Empty Then
         Return _folderInfo.GetFiles("*.*", searchOption)
      Else
         Return _folderInfo.GetFiles(searchPattern, searchOption)
      End If

   End Function

   Public Shared Function GetJsonFile(filePath As String) As Object

      Dim _fileName As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath)
      If Not File.Exists(_fileName) Then
         Throw New FileNotFoundException(filePath)
      End If

      Dim _json As String = File.ReadAllText(_fileName)
      Return JsonConvert.DeserializeObject(_json)

   End Function

   Public Shared Function IsTargetFileLargerThanSourceFile(targetFileName As String, sourceFileName As String) As Boolean
      Return New FileInfo(targetFileName).Length > New FileInfo(sourceFileName).Length
   End Function

   Public Shared Function IsXmlFile(filePath As String) As Boolean
      Return FileSystemLib.IsXmlFile(filePath, False)
   End Function

   Public Shared Function IsXmlFile(filePath As String, wellFormedCheck As Boolean) As Boolean

      ' Check for XML declaration (basic check, reading first few bytes of file)

      Dim _bytes() As Byte = New Byte(18) {}

      Using _stream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None)
         _stream.Read(_bytes, 0, _bytes.Length)
      End Using

      Dim _declaration As String = Encoding.UTF8.GetString(_bytes).ToLowerInvariant

      If _declaration <> "<?xml version=""1.0""" Then
         Return False
      End If

      If wellFormedCheck Then
         Try
            Dim _xmlDoc As New XmlDocument
            _xmlDoc.Load(filePath)
         Catch When True
            Return False
         End Try
      End If

      Return True

   End Function

End Class

Public Class FileUploadResponse
   Public Property CreatedCount As Integer
   Public Property FailedCount As Integer
   Public Property Details As FileUploadDetaiiList

End Class

Public Class FileUploadDetaii
   Public Property FileName As String
   Public Property StatusCode As Integer
   Public Property StatusText As String

End Class

Public Class FileUploadDetaiiList
   Inherits List(Of FileUploadDetaii)

End Class
