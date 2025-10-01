// Payee Definition

<template>
  <section class="container p-0  w-80" :key="ts">
    <sym-form id="aps0040" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>


          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit"  > <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      </div>
      </div>


      <div class="header-containerXXX">
        <div class="payeeid-info fw-110">
          <sym-int v-model="payee.payeeId" :caption-width="20" caption="Payee ID" lookupId="ApsPayee" @lostfocus="onPayeeIdLostFocus" @changing="onPayeeIdChanging" @changed="onPayeeIdChanged" @searchresult="onPayeeIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
        </div>     
      </div>
         
            <div class=" main-box  app-box-style gap w-100">

                <div class="text-center border-light curved p-1 app-form-header mb-2">
                  <span class="serif lg-3"> PROFILE</span>
                </div>     

              <div class="box-1 gap">
                <sym-text v-model="payee.payeeName" caption="Payee Name" :caption-width="30" align="bottom" ></sym-text>            
                <sym-text v-model="payee.checkName" caption="Check Name" :caption-width="30" align="bottom" ></sym-text>            
                <sym-combo v-model="payee.payeeTypeId" align="bottom" :caption-width="20"  caption="Type" display-field="payeeTypeName" :datasource="types" @changed="onPayeeTypeIdChanged()"></sym-combo>
              </div> 
              <div class="box-2 gap">
                <sym-text v-model="payee.address1" caption="Billing Address":caption-width="150" align="bottom" ></sym-text>               
                <sym-text v-model="payee.address2" caption="Main Office address" :caption-width="150" align="bottom" ></sym-text>
                <sym-text v-model="payee.postalCode" caption="Postal Code" :caption-width="50" align="bottom" ></sym-text>               
              </div> 

                <div class="text-center border-light curved p-1 app-form-header mb-2">
                  <span class="serif lg-3">ACCOUNT</span>
                </div>   
              <div class="box-3 gap">
                  <div class="MobileContainer">
                    <span class="MobileHeader">Mobile Number</span>
                    <input class="MobilePhone"  v-model="payee.mobileNumber" @input="validateMobileNumber" placeholder="(09**)*******" type="mobile" :disabled="payee.payeeId === 0" />
                  </div>
                  <div class="MobileContainer">
                    <span class="MobileHeader">Phone Number</span>
                    <input class="MobilePhone"  v-model="payee.phoneNumber" @input="validatePhoneNumber" type="mobile" :disabled="payee.payeeId ===0" />
                  </div>
                  <sym-text v-model="payee.email" :caption-width="46"  align="bottom" caption="Email" @changing="onEmailChanging"></sym-text>

                  <!-- <sym-text v-model="payee.email" caption="Email" :caption-width="50" align="bottom" ></sym-text> -->
                <sym-text v-model="payee.contactPerson" caption="Contact Person" :caption-width="100" align="bottom" ></sym-text>
                <sym-text v-model="payee.taxIdNumber" caption="TIN" :caption-width="50" align="bottom" ></sym-text>                         
              </div>
              <div class="box-4 gap">
                 <sym-combo v-model="payee.payableTermId" align="bottom" :caption-width="72"  caption="Term" display-field="payableTermName" :datasource="terms"></sym-combo>
                 <sym-combo v-model="payee.payableTaxCode" align="bottom" :caption-width="72"  caption="Tax Code" display-field="payableTaxName" :datasource="taxes" @changed="onPayableTaxCodeChanged()"></sym-combo>
                 <sym-decimal v-model="payee.wTaxRate" caption="Perc (%)" :caption-width="35" align="bottom" ></sym-decimal>                     
                 <sym-combo v-model="payee.aTaxCode" align="bottom" :caption-width="72" caption="ATC" display-field="aTaxName" :datasource="ataxes" @changed="onATaxCodeChanged()"></sym-combo>
                 <sym-decimal v-model="payee.ewtRate" caption="Perc (%)" :caption-width="35" align="bottom" ></sym-decimal>                     
                 <sym-text v-model="payee.accountId" :caption-width="30" align="bottom" caption="Account ID" lookupId="FinAccountCore" @changing="onAccountIdChanging" @searchfill="onAccountIdSearchFill" @searchresult="onAccountIdSearchResult"></sym-text>
                 <sym-text v-model="payee.accountName" caption="Account Name" :caption-width="35" align="bottom" ></sym-text>          
              </div>
              <div class="box-5 gap">
                <sym-memo v-model="payee.remarks" caption="Remarks" :input-width="200" align="bottom" ></sym-memo>     
              </div>              
                 
            </div> 
     
    


        
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
  name: "aps0040",

  data() {
    return {
      
      payee: {
        payeeId: 0,
        orgId: 0,
        payeeName: '',
        checkName: '',
        
        payeeTypeId: 0,
        address1: '',
        address2: '',
        postalCode: '',
        phoneNumber: '',
        mobileNumber: '',
        faxNumber:'',
        email:'',
        contactPerson: '',
        remarks: '',
        vatFlag: false,
        taxIdNumber:'',
        payableTermId: 0,
        payableTaxCode: 0,
        aTaxCode: '',
        wTaxRate: 0,
        ewtRate: 0,
        accountId: '',
        accountName: '',
        lockId: '',
      },

      oldPayee: [],
    };


  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
 
  },
  
  methods: {

        onEmailChanging(e) {
      if (!e.proposedValue) {
        return;
      }

      if (!this.core.isValidEmail(e.proposedValue)) {
        e.message = "(" + e.proposedValue + ") is not a valid email address.";
        e.preventDefault();
        return;
      }

      // e.callback = this.emailCallback;
    },

    validatePhoneNumber() {
      if (!this.payee.phoneNumber) {
        this.result = null;
        return;
      }

      if (this.payee.phoneNumber.length > 11) {
        this.payee.phoneNumber = this.payee.phoneNumber.slice(0, 11);
      }

      this.payee.phoneNumber = this.payee.phoneNumber.replace(/\D/g, "");
    },

    validateMobileNumber() {
      if (!this.payee.mobileNumber) {
        this.result = null;
        return;
      }

      if (this.payee.mobileNumber.length > 11) {
        this.payee.mobileNumber = this.payee.mobileNumber.slice(0, 11);
      }

      this.payee.mobileNumber = this.payee.mobileNumber.replace(/\D/g, "");
    },

    onATaxCodeChanged() {
    const me = this;

    me.ataxes.forEach((atax) => {
      if (atax.aTaxCode == me.payee.aTaxCode) {
        me.payee.ewtRate = atax.percentage;
      }
      });

 
  },


    onPayableTaxCodeChanged() {
    const me = this;

    me.taxes.forEach((tax) => {
      if (tax.payableTaxCode == me.payee.payableTaxCode) {
        me.payee.wTaxRate = tax.percentage;
      }
      });

 
  },


  onPayeeTypeIdChanged() {
    const me = this;

    me.types.forEach((type) => {
            if (type.payeeTypeId == me.payee.payeeTypeId) {
              me.payee.payableTermId = type.payableTermId;
              me.payee.accountId = type.accountId;
            me.payee.accountName = type.accountName;            }
          });

 
},

    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) { 
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      if (e.proposedValue) {
      return getList('dbo.QFinAccountCore', 'AccountId, AccountName', '', filter).then(
        acct => {
          if (acct && acct.length) {
            me.payee.accountName = acct[0].accountName;
            return true;
          }
          return false;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      } else
      {
        me.payee.accountName = '';
        return true
      }
    },

    onAccountIdSearchFill (e) {
      e.filter = "HeaderFlag = 0"   
    },

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        payee = this.payee;

      payee.accountId = data.accountId;
      payee.accountName = data.accountName;

      this.focusNext();

    },



    getTargetPath() {
      const me = this,
        q = {};

      if (me.payee.payeeId) {
        q.payeeId = me.payee.payeeId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("payeeId" in q && me.core.isInteger(q.payeeId)) {
       
        me.payee.payeeId = parseInt(q.payeeId);
      }

     },

    // API calls

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.types = data.payeeType;
            me.terms = data.term;
            me.taxes = data.tax;
            me.ataxes = data.atax;
          }

          if (me.payee.payeeId < 0) {
            return Promise.resolve(null);
          }
          return me.getPayee();
        })
        .then((payee) => {
          me.stopWait(wait);
          if (payee && payee.payee.length) {
            me.setModels(payee);
          } else {
            
            if (me.payee.payeeId > -1) {
              let message = "Payee ID '<b>" + me.payee.payeeId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
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
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    
    },

    setModels(info) {
      const me = this
        payee = info.payee[0]; 
      me.payee = info.payee[0];
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldPayee = JSON.stringify(me.payee);      
    },

    getPayee() {
      if (this.payee.payeeId < 0) {
        return Promise.resolve(null);
      }

      return get("api/payees/" + this.payee.payeeId);
    },

    getReferences() {
      const me = this;
      if (me.types.length) {

        return Promise.resolve(true);
      }

      return get("api/references/aps0040");
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
        "payee"

      );
      dc.keyField = "payeeId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
  
      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onPayeeIdChanging(e) {
      e.callback = this.payeeIdCallback;
    },

    payeeIdCallback(e) {
      e.message = "Payee ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.ApsPayee", "payeeId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onPayeeIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onPayeeIdLostFocus() {
      const me = this;

      if (!me.payee.payeeId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onPayeeIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.payee.payeeId = data.payeeId;
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
            // set auto-generated ID returned by API so F9 works
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
      console.log(me.payee)
      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "payeeId",
          "payeeName",
          "payeeTypeId",
          'payableTermId'
        );

        me.setDisplayMode(
          "accountName",          
          "wTaxRate",         
          "ewtRate",           
        );

        me.setFocus("payeeName");

      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.payee) !== me.oldPayee) {return true;}

      return false;

    },
  },

  created () {
    const me = this;
    
    me.oldPayee = '';

    me.types = []; 
    me.terms = []; 
    me.taxes = []; 

    me.ataxes = []; 
  }
}

</script>

<style scoped>
  input::-webkit-outer-spin-button,
  input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
  .action-buttons{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  }

  .payeeid-info {
    display: flex;
    flex-direction: row;
    gap: 1rem;
    width: 100%;
  }
  .box-1 {
    display: grid;
    grid-template-columns: 1.5fr 1.5fr 1fr;
  }
  .box-2 {
    display: grid;
    grid-template-columns: 1fr 1fr .3fr;

  }
  .box-3 {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1.5fr 1fr;
  }
  .box-4 {
    display: grid;
    grid-template-columns: .7fr 1fr .5fr 1fr 0.5fr .5fr 2fr;
  }
  .box-5 {
    display: flex;
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

  .payeeid-info {
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
    grid-template-columns: 1.5fr 1.5fr 1fr;
  }
  .box-2 {
    display: grid;
    grid-template-columns: 1fr 1fr .3fr;
  }
  .box-3 {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1.5fr 1fr;
  }
  .box-4 {
     display: grid;
    grid-template-columns: .7fr 1fr .5fr 1fr 0.5fr .5fr 2fr;
  }
  .box-5 {
    display: flex;
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

.payeeid-info {
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
  grid-template-columns: 1.5fr 1.5fr 1fr;
}
.box-2{
  display: grid;
  grid-template-columns: 1fr 1fr .3fr;
}
.box-3{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1.5fr 1fr;
}
.box-4{
  display: grid;
  grid-template-columns: .3fr .5fr .2fr .5fr .2fr .2fr .5fr;
}
.box-5{
  display: flex;
}
.matrix{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr ; 
}
}

.MobileContainer {
  display: flex;
  flex-direction: column;
  margin-bottom: 0.5rem;
}
.MobileHeader {
  border: 1px solid rgb(206, 203, 203);
  background-color: rgb(245, 243, 243);
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  width: 100%;
  padding: 0.5rem;
}
.MobilePhone {
  border-top-right-radius: 0;
  border-bottom-right-radius: 4px;
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