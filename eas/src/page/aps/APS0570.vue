// Payable Request Transaction Mapping

<template>
<section class="container p-0" :key="ts">
<sym-form id="aps0570" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" @click="onSubmit">
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear">
        <i class="fa fa-undo fa-lg"></i><span>Clear</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete" @click="onDelete">
        <i class="fa fa-times-circle fa-lg"></i><span>Delete</span>
      </button>
    </div>

  </div>

  <sym-int v-model="org.orgId" :caption-width="30" :input-width="20" caption="Org ID" lookupId="SecOrg" @lostfocus="onOrgIdLostFocus" @changing="onOrgIdChanging" @changed="onOrgIdChanged" @searchresult="onOrgIdSearchResult"></sym-int>
  <sym-text v-model="org.orgName" :caption-width="30" :input-width="110" caption="Org Name"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 w-100">
      <thead>
        <tr>
          <th class="w-5">ID</th>
          <th class="w-25">Trx Name</th>
          <th class="w-15">Type</th>
          <th class="w-5">Account ID</th>
          <th class="w-40">Account Name</th>
          <th class="w-10">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in orgDetails" :key="index">
          <td>{{ dtl.orgTrxId }}</td>
          <td>{{ dtl.requestTrxName }}</td>
          <td>{{ dtl.requestTrxTypeName }}</td>
          <td>{{ dtl.accountId }}</td>
          <td>{{ dtl.accountName }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
              <button type="button" class="justify-between infoXXX info-light fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="warningXXX danger-light btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)">
                <i class="fa fa-times fa-lg"></i>
              </button>
            </div>
          </td>

        </tr>
      </tbody>
    </table>
  </div>

  <div class="command-buttons light border-main p-1 border-top-0 mb-2">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between btn-add" @click="onAddDetail">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>

</sym-form>

<sym-modal
  id="detail-editor"
  v-model="isDetailEditorVisible"
  size="lg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="editorTitle"
  @show="onShowDetailEditor($event)"
  @hide="onHideDetailEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="aps0570A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text  v-model="detail.requestTrxName" caption="Trx Name" align="bottom" @changing="onRequestTrxNameChanging"></sym-text>
      <sym-combo v-model="detail.requestTrxTypeId" caption="Type" align="bottom" display-field="requestTrxTypeName" :datasource="requestTrxTypes"></sym-combo>
      <sym-text v-model="detail.accountId" :caption-width="30" align="bottom" caption="Account ID" lookupId="FinAccountCore" @changing="onAccountIdChanging" @searchfill="onAccountIdSearchFill" @searchresult="onAccountIdSearchResult"></sym-text>
      <sym-text v-model="detail.accountName" caption="Account Name" :caption-width="35" align="bottom" ></sym-text>          

    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isDetailEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>
</div>

</sym-modal>

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

export default {
  extends: PageMaintenance,
  name: 'aps0570',

  data () {
    return {
      org: {
        orgId: 0,
        orgName: '',
        orgShortName: '',
        sortSeq: 0,
        lockId: ''
      },

      orgDetails: [],

      isDetailEditorVisible: false,

      detail: {
        orgTrxId: 0,
        requestTrxName: '',
        requestTrxTypeId: 0,
        requestTrxTypeName: '',
        orgId: 0,
        accountId: '',
        accountName: '',
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    detailTag () {
      return 'Transactions: ' + this.orgDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Transaction';
      }
      return 'Edit Transaction';
    }

  },

  methods: {

    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) { 
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      return getList('dbo.QFinAccountCore', 'AccountId, AccountName', '', filter).then(
        acct => {
          if (acct && acct.length) {
            me.detail.accountName = acct[0].accountName;
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

    onAccountIdSearchFill (e) {
      e.filter = "HeaderFlag = 0"  
    },

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.accountId = data.accountId;
      detail.accountName = data.accountName;

      this.focusNext();

    },

    onRequestTrxNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.requestTrxNameCallback;
    },


    requestTrxNameCallback (e) {
      const me = this;
      let filter = "RequestTrxName='" + e.proposedValue +"'";
      return getList('dbo.ApsOrgTrx', 'OrgTrxId, RequestTrxName', '', filter).then(
        requestTrx => {
          if (requestTrx && requestTrx.length) {        
            let index = me.orgDetails.findIndex(obj => obj.requestTrxName === e.proposedValue);
            if (index > -1) {
              e.message = 'Request Trx Name <b>' + e.proposedValue + '</b> is already in the list.'
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
  
    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getOrg()
        .then( info => {
          me.stopWait(wait);
          if (info && info.org.length) {
            me.setModels(info);
          } else {
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Organization <b>' + me.org.orgId + '</b> not found.';
              me.advice.fault(mssg, { duration: 5 });
              me.onReset();
              return;
            }
          }
          me.setupControls();
          me.isFilled = true;
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })
    },

    setModels (info) {
      const
        me = this,
        org = info.org[0];

      me.org = org;
      me.orgDetails = info.orgDetails;
      me.requestTrxTypes = info.requestTrxTypes;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldOrg = JSON.stringify(me.org);
      me.oldOrgDetails = JSON.stringify(me.orgDetails);
    },

    // API calls

    getOrg () {
      return get('api/aps-orgs/' + this.org.orgId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/aps-orgs/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/aps-orgs/' + this.org.orgId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        org = this.org,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/aps-orgs/' + org.orgId + this.getDeleteQuery(org.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        org = {};

      Object.assign(org, me.org);
      org.details = me.orgDetails;

      return org;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('org', 'orgDetails', 'detail');
      dc.keyField = 'orgId';
      dc.autoAssignKey = false;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 100);
    },

    onOrgIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.orgIdCallback;
    },

    orgIdCallback (e) {
      e.message = "Org ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.SecOrg', 'orgId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onOrgIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onOrgIdLostFocus () {
      const me = this;

      if (!me.org.orgId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onOrgIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.org.orgId = data.orgId;
      me.replaceUrl();
      me.loadData();
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 3 });
        me.onReset();
        return;
      }

      let
        promise,
        message,
        wait = me.wait(),
        isNew = me.isNew();

      if (isNew) {
        promise = me.createRecord();
      } else {
        promise = me.modifyRecord();
      }

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'
            }
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

          me.onReset();
        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

    },

    onDelete () {
      const me = this;

      getSafeDeleteFlag('SecOrg', me.org.orgId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Document will be deleted.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" } );
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
            me.advice.success('Document deleted.', { duration: 4 });
            me.onReset();
          }
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },

    onShowDetailEditor () {
      const me = this;

      let target = 'requestTrxName';

      me.setActiveModel('detail');

      me.setRequiredMode(
        'requestTrxName',
        'requestTrxTypeId',
        'accountId',
      );

        me.setOptionalMode(
        'requestTrxName',
      );

      me.setDisplayMode(
        'accountName',
      );
      
      setTimeout(() => {
        this.setFocus(target);
      }, 200);
    },

    onHideDetailEditor () {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
    },

    onEditDetail (dtl, index) {
      // hold current property values of selected detail for editing; passed index too.
      const d = this.detail;
      this.detailIndex = index;

      d.orgTrxId = dtl.orgTrxId;
      d.requestTrxName = dtl.requestTrxName;
      d.requestTrxTypeId = dtl.requestTrxTypeId;
      d.requestTrxTypeName = dtl.requestTrxTypeName;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;
      d.lockId = dtl.lockId;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.orgTrxId = -1;
      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      if (!me.orgDetails[index].lockId) {
        me.orgDetails.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('ApsOrgTrx', me.orgDetails[index].orgId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Request Trx Name <b>' + me.orgDetails[index].requestTrxName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.orgDetails.splice(index, 1);
          }
          return;
        })
        .catch( fault => {
          me.showFault(fault);
        });

    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      if (!d.requestTrxName) {
        me.isDetailEditorVisible = false;
        return;
      }


      if (!me.isValid("aps0570A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }


      let dtl = {};

      if (me.isAdding) {

        Object.assign(dtl, d);

        me.requestTrxTypes.forEach( types => {
          if (types.requestTrxTypeId === dtl.requestTrxTypeId) {
            dtl.requestTrxTypeName = types.requestTrxTypeName;
            dtl.orgId = me.org.orgId;
          }
        });

        me.orgDetails.push(dtl);

        me.clearDetailPad();
        me.advice.info("Request Trx Name '" + dtl.requestTrxName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('requestTrxName');
        }, 100);
      } else {
        dtl = me.orgDetails[me.detailIndex];

        me.requestTrxTypes.forEach( dtl => {
          if (dtl.requestTrxTypeId === d.requestTrxTypeId) {
            d.requestTrxTypeName = dtl.requestTrxTypeName;
          }
        });
        
        dtl.requestTrxTypeId = d.requestTrxTypeId;
        dtl.requestTrxName = d.requestTrxName;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;
        dtl.orgId = me.org.orgId;
        dtl.lockId = d.lockId;

        me.isDetailEditorVisible = false;
      }

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
          'orgName',
          'orgShortName',
          'sortSeq'
        );

        me.setDisplayMode(
          'orgName'
        );

        me.setFocus('orgName');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.org) !== me.oldOrg) { return true; }
      if (JSON.stringify(me.orgDetails) !== me.oldOrgDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.orgTrxId = 0;
      d.requestTrxName = '';
      d.requestTrxTypeId = 0;
      d.requestTrxTypeName = '';
      d.accountId = '';
      d.accountName = '';
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldOrg = '';
    me.oldOrgDetails = '';
    me.requestTrxTypes = [];   
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

.command-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.detail-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr;
  gap: .5rem;
}

.fixed-header {
  overflow: auto;
  max-height: 50vh;
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
    width: 60%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 75%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 85%;
  }
}

@media (max-width: 700px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #aps0570 >>> .card-body {
    padding: .5rem;
  }
}

</style>