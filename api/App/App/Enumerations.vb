Public Enum LogActionId
   Add = 1
   Edit = 2
   Delete = 3
End Enum

Public Enum LogColumnId
   Reference = 5
   Remarks = 6
   Particulars = 7
   Amount = 8

   ExpiryDate = 153
   AccountId = 200
   AccountName = 201
   AccountTypeId = 202
   AccountNatureId = 203
   HeaderAccountId = 204

   CostCenterId = 210
   CostCenterName = 211
   CostCenterShortName = 212
   'Signatory = 213
   SignatoryName = 213
   CostCenterCode = 214

   ActivityId = 220
   ActivityName = 221
   ActivityShortName = 222
   PapCode = 223
   HeaderActivityId = 224
   ExcludeFlag = 225

   AppropriationId = 230
   AppropriationName = 231
   PostDate = 232
   ReferenceDate = 233
   ReferenceDocument = 234
   BudgetClassId = 235

   AllotmentId = 237
   AllotmentName = 238

   PSRAmount = 241
   RLIPAmount = 242
   MOOEAmount = 243
   FEAmount = 244
   COAmount = 245

   TrxId = 250
   TrxDate = 251
   DocumentId = 252
   'Reference = 253
   'Particulars = 254
   ADASeriesFrom = 253
   ADASeriesTo = 254
   CollectorName = 255
   DisburserName = 256
   CheckSeriesFrom = 257
   CheckSeriesTo = 258
   CheckNumber = 259

   FundClusterId = 260
   FundClusterName = 261

   ReferenceGJ = 263
   ReferenceCRJ = 264
   ReferenceCDJ = 265
   ReferenceCKDJ = 266
   ReferenceADADJ = 267
   'ReferenceEMDSDJ = 268

   DebitAmount = 271
   CreditAmount = 272
   ADASeries = 273
   EMDSSeries = 274

   TemplateId = 280
   TemplateName = 281

   JournalId = 290
   JournalName = 291

   PayorName = 294
   CollectorId = 295
   DisburserId = 296

   ObxId = 300
   ObxDate = 301
   ObxStatusId = 302
   PayeeId = 303
   PayeeName = 304
   PayeeOffice = 305
   PayeeAddress = 306
   ApprovedFlag = 307
   DisburseFlag = 308

   TrxCode = 310
   TrxName = 311
   TrxClassId = 312
   PayOrderFlag = 313

   TrxStatusId = 320
   CoxTypeId = 322
   DbxTypeId = 323
   ReceiptNumber = 324

   VoucherId = 330
   TotalAmount = 331
   TaxAmount = 332

   MemberId = 400
   MemberLastName = 401
   MemberFirstName = 402
   MemberMiddleName = 403
   MemberSuffix = 404
   MemberTypeId = 405
   MemberStatusId = 406
   BirthDate = 407
   BirthPlace = 408
   SexId = 409
   BloodTypeId = 410
   Height = 411
   Weight = 412
   CivilStatusId = 413
   ReligionId = 414
   SSSNumber = 415
   TaxIdNumber = 416
   PagIbigNumber = 417
   PhilHealthNumber = 418
   Address1 = 419
   Address2 = 420
   PostalCode = 421
   PhoneNumber = 422
   MobileNumber = 423
   Email = 424
   RegionId = 425
   ProvinceId = 426
   MunicipalityId = 427
   BarangayId = 428
   Facebook = 429
   Instagram = 430
   AlternateAddress1 = 431
   AlternateAddress2 = 432

   SchoolName = 433
   CourseName = 434
   EduStartYear = 435
   EduEndYear = 436

   WorkPosition = 437
   CompanyName = 438
   CompanyAddress = 439
   CompanyPhoneNumber = 440
   WorkStartDate = 441
   WorkEndDate = 442

   KinName = 443
   RelationId = 444
   KinPhoneNumber = 445
   EducationLevelId = 446
   DisabilityId = 447
   MemberBadgeId = 448
   TypeQualificationDetailId = 449

   CertificateDetailId = 450
   CertificateName = 451
   Rating = 452
   IssuedBy = 453
   IssuedDate = 454

   EligibilityDetailId = 455
   EligibilityName = 456
   YearTaken = 457

   LicenseTitle = 458
   SkillSetDetailId = 459
   SkillDetailId = 460
   SkillName = 461
   SkillSetName = 462
   DocTypeDetailId = 463
   FileName = 464
   CDAMemberTypeId = 465
   EmploymentStatusId = 466
   AbroadFlag = 467
   RelocateFlag = 468
   WeekendHolidayFlag = 469
   ExpectedSalary = 470
   GCashNumber = 471
   BankId = 472
   BankAccountNumber = 473
   BirthRegionId = 474
   BirthProvinceId = 475
   BirthMunicipalityId = 476
   LanguageId = 477
   RnrRecordingId = 478
   RnrNumber = 479
   VaccineTypeId = 480
   VaccineName = 481
   VaccineDate = 482
   KinOccupation = 483
   KinEmail = 484
   KinFileName = 485
   KinGUID = 486
   DocTypeId = 487
   DocTypeReference = 488
   DocTypeFileName = 489
   DocTypeGUID = 490
   EduFileName = 491
   LicenseProfessionId = 492
   PRCIdNo = 493
   LicenseFileName = 494
   CertificateNumber = 495
   CDATrainingId = 496
   TrainingDate = 497
   CDAFileName = 498
   NCIIQualificationTitleId = 499
   IssuanceDate = 500
   ValidityDate = 501
   TrainingInstitutionId = 502
   AssessmentCenterId = 503
   NCIIFileName = 504
   NCIIQualificationTitleName = 505
   TrainingInstitutionName = 506
   AssessmentCenterName = 507
   ComplianceTrainingId = 508
   ComplianceTrainingName = 509
   ComplianceFileName = 510
   ReasonForLeaving = 511
   WorkFileName = 512
   AffiliationId = 513
   AffiliationDate = 514
   AffiliationPosition = 515
   AffiliationDescription = 516
   AffiliationFileName = 517
   RecruiterId = 518
   ApplicationSourceId = 519
   PhotoFileName = 520
   MedicalResultTypeId = 521
   MedicalResultTypeFileName = 522
   EmploymentTypeId = 523
   RnrNumberId = 524
   ApplicantId = 525
   ApplicantLastName = 526
   ApplicantFirstName = 527
   ApplicantMiddleName = 528
   ApplicantSuffix = 529
   ApplicationDate = 530
   ApplicantStatusId = 531
   MemberEmployeeId = 532
   ApplicantSuffixId = 533
   MemberSuffixId = 534
   AlternateRegionId = 535
   AlternateProvinceId = 536
   AlternateMunicipalityId = 537
   AlternateBarangayId = 538
   AlternatePostalCode = 539
   ApplicantDetailId = 540
   ApplicantScreeningId = 541
   HiredDate = 542
   DeploymentDate = 543
   ScreeningStatusId = 544
   EmploymentStatusActionId = 545
   EmploymentStatusMemberId = 546
   EffectivityDate = 547
   StatusActionId = 548
   MemberRequestId = 549
   KinLastName = 550
   KinFirstName = 551
   KinMiddleName = 552
   KinSuffixId = 553
   EmergencyContactFlag = 554
   CDAMemberTypeAmount = 557
    EndDate = 558

End Enum

Public Enum DocSequencerId
   ORS = 1
   JEV_GJ = 2
   JEV_CRJ = 3
   JEV_CDJ = 4
   JEV_CKDJ = 5
   JEV_ADADJ = 6
   RCI = 7
   RADAI = 8
   OP = 9
   NORSA = 10
   JEV_EMDSDJ = 11
   LC = 12
   JEV_CAEDJ = 13      ' CK, ADA, EMDS
End Enum

Public Enum NextSeqId
   JournalVoucher = 1
   SetupBalance = 2
   AccountsReceivable = 11
   CashReceipt = 12
   AccountsPayable = 21
   CheckVoucher = 22

End Enum


Public Enum FractionStyle
   Word
   PerHundred
End Enum
Public Enum EmploymentStatus
   Active = 1
   LeaveOfAbsence = 2
   Inactive = 3
   AbsenceWithoutLeave = 4
   Floating = 5
   Suspended = 6
   Resigned = 7
   Separated = 8
   Terminated = 9
   EndOfContract = 10
   Retired = 11
   Delisted = 12
End Enum
Public Enum MemberStatus
    Active = 1
    InActive = 2
    Suspended = 3
    Delisted = 4
End Enum

Public Enum LogTable
   Member = 1
   Language = 2
   RnrRecording = 3
   Vaccine = 4
   SkillSet = 5
   Kin = 6
   DocType = 7
   Education = 8
   LicenseProfession = 9
   CDATraining = 10
   NCIIQualificationTitle = 11
   ComplianceTraining = 12
   Work = 13
   Affiliation = 14
   MedicalResultType = 15
   Applicant = 16
End Enum

Public Enum MemberRequestStatus
   Active = 1
   InActive = 2
   Canceled = 3
   Closed = 4
End Enum
Public Enum ApplicantStatus
   Active = 1
   Member = 2
   Canceled = 3
End Enum
Public Enum ClientPayGroupStatus
   Active = 1
   InActive = 2
   Canceled = 3
   Closed = 4
End Enum
Public Enum EmploymentApplicantStatus
   Listed = 1
   Delisted = 2
    Hired = 3
    Failed = 4
End Enum
