// Member Deduction Entry

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="ars0080"
      :caption="pageName"
      cardClass="curved-0"
      headerClass="app-form-header"
      footerClass="border-top-main frs-form-footer darken-2 py-0"
      bodyClass="frs-form-body pb-3 "
    >

     <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          
          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit" v-if="!isCanceled"> <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete" @click="onCancel" v-if="!isCanceled"><i class="fa fa-times-circle fa-lg"></i><span class="bold">CANCEL</span></button>
          
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      </div>
      </div>


      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="deduction.deductionTrxId" :caption-width="35" caption="Deduction Trx ID" lookupId="ArsMemberDeductionTrx" @lostfocus="onDeductionTrxIdLostFocus" @changing="onDeductionTrxIdChanging" @changed="onDeductionTrxIdChanged" @searchresult="onDeductionTrxIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
          <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCanceled">Cancelled</sym-tag>
          <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActive">Active</sym-tag>
        </div>
          

      </div>
        <div class="box-field app-box-style">

              <div class="box-1 gap">
                <sym-int v-model="deduction.memberId" align="bottom" :caption-width="50" caption="Member ID " display-field="memberName" lookupId="HrsMember" @changing="onMemberIdChanging" @searchresult="onMemberIdSearchResult"></sym-int> 
                <sym-text v-model="deduction.memberName" align="bottom" :caption-width="60" caption="Member Name" ></sym-text>


              
                <sym-combo v-model="deduction.memberDeductionTypeId" align="bottom" :caption-width="60" caption="Deduction Type" display-field="memberDeductionTypeName" :datasource="deductionType"></sym-combo>

                <sym-int v-model="deduction.memberRequestId" align="bottom" :caption-width="34" caption="MRF #" display-field="memberRequestName" lookupId="ArsMemberRequestDeduction" @changing="onMemberRequestIdChanging" @searchfill="onMemberRequestIdSearchFill" @searchresult="onMemberRequestIdSearchResult"></sym-int> 
                <sym-text v-model="deduction.memberRequestName" align="bottom" :caption-width="10" caption="Member Request Name" ></sym-text>
                <sym-dec v-model="deduction.totalAmount" align="bottom" :caption-width="10" caption="Amount" ></sym-dec>          
                <sym-date v-model="deduction.startDate" align="bottom" :caption-width="10" caption="Start Date" ></sym-date>          
                <sym-int v-model="deduction.termsInMonth" align="bottom" :caption-width="34" caption="Terms in Month"></sym-int> 
              </div>

        </div>

    </sym-form>

</section>
</template>

<script>
import {
  get,
  upload, 
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
  name: "ars0080",

  data() {
    return {
      
      deduction: {
        deductionTrxId: 0,
        memberId: 0,
        memberName: '',
        memberRequestId: 0,
        memberRequestName: '',
        memberDeductionTypeId: 0,
        deductionTrxStatusId: 0,
        totalAmount: 0,
        deductionAmount: 0,
        startDate: null,
        termsInMonth: 0,
        lockId: "",
      },

      oldDeduction: [],
    };


  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

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
            me.deduction.memberName = member[0].memberName;
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
        item = this.deduction;

      item.memberId = data.memberId;
      item.memberName = data.memberName;
      
      this.focusNext();

    },

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.deductionType = data.deductionType;
          }
          if (me.deduction.deductionTrxId < 0) {
            return Promise.resolve(null);
          }
          return me.getDeduction();
        })
        .then((deduction) => {
          me.stopWait(wait);
          if (deduction && deduction.deduction.length) {
            me.setModels(deduction);
          } else {
            if (me.deduction.deductionTrxId > -1) {
              let message =
                "Deduction Trx ID '<b>" + me.deduction.deductionTrxId + "</b>' not found.";
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


            me.deduction.startDate = me.today;
            
          }

          if (me.isCancelled ) {
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

    setModels(info) {
      const me = this,
        deduction = info.deduction[0];
    
      me.deduction = me.core.convertDates(deduction);
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldDeduction = JSON.stringify(me.deduction);      
    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.deduction.deductionTrxId) {
        q.deduction = me.deduction.deductionTrxId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("deductionTrxId" in q && me.core.isInteger(q.deductionTrxId)) {
       
        me.deduction.deductionTrxId = parseInt(q.deductionTrxId);
      }

     },


    onMemberRequestIdChanging (e) {
      e.message = "Member Request ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback (e) {
      const me = this;
      let filter = "MemberRequestId = " + e.proposedValue + " AND MemberId=" + me.deduction.memberId;
      return getList('dbo.QArsMemberRequestHiredList', 'MemberRequestId, MemberRequestName', '', filter).then(
        request => {
          if (request && request.length) {
            me.deduction.memberRequestName = request[0].memberRequestName;
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

    onMemberRequestIdSearchFill (e) {
      e.filter = "memberId = " + this.deduction.memberId 
     
    },


    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.deduction;

      item.memberRequestId = data.memberRequestId;
      item.memberRequestName = data.memberRequestName;
      
      this.focusNext();

    },
    onSetCanceled () {
      const me = this;
            
      me.onCancel();

    },

    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to cancel <b>Deduction Trx #</b>' + this.deduction.deductionTrxId + ' - '+ this.deduction.memberName + '.<br>.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.deduction.deductionTrxStatusId = 3;
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    // API calls

 
    getDeduction() {
      if (this.deduction.deductionTrxId < 0) {
        return Promise.resolve(null);
      }

      return get("api/member-deductions/" + this.deduction.deductionTrxId);
    },

    getReferences() {
      const me = this;
      if (me.deductionType.length) {
        return Promise.resolve(true);
      }

      return get("api/references/ars0080");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/member-deductions/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/member-deductions/" + this.deduction.deductionTrxId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const deduction = this.deduction,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/member-deductions/" + deduction.deductionTrxId + "/" + deduction.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        deduction = {};

      Object.assign(deduction, me.deduction);
      return deduction;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "deduction",

      );
      dc.keyField = "deductionTrxId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
     
      this.isCancelling = false;

      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement("btn-add");
      }, 100);
    },

    onDeductionTrxIdChanging(e) {
      e.callback = this.deductionTrxIdCallback;
    },

    deductionTrxIdCallback(e) {
      e.message = "Deduction Trx ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.ArsMemberDeductionTrx", "DeductionTrxId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onDeductionTrxIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onDeductionTrxIdLostFocus() {
      const me = this;

      if (!me.deduction.deductionTrxId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onDeductionTrxIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.deduction.deductionTrxId = data.deductionTrxId;
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
              me.deduction.deductionTrxId = success;
            }

            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";

              if (me.isCancelling) {
                message = "Deduction Trx ID # '" + me.deduction.deductionTrxId + "' cancelled."
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

      getSafeDeleteFlag("ArsMemberDeductionTrx", me.deduction.deductionTrxId)
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
          "memberId",
          "memberDeductionTypeId",
          "memberId",
          "totalAmount",
          "termsInMonth"
        );

        me.setDisplayMode(
          "memberName",          
          "memberRequestName",          
        );

        me.setFocus("memberId");
      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.deduction) !== me.oldDeduction) {return true;} 
      return false;

    },


  },

  created () {
    const me = this;

    me.oldDeduction = "";
    me.deductionType = []; 
    me.isCancelling = false;
    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {
 
    isCanceled () {
      return this.deduction.deductionTrxStatusId === 3;
    },
    isActive () {
      return this.deduction.deductionTrxStatusId === 1;
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
grid-template-columns: 1fr;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .6fr 1.5fr 1.2fr .5fr 1.5fr .5fr .7fr .6fr;
}
.box-2 {
  display: grid;
  grid-template-columns:  .5fr 1fr 1fr 1fr .4fr 2fr ;
}
.box-3 {
  display: grid;
  grid-template-columns:  1fr 1fr 1.5fr 1fr 1fr .6fr;
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
#ars0080 >>> ul.tabs {
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
</style>
