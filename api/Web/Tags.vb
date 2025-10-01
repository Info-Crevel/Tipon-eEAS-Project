Public NotInheritable Class Tags

#Region " Constructor "

   Private Sub New()
      MyBase.New()
   End Sub

#End Region

#Region " System Constants "

   Public Const EmptyDate As Date = #12:00:00 AM#           ' DotNet empty date 
   Public Const SystemId As String = "sys"
   Public Const DateFormatIso As String = "yyyyMMdd"
   Public Const DateDisplayFormat As String = "dd MMM yyyy"
   Public Const TimeDisplayFormat As String = "h:mm tt"     ' US format (Default)

   Friend Const SqlDirectCount As String = "SqlDirectCount"
   Friend Const CommandTimeout As Integer = 120

#End Region

#Region " System-wide "

   Public Shared ReadOnly Property Linefeed As Char = Tags.Character(10)
   Public Shared ReadOnly Property CarriageReturn As Char = Tags.Character(13)

   Public Shared ReadOnly Property NewLine As String
      Get
         Return Tags.CarriageReturn + Tags.Linefeed
      End Get
   End Property

   Public Shared ReadOnly Property NewLine2 As String
      Get
         Return Tags.NewLine + Tags.NewLine
      End Get
   End Property

#End Region

#Region " Helpers "

   Private Shared Function Character(charCode As Integer) As Char

      If (charCode < -32768) OrElse (charCode > &HFFFF) Then
         Throw New ArgumentException("Character Code is out Of range.")
      End If

      If (charCode >= 0) AndAlso (charCode <= &H7F) Then
         Return Convert.ToChar(charCode)
      End If

      Dim _count As Integer
      Dim _encoding As Encoding = Encoding.GetEncoding(Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage)
      If _encoding.IsSingleByte AndAlso ((charCode < 0) OrElse (charCode > &HFF)) Then
         Throw New ArgumentOutOfRangeException("charCode")
      End If

      Dim _chars As Char() = New Char(2 - 1) {}
      Dim _bytes As Byte() = New Byte(2 - 1) {}
      Dim _decoder As Decoder = _encoding.GetDecoder

      If (charCode >= 0) AndAlso (charCode <= &HFF) Then
         _bytes(0) = CByte((charCode And &HFF))
         _count = _decoder.GetChars(_bytes, 0, 1, _chars, 0)
      Else
         _bytes(0) = CByte((charCode And &HFF00) >> 8)
         _bytes(1) = CByte((charCode And &HFF))
         _count = _decoder.GetChars(_bytes, 0, 2, _chars, 0)
      End If

      Return _chars(0)

   End Function

#End Region

#Region " ParameterTable Column Alias "

   Friend Const ParameterColumnParameterNameAlias As Char = "P"c
   Friend Const ParameterColumnValueAlias As Char = "V"c

#End Region

End Class
