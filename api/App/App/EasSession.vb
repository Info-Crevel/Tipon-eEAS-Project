Public NotInheritable Class EasSession

   'Private Shared ReadOnly _glsAccountType As New QGlsAccountType
   'Private Shared ReadOnly _glsAccountNature As New QGlsAccountNature
   'Private Shared ReadOnly _dbsBudgetClass As New QDbsBudgetClass
   'Private Shared ReadOnly _dbsCostCenter As New QDbsCostCenterCore
   'Private Shared ReadOnly _dbsFundCluster As New QDbsFundCluster
   'Private Shared ReadOnly _dbsJournal As New QDbsJournal
   'Private Shared ReadOnly _dbsTrxClass As New QDbsTrxClass
   'Private Shared ReadOnly _dbsTrxStatus As New QDbsTrxStatus
   'Private Shared ReadOnly _dbsCoxType As New QDbsCoxType
   'Private Shared ReadOnly _dbsDbxType As New QDbsDbxType
   'Private Shared ReadOnly _dbsObxStatus As New QDbsObxStatus

   Private Shared ReadOnly _hrsMemberType As New QHrsMemberType
   Private Shared ReadOnly _hrsMemberStatus As New QHrsMemberStatus

   Private Shared ReadOnly _dbsSex As New QDbsSex
   Private Shared ReadOnly _dbsBloodType As New QDbsBloodTYpe

   Private Shared ReadOnly _dbsCivilStatus As New QDbsCivilStatus
   Private Shared ReadOnly _dbsReligion As New QDbsReligion

   Private Shared ReadOnly _dbsRegion As New QDbsRegion
   Private Shared ReadOnly _dbsProvince As New QDbsProvince
   Private Shared ReadOnly _dbsMunicipality As New QDbsMunicipality
   Private Shared ReadOnly _dbsBarangay As New QDbsBarangay

   Private Shared ReadOnly _dbsRelation As New QDbsRelation
   Private Shared ReadOnly _dbsEducationLevel As New QDbsEducationLevel
   Private Shared ReadOnly _dbsDisability As New QDbsDisability

   Private Shared ReadOnly _hrsMemberTypeQualification As New QHrsMemberTypeQualification

   Private Shared ReadOnly _dbsCDAMemberType As New QDbsCDAMemberType
   Private Shared ReadOnly _dbsEmploymentStatus As New QDbsEmploymentStatus
   Private Shared ReadOnly _dbsBank As New QDbsBank
   Private Shared ReadOnly _dbsLanguage As New QDbsLanguage
   Private Shared ReadOnly _dbsRnrRecording As New QDbsRnrRecording
   Private Shared ReadOnly _dbsVaccineType As New QDbsVaccineType
   Private Shared ReadOnly _dbsDocType As New QDbsDocType
   Private Shared ReadOnly _dbsLicenseProfession As New QDbsLicenseProfession
   Private Shared ReadOnly _dbsCDATraining As New QDbsCDATraining
   Private Shared ReadOnly _dbsNCIIQualificationTitle As New QDbsNCIIQualificationTitle
   Private Shared ReadOnly _dbsTrainingInstitution As New QDbsTrainingInstitution
   Private Shared ReadOnly _dbsAssessmentCenter As New QDbsAssessmentCenter
   Private Shared ReadOnly _dbsComplianceTraining As New QDbsComplianceTraining
   Private Shared ReadOnly _dbsAffiliation As New QDbsAffiliation
   Private Shared ReadOnly _hrsRecruiter As New QHrsRecruiter
   Private Shared ReadOnly _dbsApplicationSource As New QDbsApplicationSource
   Private Shared ReadOnly _dbsMedicalResultType As New QDbsbsMedicalResultType
   'Private Shared ReadOnly _dbsSkillSetDetail As New QDbsSkillSetDetailLog
   Private Shared ReadOnly _dbsSkillSetDetailLog As New QDbsSkillSetDetailLog
   Private Shared ReadOnly _dbsEmploymentType As New QDbsEmploymentType
   Private Shared ReadOnly _dbsRnrNumber As New QDbsRnrNumber

   Private Shared ReadOnly _hrsApplicantStatus As New QHrsApplicantStatus
   Private Shared ReadOnly _dbsMemberSuffix As New QDbsMemberSuffix
   Private Shared ReadOnly _dbsApplicantScreening As New QDbsApplicantScreening
   Private Shared ReadOnly _dbsScreeningStatus As New QDbsScreeningStatus
   Private Shared ReadOnly _dbsApplicantStatus As New QDbsApplicantStatus

   Private Shared _currencyWords As New CurrencyWordCollection
   Private Shared _cultureInfo As System.Globalization.CultureInfo = Threading.Thread.CurrentThread.CurrentCulture

   Private Sub New()
      MyBase.New()
   End Sub

   Friend Shared Sub Open()

      Try
         Using _direct As New SqlDirect("web.ZEasSession")
            Using _dataSet As DataSet = _direct.ExecuteDataSet()
               With _dataSet
                  EasSession.LoadBackOfficeConfig(_dataSet)
               End With
            End Using
         End Using

         EasSession.LoadCurrencyWords()

      Catch _exception As Exception
         Throw _exception
      End Try

   End Sub

   Private Shared Sub LoadBackOfficeConfig(dataSet As DataSet)

      Dim _memberType As HrsMemberType
      Dim _memberStatus As HrsMemberStatus
      Dim _sex As DbsSex
      Dim _bloodType As DbsBloodType
      Dim _civilStatus As DbsCivilStatus
      Dim _religion As DbsReligion
      Dim _region As DbsRegion
      Dim _province As DbsProvince
      Dim _municipality As DbsMunicipality
      Dim _barangay As DbsBarangay
      Dim _relation As DbsRelation
      Dim _educationLevel As DbsEducationLevel
      Dim _disability As DbsDisability
      Dim _memberTypeQualification As HrsMemberTypeQualification
      Dim _cdaMemberType As DbsCDAMemberType
      Dim _employmentStatus As DbsEmploymentStatus
      Dim _bank As DbsBank
      Dim _vaccineType As DbsVaccineType
      Dim _docType As DbsDocType
      Dim _LicenseProfession As DbsLicenseProfession
      Dim _cDATraining As DbsCDATraining

      Dim _language As DbsLanguage
      Dim _rnrRecording As DbsRnrRecording
      Dim _nCIIQualificationTitle As DbsNCIIQualificationTitle
      Dim _trainingInstitution As DbsTrainingInstitution
      Dim _assessmentCenter As DbsAssessmentCenter
      Dim _complianceTraining As DbsComplianceTraining
      Dim _affiliation As DbsAffiliation
      Dim _recruiter As HrsRecruiter
      Dim _applicationSource As DbsApplicationSource
      Dim _medicalResultType As DbsMedicalResultType
      Dim _skillSet As QDbsSkillSetDetail
      Dim _employmentType As DbsEmploymentType
      Dim _rnrNumber As DbsRnrNumber
      Dim _applicantStatus As HrsApplicantStatus
      Dim _memberSuffix As DbsMemberSuffix
      Dim _applicantScreening As DbsApplicantScreening
      Dim _screeningStatus As DbsScreeningStatus

      Dim _appStatus As DbsApplicantStatus

      For Each _row As DataRow In dataSet.Tables(0).Rows
         _memberType = New HrsMemberType
         With _memberType
            .MemberTypeId = _row.ToInt32("MemberTypeId")
            .MemberTypeName = _row.ToString("MemberTypeName")
         End With

         _hrsMemberType.Rows.Add(_memberType)
      Next


      For Each _row As DataRow In dataSet.Tables(1).Rows
         _memberStatus = New HrsMemberStatus
         With _memberStatus
            .MemberStatusId = _row.ToInt32("MemberStatusId")
            .MemberStatusName = _row.ToString("MemberStatusName")
         End With

         _hrsMemberStatus.Rows.Add(_memberStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(2).Rows
         _sex = New DbsSex
         With _sex
            .SexId = _row.ToString("SexId")
            .SexName = _row.ToString("SexName")
         End With

         _dbsSex.Rows.Add(_sex)
      Next

      For Each _row As DataRow In dataSet.Tables(3).Rows
         _bloodType = New DbsBloodType
         With _bloodType
            .BloodTypeId = _row.ToInt32("BloodTypeId")
            .BloodTypeName = _row.ToString("BloodTypeName")
         End With

         _dbsBloodType.Rows.Add(_bloodType)
      Next

      For Each _row As DataRow In dataSet.Tables(4).Rows
         _civilStatus = New DbsCivilStatus
         With _civilStatus
            .CivilStatusId = _row.ToInt32("CivilStatusId")
            .CivilStatusName = _row.ToString("CivilStatusName")
         End With

         _dbsCivilStatus.Rows.Add(_civilStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(5).Rows
         _religion = New DbsReligion
         With _religion
            .ReligionId = _row.ToInt32("ReligionId")
            .ReligionName = _row.ToString("ReligionName")
         End With

         _dbsReligion.Rows.Add(_religion)
      Next

      For Each _row As DataRow In dataSet.Tables(6).Rows
         _region = New DbsRegion
         With _region
            .RegionId = _row.ToString("RegionId")
            .RegionName = _row.ToString("RegionName")
         End With

         _dbsRegion.Rows.Add(_region)
      Next

      For Each _row As DataRow In dataSet.Tables(7).Rows
         _province = New DbsProvince
         With _province
            .ProvinceId = _row.ToString("ProvinceId")
            .ProvinceName = _row.ToString("ProvinceName")
         End With

         _dbsProvince.Rows.Add(_province)
      Next

      For Each _row As DataRow In dataSet.Tables(8).Rows
         _municipality = New DbsMunicipality
         With _municipality
            .MunicipalityId = _row.ToString("MunicipalityId")
            .MunicipalityName = _row.ToString("MunicipalityName")
         End With

         _dbsMunicipality.Rows.Add(_municipality)
      Next

      For Each _row As DataRow In dataSet.Tables(9).Rows
         _barangay = New DbsBarangay
         With _barangay
            .BarangayId = _row.ToString("BarangayId")
            .BarangayName = _row.ToString("BarangayName")
         End With

         _dbsBarangay.Rows.Add(_barangay)
      Next

      For Each _row As DataRow In dataSet.Tables(10).Rows
         _relation = New DbsRelation
         With _relation
            .RelationId = _row.ToInt32("RelationId")
            .RelationName = _row.ToString("RelationName")
         End With

         _dbsRelation.Rows.Add(_relation)
      Next


      For Each _row As DataRow In dataSet.Tables(11).Rows
         _educationLevel = New DbsEducationLevel
         With _educationLevel
            .EducationLevelId = _row.ToInt32("EducationLevelId")
            .EducationLevelName = _row.ToString("EducationLevelName")
            .CourseNameRequiredFlag = _row.ToBoolean("CourseNameRequiredFlag")
         End With

         _dbsEducationLevel.Rows.Add(_educationLevel)
      Next

      For Each _row As DataRow In dataSet.Tables(12).Rows
         _disability = New DbsDisability
         With _disability
            .DisabilityId = _row.ToInt32("DisabilityId")
            .DisabilityName = _row.ToString("DisabilityName")
            '.CourseNameRequiredFlag = _row.ToBoolean("CourseNameRequiredFlag")
         End With

         _dbsDisability.Rows.Add(_disability)
      Next

      For Each _row As DataRow In dataSet.Tables(13).Rows
         _memberTypeQualification = New HrsMemberTypeQualification
         With _memberTypeQualification
            .TypeQualificationDetailId = _row.ToInt32("TypeQualificationDetailId")
            .TypeQualificationName = _row.ToString("TypeQualificationName")
            .MemberTypeId = _row.ToInt32("MemberTypeId")
            '.CourseNameRequiredFlag = _row.ToBoolean("CourseNameRequiredFlag")
         End With

         _hrsMemberTypeQualification.Rows.Add(_memberTypeQualification)
      Next

      For Each _row As DataRow In dataSet.Tables(14).Rows
         _cdaMemberType = New DbsCDAMemberType
         With _cdaMemberType
            .CDAMemberTypeId = _row.ToInt32("CDAMemberTypeId")
            .CDAMemberTypeName = _row.ToString("CDAMemberTypeName")
         End With

         _dbsCDAMemberType.Rows.Add(_cdaMemberType)
      Next

      For Each _row As DataRow In dataSet.Tables(15).Rows
         _employmentStatus = New DbsEmploymentStatus
         With _employmentStatus
            .EmploymentStatusId = _row.ToInt32("EmploymentStatusId")
            .EmploymentStatusName = _row.ToString("EmploymentStatusName")
         End With

         _dbsEmploymentStatus.Rows.Add(_employmentStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(16).Rows
         _bank = New DbsBank
         With _bank
            .BankId = _row.ToInt32("BankId")
            .BankName = _row.ToString("BankName")
         End With

         _dbsBank.Rows.Add(_bank)
      Next

      For Each _row As DataRow In dataSet.Tables(17).Rows
         _language = New DbsLanguage
         With _language
            .LanguageId = _row.ToInt32("LanguageId")
            .LanguageName = _row.ToString("LanguageName")
         End With

         _dbsLanguage.Rows.Add(_language)
      Next

      For Each _row As DataRow In dataSet.Tables(18).Rows
         _rnrRecording = New DbsRnrRecording
         With _rnrRecording
            .RnrRecordingId = _row.ToInt32("RnrRecordingId")
            .RnrRecordingName = _row.ToString("RnrRecordingName")
         End With

         _dbsRnrRecording.Rows.Add(_rnrRecording)
      Next

      For Each _row As DataRow In dataSet.Tables(19).Rows
         _vaccineType = New DbsVaccineType
         With _vaccineType
            .VaccineTypeId = _row.ToInt32("VaccineTypeId")
            .VaccineTypeName = _row.ToString("VaccineTypeName")
         End With

         _dbsVaccineType.Rows.Add(_vaccineType)
      Next

      For Each _row As DataRow In dataSet.Tables(20).Rows
         _docType = New DbsDocType
         With _docType
            .DocTypeId = _row.ToInt32("DocTypeId")
            .DocTypeName = _row.ToString("DocTypeName")
         End With

         _dbsDocType.Rows.Add(_docType)
      Next

      For Each _row As DataRow In dataSet.Tables(21).Rows
         _LicenseProfession = New DbsLicenseProfession
         With _LicenseProfession
            .LicenseProfessionId = _row.ToInt32("LicenseProfessionId")
            .LicenseProfessionName = _row.ToString("LicenseProfessionName")
         End With

         _dbsLicenseProfession.Rows.Add(_LicenseProfession)
      Next

      For Each _row As DataRow In dataSet.Tables(22).Rows
         _cDATraining = New DbsCDATraining
         With _cDATraining
            .CDATrainingId = _row.ToInt32("CDATrainingId")
            .CDATrainingName = _row.ToString("CDATrainingName")
         End With

         _dbsCDATraining.Rows.Add(_cDATraining)
      Next

      For Each _row As DataRow In dataSet.Tables(23).Rows
         _nCIIQualificationTitle = New DbsNCIIQualificationTitle
         With _nCIIQualificationTitle
            .NCIIQualificationTitleId = _row.ToInt32("NCIIQualificationTitleId")
            .NCIIQualificationTitleName = _row.ToString("NCIIQualificationTitleName")
         End With

         _dbsNCIIQualificationTitle.Rows.Add(_nCIIQualificationTitle)
      Next

      For Each _row As DataRow In dataSet.Tables(24).Rows
         _trainingInstitution = New DbsTrainingInstitution
         With _trainingInstitution
            .TrainingInstitutionId = _row.ToInt32("TrainingInstitutionId")
            .TrainingInstitutionName = _row.ToString("TrainingInstitutionName")
         End With

         _dbsTrainingInstitution.Rows.Add(_trainingInstitution)
      Next

      For Each _row As DataRow In dataSet.Tables(25).Rows
         _assessmentCenter = New DbsAssessmentCenter
         With _assessmentCenter
            .AssessmentCenterId = _row.ToInt32("AssessmentCenterId")
            .AssessmentCenterName = _row.ToString("AssessmentCenterName")
         End With

         _dbsAssessmentCenter.Rows.Add(_assessmentCenter)
      Next

      For Each _row As DataRow In dataSet.Tables(26).Rows
         _complianceTraining = New DbsComplianceTraining
         With _complianceTraining
            .ComplianceTrainingId = _row.ToInt32("ComplianceTrainingId")
            .ComplianceTrainingName = _row.ToString("ComplianceTrainingName")
         End With

         _dbsComplianceTraining.Rows.Add(_complianceTraining)
      Next

      For Each _row As DataRow In dataSet.Tables(27).Rows
         _affiliation = New DbsAffiliation
         With _affiliation
            .AffiliationId = _row.ToInt32("AffiliationId")
            .AffiliationName = _row.ToString("AffiliationName")
         End With

         _dbsAffiliation.Rows.Add(_affiliation)
      Next

      For Each _row As DataRow In dataSet.Tables(28).Rows
         _recruiter = New HrsRecruiter
         With _recruiter
            .RecruiterId = _row.ToInt32("RecruiterId")
            .RecruiterName = _row.ToString("RecruiterName")
         End With

         _hrsRecruiter.Rows.Add(_recruiter)
      Next

      For Each _row As DataRow In dataSet.Tables(29).Rows
         _applicationSource = New DbsApplicationSource
         With _applicationSource
            .ApplicationSourceId = _row.ToInt32("ApplicationSourceId")
            .ApplicationSourceName = _row.ToString("ApplicationSourceName")
         End With

         _dbsApplicationSource.Rows.Add(_applicationSource)
      Next

      For Each _row As DataRow In dataSet.Tables(30).Rows
         _medicalResultType = New DbsMedicalResultType
         With _medicalResultType
            .MedicalResultTypeId = _row.ToInt32("MedicalResultTypeId")
            .MedicalResultTypeName = _row.ToString("MedicalResultTypeName")
         End With

         _dbsMedicalResultType.Rows.Add(_medicalResultType)
      Next

      For Each _row As DataRow In dataSet.Tables(31).Rows
         _skillSet = New QDbsSkillSetDetail
         With _skillSet
            .SkillDetailId = _row.ToInt32("SkillDetailId")
            .SkillDetailName = _row.ToString("SkillDetailName")
         End With

         _dbsSkillSetDetailLog.Rows.Add(_skillSet)
      Next

      For Each _row As DataRow In dataSet.Tables(32).Rows
         _employmentType = New DbsEmploymentType
         With _employmentType
            .EmploymentTypeId = _row.ToInt32("EmploymentTypeId")
            .EmploymentTypeName = _row.ToString("EmploymentTypeName")
         End With

         _dbsEmploymentType.Rows.Add(_employmentType)
      Next

      For Each _row As DataRow In dataSet.Tables(33).Rows
         _rnrNumber = New DbsRnrNumber
         With _rnrNumber
            .RnrNumberId = _row.ToInt32("RnrNumberId")
            .RnrNumber = _row.ToDecimal("RnrNumber")
         End With

         _dbsRnrNumber.Rows.Add(_rnrNumber)
      Next


      For Each _row As DataRow In dataSet.Tables(34).Rows
         _applicantStatus = New HrsApplicantStatus
         With _applicantStatus
            .ApplicantStatusId = _row.ToInt32("ApplicantStatusId")
            .ApplicantStatusName = _row.ToString("ApplicantStatusName")
         End With

         _hrsApplicantStatus.Rows.Add(_applicantStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(35).Rows
         _memberSuffix = New DbsMemberSuffix
         With _memberSuffix
            .MemberSuffixId = _row.ToInt32("MemberSuffixId")
            .MemberSuffixName = _row.ToString("MemberSuffixName")
         End With

         _dbsMemberSuffix.Rows.Add(_memberSuffix)
      Next

      For Each _row As DataRow In dataSet.Tables(36).Rows
         _applicantScreening = New DbsApplicantScreening
         With _applicantScreening
            .ApplicantScreeningId = _row.ToInt32("ApplicantScreeningId")
            .ApplicantScreeningName = _row.ToString("ApplicantScreeningName")
         End With

         _dbsApplicantScreening.Rows.Add(_applicantScreening)
      Next

      For Each _row As DataRow In dataSet.Tables(37).Rows
         _screeningStatus = New DbsScreeningStatus
         With _screeningStatus
            .ScreeningStatusId = _row.ToInt32("ScreeningStatusId")
            .ScreeningStatusName = _row.ToString("ScreeningStatusName")
         End With

         _dbsScreeningStatus.Rows.Add(_screeningStatus)
      Next

      For Each _row As DataRow In dataSet.Tables(38).Rows
         _appStatus = New DbsApplicantStatus
         With _appStatus
            .ApplicantStatusId = _row.ToInt32("ApplicantStatusId")
            .ApplicantStatusName = _row.ToString("ApplicantStatusName")
         End With

         _dbsApplicantStatus.Rows.Add(_appStatus)
      Next

      'Dim _accountType As GlsAccountType
      'Dim _accountNature As GlsAccountNature
      'Dim _budgetClass As DbsBudgetClass
      'Dim _costCenter As DbsCostCenterCore
      'Dim _fundCluster As DbsFundCluster
      'Dim _journal As DbsJournal
      'Dim _trxClass As DbsTrxClass
      'Dim _trxStatus As DbsTrxStatus
      'Dim _coxType As DbsCoxType
      'Dim _dbxType As DbsDbxType
      'Dim _obxStatus As DbsObxStatus

      'For Each _row As DataRow In dataSet.Tables(0).Rows
      '   _accountType = New GlsAccountType
      '   With _accountType
      '      .AccountTypeId = _row.ToInt32("AccountTypeId")
      '      .AccountTypeName = _row.ToString("AccountTypeName")
      '   End With

      '   _glsAccountType.Rows.Add(_accountType)
      'Next

      'For Each _row As DataRow In dataSet.Tables(1).Rows
      '   _accountNature = New GlsAccountNature
      '   With _accountNature
      '      .AccountNatureId = _row.ToInt32("AccountNatureId")
      '      .AccountNatureName = _row.ToString("AccountNatureName")
      '   End With

      '   _glsAccountNature.Rows.Add(_accountNature)
      'Next

      'For Each _row As DataRow In dataSet.Tables(2).Rows
      '   _budgetClass = New DbsBudgetClass
      '   With _budgetClass
      '      .BudgetClassId = _row.ToInt32("BudgetClassId")
      '      .BudgetClassName = _row.ToString("BudgetClassName")
      '   End With

      '   _dbsBudgetClass.Rows.Add(_budgetClass)
      'Next

      'For Each _row As DataRow In dataSet.Tables(3).Rows
      '   _costCenter = New DbsCostCenterCore
      '   With _costCenter
      '      .CostCenterId = _row.ToInt32("CostCenterId")
      '      .CostCenterName = _row.ToString("CostCenterName")
      '      .CostCenterShortName = _row.ToString("CostCenterShortName")
      '   End With

      '   _dbsCostCenter.Rows.Add(_costCenter)
      'Next

      'For Each _row As DataRow In dataSet.Tables(4).Rows
      '   _fundCluster = New DbsFundCluster
      '   With _fundCluster
      '      .FundClusterId = _row.ToString("FundClusterId")
      '      .FundClusterName = _row.ToString("FundClusterName")
      '   End With

      '   _dbsFundCluster.Rows.Add(_fundCluster)
      'Next

      'For Each _row As DataRow In dataSet.Tables(5).Rows
      '   _journal = New DbsJournal
      '   With _journal
      '      .JournalId = _row.ToInt32("JournalId")
      '      .JournalName = _row.ToString("JournalName")
      '      .JournalShortName = _row.ToString("JournalShortName")
      '   End With

      '   _dbsJournal.Rows.Add(_journal)
      'Next

      'For Each _row As DataRow In dataSet.Tables(6).Rows
      '   _trxClass = New DbsTrxClass
      '   With _trxClass
      '      .TrxClassId = _row.ToInt32("TrxClassId")
      '      .TrxClassName = _row.ToString("TrxClassName")
      '   End With

      '   _dbsTrxClass.Rows.Add(_trxClass)
      'Next

      'For Each _row As DataRow In dataSet.Tables(7).Rows
      '   _trxStatus = New DbsTrxStatus
      '   With _trxStatus
      '      .TrxStatusId = _row.ToInt32("TrxStatusId")
      '      .TrxStatusName = _row.ToString("TrxStatusName")
      '   End With

      '   _dbsTrxStatus.Rows.Add(_trxStatus)
      'Next

      'For Each _row As DataRow In dataSet.Tables(8).Rows
      '   _coxType = New DbsCoxType
      '   With _coxType
      '      .CoxTypeId = _row.ToInt32("CoxTypeId")
      '      .CoxTypeName = _row.ToString("CoxTypeName")
      '   End With

      '   _dbsCoxType.Rows.Add(_coxType)
      'Next

      'For Each _row As DataRow In dataSet.Tables(9).Rows
      '   _dbxType = New DbsDbxType
      '   With _dbxType
      '      .DbxTypeId = _row.ToInt32("DbxTypeId")
      '      .DbxTypeName = _row.ToString("DbxTypeName")
      '   End With

      '   _dbsDbxType.Rows.Add(_dbxType)
      'Next

      'For Each _row As DataRow In dataSet.Tables(10).Rows
      '   _obxStatus = New DbsObxStatus
      '   With _obxStatus
      '      .ObxStatusId = _row.ToInt32("ObxStatusId")
      '      .ObxStatusName = _row.ToString("ObxStatusName")
      '   End With

      '   _dbsObxStatus.Rows.Add(_obxStatus)
      'Next

   End Sub
   Friend Shared ReadOnly Property HrsMemberType As QHrsMemberType
      Get
         Return _hrsMemberType
      End Get
   End Property
   Friend Shared ReadOnly Property HrsMemberStatus As QHrsMemberStatus
      Get
         Return _hrsMemberStatus
      End Get
   End Property

   Friend Shared ReadOnly Property DbsSex As QDbsSex
      Get
         Return _dbsSex
      End Get
   End Property

   Friend Shared ReadOnly Property DbsBloodType As QDbsBloodTYpe
      Get
         Return _dbsBloodType
      End Get
   End Property
   Friend Shared ReadOnly Property DbsCivilStatus As QDbsCivilStatus
      Get
         Return _dbsCivilStatus
      End Get
   End Property

   Friend Shared ReadOnly Property DbsReligion As QDbsReligion
      Get
         Return _dbsReligion
      End Get
   End Property

   Friend Shared ReadOnly Property DbsRegion As QDbsRegion
      Get
         Return _dbsRegion
      End Get
   End Property

   Friend Shared ReadOnly Property DbsProvince As QDbsProvince
      Get
         Return _dbsProvince
      End Get
   End Property

   Friend Shared ReadOnly Property DbsMunicipality As QDbsMunicipality
      Get
         Return _dbsMunicipality
      End Get
   End Property

   Friend Shared ReadOnly Property DbsBarangay As QDbsBarangay
      Get
         Return _dbsBarangay
      End Get
   End Property

   Friend Shared ReadOnly Property DbsRelation As QDbsRelation
      Get
         Return _dbsRelation
      End Get
   End Property

   Friend Shared ReadOnly Property DbsEducationLevel As QDbsEducationLevel
      Get
         Return _dbsEducationLevel
      End Get
   End Property

   Friend Shared ReadOnly Property DbsDisability As QDbsDisability
      Get
         Return _dbsDisability
      End Get
   End Property
   Friend Shared ReadOnly Property HrsMemberTypeQualification As QHrsMemberTypeQualification
      Get
         Return _hrsMemberTypeQualification
      End Get
   End Property
   Friend Shared ReadOnly Property DbsCDAMemberType As QDbsCDAMemberType
      Get
         Return _dbsCDAMemberType
      End Get
   End Property
   Friend Shared ReadOnly Property DbsEmploymentStatus As QDbsEmploymentStatus
      Get
         Return _dbsEmploymentStatus
      End Get
   End Property
   Friend Shared ReadOnly Property DbsBank As QDbsBank
      Get
         Return _dbsBank
      End Get
   End Property
   Friend Shared ReadOnly Property DbsLanguage As QDbsLanguage
      Get
         Return _dbsLanguage
      End Get
   End Property
   Friend Shared ReadOnly Property DbsRnrRecording As QDbsRnrRecording
      Get
         Return _dbsRnrRecording
      End Get
   End Property
   Friend Shared ReadOnly Property DbsVaccineType As QDbsVaccineType
      Get
         Return _dbsVaccineType
      End Get
   End Property
   Friend Shared ReadOnly Property DbsDocType As QDbsDocType
      Get
         Return _dbsDocType
      End Get
   End Property
   Friend Shared ReadOnly Property DbsLicenseProfession As QDbsLicenseProfession
      Get
         Return _dbsLicenseProfession
      End Get
   End Property
   Friend Shared ReadOnly Property DbsCDATraining As QDbsCDATraining
      Get
         Return _dbsCDATraining
      End Get
   End Property
   Friend Shared ReadOnly Property DbsNCIIQualificationTitle As QDbsNCIIQualificationTitle
      Get
         Return _dbsNCIIQualificationTitle
      End Get
   End Property
   Friend Shared ReadOnly Property DbsTrainingInstitution As QDbsTrainingInstitution
      Get
         Return _dbsTrainingInstitution
      End Get
   End Property
   Friend Shared ReadOnly Property DbsAssessmentCenter As QDbsAssessmentCenter
      Get
         Return _dbsAssessmentCenter
      End Get
   End Property
   Friend Shared ReadOnly Property DbsComplianceTraining As QDbsComplianceTraining
      Get
         Return _dbsComplianceTraining
      End Get
   End Property
   Friend Shared ReadOnly Property DbsAffiliation As QDbsAffiliation
      Get
         Return _dbsAffiliation
      End Get
   End Property
   Friend Shared ReadOnly Property HrsRecruiter As QHrsRecruiter
      Get
         Return _hrsRecruiter
      End Get
   End Property
   Friend Shared ReadOnly Property DbsApplicationSource As QDbsApplicationSource
      Get
         Return _dbsApplicationSource
      End Get
   End Property
   Friend Shared ReadOnly Property DbsMedicalResultType As QDbsbsMedicalResultType
      Get
         Return _dbsMedicalResultType
      End Get
   End Property
   Friend Shared ReadOnly Property DbsSkillSetDetail As QDbsSkillSetDetailLog
      Get
         Return _dbsSkillSetDetailLog
      End Get
   End Property
   Friend Shared ReadOnly Property DbsSkillSetDetailLog As QDbsSkillSetDetailLog
      Get
         Return _dbsSkillSetDetailLog
      End Get
   End Property
   Friend Shared ReadOnly Property DbsEmploymentType As QDbsEmploymentType
      Get
         Return _dbsEmploymentType
      End Get
   End Property

   Friend Shared ReadOnly Property DbsRnrNumber As QDbsRnrNumber
      Get
         Return _dbsRnrNumber
      End Get
   End Property
   Friend Shared ReadOnly Property HrsApplicantStatus As QHrsApplicantStatus
      Get
         Return _hrsApplicantStatus
      End Get
   End Property
   Friend Shared ReadOnly Property DbsMemberSuffix As QDbsMemberSuffix
      Get
         Return _dbsMemberSuffix
      End Get
   End Property

   Friend Shared ReadOnly Property DbsApplicantScreening As QDbsApplicantScreening
      Get
         Return _dbsApplicantScreening
      End Get
   End Property

   Friend Shared ReadOnly Property DbsScreeningStatus As QDbsScreeningStatus
      Get
         Return _dbsScreeningStatus
      End Get
   End Property

   Friend Shared ReadOnly Property DbsApplicantStatus As QDbsApplicantStatus
      Get
         Return _dbsApplicantStatus
      End Get
   End Property

   'Friend Shared ReadOnly Property GlsAccountType As QGlsAccountType
   '   Get
   '      Return _glsAccountType
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property GlsAccountNature As QGlsAccountNature
   '   Get
   '      Return _glsAccountNature
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsBudgetClass As QDbsBudgetClass
   '   Get
   '      Return _dbsBudgetClass
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsCostCenter As QDbsCostCenterCore
   '   Get
   '      Return _dbsCostCenter
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsFundCluster As QDbsFundCluster
   '   Get
   '      Return _dbsFundCluster
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsJournal As QDbsJournal
   '   Get
   '      Return _dbsJournal
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsTrxClass As QDbsTrxClass
   '   Get
   '      Return _dbsTrxClass
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsTrxStatus As QDbsTrxStatus
   '   Get
   '      Return _dbsTrxStatus
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsCoxType As QDbsCoxType
   '   Get
   '      Return _dbsCoxType
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsDbxType As QDbsDbxType
   '   Get
   '      Return _dbsDbxType
   '   End Get
   'End Property

   'Friend Shared ReadOnly Property DbsObxStatus As QDbsObxStatus
   '   Get
   '      Return _dbsObxStatus
   '   End Get
   'End Property

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
