// Municipality Setup

<template>
<section class="container p-0 w-60" :key="ts">
<sym-form id="dbs0070" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <sym-text v-model="municipality.municipalityId" :caption-width="40" :input-width="22" caption="Municipality ID" lookupId="DbsMunicipality" @lostfocus="onMunicipalityIdLostFocus" @changing="onMunicipalityIdChanging" @changed="onMunicipalityIdChanged" @searchresult="onMunicipalityIdSearchResult"></sym-text>
    <sym-tag class="beige text-center border-main lg-2XXX ml-2" :width="64" v-if="municipality.locationName">{{ municipality.locationName }}</sym-tag>
  </div>
  <sym-text v-model="municipality.municipalityName" :caption-width="40" :input-width="98" caption="Municipality Name"></sym-text>
  <sym-text v-model="municipality.postalCode" :caption-width="40" :input-width="20" caption="Postal Code"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 ">
      <thead>
        <tr>
          <th class="w-15">ID</th>
          <th class="w-50">Barangay Name</th>
          <th class="w-10">Postal Code</th>
          <th class="w-15">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in municipalityDetails" :key="index">
          <td>{{ dtl.barangayId }}</td>
          <td>{{ dtl.barangayName }}</td>
          <td>{{ dtl.postalCode }}</td>
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
  <form id="dbs0070A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text v-model="detail.barangayId" caption="Barangay ID" align="bottom" @changing="onBarangayIdChanging"></sym-text>
      <sym-text v-model="detail.barangayName" caption="Barangay Name" align="bottom"></sym-text>
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
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0070',

  data () {
    return {
      municipality: {
        municipalityId: '',
        municipalityName: '',
        postalCode: '',
        provinceId: '',
        locationName: '',
        lockId: ''
      },

      municipalityDetails: [],

      isDetailEditorVisible: false,

      detail: {
        barangayId: '',
        municipalityId: '',
        barangayName: '',
        postalCode: '',
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    detailTag () {
      return 'Barangays: ' + this.municipalityDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Barangay';
      }
      return 'Edit Barangay';
    }

  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getMunicipality()
        .then( info => {
          me.stopWait(wait);
          if (info && info.municipality.length) {
            me.setModels(info);
          } else {
      
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Municiplaity <b>' + me.municipality.municipalityId + '</b> not found.';
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
        municipality = info.municipality[0];

      me.municipality = municipality;
      me.municipalityDetails = info.municipalityDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldMunicipality = JSON.stringify(me.municipality);
      me.oldMunicipalityDetails = JSON.stringify(me.municipalityDetails);
    },

    // API calls

    getMunicipality () {
      return get('api/municipalities/' + this.municipality.municipalityId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/municipalities/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/municipalities/' + this.municipality.municipalityId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        municipality = this.municipality,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/municipalities/' + municipality.municipalityId + this.getDeleteQuery(municipality.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        municipality = {};

      Object.assign(municipality, me.municipality);
      municipality.details = me.municipalityDetails;

      return municipality;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('municipality', 'municipalityDetails', 'detail');
      dc.keyField = 'municipalityId';
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

    onMunicipalityIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.municipalityIdCallback;
    },

    municipalityIdCallback (e) {
      const me = this;

      if (!me.core.isInteger(e.proposedValue)) {
        e.message = 'Municipality ID format is 7 digits.'
        return false;
      }

     return true;
    },

    onMunicipalityIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onMunicipalityIdLostFocus () {
      const me = this;

      if (!me.municipality.municipalityId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMunicipalityIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.municipality.municipalityId = data.municipalityId;
      me.replaceUrl();
      me.loadData();
    },

    onBarangayIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.barangayIdCallback;
    },

    barangayIdCallback (e) {
      const me = this;

      let index = me.municipalityDetails.findIndex(obj => obj.barangayId.trim() === e.proposedValue);
      if (index > -1) {
        e.message = 'Barangay ID <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      // return true;

      const filter = `BarangayId='${e.proposedValue}' AND municipalityId<>'${me.municipality.municipalityId}'`;

        return getList('dbo.DbsBarangay', 'BarangayId, BarangayName, MunicipalityId', '', filter)
        .then(barangay => {
          if (barangay && barangay.length) {
            e.message = `Barangay ID <b>${e.proposedValue}</b> already exists in the database.`;
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

      getSafeDeleteFlag('DbsMunicipality', me.municipality.municipalityId)
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

      let target = 'barangayId';

      me.setActiveModel('detail');

      me.setRequiredMode(
        'barangayId',
        'barangayName'
      );

      if (!me.isAdding) {
        if (me.detail.lockId) {
          target = 'barangayName';
          me.setDisplayMode(
            'barangayId'
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

      d.barangayId = dtl.barangayId;
      d.barangayName = dtl.barangayName;
      d.postalCode = dtl.postalCode;
      d.lockId = dtl.lockId;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.barangayId = me.municipality.municipalityId;  // set as default 'prefix'

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      if (!me.municipalityDetails[index].lockId) {
        me.municipalityDetails.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('DbsBarangay', me.municipalityDetails[index].barangayId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Barangay <b>' + me.municipalityDetails[index].barangayName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.municipalityDetails.splice(index, 1);
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

      if (!d.barangayId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (d.barangayId === me.municipality.municipalityId) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (!me.isValid('dbs0070A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAdding) {

        Object.assign(dtl, d);
        me.municipalityDetails.push(dtl);

        me.clearDetailPad();
        me.detail.barangayId = me.municipality.municipalityId;  // set as default 'prefix'
        me.advice.info("Barangay '" + dtl.barangayName + "' added to list.", { duration: 5 });
        // me.setFocus('barangayId');
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('barangayId');
        }, 100);
      } else {
        dtl = me.municipalityDetails[me.detailIndex];

        dtl.barangayId = d.barangayId;
        dtl.barangayName = d.barangayName;
        dtl.postalCode = d.postalCode;
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
          'municipalityName'
        );

        me.setFocus('municipalityName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.municipality) !== me.oldMunicipality) { return true; }
      if (JSON.stringify(me.municipalityDetails) !== me.oldMunicipalityDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.barangayId = '';
      d.barangayName = '';
      d.postalCode = '';
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldMunicipality = '';
    me.oldMunicipalityDetails = '';
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

@media (max-width: 640px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0070 >>> .card-body {
    padding: .5rem;
  }
}

</style>