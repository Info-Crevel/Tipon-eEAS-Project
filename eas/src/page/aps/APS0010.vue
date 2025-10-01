// Payee Type Setup

<template>
  <section class="container p-0 w-100" :key="ts">
  <sym-form id="aps0010" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
  
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
          <th class="w-30">Payee Type Name</th>
          <th class="w-10">Term</th>
          <th class="w-10">Account ID</th>
          <th class="w-30">Account Name</th>
          <th class="w-7">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in payeeTypes" :key="index">
          <td class="text-right">{{ dtl.payeeTypeId }}</td>
          <td>{{ dtl.payeeTypeName }}</td>
          <td>{{ dtl.payableTermName }}</td>
          <td>{{ dtl.accountId }}</td>
          <td>{{ dtl.accountName }}</td>
       
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

  <div class="d-flex justify-center mt-3" v-if="!payeeTypes.length">
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
    <form id="aps0010A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="scroller-container">
        <div class="record-editor-boxes">
          <sym-int v-model="detail.payeeTypeId" caption="ID" align="bottom" :caption-width="100"></sym-int>
          <sym-text v-model="detail.payeeTypeName" caption="Payee Type Name" align="bottom" @changing="onPayeeTypeNameChanging"></sym-text>
          <sym-combo v-model="detail.payableTermId" caption="Term" align="bottom" display-field="payableTermName" :datasource="terms"></sym-combo>


          <sym-text v-model="detail.accountId" :caption-width="30" align="bottom" caption="Account ID" lookupId="FinAccountCore" @changing="onAccountIdChanging" @searchfill="onAccountIdSearchFill" @searchresult="onAccountIdSearchResult"></sym-text>
          <sym-text v-model="detail.accountName" caption="Account Name" :caption-width="35" align="bottom" ></sym-text>          
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
  getSafeDeleteFlag,
  getList
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymDecimal },
  extends: PageMaintenance,
  name: 'aps0010',

  data () {
    return {
      payeeTypes: [],

      detail: {
        payeeTypeId: 0,
        payeeTypeName: '',
        payableTermId: 0,
        payableTermName: '',
        accountId: '',
        accountName: '',
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
        return 'Add Payee Type';
      }
      return 'Edit Payee Type';
    },

  },  

  methods: {
       
 
    onPayeeTypeNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payeeTypeNameCallback;
    },

    payeeTypeNameCallback (e) {
      const me = this;

      let index = me.payeeTypes.findIndex(obj => obj.payeeTypeName === e.proposedValue);
      if (index > -1) {
        e.message = 'Payee Type Name <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      return true;
    },

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
      e.filter = "HeaderFlag = 0"     
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

      me.getPayeeTypeList()
        .then( list => {
          me.stopWait(wait);
          me.payeeTypes = list.payeeTypes;
          me.terms = list.terms;
          me.setupGridControls();  
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

    getPayeeTypeList () {
      return get('api/payee-types');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/payee-types/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/payee-types/' + this.detail.payeeTypeId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');
        return ajax('api/payee-types/' + detail.payeeTypeId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'payeeTypeId';
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
            let index = me.payeeTypes.findIndex(obj => obj.payeeTypeId === success[0].payeeTypeId);
            if (index > -1) {
              me.payeeTypes[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.payeeTypes.push(success[0]); 
                me.setupDetailControls(); 
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.payeeTypeId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom();
                this.setFocus('payeeTypeName');
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
        'payeeTypeName',
        'payableTermId',
        'accountId'
      );  

      me.setDisplayMode(
        'payeeTypeId',
        'accountName'
      );

      setTimeout(() => {
        this.setFocus('payeeTypeName');
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

      d.payeeTypeId = dtl.payeeTypeId;
      d.payeeTypeName = dtl.payeeTypeName;
      d.payableTermId = dtl.payableTermId;
      d.payableTermName = dtl.payableTermName;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.payeeTypeId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.payeeTypes[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.payeeTypeId = dtl.payeeTypeId;
      d.payeeTypeName = dtl.payeeTypeName;
      d.payableTermId = dtl.payableTermId;
      d.payableTermName = dtl.payableTermName;

      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('ApsPayeeType', me.detail.payeeTypeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Payee Type <b>' + me.payeeTypes[index].payeeTypeName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.payeeTypes.splice(index, 1);
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

      if (!me.isValid('aps0010A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }


      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.payeeTypes[me.recordIndex];

        me.terms.forEach((term) => {
          if (term.payableTermId == d.payableTermId) {
            dtl.payableTermName = term.payableTermName;
          }
        });

        dtl.payeeTypeId = d.payeeTypeId;
        dtl.payeeTypeName = d.payeeTypeName;
        dtl.payableTermId = d.payableTermId;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;

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

      d.payeeTypeId = 0;
      d.payeeTypeName = '';
      d.payableTermId = 0;
      d.payableTermName = '';
      d.accountId = '';
      d.accountName = '';
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
    me.oldDetail = JSON.stringify(me.detail);
    me.terms = [];

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
  grid-template-columns: .2fr .6fr .3fr .4fr  1fr ;
  gap: .5rem;
  
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

  #aps0010 >>> .card-body {
    padding: .25rem;
  }
}

</style>