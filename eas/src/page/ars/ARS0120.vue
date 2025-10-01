// MRF Transfer

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="ars0120" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
        </div>
      </div>

    <sym-tabs id="request-info-tabsXXX" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">

      <sym-tab title="Source" icon="user-times">
      <div class="header-containerXXX mt-2">
        <div class="applicantid-info">
          <sym-int v-model="sourceRequest.memberRequestId" :caption-width="20" caption="MRF #" lookupId="ArsMemberRequest" @lostfocus="onMemberRequestIdLostFocus" @changing="onMemberRequestIdChanging" @changed="onMemberRequestIdChanged" @searchresult="onMemberRequestIdSearchResult" ></sym-int>
          <sym-text v-model="sourceRequest.memberRequestName" caption="Member Request Name" :caption-width="50"  :input-width="150" align="left" ></sym-text>            
        </div>
          

      </div>

      <div class="search-container mb-2 d-flex align-center gap">
      <input v-model="searchName" type="text" placeholder="Search Member Name" class="input-text" list="memberRequestNames"/>
      <datalist id="memberRequestNames"><option v-for="item in sourceMember" :key="item.memberName" :value="item.memberName" @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist> 
    </div>


           <div class="box-table  gap"> 
    
            </div>
                  <div class="text-center border-light curved p-1 info mt-2">
                    <span class="serif lg-3">{{ detailTag }}</span>
                  </div>

            <div class=" fixed-header table-scroller">
                  
                      <table class="striped-even mb-0" ref="table" id="loremTable ">
                        <thead>
                          <tr>
                            <th class="w-10">Member ID</th>
                            <th class="w-30">Member Name</th>
                            <th class="w-10">Hired Date</th>
                            <th class="w-15">Employment Status</th>
                            <th class="w-10">Action</th>
                          </tr>
                        </thead>
                        <tbody class="white">
                          <tr v-for="(dtl, index) in filteredMembers" :key="index">
                            <td>{{ dtl.memberId }}</td>
                            <td>{{ dtl.memberName }}</td>
                            <td>{{ dtl.hiredDate }}</td>
                            <td>{{ dtl.employmentStatusName }}</td>
                            <td class="p-1">
                              <div class="d-flex justify-center" sm-1 mb-0>
                 <button type="button" :class="logButtonClass" class="justify-center btn-log w-60 danger"  v-if="isTransfer" @click="onTransfer(dtl)">
                    <i class="fa fa-exchange fa-lg"></i><span></span> 
                  </button>

                              </div>
                            </td>
                          </tr>
                        </tbody>      
                      </table>
               
              
                  </div>  
                </sym-tab>

      <sym-tab title="Target" icon="user-plus">

  <div class="header-containerXXX">
      <div class="header-containerXXX mt-2">
        <div class="applicantid-info">
          <sym-int v-model="sourceRequest.targetRequestId" :caption-width="20" caption="MRF #" lookupId="ArsMemberRequest" @lostfocus="onTargetMemberRequestIdLostFocus"  @changed="onTargetMemberRequestIdChanged" @searchfill="onTargetMemberRequestIdSearchFill" @searchresult="onTargetMemberRequestIdSearchResult" ></sym-int>
          <sym-text :disable="true" v-model="sourceRequest.targetRequestName" caption="Member Request Name" :caption-width="50"  :input-width="150" align="left" ></sym-text>            
        </div>
          

      </div>

  </div>

       <div class="text-center border-light curved p-1 info mt-2">
                    <span class="serif lg-3">{{ targetDetailTag }}</span>
                  </div>

                  <div class=" fixed-header table-scroller">
                  
                     
                      <table class="striped-even mb-0" ref="table" id="loremTable ">
                        <thead>
                          <tr>
                            <th class="w-10">Member ID</th>
                            <th class="w-30">Member Name</th>
                            <th class="w-10">Hired Date</th>
                            <th class="w-15">Employment Status</th>
                            <th class="w-10">Action</th>
                          </tr>
                        </thead>
                        <tbody class="white">
                          <tr v-for="(dtl, index) in targetMember" :key="index"  :class="{ 'highlight-add': dtl.highlight }">
                            <td>{{ dtl.memberId }}</td>
                            <td>{{ dtl.memberName }}</td>
                            <td>{{ dtl.hiredDate }}</td>
                            <td>{{ dtl.employmentStatusName }}</td>
                            <td class="p-1">
                              <div class="d-flex justify-center" sm-1 mb-0>
    
               <button type="button" :class="logButtonClass" class="justify-center btn-log w-60 danger"   @click="onRemove(dtl)">
                    <i class="fa fa-undo fa-lg"></i><span></span> 
                  </button>
                          </div>
                            </td>
                          </tr>
                        </tbody>      
                      </table>
               
              
                  </div>  

</sym-tab>
              </sym-tabs>  
            
    </sym-form>

</section>
</template>

<script>

import {
  get,
  ajax,
} from "../../js/http";

import {
  getList,
  getSafeDeleteFlag,
} from "../../js/dbSys";

import PageMaintenance from "../PageMaintenance.vue";
import SymImageSelect from "../../comp/SymImageSelect.vue";
import SymInteger from '../../comp/SymInteger.vue';
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymImageSelect, SymInteger, SymDecimal },
  extends: PageMaintenance,
  name: "ars0120",

  data() {
    return {
      
      sourceRequest: {
        memberRequestId: 0,
        memberRequestName: '',
        targetRequestId: 0,
        targetRequestName: '',
        lockId: '',
      },

      targetRequest: {
        memberRequestId: 0,
        memberRequestName: '',
        lockId: '',
      },




      sourceMember: [],
      targetMember: [],
      oldSourceRequest: [],
      oldSourceMember: [],

      oldTargetRequest: [],
      oldTargetMember: [],

      activeTabIndex: 0,
      lastAction: null,
      searchName: '', 
    };


  },

   computed: {   
    isTransfer () {
      return this.sourceRequest.targetRequestId !== 0;
    },

    detailTag () {
      return 'Member/s: ' + this.sourceMember.length;
    },

    targetDetailTag () {
      return 'Member/s: ' + this.targetMember.length;
    },

     filteredMembers() {
    if (!this.searchName) return this.sourceMember;
    const searchLower = this.searchName.toLowerCase();
    return this.sourceMember.filter(member =>
      member.memberName.toLowerCase().includes(searchLower)
    );
  }

  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
 
  },
  
  methods: {

  onSearchName() {
  },
  
  isMatchingName(name) {
    if (!this.searchName) return false;
    return name.toLowerCase().includes(this.searchName.toLowerCase());
  },

    onTargetMemberRequestIdSearchFill (e) {
      e.filter = "MemberRequestId <> " + this.sourceRequest.memberRequestId    
    },


onTransfer(member) {
  if (this.targetMember.some(m => m.memberId === member.memberId)) return;
  const index = this.sourceMember.findIndex(m => m.memberId === member.memberId);
  if (index === -1) return;

  const transferred = this.sourceMember.splice(index, 1)[0];
  this.targetMember.push({ ...transferred, highlight: true });

  this.lastAction = { type: 'transfer', member: transferred, index };
},

onRemove(member) {
  if (this.sourceMember.some(m => m.memberId === member.memberId)) return;

  const index = this.targetMember.findIndex(m => m.memberId === member.memberId);
  if (index === -1) return;

  const removed = this.targetMember.splice(index, 1)[0];
  this.sourceMember.push({ ...removed, highlight: true });

  this.lastAction = { type: 'remove', member: removed, index };
},

    onTargetMemberRequestIdChanged () {
      const me = this,
      wait = me.wait();


      me.getTargetRequest().then(
        (data) => {
          me.sourceMember.targetRequestId = data.targetRequest[0].memberRequestId;
          me.sourceMember.targetRequestName = data.targetRequest[0].memberRequestName;
          me.stopWait(wait);
        },
        (fault) => {
          me.targetRequest = {};
          me.stopWait(wait);
          
        }
      );
      me.stopWait(wait);
    },

  
    onTargetMemberRequestIdLostFocus() {
      const me = this;

      if (!me.sourceRequest.targetRequestId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTargetMemberRequestIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.sourceRequest.targetRequestId = data.memberRequestId;
      me.sourceRequest.targetRequestName = data.memberRequestName;
       
      me.onTargetMemberRequestIdChanged()
        me.setFocus("memberRequestId");

    },


    onActiveTabIndexChanged () {
      this.reload();
    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.sourceRequest.memberRequestId) {
        q.memberRequestId = me.sourceRequest.memberRequestId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("memberRequestId" in q && me.core.isInteger(q.memberRequestId)) {
       
        me.sourceRequest.memberRequestId = parseInt(q.memberRequestId);
      }

     },

    // API calls

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.orgs = data.org;
          }

          if (me.sourceRequest.memberRequestId < 0) {
            return Promise.resolve(null);
          }
          return me.getSourceRequest();
        })
        .then((sourceRequest) => {
          me.stopWait(wait);
          if (sourceRequest && sourceRequest.sourceRequest.length) {
            me.setModels(sourceRequest);
          } else {
            
            if (me.sourceRequest.memberRequestId > -1) {
              let message = "MRF # '<b>" + me.sourceRequest.memberRequestId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
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
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    
    },

    setModels(info) {
      const me = this,
        sourceRequest = info.sourceRequest[0]; 
        me.sourceRequest = sourceRequest;
        me.sourceMember = info.sourceMember;
        
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldSourceRequest = JSON.stringify(me.oldSourceRequest);      
      me.oldSourceMember = JSON.stringify(me.oldSourceMember);    
      me.oldTargetRequest = JSON.stringify(me.oldTargetRequest);      
      me.oldTargetMember = JSON.stringify(me.oldTargetMember);      
    },

    getSourceRequest() {
      if (this.sourceRequest.memberRequestId < 0) {
        return Promise.resolve(null);
      }

      return get("api/source-requests/" + this.sourceRequest.memberRequestId);
    },

    getTargetRequest() {
      if (this.sourceRequest.targetRequestId < 0) {
        return Promise.resolve(null);
      }

      return get("api/target-requests/" + this.sourceRequest.targetRequestId);
    },



    getReferences() {
      const me = this;
      if (me.orgs.length) {
        return Promise.resolve(true);
      }

      return get("api/references/ars0120");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/payees/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/payees/" + this.payee.payeeId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const payee = this.payee,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/payees/" + this.payee.payeeId + "/" + payee.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        payee = {};

      Object.assign(payee, me.payee);
      return payee;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "sourceRequest","sourceMember"

      );
      dc.keyField = "memberRequestId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
  
      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onMemberRequestIdChanging(e) {
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback(e) {
      e.message = "MRF # '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.ArsMemberRequest", "memberRequestId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onMemberRequestIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onMemberRequestIdLostFocus() {
      const me = this;

      if (!me.sourceRequest.memberRequestId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberRequestIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.sourceRequest.memberRequestId = data.memberRequestId;
      me.loadData();
      me.replaceUrl();

    },

    onSubmit(nextRoute) {
      const me = this;
      if (!me.isValid(me.$options.name)) {
        me.advice.fault(
          "Fill in the required fields (marked in red) before saving.",
          { duration: 5 }
        );
        return;
      }
      if (!me.hasChanges()) {
        me.advice.success("Document updated.", { duration: 5 });
        me.onReset();
        return;
      }
      let promise,
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
            if (isNew && typeof success === "number" && success > 0) {
              me.payee.payeeId = success;

            }
            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";


            }
            me.setCopyData();
            if (nextRoute && !(nextRoute instanceof MouseEvent)) {
              me.dialog.success(message, { size: "md" }).then(() => {
                me.refreshOldRefs();
                me.go(nextRoute);
                return;
              });
            } else {
    
              me.advice.success(message, { duration: 5 });
          }
          }

          me.onReset();
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onDelete() {
      const me = this;

      getSafeDeleteFlag("ApsPayee", me.payee.payeeId)
        .then((safe) => {
          if (safe) {
            return me.dialog.confirm(
              "Document will be deleted.<br><br>Continue?",
              { scheme: "warning", icon: "warning", size: "md" }
            );
          } else {
            me.advice.fault(
              "Delete attempt failed.<hr>Cannot delete document at this time.",
              { duration: 5 }
            );
            return "";
          }
        })
        .then((reply) => {
          if (reply === "yes") {
            return me.deleteRecord();
          }
          return false;
        })
        .then((success) => {
          if (success) {
            me.advice.success("Document deleted.", { duration: 4 });
            me.onReset();
          }
        })
        .catch((fault) => {
          me.showFault(fault);
        });
    },

    setupControls() {
      const me = this;

        me.setDisplayMode(
          "targetRequestName",              
        );

      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "memberRequestId",
          "targetRequestId",
          
        );

        me.setDisplayMode(
          "memberRequestName",
          "targetRequestName",              
        );

        me.setFocus("targetRequestId");

      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.sourceRequest) !== me.oldSourceRequest) {return true;}
      if (JSON.stringify(me.sourceMember) !== me.oldSourceMember) {return true;}
      if (JSON.stringify(me.targetRequest) !== me.oldTargetRequest) {return true;}
      if (JSON.stringify(me.targetMember) !== me.oldTargetMember) {return true;}

      return false;

    },
  },

  created () {
    const me = this;
    
    me.oldSourceRequest = '';
    me.oldSourceMember = '';

    me.oldTargetRequest = '';
    me.oldTargetMember = '';

    me.orgs = []; 

  }
}

</script>

<style scoped>
 
 .highlight-row {
  background-color: #fffacc; 
  
}

 .highlight-add {
  background-color: #d4edda !important; 
 
}

.highlight-remove {
  background-color: #f8d7da !important;
  font-weight: bold;
}

 
 input::-webkit-outer-spin-button,
  input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}


.fade-move-enter-active,
.fade-move-leave-active {
  transition: all 0.4s ease;
}
.fade-move-enter-from,
.fade-move-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

  .action-buttons{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  }

  .applicantid-info {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    width: 100%;
  }

  .box-field {
  display: grid;
  grid-template-columns: 1fr ;
  gap: .5rem;
  }
  .box-1 {
    display: grid;
    grid-template-columns:  1fr  ;
  }
  .box-2 {
    display: flex;
    flex-wrap: wrap;
  }
  .box-3 {
    display: flex;
    flex-wrap: wrap
  }
  .box-4 {
    display: flex;
    flex-wrap: wrap
  }
  .box-5 {
    display: flex;
    flex-direction: column;
    flex-wrap: wrap
  }
  .box-6 {
    display: flex;
    
    flex-wrap: wrap
  }
  .box-7 {
    display: flex;
    flex-wrap: wrap
  }
  .box-8 {
    display: flex;
    flex-wrap: wrap
  }
  .box-9 {
    display: grid;
    grid-template-rows: 1fr ;
    gap: 0;
  }
  .schedule-editor-boxes{
    display: grid;
    grid-template-columns: .5fr .5fr .6fr;
    }
  .box-date{
    display: grid;
    grid-template-columns: 1fr 1fr .5fr .5fr;
    
  }
  .box-sched{
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr;
    
  }
  .box-member{
    display: grid;
    grid-template-columns: .3fr 1fr;
    
  }
  .box-table{
    display: grid;
    grid-template-columns: 1fr  1fr;
  }

  .box-table-2{
    display: grid;
    grid-template-columns: 1fr  .5fr;
  }
  .table-scroller{
    overflow: auto;
    height: 55vh;
  }
  .scroller{
    width: 120vw;
    
  }

  .sched-code{
    width: 1.5rem;
  }
  .time-in{
    width: 3rem;
  }
  .time-out{
    width: 3rem;
  }
  .working-hours{
    width: 4rem;
  }
  .upaid-breaks{
    width: 4rem;
  }
  .actions{
    width: 2rem;
  }


@media(max-width: 1980px){ 
  input::-webkit-outer-spin-button,
  input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
  .action-buttons{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  }

  .applicantid-info {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    width: 100%;
  }

  .box-field {
  display: grid;
  grid-template-columns: 1fr ;
  gap: .5rem;
  }
  .box-1 {
    display: grid;
    grid-template-columns:  1fr  ;
  }
  .box-2 {
    display: flex;
    flex-wrap: wrap;
  }
  .box-3 {
    display: flex;
    flex-wrap: wrap
  }
  .box-4 {
    display: flex;
    flex-wrap: wrap
  }
  .box-5 {
    display: flex;
    flex-direction: column;
    flex-wrap: wrap
  }
  .box-6 {
    display: flex;
    
    flex-wrap: wrap
  }
  .box-7 {
    display: flex;
    flex-wrap: wrap
  }
  .box-8 {
    display: flex;
    flex-wrap: wrap
  }
  .box-9 {
    display: grid;
    grid-template-rows: 1fr ;
    gap: 0;
  }
  .schedule-editor-boxes{
    display: grid;
    grid-template-columns: .5fr .5fr .6fr;
    }
  .box-date{
    display: grid;
    grid-template-columns: 1fr 1fr .5fr .5fr;
    
  }
  .box-sched{
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr;
    
  }
  .box-member{
    display: grid;
    grid-template-columns: .3fr 1fr;
    
  }
  .box-table{
    display: grid;
    grid-template-columns: 1fr  1fr;
  }

  .box-table-2{
    display: grid;
    grid-template-columns: 1fr  .5fr;
  }
  .table-scroller{
    overflow: auto;
    height: 55vh;
  }
  .scroller{
    width: 120vw;
    
  }

  .sched-code{
    width: 1.5rem;
  }
  .time-in{
    width: 3rem;
  }
  .time-out{
    width: 3rem;
  }
  .working-hours{
    width: 4rem;
  }
  .upaid-breaks{
    width: 4rem;
  }
  .actions{
    width: 2rem;
  }

}
@media(max-width: 1600px){ 
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.schedule-editor-boxes{
  display: grid;
  grid-template-columns: .5fr .5fr .6fr;
  }
.box-date{
  display: grid;
  grid-template-columns: 1fr 1fr .5fr .5fr;
  
}
.box-sched{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  
}
.box-member{
  display: grid;
  grid-template-columns: .3fr 1fr;
  
}
.box-table{
 display: grid;
 grid-template-columns: 1fr 1.2fr;
  
}
.box-table-2{
  display: grid;
  grid-template-columns: 1fr  1fr;
}

.table-scroller{
  overflow: auto;
  height: 46vh;
}
.scroller{
  width: 100vw;
  
}
.sheet{
display: flex;
flex-wrap: wrap;
}

.sched-code{
  width: .1rem;
}
.time-in{
  width: 1rem;
}
.time-out{
  width: 1rem;
}
.working-hours{
  width: 2rem;
}
.upaid-breaks{
  width: 2rem;
}
.actions{
  width: .3rem;
}
.box-1 {
  display: grid;
  grid-template-columns: 1fr  ;
}
.box-6{
  display: grid;
  grid-template-rows: .5fr 1fr ;
}
.box-9{
  display: grid;
  grid-template-rows: .5fr 1fr ;
}
.matrix{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr ; 
}
}
@media(max-width: 1000px) {
  .box-table{
  display: flex;
  flex-wrap: wrap; 
  
  }
  .box-table-2{
  display: grid;
  grid-template-columns: 1fr  1fr;
  }
  .schedule-editor-boxes{
    display: grid;
    grid-template-columns: .5fr .5fr .6fr;
  }
  .box-1 {
    display: grid;
    grid-template-columns: 1fr  ;
  }
  .sched-code{
    width: 1rem;
  }
  .time-in{
    width: 1rem;
  }
  .time-out{
    width: 1rem;
  }
  .working-hours{
    width: 1rem;
  }
  .upaid-breaks{
    width: 1rem;
  }
  .actions{
    width: 1rem;
  }
  .box-table-2{
    display: flex;
    flex-wrap: wrap;
  }
  .table-scroller{
    overflow: auto;
    height: 50vh;
  }
  .scroller{
    width: 100vw;
    
  }
}
@media(max-width: 800px) {
  .schedule-editor-boxes{
    display: grid;
    grid-template-columns: .5fr .5fr .6fr;
  }
  .box-1 {
    display: grid;
    grid-template-columns: 1fr  ;
  }
  .sched-code{
    width: 1rem;
  }
  .time-in{
    width: 1rem;
  }
  .time-out{
    width: 1rem;
  }
  .working-hours{
    width: 2rem;
  }
  .upaid-breaks{
    width: 2rem;
  }
  .actions{
    width: 2rem;
  }
  .box-table-2{
    display: flex;
    flex-wrap: wrap;
  }
  .table-scroller{
    overflow: auto;
    height: 50vh;
  }
  .scroller{
    width: 500vw;

  }
}
@media(max-width: 600px) {
  .schedule-editor-boxes{
    display: grid;
  grid-template-columns: .5fr .5fr .6fr;
  }
  .action-buttons{
  display: grid;
  grid-template-columns: .5fr 4fr .5fr;
  }
  .box-1 {
    display: grid;
    grid-template-columns:  1fr  ;
  }
  .box-2 {
    display: flex;
    flex-wrap: wrap;
  }
  .box-3 {
    display: flex;
    flex-wrap: wrap
  }
  .box-4 {
    display: flex;
    flex-wrap: wrap
  }
  .box-5 {
    display: flex;
    flex-direction: column;
    flex-wrap: wrap
  }
  .box-6 {
    display: flex;
    flex-wrap: wrap;
    gap: 0;
  }
  .box-7 {
    display: flex;
    flex-wrap: wrap;
  }
  .box-8 {
    display: flex;
    flex-wrap: wrap
  }
  .box-9 {
    display: flex;
    flex-direction: column;
    flex-wrap: wrap;
    gap: 0;
  }
  .sched-code{
    width: 1.5rem;
  }
  .time-in{
    width: 1.5rem;
  }
  .time-out{
    width: 1.5rem;
  }

  .working-hours{
    width: 2rem;
  }
  .upaid-breaks{
    width: 2rem;
  }
  .actions{
    width: 2rem;
  }
  .box-table-2{
    display: flex;
    flex-wrap: wrap;
  }
  .table-scroller{
    overflow: auto;
    height: 50vh;
  }
  .scroller{
    overflow-x:auto;
  } 

  .late-matrix {
    width: 100%;
  }

  .late-matrix-button {
    width: 20%;
  }


}
</style>