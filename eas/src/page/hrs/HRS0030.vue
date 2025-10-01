// Applicant Profile

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="hrs0030"
      :caption="pageName"
      cardClass="curved-0"
      headerClass="app-form-header"
      footerClass="border-top-main frs-form-footer darken-2 py-0"
      bodyClass="frs-form-body pb-3 "
    >

      <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onSetCanceled" v-if="canSetCanceled && !isCancelled">
          <i class="fa fa-thumbs-o-down fa-lg"></i><span>Cancel</span>
        </button>

        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit" v-show="!isPoolingFlag"> <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear" v-show="!isPoolingFlag"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(16)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="onAccept" v-show="isPoolingFlag"> <i class="fa fa-check mr-2" ></i><span>Accept</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      </div>
      </div>


      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="applicant.applicantId" :caption-width="35" caption="Applicant ID" lookupId="HrsApplicant" @lostfocus="onApplicantIdLostFocus" @changing="onApplicantIdChanging" @changed="onApplicantIdChanged" @searchresult="onApplicantIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
          <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
          <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActive">Active</sym-tag>
          <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isMember">Member</sym-tag>
        </div>
          

      </div>
        <div class="box-field">

            <div class="app-box-style">
              <div class="box-1 gap">
              <sym-text v-model="applicant.applicantLastName" align="bottom" :caption-width="35" caption="Last Name" @changing="onApplicantLastNameChanging"> </sym-text>
              <sym-text v-model="applicant.applicantFirstName" align="bottom" :caption-width="35" caption="First Name" @changing="onApplicantFirstNameChanging"></sym-text>
              <sym-text v-model="applicant.applicantMiddleName" align="bottom" :caption-width="35" caption="Middle Name" @changing="onApplicantMiddleNameChanging"></sym-text>
              <sym-combo v-model="applicant.applicantSuffixId" align="bottom" :caption-width="46" caption="Suffix" display-field="memberSuffixName" :datasource="suffix"></sym-combo>
              <sym-combo v-model="applicant.sexId" align="bottom" :caption-width="46" caption="Sex" display-field="sexName" :datasource="sex"></sym-combo>
              <sym-date v-model="applicant.birthDate" align="bottom" :caption-width="35" caption="Birth Date"  @changing="onBirthDateChanging" @changed="onBirthDateChanged" @keyup.enter="formatDate" ></sym-date>
              <sym-int v-model="applicant.age" align="bottom" :caption-width="35" caption="Age" class="text-left"></sym-int>
            </div>
            <div class="box-2 gap"> 
              <sym-text v-model="applicant.provinceId" align="bottom" :caption-width="34" caption="Province ID" display-field="provinceName" :datasource="provinces" lookupId="HrsProvince" @changing="onProvinceIdChanging" @changed="onProvinceIdChanged" @searchresult="onProvinceIdSearchResult"></sym-text> 
              <sym-text v-model="applicant.provinceName"  align="bottom" :caption-width="34" caption="Province Name"></sym-text>
              <sym-combo v-model="applicant.municipalityId" align="bottom" :caption-width="45" caption="Municipality" display-field="municipalityName" :datasource="municipalities" @changed="onMunicipalityIdChanged()" ></sym-combo>
              <sym-combo v-model="applicant.barangayId" align="bottom" :caption-width="45" caption="Barangay" display-field="barangayName" :datasource="barangays" @changed="onBarangayIdChanged(applicant.barangayId)"></sym-combo>
              <div class="MobileContainer">
              <span class="MobileHeader">Postal Code</span> 
              <input class="MobilePhone"  v-model="applicant.postalCode" @input="validatePostalCode" type="mobile"  :disabled="applicant.applicantId ===0"   />
              </div>
              <sym-text  v-model="applicant.address1" align="bottom" :caption-width="35" caption="Address (House/Lot No, Building/Subdivision/Village Name, Street)" ></sym-text>
            </div>
            <div class="box-3 gap">
              <sym-combo v-model="applicant.religionId" align="bottom" :caption-width="46" caption="Religion" display-field="religionName" :datasource="religion" ></sym-combo>
              <sym-combo v-model="applicant.civilStatusId" align="bottom" :caption-width="46" caption="Civil Status" display-field="civilStatusName" :datasource="civilStatus" ></sym-combo>
              <div class="mobile-fields">
                <div class="MobileContainer">
                  <span class="MobileHeader">Phone Number</span> 
                  <input class="MobilePhone"  v-model="applicant.phoneNumber" @input="validatePhoneNumber"  type="mobile"  />
                </div>
                <div class="MobileContainer">
                  <span class="MobileHeader">Mobile Number</span> 
                  <input class="MobilePhone"  v-model="applicant.mobileNumber" @input="validateMobileNumber" placeholder="(09***)******" type="mobile"   :style="{ 
                  borderColor: isValidMobileNumber == 'valid' ? 'green' : isValidMobileNumber == 'invalid' ? '' : '', }" />
                </div>
                
              </div>
              <sym-text v-model="applicant.email" align="bottom" :caption-width="35" caption="Email" @changing="onEmailChanging"></sym-text>
              <sym-combo v-model="applicant.applicationSourceId" align="bottom" :caption-width="90" caption="Application Source" display-field="applicationSourceName" :datasource="sources" ></sym-combo>   
              <sym-date v-model="applicant.applicationDate" align="bottom" :caption-width="35" caption="Application Date"></sym-date>      
            </div>
            <div class="box-4 gap">
              <sym-int v-model="applicant.memberRequestId" align="bottom" :caption-width="34" caption="MRF #" display-field="memberRequestName" :datasource="memberRequest" lookupId="ArsMemberRequestActive" @changing="onMemberRequestIdChanging" @searchresult="onMemberRequestIdSearchResult"></sym-int> 
              <sym-text v-model="applicant.memberRequestName" align="bottom" :caption-width="10" caption="Member Request Name" ></sym-text>
            </div>

        </div>
      </div>
        <!-- Doc -->
          <div>
            <div class="text-center border-light curved p-1 app-blue-alt mt-2">
              <span class="serif lg-3 center">Documents</span>
            </div>
            <div class="doc-scroller">
              <table class="light striped-even mb-0 doc-tb">
                <thead>
                  <tr>
                    <th class="w-25">Doc Type</th>
                    <th class="w-20">Reference</th>
                    <th class="w-40">File Name</th>
                    <th class="w-15 text-center">Action</th>
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
                           <button class="button info sm-4 ml-2" @click="openDocPreview(dtl.fileUrl, dtl.docTypeFileName)"><i class="fa fa-image mr-2"></i><span class="button-caption ml-2">Preview</span>  </button>
                        </div>
                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDoc(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button" class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteDoc(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons d-flex justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddDoc" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(7)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

    </sym-form>

<!-- logs     -->
<sym-modal 
        v-model="isLogVisible"
        size="xl"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="true"
        :dismissible="true"
        :closeOnBackButton="false"
        title="Applicant Profile Change Log"
        headerClass="frs-form-header"
        dismissButtonClass="danger"
      >
        <div class="fixed-header">
          <table id="logs" class="striped-odd">
            <thead>
              <tr>
                <th class="w-7">Action</th>
                <th class="w-15">Column</th>
                <th class="w-18">Old Value</th>
                <th class="w-18">New Value</th>
                <th class="w-15">Log Date/Time</th>
                <th class="w-10">User</th>
                <th class="w-22">Reference</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="(log, index) in logs" :key="index">
                <td>{{ log.logDescription }}</td>
                <td>{{ log.columnCaption }}</td>
                <td>{{ log.oldValue }}</td>
                <td>{{ log.newValue }}</td>
                <td>
                  {{
                    core.toDateFormat(log.logDateTime, true, "MM/dd/yyyy h:mm tt")
                  }}
                </td>
                <td>{{ log.logUserInfo }}</td>
                <td>{{ log.logReference }}</td>
              </tr>
            </tbody>
          </table>
        </div>
  </sym-modal>
<!-- logs     -->

  <!-- Doc  -->
  <sym-modal
        id="doc-editor"
        v-model="isDocEditorVisible"
        size="md"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="false"
        :dismissible="false"
        :closeOnBackButton="false"
        :title="editorDocTitle"
        @show="onShowDocEditor($event)"
        @hide="onHideDocEditor($event)"
          headerClass="app-form-header"
        dismissButtonClass="danger"
      >
        <div class="board p-1 mb-0 w-100">
          <form id="hrs0010D" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
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

              <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
                <button type="button"  class="info justify-between border-main" @click="onSubmitDoc()">
                <i class="fa fa-check mr-2"></i>Submit</button>
              <button  type="button" class="warning justify-between border-main mr-0" @click="isDocEditorVisible = false" >
                <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
            </div>
          </form>
        </div>
  </sym-modal>

   <!-- Image  -->
  <sym-modal
        id="doc-editor"
        v-model="isDocPreviewVisible"
        size="lg"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="false"
        :dismissible="false"
        :closeOnBackButton="false"
        :title="editorDocPreviewTitle"
        headerClass="app-form-header"
        dismissButtonClass="danger"
      >
        <div class="board p-1 mb-0 w-100" @click.self="closeDocPreview">
          <form id="hrs0010E" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
            <div class="modal-content detail-editor-boxes">
        <div class="modal-content detail-editor-boxes">
          <template v-if="isPDF">
            <iframe :src="previewFileUrl" width="100%" height="600px"></iframe>
          </template>

          <template v-else>
            <div class="image-container">
              <img :src="previewFileUrl" :style="{ transform: 'scale(' + imageZoom + ')' }" />
            </div>
          </template>
        </div>



            </div>

            <div class="buttons w-100 d-flex gap justify-end mt-3 mb-2 fw-26 shadow-soft mb-0"> 
              <div v-if="previewFileName">
                <a  class="justify-between border-main button success sm-4" :href="previewFileUrl"  :download="previewFileName">
                  <i class="fa fa-download fa-lg mr-2"></i> <span class="button-caption ml-2">Download</span>
                </a>
              </div>      
              <button  type="button" class="warning justify-between border-main mr-0" @click="isDocPreviewVisible = false" >
                <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
            </div>
          </form>
        </div>
      </sym-modal>
 

  <!-- Upload File -->
  <sym-upload
      ref="uploader"
      class="d-none lams-light border-main curved-1 p-1 shadow-light mb-3"
      inputClass="lams-light shadow"
      uploadButtonClass="success shadow"
      resetButtonClass="danger-light border-main shadow-light"
      iconClass="fa-files-o fa-fw fa-3x"
      :multiple="false"
      accept=".pdf,.jpg,.png"
      instructions="Drag documents here<br>or click to browse"
      uploadButtonText="Click to Upload Now"
      :blinkUploadIcon="true"
      @select="onSelectDocuments"
      @upload="onUploadDocuments"
      @selectedchanged="onSelectedChanged"
      size="sm"
    >
  </sym-upload>

</section>
</template>

<script>

import { DateTime } from "../../js/core";

import {
  get,
  upload, //PHOTO
  ajax,
} from "../../js/http";

import {
  getCount,
  getList,
  getSafeDeleteFlag,
} from "../../js/dbSys";

import PageMaintenance from "../PageMaintenance.vue";
import SymImageSelect from "../../comp/SymImageSelect.vue";
import SymInteger from '../../comp/SymInteger.vue';

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "hrs0030",

  data() {
    return {
      
      applicant: {
        applicantId: 0,
        memberId: 0,
        applicantLastName: "",
        applicantFirstName: "",
        applicantMiddleName: "",
        applicantSuffixId: 0,
        applicantName: "", 
        birthDate: null,
        age: 0,
        sexId: "",
        civilStatusId: 0,
        religionId: 0,
        address1: "",
        postalCode: "",
        phoneNumber: "",
        mobileNumber: "",
        email: "",
        applicationSourceId: 0,
        applicationDate: null,
        applicantStatusId: 0,
        applicantStatusName: "",
        regionId: "",
        provinceId: "",
        provinceName:"",
        municipalityId: "",
        barangayId: "",
        memberRequestId: 0,
        memberRequestName: "",
        name3: "",
        memberTypeId: 0,
        lockId: "",
      },

      oldApplicant: [],
      logs: [],
      isLogVisible: false,

      provinces: [],
      municipalities: [],
      barangays: [],

      docs: [],
      
      isDocPreviewVisible: false,
      previewFileUrl: '', 
      previewFileName: '',

      fullImageFileNames: [],
      fullscreenElement: null,
      imageZoom: .75,

      doc: {
        docTypeDetailId: 0,
        memberId: 0,
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
      memberRequest: [],
      isPoolingFlag: false,
    };


  },

  mounted() {
    const me = this;
    me.isPoolingFlag = false;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

    async fileExists(url) {
      try {
        const response = await fetch(url, { method: 'HEAD' });
        return response.ok;
      } catch (error) {
        this.advice.fault("Unsupported File Type.");
        return false;
      }
    },

    async openDocPreview(fileUrl, fileName) {
      const isSupported = /\.(pdf|jpg|jpeg|png)$/i.test(fileUrl);
      if (!isSupported) {
        this.advice.fault("Unsupported File Type.");
        return;
      }

    const exists = await this.fileExists(fileUrl);
    if (!exists) {
      this.advice.fault("File does not exists.");

      return;
    }

    this.previewFileUrl = fileUrl +  '#toolbar=0&navpanes=0&scrollbar=0';
    this.previewFileName = fileName;
    this.isDocPreviewVisible = true;    
    },

    closeDocPreview() {
      this.isDocPreviewVisible = false;
      this.previewFileUrl = '';
      this.previewFileName = '';
    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.applicant.applicantId) {
        q.applicantId = me.applicant.applicantId;
      }

      if (me.applicant.memberTypeId) {
        q.memberTypeId = me.applicant.memberTypeId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("applicantId" in q && me.core.isInteger(q.applicantId)) {
       
        me.applicant.applicantId = parseInt(q.applicantId);

      }


    if ("memberTypeId" in q && me.core.isInteger(q.memberTypeId)) {
        me.isPoolingFlag = true       
        me.applicant.memberTypeId = parseInt(q.memberTypeId);
   
      }

    },
     
    onMemberRequestIdChanging (e) {
      e.message = "Member Request ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback (e) {
      const me = this;
      let filter = "MemberRequestId = '" + e.proposedValue + "'AND MemberRequestStatusId=1" ;
      return getList('dbo.ArsMemberRequest', 'MemberRequestId, MemberRequestName', '', filter).then(
        request => {
          if (request && request.length) {
            me.applicant.memberRequestName = request[0].memberRequestName;
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

    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.applicant;

      item.memberRequestId = data.memberRequestId;
      item.memberRequestName = data.memberRequestName;
      
      this.focusNext();

    },


    showUploadDocumentsResponse (info) {
      if (!info || (!info.createdCount && !info.failedCount)) {
        return Promise.resolve();
      }

      this.docsUploadResult = info;

      setTimeout(() => {
        this.isModalVisible = true;
      }, 200);

      return Promise.resolve();
    },


    onSelectDocuments () {
    },

    onSelectedChanged (fileList) {

    this.selectedFileList = fileList;

    if (fileList.length) {
      const uploader = this.$refs.uploader;
      if (uploader) {
        uploader.invokeUpload();
      }
    }

    },

    onUploadDocuments (e) {
      const
        me = this,
        wait = me.wait();

        me.fileName = "";
        me.guidReference = this.guid();

        me.uploadDocuments(e.fileList)
        .then( info => {
          me.stopWait(wait);

          e.reset();
            e.showUploadResponse(info)
            e.fileList.forEach(file => {
            me.fileName = file.name;
          });

          me.showUploadDocumentsResponse(info)
            .then( () => {

            });

        })
        .catch( fault => {
          me.stopWait(wait);
          e.reset();
          me.showFault(fault);
        });

    },

    onSelectFile () {
     
    const uploader = this.$refs.uploader;
    
    if (uploader) {
      uploader.invokeClick();
    }
    },

    onDocTypeIdChanging (e) {
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

    onDocTypeReferenceChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.docTypeReferenceCallback;
    },

    docTypeReferenceCallback (e) {
      const me = this;

      let docTypeReference = e.proposedValue;
      let filter = "docTypeReference='" + docTypeReference + "'";
      return getCount('dbo.HrsMemberDocType', filter).then(
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
    me.doc.docTypeLength = (selectedDocType.docTypeLength !== undefined && selectedDocType.docTypeLength !== null) 
        ? selectedDocType.docTypeLength 
        : 12;
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
      
      const me = this, d = me.doc;

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

      if (!me.isValid("hrs0010D")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 });
        return;
      }

      if (this.doc.docTypeReference.length < this.doc.docTypeLength) {
        this.advice.fault('Entry is not valid.', { duration: 5 });
        return;
      }


      let docTypeReference = d.docTypeReference;
      let filter = "docTypeReference='" + docTypeReference + "' AND docTypeId=" + d.docTypeId  + " AND docTypeDetailId!=" + d.docTypeDetailId; 

      getCount('dbo.HrsMemberDocType', filter).then(count => {
        if (count > 0) {
          this.advice.fault( '<b>' + docTypeReference + '</b> already exists.');
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

          dtl.docTypeFileName = me.fileName;
          dtl.docTypeGUID = me.guidReference;

          me.clearDocPad();
          me.advice.info("Doc Type Name '" + dtl.docTypeName + "' added to list.", { duration: 5 });
          me.setFocus("DocTypeId");
        } else {
          dtl = me.docs[me.docIndex];

          me.docType.forEach((doc) => {
            if (doc.docTypeId == d.docTypeId) {
              dtl.docTypeName = doc.docTypeName;
            }
          });

          dtl.docTypeDetailId = d.docTypeDetailId;
          dtl.docTypeId = d.docTypeId;
          dtl.docTypeReference = d.docTypeReference;
          dtl.docTypeFileName = me.fileName || d.docTypeFileName;
          dtl.docTypeGUID = me.guidReference || d.docTypeGUID;

          me.isDocEditorVisible = false;
        }
      });

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
       this.docIndex = index;
       this.docType.forEach((doctype) => {
          if (doctype.docTypeId == dtl.docTypeId) {
            dtl.docTypeLength = doctype.docTypeLength;
          }
        });
      d.docTypeDetailId = dtl.docTypeDetailId;
      d.docTypeId = dtl.docTypeId;
      d.docTypeLength = dtl.docTypeLength;
      d.docTypeReference = dtl.docTypeReference;
      d.docTypeFileName = dtl.docTypeFileName;
      d.docTypeGUID = dtl.docTypeGUID;
      this.fileName = dtl.docTypeFileName

      this.isDocEditorVisible = true;
    },

    onAddDoc() {
      const me = this;

      me.clearDocPad();
      me.doc.docTypeDetailId = -1;

      me.isDocEditorVisible = true;
      me.isAddingDoc = true;
    },

    onDeleteDoc(index) {
      const me = this;

      me.dialog.confirm( "Doc <b>" + me.docs[index].docTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.docs.splice(index, 1);
          }
          return;
        });
    },

    clearDocPad() {
      const d = this.doc;

      d.docTypeDetailId = 0;
      d.docTypeId = 0;
      d.docTypeReference = "";
      d.docTypeFileName = "";
      d.docTypeGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },
    onSetCanceled () {
      const me = this;
            
      let promise = me.isActionAllowed('CAN-STATUS-APP');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.onCancel();
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },


    onBarangayIdChanged (newValue) {
      const me = this;
      let o = me.barangays.find( o => o.barangayId == newValue);
      if (o) {
        this.applicant.postalCode =  o.postalCode
      }
    },
     
    onProvinceIdChanging (e) {
      e.message = "Province ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.provinceIdCallback;
    },

    provinceIdCallback (e) {
      const me = this;
      let filter = "ProvinceId='" + e.proposedValue + "'";
      return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter).then(
        province => {
          if (province && province.length) {
            me.applicant.provinceName = province[0].provinceName;
            me.applicant.regionId = province[0].regionId;
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

    onProvinceIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.applicant;

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
      me.barangays = [];

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

    onMunicipalityIdChanged() {
      const me = this,
        wait = me.wait();

      me.getBarangays().then(
        (data) => {
          me.stopWait(wait);
          me.barangays = data;
        
          if (me.barangays.length) {
            me.barangays.unshift({
              barangayId: 0,
              barangayName: "--- Select Barangay ---",
            });
            me.setFocus("barangayId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },


    onAccept () {
      const me = this;

      me.dialog.confirm('Ready to accept Applicant #' + this.applicant.applicantId + ' - <b> '+ this.applicant.applicantName + '</b>.<br><br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.applicant.applicantStatusId = 2;
            me.onSubmit();
            
          }

          return;
        }
      );
    },



    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to cancel <b>Applicant #</b>' + this.applicant.applicantId + ' - '+ this.applicant.applicantName + '.<br>Once cancelled, Applicant cannot be modified and will not be included in the pooling.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.applicant.applicantStatusId = 3;
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    // API calls

    getFileName (fileName, GUID) {
      return get('api/applicant/download-file/' + this.applicant.applicantId + '/' + fileName + '/' + GUID);
    },

    guid () {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
      (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
    },

    uploadDocuments (files) {
      let q = this.guid();

      return upload('api/applicant/' + this.applicant.applicantId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
    },

    onEmailChanging(e) {
      if (!e.proposedValue) {
        return;
      }

      if (!this.core.isValidEmail(e.proposedValue)) {
        e.message = "(" + e.proposedValue + ") is not a valid email address.";
        e.preventDefault();
        return;
      }

    },

    validatePostalCode() {
      if (!this.applicant.postalCode) {
        this.result = null;
        return;
      }

      if (this.applicant.postalCode.length > 4) {
        this.applicant.postalCode = this.applicant.postalCode.slice(0, 4);
      }

      this.applicant.postalCode = this.applicant.postalCode.replace(/\D/g, "");
    },

    validatePhoneNumber() {
      if (!this.applicant.phoneNumber) {
        this.result = null;
        return;
      }

      if (this.applicant.phoneNumber.length > 11) {
        this.applicant.phoneNumber = this.applicant.phoneNumber.slice(0, 11);
      }

      this.applicant.phoneNumber = this.applicant.phoneNumber.replace(/\D/g, "");
    },

    validateMobileNumber() {
      if (!this.applicant.mobileNumber) {
        this.result = null;
        return;
      }

      if (this.applicant.mobileNumber.length > 11) {
        this.applicant.mobileNumber = this.applicant.mobileNumber.slice(0, 11);
      }

      this.applicant.mobileNumber = this.applicant.mobileNumber.replace(/\D/g, "");
    },

    onApplicantLastNameChanging (e) {
      e.callback = this.applicantLastNameCallback;
    },

    applicantLastNameCallback (e) {
      const me = this;
      if (e.proposedValue && me.applicant.applicantFirstName && me.applicant.birthDate) {
      let filter = "ApplicantLastName='" + e.proposedValue + "' AND ApplicantFirstName='" + me.applicant.applicantFirstName + "' AND ApplicantMiddleName='" + me.applicant.applicantMiddleName + "' AND BirthDate='" + this.applicant.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsApplicant', 'ApplicantLastName, ApplicantFirstName, ApplicantMiddleName, BirthDate, ApplicantId', '', filter).then(
        applicant => {
          if (applicant && applicant.length) {
            
          me.dialog.confirm('Already exist. Load exsiting record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.applicant.applicantId = applicant[0].applicantId;
                me.onApplicantIdChanged()            
              return true  
              }
           
              return false; 
            }
          );
            
            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },

    onApplicantFirstNameChanging (e) {
      e.callback = this.applicantFirstNameCallback;
    },

    applicantFirstNameCallback (e) {
      const me = this;
      if (e.proposedValue && me.applicant.applicantLastName && me.applicant.birthDate) {
      let filter = "ApplicantFirstName='" + e.proposedValue + "' AND ApplicantLastName='" + me.applicant.applicantLastName + "' AND ApplicantMiddleName='" + me.applicant.applicantMiddleName + "' AND BirthDate='" + this.applicant.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsApplicant', 'ApplicantLastName, ApplicantFirstName, ApplicantMiddleName, BirthDate, ApplicantId', '', filter).then(
        applicant => {
          if (applicant && applicant.length) {
            
          me.dialog.confirm('Already exist. Load exsiting record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.applicant.applicantId = applicant[0].applicantId;
                me.onApplicantIdChanged()            
              return true  
              }

              return false;
            }
          );
            
            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },

    onApplicantMiddleNameChanging (e) {
      e.callback = this.applicantMiddleNameCallback;
    },

    applicantMiddleNameCallback (e) {
      const me = this;
      if (me.applicant.applicantFirstName && me.applicant.applicantLastName && me.applicant.birthDate) {
      let filter = "ApplicantMiddleName='" + e.proposedValue + "' AND ApplicantLastName='" + me.applicant.applicantLastName + "' AND ApplicantFirstName='" + me.applicant.applicantFirstName + "' AND BirthDate='" + this.applicant.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsApplicant', 'ApplicantLastName, ApplicantFirstName, ApplicantMiddleName, BirthDate, ApplicantId', '', filter).then(
        applicant => {
          if (applicant && applicant.length) {
            
          me.dialog.confirm('Already exist. Load exsiting record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.applicant.applicantId = applicant[0].applicantId;
                me.onApplicantIdChanged()            
              return true  
              }

              return false;
            }
          );
            
            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },


    onBirthDateChanging(e) {

      if (e.proposedValue instanceof DateTime) {
        let me = this,
          p = e.proposedValue;          
          let message ="Minimum Age is '<b>" + me.applicantValidation[0].minimumAge + "</b>'";
          
        if (p > me.applicantValidation[0].minimumBirthDate) {
          e.preventDefault();
          me.advice.fault(message, { duration: 5 });
          return;
        }

      }

      
      const me = this;   
      
      if (me.applicant.applicantLastName && me.applicant.applicantFirstName && e.proposedValue) {
      let filter = "ApplicantLastName='" + me.applicant.applicantLastName + "' AND ApplicantFirstName='" + me.applicant.applicantFirstName + "' AND ApplicantMiddleName='" + me.applicant.applicantMiddleName + "' AND BirthDate='" + e.proposedValue.toDateFormat() + "'" ;
      
      return getList('dbo.HrsApplicant', 'ApplicantLastName, ApplicantFirstName, ApplicantMiddleName, BirthDate, ApplicantId', '', filter).then(
        applicant => {
          if (applicant && applicant.length) {
            
          me.dialog.confirm('Already exist. Load exsiting record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.applicant.applicantId = applicant[0].applicantId;
                me.onApplicantIdChanged()            
              return true  
              }

              return false;
            }
          );
            
            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;

    },

   onBirthDateChanged() {

    const birthDate = new Date(this.applicant.birthDate.year, this.applicant.birthDate.month - 1, this.applicant.birthDate.day);
    const today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear(); 
    const monthDifference = today.getMonth() - birthDate.getMonth();
    
    if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

      this.applicant.age = age;
    }, 

    formatDate() {
      let formattedDate = this.applicant.birthDate.replace(/^(\d{2})(\d{2})(\d{4})$/, '$1/$2/$3');
      this.applicant.birthDate = formattedDate;
    },
  
    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.sources = data.sources;
            me.sex = data.sex;
            me.civilStatus = data.civilStatus;
            me.religion = data.religion;
            me.applicantValidation = data.applicantValidation;   
            me.suffix = data.suffix;
            me.docType = data.docType;
          }
          if (me.applicant.applicantId < 0) {
            return Promise.resolve(null);
          }
          return me.getApplicant();
        })
        .then((applicant) => {
          me.stopWait(wait);
          if (applicant && applicant.applicant.length) {
            me.setModels(applicant);
          } else {
            if (me.applicant.applicantId > -1) {
              let message =
                "Applicant ID '<b>" + me.applicant.applicantId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'You are not allowed to create new documents.';
              me.advice.fault(mssg, { duration: 5 });
              me.onReset();
              return;
            }


            me.applicant.applicationDate = me.today;
            
          }
          if (me.isCancelled || me.isMember) {
            me.setupCancelledState();
          } else {
            me.setupControls();
          }



          me.isFilled = true;
        })
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    },
    onViewLog(log) {
      const me = this,
        wait = me.wait();

        me.getChangeLog(log).then(
        (logs) => {
          me.stopWait(wait);
          me.logs = logs;
          me.isLogVisible = true;
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },


    setModels(info) {
      const me = this,
        applicant = info.applicant[0];
    
      me.applicant = me.core.convertDates(applicant);
      me.provinces = info.province;
      me.municipalities = info.municipality;
      me.barangays = info.barangay;
      me.docs = info.docs;

      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldApplicant = JSON.stringify(me.applicant);      
      me.oldDocs = JSON.stringify(me.docs);
    },

    getProvinces() {
      return get("api/applicant-provinces");
    },

    getMunicipalities() {
      const me = this;
      return get("api/applicant-municipalities/" + me.applicant.provinceId );
    },

    getBarangays() {
      const me = this;
      return get("api/applicant-barangays/" + me.applicant.municipalityId);
    },

 
    getApplicant() {
      if (this.applicant.applicantId < 0) {
        return Promise.resolve(null);
      }

      return get("api/applicants/" + this.applicant.applicantId);
    },

    getReferences() {
      const me = this;
      if (me.sources.length) {
        return Promise.resolve(true);
      }

      return get("api/references/hrs0030");
    },

    getChangeLog(log) {
      return get("api/applicant/" + log + "/" +  this.applicant.applicantId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/applicants/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/applicants/" + this.applicant.applicantId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const applicant = this.applicant,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/applicants/" + applicant.applicantId + "/" + applicant.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        applicant = {};

      Object.assign(applicant, me.applicant);
      applicant.docs = me.docs;
      return applicant;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "applicant","docs","doc",

      );
      dc.keyField = "applicantId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
      (this.docs = []),
      this.isCancelling = false;

      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement("btn-add");
      }, 100);
    },

    onApplicantIdChanging(e) {
      e.callback = this.applicantIdCallback;
    },

    applicantIdCallback(e) {
      e.message = "Applicant ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.HrsApplicant", "applicantId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onApplicantIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onApplicantIdLostFocus() {
      const me = this;

      if (!me.applicant.applicantId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onApplicantIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.applicant.applicantId = data.applicantId;
      me.loadData();
      me.replaceUrl();

    },

    onSubmit(nextRoute) {
      const me = this;
  
      if (!me.isValid(me.$options.name)) {
        me.advice.fault(
          "Fill in the required fields (marked in red) before saving.",
          { duration: 5 }
        );
        return;
      }

      if (me.applicant.applicantTypeId == 2) {
        let total = 0; 
        me.rnrRecordings.forEach(dtl => { 
          if (typeof Number(dtl.rnrNumber) === 'number' && !isNaN(dtl.rnrNumber)) {
            total += Number(dtl.rnrNumber);
          }
        }); 
  
        if (total !== 1) {
          me.advice.fault(
            "Rnr Recording must be equal to 1 before saving.",
            { duration: 5 }
          );
          return; 
        }
      }
  
      if (!me.hasChanges()) {
        me.advice.success("Document updated.", { duration: 5 });
        me.onReset();
        return;
      }
      let promise,
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
              me.applicant.applicantId = success;
            }
        
            if (isNew) {
              message = "New document created.";
       
            } else {
              message = "Document updated.";
        
              if (me.isCancelling) {
                message = "Applicant # '" + me.applicant.applicantId + " - " + me.applicant.applicantName + "' cancelled."
              }
            

            }
            me.setCopyData();

            if (nextRoute && !(nextRoute instanceof MouseEvent)) {
              me.dialog.success(message, { size: "md" }).then(() => {
                me.refreshOldRefs();
                me.go(nextRoute);
     
                return;
                
              });
            } else {
              if (me.isPoolingFlag){
                 me.goBack();
              } else {
              me.advice.success(message, { duration: 5 });
            }
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

      getSafeDeleteFlag("HrsApplicant", me.applicant.applicantId)
        .then((safe) => {
          if (safe) {
            return me.dialog.confirm(
              "Document will be deleted.<br><br>Continue?",
              { scheme: "warning", icon: "warning", size: "md" }
            );
          } else {
            me.advice.fault(
              "Delete attempt failed.<hr>Cannot delete document at this time.",
              { duration: 5 }
            );
            return "";
          }
        })
        .then((reply) => {
          if (reply === "yes") {
            return me.deleteRecord();
          }
          return false;
        })
        .then((success) => {
          if (success) {
            me.advice.success("Document deleted.", { duration: 4 });
            me.onReset();
          }
        })
        .catch((fault) => {
          me.showFault(fault);
        });
    },

    // helpers
    setupCancelledState () {
      const me = this;

      setTimeout(() => {
        me.setDefaultControlStates();

        me.disableElement(
          'btn-save',
          'btn-delete',
          'btn-dtl-edit',
          'btn-dtl-delete',
          'btn-add',
        );

        me.setDisplayMode(
          "applicantLastName",
          "applicantFirstName",
          "applicantMiddleName",
          "applicantSuffix",
          "birthDate",
          "sexId",
          "civilStatusId",
          "religionId",
          "address1",
          "mobileNumber",
          "postalCode",
          "email",  
          "applicationSourceId",  
          "applicationDate",
          "age",
          "memberRequestId",
          "memberRequestName",
        );

        me.setFocus('applicantLastName');
      }, 100);

    },


    setupControls() {
      const me = this;

      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "applicantLastName",
          "applicantFirstName",
          "birthDate",
          "sexId",
          "civilStatusId",
          "religionId",
          "address1",
          "postalCode",
          "applicationSourceId",  
          "applicationDate",
          "memberRequestId"
        );

        me.setDisplayMode(
          "age",          
          "provinceName",
          "memberRequestName"
        );

        me.setFocus("applicantLastName");
      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.applicant) !== me.oldApplicant) {return true;}

      if (JSON.stringify(me.docs) !== me.oldDocs) { return true; }
 
      return false;

    },


  },

  created () {
    const me = this;

    me.oldApplicant = "";
    me.oldDocs = "";
    me.sex = []; 
    me.civilStatus = []; 
    me.religion = []; 

    me.sources = []; 
    me.applicantValidation = []; 
    me.isCancelling = false;
    me.suffix = []; 
    me.docType = [];
    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {
    
 isPDF() {
    const cleanUrl = (this.previewFileName || '').trim().toLowerCase();
    const result = cleanUrl.endsWith('.pdf') || /\.pdf(\w*)?$/.test(cleanUrl);
    return result;
  },

    editorDocPreviewTitle() {
      return this.previewFileName ;
    },

    editorDocTitle() {
      if (this.isAddingDoc) {
        return 'Add Doc Type Detail';
      }
      return 'Doc Type Detail';
    },
     canSetCanceled () {
      return this.applicant.applicantStatusId === 1;
    },

    isCancelled () {
      return this.applicant.applicantStatusId === 3;
    },
       
    isMember () {
      return this.applicant.applicantStatusId === 2;
    },
  
    isActive () {
      return this.applicant.applicantStatusId === 1;
    },

    isValidMobileNumber() {

    let value = "";

      if (this.applicant.mobileNumber) {

      if (this.applicant.mobileNumber.length == 0) {
        value = "default";
        return value; 
      }    
      
      }
      
      const mobileNumberRegex = /^09\d{09}$/;
      const isValid = mobileNumberRegex.test(this.applicant.mobileNumber);

      value = isValid ? "valid" : "invalid";
      return value;
    },  
        

  },
};
</script>

<style scoped>
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.box-field {
display: grid;
grid-template-columns: 1fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr .3fr .5fr .5fr .3fr;
}
.box-2 {
  display: grid;
  grid-template-columns:  .5fr 1fr 1fr 1fr .4fr 2fr ;
}
.box-3 {
  display: grid;
  grid-template-columns:  1fr 1fr 1fr 1fr 1fr 1fr ;
}
.box-4 {
 display: grid;
  grid-template-columns:  .5fr 1.5fr 1fr 1fr 1fr 1fr 1fr;
}

.mobile-fields{
  display: grid;
  grid-template-columns: 1fr  1fr;
  gap: .5rem;
}

.personal {
  display: flex;
  flex-direction: column;
  justify-content: center;
  width: 100%;
  padding: 0;
  margin: 0;
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
.input-file {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
}
.upload-btn{
  border-top-left-radius: 0px;
  border-top-right-radius: 0px;
  width: 100%;
}
.img {
  border: 1px solid black;
  height: 23%;
}
.btn-uploadPhoto {
  width: 100%;

  border-bottom-left-radius: 5px;
  border-bottom-right-radius: 5px;
  border-top-left-radius: 0;
  border-top-right-radius: 0;
}
.tb {
  width: 100%;
  margin-left: 3px;
}

.work-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.status{
  width: 20%;
}
.scroller {
  overflow: hidden;
}
.doc-tb {
  width: 100%;
}

.physic {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.contact {
  display: flex;
  flex-direction: column;
  gap: 0;
  padding: 0px;
}
.soc {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.identify {
  display: flex;
  flex-direction: column;
  gap: 0reemm;
  margin-top: 0;
  padding: 0px;
}
.religion {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.tab1 {
  display: flex;
  flex-direction: column;
  width: 150%;
}
.scroller-tabs {
  padding-top: 10px;
}

.status {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.btns {
  display: flex;

  justify-content: flex-end;
  gap: 0.5rem;
}

.parent-container {
  display: grid;
  grid-template-columns: 7fr 1fr 1fr;
  gap: 0.5rem;
}
.pwd-container {
  display: grid;
  grid-template-columns: 7fr 1fr 1fr;
  gap: 0.5rem;
}
.medical-scroller{
  overflow: auto;
}

.medical-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.kin-scroller {
  overflow: auto;
}
.kin-tb {
  width: 110vw;
}
.kin-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.kin-name {
  width: 81%;
}
.kin-relation {
  width: 11%;
}
.kin-contact {
  width: 8%;
}
.kin-occupation {
  width: 15%;
}
.kin-name {
  width: 15%;
}
.kin-email {
  width: 15%;
}
.kin-file {
  width: 15%;
}
.kin-action {
  width: 6%;
}
.education-scroller {
  overflow: auto;
}
.edu-tb {
  width: 100vw;
}
.edu-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.educ-name {
  width: 12%;
}
.educ-school {
  width: 17%;
}
.educ-course {
  width: 30%;
}
.educ-start {
  width: 4%;
}
.educ-end {
  width: 4%;
}
.educ-file {
  width: 10%;
}
.educ-action {
  width: 10%;
}
.license-scroller{
  overflow: auto;
}
.linces-tb{
  width: 100vw;
}
.license-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.license-name {
  width: 51%;
}
.license-prcid {
  width: 51%;
}
.license-file {
  width: 51%;
}
.license-action {
  width: 20%;
}
.ncii-scroller{
  overflow: auto;
}
.ncii-tb {
  width: 110vw;
}
.ncii-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.ncii-cert {
  width: 15%;
}
.ncii-quali {
  width: 51%;
}
.ncii-issuance {
  width: 15%;
}
.ncii-validity {
  width: 15%;
}
.ncii-training {
  width: 51%;
}
.ncii-assesment {
  width: 51%;
}
.ncii-file {
  width: 25%;
}
.ncii-action {
  width: 25%;
}
.compliance-scroller{
  overflow: auto;
}
.compliance-tb {
  width: 115vw;
}
.compliance-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.compli-cert {
  width: 25%;
}
.compli-compli {
  width: 51%;
}
.compli-issuance {
  width: 15%;
}
.compli-validity {
  width: 15%;
}
.compli-training {
  width: 51%;
}
.compli-assessment {
  width: 51%;
}
.compli-file {
  width: 25%;
}
.compli-action {
  width: 20%;
}
.cda-scroller{
 overflow: auto;
}
.cda-tb{
  width: 100vw;
}   
.cda-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.affiliation-scroller{
 overflow: auto;
}
.affiliation-tb{
  width: 100vw;
}
.affiliation-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.work-scroller {
  overflow: auto;
}
.work-tb {
  width: 120vw;
}
.work-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.work-title {
  width: 31%;
}
.work-company {
  width: 51%;
}
.work-address {
  width: 51%;
}
.work-phone {
  width: 20%;
}
.work-start {
  width: 20%;
}
.work-end {
  width: 20%;
}
.work-action {
  width: 20%;
}

.cert-name {
  width: 51%;
}
.cert-rating {
  width: 15%;
}
.cert-issuedBy {
  width: 51%;
}
.cert-issuedDate {
  width: 15%;
}
.cert-action {
  width: 15%;
}
.elig-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.elig-name {
  width: 81%;
}
.elig-year {
  width: 15%;
}
.elig-action {
  width: 11%;
}
.licen-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.licen-title {
  width: 51%;
}
.licen-expiry {
  width: 10%;
}
.licen-action {
  width: 7%;
}
.skill-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.skill-name {
  width: 51%;
}
.skill-remarks {
  width: 40%;
}
.skill-action {
  width: 2%;
}
.disability-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.docs-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.kin-upload {
  display: grid;
  grid-template-columns: 50fr 5fr;
  gap: 0.5rem;
}
#hrs0030 >>> ul.tabs {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 1rem;
}
.detail-editor-boxes {
  display: flex;
  flex-direction: column;
}
.license-editor-boxes {
  display: flex;
  flex-direction: column;
}
.skill-editor-boxes {
  display: grid;
  grid-template-columns: 3fr 6fr 3fr;
  gap: 0.5rem;
}
.vaccine-editor-boxes {
  display: grid;
  grid-template-columns: 3fr 6fr 3fr;
  gap: 0.5rem;
}
.rnr-editor-boxes{
  display: grid;
  grid-template-columns: 5fr  3fr;
  gap: 0.5rem;
}
.kins-btns {
  display: flex;
  flex-direction: column;
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
.MobilePhone {
  border-top-right-radius: 0;
  border-bottom-right-radius: 4px;
}
.act-btns {
  display: flex;
  justify-content: space-evenly;
}
.remarks-container{
  padding: 0;
}
.remarks{
  border-radius: 0;
  border: 1px solid rgb(154, 150, 150);
}
.upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{

  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.province-field{
  display: grid;
  grid-template-columns: 2fr 5fr;
  width: 100%;
  margin: 0;
  gap: .5rem;
}
.dates{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.box-grid-column-2{
  display: grid;
  grid-template-columns: 3fr 1fr;

}

.modal-content {
 transition: transform 0.2s ease;
  overflow: auto;
}

.image-container {
  overflow: auto;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 600px;
}

</style>
