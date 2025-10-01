
Imports ImageMagick

<RoutePrefix("api")>
Public Class HRS0010_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetMemberPoolingLog")>
    <Route("member-pooling/{applicantDetailId}/log")>
    <HttpGet>
    Public Function GetMemberPoolingLog(applicantDetailId As Integer) As IHttpActionResult

        If applicantDetailId <= 0 Then
            Throw New ArgumentException("Applicant Detail ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("Web.ARS0050_Log")
                With _direct
                    .AddParameter("applicantDetailId", applicantDetailId)

                    ' Allow DateTime
                    Dim _settings As New JsonSerializerSettings
                    With _settings
                        .ContractResolver = New CamelCasePropertyNamesContractResolver
                        .DateFormatString = "yyyy-MM-ddTHH:mm"
                    End With

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        'Return Me.Ok(_dataTable)
                        Return Json(_dataTable, _settings)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function
    <SymAuthorization("GetMemberTypeQualifications")>
    <Route("member-type-qualifications/{memberTypeId}")>
    <HttpGet>
    Public Function GetMemberTypeQualifications(memberTypeId As Integer) As IHttpActionResult

        Dim _filter As String = "MemberTypeId=" + memberTypeId.ToString
        Dim _dataSource As String = "dbo.HrsMemberTypeQualification"
        Dim _fields As String = "TypeQualificationDetailId, MemberTypeId, TypeQualificationName, SortSeq"
        Dim _sort As String = "TypeQualificationName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetReferences_HRS0010")>
    <Route("references/hrs0010")>
    <HttpGet>
    Public Function GetReferences_HRS0010() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.HRS0010_References")
                With _direct

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "memberType"
                            .Tables(1).TableName = "memberStatus"
                            .Tables(2).TableName = "sex"
                            .Tables(3).TableName = "bloodType"
                            .Tables(4).TableName = "civilStatus"
                            .Tables(5).TableName = "religion"
                            .Tables(6).TableName = "relation"
                            .Tables(7).TableName = "memberValidation"
                            .Tables(8).TableName = "region"
                            .Tables(9).TableName = "birthRegion"
                            .Tables(10).TableName = "memberTypeQualification"
                            .Tables(11).TableName = "cdaMemberTypes"
                            .Tables(12).TableName = "employmentStatus"
                            .Tables(13).TableName = "banks"
                            .Tables(14).TableName = "languages"
                            .Tables(15).TableName = "disabilities"
                            .Tables(16).TableName = "recordings"
                            .Tables(17).TableName = "vaccineType"
                            .Tables(18).TableName = "docType"
                            .Tables(19).TableName = "educationLevel"
                            .Tables(20).TableName = "licenseProfession"
                            .Tables(21).TableName = "cdaTrainingType"
                            .Tables(22).TableName = "nciiTitle"
                            .Tables(23).TableName = "trainingInstitution"
                            .Tables(24).TableName = "assessmentCenter"
                            .Tables(25).TableName = "complianceTraining"
                            .Tables(26).TableName = "affiliationName"
                            .Tables(27).TableName = "sources"
                            .Tables(28).TableName = "medicalResultType"
                            .Tables(29).TableName = "employmentType"
                            .Tables(30).TableName = "rnrNumbers"
                            .Tables(31).TableName = "suffix"
                            .Tables(32).TableName = "kinSuffix"


                            '.Tables(26).TableName = "municipality"

                            '.Tables(11).TableName = "birthProvince"
                            '.Tables(8).TableName = "educationLevel"
                            '.Tables(9).TableName = "disability"
                            '.Tables(10).TableName = "docType"

                            '.Tables(7).TableName = "civilStatus"
                        End With

                        Return Me.Ok(_dataSet)

                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberProvinces")>
    <Route("member-provinces")>
    <HttpGet>
    Public Function GetMemberProvinces() As IHttpActionResult
        Dim _filter As String = ""
        Dim _dataSource As String = "dbo.DbsProvince"
        Dim _fields As String = "ProvinceId, ProvinceName, RegionId"
        Dim _sort As String = "ProvinceName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberMunicipalities")>
    <Route("member-municipalities/{provinceId}")>
    <HttpGet>
    Public Function GetMemberMunicipalities(provinceId As Integer) As IHttpActionResult

        Dim _filter As String = "ProvinceId=" + provinceId.ToString
        Dim _dataSource As String = "dbo.QDbsMunicipality"
        Dim _fields As String = "MunicipalityId, MunicipalityName, ProvinceId, RegionId"
        Dim _sort As String = "MunicipalityName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberBarangays")>
    <Route("member-barangays/{municipalityId}")>
    <HttpGet>
    Public Function GetMemberBarangays(municipalityId As Integer) As IHttpActionResult

        Dim _filter As String = "MunicipalityId=" + municipalityId.ToString
        Dim _dataSource As String = "dbo.QDbsBarangay"
        Dim _fields As String = "BarangayId, BarangayName, MunicipalityId, PostalCode"
        Dim _sort As String = "BarangayName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberPostals")>
    <Route("member-postals/{barangayId}")>
    <HttpGet>
    Public Function GetMemberPostals(barangayId As Integer) As IHttpActionResult

        Dim _filter As String = "BarangayId=" + barangayId.ToString
        Dim _dataSource As String = "dbo.QDbsBarangay"
        Dim _fields As String = "BarangayId, BarangayName, MunicipalityId, PostalCode"
        Dim _sort As String = "BarangayName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberBirthProvinces")>
    <Route("member-birth-provinces")>
    <HttpGet>
    Public Function GetMemberBirthProvinces() As IHttpActionResult
        Dim _filter As String = ""
        Dim _dataSource As String = "dbo.DbsProvince"
        Dim _fields As String = "ProvinceId as BirthProvinceId, ProvinceName, RegionId"
        Dim _sort As String = "ProvinceName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberBirthMunicipalities")>
    <Route("member-birth-municipalities/{provinceId}")>
    <HttpGet>
    Public Function GetMemberBirthMunicipalities(provinceId As Integer) As IHttpActionResult

        Dim _filter As String = "ProvinceId=" + provinceId.ToString
        Dim _dataSource As String = "dbo.QDbsMunicipality"
        Dim _fields As String = "MunicipalityId as BirthMunicipalityId, MunicipalityName, ProvinceId as BirthProvinceId, RegionId as BirthRegionId"
        Dim _sort As String = "MunicipalityName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberAlternateProvinces")>
    <Route("member-alternate-provinces")>
    <HttpGet>
    Public Function GetMemberAlternateProvinces() As IHttpActionResult
        Dim _filter As String = ""
        Dim _dataSource As String = "dbo.DbsProvince"
        Dim _fields As String = "ProvinceId as AlternateProvinceId, ProvinceName, RegionId as AlternateRegionId"
        Dim _sort As String = "ProvinceName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberAlternateMunicipalities")>
    <Route("member-alternate-municipalities/{provinceId}")>
    <HttpGet>
    Public Function GetMemberAlternateMunicipalities(provinceId As Integer) As IHttpActionResult

        Dim _filter As String = "ProvinceId=" + provinceId.ToString
        Dim _dataSource As String = "dbo.QDbsMunicipality"
        Dim _fields As String = "MunicipalityId as AlternateMunicipalityId, MunicipalityName, ProvinceId as AlternateProvinceId, RegionId as AlternateRegionId"
        Dim _sort As String = "MunicipalityName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberAlternateBarangays")>
    <Route("member-alternate-barangays/{municipalityId}")>
    <HttpGet>
    Public Function GetMemberAlternateBarangays(municipalityId As Integer) As IHttpActionResult

        Dim _filter As String = "MunicipalityId=" + municipalityId.ToString
        Dim _dataSource As String = "dbo.QDbsBarangay"
        Dim _fields As String = "BarangayId as AlternateBarangayId, BarangayName, MunicipalityId as AlternateMunicipalityId, PostalCode"
        Dim _sort As String = "BarangayName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMemberAlternatePostals")>
    <Route("member-alternate-postals/{barangayId}")>
    <HttpGet>
    Public Function GetMemberAlternatePostals(barangayId As Integer) As IHttpActionResult

        Dim _filter As String = "BarangayId=" + barangayId.ToString
        Dim _dataSource As String = "dbo.QDbsBarangay"
        Dim _fields As String = "BarangayId, BarangayName, MunicipalityId, PostalCode"
        Dim _sort As String = "BarangayName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetMember")>
    <Route("members/{memberId}")>
    <HttpGet>
    Public Function GetMember(memberId As Integer) As IHttpActionResult

        If memberId <= 0 Then
            Throw New ArgumentException("Member ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.HRS0010")
                With _direct
                    .AddParameter("MemberId", memberId)

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "member"
                            .Tables(1).TableName = "province"
                            .Tables(2).TableName = "municipality"
                            .Tables(3).TableName = "barangay"
                            .Tables(4).TableName = "birthProvince"
                            .Tables(5).TableName = "birthMunicipality"
                            .Tables(6).TableName = "languages"
                            .Tables(7).TableName = "disabilities"
                            .Tables(8).TableName = "recordings"
                            .Tables(9).TableName = "vaccines"
                            .Tables(10).TableName = "skillSets"
                            .Tables(11).TableName = "kins"
                            .Tables(12).TableName = "docs"
                            .Tables(13).TableName = "educations"
                            .Tables(14).TableName = "licenses"
                            .Tables(15).TableName = "cdaTrainings"
                            .Tables(16).TableName = "nciis"
                            .Tables(17).TableName = "compliances"
                            .Tables(18).TableName = "works"
                            .Tables(19).TableName = "affiliations"
                            .Tables(20).TableName = "medicals"

                            .Tables(21).TableName = "alternateProvince"
                            .Tables(22).TableName = "alternateMunicipality"
                            .Tables(23).TableName = "alternateBarangay"
                            .Tables(24).TableName = "pooling"
                            '.Tables(1).TableName = "educations"
                            '.Tables(2).TableName = "kins"
                            '.Tables(3).TableName = "works"
                            '.Tables(4).TableName = "disabilities"
                            '.Tables(5).TableName = "docTypes"
                            '.Tables(6).TableName = "certificates"
                            '.Tables(7).TableName = "eligibilities"
                            '.Tables(8).TableName = "licenses"
                            '.Tables(9).TableName = "skillSets"
                            '.Tables(10).TableName = "soloParent"
                            '.Tables(11).TableName = "licenseProfessions"
                            '.Tables(12).TableName = "nciiQualificationTitles"
                            '.Tables(13).TableName = "complianceTrainings"
                            '.Tables(14).TableName = "cdaTrainings"
                        End With

                        Return Me.Ok(_dataSet)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetHrsMemberLog")>
    <Route("member/{logId}/{memberId}/log")>
    <HttpGet>
    Public Function GetHrsMemberLog(logId As Integer, memberId As Integer) As IHttpActionResult

        If memberId <= 0 Then
            Throw New ArgumentException("Member ID is required.")
        End If

        Dim _member As String = "web.HRS0010_Log"

        Select Case logId
            Case LogTable.Member
                _member = "web.HRS0010_MemberLog"
            Case LogTable.Language
                _member = "web.HRS0010_LanguageLog"
            Case LogTable.RnrRecording
                _member = "web.HRS0010_RnrRecordingLog"
            Case LogTable.Vaccine
                _member = "web.HRS0010_VaccineLog"
            Case LogTable.SkillSet
                _member = "web.HRS0010_SkillSetLog"
            Case LogTable.Kin
                _member = "web.HRS0010_KinLog"
            Case LogTable.DocType
                _member = "web.HRS0010_DocTypeLog"
            Case LogTable.Education
                _member = "web.HRS0010_EducationLog"
            Case LogTable.LicenseProfession
                _member = "web.HRS0010_LicenseProfessionLog"
            Case LogTable.CDATraining
                _member = "web.HRS0010_CDATrainingLog"
            Case LogTable.NCIIQualificationTitle
                _member = "web.HRS0010_NCIIQualificationTitleLog"
            Case LogTable.ComplianceTraining
                _member = "web.HRS0010_ComplianceTrainingLog"
            Case LogTable.Work
                _member = "web.HRS0010_WorkLog"
            Case LogTable.Affiliation
                _member = "web.HRS0010_AffiliationLog"
            Case LogTable.MedicalResultType
                _member = "web.HRS0010_MedicalResultTypeLog"
        End Select

        Try
            Using _direct As New SqlDirect(_member)
                With _direct
                    .AddParameter("memberId", memberId)

                    ' Allow DateTime
                    Dim _settings As New JsonSerializerSettings
                    With _settings
                        .ContractResolver = New CamelCasePropertyNamesContractResolver
                        .DateFormatString = "yyyy-MM-ddTHH:mm"
                    End With

                    Using _dataTable As DataTable = _direct.ExecuteDataTable()
                        'Return Me.Ok(_dataTable)
                        Return Json(_dataTable, _settings)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateMember")>
    <Route("members/{currentUserId}")>
    <HttpPost>
    Public Function CreateMember(currentUserId As Integer, <FromBody> member As MemberBody) As IHttpActionResult

        If member.MemberId <> -1 Then
            Throw New ArgumentException("Member ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _memberId As Integer = SysLib.GetNextSequence("MemberId")

            member.MemberId = _memberId

            '
            ' Load proposed values from payload
            '
            Dim _hrsMember As New HrsMember

            Dim _hrsMemberLanguageList As New HrsMemberLanguageList
            Dim _hrsMemberDisabilityList As New HrsMemberDisabilityList
            Dim _hrsMemberRnrRecordingList As New HrsMemberRnrRecordingList
            Dim _hrsMemberVaccineList As New HrsMemberVaccineList
            Dim _hrsMemberSkillSetList As New HrsMemberSkillSetList
            Dim _hrsMemberKinList As New HrsMemberKinList
            Dim _hrsMemberDocTypeList As New HrsMemberDocTypeList
            Dim _hrsMemberEducationList As New HrsMemberEducationList
            Dim _hrsMemberLicenseProfessionList As New HrsMemberLicenseProfessionList
            Dim _hrsMemberCDATrainingList As New HrsMemberCDATrainingList
            Dim _hrsMemberNCIIQualificationTitleList As New HrsMemberNCIIQualificationTitleList
            Dim _hrsMemberComplianceTrainingList As New HrsMemberComplianceTrainingList
            Dim _hrsMemberWorkList As New HrsMemberWorkList
            Dim _hrsMemberAffiliationList As New HrsMemberAffiliationList
            Dim _hrsMemberMedicalResultTypeList As New HrsMemberMedicalResultTypeList
            'Dim _hrsMemberWorkList As New HrsMemberWorkList

            'Dim _hrsMemberDisabilityList As New HrsMemberDisabilityList
            'Dim _hrsMemberDocTypeList As New HrsMemberDocTypeList

            'Dim _hrsMemberCertificateList As New HrsMemberCertificateList
            'Dim _hrsMemberEligibilityList As New HrsMemberEligibilityList
            'Dim _hrsMemberLicenseList As New HrsMemberLicenseList
            'Dim _hrsMemberSkillSetList As New HrsMemberSkillSetList

            Me.LoadHrsMember(member, _hrsMember)

            ' HrsMemberLanguage

            For Each _language As HrsMemberLanguage In member.Languages
                _language.MemberId = _memberId
                _hrsMemberLanguageList.Add(_language)
            Next

            ' HrsMemberDisability

            For Each _disability As HrsMemberDisability In member.Disabilities
                _disability.MemberId = _memberId
                _hrsMemberDisabilityList.Add(_disability)
            Next

            ' HrsMemberRnrRecording

            For Each _rnrRecording As HrsMemberRnrRecording In member.RnrRecordings
                _rnrRecording.MemberId = _memberId
                _hrsMemberRnrRecordingList.Add(_rnrRecording)
            Next

            ' HrsMemberVaccine

            For Each _vaccine As HrsMemberVaccine In member.Vaccines
                _vaccine.MemberId = _memberId
                _hrsMemberVaccineList.Add(_vaccine)
            Next

            ' HrsMemberSkillSet
            For Each _skillSet As HrsMemberSkillSet In member.SkillSets
                _skillSet.MemberId = _memberId
                _hrsMemberSkillSetList.Add(_skillSet)
            Next

            ' HrsMemberKin
            For Each _kin As HrsMemberKin In member.Kins
                _kin.MemberId = _memberId
                _hrsMemberKinList.Add(_kin)
            Next

            ' HrsMemberDoc
            For Each _doc As HrsMemberDocType In member.Docs
                _doc.MemberId = _memberId
                _hrsMemberDocTypeList.Add(_doc)
            Next

            ' HrsMemberEducation
            For Each _education As HrsMemberEducation In member.Educations
                _education.MemberId = _memberId
                _hrsMemberEducationList.Add(_education)
            Next

            ' HrsMemberLicenseProfession
            For Each _licenses As HrsMemberLicenseProfession In member.Licenses
                _licenses.MemberId = _memberId
                _hrsMemberLicenseProfessionList.Add(_licenses)
            Next

            ' HrsMemberCDATraining
            For Each _cdaTrainings As HrsMemberCDATraining In member.CDATrainings
                _cdaTrainings.MemberId = _memberId
                _hrsMemberCDATrainingList.Add(_cdaTrainings)
            Next

            ' HrsMemberNCIIQualificationTitle
            For Each _nCIIQualificationTitles As HrsMemberNCIIQualificationTitle In member.NCIIs
                _nCIIQualificationTitles.MemberId = _memberId
                _hrsMemberNCIIQualificationTitleList.Add(_nCIIQualificationTitles)
            Next

            ' HrsMemberComplianceTraining
            For Each _complianceTrainings As HrsMemberComplianceTraining In member.Compliances
                _complianceTrainings.MemberId = _memberId
                _hrsMemberComplianceTrainingList.Add(_complianceTrainings)
            Next

            ' HrsMemberWork
            For Each _works As HrsMemberWork In member.Works
                _works.MemberId = _memberId
                _hrsMemberWorkList.Add(_works)
            Next

            ' HrsMemberAffiliation
            For Each _affiliations As HrsMemberAffiliation In member.Affiliations
                _affiliations.MemberId = _memberId
                _hrsMemberAffiliationList.Add(_affiliations)
            Next

            ' HrsMemberMedicalResultType
            For Each _medicals As HrsMemberMedicalResultType In member.Medicals
                _medicals.MemberId = _memberId
                _hrsMemberMedicalResultTypeList.Add(_medicals)
            Next

            '' HrsMemberWork

            'For Each _work As HrsMemberWork In member.Works
            '    _work.MemberId = _memberId
            '    _hrsMemberWorkList.Add(_work)
            'Next


            '' HrsMemberDisability
            'For Each _disability As HrsMemberDisability In member.Disabilities
            '    _disability.MemberId = _memberId
            '    _hrsMemberDisabilityList.Add(_disability)
            'Next

            '' HrsMemberDocType
            'For Each _docType As HrsMemberDocType In member.DocTypes
            '    _docType.MemberId = _memberId
            '    _hrsMemberDocTypeList.Add(_docType)
            'Next

            '' HrsMemberCertificate

            'For Each _certificate As HrsMemberCertificate In member.Certificates
            '    _certificate.MemberId = _memberId
            '    _hrsMemberCertificateList.Add(_certificate)
            'Next

            '' HrsMemberEligibility

            'For Each _eligibility As HrsMemberEligibility In member.Eligibilities
            '    _eligibility.MemberId = _memberId
            '    _hrsMemberEligibilityList.Add(_eligibility)
            'Next

            '' HrsMemberLicense

            'For Each _license As HrsMemberLicense In member.Licenses
            '    _license.MemberId = _memberId
            '    _hrsMemberLicenseList.Add(_license)
            'Next

            '' HrsMemberSkillSet

            'For Each _skillSet As HrsMemberSkillSet In member.SkillSets
            '    _skillSet.MemberId = _memberId
            '    _hrsMemberSkillSetList.Add(_skillSet)
            'Next

            '
            ' Log addition, save to DB
            '

            Dim _logKeyList As New LogKeyList

            Dim _id As Integer
            'mod.HrsMember
            Dim _hrsMemberLogDetailList As New SysLogDetailList
            Dim _hrsMemberLanguageLogDetailList As New SysLogDetailList
            Dim _hrsMemberDisabilityLogDetailList As New SysLogDetailList
            Dim _hrsMemberRnrRecordingLogDetailList As New SysLogDetailList
            Dim _hrsMemberVaccineLogDetailList As New SysLogDetailList
            Dim _hrsMemberSkillSetLogDetailList As New SysLogDetailList
            Dim _hrsMemberKinLogDetailList As New SysLogDetailList
            Dim _hrsMemberDocTypeLogDetailList As New SysLogDetailList
            Dim _hrsMemberEducationLogDetailList As New SysLogDetailList
            Dim _hrsMemberLicenseProfessionLogDetailList As New SysLogDetailList
            Dim _hrsMemberCDATrainingLogDetailList As New SysLogDetailList
            Dim _hrsMemberNCIIQualificationTitleLogDetailList As New SysLogDetailList
            Dim _hrsMemberComplianceTrainingLogDetailList As New SysLogDetailList
            Dim _hrsMemberWorkLogDetailList As New SysLogDetailList
            Dim _hrsMemberAffiliationLogDetailList As New SysLogDetailList
            Dim _hrsMemberMedicalResultTypeLogDetailList As New SysLogDetailList

            ''mod.HrsMemberEducation
            'Dim _hrsMemberEducationLogDetailList As New SysLogDetailList
            ''mod.HrsMemberWork
            'Dim _hrsMemberWorkLogDetailList As New SysLogDetailList
            ''mod.HrsMemberKin
            ''mod.HrsMemberDisability
            'Dim _hrsMemberDisabilityLogDetailList As New SysLogDetailList
            ''mod.HrsMemberCertificate
            'Dim _hrsMemberCertificateLogDetailList As New SysLogDetailList
            ''mod.HrsMemberEligibility
            'Dim _hrsMemberEligibilityLogDetailList As New SysLogDetailList
            ''mod.HrsMemberLicense
            'Dim _hrsMemberLicenseLogDetailList As New SysLogDetailList
            ''mod.HrsMemberLicense
            'Dim _hrsMemberSkillSetLogDetailList As New SysLogDetailList
            Try


                With _hrsMember

                    .BirthRegionId = GetRegion(member.BirthProvinceId.ToString)
                    .RegionId = GetRegion(member.ProvinceId.ToString)

                    If Not String.IsNullOrEmpty(.AlternateProvinceId) Then
                        .AlternateRegionId = GetRegion(.AlternateProvinceId)
                    End If

                    .MemberStatusId = MemberStatus.Active

                    '.MemberStatusId = GetMemberStatus(member.EmploymentStatusId)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberEmployeeId, String.Empty, .MemberEmployeeId)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberLastName, String.Empty, .MemberLastName)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberFirstName, String.Empty, .MemberFirstName)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberMiddleName, String.Empty, .MemberMiddleName)
                    'AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberSuffix, String.Empty, .MemberSuffix)

                    Dim _memberSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .MemberSuffixId).MemberSuffixName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberSuffixId, String.Empty, .MemberSuffixId.ToString, .MemberSuffixId.ToString + "=" + _memberSuffixName)

                    Dim _memberTypeName As String = EasSession.HrsMemberType.Rows.Find(Function(m) m.MemberTypeId = .MemberTypeId).MemberTypeName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberTypeId, String.Empty, .MemberTypeId.ToString, .MemberTypeId.ToString + "=" + _memberTypeName)

                    Dim _memberTypeQualificationName As String = EasSession.HrsMemberTypeQualification.Rows.Find(Function(m) m.TypeQualificationDetailId = .TypeQualificationDetailId).TypeQualificationName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.TypeQualificationDetailId, String.Empty, .TypeQualificationDetailId.ToString, .TypeQualificationDetailId.ToString + "=" + _memberTypeQualificationName)


                    Dim _memberStatusName As String = EasSession.HrsMemberStatus.Rows.Find(Function(m) m.MemberStatusId = .MemberStatusId).MemberStatusName

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberStatusId, String.Empty, .MemberStatusId.ToString, .MemberStatusId.ToString + "=" + _memberStatusName)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthDate, String.Empty, .BirthDate.ToDisplayFormat)

                    'AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthPlace, String.Empty, .BirthPlace)

                    Dim _birthRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .BirthRegionId).RegionName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthRegionId, String.Empty, .BirthRegionId.ToString, .BirthRegionId.ToString + "=" + _birthRegionName)

                    Dim _birthProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .BirthProvinceId).ProvinceName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthProvinceId, String.Empty, .BirthProvinceId.ToString, .BirthProvinceId.ToString + "=" + _birthProvinceName)

                    Dim _birthMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .BirthMunicipalityId).MunicipalityName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthMunicipalityId, String.Empty, .BirthMunicipalityId.ToString, .BirthMunicipalityId.ToString + "=" + _birthMunicipalityName)


                    Dim _sexName As String = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = .SexId).SexName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.SexId, String.Empty, .SexId.ToString, .SexId.ToString + "=" + _sexName)

                    If Not .BloodTypeId.ToString = "0" Then
                        Dim _bloodTypeName As String = EasSession.DbsBloodType.Rows.Find(Function(m) m.BloodTypeId = .BloodTypeId).BloodTypeName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BloodTypeId, String.Empty, .BloodTypeId.ToString, .BloodTypeId.ToString + "=" + _bloodTypeName)

                    End If

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Height, String.Empty, .Height)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Weight, String.Empty, .Weight)

                    Dim _civilStatusName As String = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = .CivilStatusId).CivilStatusName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CivilStatusId, String.Empty, .CivilStatusId.ToString, .CivilStatusId.ToString + "=" + _civilStatusName)

                    Dim _religionName As String = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = .ReligionId).ReligionName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ReligionId, String.Empty, .ReligionId.ToString, .ReligionId.ToString + "=" + _religionName)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Address1, String.Empty, .Address1)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Address2, String.Empty, .Address2)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PostalCode, String.Empty, .PostalCode)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PhoneNumber, String.Empty, .PhoneNumber)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MobileNumber, String.Empty, .MobileNumber)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Email, String.Empty, .Email)

                    Dim _regionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .RegionId).RegionName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RegionId, String.Empty, .RegionId.ToString, .RegionId.ToString + "=" + _regionName)

                    Dim _provinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .ProvinceId).ProvinceName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ProvinceId, String.Empty, .ProvinceId.ToString, .ProvinceId.ToString + "=" + _provinceName)

                    Dim _municipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _municipalityName)

                    Dim _barangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .BarangayId).BarangayName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BarangayId, String.Empty, .BarangayId.ToString, .BarangayId.ToString + "=" + _barangayName)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Facebook, String.Empty, .Facebook)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Instagram, String.Empty, .Instagram)


                    If Not String.IsNullOrEmpty(.AlternateProvinceId) Then
                        Dim _alternateRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .AlternateRegionId).RegionName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateRegionId, String.Empty, .AlternateRegionId.ToString, .AlternateRegionId.ToString + "=" + _alternateRegionName)

                        Dim _AlternateProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .AlternateProvinceId).ProvinceName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateProvinceId, String.Empty, .AlternateProvinceId.ToString, .AlternateProvinceId.ToString + "=" + _AlternateProvinceName)

                    End If

                    If Not String.IsNullOrEmpty(.AlternateMunicipalityId) Then
                        Dim _alternateMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .AlternateMunicipalityId).MunicipalityName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateMunicipalityId, String.Empty, .AlternateMunicipalityId.ToString, .AlternateMunicipalityId.ToString + "=" + _alternateMunicipalityName)

                    End If

                    If Not String.IsNullOrEmpty(.AlternateBarangayId) Then
                        Dim _alternateBarangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .AlternateBarangayId).BarangayName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateBarangayId, String.Empty, .AlternateBarangayId.ToString, .AlternateBarangayId.ToString + "=" + _alternateBarangayName)
                    End If

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternatePostalCode, String.Empty, .AlternatePostalCode)



                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateAddress1, String.Empty, .AlternateAddress1)
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateAddress2, String.Empty, .AlternateAddress2)

                    Dim _cdaMemberTypeName As String = EasSession.DbsCDAMemberType.Rows.Find(Function(m) m.CDAMemberTypeId = .CDAMemberTypeId).CDAMemberTypeName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CDAMemberTypeId, String.Empty, .CDAMemberTypeId.ToString, .CDAMemberTypeId.ToString + "=" + _cdaMemberTypeName)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CDAMemberTypeAmount, String.Empty, .CDAMemberTypeAmount.ToString)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CDAMemberTypeAmount, String.Empty, .CDAMemberTypeAmount.ToString)

                    'Dim _employmentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                    'AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.EmploymentStatusId, String.Empty, .EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _employmentStatusName)

                    'If Not .EmploymentTypeId.ToString = "0" Then
                    '   Dim _employmentTypeName As String = EasSession.DbsEmploymentType.Rows.Find(Function(m) m.EmploymentTypeId = .EmploymentTypeId).EmploymentTypeName
                    '   AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.EmploymentTypeId, String.Empty, .EmploymentTypeId.ToString, .EmploymentTypeId.ToString + "=" + _employmentTypeName)
                    'End If

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AbroadFlag, String.Empty, .AbroadFlag.ToString)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RelocateFlag, String.Empty, .RelocateFlag.ToString)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.WeekendHolidayFlag, String.Empty, .WeekendHolidayFlag.ToString)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ExpectedSalary, String.Empty, .ExpectedSalary.ToString)

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.GCashNumber, String.Empty, .GCashNumber.ToString)

                    'Try
                    If Not .BankId.ToString = "0" Then
                        Dim _bankName As String = EasSession.DbsBank.Rows.Find(Function(m) m.BankId = .BankId).BankName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BankId, String.Empty, .BankId.ToString, .BankId.ToString + "=" + _bankName)
                    End If

                    If Not .RecruiterId.ToString = "0" Then
                        Dim _recruiterName As String = EasSession.HrsRecruiter.Rows.Find(Function(m) m.RecruiterId = .RecruiterId).RecruiterName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RecruiterId, String.Empty, .RecruiterId.ToString, .RecruiterId.ToString + "=" + _recruiterName)
                    End If

                    'If Not .ApplicationSourceId.ToString = "0" Then
                    '   Dim _applicationSourceName As String = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = .ApplicationSourceId).ApplicationSourceName
                    '   AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ApplicationSourceId, String.Empty, .ApplicationSourceId.ToString, .ApplicationSourceId.ToString + "=" + _applicationSourceName)
                    'End If

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BankAccountNumber, String.Empty, .BankAccountNumber.ToString)


                    Try
                        UploadMemberFile(_memberId, .PhotoFileName, .PhotoGUID)
                        'RemoveMemberFile(_memberId, .PhotoFileName, .PhotoGUID)

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PhotoFileName, .PhotoFileName, .PhotoFileName)
                        _hrsMember.ImageExtension = Path.GetExtension(_hrsMember.PhotoFileName).ToLowerInvariant

                    Catch ex As Exception
                    End Try



                End With

            Catch ex As Exception

            End Try

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.InsertHrsMember(_hrsMember)

                If _hrsMemberLogDetailList.Count > 0 Then
                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _hrsMember.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberLog", _logKeyList, LogActionId.Add, currentUserId)
                    AppLib.AssignLogHeaderId(_id, _hrsMemberLogDetailList)
                    AppLib.CreateLogDetails(_hrsMemberLogDetailList, "HrsMemberLogDetail")

                End If

#Region "HrsMemberLanguage"

                If _hrsMemberLanguageList.Count > 0 Then
                    Me.InsertHrsMemberLanguages(_hrsMemberLanguageList)
                End If

                ' HrsMemberLanguage
                For Each _new As HrsMemberLanguage In _hrsMemberLanguageList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberLanguageLog", _logKeyList, LogActionId.Add, currentUserId)
                    _hrsMemberLanguageLogDetailList.Clear()

                    With _new
                        Dim _languageName As String = EasSession.DbsLanguage.Rows.Find(Function(m) m.LanguageId = .LanguageId).LanguageName
                        AppLib.AddLogDetail(_hrsMemberLanguageLogDetailList, _id, LogColumnId.LanguageId, String.Empty, .LanguageId.ToString, .LanguageId.ToString + "=" + _languageName)
                        AppLib.CreateLogDetails(_hrsMemberLanguageLogDetailList, "HrsMemberLanguageLogDetail")
                    End With
                Next

#End Region

#Region "HrsMemberDisability"

                If _hrsMemberDisabilityList.Count > 0 Then
                    Me.InsertHrsMemberDisabilities(_hrsMemberDisabilityList)
                End If

                ' HrsMemberDisability
                For Each _new As HrsMemberDisability In _hrsMemberDisabilityList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberDisabilityLog", _logKeyList, LogActionId.Add, currentUserId)
                    _hrsMemberDisabilityLogDetailList.Clear()

                    With _new
                        Dim _disabilityName As String = EasSession.DbsDisability.Rows.Find(Function(m) m.DisabilityId = .DisabilityId).DisabilityName
                        AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.DisabilityId, String.Empty, .DisabilityId.ToString, .DisabilityId.ToString + "=" + _disabilityName)
                        AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.Remarks, String.Empty, .Remarks)
                        AppLib.CreateLogDetails(_hrsMemberDisabilityLogDetailList, "HrsMemberDisabilityLogDetail")
                    End With
                Next

#End Region

#Region "HrsMemberRnrRecording"

                If _hrsMemberRnrRecordingList.Count > 0 Then
                    Me.InsertHrsMemberRnrRecordings(_hrsMemberRnrRecordingList)
                End If

                For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberRnrRecordingLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberRnrRecordingLogDetailList.Clear()

                    With _new
                        Dim _rnrRecordingName As String = EasSession.DbsRnrRecording.Rows.Find(Function(m) m.RnrRecordingId = .RnrRecordingId).RnrRecordingName
                        AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrRecordingId, String.Empty, .RnrRecordingId.ToString, .RnrRecordingId.ToString + "=" + _rnrRecordingName)
                        'AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumber, "", .RnrNumber.ToString)

                        Dim _rnrNumber As String = EasSession.DbsRnrNumber.Rows.Find(Function(m) m.RnrNumberId = .RnrNumberId).RnrNumber.ToString
                        AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumberId, String.Empty, .RnrNumberId.ToString, .RnrNumberId.ToString + "=" + _rnrNumber)

                        AppLib.CreateLogDetails(_hrsMemberRnrRecordingLogDetailList, "HrsMemberRnrRecordingLogDetail")
                    End With
                Next

#End Region

#Region "HrsMemberVaccine"

                If _hrsMemberVaccineList.Count > 0 Then
                    Me.InsertHrsMemberVaccines(_hrsMemberVaccineList)
                End If

                For Each _new As HrsMemberVaccine In _hrsMemberVaccineList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberVaccineLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberVaccineLogDetailList.Clear()

                    With _new
                        Dim _vaccineTypeName As String = EasSession.DbsVaccineType.Rows.Find(Function(m) m.VaccineTypeId = .VaccineTypeId).VaccineTypeName
                        AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineTypeId, String.Empty, .VaccineTypeId.ToString, .VaccineTypeId.ToString + "=" + _vaccineTypeName)
                        AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineName, String.Empty, .VaccineName)
                        AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineDate, String.Empty, .VaccineDate.ToDisplayFormat.ToString)
                        AppLib.CreateLogDetails(_hrsMemberVaccineLogDetailList, "HrsMemberVaccineLogDetail")
                    End With
                Next

#End Region

#Region "HrsMemberSkillSet"

                If _hrsMemberSkillSetList.Count > 0 Then
                    Me.InsertHrsMemberSkillSets(_hrsMemberSkillSetList)
                End If

                For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberSkillSetLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberSkillSetLogDetailList.Clear()

                    With _new
                        'AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, "", .SkillDetailId.ToString)

                        Dim _skillSetName As String = EasSession.DbsSkillSetDetailLog.Rows.Find(Function(m) m.SkillDetailId = .SkillDetailId).SkillDetailName
                        AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, String.Empty, .SkillDetailId.ToString, .SkillDetailId.ToString + "=" + _skillSetName)


                        AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.Remarks, "", .Remarks)
                        AppLib.CreateLogDetails(_hrsMemberSkillSetLogDetailList, "HrsMemberSkillSetLogDetail")

                    End With

                Next



#End Region

#Region "HrsMemberKin"
                'Try

                If _hrsMemberKinList.Count > 0 Then
                    Me.InsertHrsMemberKins(_hrsMemberKinList)
                End If

                For Each _new As HrsMemberKin In _hrsMemberKinList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberKinLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberKinLogDetailList.Clear()

                    With _new
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinLastName, String.Empty, .KinLastName)

                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFirstName, String.Empty, .KinFirstName)
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinMiddleName, String.Empty, .KinMiddleName)
                        Try
                            Dim _suffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .KinSuffixId).MemberSuffixName
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinSuffixId, String.Empty, .KinSuffixId.ToString, .KinSuffixId.ToString + "=" + _suffixName)
                        Catch ex As Exception

                        End Try


                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.EmergencyContactFlag, String.Empty, .EmergencyContactFlag.ToString)


                        Dim _relationName As String = EasSession.DbsRelation.Rows.Find(Function(m) m.RelationId = .RelationId).RelationName
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.RelationId, String.Empty, .RelationId.ToString, .RelationId.ToString + "=" + _relationName)
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinPhoneNumber, String.Empty, .KinPhoneNumber)
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinOccupation, String.Empty, .KinOccupation)
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinEmail, String.Empty, .KinEmail)
                        AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFileName, String.Empty, .KinFileName)

                        AppLib.CreateLogDetails(_hrsMemberKinLogDetailList, "HrsMemberKinLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberDoc"

                If _hrsMemberDocTypeList.Count > 0 Then
                    Me.InsertHrsMemberDocTypes(_hrsMemberDocTypeList)
                End If

                For Each _new As HrsMemberDocType In _hrsMemberDocTypeList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberDocTypeLogDetailList.Clear()

                    With _new
                        Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                        AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)

                        AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, String.Empty, .DocTypeReference)
                        AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, String.Empty, .DocTypeFileName)

                        AppLib.CreateLogDetails(_hrsMemberDocTypeLogDetailList, "HrsMemberDocTypeLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberEducation"
                'Try

                If _hrsMemberEducationList.Count > 0 Then
                    Me.InsertHrsMemberEducations(_hrsMemberEducationList)
                End If

                For Each _new As HrsMemberEducation In _hrsMemberEducationList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberEducationLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberEducationLogDetailList.Clear()

                    With _new
                        Dim _educationLevelName As String = EasSession.DbsEducationLevel.Rows.Find(Function(m) m.EducationLevelId = .EducationLevelId).EducationLevelName
                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, String.Empty, .EducationLevelId.ToString, .EducationLevelId.ToString + "=" + _educationLevelName)

                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.SchoolName, String.Empty, .SchoolName)
                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.CourseName, String.Empty, .CourseName)
                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduStartYear, String.Empty, .EduStartYear.ToString)
                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduEndYear, String.Empty, .EduEndYear.ToString)
                        AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduFileName, String.Empty, .EduFileName)

                        AppLib.CreateLogDetails(_hrsMemberEducationLogDetailList, "HrsMemberEducationLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberLicenseProfession"
                'Try

                If _hrsMemberLicenseProfessionList.Count > 0 Then
                    Me.InsertHrsMemberLicenseProfessions(_hrsMemberLicenseProfessionList)
                End If

                For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberLicenseProfessionLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberLicenseProfessionLogDetailList.Clear()

                    With _new
                        Dim _licenseProfessionName As String = EasSession.DbsLicenseProfession.Rows.Find(Function(m) m.LicenseProfessionId = .LicenseProfessionId).LicenseProfessionName
                        AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseProfessionId, String.Empty, .LicenseProfessionId.ToString, .LicenseProfessionId.ToString + "=" + _licenseProfessionName)

                        AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.PRCIdNo, String.Empty, .PRCIdNo)
                        AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseFileName, String.Empty, .LicenseFileName)

                        AppLib.CreateLogDetails(_hrsMemberLicenseProfessionLogDetailList, "HrsMemberLicenseProfessionLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberCDATraining"

                If _hrsMemberCDATrainingList.Count > 0 Then
                    Me.InsertHrsMemberCDATrainings(_hrsMemberCDATrainingList)
                End If

                For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberCDATrainingLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberCDATrainingLogDetailList.Clear()

                    With _new
                        Dim _cDATrainingName As String = EasSession.DbsCDATraining.Rows.Find(Function(m) m.CDATrainingId = .CDATrainingId).CDATrainingName
                        AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDATrainingId, String.Empty, .CDATrainingId.ToString, .CDATrainingId.ToString + "=" + _cDATrainingName)

                        AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                        AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.TrainingDate, String.Empty, .TrainingDate.ToDisplayFormat)
                        AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDAFileName, String.Empty, .CDAFileName)

                        AppLib.CreateLogDetails(_hrsMemberCDATrainingLogDetailList, "HrsMemberCDATrainingLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberNCIIQualificationTitle"

                If _hrsMemberNCIIQualificationTitleList.Count > 0 Then
                    Me.InsertHrsMemberNCIIQualificationTitles(_hrsMemberNCIIQualificationTitleList)
                End If

                For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberNCIIQualificationTitleLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberNCIIQualificationTitleLogDetailList.Clear()

                    With _new
                        Dim _nCIIQualificationTitleName As String = EasSession.DbsNCIIQualificationTitle.Rows.Find(Function(m) m.NCIIQualificationTitleId = .NCIIQualificationTitleId).NCIIQualificationTitleName
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIQualificationTitleId, String.Empty, .NCIIQualificationTitleId.ToString, .NCIIQualificationTitleId.ToString + "=" + _nCIIQualificationTitleName)
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.IssuanceDate, String.Empty, .IssuanceDate.ToDisplayFormat)
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.ValidityDate, String.Empty, .ValidityDate.ToDisplayFormat)
                        Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)
                        Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                        AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIFileName, String.Empty, .NCIIFileName)
                        AppLib.CreateLogDetails(_hrsMemberNCIIQualificationTitleLogDetailList, "HrsMemberNCIIQualificationTitleLogDetail")

                    End With
                Next

#End Region

#Region "HrsMemberComplianceTraining"

                If _hrsMemberComplianceTrainingList.Count > 0 Then
                    Me.InsertHrsMemberComplianceTrainings(_hrsMemberComplianceTrainingList)
                End If

                For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberComplianceTrainingLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberComplianceTrainingLogDetailList.Clear()

                    With _new
                        Dim _complianceTrainingName As String = EasSession.DbsComplianceTraining.Rows.Find(Function(m) m.ComplianceTrainingId = .ComplianceTrainingId).ComplianceTrainingName
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceTrainingId, String.Empty, .ComplianceTrainingId.ToString, .ComplianceTrainingId.ToString + "=" + _complianceTrainingName)
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.IssuanceDate, String.Empty, .IssuanceDate.ToDisplayFormat)
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ValidityDate, String.Empty, .ValidityDate.ToDisplayFormat)
                        Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)
                        Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                        AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceFileName, String.Empty, .ComplianceFileName)
                        AppLib.CreateLogDetails(_hrsMemberComplianceTrainingLogDetailList, "HrsMemberComplianceTrainingLogDetail")

                    End With
                Next

#End Region

#Region "HrsMemberWork"

                If _hrsMemberWorkList.Count > 0 Then
                    Me.InsertHrsMemberWorks(_hrsMemberWorkList)
                End If

                For Each _new As HrsMemberWork In _hrsMemberWorkList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberWorkLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberWorkLogDetailList.Clear()

                    With _new
                        Dim _municipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _municipalityName)

                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkPosition, String.Empty, .WorkPosition)
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyName, String.Empty, .CompanyName)
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyPhoneNumber, String.Empty, .CompanyPhoneNumber)
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkStartDate, String.Empty, .WorkStartDate.ToDisplayFormat)
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkEndDate, String.Empty, .WorkEndDate.ToDisplayFormat)
                        AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.ReasonForLeaving, String.Empty, .ReasonForLeaving)
                        'AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkFileName, String.Empty, .WorkFileName)
                        AppLib.CreateLogDetails(_hrsMemberWorkLogDetailList, "HrsMemberWorkLogDetail")

                    End With
                Next

#End Region

#Region "HrsMemberAffiliation"
                'Try

                If _hrsMemberAffiliationList.Count > 0 Then
                    Me.InsertHrsMemberAffiliations(_hrsMemberAffiliationList)
                End If

                For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberAffiliationLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberAffiliationLogDetailList.Clear()

                    With _new

                        Dim _affiliationName As String = EasSession.DbsAffiliation.Rows.Find(Function(m) m.AffiliationId = .AffiliationId).AffiliationName
                        AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationId, String.Empty, .AffiliationId.ToString, .AffiliationId.ToString + "=" + _affiliationName)

                        AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDate, String.Empty, .AffiliationDate.ToString)
                        AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationPosition, String.Empty, .AffiliationPosition)
                        AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDescription, String.Empty, .AffiliationDescription)
                        'AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationFileName, String.Empty, .AffiliationFileName)
                        'AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDate, String.Empty, .AffiliationDate)


                        AppLib.CreateLogDetails(_hrsMemberAffiliationLogDetailList, "HrsMemberAffiliationLogDetail")

                    End With
                Next


#End Region

#Region "HrsMemberMedicalResultType"

                If _hrsMemberMedicalResultTypeList.Count > 0 Then
                    Me.InsertHrsMemberMedicalResultTypes(_hrsMemberMedicalResultTypeList)
                End If

                For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeList

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _new.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberMedicalResultTypeLog", _logKeyList, LogActionId.Add, currentUserId)

                    _hrsMemberMedicalResultTypeLogDetailList.Clear()

                    With _new
                        Dim _medicalResultTypeName As String = EasSession.DbsMedicalResultType.Rows.Find(Function(m) m.MedicalResultTypeId = .MedicalResultTypeId).MedicalResultTypeName
                        AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeId, String.Empty, .MedicalResultTypeId.ToString, .MedicalResultTypeId.ToString + "=" + _medicalResultTypeName)

                        AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.Remarks, String.Empty, .Remarks)
                        AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeFileName, String.Empty, .MedicalResultTypeFileName)

                        AppLib.CreateLogDetails(_hrsMemberMedicalResultTypeLogDetailList, "HrsMemberMedicalResultTypeLogDetail")

                    End With
                Next


#End Region


                _successFlag = True


            Catch _exception As Exception
                Return Me.BadRequest(_exception.Message)
            Finally
                With _transaction
                    If _successFlag Then
                        .Commit()
                    Else
                        .Rollback()
                    End If
                    .Dispose()
                End With

                _transaction = Nothing
                DataCore.Disconnect()

            End Try

            'Return Me.Ok(True)
            Return Me.Ok(member.MemberId)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyMember")>
    <Route("members/{memberId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyMember(memberId As Integer, currentUserId As Integer, <FromBody> member As MemberBody) As IHttpActionResult

        If memberId <= 0 Then
            Throw New ArgumentException("Member ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _hrsMember As New HrsMember
            Dim _hrsMemberLanguageList As New HrsMemberLanguageList
            Dim _hrsMemberDisabilityList As New HrsMemberDisabilityList
            Dim _hrsMemberRnrRecordingList As New HrsMemberRnrRecordingList
            Dim _hrsMemberVaccineList As New HrsMemberVaccineList
            Dim _hrsMemberSkillSetList As New HrsMemberSkillSetList
            Dim _hrsMemberKinList As New HrsMemberKinList
            Dim _hrsMemberDocTypeList As New HrsMemberDocTypeList
            Dim _hrsMemberEducationList As New HrsMemberEducationList
            Dim _hrsMemberLicenseProfessionList As New HrsMemberLicenseProfessionList

            Dim _hrsMemberCDATrainingList As New HrsMemberCDATrainingList
            Dim _hrsMemberNCIIQualificationTitleList As New HrsMemberNCIIQualificationTitleList
            Dim _hrsMemberComplianceTrainingList As New HrsMemberComplianceTrainingList
            Dim _hrsMemberWorkList As New HrsMemberWorkList
            Dim _hrsMemberAffiliationList As New HrsMemberAffiliationList

            Dim _hrsMemberMedicalResultTypeList As New HrsMemberMedicalResultTypeList

            'Dim _hrsMemberSoloParentList As New HrsMemberSoloParentList

            '
            'Dim _hrsMemberWorkList As New HrsMemberWorkList
            'Dim _hrsMemberKinList As New HrsMemberKinList
            'Dim _hrsMemberDisabilityList As New HrsMemberDisabilityList
            'Dim _hrsMemberCertificateList As New HrsMemberCertificateList
            'Dim _hrsMemberEligibilityList As New HrsMemberEligibilityList
            'Dim _hrsMemberLicenseList As New HrsMemberLicenseList
            'Dim _hrsMemberSkillSetList As New HrsMemberSkillSetList
            'Dim _hrsMemberDocTypeList As New HrsMemberDocTypeList
            Me.LoadHrsMember(member, _hrsMember)

            With _hrsMember
                .BirthRegionId = GetRegion(member.BirthProvinceId.ToString)
                .RegionId = GetRegion(member.ProvinceId.ToString)

                If Not String.IsNullOrEmpty(.AlternateProvinceId) Then
                    .AlternateRegionId = GetRegion(.AlternateProvinceId)
                End If

                '.MemberStatusId = GetMemberStatus(member.EmploymentStatusId)
            End With

            For Each _language As HrsMemberLanguage In member.Languages
                _hrsMemberLanguageList.Add(_language)
            Next

            For Each _disability As HrsMemberDisability In member.Disabilities
                _hrsMemberDisabilityList.Add(_disability)
            Next

            For Each _recording As HrsMemberRnrRecording In member.RnrRecordings
                _hrsMemberRnrRecordingList.Add(_recording)
            Next

            For Each _vaccine As HrsMemberVaccine In member.Vaccines
                _hrsMemberVaccineList.Add(_vaccine)
            Next

            For Each _skillSet As HrsMemberSkillSet In member.SkillSets
                _hrsMemberSkillSetList.Add(_skillSet)
            Next

            For Each _kin As HrsMemberKin In member.Kins
                _hrsMemberKinList.Add(_kin)
            Next

            For Each _docType As HrsMemberDocType In member.Docs
                _hrsMemberDocTypeList.Add(_docType)
            Next

            For Each _education As HrsMemberEducation In member.Educations
                _hrsMemberEducationList.Add(_education)
            Next

            For Each _licenses As HrsMemberLicenseProfession In member.Licenses
                _hrsMemberLicenseProfessionList.Add(_licenses)
            Next

            For Each _cdaTrainings As HrsMemberCDATraining In member.CDATrainings
                _hrsMemberCDATrainingList.Add(_cdaTrainings)
            Next

            For Each _nccis As HrsMemberNCIIQualificationTitle In member.NCIIs
                _hrsMemberNCIIQualificationTitleList.Add(_nccis)
            Next

            For Each _compliances As HrsMemberComplianceTraining In member.Compliances
                _hrsMemberComplianceTrainingList.Add(_compliances)
            Next

            For Each _works As HrsMemberWork In member.Works
                _hrsMemberWorkList.Add(_works)
            Next

            For Each _affiliations As HrsMemberAffiliation In member.Affiliations
                _hrsMemberAffiliationList.Add(_affiliations)
            Next

            For Each _medicals As HrsMemberMedicalResultType In member.Medicals
                _hrsMemberMedicalResultTypeList.Add(_medicals)
            Next

            'For Each _detail As HrsMemberSoloParent In member.SoloParent
            '    _hrsMemberSoloParentList.Add(_detail)
            'Next

            ''Me.LoadHrsMemberSoloParent(member, _hrsMemberSoloParent)


            'For Each _work As HrsMemberWork In member.Works
            '    _hrsMemberWorkList.Add(_work)
            'Next


            'For Each _disability As HrsMemberDisability In member.Disabilities
            '    _hrsMemberDisabilityList.Add(_disability)
            'Next

            'For Each _certificate As HrsMemberCertificate In member.Certificates
            '    _hrsMemberCertificateList.Add(_certificate)
            'Next

            'For Each _eligibility As HrsMemberEligibility In member.Eligibilities
            '    _hrsMemberEligibilityList.Add(_eligibility)
            'Next

            'For Each _license As HrsMemberLicense In member.Licenses
            '    _hrsMemberLicenseList.Add(_license)
            'Next

            'For Each _skillSet As HrsMemberSkillSet In member.SkillSets
            '    _hrsMemberSkillSetList.Add(_skillSet)
            'Next

            'For Each _docType As HrsMemberDocType In member.DocTypes
            '    _hrsMemberDocTypeList.Add(_docType)
            'Next

            '
            ' Load old values from DB
            '
            Dim _hrsMemberOld As New HrsMember
            Dim _hrsMemberLanguageListOld As New HrsMemberLanguageList
            Dim _hrsMemberDisabilityListOld As New HrsMemberDisabilityList
            Dim _hrsMemberRnrRecordingListOld As New HrsMemberRnrRecordingList
            Dim _hrsMemberVaccineListOld As New HrsMemberVaccineList
            Dim _hrsMemberSkillSetListOld As New HrsMemberSkillSetList
            Dim _hrsMemberKinListOld As New HrsMemberKinList
            Dim _hrsMemberDocTypeListOld As New HrsMemberDocTypeList
            Dim _hrsMemberEducationListOld As New HrsMemberEducationList
            Dim _hrsMemberLicenseProfessionListOld As New HrsMemberLicenseProfessionList
            Dim _hrsMemberCDATrainingListOld As New HrsMemberCDATrainingList
            Dim _hrsMemberNCIIQualificationTitleListOld As New HrsMemberNCIIQualificationTitleList
            Dim _hrsMemberComplianceTrainingListOld As New HrsMemberComplianceTrainingList
            Dim _hrsMemberWorkListOld As New HrsMemberWorkList
            Dim _hrsMemberAffiliationListOld As New HrsMemberAffiliationList

            Dim _hrsMemberMedicalResultTypeListOld As New HrsMemberMedicalResultTypeList

            Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMember(memberId), Results.OkNegotiatedContentResult(Of DataSet))

            Using _dataSet As DataSet = _result.Content
                Dim _row As DataRow = _dataSet.Tables("member").Rows(0)

                Me.LoadHrsMember(_row, _hrsMemberOld)

                Me.LoadHrsMemberLanguageList(_dataSet.Tables("languages").Rows, _hrsMemberLanguageListOld)
                Me.LoadHrsMemberDisabilityList(_dataSet.Tables("disabilities").Rows, _hrsMemberDisabilityListOld)
                Me.LoadHrsMemberRnrRecordingList(_dataSet.Tables("recordings").Rows, _hrsMemberRnrRecordingListOld)
                Me.LoadHrsMemberVaccineList(_dataSet.Tables("vaccines").Rows, _hrsMemberVaccineListOld)
                Me.LoadHrsMemberSkillSetList(_dataSet.Tables("skillSets").Rows, _hrsMemberSkillSetListOld)
                Me.LoadHrsMemberKinList(_dataSet.Tables("kins").Rows, _hrsMemberKinListOld)
                Me.LoadHrsMemberDocTypeList(_dataSet.Tables("docs").Rows, _hrsMemberDocTypeListOld)
                Me.LoadHrsMemberEducationList(_dataSet.Tables("educations").Rows, _hrsMemberEducationListOld)


                Me.LoadHrsMemberLicenseProfessionList(_dataSet.Tables("licenses").Rows, _hrsMemberLicenseProfessionListOld)
                Me.LoadHrsMemberCDATrainingList(_dataSet.Tables("cdaTrainings").Rows, _hrsMemberCDATrainingListOld)
                Me.LoadHrsMemberNCIIQualificationTitleList(_dataSet.Tables("nciis").Rows, _hrsMemberNCIIQualificationTitleListOld)
                Me.LoadHrsMemberComplianceTrainingList(_dataSet.Tables("compliances").Rows, _hrsMemberComplianceTrainingListOld)
                Me.LoadHrsMemberWorkList(_dataSet.Tables("works").Rows, _hrsMemberWorkListOld)
                Me.LoadHrsMemberAffiliationList(_dataSet.Tables("affiliations").Rows, _hrsMemberAffiliationListOld)
                Me.LoadHrsMemberMedicalResultTypeList(_dataSet.Tables("medicals").Rows, _hrsMemberMedicalResultTypeListOld)
            End Using

            '
            ' Apply and log changes, save to DB
            '
            Dim _logKeyList As New LogKeyList
            Dim _id As Integer

            '
            ' HrsMember
            '

            Dim _hrsMemberLogDetailList As New SysLogDetailList

            With _hrsMemberOld

                If .MemberEmployeeId <> _hrsMember.MemberEmployeeId Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberEmployeeId, .MemberEmployeeId, _hrsMember.MemberEmployeeId)

                End If

                If .MemberLastName <> _hrsMember.MemberLastName Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberLastName, .MemberLastName, _hrsMember.MemberLastName)

                End If

                If .MemberFirstName <> _hrsMember.MemberFirstName Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberFirstName, .MemberFirstName, _hrsMember.MemberFirstName)
                End If

                If .MemberMiddleName <> _hrsMember.MemberMiddleName Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberMiddleName, .MemberMiddleName, _hrsMember.MemberMiddleName)
                End If

                'If .MemberSuffix <> _hrsMember.MemberSuffix Then
                '    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberSuffix, .MemberSuffix, _hrsMember.MemberSuffix)
                'End If

                If .MemberSuffixId <> _hrsMember.MemberSuffixId Then
                    Dim _oldMemberSuffixName As String = String.Empty
                    Try
                        _oldMemberSuffixName = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .MemberSuffixId).MemberSuffixName
                        Dim _newMemberSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = _hrsMember.MemberSuffixId).MemberSuffixName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberSuffixId, .MemberSuffixId.ToString, _hrsMember.MemberSuffixId.ToString, .MemberSuffixId.ToString + "=" + _oldMemberSuffixName + "; " + _hrsMember.MemberSuffixId.ToString + "=" + _newMemberSuffixName)


                    Catch ex As Exception

                    End Try

                End If

                If .MemberTypeId <> _hrsMember.MemberTypeId Then

                    Dim _oldMemberTypeName As String = String.Empty
                    Try
                        _oldMemberTypeName = EasSession.HrsMemberType.Rows.Find(Function(m) m.MemberTypeId = .MemberTypeId).MemberTypeName
                        Dim _newMemberTypeName As String = EasSession.HrsMemberType.Rows.Find(Function(m) m.MemberTypeId = _hrsMember.MemberTypeId).MemberTypeName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberTypeId, .MemberTypeId.ToString, _hrsMember.MemberTypeId.ToString, .MemberTypeId.ToString + "=" + _oldMemberTypeName + "; " + _hrsMember.MemberTypeId.ToString + "=" + _newMemberTypeName)

                    Catch ex As Exception

                    End Try

                End If

                If .TypeQualificationDetailId <> _hrsMember.TypeQualificationDetailId Then

                    Dim _oldMemberTypeQualificationName As String = String.Empty

                    'If .TypeQualificationDetailId.ToString <> "0" Then
                    Try

                        _oldMemberTypeQualificationName = EasSession.HrsMemberTypeQualification.Rows.Find(Function(m) m.TypeQualificationDetailId = .TypeQualificationDetailId).TypeQualificationName


                        Dim _newMemberTypeQualificationName As String = EasSession.HrsMemberTypeQualification.Rows.Find(Function(m) m.TypeQualificationDetailId = _hrsMember.TypeQualificationDetailId).TypeQualificationName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.TypeQualificationDetailId, .TypeQualificationDetailId.ToString, _hrsMember.TypeQualificationDetailId.ToString, .TypeQualificationDetailId.ToString + "=" + _oldMemberTypeQualificationName + "; " + _hrsMember.TypeQualificationDetailId.ToString + "=" + _newMemberTypeQualificationName)

                    Catch ex As Exception

                    End Try


                End If

                If .MemberStatusId <> _hrsMember.MemberStatusId Then
                    Dim _oldMemberStatusName As String = String.Empty

                    Try
                        _oldMemberStatusName = EasSession.HrsMemberStatus.Rows.Find(Function(m) m.MemberStatusId = .MemberStatusId).MemberStatusName
                        Dim _newMemberStatusName As String = EasSession.HrsMemberStatus.Rows.Find(Function(m) m.MemberStatusId = _hrsMember.MemberStatusId).MemberStatusName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MemberStatusId, .MemberStatusId.ToString, _hrsMember.MemberStatusId.ToString, .MemberStatusId.ToString + "=" + _oldMemberStatusName + "; " + _hrsMember.MemberStatusId.ToString + "=" + _newMemberStatusName)

                    Catch ex As Exception
                    End Try

                End If

                If .BirthDate <> _hrsMember.BirthDate Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthDate, .BirthDate.ToDisplayFormat, _hrsMember.BirthDate.ToDisplayFormat)
                End If

                If .BirthRegionId <> _hrsMember.BirthRegionId Then

                    Dim _oldRegionName As String = String.Empty

                    Try
                        _oldRegionName = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .BirthRegionId).RegionName
                        Dim _newRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = _hrsMember.BirthRegionId).RegionName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthRegionId, .BirthRegionId.ToString, _hrsMember.BirthRegionId.ToString, .BirthRegionId.ToString + "=" + _oldRegionName + "; " + _hrsMember.BirthRegionId.ToString + "=" + _newRegionName)

                    Catch ex As Exception

                    End Try


                End If

                If .BirthProvinceId <> _hrsMember.BirthProvinceId Then
                    Dim _oldProvinceName As String = String.Empty
                    Try
                        _oldProvinceName = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .BirthProvinceId).ProvinceName

                        Dim _newProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = _hrsMember.BirthProvinceId).ProvinceName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthProvinceId, .BirthProvinceId.ToString, _hrsMember.BirthProvinceId.ToString, .BirthProvinceId.ToString + "=" + _oldProvinceName + "; " + _hrsMember.BirthProvinceId.ToString + "=" + _newProvinceName)

                    Catch ex As Exception
                    End Try

                End If

                If .BirthMunicipalityId <> _hrsMember.BirthMunicipalityId Then
                    Dim _oldMunicipalityName As String = String.Empty
                    Try
                        _oldMunicipalityName = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .BirthMunicipalityId).MunicipalityName

                        Dim _newMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = _hrsMember.BirthMunicipalityId).MunicipalityName
                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BirthMunicipalityId, .BirthMunicipalityId.ToString, _hrsMember.BirthMunicipalityId.ToString, .BirthMunicipalityId.ToString + "=" + _oldMunicipalityName + "; " + _hrsMember.BirthMunicipalityId.ToString + "=" + _newMunicipalityName)

                    Catch ex As Exception

                    End Try

                End If

                If .SexId <> _hrsMember.SexId Then
                    Dim _oldSexName As String = String.Empty
                    Try
                        _oldSexName = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = .SexId).SexName
                        Dim _newSexName As String = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = _hrsMember.SexId).SexName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.SexId, .SexId.ToString, _hrsMember.SexId.ToString, .SexId.ToString + "=" + _oldSexName + "; " + _hrsMember.SexId.ToString + "=" + _newSexName)

                    Catch ex As Exception

                    End Try

                End If

                If .BloodTypeId <> _hrsMember.BloodTypeId Then
                    Dim _oldBloodTypeName As String = ""
                    Dim _newBloodTypeName As String = ""

                    If .BloodTypeId.ToString = "0" Then
                    Else
                        _oldBloodTypeName = EasSession.DbsBloodType.Rows.Find(Function(m) m.BloodTypeId = .BloodTypeId).BloodTypeName
                    End If

                    If _hrsMember.BloodTypeId.ToString = "0" Then
                    Else
                        _newBloodTypeName = EasSession.DbsBloodType.Rows.Find(Function(m) m.BloodTypeId = _hrsMember.BloodTypeId).BloodTypeName
                    End If

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BloodTypeId, .BloodTypeId.ToString, _hrsMember.BloodTypeId.ToString, .BloodTypeId.ToString + "=" + _oldBloodTypeName + "; " + _hrsMember.BloodTypeId.ToString + "=" + _newBloodTypeName)
                End If

                If .Height <> _hrsMember.Height Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Height, .Height, _hrsMember.Height)
                End If

                If .Weight <> _hrsMember.Weight Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Weight, .Weight, _hrsMember.Weight)
                End If

                If .CivilStatusId <> _hrsMember.CivilStatusId Then
                    Dim _oldCivilStatusName As String = String.Empty
                    Try
                        _oldCivilStatusName = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = .CivilStatusId).CivilStatusName
                        Dim _newCivilStatusName As String = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = _hrsMember.CivilStatusId).CivilStatusName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CivilStatusId, .CivilStatusId.ToString, _hrsMember.CivilStatusId.ToString, .CivilStatusId.ToString + "=" + _oldCivilStatusName + "; " + _hrsMember.CivilStatusId.ToString + "=" + _newCivilStatusName)


                    Catch ex As Exception

                    End Try

                End If

                If .ReligionId <> _hrsMember.ReligionId Then

                    Dim _oldReligionName As String = String.Empty

                    Try
                        _oldReligionName = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = .ReligionId).ReligionName

                        Dim _newReligionName As String = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = _hrsMember.ReligionId).ReligionName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ReligionId, .ReligionId.ToString, _hrsMember.ReligionId.ToString, .ReligionId.ToString + "=" + _oldReligionName + "; " + _hrsMember.ReligionId.ToString + "=" + _newReligionName)

                    Catch ex As Exception

                    End Try


                End If

                If .Address1 <> _hrsMember.Address1 Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Address1, .Address1, _hrsMember.Address1)
                End If

                If .Address2 <> _hrsMember.Address2 Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Address2, .Address2, _hrsMember.Address2)
                End If

                If .PostalCode <> _hrsMember.PostalCode Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PostalCode, .PostalCode, _hrsMember.PostalCode)
                End If

                If .PhoneNumber <> _hrsMember.PhoneNumber Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PhoneNumber, .PhoneNumber, _hrsMember.PhoneNumber)
                End If

                If .MobileNumber <> _hrsMember.MobileNumber Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MobileNumber, .MobileNumber, _hrsMember.MobileNumber)
                End If

                If .Email <> _hrsMember.Email Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Email, .Email, _hrsMember.Email)
                End If


                If .RegionId <> _hrsMember.RegionId Then
                    Dim _oldRegionName As String = String.Empty
                    Try
                        _oldRegionName = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .RegionId).RegionName
                        Dim _newRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = _hrsMember.RegionId).RegionName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RegionId, .RegionId.ToString, _hrsMember.RegionId.ToString, .RegionId.ToString + "=" + _oldRegionName + "; " + _hrsMember.RegionId.ToString + "=" + _newRegionName)

                    Catch ex As Exception

                    End Try

                End If

                If .ProvinceId <> _hrsMember.ProvinceId Then
                    Dim _oldProvinceName As String = String.Empty
                    Try
                        _oldProvinceName = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .ProvinceId).ProvinceName

                        Dim _newProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = _hrsMember.ProvinceId).ProvinceName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ProvinceId, .ProvinceId.ToString, _hrsMember.ProvinceId.ToString, .ProvinceId.ToString + "=" + _oldProvinceName + "; " + _hrsMember.ProvinceId.ToString + "=" + _newProvinceName)

                    Catch ex As Exception

                    End Try
                End If

                If .MunicipalityId <> _hrsMember.MunicipalityId Then
                    Dim _oldMunicipalityName As String = String.Empty
                    Try
                        _oldMunicipalityName = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
                        Dim _newMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = _hrsMember.MunicipalityId).MunicipalityName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.MunicipalityId, .MunicipalityId.ToString, _hrsMember.MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _oldMunicipalityName + "; " + _hrsMember.MunicipalityId.ToString + "=" + _newMunicipalityName)

                    Catch ex As Exception

                    End Try


                End If

                If .BarangayId <> _hrsMember.BarangayId Then
                    Dim _oldBarangayName As String = String.Empty

                    Try
                        _oldBarangayName = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .BarangayId).BarangayName
                        Dim _newBarangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = _hrsMember.BarangayId).BarangayName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BarangayId, .BarangayId.ToString, _hrsMember.BarangayId.ToString, .BarangayId.ToString + "=" + _oldBarangayName + "; " + _hrsMember.BarangayId.ToString + "=" + _newBarangayName)

                    Catch ex As Exception

                    End Try




                End If


                If .Facebook <> _hrsMember.Facebook Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Facebook, .Facebook, _hrsMember.Facebook)
                End If

                If .Instagram <> _hrsMember.Instagram Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.Instagram, .Instagram, _hrsMember.Instagram)
                End If


                If .AlternateRegionId <> _hrsMember.AlternateRegionId Then
                    Dim _oldRegionName As String = String.Empty
                    Try
                        _oldRegionName = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .AlternateRegionId).RegionName
                        Dim _newRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = _hrsMember.AlternateRegionId).RegionName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateRegionId, .AlternateRegionId.ToString, _hrsMember.AlternateRegionId.ToString, .AlternateRegionId.ToString + "=" + _oldRegionName + "; " + _hrsMember.AlternateRegionId.ToString + "=" + _newRegionName)

                    Catch ex As Exception

                    End Try

                End If

                If .AlternateProvinceId <> _hrsMember.AlternateProvinceId Then
                    Dim _oldProvinceName As String = String.Empty
                    Try
                        _oldProvinceName = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .AlternateProvinceId).ProvinceName
                        Dim _newProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = _hrsMember.AlternateProvinceId).ProvinceName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateProvinceId, .AlternateProvinceId.ToString, _hrsMember.AlternateProvinceId.ToString, .AlternateProvinceId.ToString + "=" + _oldProvinceName + "; " + _hrsMember.AlternateProvinceId.ToString + "=" + _newProvinceName)
                    Catch ex As Exception

                    End Try


                End If

                If .AlternateMunicipalityId <> _hrsMember.AlternateMunicipalityId Then
                    Dim _oldMunicipalityName As String = String.Empty
                    Try
                        _oldMunicipalityName = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .AlternateMunicipalityId).MunicipalityName
                        Dim _newMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = _hrsMember.AlternateMunicipalityId).MunicipalityName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateMunicipalityId, .AlternateMunicipalityId.ToString, _hrsMember.AlternateMunicipalityId.ToString, .AlternateMunicipalityId.ToString + "=" + _oldMunicipalityName + "; " + _hrsMember.AlternateMunicipalityId.ToString + "=" + _newMunicipalityName)
                    Catch ex As Exception

                    End Try



                End If

                If .AlternateBarangayId <> _hrsMember.AlternateBarangayId Then
                    Dim _oldBarangayName As String = String.Empty

                    Try
                        _oldBarangayName = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .AlternateBarangayId).BarangayName
                        Dim _newBarangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = _hrsMember.AlternateBarangayId).BarangayName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateBarangayId, .AlternateBarangayId.ToString, _hrsMember.AlternateBarangayId.ToString, .AlternateBarangayId.ToString + "=" + _oldBarangayName + "; " + _hrsMember.AlternateBarangayId.ToString + "=" + _newBarangayName)
                    Catch ex As Exception

                    End Try


                End If

                If .AlternatePostalCode <> _hrsMember.AlternatePostalCode Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternatePostalCode, .AlternatePostalCode, _hrsMember.AlternatePostalCode)
                End If

                If .AlternateAddress1 <> _hrsMember.AlternateAddress1 Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateAddress1, .AlternateAddress1, _hrsMember.AlternateAddress1)
                End If

                If .AlternateAddress2 <> _hrsMember.AlternateAddress2 Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AlternateAddress2, .AlternateAddress2, _hrsMember.AlternateAddress2)
                End If


                If .CDAMemberTypeId <> _hrsMember.CDAMemberTypeId Then
                    Dim _oldCDAMemberTypeName As String = String.Empty

                    Try
                        _oldCDAMemberTypeName = EasSession.DbsCDAMemberType.Rows.Find(Function(m) m.CDAMemberTypeId = .CDAMemberTypeId).CDAMemberTypeName
                        Dim _newCDAMemberTypeName As String = EasSession.DbsCDAMemberType.Rows.Find(Function(m) m.CDAMemberTypeId = _hrsMember.CDAMemberTypeId).CDAMemberTypeName

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CDAMemberTypeId, .CDAMemberTypeId.ToString, _hrsMember.CDAMemberTypeId.ToString, .CDAMemberTypeId.ToString + "=" + _oldCDAMemberTypeName + "; " + _hrsMember.CDAMemberTypeId.ToString + "=" + _newCDAMemberTypeName)

                    Catch ex As Exception

                    End Try


                End If

                If .CDAMemberTypeAmount <> _hrsMember.CDAMemberTypeAmount Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.CDAMemberTypeAmount, .CDAMemberTypeAmount.ToString, _hrsMember.CDAMemberTypeAmount.ToString)
                End If

                'If .EmploymentStatusId <> _hrsMember.EmploymentStatusId Then

                '   Dim _oldEmploymentStatusName As String = String.Empty

                '   Try
                '      _oldEmploymentStatusName = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                '      Dim _newEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = _hrsMember.EmploymentStatusId).EmploymentStatusName

                '      AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.EmploymentStatusId, .EmploymentStatusId.ToString, _hrsMember.EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _oldEmploymentStatusName + "; " + _hrsMember.EmploymentStatusId.ToString + "=" + _newEmploymentStatusName)

                '   Catch ex As Exception

                '   End Try


                'End If

                'If .EmploymentTypeId <> _hrsMember.EmploymentTypeId Then
                '   Dim _oldEmploymentTypeName As String = String.Empty

                '   If .EmploymentTypeId > 0 Then
                '      _oldEmploymentTypeName = EasSession.DbsEmploymentType.Rows.Find(Function(m) m.EmploymentTypeId = .EmploymentTypeId).EmploymentTypeName
                '   End If

                '   Dim _newEmploymentTypeName As String = EasSession.DbsEmploymentType.Rows.Find(Function(m) m.EmploymentTypeId = _hrsMember.EmploymentTypeId).EmploymentTypeName
                '   AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.EmploymentTypeId, .EmploymentTypeId.ToString, _hrsMember.EmploymentTypeId.ToString, .EmploymentTypeId.ToString + "=" + _oldEmploymentTypeName + "; " + _hrsMember.EmploymentTypeId.ToString + "=" + _newEmploymentTypeName)

                '   'Dim _oldEmploymentTypeName As String = EasSession.DbsEmploymentType.Rows.Find(Function(m) m.EmploymentTypeId = .EmploymentTypeId).EmploymentTypeName
                'End If

                If .AbroadFlag <> _hrsMember.AbroadFlag Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.AbroadFlag, .AbroadFlag.ToString, _hrsMember.AbroadFlag.ToString)
                End If

                If .RelocateFlag <> _hrsMember.RelocateFlag Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RelocateFlag, .RelocateFlag.ToString, _hrsMember.RelocateFlag.ToString)
                End If

                If .WeekendHolidayFlag <> _hrsMember.WeekendHolidayFlag Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.WeekendHolidayFlag, .WeekendHolidayFlag.ToString, _hrsMember.WeekendHolidayFlag.ToString)
                End If

                If .ExpectedSalary <> _hrsMember.ExpectedSalary Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ExpectedSalary, .ExpectedSalary.ToString, _hrsMember.ExpectedSalary.ToString)
                End If

                If .GCashNumber <> _hrsMember.GCashNumber Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.GCashNumber, .GCashNumber.ToString, _hrsMember.GCashNumber.ToString)
                End If

                If .BankId <> _hrsMember.BankId Then
                    Dim _oldBankName As String = String.Empty

                    If .BankId > 0 Then
                        _oldBankName = EasSession.DbsBank.Rows.Find(Function(m) m.BankId = .BankId).BankName
                    End If

                    Dim _newBankName As String = EasSession.DbsBank.Rows.Find(Function(m) m.BankId = _hrsMember.BankId).BankName

                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BankId, .BankId.ToString, _hrsMember.BankId.ToString, .BankId.ToString + "=" + _oldBankName + "; " + _hrsMember.BankId.ToString + "=" + _newBankName)
                End If

                If .RecruiterId <> _hrsMember.RecruiterId Then
                    Dim _oldRecruiterName As String = String.Empty

                    If .RecruiterId > 0 Then
                        _oldRecruiterName = EasSession.HrsRecruiter.Rows.Find(Function(m) m.RecruiterId = .RecruiterId).RecruiterName
                    End If
                    Dim _newRecruiterName As String = EasSession.HrsRecruiter.Rows.Find(Function(m) m.RecruiterId = _hrsMember.RecruiterId).RecruiterName
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.RecruiterId, .RecruiterId.ToString, _hrsMember.RecruiterId.ToString, .RecruiterId.ToString + "=" + _oldRecruiterName + "; " + _hrsMember.RecruiterId.ToString + "=" + _newRecruiterName)

                End If

                'If .ApplicationSourceId <> _hrsMember.ApplicationSourceId Then
                '    Dim _oldApplicationSourceName As String = String.Empty

                '    If .ApplicationSourceId > 0 Then
                '        _oldApplicationSourceName = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = .ApplicationSourceId).ApplicationSourceName
                '    End If

                '    Dim _newApplicationSourceName As String = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = _hrsMember.ApplicationSourceId).ApplicationSourceName

                '    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.ApplicationSourceId, .ApplicationSourceId.ToString, _hrsMember.ApplicationSourceId.ToString, .ApplicationSourceId.ToString + "=" + _oldApplicationSourceName + "; " + _hrsMember.ApplicationSourceId.ToString + "=" + _newApplicationSourceName)
                'End If

                If .BankAccountNumber <> _hrsMember.BankAccountNumber Then
                    AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.BankAccountNumber, .BankAccountNumber.ToString, _hrsMember.BankAccountNumber.ToString)
                End If

                If .PhotoFileName <> _hrsMember.PhotoFileName Then

                    Try

                        UploadMemberFile(memberId, _hrsMember.PhotoFileName, _hrsMember.PhotoGUID)
                        RemoveMemberFile(memberId, .PhotoFileName, .PhotoGUID)

                        AppLib.AddLogDetail(_hrsMemberLogDetailList, 0, LogColumnId.PhotoFileName, .PhotoFileName.ToString, _hrsMember.PhotoFileName.ToString)
                        _hrsMember.ImageExtension = Path.GetExtension(_hrsMember.PhotoFileName).ToLowerInvariant
                        '.PhotoFileName = memberId.ToString + "\" + _hrsMember.PhotoFileName + Path.GetExtension(_hrsMember.PhotoFileName).ToLowerInvariant

                    Catch ex As Exception
                        '
                        'AllText("e:\4.txt", ex.Message)
                    End Try


                End If


            End With


#Region "HrsMemberLanguage"

            Dim _hrsMemberLanguageLogDetailList As New SysLogDetailList

            Dim _removeLanguageCount As Integer
            Dim _addLanguageCount As Integer
            Dim _editLanguageCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberLanguage In _hrsMemberLanguageListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberLanguage In _hrsMemberLanguageList
                    If _new.LanguageDetailId = _old.LanguageDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeLanguageCount = _removeLanguageCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberLanguage In _hrsMemberLanguageList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberLanguage In _hrsMemberLanguageListOld
                    If _new.LanguageDetailId = _old.LanguageDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .LanguageId <> _old.LanguageId Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addLanguageCount = _addLanguageCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editLanguageCount = _editLanguageCount + 1
                End If

            Next

            Dim _hrsMemberLanguageListNew As New HrsMemberLanguageList      ' for adding new Trx Details

            If _addLanguageCount > 0 Then
                Dim _hrsMemberLanguage As HrsMemberLanguage

                For Each _new As HrsMemberLanguage In _hrsMemberLanguageList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberLanguage = New HrsMemberLanguage
                        _hrsMemberLanguageListNew.Add(_hrsMemberLanguage)
                        DataLib.ScatterValues(_new, _hrsMemberLanguage)
                        _hrsMemberLanguage.MemberId = member.MemberId

                    End If
                Next

            End If

#End Region
            '
#Region "HrsMemberDisability"

            Dim _hrsMemberDisabilityLogDetailList As New SysLogDetailList

            Dim _removeDisabilityCount As Integer
            Dim _addDisabilityCount As Integer
            Dim _editDisabilityCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberDisability In _hrsMemberDisabilityListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberDisability In _hrsMemberDisabilityList
                    If _new.DisabilityDetailId = _old.DisabilityDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeDisabilityCount = _removeDisabilityCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberDisability In _hrsMemberDisabilityList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberDisability In _hrsMemberDisabilityListOld
                    If _new.DisabilityDetailId = _old.DisabilityDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .DisabilityId <> _old.DisabilityId OrElse .Remarks <> _old.Remarks Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addDisabilityCount = _addDisabilityCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editDisabilityCount = _editDisabilityCount + 1
                End If

            Next

            Dim _hrsMemberDisabilityListNew As New HrsMemberDisabilityList      ' for adding new Trx Details

            If _addDisabilityCount > 0 Then
                'Dim _trxDetailId As Integer = SysLib.GetNextSequence("TrxDetailId", _addDetailCount)
                Dim _hrsMemberDisability As HrsMemberDisability

                For Each _new As HrsMemberDisability In _hrsMemberDisabilityList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberDisability = New HrsMemberDisability
                        _hrsMemberDisabilityListNew.Add(_hrsMemberDisability)
                        DataLib.ScatterValues(_new, _hrsMemberDisability)
                        _hrsMemberDisability.MemberId = member.MemberId

                        '_finTrxDetail.TrxDetailId = _trxDetailId
                        '_trxDetailId = _trxDetailId + 1
                    End If
                Next

            End If

#End Region

#Region "HrsMemberRnrRecording"

            Dim _hrsMemberRnrRecordingLogDetailList As New SysLogDetailList

            Dim _removeRnrRecordingCount As Integer
            Dim _addRnrRecordingCount As Integer
            Dim _editRnrRecordingCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberRnrRecording In _hrsMemberRnrRecordingListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingList
                    If _new.RnrRecordingDetailId = _old.RnrRecordingDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeRnrRecordingCount = _removeRnrRecordingCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberRnrRecording In _hrsMemberRnrRecordingListOld
                    If _new.RnrRecordingDetailId = _old.RnrRecordingDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .RnrRecordingId <> _old.RnrRecordingId OrElse .RnrNumberId <> _old.RnrNumberId Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addRnrRecordingCount = _addRnrRecordingCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editRnrRecordingCount = _editRnrRecordingCount + 1
                End If

            Next

            Dim _hrsMemberRnrRecordingListNew As New HrsMemberRnrRecordingList      ' for adding new Trx Details

            If _addRnrRecordingCount > 0 Then
                Dim _hrsMemberRnrRecording As HrsMemberRnrRecording

                For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberRnrRecording = New HrsMemberRnrRecording
                        _hrsMemberRnrRecordingListNew.Add(_hrsMemberRnrRecording)
                        DataLib.ScatterValues(_new, _hrsMemberRnrRecording)
                        _hrsMemberRnrRecording.MemberId = member.MemberId

                    End If
                Next

            End If

#End Region

#Region "HrsMemberVaccine"

            Dim _hrsMemberVaccineLogDetailList As New SysLogDetailList

            Dim _removeVaccineCount As Integer
            Dim _addVaccineCount As Integer
            Dim _editVaccineCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberVaccine In _hrsMemberVaccineListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberVaccine In _hrsMemberVaccineList
                    If _new.VaccineDetailId = _old.VaccineDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeVaccineCount = _removeVaccineCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberVaccine In _hrsMemberVaccineList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberVaccine In _hrsMemberVaccineListOld
                    If _new.VaccineDetailId = _old.VaccineDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .VaccineTypeId <> _old.VaccineTypeId OrElse .VaccineName <> _old.VaccineName OrElse .VaccineDate <> _old.VaccineDate Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addVaccineCount = _addVaccineCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editVaccineCount = _editVaccineCount + 1
                End If

            Next

            Dim _hrsMemberVaccineListNew As New HrsMemberVaccineList      ' for adding new Trx Details

            If _addVaccineCount > 0 Then
                Dim _hrsMemberVaccine As HrsMemberVaccine

                For Each _new As HrsMemberVaccine In _hrsMemberVaccineList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberVaccine = New HrsMemberVaccine
                        _hrsMemberVaccineListNew.Add(_hrsMemberVaccine)
                        DataLib.ScatterValues(_new, _hrsMemberVaccine)
                        _hrsMemberVaccine.MemberId = member.MemberId

                    End If
                Next

            End If

#End Region

#Region "HrsMemberSkillSet"

            Dim _hrsMemberSkillSetLogDetailList As New SysLogDetailList

            Dim _removeSkillSetCount As Integer
            Dim _addSkillSetCount As Integer
            Dim _editSkillSetCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberSkillSet In _hrsMemberSkillSetListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetList

                    If _new.SkillSetDetailId = _old.SkillSetDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then

                    _removeSkillSetCount = _removeSkillSetCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberSkillSet In _hrsMemberSkillSetListOld
                    If _new.SkillSetDetailId = _old.SkillSetDetailId Then

                        _new.LogActionId = 0   ' don't add

                        With _new
                            'If .ed <> _old.AccountId Then
                            '    .LogActionId = LogActionId.Edit
                            'End If

                            If .SkillDetailId <> _old.SkillDetailId OrElse .Remarks <> _old.Remarks Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addSkillSetCount = _addSkillSetCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editSkillSetCount = _editSkillSetCount + 1
                End If

            Next

            Dim _hrsMemberSkillSetListNew As New HrsMemberSkillSetList      ' for adding new Trx Details

            If _addSkillSetCount > 0 Then
                Dim _hrsMemberSkillSet As HrsMemberSkillSet

                For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberSkillSet = New HrsMemberSkillSet
                        _hrsMemberSkillSetListNew.Add(_hrsMemberSkillSet)
                        DataLib.ScatterValues(_new, _hrsMemberSkillSet)
                        _hrsMemberSkillSet.MemberId = member.MemberId
                    End If
                Next

            End If


#End Region

#Region "HrsMemberKin"

            Dim _hrsMemberKinLogDetailList As New SysLogDetailList

            Dim _removeKinCount As Integer
            Dim _addKinCount As Integer
            Dim _editKinCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberKin In _hrsMemberKinListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberKin In _hrsMemberKinList
                    If _new.KinDetailId = _old.KinDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeKinCount = _removeKinCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberKin In _hrsMemberKinList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberKin In _hrsMemberKinListOld
                    If _new.KinDetailId = _old.KinDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .KinLastName <> _old.KinLastName OrElse .KinFirstName <> _old.KinFirstName OrElse .KinMiddleName <> _old.KinMiddleName OrElse .KinSuffixId <> _old.KinSuffixId OrElse .EmergencyContactFlag <> _old.EmergencyContactFlag OrElse .RelationId <> _old.RelationId OrElse .KinPhoneNumber <> _old.KinPhoneNumber OrElse .KinOccupation <> _old.KinOccupation OrElse .KinEmail <> _old.KinEmail OrElse .KinFileName <> _old.KinFileName OrElse .KinGUID <> _old.KinGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addKinCount = _addKinCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editKinCount = _editKinCount + 1
                End If

            Next

            Dim _hrsMemberKinListNew As New HrsMemberKinList      ' for adding new Trx Details

            If _addKinCount > 0 Then
                Dim _hrsMemberKin As HrsMemberKin
                For Each _new As HrsMemberKin In _hrsMemberKinList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberKin = New HrsMemberKin
                        _hrsMemberKinListNew.Add(_hrsMemberKin)
                        DataLib.ScatterValues(_new, _hrsMemberKin)
                        _hrsMemberKin.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberDocType"

            Dim _hrsMemberDocTypeLogDetailList As New SysLogDetailList

            Dim _removeDocTypeCount As Integer
            Dim _addDocTypeCount As Integer
            Dim _editDocTypeCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberDocType In _hrsMemberDocTypeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberDocType In _hrsMemberDocTypeList
                    If _new.DocTypeDetailId = _old.DocTypeDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeDocTypeCount = _removeDocTypeCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberDocType In _hrsMemberDocTypeList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberDocType In _hrsMemberDocTypeListOld
                    If _new.DocTypeDetailId = _old.DocTypeDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .DocTypeId <> _old.DocTypeId OrElse .DocTypeReference <> _old.DocTypeReference OrElse .DocTypeFileName <> _old.DocTypeFileName OrElse .DocTypeGUID <> _old.DocTypeGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addDocTypeCount = _addDocTypeCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editDocTypeCount = _editDocTypeCount + 1
                End If

            Next

            Dim _hrsMemberDocTypeListNew As New HrsMemberDocTypeList      ' for adding new Trx Details

            If _addDocTypeCount > 0 Then
                Dim _hrsMemberDocType As HrsMemberDocType
                For Each _new As HrsMemberDocType In _hrsMemberDocTypeList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberDocType = New HrsMemberDocType
                        _hrsMemberDocTypeListNew.Add(_hrsMemberDocType)
                        DataLib.ScatterValues(_new, _hrsMemberDocType)
                        _hrsMemberDocType.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberEducation"

            Dim _hrsMemberEducationLogDetailList As New SysLogDetailList

            Dim _removeEducationCount As Integer
            Dim _addEducationCount As Integer
            Dim _editEducationCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberEducation In _hrsMemberEducationListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberEducation In _hrsMemberEducationList
                    If _new.EduDetailId = _old.EduDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeEducationCount = _removeEducationCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberEducation In _hrsMemberEducationList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberEducation In _hrsMemberEducationListOld
                    If _new.EduDetailId = _old.EduDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .EducationLevelId <> _old.EducationLevelId OrElse .SchoolName <> _old.SchoolName OrElse .CourseName <> _old.CourseName OrElse .EduStartYear <> _old.EduStartYear OrElse .EduEndYear <> _old.EduEndYear OrElse .EduFileName <> _old.EduFileName OrElse .EduGUID <> _old.EduGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addEducationCount = _addEducationCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editEducationCount = _editEducationCount + 1
                End If

            Next

            Dim _hrsMemberEducationListNew As New HrsMemberEducationList      ' for adding new Trx Details

            If _addEducationCount > 0 Then
                Dim _hrsMemberEducation As HrsMemberEducation
                For Each _new As HrsMemberEducation In _hrsMemberEducationList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberEducation = New HrsMemberEducation
                        _hrsMemberEducationListNew.Add(_hrsMemberEducation)
                        DataLib.ScatterValues(_new, _hrsMemberEducation)
                        _hrsMemberEducation.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberLicenseProfession"

            Dim _hrsMemberLicenseProfessionLogDetailList As New SysLogDetailList

            Dim _removeLicenseProfessionCount As Integer
            Dim _addLicenseProfessionCount As Integer
            Dim _editLicenseProfessionCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionList
                    If _new.LicenseProfessionDetailId = _old.LicenseProfessionDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeLicenseProfessionCount = _removeLicenseProfessionCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionListOld
                    If _new.LicenseProfessionDetailId = _old.LicenseProfessionDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .LicenseProfessionId <> _old.LicenseProfessionId OrElse .PRCIdNo <> _old.PRCIdNo OrElse .LicenseFileName <> _old.LicenseFileName OrElse .LicenseGUID <> _old.LicenseGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addLicenseProfessionCount = _addLicenseProfessionCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editLicenseProfessionCount = _editLicenseProfessionCount + 1
                End If

            Next

            Dim _hrsMemberLicenseProfessionListNew As New HrsMemberLicenseProfessionList      ' for adding new Trx Details

            If _addLicenseProfessionCount > 0 Then
                Dim _hrsMemberLicenseProfession As HrsMemberLicenseProfession
                For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberLicenseProfession = New HrsMemberLicenseProfession
                        _hrsMemberLicenseProfessionListNew.Add(_hrsMemberLicenseProfession)
                        DataLib.ScatterValues(_new, _hrsMemberLicenseProfession)
                        _hrsMemberLicenseProfession.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberCDATraining"

            Dim _hrsMemberCDATrainingLogDetailList As New SysLogDetailList

            Dim _removeCDATrainingCount As Integer
            Dim _addCDATrainingCount As Integer
            Dim _editCDATrainingCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberCDATraining In _hrsMemberCDATrainingListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingList
                    If _new.CDATrainingDetailId = _old.CDATrainingDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeCDATrainingCount = _removeCDATrainingCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberCDATraining In _hrsMemberCDATrainingListOld
                    If _new.CDATrainingDetailId = _old.CDATrainingDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .CertificateNumber <> _old.CertificateNumber OrElse .CDATrainingId <> _old.CDATrainingId OrElse .TrainingDate <> _old.TrainingDate OrElse .CDAFileName <> _old.CDAFileName OrElse .CDAGUID <> _old.CDAGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addCDATrainingCount = _addCDATrainingCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editCDATrainingCount = _editCDATrainingCount + 1
                End If

            Next

            Dim _hrsMemberCDATrainingListNew As New HrsMemberCDATrainingList      ' for adding new Trx Details

            If _addCDATrainingCount > 0 Then
                Dim _hrsMemberCDATraining As HrsMemberCDATraining
                For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberCDATraining = New HrsMemberCDATraining
                        _hrsMemberCDATrainingListNew.Add(_hrsMemberCDATraining)
                        DataLib.ScatterValues(_new, _hrsMemberCDATraining)
                        _hrsMemberCDATraining.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberNCIIQualificationTitle"

            Dim _hrsMemberNCIIQualificationTitleLogDetailList As New SysLogDetailList

            Dim _removeNCIIQualificationTitleCount As Integer
            Dim _addNCIIQualificationTitleCount As Integer
            Dim _editNCIIQualificationTitleCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleList
                    If _new.NCIIQualificationTitleDetailId = _old.NCIIQualificationTitleDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeNCIIQualificationTitleCount = _removeNCIIQualificationTitleCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleListOld
                    If _new.NCIIQualificationTitleDetailId = _old.NCIIQualificationTitleDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .CertificateNumber <> _old.CertificateNumber OrElse .NCIIQualificationTitleId <> _old.NCIIQualificationTitleId OrElse .IssuanceDate <> _old.IssuanceDate OrElse .ValidityDate <> _old.ValidityDate OrElse .TrainingInstitutionId <> _old.TrainingInstitutionId OrElse .AssessmentCenterId <> _old.AssessmentCenterId OrElse .NCIIFileName <> _old.NCIIFileName OrElse .NCIIGUID <> _old.NCIIGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addNCIIQualificationTitleCount = _addNCIIQualificationTitleCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editNCIIQualificationTitleCount = _editNCIIQualificationTitleCount + 1
                End If

            Next

            Dim _hrsMemberNCIIQualificationTitleListNew As New HrsMemberNCIIQualificationTitleList      ' for adding new Trx Details

            If _addNCIIQualificationTitleCount > 0 Then
                Dim _hrsMemberNCIIQualificationTitle As HrsMemberNCIIQualificationTitle
                For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberNCIIQualificationTitle = New HrsMemberNCIIQualificationTitle
                        _hrsMemberNCIIQualificationTitleListNew.Add(_hrsMemberNCIIQualificationTitle)
                        DataLib.ScatterValues(_new, _hrsMemberNCIIQualificationTitle)
                        _hrsMemberNCIIQualificationTitle.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberComplianceTraining"

            Dim _hrsMemberComplianceTrainingLogDetailList As New SysLogDetailList

            Dim _removeComplianceTrainingCount As Integer
            Dim _addComplianceTrainingCount As Integer
            Dim _editComplianceTrainingCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingList
                    If _new.ComplianceTrainingDetailId = _old.ComplianceTrainingDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeComplianceTrainingCount = _removeComplianceTrainingCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingListOld
                    If _new.ComplianceTrainingDetailId = _old.ComplianceTrainingDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .CertificateNumber <> _old.CertificateNumber OrElse .ComplianceTrainingId <> _old.ComplianceTrainingId OrElse .IssuanceDate <> _old.IssuanceDate OrElse .ValidityDate <> _old.ValidityDate OrElse .TrainingInstitutionId <> _old.TrainingInstitutionId OrElse .AssessmentCenterId <> _old.AssessmentCenterId OrElse .ComplianceFileName <> _old.ComplianceFileName OrElse .ComplianceGUID <> _old.ComplianceGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addComplianceTrainingCount = _addComplianceTrainingCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editComplianceTrainingCount = _editComplianceTrainingCount + 1
                End If

            Next

            Dim _hrsMemberComplianceTrainingListNew As New HrsMemberComplianceTrainingList      ' for adding new Trx Details

            If _addComplianceTrainingCount > 0 Then
                Dim _hrsMemberComplianceTraining As HrsMemberComplianceTraining
                For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberComplianceTraining = New HrsMemberComplianceTraining
                        _hrsMemberComplianceTrainingListNew.Add(_hrsMemberComplianceTraining)
                        DataLib.ScatterValues(_new, _hrsMemberComplianceTraining)
                        _hrsMemberComplianceTraining.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberWork"

            Dim _hrsMemberWorkLogDetailList As New SysLogDetailList

            Dim _removeWorkCount As Integer
            Dim _addWorkCount As Integer
            Dim _editWorkCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberWork In _hrsMemberWorkListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberWork In _hrsMemberWorkList
                    If _new.WorkDetailId = _old.WorkDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeWorkCount = _removeWorkCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberWork In _hrsMemberWorkList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberWork In _hrsMemberWorkListOld
                    If _new.WorkDetailId = _old.WorkDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .WorkPosition <> _old.WorkPosition OrElse .CompanyName <> _old.CompanyName OrElse .MunicipalityId <> _old.MunicipalityId OrElse .CompanyPhoneNumber <> _old.CompanyPhoneNumber OrElse .WorkStartDate <> _old.WorkStartDate OrElse .WorkEndDate <> _old.WorkEndDate OrElse .ReasonForLeaving <> _old.ReasonForLeaving OrElse .WorkFileName <> _old.WorkFileName OrElse .WorkGUID <> _old.WorkGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addWorkCount = _addWorkCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editWorkCount = _editWorkCount + 1
                End If

            Next

            Dim _hrsMemberWorkListNew As New HrsMemberWorkList      ' for adding new Trx Details

            If _addWorkCount > 0 Then
                Dim _hrsMemberWork As HrsMemberWork
                For Each _new As HrsMemberWork In _hrsMemberWorkList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberWork = New HrsMemberWork
                        _hrsMemberWorkListNew.Add(_hrsMemberWork)
                        DataLib.ScatterValues(_new, _hrsMemberWork)
                        _hrsMemberWork.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberAffiliation"

            Dim _hrsMemberAffiliationLogDetailList As New SysLogDetailList

            Dim _removeAffiliationCount As Integer
            Dim _addAffiliationCount As Integer
            Dim _editAffiliationCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberAffiliation In _hrsMemberAffiliationListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationList
                    If _new.AffiliationDetailId = _old.AffiliationDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeAffiliationCount = _removeAffiliationCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberAffiliation In _hrsMemberAffiliationListOld
                    If _new.AffiliationDetailId = _old.AffiliationDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .AffiliationId <> _old.AffiliationId OrElse .AffiliationDate <> _old.AffiliationDate OrElse .AffiliationPosition <> _old.AffiliationPosition OrElse .AffiliationDescription <> _old.AffiliationDescription OrElse .AffiliationFileName <> _old.AffiliationFileName OrElse .AffiliationGUID <> _old.AffiliationGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addAffiliationCount = _addAffiliationCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editAffiliationCount = _editAffiliationCount + 1
                End If

            Next

            Dim _hrsMemberAffiliationListNew As New HrsMemberAffiliationList      ' for adding new Trx Details

            If _addAffiliationCount > 0 Then
                Dim _hrsMemberAffiliation As HrsMemberAffiliation
                For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberAffiliation = New HrsMemberAffiliation
                        _hrsMemberAffiliationListNew.Add(_hrsMemberAffiliation)
                        DataLib.ScatterValues(_new, _hrsMemberAffiliation)
                        _hrsMemberAffiliation.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region

#Region "HrsMemberMedicalResultType"

            Dim _hrsMemberMedicalResultTypeLogDetailList As New SysLogDetailList

            Dim _removeMedicalResultTypeCount As Integer
            Dim _addMedicalResultTypeCount As Integer
            Dim _editMedicalResultTypeCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeList
                    If _new.MedicalResultTypeDetailId = _old.MedicalResultTypeDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeMedicalResultTypeCount = _removeMedicalResultTypeCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeList
                _new.LogActionId = LogActionId.Add
                For Each _old As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeListOld
                    If _new.MedicalResultTypeDetailId = _old.MedicalResultTypeDetailId Then
                        _new.LogActionId = 0   ' don't add

                        With _new

                            If .MedicalResultTypeId <> _old.MedicalResultTypeId OrElse .Remarks <> _old.Remarks OrElse .MedicalResultTypeFileName <> _old.MedicalResultTypeFileName OrElse .MedicalResultTypeGUID <> _old.MedicalResultTypeGUID Then
                                .LogActionId = LogActionId.Edit
                            End If
                        End With

                        Exit For
                    End If
                Next

                If _new.LogActionId = LogActionId.Add Then
                    _addMedicalResultTypeCount = _addMedicalResultTypeCount + 1
                End If

                If _new.LogActionId = LogActionId.Edit Then
                    _editMedicalResultTypeCount = _editMedicalResultTypeCount + 1
                End If

            Next

            Dim _hrsMemberMedicalResultTypeListNew As New HrsMemberMedicalResultTypeList      ' for adding new Trx Details

            If _addMedicalResultTypeCount > 0 Then
                Dim _hrsMemberMedicalResultType As HrsMemberMedicalResultType
                For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeList
                    If _new.LogActionId = LogActionId.Add Then
                        _hrsMemberMedicalResultType = New HrsMemberMedicalResultType
                        _hrsMemberMedicalResultTypeListNew.Add(_hrsMemberMedicalResultType)
                        DataLib.ScatterValues(_new, _hrsMemberMedicalResultType)
                        _hrsMemberMedicalResultType.MemberId = member.MemberId
                    End If
                Next

            End If

#End Region


            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.UpdateHrsMember(_hrsMember)

                If _hrsMemberLogDetailList.Count > 0 Then

                    With _logKeyList
                        .Clear()
                        .Add("MemberId", _hrsMember.MemberId)
                    End With

                    _id = AppLib.CreateLogHeader("InsHrsMemberLog", _logKeyList, LogActionId.Edit, currentUserId)
                    AppLib.AssignLogHeaderId(_id, _hrsMemberLogDetailList)
                    AppLib.CreateLogDetails(_hrsMemberLogDetailList, "HrsMemberLogDetail")

                End If

                '
#Region "HrsMemberLanguage"
                '
                If _removeLanguageCount > 0 Then
                    Me.DeleteHrsMemberLanguageDetails(_hrsMemberLanguageListOld)
                    For Each _old As HrsMemberLanguage In _hrsMemberLanguageListOld
                        If _old.LogActionId = LogActionId.Delete Then
                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With
                            _id = AppLib.CreateLogHeader("InsHrsMemberLanguageLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberLanguageLogDetailList.Clear()

                            With _old

                                Dim _languageName As String = EasSession.DbsLanguage.Rows.Find(Function(m) m.LanguageId = .LanguageId).LanguageName
                                AppLib.AddLogDetail(_hrsMemberLanguageLogDetailList, _id, LogColumnId.LanguageId, String.Empty, .LanguageId.ToString, .LanguageId.ToString + "=" + _languageName)
                            End With

                            AppLib.CreateLogDetails(_hrsMemberLanguageLogDetailList, "HrsMemberLanguageLogDetail")

                        End If
                    Next

                End If

                If _addLanguageCount > 0 Then
                    Me.InsertHrsMemberLanguages(_hrsMemberLanguageListNew)

                    For Each _new As HrsMemberLanguage In _hrsMemberLanguageListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberLanguageLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberLanguageLogDetailList.Clear()

                        With _new
                            Dim _languageName As String = EasSession.DbsLanguage.Rows.Find(Function(m) m.LanguageId = .LanguageId).LanguageName
                            AppLib.AddLogDetail(_hrsMemberLanguageLogDetailList, _id, LogColumnId.LanguageId, String.Empty, .LanguageId.ToString, .LanguageId.ToString + "=" + _languageName)
                            AppLib.CreateLogDetails(_hrsMemberLanguageLogDetailList, "HrsMemberLanguageLogDetail")
                        End With
                    Next

                End If

                If _editLanguageCount > 0 Then
                    Me.UpdateHrsMemberLanguages(_hrsMemberLanguageList)

                    For Each _new As HrsMemberLanguage In _hrsMemberLanguageList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberLanguageLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberLanguageLogDetailList.Clear()

                            For Each _old As HrsMemberLanguage In _hrsMemberLanguageListOld
                                If _new.LanguageDetailId = _old.LanguageDetailId Then

                                    With _new

                                        If .LanguageId <> _old.LanguageId Then
                                            Dim _oldLanguageName As String = EasSession.DbsLanguage.Rows.Find(Function(m) m.LanguageId = _old.LanguageId).LanguageName
                                            Dim _newLanguageName As String = EasSession.DbsLanguage.Rows.Find(Function(m) m.LanguageId = .LanguageId).LanguageName

                                            AppLib.AddLogDetail(_hrsMemberLanguageLogDetailList, _id, LogColumnId.LanguageId, .LanguageId.ToString, _old.LanguageId.ToString, .LanguageId.ToString + "=" + _oldLanguageName + "; " + .LanguageId.ToString + "=" + _newLanguageName)
                                        End If


                                        AppLib.CreateLogDetails(_hrsMemberLanguageLogDetailList, "HrsMemberLanguageLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If
#End Region

#Region "HrsMemberDisability"

                If _removeDisabilityCount > 0 Then
                    Me.DeleteHrsMemberDisabilityDetails(_hrsMemberDisabilityListOld)
                    For Each _old As HrsMemberDisability In _hrsMemberDisabilityListOld
                        If _old.LogActionId = LogActionId.Delete Then
                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With
                            _id = AppLib.CreateLogHeader("InsHrsMemberDisabilityLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberDisabilityLogDetailList.Clear()

                            With _old
                                AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.Remarks, .Remarks, String.Empty)

                                Dim _disabilityName As String = EasSession.DbsDisability.Rows.Find(Function(m) m.DisabilityId = .DisabilityId).DisabilityName
                                AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.DisabilityId, String.Empty, .DisabilityId.ToString, .DisabilityId.ToString + "=" + _disabilityName)
                            End With

                            AppLib.CreateLogDetails(_hrsMemberDisabilityLogDetailList, "HrsMemberDisabilityLogDetail")

                        End If
                    Next

                End If

                If _addDisabilityCount > 0 Then
                    Me.InsertHrsMemberDisabilities(_hrsMemberDisabilityListNew)

                    For Each _new As HrsMemberDisability In _hrsMemberDisabilityListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberDisabilityLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberDisabilityLogDetailList.Clear()

                        With _new

                            AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.Remarks, String.Empty, .Remarks)

                            Dim _disabilityName As String = EasSession.DbsDisability.Rows.Find(Function(m) m.DisabilityId = .DisabilityId).DisabilityName
                            AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.DisabilityId, String.Empty, .DisabilityId.ToString, .DisabilityId.ToString + "=" + _disabilityName)

                            AppLib.CreateLogDetails(_hrsMemberDisabilityLogDetailList, "HrsMemberDisabilityLogDetail")

                        End With
                    Next

                End If

                If _editDisabilityCount > 0 Then
                    Me.UpdateHrsMemberDisabilities(_hrsMemberDisabilityList)

                    For Each _new As HrsMemberDisability In _hrsMemberDisabilityList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberDisabilityLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberDisabilityLogDetailList.Clear()

                            For Each _old As HrsMemberDisability In _hrsMemberDisabilityListOld
                                If _new.DisabilityDetailId = _old.DisabilityDetailId Then

                                    With _new

                                        If .DisabilityId <> _old.DisabilityId Then
                                            Dim _oldDisabilityName As String = EasSession.DbsDisability.Rows.Find(Function(m) m.DisabilityId = _old.DisabilityId).DisabilityName
                                            Dim _newDisabilityName As String = EasSession.DbsDisability.Rows.Find(Function(m) m.DisabilityId = .DisabilityId).DisabilityName

                                            AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.DisabilityId, .DisabilityId.ToString, _old.DisabilityId.ToString, .DisabilityId.ToString + "=" + _oldDisabilityName + "; " + .DisabilityId.ToString + "=" + _newDisabilityName)
                                        End If

                                        If .Remarks <> _old.Remarks Then
                                            AppLib.AddLogDetail(_hrsMemberDisabilityLogDetailList, _id, LogColumnId.Remarks, _old.Remarks, .Remarks)
                                        End If


                                        AppLib.CreateLogDetails(_hrsMemberDisabilityLogDetailList, "HrsMemberDisabilityLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If

#End Region

#Region "HrsMemberRnrRecording"

                If _removeRnrRecordingCount > 0 Then
                    Me.DeleteHrsMemberRnrRecordingDetails(_hrsMemberRnrRecordingListOld)

                    For Each _old As HrsMemberRnrRecording In _hrsMemberRnrRecordingListOld
                        If _old.LogActionId = LogActionId.Delete Then
                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With
                            _id = AppLib.CreateLogHeader("InsHrsMemberRnrRecordingLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberRnrRecordingLogDetailList.Clear()

                            With _old
                                'AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumber, .RnrNumber.ToString, String.Empty)

                                Dim _rnrRecordingName As String = EasSession.DbsRnrRecording.Rows.Find(Function(m) m.RnrRecordingId = .RnrRecordingId).RnrRecordingName
                                AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrRecordingId, String.Empty, .RnrRecordingId.ToString, .RnrRecordingId.ToString + "=" + _rnrRecordingName)
                            End With

                            With _old
                                Dim _rnrNumber As String = EasSession.DbsRnrNumber.Rows.Find(Function(m) m.RnrNumberId = m.RnrNumberId).RnrNumber.ToString
                                AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumberId, String.Empty, .RnrNumberId.ToString, .RnrNumberId.ToString + "=" + _rnrNumber)
                            End With

                            AppLib.CreateLogDetails(_hrsMemberRnrRecordingLogDetailList, "HrsMemberRnrRecordingLogDetail")

                        End If
                    Next

                End If

                If _addRnrRecordingCount > 0 Then
                    Me.InsertHrsMemberRnrRecordings(_hrsMemberRnrRecordingListNew)

                    For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberRnrRecordingLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberRnrRecordingLogDetailList.Clear()

                        With _new
                            'AppLib.AddLogDetail(_payTrxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

                            'AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumber, String.Empty, .RnrNumber.ToString)
                            'AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, "", .EducationLevelId.ToString)
                            Dim _rnrRecordingName As String = EasSession.DbsRnrRecording.Rows.Find(Function(m) m.RnrRecordingId = .RnrRecordingId).RnrRecordingName
                            AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrRecordingId, String.Empty, .RnrRecordingId.ToString, .RnrRecordingId.ToString + "=" + _rnrRecordingName)

                            Dim _rnrNumber As String = EasSession.DbsRnrNumber.Rows.Find(Function(m) m.RnrNumberId = .RnrNumberId).RnrNumber.ToString
                            AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumberId, String.Empty, .RnrNumberId.ToString, .RnrNumberId.ToString + "=" + _rnrNumber)

                            AppLib.CreateLogDetails(_hrsMemberRnrRecordingLogDetailList, "HrsMemberRnrRecordingLogDetail")

                        End With
                    Next

                End If

                If _editRnrRecordingCount > 0 Then
                    Me.UpdateHrsMemberRnrRecordings(_hrsMemberRnrRecordingList)

                    For Each _new As HrsMemberRnrRecording In _hrsMemberRnrRecordingList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberRnrRecordingLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberRnrRecordingLogDetailList.Clear()

                            For Each _old As HrsMemberRnrRecording In _hrsMemberRnrRecordingListOld
                                If _new.RnrRecordingDetailId = _old.RnrRecordingDetailId Then

                                    With _new
                                        'If .RnrNumber <> _old.RnrNumber Then
                                        '    AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumber, _old.RnrNumber.ToString, .RnrNumber.ToString)
                                        'End If

                                        If .RnrRecordingId <> _old.RnrRecordingId Then
                                            Dim _oldRnrRecordingName As String = EasSession.DbsRnrRecording.Rows.Find(Function(m) m.RnrRecordingId = _old.RnrRecordingId).RnrRecordingName
                                            Dim _newRnrRecordingName As String = EasSession.DbsRnrRecording.Rows.Find(Function(m) m.RnrRecordingId = .RnrRecordingId).RnrRecordingName

                                            AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrRecordingId, .RnrRecordingId.ToString, _old.RnrRecordingId.ToString, .RnrRecordingId.ToString + "=" + _oldRnrRecordingName + "; " + .RnrRecordingId.ToString + "=" + _newRnrRecordingName)
                                        End If

                                        If .RnrNumberId <> _old.RnrNumberId Then
                                            Dim _oldRnrNumber As String = EasSession.DbsRnrNumber.Rows.Find(Function(m) m.RnrNumberId = _old.RnrNumberId).RnrNumber.ToString
                                            Dim _newRnrNumber As String = EasSession.DbsRnrNumber.Rows.Find(Function(m) m.RnrNumberId = .RnrNumberId).RnrNumber.ToString

                                            AppLib.AddLogDetail(_hrsMemberRnrRecordingLogDetailList, _id, LogColumnId.RnrNumberId, .RnrNumberId.ToString, _old.RnrNumberId.ToString, .RnrRecordingId.ToString + "=" + _oldRnrNumber + "; " + .RnrNumberId.ToString + "=" + _newRnrNumber)
                                        End If


                                        AppLib.CreateLogDetails(_hrsMemberRnrRecordingLogDetailList, "HrsMemberRnrRecordingLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If

#End Region

#Region "HrsMemberVaccine"

                If _removeVaccineCount > 0 Then
                    Me.DeleteHrsMemberVaccineDetails(_hrsMemberVaccineListOld)

                    For Each _old As HrsMemberVaccine In _hrsMemberVaccineListOld
                        If _old.LogActionId = LogActionId.Delete Then
                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With
                            _id = AppLib.CreateLogHeader("InsHrsMemberVaccineLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberVaccineLogDetailList.Clear()

                            With _old
                                AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineName, .VaccineName.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineDate, .VaccineDate.ToDisplayFormat, String.Empty)

                                Dim _vaccineTypeName As String = EasSession.DbsVaccineType.Rows.Find(Function(m) m.VaccineTypeId = .VaccineTypeId).VaccineTypeName
                                AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineTypeId, String.Empty, .VaccineTypeId.ToString, .VaccineTypeId.ToString + "=" + _vaccineTypeName)
                            End With

                            AppLib.CreateLogDetails(_hrsMemberVaccineLogDetailList, "HrsMemberVaccineLogDetail")

                        End If
                    Next

                End If

                If _addVaccineCount > 0 Then
                    Me.InsertHrsMemberVaccines(_hrsMemberVaccineListNew)

                    For Each _new As HrsMemberVaccine In _hrsMemberVaccineListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberVaccineLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberVaccineLogDetailList.Clear()

                        With _new
                            'AppLib.AddLogDetail(_payTrxDetailLogDetailList, _id, LogColumnId.AccountId, "", .AccountId)

                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineName, String.Empty, .VaccineName.ToString)
                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineDate, String.Empty, .VaccineDate.ToDisplayFormat)

                            'AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, "", .EducationLevelId.ToString)
                            Dim _vaccineTypeName As String = EasSession.DbsVaccineType.Rows.Find(Function(m) m.VaccineTypeId = .VaccineTypeId).VaccineTypeName
                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineTypeId, String.Empty, .VaccineTypeId.ToString, .VaccineTypeId.ToString + "=" + _vaccineTypeName)

                            AppLib.CreateLogDetails(_hrsMemberVaccineLogDetailList, "HrsMemberVaccineLogDetail")

                        End With
                    Next

                End If

                If _editVaccineCount > 0 Then
                    Me.UpdateHrsMemberVaccines(_hrsMemberVaccineList)

                    For Each _new As HrsMemberVaccine In _hrsMemberVaccineList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMembervaccineLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberVaccineLogDetailList.Clear()

                            For Each _old As HrsMemberVaccine In _hrsMemberVaccineListOld
                                If _new.VaccineDetailId = _old.VaccineDetailId Then

                                    With _new
                                        If .VaccineName <> _old.VaccineName Then
                                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineName, _old.VaccineName.ToString, .VaccineName.ToString)
                                        End If

                                        If .VaccineDate <> _old.VaccineDate Then
                                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineDate, _old.VaccineDate.ToString, .VaccineDate.ToDisplayFormat)
                                        End If

                                        If .VaccineTypeId <> _old.VaccineTypeId Then
                                            Dim _oldVaccineTypeName As String = EasSession.DbsVaccineType.Rows.Find(Function(m) m.VaccineTypeId = _old.VaccineTypeId).VaccineTypeName
                                            Dim _newVaccineTypeName As String = EasSession.DbsVaccineType.Rows.Find(Function(m) m.VaccineTypeId = .VaccineTypeId).VaccineTypeName

                                            AppLib.AddLogDetail(_hrsMemberVaccineLogDetailList, _id, LogColumnId.VaccineTypeId, .VaccineTypeId.ToString, _old.VaccineTypeId.ToString, .VaccineTypeId.ToString + "=" + _oldVaccineTypeName + "; " + .VaccineTypeId.ToString + "=" + _newVaccineTypeName)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberVaccineLogDetailList, "HrsMemberVaccineLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If

#End Region

#Region "HrsMemberSkillSet"

                If _removeSkillSetCount > 0 Then
                    Me.DeleteHrsMemberSkillSetDetails(_hrsMemberSkillSetListOld)
                    For Each _old As HrsMemberSkillSet In _hrsMemberSkillSetListOld
                        If _old.LogActionId = LogActionId.Delete Then
                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                                '.Add("AccountId", _old.AccountId)
                            End With
                            _id = AppLib.CreateLogHeader("InsHrsMemberSkillSetLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberSkillSetLogDetailList.Clear()

                            With _old
                                'AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.AccountId, .AccountId, "")

                                Dim _skillSetName As String = EasSession.DbsSkillSetDetailLog.Rows.Find(Function(m) m.SkillDetailId = .SkillDetailId).SkillDetailName
                                AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, String.Empty, .SkillDetailId.ToString, .SkillDetailId.ToString + "=" + _skillSetName)

                                AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.Remarks, .Remarks, "")

                            End With

                            AppLib.CreateLogDetails(_hrsMemberSkillSetLogDetailList, "HrsMemberSkillSetLogDetail")

                        End If
                    Next

                End If

                If _addSkillSetCount > 0 Then

                    Me.InsertHrsMemberSkillSets(_hrsMemberSkillSetListNew)

                    For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberSkillSetLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberSkillSetLogDetailList.Clear()

                        With _new


                            Dim _skillSetName As String = EasSession.DbsSkillSetDetailLog.Rows.Find(Function(m) m.SkillDetailId = .SkillDetailId).SkillDetailName
                            AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, String.Empty, .SkillDetailId.ToString, .SkillDetailId.ToString + "=" + _skillSetName)

                            AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.Remarks, .Remarks, "")

                            AppLib.CreateLogDetails(_hrsMemberSkillSetLogDetailList, "HrsMemberSkillSetLogDetail")

                        End With
                    Next

                End If

                If _editSkillSetCount > 0 Then
                    Me.UpdateHrsMemberSkillSets(_hrsMemberSkillSetList)

                    For Each _new As HrsMemberSkillSet In _hrsMemberSkillSetList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberSkillSetLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberSkillSetLogDetailList.Clear()

                            For Each _old As HrsMemberSkillSet In _hrsMemberSkillSetListOld
                                If _new.SkillSetDetailId = _old.SkillSetDetailId Then

                                    With _new
                                        'If .SkillDetailId <> _old.SkillDetailId Then
                                        '    AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, _old.SkillDetailId.ToString, .SkillDetailId.ToString)
                                        'End If

                                        If .Remarks <> _old.Remarks Then
                                            AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.Remarks, _old.Remarks, .Remarks)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberSkillSetLogDetailList, "HrsMemberSkillSetLogDetail")

                                        If .SkillDetailId <> _old.SkillDetailId Then
                                            Dim _oldSkillSetName As String = EasSession.DbsSkillSetDetailLog.Rows.Find(Function(m) m.SkillDetailId = _old.SkillDetailId).SkillDetailName
                                            Dim _newSkillSetName As String = EasSession.DbsSkillSetDetailLog.Rows.Find(Function(m) m.SkillDetailId = .SkillDetailId).SkillDetailName

                                            AppLib.AddLogDetail(_hrsMemberSkillSetLogDetailList, _id, LogColumnId.SkillDetailId, .SkillDetailId.ToString, _old.SkillDetailId.ToString, .SkillDetailId.ToString + "=" + _oldSkillSetName + "; " + .SkillDetailId.ToString + "=" + _newSkillSetName)
                                        End If

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If

#End Region

#Region "HrsMemberKin"

                If _removeKinCount > 0 Then
                    Me.DeleteHrsMemberKinDetails(_hrsMemberKinListOld)
                    For Each _old As HrsMemberKin In _hrsMemberKinListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberKinLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberKinLogDetailList.Clear()

                            With _old

                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinLastName, .KinLastName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFirstName, .KinFirstName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinMiddleName, .KinMiddleName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.EmergencyContactFlag, .EmergencyContactFlag.ToString, String.Empty)
                                Dim _suffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .KinSuffixId).MemberSuffixName
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinSuffixId, String.Empty, .KinSuffixId.ToString, .KinSuffixId.ToString + "=" + _suffixName)


                                Dim _relationName As String = EasSession.DbsRelation.Rows.Find(Function(m) m.RelationId = .RelationId).RelationName
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.RelationId, String.Empty, .RelationId.ToString, .RelationId.ToString + "=" + _relationName)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinPhoneNumber, .KinPhoneNumber.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinOccupation, .KinOccupation.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinEmail, .KinEmail.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFileName, .KinFileName.ToString, String.Empty)
                                'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, .KinGUID.ToString, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .KinFileName, .KinGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberKinLogDetailList, "HrsMemberKinLogDetail")

                        End If
                    Next

                End If

                If _addKinCount > 0 Then

                    Me.InsertHrsMemberKins(_hrsMemberKinListNew)


                    For Each _new As HrsMemberKin In _hrsMemberKinListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberKinLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberKinLogDetailList.Clear()

                        With _new

                     AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinLastName, String.Empty, .KinLastName)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFirstName, String.Empty, .KinFirstName)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinMiddleName, String.Empty, .KinMiddleName)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.EmergencyContactFlag, String.Empty, .EmergencyContactFlag.ToString)


                            Try
                                Dim _suffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .KinSuffixId).MemberSuffixName

                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinSuffixId, String.Empty, .KinSuffixId.ToString, .KinSuffixId.ToString + "=" + _suffixName)

                            Catch ex As Exception

                            End Try


                            Dim _relationName As String = EasSession.DbsRelation.Rows.Find(Function(m) m.RelationId = .RelationId).RelationName

                     AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.RelationId, String.Empty, .RelationId.ToString, .RelationId.ToString + "=" + _relationName)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinPhoneNumber, String.Empty, .KinPhoneNumber)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinOccupation, String.Empty, .KinOccupation)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinEmail, String.Empty, .KinEmail)
                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFileName, String.Empty, .KinFileName)
                            'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, String.Empty, .KinGUID)

                            AppLib.CreateLogDetails(_hrsMemberKinLogDetailList, "HrsMemberKinLogDetail")

                  End With
                    Next

                End If

                If _editKinCount > 0 Then
                    Me.UpdateHrsMemberKins(_hrsMemberKinList)

                    For Each _new As HrsMemberKin In _hrsMemberKinList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberKinLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberKinLogDetailList.Clear()

                            For Each _old As HrsMemberKin In _hrsMemberKinListOld
                                If _new.KinDetailId = _old.KinDetailId Then

                                    With _new

                                        If .KinLastName <> _old.KinLastName Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinLastName, _old.KinLastName, .KinLastName)
                                        End If


                                        If .KinFirstName <> _old.KinFirstName Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFirstName, _old.KinFirstName, .KinFirstName)
                                        End If


                                        If .KinMiddleName <> _old.KinMiddleName Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinMiddleName, _old.KinMiddleName, .KinMiddleName)
                                        End If

                                        If .EmergencyContactFlag <> _old.EmergencyContactFlag Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.EmergencyContactFlag, _old.EmergencyContactFlag.ToString, .EmergencyContactFlag.ToString)
                                        End If
                                        Try
                                            If .KinSuffixId <> _old.KinSuffixId Then
                                                Dim _oldKinSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = _old.KinSuffixId).MemberSuffixName
                                                Dim _newKinSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .KinSuffixId).MemberSuffixName

                                                AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinSuffixId, .KinSuffixId.ToString, _old.KinSuffixId.ToString, .KinSuffixId.ToString + "=" + _oldKinSuffixName + "; " + .KinSuffixId.ToString + "=" + _newKinSuffixName)
                                            End If
                                        Catch ex As Exception

                                        End Try




                                        If .RelationId <> _old.RelationId Then
                                            Dim _oldRelationName As String = EasSession.DbsRelation.Rows.Find(Function(m) m.RelationId = _old.RelationId).RelationName
                                            Dim _newRelationName As String = EasSession.DbsRelation.Rows.Find(Function(m) m.RelationId = .RelationId).RelationName

                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.RelationId, .RelationId.ToString, _old.RelationId.ToString, .RelationId.ToString + "=" + _oldRelationName + "; " + .RelationId.ToString + "=" + _newRelationName)
                                        End If

                                        If .KinPhoneNumber <> _old.KinPhoneNumber Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinPhoneNumber, _old.KinPhoneNumber, .KinPhoneNumber)
                                        End If

                                        If .KinOccupation <> _old.KinOccupation Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinPhoneNumber, _old.KinOccupation, .KinOccupation)
                                        End If

                                        If .KinEmail <> _old.KinEmail Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinEmail, _old.KinEmail, .KinEmail)
                                        End If

                                        If .KinFileName <> _old.KinFileName Then
                                            AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinFileName, _old.KinFileName, .KinFileName)
                                        End If

                                        If .KinGUID <> _old.KinGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.KinFileName, _old.KinGUID)
                                            UploadMemberFile(.MemberId, .KinFileName, .KinGUID)
                                            'Upload File

                                            'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberKinLogDetailList, "HrsMemberKinLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberDocType"

                If _removeDocTypeCount > 0 Then
                    Me.DeleteHrsMemberDocTypeDetails(_hrsMemberDocTypeListOld)
                    For Each _old As HrsMemberDocType In _hrsMemberDocTypeListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberDocTypeLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberDocTypeLogDetailList.Clear()

                            With _old
                                Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.docTypeId = .DocTypeId).DocTypeName
                                AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)

                                AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, .DocTypeReference, String.Empty)
                                'AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.ExpirationDate, .ExpirationDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, .DocTypeFileName.ToString, String.Empty)
                                'AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeGUID, .DocTypeGUID.ToString, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .DocTypeFileName, .DocTypeGUID)
                                Catch ex As Exception

                                End Try


                            End With



                            AppLib.CreateLogDetails(_hrsMemberDocTypeLogDetailList, "HrsMemberDocTypeLogDetail")

                        End If
                    Next

                End If

                If _addDocTypeCount > 0 Then
                    Me.InsertHrsMemberDocTypes(_hrsMemberDocTypeListNew)

                    For Each _new As HrsMemberDocType In _hrsMemberDocTypeListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberDocTypeLogDetailList.Clear()

                        With _new

                            Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)
                            'AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.ExpirationDate, .ExpirationDate.ToDisplayFormat, String.Empty)
                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, String.Empty, .DocTypeReference)
                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, String.Empty, .DocTypeFileName)

                            AppLib.CreateLogDetails(_hrsMemberDocTypeLogDetailList, "HrsMemberDocTypeLogDetail")

                        End With
                    Next

                End If

                If _editDocTypeCount > 0 Then
                    Me.UpdateHrsMemberDocTypes(_hrsMemberDocTypeList)

                    For Each _new As HrsMemberDocType In _hrsMemberDocTypeList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberDocTypeLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberDocTypeLogDetailList.Clear()

                            For Each _old As HrsMemberDocType In _hrsMemberDocTypeListOld
                                If _new.DocTypeDetailId = _old.DocTypeDetailId Then

                                    With _new

                                        If .DocTypeId <> _old.DocTypeId Then
                                            Dim _oldDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = _old.DocTypeId).DocTypeName
                                            Dim _newDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName

                                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeId, .DocTypeId.ToString, _old.DocTypeId.ToString, .DocTypeId.ToString + "=" + _oldDocTypeName + "; " + .DocTypeId.ToString + "=" + _newDocTypeName)
                                        End If
                                        'If .ExpirationDate <> _old.ExpirationDate Then
                                        '    AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.ExpirationDate, _old.ExpirationDate.ToDisplayFormat, .ExpirationDate.ToDisplayFormat)
                                        'End If
                                        If .DocTypeReference <> _old.DocTypeReference Then
                                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, _old.DocTypeReference, .DocTypeReference)
                                        End If

                                        If .DocTypeFileName <> _old.DocTypeFileName Then
                                            AppLib.AddLogDetail(_hrsMemberDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, _old.DocTypeFileName, .DocTypeFileName)
                                        End If

                                        If .DocTypeGUID <> _old.DocTypeGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.DocTypeFileName, _old.DocTypeGUID)
                                            UploadMemberFile(.MemberId, .DocTypeFileName, .DocTypeGUID)
                                            'Upload File

                                            'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberDocTypeLogDetailList, "HrsMemberDocTypeLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberEducation"

                If _removeEducationCount > 0 Then
                    Me.DeleteHrsMemberEducationDetails(_hrsMemberEducationListOld)
                    For Each _old As HrsMemberEducation In _hrsMemberEducationListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberEducationLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberEducationLogDetailList.Clear()

                            With _old
                                Dim _educationLevelName As String = EasSession.DbsEducationLevel.Rows.Find(Function(m) m.EducationLevelId = .EducationLevelId).EducationLevelName
                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, String.Empty, .EducationLevelId.ToString, .EducationLevelId.ToString + "=" + _educationLevelName)

                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.SchoolName, .SchoolName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.CourseName, .CourseName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduStartYear, .EduStartYear.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduEndYear, .EduEndYear.ToString, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduFileName, .EduFileName, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .EduFileName, .EduGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberEducationLogDetailList, "HrsMemberEducationLogDetail")

                        End If
                    Next

                End If

                If _addEducationCount > 0 Then

                    Me.InsertHrsMemberEducations(_hrsMemberEducationListNew)

                    For Each _new As HrsMemberEducation In _hrsMemberEducationListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberEducationLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberEducationLogDetailList.Clear()

                        With _new

                            Dim _educationLevelName As String = EasSession.DbsEducationLevel.Rows.Find(Function(m) m.EducationLevelId = .EducationLevelId).EducationLevelName
                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, String.Empty, .EducationLevelId.ToString, .EducationLevelId.ToString + "=" + _educationLevelName)

                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.SchoolName, String.Empty, .SchoolName)
                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.CourseName, String.Empty, .CourseName)
                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduStartYear, String.Empty, .EduStartYear.ToString)
                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduEndYear, String.Empty, .EduEndYear.ToString)
                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduFileName, String.Empty, .EduFileName)

                            AppLib.CreateLogDetails(_hrsMemberEducationLogDetailList, "HrsMemberEducationLogDetail")

                        End With
                    Next

                End If

                If _editEducationCount > 0 Then
                    Me.UpdateHrsMemberEducations(_hrsMemberEducationList)

                    For Each _new As HrsMemberEducation In _hrsMemberEducationList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberEducationLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberEducationLogDetailList.Clear()


                            For Each _old As HrsMemberEducation In _hrsMemberEducationListOld
                                If _new.EduDetailId = _old.EduDetailId Then

                                    With _new

                                        If .EducationLevelId <> _old.EducationLevelId Then
                                            Dim _oldEducationLevelName As String = EasSession.DbsEducationLevel.Rows.Find(Function(m) m.EducationLevelId = _old.EducationLevelId).EducationLevelName
                                            Dim _newEducationLevelName As String = EasSession.DbsEducationLevel.Rows.Find(Function(m) m.EducationLevelId = .EducationLevelId).EducationLevelName

                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EducationLevelId, .EducationLevelId.ToString, _old.EducationLevelId.ToString, .EducationLevelId.ToString + "=" + _oldEducationLevelName + "; " + .EducationLevelId.ToString + "=" + _newEducationLevelName)
                                        End If

                                        If .SchoolName <> _old.SchoolName Then
                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.SchoolName, _old.SchoolName, .SchoolName)
                                        End If


                                        If .CourseName <> _old.CourseName Then
                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.CourseName, _old.CourseName, .CourseName)
                                        End If

                                        If .EduStartYear <> _old.EduStartYear Then
                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduStartYear, _old.EduStartYear.ToString, .EduStartYear.ToString)
                                        End If

                                        If .EduEndYear <> _old.EduEndYear Then
                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduEndYear, _old.EduEndYear.ToString, .EduEndYear.ToString)
                                        End If

                                        If .EduFileName <> _old.EduFileName Then
                                            AppLib.AddLogDetail(_hrsMemberEducationLogDetailList, _id, LogColumnId.EduFileName, _old.EduFileName, .EduFileName)
                                        End If

                                        If .EduGUID <> _old.EduGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.EduFileName, _old.EduGUID)
                                            UploadMemberFile(.MemberId, .EduFileName, .EduGUID)
                                            'Upload File

                                            'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberEducationLogDetailList, "HrsMemberEducationLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberLicenseProfession"

                If _removeLicenseProfessionCount > 0 Then
                    Me.DeleteHrsMemberLicenseProfessionDetails(_hrsMemberLicenseProfessionListOld)
                    For Each _old As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberLicenseProfessionLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberLicenseProfessionLogDetailList.Clear()

                            With _old
                                Dim _licenseProfessionName As String = EasSession.DbsLicenseProfession.Rows.Find(Function(m) m.LicenseProfessionId = .LicenseProfessionId).LicenseProfessionName
                                AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseProfessionId, String.Empty, .LicenseProfessionId.ToString, .LicenseProfessionId.ToString + "=" + _licenseProfessionName)

                                AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.PRCIdNo, .PRCIdNo, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseFileName, .LicenseFileName, String.Empty)
                                Try
                                    RemoveMemberFile(memberId, .LicenseFileName, .LicenseGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberLicenseProfessionLogDetailList, "HrsMemberLicenseProfessionLogDetail")

                        End If
                    Next

                End If

                If _addLicenseProfessionCount > 0 Then
                    Me.InsertHrsMemberLicenseProfessions(_hrsMemberLicenseProfessionListNew)

                    For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberLicenseProfessionLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberLicenseProfessionLogDetailList.Clear()

                        With _new

                            Dim _licenseProfessionName As String = EasSession.DbsLicenseProfession.Rows.Find(Function(m) m.LicenseProfessionId = .LicenseProfessionId).LicenseProfessionName
                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseProfessionId, String.Empty, .LicenseProfessionId.ToString, .LicenseProfessionId.ToString + "=" + _licenseProfessionName)

                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.PRCIdNo, String.Empty, .PRCIdNo)
                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseFileName, String.Empty, .LicenseFileName)

                            AppLib.CreateLogDetails(_hrsMemberLicenseProfessionLogDetailList, "HrsMemberLicenseProfessionLogDetail")

                        End With
                    Next

                End If

                If _editLicenseProfessionCount > 0 Then
                    Me.UpdateHrsMemberLicenseProfessions(_hrsMemberLicenseProfessionList)

                    For Each _new As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberLicenseProfessionLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberLicenseProfessionLogDetailList.Clear()


                            For Each _old As HrsMemberLicenseProfession In _hrsMemberLicenseProfessionListOld
                                If _new.LicenseProfessionDetailId = _old.LicenseProfessionDetailId Then

                                    With _new

                                        If .LicenseProfessionId <> _old.LicenseProfessionId Then
                                            Dim _oldLicenseProfessionName As String = EasSession.DbsLicenseProfession.Rows.Find(Function(m) m.LicenseProfessionId = _old.LicenseProfessionId).LicenseProfessionName
                                            Dim _newLicenseProfessionName As String = EasSession.DbsLicenseProfession.Rows.Find(Function(m) m.LicenseProfessionId = .LicenseProfessionId).LicenseProfessionName

                                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseProfessionId, .LicenseProfessionId.ToString, _old.LicenseProfessionId.ToString, .LicenseProfessionId.ToString + "=" + _oldLicenseProfessionName + "; " + .LicenseProfessionId.ToString + "=" + _newLicenseProfessionName)
                                        End If

                                        If .PRCIdNo <> _old.PRCIdNo Then
                                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.PRCIdNo, _old.PRCIdNo, .PRCIdNo)
                                        End If

                                        If .LicenseFileName <> _old.LicenseFileName Then
                                            AppLib.AddLogDetail(_hrsMemberLicenseProfessionLogDetailList, _id, LogColumnId.LicenseFileName, _old.LicenseFileName, .LicenseFileName)
                                        End If

                                        If .LicenseGUID <> _old.LicenseGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.LicenseFileName, _old.LicenseGUID)
                                            UploadMemberFile(.MemberId, .LicenseFileName, .LicenseGUID)
                                            'Upload File

                                            'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberLicenseProfessionLogDetailList, "HrsMemberLicenseProfessionLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberCDATraining"

                If _removeCDATrainingCount > 0 Then
                    Me.DeleteHrsMemberCDATrainingDetails(_hrsMemberCDATrainingListOld)
                    For Each _old As HrsMemberCDATraining In _hrsMemberCDATrainingListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberCDATrainingLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberCDATrainingLogDetailList.Clear()

                            With _old
                                Dim _cDATrainingName As String = EasSession.DbsCDATraining.Rows.Find(Function(m) m.CDATrainingId = .CDATrainingId).CDATrainingName
                                AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDATrainingId, String.Empty, .CDATrainingId.ToString, .CDATrainingId.ToString + "=" + _cDATrainingName)

                                AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CertificateNumber, .CertificateNumber, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.TrainingDate, .TrainingDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDAFileName, .CDAFileName, String.Empty)

                                AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDAFileName, .CDAFileName, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .CDAFileName, .CDAGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberCDATrainingLogDetailList, "HrsMemberCDATrainingLogDetail")

                        End If
                    Next

                End If

                If _addCDATrainingCount > 0 Then
                    Me.InsertHrsMemberCDATrainings(_hrsMemberCDATrainingListNew)

                    For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberCDATrainingLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberCDATrainingLogDetailList.Clear()

                        With _new

                            Dim _cDATrainingName As String = EasSession.DbsCDATraining.Rows.Find(Function(m) m.CDATrainingId = .CDATrainingId).CDATrainingName
                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDATrainingId, String.Empty, .CDATrainingId.ToString, .CDATrainingId.ToString + "=" + _cDATrainingName)

                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.TrainingDate, String.Empty, .TrainingDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDAFileName, String.Empty, .CDAFileName)

                            AppLib.CreateLogDetails(_hrsMemberCDATrainingLogDetailList, "HrsMemberCDATrainingLogDetail")

                        End With
                    Next

                End If

                If _editCDATrainingCount > 0 Then
                    Me.UpdateHrsMemberCDATrainings(_hrsMemberCDATrainingList)

                    For Each _new As HrsMemberCDATraining In _hrsMemberCDATrainingList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberCDATrainingLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberCDATrainingLogDetailList.Clear()


                            For Each _old As HrsMemberCDATraining In _hrsMemberCDATrainingListOld
                                If _new.CDATrainingDetailId = _old.CDATrainingDetailId Then
                                    With _new

                                        If .CDATrainingId <> _old.CDATrainingId Then
                                            Dim _oldCDATrainingName As String = EasSession.DbsCDATraining.Rows.Find(Function(m) m.CDATrainingId = _old.CDATrainingId).CDATrainingName
                                            Dim _newCDATrainingName As String = EasSession.DbsCDATraining.Rows.Find(Function(m) m.CDATrainingId = .CDATrainingId).CDATrainingName

                                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDATrainingId, .CDATrainingId.ToString, _old.CDATrainingId.ToString, .CDATrainingId.ToString + "=" + _oldCDATrainingName + "; " + .CDATrainingId.ToString + "=" + _newCDATrainingName)
                                        End If

                                        If .CertificateNumber <> _old.CertificateNumber Then
                                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CertificateNumber, _old.CertificateNumber, .CertificateNumber)
                                        End If

                                        If .TrainingDate <> _old.TrainingDate Then
                                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.TrainingDate, _old.TrainingDate.ToDisplayFormat, .TrainingDate.ToDisplayFormat)
                                        End If

                                        If .CDAFileName <> _old.CDAFileName Then
                                            AppLib.AddLogDetail(_hrsMemberCDATrainingLogDetailList, _id, LogColumnId.CDAFileName, _old.CDAFileName, .CDAFileName)
                                        End If

                                        If .CDAGUID <> _old.CDAGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.CDAFileName, _old.CDAGUID)
                                            UploadMemberFile(.MemberId, .CDAFileName, .CDAGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberCDATrainingLogDetailList, "HrsMemberCDATrainingLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberNCIIQualificationTitle"

                If _removeNCIIQualificationTitleCount > 0 Then
                    Me.DeleteHrsMemberNCIIQualificationTitleDetails(_hrsMemberNCIIQualificationTitleListOld)
                    For Each _old As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberNCIIQualificationTitleLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberNCIIQualificationTitleLogDetailList.Clear()

                            With _old
                                Dim _nCIIQualificationTitleName As String = EasSession.DbsNCIIQualificationTitle.Rows.Find(Function(m) m.NCIIQualificationTitleId = .NCIIQualificationTitleId).NCIIQualificationTitleName
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIQualificationTitleId, String.Empty, .NCIIQualificationTitleId.ToString, .NCIIQualificationTitleId.ToString + "=" + _nCIIQualificationTitleName)

                                Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)

                                Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.CertificateNumber, .CertificateNumber, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.IssuanceDate, .IssuanceDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.ValidityDate, .ValidityDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIFileName, .NCIIFileName, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .NCIIFileName, .NCIIGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberNCIIQualificationTitleLogDetailList, "HrsMemberNCIIQualificationTitleLogDetail")

                        End If
                    Next

                End If

                If _addNCIIQualificationTitleCount > 0 Then
                    Me.InsertHrsMemberNCIIQualificationTitles(_hrsMemberNCIIQualificationTitleListNew)

                    For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberNCIIQualificationTitleLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberNCIIQualificationTitleLogDetailList.Clear()

                        With _new

                            Dim _nCIIQualificationTitleName As String = EasSession.DbsNCIIQualificationTitle.Rows.Find(Function(m) m.NCIIQualificationTitleId = .NCIIQualificationTitleId).NCIIQualificationTitleName
                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIQualificationTitleId, String.Empty, .NCIIQualificationTitleId.ToString, .NCIIQualificationTitleId.ToString + "=" + _nCIIQualificationTitleName)

                            Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)

                            Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.IssuanceDate, String.Empty, .IssuanceDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.ValidityDate, String.Empty, .ValidityDate.ToDisplayFormat)

                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIFileName, String.Empty, .NCIIFileName)

                            AppLib.CreateLogDetails(_hrsMemberNCIIQualificationTitleLogDetailList, "HrsMemberNCIIQualificationTitleLogDetail")

                        End With
                    Next

                End If

                If _editNCIIQualificationTitleCount > 0 Then
                    Me.UpdateHrsMemberNCIIQualificationTitles(_hrsMemberNCIIQualificationTitleList)

                    For Each _new As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberNCIIQualificationTitleLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberNCIIQualificationTitleLogDetailList.Clear()


                            For Each _old As HrsMemberNCIIQualificationTitle In _hrsMemberNCIIQualificationTitleListOld
                                If _new.NCIIQualificationTitleDetailId = _old.NCIIQualificationTitleDetailId Then
                                    With _new

                                        If .NCIIQualificationTitleId <> _old.NCIIQualificationTitleId Then
                                            Dim _oldNCIIQualificationTitleName As String = EasSession.DbsNCIIQualificationTitle.Rows.Find(Function(m) m.NCIIQualificationTitleId = _old.NCIIQualificationTitleId).NCIIQualificationTitleName
                                            Dim _newNCIIQualificationTitleName As String = EasSession.DbsNCIIQualificationTitle.Rows.Find(Function(m) m.NCIIQualificationTitleId = .NCIIQualificationTitleId).NCIIQualificationTitleName

                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIQualificationTitleId, .NCIIQualificationTitleId.ToString, _old.NCIIQualificationTitleId.ToString, .NCIIQualificationTitleId.ToString + "=" + _oldNCIIQualificationTitleName + "; " + .NCIIQualificationTitleId.ToString + "=" + _newNCIIQualificationTitleName)
                                        End If

                                        If .TrainingInstitutionId <> _old.TrainingInstitutionId Then
                                            Dim _oldTrainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = _old.TrainingInstitutionId).TrainingInstitutionName
                                            Dim _newTrainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName

                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.TrainingInstitutionId, .TrainingInstitutionId.ToString, _old.TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _oldTrainingInstitutionName + "; " + .TrainingInstitutionId.ToString + "=" + _newTrainingInstitutionName)
                                        End If

                                        If .AssessmentCenterId <> _old.AssessmentCenterId Then
                                            Dim _oldAssessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = _old.AssessmentCenterId).AssessmentCenterName
                                            Dim _newAssessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName

                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.AssessmentCenterId, .AssessmentCenterId.ToString, _old.AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _oldAssessmentCenterName + "; " + .AssessmentCenterId.ToString + "=" + _newAssessmentCenterName)
                                        End If


                                        If .CertificateNumber <> _old.CertificateNumber Then
                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.CertificateNumber, _old.CertificateNumber, .CertificateNumber)
                                        End If

                                        If .IssuanceDate <> _old.IssuanceDate Then
                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.IssuanceDate, _old.IssuanceDate.ToDisplayFormat, .IssuanceDate.ToDisplayFormat)
                                        End If

                                        If .ValidityDate <> _old.ValidityDate Then
                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.ValidityDate, _old.ValidityDate.ToDisplayFormat, .ValidityDate.ToDisplayFormat)
                                        End If

                                        If .NCIIFileName <> _old.NCIIFileName Then
                                            AppLib.AddLogDetail(_hrsMemberNCIIQualificationTitleLogDetailList, _id, LogColumnId.NCIIFileName, _old.NCIIFileName, .NCIIFileName)
                                        End If

                                        If .NCIIGUID <> _old.NCIIGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.NCIIFileName, _old.NCIIGUID)
                                            UploadMemberFile(.MemberId, .NCIIFileName, .NCIIGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberNCIIQualificationTitleLogDetailList, "HrsMemberNCIIQualificationTitleLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberComplianceTraining"

                If _removeComplianceTrainingCount > 0 Then
                    Me.DeleteHrsMemberComplianceTrainingDetails(_hrsMemberComplianceTrainingListOld)
                    For Each _old As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberComplianceTrainingLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberComplianceTrainingLogDetailList.Clear()

                            With _old
                                Dim _nCIIQualificationTitleName As String = EasSession.DbsComplianceTraining.Rows.Find(Function(m) m.ComplianceTrainingId = .ComplianceTrainingId).ComplianceTrainingName
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceTrainingId, String.Empty, .ComplianceTrainingId.ToString, .ComplianceTrainingId.ToString + "=" + _nCIIQualificationTitleName)

                                Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)

                                Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.CertificateNumber, .CertificateNumber, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.IssuanceDate, .IssuanceDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ValidityDate, .ValidityDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceFileName, .ComplianceFileName, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .ComplianceFileName, .ComplianceGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberComplianceTrainingLogDetailList, "HrsMemberComplianceTrainingLogDetail")

                        End If
                    Next

                End If

                If _addComplianceTrainingCount > 0 Then
                    Me.InsertHrsMemberComplianceTrainings(_hrsMemberComplianceTrainingListNew)

                    For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberComplianceTrainingLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberComplianceTrainingLogDetailList.Clear()

                        With _new

                            Dim _nCIIQualificationTitleName As String = EasSession.DbsComplianceTraining.Rows.Find(Function(m) m.ComplianceTrainingId = .ComplianceTrainingId).ComplianceTrainingName
                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceTrainingId, String.Empty, .ComplianceTrainingId.ToString, .ComplianceTrainingId.ToString + "=" + _nCIIQualificationTitleName)

                            Dim _trainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName
                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.TrainingInstitutionId, String.Empty, .TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _trainingInstitutionName)

                            Dim _assessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName
                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.AssessmentCenterId, String.Empty, .AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _assessmentCenterName)

                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.CertificateNumber, String.Empty, .CertificateNumber)
                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.IssuanceDate, String.Empty, .IssuanceDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ValidityDate, String.Empty, .ValidityDate.ToDisplayFormat)

                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceFileName, String.Empty, .ComplianceFileName)

                            AppLib.CreateLogDetails(_hrsMemberComplianceTrainingLogDetailList, "HrsMemberComplianceTrainingLogDetail")

                        End With
                    Next

                End If

                If _editComplianceTrainingCount > 0 Then
                    Me.UpdateHrsMemberComplianceTrainings(_hrsMemberComplianceTrainingList)

                    For Each _new As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberComplianceTrainingLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberComplianceTrainingLogDetailList.Clear()


                            For Each _old As HrsMemberComplianceTraining In _hrsMemberComplianceTrainingListOld
                                If _new.ComplianceTrainingDetailId = _old.ComplianceTrainingDetailId Then
                                    With _new

                                        If .ComplianceTrainingId <> _old.ComplianceTrainingId Then
                                            Dim _oldComplianceTrainingName As String = EasSession.DbsComplianceTraining.Rows.Find(Function(m) m.ComplianceTrainingId = _old.ComplianceTrainingId).ComplianceTrainingName
                                            Dim _newComplianceTrainingName As String = EasSession.DbsComplianceTraining.Rows.Find(Function(m) m.ComplianceTrainingId = .ComplianceTrainingId).ComplianceTrainingName

                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceTrainingId, .ComplianceTrainingId.ToString, _old.ComplianceTrainingId.ToString, .ComplianceTrainingId.ToString + "=" + _oldComplianceTrainingName + "; " + .ComplianceTrainingId.ToString + "=" + _newComplianceTrainingName)
                                        End If

                                        If .TrainingInstitutionId <> _old.TrainingInstitutionId Then
                                            Dim _oldTrainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = _old.TrainingInstitutionId).TrainingInstitutionName
                                            Dim _newTrainingInstitutionName As String = EasSession.DbsTrainingInstitution.Rows.Find(Function(m) m.TrainingInstitutionId = .TrainingInstitutionId).TrainingInstitutionName

                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.TrainingInstitutionId, .TrainingInstitutionId.ToString, _old.TrainingInstitutionId.ToString, .TrainingInstitutionId.ToString + "=" + _oldTrainingInstitutionName + "; " + .TrainingInstitutionId.ToString + "=" + _newTrainingInstitutionName)
                                        End If

                                        If .AssessmentCenterId <> _old.AssessmentCenterId Then
                                            Dim _oldAssessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = _old.AssessmentCenterId).AssessmentCenterName
                                            Dim _newAssessmentCenterName As String = EasSession.DbsAssessmentCenter.Rows.Find(Function(m) m.AssessmentCenterId = .AssessmentCenterId).AssessmentCenterName

                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.AssessmentCenterId, .AssessmentCenterId.ToString, _old.AssessmentCenterId.ToString, .AssessmentCenterId.ToString + "=" + _oldAssessmentCenterName + "; " + .AssessmentCenterId.ToString + "=" + _newAssessmentCenterName)
                                        End If


                                        If .CertificateNumber <> _old.CertificateNumber Then
                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.CertificateNumber, _old.CertificateNumber, .CertificateNumber)
                                        End If

                                        If .IssuanceDate <> _old.IssuanceDate Then
                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.IssuanceDate, _old.IssuanceDate.ToDisplayFormat, .IssuanceDate.ToDisplayFormat)
                                        End If

                                        If .ValidityDate <> _old.ValidityDate Then
                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ValidityDate, _old.ValidityDate.ToDisplayFormat, .ValidityDate.ToDisplayFormat)
                                        End If

                                        If .ComplianceFileName <> _old.ComplianceFileName Then
                                            AppLib.AddLogDetail(_hrsMemberComplianceTrainingLogDetailList, _id, LogColumnId.ComplianceFileName, _old.ComplianceFileName, .ComplianceFileName)
                                        End If

                                        If .ComplianceGUID <> _old.ComplianceGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.ComplianceFileName, _old.ComplianceGUID)
                                            UploadMemberFile(.MemberId, .ComplianceFileName, .ComplianceGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberComplianceTrainingLogDetailList, "HrsMemberComplianceTrainingLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberWork"

                If _removeWorkCount > 0 Then
                    Me.DeleteHrsMemberWorkDetails(_hrsMemberWorkListOld)
                    For Each _old As HrsMemberWork In _hrsMemberWorkListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberWorkLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberWorkLogDetailList.Clear()

                            With _old

                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkPosition, .WorkPosition, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyName, .CompanyName, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, .MunicipalityId, String.Empty)

                                Dim _municipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _municipalityName)


                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyPhoneNumber, .CompanyPhoneNumber, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkStartDate, .WorkStartDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkEndDate, .WorkEndDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.ReasonForLeaving, .ReasonForLeaving, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkFileName, .WorkFileName, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .WorkFileName, .WorkGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberWorkLogDetailList, "HrsMemberWorkLogDetail")

                        End If
                    Next

                End If

                If _addWorkCount > 0 Then
                    Me.InsertHrsMemberWorks(_hrsMemberWorkListNew)

                    For Each _new As HrsMemberWork In _hrsMemberWorkListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberWorkLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberWorkLogDetailList.Clear()

                        With _new


                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkPosition, String.Empty, .WorkPosition)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyName, String.Empty, .CompanyName)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId)

                            Dim _municipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _municipalityName)

                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyPhoneNumber, String.Empty, .CompanyPhoneNumber)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkStartDate, String.Empty, .WorkStartDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkEndDate, String.Empty, .WorkEndDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.ReasonForLeaving, String.Empty, .ReasonForLeaving)
                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkFileName, String.Empty, .WorkFileName)

                            AppLib.CreateLogDetails(_hrsMemberWorkLogDetailList, "HrsMemberWorkLogDetail")

                        End With
                    Next

                End If

                If _editWorkCount > 0 Then
                    Me.UpdateHrsMemberWorks(_hrsMemberWorkList)

                    For Each _new As HrsMemberWork In _hrsMemberWorkList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberWorkLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberWorkLogDetailList.Clear()


                            For Each _old As HrsMemberWork In _hrsMemberWorkListOld
                                If _new.WorkDetailId = _old.WorkDetailId Then
                                    With _new


                                        If .WorkPosition <> _old.WorkPosition Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkPosition, _old.WorkPosition, .WorkPosition)
                                        End If

                                        If .CompanyName <> _old.CompanyName Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyName, _old.CompanyName, .CompanyName)
                                        End If

                                        If .MunicipalityId <> _old.MunicipalityId Then
                                            Dim _oldMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = _old.MunicipalityId).MunicipalityName
                                            Dim _newMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName

                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.MunicipalityId, .MunicipalityId.ToString, _old.MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _oldMunicipalityName + "; " + .MunicipalityId.ToString + "=" + _newMunicipalityName)
                                        End If


                                        If .CompanyPhoneNumber <> _old.CompanyPhoneNumber Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.CompanyPhoneNumber, _old.CompanyPhoneNumber, .CompanyPhoneNumber)
                                        End If

                                        If .WorkStartDate <> _old.WorkStartDate Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkStartDate, _old.WorkStartDate.ToDisplayFormat, .WorkStartDate.ToDisplayFormat)
                                        End If

                                        If .WorkEndDate <> _old.WorkEndDate Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkEndDate, _old.WorkEndDate.ToDisplayFormat, .WorkEndDate.ToDisplayFormat)
                                        End If

                                        If .ReasonForLeaving <> _old.ReasonForLeaving Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.ReasonForLeaving, _old.ReasonForLeaving, .ReasonForLeaving)
                                        End If

                                        If .WorkFileName <> _old.WorkFileName Then
                                            AppLib.AddLogDetail(_hrsMemberWorkLogDetailList, _id, LogColumnId.WorkFileName, _old.WorkFileName, .WorkFileName)
                                        End If

                                        If .WorkGUID <> _old.WorkGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.WorkFileName, _old.WorkGUID)
                                            UploadMemberFile(.MemberId, .WorkFileName, .WorkGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberWorkLogDetailList, "HrsMemberWorkLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberAffiliation"

                If _removeAffiliationCount > 0 Then
                    Me.DeleteHrsMemberAffiliationDetails(_hrsMemberAffiliationListOld)
                    For Each _old As HrsMemberAffiliation In _hrsMemberAffiliationListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberAffiliationLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberKinLogDetailList.Clear()

                            With _old
                                Dim _affiliationName As String = EasSession.DbsAffiliation.Rows.Find(Function(m) m.AffiliationId = .AffiliationId).AffiliationName
                                AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationId, String.Empty, .AffiliationId.ToString, .AffiliationId.ToString + "=" + _affiliationName)

                                AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDate, .AffiliationDate.ToDisplayFormat, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationPosition, .AffiliationPosition, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDescription, .AffiliationDescription, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationFileName, .AffiliationFileName, String.Empty)

                                'AppLib.AddLogDetail(_hrsMemberKinLogDetailList, _id, LogColumnId.KinGUID, .KinGUID.ToString, String.Empty)

                                Try
                                    RemoveMemberFile(memberId, .AffiliationFileName, .AffiliationGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberAffiliationLogDetailList, "HrsMemberAffiliationLogDetail")

                        End If
                    Next

                End If

                If _addAffiliationCount > 0 Then
                    Me.InsertHrsMemberAffiliations(_hrsMemberAffiliationListNew)

                    For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberAffiliationLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberAffiliationLogDetailList.Clear()

                        With _new

                            Dim _affiliationName As String = EasSession.DbsAffiliation.Rows.Find(Function(m) m.AffiliationId = .AffiliationId).AffiliationName
                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationId, String.Empty, .AffiliationId.ToString, .AffiliationId.ToString + "=" + _affiliationName)


                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDate, String.Empty, .AffiliationDate.ToDisplayFormat)
                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationPosition, String.Empty, .AffiliationPosition)
                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDescription, String.Empty, .AffiliationDescription)
                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationFileName, String.Empty, .AffiliationFileName)

                            AppLib.CreateLogDetails(_hrsMemberAffiliationLogDetailList, "HrsMemberAffiliationLogDetail")

                        End With
                    Next

                End If

                If _editAffiliationCount > 0 Then
                    Me.UpdateHrsMemberAffiliations(_hrsMemberAffiliationList)

                    For Each _new As HrsMemberAffiliation In _hrsMemberAffiliationList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberAffiliationLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberAffiliationLogDetailList.Clear()

                            For Each _old As HrsMemberAffiliation In _hrsMemberAffiliationListOld
                                If _new.AffiliationDetailId = _old.AffiliationDetailId Then

                                    With _new

                                        If .AffiliationId <> _old.AffiliationId Then
                                            Dim _oldAffiliationName As String = EasSession.DbsAffiliation.Rows.Find(Function(m) m.AffiliationId = _old.AffiliationId).AffiliationName
                                            Dim _newAffiliationName As String = EasSession.DbsAffiliation.Rows.Find(Function(m) m.AffiliationId = .AffiliationId).AffiliationName

                                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationId, .AffiliationId.ToString, _old.AffiliationId.ToString, .AffiliationId.ToString + "=" + _oldAffiliationName + "; " + .AffiliationId.ToString + "=" + _newAffiliationName)
                                        End If

                                        If .AffiliationDate <> _old.AffiliationDate Then
                                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDate, _old.AffiliationDate.ToDisplayFormat, .AffiliationDate.ToDisplayFormat)
                                        End If

                                        If .AffiliationPosition <> _old.AffiliationPosition Then
                                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationPosition, _old.AffiliationPosition, .AffiliationPosition)
                                        End If

                                        If .AffiliationDescription <> _old.AffiliationDescription Then
                                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationDescription, _old.AffiliationDescription, .AffiliationDescription)
                                        End If

                                        If .AffiliationFileName <> _old.AffiliationFileName Then
                                            AppLib.AddLogDetail(_hrsMemberAffiliationLogDetailList, _id, LogColumnId.AffiliationFileName, _old.AffiliationFileName, .AffiliationFileName)
                                        End If


                                        If .AffiliationGUID <> _old.AffiliationGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.AffiliationFileName, _old.AffiliationGUID)
                                            UploadMemberFile(.MemberId, .AffiliationFileName, .AffiliationGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberAffiliationLogDetailList, "HrsMemberAffiliationLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

#Region "HrsMemberMedicalResultType"

                If _removeMedicalResultTypeCount > 0 Then
                    Me.DeleteHrsMemberMedicalResultTypeDetails(_hrsMemberMedicalResultTypeListOld)
                    For Each _old As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeListOld

                        If _old.LogActionId = LogActionId.Delete Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", memberId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberMedicalResultTypeLog", _logKeyList, LogActionId.Delete, currentUserId)
                            _hrsMemberMedicalResultTypeLogDetailList.Clear()

                            With _old
                                Dim _medicalResultTypeName As String = EasSession.DbsMedicalResultType.Rows.Find(Function(m) m.MedicalResultTypeId = .MedicalResultTypeId).MedicalResultTypeName
                                AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeId, String.Empty, .MedicalResultTypeId.ToString, .MedicalResultTypeId.ToString + "=" + _medicalResultTypeName)

                                AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.Remarks, .Remarks, String.Empty)
                                AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeFileName, .MedicalResultTypeFileName, String.Empty)
                                Try
                                    RemoveMemberFile(memberId, .MedicalResultTypeFileName, .MedicalResultTypeGUID)
                                Catch ex As Exception

                                End Try


                            End With

                            AppLib.CreateLogDetails(_hrsMemberMedicalResultTypeLogDetailList, "HrsMemberMedicalResultTypeLogDetail")

                        End If
                    Next

                End If

                If _addMedicalResultTypeCount > 0 Then
                    Me.InsertHrsMemberMedicalResultTypes(_hrsMemberMedicalResultTypeListNew)

                    For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeListNew

                        With _logKeyList
                            .Clear()
                            .Add("MemberId", _new.MemberId)
                            '.Add("AccountId", _new.AccountId)
                        End With

                        _id = AppLib.CreateLogHeader("InsHrsMemberMedicalResultTypeLog", _logKeyList, LogActionId.Add, currentUserId)

                        _hrsMemberMedicalResultTypeLogDetailList.Clear()

                        With _new

                            Dim _medicalResultTypeName As String = EasSession.DbsMedicalResultType.Rows.Find(Function(m) m.MedicalResultTypeId = .MedicalResultTypeId).MedicalResultTypeName
                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeId, String.Empty, .MedicalResultTypeId.ToString, .MedicalResultTypeId.ToString + "=" + _medicalResultTypeName)

                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.Remarks, String.Empty, .Remarks)
                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeFileName, String.Empty, .MedicalResultTypeFileName)

                            AppLib.CreateLogDetails(_hrsMemberMedicalResultTypeLogDetailList, "HrsMemberMedicalResultTypeLogDetail")

                        End With
                    Next

                End If

                If _editMedicalResultTypeCount > 0 Then
                    Me.UpdateHrsMemberMedicalResultTypes(_hrsMemberMedicalResultTypeList)

                    For Each _new As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeList
                        If _new.LogActionId = LogActionId.Edit Then

                            With _logKeyList
                                .Clear()
                                .Add("MemberId", _new.MemberId)
                                '.Add("AccountId", _new.AccountId)
                            End With

                            _id = AppLib.CreateLogHeader("InsHrsMemberMedicalResultTypeLog", _logKeyList, LogActionId.Edit, currentUserId)

                            _hrsMemberMedicalResultTypeLogDetailList.Clear()


                            For Each _old As HrsMemberMedicalResultType In _hrsMemberMedicalResultTypeListOld
                                If _new.MedicalResultTypeDetailId = _old.MedicalResultTypeDetailId Then

                                    With _new

                                        If .MedicalResultTypeId <> _old.MedicalResultTypeId Then
                                            Dim _oldMedicalResultTypeName As String = EasSession.DbsMedicalResultType.Rows.Find(Function(m) m.MedicalResultTypeId = _old.MedicalResultTypeId).MedicalResultTypeName
                                            Dim _newMedicalResultTypeName As String = EasSession.DbsMedicalResultType.Rows.Find(Function(m) m.MedicalResultTypeId = .MedicalResultTypeId).MedicalResultTypeName

                                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeId, .MedicalResultTypeId.ToString, _old.MedicalResultTypeId.ToString, .MedicalResultTypeId.ToString + "=" + _oldMedicalResultTypeName + "; " + .MedicalResultTypeId.ToString + "=" + _newMedicalResultTypeName)
                                        End If

                                        If .Remarks <> _old.Remarks Then
                                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.Remarks, _old.Remarks, .Remarks)
                                        End If

                                        If .MedicalResultTypeFileName <> _old.MedicalResultTypeFileName Then
                                            AppLib.AddLogDetail(_hrsMemberMedicalResultTypeLogDetailList, _id, LogColumnId.MedicalResultTypeFileName, _old.MedicalResultTypeFileName, .MedicalResultTypeFileName)
                                        End If

                                        If .MedicalResultTypeGUID <> _old.MedicalResultTypeGUID Then

                                            'Upload File
                                            RemoveMemberFile(.MemberId, _old.MedicalResultTypeFileName, _old.MedicalResultTypeGUID)
                                            UploadMemberFile(.MemberId, .MedicalResultTypeFileName, .MedicalResultTypeGUID)
                                            'Upload File

                                        End If

                                        AppLib.CreateLogDetails(_hrsMemberMedicalResultTypeLogDetailList, "HrsMemberMedicalResultTypeLogDetail")

                                    End With

                                End If
                            Next

                        End If

                    Next

                End If


#End Region

                _successFlag = True

            Catch _exception As Exception
                Return Me.BadRequest(_exception.Message)
            Finally
                With _transaction
                    If _successFlag Then
                        .Commit()
                    Else
                        .Rollback()
                    End If
                    .Dispose()
                End With

                _transaction = Nothing
                DataCore.Disconnect()

            End Try

            Return Me.Ok(True)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("RemoveMember")>
    <Route("members/{memberId}")>
    <HttpDelete>
    Public Function RemoveMember(memberId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

        If memberId <= 0 Then
            Throw New ArgumentException("Member ID is required.")
        End If

        Try

            Dim _currentUserId As Integer = 1      ' System (default)
            If q.CurrentUserId > 0 Then
                _currentUserId = q.CurrentUserId
            End If

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.DeleteHrsMember(memberId, q.LockId)

                _successFlag = True

            Catch _exception As Exception
                Return Me.BadRequest(_exception.Message)
            Finally
                With _transaction
                    If _successFlag Then
                        .Commit()
                    Else
                        .Rollback()
                    End If
                    .Dispose()
                End With

                _transaction = Nothing
                DataCore.Disconnect()

            End Try

            Return Me.Ok(True)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function
    Private Sub LoadHrsMember(member As MemberBody, hrsMember As HrsMember)

        DataLib.ScatterValues(member, hrsMember)

    End Sub
    Private Sub LoadHrsMemberSoloParent(member As MemberBody, hrsMemberSoloParent As HrsMemberSoloParent)

        DataLib.ScatterValues(member, hrsMemberSoloParent)

    End Sub

    Private Sub LoadHrsMember(row As DataRow, member As HrsMember)

        With member
            .MemberId = row.ToInt32("MemberId")
            .MemberEmployeeId = row.ToString("MemberEmployeeId")
            .MemberLastName = row.ToString("MemberLastName")
            .MemberFirstName = row.ToString("MemberFirstName")
            .MemberMiddleName = row.ToString("MemberMiddleName")
            .MemberSuffixId = row.ToInt32("MemberSuffixId")
            .MemberTypeId = row.ToInt32("MemberTypeId")
            .TypeQualificationDetailId = row.ToInt32("TypeQualificationDetailId")

            .MemberStatusId = row.ToInt32("MemberStatusId")
            .BirthDate = row.ToDate("BirthDate")
            '.BirthPlace = row.ToString("BirthPlace")
            .BirthRegionId = row.ToString("BirthRegionId")
            .BirthProvinceId = row.ToString("BirthProvinceId")
            .BirthMunicipalityId = row.ToString("BirthMunicipalityId")

            .SexId = row.ToString("SexId")
            .BloodTypeId = row.ToInt32("BloodTypeId")
            .Height = row.ToString("Height")
            .Weight = row.ToString("Weight")
            .CivilStatusId = row.ToInt32("CivilStatusId")
            .ReligionId = row.ToInt32("ReligionId")

            .Address1 = row.ToString("Address1")
            .Address2 = row.ToString("Address2")
            .PostalCode = row.ToString("PostalCode")
            .PhoneNumber = row.ToString("PhoneNumber")
            .MobileNumber = row.ToString("MobileNumber")
            .Email = row.ToString("Email")

            .RegionId = row.ToString("RegionId")
            .ProvinceId = row.ToString("ProvinceId")
            .MunicipalityId = row.ToString("MunicipalityId")
            .BarangayId = row.ToString("BarangayId")

            .Facebook = row.ToString("Facebook")
            .Instagram = row.ToString("Instagram")

            .AlternateRegionId = row.ToString("AlternateRegionId")
            .AlternateProvinceId = row.ToString("AlternateProvinceId")
            .AlternateMunicipalityId = row.ToString("AlternateMunicipalityId")
            .AlternateBarangayId = row.ToString("AlternateBarangayId")
            .AlternatePostalCode = row.ToString("AlternatePostalCode")

            .AlternateAddress1 = row.ToString("AlternateAddress1")
            .AlternateAddress2 = row.ToString("AlternateAddress2")

            .CDAMemberTypeId = row.ToInt32("CDAMemberTypeId")
            .CDAMemberTypeAmount = row.ToDecimal("CDAMemberTypeAmount")

            '.EmploymentStatusId = row.ToInt32("EmploymentStatusId")
            '.EmploymentTypeId = row.ToInt32("EmploymentTypeId")

            .AbroadFlag = row.ToBoolean("AbroadFlag")
            .RelocateFlag = row.ToBoolean("RelocateFlag")
            .AbroadFlag = row.ToBoolean("AbroadFlag")
            .WeekendHolidayFlag = row.ToBoolean("WeekendHolidayFlag")
            .ExpectedSalary = row.ToInt32("ExpectedSalary")
            .InitialHiredDate = row.ToDate("InitialHiredDate")
            .GCashNumber = row.ToString("GCashNumber")
            .BankId = row.ToInt32("BankId")
            .BankAccountNumber = row.ToString("BankAccountNumber")
            .RecruiterId = row.ToInt32("RecruiterId")
            '.ApplicationSourceId = row.ToInt32("ApplicationSourceId")
            .PhotoFileName = row.ToString("PhotoFileName")
            .PhotoGUID = row.ToString("PhotoGUID")
            .ImageExtension = row.ToString("ImageExtension")

        End With

    End Sub
    Private Sub LoadHrsMemberSoloParent(row As DataRow, memberSoloParent As HrsMemberSoloParent)

        With memberSoloParent
            .MemberId = row.ToInt32("MemberId")
            .SoloParentFlag = row.ToBoolean("SoloParentFlag")
        End With

        'Dim _detail As HrsMemberSoloParent
        'For Each _row As DataRow In rows
        '    _detail = New HrsMemberSoloParent

        '    With _detail
        '        .MemberId = rows.ToInt32("MemberId")
        '        .SoloParentFlag = rows.ToBoolean("SoloParentFlag")

        '    End With

        '    List.Add(_detail)

        'Next


    End Sub
    Private Sub LoadHrsMemberLanguageList(rows As DataRowCollection, list As HrsMemberLanguageList)

        Dim _detail As HrsMemberLanguage
        For Each _row As DataRow In rows
            _detail = New HrsMemberLanguage

            With _detail
                .LanguageDetailId = _row.ToInt32("LanguageDetailId")
                .LanguageId = _row.ToInt32("LanguageId")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub LoadHrsMemberEducationList(rows As DataRowCollection, list As HrsMemberEducationList)

        Dim _detail As HrsMemberEducation
        For Each _row As DataRow In rows
            _detail = New HrsMemberEducation

            With _detail
                .EduDetailId = _row.ToInt32("EduDetailId")
                .SchoolName = _row.ToString("SchoolName")
                .EducationLevelId = _row.ToInt32("EducationLevelId")
                .CourseName = _row.ToString("CourseName")
                .EduStartYear = _row.ToInt32("EduStartYear")
                .EduEndYear = _row.ToInt32("EduEndYear")
                .EduFileName = _row.ToString("EduFileName")
                .EduGUID = _row.ToString("EduGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberLicenseProfessionList(rows As DataRowCollection, list As HrsMemberLicenseProfessionList)

        Dim _detail As HrsMemberLicenseProfession
        For Each _row As DataRow In rows
            _detail = New HrsMemberLicenseProfession

            With _detail
                .LicenseProfessionDetailId = _row.ToInt32("LicenseProfessionDetailId")
                .LicenseProfessionId = _row.ToInt32("LicenseProfessionId")
                .PRCIdNo = _row.ToString("PRCIdNo")
                .LicenseFileName = _row.ToString("LicenseFileName")
                .LicenseGUID = _row.ToString("LicenseGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberCDATrainingList(rows As DataRowCollection, list As HrsMemberCDATrainingList)

        Dim _detail As HrsMemberCDATraining
        For Each _row As DataRow In rows
            _detail = New HrsMemberCDATraining

            With _detail
                .CDATrainingDetailId = _row.ToInt32("CDATrainingDetailId")
                .CertificateNumber = _row.ToString("CertificateNumber")
                .CDATrainingId = _row.ToInt32("CDATrainingId")
                .TrainingDate = _row.ToDate("TrainingDate")
                .CDAFileName = _row.ToString("CDAFileName")
                .CDAGUID = _row.ToString("CDAGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub LoadHrsMemberNCIIQualificationTitleList(rows As DataRowCollection, list As HrsMemberNCIIQualificationTitleList)

        Dim _detail As HrsMemberNCIIQualificationTitle
        For Each _row As DataRow In rows
            _detail = New HrsMemberNCIIQualificationTitle

            With _detail
                .NCIIQualificationTitleDetailId = _row.ToInt32("NCIIQualificationTitleDetailId")
                .CertificateNumber = _row.ToString("CertificateNumber")
                .NCIIQualificationTitleId = _row.ToInt32("NCIIQualificationTitleId")
                .IssuanceDate = _row.ToDate("IssuanceDate")
                .ValidityDate = _row.ToDate("ValidityDate")
                .TrainingInstitutionId = _row.ToInt32("TrainingInstitutionId")
                .AssessmentCenterId = _row.ToInt32("AssessmentCenterId")
                .NCIIFileName = _row.ToString("NCIIFileName")
                .NCIIGUID = _row.ToString("NCIIGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub LoadHrsMemberComplianceTrainingList(rows As DataRowCollection, list As HrsMemberComplianceTrainingList)

        Dim _detail As HrsMemberComplianceTraining
        For Each _row As DataRow In rows
            _detail = New HrsMemberComplianceTraining

            With _detail
                .ComplianceTrainingDetailId = _row.ToInt32("ComplianceTrainingDetailId")
                .CertificateNumber = _row.ToString("CertificateNumber")
                .ComplianceTrainingId = _row.ToInt32("ComplianceTrainingId")
                .IssuanceDate = _row.ToDate("IssuanceDate")
                .ValidityDate = _row.ToDate("ValidityDate")
                .TrainingInstitutionId = _row.ToInt32("TrainingInstitutionId")
                .AssessmentCenterId = _row.ToInt32("AssessmentCenterId")
                .ComplianceFileName = _row.ToString("ComplianceFileName")
                .ComplianceGUID = _row.ToString("ComplianceGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberWorkList(rows As DataRowCollection, list As HrsMemberWorkList)

        Dim _detail As HrsMemberWork
        For Each _row As DataRow In rows
            _detail = New HrsMemberWork

            With _detail
                .WorkDetailId = _row.ToInt32("WorkDetailId")
                .WorkPosition = _row.ToString("WorkPosition")
                .CompanyName = _row.ToString("CompanyName")
                .MunicipalityId = _row.ToString("MunicipalityId")
                .CompanyPhoneNumber = _row.ToString("CompanyPhoneNumber")
                .WorkStartDate = _row.ToDate("WorkStartDate")
                .WorkEndDate = _row.ToDate("WorkEndDate")
                .ReasonForLeaving = _row.ToString("ReasonForLeaving")
                .WorkFileName = _row.ToString("WorkFileName")
                .WorkGUID = _row.ToString("WorkGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberAffiliationList(rows As DataRowCollection, list As HrsMemberAffiliationList)

        Dim _detail As HrsMemberAffiliation
        For Each _row As DataRow In rows
            _detail = New HrsMemberAffiliation

            With _detail
                .AffiliationDetailId = _row.ToInt32("AffiliationDetailId")
                .AffiliationId = _row.ToInt32("AffiliationId")
                .AffiliationDate = _row.ToDate("AffiliationDate")
                .AffiliationPosition = _row.ToString("AffiliationPosition")
                .AffiliationDescription = _row.ToString("AffiliationDescription")
                .AffiliationFileName = _row.ToString("AffiliationFileName")
                .AffiliationGUID = _row.ToString("AffiliationGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberMedicalResultTypeList(rows As DataRowCollection, list As HrsMemberMedicalResultTypeList)

        Dim _detail As HrsMemberMedicalResultType
        For Each _row As DataRow In rows
            _detail = New HrsMemberMedicalResultType

            With _detail
                .MedicalResultTypeDetailId = _row.ToInt32("MedicalResultTypeDetailId")
                .MedicalResultTypeId = _row.ToInt32("MedicalResultTypeId")
                .Remarks = _row.ToString("Remarks")
                .MedicalResultTypeFileName = _row.ToString("MedicalResultTypeFileName")
                .MedicalResultTypeGUID = _row.ToString("MedicalResultTypeGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub


    Private Sub LoadHrsMemberCertificateList(rows As DataRowCollection, list As HrsMemberCertificateList)

        Dim _detail As HrsMemberCertificate

        For Each _row As DataRow In rows
            _detail = New HrsMemberCertificate

            With _detail
                .CertificateDetailId = _row.ToInt32("CertificateDetailId")
                .CertificateName = _row.ToString("CertificateName")
                .Rating = _row.ToString("Rating")
                .IssuedBy = _row.ToString("IssuedBy")
                .IssuedDate = _row.ToDate("IssuedDate")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberEligibilityList(rows As DataRowCollection, list As HrsMemberEligibilityList)

        Dim _detail As HrsMemberEligibility

        For Each _row As DataRow In rows
            _detail = New HrsMemberEligibility

            With _detail
                .EligibilityDetailId = _row.ToInt32("EligibilityDetailId")
                .EligibilityName = _row.ToString("EligibilityName")
                .YearTaken = _row.ToInt32("YearTaken")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberLicenseList(rows As DataRowCollection, list As HrsMemberLicenseList)

        Dim _detail As HrsMemberLicense

        For Each _row As DataRow In rows
            _detail = New HrsMemberLicense

            With _detail
                .LicenseDetailId = _row.ToInt32("LicenseDetailId")
                .LicenseTitle = _row.ToString("LicenseTitle")
                .ExpiryDate = _row.ToDate("ExpiryDate")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberSkillSetList(rows As DataRowCollection, list As HrsMemberSkillSetList)

        Dim _detail As HrsMemberSkillSet

        For Each _row As DataRow In rows
            _detail = New HrsMemberSkillSet

            With _detail
                .SkillSetDetailId = _row.ToInt32("SkillSetDetailId")
                .SkillDetailId = _row.ToInt32("SkillDetailId")
                .Remarks = _row.ToString("Remarks")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberKinList(rows As DataRowCollection, list As HrsMemberKinList)

        Dim _detail As HrsMemberKin

        For Each _row As DataRow In rows
            _detail = New HrsMemberKin

            With _detail
                .KinDetailId = _row.ToInt32("KinDetailId")
                .KinLastName = _row.ToString("KinLastName")
                .KinFirstName = _row.ToString("KinFirstName")
                .KinMiddleName = _row.ToString("KinMiddleName")
                .KinSuffixId = _row.ToInt32("KinSuffixId")
                .EmergencyContactFlag = _row.ToBoolean("EmergencyContactFlag")

                .RelationId = _row.ToInt32("RelationId")
                .KinPhoneNumber = _row.ToString("KinPhoneNumber")
                .KinOccupation = _row.ToString("KinOccupation")
                .KinEmail = _row.ToString("KinEmail")
                .KinFileName = _row.ToString("KinFileName")
                .KinGUID = _row.ToString("KinGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")

            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberDocTypeList(rows As DataRowCollection, list As HrsMemberDocTypeList)

        Dim _detail As HrsMemberDocType

        For Each _row As DataRow In rows
            _detail = New HrsMemberDocType

            With _detail
                .DocTypeDetailId = _row.ToInt32("DocTypeDetailId")
                .DocTypeId = _row.ToInt32("DocTypeId")
                .ExpirationDate = _row.ToDate("ExpirationDate")
                .DocTypeReference = _row.ToString("DocTypeReference")
                .DocTypeFileName = _row.ToString("DocTypeFileName")
                .DocTypeGUID = _row.ToString("DocTypeGUID")
                .FileExtension = _row.ToString("FileExtension")
                .FileUrl = _row.ToString("FileUrl")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub LoadHrsMemberDisabilityList(rows As DataRowCollection, list As HrsMemberDisabilityList)

        Dim _detail As HrsMemberDisability

        For Each _row As DataRow In rows
            _detail = New HrsMemberDisability

            With _detail
                .DisabilityDetailId = _row.ToInt32("DisabilityDetailId")
                .DisabilityId = _row.ToInt32("DisabilityId")
                .Remarks = _row.ToString("Remarks")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberRnrRecordingList(rows As DataRowCollection, list As HrsMemberRnrRecordingList)

        Dim _detail As HrsMemberRnrRecording

        For Each _row As DataRow In rows
            _detail = New HrsMemberRnrRecording

            With _detail
                .RnrRecordingDetailId = _row.ToInt32("RnrRecordingDetailId")
                .RnrRecordingId = _row.ToInt32("RnrRecordingId")
                .RnrNumberId = _row.ToInt32("RnrNumberId")
                .RnrNumber = _row.ToDecimal("RnrNumber")
            End With

            list.Add(_detail)

        Next

    End Sub
    Private Sub LoadHrsMemberVaccineList(rows As DataRowCollection, list As HrsMemberVaccineList)

        Dim _detail As HrsMemberVaccine

        For Each _row As DataRow In rows
            _detail = New HrsMemberVaccine

            With _detail
                .VaccineDetailId = _row.ToInt32("VaccineDetailId")
                .VaccineTypeId = _row.ToInt32("VaccineTypeId")
                .VaccineName = _row.ToString("VaccineName")
                .VaccineDate = _row.ToDate("VaccineDate")
            End With

            list.Add(_detail)

        Next

    End Sub

    Private Sub InsertHrsMember(member As HrsMember)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("Age")
            .Add("LockId")
            .Add("MemberEmployeeId")

        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMember", member, _excludedFields)
            .CommandType = CommandType.Text
            Try


                Me.AddInsertUpdateParamsHrsMember(member)
            Catch ex As Exception

            End Try
            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub InsertHrsMemberLanguages(list As HrsMemberLanguageList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LanguageDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberLanguage", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLanguage In list
                Me.AddInsertUpdateParamsHrsMemberLanguage(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberCertificates(list As HrsMemberCertificateList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("CertificateDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberCertificate", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberCertificate In list
                Me.AddInsertUpdateParamsHrsMemberCertificate(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberEligibilities(list As HrsMemberEligibilityList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("EligibilityDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberEligibility", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberEligibility In list
                Me.AddInsertUpdateParamsHrsMemberEligibility(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberLicenses(list As HrsMemberLicenseList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LicenseDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberLicense", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLicense In list
                Me.AddInsertUpdateParamsHrsMemberLicense(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberSkillSets(list As HrsMemberSkillSetList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("SkillSetDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberSkillSet", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberSkillSet In list
                Me.AddInsertUpdateParamsHrsMemberSkillSet(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberKins(list As HrsMemberKinList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("KinDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberKin", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberKin In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.KinFileName, _detail.KinGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberKin(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberDocTypes(list As HrsMemberDocTypeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("DocTypeDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDocType", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDocType In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.DocTypeFileName, _detail.DocTypeGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberDocType(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberEducations(list As HrsMemberEducationList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("EduDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberEducation", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberEducation In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.EduFileName, _detail.EduGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberEducation(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberLicenseProfessions(list As HrsMemberLicenseProfessionList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LicenseProfessionDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberLicenseProfession", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLicenseProfession In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.LicenseFileName, _detail.LicenseGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberLicenseProfession(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberCDATrainings(list As HrsMemberCDATrainingList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("CDATrainingDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberCDATraining", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberCDATraining In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.CDAFileName, _detail.CDAGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberCDATraining(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub InsertHrsMemberNCIIQualificationTitles(list As HrsMemberNCIIQualificationTitleList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("NCIIQualificationTitleDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberNCIIQualificationTitle", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberNCIIQualificationTitle In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.NCIIFileName, _detail.NCIIGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberNCIIQualificationTitle(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberComplianceTrainings(list As HrsMemberComplianceTrainingList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ComplianceTrainingDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberComplianceTraining", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberComplianceTraining In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.ComplianceFileName, _detail.ComplianceGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberComplianceTraining(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberWorks(list As HrsMemberWorkList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("WorkDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberWork", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberWork In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.WorkFileName, _detail.WorkGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberWork(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub InsertHrsMemberAffiliations(list As HrsMemberAffiliationList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("AffiliationDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberAffiliation", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberAffiliation In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.AffiliationFileName, _detail.AffiliationGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberAffiliation(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub InsertHrsMemberMedicalResultTypes(list As HrsMemberMedicalResultTypeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("MedicalResultTypeDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberMedicalResultType", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberMedicalResultType In list

                Try
                    UploadMemberFile(_detail.MemberId, _detail.MedicalResultTypeFileName, _detail.MedicalResultTypeGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsHrsMemberMedicalResultType(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub


    'Private Sub InsertHrsMemberKin(list As HrsMemberKinList)

    '    Dim _excludedFields As New List(Of String)

    '    With _excludedFields
    '        .Add("KinDetailId")     ' auto-assigned by DB
    '        .Add("LogActionId")
    '        .Add("LockId")
    '    End With

    '    With DataCore.Command
    '        .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberKin", list(0), _excludedFields)
    '        .CommandType = CommandType.Text

    '        For Each _detail As HrsMemberKin In list
    '            Me.AddInsertUpdateParamsHrsMemberKin(_detail)
    '            .ExecuteNonQuery()
    '        Next

    '    End With

    'End Sub
    Private Sub InsertHrsMemberDisabilities(list As HrsMemberDisabilityList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("DisabilityDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDisability", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDisability In list
                Me.AddInsertUpdateParamsHrsMemberDisability(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberRnrRecordings(list As HrsMemberRnrRecordingList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("RnrRecordingDetailId")     ' auto-assigned by DB
            .Add("RnrNumber")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberRnrRecording", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberRnrRecording In list
                Me.AddInsertUpdateParamsHrsMemberRnrRecording(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberVaccines(list As HrsMemberVaccineList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("VaccineDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberVaccine", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberVaccine In list
                Me.AddInsertUpdateParamsHrsMemberVaccine(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub

    Private Sub InsertHrsMemberDisability(list As HrsMemberDisabilityList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("DisabilityDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDisability", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDisability In list
                Me.AddInsertUpdateParamsHrsMemberDisability(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub InsertHrsMemberDocType(list As HrsMemberDocTypeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("DocTypeDetailId")     ' auto-assigned by DB
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDocType", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDocType In list
                Me.AddInsertUpdateParamsHrsMemberDocType(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub UpdateHrsMember(member As HrsMember)

        Dim _excludedFields As New List(Of String)
        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberId")
            .Add("LockId")
        End With

        With _excludedFields
            .Add("Age")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMember", member, _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsHrsMember(member)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(member.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateHrsMemberSoloParent(list As HrsMemberSoloParentList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("MemberId")
            '.Add("LockId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberSoloParent", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberSoloParent In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberSoloParent(_detail)
                    '.Parameters.AddWithValue("@EduDetailId", _detail.EduDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberLanguages(list As HrsMemberLanguageList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("LanguageDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberLanguage", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLanguage In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberLanguage(_detail)
                    .Parameters.AddWithValue("@LanguageDetailId", _detail.LanguageDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdatHrsMemberEducations(list As HrsMemberEducationList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("EduDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberEducation", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberEducation In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberEducation(_detail)
                    .Parameters.AddWithValue("@EduDetailId", _detail.EduDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberCertificates(list As HrsMemberCertificateList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("CertificateDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberCertificate", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberCertificate In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberCertificate(_detail)
                    .Parameters.AddWithValue("@CertificateDetailId", _detail.CertificateDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberEligibilities(list As HrsMemberEligibilityList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("EligibilityDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberEligibility", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberEligibility In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberEligibility(_detail)
                    .Parameters.AddWithValue("@EligibilityDetailId", _detail.EligibilityDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberLicenses(list As HrsMemberLicenseList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("LicenseDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberLicense", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLicense In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberLicense(_detail)
                    .Parameters.AddWithValue("@LicenseDetailId", _detail.LicenseDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberSkillSets(list As HrsMemberSkillSetList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("SkillSetDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberSkillSet", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberSkillSet In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberSkillSet(_detail)
                    .Parameters.AddWithValue("@SkillSetDetailId", _detail.SkillSetDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberKins(list As HrsMemberKinList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("KinDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberKin", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberKin In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberKin(_detail)
                    .Parameters.AddWithValue("@KinDetailId", _detail.KinDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberDocTypes(list As HrsMemberDocTypeList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("DocTypeDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberDocType", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDocType In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberDocType(_detail)
                    .Parameters.AddWithValue("@DocTypeDetailId", _detail.DocTypeDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberEducations(list As HrsMemberEducationList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("EduDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberEducation", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberEducation In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberEducation(_detail)
                    .Parameters.AddWithValue("@EduDetailId", _detail.EduDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberLicenseProfessions(list As HrsMemberLicenseProfessionList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("LicenseProfessionDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberLicenseProfession", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberLicenseProfession In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberLicenseProfession(_detail)
                    .Parameters.AddWithValue("@LicenseProfessionDetailId", _detail.LicenseProfessionDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberCDATrainings(list As HrsMemberCDATrainingList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("CDATrainingDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberCDATraining", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberCDATraining In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberCDATraining(_detail)
                    .Parameters.AddWithValue("@CDATrainingDetailId", _detail.CDATrainingDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberNCIIQualificationTitles(list As HrsMemberNCIIQualificationTitleList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("NCIIQualificationTitleDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberNCIIQualificationTitle", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberNCIIQualificationTitle In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberNCIIQualificationTitle(_detail)
                    .Parameters.AddWithValue("@NCIIQualificationTitleDetailId", _detail.NCIIQualificationTitleDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberComplianceTrainings(list As HrsMemberComplianceTrainingList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ComplianceTrainingDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberComplianceTraining", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberComplianceTraining In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberComplianceTraining(_detail)
                    .Parameters.AddWithValue("@ComplianceTrainingDetailId", _detail.ComplianceTrainingDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberWorks(list As HrsMemberWorkList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("WorkDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberWork", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberWork In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberWork(_detail)
                    .Parameters.AddWithValue("@WorkDetailId", _detail.WorkDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberAffiliations(list As HrsMemberAffiliationList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("AffiliationDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberAffiliation", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberAffiliation In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberAffiliation(_detail)
                    .Parameters.AddWithValue("@AffiliationDetailId", _detail.AffiliationDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberMedicalResultTypes(list As HrsMemberMedicalResultTypeList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("MedicalResultTypeDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberMedicalResultType", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberMedicalResultType In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberMedicalResultType(_detail)
                    .Parameters.AddWithValue("@MedicalResultTypeDetailId", _detail.MedicalResultTypeDetailId)
                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberDisabilities(list As HrsMemberDisabilityList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("DisabilityDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberDisability", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberDisability In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberDisability(_detail)
                    .Parameters.AddWithValue("@DisabilityDetailId", _detail.DisabilityDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub UpdateHrsMemberRnrRecordings(list As HrsMemberRnrRecordingList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("RnrRecordingDetailId")
        End With

        With _excludedFields
            .Add("RnrNumber")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberRnrRecording", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberRnrRecording In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberRnrRecording(_detail)
                    .Parameters.AddWithValue("@RnrRecordingDetailId", _detail.RnrRecordingDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberVaccines(list As HrsMemberVaccineList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("VaccineDetailId")
        End With

        With _excludedFields
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberVaccine", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberVaccine In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsHrsMemberVaccine(_detail)
                    .Parameters.AddWithValue("@VaccineDetailId", _detail.VaccineDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub AddInsertUpdateParamsHrsMember(member As HrsMember)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@MemberId", member.MemberId)
            .AddWithValue("@MemberEmployeeId", member.MemberEmployeeId)

            .AddWithValue("@MemberLastName", member.MemberLastName)
            .AddWithValue("@MemberFirstName", member.MemberFirstName)
            .AddWithValue("@MemberMiddleName", member.MemberMiddleName.ToNullable)

            .AddWithValue("@MemberSuffixId", member.MemberSuffixId.ToNullable)
            .AddWithValue("@MemberTypeId", member.MemberTypeId)
            .AddWithValue("@TypeQualificationDetailId", member.TypeQualificationDetailId)
            .AddWithValue("@MemberStatusId", member.MemberStatusId)

            .AddWithValue("@BirthDate", member.BirthDate)

            '.AddWithValue("@BirthPlace", member.BirthPlace)
            .AddWithValue("@BirthRegionId", member.BirthRegionId.ToNullable)
            .AddWithValue("@BirthProvinceId", member.BirthProvinceId.ToNullable)
            .AddWithValue("@BirthMunicipalityId", member.BirthMunicipalityId.ToNullable)

            .AddWithValue("@SexId", member.SexId)
            .AddWithValue("@BloodTypeId", member.BloodTypeId.ToNullable)
            .AddWithValue("@Height", member.Height.ToNullable)
            .AddWithValue("@Weight", member.Weight.ToNullable)
            .AddWithValue("@CivilStatusId", member.CivilStatusId)
            .AddWithValue("@ReligionId", member.ReligionId)
            .AddWithValue("@Address1", member.Address1)
            .AddWithValue("@Address2", member.Address2.ToNullable)
            .AddWithValue("@PostalCode", member.PostalCode.ToNullable)

            .AddWithValue("@PhoneNumber", member.PhoneNumber.ToNullable)
            .AddWithValue("@MobileNumber", member.MobileNumber.ToNullable)
            .AddWithValue("@Email", member.Email.ToNullable)
            .AddWithValue("@RegionId", member.RegionId.ToNullable)
            .AddWithValue("@ProvinceId", member.ProvinceId.ToNullable)
            .AddWithValue("@MunicipalityId", member.MunicipalityId.ToNullable)
            .AddWithValue("@BarangayId", member.BarangayId.ToNullable)
            '.AddWithValue("@Photo", member.Photo)
            '.AddWithValue("@Photo", member.)

            .AddWithValue("@Facebook", member.Facebook.ToNullable)
            .AddWithValue("@Instagram", member.Instagram.ToNullable)

            .AddWithValue("@AlternateRegionId", member.AlternateRegionId.ToNullable)

            .AddWithValue("@AlternateProvinceId", member.AlternateProvinceId.ToNullable)
            .AddWithValue("@AlternateMunicipalityId", member.AlternateMunicipalityId.ToNullable)
            .AddWithValue("@AlternateBarangayId", member.AlternateBarangayId.ToNullable)
            .AddWithValue("@AlternatePostalCode", member.AlternatePostalCode.ToNullable)

            .AddWithValue("@AlternateAddress1", member.AlternateAddress1.ToNullable)
            .AddWithValue("@AlternateAddress2", member.AlternateAddress2.ToNullable)

            .AddWithValue("@CDAMemberTypeId", member.CDAMemberTypeId.ToNullable)
            .AddWithValue("@CDAMemberTypeAmount", member.CDAMemberTypeAmount)

            '.AddWithValue("@EmploymentStatusId", member.EmploymentStatusId.ToNullable)
            '.AddWithValue("@EmploymentTypeId", member.EmploymentTypeId.ToNullable)

            .AddWithValue("@AbroadFlag", member.AbroadFlag)
            .AddWithValue("@RelocateFlag", member.RelocateFlag)

            .AddWithValue("@WeekendHolidayFlag", member.WeekendHolidayFlag)
            .AddWithValue("@ExpectedSalary", member.ExpectedSalary)
            .AddWithValue("@InitialHiredDate", member.InitialHiredDate.ToNullable)
            .AddWithValue("@GCashNumber", member.GCashNumber.ToNullable)
            .AddWithValue("@BankId", member.BankId.ToNullable)
            .AddWithValue("@BankAccountNumber", member.BankAccountNumber.ToNullable)
            .AddWithValue("@RecruiterId", member.RecruiterId.ToNullable)
            '.AddWithValue("@ApplicationSourceId", member.ApplicationSourceId.ToNullable)
            .AddWithValue("@PhotoFileName", member.PhotoFileName.ToNullable)
            .AddWithValue("@PhotoGUID", member.PhotoGUID.ToNullable)
            .AddWithValue("@ImageExtension", member.ImageExtension.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberSoloParent(memberSoloParent As HrsMemberSoloParent)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@MemberId", memberSoloParent.MemberId)
            .AddWithValue("@SoloParentFlag", memberSoloParent.SoloParentFlag)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberLanguage(dtl As HrsMemberLanguage)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@MemberId", dtl.MemberId)
            .AddWithValue("@LanguageId", dtl.LanguageId)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberCertificate(dtl As HrsMemberCertificate)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", dtl.MemberId)
            .AddWithValue("@CertificateName", dtl.CertificateName)
            .AddWithValue("@Rating", dtl.Rating.ToNullable)
            .AddWithValue("@IssuedBy", dtl.IssuedBy.ToNullable)
            .AddWithValue("@IssuedDate", dtl.IssuedDate.ToNullable)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberEligibility(dtl As HrsMemberEligibility)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", dtl.MemberId)
            .AddWithValue("@EligibilityName", dtl.EligibilityName)
            .AddWithValue("@YearTaken", dtl.YearTaken)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberLicense(dtl As HrsMemberLicense)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", dtl.MemberId)
            .AddWithValue("@LicenseTitle", dtl.LicenseTitle)
            .AddWithValue("@ExpiryDate", dtl.ExpiryDate)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberSkillSet(dtl As HrsMemberSkillSet)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", dtl.MemberId)
            .AddWithValue("@SkillDetailId", dtl.SkillDetailId)
            .AddWithValue("@Remarks", dtl.Remarks)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberEducation(education As HrsMemberEducation)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", education.MemberId)
            .AddWithValue("@EducationLevelId", education.EducationLevelId)
            .AddWithValue("@SchoolName", education.SchoolName)
            .AddWithValue("@CourseName", education.CourseName.ToNullable)
            .AddWithValue("@EduStartYear", education.EduStartYear.ToNullable)
            .AddWithValue("@EduEndYear", education.EduEndYear.ToNullable)
            .AddWithValue("@EduFileName", education.EduFileName.ToNullable)
            .AddWithValue("@EduGUID", education.EduGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(education.EduFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberLicenseProfession(licenseProfession As HrsMemberLicenseProfession)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", licenseProfession.MemberId)
            .AddWithValue("@LicenseProfessionId", licenseProfession.LicenseProfessionId)
            .AddWithValue("@PRCIdNo", licenseProfession.PRCIdNo)
            .AddWithValue("@LicenseFileName", licenseProfession.LicenseFileName.ToNullable)
            .AddWithValue("@LicenseGUID", licenseProfession.LicenseGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(licenseProfession.LicenseFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberCDATraining(cdaTraining As HrsMemberCDATraining)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", cdaTraining.MemberId)
            .AddWithValue("@CertificateNumber", cdaTraining.CertificateNumber)
            .AddWithValue("@CDATrainingId", cdaTraining.CDATrainingId)
            .AddWithValue("@TrainingDate", cdaTraining.TrainingDate.ToNullable)
            .AddWithValue("@CDAFileName", cdaTraining.CDAFileName.ToNullable)
            .AddWithValue("@CDAGUID", cdaTraining.CDAGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(cdaTraining.CDAFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberNCIIQualificationTitle(nciiQualificationTitle As HrsMemberNCIIQualificationTitle)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", nciiQualificationTitle.MemberId)
            .AddWithValue("@CertificateNumber", nciiQualificationTitle.CertificateNumber)
            .AddWithValue("@NCIIQualificationTitleId", nciiQualificationTitle.NCIIQualificationTitleId)
            .AddWithValue("@TrainingInstitutionId", nciiQualificationTitle.TrainingInstitutionId)
            .AddWithValue("@AssessmentCenterId", nciiQualificationTitle.AssessmentCenterId)
            .AddWithValue("@IssuanceDate", nciiQualificationTitle.IssuanceDate)
            .AddWithValue("@ValidityDate", nciiQualificationTitle.ValidityDate)
            .AddWithValue("@NCIIFileName", nciiQualificationTitle.NCIIFileName.ToNullable)
            .AddWithValue("@NCIIGUID", nciiQualificationTitle.NCIIGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(nciiQualificationTitle.NCIIFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberComplianceTraining(complianceTraining As HrsMemberComplianceTraining)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", complianceTraining.MemberId)
            .AddWithValue("@CertificateNumber", complianceTraining.CertificateNumber)
            .AddWithValue("@ComplianceTrainingId", complianceTraining.ComplianceTrainingId)
            .AddWithValue("@TrainingInstitutionId", complianceTraining.TrainingInstitutionId)
            .AddWithValue("@AssessmentCenterId", complianceTraining.AssessmentCenterId)
            .AddWithValue("@IssuanceDate", complianceTraining.IssuanceDate)
            .AddWithValue("@ValidityDate", complianceTraining.ValidityDate)
            .AddWithValue("@ComplianceFileName", complianceTraining.ComplianceFileName.ToNullable)
            .AddWithValue("@ComplianceGUID", complianceTraining.ComplianceGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(complianceTraining.ComplianceFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberWork(work As HrsMemberWork)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", work.MemberId)
            .AddWithValue("@WorkPosition", work.WorkPosition)
            .AddWithValue("@CompanyName", work.CompanyName)
            .AddWithValue("@MunicipalityId", work.MunicipalityId)
            .AddWithValue("@CompanyPhoneNumber", work.CompanyPhoneNumber)
            .AddWithValue("@WorkStartDate", work.WorkStartDate)
            .AddWithValue("@WorkEndDate", work.WorkEndDate)
            .AddWithValue("@ReasonForLeaving", work.ReasonForLeaving)
            .AddWithValue("@WorkFileName", work.WorkFileName.ToNullable)
            .AddWithValue("@WorkGUID", work.WorkGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(work.WorkFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberAffiliation(affiliation As HrsMemberAffiliation)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", affiliation.MemberId)
            .AddWithValue("@AffiliationId", affiliation.AffiliationId)
            .AddWithValue("@AffiliationDate", affiliation.AffiliationDate)
            .AddWithValue("@AffiliationPosition", affiliation.AffiliationPosition)
            .AddWithValue("@AffiliationDescription", affiliation.AffiliationDescription)
            .AddWithValue("@AffiliationFileName", affiliation.AffiliationFileName.ToNullable)
            .AddWithValue("@AffiliationGUID", affiliation.AffiliationGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(affiliation.AffiliationFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberMedicalResultType(medicalResultType As HrsMemberMedicalResultType)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", medicalResultType.MemberId)
            .AddWithValue("@MedicalResultTypeId", medicalResultType.MedicalResultTypeId)
            .AddWithValue("@Remarks", medicalResultType.Remarks)
            .AddWithValue("@MedicalResultTypeFileName", medicalResultType.MedicalResultTypeFileName.ToNullable)
            .AddWithValue("@MedicalResultTypeGUID", medicalResultType.MedicalResultTypeGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(medicalResultType.MedicalResultTypeFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub

    Private Sub DeleteHrsMember(memberId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("MemberId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsMember", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@MemberId", memberId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub DeleteHrsMemberLanguageDetails(list As HrsMemberLanguageList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberLanguage WHERE LanguageDetailId=@LanguageDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberLanguage In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@LanguageDetailId", _old.LanguageDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteHrsMemberEducationDetails(list As HrsMemberEducationList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberEducation WHERE EduDetailId=@EduDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberEducation In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@EduDetailId", _old.EduDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberLicenseProfessionDetails(list As HrsMemberLicenseProfessionList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberLicenseProfession WHERE LicenseProfessionDetailId=@LicenseProfessionDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberLicenseProfession In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@LicenseProfessionDetailId", _old.LicenseProfessionDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberCDATrainingDetails(list As HrsMemberCDATrainingList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberCDATraining WHERE CDATrainingDetailId=@CDATrainingDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberCDATraining In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@CDATrainingDetailId", _old.CDATrainingDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberNCIIQualificationTitleDetails(list As HrsMemberNCIIQualificationTitleList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberNCIIQualificationTitle WHERE NCIIQualificationTitleDetailId=@NCIIQualificationTitleDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberNCIIQualificationTitle In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@NCIIQualificationTitleDetailId", _old.NCIIQualificationTitleDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberComplianceTrainingDetails(list As HrsMemberComplianceTrainingList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberComplianceTraining WHERE ComplianceTrainingDetailId=@ComplianceTrainingDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberComplianceTraining In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ComplianceTrainingDetailId", _old.ComplianceTrainingDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberWorkDetails(list As HrsMemberWorkList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberWork WHERE WorkDetailId=@WorkDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberWork In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@WorkDetailId", _old.WorkDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberAffiliationDetails(list As HrsMemberAffiliationList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberAffiliation WHERE AffiliationDetailId=@AffiliationDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberAffiliation In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@AffiliationDetailId", _old.AffiliationDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteHrsMemberMedicalResultTypeDetails(list As HrsMemberMedicalResultTypeList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberMedicalResultType WHERE MedicalResultTypeDetailId=@MedicalResultTypeDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberMedicalResultType In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@MedicalResultTypeDetailId", _old.MedicalResultTypeDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteHrsMemberCertificateDetails(list As HrsMemberCertificateList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberCertificate WHERE CertificateDetailId=@CertificateDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberCertificate In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@CertificateDetailId", _old.CertificateDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberEligibilityDetails(list As HrsMemberEligibilityList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberEligibility WHERE EligibilityDetailId=@EligibilityDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberEligibility In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@EligibilityDetailId", _old.EligibilityDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberLicenseDetails(list As HrsMemberLicenseList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberLicense WHERE LicenseDetailId=@LicenseDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberLicense In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@LicenseDetailId", _old.LicenseDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberSkillSetDetails(list As HrsMemberSkillSetList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberSkillSet WHERE SkillSetDetailId=@SkillSetDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberSkillSet In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@SkillSetDetailId", _old.SkillSetDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberDocTypeDetails(list As HrsMemberDocTypeList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberDocType WHERE DocTypeDetailId=@DocTypeDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberDocType In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@DocTypeDetailId", _old.DocTypeDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteHrsMemberKinDetails(list As HrsMemberKinList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberKin WHERE KinDetailId=@KinDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberKin In list


                If _old.LogActionId = LogActionId.Delete Then

                    With .Parameters
                        .Clear()
                        .AddWithValue("@KinDetailId", _old.KinDetailId)
                    End With

                    .ExecuteNonQuery()


                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberDisabilityDetails(list As HrsMemberDisabilityList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberDisability WHERE DisabilityDetailId=@DisabilityDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberDisability In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@DisabilityDetailId", _old.DisabilityDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberRnrRecordingDetails(list As HrsMemberRnrRecordingList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberRnrRecording WHERE RnrRecordingDetailId=@RnrRecordingDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberRnrRecording In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@RnrRecordingDetailId", _old.RnrRecordingDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
    Private Sub DeleteHrsMemberVaccineDetails(list As HrsMemberVaccineList)

        With DataCore.Command
            .CommandText = "DELETE dbo.HrsMemberVaccine WHERE VaccineDetailId=@VaccineDetailId"
            .CommandType = CommandType.Text

            For Each _old As HrsMemberVaccine In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@VaccineDetailId", _old.VaccineDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub UpdateHrsMemberKin(list As HrsMemberKinList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("KinDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            '.Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMemberKin", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As HrsMemberKin In list
                'If _detail.LogActionId = LogActionId.Edit Then
                Me.AddInsertUpdateParamsHrsMemberKin(_detail)
                .Parameters.AddWithValue("@KinDetailId", _detail.KinDetailId)

                .ExecuteNonQuery()
                'End If
            Next

        End With

    End Sub

    Private Sub AddInsertUpdateParamsHrsMemberKin(kin As HrsMemberKin)

        With DataCore.Command.Parameters
            .Clear()

            '.AddWithValue("@TrxDetailId", dtl.TrxDetailId)
            .AddWithValue("@MemberId", kin.MemberId)
            .AddWithValue("@KinLastName", kin.KinLastName)
            .AddWithValue("@KinFirstName", kin.KinFirstName)
            .AddWithValue("@KinMiddleName", kin.KinMiddleName)
            .AddWithValue("@KinSuffixId", kin.KinSuffixId)
            .AddWithValue("@EmergencyContactFlag", kin.EmergencyContactFlag)

            .AddWithValue("@RelationId", kin.RelationId)
            .AddWithValue("@KinPhoneNumber", kin.KinPhoneNumber.ToNullable)
            .AddWithValue("@KinOccupation", kin.KinOccupation.ToNullable)
            .AddWithValue("@KinEmail", kin.KinEmail.ToNullable)
            .AddWithValue("@KinFileName", kin.KinFileName.ToNullable)
            .AddWithValue("@KinGUID", kin.KinGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(kin.KinFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberDocType(docType As HrsMemberDocType)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@MemberId", docType.MemberId)
            .AddWithValue("@DocTypeId", docType.DocTypeId)
            .AddWithValue("@ExpirationDate", docType.ExpirationDate.ToNullable)
            .AddWithValue("@DocTypeReference", docType.DocTypeReference)
            .AddWithValue("@DocTypeFileName", docType.DocTypeFileName.ToNullable)
            .AddWithValue("@DocTypeGUID", docType.DocTypeGUID.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(docType.DocTypeFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With


    End Sub

    Private Sub AddInsertUpdateParamsHrsMemberDisability(disability As HrsMemberDisability)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@MemberId", disability.MemberId)
            .AddWithValue("@DisabilityId", disability.DisabilityId)
            .AddWithValue("@Remarks", disability.Remarks.ToNullable)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberRnrRecording(rnrRecording As HrsMemberRnrRecording)
        'JENONLEE
        rnrRecording.RnrNumberId = GetRnrNumber(rnrRecording.RnrNumber)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@MemberId", rnrRecording.MemberId)
            .AddWithValue("@RnrRecordingId", rnrRecording.RnrRecordingId)
            .AddWithValue("@RnrNumberId", rnrRecording.RnrNumberId.ToNullable)

        End With

    End Sub
    Private Sub AddInsertUpdateParamsHrsMemberVaccine(vaccine As HrsMemberVaccine)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@MemberId", vaccine.MemberId)
            .AddWithValue("@VaccineTypeId", vaccine.VaccineTypeId)
            .AddWithValue("@VaccineName", vaccine.VaccineName)
            .AddWithValue("@VaccineDate", vaccine.VaccineDate)
        End With

    End Sub

    '<SymAuthorization("UploadTest")>
    '<Route("images/{memberId}")>
    '<HttpPost>
    'Public Async Function UploadTest(memberId As Integer) As Threading.Tasks.Task(Of IHttpActionResult)

    '    'If Not Me.Request.Content.IsMimeMultipartContent() Then
    '    '    Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
    '    'End If

    '    'Dim _memberId As Integer = GetMemberId(memberCode)

    '    If memberId = 0 Then
    '        Return Me.BadRequest("Unknown member")
    '    End If

    '    Try
    '        'TESTING OLNY
    '        'Dim _rootPath As String = "D:\eEAS\eas\public\img" 'System.Web.Hosting.HostingEnvironment.MapPath("/img") 'System.Web.Hosting.HostingEnvironment.MapPath("~/img")
    '        Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp") '+ "/" + Guid.NewGuid.ToString
    '        Dim _fileName As String = Guid.NewGuid.ToString
    '        Dim _provider As New MultipartFormDataStreamProvider(_rootPath)
    '        Dim _subFolder As String = "photo"

    '        If Not Directory.Exists(_rootPath) Then
    '            Directory.CreateDirectory(_rootPath)
    '        End If

    '        Dim _subFolderPath As String = Path.Combine(_rootPath, _subFolder)

    '        If Not Directory.Exists(_subFolderPath) Then
    '            Directory.CreateDirectory(_subFolderPath)
    '        End If

    '        Await Me.Request.Content.ReadAsMultipartAsync(_provider)

    '        Dim _file As MultipartFileData = _provider.FileData.FirstOrDefault
    '        Dim _newFileName As String = _fileName.Replace("""", "")    ' removes double quotes in string
    '        Dim _imageInfo As MagickImageInfo = ImageLib.GetFileInfo(_file.LocalFileName)
    '        Dim _extension As String = Path.GetExtension(_fileName).ToLowerInvariant
    '        Dim _targetFileName As String = Path.Combine(_rootPath, _newFileName)

    '        'Select Case _imageInfo.Format
    '        '    Case MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Png, MagickFormat.Png00, MagickFormat.Png24, MagickFormat.Png32, MagickFormat.Png48, MagickFormat.Png64, MagickFormat.Png8

    '        '        If _imageInfo.Width > _width OrElse _imageInfo.Quality > _quality Then
    '        '            Using _image As New MagickImage(_file.LocalFileName)

    '        '                If _image.Quality > _quality Then
    '        '                    _image.Quality = _quality
    '        '                End If

    '        '                If _image.Width > _width Then
    '        '                    _image.Resize(_width, 0)
    '        '                End If

    '        '                _image.Write(_targetFileName)

    '        'If FileSystemLib.IsTargetFileLargerThanSourceFile(_targetFileName, _file.LocalFileName) Then
    '        File.Copy(_file.LocalFileName, _targetFileName, True)

    '        'End If

    '        'End Using


    '        'File.Delete(_file.LocalFileName)

    '        '        Else

    '        '            File.Move(_file.LocalFileName, _targetFileName)
    '        '        End If


    '        '    Case Else

    '        '        File.Delete(_file.LocalFileName)
    '        '        Return Me.Ok(False)

    '        'End Select

    '        'Dim _updatedFlag As Boolean
    '        'Dim _transaction As SqlTransaction = Nothing

    '        'Try
    '        '    If Not DataCore.Connect(_databaseId) Then
    '        '        Return Me.InternalServerError
    '        '    End If

    '        '    _transaction = DataCore.Connection.BeginTransaction()

    '        '    DataCore.Command.Transaction = _transaction
    '        '    DataCore.Command.Connection = DataCore.Connection

    '        '    With DataCore.Command
    '        '        .CommandText = "UPDATE dbo.HrsMember SET ImageExtension=@ImageExtension WHERE MemberId=@MemberId"
    '        '        .CommandType = CommandType.Text

    '        '        With .Parameters
    '        '            .Clear()
    '        '            .AddWithValue("@ImageExtension", _extension.Substring(1))   ' excludes dot prefix
    '        '            .AddWithValue("@MemberId", _memberId)
    '        '        End With

    '        '        .ExecuteNonQuery()

    '        '    End With

    '        '    _updatedFlag = True

    '        'Catch _exception As Exception
    '        '    Return Me.BadRequest(_exception.Message)
    '        'Finally
    '        '    With _transaction
    '        '        If _updatedFlag Then
    '        '            .Commit()
    '        '        Else
    '        '            .Rollback()
    '        '        End If
    '        '        .Dispose()
    '        '    End With

    '        '    _transaction = Nothing
    '        '    DataCore.Disconnect()

    '        'End Try

    '        'If Not _updatedFlag Then
    '        '    Return BadRequest()
    '        'End If

    '        Return Me.Ok(True)

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function

    <SymAuthorization("UploadPhoto")>
    <Route("images/{guid}")>
    <HttpPost>
    Public Async Function UploadPhoto(guid As String, <FromUri> p As UploadPhotoParams) As Threading.Tasks.Task(Of IHttpActionResult)

        If Not Me.Request.Content.IsMimeMultipartContent() Then
            Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
        End If
        Try
            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")
            Dim _provider As New MultipartFormDataStreamProvider(_rootPath)

            If Not Directory.Exists(_rootPath) Then
                Directory.CreateDirectory(_rootPath)
            End If

            Await Request.Content.ReadAsMultipartAsync(_provider)

            Dim _fileName As String
            Dim _extension As String
            Dim _targetFileName As String
            Dim _detail As FileUploadDetaii
            Dim _detailList As New FileUploadDetaiiList
            Dim _uploadResponse As New FileUploadResponse
            Dim _createdCount As Integer
            Dim _failedCount As Integer
            Dim _imageInfo As MagickImageInfo
            Dim _isFileValid As Boolean
            Dim _pageCount As Integer

            For Each _file As MultipartFileData In _provider.FileData
                _fileName = guid '_file.Headers.ContentDisposition.FileName.Replace("""", "")    ' removes double quotes in string
                _extension = Path.GetExtension(_file.Headers.ContentDisposition.FileName.Replace("""", "")).ToLowerInvariant

                '_targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_file.LocalFileName).Replace("BodyPart_", "") + _extension)
                _targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), _fileName + _extension)
                _isFileValid = False
                _pageCount = 0

                _detail = New FileUploadDetaii
                _detail.FileName = _file.Headers.ContentDisposition.FileName.Replace("""", "")
                _detailList.Add(_detail)

                _imageInfo = ImageLib.GetFileInfo(_file.LocalFileName)

                File.Move(_file.LocalFileName, _targetFileName)
                _isFileValid = True

                _detail.StatusCode = 201
                _detail.StatusText = "Uploaded"
                _createdCount = _createdCount + 1

            Next

            With _uploadResponse
                .CreatedCount = _createdCount
                .FailedCount = _failedCount
                .Details = _detailList
            End With

            Return Me.Ok(_uploadResponse)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function




    '<SymAuthorization("UploadPhoto")>
    '<Route("images/{memberCode}")>
    '<HttpPost>
    'Public Async Function UploadPhoto(memberCode As String, <FromUri> q As UploadImageQuery) As Threading.Tasks.Task(Of IHttpActionResult)

    '    If Not Me.Request.Content.IsMimeMultipartContent() Then
    '        Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
    '    End If

    '    Dim _memberId As Integer = GetMemberId(memberCode)

    '    If _memberId = 0 Then
    '        Return Me.BadRequest("Unknown member")
    '    End If

    '    Dim _width As Integer = 300
    '    Dim _quality As Integer = 96

    '    With q
    '        If .Width > 0 Then
    '            _width = .Width
    '        End If

    '        If .Quality > 0 Then
    '            _quality = .Quality
    '        End If
    '    End With

    '    Try
    '        'TESTING OLNY
    '        Dim _rootPath As String = "D:\eEAS\eas\public\img" 'System.Web.Hosting.HostingEnvironment.MapPath("/img") 'System.Web.Hosting.HostingEnvironment.MapPath("~/img")
    '        'Dim _rootPath1 As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
    '        'Dim _rootPath2 As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp") + "/" + Guid.NewGuid.ToString
    '        'Dim _rootPath3 As String = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/img"), "")
    '        'Dim _rootPath4 As String = "D:\eEAS\eas\public\img" 'System.Web.Hosting.HostingEnvironment.MapPath("/img")

    '        Dim _provider As New MultipartFormDataStreamProvider(_rootPath)
    '        Dim _subFolder As String = "photo"

    '        If Not Directory.Exists(_rootPath) Then
    '            Directory.CreateDirectory(_rootPath)
    '        End If

    '        Dim _subFolderPath As String = Path.Combine(_rootPath, _subFolder)

    '        If Not Directory.Exists(_subFolderPath) Then
    '            Directory.CreateDirectory(_subFolderPath)
    '        End If

    '        Await Me.Request.Content.ReadAsMultipartAsync(_provider)

    '        Dim _file As MultipartFileData = _provider.FileData.FirstOrDefault
    '        Dim _fileName As String = _file.Headers.ContentDisposition.FileName.Replace("""", "")    ' removes double quotes in string
    '        Dim _imageInfo As MagickImageInfo = ImageLib.GetFileInfo(_file.LocalFileName)
    '        Dim _extension As String = Path.GetExtension(_fileName).ToLowerInvariant
    '        Dim _targetFileName As String = Path.Combine(_rootPath, _subFolder, memberCode + _extension)

    '        Select Case _imageInfo.Format
    '            Case MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Png, MagickFormat.Png00, MagickFormat.Png24, MagickFormat.Png32, MagickFormat.Png48, MagickFormat.Png64, MagickFormat.Png8

    '                If _imageInfo.Width > _width OrElse _imageInfo.Quality > _quality Then
    '                    Using _image As New MagickImage(_file.LocalFileName)

    '                        If _image.Quality > _quality Then
    '                            _image.Quality = _quality
    '                        End If

    '                        If _image.Width > _width Then
    '                            _image.Resize(_width, 0)
    '                        End If

    '                        _image.Write(_targetFileName)

    '                        'If FileSystemLib.IsTargetFileLargerThanSourceFile(_targetFileName, _file.LocalFileName) Then
    '                        File.Copy(_file.LocalFileName, _targetFileName, True)

    '                        'End If

    '                    End Using


    '                    File.Delete(_file.LocalFileName)

    '                Else

    '                    File.Move(_file.LocalFileName, _targetFileName)
    '                End If


    '            Case Else

    '                File.Delete(_file.LocalFileName)
    '                Return Me.Ok(False)

    '        End Select

    '        Dim _updatedFlag As Boolean
    '        Dim _transaction As SqlTransaction = Nothing

    '        Try
    '            If Not DataCore.Connect(_databaseId) Then
    '                Return Me.InternalServerError
    '            End If

    '            _transaction = DataCore.Connection.BeginTransaction()

    '            DataCore.Command.Transaction = _transaction
    '            DataCore.Command.Connection = DataCore.Connection

    '            With DataCore.Command
    '                .CommandText = "UPDATE dbo.HrsMember SET ImageExtension=@ImageExtension WHERE MemberId=@MemberId"
    '                .CommandType = CommandType.Text

    '                With .Parameters
    '                    .Clear()
    '                    .AddWithValue("@ImageExtension", _extension.Substring(1))   ' excludes dot prefix
    '                    .AddWithValue("@MemberId", _memberId)
    '                End With

    '                .ExecuteNonQuery()

    '            End With

    '            _updatedFlag = True

    '        Catch _exception As Exception
    '            Return Me.BadRequest(_exception.Message)
    '        Finally
    '            With _transaction
    '                If _updatedFlag Then
    '                    .Commit()
    '                Else
    '                    .Rollback()
    '                End If
    '                .Dispose()
    '            End With

    '            _transaction = Nothing
    '            DataCore.Disconnect()

    '        End Try

    '        If Not _updatedFlag Then
    '            Return BadRequest()
    '        End If

    '        Return Me.Ok(True)

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function


    '<SymAuthorization("GetFinTrx")>
    '<Route("trans-ckd/{trxId}")>
    '<HttpGet>
    'Public Function GetFinTrx(trxId As Integer) As IHttpActionResult

    '    If trxId <= 0 Then
    '        Throw New ArgumentException("Transaction ID is required.")
    '    End If

    '    Try
    '        Using _direct As New SqlDirect("web.GLS0050")
    '            With _direct
    '                .AddParameter("TrxId", trxId)

    '                Using _dataSet As DataSet = _direct.ExecuteDataSet()
    '                    With _dataSet
    '                        .Tables(0).TableName = "trx"
    '                        .Tables(1).TableName = "trxDetails"
    '                    End With

    '                    Return Me.Ok(_dataSet)
    '                End Using

    '            End With
    '        End Using

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function

    Friend Shared Function GetMemberId(accountCode As String) As Integer

        Using _table As DataTable = DataLib.GetList("dbo.HrsMember", "MemberId", "MemberCode=" + accountCode.Enclose(EncloseCharacter.Quote), String.Empty)
            With _table
                If .Rows.Count > 0 Then
                    Return .Rows(0).ToInt32("MemberId")
                Else
                    Return 0
                End If
            End With
        End Using

    End Function
    Friend Shared Function GetMemberCode(memberId As Integer) As String

        Using _table As DataTable = DataLib.GetList("dbo.HrsMember", "MemberCode", "MemberId=" + memberId.ToString, String.Empty)
            With _table
                If .Rows.Count > 0 Then
                    Return .Rows(0).ToString("MemberCode")
                Else
                    Return String.Empty
                End If
            End With
        End Using

    End Function
    Friend Shared Function GetRegion(provinceId As String) As String

        Using _table As DataTable = DataLib.GetList("dbo.DbsProvince", "RegionId", "ProvinceId=" + provinceId.Enclose(EncloseCharacter.Quote), String.Empty)
            With _table
                If .Rows.Count > 0 Then
                    Return .Rows(0).ToString("RegionId")
                Else
                    Return "0"
                End If
            End With
        End Using

    End Function
    Friend Shared Function GetRnrNumber(rnrNumber As Decimal) As Integer

        Using _table As DataTable = DataLib.GetList("dbo.DbsRnrNumber", "RnrNumberId", "RnrNumber=" + rnrNumber.ToString, String.Empty)
            With _table
                If .Rows.Count > 0 Then
                    Return .Rows(0).ToInt32("RnrNumberId")
                Else
                    Return 0
                End If
            End With
        End Using

    End Function

    'Friend Shared Function GetMemberStatus(employmentStatusId As Integer) As Integer

    '    Select Case employmentStatusId
    '        Case EmploymentStatus.Active, EmploymentStatus.LeaveOfAbsence
    '            Return MemberStatus.Active
    '        Case EmploymentStatus.AbsenceWithoutLeave, EmploymentStatus.Floating, EmploymentStatus.Suspended
    '            Return MemberStatus.InActive
    '        Case EmploymentStatus.Resigned, EmploymentStatus.Separated, EmploymentStatus.Terminated, EmploymentStatus.Retired, EmploymentStatus.EndOfContract
    '            Return MemberStatus.Suspended
    '        Case Else
    '            Return MemberStatus.Delisted
    '    End Select

    '    'Using _table As DataTable = DataLib.GetList("dbo.DbsProvince", "RegionId", "ProvinceId=" + provinceId.Enclose(EncloseCharacter.Quote), String.Empty)
    '    '    With _table
    '    '        If .Rows.Count > 0 Then
    '    '            Return .Rows(0).ToString("RegionId")
    '    '        Else 
    '    '            Return "0"
    '    '        End If
    '    '    End With
    '    'End Using

    'End Function
    Friend Shared Function UploadMemberFile(memberId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _memberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant


            Dim _fileName As String = Path.Combine(_rootPath, guid + _extension)

            If Not Directory.Exists(_uploadRootPath) Then
                Directory.CreateDirectory(_uploadRootPath)
            End If

            If Not Directory.Exists(_memberFolder) Then
                Directory.CreateDirectory(_memberFolder)
            End If

            If File.Exists(_fileName) Then
                File.Move(_fileName, Path.Combine(_memberFolder, Path.GetFileName(_fileName)))
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function
    Friend Shared Function RemoveMemberFile(memberId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant
            Dim _memberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())

            Dim _fileName As String = Path.Combine(_memberFolder, guid + _extension)

            If File.Exists(_fileName) Then
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function

    '<SymAuthorization("UploadDocuments")>
    '<Route("member/{memberId}/{currentUserId}/upload-documents")>
    '<HttpPost>
    'Public Async Function UploadDocuments(memberId As Integer, currentUserId As Integer) As Threading.Tasks.Task(Of IHttpActionResult)

    '    Try

    '        Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp") + "/" + Guid.NewGuid.ToString
    '        Dim _provider As New MultipartFormDataStreamProvider(_rootPath)

    '        If Not Directory.Exists(_rootPath) Then
    '            Directory.CreateDirectory(_rootPath)
    '        End If

    '        Await Request.Content.ReadAsMultipartAsync(_provider)

    '        File.Move(_file.LocalFileName, _targetFileName)

    '        Return Me.Ok(_uploadResponse)

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    'End Function

    <SymAuthorization("UploadMemberFiles")>
    <Route("member/{memberId}/{currentUserId}/{guid}/files")>
    <HttpPost>
    Public Async Function UploadMemberFiles(memberId As Integer, currentUserId As Integer, guid As String, <FromUri> q As UploadDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

        If Not Me.Request.Content.IsMimeMultipartContent() Then
            Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
        End If
        Try
            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")
            Dim _provider As New MultipartFormDataStreamProvider(_rootPath)

            If Not Directory.Exists(_rootPath) Then
                Directory.CreateDirectory(_rootPath)
            End If

            Await Request.Content.ReadAsMultipartAsync(_provider)

            Dim _fileName As String
            Dim _extension As String
            Dim _targetFileName As String
            Dim _detail As FileUploadDetaii
            Dim _detailList As New FileUploadDetaiiList
            Dim _uploadResponse As New FileUploadResponse
            Dim _createdCount As Integer
            Dim _failedCount As Integer
            Dim _imageInfo As MagickImageInfo
            Dim _isFileValid As Boolean
            Dim _pageCount As Integer

            For Each _file As MultipartFileData In _provider.FileData
                _fileName = guid '_file.Headers.ContentDisposition.FileName.Replace("""", "")    ' removes double quotes in string
                _extension = Path.GetExtension(_file.Headers.ContentDisposition.FileName.Replace("""", "")).ToLowerInvariant
                '_targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_file.LocalFileName).Replace("BodyPart_", "") + _extension)
                _targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), _fileName + _extension)
                _isFileValid = False
                _pageCount = 0

                _detail = New FileUploadDetaii
                _detail.FileName = _file.Headers.ContentDisposition.FileName.Replace("""", "")
                _detailList.Add(_detail)

                _imageInfo = ImageLib.GetFileInfo(_file.LocalFileName)

                File.Move(_file.LocalFileName, _targetFileName)
                _isFileValid = True

                _detail.StatusCode = 201
                _detail.StatusText = "Uploaded"
                _createdCount = _createdCount + 1

            Next

            With _uploadResponse
                .CreatedCount = _createdCount
                .FailedCount = _failedCount
                .Details = _detailList
            End With

            Return Me.Ok(_uploadResponse)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("UploadMemberDocuments")>
    <Route("member/{memberId}/{currentUserId}/documents")>
    <HttpPost>
    Public Async Function UploadMemberDocuments(memberId As Integer, currentUserId As Integer, <FromUri> q As UploadDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

        If Not Me.Request.Content.IsMimeMultipartContent() Then
            Throw New HttpResponseException(HttpStatusCode.UnsupportedMediaType)
        End If
        Try

            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")
            Dim _provider As New MultipartFormDataStreamProvider(_rootPath)

            If Not Directory.Exists(_rootPath) Then
                Directory.CreateDirectory(_rootPath)
            End If
            Await Request.Content.ReadAsMultipartAsync(_provider)

            Dim _fileName As String
            Dim _extension As String
            Dim _targetFileName As String
            Dim _detail As FileUploadDetaii
            Dim _detailList As New FileUploadDetaiiList
            Dim _uploadResponse As New FileUploadResponse
            Dim _createdCount As Integer
            Dim _failedCount As Integer
            Dim _imageInfo As MagickImageInfo
            Dim _isFileValid As Boolean
            Dim _pageCount As Integer
            'Dim _imageFormatId As Integer = DbsImageFormat.Pdf

            For Each _file As MultipartFileData In _provider.FileData
                _fileName = _file.Headers.ContentDisposition.FileName.Replace("""", "")    ' removes double quotes in string
                _extension = Path.GetExtension(_fileName).ToLowerInvariant
                _targetFileName = Path.Combine(Path.GetDirectoryName(_file.LocalFileName), Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_file.LocalFileName).Replace("BodyPart_", "") + _extension)
                _isFileValid = False
                _pageCount = 0

                _detail = New FileUploadDetaii
                _detail.FileName = _fileName
                _detailList.Add(_detail)

                _imageInfo = ImageLib.GetFileInfo(_file.LocalFileName)
                ' Accept only PDF files

                Select Case _imageInfo.Format
                    Case MagickFormat.Pdf, MagickFormat.Pdfa

                        File.Move(_file.LocalFileName, _targetFileName)

                        Try
                            ' Load the PDF with MagickImageCollection to access pages
                            Using collection As New MagickImageCollection(_targetFileName)
                                ' Get the page count (i.e., the number of pages in the PDF)

                                _pageCount = collection.Count
                            End Using
                        Catch ex As Exception
                            Console.WriteLine("Error: " & ex.Message)
                        End Try

                        '
                        ' Get number of pages
                        '
                        'Dim _pdfInfo As ImageMagick.Formats.PdfInfo = ImageMagick.Formats.PdfInfo.Create(_targetFileName)
                        'File.WriteAllText("e:\image7.txt", _pdfInfo.PageCount.ToString)

                        '_pageCount = _pdfInfo.PageCount

                        If _pageCount > 0 Then
                            _isFileValid = True

                            _detail.StatusCode = 201
                            _detail.StatusText = "Uploaded"
                            _createdCount = _createdCount + 1
                        Else
                            _detail.StatusCode = 415
                            _detail.StatusText = "PDF file is empty (no pages)"
                            _failedCount = _failedCount + 1
                        End If

                    Case Else

                        File.Delete(_file.LocalFileName)

                        _detail.StatusCode = 415
                        _detail.StatusText = "Unsupported Media Type"
                        _failedCount = _failedCount + 1

                End Select

                If _isFileValid Then
                    '
                    ' Commit document file to table
                    '
                    Try

                        ' New in .NET 4.5+ - using stream directly as parameter instead of writing first to array 
                        ' https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sqlclient-streaming-support
                        '

                        Dim _cs As String = DataLib.GetConnectionInfo(_databaseId).ConnectionString

                        Using _connection As New SqlConnection(_cs)

                            Await _connection.OpenAsync()

                            Using _command As New SqlCommand("web.InsHrsMemberDocType", _connection)

                                Using _stream As New FileStream(_targetFileName, FileMode.Open)
                                    With _command
                                        .CommandType = CommandType.StoredProcedure

                                        With .Parameters
                                            .AddWithValue("@memberId", memberId).DbType = DbType.Int32
                                            .AddWithValue("@PageCount", _pageCount)
                                            .AddWithValue("@DocTypeId", q.DocTypeId)
                                            .AddWithValue("@FileName", _fileName)
                                            .AddWithValue("@UserId", currentUserId)
                                            .Add("@DocTypeImage", SqlDbType.VarBinary, -1).Value = _stream
                                        End With

                                    End With

                                    Await _command.ExecuteNonQueryAsync()

                                End Using

                            End Using

                        End Using

                    Catch _exception As Exception

                        With _detail
                            .FileName = _fileName
                            .StatusCode = 500
                            .StatusText = "Upload failed. Problem encountered while attempting to save document file to the server."
                        End With

                        With _uploadResponse

                            .CreatedCount = 0
                            .FailedCount = 1
                        End With

                    End Try

                End If

                File.Delete(_targetFileName)

            Next

            With _uploadResponse
                .CreatedCount = _createdCount
                .FailedCount = _failedCount
                .Details = _detailList
            End With

            Return Me.Ok(_uploadResponse)

        Catch _exception As Exception
            'System.IO.File.WriteAllText("C:\Users\user\Desktop\UPLOADERROR.txt", _exception.Message + Tags.NewLine + _exception.StackTrace)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetDocTypeDocuments")>
    <Route("member/{memberIdd}/documents")>
    <HttpGet>
    Public Function GetApplicationDocuments(memberId As Integer) As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.WebGetApplicationDocuments", CommandType.StoredProcedure)
                With _direct
                    .AddParameter("memberId", memberId).DbType = DbType.Int32
                End With

                Using _table As DataTable = _direct.ExecuteDataTable()
                    Return Me.Ok(_table)
                End Using
            End Using

        Catch _exception As Exception
            'System.IO.File.WriteAllText("C:\Users\user\Desktop\GETUPLOAD.txt", _exception.Message + Tags.NewLine + _exception.StackTrace)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("MemberDownloadFile")>
    <Route("member/download-file/{memberId}/{fileName}/{guid}")>
    <HttpGet>
    Public Function GetDownloadFile(memberId As Integer, fileName As String, guid As String) As IHttpActionResult

        Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
        Dim _memberFolder As String = Path.Combine(_uploadRootPath, memberId.ToString())
        Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant

        Dim _fileName As String = Path.Combine(_memberFolder, guid + _extension)
        Dim _link As String = ""

        '_link = Path.Combine(Me.Request.RequestUri.GetLeftPart(UriPartial.Authority), memberId.ToString(), Path.GetFileName(_fileName))
        'site
        '_link = Path.Combine(Me.Request.RequestUri.GetLeftPart(UriPartial.Authority), memberId.ToString(), Path.GetFileName(_fileName))

        'dev
        _link = Path.Combine(Me.Request.RequestUri.GetLeftPart(UriPartial.Authority), "upload\" + memberId.ToString(), Path.GetFileName(_fileName))

        Return Me.Ok(_link)

    End Function

    <SymAuthorization("MemberDownloadDocument")>
    <Route("member/download-documents/{memberId}/{docTypeDetailId}")>
    <HttpGet>
    Public Async Function GetDownloadLink(memberId As Integer, docTypeDetailId As Integer) As Threading.Tasks.Task(Of IHttpActionResult)

        'If q Is Nothing Then
        '    Return Me.BadRequest("Query string is required.")
        'End If

        Dim _sql As String
        Dim _imageFormatId As Integer = 2 'q.ImageFormatId
        Dim _extension As String
        Dim _targetFileName As String


        _sql = "SELECT DocTypeImage FROM dbo.HrsMemberDocType WHERE DocTypeDetailId=" + docTypeDetailId.ToString


        'If _imageFormatId > 2 Then
        '    _imageFormatId = 2
        'End If

        'If _imageFormatId = 2 Then
        '    _extension = ".tif"
        'Else
        _extension = ".pdf"
        Dim _fileName As String = Guid.NewGuid.ToString()
        Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")
        Dim _filePath As String = Path.Combine(_rootPath, _fileName + _extension)
        Dim _pdfPagePath As String = Path.Combine(_rootPath, _fileName + ".png")


        Dim _rootPathFileDestination As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
        'Dim _rootPathFileDestination1 As String = System.Web.Hosting.HostingEnvironment.MapPath("~/public")
        'Dim _rootPathFileDestination2 As String = System.Web.Hosting.HostingEnvironment.MapPath("~/public/uploads")

        Dim _filePathDestination As String = Path.Combine("D:\eEAS\eas\public\upload\", _fileName + _extension)

        If Not Directory.Exists(_rootPath) Then
            Directory.CreateDirectory(_rootPath)
        End If

        _targetFileName = Path.Combine(Path.GetDirectoryName(_filePathDestination), Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_filePathDestination).Replace("BodyPart_", "") + _extension)

        Await CopyBlobToFile(_sql, _filePath)

        File.Move(_filePath, _targetFileName)

        'If _imageFormatId = 1 Then
        '    '
        '    ' PDF handling
        '    '
        '    Using _collection As New MagickImageCollection
        '        Dim _settings As New MagickReadSettings()

        '        With _settings
        '            '.Density = New Density(96, 96)
        '            .FrameIndex = 0      ' start index
        '            .FrameCount = 1      ' number of pages
        '        End With

        '        _collection.Read(_filePath, _settings)
        '        _collection.Write(_pdfPagePath, MagickFormat.Png)

        '        File.Delete(_filePath)

        '        _filePath = _pdfPagePath

        '    End Using

        'End If

        'Using _image As New MagickImage(_filePath)

        '    With _image
        '        .Format = MagickFormat.Jpg
        '        '.Quality = 50
        '        .Quality = 20

        '        'If .Width > 1366 Then
        '        '   .Resize(1366, 1366)
        '        'End If

        '        .Resize(640, 640)

        '        Dim _stampWidth As Integer = 220
        '        Dim _stampHeight As Integer = 100
        '        Dim _fontSize As Integer = 22

        '        If .Height < 800 Then
        '            _fontSize = 18
        '        End If

        '        Using _unofficial As New MagickImage("xc:none", _stampWidth, _stampHeight)

        '            With _unofficial
        '                Dim _drawable As New Drawables()

        '                With _drawable
        '                    '.FontPointSize(30.0)
        '                    .FontPointSize(_fontSize)

        '                    '.FillColor(New MagickColor("grey"))
        '                    .FillColor(New MagickColor("#808080"))

        '                    '.Gravity(Gravity.Northwest)
        '                    .Gravity(Gravity.Center)
        '                    '.Rotation(45)
        '                    .Text(0, 10, "Unofficial Copy")
        '                End With

        '                .Draw(_drawable)

        '            End With

        '            _image.Tile(_unofficial, CompositeOperator.Over)

        '        End Using

        '    End With

        'Using _table As New DataTable
        '        Dim _row As DataRow

        '        With _table
        '            .Columns.Add("Blob", GetType(Byte()))
        '            _row = .NewRow()
        '            _row("Blob") = _image.ToByteArray()
        '            .Rows.Add(_row)
        '        End With

        '        File.Delete(_filePath)

        'TESTING ONLY
        Return Me.Ok("./upload/" + Path.GetFileNameWithoutExtension(_fileName) + "~" + Path.GetFileNameWithoutExtension(_filePathDestination).Replace("BodyPart_", "") + _extension.ToString)

        '    End Using

        'End Using

    End Function


    '<SymAuthorization("MemberDownloadDocument")>
    '<Route("member/download-documents/{memberId}/{docTypeDetailId}")>
    '<HttpGet>
    'Public Async Function GetDownloadLink(memberId As Integer, docTypeDetailId As Integer) As Threading.Tasks.Task(Of IHttpActionResult)

    '    Dim _link As String = ""

    '    Try

    '        Dim _sql As String
    '        Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp") + "/" + Guid.NewGuid.ToString
    '        Dim _fileName As String
    '        Dim _extension As String
    '        Dim _filePath As String

    '        If Not Directory.Exists(_rootPath) Then
    '            Directory.CreateDirectory(_rootPath)
    '        End If

    '        _sql = "SELECT DocTypeImage FROM dbo.HrsMemberDocType WHERE DocTypeDetailId=" + docTypeDetailId.ToString
    '        _extension = ".pdf"
    '        _fileName = Me.ReplaceInvalidChars("") + "_" + Guid.NewGuid.ToString()
    '        _filePath = Path.Combine(_rootPath, _fileName + _extension)

    '            Await CopyBlobToFile(_sql, _filePath)

    '            Dim _pageCount As Integer
    '            Dim _targetPagePath As String
    '            Dim _targetExtension As String

    '            Dim _sourcePagePath As String
    '            Dim _sourceExtension As String

    '            Dim _settings As New MagickReadSettings
    '        'If _item.ImageFormatId = 1 Then
    '        '    _settings.Density = New Density(150)
    '        'Else
    '        '    _settings.Density = New Density(72)
    '        '    _settings.Compression = CompressionMethod.Group4
    '        'End If

    '        'Using _pages As New MagickImageCollection
    '        '        _pages.Read(_filePath, _settings)
    '        '        _pageCount = _pages.Count

    '        '        Dim _pageCounter As Integer = 1
    '        '        For Each _page As MagickImage In _pages
    '        '            If _item.ImageFormatId = 1 Then
    '        '                _page.Format = MagickFormat.Jpg
    '        '                _targetExtension = ".jpg"
    '        '            Else
    '        '                _page.Format = MagickFormat.Png
    '        '                _targetExtension = ".png"
    '        '            End If

    '        _targetPagePath = Path.Combine(Path.GetDirectoryName(_filePath), Path.GetFileName(_filePath) + "-" + _pageCounter.ToString + _targetExtension)

    '        '    _page.Write(_targetPagePath)

    '        '    _pageCounter = _pageCounter + 1
    '        'Next


    '        _pages.Write(_filePath)

    '        '
    '        ' Delete individual pages and QR code file
    '        '
    '        For Each _path As String In _sourcePages
    '                File.Delete(_path)
    '            Next

    '            File.Delete(_qrCodePath)

    '        Next

    '        Dim _mapPath As String = System.Web.Hosting.HostingEnvironment.MapPath("/downloads")
    '        'Dim _zipFilePath As String = Path.Combine(_mapPath, Guid.NewGuid.ToString + ".zip")
    '        Dim _zipFilePath As String = Path.Combine(_mapPath, trxId.ToString + "-" + Guid.NewGuid.ToString + ".zip")


    '        Try
    '            Directory.Delete(_rootPath, True)
    '        Catch When True
    '        End Try

    '        _link = Path.Combine(Me.Request.RequestUri.GetLeftPart(UriPartial.Authority), "downloads", Path.GetFileName(_zipFilePath))

    '        Try
    '            Using _sqlDirect As New SqlDirect("UPDATE dbo.WebTrx SET zipFileUrl = " + _link.Enclose(EncloseCharacter.Quote) + " WHERE TrxId =" + trxId.ToString(), CommandType.Text)
    '                _sqlDirect.ExecuteNonQuery()
    '            End Using
    '        Catch ex As Exception
    '        End Try

    '        Return Me.Ok(_link)

    '    Catch _exception As Exception
    '        Return Me.BadRequest(_exception.Message)
    '    End Try

    '    Return Me.Ok(_link)

    'End Function
    Private Async Function CopyBlobToFile(sql As String, filePath As String) As Threading.Tasks.Task

        Dim _cs As String = DataLib.GetConnectionInfo(_databaseId).ConnectionString

        Using _connection As New SqlConnection(_cs)

            Await _connection.OpenAsync()

            Using _command As New SqlCommand(sql, _connection)
                Using _reader As SqlDataReader = Await _command.ExecuteReaderAsync(CommandBehavior.SequentialAccess)
                    If Await _reader.ReadAsync() Then
                        If Not Await _reader.IsDBNullAsync(0) Then
                            Using _stream As New FileStream(filePath, FileMode.Create, FileAccess.Write)
                                Using _data As Stream = _reader.GetStream(0)
                                    Await _data.CopyToAsync(_stream)
                                End Using
                            End Using
                        End If
                    End If
                End Using
            End Using

        End Using

    End Function

    Private Function ReplaceInvalidChars(fileName As String) As String
        '
        ' Replace each invalid filename character with space
        '
        Return String.Join(" ", fileName.Split(Path.GetInvalidFileNameChars()))
    End Function

End Class


'Public Class MemberBody
'    Inherits HrsMember

'End Class

Public Class MemberBody
    Inherits HrsMember
    Public Property Languages As HrsMemberLanguage()
    Public Property Disabilities As HrsMemberDisability()
    Public Property RnrRecordings As HrsMemberRnrRecording()
    Public Property Vaccines As HrsMemberVaccine()
    Public Property SkillSets As HrsMemberSkillSet()
    Public Property Kins As HrsMemberKin()
    Public Property Docs As HrsMemberDocType()
    Public Property Educations As HrsMemberEducation()
    Public Property Licenses As HrsMemberLicenseProfession()
    Public Property CDATrainings As HrsMemberCDATraining()
    Public Property NCIIs As HrsMemberNCIIQualificationTitle()
    Public Property Compliances As HrsMemberComplianceTraining()
    Public Property Works As HrsMemberWork()
    Public Property Affiliations As HrsMemberAffiliation()
    Public Property Medicals As HrsMemberMedicalResultType()

    'Public Property Certificates As HrsMemberCertificate()
    'Public Property Eligibilities As HrsMemberEligibility()
    ''Public Property Licenses As HrsMemberLicense()

    ''Public Property NCIIQualificationTitles As HrsMemberNCIIQualificationTitle()

    'Public Property SoloParent As HrsMemberSoloParent()
End Class


'Public Class PayTrxBody
'    Inherits PayTrx

'    Public Property Details As PayTrxDetail()

'End Class

Public Class UploadPhotoParams
    Public Property Width As Integer
    Public Property Quality As Integer
    Public Property GUID As String
End Class

'Public Class UploadImageQuery
'    Public Property Width As Integer
'    Public Property Quality As Integer

'End Class
Public Class UploadDocumentsQuery
    Public Property DocTypeId As Integer
    Public Property GUID As String
End Class

Public Class MemberDetailInquiryQuery

    Public Property MemberLastName As String
    Public Property MemberFirstName As String
    Public Property MemberMiddleName As String
    'Public Property BirthDate As Date

End Class

