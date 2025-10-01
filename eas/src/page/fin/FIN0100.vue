// Chart of Accounts Maintenance

<template>
<section class="container p-0" :key="ts">
<sym-form id="fin0100" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">


  <!-- footer / no log -->
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

  <sym-text v-model="acct.accountId" :caption-width="35" :input-width="30" caption="Account ID" lookupId="FinAccountOrgCore" @lostfocus="onAccountIdLostFocus" @changed="onAccountIdChanged" @searchfill="onAccountIdSearchFill" @searchresult="onAccountIdSearchResult"></sym-text>
 
  <sym-text v-model="acct.accountName" :caption-width="35" :input-width="200" caption="Account Name"></sym-text>
   <sym-combo v-model="acct.orgId" :caption-width="35" :input-width="120" caption="Cooperative" display-field="orgName" :datasource="orgs"> </sym-combo>
 
  <sym-memo v-model="acct.accountDescription" :caption-width="35"  caption="Account Description"></sym-memo>
 <div class="d-flex gap-2">
  <sym-combo v-model="acct.accountTypeId" :caption-width="35" :input-width="40" caption="Type" display-field="accountTypeName" :datasource="types" @changed="onAccountTypeIdChanged"></sym-combo>
  <sym-combo v-model="acct.accountNatureId" :caption-width="35" :input-width="40" caption="Nature" display-field="accountNatureName" :datasource="natures"></sym-combo>
 </div>
  <sym-text v-model="acct.headerAccountId" :caption-width="35" :input-width="30" caption="Header" lookupId="FinAccountHeader" @changing="onHeaderAccountIdChanging" @changed="onHeaderAccountIdChanged" @searchfill="onHeaderAccountIdSearchFill" @searchresult="onHeaderAccountIdSearchResult"></sym-text>
  <sym-text v-model="acct.headerAccountName"></sym-text>

</sym-form>

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
  name: 'fin0100',

  data () {
    return {
      acct: {
        accountId: '',
        accountName: '',
        accountDescription: '',
        orgId: 0,
        accountTypeId: 0,
        accountNatureId: 0,
        headerAccountId: '',
        headerAccountName: '',
        lockId: ''
      },

      // logs: [],
      // isLogVisible: false
    };
  },

  methods: {

    clearSelection() {
  const oldValue = this.selected;
  this.selected = '';
  this.onChange('', oldValue); // Trigger change handling
},
    // getTargetPath () {
    //   const
    //     q = {},
    //     me = this;

    //   if (me.acct.accountId) {
    //     q.accountId = me.acct.accountId;
    //   }

    //   return {
    //     name: me.$options.name,
    //     query: q
    //   };
    // },

    // syncValues (p, q) {
    //   const me = this;

    //   if ('accountId' in q && me.core.isInteger(q.accountId)) {
    //     me.acct.accountId = q.accountId;
    //   }
    // },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.types = data.types;
            me.natures = data.natures;
            me.orgs = data.orgs;
          }
          return me.getAccount();
        })
        .then( acct => {
          me.stopWait(wait);
          if (acct && acct.length) {
            me.setModels(acct);
          } else {
            // me.advice.warning('You are adding a new document.', { duration: 5 });
            // 05 Feb 2025 - EMT
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Account <b>' + me.acct.accountId + '</b> not found.';
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

      me.acct = info[0];
      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldAcct = JSON.stringify(me.acct);
    },

    // API calls

    getAccount () {
      return get('api/fin-accounts/' + this.acct.accountId);
    },

    getReferences () {
      const me = this;

      if (me.types.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }
      return get('api/references/fin0100');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/fin-accounts', options);
    },

    // createRecord () {
    //   const
    //     payload = this.getApiPayload(),
    //     body = JSON.stringify(payload),
    //     currentUserId = this.sym.userInfo.userId,
    //     options = this.core.getAjaxOptions('POST', body);

    //   return ajax('api/fin-accounts/' + currentUserId, options);
    // },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/fin-accounts/' + this.acct.accountId, options);
    },

    // modifyRecord () {
    //   const
    //     payload = this.getApiPayload(),
    //     body = JSON.stringify(payload),
    //     currentUserId = this.sym.userInfo.userId,
    //     options = this.core.getAjaxOptions('PUT', body);

    //   return ajax('api/fin-accounts/' + this.acct.accountId + '/' + currentUserId, options);
    // },

    deleteRecord () {
      const
        acct = this.acct,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/fin-accounts/' + acct.accountId + this.getDeleteQuery(acct.lockId), options);
    },

    // deleteRecord () {
    //   const
    //     acct = this.acct,
    //     currentUserId = this.sym.userInfo.userId,
    //     options = this.core.getAjaxOptions('DELETE');

    //   return ajax('api/fin-accounts/' + acct.accountId + '/' + acct.lockId + '/' + currentUserId, options);
    // },

    // getChangeLog () {
    //   return get('api/fin-accounts/' + this.acct.accountId + '/log');
    // },

    getApiPayload () {
      const
        me = this,
        acct = {};

      Object.assign(acct, me.acct);
      return acct;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('acct');
      dc.keyField = 'accountId';
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

    onAccountIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onAccountIdLostFocus () {
      const me = this;

      if (!me.acct.accountId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onAccountIdSearchFill (e) {
      // auto-add percent char (wildcard) if searching for accountName

      if (e.filter.startsWith('accountName')) {
        if (e.filter.indexOf("accountName LIKE '%") < 0) {
          e.filter = e.filter.replace("accountName LIKE '", "accountName LIKE '%");
        }
      }
    },

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.acct.accountId = data.accountId;
      me.replaceUrl();
      me.loadData();
    },

    onAccountTypeIdChanged (newValue) {
      const me = this;

      let accountType = me.types.find(o => o.accountTypeId === newValue);
      if (accountType) {
        me.acct.accountNatureId = accountType.accountNatureId;
      }

    },

    onHeaderAccountIdChanging (e) {
      if (!e.proposedValue) {
        return;
      }

      const me = this;
      e.message = "Header account '<b>" + e.proposedValue + "</b>' not found.";

      e.callback = me.headerAccountIdCallback;
    },

    headerAccountIdCallback (e) {
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND left(AccountName, 1) = '#'";
      return getList('dbo.FinAccount', 'AccountId, AccountName, AccountTypeId', '', filter).then(
        header => {
          if (header && header.length) {
            if (header[0].accountTypeId !== me.acct.accountTypeId) {
              e.message = "Account Type mismatch."
              return false;
            }

            me.acct.headerAccountName = header[0].accountName;
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

    onHeaderAccountIdChanged (newValue) {
      if (!newValue) {
        this.acct.headerAccountName = '';
      }
    },

    onHeaderAccountIdSearchFill (e) {
      const me = this;

      // auto-add hash (#) if searching for accountName since header accounts are
      // prefixed with it by convention

      if (e.filter.startsWith('accountName')) {
        if (e.filter.indexOf("accountName LIKE '#") < 0) {
          e.filter = e.filter.replace("accountName LIKE '", "accountName LIKE '#");
        }
      }

      if (e.filter) {
        e.filter = e.filter + ' AND (accountTypeId = ' + me.acct.accountTypeId + ')';
      } else {
        e.filter = 'accountTypeId = ' + me.acct.accountTypeId;
      }
    },

    onHeaderAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        acct = this.acct;

      acct.headerAccountId = data.accountId;
      acct.headerAccountName = data.accountName;

      this.focusNext();
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

      getSafeDeleteFlag('FinAccount', me.acct.accountId)
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

    // onViewLog () {
    //   const
    //     me = this,
    //     wait = me.wait();

    //   this.getChangeLog().then(
    //     (logs) => {
    //       me.stopWait(wait);
    //       me.logs = logs;
    //       me.isLogVisible = true;
    //     },
    //     (fault) => {
    //       me.stopWait(wait);
    //       me.showFault(fault);
    //     }
    //   );

    // },

    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'accountName',
          'accountTypeId',
          'accountNatureId'
        );

        me.setDisplayMode(
          'headerAccountName'
        );

        me.setFocus('accountName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.acct) !== me.oldAcct) { return true; }
      return false;
    }

  },

  created () {
    const me = this;

    me.oldAcct = '';
    me.types = [];      // all account types (cache)
    me.natures = [];    // all account natures (cache)
    me.orgs= [];        // all orgs (cache)
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

/* #logs tr {
  vertical-align: top;
} */

/* #logs td {
  font-size: .875rem;
  padding: .375rem;
} */

/* .fixed-header {
  overflow-y: auto;
  max-height: 70vh;
} */

/* .fixed-header th {
  position: sticky;
  top: 0;
  background: rgb(221, 221, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
} */

#finaccountcore .modal-xl {
  min-width: 85%;
}

#finaccountheader .modal-xl {
  min-width: 85%;
}

</style>