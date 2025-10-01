// Payables

<template>
<section class="container p-0" :key="ts">
   <sym-form id="aps0100" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main app-form-footer darken-2 py-0" bodyClass="app-form-body pb-3 ">

  <div slot="toolbar" class="action-buttons app-form-toolbar darken-2 p-1 px-3 border-bottom-main">
    <!-- <div></div> -->
    <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onCancel" v-if="!isCancelled">
        <i class="fa fa-times fa-lg"></i><span class="bold">CANCEL</span>
      </button>
    </div>

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
        <i class="fa fa-print fa-lg"></i><span>Voucher</span>
      </button>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onPrint2307">
        <i class="fa fa-print fa-lg"></i><span>2307</span>
      </button>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog">
        <i class="fa fa-database fa-lg"></i><span>Log</span>
      </button>
    </div>

  </div>

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <!-- <div></div> -->
    <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onCancel" v-if="!isCancelled">
        <i class="fa fa-times fa-lg"></i><span class="bold">CANCEL</span>
      </button>
    </div>

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
        <i class="fa fa-print fa-lg"></i><span>Voucher</span>
      </button>
          <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onPrint2307">
        <i class="fa fa-print fa-lg"></i><span>2307</span>
      </button>
      <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog">
        <i class="fa fa-database fa-lg"></i><span>Log</span>
      </button>
    </div>

  </div>

  <div>
    <!-- <div class="d-flex justify-between align-items-end fw-118 ">
      <sym-int v-model="trx.trxId" :caption-width="42" :input-width="34" caption="Transaction ID" lookupId="FinTrxGeneral" @lostfocus="onTrxIdLostFocus" @changing="onTrxIdChanging" @changed="onTrxIdChanged" @searchresult="onTrxIdSearchResult"></sym-int>
      <div class="buttons d-inline">
        <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
      </div>
    </div> -->

    <div class="box-container">
      <div class="d-flex justify-between align-items-end fw-118 ">
        <sym-int v-model="trx.trxId" :caption-width="42" :input-width="34" caption="Transaction ID" lookupId="FinTrxPayable" @lostfocus="onTrxIdLostFocus" @changing="onTrxIdChanging" @changed="onTrxIdChanged" @searchresult="onTrxIdSearchResult"></sym-int>
        <div class="buttons d-inline">
          <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
        </div>
        <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
      </div>
      
      <sym-text v-model="trx.documentId" :caption-width="42" :input-width="44" caption="Voucher Number"></sym-text>
    </div>

    <div class="box-1 gap-1">
 
    <sym-date v-model="trx.trxDate" :caption-width="42" :input-width="34" caption="Trx Date" @changing="onTrxDateChanging"></sym-date>
    <sym-combo v-model="trx.payableTypeId" :caption-width="20" :input-width="90" caption="Type" display-field="payableTypeName" :datasource="types"></sym-combo>
    
     </div>

    <div class="d-flex gap-1">
      <sym-int v-model="trx.payeeId" :caption-width="42" :input-width="34" caption="Payee ID" lookupId="ApsPayee" @changing="onPayeeIdChanging" @searchresult="onPayeeIdSearchResult"  @changed="onAmountChanged"></sym-int>
      <sym-text v-model="trx.payeeName" class="w-43"></sym-text>
    </div>

      <div class="tax-container gap-1">

       <sym-combo v-model="trx.payableTaxCode" :caption-width="42" :input-width="90" caption="Tax" display-field="payableTaxName" :datasource="taxes" @changed="onPayableTaxCodeChanged"></sym-combo>
        <sym-dec class="text-right"  v-model="trx.taxPercentage" :caption-width="30" :input-width="20" caption="Percentage" @changed="onAmountChanged"></sym-dec>
        <sym-combo v-model="trx.aTaxCode" :caption-width="20" :input-width="90" caption="ATC" display-field="aTaxName" :datasource="ataxes" @changed="onATaxCodeChanged"></sym-combo>
        <sym-dec class="text-right"  v-model="trx.aTaxPercentage" :caption-width="30" :input-width="20" caption="Percentage" @changed="onAmountChanged"></sym-dec>
      </div>
 
    <div class="box-3 gap-1">
       
      <sym-text v-model="trx.reference" :caption-width="42" :input-width="70" caption="Reference" ></sym-text>
      <sym-dec class="text-right"  v-model="trx.amount" align="left"  :caption-width="20" :input-width="40" caption="Amount" @changed="onAmountChanged"></sym-dec>
    </div>

    <div class="grid cols-4 gap-1">
      <sym-memo v-model="trx.particulars" caption="Particulars"></sym-memo>
      <sym-memo v-model="trx.remarks" caption="Remarks"></sym-memo>
        <!-- <sym-text v-model="trx.reference" align="bottom" :caption-width="42" :input-width="70" caption="Reference" ></sym-text> -->
    
      
    </div>

    <!-- <div class="grid cols-4 gap-1">
    <sym-combo v-model="trx.bankId" :caption-width="30" :input-width="100" caption="Bank" display-field="bankName" :datasource="banks"></sym-combo>
    <sym-text v-model="trx.reference" :caption-width="30" :input-width="50" caption="Reference" ></sym-text>
    </div> -->


    <!-- <sym-tabs>  
        <sym-tab title="Account Distribution" icon="paper"> -->
    <div>
      <div class="text-center border-light curved p-1 slategray mt-2">
        <span class="serif lg-3">Details</span>
      </div>
      <div class="tb-container">

      
      <table class="light striped-even mb-0 tb-scroller">
        <thead>
          <tr>
            <th class="w-10 text-center" >Action</th>
            <th class="w-10">Account ID</th>
            <th class="w-30">Account Name</th>
            <th class="w-10 text-right">Debit</th>
            <th class="w-10 text-right">Credit</th>
            <th class="w-30">Cooperative</th>
            <th class="w-30">Platform</th>
            <th class="w-20">Cluster</th>
            <th class="w-40">Pay Group</th>
            <th class="w-20">SL Name</th>

        
        
          </tr>
        </thead>
        <tbody>
          <tr v-for="(dtl, index) in trxDetails" :key="index">
        
            <td class="p-1">
              <div class="act-btn gap" sm-1 mb-0 >
                <button type="button" class=" info edit-btn btn-dtl-edit w-100" @click="onEditDetail(dtl, index)">
                  <i class="fa fa-edit fa-lg"></i><span></span>
                </button>
                <button type="button" class="warning edit-btn btn-dtl-delete w-100" title="Delete" @click="onDeleteDetail(index)">
                  <i class="fa fa-times fa-lg"></i>
                </button>
              </div>
            </td>
            <td>{{ dtl.accountId }}</td>
            <td>{{ dtl.accountName }}</td>
    <td class="text-right">{{ core.toDecimalFormat(dtl.debitAmount, 2, true) }}</td>
            <td class="text-right">{{ core.toDecimalFormat(dtl.creditAmount, 2, true) }}</td>
            <td>{{ dtl.orgName }}</td>
            <td>{{ dtl.platformName }}</td>
            <td>{{ dtl.clusterName }}</td>
            <td>{{ dtl.clientPayGroupName }}</td>
            <td>{{ dtl.payeeName }}</td>


          </tr>
          <tr :class="totalsClass">
            <td colspan="3" class="text-right bold">Total</td>
            <td class="text-right">{{ core.toDecimalFormat(trx.debitTotal) }}</td>
            <td class="text-right">{{ core.toDecimalFormat(trx.creditTotal) }}</td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
          </tr>  
        </tbody>
      </table>
      </div>

      <!-- <div class="action-buttons light border-main p-1 border-top-0 mb-2"> -->
      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
        <div></div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
          <button type="button" class="justify-between btn-add" @click="onAddDetail">
            <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
          </button>
          <!-- <button type="button" class="justify-between btn-template" @click="onSelectTemplate">
            <i class="fa fa-search fa-lg"></i><span>Template</span>
          </button>
          <button type="button" class="justify-between btn-replace" @click="isAmountEditorVisible=true">
            <i class="fa fa-copy fa-lg"></i><span>Replace</span>
          </button> -->
        </div>
      </div>

    </div>

        <!-- </sym-tab>



      </sym-tabs>   -->


  </div>  

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
    :title="editorTitle"
    @show="onShowDetailEditor($event)"
    @hide="onHideDetailEditor($event)"
    headerClass="app-form-header"
    dismissButtonClass="danger"
  >

  <div class="board p-1 mb-0 w-100">
    <form id="aps0100A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="detail-editor-boxes">
         <div class="acc-container gap">
          <sym-text v-model="detail.accountId" caption="Account ID" align="bottom" lookupId="FinChartDetail" @changing="onAccountIdChanging" @searchresult="onAccountIdSearchResult"></sym-text>
          <sym-text v-model="detail.accountName" caption="Account Name" align="bottom"></sym-text>
          <sym-dec v-model="detail.debitAmount" caption="Debit" align="bottom" captionAlign="right" @changed="onDebitAmountChanged"></sym-dec>
          <sym-dec v-model="detail.creditAmount" caption="Credit" align="bottom" captionAlign="right" @changed="onCreditAmountChanged"></sym-dec>
        </div>
<!-- 
        <div class="coop-container gap">
          
        </div> -->

        <div class="coop-container ">
          <sym-combo v-model="detail.orgId" caption="Cooperative" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
          <sym-combo v-model="detail.platformId" caption="Platform" align="bottom" display-field="platformName" :datasource="platforms" ></sym-combo>
        </div>
        <div class="lookup-container-1 gap">
          <sym-int v-model="detail.clusterId" caption="Cluster" align="bottom" lookupId="DbsCluster" @changing="onClusterIdChanging" @searchresult="onClusterIdSearchResult"></sym-int> 
          <sym-text v-model="detail.clusterName" caption="Cluster Name" align="bottom"></sym-text>

        </div>

        <!-- <sym-combo v-model="detail.clientPayGroupId" caption="Pay Group" align="bottom" display-field="clientPayGroupName" :datasource="payGroups" ></sym-combo> -->
        <div class="lookup-container-3 gap">
          <sym-int v-model="detail.clientPayGroupId" caption="Pay Group ID" align="bottom" lookupId="ArsclientPayGroup" @changing="onClientPayGroupIdChanging" @searchresult="onClientPayGroupIdSearchResult"></sym-int>
          <sym-text v-model="detail.clientPayGroupName" caption=" Pay Group Name" align="bottom"></sym-text>
        </div>
        
        <div class="lookup-container-1 gap">
          <sym-int v-model="detail.payeeId" caption="Payee ID" align="bottom" lookupId="ApsPayee" @changing="onDetailPayeeIdChanging" @searchresult="onDetailPayeeIdSearchResult"></sym-int>
          <sym-text v-model="detail.payeeName" caption=" Payee Name" align="bottom"></sym-text>
        </div>
        


        <!-- <sym-combo v-model="detail.payeeId" caption="Cluster" align="bottom" display-field="clusterName" :datasource="clusters" lookupId="DbsCluster" @changing="onClusterIdChanging" @searchresult="onClusterIdSearchResult"></sym-combo> -->
       
        
        
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isDetailEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>  
  </div>

  </sym-modal>

  <sym-modal
    id="amount-editor"
    v-model="isAmountEditorVisible"
    size="md"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="true"
    :dismissible="false"
    :closeOnBackButton="false"
    title="Debit / Credit Amount Replacer"
    @show="onShowAmountEditor($event)"
    @hide="onHideAmountEditor($event)"
    headerClass="app-form-header"
    dismissButtonClass="danger"
  >

  <div class="board p-1 mb-0 w-100">
    <form id="aps0100B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="amount-editor-boxes">
        <sym-dec v-model="amount.oldValue" caption="Replace Amount" align="bottom" captionAlign="right"></sym-dec>
        <sym-dec v-model="amount.newValue" caption="With This Amount" align="bottom" captionAlign="right"></sym-dec>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitAmount()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isAmountEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>  
  </div>

  </sym-modal>

</sym-form>

<sym-modal
  v-model="isLogVisible"
  size="xl"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="true"
  :dismissible="true"
  :closeOnBackButton="false"
  title="Payables Change Log"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>
  <div class="fixed-header">
    <table id="logs" class="striped-odd">
      <thead>
        <tr>
          <th class="w-7">Action</th>
          <th class="w-15">Column</th>
          <th class="w-18">Old Value</th>
          <th class="w-18">New Value</th>
          <th class="w-15">Log Date/Time</th>
          <th class="w-10">User</th>
          <th class="w-22">Reference</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="(log, index) in logs" :key="index">
          <td>{{ log.logDescription }}</td>
          <td>{{ log.columnCaption }}</td>
          <td>{{ log.oldValue }}</td>
          <td>{{ log.newValue }}</td>
          <td>{{ core.toDateFormat(log.logDateTime, true, 'MM/dd/yyyy h:mm tt' ) }}</td>
          <td>{{ log.logUserInfo }}</td>
          <td>{{ log.logReference }}</td>
        </tr>
      </tbody>

    </table>
  </div>

</sym-modal>

<div class="to-print">
  <table class="mt-2 white text-charcoal mb-0">
    <tbody>
      <tr class="structure">
        <td class="w-20"></td>
        <td class="w-10"></td>
        <td class="w-10"></td>
        <td class="w-7"></td>
        <!-- <td class="w-2"></td> -->
      </tr>

      <tr class="doc-header">
        <td colspan="3">Voucher No. <span class="lg-1 app-bold">{{ trx.documentId }}</span></td>
      </tr>


      <tr class="doc-header">
        <!-- <td colspan="1">Voucher No. <span class="lg-1 app-bold">{{ trx.documentId }}</span></td> -->

        <td colspan="3" class="text-center display-9 app-bold doc-title">PAYABLE VOUCHER</td>

        <td colspan="1" class="text-left">Trx No. <span class="lg-1 app-bold">{{ trx.trxId }}</span></td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-center lg-2 app-bold">{{ sym.sysInfo.siteName }}</td>
        <td colspan="1" v-if="trx.trxDate">Date: {{ trx.trxDate.toDateDisplayFormat() }}</td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-center sm-1 pt-0">Organization Name</td>
        <td colspan="1" class="text-center app-bold">Amount</td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-left sm-1 pt-0">Payee ID: {{ trx.payeeId }} &nbsp;&nbsp;  Payee Name: {{ trx.payeeName }}</td>
      </tr>
      <!-- <tr class="doc-header">
        <td colspan="3" class="text-left sm-1 pt-0">Payee Name: {{ trx.payeeName }}</td>
      </tr> -->

      <tr class="col-header app-bold align-top">
        <!-- <td>Cost Ctr</td> -->
        <td>Accounts</td>
        <td>Acct Code</td>
        <td class="text-right">Debit</td>
        <td class="text-right">Credit</td>
        <!-- <td ></td> -->
      </tr>

      <tr v-for="(dtl, index) in trxDetails" :key="index" class="align-top">
        <!-- <td><span v-show="index === 0">{{ trx.costCenterId }}</span></td> -->
        <td>{{ dtl.accountName }}</td>
        <td>{{ dtl.accountId }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.debitAmount, 2, true) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.creditAmount, 2, true) }}</td>
      </tr>

      <tr>
        <!-- <td class="spacer"></td> -->
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
      </tr>

      <tr>
        <!-- <td></td> -->
        <td><i>{{ trx.particulars }}</i></td>
        <td></td>
        <td></td>
        <td></td>
      </tr>

      <tr>
        <td colspan="2" class="text-right app-bold">Total</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.debitTotal) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.creditTotal) }}</td>
        <!-- <td></td> -->
      </tr>  


    </tbody>
    <tfoot class="w-100">
      
     
          <td  colspan="1">
            Prepared By: <br><br><br><br>
            <span class="lg-1 app-bold">{{ trx.requestSignatoryName }}</span> <br>
            <span>{{ trx.requestSignatoryPosition }}</span>
          </td>
          <td colspan="2 ">
            Certified Correct: <br><br><br><br>
            <span class="lg-1 app-bold">{{ trx.signatoryName }}</span> <br>
            <span>{{ trx.signatoryPosition }}</span>
          </td>  
          <td colspan="1">
            Approved By: <br><br><br><br>
            <span class="lg-1 app-bold">{{ trx.signatoryName }}</span> <br>
            <span>{{ trx.signatoryPosition }}</span>
          </td> 

    
    </tfoot>
  </table>

  <span v-show="trx.remarks">* {{ trx.remarks }}</span>

  <br>
  <hr>

  <table class="mt-2 white text-charcoal mb-0">
    <tbody>
      <tr class="structure">
        <!-- <td class="w-11"></td> -->
        <td class="w-43"></td>
        <td class="w-14"></td>
        <td class="w-16"></td>
        <td class="w-16"></td>
      </tr>

    <tr class="doc-header">
        <td colspan="3">Voucher No. <span class="lg-1 app-bold">{{ trx.documentId }}</span></td>
      </tr>


      <tr class="doc-header">
        <!-- <td colspan="1">Voucher No. <span class="lg-1 app-bold">{{ trx.documentId }}</span></td> -->

        <td colspan="3" class="text-center display-9 app-bold doc-title">PAYABLE VOUCHER</td>

        <td colspan="1">Trx No. <span class="lg-1 app-bold">{{ trx.trxId }}</span></td>
      </tr>
      
      <tr class="doc-header">
        <td colspan="3" class="text-center lg-2 app-bold">{{ sym.sysInfo.siteName }}</td>
        <td colspan="1" v-if="trx.trxDate">Date: {{ trx.trxDate.toDateDisplayFormat() }}</td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-center sm-1 pt-0">Organization Name</td>
        <td colspan="1" class="text-center app-bold">Amount</td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-left sm-1 pt-0">Payee ID: {{ trx.payeeId }} &nbsp;&nbsp;  Payee Name: {{ trx.payeeName }}</td>
      </tr>
      <tr class="col-header app-bold align-top">
        <!-- <td>Cost Ctr</td> -->
        <td>Accounts</td>
        <td>Acct Code</td>
        <td class="text-right">Debit</td>
        <td class="text-right">Credit</td>
      </tr>

      <tr v-for="(dtl, index) in trxDetails" :key="index" class="align-top">
        <!-- <td><span v-show="index === 0">{{ trx.costCenterId }}</span></td> -->
        <td>{{ dtl.accountName }}</td>
        <td>{{ dtl.accountId }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.debitAmount, 2, true) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.creditAmount, 2, true) }}</td>
      </tr>

      <tr>
        <!-- <td class="spacer"></td> -->
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
      </tr>

      <tr>
        <!-- <td></td> -->
        <td><i>{{ trx.particulars }}</i></td>
        <td></td>
        <td></td>
        <td></td>
      </tr>

      <tr>
        <td colspan="2" class="text-right app-bold">Total</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.debitTotal) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.creditTotal) }}</td>
        <!-- <td></td> -->
      </tr>  

      <tr class="align-top">
        <!-- <td></td> -->
        <td>
          Prepared By: <br><br><br><br>
          <span class="lg-1 app-bold">{{ trx.requestSignatoryName }}</span> <br>
          <span>{{ trx.requestSignatoryPosition }}</span>
        </td>
        <td colspan="3">
          Certified Correct: <br><br><br><br>
          <span class="lg-1 app-bold">{{ trx.signatoryName }}</span> <br>
          <span>{{ trx.signatoryPosition }}</span>
        </td>
                
      </tr>

    </tbody>
  </table>

  <span v-show="trx.remarks">* {{ trx.remarks }}</span>


  <!-- <br> -->

  <!-- <table class="mt-2 white text-charcoal mb-0">
    <tbody>
      <tr class="structure">
        <td class="w-11"></td>
        <td class="w-43"></td>
        <td class="w-14"></td>
        <td class="w-16"></td>
        <td class="w-16"></td>
      </tr>

      <tr class="doc-header">
        <td colspan="3" class="text-center display-9 app-bold doc-title">JOURNAL ENTRY VOUCHER</td>
        <td colspan="2">No. <span class="lg-1 app-bold">{{ docId }}</span></td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-center lg-2 app-bold">{{ sym.sysInfo.siteName }}</td>
        <td colspan="2" v-if="trx.trxDate">Date: {{ trx.trxDate.toDateDisplayFormat() }}</td>
      </tr>
      <tr class="doc-header">
        <td colspan="3" class="text-center sm-1 pt-0">Agency Name</td>
        <td colspan="2" class="text-center app-bold">Amount</td>
      </tr>

      <tr class="col-header app-bold align-top">
        <td>Cost Ctr</td>
        <td>Accounts and Explanation</td>
        <td>Acct Code</td>
        <td class="text-right">Debit</td>
        <td class="text-right">Credit</td>
      </tr>

      <tr v-for="(dtl, index) in trxDetails" :key="index" class="align-top">
        <td><span v-show="index === 0">{{ trx.costCenterId }}</span></td>
        <td>{{ dtl.accountName }}</td>
        <td>{{ dtl.accountId }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.debitAmount, 2, true) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(dtl.creditAmount, 2, true) }}</td>
      </tr>

      <tr>
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
        <td class="spacer"></td>
      </tr>

      <tr>
        <td></td>
        <td><i>{{ trx.particulars }}</i></td>
        <td></td>
        <td></td>
        <td></td>
      </tr>

      <tr>
        <td colspan="3" class="text-right app-bold">Total</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.debitTotal) }}</td>
        <td class="text-right">{{ core.toDecimalFormat(trx.creditTotal) }}</td>
        <td></td>
      </tr>  

      <tr class="align-top">
        <td></td>
        <td>
          Prepared By: <br><br><br><br>
          <span class="lg-1 app-bold">{{ trx.requestSignatoryName }}</span> <br>
          <span>{{ trx.requestSignatoryPosition }}</span>
        </td>
        <td colspan="3">
          Certified Correct: <br><br><br><br>
          <span class="lg-1 app-bold">{{ trx.signatoryName }}</span> <br>
          <span>{{ trx.signatoryPosition }}</span>
        </td>          
      </tr>

    </tbody>
  </table>

  <span v-show="trx.remarks">* {{ trx.remarks }}</span> -->

</div>

</section>  
</template>

<script>

import {
  appConfig
} from '../../js/session';

import {
  createPDF
} from '../../js/pdf2307';

import {
  DateTime
} from '../../js/core';

import {
  get,
  ajax
} from '../../js/http';

import {
  getList,
  getCount,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'aps0100',

  data () {
    return {

      trx: {
        trxId: 0,
        trxDate: null,
        trxStatusId: 0,
        documentId: '',
        particulars: '',
        bankId: 0,
        remarks: '',
        reference: '',
        postUserId: 0,
        lockId: '',
        debitTotal: 0,
        creditTotal: 0,
        payeeId: 0,
        payeeName: '',
        payableTypeId: 0,
        payableTaxCode: 0,
        aTaxCode: '',
        details: '',
        dueDate: null,
        checkDate: null,
        amount: 0,
        taxPercentage: 0,
        aTaxPercentage: 0,
        taxPercentage: 0,
        taxAccount: '',
        taxAccountName: '',
        aTaxAccount: '',
        aTaxAccountName: '',
        payableTradeAccount: '',
        payeeAccount: '',
        payeeAccountName: '',
        payableTradeAmount: 0,
        apsLockId: '',
        address1: '',
        postalCode: '',
        taxIdNumber:'',
        aTaxAmount: 0,
        aTaxName: '',
      },

      // aps: {
      //   trxId: 0,
      //   payeeId: 0,
      //   payeeName: '',
      //   payableTypeId: 0,
      //   payableTaxCode: 0,
      //   aTaxCode: '',
      //   details: '',
      //   dueDate: null,
      //   checkDate: null,
      // },

      trxDetails: [],

      isDetailEditorVisible: false,
      isAmountEditorVisible: false,

      detail: {
        trxDetailId: 0,
        accountId: '',
        accountName: '',
        debitAmount: 0,
        creditAmount: 0,        

        orgName: '',        
        platformName: '',
        clusterName: '',        
        clientPayGroupName: '',
        payeeName: '',        

        orgId: 0,        
        platformId: 0,
        clusterId: 0,        
        clientPayGroupId: 0,
        payeeId: 0,


      },

      amount: {
        oldValue: 0,
        newValue: 0
      },

      detailIndex: -1,
      logs: [],
      isLogVisible: false,
      isAdding: false,

      isFundClusterVisible: true,
      isCostCenterVisible: true,
      isAutoDocSequence: false

      
      
    };
  },

  computed: {
    totalsClass () {
      if (this.trx.debitTotal !== this.trx.creditTotal) {
        return 'danger-light';
      }
      return 'success-light';
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Account Detail - ( Add )';
      }
      return 'Account Detail - ( Edit )';
    },

    docId () {
      return this.trx.documentId + ' (GJ)';
    },

    isCancelled () {
      return this.trx.trxStatusId === 9;
    },

    isEditDocumentIdVisible () {
      const me = this;

      return me.isFilled && me.isAutoDocSequence && !me.isNew() && !me.isCancelled;
    }
  },

  methods: {


    onDetailPayeeIdChanging (e) {
      e.message = "Payee ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.detailPayeeIdCallback;
    },

    detailPayeeIdCallback (e) {
      const me = this;
      let filter = "PayeeId='" + e.proposedValue ;//+ "' AND HeaderFlag = 0";
      return getList('dbo.ApsPayee', 'PayeeId, PayeeName', '', filter).then(
        payee => {
          if (payee && payee.length) {
            me.detail.payeeName = payee[0].payeeName;
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

    onDetailPayeeIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.payeeId = data.payeeId;
      detail.payeeName = data.payeeName;

      this.focusNext();

    },




    onClientPayGroupIdChanging (e) {
      e.message = "Client Pay Group ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId='" + e.proposedValue ;//+ "' AND HeaderFlag = 0";
      return getList('dbo.ArsClientPayGroup', 'ClientPayGroupId, ClientPayGroupName', '', filter).then(
        payGroup => {
          if (payGroup && payGroup.length) {
            me.detail.clientPayGroupName = payGroup[0].clientPayGroupName;
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
        data = result[0],
        detail = this.detail;

      detail.clientPayGroupId = data.clientPayGroupId;
      detail.clientPayGroupName = data.clientPayGroupName;

      this.focusNext();

    },



    onClusterIdChanging (e) {
      e.message = "Cluster ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.clusterIdCallback;
    },

    clusterIdCallback (e) {
      const me = this;
      let filter = "ClusterId='" + e.proposedValue ;//+ "' AND HeaderFlag = 0";
      return getList('dbo.DbsCluster', 'ClusterId, ClusterName', '', filter).then(
        cluster => {
          if (cluster && cluster.length) {
            me.detail.clusterName = cluster[0].clusterName;
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

    onClusterIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      detail.clusterId = data.clusterId;
      detail.clusterName = data.clusterName;

      this.focusNext();

    },


    onPrint2307() {
    const match = this.trxDetails.find(
      detail => detail.accountId === this.trx.aTaxAccount
    );

    const aTaxAmount = match ? match.creditAmount || 0 : 0;

      this.trx.aTaxAmount = aTaxAmount;


    const matchPayeeAccount = this.trxDetails.find(
      detail => detail.accountId === this.trx.payeeAccount
    );

    const payeeAmount = matchPayeeAccount ? matchPayeeAccount.debitAmount || 0 : 0;

      this.trx.payeeAmount = payeeAmount;


      createPDF(this.trx);
    },

    onPayableTaxCodeChanged(){
      this.taxes.forEach((tax) => {
          if (tax.payableTaxCode == this.trx.payableTaxCode) {
            this.trx.taxPercentage = tax.percentage;
            this.trx.taxAccount = tax.accountId;
            this.trx.taxAccountName = tax.accountName;
          }
        });
      this.onDefaultEntry();    
    },

    onATaxCodeChanged(){
      this.ataxes.forEach((tax) => {
          if (tax.aTaxCode == this.trx.aTaxCode) {
            this.trx.aTaxPercentage = tax.percentage;
            this.trx.aTaxAccount = tax.accountId;
            this.trx.aTaxAccountName = tax.accountName;
          }
        });
      this.onDefaultEntry();    
    },

    onAmountChanged(){
      this.onDefaultEntry()
    },

  onDefaultEntry() {




  const percentage = this.trx.taxPercentage || 0;
  const aPercentage = this.trx.aTaxPercentage || 0;
  let amount = this.trx.amount || 0;

      if (this.trx.debitTotal) {
       amount = this.trx.debitTotal;
      }


  let newTaxAmount = 0;
  let newATaxAmount = 0;
  // Calculate tax amount
  if (percentage > 0) {
    newTaxAmount = this.core.toDecimal(((amount / (1 + (percentage / 100))) * (percentage / 100)));
  }

  // Calculate aTax amount
  if (aPercentage > 0 && percentage > 0) {
    newATaxAmount = this.core.toDecimal((amount / (1 + (percentage / 100))));
    newATaxAmount = this.core.toDecimal((newATaxAmount * (aPercentage / 100)));
  } else if (aPercentage > 0) {
    newATaxAmount = this.core.toDecimal((amount * (aPercentage / 100)));
  }

  // Clear previous entries
  this.trxDetails = [];

  // Add ATax credit line if applicable
  if (aPercentage > 0) {
    this.trxDetails.push({
      accountId: this.trx.aTaxAccount || '',
      accountName: this.trx.aTaxAccountName || 'NO ACCOUNT',
      creditAmount: newATaxAmount,
      debitAmount: 0
    });
  }

  // Add trade account credit line
  
  const tradeAccount = this.control[0].payableTradeAccount || '';
  const tradeAccountName = this.control[0].payableTradeAccountName || 'NO ACCOUNT';
  this.trxDetails.push({
    accountId: tradeAccount,
    accountName: tradeAccountName,
    creditAmount: amount - newATaxAmount,
    debitAmount: 0
  });

  // Add payee debit line
  this.trxDetails.push({
    accountId: this.trx.payeeAccount || '',
    accountName: this.trx.payeeAccountName || 'NO ACCOUNT',
    debitAmount: amount - newTaxAmount,
    creditAmount: 0
  });

  // Add tax debit line if tax applies
  if (percentage > 0) {
    this.trxDetails.push({
      accountId: this.trx.taxAccount || '',
      accountName: this.trx.taxAccountName || 'NO ACCOUNT',
      debitAmount: newTaxAmount,
      creditAmount: 0
    });
  }

  this.trx.payableTradeAmount = this.core.toDecimal(amount - newATaxAmount)
  this.refreshTotals();

},

   
    // onDefaultEntry(){
    //   let percentage = this.trx.taxPercentage;
    //   let aPercentage = this.trx.aTaxPercentage;

    //   let newTaxAmount = 0;
    //   let newATaxAmount = 0;

    //   if (percentage > 0) {
    //     newTaxAmount = Math.Round(CDec((this.trx.amount / (1 + (percentage / 100))) * (percentage / 100)), 2)  
    //   }

    //   if ( aPercentage > 0 && percentage > 0) {
    //      newATaxAmount = Math.Round(CDec((this.trx.amount / (1 + (percentage / 100)))), 2)   
    //      newATaxAmount = Math.Round(newATaxAmount * (aPercentage / 100), 2)
    //   } else {
    //       newATaxAmount = Math.Round(CDec((this.trx.amount * ((aPercentage / 100)))), 2)  
    //   }

    //   this.details is an array
    //   if aPercentage > 0 then 
    //   push this.details.accountId = this.trx.aTaxAccount if this.trx.aTaxAccount is not null else 'NO ACCOUNT',   
    //   push this.details.creditAmount = newATaxAmount,   


    //   if this.control[0].payableTradeAccount is not null then push this.details.accountId = this.control[0].payableTradeAccount else 'NO ACCOUNT'
    //         push this.details.creditAmount = this.trx.amount - newATaxAmount,   

    //   if this.trx.payeeAccount is not null then push this.details.accountId =this.trx.payeeAccount  else 'NO ACCOUNT'
    //         push this.details.debitAmount = this.trx.amount - newTaxAmount

    //   if this.trx.percentage > 0 then       
    //  push this.details.accountId = this.trx.taxAccount if this.trx.taxAccount is not null else 'NO ACCOUNT',   
    //   push this.details.debitAmount = newTaxAmount,   



    // }, 

    onPayeeIdChanging (e) {
      e.message = "Payee ID '<b>" + e.proposedValue + "</b>' not found.";

      e.callback = this.payeeIdCallback;
    },

    payeeIdCallback (e) {
      const me = this;
      let filter = "PayeeId=" + e.proposedValue;
      return getList('dbo.ApsPayee', 'PayeeId, PayeeName, AccountId', '', filter).then(
        data => {
          if (data && data.length) {
            me.trx.payeeName = data[0].payeeName;
            me.trx.payeeAccount = data[0].payeeAccount;
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

    onPayeeIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        trx = this.trx;

      trx.payeeId = data.payeeId;
      trx.payeeName = data.payeeName;
      trx.payeeAccount = data.accountId;

      this.focusNext();

    },


    getTargetPath () {
      const
        q = {},
        me = this;

      if (me.trx.trxId) {
        q.trxId = me.trx.trxId;
      }

      return {
        name: me.$options.name,
        query: q
      };
    },

    syncValues (p, q) {
      const me = this;

      if ('trxId' in q && me.core.isInteger(q.trxId)) {
        me.trx.trxId = parseInt(q.trxId);
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
            me.terms = data.terms;
            me.banks = data.banks;
            me.taxes = data.taxes;
            me.ataxes = data.ataxes;
            me.control = data.control;
            me.orgs = me.sym.userInfo.orgs;
            me.platforms = data.platforms;
            me.clusters = data.clusters;
            me.payGroups = data.payGroups;
            me.payees = data.payees;
          }
          if (me.trx.trxId < 0) {
            return Promise.resolve(null);
          }
          return me.getTrx();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.trx.length) {
            me.setModels(info);
          } else {
            if (me.trx.trxId > -1) {
              let message = "Transaction ID '<b>" + me.trx.trxId + "</b>' not found.";
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

            me.trx.trxDate = me.today;
       
            me.trx.trxStatusId = 1;             // Posted
            me.trx.postUserId = me.sym.userInfo.userId;
            // me.aps.trxId = -1
            if (me.isAutoDocSequence) {
              me.trx.documentId = '< NEW >';
            }
          }
          // me.setupControls();
          if (me.isCancelled) {
            me.setupCancelledState();
          } else {
            me.setupControls();
          }
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
        trx = info.trx[0];


      me.trx = me.core.convertDates(trx);

      if (me.trx.trxDate === null) {
        me.trx.trxDate = me.core.emptyDateTime();
      }

      me.trxDetails = info.trxDetails;
      // me.trx.payeeId = info.aps[0].payeeId;
      // me.trx.payeeName = info.aps[0].payeeName;
      // me.trx.payableTypeId = info.aps[0].payableTypeId;

      // me.aps = me.core.convertDates(info.aps[0]);

      // if (me.aps.dueDate === null) {
      //   me.aps.dueDate = me.core.emptyDateTime();
      // }

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldTrx = JSON.stringify(me.trx);
      me.oldTrxDetails = JSON.stringify(me.trxDetails);
      // me.oldAps = JSON.stringify(me.aps);
    },

    // API calls

    getTrx () {
      return get('api/trans-pv/' + this.trx.trxId);
    },

    getReferences () {
      const me = this;

      if (me.orgs.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }
      return get('api/references/aps0100');
    },

    getChangeLog () {
      return get('api/trans-pv/' + this.trx.trxId + '/log');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/trans-pv/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/trans-pv/' + this.trx.trxId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        trx = this.trx,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/trans-pv/' + trx.trxId + '/' + trx.lockId, options);
    },

    getApiPayload () {
      const
        me = this,
        trx = {};

      Object.assign(trx, me.trx);
      trx.details = me.trxDetails;
      
      return trx;
    },

    getTemplate (templateId) {
      return get('api/fin-templates/' + templateId);
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('trx', 'trxDetails', 'detail','amount');
      dc.keyField = 'trxId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.isCancelling = false;
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add',
          'btn-template',
          'btn-replace'
        );
      }, 100);
    },

    onTrxIdChanging (e) {
      e.callback = this.trxIdCallback;
    },

    trxIdCallback (e) {
      e.message = "Transaction ID '<b>" + e.proposedValue + "</b>' not found.";

      // return this.isValidEntity('dbo.FinTrx', 'trxId', e.proposedValue).then(
      //   result => {
      //     return result;
      //   }
      // );

      let filter = 'TrxId=' + e.proposedValue; 
      return getCount('dbo.FinTrx', filter).then(
        count => {
          return count > 0;
        },
        () => {
          return false;
        }
      );

    },

    onTrxIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onTrxIdLostFocus () {
      const me = this;

      if (!me.trx.trxId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTrxIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.trx.trxId = data.trxId;
      me.replaceUrl();
      me.loadData();
    },

    onTrxDateChanging (e) {
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

    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) {
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      return getList('dbo.GlsChart', 'AccountId, AccountName', '', filter).then(
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

      const hasInvalidAccount = this.trxDetails.some(item => !item.accountId);

      if (hasInvalidAccount) {
        me.advice.fault('Save attempt failed. Account ID cannot be empty.', { duration: 3 });
        return;
      }      

      if (!me.trx.debitTotal && !me.trx.creditTotal) {
        me.advice.fault('Save attempt failed. Transaction Details cannot be empty.', { duration: 3 } )
        return;
      }

      if (appConfig.balancedGeneralJournal) {
        if ((me.trx.debitTotal || me.trx.creditTotal) && (me.trx.debitTotal !== me.trx.creditTotal)) {
          me.advice.fault('Save attempt failed. Total debits must equal credits.', { duration: 3 } )
          return;
        }
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
      // console.log(this.control)
      
    const match = this.trxDetails.find(
      detail => detail.accountId === this.control[0].payableTradeAccount
    );
    console.log(2)
    const payableTradeAmount = match ? match.creditAmount || 0 : 0;


      me.trx.amount = payableTradeAmount;



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
              me.trx.trxId = success; 
            }

            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'

              if (me.isCancelling) {
                message = "Transaction '" + me.trx.trxId + "' cancelled."
              }
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

      getSafeDeleteFlag('FinTrx', me.trx.trxId)
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
        'accountName',
        'clusterName',
        'clientPayGroupName',
        'payeeName'
      );

      setTimeout(() => {
        this.setFocus('accountId');
      }, 200);
    },

    onHideDetailEditor () {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
      // let index = me.detailIndex;

      // let el = document.getElementById('activity' + index);
      // me.dom.flash(el, 2);
    },

    onEditDetail (dtl, index) {
      // hold current property values of selected detail for editing; passed index too.
      const d = this.detail;
      this.detailIndex = index;

      d.trxDetailId = dtl.trxDetailId;
      d.accountId = dtl.accountId;
      d.accountName = dtl.accountName;
      d.debitAmount = dtl.debitAmount;
      d.creditAmount = dtl.creditAmount;

      d.orgId = dtl.orgId;
      d.orgName = dtl.orgName;
      d.platformId = dtl.platformId;
      d.platformName = dtl.platformName;
      d.clusterId = dtl.clusterId;
      d.clusterName = dtl.clusterName;
      d.clientPayGroupId = dtl.clientPayGroupId;
      d.clientPayGroupName = dtl.clientPayGroupName;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.trxDetailId = -1;

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Account <b>' + me.trxDetails[index].accountName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.trxDetails.splice(index, 1);
            me.refreshTotals();
          }
          return;
        }
      );
    },

    onSelectTemplate () {
      const me = this;

      let options = {
        triggerControl: me,
        filter: '',      // general journal
        sort: '',
        size: '',
        dismissible: true,
        searchValue: ''
      };
      const lookupId = 'FinTemplate';

      me.lookup.show(lookupId, options).then(
        result => {
          if (result && result.length) {

            me.getTemplate(result[0].templateId).then(
              info => {
                if (info) {
                  // load template details
                  let dtl;
                  me.trxDetails = [];

                  info.templateDetails.forEach( o => {
                    dtl = {};
                    dtl.trxDetailId = -1;
                    dtl.trxId = me.trx.trxId;
                    dtl.accountId = o.accountId;
                    dtl.accountName = o.accountName;
                    dtl.debitAmount = o.debitAmount;
                    dtl.creditAmount = o.creditAmount;

                    me.trxDetails.push(dtl);
                  });

                  me.refreshTotals();

                }
              },
              fault => {
                me.showFault(fault);
              }
            );
          }
        },
        fault => {
          let message;
          if (fault.status === 404) {
            message = "Lookup (" + lookupId + ") not found."
          } else {
            message = String(fault.status) +  "Problem loading lookup (" + lookupId + ")."
          }
          me.advice.fault(message);
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
        me.trxDetails.push(dtl);

        me.clearDetailPad();
        me.advice.info("Account '" + dtl.accountName + "' added to list.", { duration: 5 });
        me.setFocus('accountId');
        me.refreshTotals();

        if (me.trx.debitTotal === me.trx.creditTotal) {
          me.isDetailEditorVisible = false;
        }

      } else {
        dtl = me.trxDetails[me.detailIndex];

        dtl.trxDetailId = d.trxDetailId;
        dtl.accountId = d.accountId;
        dtl.accountName = d.accountName;
        dtl.debitAmount = d.debitAmount;
        dtl.creditAmount = d.creditAmount;

        me.orgs.forEach((dtl) => {
          if (dtl.orgId == d.orgId) {
            d.orgName = dtl.orgName;
          }
        });

        me.platforms.forEach((dtl) => {
          if (dtl.platformId == d.platformId) {
            d.platformName = dtl.platformName;
          }
        });

        me.clusters.forEach((dtl) => {
          if (dtl.clusterId == d.clusterId) {
            d.clusterName = dtl.clusterName;
          }
        });

        me.payGroups.forEach((dtl) => {
          if (dtl.clientPayGroupId == d.clientPayGroupId) {
            d.clientPayGroupName = dtl.clientPayGroupName;
          }
        });

        me.payees.forEach((dtl) => {
          if (dtl.payeeId == d.payeeId) {
            d.payeeName = dtl.payeeName;
          }
        });




        dtl.orgId = d.orgId;
        dtl.orgName = d.orgName;
        dtl.platformId = d.platformId;
        dtl.platformName = d.platformName;
        dtl.clusterId = d.clusterId;
        dtl.clusterName = d.clusterName;
        dtl.clientPayGroupId = d.clientPayGroupId;
        dtl.clientPayGroupName = d.clientPayGroupName;
        dtl.payeeId = d.payeeId;
        dtl.payeeName = d.payeeName;


        me.isDetailEditorVisible = false;
        me.refreshTotals();
      }
   
    },

    onShowAmountEditor () {
      const me = this;

      me.setActiveModel('amount');

      me.setRequiredMode(
        'oldValue',
        'newValue'
      );
      
      me.amount.oldValue = 1;

      setTimeout(() => {
        this.setFocus('oldValue');
      }, 200);
    },

    onHideAmountEditor () {
      const me = this;

      me.setActiveModel();
    },

    onSubmitAmount () {
      const me = this;

      if (me.amount.oldValue === me.amount.newValue) {
        return;
      }

      me.trxDetails.forEach( o => {
        if (o.debitAmount === me.amount.oldValue) {
          o.debitAmount = me.amount.newValue;
        }
        if (o.creditAmount === me.amount.oldValue) {
          o.creditAmount = me.amount.newValue;
        }
      });

      me.refreshTotals();
      me.isAmountEditorVisible = false;

    },  

    onViewLog () {
      const
        me = this,
        wait = me.wait();

      me.getChangeLog().then(
        (logs) => {
          me.stopWait(wait);
          me.logs = logs;
          me.isLogVisible = true;
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );

    },

    onPrint () {
      const me = this;

      if (me.hasChanges()) {
        me.advice.fault('Cannot print with uncommitted changes.', { duration: 3 } )
        return;
      }

      window.print();
    },

    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to <b>cancel</b> transaction.<br>Once cancelled, transaction cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.trx.trxStatusId = 9;
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onEditDocumentId () {
      const me = this;

      me.setRequiredMode(
        'documentId'
      );  

      me.setFocus('documentId');
    },

    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add',
          'btn-template',
          'btn-replace'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'trxDate',
          // 'fundClusterId',
          // 'costCenterId',
          'particulars',
          // 'requestSignatoryId'
        );

        me.setDisplayMode(
          'requestSignatoryName',
          'taxPercentage',
          'aTaxPercentage',
          'payeeName',
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

        me.setFocus('trxDate');
      }, 100);

    },

    setupCancelledState () {
      const me = this;

      setTimeout(() => {
        me.setDefaultControlStates();

        me.disableElement(
          'btn-save',
          'btn-delete',
          'btn-dtl-edit',
          'btn-dtl-delete',
          'btn-add',
          'btn-template',
          'btn-replace'
        );

        me.setDisplayMode(
          'fundClusterId',
          'documentId',
          'costCenterId',
          'particulars',
          'requestSignatoryId',
          'requestSignatoryName'
        );

        me.setFocus('trxDate');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.trx) !== me.oldTrx) { return true; }
      if (JSON.stringify(me.trxDetails) !== me.oldTrxDetails) { return true; }


      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.trxDetailId = 0;
      d.accountId = '';
      d.accountName = '';
      d.debitAmount = 0;
      d.creditAmount = 0;
      d.orgId = 0;
      d.orgName = '';
      d.platformId = 0;
      d.platformName = '';
      d.clusterId = 0;
      d.clusterName = '';
      d.clientPayGroupId = 0;
      d.clientPayGroupName = '';


      
    },

    refreshTotals () {
      const me = this;

      me.trx.debitTotal = 0;
      me.trx.creditTotal = 0;

      me.trxDetails.forEach( dtl => {

        me.trx.debitTotal = me.trx.debitTotal + dtl.debitAmount;
        me.trx.creditTotal = me.trx.creditTotal + dtl.creditAmount;
      });

      // me.trx.debitTotal = me.trx.debitTotal.toFixed(2) / 1;
      // me.trx.creditTotal = me.trx.creditTotal.toFixed(2) / 1;

      me.trx.debitTotal = me.core.toDecimal(me.trx.debitTotal);
      me.trx.creditTotal = me.core.toDecimal(me.trx.creditTotal);

    }

  },
  
  created () {
    const me = this;

    me.oldTrx = '';
    me.oldTrxDetails = '';
    me.oldAps = '';
    me.types = [];     
    me.terms = [];     
    me.banks = [];     
    me.taxes = [];     
    me.ataxes = [];         
    me.control = [];         
    me.isCancelling = false;
    me.isFundClusterVisible = appConfig.fundClusterPrompt;
    me.isCostCenterVisible = appConfig.costCenterPrompt;
    me.isAutoDocSequence = appConfig.autoDocSequence;

    me.orgs = [];     // all transaction types (cache)
    me.platforms = [];
    me.clusters = [];
    me.payGroups = [];

    me.today = me.sym.dateInfo.serverDate;
    me.payees = [];

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

#logs tr {
  vertical-align: top;
}

#logs td {
  font-size: .875rem;
  padding: .375rem;
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

.detail-editor-boxes {
  display: grid;
  /* grid-auto-flow: column; */
  grid-template-rows: .5fr;
  /* gap: .5rem; */
}

#detail-editor >>> .modal-lg {
  min-width: 85%;
}

.amount-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.to-print table {
  border: 2px solid dimgray !important;
  line-height: 1.0;
}

.to-print .structure td {
  border: 1px solid red !important;
  padding: 0;
}

.to-print .doc-header td {
  border-top: none !important;
  border-bottom: none !important;
  padding-bottom: .25rem;
}

.to-print .doc-header td.doc-title {
  line-height: 1;
}

.to-print .col-header td {
  border: 1px solid dimgray !important;
}

.to-print .spacer {
  padding: 1rem;
}
.act-btn{
  display: flex;
  justify-content: space-evenly;
}
.edit-btn{
  display: flex;
  justify-content: center;
  align-items: center;
}

.tb-container{
  overflow: auto;
  max-height: 50vh;
}
.tb-scroller{
  /* max-width: 110vw; */
  width: 150vw;
}
.acc-container{
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr;
}
.lookup-container-1{
  display: grid;
  grid-template-columns: .5fr 3fr;
}
.lookup-container-2{
  display: grid;
  grid-template-columns: .5fr 2fr;
}
.lookup-container-3{
  display: grid;
  grid-template-columns: .5fr 3fr;
}
.coop-container{
  display: grid;
  grid-template-rows: .5fr .5fr;
}
.tax-container{
  display: flex;
 flex-direction: row  ;
}
.box-container{
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}
.box-1{
  display: flex;
  flex-direction: row;
}
.box-3{
  display: flex;
  flex-direction: row;
}
/* .signatory-container{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
} */
@page {
  margin: .25in;
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

.app-bold {
  font-family: 'Heebo Bold';
}

</style>