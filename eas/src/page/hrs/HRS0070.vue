// Member Engagement Monitoring

<template>
<section class="container p-0" :key="ts">
<sym-form id="hrs0070" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" >
         <i class="fa fa-arrow-left mr-2"></i><span>Back</span>
       </button>
    </div>
  </div>
  <div class="box-container">
    <div class="Header app-form-header curved-1 ">Filter/s</div>

  <div class="box-name-column">
  <sym-text  v-model="memberName" caption="Name" align="bottom" :caption-width="35" ></sym-text>
  <sym-combo v-model="employmentStatusId" align="bottom" :caption-width="46" caption="Employment Status" display-field="employmentStatusName" :datasource="employmentStatus"></sym-combo> 
  <sym-int v-model="memberRequestId" align="bottom" :caption-width="10" caption="MRF #" display-field="memberRequestName" :datasource="memberRequests" lookupId="ArsMemberRequest" @changing="onMemberRequestIdChanging" @searchresult="onMemberRequestIdSearchResult"></sym-int> 
  <sym-text v-model="memberRequestName" :disabled="true" align="bottom" :caption-width="10" caption="MRF Name" ></sym-text>       
  <sym-combo v-model="platformId" align="bottom" :caption-width="46" caption="Business Platform" display-field="platformName" :datasource="platforms"></sym-combo>    
  <sym-combo v-model="memberTypeId" align="bottom" :caption-width="46" caption="Member Type" display-field="memberTypeName" :datasource="memberType"></sym-combo> 
  </div>
  <div class="box-column">
  
  </div>

   <div class="box-date-column">
    <sym-date  v-model="hiredDate" caption="Hired Date"  :caption-width="35" ></sym-date>
    <sym-date   v-model="deploymentDate" caption="Deployment Date" :caption-width="60" ></sym-date>
    <sym-date   v-model="endDate" caption="Duration Date" :caption-width="50" ></sym-date>
      <div class=" d-flex gap " :info-light="isMono" :outline="isMono"  border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class=" applicant-btn " @click="onSearch">
        <i class="fa fa-search fa-lg"></i><span class="bold">Search</span>
      </button>
      <button type="button" :class="logButtonClass" class=" applicant-btn " @click="onResetFilter">
      <i class="fa fa-undo  fa-lg"></i><span class="bold">Reset</span>
      </button>
  </div>
  </div>
  
  </div>
  <div class="box-table-container">

  <div class="Header app-form-header curved-1 ">{{ detailTag }}</div>

  <div class="table-scroll-wrapper">
    <div class="fixed-header">
      <table class="table-scroll light striped-even mb-0 "> 
        <thead>
          <tr>
            <th class="w-20">Member Id</th>
            <th class="w-30">Employee Id</th>
            <th class="w-50">Member Name</th>
            <th class="w-30">Employment Status</th>
            <th class="w-30">Member Type</th>
            <th class="w-15">MRF #</th>
            <th class="w-60">MRF Name</th>
            <th class="w-50">Platform</th>
            <th class="w-30">Hired Date</th>
            <th class="w-30">Deployment Date</th>
          </tr>
        </thead>
        <tbody class="white">
          <tr v-for="(dtl, index) in memberList" :key="index">
            <td @click="onClickMember(dtl.memberId)">{{ dtl.memberId }}</td>
            <td>{{ dtl.memberEmployeeId }}</td>
            <td>{{ dtl.memberName }}</td>
            <td>{{ dtl.employmentStatusName }}</td>
            <td>{{ dtl.memberTypeName }}</td>
            <td>{{ dtl.memberRequestId }}</td>
            <td class="platform">{{ dtl.memberRequestName }}</td>
            <td class="platform">{{ dtl.platformName }}</td>
            <td>{{ dtl.hiredDate }}</td>
            <td>{{ dtl.deploymentDate }}</td>
          </tr>
        </tbody>
      </table>
  </div>
      </div>
    </div>

  <div class="d-flex justify-center mt-3" v-if="!memberList.length">
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
  name: 'hrs0070',

  data () {
    return {
      memberList: [],
      memberName:'',
      platformId:0,
      hiredDate:null,
      endDate:null,
      deploymentDate:null,
      memberRequestId:0,
      memberRequestName: '',
      memberTypeId: 0,
      memberTypeName: '',
      employmentStatusId:0,
    };
  },

  computed: {

    detailTag () {
      return 'Member/s: ' + this.memberList.length;
    },

    isCancelled () {
      return this.detail.statusActionId === 3;
    },

  },

  methods: {

    onClickMember(memberId) {
      const me = this;

      let route = {
        name: "hrs0010",
        query: {
          memberId: memberId,
          activeTabIndex: 1 
        },
      };
      me.go(route);
    },


    setupControls () {
      const me = this;

      setTimeout(() => {

        me.setDisplayMode(
          'memberRequestName'
        );
        me.setFocus('memberRequestId');
      }, 100);

    },



    onResetFilter() {
      const me = this;
      me.memberName = '';
      me.platformId =0;
      me.hiredDate = null;
      me.endDate = null;
      me.deploymentDate = null;
      me.memberRequestId = 0;
      me.memberRequestName = '';
      me.employmentStatusId = 0;
      me.memberTypeId= 0;
      me.memberTypeName= '';
      me.onSearch();
    },

    onSearch() {
      const me = this;      

      let filter = 'MemberName is not NULL ';
      let filterName = '';
      if (me.memberName) {
        filterName = " AND MemberName like '%" + me.memberName + "%'"
      }

      if (me.employmentStatusId) {
        filterName = filterName + " AND EmploymentStatusID = " + me.employmentStatusId + ""
      }

      if (me.memberRequestId) {
        filterName = filterName + " AND MemberRequestId = " + me.memberRequestId + ""
      }

      if (me.platformId) {
        filterName = filterName + " AND PlatformId = " + me.platformId + ""
      }

      if (me.memberTypeId) {
        filterName = filterName + " AND MemberTypeId = " + me.memberTypeId + ""
      }


      if (me.hiredDate) {
        filterName = filterName + " AND HiredDate = '" + me.hiredDate.toDateFormatISO() + "'"
      }

      if (me.endDate) {
        filterName = filterName + " AND EndDate = '" + me.endDate.toDateFormatISO() + "'"
      }

      if (me.deploymentDate) {
        filterName = filterName + " AND DeploymentDate = '" + me.deploymentDate.toDateFormatISO() + "'"
      }


      me.filterParam = {}; 
      me.filterParam.filter = filter + filterName;
      
      me.getMemberListFilter().then(
        (data) => {
          me.memberList = [];
          me.memberList = data;
          if (!me.memberList.length > 0) {
            me.advice.fault('No record found. Clear the filter and try again.', { duration: 5 })
            return;
          }  
        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },
 
    onMemberRequestIdChanging (e) {
      e.message = "Member Request ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback (e) {
      const me = this;
      let filter = "MemberRequestId='" + e.proposedValue + "'";
      return getList('dbo.ArsMemberRequest', 'MemberRequestId, MemberRequestName, PlatformId', '', filter).then(
        request => {
          if (request && request.length) {
            me.memberRequestName = request[0].memberRequestName;
            me.platformId = request[0].platformId;
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

    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0]
      this.memberRequestId = data.memberRequestId;
      this.memberRequestName = data.memberRequestName;
      this.platformId = data.platformId;
      this.focusNext();

    },

    loadData () {
      const
        me = this,
        wait = me.wait();
        
        me.getMemberList()
        .then( list => {
          me.stopWait(wait);
          me.memberList = list.memberList;
          me.platforms = list.platform;
          me.memberRequests = list.memberRequest;
          me.employmentStatus = list.employmentStatus;
          me.memberType = list.memberType;
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



    getMemberListFilter () {
      const
        payload = this.getApiPayloadFilter(),
        body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-engagements', options);
    },


    getMemberList () {
      return get('api/member-engagements');
    },
    onRefresh () {
      this.loadData();
    },
  },

  created () {
    const me = this;
    me.platforms = [];
    me.memberRequests = [];
    me.employmentStatus = [];
    me.memberType = [];


  },

  mounted () {
    const me = this;
    me.loadData();
  }

}

</script>

<style scoped>
.Header {
  width: 100%;
  border: 0;
  padding: 5px;
  text-align: center;
  font-weight: bold;
  color: white;
  
  text-transform: uppercase;
}
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
  grid-template-columns: .3fr 1fr 1fr;
  gap: .5rem;
}
.box-date-column{
  display: grid;
  grid-template-columns: .5fr .5fr .5fr 1fr;
  gap: .5rem;
}
.box-name-column{
  display: grid;
  grid-template-columns: 1fr .6fr .4fr 1fr 1fr .5fr;
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
  overflow-x: auto;
}

.table-scroll {
  white-space: nowrap;
  width: -moz-available;
  width: -webkit-fill-available;
}

.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
.platform{
  text-wrap: wrap;
}
</style>