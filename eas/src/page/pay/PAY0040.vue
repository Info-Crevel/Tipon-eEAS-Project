// Daily Time Record Generation

<template>
  <section class="container p-0 h-100" :key="ts">
    <sym-form
      id="pay0040" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">
       
      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="timekeeping.timekeepingSheetId" :caption-width="37" :input-width="40" caption="Sheet ID" lookupId="PayTimekeepingSheet" @lostfocus="onTimekeepingSheetIdLostFocus" @changing="onTimekeepingSheetIdChanging" @changed="onTimekeepingSheetIdChanged" @searchresult="onTimekeepingSheetIdSearchResult" ></sym-int>

          <div class="buttons d-flex justify-start  mb-2" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit"> <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>
        </div>
        
      </div>

        <div class="box-field">

          <div class="app-box-style box-main-container ">
       
            <div class="box-1 gap">
                <sym-int v-model="timekeeping.clientPayGroupId" :caption-width="33" :input-width="40" align="left" caption="Pay ID" lookupId="ArsClientPayGroupSheet" @lostfocus="onClientPayGroupIdLostFocus" @changing="onClientPayGroupIdChanging" @changed="onClientPayGroupIdChanged" @searchresult="onClientPayGroupIdSearchResult"></sym-int>
                <sym-text v-model="timekeeping.clientPayGroupName" align="left" :caption-width="37" caption="Pay Group Name" ></sym-text>          
                <sym-date v-model="timekeeping.cutOffStartDate" align="left" :caption-width="37" caption="Start" ></sym-date>
                <sym-date v-model="timekeeping.cutOffEndDate" align="left" :caption-width="37" caption="End" ></sym-date>

              </div>
              <div class="box-2 gap mb-0">
                <sym-text v-model="timekeeping.remarks" align="left" :caption-width="33" caption="Remarks" ></sym-text>
              </div>


             

              <input ref="excelInput" type="file" accept=".xlsx, .xls" style="display: none" @change="handleFileUpload"/>
              
              <div v-if="isReading" class="loading-bar">
                <i class="fa fa-spinner fa-spin"></i> Reading Excel file...
              </div>

          </div>

           <div class=" box-6 gap d">
             <button type="button" :class="logButtonClass" class="applicant-btn w-10 secondary" @click="onCalculate" v-show="timekeeping.manualUploadFlag==0 && timekeeping.manualEditFlag==0"> 
              <i class="fa fa-calculator"></i><span class="bold">Compute </span> 
            </button>

            <button type="button" :class="logButtonClass" class="applicant-btn w-10 info" @click="onApplyOffset" v-show="timekeeping.manualUploadFlag==0 && timekeeping.manualEditFlag==0"> 
              <i class="fa fa-calculator"></i><span class="bold">Apply Off Set </span> 
            </button>              

            <button type="button" :class="logButtonClass" class="applicant-btn w-10 success-light" @click="onUploadManual" v-show="timekeeping.manualEditFlag==0" > 
              <i class="fa fa-folder-open-o fa-lg"></i><span class="bold">Upload Data </span> 
            </button>              
              </div>

          

          <div class="app-box-style caption-white">

              <div class="text-center border-light curved p-1 info mt-2">

                <span class="serif lg-3">Member/s: {{ filteredGroupedSchedules.length }}</span>
              </div>
              <div class="filter-container">
                <sym-text v-model="searchMemberName" align="left" :caption-width="35" :input-width="100" list="memberNames" caption="Member Name"></sym-text>    
              <datalist id="memberNames"><option v-for="item in memberList" :key="item.memberName" :value="item.memberName" @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist> 


                <button type="button" class="success w-100 text-center lg-2 shadow-soft upload-btn" @click="generateExcelReport" ><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>

              </div>
          
          <div>
            <div>
             
            
              <div class="main-scroll-wrapper mt-0 mb-0">
            <div
            v-for="(group, index) in filteredGroupedSchedules"
            :key="group.memberId"
            class="member-block app-box-style mb-1 "
            style="margin-bottom: 2rem;"
          >
            <div class="text-info-container info-light">
              
                 <div class="text-info-1">
                <div class="label-row">
                  <span class="label">Member ID </span>
                  <div class="label-value">: &nbsp; &nbsp;{{ group.memberId }}</div>
                </div>

                <div class="label-row">
                  <span class="label-2">Member Name &nbsp;</span>
                  <div class="label-value">:&nbsp; &nbsp;{{ group.memberName }}</div>
                </div>

                <div class="label-row">
                  <span class="label-3"> Department </span>
                  <div class="label-value">:&nbsp; &nbsp;{{ group.departmentName }}</div>
                </div>
              </div>
              <div class="text-info-2">

                <div class="label-row">
                  <span class="label-4">Overtime Form </span>
                  <div class="label-value">:{{ group.otRemarks }}</div>

                <!-- <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDoc(group, index)"> <i class="fa fa-upload fa-lg"></i><span>Upload</span> </button> -->

                          


                </div>
                
                <div class="btns ">
                  <button type="button" :class="logButtonClass" class="info "  @click="onEditDoc(group, index)">            
                    <i class="fa fa-upload fa-lg"></i><span class="bold">Upload</span> 
                  </button>
                    <div v-if="group.otFileName"> 
                      <a class="button success sm-4 " :href="group.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                    </div> 
                    
                </div>
            </div>
            </div>


  <hr class="mt-0"/>

          <div class="table-scroll-wrapper ">    
  <!-- compute -->
            <table v-show = "!timekeeping.applyOffSetFlag" class="table-scroll striped-even mb-0"  ref="table" id="loremTable" summary="lorem ipsum sit amet" rules="groups" frame="hsides" border="2" :style="getTableWrapperStyle(group.entries.length)">
   
           
              <!-- <table class="striped-even mb-0" style="width: 100%;"> -->
              <thead class="info-light">
                <tr>
                  <th class="text-center" >Action</th>
                  <th>Date</th>
                  <th>Day Type</th>
                  <th>Schedule</th>
                  <!-- <th>Approved Form</th> -->
                  <!-- <th>Payable To Member</th>
                  <th>Billable To Client</th> -->
                  <th @click="toggleAllPayableToMember(group.entries, group.memberId)" style="cursor: pointer;" class="payable">
                    <i class="fa fa-check text-success "></i>
                    Payable To Member
                  </th>

                  <th @click="toggleAllBillableToClient(group.entries)" style="cursor: pointer;" class="billable">
                       <i class="fa fa-check text-success"></i>
                    Billable To Client
                  </th>
                  <th class="text-center remarks">Remarks</th>
                  <th class="text-center">Time In</th>
                  <th class="text-center">Time Out</th>
                  <th class="text-center">Reg Hrs</th>
                  <th class="text-center">OT</th>
                  <th class="text-center">ND</th>
                  <th class="text-center">ND OT</th>
                  <th class="text-center">Late/mins</th>
                  <th class="text-center">Undertime/mins</th>
                                    
                  <th class="text-center">RD</th>
                  <th class="text-center">RDOT</th>
                  <th class="text-center">RDND</th>
                  <th class="text-center">RDNDOT</th>
                  <th class="text-center">SH</th>
                  <th class="text-center">SHOT</th>
                  <th class="text-center">SHND</th>
                  <th class="text-center">SHNDOT</th>
                  <th class="text-center">SHRD</th>
                  <th class="text-center">SHRDOT</th>
                  <th class="text-center">SHRDND</th>
                  <th class="text-center">SHRDNDOT</th>
                  <th class="text-center">LHRD</th>
                  <th class="text-center">LHRDOT</th>
                  <th class="text-center">LHRDND</th>
                  <th class="text-center">LHRDNDOT</th>
                  <th class="text-center">LH</th>
                  <th class="text-center">LHOT</th>
                  <th class="text-center">LHND</th>
                  <th class="text-center">LHNDOT</th>
                  <th class="text-center">LHSH</th>
                  <!-- here   -->
                  <th class="text-center">LHSHOT</th>
                  <th class="text-center">LHSHND</th>
                  <th class="text-center">LHSHNDOT</th>
                  <!-- here   -->
                  
                  <th class="text-center">LHSHRD</th>
                  
                  <!-- here   -->
                  <th class="text-center">LHSHRDOT</th>
                  <th class="text-center">LHSHRDND</th>
                  <th class="text-center">LHSHRDNDOT</th>
                  <!-- here   -->

                  <th class="text-center">HU-100</th>
                  <th class="text-center">DRH</th>
                  <th class="text-center">DRHOT</th>
<!-- here   -->
<th class="text-center">DRHND</th>
<th class="text-center">DRHNDOT</th>

                  <th class="text-center">DRHRD</th>
                  <th class="text-center">DRHRDOT</th>
               <!-- here   --> 
<th class="text-center">DRHRDND</th>
<th class="text-center">DRHRDNDOT</th>


                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(dtl, index) in group.entries"
                  :key="`${group.memberId}-${index}`" :class="getNoDataColor(dtl.scheduleCode, dtl.scheduleTimeIn, dtl.scheduleTimeOut )"
                >
                                      <td class="p-1">
            <div class="buttons" sm-1 mb-0>

                    <button v-show="timekeeping.manualEditFlag==0" @click="onEditSchedule(dtl, dtl.flatIndex)" class="btn-dtl-edit info" type="button" title="Edit Schedule" >
                      <i class="fa fa-edit"></i>
                    </button>

                    <button @click="onEditManualSchedule(dtl, dtl.flatIndex)" class="btn-dtl-edit danger" type="button" title="Manual Entry Time">
                      <i class="fa fa-edit"></i>
                    </button>
            </div>                   
                  </td>
                  <td>{{ dtl.scheduleDate }}</td>
                  <td :class="{ 'bold italic': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
                  <!-- <td>{{ dtl.scheduleCode }}</td> -->
                  <td  :class="{ 'bold': dtl.timekeepingScheduleId === 999999999 || dtl.timekeepingScheduleId === 999999998 }">
                    {{ dtl.scheduleCode }}
                  </td>
                  <td class="text-center">
                    <i
                      :class="getBooleanIconClass(dtl.payableToMemberFlag)"
                      @click="togglePayableToMemberFlag(dtl)"
                      style="cursor: pointer;"
                    ></i>
                  </td>

                  <td class="text-center">
                    <i
                      :class="getBooleanIconClass(dtl.billableToClientFlag)"
                      @click="toggleBillableToClientFlag(dtl)"
                      style="cursor: pointer;"
                    ></i>
                  </td>

                  <td class="text-center">{{ dtl.remarks }}</td>
                  <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeIn) }}</td>
                  <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeOut) }}</td>
                  <td class="text-center">{{ dtl.regularHour }}</td>
                  <td class="text-center">{{ dtl.overtimeHour }}</td>
                  <td class="text-center">{{ dtl.nd }}</td>
                  <td class="text-center">{{ dtl.ndot }}</td>
                  <td class="text-center">{{ core.toDecimalFormat(dtl.lateMinute) }}</td>
                  <td class="text-center">{{ dtl.undertimeMinute }}</td>

                  <th class="text-center">{{ dtl.rd === 0 ? '' : dtl.rd }}</th>
                  <th class="text-center">{{ dtl.rdot === 0 ? '' : dtl.rdot }}</th>
                  <th class="text-center">{{ dtl.rdnd === 0 ? '' : dtl.rdnd }}</th>
                  <th class="text-center">{{ dtl.rdndot === 0 ? '' : dtl.rdndot }}</th>
                  <th class="text-center">{{ dtl.sh === 0 ? '' : dtl.sh }}</th>
                  <th class="text-center">{{ dtl.shot === 0 ? '' : dtl.shot }}</th>
                  <th class="text-center">{{ dtl.shnd === 0 ? '' : dtl.shnd }}</th>
                  <th class="text-center">{{ dtl.shndot === 0 ? '' : dtl.shndot }}</th>
                  <th class="text-center">{{ dtl.shrd === 0 ? '' : dtl.shrd }}</th>
                  <th class="text-center">{{ dtl.shrdot === 0 ? '' : dtl.shrdot }}</th>
                  <th class="text-center">{{ dtl.shrdnd === 0 ? '' : dtl.shrdnd }}</th>
                  <th class="text-center">{{ dtl.shrdndot === 0 ? '' : dtl.shrdndot }}</th>
                  <th class="text-center">{{ dtl.lhrd === 0 ? '' : dtl.lhrd }}</th>
                  <th class="text-center">{{ dtl.lhrdot === 0 ? '' : dtl.lhrdot }}</th>
                  <th class="text-center">{{ dtl.lhrdnd === 0 ? '' : dtl.lhrdnd }}</th>
                  <th class="text-center">{{ dtl.lhrdndot === 0 ? '' : dtl.lhrdndot }}</th>
                  <th class="text-center">{{ dtl.lh === 0 ? '' : dtl.lh }}</th>
                  <th class="text-center">{{ dtl.lhot === 0 ? '' : dtl.lhot }}</th>
                  <th class="text-center">{{ dtl.lhnd === 0 ? '' : dtl.lhnd }}</th>
                  <th class="text-center">{{ dtl.lhndot === 0 ? '' : dtl.lhndot }}</th>
                  
                  <th class="text-center">{{ dtl.lhsh === 0 ? '' : dtl.lhsh }}</th>
                  <th class="text-center">{{ dtl.lhshot === 0 ? '' : dtl.lhshot }}</th>
                  <th class="text-center">{{ dtl.lhshnd === 0 ? '' : dtl.lhshnd }}</th>
                  <th class="text-center">{{ dtl.lhshndot === 0 ? '' : dtl.lhshndot }}</th>

                  <th class="text-center">{{ dtl.lhshrd === 0 ? '' : dtl.lhshrd }}</th>
                  <th class="text-center">{{ dtl.lhshrdot === 0 ? '' : dtl.lhshrdot }}</th>
                  <th class="text-center">{{ dtl.lhshrdnd === 0 ? '' : dtl.lhshrdnd }}</th>
                  <th class="text-center">{{ dtl.lhshrdndot === 0 ? '' : dtl.lhshrdndot }}</th>

                  <th class="text-center">{{ dtl.hu100 === 0 ? '' : dtl.hu100 }}</th>
                  <th class="text-center">{{ dtl.drh === 0 ? '' : dtl.drh }}</th>
                  <th class="text-center">{{ dtl.drhot === 0 ? '' : dtl.drhot }}</th>
                  <th class="text-center">{{ dtl.drhnd === 0 ? '' : dtl.drhnd }}</th>
                  <th class="text-center">{{ dtl.drhndot === 0 ? '' : dtl.drhndot }}</th>


                  <th class="text-center">{{ dtl.drhrd === 0 ? '' : dtl.drhrd }}</th>
                  <th class="text-center">{{ dtl.drhrdot === 0 ? '' : dtl.drhrdot }}</th>
                  <th class="text-center">{{ dtl.drhrdnd === 0 ? '' : dtl.drhrdnd }}</th>
                  <th class="text-center">{{ dtl.drhrdndot === 0 ? '' : dtl.drhrdndot }}</th>


                </tr>
              </tbody>
              <tfoot>
            <tr class="info-light bold">
              <td colspan="9" style="text-align:right;"><strong>Total:</strong></td>


            <td class="text-center">{{ getGroupTotals(group.entries).regularHour }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).overtimeHour }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).nd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).ndot }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lateMinute }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).undertimeMinute }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdndot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).sh }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shndot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdndot }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lhrd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdndot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lh }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhndot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhsh }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshndot }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lhshrd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdndot }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).hu100 }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drh }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhndot }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).drhrd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdot }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdnd }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdndot }}</td>

            </tr>
              </tfoot>
            </table>
<!-- offset -->
            <table v-show = "timekeeping.applyOffSetFlag" class="table-scroll striped-even mb-0"  ref="table" id="loremTable" summary="lorem ipsum sit amet" rules="groups" frame="hsides" border="2" :style="getTableWrapperStyle(group.entries.length)">
   
           
              <!-- <table class="striped-even mb-0" style="width: 100%;"> -->
              <thead class="info-light">
                <tr>
                  <th class="text-center">Action</th>
                  <th>Date</th>
                  <th>Day Type</th>
                  <th>Schedule</th>
                  <!-- <th>Approved Form</th> -->
                  <!-- <th>Payable To Member</th>
                  <th>Billable To Client</th> -->
                  <th @click="toggleAllPayableToMember(group.entries, group.memberId)" style="cursor: pointer;" class="payable">
                    <i class="fa fa-check text-success "></i>
                    Payable To Member
                  </th>

                  <th @click="toggleAllBillableToClient(group.entries)" style="cursor: pointer;" class="billable">
                       <i class="fa fa-check text-success"></i>
                    Billable To Client
                  </th>
                  <th class="text-center fremarks">Remarks</th>
                  <th class="text-center">Time In</th>
                  <th class="text-center">Time Out</th>
                  <th class="text-center">Reg Hrs</th>
                  <th class="text-center">OT</th>
                  <th class="text-center">ND</th>
                  <th class="text-center">ND OT</th>
                  <th class="text-center">Late/mins</th>
                  <th class="text-center">Undertime/mins</th>
                                    
                  <th class="text-center">RD</th>
                  <th class="text-center">RDOT</th>
                  <th class="text-center">RDND</th>
                  <th class="text-center">RDNDOT</th>
                  <th class="text-center">SH</th>
                  <th class="text-center">SHOT</th>
                  <th class="text-center">SHND</th>
                  <th class="text-center">SHNDOT</th>
                  <th class="text-center">SHRD</th>
                  <th class="text-center">SHRDOT</th>
                  <th class="text-center">SHRDND</th>
                  <th class="text-center">SHRDNDOT</th>
                  <th class="text-center">LHRD</th>
                  <th class="text-center">LHRDOT</th>
                  <th class="text-center">LHRDND</th>
                  <th class="text-center">LHRDNDOT</th>
                  <th class="text-center">LH</th>
                  <th class="text-center">LHOT</th>
                  <th class="text-center">LHND</th>
                  <th class="text-center">LHNDOT</th>
                  <th class="text-center">LHSH</th>

                  <!-- here   -->
                  <th class="text-center">LHSHOT</th>
                  <th class="text-center">LHSHND</th>
                  <th class="text-center">LHSHNDOT</th>
                  <!-- here   -->
                  
                  <th class="text-center">LHSHRD</th>
                  
                  <!-- here   -->
                  <th class="text-center">LHSHRDOT</th>
                  <th class="text-center">LHSHRDND</th>
                  <th class="text-center">LHSHRDNDOT</th>
                  <!-- here   -->

                  <th class="text-center">HU-100</th>
                  <th class="text-center">DRH</th>
                  <th class="text-center">DRHOT</th>
<!-- here   -->
<th class="text-center">DRHND</th>
<th class="text-center">DRHNDOT</th>

                  <th class="text-center">DRHRD</th>
                  <th class="text-center">DRHRDOT</th>
               <!-- here   --> 
<th class="text-center">DRHRDND</th>
<th class="text-center">DRHRDNDOT</th>

                

                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(dtl, index) in group.entries"
                  :key="`${group.memberId}-${index}`" :class="getNoDataColor(dtl.scheduleCode, dtl.scheduleTimeIn, dtl.scheduleTimeOut )"
                >
                  <td class="p-1">
                  <div class="buttons" sm-1 mb-0 >

                    <button v-show="timekeeping.manualEditFlag==0"  :disabled="timekeeping.manualUploadFlag==1" @click="onEditSchedule(dtl, dtl.flatIndex)" class="btn-dtl-edit info" type="button" title="Edit Schedule">
                      <i class="fa fa-edit"></i> 
                    </button>
                      
                    <button @click="onEditManualScheduleAccess(dtl, dtl.flatIndex)" class="btn-dtl-edit danger" type="button" title="Manual Entry Time">
                      <i class="fa fa-edit"></i>
                    </button>
              </div>

                  </td>
                  <td>{{ dtl.scheduleDate }}</td>
                  <td :class="{ 'bold italic': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
                  <!-- <td>{{ dtl.scheduleCode }}</td> -->
                  <td  :class="{ 'bold': dtl.timekeepingScheduleId === 999999999 || dtl.timekeepingScheduleId === 999999998 }">
                    {{ dtl.scheduleCode }}
                  </td>
                  <td class="text-center">
                    <i
                      :class="getBooleanIconClass(dtl.payableToMemberFlag)"
                      @click="togglePayableToMemberFlag(dtl)"
                      style="cursor: pointer;"
                    ></i>
                  </td>

                  <td class="text-center">
                    <i
                      :class="getBooleanIconClass(dtl.billableToClientFlag)"
                      @click="toggleBillableToClientFlag(dtl)"
                      style="cursor: pointer;"
                    ></i>
                  </td>

                  <td class="text-center">{{ dtl.remarks }}</td>
                  <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeIn) }}</td>
                  <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeOut) }}</td>
                  <td class="text-center">{{ dtl.regularHourP }}</td>
                  <td class="text-center">{{ dtl.overtimeHourP }}</td>
                  <td class="text-center">{{ dtl.ndp }}</td>
                  <td class="text-center">{{ dtl.ndotp }}</td>
                  <td class="text-center">{{ core.toDecimalFormat(dtl.lateMinuteP) }}</td>
                  <td class="text-center">{{ dtl.undertimeMinuteP }}</td>

                  <th class="text-center">{{ dtl.rdp === 0 ? '' : dtl.rdp }}</th>
                  <th class="text-center">{{ dtl.rdotp === 0 ? '' : dtl.rdotp }}</th>
                  <th class="text-center">{{ dtl.rdndp === 0 ? '' : dtl.rdndp }}</th>
                  <th class="text-center">{{ dtl.rdndotp === 0 ? '' : dtl.rdndotp }}</th>
                  <th class="text-center">{{ dtl.shp === 0 ? '' : dtl.shp }}</th>
                  <th class="text-center">{{ dtl.shotp === 0 ? '' : dtl.shotp }}</th>
                  <th class="text-center">{{ dtl.shndp === 0 ? '' : dtl.shndp }}</th>
                  <th class="text-center">{{ dtl.shndotp === 0 ? '' : dtl.shndotp }}</th>
                  <th class="text-center">{{ dtl.shrdp === 0 ? '' : dtl.shrdp }}</th>
                  <th class="text-center">{{ dtl.shrdotp === 0 ? '' : dtl.shrdotp }}</th>
                  <th class="text-center">{{ dtl.shrdndp === 0 ? '' : dtl.shrdndp }}</th>
                  <th class="text-center">{{ dtl.shrdndotp === 0 ? '' : dtl.shrdndotp }}</th>
                  <th class="text-center">{{ dtl.lhrdp === 0 ? '' : dtl.lhrdp }}</th>
                  <th class="text-center">{{ dtl.lhrdotp === 0 ? '' : dtl.lhrdotp }}</th>
                  <th class="text-center">{{ dtl.lhrdndp === 0 ? '' : dtl.lhrdndp }}</th>
                  <th class="text-center">{{ dtl.lhrdndotp === 0 ? '' : dtl.lhrdndotp }}</th>
                 
                  <th class="text-center">{{ dtl.lhp === 0 ? '' : dtl.lhp }}</th>
                  <th class="text-center">{{ dtl.lhotp === 0 ? '' : dtl.lhotp }}</th>
                  <th class="text-center">{{ dtl.lhndp === 0 ? '' : dtl.lhndp }}</th>
                  <th class="text-center">{{ dtl.lhndotp === 0 ? '' : dtl.lhndotp }}</th>
                  <th class="text-center">{{ dtl.lhshp === 0 ? '' : dtl.lhshp }}</th>
                  <th class="text-center">{{ dtl.lhshotp === 0 ? '' : dtl.lhshotp }}</th>
                  <th class="text-center">{{ dtl.lhshndp === 0 ? '' : dtl.lhshndp }}</th>
                  <th class="text-center">{{ dtl.lhshndotp === 0 ? '' : dtl.lhshndotp }}</th>
                  
                  <th class="text-center">{{ dtl.lhshrdp === 0 ? '' : dtl.lhshrdp }}</th>

                  <th class="text-center">{{ dtl.lhshrdotp === 0 ? '' : dtl.lhshrdotp }}</th>
                  <th class="text-center">{{ dtl.lhshrdndp === 0 ? '' : dtl.lhshrdndp }}</th>
                  <th class="text-center">{{ dtl.lhshrdndotp === 0 ? '' : dtl.lhshrdndotp }}</th>

                  <th class="text-center">{{ dtl.hu100p === 0 ? '' : dtl.hu100p }}</th>
                  <th class="text-center">{{ dtl.drhp === 0 ? '' : dtl.drhp }}</th>
                  <th class="text-center">{{ dtl.drhotp === 0 ? '' : dtl.drhotp }}</th>
                  <th class="text-center">{{ dtl.drhndp === 0 ? '' : dtl.drhndp }}</th>
                  <th class="text-center">{{ dtl.drhndotp === 0 ? '' : dtl.drhndotp }}</th>

                  <th class="text-center">{{ dtl.drhrdp === 0 ? '' : dtl.drhrdp }}</th>
                  <th class="text-center">{{ dtl.drhrdotp === 0 ? '' : dtl.drhrdotp }}</th>
                  <th class="text-center">{{ dtl.drhrdndp === 0 ? '' : dtl.drhrdndp }}</th>
                  <th class="text-center">{{ dtl.drhrdndotp === 0 ? '' : dtl.drhrdndotp }}</th>
                </tr>
              </tbody>
              <tfoot>
            <tr class="info-light bold">
              <td colspan="9" style="text-align:right;"><strong>Total:</strong></td>


            <td class="text-center">{{ getGroupTotals(group.entries).regularHourP }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).overtimeHourP }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).ndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).ndotp }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lateMinuteP }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).undertimeMinuteP }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).rdndotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shndotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).shrdndotp }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lhrdp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhrdndotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhndotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshndotp }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).lhshrdndotp }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).hu100p }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhndotp }}</td>

            <td class="text-center">{{ getGroupTotals(group.entries).drhrdp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdotp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdndp }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).drhrdndotp }}</td>

            </tr>
              </tfoot>
            </table>
<!-- off set hide details -->

          </div>

                </div>
              </div>
          
     
      </div>

      </div>



       
        
          </div>

        </div>


      



    </sym-form>


<sym-modal
  id="schedule-editor"
  v-model="isScheduleEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="editorTitle"
  @show="onShowScheduleEditor($event)"
  @hide="onHideScheduleEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="pay0040A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    
      <div class="box-edit-1">
        <sym-int  v-model="schedule.memberId" caption="Member ID" align="bottom"></sym-int>
        <sym-text  v-model="schedule.memberName" caption="Member Name" align="bottom"></sym-text>
        <sym-date  v-model="schedule.scheduleDate" caption="Date" align="bottom"></sym-date>
        
      </div>
      <div class="box-edit-2">
        <!-- <sym-check v-model="schedule.approvedFormFlag" caption="Approved Form" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>  -->
        <sym-check v-model="schedule.payableToMemberFlag" caption="Payable To Member" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-check v-model="schedule.billableToClientFlag" caption="Billable To Client" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-text v-model="schedule.remarks" caption="Remarks" align="bottom"></sym-text>
      </div>
    

        <div class="box-4">

        <sym-combo  v-model="schedule.noScheduleId"  align="bottom" :caption-width="46" caption="No Schedule" display-field="noScheduleName" :datasource="noSchedules"/>

        <sym-combo v-if="visibleCombos >= 1" v-model="schedule.timekeepingScheduleId" align="bottom" :caption-width="46" caption="Schedule" display-field="scheduleName" :datasource="scheduleList"/>
        <sym-combo v-if="visibleCombos >= 2" v-model="schedule.timekeepingScheduleIdA" align="bottom" :caption-width="46" caption="Schedule A" display-field="scheduleName" :datasource="scheduleListA"/>
        <sym-combo v-if="visibleCombos >= 3" v-model="schedule.timekeepingScheduleIdB" align="bottom" :caption-width="46" caption="Schedule B" display-field="scheduleName" :datasource="scheduleListB"/>
        <sym-combo v-if="visibleCombos >= 4" v-model="schedule.timekeepingScheduleIdC" align="bottom" :caption-width="46" caption="Schedule C" display-field="scheduleName" :datasource="scheduleListC"/>
        <sym-combo v-if="visibleCombos >= 5" v-model="schedule.timekeepingScheduleIdD" align="bottom" :caption-width="46" caption="Schedule D" display-field="scheduleName" :datasource="scheduleListD"/>
        </div> 
      <div class="box-3">
        <sym-time  v-model="schedule.scheduleTimeIn" caption="Time In"  :input-width="20" :isoFormat="true" align="bottom"></sym-time>
      <sym-time  v-model="schedule.scheduleTimeOut" caption="Time Out"  :input-width="23"  :isoFormat="true" align="bottom" ></sym-time>
       <sym-date v-model="schedule.scheduleDateTimeOut" align="bottom" :caption-width="30" caption="Date Out" ></sym-date>

      <!-- <sym-combo v-if="visibleCombos >= 1" v-model="schedule.timekeepingScheduleId" align="bottom" :caption-width="46" caption="Schedule" display-field="scheduleName" :datasource="scheduleList"/> -->
   
     
    </div>  
    <div class="edit-btns">

    
    <div class="buttons w-100 justify-left " fw-26 shadow-soft mb-0>
      <button @click="addCombo" :disabled="visibleCombos >= maxCombos" class="success-light w-40"><i class="fa fa-plus mr-2"></i>Add Schedule</button>
      <button @click="removeCombo" :disabled="visibleCombos <= 1" class="danger-light"><i class="fa fa-minus mr-2"></i>Remove</button>
    </div>

        <!-- <button type="button" class="info justify-between border-main" ><i class="fa fa-check mr-2"></i>Upload OT Form</button> -->
    <div class="buttons w-100 justify-end  m" fw-26 shadow-soft mb-0>
      
      <button type="button" class="info justify-between border-main" @click="onSubmitSchedule()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isScheduleEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>
  </div>

  </form>  
</div>

</sym-modal>

<!-- Manual -->
<sym-modal
  id="manual-schedule-editor"
  v-model="isManualScheduleEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="editorTitle"
  @show="onShowManualScheduleEditor($event)"
  @hide="onHideManualScheduleEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="pay0040A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    
      <div class="box-edit-1">
        <sym-int  v-model="schedule.memberId" caption="Member ID" align="bottom"></sym-int>
        <sym-text  v-model="schedule.memberName" caption="Member Name" align="bottom"></sym-text>
        <sym-date  v-model="schedule.scheduleDate" caption="Date" align="bottom"></sym-date>
        
      </div>
      <div class="box-edit-2">
        <!-- <sym-check v-model="schedule.approvedFormFlag" caption="Approved Form" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>  -->
        <sym-check v-model="schedule.payableToMemberFlag" caption="Payable To Member" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-check v-model="schedule.billableToClientFlag" caption="Billable To Client" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-text v-model="schedule.remarks" caption="Remarks" align="bottom"></sym-text>
      </div>
    
        <div class="box-4 gap">

        <sym-combo  v-model="schedule.noScheduleId"  align="bottom" :caption-width="46" caption="No Schedule" display-field="noScheduleName" :datasource="noSchedules"/>

        <sym-combo v-if="visibleCombos >= 1" v-model="schedule.timekeepingScheduleId" align="bottom" :caption-width="46" caption="Schedule" display-field="scheduleName" :datasource="scheduleList"/>
        <sym-combo v-if="visibleCombos >= 2" v-model="schedule.timekeepingScheduleIdA" align="bottom" :caption-width="46" caption="Schedule A" display-field="scheduleName" :datasource="scheduleListA"/>
        <sym-combo v-if="visibleCombos >= 3" v-model="schedule.timekeepingScheduleIdB" align="bottom" :caption-width="46" caption="Schedule B" display-field="scheduleName" :datasource="scheduleListB"/>
        <sym-combo v-if="visibleCombos >= 4" v-model="schedule.timekeepingScheduleIdC" align="bottom" :caption-width="46" caption="Schedule C" display-field="scheduleName" :datasource="scheduleListC"/>
        <sym-combo v-if="visibleCombos >= 5" v-model="schedule.timekeepingScheduleIdD" align="bottom" :caption-width="46" caption="Schedule D" display-field="scheduleName" :datasource="scheduleListD"/>
        
        </div> 
     
     
        <div class="box-3">
            <sym-time  v-model="schedule.scheduleTimeIn" caption="Time In"  :input-width="20" :isoFormat="true" align="bottom"></sym-time>
            <sym-time  v-model="schedule.scheduleTimeOut" caption="Time Out"  :input-width="23"  :isoFormat="true" align="bottom" ></sym-time>
             <sym-date v-model="schedule.scheduleDateTimeOut" align="bottom" :caption-width="30" caption="Date Out" ></sym-date>
        </div>  
      <div class="time-container-1 gap">
        <sym-dec v-model="schedule.regularHourP" align="bottom" :caption-width="50" caption="Reg Hrs" :blankZero="false" ></sym-dec>
        <sym-dec v-model="schedule.overtimeHourP" align="bottom" :caption-width="50" caption="OT" :blankZero="false" ></sym-dec>
        <sym-dec v-model="schedule.ndp" align="bottom" :caption-width="50" caption="ND" :blankZero="false" ></sym-dec>
        <sym-dec v-model="schedule.ndotp" align="bottom" :caption-width="50" caption="NDOT" :blankZero="false" ></sym-dec>
        <sym-dec v-model="schedule.lateMinuteP" align="bottom" :caption-width="50" caption="Late/mins" :blankZero="false" ></sym-dec>
        <sym-dec v-model="schedule.undertimeMinuteP" align="bottom" :caption-width="50" caption="Undertime/mins" :blankZero="false" ></sym-dec>
      </div>

      <div class="time-container-2 gap">
        <!-- Rest Day -->
        <sym-dec v-model="schedule.rdp" align="bottom" :caption-width="50" caption="RD" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.rdotp" align="bottom" :caption-width="50" caption="RDOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.rdndp" align="bottom" :caption-width="50" caption="RDND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.rdndotp" align="bottom" :caption-width="50" caption="RDNDOT" :blankZero="false"></sym-dec>
        
        <!-- Special Holiday -->
        <sym-dec v-model="schedule.shp" align="bottom" :caption-width="50" caption="SH" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shotp" align="bottom" :caption-width="50" caption="SHOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shndp" align="bottom" :caption-width="50" caption="SHND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shndotp" align="bottom" :caption-width="50" caption="SHNDOT" :blankZero="false"></sym-dec>

      </div>

      <div class="time-container-2 gap">

        <!-- Special + Rest Day -->
        <sym-dec v-model="schedule.shrdp" align="bottom" :caption-width="50" caption="SHRD" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shrdotp" align="bottom" :caption-width="50" caption="SHRDOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shrdndp" align="bottom" :caption-width="50" caption="SHRDND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.shrdndotp" align="bottom" :caption-width="50" caption="SHRDNDOT" :blankZero="false"></sym-dec>

        <!-- Legal Holiday + Rest Day -->
        <sym-dec v-model="schedule.lhrdp" align="bottom" :caption-width="50" caption="LHRD" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhrdotp" align="bottom" :caption-width="50" caption="LHRDOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhrdndp" align="bottom" :caption-width="50" caption="LHRDND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhrdndotp" align="bottom" :caption-width="50" caption="LHRDNDOT" :blankZero="false"></sym-dec>

      </div>


      <div class="time-container-2 gap">

       <!-- Legal Holiday -->
        <sym-dec v-model="schedule.lhp" align="bottom" :caption-width="50" caption="LH" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhotp" align="bottom" :caption-width="50" caption="LHOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhndp" align="bottom" :caption-width="50" caption="LHND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhndotp" align="bottom" :caption-width="50" caption="LHNDOT" :blankZero="false"></sym-dec>
      
        <!-- LH + SH -->
        <sym-dec v-model="schedule.lhshp" align="bottom" :caption-width="50" caption="LHSH" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshotp" align="bottom" :caption-width="50" caption="LHSHOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshndp" align="bottom" :caption-width="50" caption="LHSHND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshndotp" align="bottom" :caption-width="50" caption="LHSHNDOT" :blankZero="false"></sym-dec>
      
      </div> 


      <div class="time-container-2 gap">

        <!-- LH + SH + Rest Day -->
        <sym-dec v-model="schedule.lhshrdp" align="bottom" :caption-width="50" caption="LHSHRD" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshrdotp" align="bottom" :caption-width="50" caption="LHSHRDOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshrdndp" align="bottom" :caption-width="50" caption="LHSHRDND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.lhshrdndotp" align="bottom" :caption-width="50" caption="LHSHRDNDOT" :blankZero="false"></sym-dec>
      
        <!-- Double Rest Holiday -->
        <sym-dec v-model="schedule.drhp" align="bottom" :caption-width="50" caption="DRH" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhotp" align="bottom" :caption-width="50" caption="DRHOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhndp" align="bottom" :caption-width="50" caption="DRHND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhndotp" align="bottom" :caption-width="50" caption="DRHNDOT" :blankZero="false"></sym-dec>
      
      </div> 
      

      <div class="time-container-2 gap">

      
        <!-- DRH + Rest Day -->
        <sym-dec v-model="schedule.drhrdp" align="bottom" :caption-width="50" caption="DRHRD" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhrdotp" align="bottom" :caption-width="50" caption="DRHRDOT" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhrdndp" align="bottom" :caption-width="50" caption="DRHRDND" :blankZero="false"></sym-dec>
        <sym-dec v-model="schedule.drhrdndotp" align="bottom" :caption-width="50" caption="DRHRDNDOT" :blankZero="false"></sym-dec>
        
        <!-- Holiday 100% -->
        <sym-dec v-model="schedule.hu100p" align="bottom" :caption-width="50" caption="HU100" :blankZero="false"></sym-dec>
      
      
      </div>  
    <div class="edit-btns">

    
    <div class="buttons w-100 justify-left " fw-26 shadow-soft mb-0>
      <button @click="addCombo" :disabled="visibleCombos >= maxCombos" class="success-light w-40"><i class="fa fa-plus mr-2"></i>Add Schedule</button>
      <button @click="removeCombo" :disabled="visibleCombos <= 1" class="danger-light"><i class="fa fa-minus mr-2"></i>Remove</button>
    </div>

        <!-- <button type="button" class="info justify-between border-main" ><i class="fa fa-check mr-2"></i>Upload OT Form</button> -->
    <div class="buttons w-100 justify-end  m" fw-26 shadow-soft mb-0>
      
      <button type="button" class="info justify-between border-main" @click="onSubmitManualSchedule()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isManualScheduleEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>
  </div>

  </form>  
</div>

</sym-modal>



  <!-- Doc  -->
  <sym-modal
        id="doc-editor"
        v-model="isDocEditorVisible"
        size="md"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="false"
        :dismissible="false"
        :closeOnBackButton="false"
        :title="editorDocTitle"
        @show="onShowDocEditor($event)"
        @hide="onHideDocEditor($event)"
          headerClass="app-form-header"
        dismissButtonClass="danger"
      >
        <div class="board p-1 mb-0 w-100">
          <form id="hrs0010D" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
            <div class="detail-editor-boxes">
              <sym-text v-model="schedule.otRemarks" caption="Name" align="bottom"></sym-text>
              <!-- <div class="doctype-fields" v-show="doc.docTypeId != 0"> -->
              <!-- <sym-text v-model="doc.docTypeReference" caption="Reference" align="bottom" @changing="onDocTypeReferenceChanging"></sym-text> -->
          
              <!-- <input class="MobilePhone" v-model="doc.docTypeReference" :maxlength="doc.docTypeLength" @changing="onDocTypeReferenceChanging" /> -->
              <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            <!-- </div> -->
            </div>
            </div>

              <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
                <button type="button"  class="info justify-between border-main" @click="onSubmitDoc()">
                <i class="fa fa-check mr-2"></i>Submit</button>
              <button  type="button" class="warning justify-between border-main mr-0" @click="isDocEditorVisible = false" >
                <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
            </div>
          </form>
        </div>
  </sym-modal>

  <!-- Upload File -->
  <sym-upload
      ref="uploader"
      class="d-none lams-light border-main curved-1 p-1 shadow-light mb-3"
      inputClass="lams-light shadow"
      uploadButtonClass="success shadow"
      resetButtonClass="danger-light border-main shadow-light"
      iconClass="fa-files-o fa-fw fa-3x"
      :multiple="false"
      accept=".pdf,.jpg,.png"
      instructions="Drag documents here<br>or click to browse"
      uploadButtonText="Click to Upload Now"
      :blinkUploadIcon="true"
      @select="onSelectDocuments"
      @upload="onUploadDocuments"
      @selectedchanged="onSelectedChanged"
      size = "sm"
    >
  </sym-upload>


</section>
</template>

<script>

import * as XLSX from 'xlsx';

// import * as XLSX from 'xlsx';
// import jsPDF from "jspdf";
import "jspdf-autotable";
// import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import ExcelJS from 'exceljs';


import {
  get,
  ajax,
  upload, //PHOTO
} from "../../js/http";

import {
  getList,
  getSafeDeleteFlag,
} from "../../js/dbSys";

import PageMaintenance from "../PageMaintenance.vue";
import SymImageSelect from "../../comp/SymImageSelect.vue";
import SymInteger from '../../comp/SymInteger.vue';
import { thresholdScott } from 'd3';
import { escText } from '../../js/core';

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "pay0040",

  data() {
    return {
      
      timekeeping: {
        timekeepingSheetId: 0,
        clientPayGroupId: 0,
        clientPayGroupName: '',
        payFreqId: 0,
        periodId: 0,
        yearId: 0,
        payrollPeriodId: 0,
        // memberRequestId: 0,
        // memberRequestName: '',
        startDate: null,
        endDate: null,
        cutOffDate: null,
        cutOffStartDate: null,
        cutOffEndDate: null,
        remarks: '',
        lateCount:0,
	      lateGracePeriodCount:0,

        minimumRequiredOvertimeCount:0, // Late counting interval in minutes
        overtimeDurationCount:0,
        overtimeCountId:0,
        maximumRegularHourCount: 0,
        ndCount:0,
        paidUpHolidayId: 0,
        nightDifferentialCountId: 0,
        restDayOffSetFlag: false,
        restDayConsideration: 0,
        lateCountId: 0,
        holidayCountId: 0,
        undertimeCountId:0,
        applyOffSetFlag: false,
        detailsFlag: false,
        constructionFlag: false,
        manualUploadFlag: false,
        manualEditFlag: false,
        underTimeDurationPerCount: 0,
        lockId: "",
      },

      oldTimekeeping: [],
      logs: [],
      isLogVisible: false,

      memberRequest: [],

      schedules: [],

      isScheduleEditorVisible: false,
      isManualScheduleEditorVisible: false,

      schedule: {
        sheetMemberId: 0,
        timekeepingSheetId: 0,
        memberId: 0,
        memberName: '',
        departmentName: '',
        scheduleDate: null,
        noScheduleId: 0,
        timekeepingScheduleId: 0,
        timekeepingScheduleIdA: 0,
        timekeepingScheduleIdB: 0,
        timekeepingScheduleIdC: 0,
        timekeepingScheduleIdD: 0,
        scheduleCode: '',
        scheduleCodeA: '',
        scheduleCodeB: '',
        scheduleCodeC: '',
        scheduleCodeD: '',
        dayTypeName: '',
        dayTypeNameRef: '',
        scheduleTimeIn:  null,
        scheduleTimeOut:  null,
        scheduleDateTimeOut:  null,
 
        totalWorkingHour:  null,
        approvedFormFlag: false,
        payableToMemberFlag: false,
        billableToClientFlag: false,
        regularHour:0,
        unpaidBreak:0,
        overtimeHour:0,
        regularWorkingDayHour:0,
        regularOvertimeHour:0,
        lateMinute:0,
        undertimeMinute:0,
        remarks:'',
        
        nd: 0,
        ndot: 0,
        rd: 0,
        rdot: 0,
        rdnd: 0,
        rdndot: 0,
        sh: 0,
        shot: 0,
        shnd: 0,
        shndot: 0,
        shrd: 0,
        shrdot: 0,
        shrdnd: 0,
        shrdndot: 0,
        lhrd: 0,
        lhrdot: 0,
        lhrdnd: 0,
        lhrdndot: 0,
        lh: 0,
        lhot: 0,
        lhnd: 0,
        lhndot: 0,
        lhsh: 0,
        lhshot: 0,
        lhshnd: 0,
        lhshndot: 0,

        lhshrd: 0,
        lhshrdot: 0,
        lhshrdnd: 0,
        lhshrdndot: 0,

        hu100: 0,
        drh: 0,
        drhot: 0,
        drhnd: 0,
        drhndot: 0,

        drhrd: 0,
        drhrdot: 0,
        drhrdnd: 0,
        drhrdndot: 0,

        regularHourP:0,
        overtimeHourP:0,
        lateMinuteP:0,
        undertimeMinuteP:0,       
        ndp: 0,
        ndotp: 0,
        rdp: 0,
        rdotp: 0,
        rdndp: 0,
        rdndotp: 0,
        shp: 0,
        shotp: 0,
        shndp: 0,
        shndotp: 0,
        shrdp: 0,
        shrdotp: 0,
        shrdndp: 0,
        shrdndotp: 0,
        lhrdp: 0,
        lhrdotp: 0,
        lhrdndp: 0,
        lhrdndotp: 0,
        lhp: 0,
        lhotp: 0, 
        lhndp: 0,
        lhndotp: 0,

        lhshp: 0,
        lhshotp: 0,
        lhshndp: 0,
        lhshndotp: 0,

        lhshrdp: 0,
        lhshrdotp: 0,
        lhshrdndp: 0,
        lhshrdndotp: 0,

        hu100p: 0,
        drhp: 0,
        drhotp: 0,
        drhndp: 0,
        drhndotp: 0,

        drhrdp: 0,
        drhrdotp: 0,
        drhrdndp: 0,
        drhrdndotp: 0,


        dayTypeCode: '',
        otRemarks:'',
        otFileName:'',
        otGUID:'',
        fileExtension:'',
        fileUrl:'',

        lockId: '',
        offSetFlag: false,
      },

      scheduleIndex: -1,
      isAddingSchedule: false,
      isAddingManualSchedule: false,
      excelData: [],
      isReading: false, // For loading bar,

    visibleCombos: 1, // Start with the first combo visible
    maxCombos: 5,
    searchMemberName:'',
        uri :'data:application/vnd.ms-excel;base64,',
        template:'<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64: function(s){ return window.btoa(unescape(encodeURIComponent(s))) },
        format: function(s, c) { return s.replace(/{(\w+)}/g, function(m, p) { return c[p]; }) },

    reportData: [
        { date: "2025-05-16", name: "AQUINO, TERESA C.", schedule: "07:30-18:00" },
        { date: "2025-05-17", name: "NAVA, PIOLO B.", schedule: "08:30-18:30" },
        { date: "2025-05-18", name: "JUBASAN, DAVE B.", schedule: "REST DAY" },
        // more data here...
      ],

     isAddingDoc: false,
      isDocEditorVisible: false,
      docIndex: -1,

      fileName: "",
      isUploadRequired: false,
      pathFileName: "",
      guidReference: "",

      otMemberId:0,
      passwordFlag: false,    
  };



  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

    onShowManualScheduleEditor () {
      const me = this;
      // me.visibleCombos =1;
      let target = 'scheduleMemberId';

      me.setActiveModel('schedule');

      me.setRequiredMode(
        'scheduleDate', 
      );  

      me.setDisplayMode(
        'memberId',
        'memberName',
        'scheduleDate',
      );  

      if (!me.isAddingManualSchedule) {
        if (me.schedule.lockId) {
          target = 'scheduleDate';
        }
      }

      setTimeout(() => {
        this.setFocus(target);
      }, 200);
    },

    onHideManualScheduleEditor () {
      const me = this;
     
      me.isAddingManualSchedule = false;
      me.setActiveModel();
    },

    onEditManualScheduleAccess(dtl, index) {

      const me = this;
      if (!me.passwordFlag) {    
      let promise = me.isActionAllowed('EDIT-DTR-GEN');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.passwordFlag =  true;
            me.onEditManualSchedule(dtl, index);
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );
} else {
  me.onEditManualSchedule(dtl, index);
  
}
    },

    onEditManualSchedule (dtl, index) {
      const me = this, d = this.schedule;

      this.scheduleIndex = index;
      this.passwordFlag = true;
      this.visibleCombos = 1;
      
      dtl = this.core.convertDates(dtl);
      dtl = this.core.convertTimes(dtl);

      if (dtl.scheduleDate === null) {
        dtl.scheduleDate = this.core.emptyDateTime();
      }

      d.scheduleDate = dtl.scheduleDate;
      d.timekeepingSheetId = dtl.timekeepingSheetId;
      d.memberId = dtl.memberId;
      d.memberName = dtl.memberName;
      d.scheduleCode = dtl.scheduleCode;
      d.noScheduleId = dtl.noScheduleId;
      d.dayTypeName = dtl.dayTypeName;
      d.dayTypeNameRef = dtl.dayTypeNameRef;

      d.timekeepingScheduleId = dtl.timekeepingScheduleId;
      d.unpaidBreak = dtl.unpaidBreak;
      d.timekeepingScheduleIdA = dtl.timekeepingScheduleIdA;
      d.timekeepingScheduleIdB = dtl.timekeepingScheduleIdB;
      d.timekeepingScheduleIdC = dtl.timekeepingScheduleIdC;
      d.timekeepingScheduleIdD = dtl.timekeepingScheduleIdD;
      d.scheduleTimeIn = dtl.scheduleTimeIn;
      d.scheduleTimeOut = dtl.scheduleTimeOut;
      d.scheduleDateTimeOut = dtl.scheduleDateTimeOut;

      d.regularHourP = dtl.regularHourP;
      d.overtimeHourP = dtl.overtimeHourP;
      d.ndp = dtl.ndp;
      d.ndotp = dtl.ndotp;
      d.lateMinuteP = dtl.lateMinuteP;
      d.undertimeMinuteP = dtl.undertimeMinuteP;

      d.rdp = dtl.rdp;
      d.rdotp = dtl.rdotp;
      d.rdndp = dtl.rdndp;
      d.rdndotp = dtl.rdndotp;

      d.shp = dtl.shp;
      d.shotp = dtl.shotp;
      d.shndp = dtl.shndp;
      d.shndotp = dtl.shndotp;

      d.shrdp = dtl.shrdp;
      d.shrdotp = dtl.shrdotp;
      d.shrdndp = dtl.shrdndp;
      d.shrdndotp = dtl.shrdndotp;

      d.lhrdp = dtl.lhrdp;
      d.lhrdotp = dtl.lhrdotp;
      d.lhrdndp = dtl.lhrdndp;
      d.lhrdndotp = dtl.lhrdndotp;

      d.lhp = dtl.lhp;
      d.lhotp = dtl.lhotp;
      d.lhndp = dtl.lhndp;
      d.lhndotp = dtl.lhndotp;

      d.lhshp = dtl.lhshp;
      d.lhshotp = dtl.lhshotp;
      d.lhshndp = dtl.lhshndp;
      d.lhshndotp = dtl.lhshndotp;

      d.lhshrdp = dtl.lhshrdp;
      d.lhshrdotp = dtl.lhshrdotp;
      d.lhshrdndp = dtl.lhshrdndp;
      d.lhshrdndotp = dtl.lhshrdndotp;

      d.hu100p = dtl.hu100p;

      d.drhp = dtl.drhp;
      d.drhotp = dtl.drhotp;
      d.drhndp = dtl.drhndp;
      d.drhndotp = dtl.drhndotp;

      d.drhrdp = dtl.drhrdp;
      d.drhrdotp = dtl.drhrdotp;
      d.drhrdndp = dtl.drhrdndp;
      d.drhrdndotp = dtl.drhrdndotp;

      d.totalWorkingHour = dtl.totalWorkingHour;
      // d.scheduleCode = dtl.scheduleCode;
      d.approvedFormFlag = dtl.approvedFormFlag;
      d.payableToMemberFlag = dtl.payableToMemberFlag;
      d.billableToClientFlag = dtl.billableToClientFlag;
      d.remarks = dtl.remarks;

      this.visibleCombos = 1;

      if (dtl.timekeepingScheduleIdA) this.visibleCombos = 2;
      if (dtl.timekeepingScheduleIdB) this.visibleCombos = 3;
      if (dtl.timekeepingScheduleIdC) this.visibleCombos = 4;
      if (dtl.timekeepingScheduleIdD) this.visibleCombos = 5;
      d.lockId = dtl.lockId;
     
      this.offSetFlag = true
      this.timekeeping.applyOffSetFlag = true;
      this.isManualScheduleEditorVisible = true;

      // this.scheduleIndex = index;

      // this.visibleCombos = 1;
      
      // dtl = this.core.convertDates(dtl);
      // dtl = this.core.convertTimes(dtl);

      // if (dtl.scheduleDate === null) {
      //   dtl.scheduleDate = this.core.emptyDateTime();
      // }

      // d.scheduleDate = dtl.scheduleDate;
      // d.timekeepingSheetId = dtl.timekeepingSheetId;
      // d.memberId = dtl.memberId;
      // d.memberName = dtl.memberName;
      // d.scheduleCode = dtl.scheduleCode;
      // d.noScheduleId = dtl.noScheduleId;
      // d.dayTypeName = dtl.dayTypeName;
      // d.dayTypeNameRef = dtl.dayTypeNameRef;

      // d.timekeepingScheduleId = dtl.timekeepingScheduleId;
      // d.unpaidBreak = dtl.unpaidBreak;
      // d.timekeepingScheduleIdA = dtl.timekeepingScheduleIdA;
      // d.timekeepingScheduleIdB = dtl.timekeepingScheduleIdB;
      // d.timekeepingScheduleIdC = dtl.timekeepingScheduleIdC;
      // d.timekeepingScheduleIdD = dtl.timekeepingScheduleIdD;
      // d.scheduleTimeIn = dtl.scheduleTimeIn;
      // d.scheduleTimeOut = dtl.scheduleTimeOut;
      // d.scheduleDateTimeOut = dtl.scheduleDateTimeOut;

      // d.regularHourP = dtl.regularHourP;
      // d.overtimeHourP = dtl.overtimeHourP;
      // d.ndp = dtl.ndp;
      // d.ndotp = dtl.ndotp;
      // d.lateMinuteP = dtl.lateMinuteP;
      // d.undertimeMinuteP = dtl.undertimeMinuteP;

      // d.rdp = dtl.rdp;
      // d.rdotp = dtl.rdotp;
      // d.rdndp = dtl.rdndp;
      // d.rdndotp = dtl.rdndotp;

      // d.shp = dtl.shp;
      // d.shotp = dtl.shotp;
      // d.shndp = dtl.shndp;
      // d.shndotp = dtl.shndotp;

      // d.shrdp = dtl.shrdp;
      // d.shrdotp = dtl.shrdotp;
      // d.shrdndp = dtl.shrdndp;
      // d.shrdndotp = dtl.shrdndotp;

      // d.lhrdp = dtl.lhrdp;
      // d.lhrdotp = dtl.lhrdotp;
      // d.lhrdndp = dtl.lhrdndp;
      // d.lhrdndotp = dtl.lhrdndotp;

      // d.lhp = dtl.lhp;
      // d.lhotp = dtl.lhotp;
      // d.lhndp = dtl.lhndp;
      // d.lhndotp = dtl.lhndotp;

      // d.lhshp = dtl.lhshp;
      // d.lhshotp = dtl.lhshotp;
      // d.lhshndp = dtl.lhshndp;
      // d.lhshndotp = dtl.lhshndotp;

      // d.lhshrdp = dtl.lhshrdp;
      // d.lhshrdotp = dtl.lhshrdotp;
      // d.lhshrdndp = dtl.lhshrdndp;
      // d.lhshrdndotp = dtl.lhshrdndotp;

      // d.hu100p = dtl.hu100p;

      // d.drhp = dtl.drhp;
      // d.drhotp = dtl.drhotp;
      // d.drhndp = dtl.drhndp;
      // d.drhndotp = dtl.drhndotp;

      // d.drhrdp = dtl.drhrdp;
      // d.drhrdotp = dtl.drhrdotp;
      // d.drhrdndp = dtl.drhrdndp;
      // d.drhrdndotp = dtl.drhrdndotp;

      // d.totalWorkingHour = dtl.totalWorkingHour;
      // // d.scheduleCode = dtl.scheduleCode;
      // d.approvedFormFlag = dtl.approvedFormFlag;
      // d.payableToMemberFlag = dtl.payableToMemberFlag;
      // d.billableToClientFlag = dtl.billableToClientFlag;
      // d.remarks = dtl.remarks;

      // this.visibleCombos = 1;

      // if (dtl.timekeepingScheduleIdA) this.visibleCombos = 2;
      // if (dtl.timekeepingScheduleIdB) this.visibleCombos = 3;
      // if (dtl.timekeepingScheduleIdC) this.visibleCombos = 4;
      // if (dtl.timekeepingScheduleIdD) this.visibleCombos = 5;
      // d.lockId = dtl.lockId;
     
      // this.offSetFlag = true
      // this.timekeeping.applyOffSetFlag = true;
      // this.isManualScheduleEditorVisible = true;
    },


    onAddManualSchedule () {
      const me = this;

      me.clearManualSchedulePad();
      me.schedule.sheetMemberId = -1;
      me.isManualScheduleEditorVisible = true;
      me.isAddingManualSchedule = true;
    },

    onDeleteManualSchedule (index) {
      const me = this;

      if (!me.schedules[index].lockId) {
        me.schedules.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('PayTimekeepingScheduleMember', me.schedules[index].timekeepingSheetId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Member for <b>' + me.schedules[index].timekeepingSheetId + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.schedules.splice(index, 1);
          }
          return;
        })
        .catch( fault => {
          me.showFault(fault);
        });

    },

    onSubmitManualSchedule () {
      const
        me = this,
        d = me.schedule;
        
      if (!me.isValid('pay0040A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }


      let dtl = {};

      if (me.isAddingManualSchedule) {

        Object.assign(dtl, d);
                
        me.schedules.push(dtl);
        dtl.timekeepingSheetId = me.timekeeping.timekeepingSheetId;
        me.clearManualSchedulePad();
        me.advice.info("Schedule for '" + dtl.scheduleDate + "' added to list.", { duration: 5 });
        me.setFocus('startDate');
      
      } else {
        dtl = me.schedules[me.scheduleIndex];
    
        dtl.memberId = d.memberId;
        dtl.memberName = d.memberName;
        dtl.timekeepingSheetId = d.timekeepingSheetId;
        dtl.scheduleDate = d.scheduleDate;
        dtl.noScheduleId = d.noScheduleId;
        dtl.dayTypeName = d.dayTypeName;

        dtl.timekeepingScheduleId = d.timekeepingScheduleId;
        dtl.timekeepingScheduleIdA = d.timekeepingScheduleIdA;
        dtl.timekeepingScheduleIdB = d.timekeepingScheduleIdB;
        dtl.timekeepingScheduleIdC = d.timekeepingScheduleIdC;
        dtl.timekeepingScheduleIdD = d.timekeepingScheduleIdD;

        //Unpaid Breaks

        let unpaidBreak = 0;

        me.scheduleList.forEach((dtl) => {          
          if (dtl.timekeepingScheduleId == d.timekeepingScheduleId) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });

        me.scheduleListA.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdA == d.timekeepingScheduleIdA) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListB.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdB == d.timekeepingScheduleIdB) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListC.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdC == d.timekeepingScheduleIdC) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListD.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdD == d.timekeepingScheduleIdD) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        dtl.unpaidBreak = unpaidBreak;
      
      
        dtl.scheduleTimeIn = d.scheduleTimeIn;
        dtl.scheduleTimeOut = d.scheduleTimeOut;
        dtl.scheduleDateTimeOut = d.scheduleDateTimeOut;
    
        dtl.approvedFormFlag = d.approvedFormFlag;
        dtl.payableToMemberFlag = d.payableToMemberFlag;
        dtl.billableToClientFlag = d.billableToClientFlag;
        dtl.remarks = d.remarks;

        dtl.scheduleCode = d.scheduleCode;

        dtl.totalWorkingHour = d.totalWorkingHour;

        dtl.regularHourP = d.regularHourP;
        dtl.overtimeHourP = d.overtimeHourP;
        dtl.ndp = d.ndp;
        dtl.ndotp = d.ndotp;
        dtl.lateMinuteP = d.lateMinuteP;
        dtl.undertimeMinuteP = d.undertimeMinuteP;

        dtl.rdp = d.rdp;
        dtl.rdotp = d.rdotp;
        dtl.rdndp = d.rdndp;
        dtl.rdndotp = d.rdndotp;

        dtl.shp = d.shp;
        dtl.shotp = d.shotp;
        dtl.shndp = d.shndp;
        dtl.shndotp = d.shndotp;

        dtl.shrdp = d.shrdp;
        dtl.shrdotp = d.shrdotp;
        dtl.shrdndp = d.shrdndp;
        dtl.shrdndotp = d.shrdndotp;

        dtl.lhrdp = d.lhrdp;
        dtl.lhrdotp = d.lhrdotp;
        dtl.lhrdndp = d.lhrdndp;
        dtl.lhrdndotp = d.lhrdndotp;

        dtl.lhp = d.lhp;
        dtl.lhotp = d.lhotp;
        dtl.lhndp = d.lhndp;
        dtl.lhndotp = d.lhndotp;

        dtl.lhshp = d.lhshp;
        dtl.lhshotp = d.lhshotp;
        dtl.lhshndp = d.lhshndp;
        dtl.lhshndotp = d.lhshndotp;

        dtl.lhshrdp = d.lhshrdp;
        dtl.lhshrdotp = d.lhshrdotp;
        dtl.lhshrdndp = d.lhshrdndp;
        dtl.lhshrdndotp = d.lhshrdndotp;

        dtl.hu100p = d.hu100p;

        dtl.drhp = d.drhp;
        dtl.drhotp = d.drhotp;
        dtl.drhndp = d.drhndp;
        dtl.drhndotp = d.drhndotp;

        dtl.drhrdp = d.drhrdp;
        dtl.drhrdotp = d.drhrdotp;
        dtl.drhrdndp = d.drhrdndp;
        dtl.drhrdndotp = d.drhrdndotp;

        dtl.lockId = d.lockId;

// Build scheduleCode list carefully
const scheduleNames = [];
let dayTypeNameList = '';

if (d.noScheduleId) {
  const found = me.noSchedules.find(s => s.noScheduleId == d.noScheduleId);
  if (found) dayTypeNameList = found.noScheduleName;
}

        dtl.dayTypeName = d.dayTypeNameRef + ' | ' + dayTypeNameList;


if (d.timekeepingScheduleId) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleId);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdA) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdA);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdB) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdB);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdC) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdC);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdD) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdD);
  if (found) scheduleNames.push(found.scheduleName);
}

dtl.scheduleCode = scheduleNames.join(' | ');


// const metrics = this.computeScheduleMetrics(dtl);

// dtl.regularHour = metrics.regularHour;
// dtl.overtimeHour = metrics.overtimeHour;
// dtl.lateMinute = metrics.lateMinute;
// dtl.undertimeMinute = metrics.undertimeMinute;

// if (this.timekeeping.applyOffSetFlag) {
//   this.onApplyRestDayOffSetting();
// }

// this.offSetFlag = true;
//   this.timekeeping.applyOffSetFlag =  true;
  this.offSetFlag = true;
  this.timekeeping.applyOffSetFlag =  true;
  // this.timekeeping.manualUploadFlag =  true;
  this.timekeeping.manualEditFlag =  true;
  me.isManualScheduleEditorVisible = false;
      }

    },




computeRegularHour(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut,unpaidBreak) {

  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes


  let scheduleRangesRegularHour = scheduleCode.split('|').map(range => range.trim());


  let totalRegularTimeRegularHour = 0;

  if (!scheduleCode || typeof scheduleCode !== 'string') {    
    totalRegularTimeRegularHour =  0;  
  }


  for (let range of scheduleRangesRegularHour) {
   
    if (range === 'RESTDAY') continue;


    let [startTime, endTime] = range.split('-').map(time => time.trim());


    let [startHours, startMinutes] = startTime.split(':').map(Number);
    let [endHours, endMinutes] = endTime.split(':').map(Number);

 
    let startTotalMinutes = startHours * 60 + startMinutes;
    let endTotalMinutes = endHours * 60 + endMinutes;


    if (endTotalMinutes < startTotalMinutes) {
      endTotalMinutes += 1440; 
    }

 
    let timeDifference = endTotalMinutes - startTotalMinutes;
    totalRegularTimeRegularHour += timeDifference;

  }
  
  let regularHour = (totalRegularTimeRegularHour/60).toFixed(1) - unpaidBreak; 


  if (regularHour < maximumRegularHourCount) {
    
    regularHour = (totalRegularTimeRegularHour/60).toFixed(1) ;

  }

return regularHour;
},


// computeUndertime(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut) {
//  // Convert MM/DD/YYYY to ISO format YYYY-MM-DD
  

// let scheduleStartTime = this.getFirstTimeInSchedule(scheduleCode);

//   // Split scheduleCode into blocks

//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);
//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleStartTime);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !timeOutDate) {
//     // console.warn('Invalid timeIn or timeOut');
//     return { undertimeMinute: 0 };
//   }

//   const timeInMs = timeInDate.getTime();
//   const timeOutMs = timeOutDate.getTime();

//   let scheduledMs = 0;
//   let workedWithinScheduleMs = 0;

//   blocks.forEach((block, index) => {
//     const [startStr, endStr] = block.split('-').map(s => s.trim());
//     if (!startStr || !endStr) {
//       console.warn('Invalid block format:', block);
//       return;
//     }

//     // Set schedule date: first block = scheduleDate, rest = +1 day
//     // const blockDate = index === 0 ? isoDateIn : addDays(isoDateIn, 1);

//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle blocks that cross midnight
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // If block start was before midnight (i.e., early hours like 01:00), shift to next day
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     const blockDuration = blockEnd.getTime() - blockStart.getTime();
//     scheduledMs += blockDuration;

//     const overlapStart = Math.max(timeInMs, blockStart.getTime());
//     const overlapEnd = Math.min(timeOutMs, blockEnd.getTime());
//     const overlap = Math.max(0, overlapEnd - overlapStart);
//     workedWithinScheduleMs += overlap;

//     console.log(
//       `Block: ${block} | Scheduled (min): ${blockDuration / 60000} | Overlap (min): ${overlap / 60000}`
//     );
//   });

//   const undertimeMs = scheduledMs - workedWithinScheduleMs;
//   const undertimeMinute = Math.max(0, Math.round(undertimeMs / 60000));

//   console.log('Total Scheduled (min):', scheduledMs / 60000);
//   console.log('Total Worked Within Schedule (min):', workedWithinScheduleMs / 60000);
//   console.log('Undertime (min):', undertimeMinute);



//   return undertimeMinute;//+(overtimeNDMinutes / 60).toFixed(2);
// },

// computeUndertime(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !timeOutDate) {
//     return { undertimeMinute: 0 };
//   }

//   const timeInMs = timeInDate.getTime();
//   const timeOutMs = timeOutDate.getTime();

//   let undertimeMinute = 0;

//   let blockDay = new Date(isoDateIn); // start from scheduleDate
//   let prevStartMinutes = this.parseTimeToMinutes('00:00');

//   blocks.forEach((block, index) => {
//     const [startStr, endStr] = block.split('-').map(s => s.trim());

//     const currentStartMinutes = this.parseTimeToMinutes(startStr);

//     // Shift to next day if current block starts earlier than previous
//     if (index > 0 && currentStartMinutes <= prevStartMinutes) {
//       blockDay = this.addDays(blockDay, 1);
//     }

//     prevStartMinutes = currentStartMinutes;

//     let blockStart = this.parseDateTime(blockDay, startStr);
//     let blockEnd = this.parseDateTime(blockDay, endStr);

//     // Cross-midnight adjustment
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     const blockDurationMs = blockEnd.getTime() - blockStart.getTime();

//     const overlapStart = Math.max(timeInMs, blockStart.getTime());
//     const overlapEnd = Math.min(timeOutMs, blockEnd.getTime());
//     const overlapMs = Math.max(0, overlapEnd - overlapStart);

//     const isBlockFullyCovered = overlapMs >= blockDurationMs;

//     console.log(`Block: ${startStr}-${endStr} | Scheduled (min): ${blockDurationMs / 60000} | Overlap (min): ${overlapMs / 60000}`);

//     if (!isBlockFullyCovered) {
//       undertimeMinute += Math.round((blockDurationMs - overlapMs) / 60000);
//     }
//   });

//   console.log('Final Undertime (min):', undertimeMinute);

//   return undertimeMinute;
// },

computeUndertime(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {

  // Convert MM/DD/YYYY to ISO if needed (assuming you have this.toISODate)
  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);

  const scheduleStartTime = this.getFirstTimeInSchedule(scheduleCode);

  const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

  const timeInDate = this.parseDateTime(isoDateIn, scheduleStartTime);
  const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
  // console.log('Parsed Time In:', timeInDate?.toString());
  // console.log('Parsed Time Out:', timeOutDate?.toString());

  if (!timeInDate || !timeOutDate) {
    return 0;
  }

  const timeInMs = timeInDate.getTime();
  const timeOutMs = timeOutDate.getTime();

  let scheduledMs = 0;
  let workedWithinScheduleMs = 0;

blocks.forEach((block, index) => {
  const [startStr, endStr] = block.split('-').map(s => s.trim());
  // ...

  let blockStart = this.parseDateTime(isoDateIn, startStr);
  let blockEnd = this.parseDateTime(isoDateIn, endStr);

  if (blockEnd <= blockStart) {
    blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
  }

  // Only shift blocks that start before 06:00 **and are NOT the first block**
  if (index !== 0 && this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
    const nextDay = this.addDays(isoDateIn, 1);
    blockStart = this.parseDateTime(nextDay, startStr);
    blockEnd = this.parseDateTime(nextDay, endStr);
    if (blockEnd <= blockStart) {
      blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
    }
 
  }
    const blockDuration = blockEnd.getTime() - blockStart.getTime();
    scheduledMs += blockDuration;

    const overlapStart = Math.max(timeInMs, blockStart.getTime());
    const overlapEnd = Math.min(timeOutMs, blockEnd.getTime());
    const overlap = Math.max(0, overlapEnd - overlapStart);
    workedWithinScheduleMs += overlap;
  });

  const undertimeMs = scheduledMs - workedWithinScheduleMs;
  const undertimeMinute = Math.max(0, Math.round(undertimeMs / 60000));

  return undertimeMinute;
},



computeOvertime(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
  const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);
  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);

  const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

  const minimumRequiredOvertimeCount = this.timekeeping.minimumRequiredOvertimeCount; // Late counting interval in minutes
  const overtimeDurationCount = this.timekeeping.overtimeDurationCount; // Late counting interval in minutes

  if (!timeOutDate) {
    // console.warn('Invalid timeOut');
    return 0;
  }

  // Get the last block's end time
  const lastBlock = blocks[blocks.length - 1];
  const [startStr, endStr] = lastBlock.split('-').map(s => s.trim());

  let lastBlockStart = this.parseDateTime(isoDateIn, startStr);
  let lastBlockEnd = this.parseDateTime(isoDateIn, endStr);

  if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
    // Block belongs to next day
    lastBlockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
    lastBlockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
  }

  if (lastBlockEnd <= lastBlockStart) {
    lastBlockEnd = new Date(lastBlockEnd.getTime() + 24 * 60 * 60 * 1000); // handle overnight
  }

  const overtimeMs = timeOutDate.getTime() - lastBlockEnd.getTime();
  const overtimeHour = Math.max(0, overtimeMs / (1000 * 60 * 60));
  let overtimeMinutes = Math.max(0, overtimeMs / 60000);


  // Count overtime minutes only if it meets minimum required, and count by overtimeDurationCount intervals
  if (overtimeMinutes >= minimumRequiredOvertimeCount) {
    // Floor to nearest count interval (e.g., 15-minute blocks)
    overtimeMinutes = Math.floor(overtimeMinutes / overtimeDurationCount) * overtimeDurationCount;
  } else {
    overtimeMinutes = 0;
  }

 // Convert to hours
 let overtimeHours = overtimeMinutes / 60;

  // const overtimeMinutes = Math.max(0, overtimeMs / 60000);
  // const minimumRequiredOvertimeCount = this.timekeeping.minimumRequiredOvertimeCount; // Late counting interval in minutes
  // const overtimeDurationCount = this.timekeeping.overtimeDurationCount; // Late counting interval in minutes


  // if (overtimeMinutes >= minimumRequiredOvertimeCount) {
  //    overtimeMinutes = overtimeMinutes / overtimeDurationCount; count overtimeMinutes every  overtimeDurationCount interval
  // }

  // convert overtimeMinutes to hours

  // console.log('overtimeMinutes', overtimeMinutes);
  // console.log('overtimeMs', overtimeMs);
  // console.log('Schedule End:', lastBlockEnd);
  // console.log('Actual Time Out:', timeOutDate);
  // console.log('Overtime (hours):', overtimeHour.toFixed(2));



  return parseFloat(overtimeHours.toFixed(2));
},

//ORIG
// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00'; // 10 PM
//   const ND_END = '06:00';   // 6 AM next day

//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);
//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   // Parse actual time in/out
//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
//   if (!timeInDate || !timeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Parse ND window start/end as Date objects
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000); // next day 6AM
//   }

//   let totalNdMs = 0;

//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());

//     // Parse block start and end dates
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle overnight blocks crossing midnight
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Shift block to next day if start is before 6AM (ND_END)
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Calculate overlap of block with actual worked time
//     const actualStartMs = Math.max(timeInDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(timeOutDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return; // no overlap

//     // Calculate overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += (ndOverlapEnd - ndOverlapStart);
//     }
//   });

//   const ndMinutes = Math.round(totalNdMs / 60000); // convert ms to minutes
//   console.log('Night Differential Minutes:', ndMinutes);
//   return ndMinutes;
// },

//Sept 09172025
// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes

//   const MAX_REGULAR_MINUTES = maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   let timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !timeOutDate) {
//     // console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Cap timeOut to only 8 hours (regular time) from timeIn
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);
//   if (timeOutDate > maxRegularEndDate) {
//     timeOutDate = maxRegularEndDate;
//   }

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   let totalNdMs = 0;

//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle blocks crossing midnight
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Handle blocks that start before 6 AM (e.g. next day early shift)
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Calculate overlap between actual worked time and block
//     const actualStartMs = Math.max(timeInDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(timeOutDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Calculate overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       // console.log(`[ND Overlap] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   const ndMinutes = Math.round(totalNdMs / 60000);
//   // console.log('Night Differential Minutes (within 8hr regular):', ndMinutes);
//   return ndMinutes;
// },

// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   console.log('Parsed timeInDate:', timeInDate);
//   console.log('Parsed timeOutDate:', timeOutDate);

//   if (!timeInDate || !timeOutDate) {
//     console.log('Invalid timeInDate or timeOutDate');
//     return 0;
//   }

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // add a day
//   }

//   console.log('Night Differential window:', ndStart, 'to', ndEnd);

//   // Parse and sort schedule blocks chronologically
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       let blockStart = this.parseDateTime(isoDateIn, startStr);
//       let blockEnd = this.parseDateTime(isoDateIn, endStr);

//       // Handle overnight block
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       // For early morning schedules (before 06:00), treat as next day
//       if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//         blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//         blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//         if (blockEnd <= blockStart) {
//           blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//         }
//       }

//       console.log(`Block parsed: ${startStr}-${endStr} => ${blockStart} to ${blockEnd}`);
//       return { start: blockStart, end: blockEnd };
//     })
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const [index, block] of blocks.entries()) {
//     if (block.end <= timeInDate || block.start >= timeOutDate) {
//       console.log(`Block #${index + 1} skipped: outside actual worked time`);
//       continue; // outside of actual work
//     }

//     // Overlap with actual time in/out
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) {
//       console.log(`Block #${index + 1} skipped: no overlap with actual worked time`);
//       continue;
//     }

//     // Trim to remaining allowed scheduled time
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) {
//       console.log('Reached max regular minutes cap; stopping further calculations');
//       break; // hit 8-hr cap
//     }

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//       console.log(`Block #${index + 1} duration trimmed to remaining allowed minutes: ${durationMs / 60000} minutes`);
//     }

//     totalScheduledMs += durationMs;

//     // Compute ND overlap within this trimmed block
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const ndDuration = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += ndDuration;
//       console.log(`Block #${index + 1} ND overlap: ${ndDuration / 60000} minutes`);
//     } else {
//       console.log(`Block #${index + 1} no ND overlap`);
//     }
//   }

//   console.log('Total scheduled minutes considered:', totalScheduledMs / 60000);
//   console.log('Total night differential minutes:', totalNdMs / 60000);

//   return Math.round(totalNdMs / 60000); // return in minutes
// },

// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480 (8 hrs)

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   console.log('Parsed timeInDate:', timeInDate);
//   console.log('Parsed timeOutDate:', timeOutDate);

//   if (!timeInDate || !timeOutDate) return 0;

//   // Night Differential window: 22:00 current day to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000);
//   }
//   console.log('Night Differential window:', ndStart, 'to', ndEnd);

//   // Parse schedule blocks and assign dates correctly
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       // If start time < 06:00, treat block as next day (shift date)
//       const startMinutes = this.parseTimeToMinutes(startStr);
//       let blockStartDate = isoDateIn;
//       if (startMinutes < this.parseTimeToMinutes('06:00')) {
//         blockStartDate = this.addDays(isoDateIn, 1);
//       }

//       let blockStart = this.parseDateTime(blockStartDate, startStr);
//       let blockEnd = this.parseDateTime(blockStartDate, endStr);

//       // Handle overnight block (if end <= start, add 1 day)
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       console.log(`Block parsed: ${startStr}-${endStr} => ${blockStart} to ${blockEnd}`);
//       return { start: blockStart, end: blockEnd };
//     })
//     // Sort blocks chronologically
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const block of blocks) {
//     // Ignore blocks outside actual working time
//     if (block.end <= timeInDate || block.start >= timeOutDate) continue;

//     // Compute overlap of actual worked time with this block
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) continue;

//     // Limit duration to remaining allowed regular minutes
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) {
//       console.log('Reached max regular minutes cap; stopping further calculations');
//       break;
//     }

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//     }

//     totalScheduledMs += durationMs;

//     // Calculate overlap with ND window inside this time slice
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const ndBlockMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += ndBlockMs;
//       console.log(`ND overlap in block: ${new Date(ndOverlapStart)} to ${new Date(ndOverlapEnd)} (${Math.round(ndBlockMs / 60000)} minutes)`);
//     } else {
//       console.log('Block no ND overlap');
//     }
//   }

//   const totalNdMinutes = Math.round(totalNdMs / 60000);
//   console.log('Total scheduled minutes considered:', totalScheduledMs / 60000);
//   console.log('Total night differential minutes:', totalNdMinutes);

//   return totalNdMinutes;
// },

// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
 
//   if (!timeInDate || !timeOutDate) return 0;

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // add a day
//   }

//   // Parse and sort schedule blocks chronologically
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       // Parse tentative block start/end on scheduleDate
//       let blockStart = this.parseDateTime(isoDateIn, startStr);
//       let blockEnd = this.parseDateTime(isoDateIn, endStr);

//       // Handle overnight block that crosses midnight
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       // Refined rule for early morning blocks (start before 06:00)
//       if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//         // Only treat as next day if block ends <= 07:00 (early morning block)
//         if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes('07:00')) {
//           blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//           blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//           if (blockEnd <= blockStart) {
//             blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//           }
//         }
//         // else keep on scheduleDate (do nothing)
//       }

//       return { start: blockStart, end: blockEnd };
//     })
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const block of blocks) {
//     console.log(`Block parsed: ${block.start.toLocaleTimeString()} to ${block.end.toLocaleTimeString()} on ${block.start.toDateString()}`);

//     if (block.end <= timeInDate || block.start >= timeOutDate) {
//       console.log('Block outside actual work time; skipping.');
//       continue; // outside of actual work time
//     }

//     // Overlap with actual time in/out
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) {
//       console.log('No overlap duration; skipping block.');
//       continue;
//     }

//     // Trim to remaining allowed scheduled time
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) {
//       console.log('Reached max regular minutes cap; stopping further calculations');
//       break; // hit max regular minutes cap
//     }

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//     }

//     totalScheduledMs += durationMs;

// console.log('ND window start:', ndStart);
// console.log('ND window end:', ndEnd);
// console.log('Actual block start:', actualStart);
// console.log('Actual block end:', actualEnd);

//     // Compute ND overlap within this trimmed block
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//       console.log(`ND overlap for this block: ${(ndOverlapEnd - ndOverlapStart) / 60000} minutes`);
//     } else {
//       console.log('Block no ND overlap');
//     }
//   }

//   console.log(`Total scheduled minutes considered: ${totalScheduledMs / 60000}`);
//   console.log(`Total night differential minutes: ${totalNdMs / 60000}`);

//   return Math.round(totalNdMs / 60000); // return in minutes
// },

// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !timeOutDate) return 0;

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // add a day
//   }

//   // Parse and sort schedule blocks chronologically
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       let blockStart = this.parseDateTime(isoDateIn, startStr);
//       let blockEnd = this.parseDateTime(isoDateIn, endStr);

//       // Handle overnight block
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       // For early morning schedules (before 06:00), treat as next day
//       if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//         blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//         blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//         if (blockEnd <= blockStart) {
//           blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//         }
//       }

//       return { start: blockStart, end: blockEnd };
//     })
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const block of blocks) {
//     if (block.end <= timeInDate || block.start >= timeOutDate) continue; // outside of actual work

//     // Overlap with actual time in/out
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) continue;

//     // Trim to remaining allowed scheduled time
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) break; // hit 8-hr cap

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//     }

//     totalScheduledMs += durationMs;

//     // Compute ND overlap within this trimmed block
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000); // return in minutes
// },

// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   console.log('Input parameters:', { scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut });
//   console.log('Parsed timeInDate:', timeInDate);
//   console.log('Parsed timeOutDate:', timeOutDate);

//   if (!timeInDate || !timeOutDate) {
//     console.log('Invalid timeInDate or timeOutDate');
//     return 0;
//   }

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // add a day
//   }

//   console.log('Night Differential window:', ndStart, 'to', ndEnd);

//   // Parse and sort schedule blocks chronologically
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       let blockStart = this.parseDateTime(isoDateIn, startStr);
//       let blockEnd = this.parseDateTime(isoDateIn, endStr);

//       // Handle overnight block
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       // For early morning schedules (before 06:00), treat as next day
//       if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//         blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//         blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//         if (blockEnd <= blockStart) {
//           blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//         }
//       }

//       console.log(`Block parsed: ${blockStart.toLocaleTimeString()} to ${blockEnd.toLocaleTimeString()} on ${blockStart.toDateString()}`);

//       return { start: blockStart, end: blockEnd };
//     })
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const block of blocks) {
//     if (block.end <= timeInDate || block.start >= timeOutDate) {
//       console.log('Block outside actual work time; skipping:', block.start.toLocaleTimeString(), '-', block.end.toLocaleTimeString());
//       continue; // outside of actual work
//     }

//     // Overlap with actual time in/out
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) {
//       console.log('No overlap duration; skipping block:', actualStart.toLocaleTimeString(), '-', actualEnd.toLocaleTimeString());
//       continue;
//     }

//     // Trim to remaining allowed scheduled time
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) {
//       console.log('Reached max regular minutes cap; stopping further calculations');
//       break; // hit 8-hr cap
//     }

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//     }

//     totalScheduledMs += durationMs;

//     console.log('Actual block start:', actualStart.toLocaleString());
//     console.log('Actual block end:', actualEnd.toLocaleString());
//     console.log('Block duration (minutes):', durationMs / 60000);

//     // Compute ND overlap within this trimmed block
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMinutes = (ndOverlapEnd - ndOverlapStart) / 60000;
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//       console.log(`ND overlap for this block: ${overlapMinutes} minutes`);
//     } else {
//       console.log('Block no ND overlap');
//     }
//   }

//   console.log(`Total scheduled minutes considered: ${totalScheduledMs / 60000}`);
//   console.log(`Total night differential minutes: ${totalNdMs / 60000}`);

//   return Math.round(totalNdMs / 60000); // return in minutes
// },

computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
  const ND_START = '22:00';
  const ND_END = '06:00';
  const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

  // console.log('Input parameters:', {scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut});

  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);

  const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
  const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

  if (!timeInDate || !timeOutDate) {
    // console.log('Invalid timeInDate or timeOutDate.');
    return 0;
  }

  // console.log('Parsed timeInDate:', timeInDate);
  // console.log('Parsed timeOutDate:', timeOutDate);

  // Parse and sort schedule blocks chronologically
  const blocks = scheduleCode
    .split('|')
    .map(b => b.trim())
    .filter(b => b.length > 0)
    .map(block => {
      let [startStr, endStr] = block.split('-').map(s => s.trim());

      // Parse tentative block start/end on scheduleDate
      let blockStart = this.parseDateTime(isoDateIn, startStr);
      let blockEnd = this.parseDateTime(isoDateIn, endStr);

      // Handle overnight block that crosses midnight
      if (blockEnd <= blockStart) {
        blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
      }

      // Refined rule for early morning blocks (start before 06:00)
      if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
        // Only treat as next day if block ends <= 07:00 (early morning block)
        if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes('07:00')) {
          blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
          blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
          if (blockEnd <= blockStart) {
            blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
          }
          // console.log(`Early morning block treated as next day: ${startStr}-${endStr}`);
        } else {
          // console.log(`Early morning block kept on scheduleDate: ${startStr}-${endStr}`);
        }
      }

      // console.log(`Block parsed: ${blockStart.toLocaleTimeString()} to ${blockEnd.toLocaleTimeString()} on ${blockStart.toDateString()}`);

      return { start: blockStart, end: blockEnd };
    })
    .sort((a, b) => a.start - b.start);

  let totalScheduledMs = 0;
  let totalNdMs = 0;

  // Helper to convert Date to minutes since midnight
  function timeToMinutes(date) {
    return date.getHours() * 60 + date.getMinutes();
  }

  // ND window in minutes (time only, no date)
  const ndStartMinutes = 22 * 60; // 22:00 => 1320
  const ndEndMinutes = 6 * 60;    // 06:00 => 360

  // Helper to calculate overlap between two time intervals (minutes), considering crossing midnight
  function calcOverlap(startA, endA, startB, endB) {
    if (endA <= startA) endA += 1440; // crosses midnight
    if (endB <= startB) endB += 1440;

    const overlapStart = Math.max(startA, startB);
    const overlapEnd = Math.min(endA, endB);

    return Math.max(0, overlapEnd - overlapStart);
  }

  for (const block of blocks) {
    if (block.end <= timeInDate || block.start >= timeOutDate) {
      // console.log('Block outside actual work time; skipping:', block);
      continue; // outside of actual work time
    }

    // Overlap with actual time in/out
    const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
    const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

    let durationMs = actualEnd - actualStart;
    if (durationMs <= 0) {
      // console.log('No overlap duration; skipping block:', block);
      continue;
    }

    // Trim to remaining allowed scheduled time
    const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
    if (remainingMs <= 0) {
      // console.log('Reached max regular minutes cap; stopping further calculations');
      break; // hit max regular minutes cap
    }

    if (durationMs > remainingMs) {
      durationMs = remainingMs;
      actualEnd.setTime(actualStart.getTime() + durationMs);
    }

    totalScheduledMs += durationMs;

    // console.log('Actual block start:', actualStart);
    // console.log('Actual block end:', actualEnd);
    // console.log('Block duration (minutes):', durationMs / 60000);

    // Time only minutes for actual start/end
    const actualStartMinutes = timeToMinutes(actualStart);
    const actualEndMinutes = timeToMinutes(actualEnd);

    let ndOverlapMinutes = 0;

    if (actualEndMinutes < actualStartMinutes) {
      // Work block crosses midnight
      ndOverlapMinutes += calcOverlap(actualStartMinutes, actualEndMinutes + 1440, ndStartMinutes, ndEndMinutes + 1440);
    } else {
      ndOverlapMinutes += calcOverlap(actualStartMinutes, actualEndMinutes, ndStartMinutes, 1440); // 22:00 to midnight
      ndOverlapMinutes += calcOverlap(actualStartMinutes, actualEndMinutes, 0, ndEndMinutes);      // midnight to 06:00
    }

    if (ndOverlapMinutes > 0) {
      totalNdMs += ndOverlapMinutes * 60000;
      // console.log(`ND overlap for this block: ${ndOverlapMinutes} minutes`);
    } else {
      // console.log('Block no ND overlap');
    }
  }

  // console.log(`Total scheduled minutes considered: ${totalScheduledMs / 60000}`);
  // console.log(`Total night differential minutes: ${totalNdMs / 60000}`);

  return Math.round(totalNdMs / 60000); // return in minutes
},


// computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; // usually 480

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !timeOutDate) return 0;

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // add a day
//   }

//   // Parse and sort schedule blocks chronologically
//   const blocks = scheduleCode
//     .split('|')
//     .map(b => b.trim())
//     .filter(b => b.length > 0)
//     .map(block => {
//       let [startStr, endStr] = block.split('-').map(s => s.trim());

//       let blockStart = this.parseDateTime(isoDateIn, startStr);
//       let blockEnd = this.parseDateTime(isoDateIn, endStr);

//       // Handle overnight block
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }

//       // For early morning schedules (before 06:00), treat as next day
//       if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//         blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//         blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//         if (blockEnd <= blockStart) {
//           blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//         }
//       }

//       return { start: blockStart, end: blockEnd };
//     })
//     .sort((a, b) => a.start - b.start);

//   let totalScheduledMs = 0;
//   let totalNdMs = 0;

//   for (const block of blocks) {
//     if (block.end <= timeInDate || block.start >= timeOutDate) continue; // outside of actual work

//     // Overlap with actual time in/out
//     const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
//     const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

//     let durationMs = actualEnd - actualStart;
//     if (durationMs <= 0) continue;

//     // Trim to remaining allowed scheduled time
//     const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
//     if (remainingMs <= 0) break; // hit 8-hr cap

//     if (durationMs > remainingMs) {
//       durationMs = remainingMs;
//       actualEnd.setTime(actualStart.getTime() + durationMs);
//     }

//     totalScheduledMs += durationMs;

//     // Compute ND overlap within this trimmed block
//     const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
//     const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000); // return in minutes
// },

//09172025
// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes

//   const MAX_REGULAR_MINUTES = maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     // console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   if (fullTimeOutDate <= maxRegularEndDate) {
//     return 0; // no overtime
//   }

//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   // Convert blocks to Date ranges, accounting for overnight blocks and next day
//   const blockIntervals = blocks.map(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }
//     return { start: blockStart, end: blockEnd };
//   });

//   // Find last scheduled block end
//   let lastScheduledEnd = blockIntervals.reduce((max, b) => b.end > max ? b.end : max, new Date(0));

//   let totalNdMs = 0;

//   // Only count overtime that happens inside scheduled blocks (ignore gaps)

//   blockIntervals.forEach(({ start, end }) => {
//     // Find overlap between overtime period and this block
//     const otBlockStart = Math.max(otStartDate.getTime(), start.getTime());
//     const otBlockEnd = Math.min(otEndDate.getTime(), end.getTime());

//     if (otBlockEnd <= otBlockStart) return;

//     // Find overlap with ND window
//     const ndOverlapStart = Math.max(otBlockStart, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(otBlockEnd, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//     }
//   });

//   // Also count ND OT after last scheduled block, if any
//   if (otEndDate > lastScheduledEnd) {
//     const afterLastSchedStart = Math.max(otStartDate.getTime(), lastScheduledEnd.getTime());
//     const afterLastSchedEnd = otEndDate.getTime();

//     const ndOverlapStart = Math.max(afterLastSchedStart, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(afterLastSchedEnd, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000);
// },


// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   if (fullTimeOutDate <= maxRegularEndDate) {
//     return 0;
//   }

//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   // Find last scheduled end time
//   let lastScheduledEndDate = null;
//   blocks.forEach(block => {
//     let [, endStr] = block.split('-').map(s => s.trim());
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);
//     if (this.parseTimeToMinutes(endStr) < this.parseTimeToMinutes('06:00')) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }
//     if (!lastScheduledEndDate || blockEnd > lastScheduledEndDate) {
//       lastScheduledEndDate = blockEnd;
//     }
//   });

//   let totalNdMs = 0;

//   // Calculate ND OT inside scheduled blocks ONLY
//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//     }
//   });

//   // ND OT outside scheduled blocks ONLY AFTER last scheduled block end time
//   if (otEndDate > lastScheduledEndDate) {
//     const afterScheduleStart = Math.max(otStartDate.getTime(), lastScheduledEndDate.getTime());
//     const afterScheduleEnd = otEndDate.getTime();

//     const ndOverlapStart = Math.max(afterScheduleStart, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(afterScheduleEnd, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//     }
//   }

//   return Math.round(totalNdMs / 60000);
// },


// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   // Helper to parse schedule blocks and find last scheduled end datetime
//   const getLastScheduledEnd = (isoDate, blocks) => {
//     let lastEnd = null;
//     blocks.forEach(block => {
//       let [, endStr] = block.split('-').map(s => s.trim());
//       let blockEnd = this.parseDateTime(isoDate, endStr);
//       if (this.parseTimeToMinutes(endStr) < this.parseTimeToMinutes('06:00')) {
//         // Overnight block, add 1 day
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//       if (!lastEnd || blockEnd > lastEnd) {
//         lastEnd = blockEnd;
//       }
//     });
//     return lastEnd;
//   };

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Calculate if there's any overtime
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   console.log(`Time In: ${timeInDate}`);
//   console.log(`Full Time Out: ${fullTimeOutDate}`);
//   console.log(`Max Regular End: ${maxRegularEndDate}`);

//   if (fullTimeOutDate <= maxRegularEndDate) {
//     console.log('[INFO] No overtime — so no ND OT minutes.');
//     return 0;
//   }

//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window: 22:00 of schedule date to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   // Find last scheduled block end time
//   const lastScheduledEndDate = getLastScheduledEnd(isoDateIn, blocks);

//   let totalNdMs = 0;

//   // Calculate ND OT within scheduled blocks only
//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       // Overnight block
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       // Overnight block starts after midnight
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Calculate overlap between scheduled block and overtime window
//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Calculate overlap with ND window inside the block overtime window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       console.log(`[ND OT Overlap inside schedule] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   // Calculate ND OT outside schedule blocks ONLY AFTER last scheduled end time
//   if (otEndDate > lastScheduledEndDate) {
//     const afterScheduleStart = Math.max(otStartDate.getTime(), lastScheduledEndDate.getTime());
//     const afterScheduleEnd = otEndDate.getTime();

//     // ND window overlap after schedule end
//     const ndOverlapStart = Math.max(afterScheduleStart, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(afterScheduleEnd, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       console.log(`[ND OT Overlap outside schedule after last end] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   }

//   const ndOtMinutes = Math.round(totalNdMs / 60000);
//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);

//   return ndOtMinutes;
// },



// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Calculate if there's any overtime
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   // Logs for debugging
//   console.log(`Time In: ${timeInDate}`);
//   console.log(`Full Time Out: ${fullTimeOutDate}`);
//   console.log(`Max Regular End: ${maxRegularEndDate}`);

//   if (fullTimeOutDate <= maxRegularEndDate) {
//     console.log('[INFO] No overtime — so no ND OT minutes.');
//     return 0;
//   }

//   // Overtime starts after the regular end
//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window: 22:00 of schedule date to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   let totalNdMsInBlocks = 0;

//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle overnight blocks
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Adjust blocks that start before 6AM (assume next day)
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Get the overlap between this shift block and the overtime period
//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Find overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMsInBlocks += overlapMs;
//       console.log(`[ND OT Overlap] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   // Calculate ND overlap outside schedule blocks to avoid double counting
//   const otStartMs = otStartDate.getTime();
//   const otEndMs = otEndDate.getTime();
//   const ndStartMs = ndStartDate.getTime();
//   const ndEndMs = ndEndDate.getTime();

//   // Total ND overlap in entire OT window
//   const ndOverlapStart = Math.max(otStartMs, ndStartMs);
//   const ndOverlapEnd = Math.min(otEndMs, ndEndMs);

//   let totalNdMs = totalNdMsInBlocks; // start with blocks overlap

//   if (ndOverlapEnd > ndOverlapStart) {
//     const overlapMs = ndOverlapEnd - ndOverlapStart;
//     // Only add ND OT minutes outside blocks (no double counting)
//     const outsideOverlapMs = Math.max(0, overlapMs - totalNdMsInBlocks);
//     if (outsideOverlapMs > 0) {
//       totalNdMs += outsideOverlapMs;
//       console.log(`[ND OT Outside Schedule Block] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(outsideOverlapMs / 60000)} mins`);
//     }
//   }

//   const ndOtMinutes = Math.round(totalNdMs / 60000);
//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },




// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Calculate max regular end datetime
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   // Logs
//   console.log(`Time In: ${timeInDate}`);
//   console.log(`Full Time Out: ${fullTimeOutDate}`);
//   console.log(`Max Regular End: ${maxRegularEndDate}`);

//   // If no overtime, return 0
//   if (fullTimeOutDate <= maxRegularEndDate) {
//     console.log('[INFO] No overtime — so no ND OT minutes.');
//     return 0;
//   }

//   // Overtime window
//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window (22:00 to 06:00 next day)
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   let totalNdMs = 0;

//   // Calculate ND OT minutes inside schedule blocks
//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Overnight block adjustment
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Adjust blocks starting before 06:00 to next day
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Overlap between block and overtime window
//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       console.log(`[ND OT Overlap] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   // **Calculate ND OT minutes outside schedule blocks**
//   const otStartMs = otStartDate.getTime();
//   const otEndMs = otEndDate.getTime();

//   const ndStartMs = ndStartDate.getTime();
//   const ndEndMs = ndEndDate.getTime();

//   // Overlap of entire OT window with ND window
//   const ndOverlapStart = Math.max(otStartMs, ndStartMs);
//   const ndOverlapEnd = Math.min(otEndMs, ndEndMs);

//   if (ndOverlapEnd > ndOverlapStart) {
//     const overlapMs = ndOverlapEnd - ndOverlapStart;
//     totalNdMs += overlapMs;
//     console.log(`[ND OT Outside Schedule Block] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//   }

//   // Convert total ms to minutes
//   const ndOtMinutes = Math.round(totalNdMs / 60000);
//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },


//ORIG
// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeOut, scheduleDateTimeOut, lastScheduledEndTime) {
//   const ND_START = '22:00'; // 10 PM
//   const ND_END = '06:00';   // 6 AM next day

//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   console.log('[INFO] ISO Date Out:', isoDateOut);
//   console.log('[INFO] scheduleTimeOut:', scheduleTimeOut);
//   console.log('[INFO] lastScheduledEndTime:', lastScheduledEndTime);

//   // Step 1: Parse last scheduled end datetime
//   let lastScheduledEndDate = this.parseDateTime(isoDateOut, lastScheduledEndTime);
//   console.log('[DEBUG] Initial lastScheduledEndDate:', lastScheduledEndDate);

//   // Step 2: Parse actual time out datetime
//   let timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
//   console.log('[DEBUG] Initial timeOutDate:', timeOutDate);

//   // Step 3: Adjust lastScheduledEndDate if it is before 6 AM (belongs to previous day)
//   if (this.parseTimeToMinutes(lastScheduledEndTime) < this.parseTimeToMinutes('06:00')) {
//     lastScheduledEndDate = this.parseDateTime(this.addDays(isoDateOut, -1), lastScheduledEndTime);
//     console.log('[ADJUSTED] lastScheduledEndDate shifted to previous day:', lastScheduledEndDate);
//   }

//   // Step 4: Adjust timeOutDate if it is before or equal to lastScheduledEndDate (i.e. next day)
//   if (timeOutDate <= lastScheduledEndDate) {
//     timeOutDate = this.parseDateTime(this.addDays(isoDateOut, 1), scheduleTimeOut);
//     console.log('[ADJUSTED] timeOutDate shifted to next day:', timeOutDate);
//   }

//   // Step 5: No overtime if timeOut <= lastScheduledEnd
//   if (timeOutDate <= lastScheduledEndDate) {
//     console.log('[RESULT] No overtime — returning 0 minutes');
//     return 0;
//   }

//   // Step 6: Define ND window (from 22:00 today to 06:00 next day)
//   const ndStartDate = this.parseDateTime(isoDateOut, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateOut, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000); // move to next day
//   }

//   console.log('[INFO] ND Start:', ndStartDate);
//   console.log('[INFO] ND End:', ndEndDate);

//   // Step 7: Calculate overlap between OT and ND window
//   const otStartMs = lastScheduledEndDate.getTime();
//   const otEndMs = timeOutDate.getTime();

//   console.log('[DEBUG] OT Start (ms):', otStartMs, new Date(otStartMs));
//   console.log('[DEBUG] OT End (ms):', otEndMs, new Date(otEndMs));

//   const ndStartMs = ndStartDate.getTime();
//   const ndEndMs = ndEndDate.getTime();

//   const ndOverlapStart = Math.max(otStartMs, ndStartMs);
//   const ndOverlapEnd = Math.min(otEndMs, ndEndMs);

//   console.log('[DEBUG] ND Overlap Start (ms):', ndOverlapStart, new Date(ndOverlapStart));
//   console.log('[DEBUG] ND Overlap End (ms):', ndOverlapEnd, new Date(ndOverlapEnd));

//   // Step 8: If no overlap
//   if (ndOverlapEnd <= ndOverlapStart) {
//     console.log('[RESULT] No ND-OT overlap — returning 0 minutes');
//     return 0;
//   }

//   // Step 9: Calculate ND OT minutes
//   const ndOtMs = ndOverlapEnd - ndOverlapStart;
//   const ndOtMinutes = Math.round(ndOtMs / 60000);

//   console.log('[RESULT] Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },

// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);


//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Calculate if there's any overtime
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   // **Add logs here:**
//   console.log(`Time In: ${timeInDate}`);
//   console.log(`Full Time Out: ${fullTimeOutDate}`);
//   console.log(`Max Regular End: ${maxRegularEndDate}`);


//   if (fullTimeOutDate <= maxRegularEndDate) {
//     console.log('[INFO] No overtime — so no ND OT minutes.');
//     return 0;
//   }

//   // Overtime starts after the regular end
//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window: 22:00 of schedule date to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   let totalNdMs = 0;

//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle overnight blocks
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Adjust blocks that start before 6AM (assume next day)
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Get the overlap between this shift block and the **overtime period**
//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Now find overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       console.log(`[ND OT Overlap] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   const ndOtMinutes = Math.round(totalNdMs / 60000);
//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },

// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = 8 * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     console.warn('Invalid timeIn or timeOut');
//     return 0;
//   }

//   // Calculate if there's any overtime
//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   // Logs
//   console.log(`Time In: ${timeInDate}`);
//   console.log(`Full Time Out: ${fullTimeOutDate}`);
//   console.log(`Max Regular End: ${maxRegularEndDate}`);

//   if (fullTimeOutDate <= maxRegularEndDate) {
//     console.log('[INFO] No overtime — so no ND OT minutes.');
//     return 0;
//   }

//   // Overtime starts after the regular end
//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window: 22:00 of schedule date to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   let totalNdMs = 0;

//   blocks.forEach(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // Handle overnight blocks
//     if (blockEnd <= blockStart) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // Adjust blocks that start before 6AM (assume next day)
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     // Get the overlap between this shift block and the **overtime period**
//     const actualStartMs = Math.max(otStartDate.getTime(), blockStart.getTime());
//     const actualEndMs = Math.min(otEndDate.getTime(), blockEnd.getTime());

//     if (actualEndMs <= actualStartMs) return;

//     // Now find overlap with ND window
//     const ndOverlapStart = Math.max(actualStartMs, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(actualEndMs, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       const overlapMs = ndOverlapEnd - ndOverlapStart;
//       totalNdMs += overlapMs;
//       console.log(`[ND OT Overlap] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//     }
//   });

//   // // *** Additional check for ND overtime outside schedule blocks ***
//   // const otBlockStart = otStartDate.getTime();
//   // const otBlockEnd = otEndDate.getTime();

//   // // Calculate ND overlap outside scheduled blocks (if any)
//   // // Compare entire OT period vs ND window
//   // const ndOverlapStart = Math.max(otBlockStart, ndStartDate.getTime());
//   // const ndOverlapEnd = Math.min(otBlockEnd, ndEndDate.getTime());

//   // // To avoid double counting, check if this ND overlap is already accounted for by blocks.
//   // // For simplicity, just add this overlap anyway if positive.
//   // if (ndOverlapEnd > ndOverlapStart) {
//   //   const overlapMs = ndOverlapEnd - ndOverlapStart;
//   //   totalNdMs += overlapMs;
//   //   console.log(`[ND OT Outside Schedule Block] ${new Date(ndOverlapStart).toTimeString()} - ${new Date(ndOverlapEnd).toTimeString()} = ${Math.round(overlapMs / 60000)} mins`);
//   // }

//   const ndOtMinutes = Math.round(totalNdMs / 60000);
//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },


// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeOut, scheduleDateTimeOut, lastScheduledEndTime) {
//   const ND_START = '22:00'; // 10 PM
//   const ND_END = '06:00';   // 6 AM next day

//   const isoDateOut = this.toISODate(scheduleDateTimeOut);

  
//   // Parse last scheduled end datetime first
//   let lastScheduledEndDate = this.parseDateTime(isoDateOut, lastScheduledEndTime);

//   // Parse actual time out datetime
//   let timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   // Adjust lastScheduledEndDate if it is before 6 AM (belongs to previous day)
//   if (this.parseTimeToMinutes(lastScheduledEndTime) < this.parseTimeToMinutes('06:00')) {
//     lastScheduledEndDate = this.parseDateTime(this.addDays(isoDateOut, -1), lastScheduledEndTime);
//   }

//   // Adjust timeOutDate if it is before or equal lastScheduledEndDate (means next day)
//   if (timeOutDate <= lastScheduledEndDate) {
//     timeOutDate = this.parseDateTime(this.addDays(isoDateOut, 1), scheduleTimeOut);
//   }

//   // No overtime if timeout is before or equal scheduled end
//   if (timeOutDate <= lastScheduledEndDate) {
//     return 0;
//   }

//   // Night diff window: from 22:00 of schedule date to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateOut, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateOut, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000); // next day 6 AM
//   }

//   // Calculate overlap between OT and ND window
//   const otStartMs = lastScheduledEndDate.getTime();
//   const otEndMs = timeOutDate.getTime();

//   console.log('otStartMs John', otStartMs)
//   console.log('otEndMs John', otEndMs)


//   const ndOverlapStart = Math.max(otStartMs, ndStartDate.getTime());
//   const ndOverlapEnd = Math.min(otEndMs, ndEndDate.getTime());

//   console.log('ndOverlapStart John', ndOverlapStart)
//   console.log('ndOverlapEnd John', ndOverlapEnd)


//   if (ndOverlapEnd <= ndOverlapStart) {
//     return 0;
//   }

//   const ndOtMs = ndOverlapEnd - ndOverlapStart;
//   const ndOtMinutes = Math.round(ndOtMs / 60000);

//   console.log('Night Diff Overtime Minutes:', ndOtMinutes);
//   return ndOtMinutes;
// },

// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes

//   const MAX_REGULAR_MINUTES = maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

//   if (!timeInDate || !fullTimeOutDate) {
//     return 0;
//   }

//   const maxRegularEndDate = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);

//   // No overtime if timeOut is within regular hours
//   if (fullTimeOutDate <= maxRegularEndDate) {
//     return 0;
//   }

//   const otStartDate = maxRegularEndDate;
//   const otEndDate = fullTimeOutDate;

//   // ND window from 22:00 of scheduleDate to 06:00 next day
//   const ndStartDate = this.parseDateTime(isoDateIn, ND_START);
//   let ndEndDate = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEndDate <= ndStartDate) {
//     ndEndDate = new Date(ndEndDate.getTime() + 24 * 60 * 60 * 1000);
//   }

//   // Convert blocks to Date ranges, accounting for overnight blocks and next day
//   const blockIntervals = blocks.map(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }
//     return { start: blockStart, end: blockEnd };
//   });

//   // Find the end time of the last scheduled block
//   let lastScheduledEnd = blockIntervals.reduce((max, b) => b.end > max ? b.end : max, new Date(0));

//   let totalNdMs = 0;

//   // Only count ND OT after last scheduled block
//   if (otEndDate > lastScheduledEnd) {
//     const afterLastSchedStart = Math.max(otStartDate.getTime(), lastScheduledEnd.getTime());
//     const afterLastSchedEnd = otEndDate.getTime();

//     const ndOverlapStart = Math.max(afterLastSchedStart, ndStartDate.getTime());
//     const ndOverlapEnd = Math.min(afterLastSchedEnd, ndEndDate.getTime());

//     if (ndOverlapEnd > ndOverlapStart) {
//       totalNdMs += ndOverlapEnd - ndOverlapStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000);
// },

//09182025
// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
//   if (!timeInDate || !fullTimeOutDate) return 0;

//   const regularEnd = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);
//   if (fullTimeOutDate <= regularEnd) return 0; // no overtime

//   const otStart = regularEnd;
//   const otEnd = fullTimeOutDate;

//   // Night Differential window: 22:00 of current day to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // move to next day
//   }

//   // Build block intervals from schedule
//   const blockIntervals = blocks.map(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // If block crosses midnight
//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // If start is before 06:00, shift to next day
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     return { start: blockStart, end: blockEnd };
//   });

//   // Find end of last scheduled block
//   const lastScheduledEnd = blockIntervals.reduce((max, b) => b.end > max ? b.end : max, new Date(0));

//   let totalNdMs = 0;

//   // Step 1: Count ND overtime inside blocks
//   blockIntervals.forEach(({ start, end }) => {
//     const overlapStart = Math.max(otStart.getTime(), start.getTime(), ndStart.getTime());
//     const overlapEnd = Math.min(otEnd.getTime(), end.getTime(), ndEnd.getTime());

//     if (overlapEnd > overlapStart) {
//       totalNdMs += overlapEnd - overlapStart;
//     }
//   });

//   // Step 2: Count ND overtime AFTER last scheduled block
//   if (otEnd > lastScheduledEnd) {
//     const extraStart = Math.max(lastScheduledEnd.getTime(), otStart.getTime(), ndStart.getTime());
//     const extraEnd = Math.min(otEnd.getTime(), ndEnd.getTime());

//     if (extraEnd > extraStart) {
//       totalNdMs += extraEnd - extraStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000); // convert ms to minutes
// },

// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
//   if (!timeInDate || !fullTimeOutDate) return 0;

//   const regularEnd = new Date(timeInDate.getTime() + MAX_REGULAR_MINUTES * 60000);
//   if (fullTimeOutDate <= regularEnd) return 0; // no overtime

//   const otStart = regularEnd;
//   const otEnd = fullTimeOutDate;

//   // ND window: 22:00 on scheduleDate to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); // shift to next day
//   }

//   // Build block intervals (adjusting for overnight times)
//   const blockIntervals = blocks.map(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     return { start: blockStart, end: blockEnd };
//   });

//   // Find end of last scheduled block
//   const lastScheduledEnd = blockIntervals.reduce((max, b) => b.end > max ? b.end : max, new Date(0));

//   let totalNdMs = 0;

//   // Step 1: Count ND OT inside scheduled blocks
//   blockIntervals.forEach(({ start, end }) => {
//     const overlapStart = Math.max(start.getTime(), otStart.getTime(), ndStart.getTime());
//     const overlapEnd = Math.min(end.getTime(), otEnd.getTime(), ndEnd.getTime());

//     if (overlapEnd > overlapStart) {
//       totalNdMs += overlapEnd - overlapStart;
//     }
//   });

//   // Step 2: Count ND OT AFTER last scheduled block (e.g., 04:00 → 06:00)
//   if (otEnd > lastScheduledEnd) {
//     const afterBlockStart = Math.max(lastScheduledEnd.getTime(), otStart.getTime(), ndStart.getTime());
//     const afterBlockEnd = Math.min(otEnd.getTime(), ndEnd.getTime());

//     if (afterBlockEnd > afterBlockStart) {
//       totalNdMs += afterBlockEnd - afterBlockStart;
//     }
//   }

//   return Math.round(totalNdMs / 60000); // Convert to minutes
// },

// computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
//   const ND_START = '22:00';
//   const ND_END = '06:00';
//   const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60;

//   const isoDateIn = this.toISODate(scheduleDate);
//   const isoDateOut = this.toISODate(scheduleDateTimeOut);
//   const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

//   const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
//   const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
//   if (!timeInDate || !fullTimeOutDate) return 0;

//   // Build block intervals from schedule (same as before)
//   const blockIntervals = blocks.map(block => {
//     let [startStr, endStr] = block.split('-').map(s => s.trim());
//     let blockStart = this.parseDateTime(isoDateIn, startStr);
//     let blockEnd = this.parseDateTime(isoDateIn, endStr);

//     // If block crosses midnight
//     if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
//       blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//     }

//     // If start is before 06:00, shift to next day
//     if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
//       blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
//       blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
//       if (blockEnd <= blockStart) {
//         blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
//       }
//     }

//     return { start: blockStart, end: blockEnd };
//   });

//   // --- New helper function to calculate regular end time accounting for schedule gaps ---
//   function calculateRegularEnd(blockIntervals, timeInDate, regularMinutes) {
//     let accumulated = 0;
//     for (const { start, end } of blockIntervals) {
//       if (end <= timeInDate) continue;

//       const countStart = start < timeInDate ? timeInDate : start;
//       const blockDuration = (end - countStart) / 60000; // in minutes

//       if (accumulated + blockDuration >= regularMinutes) {
//         const extraMinutes = regularMinutes - accumulated;
//         return new Date(countStart.getTime() + extraMinutes * 60000);
//       } else {
//         accumulated += blockDuration;
//       }
//     }

//     // If regular minutes exceed schedule blocks, return last block end
//     return blockIntervals[blockIntervals.length - 1].end;
//   }

//   const regularEnd = calculateRegularEnd(blockIntervals, timeInDate, MAX_REGULAR_MINUTES);

//   if (fullTimeOutDate <= regularEnd) return 0; // no overtime

//   const otStart = regularEnd;
//   const otEnd = fullTimeOutDate;

//   // Night Differential window: 22:00 current day to 06:00 next day
//   const ndStart = this.parseDateTime(isoDateIn, ND_START);
//   let ndEnd = this.parseDateTime(isoDateIn, ND_END);
//   if (ndEnd <= ndStart) {
//     ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000);
//   }

//   // Find end of last scheduled block
//   const lastScheduledEnd = blockIntervals.reduce((max, b) => (b.end > max ? b.end : max), new Date(0));

//   let totalNdMs = 0;

//   // Step 1: Count ND overtime inside blocks
//   blockIntervals.forEach(({ start, end }) => {
//     const overlapStart = Math.max(otStart.getTime(), start.getTime(), ndStart.getTime());
//     const overlapEnd = Math.min(otEnd.getTime(), end.getTime(), ndEnd.getTime());

//     if (overlapEnd > overlapStart) {
//       totalNdMs += overlapEnd - overlapStart;
//     }
//   });

//   // Step 2: Count ND overtime AFTER last scheduled block
//   if (otEnd > lastScheduledEnd) {
//     const extraStart = Math.max(lastScheduledEnd.getTime(), otStart.getTime(), ndStart.getTime());
//     const extraEnd = Math.min(otEnd.getTime(), ndEnd.getTime());

//     if (extraEnd > extraStart) {
//       totalNdMs += extraEnd - extraStart;
//     }
//   }
//   console.log('JENON',totalNdMs / 60000)
//   return Math.round(totalNdMs / 60000); // convert ms to minutes
// },

computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
  const ND_START = '22:00';
  const ND_END = '06:00';
  const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60;

  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);
  const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);

  const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
  const fullTimeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);
  if (!timeInDate || !fullTimeOutDate) return 0;

  // Assign correct day to blocks based on start times (like undertime function)
  let blockDay = new Date(isoDateIn);
  let prevStartMinutes = this.parseTimeToMinutes('00:00');

  const blockIntervals = blocks.map((block, index) => {
    const [startStr, endStr] = block.split('-').map(s => s.trim());
    const currentStartMinutes = this.parseTimeToMinutes(startStr);

    if (index > 0 && currentStartMinutes <= prevStartMinutes) {
      blockDay = this.addDays(blockDay, 1);
    }
    prevStartMinutes = currentStartMinutes;

    let blockStart = this.parseDateTime(blockDay, startStr);
    let blockEnd = this.parseDateTime(blockDay, endStr);

    // Cross-midnight blocks
    if (blockEnd <= blockStart) {
      blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
    }
    return { start: blockStart, end: blockEnd };
  });

  // Calculate regular end time considering gaps
  function calculateRegularEnd(blockIntervals, timeInDate, regularMinutes) {
    let accumulated = 0;
    for (const { start, end } of blockIntervals) {
      if (end <= timeInDate) continue;
      const countStart = start < timeInDate ? timeInDate : start;
      const blockDuration = (end - countStart) / 60000;
      if (accumulated + blockDuration >= regularMinutes) {
        const extraMinutes = regularMinutes - accumulated;
        return new Date(countStart.getTime() + extraMinutes * 60000);
      } else {
        accumulated += blockDuration;
      }
    }
    return blockIntervals[blockIntervals.length - 1].end;
  }

  const regularEnd = calculateRegularEnd(blockIntervals, timeInDate, MAX_REGULAR_MINUTES);

  if (fullTimeOutDate <= regularEnd) return 0; // no overtime

  const otStart = regularEnd;
  const otEnd = fullTimeOutDate;

  // Night Differential window from 22:00 isoDateIn to 06:00 isoDateIn +1 day
  const ndStart = this.parseDateTime(isoDateIn, ND_START);
  let ndEnd = this.parseDateTime(this.addDays(isoDateIn, 1), ND_END);

  let totalNdMs = 0;

  // Count ND overtime inside blocks
  blockIntervals.forEach(({ start, end }) => {
    const overlapStart = Math.max(otStart.getTime(), start.getTime(), ndStart.getTime());
    const overlapEnd = Math.min(otEnd.getTime(), end.getTime(), ndEnd.getTime());
    if (overlapEnd > overlapStart) {
      totalNdMs += overlapEnd - overlapStart;
    }
  });

  // Count ND overtime after last scheduled block
  const lastScheduledEnd = blockIntervals.reduce((max, b) => (b.end > max ? b.end : max), new Date(0));
  if (otEnd > lastScheduledEnd) {
    const extraStart = Math.max(lastScheduledEnd.getTime(), otStart.getTime(), ndStart.getTime());
    const extraEnd = Math.min(otEnd.getTime(), ndEnd.getTime());
    if (extraEnd > extraStart) {
      totalNdMs += extraEnd - extraStart;
    }
  }

  return Math.round(totalNdMs / 60000);
},

getFirstTimeInSchedule(scheduleCode) {
  const blocks = scheduleCode.split('|').map(b => b.trim());
  const firstBlock = blocks[0];
  const timeParts = firstBlock.split('-');

  const firstTimeStr = timeParts.length > 0 ? timeParts[0].trim() : null;
  
  return firstTimeStr;
},

toISODate(dateInput) {
    if (!dateInput) return '';

    // Luxon DateTime object
    if (
      typeof dateInput === 'object' &&
      'year' in dateInput &&
      'month' in dateInput &&
      'day' in dateInput
    ) {
      const y = String(dateInput.year);
      const m = String(dateInput.month).padStart(2, '0');
      const d = String(dateInput.day).padStart(2, '0');
      return `${y}-${m}-${d}`;
    }

    // ISO string
    if (typeof dateInput === 'string' && dateInput.includes('-')) {
      const parsed = new Date(dateInput);
      if (!isNaN(parsed.getTime())) {
        return parsed.toISOString().slice(0, 10);
      }
    }

    // MM/DD/YYYY or MM/DD/YYYY HH:mm
    if (typeof dateInput === 'string') {
      const dateOnly = dateInput.split(' ')[0];
      const parts = dateOnly.split('/');
      if (parts.length === 3) {
        const [month, day, year] = parts;
        return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
      }
    }

    console.warn('Unsupported date format:', dateInput);
    return '';
  },

  addDays(dateStr, days) {
    const date = new Date(dateStr);
    date.setDate(date.getDate() + days);
    return date.toISOString().slice(0, 10); // Returns YYYY-MM-DD
  },


  parseTimeToMinutes(timeStr) {
    if (!timeStr || typeof timeStr !== 'string' || !timeStr.includes(':')) {
      console.warn('Invalid timeStr passed to parseTimeToMinutes:', timeStr);
      return NaN;
    }
    const [h, m] = timeStr.split(':').map(Number);
    if (isNaN(h) || isNaN(m)) {
      console.warn('Non-numeric hours or minutes in timeStr:', timeStr);
      return NaN;
    }
    return h * 60 + m;
  },


  parseDateTime(dateStr, timeStr) {
    if (typeof timeStr !== 'string') return null;
    const [hh, mm] = timeStr.split(':').map(Number);
    if (isNaN(hh) || isNaN(mm)) return null;
    const date = new Date(dateStr);
    date.setHours(hh, mm, 0, 0);
    return date;
  },

// //
// computeLateMinute(scheduleCode, scheduleTimeIn, scheduleDate) {
//   const firstTimeStr = this.getFirstTimeInSchedule(scheduleCode);
  
//   if (!firstTimeStr || !scheduleTimeIn || !scheduleDate) return 0;

//   const isoDate = this.toISODate(scheduleDate);
//   const expectedTimeIn = this.parseDateTime(isoDate, firstTimeStr);
//   const actualTimeIn = this.parseDateTime(isoDate, scheduleTimeIn);

//   if (!expectedTimeIn || !actualTimeIn) return 0;

//   const diffMs = actualTimeIn.getTime() - expectedTimeIn.getTime();
//   return diffMs > 0 ? Math.round(diffMs / 60000) : 0;
// },
computeLateMinute(scheduleCode, scheduleTimeIn, scheduleDate) {
  const firstTimeStr = this.getFirstTimeInSchedule(scheduleCode);
  if (!firstTimeStr || !scheduleTimeIn || !scheduleDate) return 0;

  const isoDate = this.toISODate(scheduleDate); // '2025-08-16'
  const expectedTimeIn = this.parseDateTime(isoDate, firstTimeStr);
  const actualTimeIn = this.parseDateTime(isoDate, scheduleTimeIn);

  if (!expectedTimeIn || !actualTimeIn) return 0;

  const diffMs = actualTimeIn.getTime() - expectedTimeIn.getTime();
  return diffMs > 0 ? Math.round(diffMs / 60000) : 0;
},


computeScheduleMetrics(item) {

  const lateGracePeriod = this.timekeeping.lateGracePeriodCount; 
  const lateCountMinutes = this.timekeeping.lateCount; 
  const ndCountMinutes = this.timekeeping.ndCount; 
  
  let earliestScheduleTimeIn = 0;
  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes
  let lateMinute = 0 ;

  let {
    scheduleCode,
    scheduleTimeIn,
    scheduleTimeOut,
    scheduleDate,
    scheduleDateTimeOut,
    unpaidBreak = 0,
    dayTypeCode, 
    noScheduleId,
  } = item;

    if (scheduleTimeIn === null || scheduleTimeOut === null || scheduleCode === null || scheduleDateTimeOut === null || scheduleCode === '' )  {

      return {
        lateMinute: 0, undertimeMinute: 0, regularHour: 0, nd: 0, overtimeHour: 0, ndot: 0,
        lh: 0, lhot: 0, lhnd: 0, lhndot: 0,
        sh: 0, shot: 0, shnd: 0, shndot: 0,
        lhsh: 0, lhshot: 0, lhshnd: 0, lhshndot: 0,
        drh: 0, drhot: 0, drhnd: 0, drhndot: 0,
        rd: 0, rdot: 0, rdnd: 0, rdndot: 0,

       regularHourP: 0, ndP: 0, overtimeHourP: 0, ndotp: 0,
        lhp: 0, lhotp: 0, lhndp: 0, lhndotp: 0,
        shp: 0, shotp: 0, shndp: 0, shndotp: 0,
        lhshp: 0, lhshotp: 0, lhshndp: 0, lhshndotp: 0,
        drhp: 0, drhotp: 0, drhndp: 0, drhndotp: 0,
        rdp: 0, rdotp: 0, rdndp: 0, rdndotp: 0


      };
    }

    let hours = scheduleTimeIn.hours;
   
    let minutes = scheduleTimeIn.minutes;

    let timeString = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
    scheduleTimeIn = timeString
    let outhours = scheduleTimeOut.hours;
    let outminutes = scheduleTimeOut.minutes;


    let outTimeString = `${String(outhours).padStart(2, '0')}:${String(outminutes).padStart(2, '0')}`;
    scheduleTimeOut = outTimeString



  if (!scheduleDateTimeOut) scheduleDateTimeOut = scheduleDate;

  let undertimeMinute = this.computeUndertime(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut);
  
  undertimeMinute = undertimeMinute - lateMinute;
  
  // LATE

  lateMinute = this.computeLateMinute(scheduleCode, scheduleTimeIn, scheduleDate);

  if (lateMinute <= lateGracePeriod) {
     lateMinute = 0;
  }


  if (lateCountMinutes > 0) {
     lateMinute = Math.floor(lateMinute / lateCountMinutes) * lateCountMinutes;
  
  }

  
  let overtimeHour = this.computeOvertime(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut);
  let regularHour = this.computeRegularHour(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut, unpaidBreak);

  let nd =  this.computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut);
  // let ndot = this.computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeOut, scheduleDateTimeOut, scheduleDateTimeOut);
  


let lastScheduledEndTime = null;
if (scheduleCode) {
  const blocks = scheduleCode.split('|').map(b => b.trim()).filter(Boolean);
  if (blocks.length) {
    const lastBlock = blocks[blocks.length - 1];
    lastScheduledEndTime = lastBlock.split('-')[1].trim();
  }
}

const isoDateOut = this.toISODate(scheduleDateTimeOut);


let ndot = this.computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut);
// let ndot = this.computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeOut, scheduleDateTimeOut, lastScheduledEndTime);

const ndCount = this.timekeeping.ndCount; // Late counting interval in minutes
    
  if (nd>=ndCount) {
     nd = Math.floor(nd / ndCount) * ndCount;
    } else {
      nd = 0;
    }

if (ndot>=ndCount) {
     ndot = Math.floor(ndot / ndCount) * ndCount;
    } else {
      ndot = 0;
    }



// let ndot = this.computeNightDiffOvertimeMinutes(scheduleCode, scheduleTimeOut, scheduleDateTimeOut, lastScheduledEndTime);

  let lh = 0;
  let lhot = 0;
  let lhnd = 0;
  let lhndot = 0;

  let sh = 0;
  let shot = 0;
  let shnd = 0;
  let shndot = 0;

  let lhsh = 0;
  let lhshot = 0;
  let lhshnd = 0;
  let lhshndot = 0;


  let drh = 0;
  let drhot = 0;
  let drhnd = 0;
  let drhndot = 0;

  let rd = 0;
  let rdot = 0;
  let rdnd = 0;
  let rdndot = 0;

  let lhrd = 0;
  let lhrdot = 0;
  let lhrdnd = 0;
  let lhrdndot = 0;

  let shrd = 0;
  let shrdot = 0;
  let shrdnd = 0;
  let shrdndot = 0;

  let lhshrd = 0;
  let lhshrdot = 0;
  let lhshrdnd = 0;
  let lhshrdndot = 0;

  let drhrd = 0;
  let drhrdot = 0;
  let drhrdnd = 0;
  let drhrdndot = 0;

  let hu100 = 0;


// let regularHour  = 0;
// let overtimeHour = 0;
// let nd = 0;
// let ndot = 0;

if (this.timekeeping.constructionFlag) {
  //  overtimeHour = 0//regularHour - this.timekeeping.maximumRegularHourCount
  // regularHour = this.timekeeping.maximumRegularHourCount
}


if (dayTypeCode=='LH' && noScheduleId!==1) {
  lh = regularHour;
  lhot = overtimeHour;
 
  lhnd = nd;
  lhndot = ndot;
  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}

if (noScheduleId==1 && dayTypeCode=='LH') {
  lhrd = regularHour;
  lhrdot = overtimeHour;

  lhrdnd = nd;
  lhrdndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}


if (dayTypeCode=='SH' && noScheduleId!==1) {
  sh = regularHour;
  shot = overtimeHour;
  shnd = nd;
  shndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}

if (noScheduleId==1 && dayTypeCode=='SH') {
  shrd = regularHour;
  shrdot = overtimeHour;
  shrdnd = nd;
  shrdndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}




if (dayTypeCode=='LHSH' && noScheduleId!==1) {
  lhsh = regularHour;
  lhshot = overtimeHour;
  lhshnd = nd;
  lhshndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}

if (noScheduleId==1 && dayTypeCode=='LHSH') {
  lhshrd = regularHour;
  lhshrdot = overtimeHour;
  lhshrdnd = nd;
  lhshrdndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}



if (dayTypeCode=='DRH' && noScheduleId!==1) {
  drh = regularHour;
  drhot = overtimeHour;
  drhnd = nd;
  drhndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}

if (noScheduleId==1 && dayTypeCode=='DRH') {
  drhrd = regularHour;
  drhrdot = overtimeHour;
  drhrdnd = nd;
  drhrdndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}


if (dayTypeCode=='HU-100' && noScheduleId!==1) {
  // hu100 = regularHour;
  // drhot = overtimeHour;
  // drhnd = nd;
  // drhndot = ndot;

    // regularHour = 0; 
  // nd = 0;  
  // overtimeHour = 0;  
  // ndot = 0; 
  hu100 = regularHour;
  nd = overtimeHour;
  overtimeHour = nd;
  ndot = ndot;

  regularHour = 0; 
  // nd = 0;  
  // overtimeHour = 0;  
  // ndot = 0; 

}

if (noScheduleId==1 && dayTypeCode=='HU-100') {
  hu100 = regularHour;
  nd = overtimeHour;
  overtimeHour = nd;
  ndot = ndot;

   regularHour = 0; 
  // nd = 0;  
  // overtimeHour = 0;  
  // ndot = 0; 
}


if (noScheduleId==1 && dayTypeCode==null) {
  rd = regularHour;
  rdot = overtimeHour;
  rdnd = nd;
  rdndot = ndot;

  regularHour = 0; 
  nd = 0;  
  overtimeHour = 0;  
  ndot = 0; 
}


return {lateMinute, undertimeMinute, regularHour, nd, overtimeHour, ndot, lh,lhot,lhnd,lhndot, 
sh,shot,shnd,shndot,
lhsh,lhshot,lhshnd,lhshndot,
drh,drhot,drhnd,drhndot,
rd, rdot,rdnd, rdndot,
lhrd,lhrdot,lhrdnd, lhrdndot,
shrd,shrdot,shrdnd,shrdndot,
lhshrd,lhshrdot,lhshrdnd,lhshrdndot,
drhrd,drhrdot,drhrdnd,drhrdndot,
hu100
}


  // // Return result
  // return {
  //   lateMinute,
  //   undertimeMinute,
  //   regularHour: workedWithinSchedule / 60,

  //   // You can implement these later
  //   nd: 0, overtimeHour: 0, ndot: 0,
  //   lh: 0, lhot: 0, lhnd: 0, lhndot: 0,
  //   sh: 0, shot: 0, shnd: 0, shndot: 0,
  //   lhsh: 0, lhshot: 0, lhshnd: 0, lhshndot: 0,
  //   drh: 0, drhot: 0, drhnd: 0, drhndot: 0,
  //   rd: 0, rdot: 0, rdnd: 0, rdndot: 0,

  //   regularHourP: 0, ndP: 0, overtimeHourP: 0, ndotP: 0,
  //   lhp: 0, lhotp: 0, lhndp: 0, lhndotp: 0,
  //   shp: 0, shotp: 0, shndp: 0, shndotp: 0,
  //   lhshp: 0, lhshotp: 0, lhshndp: 0, lhshndotp: 0,
  //   drhp: 0, drhotp: 0, drhndp: 0, drhndotp: 0,
  //   rdp: 0, rdotp: 0, rdndp: 0, rdndotp: 0
  // };


},

// Helper: return all-zero object for invalid/empty schedules
generateEmptyMetrics() {
  return {
    lateMinute: 0, undertimeMinute: 0, regularHour: 0, nd: 0, overtimeHour: 0, ndot: 0,
    lh: 0, lhot: 0, lhnd: 0, lhndot: 0,
    sh: 0, shot: 0, shnd: 0, shndot: 0,
    lhsh: 0, lhshot: 0, lhshnd: 0, lhshndot: 0,
    drh: 0, drhot: 0, drhnd: 0, drhndot: 0,
    rd: 0, rdot: 0, rdnd: 0, rdndot: 0,
    regularHourP: 0, ndp: 0, overtimeHourP: 0, ndotp: 0,
    lhp: 0, lhotp: 0, lhndp: 0, lhndotp: 0,
    shp: 0, shotp: 0, shndp: 0, shndotp: 0,
    lhshp: 0, lhshotp: 0, lhshndp: 0, lhshndotp: 0,
    drhp: 0, drhotp: 0, drhndp: 0, drhndotp: 0,
    rdp: 0, rdotp: 0, rdndp: 0, rdndotp: 0
  };
},

    
   onUploadManual() {
      this.$refs.excelInput.click();
      
    },

handleFileUpload(event) {
  const file = event.target.files[0];

  if (!file) return;

  const reader = new FileReader();

  reader.onload = (e) => {
    const data = new Uint8Array(e.target.result);
    const workbook = XLSX.read(data, { type: 'array' });
    const firstSheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[firstSheetName];


    const rawData = XLSX.utils.sheet_to_json(worksheet, { defval: '', raw: false });

    const isValidTime = str => /^([01]?\d|2[0-3]):[0-5]\d$/.test(str);

rawData.forEach(row => {
  const dateRaw = row['DATE'];
  const MEMBERID = row['MEMBERID'];
  const TIMEIN = row['TIMEIN'];
  const TIMEOUT = row['TIMEOUT'];

  const isValidTime = str => /^([01]?\d|2[0-3]):[0-5]\d$/.test(str);

  const isMissingRequiredField =
    !MEMBERID || MEMBERID === 0 ||
    !dateRaw ||
    !isValidTime(TIMEIN) ||
    !isValidTime(TIMEOUT);

  let scheduleDate = '';
  let timeIn = null;
  let timeOut = null;

  if (!isMissingRequiredField) {
    const dateObj = new Date(dateRaw);
    scheduleDate = `${dateObj.getFullYear()}-${String(dateObj.getMonth() + 1).padStart(2, '0')}-${String(dateObj.getDate()).padStart(2, '0')}`;

    const timeInParts = TIMEIN.split(':').map(Number);
    const timeOutParts = TIMEOUT.split(':').map(Number);

    timeIn = new Date(dateObj);
    timeIn.setHours(timeInParts[0], timeInParts[1], 0, 0);

    timeOut = new Date(dateObj);
    timeOut.setHours(timeOutParts[0], timeOutParts[1], 0, 0);

    if (timeOut <= timeIn) {
      timeOut.setDate(timeOut.getDate() + 1); // Overnight shift
    }
  }

  const matchedMember = this.memberList.find(m => String(m.memberId) === String(MEMBERID));
  const existingIndex = this.schedules.findIndex(s =>
    String(s.memberId) === String(MEMBERID) &&
    s.scheduleDate === scheduleDate
  );

  const invalidFlag = isMissingRequiredField || existingIndex === -1 || !matchedMember;

  const scheduleItem = {
    timekeepingSheetId: this.timekeeping.timekeepingSheetId,
    memberId: MEMBERID || '',
    memberName: matchedMember ? matchedMember.memberName : '',
    scheduleDate,
    scheduleTimeIn: timeIn ? timeIn.toTimeString().split(' ')[0] : '',
    scheduleTimeOut: timeOut ? timeOut.toTimeString().split(' ')[0] : '',
    invalidFlag,

    // Add all additional fields from the sheet
    payableToMemberFlag: row['PAYABLETOMEMBER'],
    billableToClientFlag: row['BILLABLETOCLIENT'],
    remarks: row['REMARKS'],

    regularHourP: row['REGHRS'],
    overtimeHourP: row['OT'],
    ndp: row['ND'],
    ndotp: row['NDOT'],
    lateMinuteP: row['LATE'],
    undertimeMinuteP: row['UNDERTIME'],
    rdp: row['RD'],
    rdotp: row['RDOT'],
    rdndp: row['RDND'],
    rdndotp: row['RDNDOT'],
    shp: row['SH'],
    shotp: row['SHOT'],
    shndp: row['SHND'],
    shndotp: row['SHNDOT'],
    shrdp: row['SHRD'],
    shrdotp: row['SHRDOT'],
    shrdndp: row['SHRDND'],
    shrdndotp: row['SHRDNDOT'],
    lhrdp: row['LHRD'],
    lhrdotp: row['LHRDOT'],
    lhrdndp: row['LHRDND'],
    lhrdndotp: row['LHRDNDOT'],
    lhp: row['LH'],
    lhotp: row['LHOT'],
    lhndp: row['LHND'],
    lhndotp: row['LHNDOT'],
    lhshp: row['LHSH'],
    lhshotp: row['LHSHOT'],
    lhshndp: row['LHSHND'],
    lhshndotp: row['LHSHNDOT'],
    lhshrdp: row['LHSHRD'],
    lhshrdotp: row['LHSHRDOT'],
    lhshrdndp: row['LHSHRDND'],
    lhshrdndotp: row['LHSHRDNDOT'],
    hu100p: row['HU-100'],
    drhp: row['DRH'],
    drhotp: row['DRHOT'],
    drhndp: row['DRHND'],
    drhndotp: row['DRHNDOT'],
    drhrdp: row['DRHRD'],
    drhrdotp: row['DRHRDOT'],
    drhrdndp: row['DRHRDND'],
    drhrdndotp: row['DRHRDNDOT']
  };

  if (existingIndex !== -1) {
    const existingRecord = this.schedules[existingIndex];
    const scheduleCode = existingRecord.scheduleCode;
    const dayTypeName = existingRecord.dayTypeName;

    const mergedItem = {
      ...scheduleItem,
      scheduleCode,
      dayTypeName
    };

    this.$set(this.schedules, existingIndex, mergedItem);
  } else {
    // Reject the change – do NOT push to this.schedules
  }
});

    // Optional: set excelData if needed
    // this.excelData = this.schedules;
  };

  this.offSetFlag = true;
  this.timekeeping.applyOffSetFlag =  true;
  this.timekeeping.manualUploadFlag =  true;

  reader.readAsArrayBuffer(file);
},


  getNoDataColor(scheduleCode, scheduleTimeIn, scheduleTimeOut) {
    return scheduleTimeIn === null || scheduleTimeOut === null || scheduleCode === null || scheduleCode === '' ? "danger-light": "xxx" ;
  },


  onCalculate() {
    this.offSetFlag = false;
      this.timekeeping.applyOffSetFlag =  false;

      this.schedules = this.schedules.map(item => ({
          ...item,
          ...this.computeScheduleMetrics(this.core.convertTimes(item))
        }));
},


onApplyRestDayOffSetting(){
  
  this.schedules = this.schedules.map(schedule => {
  // Clone schedule to avoid mutating the original object
  const newSchedule = { ...schedule };

  if (this.timekeeping.restDayOffSetFlag && this.timekeeping.restDayConsideration !== 0) {
    const hasNoScheduleId1 = this.schedules.some(item => item.noScheduleId === 1);

    let count = 0;
    if (hasNoScheduleId1) {
      count = this.schedules.filter(item =>
        item.noScheduleId !== 1 && !!item.timekeepingScheduleId
      ).length;
    }

    if (count < this.timekeeping.restDayConsideration && newSchedule.noScheduleId === 1) {
      // Reclassify rest day as regular
      newSchedule.regularHourP  = newSchedule.rdp     || 0;
      newSchedule.overtimeHourP = newSchedule.rdotp   || 0;
      newSchedule.ndp           = newSchedule.rdndp   || 0;
      newSchedule.ndotp         = newSchedule.rdndotp || 0;

      // Clear rest day values
      newSchedule.rdp      = 0;
      newSchedule.rdotp    = 0;
      newSchedule.rdndp    = 0;
      newSchedule.rdndotp  = 0;
    }
  }

  return newSchedule;
});

},

onApplyOffset() {

  const lateGracePeriod = this.timekeeping.lateGracePeriodCount;  // Grace period in minutes
  const lateCountMinutes = this.timekeeping.lateCount; // Late counting interval in minutes
  const minimumRequiredOvertimeCount = this.timekeeping.minimumRequiredOvertimeCount; // Late counting interval in minutes
  const overtimeDurationCount = this.timekeeping.overtimeDurationCount; // Late counting interval in minutes
  const overtimeCountId = this.timekeeping.overtimeCountId; // Late counting interval in minutes
  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; // Late counting interval in minutes
  const ndCount = this.timekeeping.ndCount; // Late counting interval in minutes
  const paidUpHolidayId = this.timekeeping.paidUpHolidayId; // Late counting interval in minutes
  const nightDifferentialCountId = this.timekeeping.nightDifferentialCountId; // Late counting interval in minutes
  const restDayOffSetFlag = this.timekeeping.restDayOffSetFlag; // Late counting interval in minutes
  const restDayConsideration = this.timekeeping.restDayConsideration; // Late counting interval in minutes
  const lateCountId = this.timekeeping.lateCountId; // Late counting interval in minutes
  const holidayCountId = this.timekeeping.holidayCountId; // Late counting interval in minutes
  const undertimeCountId = this.timekeeping.undertimeCountId; // Late counting interval in minutes
  const underTimeDurationPerCount=this.timekeeping.underTimeDurationPerCount;
  this.offSetFlag = true;
  this.timekeeping.applyOffSetFlag =  true;
  this.schedules = this.schedules.map(schedule => {
    // Regular
    
    let regularHourP = schedule.regularHour;
    let overtimeHourP = schedule.overtimeHour;
    let ndP = (schedule.nd /60).toFixed(1);
    let ndotP = (schedule.ndot /60).toFixed(1);
    let lateMinuteP = 0;
    console.log('lateCountId',lateCountId) 
    if (lateCountId==1) {
      lateMinuteP = schedule.lateMinute;
    }
    
    let undertimeMinuteP = schedule.undertimeMinute;

    if (undertimeMinuteP>=underTimeDurationPerCount) {
     undertimeMinuteP = Math.floor(undertimeMinuteP / underTimeDurationPerCount) * underTimeDurationPerCount;
    } else {
      undertimeMinuteP = 0;
    }
    // // late matrix    
  // Update lateMinuteP based on the lateMatrix
  const matchedLate = this.lateMatrix.find(matrix => 
    lateMinuteP >= matrix.startMinute && lateMinuteP <= matrix.endMinute
  );

  if (matchedLate) {
    lateMinuteP = matchedLate.lateCount;
  };



  if (regularHourP > maximumRegularHourCount) {
    regularHourP = (maximumRegularHourCount) 
    overtimeHourP =  ((schedule.regularHour - (lateMinuteP/60)) - maximumRegularHourCount)     
    
    if (schedule.nd > 0) {
    ndP =  schedule.nd - (overtimeHourP + (lateMinuteP/60))
    if (schedule.ndot>0) {
    ndotP = schedule.nd - ndP
    } else {
    ndotP = 0  
    }

    if (ndP < 0) {
      ndotP = (ndotP + ndP) 
      ndP = 0;
    }

    if (ndotP < 0) {
      ndotP = 0
      
    }
    
  } 
};



if (this.timekeeping.constructionFlag && regularHourP>0) {

  let  overtimeHour = schedule.regularHour - this.timekeeping.maximumRegularHourCount 
  
  let regularHour = this.timekeeping.maximumRegularHourCount
  
  regularHourP = regularHour


    overtimeHourP =  Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.overtimeHour)   //round 0'

    if (overtimeHourP<0) {
       regularHourP = regularHourP + overtimeHourP;

      if (regularHourP < maximumRegularHourCount && ((regularHourP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        regularHourP = regularHourP + schedule.unpaidBreak;
      }

      overtimeHourP = 0;

    }


    undertimeMinuteP = 0

    ndP = (schedule.nd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    ndotP = (schedule.ndot /60).toFixed(1)
    // if ((schedule.nd /60).toFixed(1) > 0) {
    // ndP = (schedule.nd /60).toFixed(1); - (overtimeHourP + (lateMinuteP/60))
    // ndotP = (schedule.nd /60).toFixed(1); - ndP

    // if (ndP < 0) {
    //   ndotP = (ndotP + ndP) 
    //   ndP = 0;
    // }
  // } 
};

  // RestDay
    let rdP = schedule.rd;
    let rdotP = schedule.rdot;
    let rdndP = schedule.rdnd;
    let rdndotP = schedule.rdndot;

  if (rdP > maximumRegularHourCount) {
    
    rdP = maximumRegularHourCount
    rdotP =  (schedule.rd - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.rdnd>0) {
    rdndP = schedule.rdnd - (rdotP + (lateMinuteP/60))
    rdndotP = schedule.rdnd - rdndP

    if (rdndP < 0) {
      rdndotP = rdndotP + rdndP;
      rdndP = 0;
    }

    }
  };


  if (this.timekeeping.constructionFlag && rdP > 0 ) {


    let  overtimeHour = schedule.rd - this.timekeeping.maximumRegularHourCount
    let regularHour = this.timekeeping.maximumRegularHourCount

    rdP = regularHour
    rdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.rdot)   //round 0'

    if (rdotP<0) {
      rdP = rdP + rdotP;

      if (rdP < maximumRegularHourCount && ((rdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        rdP = rdP + schedule.unpaidBreak;
      }


      rdotP = 0;
    }



    undertimeMinuteP = 0

  //   if ((schedule.rdndP /60).toFixed(1) > 0) {
  //   rdndP = (schedule.rdnd /60).toFixed(1) - (rdotP + (lateMinuteP/60))
  //   rdndotP = (schedule.rdnd /60).toFixed(1) - rdndP

  //   if (rdndP < 0) {
  //     rdndotP = (rdndotP + rdndP) 
  //     rdndP = 0;
  //   }
  // }
  
      rdndP = (schedule.rdnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    rdndotP = (schedule.rdndot /60).toFixed(1)


};


// Special Holiday
    let shP = schedule.sh;
    let shotP = schedule.shot;
    let shndP = schedule.shnd;
    let shndotP = schedule.shndot;

  if (shP > maximumRegularHourCount) {
    
    shP = maximumRegularHourCount
    shotP =  (schedule.sh - (lateMinuteP/60)) - maximumRegularHourCount

    if (schedule.shnd>0) {
    shndP = schedule.shnd - (shotP + (lateMinuteP/60))
    shndotP = schedule.shnd - shndP


    if (shndP < 0) {
      shndotP = shndotP + shndP;
  
  
      shndP = 0;
    }


  }
  };

  
  if (this.timekeeping.constructionFlag && shP > 0 ) {

  let  overtimeHour = schedule.sh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount




    shP = regularHour
    shotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.shot)   //round 0'

    if (shotP<0) {
      shP = shP + shotP;
  
      if (shP < maximumRegularHourCount && ((shP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        shP = shP + schedule.unpaidBreak;
      }

  
      shotP = 0;
    }


    undertimeMinuteP = 0

      shndP = (schedule.shnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    shndotP = (schedule.shndot /60).toFixed(1)

    
  //   if ((schedule.shnd /60).toFixed(1)>0) {
  //   shndP = (schedule.shnd /60).toFixed(1) - (shotP + (lateMinuteP/60))
  //   shndotP = (schedule.shnd /60).toFixed(1) - shndP


  //   if (shndP < 0) {
  //     shndotP = shndotP + shndP;
  //     shndP = 0;
  //   }

  // } 
};




  
 // Special Double Holiday
    let shrdP = schedule.shrd;
    let shrdotP = schedule.shrdot;
    let shrdndP = schedule.shrdnd;
    let shrdndotP = schedule.shrdndot;

  if (shrdP > maximumRegularHourCount) {
    
    shrdP = maximumRegularHourCount
    shrdotP =  (schedule.shrd - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.shrdnd>0) {
    shrdndP = schedule.shrdnd - (shrdot + (lateMinuteP/60))
    shrdndotP = schedule.shrdnd - shrdndP


    if (shrdndP < 0) {
      shrdndotP = shrdndotP + shrdndP;
      shrdndP = 0;
    }

  }
  };
 
 
  if (this.timekeeping.constructionFlag && shrdP > 0 ) {

  let  overtimeHour = schedule.shrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount

    shrdP = regularHour
    shrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.shrdot)   //round 0'


    if (shrdotP<0) {
      shrdP = shrdP + shrdotP;

      if (shrdP < maximumRegularHourCount && ((shrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        shrdP = shrdP + schedule.unpaidBreak;
      }


      shrdotP = 0;
    }






    undertimeMinuteP = 0
      shrdndP = (schedule.shrdnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    shrdndotP = (schedule.shrdndot /60).toFixed(1)


  // if ((schedule.shrdnd /60).toFixed(1)>0) {
  //   shrdndP = (schedule.shrdnd /60).toFixed(1) - (shrdot + (lateMinuteP/60))
  //   shrdndotP = (schedule.shrdnd /60).toFixed(1) - shrdndP


  //   if (shrdndP < 0) {
  //     shrdndotP = shrdndotP + shrdndP;
  //     shrdndP = 0;
  //   }
  // } 
};

 


 // Legal Restday Holiday
    let lhrdP = schedule.lhrd ;
    let lhrdotP = schedule.lhrdot;
    let lhrdndP = schedule.lhrdnd;
    let lhrdndotP = schedule.lhrdndot;

    if (lhrdP > maximumRegularHourCount) {
    
    lhrdP = maximumRegularHourCount
    lhrdotP =  (schedule.lhrd - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.lhrdnd>0) {
    lhrdndP = schedule.lhrdnd - (lhrdotP + (lateMinuteP/60))
    lhrdndotP = schedule.lhrdnd - lhrdndP

    if (lhrdndP < 0) {
      lhrdndotP = lhrdndotP + lhrdndP;
      lhrdndP = 0;
    }


    }
  };

 
  if (this.timekeeping.constructionFlag && lhrdP > 0 ) {

  let  overtimeHour = schedule.lhrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhrdP = regularHour
    lhrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhrdot)   //round 0'

    if (lhrdotP<0) {
      lhrdP = lhrdP + lhrdotP;

      if (lhrdP < maximumRegularHourCount && ((lhrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhrdP = lhrdP + schedule.unpaidBreak;
      }

      lhrdotP = 0;
    }


    undertimeMinuteP = 0
  //   if ((schedule.lhrdnd /60).toFixed(1)>0) {
  //   lhrdndP = (schedule.lhrdnd /60).toFixed(1) - (lhrdotP + (lateMinuteP/60))
  //   lhrdndotP = (schedule.lhrdnd /60).toFixed(1) - lhrdndP

  //   if (lhrdndP < 0) {
  //     lhrdndotP = lhrdndotP + lhrdndP;
  //     lhrdndP = 0;
  //   }
  // } 


      lhrdndP = (schedule.lhrdnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    lhrdndotP = (schedule.lhrdndot /60).toFixed(1)


};




 // Legal Holiday
    let lhP = schedule.lh;
    let lhotP = schedule.lhot;
    let lhndP = schedule.lhnd;
    let lhndotP = schedule.lhndot;

  if (lhP > maximumRegularHourCount) {
    
    lhP = maximumRegularHourCount
    lhotP =  (schedule.lh - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.lhnd>0) {
    lhndP = schedule.lhnd - (lhotP + (lateMinuteP/60))
    lhndotP = schedule.lhnd - lhndP

    if (lhndP < 0) {
      lhndotP = lhndotP + lhndP;


      lhndP = 0;
    }

    }
  };
 
  if (this.timekeeping.constructionFlag && lhP > 0 ) {

  let  overtimeHour = schedule.lh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhP = regularHour
    lhotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhot )   //round 0'

    if (lhotP<0) {
      lhP = lhP + lhotP;

      if (lhP < maximumRegularHourCount && ((lhP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhP = lhP + schedule.unpaidBreak;
      }

      lhotP = 0;
    }



    undertimeMinuteP = 0
  //  if ((schedule.lhnd /60).toFixed(1)>0) {
  //   lhndP = (schedule.lhnd /60).toFixed(1) - (lhotP + (lateMinuteP/60))
  //   lhndotP = (schedule.lhnd /60).toFixed(1) - lhndP

  //   if (lhndP < 0) {
  //     lhndotP = lhndotP + lhndP;
  //     lhndP = 0;
  //   }

        lhndP = (schedule.lhnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    lhndotP = (schedule.lhndot /60).toFixed(1)


  // }  
  
  //  if ((+(schedule.lhnd / 60).toFixed(1)) === 0 && +(schedule.lhndot / 60).toFixed(1) > 0) {
  //   lhndP = 0;

  //   lhndotP = (schedule.lhndot /60).toFixed(1) - lhndP;

  //   if (lhndP < 0) {
  //     lhndotP = lhndotP + lhndP;
  //     lhndP = 0;
  //   }
    
  // }  
  



};




  // Double Regular Holiday
    let drhP = schedule.drh;
    let drhotP = schedule.drhot;
    let drhndP = schedule.drhnd;
    let drhndotP = schedule.drhndot;

  if (drhP > maximumRegularHourCount) {
    
    drhP = maximumRegularHourCount
    drhotP =  (schedule.drh - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.drhnd>0) {
    drhndP = schedule.drhnd - (drhotP + (lateMinuteP/60))
    drhndotP = schedule.drhnd - drhndP

    if (drhndP < 0) {
      drhndotP = drhndotP + drhndP;
      drhndP = 0;
    }


    }
  };

  if (this.timekeeping.constructionFlag && drhP > 0 ) {

  let  overtimeHour = schedule.drh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    drhP = regularHour
    drhotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.drhot )   //round 0'

    if (drhotP<0) {
      drhP = drhP + drhotP;

      if (drhP < maximumRegularHourCount && ((drhP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        drhP = drhP + schedule.unpaidBreak;
      }

      drhotP = 0;
    }


    undertimeMinuteP = 0

    drhndP = (schedule.drhnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    drhndotP = (schedule.drhndot /60).toFixed(1)


  // if ((schedule.drhnd /60).toFixed(1)>0) {
  //   drhndP = (schedule.drhnd /60).toFixed(1) - (drhotP + (lateMinuteP/60))
  //   drhndotP = (schedule.drhnd /60).toFixed(1) - drhndP

  //   if (drhndP < 0) {
  //     drhndotP = drhndotP + drhndP;
  //     drhndP = 0;
  //   }


  // } 
};




  // Legal Special Holiday
    let lhshP = schedule.lhsh;
    let lhshotP = schedule.lhshot;
    let lhshndP = schedule.lhshnd;
    let lhshndotP = schedule.lhshndot;

  if (lhshP > maximumRegularHourCount) {
    
    lhshP = maximumRegularHourCount
    lhshotP =  (schedule.lhsh - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.lhshnd>0) {
    lhshndP = schedule.lhshnd - (lhshotP + (lateMinuteP/60))
    lhshndotP = schedule.lhshnd - lhshndP

    if (lhshndP < 0) {
      lhshndotP = lhshndotP + lhshndP;
      lhshndP = 0;
    }



    }
  };


  if (this.timekeeping.constructionFlag && lhshP > 0 ) {

  let  overtimeHour = schedule.lhsh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhshP = regularHour
    lhshotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhshot  )   //round 0'


    if (lhshotP<0) {
      lhshP = lhshP + lhshotP;

      if (lhshP < maximumRegularHourCount && ((lhshP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhshP = lhshP + schedule.unpaidBreak;
      }

      lhshotP = 0;
    }


    undertimeMinuteP = 0

  lhshndP = (schedule.lhshnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    lhshndotP = (schedule.lhshndot /60).toFixed(1)


//  if ((schedule.lhshnd /60).toFixed(1)>0) {
//     lhshndP = (schedule.lhshnd /60).toFixed(1) - (lhshotP + (lateMinuteP/60))
//     lhshndotP = (schedule.lhshnd /60).toFixed(1) - lhshndP

//     if (lhshndP < 0) {
//       lhshndotP = lhshndotP + lhshndP;
//       lhshndP = 0;
//     }


//   } 
};


  // Legal Special Holiday Rest Day
    let lhshrdP = schedule.lhshrd;
    let lhshrdotP = schedule.lhshrdot;
    let lhshrdndP = schedule.lhshrdnd;
    let lhshrdndotP = schedule.lhshrdndot;

  if (lhshrdP > maximumRegularHourCount) {
    
    lhshrdP = maximumRegularHourCount
    lhshrdotP =  (schedule.lhshrd - (lateMinuteP/60)) - maximumRegularHourCount
    if ((schedule.lhshrdnd /60).toFixed(1)>0) {
    lhshrdndP = (schedule.lhshrdnd /60).toFixed(1) - (lhshrdotP + (lateMinuteP/60))
    lhshrdndotP = (schedule.lhshrdnd /60).toFixed(1) - lhshrdndP

    if (lhshrdndP < 0) {
      lhshrdndotP = lhshrdndotP + lhshrdndP;
      lhshrdndP = 0;
    }


    }
  };

  if (this.timekeeping.constructionFlag && lhshrdP > 0 ) {

  let  overtimeHour = schedule.lhshrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhshrdP = regularHour
    lhshrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhshrdot)   //round 0'


    if (lhshrdotP<0) {
      lhshrdP = lhshrdP + lhshrdotP;

      if (lhshrdP < maximumRegularHourCount && ((lhshrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhshrdP = lhshrdP + schedule.unpaidBreak;
      }

      lhshrdotP = 0;
    }


    undertimeMinuteP = 0


  lhshrdndP = (schedule.lhshrdnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    lhshrdndotP = (schedule.lhshrdndot /60).toFixed(1)


//  if ((schedule.lhshrdnd /60).toFixed(1)>0) {
//     lhshrdndP = (schedule.lhshrdnd /60).toFixed(1) - (lhshrdotP + (lateMinuteP/60))
//     lhshrdndotP = (schedule.lhshrdnd /60).toFixed(1) - lhshrdndP

//     if (lhshrdndP < 0) {
//       lhshrdndotP = lhshrdndotP + lhshrdndP;
//       lhshrdndP = 0;
//     }


//   } 
};



  // Double Special Holiday Rest Day
    let drhrdP = schedule.drhrd;
    let drhrdotP = schedule.drhrdot;
    let drhrdndP = schedule.drhrdnd;
    let drhrdndotP = schedule.drhrdnd;

  if (drhrdP > maximumRegularHourCount) {
    
    drhrdP = maximumRegularHourCount
    drhrdotP =  (schedule.drhrd - (lateMinuteP/60)) - maximumRegularHourCount
    if (schedule.drhrdnd>0) {
    drhrdndP = schedule.drhrdnd - (drhrdotP + (lateMinuteP/60))
    drhrdndotP = schedule.drhrdnd - drhrdndP


    if (drhrdndP < 0) {
      drhrdndotP = drhrdndotP + drhrdndP;

      drhrdndP = 0;
    }


    }
  };


  if (this.timekeeping.constructionFlag && drhrdP > 0 ) {
  let  overtimeHour = schedule.drhrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    drhrdP = regularHour
    drhrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.drhrdot)   //round 0'



    if (drhrdotP<0) {
      drhrdP = drhrdP + drhrdotP;

      if (drhrdP < maximumRegularHourCount && ((drhrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        drhrdP = drhrdP + schedule.unpaidBreak;
      }


      drhrdotP = 0;
    }


    undertimeMinuteP = 0

  drhrdndP = (schedule.drhrdnd /60).toFixed(1)  //- (overtimeHourP + (lateMinuteP/60))
    drhrdndotP = (schedule.drhrdndot /60).toFixed(1)


//  if ((schedule.drhrdnd /60).toFixed(1)>0) {
//     drhrdndP = (schedule.drhrdnd /60).toFixed(1) - (drhrdotP + (lateMinuteP/60))
//     drhrdndotP = (schedule.drhrdnd /60).toFixed(1) - drhrdndP


//     if (drhrdndP < 0) {
//       drhrdndotP = drhrdndotP + drhrdndP;
//       drhrdndP = 0;
//     }



//   } 



};


    if (lateCountId==1) {
      lateMinuteP = schedule.lateMinute;
    }


  //Paid Up Holiday
  // this.schedules is an array if this.schedules.lhrdP > 0 then locate backward in this.schedules 
  // if the first record backward this.schedules.regularHourP > 0 then 
  // 

  // Last Scheduled Working Day  

  let hu100P = 0 ;
  if (paidUpHolidayId==2 && lhrdP> 0) {
  
    if (lhrdP > 0) {

    for (let i = this.schedules.length - 1; i >= 0; i--) {
      if (this.schedules[i].regularHourP > 0 || this.schedules[i].rdp > 0 || this.schedules[i].shp > 0
        || this.schedules[i].shrdp > 0 || this.schedules[i].lhrdp > 0 || this.schedules[i].lhp > 0
        || this.schedules[i].lhshp > 0 || this.schedules[i].lhshrdp > 0 || this.schedules[i].drhp > 0
        || this.schedules[i].drhrdP > 0
      ) {
        for (let j = i - 1; j >= 0; j--) {
          if (this.schedules[j].regularHourP > 0 || this.schedules[j].rdp > 0 || this.schedules[j].shp > 0
        || this.schedules[j].shrdp > 0 || this.schedules[j].lhrdp > 0 || this.schedules[j].lhp > 0
        || this.schedules[j].lhshp > 0 || this.schedules[j].lhshrdp > 0 || this.schedules[j].drhp > 0
        || this.schedules[j].drhrdP > 0 ) {
 
const row = this.schedules[j];

 hu100P = 
  (typeof row.regularHourP === 'number' ? row.regularHourP : 0) +
  (typeof row.rdp === 'number' ? row.rdp : 0) +
  (typeof row.shp === 'number' ? row.shp : 0) +
  (typeof row.shrdp === 'number' ? row.shrdp : 0) +
  (typeof row.lhrdp === 'number' ? row.lhrdp : 0) +
  (typeof row.lhp === 'number' ? row.lhp : 0) +
  (typeof row.lhshp === 'number' ? row.lhshp : 0) +
  (typeof row.lhshrdp === 'number' ? row.lhshrdp : 0) +
  (typeof row.drhp === 'number' ? row.drhp : 0) +
  (typeof row.drhrdP === 'number' ? row.drhrdP : 0);
            
            break;
          }
        }
        break; 
      }
    }
  }


} 

if (paidUpHolidayId==1 && lhrdP> 0) {

for (let i = 1; i < this.schedules.length; i++) {
  if (this.schedules[i - 1].regularHourP > 0 || this.schedules[i - 1].rdp > 0 || this.schedules[i - 1].shp > 0
|| this.schedules[i - 1].shrdp > 0
|| this.schedules[i - 1].lhrdp > 0
|| this.schedules[i - 1].lhp > 0
|| this.schedules[i - 1].lhshp > 0
|| this.schedules[i - 1].lhshrdp > 0
|| this.schedules[i - 1].drhp > 0
|| this.schedules[i - 1].drhrdP > 0

  ) {
  
  
  const row = this.schedules[i - 1];

hu100P =
  (typeof row.regularHourP === 'number' ? row.regularHourP : 0) +
  (typeof row.rdp === 'number' ? row.rdp : 0) +
  (typeof row.shp === 'number' ? row.shp : 0) +
  (typeof row.shrdp === 'number' ? row.shrdp : 0) +
  (typeof row.lhrdp === 'number' ? row.lhrdp : 0) +
  (typeof row.lhp === 'number' ? row.lhp : 0) +
  (typeof row.lhshp === 'number' ? row.lhshp : 0) +
  (typeof row.lhshrdp === 'number' ? row.lhshrdp : 0) +
  (typeof row.drhp === 'number' ? row.drhp : 0) +
  (typeof row.drhrdP === 'number' ? row.drhrdP : 0);



    break; // Stop after the first match
  }
}


}



// if (restDayOffSetFlag && restDayConsideration!==0) {
// const hasNoScheduleId1 = this.schedules.some(item => item.noScheduleId === 1);

// let count = 0;

// if (hasNoScheduleId1) {
//   count = this.schedules.filter(item =>
//     item.noScheduleId !== 1 &&
//     !!item.timekeepingScheduleId 
//   ).length;
// }

// console.log("Count:", count);
// console.log("Count:", restDayConsideration);

// if (count < restDayConsideration) {
//   this.schedules.forEach(item => {
//     if (item.noScheduleId === 1) {
//       item.regularHourP  = item.rdP;
//       item.overtimeHourP = item.rdotP || 0;
//       item.ndP           = item.rdndP || 0;
//       item.ndotP         = item.rdndotP || 0;

//       item.rdP      = 0;
//       item.rdotP    = 0;
//       item.rdndP    = 0;
//       item.rdndotP  = 0;
//     }
//   });
// }

// }
// Late
  if (this.timekeeping.constructionFlag && lateMinuteP > 0)  {
    lateMinuteP = lateMinuteP/60;
  }

  // undertimeMinuteP = undertimeMinute;
  
  // Return the updated object
    return {
      ...schedule,
      regularHourP: regularHourP,
      overtimeHourP: overtimeHourP,
      lateMinuteP: lateMinuteP,
      undertimeMinuteP: undertimeMinuteP,
      ndp: ndP,
      ndotp: ndotP,

      rdp: rdP,
      rdotp: rdotP,
      rdndp: rdndP,
      rdndotp: rdndotP,

      shp: shP,
      shotp: shotP,
      shndp: shndP,
      shndotp: shndotP,

      shrdp: shrdP,
      shrdotp: shrdotP,
      shrdndp: shrdndP,
      shrdndotp: shrdndotP,

      lhrdp: lhrdP,
      lhrdotp: lhrdotP,
      lhrdndp: lhrdndP,
      lhrdndotp: lhrdndotP,

      lhp: lhP,
      lhotp: lhotP,
      lhndp: lhndP,
      lhndotp: lhndotP,

      lhshp: lhshP,
      lhshotp: lhshotP,
       lhshndp: lhshndP,
      lhshndotp: lhshndotP,
      
      lhshrdp: lhshrdP,
      lhshrdotp: lhshrdotP,
      lhshrdndp:  lhshrdndP,
      lhshrdndotp: lhshrdndotP,

      hu100p: hu100P,

      drhp: drhP,
      drhotp: drhotP,
      drhndp: drhndP,
      drhndotp: drhndotP,

      drhrdp: drhrdP,
      drhrdotp: drhrdotP,
      drhrdndp: drhrdndP,
      drhrdndotp: drhrdndotP

    };
  });


  this.onApplyRestDayOffSetting();

  
  // console.log(this.schedules);
},

    getTableWrapperStyle(rowCount) {
      const rowHeight = 10;
    const headerHeight = 50;
    const footerHeight = 40;

    const totalContentHeight = headerHeight + (rowCount * rowHeight) + footerHeight;
    const maxAllowedHeight = window.innerHeight * 1.5;

    return {
      maxHeight: `${maxAllowedHeight}px`,
      minHeight: `${totalContentHeight}px`,
      height: totalContentHeight > maxAllowedHeight ? `${maxAllowedHeight}px` : `${totalContentHeight}px`,
      overflowY: totalContentHeight > maxAllowedHeight ? 'auto' : 'visible',
    };
    },

    onEditDoc(dtl, index) {
      
      // hold current property values of selected detail for editing; passed index too.
      const d = this.schedule;
      this.docIndex = index;

      //  this.docIndex = index;
   
      d.sheetMemberId = dtl.sheetMemberId;
      d.memberId = dtl.memberId;
      d.otRemarks = dtl.otRemarks;
      d.otFileName = dtl.otFileName;
      d.otGUID = dtl.otGUID;
      this.fileName = dtl.otFileName

      this.isDocEditorVisible = true;
    },


toggleAllPayableToMember(entries) {
    entries.forEach(dtl => {
      dtl.payableToMemberFlag = dtl.payableToMemberFlag ? 0 : 1;
      const sched = this.schedules.find(s =>
        s.memberId === dtl.memberId && s.scheduleDate === dtl.scheduleDate
      );
      if (sched) sched.payableToMemberFlag = dtl.payableToMemberFlag;
    });
  },

  toggleAllBillableToClient(entries) {
    entries.forEach(dtl => {
      dtl.billableToClientFlag = dtl.billableToClientFlag ? 0 : 1;
      const sched = this.schedules.find(s =>
        s.memberId === dtl.memberId && s.scheduleDate === dtl.scheduleDate
      );
      if (sched) sched.billableToClientFlag = dtl.billableToClientFlag;
    });
  },

    toggleBillableToClientFlag(dtl) {
    
      dtl.billableToClientFlag = dtl.billableToClientFlag ? 0 : 1;

      const match = this.schedules.find(
        item =>
          item.memberId === dtl.memberId &&
          item.scheduleDate === dtl.scheduleDate
      );

      if (match) {
        match.billableToClientFlag = dtl.billableToClientFlag;
      }
    },



    togglePayableToMemberFlag(dtl) {
    
      dtl.payableToMemberFlag = dtl.payableToMemberFlag ? 0 : 1;

      const match = this.schedules.find(
        item =>
          item.memberId === dtl.memberId &&
          item.scheduleDate === dtl.scheduleDate
      );

      if (match) {
        match.payableToMemberFlag = dtl.payableToMemberFlag;
      }
    },

// getBooleanIconClass(value) {
//   return value === 1
//     ? 'fa fa-check-circle text-success'
//     : 'fa fa-times-circle text-danger';
// },

  // getBooleanIconClass(value) {
  //   return value ? 'fa fa-check-circle text-success' : 'fa fa-times-circle text-danger';
  // },

        onSubmitDoc() {
      
      // const me = this, d = me.schedule;


      // if (me.isUploadRequired) {
      // }

      // if (!me.isValid("hrs0010D")) {
      //   me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 });
      //   return;
      // }

      // if (this.doc.docTypeReference.length < this.doc.docTypeLength) {
      //   this.advice.fault('Entry is not valid.', { duration: 5 });
      //   return;
      // }

        // let dtl = {};

const me = this;
const d = me.schedule; // assuming d is the updated doc data
        if (!me.fileName || me.fileName.trim() === "") {
          me.advice.fault('Upload required. Please select a file.', { duration: 5 });
          return;
        }

        // Update the doc at the current index
let dtl = me.schedules[me.docIndex];

dtl.memberId = d.memberId;
dtl.otRemarks = d.otRemarks;
dtl.otFileName = me.fileName || d.otFileName;

dtl.otGUID = me.guidReference || d.otGUID;

// Update ALL schedules with the same memberId
me.schedules.forEach(sch => {

  if (sch.memberId == dtl.memberId) {
    
    sch.otRemarks = dtl.otRemarks;    
    sch.otFileName = dtl.otFileName 
    sch.otGUID = dtl.otGUID;
  }
});

// console.log(me.schedules)
// Close editor
me.isDocEditorVisible = false;

        // dtl = me.schedules[me.docIndex];
        
        // // if (me.isAddingDoc) {
        // //   Object.assign(dtl, d);

        // //   me.schedules.push(dtl);

        // //   dtl.otFileName = me.fileName;
        // //   dtl.otGUID = me.guidReference;

        // //   me.clearDocPad();
        // //   me.advice.info("Doc Type Name '" + dtl.OTRemarks + "' added to list.", { duration: 5 });
        // //   me.setFocus("DocTypeId");
        // // } else {
        //   // dtl = me.schedules[me.docIndex];
        //   dtl.memberId = d.memberId;
        //   dtl.otRemarks = d.otRemarks;
        //   dtl.otFileName = me.fileName || d.otFileName;
        //   dtl.otGUID = me.guidReference || d.otGUID;


        //   update this.schedules where memberId = dtl.memberId
        //   schedules.otRemarks = d.otRemarks;
        //   schedules.otFileName = me.fileName || d.otFileName;
        //   schedules.otGUID = me.guidReference || d.otGUID;


        //   me.isDocEditorVisible = false;
        // }
      
    },

    
    showUploadDocumentsResponse (info) {
      if (!info || (!info.createdCount && !info.failedCount)) {
        return Promise.resolve();
      }

      this.docsUploadResult = info;

      setTimeout(() => {
        this.isModalVisible = true;
      }, 200);

      return Promise.resolve();
    },


    onSelectDocuments () {
      // const me = this;

      // testing for file size limit as set in AppConfig.json
      // loop in reverse, so splice works

      // if (appConfig.uploadMaxFileSize) {
      //   for (let i = e.files.length - 1; i >= 0; i--) {
      //     if (e.files[i].size > appConfig.uploadMaxFileSize) {
      //       let mssg = 'File rejected. (' + e.files[i].name + ')<hr>Maximum file size is ' + appConfig.uploadMaxFileSizeText;
      //       this.advice.fault(mssg, 10);
      //       e.files.splice(i, 1);
      //     }
      //   }
      // }


      // omit files already uploaded

      //  if (e.files.length && this.docTypes.length) {
      //    for (let i = e.files.length - 1; i >= 0; i--) {
      //     if (me.documentNames.findIndex(obj => obj.documentName === e.files[i].name) > -1) {
      //       let mssg = 'File omitted; already uploaded.<hr>' + e.files[i].name;
      //        me.advice.fault(mssg, 8);
      //        e.files.splice(i, 1);
      //     }
      //    }
      //  }

    },

    onSelectedChanged (fileList) {

    this.selectedFileList = fileList;

    if (fileList.length) {
      const uploader = this.$refs.uploader;
      if (uploader) {
        uploader.invokeUpload();
      }
    }

    },

    onUploadDocuments (e) {
      const
        me = this,
        wait = me.wait();

        me.fileName = "";
        me.guidReference = this.guid();

        me.uploadDocuments(e.fileList)
        .then( info => {
          me.stopWait(wait);

          e.reset();
            e.showUploadResponse(info)
            e.fileList.forEach(file => {
            me.fileName = file.name;
          });

          me.showUploadDocumentsResponse(info)
            .then( () => {

            });

        })
        .catch( fault => {
          me.stopWait(wait);
          e.reset();
          me.showFault(fault);
        });

    },

    onSelectFile () {
     
    const uploader = this.$refs.uploader;
    
    if (uploader) {
      uploader.invokeClick();
    }
    },

    onShowDocEditor() {
      const me = this;

      me.setActiveModel("schedule");

      // me.setRequiredMode(
      //   'docTypeId',
      //   'docTypeReference'
      // );

      // setTimeout(() => {
      //   this.setFocus("docTypeId");
      // }, 200);
    },

    onHideDocEditor() {
      const me = this;

      me.isAddingDoc = false;
      me.setActiveModel();
    },


    onAddDoc(memberId) {
      const me = this;

      me.clearDocPad();
      // me.shedules.docTypeDetailId = -1;
      me.otMemberId =memberId;

      me.isDocEditorVisible = true;
      me.isAddingDoc = true;
    },

    clearDocPad() {
      const d = this.schedule;

   
      d.otReference = "";
      d.otFileName = "";
      d.otGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },


parseTimeWithoutSeconds(timeStr) {
  if (!timeStr) return null;
  const [hours, minutes] = timeStr.split(':');
  if (hours === undefined || minutes === undefined) return null;
  // Create a Date object for Excel (time only)
  const date = new Date(1899, 11, 30, +hours, +minutes); 
  // Excel’s "zero" date is Dec 30, 1899 — common for time-only values
  return date;
},

async generateExcelReport() {
  const grouped = {};

  // Group schedules by memberId and calculate totals
  this.schedules.forEach(s => {
    if (!grouped[s.memberId]) {
      grouped[s.memberId] = {
        memberName: s.memberName,
        departmentName: s.departmentName,
        otFileName: s.otFileName,
        otRemarks: s.otRemarks,
        fileUrl: s.fileUrl,
        entries: [],
        totals: {
          regularHour: 0,
          overtimeHour: 0,
          regularWorkingDayHour: 0,
          regularOvertimeHour: 0,
          lateMinute: 0,
          undertimeMinute: 0,
          nd: 0,
          ndot: 0,
          rd: 0,
          rdot: 0,
          rdnd: 0,
          rdndot: 0,
          sh: 0,
          shot: 0,
          shnd: 0,
          shndot: 0,
          shrd: 0,
          shrdot: 0,
          shrdnd: 0,
          shrdndot: 0,
          lhrd: 0,
          lhrdot: 0,
          lhrdnd: 0,
          lhrdndot: 0,
          lh: 0,
          lhot: 0,
          lhnd: 0,
          lhndot: 0,
          lhsh: 0,
          lhshrd: 0,
          hu100: 0,
          drh: 0,
          drhot: 0,
          drhrd: 0,
          drhrdot: 0,

          regularHourP: 0,
          overtimeHourP: 0,
          regularWorkingDayHourP: 0,
          regularOvertimeHourP: 0,
          lateMinuteP: 0,
          undertimeMinuteP: 0,
          ndp: 0,
          ndotp: 0,
          rdp: 0,
          rdotp: 0,
          rdndp: 0,
          rdndotp: 0,
          shp: 0,
          shotp: 0,
          shndp: 0,
          shndotp: 0,
          shrdp: 0,
          shrdotp: 0,
          shrdndp: 0,
          shrdndotp: 0,
          lhrdp: 0,
          lhrdotp: 0,
          lhrdndp: 0,
          lhrdndotp: 0,
          lhp: 0,
          lhotp: 0,
          lhndp: 0,
          lhndotp: 0,
          lhshp: 0,
          lhshrdp: 0,
          hu100p: 0,
          drhp: 0,
          drhotp: 0,
          drhrdp: 0,
          drhrdotp: 0,

      
        }
      };
    }
     
    console.log(s)

    grouped[s.memberId].entries.push(s);

    const t = grouped[s.memberId].totals;
    console.log(this.timekeeping.applyOffSetFlag)

    if (!this.timekeeping.applyOffSetFlag) {  
      t.regularHour += +s.regularHour || 0;
      t.overtimeHour += +s.overtimeHour || 0;
      t.lateMinute += +s.lateMinute || 0;
      t.undertimeMinute += +s.undertimeMinute || 0;

      t.nd += +s.nd || 0;
      t.ndot += +s.ndot || 0;

      t.rd += +s.rd || 0;
      t.rdot += +s.rdot || 0;
      t.rdnd += +s.rdnd || 0;
      t.rdndot += +s.rdndot || 0;
      t.sh += +s.sh || 0;
      t.shot += +s.shot || 0;
      t.shnd += +s.shnd || 0;
      t.shndot += +s.shndot || 0;
      t.shrd += +s.shrd || 0;
      t.shrdot += +s.shrdot || 0;
      t.shrdnd += +s.shrdnd || 0;
      t.shrdndot += +s.shrdndot || 0;
      t.lhrd += +s.lhrd || 0;
      t.lhrdot += +s.lhrdot || 0;
      t.lhrdnd += +s.lhrdnd || 0;
      t.lhrdndot += +s.lhrdndot || 0;
      t.lh += +s.lh || 0;
      t.lhot += +s.lhot || 0;
      t.lhnd += +s.lhnd || 0;
      t.lhndot += +s.lhndot || 0;
      t.lhsh += +s.lhsh || 0;
      t.lhshrd += +s.lhshrd || 0;
      t.hu100 += +s.hu100 || 0;
      t.drh += +s.drh || 0;
      t.drhot += +s.drhot || 0;
      t.drhrd += +s.drhrd || 0;
      t.drhrdot += +s.drhrdot || 0;
    } else {
      console.log('+s.+s.undertimeMinuteP',+s.undertimeMinuteP)  
      t.regularHour += +s.regularHourP || 0;
      t.overtimeHour += +s.overtimeHourP || 0;
      t.lateMinute += +s.lateMinuteP || 0;
      t.undertimeMinute += +s.undertimeMinuteP || 0;

      t.nd += +s.ndp || 0;
      t.ndot += +s.ndotp || 0;

      t.rd += +s.rdp || 0;
      t.rdot += +s.rdotp || 0;
      t.rdnd += +s.rdndp || 0;
      t.rdndot += +s.rdndotp || 0;
      t.sh += +s.shp || 0;
      t.shot += +s.shotp || 0;
      t.shnd += +s.shndp || 0;
      t.shndot += +s.shndotp || 0;
      t.shrd += +s.shrdp || 0;
      t.shrdot += +s.shrdotp || 0;
      t.shrdnd += +s.shrdndp || 0;
      t.shrdndot += +s.shrdndotp || 0;
      t.lhrd += +s.lhrdp || 0;
      t.lhrdot += +s.lhrdotp || 0;
      t.lhrdnd += +s.lhrdndp || 0;
      t.lhrdndot += +s.lhrdndotp || 0;
      t.lh += +s.lhp || 0;
      t.lhot += +s.lhotp || 0;
      t.lhnd += +s.lhndp || 0;
      t.lhndot += +s.lhndotp || 0;
      t.lhsh += +s.lhshp || 0;
      t.lhshrd += +s.lhshrdp || 0;
      t.hu100 += +s.hu100p || 0;
      t.drh += +s.drhp || 0;
      t.drhot += +s.drhotp || 0;
      t.drhrd += +s.drhrdp || 0;
      t.drhrdot += +s.drhrdotp || 0;

    }
      });

  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet('Schedules');

  const headerFill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FF0070C0' }
  };
  const headerFont = { bold: true, color: { argb: 'FFFFFFFF' } };
  const borderStyle = { style: 'thin', color: { argb: 'FF000000' } };
  const border = {
    top: borderStyle,
    bottom: borderStyle,
    left: borderStyle,
    right: borderStyle
  };

  let currentRow = 1;

  for (const memberId in grouped) {
    const group = grouped[memberId];

    // Member info rows
    worksheet.getCell(`A${currentRow}`).value = "Member ID:";
    worksheet.getCell(`A${currentRow}`).font = { bold: true };
    worksheet.getCell(`A${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(currentRow, 2, currentRow, 15);
    worksheet.getCell(`B${currentRow}`).value = memberId;
    worksheet.getCell(`B${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    currentRow++;

    worksheet.getCell(`A${currentRow}`).value = "Member Name:";
    worksheet.getCell(`A${currentRow}`).font = { bold: true };
    worksheet.getCell(`A${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(currentRow, 2, currentRow, 15);
    worksheet.getCell(`B${currentRow}`).value = group.memberName;
    worksheet.getCell(`B${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    currentRow++;

    worksheet.getCell(`A${currentRow}`).value = "Department:";
    worksheet.getCell(`A${currentRow}`).font = { bold: true };
    worksheet.getCell(`A${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(currentRow, 2, currentRow, 15);
    worksheet.getCell(`B${currentRow}`).value = group.departmentName;
    worksheet.getCell(`B${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    currentRow++;

    worksheet.getCell(`A${currentRow}`).value = "Covered Date:";
    worksheet.getCell(`A${currentRow}`).font = { bold: true };
    worksheet.getCell(`A${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    worksheet.mergeCells(currentRow, 2, currentRow, 15);
    worksheet.getCell(`B${currentRow}`).value = `${this.timekeeping.cutOffStartDate.toDateFormat()} to ${this.timekeeping.cutOffEndDate.toDateFormat()}`;
    worksheet.getCell(`B${currentRow}`).alignment = { vertical: 'middle', horizontal: 'left' };
    currentRow++;

    // Column headers
    const headers = [
      "Schedule Date", "Day Type", "Schedule Code", "Time In", "Time Out",
      "Regular Hours",
      "Regular Overtime Hours", "Late (mins)", "Undertime (mins)",
      "Payable To Member", "Billable To Client", "Remarks",
      "ND", "NDOT","RD", "RDOT", "RDND", "RDNDOT",
      "SH", "SHOT", "SHND", "SHNDOT",
      "SHRD", "SHRDOT", "SHRDND", "SHRDNDOT",
      "LHRD", "LHRDOT", "LHRDND", "LHRDNDOT",
      "LH", "LHOT", "LHND", "LHNDOT",
      "LHSH", "LHSHRD", "HU-100",
      "DRH", "DRHOT", "DRHRD", "DRHRDOT"
    ];

    worksheet.addRow(headers);
    headers.forEach((_, i) => {
      const cell = worksheet.getCell(currentRow, i + 1);
      cell.fill = headerFill;
      cell.font = headerFont;
      cell.border = border;
      cell.alignment = { horizontal: 'center', vertical: 'middle' };
    });
    currentRow++;

    // Data rows
    group.entries.forEach(entry => {
      const timeIn = entry.scheduleTimeIn
  ? (parseInt(entry.scheduleTimeIn.split(':')[0], 10) / 24 +
     parseInt(entry.scheduleTimeIn.split(':')[1], 10) / 1440)
  : null;

const timeOut = entry.scheduleTimeOut
  ? (parseInt(entry.scheduleTimeOut.split(':')[0], 10) / 24 +
     parseInt(entry.scheduleTimeOut.split(':')[1], 10) / 1440)
  : null;
        if (!this.timekeeping.applyOffSetFlag) {    
    const row = worksheet.addRow([
      entry.scheduleDate ? new Date(entry.scheduleDate) : null,
      entry.dayTypeName || '',
      entry.scheduleCode || '',
      timeIn,
      timeOut,
      +entry.regularHour || '',
      +entry.overtimeHour || '',
      // +entry.regularWorkingDayHour || '',
      // +entry.regularOvertimeHour || '',
      +entry.lateMinute || '',
      +entry.undertimeMinute || '',
      // entry.approvedFormFlag ? '✔️' : '',
      entry.payableToMemberFlag ? '✔️' : '',
      entry.billableToClientFlag ? '✔️' : '',
      entry.remarks || '',
      +entry.nd || '',
      +entry.ndot || '',
      +entry.rd || '',
      +entry.rdot || '',
      +entry.rdnd || '',
      +entry.rdndot || '',
      +entry.sh || '',
      +entry.shot || '',
      +entry.shnd || '',
      +entry.shndot || '',
      +entry.shrd || '',
      +entry.shrdot || '',
      +entry.shrdnd || '',
      +entry.shrdndot || '',
      +entry.lhrd || '',
      +entry.lhrdot || '',
      +entry.lhrdnd || '',
      +entry.lhrdndot || '',
      +entry.lh || '',
      +entry.lhot || '',
      +entry.lhnd || '',
      +entry.lhndot || '',
      +entry.lhsh || '',
      +entry.lhshrd || '',
      +entry.hu100 || '',
      +entry.drh || '',
      +entry.drhot || '',
      +entry.drhrd || '',
      +entry.drhrdot || ''

    ]);
   } else {
    const row = worksheet.addRow([
      entry.scheduleDate ? new Date(entry.scheduleDate) : null,
      entry.dayTypeName || '',
      entry.scheduleCode || '',
      timeIn,
      timeOut,
      +entry.regularHourP || '',
      +entry.overtimeHourP || '',
      // +entry.regularWorkingDayHour || '',
      // +entry.regularOvertimeHour || '',
      +entry.lateMinute || '',
      +entry.undertimeMinute || '',
      // entry.approvedFormFlag ? '✔️' : '',
      entry.payableToMemberFlag ? '✔️' : '',
      entry.billableToClientFlag ? '✔️' : '',
      entry.remarks || '',
      +entry.ndp || '',
      +entry.ndotp || '',
      +entry.rdp || '',
      +entry.rdotp || '',
      +entry.rdndp || '',
      +entry.rdndotp || '',
      +entry.shp || '',
      +entry.shotp || '',
      +entry.shndp || '',
      +entry.shndotp || '',
      +entry.shrdp || '',
      +entry.shrdotp || '',
      +entry.shrdndp || '',
      +entry.shrdndotp || '',
      +entry.lhrdp || '',
      +entry.lhrdotp || '',
      +entry.lhrdndp || '',
      +entry.lhrdndotp || '',
      +entry.lhp || '',
      +entry.lhotp || '',
      +entry.lhndp || '',
      +entry.lhndotp || '',
      +entry.lhshp || '',
      +entry.lhshrdp || '',
      +entry.hu100p || '',
      +entry.drhp || '',
      +entry.drhotp || '',
      +entry.drhrdp || '',
      +entry.drhrdotp || ''

    ]);
    
   }
  
      currentRow++;
    });
  
    // Totals
    const t = group.totals;
    const totalRow = worksheet.addRow([
      "TOTAL", "", "", "", "",
     
     
     
      t.regularHour.toFixed(2),
      t.overtimeHour.toFixed(2),
      t.lateMinute.toFixed(2),
      t.undertimeMinute.toFixed(2),
      "", "", "", 
      t.nd.toFixed(2),
      t.ndot.toFixed(2),
      t.rd.toFixed(2),
      t.rdot.toFixed(2),
      t.rdnd.toFixed(2),
      t.rdndot.toFixed(2),
      t.sh.toFixed(2),
      t.shot.toFixed(2),
      t.shnd.toFixed(2),
      t.shndot.toFixed(2),
      t.shrd.toFixed(2),
      t.shrdot.toFixed(2),
      t.shrdnd.toFixed(2),
      t.shrdndot.toFixed(2),
      t.lhrd.toFixed(2),
      t.lhrdot.toFixed(2),
      t.lhrdnd.toFixed(2),
      t.lhrdndot.toFixed(2),
      t.lh.toFixed(2),
      t.lhot.toFixed(2),
      t.lhnd.toFixed(2),
      t.lhndot.toFixed(2),
      t.lhsh.toFixed(2),
      t.lhshrd.toFixed(2),
      t.hu100.toFixed(2),
      t.drh.toFixed(2),
      t.drhot.toFixed(2),
      t.drhrd.toFixed(2),
      t.drhrdot.toFixed(2)

    ]);


    for (let col = 1; col <= 40; col++) {
      const cell = totalRow.getCell(col);
      cell.font = { bold: true, color: { argb: 'FFFFFFFF' } };
      cell.alignment = { horizontal: 'center', vertical: 'middle' };
      cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF0070C0' } };
      cell.border = border;
    }

    currentRow += 2; // one empty row between members
  }

  // Column formatting
  worksheet.getColumn(1).numFmt = 'mm/dd/yyyy'; // Schedule Date
  worksheet.getColumn(4).numFmt = 'hh:mm';      // Time In
  worksheet.getColumn(5).numFmt = 'hh:mm';      // Time Out

const colWidths = [
  15, 12, 15, 12, 12,
  15, 15, 12, 14,
  15, 18, 25,
  10,10,10, 10, 10, 10,
  10, 10, 10, 10,
  10, 10, 10, 10,
  10, 10, 10, 10,
  10, 10, 10, 10,
  10, 10, 10,
  10, 10, 10, 10
];

  colWidths.forEach((w, i) => worksheet.getColumn(i + 1).width = w);

  // Save file
  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], { type: 'application/octet-stream' });
  saveAs(blob, 'schedule-report.xlsx');
},


// Helper functions (adjust as needed)
formatDate(dt) {
  if (!dt) return "";
  // If dt has toJSDate (Luxon DateTime), convert it first
  if (typeof dt.toJSDate === "function") {
    return dt.toJSDate().toLocaleDateString("en-US");
  }
  // fallback if dt is plain Date or string
  const d = new Date(dt);
  if (isNaN(d.getTime())) return "";
  return d.toLocaleDateString("en-US");
},

formatTime(time) {
  if (!time) return "";
  // Assumes time string like "HH:mm:ss" or "HH:mm"
  const d = new Date(`1970-01-01T${time}`);
  return d.toLocaleTimeString("en-US", { hour: '2-digit', minute: '2-digit' });
},

tableToExcel(tableRef, worksheetName) {
  let table = tableRef;

  if (typeof tableRef === 'string') {
    table = this.$refs[tableRef];
  }

  const template = `
    <html xmlns:o="urn:schemas-microsoft-com:office:office"
          xmlns:x="urn:schemas-microsoft-com:office:excel"
          xmlns="http://www.w3.org/TR/REC-html40">
      <head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>
      <x:Name>${worksheetName}</x:Name>
      <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet>
      </x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head>
      <body><table>${table.innerHTML}</table></body>
    </html>`;

  const base64 = s => window.btoa(unescape(encodeURIComponent(s)));

  const uri = 'data:application/vnd.ms-excel;base64,';

  const link = document.createElement('a');
  link.href = uri + base64(template);
  link.download = `${worksheetName}.xls`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
},

  //     getGroupTotals(entries) {
  //   const totals = {
  //     regularHour: 0,
  //     overtimeHour: 0,
  //     regularWorkingDayHour: 0,
  //     regularOvertimeHour: 0,
  //     lateMinute: 0,
  //     undertimeMinute: 0,
  //   };

  //   entries.forEach(dtl => {
  //     totals.regularHour += parseFloat(dtl.regularHour) || 0;
  //     totals.overtimeHour += parseFloat(dtl.overtimeHour) || 0;
  //     totals.regularWorkingDayHour += parseFloat(dtl.regularWorkingDayHour) || 0;
  //     totals.regularOvertimeHour += parseFloat(dtl.regularOvertimeHour) || 0;
  //     totals.lateMinute += parseFloat(dtl.lateMinute) || 0;
  //     totals.undertimeMinute += parseFloat(dtl.undertimeMinute) || 0;
  //   });

  //   return {
  //     regularHour: totals.regularHour.toFixed(2),
  //     overtimeHour: totals.overtimeHour.toFixed(2),
  //     regularWorkingDayHour: totals.regularWorkingDayHour.toFixed(2),
  //     regularOvertimeHour: totals.regularOvertimeHour.toFixed(2),
  //     lateMinute: Math.round(totals.lateMinute),
  //     undertimeMinute: Math.round(totals.undertimeMinute),
  //   };
  // },

      getGroupTotals(entries) {
    const totals = {
      regularHour: 0,
      overtimeHour: 0,
      regularWorkingDayHour: 0,
      regularOvertimeHour: 0,
      lateMinute: 0,
      undertimeMinute: 0,
      nd: 0,
      ndot: 0,
      rd: 0,
      rdot: 0,
      rdnd: 0,
      rdndot: 0,
      sh: 0,
      shot: 0,
      shnd: 0,
      shndot: 0,
      shrd: 0,
      shrdot: 0,
      shrdnd: 0,
      shrdndot: 0,
      lhrd: 0,
      lhrdot: 0,
      lhrdnd: 0,
      lhrdndot: 0,
      lh: 0,
      lhot: 0,
      lhnd: 0,
      lhndot: 0,
      lhsh: 0,
      lhshot: 0,
      lhshnd: 0,
      lhshndot: 0,

      lhshrd: 0,
      lhshrdot: 0,
      lhshrdnd: 0,
      lhshrdndot: 0,

      hu100: 0,
      drh: 0,
      drhot: 0,
      drhnd: 0,
      drhndot: 0,
      drhrd: 0,
      drhrdot: 0,
      drhrdnd: 0,
      drhrdndot: 0,



      regularHourP: 0,
      overtimeHourP: 0,
      regularWorkingDayHourP: 0,
      regularOvertimeHourP: 0,
      lateMinuteP: 0,
      undertimeMinuteP: 0,
      ndp: 0,
      ndotp: 0,


      rdp: 0,
      rdotp: 0,
      rdndp: 0,
      rdndotp: 0,
      shp: 0,
      shotp: 0,
      shndp: 0,
      shndotp: 0,
      shrdp: 0,
      shrdotp: 0,
      shrdndp: 0,
      shrdndotp: 0,
      lhrdp: 0,
      lhrdotp: 0,
      lhrdndp: 0,
      lhrdndotp: 0,
      lhp: 0,
      lhotp: 0,
      lhndp: 0,
      lhndotp: 0,
      lhshp: 0,
      lhshotp: 0,
      lhshndp: 0,
      lhshndotp: 0,

      lhshrdp: 0,
      lhshrdotp: 0,
      lhshrdndp: 0,
      lhshrdndotp: 0,

      hu100p: 0,
      drhp: 0,
      drhotp: 0,
      drhndp: 0,
      drhndotp: 0,
      drhrdp: 0,
      drhrdotp: 0,
      drhrdndp: 0,
      drhrdndotp: 0,


    };

    entries.forEach(dtl => {
      totals.regularHour += parseFloat(dtl.regularHour) || 0;
      totals.overtimeHour += parseFloat(dtl.overtimeHour) || 0;
      totals.regularWorkingDayHour += parseFloat(dtl.regularWorkingDayHour) || 0;
      totals.regularOvertimeHour += parseFloat(dtl.regularOvertimeHour) || 0;
      totals.lateMinute += parseFloat(dtl.lateMinute) || 0;
      totals.undertimeMinute += parseFloat(dtl.undertimeMinute) || 0;
  totals.nd += parseFloat(dtl.nd) || 0;
  totals.ndot += parseFloat(dtl.ndot) || 0;
  totals.rd += parseFloat(dtl.rd) || 0;
  totals.rdot += parseFloat(dtl.rdot) || 0;
  totals.rdnd += parseFloat(dtl.rdnd) || 0;
  totals.rdndot += parseFloat(dtl.rdndot) || 0;
  totals.sh += parseFloat(dtl.sh) || 0;
  totals.shot += parseFloat(dtl.shot) || 0;
  totals.shnd += parseFloat(dtl.shnd) || 0;
  totals.shndot += parseFloat(dtl.shndot) || 0;
  totals.shrd += parseFloat(dtl.shrd) || 0;
  totals.shrdot += parseFloat(dtl.shrdot) || 0;
  totals.shrdnd += parseFloat(dtl.shrdnd) || 0;
  totals.shrdndot += parseFloat(dtl.shrdndot) || 0;
  totals.lhrd += parseFloat(dtl.lhrd) || 0;
  totals.lhrdot += parseFloat(dtl.lhrdot) || 0;
  totals.lhrdnd += parseFloat(dtl.lhrdnd) || 0;
  totals.lhrdndot += parseFloat(dtl.lhrdndot) || 0;
  totals.lh += parseFloat(dtl.lh) || 0;
  totals.lhot += parseFloat(dtl.lhot) || 0;
  totals.lhnd += parseFloat(dtl.lhnd) || 0;
  totals.lhndot += parseFloat(dtl.lhndot) || 0;
  totals.lhsh += parseFloat(dtl.lhsh) || 0;
  totals.lhshot += parseFloat(dtl.lhshot) || 0;
  totals.lhshnd += parseFloat(dtl.lhshnd) || 0;
  totals.lhshndot += parseFloat(dtl.lhshndot) || 0;

  totals.lhshrd += parseFloat(dtl.lhshrd) || 0;
  totals.lhshrdot += parseFloat(dtl.lhshrdot) || 0;
  totals.lhshrdnd += parseFloat(dtl.lhshrdnd) || 0;
  totals.lhshrdndot += parseFloat(dtl.lhshrdndot) || 0;

  totals.hu100 += parseFloat(dtl.hu100) || 0;
  totals.drh += parseFloat(dtl.drh) || 0;
  totals.drhot += parseFloat(dtl.drhot) || 0;
  totals.drhnd += parseFloat(dtl.drhnd) || 0;
  totals.drhndot += parseFloat(dtl.drhndot) || 0;

  totals.drhrd += parseFloat(dtl.drhrd) || 0;
  totals.drhrdot += parseFloat(dtl.drhrdot) || 0;
  totals.drhrdnd += parseFloat(dtl.drhrdnd) || 0;
    totals.drhrdndot += parseFloat(dtl.drhrdndot) || 0;


      totals.regularHourP += parseFloat(dtl.regularHourP) || 0;
      totals.overtimeHourP += parseFloat(dtl.overtimeHourP) || 0;
      totals.regularWorkingDayHourP += parseFloat(dtl.regularWorkingDayHourP) || 0;
      totals.regularOvertimeHourP += parseFloat(dtl.regularOvertimeHourP) || 0;
      totals.lateMinuteP += parseFloat(dtl.lateMinuteP) || 0;
      totals.undertimeMinuteP += parseFloat(dtl.undertimeMinuteP) || 0;
  totals.ndp += parseFloat(dtl.ndp) || 0;
  totals.ndotp += parseFloat(dtl.ndotp) || 0;

totals.rdp += parseFloat(dtl.rdp) || 0;
  totals.rdotp += parseFloat(dtl.rdotp) || 0;
  totals.rdndp += parseFloat(dtl.rdndp) || 0;
  totals.rdndotp += parseFloat(dtl.rdndotp) || 0;
  totals.shp += parseFloat(dtl.shp) || 0;
  totals.shotp += parseFloat(dtl.shotp) || 0;
  totals.shndp += parseFloat(dtl.shndp) || 0;
  totals.shndotp += parseFloat(dtl.shndotp) || 0;
  totals.shrdp += parseFloat(dtl.shrdp) || 0;
  totals.shrdotp += parseFloat(dtl.shrdotp) || 0;
  totals.shrdndp += parseFloat(dtl.shrdndp) || 0;
  totals.shrdndotp += parseFloat(dtl.shrdndotp) || 0;
  totals.lhrdp += parseFloat(dtl.lhrdp) || 0;
  totals.lhrdotp += parseFloat(dtl.lhrdotp) || 0;
  totals.lhrdndp += parseFloat(dtl.lhrdndp) || 0;
  totals.lhrdndotp += parseFloat(dtl.lhrdndotp) || 0;
  totals.lhp += parseFloat(dtl.lhp) || 0;
  totals.lhotp += parseFloat(dtl.lhotp) || 0;
  totals.lhndp += parseFloat(dtl.lhndp) || 0;
  totals.lhndotp += parseFloat(dtl.lhndotp) || 0;
  totals.lhshp += parseFloat(dtl.lhshp) || 0;
  totals.lhshotp += parseFloat(dtl.lhshotp) || 0;
  totals.lhshndp += parseFloat(dtl.lhshndp) || 0;
  totals.lhshndotp += parseFloat(dtl.lhshndotp) || 0;

  totals.lhshrdp += parseFloat(dtl.lhshrdp) || 0;
  totals.lhshrdotp += parseFloat(dtl.lhshrdotp) || 0;
  totals.lhshrdndp += parseFloat(dtl.lhshrdndp) || 0;
  totals.lhshrdndotp += parseFloat(dtl.lhshrdndotp) || 0;

  totals.hu100p += parseFloat(dtl.hu100p) || 0;
  totals.drhp += parseFloat(dtl.drhp) || 0;
  totals.drhotp += parseFloat(dtl.drhotp) || 0;
  totals.drhndp += parseFloat(dtl.drhndp) || 0;
  totals.drhndotp += parseFloat(dtl.drhndotp) || 0;

  totals.drhrdp += parseFloat(dtl.drhrdp) || 0;
  totals.drhrdotp += parseFloat(dtl.drhrdotp) || 0;
  totals.drhrdndp += parseFloat(dtl.drhrdndp) || 0;
    totals.drhrdndotp += parseFloat(dtl.drhrdndotp) || 0;






    });

    return {
      regularHour: totals.regularHour.toFixed(2),
      overtimeHour: totals.overtimeHour.toFixed(2),
      regularWorkingDayHour: totals.regularWorkingDayHour.toFixed(2),
      regularOvertimeHour: totals.regularOvertimeHour.toFixed(2),
      lateMinute: Math.round(totals.lateMinute),
      undertimeMinute: Math.round(totals.undertimeMinute),
      nd: totals.nd.toFixed(2),
      ndot: totals.ndot.toFixed(2),
      rd: totals.rd.toFixed(2),
      rdot: totals.rdot.toFixed(2),
      rdnd: totals.rdnd.toFixed(2),
      rdndot: totals.rdndot.toFixed(2),
      sh: totals.sh.toFixed(2),
      shot: totals.shot.toFixed(2),
      shnd: totals.shnd.toFixed(2),
      shndot: totals.shndot.toFixed(2),
      shrd: totals.shrd.toFixed(2),
      shrdot: totals.shrdot.toFixed(2),
      shrdnd: totals.shrdnd.toFixed(2),
      shrdndot: totals.shrdndot.toFixed(2),
      lhrd: totals.lhrd.toFixed(2),
      lhrdot: totals.lhrdot.toFixed(2),
      lhrdnd: totals.lhrdnd.toFixed(2),
      lhrdndot: totals.lhrdndot.toFixed(2),
      lh: totals.lh.toFixed(2),
      lhot: totals.lhot.toFixed(2),
      lhnd: totals.lhnd.toFixed(2),
      lhndot: totals.lhndot.toFixed(2),
      lhsh: totals.lhsh.toFixed(2),
      lhshot: totals.lhsh.toFixed(2),
      lhshnd: totals.lhsh.toFixed(2),
      lhshndot: totals.lhsh.toFixed(2),

      lhshrd: totals.lhshrd.toFixed(2),
      lhshrdot: totals.lhshrdot.toFixed(2),
      lhshrdnd: totals.lhshrdnd.toFixed(2),
      lhshrdndot: totals.lhshrdndot.toFixed(2),

      hu100: totals.hu100.toFixed(2),
      drh: totals.drh.toFixed(2),
      drhot: totals.drhot.toFixed(2),
      drhnd: totals.drhnd.toFixed(2),
      drhndot: totals.drhndot.toFixed(2),

      drhrd: totals.drhrd.toFixed(2),
      drhrdot: totals.drhrdot.toFixed(2),
      drhrdnd: totals.drhrdnd.toFixed(2),
      drhrdndot: totals.drhrdndot.toFixed(2),


      regularHourP: totals.regularHourP.toFixed(2),
      overtimeHourP: totals.overtimeHourP.toFixed(2),
      lateMinuteP: Math.round(totals.lateMinuteP),
      undertimeMinuteP: Math.round(totals.undertimeMinuteP),
      ndp: totals.ndp.toFixed(2),
      ndotp: totals.ndotp.toFixed(2),

      
      rdp: totals.rdp.toFixed(2),
      rdotp: totals.rdotp.toFixed(2),
      rdndp: totals.rdndp.toFixed(2),
      rdndotp: totals.rdndotp.toFixed(2),
      shp: totals.shp.toFixed(2),
      shotp: totals.shotp.toFixed(2),
      shndp: totals.shndp.toFixed(2),
      shndotp: totals.shndotp.toFixed(2),
      shrdp: totals.shrdp.toFixed(2),
      shrdotp: totals.shrdotp.toFixed(2),
      shrdndp: totals.shrdndp.toFixed(2),
      shrdndotp: totals.shrdndotp.toFixed(2),
      lhrdp: totals.lhrdp.toFixed(2),
      lhrdotp: totals.lhrdotp.toFixed(2),
      lhrdndp: totals.lhrdndp.toFixed(2),
      lhrdndotp: totals.lhrdndotp.toFixed(2),
      lhp: totals.lhp.toFixed(2),
      lhotp: totals.lhotp.toFixed(2),
      lhndp: totals.lhndp.toFixed(2),
      lhndotp: totals.lhndotp.toFixed(2),
      lhshp: totals.lhshp.toFixed(2),
      lhshotp: totals.lhshp.toFixed(2),
      lhshndp: totals.lhshp.toFixed(2),
      lhshndotp: totals.lhshp.toFixed(2),

      lhshrdp: totals.lhshrdp.toFixed(2),
      lhshrdotp: totals.lhshrdotp.toFixed(2),
      lhshrdndp: totals.lhshrdndp.toFixed(2),
      lhshrdndotp: totals.lhshrdndotp.toFixed(2),

      hu100p: totals.hu100p.toFixed(2),
      drhp: totals.drhp.toFixed(2),
      drhotp: totals.drhotp.toFixed(2),
      drhndp: totals.drhndp.toFixed(2),
      drhndotp: totals.drhndotp.toFixed(2),

      drhrdp: totals.drhrdp.toFixed(2),
      drhrdotp: totals.drhrdotp.toFixed(2),
      drhrdndp: totals.drhrdndp.toFixed(2),
      drhrdndotp: totals.drhrdndotp.toFixed(2),




    };
  },

parseTimeToMinutes(time) {
  if (!time) return 0;

  if (typeof time === 'object') {
    if ('totalMinutes' in time) {
      return time.totalMinutes;
    }
    if ('hours' in time && 'minutes' in time) {
      return time.hours * 60 + time.minutes;
    }
    return 0;
  }

  if (typeof time === 'string') {
    const [h, m, s] = time.split(':').map(Number);
    return h * 60 + m + (s ? s / 60 : 0);
  }

  return 0;
},

  parseScheduleCode(code) {
    const ranges = code.split('|').map(s => s.trim());
    let totalMinutes = 0;
    let firstStart = null;

    ranges.forEach(range => {
      const [start, end] = range.split('-').map(t => t.trim());
      const startMin = this.parseTimeToMinutes(start);
      const endMin = this.parseTimeToMinutes(end);
      totalMinutes += endMin - startMin;

      if (firstStart === null || startMin < firstStart) {
        firstStart = startMin;
      }
    });

    return { totalScheduledMinutes: totalMinutes, firstStartMinute: firstStart };
  },

  isValidScheduleCode(scheduleCode) {
  if (!scheduleCode || scheduleCode.trim() === '') return false;

  const ranges = scheduleCode.split('|').map(r => r.trim());
  const timeRangeRegex = /^\d{1,2}:\d{2}-\d{1,2}:\d{2}$/; // e.g. "11:00-18:00"

  return ranges.every(range => timeRangeRegex.test(range));
},

isNonEmptyString(str) {
  return typeof str === 'string' && str.trim() !== '';
},

computeND(scheduleCode, scheduleTimeIn, scheduleTimeOut, ndCountMinutes) {
  const toMinutes = (timeStr) => {
    const [h, m] = timeStr.split(':').map(Number);
    return h * 60 + m;
  };

  const ndWindows = [
    { start: 1320, end: 1440 }, // 22:00 - 00:00
    { start: 0, end: 360 }      // 00:00 - 06:00
  ];

  let inMinutes = toMinutes(scheduleTimeIn);
  let outMinutes = toMinutes(scheduleTimeOut);
  if (outMinutes <= inMinutes) outMinutes += 1440;

  const scheduleSegments = scheduleCode
    .split('|')
    .map(s => s.trim())
    .filter(s => s !== 'RESTDAY' && s !== '')
    .map((segment, index, arr) => {
      const [startStr, endStr] = segment.split('-').map(s => s.trim());
      let start = toMinutes(startStr);
      let end = toMinutes(endStr);
      if (end <= start) end += 1440;

      // Shift segments starting before 6AM (not first segment) to next day
      if (index > 0 && start < toMinutes('06:00') && arr.length > 1) {
        start += 1440;
        end += 1440;
      }

      return { start, end };
    });

  let ndMinutes = 0;

  for (const { start: segStart, end: segEnd } of scheduleSegments) {
    const workStart = Math.max(segStart, inMinutes);
    const workEnd = Math.min(segEnd, outMinutes);
    if (workEnd <= workStart) continue;

    // Check ND windows for both days (0–1440 and 1440–2880)
    for (let dayOffset of [0, 1440]) {
      for (const { start: ndStart, end: ndEnd } of ndWindows) {
        const ndStartAdj = ndStart + dayOffset;
        const ndEndAdj = ndEnd + dayOffset;

        const overlapStart = Math.max(workStart, ndStartAdj);
        const overlapEnd = Math.min(workEnd, ndEndAdj);

        if (overlapEnd > overlapStart) {
          ndMinutes += (overlapEnd - overlapStart);
        }
      }
    }
  }

  if (ndCountMinutes > 0) {
    ndMinutes = Math.floor(ndMinutes / ndCountMinutes) * ndCountMinutes;
  }

  return ndMinutes; //+(ndMinutes / 60).toFixed(2);
},




normalizeTime(t) {
  // If time less than 600 (10 AM), consider it next day, add 1440
  return t < 600 ? t + 1440 : t;
},

computeOvertimeND(actualScheduleTimeOut, actualScheduleCode,ndCountMinutes) {
  actualScheduleTimeOut = this.normalizeTime(actualScheduleTimeOut);
  actualScheduleCode = this.normalizeTime(actualScheduleCode);

  if (actualScheduleTimeOut <= actualScheduleCode) return 0;

  const ND_START = 1320; // 22:00
  const ND_END = 1800;   // 06:00 next day (24h + 6h)

  const overtimeStart = actualScheduleCode;
  const overtimeEnd = actualScheduleTimeOut;

  const overlapStart = Math.max(overtimeStart, ND_START);
  const overlapEnd = Math.min(overtimeEnd, ND_END);

  if (overlapEnd <= overlapStart) return 0;

  let overtimeNDMinutes = overlapEnd - overlapStart;

  if (ndCountMinutes > 0) {
    overtimeNDMinutes = Math.floor(overtimeNDMinutes / ndCountMinutes) * ndCountMinutes;
  }



  return overtimeNDMinutes;//+(overtimeNDMinutes / 60).toFixed(2);
},

    onClientPayGroupIdChanged () {
      const me = this;

      me.getClientPayGroupInfo().then(
        (data) => {
          // me.stopWait(wait);
          me.scheduleList = data.scheduleA;
          me.scheduleListA = data.scheduleB;
          me.scheduleListB = data.scheduleC;
          me.scheduleListC = data.scheduleD;
          me.scheduleListD = data.scheduleE;        
          me.holiday = data.holiday; 

        },
        (fault) => {
          // me.stopWait(wait);
          me.showFault(fault);
        }
      );

    },



   addCombo() {
  

  const fields = [
    'timekeepingScheduleId',
    'timekeepingScheduleIdA',
    'timekeepingScheduleIdB',
    'timekeepingScheduleIdC',
    'timekeepingScheduleIdD'
  ];

  const currentField = fields[this.visibleCombos - 1];

  if (this.schedule[currentField]) {
    if (this.visibleCombos < this.maxCombos) {
      this.visibleCombos++;
    }
  } else {
    this.advice.fault("Schedule is required before adding a new one.",{ duration: 5 });
  }
}, 


  removeCombo() {
    if (this.visibleCombos > 1) {
      const fields = [
        "timekeepingScheduleId",
        "timekeepingScheduleIdA",
        "timekeepingScheduleIdB",
        "timekeepingScheduleIdC",
        "timekeepingScheduleIdD",
      ];
      const fieldToClear = fields[this.visibleCombos - 1];
      this.schedule[fieldToClear] = null;

      this.visibleCombos--;
    }
  },

    onClientPayGroupIdChanging (e) {
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId='" + e.proposedValue + "'";
      return getList('dbo.ArsClientPayGroup', 'ClientPayGroupId, ClientPayGroupName', '', filter).then(
        payGroup => {
          if (payGroup && payGroup.length) {
            me.timekeeping.clientPayGroupId = payGroup[0].clientPayGroupId;
            me.timekeeping.clientPayGroupName = payGroup[0].clientPayGroupName;
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

    // onClientPayGroupIdChanged () {
    //   const me = this;

    //   me.loadData();
    //   me.replaceUrl();
    // },

    onClientPayGroupIdLostFocus () {
      const me = this;

      if (!me.timekeeping.clientPayGroupId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onClientPayGroupIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.timekeeping.clientPayGroupId = data.clientPayGroupId;
      me.timekeeping.clientPayGroupName = data.clientPayGroupName;
      me.replaceUrl();
      me.loadData();
    },




    onShowScheduleEditor () {
      const me = this;
      // me.visibleCombos =1;
      let target = 'scheduleMemberId';

      me.setActiveModel('schedule');

      me.setRequiredMode(
        'scheduleDate', 
      );  

      me.setDisplayMode(
        'memberId',
        'memberName',
        'scheduleDate',
      );  

      if (!me.isAddingSchedule) {
        if (me.schedule.lockId) {
          target = 'scheduleDate';
        }
      }

      setTimeout(() => {
        this.setFocus(target);
      }, 200);
    },

    onHideScheduleEditor () {
      const me = this;
     
      me.isAddingSchedule = false;
      me.setActiveModel();
    },

    onEditSchedule (dtl, index) {
      const d = this.schedule;
      this.scheduleIndex = index;

      this.visibleCombos = 1;
      
      dtl = this.core.convertDates(dtl);
      dtl = this.core.convertTimes(dtl);

      if (dtl.scheduleDate === null) {
        dtl.scheduleDate = this.core.emptyDateTime();
      }

      d.scheduleDate = dtl.scheduleDate;
      d.timekeepingSheetId = dtl.timekeepingSheetId;
      d.memberId = dtl.memberId;
      d.memberName = dtl.memberName;
      d.scheduleCode = dtl.scheduleCode;
      d.noScheduleId = dtl.noScheduleId;
      d.dayTypeName = dtl.dayTypeName;
      d.dayTypeNameRef = dtl.dayTypeNameRef;

      d.timekeepingScheduleId = dtl.timekeepingScheduleId;
      d.unpaidBreak = dtl.unpaidBreak;
      d.timekeepingScheduleIdA = dtl.timekeepingScheduleIdA;
      d.timekeepingScheduleIdB = dtl.timekeepingScheduleIdB;
      d.timekeepingScheduleIdC = dtl.timekeepingScheduleIdC;
      d.timekeepingScheduleIdD = dtl.timekeepingScheduleIdD;
      d.scheduleTimeIn = dtl.scheduleTimeIn;
      d.scheduleTimeOut = dtl.scheduleTimeOut;
      d.scheduleDateTimeOut = dtl.scheduleDateTimeOut;

      d.totalWorkingHour = dtl.totalWorkingHour;
      // d.scheduleCode = dtl.scheduleCode;
      d.approvedFormFlag = dtl.approvedFormFlag;
      d.payableToMemberFlag = dtl.payableToMemberFlag;
      d.billableToClientFlag = dtl.billableToClientFlag;
      d.remarks = dtl.remarks;

      this.visibleCombos = 1;

      if (dtl.timekeepingScheduleIdA) this.visibleCombos = 2;
      if (dtl.timekeepingScheduleIdB) this.visibleCombos = 3;
      if (dtl.timekeepingScheduleIdC) this.visibleCombos = 4;
      if (dtl.timekeepingScheduleIdD) this.visibleCombos = 5;
      d.lockId = dtl.lockId;

      this.isScheduleEditorVisible = true;
    },


    onAddSchedule () {
      const me = this;

      me.clearSchedulePad();
      me.schedule.sheetMemberId = -1;
      me.isScheduleEditorVisible = true;
      me.isAddingSchedule = true;
    },

    onDeleteSchedule (index) {
      const me = this;

      if (!me.schedules[index].lockId) {
        me.schedules.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('PayTimekeepingScheduleMember', me.schedules[index].timekeepingSheetId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Member for <b>' + me.schedules[index].timekeepingSheetId + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.schedules.splice(index, 1);
          }
          return;
        })
        .catch( fault => {
          me.showFault(fault);
        });

    },

    onSubmitSchedule () {
      const
        me = this,
        d = me.schedule;
        
      if (!me.isValid('pay0040A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }


      let dtl = {};

      if (me.isAddingSchedule) {

        Object.assign(dtl, d);
                
        me.schedules.push(dtl);
        dtl.timekeepingSheetId = me.timekeeping.timekeepingSheetId;
        me.clearSchedulePad();
        me.advice.info("Schedule for '" + dtl.scheduleDate + "' added to list.", { duration: 5 });
        me.setFocus('startDate');
      
      } else {
        dtl = me.schedules[me.scheduleIndex];
    
        dtl.memberId = d.memberId;
        dtl.memberName = d.memberName;
        dtl.timekeepingSheetId = d.timekeepingSheetId;
        dtl.scheduleDate = d.scheduleDate;
        dtl.noScheduleId = d.noScheduleId;
        dtl.dayTypeName = d.dayTypeName;

        dtl.timekeepingScheduleId = d.timekeepingScheduleId;
        dtl.timekeepingScheduleIdA = d.timekeepingScheduleIdA;
        dtl.timekeepingScheduleIdB = d.timekeepingScheduleIdB;
        dtl.timekeepingScheduleIdC = d.timekeepingScheduleIdC;
        dtl.timekeepingScheduleIdD = d.timekeepingScheduleIdD;

        //Unpaid Breaks

        let unpaidBreak = 0;

        me.scheduleList.forEach((dtl) => {          
          if (dtl.timekeepingScheduleId == d.timekeepingScheduleId) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });

        me.scheduleListA.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdA == d.timekeepingScheduleIdA) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListB.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdB == d.timekeepingScheduleIdB) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListC.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdC == d.timekeepingScheduleIdC) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        me.scheduleListD.forEach((dtl) => {          
          if (dtl.timekeepingScheduleIdD == d.timekeepingScheduleIdD) {
            unpaidBreak = unpaidBreak + dtl.unpaidBreak;
          }
        });
      
        dtl.unpaidBreak = unpaidBreak;
      
      
        dtl.scheduleTimeIn = d.scheduleTimeIn;
        dtl.scheduleTimeOut = d.scheduleTimeOut;
        dtl.scheduleDateTimeOut = d.scheduleDateTimeOut;
    
        dtl.approvedFormFlag = d.approvedFormFlag;
        dtl.payableToMemberFlag = d.payableToMemberFlag;
        dtl.billableToClientFlag = d.billableToClientFlag;
        dtl.remarks = d.remarks;

        dtl.scheduleCode = d.scheduleCode;

        dtl.totalWorkingHour = d.totalWorkingHour;

        dtl.lockId = d.lockId;

// Build scheduleCode list carefully
const scheduleNames = [];
let dayTypeNameList = '';

if (d.noScheduleId) {
  const found = me.noSchedules.find(s => s.noScheduleId == d.noScheduleId);
  if (found) dayTypeNameList = found.noScheduleName;
}

        dtl.dayTypeName = d.dayTypeNameRef + ' | ' + dayTypeNameList;


if (d.timekeepingScheduleId) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleId);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdA) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdA);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdB) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdB);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdC) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdC);
  if (found) scheduleNames.push(found.scheduleName);
}

if (d.timekeepingScheduleIdD) {
  const found = me.scheduleList.find(s => s.timekeepingScheduleId == d.timekeepingScheduleIdD);
  if (found) scheduleNames.push(found.scheduleName);
}

dtl.scheduleCode = scheduleNames.join(' | ');


const metrics = this.computeScheduleMetrics(dtl);

dtl.regularHour = metrics.regularHour;
dtl.overtimeHour = metrics.overtimeHour;
dtl.lateMinute = metrics.lateMinute;
dtl.undertimeMinute = metrics.undertimeMinute;

if (this.timekeeping.applyOffSetFlag) {
  this.onApplyRestDayOffSetting();
}

// this.offSetFlag = true;
//   this.timekeeping.applyOffSetFlag =  true;

        me.isScheduleEditorVisible = false;
      }

    },


    getTargetPath() {
      const me = this,
        q = {};

      if (me.timekeeping.timekeepingSheetId) {
        q.timekeepingSheetId = me.timekeeping.timekeepingSheetId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("timekeepingSheetId" in q && me.core.isInteger(q.timekeepingSheetId)) {
       
        me.timekeeping.timekeepingSheetId = parseInt(q.timekeepingSheetId);
      }

     },

    // API calls

    uploadDocuments (files) {
      let q = this.guid();

      return upload('api/timekeeping-output-member/' + this.otMemberId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
    },


    getFileName (fileName, GUID) {
      return get('api/applicant/download-file/' + this.schedules.memberId + '/' + fileName + '/' + GUID);
    },

    guid () {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
      (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
    },

    loadData() {
      const me = this,
        wait = me.wait();
       this.offSetFlag = false;
       me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            // me.scheduleList = data.schedule;
            // me.scheduleListA = data.scheduleA;
            // me.scheduleListB = data.scheduleB;
            // me.scheduleListC = data.scheduleC;
            // me.scheduleListD = data.scheduleD;
            me.payFreq = data.payFreq;
            me.period = data.period;
            me.payrollPeriod = data.payrollPeriod;
            me.noSchedules = data.noSchedules;
          }
          if (me.timekeeping.timekeepingSheetId < 0) {
            return Promise.resolve(null);
          }
          return me.getTimekeeping();
        })
        .then((timekeeping) => {
          me.stopWait(wait);
          if (timekeeping && timekeeping.timekeeping.length) {
            me.setModels(timekeeping);
          } else {
            if (me.timekeeping.timekeepingSheetId > -1) {
              let message = "Timekeeping Sheet ID '<b>" + me.timekeeping.timekeepingSheetId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
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


            me.timekeeping.startDate = me.today;
            me.timekeeping.endDate = me.today;
            
          }

          me.setupControls();

          me.isFilled = true;
        })
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    },

    onViewLog(log) {
      const me = this,
        wait = me.wait();

        me.getChangeLog(log).then(
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


    setModels(info) {
      const me = this,
        timekeeping = info.timekeeping[0];
    
      me.timekeeping = me.core.convertDates(timekeeping);
      // me.restDayList = info.restDayList;
      // console.log(info.me.schedules )
      me.schedules = info.schedule;
      me.memberList = info.member;

      me.scheduleList = info.scheduleA;
      me.scheduleListA = info.scheduleB;
      me.scheduleListB = info.scheduleC;
      me.scheduleListC = info.scheduleD;
      me.scheduleListD = info.scheduleE;
      me.lateMatrix = info.lateMatrix;
      // console.log('LateMatrix', me.lateMatrix)
    //  console.log(me.timekeeping)
      // console.log(me.schedules)
// me.schedules = this.schedules.map(item => ({
//   ...item,
//   ...this.computeScheduleMetrics(item)
// }));


      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldSchedules = JSON.stringify(me.schedules);
    },

    getClientPayGroupInfo() {
      const me = this;
      return get("api/timekeeping-outputs-client-pay-group-id/" + me.timekeeping.clientPayGroupId );
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingSheetId < 0) {
        return Promise.resolve(null);
      }

      return get("api/timekeeping-outputs/" + this.timekeeping.timekeepingSheetId);
    },

    getReferences() {
      const me = this;
      if (me.schedules.length) {
        //
        // just return True to indicate presence of cached data
        //
        return Promise.resolve(true);
      }

      return get("api/references/pay0040");
    },

    getChangeLog(log) {
      return get("api/timekeeping-outputs/" + log + "/" +  this.timekeeping.timekeepingSheetId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/timekeeping-outputs/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeeping-outputs/" + this.timekeeping.timekeepingSheetId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeeping-outputs/" + this.timekeeping.timekeepingSheetId + "/" + timekeeping.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        timekeeping = {};

      Object.assign(timekeeping, me.timekeeping);
      timekeeping.schedules = me.schedules;
     console.log("XXXXXXXX",me.schedules);
      return timekeeping;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "timekeeping","schedules","schedule",

      );
      dc.keyField = "timekeepingSheetId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
      (this.docs = []),
      this.isCancelling = false;

      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onTimekeepingSheetIdChanging(e) {
      e.callback = this.timekeepingSheetIdCallback;
    },

    timekeepingSheetIdCallback(e) {
      e.message = "Timekeeping Sheet ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.PayTimekeepingSheet", "timekeepingSheetId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onTimekeepingSheetIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onTimekeepingSheetIdLostFocus() {
      const me = this;

      if (!me.timekeeping.timekeepingSheetId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingSheetIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.timekeeping.timekeepingSheetId = data.timekeepingSheetId;
      me.loadData();
      me.replaceUrl();

    },

//

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
              me.timekeeping.timekeepingSheetId = success;
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

        // me.setRequiredMode(
        //   "clientPayGroupId",
        //   "payFreqId",
        //   "periodId",
        //   "yearId",
        //   "payrollPeriodId",
        //   "startDate",
        //   "endDate",
        //   "cutOffDate",
        //   "cutOffStartDate",
        //   "cutOffEndDate",
        // );

        me.setDisplayMode(
          "clientPayGroupName",
          "clientPayGroupId",
          "payFreqId",
          "periodId",
          "yearId",
          "payrollPeriodId",
          "startDate",
          "endDate",
          "cutOffDate",
          "cutOffStartDate",
          "cutOffEndDate",
          "remarks"

        );

        me.setFocus("clientPayGroupId");
      }, 100);
    },

    hasChanges() {
      const me = this;
      
       // 05 Feb 2025 - EMT
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.timekeeping) !== me.oldTimekeeping) {return true;}
      if (JSON.stringify(me.schedules) !== me.oldSchedules) { return true; }

      return false;

    },

    clearSchedulePad () {
      const d = this.schedule;

      d.memberId = 0;
      d.memberName = '';
      d.scheduleDate = null;
      d.noScheduleId = 0;
      d.dayTypeName = '';
      d.dayTypeNameRef = '';
      d.timekeepingScheduleId = 0;
      d.timekeepingScheduleIdA = 0;
      d.timekeepingScheduleIdB = 0;
      d.timekeepingScheduleIdC = 0;
      d.timekeepingScheduleIdD = 0;
      d.unpaidBreak = 0;
      d.scheduleTimeIn = null;
      d.scheduleTimeOut = null;
      d.scheduleDateTimeOut = null;
      d.totalWorkingHour = null;
      d.approvedFormFlag = false;
      d.payableToMemberFlag = false;
      d.billableToClientFlag = false;
      d.remarks = '';
      d.lockId = '';
      this.visibleCombos= 1;
    },


    clearManualSchedulePad () {
      const d = this.schedule;

      d.memberId = 0;
      d.memberName = '';
      d.scheduleDate = null;
      d.noScheduleId = 0;
      d.dayTypeName = '';
      d.dayTypeNameRef = '';
      d.timekeepingScheduleId = 0;
      d.timekeepingScheduleIdA = 0;
      d.timekeepingScheduleIdB = 0;
      d.timekeepingScheduleIdC = 0;
      d.timekeepingScheduleIdD = 0;
      d.unpaidBreak = 0;
      d.scheduleTimeIn = null;
      d.scheduleTimeOut = null;
      d.scheduleDateTimeOut = null;
      d.totalWorkingHour = null;
      d.approvedFormFlag = false;
      d.payableToMemberFlag = false;
      d.billableToClientFlag = false;
      d.remarks = '';
      d.regularHourP = 0;
      d.overtimeHourP = 0;
      d.ndp = 0;
      d.ndotp = 0;
      d.lateMinuteP = 0;
      d.undertimeMinuteP = 0;

      d.rdp = 0;
      d.rdotp = 0;
      d.rdndp = 0;
      d.rdndotp = 0;

      d.shp = 0;
      d.shotp = 0;
      d.shndp = 0;
      d.shndotp = 0;

      d.shrdp = 0;
      d.shrdotp = 0;
      d.shrdndp = 0;
      d.shrdndotp = 0;

      d.lhrdp = 0;
      d.lhrdotp = 0;
      d.lhrdndp = 0;
      d.lhrdndotp = 0;

      d.lhp = 0;
      d.lhotp = 0;
      d.lhndp = 0;
      d.lhndotp = 0;

      d.lhshp = 0;
      d.lhshotp = 0;
      d.lhshndp = 0;
      d.lhshndotp = 0;

      d.lhshrdp = 0;
      d.lhshrdotp = 0;
      d.lhshrdndp = 0;
      d.lhshrdndotp = 0;

      d.hu100p = 0;

      d.drhp = 0;
      d.drhotp = 0;
      d.drhndp = 0;
      d.drhndotp = 0;

      d.drhrdp = 0;
      d.drhrdotp = 0;
      d.drhrdndp = 0;
      d.drhrdndotp = 0;

      d.lockId = '';
      this.visibleCombos= 1;
    },


  },

  created () {
    const me = this;
    
    me.oldTimekeeping = '';
    me.oldSchedules = '';
    me.noSchedules = []; 
    me.scheduleList = []; 
    me.scheduleListA = []; 
    me.scheduleListB = []; 
    me.scheduleListC = []; 
    me.scheduleListD = []; 
    me.lateMatrix = []; 
    me.holiday = []; 
    me.payFreq = []; 
    me.period = []; 
    me.payrollPeriod = []; 
    me.memberList = [];
    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {
    
    editorDocTitle() {
      if (this.isAddingDoc) {
        return 'Add OT Form Detail';
      }
      return 'OT Form Detail';
    },

    filteredGroupedSchedules() {
      if (!this.searchMemberName) return this.groupedSchedules;
      const searchTerm = this.searchMemberName.toLowerCase();
      return this.groupedSchedules.filter(group =>
        group.memberName.toLowerCase().includes(searchTerm)
      );
    },

    groupedSchedules() {
    const grouped = {};
    this.schedules.forEach((item, i) => {
      const key = item.memberId;
      if (!grouped[key]) {
        grouped[key] = {
          memberId: item.memberId,
          memberName: item.memberName,
          departmentName: item.departmentName,
          otFileName: item.otFileName,
          otRemarks: item.otRemarks,
          fileUrl: item.fileUrl,
          entries: []
        };
      }

      // Assign original index for use in onEditSchedule
      grouped[key].entries.push({ ...item, flatIndex: i });
    });

    return Object.values(grouped);
  },

  // groupedSchedules() {
  //     const groups = {};

  //     this.schedules.forEach((item) => {
  //       if (!groups[item.memberId]) {
  //         groups[item.memberId] = {
  //           memberId: item.memberId,
  //           memberName: item.memberName,
  //           departmentName: item.departmentName,
  //           entries: [],
  //         };
  //       }
  //       groups[item.memberId].entries.push(item);
  //     });

  //     return Object.values(groups);
  //   },

    isSchedule () {
      return this.schedules.length;
    },

    detailTag () {
      return 'Schedules: ' + this.schedules.length;
    },

  editorTitle () {
      if (this.isAddingSchedule) {
        return 'Add Time-In & Time-Out';
      }
      return 'Edit Time-In & Time-Out';
    },

 uniqueDates() {
    const dates = this.schedules.map(s => s.scheduleDate);
    return [...new Set(dates)].sort();
  },
  pivotedData() {
    const map = new Map();

    this.schedules.forEach(s => {
      if (!map.has(s.memberId)) {
        map.set(s.memberId, {
          memberId: s.memberId,
          memberName: s.memberName,
          schedules: {}
        });
      }

      let range = '';
      // range = '';//s.scheduleCode + '|' + s.scheduleCodeA + '|' +  s.scheduleCodeB + '|' +  s.scheduleCodeC + '|' +  s.scheduleCodeD 
      if (s.timekeepingScheduleId !== 0 ) range = s.scheduleCode;
      if (s.timekeepingScheduleIdA !== 0) range = range + "|" + s.scheduleCodeA;
      if (s.timekeepingScheduleIdB !== 0 || s.timekeepingScheduleIdB !== 0 ) range = range + "|" + s.scheduleCodeB;
      if (s.timekeepingScheduleIdC !== 0 || s.timekeepingScheduleIdC !== 0 ) range = range + "|" + s.scheduleCodeC;
      if (s.timekeepingScheduleIdD !== 0 || s.timekeepingScheduleIdD !== 0 ) range = range + "|" + s.scheduleCodeD;
      
      map.get(s.memberId).schedules[s.scheduleDate] = {
        id: '',
        range: range
      };
    });

    return Array.from(map.values());
  }

//  uniqueDates() {
//     const dates = [...new Set(this.schedules.map(d => d.scheduleDate))];
//     return dates.sort(); // Optional: sort dates
//   },
//   pivotedData() {
//     const map = new Map();

//     this.schedules.forEach(entry => {
//       if (!map.has(entry.memberId)) {
//         map.set(entry.memberId, {
//           memberId: entry.memberId,
//           memberName: entry.memberName,
//           schedules: {}
//         });
//       }
//       map.get(entry.memberId).schedules[entry.scheduleDate] = entry.schedule;
//     });

//     return Array.from(map.values());
//   }



  },
};
</script>

<!-- <style scoped>
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: 1fr 1.2fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}

.box-3 {
  display: grid;
  
  grid-template-columns: .1fr .1fr .15fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-rows: 1fr;
}

.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}

.box-6 {
  display: flex;
  /* grid-template-columns: 1fr .5fr  ; */
}

.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
   max-height: 10%;
  
}
.table-scroll-wrapper {
  overflow: auto;
  /* height: 45vh; */
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
thead{
  position: sticky;
  top:0;
  z-index: 20;

}

thead{
  position: sticky;
  left: 0;
  z-index: 20;
  background-color: #cee0eb;
  border-right: 2px solid grey;
  width: 8rem;
}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 20;
  background-color: #cee0eb;
  border-right: 2px solid grey;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 20;
  background-color: #cee0eb;
  border-right: 2px solid grey;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 227px;
  z-index: 20;
  background-color: #cee0eb;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 485px;
  z-index: 20;
  background-color: #cee0eb;
  border-right: 2px solid grey;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 20;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 20;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 227px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 485px;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 20;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
    margin: 0;
  }
  .label{
    font-weight: bold;
    margin-right: 55px;

  }
  .label-2{
    font-weight: bold;
    margin-right: 25px;

  }
  .label-3{
    font-weight: bold;
    margin-right: 50px;

  }
  .label-4{
    font-weight: bold;
    margin-right: 30px;

  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row; 
  margin-left: 8rem; 
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
  padding: 10px;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}

@media(max-width: 2000px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr  1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
  
  grid-template-columns: .1fr .1fr .15fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-rows: 1fr;
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
  max-height: 20%;
  
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
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
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
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  
  background-color: #FFFFE0;
   border-right: 1px solid grey;
  z-index: 2;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
   border-right: 1px solid grey;
  z-index: 2;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid #005fa3;
  width: 15rem;
}

tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
 border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
 border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 0;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}

}
@media(max-width: 1700px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr .8fr 1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
  
  grid-template-columns: .1fr .1fr 1fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-rows: 1fr;
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
  height: 45vh;
  
}
.table-scroll-wrapper {
  width: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; /* enough width to allow scroll */
  table-layout: fixed;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
thead{
  position: sticky;
  top:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}


}
@media(max-width: 1400px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr .8fr 1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
  
  grid-template-columns: .1fr .1fr .5fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-rows: 1fr;
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
  height: 49vh;
  
}
.table-scroll-wrapper {
  width: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}


.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; /* enough width to allow scroll */
  table-layout: fixed;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
thead{
  position: sticky;
  top:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}


}
</style> -->
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

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr .8fr 1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
   /* border: 1px solid black; */
  grid-template-columns: .1fr .1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
  /* border: 1px solid black; */
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-6 {
  display: flex;
  flex-direction: row;
  /* grid-template-columns: 1fr 1fr; */
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  /* overflow: auto;
  height: 45vh;
   */
}
.table-scroll-wrapper {
  width: 100%;
    max-height: 50vh;
  height: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; /* enough width to allow scroll */
  table-layout: fixed;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
thead{
  position: sticky;
  top:0;
  z-index: 20;

}
tfoot{
  position: sticky;
  bottom:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}
.box-6{
  position: sticky;
  top: 0;
  z-index: 50;
 background-color: rgba(219, 215, 215, 1);
 display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  border: 1px solid rgba(190, 190, 190, 0.918);
 
  padding: 5px;
  border-radius: 10px;
}
.box-main-container{
  padding: 5px;
  
}
.time-container-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
}
.time-container-1{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr ;
}
@media(max-width: 2000px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr  1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
   /* border: 1px solid black; */
  grid-template-columns: .1fr .1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
  /* border: 1px solid black; */
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
  max-height: 20%;
  
}
.table-scroll-wrapper {
  width: 100%;
    max-height: 50vh;
  height: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
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
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
  /* gap: .5rem; */
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
    z-index: 20;
  background-color: #FFFFE0;
   border-right: 1px solid grey;
  z-index: 2;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
   border-right: 1px solid grey;
     background-color: #FFFFE0;
    z-index: 10;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid #005fa3;
  width: 15rem;
}

tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
  gap: .5rem;
  border: 1px solid black
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
 border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 0;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 1px solid grey;
  z-index: 2;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}
.time-container-1{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr ;
}
.time-container-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
}
}
@media(max-width: 1700px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr .8fr 1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
   /* border: 1px solid black; */
  grid-template-columns: .1fr .1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
  /* border: 1px solid black; */
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-6 {
  display: flex;
  flex-direction: row;
  /* grid-template-columns: 1fr 1fr; */
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.time-container-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
}
.time-container-1{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr ;
}
.table-scroll-wrapper {
  width: 100%;
  max-height: 50vh;
  height: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; /* enough width to allow scroll */
  table-layout: fixed;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
thead{
  position: sticky;
  top:0;
  z-index: 20;

}
tfoot{
  position: sticky;
  bottom:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}


}
@media(max-width: 1400px){
  input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.action-buttons{
display: grid;
grid-template-columns: 1fr 1fr 1fr;
}

.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
 
}

.applicantid-info {
  display:grid;
  grid-template-columns: .2fr .8fr 1.5fr;
  width: 100%;
  gap: .5rem;
}
.box-field {
display: grid;
grid-template-rows:.5fr ;
gap: .5rem;
}
.box-1 {
  display: grid;
  grid-template-columns: .5fr 2fr .5fr .5fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.2fr .5fr  ;
}
.box-3 {
  display: grid;
   /* border: 1px solid black; */
  grid-template-columns: .1fr .1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-4 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
  /* border: 1px solid black; */
}
.box-5 {
  display: grid;
  grid-template-columns: .3fr 1fr;
  gap: .5rem;
}
.box-edit-1{
  display: grid;
  grid-template-columns: .7fr 2fr .7fr ;
  gap: .5rem;
}
.box-edit-2{
  display: grid;
  grid-template-columns: .8fr .8fr 1.8fr ;
  gap: .5rem;
}
.main-scroll-wrapper {
  overflow: auto;
  height: 49vh;
  
}
.table-scroll-wrapper {
  width: 100%;
  max-height: 50vh;
  height: 100%;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}


.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; /* enough width to allow scroll */
  table-layout: fixed;
}

.table-scroll tbody tr:hover {
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
thead{
  position: fixed;
  top:0;
  z-index: 20;

}
thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid red;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
thead th:nth-child(3) {
  position: sticky;
  left: 230px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 20rem;
}
thead th:nth-child(4) {
  position: sticky;
  left: 490px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 15rem;
}
tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(2) {
  position: sticky;
  left: 100px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(3) {
  position: sticky;
  left: 230px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
tbody td:nth-child(4) {
  position: sticky;
  left: 490px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 10;
  width: 25rem;
  text-align: left;
}
   
    
  .label-row {
    display: flex;
    margin-bottom: 4px;
  }

  .label-row strong {
    width: 150px; /* fixed width for label column */
    font-weight: bold;
  }

  .label-value {
    flex: 1;
  }
  .upload-files{
  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
.btns{
  display: flex;
  flex-direction: row;  
  gap: 1rem;
}
.payable{
  width: 10rem;
  text-align: center;
}
.billable{
  width: 13rem;
  text-align: center;
}
.remarks{
  width: 15rem;
}
.edit-btns{
  display: flex;
  justify-content: space-between;
}
.text-info-container{
  display: grid;
  grid-template-columns: .6fr 1fr;
}
.filter-container{
  display: grid;
  grid-template-columns: 1fr .2fr;
  margin-top: 5px;
  /* border: 1px solid black; */
  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}


}
</style>