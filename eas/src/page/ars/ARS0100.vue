// Client Billing

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="ars0100" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="timekeeping.timekeepingBillingId" :caption-width="35" caption="Billing ID" lookupId="PayTimekeepingBilling" @lostfocus="onTimekeepingBillingIdLostFocus" @changing="onTimekeepingBillingIdChanging" @changed="onTimekeepingBillingIdChanged" @searchresult="onTimekeepingBillingIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
            <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCanceled">Cancelled</sym-tag>
            <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isPosted">Posted</sym-tag>
            <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isSubmitted">Submitted</sym-tag>

        </div>
          

      </div>
        <div class="box-field">

           
      <div class="app-box-style">
              <div class="box-1 gap">
                <sym-int v-model="timekeeping.timekeepingPayOutId" :caption-width="40" align="left" caption="Pay Out ID" lookupId="PayTimekeepingPayOut" @lostfocus="onTimekeepingPayOutIdLostFocus" @changing="onTimekeepingPayOutIdChanging" @changed="onTimekeepingPayOutIdChanged" @searchresult="onTimekeepingPayOutIdSearchResult"></sym-int>

             <button :disabled ="!timekeeping.timekeepingPayOutId"  type="button" :class="logButtonClass" class="secondary w-100 mb-2" @click="onClickTimekeepingPayOut"> 
              <i class="fa fa-check"></i><span class="bold"></span> 
            </button>

                <sym-int v-model="timekeeping.clientPayGroupId" align="left" :caption-width="50" caption="Pay Group ID" ></sym-int>

                <button :disabled ="!timekeeping.clientPayGroupId" type="button" :class="logButtonClass" class="secondary w-100 mb-2" @click="onClickPayGroup"> 
                  <i class="fa fa-check"></i><span class="bold"></span> 
                </button>
                 <sym-text v-model="timekeeping.clientPayGroupName" align="left" :caption-width="40" caption="Pay Group Name" ></sym-text>

                <sym-date v-model="timekeeping.cutOffStartDate" align="left" :caption-width="40" :input-width="30" caption="Cut Off Start Date" ></sym-date>
                <sym-date v-model="timekeeping.cutOffEndDate" align="left" :caption-width="35" :input-width="30" caption="Cut Off End Date" ></sym-date>


                  </div>
              <div class="box-2 gap">

                <sym-date v-model="timekeeping.payOutDate" align="left" :caption-width="30" :input-width="30" caption="Pay Date" ></sym-date>      
                <sym-text v-model="timekeeping.remarks" align="left" :caption-width="50" caption="Remarks" ></sym-text>
         
            </div>
            
              
             <button :disabled ="isCanceled || isPosted || !timekeeping.timekeepingPayOutId" type="button" :class="logButtonClass" class="applicant-btn w-10 info" @click="onProcess"> 
              <i class="fa fa-calculator"></i><span class="bold">Calculate Billing</span> 
            </button>

          <div>

        </div>

        </div>


      <div class="btn-container">
          <div class="post-btn "  :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
            <button type="button" :class="deleteButtonClass" class="justify-between btn-cancel white" @click="onPost" :disabled="isPosted || isCanceled"><i class="fa fa-check fa-lg"></i><span class="bold">Post</span></button>
         
          </div>
          <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
            <!-- <button type="button" :class="deleteButtonClass" class="justify-between btn-delete " @click="onPost" :disabled="isCanceled && isPosted"><i class="fa fa-check fa-lg"></i><span class="bold">Post</span></button> -->
            <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit" > <i class="fa fa-save fa-lg"></i><span>Save</span> </button>
            <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear" > <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
            <!-- <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="onDelete" > <i class="fa fa-times-circle fa-lg"></i><span>Delete</span> </button> -->
            <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(1)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
            <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
          </div>
          <div class="cancel-btn">            
            <button type="button" :class="deleteButtonClass" class=" justify-between btn-cancel white  w-120" @click="onCancel" :disabled="isCanceled || isPosted"><i class="fa fa-times-circle fa-lg "></i><span class="bold">CANCEL</span></button>
          </div>
        
      </div>




              <div class="app-box-style">
        <div class="text-center border-light curved p-1 info mt-2">
          <span class="serif lg-3">Total Member/s:  {{ schedules.length}}</span>
        </div>
      
          <div class="d-flex align-center justify-between mb-2">
            <sym-text v-model="searchMemberNameInput" align="left" :caption-width="32" :input-width="100" caption=" Name" list="memberNames" @changed="applyMemberNameSearch"></sym-text>
            <datalist id="memberNames"><option v-for="item in memberList" :key="item.memberName"  :value="item.memberName"  @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist>
            <button type="button" class="success mb-3 w-10 text-center lg-2 p-1 shadow-soft" @click="tableToExcel('table', 'loremTable')"><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>
          </div>

<!-- Timekeeping Table -->
 <div class="main-scroll-wrapper " >
<div class="table-scroller " v-show="timekeeping.timekeepingPayOutId != 0">
  <div class="d-flex justify-centerXXX fixed-header ">
    <table class="striped-even mb-0 scroller info-light" ref="table" id="loremTable">
      <thead >
          <tr>
            <!-- <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th> -->
            <th class="text-center" style="width: 60px;"></th>
            <th class="text-center" style="width: 100px;"></th>
            <th class="text-center" style="width: 400px;"></th>
            <th class="text-center" style="width: 300px;"></th>
            <!-- <th colspan="1"></th> -->
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="6">REGULAR DAY</th>
            <th class= "text-center" colspan="1">TOTAL BASIC</th>
            <th class= "text-center" colspan="4">REGULAR DAY</th>
            <th class= "text-center" colspan="9">REST DAY</th>
            <th class= "text-center" colspan="17">SPECIAL HOLIDAY</th>
            <th class= "text-center" colspan="19">LEGAL HOLIDAY</th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center"  colspan="1"></th>
          
          </tr>

          <tr>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="1"></th>
            <th class= "text-center" colspan="4">Basic Pay</th>
            <th class= "text-center" colspan="2">REG ND HRS</th>
            <th colspan="1"></th>
            <th class= "text-center" colspan="2">REGULAR OVERTIME</th>
            <th class= "text-center" colspan="2">REG. NIGHT DIFF OT</th>
            <th class= "text-center" colspan="3">RESTDAY (100%)</th>
            <th class= "text-center" colspan="2">RESTDAY EXTENDED (169%)</th>
            <th class= "text-center" colspan="2">RD ND HRS</th>
            <th class= "text-center" colspan="2">RD ND OT</th>
            <th class= "text-center" colspan="3">SPHOLIDAY (130%)</th>
            <th class= "text-center" colspan="2">SPHOLIDAY EXTENDED (169%)</th>
           <th class= "text-center" colspan="2">SPECIAL HOLIDAY ND HRS</th>
           <th class= "text-center" colspan="2">SPECIAL HOLIDAY ND OT HRS</th>
           <th class= "text-center" colspan="2">SPECIAL HOL RESTDAY</th>
           <th class= "text-center" colspan="2">SPECIAL HOL RESTDAY EXTENDED HRS (195%)</th>
           <th class= "text-center" colspan="2">SPECIAL HOL RESTDAY ND HRS ()</th>
           <th class= "text-center" colspan="2">SPECIAL HOL RESTDAY ND OT HRS ()</th>
           <th class= "text-center" colspan="2">LEGAL HOLIDAY (100%) w/o AF UNWORKED</th>
           <th class= "text-center" colspan="3">LEGAL HOLIDAY 200% (worked) w/ AF</th>
           <th class= "text-center" colspan="2">LEGAL HOLIDAY EW (260%)</th>
           <th class= "text-center" colspan="2">LEGAL HOL ND HRS</th>
           <th class= "text-center" colspan="2">LEGAL HOLIDAY ND OT</th>
           <th class= "text-center" colspan="2">LEGAL HOL RESTDAY</th>
           <th class= "text-center" colspan="2">LEGAL HOL RESTDAY OT</th>
           <th class= "text-center" colspan="2">LEGAL HOL RESTDAY ND HRS</th>
           <th class= "text-center" colspan="2">LEGAL HOL RESTDAY ND OT HRS</th>
           <th class= "text-center" colspan="1">ALLOWANCE</th>
           <th class= "text-center" colspan="1"></th>
           <th class= "text-center" colspan="1"></th>
           <th class= "text-center" colspan="1"></th>
           <th class= "text-center" colspan="1"></th>
           <th class= "text-center" colspan="1"></th>
           <th class= "text-center" colspan="1">Admin Fee</th>
           <th class= "text-center" colspan="1"></th>
          </tr>

          <tr>
            <th class="text-center" style="width: 60px;">#</th>
            <th colspan="1">Member ID</th>
            <th colspan="1">Member Name</th>
            <th class= "text-center" colspan="1">POSITION</th>
            <th class= "text-center" colspan="1">ALLOWANCE</th>
            <th class= "text-center" colspan="1">Rate / Day</th>
            <th class= "text-center" colspan="1">Days</th>
            <th class= "text-center" colspan="1">Hrs</th>
            <th class= "text-center" colspan="1">Amount</th>
            <th class= "text-center" colspan="1">Hrs</th>
            <th class= "text-center" colspan="1">Amount</th>
            <th class= "text-center" colspan="1">TOTAL BASIC</th>
            <th class= "text-center" colspan="1">Hrs</th>
            <th class= "text-center" colspan="1">Amount</th>
            <th class= "text-center" colspan="1">Hrs</th>
            <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount 100% w/AF</th>
           <th class= "text-center" colspan="1">Amount 30% w/o AF</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount (100%)</th>
           <th class= "text-center" colspan="1">Amount (30%)</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount w/o AF</th>
           <th class= "text-center" colspan="1">Amount w/ AF</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Hrs</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">Amount</th>
           <th class= "text-center" colspan="1">EXTENDED WORK TOTAL</th>
           <th class= "text-center" colspan="1">GROSS PAY</th>
           <th class= "text-center" colspan="1">YEI</th>
           <th class= "text-center" colspan="1">TOTAL GROSS PAY</th>
           <th class= "text-center" colspan="1">TOTAL GROSS PAY (w/ %AF)</th>
           <th class= "text-center" colspan="1">10%</th>
           <th class= "text-center" colspan="1">NET BILLING</th>
                                                                                      
        </tr>
      </thead>
      <tbody class="white">
        <!-- <tr v-for="(dtl, index) in schedules" :key="index"> -->
          <tr v-for="(dtl, index) in filteredSchedules" :key="index"  :class="{ 'running-highlight': index === currentHighlightIndex }">
          <td class="text-center">{{ index + 1 }}</td>

          <td>{{ dtl.memberId }}</td>
          <td>{{ dtl.memberName }}</td>        
          <td>{{ dtl.departmentName }}</td>        
          <td class="text-right">{{ dtl.allowanceDaily === 0 ? '' : core.toDecimalFormat(dtl.allowanceDaily)  }}</td>
          <td class="text-right">{{ dtl.payingDailyRate === 0 ? '' : core.toDecimalFormat(dtl.payingDailyRate)  }}</td>
          <td class="text-right">{{ dtl.totalWorkingDay === 0 ? '' : core.toDecimalFormat(dtl.totalWorkingDay)  }}</td>
          <td class="text-right">{{ dtl.totalWorkingHour === 0 ? '' : core.toDecimalFormat(dtl.totalWorkingHour)  }}</td>          
          <td class="text-right">{{ dtl.basicPayOutRate === 0 ? '' : core.toDecimalFormat(dtl.basicPayOutRate)  }}</td>

           <!-- Night Differential	 --> 
          <td class="text-right">{{ dtl.ndHour === 0 ? '' : core.toDecimalFormat(dtl.ndHour)  }}</td>
          <td class="text-right">{{ dtl.ndRate === 0 ? '' : core.toDecimalFormat(dtl.ndRate)  }}</td>
                   
          <!-- SHOULD BE totalBasic   -->
          <td class="text-right">{{ dtl.totalBasic === 0 ? '' : core.toDecimalFormat(dtl.totalBasic)  }}</td>
 
           <!-- Regular Overtime   -->
          <td class="text-right">{{ dtl.otHour === 0 ? '' : core.toDecimalFormat(dtl.otHour)  }}</td>
          <td class="text-right">{{ dtl.otRate === 0 ? '' : core.toDecimalFormat(dtl.otRate)  }}</td>

           <!-- Regular ND Overtime   -->
          <td class="text-right">{{ dtl.ndotHour === 0 ? '' : core.toDecimalFormat(dtl.ndotHour)  }}</td>
          <td class="text-right">{{ dtl.ndotRate === 0 ? '' : core.toDecimalFormat(dtl.ndotRate)  }}</td>

          <!-- RestDay  -->
          <td class="text-right">{{ dtl.rdHour === 0 ? '' : core.toDecimalFormat(dtl.rdHour)  }}</td>
          <td class="text-right">{{ dtl.rdRate === 0 ? '' : core.toDecimalFormat(dtl.rdRate)  }}</td>
          <td class="text-right">{{ dtl.rdPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.rdPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.rdAmount === 0 ? '' : core.toDecimalFormat(dtl.rdAmount)  }}</td> -->
          <td class="text-right">{{ dtl.rdotHour === 0 ? '' : core.toDecimalFormat(dtl.rdotHour)  }}</td>
          <td class="text-right">{{ dtl.rdotRate === 0 ? '' : core.toDecimalFormat(dtl.rdotRate)  }}</td>

          <td class="text-right">{{ dtl.rdndHour === 0 ? '' : core.toDecimalFormat(dtl.rdndHour)  }}</td>
          <td class="text-right">{{ dtl.rdndRate === 0 ? '' : core.toDecimalFormat(dtl.rdndRate)  }}</td>

         <td class="text-right">{{ dtl.rdndotHour === 0 ? '' : core.toDecimalFormat(dtl.rdndotHour)  }}</td>
          <td class="text-right">{{ dtl.rdndotRate === 0 ? '' : core.toDecimalFormat(dtl.rdndotRate)  }}</td>


          <!-- Special Holiday	  -->

          <td class="text-right">{{ dtl.shHour === 0 ? '' : core.toDecimalFormat(dtl.shHour)  }}</td>
          <td class="text-right">{{ dtl.shRate === 0 ? '' : core.toDecimalFormat(dtl.shRate)  }}</td>
          <td class="text-right">{{ dtl.shPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.shPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.shAmount === 0 ? '' : core.toDecimalFormat(dtl.shAmount)  }}</td> -->

          <td class="text-right">{{ dtl.shotHour === 0 ? '' : core.toDecimalFormat(dtl.shotHour)  }}</td>
          <td class="text-right">{{ dtl.shotRate === 0 ? '' : core.toDecimalFormat(dtl.shotRate)  }}</td>

          <td class="text-right">{{ dtl.shndHour === 0 ? '' : core.toDecimalFormat(dtl.shndHour)  }}</td>
          <td class="text-right">{{ dtl.shndRate === 0 ? '' : core.toDecimalFormat(dtl.shndRate)  }}</td>

          <td class="text-right">{{ dtl.shndotHour === 0 ? '' : core.toDecimalFormat(dtl.shndotHour)  }}</td>
          <td class="text-right">{{ dtl.shndotRate === 0 ? '' : core.toDecimalFormat(dtl.shndotRate)  }}</td>

          <td class="text-right">{{ dtl.shrdHour === 0 ? '' : core.toDecimalFormat(dtl.shrdHour)  }}</td>
          <td class="text-right">{{ dtl.shrdRate === 0 ? '' : core.toDecimalFormat(dtl.shrdRate)  }}</td>

          <td class="text-right">{{ dtl.shrdotHour === 0 ? '' : core.toDecimalFormat(dtl.shrdotHour)  }}</td>
          <td class="text-right">{{ dtl.shrdotRate === 0 ? '' : core.toDecimalFormat(dtl.shrdotRate)  }}</td>

          <td class="text-right">{{ dtl.shrdndHour === 0 ? '' : core.toDecimalFormat(dtl.shrdndHour)  }}</td>
          <td class="text-right">{{ dtl.shrdndRate === 0 ? '' : core.toDecimalFormat(dtl.shrdndRate)  }}</td>

          <td class="text-right">{{ dtl.shrdndotHour === 0 ? '' : core.toDecimalFormat(dtl.shrdndotHour)  }}</td>
          <td class="text-right">{{ dtl.shrdndotRate === 0 ? '' : core.toDecimalFormat(dtl.shrdndotRate)  }}</td>


          <!-- Legal Holiday Unworked -->

          <td class="text-right">{{ dtl.hu100Hour === 0 ? '' : core.toDecimalFormat(dtl.hu100Hour)  }}</td>
          <td class="text-right">{{ dtl.hu100Rate === 0 ? '' : core.toDecimalFormat(dtl.hu100Rate)  }}</td>

          <!-- Legal Holiday  -->
 
          <td class="text-right">{{ dtl.lhHour === 0 ? '' : core.toDecimalFormat(dtl.lhHour)  }}</td>
          <td class="text-right">{{ dtl.lhRate === 0 ? '' : core.toDecimalFormat(dtl.lhRate)  }}</td>
          <td class="text-right">{{ dtl.lhPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.lhPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.lhAmount === 0 ? '' : core.toDecimalFormat(dtl.lhAmount)  }}</td> -->
          <td class="text-right">{{ dtl.lhotHour === 0 ? '' : core.toDecimalFormat(dtl.lhotHour)  }}</td>
          <td class="text-right">{{ dtl.lhotRate === 0 ? '' : core.toDecimalFormat(dtl.lhotRate)  }}</td>

          <td class="text-right">{{ dtl.lhndHour === 0 ? '' : core.toDecimalFormat(dtl.lhndHour)  }}</td>
          <td class="text-right">{{ dtl.lhndRate === 0 ? '' : core.toDecimalFormat(dtl.lhndRate)  }}</td>

          <td class="text-right">{{ dtl.lhndotHour === 0 ? '' : core.toDecimalFormat(dtl.lhndotHour)  }}</td>
          <td class="text-right">{{ dtl.lhndotRate === 0 ? '' : core.toDecimalFormat(dtl.lhndotRate)  }}</td>

          <td class="text-right">{{ dtl.lhrdHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdRate)  }}</td>

          <td class="text-right">{{ dtl.lhrdotHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdotHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdotRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdotRate)  }}</td>

          <td class="text-right">{{ dtl.lhrdndHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdndHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdndRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdndRate)  }}</td>

          <td class="text-right">{{ dtl.lhrdndotHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdndotHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdndotRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdndotRate)  }}</td>

          <!-- Other Allowance -->  
          <td class="text-right">{{ dtl.otherAllowance === 0 ? '' : core.toDecimalFormat(dtl.otherAllowance)  }}</td>

          <!-- Extended Work -->
          <td class="text-right">{{ dtl.extendedWorkRate === 0 ? '' : core.toDecimalFormat(dtl.extendedWorkRate)  }}</td>

          <!-- Gross Pay -->
          <td class="text-right">{{ dtl.grossPay === 0 ? '' : core.toDecimalFormat(dtl.grossPay)  }}</td>
         
          <!-- YEI -->
          <td class="text-right">{{ dtl.yei === 0 ? '' : core.toDecimalFormat(dtl.yei)  }}</td>

          <!-- Total Gross Pay -->
          <td class="text-right">{{ dtl.totalGrossPay === 0 ? '' : core.toDecimalFormat(dtl.totalGrossPay)  }}</td>

          <!-- Total Gross Pay w % AF-->
          <td class="text-right">{{ dtl.totalGrossPayAF === 0 ? '' : core.toDecimalFormat(dtl.totalGrossPayAF)  }}</td>

          <!-- AF-->
          <td class="text-right">{{ dtl.adminFee === 0 ? '' : core.toDecimalFormat(dtl.adminFee)  }}</td>

          <!-- Net Billing-->
          <td class="text-right">{{ dtl.netBilling === 0 ? '' : core.toDecimalFormat(dtl.netBilling)  }}</td>

        </tr>
      </tbody> 
      
<tfoot>
  <tr>
    <td colspan="4" style="text-align:right;"><strong>Total:</strong></td>
    <!-- <td>{{ totals.totalWorkingDay.toFixed(2) }}</td> -->
          <td class="text-right">{{ totals.allowanceDaily === 0 ? '' : core.toDecimalFormat(totals.allowanceDaily)  }}</td>
          <td class="text-right">{{ totals.payingDailyRate === 0 ? '' : core.toDecimalFormat(totals.payingDailyRate)  }}</td>
          <td class="text-right">{{ totals.totalWorkingDay === 0 ? '' : core.toDecimalFormat(totals.totalWorkingDay)  }}</td>
          <td class="text-right">{{ totals.totalWorkingHour === 0 ? '' : core.toDecimalFormat(totals.totalWorkingHour)  }}</td>          
          <td class="text-right">{{ totals.basicPayOutRate === 0 ? '' : core.toDecimalFormat(totals.basicPayOutRate)  }}</td>

           <!-- Night Differential	 --> 
          <td class="text-right">{{ totals.ndHour === 0 ? '' : core.toDecimalFormat(totals.ndHour)  }}</td>
          <td class="text-right">{{ totals.ndRate === 0 ? '' : core.toDecimalFormat(totals.ndRate)  }}</td>
                   
          <!-- SHOULD BE totalBasic   -->
          <td class="text-right">{{ totals.totalBasic === 0 ? '' : core.toDecimalFormat(totals.totalBasic)  }}</td>
 
           <!-- Regular Overtime   -->
          <td class="text-right">{{ totals.otHour === 0 ? '' : core.toDecimalFormat(totals.otHour)  }}</td>
          <td class="text-right">{{ totals.otRate === 0 ? '' : core.toDecimalFormat(totals.otRate)  }}</td>

           <!-- Regular ND Overtime   -->
          <td class="text-right">{{ totals.ndotHour === 0 ? '' : core.toDecimalFormat(totals.ndotHour)  }}</td>
          <td class="text-right">{{ totals.ndotRate === 0 ? '' : core.toDecimalFormat(totals.ndotRate)  }}</td>

          <!-- RestDay  -->
          <td class="text-right">{{ totals.rdHour === 0 ? '' : core.toDecimalFormat(totals.rdHour)  }}</td>
          <td class="text-right">{{ totals.rdRate === 0 ? '' : core.toDecimalFormat(totals.rdRate)  }}</td>
          <td class="text-right">{{ totals.rdPremiumRate === 0 ? '' : core.toDecimalFormat(totals.rdPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ totals.rdAmount === 0 ? '' : core.toDecimalFormat(totals.rdAmount)  }}</td> -->
          <td class="text-right">{{ totals.rdotHour === 0 ? '' : core.toDecimalFormat(totals.rdotHour)  }}</td>
          <td class="text-right">{{ totals.rdotRate === 0 ? '' : core.toDecimalFormat(totals.rdotRate)  }}</td>

          <td class="text-right">{{ totals.rdndHour === 0 ? '' : core.toDecimalFormat(totals.rdndHour)  }}</td>
          <td class="text-right">{{ totals.rdndRate === 0 ? '' : core.toDecimalFormat(totals.rdndRate)  }}</td>

         <td class="text-right">{{ totals.rdndotHour === 0 ? '' : core.toDecimalFormat(totals.rdndotHour)  }}</td>
          <td class="text-right">{{ totals.rdndotRate === 0 ? '' : core.toDecimalFormat(totals.rdndotRate)  }}</td>


          <!-- Special Holiday	  -->

          <td class="text-right">{{ totals.shHour === 0 ? '' : core.toDecimalFormat(totals.shHour)  }}</td>
          <td class="text-right">{{ totals.shRate === 0 ? '' : core.toDecimalFormat(totals.shRate)  }}</td>
          <td class="text-right">{{ totals.shPremiumRate === 0 ? '' : core.toDecimalFormat(totals.shPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ totals.shAmount === 0 ? '' : core.toDecimalFormat(totals.shAmount)  }}</td> -->

          <td class="text-right">{{ totals.shotHour === 0 ? '' : core.toDecimalFormat(totals.shotHour)  }}</td>
          <td class="text-right">{{ totals.shotRate === 0 ? '' : core.toDecimalFormat(totals.shotRate)  }}</td>

          <td class="text-right">{{ totals.shndHour === 0 ? '' : core.toDecimalFormat(totals.shndHour)  }}</td>
          <td class="text-right">{{ totals.shndRate === 0 ? '' : core.toDecimalFormat(totals.shndRate)  }}</td>

          <td class="text-right">{{ totals.shndotHour === 0 ? '' : core.toDecimalFormat(totals.shndotHour)  }}</td>
          <td class="text-right">{{ totals.shndotRate === 0 ? '' : core.toDecimalFormat(totals.shndotRate)  }}</td>

          <td class="text-right">{{ totals.shrdHour === 0 ? '' : core.toDecimalFormat(totals.shrdHour)  }}</td>
          <td class="text-right">{{ totals.shrdRate === 0 ? '' : core.toDecimalFormat(totals.shrdRate)  }}</td>

          <td class="text-right">{{ totals.shrdotHour === 0 ? '' : core.toDecimalFormat(totals.shrdotHour)  }}</td>
          <td class="text-right">{{ totals.shrdotRate === 0 ? '' : core.toDecimalFormat(totals.shrdotRate)  }}</td>

          <td class="text-right">{{ totals.shrdndHour === 0 ? '' : core.toDecimalFormat(totals.shrdndHour)  }}</td>
          <td class="text-right">{{ totals.shrdndRate === 0 ? '' : core.toDecimalFormat(totals.shrdndRate)  }}</td>

          <td class="text-right">{{ totals.shrdndotHour === 0 ? '' : core.toDecimalFormat(totals.shrdndotHour)  }}</td>
          <td class="text-right">{{ totals.shrdndotRate === 0 ? '' : core.toDecimalFormat(totals.shrdndotRate)  }}</td>


          <!-- Legal Holiday Unworked -->

          <td class="text-right">{{ totals.hu100Hour === 0 ? '' : core.toDecimalFormat(totals.hu100Hour)  }}</td>
          <td class="text-right">{{ totals.hu100Rate === 0 ? '' : core.toDecimalFormat(totals.hu100Rate)  }}</td>

          <!-- Legal Holiday  -->
 
          <td class="text-right">{{ totals.lhHour === 0 ? '' : core.toDecimalFormat(totals.lhHour)  }}</td>
          <td class="text-right">{{ totals.lhRate === 0 ? '' : core.toDecimalFormat(totals.lhRate)  }}</td>
          <td class="text-right">{{ totals.lhPremiumRate === 0 ? '' : core.toDecimalFormat(totals.lhPremiumRate)  }}</td>
          <!-- <td class="text-right">{{ totals.lhAmount === 0 ? '' : core.toDecimalFormat(totals.lhAmount)  }}</td> -->
          <td class="text-right">{{ totals.lhotHour === 0 ? '' : core.toDecimalFormat(totals.lhotHour)  }}</td>
          <td class="text-right">{{ totals.lhotRate === 0 ? '' : core.toDecimalFormat(totals.lhotRate)  }}</td>

          <td class="text-right">{{ totals.lhndHour === 0 ? '' : core.toDecimalFormat(totals.lhndHour)  }}</td>
          <td class="text-right">{{ totals.lhndRate === 0 ? '' : core.toDecimalFormat(totals.lhndRate)  }}</td>

          <td class="text-right">{{ totals.lhndotHour === 0 ? '' : core.toDecimalFormat(totals.lhndotHour)  }}</td>
          <td class="text-right">{{ totals.lhndotRate === 0 ? '' : core.toDecimalFormat(totals.lhndotRate)  }}</td>

          <td class="text-right">{{ totals.lhrdHour === 0 ? '' : core.toDecimalFormat(totals.lhrdHour)  }}</td>
          <td class="text-right">{{ totals.lhrdRate === 0 ? '' : core.toDecimalFormat(totals.lhrdRate)  }}</td>

          <td class="text-right">{{ totals.lhrdotHour === 0 ? '' : core.toDecimalFormat(totals.lhrdotHour)  }}</td>
          <td class="text-right">{{ totals.lhrdotRate === 0 ? '' : core.toDecimalFormat(totals.lhrdotRate)  }}</td>

          <td class="text-right">{{ totals.lhrdndHour === 0 ? '' : core.toDecimalFormat(totals.lhrdndHour)  }}</td>
          <td class="text-right">{{ totals.lhrdndRate === 0 ? '' : core.toDecimalFormat(totals.lhrdndRate)  }}</td>

          <td class="text-right">{{ totals.lhrdndotHour === 0 ? '' : core.toDecimalFormat(totals.lhrdndotHour)  }}</td>
          <td class="text-right">{{ totals.lhrdndotRate === 0 ? '' : core.toDecimalFormat(totals.lhrdndotRate)  }}</td>

          <!-- Other Allowance -->  
          <td class="text-right">{{ totals.otherAllowance === 0 ? '' : core.toDecimalFormat(totals.otherAllowance)  }}</td>

          <!-- Extended Work -->
          <td class="text-right">{{ totals.extendedWorkRate === 0 ? '' : core.toDecimalFormat(totals.extendedWorkRate)  }}</td>

          <!-- Gross Pay -->
          <td class="text-right">{{ totals.grossPay === 0 ? '' : core.toDecimalFormat(totals.grossPay)  }}</td>
         
          <!-- YEI -->
          <td class="text-right">{{ totals.yei === 0 ? '' : core.toDecimalFormat(totals.yei)  }}</td>

          <!-- Total Gross Pay -->
          <td class="text-right">{{ totals.totalGrossPay === 0 ? '' : core.toDecimalFormat(totals.totalGrossPay)  }}</td>

          <!-- Total Gross Pay w % AF-->
          <td class="text-right">{{ totals.totalGrossPayAF === 0 ? '' : core.toDecimalFormat(totals.totalGrossPayAF)  }}</td>

          <!-- AF-->
          <td class="text-right">{{ totals.adminFee === 0 ? '' : core.toDecimalFormat(totals.adminFee)  }}</td>

          <!-- Net Billing-->
          <td class="text-right">{{ totals.netBilling === 0 ? '' : core.toDecimalFormat(totals.netBilling)  }}</td>



  </tr>
</tfoot>

    </table> 
  </div>  
</div>
</div>
      
      </div>
       
     
      </div>




    </sym-form>




</section>
</template>

<script>

import * as XLSX from 'xlsx';


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
import { thresholdScott } from 'd3';

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "ars0100",

  data() {
    return {
      
      timekeeping: {
        timekeepingBillingId: 0,
        timekeepingPayOutId: 0,
        timekeepingBillingStatusId: 0,
        clientPayGroupId:0,
        clientPayGroupName:'',
        remarks:'',
        cutOffStartDate: null,
        cutOffEndDate: null,
        billingDate: null,
        remarks: '',
        lockId: "",
      },

      oldTimekeeping: [],
      logs: [],
      isLogVisible: false,

      memberRequest: [],

      schedules: [],

      isScheduleEditorVisible: false,

      schedule: {
        billingMemberId: 0,
        timekeepingBillingId: 0,
        memberId: 0,
        memberName: '',
        departmentName: '',
        totalWorkingDay:0,
        allowanceDaily:0,
        totalWorkingHour:0,
        payingDailyRate:0,
        basicPayOutRate:0,
        regularHourRate:0,
        overtimeHourRate:0,
        lateMinuteRate:0,
        undertimeMinuteRate:0,
        hU100Rate:0,
        oTRate:0,
        rDRate:0,
        rDOTRate:0,
        sHRate:0,
        sHOTRate:0,
        sHRDRate:0,
        sHRDOTRate:0,
        lHRate:0,
        lHOTRate:0,
        lHRDRate:0,
        lHRDOTRate:0,
        lHSHRate:0,
        lHSHRDRate:0,
        dRHRate:0,
        dRHOTRate:0,
        dRHRDRate:0,
        dRHRDOTRate:0,
        nDRate:0,
        nDOTRate:0,
        rDNDRate:0,
        rDNDOTRate:0,
        sHNDRate:0,
        sHNDOTRate:0,
        sHRDNDRate:0,
        sHRDNDOTRate:0,
        lHNDRate:0,
        lHNDOTRate:0,
        lHRDNDRate:0,
        lHRDNDOTRate:0,
        hU100Hour:0,
        otHour:0,
        rdHour:0,
        rdOTHour:0,
        shHour:0,
        shOTHour:0,
        shRDHour:0,
        shRDOTHour:0,
        lhHour:0,
        lhOTHour:0,
        lhRDHour:0,
        lhRDOTHour:0,
        lhSHHour:0,
        lhSHRDHour:0,
        drHHour:0,
        drHOTHour:0,
        drHRDHour:0,
        drHRDOTHour:0,
        ndHour:0,
        ndOTHour:0,
        rdNDHour:0,
        rdNDOTHour:0,
        shNDHour:0,
        shNDOTHour:0,
        shRDNDHour:0,
        shRDNDOTHour:0,
        lhNDHour:0,
        lhNDOTHour:0,
        lhRDNDHour:0,
        lhRDNDOTHour:0,
        rdPremiumRate:0,
        shPremiumRate:0,
        lhPremiumrRate:0,
        shRDPremiumRate:0,
        lhRDPremiumRate:0,
        rdAmount:0,
        shAmount:0,
        lHAmount:0,
        shRDAmount:0,
        lhRDAmount:0,
        extendedWorkRate:0,
        otherAllowance:0,


        grossPay:0,
        totalGrossPay:0,
        adminFee:0,
        netBilling:0,
        totalGrossPayAF:0,
        totalBasic:0,
        yei:0,
        
        lockId: ''
      },

      scheduleIndex: -1,
      isAddingSchedule: false,
      excelData: [],

      searchMemberNameInput: '',

      isProcessed: false,

      currentHighlightIndex: -1, 
       totals: {
      totalWorkingDay: 0,
    allowanceDaily: 0,
    totalWorkingHour: 0,
    payingDailyRate: 0,
    basicPayOutRate: 0,
    regularHourRate: 0,
    overtimeHourRate: 0,
    lateMinuteRate: 0,
    undertimeMinuteRate: 0,
    hU100Rate: 0,
    otRate: 0,
    rdRate: 0,
    rdOTRate: 0,
    shRate: 0,
    shOTRate: 0,
    shRDRate: 0,
    shRDOTRate: 0,
    lhRate: 0,
    lhOTRate: 0,
    lhRDRate: 0,
    lhRDOTRate: 0,
    lhSHRate: 0,
    lhSHRDRate: 0,
    drHRate: 0,
    drHOTRate: 0,
    drHRDRate: 0,
    drHRDOTRate: 0,
    ndRate: 0,
    ndOTRate: 0,
    rdNDRate: 0,
    rdNDOTRate: 0,
    shNDRate: 0,
    shNDOTRate: 0,
    shRDNDRate: 0,
    shRDNDOTRate: 0,
    lhNDRate: 0,
    lhNDOTRate: 0,
    lhRDNDRate: 0,
    lhRDNDOTRate: 0,
    hu100Hour: 0,
    otHour: 0,
    rdHour: 0,
    rdOTHour: 0,
    shHour: 0,
    shOTHour: 0,
    shRDHour: 0,
    shRDOTHour: 0,
    lhHour: 0,
    lhOTHour: 0,
    lhRDHour: 0,
    lhRDOTHour: 0,
    lhSHHour: 0,
    lhSHRDHour: 0,
    drHHour: 0,
    drHOTHour: 0,
    drHRDHour: 0,
    drHRDOTHour: 0,
    ndHour: 0,
    ndOTHour: 0,
    rdNDHour: 0,
    rdNDOTHour: 0,
    shNDHour: 0,
    shNDOTHour: 0,
    shRDNDHour: 0,
    shRDNDOTHour: 0,
    lhNDHour: 0,
    lhNDOTHour: 0,
    lhRDNDHour: 0,
    lhRDNDOTHour: 0,
    rdPremiumRate: 0,
    shPremiumRate: 0,
    lhPremiumrRate: 0,
    shRDPremiumRate: 0,
    lhRDPremiumRate: 0,
    rdAmount: 0,
    shAmount: 0,
    lhAmount: 0,
    shRDAmount: 0,
    lhRDAmount: 0,
    extendedWorkRate: 0,
    otherAllowance: 0,
    grossPay: 0,
    totalGrossPay: 0,
    adminFee: 0,
    netBilling: 0,

    totalGrossPayAF: 0,
    yei: 0,
    totalBasic: 0,
    },
        uri :'data:application/vnd.ms-excel;base64,',
    template:'<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
    base64: function(s){ return window.btoa(unescape(encodeURIComponent(s))) },
    format: function(s, c) { return s.replace(/{(\w+)}/g, function(m, p) { return c[p]; }) },



    };



  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },


  methods: {

        tableToExcel(table, name) {
  if (!table.nodeType) table = this.$refs.table;

  const uri = 'data:application/vnd.ms-excel;base64,';
  const template = `
    <html xmlns:o="urn:schemas-microsoft-com:office:office"
          xmlns:x="urn:schemas-microsoft-com:office:excel"
          xmlns="http://www.w3.org/TR/REC-html40">
    <head>
      <meta charset="UTF-8">
      <!-- Excel-specific styling -->
      <style>
        table, th, td {
          border: 1px solid #000;
          border-collapse: collapse;
        }
        th {
          background-color: #f2f2f2;
          color: black;
          text-align: center;
        }
        td {
          padding: 5px;
        }
      </style>
    </head>
    <body>
      <table>{table}</table>
    </body>
    </html>`;

  const base64 = s => window.btoa(unescape(encodeURIComponent(s)));
  const format = (s, c) => s.replace(/{(\w+)}/g, (m, p) => c[p]);

  const ctx = { worksheet: name || 'Worksheet', table: table.innerHTML };
  const href = uri + base64(format(template, ctx));
  window.location.href = href;
},


  onClickTimekeepingPayOut(){ 
      const me = this;
      let route = {
        name: "ars0090",
        query: {
          timekeepingPayOutId: me.timekeeping.timekeepingPayOutId,
        },
      };
      me.go(route);
  },


  onClickPayGroup(){ 
      const me = this;
      let route = {
        name: "ars0040",
        query: {
          clientPayGroupId: me.timekeeping.clientPayGroupId,
        },
      };
      me.go(route);
  },

  onPost () {
      const me = this;

      me.dialog.confirm('Ready to post <b>Pay Out Trx #</b>' + this.timekeeping.timekeepingPayOutId + ' - '+ this.timekeeping.clientPayGroupName + '.<br>.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.timekeeping.timekeepingBillingStatusId = 2;
            
            me.onSubmit();
          }
          return;
        }
      );
    },

    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to cancel <b>Pay Out Trx #</b>' + this.timekeeping.timekeepingPayOutId + ' - '+ this.timekeeping.clientPayGroupName + '.<br>.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.timekeeping.timekeepingBillingStatusId = 3;
           
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },


refreshTotal() {
  const totals = {
    totalWorkingDay: 0,
    allowanceDaily: 0,
    totalWorkingHour: 0,
    payingDailyRate: 0,
    basicPayOutRate: 0,
    regularHourRate: 0,
    overtimeHourRate: 0,
    lateMinuteRate: 0,
    undertimeMinuteRate: 0,
    hU100Rate: 0,
    otRate: 0,
    rdRate: 0,
    rdotRate: 0,
    shRate: 0,
    shotRate: 0,
    shrdRate: 0,
    shrdotRate: 0,
    lhRate: 0,
    lhotRate: 0,
    lhrdRate: 0,
    lhrdotRate: 0,
    lhshRate: 0,
    lhshrdRate: 0,
    drhRate: 0,
    drhotRate: 0,
    drhrdRate: 0,
    drhrdotRate: 0,
    ndRate: 0,
    ndotRate: 0,
    rdndRate: 0,
    rdndotRate: 0,
    shndRate: 0,
    shndotRate: 0,
    shrdndRate: 0,
    shrdndotRate: 0,
    lhndRate: 0,
    lhndotRate: 0,
    lhrdndRate: 0,
    lhrdndotRate: 0,
    hu100Hour: 0,
    otHour: 0,
    rdHour: 0,
    rdotHour: 0,
    shHour: 0,
    shotHour: 0,
    shrdHour: 0,
    shrdotHour: 0,
    lhHour: 0,
    lhotHour: 0,
    lhrdHour: 0,
    lhrdotHour: 0,
    lhshHour: 0,
    lhshrdHour: 0,
    drhHour: 0,
    drhotHour: 0,
    drhrdHour: 0,
    drhrdotHour: 0,
    ndHour: 0,
    ndotHour: 0,
    rdndHour: 0,
    rdndotHour: 0,
    shndHour: 0,
    shndotHour: 0,
    shrdndHour: 0,
    shrdndotHour: 0,
    lhndHour: 0,
    lhndotHour: 0,
    lhrdndHour: 0,
    lhrdndotHour: 0,
    rdPremiumRate: 0,
    shPremiumRate: 0,
    lhPremiumrRate: 0,
    shRDPremiumRate: 0,
    lhRDPremiumRate: 0,
    rdAmount: 0,
    shAmount: 0,
    lhAmount: 0,
    shrdAmount: 0,
    lhrdAmount: 0,
    extendedWorkRate: 0,
    otherAllowance: 0,
    grossPay: 0,
    grossShare: 0,
    totalShare: 0,
    
    grossPay:0,
    totalGrossPay:0,
    totalGrossPayAF:0,
    totalBasic:0,
    adminFee:0,
    netBilling:0,
    yei:0,
  };
  this.schedules.forEach(entry => {
    for (const key in totals) {
      if (entry[key] !== undefined) {
        totals[key] += parseFloat(entry[key]) || 0;
      }
    }
  });

  this.totals = totals; 
},

    async onProcess() {
   const me = this;

me.getBillingDetails().then(
  
(data) => {
  
  me.schedules = data;
  me.refreshTotal(0);
  },
  (fault) => {
    me.showFault(fault);
  }
);

  const length = this.filteredSchedules.length;
    for (let i = 0; i < length; i++) {
      this.currentHighlightIndex = i;
      await new Promise(resolve => setTimeout(resolve, 300)); 
    }
    this.currentHighlightIndex = -1;
  },

     applyMemberNameSearch() {
    this.searchMemberName = this.searchMemberNameInput;
  },

    onTimekeepingPayOutIdChanged () {
      const me = this;

      me.getTimekeepingPayOutInfo().then(
        (data) => {
          me.schedules = me.core.convertDates(data.sheet);
         me.setFocus("billingDate");
         me.onProcess();

        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },

    onTimekeepingPayOutIdChanging (e) {
      e.callback = this.timekeepingPayOutIdCallback;
    },

    timekeepingPayOutIdCallback (e) {
      const me = this;
      let filter = "TimekeepingPayOutId='" + e.proposedValue + "'";
      return getList('dbo.QPayTimekeepingPayOut', 'TimekeepingPayOutId, ClientPayGroupId, ClientPayGroupName, CutOffStartDate, CutOffEndDate, Remarks', '', filter).then(
        sheet => {
          if (sheet && sheet.length) {
            me.timekeeping.timekeepingPayOutId = sheet[0].timekeepingPayOutId;
            me.timekeeping.clientPayGroupId = sheet[0].clientPayGroupId;
            me.timekeeping.clientPayGroupName = sheet[0].clientPayGroupName;            
            me.timekeeping.cutOffStartDate = this.core.convertDates(sheet[0]).cutOffStartDate;
            me.timekeeping.cutOffEndDate = this.core.convertDates(sheet[0]).cutOffEndDate;
            me.timekeeping.remarks = sheet[0].remarks;
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

    onTimekeepingPayOutIdLostFocus () {
      const me = this;

      if (!me.timekeeping.timekeepingPayOutId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingPayOutIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];
      me.timekeeping.timekeepingPayOutId = data.timekeepingPayOutId;
      me.timekeeping.clientPayGroupId = data.clientPayGroupId;
      me.timekeeping.clientPayGroupName = data.clientPayGroupName;
      me.timekeeping.cutOffStartDate = this.core.convertDates(data).cutOffStartDate;
      me.timekeeping.cutOffEndDate = this.core.convertDates(data).cutOffEndDate;
      me.timekeeping.remarks = data.remarks;
      me.setFocus("billingDate");
      me.onTimekeepingPayOutIdChanged();
    },



    getTargetPath() {
      const me = this,
        q = {};

      if (me.timekeeping.timekeepingBillingId) {
        q.timekeepingBillingId = me.timekeeping.timekeepingBillingId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("timekeepingBillingId" in q && me.core.isInteger(q.timekeepingBillingId)) {
       
        me.timekeeping.timekeepingBillingId = parseInt(q.timekeepingBillingId);
      }

     },

    // API calls


    loadData() {
      const me = this,
        wait = me.wait();
        
        me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {  
            me.deduction = data.deduction
          }
       
          if (me.timekeeping.timekeepingBillingId < 0) {
            return Promise.resolve(null);
          }
          
          return me.getTimekeeping();
        })
        
        .then((timekeeping) => {
          me.stopWait(wait);
          if (timekeeping && timekeeping.timekeeping.length) {
            me.setModels(timekeeping);
          } else {
            if (me.timekeeping.timekeepingBillingId > -1) {
              let message = "Timekeeping Billing ID '<b>" + me.timekeeping.timekeepingBillingId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
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


            me.timekeeping.billingDate= me.today;
            
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
        timekeeping = info.timekeeping[0];
    
      me.timekeeping = me.core.convertDates(timekeeping);
      
      me.schedules = info.schedule;
      me.memberList = info.member;
      me.refreshTotal();
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldSchedules = JSON.stringify(me.schedules);
    },

    getBillingDetails() {
      const me = this;
      return get("api/timekeeping-billing-process/" + me.timekeeping.timekeepingPayOutId);
    },

    
    getTimekeepingPayOutInfo() {
      const me = this;
      return get("api/timekeeping-pay-out-id/" + me.timekeeping.timekeepingPayOutId );
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingBilingId < 0) {
        return Promise.resolve(null);
      }

      return get("api/timekeeping-billing/" + this.timekeeping.timekeepingBillingId);
    },

    getReferences() {
      const me = this;
      if (me.schedules.length) {
        return Promise.resolve(true);
      }

      return get("api/references/ars0100");
    },

    getChangeLog(log) {
      return get("api/timekeeping-records/" + log + "/" +  this.timekeeping.timekeepingPayOutId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/timekeeping-billing/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeeping-billing/" + this.timekeeping.timekeepingBillingId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeeping-billing/" + this.timekeeping.timekeepingBillingId + "/" + timekeeping.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        timekeeping = {};

      Object.assign(timekeeping, me.timekeeping);
      timekeeping.schedules = me.schedules;
      return timekeeping;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "timekeeping","schedules","schedule",

      );
      dc.keyField = "timekeepingBillingId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
      (this.docs = []),
      this.isCancelling = false;

      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onTimekeepingBillingIdChanging(e) {
      e.callback = this.timekeepingBillingIdCallback;
    },

    timekeepingBillingIdCallback(e) {
      e.message = "Timekeeping Billing ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.PayTimekeepingBilling", "timekeepingBillingId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onTimekeepingBillingIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onTimekeepingBillingIdLostFocus() {
      const me = this;

      if (!me.timekeeping.timekeepingBillingId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingBillingIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.timekeeping.timekeepingBillingId = data.timekeepingBillingId;

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
              me.timekeeping.timekeepingBillingId = success;
            }

            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";

              if (me.isCancelling) {
                message = "Billing Trx ID # '" + me.timekeeping.timekeepingBillingId + "' cancelled."
              }



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

      getSafeDeleteFlag("PayTimekeepingSheet", me.timekeeping.timekeepingSheetId)
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

      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "timekeepingPayOutId",
          "billingDate",
        );

        me.setDisplayMode(
          "clientPayGroupName",
          "clientPayGroupId",
          "cutOffStartDate",
          "cutOffEndDate",
          "remarks",

        );

        me.setFocus("timekeepingPayOutId");
      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.timekeeping) !== me.oldTimekeeping) {return true;}
      if (JSON.stringify(me.schedules) !== me.oldSchedules) { return true; }

      return false;

    },

  },

  created () {
    const me = this;

    me.isCancelling = false;
    me.oldTimekeeping = '';
    me.oldSchedules = '';
    me.memberList = [];
    me.deduction = [];
    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {
    isCanceled () {
      return this.timekeeping.timekeepingBillingStatusId == 3;
    },

    isPosted () {
      return this.timekeeping.timekeepingBillingStatusId === 2;
    },
       
    isSubmitted () {
      return this.timekeeping.timekeepingBillingStatusId === 1;
    },

  filteredSchedules() {
    if (!this.searchMemberNameInput) {
      return this.schedules;
    }
    const search = this.searchMemberNameInput.toLowerCase();
    return this.schedules.filter(dtl =>
      dtl.memberName.toLowerCase().includes(search)
    );
  },

    isSchedule () {
      return this.schedules.length;
    },

    detailTag () {
      return 'Schedules: ' + this.schedules.length;
    },

  },
};
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

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.box-field {
display: grid;
grid-template-rows: 1fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr .1fr .5fr .1fr 2fr .5fr .5fr ;
}
.box-2 {
  display: grid;
  grid-template-columns: .3fr 2fr  ;
}
.box-3 {
  display: grid;
  grid-template-columns: .6fr .5fr .5fr;
}
.box-4 {
  display: grid;
  grid-template-columns: .5fr .5fr .5fr;
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
.main-scroll-wrapper{
  overflow: auto;
}
.table-scroller{
  
   overflow: auto;
  min-width: 86vw;
  width: 1000%;
  height: 50vh;
  
}
table.scroller {
  min-width: 50vw; 
  border-collapse: collapse;
   width: 1000%;

  white-space: nowrap;
}
.id{
  width: 6rem;
}
.name{
  width: 25rem;
}
.date{
  width: 7rem;
}
.type{
  width: 15rem;
}
.sched{
  width: 15rem;
  text-wrap: wrap;
}
.in{
  width: 5rem;
}
.out{
  width: 6rem;
}
.hour{
  width: 10rem;
}
.actions{
  width: 6rem;
}
.buttons{
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  gap: .5rem;
}
.post-btn{
  background-color: none;
}
.act-btns{
  display: flex;
  gap: .5rem;
  justify-content: center;
  border: none;
}

 .table-scroll-wrapper {
  overflow: auto;
  max-height: 10%;
  
}

.table-scroll-wrapper {
  width: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll tbody tr:hover {
  background-color: rgb(88, 180, 207);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
}

.running-highlight {
  color: #d9534f;
  font-weight: bold;
  transition: background-color 0.05s ease, color 0.05s ease, font-weight 0.05s ease;
} 
.main-scroll-wrapper{
    overflow: auto;
 min-width: 56vw;
 width: 100%;
 height: 50vh;
}
.btn-container{
  display: flex;
  justify-content: space-between;
}


@media(max-width: 1700px){ 
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: .5fr 2fr .5fr;
}
.btn-container{
  display: flex;
  justify-content: space-between;
}

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.box-field {
display: grid;
grid-template-rows: 1fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr .1fr .5fr .1fr 2fr .5fr .5fr ;
}
.box-2 {
  display: grid;
  grid-template-columns: .3fr 2fr  ;
}
.box-3 {
  display: grid;
  grid-template-columns: .6fr .5fr .5fr;
}
.box-4 {
  display: grid;
  grid-template-columns: .5fr .5fr .5fr;
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
.table-scroller{
  
  overflow: auto;
 min-width: 86vw;
 width: 1500%;
 height: 50vh;
 
}
table.scroller {
 min-width: 50vw; 
 border-collapse: collapse;
  width: 1200%;

 white-space: nowrap;
}
.main-scroll-wrapper{
    overflow: auto;
 min-width: 56vw;
 width: 100%;
 height: 50vh;
}
.id{
  width: 6rem;
}
.name{
  width: 25rem;
}
.date{
  width: 7rem;
}
.type{
  width: 15rem;
}
.sched{
  width: 15rem;
  text-wrap: wrap;
}
.in{
  width: 5rem;
}
.out{
  width: 6rem;
}
.hour{
  width: 10rem;
}
.actions{
  width: 6rem;
}
}
</style>
