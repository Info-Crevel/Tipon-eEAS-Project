// Order of Payment Entry

<template>
<section class="container p-0" :key="ts">
<sym-form id="gls0200" :caption="pageName" cardClass="curved-0" headerClass="frs-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3">

  <div slot="toolbar" class="action-buttons frs-form-toolbar darken-2 p-1 px-3 border-bottom-main">
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

    <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onPrint">
        <i class="fa fa-print fa-lg"></i><span>Print</span>
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

    <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onPrint">
        <i class="fa fa-print fa-lg"></i><span>Print</span>
      </button>
    </div>

  </div>

  <div>
    <div class="d-flex justify-between align-items-end fw-118 ">
      <sym-int v-model="opx.opxId" :caption-width="42" :input-width="34" caption="Order ID" lookupId="FinOpx" @lostfocus="onOpxIdLostFocus" @changing="onOpxIdChanging" @changed="onOpxIdChanged" @searchresult="onOpxIdSearchResult"></sym-int>
      <div class="buttons d-inline">
        <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
      </div>
    </div>

    <sym-date v-model="opx.opxDate" :caption-width="42" :input-width="34" caption="Order Date" @changing="onOpxDateChanging"></sym-date>
    <sym-text v-model="opx.documentId" :caption-width="42" :input-width="44" caption="Order Number"></sym-text>
    <sym-combo v-model="opx.fundClusterId" :caption-width="42" :input-width="118" caption="Fund Cluster" display-field="fundClusterName" :datasource="fundClusters" v-if="isFundClusterVisible"></sym-combo>
    <!-- <sym-combo v-model="opx.bankId" :caption-width="42" :input-width="118" caption="Bank" display-field="bankName" :datasource="banks"></sym-combo> -->
    <sym-combo v-model="opx.coxTypeId" :caption-width="42" :input-width="44" caption="Payment Type" display-field="coxTypeName" :datasource="coxTypes"></sym-combo>
    <sym-text v-model="opx.billNumber" :caption-width="42" :input-width="44" caption="Bill Number"></sym-text>
    <sym-date v-model="opx.billDate" :caption-width="42" :input-width="34" caption="Bill Date"></sym-date>
    <!-- <sym-memo v-model="opx.particulars" :input-width="278" caption="Particulars"></sym-memo> -->
    <sym-memo v-model="opx.particulars" caption="Particulars"></sym-memo>
    <!-- <sym-text v-model="opx.payorName" :caption-width="42" :input-width="226" caption="Payor" lookupId="DbsPayor" @searchresult="onPayorNameSearchResult"></sym-text> -->
    <!-- <sym-text v-model="opx.payorName" :caption-width="42" caption="Payor" lookupId="DbsPayor" @searchresult="onPayorNameSearchResult"></sym-text> -->
    <sym-text v-model="opx.payorName" caption="Payor" align="bottom" class="w-100" lookupId="DbsPayor" @searchresult="onPayorNameSearchResult"></sym-text>

    <div>
      <div class="text-center border-light curved p-1 slategray mt-2">
        <span class="serif lg-3">Details</span>
      </div>

      <table class="light striped-even mb-0">
        <thead>
          <tr>
            <th class="w-10">Item Code</th>
            <th class="w-60">Name / Particulars</th>
            <th class="w-15 text-right">Amount</th>
            <th class="w-15">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(dtl, index) in opxDetails" :key="index" class="align-top">
            <td>{{ dtl.trxCode }}</td>
            <td>{{ dtl.trxName }}</td>
            <td class="text-right">{{ core.toDecimalFormat(dtl.amount, 2, true) }}</td>
            <td class="p-1">
              <div class="buttons" sm-1 mb-0 >
                <button type="button" class="justify-between info fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)">
                  <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                </button>
                <button type="button" class="warning btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)">
                  <i class="fa fa-times fa-lg"></i>
                </button>
              </div>
            </td>
          </tr>
          <tr>
            <td colspan="2" class="text-right bold">Total</td>
            <td class="text-right">{{ core.toDecimalFormat(opx.totalAmount) }}</td>
            <td></td>
          </tr>  
        </tbody>  
      </table>  

      <!-- <div class="action-buttons light border-main p-1 border-top-0 mb-2"> -->
      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
        <div></div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
          <button type="button" class="justify-between btn-add" @click="onAddDetail">
            <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
          </button>
        </div>
      </div>

    </div>

  </div>

  <sym-modal
    id="detail-editor"
    v-model="isDetailEditorVisible"
    size="lg"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="false"
    :dismissible="false"
    :closeOnBackButton="false"
    :title="editorTitle"
    @show="onShowDetailEditor($event)"
    @hide="onHideDetailEditor($event)"
    headerClass="frs-form-header"
    dismissButtonClass="danger"
  >

  <div class="board p-1 mb-0 w-100">
    <form id="gls0200A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="detail-editor-boxes">
        <sym-int v-model="detail.trxCode" caption="Item Code" align="bottom" lookupId="DbsTrx" @changing="onTrxCodeChanging" @searchfill="onTrxCodeSearchFill" @searchresult="onTrxCodeSearchResult"></sym-int>
        <sym-text v-model="detail.trxName" caption="Name / Particulars" align="bottom"></sym-text>
        <sym-dec v-model="detail.amount" caption="Amount" align="bottom" captionAlign="right"></sym-dec>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isDetailEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>
  </div>

  </sym-modal>

</sym-form>

<div class="to-print order">
  <table class="mt-2XXX white text-charcoal mb-0">
    <tbody>
      <tr class="structure">
        <td class="w-36"></td>
        <td class="w-40"></td>
        <td class="w-24"></td>
      </tr>

      <tr class="doc-header">
        <td class="frs-bold lg-1">{{ sym.sysInfo.siteName }}</td>
        <td></td>
        <td>No.: <span class="frs-bold ml-2">{{ opx.documentId }}</span></td>
      </tr>
      <tr class="doc-header">
        <td>Fund Cluster: <span class="frs-bold">{{ opx.fundClusterId }}</span></td>
        <td></td>
        <td>Date: <span class="frs-bold" v-if="opx.opxDate">{{ opx.opxDate.toDateDisplayFormat() }}</span></td>
      </tr>

      <tr class="doc-header text-center">
        <td colspan="3" class="frs-bold display-9">ORDER OF PAYMENT</td>
      </tr>

      <tr class="doc-header">
        <td class="frs-bold">The Collecting Officer</td>
      </tr>
      <tr class="doc-header">
        <td>Cash/Treasury Unit</td>
      </tr>
      <tr class="doc-header">
        <td></td>
      </tr>

      <tr>
        <td colspan="3">
          <p class="order-text">Please issue Official Receipt in favor of <span class="fill-text">&emsp;{{ opx.payorName }}&emsp;</span>
            in the amount of <span class="fill-text">&emsp;{{ amountText }}&emsp;</span> (Php <span class="fill-text">&emsp;{{ core.toDecimalFormat(opx.totalAmount, 2, true) }}&emsp;</span>) for
            payment of <span class="fill-text">&emsp;{{ opx.particulars }}&emsp;</span> per Bill No.
            <span class="fill-text">&emsp;{{ opx.billNumber }}&emsp;</span> dated <span class="fill-text" v-if="opx.billDate">&emsp;{{ opx.billDate.toDateDisplayFormat() }}&emsp;</span>.
          </p>
        </td>
      </tr>
      <tr class="doc-header">
        <td></td>
      </tr>
      <tr class="deposit">
        <td colspan="3">Please deposit the collections under bank account/s:</td>
      </tr>

    </tbody>  
  </table>

  <table>
    <tbody>
      <tr class="doc-header">
        <td class="w-18">Number</td>
        <td class="w-32">Name of Bank</td>
        <td class="w-14">Payment Mode</td>
        <td class="w-16">Account Code</td>
        <td class="w-20 text-right">Amount</td>
      </tr>

      <tr v-for="(dtl, index) in opxDetails" :key="index" class="deposit-info align-top">
        <td><span v-if="index ===  0">{{ opx.bankAccountNumber}}</span></td>
        <td><span v-if="index ===  0">{{ opx.bankName}}</span></td>
        <td><span v-if="index ===  0">{{ opx.coxTypeName }}</span></td>
        <td>{{ dtl.accountId }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.amount, 2, true) }}</td>
      </tr>
      <tr>
        <td colspan="4" class=" py-0"></td>
        <td class="text-right py-0">─────────</td>
      </tr>
      <tr class="deposit-info">
        <td colspan="2"></td>
        <td class="frs-bold">Total</td>
        <td></td>
        <td class="text-right">{{ core.toDecimalFormat(opx.totalAmount) }}</td>
      </tr>

      <tr class="align-top">
        <td colspan="2"></td>
        <td colspan="3" class="text-center">
          <br><br><br>
          <span class="frs-bold">{{ opx.signatoryName }}</span><br>
          <span>{{ opx.signatoryPosition }}</span><br>
        </td>
      </tr>

    </tbody>
  </table>
</div>

</section>  
</template>

<script>

import {
  appConfig
} from '../../js/session';

import {
  DateTime
} from '../../js/core';

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
  name: 'gls0200',

  data () {
    return {
      opx: {
        opxId: 0,
        opxDate: null,
        documentId: '',
        fundClusterId: '',
        bankId: 0,
        bankName: '',
        bankAccountNumber: '',
        coxTypeId: 0,
        coxTypeName: '',
        billNumber: '',
        billDate: null,
        particulars: '',
        payorName: '',
        postUserId: 0,
        signatoryId: 0,
        signatoryName: '',
        signatoryPosition: '',
        lockId: '',
        totalAmount: 0
      },

      opxDetails: [],
      isDetailEditorVisible: false,

      detail: {
        opxDetailId: 0,
        trxCode: 0,
        trxName: '',
        amount: 0
      },

      detailIndex: -1,
      isAdding: false,

      amountText: '',
      isFundClusterVisible: true,
      isAutoDocSequence: false

    };
  },

  computed: {
    editorTitle () {
      if (this.isAdding) {
        return 'Order Detail - ( Add )';
      }
      return 'Order Detail - ( Edit )';
    }

  },

  methods: {
    getTargetPath () {
      const
        q = {},
        me = this;

      if (me.opx.opxId) {
        q.opxId = me.opx.opxId;
      }

      return {
        name: me.$options.name,
        query: q
      };
    },

    syncValues (p, q) {
      const me = this;

      if ('opxId' in q && me.core.isInteger(q.opxId)) {
        me.opx.opxId = parseInt(q.opxId);
      }
    },

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.fundClusters = data.fundClusters;
            me.coxTypes = data.coxTypes;
            me.banks = data.banks;
          }
          if (me.opx.opxId < 0) {
            return Promise.resolve(null);
          }
          return me.getOpx();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.opx.length) {
            me.setModels(info);
          } else {
            if (me.opx.opxId > -1) {
              let message = "Order ID '<b>" + me.opx.opxId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }

            me.advice.warning('You are adding a new document.', { duration: 5 });
            me.opx.opxDate = me.today;
            me.opx.fundClusterId = '01';        // Regular Agency Fund
            me.opx.bankId = 1;                  // default
            me.opx.coxTypeId = 1;               // Cash
            me.opx.postUserId = me.sym.userInfo.userId;
            me.opx.signatoryId = appConfig.accountingSignatoryId;   // head, accouning

            if (me.isAutoDocSequence) {
              me.opx.documentId = '< NEW >';
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
      const
        me = this,
        opx = info.opx[0];

      me.opx = me.core.convertDates(opx);
      if (me.opx.opxDate === null) {
        me.opx.opxDate = me.core.emptyDateTime();
      }
      if (me.opx.billDate === null) {
        me.opx.billDate = me.core.emptyDateTime();
      }
      me.opxDetails = info.opxDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldOpx = JSON.stringify(me.opx);
      me.oldOpxDetails = JSON.stringify(me.opxDetails);
    },

    // API calls

    getOpx () {
      return get('api/ops/' + this.opx.opxId);
    },

    getReferences () {
      const me = this;

      if (me.fundClusters.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }
      return get('api/references/gls0200');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/ops/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/ops/' + this.opx.opxId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        opx = this.opx,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/ops/' + opx.opxId + '/' + opx.lockId, options);
    },

    getApiPayload () {
      const
        me = this,
        opx = {};

      Object.assign(opx, me.opx);
      opx.details = me.opxDetails;

      return opx;
    },

    getAmountText () {
      return get('api/amount-text?amount=' + this.opx.totalAmount);
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('opx', 'opxDetails', 'detail');
      dc.keyField = 'opxId';
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

    onOpxIdChanging (e) {
      e.callback = this.opxIdCallback;
    },

    opxIdCallback (e) {
      e.message = "Order ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.FinOpx', 'opxId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onOpxIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onOpxIdLostFocus () {
      const me = this;

      if (!me.opx.opxId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onOpxIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.opx.opxId = data.opxId;
      me.replaceUrl();
      me.loadData();
    },

    onOpxDateChanging (e) {
      if (e.proposedValue instanceof DateTime) {
        let
          me = this,
          p = e.proposedValue;

        if (p > me.today) {
          e.preventDefault();
          return;
        }
      }
    },

    onPayorNameSearchResult (result) {
      if (!result) { return; }

      this.opx.payorName = result[0].payorName;
      this.focusNext();
    },

    onTrxCodeChanging (e) {
      e.message = "Item Code '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.trxCodeCallback;
    },

    trxCodeCallback (e) {
      const me = this;
      let filter = "TrxCode='" + e.proposedValue + "' AND TrxClassId = 1 AND PayOrderFlag = 1";
      return getList('dbo.DbsTrx', 'TrxCode, TrxName, Amount', '', filter).then(
        item => {
          if (item && item.length) {
            me.detail.trxName = item[0].trxName;
            // me.opx.particulars = item[0].trxName;
            if (item[0].amount) {
              me.detail.amount = item[0].amount;
            }
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

    onTrxCodeSearchFill (e) {
      e.filter = "PayOrderFlag = 1"
    },

    onTrxCodeSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.trxCode = data.trxCode;
      detail.trxName = data.trxName;
      // this.opx.particulars = data.trxName;
      if (data.amount) {
        detail.amount = data.amount;
      }

      this.focusNext();

    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (!me.opx.totalAmount) {
        me.advice.fault('Save attempt failed. Order Details cannot be empty.', { duration: 3 } )
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
              me.opx.opxId = success; 
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

      getSafeDeleteFlag('FinOpx', me.opx.opxId)
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
        'trxCode',
        'amount'
      );  

      me.setDisplayMode(
        'trxName'
      );

      setTimeout(() => {
        this.setFocus('trxCode');
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

      d.opxDetailId = dtl.opxDetailId;
      d.trxCode = dtl.trxCode;
      d.trxName = dtl.trxName;
      d.amount = dtl.amount;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.opxDetailId = -1;

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Item <b>' + me.opxDetails[index].trxName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.opxDetails.splice(index, 1);
            me.refreshTotal();
          }
          return;
        }
      );
    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      if (!d.trxCode) {
        me.isDetailEditorVisible = false;
        return;
      }

      if (!me.isValid('gls0200A')) {
        me.advice.fault('Fill in the required fields (marked in red) before submission.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAdding) {

        Object.assign(dtl, d);
        me.opxDetails.push(dtl);

        me.clearDetailPad();
        me.advice.info("Item '" + dtl.trxName + "' added to list.", { duration: 5 });
        // me.setFocus('trxCode');
        me.isDetailEditorVisible = false;     // close editor after adding an item
        me.refreshTotal();

      } else {
        dtl = me.opxDetails[me.detailIndex];

        dtl.opxDetailId = d.opxDetailId;
        dtl.trxCode = d.trxCode;
        dtl.trxName = d.trxName;
        dtl.amount = d.amount;

        me.isDetailEditorVisible = false;
        me.refreshTotal();
      }

    },

    onPrint () {
      const me = this;

      if (me.hasChanges()) {
        me.advice.fault('Cannot print with uncommitted changes.', { duration: 3 } )
        return;
      }

      me.getAmountText().then(
        text => {
          me.amountText = text;
          me.$nextTick( () => {
            window.print();
          })
        },
        fault => {
          me.showFault(fault);
        }
      );

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
          'opxDate',
          'fundClusterId',
          'bankId',
          'coxTypeId',
          'billNumber',
          'billDate',
          'particulars',
          'payorName'
        );

        if (me.isAutoDocSequence) {
          me.setDisplayMode(
            'documentId'
          );
        } else {
          me.setRequiredMode(
            'documentId'
          );  
        }

        me.setFocus('opxDate');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (JSON.stringify(me.opx) !== me.oldOpx) { return true; }
      if (JSON.stringify(me.opxDetails) !== me.oldOpxDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.coxDetailId = 0;
      d.trxCode = 0;
      d.trxName = '';
      d.amount = 0;
    },

    refreshTotal () {
      const me = this;

      me.opx.totalAmount = 0;

      me.opxDetails.forEach( dtl => {
        me.opx.totalAmount = me.opx.totalAmount + dtl.amount;
      });
    },

  },

  created () {
    const me = this;

    me.oldOpx = '';
    me.oldOpxDetails = '';
    me.fundClusters = [];     // all fund clusters (cache)
    me.coxTypes = [];         // all collection types (cache)
    me.banks = [];            // all banks (cache)

    me.isFundClusterVisible = appConfig.fundClusterPrompt;
    me.isAutoDocSequence = appConfig.autoDocSequence;

    me.today = me.sym.dateInfo.serverDate;

  }

}

</script>

<style>

@media print {
  body * {
    visibility: hidden;
  }

  html, body {
    /* height:100vh;  */
    /* margin: 0 !important;  */
    /* padding: 0 !important; */
    overflow: hidden;
  }
}

</style>

<style scoped>

.action-buttons {
  display: grid;
  /* grid-template-columns: 1fr 3fr 1fr; */
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.command-buttons {
  display: grid;
  grid-template-columns: 1fr 3fr 1fr;
  gap: .75rem;
}

.detail-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 2fr 6fr 2fr;
  gap: .5rem;
}

@page {
  size: 9.5in 5.5in;
  /* margin: 0in;  */
  margin: .25in; 
  /* margin-top: .25in; */
  /* margin-left: .25in;
  margin-right: .25in; */
}

.to-print.order {
  height: 5in;
  padding: .5rem;
  border: 2px solid dimgray;
}

.to-print table {
  border: none;
  line-height: 1.0;
}

.to-print .structure td {
  border: none !important;
  padding: 0;
}

.to-print .doc-header td {
  padding-top: .5rem;
  padding-bottom: 0;
  line-height: .8
}

.to-print td {
  border: none !important;
}

.to-print .order-text {
  text-indent: 2rem;
  font-family: 'Heebo';
  line-height: 1.4;
  margin-bottom: 0;
}

.to-print .fill-text {
  /* font-family: 'Noto'; */
  font-family: 'Roboto Condensed';
  font-size: 1.125rem;
  text-decoration-line: underline;
}

.to-print .deposit {
  font-family: 'Heebo';
}

.to-print .deposit-info td {
  font-family: 'Heebo';
  padding-top: .625rem;
  padding-bottom: 0;
  line-height: .8
}

@media only screen {
  .to-print,
  .to-print * {
    visibility: hidden;
    display: none;
  }
}

@media print {
  .to-print,
  .to-print * {
    visibility: visible;
  }

  .to-print {
    position: absolute;
    left: 0;
    top: 0;
  }
}

.frs-bold {
  font-family: 'Heebo Bold' !important;
}

</style>