// PhilHealth Contibution Table

<template>
<section class="container p-0 w-64" :key="ts">
<sym-form id="dbs0540" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">


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
      <button type="button" :class="copyButtonClass" class="justify-between btn-copy" @click="onCopy">
        <i class="fa fa-copy fa-lg"></i><span>Copy</span>
      </button>
    </div>
  </div>

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="matrixPhh.phhId" :caption-width="35" :input-width="20" caption="Phil Health ID" lookupId="DbsMatrixPhh" @lostfocus="onPhhIdLostFocus" @changing="onPhhIdChanging" @changed="onPhhIdChanged" @searchresult="onPhhIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>

  <sym-text v-model="matrixPhh.phhName" :caption-width="35" :input-width="80" caption="Description"></sym-text>
  <sym-date v-model="matrixPhh.startDate" :caption-width="35" :input-width="30" caption="Effectivity Date" @changing="onStartDateChanging"></sym-date>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Range Table</span>
  </div>

  <div class="d-flex justify-center fixed-header" ref="list">
    <table class="striped-even mb-0 ">
      <thead>
        <tr class="align-bottom">
          <th class="w-10 text-right">ID</th>
          <th class="w-18 text-right">Minimum Pay</th>
          <th class="w-18 text-right">Maximum Pay</th>
          <th class="w-14 text-right">Rate %</th>
          <th class="w-16 text-right">Fixed Amount</th>
          <th class="w-10">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in details" :key="index">
          <td data-label="Range ID" class="text-right">{{ dtl.phhRangeId }}</td>
          <td data-label="Minimum Pay" class="text-right">{{ core.toDecimalFormat(dtl.minAmount) }}</td>
          <td data-label="Maximum Pay" class="text-right">{{ core.toDecimalFormat(dtl.maxAmount) }}</td>
          <td data-label="Rate %" class="text-right">{{ core.toDecimalFormat(dtl.premiumRate) }}</td>
          <td data-label="Fixed Amount" class="text-right">{{ core.toDecimalFormat(dtl.fixedAmount) }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditRecord(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
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


  <div class="command-buttons light border-main p-1 border-top-0 mb-2">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between btn-add" @click="onAddRecord">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>


  <div class="d-flex justify-center mt-3" v-if="!details.length">
    <sym-alert class="info w-100" icon="none">
      <span>No records found. File is empty.</span>
    </sym-alert>
  </div>

  <sym-modal
    id="record-editor"
    v-model="isRecordEditorVisible"
    size="rg"
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
    <form id="dbs0540A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.phhRangeId" caption="Range ID" align="bottom"></sym-int>
        <sym-dec v-model="detail.minAmount" caption="Minimum Pay" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.maxAmount" caption="Maximum Pay" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.premiumRate" caption="Rate %" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.fixedAmount" caption="Fixed Amount" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
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
  getCount,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0540',

  data () {
    return {
      matrixPhh: {
        phhId: 0,
        phhName: '',
        startDate: null,
        lockId: ''
      },

      details: [],

      detail: {
        phhRangeId: 0,
        minAmount: 0,
        maxAmount: 0,
        premiumRate: 0,
        fixedAmount: 0,
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
        return 'Add Range';
      }
      return 'Edit Range';
    }
  },

  methods: {

    onCopy () {
      const me = this;

      me.dialog.confirm('Create new record based on <b>ID #</b>' + me.matrixPhh.phhId + ' - '+ me.matrixPhh.phhName + '.Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.matrixPhh.phhId = -1;
            me.matrixPhh.startDate = null; 
            me.matrixPhh.phhName = ''; 
          }
          return;
        }
      );

    },

    onStartDateChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.startDateCallback;
    },

    startDateCallback (e) {
      let startDate = e.proposedValue;
      let filter = "StartDate='" + startDate.toDateFormatISO()  + "'";
      return getCount('dbo.DbsMatrixPhh', filter).then(
        count => {
          if (count > 0) {
          e.message = 'Definition for <b>' + startDate + '</b> already exists.';
          return false;
        } else {
          return true;
        }
        },
        () => {
          return true;
        }
      );

    },

    onPhhIdChanging (e) {
      e.callback = this.phhIdCallback;
    },

    phhIdCallback (e) {
      e.message = "PHH ID '<b>" + e.proposedValue + "</b>' not found.";
      return this.isValidEntity('dbo.DbsMatrixPhh', 'phhId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onPhhIdChanged () {
      
      const me = this;
      
      me.loadData();
      me.replaceUrl();
    },

    onPhhIdLostFocus () {
      const me = this;
      
      if (!me.matrixPhh.phhId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onPhhIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.matrixPhh.phhId = data.phhId;
      me.replaceUrl();
      me.loadData();
    },


    loadData () {
      const
        me = this,
        wait = me.wait();

        me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
          }
          if (me.matrixPhh.phhId < 0) {
            return Promise.resolve(null);
          }
          return me.getMatrixPhh();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.matrixPhh.length) {
            me.setModels(info);
          } else {
            if (me.matrixPhh.phhId > -1) {
              let message = "PHH ID '<b>" + me.matrixPhh.phhId + "</b>' not found.";
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

      me.matrixPhh = me.core.convertDates(info.matrixPhh[0]);
      me.details = info.details;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;
        me.oldMatrixPhh = JSON.stringify(me.matrixPhh);
        me.oldDetails = JSON.stringify(me.details);
    },


    // API calls

    getMatrixPhh () {
      return get('api/matrix-phh/' + this.matrixPhh.phhId);
    },

    getReferences () {
      const me = this;

      if (me.payFrequencies.length) {
        return Promise.resolve(true);
      }
      return get('api/references/dbs0530');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

        return ajax('api/matrix-phh/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

        return ajax('api/matrix-phh/' + this.matrixPhh.phhId + '/' + currentUserId, options);    

    },

    deleteRecord () {
      const
        matrixPhh = this.matrixPhh,
        options = this.core.getAjaxOptions('DELETE');
      
        return ajax('api/matrix-phh/' + matrixPhh.phhId + this.getDeleteQuery(matrixPhh.lockId), options);
        
    },

    getApiPayload () {
      const
        me = this,
        matrixPhh = {};

        Object.assign(matrixPhh, me.matrixPhh);
        matrixPhh.details = me.details;
      
        return matrixPhh;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

        dc.models.push('matrixPhh', 'details','detail');
        dc.keyField = 'phhId';

      dc.autoAssignKey = true;
    },

    onSubmit (nextRoute) {
      const me = this;
      
      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
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
              me.matrixPhh.phhId = success;
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

      getSafeDeleteFlag('DbsMatrixPhh', me.matrixPhh.phhId)
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
    onShowRecordEditor () {
      const me = this;

      me.setActiveModel('detail');

      me.setOptionalMode(
        'minAmount',
        'maxAmount',
        'premiumRate',
        'fixedAmount'
      );

      me.setDisplayMode(
        'phhRangeId',
      );

      setTimeout(() => {
        this.setFocus('minAmount');
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

      d.phhRangeId = dtl.phhRangeId;
      d.minAmount = dtl.minAmount;
      d.maxAmount = dtl.maxAmount;
      d.premiumRate = dtl.premiumRate;
      d.fixedAmount = dtl.fixedAmount;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.phhRangeId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.details[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.phhRangeId = dtl.phhRangeId;
      d.minAmount = dtl.minAmount;
      d.maxAmount = dtl.maxAmount;
      d.premiumRate = dtl.premiumRate;
      d.fixedAmount = dtl.fixedAmount;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsMatrixPhhDetail', me.detail.phhRangeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Range <b>' + me.details[index].phhRangeId + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.details.splice(index, 1);
          }
          return false;
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

      if (!me.isValid('dbs0540A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        Object.assign(dtl, d);
        me.details.push(dtl);
        me.clearRecordPad();
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('phhRangeId');
        }, 100);


      } else {
        dtl = me.details[me.recordIndex];

        dtl.phhRangeId = d.phhRangeId;
        dtl.minAmount = d.minAmount;
        dtl.maxAmount = d.maxAmount;
        dtl.premiumRate = d.premiumRate;
        dtl.fixedAmount = d.fixedAmount;
        dtl.lockId = d.lockId;

        me.isRecordEditorVisible = false;
      }

    },

    onCloseDetail () {
      const me = this;

      me.clearRecordPad();
      me.refreshOldRefs();
      me.isRecordEditorVisible = false;
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
          'phhName',
          'startDate'
        );

        me.setFocus('phhName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.matrixPhh) !== me.oldMatrixPhh) { return true; }
      if (JSON.stringify(me.details) !== me.oldDetails) { return true; }

    },


    clearRecordPad () {
      const d = this.detail;

      d.phhRangeId = 0;
      d.minAmount = 0;
      d.maxAmount = 0;
      d.premiumRate = 0;
      d.fixedAmount = 0;
      d.lockId = '';
    }

  },


  created () {
    const me = this;

    me.oldMatrixPhh = '';
    me.oldDetails = '';
    me.payFrequencies = [];   

  },
}

</script>

<style scoped>

.command-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.action-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.record-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 3fr 5fr 5fr 3fr 4fr;
  gap: .5rem;
}

#record-editor >>> .modal-rg {
  min-width: 56%;
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
  display: flex;
  justify-content: space-evenly;
  flex-wrap: nowrap;
}


</style>