// Day Type Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0380" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <table class="striped-even mb-0 w-65">
      <thead>
        <tr>
          <th class="w-6 text-right">ID</th>
          <th class="w-1O">Code</th>
          <th class="w-31">Day Type Name</th>
          <th class="w-12 text-right">Percentage</th>
          <th class="w-11 text-center">Night Diff</th>
          <th class="w-10 text-right">Sort Seq</th>
          <th class="w-20">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in dayTypes" :key="index">
          <td data-label="Day Type ID" class="text-right">{{ dtl.dayTypeId }}</td>
          <td data-label="Day Type Code">{{ dtl.dayTypeCode }}</td>
          <td data-label="Day Type Name">{{ dtl.dayTypeName }}</td>
          <td data-label="Percentage" class="text-right">{{ core.toDecimalFormat(dtl.premiumPercentage) }}</td>
          <td data-label="Night Differential" class="text-center">
            <i :class="getBooleanIconClass(dtl.nightDifferentialFlag)"></i>
          </td>
          <td data-label="Sort Seq" class="text-right">{{ dtl.sortSeq }}</td>
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

  <div class="d-flex justify-center mt-3" v-if="!dayTypes.length">
    <sym-alert class="info w-100 text-center" icon="none">
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
    <form id="dbs0380A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.dayTypeId" caption="Day Type ID" align="bottom"></sym-int>
        <sym-text v-model="detail.dayTypeCode" caption="Code" align="bottom" @changing="onDayTypeCodeChanging"></sym-text>
        <sym-text v-model="detail.dayTypeName" caption="Day Type Name" align="bottom"></sym-text>
        <sym-dec v-model="detail.premiumPercentage" caption="Percentage" align="bottom" caption-align="right"></sym-dec>
        <sym-check v-model="detail.nightDifferentialFlag" caption="Night Diff" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
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
  name: 'dbs0380',

  data () {
    return {
      dayTypes: [],

      detail: {
        dayTypeId: 0,
        dayTypeCode: '',
        dayTypeName: '',
        premiumPercentage: 0,
        nightDifferentialFlag: false,
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
        return 'Add Day Type';
      }
      return 'Edit Day Type';
    }
  },

  methods: {
    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getDayTypeList()
        .then( list => {
          me.stopWait(wait);
          me.dayTypes = list;
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

    getDayTypeList () {
      return get('api/day-types');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/day-types/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/day-types/' + this.detail.dayTypeId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/day-types/' + detail.dayTypeId + this.getDeleteQuery(detail.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        detail = {};

      Object.assign(detail, me.detail);
      return detail;
    },

    // event handlers

    onDayTypeCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.dayTypeCodeCallback;
    },

    dayTypeCodeCallback (e) {
      const me = this;

      let index = me.dayTypes.findIndex(obj => obj.dayTypeCode === e.proposedValue);
      if (index > -1) {
        e.message = 'Day Code <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      return true;
    },

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('detail');
      dc.keyField = 'dayTypeId';
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
            let index = me.dayTypes.findIndex(obj => obj.dayTypeId === success[0].dayTypeId);
            if (index > -1) {
              me.dayTypes[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.dayTypes.push(success[0]);
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.dayTypeId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('dayTypeCode');
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
        'dayTypeCode',
        'dayTypeName',
        'premiumPercentage',
        'sortSeq'
      );

      me.setOptionalMode(
        'nightDifferentialFlag'
      );

      me.setDisplayMode(
        'dayTypeId',
      );

      setTimeout(() => {
        this.setFocus('dayTypeCode');
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

      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeCode = dtl.dayTypeCode;
      d.dayTypeName = dtl.dayTypeName;
      d.premiumPercentage = dtl.premiumPercentage;
      d.nightDifferentialFlag = dtl.nightDifferentialFlag;

      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.dayTypeId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.dayTypes[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeCode = dtl.dayTypeCode;
      d.dayTypeName = dtl.dayTypeName;
      d.premiumPercentage = dtl.premiumPercentage;
      d.nightDifferentialFlag = dtl.nightDifferentialFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsDayType', me.detail.dayTypeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Day Type <b>' + me.dayTypes[index].dayTypeName + '</b> will be deleted.<br><br>Continue?', { size: 'md', scheme: 'warning' });
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
            me.dayTypes.splice(index, 1);
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

      if (!me.isValid('dbs0380A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.dayTypes[me.recordIndex];

        dtl.dayTypeId = d.dayTypeId;
        dtl.dayTypeCode = d.dayTypeCode;
        dtl.dayTypeName = d.dayTypeName;
        dtl.premiumPercentage = d.premiumPercentage;
        dtl.nightDifferentialFlag = d.nightDifferentialFlag;

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

      d.dayTypeId = 0;
      d.dayTypeCode = '';
      d.dayTypeName = '';
      d.premiumPercentage = 0;
      d.nightDifferentialFlag = false;
      d.sortSeq = 0;
      d.lockId = '';
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
  grid-template-columns: 2fr 2fr 4fr 2fr 2fr 2fr;
  gap: .5rem;
}

#record-editor >>> .modal-sm {
  min-width: 55%;
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
  #record-editor >>> .modal-sm {
    min-width: 65%;
  }

  .fixed-header table {
    width: 80%;
  }
}

@media (max-width: 1100px) {
  .fixed-header table {
    width: 90%;
  }
}

@media (max-width: 1000px) {
  #record-editor >>> .modal-sm {
    min-width: 75%;
  }
}

@media (max-width: 900px) {
  #record-editor >>> .modal-sm {
    min-width: 85%;
  }

  .fixed-header table {
    width: 98%;
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
  #dbs0380 >>> .card-body {
    padding: .25rem;
  }
}

@media (max-width: 700px) {
  #record-editor >>> .modal-sm {
    min-width: 100%;
  }

  /* reduce padding of record editor form */
  #dbs0380A {
    padding: .5rem !important;
  }
}

@media (max-width: 610px) {
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