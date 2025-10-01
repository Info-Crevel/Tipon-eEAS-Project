// Disbursement Request

<template>
<section class="container p-0" :key="ts">
<sym-form id="aps0600" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <div slot="toolbar" class="action-buttons app-form-toolbar darken-2 p-1 px-3 border-bottom-main">
    <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onCancel" v-if="!isCancelled && !isApproved">
        <i class="fa fa-times fa-lg"></i><span class="bold">CANCEL</span>
      </button>

      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onReview" v-if="isSubmitted">
        <i class="fa fa-thumbs-up fa-lg"></i><span class="bold">Review</span>
      </button>

      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onApprove" v-if="isReviewed">
        <i class="fa fa-thumbs-o-up fa-lg"></i><span class="bold">Approve</span>
      </button>


    </div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" v-if="!isCancelled && !isApproved" @click="onSubmit">
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear">
        <i class="fa fa-undo fa-lg"></i><span>Clear</span>
      </button>
    </div>

    <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" >
    <i class="fa fa-print fa-lg"></i><span>Voucher</span>
      </button>
    
      <button type="button" :class="logButtonClass" class="justify-between btn-log" >
        <i class="fa fa-database fa-lg"></i><span>Log</span>
      </button>
    </div>

  </div>

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onCancel" v-if="!isCancelled && !isApproved">
        <i class="fa fa-times fa-lg"></i><span class="bold">CANCEL</span>
      </button>


      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onReview" v-if="isSubmitted">
        <i class="fa fa-thumbs-up fa-lg"></i><span class="bold">Review</span>
      </button>

      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onApprove" v-if="isReviewed">
        <i class="fa fa-thumbs-o-up fa-lg"></i><span class="bold">Approve</span>
      </button>

    </div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" v-if="!isCancelled && !isApproved" @click="onSubmit">

        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear">
        <i class="fa fa-undo fa-lg"></i><span>Clear</span>
      </button>
    </div>

    <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" >
    <i class="fa fa-print fa-lg"></i><span>Voucher</span>
      </button>
    
      <button type="button" :class="logButtonClass" class="justify-between btn-log" >
        <i class="fa fa-database fa-lg"></i><span>Log</span>
      </button>
    </div>

  </div>

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="request.requestTrxId" :caption-width="35" :input-width="20" caption="Request #" lookupId="ApsRequestTrx" @lostfocus="onRequestTrxIdLostFocus" @changing="onRequestTrxIdChanging" @changed="onRequestTrxIdChanged" @searchresult="onRequestTrxIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>

    <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
    <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isReviewed">Reviewed</sym-tag>
    <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isApproved">Approved</sym-tag>
    <sym-tag class="warning text-center border-light lg-2 ml-3" :width="38" v-if="isSubmitted">Submitted</sym-tag>


      <div class="buttons d-inline ml-auto">
        <button class="btn-copy border-main justify-between fw-40 danger-light mb-2" type="button" :class="copyButtonClass" v-if="isApproved">
          <i class="fa fa-long-arrow-right fa-lg"></i><span>POST</span>
        </button>
      </div>

  </div>
  
  <div class="d-flex gap-1">
    <sym-date v-model="request.trxDate" :caption-width="35" :input-width="34" caption="Trx Date" @changing="onTrxDateChanging"></sym-date>
    <sym-combo v-model="request.particularsId" :caption-width="30"  caption="Particulars"  display-field="particularsName" :datasource="particulars"></sym-combo>
  <sym-combo v-model="request.apsRequestTypeId" :caption-width="30" :input-width="100"  caption="Request Type" display-field="requestTypeName" :datasource="requestTypes"></sym-combo>

  </div>
    <div class="d-flex gap-1">
  <sym-int v-model="request.memberId" :caption-width="35" caption="Member ID " :input-width="30"  display-field="memberName" lookupId="HrsMember" @changing="onMemberIdChanging" @searchresult="onMemberIdSearchResult"></sym-int> 
    <sym-text v-model="request.memberName" :caption-width="35"  :input-width="150" caption="Member Name" ></sym-text>
  

  </div>

    <div class="d-flex gap-1">
      <sym-int v-model="request.reviewerId" :caption-width="35" :input-width="30"  caption="Reviewer ID" display-field="memberName" lookupId="ApsReviewer" @changing="onReviewerIdChanging" @searchresult="onReviewerIdSearchResult"></sym-int> 
      <sym-text v-model="request.reviewerName" :caption-width="35" :input-width="150"  caption="Reviewer Name" ></sym-text>
    </div>
  <div class="d-flex gap-1">
      <sym-int v-model="request.approverId" :caption-width="35"  :input-width="30"  caption="Approver ID" display-field="memberName" lookupId="ApsApprover" @changing="onApproverIdChanging" @searchresult="onApproverIdSearchResult"></sym-int> 
      <sym-text v-model="request.approverName" :caption-width="35"  :input-width="150"  caption="Approver Name" ></sym-text>
  </div>


    <div class="d-flex ">
      <sym-memo v-model="request.particulars" :input-width="150" caption="Details"></sym-memo>
    </div>


  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Details</span>
  </div>

  <div id="list-detail" class="d-flex fixed-header" ref="list-detail">
    <table class="light striped-even mb-0">
      <thead>
        <tr>
          <th class="w-15">Trx Name</th>
          <th class="w-15">Cooperative</th>
          <th class="w-15">Platform</th>
          <th class="w-10">Cluster</th>
          <th class="w-10">Pay Group</th>
          <th class="w-13">Doc Type</th>
          <th class="w-5 text-right">Amount</th>
          <th class="w-7 ">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in details" :key="index">
          <td >{{ dtl.requestTrxName }}</td>
          <td >{{ dtl.orgName }}</td>
          <td >{{ dtl.platformName }}</td>
          <td >{{ dtl.clusterName }}</td>
          <td >{{ dtl.clientPayGroupName }}</td>
          <td >{{ dtl.docTypeName }}</td>
          <td class="text-right">{{ core.toDecimalFormat(dtl.amount, 2, true) }}</td>

          <td class="p-1">
            <div class="buttons" sm-1 mb-0 >
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)">
                <i class="fa fa-times fa-lg"></i>
              </button>
            </div>
          </td>
        </tr>
          <tr :class="totalsClass">
            <td colspan="6" class="text-right bold">Total</td>
            <td class="text-right">{{ core.toDecimalFormat(request.totalAmount) }}</td>
            <td></td>
          </tr>  

      </tbody>
    </table>
  </div>

  <div class="command-buttons light border-main p-1 border-top-0 mb-2">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between btn-add" @click="onAddDetail">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>


</sym-form>

<sym-modal
  id="detail-editor"
  v-model="isDetailEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="detailEditorTitle"
  @show="onShowDetailEditor($event)"
  @hide="onHideDetailEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="aps0600A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-combo v-model="detail.orgTrxId" caption="Trx Name" align="bottom" display-field="requestTrxName" :datasource="requestTrxs"></sym-combo>
      <sym-combo v-model="detail.orgId" caption="Cooperative" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
      <sym-combo v-model="detail.platformId" caption="Platform" align="bottom" display-field="platformName" :datasource="platforms" ></sym-combo>
      <sym-combo v-model="detail.clusterId" caption="Cluster" align="bottom" display-field="clusterName" :datasource="clusters" ></sym-combo>
      <sym-combo v-model="detail.clientPayGroupId" caption="Pay Group" align="bottom" display-field="clientPayGroupName" :datasource="payGroups" ></sym-combo>
      <sym-dec v-model="detail.amount" caption="Amount" align="bottom" captionAlign="right"></sym-dec>
      <sym-combo v-model="detail.apsDocTypeId" caption="Doc Type" align="bottom" display-field="docTypeName" :datasource="docTypes" ></sym-combo>

    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isDetailEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>
</div>

</sym-modal>


</section>
</template>

<script>

import {
  get,
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
  name: 'aps0600',

  data () {
    return {

      request: {

        requestTrxId: 0,
        trxDate: null,
        memberId: 0,
        memberName: '',
        reviewerId: 0,
        reviewerName: '',
        requestStatusId: 0,
        approverId: 0,
        approverName: '',
        apsRequestTypeId: 0,
        particularsId: 0,
        particulars:'',
        totalAmount:0,
        lockId:''

      },

      details: [],

      isDetailEditorVisible: false,



      detail: {
        requestTrxDetailId: 0,
        requestTrxId: 0,
        requestTrxName: '',

        orgTrxId: 0,
        orgId: 0,
        orgName: '',

        platformId: 0,
        platformName: '',

        clusterId: 0,
        clusterName: '',

        clientPayGroupId: 0,
        clientPayGroupName: '',

        apsDocTypeId: 0,
        docTypeName: '',

        amount: 0,
      },


      detailIndex: -1,
      isAddingDetail: false,

    };
  },

  computed: {

    totalsClass () {    
        return 'success-light';
    },

    isSubmitted () {
      return this.request.requestStatusId === 1;
    },

    isCancelled () {
      return this.request.requestStatusId === 4;
    },

    isReviewed () {
      return this.request.requestStatusId === 2;
    },

    isApproved () {
      return this.request.requestStatusId === 3;
    },

    detailEditorTitle () {
      if (this.isAddingDetail) {
        return 'Add Detail';
      }
      return 'Edit Detail';
    },

  },

  methods: {

    refreshTotals () {
      const me = this;

      me.request.totalAmount = 0;

      me.details.forEach( dtl => {

        me.request.totalAmount = me.request.totalAmount + dtl.amount;
      });

      me.request.totalAmount = me.core.toDecimal(me.request.totalAmount);

    },


    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to <b>cancel</b> transaction.<br>Once cancelled, transaction cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.request.requestStatusId = 4; //Cancelled
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onReview () {
      const me = this;

      me.dialog.confirm('Ready to <b>review</b> transaction.<br><br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.request.requestStatusId = 2; //Reviewed
            me.isReviewing = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onApprove () {
      const me = this;

      me.dialog.confirm('Ready to <b>approve</b> transaction.<br><br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.request.requestStatusId = 3; //Approved
            me.isApproving = true;
            me.onSubmit();
          }
          return;
        }
      );
    },


    onReviewerIdChanging (e) {
      e.message = "Reviewer ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.reviewerIdCallback;
    },

    reviewerIdCallback (e) {
      const me = this;
      let filter = "ReviewerId = " + e.proposedValue;
      return getList('dbo.QApsReviewer', 'ReviewerId, ReviewerName', '', filter).then(
        member => {
          if (member && member.length) {
            me.request.reviewerName = member[0].reviewerName;
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

    onReviewerIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.request;

      item.reviewerId = data.reviewerId;
      item.reviewerName = data.reviewerName;
      
      this.focusNext();

    },

    
    onApproverIdChanging (e) {
      e.message = "Approver ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.approverIdCallback;
    },

    approverIdCallback (e) {
      const me = this;
      let filter = "ApproverId = " + e.proposedValue;
      return getList('dbo.QApsApprover', 'ApproverId, ApproverName', '', filter).then(
        member => {
          if (member && member.length) {
            me.request.approverName = member[0].approverName;
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

    onApproverIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.request;

      item.approverId = data.approverId;
      item.approverName = data.approverName;
      
      this.focusNext();

    },




    onMemberIdChanging (e) {
      e.message = "Member ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberIdCallback;
    },

    memberIdCallback (e) {
      const me = this;
      let filter = "MemberId = " + e.proposedValue;
      return getList('dbo.HrsMember', 'MemberId, MemberName', '', filter).then(
        member => {
          if (member && member.length) {
            me.request.memberName = member[0].memberName;
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
        item = this.request;

      item.memberId = data.memberId;
      item.memberName = data.memberName;
      
      this.focusNext();

    },



        onTrxDateChanging (e) {
      if (e.proposedValue instanceof DateTime) {
        let
          me = this,
          p = e.proposedValue;

        if (p > me.today) {
          e.preventDefault();
          return;
        }
      }
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.orgs = me.sym.userInfo.orgs;
            me.platforms = data.platforms;
            me.requestTypes = data.requestTypes;
            me.particulars = data.particulars;
            me.clusters = data.clusters;
            me.payGroups = data.payGroups;
            me.docTypes = data.docTypes;
            me.requestTrxs = data.requestTrxs;
          }
          if (me.request.requestTrxId < 0) {
            return Promise.resolve(null);
          }
          return me.getRequest();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.request.length) {
            me.setModels(info);
          } else {
            if (me.request.requestTrxId > -1) {
              let message = "Request Trx ID '<b>" + me.request.requestTrxId + "</b>' not found.";
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

            me.request.requestStatusId = 1 ;//Submit

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

      me.request = me.core.convertDates(info.request[0]);
      me.details = info.details;
      me.refreshTotals();

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldRequest = JSON.stringify(me.request);
      me.oldDetails = JSON.stringify(me.details);
    },

    // API calls

    getRequest () {
      return get('api/aps-request-trxs/' + this.request.requestTrxId);
    },

    getReferences () {
      const me = this;

      if (me.orgs.length) {
        return Promise.resolve(true);
      }
      return get('api/references/aps0600');
    },


    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/aps-request-trxs/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/aps-request-trxs/' + this.request.requestTrxId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        request = this.request,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/aps-request-trxs/' + request.requestTrxId + this.getDeleteQuery(request.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        request = {};

      Object.assign(request, me.request);
      request.details = me.details;

      return request;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('request', 'details', 'detail');
      dc.keyField = 'requestTrxId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.refreshOldRefs();
      this.isCancelling = false;
      this.isReviewing = false;
      this.isApproving = false;

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 50);
    },

    onRequestTrxIdChanging (e) {
      e.callback = this.requestTrxIdCallback;
    },

    requestTrxIdCallback (e) {
      e.message = "Request Trx ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.ApsRequestTrx', 'requestTrxId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onRequestTrxIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onRequestTrxIdLostFocus () {
      const me = this;

      if (!me.request.requestTrxId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onRequestTrxIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.request.requestTrxId = data.requestTrxId;
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
            // set auto-generated ID returned by API so F9 works
            if (isNew && typeof success === 'number' && success > 0) {
              me.request.requestTrxId = success;
            }

            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'

              if (me.isCancelling) {
                message = "Transaction '" + me.request.requestTrxId + "' cancelled."
              }

              if (me.isReviewing) {
                message = "Transaction '" + me.request.requestTrxId + "' reviewed."
              }

              if (me.isApproving) {
                message = "Transaction '" + me.request.requestTrxId + "' approved."
              }

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

      getSafeDeleteFlag('ApsRequestTrx', me.request.requestTrxId)
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

    onShowDetailEditor () {
      const me = this;

      me.setActiveModel('detail');

      me.setRequiredMode(
        'orgTrxId',
        'orgId',
        'platformId',
        'clusterId',
        'clientPayGroupId',
        'apsDocTypeId',
        'amount'
      );

      setTimeout(() => {
        this.setFocus('orgTrxId');
      }, 200);
    },

    onHideDetailEditor () {
      const me = this;

      me.isAddingDetail = false;
      me.setActiveModel();
    },

    onEditDetail (dtl, index) {

      const d = this.detail;
      this.detailIndex = index;

      d.requestTrxDetailId = dtl.requestTrxDetailId;
      d.orgTrxId = dtl.orgTrxId;
      d.orgId = dtl.orgId;
      d.orgName = dtl.orgName;
      d.platformId = dtl.platformId;
      d.clusterId = dtl.clusterId;
      d.clusterName = dtl.clusterName;
      d.clientPayGroupId = dtl.clientPayGroupId;
      d.clientPayGroupName = dtl.clientPayGroupName;
      d.apsDocTypeId = dtl.apsDocTypeId;
      d.docTypeName = dtl.docTypeName;
      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.requestTrxDetailId = -1

      me.isDetailEditorVisible = true;
      me.isAddingDetail = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Request Trx Name <b>' + me.details[index].orgTrxId + '</b> - Platform <b>' + me.details[index].orgTrxId + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.details.splice(index, 1);
            this.refreshTotals();

          }
          return;
        }
      );
    },

    onSubmitDetail() {
      const me = this,
        d = me.detail;
      if (!me.isValid("aps0600A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingDetail) {
        Object.assign(dtl, d);

        me.orgs.forEach((org) => {
          if (org.orgId == dtl.orgId) {
            dtl.orgName = org.orgName;
          }
        });

        me.platforms.forEach((platform) => {
          if (platform.platformId == dtl.platformId) {
            dtl.platformName = platform.platformName;
          }
        });

        me.clusters.forEach((cluster) => {
          if (cluster.clusterId == dtl.clusterId) {
            dtl.clusterName = cluster.clusterName;
          }
        });

        me.payGroups.forEach((payGroup) => {
          if (payGroup.clientPayGroupId == dtl.clientPayGroupId) {
            dtl.clientPayGroupName = payGroup.clientPayGroupName;
          }
        });

        me.docTypes.forEach((docType) => {
          if (docType.apsDocTypeId == dtl.apsDocTypeId) {
            dtl.docTypeName = docType.docTypeName;
          }
        });

        me.requestTrxs.forEach((requestTrx) => {
          if (requestTrx.orgTrxId == dtl.orgTrxId) {
            dtl.requestTrxName = requestTrx.requestTrxName;
          }
        });

        me.details.push(dtl);
        me.clearDetailPad();
        me.advice.info("Trx Name '" + dtl.requestTrxName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list-detail');
          me.setFocus('orgTrxId');
        }, 100);
      } else {

        dtl = me.details[me.detailIndex];

        me.orgs.forEach((dtl) => {
          if (dtl.orgId == d.orgId) {
            d.orgName = dtl.orgName;
          }
        });

        me.platforms.forEach((dtl) => {
          if (dtl.platformId == d.platformId) {
            d.platformName = dtl.platformName;
          }
        });

        me.clusters.forEach((dtl) => {
          if (dtl.clusterId == d.clusterId) {
            d.clusterName = dtl.clusterName;
          }
        });

        me.payGroups.forEach((dtl) => {
          if (dtl.clientPayGroupId == d.clientPayGroupId) {
            d.clientPayGroupName = dtl.clientPayGroupName;
          }
        });

        me.docTypes.forEach((dtl) => {
          if (dtl.apsDocTypeId == d.apsDocTypeId) {
            d.docTypeName = dtl.docTypeName;
          }
        });

        me.requestTrxs.forEach((dtl) => {
          if (dtl.orgTrxId == d.orgTrxId) {
            d.requestTrxName = dtl.requestTrxName;
          }
        });



        dtl.requestTrxDetailId = d.requestTrxDetailId;
        dtl.orgTrxId = d.orgTrxId;
        dtl.requestTrxName = d.requestTrxName;

        dtl.orgName = d.orgName;
        dtl.platformId = d.platformId;
        dtl.clusterId = d.clusterId;
        dtl.clusterName = d.clusterName;
        dtl.clientPayGroupId = d.clientPayGroupId;
        dtl.clientPayGroupName = d.clientPayGroupName;

        dtl.platformName = d.platformName;
        dtl.apsDocTypeId = d.apsDocTypeId;
        dtl.docTypeName = d.docTypeName;

        dtl.amount = d.amount;

        me.isDetailEditorVisible = false;

      }

      this.refreshTotals();

    },

  
    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'trxDate',
          'memberId',
          'reviewerId',
          'requestStatusId',
          'approverId',
          'apsRequestTypeId',
          'particularsId',
          'particulars',

        );

        me.setDisplayMode(
          'memberName',
          'reviewerName',
          'approverName'
        );

 
        me.setFocus('trxDate');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.request) !== me.oldRequest) { return true; }
      if (JSON.stringify(me.details) !== me.oldDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.requestTrxDetailId = 0;
      d.orgTrxId = 0;
      d.requestTrxName = '';
      d.orgId = 0;
      d.orgName = '';
      d.platformId = 0;
      d.platformName = '';
      d.clusterId = 0;
      d.clusterName = '';
      d.clientPayGroupId = 0;
      d.clientPayGroupName = '';
      d.apsDocTypeId = 0;
      d.docTypeName = '';
      d.amount = 0;
    },

  },

  created () {
    const me = this;

    me.oldRequest = '';
    me.oldDetails = '';
    me.orgs = [];     
    me.platforms = [];
    me.requestTypes = [];
    me.particulars = [];
    me.clusters = [];
    me.payGroups = [];
    me.docTypes = [];
    me.requestTrxs = [];
    me.isCancelling = false;
    me.isReviewing = false;
    me.isApproving = false;

}

}

</script>

<style scoped>

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

#religion-editor >>> .modal-sm {
  min-width: 30%;
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
  #list-detail table,
  #list-detail thead,
  #list-detail tbody,
  #list-detail th,
  #list-detail td,
  #list-detail tr {
    display: block;
  }

  #list-detail thead tr {
		position: absolute;
		top: -9999px;
		left: -9999px;
	}

  #list-detail td {
		position: relative;
		padding-left: 50%;
		white-space: normal;
		text-align: left !important;
	}

  #list-detail td:before {
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

  #aps0600 >>> .card-body {
    padding: .5rem;
  }
}

</style>