// Cluster Definition

<template>
<section class="container w-78" :key="ts">
<sym-form id="dbs0920" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="cluster.clusterId" :caption-width="35" :input-width="20" caption="Cluster ID" lookupId="DbsCluster" @lostfocus="onClusterIdLostFocus" @changing="onClusterIdChanging" @changed="onClusterIdChanged" @searchresult="onClusterIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>

  <sym-text v-model="cluster.clusterName" :caption-width="35" :input-width="80" caption="Cluster Name"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Details</span>
  </div>

  <div id="list-detail" class="d-flex fixed-header" ref="list-detail">
    <table class="light striped-even w-100">
      <thead>
        <tr>
          <th class="w-26">Cooperative</th>
          <th class="w-24">Platform</th>
          <th class="w-7">Account ID</th>
          <th class="w-31">Account Name</th>
          <th class="w-10">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in details" :key="index">
          <td >{{ dtl.orgName }}</td>
          <td >{{ dtl.platformName }}</td>
          <td >{{ dtl.accountId }}</td>
          <td >{{ dtl.accountName }}</td>
        
          <td class="p-1">
            <div class="buttons" sm-1 mb-0 >
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)">
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
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="detailEditorTitle"
  @show="onShowDetailEditor($event)"
  @hide="onHideDetailEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="dbs0920A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-combo v-model="detail.orgId" caption="Cooperative" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
      <sym-combo v-model="detail.platformId" caption="Platform" align="bottom" display-field="platformName" :datasource="platforms" ></sym-combo>
      
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
  getCount,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0920',

  data () {
    return {

      cluster: {
        clusterId: 0,
        clusterName: '',
        accountId: '',
        lockId: ''
      },

      details: [],

      isDetailEditorVisible: false,

      detail: {
        clusterOrgPlatformId: 0,
        clusterId: 0,
        orgId: 0,
        orgName: '',
        platformId: 0,
        platformName: '',
        accountId: '',
        accountName: ''
      },

      detailIndex: -1,
      isAddingDetail: false,

    };
  },

  computed: {
    detailEditorTitle () {
      if (this.isAddingDetail) {
        return 'Add Detail';
      }
      return 'Edit Detail';
    },

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

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.orgs = data.orgs;
            me.platforms = data.platforms;
          }
          if (me.cluster.clusterId < 0) {
            return Promise.resolve(null);
          }
          return me.getCluster();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.cluster.length) {
            me.setModels(info);
          } else {
            if (me.cluster.clusterId > -1) {
              let message = "Cluster ID '<b>" + me.cluster.clusterId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
            // me.advice.warning('You are adding a new document.', { duration: 5 });
            // 05 Feb 2025 - EMT
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'You are not allowed to create new documents.';
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
      const me = this;

      me.cluster = me.core.convertDates(info.cluster[0]);
      me.details = info.details;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldCluster = JSON.stringify(me.cluster);
      me.oldDetails = JSON.stringify(me.details);
    },

    // API calls

    getCluster () {
      return get('api/clusters/' + this.cluster.clusterId);
    },

    getReferences () {
      const me = this;

      if (me.orgs.length) {
        return Promise.resolve(true);
      }
      return get('api/references/dbs0920');
    },


    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/clusters/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/clusters/' + this.cluster.clusterId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        cluster = this.cluster,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/clusters/' + cluster.clusterId + this.getDeleteQuery(cluster.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        cluster = {};

      Object.assign(cluster, me.cluster);
      cluster.details = me.details;

      return cluster;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('cluster', 'details', 'detail');
      dc.keyField = 'clusterId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 50);
    },

    onClusterIdChanging (e) {
      e.callback = this.clusterIdCallback;
    },

    clusterIdCallback (e) {
      e.message = "Cluster ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.DbsCluster', 'clusterId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onClusterIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onClusterIdLostFocus () {
      const me = this;

      if (!me.cluster.clusterId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onClusterIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.cluster.clusterId = data.clusterId;
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
        me.advice.success('Document updated.', { duration: 5 });
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
            // set auto-generated ID returned by API so F9 works
            if (isNew && typeof success === 'number' && success > 0) {
              me.cluster.clusterId = success;
            }

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

      getSafeDeleteFlag('DbsCluster', me.cluster.clusterId)
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

      me.setActiveModel('detail');

      me.setRequiredMode(
        'orgId',
        'platformId',
        'accountId',
      );

      me.setDisplayMode(
        'accountName',
      );

      setTimeout(() => {
        this.setFocus('orgId');
      }, 200);
    },

    onHideDetailEditor () {
      const me = this;

      me.isAddingDetail = false;
      me.setActiveModel();
    },

    onEditDetail (dtl, index) {

      const d = this.detail;
      this.detailIndex = index;

      d.clusterOrgPlatformId = dtl.clusterOrgPlatformId;
      d.orgId = dtl.orgId;
      d.orgName = dtl.orgName;
      d.platformId = dtl.platformId;
      d.platformName = dtl.platformName;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.clusterOrgPlatformId = -1

      me.isDetailEditorVisible = true;
      me.isAddingDetail = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Cooperative <b>' + me.details[index].orgName + '</b> - Platform <b>' + me.details[index].platformName + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.details.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitDetail() {
      const me = this,
        d = me.detail;

      let indexOrg = me.details.findIndex(obj =>
        obj.orgId === d.orgId &&
        obj.platformId === d.platformId 
      );

      if (indexOrg > -1) {
        let message = 'Cooperative <b>' + me.details[indexOrg].orgName  + '</b> - Platform <b>' + me.details[indexOrg].platformName  + '</b> is already in the list.';
        me.dialog.fault(message, { size: 'sm' });
        return;
      }

      if (!me.isValid("dbs0920A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingDetail) {
        Object.assign(dtl, d);

        me.orgs.forEach((org) => {
          if (org.orgId == dtl.orgId) {
            dtl.orgName = org.orgName;
          }
        });

        me.platforms.forEach((platform) => {
          if (platform.platformId == dtl.platformId) {
            dtl.platformName = platform.platformName;
          }
        });

        me.details.push(dtl);
        me.clearDetailPad();
        me.advice.info("Cooperative '" + dtl.orgName + " - Platform '" + dtl.platformName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list-detail');
          me.setFocus('orgId');
        }, 100);
      } else {
        dtl = me.details[me.detailIndex];

        me.orgs.forEach((dtl) => {
          if (dtl.orgId == d.orgId) {
            d.orgName = dtl.orgName;
          }
        });

        me.platforms.forEach((dtl) => {
          if (dtl.platformId == d.platformId) {
            d.platformName = dtl.platformName;
          }
        });

        dtl.clusterOrgPlatformId = d.clusterOrgPlatformId;
        dtl.orgId = d.orgId;
        dtl.orgName = d.orgName;
        dtl.platformId = d.platformId;
        dtl.platformName = d.platformName;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;

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
          'clusterName'
        );

        me.setFocus('clusterName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.cluster) !== me.oldCluster) { return true; }
      if (JSON.stringify(me.details) !== me.oldDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.clusterOrgPlatformId = 0;
      d.orgId = 0;
      d.orgName = '';
      d.platformId = 0;
      d.platformName = '';
      d.accountId = '';
      d.accountName = '';
    },

  },

  created () {
    const me = this;

    me.oldCluster = '';
    me.oldDetails = '';
    me.orgs = [];     
    me.platforms = [];
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
#religion-editor >>> .modal-sm {
  min-width: 20%;
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

#religion-editor >>> .modal-sm {
  min-width: 30%;
}

@media (max-width: 1200px) {
  #list-religion table {
    width: 50%;
  }
}

@media (max-width: 1100px) {
  #religion-editor >>> .modal-sm {
    min-width: 40%;
  }
}

@media (max-width: 900px) {
  #religion-editor >>> .modal-sm {
    min-width: 50%;
  }

  #list-religion table {
    width: 60%;
  }
}

@media (max-width: 840px) {
  #list-detail table,
  #list-detail thead,
  #list-detail tbody,
  #list-detail th,
  #list-detail td,
  #list-detail tr {
    display: block;
  }

  #list-detail thead tr {
		position: absolute;
		top: -9999px;
		left: -9999px;
	}

  #list-detail td {
		position: relative;
		padding-left: 50%;
		white-space: normal;
		text-align: left !important;
	}

  #list-detail td:before {
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

@media (max-width: 800px) {
  #list-religion table {
    width: 70%;
  }
}

@media (max-width: 700px) {
  #religion-editor >>> .modal-sm {
    min-width: 60%;
  }

  #list-religion table {
    width: 80%;
  }
}

@media (max-width: 560px) {
  #religion-editor >>> .modal-sm {
    min-width: 65%;
  }

  #list-religion table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0920 >>> .card-body {
    padding: .5rem;
  }
}

</style>