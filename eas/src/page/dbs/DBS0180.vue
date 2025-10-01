// Charging Consideration Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0180" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <table class="striped-even mb-0 w-50">
      <thead>
        <tr>
          <th class="w-10 text-right">ID</th>
          <th class="w-50">Charging Consideration Name</th>
          <th class="w-14 text-right">Sort Seq</th>
          <th class="w-26">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in chargingConsiderations" :key="index">
          <td data-label="Charging Consideration ID" class="text-right">{{ dtl.chargingConsiderationId }}</td>
          <td data-label="Charging Consideration Name">{{ dtl.chargingConsiderationName }}</td>
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

  <div class="d-flex justify-center mt-3" v-if="!chargingConsiderations.length">
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
    <form id="dbs0180A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.chargingConsiderationId" caption="Charging Consideration ID" align="bottom"></sym-int>
        <sym-text v-model="detail.chargingConsiderationName" caption="Charging Consideration Name" align="bottom"></sym-text>
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
  name: 'dbs0180',

  data () {
    return {
      chargingConsiderations: [],

      detail: {
        chargingConsiderationId: 0,
        chargingConsiderationName: '',
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
        return 'Add Charging Consideration';
      }
      return 'Edit Charging Consideration';
    }
  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getChargingConsiderationList()
        .then( list => {
          me.stopWait(wait);
          me.chargingConsiderations = list;
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

    getChargingConsiderationList () {
      return get('api/charging-considerations');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/charging-considerations/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/charging-considerations/' + this.detail.chargingConsiderationId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/charging-considerations/' + detail.chargingConsiderationId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'chargingConsiderationId';
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
            let index = me.chargingConsiderations.findIndex(obj => obj.chargingConsiderationId === success[0].chargingConsiderationId);
            if (index > -1) {
              me.chargingConsiderations[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.chargingConsiderations.push(success[0]);
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.chargingConsiderationId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('chargingConsiderationName');
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
        'chargingConsiderationName',
        'sortSeq'
      );

      me.setDisplayMode(
        'chargingConsiderationId',
      );

      setTimeout(() => {
        this.setFocus('chargingConsiderationName');
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

      d.chargingConsiderationId = dtl.chargingConsiderationId;
      d.chargingConsiderationName = dtl.chargingConsiderationName;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.chargingConsiderationId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.chargingConsiderations[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.chargingConsiderationId = dtl.chargingConsiderationId;
      d.chargingConsiderationName = dtl.chargingConsiderationName;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsChargingConsideration', me.detail.chargingConsiderationId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Charging Consideration <b>' + me.chargingConsiderations[index].chargingConsiderationName + '</b> will be deleted.<br><br>Continue?', { size: 'md', scheme: 'warning' });
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
            me.chargingConsiderations.splice(index, 1);
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

      if (!me.isValid('dbs0180A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.chargingConsiderations[me.recordIndex];

        dtl.chargingConsiderationId = d.chargingConsiderationId;
        dtl.chargingConsiderationName = d.chargingConsiderationName;
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

      d.chargingConsiderationId = 0;
      d.chargingConsiderationName = '';
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
  grid-template-columns: 4fr 5fr 2fr;
  gap: .5rem;
}

#record-editor >>> .modal-sm {
  min-width: 45%;
}

.fixed-header {
  /* overflow-y: auto; */
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

@media (max-width: 1100px) {
  .fixed-header table {
    width: 65%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 80%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 90%;
  }
}

@media (max-width: 600px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0180 >>> .card-body {
    padding: .25rem;
  }
}

@media (max-width: 500px) {
  #record-editor >>> .modal-sm {
    min-width: 98%;
  }

  #dbs0180A {
    padding: .5rem !important;
  }

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
		/* Behave like a "row" */
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