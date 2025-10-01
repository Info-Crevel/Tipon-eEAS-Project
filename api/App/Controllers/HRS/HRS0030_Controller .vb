
Imports ImageMagick

<RoutePrefix("api")>
Public Class HRS0030_Controller
   Inherits ApiController

   Private ReadOnly _databaseId As String = "sys"
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

   <SymAuthorization("GetApplicantProvinces")>
   <Route("applicant-provinces")>
   <HttpGet>
   Public Function GetApplicantProvinces() As IHttpActionResult
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

   <SymAuthorization("GetApplicantMunicipalities")>
   <Route("applicant-municipalities/{provinceId}")>
   <HttpGet>
   Public Function GetApplicantMunicipalities(provinceId As Integer) As IHttpActionResult

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

   <SymAuthorization("GetApplicantBarangays")>
   <Route("applicant-barangays/{municipalityId}")>
   <HttpGet>
   Public Function GetApplicantBarangays(municipalityId As Integer) As IHttpActionResult

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

   <SymAuthorization("GetApplicantPostals")>
   <Route("applicant-postals/{barangayId}")>
   <HttpGet>
   Public Function GetApplicantPostals(barangayId As Integer) As IHttpActionResult

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

   <SymAuthorization("GetReferences_HRS0030")>
   <Route("references/hrs0030")>
   <HttpGet>
   Public Function GetReferences_HRS0030() As IHttpActionResult

      Try
         Using _direct As New SqlDirect("web.HRS0030_References")
            With _direct

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "sources"
                     .Tables(1).TableName = "sex"
                     .Tables(2).TableName = "civilStatus"
                     .Tables(3).TableName = "religion"
                     .Tables(4).TableName = "applicantValidation"
                     .Tables(5).TableName = "applicantStatus"
                     .Tables(6).TableName = "suffix"
                     .Tables(7).TableName = "docType"
                  End With

                  Return Me.Ok(_dataSet)

               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetApplicant")>
   <Route("applicants/{applicantId}")>
   <HttpGet>
   Public Function GetApplicant(applicantId As Integer) As IHttpActionResult

      If applicantId <= 0 Then
         Throw New ArgumentException("Applicant ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.HRS0030")
            With _direct
               .AddParameter("ApplicantId", applicantId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "applicant"
                     .Tables(1).TableName = "province"
                     .Tables(2).TableName = "municipality"
                     .Tables(3).TableName = "barangay"
                     .Tables(4).TableName = "docs"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetHrsApplicantLog")>
   <Route("applicant/{logId}/{applicantId}/log")>
   <HttpGet>
   Public Function GetHrsApplicantLog(logId As Integer, applicantId As Integer) As IHttpActionResult

      If applicantId <= 0 Then
         Throw New ArgumentException("Applicant ID is required.")
      End If

      Dim _applicant As String = "web.HRS0030_Log"

      Select Case logId
         'Case LogTable.Applicant
         '   _applicant = "web.HRS0010_MemberLog"
         Case LogTable.DocType
            _applicant = "web.HRS0030_DocTypeLog"
      End Select

      Try
         Using _direct As New SqlDirect(_applicant)
            With _direct
               .AddParameter("applicantId", applicantId)

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


   '<SymAuthorization("GetHrsApplicantLog")>
   '<Route("applicant/{applicantId}/log")>
   '<HttpGet>
   'Public Function GetHrsApplicantLog(applicantId As Integer) As IHttpActionResult

   '    If applicantId <= 0 Then
   '        Throw New ArgumentException("Applicant ID is required.")
   '    End If

   '    Try
   '        Using _direct As New SqlDirect("Web.HRS0030_Log")
   '            With _direct
   '                .AddParameter("ApplicantId", applicantId)

   '                ' Allow DateTime
   '                Dim _settings As New JsonSerializerSettings
   '                With _settings
   '                    .ContractResolver = New CamelCasePropertyNamesContractResolver
   '                    .DateFormatString = "yyyy-MM-ddTHH:mm"
   '                End With

   '                Using _dataTable As DataTable = _direct.ExecuteDataTable()
   '                    'Return Me.Ok(_dataTable)
   '                    Return Json(_dataTable, _settings)
   '                End Using

   '            End With
   '        End Using

   '    Catch _exception As Exception
   '        Return Me.BadRequest(_exception.Message)
   '    End Try

   'End Function

   <SymAuthorization("CreateApplicant")>
   <Route("applicants/{currentUserId}")>
   <HttpPost>
   Public Function CreateApplicant(currentUserId As Integer, <FromBody> applicant As ApplicantBody) As IHttpActionResult

      If applicant.ApplicantId <> -1 Then
         Throw New ArgumentException("Applicant ID is unrecognized.")
      End If

      Try

         '
         ' Assign new ID from sequencer
         '
         Dim _applicantId As Integer = SysLib.GetNextSequence("ApplicantId")

         applicant.ApplicantId = _applicantId
         applicant.ApplicantStatusId = ApplicantStatus.Active
         applicant.RegionId = GetRegion(applicant.ProvinceId.ToString)

         '
         ' Load proposed values from payload
         '

         Dim _hrsApplicant As New HrsApplicant
         Dim _hrsApplicantDocTypeList As New HrsApplicantDocTypeList


         Me.LoadHrsApplicant(applicant, _hrsApplicant)

         ' HrsApplicantDoc
         For Each _doc As HrsApplicantDocType In applicant.Docs
            _doc.ApplicantId = _applicantId
            _hrsApplicantDocTypeList.Add(_doc)
         Next


         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList

         Dim _id As Integer
         Dim _hrsApplicantLogDetailList As New SysLogDetailList
         Dim _hrsApplicantDocTypeLogDetailList As New SysLogDetailList

         Try


            With _hrsApplicant

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantLastName, String.Empty, .ApplicantLastName)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantFirstName, String.Empty, .ApplicantFirstName)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantMiddleName, String.Empty, .ApplicantMiddleName)
               'AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantSuffix, String.Empty, .ApplicantSuffix)

               Dim _memberSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .ApplicantSuffixId).MemberSuffixName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantSuffixId, String.Empty, .ApplicantSuffixId.ToString, .ApplicantSuffixId.ToString + "=" + _memberSuffixName)

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.BirthDate, String.Empty, .BirthDate.ToDisplayFormat)

               Dim _sexName As String = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = .SexId).SexName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.SexId, String.Empty, .SexId.ToString, .SexId.ToString + "=" + _sexName)

               Dim _civilStatusName As String = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = .CivilStatusId).CivilStatusName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.CivilStatusId, String.Empty, .CivilStatusId.ToString, .CivilStatusId.ToString + "=" + _civilStatusName)

               Dim _religionName As String = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = .ReligionId).ReligionName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ReligionId, String.Empty, .ReligionId.ToString, .ReligionId.ToString + "=" + _religionName)

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Address1, String.Empty, .Address1)
               'AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Address2, String.Empty, .Address2)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.PostalCode, String.Empty, .PostalCode)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.PhoneNumber, String.Empty, .PhoneNumber)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MobileNumber, String.Empty, .MobileNumber)
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Email, String.Empty, .Email)

               Dim _applicationSourceName As String = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = .ApplicationSourceId).ApplicationSourceName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicationSourceId, String.Empty, .ApplicationSourceId.ToString, .ApplicationSourceId.ToString + "=" + _applicationSourceName)

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicationDate, String.Empty, .ApplicationDate.ToDisplayFormat)

               Dim _applicantStatusName As String = EasSession.HrsApplicantStatus.Rows.Find(Function(m) m.ApplicantStatusId = .ApplicantStatusId).ApplicantStatusName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantStatusId, String.Empty, .ApplicantStatusId.ToString, .ApplicantStatusId.ToString + "=" + _applicantStatusName)

               Dim _regionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .RegionId).RegionName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.RegionId, String.Empty, .RegionId.ToString, .RegionId.ToString + "=" + _regionName)

               Dim _provinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .ProvinceId).ProvinceName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ProvinceId, String.Empty, .ProvinceId.ToString, .ProvinceId.ToString + "=" + _provinceName)

               Dim _municipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MunicipalityId, String.Empty, .MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _municipalityName)

               Dim _barangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .BarangayId).BarangayName
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.BarangayId, String.Empty, .BarangayId.ToString, .BarangayId.ToString + "=" + _barangayName)

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MemberRequestId, String.Empty, .MemberRequestId.ToString)

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

            Me.InsertHrsApplicant(_hrsApplicant)

            If _hrsApplicantLogDetailList.Count > 0 Then
               With _logKeyList
                  .Clear()
                  .Add("ApplicantId", _hrsApplicant.ApplicantId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsApplicantLog", _logKeyList, LogActionId.Add, currentUserId)
               AppLib.AssignLogHeaderId(_id, _hrsApplicantLogDetailList)
               AppLib.CreateLogDetails(_hrsApplicantLogDetailList, "HrsApplicantLogDetail")

            End If


#Region "HrsApplicantDoc"

            If _hrsApplicantDocTypeList.Count > 0 Then
               Me.InsertHrsApplicantDocTypes(_hrsApplicantDocTypeList)
            End If

            For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeList

               With _logKeyList
                  .Clear()
                  .Add("ApplicantId", _new.ApplicantId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)

               _hrsApplicantDocTypeLogDetailList.Clear()

               With _new
                  Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                  AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)

                  AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, String.Empty, .DocTypeReference)
                  AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, String.Empty, .DocTypeFileName)

                  AppLib.CreateLogDetails(_hrsApplicantDocTypeLogDetailList, "HrsApplicantDocTypeLogDetail")

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

         Return Me.Ok(applicant.ApplicantId)

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)

      End Try

   End Function

   <SymAuthorization("ModifyApplicant")>
   <Route("applicants/{applicantId}/{currentUserId}")>
   <HttpPut>
   Public Function ModifyApplicant(applicantId As Integer, currentUserId As Integer, <FromBody> applicant As ApplicantBody) As IHttpActionResult

      If applicantId <= 0 Then
         Throw New ArgumentException("Applicant ID is required.")
      End If

      Dim _hrsApplicantMember As New HrsApplicantMember

      Try
         '
         ' Load proposed values from payload
         '

         applicant.RegionId = GetRegion(applicant.ProvinceId.ToString)
         Dim _hrsApplicant As New HrsApplicant
         Dim _arsMemberRequestApplicant As New ArsMemberRequestApplicant

         Dim _hrsApplicantMemberDocTypeList As New HrsApplicantMemberDocTypeList

         Dim _memberId As Integer

         If applicant.ApplicantStatusId = ApplicantStatus.Member Then

            _memberId = SysLib.GetNextSequence("MemberId")

                Dim _applicantDetailId As Integer = SysLib.GetNextSequence("ApplicantDetailId")


                With _hrsApplicantMember
               '.ApplicantId = _hrsApplicant.ApplicantId
               .MemberId = _memberId
               .MemberLastName = applicant.ApplicantLastName
               .MemberFirstName = applicant.ApplicantFirstName
               .MemberMiddleName = applicant.ApplicantMiddleName
               .MemberSuffixId = applicant.ApplicantSuffixId
               .BirthDate = applicant.BirthDate
               .SexId = applicant.SexId
               .CivilStatusId = applicant.CivilStatusId
               .ReligionId = applicant.ReligionId
                    .Address1 = applicant.Address1
                    .MemberTypeId = applicant.MemberTypeId
                    '.Address2 = _hrsApplicant.Address2
                    .PostalCode = applicant.PostalCode
               .PhoneNumber = applicant.PhoneNumber
               .MobileNumber = applicant.MobileNumber
               .Email = applicant.Email
               .RegionId = applicant.RegionId
               .ProvinceId = applicant.ProvinceId
               .MunicipalityId = applicant.MunicipalityId
               .BarangayId = applicant.BarangayId

            End With

            With _arsMemberRequestApplicant
               '.ApplicantId = _hrsApplicant.ApplicantId
               .ApplicantDetailId = _applicantDetailId
               .MemberId = _memberId
               .MemberRequestId = applicant.MemberRequestId
               .ApplicantStatusId = 1 'Listed
               .ScreeningStatusId = 1 'Pending
            End With

            applicant.MemberId = _memberId


         End If



         Dim _hrsApplicantDocTypeList As New HrsApplicantDocTypeList

         Me.LoadHrsApplicant(applicant, _hrsApplicant)

         For Each _docType As HrsApplicantDocType In applicant.Docs
            _hrsApplicantDocTypeList.Add(_docType)
         Next

         Dim _detail As HrsApplicantMemberDocType

         For Each _row As HrsApplicantDocType In _hrsApplicantDocTypeList
            _detail = New HrsApplicantMemberDocType

            With _detail
               .MemberId = _memberId

               .DocTypeDetailId = _row.DocTypeDetailId
               .DocTypeId = _row.DocTypeId

               .DocTypeReference = _row.DocTypeReference
               .DocTypeFileName = _row.DocTypeFileName
               .DocTypeGUID = _row.DocTypeGUID
               .FileExtension = _row.FileExtension
               .FileUrl = _row.FileUrl

            End With

            _hrsApplicantMemberDocTypeList.Add(_detail)

         Next




         '
         ' Load old values from DB
         '
         Dim _hrsApplicantOld As New HrsApplicant
         Dim _hrsApplicantDocTypeListOld As New HrsApplicantDocTypeList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetApplicant(applicantId), Results.OkNegotiatedContentResult(Of DataSet))


         Using _dataSet As DataSet = _result.Content
            Dim _row As DataRow = _dataSet.Tables("applicant").Rows(0)
            Me.LoadHrsApplicant(_row, _hrsApplicantOld)
            Me.LoadHrsApplicantDocTypeList(_dataSet.Tables("docs").Rows, _hrsApplicantDocTypeListOld)
         End Using


         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
         Dim _id As Integer

         '
         ' HrsApplicant
         '

         Dim _hrsApplicantLogDetailList As New SysLogDetailList

         With _hrsApplicantOld


            If .ApplicantLastName <> _hrsApplicant.ApplicantLastName Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantLastName, .ApplicantLastName, _hrsApplicant.ApplicantLastName)
            End If

            If .MemberId <> _hrsApplicant.MemberId Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MemberId, .MemberId.ToString, _hrsApplicant.MemberId.ToString)
            End If

            If .ApplicantFirstName <> _hrsApplicant.ApplicantFirstName Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantFirstName, .ApplicantFirstName, _hrsApplicant.ApplicantFirstName)
            End If

            If .ApplicantMiddleName <> _hrsApplicant.ApplicantMiddleName Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantMiddleName, .ApplicantMiddleName, _hrsApplicant.ApplicantMiddleName)
            End If

            'If .ApplicantSuffix <> _hrsApplicant.ApplicantSuffix Then
            '    AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantSuffix, .ApplicantSuffix, _hrsApplicant.ApplicantSuffix)
            'End If


            If .BirthDate <> _hrsApplicant.BirthDate Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.BirthDate, .BirthDate.ToDisplayFormat, _hrsApplicant.BirthDate.ToDisplayFormat)
            End If

            If .ApplicationDate <> _hrsApplicant.ApplicationDate Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicationDate, .ApplicationDate.ToDisplayFormat, _hrsApplicant.ApplicationDate.ToDisplayFormat)
            End If

            If .SexId <> _hrsApplicant.SexId Then
               Dim _oldSexName As String = String.Empty
               Try
                  _oldSexName = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = .SexId).SexName

               Catch ex As Exception

               End Try

               Dim _newSexName As String = EasSession.DbsSex.Rows.Find(Function(m) m.SexId = _hrsApplicant.SexId).SexName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.SexId, .SexId.ToString, _hrsApplicant.SexId.ToString, .SexId.ToString + "=" + _oldSexName + "; " + _hrsApplicant.SexId.ToString + "=" + _newSexName)
            End If


            If .CivilStatusId <> _hrsApplicant.CivilStatusId Then
               Dim _oldCivilStatusName As String = String.Empty
               Try
                  _oldCivilStatusName = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = .CivilStatusId).CivilStatusName

               Catch ex As Exception

               End Try

               Dim _newCivilStatusName As String = EasSession.DbsCivilStatus.Rows.Find(Function(m) m.CivilStatusId = _hrsApplicant.CivilStatusId).CivilStatusName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.CivilStatusId, .CivilStatusId.ToString, _hrsApplicant.CivilStatusId.ToString, .CivilStatusId.ToString + "=" + _oldCivilStatusName + "; " + _hrsApplicant.CivilStatusId.ToString + "=" + _newCivilStatusName)
            End If

            If .ReligionId <> _hrsApplicant.ReligionId Then

               Dim _oldReligionName As String = String.Empty

               Try
                  _oldReligionName = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = .ReligionId).ReligionName

               Catch ex As Exception

               End Try


               Dim _newReligionName As String = EasSession.DbsReligion.Rows.Find(Function(m) m.ReligionId = _hrsApplicant.ReligionId).ReligionName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ReligionId, .ReligionId.ToString, _hrsApplicant.ReligionId.ToString, .ReligionId.ToString + "=" + _oldReligionName + "; " + _hrsApplicant.ReligionId.ToString + "=" + _newReligionName)
            End If

            If .Address1 <> _hrsApplicant.Address1 Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Address1, .Address1, _hrsApplicant.Address1)
            End If

            'If .Address2 <> _hrsApplicant.Address2 Then
            '   AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Address2, .Address2, _hrsApplicant.Address2)
            'End If

            If .PostalCode <> _hrsApplicant.PostalCode Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.PostalCode, .PostalCode, _hrsApplicant.PostalCode)
            End If

            If .PhoneNumber <> _hrsApplicant.PhoneNumber Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.PhoneNumber, .PhoneNumber, _hrsApplicant.PhoneNumber)
            End If

            If .MobileNumber <> _hrsApplicant.MobileNumber Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MobileNumber, .MobileNumber, _hrsApplicant.MobileNumber)
            End If

            If .Email <> _hrsApplicant.Email Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.Email, .Email, _hrsApplicant.Email)
            End If


            If .ApplicationSourceId <> _hrsApplicant.ApplicationSourceId Then
               Dim _oldApplicationSourceName As String = String.Empty

               If .ApplicationSourceId > 0 Then
                  _oldApplicationSourceName = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = .ApplicationSourceId).ApplicationSourceName
               End If

               Dim _newApplicationSourceName As String = EasSession.DbsApplicationSource.Rows.Find(Function(m) m.ApplicationSourceId = _hrsApplicant.ApplicationSourceId).ApplicationSourceName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicationSourceId, .ApplicationSourceId.ToString, _hrsApplicant.ApplicationSourceId.ToString, .ApplicationSourceId.ToString + "=" + _oldApplicationSourceName + "; " + _hrsApplicant.ApplicationSourceId.ToString + "=" + _newApplicationSourceName)
            End If

            If .ApplicantStatusId <> _hrsApplicant.ApplicantStatusId Then
               Dim _oldApplicantStatusName As String = String.Empty

               Try
                  _oldApplicantStatusName = EasSession.HrsApplicantStatus.Rows.Find(Function(m) m.ApplicantStatusId = .ApplicantStatusId).ApplicantStatusName
               Catch ex As Exception
               End Try

               Dim _newApplicantStatusName As String = EasSession.HrsApplicantStatus.Rows.Find(Function(m) m.ApplicantStatusId = _hrsApplicant.ApplicantStatusId).ApplicantStatusName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantStatusId, .ApplicantStatusId.ToString, _hrsApplicant.ApplicantStatusId.ToString, .ApplicantStatusId.ToString + "=" + _oldApplicantStatusName + "; " + _hrsApplicant.ApplicantStatusId.ToString + "=" + _newApplicantStatusName)
            End If

            If .ApplicantSuffixId <> _hrsApplicant.ApplicantSuffixId Then
               Dim _oldMemberSuffixName As String = String.Empty
               Try
                  _oldMemberSuffixName = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = .ApplicantSuffixId).MemberSuffixName

               Catch ex As Exception

               End Try

               Dim _newMemberSuffixName As String = EasSession.DbsMemberSuffix.Rows.Find(Function(m) m.MemberSuffixId = _hrsApplicant.ApplicantSuffixId).MemberSuffixName

               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ApplicantSuffixId, .ApplicantSuffixId.ToString, _hrsApplicant.ApplicantSuffixId.ToString, .ApplicantSuffixId.ToString + "=" + _oldMemberSuffixName + "; " + _hrsApplicant.ApplicantSuffixId.ToString + "=" + _newMemberSuffixName)
            End If

            If .RegionId <> _hrsApplicant.RegionId Then
               Dim _oldRegionName As String = String.Empty
               Try
                  _oldRegionName = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = .RegionId).RegionName

                  Dim _newRegionName As String = EasSession.DbsRegion.Rows.Find(Function(m) m.RegionId = _hrsApplicant.RegionId).RegionName

                  AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.RegionId, .RegionId.ToString, _hrsApplicant.RegionId.ToString, .RegionId.ToString + "=" + _oldRegionName + "; " + _hrsApplicant.RegionId.ToString + "=" + _newRegionName)


               Catch ex As Exception

               End Try

            End If

            If .ProvinceId <> _hrsApplicant.ProvinceId Then
               Dim _oldProvinceName As String = String.Empty
               Try
                  _oldProvinceName = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = .ProvinceId).ProvinceName

               Catch ex As Exception

                  Dim _newProvinceName As String = EasSession.DbsProvince.Rows.Find(Function(m) m.ProvinceId = _hrsApplicant.ProvinceId).ProvinceName

                  AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.ProvinceId, .ProvinceId.ToString, _hrsApplicant.ProvinceId.ToString, .ProvinceId.ToString + "=" + _oldProvinceName + "; " + _hrsApplicant.ProvinceId.ToString + "=" + _newProvinceName)

               End Try

            End If

            If .MunicipalityId <> _hrsApplicant.MunicipalityId Then
               Dim _oldMunicipalityName As String = String.Empty
               Try
                  _oldMunicipalityName = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = .MunicipalityId).MunicipalityName

                  Dim _newMunicipalityName As String = EasSession.DbsMunicipality.Rows.Find(Function(m) m.MunicipalityId = _hrsApplicant.MunicipalityId).MunicipalityName

                  AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MunicipalityId, .MunicipalityId.ToString, _hrsApplicant.MunicipalityId.ToString, .MunicipalityId.ToString + "=" + _oldMunicipalityName + "; " + _hrsApplicant.MunicipalityId.ToString + "=" + _newMunicipalityName)

               Catch ex As Exception

               End Try


            End If

            If .BarangayId <> _hrsApplicant.BarangayId Then
               Dim _oldBarangayName As String = String.Empty

               Try
                  _oldBarangayName = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = .BarangayId).BarangayName

                  Dim _newBarangayName As String = EasSession.DbsBarangay.Rows.Find(Function(m) m.BarangayId = _hrsApplicant.BarangayId).BarangayName

                  AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.BarangayId, .BarangayId.ToString, _hrsApplicant.BarangayId.ToString, .BarangayId.ToString + "=" + _oldBarangayName + "; " + _hrsApplicant.BarangayId.ToString + "=" + _newBarangayName)

               Catch ex As Exception

               End Try

            End If

            If .MemberRequestId <> _hrsApplicant.MemberRequestId Then
               AppLib.AddLogDetail(_hrsApplicantLogDetailList, 0, LogColumnId.MemberRequestId, .MemberRequestId.ToString, _hrsApplicant.MemberRequestId.ToString)
            End If


         End With


#Region "HrsApplicantDocType"

         Dim _hrsApplicantDocTypeLogDetailList As New SysLogDetailList

         Dim _removeDocTypeCount As Integer
         Dim _addDocTypeCount As Integer
         Dim _editDocTypeCount As Integer

         ' Mark details for deletion if not found in new list

         For Each _old As HrsApplicantDocType In _hrsApplicantDocTypeListOld
            _old.LogActionId = LogActionId.Delete
            For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeList
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

         For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeList
            _new.LogActionId = LogActionId.Add
            For Each _old As HrsApplicantDocType In _hrsApplicantDocTypeListOld
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

         Dim _hrsApplicantDocTypeListNew As New HrsApplicantDocTypeList      ' for adding new Trx Details

         If _addDocTypeCount > 0 Then
            Dim _hrsApplicantDocType As HrsApplicantDocType
            For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeList
               If _new.LogActionId = LogActionId.Add Then
                  _hrsApplicantDocType = New HrsApplicantDocType
                  _hrsApplicantDocTypeListNew.Add(_hrsApplicantDocType)
                  DataLib.ScatterValues(_new, _hrsApplicantDocType)
                  _hrsApplicantDocType.ApplicantId = applicant.ApplicantId
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

            Me.UpdateHrsApplicant(_hrsApplicant)

            If _hrsApplicantLogDetailList.Count > 0 Then

               With _logKeyList
                  .Clear()
                  .Add("ApplicantId", _hrsApplicant.ApplicantId)
               End With

               _id = AppLib.CreateLogHeader("InsHrsApplicantLog", _logKeyList, LogActionId.Edit, currentUserId)
               AppLib.AssignLogHeaderId(_id, _hrsApplicantLogDetailList)
               AppLib.CreateLogDetails(_hrsApplicantLogDetailList, "HrsApplicantLogDetail")

            End If


#Region "HrsApplicantDocType"

            If _removeDocTypeCount > 0 Then
               Me.DeleteHrsApplicantDocTypeDetails(_hrsApplicantDocTypeListOld)
               For Each _old As HrsApplicantDocType In _hrsApplicantDocTypeListOld

                  If _old.LogActionId = LogActionId.Delete Then

                     With _logKeyList
                        .Clear()
                        .Add("ApplicantId", applicantId)
                     End With

                     _id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Delete, currentUserId)
                     _hrsApplicantDocTypeLogDetailList.Clear()

                     With _old
                        Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                        AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)

                        AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, .DocTypeReference, String.Empty)

                        AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, .DocTypeFileName.ToString, String.Empty)
                        'AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeGUID, .DocTypeGUID.ToString, String.Empty)

                        Try
                           RemoveApplicantFile(applicantId, .DocTypeFileName, .DocTypeGUID)
                        Catch ex As Exception

                        End Try


                     End With



                     AppLib.CreateLogDetails(_hrsApplicantDocTypeLogDetailList, "HrsApplicantDocTypeLogDetail")

                  End If
               Next

            End If

            If _addDocTypeCount > 0 Then
               Me.InsertHrsApplicantDocTypes(_hrsApplicantDocTypeListNew)

               For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeListNew

                  With _logKeyList
                     .Clear()
                     .Add("ApplicantId", _new.ApplicantId)
                     '.Add("AccountId", _new.AccountId)
                  End With

                  _id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)

                  _hrsApplicantDocTypeLogDetailList.Clear()

                  With _new

                     Dim _docTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                     AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeId, String.Empty, .DocTypeId.ToString, .DocTypeId.ToString + "=" + _docTypeName)

                     AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, String.Empty, .DocTypeReference)
                     AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, String.Empty, .DocTypeFileName)

                     AppLib.CreateLogDetails(_hrsApplicantDocTypeLogDetailList, "HrsApplicantDocTypeLogDetail")

                  End With
               Next

            End If

            If _editDocTypeCount > 0 Then
               Me.UpdateHrsApplicantDocTypes(_hrsApplicantDocTypeList)

               For Each _new As HrsApplicantDocType In _hrsApplicantDocTypeList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("ApplicantId", _new.ApplicantId)
                        '.Add("AccountId", _new.AccountId)
                     End With

                     _id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Edit, currentUserId)

                     _hrsApplicantDocTypeLogDetailList.Clear()

                     For Each _old As HrsApplicantDocType In _hrsApplicantDocTypeListOld
                        If _new.DocTypeDetailId = _old.DocTypeDetailId Then

                           With _new

                              If .DocTypeId <> _old.DocTypeId Then
                                 Dim _oldDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = _old.DocTypeId).DocTypeName
                                 Dim _newDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName

                                 AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeId, .DocTypeId.ToString, _old.DocTypeId.ToString, .DocTypeId.ToString + "=" + _oldDocTypeName + "; " + .DocTypeId.ToString + "=" + _newDocTypeName)
                              End If

                              If .DocTypeReference <> _old.DocTypeReference Then
                                 AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeReference, _old.DocTypeReference, .DocTypeReference)
                              End If

                              If .DocTypeFileName <> _old.DocTypeFileName Then
                                 AppLib.AddLogDetail(_hrsApplicantDocTypeLogDetailList, _id, LogColumnId.DocTypeFileName, _old.DocTypeFileName, .DocTypeFileName)
                              End If

                              If .DocTypeGUID <> _old.DocTypeGUID Then

                                 'Upload File
                                 RemoveApplicantFile(.ApplicantId, _old.DocTypeFileName, _old.DocTypeGUID)
                                 UploadApplicantFile(.ApplicantId, .DocTypeFileName, .DocTypeGUID)
                                 'Upload File

                                 'AppLib.AddLogDetail(_hrsApplicantKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                              End If

                              AppLib.CreateLogDetails(_hrsApplicantDocTypeLogDetailList, "HrsApplicantDocTypeLogDetail")

                           End With

                        End If
                     Next

                  End If

               Next

            End If


#End Region

            If applicant.ApplicantStatusId = ApplicantStatus.Member Then

                    Try
                        File.WriteAllText("D:\1.txt", _arsMemberRequestApplicant.ToString)
                        Try
                            Me.InsertHrsApplicantMember(_hrsApplicantMember)
                        Catch ex As Exception
                            File.WriteAllText("D:\2.txt", ex.Message.ToString)

                        End Try


                        If _hrsApplicantMemberDocTypeList.Count > 0 Then
                            File.WriteAllText("D:\3.txt", _arsMemberRequestApplicant.ToString)

                            Me.InsertHrsApplicantMemberDocTypes(_hrsApplicantMemberDocTypeList)

                            Dim _hrsMemberDocTypeLogDetailList As New SysLogDetailList

                            For Each _new As HrsApplicantMemberDocType In _hrsApplicantMemberDocTypeList

                                With _logKeyList
                                    .Clear()
                                    .Add("MemberId", _memberId)
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
                        _arsMemberRequestApplicant.Name3 = _hrsApplicant.Name3
                        File.WriteAllText("D:\before.txt", _arsMemberRequestApplicant.ToString)
                        Me.InsertArsMemberRequestApplicant(_arsMemberRequestApplicant)
                        File.WriteAllText("D:\after.txt", _arsMemberRequestApplicant.ToString)
                        'Dim _logKeyList As New LogKeyList

                        'Dim _id As Integer
                        Dim _arsMemberRequestApplicantLogDetailList As New SysLogDetailList

                        With _arsMemberRequestApplicant
                            AppLib.AddLogDetail(_arsMemberRequestApplicantLogDetailList, 0, LogColumnId.MemberId, String.Empty, .MemberId.ToString)
                        End With

                        If _arsMemberRequestApplicantLogDetailList.Count > 0 Then
                            With _logKeyList
                                .Clear()
                                .Add("ApplicantDetailId", _arsMemberRequestApplicant.ApplicantDetailId)
                            End With

                            _id = AppLib.CreateLogHeader("InsArsMemberRequestApplicantLog", _logKeyList, LogActionId.Add, currentUserId)
                            AppLib.AssignLogHeaderId(_id, _arsMemberRequestApplicantLogDetailList)
                            AppLib.CreateLogDetails(_arsMemberRequestApplicantLogDetailList, "ArsMemberRequestApplicantLogDetail")

                        End If


                    Catch ex As Exception

                    End Try

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
   Private Sub AddInsertUpdateParamsHrsApplicanMembertDocType(memberDocType As HrsApplicantMemberDocType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberId", memberDocType.MemberId)
         .AddWithValue("@DocTypeId", memberDocType.DocTypeId)
         .AddWithValue("@DocTypeReference", memberDocType.DocTypeReference)
         .AddWithValue("@DocTypeFileName", memberDocType.DocTypeFileName)
         .AddWithValue("@DocTypeGUID", memberDocType.DocTypeGUID)
         .AddWithValue("@FileExtension", Path.GetExtension(memberDocType.DocTypeFileName.Replace("""", "")).ToLowerInvariant)
      End With


   End Sub


   Private Sub InsertHrsApplicantMemberDocTypes(list As HrsApplicantMemberDocTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DocTypeDetailId")     ' auto-assigned by DB
         .Add("FileUrl")
         '.Add("MemberId")
         .Add("ApplicantId")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMemberDocType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsApplicantMemberDocType In list

            Try
               UploadApplicantFile(_detail.MemberId, _detail.DocTypeFileName, _detail.DocTypeGUID)
            Catch ex As Exception

            End Try

            Me.AddInsertUpdateParamsHrsApplicanMembertDocType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub


   <SymAuthorization("RemoveApplicant")>
   <Route("applicants/{applicantId}/{lockId}/{currentUserId}")>
   <HttpDelete>
   Public Function RemoveApplicant(applicantId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

      If applicantId <= 0 Then
         Throw New ArgumentException("Applicant ID is required.")
      End If

      Try

         Dim _currentUserId As Integer = 1      ' System (default)
         If currentUserId > 0 Then
            _currentUserId = currentUserId
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

            Me.DeleteHrsApplicant(applicantId, lockId)

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
   Private Sub LoadHrsApplicant(applicant As ApplicantBody, hrsApplicant As HrsApplicant)

      DataLib.ScatterValues(applicant, hrsApplicant)

   End Sub
   Private Sub LoadHrsApplicant(row As DataRow, applicant As HrsApplicant)

      With applicant
         .ApplicantId = row.ToInt32("ApplicantId")
         .MemberId = row.ToInt32("MemberId")
         .ApplicantLastName = row.ToString("ApplicantLastName")
         .Name3 = row.ToString("Name3")
         .ApplicantFirstName = row.ToString("ApplicantFirstName")
         .ApplicantMiddleName = row.ToString("ApplicantMiddleName")
         .ApplicantSuffixId = row.ToInt32("ApplicantSuffixId")
         .BirthDate = row.ToDate("BirthDate")
         .SexId = row.ToString("SexId")
         .CivilStatusId = row.ToInt32("CivilStatusId")
         .ReligionId = row.ToInt32("ReligionId")

         .Address1 = row.ToString("Address1")
         '.Address2 = row.ToString("Address2")
         .PostalCode = row.ToString("PostalCode")
         .PhoneNumber = row.ToString("PhoneNumber")
         .MobileNumber = row.ToString("MobileNumber")
         .Email = row.ToString("Email")
         .ApplicationSourceId = row.ToInt32("ApplicationSourceId")
         .ApplicationDate = row.ToDate("ApplicationDate")
         .ApplicantStatusId = row.ToInt32("ApplicantStatusId")
         .RegionId = row.ToString("RegionId")
         .ProvinceId = row.ToString("ProvinceId")
         .MunicipalityId = row.ToString("MunicipalityId")
         .BarangayId = row.ToString("BarangayId")
         .MemberRequestId = row.ToInt32("MemberRequestId")
      End With

   End Sub
   Private Sub LoadHrsApplicantDocTypeList(rows As DataRowCollection, list As HrsApplicantDocTypeList)

      Dim _detail As HrsApplicantDocType

      For Each _row As DataRow In rows
         _detail = New HrsApplicantDocType

         With _detail
            .DocTypeDetailId = _row.ToInt32("DocTypeDetailId")
            .DocTypeId = _row.ToInt32("DocTypeId")
            .DocTypeReference = _row.ToString("DocTypeReference")
            .DocTypeFileName = _row.ToString("DocTypeFileName")
            .DocTypeGUID = _row.ToString("DocTypeGUID")
            .FileExtension = _row.ToString("FileExtension")
            .FileUrl = _row.ToString("FileUrl")
         End With

         list.Add(_detail)

      Next

   End Sub

   Private Sub InsertHrsApplicant(applicant As HrsApplicant)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("Age")
            .Add("Name3")
            .Add("MemberTypeId")
            '.Add("Address2")
            .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsApplicant", applicant, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsApplicant(applicant)

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub InsertHrsApplicantMember(applicant As HrsApplicantMember)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("Age")
         .Add("Name3")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsMember", applicant, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsApplicantMember(applicant)

         .ExecuteNonQuery()

      End With

   End Sub


   Private Sub InsertHrsApplicantDocTypes(list As HrsApplicantDocTypeList)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("DocTypeDetailId")     ' auto-assigned by DB
         .Add("FileUrl")
         .Add("LogActionId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.HrsApplicantDocType", list(0), _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsApplicantDocType In list

            Try
               UploadApplicantFile(_detail.ApplicantId, _detail.DocTypeFileName, _detail.DocTypeGUID)
            Catch ex As Exception

            End Try

            Me.AddInsertUpdateParamsHrsApplicantDocType(_detail)
            .ExecuteNonQuery()
         Next

      End With

   End Sub
   Private Sub InsertArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

      Dim _excludedFields As New List(Of String)

      With _excludedFields
         .Add("ApplicantId")
         .Add("LogActionId")
         .Add("ApplicantId")
            .Add("Remarks")
            .Add("RequestDocTypeId")
            .Add("RequestDocTypeReference")
            .Add("RequestDocTypeFileName")
            .Add("RequestDocTypeGUID")
            .Add("FileUrl")
            .Add("FileExtension")
            .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildInsertCommandText("dbo.ArsMemberRequestApplicant", applicant, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsArsMemberRequestApplicant(applicant)

         .ExecuteNonQuery()

      End With

   End Sub

   Private Sub AddInsertUpdateParamsArsMemberRequestApplicant(applicant As ArsMemberRequestApplicant)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicantDetailId", applicant.ApplicantDetailId)
         .AddWithValue("@MemberId", applicant.MemberId)
         .AddWithValue("@Name3", applicant.Name3)
         .AddWithValue("@MemberRequestId", applicant.MemberRequestId)
         .AddWithValue("@ApplicantScreeningId", applicant.ApplicantScreeningId)
         .AddWithValue("@ScreeningStatusId", applicant.ScreeningStatusId)
         .AddWithValue("@ApplicantStatusId", applicant.ApplicantStatusId)
         '.AddWithValue("@Remarks", applicant.Remarks)
         .AddWithValue("@HiredDate", applicant.HiredDate.ToNullable)
         .AddWithValue("@EmploymentStatusId", applicant.EmploymentStatusId)
            .AddWithValue("@DeploymentDate", applicant.DeploymentDate.ToNullable)

        End With

   End Sub

   Private Sub UpdateHrsApplicant(applicant As HrsApplicant)

      Dim _excludedFields As New List(Of String)
      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantId")
         .Add("LockId")
      End With

      With _excludedFields
         .Add("Age")
            .Add("Name3")
            .Add("MemberTypeId")
            '.Add("Address2")
        End With

      With DataCore.Command
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsApplicant", applicant, _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         Me.AddInsertUpdateParamsHrsApplicant(applicant)
         .Parameters.AddWithValue("@LockId", Convert.FromBase64String(applicant.LockId)).DbType = DbType.Binary

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub UpdateHrsApplicantDocTypes(list As HrsApplicantDocTypeList)

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
         .CommandText = DataLib.BuildUpdateCommandText("dbo.HrsApplicantDocType", list(0), _keyFields, _excludedFields)
         .CommandType = CommandType.Text

         For Each _detail As HrsApplicantDocType In list
            If _detail.LogActionId = LogActionId.Edit Then
               Me.AddInsertUpdateParamsHrsApplicantDocType(_detail)
               .Parameters.AddWithValue("@DocTypeDetailId", _detail.DocTypeDetailId)

               .ExecuteNonQuery()
            End If
         Next

      End With

   End Sub

   Private Sub AddInsertUpdateParamsHrsApplicant(applicant As HrsApplicant)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicantId", applicant.ApplicantId)
         .AddWithValue("@MemberId", applicant.MemberId)

         .AddWithValue("@ApplicantLastName", applicant.ApplicantLastName)
         .AddWithValue("@ApplicantFirstName", applicant.ApplicantFirstName)
         .AddWithValue("@ApplicantMiddleName", applicant.ApplicantMiddleName)

         .AddWithValue("@ApplicantSuffixId", applicant.ApplicantSuffixId)
         .AddWithValue("@BirthDate", applicant.BirthDate)

         .AddWithValue("@SexId", applicant.SexId)
         .AddWithValue("@CivilStatusId", applicant.CivilStatusId)
         .AddWithValue("@ReligionId", applicant.ReligionId)
         .AddWithValue("@Address1", applicant.Address1)
         '.AddWithValue("@Address2", applicant.Address2)
         .AddWithValue("@PostalCode", applicant.PostalCode)
         .AddWithValue("@PhoneNumber", applicant.PhoneNumber)
         .AddWithValue("@MobileNumber", applicant.MobileNumber)
         .AddWithValue("@Email", applicant.Email)
         .AddWithValue("@ApplicationSourceId", applicant.ApplicationSourceId)
         .AddWithValue("@ApplicationDate", applicant.ApplicationDate)
         .AddWithValue("@ApplicantStatusId", applicant.ApplicantStatusId)
         .AddWithValue("@RegionId", applicant.RegionId)
         .AddWithValue("@ProvinceId", applicant.ProvinceId)
         .AddWithValue("@MunicipalityId", applicant.MunicipalityId)
         .AddWithValue("@BarangayId", applicant.BarangayId)
         .AddWithValue("@MemberRequestId", applicant.MemberRequestId)

      End With

   End Sub
   Private Sub AddInsertUpdateParamsHrsApplicantMember(applicant As HrsApplicantMember)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@MemberId", applicant.MemberId)
         .AddWithValue("@MemberEmployeeId", "")

         .AddWithValue("@MemberLastName", applicant.MemberLastName)

         .AddWithValue("@MemberFirstName", applicant.MemberFirstName)
         .AddWithValue("@MemberMiddleName", applicant.MemberMiddleName)

         .AddWithValue("@MemberSuffixId", applicant.MemberSuffixId)
         .AddWithValue("@BirthDate", applicant.BirthDate)

         .AddWithValue("@SexId", applicant.SexId)
         .AddWithValue("@CivilStatusId", applicant.CivilStatusId)
         .AddWithValue("@ReligionId", applicant.ReligionId)
         .AddWithValue("@Address1", applicant.Address1)
         '.AddWithValue("@Address2", applicant.Address2)
         .AddWithValue("@PostalCode", applicant.PostalCode)
         .AddWithValue("@PhoneNumber", applicant.PhoneNumber)
         .AddWithValue("@MobileNumber", applicant.MobileNumber)
         .AddWithValue("@Email", applicant.Email)

         .AddWithValue("@RegionId", applicant.RegionId)
         .AddWithValue("@ProvinceId", applicant.ProvinceId)
         .AddWithValue("@MunicipalityId", applicant.MunicipalityId)
         .AddWithValue("@BarangayId", applicant.BarangayId)
            .AddWithValue("@MemberTypeId", applicant.MemberTypeId)

        End With

   End Sub
   Private Sub AddInsertUpdateParamsHrsApplicantDocType(docType As HrsApplicantDocType)

      With DataCore.Command.Parameters
         .Clear()

         .AddWithValue("@ApplicantId", docType.ApplicantId)
         .AddWithValue("@DocTypeId", docType.DocTypeId)
         .AddWithValue("@DocTypeReference", docType.DocTypeReference)
         .AddWithValue("@DocTypeFileName", docType.DocTypeFileName)
         .AddWithValue("@DocTypeGUID", docType.DocTypeGUID)
         .AddWithValue("@FileExtension", Path.GetExtension(docType.DocTypeFileName.Replace("""", "")).ToLowerInvariant)
      End With


   End Sub

   Private Sub DeleteHrsApplicant(applicantId As Integer, lockId As String)

      Dim _keyFields As New List(Of String)

      With _keyFields
         .Add("ApplicantId")
         .Add("LockId")
      End With

      With DataCore.Command
         .CommandText = DataLib.BuildDeleteCommandText("dbo.HrsApplicant", _keyFields)
         .CommandType = CommandType.Text

         With .Parameters
            .Clear()
            .AddWithValue("@ApplicantId", applicantId)
            .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
         End With

         .ExecuteNonQuery()

      End With

   End Sub
   Private Sub DeleteHrsApplicantDocTypeDetails(list As HrsApplicantDocTypeList)

      With DataCore.Command
         .CommandText = "DELETE dbo.HrsApplicantDocType WHERE DocTypeDetailId=@DocTypeDetailId"
         .CommandType = CommandType.Text

         For Each _old As HrsApplicantDocType In list
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

   Friend Shared Function RemoveApplicantFile(applicantId As Integer, fileName As String, guid As String) As Boolean

      Try

         Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
         Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant
         Dim _applicantFolder As String = Path.Combine(_uploadRootPath, applicantId.ToString())

         Dim _fileName As String = Path.Combine(_applicantFolder, guid + _extension)

         If File.Exists(_fileName) Then
            File.Delete(_fileName)
            Return True
         End If

      Catch ex As Exception

      End Try

      Return False

   End Function
   Friend Shared Function UploadApplicantFile(applicantId As Integer, fileName As String, guid As String) As Boolean

      Try

         Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")

         Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
         Dim _applicantFolder As String = Path.Combine(_uploadRootPath, applicantId.ToString())
         Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant


         Dim _fileName As String = Path.Combine(_rootPath, guid + _extension)

         If Not Directory.Exists(_uploadRootPath) Then
            Directory.CreateDirectory(_uploadRootPath)
         End If

         If Not Directory.Exists(_applicantFolder) Then
            Directory.CreateDirectory(_applicantFolder)
         End If

         If File.Exists(_fileName) Then
            File.Move(_fileName, Path.Combine(_applicantFolder, Path.GetFileName(_fileName)))
            File.Delete(_fileName)
            Return True
         End If

      Catch ex As Exception

      End Try

      Return False

   End Function

   <SymAuthorization("UploadApplicantFiles")>
   <Route("applicant/{applicantId}/{currentUserId}/{guid}/files")>
   <HttpPost>
   Public Async Function UploadApplicantFiles(applicantId As Integer, currentUserId As Integer, guid As String, <FromUri> q As UploadDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

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


End Class

Public Class ApplicantBody
   Inherits HrsApplicant
   Public Property Docs As HrsApplicantDocType()


End Class

