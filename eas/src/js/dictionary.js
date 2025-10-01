/* jslint esversion: 6  */

export const

dict = {
  sysDate: {
    caption: 'Date'
  },

  auditDate: {
    caption: 'Date'
  },

  password: {
    maxLength: 100
  },

  accountId: {
    maxLength: 5,
    // minLength: 5
  },

  accountName: {
    maxLength: 255,
  },

  headerAccountId: {
    maxLength: 5,
    minLength: 5
  },

  religionId: {
    entityId: true
  },

  religionName: {
    maxLength: 25
  },

  relationId: {
    entityId: true
  },

  relationName: {
    maxLength: 25
  },

  platformId: {
    entityId: true
  },

  platformName: {
    maxLength: 50
  },

  platformShortName: {
    maxLength: 10,
    mask: '!'
  },

  industryId: {
    entityId: true
  },

  industryName: {
    maxLength: 30
  },

   regionId: {
    datatype: 'S',
    maxLength: 2,
    minLength: 2
  },

  provinceId: {
    datatype: 'S',
    maxLength: 5,
    mask: "!",
    entityId: true,
  },

  municipalityId: {
    datatype: 'S',
    maxLength: 7,
  },

  barangayId: {
    datatype: 'S',
    maxLength: 10,
  },

  barangayName: {
    maxLength: 50
  },

  memberId: {
    entityId: true
  },

  memberBadgeId: {
    maxLength: 6,
  },
  memberEmployeeId: {
    maxLength: 8,
  },
  memberLastName: {
    maxLength: 30,
    mask: '!'
  },

  memberFirstName: {
    maxLength: 30,
    mask: '!'
  },

  memberMiddleName: {
    maxLength: 30,
    mask: '!'
  },

  memberSuffix: {
    maxLength: 5
  },

  birthPlace: {
    maxLength: 30
  },

  height: {
    entityId: true,
    maxLength: 3
  },

  weight: {
    entityId: true,
    maxLength: 3
  },

  sssNumber: {
    maxLength: 20
  },

  taxIdNumber: {
    maxLength: 20
  },

  pagIbigNumber: {
    maxLength: 20
  },

  philHealthNumber: {
    maxLength: 20
  },

  address1: {
    maxLength: 100,
     mask: '!'

  },

  address2: {
    maxLength: 100,
     mask: '!'
  },

  postalCode: {
    entityId: true,
    maxLength: 4
  },

  phoneNumber: {
    maxLength: 25
  },

  mobileNumber: {
    maxLength: 11,
    entityId: true,
    mask: '!'
  },

  email: {
    maxLength: 80
  },

  facebook: {
    maxLength: 80
  },

  instagram: {
    maxLength: 80
  },

  alternateAddress1: {
    maxLength: 50
  },

  alternateAddress2: {
    maxLength: 50
  },

  kinName: {
    maxLength: 50
  },

  kinPhoneNumber: {
    maxLength: 15
  },

  kinOccupation: {
    maxLength: 25
  },

  kinEmail: {
    maxLength: 50
  },

  kinFileName: {
    maxLength: 255
  },

  skillId: {
    entityId: true
  },

  skillName: {
    maxLength: 25
  },

  skillSetId: {
    entityId: true
  },

  skillSetName: {
    maxLength: 50
  },

  typeQualificationDetailId: {
    entityId: true
  },

  educationLevelId: {
    entityId: true
  },

  schoolName: {
    maxLength: 50
  },

  courseName: {
    maxLength: 50
  },

  eduStartYear: {
    entityId: true,
    maxLength: 4
  },

  eduEndYear: {
    entityId: true,
    maxLength: 4
  },

  prcIdNo: {
    maxLength: 15
  },

  workTitle: {
    maxLength: 30
  },

  reasonForLeaving:{
    maxLength: 50
  },

  companyName: {
    maxLength: 50
  },

  companyAddress: {
    maxLength: 50
  },

  companyPhoneNumber: {
    maxLength: 15
  },

  certificateName: {
    maxLength: 50
  },

  rating: {
    maxLength: 15
  },

  issuedBy: {
    maxLength: 50
  },

  eligibilityName: {
    maxLength: 50
  },

  yearTaken: {
    maxLength: 4
  },

  licenseTitle: {
    maxLength: 50
  },

  skillDetailId: {
    entityId: true
  },

  remarks: {
    maxLength: 50
  },

  fileName: {
    maxLength: 255
  },

  expectedSalary: {
    entityId: true
  },

  docTypeId: {
    entityId: true
  },

  docTypeReference: {
    maxLength: 25
  },

  affiliationId: {
    entityId: true
  },

  affiliationPosition: {
    maxLength: 50
  },

  affiliationDescription: {
    maxLength: 50
  },

  workPosition: {
    maxLength: 50
  },

  certificateNumber: {
    maxLength: 15
  },

  rnrNumber: {
    // entityId: true,
    maxLength: 3
  },

  birthRegionId: {
    datatype: 'S',
    // maxLength: 2,
    // minLength: 2
  },

  birthProvinceId: {
    datatype: 'S',
    // maxLength: 5,
    // minLength: 5
  },

  birthMunicipalityId: {
    datatype: 'S',
    // maxLength: 7,
    // minLength: 7
  },

  gCashNumber: {
    maxLength: 12
  },

  bankAccountNumber: {
    maxLength: 15
  },

  affiliationDate: {
    datatype: 'D',
    maxLength: 8,
    minLength: 8
  },

  clientName: {
    maxLength: 100
  },

  faxNumber: {
    maxLength: 25
  },

  dueDays: {
    maxLength: 3
  },

  clientId: {
    entityId: true
  },

  contactPerson: {
    maxLength: 50
  },

  contactPosition: {
    maxLength: 50
  },

  // activityId: {
  //   minValue: 10000,
  //   maxValue: 32767,
  //   entityId: true
  // },

  referenceDocument: {
    maxLength: 30
  },

  trxId: {
    entityId: true
  },

  reference: {
    maxLength: 15
  },

  templateId: {
    entityId: true
  },

  templateName: {
    maxLength: 30
  },

  // 05 Feb 2025 - EMT (orgId onwards)

  orgId: {
    entityId: true
  },

  orgName: {
    maxLength: 50
  },

  orgShortName: {
    maxLength: 25
  },

  educationLevelName: {
    maxLength: 40
  },

  educationLevelCode: {
    maxLength: 3
  },

  disabilityId: {
    entityId: true
  },

  disabilityName: {
    maxLength: 20
  },

  docTypeName: {
    maxLength: 50
  },

  memberTypeId: {
    entityId: true
  },

  memberTypeName: {
    maxLength: 20
  },

  typeQualificationName: {
    maxLength: 50
  },

  applicationSourceId: {
    entityId: true
  },

  applicationSourceName: {
    maxLength: 25
  },

  licenseProfessionId: {
    entityId: true
  },

  licenseProfessionName: {
    maxLength: 50
  },

  applicantPositionId: {
    entityId: true
  },

  applicantPositionName: {
    maxLength: 50
  },

  contractTypeId: {
    entityId: true
  },

  contractTypeName: {
    maxLength: 50
  },

  chargingConsiderationId: {
    entityId: true
  },

  chargingConsiderationName: {
    maxLength: 25
  },

  inHouseBenefitId: {
    entityId: true
  },

  inHouseBenefitName: {
    maxLength: 25
  },

  statutoryBenefitId: {
    entityId: true
  },

  statutoryBenefitName: {
    maxLength: 25
  },

  nciiQualificationTitleId: {
    entityId: true
  },

  nciiQualificationTitleName: {
    maxLength: 100
  },

  trainingInstitutionId: {
    entityId: true
  },

  trainingInstitutionName: {
    maxLength: 50
  },

  assessmentCenterId: {
    entityId: true
  },

  assessmentCenterName: {
    maxLength: 50
  },

  complianceTrainingId: {
    entityId: true
  },

  complianceTrainingName: {
    maxLength: 100
  },

  insuranceCoverageId: {
    entityId: true
  },

  insuranceCoverageName: {
    maxLength: 50
  },

  savingLoanId: {
    entityId: true
  },

  savingLoanName: {
    maxLength: 50
  },

  schoolId: {
    entityId: true
  },

  // schoolName: {
  //   maxLength: 50
  // },

  courseId: {
    entityId: true
  },

  // courseName: {
  //   maxLength: 50
  // },

  bankId: {
    entityId: true
  },

  bankName: {
    maxLength: 50
  },

  bankLocationId: {
    entityId: true
  },

  bankLocationName: {
    maxLength: 50
  },

  memberTransactionTypeId: {
    entityId: true
  },

  memberTransactionTypeName: {
    maxLength: 50
  },

  expenseChargingId: {
    entityId: true
  },

  expenseChargingName: {
    maxLength: 50
  },

  inventoryChargeId: {
    entityId: true
  },

  inventoryChargeName: {
    maxLength: 25
  },

  allowanceId: {
    entityId: true
  },

  allowanceName: {
    maxLength: 25
  },

  deminimisId: {
    entityId: true
  },

  deminimisName: {
    maxLength: 25
  },

  // affiliationId: {
  //   entityId: true
  // },

  affiliationName: {
    maxLength: 25
  },

  cdaTrainingId: {
    entityId: true
  },

  cdaTrainingName: {
    maxLength: 25
  },

  dayTypeId: {
    entityId: true
  },

  dayTypeName: {
    maxLength: 25
  },

  dayTypeCode: {
    maxLength: 15
  },

  holidayId: {
    entityId: true
  },

  holidayName: {
    maxLength: 25
  },

  cdaMemberTypeId: {
    entityId: true
  },

  cdaMemberTypeName: {
    maxLength: 25
  },

  languageId: {
    entityId: true
  },

  languageName: {
    maxLength: 25
  },

  rnrRecordingId: {
    entityId: true
  },

  rnrRecordingName: {
    maxLength: 25
  },

  revenueQualificationId: {
    entityId: true
  },

  revenueQualificationName: {
    maxLength: 25
  },

  nonRevenueQualificationId: {
    entityId: true
  },

  nonRevenueQualificationName: {
    maxLength: 25
  },

  vaccineTypeId: {
    entityId: true
  },

  vaccineTypeName: {
    maxLength: 25
  },

  medicalResultTypeId: {
    entityId: true
  },

  medicalResultTypeName: {
    maxLength: 50
  },

  medicalResultTypeCode: {
    maxLength: 15,
    mask: '!'
  },

  engagementTypeId: {
    entityId: true
  },

  engagementTypeName: {
    maxLength: 50
  },

  engagementTypeCode: {
    maxLength: 15
  },

  whtRangeId: {
    entityId: true
  },

  sssRangeId: {
    entityId: true
  },

  pbgRangeId: {
    entityId: true
  },

  phhRangeId: {
    entityId: true
  },
  workingDays: {
    maxLength: 3
  },
  
  adminFee: {
    maxLength: 3,
    entityId: true

  },

  applicantLastName: {
    maxLength: 30,
    mask: '!'
  },

  applicantFirstName: {
    maxLength: 30,
    mask: '!'
  },

  applicantMiddleName: {
    maxLength: 30,
    mask: '!'
  },

  kinLastName: {
    maxLength: 30,
    mask: '!'
  },

  kinFirstName: {
    maxLength: 30,
    mask: '!'
  },

  kinMiddleName: {
    maxLength: 30,
    mask: '!'
  },

  payTrxCode: {
    maxLength: 10,
    mask: '!'
  },
  
  payTrxName: {
    maxLength: 50,
    // mask: '!'
  },

  alternateMunicipalityId: {
   datatype: 'S',
    // maxLength: 7,
    // minLength: 7
  },

    alternateProvinceId: {
    datatype: 'S',
    // maxLength: 5,
    // minLength: 5
  },
    
    alternateBarangayId: {
    datatype: 'S',
    // maxLength: 10,
    // minLength: 10
  },
  
  memberRequestRemarks: {
    maxLength: 300
  },

  scheduleCode: {
    mask: '!'
  },

  scheduleIn: {
    maxLength: 4,
  },

  scheduleOut: {
    maxLength: 4,
  },

  memberRequestName: {
    maxLength: 50,
  },

  yearId: {
    maxLength: 4,
    mask: '!',
    entityId: true
  },

  clientPayGroupName: {
    maxLength: 50,
    mask: '!'
  },

  timekeepingDescription: {
    maxLength: 50,
    mask: '!' 
  },

  minimumRequiredOvertimeCount: {
    maxLength: 3,
    mask: '!'
  },

  overtimeDurationCount: {
    maxLength: 3,
    mask: '!'
  },

  maximumRegularHourCount: {
    maxLength: 3,
    mask: '!'
  },

  maximumRegularHourCount: {
    maxLength: 3,
    mask: '!'
  },

  lateGracePeriodCount: {
    maxLength: 3,
    mask: '!'
  },

  lateCount: {
    maxLength: 3,
    mask: '!'
  },

  unpaidBreak: {
    maxLength: 3  ,
    mask: '!'
  },

  scheduleTimeIn: {
    maxLength: 4,
    mask: '!'
  },


  scheduleTimeOut: {
    maxLength: 4,
    mask: '!'
  },


  phoneNumber: {
    maxLength: 12,
    entityId: true,
    mask: '!'
  },

  // mobileNumber: {
  //   maxLength: 11,
  //   entityId: true,
  //   mask: '!'
  // },

  timeIn: {
    maxLength: 4,
    
  },

  timeOut: {
    maxLength: 4,
    
  },

  contactMobileNumber:{
    maxLength: 11,
    entityId: true,
    mask: '!'
  },

  age:{
    maxLength: 2,
    entityId: true,
    mask: '!'
  },

  clientContractCRN:{
    maxLength: 15,
    entityId: true,
    mask: '!'
  },

  taxIdNumber:{
    maxLength: 12,
    
  },

  clientPayOutDetailId: {
    maxLength: 8,
    entityId: true

  },

};