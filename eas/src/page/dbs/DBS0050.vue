// Region Master

<template>
<section class="container p-0 w-60" :key="ts">
<sym-form id="dbs0050" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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

  <sym-text v-model="region.regionId" :caption-width="36" :input-width="20" caption="Region ID" lookupId="DbsRegion" @lostfocus="onRegionIdLostFocus" @changed="onRegionIdChanged" @searchresult="onRegionIdSearchResult"></sym-text>
  <sym-text v-model="region.regionName" :caption-width="36" :input-width="86" caption="Region Name"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 ">
      <thead>
        <tr>
          <th class="w-15">ID</th>
          <th class="w-59">Province Name</th>
          <th class="w-26">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in regionDetails" :key="index">
          <td>{{ dtl.provinceId }}</td>
          <td>{{ dtl.provinceName }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
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
  size="sm"
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
  <form id="dbs0050A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text v-model="detail.provinceId" caption="Province ID" align="bottom" @changing="onProvinceIdChanging"></sym-text>
      <sym-text v-model="detail.provinceName" caption="Province Name" align="bottom"></sym-text>
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
  name: 'dbs0050',

  data () {
    return {
      region: {
        regionId: '',
        regionName: '',
        lockId: ''
      },

      regionDetails: [],

      isDetailEditorVisible: false,

      detail: {
        provinceId: '',
        provinceName: '',
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    detailTag () {
      return 'Provinces / Districts: ' + this.regionDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Province';
      }
      return 'Edit Province';
    }

  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getRegion()
        .then( info => {
          me.stopWait(wait);
          if (info && info.region.length) {
            me.setModels(info);
          } else {   
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Region <b>' + me.region.regionId + '</b> not found.';
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
        region = info.region[0];

      me.region = region;
      me.regionDetails = info.regionDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldRegion = JSON.stringify(me.region);
      me.oldRegionDetails = JSON.stringify(me.regionDetails);
    },

    // API calls

    getRegion () {
      return get('api/regions/' + this.region.regionId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/regions/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/regions/' + this.region.regionId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        region = this.region,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/regions/' + region.regionId + this.getDeleteQuery(region.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        region = {};

      Object.assign(region, me.region);
      region.details = me.regionDetails;

      return region;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('region', 'regionDetails', 'detail');
      dc.keyField = 'regionId';
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

    onRegionIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onRegionIdLostFocus () {
      const me = this;

      if (!me.region.regionId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onRegionIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.region.regionId = data.regionId;
      me.replaceUrl();
      me.loadData();
    },

    onProvinceIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.provinceIdCallback;
    },

  provinceIdCallback(e) {
    const me = this;

    const index = me.regionDetails.findIndex(obj => obj.provinceId.trim() === e.proposedValue);
    if (index > -1) {
      e.message = `Province ID <b>${e.proposedValue}</b> is already in the list.`;
      return false;
    }

    const filter = `ProvinceId='${e.proposedValue}' AND RegionId<>'${me.region.regionId}'`;

    return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter)
      .then(region => {
        if (region && region.length) {
          e.message = `Province ID <b>${e.proposedValue}</b> already exists in the database.`;
          return false;
        }

        return true;
      })
      .catch(fault => {
        me.showFault(fault);
        return false;
      });
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

      getSafeDeleteFlag('DbsRegion', me.region.regionId)
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

      let target = 'provinceId';

      me.setActiveModel('detail');

      me.setRequiredMode(
        'provinceId',
        'provinceName'
      );

      if (!me.isAdding) {
        if (me.detail.lockId) {
          target = 'provinceName';
          me.setDisplayMode(
            'provinceId'
          );
        }
      }

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

      d.provinceId = dtl.provinceId;
      d.provinceName = dtl.provinceName;
      d.lockId = dtl.lockId;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.provinceId = me.region.regionId; 
      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      if (!me.regionDetails[index].lockId) {
        me.regionDetails.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('DbsProvince', me.regionDetails[index].provinceId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Province <b>' + me.regionDetails[index].provinceName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.regionDetails.splice(index, 1);
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

      if (!d.provinceId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (d.provinceId === me.region.regionId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (!me.isValid('dbs0050A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAdding) {
        Object.assign(dtl, d);
        me.regionDetails.push(dtl);

        me.clearDetailPad();
        me.detail.provinceId = me.region.regionId;  // set as default 'prefix'
        me.advice.info("Province / District '" + dtl.provinceName + "' added to list.", { duration: 5 });
        // me.setFocus('provinceId');
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('provinceId');
        }, 100);
      } else {
        dtl = me.regionDetails[me.detailIndex];

        dtl.provinceId = d.provinceId;
        dtl.provinceName = d.provinceName;
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
          'regionName'
        );

        me.setFocus('regionName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.region) !== me.oldRegion) { return true; }
      if (JSON.stringify(me.regionDetails) !== me.oldRegionDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.provinceId = '';
      d.provinceName = '';
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldRegion = '';
    me.oldRegionDetails = '';
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
  grid-template-columns: 2fr 8fr;
  gap: .5rem;
}

.fixed-header {
  /* overflow-y: auto; */
  overflow: auto;
  max-height: 44vh;
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

  #dbs0050 >>> .card-body {
    padding: .5rem;
  }
}

</style>