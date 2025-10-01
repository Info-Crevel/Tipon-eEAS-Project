// MRF Internal Position Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0480" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
          <th class="w-10 text-right">Position ID</th>
          <th class="w-20 ">Position Name</th>
          <th class="w-50 ">Description</th>
          <th class="w-10  text-right">Sort Seq</th>
          <th class="w-10 ">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in memberRequestPositions" :key="index">
          <td class="text-right">{{ dtl.memberRequestPositionId }}</td>
          <td>{{ dtl.memberRequestPositionName }}</td>
          <td>{{ dtl.memberRequestPositionDescription }}</td>
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

  <div class="d-flex justify-center mt-3" v-if="!memberRequestPositions.length">
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
    <form id="dbs0480A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.memberRequestPositionId" caption="Position ID" align="bottom"></sym-int>
        <sym-text v-model="detail.memberRequestPositionName" caption="Position Name" align="bottom" @changing="onMemberRequestPositionNameChanging"></sym-text>
        <sym-memo v-model="detail.memberRequestPositionDescription" caption="Description" align="bottom"></sym-memo>
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
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymDecimal },
  extends: PageMaintenance,
  name: 'dbs0480',

  data () {
    return {
      memberRequestPositions: [],

      detail: {
        memberRequestPositionId: 0,
        memberRequestPositionName: '',
        memberRequestPositionDescription: '',
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
        return 'Add MRF Internal Position';
      }
      return 'Edit MRF Internal Position';
    }
  },  

  methods: {
    
    onMemberRequestPositionNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.memberRequestPositionNameCallback;
    },

    memberRequestPositionNameCallback (e) {
      const me = this;

      let index = me.memberRequestPositions.findIndex(obj => obj.memberRequestPositionName === e.proposedValue);
      if (index > -1) {
        e.message = 'MRF Internal Position <b>' + e.proposedValue + '</b> is already in the list.'
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

      me.getMemberRequestPositionList()
        .then( list => {
          me.stopWait(wait);
          me.memberRequestPositions = list;
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

    getMemberRequestPositionList () {
      return get('api/member-request-positions');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-request-positions/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/member-request-positions/' + this.detail.memberRequestPositionId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        // currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('DELETE');

        // 05 Feb 2025 - EMT (change of signature to fix 404 issue when lockId contains a slash character)
        return ajax('api/member-request-positions/' + detail.memberRequestPositionId + this.getDeleteQuery(detail.lockId), options);

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
      dc.keyField = 'memberRequestPositionId';
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
            let index = me.memberRequestPositions.findIndex(obj => obj.memberRequestPositionId === success[0].memberRequestPositionId);
            if (index > -1) {
              me.memberRequestPositions[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.memberRequestPositions.push(success[0]); 
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.memberRequestPositionId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom();
                this.setFocus('memberRequestPositionName');
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
        'memberRequestPositionName',
        'memberRequestPositionDescription',
        'sortSeq'
      );  

      me.setDisplayMode(
        'memberRequestPositionId',
      );

      setTimeout(() => {
        this.setFocus('memberRequestPositionName');
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

      d.memberRequestPositionId = dtl.memberRequestPositionId;
      d.memberRequestPositionName = dtl.memberRequestPositionName;
      d.memberRequestPositionDescription = dtl.memberRequestPositionDescription;

      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.memberRequestPositionId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.memberRequestPositions[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.memberRequestPositionId = dtl.memberRequestPositionId;
      d.memberRequestPositionName = dtl.memberRequestPositionName;
      d.memberRequestPositionDescription = dtl.memberRequestPositionDescription;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('ArsMemberRequestPosition', me.detail.memberRequestPositionId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('MRF Internal Position <b>' + me.memberRequestPositions[index].memberRequestPositionName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.memberRequestPositions.splice(index, 1);
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

      if (!me.isValid('dbs0480A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.memberRequestPositions[me.recordIndex];

        dtl.memberRequestPositionId = d.memberRequestPositionId;
        dtl.memberRequestPositionName = d.memberRequestPositionName;
        dtl.memberRequestPositionDescription = d.memberRequestPositionDescription;
   
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

      d.memberRequestPositionId = 0;
      d.memberRequestPositionName = '';
      d.memberRequestPositionDescription = '';
     
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

  #dbs0480 >>> .card-body {
    padding: .25rem;
  }
}


</style>