// Holiday Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0390" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <sym-int v-model="holiday.holidayId" :caption-width="35" :input-width="20" caption="Holiday ID" lookupId="DbsHoliday" @lostfocus="onHolidayIdLostFocus" @changing="onHolidayIdChanging" @changed="onHolidayIdChanged" @searchresult="onHolidayIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>
  
  <div class="d-flex gap-2">
  <sym-text v-model="holiday.holidayName" :caption-width="35" :input-width="80" caption="Holiday Name"></sym-text>
  <sym-check v-model="holiday.nationalFlag" :caption-width="30" caption="National?" @changed="onNationalFlagChanged()"></sym-check>
  </div>

  <sym-combo v-model="holiday.dayTypeId" :caption-width="35" :input-width="80" caption="Day Type" display-field="dayTypeName" :datasource="type"> </sym-combo>
  
  <sym-memo v-model="holiday.remarks" :caption-width="35" :input-width="80" caption="Remarks" align="right"></sym-memo>
  <sym-date v-model="holiday.holidayDate" :caption-width="35" :input-width="30" caption="Date" @changing="onHolidayDateChanging"></sym-date>

  <div class="loca" v-if="!isNational">

    
    <div class="text-center border-light curved p-1 slategray mt-2">
      <span class="serif lg-3">Locations</span>
    </div>

    <div id="list-location" class="d-flex fixed-header" ref="list-location">
      <table class="light striped-even mb-0">
        <thead>
          <tr>
            <th class="w-28">Region</th>
            <th class="w-28">Province</th>
            <th class="w-28">Municipality</th>
            <th class="w-16">Action</th>
          </tr>
        </thead>
        <tbody class="white">
          <tr v-for="(dtl, index) in locations" :key="index">
            <td data-label="Region">{{ dtl.regionName }}</td>
            <td data-label="Province / District">{{ dtl.provinceName === null ? '-' : dtl.provinceName }}</td>
            <td data-label="Municipality / City">{{ dtl.municipalityName === null ? '-' : dtl.municipalityName }}</td>
            <td class="p-1">
              <div class="buttons" sm-1 mb-0 >
                <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditLocation(dtl, index)">
                  <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                </button>
                <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteLocation(index)">
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
        <button type="button" class="justify-between btn-add"  @click="onAddLocation">
          <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
        </button>
      </div>
    </div>
  </div>
<!-- Religion -->

  <div class="text-center border-light curved p-1 slategray mt-3">
    <span class="serif lg-3">Religions</span>
  </div>

  <div id="list-religion" class="d-flex fixed-header" ref="list-religion">
    <table class="light striped-even mb-0 ">
      <thead>
        <tr>
          <th class="w-70">Religion</th>
          <th class="w-30">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in religions" :key="index">
          <td>{{ dtl.religionName }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0 >
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditReligion(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteReligion(index)">
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
      <button type="button" class="justify-between btn-add" @click="onAddReligion">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>

</sym-form>

<sym-modal
  id="location-editor"
  v-model="isLocationEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="locationEditorTitle"
  @show="onShowLocationEditor($event)"
  @hide="onHideLocationEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="dbs0390A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="location-editor-boxes">
      <sym-combo v-model="location.regionId" caption="Region" align="bottom" display-field="regionName" :datasource="region" @changed="onRegionIdChanged()"></sym-combo>
      <sym-combo v-model="location.provinceId" caption="Province" align="bottom" display-field="provinceName" :datasource="province" @changed="onProvinceIdChanged()"></sym-combo>
      <sym-combo v-model="location.municipalityId" caption="Municipality" align="bottom" display-field="municipalityName" :datasource="municipality"></sym-combo>
    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitLocation()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isLocationEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>
</div>

</sym-modal>

<!-- Religion -->

<sym-modal
  id="religion-editor"
  v-model="isReligionEditorVisible"
  size="sm"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="editorReligionTitle"
  @show="onShowReligionEditor($event)"
  @hide="onHideReligionEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="dbs0390B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="religion-editor-boxes">
      <sym-combo v-model="religion.religionId" caption="Religion" align="bottom" display-field="religionName" :datasource="religionlist" @changing="onReligionIdChanging"></sym-combo>
    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitReligion()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isReligionEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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
  name: 'dbs0390',

  data () {
    return {

      holiday: {
        holidayId: 0,
        holidayName: '',
        dayTypeId: 0,
        holidayDate: null,
        nationalFlag: false,
        remarks: '',
        lockId: ''
      },

      locations: [],

      isLocationEditorVisible: false,

      location: {
        holidayLocationDetailId: 0,
        holidayId: 0,
        regionId: '',
        regionName: '',
        provinceId: '',
        provinceName: '',
        municipalityId: '',
        municipalityName: ''
      },

      locationIndex: -1,
      isAddingLocation: false,

      religions: [],

      isReligionEditorVisible: false,

      religion: {
        holidayReligionDetailId: 0,
        holidayId: 0,
        religionId: 0,
        religionName: '',
      },

      religionIndex: -1,
      isAddingReligion: false,

      regionId: '',
      province: [],
      municipality: [],

    };
  },

  computed: {

    isNational () {
      return this.holiday.nationalFlag;
    },

    locationEditorTitle () {
      if (this.isAddingLocation) {
        return 'Add Location Detail';
      }
      return 'Edit Location Detail';
    },

    editorReligionTitle () {
      if (this.isAddingReligion) {
        return 'Add Religion Detail';
      }
      return 'Edit Religion Detail';
    }

  },

  methods: {

    onNationalFlagChanged(){
      this.locations = [];
      
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.type = data.type;
            me.region = data.region;
            me.religionlist = data.religion;
          }
          if (me.holiday.holidayId < 0) {
            return Promise.resolve(null);
          }
          return me.getHoliday();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.holiday.length) {
            me.setModels(info);
          } else {
            if (me.holiday.holidayId > -1) {
              let message = "Holiday ID '<b>" + me.holiday.holidayId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
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

      me.holiday = me.core.convertDates(info.holiday[0]);
      me.locations = info.locations;
      me.religions = info.religions;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldHoliday = JSON.stringify(me.holiday);
      me.oldLocations = JSON.stringify(me.locations);
      me.oldReligions = JSON.stringify(me.religions);
    },

    // API calls

    getHoliday () {
      return get('api/holidays/' + this.holiday.holidayId);
    },

    getReferences () {
      const me = this;

      if (me.type.length) {
        
        return Promise.resolve(true);
      }
      return get('api/references/dbs0390');
    },

    getProvinces () {
      return get("api/holiday-provinces/" + this.location.regionId);
    },

    getMunicipalities () {
      const me = this;
      return get("api/holiday-municipalities/" + me.location.provinceId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/holidays/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/holidays/' + this.holiday.holidayId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        holiday = this.holiday,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/holidays/' + holiday.holidayId + this.getDeleteQuery(holiday.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        holiday = {};

      Object.assign(holiday, me.holiday);
      holiday.locations = me.locations;
      holiday.religions = me.religions;

      return holiday;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('holiday', 'locations', 'religions', 'location', 'religion');
      dc.keyField = 'holidayId';
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

    onHolidayIdChanging (e) {
      e.callback = this.holidayIdCallback;
    },

    holidayIdCallback (e) {
      e.message = "Holiday ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.DbsHoliday', 'holidayId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onHolidayIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onHolidayIdLostFocus () {
      const me = this;

      if (!me.holiday.holidayId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onHolidayIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.holiday.holidayId = data.holidayId;
      me.replaceUrl();
      me.loadData();
    },

    onHolidayDateChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.holidayDateCallback;
    },

    holidayDateCallback (e) {
      let holidayDate = e.proposedValue;
      let filter = "HolidayDate='" + holidayDate.toDateFormatISO()  + "'";
      return getCount('dbo.DbsHoliday', filter).then(
        count => {
          if (count > 0) {
          e.message = 'Definition for <b>' + holidayDate + '</b> already exists.';
          return false;
        } else {
          return true;
        }
        },
        () => {
          return true;
        }
      );

    },

    onReligionIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.religionIdCallback;
    },

    religionIdCallback (e) {
      const me = this;

      let index = me.religions.findIndex(obj => obj.religionId === e.proposedValue);
      if (index > -1) {
        e.message = 'Religion <b>' + me.religions[index].religionName + '</b> is already in the list.'
        return false;
      }

      return true;
    },

    onRegionIdChanged() {
      const me = this,
        wait = me.wait();

      me.province = [];
      me.municipality = [];

      me.getProvinces().then(
        (data) => {
          me.stopWait(wait);
          me.province = data;

          if (me.province.length) {
            me.province.unshift({
              provinceId: 0,
              provinceName: "--- Select Province ---",
            });

            me.setFocus("provinceId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onProvinceIdChanged() {
      const me = this,
        wait = me.wait();

      me.municipality = [];

      me.getMunicipalities().then(
        (data) => {
          me.stopWait(wait);
          me.municipality = data;

          if (me.municipality.length) {
            me.municipality.unshift({
              municipalityId: 0,
              municipalityName: "--- Select Municipality ---",
            });
            me.setFocus("municipalityId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
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
              me.holiday.holidayId = success;
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

      getSafeDeleteFlag('DbsHoliday', me.holiday.holidayId)
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

    onShowLocationEditor () {
      const me = this;

      me.setActiveModel('location');

      setTimeout(() => {
        this.setFocus('regionId');
      }, 200);
    },

    onHideLocationEditor () {
      const me = this;

      me.isAddingLocation = false;
      me.setActiveModel();
    },

    onEditLocation (dtl, index) {

      const d = this.location;
      this.locationIndex = index;

      d.holidayLocationDetailId = dtl.holidayLocationDetailId;
      d.regionId = dtl.regionId;
      d.regionName = dtl.regionName;
      d.provinceId = dtl.provinceId;
      d.provinceName = dtl.provinceName;
      d.municipalityId = dtl.municipalityId;
      d.municipalityName = dtl.municipalityName;

      this.onRegionIdChanged();

      if (d.provinceId) {
        this.onProvinceIdChanged();
      }

      this.isLocationEditorVisible = true;
    },

    onAddLocation () {
      const me = this;

      me.clearLocationPad();
      me.location.holidayLocationDetailId = -1

      me.isLocationEditorVisible = true;
      me.isAddingLocation = true;
    },

    onDeleteLocation (index) {
      const me = this;

      me.dialog.confirm('Region <b>' + me.locations[index].regionName + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.locations.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitLocation() {
      const me = this,
        d = me.location;

      let indexRegion = me.locations.findIndex(obj =>
        obj.regionId === d.regionId &&
        obj.provinceId === null &&
        obj.municipalityId === null &&
        d.municipalityId !== null && d.municipalityId !== undefined
      );

      if (indexRegion > -1) {
        let message = 'Region <b>' + me.locations[indexRegion].regionName + '</b> is already in the list.';
        me.dialog.fault(message, { size: 'sm' });
        return;
      }

      let index = me.locations.findIndex(obj =>
        obj.regionId === d.regionId && (d.provinceId ? obj.provinceId === d.provinceId : true) && (d.municipalityId ? obj.municipalityId === d.municipalityId : true)
      );

      if (index > -1) {
    
        let message = 'Region <b>' + d.regionName + '</b>' +
                      (d.provinceId ? ' and Province <b>' + d.provinceName + '</b>' : '') +
                      (d.municipalityId ? ' and Municipality <b>' + d.municipalityName + '</b>' : '') +
                      ' is already in the list.';
        me.dialog.fault(message, { size: 'sm' });
        return;
      }

      if (!d.regionId) {
        me.isLocationEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingLocation) {
        Object.assign(dtl, d);

        me.region.forEach((region) => {
          if (region.regionId == dtl.regionId) {
            dtl.regionName = region.regionName;
          }
        });

        me.province.forEach((province) => {
          if (province.provinceId == dtl.provinceId) {
            dtl.provinceName = province.provinceName;
          }
        });

        me.municipality.forEach((municipality) => {
          if (municipality.municipalityId == dtl.municipalityId) {
            dtl.municipalityName = municipality.municipalityName;
          }
        });

        me.locations.push(dtl);
        me.clearLocationPad();
        me.advice.info("Region '" + dtl.regionName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list-location');
          me.setFocus('regionId');
        }, 100);
      } else {
        dtl = me.locations[me.locationIndex];

        me.region.forEach((dtl) => {
          if (dtl.regionId == d.regionId) {
            d.regionName = dtl.regionName;
          }
        });

        me.province.forEach((dtl) => {
          if (dtl.provinceId == d.provinceId) {
            d.provinceName = dtl.provinceName;
          }
        });

        me.municipality.forEach((dtl) => {
          if (dtl.municipalityId == d.municipalityId) {
            d.municipalityName = dtl.municipalityName;
          }
        });

        dtl.holidayLocationDetailId = d.holidayLocationDetailId;
        dtl.regionId = d.regionId;
        dtl.regionName = d.regionName;
        dtl.provinceId = d.provinceId;
        dtl.provinceName = d.provinceName;
        dtl.municipalityId = d.municipalityId;
        dtl.municipalityName = d.municipalityName;

        me.isLocationEditorVisible = false;

      }
    },

    // Religion

    onShowReligionEditor () {
      const me = this;

      me.setActiveModel('religion');

      setTimeout(() => {
        this.setFocus('religionId');
      }, 200);
    },

    onHideReligionEditor () {
      const me = this;

      me.isAddingReligion = false;
      me.setActiveModel();
    },

    onEditReligion (dtl, index) {
      const d = this.religion;
      this.religionIndex = index;

      d.holidayReligionDetailId = dtl.holidayReligionDetailId;
      d.religionId = dtl.religionId;
      d.religionName = dtl.religionName;

      this.isReligionEditorVisible = true;
    },

    onAddReligion () {
      const me = this;

      me.clearReligionPad();
      me.religion.holidayReligionDetailId = -1

      me.isReligionEditorVisible = true;
      me.isAddingReligion = true;
    },

    onDeleteReligion (index) {
      const me = this;

      me.dialog.confirm('Religion <b>' + me.religions[index].religionName + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.religions.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitReligion() {
      const me = this,
        d = me.religion;

      if (!d.religionId) {
        me.isReligionEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingReligion) {
        Object.assign(dtl, d);

        me.religionlist.forEach((religionlist) => {
          if (religionlist.religionId == dtl.religionId) {
            dtl.religionName = religionlist.religionName;
          }
        });

        me.religions.push(dtl);
        me.clearReligionPad();
        me.advice.info("Religion '" + dtl.religionName + "' added to list.", { duration: 5 });
        setTimeout(() => {
          me.scrollToBottom('list-religion');
          me.setFocus('religionId');
        }, 100);
      } else {
        dtl = me.religions[me.religionIndex];

        me.religionlist.forEach((dtl) => {
          if (dtl.religionId == d.religionId) {
            d.religionName = dtl.religionName;
          }
        });

        dtl.holidayReligionDetailId = d.holidayReligionDetailId;
        dtl.religionId = d.religionId;
        dtl.religionName = d.religionName;

        me.isReligionEditorVisible = false;

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
          'holidayName',
          'dayTypeId',
          'holidayDate',
          'remarks'
        );

        me.setFocus('holidayName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.holiday) !== me.oldHoliday) { return true; }
      if (JSON.stringify(me.locations) !== me.oldLocations) { return true; }
      if (JSON.stringify(me.religions) !== me.oldReligions) { return true; }
      return false;
    },

    clearLocationPad () {
      const d = this.location;

      d.holidayLocationDetailId = 0;
      d.regionId = '';
      d.regionName = '';
      d.provinceId = '';
      d.provinceName = '';
      d.municipalityId = '';
      d.municipalityName = '';
    },

    clearReligionPad () {
      const d = this.religion;

      d.holidayReligionDetailId = 0;
      d.religionId = 0;
      d.religionName = '';
    }

  },

  created () {
    const me = this;

    me.oldHoliday = '';
    me.oldLocations = '';
    me.oldReligions = '';
    me.type = [];     
    me.region = [];
    me.religionlist = [];
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
  #list-location table,
  #list-location thead,
  #list-location tbody,
  #list-location th,
  #list-location td,
  #list-location tr {
    display: block;
  }

  #list-location thead tr {
		position: absolute;
		top: -9999px;
		left: -9999px;
	}

  #list-location td {
		position: relative;
		padding-left: 50%;
		white-space: normal;
		text-align: left !important;
	}

  #list-location td:before {
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

  #dbs0390 >>> .card-body {
    padding: .5rem;
  }
}

</style>