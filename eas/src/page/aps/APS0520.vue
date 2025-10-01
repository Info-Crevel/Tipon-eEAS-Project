// Payable Request Type Particulars

<template>
  <section class="container p-0 w-100" :key="ts">
  <sym-form id="aps0520" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
  
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
          <th class="w-30">Particulars Name</th>
          <th class="w-50">Description</th>
          <th class="w-5">Sort Seq</th>
          <th class="w-10">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in requestTypeParticulars" :key="index">
          <td class="text-right">{{ dtl.particularsId }}</td>
          <td>{{ dtl.particularsName }}</td>
          <td>{{ dtl.particularsDescription }}</td>
          <td>{{ dtl.sortSeq}}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
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

  <div class="d-flex justify-center mt-3" v-if="!requestTypeParticulars.length">
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
    <form id="aps0520A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.particularsId" caption="ID" align="bottom"></sym-int>
        <sym-text v-model="detail.particularsName" caption="Particulars Name" align="bottom" @changing="onParticularsNameChanging"></sym-text>
        <sym-text v-model="detail.particularsDescription" caption="Description" align="bottom" ></sym-text>
        <sym-int v-model="detail.sortSeq" caption="Sort" align="bottom"></sym-int>
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
  getList,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymDecimal },
  extends: PageMaintenance,
  name: 'aps0520',

  data () {
    return {
      requestTypeParticulars: [],
      
      detail: {
        particularsId: 0,
        particularsName: '',
        particularsDescription: '',
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
        return 'Add Request Type Particulars';
      }
      return 'Edit Request Type Particulars';
    }
  },  

  methods: {
    
    onParticularsNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.particularsNameCallback;
    },


    particularsNameCallback (e) {
      const me = this;
      const escapedValue = e.proposedValue.replace(/'/g, "''");  // Escape single quotes for SQL
      const filter = "ParticularsName='" + escapedValue + "'";

      // let filter = "ParticularsName='" + e.proposedValue +"'";
      return getList('dbo.ApsRequestTypeParticulars', 'ParticularsId, ParticularsName', '', filter).then(
        requestTypeParticulars => {
          if (requestTypeParticulars && requestTypeParticulars.length) {        
            let index = me.requestTypeParticulars.findIndex(obj => obj.particularsName === e.proposedValue);
            if (index > -1) {
              e.message = 'Particulars Name <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }

            return true;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }

      );
      
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

      me.getRequestTypeList()
        .then( list => {
          me.stopWait(wait);
          me.requestTypeParticulars = list;
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

    getRequestTypeList () {
      return get('api/payable-request-type-particulars');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/payable-request-type-particulars/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/payable-request-type-particulars/' + this.detail.particularsId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');
      return ajax('api/payable-request-type-particulars/' + detail.particularsId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'particularsId';
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
            let index = me.requestTypeParticulars.findIndex(obj => obj.particularsId === success[0].particularsId);
            if (index > -1) {
              me.requestTypeParticulars[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.requestTypeParticulars.push(success[0]); 
                me.setupDetailControls();   // 05 Feb 2025 - EMT
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.particularsId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('particularsName');
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
        'particularsName',
        'particularsDescription',
        'sortSeq'
      );  

      setTimeout(() => {
        this.setFocus('particularsName');
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

      d.particularsId = dtl.particularsId;
      d.particularsName = dtl.particularsName;
      d.particularsDescription = dtl.particularsDescription;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.particularsId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.requestTypeParticulars[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.particularsId = dtl.particularsId;
      d.particularsName = dtl.particularsName;
      d.particularsDescription = dtl.particularsDescription;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('ApsRequestTypeParticulars', me.detail.particularsId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Request Type <b>' + me.requestTypeParticulars[index].particularsName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.requestTypeParticulars.splice(index, 1);
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

      if (!me.isValid('aps0520A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.requestTypeParticulars[me.recordIndex];

        dtl.particularsId = d.particularsId;
        dtl.particularsName = d.particularsName;
        dtl.particularsDescription = d.particularsDescription;
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

    hasChanges () {
      const me = this;
      if (JSON.stringify(me.detail) !== me.oldDetail) { return true; }
      return false;
    },

    clearRecordPad () {
      const d = this.detail;

      d.particularsId = 0;
      d.particularsName = '';
      d.particularsDescription = '';
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
  grid-template-columns: .5fr 3fr 3fr .5fr;
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

  #aps0520 >>> .card-body {
    padding: .25rem;
  }
}

</style>