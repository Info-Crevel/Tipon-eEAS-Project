// Client Contract

<template>
    <section class="container p-0" :key="ts">
    
        <sym-form id="ars0020" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
    
    
    
            <!-- footer -->
    
            <div slot="footer" class="action-buttons p-1">
    
                <div></div>
    
    
    
                <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
    
                    <button type="button" :class="submitButtonClass" class="justify-between btn-save" @click="onSubmit">
    
            <i class="fa fa-save fa-lg"></i><span>Save</span>
    
          </button>
    
                    <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear">
    
            <i class="fa fa-undo fa-lg"></i><span>Clear</span>
    
          </button>
    
                    <button type="button" :class="deleteButtonClass" class="justify-between btn-delete" @click="onDelete">
    
            <i class="fa fa-times-circle fa-lg"></i><span>Delete</span>
    
          </button>
    
                </div>
    
    
    
                <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
    
                    <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog">
    
            <i class="fa fa-database fa-lg"></i><span>Log</span>
    
          </button>
    
                </div>
    
            </div>
    
    
    
            <div>
    
                <div class="d-grid cols-2 gap fw-100">
    
                    <sym-int v-model="contract.clientContractId" :caption-width="35" :input-width="30" caption="Contract ID" lookupId="ArsClientContract" @lostfocus="onClientContractIdLostFocus" @changing="onClientContractIdChanging" @changed="onClientContractIdChanged"
    
                        @searchfill="onClientContractIdSearchFill" @searchresult="onClientContractIdSearchResult"></sym-int>
    
                    <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    
    
    
                </div>
    
    
    
                <div class="contract-info">
    
                    <div class="app-box-style">
    
                        <div class="text-center border-light curved p-1 app-form-header mb-2 fw-33.33">
    
                            <span class="serif lg-3">INFORMATION</span>
    
                        </div>
    
                        <div class="box-info gap">
    
                            <sym-text v-model="contract.clientContractName" :caption-width="35" caption="Project Name" align="bottom" @changing="onClientContractNameChanging"></sym-text>
    
                            <sym-combo v-model="contract.clientContractStatusId" :caption-width="35" caption="Status" align="bottom" display-field="clientContractStatusName" :datasource="contractStatus"></sym-combo>
    
                            <sym-combo v-model="contract.orgId" :caption-width="35" caption="Cooperative" align="bottom" display-field="orgName" :datasource="org"></sym-combo>
    
                            <sym-combo v-model="contract.clusterId" :caption-width="35" caption="Cluster" align="bottom" display-field="clusterName" :datasource="clusters" @changed="onClusterIdChanged()"></sym-combo>
    
                            <sym-combo v-model="contract.platformId" :caption-width="35" :key="contact.clusterId" caption="Business Platform" align="bottom" display-field="platformName" :datasource="platform"></sym-combo>
    
                            <sym-combo v-model="contract.contractTypeId" :caption-width="35" caption="Contract Type" align="bottom" display-field="contractTypeName" :datasource="contractType"></sym-combo>
    
                        </div>
    
                        <div class="box-info-sub gap">
    
                            <sym-int v-model="contract.clientId" caption="Client ID" align="bottom" lookupId="ArsClient" @changing="onClientIdChanging" @searchresult="onClientIdSearchResult" @changed="onClientIdChanged(contract.clientId)"></sym-int>
    
                            <sym-text v-model="contract.clientName" caption="Client Name" align="bottom"></sym-text>
    
                            <sym-combo v-model="contract.contactDetailId" :caption-width="35" caption="Contact Person" align="bottom" display-field="contactPerson" :datasource="contactPerson" @changed="onContactPersonChanged()"></sym-combo>
    
                            <sym-text v-model="contract.contactPosition" :caption-width="35" caption="Contact Position" align="bottom"></sym-text>
    
                            <sym-text v-model="contract.contactNumber" :caption-width="35" caption="Contact Number" align="bottom"></sym-text>
    
                        </div>
    
                        <div class="box-info-footer gap">
    
                            <sym-text v-model="contract.provinceId" align="bottom" :input-width="36" caption="Province ID" display-field="provinceName" :datasource="provinces" lookupId="HrsProvince" @changing="onProvinceIdChanging" @changed="onProvinceIdChanged" @searchresult="onProvinceIdSearchResult"></sym-text>
    
                            <sym-text v-model="contract.provinceName" align="bottom" :caption-width="45" caption="Province Name"></sym-text>
    
                            <sym-combo v-model="contract.municipalityId" :caption-width="36" caption="Municipality" align="bottom" display-field="municipalityName" :datasource="municipalities"></sym-combo>
    
                        </div>
    
                    </div>
    
                    <div class="container-2 gap">
    
    
    
    
    
                        <div class="app-box-style ">
    
    
    
                            <div class="text-center border-light curved p-1 app-form-header mb-2">
    
                                <span class="serif lg-3">CONTRACT DETAILS</span>
    
                            </div>
    
                            <div class="box-contract gap">
    
                                <sym-text v-model="contract.clientContractCRN" align="bottom" :caption-width="46" caption="Contract CRN" @changing="onClientContractNameChanging"></sym-text>
    
                                <sym-date v-model="contract.contractDate" align="bottom" :caption-width="50" caption="Sign Date" @changing="onContractDateChanging"></sym-date>
    
                                <sym-date v-model="contract.notarizeDate" align="bottom" :caption-width="50" caption="Notary Date" @changing="onNotarizeDateChanging"></sym-date>
    
                                <sym-date v-model="contract.startDate" align="bottom" :caption-width="50" caption="Start Date" @changing="onEndDateChanging"></sym-date>
    
                                <sym-date v-model="contract.endDate" align="bottom" :caption-width="50" caption="End Date" @changing="onEndDateChanging"></sym-date>
    
                            </div>
    
                            <div class="box-contract-sub gap">
    
    
    
                                <sym-text v-model="contract.otherProvisions" align="bottom" :caption-width="46" caption="Other Provisions" @changing="onClientContractNameChanging"></sym-text>
    
                                <sym-text v-model="contract.specialArrangement" align="bottom" :caption-width="46" caption="Special Arrangements" @changing="onClientContractNameChanging"></sym-text>
    
                                <sym-text v-model="contract.penaltyChargeDetails" :caption-width="46" caption="Details of Penalty Charging Due to Losses" align="bottom" @changing="onClientContractNameChanging"></sym-text>
    
                            </div>
    
                            <div class="box-contract-footer gap">
    
                                <sym-check v-model="contract.forceMajeurFlag" :caption-width="30" caption="Force Majeur"></sym-check>
    
                                <sym-check v-model="contract.nonCompeteFlag" :caption-width="46" caption="Non-Compete Clause"></sym-check>
    
                                <sym-check v-model="contract.penaltyChargeFlag" :caption-width="76" caption="Penalty Charge Due to Losses"></sym-check>
    
                                <sym-check v-model="contract.bondProvisionFlag" :caption-width="46" caption="Provision of Bond"></sym-check>
    
                                <sym-check v-model="contract.ndaFlag" :caption-width="56" caption="Non Disclosure Agreement"></sym-check>
    
                            </div>
    
                        </div>
    
    
    
                        <div class="app-box-style ">
    
                            <div class="text-center border-light curved p-1 app-form-header mb-2">
    
                                <span class="serif lg-3"> PAYMENT DETAILS </span>
    
                            </div>
    
                            <div class="box-payment">
    
                                <div class="dummy-box gap">
    
                                    <sym-dec v-model="contract.adminFee" :caption-width="55" caption="Admin Fee (%)"></sym-dec>
    
                                    <sym-int v-model="contract.paymentTerms" :caption-width="50" caption="Terms (days)"></sym-int>
    
                                    <sym-combo v-model="contract.bankId" :caption-width="30" caption="Bank" display-field="bankName" :datasource="banks"></sym-combo>
    
                                    <sym-check v-model="contract.penaltyFlag" :caption-width="76" caption="With Penalty Clause on Late Collection"></sym-check>
    
                                    <sym-check v-model="contract.advancePaymentFlag" :caption-width="76" caption="With Advance Payment from Client"></sym-check>
    
                                </div>
    
    
    
                            </div>
    
    
    
    
    
                        </div>
    
                    </div>
    
    
    
    
    
                    <div class="container-3 gap">
    
                        <div class="app-box-style">
    
                            <div class="text-center border-light curved p-1 app-form-header mb-2">
    
                                <span class="serif lg-3">CHARGING CONSIDERATIONS</span>
    
                            </div>
    
                            <div class="box-charging gap">
    
                                <sym-combo v-model="contract.separationPayId" align="bottom" caption="Separation Pay" display-field="chargingConsiderationName" :datasource="separationPay"></sym-combo>
    
                                <sym-combo v-model="contract.retirementPayId" align="bottom" caption="Retirement Pay" display-field="chargingConsiderationName" :datasource="retirementPay"></sym-combo>
    
                                <sym-combo v-model="contract.thirteenMonthPayId" align="bottom" caption="13th Month Pay" display-field="chargingConsiderationName" :datasource="thirteenMonthPay"></sym-combo>
    
                                <sym-combo v-model="contract.apeChargeId" align="bottom" caption="APE Charge to Client" display-field="chargingConsiderationName" :datasource="apeCharge"></sym-combo>
    
                            </div>
    
                            <div class="box-charging gap">
    
                                <sym-combo v-model="contract.ppeChargeId" align="bottom" caption="PPE Charge to Client" display-field="chargingConsiderationName" :datasource="ppeCharge"></sym-combo>
    
                                <sym-combo v-model="contract.uniformChargeId" align="bottom" caption="Uniform Charge to Client" display-field="chargingConsiderationName" :datasource="uniformCharge"></sym-combo>
    
                                <sym-combo v-model="contract.oshTrainingChargeId" align="bottom" caption="Mandatory OSH Training Charge" display-field="chargingConsiderationName" :datasource="oshTrainingCharge"></sym-combo>
    
                                <sym-combo v-model="contract.oshPersonnelChargeId" align="bottom" caption="OSH Personnel Charge" display-field="chargingConsiderationName" :datasource="oshPersonnelCharge"></sym-combo>
    
                            </div>
    
                        </div>
    
    
    
                        <div class="app-box-style">
    
                            <div class="text-center border-light curved p-1 app-form-header mb-2">
    
                                <span class="serif lg-3"> BILLING ARRANGEMENT </span>
    
                            </div>
    
                            <div class="box-biling gap">
    
                                <sym-combo v-model="contract.separationPayBillingId" align="bottom" caption="Separation Pay Billing" display-field="billingArrangementName" :datasource="separationPayBilling"></sym-combo>
    
                                <sym-combo v-model="contract.retirementPayBillingId" align="bottom" caption="Retirement Pay Billing" display-field="billingArrangementName" :datasource="retirementPayBilling"></sym-combo>
    
                                <sym-combo v-model="contract.thirteenMonthPayBillingId" align="bottom" caption="13th Month Pay Billing" display-field="billingArrangementName" :datasource="thirteenMonthPayBilling"></sym-combo>
    
                            </div>
    
                            <sym-memo v-model="contract.remarks" :caption-width="46" caption="Remarks"></sym-memo>
    
                        </div>
    
                    </div>
    
                </div>
    
            </div>
    
    
    
            <!-- Doc  -->
    
            <div>
    
                <div class="text-left border-light curved p-1 slategray mt-2">
    
                    <span class="serif lg-3">Documents</span>
    
                </div>
    
                <div class="doc-scroller">
    
                    <table class="light striped-even mb-0 doc-tb">
    
                        <thead>
    
                            <tr>
    
                                <th class="w-20">Doc Type</th>
    
                                <th class="w-10">Reference</th>
    
                                <th class="w-5">File Name</th>
    
                                <th class="w-10">Action</th>
    
                            </tr>
    
                        </thead>
    
                        <tbody>
    
                            <tr v-for="(dtl, index) in docs" :key="index">
    
    
    
                                <td>{{ dtl.docTypeName }}</td>
    
                                <td>{{ dtl.docTypeReference }}</td>
    
                                <td>{{ dtl.docTypeFileName }}</td>
    
                                <td class="p-1">
    
                                    <div class="docs-btns" sm-1 mb-0>
    
                                        <div v-if="dtl.docTypeFileName">
    
                                            <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
    
                                        </div>
    
                                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDoc(dtl, index)"> <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
    
                                        <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteDoc(index)"> <i class="fa fa-times fa-lg"></i> </button>
    
                                    </div>
    
                                </td>
    
                            </tr>
    
                        </tbody>
    
                    </table>
    
                </div>
    
    
    
                <div class="command-buttons light border-main p-1 border-top-0 mb-2">
    
                    <div></div>
    
                    <div class="buttons d-flex justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
    
                        <button type="button" class="justify-between btn-add" @click="onAddDoc"> <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
    
                    </div>
    
                </div>
    
            </div>
    
    
    
        </sym-form>
    
    
    
        <sym-modal id="doc-editor" v-model="isDocEditorVisible" size="md" :header="true" :customBody="true" :footer="false" :keyboard="false" :dismissible="false" :closeOnBackButton="false" :title="editorDocTitle" @show="onShowDocEditor($event)" @hide="onHideDocEditor($event)"
    
            headerClass="app-form-header" dismissButtonClass="danger">
    
            <div class="board p-1 mb-0 w-100">
    
                <form id="ars0020D" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    
                    <div class="detail-editor-boxes">
    
                        <sym-combo v-model="doc.docTypeId" caption="Doc Type" align="bottom" display-field="docTypeName" :datasource="docType" @changing="onDocTypeIdChanging" @changed="onDocTypeIdChanged(doc.docTypeId)"></sym-combo>
    
                        <div class="doctype-fields" v-show="doc.docTypeId != 0">
    
                            <div class="MobileContainer">
    
                                <span class="MobileHeader">Reference</span>
    
                                <input type="text" v-model="doc.docTypeReference" @input="validateInput" :maxlength="doc.docTypeLength" placeholder="Enter reference">
    
                            </div>
    
                            <div class="upload-files">
    
                                <sym-tag class="upload-text">{{ fileName }}</sym-tag>
    
                                <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
    
                            </div>
    
                        </div>
    
                    </div>
    
    
    
                    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
    
                        <button type="button" class="info justify-between border-main" @click="onSubmitDoc()">
    
                    <i class="fa fa-check mr-2"></i>Submit</button>
    
                        <button type="button" class="warning justify-between border-main mr-0" @click="isDocEditorVisible = false">
    
                    <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
    
                    </div>
    
                </form>
    
            </div>
    
        </sym-modal>
    
    
    
        <!-- Upload File -->
    
        <sym-upload ref="uploader" class="d-none lams-light border-main curved-1 p-1 shadow-light mb-3" inputClass="lams-light shadow" uploadButtonClass="success shadow" resetButtonClass="danger-light border-main shadow-light" iconClass="fa-files-o fa-fw fa-3x"
    
            :multiple="false" accept=".pdf,.jpg,.png" instructions="Drag documents here<br>or click to browse" uploadButtonText="Click to Upload Now" :blinkUploadIcon="true" @select="onSelectDocuments" @upload="onUploadDocuments" @selectedchanged="onSelectedChanged"
    
            size="sm">
    
        </sym-upload>
    
    
    
    </section>
</template>

<script>
import {
    get,
    upload,
    ajax
} from '../../js/http';

import {
    getCount,
    getList,
    getSafeDeleteFlag
} from '../../js/dbSys';

import PageMaintenance from '../PageMaintenance.vue';
import SymImageSelect from "../../comp/SymImageSelect.vue";
import SymInteger from '../../comp/SymInteger.vue';

export default {
    components: { SymImageSelect, SymInteger },
    extends: PageMaintenance,
    name: 'ars0020',

    data() {
        return {
            contract: {
                clientContractId: 0,
                clientContractStatusId: 0,
                platformId: 0,
                orgId: 0,
                clusterId: 0,
                contractTypeId: 0,
                clientId: 0,
                clientName: '',
                clientContractName: '',
                contactDetailId: 0,
                contactPerson: '',
                contactPosition: '',
                contactNumber: '',
                regionId: '',
                provinceId: '',
                municipalityId: '',
                contractDate: null,
                notarizeDate: null,
                clientContractCRN: '',

                adminFee: 0,
                startDate: null,
                endDate: null,
                paymentTerms: 0,
                bankId: 0,
                penaltyFlag: false,
                advancePaymentFlag: false,
                separationPayId: 0,
                separationPayBillingId: 0,
                retirementPayId: 0,
                retirementPayBillingId: 0,
                thirteenMonthPayId: 0,
                thirteenMonthPayBillingId: 0,
                apeChargeId: 0,
                ppeChargeId: 0,
                uniformChargeId: 0,
                oshTrainingChargeId: 0,
                oshPersonnelChargeId: 0,
                otherProvisions: '',
                specialArrangement: '',
                forceMajeurFlag: false,
                nonCompeteFlag: false,
                penaltyChargeFlag: false,
                penaltyChargeDetails: '',
                bondProvisionFlag: false,
                ndaFlag: false,
                remarks: '',
                lockId: '',
            },
            thirteenMonthPayBilling: [],
            retirementPayBilling: [],
            separationPayBilling: [],
            provinces: [],
            municipalities: [],

            contacts: [],

            docs: [],

            contact: {
                contactDetailId: 0,
                clientId: 0,
                contactPerson: '',
                contactPosition: '',
                contactMobileNumber: '',

                lockId: '',
            },


            doc: {
                contractDocTypeDetailId: 0,
                clientContractId: 0,
                docTypeId: 0,
                docTypeName: "",
                docTypeReference: "",
                docTypeLength: 0,
                docTypeFileName: "",
                docTypeGUID: "",
                fileURL: "",
                lockId: "",
            },

            docIndex: -1,
            isAddingDoc: false,
            isDocEditorVisible: false,

            fileName: "",
            isUploadRequired: false,
            pathFileName: "",
            guidReference: "",

            docsUploadResult: null,
        };
    },

    computed: {


        isValidMobileNumber() {

            let value = "";

            if (this.contract.contactNumber) {

                if (this.contract.contactNumber.length == 0) {
                    value = "default";
                    return value;
                }

            }

            const mobileNumberRegex = /^09\d{09}$/;
            const isValid = mobileNumberRegex.test(this.contract.contactNumber);

            value = isValid ? "valid" : "invalid";
            return value;
        },


        editorDocTitle() {
            if (this.isAddingDoc) {
                return 'Add Doc Type Detail';
            }
            return 'Doc Type Detail';
        },

    },

    methods: {

        onOrgIdChanged() {

            const me = this;
            me.getOrgCluster().then(
                (data) => {
                    me.clusters = data.clusters;
                    me.refresh();
                },
                (fault) => {
                    me.showFault(fault);
                }
            );
        },

        onClusterIdChanged() {
            const me = this;

            me.getOrgClusterPlatform().then(
                (data) => {
                    me.platform = data.platforms;
                    me.refresh();

                    me.contact.platformId = 0;
                },
                (fault) => {
                    me.showFault(fault);
                }
            );
        },


        getTargetPath() {
            const q = {},
                me = this;

            if (me.contract.clientContractId) {
                q.clientContractId = me.contract.clientContractId;
            }

            return {
                name: me.$options.name,
                query: q,
            };
        },

        syncValues(p, q) {
            const me = this;

            if ("clientContractId" in q && me.core.isInteger(q.clientContractId)) {
                me.contract.clientContractId = parseInt(q.clientContractId);
            }
        },

        onProvinceIdChanging(e) {
            e.message = "Province ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
            e.callback = this.provinceIdCallback;
        },

        provinceIdCallback(e) {
            const me = this;
            let filter = "ProvinceId='" + e.proposedValue + "'";
            return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter).then(
                province => {
                    if (province && province.length) {
                        me.contract.provinceName = province[0].provinceName;
                        me.contract.regionId = province[0].regionId;
                        return true;
                    }
                    return false;
                },
                fault => {
                    me.showFault(fault);
                    return false;
                }
            );
        },

        onProvinceIdSearchResult(result) {
            if (!result) { return; }

            const
                data = result[0],
                item = this.contract;

            item.provinceId = data.provinceId;
            item.provinceName = data.provinceName;
            item.regionId = data.regionId;

            this.onProvinceIdChanged()

            this.focusNext();

        },

        onProvinceIdChanged() {
            const me = this,
                wait = me.wait();

            me.municipalities = [];

            me.getMunicipalities().then(
                (data) => {
                    me.stopWait(wait);
                    me.municipalities = data;

                    if (me.municipalities.length) {
                        me.municipalities.unshift({
                            municipalityId: 0,
                            municipalityName: "--- Select Municipality ---",
                        });
                        me.setFocus("municipalityId");
                    }
                },
                (fault) => {
                    me.stopWait(wait);
                    me.showFault(fault);
                }
            );
        },

        showUploadDocumentsResponse(info) {
            if (!info || (!info.createdCount && !info.failedCount)) {
                return Promise.resolve();
            }

            this.docsUploadResult = info;

            setTimeout(() => {
                this.isModalVisible = true;
            }, 200);

            return Promise.resolve();
        },

        onSelectedChanged(fileList) {

            this.selectedFileList = fileList;

            if (fileList.length) {
                const uploader = this.$refs.uploader;
                if (uploader) {
                    uploader.invokeUpload();
                }
            }

        },

        onSelectDocuments() {},


        onUploadDocuments(e) {
            const
                me = this,
                wait = me.wait();

            me.fileName = "";
            me.guidReference = this.guid();

            me.uploadDocuments(e.fileList)
                .then(info => {
                    me.stopWait(wait);

                    e.reset();
                    e.showUploadResponse(info)
                    e.fileList.forEach(file => {
                        me.fileName = file.name;
                    });

                    me.showUploadDocumentsResponse(info)
                        .then(() => {

                        });

                })
                .catch(fault => {
                    me.stopWait(wait);
                    e.reset();
                    me.showFault(fault);
                });

        },

        onSelectFile() {

            const uploader = this.$refs.uploader;

            if (uploader) {
                uploader.invokeClick();
            }
        },

        onDocTypeIdChanging(e) {
            e.message = 'Entry rejected.'
            e.callback = this.docTypeIdCallback;
        },

        docTypeIdCallback(e) {
            const me = this;
            let index = me.docs.findIndex(obj => obj.docTypeId === e.proposedValue);
            if (index > -1) {

                const docTypeName = me.docs[index].docTypeName;
                e.message = 'Doc Type <b>' + docTypeName + '</b> is already in the list.';
                return false;
            }

            return true;
        },

        onDocTypeReferenceChanging(e) {
            e.message = 'Entry rejected.'
            e.callback = this.docTypeReferenceCallback;
        },

        docTypeReferenceCallback(e) {
            const me = this;

            let docTypeReference = e.proposedValue;
            let filter = "docTypeReference='" + docTypeReference + "'";
            return getCount('dbo.ArsClientContractDocType', filter).then(
                count => {
                    if (count > 0) {
                        e.message = '<b>' + docTypeReference + '</b> already exists.';
                        return false;
                    } else {
                        let index = me.docs.findIndex(obj => obj.docTypeReference === docTypeReference);

                        if (index > -1) {
                            e.message = 'Reference <b>' + docTypeReference + '</b> is already in the list.'
                            return false;
                        }

                        return true;
                    }
                },
                () => {
                    return true;
                }
            );


        },

        validateInput() {

            const regex = /^[0-9]*$/;


            if (!regex.test(this.doc.docTypeReference)) {

                this.doc.docTypeReference = this.doc.docTypeReference.slice(0, -1);
            }


            if (this.doc.docTypeReference.length > this.doc.docTypeLength) {
                this.doc.docTypeReference = this.doc.docTypeReference.slice(0, this.doc.docTypeLength); // Trim to max length if exceeded
            }
        },

        onDocTypeIdChanged(newValue) {
            const me = this;

            let selectedDocType = me.docType.find(o => o.docTypeId === newValue);
            if (selectedDocType) {
                me.doc.docTypeLength = (selectedDocType.docTypeLength !== undefined && selectedDocType.docTypeLength !== null) ?
                    selectedDocType.docTypeLength :
                    12;
                if (me.doc.docTypeLength === null) {
                    me.doc.docTypeLength = 12;
                }


                me.isUploadRequired = selectedDocType.uploadRequiredFlag;
                me.doc.docTypeReference = '';
                this.validateInput();
            } else {
                me.isUploadRequired = false;
            }
        },

        onSubmitDoc() {
            const me = this,
                d = me.doc;

            me.isUploadRequired = true;

            if (!d.docTypeId) {
                me.isDocEditorVisible = false;
                return;
            }

            if (me.isUploadRequired) {
                if (!me.fileName || me.fileName.trim() === "") {
                    me.advice.fault('Upload required. Please select a file.', { duration: 5 });
                    return;
                }
            }

            if (!me.isValid("ars0020D")) {
                me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 })
                return;
            }

            let dtl = {};

            if (me.isAddingDoc) {
                Object.assign(dtl, d);

                me.docs.push(dtl);

                me.docType.forEach((docType) => {
                    if (docType.docTypeId == dtl.docTypeId) {

                        dtl.docTypeName = docType.docTypeName;
                    }
                });

                dtl.docTypeFileName = this.fileName;
                dtl.docTypeGUID = this.guidReference;

                me.clearDocPad();
                me.advice.info("Doc Type Name '" + dtl.docTypeName + "' added to list.", {
                    duration: 5,
                });

                me.setFocus("DocTypeId");

            } else {
                dtl = me.docs[me.docIndex];

                me.docType.forEach((doc) => {
                    if (doc.docTypeId == d.docTypeId) {

                        dtl.docTypeName = doc.docTypeName;
                    }
                });

                dtl.contractDocTypeDetailId = d.contractDocTypeDetailId;
                dtl.docTypeId = d.docTypeId;
                dtl.docTypeReference = d.docTypeReference;

                if (this.fileName) {
                    dtl.docTypeFileName = this.fileName;
                } else {
                    dtl.docTypeFileName = d.docTypeFileName;
                }

                if (this.guidReference) {
                    dtl.docTypeGUID = this.guidReference;
                } else {
                    dtl.docTypeGUID = d.docTypeGUID;
                }
                me.isDocEditorVisible = false;
            }
        },

        onShowDocEditor() {
            const me = this;

            me.setActiveModel("doc");

            me.setRequiredMode(
                'docTypeId',
                'docTypeReference'
            );

            setTimeout(() => {
                this.setFocus("docTypeId");
            }, 200);
        },

        onHideDocEditor() {
            const me = this;

            me.isAddingDoc = false;
            me.setActiveModel();
        },

        onEditDoc(dtl, index) {
            const d = this.doc;
            this.docIndex = index;

            d.contractDocTypeDetailId = dtl.contractDocTypeDetailId;
            d.docTypeId = dtl.docTypeId;
            d.docTypeReference = dtl.docTypeReference;
            d.docTypeFileName = dtl.docTypeFileName;
            d.docTypeGUID = dtl.docTypeGUID;
            this.fileName = dtl.docTypeFileName

            this.isDocEditorVisible = true;
        },

        onAddDoc() {
            const me = this;

            me.clearDocPad();
            me.doc.contractDocTypeDetailId = -1;

            me.isDocEditorVisible = true;
            me.isAddingDoc = true;
        },

        onDeleteDoc(index) {
            const me = this;

            me.dialog.confirm("Doc <b>" + me.docs[index].docTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
                .then((reply) => {
                    if (reply === "yes") {
                        me.docs.splice(index, 1);
                    }
                    return;
                });
        },

        clearDocPad() {
            const d = this.doc;

            d.contractDocTypeDetailId = 0;
            d.docTypeId = 0;
            d.docTypeReference = "";
            d.docTypeFileName = "";
            d.docTypeGUID = "";

            this.fileName = "";
            this.guidReference = "";
        },



        loadData() {
            const
                me = this,
                wait = me.wait();

            me.getReferences()
                .then(data => {
                    if (!me.core.isBoolean(data)) {
                        me.contractStatus = data.contractStatus;
                        me.contractType = data.contractType;
                        me.platform = data.platform;
                        me.org = data.org;
                        me.region = data.region;
                        me.client = data.client;
                        me.banks = data.banks;
                        me.separationPay = data.separationPay;
                        me.retirementPay = data.retirementPay;
                        me.thirteenMonthPay = data.thirteenMonthPay;
                        me.apeCharge = data.apeCharge;
                        me.ppeCharge = data.ppeCharge;
                        me.uniformCharge = data.uniformCharge;
                        me.oshTrainingCharge = data.oshTrainingCharge;
                        me.oshPersonnelCharge = data.oshPersonnelCharge;
                        me.separationPayBilling = data.separationPayBilling;
                        me.retirementPayBilling = data.retirementPayBilling;

                        me.thirteenMonthPayBilling = data.thirteenMonthPayBilling;
                        me.contactPersonRef = data.contactPerson;
                        me.docType = data.docType;
                    }
                    if (me.contract.clientContractId < 0) {
                        return Promise.resolve(null);
                    }
                    return me.getClientContract();
                })

                .then((contract) => {

                    me.stopWait(wait);
                    if (contract && contract.clientContract.length) {

                        me.setModels(contract);

                    } else {

                        if (me.contract.clientContractId > -1) {
                            let message =
                                "Contract ID '<b>" + me.contract.clientContractId + "</b>' not found.";

                            me.advice.fault(message, { duration: 5 });
                            me.onReset();
                            return;
                        }
                        if (me.canAdd) {
                            me.advice.warning('You are adding a new document.', { duration: 5 });
                            me.contract.orgId = me.sym.userInfo.userOrgId; //Default User Org
                            me.onOrgIdChanged()

                        } else {
                            let mssg = 'You are not allowed to create new documents.';
                            me.advice.fault(mssg, { duration: 5 });
                            me.onReset();
                            return;
                        }
                    }
                    me.setupControls();
                    me.isFilled = true;
                })
                .catch((fault) => {
                    me.stopWait(wait);
                    me.showFault(fault);
                });
        },

        setModels(info) {


            const me = this,
                contract = info.clientContract[0];

            me.contract = me.core.convertDates(contract);
            me.provinces = info.province;
            me.municipalities = info.municipality;
            me.docs = info.docs;
            me.clusters = info.clusters;

            me.platform = info.platforms;
            me.contactPerson = info.contacts;


            me.refreshOldRefs();
        },

        refreshOldRefs() {
            const me = this;

            me.oldClientContract = JSON.stringify(me.contract);
            me.oldDocs = JSON.stringify(me.docs);
        },

        // API calls

        getFileName(fileName, GUID) {
            return get('api/contracts/download-file/' + this.contract.ClientContractId + '/' + fileName + '/' + GUID);
        },

        guid() {
            return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
                (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
            );
        },

        uploadDocuments(files) {
            let q = this.guid();

            return upload('api/contracts/' + this.contract.clientContractId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
        },

        getClientContract() {
            if (this.contract.clientContractId < 0) {
                return Promise.resolve(null);
            }
            return get("api/contracts/" + this.contract.clientContractId);
        },

        getProvinces() {
            return get("api/contract-provinces");
        },

        getMunicipalities() {
            const me = this;
            return get("api/contract-municipalities/" + me.contract.provinceId);
        },

        getReferences() {
            const me = this;
            if (me.contractType.length) {

                return Promise.resolve(true);
            }
            return get("api/references/ars0020");
        },

        getOrgCluster() {
            return get("api/contract-org-clusters/" + this.contract.orgId);
        },

        getOrgClusterPlatform() {
            return get("api/contract-org-cluster-platforms/" + this.contract.orgId + "/" + this.contract.clusterId);
        },


        createRecord() {
            const
                payload = this.getApiPayload(),
                body = JSON.stringify(payload),
                currentUserId = this.sym.userInfo.userId,
                options = this.core.getAjaxOptions("POST", body);

            return ajax("api/contracts/" + currentUserId, options);
        },

        modifyRecord() {
            const
                payload = this.getApiPayload(),
                body = JSON.stringify(payload),
                currentUserId = this.sym.userInfo.userId,
                options = this.core.getAjaxOptions("PUT", body);

            return ajax("api/contracts/" + this.contract.clientContractId + "/" + currentUserId, options);
        },

        deleteRecord() {
            const
                contract = this.contract,
                currentUserId = this.sym.userInfo.userId,
                options = this.core.getAjaxOptions('DELETE');
            return ajax('api/contracts/' + contract.clientContractId + '/' + contract.lockId + "/" + currentUserId, options);
        },


        getApiPayload() {
            const
                me = this,
                contract = {};

            Object.assign(contract, me.contract);
            contract.contacts = me.contacts;
            contract.docs = me.docs;

            return contract;
        },

        // event handlers

        onLoad() {
            const
                me = this,
                dc = me.dataConfig;

            dc.models.push('contract', 'contacts', 'contact', 'docs', 'doc', );
            dc.keyField = 'clientContractId';
            dc.autoAssignKey = true;
        },

        onResetAfter() {
            this.contacts = [];
            (this.docs = []),
            this.refreshOldRefs();

            setTimeout(() => {
                this.disableElement(
                    'btn-add'
                );
            }, 100);
        },

        onClientContractIdChanging(e) {
            e.callback = this.clientContractIdCallback;
        },

        clientContractIdCallback(e) {
            e.message = "Contract ID '<b>" + e.proposedValue + "</b>' not found.";

            let filter = 'ClientContractId=' + e.proposedValue + ' AND OrgId=' + this.sym.userInfo.userOrgId;
            return getCount('dbo.ArsClientContract', filter).then(
                count => {
                    return count > 0;
                },
                () => {
                    return false;
                }
            );

        },

        onClientContractIdChanged() {
            const me = this;

            me.loadData();
            me.replaceUrl();
        },

        onClientContractIdLostFocus() {
            const me = this;

            if (!me.contract.clientContractId && !me.isModalShown()) {
                me.goTop();
            }
        },

        onClientContractIdSearchFill(e) {
            e.filter = "OrgId = " + this.sym.userInfo.userOrgId
        },

        onClientContractIdSearchResult(result) {
            if (!result) { return; }

            const
                me = this,
                data = result[0];

            me.contract.clientContractId = data.clientContractId;
            me.replaceUrl();
            me.loadData();
        },

        onClientContractNameChanging(e) {
            e.callback = this.clientContractNameCallback;
        },

        clientContractNameCallback(e) {
            e.message = "Contract '<b>" + e.proposedValue + "</b>' already defined.";
            return true;
        },

        onClientIdChanging(e) {
            e.message = "Client ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
            e.callback = this.clientIdCallback;
        },

        clientIdCallback(e) {
            const me = this;
            let filter = "ClientId=" + e.proposedValue;

            return getList('dbo.ArsClient', 'ClientId, ClientName', '', filter).then(
                client => {
                    if (client && client.length) {

                        me.contract.clientName = client[0].clientName;
                        return true;
                    }
                    return false;
                },
                fault => {
                    me.showFault(fault);
                    return false;
                }
            );
        },
        onClientIdChanged(newValue) {
            const me = this;

            me.contactPerson = me.contactPersonRef;
            const originalLength = me.contactPerson.length;
            me.contactPerson = me.contactPerson.filter(obj => obj.clientId === newValue);
            me.refresh();
        },

        onClientIdSearchResult(result) {
            if (!result) { return; }

            const
                data = result[0],
                contract = this.contract;

            contract.clientId = data.clientId;
            contract.clientName = data.clientName;
            contract.contactPerson = data.contactPerson;
            contract.contactPosition = data.contactPosition;
            contract.contactNumber = data.contactMobileNumber;

            this.onClientIdChanged(contract.clientId);

            this.focusNext();

        },

        onContactPersonChanging(e) {
            e.message = 'Entry rejected.'
            e.callback = this.contactPersonCallback;
        },

        contactPersonCallback(e) {
            const me = this;

            let index = me.contacts.findIndex(obj => obj.contactPerson === e.proposedValue);
            if (index > -1) {
                e.message = 'Contact Person <b>' + e.proposedValue + '</b> is already in the list.'
                return false;
            }
            return true;
        },

        onSubmit(nextRoute) {
            const me = this;

            if (!me.isValid(me.$options.name)) {
                me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 });
                return;
            }

            if (!me.hasChanges()) {
                me.advice.success("Document updated.", { duration: 5 });
                me.onReset();
                return;
            }

            let
                promise,
                message,
                wait = me.wait(),
                isNew = me.isNew();

            if (isNew) {
                promise = me.createRecord();
            } else {
                promise = me.modifyRecord();
            }

            promise.then(
                (success) => {
                    me.stopWait(wait);
                    if (success) {
                        if (isNew && typeof success === "number" && success > 0) {
                            me.contract.clientContractId = success;
                        }

                        if (isNew) {
                            message = "New document (created).";
                        } else {
                            message = "Document updated.";
                        }
                        me.setCopyData();

                        if (nextRoute && !(nextRoute instanceof MouseEvent)) {
                            me.dialog.success(message, { size: "md" }).then(
                                () => {
                                    me.refreshOldRefs();
                                    me.go(nextRoute);
                                    return;
                                }
                            );
                        } else {
                            me.advice.success(message, { duration: 5 });
                        }
                    }

                    me.onReset();
                },
                (fault) => {
                    me.stopWait(wait);
                    me.showFault(fault);
                }
            );
        },

        onDelete() {
            const me = this;

            getSafeDeleteFlag('ArsClientContract', me.contract.clientContractId)
                .then(safe => {
                    if (safe) {
                        return me.dialog.confirm('Document will be deleted.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" });
                    } else {
                        me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5 });
                        return '';
                    }
                })
                .then(reply => {
                    if (reply === 'yes') {
                        return me.deleteRecord();
                    }
                    return false;
                })
                .then(success => {
                    if (success) {
                        me.advice.success('Document deleted.', { duration: 4 });
                        me.onReset();
                    }
                })
                .catch(fault => {
                    me.showFault(fault);
                });
        },


        onContractDateChanging(e) {
            e.message = 'Entry rejected.';
            e.callback = this.onContractDateCallback;
        },

        onContractDateCallback(e) {
            const me = this;

            if (e.proposedValue > me.today) {
                e.message = 'Invalid Date <b>[' + e.proposedValue + ']</b>.';
                return false;
            }
            return true;
        },

        onNotarizeDateChanging(e) {
            e.message = 'Entry rejected.';
            e.callback = this.onNotarizeDateCallback;
        },

        onNotarizeDateCallback(e) {
            const me = this;

            if (e.proposedValue > me.today) {
                e.message = 'Notarize Date <b>[' + e.proposedValue + ']</b>.';
                return false;
            }

            return true;
        },

        onStartDateChanging(e) {
            e.message = 'Entry rejected.';
            e.callback = this.onStartDateCallback;
        },

        onStartDateCallback(e) {
            const me = this;

            if (e.proposedValue > me.today) {
                e.message = 'Invalid Start Date <b>[' + e.proposedValue + ']</b>.';
                return false;
            }

            if (me.contract.startDate && e.proposedValue >= me.contract.startDate) {
                e.message = 'Start Date must be earlier than End Date.';
                return false;
            }

            return true;
        },

        onEndDateChanging(e) {
            e.message = 'Entry rejected.';
            e.callback = this.onEndDateCallback;
        },

        onEndDateCallback(e) {
            const me = this;

            if (e.proposedValue > me.today) {
                e.message = 'End Date <b>[' + e.proposedValue + ']</b>.';
                return false;
            }

            if (e.proposedValue <= me.contract.startDate) {
                e.message = 'End Date must be later than Start Date.';
                return false;
            }
            return true;
        },

        // helpers

        setupControls() {
            const me = this;

            setTimeout(() => {
                me.enableElement("btn-add");

                me.setDefaultControlStates();

                me.setRequiredMode(
                    "clientContractName",
                    "contractTypeId",
                    "clientContractStatusId",
                    "contractDate",
                    "notarizeDate",
                    "startDate",
                    "endDate",
                    "clientId",

                );

                me.setDisplayMode(
                    "orgId",
                    "provinceName",
                    "clientName",
                    "contactPosition",
                    "contactNumber"

                );

                me.setFocus("clientContractName");
            }, 100);
        },

        hasChanges() {
            const me = this;

            // 05 Feb 2025 - EMT
            if (!me.isNew() && me.noEditFlag) {
                return false;
            }

            if (JSON.stringify(me.contract) !== me.oldClientContract) { return true; }
            if (JSON.stringify(me.docs) !== me.oldDocs) { return true; }
            return false;
        },

        onContactPersonChanged() {
            const me = this;

            let o = me.contactPerson.find(o => o.contactDetailId == me.contract.contactDetailId);
            if (o) {

                if (o.contactPosition) {
                    me.contract.contactPosition = o.contactPosition
                }

                if (o.contactMobileNumber) {

                    me.contract.contactNumber = o.contactMobileNumber;
                }
            }
        },
        validateMobileNumber() {
            if (!this.contract.contactNumber) {
                this.result = null;
                return;
            }

            if (this.contract.contactNumber.length > 11) {
                this.contract.contactNumber = this.contract.contactNumber.slice(0, 11);
            }

            this.contract.contactNumber = this.contract.contactNumber.replace(/\D/g, "");
        },

    },
    created() {
        const me = this;

        me.oldClientContract = '';
        me.oldContacts = '';
        me.oldDocs = '';
        me.contractStatus = [];
        me.contractType = [];
        me.platform = [];
        me.org = [];
        me.client = [];
        me.contactPerson = [];
        me.region = [];
        me.banks = [];
        me.separationPay = [];
        me.retirementPay = [];
        me.thirteenMonthPay = [];
        me.apeCharge = [];
        me.ppeCharge = [];
        me.uniformCharge = [];
        me.oshTrainingCharge = [];
        me.oshPersonnelCharge = [];
        me.separationPayBilling = [];
        me.retirementPayBilling = [];
        me.thirteenMonthPayBilling = [];
        me.contactPersonRef = [];
        me.docType = [];
        me.clusters = [];

    }

};
</script>

<style scoped>
.contract {
    display: grid;
}

.action-buttons {
    display: grid;
    grid-auto-flow: column;
    grid-template-columns: 1fr 1fr 1fr;
    gap: .75rem;
}

.command-buttons {
    display: grid;
    grid-auto-flow: column;
    grid-template-columns: 1fr 1fr 1fr;
    gap: .75rem;
}

.clientContractid-info {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    width: 100%;
}

#logs tr {
    vertical-align: top;
}

#logs td {
    font-size: 0.875rem;
    padding: 0.375rem;
}

.main-container {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    margin-top: 0.5rem;
}

.container-1 {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.container-2 {
    display: grid;
    grid-template-columns: 1fr 1fr;
}

.container-3 {
    display: grid;
    grid-template-columns: 1fr .6fr;
}

.Header {
    width: 100%;
    border: 0;
    padding: 10px;
    text-align: center;
    font-weight: bold;
    margin-bottom: 0.5rem;
    color: white;
}

.contract-info {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.contact-person {
    display: flex;
    flex-direction: column;
    border: 1px solid rgba(190, 190, 190, 0.918);
    background-color: rgba(219, 215, 215, 0.473);
    padding: 10px;
    border-radius: 10px;
}

.province-field {
    display: grid;
    grid-template-columns: 2fr 5fr;
    width: 100%;
    margin: 0;
    gap: .5rem;
}

.dates {
    display: grid;
    flex-direction: row;
    grid-template-columns: 10fr 10fr;
    width: 100%;
    margin: 0;
    gap: 2rem;
}

.payment-details {
    display: grid;
    flex-direction: row;
    grid-template-columns: 10fr 10fr;
    gap: 2rem;
}

.detail-editor-boxes {
    display: flex;
    flex-direction: column;
}

.upload-files {
    display: flex;
    flex-direction: row;
    gap: .5rem;
}

.upload-text {
    width: 100%;
    margin-left: 0;
    margin-bottom: 0;
}

.docs-btns {
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
}

.box-info {
    display: grid;
    grid-template-columns: 1fr .5fr 1.5fr 1fr 1.5fr 1fr;
}

.box-info-sub {
    display: grid;
    grid-template-columns: .5fr 1.5fr 1fr 1fr 1fr;
}

.box-info-footer {
    display: grid;
    grid-template-columns: .5fr 1.5fr 1.5fr 1fr 1fr 1fr;
}

.box-contract {
    display: grid;
    grid-template-columns: 1fr .5fr .5fr .5fr .5fr;
}

.box-contract-sub {
    display: grid;
    grid-template-columns: 1.5fr 1.5fr 1.5fr;
}

.box-contract-footer {
    display: grid;
    grid-template-columns: .5fr .5fr .5fr .5fr .5fr .5fr;
}

.box-payment {
    display: grid;
    grid-template-rows: 1fr;
}

.dummy-box {
    display: grid;
    grid-template-rows: 1fr 1fr 1fr;
}

.box-charging {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr;
}

.box-biling {
    display: grid;
    grid-template-columns: .5fr .5fr .5fr;
}

.MobileContainer {
    display: flex;
    flex-direction: column;
    margin-bottom: 0.5rem;
}

.MobileHeader {
    border: 1px solid rgb(206, 203, 203);
    background-color: rgb(245, 243, 243);
    border-top-left-radius: 4px;
    border-top-right-radius: 4px;
    width: 100%;
    padding: 0.5rem;
}
</style>
