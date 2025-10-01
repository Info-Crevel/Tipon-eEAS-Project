// Chart of Accounts Structure

<template>
<section class="container p-0" :key="ts">
<sym-form id="gls1000" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <div>
    <sym-combo v-model="accountTypeId" :caption-width="33" :input-width="40" caption="Type" display-field="accountTypeName" :datasource="types" @changed="onAccountTypeIdChanged"></sym-combo>
  </div>

  <div class="fixed-header">
    <table class="accounts striped-even">
      <thead>
        <tr>
          <th class="w-8">Account ID</th>
          <th class="w-34">Account Name</th>
          <th class="w-8">Type</th>
          <th class="w-8">Header ID</th>
          <th class="w-30">Header Name</th>
          <th class="w-12 text-right">Balance</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(acct, index) in accounts" :key="index" class="align-top">
          <td class="bold lg-1">{{ acct.accountId }}</td>
          <td :class="getAccountNameClass(acct)">{{ acct.accountName }}</td>
          <td>{{ acct.accountTypeName }}</td>
          <td>{{ acct.headerAccountId }}</td>
          <td>{{ acct.headerAccountName }}</td>
          <td class="text-right">{{ core.toFinancialFormat(acct.balance) }}</td>
        </tr>
      </tbody>
    </table>

  </div>

</sym-form>
</section>
</template>

<script>

import {
  get
} from '../../js/http';

import
  PageBase
from '../PageBase.vue';

export default {
  extends: PageBase,
  name: 'gls1000',

  data () {
    return {
      accountTypeId: 0,
      accounts: []
    }
  },

  methods: {
    getTargetPath () {
      const
        q = {},
        me = this;

      if (me.accountTypeId) {
        q.accountTypeId = me.accountTypeId;
      }

      return {
        name: me.$options.name,
        query: q
      };
    },

    syncValues (p, q) {
      const me = this;

      if ('accountTypeId' in q && me.core.isInteger(q.accountTypeId)) {
        me.accountTypeId = q.accountTypeId;
      }
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.types = data.types;
            me.types.unshift(
              { accountTypeId: 0, accountTypeName: '--- All Types ---' }
            );
          }
          return me.getAccounts();
        })
        .then( data => {
          me.stopWait(wait);
          me.accounts = data;
          me.replaceUrl();
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })
    },

    // API calls

    getAccounts () {
      return get('api/fin-accounts-structure?accountTypeId=' + this.accountTypeId);
    },

    getReferences () {
      const me = this;

      if (me.types.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }
      return get('api/references/gls1000');
    },

    // event handlers

    onAccountTypeIdChanged () {
      this.loadData();
    },

    // helpers

    getAccountNameClass (acct) {
      let cls = '';

      if (acct.levelId) {
        cls = 'tab-left-' + acct.levelId;
      }
      if (acct.headerFlag) {
        if (cls) {
          cls = cls + ' bold';
        } else {
          cls = 'bold';
        }
      }
      if (cls) {
        return cls;
      }
      return false;
    }

  },

  created () {
    this.types = [];      // all account types (cache)
  },

  mounted () {
    const me = this;

    me.syncValues(me.params, me.query);
    me.loadData();
  }

}

</script>

<style scoped>

.accounts td {
  padding-top: .375rem;
  padding-bottom: .375rem;
}

.tab-left-1 {
  padding-left: 1rem;
}

.tab-left-2 {
  padding-left: 1.5rem;
}

.tab-left-3 {
  padding-left: 2rem;
}

.tab-left-4 {
  padding-left: 2.5rem;
}

.tab-left-5 {
  padding-left: 3rem;
}

#gls1000 >>> #body {      /* deep selector */
  padding: .75rem !important;
}

.fixed-header {
  overflow-y: auto;
  max-height: 70vh;
}

.fixed-header th {
  position: sticky;
  top: 0;
  background: rgb(221, 221, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
}

</style>