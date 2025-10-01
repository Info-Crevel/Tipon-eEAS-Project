Public Structure CurrencyWord

   Private _cardinalSingle As String
   Private _cardinalPlural As String
   Private _fractionSingle As String
   Private _fractionPlural As String

   Friend Sub New(cardinalSingle As String, cardinalPlural As String, fractionSingle As String, fractionPlural As String)
      _cardinalSingle = cardinalSingle
      _cardinalPlural = cardinalPlural
      _fractionSingle = fractionSingle
      _fractionPlural = fractionPlural
   End Sub

   Public ReadOnly Property CardinalSingle As String
      Get
         Return _cardinalSingle
      End Get
   End Property

   Public ReadOnly Property CardinalPlural As String
      Get
         Return _cardinalPlural
      End Get
   End Property

   Public ReadOnly Property FractionSingle As String
      Get
         Return _fractionSingle
      End Get
   End Property

   Public ReadOnly Property FractionPlural As String
      Get
         Return _fractionPlural
      End Get
   End Property

End Structure

Public Class CurrencyWordCollection
   Inherits Dictionary(Of String, CurrencyWord)       ' CultureName, CurrencyWord

End Class
