// Member Pay Out

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="ars0090" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="timekeeping.timekeepingPayOutId" :caption-width="35" caption="Pay Out ID" lookupId="PayTimekeepingPayOut" @lostfocus="onTimekeepingPayOutIdLostFocus" @changing="onTimekeepingPayOutIdChanging" @changed="onTimekeepingPayOutIdChanged" @searchresult="onTimekeepingPayOutIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
            <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCanceled">Cancelled</sym-tag>
            <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isPosted">Posted</sym-tag>
            <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isSubmitted">Submitted</sym-tag>

      <div class="buttons d-inline ml-auto">
        <button class="btn-copy border-main justify-between fw-50 primary mb-2" type="button" v-if="isPosted && isPostToPayable" @click="onPayableSubmit">
          <i class="fa fa-tasks   fa-lg"></i><span>POST TO PAYABLE</span>
        </button>
      </div>

          </div>
          

      </div>
        <div class="box-field">

            <div class="app-box-style">
              <div class="box-1 gap">
                <sym-int v-model="timekeeping.timekeepingSheetId" :caption-width="35" align="left" caption="Sheet ID" lookupId="PayTimekeepingSheet" @lostfocus="onTimekeepingSheetIdLostFocus" @changing="onTimekeepingSheetIdChanging" @changed="onTimekeepingSheetIdChanged" @searchresult="onTimekeepingSheetIdSearchResult"></sym-int>

             <button :disabled ="!timekeeping.timekeepingSheetId"  type="button" :class="logButtonClass" class="secondary w-100 mb-2" @click="onClickTimekeepingSheet"> 
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
            
              
             <button :disabled ="isCanceled && isPosted && !timekeeping.timekeepingPayOutId && !timekeeping.timekeepingSheetId" type="button" :class="logButtonClass" class="applicant-btn w-10 info" @click="onProcess"> 
              <i class="fa fa-calculator"></i><span class="bold">Calculate Payout</span> 
            </button>



            <div>

        </div>

              </div>


        <!-- <div class="buttons " :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
 
            
          
         

          
          <button type="button" :class="submitButtonClass" class="justify-between btn-save" :disabled="isCanceled || isPosted" @click="onSubmit"> <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="logButtonClass" class="justify-between btn-log"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
         
     
         

       
        </div> -->
        <div class="btn-container">
          <div class="post-btn "  :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
            <button type="button" :class="deleteButtonClass" class="justify-between btn-delete white" @click="onPost" :disabled="isCanceled && isPosted"><i class="fa fa-check fa-lg"></i><span class="bold">Post</span></button>
         
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
            
            <button type="button" :class="deleteButtonClass" class=" justify-between btn-delete white  " @click="onCancel" :disabled="isCanceled || isPosted"><i class="fa fa-times-circle fa-lg "></i><span class="bold">CANCEL</span></button>
          </div>
        
      </div>


              <div class="app-box-style">
        <div class="text-center border-light curved p-1 info mt-2">
          <span class="serif lg-3">Total Member/s:  {{ schedules.length}}</span>
        </div>
      
<!-- <button type="button" class="success mb-3 w-100 text-center lg-2 p-1 shadow-soft" @click="tableToExcel('table', 'loremTable')"><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button> -->
      <!-- Search Filter -->
<div class="d-flex align-center justify-between mb-2">
  <sym-text v-model="searchMemberNameInput" align="left" :caption-width="32" :input-width="100" caption=" Name" list="memberNames" @changed="applyMemberNameSearch"></sym-text>
  <datalist id="memberNames"><option v-for="item in memberList" :key="item.memberName"  :value="item.memberName"  @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist>
  <button type="button" class="success mb-3 w-10 text-center lg-2 p-1 shadow-soft" @click="tableToExcel('table', 'loremTable')"><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>
</div>


          <!-- <div class="filter-container">
                <sym-text v-model="searchMemberName" align="left" :caption-width="35" :input-width="100" list="memberNames" caption="Member Name"></sym-text>    
                <datalist id="memberNames"><option v-for="item in memberList" :key="item.memberName" :value="item.memberName" @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist> 


                <button type="button" class="success w-100 text-center lg-2 shadow-soft upload-btn" @click="generateExcelReport" ><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>

              </div>
            
 -->



<!-- Timekeeping Table -->
 <div class="main-scroll-wrapper " >
<div class="table-scroller " v-show="timekeeping.timekeepingPayOutId != 0">
  <div class="table-scroll d-flex justify-centerXXX fixed-header info-light ">
    <table class="striped-even mb-0 scroller info-light" ref="table" id="loremTable" >
      <thead >
          <tr>
            <!-- <th  class="text-center w-.5" colspan="1">#</th> -->
            <th class="text-center " >#</th>
            <th class="text-center" >Member ID</th>
            <th class="text-center" >Member Name</th>
            <!-- <th class="w-1" colspan="1">Member ID</th> -->
            <!-- <th class="w-2" colspan="1">Member Name</th> -->
            <th colspan="1">No. of Days Worked</th>
            <!-- <th colspan="1">Allowance per Day</th> -->
            <th colspan="1">Total Manhrs Worked</th>
            <th colspan="1">Paying Rate per Day</th>
            <th colspan="1">Basic Payout Amount</th>
            <th class="text-center" colspan="2">Regular Overtime</th>
            <th class="text-center" colspan="2">Rest Day</th>
            <th class="text-center" colspan="2">Rest Day OT</th>
            <th class="text-center" colspan="2">Special Holiday</th>
            <th class="text-center" colspan="2">Special Holiday OT</th>
            <th class="text-center" colspan="2">Legal Holiday Unworked</th>

            <th class="text-center" colspan="2">Legal Holiday</th>
            <th class="text-center" colspan="2">Legal Holiday OT</th>

            <th class="text-center" colspan="2">Special Holiday Rest Day </th>
            <th class="text-center" colspan="2">Special Holiday Rest Day OT</th>

            <th class="text-center" colspan="2">Legal Holiday Rest Day</th>
            <th class="text-center" colspan="2">Legal Holiday Rest Day OT</th>
            <th class="text-center" colspan="1">Extended Work</th>
            <th colspan="2">Late</th>
            <th colspan="2">Undertime</th>

            <th class="text-center" colspan="2">Night Differential</th>
            <th class="text-center" colspan="2">Night Differential OT</th>
            <th class="text-center" colspan="2">Night Differential RD</th>
            <th class="text-center" colspan="2">Night Differential RD OT</th>
           <th class="text-center" colspan="2">Night Differential SH</th>
           <th class="text-center" colspan="2">Night Differential SH OT</th>
           <th class="text-center" colspan="2">Night Differential SH RD</th>
           <th class="text-center" colspan="2">Night Differential SH RD OT</th>
           <th class="text-center" colspan="2">Night Differential LH</th>
           <th class="text-center" colspan="2">Night Differential LH OT</th>
           <th class="text-center" colspan="2">Night Differential LH RD</th>
           <th class="text-center" colspan="2">Night Differential LH RD OT</th>
           <th class="text-center" colspan="1">Other Allowance</th>
           <th class="text-center" colspan="5">Billable Shares</th>
           <th class="text-center" colspan="1">Gross Pay</th>
           <th class="text-center" colspan="1">Gross Share</th>
           <th class="text-center" colspan="1">Total Share</th>
           <th class="text-center" colspan="32">Deductions</th>
           <th class="text-center" colspan="1">Total</th>
           <th class="text-center" colspan="1">Net Share</th>

           <!-- <th class="text-center">Actions</th> -->
          </tr>

          <tr>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <th colspan="1"></th>
            <!-- <th colspan="1"></th> -->
            <!-- RD -->
            <!-- <th class="text-center" colspan="2">125%</th> jenon--> 
             <th class="text-center" colspan="2">{{percot}} %</th>
            <!-- <th class="text-center" colspan="1"></th> -->
            <!-- <th class="text-center" colspan="1">100%</th>
            <th class="text-center" colspan="1">30%</th> -->

  <!-- me.percrd =  me.schedules[0].rdPerc;
  me.percrdot=  me.schedules[0].rdotPerc;
  me.percsh=  me.schedules[0].shPerc;
  me.percshot=  me.schedules[0].shotPerc;
  me.percshrd=  me.schedules[0].shrdPerc;
  me.percshrdot=  me.schedules[0].shrdotPerc;
  me.perclh=  me.schedules[0].lhPerc;
  me.perclhot=  me.schedules[0].lhotPerc;
  me.perclhrd=  me.schedules[0].lhrdPerc;
  me.perclhrdot=  me.schedules[0].lhrdotPerc;
  me.perclhsh=  me.schedules[0].lhshPerc;
  me.perclhshrd=  me.schedules[0].lhshrdPerc;
  me.percdrh=  me.schedules[0].drhPerc;
  me.percdrhot=  me.schedules[0].drhotPerc;
  me.percdrhrd=  me.schedules[0].drhrdPerc;
  me.percdrhrdot=  me.schedules[0].drhrdotPerc;
  me.percnd=  me.schedules[0].ndPerc;
  me.percndot=  me.schedules[0].ndotPerc;
  me.percrdnd=  me.schedules[0].rdndPerc;
  me.percrdndot=  me.schedules[0].rdndotPerc;
  me.percshnd=  me.schedules[0].shndPerc;
  me.percshndot=  me.schedules[0].shndotPerc;
  me.percshrdnd=  me.schedules[0].shrdndPerc;
  me.percshrdndot=  me.schedules[0].shrdndPerc;
  me.perclhnd=  me.schedules[0].lhndPerc;
  me.perclhndot=  me.schedules[0].lhndotPerc;
  me.perclhrdnd=  me.schedules[0].lhrdndPerc;
  me.perclhrdndot=  me.schedules[0].lhrdndotPerc; -->


            <th class="text-center" colspan="2">{{percrd}} %</th> 
            <!-- <th class="text-center" colspan="2">130%</th> -->
            <th class="text-center" colspan="2">{{percrdot}} %</th>
            <!-- SH -->
            <!-- <th class="text-center" colspan="1"></th> -->
            <!-- <th class="text-center" colspan="1">100%</th>
            <th class="text-center" colspan="1">30%</th> -->
            <th class="text-center" colspan="2">{{percsh}} %</th>
            <th class="text-center" colspan="2">{{percshot}} %</th>
            <!-- LH Unworked -->
            <th class="text-center" colspan="2">{{perchu100}} %</th>
            <!-- LH -->
            <!-- <th class="text-center" colspan="1"></th> -->
            <!-- <th class="text-center" colspan="1">100%</th>
            <th class="text-center" colspan="1">100%</th> -->
            <th class="text-center" colspan="2">{{perclh}} %</th>
            <th class="text-center" colspan="2">{{perclhot}} %</th>
            <!-- SHRD -->
            <!-- <th class="text-center" colspan="1"></th> -->
            <!-- <th class="text-center" colspan="1">100%</th>
            <th class="text-center" colspan="1">50%</th> -->
            <th class="text-center" colspan="2">{{percshrd}} %</th>
            <th class="text-center" colspan="2">{{percshrdot}} %</th>
            <!-- LHRD -->
            <!-- <th class="text-center" colspan="1"></th> -->
            <!-- <th class="text-center" colspan="1">100%</th>
            <th class="text-center" colspan="1">160%</th> -->
            <th class="text-center" colspan="2">{{perclhrd}} %</th>
            <th class="text-center" colspan="2">{{perclhrdot}} %</th>
            <!-- EW -->
            <th class="text-center" colspan="1">Total</th>
            <!-- Late -->
            <th class="text-center" colspan="2">100 %</th>
            <!-- Undertime -->
            <th class="text-center" colspan="2">100 %</th>


            <!-- ND -->
            <th class="text-center" colspan="2">{{percnd}} %</th>
            <!-- ND OT-->
            <th class="text-center" colspan="2">{{percndot}} %</th>
            <!-- ND RD-->
            <th class="text-center" colspan="2">{{percrdnd}} %</th> 

            <!-- ND RD OT-->
            <th class="text-center" colspan="2">{{percrdndot}} %</th>

            <!-- SH ND -->
            <th class="text-center" colspan="2">{{percshnd}} %</th>
            <!-- SH ND OT-->
            <th class="text-center" colspan="2">{{percshndot}} %</th>
            <!-- SH ND RD-->
            <th class="text-center" colspan="2">{{percshrdnd}} %</th>
            <!-- SH ND RD OT-->
            <th class="text-center" colspan="2">{{percshrdndot}} %</th>
            <!-- LH ND-->
            <th class="text-center" colspan="2">{{perclhnd}} %</th>
            <!-- LH ND OT-->
            <th class="text-center" colspan="2">{{perclhndot}} %</th>
            <!-- LH ND RD-->
            <th class="text-center" colspan="2">{{perclhrdnd}} %</th>
            <!-- LH ND RD OT-->
            <th class="text-center" colspan="2">{{perclhrdndot}} %</th>
            <!-- Allowance   -->
            <th class="text-center" colspan="1"></th>
            <!-- Billable Shares   -->
            <th class="text-center" colspan="5"></th>
            <!-- Gross -->
            <th class="text-center" colspan="1"></th>
            <th class="text-center" colspan="1"></th>
            <th class="text-center" colspan="1"></th>
            <!-- SSS -->
            <th class="text-center" colspan="3">SSS</th>
            <!-- PF -->
            <th class="text-center" colspan="2">PF</th>
            <!-- HDMF -->
            <th class="text-center" colspan="2">HDMF</th>
            <!-- PHIC -->
            <th class="text-center" colspan="2">PHIC</th>
            <!-- Deductions -->
            <th class="text-center" colspan="23"></th>
            <th class="text-center" colspan="1"></th>
            <th class="text-center" colspan="1"></th>

            <!-- <th class="text-center">Actions</th> -->
          </tr>



          <tr >
          <th class="id"></th>
          <th class="id"></th>
          <th class="name"></th>
          <th class="text-right"></th>
          <!-- <th class="text-right"></th> -->
          <th class="text-right"></th>
          <th class="text-right"></th>
          <th class="text-right"></th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- RestDay  -->
          <th class="text-right">Hrs</th>
          <!-- <th class="text-right">Amount</th>
          <th class="text-right">Premium Amount</th> -->
          <th class="text-right">Rest Day</th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Special Holiday	  -->
          <th class="text-right">Hrs</th>
          <!-- <th class="text-right">Amount</th>
          <th class="text-right">Premium Amount</th> -->
          <th class="text-right">Special Holiday</th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
           <!-- Legal Holiday Unworked -->
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Legal Holiday  -->
          <th class="text-right">Hrs</th>
          <!-- <th class="text-right">Amount</th>
          <th class="text-right">Premium Amount</th> -->
          <th class="text-right">Legal Holiday</th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Special Holiday Rest Day			  -->
          <th class="text-right">Hrs</th>
          <!-- <th class="text-right">Amount</th>
          <th class="text-right">Premium Amount</th> -->
          <th class="text-right">Special Holiday Rest Day</th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Legal Holiday Rest Day -->
          <th class="text-right">Hrs</th>
          <!-- <th class="text-right">Amount</th>
          <th class="text-right">Premium Amount</th> -->
          <th class="text-right">Legal Holiday Rest Day</th>
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Extended Work -->
          <th class="text-right">Amount</th>
          <!-- Late -->
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Undertime -->
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>



          <!-- Night Differential	 -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
            <!-- Night Differential	RD -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	RD OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential SH	 -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	SH OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential SH RD	 -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	SH RD OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	LH -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	LH OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	LH RD -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Night Differential	LH RD OT -->  
          <th class="text-right">Hrs</th>
          <th class="text-right">Amount</th>
          <!-- Other Allowance -->  
          <th class="text-right">Allowance</th>

          <!-- Billable Shares -->  
          <th class="text-right">SSS</th>
          <th class="text-right">PF</th>
          <th class="text-right">EC</th>
          <th class="text-right">Pag-Ibig</th>
          <th class="text-right">PHIC</th>
          <th class="text-right"></th>
          <th class="text-right"></th>
          <th class="text-right"></th>
          <th class="text-right">ER</th>
          <th class="text-right">EE</th>
          <th class="text-right">EC</th>

          <th class="text-right">ER</th>
          <th class="text-right">EE</th>

          <th class="text-right">ER</th>
          <th class="text-right">EE</th>

          <th class="text-right">ER</th>
          <th class="text-right">EE</th>
          <!-- Deductions     -->
          <th class="text-right">Multi-Purpose Loan</th>
          <th class="text-right">Special Loan 4</th>
          <th class="text-right">SPECIAL LOAN 1 (Cash)</th>
          <th class="text-right">Special Loan</th>
          <th class="text-right">Emergency Loan</th>
          <th class="text-right">Special Loan 3</th>
          <th class="text-right">Salary Loan</th>
          <th class="text-right">E-Promo</th>
          <th class="text-right">CWCC Share Capital</th>
          <th class="text-right">Pag-IBIG MPL Loan 3</th>
          <th class="text-right">CWCC Membership Fee</th>
          <th class="text-right">PagIBIG Calamity Loan2</th>
          <th class="text-right">Coop Share</th>
          <th class="text-right">SSS Salary Loan</th>
          <th class="text-right">SSS Salary Loan 2</th>
          <th class="text-right">SSS Calamity Loan</th>
          <th class="text-right">Pag-IBIG MPL Loan</th>
          <th class="text-right">PagIBIG Calamity Loan</th>
          <th class="text-right">Pag-IBIG MPL 2</th>
          <th class="text-right">Personal Accident Insurance</th>
          <th class="text-right">Canteen</th>
          <th class="text-right">Accountability</th>
          <th class="text-right">Savings Deposit</th>
          <th class="text-right">Deductions</th>
          <th class="text-right"></th>

        </tr>
      </thead>
      <tbody >
          <tr v-for="(dtl, index) in filteredSchedules" :key="index"  :class="{ 'running-highlight': index === currentHighlightIndex }">
          <td class="text-center">{{ index + 1 }}</td>
          <td class="text-center">{{ dtl.memberId }}</td>
          <td>{{ dtl.memberName }}</td>        
          <td class="text-right">{{ dtl.totalWorkingDay === 0 ? '' : core.toDecimalFormat(dtl.totalWorkingDay)  }}</td>         
          <!-- <td class="text-right">{{ dtl.allowanceDaily === 0 ? '' : core.toDecimalFormat(dtl.allowanceDaily)  }}</td> -->
          <td class="text-right">{{ dtl.totalWorkingHour === 0 ? '' : core.toDecimalFormat(dtl.totalWorkingHour)  }}</td>
          <td class="text-right">{{ dtl.payingDailyRate === 0 ? '' : core.toDecimalFormat(dtl.payingDailyRate)  }}</td>

   
          
          <td class="text-right">{{ dtl.basicPayOutRate === 0 ? '' : core.toDecimalFormat(dtl.basicPayOutRate)  }}</td>
          
          <td class="text-right">{{ dtl.otHour === 0 ? '' : core.toDecimalFormat(dtl.otHour)  }}</td>
          <td class="text-right">{{ dtl.otRate === 0 ? '' : core.toDecimalFormat(dtl.otRate)  }}</td>

          
          <!-- RestDay  -->
          <td class="text-right">{{ dtl.rdHour === 0 ? '' : core.toDecimalFormat(dtl.rdHour)  }}</td>
          <td class="text-right">{{ dtl.rdRate === 0 ? '' : core.toDecimalFormat(dtl.rdRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.rdPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.rdPremiumRate)  }}</td>
          <td class="text-right">{{ dtl.rdAmount === 0 ? '' : core.toDecimalFormat(dtl.rdAmount)  }}</td> -->
          <td class="text-right">{{ dtl.rdotHour === 0 ? '' : core.toDecimalFormat(dtl.rdotHour)  }}</td>
          <td class="text-right">{{ dtl.rdotRate === 0 ? '' : core.toDecimalFormat(dtl.rdotRate)  }}</td>

          <!-- Special Holiday	  -->

          <td class="text-right">{{ dtl.shHour === 0 ? '' : core.toDecimalFormat(dtl.shHour)  }}</td>
          <td class="text-right">{{ dtl.shRate === 0 ? '' : core.toDecimalFormat(dtl.shRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.shPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.shPremiumRate)  }}</td>
          <td class="text-right">{{ dtl.shAmount === 0 ? '' : core.toDecimalFormat(dtl.shAmount)  }}</td> -->
          <td class="text-right">{{ dtl.shotHour === 0 ? '' : core.toDecimalFormat(dtl.shotHour)  }}</td>
          <td class="text-right">{{ dtl.shotRate === 0 ? '' : core.toDecimalFormat(dtl.shotRate)  }}</td>


          <!-- Legal Holiday Unworked -->

          <td class="text-right">{{ dtl.hu100Hour === 0 ? '' : core.toDecimalFormat(dtl.hu100Hour)  }}</td>
          <td class="text-right">{{ dtl.hu100Rate === 0 ? '' : core.toDecimalFormat(dtl.hu100Rate)  }}</td>

          <!-- Legal Holiday  -->
 
          <td class="text-right">{{ dtl.lhHour === 0 ? '' : core.toDecimalFormat(dtl.lhHour)  }}</td>
          <td class="text-right">{{ dtl.lhRate === 0 ? '' : core.toDecimalFormat(dtl.lhRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.lhPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.lhPremiumRate)  }}</td>
          <td class="text-right">{{ dtl.lhAmount === 0 ? '' : core.toDecimalFormat(dtl.lhAmount)  }}</td> -->
          <td class="text-right">{{ dtl.lhotHour === 0 ? '' : core.toDecimalFormat(dtl.lhotHour)  }}</td>
          <td class="text-right">{{ dtl.lhotRate === 0 ? '' : core.toDecimalFormat(dtl.lhotRate)  }}</td>

          <!-- Special Holiday Rest Day			  -->

          <td class="text-right">{{ dtl.shrdHour === 0 ? '' : core.toDecimalFormat(dtl.shrdHour)  }}</td>
          <td class="text-right">{{ dtl.shrdRate === 0 ? '' : core.toDecimalFormat(dtl.shrdRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.shrdPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.shrdPremiumRate)  }}</td>
          <td class="text-right">{{ dtl.shrdAmount === 0 ? '' : core.toDecimalFormat(dtl.shrdAmount)  }}</td> -->
          <td class="text-right">{{ dtl.shrdotHour === 0 ? '' : core.toDecimalFormat(dtl.shrdotHour)  }}</td>
          <td class="text-right">{{ dtl.shrdotRate === 0 ? '' : core.toDecimalFormat(dtl.shrdotRate)  }}</td>

          <!-- Legal Holiday Rest Day -->

          <td class="text-right">{{ dtl.lhrdHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdRate)  }}</td>
          <!-- <td class="text-right">{{ dtl.lhrdPremiumRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdPremiumRate)  }}</td>
          <td class="text-right">{{ dtl.lhrdAmount === 0 ? '' : core.toDecimalFormat(dtl.lhrdAmount)  }}</td> -->
          <td class="text-right">{{ dtl.lhrdotHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdotHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdotRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdotRate)  }}</td>




          <!-- Extended Work -->
          <td class="text-right">{{ dtl.extendedWorkRate === 0 ? '' : core.toDecimalFormat(dtl.extendedWorkRate)  }}</td>

          <!-- Late -->
          <td class="text-right">{{ dtl.lateMinuteP === 0 ? '' : core.toDecimalFormat(dtl.lateMinuteP)  }}</td>
          <td class="text-right">{{ dtl.lateMinuteRate === 0 ? '' : core.toDecimalFormat(dtl.lateMinuteRate)  }}</td>

          <!-- Undertime -->
          <td class="text-right">{{ dtl.undertimeMinute === 0 ? '' : core.toDecimalFormat(dtl.undertimeMinute)  }}</td>
          <td class="text-right">{{ dtl.undertimeMinuteRate === 0 ? '' : core.toDecimalFormat(dtl.undertimeMinuteRate)  }}</td>


          <!-- Night Differential	 --> 
          <td class="text-right">{{ dtl.ndHour === 0 ? '' : core.toDecimalFormat(dtl.ndHour)  }}</td>
          <td class="text-right">{{ dtl.ndRate === 0 ? '' : core.toDecimalFormat(dtl.ndRate)  }}</td>

          <!-- Night Differential	OT -->  
          <td class="text-right">{{ dtl.ndotHour === 0 ? '' : core.toDecimalFormat(dtl.ndotHour)  }}</td>
          <td class="text-right">{{ dtl.ndotRate === 0 ? '' : core.toDecimalFormat(dtl.ndotRate)  }}</td>

          <!-- Night Differential	RD -->  
          <td class="text-right">{{ dtl.rdndHour === 0 ? '' : core.toDecimalFormat(dtl.rdndHour)  }}</td>
          <td class="text-right">{{ dtl.rdndRate === 0 ? '' : core.toDecimalFormat(dtl.rdndRate)  }}</td>

          <!-- Night Differential	RD OT -->            
          <td class="text-right">{{ dtl.rdndotHour === 0 ? '' : core.toDecimalFormat(dtl.rdndotHour)  }}</td>
          <td class="text-right">{{ dtl.rdndotRate === 0 ? '' : core.toDecimalFormat(dtl.rdndotRate)  }}</td>

          <!-- Night Differential SH	 -->   
          <td class="text-right">{{ dtl.shHour === 0 ? '' : core.toDecimalFormat(dtl.shHour)  }}</td>
          <td class="text-right">{{ dtl.shRate === 0 ? '' : core.toDecimalFormat(dtl.shRate)  }}</td>

          <!-- Night Differential	SH OT -->  
          <td class="text-right">{{ dtl.shotHour === 0 ? '' : core.toDecimalFormat(dtl.shotHour)  }}</td>
          <td class="text-right">{{ dtl.shotRate === 0 ? '' : core.toDecimalFormat(dtl.shotRate)  }}</td>

          <!-- Night Differential SH RD	 -->  
          <td class="text-right">{{ dtl.shrdHour === 0 ? '' : core.toDecimalFormat(dtl.shrdHour)  }}</td>
          <td class="text-right">{{ dtl.shrdRate === 0 ? '' : core.toDecimalFormat(dtl.shrdRate)  }}</td>

          <!-- Night Differential	SH RD OT -->  
          <td class="text-right">{{ dtl.shrdotHour === 0 ? '' : core.toDecimalFormat(dtl.shrdotHour)  }}</td>
          <td class="text-right">{{ dtl.shrdotRate === 0 ? '' : core.toDecimalFormat(dtl.shrdotRate)  }}</td>

          <!-- Night Differential	LH -->  

          <td class="text-right">{{ dtl.lhHour === 0 ? '' : core.toDecimalFormat(dtl.lhHour)  }}</td>
          <td class="text-right">{{ dtl.lhRate === 0 ? '' : core.toDecimalFormat(dtl.lhRate)  }}</td>

          <!-- Night Differential	LH OT -->  
          <td class="text-right">{{ dtl.lhotHour === 0 ? '' : core.toDecimalFormat(dtl.lhotHour)  }}</td>
          <td class="text-right">{{ dtl.lhotRate === 0 ? '' : core.toDecimalFormat(dtl.lhotRate)  }}</td>

          <!-- Night Differential	LH RD -->  
          <td class="text-right">{{ dtl.lhrdHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdRate)  }}</td>

          <!-- Night Differential	LH RD OT -->  

          <td class="text-right">{{ dtl.lhrdotHour === 0 ? '' : core.toDecimalFormat(dtl.lhrdotHour)  }}</td>
          <td class="text-right">{{ dtl.lhrdotRate === 0 ? '' : core.toDecimalFormat(dtl.lhrdotRate)  }}</td>


          <!-- Other Allowance -->  
          <td class="text-right">{{ dtl.otherAllowance === 0 ? '' : core.toDecimalFormat(dtl.otherAllowance)  }}</td>

          <!-- Billable Shares -->  
          <td class="text-right">{{ dtl.ssser === 0 ? '' : core.toDecimalFormat(dtl.ssser)  }}</td>
          <td class="text-right">{{ dtl.pfer === 0 ? '' : core.toDecimalFormat(dtl.pfer)  }}</td>
          <td class="text-right">{{ dtl.sssec === 0 ? '' : core.toDecimalFormat(dtl.sssec)  }}</td>
          <td class="text-right">{{ dtl.pbger === 0 ? '' : core.toDecimalFormat(dtl.pbger)  }}</td>
          <td class="text-right">{{ dtl.phher === 0 ? '' : core.toDecimalFormat(dtl.phher)  }}</td>

          <!-- Gross    -->
          <td class="text-right">{{ dtl.grossPay === 0 ? '' : core.toDecimalFormat(dtl.grossPay)  }}</td>
          <td class="text-right">{{ dtl.grossShare === 0 ? '' : core.toDecimalFormat(dtl.grossShare)  }}</td>
          <td class="text-right">{{ dtl.totalShare === 0 ? '' : core.toDecimalFormat(dtl.totalShare)  }}</td>

          <!-- SSS    -->
          <td class="text-right">{{ dtl.ssser === 0 ? '' : core.toDecimalFormat(dtl.ssser)  }}</td>
          <td class="text-right">{{ dtl.sssee === 0 ? '' : core.toDecimalFormat(dtl.sssee)  }}</td>
          <td class="text-right">{{ dtl.sssee === 0 ? '' : core.toDecimalFormat(dtl.sssec)  }}</td>
  
          <!-- PF    -->
          <td class="text-right">{{ dtl.pfer === 0 ? '' : core.toDecimalFormat(dtl.pfer)  }}</td>
          <td class="text-right">{{ dtl.pfee === 0 ? '' : core.toDecimalFormat(dtl.pfee)  }}</td>

          <!-- HDMF    -->
          <td class="text-right">{{ dtl.pbger === 0 ? '' : core.toDecimalFormat(dtl.pbger)  }}</td>
          <td class="text-right">{{ dtl.pbger === 0 ? '' : core.toDecimalFormat(dtl.pbger)  }}</td>
          
          <!-- PHIC    -->
          <td class="text-right">{{ dtl.phher === 0 ? '' : core.toDecimalFormat(dtl.phher)  }}</td>
          <td class="text-right">{{ dtl.phhee === 0 ? '' : core.toDecimalFormat(dtl.phhee)  }}</td>
          <!-- Deductions   -->
          <td class="text-right">{{ dtl.mpl === 0 ? '' : core.toDecimalFormat(dtl.mpl)  }}</td>
          <td class="text-right">{{ dtl.sl4 === 0 ? '' : core.toDecimalFormat(dtl.sl4)  }}</td>
          <td class="text-right">{{ dtl.sl1 === 0 ? '' : core.toDecimalFormat(dtl.sl1)  }}</td>
          <td class="text-right">{{ dtl.spl === 0 ? '' : core.toDecimalFormat(dtl.spl)  }}</td>
          <td class="text-right">{{ dtl.el === 0 ? '' : core.toDecimalFormat(dtl.el)  }}</td>
          <td class="text-right">{{ dtl.sl3 === 0 ? '' : core.toDecimalFormat(dtl.sl3)  }}</td>
          <td class="text-right">{{ dtl.sal === 0 ? '' : core.toDecimalFormat(dtl.sal)  }}</td>
          <td class="text-right">{{ dtl.ep === 0 ? '' : core.toDecimalFormat(dtl.ep)  }}</td>
          <td class="text-right">{{ dtl.csc === 0 ? '' : core.toDecimalFormat(dtl.csc)  }}</td>
          <td class="text-right">{{ dtl.pml3 === 0 ? '' : core.toDecimalFormat(dtl.pml3)  }}</td>
          <td class="text-right">{{ dtl.cmf === 0 ? '' : core.toDecimalFormat(dtl.cmf)  }}</td>
          <td class="text-right">{{ dtl.pcl2 === 0 ? '' : core.toDecimalFormat(dtl.pcl2)  }}</td>
          <td class="text-right">{{ dtl.cs === 0 ? '' : core.toDecimalFormat(dtl.cs)  }}</td>
          <td class="text-right">{{ dtl.ssl === 0 ? '' : core.toDecimalFormat(dtl.ssl)  }}</td>
          <td class="text-right">{{ dtl.ssl2 === 0 ? '' : core.toDecimalFormat(dtl.ssl2)  }}</td>
          <td class="text-right">{{ dtl.scl === 0 ? '' : core.toDecimalFormat(dtl.scl)  }}</td>
          <td class="text-right">{{ dtl.pml === 0 ? '' : core.toDecimalFormat(dtl.pml)  }}</td>
          <td class="text-right">{{ dtl.pcl === 0 ? '' : core.toDecimalFormat(dtl.pcl)  }}</td>
          <td class="text-right">{{ dtl.pml2 === 0 ? '' : core.toDecimalFormat(dtl.pml2)  }}</td>
          <td class="text-right">{{ dtl.pai === 0 ? '' : core.toDecimalFormat(dtl.pai)  }}</td>
          <td class="text-right">{{ dtl.can === 0 ? '' : core.toDecimalFormat(dtl.can)  }}</td>
          <td class="text-right">{{ dtl.acc === 0 ? '' : core.toDecimalFormat(dtl.acc)  }}</td>
          <td class="text-right">{{ dtl.sd === 0 ? '' : core.toDecimalFormat(dtl.sd)  }}</td>
          <td class="text-right">{{ dtl.totalDeduction === 0 ? '' : core.toDecimalFormat(dtl.totalDeduction)  }}</td>
          <td class="text-right">{{ dtl.netShare === 0 ? '' : core.toDecimalFormat(dtl.netShare)  }}</td>

          <!-- <td class="text-center">{{ core.toDateFormat(dtl.scheduleDate) }}</td>
          <td class="text-center" :class="{ 'text-red': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
          <td class="text-center sched">{{ dtl.scheduleCode }}</td>
          <td class="text-center">{{ dtl.scheduleTimeIn }}</td>
          <td class="text-center">{{ dtl.scheduleTimeOut }}</td>
          <td class="text-center">{{ dtl.totalWorkingHour === 0 ? '' : dtl.totalWorkingHour }}</td> -->
          <!-- <td class="p-1">
            <div class="buttons" sm-1 mb-0>
              <button type="button" class="justify-between infoXXX info-light fw-22 btn-dtl-edit" @click="onEditSchedule(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
            </div>
          </td> -->
        </tr>
      </tbody> 
      
<tfoot>
  <tr>
    <td colspan="3" style="text-align:right;"><strong>Total:</strong></td>
    <!-- <td>{{ totals.totalWorkingDay.toFixed(2) }}</td> -->

         <td class="text-right">{{ totals.totalWorkingDay === 0 ? '' : core.toDecimalFormat(totals.totalWorkingDay)  }}</td>         
          <!-- <td class="text-right">{{ totals.allowanceDaily === 0 ? '' : core.toDecimalFormat(totals.allowanceDaily)  }}</td> -->
          <td class="text-right">{{ totals.totalWorkingHour === 0 ? '' : core.toDecimalFormat(totals.totalWorkingHour)  }}</td>
          <td class="text-right">{{ totals.payingDailyRate === 0 ? '' : core.toDecimalFormat(totals.payingDailyRate)  }}</td>

   
          
          <td class="text-right">{{ totals.basicPayOutRate === 0 ? '' : core.toDecimalFormat(totals.basicPayOutRate)  }}</td>
          <td class="text-right">{{ totals.otHour === 0 ? '' : core.toDecimalFormat(totals.otHour)  }}</td>
          <td class="text-right">{{ totals.otRate === 0 ? '' : core.toDecimalFormat(totals.otRate)  }}</td>

          
          <!-- RestDay  -->
          <td class="text-right">{{ totals.rdHour === 0 ? '' : core.toDecimalFormat(totals.rdHour)  }}</td>
          <td class="text-right">{{ totals.rdRate === 0 ? '' : core.toDecimalFormat(totals.rdRate)  }}</td>
          <!-- <td class="text-right">{{ totals.rdPremiumRate === 0 ? '' : core.toDecimalFormat(totals.rdPremiumRate)  }}</td>
          <td class="text-right">{{ totals.rdAmount === 0 ? '' : core.toDecimalFormat(totals.rdAmount)  }}</td> -->
          <td class="text-right">{{ totals.rdotHour === 0 ? '' : core.toDecimalFormat(totals.rdotHour)  }}</td>
          <td class="text-right">{{ totals.rdotRate === 0 ? '' : core.toDecimalFormat(totals.rdotRate)  }}</td>

          <!-- Special Holiday	  -->

          <td class="text-right">{{ totals.shHour === 0 ? '' : core.toDecimalFormat(totals.shHour)  }}</td>
          <td class="text-right">{{ totals.shRate === 0 ? '' : core.toDecimalFormat(totals.shRate)  }}</td>
          <!-- <td class="text-right">{{ totals.shPremiumRate === 0 ? '' : core.toDecimalFormat(totals.shPremiumRate)  }}</td>
          <td class="text-right">{{ totals.shAmount === 0 ? '' : core.toDecimalFormat(totals.shAmount)  }}</td> -->
          <td class="text-right">{{ totals.shotHour === 0 ? '' : core.toDecimalFormat(totals.shotHour)  }}</td>
          <td class="text-right">{{ totals.shotRate === 0 ? '' : core.toDecimalFormat(totals.shotRate)  }}</td>


          <!-- Legal Holiday Unworked -->

          <td class="text-right">{{ totals.hu100Hour === 0 ? '' : core.toDecimalFormat(totals.hu100Hour)  }}</td>
          <td class="text-right">{{ totals.hu100Rate === 0 ? '' : core.toDecimalFormat(totals.hu100Rate)  }}</td>

          <!-- Legal Holiday  -->
 
          <td class="text-right">{{ totals.lhHour === 0 ? '' : core.toDecimalFormat(totals.lhHour)  }}</td>
          <td class="text-right">{{ totals.lhRate === 0 ? '' : core.toDecimalFormat(totals.lhRate)  }}</td>
          <!-- <td class="text-right">{{ totals.lhPremiumRate === 0 ? '' : core.toDecimalFormat(totals.lhPremiumRate)  }}</td>
          <td class="text-right">{{ totals.lhAmount === 0 ? '' : core.toDecimalFormat(totals.lhAmount)  }}</td> -->
          <td class="text-right">{{ totals.lhotHour === 0 ? '' : core.toDecimalFormat(totals.lhotHour)  }}</td>
          <td class="text-right">{{ totals.lhotRate === 0 ? '' : core.toDecimalFormat(totals.lhotRate)  }}</td>

          <!-- Special Holiday Rest Day			  -->

          <td class="text-right">{{ totals.shrdHour === 0 ? '' : core.toDecimalFormat(totals.shrdHour)  }}</td>
          <td class="text-right">{{ totals.shrdRate === 0 ? '' : core.toDecimalFormat(totals.shrdRate)  }}</td>
          <!-- <td class="text-right">{{ totals.shrdPremiumRate === 0 ? '' : core.toDecimalFormat(totals.shrdPremiumRate)  }}</td>
          <td class="text-right">{{ totals.shrdAmount === 0 ? '' : core.toDecimalFormat(totals.shrdAmount)  }}</td> -->
          <td class="text-right">{{ totals.shrdotHour === 0 ? '' : core.toDecimalFormat(totals.shrdotHour)  }}</td>
          <td class="text-right">{{ totals.shrdotRate === 0 ? '' : core.toDecimalFormat(totals.shrdotRate)  }}</td>

          <!-- Legal Holiday Rest Day -->

          <td class="text-right">{{ totals.lhrdHour === 0 ? '' : core.toDecimalFormat(totals.lhrdHour)  }}</td>
          <td class="text-right">{{ totals.lhrdRate === 0 ? '' : core.toDecimalFormat(totals.lhrdRate)  }}</td>
          <!-- <td class="text-right">{{ totals.lhrdPremiumRate === 0 ? '' : core.toDecimalFormat(totals.lhrdPremiumRate)  }}</td>
          <td class="text-right">{{ totals.lhrdAmount === 0 ? '' : core.toDecimalFormat(totals.lhrdAmount)  }}</td> -->
          <td class="text-right">{{ totals.lhrdotHour === 0 ? '' : core.toDecimalFormat(totals.lhrdotHour)  }}</td>
          <td class="text-right">{{ totals.lhrdotRate === 0 ? '' : core.toDecimalFormat(totals.lhrdotRate)  }}</td>




          <!-- Extended Work -->
          <td class="text-right">{{ totals.extendedWorkRate === 0 ? '' : core.toDecimalFormat(totals.extendedWorkRate)  }}</td>

          <!-- Late -->
          <td class="text-right">{{ totals.lateMinuteP === 0 ? '' : core.toDecimalFormat(totals.lateMinuteP)  }}</td>
          <td class="text-right">{{ totals.lateMinuteRate === 0 ? '' : core.toDecimalFormat(totals.lateMinuteRate)  }}</td>

          <!-- Undertime -->
          <td class="text-right">{{ totals.undertimeMinuteP === 0 ? '' : core.toDecimalFormat(totals.undertimeMinuteP)  }}</td>
          <td class="text-right">{{ totals.undertimeMinuteRate === 0 ? '' : core.toDecimalFormat(totals.undertimeMinuteRate)  }}</td>

          <!-- Night Differential	 --> 
          <td class="text-right">{{ totals.ndHour === 0 ? '' : core.toDecimalFormat(totals.ndHour)  }}</td>
          <td class="text-right">{{ totals.ndRate === 0 ? '' : core.toDecimalFormat(totals.ndRate)  }}</td>

          <!-- Night Differential	OT -->  
          <td class="text-right">{{ totals.ndotHour === 0 ? '' : core.toDecimalFormat(totals.ndotHour)  }}</td>
          <td class="text-right">{{ totals.ndotRate === 0 ? '' : core.toDecimalFormat(totals.ndotRate)  }}</td>

          <!-- Night Differential	RD -->  
          <td class="text-right">{{ totals.rdndHour === 0 ? '' : core.toDecimalFormat(totals.rdndHour)  }}</td>
          <td class="text-right">{{ totals.rdndRate === 0 ? '' : core.toDecimalFormat(totals.rdndRate)  }}</td>

          <!-- Night Differential	RD OT -->            
          <td class="text-right">{{ totals.rdndotHour === 0 ? '' : core.toDecimalFormat(totals.rdndotHour)  }}</td>
          <td class="text-right">{{ totals.rdndotRate === 0 ? '' : core.toDecimalFormat(totals.rdndotRate)  }}</td>

          <!-- Night Differential SH	 -->   
          <td class="text-right">{{ totals.shHour === 0 ? '' : core.toDecimalFormat(totals.shHour)  }}</td>
          <td class="text-right">{{ totals.shRate === 0 ? '' : core.toDecimalFormat(totals.shRate)  }}</td>

          <!-- Night Differential	SH OT -->  
          <td class="text-right">{{ totals.shotHour === 0 ? '' : core.toDecimalFormat(totals.shotHour)  }}</td>
          <td class="text-right">{{ totals.shotRate === 0 ? '' : core.toDecimalFormat(totals.shotRate)  }}</td>

          <!-- Night Differential SH RD	 -->  
          <td class="text-right">{{ totals.shrdHour === 0 ? '' : core.toDecimalFormat(totals.shrdHour)  }}</td>
          <td class="text-right">{{ totals.shrdRate === 0 ? '' : core.toDecimalFormat(totals.shrdRate)  }}</td>

          <!-- Night Differential	SH RD OT -->  
          <td class="text-right">{{ totals.shrdotHour === 0 ? '' : core.toDecimalFormat(totals.shrdotHour)  }}</td>
          <td class="text-right">{{ totals.shrdotRate === 0 ? '' : core.toDecimalFormat(totals.shrdotRate)  }}</td>

          <!-- Night Differential	LH -->  

          <td class="text-right">{{ totals.lhHour === 0 ? '' : core.toDecimalFormat(totals.lhHour)  }}</td>
          <td class="text-right">{{ totals.lhRate === 0 ? '' : core.toDecimalFormat(totals.lhRate)  }}</td>

          <!-- Night Differential	LH OT -->  
          <td class="text-right">{{ totals.lhotHour === 0 ? '' : core.toDecimalFormat(totals.lhotHour)  }}</td>
          <td class="text-right">{{ totals.lhotRate === 0 ? '' : core.toDecimalFormat(totals.lhotRate)  }}</td>

          <!-- Night Differential	LH RD -->  
          <td class="text-right">{{ totals.lhrdHour === 0 ? '' : core.toDecimalFormat(totals.lhrdHour)  }}</td>
          <td class="text-right">{{ totals.lhrdRate === 0 ? '' : core.toDecimalFormat(totals.lhrdRate)  }}</td>

          <!-- Night Differential	LH RD OT -->  

          <td class="text-right">{{ totals.lhrdotHour === 0 ? '' : core.toDecimalFormat(totals.lhrdotHour)  }}</td>
          <td class="text-right">{{ totals.lhrdotRate === 0 ? '' : core.toDecimalFormat(totals.lhrdotRate)  }}</td>


          <!-- Other Allowance -->  
          <td class="text-right">{{ totals.otherAllowance === 0 ? '' : core.toDecimalFormat(totals.otherAllowance)  }}</td>

          <!-- Billable Shares -->  
          <td class="text-right">{{ totals.ssser === 0 ? '' : core.toDecimalFormat(totals.ssser)  }}</td>
          <td class="text-right">{{ totals.pfer === 0 ? '' : core.toDecimalFormat(totals.pfer)  }}</td>
          <td class="text-right">{{ totals.sssec === 0 ? '' : core.toDecimalFormat(totals.sssec)  }}</td>
          <td class="text-right">{{ totals.pbger === 0 ? '' : core.toDecimalFormat(totals.pbger)  }}</td>
          <td class="text-right">{{ totals.phher === 0 ? '' : core.toDecimalFormat(totals.phher)  }}</td>

          <!-- Gross    -->
          <td class="text-right">{{ totals.grossPay === 0 ? '' : core.toDecimalFormat(totals.grossPay)  }}</td>
          <td class="text-right">{{ totals.grossShare === 0 ? '' : core.toDecimalFormat(totals.grossShare)  }}</td>
          <td class="text-right">{{ totals.totalShare === 0 ? '' : core.toDecimalFormat(totals.totalShare)  }}</td>

          <!-- SSS    -->
          <td class="text-right">{{ totals.ssser === 0 ? '' : core.toDecimalFormat(totals.ssser)  }}</td>
          <td class="text-right">{{ totals.sssee === 0 ? '' : core.toDecimalFormat(totals.sssee)  }}</td>
          <td class="text-right">{{ totals.sssec === 0 ? '' : core.toDecimalFormat(totals.sssec)  }}</td>
  
          <!-- PF    -->
          <td class="text-right">{{ totals.pfer === 0 ? '' : core.toDecimalFormat(totals.pfer)  }}</td>
          <td class="text-right">{{ totals.pfee === 0 ? '' : core.toDecimalFormat(totals.pfee)  }}</td>

          <!-- HDMF    -->
          <td class="text-right">{{ totals.pbger === 0 ? '' : core.toDecimalFormat(totals.pbger)  }}</td>
          <td class="text-right">{{ totals.pbgee === 0 ? '' : core.toDecimalFormat(totals.pbgee)  }}</td>
          
          <!-- PHIC    -->
          <td class="text-right">{{ totals.phher === 0 ? '' : core.toDecimalFormat(totals.phher)  }}</td>
          <td class="text-right">{{ totals.phhee === 0 ? '' : core.toDecimalFormat(totals.phhee)  }}</td>
          <!-- Deductions   -->
          <td class="text-right">{{ totals.mpl === 0 ? '' : core.toDecimalFormat(totals.mpl)  }}</td>
          <td class="text-right">{{ totals.sl4 === 0 ? '' : core.toDecimalFormat(totals.sl4)  }}</td>
          <td class="text-right">{{ totals.sl1 === 0 ? '' : core.toDecimalFormat(totals.sl1)  }}</td>
          <td class="text-right">{{ totals.spl === 0 ? '' : core.toDecimalFormat(totals.spl)  }}</td>
          <td class="text-right">{{ totals.el === 0 ? '' : core.toDecimalFormat(totals.el)  }}</td>
          <td class="text-right">{{ totals.sl3 === 0 ? '' : core.toDecimalFormat(totals.sl3)  }}</td>
          <td class="text-right">{{ totals.sal === 0 ? '' : core.toDecimalFormat(totals.sal)  }}</td>
          <td class="text-right">{{ totals.ep === 0 ? '' : core.toDecimalFormat(totals.ep)  }}</td>
          <td class="text-right">{{ totals.csc === 0 ? '' : core.toDecimalFormat(totals.csc)  }}</td>
          <td class="text-right">{{ totals.pml3 === 0 ? '' : core.toDecimalFormat(totals.pml3)  }}</td>
          <td class="text-right">{{ totals.cmf === 0 ? '' : core.toDecimalFormat(totals.cmf)  }}</td>
          <td class="text-right">{{ totals.pcl2 === 0 ? '' : core.toDecimalFormat(totals.pcl2)  }}</td>
          <td class="text-right">{{ totals.cs === 0 ? '' : core.toDecimalFormat(totals.cs)  }}</td>
          <td class="text-right">{{ totals.ssl === 0 ? '' : core.toDecimalFormat(totals.ssl)  }}</td>
          <td class="text-right">{{ totals.ssl2 === 0 ? '' : core.toDecimalFormat(totals.ssl2)  }}</td>
          <td class="text-right">{{ totals.scl === 0 ? '' : core.toDecimalFormat(totals.scl)  }}</td>
          <td class="text-right">{{ totals.pml === 0 ? '' : core.toDecimalFormat(totals.pml)  }}</td>
          <td class="text-right">{{ totals.pcl === 0 ? '' : core.toDecimalFormat(totals.pcl)  }}</td>
          <td class="text-right">{{ totals.pml2 === 0 ? '' : core.toDecimalFormat(totals.pml2)  }}</td>
          <td class="text-right">{{ totals.pai === 0 ? '' : core.toDecimalFormat(totals.pai)  }}</td>
          <td class="text-right">{{ totals.can === 0 ? '' : core.toDecimalFormat(totals.can)  }}</td>
          <td class="text-right">{{ totals.acc === 0 ? '' : core.toDecimalFormat(totals.acc)  }}</td>
          <td class="text-right">{{ totals.sd === 0 ? '' : core.toDecimalFormat(totals.sd)  }}</td>
          <td class="text-right">{{ totals.totalDeduction === 0 ? '' : core.toDecimalFormat(totals.totalDeduction)  }}</td>
          <td class="text-right">{{ totals.netShare === 0 ? '' : core.toDecimalFormat(totals.netShare)  }}</td>



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

import { saveAs } from 'file-saver';


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
  name: "ars0090",

  data() {
    return {
      
      timekeeping: {
        timekeepingPayOutId: 0,
        trxId: 0,
    
        timekeepingSheetId: 0,     
        timekeepingPayOutStatusId: 0,
        clientPayGroupId:0,
        clientPayGroupName:'',
        remarks:'',
        cutOffStartDate: null,
        cutOffEndDate: null,
        payOutDate: null,
        remarks: '',
        lockId: "",
      },

      oldTimekeeping: [],
      logs: [],
      isLogVisible: false,

      memberRequest: [],

      schedules: [],
      payables: [],

      isScheduleEditorVisible: false,

      schedule: {
        payOutMemberId: 0,
        timekeepingPayOutId: 0,
        memberId: 0,
        memberName: '',
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

        ssser:0,
        sssee:0,
        sssec:0,
        pfer:0,
        pfee:0,
        pbger:0,
        pbgee:0,
        phher:0,
        phhee:0,

        grossPay:0,
        grossShare:0,
        totalShare:0,
        

        mpl:0,
        sl4:0,
        sl1:0,
        spl:0,
        el:0,
        sl3:0,
        saL:0,
        ep:0,
        csc:0,
        pml3:0,
        cmf:0,
        pcl2:0,
        cs:0,
        ssl:0,
        ssl2:0,
        scl:0,
        pml:0,
        pcl:0,
        pml2:0,
        pai:0,
        cas:0,
        acc:0,
        sd:0,
        totalDeduction:0,
        netShare:0,
        lateMinuteP:0,
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


    lateMinuteP: 0,
    undertimeMinuteP: 0,

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
    ssser: 0,
    sssee: 0,
    sssec: 0,
    pfer: 0,
    pfee: 0,
    pbger: 0,
    pbgee: 0,
    phher: 0,
    phhee: 0,

    grossPay: 0,
    grossShare: 0,
    totalShare: 0,
    mpl: 0,
    sl4: 0,
    sl1: 0,
    spl: 0,
    el: 0,
    sl3: 0,
    saL: 0,
    ep: 0,
    csc: 0,
    pml3: 0,
    cmf: 0,
    pcl2: 0,
    cs: 0,
    ssl: 0,
    ssl2: 0,
    scl: 0,
    pml: 0,
    pcl: 0,
    pml2: 0,
    pai: 0,
    cas: 0,
    acc: 0,
    sd: 0,
    totalDeduction: 0,
    netShare: 0
    

    },

    uri :'data:application/vnd.ms-excel;base64,',
    template:'<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
    base64: function(s){ return window.btoa(unescape(encodeURIComponent(s))) },
    format: function(s, c) { return s.replace(/{(\w+)}/g, function(m, p) { return c[p]; }) },

  perchu100: 0,
  percot: 0,
  percrd: 0,
  percrdot: 0,
  percsh: 0,
  percshot: 0,
  percshrd: 0,
  percshrdot: 0,
  perclh: 0,
  perclhot: 0,
  perclhrd: 0,
  perclhrdot: 0,
  perclhsh: 0,
  perclhshrd: 0,
  percdrh: 0,
  percdrhot: 0,
  percdrhrd: 0,
  percdrhrdot: 0,
  percnd: 0,
  percndot: 0,
  percrdnd: 0,
  percrdndot: 0,
  percshnd: 0,
  percshndot: 0,
  percshrdnd: 0,
  percshrdndot: 0,
  perclhnd: 0,
  perclhndot: 0,
  perclhrdnd: 0,
  perclhrdndot: 0

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
    
    getTableWrapperStyle(rowCount) {
      const rowHeight = 10;
      const headerHeight = 50;
      const footerHeight = 40;

      const totalContentHeight = headerHeight + (rowCount * rowHeight) + footerHeight;
      const maxAllowedHeight = window.innerHeight * 1.5;
    },
    onClickTimekeepingSheet() { 
      const me = this;
      let route = {
        name: "pay0040",
        query: {
          timekeepingSheetId: me.timekeeping.timekeepingSheetId,
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
            me.timekeeping.timekeepingPayOutStatusId = 2;
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
            me.timekeeping.timekeepingPayOutStatusId = 3;
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

    lateMinuteP: 0,
    undertimeMinuteP: 0,

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
    shRDAmount: 0,
    lhRDAmount: 0,
    extendedWorkRate: 0,
    otherAllowance: 0,
    
    ssser: 0,
    sssee: 0,
    sssec: 0,
    pfer: 0,
    pfee: 0,
    pbger: 0,
    pbgee: 0,
    phher: 0,
    phhee: 0,
        
    
    grossPay: 0,
    grossShare: 0,
    totalShare: 0,
    mpl: 0,
    sl4: 0,
    sl1: 0,
    spl: 0,
    el: 0,
    sl3: 0,
    saL: 0,
    ep: 0,
    csc: 0,
    pml3: 0,
    cmf: 0,
    pcl2: 0,
    cs: 0,
    ssl: 0,
    ssl2: 0,
    scl: 0,
    pml: 0,
    pcl: 0,
    pml2: 0,
    pai: 0,
    cas: 0,
    acc: 0,
    sd: 0,
    totalDeduction: 0,
    netShare: 0
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

me.getPayOutDetails().then(
  
(data) => {

  me.schedules = data;
  me.perchu100 =  me.schedules[0].hU100Perc;
  me.percot =  me.schedules[0].otPerc;
  me.percrd =  me.schedules[0].rdPerc;
  me.percrdot=  me.schedules[0].rdotPerc;
  me.percsh=  me.schedules[0].shPerc;
  me.percshot=  me.schedules[0].shotPerc;
  me.percshrd=  me.schedules[0].shrdPerc;
  me.percshrdot=  me.schedules[0].shrdotPerc;
  me.perclh=  me.schedules[0].lhPerc;
  me.perclhot=  me.schedules[0].lhotPerc;
  me.perclhrd=  me.schedules[0].lhrdPerc;
  me.perclhrdot=  me.schedules[0].lhrdotPerc;
  me.perclhsh=  me.schedules[0].lhshPerc;
  me.perclhshrd=  me.schedules[0].lhshrdPerc;
  me.percdrh=  me.schedules[0].drhPerc;
  me.percdrhot=  me.schedules[0].drhotPerc;
  me.percdrhrd=  me.schedules[0].drhrdPerc;
  me.percdrhrdot=  me.schedules[0].drhrdotPerc;
  me.percnd=  me.schedules[0].ndPerc;
  me.percndot=  me.schedules[0].ndotPerc;
  me.percrdnd=  me.schedules[0].rdndPerc;
  me.percrdndot=  me.schedules[0].rdndotPerc;
  me.percshnd=  me.schedules[0].shndPerc;
  me.percshndot=  me.schedules[0].shndotPerc;
  me.percshrdnd=  me.schedules[0].shrdndPerc;
  me.percshrdndot=  me.schedules[0].shrdndPerc;
  me.perclhnd=  me.schedules[0].lhndPerc;
  me.perclhndot=  me.schedules[0].lhndotPerc;
  me.perclhrdnd=  me.schedules[0].lhrdndPerc;
  me.perclhrdndot=  me.schedules[0].lhrdndotPerc;


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

    onTimekeepingSheetIdChanged () {
      const me = this;

      me.getTimekeepingSheetInfo().then(
        (data) => {
          me.schedules = me.core.convertDates(data.sheet);
         me.setFocus("payOutDate");
         me.onProcess();

        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },

    onTimekeepingSheetIdChanging (e) {
      e.callback = this.timekeepingSheetIdCallback;
    },

    timekeepingSheetIdCallback (e) {
      const me = this;
      let filter = "TimekeepingSheetId='" + e.proposedValue + "'";
      return getList('dbo.QPayTimekeepingSheet', 'TimekeepingSheetId, ClientPayGroupId, ClientPayGroupName, CutOffStartDate, CutOffEndDate, Remarks', '', filter).then(
        sheet => {
          if (sheet && sheet.length) {
            me.timekeeping.timekeepingSheetId = sheet[0].timekeepingSheetId;
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

    onTimekeepingSheetIdLostFocus () {
      const me = this;

      if (!me.timekeeping.timekeepingSheetId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingSheetIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];
      me.timekeeping.timekeepingSheetId = data.timekeepingSheetId;
      me.timekeeping.clientPayGroupId = data.clientPayGroupId;
      me.timekeeping.clientPayGroupName = data.clientPayGroupName;
      me.timekeeping.cutOffStartDate = this.core.convertDates(data).cutOffStartDate;
      me.timekeeping.cutOffEndDate = this.core.convertDates(data).cutOffEndDate;
      me.timekeeping.remarks = data.remarks;
      me.setFocus("payOutDate");
      me.onTimekeepingSheetIdChanged();

    },


    getTargetPath() {
      const me = this,
        q = {};

      if (me.timekeeping.timekeepingPayOutId) {
        q.timekeepingPayOutId = me.timekeeping.timekeepingPayOutId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("timekeepingPayOutId" in q && me.core.isInteger(q.timekeepingPayOutId)) {
       
        me.timekeeping.timekeepingPayOutId = parseInt(q.timekeepingPayOutId);
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
       
          if (me.timekeeping.timekeepingPayOutId < 0) {
            return Promise.resolve(null);
          }
          
          return me.getTimekeeping();
        })
        
        .then((timekeeping) => {
          me.stopWait(wait);
          if (timekeeping && timekeeping.timekeeping.length) {
            me.setModels(timekeeping);
          } else {
            if (me.timekeeping.timekeepingPayOutId > -1) {
              let message = "Timekeeping Pay Out ID '<b>" + me.timekeeping.timekeepingPayOutId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
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


            me.timekeeping.payOutDate= me.today;
            
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
      me.payables = info.payables;

      me.refreshTotal();
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldSchedules = JSON.stringify(me.schedules);
    },

    getPayOutDetails() {
      const me = this;
      
      return get("api/timekeeping-pay-out-process/" + me.timekeeping.timekeepingSheetId + "/" + me.timekeeping.clientPayGroupId);
    },

    
    getTimekeepingSheetInfo() {
      const me = this;
      return get("api/timekeeping-sheet-id/" + me.timekeeping.timekeepingSheetId + "/" + me.timekeeping.clientPayGroupId);
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingPayOutId < 0) {
        return Promise.resolve(null);
      }

      return get("api/timekeeping-pay-out/" + this.timekeeping.timekeepingPayOutId);
    },

    getReferences() {
      const me = this;
      if (me.schedules.length) {
        return Promise.resolve(true);
      }

      return get("api/references/ars0090");
    },

    getChangeLog(log) {
      return get("api/timekeeping-records/" + log + "/" +  this.timekeeping.timekeepingPayOutId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/timekeeping-pay-out/" + currentUserId, options);
    },


    createPayableRecord() {
      const payload = this.getPayableApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/pay-out-payable/"  + this.timekeeping.timekeepingPayOutId + "/" + currentUserId, options);
    },


    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeeping-pay-out/" + this.timekeeping.timekeepingPayOutId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeeping-pay-out/" + this.timekeeping.timekeepingPayOutId + "/" + timekeeping.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        timekeeping = {};

      Object.assign(timekeeping, me.timekeeping);
      timekeeping.schedules = me.schedules;
      return timekeeping;
    },

    getPayableApiPayload() {
      const me = this,
        payables = {};

      Object.assign(payables, me.payables);
      payables.payables = me.payables;
      return payables;
    },



    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "timekeeping","schedules","schedule",

      );
      dc.keyField = "timekeepingPayOutId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
      (this.docs = []),
      this.isCancelling = false;

      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onTimekeepingPayOutIdChanging(e) {
      e.callback = this.timekeepingPayOutIdCallback;
    },

    timekeepingPayOutIdCallback(e) {
      e.message = "Timekeeping Pay Out ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.PayTimekeepingPayOut", "timekeepingPayOutId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onTimekeepingPayOutIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onTimekeepingPayOutIdLostFocus() {
      const me = this;

      if (!me.timekeeping.timekeepingPayOutId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingPayOutIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.timekeeping.timekeepingPayOutId = data.timekeepingPayOutId;

      me.loadData();
      me.replaceUrl();

    },



    onPayableSubmit(nextRoute) {
      const me = this;
      
      let promise,
        message,
        wait = me.wait(),
        isNew = me.isNew();
        promise = me.createPayableRecord();

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (isNew && typeof success === "number" && success > 0) {
              me.timekeeping.timekeepingPayOutId = success;
            }

            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";

              if (me.isCancelling) {
                message = "Pay Out Trx ID # '" + me.timekeeping.timekeepingPayOutId + "' cancelled."
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
              me.timekeeping.timekeepingPayOutId = success;
            }

            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";

              if (me.isCancelling) {
                message = "Pay Out Trx ID # '" + me.timekeeping.timekeepingPayOutId + "' cancelled."
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
          "timekeepingSheetId",
          "payOutDate",
        );

        me.setDisplayMode(
          "clientPayGroupName",
          "clientPayGroupId",
          "cutOffStartDate",
          "cutOffEndDate",
          "remarks",

        );

        me.setFocus("timekeepingSheetId");
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

    isPostToPayable () {
      return this.timekeeping.trxId ==0 ;
    },

    isCanceled () {
      return this.timekeeping.timekeepingPayOutStatusId == 3;
    },

    isPosted () {
      return this.timekeeping.timekeepingPayOutStatusId == 2;
    },
       
    isSubmitted () {
      return this.timekeeping.timekeepingPayOutStatusId == 1;
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
 width: 1200%;
 height: 50vh;
 
}
table.scroller {
 min-width: 50vw; 
 border-collapse: collapse;
  width: 1200%;

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

thead{
  position: sticky;
  top:0;
  z-index: 20;

}

thead{
  position: sticky;
  top:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 20;
  border-right: 2px solid grey;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 20;
  border-right: 2px solid grey;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 227px;
  z-index: 20;
  border: 2px solid red;
  width: 20rem;
}
tbody{
  background-color: #fffbb3;
}
tbody th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 20;
  border-right: 2px solid grey;
  width: 8rem;
}
tbody th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 20;
  border-right: 2px solid grey;
  width: 10rem;
}
tbody th:nth-child(3) {
  position: sticky;
  left: 227px;
  z-index: 20;
  border: 2px solid red;
  width: 20rem;
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
  transition: all 0.3s ease-in-out;
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
 width: 1200%;
 height: 50vh;
 
}
table.scroller {
 min-width: 50vw; 
 border-collapse: collapse;
  width: 1200%;

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

thead{
  position: sticky;
  top:0;
  z-index: 20;

}

thead{
  position: sticky;
  top:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 20;
  border-right: 2px solid grey;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 20;
  border-right: 2px solid grey;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 227px;
  z-index: 20;
  border: 2px solid red;
  width: 20rem;
}
tbody{
  background-color: #fffbb3;
}
tbody th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 20;
  border-right: 2px solid grey;
  width: 8rem;
}
tbody th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 20;
  border-right: 2px solid grey;
  width: 10rem;
}
tbody th:nth-child(3) {
  position: sticky;
  left: 227px;
  z-index: 20;
  border: 2px solid red;
  width: 20rem;
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
  transition: all 0.3s ease-in-out;
}
}
</style>
