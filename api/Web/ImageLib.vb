Public NotInheritable Class ImageLib

   Private Sub New()
      MyBase.New()
   End Sub

   Public Shared Function IsHeic(fileName As String) As Boolean

      With ImageLib.GetFileInfo(fileName)
         Return .Format = MagickFormat.Heic
      End With

   End Function

   Public Shared Function IsJpg(fileName As String) As Boolean

      With ImageLib.GetFileInfo(fileName)
         Return .Format = MagickFormat.Jpg OrElse .Format = MagickFormat.Jpeg
      End With

   End Function

   Public Shared Function IsPng(fileName As String) As Boolean
      Return ImageLib.GetFileInfo(fileName).Format = MagickFormat.Png
   End Function

   Public Shared Function IsPdf(fileName As String) As Boolean

      With ImageLib.GetFileInfo(fileName)
         Return .Format = MagickFormat.Pdf OrElse .Format = MagickFormat.Pdfa
      End With

   End Function

   Public Shared Function IsTiff(fileName As String) As Boolean

      With ImageLib.GetFileInfo(fileName)
         Return .Format = MagickFormat.Tif OrElse .Format = MagickFormat.Tiff OrElse .Format = MagickFormat.Tiff64
      End With

   End Function

   Public Shared Function IsWebP(fileName As String) As Boolean

      With ImageLib.GetFileInfo(fileName)
         Return .Format = MagickFormat.WebP
      End With

   End Function

   Public Shared Function Convert(sourceFileName As String, targetFormat As MagickFormat, targetFileName As String, quality As Integer, autoOrient As Boolean) As Boolean

      If String.IsNullOrEmpty(sourceFileName) Then
         Throw New ArgumentNullException("sourceFileName")
         Return False
      End If

      If String.IsNullOrEmpty(targetFileName) Then
         Throw New ArgumentNullException("targetFileName")
         Return False
      End If

      If sourceFileName.Equals(targetFileName) Then
         Return False
      End If

      Try

         Dim _quality As Integer = Math.Min(92, quality)

         Using _image As New MagickImage(sourceFileName)

            _image.Quality = _quality

            If autoOrient Then
               _image.AutoOrient()
            End If

            If _image.Width > 3000 OrElse _image.Height > 3000 Then
               _image.Resize(New Percentage(80))
            End If

            Select Case _image.Format
               Case targetFormat
                  ' same format, no conversion needed
                  ' just copy to target filename

                  _image.Write(targetFileName)

               Case Else
                  _image.Format = targetFormat
                  _image.Write(targetFileName)

            End Select

         End Using

      Catch _exception As Exception
         Return False
      End Try

      Return True

   End Function

   Public Shared Function GetFileInfo(fileName As String) As MagickImageInfo
      Return New MagickImageInfo(fileName)
   End Function

   Public Shared Function CreateThumbnail(info As FileInfo, width As Integer, height As Integer) As MemoryStream
      Return ImageLib.CreateThumbnail(info.FullName, width, height, True)
   End Function

   Public Shared Function CreateThumbnail(info As FileInfo, width As Integer, height As Integer, autoOrient As Boolean) As MemoryStream
      Return ImageLib.CreateThumbnail(info.FullName, width, height, autoOrient)
   End Function

   Public Shared Function CreateThumbnail(fileName As String, width As Integer, height As Integer) As MemoryStream
      Return ImageLib.CreateThumbnail(fileName, width, height, True)
   End Function

   Public Shared Function CreateThumbnail(fileName As String, width As Integer, height As Integer, autoOrient As Boolean) As MemoryStream

      Dim _stream As New MemoryStream()
      _stream.Position = 0

      Using _image As New MagickImage(fileName)
         With _image

            If autoOrient Then
               .AutoOrient()
            End If

            If .Width > width AndAlso .Height > height Then
               .Thumbnail(New MagickGeometry(width, height))
            End If

            .Write(_stream)

         End With
      End Using

      _stream.Position = 0

      Return _stream

   End Function

End Class
