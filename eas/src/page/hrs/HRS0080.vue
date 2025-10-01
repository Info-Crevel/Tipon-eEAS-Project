// Member Incomplete Identification 

<template>
<section class="container p-0" :key="ts">
<sym-form id="hrs0080" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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
    <div class="box-1">
      <sym-text  v-model="memberName" caption="Member Name"  :caption-width="35"  :input-width="200" list="names" @changed="onMemberNameChanged"></sym-text>
    <datalist id="names"><option v-for="item in memberReferenceList" :key="item.memberName" :value="item.memberName" @input="e => memberName = e.toUpperCase()" ></option></datalist> 
    </div>
    
   
  
    <hr class="info-light darken-1 mb-0 mt-0 fh-1" v-show="memberCount >= 0" />
    <p class="display-10 bold text-center my-1" v-show="memberCount > 0">{{ core.toIntegerFormat(memberCount, false) }} Member(s) found.</p>
    <p class="display-10 bold text-center my-1" v-show="memberCount < 1"> No record found.</p>
    <hr class="info-light darken-1 mt-0 mb-1  fh-1" v-show="memberCount >= 0" />
    <div class="box-2 success-light">
      <div></div>
    
    <sym-pager
      v-model="currentPage"
      :totalPages="totalPages"
      align="center"
      @change="onPageChange"
      @pager:rangeChanged="rangeStart = $event"
      v-if="currentPage && memberCount > 0"        
      class="success-light darken-1 curved pt-2 px-2 "
    >
    
    </sym-pager>
    <div class="box-4">
        <sym-combo v-model="rowPerPageId" align="left" :caption-width="46"  :input-width="30" caption="Item Per Page" display-field="rowPerPageName" :datasource="rowPerPage" @changed="onRowPerPageIdChanged"></sym-combo>
      </div>
    </div>

    <div class="table-scroll-wrapper">
      <div class="fixed-header">
        <table class="table-scroll light striped-even mb-0 mt-1" v-show="memberList ">
          <thead>
            <tr>
              <th class="w-7">Member ID</th>
              <th class="w-7">Employee Id</th>
              <th class="w-15">Last Name</th>
              <th class="w-15">First Name</th>
              <th class="w-15">Middle Name</th>
              <th class="w-2 text-center">SSS</th>
              <th class="w-4 text-center">Pag-Ibig</th>
              <th class="w-4 text-center">P-Health</th>
              <th class="w-2 text-center">TIN</th>
            </tr>
          </thead>
          <tbody class="white">
            <tr v-for="(dtl, index) in memberList" :key="index">
        
              <td class="text-left">
              <button type="button" class="justify-between info-light fw-30 btn-dtl-edit" @click="onClickMember(dtl.memberId)">
                <i class="fa fa-edit fa-lg" style="margin-right: 8px;"></i>
                <span>{{ dtl.memberId }}</span>
              </button>
              </td>

              <td>{{ dtl.memberEmployeeId }}</td>
              <td>{{ dtl.memberLastName }}</td>
              <td>{{ dtl.memberFirstName }}</td>
              <td>{{ dtl.memberMiddleName }}</td>
              <td class="text-center">
                <i :class="getBooleanIconClass(dtl.sssFlag)"></i>
              </td>
              <td class="text-center">
                <i :class="getBooleanIconClass(dtl.pgbFlag)"></i>
              </td>
              <td class="text-center">
                <i :class="getBooleanIconClass(dtl.phhFlag)"></i>
              </td>
              <td class="text-center">
                <i :class="getBooleanIconClass(dtl.whtFlag)"></i>
              </td>
            </tr>
          </tbody>
        </table>
        
    
      </div>

    </div>

  </div>
    
  <sym-pager
      v-model="currentPage"
      :totalPages="totalPages"
      align="center"
      @change="onPageChange"
      @pager:rangeChanged="rangeStart = $event"
        v-if="currentPage && memberCount > 0" 
      class="success-light darken-1 curved pt-2 px-2 mt-2"
    >
    </sym-pager>

      <div class="d-flex justify-center mt-3" v-if="!memberCount" >
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
} from '../../js/http';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'hrs0080',

  data () {
    return {
      memberList: [],
      memberReferenceList: [],
      memberName:'',
      sssFlag:false,
      pgbFlag:false,
      phhFlag:false,
      whtFlag:false,
      
      currentPage: 1,
      totalPages: 0,

      memberCount: 0,
      rowPerPageId: 0,
    };
  },

  computed: {
  
    detailTag () {
      return 'Member/s: '+ this.memberList.length;
    },

  },

  methods: {
    
    onRowPerPageIdChanged (){
      this.onSearch();
    },


    onMemberNameChanged (){
      this.onSearch();
    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.memberName) {
        q.memberName = me.memberName;
      }

      if (me.sssFlag) {
        q.sssFlag = me.sssFlag;
      }

      if (me.pgbFlag) {
        q.pgbFlag = me.pgbFlag;
      }

      if (me.phhFlag) {
        q.phhFlag = me.phhFlag;
      }

      if (me.whtFlag) {
        q.whtFlag = me.whtFlag;
      }

      q.page = me.currentPage;

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;

      if ("memberName" in q) {
        me.memberName = q.memberName;
      }

      if ("sssFlag" in q) {
        me.sssFlag = q.sssFlag;
      }

      if ("pgbFlag" in q) {
        me.pgbFlag = q.pgbFlag;
      }

      if ("phhFlag" in q) {
        me.phhFlag = q.phhFlag;
      }

      if ("whtFlag" in q) {
        me.whtFlag = q.whtFlag;
      }

      if ("page" in q && me.core.isInteger(q.page)) {
        me.currentPage = parseInt(q.page);
      }
    },

    onClickMember(memberId) {
      const me = this;

      let route = {
        name: "hrs0010",
        query: {
          memberId: memberId,
          activeTabIndex: 6 
        },
      };
      me.go(route);
    },

    setupControls () {
      const me = this;
      setTimeout(() => {
        me.setFocus('memberName');
      }, 100);

    },

    onResetFilter() {
      const me = this;
      me.memberName = '';
      me.sssFlag = false;
      me.pgbFlag = false;
      me.phhFlag = false;
      me.whtFlag = false;

      me.onSearch();
    },

    onSearch() {
 
      const me = this;
      me.memberCount = 0;
      this.checkCount = -1;
      me.currentPage = 1;

      me.loadData();
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

        me.itemsPerPage = me.rowPerPageId 

        me.getMemberCount()
          .then((count) => {
            if (!count) {
              return [];
            }
            
            me.memberCount = count;
            me.totalPages = Math.ceil(me.memberCount / me.itemsPerPage);
            return me.getMemberList();
          })
          .then((memberList) => {
            me.stopWait(wait);
            me.memberList = memberList.memberList;
            me.memberReferenceList = memberList.memberList;
            me.rowPerPage = memberList.rowPerPage;
            me.replaceUrl();
          })
          .catch((fault) => {
            me.stopWait(wait);
            me.showFault(fault);
          });
    },

    getMemberCount() {
      const me = this;

      let q = this.getSearchQuery();

      if (q) {
        return get("api/member-id-monitoring/count?" + q);
      } else {
        return get("api/member-id-monitoring/count");
      }
    },

    onPageChange(page) {
      this.currentPage = page;
      this.reload();
    },

    getMemberList() {
      const me = this;

      let q = "ipp=" + (me.itemsPerPage || 10);
      q = q + "&page=" + (me.query.page || 1);

      let sq = me.getSearchQuery();
      if (sq) {
        return get("api/member-id-monitoring/inquiry?" + me.getSearchQuery() + "&" + q);
      } else {
        return get("api/member-id-monitoring/inquiry?" + q);
      }
    },

    getSearchQuery() {
      const me = this;
      let q = "";
      
      q = "memberName=" + me.query.memberName;
      
      if (me.sssFlag) {
        if (q) {
          q += "&";
        }
        q += "sssFlag=" + me.sssFlag;
      }

      if (me.pgbFlag) {
        if (q) {
          q += "&";
        }
        q += "pgbFlag=" + me.pgbFlag;
      }

      if (me.phhFlag) {
        if (q) {
          q += "&";
        }
        q += "phhFlag=" + me.phhFlag;
      }

      if (me.whtFlag) {
        if (q) {
          q += "&";
        }
        q += "whtFlag=" + me.whtFlag;
      }

      return q;
    },

    refreshOldRefs () {
      const me = this;
      me.oldDetail = JSON.stringify(me.detail);
    },
 
    onRefresh () {
      this.loadData();
    },

  },

  created () {
    const me = this;
    me.itemsPerPage = 15;
    me.rowPerPageId = 10;
    me.rowPerPage = [];

  },

  mounted () {
    const me = this;
    me.currentPage = 1;
    me.syncValues(me.params, me.query);
    me.loadData();

  },

  beforeRouteUpdate(to, from, next) {
    this.syncValues(to.params, to.query);
    next();
    this.loadData();
  },

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
  max-height: 100vh;
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
  grid-template-columns: 1fr .6fr .4fr 1fr 1fr;
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
  max-width: 100%;
  width: 100vw;
}

.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}

.optionContainer {
  width: 100%;
  padding: 10px;
  background-color: seagreen;
}
.box-1{
  margin-bottom: 0;
}
.box-4{
  display: flex;
  justify-content: flex-end;
  margin-top: 5px;
}
.box-2 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
}
@media (max-width: 1920px) {
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
  display: flex;
  justify-content: space-evenly;
  flex-wrap: nowrap;
}
.box-container{
  display: flex;
  flex-direction: column;
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
  grid-template-columns: 1fr .6fr .4fr 1fr 1fr;
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
  max-width: 100%;
  width: 100vw;
}

.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}

.optionContainer {
  width: 100%;
  padding: 10px;
  background-color: seagreen;
}
.box-1{
  margin-bottom: 0;
}
.box-4{
  display: flex;
  justify-content: flex-end;
  margin-top: 5px;
}
.box-2 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
}
}

</style>