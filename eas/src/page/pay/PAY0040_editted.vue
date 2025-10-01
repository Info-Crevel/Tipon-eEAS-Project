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
             <button type="button" :class="logButtonClass" class="applicant-btn w-10 secondary" @click="onCalculate" v-show="timekeeping.manualUploadFlag==0"> 
              <i class="fa fa-calculator"></i><span class="bold">Compute </span> 
            </button>

            <button type="button" :class="logButtonClass" class="applicant-btn w-10 info" @click="onApplyOffset" v-show="timekeeping.manualUploadFlag==0"> 
              <i class="fa fa-calculator"></i><span class="bold">Apply Off Set </span> 
            </button>              

            <button type="button" :class="logButtonClass" class="applicant-btn w-10 success-light" @click="onUploadManual"> 
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
            <table v-show = "!timekeeping.applyOffSetFlag" class="table-scroll striped-even mb-0"  ref="table" id="loremTable" summary="lorem ipsum sit amet" rules="groups" frame="hsides" border="2" :style="getTableWrapperStyle(group.entries.length)">
   
              <thead class="info-light">
                <tr>
                  <th>Action</th>
                  <th>Date</th>
                  <th>Day Type</th>
                  <th>Schedule</th>
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
                  <th class="text-center">LHSHOT</th>
                  <th class="text-center">LHSHND</th>
                  <th class="text-center">LHSHNDOT</th>
                  <th class="text-center">LHSHRD</th>
                  <th class="text-center">LHSHRDOT</th>
                  <th class="text-center">LHSHRDND</th>
                  <th class="text-center">LHSHRDNDOT</th>
                  <th class="text-center">HU-100</th>
                  <th class="text-center">DRH</th>
                  <th class="text-center">DRHOT</th>
                  <th class="text-center">DRHND</th>
                  <th class="text-center">DRHNDOT</th>
                  <th class="text-center">DRHRD</th>
                  <th class="text-center">DRHRDOT</th>
                  <th class="text-center">DRHRDND</th>
                  <th class="text-center">DRHRDNDOT</th>


                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(dtl, index) in group.entries"
                  :key="`${group.memberId}-${index}`" :class="getNoDataColor(dtl.scheduleCode, dtl.scheduleTimeIn, dtl.scheduleTimeOut )"
                >
                  <td>
                    <button
                      @click="onEditSchedule(dtl, dtl.flatIndex)"
                      class="btn-dtl-edit info"
                      type="button"
                    >
                      <i class="fa fa-edit"></i> Edit
                    </button>
                  </td>
                  <td>{{ dtl.scheduleDate }}</td>
                  <td :class="{ 'bold italic': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
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
            <table v-show = "timekeeping.applyOffSetFlag" class="table-scroll striped-even mb-0"  ref="table" id="loremTable" summary="lorem ipsum sit amet" rules="groups" frame="hsides" border="2" :style="getTableWrapperStyle(group.entries.length)">
              <thead class="info-light">
                <tr>
                  <th>Action</th>
                  <th>Date</th>
                  <th>Day Type</th>
                  <th>Schedule</th>
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
                  <th class="text-center">LHSHOT</th>
                  <th class="text-center">LHSHND</th>
                  <th class="text-center">LHSHNDOT</th>
                  <th class="text-center">LHSHRD</th>
                  <th class="text-center">LHSHRDOT</th>
                  <th class="text-center">LHSHRDND</th>
                  <th class="text-center">LHSHRDNDOT</th>
                  <th class="text-center">HU-100</th>
                  <th class="text-center">DRH</th>
                  <th class="text-center">DRHOT</th>
                  <th class="text-center">DRHND</th>
                  <th class="text-center">DRHNDOT</th>
                  <th class="text-center">DRHRD</th>
                  <th class="text-center">DRHRDOT</th>
                  <th class="text-center">DRHRDND</th>
                  <th class="text-center">DRHRDNDOT</th>

                

                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(dtl, index) in group.entries"
                  :key="`${group.memberId}-${index}`" :class="getNoDataColor(dtl.scheduleCode, dtl.scheduleTimeIn, dtl.scheduleTimeOut )"
                >
                  <td>
                    <button
                     :disabled="timekeeping.manualUploadFlag==1"
                      @click="onEditSchedule(dtl, dtl.flatIndex)"
                      class="btn-dtl-edit info"
                      type="button"
                    >
                      <i class="fa fa-edit"></i> Edit
                    </button>
                  </td>
                  <td>{{ dtl.scheduleDate }}</td>
                  <td :class="{ 'bold italic': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
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
                  <td class="text-center">{{ dtl.ndP }}</td>
                  <td class="text-center">{{ dtl.ndotP }}</td>
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
            <td class="text-center">{{ getGroupTotals(group.entries).ndP }}</td>
            <td class="text-center">{{ getGroupTotals(group.entries).ndotP }}</td>

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
        <sym-check v-model="schedule.payableToMemberFlag" caption="Payable To Member" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-check v-model="schedule.billableToClientFlag" caption="Billable To Client" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check> 
        <sym-text v-model="schedule.remarks" caption="Remarks" align="bottom"></sym-text>
      </div>
      <div class="box-5">

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
      </div>
      
     
    </div>  
    <div class="edit-btns">

    
    <div class="buttons w-100 justify-left " fw-26 shadow-soft mb-0>
      <button @click="addCombo" :disabled="visibleCombos >= maxCombos" class="success-light w-40"><i class="fa fa-plus mr-2"></i>Add Schedule</button>
      <button @click="removeCombo" :disabled="visibleCombos <= 1" class="danger-light"><i class="fa fa-minus mr-2"></i>Remove</button>
    </div>
        <div class="buttons w-100 justify-end  m" fw-26 shadow-soft mb-0>
      
      <button type="button" class="info justify-between border-main" @click="onSubmitSchedule()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isScheduleEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>
  </div>

  </form>  
</div>

</sym-modal>

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
              <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
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
import "jspdf-autotable";
import { saveAs } from "file-saver";
import ExcelJS from 'exceljs';


import {
  get,
  ajax,
  upload, 
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
        startDate: null,
        endDate: null,
        cutOffDate: null,
        cutOffStartDate: null,
        cutOffEndDate: null,
        remarks: '',
        lateCount:0,
	      lateGracePeriodCount:0,

        minimumRequiredOvertimeCount:0,
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
        underTimeDurationPerCount: 0,
        lockId: "",
      },

      oldTimekeeping: [],
      logs: [],
      isLogVisible: false,

      memberRequest: [],

      schedules: [],

      isScheduleEditorVisible: false,

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
        ndP: 0,
        ndotP: 0,
        rdP: 0,
        rdotP: 0,
        rdndP: 0,
        rdndotP: 0,
        shP: 0,
        shotP: 0,
        shndP: 0,
        shndotP: 0,
        shrdP: 0,
        shrdotP: 0,
        shrdndP: 0,
        shrdndotP: 0,
        lhrdP: 0,
        lhrdotP: 0,
        lhrdndP: 0,
        lhrdndotP: 0,
        lhP: 0,
        lhotP: 0, 
        lhndP: 0,
        lhndotP: 0,

        lhshP: 0,
        lhshotP: 0,
        lhshndP: 0,
        lhshndotP: 0,

        lhshrdP: 0,
        lhshrdotP: 0,
        lhshrdndP: 0,
        lhshrdndotP: 0,

        hu100P: 0,
        drhP: 0,
        drhotP: 0,
        drhndP: 0,
        drhndotP: 0,

        drhrdp: 0,
        drhrdotP: 0,
        drhrdndP: 0,
        drhrdndotP: 0,


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
      excelData: [],
      isReading: false,
    visibleCombos: 1, 
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
      ],

     isAddingDoc: false,
      isDocEditorVisible: false,
      docIndex: -1,

      fileName: "",
      isUploadRequired: false,
      pathFileName: "",
      guidReference: "",

      otMemberId:0,

  };



  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

computeRegularHour(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut,unpaidBreak) {

  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; 


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


computeUndertime(scheduleCode, scheduleTimeIn, scheduleTimeOut,scheduleDate,scheduleDateTimeOut) {
  
let scheduleStartTime = this.getFirstTimeInSchedule(scheduleCode);

  const blocks = scheduleCode.split('|').map(b => b.trim()).filter(b => b.length > 0);
  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);

  const timeInDate = this.parseDateTime(isoDateIn, scheduleStartTime);
  const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

  if (!timeInDate || !timeOutDate) {
    return { undertimeMinute: 0 };
  }

  const timeInMs = timeInDate.getTime();
  const timeOutMs = timeOutDate.getTime();

  let scheduledMs = 0;
  let workedWithinScheduleMs = 0;

  blocks.forEach((block, index) => {
    const [startStr, endStr] = block.split('-').map(s => s.trim());
    if (!startStr || !endStr) {
      return;
    }
    let blockStart = this.parseDateTime(isoDateIn, startStr);
    let blockEnd = this.parseDateTime(isoDateIn, endStr);
    if (blockEnd <= blockStart) {
      blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
    }
    if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
      blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
      blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
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

  const minimumRequiredOvertimeCount = this.timekeeping.minimumRequiredOvertimeCount; 
  const overtimeDurationCount = this.timekeeping.overtimeDurationCount;

  if (!timeOutDate) {

    return 0;
  }

  const lastBlock = blocks[blocks.length - 1];
  const [startStr, endStr] = lastBlock.split('-').map(s => s.trim());

  let lastBlockStart = this.parseDateTime(isoDateIn, startStr);
  let lastBlockEnd = this.parseDateTime(isoDateIn, endStr);

  if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {

    lastBlockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
    lastBlockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
  }

  if (lastBlockEnd <= lastBlockStart) {
    lastBlockEnd = new Date(lastBlockEnd.getTime() + 24 * 60 * 60 * 1000); 
  }

  const overtimeMs = timeOutDate.getTime() - lastBlockEnd.getTime();
  const overtimeHour = Math.max(0, overtimeMs / (1000 * 60 * 60));
  let overtimeMinutes = Math.max(0, overtimeMs / 60000);


  if (overtimeMinutes >= minimumRequiredOvertimeCount) {
    overtimeMinutes = Math.floor(overtimeMinutes / overtimeDurationCount) * overtimeDurationCount;
  } else {
    overtimeMinutes = 0;
  }
 let overtimeHours = overtimeMinutes / 60;

  return parseFloat(overtimeHours.toFixed(2));
},


computeNightDifferential(scheduleCode, scheduleTimeIn, scheduleTimeOut, scheduleDate, scheduleDateTimeOut) {
  const ND_START = '22:00';
  const ND_END = '06:00';
  const MAX_REGULAR_MINUTES = this.timekeeping.maximumRegularHourCount * 60; 

  const isoDateIn = this.toISODate(scheduleDate);
  const isoDateOut = this.toISODate(scheduleDateTimeOut);

  const timeInDate = this.parseDateTime(isoDateIn, scheduleTimeIn);
  const timeOutDate = this.parseDateTime(isoDateOut, scheduleTimeOut);

  if (!timeInDate || !timeOutDate) return 0;

  const ndStart = this.parseDateTime(isoDateIn, ND_START);
  let ndEnd = this.parseDateTime(isoDateIn, ND_END);
  if (ndEnd <= ndStart) {
    ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000); 
  }

  const blocks = scheduleCode
    .split('|')
    .map(b => b.trim())
    .filter(b => b.length > 0)
    .map(block => {
      let [startStr, endStr] = block.split('-').map(s => s.trim());

      let blockStart = this.parseDateTime(isoDateIn, startStr);
      let blockEnd = this.parseDateTime(isoDateIn, endStr);

      if (blockEnd <= blockStart) {
        blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
      }
      if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
        blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
        blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
        if (blockEnd <= blockStart) {
          blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
        }
      }

      return { start: blockStart, end: blockEnd };
    })
    .sort((a, b) => a.start - b.start);

  let totalScheduledMs = 0;
  let totalNdMs = 0;

  for (const block of blocks) {
    if (block.end <= timeInDate || block.start >= timeOutDate) continue; 
    const actualStart = new Date(Math.max(timeInDate.getTime(), block.start.getTime()));
    const actualEnd = new Date(Math.min(timeOutDate.getTime(), block.end.getTime()));

    let durationMs = actualEnd - actualStart;
    if (durationMs <= 0) continue;
    const remainingMs = (MAX_REGULAR_MINUTES * 60000) - totalScheduledMs;
    if (remainingMs <= 0) break;

    if (durationMs > remainingMs) {
      durationMs = remainingMs;
      actualEnd.setTime(actualStart.getTime() + durationMs);
    }

    totalScheduledMs += durationMs;
    const ndOverlapStart = Math.max(actualStart.getTime(), ndStart.getTime());
    const ndOverlapEnd = Math.min(actualEnd.getTime(), ndEnd.getTime());

    if (ndOverlapEnd > ndOverlapStart) {
      totalNdMs += ndOverlapEnd - ndOverlapStart;
    }
  }

  return Math.round(totalNdMs / 60000); 
},

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

  const blockIntervals = blocks.map(block => {
    let [startStr, endStr] = block.split('-').map(s => s.trim());
    let blockStart = this.parseDateTime(isoDateIn, startStr);
    let blockEnd = this.parseDateTime(isoDateIn, endStr);

    if (this.parseTimeToMinutes(endStr) <= this.parseTimeToMinutes(startStr)) {
      blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
    }

    if (this.parseTimeToMinutes(startStr) < this.parseTimeToMinutes('06:00')) {
      blockStart = this.parseDateTime(this.addDays(isoDateIn, 1), startStr);
      blockEnd = this.parseDateTime(this.addDays(isoDateIn, 1), endStr);
      if (blockEnd <= blockStart) {
        blockEnd = new Date(blockEnd.getTime() + 24 * 60 * 60 * 1000);
      }
    }

    return { start: blockStart, end: blockEnd };
  });


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

  if (fullTimeOutDate <= regularEnd) return 0; 

  const otStart = regularEnd;
  const otEnd = fullTimeOutDate;


  const ndStart = this.parseDateTime(isoDateIn, ND_START);
  let ndEnd = this.parseDateTime(isoDateIn, ND_END);
  if (ndEnd <= ndStart) {
    ndEnd = new Date(ndEnd.getTime() + 24 * 60 * 60 * 1000);
  }

  const lastScheduledEnd = blockIntervals.reduce((max, b) => (b.end > max ? b.end : max), new Date(0));

  let totalNdMs = 0;


  blockIntervals.forEach(({ start, end }) => {
    const overlapStart = Math.max(otStart.getTime(), start.getTime(), ndStart.getTime());
    const overlapEnd = Math.min(otEnd.getTime(), end.getTime(), ndEnd.getTime());

    if (overlapEnd > overlapStart) {
      totalNdMs += overlapEnd - overlapStart;
    }
  });


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
    if (typeof dateInput === 'string' && dateInput.includes('-')) {
      const parsed = new Date(dateInput);
      if (!isNaN(parsed.getTime())) {
        return parsed.toISOString().slice(0, 10);
      }
    }
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
    return date.toISOString().slice(0, 10);
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


computeLateMinute(scheduleCode, scheduleTimeIn, scheduleDate) {
  const firstTimeStr = this.getFirstTimeInSchedule(scheduleCode);
  if (!firstTimeStr || !scheduleTimeIn || !scheduleDate) return 0;

  const isoDate = this.toISODate(scheduleDate);
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
  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; 
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

       regularHourP: 0, ndP: 0, overtimeHourP: 0, ndotP: 0,
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

const ndCount = this.timekeeping.ndCount; 
    
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


if (this.timekeeping.constructionFlag) {
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
  hu100 = regularHour;
  nd = overtimeHour;
  overtimeHour = nd;
  ndot = ndot;

  regularHour = 0; 

}

if (noScheduleId==1 && dayTypeCode=='HU-100') {
  hu100 = regularHour;
  nd = overtimeHour;
  overtimeHour = nd;
  ndot = ndot;

   regularHour = 0; 
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

},
generateEmptyMetrics() {
  return {
    lateMinute: 0, undertimeMinute: 0, regularHour: 0, nd: 0, overtimeHour: 0, ndot: 0,
    lh: 0, lhot: 0, lhnd: 0, lhndot: 0,
    sh: 0, shot: 0, shnd: 0, shndot: 0,
    lhsh: 0, lhshot: 0, lhshnd: 0, lhshndot: 0,
    drh: 0, drhot: 0, drhnd: 0, drhndot: 0,
    rd: 0, rdot: 0, rdnd: 0, rdndot: 0,
    regularHourP: 0, ndP: 0, overtimeHourP: 0, ndotP: 0,
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
      timeOut.setDate(timeOut.getDate() + 1); 
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

    regularHourP: row['REGHRS'],
    overtimeHourP: row['OT'],
    ndP: row['ND'],
    ndotP: row['NDOT'],
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
  }
});
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
      newSchedule.regularHourP  = newSchedule.rdp     || 0;
      newSchedule.overtimeHourP = newSchedule.rdotp   || 0;
      newSchedule.ndP           = newSchedule.rdndp   || 0;
      newSchedule.ndotP         = newSchedule.rdndotp || 0;
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


  const lateGracePeriod = this.timekeeping.lateGracePeriodCount;
  const lateCountMinutes = this.timekeeping.lateCount;
  const minimumRequiredOvertimeCount = this.timekeeping.minimumRequiredOvertimeCount;
  const overtimeDurationCount = this.timekeeping.overtimeDurationCount;
  const overtimeCountId = this.timekeeping.overtimeCountId; 
  const maximumRegularHourCount = this.timekeeping.maximumRegularHourCount; 
  const ndCount = this.timekeeping.ndCount; 
  const paidUpHolidayId = this.timekeeping.paidUpHolidayId; 
  const nightDifferentialCountId = this.timekeeping.nightDifferentialCountId; 
  const restDayOffSetFlag = this.timekeeping.restDayOffSetFlag; 
  const restDayConsideration = this.timekeeping.restDayConsideration; 
  const lateCountId = this.timekeeping.lateCountId; 
  const holidayCountId = this.timekeeping.holidayCountId; 
  const undertimeCountId = this.timekeeping.undertimeCountId; 
  const underTimeDurationPerCount=this.timekeeping.underTimeDurationPerCount;
  this.offSetFlag = true;
  this.timekeeping.applyOffSetFlag =  true;
  this.schedules = this.schedules.map(schedule => {
   
    let regularHourP = schedule.regularHour;
    let overtimeHourP = schedule.overtimeHour;
    let ndP = (schedule.nd /60).toFixed(1);
    let ndotP = (schedule.ndot /60).toFixed(1);
    let lateMinuteP = 0;

    if (lateCountId!==1) {
      lateMinuteP = schedule.lateMinute;
    }
    
    let undertimeMinuteP = schedule.undertimeMinute;

    if (undertimeMinuteP>=underTimeDurationPerCount) {
     undertimeMinuteP = Math.floor(undertimeMinuteP / underTimeDurationPerCount) * underTimeDurationPerCount;
    } else {
      undertimeMinuteP = 0;
    }
  const matchedLate = this.lateMatrix.find(matrix => 
    lateMinuteP >= matrix.startMinute && lateMinuteP <= matrix.endMinute
  );

  if (matchedLate) {
    lateMinuteP = matchedLate.lateCount;
  }



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
}



if (this.timekeeping.constructionFlag && regularHourP>0) {

  let  overtimeHour = schedule.regularHour - this.timekeeping.maximumRegularHourCount 
  
  let regularHour = this.timekeeping.maximumRegularHourCount
  
  regularHourP = regularHour


    overtimeHourP =  Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.overtimeHour)  

    if (overtimeHourP<0) {
       regularHourP = regularHourP + overtimeHourP;

      if (regularHourP < maximumRegularHourCount && ((regularHourP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        regularHourP = regularHourP + schedule.unpaidBreak;
      }

      overtimeHourP = 0;

    }


    undertimeMinuteP = 0

    ndP = (schedule.nd /60).toFixed(1) 
}

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
  }


  if (this.timekeeping.constructionFlag && rdP > 0 ) {


    let  overtimeHour = schedule.rd - this.timekeeping.maximumRegularHourCount
    let regularHour = this.timekeeping.maximumRegularHourCount

    rdP = regularHour
    rdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.rdot)  

    if (rdotP<0) {
      rdP = rdP + rdotP;

      if (rdP < maximumRegularHourCount && ((rdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        rdP = rdP + schedule.unpaidBreak;
      }


      rdotP = 0;
    }



    undertimeMinuteP = 0
  
      rdndP = (schedule.rdnd /60).toFixed(1)
    rdndotP = (schedule.rdndot /60).toFixed(1)


}


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
  }

  
  if (this.timekeeping.constructionFlag && shP > 0 ) {

  let  overtimeHour = schedule.sh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount




    shP = regularHour
    shotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.shot)  

    if (shotP<0) {
      shP = shP + shotP;
  
      if (shP < maximumRegularHourCount && ((shP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        shP = shP + schedule.unpaidBreak;
      }

  
      shotP = 0;
    }


    undertimeMinuteP = 0

      shndP = (schedule.shnd /60).toFixed(1) 
    shndotP = (schedule.shndot /60).toFixed(1)
}

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
  }
 
 
  if (this.timekeeping.constructionFlag && shrdP > 0 ) {

  let  overtimeHour = schedule.shrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount

    shrdP = regularHour
    shrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.shrdot) 


    if (shrdotP<0) {
      shrdP = shrdP + shrdotP;

      if (shrdP < maximumRegularHourCount && ((shrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        shrdP = shrdP + schedule.unpaidBreak;
      }


      shrdotP = 0;
    }


    undertimeMinuteP = 0
      shrdndP = (schedule.shrdnd /60).toFixed(1) 
    shrdndotP = (schedule.shrdndot /60).toFixed(1)
}

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
  }

 
  if (this.timekeeping.constructionFlag && lhrdP > 0 ) {

  let  overtimeHour = schedule.lhrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhrdP = regularHour
    lhrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhrdot)   

    if (lhrdotP<0) {
      lhrdP = lhrdP + lhrdotP;

      if (lhrdP < maximumRegularHourCount && ((lhrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhrdP = lhrdP + schedule.unpaidBreak;
      }

      lhrdotP = 0;
    }


    undertimeMinuteP = 0

      lhrdndP = (schedule.lhrdnd /60).toFixed(1)
    lhrdndotP = (schedule.lhrdndot /60).toFixed(1)


    }

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
  }
 
  if (this.timekeeping.constructionFlag && lhP > 0 ) {

  let  overtimeHour = schedule.lh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhP = regularHour
    lhotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhot ) 

    if (lhotP<0) {
      lhP = lhP + lhotP;

      if (lhP < maximumRegularHourCount && ((lhP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhP = lhP + schedule.unpaidBreak;
      }

      lhotP = 0;
    }



    undertimeMinuteP = 0
   lhndP = (schedule.lhnd /60).toFixed(1) 
    lhndotP = (schedule.lhndot /60).toFixed(1)

}

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
  }

  if (this.timekeeping.constructionFlag && drhP > 0 ) {

  let  overtimeHour = schedule.drh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    drhP = regularHour
    drhotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.drhot )  

    if (drhotP<0) {
      drhP = drhP + drhotP;

      if (drhP < maximumRegularHourCount && ((drhP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        drhP = drhP + schedule.unpaidBreak;
      }

      drhotP = 0;
    }


    undertimeMinuteP = 0

    drhndP = (schedule.drhnd /60).toFixed(1)  
    drhndotP = (schedule.drhndot /60).toFixed(1)

}

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
  }


  if (this.timekeeping.constructionFlag && lhshP > 0 ) {

  let  overtimeHour = schedule.lhsh - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhshP = regularHour
    lhshotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.lhshot  )


    if (lhshotP<0) {
      lhshP = lhshP + lhshotP;

      if (lhshP < maximumRegularHourCount && ((lhshP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhshP = lhshP + schedule.unpaidBreak;
      }

      lhshotP = 0;
    }


    undertimeMinuteP = 0

  lhshndP = (schedule.lhshnd /60).toFixed(1)  
    lhshndotP = (schedule.lhshndot /60).toFixed(1)
}

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
  }

  if (this.timekeeping.constructionFlag && lhshrdP > 0 ) {

  let  overtimeHour = schedule.lhshrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    lhshrdP = regularHour
    lhshrdotP = Math.floor((overtimeHour - (lateMinuteP / 60)) - (undertimeMinuteP / 60) + schedule.lhshrdot)   
    

    if (lhshrdotP<0) {
      lhshrdP = lhshrdP + lhshrdotP;

      if (lhshrdP < maximumRegularHourCount && ((lhshrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        lhshrdP = lhshrdP + schedule.unpaidBreak;
      }

      lhshrdotP = 0;
    }


    undertimeMinuteP = 0


  lhshrdndP = (schedule.lhshrdnd /60).toFixed(1)  
    lhshrdndotP = (schedule.lhshrdndot /60).toFixed(1)

}

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
  }


  if (this.timekeeping.constructionFlag && drhrdP > 0 ) {
  let  overtimeHour = schedule.drhrd - this.timekeeping.maximumRegularHourCount
  let regularHour = this.timekeeping.maximumRegularHourCount


    drhrdP = regularHour
    drhrdotP =   Math.floor((overtimeHour - (lateMinuteP/60)) - (undertimeMinuteP/60) + schedule.drhrdot)  



    if (drhrdotP<0) {
      drhrdP = drhrdP + drhrdotP;

      if (drhrdP < maximumRegularHourCount && ((drhrdP + schedule.unpaidBreak) <= maximumRegularHourCount  )) {
        drhrdP = drhrdP + schedule.unpaidBreak;
      }


      drhrdotP = 0;
    }


    undertimeMinuteP = 0

  drhrdndP = (schedule.drhrdnd /60).toFixed(1)  
    drhrdndotP = (schedule.drhrdndot /60).toFixed(1)

}


    if (lateCountId==1) {
      lateMinuteP = schedule.lateMinute;
    }

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



    break;
  }
}


}

  if (this.timekeeping.constructionFlag && lateMinuteP > 0)  {
    lateMinuteP = lateMinuteP/60;
  }


    return {
      ...schedule,
      regularHourP: regularHourP,
      overtimeHourP: overtimeHourP,
      lateMinuteP: lateMinuteP,
      undertimeMinuteP: undertimeMinuteP,
      ndP: ndP,
      ndotP: ndotP,

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
      
      const d = this.schedule;
      this.docIndex = index;
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

        onSubmitDoc() {
      
        const me = this;
        const d = me.schedule;
        if (!me.fileName || me.fileName.trim() === "") {
          me.advice.fault('Upload required. Please select a file.', { duration: 5 });
          return;
        }
let dtl = me.schedules[me.docIndex];

dtl.memberId = d.memberId;
dtl.otRemarks = d.otRemarks;
dtl.otFileName = me.fileName || d.otFileName;

dtl.otGUID = me.guidReference || d.otGUID;
me.schedules.forEach(sch => {

  if (sch.memberId == dtl.memberId) {
    
    sch.otRemarks = dtl.otRemarks;    
    sch.otFileName = dtl.otFileName 
    sch.otGUID = dtl.otGUID;
  }
});

me.isDocEditorVisible = false;
      
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
    },

    onHideDocEditor() {
      const me = this;

      me.isAddingDoc = false;
      me.setActiveModel();
    },


    onAddDoc(memberId) {
      const me = this;

      me.clearDocPad();
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

  const date = new Date(1899, 11, 30, +hours, +minutes); 

  return date;
},

async generateExcelReport() {
  const grouped = {};

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

        }
      };
    }

    grouped[s.memberId].entries.push(s);

    const t = grouped[s.memberId].totals;
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
    group.entries.forEach(entry => {
      const timeIn = entry.scheduleTimeIn
  ? (parseInt(entry.scheduleTimeIn.split(':')[0], 10) / 24 +
     parseInt(entry.scheduleTimeIn.split(':')[1], 10) / 1440)
  : null;

const timeOut = entry.scheduleTimeOut
  ? (parseInt(entry.scheduleTimeOut.split(':')[0], 10) / 24 +
     parseInt(entry.scheduleTimeOut.split(':')[1], 10) / 1440)
  : null;

    const row = worksheet.addRow([
      entry.scheduleDate ? new Date(entry.scheduleDate) : null,
      entry.dayTypeName || '',
      entry.scheduleCode || '',
      timeIn,
      timeOut,
      +entry.regularHour || '',
      +entry.overtimeHour || '',
      +entry.lateMinute || '',
      +entry.undertimeMinute || '',
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

      currentRow++;
    });

    const t = group.totals;
    const totalRow = worksheet.addRow([
      "TOTAL", "", "", "", "",
      t.regularHour.toFixed(2),
      t.overtimeHour.toFixed(2),
      t.lateMinute,
      t.undertimeMinute,
      "", "", "", "",
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

    currentRow += 2;
  }

  
  worksheet.getColumn(1).numFmt = 'mm/dd/yyyy'; 
  worksheet.getColumn(4).numFmt = 'hh:mm';      
  worksheet.getColumn(5).numFmt = 'hh:mm';      

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


  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], { type: 'application/octet-stream' });
  saveAs(blob, 'schedule-report.xlsx');
},



formatDate(dt) {
  if (!dt) return "";

  if (typeof dt.toJSDate === "function") {
    return dt.toJSDate().toLocaleDateString("en-US");
  }

  const d = new Date(dt);
  if (isNaN(d.getTime())) return "";
  return d.toLocaleDateString("en-US");
},

formatTime(time) {
  if (!time) return "";

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
      ndP: 0,
      ndotP: 0,


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
  totals.ndP += parseFloat(dtl.ndP) || 0;
  totals.ndotP += parseFloat(dtl.ndotP) || 0;

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
      ndP: totals.ndP.toFixed(2),
      ndotP: totals.ndotP.toFixed(2),

      
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
  const timeRangeRegex = /^\d{1,2}:\d{2}-\d{1,2}:\d{2}$/; 

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
    { start: 1320, end: 1440 }, 
    { start: 0, end: 360 }      
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

  return ndMinutes; 
},




normalizeTime(t) {

  return t < 600 ? t + 1440 : t;
},

computeOvertimeND(actualScheduleTimeOut, actualScheduleCode,ndCountMinutes) {
  actualScheduleTimeOut = this.normalizeTime(actualScheduleTimeOut);
  actualScheduleCode = this.normalizeTime(actualScheduleCode);

  if (actualScheduleTimeOut <= actualScheduleCode) return 0;

  const ND_START = 1320; 
  const ND_END = 1800;  

  const overtimeStart = actualScheduleCode;
  const overtimeEnd = actualScheduleTimeOut;

  const overlapStart = Math.max(overtimeStart, ND_START);
  const overlapEnd = Math.min(overtimeEnd, ND_END);

  if (overlapEnd <= overlapStart) return 0;

  let overtimeNDMinutes = overlapEnd - overlapStart;

  if (ndCountMinutes > 0) {
    overtimeNDMinutes = Math.floor(overtimeNDMinutes / ndCountMinutes) * ndCountMinutes;
  }



  return overtimeNDMinutes;
},

    onClientPayGroupIdChanged () {
      const me = this;

      me.getClientPayGroupInfo().then(
        (data) => {
       
          me.scheduleList = data.scheduleA;
          me.scheduleListA = data.scheduleB;
          me.scheduleListB = data.scheduleC;
          me.scheduleListC = data.scheduleD;
          me.scheduleListD = data.scheduleE;        
          me.holiday = data.holiday; 

        },
        (fault) => {
        
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
      me.schedules = info.schedule;

      me.memberList = info.member;

      me.scheduleList = info.scheduleA;
      me.scheduleListA = info.scheduleB;
      me.scheduleListB = info.scheduleC;
      me.scheduleListC = info.scheduleD;
      me.scheduleListD = info.scheduleE;
      me.lateMatrix = info.lateMatrix;

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
     
      return timekeeping;
    },
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
      grouped[key].entries.push({ ...item, flatIndex: i });
    });

    return Object.values(grouped);
  },


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
  
  grid-template-columns: .1fr .1fr ;
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
  flex-direction: row;

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
.table-scroll-wrapper {
  width: 100%;
   height: 45vh;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%; 
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
    width: 150px; 
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
  
  grid-template-columns: .1fr .1fr ;
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
    width: 150px;
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
  
  grid-template-columns: .1fr .1fr ;
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
  flex-direction: row;
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
.table-scroll-wrapper {
  width: 100%;
  height: 100%;
  max-height: 50vh;
  overflow-x: auto;
}

.table-scroll-wrapper table {
  width: 250%;
  border-collapse: collapse;
}

.table-scroll {
  border-collapse: collapse;
  width: 250%;
  min-width: 100%;
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
    width: 150px;
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
  
  grid-template-columns: .1fr .1fr ;
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
  min-width: 100%; 
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
    width: 150px; 
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

  margin-bottom: 0;

}
.upload-btn{
 margin-bottom: 5px;
 padding: 0;
}


}
</style>