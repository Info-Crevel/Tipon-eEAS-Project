<RoutePrefix("api")>
Public Class ARS0050_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   '<SymAuthorization("GetClientBillingTransactions")>
   '<Route("client-billing-transactions/{clientPayGroupId}")>
   '<HttpGet>
   'Public Function GetClientBillingTransactions(clientPayGroupId As Integer) As IHttpActionResult

   '   Dim _filter As String = "ClientPayGroupId=" + clientPayGroupId.ToString
   '   Dim _dataSource As String = "dbo.ArsClientBilling"
   '   Dim _fields As String = "ClientPayGroupId, PayTrxCode, BillingSheetFormula, FixedAmount"
   '   Dim _sort As String = "ClientBillingDetailId"

   '   Try
   '      Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
   '         Return Me.Ok(_table)
   '      End Using

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function

   '<SymAuthorization("GetClientPayOutTransactions")>
   '<Route("client-pay-out-transactions/{clientPayGroupId}")>
   '<HttpGet>
   'Public Function GetClientPayOutTransactions(clientPayGroupId As Integer) As IHttpActionResult

   '   Dim _filter As String = "ClientPayGroupId=" + clientPayGroupId.ToString
   '   Dim _dataSource As String = "dbo.ArsClientPayOut"
   '   Dim _fields As String = "ClientPayGroupId, PayTrxCode, PayOutSheetFormula, FixedAmount"
   '   Dim _sort As String = "ClientPayOutDetailId"

   '   Try
   '      Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
   '         Return Me.Ok(_table)
   '      End Using

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function

   '<SymAuthorization("GetRequestTypeQualifications")>
   '<Route("request-type-qualifications/{memberTypeId}")>
   '<HttpGet>
   'Public Function GetRequestTypeQualifications(memberTypeId As Integer) As IHttpActionResult

   '   Dim _filter As String = "MemberTypeId=" + memberTypeId.ToString
   '   Dim _dataSource As String = "dbo.HrsMemberTypeQualification"
   '   Dim _fields As String = "TypeQualificationDetailId, MemberTypeId, TypeQualificationName, SortSeq"
   '   Dim _sort As String = "TypeQualificationName"

   '   Try
   '      Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
   '         Return Me.Ok(_table)
   '      End Using

   '   Catch _exception As Exception
   '      Return Me.BadRequest(_exception.Message)
   '   End Try

   'End Function
   'Public Function GetRequestPoolingMembers(filter As String, positionKeywordFilter As String, skillNameKeywordFilter As String) As IHttpActionResult

   '<SymAuthorization("GetRequestPoolingMembers")>
   '<Route("request-pooling-members/{filter}/{positionKeywordFilter}/{skillNameKeywordFilter}")>

   '<SymAuthorization("GetRequestPoolingMembers")>
   '<Route("request-pooling-members/inquiry")>
   '<HttpGet>

   <SymAuthorization("GetRequestPoolingMembers")>
   <Route("request-pooling-members")>
   <HttpPost>
   Public Function GetRequestPoolingMembers(<FromBody> filterParam As FilterParam) As IHttpActionResult


      Dim positionKeywordFilter As String = String.Empty
      'File.WriteAllText("e:\aaa1.txt", positionKeywordFilter)

      If Not String.IsNullOrEmpty(filterParam.positionKeyword) And filterParam.positionKeyword <> "0" Then
         Dim keywords As String() = filterParam.positionKeyword.Split(";"c)
         Dim likeConditions As New List(Of String)

         For Each keyword As String In keywords
            Dim trimmedKeyword As String = keyword.Trim()
            If Not String.IsNullOrEmpty(trimmedKeyword) Then
               likeConditions.Add("WorkPosition LIKE '%" & trimmedKeyword & "%'")
            End If
         Next
         If likeConditions.Count > 0 Then
            positionKeywordFilter = " AND (" & String.Join(" OR ", likeConditions) & ")"
         End If
      End If

      'File.WriteAllText("e:\aaa.txt", positionKeywordFilterA)
      Dim skillNameKeywordFilter As String = String.Empty

      If Not String.IsNullOrEmpty(filterParam.skillNameKeyword) And filterParam.skillNameKeyword <> "0" Then
         Dim keywords As String() = filterParam.skillNameKeyword.Split(";"c)
         Dim likeConditions As New List(Of String)

         For Each keyword As String In keywords
            Dim trimmedKeyword As String = keyword.Trim()

            If Not String.IsNullOrEmpty(trimmedKeyword) Then
               likeConditions.Add("SkillDetailName LIKE '%" & trimmedKeyword & "%'")
            End If
         Next

         If likeConditions.Count > 0 Then
            skillNameKeywordFilter = " AND (" & String.Join(" OR ", likeConditions) & ")"
         End If
      End If
      'File.WriteAllText("e:\bbb.txt", skillNameKeywordFilterA)

      Dim _filter As String = filterParam.filter + positionKeywordFilter + skillNameKeywordFilter

      File.WriteAllText("e:\filter.txt", _filter)

      Dim _dataSource As String = "dbo.QArsMemberRequestPooling"
      Dim _fields As String = "MemberId, ApplicantId, MemberName, BirthDate, Age, SexName, MobileNumber"
      Dim _sort As String = "MemberName"

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetReferences_ARS0050")>
   <Route("references/ars0050")>
   <HttpGet>
   Public Function GetReferences_ARS0050() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.ARS0050_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "org"     ' all defined Transaction Types
                     .Tables(1).TableName = "platform"     ' all defined Transaction Types
                     .Tables(2).TableName = "contract"     ' all defined Transaction Types
                     .Tables(3).TableName = "position"     ' all defined Transaction Types
                     .Tables(4).TableName = "type"     ' all defined Transaction Types
                     .Tables(5).TableName = "payout"     ' all defined Transaction Types
                     .Tables(6).TableName = "educationLevel"     ' all defined Transaction Types
                     .Tables(7).TableName = "region"     ' all defined Transaction Types
                     .Tables(8).TableName = "religion"     ' all defined Transaction Types
                     .Tables(9).TableName = "civilStatus"     ' all defined Transaction Types
                     .Tables(10).TableName = "sex"     ' all defined Transaction Types
                     .Tables(11).TableName = "employmentType"     ' all defined Transaction Types
                     .Tables(12).TableName = "nciiTitle"     ' all defined Transaction Types
                     .Tables(13).TableName = "licenseTitle"     ' all defined Transaction Types
                     .Tables(14).TableName = "cdaTitle"     ' all defined Transaction Types
                     .Tables(15).TableName = "complianceTitle"     ' all defined Transaction Types
                     .Tables(16).TableName = "medicalResultType"     ' all defined Transaction Types
                     .Tables(17).TableName = "payGroup"     ' all defined Transaction Types
                     .Tables(18).TableName = "payTrx"     ' all defined Transaction Types
                     .Tables(19).TableName = "applicantScreening"     ' all defined Transaction Types

                     '.Tables(7).TableName = "region"     ' all defined Transaction Types
                     '.Tables(8).TableName = "province"     ' all defined Transaction Types
                     '.Tables(9).TableName = "municipality"     ' all defined Transaction Types
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetMemberRequestPooling")>
   <Route("member-request-pooling/{memberRequestId}")>
   <HttpGet>
   Public Function GetMemberRequestPooling(memberRequestId As Integer) As IHttpActionResult

      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0050")
            With _direct
               .AddParameter("MemberRequestId", memberRequestId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "request"
                     .Tables(1).TableName = "province"
                     .Tables(2).TableName = "municipality"
                     .Tables(3).TableName = "educations"
                     .Tables(4).TableName = "religions"
                     .Tables(5).TableName = "civilStatus"
                     .Tables(6).TableName = "sexes"
                     .Tables(7).TableName = "employmentTypes"
                     .Tables(8).TableName = "typeQualifications"
                     .Tables(9).TableName = "memberTypeQualifications"
                     .Tables(10).TableName = "nciis"
                     .Tables(11).TableName = "licenses"
                     .Tables(12).TableName = "cdas"
                     .Tables(13).TableName = "compliances"
                     .Tables(14).TableName = "medicalResults"
                     .Tables(15).TableName = "billings"
                     .Tables(16).TableName = "payOuts"
                     .Tables(17).TableName = "screenings"

                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetRequestPoolingProvinces")>
   <Route("request-pooling-provinces/{regionId}")>
   <HttpGet>
   Public Function GetRequestPoolingProvinces(regionId As Integer) As IHttpActionResult
      Dim _filter As String = "RegionId=" + regionId.ToString
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

   <SymAuthorization("GetRequestPoolingMunicipalities")>
   <Route("request-pooling-municipalities/{regionId}/{provinceId}")>
   <HttpGet>
   Public Function GetRequestPoolingMunicipalities(regionId As Integer, provinceId As Integer) As IHttpActionResult

      Dim _filter As String = "RegionId=" + regionId.ToString + " AND ProvinceId=" + provinceId.ToString
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

   <SymAuthorization("CreateMemberRequest")>
   <Route("member-request-pooling/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberRequest(currentUserId As Integer, <FromBody> memberRequest As ArsMemberRequestBody) As IHttpActionResult

      If memberRequest.MemberRequestId <> -1 Then
         Throw New ArgumentException("Member Request ID not recognized.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)

         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Assign new ID from sequencer
         '
         Dim _memberRequestId As Integer = SysLib.GetNextSequence("MemberRequestId")

         memberRequest.MemberRequestId = _memberRequestId
         memberRequest.MemberRequestStatusId = MemberRequestStatus.InActive

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberRequest As New ArsMemberRequest
         Dim _arsMemberRequestEducationList As New ArsMemberRequestEducationList
         Dim _arsMemberRequestReligionList As New ArsMemberRequestReligionList
         Dim _arsMemberRequestCivilStatusList As New ArsMemberRequestCivilStatusList
         Dim _arsMemberRequestSexList As New ArsMemberRequestSexList
         Dim _arsMemberRequestEmploymentTypeList As New ArsMemberRequestEmploymentTypeList
         Dim _arsMemberRequestTypeQualificationList As New ArsMemberRequestTypeQualificationList
         Dim _arsMemberRequestNCIIList As New ArsMemberRequestNCIIList
         Dim _arsMemberRequestLicenseProfessionList As New ArsMemberRequestLicenseProfessionList
         Dim _arsMemberRequestCDATrainingList As New ArsMemberRequestCDATrainingList
         Dim _arsMemberRequestComplianceTrainingList As New ArsMemberRequestComplianceTrainingList
         Dim _arsMemberRequestMedicalResultList As New ArsMemberRequestMedicalResultList

         Dim _arsMemberRequestBillingList As New ArsMemberRequestBillingList
         Dim _arsMemberRequestPayOutList As New ArsMemberRequestPayOutList
         Dim _arsMemberRequestScreeningList As New ArsMemberRequestScreeningList

         Me.LoadArsMemberRequest(memberRequest, _arsMemberRequest)

         For Each _education As ArsMemberRequestEducation In memberRequest.Educations
            _education.MemberRequestId = _memberRequestId
            _arsMemberRequestEducationList.Add(_education)
         Next

         For Each _religion As ArsMemberRequestReligion In memberRequest.Religions
            _religion.MemberRequestId = _memberRequestId
            _arsMemberRequestReligionList.Add(_religion)
         Next

         For Each _civilStatus As ArsMemberRequestCivilStatus In memberRequest.CivilStatus
            _civilStatus.MemberRequestId = _memberRequestId
            _arsMemberRequestCivilStatusList.Add(_civilStatus)
         Next

         For Each _sex As ArsMemberRequestSex In memberRequest.Sexes
            _sex.MemberRequestId = _memberRequestId
            _arsMemberRequestSexList.Add(_sex)
         Next

         For Each _employmentType As ArsMemberRequestEmploymentType In memberRequest.EmploymentTypes
            _employmentType.MemberRequestId = _memberRequestId
            _arsMemberRequestEmploymentTypeList.Add(_employmentType)
         Next

         For Each _typeQualification As ArsMemberRequestTypeQualification In memberRequest.TypeQualifications
            _typeQualification.MemberRequestId = _memberRequestId
            _arsMemberRequestTypeQualificationList.Add(_typeQualification)
         Next

         For Each _ncii As ArsMemberRequestNCII In memberRequest.NCIIs
            _ncii.MemberRequestId = _memberRequestId
            _arsMemberRequestNCIIList.Add(_ncii)
         Next

         For Each _license As ArsMemberRequestLicenseProfession In memberRequest.Licenses
            _license.MemberRequestId = _memberRequestId
            _arsMemberRequestLicenseProfessionList.Add(_license)
         Next

         For Each _compliance As ArsMemberRequestComplianceTraining In memberRequest.Compliances
            _compliance.MemberRequestId = _memberRequestId
            _arsMemberRequestComplianceTrainingList.Add(_compliance)
         Next

         For Each _medicalResult As ArsMemberRequestMedicalResult In memberRequest.MedicalResults
            _medicalResult.MemberRequestId = _memberRequestId
            _arsMemberRequestMedicalResultList.Add(_medicalResult)
         Next

         For Each _billing As ArsMemberRequestBilling In memberRequest.Billings
            _billing.MemberRequestId = _memberRequestId
            _arsMemberRequestBillingList.Add(_billing)
         Next

         For Each _payOut As ArsMemberRequestPayOut In memberRequest.PayOutS
            _payOut.MemberRequestId = _memberRequestId
            _arsMemberRequestPayOutList.Add(_payOut)
         Next

         For Each _screening As ArsMemberRequestScreening In memberRequest.Screenings
            _screening.MemberRequestId = _memberRequestId
            _arsMemberRequestScreeningList.Add(_screening)
         Next

         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.InsertArsMemberRequest(_arsMemberRequest)

            If _arsMemberRequestEducationList.Count > 0 Then
               Me.InsertArsMemberRequestEducations(_arsMemberRequestEducationList)
            End If

            If _arsMemberRequestReligionList.Count > 0 Then
               Me.InsertArsMemberRequestReligions(_arsMemberRequestReligionList)
            End If

            If _arsMemberRequestCivilStatusList.Count > 0 Then
               Me.InsertArsMemberRequestCivilStatus(_arsMemberRequestCivilStatusList)
            End If

            If _arsMemberRequestSexList.Count > 0 Then
               Me.InsertArsMemberRequestSex(_arsMemberRequestSexList)
            End If

            If _arsMemberRequestEmploymentTypeList.Count > 0 Then
               Me.InsertArsMemberRequestEmploymentTypes(_arsMemberRequestEmploymentTypeList)
            End If

            If _arsMemberRequestTypeQualificationList.Count > 0 Then
               Me.InsertArsMemberRequestTypeQualifications(_arsMemberRequestTypeQualificationList)
            End If

            If _arsMemberRequestNCIIList.Count > 0 Then
               Me.InsertArsMemberRequestNCIIs(_arsMemberRequestNCIIList)
            End If

            If _arsMemberRequestLicenseProfessionList.Count > 0 Then
               Me.InsertArsMemberRequestLicenseProfessions(_arsMemberRequestLicenseProfessionList)
            End If

            If _arsMemberRequestCDATrainingList.Count > 0 Then
               Me.InsertArsMemberRequestCDATrainings(_arsMemberRequestCDATrainingList)
            End If

            If _arsMemberRequestComplianceTrainingList.Count > 0 Then
               Me.InsertArsMemberRequestComplianceTrainings(_arsMemberRequestComplianceTrainingList)
            End If

            If _arsMemberRequestMedicalResultList.Count > 0 Then
               Me.InsertArsMemberRequestMedicalResults(_arsMemberRequestMedicalResultList)
            End If

            If _arsMemberRequestBillingList.Count > 0 Then
               Me.InsertArsMemberRequestBillings(_arsMemberRequestBillingList)
            End If

            If _arsMemberRequestPayOutList.Count > 0 Then
               Me.InsertArsMemberRequestPayOuts(_arsMemberRequestPayOutList)
            End If

            If _arsMemberRequestScreeningList.Count > 0 Then
               Me.InsertArsMemberRequestScreenings(_arsMemberRequestScreeningList)
            End If

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

         Return Me.Ok(memberRequest.MemberRequestId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("ModifyArsMemberRequest")>
   <Route("member-request-pooling/{memberRequestId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyArsMemberRequest(memberRequestId As Integer, currentUserId As Integer, <FromBody> memberRequest As ArsMemberRequestBody) As IHttpActionResult

      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
         End If

         '
         ' Load proposed values from payload
         '
         Dim _arsMemberRequest As New ArsMemberRequest
         Dim _arsMemberRequestEducationList As New ArsMemberRequestEducationList
         Dim _arsMemberRequestReligionList As New ArsMemberRequestReligionList
         Dim _arsMemberRequestCivilStatusList As New ArsMemberRequestCivilStatusList
         Dim _arsMemberRequestSexList As New ArsMemberRequestSexList
         Dim _arsMemberRequestEmploymentTypeList As New ArsMemberRequestEmploymentTypeList
         Dim _arsMemberRequestTypeQualificationList As New ArsMemberRequestTypeQualificationList
         Dim _arsMemberRequestNCIIList As New ArsMemberRequestNCIIList
         Dim _arsMemberRequestLicenseProfessionList As New ArsMemberRequestLicenseProfessionList
         Dim _arsMemberRequestCDATrainingList As New ArsMemberRequestCDATrainingList
         Dim _arsMemberRequestComplianceTrainingList As New ArsMemberRequestComplianceTrainingList

         Dim _arsMemberRequestMedicalResultList As New ArsMemberRequestMedicalResultList
         Dim _arsMemberRequestBillingList As New ArsMemberRequestBillingList
         Dim _arsMemberRequestPayOutList As New ArsMemberRequestPayOutList
         Dim _arsMemberRequestScreeningList As New ArsMemberRequestScreeningList

         Me.LoadArsMemberRequest(memberRequest, _arsMemberRequest)

         For Each _education As ArsMemberRequestEducation In memberRequest.Educations
            _arsMemberRequestEducationList.Add(_education)
         Next

         For Each _religion As ArsMemberRequestReligion In memberRequest.Religions
            _arsMemberRequestReligionList.Add(_religion)
         Next

         For Each _civilStatus As ArsMemberRequestCivilStatus In memberRequest.CivilStatus
            _arsMemberRequestCivilStatusList.Add(_civilStatus)
         Next

         For Each _sex As ArsMemberRequestSex In memberRequest.Sexes
            _arsMemberRequestSexList.Add(_sex)
         Next

         For Each _employmentType As ArsMemberRequestEmploymentType In memberRequest.EmploymentTypes
            _arsMemberRequestEmploymentTypeList.Add(_employmentType)
         Next

         For Each _typeQualification As ArsMemberRequestTypeQualification In memberRequest.TypeQualifications
            _arsMemberRequestTypeQualificationList.Add(_typeQualification)
         Next

         For Each _nciis As ArsMemberRequestNCII In memberRequest.NCIIs
            _arsMemberRequestNCIIList.Add(_nciis)
         Next

         For Each _license As ArsMemberRequestLicenseProfession In memberRequest.Licenses
            _arsMemberRequestLicenseProfessionList.Add(_license)
         Next

         For Each _cda As ArsMemberRequestCDATraining In memberRequest.CDAs
            _arsMemberRequestCDATrainingList.Add(_cda)
         Next

         For Each _compliance As ArsMemberRequestComplianceTraining In memberRequest.Compliances
            _arsMemberRequestComplianceTrainingList.Add(_compliance)
         Next

         For Each _medicalResult As ArsMemberRequestMedicalResult In memberRequest.MedicalResults
            _arsMemberRequestMedicalResultList.Add(_medicalResult)
         Next

         For Each _billing As ArsMemberRequestBilling In memberRequest.Billings
            _arsMemberRequestBillingList.Add(_billing)
         Next

         For Each _payOut As ArsMemberRequestPayOut In memberRequest.PayOutS
            _arsMemberRequestPayOutList.Add(_payOut)
         Next

         For Each _screening As ArsMemberRequestScreening In memberRequest.Screenings
            _arsMemberRequestScreeningList.Add(_screening)
         Next

         '
         ' Load old values from DB
         '
         Dim _arsMemberRequestOld As New ArsMemberRequest
         Dim _arsMemberRequestEducationListOld As New ArsMemberRequestEducationList

         Dim _arsMemberRequestReligionListOld As New ArsMemberRequestReligionList
         Dim _arsMemberRequestCivilStatusListOld As New ArsMemberRequestCivilStatusList
         Dim _arsMemberRequestSexListOld As New ArsMemberRequestSexList
         Dim _arsMemberRequestEmploymentTypeListOld As New ArsMemberRequestEmploymentTypeList
         Dim _arsMemberRequestTypeQualificationListOld As New ArsMemberRequestTypeQualificationList
         Dim _arsMemberRequestNCIIListOld As New ArsMemberRequestNCIIList
         Dim _arsMemberRequestLicenseProfessionListOld As New ArsMemberRequestLicenseProfessionList
         Dim _arsMemberRequestCDATrainingListOld As New ArsMemberRequestCDATrainingList
         Dim _arsMemberRequestComplianceTrainingListOld As New ArsMemberRequestComplianceTrainingList

         Dim _arsMemberRequestMedicalResultListOld As New ArsMemberRequestMedicalResultList
         Dim _arsMemberRequestBillingListOld As New ArsMemberRequestBillingList
         Dim _arsMemberRequestPayOutListOld As New ArsMemberRequestPayOutList
         Dim _arsMemberRequestScreeningListOld As New ArsMemberRequestScreeningList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetMemberRequestPooling(memberRequestId), Results.OkNegotiatedContentResult(Of DataSet))

         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("request").Rows(0)
            Me.LoadArsMemberRequest(_row, _arsMemberRequestOld)
            Me.LoadArsMemberRequestEducationList(_dataSet.Tables("educations").Rows, _arsMemberRequestEducationListOld)
            Me.LoadArsMemberRequestReligionList(_dataSet.Tables("religions").Rows, _arsMemberRequestReligionListOld)
            Me.LoadArsMemberRequestCivilStatusList(_dataSet.Tables("civilStatus").Rows, _arsMemberRequestCivilStatusListOld)
            Me.LoadArsMemberRequestSexList(_dataSet.Tables("sexes").Rows, _arsMemberRequestSexListOld)
            Me.LoadArsMemberRequestEmploymentTypeList(_dataSet.Tables("employmentTypes").Rows, _arsMemberRequestEmploymentTypeListOld)
            Me.LoadArsMemberRequestTypeQualificationList(_dataSet.Tables("typeQualifications").Rows, _arsMemberRequestTypeQualificationListOld)
            Me.LoadArsMemberRequestNCIIList(_dataSet.Tables("nciis").Rows, _arsMemberRequestNCIIListOld)
            Me.LoadArsMemberRequestLicenseProfessionList(_dataSet.Tables("licenses").Rows, _arsMemberRequestLicenseProfessionListOld)
            Me.LoadArsMemberRequestCDATrainingList(_dataSet.Tables("cdas").Rows, _arsMemberRequestCDATrainingListOld)
            Me.LoadArsMemberRequestComplianceTrainingList(_dataSet.Tables("compliances").Rows, _arsMemberRequestComplianceTrainingListOld)
            Me.LoadArsMemberRequestMedicalResultList(_dataSet.Tables("medicalResults").Rows, _arsMemberRequestMedicalResultListOld)
            Me.LoadArsMemberRequestBillingList(_dataSet.Tables("billings").Rows, _arsMemberRequestBillingListOld)
            Me.LoadArsMemberRequestPayOutList(_dataSet.Tables("payOuts").Rows, _arsMemberRequestPayOutListOld)
            Me.LoadArsMemberRequestScreeningList(_dataSet.Tables("screenings").Rows, _arsMemberRequestScreeningListOld)

         End Using

#Region "ArsMemberRequestEducation"

         Dim _removeEducationCount As Integer
         Dim _addEducationCount As Integer
         Dim _editEducationCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestEducation In _arsMemberRequestEducationListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestEducation In _arsMemberRequestEducationList
               If _new.MemberRequestEducationDetailId = _old.MemberRequestEducationDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeEducationCount = _removeEducationCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestEducation In _arsMemberRequestEducationList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestEducation In _arsMemberRequestEducationListOld
               If _new.MemberRequestEducationDetailId = _old.MemberRequestEducationDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .EducationLevelId <> _old.EducationLevelId Then
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

         Dim _arsMemberRequestEducationListNew As New ArsMemberRequestEducationList      ' for adding new Template Details

         If _addEducationCount > 0 Then
            Dim _arsMemberRequestEducation As ArsMemberRequestEducation

            For Each _new As ArsMemberRequestEducation In _arsMemberRequestEducationList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestEducation = New ArsMemberRequestEducation
                  _arsMemberRequestEducationListNew.Add(_arsMemberRequestEducation)
                  DataLib.ScatterValues(_new, _arsMemberRequestEducation)
                  _arsMemberRequestEducation.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestReligion"

         Dim _removeReligionCount As Integer
         Dim _addReligionCount As Integer
         Dim _editReligionCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestReligion In _arsMemberRequestReligionListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestReligion In _arsMemberRequestReligionList
               If _new.MemberRequestReligionDetailId = _old.MemberRequestReligionDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeReligionCount = _removeReligionCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestReligion In _arsMemberRequestReligionList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestReligion In _arsMemberRequestReligionListOld
               If _new.MemberRequestReligionDetailId = _old.MemberRequestReligionDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .ReligionId <> _old.ReligionId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addReligionCount = _addReligionCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editReligionCount = _editReligionCount + 1
            End If

         Next

         Dim _arsMemberRequestReligionListNew As New ArsMemberRequestReligionList      ' for adding new Template Details

         If _addReligionCount > 0 Then
            Dim _arsMemberRequestReligion As ArsMemberRequestReligion

            For Each _new As ArsMemberRequestReligion In _arsMemberRequestReligionList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestReligion = New ArsMemberRequestReligion
                  _arsMemberRequestReligionListNew.Add(_arsMemberRequestReligion)
                  DataLib.ScatterValues(_new, _arsMemberRequestReligion)
                  _arsMemberRequestReligion.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestCivilStatus"

         Dim _removeCivilStatusCount As Integer
         Dim _addCivilStatusCount As Integer
         Dim _editCivilStatusCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestCivilStatus In _arsMemberRequestCivilStatusListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestCivilStatus In _arsMemberRequestCivilStatusList
               If _new.MemberRequestCivilStatusDetailId = _old.MemberRequestCivilStatusDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeCivilStatusCount = _removeCivilStatusCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestCivilStatus In _arsMemberRequestCivilStatusList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestCivilStatus In _arsMemberRequestCivilStatusListOld
               If _new.MemberRequestCivilStatusDetailId = _old.MemberRequestCivilStatusDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .CivilStatusId <> _old.CivilStatusId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addCivilStatusCount = _addCivilStatusCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editCivilStatusCount = _editCivilStatusCount + 1
            End If

         Next

         Dim _arsMemberRequestCivilStatusListNew As New ArsMemberRequestCivilStatusList      ' for adding new Template Details

         If _addCivilStatusCount > 0 Then
            Dim _arsMemberRequestCivilStatus As ArsMemberRequestCivilStatus

            For Each _new As ArsMemberRequestCivilStatus In _arsMemberRequestCivilStatusList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestCivilStatus = New ArsMemberRequestCivilStatus
                  _arsMemberRequestCivilStatusListNew.Add(_arsMemberRequestCivilStatus)
                  DataLib.ScatterValues(_new, _arsMemberRequestCivilStatus)
                  _arsMemberRequestCivilStatus.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestSex"

         Dim _removeSexCount As Integer
         Dim _addSexCount As Integer
         Dim _editSexCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestSex In _arsMemberRequestSexListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestSex In _arsMemberRequestSexList
               If _new.MemberRequestSexDetailId = _old.MemberRequestSexDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeSexCount = _removeSexCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestSex In _arsMemberRequestSexList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestSex In _arsMemberRequestSexListOld
               If _new.MemberRequestSexDetailId = _old.MemberRequestSexDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .SexId <> _old.SexId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addSexCount = _addSexCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editSexCount = _editSexCount + 1
            End If

         Next

         Dim _arsMemberRequestSexListNew As New ArsMemberRequestSexList      ' for adding new Template Details

         If _addSexCount > 0 Then
            Dim _arsMemberRequestSex As ArsMemberRequestSex

            For Each _new As ArsMemberRequestSex In _arsMemberRequestSexList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestSex = New ArsMemberRequestSex
                  _arsMemberRequestSexListNew.Add(_arsMemberRequestSex)
                  DataLib.ScatterValues(_new, _arsMemberRequestSex)
                  _arsMemberRequestSex.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestEmploymentType"

         Dim _removeEmploymentTypeCount As Integer
         Dim _addEmploymentTypeCount As Integer
         Dim _editEmploymentTypeCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestEmploymentType In _arsMemberRequestEmploymentTypeListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestEmploymentType In _arsMemberRequestEmploymentTypeList
               If _new.MemberRequestEmploymentTypeDetailId = _old.MemberRequestEmploymentTypeDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeEmploymentTypeCount = _removeEmploymentTypeCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestEmploymentType In _arsMemberRequestEmploymentTypeList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestEmploymentType In _arsMemberRequestEmploymentTypeListOld
               If _new.MemberRequestEmploymentTypeDetailId = _old.MemberRequestEmploymentTypeDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .EmploymentTypeId <> _old.EmploymentTypeId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addEmploymentTypeCount = _addEmploymentTypeCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editEmploymentTypeCount = _editEmploymentTypeCount + 1
            End If

         Next

         Dim _arsMemberRequestEmploymentTypeListNew As New ArsMemberRequestEmploymentTypeList      ' for adding new Template Details

         If _addEmploymentTypeCount > 0 Then
            Dim _arsMemberRequestEmploymentType As ArsMemberRequestEmploymentType

            For Each _new As ArsMemberRequestEmploymentType In _arsMemberRequestEmploymentTypeList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestEmploymentType = New ArsMemberRequestEmploymentType
                  _arsMemberRequestEmploymentTypeListNew.Add(_arsMemberRequestEmploymentType)
                  DataLib.ScatterValues(_new, _arsMemberRequestEmploymentType)
                  _arsMemberRequestEmploymentType.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestTypeQualification"

         Dim _removeTypeQualificationCount As Integer
         Dim _addTypeQualificationCount As Integer
         Dim _editTypeQualificationCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestTypeQualification In _arsMemberRequestTypeQualificationListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestTypeQualification In _arsMemberRequestTypeQualificationList
               If _new.MemberRequestTypeQualificationDetailId = _old.MemberRequestTypeQualificationDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeTypeQualificationCount = _removeTypeQualificationCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestTypeQualification In _arsMemberRequestTypeQualificationList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestTypeQualification In _arsMemberRequestTypeQualificationListOld
               If _new.MemberRequestTypeQualificationDetailId = _old.MemberRequestTypeQualificationDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .TypeQualificationDetailId <> _old.TypeQualificationDetailId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addTypeQualificationCount = _addTypeQualificationCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editTypeQualificationCount = _editTypeQualificationCount + 1
            End If

         Next

         Dim _arsMemberRequestTypeQualificationListNew As New ArsMemberRequestTypeQualificationList      ' for adding new Template Details

         If _addTypeQualificationCount > 0 Then
            Dim _arsMemberRequestTypeQualification As ArsMemberRequestTypeQualification

            For Each _new As ArsMemberRequestTypeQualification In _arsMemberRequestTypeQualificationList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestTypeQualification = New ArsMemberRequestTypeQualification
                  _arsMemberRequestTypeQualificationListNew.Add(_arsMemberRequestTypeQualification)
                  DataLib.ScatterValues(_new, _arsMemberRequestTypeQualification)
                  _arsMemberRequestTypeQualification.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestNCII"

         Dim _removeNCIICount As Integer
         Dim _addNCIICount As Integer
         Dim _editNCIICount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestNCII In _arsMemberRequestNCIIListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestNCII In _arsMemberRequestNCIIList
               If _new.NCIIDetailId = _old.NCIIDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeNCIICount = _removeNCIICount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestNCII In _arsMemberRequestNCIIList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestNCII In _arsMemberRequestNCIIListOld
               If _new.NCIIDetailId = _old.NCIIDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .NCIIQualificationTitleId <> _old.NCIIQualificationTitleId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addNCIICount = _addNCIICount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editNCIICount = _editNCIICount + 1
            End If

         Next

         Dim _arsMemberRequestNCIIListNew As New ArsMemberRequestNCIIList      ' for adding new Template Details

         If _addNCIICount > 0 Then
            Dim _arsMemberRequestNCII As ArsMemberRequestNCII

            For Each _new As ArsMemberRequestNCII In _arsMemberRequestNCIIList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestNCII = New ArsMemberRequestNCII
                  _arsMemberRequestNCIIListNew.Add(_arsMemberRequestNCII)
                  DataLib.ScatterValues(_new, _arsMemberRequestNCII)
                  _arsMemberRequestNCII.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestLicenseProfession"

         Dim _removeLicenseProfessionCount As Integer
         Dim _addLicenseProfessionCount As Integer
         Dim _editLicenseProfessionCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestLicenseProfession In _arsMemberRequestLicenseProfessionListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestLicenseProfession In _arsMemberRequestLicenseProfessionList
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

         For Each _new As ArsMemberRequestLicenseProfession In _arsMemberRequestLicenseProfessionList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestLicenseProfession In _arsMemberRequestLicenseProfessionListOld
               If _new.LicenseProfessionDetailId = _old.LicenseProfessionDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .LicenseProfessionId <> _old.LicenseProfessionId Then
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

         Dim _arsMemberRequestLicenseProfessionListNew As New ArsMemberRequestLicenseProfessionList      ' for adding new Template Details

         If _addLicenseProfessionCount > 0 Then
            Dim _arsMemberRequestLicenseProfession As ArsMemberRequestLicenseProfession

            For Each _new As ArsMemberRequestLicenseProfession In _arsMemberRequestLicenseProfessionList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestLicenseProfession = New ArsMemberRequestLicenseProfession
                  _arsMemberRequestLicenseProfessionListNew.Add(_arsMemberRequestLicenseProfession)
                  DataLib.ScatterValues(_new, _arsMemberRequestLicenseProfession)
                  _arsMemberRequestLicenseProfession.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestCDATraining"

         Dim _removeCDATrainingCount As Integer
         Dim _addCDATrainingCount As Integer
         Dim _editCDATrainingCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestCDATraining In _arsMemberRequestCDATrainingListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestCDATraining In _arsMemberRequestCDATrainingList
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

         For Each _new As ArsMemberRequestCDATraining In _arsMemberRequestCDATrainingList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestCDATraining In _arsMemberRequestCDATrainingListOld
               If _new.CDATrainingDetailId = _old.CDATrainingDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .CDATrainingId <> _old.CDATrainingId Then
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

         Dim _arsMemberRequestCDATrainingListNew As New ArsMemberRequestCDATrainingList      ' for adding new Template Details

         If _addCDATrainingCount > 0 Then
            Dim _arsMemberRequestCDATraining As ArsMemberRequestCDATraining

            For Each _new As ArsMemberRequestCDATraining In _arsMemberRequestCDATrainingList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestCDATraining = New ArsMemberRequestCDATraining
                  _arsMemberRequestCDATrainingListNew.Add(_arsMemberRequestCDATraining)
                  DataLib.ScatterValues(_new, _arsMemberRequestCDATraining)
                  _arsMemberRequestCDATraining.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestComplianceTraining"

         Dim _removeComplianceTrainingCount As Integer
         Dim _addComplianceTrainingCount As Integer
         Dim _editComplianceTrainingCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestComplianceTraining In _arsMemberRequestComplianceTrainingListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestComplianceTraining In _arsMemberRequestComplianceTrainingList
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

         For Each _new As ArsMemberRequestComplianceTraining In _arsMemberRequestComplianceTrainingList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestComplianceTraining In _arsMemberRequestComplianceTrainingListOld
               If _new.ComplianceTrainingDetailId = _old.ComplianceTrainingDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .ComplianceTrainingId <> _old.ComplianceTrainingId Then
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

         Dim _arsMemberRequestComplianceTrainingListNew As New ArsMemberRequestComplianceTrainingList      ' for adding new Template Details

         If _addComplianceTrainingCount > 0 Then
            Dim _arsMemberRequestComplianceTraining As ArsMemberRequestComplianceTraining

            For Each _new As ArsMemberRequestComplianceTraining In _arsMemberRequestComplianceTrainingList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestComplianceTraining = New ArsMemberRequestComplianceTraining
                  _arsMemberRequestComplianceTrainingListNew.Add(_arsMemberRequestComplianceTraining)
                  DataLib.ScatterValues(_new, _arsMemberRequestComplianceTraining)
                  _arsMemberRequestComplianceTraining.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestMedicalResult"

         Dim _removeMedicalResultCount As Integer
         Dim _addMedicalResultCount As Integer
         Dim _editMedicalResultCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestMedicalResult In _arsMemberRequestMedicalResultListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestMedicalResult In _arsMemberRequestMedicalResultList
               If _new.MedicalResultDetailId = _old.MedicalResultDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeMedicalResultCount = _removeMedicalResultCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestMedicalResult In _arsMemberRequestMedicalResultList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestMedicalResult In _arsMemberRequestMedicalResultListOld
               If _new.MedicalResultDetailId = _old.MedicalResultDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .MedicalResultTypeId <> _old.MedicalResultTypeId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addMedicalResultCount = _addMedicalResultCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editMedicalResultCount = _editMedicalResultCount + 1
            End If

         Next

         Dim _arsMemberRequestMedicalResultListNew As New ArsMemberRequestMedicalResultList      ' for adding new Template Details

         If _addMedicalResultCount > 0 Then
            Dim _arsMemberRequestMedicalResult As ArsMemberRequestMedicalResult

            For Each _new As ArsMemberRequestMedicalResult In _arsMemberRequestMedicalResultList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestMedicalResult = New ArsMemberRequestMedicalResult
                  _arsMemberRequestMedicalResultListNew.Add(_arsMemberRequestMedicalResult)
                  DataLib.ScatterValues(_new, _arsMemberRequestMedicalResult)
                  _arsMemberRequestMedicalResult.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestBilling"

         Dim _removeBillingCount As Integer
         Dim _addBillingCount As Integer
         Dim _editBillingCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestBilling In _arsMemberRequestBillingListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestBilling In _arsMemberRequestBillingList
               If _new.BillingDetailId = _old.BillingDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeBillingCount = _removeBillingCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestBilling In _arsMemberRequestBillingList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestBilling In _arsMemberRequestBillingListOld
               If _new.BillingDetailId = _old.BillingDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .BillingDetailId <> _old.BillingDetailId OrElse .DailyRate <> _old.DailyRate OrElse .PayTrxCode <> _old.PayTrxCode Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addBillingCount = _addBillingCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editBillingCount = _editBillingCount + 1
            End If

         Next

         Dim _arsMemberRequestBillingListNew As New ArsMemberRequestBillingList      ' for adding new Template Details

         If _addBillingCount > 0 Then
            Dim _arsMemberRequestBilling As ArsMemberRequestBilling

            For Each _new As ArsMemberRequestBilling In _arsMemberRequestBillingList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestBilling = New ArsMemberRequestBilling
                  _arsMemberRequestBillingListNew.Add(_arsMemberRequestBilling)
                  DataLib.ScatterValues(_new, _arsMemberRequestBilling)
                  _arsMemberRequestBilling.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region

#Region "ArsMemberRequestPayOut"

         Dim _removePayOutCount As Integer
         Dim _addPayOutCount As Integer
         Dim _editPayOutCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestPayOut In _arsMemberRequestPayOutListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestPayOut In _arsMemberRequestPayOutList
               If _new.PayOutDetailId = _old.PayOutDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removePayOutCount = _removePayOutCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestPayOut In _arsMemberRequestPayOutList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestPayOut In _arsMemberRequestPayOutListOld
               If _new.PayOutDetailId = _old.PayOutDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .PayOutDetailId <> _old.PayOutDetailId OrElse .DailyRate <> _old.DailyRate OrElse .PayTrxCode <> _old.PayTrxCode Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addPayOutCount = _addPayOutCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editPayOutCount = _editPayOutCount + 1
            End If

         Next

         Dim _arsMemberRequestPayOutListNew As New ArsMemberRequestPayOutList      ' for adding new Template Details

         If _addPayOutCount > 0 Then
            Dim _arsMemberRequestPayOut As ArsMemberRequestPayOut

            For Each _new As ArsMemberRequestPayOut In _arsMemberRequestPayOutList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestPayOut = New ArsMemberRequestPayOut
                  _arsMemberRequestPayOutListNew.Add(_arsMemberRequestPayOut)
                  DataLib.ScatterValues(_new, _arsMemberRequestPayOut)
                  _arsMemberRequestPayOut.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region


#Region "ArsMemberRequestScreening"

         Dim _removeScreeningCount As Integer
         Dim _addScreeningCount As Integer
         Dim _editScreeningCount As Integer
         ' Mark details for deletion if not found in new list

         For Each _old As ArsMemberRequestScreening In _arsMemberRequestScreeningListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As ArsMemberRequestScreening In _arsMemberRequestScreeningList
               If _new.ScreeningDetailId = _old.ScreeningDetailId Then
                  _old.LogActionId = 0  ' retain
               End If
            Next

            If _old.LogActionId = LogActionId.Delete Then
               _removeScreeningCount = _removeScreeningCount + 1
            End If
         Next

         ' Mark details for addition if not found in old list;
         ' Mark details for modification if found in old list but with at least 1 property mismatch

         For Each _new As ArsMemberRequestScreening In _arsMemberRequestScreeningList
            _new.LogActionId = LogActionId.Add
            For Each _old As ArsMemberRequestScreening In _arsMemberRequestScreeningListOld
               If _new.ScreeningDetailId = _old.ScreeningDetailId Then
                  _new.LogActionId = 0   ' don't add

                  With _new
                     'If .ed <> _old.AccountId Then
                     '    .LogActionId = LogActionId.Edit
                     'End If

                     If .ScreeningDetailId <> _old.ScreeningDetailId Then
                        .LogActionId = LogActionId.Edit
                     End If
                  End With



                  Exit For
               End If
            Next

            If _new.LogActionId = LogActionId.Add Then

               _addScreeningCount = _addScreeningCount + 1
            End If

            If _new.LogActionId = LogActionId.Edit Then
               _editScreeningCount = _editScreeningCount + 1
            End If

         Next

         Dim _arsMemberRequestScreeningListNew As New ArsMemberRequestScreeningList      ' for adding new Template Details

         If _addScreeningCount > 0 Then
            Dim _arsMemberRequestScreening As ArsMemberRequestScreening

            For Each _new As ArsMemberRequestScreening In _arsMemberRequestScreeningList
               If _new.LogActionId = LogActionId.Add Then
                  _arsMemberRequestScreening = New ArsMemberRequestScreening
                  _arsMemberRequestScreeningListNew.Add(_arsMemberRequestScreening)
                  DataLib.ScatterValues(_new, _arsMemberRequestScreening)
                  _arsMemberRequestScreening.MemberRequestId = _arsMemberRequest.MemberRequestId
               End If
            Next

         End If

#End Region


         Dim _isArsMemberRequestChanged As Boolean = Me.HasArsMemberRequestChanges(_arsMemberRequestOld, _arsMemberRequest)

         If Not _isArsMemberRequestChanged AndAlso _addEducationCount = 0 AndAlso _removeEducationCount = 0 AndAlso _editEducationCount = 0 AndAlso _addReligionCount = 0 AndAlso _removeReligionCount = 0 AndAlso _editReligionCount = 0 AndAlso _addCivilStatusCount = 0 AndAlso _removeCivilStatusCount = 0 AndAlso _editCivilStatusCount = 0 AndAlso _addSexCount = 0 AndAlso _removeSexCount = 0 AndAlso _editSexCount = 0 AndAlso _addEmploymentTypeCount = 0 AndAlso _removeEmploymentTypeCount = 0 AndAlso _editEmploymentTypeCount = 0 AndAlso _addTypeQualificationCount = 0 AndAlso _removeTypeQualificationCount = 0 AndAlso _editTypeQualificationCount = 0 AndAlso _addNCIICount = 0 AndAlso _removeNCIICount = 0 AndAlso _editNCIICount = 0 AndAlso _addLicenseProfessionCount = 0 AndAlso _removeLicenseProfessionCount = 0 AndAlso _editLicenseProfessionCount = 0 AndAlso _addCDATrainingCount = 0 AndAlso _removeCDATrainingCount = 0 AndAlso _editCDATrainingCount = 0 AndAlso _addComplianceTrainingCount = 0 AndAlso _removeComplianceTrainingCount = 0 AndAlso _editComplianceTrainingCount = 0 AndAlso _addMedicalResultCount = 0 AndAlso _removeMedicalResultCount = 0 AndAlso _editMedicalResultCount = 0 AndAlso _addBillingCount = 0 AndAlso _removeBillingCount = 0 AndAlso _editBillingCount = 0 AndAlso _addPayOutCount = 0 AndAlso _removePayOutCount = 0 AndAlso _editPayOutCount = 0 AndAlso _addScreeningCount = 0 AndAlso _removeScreeningCount = 0 AndAlso _editScreeningCount = 0 Then

            '
            ' No changes; just return current transaction
            ' 
            Return Me.GetMemberRequestPooling(memberRequestId)
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

            If _isArsMemberRequestChanged Then
               Me.UpdateArsMemberRequest(_arsMemberRequest)
            End If

            If _removeEducationCount > 0 Then
               Me.DeleteArsMemberRequestEducations(_arsMemberRequestEducationListOld)
            End If

            If _addEducationCount > 0 Then
               Me.InsertArsMemberRequestEducations(_arsMemberRequestEducationListNew)
            End If

            If _editEducationCount > 0 Then
               Me.UpdateArsMemberRequestEducations(_arsMemberRequestEducationList)
            End If

            If _removeReligionCount > 0 Then
               Me.DeleteArsMemberRequestReligions(_arsMemberRequestReligionListOld)
            End If

            If _addReligionCount > 0 Then
               Me.InsertArsMemberRequestReligions(_arsMemberRequestReligionListNew)
            End If

            If _editReligionCount > 0 Then
               Me.UpdateArsMemberRequestReligions(_arsMemberRequestReligionList)
            End If

            If _removeCivilStatusCount > 0 Then
               Me.DeleteArsMemberRequestCivilStatus(_arsMemberRequestCivilStatusListOld)
            End If

            If _addCivilStatusCount > 0 Then
               Me.InsertArsMemberRequestCivilStatus(_arsMemberRequestCivilStatusListNew)
            End If

            If _editCivilStatusCount > 0 Then
               Me.UpdateArsMemberRequestCivilStatus(_arsMemberRequestCivilStatusList)
            End If

            If _removeSexCount > 0 Then
               Me.DeleteArsMemberRequestSex(_arsMemberRequestSexListOld)
            End If

            If _addSexCount > 0 Then
               Me.InsertArsMemberRequestSex(_arsMemberRequestSexListNew)
            End If

            If _editSexCount > 0 Then
               Me.UpdateArsMemberRequestSex(_arsMemberRequestSexList)
            End If

            If _removeEmploymentTypeCount > 0 Then
               Me.DeleteArsMemberRequestEmploymentTypes(_arsMemberRequestEmploymentTypeListOld)
            End If

            If _addEmploymentTypeCount > 0 Then
               Me.InsertArsMemberRequestEmploymentTypes(_arsMemberRequestEmploymentTypeListNew)
            End If

            If _editEmploymentTypeCount > 0 Then
               Me.UpdateArsMemberRequestEmploymentTypes(_arsMemberRequestEmploymentTypeList)
            End If

            If _removeTypeQualificationCount > 0 Then
               Me.DeleteArsMemberRequestTypeQualifications(_arsMemberRequestTypeQualificationListOld)
            End If

            If _addTypeQualificationCount > 0 Then
               Me.InsertArsMemberRequestTypeQualifications(_arsMemberRequestTypeQualificationListNew)
            End If

            If _editTypeQualificationCount > 0 Then
               Me.UpdateArsMemberRequestTypeQualifications(_arsMemberRequestTypeQualificationList)
            End If

            If _removeNCIICount > 0 Then
               Me.DeleteArsMemberRequestNCIIs(_arsMemberRequestNCIIListOld)
            End If

            If _addNCIICount > 0 Then
               Me.InsertArsMemberRequestNCIIs(_arsMemberRequestNCIIListNew)
            End If

            If _editNCIICount > 0 Then
               Me.UpdateArsMemberRequestNCIIs(_arsMemberRequestNCIIList)
            End If

            If _removeLicenseProfessionCount > 0 Then
               Me.DeleteArsMemberRequestLicenseProfessions(_arsMemberRequestLicenseProfessionListOld)
            End If

            If _addLicenseProfessionCount > 0 Then
               Me.InsertArsMemberRequestLicenseProfessions(_arsMemberRequestLicenseProfessionListNew)
            End If

            If _editLicenseProfessionCount > 0 Then
               Me.UpdateArsMemberRequestLicenseProfessions(_arsMemberRequestLicenseProfessionList)
            End If

            If _removeCDATrainingCount > 0 Then
               Me.DeleteArsMemberRequestCDATrainings(_arsMemberRequestCDATrainingListOld)
            End If

            If _addCDATrainingCount > 0 Then
               Me.InsertArsMemberRequestCDATrainings(_arsMemberRequestCDATrainingListNew)
            End If

            If _editCDATrainingCount > 0 Then
               Me.UpdateArsMemberRequestCDATrainings(_arsMemberRequestCDATrainingList)
            End If

            If _removeComplianceTrainingCount > 0 Then
               Me.DeleteArsMemberRequestComplianceTrainings(_arsMemberRequestComplianceTrainingListOld)
            End If

            If _addComplianceTrainingCount > 0 Then
               Me.InsertArsMemberRequestComplianceTrainings(_arsMemberRequestComplianceTrainingListNew)
            End If

            If _editComplianceTrainingCount > 0 Then
               Me.UpdateArsMemberRequestComplianceTrainings(_arsMemberRequestComplianceTrainingList)
            End If

            If _removeMedicalResultCount > 0 Then
               Me.DeleteArsMemberRequestMedicalResults(_arsMemberRequestMedicalResultListOld)
            End If

            If _addMedicalResultCount > 0 Then
               Me.InsertArsMemberRequestMedicalResults(_arsMemberRequestMedicalResultListNew)
            End If

            If _editMedicalResultCount > 0 Then
               Me.UpdateArsMemberRequestMedicalResults(_arsMemberRequestMedicalResultList)
            End If

            If _removeBillingCount > 0 Then
               Me.DeleteArsMemberRequestBillings(_arsMemberRequestBillingListOld)
            End If

            If _addBillingCount > 0 Then
               Me.InsertArsMemberRequestBillings(_arsMemberRequestBillingListNew)
            End If

            If _editBillingCount > 0 Then
               Me.UpdateArsMemberRequestBillings(_arsMemberRequestBillingList)
            End If

            If _removePayOutCount > 0 Then
               Me.DeleteArsMemberRequestPayOuts(_arsMemberRequestPayOutListOld)
            End If

            If _addPayOutCount > 0 Then
               Me.InsertArsMemberRequestPayOuts(_arsMemberRequestPayOutListNew)
            End If

            If _editPayOutCount > 0 Then
               Me.UpdateArsMemberRequestPayOuts(_arsMemberRequestPayOutList)
            End If

            If _removeScreeningCount > 0 Then
               Me.DeleteArsMemberRequestScreenings(_arsMemberRequestScreeningListOld)
            End If

            If _addScreeningCount > 0 Then
               Me.InsertArsMemberRequestScreenings(_arsMemberRequestScreeningListNew)
            End If

            If _editScreeningCount > 0 Then
               Me.UpdateArsMemberRequestScreenings(_arsMemberRequestScreeningList)
            End If


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

   <SymAuthorization("RemoveArsMemberRequest")>
   <Route("member-request-pooling/{memberRequestId}/{lockId}")>
   <HttpDelete>
   Public Function RemoveArsMemberRequest(memberRequestId As Integer, lockId As String) As IHttpActionResult

      If memberRequestId <= 0 Then
         Throw New ArgumentException("Member Request ID is required.")
      End If

      Try
         Dim _transaction As SqlTransaction = Nothing
         Dim _successFlag As Boolean

         Try
            If Not DataCore.Connect(_databaseId) Then
               ' TODO: Cannot connect
            End If

            _transaction = DataCore.Connection.BeginTransaction()
            DataCore.Command.Transaction = _transaction
            DataCore.Command.Connection = DataCore.Connection

            Me.DeleteArsMemberRequest(memberRequestId, lockId)

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
   Private Sub LoadArsMemberRequest(memberRequest As ArsMemberRequestBody, arsMemberRequest As ArsMemberRequest)

      DataLib.ScatterValues(memberRequest, arsMemberRequest)

   End Sub
   Private Sub LoadArsMemberRequest(row As DataRow, memberRequest As ArsMemberRequest)

      With memberRequest
         .MemberRequestId = row.ToInt32("MemberRequestId")
         .MemberRequestName = row.ToString("MemberRequestName")
         .OrgId = row.ToInt32("OrgId")
         .PlatformId = row.ToInt32("PlatformId")
         .ClientContractId = row.ToInt32("ClientContractId")
         .ClientPosition = row.ToString("ClientPosition")
         .MemberRequestPositionId = row.ToInt32("MemberRequestPositionId")
         .JobCode = row.ToString("JobCode")
         .JobDescription = row.ToString("JobDescription")
         .MemberTypeId = row.ToInt32("MemberTypeId")
         .PayoutTypeId = row.ToInt32("PayoutTypeId")
         .VacancyCount = row.ToInt32("VacancyCount")
         .DeploymentDate = row.ToDate("DeploymentDate")
         .RegionId = row.ToString("RegionId")
         .ProvinceId = row.ToString("ProvinceId")
         .MunicipalityId = row.ToString("MunicipalityId")
         .PositionKeyword = row.ToString("PositionKeyword")
         .SkillNameKeyword = row.ToString("SkillNameKeyword")
         .MemberRequestStatusId = row.ToInt32("MemberRequestStatusId")
         .ClientPayGroupId = row.ToInt32("ClientPayGroupId")
         .WorkingDays = row.ToInt32("WorkingDays")
      End With

   End Sub
   Private Sub LoadArsMemberRequestEducationList(rows As DataRowCollection, list As ArsMemberRequestEducationList)

      Dim _detail As ArsMemberRequestEducation
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestEducation

         With _detail
            .MemberRequestEducationDetailId = _row.ToInt32("MemberRequestEducationDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .EducationLevelId = _row.ToInt32("EducationLevelId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestReligionList(rows As DataRowCollection, list As ArsMemberRequestReligionList)

      Dim _detail As ArsMemberRequestReligion
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestReligion

         With _detail
            .MemberRequestReligionDetailId = _row.ToInt32("MemberRequestReligionDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .ReligionId = _row.ToInt32("ReligionId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestCivilStatusList(rows As DataRowCollection, list As ArsMemberRequestCivilStatusList)

      Dim _detail As ArsMemberRequestCivilStatus
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestCivilStatus

         With _detail
            .MemberRequestCivilStatusDetailId = _row.ToInt32("MemberRequestCivilStatusDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .CivilStatusId = _row.ToInt32("CivilStatusId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestSexList(rows As DataRowCollection, list As ArsMemberRequestSexList)

      Dim _detail As ArsMemberRequestSex
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestSex

         With _detail
            .MemberRequestSexDetailId = _row.ToInt32("MemberRequestSexDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .SexId = _row.ToString("SexId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestEmploymentTypeList(rows As DataRowCollection, list As ArsMemberRequestEmploymentTypeList)

      Dim _detail As ArsMemberRequestEmploymentType
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestEmploymentType

         With _detail
            .MemberRequestEmploymentTypeDetailId = _row.ToInt32("MemberRequestEmploymentTypeDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .EmploymentTypeId = _row.ToInt32("EmploymentTypeId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestTypeQualificationList(rows As DataRowCollection, list As ArsMemberRequestTypeQualificationList)

      Dim _detail As ArsMemberRequestTypeQualification
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestTypeQualification

         With _detail
            .MemberRequestTypeQualificationDetailId = _row.ToInt32("MemberRequestTypeQualificationDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .TypeQualificationDetailId = _row.ToInt32("TypeQualificationDetailId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestNCIIList(rows As DataRowCollection, list As ArsMemberRequestNCIIList)

      Dim _detail As ArsMemberRequestNCII
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestNCII

         With _detail
            .NCIIDetailId = _row.ToInt32("NCIIDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .NCIIQualificationTitleId = _row.ToInt32("NCIIQualificationTitleId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestLicenseProfessionList(rows As DataRowCollection, list As ArsMemberRequestLicenseProfessionList)

      Dim _detail As ArsMemberRequestLicenseProfession
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestLicenseProfession

         With _detail
            .LicenseProfessionDetailId = _row.ToInt32("LicenseProfessionDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .LicenseProfessionId = _row.ToInt32("LicenseProfessionId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestCDATrainingList(rows As DataRowCollection, list As ArsMemberRequestCDATrainingList)

      Dim _detail As ArsMemberRequestCDATraining
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestCDATraining

         With _detail
            .CDATrainingDetailId = _row.ToInt32("CDATrainingDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .CDATrainingId = _row.ToInt32("CDATrainingId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestComplianceTrainingList(rows As DataRowCollection, list As ArsMemberRequestComplianceTrainingList)

      Dim _detail As ArsMemberRequestComplianceTraining
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestComplianceTraining

         With _detail
            .ComplianceTrainingDetailId = _row.ToInt32("ComplianceTrainingDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .ComplianceTrainingId = _row.ToInt32("ComplianceTrainingId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestMedicalResultList(rows As DataRowCollection, list As ArsMemberRequestMedicalResultList)

      Dim _detail As ArsMemberRequestMedicalResult
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestMedicalResult

         With _detail
            .MedicalResultDetailId = _row.ToInt32("MedicalResultDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .MedicalResultTypeId = _row.ToInt32("MedicalResultTypeId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestBillingList(rows As DataRowCollection, list As ArsMemberRequestBillingList)

      Dim _detail As ArsMemberRequestBilling
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestBilling

         With _detail
            .BillingDetailId = _row.ToInt32("BillingDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .PayTrxCode = _row.ToString("PayTrxCode")
            .DailyRate = _row.ToDecimal("DailyRate")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub LoadArsMemberRequestPayOutList(rows As DataRowCollection, list As ArsMemberRequestPayOutList)

      Dim _detail As ArsMemberRequestPayOut
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestPayOut

         With _detail
            .PayOutDetailId = _row.ToInt32("PayOutDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .PayTrxCode = _row.ToString("PayTrxCode")
            .DailyRate = _row.ToDecimal("DailyRate")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub LoadArsMemberRequestScreeningList(rows As DataRowCollection, list As ArsMemberRequestScreeningList)

      Dim _detail As ArsMemberRequestScreening
      For Each _row As DataRow In rows
         _detail = New ArsMemberRequestScreening

         With _detail
            .ScreeningDetailId = _row.ToInt32("ScreeningDetailId")
            .MemberRequestId = _row.ToInt32("MemberRequestId")
            .ApplicantScreeningId = _row.ToInt32("ApplicantScreeningId")
         End With

         list.Add(_detail)

      Next

   End Sub
   Private Sub InsertArsMemberRequest(memberRequest As ArsMemberRequest)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequest", memberRequest, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberRequest(memberRequest)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertArsMemberRequestEducations(list As ArsMemberRequestEducationList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestEducationDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestEducation", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestEducation In list
            Me.AddInsertUpdateParamsArsMemberRequestEducation(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestReligions(list As ArsMemberRequestReligionList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestReligionDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestReligion", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestReligion In list
            Me.AddInsertUpdateParamsArsMemberRequestReligion(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestCivilStatus(list As ArsMemberRequestCivilStatusList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestCivilStatusDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestCivilStatus", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestCivilStatus In list
            Me.AddInsertUpdateParamsArsMemberRequestCivilStatus(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestSex(list As ArsMemberRequestSexList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestSexDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestSex", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestSex In list
            Me.AddInsertUpdateParamsArsMemberRequestSex(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub InsertArsMemberRequestEmploymentTypes(list As ArsMemberRequestEmploymentTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestEmploymentTypeDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestEmploymentType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestEmploymentType In list
            Me.AddInsertUpdateParamsArsMemberRequestEmploymentType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestTypeQualifications(list As ArsMemberRequestTypeQualificationList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MemberRequestTypeQualificationDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestTypeQualification", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestTypeQualification In list
            Me.AddInsertUpdateParamsArsMemberRequestTypeQualification(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestNCIIs(list As ArsMemberRequestNCIIList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("NCIIDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestNCII", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestNCII In list
            Me.AddInsertUpdateParamsArsMemberRequestNCII(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestLicenseProfessions(list As ArsMemberRequestLicenseProfessionList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("LicenseProfessionDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestLicenseProfession", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestLicenseProfession In list
            Me.AddInsertUpdateParamsArsMemberRequestLicenseProfession(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestCDATrainings(list As ArsMemberRequestCDATrainingList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("CDATrainingDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestCDATraining", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestCDATraining In list
            Me.AddInsertUpdateParamsArsMemberRequestCDATraining(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestComplianceTrainings(list As ArsMemberRequestComplianceTrainingList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ComplianceTrainingDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestComplianceTraining", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestComplianceTraining In list
            Me.AddInsertUpdateParamsArsMemberRequestComplianceTraining(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestMedicalResults(list As ArsMemberRequestMedicalResultList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("MedicalResultDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestMedicalResult", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestMedicalResult In list
            Me.AddInsertUpdateParamsArsMemberRequestMedicalResult(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestBillings(list As ArsMemberRequestBillingList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("BillingDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestBilling", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestBilling In list
            Me.AddInsertUpdateParamsArsMemberRequestBilling(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestPayOuts(list As ArsMemberRequestPayOutList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("PayOutDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestPayOut", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestPayOut In list
            Me.AddInsertUpdateParamsArsMemberRequestPayOut(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestScreenings(list As ArsMemberRequestScreeningList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ScreeningDetailId")     ' auto-assigned by DB
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestScreening", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestScreening In list
            Me.AddInsertUpdateParamsArsMemberRequestScreening(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub

   Private Sub UpdateArsMemberRequest(memberRequest As ArsMemberRequest)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequest", memberRequest, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberRequest(memberRequest)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(memberRequest.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateArsMemberRequestEducations(list As ArsMemberRequestEducationList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestEducationDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestEducation", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestEducation In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestEducation(_detail)
               .Parameters.AddWithValue("@MemberRequestEducationDetailId", _detail.MemberRequestEducationDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestReligions(list As ArsMemberRequestReligionList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestReligionDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestReligion", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestReligion In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestReligion(_detail)
               .Parameters.AddWithValue("@MemberRequestReligionDetailId", _detail.MemberRequestReligionDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestCivilStatus(list As ArsMemberRequestCivilStatusList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestCivilStatusDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestCivilStatus", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestCivilStatus In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestCivilStatus(_detail)
               .Parameters.AddWithValue("@MemberRequestCivilStatusDetailId", _detail.MemberRequestCivilStatusDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestSex(list As ArsMemberRequestSexList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestSexDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestSex", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestSex In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestSex(_detail)
               .Parameters.AddWithValue("@MemberRequestSexDetailId", _detail.MemberRequestSexDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestEmploymentTypes(list As ArsMemberRequestEmploymentTypeList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestEmploymentTypeDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestEmploymentType", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestEmploymentType In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestEmploymentType(_detail)
               .Parameters.AddWithValue("@MemberRequestEmploymentTypeDetailId", _detail.MemberRequestEmploymentTypeDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestTypeQualifications(list As ArsMemberRequestTypeQualificationList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestTypeQualificationDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestTypeQualification", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestTypeQualification In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestTypeQualification(_detail)
               .Parameters.AddWithValue("@MemberRequestTypeQualificationDetailId", _detail.MemberRequestTypeQualificationDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestNCIIs(list As ArsMemberRequestNCIIList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("NCIIDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestNCII", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestNCII In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestNCII(_detail)
               .Parameters.AddWithValue("@NCIIDetailId", _detail.NCIIDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestLicenseProfessions(list As ArsMemberRequestLicenseProfessionList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("LicenseProfessionDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestLicenseProfession", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestLicenseProfession In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestLicenseProfession(_detail)
               .Parameters.AddWithValue("@LicenseProfessionDetailId", _detail.LicenseProfessionDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestCDATrainings(list As ArsMemberRequestCDATrainingList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("CDATrainingDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestCDATraining", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestCDATraining In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestCDATraining(_detail)
               .Parameters.AddWithValue("@CDATrainingDetailId", _detail.CDATrainingDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestComplianceTrainings(list As ArsMemberRequestComplianceTrainingList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ComplianceTrainingDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestComplianceTraining", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestComplianceTraining In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestComplianceTraining(_detail)
               .Parameters.AddWithValue("@ComplianceTrainingDetailId", _detail.ComplianceTrainingDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestMedicalResults(list As ArsMemberRequestMedicalResultList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("MedicalResultDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestMedicalResult", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestMedicalResult In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestMedicalResult(_detail)
               .Parameters.AddWithValue("@MedicalResultDetailId", _detail.MedicalResultDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestBillings(list As ArsMemberRequestBillingList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("BillingDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestBilling", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestBilling In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestBilling(_detail)
               .Parameters.AddWithValue("@BillingDetailId", _detail.BillingDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestPayOuts(list As ArsMemberRequestPayOutList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("PayOutDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestPayOut", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestPayOut In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestPayOut(_detail)
               .Parameters.AddWithValue("@PayOutDetailId", _detail.PayOutDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub UpdateArsMemberRequestScreenings(list As ArsMemberRequestScreeningList)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _keyFields
         .Add("ScreeningDetailId")
      End With

      With _excludedFields
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestScreening", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As ArsMemberRequestScreening In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsArsMemberRequestScreening(_detail)
               .Parameters.AddWithValue("@ScreeningDetailId", _detail.ScreeningDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequest(memberRequest As ArsMemberRequest)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", memberRequest.MemberRequestId)
         .AddWithValue("@MemberRequestName", memberRequest.MemberRequestName)
         .AddWithValue("@OrgId", memberRequest.OrgId.ToNullable)
         .AddWithValue("@PlatformId", memberRequest.PlatformId.ToNullable)
         .AddWithValue("@ClientContractId", memberRequest.ClientContractId)
         .AddWithValue("@ClientPosition", memberRequest.ClientPosition)
         .AddWithValue("@MemberRequestPositionId", memberRequest.MemberRequestPositionId)
         .AddWithValue("@JobCode", memberRequest.JobCode)
         .AddWithValue("@JobDescription", memberRequest.JobDescription)
         .AddWithValue("@MemberTypeId", memberRequest.MemberTypeId)
         .AddWithValue("@PayoutTypeId", memberRequest.PayoutTypeId)
         .AddWithValue("@VacancyCount", memberRequest.VacancyCount)
         .AddWithValue("@DeploymentDate", memberRequest.DeploymentDate)
         .AddWithValue("@RegionId", memberRequest.RegionId)
         .AddWithValue("@ProvinceId", memberRequest.ProvinceId.ToNullable)
         .AddWithValue("@MunicipalityId", memberRequest.MunicipalityId.ToNullable)
         .AddWithValue("@PositionKeyword", memberRequest.PositionKeyword.ToNullable)
         .AddWithValue("@SkillNameKeyword", memberRequest.SkillNameKeyword.ToNullable)
         .AddWithValue("@MemberRequestStatusId", memberRequest.MemberRequestStatusId)
         .AddWithValue("@ClientPayGroupId", memberRequest.ClientPayGroupId)
         .AddWithValue("@WorkingDays", memberRequest.WorkingDays)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestEducation(dtl As ArsMemberRequestEducation)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@EducationLevelId", dtl.EducationLevelId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestReligion(dtl As ArsMemberRequestReligion)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@ReligionId", dtl.ReligionId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestCivilStatus(dtl As ArsMemberRequestCivilStatus)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@CivilStatusId", dtl.CivilStatusId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestSex(dtl As ArsMemberRequestSex)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@SexId", dtl.SexId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestEmploymentType(dtl As ArsMemberRequestEmploymentType)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@EmploymentTypeId", dtl.EmploymentTypeId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequestTypeQualification(dtl As ArsMemberRequestTypeQualification)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@TypeQualificationDetailId", dtl.TypeQualificationDetailId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestNCII(dtl As ArsMemberRequestNCII)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@NCIIQualificationTitleId", dtl.NCIIQualificationTitleId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequestLicenseProfession(dtl As ArsMemberRequestLicenseProfession)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@LicenseProfessionId", dtl.LicenseProfessionId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestCDATraining(dtl As ArsMemberRequestCDATraining)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@CDATrainingId", dtl.CDATrainingId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestComplianceTraining(dtl As ArsMemberRequestComplianceTraining)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@ComplianceTrainingId", dtl.ComplianceTrainingId)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequestMedicalResult(dtl As ArsMemberRequestMedicalResult)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@MedicalResultTypeId", dtl.MedicalResultTypeId)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestBilling(dtl As ArsMemberRequestBilling)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
         .AddWithValue("@DailyRate", dtl.DailyRate)
      End With

   End Sub
   Private Sub AddInsertUpdateParamsArsMemberRequestPayOut(dtl As ArsMemberRequestPayOut)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@PayTrxCode", dtl.PayTrxCode)
         .AddWithValue("@DailyRate", dtl.DailyRate)
      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequestScreening(dtl As ArsMemberRequestScreening)

      With DataCore.Command.Parameters
         .Clear()
         .AddWithValue("@MemberRequestId", dtl.MemberRequestId)
         .AddWithValue("@ApplicantScreeningId", dtl.ApplicantScreeningId)
      End With

   End Sub

   Private Sub DeleteArsMemberRequest(memberRequestId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("MemberRequestId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberRequest", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@MemberRequestId", memberRequestId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub DeleteArsMemberRequestEducations(list As ArsMemberRequestEducationList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestEducation WHERE MemberRequestEducationDetailId=@MemberRequestEducationDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestEducation In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestEducationDetailId", _old.MemberRequestEducationDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestReligions(list As ArsMemberRequestReligionList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestReligion WHERE MemberRequestReligionDetailId=@MemberRequestReligionDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestReligion In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestReligionDetailId", _old.MemberRequestReligionDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestNCIIs(list As ArsMemberRequestNCIIList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestNCII WHERE NCIIDetailId=@NCIIDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestNCII In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@NCIIDetailId", _old.NCIIDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestLicenseProfessions(list As ArsMemberRequestLicenseProfessionList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestLicenseProfession WHERE LicenseProfessionDetailId=@LicenseProfessionDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestLicenseProfession In list
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
   Private Sub DeleteArsMemberRequestCDATrainings(list As ArsMemberRequestCDATrainingList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestCDATraining WHERE CDATrainingDetailId=@CDATrainingDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestCDATraining In list
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
   Private Sub DeleteArsMemberRequestComplianceTrainings(list As ArsMemberRequestComplianceTrainingList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestComplianceTraining WHERE ComplianceTrainingDetailId=@ComplianceTrainingDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestComplianceTraining In list
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
   Private Sub DeleteArsMemberRequestCivilStatus(list As ArsMemberRequestCivilStatusList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestCivilStatus WHERE MemberRequestCivilStatusDetailId=@MemberRequestCivilStatusDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestCivilStatus In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestCivilStatusDetailId", _old.MemberRequestCivilStatusDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestSex(list As ArsMemberRequestSexList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestSex WHERE MemberRequestSexDetailId=@MemberRequestSexDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestSex In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestSexDetailId", _old.MemberRequestSexDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestEmploymentTypes(list As ArsMemberRequestEmploymentTypeList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestEmploymentType WHERE MemberRequestEmploymentTypeDetailId=@MemberRequestEmploymentTypeDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestEmploymentType In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestEmploymentTypeDetailId", _old.MemberRequestEmploymentTypeDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestTypeQualifications(list As ArsMemberRequestTypeQualificationList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestTypeQualification WHERE MemberRequestTypeQualificationDetailId=@MemberRequestTypeQualificationDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestTypeQualification In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MemberRequestTypeQualificationDetailId", _old.MemberRequestTypeQualificationDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestMedicalResults(list As ArsMemberRequestMedicalResultList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestMedicalResult WHERE MedicalResultDetailId=@MedicalResultDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestMedicalResult In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@MedicalResultDetailId", _old.MedicalResultDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestBillings(list As ArsMemberRequestBillingList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestBilling WHERE BillingDetailId=@BillingDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestBilling In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@BillingDetailId", _old.BillingDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestPayOuts(list As ArsMemberRequestPayOutList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestPayOut WHERE PayOutDetailId=@PayOutDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestPayOut In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@PayOutDetailId", _old.PayOutDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub
   Private Sub DeleteArsMemberRequestScreenings(list As ArsMemberRequestScreeningList)

      With DataCore.Command
         .CommandText = "DELETE dbo.ArsMemberRequestScreening WHERE ScreeningDetailId=@ScreeningDetailId"
         .CommandType = CommandType.Text

         For Each _old As ArsMemberRequestScreening In list
            If _old.LogActionId = LogActionId.Delete Then
               With .Parameters
                  .Clear()
                  .AddWithValue("@ScreeningDetailId", _old.ScreeningDetailId)
               End With

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Function HasArsMemberRequestChanges(oldRecord As ArsMemberRequest, newRecord As ArsMemberRequest) As Boolean

      With oldRecord
         If .MemberRequestName <> newRecord.MemberRequestName Then Return True
         If .OrgId <> newRecord.OrgId Then Return True
         If .PlatformId <> newRecord.PlatformId Then Return True
         If .ClientContractId <> newRecord.ClientContractId Then Return True
         If .ClientPosition <> newRecord.ClientPosition Then Return True
         If .MemberRequestPositionId <> newRecord.MemberRequestPositionId Then Return True
         If .JobCode <> newRecord.JobCode Then Return True
         If .JobDescription <> newRecord.JobDescription Then Return True
         If .MemberTypeId <> newRecord.MemberTypeId Then Return True
         If .PayoutTypeId <> newRecord.PayoutTypeId Then Return True
         'If .EmploymentTypeId <> newRecord.EmploymentTypeId Then Return True
         If .VacancyCount <> newRecord.VacancyCount Then Return True
         If .DeploymentDate <> newRecord.DeploymentDate Then Return True
         If .RegionId <> newRecord.RegionId Then Return True
         If .ProvinceId <> newRecord.ProvinceId Then Return True
         If .MunicipalityId <> newRecord.MunicipalityId Then Return True
         If .PositionKeyword <> newRecord.PositionKeyword Then Return True
         If .SkillNameKeyword <> newRecord.SkillNameKeyword Then Return True
         If .ClientPayGroupId <> newRecord.ClientPayGroupId Then Return True
         If .WorkingDays <> newRecord.WorkingDays Then Return True
      End With

      Return False

   End Function

End Class

Public Class ArsMemberRequestBodyA

   Inherits ArsMemberRequest
   Public Property Educations As ArsMemberRequestEducation()

   Public Property Religions As ArsMemberRequestReligion()
   Public Property CivilStatus As ArsMemberRequestCivilStatus()
   Public Property Sexes As ArsMemberRequestSex()
   Public Property EmploymentTypes As ArsMemberRequestEmploymentType()
   Public Property TypeQualifications As ArsMemberRequestTypeQualification()
   Public Property NCIIs As ArsMemberRequestNCII()
   Public Property Licenses As ArsMemberRequestLicenseProfession()
   Public Property CDAs As ArsMemberRequestCDATraining()
   Public Property Compliances As ArsMemberRequestComplianceTraining()
   Public Property MedicalResults As ArsMemberRequestMedicalResult()
   Public Property Billings As ArsMemberRequestBilling()
   Public Property PayOutS As ArsMemberRequestPayOut()
   Public Property Screenings As ArsMemberRequestScreening()

End Class

Public Class FilterParam
   Public Property filter As String
   Public Property skillNameKeyword As String
   Public Property positionKeyword As String
End Class
