// Payroll Transaction Setup

<template>
  <section class="container p-0 w-100" :key="ts">
  <sym-form id="dbs0490" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
  
  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
      <button type="button" class="justify-between btn-add" @click="onAddRecord">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>

  <div class="d-flex justify-center fixed-header" ref="list">
    <table class="striped-even mb-0 ">
      <thead>
        <tr>
          <th class="w-5 text-right">ID</th>
          <th class="w-5">Code</th>
          <th class="w-15">Transaction Name</th>
          <th class="w-5 text-center">Pay Group</th>
          <th class="w-5 text-center">MRF</th>
          <th class="w-15 text-center">Pay Formula</th>
          <th class="w-5">Account ID</th>
          <th class="w-33">Account Name</th>
          <th class="w-5 text-right">Sort Seq</th>
          <th class="w-7">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in payTrxs" :key="index">
          <td class="text-right">{{ dtl.payTrxId }}</td>
          <td>{{ dtl.payTrxCode }}</td>
          <td>{{ dtl.payTrxName }}</td>
          
          <td class="text-center">
            <i :class="getBooleanIconClass(dtl.payGroupFlag)"></i>
          </td>

          <td class="text-center">
            <i :class="getBooleanIconClass(dtl.memberRequestFlag)"></i>
          </td>
          <td>{{ dtl.trxFormula }}</td>
          <td>{{ dtl.accountId }}</td>
          <td>{{ dtl.accountName }}</td>
       

          <td class="text-right">{{ dtl.sortSeq }}</td>
          <td class="p-1">
            <div class="buttons d-flex justify-between" sm-1 mb-0>
              <button type="button" class="justify-between infoXXX info-light fw-22 btn-dtl-edit" @click="onEditRecord(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="warningXXX danger-light btn-dtl-delete" title="Delete" @click="onDeleteRecord(index)">
                <i class="fa fa-times fa-lg"></i>
              </button>
            </div>
          </td>
        </tr>
      </tbody>  
    </table>
  </div>

  <div class="d-flex justify-center mt-3" v-if="!payTrxs.length">
    <sym-alert class="info w-100 text-center"  icon="none">
      <span>No records found. File is empty.</span>
    </sym-alert>  
  </div>

  <sym-modal
    id="record-editor"
    v-model="isRecordEditorVisible"
    size="lg"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="false"
    :dismissible="false"
    :closeOnBackButton="false"
    :backdrop="false"
    :title="editorTitle"
    @show="onShowRecordEditor($event)"
    @hide="onHideRecordEditor($event)"
    headerClass="app-form-header"
    dismissButtonClass="danger"
  >

  <div class="board p-1 mb-0 w-100">
    <form id="dbs0490A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="scroller-container">
        <div class="record-editor-boxes">
          <sym-int v-model="detail.payTrxId" caption="Pay Trx ID" align="bottom" :caption-width="100"></sym-int>
          <sym-text v-model="payTrxCodeUpper" caption="Code" align="bottom" @changing="onPayTrxCodeChanging" ></sym-text>
          <sym-text v-model="detail.payTrxName" caption="Transaction Name" align="bottom"></sym-text>
          <sym-check v-model="detail.payGroupFlag" caption="Pay Group" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
          <sym-check v-model="detail.memberRequestFlag" caption="MRF" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
          <sym-text v-model="detail.trxFormula" caption="Pay Formula" :caption-width="35" align="bottom" ></sym-text>
          <sym-text v-model="detail.accountId" :caption-width="30" align="bottom" caption="Account ID" lookupId="FinAccountCore" @changing="onAccountIdChanging" @searchfill="onAccountIdSearchFill" @searchresult="onAccountIdSearchResult"></sym-text>
          <sym-text v-model="detail.accountName" caption="Account Name" :caption-width="35" align="bottom" ></sym-text>
          
          
          
          <sym-int v-model="detail.sortSeq" caption="Sort Seq" align="bottom" captionAlign="right"></sym-int>
        </div>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="onCloseDetail()"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>  
  </div>

  </sym-modal>

</sym-form>
</section>  
</template>

<script>

import {
  get,
  ajax
} from '../../js/http';

import {
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymDecimal },
  extends: PageMaintenance,
  name: 'dbs0490',

  data () {
    return {
      payTrxs: [],

      detail: {
        payTrxId: 0,
        payTrxCode: '',
        payTrxName: '',
        payGroupFlag: false,
        memberRequestFlag: false,
        accountId: '',
        accountName: '',
        trxFormula: '',
        sortSeq: 0,
        lockId: ''
      },

      isRecordEditorVisible: false,
      recordIndex: -1,
      isAdding: false,

    };
  },

  computed: {
    editorTitle () {
      if (this.isAdding) {
        return 'Add Pay Trx';
      }
      return 'Edit Pay Trx';
    },

    payTrxCodeUpper: {
    get() {
      return this.detail.payTrxCode.toUpperCase();
    },
    set(value) {
      this.detail.payTrxCode = value.toUpperCase();
    }
  }

  },  

  methods: {
    
    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) { 
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      return getList('dbo.QFinAccountCore', 'AccountId, AccountName', '', filter).then(
        acct => {
          if (acct && acct.length) {
            me.detail.accountName = acct[0].accountName;
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

    onAccountIdSearchFill (e) {
      e.filter = "HeaderFlag = 0"     // Level 1 = Regular Acctg Staff, 2 = Acctg Head
    },

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.accountId = data.accountId;
      detail.accountName = data.accountName;

      this.focusNext();

    },

    onPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payTrxCodeCallback;
    },

    payTrxCodeCallback (e) {
      const me = this;

      let index = me.payTrxs.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {
        e.message = 'Pay Trx <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      return true;
    },


    getTargetPath () {
      const
        q = {},
        me = this;

      return {
        name: me.$options.name,
        query: q
      };
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getPayTrxList()
        .then( list => {
          me.stopWait(wait);
          me.payTrxs = list;
          me.setupGridControls();   // 05 Feb 2025 - EMT
          me.isFilled = true;
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })

    },

    refreshOldRefs () {
      const me = this;
      me.oldDetail = JSON.stringify(me.detail);
    },

    // API calls

    getPayTrxList () {
      return get('api/pay-trxs');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/pay-trxs/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/pay-trxs/' + this.detail.payTrxId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        // currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('DELETE');

        // 05 Feb 2025 - EMT (change of signature to fix 404 issue when lockId contains a slash character)
        return ajax('api/pay-trxs/' + detail.payTrxId + this.getDeleteQuery(detail.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        detail = {};

      Object.assign(detail, me.detail);
      return detail;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('detail');
      dc.keyField = 'payTrxId';
      dc.autoAssignKey = true;
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 5 });
        return;
      }

      let
        promise,
        message,
        wait = me.wait();

      if (me.isAdding) {
        promise = me.createRecord();
      } else {
        promise = me.modifyRecord();
      }

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success && success.length) {
            let index = me.payTrxs.findIndex(obj => obj.payTrxId === success[0].payTrxId);
            if (index > -1) {
              me.payTrxs[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.payTrxs.push(success[0]); 
                me.setupDetailControls();  
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.payTrxId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom();
                this.setFocus('payTrxCode');
              }, 100);

            } else {
              message = 'Document updated.'

              me.clearRecordPad();
              me.refreshOldRefs();
            }

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

        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

    },

    onRefresh () {
      this.loadData();
    },

    onShowRecordEditor () {
      const me = this;

      me.setActiveModel('detail');

      me.setRequiredMode(
        'payTrxCode',
        'payTrxName',
        'accountId',
        'sortSeq'
      );  

      me.setOptionalMode(
        'payGroupFlag',
        'memberRequestFlag',
        'trxFormula' 
      );

      me.setDisplayMode(
        'payTrxId',
        'accountName'
      );

      setTimeout(() => {
        this.setFocus('payTrxCode');
      }, 200);
    },

    onHideRecordEditor () {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
    },

    onEditRecord (dtl, index) {
      // hold current property values of selected detail for editing; passed index too.
      const
        me = this,
        d = me.detail;

      me.recordIndex = index;
      me.clearRecordPad();

      d.payTrxId = dtl.payTrxId;
      d.payTrxCode = dtl.payTrxCode;
      d.payTrxName = dtl.payTrxName;
      d.trxFormula = dtl.trxFormula;
      d.payGroupFlag = dtl.payGroupFlag;
      d.memberRequestFlag = dtl.memberRequestFlag;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.payTrxId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.payTrxs[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.payTrxId = dtl.payTrxId;
      d.payTrxCode = dtl.payTrxCode;
      d.payTrxName = dtl.payTrxName;
      d.trxFormula = dtl.trxFormula;
      d.payGroupFlag = dtl.payGroupFlag;
      d.memberRequestFlag = dtl.memberRequestFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsPayTrx', me.detail.payTrxId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Pay Trx <b>' + me.payTrxs[index].payTrxName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.payTrxs.splice(index, 1);
            me.advice.success('Document deleted.', { duration: 4 });
          }
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      let dtl = {};

      if (!me.isValid('dbs0490A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.payTrxs[me.recordIndex];

        dtl.payTrxId = d.payTrxId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.trxFormula = d.trxFormula;
        dtl.payGroupFlag = d.payGroupFlag;
        dtl.memberRequestFlag = d.memberRequestFlag;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;
        dtl.sortSeq = d.sortSeq;

        me.isRecordEditorVisible = false;
        me.onSubmit();
      }

    },

    onCloseDetail () {
      const me = this;

      me.clearRecordPad();
      me.refreshOldRefs();
      me.isRecordEditorVisible = false;
    },

    // helpers

    hasChanges () {
      const me = this;
      if (JSON.stringify(me.detail) !== me.oldDetail) { return true; }
      return false;
    },

    clearRecordPad () {
      const d = this.detail;

      d.payTrxId = 0;
      d.payTrxCode = '';
      d.payTrxName = '';
      d.trxName = '';
      d.payGroupFlag = false;
      d.memberRequestFlag = false;
      d.accountId = '';
      d.accountName = '';
      d.sortSeq = 0;
      d.lockId = '';
    },

    scrollToBottom () {
      const me = this;
      let el = me.$refs.list;
      if (el) {
        me.dom.scrollToBottom(el);
      }
    }

  },

  created () {
    const me = this;
    me.oldDetail = JSON.stringify(me.detail)
  },

  mounted () {
    const me = this;
    me.loadData();
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

.record-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: .4fr .6fr 2fr .4fr .4fr .6fr 1.5fr .4fr;
  gap: .5rem;
  width: 110vw;
}

.fixed-header {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 70vh;
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
.scroller-container{
  overflow: auto;
}
@media (max-width: 1200px) {
  .fixed-header table {
    width: 85%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 95%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0490 >>> .card-body {
    padding: .25rem;
  }
}

</style>