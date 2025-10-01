// Hiring Process Screening

<template>
  <section class="container p-0 w-40"  :key="ts">
  <sym-form id="dbs0590" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
  
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
          <th class="w-10 text-right">ID</th>
          <th class="w-50">Screening Name</th>
          <th class="w-10 text-center">Upload</th>
          <th class="w-15 text-right">Sort Seq</th>
          <th class="w-20">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in applicantScreening" :key="index">
          <td class="text-right">{{ dtl.applicantScreeningId }}</td>
          <td>{{ dtl.applicantScreeningName }}</td>
          <td data-label="Upload" class="text-center">
            <i :class="getBooleanIconClass(dtl.uploadRequiredFlag)"></i>
          </td>
          <td class="text-right">{{ dtl.sortSeq }}</td>
          <td class="p-1">
            <div class="d-flex justify-center gap" sm-1 mb-0>
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

  <div class="d-flex justify-center mt-3" v-if="!applicantScreening.length">
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
    <form id="dbs0590A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.applicantScreeningId" caption="ID" align="bottom"></sym-int>
        <sym-text v-model="detail.applicantScreeningName" caption="Screening Name" align="bottom" @changing="onRequestTypeNameChanging"></sym-text>
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
  name: 'dbs0590',

  data () {
    return {
      applicantScreening: [],

      detail: {
        applicantScreeningId: 0,
        applicantScreeningName: '',
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
        return 'Add Applicant Screening';
      }
      return 'Edit Applicant Screening';
    }
  },  

  methods: {


    onRequestTypeNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.applicantScreeningNameCallback;
    },

    applicantScreeningNameCallback (e) {
      const me = this;

      let index = me.applicantScreening.findIndex(obj => obj.applicantScreeningName === e.proposedValue);
      if (index > -1) {
        e.message = 'Applicant Screening <b>' + e.proposedValue + '</b> is already in the list.'
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

      me.getApplicantSreeningList()
        .then( list => {
          me.stopWait(wait);
          me.applicantScreening = list;
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

    getApplicantSreeningList () {
      return get('api/applicant-screenings');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/applicant-screenings/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/applicant-screenings/' + this.detail.applicantScreeningId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');
        return ajax('api/applicant-screenings/' + detail.applicantScreeningId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'applicantScreeningId';
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
            let index = me.applicantScreening.findIndex(obj => obj.applicantScreeningId === success[0].applicantScreeningId);
            if (index > -1) {
              me.applicantScreening[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.applicantScreening.push(success[0]); 
                me.setupDetailControls();  
 
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.applicantScreeningId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('applicantScreeningName');
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
        'applicantScreeningName',
        'sortSeq'
      );  

      me.setOptionalMode(
        'uploadRequiredFlag'
      );

      me.setDisplayMode(
        'applicantScreeningId',
      );

      setTimeout(() => {
        this.setFocus('applicantScreeningName');
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

      d.applicantScreeningId = dtl.applicantScreeningId;
      d.applicantScreeningName = dtl.applicantScreeningName;
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
      me.detail.applicantScreeningId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.applicantScreening[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.applicantScreeningId = dtl.applicantScreeningId;
      d.applicantScreeningName = dtl.applicantScreeningName;
      d.uploadRequiredFlag = dtl.uploadRequiredFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsRequestType', me.detail.applicantScreeningId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Applicant Screening <b>' + me.applicantScreening[index].applicantScreeningName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.applicantScreening.splice(index, 1);
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

      if (!me.isValid('dbs0590A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.applicantScreening[me.recordIndex];

        dtl.applicantScreeningId = d.applicantScreeningId;
        dtl.applicantScreeningName = d.applicantScreeningName;
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

      d.applicantScreeningId = 0;
      d.applicantScreeningName = '';
      d.uploadRequiredFlag = false;
      d.sortSeq = 0;
      d.lockId = '';
    },
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
  grid-template-columns: 2fr 7fr 2fr;
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

  #dbs0590 >>> .card-body {
    padding: .25rem;
  }
}

</style>