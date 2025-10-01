Public NotInheritable Class FrsSession

   Private Shared ReadOnly _glsAccountType As New QGlsAccountType
   Private Shared ReadOnly _glsAccountNature As New QGlsAccountNature
   Private Shared ReadOnly _dbsBudgetClass As New QDbsBudgetClass
   Private Shared ReadOnly _dbsCostCenter As New QDbsCostCenterCore
   Private Shared ReadOnly _dbsFundCluster As New QDbsFundCluster
   Private Shared ReadOnly _dbsJournal As New QDbsJournal
   Private Shared ReadOnly _dbsTrxClass As New QDbsTrxClass
   Private Shared ReadOnly _dbsTrxStatus As New QDbsTrxStatus
   Private Shared ReadOnly _dbsCoxType As New QDbsCoxType
   Private Shared ReadOnly _dbsDbxType As New QDbsDbxType
   Private Shared ReadOnly _dbsObxStatus As New QDbsObxStatus

   Private Shared _currencyWords As New CurrencyWordCollection
   Private Shared _cultureInfo As System.Globalization.CultureInfo = Threading.Thread.CurrentThread.CurrentCulture

   Private Sub New()
      MyBase.New()
   End Sub

   Friend Shared Sub Open()

      Try
         Using _direct As New SqlDirect("web.ZFrsSession")
            Using _dataSet As DataSet = _direct.ExecuteDataSet()
               With _dataSet
                  FrsSession.LoadBackOfficeConfig(_dataSet)
               End With
            End Using
         End Using

         FrsSession.LoadCurrencyWords()

      Catch _exception As Exception
         Throw _exception
      End Try

   End Sub

   Private Shared Sub LoadBackOfficeConfig(dataSet As DataSet)

      Dim _accountType As GlsAccountType
      Dim _accountNature As GlsAccountNature
      Dim _budgetClass As DbsBudgetClass
      Dim _costCenter As DbsCostCenterCore
      Dim _fundCluster As DbsFundCluster
      Dim _journal As DbsJournal
      Dim _trxClass As DbsTrxClass
      Dim _trxStatus As DbsTrxStatus
      Dim _coxType As DbsCoxType
      Dim _dbxType As DbsDbxType
      Dim _obxStatus As DbsObxStatus

      For Each _row As DataRow In dataSet.Tables(0).Rows
         _accountType = New GlsAccountType
         With _accountType
            .AccountTypeId = _row.ToInt32("AccountTypeId")
            .AccountTypeName = _row.ToString("AccountTypeName")
         End With

         _glsAccountType.Rows.Add(_accountType)
      Next

      For Each _row As DataRow In dataSet.Tables(1).Rows
         _accountNature = New GlsAccountNature
         With _accountNature
            .AccountNatureId = _row.ToInt32("AccountNatureId")
            .AccountNatureName = _row.ToString("AccountNatureName")
         End With

         _glsAccountNature.Rows.Add(_accountNature)
      Next

      For Each _row As DataRow In dataSet.Tables(2).Rows
         _budgetClass = New DbsBudgetClass
         With _budgetClass
            .BudgetClassId = _row.ToInt32("BudgetClassId")
            .BudgetClassName = _row.ToString("BudgetClassName")
         End With

         _dbsBudgetClass.Rows.Add(_budgetClass)
      Next

      For Each _row As DataRow In dataSet.Tables(3).Rows
         _costCenter = New DbsCostCenterCore
         With _costCenter
            .CostCenterId = _row.ToInt32("CostCenterId")
            .CostCenterName = _row.ToString("CostCenterName")
            .CostCenterShortName = _row.ToString("CostCenterShortName")
         End With

         _dbsCostCenter.Rows.Add(_costCenter)
      Next

      For Each _row As DataRow In dataSet.Tables(4).Rows
         _fundCluster = New DbsFundCluster
         With _fundCluster
            .FundClusterId = _row.ToString("FundClusterId")
            .FundClusterName = _row.ToString("FundClusterName")
         End With

         _dbsFundCluster.Rows.Add(_fundCluster)
      Next

      For Each _row As DataRow In dataSet.Tables(5).Rows
         _journal = New DbsJournal
         With _journal
            .JournalId = _row.ToInt32("JournalId")
            .JournalName = _row.ToString("JournalName")
            .JournalShortName = _row.ToString("JournalShortName")
         End With

         _dbsJournal.Rows.Add(_journal)
      Next

      For Each _row As DataRow In dataSet.Tables(6).Rows
         _trxClass = New DbsTrxClass
         With _trxClass
            .TrxClassId = _row.ToInt32("TrxClassId")
            .TrxClassName = _row.ToString("TrxClassName")
         End With

         _dbsTrxClass.Rows.Add(_trxClass)
      Next

      For Each _row As DataRow In dataSet.Tables(7).Rows
         _trxStatus = New DbsTrxStatus
         With _trxStatus
            .TrxStatusId = _row.ToInt32("TrxStatusId")
            .TrxStatusName = _row.ToString("TrxStatusName")
         End With

         _dbsTrxStatus.Rows.Add(_trxStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(8).Rows
         _coxType = New DbsCoxType
         With _coxType
            .CoxTypeId = _row.ToInt32("CoxTypeId")
            .CoxTypeName = _row.ToString("CoxTypeName")
         End With

         _dbsCoxType.Rows.Add(_coxType)
      Next

      For Each _row As DataRow In dataSet.Tables(9).Rows
         _dbxType = New DbsDbxType
         With _dbxType
            .DbxTypeId = _row.ToInt32("DbxTypeId")
            .DbxTypeName = _row.ToString("DbxTypeName")
         End With

         _dbsDbxType.Rows.Add(_dbxType)
      Next

      For Each _row As DataRow In dataSet.Tables(10).Rows
         _obxStatus = New DbsObxStatus
         With _obxStatus
            .ObxStatusId = _row.ToInt32("ObxStatusId")
            .ObxStatusName = _row.ToString("ObxStatusName")
         End With

         _dbsObxStatus.Rows.Add(_obxStatus)
      Next

   End Sub

   Friend Shared ReadOnly Property GlsAccountType As QGlsAccountType
      Get
         Return _glsAccountType
      End Get
   End Property

   Friend Shared ReadOnly Property GlsAccountNature As QGlsAccountNature
      Get
         Return _glsAccountNature
      End Get
   End Property

   Friend Shared ReadOnly Property DbsBudgetClass As QDbsBudgetClass
      Get
         Return _dbsBudgetClass
      End Get
   End Property

   Friend Shared ReadOnly Property DbsCostCenter As QDbsCostCenterCore
      Get
         Return _dbsCostCenter
      End Get
   End Property

   Friend Shared ReadOnly Property DbsFundCluster As QDbsFundCluster
      Get
         Return _dbsFundCluster
      End Get
   End Property

   Friend Shared ReadOnly Property DbsJournal As QDbsJournal
      Get
         Return _dbsJournal
      End Get
   End Property

   Friend Shared ReadOnly Property DbsTrxClass As QDbsTrxClass
      Get
         Return _dbsTrxClass
      End Get
   End Property

   Friend Shared ReadOnly Property DbsTrxStatus As QDbsTrxStatus
      Get
         Return _dbsTrxStatus
      End Get
   End Property

   Friend Shared ReadOnly Property DbsCoxType As QDbsCoxType
      Get
         Return _dbsCoxType
      End Get
   End Property

   Friend Shared ReadOnly Property DbsDbxType As QDbsDbxType
      Get
         Return _dbsDbxType
      End Get
   End Property

   Friend Shared ReadOnly Property DbsObxStatus As QDbsObxStatus
      Get
         Return _dbsObxStatus
      End Get
   End Property

   Friend Shared ReadOnly Property CurrencyWords() As CurrencyWordCollection
      Get
         Return _currencyWords
      End Get
   End Property

   Public Shared ReadOnly Property CurrencyDecimalSeparator() As String
      Get
         Return _cultureInfo.NumberFormat.CurrencyDecimalSeparator
      End Get
   End Property

   Private Shared Sub LoadCurrencyWords()

      _currencyWords.Add("en-US", New CurrencyWord("Dollar", "Dollars", "Cent", "Cents"))
      _currencyWords.Add("en-PH", New CurrencyWord("Peso", "Pesos", "Centavo", "Centavos"))

   End Sub

End Class
