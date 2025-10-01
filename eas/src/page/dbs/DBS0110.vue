// Doc Type Setup

<template>
<section class="container p-0 w-80" :key="ts">
<sym-form id="dbs0110" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg "></i><span>Refresh</span>
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
          <th class="w-30">Doc Type Name</th>
          <th class="w-10 text-center">ID Length</th>
          <th class="w-15 text-center">Member Flag</th>
          <th class="w-15 text-center">Applicant Flag</th>
          <th class="w-15 text-center">Contract Flag</th>
          <th class="w-15 text-center">Hiring Process Flag</th>
          <th class="w-15 text-center">Expiration Flag</th>
          <th class="w-10 text-right">Sort Seq</th>
          <th class="w-15">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in docTypes" :key="index">
          <td class="text-right">{{ dtl.docTypeId }}</td>
          <td>{{ dtl.docTypeName }}</td>
          <td class="text-center">{{ dtl.docTypeLength }}</td>
          <td data-label="Member Flag" class="text-center">
            <i :class="getBooleanIconClass(dtl.memberFlag)"></i>
          </td>
          <td data-label="Applicant Flag" class="text-center">
            <i :class="getBooleanIconClass(dtl.applicantFlag)"></i>
          </td>
          <td data-label="Contract Flag" class="text-center">
            <i :class="getBooleanIconClass(dtl.contractFlag)"></i>
          </td>
          <td data-label="Contract Flag" class="text-center">
            <i :class="getBooleanIconClass(dtl.tradeTestFlag)"></i>
          </td>
          <td data-label="Contract Flag" class="text-center">
            <i :class="getBooleanIconClass(dtl.expirationFlag)"></i>
          </td>
          <td class="text-right">{{ dtl.sortSeq }}</td>
          <td class="p-1">
            <div class="buttons d-flex justify-center gap" sm-1 mb-0>
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

  <div class="d-flex justify-center mt-3" v-if="!docTypes.length">
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
    <form id="dbs0110A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.docTypeId" caption="Doc Type ID" align="bottom"></sym-int>
        <sym-text v-model="detail.docTypeName" caption="Doc Type Name" align="bottom"></sym-text>
        <sym-int v-model="detail.docTypeLength" caption="Format Length" align="bottom"></sym-int>
        <sym-check v-model="detail.memberFlag" caption="Member Flag" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-check v-model="detail.applicantFlag" caption="Applicant Flag" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-check v-model="detail.contractFlag" caption="Contract Flag" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-check v-model="detail.tradeTestFlag" caption="Hiring Process" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        <sym-check v-model="detail.expirationFlag" caption="Expiration Flag" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
        
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
  name: 'dbs0110',

  data () {
    return {
      docTypes: [],

      detail: {
        docTypeId: 0,
        docTypeName: '',
        docTypeLength: 0,
        memberFlag: false,
        applicantFlag: false,
        contractFlag: false,
        tradeTestFlag: false,
        expirationFlag: false,
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
        return 'Add Document Type';
      }
      return 'Edit Document Type';
    }
  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();
     
      me.getdocTypeList()
      
        .then( list => {
          me.stopWait(wait);
          me.docTypes = list;
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

    getdocTypeList () {
      return get('api/doc-types');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/doc-types/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/doc-types/' + this.detail.docTypeId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/doc-types/' + detail.docTypeId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'docTypeId';
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
            let index = me.docTypes.findIndex(obj => obj.docTypeId === success[0].docTypeId);
            if (index > -1) {
              me.docTypes[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.docTypes.push(success[0]);
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.docTypeId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('docTypeName');
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
        'docTypeName',
        'sortSeq'
      );

      me.setOptionalMode(
        'memberFlag',
        'applicantFlag',
        'contractFlag',
        'tradeTestFlag',
        'expirationFlag',
        'docTypeLength'
      );

      me.setDisplayMode(
        'docTypeId',
      );

      setTimeout(() => {
        this.setFocus('docTypeName');
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

      d.docTypeId = dtl.docTypeId;
      d.docTypeName = dtl.docTypeName;
      d.docTypeLength = dtl.docTypeLength;
      d.applicantFlag = dtl.applicantFlag;
      d.contractFlag = dtl.contractFlag;
      d.tradeTestFlag = dtl.tradeTestFlag;
      d.expirationFlag = dtl.expirationFlag;
      d.memberFlag = dtl.memberFlag;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.docTypeId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.docTypes[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.docTypeId = dtl.docTypeId;
      d.docTypeName = dtl.docTypeName;
      d.docTypeLength = dtl.docTypeLength;
      d.contractFlag = dtl.contractFlag;
      d.tradeTestFlag = dtl.tradeTestFlag;
      d.expirationFlag = dtl.expirationFlag;
      d.applicantFlag = dtl.applicantFlag;
      d.memberFlag = dtl.memberFlag;

      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('DbsDocType', me.detail.docTypeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Document Type <b>' + me.docTypes[index].docTypeName + '</b> will be deleted.<br><br>Continue?', { size: 'md', scheme: 'warning' });
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
            me.docTypes.splice(index, 1);
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

      if (!me.isValid('dbs0110A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.docTypes[me.recordIndex];
        // console.log(d.docTypeLength)
        dtl.docTypeId = d.docTypeId;
        dtl.docTypeName = d.docTypeName;
        dtl.docTypeLength = d.docTypeLength;
        dtl.contractFlag = d.contractFlag;
        dtl.tradeTestFlag = d.tradeTestFlag;
        dtl.expirationFlag = d.expirationFlag;
        dtl.applicantFlag = d.applicantFlag;
        dtl.memberFlag = d.memberFlag;

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

      d.docTypeId = 0;
      d.docTypeName = '';
      d.docTypeLength = 0;
      d.applicantFlag = false;
      d.contractFlag = false,
      d.tradeTestFlag = false,
      d.expirationFlag = false,
      d.memberFlag = false;

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
  grid-template-columns: 1.5fr 2.5fr 2fr 2fr 2fr 2fr 2fr 2fr 1.5fr;
  gap: .5rem;
}

#record-editor >>> .modal-sm {
  min-width: 40%;
}

.fixed-header {
  /* overflow-y: auto; */
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

</style>