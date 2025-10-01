// Member Allowance 

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars0110" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" @click="onSubmit">
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete" @click="onDelete">
        <i class="fa fa-times-circle fa-lg"></i><span>Delete</span>
      </button>
      <button type="button" :class="backButtonClass" class="justify-between btn-back"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
    </div>
  </div>

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="allowance.memberAllowanceId" :caption-width="35" :input-width="20" caption="Allowance ID" lookupId="ArsMemberAllowance" @lostfocus="onMemberAllowanceIdLostFocus" @changing="onMemberAllowanceIdChanging" @changed="onMemberAllowanceIdChanged" @searchfill="onMemberAllowanceIdSearchFill" @searchresult="onMemberAllowanceIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>
  
  <div class="box-1 app-box-style gap">
    <sym-int v-model="allowance.memberId" align="bottom" :caption-width="50" caption="Member ID " display-field="memberName" lookupId="HrsMemberAllowance" @changing="onMemberIdChanging" @searchresult="onMemberIdSearchResult"></sym-int> 
    <sym-text v-model="allowance.memberName" align="bottom" :caption-width="60" caption="Member Name" ></sym-text>
    <sym-int v-model="allowance.memberRequestId" align="bottom" :caption-width="34" caption="MRF #" display-field="memberRequestName" lookupId="ArsMemberRequestDeduction" @changing="onMemberRequestIdChanging" @searchfill="onMemberRequestIdSearchFill" @searchresult="onMemberRequestIdSearchResult"></sym-int> 
    <sym-text v-model="allowance.memberRequestName" align="bottom" :caption-width="10" caption="Member Request Name" ></sym-text>
    <sym-memo v-model="allowance.remarks" :caption-width="35"  caption="Remarks" align="bottom"></sym-memo>
  </div>
  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Details</span>
  </div>

  <div id="list-location" class="d-flex fixed-header" ref="list-trx">
    <table class="light striped-even mb-0 scroller">
      <thead>
        <tr>
          <th class="w-50">Allowance</th>
          <th class="w-5 text-right">Amount</th>
          <th class="w-5 text-center">Daily</th>
          <th class="w-6">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in trxs" :key="index">
          <td >{{ dtl.payTrxName }}</td>
          <td class="text-right" >{{  core.toDecimalFormat(dtl.amount, 2, true) }}</td>
          <td class="text-center">
            <i :class="getBooleanIconClass(dtl.dailyFlag)"></i>
          </td>

          <td class="p-1">
            <div class="buttons" sm-1 mb-0 >
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditTrx(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteTrx(index)">
                <i class="fa fa-times fa-lg"></i>
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
      <button type="button" class="justify-between btn-add" @click="onAddTrx">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>

</sym-form>

<sym-modal
  id="trx-editor"
  v-model="isTrxEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="trxEditorTitle"
  @show="onShowTrxEditor($event)"
  @hide="onHideTrxEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="ars0110A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="trx-editor-boxes gap">
      <sym-combo v-model="trx.payTrxCode" caption="Allowance" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onPayTrxCodeChanging"></sym-combo>
      <sym-dec v-model="trx.amount" caption="Amount" align="bottom"></sym-dec>
      <sym-check v-model="trx.dailyFlag" caption="Daily" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitTrx()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isTrxEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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
  name: 'ars0110',

  data () {
    return {

      allowance: {
        memberAllowanceId: 0,
        memberId: 0,
        memberName: '',
        memberRequestId: 0,
        memberRequestName: '',
        remarks: '',
        lockId: ''
      },

      trxs: [],

      isTrxEditorVisible: false,

      trx: {        
        memberAllowanceTrxId: 0,
        memberAllowanceId: 0,
        payTrxCode: '',
        payTrxName: '',
        dailyFlag: false,
        amount: 0,
      },

      trxIndex: -1,
      isAddingTrx: false,
    };
  },

  computed: {
    trxEditorTitle () {
      if (this.isAddingTrx) {
        return 'Add Trx Detail';
      }
      return 'Edit Trx Detail';
    },
  },

  methods: {

    getTargetPath() {
      const me = this,
        q = {};

      if (me.allowance.memberRequestId) {
        q.memberRequestId = me.allowance.memberRequestId;
      }
      
      if (me.allowance.memberId) {
        q.memberId = me.allowance.memberId;
      }

      if (me.allowance.memberName) {
        q.memberName = me.allowance.memberName;
      }

      if (me.allowance.memberRequestName) {
        q.memberRequestName = me.allowance.memberRequestName;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("memberRequestId" in q && me.core.isInteger(q.memberRequestId)) {
       
        me.allowance.memberRequestId = parseInt(q.memberRequestId);
    
      }

      if ("memberId" in q && me.core.isInteger(q.memberId)) {
       
        me.allowance.memberId = parseInt(q.memberId);
    
      }


      if ("memberName" in q && me.core.isString(q.memberName)) {
       
        me.allowance.memberName = q.memberName;
    
      }

      if ("memberRequestName" in q && me.core.isString(q.memberRequestName)) {
       
        me.allowance.memberRequestName = q.memberRequestName;
    
      }


    },


    onPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payTrxCodeCallback;
    },

    payTrxCodeCallback(e) {
      const me = this;
        
      let index = me.trxs.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {

        const payTrxName = me.trxs[index].payTrxName;
        e.message = 'Allowance <b>' + payTrxName + '</b> is already in the list.';
        return false;
      }

      return true;
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
            me.allowance.memberRequestName = request[0].memberRequestName;
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
      e.filter = "memberId = " + this.allowance.memberId 
     
    },


    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.allowance;

      item.memberRequestId = data.memberRequestId;
      item.memberRequestName = data.memberRequestName;
      
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
            me.allowance.memberName = member[0].memberName;
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
        item = this.allowance;

      item.memberId = data.memberId;
      item.memberName = data.memberName;
      
      this.focusNext();

    },


    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.payTrx = data.payTrx;
          }
          if (me.allowance.memberAllowanceId < 0) {
            return Promise.resolve(null);
          }
          return me.getAllowance();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.allowance.length) {
            me.setModels(info);
          } else {
            if (me.allowance.memberAllowanceId > -1) {
              let message = "Member Allowance ID '<b>" + me.allowance.memberAllowanceId + "</b>' not found.";
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

      me.allowance = me.core.convertDates(info.allowance[0]);
      me.trxs = info.trxs;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldAllowance = JSON.stringify(me.allowance);
      me.oldTrxs = JSON.stringify(me.trxs);
    },

    // API calls

    getAllowance () {
      return get('api/member-allowances/' + this.allowance.memberAllowanceId);
    },

    getReferences () {
      const me = this;

      if (me.payTrx.length) {
        return Promise.resolve(true);
      }
      return get('api/references/ars0110');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-allowances/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/member-allowances/' + this.allowance.memberAllowanceId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        allowance = this.allowance,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/member-allowances/' + allowance.memberAllowanceId + this.getDeleteQuery(allowance.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        allowance = {};

      Object.assign(allowance, me.allowance);
      allowance.trxs = me.trxs;

      return allowance;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('allowance', 'trxs', 'trx',);
      dc.keyField = 'memberAllowanceId';
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

    onMemberAllowanceIdChanging (e) {
      e.callback = this.memberAllowanceIdCallback;
    },

    memberAllowanceIdCallback (e) {
      e.message = "Member Allowance ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.ArsMemberAllowance', 'memberAllowanceId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onMemberAllowanceIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onMemberAllowanceIdLostFocus () {
      const me = this;

      if (!me.allowance.memberAllowanceId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberAllowanceIdSearchFill (e) {
      e.filter = "MemberId = " + this.allowance.memberId + " AND MemberRequestId = " + this.allowance.memberRequestId    
    },


    onMemberAllowanceIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.allowance.memberAllowanceId = data.memberAllowanceId;
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
              me.allowance.memberAllowanceId = success;
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
          me.goBack();
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

      getSafeDeleteFlag('ArsMemberAllowance', me.allowance.memberAllowanceId)
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
            me.goBack();

            me.onReset();
          }
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },

    onShowTrxEditor () {
      const me = this;

      me.setActiveModel('trx');

      setTimeout(() => {
        this.setFocus('payTrxCode');
      }, 200);
    },

    onHideTrxEditor () {
      const me = this;

      me.isAddingTrx = false;
      me.setActiveModel();
    },

    onEditTrx (dtl, index) {

      const d = this.trx;
      this.trxIndex = index;

      d.memberAllowanceTrxId = dtl.memberAllowanceTrxId;
      d.payTrxCode = dtl.payTrxCode;
      d.amount = dtl.amount;
      d.dailyFlag = dtl.dailyFlag;

      this.isTrxEditorVisible = true;
    },

    onAddTrx () {
      const me = this;

      me.clearTrxPad();
      me.trx.memberAllowanceTrxId = -1

      me.isTrxEditorVisible = true;
      me.isAddingTrx = true;
    },

    onDeleteTrx (index) {
      const me = this;

      me.dialog.confirm('Allowance <b>' + me.trxs[index].payTrxName + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.trxs.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitTrx() {
      const me = this,
        d = me.trx;

      if (!d.payTrxCode) {
        me.isTrxEditorVisible = false;
        return;
      }

      if (!me.isValid("ars0110A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingTrx) {
        Object.assign(dtl, d);

        me.payTrx.forEach((payTrx) => {
          if (payTrx.payTrxCode == dtl.payTrxCode) {
            dtl.payTrxName = payTrx.payTrxName;
          }
        });


        me.trxs.push(dtl);
        me.clearTrxPad();
        me.advice.info("Allowance '" + dtl.payTrxName + "' added to list.", { duration: 5 });

        setTimeout(() => {
          me.scrollToBottom('list-trx');
          me.setFocus('payTrxCode');
        }, 100);
      } else {
        dtl = me.trxs[me.trxIndex];

        me.payTrx.forEach((payTrx) => {
          if (payTrx.payTrxCode == d.payTrxCode) {
            d.payTrxName = payTrx.payTrxName;
          }
        });


        dtl.memberAllowanceTrxId = d.memberAllowanceTrxId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.amount = d.amount;
        dtl.dailyFlag = d.dailyFlag;
 
        me.isTrxEditorVisible = false;

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

        me.setRequiredMode(
          'memberId',
          'memberRequestId',
          'remarks'
        );

        me.setDisplayMode(
          'memberId',
          'memberName',
          'memberRequestId',
          'memberRequestName',
        );

        me.setFocus('memberId');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.allowance) !== me.oldAllowance) { return true; }
      if (JSON.stringify(me.trxs) !== me.oldTrxs) { return true; }
      return false;
    },

    clearTrxPad () {
      const d = this.trx;

      d.memberAllowanceTrxId = 0;
      d.payTrxCode = '';
      d.amount = 0;
      d.dailyFlag = false;
    },

  },

  created () {
    const me = this;

    me.oldAllowance = '';
    me.oldTrxs = '';
    me.payTrx = [];
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

.box-1{
  display: grid;
  grid-template-columns: .5fr 1.5fr .5fr 1.5fr 1fr;
}
.trx-editor-boxes{
  display: grid;
  grid-template-columns: 1.5fr .5fr .5fr;
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

  #ars0110 >>> .card-body {
    padding: .5rem;
  }

.member-1 {
 display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  align-items: flex-end;
}


}

</style>