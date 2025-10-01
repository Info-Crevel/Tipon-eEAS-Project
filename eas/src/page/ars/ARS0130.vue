// Billing Rate Sheet

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars0130" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
      <button type="button" :class="backButtonClass" class="justify-between btn-back"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
    </div>
  </div>

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="billing.clientBillingDetailId" :caption-width="46" :input-width="20" caption="Billing ID" lookupId="ArsClientBilling" @lostfocus="onClientBillingDetailIdLostFocus" @changing="onClientBillingDetailIdChanging" @changed="onClientBillingDetailIdChanged" @searchfill="onClientBillingDetailIdSearchFill" @searchresult="onClientBillingDetailIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" v-if="refClientPayGroupId" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>
  <sym-combo v-model="billing.payTrxCode" :caption-width="46" :input-width="50" caption="Trx Name" display-field="payTrxName" :datasource="payTrxs" @changed="onPayTrxCodeChanged"></sym-combo>
  <sym-check v-model="billing.orgFlag" :caption-width="46" caption="Shouldered By Coop" @changed="onOrgFlagChanged"></sym-check>  
  <sym-dec v-model="billing.adminFee" v-show="!isOrg" :caption-width="46" :input-width="30" caption="Admin Fee ( % )" ></sym-dec>          
 <sym-text v-model="billing.billingSheetFormula" :caption-width="46" :input-width="70" caption="Formula (A,B,C,D, etc):"></sym-text> 

 <sym-tabs id="rate-info-tabs" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">
     

    <sym-tab title="Basic Rate (A)">
      <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
        <sym-table-header slot="header">
          <tr class="align-top">
            <th class="w-10 text-center"><i class="fa fa-check"></i></th>
            <th class="w-90">Name</th>
          </tr>
          
        </sym-table-header>
          <sym-table-body slot="body" v-show="billing.clientBillingDetailId !=0">
            <tr v-for="(basicRate, index) in basicRateList" :key="index" class="align-top" @click="onTogglePayTrxCodeSelection(basicRate)">
              <td class="w-10 text-center"><input type="checkbox" :checked="isPayTrxCodeSelected(basicRate)"></td>
              <td class="w-90">{{ basicRate.payTrxName }}</td>
            </tr>
          </sym-table-body>
        </sym-table>

  </sym-tab>




  <sym-tab title="De Minimis (B)">
      <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
        <sym-table-header slot="header">
          <tr class="align-top">
<th class="w-10 text-center">
      <input
        type="checkbox"
        :checked="areAllDeminimisSelected"
        @change="onToggleSelectAllDeminimis"
      />
    </th>


            <th class="w-90">Name</th>
          </tr>
          
        </sym-table-header>
          <sym-table-body slot="body" v-show="billing.clientBillingDetailId !=0">
            <tr v-for="(deminimis, index) in deminimisList" :key="index" class="align-top" @click="onToggleDeminimisIdSelection(deminimis)">
              <td class="w-10 text-center"><input type="checkbox" :checked="isDeminimisIdSelected(deminimis)"></td>
              <td class="w-90">{{ deminimis.deminimisName }}</td>
            </tr>
          </sym-table-body>
        </sym-table>

  </sym-tab>


  <sym-tab title="Allowances (C)">
      <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
        <sym-table-header slot="header">
          <tr class="align-top">
          <th class="w-10 text-center">
              <input
                type="checkbox"
                :checked="areAllAllowancesSelected"
                @change="onToggleSelectAllAllowances"
              />
            </th>


            <th class="w-90">Name</th>
          </tr>
          
        </sym-table-header>
          <sym-table-body slot="body" v-show="billing.clientBillingDetailId !=0">
            <tr v-for="(allowances, index) in allowanceList" :key="index" class="align-top" @click="onToggleAllowanceIdSelection(allowances)">
              <td class="w-10 text-center"><input type="checkbox" :checked="isAllowanceIdSelected(allowances)"></td>
              <td class="w-90">{{ allowances.allowanceName }}</td>
            </tr>
          </sym-table-body>
        </sym-table>

  </sym-tab>


  <sym-tab title="Day Types (D)">
      <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
        <sym-table-header slot="header">
          <tr class="align-top">
            <!-- <th class="w-10 text-center"><i class="fa fa-check" ></i></th>  -->
          <th class="w-10 text-center">
              <input
                type="checkbox"
                :checked="areAllDayTypesSelected"
                @change="onToggleSelectAllDayTypes"
              />
            </th>


            <th class="w-90">Name</th>
          </tr>
          
        </sym-table-header>
          <sym-table-body slot="body" v-show="billing.clientBillingDetailId !=0">
            <tr v-for="(dayTypes, index) in dayTypeSheet" :key="index" class="align-top" @click="onToggleDayTypeIdSelection(dayTypes)">
              <td class="w-10 text-center"><input type="checkbox" :checked="isDayTypeIdSelected(dayTypes)"></td>
              <td class="w-90">{{ dayTypes.dayTypeName }}</td>
            </tr>
          </sym-table-body>
        </sym-table>

  </sym-tab>



</sym-tabs>
            
</sym-form>
      <sym-modal
  id="dayType-editor"
  v-model="isDayTypeEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorDayTypeTitle"
  @show="onShowDayTypeEditor($event)"
  @hide="onHideDayTypeEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0040C" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="day-editor-boxes">
        <sym-int v-model="dayType.dayTypeId" caption="Day Type ID" align="bottom" lookupId="DbsDayType" @changing="onDayTypeIdChanging" @searchresult="onDayTypeIdSearchResult"></sym-int>
        <sym-text v-model="dayType.dayTypeName" caption="Day Type Name" align="bottom" ></sym-text>
        <sym-dec v-model="dayType.premiumPercentage" caption="Percentage" align="bottom" ></sym-dec>
  
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitDayType()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isDayTypeEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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
import { disable } from '../../js/dom';

export default {
  extends: PageMaintenance,
  name: 'ars0130',

  data () {
    return {

      billing: {
        clientBillingDetailId: 0,
        clientPayGroupId: 0,
        payTrxCode: '',
        payTrxName: '',
        adminFee: 0,
        orgFlag: false,
        billingSheetFormula: '',
        lockId: ''
      },

      deminimis: [],
      allowances: [], 
      basicRate: [],

      activeTabIndex: 0,

      dayTypes: [],

      dayType: {
        billingDayTypeId: 0,
        clientBillingDetailId: 0,
        dayTypeId: 0,
        dayTypeName: '',
        premiumPercentage: 0,
        adminFee: 0,
        lockId: ''
       
      },

      dayTypeIndex: -1,
      isAddingDayType: false,
      isDayTypeEditorVisible: false,
      
      refClientPayGroupId:0,

    };
  },

  computed: {
  
  areAllDayTypesSelected() {
    if (!this.dayTypes.length) {
      return false;
    }

    const notSelected = this.dayTypes.filter(item =>
      !this.dayTypes.some(a => a.dayTypeId === item.dayTypeId)
    );

    return notSelected.length === 0;
  },



  areAllDeminimisSelected() {
    if (!this.deminimis.length) {
      return false;
    }

    const notSelected = this.deminimis.filter(item =>
      !this.deminimis.some(a => a.deminimisId === item.deminimisId)
    );

    return notSelected.length === 0;
  },


    areAllAllowancesSelected() {
    if (!this.allowances.length) {
      return false;
    }

    const notSelected = this.allowances.filter(item =>
      !this.allowances.some(a => a.allowanceId === item.allowanceId)
    );

    return notSelected.length === 0;
  },

  isOrg () {
      return this.billing.orgFlag;
    },

    detailEditorTitle () {
      if (this.isAddingDetail) {
        return 'Add Detail';
      }
      return 'Edit Detail';
    },

    editorDayTypeTitle () {
      if (this.isAddingDayType) {
        return 'Add Day Type Detail';
      }
      return 'Edit Day Type Detail';
    },


  },

  methods: {

    onToggleSelectAllDayTypes() {
    const allSelected = this.areAllDayTypesSelected;
    if (allSelected) {
      const idsToRemove = this.dayTypeSheet.map(a => a.dayTypeId);

      for (let i = this.dayTypes.length - 1; i >= 0; i--) {
        if (idsToRemove.includes(this.dayTypes[i].dayTypeId)) {
          this.dayTypes.splice(i, 1);
        }
      }

    } else {
      this.dayTypeSheet.forEach(a => {
        const exists = this.dayTypes.some(sel => sel.dayTypeId === a.dayTypeId);
        if (!exists) {
          this.dayTypes.push({
            clientBillingDetailId: this.billing.clientBillingDetailId,
            dayTypeId: a.dayTypeId
          });
        }
      });
    }
  }
,


    isDayTypeIdSelected (dayTypes) { 
      return this.dayTypes.findIndex(obj => obj.dayTypeId === dayTypes.dayTypeId) > -1;
    },

    onToggleDayTypeIdSelection (dayTypes) {
      const
        me = this,
        index = me.dayTypes.findIndex(obj => obj.dayTypeId === dayTypes.dayTypeId);

      if (index > -1) {
        me.dayTypes.splice(index, 1);
      } else {
        me.dayTypes.push({
          clientBillingDetailId: me.billing.clientBillingDetailId,
          dayTypeId: dayTypes.dayTypeId
        }); 
      }
    },


    onToggleSelectAllDeminimis() {
    const allSelected = this.areAllDeminimisSelected;

    if (allSelected) {
      const idsToRemove = this.deminimisList.map(a => a.deminimisId);

      for (let i = this.deminimis.length - 1; i >= 0; i--) {
        if (idsToRemove.includes(this.deminimis[i].deminimisId)) {
          this.deminimis.splice(i, 1);
        }
      }

    } else {
      this.deminimisList.forEach(a => {
        const exists = this.deminimis.some(sel => sel.deminimisId === a.deminimisId);
        if (!exists) {
          this.deminimis.push({
            clientBillingDetailId: this.billing.clientBillingDetailId,
            deminimisId: a.deminimisId
          });
        }
      });
    }
  }
,


    onToggleSelectAllAllowances() {
    const allSelected = this.areAllAllowancesSelected;

      if (allSelected) {
      
      const idsToRemove = this.allowanceList.map(a => a.allowanceId);

      for (let i = this.allowances.length - 1; i >= 0; i--) {
        if (idsToRemove.includes(this.allowances[i].allowanceId)) {
          this.allowances.splice(i, 1);
        }
      }

    } else {
      this.allowanceList.forEach(a => {
        const exists = this.allowances.some(sel => sel.allowanceId === a.allowanceId);
        if (!exists) {
          this.allowances.push({
            clientBillingDetailId: this.billing.clientBillingDetailId,
            allowanceId: a.allowanceId
          });
        }
      });
    }
  }
,

    getTargetPath() {
      const me = this,
        q = {};

      if (me.billing.clientPayGroupId) {
        q.clientPayGroupId = me.billing.clientPayGroupId;
        me.refClientPayGroupId = q.clientPayGroupId;
      }
      
      
      if (me.billing.clientPayOutDetailId) {
        q.clientPayOutDetailId = me.billing.clientPayOutDetailId;
      }
 

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("clientPayGroupId" in q && me.core.isInteger(q.clientPayGroupId)) {
        me.billing.clientPayGroupId = parseInt(q.clientPayGroupId);     
        me.refClientPayGroupId = parseInt(q.clientPayGroupId);
      }

      if ("clientBillingDetailId" in q && me.core.isInteger(q.clientBillingDetailId)) {       
        me.billing.clientBillingDetailId = parseInt(q.clientBillingDetailId);
      }

     },


    onPayTrxCodeChanged (newValue) {
      const me = this;
    
      let o = me.payTrxs.find( o => o.payTrxCode == newValue);
      if (o) {
        me.billing.billingSheetFormula = o.trxFormula;

      }
    },

    onOrgFlagChanged(newValue){
      if (newValue===true) {
      this.billing.adminFee = 0;
      }
   },
    
    onDayTypeIdChanging (e) {
      e.message = "Day Type ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.dayTypeIdCallback;
    },

    dayTypeIdCallback (e) {
      const me = this;
      let filter = "DayTypeId=" + e.proposedValue;
      return getList('dbo.DbsDayType', 'DayTypeId, DayTypeName, PremiumPercentage', '', filter).then(
        info => {
          if (info && info.length) {
               
            let index = me.dayTypes.findIndex(obj => obj.dayTypeId === e.proposedValue);
            if (index > -1) {
              e.message = 'Day Type ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }
            
            me.dayType.dayTypeName = dayType[0].dayTypeName;
            me.dayType.premiumPercentage = dayType[0].premiumPercentage;
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

    onDayTypeIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        dayType = this.dayType;
        let index = this.dayTypes.findIndex(obj => obj.dayTypeId === data.dayTypeId);
        if (index > -1) {
          this.advice.fault('Day Type ID <b>' + data.dayTypeId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        dayType.dayTypeId = data.dayTypeId;
        dayType.dayTypeName = data.dayTypeName;
        dayType.premiumPercentage = data.premiumPercentage;
 
        this.focusNext();

    },
 
    onShowDayTypeEditor () {
      const me = this;
      
      me.setActiveModel('dayType');

      me.setRequiredMode(
        'dayTypeId',
        'premiumPercentage'
      );

      me.setDisplayMode(
        'dayTypeName'
      );

      setTimeout(() => {
        this.setFocus('dayTypeId');
      }, 200);
    },

    onHideDayTypeEditor () {
      const me = this;

      me.isAddingDayType = false;
      me.setActiveModel();
    },

    onEditDayType (dtl, index) {

      const d = this.dayType;
      this.dayTypeIndex = index;

      d.billingDayTypeId = dtl.billingDayTypeId;
      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeName = dtl.dayTypeName;

      d.premiumPercentage = dtl.premiumPercentage;
      d.adminFee = dtl.adminFee;
      
      this.isDayTypeEditorVisible = true;
    },

    onAddDayType () {
      const me = this;
      
      me.clearDayTypePad();
      
      me.dayType.billingDayTypeId = -1 

      me.isDayTypeEditorVisible = true;
      me.isAddingDayType = true;
        
    },

    onSubmitDayType() {
      const me = this,
        d = me.dayType;

      if (!d.dayTypeId) {
        me.isDayTypeEditorVisible = false;
        return;
      }

      if (!me.isValid("ars0040C")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingDayType) {
 
        Object.assign(dtl, d);
        me.dayTypes.push(dtl);

        me.clearDayTypePad();
        
        me.advice.info("Day Type '" + dtl.dayTypeId + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("dayTypeId");
        
      } else {

        dtl = me.dayTypes[me.dayTypeIndex];

        dtl.billingDayTypeId = d.billingDayTypeId;
        dtl.dayTypeId = d.dayTypeId;
        dtl.dayTypeName = d.dayTypeName;
        dtl.premiumPercentage = d.premiumPercentage;
        dtl.adminFee = d.adminFee;
   
        me.isDayTypeEditorVisible = false;

      }
    },

    onDeleteDayType(index) {
      const me = this;

      me.dialog.confirm( "Day Type <b>" + me.dayTypes[index].dayTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.dayTypes.splice(index, 1);
          }
          return;
        });
    },

    clearDayTypePad () {
      const d = this.dayType;

      d.billingDayTypeId = 0;
      d.dayTypeId = 0;
      d.dayTypeName = '';

      d.premiumPercentage = 0;
      d.adminFee = 0;
    
    },

    onActiveTabIndexChanged () {
      this.reload();
    },

    isPayTrxCodeSelected (basicRate) { 
      return this.basicRate.findIndex(obj => obj.payTrxCode === basicRate.payTrxCode) > -1;
    },

    onTogglePayTrxCodeSelection (basicRate) {
      const
        me = this,
        index = me.basicRate.findIndex(obj => obj.payTrxCode === basicRate.payTrxCode);

      if (index > -1) {
        me.basicRate.splice(index, 1);
      } else {
        me.basicRate.push({
          clientBillingDetailId: me.billing.clientBillingDetailId,
          payTrxCode: basicRate.payTrxCode
        });
      }
    },

    isAllowanceIdSelected (allowances) { 
      return this.allowances.findIndex(obj => obj.allowanceId === allowances.allowanceId) > -1;
    },

    onToggleAllowanceIdSelection (allowances) {
      const
        me = this,
        index = me.allowances.findIndex(obj => obj.allowanceId === allowances.allowanceId);

      if (index > -1) {
        me.allowances.splice(index, 1);
      } else {
        me.allowances.push({
          clientBillingDetailId: me.billing.clientBillingDetailId,
          allowanceId: allowances.allowanceId
        }); 
      }
    },

    isDeminimisIdSelected (deminimis) { 
      return this.deminimis.findIndex(obj => obj.deminimisId === deminimis.deminimisId) > -1;
    },

    onToggleDeminimisIdSelection (deminimis) {
      const
        me = this,
        index = me.deminimis.findIndex(obj => obj.deminimisId === deminimis.deminimisId);

      if (index > -1) {
        me.deminimis.splice(index, 1);
      } else {
        me.deminimis.push({
          clientBillingDetailId: me.billing.clientBillingDetailId,
          deminimisId: deminimis.deminimisId
        });
      }
    },

    
    loadData () {
      const
        me = this,
        wait = me.wait();

        this.billing.clientPayGroupId = me.refClientPayGroupId;

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.payTrxs = data.payTrxs;
            me.deminimisList = data.deminimis;
            me.allowanceList = data.allowances;
            me.basicRateList = data.basicRate;
            me.dayTypeSheet = data.dayTypeSheet;
            me.refresh();
          }
          if (me.billing.clientBillingDetailId < 0) {
            return Promise.resolve(null);
          }
          return me.getClientBilling();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.billing.length) {
            me.setModels(info);
          } else {
            if (me.billing.clientBillingDetailId > -1) {
              let message = "Billing ID '<b>" + me.billing.clientBillingDetailId + "</b>' not found.";
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

      me.billing = info.billing[0];
      me.deminimis = info.billingDeminimis;
      me.allowances = info.billingAllowances;
      me.basicRate = info.billingBasicRate;
      me.dayTypes = info.dayTypes; 
      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldBilling = JSON.stringify(me.billing);
      me.oldDeminimis = JSON.stringify(me.deminimis);
      me.oldAllowances = JSON.stringify(me.allowances);
      me.oldBasicRate = JSON.stringify(me.basicRate);
      me.oldDayTypes = JSON.stringify(me.dayTypes);
    },

    // API calls

    getClientBilling () {
      return get('api/client-pay-group-billings/' + this.billing.clientBillingDetailId);
    },

    getReferences () {
      const me = this;

      if (me.payTrxs.length) {
        return Promise.resolve(true);
      }
      return get('api/references/ars0130');
    },


    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/client-pay-group-billings/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/client-pay-group-billings/' + this.billing.clientBillingDetailId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        billing = this.billing,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/client-pay-group-billings/' + billing.clientBillingDetailId + this.getDeleteQuery(billing.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        billing = {};
        Object.assign(billing, me.billing);
        billing.deminimis = me.deminimis;
        billing.allowances = me.allowances;
        billing.basicRate = me.basicRate;
        billing.dayTypes = me.dayTypes;
      return billing;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('billing', 'deminimis', 'allowances','basicRate','dayTypes','dayType');
      dc.keyField = 'clientBillingDetailId';
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

    onClientBillingDetailIdChanging (e) {
      e.callback = this.clientBillingDetailIdCallback;
    },

    clientBillingDetailIdCallback (e) {
      const me = this;
      let filter = "ClientBillingDetailId='" + e.proposedValue + "' AND ClientPayGroupId = " + this.refClientPayGroupId;
      return getList('dbo.ArsClientBilling', 'ClientBillingDetailId, ClientPayGroupId', '', filter).then(
        billing => {
          if (billing && billing.length) {
            me.billing.clientBillingDetailId = billing[0].clientBillingDetailId;
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

    onClientBillingDetailIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onClientBillingDetailIdLostFocus () {
      const me = this;

      if (!me.billing.clientBillingDetailId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onClientBillingDetailIdSearchFill (e) {
      e.filter = "ClientPayGroupId = " + this.refClientPayGroupId;
    },


    onClientBillingDetailIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.billing.clientBillingDetailId = data.clientBillingDetailId;
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

      if (!me.billing.orgFlag && me.billing.adminFee==0) {
       me.advice.fault('Admin fee is required before saving.', { duration: 5 } )
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
            if (isNew && typeof success === 'number' && success > 0) {
              me.billing.clientBillingDetailId = success;
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

      getSafeDeleteFlag('ArsClientBilling', me.billing.clientBillingDetailId)
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
  
    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'payTrxCode',
          'billingSheetFormula',
          'clientPayGroupId',
        );

        me.setFocus('payTrxCode');
      }, 100);

    },

    hasChanges () {
      const me = this;

      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.billing) !== me.oldBilling) { return true; }
      if (JSON.stringify(me.deminimis) !== me.oldDeminimis) { return true; }
      if (JSON.stringify(me.allowances) !== me.oldAllowances) { return true; }
      if (JSON.stringify(me.basicRate) !== me.oldBasicRate) { return true; }
      if (JSON.stringify(me.dayTypes) !== me.oldDayTypes) { return true; }
      return false;
    },

  },

  created () {
    const me = this;

    me.oldBilling = '';
    me.oldDeminimis = '';
    me.oldAllowances = '';
    me.oldBasicRate = '';
    me.oldDayTypes = '';
    me.payTrxs = []; 
    me.deminimisList = [];
    me.allowanceList = [];
    me.basicRateList = [];
    me.dayTypeSheet = []; 
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

  #ars0130 >>> .card-body {
    padding: .5rem;
  }
}

</style>