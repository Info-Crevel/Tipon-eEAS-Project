// Daily Time Record

<template>
  <section class="container p-0" :key="ts">
    <sym-form
      id="pay0030" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

      <div slot="footer" class="action-buttons p-1">
          
        <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        </div>

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit"> <i class="fa fa-save fa-lg" ></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear"> <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>
      </div>
      </div>


      <div class="header-containerXXX">
        <div class="applicantid-info fw-110">
          <sym-int v-model="timekeeping.timekeepingSheetId" :caption-width="35" caption="Sheet ID" lookupId="PayTimekeepingSheet" @lostfocus="onTimekeepingSheetIdLostFocus" @changing="onTimekeepingSheetIdChanging" @changed="onTimekeepingSheetIdChanged" @searchresult="onTimekeepingSheetIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
          </div> 
        </div>
          
      </div>
        <div class="box-field">

            <div class="app-box-style">
              <div class="box-1 gap">
                <sym-int v-model="timekeeping.clientPayGroupId" :caption-width="51" align="left" caption="Pay ID" lookupId="ArsClientPayGroupSheet" @lostfocus="onClientPayGroupIdLostFocus" @changing="onClientPayGroupIdChanging" @changed="onClientPayGroupIdChanged" @searchresult="onClientPayGroupIdSearchResult"></sym-int>
                <sym-text v-model="timekeeping.clientPayGroupName" align="left" :caption-width="35" caption="Pay Group Name" ></sym-text>          
                <sym-date v-model="timekeeping.cutOffStartDate" align="left" :caption-width="20" caption="Start" ></sym-date>
                <sym-date v-model="timekeeping.cutOffEndDate" align="left" :caption-width="20" caption="End" ></sym-date>
                
              </div>
              <div class="box-2 gap">
                <sym-text v-model="timekeeping.remarks" align="left" :caption-width="32" caption="Remarks" ></sym-text>
        
               
              </div>
              

              <div class="act-btn gap">

              
                <button type="button" :class="logButtonClass" class=" w-10 info-light" @click="onUploadExcelRaw"> 
                  <i class="fa fa-clock-o"></i><span class="bold">DTR Raw Logs</span> 
                </button>
                <button type="button" :class="logButtonClass" class=" w-10 success-light" @click="onUploadExcelManual"> 
                  <i class="fa fa-pencil"></i><span class="bold">Manual DTR</span> 
                </button>
              </div>

            <div>

          <input
            ref="excelInput"
            type="file"
            accept=".xlsx, .xls"
            style="display: none"
            @change="handleFileUpload"
          />
         
       
          <input
            ref="excelInputManual"
            type="file"
            accept=".xlsx, .xls"
            style="display: none"
            @change="handleFileUploadManual"
          />

        </div>

          </div>

              <div class="app-box-style">
        <div class="text-center border-light curved p-1 info mt-2">
          <span class="serif lg-3">Total Schedule/s:  {{ schedules.length}}</span>
        </div>

<div class="d-flex align-center mb-2">
  <sym-text v-model="searchMemberNameInput" align="left" :caption-width="32" :input-width="100" caption=" Name" list="memberNames" @changed="applyMemberNameSearch"></sym-text>
  <datalist id="memberNames">
    <option 
      v-for="item in memberList" 
      :key="item.memberName" 
      :value="item.memberName" 
      @input="e => memberName = e.toUpperCase()" 
      class="dropdown">
    </option>
  </datalist>
</div>

<div class="table-scroller" v-show="timekeeping.timekeepingSheetId != 0">
  <div class="d-flex justify-centerXXX fixed-header ">
    <table class="table-scroll striped-even mb-0 scroller">
      <thead>
        <tr>
          <th class="id">MemberId</th>
          <th class="name">Member Name</th>
          <th class="text-center date">Date</th>
          <th class="text-center type">Day Type</th>
          <th class="text-center sched">Schedule</th>
          <th class="text-center in">Time In</th>
          <th class="text-center out">Time Out</th>
          <th class="text-center date out">Date Time Out</th>
          <th class="text-center hour">Total Working Hour</th>
          <th class="actions">Action</th>
        </tr>
      </thead>
      <tbody class="white">
          <tr   v-for="(dtl, index) in filteredSchedules"
  :key="`${dtl.memberId}_${dtl.scheduleDate}`"
  :class="{ 'highlight-row': index === highlightedIndex }">
          <td>{{ dtl.memberId }}</td>
          <td>{{ dtl.memberName }}</td>
          <td>{{ dtl.scheduleDate }}</td>
          <td class="text-center" :class="{ 'text-red': dtl.dayTypeName }">{{ dtl.dayTypeName }}</td>
          <td class="text-center sched">{{ dtl.scheduleCode }}</td>
          <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeIn) }}</td>
          <td class="text-center">{{ core.toTimeDisplayFormat(dtl.scheduleTimeOut) }}</td>
          <td class="text-center sched">{{ dtl.scheduleDateTimeOut }}</td>

          <td class="text-center">{{ dtl.totalWorkingHour === 0 ? '' : dtl.totalWorkingHour }}</td>

          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
              <button type="button" class="justify-between infoXXX info-light fw-22 btn-dtl-edit" @click="onEditSchedule(dtl)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
            </div>
          </td>
        </tr>
      </tbody>      
    </table> 
  </div>  
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
  <form id="pay0030B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="schedule-editor-boxes gap">
      <sym-int  v-model="schedule.memberId" caption="Member ID" align="bottom"></sym-int>
      <sym-text  v-model="schedule.memberName" caption="Member Name" align="bottom"></sym-text>

      <sym-date  v-model="schedule.scheduleDate" caption="Date" align="bottom"></sym-date>
     
    </div>
    <div class="sched-box gap">
      <sym-text  v-model="schedule.scheduleCode" caption="Schedule" align="bottom"></sym-text>
      <sym-time  v-model="schedule.scheduleTimeIn" caption="Time In" :isoFormat="true" align="bottom"></sym-time>
      <sym-time  v-model="schedule.scheduleTimeOut" caption="Time Out" :isoFormat="true" align="bottom"></sym-time>
       <sym-date v-model="schedule.scheduleDateTimeOut" align="bottom" :caption-width="30" caption="Date Out" ></sym-date>

    </div>
    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitSchedule()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isScheduleEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>  
</div>

</sym-modal>

<sym-modal
  id="excel-editor"
  v-model="isExcelDataVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="excelDataTitle"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>
<div class="board p-1 mb-0 w-100">
  <form id="pay0030B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div>

<div class="excel-table-scroller" v-show="excelData">
  <div class="d-flex justify-centerXXX fixed-header ">
    <table class="striped-even mb-0 excel-scroller">
      <thead>
        <tr>
          <th class="text-center w-8">#</th>
          <th class="text-center w-8">Member ID</th>
          <th class="name">Member Name</th>
          <th class="text-center w-10">Date</th>
          <th class="text-center in">Time In</th>
          <th class="text-center out">Time Out</th>
          <th class="text-center w-10">Date Time Out</th>
        </tr>
      </thead>
      <tbody class="white">
          <tr v-for="(dtl, index) in excelData" :key="index" :class="{ 'danger-light bold': dtl.invalidFlag }">

            <td class="text-center">{{ index + 1 }}</td>
            <td :class="{ 'line-through': dtl.invalidFlag }">{{ dtl.memberId }}</td>
          <td :class="{ 'line-through': dtl.invalidFlag }">{{ dtl.memberName }}</td>
          <td :class="{ 'line-through': dtl.invalidFlag }">{{ dtl.scheduleDate }}</td>

          <td class="text-center" :class="{ 'line-through': dtl.invalidFlag }">{{ core.toTimeDisplayFormat(dtl.scheduleTimeIn) }}</td>
          <td class="text-center" :class="{ 'line-through': dtl.invalidFlag }">{{ core.toTimeDisplayFormat(dtl.scheduleTimeOut) }}</td>
          <td :class="{ 'line-through': dtl.invalidFlag }">{{ dtl.scheduleDateTimeOut }}</td>

        </tr>
      </tbody>      
       <tfoot>
     <tr>
    <td colspan="7" class="text-center warning-light bold">
      Total Count: {{ excelData.length }} |
      Invalid Data: {{ excelData.filter(dtl => dtl.invalidFlag).length }}
    </td>
  </tr>
  </tfoot>
    </table> 
  </div>  
</div>
    </div>
    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitExcelData()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isExcelDataVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>  
</div>

</sym-modal>

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
import { thresholdFreedmanDiaconis } from 'd3';

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "pay0030",

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
        scheduleDate: null,
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
        scheduleTimeIn:  null,
        scheduleTimeOut:  null,
        scheduleDateTimeOut:  null,
        totalWorkingHour:  0,
        lockId: ''
      },

      scheduleIndex: -1,
      isAddingSchedule: false,
      excelData: [],
      isReading: false,

      searchMemberNameInput: '',


      isExcelDataVisible: false,
      excelData: [],
      highlightedIndex: null,

    };



  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

  onSubmitExcelData() {

    this.excelData
      .filter(newItem => !newItem.invalidFlag) 
      .forEach(newItem => {
        const existingIndex = this.schedules.findIndex(oldItem =>
          String(oldItem.memberId) === String(newItem.memberId) &&
          oldItem.scheduleDate === newItem.scheduleDate
        );

        if (existingIndex !== -1) {
          const existingItem = this.schedules[existingIndex];
          this.$set(this.schedules, existingIndex, {
            ...existingItem,
            scheduleTimeIn: newItem.scheduleTimeIn,
            scheduleTimeOut: newItem.scheduleTimeOut,
            scheduleDateTimeOut: newItem.scheduleDateTimeOut
          });
        }
      });

    this.schedules = this.schedules.filter(item => item.memberName && item.memberName.trim() !== ''); 
    this.advice.success('Upload successful.', { duration: 5 });
    this.excelData = [];
    this.isExcelDataVisible = false;
  },

handleFileUploadManual(event) {
  const file = event.target.files[0];
  this.isExcelDataVisible = true;
  
  if (!file) return;

  const reader = new FileReader();

  reader.onload = (e) => {
    const data = new Uint8Array(e.target.result);
    const workbook = XLSX.read(data, { type: 'array' });
    const firstSheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[firstSheetName];

const rawData = XLSX.utils.sheet_to_json(worksheet, { defval: '', raw: false });

const isValidTime = str => /^([01]?\d|2[0-3]):[0-5]\d$/.test(str);

const newSchedules = rawData.map(row => {
  const memberId = row['MemberId'];
  const dateRaw = row['Date'];
  const timeInStr = row['TimeIn'];
  const timeOutStr = row['TimeOut'];
  const dateTimeOutStr = row['DateTimeOut'];
  const isMissingRequiredField =
    !memberId || memberId === 0 ||
    !dateRaw ||
    !isValidTime(timeInStr) ||
    !isValidTime(timeOutStr);

  let scheduleDate = '';
  let timeIn = null;
  let timeOut = null;
  let dateTimeOut = '';
    const dateObj = new Date(dateRaw);
    const dateTimeOutObj = new Date(dateTimeOutStr);
    

    scheduleDate = `${dateObj.getFullYear()}-${String(dateObj.getMonth() + 1).padStart(2, '0')}-${String(dateObj.getDate()).padStart(2, '0')}`;

    dateTimeOut = `${dateTimeOutObj.getFullYear()}-${String(dateTimeOutObj.getMonth() + 1).padStart(2, '0')}-${String(dateTimeOutObj.getDate()).padStart(2, '0')}`;

    if (isNaN(dateTimeOutObj.getTime())) {
      dateTimeOut = scheduleDate;
    }
    const timeInParts = timeInStr.split(':').map(Number);
    const timeOutParts = timeOutStr.split(':').map(Number);

    timeIn = new Date(dateObj);
    timeIn.setHours(timeInParts[0], timeInParts[1], 0, 0);

    timeOut = new Date(dateObj);
    timeOut.setHours(timeOutParts[0], timeOutParts[1], 0, 0);

    if (timeOut <= timeIn) {
      timeOut.setDate(timeOut.getDate() + 1);
    }


  const matchedMember = this.memberList.find(m => String(m.memberId) === String(memberId));

  const existsInSchedules = this.schedules.some(s =>
    String(s.memberId) === String(memberId) &&
    s.scheduleDate === scheduleDate
  );

  const invalidFlag = isMissingRequiredField || !existsInSchedules || !matchedMember || scheduleDate > dateTimeOut;

  return {
    timekeepingSheetId: this.timekeeping.timekeepingSheetId,
    memberId: memberId || '',
    memberName: matchedMember ? matchedMember.memberName : '',
    scheduleDate,
    timekeepingScheduleId: '0',
    timekeepingScheduleIdA: '0',
    timekeepingScheduleIdB: '0',
    timekeepingScheduleIdC: '0',
    timekeepingScheduleIdD: '0',
    scheduleTimeIn: timeIn ? timeIn.toTimeString().split(' ')[0] : '',
    scheduleTimeOut: timeOut ? timeOut.toTimeString().split(' ')[0] : '',
    scheduleDateTimeOut: dateTimeOut,
    invalidFlag
  };
}).filter(Boolean); 
 
   this.excelData = newSchedules;

   };

  reader.readAsArrayBuffer(file);
},

  applyMemberNameSearch() {
    this.searchMemberName = this.searchMemberNameInput;
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

    onClientPayGroupIdChanging (e) {
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId='" + e.proposedValue + "'";
      return getList('dbo.ArsClientPayGroup', 'ClientPayGroupId', '', filter).then(
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

    onUploadExcelManual() {

   const input = this.$refs.excelInputManual;
    input.value = ''; 
    input.click();    

    },


  onUploadExcelRaw() {
    const input = this.$refs.excelInput;
    input.value = ''; 
    input.click();    

    },


handleFileUpload(event) {
  const file = event.target.files[0];
    this.isExcelDataVisible = true;

  if (!file) return;

  const reader = new FileReader();

  reader.onload = (e) => {
    const data = new Uint8Array(e.target.result);
    const workbook = XLSX.read(data, { type: 'array' });
    const firstSheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[firstSheetName];

    const rawData = XLSX.utils.sheet_to_json(worksheet, { defval: '', raw: false });
    const groupedLogs = {};
    const monthMap = {
      Jan: 0, Feb: 1, Mar: 2, Apr: 3, May: 4, Jun: 5,
      Jul: 6, Aug: 7, Sep: 8, Oct: 9, Nov: 10, Dec: 11
    };

    const isValidTime = (timeStr) => /^([01]?\d|2[0-3]):[0-5]\d:[0-5]\d$/.test(timeStr);

    rawData.forEach(row => {
      const memberId = row['MemberId'];
      const dateStr = row['Date'];  
      const timeStr = row['Time'];   
      const inOut = row['IN/OUT'];   


      if (!memberId || !dateStr || !timeStr || inOut === undefined || inOut === '') return;
      const [dayStr, monthStr, yearStr] = dateStr.split('-');
      const day = parseInt(dayStr, 10);
      const month = monthMap[monthStr];
      const year = 2000 + parseInt(yearStr, 10);
      const scheduleDate = new Date(year, month, day); 
      const fullDateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
      const key = `${memberId}_${fullDateStr}`;
      const time24 = this.formatTimeTo24Hr(timeStr);

      const time = new Date(`${fullDateStr}T${time24}`);

      if (!groupedLogs[key]) {
        groupedLogs[key] = {
          memberId,
          scheduleDate: fullDateStr,
          timeIn: null,
          timeOut: null
        };
      }

      if (inOut == '0' || inOut === 0) {
        if (!groupedLogs[key].timeIn || time < groupedLogs[key].timeIn) {
          groupedLogs[key].timeIn = time;
        }
      } else if (inOut == '1' || inOut === 1) {
        if (!groupedLogs[key].timeOut || time > groupedLogs[key].timeOut) {
          groupedLogs[key].timeOut = time;
        }
      }
    });
const newSchedules = Object.values(groupedLogs).map(entry => {
  const matchedMember = this.memberList.find(m => String(m.memberId) === String(entry.memberId));

  const timeInStr = entry.timeIn ? entry.timeIn.toTimeString().split(' ')[0] : '';
  const timeOutStr = entry.timeOut ? entry.timeOut.toTimeString().split(' ')[0] : '';

  const scheduleExists = this.schedules.some(s =>
    String(s.memberId) === String(entry.memberId) &&
    s.scheduleDate === entry.scheduleDate
  );
  const invalidFlag = (
    !entry.memberId ||
    entry.memberId === 0 ||
    !matchedMember ||
    !isValidTime(timeInStr) ||
    !isValidTime(timeOutStr) ||
    !scheduleExists
  );

  return {
    timekeepingSheetId: this.timekeeping.timekeepingSheetId,
    memberId: entry.memberId,
    memberName: matchedMember ? matchedMember.memberName : '',
    scheduleDate: entry.scheduleDate,
    timekeepingScheduleId: '0',
    timekeepingScheduleIdA: '0',
    timekeepingScheduleIdB: '0',
    timekeepingScheduleIdC: '0',
    timekeepingScheduleIdD: '0',
    scheduleTimeIn: timeInStr,
    scheduleTimeOut: timeOutStr,
    invalidFlag 
  };
});

this.excelData = newSchedules;

  };

  reader.readAsArrayBuffer(file);
},

    formatTimeTo24Hr(timeStr) {
      const [time, modifier] = timeStr.split(' ');
      let [hours, minutes, seconds] = time.split(':').map(Number);

      if (modifier === 'PM' && hours < 12) hours += 12;
      if (modifier === 'AM' && hours === 12) hours = 0;

      return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    },

    onShowScheduleEditor () {
      const me = this;
      let target = 'scheduleMemberId';

      me.setActiveModel('schedule');

      me.setRequiredMode(
        'scheduleDate'
      );  

      me.setDisplayMode(
        'memberId',
        'memberName',
        'scheduleDate', 
        'scheduleCode', 
      );  

      if (!me.isAddingSchedule) {
        if (me.schedule.lockId) {
          target = 'scheduleTimeIn';
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

  onEditSchedule(dtl) {
  const d = this.schedule;

  this.scheduleIndex = this.schedules.findIndex(item =>
    item.memberId === dtl.memberId && item.scheduleDate === dtl.scheduleDate
  );

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

  d.timekeepingScheduleId = dtl.timekeepingScheduleId;
  d.timekeepingScheduleIdA = dtl.timekeepingScheduleIdA;
  d.timekeepingScheduleIdB = dtl.timekeepingScheduleIdB;
  d.timekeepingScheduleIdC = dtl.timekeepingScheduleIdC;
  d.timekeepingScheduleIdD = dtl.timekeepingScheduleIdD;

  d.scheduleTimeIn = dtl.scheduleTimeIn;
  d.scheduleTimeOut = dtl.scheduleTimeOut;
    d.scheduleDateTimeOut = dtl.scheduleDateTimeOut;
  d.totalWorkingHour = dtl.totalWorkingHour;
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

    safeFormatDate(date) {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toISOString().slice(0, 10);
  },

    onSubmitSchedule () {
      const
        me = this,
        d = me.schedule;
        
      if (!me.isValid('pay0030A')) {
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
        
        dtl.timekeepingScheduleId = d.timekeepingScheduleId;
        dtl.timekeepingScheduleIdA = d.timekeepingScheduleIdA;
        dtl.timekeepingScheduleIdB = d.timekeepingScheduleIdB;
        dtl.timekeepingScheduleIdC = d.timekeepingScheduleIdC;
        dtl.timekeepingScheduleIdD = d.timekeepingScheduleIdD;

        dtl.scheduleTimeIn = d.scheduleTimeIn;
        dtl.scheduleTimeOut = d.scheduleTimeOut;
        dtl.scheduleDateTimeOut = d.scheduleDateTimeOut;

        dtl.scheduleCode = d.scheduleCode;
  
        dtl.lockId = d.lockId;

this.highlightedIndex = this.filteredSchedules.findIndex(item =>
  item.memberId === dtl.memberId && item.scheduleDate === dtl.scheduleDate
);

      setTimeout(() => {
        this.highlightedIndex = null;
      }, 2000); 
        
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

    loadData() {
      const me = this,
        wait = me.wait();
            
        me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.payFreq = data.payFreq;
            me.period = data.period;
            me.payrollPeriod = data.payrollPeriod;
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
          me.logs = logs;
          me.isLogVisible = true;
        },
        (fault) => {
          me.showFault(fault);
        }
      );
    },


    setModels(info) {
      const me = this,
        timekeeping = info.timekeeping[0];
    
      me.timekeeping = me.core.convertDates(timekeeping);
      me.schedules =  info.schedule;
      me.memberList = info.member;
      me.scheduleList = info.scheduleA;
      me.scheduleListA = info.scheduleB;
      me.scheduleListB = info.scheduleC;
      me.scheduleListC = info.scheduleD;
      me.scheduleListD = info.scheduleE;

      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.searchMemberNameInput = '';
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldSchedules = JSON.stringify(me.schedules);
    },

    getClientPayGroupInfo() {
      const me = this;
      return get("api/timekeeping-records-client-pay-group-id/" + me.timekeeping.clientPayGroupId );
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingSheetId < 0) {
        return Promise.resolve(null);
      }

      return get("api/timekeeping-records/" + this.timekeeping.timekeepingSheetId);
    },

    getReferences() {
      const me = this;
      if (me.schedules.length) {
        return Promise.resolve(true);
      }

      return get("api/references/pay0030");
    },

    getChangeLog(log) {
      return get("api/timekeeping-records/" + log + "/" +  this.timekeeping.timekeepingSheetId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/timekeeping-records/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeeping-records/" + this.timekeeping.timekeepingSheetId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeeping-records/" + this.timekeeping.timekeepingSheetId + "/" + timekeeping.lockId + "/" + currentUserId,options );
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
          "remarks",

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
      d.scheduleCode = '';
      d.scheduleDate = null;
      d.timekeepingScheduleId = 0;
      d.timekeepingScheduleIdA = 0;
      d.timekeepingScheduleIdB = 0;
      d.timekeepingScheduleIdC = 0;
      d.timekeepingScheduleIdD = 0;
      d.scheduleTimeIn = null;
      d.scheduleTimeOut = null;
      d.scheduleDateTimeOut = null;
      d.totalWorkingHour = 0;
      d.lockId = '';
    },
  
  },

  created () {
    const me = this;
    
    me.oldTimekeeping = '';
    me.oldSchedules = '';

    me.scheduleList = []; 
    me.scheduleListA = []; 
    me.scheduleListB = []; 
    me.scheduleListC = []; 
    me.scheduleListD = []; 
    me.holiday = []; 
    me.payFreq = []; 
    me.period = []; 
    me.payrollPeriod = []; 
    me.memberList = [];
    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {

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

  editorTitle () {
      if (this.isAddingSchedule) {
        return 'Add Schedule';
      }
      return 'Edit Schedule';
    },

  excelDataTitle () {
        return 'Excel Data';
    },

  },
};
</script>

<style scoped>

.highlight-row {
  background-color: #d6d1ff !important;
  transition: background-color 0.5s ease-in-out;
  box-shadow: 0 0 10px rgba(0, 191, 255, 0.5);

}

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
  grid-template-columns: .2fr 2fr .4fr .4fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.5fr .5fr  ;
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
  overflow-x: auto;
  height: 100%;
  max-height: 50vh;
  
}
.excel-table-scroller{
  overflow: auto;
  height: 50vh;
  
}

.id{
  width: 6rem;
}
.memberId{
  width: 6rem;
}
.name{
  width: 20rem;
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
.schedule-editor-boxes {
  display: grid;
  grid-template-columns: .5fr 2.5fr .7fr ;
}
.sched-box {
  display: grid;
  grid-template-columns: 2fr .5fr .5fr .7fr;
}
.act-btn{
  display: flex;
  flex-direction: row;
}

.line-through {
  text-decoration-line: line-through;
  text-decoration-color: red;
  text-decoration-style: solid; 
}

.line-through {
  position: relative;
  color: #fff;
  font-weight: 600;
  text-decoration: none; 
}

.line-through::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  top: 50%;
  height: .5px;
  background: linear-gradient(90deg, 
    transparent, 
    #ff003c, 
    #ff1a5b, 
    #ff003c, 
    transparent);
  box-shadow:
    0 0 10px #ff003c,
    0 0 20px #ff1a5b,
    0 0 30px #ff003c,
    0 0 40px #ff1a5b;
  transform: translateY(-50%);
  animation: neon-strike 2.5s ease-in-out infinite;
  z-index: 1;
}

@keyframes neon-strike {
  0%, 100% {
    background-position: 0% 50%;
    opacity: 1;
  }
  50% {
    background-position: 100% 50%;
    opacity: 0.7;
  }

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
  background-color: rgb(152, 255, 160);
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
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
  grid-template-columns: .2fr 2fr .4fr .4fr  ;
}
.box-2 {
  display: grid;
  grid-template-columns: 1.5fr .5fr  ;
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
  overflow-x: auto;
  height: 100%;
  max-height: 50vh;
}

.id{
  width: 6rem;
}
.excel-id{
  width: 1rem;
}
.name{
  width: 20rem;
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

.line-through {
  text-decoration-line: line-through;
  text-decoration-color: red;
  text-decoration-style: solid;
}

.line-through {
  position: relative;
  color: #fff;
  font-weight: 600;
  text-decoration: none;
}

.line-through::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  top: 50%;
  height: .5px;
  background: linear-gradient(90deg, 
    transparent, 
    #ff003c, 
    #ff1a5b, 
    #ff003c, 
    transparent);
  box-shadow:
    0 0 10px #ff003c,
    0 0 20px #ff1a5b,
    0 0 30px #ff003c,
    0 0 40px #ff1a5b;
  transform: translateY(-50%);
  animation: neon-strike 2.5s ease-in-out infinite;
  z-index: 1;
}
thead{
  position: sticky;
  top: 0;
  z-index: 20;
}


@keyframes neon-strike {
  0%, 100% {
    background-position: 0% 50%;
    opacity: 1;
  }
  50% {
    background-position: 100% 50%;
    opacity: 0.7;
  }
}

}

</style>
