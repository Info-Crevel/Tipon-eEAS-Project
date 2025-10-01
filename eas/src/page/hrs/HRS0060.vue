// Member Employment Status Update

<template>
<section class="container p-0" :key="ts">
<sym-form id="hrs0060" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
    </div>
  </div>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ TotalTag }}</span>
  </div>

  <div class="table-scroll-wrapper" ref="list">
    <table class="table-scroll">
      <thead>
        <tr>
          <th class="w-20 text-right">ID</th>
          <th class="w-25">Member Id</th>
          <th class="w-25">Employee Id</th>
          <th class="w-50">Member Name</th>
          <th class="w-50">Current Status</th>
          <th class="w-30">Proposed Status</th>
          <th class="w-30">Effectivity Date</th>
          <th class="w-40">Status Action Name</th>
          <th class="w-55">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in actions" :key="index">
          <td class="text-right">{{ dtl.employmentStatusMemberId }}</td>
          <td>{{ dtl.memberId }}</td>
          <td>{{ dtl.memberEmployeeId }}</td>
          <td>{{ dtl.memberName }}</td>
          <td>{{ dtl.currentEmploymentStatusName }}</td>
          <td>{{ dtl.employmentStatusName }}</td>
          <td>{{ dtl.effectivityDate }}</td>
          <td>{{ dtl.statusActionName }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
               <button type="button" class="justify-between info-light fw-22  w-50 btn-dtl-edit" @click="onSetApproved(index)" v-if="dtl.statusActionId == 1">
                <i class="fa fa-check fa-lg w-30"></i><span>Approve</span>
              </button>


              <button type="button" class="danger-light btn-dtl-delete w-50" @click="onSetCanceled(index)" v-if="dtl.statusActionId === 1" >
                <i class="fa fa-times fa-lg"></i><span>Cancel</span>
              </button> 

              <button type="button" :class="logButtonClass" @click="onViewLog(index)">
                    <i class="fa fa-database fa-lg"></i><span>Log</span> 
                  </button>


            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
  
  <div class="d-flex justify-center mt-3" v-if="!actions.length">
    <sym-alert class="info w-100 text-center" icon="none">
      <span>No records found. File is empty.</span>
    </sym-alert>
  </div>
<!-- logs     -->
<sym-modal 
        v-model="isLogVisible"
        size="xl"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="true"
        :dismissible="true"
        :closeOnBackButton="false"
        title="Employment Status Change Log"
        headerClass="app-form-header"
        dismissButtonClass="danger"
      >
        <div class="fixed-header">
          <table id="logs" class="striped-odd">
            <thead>
              <tr>
                <th class="w-7">Action</th>
                <th class="w-15">Column</th>
                <th class="w-18">Old Value</th>
                <th class="w-18">New Value</th>
                <th class="w-15">Log Date/Time</th>
                <th class="w-10">User</th>
                <th class="w-22">Reference</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="(log, index) in logs" :key="index">
                <td>{{ log.logDescription }}</td>
                <td>{{ log.columnCaption }}</td>
                <td>{{ log.oldValue }}</td>
                <td>{{ log.newValue }}</td>
                <td>
                  {{
                    core.toDateFormat(log.logDateTime, true, "MM/dd/yyyy h:mm tt")
                  }}
                </td>
                <td>{{ log.logUserInfo }}</td>
                <td>{{ log.logReference }}</td>
              </tr>
            </tbody>
          </table>
        </div>
  </sym-modal>
<!-- logs     -->

</sym-form>
</section>
</template>

<script>

import {
  get,
  ajax
}
from '../../js/http';
import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'hrs0060',

  data () {
    return {
      actions: [],

      detail: {
        employmentStatusMemberId: 0,
        memberId: 0,
        memberName: '',
        memberEmployeeId: '',
        lockId: ''
      },

      isRecordEditorVisible: false,
      recordIndex: -1,
      isAdding: false,

      logs: [],
      isLogVisible: false,
    };
  },

  computed: {

    TotalTag () {
      return 'Total Count: ' + this.actions.length;
    },


    isCancelled () {
      return this.detail.statusActionId === 3;
    },

    
  },

  methods: {

    onViewLog(index) {
      const me = this,
        wait = me.wait(),
        d = me.detail,
        dtl = me.actions[index];
        
      d.employmentStatusActionId = dtl.employmentStatusActionId;
      d.memberId = dtl.memberId;
        me.getChangeLog(dtl).then(
        (logs) => {
          me.stopWait(wait);
          me.logs = logs;
          me.isLogVisible = true;
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onSetCanceled (index) {
      const
        me = this;
        me.recordIndex = index;
        let promise = me.isActionAllowed('CAN-STATUS-EMP');
      
        promise.then(
        reply => {
          if (reply === 'yes') {
            me.onCancelRecord(index);
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },

    onSetApproved (index) {
      const
        me = this;
        me.recordIndex = index;
        let promise = me.isActionAllowed('APP-STATUS-EMP');
      
        promise.then(
        reply => {
          if (reply === 'yes') {
            me.onApproveRecord(index);
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },


    onApproveRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.actions[index];

      me.recordIndex = index;

      d.employmentStatusMemberId = dtl.employmentStatusMemberId;
      d.statusActionName = "Approved";
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.dialog.confirm('Ready to <b>approve</b> employment action for <b>' + dtl.memberName + ' [' + dtl.memberEmployeeId+ ']</b> ? <br>Once approved, action cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            d.statusActionId = 2;
            me.onSubmit();
          }
          return;
        }
      );
      

    },


    onCancelRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.actions[index];

      me.recordIndex = index;

      d.employmentStatusMemberId = dtl.employmentStatusMemberId;
      d.statusActionName = "Canceled";
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.dialog.confirm('Ready to <b>cancel</b> employment action for <b>' + dtl.memberName + ' [' + dtl.memberEmployeeId+ ']</b> ? <br>Once cancelled, action cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            d.statusActionId = 3;
            me.onSubmit();
          }
          return;
        }
      );
      

    },
    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getMemberList()
        .then( list => {
          me.stopWait(wait);
          me.actions = list;
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

    getChangeLog(dtl) {
      return get("api/employment-status-members/" + dtl.employmentStatusActionId + "/" + dtl.memberId + "/log");
    },

    getMemberList () {
      return get('api/employment-status-member-actions');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/employment-status-member-actions/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/employment-status-member-actions/' + this.detail.employmentStatusMemberId + '/' + currentUserId, options);
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
      dc.keyField = 'employmentStatusMemberId';
      dc.autoAssignKey = true;
    },

    onSubmit (nextRoute) {
      const me = this;

      let
        promise,
        message='Action updated.',
        wait = me.wait();
        promise = me.modifyRecord();
  
        promise.then(
        (success) => {
          me.stopWait(wait);
          if (success && success.length) {
            let index = me.actions.findIndex(obj => obj.employmentStatusMemberId === success[0].employmentStatusMemberId);
            if (index > -1) {
              me.actions[index].lockId = success[0].lockId;
              this.loadData();
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

    hasChanges () {
      const me = this;
      if (JSON.stringify(me.detail) !== me.oldDetail) { return true; }
      return false;
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