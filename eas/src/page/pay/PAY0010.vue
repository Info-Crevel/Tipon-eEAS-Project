// Timekeeping Policy Setup

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="pay0010" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>

          <button type="button" :class="activeButtonClass" class="justify-between btn-active w-15" @click="onSetActive" v-show="timekeeping.timekeepingId != 0 && isActive != 1"><i class="fa fa-check-circle-o fa-lg"></i><span>Activate</span></button>

          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit"  > <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>

          <button type="button" :class="cancelButtonClass" class="justify-between btn-cancel" @click="onSetCanceled" v-if="canSetCanceled && !isCancelled"><i class="fa fa-thumbs-o-down fa-lg"></i><span>Cancel</span></button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      </div>
      </div>


      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="timekeeping.timekeepingId" :caption-width="20" caption="Policy ID" lookupId="PayTimekeeping" @lostfocus="onTimekeepingIdLostFocus" @changing="onTimekeepingIdChanging" @changed="onTimekeepingIdChanged" @searchresult="onTimekeepingIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 

          <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
          <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActive">Active</sym-tag>
          <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isInActive">In-Active</sym-tag>

        </div>
          
      </div>
          <div class="box-table  gap">
            <div class="box-table-2 gap">
              <div class="d-inline-block border-soft  app-box-style gap w-100">
                <div class="text-center border-light curved p-1 slategray mt-2">
                  <span class="serif lg-3 ">Variables</span>
                </div>
                <div class="box-1 mt-1">
                  <sym-text v-model="timekeeping.timekeepingDescription " align="left" :caption-width="30"  caption="Policy Name" :disabled ="timekeeping.timekeepingId >= 0"></sym-text>    
                </div> 
                <div class="box-2 mb-2">
                  <div class="app-box-style ">
                    <div class="text-center border-light curved p-1 slategray mt-2 ">
                      <span class="serif lg-3 ">OVERTIME SETTINGS</span>
                    </div>
                      <sym-int v-model="timekeeping.minimumRequiredOvertimeCount" align="left" :caption-width="140"   caption="Minimum Required Overtime Before Count (Minutes)" ></sym-int>
                      <sym-int v-model="timekeeping.overtimeDurationCount" align="left" :caption-width="140"  caption="Overtime Interval (Minutes)" ></sym-int>
                      <sym-combo v-model="timekeeping.overtimeCountId" align="left" :caption-width="72" caption="Basis of Overtime Consideration" display-field="overtimeCountName" :datasource="overtimeCounts"></sym-combo>
                      <sym-date v-model="timekeeping.overtimeStartDate" align="left" :caption-width="72"  caption="Start Date of Overtime Consideration" @changing="onStartDateChanging"></sym-date>
                      <sym-date v-model="timekeeping.overtimeEndDate" align="left" :caption-width="72" caption="End Date of Overtime Consideration" @changing="onEndDateChanging"></sym-date>
                      <sym-int v-model="timekeeping.maximumRegularHourCount" align="left" :caption-width="140" caption="Maximum Regular Hours" ></sym-int>
                    </div>
                </div>
                
                <div class="box-6 mb-2">
                  
                  <div class="app-box-style">
                    <div class="text-center border-light curved p-1 slategray mb-2">
                      <span class="serif lg-3 ">LATE SETTINGS</span>
                    </div>
                    <sym-int v-model="timekeeping.lateGracePeriodCount" align="left" :caption-width="140"  caption="Late Grace Period (Per Minutes)" ></sym-int>
                    <sym-int v-model="timekeeping.lateCount" align="left" :caption-width="140"  caption="Late Interval (Minutes)" ></sym-int>
                      <table>
                    <thead>
                      <tr>
                        <th class="late-matrix " colspan="3">Late Matrix (Minutes)</th>
                      
                        <th class="late-matrix-button py-1">
                          <sym-dropdown class="dropdown">

                            <button type="button" class="justify-between btn-add white w-100"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>

                            <div class="dropbox align-right shadow-dark warning-light pt-3 px-4 gap matrix ">
                              <sym-int v-model="startMinute" caption="Start" :caption-width="20" :input-width="20" align="bottom" data-ignore-close="true"></sym-int>
                              <sym-int v-model="endMinute" caption="End" :caption-width="20" :input-width="20" align="bottom" data-ignore-close="true"></sym-int>
                              <sym-int v-model="lateCount" caption="Count" :caption-width="20" :input-width="20" align="bottom" data-ignore-close="true"></sym-int>

                              <button type="button" class="info justify-between border-main " :disabled="!startMinute || !endMinute || !lateCount" @click="onAddLateMatrix()"><i class="fa fa-plus "></i>Apply</button>

                        
                            </div>
                            
                          </sym-dropdown>
                        </th>
                      </tr>
                    </thead>

          <tbody slot="body">
            <tr >
            <th>Start</th>
            <th>End</th>
            <th>Count</th>
            <th>Action</th>
            </tr>
            
            <tr v-for="(matrix, index) in lateMatrix" :key="index" class="white align-center">
              <td class="text-center">{{ matrix.startMinute }}</td> 
              <td class="text-center">{{ matrix.endMinute }}</td>
              <td class="text-center">{{ matrix.lateCount }}</td>
            
              <td class="late-matrix-button text-center">
              <button type="button"  class="warning btn-dtl-delete" title="Delete"  @click="onDeleteLateMatrix(matrix.lateMatrixId)" > <i class="fa fa-times fa-lg"></i> </button></td>


            </tr>
          </tbody>

            </table>
                  <sym-combo v-model="timekeeping.lateCountId" align="bottom" :caption-width="140"  caption="Late Count Consideration" display-field="lateCountName" :datasource="lateCounts"></sym-combo>
                  <sym-combo v-model="timekeeping.undertimeCountId" align="bottom" :caption-width="140"  caption="Undertime Count Consideration" display-field="undertimeCountName" :datasource="undertimeCounts"></sym-combo>
                <sym-int v-model="timekeeping.undertimeDurationPerCount" align="left" :caption-width="140"  caption="Undertime Interval (Minutes)" ></sym-int>

                </div>
                
                </div>
                <div class="box">
                  <div class="app-box-style">

                  
                  <sym-int v-model="timekeeping.ndCount" align="left" :caption-width="140" caption="Night Differential Interval (Minutes)" ></sym-int>

                    <sym-combo v-model="timekeeping.paidUpHolidayId" align="bottom" :caption-width="45" caption="Paid Up Holiday Basis" display-field="paidUpHolidayName" :datasource="paidUpHolidays"></sym-combo>
                    <sym-combo v-model="timekeeping.nightDifferentialCountId" align="bottom" :caption-width="60" caption="Night Differential Hours Basis" display-field="nightDifferentialCountName" :datasource="nightDifferentialCounts"></sym-combo>
                  <sym-int v-model="timekeeping.regularHourIntervalMinute" align="left" :caption-width="140" caption="Regular Hours Interval (Minutes)" ></sym-int>

                    <sym-combo v-model="timekeeping.holidayCountId" align="bottom" :caption-width="60" caption="Holiday Count Consideration" display-field="holidayCountName" :datasource="holidayCounts"></sym-combo>

                    <sym-check v-model="timekeeping.constructionFlag" :caption-width="50" caption="Construction?"></sym-check>
              
      


                  </div>  
                </div>            
              </div> 
              <div class="rest-container">            
                <div class="d-inline-block border-soft  app-box-style gap">
                  <div class="text-center border-light curved p-1 slategray mt-2">
                    <span class="serif lg-3">Rest Day</span>
                  </div>
                  <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
                    <sym-table-header slot="header">
                      <tr class="align-top ">
                        <th class="w-20 text-center"><i class="fa fa-check"></i></th>
                        <th class="w-90">Days</th>
                      </tr>                 
                    </sym-table-header>
                    <sym-table-body slot="body" v-show="timekeeping.timekeepingId !=0">
                      <tr v-for="(restDay, index) in restDays" :key="index" class="align-top">
                        <td class="w-20 text-center"><input type="checkbox" :checked="isRestDaySelected(restDay)" :disabled="isCheckboxDisabled(restDay)"  @click="onToggleRestDaySelection(restDay)"></td>
                        <td class="w-90" >{{ restDay.restDayName }}</td>
                      </tr>
                    </sym-table-body>
  
  
                  </sym-table>
                  <div>&nbsp;</div>
                  <sym-check v-model="timekeeping.restDayOffsetFlag" :caption-width="50" caption="Rest Day Offsetting"></sym-check>
                  <sym-int v-model="timekeeping.restDayConsideration" align="left" :caption-width="140" caption="Max Regular Days per Week" ></sym-int>
  
                </div> 
              </div>
            </div>

             <div class="sched-container">
                <div class="app-box-style  " >
                  <div class="text-center border-light curved p-1 slategray mt-2">
                    <span class="serif lg-3">{{ detailTag }}</span>
                  </div>

                  
                  <div class=" fixed-header table-scroller">
                  
               

                      
                      <table class="striped-even mb-0" ref="table" id="loremTable ">
                        <thead>
                          <tr>
                            <th class="sched-code">Code</th>
                            <th class="time-in text-center">Time In</th>
                            <th class="time-out text-center">Time Out</th>
                            <th class="working-hours text-center">Total Scheduled Hrs</th>
                            <th class="upaid-breaks text-center">Unpaid Breaks Hrs</th>
                            <th class="upaid-breaks text-center">Regular Hrs</th>
                            <th class="actions text-center">Action</th>
                          </tr>
                        </thead>
                        <tbody class="white">
                          <tr v-for="(dtl, index) in schedules" :key="index">
                            <td>{{ dtl.timekeepingScheduleId }}</td>
                            <td class="text-center">{{ core.toTimeDisplayFormat(dtl.timeIn) }}</td>
                            <td class="text-center">{{ core.toTimeDisplayFormat(dtl.timeOut) }}</td>
                            <td class="text-center">{{ dtl.workingHour }}</td>
                            
                            <td class="text-center">{{ dtl.unpaidBreak }}</td>
                            <td class="text-center">{{ dtl.regularHour }}</td>
                            <td class="p-1">
                              <div class="d-flex justify-center" sm-1 mb-0>
                                <button type="button" class="justify-between infoXXX info-light fw-22 btn-dtl-edit" @click="onEditSchedule(dtl, index)">
                                  <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                </button>
                                <button type="button" class="warningXXX danger-light btn-dtl-delete" title="Delete" @click="onDeleteSchedule(index)">
                                  <i class="fa fa-times fa-lg"></i>
                                </button>
                              </div>
                            </td>
                          </tr>
                        </tbody>      
                      </table>
               
              
                  </div>  
                  
                  <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                    <div></div>

                    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                      <button type="button" class="justify-between btn-add" @click="onAddSchedule">
                        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
                      </button>
                    </div>
                    
                  </div>
                  <button type="button" class="success mb-3 w-100 text-center lg-2 p-1 shadow-soft" @click="tableToExcel('table', 'Lorem Table')" v-show="timekeeping.timekeepingId != 0"><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>                
                </div>
             </div>
          </div>

    </sym-form>


<sym-modal
  id="schedule-editor"
  v-model="isScheduleEditorVisible"
  size="md"
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
  <form id="pay0010A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="schedule-editor-boxes gap">
      <sym-time  v-model="schedule.timeIn" caption="Time In" :isoFormat="true" align="bottom"  ></sym-time>
      <sym-time  v-model="schedule.timeOut" caption="Time Out" :isoFormat="true" align="bottom" ></sym-time>
     <sym-dec  v-model="schedule.unpaidBreak" caption="Unpaid Break" align="bottom"></sym-dec>

    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitSchedule()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isScheduleEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>  
</div>

</sym-modal>



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
import SymTab from '../../comp/SymTab.vue';

export default {
  components: { SymImageSelect, SymInteger, SymTab },
  extends: PageMaintenance,
  name: "pay0010",

  data() {
    return {
      
      timekeeping: {
        timekeepingId: 0,
        policyStatusId: 0,
        timekeepingDescription: '',
        minimumRequiredOvertimeCount: 0,
        overtimeDurationCount: 0,
        overtimeCountId: 0, 
        overtimeStartDate: null,
        overtimeEndDate: null,
        maximumRegularHourCount: 0,
        lateGracePeriodCount: 0,
        lateCount: 0,
        ndCount: 0,
        lateUnitId: 0,
        paidUpHolidayId: 0,
        restDayOffsetFlag: false,
        restDayConsideration:0,
        lateCountId:0,
        undertimeCountId:0,
        holidayCountId:0,
        regularHourIntervalMinute:0,
        constructionFlag:false,
        undertimeDurationPerCount:0,
        lockId: '',
      },

      oldTimekeeping: [],
      logs: [],
      isLogVisible: false,

      memberRequest: [],
      restDayList: [],

      schedules: [],

      isScheduleEditorVisible: false,

      schedule: {
        timekeepingScheduleId: 0,
        timekeepingId: 0,
        timeIn: null,
        timeOut: null,
        workingHour: 0,
        unpaidBreak: 0,
        regularHour: 0,
        lockId: ''
      },

      scheduleIndex: -1,
      isAddingSchedule: false,
      uri :'data:application/vnd.ms-excel;base64,',
        template:'<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64: function(s){ return window.btoa(unescape(encodeURIComponent(s))) },
        format: function(s, c) { return s.replace(/{(\w+)}/g, function(m, p) { return c[p]; }) },

      lateMatrix: [],
      startMinute: 0,
      endMinute: 0,
      lateCount: 0,
      lateMatrixId: 0,
      activeTabIndex: 0,

    };


  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
 
  },
  
  methods: {

    onActiveTabIndexChanged () {
      this.reload();
    },

    onAddLateMatrix () {
      const me = this;
      if (me.startMinute) {
        me.lateMatrixId = me.lateMatrixId - 1;
        me.lateMatrix.push({
          timekeepingId: me.timekeeping.timekeepingId,
          startMinute: me.startMinute,
          endMinute: me.endMinute,
          lateCount: me.lateCount,
          lateMatrixId: me.lateMatrixId,
          lockId: me.lateMatrixId.toString()
        })
      }
      me.startMinute = 0;
      me.endMinute = 0;
      me.lateCount = 0;
    },

    onDeleteLateMatrix (lateMatrixId) {
      const
        me = this,
        index = me.core.getArrayIndex(me.lateMatrix, 'lateMatrixId', lateMatrixId);

      if (index > -1) {
        me.lateMatrix.splice(index, 1);
      }
    },

  formatDateToMMDDYYYY(dateStr) {
    const d = new Date(dateStr);
    if (isNaN(d)) return ''; 
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    const yyyy = d.getFullYear();
    return `${mm}/${dd}/${yyyy}`;
  },


tableToExcel(tableRefName, sheetName) {
  let table = this.$refs[tableRefName];
  let tableClone = table.cloneNode(true);
  const thead = tableClone.querySelector('thead tr');
  if (thead) thead.removeChild(thead.lastElementChild);
  tableClone.querySelectorAll('tbody tr').forEach(row => {
    row.removeChild(row.lastElementChild);


    const timeInCell = row.children[1];
    if (timeInCell) {
      let val = timeInCell.textContent.trim();
      val = val.padStart(4, '0');
      timeInCell.textContent = val;
      timeInCell.setAttribute('style', (timeInCell.getAttribute('style') || '') + "mso-number-format:'@';");
    }

    const timeOutCell = row.children[2];
    if (timeOutCell) {
      let val = timeOutCell.textContent.trim();
      val = val.padStart(4, '0');
      timeOutCell.textContent = val;
      timeOutCell.setAttribute('style', (timeOutCell.getAttribute('style') || '') + "mso-number-format:'@';");
    }
  });

  const headerCells = tableClone.querySelectorAll('thead th');
  const colNames = ['Code', 'Time In', 'Time Out', 'Total Scheduled Hours', 'Unpaid Breaks'];
  headerCells.forEach(th => {
    if (colNames.includes(th.textContent.trim())) {
      th.style.backgroundColor = '#cce5ff'; 
      th.style.color = '#000';               
    }
  });

const overtimeCountName = (this.overtimeCounts.find(item => item.overtimeCountId === this.timekeeping.overtimeCountId) || {}).overtimeCountName || '';
const paidUpHolidayName = (this.paidUpHolidays.find(item => item.paidUpHolidayId === this.timekeeping.paidUpHolidayId) || {}).paidUpHolidayName || '';
const nightDifferentialCountName = (this.nightDifferentialCounts.find(item => item.nightDifferentialCountId === this.timekeeping.nightDifferentialCountId) || {}).nightDifferentialCountName || '';

const startDate = this.formatDateToMMDDYYYY(this.timekeeping.overtimeStartDate);
const endDate = this.formatDateToMMDDYYYY(this.timekeeping.overtimeEndDate);

  const customHeaderRows = `
    <table>
      <tr>
        <td style="font-weight: bold; background-color: #d3d3d3;">Policy ID</td>
        <td colspan="4" style="text-align: left;">${this.timekeeping.timekeepingId}</td>
      </tr>
      <tr>
        <td style="font-weight: bold; background-color: #d3d3d3;">Policy Name</td>
        <td colspan="4">${this.timekeeping.timekeepingDescription}</td>
      </tr>
      <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Minimum Required Overtime Before Count (Minutes)</td>
        <td colspan="1">${this.timekeeping.minimumRequiredOvertimeCount}</td>
      </tr>
      <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Duration of Overtime per Count (Minutes)</td>
        <td colspan="1">${this.timekeeping.overtimeDurationCount}</td>
      </tr>
      <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Basis of Overtime Consideration</td>
        <td colspan="1">${overtimeCountName}</td>
        </tr>
<tr>
  <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Start Date of Overtime Consideration</td>
  <td colspan="1" style="mso-number-format:'mm/dd/yyyy';">${this.timekeeping.overtimeStartDate}</td>
</tr>
<tr>
  <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">End Date of Overtime Consideration</td>
  <td colspan="1" style="mso-number-format:'mm/dd/yyyy';">${this.timekeeping.overtimeEndDate}</td>
</tr>
      <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Maximum Regular Hours</td>
        <td colspan="1">${this.timekeeping.maximumRegularHourCount}</td>
      </tr>
     <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Late Grace Period (Minutes)</td>
        <td colspan="1">${this.timekeeping.lateGracePeriodCount}</td>
      </tr>
     <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Late Count (Minutes)</td>
        <td colspan="1">${this.timekeeping.lateCount}</td>
      </tr>
     <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Paid Up Holiday Basis</td>
        <td colspan="1">${paidUpHolidayName}</td>
      </tr>
     <tr>
        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">Night Differential Hours Basis</td>
        <td colspan="1">${nightDifferentialCountName}</td>
      </tr>

    </table>
  `;

  const wrapper = document.createElement('div');
  wrapper.innerHTML = customHeaderRows + tableClone.outerHTML;

  const ctx = {
    worksheet: sheetName || 'Worksheet',
    table: wrapper.innerHTML
  };

  window.location.href = this.uri + this.base64(this.format(this.template, ctx));
},

    onActive () {
      const me = this;

      me.dialog.confirm('Ready to activate <b>Timekeeping Policy #</b>' + this.timekeeping.timekeepingId + ' - '+ this.timekeeping.timekeepingDescription + '.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.timekeeping.policyStatusId = 1;
            me.isActivating = true;
            me.onSubmit();
          }
          return;
        }
      );
    },


    onSetActive () {
      const me = this;
            
      let promise = me.isActionAllowed('ACT-STATUS-POL');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.onActive();
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },


    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to cancel <b>Timekeeping Policy #</b>' + this.timekeeping.timekeepingId + ' - '+ this.timekeeping.timekeepingDescription + '.<br>Once cancelled, policy cannot be use in timekeeping.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.timekeeping.policyStatusId = 3;
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },


    onSetCanceled () {
      const me = this;
            
      let promise = me.isActionAllowed('CAN-STATUS-POL');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.onCancel();
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },
  
calculateWorkingHours() {
  if (!this.schedule.timeIn || !this.schedule.timeOut) {
    this.schedule.workingHour = 0;
    this.schedule.regularHour = 0;
    return;
  }

  const getMinutes = (time) => {
    if (typeof time === 'object' && time.hours !== undefined && time.minutes !== undefined) {
      return time.hours * 60 + time.minutes;
    }
    if (typeof time === 'string' && time.length === 4) {
      const hours = parseInt(time.slice(0, 2), 10);
      const minutes = parseInt(time.slice(2), 10);
      return hours * 60 + minutes;
    }
    return 0;
  };

  const timeInMinutes = getMinutes(this.schedule.timeIn);
  const timeOutMinutes = getMinutes(this.schedule.timeOut);

  let diff = timeOutMinutes - timeInMinutes;
  if (diff < 0) {
    diff += 24 * 60;
  }

 
  const workingHours = diff / 60;


  const unpaidBreak = parseFloat(this.schedule.unpaidBreak) || 0;

  
  const regularHours = workingHours - unpaidBreak;

  this.schedule.workingHour = workingHours.toFixed(2);
  this.schedule.regularHour = Math.max(0, regularHours).toFixed(2);
},


    onShowScheduleEditor () {
      const me = this;

      let target = 'timekeepingScheduleId';

      me.setActiveModel('schedule');

      me.setRequiredMode(
        'timeIn',
        'timeOut'
      );  

      if (!me.isAddingSchedule) {
        if (me.schedule.lockId) {
          target = 'timeIn';
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
      dtl = this.core.convertTimes(dtl);
      d.timeIn = dtl.timeIn;
      d.timekeepingId = dtl.timekeepingId;
      d.timeOut = dtl.timeOut;
      d.workingHour = dtl.workingHour;
      d.unpaidBreak = dtl.unpaidBreak;
      d.regularHour = dtl.regularHour;
      d.lockId = dtl.lockId;

      this.isScheduleEditorVisible = true;
    },


    onAddSchedule () {
      const me = this;

      me.clearSchedulePad();
      me.schedule.timekeepingScheduleId = -1;
      me.isScheduleEditorVisible = true;
      me.isAddingSchedule = true;
    },

    onDeleteSchedule (index) {
      const me = this;

      if (!me.schedules[index].lockId) {
        me.schedules.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('PayTimekeepingSchedule', me.schedules[index].timekeepingId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Rate for <b>' + me.schedules[index].timekeepingId + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
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
      const me = this,
        d = me.schedule;

      if (!me.isValid('pay0010A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 });
        return;
      }

      me.calculateWorkingHours();

      let dtl = {};

      if (me.isAddingSchedule) {
        Object.assign(dtl, d); 
        dtl.timekeepingId = me.timekeeping.timekeepingId;
        me.schedules.push(dtl);

        me.clearSchedulePad();
        me.setFocus('timeIn');

      } else {
        dtl = me.schedules[me.scheduleIndex];
        dtl.timeIn = d.timeIn;
        dtl.timekeepingId = d.timekeepingId;
        dtl.timeOut = d.timeOut;
        dtl.workingHour = d.workingHour;
        dtl.unpaidBreak = d.unpaidBreak;
        dtl.regularHour = d.regularHour; 
        dtl.lockId = d.lockId;
        me.isScheduleEditorVisible = false;
      }


    },




    isRestDaySelected (restDay) { 
      
      return this.restDayList.findIndex(obj => obj.restDayId === restDay.restDayId) > -1;
    },
    onRestDayChange(restDay) {
    const id = restDay.restDayId;

    if (id === 8) {
      this.isFlexibleSelected = !this.isFlexibleSelected;
      if (this.isFlexibleSelected) {
        this.selectedWeekDays = [];
      }
    } else {
   
      const index = this.selectedWeekDays.indexOf(id);
      if (index >= 0) {
        this.selectedWeekDays.splice(index, 1);
      } else {
        this.selectedWeekDays.push(id);
        this.isFlexibleSelected = false;
      }
    }
  },
  isCheckboxDisabled(restDay) {
  const isFlexible = restDay.restDayId === 8;
  const isSelected = this.restDayList.some(day => day.restDayId === restDay.restDayId);
  const selectedWeekDays = this.selectedWeekDays;
  const selectedWeekDaysCount = selectedWeekDays.length;

  if (isFlexible) {
    return selectedWeekDaysCount > 0;
  }

  if (this.isFlexibleSelected) {
    return true;
  }
  if (selectedWeekDaysCount >= 2 && !isSelected) {
    return true;
  }

  return false;
},




    onToggleRestDaySelection (restDay) {
      const
        me = this,
        index = me.restDayList.findIndex(obj => obj.restDayId === restDay.restDayId);
       

      if (index > -1) {
        me.restDayList.splice(index, 1);
      } else {
        me.restDayList.push({
          timekeepingId: me.timekeeping.timekeepingId,
          restDayId: restDay.restDayId
        });
      }
    },


    getTargetPath() {
      const me = this,
        q = {};

      if (me.timekeeping.timekeepingId) {
        q.timekeepingId = me.timekeeping.timekeepingId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("timekeepingId" in q && me.core.isInteger(q.timekeepingId)) {
       
        me.timekeeping.timekeepingId = parseInt(q.timekeepingId);
      }

     },

    // API calls

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.minimumRequiredOvertimeUnits = data.minimumRequiredOvertimeUnit;
            me.overtimeDurationUnits = data.overtimeDurationUnit;
            me.overtimeCounts = data.overtimeCount;
            me.maximumRegularHourUnits = data.maximumRegularHourUnit;
            me.lateGracePeriodUnits = data.lateGracePeriodUnit;   
            me.lateUnits = data.lateUnit;   
            me.paidUpHolidays = data.paidUpHoliday;   
            me.nightDifferentialCounts = data.nightDifferentialCount;   
            me.restDays = data.restDay;
            me.lateCounts = data.lateCount;
            me.holidayCounts = data.holidayCount;
            me.undertimeCounts = data.undertimeCount;
          }
          if (me.timekeeping.timekeepingId < 0) {
            return Promise.resolve(null);
          }
          return me.getTimekeeping();
        })
        .then((timekeeping) => {
          me.stopWait(wait);
          if (timekeeping && timekeeping.timekeeping.length) {
            me.setModels(timekeeping);
          } else {
            if (me.timekeeping.timekeepingId > -1) {
              let message = "Timekeeping ID '<b>" + me.timekeeping.timekeepingId + "</b>' not found."; me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
              me.restDayList = [];
            } else {

              let mssg = 'You are not allowed to create new documents.';
              me.advice.fault(mssg, { duration: 5 });
              me.onReset();
              return;
            }
            me.timekeeping.overtimeStartDate = me.today;
            me.timekeeping.overtimeEndDate = me.today;
            
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
      me.restDayList = info.restDayList;
      me.schedules = info.schedule;
      me.lateMatrix = info.lateMatrix;
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldRestDayList = JSON.stringify(me.restDayList);
      me.oldSchedules = JSON.stringify(me.schedules);
      me.oldLateMatrix = JSON.stringify(me.lateMatrix);
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingId < 0) {
        return Promise.resolve(null);
      }

      return get("api/timekeepings/" + this.timekeeping.timekeepingId);
    },

    getReferences() {
      const me = this;
      if (me.minimumRequiredOvertimeUnits.length) {
        return Promise.resolve(true);
      }

      return get("api/references/pay0010");
    },

    getChangeLog(log) {
      return get("api/timekeepings/" + log + "/" +  this.timekeeping.timekeepingId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/timekeepings/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeepings/" + this.timekeeping.timekeepingId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeepings/" + this.timekeeping.timekeepingId + "/" + timekeeping.lockId + "/" + currentUserId,options );
      },

    getApiPayload() {
      const me = this,
        timekeeping = {};

      Object.assign(timekeeping, me.timekeeping);
      timekeeping.restDayList = me.restDayList;
      timekeeping.schedules = me.schedules;
      timekeeping.lateMatrix = me.lateMatrix;
      return timekeeping;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "timekeeping", "restDays","schedules","restDay","schedule","lateMatrix",

      );
      dc.keyField = "timekeepingId";
      dc.autoAssignKey = true;
    },

    
    onResetAfter() {
      (this.docs = []),
      this.isCancelling = false;
      this.isActivating = false;

      this.refreshOldRefs();

      setTimeout(() => {  this.disableElement("btn-add"); }, 100);
    },

    onTimekeepingIdChanging(e) {
      e.callback = this.timekeepingIdCallback;
    },

    timekeepingIdCallback(e) {
      e.message = "Timekeeping ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.PayTimekeeping", "timekeepingId", e.proposedValue ).then((result) => { 
        return result;
      });
    },

    onTimekeepingIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onTimekeepingIdLostFocus() {
      const me = this;

      if (!me.timekeeping.timekeepingId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onTimekeepingIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.timekeeping.timekeepingId = data.timekeepingId;
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
              me.timekeeping.timekeepingId = success;

            }
            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";

              if (me.isCancelling) {
                message = "Policy # '" + me.timekeeping.policyStatusId + " - " + me.timekeeping.timekeepingDescription + "' cancelled."
              }

              if (me.isActivating) {
                message = "Policy # '" + me.timekeeping.policyStatusId + " - " + me.timekeeping.timekeepingDescription + "' activated."
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

      getSafeDeleteFlag("PayTimekeeping", me.timekeeping.timekeepingId)
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
          "timekeepingId",
          "timekeepingDescription",
        );
        me.setFocus("timekeepingDescription");
      }, 100);
    },

    hasChanges() {
      const me = this;
       if (!me.isNew() && me.noEditFlag) {
        return false;
      }
    
      if (JSON.stringify(me.timekeeping) !== me.oldTimekeeping) {return true;}
      if (JSON.stringify(me.restDayList) !== me.oldRestDayList) { return true; }
      if (JSON.stringify(me.schedules) !== me.oldSchedules) { return true; }
      if (JSON.stringify(me.lateMatrix) !== me.oldLateMatrix) { return true; }

      return false;

    },

    clearSchedulePad () {
      const d = this.schedule;

      d.timeIn = null;
      d.timeOut = null;
      d.workingHour = 0;
      d.unpaidBreak = 0;
      d.regularHour = 0;
      d.lockId = '';

    },

    onStartDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onStartDateCallback;
    },

    onStartDateCallback (e) {
      const me = this;

      if ( e.proposedValue < me.today) {
        e.message = 'Invalid Start Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }
      return true;
    },

    onEndDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onEndDateCallback;
    },

    onEndDateCallback (e) {
      const me = this;

      if ( e.proposedValue < me.today) {
        e.message = 'Invalid End Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      return true;
    },
    isDisabled(id) {
      if (this.flexibleId === null) {
        return false;
      }
      if (this.selectedRestDays.includes(this.flexibleId)) {
        return id !== this.flexibleId;
      }
      if (this.selectedRestDays.length > 0) {
        return id === this.flexibleId;
      }
      return false;
    },
  },

  created () {
    const me = this;
    
    me.oldTimekeeping = '';
    me.oldRestDayList = '';
    me.oldSchedules = '';
    me.oldLateMatrix = '';

    me.minimumRequiredOvertimeUnits = [];
    me.overtimeDurationUnits = [];
    me.overtimeCounts = [];

    me.maximumRegularHourUnits = []; 
    me.lateGracePeriodUnits = []; 
    me.lateUnits = []; 
    me.paidUpHolidays = []; 
    me.nightDifferentialCounts = [];
    me.restDays = []; 
    me.lateCounts = []; 
    me.undertimeCounts = []; 
    me.holidayCounts = []; 
    me.today = me.sym.dateInfo.serverDate;
    me.isCancelling = false;
    me.isActivating = false;
  },

  computed: {
    
    canSetCanceled () {
      return this.timekeeping.policyStatusId === 2;
    },

    isCancelled () {
      return this.timekeeping.policyStatusId === 3;
    },
       
    isInActive () {
      
      return this.timekeeping.policyStatusId === 2;
    },
  
    isActive () {
      return this.timekeeping.policyStatusId === 1;
    },

  
    detailTag () {
      return 'Schedules: ' + this.schedules.length;
    },

    editorTitle () {
      if (this.isAddingSchedule) {
        return 'Add Schedule';
      }
      return 'Edit Schedule';
    },

    isFlexibleSelected() {
    return this.restDayList.some(day => day.restDayId === 8);
  },
  selectedWeekDays() {
    const weekDayIds = [1, 2, 3, 4, 5, 6, 7];
    return this.restDayList
      .filter(day => weekDayIds.includes(day.restDayId))
      .map(day => day.restDayId);
  },
  selectedWeekDaysCount() {
    return this.selectedWeekDays.length;
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

  .applicantid-info {
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
    grid-template-columns: 1fr  1.5fr;
  }

  .box-table-2{
    display: grid;
    grid-template-columns: 1fr  ;
  }
  .table-scroller{
    overflow: auto;
    height: auto;
    max-height: 55vh;
  }
  .scroller{
    width: 120vw;
    
  }

  .sched-code{
    width: 1.5rem;
  }
  .time-in{
    width: 2rem;
  }
  .time-out{
    width: 2rem;
  }
  .working-hours{
    width: 4rem;
  }
  .upaid-breaks{
    width: 2rem;
  }
  .actions{
    width: 3rem;
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

  .applicantid-info {
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
    height: auto;
    max-height: 50vh;
  }
  .scroller{
    width: 120vw;
    
  }

  .sched-code{
    width: 1.5rem;
  }
  .time-in{
    width: 2rem;
  }
  .time-out{
    width: 2rem;
  }
  .working-hours{
    width: 4rem;
  }
  .upaid-breaks{
    width: 2rem;
  }
  .actions{
    width: 3rem;
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

.applicantid-info {
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
  grid-template-columns: 1fr  .5fr;
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
  grid-template-columns: 1fr  ;
}
.box-6{
  display: grid;
  grid-template-rows: 1fr ;
}
.box-9{
  display: grid;
  grid-template-rows: .5fr 1fr ;
}
.matrix{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr ; 
}
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
    /* width: 90vw; */
  }
  .scroller{
    overflow-x:auto;
    /* width: 90vw;
     */
  } 

  .late-matrix {
    width: 100%;
  }

  .late-matrix-button {
    width: 20%;
  }


}
</style>