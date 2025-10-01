// Member Employment Status Action

<template>
<section class="container p-0" :key="ts">
<sym-form id="hrs0050" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
  </div>

  <div class="d-flex gap align-items-end fw-96 w-50">
    <sym-int v-model="action.employmentStatusActionId" :caption-width="60" caption="Employment Status Action ID" lookupId="HrsEmploymentStatusAction" @lostfocus="onEmploymentStatusActionIdLostFocus" @changing="onEmploymentStatusActionIdChanging" @changed="onEmploymentStatusActionIdChanged" @searchresult="onEmploymentStatusActionIdSearchResult"></sym-int>
    <div class="buttons d-inline m-10">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2"  :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
    </div>
    <div class="app-box-style gap">
      <div class="text-center border-light curved p-1 slategray mt-2">
        <span class="serif lg-3">{{ TotalTag }}</span>
      </div>

      <div id="list-location" class="d-flex fixed-header " ref="list-location">
        <table class="light striped-even mb-0 scroller">
          <thead>
            <tr>
              <th class="w-15">Member ID</th>
              <th class="w-15">Employee ID</th>
              <th class="w-28">Member Name</th>
              <th class="w-28">Current Status</th>
              <th class="w-20">Pending Status</th>
              <th class="w-20">Effectivity Date</th>
              <th class="w-20">End Date</th>
              <th class="w-28">Status</th>
              <th class="w-28">Form</th>
              <th class="w-28">File</th> 
              
              <th class="w-30">Action</th>
            </tr>
          </thead>
          <tbody class="white">
            <tr v-for="(dtl, index) in members" :key="index">
              <td data-label="Region">{{ dtl.memberId }}</td>
              <td data-label="Region">{{ dtl.memberEmployeeId }}</td>
              <td data-label="Region">{{ dtl.memberName }}</td>
              <td data-label="Region">{{ dtl.currentEmploymentStatusName }}</td>
              <td data-label="Region">{{ dtl.employmentStatusName }}</td>
              <td data-label="Region">{{ dtl.effectivityDate }}</td>
              <td data-label="Region">{{ dtl.endDate }}</td>
              <td data-label="Region">{{ dtl.statusActionName }}</td>
              <td data-label="Region">{{ dtl.workFormName }}</td>
              <td data-label="Region">{{ dtl.employmentFileName }}</td>
          
              <td class="p-1">
                <div class="d-flex justify-evemly gap" sm-1 mb-0 >
                  <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditMember(dtl, index)" v-if="dtl.statusActionId==1">
                    <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                  </button>
                  <button type="button" :class="logButtonClass" class="justify-between btn-log w-60"   @click="onViewLog(index)">
                    <i class="fa fa-database fa-lg"></i><span>Log</span> 
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
        <div></div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
          <button type="button" class="justify-between btn-add" @click="onAddMember">
            <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
          </button>
        </div>
      </div>
    </div>  

</sym-form>
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
      size = "sm"
    >
  </sym-upload>


<sym-modal
  id="member-editor"
  v-model="isMemberEditorVisible"
  size="md"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="memberEditorTitle"
  @show="onShowMemberEditor($event)"
  @hide="onHideMemberEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="hrs0050A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="member-editor-boxes">
        <div class="box-1 gap">

       
          <sym-int v-model="member.memberId" caption="Member ID" align="left" :caption-width="53" lookupId="HrsMemberEmploymentStatusAction" @changing="onMemberIdChanging" @searchresult="onMemberIdSearchResult"></sym-int>
          <sym-text v-model="member.memberEmployeeId" caption="Employee ID" :caption-width="40"  align="left"></sym-text>
        </div>
        <sym-text v-model="member.memberName" caption="Member Name" :caption-width="40"  align="left"></sym-text>
      <sym-text v-model="member.currentEmploymentStatusName" caption="Current Status"  :caption-width="40" align="left"></sym-text>
      <sym-combo v-model="member.employmentStatusId" caption="Proposed Status" align="left" :caption-width="40" display-field="employmentStatusName" :datasource="employmentStatus" @changed="onEmploymentStatusIdChanged(member.employmentStatusId)"></sym-combo>
     
        <div class="box-1 gap">
          
          <sym-date v-model="member.effectivityDate" caption="Effectivity Date" align="left" :caption-width="53"></sym-date>
          <sym-date v-model="member.endDate" caption="End Date" align="left" :caption-width="40"></sym-date>
        </div>
         <sym-combo v-model="member.workFormId" caption="Form" align="left" :caption-width="40" display-field="workFormName" :datasource="workForm" @changing="onWorkFormIdChanging" ></sym-combo>
      <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
      </div>

    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitMember()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isMemberEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>
</div>

</sym-modal>

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
        title="Employment Status Change Log"
        headerClass="app-form-header"
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

</section>
</template>

<script>

import {
  get,
  upload, 
  ajax
} from '../../js/http';

import {
  getList,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'hrs0050',

  data () {
    return {

      action: {
        employmentStatusActionId: 0,
        remarks: '',
        lockId: ''
      },

      members: [],

      isMemberEditorVisible: false,

      member: {
        employmentStatusMemberId: 0,
        employmentStatusActionId: 0,
        memberId: 0,
        memberName: '',
        memberEmployeeId: '',
      
        currentEmploymentStatusName: '',
        employmentStatusId: 0,
        effectivityDate: null,
        endDate: null,
        statusActionId: 0,
        workFormId:0,
        workFormName:"",
        employmentFileName: "",
        employmentGUID: "",
        fileURL: "",
        lockId: "",
      },
      memberIndex: -1,
      isAddingMember: false,
      
      logs: [],
      isLogVisible: false,
      selectedFileList: [],
      documentNames: [],         
      docsUploadResult: null,
      catalogItems: [],

      fileName: "",
      isUploadRequired: false,
      pathFileName: "",
      guidReference: "",

      workForms: [],

      workForm: {
        workFormId: 0,
        workFormName: "",
        lockId: "",
      },
    };
  },

  computed: {

    TotalTag () {
      return 'Member Total Count: ' + this.members.length;
    },

    memberEditorTitle () {
      if (this.isAddingMember) {
        return 'Add Member Detail';
      }
      return 'Edit Member Detail';
    },

  },

  methods: {
    
    onEmploymentStatusIdChanged (newValue) {
      const me = this;
    
      let o = me.employmentStatus.find( o => o.employmentStatusId == newValue);
      if (o) {
        if (o.endDateRequiredFlag) {
          me.setRequiredMode(
          'endDate'
          )
        }
        else {
          me.setDisplayMode(
          'endDate'
          )
        


        }

      }
    },

    onViewLog(index) {
      const me = this,
        wait = me.wait(),
        d = me.member,
        dtl = me.members[index];
    
      d.employmentStatusActionId = dtl.employmentStatusActionId;
      d.memberId = dtl.memberId;
        me.getChangeLog().then(
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

    onMemberIdChanging (e) {
      e.message = "Member ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.memberIdCallback;
    },

    memberIdCallback (e) {
      const me = this;
      let filter = "MemberId=" + e.proposedValue;
      return getList('QHrsMemberEmploymentStatusAction', 'MemberId, MemberName, MemberEmployeeId, CurrentEmploymentStatusName', '', filter).then(
        member => {
          if (member && member.length) {

            let index = me.members.findIndex(obj => obj.memberId === e.proposedValue);
            if (index > -1) {
              e.message = 'Member ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }
            me.member.memberName = member[0].memberName;
            me.member.memberEmployeeId = member[0].memberEmployeeId;
            me.member.currentEmploymentStatusName = member[0].currentEmploymentStatusName;

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

    onMemberIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        member = this.member;
        let index = this.members.findIndex(obj => obj.memberId === data.memberId);
        if (index > -1) {
          this.advice.fault('Member ID <b>' + data.skillDetailId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        member.memberId = data.memberId;
        member.memberName = data.memberName;
        member.memberEmployeeId = data.memberEmployeeId;
        member.currentEmploymentStatusId = data.currentEmploymentStatusId;
        member.currentEmploymentStatusName = data.currentEmploymentStatusName;

        this.focusNext();

    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.employmentStatus = data.employmentStatus;
            me.workForm = data.workForms;
          }
          if (me.action.employmentStatusActionId < 0) {
            return Promise.resolve(null);
          }
          return me.getAction();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.action.length) {
            me.setModels(info);
          } else {
            if (me.action.employmentStatusActionId > -1) {
              let message = "Employment Status Action ID '<b>" + me.action.employmentStatusActionId + "</b>' not found.";
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
          }
          me.setupControls();
          me.isFilled = true;
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })
    },

    setModels (info) {
      const me = this;

      me.action = me.core.convertDates(info.action[0]);
      me.members = info.members;
      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldAction = JSON.stringify(me.action);
      me.oldMembers = JSON.stringify(me.members);
    },

    // API calls
    getChangeLog() {
      const
        detail = this.member;

      return get("api/employment-status-actions/" + detail.employmentStatusActionId + "/" + detail.memberId + "/log");
    },


    getAction () {
      return get('api/employment-status-actions/' + this.action.employmentStatusActionId);
    },

    getReferences () {
      const me = this;

      if (me.employmentStatus.length) {
        return Promise.resolve(true);
      }
      if (me.workForm.length) {
        return Promise.resolve(true);
      }
      return get('api/references/hrs0050');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/employment-status-actions/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/employment-status-actions/' + this.action.employmentStatusActionId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        action = this.action,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/employment-status-actions/' + action.employmentStatusActionId + this.getDeleteQuery(action.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        action = {};

      Object.assign(action, me.action);
      action.members = me.members;

      return action;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('action', 'members', 'member');
      dc.keyField = 'employmentStatusActionId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 50);
    },

    onEmploymentStatusActionIdChanging (e) {
      e.callback = this.employmentStatusActionIdCallback;
    },

    employmentStatusActionIdCallback (e) {
      e.message = "Employment Status Action ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.HrsEmploymentStatusAction', 'EmploymentStatusActionId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onEmploymentStatusActionIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onEmploymentStatusActionIdLostFocus () {
      const me = this;

      if (!me.action.employmentStatusActionId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onEmploymentStatusActionIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.action.employmentStatusActionId = data.employmentStatusActionId;
      me.replaceUrl();
      me.loadData();
    },

  
  
    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 5 });
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
            if (isNew && typeof success === 'number' && success > 0) {
              me.action.employmentStatusActionId = success;
            }

            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'
            }
            me.setCopyData();

            if (nextRoute && !(nextRoute instanceof MouseEvent)) {
              me.dialog.success(message, { size: 'md' }).then(
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
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

    },

    onDelete () {
      const me = this;

      getSafeDeleteFlag('HrsEmploymentStatusAction', me.action.employmentStatusActionId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Document will be deleted.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" } );
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            return me.deleteRecord();
          }
          return false;
        })
        .then( success => {
          if (success) {
            me.advice.success('Document deleted.', { duration: 4 });
            me.onReset();
          }
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },

    onShowMemberEditor () {
      const me = this;

      me.setActiveModel('member');

      me.setRequiredMode(
        'memberId',
        'employmentStatusId',
        'effectivityDate',
      );

      me.setOptionalMode(
        'endDate'
      );


      me.setDisplayMode(
        'memberName',
        'memberEmployeeId',
        'currentEmploymentStatusName'
      );



      setTimeout(() => {
        this.setFocus('memberId');
      }, 200);
    },

    onHideMemberEditor () {
      const me = this;

      me.isAddingMember = false;
      me.setActiveModel();
    },

    onEditMember (dtl, index) {

      const d = this.member;
      this.memberIndex = index;

      dtl = this.core.convertDates(dtl);
      if (dtl.effectivityDate === null) {
        dtl.effectivityDate = this.core.emptyDateTime();
      }

      if (dtl.endDate === null) {
        dtl.endDate = this.core.emptyDateTime();
      }

      d.employmentStatusMemberId = dtl.employmentStatusMemberId;
      d.memberId = dtl.memberId;
      d.memberName = dtl.memberName;
      d.memberEmployeeId = dtl.memberEmployeeId;
      d.currentEmploymentStatusName = dtl.currentEmploymentStatusName;
      d.employmentStatusId = dtl.employmentStatusId;
      d.effectivityDate = dtl.effectivityDate;
      d.endDate = dtl.endDate;
      d.statusActionId = dtl.statusActionId;
      d.workFormId = dtl.workFormId;

      d.employmentFileName = dtl.employmentFileName;
      d.employmentGUID = dtl.employmentGUID;
      this.fileName = dtl.employmentFileName;

      this.isMemberEditorVisible = true;
    },

    onAddMember () {
      const me = this;

      me.clearMemberPad();
      me.member.employmentStatusMemberId = -1
      me.member.effectivityDate = me.today;

      me.isMemberEditorVisible = true;
      me.isAddingMember = true;
    },
    
    onDeleteMember (index) {
      const
        me = this,
        d = me.member,
        dtl = me.members[index];

      me.memberIndex = index;
      me.clearMemberPad();

      d.employmentStatusMemberId = dtl.employmentStatusMemberId;
      me.refreshOldRefs();

      me.isMemberEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('HrsEmploymentStatusAction', me.member.employmentStatusMemberId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Member <b>' + me.members[index].memberId + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.members.splice(index, 1);
          }
          return false;
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },




    onSubmitMember() {
      const me = this,
        d = me.member;
      me.isUploadRequired = true;
      if (!d.memberId) {
        me.isMemberEditorVisible = false;
        return;
      }

      
      if (!me.isValid("hrs0050A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
          me.advice.fault('Upload required. Please select a file.', { duration: 5 });
          return;
        }
      }
      let dtl = {};

      if (me.isAddingMember) {
        Object.assign(dtl, d);

        me.employmentStatus.forEach((employmentStatus) => {
          if (employmentStatus.employmentStatusId == dtl.employmentStatusId) {
            dtl.employmentStatusName = employmentStatus.employmentStatusName;
          }
        });
        me.workForm.forEach((workForm) => {
          if (workForm.workFormId == dtl.workFormId) {
            dtl.workFormName = workForm.workFormName;
          }
        });

        dtl.statusActionId = 1
        dtl.statusActionName = "Pending"
        dtl.employmentFileName = this.fileName;
        dtl.employmentGUID = this.guidReference;
        
        me.members.push(dtl);
        me.clearMemberPad();
        me.advice.info("Member '" + dtl.memberName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list-location');
          me.setFocus('memberId');
        }, 100);
      } else {
        dtl = me.members[me.memberIndex];
        me.employmentStatus.forEach((dtl) => {
          if (dtl.employmentStatusId == d.employmentStatusId) {
            d.employmentStatusName = dtl.employmentStatusName;
          }
        });

        me.workForm.forEach((dtl) => {
          if (dtl.workFormId == d.workFormId) {
            d.workFormName = dtl.workFormName;
          }
        });

        dtl.employmentStatusMemberId = d.employmentStatusMemberId;
        
        dtl.memberId = d.memberId;
        dtl.memberName = d.memberName;
        dtl.memberEmployeeId = d.memberEmployeeId;
        dtl.currentEmploymentStatusName = d.currentEmploymentStatusName;
        dtl.employmentStatusName = d.employmentStatusName;
        dtl.employmentStatusId = d.employmentStatusId;
        dtl.effectivityDate = d.effectivityDate;
        dtl.endDate = d.endDate;
        dtl.statusActionId = d.statusActionId;
        dtl.workFormId = d.workFormId;
        dtl.workFormName = d.workFormName;
        
        if (this.fileName) {
        dtl.employmentFileName = this.fileName;
        } else {
        dtl.employmentFileName = d.employmentFileName;
        }

        if (this.guidReference) {
        dtl.employmentGUID = this.guidReference;
        } else {
        dtl.employmentGUID = d.employmentGUID;
        }


        me.isMemberEditorVisible = false;

      }
    },


    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();
        me.setFocus('remarks');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.action) !== me.oldAction) { return true; }
      if (JSON.stringify(me.members) !== me.oldMembers) { return true; }
      return false;
    },

    clearMemberPad () {
      const d = this.member;

      d.employmentStatusActionId = 0;
      d.memberId = 0;
      d.memberName = '';
      d.memberEmployeeId = '';
      d.currentEmploymentStatusName = '';
      d.employmentStatusId = 0;
      d.effectivityDate = null;
      d.endDate = null;
      d.statusActionId = 0;
      d.employmentFileName = "";
      d.employmentGUID = "";
      d.workFormId = 0;
      d.workFormName = "";
      
      this.fileName = "";
      this.guidReference = "";
    },

    loadFile (fileName, GUID) {
      const
        me = this;
        me.getFileName(fileName, GUID).then(
        info => {
          me.pathFileName = info

          return;
        }
      );

    },

    getFileName (fileName, GUID) {
      return get('api/member/download-file/' + this.member.memberId + '/' + fileName + '/' + GUID);
    },

    guid () {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
      (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
    },

    // API calls
    uploadDocuments (files) {
      let q = this.guid();

      return upload('api/member/' + this.member.memberId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
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

    onSelectDocuments () {
    },
    onWorkFormIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.workFormIdCallback;
    },

    workFormIdCallback(e) {
      const me = this;

      let index = me.workForm.findIndex(obj => obj.WorkFormId === e.proposedValue);
      if (index > -1) {

        const workFormName = me.workForm[index].WorkFormName;
        e.message = 'Work Form <b>' + workFormName + '</b> is already in the list.';
        return false;
      }

      return true;
    },

  },

  created () {
    const me = this;

    me.oldAction = '';
    me.oldMembers = '';
    me.employmentStatus = [];
    me.workForm = [];    
  }

}

</script>

<style scoped>
.Header {
  width: 100%;
  border: 0;
  padding: 5px;
  text-align: center;
  font-weight: bold;
  color: white;
  
  text-transform: uppercase;
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

#religion-editor >>> .modal-sm {
  min-width: 20%;
}

.fixed-header {
  overflow: auto;
  max-height: 50vh;
}
.scroller {
 max-width: 100%;
 width: 100vw;
}

.fixed-header th {
  position: sticky;
  top: 0;
  background: rgb(221, 221, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
}

.fixed-header thead {
  z-index: 1;
  position: relative;
}

.buttons {
  flex-wrap: nowrap;

  
}
.member-editor-boxes{
  display: grid;
  grid-template-rows: .5fr ;
}

#religion-editor >>> .modal-sm {
  min-width: 30%;
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
.box-1{
  display: grid;
  grid-template-columns: 1fr 1fr;
}
td{
  text-wrap: wrap;
}
@media (max-width: 1200px) {
  #list-religion table {
    width: 50%;
  }
}

@media (max-width: 1100px) {
  #religion-editor >>> .modal-sm {
    min-width: 40%;
  }
}

@media (max-width: 900px) {
  #religion-editor >>> .modal-sm {
    min-width: 50%;
  }

  #list-religion table {
    width: 60%;
  }
}

@media (max-width: 840px) {
  #list-location table,
  #list-location thead,
  #list-location tbody,
  #list-location th,
  #list-location td,
  #list-location tr {
    display: block;
  }

  #list-location thead tr {
		position: absolute;
		top: -9999px;
		left: -9999px;
	}

  #list-location td {
		position: relative;
		padding-left: 50%;
		white-space: normal;
		text-align: left !important;
	}

  #list-location td:before {
    content: attr(data-label);
    position: absolute;
    top: 6px;
		left: 6px;
		width: 40%;
		padding-right: 10px;
		white-space: nowrap;
		text-align: left;
		font-weight: bold;
  }
}

@media (max-width: 800px) {
  #list-religion table {
    width: 70%;
  }
}

@media (max-width: 700px) {
  #religion-editor >>> .modal-sm {
    min-width: 60%;
  }

  #list-religion table {
    width: 80%;
  }
}

@media (max-width: 560px) {
  #religion-editor >>> .modal-sm {
    min-width: 65%;
  }

  #list-religion table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #hrs0050 >>> .card-body {
    padding: .5rem;
  }
}

</style>