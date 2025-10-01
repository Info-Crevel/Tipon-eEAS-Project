// Member Request Candidate List

<template>
<section class="container p-0" :key="ts">
<sym-form id="hrs0090" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" >
         <i class="fa fa-arrow-left mr-2"></i><span>Back</span>
       </button>
    </div>
  </div>

  <div class="box-container">
    <div class="box-date-column">

  <sym-text  v-model="memberRequestName" caption="Member Request Name"  :caption-width="50"   list="memberRequestNames" @changed="onMemberRequestIdChanged"></sym-text>
  <datalist id="memberRequestNames"><option v-for="item in memberRequestList" :key="item.memberRequestId" :value="item.memberRequestName" @input="e => memberRequestName = e.toUpperCase()" class="dropdown"></option></datalist> 

    <sym-text  v-model="memberName" caption="Member Name"  :caption-width="35"   list="names" @changed="onMemberNameChanged"></sym-text>
    <datalist id="names"><option v-for="item in memberReferenceList" :key="item.applicantDetailId" :value="item.memberName" @input="e => memberName = e.toUpperCase()" ></option></datalist> 
    </div>  
    <hr class="info-light darken-1 mb-0 mt-0 fh-1" v-show="memberCount >= 0" />
    <p class="display-10 bold text-center   my-1" v-show="memberCount > 0">{{ core.toIntegerFormat(memberCount, false) }} Member(s) found.</p>
    <p class="display-10 bold text-center my-1" v-show="memberCount < 1"> No record found.</p>
    <hr class="info-light darken-1 mt-0 mb-1 fh-1" v-show="memberCount >= 0" />
    <div class="export-buttons mb-1">
    <button type="button" class="success  w-100 text-center lg-2 p-1 shadow-soft" @click="generateExcelReport" ><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>
    </div>
    <hr class="info-light darken-1 mt-0 mb-1 fh-1" />
    <div class="box-2 success-light ">
        <div></div>
        <div class="box-3">

       
            <sym-pager
            v-model="currentPage"
            :totalPages="totalPages"
            align="center"
            @change="onPageChange"
            @pager:rangeChanged="rangeStart = $event"
            v-if="currentPage && memberCount > 0"        
            class=" curved pt-2 px-2"
          >
          </sym-pager>
        </div>
    
        <div class="box-4">
          <sym-combo v-model="rowPerPageId" align="left" :caption-width="35" caption="Item Per Page" :input-width="30" display-field="rowPerPageName" :datasource="rowPerPage" @changed="onRowPerPageIdChanged"></sym-combo>
      
        </div>
        
    </div>

    <div class="table-scroll-wrapper">
      <div class="fixed-header">
        <table class="table-scroll light striped-even mt-2" v-show="memberList ">
          <thead>
            <tr>
              <th class="w-7">Applicant ID</th>

              <th class="w-7">Member ID</th>
              <th class="w-10">Employee Id</th>
              <th class="w-30">Candidate Name</th>
              <th class="w-15">App Status</th>
              <th class="w-15">Curent Screening</th>
             <th class="w-10">Action</th>

              <th class="w-7">MRF #</th>
              <th class="w-30">Member Request Name</th>
            </tr>
          </thead>
          <tbody class="white">
            <tr v-for="(dtl, index) in memberList" :key="index">
              <td>{{ dtl.applicantDetailId }}</td>

              <td>{{ dtl.memberId }}</td>
              <td>{{ dtl.memberEmployeeId }}</td>
              <td>{{ dtl.memberName }}</td>
              <td> {{ dtl.applicantStatusName }} </td>
              <td> {{ dtl.applicantScreeningName }} </td>

                          <td class="p-1">
                            <div class="d-flex justify-center" sm-1 mb-0 >
                              <button :disabled="dtl.applicantStatusId!==1" type="button" class="justify-between success w-50"  @click="onEditScreening(dtl, index)">
                                <i class="fa fa-check fa-lg"></i><span> Edit</span>
                              </button>
                            </div>
                          </td>


              <td>{{ dtl.memberRequestId }}</td>

              <td class="group-name">{{ dtl.memberRequestName }}</td>

            </tr>
          </tbody>
        </table>
        
    
      </div>

    </div>

  </div>
        <sym-pager
          v-model="currentPage"
          :totalPages="totalPages"
          align="center"
          @change="onPageChange"
          @pager:rangeChanged="rangeStart = $event"
           v-if="currentPage && memberCount > 0" 
          class="success-light darken-1 curved pt-2 px-2 mt-2"
        >
        </sym-pager>

  <div class="d-flex justify-center mt-3" v-if="!memberCount" >
    <sym-alert class="info w-100 text-center" icon="none">
      <span>No records found. File is empty.</span>
    </sym-alert>
  </div>


<!-- Candidate -->
<sym-modal
  id="candidate-editor"
  v-model="isCandidateEditorVisible"
  size="md"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorCandidateTitle"
  @show="onShowCandidateEditor($event)"
  @hide="onHideCandidateEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0050X" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="vaccine-editor-boxes">


                      <table class="pooling-scroll light striped-even mb-0 ">
                      <thead class="pooling-thead">
                    <tr>
                      <th class="id">Screening ID</th>
                      <th class="name">Screening Name</th>
                      <th class="action">Action</th>
                    </tr>
                  </thead>
                  <tbody class="pooling-tbody">
                    <tr v-for="(dtl, index) in applicantScreenings" :key="index">
                       <td>{{ dtl.screeningDetailId }}</td>
                      <td class="name">{{ dtl.applicantScreeningName }}</td>                   
                         <td class="p-1">
                <div class="buttons gap" sm-1 mb-0 >
                
                  <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteScreening(index)">
                    <i class="fa fa-times fa-lg"></i>
                  </button>

                </div>
              </td>
                    </tr>
                  </tbody>
                </table>
        <div class="app-grid-column-2 gap">
        </div>
 
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitScreening()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isCandidateEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>
  </div>

  </sym-modal>


</sym-form>
</section>
</template>

<script>

import {
  get,
  ajax
} from '../../js/http';

import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";

import "jspdf-autotable";
import { saveAs } from "file-saver";
import ExcelJS from 'exceljs';


import {
  getList,
} from "../../js/dbSys";

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'hrs0090',

  data () {
    return {
      memberList: [],
      memberReferenceList: [],
      report: [],
      memberRequestList: [],
      memberName:'',
      sssFlag:false,
      pgbFlag:false,
      phhFlag:false,
      whtFlag:false,
      
      currentPage: 1,
      totalPages: 0,
      memberRequestId:0,
      memberRequestName:'',
      memberCount: 0,
      rowPerPageId: 0,

      candidateList : [],
      newCandidateList : [],
      candidate : {
      applicantDetailId: 0,
      memberRequestId: 0,
      name3: '',
      memberId: 0,
      memberName: '',
      applicantScreeningId: 0,
      screeningStatusId: 0,
      applicantStatusId: 0,
      remarks:'',
      hiredDate: null,
      deploymentDate: null,
      screeningFlag: 0,
      docTypeCount: 0,
      docTypeName: '',
      lockId: '',
    },

    applicantScreenings: [],

    applicantScreening: {
      screeningDetailId:0,
         applicantScreeningName:'',
               applicantScreeningId:0,

    },

    candidateIndex: -1,
      isAddingCandidate: false,
      isCandidateEditorVisible: false,
   
      isCandidateInProcessVisible: false,
      isCandidateHireVisible: false,

applicantScreeningIndex: -1,

    };
  },

  computed: {
  
    detailTag () {
      return 'Member/s: '+ this.memberList.length;
    },

    editorCandidateTitle () {
      if (this.isAddingCandidate) {
        return 'Add Candidate Detail';
      }
      return 'Candidate Progress';
    },

  },

  methods: {


    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }
      let
        promise,
        message,
        wait = me.wait(),
        isNew = me.isNew();

        promise = me.modifyRecord();

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (isNew && typeof success === 'number' && success > 0) {
              me.applicantScreenings.applicantDetailId = success;
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

    onSubmitScreening() {
      this.onSubmit();
    },

        onDeleteScreening (index) {
      const
        me = this,
        d = me.applicantScreening,
        dtl = me.applicantScreenings[index];

      me.applicantScreeningIndex = index;



       me.applicantScreenings.splice(index, 1);
    },



    onHideCandidateEditor () {
      const me = this;
    },

    onShowCandidateEditor () {
      const me = this;

      me.setActiveModel('candidate');
    },



      onEditScreening(dtl, index) {
        const d = this.candidate;
        this.candidateIndex = index;

        this.isCandidateInProcessVisible = true;
        this.isCandidateHireVisible = false;

        d.applicantDetailId = dtl.applicantDetailId;
        d.memberId = dtl.memberId;
        d.memberRequestId = dtl.memberRequestId;
      

        this.getApplicantScreening(d.applicantDetailId, d.memberRequestId).then(
        (data) => {
          this.applicantScreenings = data.screening;
          this.refresh();  
        },
        (fault) => {
          this.showFault(fault);
        }
      )

        d.applicantScreeningId = dtl.applicantScreeningId;
        d.applicantScreeningName = dtl.applicantScreeningName;
  
        d.lockId = dtl.lockId;

        this.isCandidateEditorVisible = true;

      },


     exportToCSV() {
      const headers = ['MemberId', 'EmployeeId', 'MemberLastName', 'MemberFirtName', 'MemberMiddleName'];
      const rows = this.memberList.map(member => [
        member.MemberId,
        member.EmployeeId,
        member.MemberLastName,
        member.MemberFirtName,
        member.MemberMiddleName
      ]);

      let csvContent = "data:text/csv;charset=utf-8,"
        + [headers.join(","), ...rows.map(r => r.join(","))].join("\n");

      const encodedUri = encodeURI(csvContent);
      const link = document.createElement("a");
      link.setAttribute("href", encodedUri);
      link.setAttribute("download", "pay_group_member_list.csv");
      document.body.appendChild(link);
      link.click();
    },

async generateExcelReport() {
  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet('Pay Group Members');

  const titleFill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FF4472C4' }, 
  };
  const titleFont = { bold: true, size: 16, color: { argb: 'FFFFFFFF' } }; 

  const headerFill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FFD9E1F2' },
  };
  const headerFont = { bold: true };
  const borderStyle = { style: 'thin', color: { argb: 'FF000000' } };
  const border = {
    top: borderStyle,
    left: borderStyle,
    bottom: borderStyle,
    right: borderStyle,
  };

  let currentRow = 1;
  worksheet.mergeCells(`A${currentRow}:E${currentRow}`);
  const titleCell = worksheet.getCell(`A${currentRow}`);
  titleCell.value = 'Pay Group Member List';
  titleCell.fill = titleFill;
  titleCell.font = titleFont;
  currentRow++;
worksheet.mergeCells(`A${currentRow}:E${currentRow}`);
const payGroupCell = worksheet.getCell(`A${currentRow}`);
payGroupCell.value = this.memberRequestName && this.memberRequestName !== 'undefined'
  ? this.memberRequestName
  : 'ALL';
payGroupCell.font = { bold: true, color: { argb: 'FF000000' } };
payGroupCell.alignment = { horizontal: 'center', vertical: 'middle' };
currentRow++;
  currentRow++;

  // Header row
  const headers = ['Member ID', 'Employee #', 'Last Name', 'First Name', 'Middle Name'];
  headers.forEach((header, index) => {
    const cell = worksheet.getCell(currentRow, index + 1);
    cell.value = header;
    cell.fill = headerFill;
    cell.font = headerFont;
    cell.border = border;
    cell.alignment = { horizontal: 'center', vertical: 'middle' };
  });
  currentRow++;

  // Data rows
  this.report.forEach(member => {
    const row = worksheet.getRow(currentRow);
    row.getCell(1).value = member.memberId;
    row.getCell(2).value = member.employeeId;
    row.getCell(3).value = member.memberLastName;
    row.getCell(4).value = member.memberFirstName;
    row.getCell(5).value = member.memberMiddleName;

    // Apply border to each cell in the row
    for (let i = 1; i <= 5; i++) {
      const cell = row.getCell(i);
      cell.border = border;
      cell.alignment = { horizontal: 'left', vertical: 'middle' };
    }

    currentRow++;
  });

  currentRow++;

  worksheet.mergeCells(`A${currentRow}:D${currentRow}`);
  const totalLabelCell = worksheet.getCell(`A${currentRow}`);
  totalLabelCell.value = 'Total Members:';
  totalLabelCell.font = { bold: true };
  totalLabelCell.alignment = { horizontal: 'right', vertical: 'middle' };

  const totalValueCell = worksheet.getCell(`E${currentRow}`);
  totalValueCell.value = this.report.length;
  totalValueCell.font = { bold: true };
  totalValueCell.alignment = { horizontal: 'left', vertical: 'middle' };

  const colWidths = [15, 15, 20, 20, 20];
  colWidths.forEach((width, i) => {
    worksheet.getColumn(i + 1).width = width;
  });

  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], { type: 'application/octet-stream' });
  saveAs(blob, 'PayGroupMemberList.xlsx');
},

    exportToPDF() {
      const doc = new jsPDF();
      const columns = ['MemberId', 'EmployeeId', 'Last Name', 'First Name', 'Middle Name'];
      const rows = this.memberList.map(member => [
        member.MemberId,
        member.EmployeeId,
        member.MemberLastName,
        member.MemberFirtName,
        member.MemberMiddleName
      ]);

      doc.text("Pay Group Member List", 14, 15);
      autoTable(doc, {
        startY: 20,
        head: [columns],
        body: rows,
      });
      doc.save("pay_group_member_list.pdf");
    },

    printReport() {
      const printContent = document.querySelector(".report-table").outerHTML;
      const printWindow = window.open('', '', 'height=600,width=800');
      printWindow.document.write('<html><head><title>Pay Group Member List</title></head><body>');
      printWindow.document.write(printContent);
      printWindow.document.write('</body></html>');
      printWindow.document.close();
      printWindow.print();
    },
    
    onMemberRequestIdChanging (e) {
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback (e) {
      const me = this;
      let filter = "MemberRequestId='" + e.proposedValue + "'";
      return getList('dbo.QArsMemberRequestSheet', 'MemberRequestId, MemberRequestName', '', filter).then(
        payGroup => {
          if (payGroup && payGroup.length) {
            me.memberRequestId = payGroup[0].memberRequestId;
            me.memberRequestName = payGroup[0].memberRequestName;
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


    onMemberRequestIdLostFocus () {
      const me = this;

      if (!me.memberRequestId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.memberRequestId = data.memberRequestId;
      me.memberRequestName = data.memberRequestName;
    },

    onMemberRequestIdChanged (){
      this.onSearch();
    },

    onRowPerPageIdChanged (){
      this.onSearch();
    },

    onMemberNameChanged (){
      this.onSearch();
    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.memberName) {
        q.memberName = me.memberName;
      }

      if (me.memberRequestName) {
        q.memberRequestName = me.memberRequestName;
      }

      q.page = me.currentPage;

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;

      if ("memberName" in q) {
        me.memberName = q.memberName;
      }
      
      if ("memberRequestName" in q) {
        me.memberRequestName = q.memberRequestName;
      }

      if ("page" in q && me.core.isInteger(q.page)) {
        me.currentPage = parseInt(q.page);
      }
    },

    onClickMember(memberId) {
      const me = this;

      let route = {
        name: "hrs0010",
        query: {
          memberId: memberId,
          activeTabIndex: 1 
        },
      };
      me.go(route);
    },

    setupControls () {
      const me = this;
      setTimeout(() => {
        me.setFocus('memberRequestName');
      }, 100);

    },

    onResetFilter() {
      const me = this;
      me.memberName = '';
      me.memberRequestName = '';
      me.onSearch();
    },

    onSearch() {
 
      const me = this;
      me.memberCount = 0;
      this.checkCount = -1;
      me.currentPage = 1;
      me.loadData();
    },

    loadData () {
      const
        me = this,
        wait = me.wait();
        
        me.itemsPerPage = me.rowPerPageId 
        
        me.getMemberCount()
          .then((count) => {
            if (!count) {
              return [];
            }
            me.memberCount = count;
            me.totalPages = Math.ceil(me.memberCount / me.itemsPerPage);
            return me.getMemberList();
          })
          .then((memberList) => {
            me.stopWait(wait);
            me.memberList = memberList.memberList;
            me.memberReferenceList = memberList.memberReferenceList;            
            me.report = memberList.report;            
            me.memberRequestList = memberList.memberRequestList;  
            me.rowPerPage = memberList.rowPerPage;
            me.replaceUrl();
            me.setupControls();

          })
          .catch((fault) => {
            me.stopWait(wait);
            me.showFault(fault);
          });
    },

    getMemberCount() {
      const me = this;

      let q = this.getSearchQuery();

      if (q) {
        return get("api/member-request-monitoring/count?" + q);
      } else {
        return get("api/member-request-monitoring/count");
      }
    },

    onPageChange(page) {
      this.currentPage = page;
      this.reload();
    },

    getMemberList() {
      const me = this;

      let q = "ipp=" + (me.itemsPerPage || 10);
      q = q + "&page=" + (me.query.page || 1);

      let sq = me.getSearchQuery();
      if (sq) {
        return get("api/member-request-monitoring/inquiry?" + me.getSearchQuery() + "&" + q);
      } else {
        return get("api/member-client-pay-group-monitoring/inquiry?" + q);
      }
    },

    getSearchQuery() {
      const me = this;
      let q = "";
      
      q = "memberName=" + me.query.memberName;
      
      if (me.memberRequestName) {
        if (q) {
          q += "&";
        }
        q += "memberRequestName=" + me.query.memberRequestName;
      }

      return q;
    },

    getApiPayload () {
      const
        me = this,
        applicantScreening = {};

      Object.assign(applicantScreening, me.applicantScreening);
      applicantScreening.applicantScreening = me.applicantScreening;

      return applicantScreening;
    },


    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/member-request-candidate-screenings/' + currentUserId, options);
    },


    getApplicantScreening(applicantDetailId,memberRequestId) {
      return get("api/member-request-candidate-screenings/" + applicantDetailId + "/" + memberRequestId);

    },



    refreshOldRefs () {
      const me = this;
      me.oldDetail = JSON.stringify(me.detail);
    },
 
    onRefresh () {
      this.loadData();
    },

  },

  created () {
    const me = this;
    me.itemsPerPage = 15;
    me.rowPerPageId = 10;
    me.rowPerPage = [];
  },

  mounted () {
    const me = this;
    me.currentPage = 1;
    me.syncValues(me.params, me.query);
    me.loadData();

  },

  beforeRouteUpdate(to, from, next) {
    this.syncValues(to.params, to.query);
    next();
    this.loadData();
  },

}

</script>

<style scoped>
.Header {
  width: 100%;
  border: 0;
  padding: 5px;
  text-align: center;
  font-weight: bold;
  color: white;
  
  text-transform: uppercase;
}
.action-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.record-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 2fr 7fr 2fr;
  gap: .5rem;
}

.fixed-header {
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
  display: flex;
  justify-content: space-evenly;
  flex-wrap: nowrap;
}
.box-container{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;

}
.box-column{
  display: grid;
  grid-template-columns: .3fr 1fr 1fr;
  gap: .5rem;
}
.box-date-column{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.box-name-column{
  display: grid;
  grid-template-columns: 1fr .6fr .4fr 1fr 1fr;
  gap: .5rem;
}
.box-table-container{
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  margin-top: 1rem;
  
}
.table-scroll-wrapper {
  overflow-x: auto;
}

.table-scroll {
  white-space: nowrap;
  max-width: 150%;
  width: 115vw;
}
.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}

.dropdown {
  width: 100%;
  position: relative;
  left: 0;
  padding: 10px;
  background-color: seagreen;
  border: 1px solid black;
}
.box-2 {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
}
.box-4{
  display: flex;
  justify-content: flex-end;
  margin-top: 5px;
}
</style>