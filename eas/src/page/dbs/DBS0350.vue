// Deminimis Setup

<template>
<section class="container p-0 w-60" :key="ts">
<sym-form id="dbs0350" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
        <tr class="align-top">
          <th class="w-9 text-right">ID</th>
          <th class="w-40">Deminimis Name</th>
          <th class="w-40">Transaction Name</th>
          <th class="w-10 text-center">Tax</th>
          <th class="w-10 text-center">Hourly</th>
          <th class="w-25 text-center">Maximum Amount</th>
          <th class="w-15 text-right">Sort Seq</th>
          <th class="w-20">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in deminimis" :key="index">
          <td data-label="Deminimis ID" class="text-right">{{ dtl.deminimisId }}</td>
          <td data-label="Deminimis Name">{{ dtl.deminimisName }}</td>
          <td data-label="Transaction">{{ dtl.payTrxName }}</td>
          <td data-label="Tax" class="text-center">
            <i :class="getBooleanIconClass(dtl.taxFlag)"></i>
          </td>
          <td data-label="Hourly" class="text-center">
            <i :class="getBooleanIconClass(dtl.payHourly)"></i>
          </td>
          <td data-label="Transaction" class="text-right">{{ dtl.payMaxAmount }}</td>
          <td data-label="Sort Seq" class="text-right">{{ dtl.sortSeq }}</td>
          <td class="p-1">
            <div class="d-flex justify-center gap" sm-1 mb-0>
              <button type="button" class="justify-between info-light  btn-dtl-edit" title="Edit" @click="onEditRecord(dtl, index)">
                <i class="fa fa-edit fa-lg"></i>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteRecord(index)">
                <i class="fa fa-times fa-lg"></i>
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <div class="d-flex justify-center mt-3" v-if="!deminimis.length">
    <sym-alert class="info w-100 text-center" icon="none">
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
    :dismisible="false"
    :closeOnBackButton="false"
    :backdrop="false"
    :title="editorTitle"
    @show="onShowRecordEditor($event)"
    @hide="onHideRecordEditor($event)"
    headerClass="app-form-header"
    dismisButtonClass="danger"
  >

  <div class="board p-1 mb-0 w-100">
    <form id="dbs0350A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.deminimisId" caption="Deminimis ID" align="bottom"></sym-int>
        <sym-text v-model="detail.deminimisName" caption="Deminimis Name" align="bottom"></sym-text>
        <sym-combo v-model="detail.payTrxCode" caption="Transaction" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onPayTrxCodeChanging" ></sym-combo>

        <sym-check v-model="detail.taxFlag" caption="Tax" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-check v-model="detail.payHourly" caption="Hourly" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-int v-model="detail.payMaxAmount" caption="Maximum Amount" align="bottom"  caption-align="center" check-align="center"></sym-int>
        <sym-int v-model="detail.sortSeq" caption="Sort Seq" align="bottom" captionAlign="right"></sym-int>
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

export default {
  extends: PageMaintenance,
  name: 'dbs0350',

  data () {
    return {
      deminimis: [],

      detail: {
        deminimisId: 0,
        deminimisName: '',
        payTrxCode: '',
        payTrxName: '',
        taxFlag: false,
        payHourly: false, 
        payMaxAmount: 0,
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
        return 'Add Deminimis';
      }
      return 'Edit Deminimis';
    }
  },

  methods: {

    onPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payTrxCodeCallback;
    },

    payTrxCodeCallback (e) {
      const me = this;

      let index = me.deminimis.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {
        e.message = 'Pay Trx <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      return true;
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getDeminimisList()
        .then( list => {
          me.stopWait(wait);

          me.deminimis = list.deminimis;
          me.payTrx = list.payTrx;

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

    getDeminimisList () {
      return get('api/deminimis');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/deminimis/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/deminimis/' + this.detail.deminimisId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/deminimis/' + detail.deminimisId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'deminimisId';
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
            let index = me.deminimis.findIndex(obj => obj.deminimisId === success[0].deminimisId);
            if (index > -1) {
              me.deminimis[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.deminimis.push(success[0]);
                me.setupDetailControls();   
              }
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.deminimisId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('deminimisName');
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
        'deminimisName',
        'payTrxCode',
        'sortSeq'
      );

      me.setOptionalMode(
        'taxFlag',
        'payHourly',
        'payMaxAmount'
      );

      me.setDisplayMode(
        'deminimisId',
      );

      setTimeout(() => {
        this.setFocus('deminimisName');
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

      d.deminimisId = dtl.deminimisId;
      d.deminimisName = dtl.deminimisName;
      d.payTrxCode = dtl.payTrxCode;
      d.payTrxName = dtl.payTrxName;
      d.taxFlag = dtl.taxFlag;
      d.payHourly = dtl.payHourly;
      d.payMaxAmount = dtl.payMaxAmount;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.deminimisId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.deminimis[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.deminimisId = dtl.deminimisId;
      d.deminimisName = dtl.deminimisName;
      d.payTrxCode = dtl.payTrxCode;
      d.payTrxName = dtl.payTrxName;
      d.taxFlag = dtl.taxFlag;
      d.payHourly = dtl.payHourly;
      d.payMaxAmount = dtl.payMaxAmount;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsDeminimis', me.detail.deminimisId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Deminimis <b>' + me.deminimis[index].deminimisName + '</b> will be deleted.<br><br>Continue?', { size: 'md', scheme: 'warning' });
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
            me.deminimis.splice(index, 1);
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

      if (!me.isValid('dbs0350A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.deminimis[me.recordIndex];

        dtl.deminimisId = d.deminimisId;
        dtl.deminimisName = d.deminimisName;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.taxFlag = d.taxFlag;
        dtl.payHourly = d.payHourly;
        dtl.payMaxAmount = d.payMaxAmount;        
        dtl.sortSeq = d.sortSeq;

        me.payTrx.forEach((payTrx) => {
          if (payTrx.payTrxCode == d.payTrxCode) {
            dtl.payTrxName = payTrx.payTrxName;
          }
        });

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

      d.deminimisId = 0;
      d.deminimisName = '';
      d.payTrxCode = '';
      d.payTrxName = '';
      d.taxFlag = false;
      d.payHourly = false;
      d.payMaxAmount = 0;
      d.sortSeq = 0;
      d.lockId = '';
    }

  },

  created () {
    const me = this;
    me.oldDetail = JSON.stringify(me.detail);
    me.payTrx = [];

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
  grid-template-columns: 1.4fr 4fr 4fr 1fr 1fr 2fr 1fr;
  gap: .5rem;
}

.fixed-header {
  overflow: auto;
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

@media (max-width: 1200px) {
  .fixed-header table {
    width: 80%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 90%;
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
  #dbs0350 >>> .card-body {
    padding: .25rem;
  }
}

@media (max-width: 640px) {
  .fixed-header table,
  .fixed-header thead,
  .fixed-header tbody,
  .fixed-header th,
  .fixed-header td,
  .fixed-header tr {
    display: block;
  }

  .fixed-header thead tr {
		position: absolute;
		top: -9999px;
		left: -9999px;
	}

  .fixed-header td {
		position: relative;
		padding-left: 50%;
		white-space: normal;
		text-align: left !important;
	}

  .fixed-header td:before {
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

</style>