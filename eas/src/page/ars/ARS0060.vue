// Member Request Monitoring

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars0060" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
    </div>
  </div>
  <div class="box-container">
    <div class="text-center border-light curved p-1 slategray">
      <span class="serif lg-3">Filter/s</span>
    </div>
  <div class="box-column">
  <sym-combo v-model="orgId" align="bottom" :caption-width="46" caption="Cooperative" display-field="orgName" :datasource="orgs"></sym-combo>    
  <sym-combo v-model="platformId" align="bottom" :caption-width="46" caption="Business Platform" display-field="platformName" :datasource="platforms"></sym-combo>    
  
  <sym-text  v-model="memberRequestPositionName" caption="Internal Position" align="bottom" :caption-width="35" list="names" ></sym-text>

    <datalist id="names"><option v-for="item in memberRequestReferenceList" :key="item.memberRequestPositionName" :value="item.memberRequestPositionName" @input="e => memberRequestPositionName = e.toUpperCase()" ></option></datalist> 
  <sym-combo v-model="employmentTypeId" align="bottom" :caption-width="45" caption="Employment Type" display-field="employmentTypeName" :datasource="employmentTypes"></sym-combo> 
  <sym-combo v-model="memberRequestStatusId" align="bottom" :caption-width="45" caption="Request Status" display-field="memberRequestStatusName" :datasource="requestStatus"></sym-combo> 
  </div>
  <div class="box-column-2">
  <sym-int v-model="clientContractId" align="bottom" :caption-width="5" caption="Contract #" display-field="clientContractName" :datasource="contracts" lookupId="ArsClientContract" @changing="onClientContractIdChanging" @searchresult="onClientContractIdSearchResult"></sym-int> 
  <sym-text v-model="clientContractName" :disabled="true" align="bottom" :caption-width="5" caption="Client Contract Name" ></sym-text>       

 <sym-int v-model="clientPayGroupId" align="bottom" :caption-width="5" caption="Pay Group ID" display-field="clientPayGroupName" :datasource="payGroups" lookupId="ArsClientPaygroup" @changing="onClientPayGroupIdChanging" @searchresult="onClientPayGroupIdSearchResult"></sym-int> 
  <sym-text v-model="clientPayGroupName" :disabled="true" align="bottom" :caption-width="5" caption="Pay Group Name" ></sym-text>       

  </div>
   <div class="box-date-column">
  <sym-date   v-model="deploymentDate" caption="Deployment Date" :caption-width="60" ></sym-date>
  <div class=" d-flex gap " :info-light="isMono" :outline="isMono"  border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class=" applicant-btn mb-2 success-light" @click="onSearch">
        <i class="fa fa-search fa-lg"></i><span class="bold">Search</span>
      </button>
      <button type="button" :class="logButtonClass" class=" applicant-btn mb-2 success-light" @click="onResetFilter">
      <i class="fa fa-undo  fa-lg"></i><span class="bold">Reset</span>
      </button>
  </div>
  </div>
  </div>
  <div class="box-table-container">



  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>


  <div class="table-scroll-wrapper">
    <table class="table-scroll light striped-even mb-0 "> 
      <thead>
        <tr class="align-top">
          <th class="w-3">MRF #</th>
          <th class="w-17">MRF Name</th>
          <th class="w-5">Status</th>
          <th class="w-5">Cooperative</th>
          <th class="w-5">Platform</th>
          <th class="w-10">Contract Name</th>
          <th class="w-10">Pay Group Name</th>
          <th class="w-15">Internal Position</th>
          <th class="w-5">Emp Type</th>
          <th class="w-5">Days</th>
          <th class="w-5">Vacancy</th>
          <th class="w-5">Total Hired</th>
          <th class="w-5">Total Vacancy</th>
          <th class="w-5">Deployment</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr class="align-top" v-for="(dtl, index) in memberRequestList" :key="index">
          <td @click="onClickMemberRequest(dtl.memberRequestId)">{{ dtl.memberRequestId }}</td>

          <td>{{ dtl.memberRequestName }}</td>
          <td>{{ dtl.memberRequestStatusName }}</td>
          <td>{{ dtl.orgName }}</td>
          <td>{{ dtl.platformName }}</td>
          <td>{{ dtl.clientContractName }}</td>
          <td>{{ dtl.clientPayGroupName }}</td>
          
          <td>{{ dtl.memberRequestPositionName }}</td>
          <td>{{ dtl.employmentTypeName }}</td>
          <td>{{ dtl.workingDays }}</td>
          <td>{{ dtl.vacancyCount }}</td>
          <td>{{ dtl.totalHired }}</td>
          <td>{{ dtl.totalVacancy }}</td>
          <td>{{ dtl.deploymentDate }}</td>
        </tr>
      </tbody>
    </table>
  </div>
      </div>

  <div class="d-flex justify-center mt-3" v-if="!memberRequestList.length">
    <sym-alert class="info w-100 text-center" icon="none">
      <span>No records found. File is empty.</span>
    </sym-alert>
  </div>

</sym-form>
</section>
</template>

<script>

import {
  get,
  ajax
} from '../../js/http';

import {
  getList
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'ars0060',

  data () {
    return {

      memberRequestList: [],
      memberRequestReferenceList: [],
      memberRequestName:'', 
      orgId:0,

      platformId: 0,
      clientContractId:0,
      clientContractName: '',

      clientPayGroupId:0,
      clientPayGroupName: '',


      memberRequestPositionName:'', 
      deploymentDate:null,
      memberRequestStatusId: 0,
      endDate:null,
      employmentTypeId:0,
    };
  },

  computed: {

    detailTag () {
      return 'Member Request/s: ' + this.memberRequestList.length;
    },
  },

  methods: {

    onClickMemberRequest(memberRequestId) {
     const me = this;

  const selectedRequest = this.memberRequestList.find(
    (req) => req.memberRequestId === memberRequestId
  );

  if (selectedRequest) {
    let route;

    if (selectedRequest.memberRequestStatusName === 'Active') {
      route = {
        name: "ars0050",
        query: {
          memberRequestId: memberRequestId,
        },
      };
    } else {
      route = {
        name: "ars0030",
        query: {
          memberRequestId: memberRequestId,
          activeTabIndex: 1,
        },
      };
    }

    me.go(route);
  }
    },


    setupControls () {
      const me = this;

      setTimeout(() => {

        me.setDisplayMode(
          'clientContractName',
          'clientPayGroupName'
        );
        me.setFocus('memberRequestName');
      }, 100);

    },



    onResetFilter() {
      const me = this;
      
      me.memberRequestName='', 
      me.orgId=0,

      me.platformId = 0,
      me.clientContractId=0,
        me.clientContractName = '',

      me.clientPayGroupId=0,
        me.clientPayGroupName = '',
   
      me.memberRequestPositionName='', 
      me.deploymentDate=null,
      me.memberRequestStatusId= 0,
      me.endDate=null,
      me.employmentTypeId=0,



      me.onSearch();
    },

    onSearch() {
      const me = this;      

      let filter = 'MemberRequestName is not NULL ';
      let filterName = '';



      if (me.memberRequestName) {
        filterName = " AND MemberRequestName like '%" + me.memberRequestName + "%'"
      }

      if (me.orgId) {
        filterName = filterName + " AND OrgId = " + me.orgId + ""
      }

      if (me.platformId) {
        filterName = filterName + " AND PlatformId = " + me.platformId + ""
      }

      if (me.clientContractId) {
        filterName = filterName + " AND ClientContractId = " + me.clientContractId + ""
      }

  if (me.clientPayGroupId) {
        filterName = filterName + " AND ClientPayGroupId = " + me.clientPayGroupId + ""
      }


      if (me.memberRequestPositionName) {
        filterName = filterName + " AND MemberRequestPositionName like '%" + me.memberRequestPositionName + "%'"
      }

      
      if (me.memberRequestStatusId) {
        filterName = filterName + " AND MemberRequestStatusId = " + me.memberRequestStatusId + ""
      }

      if (me.employmentTypeId) {
        filterName = filterName + " AND EmploymentTypeId = " + me.employmentTypeId + ""
      }

      if (me.endDate) {
        filterName = filterName + " AND EndDate = '" + me.endDate.toDateFormatISO() + "'"
      }

      if (me.deploymentDate) {
        filterName = filterName + " AND DeploymentDate = '" + me.deploymentDate.toDateFormatISO() + "'"
      }
      
      me.filterParam = {}; 
      me.filterParam.filter = filter + filterName;
      
      me.getMemberRequestListFilter().then(
        (data) => {
          me.memberRequestList = [];
          me.memberRequestList = data;
           
          if (!me.memberRequestList.length > 0) {
            me.advice.fault('No record found. Clear the filter and try again.', { duration: 5 })
            return;
          }  
        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },
 
    onClientContractIdChanging (e) {
      e.message = "Client Contract ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientContractIdCallback;
    },

    clientContractIdCallback (e) {
      const me = this;
      let filter = "ClientContractId=" + e.proposedValue + "";
      return getList('dbo.ArsClientContract', 'ClientContractId, ClientContractName, PlatformId, OrgId', '', filter).then(
        request => {
          if (request && request.length) {
            me.clientContractName = request[0].clientContractName;
            me.platformId = request[0].platformId;
             me.orgId = request[0].orgId;
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

    onClientContractIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0]

      this.clientContractId = data.clientContractId;
      this.clientContractName = data.clientContractName;
      this.platformId = data.platformId;
      this.orgId = data.orgId;
      this.focusNext();

    },


 onClientPayGroupIdChanging (e) {
      e.message = "Client Pay Group ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId=" + e.proposedValue + "";
      return getList('dbo.ArsClientPayGroup', 'ClientPayGroupId, ClientPayGroupName', '', filter).then(
        request => {
          if (request && request.length) {
            me.clientPayGroupName = request[0].clientPayGroupName;
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

    onClientPayGroupIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0]

      this.clientPayGroupId = data.clientPayGroupId;
      this.clientPayGroupName = data.clientPayGroupName;
  
      this.focusNext();

    },


    loadData () {
      const
        me = this,
        wait = me.wait();
        
        me.getMemberRequestList()
        .then( list => {
          me.stopWait(wait);
          me.memberRequestList = list.requestList;
          me.memberRequestReferenceList = list.requestList;
          me.platforms = list.platforms;
          me.contracts = list.contracts;
          me.requestStatus = list.requestStatus;
          me.employmentTypes = list.employmentTypes;
          me.orgs = me.sym.userInfo.orgs;
          me.payGroups = list.payGroups;
          me.setupControls();
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
    getApiPayloadFilter () {
      const
        me = this,
        filterParam = {};

      Object.assign(filterParam, me.filterParam);

      return filterParam;
    },



    getMemberRequestListFilter () {
      const
        payload = this.getApiPayloadFilter(),
        body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-request-forms', options);
    },


    getMemberRequestList () {
      return get('api/member-request-forms');
    },
    onRefresh () {
      this.loadData();
    },
  },

  created () {
    const me = this;
    me.orgs = [];

    me.platforms = [];
    me.contracts = [];
    me.requestStatus = [];
    me.employmentTypes = [];
 
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
.box-container{
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;

}
.box-column{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr .5fr 1fr ;
  gap: .5rem;
}
.box-column-2{
  display: grid;
  grid-template-columns: .3fr 2fr .3fr 2fr ;
  gap: .5rem;
}
.box-date-column{
  display: grid;
  grid-template-columns: 1fr 1fr 3fr;
  gap: .5rem;
}
.box-name-column{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.box-table-container{
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  margin-top: 1rem;
  
}


.table-scroll-wrapper {
  overflow: auto;
}

.table-scroll {
  white-space: nowrap;
  width: 150vw;
}
td{
  text-wrap: wrap ;
}
.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
</style>