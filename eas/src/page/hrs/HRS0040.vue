// Recruiter Setup

<template>
  <section class="container p-0   w-70" :key="ts">
  <sym-form id="hrs0040" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">
  
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
          <th class="w-15">Employee ID</th>
          <th class="w-20">Recruiter Name</th>
          <th class="w-10">User ID</th>
          <th class="w-20">User Name</th>
          <th class="w-10">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in recruiters" :key="index">
          <td class="text-right">{{ dtl.recruiterId }}</td>
          <td>{{ dtl.memberEmployeeId }}</td>
          <td>{{ dtl.recruiterName }}</td>
          <td>{{ dtl.userId}}</td>
          <td>{{ dtl.userName}}</td>
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

  <div class="d-flex justify-center mt-3" v-if="!recruiters.length">
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
    <form id="hrs0040A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="record-editor-boxes">
        <sym-int v-model="detail.recruiterId" caption="Recruiter ID" align="bottom"></sym-int>
        <sym-text v-model="detail.memberEmployeeId" caption="Employee ID" align="bottom" @changing="onMemberEmployeeIdChanging"></sym-text>
        <sym-text v-model="detail.recruiterName" caption="Recruiter Name" align="bottom"></sym-text>

        <sym-int v-model="detail.userId" caption="User ID" align="bottom" lookupId="SecUser" @changing="onUserIdChanging" @searchresult="onUserIdSearchResult"></sym-int>
        <sym-text v-model="detail.userName" caption="User Name" align="bottom"></sym-text>
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
  name: 'hrs0040',

  data () {
    return {
      recruiters: [],
      
      detail: {
        recruiterId: 0,
        memberEmployeeId: '',
        recruiterName: '',
        userId: 0,
        userName: '',
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
        return 'Add Recruiter';
      }
      return 'Edit Recruiter';
    }
  },  

  methods: {
    
    onUserIdChanging (e) {
      e.message = "User ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.userIdCallback;
    },

    userIdCallback (e) {
      const me = this;
      let filter = "UserId=" + e.proposedValue;
      return getList('dbo.SecUser', 'UserId, UserName', '', filter).then(
        user => {
          if (user && user.length) {        
            let index = me.recruiters.findIndex(obj => obj.userId === e.proposedValue);
            if (index > -1) {
              e.message = 'User ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }

            me.detail.userName = user[0].userName;
            return true;
          }
          return false;
        },
        fault => {
          me.showFault(fault);
          return false;
        }

      );
      
    },

    onUserIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        detail = this.detail;
        let index = this.recruiters.findIndex(obj => obj.userId === data.userId);
        if (index > -1) {
          this.advice.fault('User ID <b>' + data.userId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        detail.userId = data.userId;
        detail.userName = data.userName;
  
        this.focusNext();

    },

    onMemberEmployeeIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.memberEmployeeIdCallback;
    },


    memberEmployeeIdCallback (e) {
      const me = this;
      let filter = "MemberStatusId =1 AND MemberEmployeeId='" + e.proposedValue +"'";
      return getList('dbo.HrsMember', 'MemberEmployeeId, MemberName', '', filter).then(
        member => {
          if (member && member.length) {        
            let index = me.recruiters.findIndex(obj => obj.memberEmployeeId === e.proposedValue);
            if (index > -1) {
              e.message = 'Employee ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }

            me.detail.recruiterName = member[0].memberName;
            return true;
          }
          return false;
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

      me.getRecruiterList()
        .then( list => {
          me.stopWait(wait);
          me.recruiters = list;
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

    getRecruiterList () {
      return get('api/recruiters');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/recruiters/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/recruiters/' + this.detail.recruiterId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.detail,
        options = this.core.getAjaxOptions('DELETE');
      return ajax('api/recruiters/' + detail.recruiterId + this.getDeleteQuery(detail.lockId), options);
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
      dc.keyField = 'recruiterId';
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
            let index = me.recruiters.findIndex(obj => obj.recruiterId === success[0].recruiterId);
            if (index > -1) {
              me.recruiters[index].lockId = success[0].lockId;
            } else {
              if (me.isAdding) {
                me.recruiters.push(success[0]); 
                me.setupDetailControls(); 
              }  
            }

            if (me.isAdding) {
              message = 'New document created.'

              me.clearRecordPad();
              me.detail.recruiterId = -1;
              me.refreshOldRefs();

              setTimeout(() => {
                me.scrollToBottom('list');
                this.setFocus('memberEmployeeId');
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
        'memberEmployeeId',
        'userId'
      );  

      me.setDisplayMode(
        'recruiterId',
        'recruiterName',
        'userName',
      );

      setTimeout(() => {
        this.setFocus('memberEmployeeId');
      }, 200);
    },

    onHideRecordEditor () {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
    },

    onEditRecord (dtl, index) {
      const
        me = this,
        d = me.detail;

      me.recordIndex = index;
      me.clearRecordPad();

      d.recruiterId = dtl.recruiterId;
      d.memberEmployeeId = dtl.memberEmployeeId;
      d.recruiterName = dtl.recruiterName;

      d.userId = dtl.userId;
      d.userName = dtl.userName;

      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = false;
    },

    onAddRecord () {
      const me = this;

      me.clearRecordPad();
      me.detail.recruiterId = -1;
      me.refreshOldRefs();

      me.isRecordEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteRecord (index) {
      const
        me = this,
        d = me.detail,
        dtl = me.recruiters[index];

      me.recordIndex = index;
      me.clearRecordPad();

      d.recruiterId = dtl.recruiterId;
      d.memberEmployeeId = dtl.memberEmployeeId;
      d.recruiterName = dtl.recruiterName;
      d.userId = dtl.userId;
      d.userName = dtl.userName;
      d.lockId = dtl.lockId;

      me.refreshOldRefs();

      me.isRecordEditorVisible = false;
      me.isAdding = false;

      getSafeDeleteFlag('HrsRecruiter', me.detail.recruiterId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Recruiter <b>' + me.recruiters[index].recruiterName + '</b> will be deleted.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
            me.recruiters.splice(index, 1);
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

      if (!me.isValid('hrs0040A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (me.isAdding) {
        me.onSubmit();
      } else {
        dtl = me.recruiters[me.recordIndex];

        dtl.recruiterId = d.recruiterId;
        dtl.memberEmployeeId = d.memberEmployeeId;
        dtl.recruiterName = d.recruiterName;

        dtl.userId = d.userId;
        dtl.userName = d.userName;

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

      d.recruiterId = 0;
      d.memberEmployeeId = '';
      d.recruiterName = '';

      d.userId = 0;
      d.userName = '';

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
  grid-template-columns: 2fr 2fr 7fr 2fr;
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

  #hrs0040 >>> .card-body {
    padding: .25rem;
  }
}

</style>