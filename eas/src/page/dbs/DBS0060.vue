// Province Definition

<template>
<section class="container p-0 w-60" :key="ts">
<sym-form id="dbs0060" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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

  <div class="tag-container">
    <sym-text v-model="province.provinceId" :caption-width="36" :input-width="20" caption="Province ID" lookupId="DbsProvince" @lostfocus="onProvinceIdLostFocus" @changing="onProvinceIdChanging" @changed="onProvinceIdChanged" @searchresult="onProvinceIdSearchResult"></sym-text>
    <sym-tag class="beige text-center border-main lg-2 ml-2" :width="54" v-if="locationName">{{ locationName }}</sym-tag>
  </div>
  <sym-text v-model="province.provinceName" :caption-width="36" :input-width="86" caption="Province Name"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 ">
      <thead>
        <tr>
          <th class="w-10">ID</th>
          <th class="w-80">Municipality / City Name</th>
  
          <th class="w-15">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in provinceDetails" :key="index">
          <td>{{ dtl.municipalityId }}</td>
          <td>{{ dtl.municipalityName }}</td>
        
          <td class="p-1">
            <div class="d-flex justify-center gap" sm-1 mb-0>
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
  :title="editorTitle"
  @show="onShowDetailEditor($event)"
  @hide="onHideDetailEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="dbs0060A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text v-model="detail.municipalityId" caption="Municipality ID" align="bottom" @changing="onMunicipalityIdChanging"></sym-text>
      <sym-text v-model="detail.municipalityName" caption="Municipality / City Name" align="bottom"></sym-text>
      <sym-text v-model="detail.postalCode" caption="Postal Code" align="bottom"></sym-text>
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
  getCount,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0060',

  data () {
    return {
      province: {
        provinceId: '',
        provinceName: '',
       
        lockId: ''
      },

      provinceDetails: [],

      isDetailEditorVisible: false,

      detail: {
        municipalityId: '',
        municipalityName: '',
        postalCode: '',
        provinceId: '',
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false,
      locationName: ''

    };
  },

  computed: {
    
    detailTag () {
      return 'Municipalities / Cities: ' + this.provinceDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Municipality / City';
      }
      return 'Edit Municipality / City';
    }

  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();
      
      let provinceId = me.province.provinceId;
   
      let filter = "ProvinceId='" + provinceId + "'";

      getList('dbo.QDbsProvince', 'RegionId, RegionName', '', filter)
        .then( data => {
          if (data && data.length) {
            me.locationName = data[0].regionName;
        
          } else {
            me.locationName = '';
          }
          return me.getProvince();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.province.length) {
            me.setModels(info);
          } else {
        
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Province <b>' + me.province.provinceId + '</b> not found.';
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
        province = info.province[0];

      me.province = province;
      me.provinceDetails = info.provinceDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldProvince = JSON.stringify(me.province);
      me.oldProvinceDetails = JSON.stringify(me.provinceDetails);
    },

    // API calls

    getProvince () {
      return get('api/provinces/' + this.province.provinceId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/provinces/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/provinces/' + this.province.provinceId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        province = this.province,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/provinces/' + province.provinceId + this.getDeleteQuery(province.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        province = {};

      Object.assign(province, me.province);
      province.details = me.provinceDetails;

      return province;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('province', 'provinceDetails', 'detail');
      dc.keyField = 'provinceId';
      dc.autoAssignKey = false;
    },

    onResetAfter () {
      this.locationName = '';
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 100);
    },

    onProvinceIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.provinceIdCallback;
    },

    provinceIdCallback (e) {
      const me = this;
      let provinceId = e.proposedValue.substring(0, 2);
      let filter = "ProvinceId='" + provinceId + "'";
      return getCount('dbo.DbsProvince', filter).then(
        count => {
          e.message = '<b>' + provinceId + '</b> is not a valid Region ID.';
          return count > 0;
        },
        () => {
          return false;
        }
      );

    },

    onProvinceIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onProvinceIdLostFocus () {
      const me = this;

      if (!me.province.provinceId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onProvinceIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.province.provinceId = data.provinceId;
      me.replaceUrl();
      me.loadData();
    },

    onMunicipalityIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.municipalityIdCallback;
    },

    municipalityIdCallback (e) {
      const me = this;


      let index = me.provinceDetails.findIndex(obj => obj.municipalityId.trim() === e.proposedValue);
      if (index > -1) {

              
        e.message = 'Municipality ID <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      const filter = `MunicipalityId='${e.proposedValue}' AND provinceId<>'${me.province.provinceId}'`;

      return getList('dbo.DbsMunicipality', 'MunicipalityId, MunicipalityName, ProvinceId', '', filter)
      .then(municipality => {
        if (municipality && municipality.length) {
          e.message = `Municipality ID <b>${e.proposedValue}</b> already exists in the database.`;
          return false;
        }

        return true;
      })
      .catch(fault => {
        me.showFault(fault);
        return false;
      });

      // return true;
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

      getSafeDeleteFlag('DbsProvince', me.province.provinceId)
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

      let target = 'municipalityId';

      me.setActiveModel('detail');

      me.setRequiredMode(
        'municipalityId',
        'municipalityName',

      );

      if (!me.isAdding) {
        if (me.detail.lockId) {
          target = 'municipalityName';
          me.setDisplayMode(
            'municipalityId'
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

      d.municipalityId = dtl.municipalityId;
      d.municipalityName = dtl.municipalityName;
      d.postalCode = dtl.postalCode;
      d.provinceId = dtl.provinceId;
      d.lockId = dtl.lockId;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.municipalityId = me.province.provinceId  // set as default 'prefix'

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      if (!me.provinceDetails[index].lockId) {
        me.provinceDetails.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('DbsMunicipality', me.provinceDetails[index].municipalityId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Municipality / City <b>' + me.provinceDetails[index].municipalityName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.provinceDetails.splice(index, 1);
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

      if (!d.municipalityId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (d.municipalityId === me.province.provinceId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (!me.isValid('dbs0060A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAdding) {

        Object.assign(dtl, d);
        me.provinceDetails.push(dtl);

        me.clearDetailPad();
        me.detail.municipalityId = me.province.provinceId  
        me.advice.info("Municipality / City '" + dtl.municipalityName + "' added to list.", { duration: 5 });
    
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('municipalityId');
        }, 100);
      } else {
        dtl = me.provinceDetails[me.detailIndex];

        dtl.municipalityId = d.municipalityId;
        dtl.municipalityName = d.municipalityName;
        dtl.postalCode = d.postalCode;
        dtl.provinceId = d.provinceId;
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
          'provinceName'
        );

        me.setFocus('provinceName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.province) !== me.oldProvince) { return true; }
      if (JSON.stringify(me.provinceDetails) !== me.oldProvinceDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.municipalityId = '';
      d.municipalityName = '';
      d.postalCode = '';
      d.provinceId = '';
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldProvince = '';
    me.oldProvinceDetails = '';
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
  grid-template-columns: 4fr 9fr 4fr;
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

.tag-container {
  display: flex;
  flex-direction: row;
}

@media (max-width: 1200px) {
  .fixed-header table {
    width: 70%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 80%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 90%;
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

  #dbs0060 >>> .card-body {
    padding: .5rem;
  }
}

</style>