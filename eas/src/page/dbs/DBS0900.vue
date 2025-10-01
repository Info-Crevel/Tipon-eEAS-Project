// Member Control

<template>
<section class="container p-0 w-60"  :key="ts">
<sym-form id="dbs0900" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" @click="onSubmit">
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
    </div>

  </div>
  <div class="box-1">
   <sym-int v-model="memberControl.minimumAge" :caption-width="42" :input-width="22" caption="Minimum Age"></sym-int>
   <sym-int v-model="memberControl.fallOutDays" :caption-width="42" :input-width="22" caption="Fall Out Days"></sym-int>
   <sym-int v-model="memberControl.backOutDays" :caption-width="42" :input-width="22" caption="Back Out Days"></sym-int>
  </div>  
</sym-form>
</section>
</template>

<script>

import {
  get,
  ajax
} from '../../js/http';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0900',

  data () {
    return {
      memberControl: {
        memberControlId: 0,
        sssUploadRequiredFlag: false,
        pbgUploadRequiredFlag: false,
        phhUploadRequiredFlag: false,
        whtUploadRequiredFlag: false,
        soloParentUploadRequiredFlag: false,
        pwdUploadRequiredFlag: false,
        poolingValidationPeriod: 0,
        minimumAge: 0,
        basicPayTrxId:0,
        pbgPayTrxId:0,
        phhPayTrxId:0,
        sssPayTrxId:0,
        whtPayTrxId:0,
        deminimisPayTrxId:0,
        allowancePayTrxId:0,
        separationPayTrxId:0,
        yearEndPayTrxId:0,
        sickLeavePayTrxId:0,
        fallOutDays: 0,
        backOutDays: 0,
        lockId: ''
      }

    };
  },

  methods: {


    loadData () {
  const me = this;
  const wait = me.wait();
  me.getReferences()
    .then(data => {
      if (!me.core.isBoolean(data)) {
        me.basic = data.basic;
        me.pbg = data.pbg;
        me.phh = data.phh;
        me.sss = data.sss;
        me.wht = data.wht;
        me.deminimis = data.deminimis;
        me.allowance = data.allowance;
        me.separation = data.separation;
        me.yearEnd = data.yearEnd;
        me.sickLeave = data.sickLeave;
      }
    })
    
    me.getMemberControl()
    .then(memberControl => {
    
      if (memberControl && memberControl.length) {
        me.setModels(memberControl);
        me.stopWait(wait);
        // // me.memberControl = memberControl[0]; // Uncomment if needed
      }

      me.setupControls();
      me.isFilled = true;
    })
    .catch(fault => {
      me.stopWait(wait);
      me.showFault(fault);
    });
},
    setModels (info) {
      const me = this;
      me.memberControl = info[0];
      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldMemberControl = JSON.stringify(me.memberControl);
    },

    // API calls
    getReferences () {
      const me = this;

      if (me.basic.length) {
        return Promise.resolve(true);
      }
      return get('api/references/dbs0900');
    },



    getMemberControl () {
      if (this.memberControl.memberControlId < 0) {
        return Promise.resolve(null);
      }

      return get('api/member-control/');
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/member-control/' + currentUserId, options);
    },

    getApiPayload () {
      const
        me = this,
        memberControl = {};

      Object.assign(memberControl, me.memberControl); 
      return memberControl;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('memberControl');
      dc.keyField = 'memberControlId';
      dc.autoAssignKey = true;
      me.loadData();
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 100);
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 5 });
        return;
      }

      let
        promise,
        message,
        wait = me.wait()
        promise = me.modifyRecord();

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            message = 'Document updated.'
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

          me.loadData();
        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

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
          'minimumAge',
          'poolingValidationPeriod',
          'basicPayTrxId',
          'pbgPayTrxId',
          'phhPayTrxId',
          'sssPayTrxId',
          'whtPayTrxId',
        );
        me.setFocus('minimumAge');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.memberControl) !== me.oldMemberControl) { return true; }
      return false;
    }

  },

  created () {
    const me = this;
    me.oldMemberControl = '';
    me.basic = [];
    me.pbg = [];
    me.phh = [];
    me.sss = [];
    me.wht = [];
    me.deminimis = [];
    me.allowance = [];
    me.separation = [];
    me.yearEnd = [];
    me.sickLeave = [];
  }

}

</script>

<style scoped>

.action-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 3fr 1fr;
  gap: .75rem;
}
.box-1{
 display: grid;
 grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
 gap:.5rem;
}
.box-column{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr ;
  gap: .5rem;
}
.box-column-{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr  ;
  gap: .5rem;
}
</style>