<RoutePrefix("api")>
Public Class ARS0020_Controller
    Inherits ApiController

    Private ReadOnly _databaseId As String = "sys"

    <SymAuthorization("GetReferences_ARS0020")>
    <Route("references/ars0020")>
    <HttpGet>
    Public Function GetReferences_ARS0020() As IHttpActionResult

        Try
            Using _direct As New SqlDirect("web.ARS0020_References")
                With _direct

                    Using _dataSet As DataSet = _direct.ExecuteDataSet()
                        With _dataSet
                            .Tables(0).TableName = "contractStatus"
                            .Tables(1).TableName = "contractType"
                            .Tables(2).TableName = "platform"
                            .Tables(3).TableName = "org"
                            .Tables(4).TableName = "client"
                            .Tables(5).TableName = "contactPerson"
                            .Tables(6).TableName = "banks"

                            .Tables(7).TableName = "separationPay"
                            .Tables(8).TableName = "retirementPay"
                            .Tables(9).TableName = "thirteenMonthPay"
                            .Tables(10).TableName = "apeCharge"
                            .Tables(11).TableName = "ppeCharge"
                            .Tables(12).TableName = "uniformCharge"
                            .Tables(13).TableName = "oshTrainingCharge"
                            .Tables(14).TableName = "oshPersonnelCharge"
                            .Tables(15).TableName = "separationPayBilling"
                            .Tables(16).TableName = "retirementPayBilling"
                            .Tables(17).TableName = "thirteenMonthPayBilling"
                            .Tables(18).TableName = "docType"

                            '.Tables(8).TableName = "chargingConsideration"
                            '.Tables(17).TableName = "billingArrangement"

                            '.Tables(21).TableName = "province"
                            '.Tables(22).TableName = "municipality"
                            '.Tables(6).TableName = "region"
                        End With

                        Return Me.Ok(_dataSet)

                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

   <SymAuthorization("GetContractClusters")>
   <Route("contract-org-clusters/{orgId}")>
   <HttpGet>
   Public Function GetContractClusters(orgId As Integer) As IHttpActionResult
      If orgId <= 0 Then
         Throw New ArgumentException("Org ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0020_Cluster")
            With _direct
               .AddParameter("orgId", orgId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "clusters"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetContractClusterPlatforms")>
   <Route("contract-org-cluster-platforms/{orgId}/{clusterId}")>
   <HttpGet>
   Public Function GetContractClusterPlatforms(orgId As Integer, clusterId As Integer) As IHttpActionResult
      If clusterId <= 0 Then
         Throw New ArgumentException("Cluster ID is required.")
      End If

      Try
         Using _direct As New SqlDirect("web.ARS0020_Platform")
            With _direct
               .AddParameter("orgId", orgId)
               .AddParameter("clusterId", clusterId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "platforms"
                  End With

                  Return Me.Ok(_dataSet)
               End Using

            End With
         End Using

      Catch _exception As Exception
         Return Me.BadRequest(_exception.Message)
      End Try

   End Function

   <SymAuthorization("GetContractProvinces")>
   <Route("contract-provinces")>
   <HttpGet>
   Public Function GetContractProvinces() As IHttpActionResult
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

   <SymAuthorization("GetContractMunicipalities")>
    <Route("contract-municipalities/{provinceId}")>
    <HttpGet>
    Public Function GetContractMunicipalities(provinceId As Integer) As IHttpActionResult

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

    <SymAuthorization("GetContractClients")>
    <Route("contract-clients")>
    <HttpGet>
    Public Function GetContractClients() As IHttpActionResult
        Dim _filter As String = ""
        Dim _dataSource As String = "dbo.ArsClient"
        Dim _fields As String = "ClientId, ClientName" ', PhoneNumber, MobileNumber"
        Dim _sort As String = "ClientName"

        Try
            Using _table As DataTable = DataLib.GetList(_dataSource, _fields, _filter, _sort)
                Return Me.Ok(_table)
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("GetClientContract")>
    <Route("contracts/{clientContractId}")>
    <HttpGet>
    Public Function GetClientContract(clientContractId As Integer) As IHttpActionResult

        If clientContractId <= 0 Then
            Throw New ArgumentException("Client Contract ID is required.")
        End If

        Try
            Using _direct As New SqlDirect("web.ARS0020")
                With _direct
                    .AddParameter("ClientContractId", clientContractId)

               Using _dataSet As DataSet = _direct.ExecuteDataSet()
                  With _dataSet
                     .Tables(0).TableName = "clientContract"
                     .Tables(1).TableName = "province"
                     .Tables(2).TableName = "municipality"
                     .Tables(3).TableName = "docs"
                     .Tables(4).TableName = "contacts"
                     .Tables(5).TableName = "clusters"
                     .Tables(6).TableName = "platforms"
                  End With

                        Return Me.Ok(_dataSet)
                    End Using

                End With
            End Using

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)
        End Try

    End Function

    <SymAuthorization("CreateClientContract")>
    <Route("contracts/{currentUserId}")>
    <HttpPost>
    Public Function CreateClientContract(currentUserId As Integer, <FromBody> contract As ClientContractBody) As IHttpActionResult

        If contract.ClientContractId <> -1 Then
            Throw New ArgumentException("Client Contract ID is unrecognized.")
        End If

        Try

            '
            ' Assign new ID from sequencer
            '
            Dim _clientContractId As Integer = SysLib.GetNextSequence("ClientContractId")
         contract.ClientContractId = _clientContractId

         '
         ' Load proposed values from payload
         '
         Dim _arsClientContract As New ArsClientContract
            Dim _arsClientContractDocTypeList As New ArsClientContractDocTypeList

            Me.LoadArsClientContract(contract, _arsClientContract)

         ' ArsClientContractDoc
         Try
            For Each _doc As ArsClientContractDocType In contract.Docs
               _doc.ClientContractId = _clientContractId
               _arsClientContractDocTypeList.Add(_doc)
            Next
         Catch ex As Exception
         End Try



         '
         ' Log addition, save to DB
         '

         Dim _logKeyList As New LogKeyList
            'Dim _id As Integer


            Try
                With _arsClientContract

                    .RegionId = GetRegion(contract.ProvinceId.ToString)
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

                Me.InsertArsClientContract(_arsClientContract)

#Region "ArsClientContractDoc"

            If _arsClientContractDocTypeList.Count > 0 Then
               Me.InsertArsClientContractDocTypes(_arsClientContractDocTypeList)
            End If

            For Each _new As ArsClientContractDocType In _arsClientContractDocTypeList

                    With _logKeyList
                        .Clear()
                        .Add("ClientContractId", _new.ClientContractId)
                    End With

                    '_id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)
                    '_arsClientContractDocTypeList.Clear()

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

            Return Me.Ok(contract.ClientContractId)

        Catch _exception As Exception
            Return Me.BadRequest(_exception.Message)

        End Try

    End Function

    <SymAuthorization("ModifyClientContract")>
    <Route("contracts/{clientContractId}/{currentUserId}")>
    <HttpPut>
    Public Function ModifyClientContract(clientContractId As Integer, currentUserId As Integer, <FromBody> contract As ClientContractBody) As IHttpActionResult

        If clientContractId <= 0 Then
            Throw New ArgumentException("Client Contract ID is required.")
        End If

        Try
            '
            ' Load proposed values from payload
            '
            Dim _arsClientContract As New ArsClientContract
            Dim _arsClientContractDocTypeList As New ArsClientContractDocTypeList

            Me.LoadArsClientContract(contract, _arsClientContract)

         For Each _docType As ArsClientContractDocType In contract.Docs
                _arsClientContractDocTypeList.Add(_docType)
            Next

            '
            ' Load old values from DB
            '
            Dim _arsClientContractOld As New ArsClientContract
            Dim _arsClientContractDocTypeListOld As New ArsClientContractDocTypeList

         Dim _result As Results.OkNegotiatedContentResult(Of DataSet) = CType(Me.GetClientContract(clientContractId), Results.OkNegotiatedContentResult(Of DataSet))
            'Try

            Using _dataSet As DataSet = _result.Content
                Dim _row As DataRow = _dataSet.Tables("clientContract").Rows(0)
                Me.LoadArsClientContract(_row, _arsClientContractOld)
            Me.LoadArsClientContractDocTypeList(_dataSet.Tables("docs").Rows, _arsClientContractDocTypeListOld)

         End Using

         '
         ' Apply and log changes, save to DB
         '
         Dim _logKeyList As New LogKeyList
            'Dim _id As Integer


#Region "ArsClientContractDocType"

            'Dim _arsClientContractLogDetailList As New SysLogDetailList

            Dim _removeDocTypeCount As Integer
            Dim _addDocTypeCount As Integer
            Dim _editDocTypeCount As Integer

            ' Mark details for deletion if not found in new list

            For Each _old As ArsClientContractDocType In _arsClientContractDocTypeListOld
                _old.LogActionId = LogActionId.Delete
                For Each _new As ArsClientContractDocType In _arsClientContractDocTypeList
                    If _new.ContractDocTypeDetailId = _old.ContractDocTypeDetailId Then
                        _old.LogActionId = 0  ' retain
                    End If
                Next

                If _old.LogActionId = LogActionId.Delete Then
                    _removeDocTypeCount = _removeDocTypeCount + 1
                End If
            Next

            ' Mark details for addition if not found in old list;
            ' Mark details for modification if found in old list but with at least 1 property mismatch

            For Each _new As ArsClientContractDocType In _arsClientContractDocTypeList
                _new.LogActionId = LogActionId.Add
                For Each _old As ArsClientContractDocType In _arsClientContractDocTypeListOld
                    If _new.ContractDocTypeDetailId = _old.ContractDocTypeDetailId Then
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

            Dim _arsClientContractDocTypeListNew As New ArsClientContractDocTypeList      ' for adding new Trx Details
         Try 'k

            If _addDocTypeCount > 0 Then
               Dim _arsClientContractDocType As ArsClientContractDocType

               For Each _new As ArsClientContractDocType In _arsClientContractDocTypeList
                  If _new.LogActionId = LogActionId.Add Then
                     _arsClientContractDocType = New ArsClientContractDocType
                     _arsClientContractDocTypeListNew.Add(_arsClientContractDocType)
                     DataLib.ScatterValues(_new, _arsClientContractDocType)
                     _arsClientContractDocType.ClientContractId = contract.ClientContractId
                  End If
               Next

            End If
         Catch ex As Exception
         End Try
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
            Me.UpdateArsClientContract(_arsClientContract)

#Region "ArsClientContractDocType"

            If _removeDocTypeCount > 0 Then
                    Me.DeleteArsClientContractDocTypeDetails(_arsClientContractDocTypeListOld)

               For Each _old As ArsClientContractDocType In _arsClientContractDocTypeListOld

                        'If _old.LogActionId = LogActionId.Delete Then

                        With _logKeyList
                            .Clear()
                            .Add("ClientContractId", clientContractId)
                        End With

                        '_id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Delete, currentUserId)
                        ' _arsClientContractDocTypeLogDetailList.Clear()
                        With _old
                     Try
                        RemoveClientContractFile(clientContractId, .DocTypeFileName, .DocTypeGUID)
                     Catch ex As Exception
                     End Try
                  End With

                        'End If
                    Next

                End If

                If _addDocTypeCount > 0 Then
                    Me.InsertArsClientContractDocTypes(_arsClientContractDocTypeListNew)
               For Each _new As ArsClientContractDocType In _arsClientContractDocTypeListNew

                  With _logKeyList
                     .Clear()
                     .Add("ClientContractId", _new.ClientContractId)
                     '.Add("AccountId", _new.AccountId)
                  End With

                  '_id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Add, currentUserId)
                  '_hrsApplicantDocTypeLogDetailList.Clear()

               Next

            End If

                If _editDocTypeCount > 0 Then
                    Me.UpdateArsClientContractDocTypes(_arsClientContractDocTypeList)

               For Each _new As ArsClientContractDocType In _arsClientContractDocTypeList
                  If _new.LogActionId = LogActionId.Edit Then

                     With _logKeyList
                        .Clear()
                        .Add("ClientContractId", _new.ClientContractId)
                        '.Add("AccountId", _new.AccountId)
                     End With

                     '_id = AppLib.CreateLogHeader("InsHrsApplicantDocTypeLog", _logKeyList, LogActionId.Edit, currentUserId)
                     '_hrsApplicantDocTypeLogDetailList.Clear()

                     For Each _old As ArsClientContractDocType In _arsClientContractDocTypeListOld
                        If _new.ContractDocTypeDetailId = _old.ContractDocTypeDetailId Then

                           With _new

                              'If .DocTypeId <> _old.DocTypeId Then
                              '   Dim _oldDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = _old.DocTypeId).DocTypeName
                              '   Dim _newDocTypeName As String = EasSession.DbsDocType.Rows.Find(Function(m) m.DocTypeId = .DocTypeId).DocTypeName
                              'End If
                              If .DocTypeGUID <> _old.DocTypeGUID Then

                                 'Upload File
                                 RemoveClientContractFile(.ClientContractId, _old.DocTypeFileName, _old.DocTypeGUID)
                                 UploadClientContractFile(.ClientContractId, .DocTypeFileName, .DocTypeGUID)
                                 'Upload File

                                 'AppLib.AddLogDetail(_hrsApplicantKinLogDetailList, _id, LogColumnId.KinGUID, _old.KinGUID, .KinGUID)
                              End If

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

    <SymAuthorization("RemoveClientContract")>
    <Route("contracts/{clientContractId}/{lockId}/{currentUserId}")>
    <HttpDelete>
    Public Function RemoveClientContract(clientContractId As Integer, lockId As String, currentUserId As Integer) As IHttpActionResult

        If clientContractId <= 0 Then
            Throw New ArgumentException("Client Contract ID is required.")
        End If

        Try

            'Dim _currentUserId As Integer = 1      ' System (default)
            'If currentUserId > 0 Then
            '   _currentUserId = currentUserId
            'End If

            Dim _transaction As SqlTransaction = Nothing
            Dim _successFlag As Boolean

            Try
                If Not DataCore.Connect(_databaseId) Then
                    ' TODO: Cannot connect
                End If

                _transaction = DataCore.Connection.BeginTransaction()
                DataCore.Command.Transaction = _transaction
                DataCore.Command.Connection = DataCore.Connection

                Me.DeleteArsClientContract(clientContractId, lockId)

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
    Private Sub LoadArsClientContract(contract As ClientContractBody, arsClientContract As ArsClientContract)

        DataLib.ScatterValues(contract, arsClientContract)

    End Sub
    Private Sub LoadArsClientContract(row As DataRow, contract As ArsClientContract)

        With contract
            .ClientContractId = row.ToInt32("ClientContractId")
            .ClientContractStatusId = row.ToInt32("ClientContractStatusId")
            .PlatformId = row.ToInt32("PlatformId")
         .OrgId = row.ToInt32("OrgId")
         .ClusterId = row.ToInt32("ClusterId")
         .ContractTypeId = row.ToInt32("ContractTypeId")

            .ClientId = row.ToInt32("ClientId")
            .ClientName = row.ToString("ClientName")
            .ClientContractName = row.ToString("ClientContractName")
            .ContactDetailId = row.ToInt32("ContactDetailId")
            '.ContactPerson = row.ToString("ContactPerson")
            '.ContactPosition = row.ToString("ContactPosition")
            '.ContactNumber = row.ToString("ContactNumber")

            .RegionId = row.ToString("RegionId")
            .ProvinceId = row.ToString("ProvinceId")
            .MunicipalityId = row.ToString("MunicipalityId")
            .ContractDate = row.ToDate("ContractDate")
            .NotarizeDate = row.ToDate("NotarizeDate")
            .ClientContractCRN = row.ToString("ClientContractCRN")
            .AdminFee = row.ToDecimal("AdminFee")
            .StartDate = row.ToDate("StartDate")
            .EndDate = row.ToDate("EndDate")
            .PaymentTerms = row.ToInt32("PaymentTerms")
            .BankId = row.ToInt32("BankId")
            .PenaltyFlag = row.ToBoolean("PenaltyFlag")
            .AdvancePaymentFlag = row.ToBoolean("AdvancePaymentFlag")

            .SeparationPayId = row.ToInt32("SeparationPayID")
            .SeparationPayBillingId = row.ToInt32("SeparationPayBillingId")
            .RetirementPayId = row.ToInt32("RetirementPayId")
            .RetirementPayBillingId = row.ToInt32("RetirementPayBillingId")
            .ThirteenMonthPayId = row.ToInt32("ThirteenMonthPayId")
            .ThirteenMonthPayBillingId = row.ToInt32("ThirteenMonthPayBillingId")
            .ApeChargeId = row.ToInt32("ApeChargeId")
            .PpeChargeId = row.ToInt32("PpeChargeId")
            .UniformChargeId = row.ToInt32("UniformChargeId")
            .OshTrainingChargeId = row.ToInt32("OshTrainingChargeId")
            .OshPersonnelChargeId = row.ToInt32("OshPersonnelChargeId")
            .OtherProvisions = row.ToString("OtherProvisions")
            .SpecialArrangement = row.ToString("SpecialArrangement")

            .ForceMajeurFlag = row.ToBoolean("ForceMajeurFlag")
            .NonCompeteFlag = row.ToBoolean("NonCompeteFlag")
            .PenaltyChargeFlag = row.ToBoolean("PenaltyChargeFlag")
            .PenaltyChargeDetails = row.ToString("PenaltyChargeDetails")
            .BondProvisionFlag = row.ToBoolean("BondProvisionFlag")
            .NdaFlag = row.ToBoolean("NdaFlag")
            .Remarks = row.ToString("Remarks")

        End With

    End Sub

    Private Sub LoadArsClientContractDocTypeList(rows As DataRowCollection, list As ArsClientContractDocTypeList)

        Dim _detail As ArsClientContractDocType

        For Each _row As DataRow In rows
            _detail = New ArsClientContractDocType

            With _detail
                .ContractDocTypeDetailId = _row.ToInt32("ContractDocTypeDetailId")
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
    Private Sub InsertArsClientContract(contract As ArsClientContract)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientContract", contract, _excludedFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClientContract(contract)

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub InsertArsClientContractDocTypes(list As ArsClientContractDocTypeList)

        Dim _excludedFields As New List(Of String)

        With _excludedFields
            .Add("ContractDocTypeDetailId")     ' auto-assigned by DB
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildInsertCommandText("dbo.ArsClientContractDocType", list(0), _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientContractDocType In list

                Try
                    UploadClientContractFile(_detail.ClientContractId, _detail.DocTypeFileName, _detail.DocTypeGUID)
                Catch ex As Exception

                End Try

                Me.AddInsertUpdateParamsArsClientContractDocType(_detail)
                .ExecuteNonQuery()
            Next

        End With

    End Sub
    Private Sub UpdateArsClientContract(contract As ArsClientContract)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientContractId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientContract", contract, _keyFields)
            .CommandType = CommandType.Text

            Me.AddInsertUpdateParamsArsClientContract(contract)
            .Parameters.AddWithValue("@LockId", Convert.FromBase64String(contract.LockId)).DbType = DbType.Binary

            .ExecuteNonQuery()

        End With

    End Sub
    Private Sub UpdateArsClientContractDocTypes(list As ArsClientContractDocTypeList)

        Dim _keyFields As New List(Of String)
        Dim _excludedFields As New List(Of String)

        With _keyFields
            .Add("ContractDocTypeDetailId")
        End With

        With _excludedFields
            .Add("FileUrl")
            .Add("LogActionId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildUpdateCommandText("dbo.ArsClientContractDocType", list(0), _keyFields, _excludedFields)
            .CommandType = CommandType.Text

            For Each _detail As ArsClientContractDocType In list
                If _detail.LogActionId = LogActionId.Edit Then
                    Me.AddInsertUpdateParamsArsClientContractDocType(_detail)
                    .Parameters.AddWithValue("@ContractDocTypeDetailId", _detail.ContractDocTypeDetailId)

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub

    Private Sub DeleteArsClientContract(clientContractId As Integer, lockId As String)

        Dim _keyFields As New List(Of String)

        With _keyFields
            .Add("ClientContractId")
            .Add("LockId")
        End With

        With DataCore.Command
            .CommandText = DataLib.BuildDeleteCommandText("dbo.ArsClientContract", _keyFields)
            .CommandType = CommandType.Text

            With .Parameters
                .Clear()
                .AddWithValue("@ClientContractId", clientContractId)
                .AddWithValue("@LockId", Convert.FromBase64String(lockId)).DbType = DbType.Binary
            End With

            .ExecuteNonQuery()

        End With
    End Sub

    Private Sub AddInsertUpdateParamsArsClientContract(contract As ArsClientContract)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientContractId", contract.ClientContractId)
            .AddWithValue("@ClientContractStatusId", contract.ClientContractStatusId)
            .AddWithValue("@PlatformId", contract.PlatformId)
         .AddWithValue("@OrgId", contract.OrgId)
         .AddWithValue("@ClusterId", contract.ClusterId)
         .AddWithValue("@ContractTypeId", contract.ContractTypeId)
            .AddWithValue("@ClientId", contract.ClientId)
            .AddWithValue("@ClientName", contract.ClientName)
            .AddWithValue("@ClientContractName", contract.ClientContractName)
            .AddWithValue("@ContactDetailId", contract.ContactDetailId)
            .AddWithValue("@ContactPerson", contract.ContactPerson.ToNullable)
            .AddWithValue("@ContactPosition", contract.ContactPosition.ToNullable)
            .AddWithValue("@ContactNumber", contract.ContactNumber.ToNullable)

            .AddWithValue("@RegionId", contract.RegionId)
            .AddWithValue("@ProvinceId", contract.ProvinceId)
            .AddWithValue("@MunicipalityId", contract.MunicipalityId)

            .AddWithValue("@ContractDate", contract.ContractDate.ToNullable)
            .AddWithValue("@NotarizeDate", contract.NotarizeDate.ToNullable)
            .AddWithValue("@ClientContractCRN", contract.ClientContractCRN)
            .AddWithValue("@AdminFee", contract.AdminFee)
            .AddWithValue("@StartDate", contract.StartDate.ToNullable)
            .AddWithValue("@EndDate", contract.EndDate.ToNullable)
            .AddWithValue("@PaymentTerms", contract.PaymentTerms)
            .AddWithValue("@BankId", contract.BankId)
            .AddWithValue("@PenaltyFlag", contract.PenaltyFlag)
            .AddWithValue("@AdvancePaymentFlag", contract.AdvancePaymentFlag)

            .AddWithValue("@SeparationPayId", contract.SeparationPayId)
            .AddWithValue("@SeparationPayBillingId", contract.SeparationPayBillingId)
            .AddWithValue("@RetirementPayId", contract.RetirementPayId)
            .AddWithValue("@RetirementPayBillingId", contract.RetirementPayBillingId)
            .AddWithValue("@ThirteenMonthPayId", contract.ThirteenMonthPayId)
            .AddWithValue("@ThirteenMonthPayBillingId", contract.ThirteenMonthPayBillingId)
            .AddWithValue("@ApeChargeId", contract.ApeChargeId)
            .AddWithValue("@PpeChargeId", contract.PpeChargeId)
            .AddWithValue("@UniformChargeId", contract.UniformChargeId)
            .AddWithValue("@OshTrainingChargeId", contract.OshTrainingChargeId)
            .AddWithValue("@OshPersonnelChargeId", contract.OshPersonnelChargeId)

            .AddWithValue("@OtherProvisions", contract.OtherProvisions)
            .AddWithValue("@SpecialArrangement", contract.SpecialArrangement)
            .AddWithValue("@ForceMajeurFlag", contract.ForceMajeurFlag)
            .AddWithValue("@NonCompeteFlag", contract.NonCompeteFlag)
            .AddWithValue("@PenaltyChargeFlag", contract.PenaltyChargeFlag)
            .AddWithValue("@PenaltyChargeDetails", contract.PenaltyChargeDetails)
            .AddWithValue("@BondProvisionFlag", contract.BondProvisionFlag)
            .AddWithValue("@NdaFlag", contract.NdaFlag)
            .AddWithValue("@Remarks", contract.Remarks)

        End With

    End Sub

    Private Sub AddInsertUpdateParamsArsClientContractDocType(docType As ArsClientContractDocType)

        With DataCore.Command.Parameters
            .Clear()

            .AddWithValue("@ClientContractId", docType.ClientContractId)
            .AddWithValue("@DocTypeId", docType.DocTypeId)
            .AddWithValue("@DocTypeReference", docType.DocTypeReference)
            .AddWithValue("@DocTypeFileName", docType.DocTypeFileName)
            .AddWithValue("@DocTypeGUID", docType.DocTypeGUID)
            .AddWithValue("@FileExtension", Path.GetExtension(docType.DocTypeFileName.Replace("""", "")).ToLowerInvariant)
        End With

    End Sub

    Private Sub DeleteArsClientContractDocTypeDetails(list As ArsClientContractDocTypeList)

        With DataCore.Command
            .CommandText = "DELETE dbo.ArsClientContractDocType WHERE ContractDocTypeDetailId=@ContractDocTypeDetailId"
            .CommandType = CommandType.Text

            For Each _old As ArsClientContractDocType In list
                If _old.LogActionId = LogActionId.Delete Then
                    With .Parameters
                        .Clear()
                        .AddWithValue("@ContractDocTypeDetailId", _old.ContractDocTypeDetailId)
                    End With

                    .ExecuteNonQuery()
                End If
            Next

        End With

    End Sub
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

    Friend Shared Function RemoveClientContractFile(clientContractId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant
            Dim _clientContractFolder As String = Path.Combine(_uploadRootPath, clientContractId.ToString())

            Dim _fileName As String = Path.Combine(_clientContractFolder, guid + _extension)

            If File.Exists(_fileName) Then
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function
    Friend Shared Function UploadClientContractFile(clientContractId As Integer, fileName As String, guid As String) As Boolean

        Try

            Dim _rootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/temp")

            Dim _uploadRootPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/upload")
            Dim _clientContractFolder As String = Path.Combine(_uploadRootPath, clientContractId.ToString())
            Dim _extension As String = Path.GetExtension(fileName).ToLowerInvariant


            Dim _fileName As String = Path.Combine(_rootPath, guid + _extension)

            If Not Directory.Exists(_uploadRootPath) Then
                Directory.CreateDirectory(_uploadRootPath)
            End If

            If Not Directory.Exists(_clientContractFolder) Then
                Directory.CreateDirectory(_clientContractFolder)
            End If

            If File.Exists(_fileName) Then
                File.Move(_fileName, Path.Combine(_clientContractFolder, Path.GetFileName(_fileName)))
                File.Delete(_fileName)
                Return True
            End If

        Catch ex As Exception

        End Try

        Return False

    End Function

    <SymAuthorization("UploadClientContractFiles")>
    <Route("contracts/{clientContractId}/{currentUserId}/{guid}/files")>
    <HttpPost>
    Public Async Function UploadClientContractFiles(clientContractId As Integer, currentUserId As Integer, guid As String, <FromUri> q As UploadDocumentsQuery) As Threading.Tasks.Task(Of IHttpActionResult)

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

Public Class ClientContractBody
    Inherits ArsClientContract

    Public Property Docs As ArsClientContractDocType()

End Class
