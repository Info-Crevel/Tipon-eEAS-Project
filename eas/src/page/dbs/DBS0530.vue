// Pag-IBIG Contribution Table

<template>
<section class="container p-0 w-60" :key="ts">
<sym-form id="dbs0530" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <sym-int v-model="matrixPbg.pbgId" :caption-width="35" :input-width="20" caption="Pbg ID" lookupId="DbsMatrixPbg" @lostfocus="onPbgIdLostFocus" @changing="onPbgIdChanging" @changed="onPbgIdChanged" @searchresult="onPbgIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>

  <sym-text v-model="matrixPbg.pbgName" :caption-width="35" :input-width="80" caption="Description"></sym-text>
  <sym-date v-model="matrixPbg.startDate" :caption-width="35" :input-width="30" caption="Effectivity Date" @changing="onStartDateChanging"></sym-date>

  <div class="text-center border-light curved p-1 slategray mt-2 ">
    <span class="serif lg-3">Range Table</span>
  </div>

  <div class="d-flex justify-center fixed-header" ref="list">
    <table class="striped-even mb-0">
      <thead>
        <tr class="align-bottom">
          <th class="w-7 text-right">ID</th>
          <th class="w-18 text-right">Minimum Pay</th>
          <th class="w-18 text-right">Maximum Pay</th>
          <th class="w-15 text-right">Employee %</th>
          <th class="w-15 text-right">Employer %</th>
          <th class="w-15 text-right">Maximum Rate</th>
          <th class="w-13">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in details" :key="index">
          <td data-label="Range ID" class="text-right">{{ dtl.pbgRangeId }}</td>
          <td data-label="Minimum Pay" class="text-right">{{ core.toDecimalFormat(dtl.minAmount) }}</td>
          <td data-label="Maximum Pay" class="text-right">{{ core.toDecimalFormat(dtl.maxAmount) }}</td>
          <td data-label="Employee %" class="text-right">{{ core.toDecimalFormat(dtl.employeeRate) }}</td>
          <td data-label="Employer %" class="text-right">{{ core.toDecimalFormat(dtl.employerRate) }}</td>
          <td data-label="Maximum Pay" class="text-right">{{ core.toDecimalFormat(dtl.maxRate) }}</td>
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
    size="sm"
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
    <form id="dbs0530A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.pbgRangeId" caption="Range ID" align="bottom"></sym-int>
        <sym-dec v-model="detail.minAmount" caption="Minimum Pay" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.maxAmount" caption="Maximum Pay" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.employeeRate" caption="Employee %" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.employerRate" caption="Employer %" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
        <sym-dec v-model="detail.maxRate" caption="Maximum Rate" align="bottom" captionAlign="right" :blankZero="false"></sym-dec>
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
  name: 'dbs0530',

  data () {
    return {
      // ranges: [],

      matrixPbg: {
        pbgId: 0,
        pbgName: '',
        startDate: null,
        lockId: ''
      },

      details: [],

      detail: {
        pbgRangeId: 0,
        minAmount: 0,
        maxAmount: 0,
        employeeRate: 0,
        employerRate: 0,
        maxRate: 0,
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

      me.dialog.confirm('Create new record based on <b>ID #</b>' + me.matrixPbg.pbgId + ' - '+ me.matrixPbg.pbgName + '.Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.matrixPbg.pbgId = -1;
            me.matrixPbg.startDate = null; 
            me.matrixPbg.pbgName = ''; 
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
      return getCount('dbo.DbsMatrixPbg', filter).then(
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

    onPbgIdChanging (e) {
      e.callback = this.pbgIdCallback;
    },

    pbgIdCallback (e) {
      e.message = "PBG ID '<b>" + e.proposedValue + "</b>' not found.";
      return this.isValidEntity('dbo.DbsMatrixPbg', 'pbgId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onPbgIdChanged () {
      
      const me = this;
      
      me.loadData();
      me.replaceUrl();
    },

    onPbgIdLostFocus () {
      const me = this;
      
      if (!me.matrixPbg.pbgId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onPbgIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.matrixPbg.pbgId = data.pbgId;
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
          if (me.matrixPbg.pbgId < 0) {
            return Promise.resolve(null);
          }
          return me.getMatrixPbg();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.matrixPbg.length) {
            me.setModels(info);
          } else {
            if (me.matrixPbg.pbgId > -1) {
              let message = "PBG ID '<b>" + me.matrixPbg.pbgId + "</b>' not found.";
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

      me.matrixPbg = me.core.convertDates(info.matrixPbg[0]);
      me.details = info.details;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;
        me.oldMatrixPbg = JSON.stringify(me.matrixPbg);
        me.oldDetails = JSON.stringify(me.details);
    },


    // API calls

    getMatrixPbg () {
      return get('api/matrix-pbg/' + this.matrixPbg.pbgId);
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

        return ajax('api/matrix-pbg/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

        return ajax('api/matrix-pbg/' + this.matrixPbg.pbgId + '/' + currentUserId, options);    

    },

    deleteRecord () {
      const
        matrixPbg = this.matrixPbg,
        options = this.core.getAjaxOptions('DELETE');
      
        return ajax('api/matrix-pbg/' + matrixPbg.pbgId + this.getDeleteQuery(matrixPbg.lockId), options);
        
    },

    getApiPayload () {
      const
        me = this,
        matrixPbg = {};

        Object.assign(matrixPbg, me.matrixPbg);
        matrixPbg.details = me.details;
      
        return matrixPbg;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

        dc.models.push('matrixPbg', 'details','detail');
        dc.keyField = 'pbgId';

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
              me.matrixPbg.pbgId = success;
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

      getSafeDeleteFlag('DbsMatrixPbg', me.matrixPbg.pbgId)
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

      me.setRequiredMode(
        'employeeRate',
        'employerRate'
      );

      me.setOptionalMode(
        'minAmount',
        'maxAmount',
        'maxRate'
      );

      me.setDisplayMode(
        'pbgRangeId',
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

      d.pbgRangeId = dtl.pbgRangeId;
      d.minAmount = dtl.minAmount;
      d.maxAmount = dtl.maxAmount;
      d.employeeRate = dtl.employeeRate;
      d.employerRate = dtl.employerRate;
      d.maxRate = dtl.maxRate;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.pbgRangeId = -1;
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

      d.pbgRangeId = dtl.pbgRangeId;
      d.minAmount = dtl.minAmount;
      d.maxAmount = dtl.maxAmount;
      d.employeeRate = dtl.employeeRate;
      d.employerRate = dtl.employerRate;
      d.maxRate = dtl.maxRate;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsMatrixPbgDetail', me.detail.pbgRangeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Range <b>' + me.details[index].pbgRangeId + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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

      if (!me.isValid('dbs0530A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {

        Object.assign(dtl, d);
        me.details.push(dtl);
        me.clearRecordPad();
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('pbgRangeId');
        }, 100);


      } else {
        dtl = me.details[me.recordIndex];

        dtl.pbgRangeId = d.pbgRangeId;
        dtl.minAmount = d.minAmount;
        dtl.maxAmount = d.maxAmount;
        dtl.employeeRate = d.employeeRate;
        dtl.employerRate = d.employerRate;
        dtl.maxRate = d.maxRate;
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
          'pbgName',
          'startDate'
        );

        me.setFocus('pbgName');
      }, 100);

    },


    hasChanges () {
      const me = this;

      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.matrixPbg) !== me.oldMatrixPbg) { return true; }
      if (JSON.stringify(me.details) !== me.oldDetails) { return true; }

    },

    clearRecordPad () {
      const d = this.detail;

      d.pbgRangeId = 0;
      d.minAmount = 0;
      d.maxAmount = 0;
      d.employeeRate = 0;
      d.employerRate = 0;
      d.maxRate = 0;
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldMatrixPbg = '';
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
  grid-template-columns: 3fr 4fr 4fr 3fr 3fr;
  gap: .5rem;
}

#record-editor >>> .modal-sm {
  min-width: 50%;
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
  display: flex;
  justify-content: space-evenly;
}

@media (max-width: 1200px) {
  #record-editor >>> .modal-sm {
    min-width: 60%;
  }
}

@media (max-width: 1100px) {
  .fixed-header table {
    width: 75%;
  }
}

@media (max-width: 1000px) {
  #record-editor >>> .modal-sm {
    min-width: 70%;
  }
}

@media (max-width: 900px) {
  #record-editor >>> .modal-sm {
    min-width: 80%;
  }

  .fixed-header table {
    width: 90%;
  }
}

@media (max-width: 700px) {
  #record-editor >>> .modal-sm {
    min-width: 98%;
  }

  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0530 >>> .card-body {
    padding: .25rem;
  }
  #dbs0530A {
    padding: .5rem !important;
  }
}

@media (max-width: 500px) {
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