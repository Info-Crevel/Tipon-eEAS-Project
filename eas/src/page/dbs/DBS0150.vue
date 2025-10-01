// License Profession Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0150" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <table class="striped-even mb-0 w-60">
      <thead>
        <tr class="align-top">
          <th class="w-9 text-right">ID</th>
          <th class="w-50">License Profession Name</th>
          <th class="w-10 text-center">Upload</th>
          <th class="w-11 text-right">Sort Seq</th>
          <th class="w-20">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in licenseProfessions" :key="index">
          <td data-label="License Profession ID" class="text-right">{{ dtl.licenseProfessionId }}</td>
          <td data-label="License Profession Name">{{ dtl.licenseProfessionName }}</td>
          <td data-label="Upload" class="text-center">
            <i :class="getBooleanIconClass(dtl.uploadRequiredFlag)"></i>
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

  <div class="d-flex justify-center mt-3" v-if="!licenseProfessions.length">
    <sym-alert class="info w-100 text-center" icon="none">
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
    <form id="dbs0150A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.licenseProfessionId" caption="License Profession ID" align="bottom"></sym-int>
        <sym-text v-model="detail.licenseProfessionName" caption="License Profession Name" align="bottom"></sym-text>
        <sym-check v-model="detail.uploadRequiredFlag" caption="Upload" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
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
  name: 'dbs0150',

  data () {
    return {
      licenseProfessions: [],

      detail: {
        licenseProfessionId: 0,
        licenseProfessionName: '',
        uploadRequiredFlag: false,
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
        return 'Add License Profession';
      }
      return 'Edit License Profession';
    }
  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getLicenseProfessionList()
        .then( list => {
          me.stopWait(wait);
          me.licenseProfessions = list;
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

    getLicenseProfessionList () {
      return get('api/license-professions');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/license-professions/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/license-professions/' + this.detail.licenseProfessionId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/license-professions/' + detail.licenseProfessionId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'licenseProfessionId';
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
            let index = me.licenseProfessions.findIndex(obj => obj.licenseProfessionId === success[0].licenseProfessionId);
            if (index > -1) {
              me.licenseProfessions[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.licenseProfessions.push(success[0]);
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.licenseProfessionId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('licenseProfessionName');
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
        'licenseProfessionName',
        'sortSeq'
      );

      me.setOptionalMode(
        'uploadRequiredFlag'
      );

      me.setDisplayMode(
        'licenseProfessionId',
      );

      setTimeout(() => {
        this.setFocus('licenseProfessionName');
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

      d.licenseProfessionId = dtl.licenseProfessionId;
      d.licenseProfessionName = dtl.licenseProfessionName;
      d.uploadRequiredFlag = dtl.uploadRequiredFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.licenseProfessionId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.licenseProfessions[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.licenseProfessionId = dtl.licenseProfessionId;
      d.licenseProfessionName = dtl.licenseProfessionName;
      d.uploadRequiredFlag = dtl.uploadRequiredFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsLicenseProfession', me.detail.licenseProfessionId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('License Profession <b>' + me.licenseProfessions[index].licenseProfessionName + '</b> will be deleted.<br><br>Continue?', { size: 'md', scheme: 'warning' });
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
            me.licenseProfessions.splice(index, 1);
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

      if (!me.isValid('dbs0150A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.licenseProfessions[me.recordIndex];

        dtl.licenseProfessionId = d.licenseProfessionId;
        dtl.licenseProfessionName = d.licenseProfessionName;
        dtl.uploadRequiredFlag = d.uploadRequiredFlag;
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

      d.licenseProfessionId = 0;
      d.licenseProfessionName = '';
      d.uploadRequiredFlag = false;
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
  grid-template-columns: 6fr 11fr 1fr 3fr;
  gap: .5rem;
}

#record-editor >>> .modal-rg {
  min-width: 60%;
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

@media (max-width: 1200px) {
  .fixed-header table {
    width: 80%;
  }
}

@media (max-width: 1100px) {
  #record-editor >>> .modal-rg {
    min-width: 80%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 90%;
  }
}

@media (max-width: 880px) {
  #record-editor >>> .modal-rg {
    min-width: 90%;
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

  #dbs0150 >>> .card-body {
    padding: .25rem;
  }
}

@media (max-width: 640px) {
  #record-editor >>> .modal-rg {
    min-width: 98%;
  }

  #dbs0090A {
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