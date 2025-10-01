// Template Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="gls0100" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <div slot="toolbar" class="action-buttons app-form-toolbar p-1 px-3 border-bottom-main">
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

  <div class="d-flex justify-between align-items-end fw-102">
    <sym-int v-model="template.templateId" :caption-width="42" :input-width="20" caption="Template ID" lookupId="FinTemplateCore" @lostfocus="onTemplateIdLostFocus" @changing="onTemplateIdChanging" @changed="onTemplateIdChanged" @searchresult="onTemplateIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>

  <sym-text v-model="template.templateName" :caption-width="42" :input-width="76" caption="Template Name"></sym-text>
  <sym-combo v-model="template.trxTypeId" :caption-width="42" :input-width="76" caption="Transaction Type" display-field="trxTypeName" :datasource="trxTypes"></sym-combo>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Details</span>
  </div>

  <table class="light striped-even mb-0">
    <thead>
      <tr>
        <th class="w-10">Account ID</th>
        <th class="w-51">Account Name</th>
        <th class="w-12 text-right">Debit</th>
        <th class="w-12 text-right">Credit</th>
        <th class="w-15">Action</th>
      </tr>
    </thead>
    <tbody class="white">
      <tr v-for="(dtl, index) in templateDetails" :key="index">
        <td>{{ dtl.accountId }}</td>
        <td>{{ dtl.accountName }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.debitAmount, 2, true) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.creditAmount, 2, true) }}</td>
        <td class="p-1">
          <div class="buttons" sm-1 mb-0 >
            <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)"><i class="fa fa-edit fa-lg"></i><span>Edit</span></button>
            <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)"><i class="fa fa-times fa-lg"></i></button>
          </div>
        </td>
      </tr>
      <tr :class="totalsClass">
        <td colspan="2" class="text-right bold">Total</td>
        <td class="text-right">{{ core.toDecimalFormat(template.debitTotal) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(template.creditTotal) }}</td>
        <td></td>
      </tr>
    </tbody>
  </table>

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
  <form id="gls0100A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text v-model="detail.accountId" caption="Account ID" align="bottom" lookupId="FinAccountDetail" @changing="onAccountIdChanging" @searchresult="onAccountIdSearchResult"></sym-text>
      <sym-text v-model="detail.accountName" caption="Account Name" align="bottom"></sym-text>
      <sym-dec v-model="detail.debitAmount" caption="Debit" align="bottom" captionAlign="right" @changed="onDebitAmountChanged"></sym-dec>
      <sym-dec v-model="detail.creditAmount" caption="Credit" align="bottom" captionAlign="right" @changed="onCreditAmountChanged"></sym-dec>
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
  name: 'gls0100',

  data () {
    return {
      template: {
        templateId: 0,
        templateName: '',
        trxTypeId: 0,
        lockId: '',
        debitTotal: 0,
        creditTotal: 0
      },

      templateDetails: [],

      isDetailEditorVisible: false,

      detail: {
        templateDetailId: 0,
        accountId: '',
        accountName: '',
        debitAmount: 0,
        creditAmount: 0
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    totalsClass () {
      if (this.template.debitTotal !== this.template.creditTotal) {
        return 'danger-light';
      }
      return 'success-light';
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Account Detail';
      }
      return 'Edit Account Detail';
    }
  },

  methods: {
    // getTargetPath () {
    //   const
    //     q = {},
    //     me = this;

    //   if (me.template.templateId) {
    //     q.templateId = me.template.templateId;
    //   }

    //   return {
    //     name: me.$options.name,
    //     query: q
    //   };
    // },

    // syncValues (p, q) {
    //   const me = this;

    //   if ('templateId' in q && me.core.isInteger(q.templateId)) {
    //     me.template.templateId = parseInt(q.templateId);
    //   }
    // },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.trxTypes = data.trxTypes;
          }
          if (me.template.templateId < 0) {
            return Promise.resolve(null);
          }
          return me.getTemplate();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.template.length) {
            me.setModels(info);
          } else {
            if (me.template.templateId > -1) {
              let message = "Template ID '<b>" + me.template.templateId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
            // me.advice.warning('You are adding a new document.', { duration: 5 });
            // 05 Feb 2025 - EMT
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'You are not allowed to create new documents.';
              me.advice.fault(mssg, { duration: 5 });
              me.onReset();
              return;
            }
            me.template.trxTypeId = 1;          // JV
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

      me.template = info.template[0];
      me.templateDetails = info.templateDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldTemplate = JSON.stringify(me.template);
      me.oldTemplateDetails = JSON.stringify(me.templateDetails);
    },

    // API calls

    getTemplate () {
      return get('api/fin-templates/' + this.template.templateId);
    },

    getReferences () {
      const me = this;

      if (me.trxTypes.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }
      return get('api/references/gls0100');
    },

    getChangeLog () {
      return get('api/fin-templates/' + this.template.templateId + '/log');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/fin-templates/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/fin-templates/' + this.template.templateId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        template = this.template,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/fin-templates/' + template.templateId + this.getDeleteQuery(template.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        template = {};

      Object.assign(template, me.template);
      template.details = me.templateDetails;

      return template;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('template', 'templateDetails', 'detail');
      dc.keyField = 'templateId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 100);
    },

    onTemplateIdChanging (e) {
      e.callback = this.templateIdCallback;
    },

    templateIdCallback (e) {
      e.message = "Template ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.FinTemplate', 'templateId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onTemplateIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onTemplateIdLostFocus () {
      const me = this;

      if (!me.template.templateId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTemplateIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.template.templateId = data.templateId;
      me.replaceUrl();
      me.loadData();
    },

    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) {
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      return getList('dbo.FinAccount', 'AccountId, AccountName', '', filter).then(
        acct => {
          if (acct && acct.length) {
            me.detail.accountName = acct[0].accountName;
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

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.accountId = data.accountId;
      detail.accountName = data.accountName;

      this.focusNext();

    },

    onDebitAmountChanged (newValue) {
      if (newValue > 0) {
        this.detail.creditAmount = 0;
      }
    },

    onCreditAmountChanged (newValue) {
      if (newValue > 0) {
        this.detail.debitAmount = 0;
      }
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.template.debitTotal && !me.template.creditTotal) {
        me.advice.fault('Save attempt failed. Transaction Details cannot be empty.', { duration: 3 } )
        return;
      }

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
              me.template.templateId = success;
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

      getSafeDeleteFlag('FinTemplate', me.template.templateId)
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

      me.setActiveModel('detail');

      me.setRequiredMode(
        'accountId'
      );

      me.setDisplayMode(
        'accountName'
      );

      setTimeout(() => {
        this.setFocus('accountId');
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

      d.templateDetailId = dtl.templateDetailId;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;
      d.debitAmount = dtl.debitAmount;
      d.creditAmount = dtl.creditAmount;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      // 05 Feb 2025 - EMT (reinstated)
      me.detail.templateDetailId = -1;

      // 05 Feb 2025 - EMT (not needed)
      // me.detail.templateDetailId = me.seqTemplateDetailId;
      // me.seqTemplateDetailId = me.seqTemplateDetailId - 1;

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Account <b>' + me.templateDetails[index].accountName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.templateDetails.splice(index, 1);
            me.refreshTotals();
          }
          return;
        }
      );
    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      if (!d.accountId) {
        me.isDetailEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAdding) {
        Object.assign(dtl, d);

        me.templateDetails.push(dtl);
        me.refreshTotals();

        me.clearDetailPad();
        me.detail.templateDetailId = -1;
        // 05 Feb 2025 - EMT (bug fix)
        // me.seqTemplateDetailId = me.seqTemplateDetailId - 1;
        // me.detail.templateDetailId = me.seqTemplateDetailId;

        me.advice.info("Account '" + dtl.accountName + "' added to list.", { duration: 5 });
        me.setFocus('accountId');
      } else {
        // dtl = me.templateDetails.find(o => o.templateDetailId === d.templateDetailId);
        // 05 Feb 2025 - EMT (bug fix)
        dtl = me.templateDetails[me.detailIndex];

        dtl.templateDetailId = d.templateDetailId;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;
        dtl.debitAmount = d.debitAmount;
        dtl.creditAmount = d.creditAmount;

        me.isDetailEditorVisible = false;
        me.refreshTotals();
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
          'templateName',
          'trxTypeId'
        );

        me.setFocus('templateName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.template) !== me.oldTemplate) { return true; }
      if (JSON.stringify(me.templateDetails) !== me.oldTemplateDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.templateDetailId = 0;
      d.accountId = '';
      d.accountName = '';
      d.debitAmount = 0;
      d.creditAmount = 0;
    },

    refreshTotals () {
      const me = this;

      me.template.debitTotal = 0;
      me.template.creditTotal = 0;

      me.templateDetails.forEach( dtl => {
        me.template.debitTotal = me.template.debitTotal + dtl.debitAmount;
        me.template.creditTotal = me.template.creditTotal + dtl.creditAmount;
      });

      me.template.debitTotal = me.template.debitTotal.toFixed(2) / 1;
      me.template.creditTotal = me.template.creditTotal.toFixed(2) / 1;
    }

  },

  created () {
    const me = this;

    me.oldTemplate = '';
    me.oldTemplateDetails = '';
    me.trxTypes = [];     // all transaction types (cache)

    // me.seqTemplateDetailId = -1;   // in-memory sequencer for added template details
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
  grid-template-columns: 2fr 8fr 2fr 2fr;
  gap: .5rem;
}

#detail-editor >>> .modal-rg {
  min-width: 75%;
}

#logs tr {
  vertical-align: top;
}

#logs td {
  font-size: .875rem;
  padding: .375rem;
}

.fixed-header {
  /* overflow-y: auto; */
  overflow: auto;
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
  flex-wrap: nowrap;
}

</style>