<RoutePrefix("api")>
Public Class ARS0050_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"

   <SymAuthorization("GetMemberRequestApplicantDocType")>
   <Route("member-request-applicant-docs/{memberId}")>
   <HttpGet>
   Public Function GetMemberRequestApplicantDocType(memberId As Integer) As IHttpActionResult

      If memberId <= 0 Then
         Throw New ArgumentException("Member ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0050_Doc")
            With _direct
               .AddParameter("memberId", memberId)

                    'Using _dataTable As DataTable = _direct.ExecuteDataTable()
                    '   Return Me.Ok(_dataTable)
                    'End Using
                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "member"
                            .Tables(1).TableName = "docType"

                            '.Tables(21).TableName = "docs"
                        End With

                        Return Me.Ok(_dataSet)
                    End Using


                End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function


   <SymAuthorization("GetApplicantScreenings")>
   <Route("member-request-new-applicant-screenings/{applicantDetailId}/{memberRequestId}")>
   <HttpGet>
   Public Function GetApplicantScreenings(applicantDetailId As Integer, memberRequestId As Integer) As IHttpActionResult

      Dim _filter As String = ""
      Dim _dataSource As String = ""
      Dim _fields As String = ""
      Dim _sort As String = ""

      If applicantDetailId > 0 And memberRequestId > 0 Then
         _filter = "ApplicantDetailId=" + applicantDetailId.ToString

         _dataSource = "dbo.QArsMemberRequestApplicantScreening"
            _fields = "ApplicantScreeningId, ApplicantScreeningName, UploadRequiredFlag, SortSeq"
            _sort = "SortSeq"


      Else
         _filter = "MemberRequestId=" + memberRequestId.ToString

         _dataSource = "dbo.QArsMemberRequestScreening"
         _fields = "ApplicantScreeningId, ApplicantScreeningName, SortSeq"
         _sort = "SortSeq"


      End If

      'Dim _filter As String = "MemberTypeId=" + memberTypeId.ToString

      Try
         Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
            Return Me.Ok(_table)
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetArsMemberRequestApplicantLog")>
   <Route("member-request-new-applicants/{applicantDetailId}/log")>
   <HttpGet>
   Public Function GetArsMemberRequestApplicantLog(applicantDetailId As Integer) As IHttpActionResult

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

   <SymAuthorization("GetMemberRequestApplicant")>
   <Route("member-request-applicants/{applicantDetailId}")>
   <HttpGet>
   Public Function GetMemberRequestApplicant(applicantDetailId As Integer) As IHttpActionResult

      If applicantDetailId <= 0 Then
         Throw New ArgumentException("Applicant Detail ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0050_Detail")
            With _direct
               .AddParameter("applicantDetailId", applicantDetailId)

               Using _dataTable As DataTable = _direct.ExecuteDataTable()
                  Return Me.Ok(_dataTable)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

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

      Dim _filter As String = filterParam.filter + positionKeywordFilter + skillNameKeywordFilter

      'Dim _dataSource As String = "dbo.QArsMemberRequestPooling"
      'Dim _fields As String = "MemberId, ApplicantId, MemberName, BirthDate, Age, SexId, SexName, MobileNumber, RegionId, RegionName, ProvinceName, MunicipalityName"
      'Dim _sort As String = "MemberName"

      Try
         'Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
         '   Return Me.Ok(_table)
         'End Using
         'Dim _sql As String = "Select MemberId, ApplicantId, MemberName, BirthDate, Age, SexId, SexName, MobileNumber, RegionId, RegionName, ProvinceName, MunicipalityName,MemberStatusName,ApplicantStatusId from dbo.QArsMemberRequestPooling WHERE " + _filter + " Group by MemberId, ApplicantId, MemberName, BirthDate, Age, SexId, SexName, MobileNumber, RegionId, RegionName, ProvinceName, MunicipalityName, MemberStatusName,ApplicantStatusId Order by MemberName, MemberId"
         Dim _sql As String = "Select MemberId, ApplicantId, MemberName, BirthDate, Age, SexId, SexName, MobileNumber, RegionId, RegionName, ProvinceName, MunicipalityName,MemberStatusName from dbo.QArsMemberRequestPooling WHERE " + _filter + " Group by MemberId, ApplicantId, MemberName, BirthDate, Age, SexId, SexName, MobileNumber, RegionId, RegionName, ProvinceName, MunicipalityName, MemberStatusName Order by MemberName, MemberId"

         Using _direct As New SqlDirect(_sql, CommandType.Text)
            Using _dataTable As DataTable = _direct.ExecuteDataTable()
               Return Me.Ok(_dataTable)
            End Using
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
                     .Tables(20).TableName = "screeningStatus"     ' all defined Transaction Types
                            '.Tables(21).TableName = "province"
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
                     .Tables(18).TableName = "applicants"
                     .Tables(19).TableName = "newApplicants"
                     .Tables(20).TableName = "docType"
                            .Tables(21).TableName = "tradeTestDocType"
                            .Tables(22).TableName = "applicantProgress"
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

   <SymAuthorization("CreateMemberRequestApplicant")>
   <Route("member-request-new-applicants/{currentUserId}")>
   <HttpPost>
   Public Function CreateMemberRequestApplicant(currentUserId As Integer, <FromBody> applicant As ArsMemberRequestApplicantBody) As IHttpActionResult
      Try

         If applicant.ApplicantDetailId <> -1 Then
            Throw New ArgumentException("Applicant Detail ID is unrecognized.")
         End If
      Catch ex As Exception
      End Try

      Try
         '
         ' Assign new ID from sequencer
         '
         Dim _applicantDetailId As Integer = SysLib.GetNextSequence("ApplicantDetailId")

         applicant.ApplicantDetailId = _applicantDetailId
         '
         ' Load proposed values from payload
         '
         Dim _arsMemberRequestApplicant As New ArsMemberRequestApplicant
         Me.LoadArsMemberRequestApplicant(applicant, _arsMemberRequestApplicant)

         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList

         Dim _id As Integer
         Dim _arsMemberRequestApplicantLogDetailList As New SysLogDetailList

         Try


            With applicant

               AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.MemberId, String.Empty, .MemberId.ToString)

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

                'Try
                'File.WriteAllText("d:\_arsMemberRequestApplicant.txt", _arsMemberRequestApplicant.RequestDocTypeId.ToString)
                Me.InsertArsMemberRequestApplicant(_arsMemberRequestApplicant)

                If _arsMemberRequestApplicantLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ApplicantDetailId", applicant.ApplicantDetailId)
               End With

               _id = AppLib.CreateLogHeader("InsArsMemberRequestApplicantLog", _logKeyList, LogActionId.Add, currentUserId)
               AppLib.AssignLogHeaderId(_id, _arsMemberRequestApplicantLogDetailList)
               AppLib.CreateLogDetails(_arsMemberRequestApplicantLogDetailList, "ArsMemberRequestApplicantLogDetail")

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

         'Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestApplicant(_applicantDetailId), Results.OkNegotiatedContentResult(Of DataTable))
         ''Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestApplicant(_applicantDetailId), Results.OkNegotiatedContentResult(Of DataTable))

         'Return Me.Ok(_result.Content)
         'Try


         '   Return Me.Ok(applicant.MemberRequestId)
         'Catch ex As Exception

         'End Try

         Return Me.Ok(applicant.MemberRequestId)
         'Return Me.Ok(applicant.MemberRequestId)

         'Try


         '   Dim _resultA As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestPooling(applicant.MemberRequestId), Results.OkNegotiatedContentResult(Of DataTable))
         'Catch ex As Exception
         '   File.WriteAllText("e:\aaa.txt", ex.Message)
         'End Try
         ''Return Me.Ok(True)
         ''Return Me.Ok(recruiter.RecruiterId)
         'Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestPooling(applicant.MemberRequestId), Results.OkNegotiatedContentResult(Of DataTable))
         'Return Me.Ok(_result.Content)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

    '<SymAuthorization("ModifyMemberRequestApplicantDocType")>
    <Route("member-request-applicant-docs/{currentUserId}/{memberRequestId}")>
    <HttpPost>
    Public Function ModifyMemberRequestApplicantDocType(currentUserId As Integer, memberRequestId As Integer, <FromBody> action As HrsMemberDocTypeBody) As IHttpActionResult

        'If memberId <= 0 Then
        '   Throw New ArgumentException("Applicant Detail ID is required.")
        'End If

        Try


            '
            ' Load proposed values from payload
            '

            Dim _hrsMemberDocType As New HrsMemberDocTypeApplicant
            '.MemberStatusId = GetMemberStatus(member.EmploymentStatusId)


            Me.LoadHrsMemberDocType(action, _hrsMemberDocType)

            '
            ' Load old values from DB
            '
            Dim _hrsMemberDocTypeOld As New HrsMemberDocTypeApplicant



            '
            ' Apply and log changes, save to DB
            '
            Dim _logKeyList As New LogKeyList
            'Dim _id As Integer



            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection


                Me.InsertHrsMemberDocTypes(_hrsMemberDocType)

                'Catch ex As Exception
                '   File.WriteAllText("e:\XXX3.txt", ex.Message)

                'End Try

                _successFlag = True

            Catch _exception As Exception
                'File.WriteAllText("d:\yyy.txt", _exception.Message)
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

            'Dim _resultA As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestApplicant(applicantDetailId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(_resultA.Content)
            Return Me.Ok(memberRequestId)

            'Return Me.Ok(True)
        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("ModifyMemberRequestApplicant")>
    <Route("member-request-applicants/{applicantDetailId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyMemberRequestApplicant(applicantDetailId As Integer, currentUserId As Integer, <FromBody> action As ArsMemberRequestApplicantBody) As IHttpActionResult


        If applicantDetailId <= 0 Then
            Throw New ArgumentException("Applicant Detail ID is required.")
        End If

        Try


            '
            ' Load proposed values from payload
            '

            Dim _arsMemberRequestApplicant As New ArsMemberRequestApplicant

            'File.WriteAllText("d:\DoctypeId.txt", _arsMemberRequestApplicant.DocTypeId.ToString)

            With action

                If .ApplicantStatusId = EmploymentApplicantStatus.Hired Then
                    .EmploymentStatusId = EmploymentStatus.Active
                End If

                If .ApplicantStatusId = EmploymentApplicantStatus.Delisted Then
                    .EmploymentStatusId = EmploymentStatus.Delisted
                End If

            End With

            '.MemberStatusId = GetMemberStatus(member.EmploymentStatusId)

            Me.LoadArsMemberRequestApplicant(action, _arsMemberRequestApplicant)

            '
            ' Load old values from DB
            '
            Dim _arsMemberRequestApplicantOld As New ArsMemberRequestApplicant

            'Try
            Try

                'File.WriteAllText("d:\DoctypeId2.txt", _arsMemberRequestApplicant.DocTypeId.ToString)
                Dim _result As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestApplicant(applicantDetailId), Results.OkNegotiatedContentResult(Of DataTable))

                Using _dataTable As DataTable = _result.Content
                    If _dataTable.Rows.Count > 0 Then
                        Dim _row As DataRow = _dataTable.Rows(0)
                        Me.LoadArsMemberRequestApplicant(_row, _arsMemberRequestApplicantOld)


                    End If
                End Using

            Catch ex As Exception
            End Try
            'File.WriteAllText("d:\DoctypeId3.txt", _arsMemberRequestApplicant.DocTypeId.ToString)
            '
            ' Apply and log changes, save to DB
            '
            Dim _logKeyList As New LogKeyList
            Dim _id As Integer

            '
            ' ArsMemberRequestApplicant
            '
            'File.WriteAllText("d:\DoctypeId4.txt", _arsMemberRequestApplicant.DocTypeId.ToString)


            Dim _arsMemberRequestApplicantLogDetailList As New SysLogDetailList

            With _arsMemberRequestApplicantOld

                If .ApplicantScreeningId <> _arsMemberRequestApplicant.ApplicantScreeningId Then

                    Dim _oldApplicantScreeningName As String = String.Empty
                    Try
                        _oldApplicantScreeningName = EasSession.DbsApplicantScreening.Rows.Find(Function(m) m.ApplicantScreeningId = .ApplicantScreeningId).ApplicantScreeningName

                    Catch ex As Exception

                    End Try

                    Dim _newApplicantScreeningName As String = EasSession.DbsApplicantScreening.Rows.Find(Function(m) m.ApplicantScreeningId = _arsMemberRequestApplicant.ApplicantScreeningId).ApplicantScreeningName

                    AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.ApplicantScreeningId, .ApplicantScreeningId.ToString, _arsMemberRequestApplicant.ApplicantScreeningId.ToString, .ApplicantScreeningId.ToString + "=" + _oldApplicantScreeningName + "; " + _arsMemberRequestApplicant.ApplicantScreeningId.ToString + "=" + _newApplicantScreeningName)


                End If


                If .ScreeningStatusId <> _arsMemberRequestApplicant.ScreeningStatusId Then

                    Dim _oldScreeningStatusName As String = String.Empty
                    Try
                        _oldScreeningStatusName = EasSession.DbsScreeningStatus.Rows.Find(Function(m) m.ScreeningStatusId = .ScreeningStatusId).ScreeningStatusName
                        Dim _newScreeningStatusName As String = EasSession.DbsScreeningStatus.Rows.Find(Function(m) m.ScreeningStatusId = _arsMemberRequestApplicant.ScreeningStatusId).ScreeningStatusName

                        AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.ScreeningStatusId, .ScreeningStatusId.ToString, _arsMemberRequestApplicant.ScreeningStatusId.ToString, .ScreeningStatusId.ToString + "=" + _oldScreeningStatusName + "; " + _arsMemberRequestApplicant.ScreeningStatusId.ToString + "=" + _newScreeningStatusName)


                    Catch ex As Exception

                    End Try



                End If

                If .ApplicantStatusId <> _arsMemberRequestApplicant.ApplicantStatusId Then

                    Dim _oldApplicantStatusName As String = String.Empty
                    Try
                        _oldApplicantStatusName = EasSession.DbsApplicantStatus.Rows.Find(Function(m) m.ApplicantStatusId = .ApplicantStatusId).ApplicantStatusName
                        Dim _newApplicantStatusName As String = EasSession.DbsApplicantStatus.Rows.Find(Function(m) m.ApplicantStatusId = _arsMemberRequestApplicant.ApplicantStatusId).ApplicantStatusName

                        AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.ApplicantStatusId, .ApplicantStatusId.ToString, _arsMemberRequestApplicant.ApplicantStatusId.ToString, .ApplicantStatusId.ToString + "=" + _oldApplicantStatusName + "; " + _arsMemberRequestApplicant.ApplicantStatusId.ToString + "=" + _newApplicantStatusName)


                    Catch ex As Exception

                    End Try



                End If


                If .Remarks <> _arsMemberRequestApplicant.Remarks Then
                    AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.Remarks, .Remarks, _arsMemberRequestApplicant.Remarks)
                End If

                If .HiredDate <> _arsMemberRequestApplicant.HiredDate Then
                    AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.HiredDate, String.Empty, _arsMemberRequestApplicant.HiredDate.ToDisplayFormat)
                End If

                'If .DeploymentDate <> _arsMemberRequestApplicant.DeploymentDate Then
                '   AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.DeploymentDate, String.Empty, _arsMemberRequestApplicant.DeploymentDate.ToDisplayFormat)
                'End If


                If .EmploymentStatusId <> _arsMemberRequestApplicant.EmploymentStatusId Then

                    Dim _oldEmploymentStatusName As String = String.Empty
                    Try
                        _oldEmploymentStatusName = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = .EmploymentStatusId).EmploymentStatusName
                        Dim _newEmploymentStatusName As String = EasSession.DbsEmploymentStatus.Rows.Find(Function(m) m.EmploymentStatusId = _arsMemberRequestApplicant.EmploymentStatusId).EmploymentStatusName

                        AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.EmploymentStatusId, .EmploymentStatusId.ToString, _arsMemberRequestApplicant.EmploymentStatusId.ToString, .EmploymentStatusId.ToString + "=" + _oldEmploymentStatusName + "; " + _arsMemberRequestApplicant.EmploymentStatusId.ToString + "=" + _newEmploymentStatusName)


                    Catch ex As Exception

                    End Try



                End If

                'File.WriteAllText("d:\DoctypeId5.txt", _arsMemberRequestApplicant.DocTypeId.ToString)
            End With

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection
            Me.UpdateArsMemberRequestApplicant(_arsMemberRequestApplicant)

            _arsMemberRequestApplicant.Name3 = action.Name3

            'Try

            With _arsMemberRequestApplicant
               .RequestDocTypeId = _arsMemberRequestApplicant.RequestDocTypeId
               .RequestDocTypeFileName = _arsMemberRequestApplicant.RequestDocTypeFileName
               .RequestDocTypeGUID = _arsMemberRequestApplicant.RequestDocTypeGUID
               .FileUrl = _arsMemberRequestApplicant.FileUrl
               .FileExtension = _arsMemberRequestApplicant.FileExtension
            End With

            Me.UpdateArsMemberRequestApplicantScreening(_arsMemberRequestApplicant)
            'Catch ex As Exception
            'File.WriteAllText("d:\DoctypeId8.txt", _arsMemberRequestApplicant.DocTypeId.ToString)

            'End Try

            If _arsMemberRequestApplicantLogDetailList.Count > 0 Then

                    With _logKeyList
                        .Clear()
                        .Add("ApplicantDetailId", _arsMemberRequestApplicant.ApplicantDetailId)
                    End With

                    _id = AppLib.CreateLogHeader("InsArsMemberRequestApplicantLog", _logKeyList, LogActionId.Edit, currentUserId)
                    AppLib.AssignLogHeaderId(_id, _arsMemberRequestApplicantLogDetailList)
                    AppLib.CreateLogDetails(_arsMemberRequestApplicantLogDetailList, "ArsMemberRequestApplicantLogDetail")

                End If

                _successFlag = True

            Catch _exception As Exception
                'File.WriteAllText("d:\yyy.txt", _exception.Message)
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

            'Dim _resultA As Results.OkNegotiatedContentResult(Of DataTable) = CType(Me.GetMemberRequestApplicant(applicantDetailId), Results.OkNegotiatedContentResult(Of DataTable))

            'Return Me.Ok(_resultA.Content)
            Return Me.Ok(action.MemberRequestId)

            'Return Me.Ok(True)
        Catch _exception As Exception
            'File.WriteAllText("d:\zzz.txt", _exception.Message)
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("MemberRequestApplicant")>
    <Route("member-request-applicants/{applicantDetailId}")>
    <HttpDelete>
    Public Function RemoveMemberRequestApplicant(applicantDetailId As Integer, <FromUri> q As DeleteQuery) As IHttpActionResult

        If applicantDetailId <= 0 Then
            Throw New ArgumentException("Applicant Detail ID is required.")
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

                'Me.DeleteDbsReligion(religionId, lockId)
                Try

                    Me.DeleteArsMemberRequestApplicant(applicantDetailId, q.LockId)

                Catch ex As Exception

                End Try
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

    '<SymAuthorization("RemoveArsMemberRequest")>
    '<Route("member-request-pooling/{memberRequestId}/{lockId}")>
    '<HttpDelete>
    'Public Function RemoveArsMemberRequest(memberRequestId As Integer, lockId As String) As IHttpActionResult

    '   If memberRequestId <= 0 Then
    '      Throw New ArgumentException("Member Request ID is required.")
    '   End If

    '   Try
    '      Dim _transaction As SqlTransaction = Nothing
    '      Dim _successFlag As Boolean

    '      Try
    '         If Not DataCore.Connect(_databaseId) Then
    '            ' TODO: Cannot connect
    '         End If

    '         _transaction = DataCore.Connection.BeginTransaction()
    '         DataCore.Command.Transaction = _transaction
    '         DataCore.Command.Connection = DataCore.Connection

    '         Me.DeleteArsMemberRequest(memberRequestId, lockId)

    '         _successFlag = True

    '      Catch _exception As Exception
    '         Return Me.BadRequest(_exception.Message)
    '      Finally
    '         With _transaction
    '            If _successFlag Then
    '               .Commit()
    '            Else
    '               .Rollback()
    '            End If
    '            .Dispose()
    '         End With

    '         _transaction = Nothing
    '         DataCore.Disconnect()

    '      End Try

    '      Return Me.Ok(True)

    '   Catch _exception As Exception
    '      Return Me.BadRequest(_exception.Message)

    '   End Try

    'End Function
    Private Sub LoadArsMemberRequest(memberRequest As ArsMemberRequestBody, arsMemberRequest As ArsMemberRequest)

        DataLib.ScatterValues(memberRequest, arsMemberRequest)

    End Sub
    Private Sub LoadArsMemberRequest(row As DataRow, memberRequest As ArsMemberRequest)

        With memberRequest
            .MemberRequestId = row.ToInt32("MemberRequestId")
            .MemberRequestName = row.ToString("MemberRequestName")
            '.OrgId = row.ToInt32("OrgId") --pbn03272025
            '.PlatformId = row.ToInt32("PlatformId") --pbn03272025
            '.ClientContractId = row.ToInt32("ClientContractId") --pbn03272025
            .ClientPosition = row.ToString("ClientPosition")
            .MemberRequestPositionId = row.ToInt32("MemberRequestPositionId")
            .JobCode = row.ToString("JobCode")
            .JobDescription = row.ToString("JobDescription")
            .MemberTypeId = row.ToInt32("MemberTypeId")
            .PayoutTypeId = row.ToInt32("PayoutTypeId")
            .VacancyCount = row.ToInt32("VacancyCount")
            .DeploymentDate = row.ToDate("DeploymentDate")
            '.RegionId = row.ToString("RegionId")
            '.ProvinceId = row.ToString("ProvinceId")
            '.MunicipalityId = row.ToString("MunicipalityId")
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
    Private Sub InsertArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ApplicantId")
            .Add("LogActionId")
            .Add("FileUrl")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestApplicant", applicant, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsMemberRequestApplicant(applicant)

            .ExecuteNonQuery()

        End With

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
            '.AddWithValue("@OrgId", memberRequest.OrgId.ToNullable) --pbn03272025
            '.AddWithValue("@PlatformId", memberRequest.PlatformId.ToNullable) --pbn03272025
            '.AddWithValue("@ClientContractId", memberRequest.ClientContractId) --pbn03272025
            .AddWithValue("@ClientPosition", memberRequest.ClientPosition)
            .AddWithValue("@MemberRequestPositionId", memberRequest.MemberRequestPositionId)
            .AddWithValue("@JobCode", memberRequest.JobCode)
            .AddWithValue("@JobDescription", memberRequest.JobDescription)
            .AddWithValue("@MemberTypeId", memberRequest.MemberTypeId)
            .AddWithValue("@PayoutTypeId", memberRequest.PayoutTypeId)
            .AddWithValue("@VacancyCount", memberRequest.VacancyCount)
            .AddWithValue("@DeploymentDate", memberRequest.DeploymentDate)
            '.AddWithValue("@RegionId", memberRequest.RegionId)
            '.AddWithValue("@ProvinceId", memberRequest.ProvinceId.ToNullable)
            '.AddWithValue("@MunicipalityId", memberRequest.MunicipalityId.ToNullable)
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
    Private Sub DeleteArsMemberRequestApplicant(applicantDetailId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ApplicantDetailId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsMemberRequestApplicant", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ApplicantDetailId", applicantDetailId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

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
            'If .OrgId <> newRecord.OrgId Then Return True --pbn03272025
            'If .PlatformId <> newRecord.PlatformId Then Return True --pbn03272025
            'If .ClientContractId <> newRecord.ClientContractId Then Return True --pbn03272025
            If .ClientPosition <> newRecord.ClientPosition Then Return True
            If .MemberRequestPositionId <> newRecord.MemberRequestPositionId Then Return True
            If .JobCode <> newRecord.JobCode Then Return True
            If .JobDescription <> newRecord.JobDescription Then Return True
            If .MemberTypeId <> newRecord.MemberTypeId Then Return True
            If .PayoutTypeId <> newRecord.PayoutTypeId Then Return True
            'If .EmploymentTypeId <> newRecord.EmploymentTypeId Then Return True
            If .VacancyCount <> newRecord.VacancyCount Then Return True
            If .DeploymentDate <> newRecord.DeploymentDate Then Return True
            'If .RegionId <> newRecord.RegionId Then Return True
            'If .ProvinceId <> newRecord.ProvinceId Then Return True
            'If .MunicipalityId <> newRecord.MunicipalityId Then Return True
            If .PositionKeyword <> newRecord.PositionKeyword Then Return True
            If .SkillNameKeyword <> newRecord.SkillNameKeyword Then Return True
            If .ClientPayGroupId <> newRecord.ClientPayGroupId Then Return True
            If .WorkingDays <> newRecord.WorkingDays Then Return True
        End With

        Return False

    End Function

    Private Sub LoadArsMemberRequestApplicant(applicant As ArsMemberRequestApplicantBody, ArsMemberRequestApplicant As ArsMemberRequestApplicant)

        DataLib.ScatterValues(applicant, ArsMemberRequestApplicant)

    End Sub

    Private Sub LoadArsMemberRequestApplicant(row As DataRow, arsMemberRequestApplicant As ArsMemberRequestApplicant)

        With arsMemberRequestApplicant
            .ApplicantDetailId = row.ToInt32("ApplicantDetailId")
            .MemberId = row.ToInt32("MemberId")
            .ApplicantScreeningId = row.ToInt32("ApplicantScreeningId")
            .ScreeningStatusId = row.ToInt32("ScreeningStatusId")
            .ApplicantStatusId = row.ToInt32("ApplicantStatusId")
            .Remarks = row.ToString("Remarks")
            .HiredDate = row.ToDate("HiredDate")
            .EmploymentStatusId = row.ToInt32("EmploymentStatusId")
            .DeploymentDate = row.ToDate("DeploymentDate")
            .Name3 = row.ToString("Name3")
            .RequestDocTypeId = row.ToInt32("DocTypeId")
            '.RequestDocTypeReference = row.ToString("DocTypeReference")
            .RequestDocTypeFileName = row.ToString("DocTypeFileName")
            .RequestDocTypeGUID = row.ToString("DocTypeGUID")
            .FileExtension = row.ToString("FileExtension")
            .FileUrl = row.ToString("FileUrl")
        End With

    End Sub

    Private Sub LoadHrsMemberDocType(applicant As HrsMemberDocTypeBody, ArsMemberRequestApplicant As HrsMemberDocTypeApplicant)

        DataLib.ScatterValues(applicant, ArsMemberRequestApplicant)

    End Sub

    Private Sub LoadHrsMemberDocType(row As DataRow, arsMemberRequestApplicant As HrsMemberDocTypeApplicant)

        With arsMemberRequestApplicant

            .DocTypeDetailId = row.ToInt32("DocTypeDetailId")
            .DocTypeId = row.ToInt32("DocTypeId")
            .DocTypeReference = row.ToString("DocTypeReference")
            .DocTypeFileName = row.ToString("DocTypeFileName")
            .DocTypeGUID = row.ToString("DocTypeGUID")
            .FileExtension = row.ToString("FileExtension")
            .FileUrl = row.ToString("FileUrl")
        End With

    End Sub

   Private Sub UpdateArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

      Dim _keyFields As New List(Of String)
      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ApplicantId")
         .Add("FileUrl")
         .Add("LogActionId")
      End With

      With _keyFields
         .Add("ApplicantDetailId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsMemberRequestApplicant", applicant, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Try
            UploadMemberFile(applicant.MemberId, applicant.RequestDocTypeFileName, applicant.RequestDocTypeGUID)
         Catch ex As Exception

         End Try

         Me.AddInsertUpdateParamsArsMemberRequestApplicant(applicant)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(applicant.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

      If applicant.ApplicantStatusId = EmploymentApplicantStatus.Hired Then

         With _excludedFields
            .Add("LockId")
         End With

         With _keyFields
            .Add("MemberId")
            '.Add("LockId")
         End With

         Dim _hrsMember As New HrsMemberInitialHiredDate

         With _hrsMember
            .MemberId = applicant.MemberId
         End With

         With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsMember", _hrsMember, _keyFields, _excludedFields)
            .CommandType = CommandType.Text
            '.Parameters.AddWithValue("@MemberRequestEducationDetailId", _detail.MemberRequestEducationDetailId)

            With DataCore.Command.Parameters
               .Clear()
               .AddWithValue("@Member", _hrsMember.MemberId)
               .AddWithValue("@InitialHiredDate", Today)
            End With

            .Parameters.AddWithValue("@MemberId", _hrsMember.MemberId)

            .ExecuteNonQuery()

         End With
      End If

   End Sub

   'Private Sub InsertArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

   '   Dim _excludedFields As New List(Of String)

   '   With _excludedFields
   '      .Add("ApplicantId")
   '      .Add("LogActionId")
   '      .Add("LockId")
   '   End With

   '   With DataCore.Command
   '      .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestApplicant", applicant, _excludedFields)
   '      .CommandType = CommandType.Text

   '      Me.AddInsertUpdateParamsArsMemberRequestApplicant(applicant)

   '      .ExecuteNonQuery()

   '   End With

   'End Sub


   Private Sub UpdateArsMemberRequestApplicantScreening(applicant As ArsMemberRequestApplicant)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ScreeningDetailId")
            .Add("MemberRequestId")
            .Add("MemberId")
            .Add("ApplicantId")
            .Add("ApplicantStatusId")
            .Add("ScreeningDate")
            .Add("Remarks")
            .Add("HiredDate")
            .Add("EmploymentStatusId")
            .Add("DeploymentDate")
            .Add("LogActionId")
            .Add("Name3")

         '.Add("RequestDocTypeId")
         ''.Add("RequestDocTypeReference")
         '.Add("RequestDocTypeFileName")
         '.Add("RequestDocTypeGUID")
         .Add("FileUrl")
         '.Add("FileExtension")
         .Add("LockId")
        End With

        'With _keyFields
        '   .Add("ApplicantDetailId")
        '   .Add("LockId")
        'End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestApplicantScreening", applicant, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsMemberRequestApplicantSCreening(applicant)
            '.Parameters.AddWithValue("@LockId", Convert.FromBase64String(applicant.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub


    Private Sub AddInsertUpdateParamsArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ApplicantDetailId", applicant.ApplicantDetailId)
            .AddWithValue("@MemberId", applicant.MemberId)
            .AddWithValue("@MemberRequestId", applicant.MemberRequestId)
            .AddWithValue("@ApplicantScreeningId", applicant.ApplicantScreeningId.ToNullable)
            .AddWithValue("@ScreeningStatusId", applicant.ScreeningStatusId)
            .AddWithValue("@ApplicantStatusId", applicant.ApplicantStatusId)
            .AddWithValue("@Remarks", applicant.Remarks.ToNullable)
            .AddWithValue("@HiredDate", applicant.HiredDate.ToNullable)
            .AddWithValue("@EmploymentStatusId", applicant.EmploymentStatusId)
            .AddWithValue("@DeploymentDate", applicant.DeploymentDate.ToNullable)
            .AddWithValue("@Name3", applicant.Name3)
            .AddWithValue("@RequestDocTypeId", applicant.RequestDocTypeId)
            '.AddWithValue("@RequestDocTypeReference", applicant.RequestDocTypeReference)
            .AddWithValue("@RequestDocTypeFileName", applicant.RequestDocTypeFileName.ToNullable)
            .AddWithValue("@RequestDocTypeGUID", applicant.RequestDocTypeGUID.ToNullable)
            .AddWithValue("@FileUrl", applicant.FileUrl.ToNullable)
            .AddWithValue("@FileExtension", Path.GetExtension(applicant.RequestDocTypeFileName.Replace("""", "")).ToLowerInvariant.ToNullable)
        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsMemberRequestApplicantSCreening(applicant As ArsMemberRequestApplicant)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ApplicantDetailId", applicant.ApplicantDetailId)
            .AddWithValue("@ApplicantScreeningId", applicant.ApplicantScreeningId)
            .AddWithValue("@ScreeningStatusId", applicant.ScreeningStatusId)
         .AddWithValue("@Name3", applicant.Name3)
         .AddWithValue("@RequestDocTypeId", applicant.RequestDocTypeId)
         '.AddWithValue("@RequestDocTypeReference", applicant.RequestDocTypeReference)
         .AddWithValue("@RequestDocTypeFileName", applicant.RequestDocTypeFileName.ToNullable)
         .AddWithValue("@RequestDocTypeGUID", applicant.RequestDocTypeGUID.ToNullable)
         .AddWithValue("@FileUrl", applicant.FileUrl.ToNullable)
         .AddWithValue("@FileExtension", Path.GetExtension(applicant.RequestDocTypeFileName.Replace("""", "")).ToLowerInvariant.ToNullable)

      End With

    End Sub

    Private Sub InsertHrsMemberDocTypes(applicant As HrsMemberDocTypeApplicant)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("DocTypeDetailId")
            .Add("LockId")

        End With

        With _keyFields
            .Add("DocTypeDetailId")
            '.Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDocType", applicant, _excludedFields)

            .CommandType = CommandType.Text

            Try
                UploadMemberFile(applicant.MemberId, applicant.DocTypeFileName, applicant.DocTypeGUID)
            Catch ex As Exception

            End Try

            Me.AddInsertUpdateParamsHrsMemberDocType(applicant)


            .ExecuteNonQuery()

        End With

    End Sub

    Private Sub AddInsertUpdateParamsHrsMemberDocType(docType As HrsMemberDocTypeApplicant)

        With DataCore.Command.Parameters
            .Clear()
            .AddWithValue("@DocTypeDetailId", docType.DocTypeDetailId)
            .AddWithValue("@MemberId", docType.MemberId)

            .AddWithValue("@DocTypeId", docType.DocTypeId)

            .AddWithValue("@DocTypeReference", docType.DocTypeReference)
            .AddWithValue("@DocTypeFileName", docType.DocTypeFileName.ToNullable)

            .AddWithValue("@DocTypeGUID", docType.DocTypeGUID.ToNullable)
            .AddWithValue("@FileUrl", docType.FileUrl.ToNullable)


            .AddWithValue("@FileExtension", Path.GetExtension(docType.DocTypeFileName.Replace("""", "")).ToLowerInvariant.ToNullable)

        End With


    End Sub

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

    Friend Shared Function UploadTradeTestFile(memberId As Integer, fileName As String, guid As String) As Boolean

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
End Class

Public Class ArsMemberRequestApplicantBody
   Inherits ArsMemberRequestApplicant
End Class

Public Class HrsMemberDocTypeBody
   Inherits HrsMemberDocTypeApplicant
End Class

Public Class FilterParam
   Public Property filter As String
   Public Property skillNameKeyword As String
   Public Property positionKeyword As String
End Class
Public Class XXXX
   Public Property memberId As Integer
   Public Property skillNameKeyword As String
   Public Property positionKeyword As String
End Class
