// Member Request Form 

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars0030" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

  
  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
      <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>

        <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onSetCanceled" v-if="canSetCanceled && !isCancelled && !isActivated && !isClosed">
          <i class="fa fa-thumbs-o-down fa-lg"></i><span>Cancel</span>
        </button>

        <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onSetActive" v-if="canSetActive && !isActivated && isNotActive">
          <i class="fa fa-thumbs-o-up fa-lg"></i><span>Activate</span>
        </button>
      </div>


    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save w-30" @click="onSubmit" >
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear w-30" @click="onClear">
        <i class="fa fa-undo fa-lg"></i><span>Clear</span>
      </button>
    </div>
    <div></div>

  </div>
  
  <div class="header-containerX">
     <div class="mrf-id">
      <sym-int v-model="request.memberRequestId" :caption-width="20" caption="MRF ID" lookupId="ArsMemberRequest" @lostfocus="onMemberRequestIdLostFocus" @changing="onMemberRequestIdChanging" @changed="onMemberRequestIdChanged" @searchresult="onMemberRequestIdSearchResult" @searchfill="onMemberRequestIdSearchFill"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
   
    </div>
      <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
      <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActivated && !isClosed">Active</sym-tag>
      <sym-tag class="warning text-center border-light lg-2 ml-3" :width="38" v-if="isNotActive">In-Active</sym-tag>
      <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isClosed && request.memberRequestId">Completed</sym-tag>
 
      <sym-tag v-if="request.memberRequestId" class="info text-center border-light lg-2 ml-3" :width="70">Total Hired : {{request.totalHired}}</sym-tag>
      <sym-tag v-if="request.memberRequestId" class="success text-center border-light lg-2 ml-3" :width="70">Current Vacancy : {{request.totalVacancy}}</sym-tag>

    
      <div class="buttons d-inline ml-auto">
        <button class="btn-copy border-main justify-between fw-28 primary mb-2" type="button" :class="copyButtonClass" :disabled="request.memberRequestId==-1"   @click="onCopy">
          <i class="fa fa-copy fa-lg"></i><span>Copy</span>
        </button>
      </div>

    </div>




    <sym-tabs id="request-info-tabs" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">


      <sym-tab title="Member Request Information" icon="vcard-o">
        <div class="main-container ">
          <div class="sub-container ">
            <div class="container-1 gap">

                 
                <div class="app-box-style box-field"> 
                  <div class="box-1">
                      <sym-text v-model="request.memberRequestName" align="bottom" :caption-width="30"  caption="MRF Name"></sym-text>
                      <sym-text v-model="request.clientPosition" align="bottom" :caption-width="10" caption="Client Position"></sym-text>
                      <sym-int v-model="request.memberRequestPositionId" align="bottom" :caption-width="34" caption="Internal Position ID" display-field="memberRequestPositionName" :datasource="position" lookupId="ArsMemberRequestPosition" @changing="onMemberRequestPositionIdChanging" @searchresult="onMemberRequestPositionIdSearchResult"></sym-int> 
                      <sym-text v-model="request.memberRequestPositionName" align="bottom" :caption-width="10" caption="Internal Position"></sym-text>
                       
                    </div>
                  
                    <div class="box-2">
                      <sym-memo v-model="request.jobDescription" align="bottom" :caption-width="10" caption="Job Description"></sym-memo>
                      <sym-memo v-model="request.positionKeyword" align="bottom" :caption-width="10" caption="Position (As stated in Work Experience, Type each position, separated by a semicolon [;])"></sym-memo>
                      <sym-memo v-model="request.skillNameKeyword" align="bottom" :caption-width="10" caption="Skill (As stated in Skill Definition,Type each skill, separated by a semicolon [;])" ></sym-memo>
                      
                    </div>   
                  
                    <div class="box-3">
                        <sym-int v-model="request.clientPayGroupId" align="bottom" :caption-width="34" caption="Pay Group ID" display-field="clientPayGroupName" :datasource="payGroups" lookupId="ArsClientPayGroupActive" @changing="onClientPayGroupIdChanging" @searchresult="onClientPayGroupIdSearchResult" ></sym-int> 
                        <sym-text v-model="request.clientPayGroupName" align="bottom" :caption-width="10" caption="Pay Group Name" ></sym-text>
                        <sym-combo v-model="request.payoutTypeId" align="bottom" :caption-width="46" caption="Payout Type" display-field="payoutTypeName" :datasource="payout" ></sym-combo>
                        <sym-combo v-model="request.memberTypeId" align="bottom" :caption-width="46" caption="Member Type" display-field="memberTypeName" :datasource="type" @changed="onMemberTypeIdChanged"></sym-combo>
                        <sym-combo v-model="request.employmentTypeId" align="bottom" :caption-width="46" caption="Employment Type" display-field="employmentTypeName" :datasource="employmentTypeList"></sym-combo>
                        <sym-date v-model="request.employmentEndDate"  align="bottom" :caption-width="35" caption="Service End Date" v-show="request.employmentTypeId === 3 || request.employmentTypeId === 4 || request.employmentTypeId === 5 || request.employmentTypeId === 6"></sym-date>
                        <sym-date v-model="request.deploymentDate" align="bottom" :caption-width="60" caption="Target Deployment Date"  @changing="onDeploymentDateChanging"></sym-date>
                  
                    </div>


            <div class="box-8">
              <sym-text v-model="request.clientName" align="bottom" :caption-width="34" caption="Name 1"></sym-text> 
              <sym-text v-model="request.name2" align="bottom" :caption-width="34" caption="Name 2"></sym-text> 
              <sym-text v-model="request.name3" align="bottom" :caption-width="34" caption="Name 3"></sym-text> 
              <sym-text v-model="request.name4" align="bottom" :caption-width="34" caption="Name 4"></sym-text> 
              <sym-text v-model="request.name5" align="bottom" :caption-width="34" caption="Name 5"></sym-text> 
              <sym-text v-model="request.name6" align="bottom" :caption-width="34" caption="Name 6"></sym-text> 
              <sym-text v-model="request.name7" align="bottom" :caption-width="34" caption="Name 7"></sym-text> 
            </div>



                    <div class="box-4">
                      <sym-combo v-model="request.requestTypeId" align="bottom" :caption-width="46" caption="Request Type" display-field="requestTypeName" :datasource="requestType"></sym-combo>  
                      <sym-combo v-model="request.educationLevelId" align="bottom" :caption-width="46" caption="Education Level" display-field="educationLevelName" :datasource="educationLevels" ></sym-combo>
                      <sym-int v-model="request.workingDays" align="bottom" :caption-width="10" caption="Working Days"></sym-int>            
                      <sym-int v-model="request.vacancyCount" align="bottom" :caption-width="10" caption="Vacancy Count"></sym-int>
                      <sym-check v-model="request.minimumWageFlag" align="bottom" :caption-width="35" caption="Minimum Wager" caption-align="center" check-align="center" @changing="onMinimumWageFlagChanging"  @changed="onMinimumWageFlagChanged"></sym-check>    
                      <sym-int v-model="request.memberId" align="bottom" :caption-width="50" caption="Member ID " display-field="memberName" lookupId="ArsMemberRequestActiveMember" @changing="onMemberIdChanging" @searchresult="onMemberIdSearchResult"></sym-int> 
                      <sym-text v-model="request.memberName" caption="Immediate Head" align="bottom"></sym-text>

                    </div>

                    <div class="box-5">
                    <sym-memo v-model="request.memberRequestPerks" align="bottom" :caption-width="10" caption="Additional Perks/Benefits"></sym-memo>
                    <sym-memo v-model="request.memberRequestRemarks" align="bottom" :caption-width="10" caption="Additional Qualifications/Requirements"></sym-memo>
                  </div>

                  </div>
                <div class="app-grid-column-4 gap">
                    <!-- DocType -->
                    <div class="d-inline-block border-soft w-100 mb-3 app-box-style gap">
                      <div class="Header app-form-header curved-1">Requirement Submission</div>
                      <div class="table-container">

                      
                          <sym-table class="table-scroller striped-odd bill hover mb-0" colorClass="light">
                            <sym-table-header slot="header">
                              <tr class="align-top">
                                <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                                <th class="w-90">Documents</th>
                              </tr>
                              
                            </sym-table-header>
                              <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                                <tr v-for="(docType, index) in docTypes" :key="index" class="align-top" @click="onToggleDocTypeSelection(docType)">
                                  <td class="w-10 text-center"><input type="checkbox" :checked="isDocTypeSelected(docType)"></td>
                                  <td class="w-90">{{ docType.docTypeName }}</td>
                                </tr>
                              </sym-table-body>
                          </sym-table>
                      </div>    
                    </div> 

                    <!-- Type Qualification-->
                
                      <div class="d-inline-block border-soft w-100 mb-3 app-box-style gap">
                      <div class="Header app-form-header curved-1" > Qualification Type</div>
                        <div class="table-container">
                            <sym-table  class="table-scroller striped-odd bill hover mb-0" colorClass="light">
                            <sym-table-header slot="header">
                              <tr class="align-top">
                                <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                                <th class="w-90">Type Qualification</th>
                              </tr>
                              </sym-table-header>
                                <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                                  <tr v-for="(typeQualification, index) in typeQualificationList" :key="index" class="align-top" @click="onToggleTypeQualificationSelection(typeQualification)">
                                    <td class="w-10 text-center"><input type="checkbox" :checked="isTypeQualificationSelected(typeQualification)"></td>
                                    <td class="w-90">{{ typeQualification.typeQualificationName }}</td>
                                  </tr>
                                </sym-table-body>
                          </sym-table>
                        </div>
                      
                      </div>
             
                    <!-- Screening-->
              
                      <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
                      <div class="Header app-form-header curved-1">HIRING PROCESS</div>
                        <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
                            <sym-table-header slot="header">
                              <tr class="align-top">
                                <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                                <th class="w-90">Screening Name</th>
                              </tr>
                              
                            </sym-table-header>
                              <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                                <tr v-for="(applicantScreening, index) in applicantScreenings" :key="index" class="align-top" @click="onToggleApplicantScreeningSelection(applicantScreening)">
                                  <td class="w-10 text-center"><input type="checkbox" :checked="isApplicantScreeningSelected(applicantScreening)"></td>
                                  <td class="w-90">{{ applicantScreening.applicantScreeningName }}</td>
                                </tr>
                              </sym-table-body>
                        </sym-table>
                    </div>   
              
                       
                    <!-- Medical Result -->

                      <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
                        <div class="Header app-form-header curved-1">Medical Result</div>
                        <div class="table-container">
                           <sym-table class="table-scroller striped-odd bill hover mb-0" colorClass="light">
                            <sym-table-header slot="header">
                              <tr class="align-top">
                                <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                                <th class="w-90">Type</th>
                              </tr>
                            </sym-table-header>
                              <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                                <tr v-for="(medicalResultType, index) in medicalResultTypes" :key="index" class="align-top" @click="onToggleMedicalResultSelection(medicalResultType)">
                                  <td class="w-10 text-center"><input type="checkbox" :checked="isMedicalResultSelected(medicalResultType)"></td>
                                  <td class="w-90">{{ medicalResultType.medicalResultTypeName }}</td>
                                </tr>
                              </sym-table-body>
                            </sym-table>
                        </div>
          
                     </div>
                    
                </div>
            </div> 

                <div class="container-2 gap">
                  <div class="app-box-style gap wages-field">
                    <div class="wage-btns gap">
                        <button type="button" :class="logButtonClass" class="check-rate-btn" @click="onCheckBasic"><i class="fa fa-search fa-lg"></i><span class="bold">Check Rate</span>
                        </button>
                        <button type="button" :class="logButtonClass" class="post-basic-btn" @click="onPostBasic">
                          <i class="fa fa-plus-circle fa-lg"></i><span class="bold">Post Minimum Wage</span>
                        </button>
                        <button type="button" :class="logButtonClass" class="recalculate-btn" @click="onCheckRateDetails">
                          <i class="fa fa-calculator fa-lg"></i><span class="bold">Recalculate</span>
                        </button>
                    </div>  
                    <div></div>
                    <div></div>
                  <div class="box-6">
                        <!-- Minimum Wage -->
                    <div class="app-box-style gap">
                      
                      <div class="Header app-form-header curved-1">Minimum Wage List</div>
                   
                        <div class="table-field-sroller">
                          <table class="light striped-even mb-0 table-fixed-header">
                            <thead>
                              <tr>
                              <th class="w-20">Date</th>
                              <th class="w-30">Rate</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr v-for="(dtl, index) in minimumWageList" :key="index" v-show="request.memberRequestId !=0">
                              <td>{{ dtl.sysDate }}</td>
                              <td>{{ dtl.rate }}</td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                   
                    </div>

                    <!-- Pay Out -->
                    <div class="app-box-style gap">
                      <div class="Header app-form-header curved-1">PayOut</div>
              
            
                        <div class="table-field-sroller">
                            <table class="light striped-even mb-0 table-fixed-header ">
                            <thead >
                              <tr>
                                <th class="text-center w-40">Transaction</th>
                                <th class="text-right w-20">Daily</th>
                                <th class="text-right w-20">Monthly</th>
                                <!-- <th class="text-right w-20">Fixed</th> -->
                                <th class="text-center W-20">Action</th>
                              </tr>
                            </thead>
                            
                              <tbody  >
                                <tr v-for="(dtl, index) in payOuts" :key="index" v-show="request.memberRequestId !=0">
                                  <td>{{ dtl.payTrxName }}</td>
                                  <td class="text-right">{{ core.toDecimalFormat(dtl.dailyRate) }}</td>
                                  <td class="text-right">{{ core.toDecimalFormat(dtl.monthlyRate) }}</td>
                                  <!-- <td class="text-center">
                                    <i :class="getBooleanIconClass(dtl.fixedFlag)"></i>
                                  </td> -->
                                  <td class="p-1">
                                    <div class="act-btns" sm-1 mb-0 >
                                      <button type="button" class="justify-between info fw-22" @click="onEditPayOut(dtl, index)" v-show="!dtl.payGroupFlag">
                                        <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                      </button>
                                      <button type="button" class="warning" title="Delete" @click="onDeletePayOut(index)" v-show="!dtl.payGroupFlag">
                                        <i class="fa fa-times fa-lg"></i>
                                      </button>
                                    </div>
                                  </td>
                                </tr>
                                            
                              </tbody>
                             <tfoot v-show="request.memberRequestId != 0">
                              <tr>
                                <td class="text-right bold">Total</td>
                                <td class="text-right">{{ core.toDecimalFormat(payOutDailyTotalRate) }}</td>
                                <td class="text-right">{{ core.toDecimalFormat(payOutMonthlyTotalRate) }}</td>
                                <td></td>
                              </tr>
                            </tfoot>
                          </table>
                        </div>
                    
                      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                      <div></div>
                        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                          <button type="button" class="justify-between btn-add" @click="onAddPayOut"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                        </div>
                      </div>
                    </div>    



                    <!-- Billing -->
                    <div class="app-box-style gap">
                      <div class="Header app-form-header curved-1">Billing</div>
                      
                      
                      <div class="table-field-sroller">
                        <table class="light striped-even mb-0 table-fixed-header">
                        <thead>
                          <tr>
                            <th class="text-center w-40">Transaction</th>
                            <th class="text-right w-20">Daily</th>
                            <th class="text-right w-20">Monthly</th>
                            <!-- <th class="text-right w-20">Fixed</th> -->
                            <th class="text-center W-20">Action</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(dtl, index) in billings" :key="index" v-show="request.memberRequestId !=0">
                            <td>{{ dtl.payTrxName }}</td>
                            <td class="text-right">{{ core.toDecimalFormat(dtl.dailyRate) }}</td>
                            <td class="text-right">{{ core.toDecimalFormat(dtl.monthlyRate) }}</td>
                              <!-- <td class="text-center">
                                    <i :class="getBooleanIconClass(dtl.fixedFlag)"></i>
                                  </td> -->
                            <td class="p-1">
                              <div class="act-btns" sm-1 mb-0 >
                                <button type="button" class="justify-between info fw-22" @click="onEditBilling(dtl, index)" v-show="!dtl.payGroupFlag">
                                  <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                </button>
                                <button type="button" class="warning" title="Delete" @click="onDeleteBilling(index)" v-show="!dtl.payGroupFlag">
                                  <i class="fa fa-times fa-lg"></i>
                                </button>
                              </div>
                            </td>
                          </tr>
                        </tbody>
                          <tfoot v-show="request.memberRequestId != 0">
                              <tr>
                                <td class="text-right bold">Total</td>
                                <td class="text-right">{{ core.toDecimalFormat(billingDailyTotalRate) }}</td>
                                <td class="text-right">{{ core.toDecimalFormat(billingMonthlyTotalRate) }}</td>
                                <td></td>
                              </tr>
                            </tfoot>
                      </table>
                      </div>
                   
                      
                      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                        <div></div>

                          <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                            <button type="button" class="justify-between btn-add" @click="onAddBilling"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                          </div>

                          <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                            <button type="button" class="justify-between btn-copy" @click="onCopyPayOut"><i class="fa fa-copy fa-lg"></i><span>Copy Pay Out</span></button>
                          </div>


                        </div>
                    </div>
                  </div>    
                  </div>

                  <div class="box-7">  
                    <!-- NCII -->
                    <div class="app-box-style gap">
                    <div class="Header app-form-header curved-1">Certification</div>
                    
                    <div class="table-scroller">
                    <div class="fixed-header">
                      <table class="light striped-even mb-0 ">
                      <thead>
                        <tr>
                          <th class="w-70">Certification</th>
                          <th class="W-5">Action</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="(dtl, index) in nciis" :key="index">
                          <td>{{ dtl.nciiQualificationTitleName }}</td>
                          <td class="p-1">
                            <div class="act-btns" sm-1 mb-0 >
                              <button type="button" class="justify-between info fw-22" @click="onEditNCII(dtl, index)">
                                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                              </button>
                              <button type="button" class="warning" title="Delete" @click="onDeleteNCII(index)">
                                <i class="fa fa-times fa-lg"></i>
                              </button>
                            </div>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                    </div>
                  </div>
                    <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                    <div></div>

                      <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                        <button type="button" class="justify-between btn-add" @click="onAddNCII"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                      </div>
                    </div>
                    </div>    
                    <!-- License -->
                    <div class="app-box-style gap">
                          <div class="Header app-form-header curved-1">Professional License</div>
                          
                          <div class="table-scroller">
                          <div class="fixed-header">
                            <table class="light striped-even mb-0 ">
                            <thead>
                              <tr>
                                <th class="w-70">License Profession</th>
                                <th class="W-5">Action</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr v-for="(dtl, index) in licenses" :key="index">
                                <td>{{ dtl.licenseProfessionName }}</td>
                                <td class="p-1">
                                  <div class="act-btns" sm-1 mb-0 >
                                    <button type="button" class="justify-between info fw-22" @click="onEditLicense(dtl, index)">
                                      <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                    </button>
                                    <button type="button" class="warning" title="Delete" @click="onDeleteLicense(index)">
                                      <i class="fa fa-times fa-lg"></i>
                                    </button>
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                          </div>
                        </div>
                          <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                          <div></div>

                            <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                              <button type="button" class="justify-between btn-add" @click="onAddLicense"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                            </div>
                          </div>
                    </div>   

                    <!-- Compliance -->
                    <div class="app-box-style gap">
                        <div class="Header app-form-header curved-1">Compliance Training</div>
                        
                        <div class="table-scroller">
                        <div class="fixed-header">
                          <table class="light striped-even mb-0 ">
                          <thead>
                            <tr>
                              <th class="w-70">Compliance Training</th>
                              <th class="W-5">Action</th>
                            </tr>
                          </thead>
                          <tbody>
                            <tr v-for="(dtl, index) in compliances" :key="index">
                              <td>{{ dtl.complianceTrainingName }}</td>
                              <td class="p-1">
                                <div class="act-btns" sm-1 mb-0 >
                                  <button type="button" class="justify-between info fw-22" @click="onEditCompliance(dtl, index)">
                                    <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                  </button>
                                  <button type="button" class="warning" title="Delete" @click="onDeleteCompliance(index)">
                                    <i class="fa fa-times fa-lg"></i>
                                  </button>
                                </div>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                        </div>
                      </div>
                      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                        <div></div>

                          <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                            <button type="button" class="justify-between btn-add" @click="onAddCompliance"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                          </div>
                        </div>
                    </div>  
                  </div> 
               </div>
          </div>
          
        </div>

  

 
             
      </sym-tab> 

      <sym-tab title="Other Information" icon="reorder">
        <div class="box-container">

       
         <!-- Religion -->
    
      <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
        <div class="Header app-form-header curved-1 religion-fields ">Religion</div>
        <div class="religion-info " v-show= "request.memberRequestId != 0">
            <sym-table class="table-select striped-odd bill hover mb-0 " colorClass="light" >
              <sym-table-header slot="header"  >
                <tr class="align-top">
                  <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                  <th class="w-90">Religion</th>
                </tr>
                
              </sym-table-header >
                <sym-table-body slot="body" v-show="request.memberRequestId !=0"  >
                  <tr v-for="(religion, index) in religionList" :key="index" class="align-top" @click="onToggleReligionSelection(religion)">
                    <td class="w-10 text-center"><input type="checkbox" :checked="isReligionSelected(religion)"></td>
                    <td class="w-90">{{ religion.religionName }}</td>
                  </tr>
                </sym-table-body>
              </sym-table>
        </div>
        </div>      
     

 <!-- Civil Status -->
   
      <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
        <div class="Header app-form-header curved-1">Civil Status</div>
        <div class="civil-info" v-show= "request.memberRequestId != 0">
            <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
              <sym-table-header slot="header">
                <tr class="align-top">
                  <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                  <th class="w-90">Civil Status</th>
                </tr>
                
              </sym-table-header>
                <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                  <tr v-for="(civilStatus, index) in civilStatusList" :key="index" class="align-top" @click="onToggleCivilStatusSelection(civilStatus)">
                    <td class="w-10 text-center"><input type="checkbox" :checked="isCivilStatusSelected(civilStatus)"></td>
                    <td class="w-90">{{ civilStatus.civilStatusName }}</td>
                  </tr>
                </sym-table-body>
              </sym-table>
        </div>      
        </div>      



 <!-- Sex -->

      <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
        <div class="Header app-form-header curved-1">Sex</div>
        <div class="sex-info" v-show= "request.memberRequestId != 0">

        
            <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
              <sym-table-header slot="header">
                <tr class="align-top">
                  <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                  <th class="w-90">Sex</th>
                </tr>
                
              </sym-table-header>
                <sym-table-body slot="body" v-show="request.memberRequestId !=0">
                  <tr v-for="(sex, index) in sexList" :key="index" class="align-top" @click="onToggleSexSelection(sex)">
                    <td class="w-10 text-center"><input type="checkbox" :checked="isSexSelected(sex)"></td>
                    <td class="w-90">{{ sex.sexName }}</td>
                  </tr>
                </sym-table-body>
              </sym-table>
        </div>      
        </div>      
      </div>


      </sym-tab>

      <sym-tab title="Pool Information" icon="group">

      <!-- Candidate -->
      <div class="candicate-info">
        <div class="Header app-form-header curved-1">Hired Applicant/s</div>
        
        <div class="table-scroller">
        <div class="fixed-header">
          <table class="light striped-even mb-0 ">
          <thead>
            <tr>
              <th class="candidate-id">ID</th>
              <th class="candidate-name">Name</th>
              <th class="candidate-birth">Birth Date</th>
              <th class="candidate-age">Age</th>
              <th class="candidate-sex">Sex</th>
              <th class="candidate-mobile">Mobile</th>
              <th class="candidate-action">Hired Date</th>
              <th class="candidate-deploy">Deployment Date</th>
              <th class="candidate-deploy text-center">Allowance</th>
              <th class="candidate-deploy text-center">Deminimis</th>
              <th class="candidate-action">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(dtl, index) in candidateList" :key="index" >
                <td class="submit-hot memberid" @click="onClickMember(dtl.memberId)">{{ dtl.memberId }} </td>


              <td>{{ dtl.memberName }}</td>
              <td>{{ dtl.birthDate }}</td>
              <td>{{ dtl.age }}</td>
              <td>{{ dtl.sexId }}</td>
              <td>{{ dtl.mobileNumber }}</td>
              <td>{{ dtl.hiredDate }}</td>
              <td>{{ dtl.deploymentDate }}</td>
              <td class="text-right">
                <div class="act-btns" sm-1 mb-2 >                        
                  <button type="button" :class="logButtonClass"  class="btn-log info mb-2"  @click="onClickAllowance(dtl.memberId, dtl.memberName)">
                    <template v-if="dtl.allowanceAmount <= 0">
                      <i class="fa fa-plus fa-lg"></i>
                      <span>{{ core.toDecimalFormat(dtl.allowanceAmount, 2, true) }}</span>
                    </template>

                    <template v-else>
                      {{ core.toDecimalFormat(dtl.allowanceAmount, 2, true) }}
                    </template>
                  </button>
                </div>
              </td>

              <td class="text-right">
                <div class="act-btns" sm-1 mb-2 >                        
                  <button type="button" :class="logButtonClass"  class="btn-log info mb-2"  @click="onClickDeminimis(dtl.memberId, dtl.memberName)">
                    <template v-if="dtl.deminimisAmount <= 0">
                      <i class="fa fa-plus fa-lg"></i>
                      <span>{{ core.toDecimalFormat(dtl.deminimisAmount, 2, true) }}</span>
                    </template>

                    <template v-else>
                      {{ core.toDecimalFormat(dtl.deminimisAmount, 2, true) }}
                    </template>
                  </button>
                </div>
              </td>

              <td class="p-1">
                <div class="act-btns" sm-1 mb-0 >                 
                  <button type="button" :class="logButtonClass" class="justify-between btn-log w-60"   @click="onViewLog(index)">
                    <i class="fa fa-database fa-lg"></i><span>Log</span> 
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        </div>
      </div>
    </div>  
      </sym-tab>
      </sym-tabs>  
       </div> 
       

      </sym-form>
      
<!-- Pay Out -->
<sym-modal
  id="pay-out-editor"
  v-model="isPayOutEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorPayOutTitle"
  @show="onShowPayOutEditor($event)"
  @hide="onHidePayOutEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030E" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="billing-editor-boxes gap">
  
        <sym-combo v-model="payOut.payTrxCode" caption="Trx Name" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onPayOutTrxCodeChanging" @changed="onPayTrxCodeChanged(payOut.payTrxCode)"></sym-combo>

        
        <sym-dec v-model="payOut.dailyRate" caption="Daily Rate" align="bottom"></sym-dec>
        <!-- <sym-check v-model="payOut.fixedFlag" caption="Fixed" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>  -->

         </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitPayOut()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isPayOutEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>


      <!-- Billing -->
<sym-modal
  id="billing-editor"
  v-model="isBillingEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorBillingTitle"
  @show="onShowBillingEditor($event)"
  @hide="onHideBillingEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030E" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="billing-editor-boxes gap">
        <sym-combo v-model="billing.payTrxCode" caption="Trx Name" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onPayOutTrxCodeBillingChanging" @changed="onPayTrxCodeBillingChanged(billing.payTrxCode)"></sym-combo>

        <sym-dec v-model="billing.dailyRate" caption="Daily Rate" align="bottom"></sym-dec>
        <!-- <sym-check v-model="billing.fixedFlag" caption="Fixed" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>  -->

      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitBilling()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isBillingEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>


<!-- NCII -->
<sym-modal
  id="ncii-editor"
  v-model="isNCIIEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorNCIITitle"
  @show="onShowNCIIEditor($event)"
  @hide="onHideNCIIEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="ncii-editor-boxes">
  
        <sym-combo v-model="ncii.nciiQualificationTitleId" caption="Title" align="bottom" display-field="nciiQualificationTitleName" :datasource="nciiTitle" @changing="onNCIIQualificationTitleIdChanging"></sym-combo>
         </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitNCII()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isNCIIEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>

<!-- License -->
<sym-modal
  id="license-editor"
  v-model="isLicenseEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorLicenseTitle"
  @show="onShowLicenseEditor($event)"
  @hide="onHideLicenseEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="license-editor-boxes">
  
        <sym-combo v-model="license.licenseProfessionId" caption="License Profession" align="bottom" display-field="licenseProfessionName" :datasource="licenseTitle" @changing="onLicenseProfessionIdChanging"></sym-combo>
         </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitLicense()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isLicenseEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>

<!-- CDA  -->
<sym-modal
  id="cda-editor"
  v-model="isCDAEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorCDATitle"
  @show="onShowCDAEditor($event)"
  @hide="onHideCDAEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030C" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="cda-editor-boxes">
  
        <sym-combo v-model="cda.cdaTrainingId" caption="CDA Training" align="bottom" display-field="cdaTrainingName" :datasource="cdaTitle" @changing="onCDATrainingIdChanging"></sym-combo>
         </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitCDA()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isCDAEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>

<!-- Compliance  -->
<sym-modal
  id="compliance-editor"
  v-model="isComplianceEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorComplianceTitle"
  @show="onShowComplianceEditor($event)"
  @hide="onHideComplianceEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0030D" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="compliance-editor-boxes">
  
        <sym-combo v-model="compliance.complianceTrainingId" caption="Compliance Training" align="bottom" display-field="complianceTrainingName" :datasource="complianceTitle" @changing="onComplianceTrainingIdChanging"></sym-combo>
         </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitCompliance()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isComplianceEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>

<!-- logs     -->
<sym-modal 
        v-model="isLogVisible"
        size="xl"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="true"
        :dismissible="true"
        :closeOnBackButton="false"
        title="Applicant Change Log"
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
                <td>
                  {{
                    core.toDateFormat(log.logDateTime, true, "MM/dd/yyyy h:mm tt")
                  }}
                </td>
                <td>{{ log.logUserInfo }}</td>
                <td>{{ log.logReference }}</td>
              </tr>
            </tbody>
          </table>
        </div>
  </sym-modal>
<!-- logs     -->


</section>  
</template>

<script>

import { DateTime } from "../../js/core";

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
import SymInteger from '../../comp/SymInteger.vue';
import SymMemo from '../../comp/SymMemo.vue';

export default {
  components: { SymInteger, SymMemo },
  extends: PageMaintenance,
  name: 'ars0030',

  data () {
    return {
    
      request: {  
        memberRequestId: 0,
        memberRequestName: '',
        orgName: '',
        platformName: '',
        clientPosition: '',
        memberRequestPositionId: 0,
        memberRequestPositionName: '',
        jobCode: '',
        jobDescription: '',
        memberTypeId: 0,
        payoutTypeId: 0,
        vacancyCount: 0,
        deploymentDate: null,
        positionKeyword: '',
        skillNameKeyword: '',
        memberRequestRemarks: '',
        memberRequestPerks: '',
        memberRequestStatusId: 0,
        clientPayGroupId: 0,
        clientPayGroupName: '',
        workingDays: 0,
        employmentTypeId:0,
        employmentEndDate: null,
        requestTypeId:0, 
        educationLevelId:0, 
        rateAmount:0,
        deminimisAmount:0,
        allowanceAmount:0,
        minimumWageFlag:false,
        clientName:'',
        name2:'',
        name3:'',
        name4:'',
        name5:'',
        name6:'',
        name7:'',
        memberId: 0,
        memberName: '',
        lockId: ''
      },
      educations: [],
      medicalResults: [],
      religions: [],
      civilStatus: [],
      sexes: [],
      employmentTypes: [],
      typeQualifications: [],
      updateTypeQualificationList: false,
      payGroups: [],
      screenings: [],
      docs: [],

      billings: [],

      billing: {
        billingDetailId: 0,
        payTrxCode: '',
        payTrxName: '',
        dailyRate: 0,
        monthlyRate: 0,
        fixedFlag: false,
        payGroupFlag: false,
        memberRequestFlag: false,
        systemCode: '',
      },

      billingIndex: -1,
      isAddingBilling: false,
      isBillingEditorVisible: false,

      payOuts: [],

      payOut: {
        payOutDetailId: 0,
        payTrxCode: '',
        payTrxName: '',
        dailyRate: 0,
        monthlyRate: 0,
        fixedFlag: false,
        payGroupFlag: false,
        memberRequestFlag: false,
        systemCode: '',
      },

      payOutIndex: -1,
      isAddingPayOut: false,
      isPayOutEditorVisible: false,

      nciis: [],

      ncii: {
        nciiDetailId: 0,
        nciiQualificationTitleId: 0,
        nciiQualificationTitleName: '',
      },

      nciiIndex: -1,
      isAddingNCII: false,
      isNCIIEditorVisible: false,

      licenses: [],

      license: {
        licenseProfessionDetailId: 0,
        licenseProfessionId: 0,
        licenseProfessionName: '',
      },

      licenseIndex: -1,
      isAddingLicense: false,
      isLicenseEditorVisible: false,

      cdas: [],

      cda: {
        cdaTrainingDetailId: 0,
        cdaTrainingId: 0,
        cdaTrainingName: '',
      },

      cdaIndex: -1,
      isAddingCDA: false,
      isCDAEditorVisible: false,

      compliances: [],

      compliance: {
        complianceTrainingDetailId: 0,
        complianceTrainingId: 0,
        complianceTrainingName: '',
      },

      complianceIndex: -1,
      isAddingCompliance: false,
      isComplianceEditorVisible: false,

      memberList: [],
      candidateList: [],
      activeTabIndex: 0,

      rateAmount: 0,

      logs: [],
      isLogVisible: false,

      candidate : {
      applicantDetailId: 0,
      memberRequestId: 0,
      memberId: 0,
      memberName: '',
      applicantScreeningId: 0,
      screeningStatusId: 0,
      applicantStatusId: 0,
      remarks:'',
      
      hiredDate: null,
      deploymentDate: null,
      screeningFlag: 0,
      amount:0,
      lockId: '',

    },

    basicRate:0,
    deminimisRate:0,
    allowanceRate:0,

    refPayOuts:[],
    
    payOutDailyTotalRate : 0,
    payOutMonthlyTotalRate : 0,
    billingDailyTotalRate : 0,
    billingMonthlyTotalRate : 0,
    minimumWageList: [],

    };
  },

  computed: {

    isCancelled () {
      return this.request.memberRequestStatusId === 3;
    },

    isActivated () {
      return this.request.memberRequestStatusId === 1;
    },

    isNotActive () {
      return this.request.memberRequestStatusId === 2;
    },

    isClosed () {
      return this.request.totalHired === this.request.vacancyCount;
    },
    canSetActive () {
      return this.request.memberRequestStatusId === 2;
    },
    canSetCanceled () {
      return this.request.memberRequestStatusId === 2;
    },

    editorBillingTitle () {
      if (this.isAddingBilling) {
        return 'Add Billing Detail';
      }
      return 'Edit Billing Detail';
    },

    editorPayOutTitle () {
      if (this.isAddingPayOut) {
        return 'Add Pay Out Detail';
      }
      return 'Edit Pay Out Detail';
    },

    editorNCIITitle () {
      if (this.isAddingNCII) {
        return 'Add NCII Title Detail';
      }
      return 'Edit NCII Title Detail';
    },

    editorLicenseTitle () {
      if (this.isAddingLicense) {
        return 'Add License Detail';
      }
      return 'Edit License Detail';
    },

    editorCDATitle () {
      if (this.isAddingCDA) {
        return 'Add CDA Training Detail';
      }
      return 'Edit CDA Training Detail';
    },

    editorComplianceTitle () {
      if (this.isAddingCompliance) {
        return 'Add Compliance Training Detail';
      }
      return 'Edit Compliance Training Detail';
    },

  },

  methods: {    

    onMemberIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.request.memberId = data.memberId;
      me.request.memberName = data.memberName;
      me.setFocus("memberRequestPerks");
    },

  
    onMemberIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.memberIdCallback;
    },


    memberIdCallback (e) {
      const me = this;
      let filter = "MemberId='" + e.proposedValue +"'";
      return getList('dbo.QArsMemberRequestActiveList', 'MemberId, MemberName', '', filter).then(
        member => {
          if (member && member.length) {        
            me.request.memberName = member[0].memberName;
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

    onCopy () {
     const me = this,
      wait = me.wait();
     
      me.dialog.confirm('Create new record based on <b>MRF #</b>' + me.request.memberRequestId + ' - '+ me.request.memberRequestName + '. Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {          
          if (reply === 'yes') {
            if (me.canAdd) {
                me.advice.warning('You are adding a new document.', { duration: 5 });
                me.request.memberRequestId = -1;
                me.request.memberRequestName = ''; 
                me.setFocus("memberRequestName");
            } else {
                let mssg = 'You are not allowed to create new documents.';
                me.advice.fault(mssg, { duration: 5 });
                me.onReset();
            }

          }
           me.stopWait(wait);
          return;
        }
      );

    },

    getTargetPath() {
      const me = this,
        q = {};

      if (me.request.memberRequestId) {
        q.memberRequestId = me.request.memberRequestId;
      }
      
      if (me.activeTabIndex) {
        q.activeTabIndex = me.activeTabIndex;
      }
   

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("memberRequestId" in q && me.core.isInteger(q.memberRequestId)) {
       
        me.request.memberRequestId = parseInt(q.memberRequestId);
    
      }

      if ("activeTabIndex" in q && me.core.isInteger(q.activeTabIndex)) {
        me.activeTabIndex = parseInt(q.activeTabIndex);
     
     
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

    onClickAllowance(memberId, memberName) {
      const me = this;

      let route = {
        name: "ars0110",
        query: {
          memberId: memberId,
          memberRequestId: this.request.memberRequestId,
          memberName: memberName,
          memberRequestName: this.request.memberRequestName,
          activeTabIndex: 2 
        },
      };
      me.go(route);
    },

    onClickDeminimis(memberId, memberName) {
      const me = this;

      let route = {
        name: "ars0140",
        query: {
          memberId: memberId,
          memberRequestId: this.request.memberRequestId,
          memberName: memberName,
          memberRequestName: this.request.memberRequestName,
          activeTabIndex: 1 
        },
      };
      me.go(route);
    },


    onMinimumWageFlagChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.onMinimumWageFlagCallback;
    },

    onMinimumWageFlagCallback(e) {
      const me = this;
     
    if (!me.request.clientPayGroupId) {
        me.advice.fault('Pay Group is required.', { duration: 5 })
        return false;
      }  

      return true;
    },
 
   
    onCopyPayOut() {
      this.billings = this.payOuts.map(payOut => ({
        ...payOut, 
      }));
  
      this.refreshTotal();
    },

    onPayTrxCodeBillingChanged (newValue) {
      const me = this;
    
      let o = me.payTrx.find( o => o.payTrxCode == newValue);
      if (o) {
        me.billing.systemCode = o.systemCode;

      }
    },


    onPayTrxCodeChanged (newValue) {
      const me = this;
    
      let o = me.payTrx.find( o => o.payTrxCode == newValue);
      if (o) {
        me.payOut.systemCode = o.systemCode;

      }
    },


    onMinimumWageFlagChanged() {
    },

    refreshTotal () {
      const me = this;

      me.payOutDailyTotalRate = 0;
      me.payOutMonthlyTotalRate = 0;
      me.billingDailyTotalRate = 0;
      me.billingMonthlyTotalRate = 0;
      
      me.payOuts.forEach( dtl => {
        me.payOutDailyTotalRate = me.payOutDailyTotalRate + dtl.dailyRate;
        if (dtl.fixedFlag) {
        me.payOutMonthlyTotalRate = me.payOutMonthlyTotalRate + dtl.dailyRate;  
        } else
        me.payOutMonthlyTotalRate = me.payOutMonthlyTotalRate + dtl.monthlyRate;
      }
      );
    
      me.billings.forEach( dtl => {
        me.billingDailyTotalRate = me.billingDailyTotalRate + dtl.dailyRate;

       if (dtl.fixedFlag) {
        me.billingMonthlyTotalRate = me.billingMonthlyTotalRate + dtl.dailyRate;  
        } else
        me.billingMonthlyTotalRate = me.billingMonthlyTotalRate + dtl.monthlyRate;

      });

        
    },


    onCheckRateDetails() {
      const me = this;
      
      
      if (me.request.deploymentDate === null) {
        me.advice.fault('Deployment date is required.', { duration: 5 })
        return;
      }  

      if (me.minimumWageList.length === 0 || me.minimumWageList[0].rate == null || isNaN(me.minimumWageList[0].rate)) {
        me.advice.fault('Minimum rate is required.', { duration: 5 });
        return;
      }


      me.payOuts = me.payOuts.filter(item => item.systemCode == 'BASIC' || item.systemCode == 'DEMINIMIS' || item.systemCode == 'ALLOWANCE');
      me.billings = me.billings.filter(item => item.systemCode == 'BASIC' || item.systemCode == 'DEMINIMIS' || item.systemCode == 'ALLOWANCE');


      me.getClientBillingTransactions().then(
      (data) => {
      data.forEach(item => {
      me.billings.push({
      payTrxCode: item.payTrxCode,
      payTrxName: item.payTrxName,
      dailyRate: item.dailyRate,
      monthlyRate: item.monthlyRate,
      payGroupFlag: item.payGroupFlag,
      memberRequestFlag: item.memberRequestFlag
      });
    });

      },
      (fault) => {
        me.showFault(fault);
      }
);

me.getClientPayOutTransactions().then(
  (data) => {
    
    data.forEach(item => {
      me.payOuts.push({
      payTrxCode: item.payTrxCode,
      payTrxName: item.payTrxName,
      dailyRate: item.dailyRate,
      monthlyRate: item.monthlyRate,
      payGroupFlag: item.payGroupFlag,
      memberRequestFlag: item.memberRequestFlag
      
      });
    });

  },
  (fault) => {
    me.showFault(fault);
  }
);

      me.basicRate = 0;
      me.deminimisRate = 0;
      me.allowanceRate = 0;

      const basicRate = me.payOuts.find(payOut => payOut.systemCode === 'BASIC');
        if (basicRate) {
          me.basicRate = basicRate.dailyRate;          
        }
        
        const deminimisRate = me.payOuts.find(payOut => payOut.systemCode === 'DEMINIMIS');
        if (deminimisRate) {
          me.deminimisRate = deminimisRate.dailyRate;
        }

        const allowanceRate = me.payOuts.find(payOut => payOut.systemCode === 'ALLOWANCE');
        if (allowanceRate) {
          me.allowanceRate = allowanceRate.dailyRate;
        }



me.getRateDetails().then(
  
(data) => {

  me.refPayOuts = data;
    
      me.payOuts.forEach(record => {
        const matchingRecord = me.refPayOuts.find(memberRecording => memberRecording.payTrxCode === record.payTrxCode);
        if (matchingRecord) {
          record.dailyRate = matchingRecord.dailyRate;
          if (record.fixedFlag) {
          record.monthlyRate = matchingRecord.monthlyRate;
          }
          else    
          record.monthlyRate = matchingRecord.monthlyRate
        }
      });

      me.billings.forEach(record => {
        const matchingRecord = me.refPayOuts.find(memberRecording => memberRecording.payTrxCode === record.payTrxCode);
        if (matchingRecord) {
          record.dailyRate = matchingRecord.dailyRate;
           if (record.fixedFlag) {
          record.monthlyRate = matchingRecord.monthlyRate;
          }
          else    
          record.monthlyRate = matchingRecord.monthlyRate 



        }
      });

      me.payOuts.forEach((payOut) => {    
        if (payOut.fixedFlag) {
          payOut.monthlyRate = payOut.dailyRate;
        } else
        payOut.monthlyRate = Math.round((payOut.dailyRate * me.request.workingDays / 12) * 100) / 100;
      });

      me.billings.forEach((billing) => {   

        if (billing.fixedFlag) {
          billing.monthlyRate = billing.dailyRate;
        } else
        billing.monthlyRate = Math.round((billing.dailyRate * me.request.workingDays / 12) * 100) / 100;
      });
      me.refreshTotal();
      me.advice.info('Rates are updated.', { duration: 5 })

  },
  (fault) => {
    me.showFault(fault);
  }
);

    },


    onPostBasic() {
      const me = this;
      if (!me.request.clientPayGroupId && !me.request.workingDays && !me.request.deploymentDate) {
        me.advice.fault('Check Rate is required.', { duration: 5 })
        return;
      }  

      if (me.minimumWageList.length === 0 || me.minimumWageList[0].rate == null || isNaN(me.minimumWageList[0].rate)) {
        me.advice.fault('Minimum rate is required.', { duration: 5 });
        return;
      }

      this.onPostBasicPayout();
      this.onPostBasicBilling();
      me.refreshTotal();
    },

onPostBasicPayout() {
  const me = this;
  const d = me.payOut; 
  let dtl = {}; 

  const existingPayOut = me.payOuts.find(p => p.payTrxCode === 'BASIC');

  if (existingPayOut) {
    existingPayOut.dailyRate = me.minimumWageList[0].rate;
    existingPayOut.systemCode = 'BASIC';
    me.advice.info("[Pay Out] Pay Trx Name '" + existingPayOut.payTrxCode + "' updated in the list.", {
      duration: 5,
    });

  } else {
    Object.assign(dtl, d); 

    dtl.payTrxCode = 'BASIC'; 
    dtl.dailyRate = me.minimumWageList[0].rate; 
    dtl.systemCode = 'BASIC';
    me.payTrx.forEach((payTrx) => {
      if (payTrx.payTrxCode === dtl.payTrxCode) {
        dtl.payTrxName = payTrx.payTrxName;
      }
    });

    me.payOuts.push(dtl);
    
    me.advice.info("[Pay Out] Pay Trx Name '" + dtl.payTrxName + "' added to list.", {
      duration: 5,
    });
  }

  me.setFocus("payTrxCode");
},

onPostBasicBilling() {
  const me = this;
  const d = me.billing; 
  let dtl = {}; 

  const existingPayOut = me.billings.find(p => p.payTrxCode === 'BASIC');

  if (existingPayOut) {
    existingPayOut.dailyRate = me.minimumWageList[0].rate;
    existingPayOut.systemCode = 'BASIC';

    me.advice.info("[Billing] Pay Trx Name '" + existingPayOut.payTrxName + "' updated in the list.", {
      duration: 5,
    });
  } else {
    Object.assign(dtl, d); 

    dtl.payTrxCode = 'BASIC'; 
    dtl.dailyRate = me.minimumWageList[0].rate; 
    dtl.systemCode = 'BASIC';

    me.payTrx.forEach((payTrx) => {
      if (payTrx.payTrxCode === dtl.payTrxCode) {
        dtl.payTrxName = payTrx.payTrxName;
      }
    });

    me.billings.push(dtl);

    me.advice.info("[Billing] Pay Trx Name '" + dtl.payTrxName + "' added to list.", {
      duration: 5,
    });
  }

  me.setFocus("payTrxCode");
},

    onCheckBasic() {
      const me = this;

      if (me.request.deploymentDate === null) {
        me.advice.fault('Deployment date is required.', { duration: 5 })
        return;
      }  
 
      if (!me.request.clientPayGroupId && !me.request.workingDays && !me.request.deploymentDate === null) {
        me.advice.fault('Required: Pay Group | Working Days | Deployment Date.', { duration: 5 })
        return;
      }  
 
      me.getMinimumWage().then(

        (data) => {
          me.minimumWageList = data.basic;
          me.advice.info("Minimum wage list updated.", {
            duration: 5,
          });
        },
        (fault) => {
          me.showFault(fault);
        }
      );


    },
    onSetActive () {
      const me = this;

      let promise = me.isActionAllowed('ACT-STATUS-MRF');

      promise.then(
        reply => {
          if (reply === 'yes') {
            me.onActivate();
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );

    },

    onSetCanceled () {
      const me = this;
      
      
      let promise = me.isActionAllowed('CAN-STATUS-MRF');
      
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

    onViewLog(index) {
      const me = this,
        wait = me.wait(),
        d = me.candidate,
        dtl = me.candidateList[index];
    
       d.applicantDetailId = dtl.applicantDetailId;

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

    isApplicantScreeningSelected (applicantScreening) { 
      return this.screenings.findIndex(obj => obj.applicantScreeningId === applicantScreening.applicantScreeningId) > -1;
    },
  
    onToggleApplicantScreeningSelection (applicantScreening) {
      const
        me = this,
        index = me.screenings.findIndex(obj => obj.applicantScreeningId === applicantScreening.applicantScreeningId);

      if (index > -1) {
        me.screenings.splice(index, 1);
      } else {
        me.screenings.push({
          memberRequestId: me.request.memberRequestId,
          applicantScreeningId: applicantScreening.applicantScreeningId
        });
      }
    },

    
    onPayOutTrxCodeBillingChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payOutTrxCodeBillingCallback;
    },

    payOutTrxCodeBillingCallback(e) {
      const me = this;
     
  let index = me.billings.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {
        const payTrxName = me.billings[index].payTrxName;
        e.message = 'Pay Trx Name <b>' + payTrxName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 
    onPayOutTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payOutTrxCodeCallback;
    },

    payOutTrxCodeCallback(e) {
      const me = this;
     
  let index = me.payOuts.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {
        const payTrxName = me.payOuts[index].payTrxName;
        e.message = 'Pay Trx Name <b>' + payTrxName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 
    onShowPayOutEditor () {
      const me = this;
      
      me.setActiveModel('payOut');

      me.setRequiredMode(
        'payTrxCode'
      );

      setTimeout(() => {
        this.setFocus('payTrxCode');
      }, 200);
    },

    onHidePayOutEditor () {
      const me = this;

      me.isAddingPayOut = false;
      me.setActiveModel();
    },

    onEditPayOut (dtl, index) {

      const d = this.payOut;
      this.payOutIndex = index;

      d.payOutDetailId = dtl.payOutDetailId;
      d.payTrxCode = dtl.payTrxCode;
      d.dailyRate = dtl.dailyRate;
      d.fixedFlag = dtl.fixedFlag;
      
      this.isPayOutEditorVisible = true;
    },

    onAddPayOut () {
      const me = this;
      
      me.clearPayOutPad();
      
      me.payOut.payOutDetailId = -1 

      me.isPayOutEditorVisible = true;
      me.isAddingPayOut = true;
        
    },

    onSubmitPayOut() {
      const me = this,
        d = me.payOut;

      if (!d.payTrxCode) {
        me.isPayOutEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingPayOut) {
 
        Object.assign(dtl, d);

        me.payTrx.forEach((payTrx) => {
          if (payTrx.payTrxCode == dtl.payTrxCode) {
            dtl.payTrxName = payTrx.payTrxName;
          }
        });
        if (dtl.fixedFlag) {
          dtl.monthlyRate = dtl.dailyRate
        } else {
          dtl.monthlyRate = dtl.monthlyRate
        }

        me.payOuts.push(dtl);

        me.clearPayOutPad();
        
        me.advice.info("Pay Trx Name '" + dtl.payTrxName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("payTrxCode");
        
      } else {

        dtl = me.payOuts[me.payOutIndex];
         
        me.payTrx.forEach((dtl) => {
          if (dtl.payTrxCode == d.payTrxCode) {
            d.payTrxName = dtl.payTrxName;
          }
        });

        dtl.payOutDetailId = d.payOutDetailId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.dailyRate = d.dailyRate;
        dtl.fixedFlag = d.fixedFlag;

        if (dtl.fixedFlag) {
          dtl.monthlyRate = dtl.dailyRate 
        } else {
          dtl.monthlyRate = dtl.monthlyRate
        }


        me.isPayOutEditorVisible = false;

      }
      
      me.onCheckRateDetails();
      me.refreshTotal();
    },

    onDeletePayOut(index) {
      const me = this;

      me.dialog.confirm( "Pay Trx Name <b>" + me.payOuts[index].payTrxName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.payOuts.splice(index, 1);
            me.onCheckRateDetails();
            me.refreshTotal();
          }
          return;
        });
   
    },

    clearPayOutPad () {
      const d = this.payOut;

      d.payOutDetailId = 0;
      d.payTrxCode = '';
      d.payTrxName = '';
      d.dailyRate = 0;
      d.fixedFlag = false;
    },

    onPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payTrxCodeCallback;
    },

    payTrxCodeCallback(e) {
      const me = this;

      let index = me.billings.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {

        const payTrxName = me.billings[index].payTrxName;
        e.message = 'Pay Trx Name <b>' + payTrxName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 

    onShowBillingEditor () {
      const me = this;
      
      me.setActiveModel('billing');

      me.setRequiredMode(
        'payTrxCode'
      );

      setTimeout(() => {
        this.setFocus('payTrxCode');
      }, 200);
    },

    onHideBillingEditor () {
      const me = this;

      me.isAddingBilling = false;
      me.setActiveModel();
    },

    onEditBilling (dtl, index) {

      const d = this.billing;
      this.billingIndex = index;

      d.billingDetailId = dtl.billingDetailId;
      d.payTrxCode = dtl.payTrxCode;
      d.dailyRate = dtl.dailyRate;
      d.fixedFlag = dtl.fixedFlag;

      this.isBillingEditorVisible = true;
    },

    onAddBilling () {
      const me = this;
      
      me.clearBillingPad();
      
      me.billing.billingDetailId = -1 

      me.isBillingEditorVisible = true;
      me.isAddingBilling = true;
        
    },

    onSubmitBilling() {
      const me = this,
        d = me.billing;

      if (!d.payTrxCode) {
        me.isBillingEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingBilling) {
 
        Object.assign(dtl, d);

        me.payTrx.forEach((payTrx) => {
          if (payTrx.payTrxCode == dtl.payTrxCode) {
            dtl.payTrxName = payTrx.payTrxName;
          }
        });
        
        dtl.monthlyRate = dtl.monthlyRate 

        me.billings.push(dtl);

        me.clearBillingPad();
        
        me.advice.info("Pay Trx Name '" + dtl.payTrxName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("payTrxCode");
        
      } else {

        dtl = me.billings[me.billingIndex];
         
        me.payTrx.forEach((dtl) => {
          if (dtl.payTrxCode == d.payTrxCode) {
            d.payTrxName = dtl.payTrxName;
          }
        });

        dtl.billingDetailId = d.billingDetailId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.dailyRate = d.dailyRate;
        dtl.monthlyRate = d.monthlyRate 
        dtl.fixedFlag = d.fixedFlag;

        me.isBillingEditorVisible = false;

      }

      me.onCheckRateDetails();
      me.refreshTotal();
    },

    onDeleteBilling(index) {
      const me = this;

      me.dialog.confirm( "Pay Trx Name <b>" + me.billings[index].payTrxName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.billings.splice(index, 1);
            me.onCheckRateDetails();
            me.refreshTotal();
          }
          return;
        });
       
    },

    clearBillingPad () {
      const d = this.billing;

      d.billingDetailId = 0;
      d.payTrxCode = '';
      d.payTrxName = '';
      d.dailyRate = 0;
      d.fixedFlag = false;

    },
    onActiveTabIndexChanged () {
      this.reload();
    },
 
    onProcessMember(dtl) {
        this.advice.success(`${dtl.memberName} is in process.`, { duration: 5 });
      },


    onAddMember(dtl, index) {
      const isMemberExists = this.candidateList.some(member => member.memberId === dtl.memberId);
        if (isMemberExists) {
          this.advice.warning('This member is already in the candidate list', { duration: 5 });
          return;
        }  
      
      this.candidateList.push(dtl);
        this.memberList.splice(index, 1);
        this.advice.success(`${dtl.memberName} has been added to the candidate list.`, { duration: 5 });
      },

    onRemoveMember(index) { 
      this.memberList.splice(index, 1);
    },

    onRemoveCandidate(index) { 
      this.candidateList.splice(index, 1);
    },

    onPooling() {
      const me = this;

      me.getMembers().then(
        (data) => {
          me.memberList = [];
          me.memberList = data;
        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },

    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to <b>cancel</b> MRF.<br>Once cancelled, MRF cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
  
            me.request.memberRequestStatusId = 3;
       
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onActivate () {
      const me = this;

      me.dialog.confirm('Ready to <b>activate</b> MRF.<br>Once activated, MRF cannot be canceled.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.request.memberRequestStatusId = 1;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onClose () {
      const me = this;

      me.dialog.confirm('Ready to <b>close</b> MRF.<br>Once closed, MRF cannot be activate/canceled.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.request.memberRequestStatusId = 4;
            me.onSubmit();
          }
          return;
        }
      );
    },

 

    onComplianceTrainingIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.complianceTrainingIdCallback;
    },

    complianceTrainingIdCallback(e) {
      const me = this;

      let index = me.compliances.findIndex(obj => obj.complianceTrainingId === e.proposedValue);
      if (index > -1) {

        const complianceTrainingName = me.compliances[index].complianceTrainingName;
        e.message = 'Compliance Training <b>' + complianceTrainingName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 

    onShowComplianceEditor () {
      const me = this;
      
      me.setActiveModel('compliance');

      me.setRequiredMode(
        'complianceTrainingId'
      );

      setTimeout(() => {
        this.setFocus('complianceTrainingId');
      }, 200);
    },

    onHideComplianceEditor () {
      const me = this;

      me.isAddingCompliance = false;
      me.setActiveModel();
    },

    onEditCompliance(dtl, index) {

      const d = this.compliance;
      this.complianceIndex = index;

      d.complianceTrainingDetailId = dtl.complianceTrainingDetailId;
      d.complianceTrainingId = dtl.complianceTrainingId;
      
      this.isComplianceEditorVisible = true;
    },

    onAddCompliance () {
      const me = this;
      
      me.clearCompliancePad();
      
      me.compliance.complianceDetailId = -1 

      me.isComplianceEditorVisible = true;
      me.isAddingCompliance = true;
        
    },

    onSubmitCompliance() {
      const me = this,
        d = me.compliance;

      if (!d.complianceTrainingId) {
        me.isComplianceEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingCompliance) {
 
        Object.assign(dtl, d);

        me.complianceTitle.forEach((complianceTitle) => {
          if (complianceTitle.complianceTrainingId == dtl.complianceTrainingId) {
            dtl.complianceTrainingName = complianceTitle.complianceTrainingName;
          }
        });

        me.compliances.push(dtl);

        me.clearCompliancePad();
        
        me.advice.info("Compliance Training '" + dtl.complianceTrainingName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("complianceTrainingId");
        
      } else {

        dtl = me.compliances[me.complianceIndex];
         
        me.complianceTitle.forEach((dtl) => {
          if (dtl.complianceTrainingId == d.complianceTrainingId) {
            d.complianceTrainingName = dtl.complianceTrainingName;
          }
        });

        dtl.complianceTrainingDetailId = d.complianceTrainingDetailId;
        dtl.complianceTrainingId = d.complianceTrainingId;
        dtl.complianceTrainingName = d.complianceTrainingName;
   
        me.isComplianceEditorVisible = false;

      }
    },

    onDeleteCompliance(index) {
      const me = this;

      me.dialog.confirm( "Compliance Training <b>" + me.compliances[index].complianceTrainingName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.compliances.splice(index, 1);
          }
          return;
        });
    },

    clearCompliancePad () {
      const d = this.compliance;

      d.complianceTrainingDetailId = 0;
      d.complianceTrainingId = 0;
      d.complianceTrainingName = '';
    
    },

    onCDATrainingIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.cdaTrainingIdCallback;
    },

    cdaTrainingIdCallback(e) {
      const me = this;

      let index = me.cdas.findIndex(obj => obj.cdaTrainingId === e.proposedValue);
      if (index > -1) {

        const cdaTrainingName = me.cdas[index].cdaTrainingName;
        e.message = 'CDA Training <b>' + cdaTrainingName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 

    onShowCDAEditor () {
      const me = this;
      
      me.setActiveModel('cda');

      me.setRequiredMode(
        'cdaTrainingId'
      );

      setTimeout(() => {
        this.setFocus('cdaTrainingId');
      }, 200);
    },

    onHideCDAEditor () {
      const me = this;

      me.isAddingCDA = false;
      me.setActiveModel();
    },

    onEditCDA(dtl, index) {

      const d = this.cda;
      this.cdaIndex = index;

      d.cdaTrainingDetailId = dtl.cdaTrainingDetailId;
      d.cdaTrainingId = dtl.cdaTrainingId;
      
      this.isCDAEditorVisible = true;
    },

    onAddCDA () {
      const me = this;
      
      me.clearCDAPad();
      
      me.cda.cdaDetailId = -1 

      me.isCDAEditorVisible = true;
      me.isAddingCDA = true;
        
    },

    onSubmitCDA() {
      const me = this,
        d = me.cda;

      if (!d.cdaTrainingId) {
        me.isCDAEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingCDA) {
 
        Object.assign(dtl, d);

        me.cdaTitle.forEach((cdaTitle) => {
          if (cdaTitle.cdaTrainingId == dtl.cdaTrainingId) {
            dtl.cdaTrainingName = cdaTitle.cdaTrainingName;
          }
        });

        me.cdas.push(dtl);

        me.clearCDAPad();
        
        me.advice.info("CDA Training '" + dtl.cdaTrainingName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("cdaTrainingId");
        
      } else {

        dtl = me.cdas[me.cdaIndex];
         
        me.cdaTitle.forEach((dtl) => {
          if (dtl.cdaTrainingId == d.cdaTrainingId) {
            d.cdaTrainingName = dtl.cdaTrainingName;
          }
        });

        dtl.cdaTrainingDetailId = d.cdaTrainingDetailId;
        dtl.cdaTrainingId = d.cdaTrainingId;
        dtl.cdaTrainingName = d.cdaTrainingName;
   
        me.isCDAEditorVisible = false;

      }
    },

    onDeleteCDA(index) {
      const me = this;

      me.dialog.confirm( "CDA Training <b>" + me.cdas[index].cdaTrainingName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.cdas.splice(index, 1);
          }
          return;
        });
    },

    clearCDAPad () {
      const d = this.cda;

      d.cdaTrainingDetailId = 0;
      d.cdaTrainingId = 0;
      d.cdaTrainingName = '';
    
    },

    onLicenseProfessionIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.licenseProfessionIdCallback;
    },

    licenseProfessionIdCallback(e) {
      const me = this;

      let index = me.licenses.findIndex(obj => obj.licenseProfessionId === e.proposedValue);
      if (index > -1) {

        const licenseProfessionName = me.licenses[index].licenseProfessionName;
        e.message = 'License Profession <b>' + licenseProfessionName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 

    onShowLicenseEditor () {
      const me = this;
      
      me.setActiveModel('license');

      me.setRequiredMode(
        'licenseProfessionId'
      );

      setTimeout(() => {
        this.setFocus('licenseProfessionId');
      }, 200);
    },

    onHideLicenseEditor () {
      const me = this;

      me.isAddingLicense = false;
      me.setActiveModel();
    },

    onEditLicense (dtl, index) {

      const d = this.license;
      this.licenseIndex = index;

      d.licenseDetailId = dtl.licenseDetailId;
      d.licenseProfessionId = dtl.licenseProfessionId;
      
      this.isLicenseEditorVisible = true;
    },

    onAddLicense () {
      const me = this;
      
      me.clearLicensePad();
      
      me.license.licenseDetailId = -1 

      me.isLicenseEditorVisible = true;
      me.isAddingLicense = true;
        
    },

    onSubmitLicense() {
      const me = this,
        d = me.license;

      if (!d.licenseProfessionId) {
        me.isLicenseEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingLicense) {
 
        Object.assign(dtl, d);

        me.licenseTitle.forEach((licenseTitle) => {
          if (licenseTitle.licenseProfessionId == dtl.licenseProfessionId) {
            dtl.licenseProfessionName = licenseTitle.licenseProfessionName;
          }
        });

        me.licenses.push(dtl);

        me.clearLicensePad();
        
        me.advice.info("License Profession '" + dtl.licenseProfessionName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("licenseProfessionId");
        
      } else {

        dtl = me.licenses[me.licenseIndex];
         
        me.licenseTitle.forEach((dtl) => {
          if (dtl.licenseProfessionId == d.licenseProfessionId) {
            d.licenseProfessionName = dtl.licenseProfessionName;
          }
        });

        dtl.licenseProfessionDetailId = d.licenseProfessionDetailId;
        dtl.licenseProfessionId = d.licenseProfessionId;
        dtl.licenseProfessionName = d.licenseProfessionName;
   
        me.isLicenseEditorVisible = false;

      }
    },

    onDeleteLicense(index) {
      const me = this;

      me.dialog.confirm( "License Profession <b>" + me.licenses[index].licenseProfessionName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.licenses.splice(index, 1);
          }
          return;
        });
    },

    clearLicensePad () {
      const d = this.license;

      d.licenseProfessionDetailId = 0;
      d.licenseProfessionId = 0;
      d.licenseProfessionName = '';
    
    },
// STOP NA

    onNCIIQualificationTitleIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.nciiQualificationTitleIdCallback;
    },

    nciiQualificationTitleIdCallback(e) {
      const me = this;

      let index = me.nciis.findIndex(obj => obj.nciiQualificationTitleId === e.proposedValue);
      if (index > -1) {

        const nciiQualificationTitleName = me.nciis[index].nciiQualificationTitleName;
        e.message = 'NCII Title <b>' + nciiQualificationTitleName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 

    onShowNCIIEditor () {
      const me = this;
      
      me.setActiveModel('ncii');

      me.setRequiredMode(
        'NCIIQualificationTitleId'
      );

      setTimeout(() => {
        this.setFocus('NCIIQualificationTitleId');
      }, 200);
    },

    onHideNCIIEditor () {
      const me = this;

      me.isAddingNCII = false;
      me.setActiveModel();
    },

    onEditNCII (dtl, index) {

      const d = this.ncii;
      this.nciiIndex = index;

      d.nciiDetailId = dtl.nciiDetailId;
      d.nciiQualificationTitleId = dtl.nciiQualificationTitleId;
      
      this.isNCIIEditorVisible = true;
    },

    onAddNCII () {
      const me = this;
      
      me.clearNCIIPad();
      
      me.ncii.nciiDetailId = -1 

      me.isNCIIEditorVisible = true;
      me.isAddingNCII = true;
        
    },

    onSubmitNCII() {
      const me = this,
        d = me.ncii;

      if (!d.nciiQualificationTitleId) {
        me.isNCIIEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingNCII) {
 
        Object.assign(dtl, d);

        me.nciiTitle.forEach((nciiTitle) => {
          if (nciiTitle.nciiQualificationTitleId == dtl.nciiQualificationTitleId) {
            dtl.nciiQualificationTitleName = nciiTitle.nciiQualificationTitleName;
          }
        });

        me.nciis.push(dtl);

        me.clearNCIIPad();
        
        me.advice.info("NCII Title '" + dtl.nciiQualificationTitleName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("nciiQualificationTitleId");
        
      } else {

        dtl = me.nciis[me.nciiIndex];
         
        me.nciiTitle.forEach((dtl) => {
          if (dtl.nciiQualificationTitleId == d.nciiQualificationTitleId) {
            d.nciiQualificationTitleName = dtl.nciiQualificationTitleName;
          }
        });

        dtl.nciiDetailId = d.nciiDetailId;
        dtl.nciiQualificationTitleId = d.nciiQualificationTitleId;
        dtl.nciiQualificationTitleName = d.nciiQualificationTitleName;
   
        me.isNCIIEditorVisible = false;

      }
    },

    onDeleteNCII(index) {
      const me = this;

      me.dialog.confirm( "NCII Title <b>" + me.nciis[index].nciiQualificationTitleName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.nciis.splice(index, 1);
          }
          return;
        });
    },

    clearNCIIPad () {
      const d = this.ncii;

      d.nciiDetailId = 0;
      d.nciiQualificationTitleId = 0;
      d.nciiQualificationTitleName = '';
    
    },
// STOP NA
    onMemberTypeIdChanged() {
      const me = this;
      
      me.updateTypeQualificationList = false;
      
      me.getMemberTypeQualifications().then(
        (data) => {
          me.typeQualifications = [];
          me.typeQualificationList = data;
          me.updateTypeQualificationList = true;
        },
        (fault) => {
          me.showFault(fault);
        }
      );

    },

    isTypeQualificationSelected (typeQualification) {      
      return this.typeQualifications.findIndex(obj => obj.typeQualificationDetailId === typeQualification.typeQualificationDetailId) > -1;
    },
  
    onToggleTypeQualificationSelection (typeQualification) {
      const
        me = this,
        index = me.typeQualifications.findIndex(obj => obj.typeQualificationDetailId === typeQualification.typeQualificationDetailId);

      if (index > -1) {
        me.typeQualifications.splice(index, 1);
      } else {
        me.typeQualifications.push({
          memberRequestId: me.request.memberRequestId,
          typeQualificationDetailId: typeQualification.typeQualificationDetailId
        });
      }
    },
 

    isEmploymentTypeSelected (employmentType) {      
      return this.employmentTypes.findIndex(obj => obj.employmentTypeId === employmentType.employmentTypeId) > -1;
    },
  
    onToggleEmploymentTypeSelection (employmentType) {
      const
        me = this,
        index = me.employmentTypes.findIndex(obj => obj.employmentTypeId === employmentType.employmentTypeId);

      if (index > -1) {
        me.employmentTypes.splice(index, 1);
      } else {
        me.employmentTypes.push({
          memberRequestId: me.request.memberRequestId,
          employmentTypeId: employmentType.employmentTypeId
        });
      }
    },
 
    isSexSelected (sex) {      
      return this.sexes.findIndex(obj => obj.sexId === sex.sexId) > -1;
    },
  
    onToggleSexSelection (sex) {
      const
        me = this,
        index = me.sexes.findIndex(obj => obj.sexId === sex.sexId);

      if (index > -1) {
        me.sexes.splice(index, 1);
      } else {
        me.sexes.push({
          memberRequestId: me.request.memberRequestId,
          sexId: sex.sexId
        });
      }
    },

    isCivilStatusSelected (civilStatus) {
      return this.civilStatus.findIndex(obj => obj.civilStatusId === civilStatus.civilStatusId) > -1;
    },
  
    onToggleCivilStatusSelection (civilStatus) {
      const
        me = this,
        index = me.civilStatus.findIndex(obj => obj.civilStatusId === civilStatus.civilStatusId);

      if (index > -1) {
        me.civilStatus.splice(index, 1);
      } else {
        me.civilStatus.push({
          memberRequestId: me.request.memberRequestId,
          civilStatusId: civilStatus.civilStatusId
        });
      }
    },

    isReligionSelected (religion) {      
      return this.religions.findIndex(obj => obj.religionId === religion.religionId) > -1;
    },
  
    onToggleReligionSelection (religion) {
      const
        me = this,
        index = me.religions.findIndex(obj => obj.religionId === religion.religionId);

      if (index > -1) {
        me.religions.splice(index, 1);
      } else {
        me.religions.push({
          memberRequestId: me.request.memberRequestId,
          religionId: religion.religionId
        });
      }
    },

    isDocTypeSelected (docType) { 
      
      return this.docs.findIndex(obj => obj.docTypeId === docType.docTypeId) > -1;
    },
  
    onToggleDocTypeSelection (docType) {
      const
        me = this,
        index = me.docs.findIndex(obj => obj.docTypeId === docType.docTypeId);
       

      if (index > -1) {
        me.docs.splice(index, 1);
      } else {
        me.docs.push({
          memberRequestId: me.request.memberRequestId,
          docTypeId: docType.docTypeId
        });
      }
    },

    isEducationLevelSelected (educationLevel) { 
      return this.educations.findIndex(obj => obj.educationLevelId === educationLevel.educationLevelId) > -1;
    },
  
    onToggleEducationLevelSelection (educationLevel) {
      const
        me = this,
        index = me.educations.findIndex(obj => obj.educationLevelId === educationLevel.educationLevelId);

      if (index > -1) {
        me.educations.splice(index, 1);
      } else {
        me.educations.push({
          memberRequestId: me.request.memberRequestId,
          educationLevelId: educationLevel.educationLevelId
        });
      }
    },


    isMedicalResultSelected (medicalResultType) { 
      return this.medicalResults.findIndex(obj => obj.medicalResultTypeId === medicalResultType.medicalResultTypeId) > -1;
    },
  
    onToggleMedicalResultSelection (medicalResultType) {
      const
        me = this,
        index = me.medicalResults.findIndex(obj => obj.medicalResultTypeId === medicalResultType.medicalResultTypeId);

      if (index > -1) {
        me.medicalResults.splice(index, 1);
      } else {
        me.medicalResults.push({
          memberRequestId: me.request.memberRequestId,
          medicalResultTypeId: medicalResultType.medicalResultTypeId
        });
      }
    },

     onDeploymentDateChanging(e) {

      if (e.proposedValue instanceof DateTime) {
        let me = this,
          p = e.proposedValue;          
          let message ="Deployment Date cannot be less than '<b>" + me.today + "</b>'";

          if (p < me.today ) {
          e.preventDefault();
          me.advice.fault(message, { duration: 5 });
          return;
        }
      }
    },

    onEndDateChanging(e) {

if (e.proposedValue instanceof DateTime) {
  let me = this,
    p = e.proposedValue;          
    let message ="EndDate Date cannot be less than '<b>" + me.today + "</b>'";

    if (p < me.today ) {
    e.preventDefault();
    me.advice.fault(message, { duration: 5 });
    return;
  }
}
},
 
    onMemberRequestPositionIdChanging (e) {
      e.message = "Member Request Position ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberRequestPositionIdCallback;
    },

    memberRequestPositionIdCallback (e) {
      const me = this;
      let filter = "MemberRequestPositionId='" + e.proposedValue + "'";
      return getList('dbo.ArsMemberRequestPosition', 'MemberRequestPositionId, MemberRequestPositionName, MemberRequestPositionDescription', '', filter).then(
        position => {
          if (position && position.length) {
            me.request.memberRequestPositionName = position[0].memberRequestPositionName;
            me.request.jobDescription = position[0].memberRequestPositionDescription;
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

    onMemberRequestPositionIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.request;

      item.memberRequestPositionId = data.memberRequestPositionId;
      item.memberRequestPositionName = data.memberRequestPositionName;
      item.jobDescription = data.memberRequestPositionDescription;
      this.focusNext();

    },

    onClientPayGroupIdChanging (e) {
      e.message = "Client Pay Group ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId='" + e.proposedValue + "' AND ClientPayGroupStatusId=1";
      return getList('dbo.QArsClientPayGroup', 'ClientPayGroupId, ClientPayGroupName, ClientName', '', filter).then(
        payGroup => {
          if (payGroup && payGroup.length) {
            me.request.clientPayGroupName = payGroup[0].clientPayGroupName;
            me.request.clientName = payGroup[0].clientName;
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
        item = this.request;

      item.clientPayGroupId = data.clientPayGroupId;
      item.clientPayGroupName = data.clientPayGroupName;
      item.clientName = data.clientName;
      
      this.focusNext();

    },


    loadData () {
      const
        me = this,
        wait = me.wait();
        
        me.minimumWageList = [];
        me.payOutDailyTotalRate = 0;
        me.payOutMonthlyTotalRate = 0;
        me.billingDailyTotalRate = 0;
        me.billingMonthlyTotalRate = 0;


      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.org = data.org;
            me.platform = data.platform;
            me.contract = data.contract;
            me.position = data.position;
            me.type = data.type;   
            me.payout = data.payout;
            me.educationLevels = data.educationLevel;
            me.region = data.region;
            me.religionList = data.religion;
            me.civilStatusList = data.civilStatus;
            me.sexList = data.sex;
            me.employmentTypeList = data.employmentType;
            me.nciiTitle = data.nciiTitle; 
            me.licenseTitle = data.licenseTitle; 
            me.cdaTitle = data.cdaTitle; 
            me.complianceTitle = data.complianceTitle;
            me.medicalResultTypes = data.medicalResultType; 
            me.payGroup = data.payGroup;
            me.payTrx = data.payTrx; 
            me.applicantScreenings = data.applicantScreening; 
            me.payGroups = data.payGroup;
            me.requestType = data.requestType;
            me.docTypes = data.docType;
          
          }
          if (me.request.memberRequestId< 0) {
            return Promise.resolve(null);
          }
          return me.getRequest();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.request.length) {
        
            me.setModels(info);
          } else {
            if (me.request.memberRequestId > -1) {
              let message = "Member Request ID '<b>" + me.request.memberRequestId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
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

          }
          if (me.isCancelled || me.request.memberRequestStatusId ===4) {
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
      const me = this;
      me.request = me.core.convertDates(info.request[0]);
      me.educations = info.educations;
      me.religions = info.religions;
      me.civilStatus = info.civilStatus;
      me.sexes = info.sexes;
      me.employmentTypes = info.employmentTypes;
      me.typeQualifications = info.typeQualifications;
      me.nciis = info.nciis;
      me.licenses = info.licenses;
      me.cdas = info.cdas;
      me.compliances = info.compliances;
      me.candidateList = info.applicants;
      me.typeQualificationList = info.memberTypeQualifications;
      
      me.medicalResults = info.medicalResults;
      me.billings = info.billings;
      me.payOuts = info.payOuts;
      me.screenings = info.screenings;
      me.docs = info.docs;
      me.updateTypeQualificationList = true;
      me.minimumWageList = info.basic;
      me.onMinimumWageFlagChanged();

      me.refreshTotal();

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldRequest = JSON.stringify(me.request);
      me.oldEducations = JSON.stringify(me.educations);
      me.oldReligions = JSON.stringify(me.religions);
      me.oldCivilStatus = JSON.stringify(me.civilStatus);
      me.oldSexes = JSON.stringify(me.sexes);
      me.oldEmploymentTypes = JSON.stringify(me.employmentTypes);
      me.oldTypeQualifications = JSON.stringify(me.typeQualifications);
      me.oldNCIIs = JSON.stringify(me.nciis);
      me.oldLicenses = JSON.stringify(me.licenses);
      me.oldCDAs = JSON.stringify(me.cdas);
      me.oldCompliances = JSON.stringify(me.compliances);
      me.oldMedicalResults = JSON.stringify(me.medicalResults);
      me.oldBillings = JSON.stringify(me.billings);
      me.oldPayOuts = JSON.stringify(me.payOuts);
      me.oldScreenings = JSON.stringify(me.screenings);
      me.oldDocs = JSON.stringify(me.docs);

    },

    // API calls

    getMinimumWage() {
      const me = this;
      return get("api/member-request-minimum-wages/" + me.request.memberRequestId + "/" + me.request.clientPayGroupId+ "/" + me.request.workingDays + "/" + me.request.deploymentDate+ "/" + me.request.minimumWageFlag);
    },

    getRateDetails() {
      const me = this;
      return get("api/member-request-rate-details/" + me.request.memberRequestId + "/" + me.request.clientPayGroupId+ "/" + me.basicRate+ "/" + me.deminimisRate+ "/" + me.allowanceRate+ "/" + me.request.workingDays+ "/" + me.request.deploymentDate);
    },

    getChangeLog() {
      const
        detail = this.candidate;

      return get("api/member-request-hired-applicants/" + detail.applicantDetailId + "/log");
    },

    getMembers() {
      return get("api/request-members") 
    },

    getMemberTypeQualifications() {
      const me = this;
      return get("api/request-type-qualifications/" + me.request.memberTypeId);
    },

    getRequest () {
      return get('api/member-requests/' + this.request.memberRequestId);
    },

    getClientBillingTransactions() {
      const me = this;
      return get("api/client-billing-transactions/" + me.request.clientPayGroupId);
    },

    getClientPayOutTransactions() {
      const me = this;
      return get("api/client-pay-out-transactions/" + me.request.clientPayGroupId);
    },

    getReferences () {
      const me = this;

      if (me.type.length) {
        return Promise.resolve(true);
      }
      return get('api/references/ars0030');
    },

    createRecord() {
      
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-requests/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);
       
      return ajax('api/member-requests/' + this.request.memberRequestId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        request = this.request,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/member-requests/' + this.request.memberRequestId + '/' + request.lockId, options);
    },

    getApiPayload () {
      const
        me = this,
        request = {};
     
      Object.assign(request, me.request);
      request.educations = me.educations;
      request.religions = me.religions;
      request.civilStatus = me.civilStatus;
      request.sexes = me.sexes;
      request.employmentTypes = me.employmentTypes;
      request.typeQualifications = me.typeQualifications;
      request.nciis = me.nciis;
      request.licenses = me.licenses;
      request.cdas = me.cdas;
      request.compliances = me.compliances;
      request.medicalResults = me.medicalResults;
      request.billings = me.billings;
      request.payOuts = me.payOuts;
      request.screenings = me.screenings;
      request.docs = me.docs;
      
      return request;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('request', 'educations', 'religions', 'civilStatus', 'sexes', 'employmentTypes', 'typeQualifications', 'nciis', 'ncii', 'licenses', 'license','cdas','cda','compliances','compliance', 'medicalResults', 'billings', 'billing', 'payOuts', 'payOut','screenings','docs');
      dc.keyField = 'memberRequestId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.isCancelling = false;
      this.memberList = [];
      this.candidateList = [];
      this.refreshOldRefs();
      setTimeout(() => {
        this.disableElement(
          'btn-add',
          'post-basic-btn',
          'check-rate-btn',
          'recalculate-btn'
        );
      }, 50);
    },

    onMemberRequestIdChanging (e) {
      e.callback = this.memberRequestIdCallback;
    },

    memberRequestIdCallback (e) {
      e.message = "Member Request ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.ArsMemberRequest', 'memberRequestId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onMemberRequestIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onMemberRequestIdLostFocus () {
      const me = this;

      if (!me.request.memberRequestId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberRequestIdSearchFill (e) {

      const filter = "OrgId = " + this.sym.userInfo.userOrgId

      if (e.filter === '') {       
        e.filter = filter
      } else {
        e.filter = filter + " AND " + e.filter
      }
      
    },

    onMemberRequestIdSearchResult (result) {
      if (!result) { return; }

      const 
        me = this,
        data = result[0];

      me.request.memberRequestId = data.memberRequestId;
      me.replaceUrl();
      me.loadData();
    },


    onSubmit (nextRoute) {
      const me = this;
      if (me.billingMonthlyTotalRate<me.payOutMonthlyTotalRate ) {
        me.advice.fault('Pay Out total should not be greater than billing.', { duration: 5 } )
        return;
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
            if (isNew && typeof success === 'number' && success > 0) {
              me.request.memberRequestId = success; 
            }

            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'

              if (me.isCancelling) {
                message = "MRF# '" + me.request.memberRequestId + "' cancelled."
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

      getSafeDeleteFlag('ArsMemberRequest', me.request.memberRequestId)
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

    // helpers
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
          'post-basic-btn',
          'check-rate-btn',
          'recalculate-btn'
        );

        me.setDisplayMode(
          'memberRequestName',
          'clientContractId',
          'employmentTypeId',
          'requestTypeId',
          'employmentEndDate',
          'clientPosition',
          'jobCode',
          'jobDescription',
          'memberTypeId',
          'payoutTypeId',
          'vacancyCount',
          'deploymentDate',
          'positionKeyword',
          'skillNameKeyword',
          'clientContractName',
          'orgName',
          'platformName',
          'memberRequestPositionName',
          'jobDescription',
          'clientPayGroupId',
          'clientPayGroupName',
          'workingDays',
          'memberName',
        );

        me.setFocus('memberRequestName');
      }, 100);

    },


    setupControls () {
      const me = this;

      setTimeout(() => {

        me.enableElement(
          'btn-add',
          'post-basic-btn',
          'check-rate-btn',
          'recalculate-btn'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'memberRequestName',
          'employmentTypeId',
          'requestTypeId',
          'clientPosition',
          'memberRequestPositionId',
          'jobCode',
          'jobDescription',
          'memberTypeId',
          'payoutTypeId',
          'vacancyCount',
          'deploymentDate',
          'regionId' ,
          'clientPayGroupId',
          'workingDays',
        );

        me.setDisplayMode(
          'clientContractName',
          'orgName',
          'platformName',
          'memberRequestPositionName',
          'clientPayGroupName',
          'clientName',
          'memberName',
        );


        me.setOptionalMode(
          'memberRequestPerks',
        );

        me.setFocus('memberRequestName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      if (!me.isNew() && me.noEditFlag) {
        return false;
      }
      
      if (JSON.stringify(me.request) !== me.oldRequest) { return true; }
      if (JSON.stringify(me.educations) !== me.oldEducations) { return true; }
      if (JSON.stringify(me.religions) !== me.oldReligions) { return true; }
      if (JSON.stringify(me.civilStatus) !== me.oldCivilStatus) { return true; }
      if (JSON.stringify(me.sexes) !== me.oldSexes) { return true; }
      if (JSON.stringify(me.employmentTypes) !== me.oldEmploymentTypes) { return true; }
      if (JSON.stringify(me.typeQualifications) !== me.oldTypeQualifications) { return true; }
      if (JSON.stringify(me.nciis) !== me.oldNCIIs) { return true; }
      if (JSON.stringify(me.licenses) !== me.oldLicenses) { return true; }
      if (JSON.stringify(me.cdas) !== me.oldCDAs) { return true; }
      if (JSON.stringify(me.compliances) !== me.oldCompliances) { return true; }
      if (JSON.stringify(me.medicalResults) !== me.oldMedicalResults) { return true; }
      if (JSON.stringify(me.billings) !== me.oldBillings) { return true; }

      if (JSON.stringify(me.payOuts) !== me.oldPayOuts) { return true; }
      if (JSON.stringify(me.screenings) !== me.oldScreenings) { return true; }
      if (JSON.stringify(me.docs) !== me.oldDocs) { return true; }
      return false;
    },

  },

  created () {
    const me = this;

    me.isCancelling = false;


    me.oldRequest = '';
    me.oldEducations = '';
    me.oldReligions = '';
    me.oldCivilStatus = '';
    me.oldSexes = '';
    me.oldEmploymentTypes = '';
    me.oldTypeQualifications = '';
    me.oldNCIIs = '';
    me.oldLicenses = '';
    me.oldCDAs = '';
    me.oldCompliances = '';
    me.oldMedicalResults = ''; 
    me.oldBillings = '';
    me.oldPayOuts = '';
    me.oldScreenings = '';
    me.oldDocs = '';

    me.org = [];     
    me.platform = [];
    me.contract = [];  
    me.position = [];     
    me.type = [];
    me.payout = [];
    me.region = []; 
    me.educationLevels = []; 
    me.medicalResultTypes = []; 
    me.payGroup = [];  
    me.religionList = []; 
    me.civilStatusList = []; 
    me.sexList = []; 
    me.employmentTypeList = []; 
    me.typeQualificationList = []; 
    me.nciiTitle = []; 
    me.licenseTitle = []; 
    me.cdaTitle = []; 
    me.complianceTitle = []; 
    me.memberList = [];
    me.candidateList = [];
    me.payTrx = [];   
    me.applicantScreenings = []; 
    me.requestType = []; 
    me.docTypes = []; 
  }

}

</script>


<style scoped>
.command-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.buttons {
  flex-wrap: nowrap;
}


.header-containerX{
  display: flex;
  flex-direction: column;
}
.mrf-id{
  display: flex;
  flex-direction: row;
  gap: .5rem;
  margin-bottom: .5rem;

}

.sub-container{
  display: grid;
  grid-template-rows: .5fr .5fr ;
  gap: .5rem;
  margin-top: 10px;

}
.container-1{
  display: flex;
  flex-direction: column;
}
.container-2{
  display: flex;
  flex-direction: column;
}
.container-3{
  display: flex;
    flex-direction: column;
    gap: 1rem;
}
.skill-id{
  display: flex;
  flex-direction: row;
  gap: 1rem;
}
.skill-dtl{
  margin-top: 10px;
  display: flex;
  flex-direction: column;
 
  width: 35%;
}
.first-act-btn{
  display: flex;
  flex-direction: row ;
}
.action-buttons {
display: grid;
grid-template-columns: 2fr 2fr 2fr;
}
.detail-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 2fr 7fr 2fr;
  gap: .5rem;
}

#detail-editor >>> .modal-rg {
  min-width: 75%;
}
.fixed-header {
  overflow: hidden;
  max-height: 70vh;
}


.fixed-header th {
  position: sticky;
  top: 0;
  background: rgb(221, 221, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
}
.buttons{
  display: flex;
 justify-content:center;
}
.act-btns{
  display: flex;
 justify-content:space-evenly;
}
.select-btns{
margin: 10px 0 10px 0;
width: 100%;
}
.applicant-btn{
padding: 10px;
display: flex;
justify-content: space-between;
width: 5vw;
}
.fixed-header thead {
  z-index: 1;
  position: relative;
}
.id{
  width: 5%;
}
.name{
    width: 50%;
}
.sort{
    width: 5%;
}
.action{
    width: 10%;
}
#logs tr {
  vertical-align: top;
}

#logs td {
  font-size: .875rem;
  padding: .375rem;
}
.contract-fields{
  display: grid;
  grid-template-columns: 3fr 5fr;
  gap: .5rem;
  width: 100%;
}
.internal-pos-field{
  display: grid;
  grid-template-columns: 3fr 5fr;
  gap: .5rem;
  width: 100%;
}
.payout-field{
  display: grid;
  grid-template-columns: 3fr 3fr 3fr;
  gap: .5rem;
  width: 100%;
}


.Header {
  width: 100%;
  border: 0;
  padding: 10px;
  text-align: center;
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: white;
  text-align: center;
  text-transform: uppercase;
}
.mri-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.education-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.employement-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.qualification-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.ncii-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.license-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.cda-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.compliance-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.pooling-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.candicate-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
}
.religion-info{
  
  max-height: 50vh;
  overflow: auto;

}

.civil-info{

  max-height: 50vh;
  overflow: auto;
}
.sex-info{
   
  max-height: 50vh;
}
.box-container{
  display: grid;
  grid-template-columns: 5fr 5fr 5fr ;
  gap: .5rem;
  margin-top: 10px;
}
.location-field{
  display: grid;
  grid-template-columns: 1fr 1fr ;
  gap: .5rem;
}
.location-two{
  display: grid;
  grid-template-columns: 2fr 2affr  1fr;
  gap: .5rem;
}
.pooling-name{
  width: 20%;
}
.pooling-birth{
  width: 10%;
}
.pooling-age{
  width: 5%;
}
.pooling-sex{
  width: 5%;
}
.pooling-mobile{
  width: 10%;
}
.pooling-action{
  width: 5%;
}
.candidate-id{
  width: 7%;
}
.candidate-name{
  width: 20%;
}
.candidate-birth{
  width: 10%;
}
.candidate-age{
  width: 5%;
}
.candidate-sex{
  width: 5%;
}
.candidate-mobile{
  width: 10%;
}
.candidate-deploy{
  width: 10%;
}
.candidate-action{
  width: 6%;
}

.header-containerX {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.request-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}
  .box-1{ 
  display: grid;
  grid-template-columns: 2fr 2fr  .7fr  2fr;
  gap: .5rem;
}
.box-2{
  display: grid;
  grid-template-columns: 1fr 1.7fr 1.5fr ;
  gap: .5rem;
}
.box-3{
  display: grid;
  grid-template-columns: .5fr 1.5fr .7fr 1fr 1fr 1fr 1fr ;
  gap: .5rem;
}
.box-4{
  display: grid;
  grid-template-columns: 1fr 2fr .6fr .6fr .4fr .6fr 4fr; 
  /* border:1px solid black; */
  gap: .5rem;
}

.box-5{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;

}
.box-8{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-6{
  display: grid;
  grid-template-columns: .7fr 1.5fr 1.5fr;
  gap: .5rem;
}
.box-7{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .5rem;
}
.basic-scroller{
  overflow: auto;
  height: 20vh;
}
.basic-table-scroller{
  width: 850vw;
}
.basic-header{
  width: 850vw;
  border: 0;
  padding: 10px;
  text-align: center;
  font-weight: bold;
  margin-bottom: 0.5rem;
  color: white;
  text-align: center;
  text-transform: uppercase;
}
.wage-table-scroller{
  overflow: auto;
}
.wage-fixed-header{
  height: 45vh;
}
.wage-btns{
  display: grid;
  grid-template-columns: .5fr .8fr .5fr;
  width: 30vw;

}
.wage-field{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;

}

.table-field-sroller {
  max-height: 50vh;
  overflow-y: auto;
  border-radius: 4px;
}

.table-field-sroller {
  max-height: 50vh;
  max-width: 100%;
  overflow-y: auto;
  border-radius: 4px;
  position: relative;
}
.table-fixed-header thead th {
  position: sticky;
  top: 0;
  background-color: rgb(216, 223, 172);
  z-index: 2;
  width: 120%;
  max-width: 100%;
}
.table-fixed-header tfoot tr {
  position: sticky;
  bottom: 0;
  background-color: #f0f0f0;
  z-index: 1;
  font-weight: bold;
  border-top: 2px solid #ccc;
}
.table-fixed-header th,
.table-fixed-header td {
  padding: 10px;
  border-bottom: 1px solid #ddd;
}
.table-fixed-header tbody tr:nth-child(even) {
  background-color: #f6f6f6;
}
.billing-editor-boxes{
  display: grid;
  grid-template-columns: 2fr 1fr .5fr;
  
}

.memberid:hover{
  color: red;
  text-decoration: underline;
  cursor: pointer;
}
.table-container{
  overflow: auto;
  max-height: 50vh;
  height: 100%;
}
</style>
