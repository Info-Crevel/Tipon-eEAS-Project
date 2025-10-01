// Timekeeping Schedule

<template> 
  <section class="container p-0" :key="ts">
    <sym-form
      id="pay0020" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

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
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div> 
        </div>
      </div>

        <div class="box-field">
           
          <div class="app-box-style ">
              <div class="box-1 gap">
                <sym-int v-model="timekeeping.clientPayGroupId" :caption-width="52" align="left" caption="Pay ID" lookupId="ArsClientPayGroupSheet" @lostfocus="onClientPayGroupIdLostFocus" @changing="onClientPayGroupIdChanging" @changed="onClientPayGroupIdChanged" @searchresult="onClientPayGroupIdSearchResult"></sym-int>
                <sym-text v-model="timekeeping.clientPayGroupName" align="left" :caption-width="35" caption="Pay Group Name" ></sym-text>          
                <sym-date v-model="timekeeping.cutOffStartDate" align="left" :caption-width="20" caption="Start" ></sym-date>
                <sym-date v-model="timekeeping.cutOffEndDate" align="left" :caption-width="20" caption="End" ></sym-date> 
              </div>
              <div class="box-2 gap">
                <sym-text v-model="timekeeping.remarks" align="left" :caption-width="32" caption="Remarks" ></sym-text>
              </div>

              <div class="upload-buttons">
                <button type="button" :class="logButtonClass" class=" applicant-btn primary w-17" v-if="timekeeping.clientPayGroupId && timekeeping.cutOffStartDate && timekeeping.cutOffEndDate" @click="onPopulate" >
                  <i class="fa fa-users fa-lg"></i><span class="bold">Populate</span>
                </button>
                <button type="button" :class="logButtonClass" class="applicant-btn info w-20" v-if="timekeeping.clientPayGroupId  && timekeeping.cutOffStartDate && timekeeping.cutOffEndDate && isSchedule>0" @click="onUploadExcel">            
                <i class="fa fa-upload fa-lg"></i><span class="bold">Upload Excel</span> 
                </button>
              </div>

            <div>
              <input ref="excelInput" type="file" accept=".xlsx, .xls" style="display: none" @change="handleFileUpload"/>
              <div v-if="isReading" class="loading-bar">
                <i class="fa fa-spinner fa-spin"></i> Reading Excel file...
              </div>
            </div>

          </div>

            <div class="app-box-style w-100">
              <div class="text-center border-light curved p-1 info  mb-2">
                <span class="serif lg-3">Member/s: {{groupedSchedules.length}}</span>
              </div>

            <div class="d-flex align-center mb-2">
              <sym-text v-model="searchMemberNameInput" align="left" :caption-width="32" :input-width="100" caption=" Name" list="memberNames" @changed="applyMemberNameSearch"></sym-text>
              <datalist id="memberNames"><option v-for="item in memberList" :key="item.memberName" :value="item.memberName" @input="e => memberName = e.toUpperCase()" class="dropdown"></option></datalist> 

            </div>
              <div class="table-scroller " v-show="timekeeping.timekeepingSheetId !=0">
          
                <table class="striped-even mb-0 scroller ">
                 <thead>
                    <tr>
                      <th class="id">Member ID</th>
                      <th class="name">Member Name</th>
                      <th
                        class="arcticblue bold text-center date"
                        v-for="dateObj in uniqueDates"
                        :key="dateObj.scheduleDate"
                      >
                        <div>
                          <div class="day-type-name red-text">{{ dateObj.dayTypeCode || '\u00A0' }}</div>
                          <div
                            class="date text-center"
                            :class="{ 'sunday-date': isSunday(dateObj.scheduleDate) }"
                          >
                            {{ core.toDateFormat(dateObj.scheduleDate, true, "MM/dd/yyyy") }}
                          </div>
                          <div class="day-type-name "
                            :class="{ 'sunday-date': isSunday(dateObj.scheduleDate) }"
                          >
                            {{ core.toDateFormat(dateObj.scheduleDate, true, "dddd") }}
                          </div>
                        </div>
                      </th>


                    </tr>
                  </thead> 
                  <tbody>
                   
                    <tr v-for="(member, index) in filteredGroupedSchedules" :key="member.memberId || index" class="text-center">
                      <td>{{ member.memberId }}</td>
                      <td>{{ member.memberName }}</td>
                      <td v-for="dateObj in uniqueDates" :key="dateObj.scheduleDate">
                        <div v-if="member.dates[dateObj.scheduleDate]">
                          <span
                            class="mb-0 bold italic  schedule-code-clickable"
                            @click="onEditSchedule(member.dates[dateObj.scheduleDate], member.dates[dateObj.scheduleDate].originalIndex)"
                            role="button"
                            tabindex="0"
                            @keyup.enter="onEditSchedule(member.dates[dateObj.scheduleDate], member.dates[dateObj.scheduleDate].originalIndex)"
                            :title="`Edit Time-In & Time-Out for ${formatDate(new Date(dateObj.scheduleDate))}`"
                          >
                            {{ member.dates[dateObj.scheduleDate].scheduleCode || '-' }}
                          </span>
                        </div>
                        <div v-else>
                          <span
                            class="mb-0 italic  schedule-code-clickable text-muted"
                            @click="onEditSchedule({ memberId: member.memberId, scheduleDate: new Date(dateObj.scheduleDate) }, null)"
                            role="button"
                            tabindex="0"
                            @keyup.enter="onEditSchedule({ memberId: member.memberId, scheduleDate: new Date(dateObj.scheduleDate) }, null)"
                            :title="`Add Time-In & Time-Out for ${formatDate(new Date(dateObj.scheduleDate))}`">
                          </span>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
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
  <form id="pay0020A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="schedule-editor-boxes gap">
      <sym-int  v-model="schedule.memberId" caption="Member ID" align="bottom"></sym-int>
      <sym-text  v-model="schedule.memberName" caption="Member Name" align="bottom"></sym-text>
      <sym-text  v-model="schedule.scheduleDate" caption="Date" align="bottom"></sym-text>

      <sym-combo  v-model="schedule.noScheduleId"  align="bottom" :caption-width="46" caption="No Schedule" display-field="noScheduleName" :datasource="noSchedules"/>
      
    </div>
    <div class="schedule-editor-boxes-2 gap">
      <sym-combo v-if="visibleCombos >= 1" v-model="schedule.timekeepingScheduleId" align="bottom" :caption-width="46" caption="Schedule" display-field="scheduleName" :datasource="scheduleList"/>
      <sym-combo v-if="visibleCombos >= 2" v-model="schedule.timekeepingScheduleIdA" align="bottom" :caption-width="46" caption="Schedule B" display-field="scheduleName" :datasource="scheduleListA"/>
      <sym-combo v-if="visibleCombos >= 3" v-model="schedule.timekeepingScheduleIdB" align="bottom" :caption-width="46" caption="Schedule C" display-field="scheduleName" :datasource="scheduleListB"/>
      <sym-combo v-if="visibleCombos >= 4" v-model="schedule.timekeepingScheduleIdC" align="bottom" :caption-width="46" caption="Schedule D" display-field="scheduleName" :datasource="scheduleListC"/>
      <sym-combo v-if="visibleCombos >= 5" v-model="schedule.timekeepingScheduleIdD" align="bottom" :caption-width="46" caption="Schedule E" display-field="scheduleName" :datasource="scheduleListD"/>
    </div>

    <div class="buttons w-100 justify-left mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button @click="addCombo" :disabled="visibleCombos >= maxCombos" class="success-light w-20"><i class="fa fa-plus mr-3"></i>Add Schedule</button>
      <button @click="removeCombo" :disabled="visibleCombos <= 1" class="danger-light"><i class="fa fa-minus mr-2"></i>Remove</button>
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
  <form id="pay0020B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div>


  <div class="d-flex justify-centerXXX fixed-header table-container "  v-show="excelData">
    <table class="striped-even mb-0 table-modal">
      <thead>
        <tr>
          <th class="text-center w-5">#</th>
          <th class="text-center w-5">Member ID</th>
          <th class="text-center w-50">Member Name</th>
          <th class="text-center w-15">Date</th>
          <th class="text-center w-20">Schedule</th>
        </tr>
      </thead>
      <tbody class="white">
          <tr v-for="(dtl, index) in excelData" :key="index" :class="{ 'danger-light bold': dtl.invalidFlag }">

            <td class="text-center w-5">{{ index + 1 }}</td>
            <td class="text-center w-5">{{ dtl.memberId}}</td>
            <td class="text-center w-30">{{ dtl.memberName}}</td>
            <td class="text-center w-15">{{ dtl.scheduleDate}}</td>
            <td class="text-center w-40">{{ dtl.scheduleCode}}</td>
        </tr>
      </tbody>      
       <tfoot>
     <tr>
    <td colspan="7" class="text-center warning-light bold">
      Total Count: {{ excelData.length }} |
    </td>
  </tr>
  </tfoot>
    </table> 
  </div>  

    </div>
    <div class="buttons  d-flex justify-between mt-3 mb-2"  shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" v-show="isCopyExcelData" @click="onCopyExcelData()"><i class="fa fa-check mr-2"></i>Copy To All Member</button>
       <button type="button" class="info justify-between border-main" v-show="isCopyExcelData" @click="onCopyExcelData()"><i class="fa fa-check mr-2"></i>Show All Member</button>
      <div class="act-btn gap">
      <button type="button" class="info justify-between border-main" @click="onSubmitExcelData()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isExcelDataVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>
    </div>

  </form>  
</div>

  <div class="d-flex justify-centerXXX fixed-header table-container "  v-show="excelData">
    <table class="striped-even mb-0 table-modal">
      <thead>
        <tr>
          <th class="text-center w-5">#</th>
          <th class="text-center w-5">Member ID</th>
          <th class="text-center w-50">Member Name</th>
  
        </tr>
      </thead>
      <tbody class="white">
          <tr v-for="(dtl, index) in schedules" :key="index" :class="{ 'danger-light bold': dtl.invalidFlag }">

            <td class="text-center w-5">{{ index + 1 }}</td>
            <td class="text-center w-5">{{ dtl.memberId}}</td>
            <td class="text-center w-30">{{ dtl.memberName}}</td>
            <!-- <td class="text-center w-15">{{ dtl.scheduleDate}}</td>
            <td class="text-center w-40">{{ dtl.scheduleCode}}</td> -->
        </tr>
      </tbody>      
       <tfoot>
     <tr>
    <td colspan="7" class="text-center warning-light bold">
      Total Count: {{ excelData.length }} |
    </td>
  </tr>
  </tfoot>
    </table> 
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

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "pay0020",

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
        cutOffId: 0,
        cutOffDateString: '',
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
        dayTypeCode: '',
        lockId: ''
      },

      scheduleIndex: -1,
      isAddingSchedule: false,
      excelData: [],
      isReading: false,

    visibleCombos: 1, 
    maxCombos: 5,
    searchMemberName:'',
       searchMemberNameInput: '',
      isExcelDataVisible: false,
      isCopyExcelData: false, 
  
  };



  },

  mounted() {
    const me = this;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {
 
onCopyExcelData() {
  const multipliedData = [];

  this.schedules.forEach(schedule => {
    this.excelData.forEach(template => {
      multipliedData.push({
        ...template,
        memberId: schedule.memberId,
        memberName: schedule.memberName
      });
    });
  });
  const seenKeys = new Set();
  const uniqueData = multipliedData.filter(item => {
    const key = `${item.memberId}_${item.scheduleDate}`;
    if (seenKeys.has(key)) {
      return false;
    }
    seenKeys.add(key);
    return true;
  });

  this.excelData = uniqueData;
  this.isCopyExcelData = false;
},

    onSubmitExcelData () {
 
      this.excelData.forEach(newItem => {
     
      const incomingDate = new Date(newItem.scheduleDate);

      const year = incomingDate.getFullYear();
      const month = String(incomingDate.getMonth() + 1).padStart(2, '0');
      const day = String(incomingDate.getDate()).padStart(2, '0');

      const formattedIncomingDate = `${year}-${month}-${day}`;

      const existingIndex = this.schedules.findIndex(oldItem => {
        const existingDate = new Date(oldItem.scheduleDate).toISOString().split('T')[0];
        return String(oldItem.memberId) === String(newItem.memberId) &&
               existingDate === formattedIncomingDate;
      });

      const updatedItem = {
        ...newItem,
        scheduleDate: formattedIncomingDate
      };

 

if (existingIndex !== -1) {
  const existingRecord = this.schedules[existingIndex];

  const isRestDay = existingRecord.noScheduleId == 1; 

  const mergedItem = {
    ...existingRecord,
    ...updatedItem,
    noScheduleId: isRestDay ? existingRecord.noScheduleId : newItem.noScheduleId,
    scheduleCode: newItem.scheduleCode,
    timekeepingScheduleId: newItem.timekeepingScheduleId,
    timekeepingScheduleIdA: newItem.timekeepingScheduleIdA,
    timekeepingScheduleIdB: newItem.timekeepingScheduleIdB,
    timekeepingScheduleIdC: newItem.timekeepingScheduleIdC,
    timekeepingScheduleIdD: newItem.timekeepingScheduleIdD
  };

  this.$set(this.schedules, existingIndex, mergedItem);

} else {

  this.schedules.push(updatedItem);
}

    });

    this.schedules = this.schedules.filter(item => item.memberName && item.memberName.trim() !== '');
    this.advice.success('Upload successful.', { duration: 5 });
    this.excelData = [];
    this.isExcelDataVisible = false;

    },



     applyMemberNameSearch() {
    this.searchMemberName = this.searchMemberNameInput;
  },

    onCutOffId() {

      this.cutOff = this.core.convertDates(this.cutOff)

      const selectedCutOff = this.cutOff.find(item => {
        return item.cutOffId == this.timekeeping.cutOffId;
      });

    
      if (selectedCutOff) {
        this.timekeeping.cutOffDate = selectedCutOff.startDate;
        this.timekeeping.cutOffStartDate = new Date(selectedCutOff.startDate);
        this.timekeeping.cutOffEndDate = new Date(selectedCutOff.endDate) ;

        const startDate = new Date(selectedCutOff.startDate);

        this.timekeeping.periodId = startDate.getMonth() + 1;
        this.timekeeping.yearId = startDate.getFullYear();

        this.timekeeping.startDate = selectedCutOff.startDate;
        this.timekeeping.endDate = selectedCutOff.endDate;
      }
    },

    groupedSchedulesMemberName() {
    const grouped = {};
    this.schedules.forEach((item, i) => {
      const key = item.memberId;
      if (!grouped[key]) {
        grouped[key] = {
          entries: []
        };
      }
      grouped[key].entries.push({ ...item, flatIndex: i });
    });

    return Object.values(grouped);
  },


    isSunday(dateStr) {
    const date = new Date(dateStr);
    return date.getDay() === 0;
  },

   formatDateWithDayName(dateStr) {
    const dateObj = new Date(dateStr);
    if (isNaN(dateObj)) return dateStr;

    const options = { year: 'numeric', month: '2-digit', day: '2-digit', weekday: 'short' };
    return dateObj.toLocaleDateString('en-US', options); 
  },
formatDate(date) {
  if (!date) return '';
  

  if (typeof date.toLocaleString === 'function') {
  
    return date.toLocaleString(); 
  }

  if (typeof date === 'string') {
    const d = new Date(date);
    if (!isNaN(d.getTime())) {
      return d.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
    }
  }

  return '';
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
          me.timekeeping.cutOffId = 0;
          me.cutOff = data.cutOff;
          me.restDay = data.restDay

        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },



   addCombo() {
    if (this.schedule.timekeepingScheduleId==999999999) {
      return ; 
    }

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
      return getList('dbo.QArsClientPayGroupSheet', 'ClientPayGroupId, ClientPayGroupName', '', filter).then(
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
      me.onClientPayGroupIdChanged();
    },


   onUploadExcel() {
      this.$refs.excelInput.click();
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

    const me = this;
    const validSchedules = [];
    const invalidSchedules = [];

    const startDate = new Date(
      this.timekeeping.cutOffStartDate.year,
      this.timekeeping.cutOffStartDate.month - 1,
      this.timekeeping.cutOffStartDate.day 
    );
    const endDate = new Date(
      this.timekeeping.cutOffEndDate.year,
      this.timekeeping.cutOffEndDate.month - 1,
      this.timekeeping.cutOffEndDate.day 
    );

    rawData.forEach(row => {
      const keys = Object.keys(row).reduce((acc, key) => {
        acc[key.trim().toUpperCase()] = row[key];
        return acc;
      }, {});

      const scheduleParts = (keys.SCHEDULE || '')
        .split(',')
        .map(s => s.trim())
        .filter(Boolean);

      const scheduleIds = [
        scheduleParts[0] || '',
        scheduleParts[1] || '',
        scheduleParts[2] || '',
        scheduleParts[3] || '',
        scheduleParts[4] || ''
      ];

      let scheduleCodeList = '';
      scheduleIds.forEach(id => {
        if (!id) return;
        const match = me.scheduleList.find(s => s.timekeepingScheduleId == id);
        if (match) {
          scheduleCodeList += (scheduleCodeList ? ' | ' : '') + match.scheduleName;
        }
      });

      const matchedMember = me.memberList.find(m => m.memberId == keys.MEMBERID);
      const memberName = matchedMember ? matchedMember.memberName : '';
      const noSchedule = keys.NOWORK;
      const noScheduleCode = noSchedule==='NS'? 2 : noSchedule ==='RD'? 1: 0 ;
      const scheduleCodeFinal = scheduleCodeList; 

      const noScheduleName = noSchedule === 'NS' ? 'NO SCHEDULE' : noSchedule === 'RD' ? 'REST DAY' :'';
      const scheduleDate = new Date(keys.DATE);

      let scheduleCode = noScheduleName;

      if (scheduleCodeFinal !== '' && noScheduleName !== '') {
        scheduleCode += ' | ' + scheduleCodeFinal;
      } else {
        scheduleCode += scheduleCodeFinal;
      }



      const record = {
        timekeepingSheetId: this.timekeeping.timekeepingSheetId,
        memberId: keys.MEMBERID || '',
        memberName: memberName,
        scheduleDate: keys.DATE || '',
        noScheduleId: noScheduleCode,
        timekeepingScheduleId:  scheduleIds[0],
        timekeepingScheduleIdA: scheduleIds[1],
        timekeepingScheduleIdB: scheduleIds[2],
        timekeepingScheduleIdC: scheduleIds[3],
        timekeepingScheduleIdD: scheduleIds[4],
        scheduleCode: scheduleCode
      };

      const isValidDate = scheduleDate >= startDate && scheduleDate <= endDate;
      const isValidMember = !!matchedMember;

      if (isValidDate && isValidMember) {
        validSchedules.push(record);
      } else {
        invalidSchedules.push(record);
      }
    });

      this.excelData = validSchedules;
    
      this.isCopyExcelData = true;

  };

  reader.readAsArrayBuffer(file);
},


onPopulate() {
  const me = this;

  let filter = "ClientPayGroupId='" + me.timekeeping.clientPayGroupId + "'";

  return getList('dbo.QArsMemberRequestHiredList', 'ClientPayGroupId, MemberId, MemberName', '', filter)
    .then(member => {
      if (member && member.length) {
        me.memberList = member;
        me.schedules = [];

        let currentDate = new Date(
          this.timekeeping.cutOffStartDate.year,
          this.timekeeping.cutOffStartDate.month - 1,
          this.timekeeping.cutOffStartDate.day 
        );

        const endDate = new Date(
          this.timekeeping.cutOffEndDate.year,
          this.timekeeping.cutOffEndDate.month - 1,
          this.timekeeping.cutOffEndDate.day 
        );

        const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const pad = n => n.toString().padStart(2, '0');
const formatLocalDate = (date) => {
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

while (currentDate <= endDate) {
  const dayName = dayNames[currentDate.getDay()];
  this.memberList.forEach(member => {
    const isRestDay = this.restDay.some(rest => {
  return rest.restDayName === dayName;
    });
    this.schedules.push({
      scheduleDate: formatLocalDate(currentDate),
      scheduledate: formatLocalDate(currentDate),
      dayName: dayName,
      memberId: member.memberId,
      memberName: member.memberName,
      noScheduleId: isRestDay ? 1 : 2, 
      timekeepingSheetId: this.timekeeping.timekeepingSheetId,
      scheduleCode: isRestDay ? 'REST DAY' : 'NO SCHEDULE'
    });
  });

  currentDate.setDate(currentDate.getDate() + 1);
}
        this.schedules = this.schedules.map(scheduleItem => {
          const matchingHoliday = this.holiday.find(holidayItem =>
            new Date(holidayItem.holidayDate).toISOString().split('T')[0] ===
            new Date(scheduleItem.scheduleDate).toISOString().split('T')[0]
          );

          return {
            ...scheduleItem,
            dayTypeCode: matchingHoliday ? matchingHoliday.dayTypeCode : (scheduleItem.dayTypeCode || '')
          };
        });

        this.refresh();
        return true;
      }
      return false;
    })
    .catch(fault => {
      me.showFault(fault);
      return false;
    });
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

      d.scheduleDate = dtl.scheduleDate;
     
      d.timekeepingSheetId = dtl.timekeepingSheetId;
      d.memberId = dtl.memberId;
      d.memberName = dtl.memberName;
      d.noScheduleId = dtl.noScheduleId;

      d.timekeepingScheduleId = dtl.timekeepingScheduleId;
      d.timekeepingScheduleIdA = dtl.timekeepingScheduleIdA;
      d.timekeepingScheduleIdB = dtl.timekeepingScheduleIdB;
      d.timekeepingScheduleIdC = dtl.timekeepingScheduleIdC;
      d.timekeepingScheduleIdD = dtl.timekeepingScheduleIdD;
      
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
        
      if (!me.isValid('pay0020A')) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

const normalizedDate = me.safeFormatDate(d.scheduleDate);

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
        dtl.noScheduleId = d.noScheduleId;
        dtl.scheduleDate = d.scheduleDate;
        dtl.scheduledate = d.scheduleDate;
        dtl.timekeepingScheduleId = d.timekeepingScheduleId;
        dtl.timekeepingScheduleIdA = d.timekeepingScheduleIdA;
        dtl.timekeepingScheduleIdB = d.timekeepingScheduleIdB;
        dtl.timekeepingScheduleIdC = d.timekeepingScheduleIdC;
        dtl.timekeepingScheduleIdD = d.timekeepingScheduleIdD;
        dtl.lockId = d.lockId;      
        let scheduleCodeList = '';  

        me.noSchedules.forEach((scheduleList) => {
          if (scheduleList.noScheduleId == d.noScheduleId) {
            scheduleCodeList = scheduleList.noScheduleName;
          }
        });


        me.scheduleList.forEach((scheduleList) => {
          if (scheduleList.timekeepingScheduleId == d.timekeepingScheduleId) {
            scheduleCodeList = scheduleCodeList + ' | ' + scheduleList.scheduleName;
          }
        });

        me.scheduleList.forEach((scheduleList) => {
          if (scheduleList.timekeepingScheduleId == d.timekeepingScheduleIdA) {
            scheduleCodeList = scheduleCodeList + ' | ' + scheduleList.scheduleName;
          }
        });

        me.scheduleList.forEach((scheduleList) => {
          if (scheduleList.timekeepingScheduleId == d.timekeepingScheduleIdB) {
            scheduleCodeList = scheduleCodeList + ' | ' + scheduleList.scheduleName;
          }
        });

        me.scheduleList.forEach((scheduleList) => {
          if (scheduleList.timekeepingScheduleId == d.timekeepingScheduleIdC) {
            scheduleCodeList = scheduleCodeList + ' | ' + scheduleList.scheduleName;
          }
        });
  
        me.scheduleList.forEach((scheduleList) => {
          if (scheduleList.timekeepingScheduleId == d.timekeepingScheduleIdD) {
            scheduleCodeList = scheduleCodeList + ' | ' + scheduleList.scheduleName;
          }
        });

        dtl.scheduleCode = scheduleCodeList
      
        me.isScheduleEditorVisible = false;
      }

    },

safeFormatDate(date) {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    return d.toISOString().slice(0, 10);
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

            me.timekeeping.cutOffDate = me.today;
            me.timekeeping.cutOffStartDate = me.today;
            me.timekeeping.cutOffEndDate = me.today;

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
      me.cutOff = info.cutOff;
      me.restDay = info.restDay;
  
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldTimekeeping = JSON.stringify(me.timekeeping);      
      me.oldSchedules = JSON.stringify(me.schedules);
    },

    getClientPayGroupInfo() {
      const me = this;
      return get("api/timekeeping-schedules-client-pay-group-id/" + me.timekeeping.clientPayGroupId );
    },

    getTimekeeping() {
      if (this.timekeeping.timekeepingSheetId < 0) {
        return Promise.resolve(null);
      }
      
      return get("api/timekeeping-schedules/" + this.timekeeping.timekeepingSheetId);
    },

    getReferences() {
      const me = this;
      if (me.schedules.length) {
        return Promise.resolve(true);
      }

      return get("api/references/pay0020");
    },

    getChangeLog(log) {
      return get("api/timekeeping-schedules/" + log + "/" +  this.timekeeping.timekeepingSheetId + "/log");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
        return ajax("api/timekeeping-schedules/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/timekeeping-schedules/" + this.timekeeping.timekeepingSheetId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const timekeeping = this.timekeeping,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");  
        return ajax("api/timekeeping-schedules/" + this.timekeeping.timekeepingSheetId + "/" + timekeeping.lockId + "/" + currentUserId,options );
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

      return this.isValidEntity("dbo.PayTimekeepingSheet", "TimekeepingSheetId", e.proposedValue,"TimekeepingSheetId" ).then((result) => { 

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

        me.setRequiredMode(
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
        );

        me.setDisplayMode(
          "clientPayGroupName",
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
      d.timekeepingScheduleId = 0;
      d.timekeepingScheduleIdA = 0;
      d.timekeepingScheduleIdB = 0;
      d.timekeepingScheduleIdC = 0;
      d.timekeepingScheduleIdD = 0;
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
    me.holiday = []; 
    me.payFreq = []; 
    me.period = []; 
    me.payrollPeriod = []; 
    me.memberList = [];
    me.cutOff = []; 
    me.restDay = []; 
    me.today = me.sym.dateInfo.serverDate;

    
  },


  computed: {
 
 
  excelDataTitle () {
        return 'Excel Data';
    },

filteredGroupedSchedules() {
    const keyword = this.searchMemberName
      ? this.searchMemberName.toLowerCase().trim()
      : '';

    if (!keyword) return this.groupedSchedules;

    return this.groupedSchedules.filter(member =>
      member.memberName &&
      member.memberName.toLowerCase().includes(keyword)
    );
  },
groupSchedulesByMemberAndDate() {
  const grouped = {};
  this.schedules.forEach(sch => {
    if (!sch.scheduleDate) return;

    const memberId = sch.memberId;
    const dateObj = new Date(sch.scheduleDate);

    if (isNaN(dateObj.getTime())) return;

    const scheduleDateKey = dateObj.toISOString();

    if (!grouped[memberId]) {
      grouped[memberId] = { memberId, memberName: sch.memberName, dates: {} };
    }

    grouped[memberId].dates[scheduleDateKey] = sch;
  });

  return Object.values(grouped);
},

  groupedSchedules() {
      const grouped = {};

      this.schedules.forEach((item, index) => {
        const key = item.memberId;

        if (!grouped[key]) {
          grouped[key] = {
            memberId: item.memberId,
            memberName: item.memberName,
            dates: {}
          };
        }

        grouped[key].dates[item.scheduleDate] = {
          ...item,
          originalIndex: index
        };
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
  const dateMap = new Map();

  this.schedules.forEach(s => {
    const date = typeof s.scheduleDate === 'string'
      ? s.scheduleDate
      : this.formatDate(s.scheduleDate); 

    if (!dateMap.has(date)) {
      dateMap.set(date, {
        scheduleDate: date,
        dayTypeCode: s.dayTypeCode || ''
      });
    }
  });


  return Array.from(dateMap.values()).sort(
    (a, b) => new Date(a.scheduleDate) - new Date(b.scheduleDate)
  );
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

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.box-field {
display: grid;
grid-template-rows:.5fr;
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
.schedule-editor-boxes{
  display: grid;
  grid-template-columns: .5fr 1fr .5fr .5fr ;
}
.schedule-editor-boxes-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
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
.table-scroller {
  overflow-x: auto;
  max-width: 91vw;
  height: 100%;
  max-height: 50vh;
  border: 1px solid #ddd;
} 


.id{
  width: 7rem;
}
.name{
  width: 27rem;
}
.date{
  width: 10rem;
}
.upload-buttons{
  display: flex;
  justify-content: flex-start;
  gap: .5rem;
  width: 50rem;
}
.schedule-code-clickable {
  font-weight: bold;
  
  color: black  ;
  padding: 2px 8px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}

.schedule-code-clickable:hover {
  color: rgb(47, 153, 47);
  text-decoration: underline;
  outline: none;
}
.schedule-code-clickable:active {
  color: rgb(47, 153, 47);
}
.schedule-code-clickable:focus {
  color: rgb(47, 153, 47);
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
  z-index: 1;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 1;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 25rem;
}
tbody td:nth-child(1) {
      position: sticky;
      left: 0;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
    tbody td:nth-child(2) {
      position: sticky;
      left: 100px;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }

.table-modal thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 1;
  border-right: 2px solid #005fa3;
  width: 8rem;
}
.table-modal thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 1;
  border-right: 2px solid #005fa3;
  width: 10rem;
}
.table-modal tbody td:nth-child(1) {
      position: sticky;
      left: 0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
.table-modal tbody td:nth-child(2) {
      position: sticky;
      left: 100px;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
}    
.schedule-code-clickable {
  font-weight: bold; 
  text-decoration: underline;
  text-align: center;
  text-wrap: wrap;
  background-color:  #6096d0;
  color: rgb(3, 3, 3);
  padding: 2px 8px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}  
.schedule-code-clickable:hover,
.schedule-code-clickable:focus {
  background-color: #0056b3;
  outline: none;
}
.justify-bold {
  font-weight: 700; 
}

.red-text {
  color: red;
  font-weight: bold;
}
.sunday-date {
  color: #d90429; 
  font-weight: bold;
}
.act-btn{
  display: flex;
  flex-direction: row;
}
.table-container{
  overflow: auto;
  height: 50vh;
  max-height: 50vh;
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
grid-template-rows:.5fr;
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
.schedule-editor-boxes{
  display: grid;
  grid-template-columns: .5fr 1fr .5fr .5fr ;
}
.schedule-editor-boxes-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
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
.table-scroller {
  overflow-x: auto;
  max-width: 90vw; 
  height: 100%;
  max-height: 50vh;
  border: 1px solid #ddd;
} 

table.scroller {

  min-width: 50vw; 
  border-collapse: collapse;
  width: 100vw;
  white-space: nowrap;
}

.id{
  width: 7rem;
}
.name{
  width: 27rem;
}
.date{
  width: 10rem;
}
.upload-buttons{
  display: flex;
  justify-content: flex-start;
  width: 50rem;
}
.schedule-code-clickable {
  font-weight: bold; 
  color: black  ;
  padding: 2px 8px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}

.schedule-code-clickable:hover {
  color: rgb(47, 153, 47);
  text-decoration: underline;
  outline: none;
}
.schedule-code-clickable:active {
  color: rgb(47, 153, 47);
}
.schedule-code-clickable:focus {
  color: rgb(47, 153, 47);
}

thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 1;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 1;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 25rem;
}
tbody td:nth-child(1) {
      position: sticky;
      left: 0;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
    tbody td:nth-child(2) {
      position: sticky;
      left: 100px;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
.schedule-code-clickable {
  font-weight: bold;           
  text-decoration: underline;
  text-align: center;
  text-wrap: wrap;
  background-color: #6096d0;
  color: white;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition:  0.2s ease;
  cursor: pointer;
}
    .schedule-code-clickable:hover,
.schedule-code-clickable:focus {
  background-color: #0056b3;
  outline: none;
}
.justify-bold {
  font-weight: 700; 
}

.red-text {
  color: red;
  font-weight: bold;
}
.sunday-date {
  color: #d90429; 
  font-weight: bold;
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

.applicantid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.box-field {
display: grid;
grid-template-rows:.5fr;
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
.schedule-editor-boxes{
  display: grid;
  grid-template-columns: .5fr 1fr .5fr .5fr ;
}
.schedule-editor-boxes-2{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
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
.table-scroller {
  overflow-x: auto;
  max-width: 89vw;
  height: 100%;
  max-height: 50vh;
  border: 1px solid #ddd;
} 
table.scroller {

  min-width: 50vw;
  border-collapse: collapse;
  width: 100vw;
  white-space: nowrap;
}

.id{
  width: 7rem;
}
.name{
  width: 27rem;
}
.date{
  width: 10rem;
}
.upload-buttons{
  display: flex;
  justify-content: flex-start;
  width: 50rem;
}
.schedule-code-clickable {
  font-weight: bold; 
  color: black  ;
  padding: 2px 8px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}

.schedule-code-clickable:hover {
  color: rgb(47, 153, 47);
  text-decoration: underline;
  outline: none;
}
.schedule-code-clickable:active {
  color: rgb(47, 153, 47);
}
.schedule-code-clickable:focus {
  color: rgb(47, 153, 47);
}

thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 1;
  background-color: #FFFFE0;
  border: 2px solid #005fa3;
  width: 8rem;
}
thead th:nth-child(2) {
  position: sticky;
  left: 100px;
  z-index: 1;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 25rem;
}
tbody td:nth-child(1) {
      position: sticky;
      left: 0;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
    tbody td:nth-child(2) {
      position: sticky;
      left: 100px;
      background-color: #FFFFE0;
      border-right: 2px solid #ddd;
      z-index: 2;
      width: 25rem;
      text-align: left;
    }
.schedule-code-clickable {
  font-weight: bold;
  text-decoration: underline;
  text-wrap: wrap;
  background-color: #007bff;
  color: white;
  padding: 2px 8px;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}  
.schedule-code-clickable:hover,
.schedule-code-clickable:focus {
  background-color: #0056b3;
  outline: none;
}
.justify-bold {
  font-weight: 700;  
}

.red-text {
  color: red;
  font-weight: bold;
}
.sunday-date {
  color: #d90429;
  font-weight: bold;
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
  grid-template-columns: .2fr 1.2fr .6fr .6fr 1fr ;
}
.schedule-editor-boxes{
  display: flex;
  flex-direction: column;
  flex-wrap: wrap
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
  height: 100%;
  max-height: 50vh;

}
.scroller{
  width: 130vw;
  
}

.member-id{
  width: 7rem;
}
.member-name{
  width: 25rem;
}
.sched{
  width: 10rem;
}
.actual-in{
  width: 10rem;
}
.actual-out{
  width: 10rem;
}
.actions{
  width: 7rem;
}

.schedule-code-clickable {
  font-weight: bold;            
  text-decoration: underline;
  text-align: center;
  background-color: #007bff;
  color: white;
  border-radius: 4px;
  cursor: pointer;
  user-select: none;
  display: inline-block;
  transition: background-color 0.2s ease;
  cursor: pointer;
}

.schedule-code-clickable:hover,
.schedule-code-clickable:focus {
  background-color: #0056b3;
  outline: none;
}
.justify-bold {
  font-weight: 700; 
}

.red-text {
  color: red;
  font-weight: bold;
}
.sunday-date {
  color: #d90429;
  font-weight: bold;
}
}
</style>
