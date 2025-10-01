// Member Request Pooling 

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars0050" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main frs-form-footer darken-2 py-0" bodyClass="frs-form-body pb-3 ">

  
  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">

      <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
 
      <button type="button" class="justify-between w-20" @click="onRefresh"><i class="fa fa-refresh fa-lg"></i><span>Refresh</span></button>
      <button type="button" class="justify-between btn-clear" @click="onClear" > <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>

 
    </div>


    <div></div>

  </div>

  <div class="header-containerX">
    <div class="box-main-container">

    

     <div class="mrf-id">
      <sym-int v-model="request.memberRequestId" :caption-width="20" caption="MRF ID" lookupId="ArsMemberRequestPooling" @lostfocus="onMemberRequestIdLostFocus" @changing="onMemberRequestIdChanging" @changed="onMemberRequestIdChanged" @searchresult="onMemberRequestIdSearchResult" @searchfill="onMemberRequestIdSearchFill"></sym-int>
     <div class="buttons d-inline">
    </div>
      <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled && request.memberRequestId">Cancelled</sym-tag>
      <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActivated && !isClosed && request.memberRequestId">Active</sym-tag>
      <sym-tag class="warning text-center border-light lg-2 ml-3" :width="38" v-if="isNotActive && request.memberRequestId">In-Active</sym-tag>
      <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isClosed && request.memberRequestId">Completed</sym-tag>

      <sym-tag v-if="request.memberRequestId" class="info text-center border-light lg-2 ml-3" :width="70">Total Hired : {{request.totalHired}}</sym-tag>
      <sym-tag v-if="request.memberRequestId" class="success text-center border-light lg-2 ml-3" :width="70">Current Vacancy : {{request.totalVacancy}}</sym-tag>
      <sym-tag v-if="request.memberRequestId" class="warning text-center border-light lg-2 ml-3" :width="70">Total Applicant/s : {{request.totalApplicant}}</sym-tag>
 
    </div>


    <sym-tabs id="request-info-tabs" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">


      <sym-tab title="Member Request Information" icon="user-o">
        <div class="main-container gap">
          <div class="sub-container">
            <div class="container-1">
            
                <div class="app-box-style gap"> 
                  <div class="Header app-form-header curved-1 mb-2 ">MRF DETAILS</div>
                    <table class="box-table " v-show="request.memberRequestId !== 0">
                      <tr>
                        <td class="box-description">CLIENT NAME</td>
                        <td>{{request.memberRequestName.toLocaleUpperCase()}}</td>
                        
                      </tr>
                      <tr>
                        <td class="box-description">CONTRACT NAME</td>
                        <td>{{request.clientContractName.toLocaleUpperCase()}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">POSITION</td>
                        <td>{{request.memberRequestPositionName.toLocaleUpperCase()}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">PAY GROUP</td>
                        <td>{{request.clientPayGroupName.toLocaleUpperCase()}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">PAYOUT TYPE</td>
                        <td>{{request.payoutTypeName }}</td>
                      </tr>
                      <tr>
                        <td class="box-description">MEMBER TYPE</td>
                        <td>{{request.memberTypeName}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">VACANCY COUNT</td>
                        <td>{{request.vacancyCount}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">WORKING DAYS</td>
                        <td>{{request.workingDays}}</td>
                      </tr>
                      <tr>
                        <td class="box-description">LOCATION</td>
                        <td>{{ request.regionName }}  {{ request.provinceName }}  {{ request.municipalityName }}</td>
                      </tr>
                      <tr>
                        <td class="box-description">TARGET DEPLYOMENT DATE</td>
                        <td>{{request.deploymentDate }}</td>
                      </tr>  
                    </table>
                  </div>
                </div>

            <div class="container-2">
                <div class="app-box-style gap ">
                <div class="Header app-form-header curved-1 ">Pay Out</div>
                
                <div class="table-scroller">
                <div class="fixed-header">
                  <table class="light striped-even mb-0 " v-show="request.memberRequestId != 0">
                  <thead>
                    <tr>
                      <th class="text-center w-20">Trx Code </th>
                      <th class="text-right w-30">Daily</th>
                      <th class="text-right w-30">Monthly</th>

                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(dtl, index) in payOuts" :key="index" >
                      <td>{{ dtl.payTrxCode }}</td>
                      <td class="text-right">{{ dtl.dailyRate }}</td>
                      <td class="text-right">{{ dtl.monthlyRate }}</td>

                    </tr>
                    <tr>
                <td colspan="1" class="text-right bold" v-show="request.memberRequestId != 0">Total</td>
                <td class="text-right" v-show="request.memberRequestId != 0">{{ core.toDecimalFormat(payOutDailyTotalRate) }}</td>
                <td class="text-right" v-show="request.memberRequestId != 0">{{ core.toDecimalFormat(payOutMonthlyTotalRate) }}</td>
              </tr>                


                  </tbody>
                </table>
                </div>
              </div>
                </div>    
                      
                   
            </div> 

            <div class="container-3">
               <div class="app-box-style gap">
                <div class="Header app-form-header curved-1 ">Billing</div>
                <div class="fixed-header">
                  <table class="light striped-even mb-0 "  v-show="request.memberRequestId != 0">
                  <thead>
                    <tr>
                      <th class="text-center w-20">Trx Code </th>
                      <th class="text-right w-30">Daily</th>
                      <th class="text-right w-30">Monthly</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(dtl, index) in billings" :key="index ">
                      <td>{{ dtl.payTrxCode }}</td>
                      <td class="text-right" >{{ dtl.dailyRate }}</td>
                      <td class="text-right" >{{ dtl.monthlyRate }}</td>
                    </tr>
                    <tr>
                <td colspan="1" class="text-right bold" v-show="request.memberRequestId != 0">Total</td>
                <td class="text-right" v-show="request.memberRequestId != 0">{{ core.toDecimalFormat(billingDailyTotalRate) }}</td>
                <td class="text-right" v-show="request.memberRequestId != 0">{{ core.toDecimalFormat(billingMonthlyTotalRate) }}</td>
              </tr>                
                  </tbody>
                </table>
              
              </div>
                </div> 
                  
            </div>
            
          </div>  

             
         </div>

      </sym-tab> 
         <sym-tab title="Member List" icon="user-o">

            <div class="app-box-style w-100">
              <div class="Header app-form-header curved-1 mb-2">Filter/s</div>
                <div class="box-remarks">
  
                  <sym-memo v-model="request.memberRequestRemarks" align="bottom" :caption-width="10" :input-height="1" caption="Additional Qualifications/Requirements" ></sym-memo>    
                  <sym-memo v-model="request.memberRequestPerks" align="bottom" :caption-width="40" :input-height="1" caption="Additional Perks/Benefits." ></sym-memo>  
                  <sym-memo v-model="request.positionKeyword" align="bottom" :caption-width="10" :input-height="1" caption="Position (Type each position, separated by a semicolon [;])"></sym-memo>
                    <sym-memo v-model="request.skillNameKeyword" align="bottom" :caption-width="10" :input-height="1" caption="Skill (Type each skill, separated by a semicolon [;])" ></sym-memo>
                </div> 
  
                <div class="box-row">
                    
                  <sym-combo v-model="request.educationLevelId"  :caption-width="35" caption="Education Level" display-field="educationLevelName" :datasource="educationLevels" ></sym-combo>
                  <sym-int v-model="request.age"  :caption-width="20" caption="Age"></sym-int>
                  <sym-combo v-model="request.regionId"  :caption-width="20" caption="Region" display-field="regionName" :datasource="region" @changed="onRegionIdChanged()"></sym-combo>
                  <sym-combo v-model="request.provinceId" :caption-width="20" caption="Province" display-field="provinceName" :datasource="provinces" @changed="onProvinceIdChanged()" ></sym-combo>
                  <sym-combo v-model="request.municipalityId"  :caption-width="30" caption="Municipality"  display-field="municipalityName" :datasource="municipalities" ></sym-combo>
                </div> 
            
                <div class="sub-container-2 ">                  
                    <div class="box-1">

                      <!-- QUALIFICATION -->
                         <div class="memee">
                              <div class=" border-soft w-100  app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleQualificationTypeVisibility" class="btn btn-toggle curved-1">
                                          <i :class="qualificationTypeVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                            
                                      <div class="Header app-form-header curved-1" > Qualification Type</div>
                                    </div>
                                    <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light" v-show="qualificationTypeVisible">
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
                      <!-- RELIGION -->
                       <div class="memee">
                      <div class=" border-soft w-100  app-box-style"> 
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleReligionVisibility" class="btn btn-toggle">
                                          <i :class="religionVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                                      <div class="Header app-form-header curved-1 religion-fields">Religion</div>
                                    </div>
                                    <div class="religion-scroller">

                                      <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light" v-show="religionVisible">
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
                      </div>
                       <div class="memee">
                        <!-- MEDICAL -->
                      <div class=" border-soft w-100  app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleMedicalResultTypeVisibility" class="btn btn-toggle">
                                            <i :class="medicalResultTypeVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>

                                      </div>
                                      <div class="Header app-form-header curved-1">Medical Result</div>
                                    </div>
                                    <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light" v-show="medicalResultTypeVisible">
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
                      <!-- CIVIL STATUS -->
                       <div class="memee">
                      <div class=" border-soft w-100  app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleCivilStatusVisibility" class="btn btn-toggle">
                                          <i :class="civilStatusVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                                      <div class="Header app-form-header curved-1">Civil Status</div>
                                    </div>
                                    <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light" v-show="civilStatusVisible">
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
                    </div>

                    <div class="box-3 ">
                       <!-- SEX -->
                      <div class="memee">
                      <div class=" border-soft w-100  app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleSexVisibility" class="btn btn-toggle">
                                        <i :class="sexVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                                      <div class="Header app-form-header curved-1">Sex</div>
                                    </div>
                                    <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light" v-show="sexVisible">
                                    
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
                      <!-- NCII -->
                      <div class="memee">
                      <div class="border-soft w-100 app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleNCIIVisibility" class="btn btn-toggle">
                                          <i :class="nciiVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                                      <div class="Header app-form-header curved-1">NCII Title</div>
                                    </div>
                                        
                                      <div class="table-scroller" v-show="nciiVisible">
                                        <div class="fixed-header" >
                                          <table class="light striped-even mb-0 ">
                                          <thead>
                                            <tr>
                                              <th class="w-70">NCII Title</th>
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
                                      <div class="command-buttons light border-main p-1 border-top-0 mb-2" v-show="nciiVisible">
                                        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                                          <button type="button" class="justify-between btn-add" @click="onAddNCII"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                                        </div>
                                      </div>
                      </div>
                      </div>     
                      <!-- LICENSE -->
                       <div class="memee">
                      <div class="border-soft w-100  app-box-style">
                                    <div class="box-container">
                                      <div class="plus-btn">
                                        <button @click="toggleLicenseVisibility" class="btn btn-toggle">
                                          <i :class="licenseVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                        </button>
                                      </div>
                                      <div class="Header app-form-header curved-1">License Profession</div>    
                                    </div>
                                    <div class="table-scroller" v-show="licenseVisible">
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
                                      <div class="command-buttons light border-main p-1 border-top-0 mb-2" v-show="licenseVisible">
                                        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                                          <button type="button" class="justify-between btn-add" @click="onAddLicense"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                                        </div>
                                      </div>
                      </div>
                      </div>   
                      <!-- COMPLIANCE  -->
                       <div class="memee">
                      <div class="border-soft w-100 app-box-style">
                                      <div class="box-container">
                                        <div class="plus-btn">   
                                          <button @click="toggleComplianceVisibility" class="btn btn-toggle">
                                          <i :class="complianceVisible ? 'fa fa-minus' : 'fa fa-plus'"></i> 
                                          </button>
                                        </div>
                                        <div class="Header app-form-header curved-1">Compliance Training</div>
                                  
                                      </div>
                                      <div class="table-scroller" v-show="complianceVisible">
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
                              <div class="command-buttons light border-main p-1 border-top-0 mb-2" v-show="complianceVisible">
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




  
 
              <!-- Pooling -->
              <div class="app-box-style gap w-100" v-if="!isClosed">
                    <div class="Header app-form-header curved-1"><span>Pooling : {{ memberList.length }}</span></div>                      
                    <div class="select-btns gap " :info-light="isMono" :outline="isMono"  border-main fw-28 mb-0 sm-1 shadow-light>
                        <button type="button" :class="logButtonClass" class=" applicant-btn success-light" @click="onPooling" v-if="isActivated && !isClosed" >
                          <i class="fa fa-search fa-lg"></i><span class="bold">Search</span>
                        </button>

                        <button type="button" :class="logButtonClass" class=" applicant-btn success-light" @click="onResetFilter" v-if="isActivated && !isClosed">
                        <i class="fa fa-undo fa-lg"></i><span class="bold">Reset Filter</span>
                        </button>
                        <div class="mb-0">
                          <sym-text plain vertical  v-model="memberName" :input-width="100" v-if="isActivated && !isClosed" ></sym-text>
                        </div>
                      </div>
                          <div class="pooling-scroll-wrapper">
                              <div class="pooling-fixed-header">
                            <table class="pooling-scroll light striped-even mb-0 ">
                            <thead class="pooling-thead">
                          <tr>
                            <th class="action text-center">Action</th>
                            <th class="id">Member ID</th>
                            <th class="name">Name</th>
                            <th class="age">Age</th>
                            <th class="sex">Sex</th>
                            <th class="mobile">Mobile</th>
                            <th class="region">Region</th>
                            <th class="province">Province</th>
                            <th class="municipality">Municipality</th>
                            <th class="status">Status</th>  
                            
                          </tr>
                        </thead>
                        <tbody class="pooling-tbody">
                          <tr v-for="(dtl, index) in memberList" :key="index">
                            <td class="p-1">
                              <div class="act-btns gap" sm-1 mb-0 >
                                <button type="button" class=" info  " title="Add Member" @click="onAddMember(dtl, index)">
                                  <i class="fa fa-user-plus fa-lg"></i>
                                </button>
                                <button type="button" class="warning" title="Delete" @click="onRemoveMember(index)">
                                  <i class="fa fa-times fa-lg"></i>
                                </button>
                              </div>
                            </td>
                            <td>{{ dtl.memberId }}</td>
                            <td class="name">{{ dtl.memberName }}</td>
                            <td>{{ dtl.age }}</td>
                            <td>{{ dtl.sexId }}</td>
                            <td>{{ dtl.mobileNumber  }}</td> 
                            <td class="region">{{ dtl.regionName }}</td>
                            <td class="province">{{ dtl.provinceName }}</td>
                            <td class="municipality">{{ dtl.municipalityName }}</td>
                            <td>{{ dtl.memberStatusName }}</td>
                        
                          
                          
                          </tr>
                        </tbody>
                      </table>
                              </div>
                          </div>
            
              </div>
              </sym-tab>

      <sym-tab title="Candidate List" icon="user-o">

          <div class="d-grid gap">               
              <!-- New Applicants -->
              <div class="app-box-style gap"  v-if="!isClosed">
                    <div class="Header app-form-header curved-1"><span>Non-Member Applicant : {{ newCandidateList.length }}</span></div>                    
                    <div class="table-scroller">
                    <div class="fixed-header">
                      <table class="table-scroll light striped-even mb-0 ">
                      <thead>
                        <tr>
                          <th class="w-3 ">App ID</th>
                          <th class="w-15">Name</th>
                     
                          <th class="w-2">Age</th>
                          <th class="w-2">Sex</th>
                          <th class="w-8">Mobile</th>
                          <th class="w-15">Region</th>
                          <th class="w-10">Province</th>
                          <th class="w-10">Municipality</th>
                          <th class="w-10">Religion</th>
                          <th class="w-6">Civil Status</th>
                          <th class="w-3 text-center">Action</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="(dtl, index) in newCandidateList" :key="index" :class="getRowColor(dtl)" >
                          <td class="submit-hot memberid" @click="onClickApplicant(dtl.applicantId)">{{ dtl.applicantId }}</td>
                          <td>{{ dtl.applicantName }}</td>
                          <td>{{ dtl.age }}</td>
                          <td>{{ dtl.sexId }}</td>
                          <td>{{ dtl.mobileNumber }}</td>
                          <td>{{ dtl.regionName }}</td>
                          <td>{{ dtl.provinceName }}</td>
                          <td>{{ dtl.municipalityName }}</td>
                          <td>{{ dtl.religionName }}</td>
                          <td>{{ dtl.civilStatusName }}</td>
                          <td class="p-1">
                            <div class="act-btns" sm-1 mb-2 >
                              <button :disabled="!dtl.screeningFlag==0 || isClosed" type="button" class="justify-between success" title="Details" @click="onCreateMemberRecord(dtl, index)">
                                <i class="fa fa-check fa-lg fa-align-justify"></i>
                              </button>
                            </div>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                    </div>
                  </div>
              </div> 

              <!-- Candidate -->

              <div class="app-box-style gap"  v-if="!isClosed">
                    <div class="Header app-form-header curved-1"><span>Member Applicant : {{ candidateList.length }}</span></div>
                    
              
                    
                    <div class="table-scroll-wrapper">
                        <div class="fixed-header">
                      <table class="table-scroll light striped-even mb-0 ">
                      <thead>
                        <tr>
                          <th class="w-5 ">Member ID</th>
                          <th class="candidate-name">Name</th>
                          
                          <th class="candidate-age">Age</th>
                          <th class="candidate-sex">Sex</th>
                          <th class="candidate-mobile">Mobile</th>
                          
                          
                          <th class="candidate-religion">Religion</th>
                          <th class="candidate-civil">Civil Status</th>
                          <th class="candidate-status">App Status</th>
                          <th class="candidate-member-stat">Member Status</th>
                          <th class="candidate-action text-center">Action</th>
                          <th class="candidate-others text-center">Other</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="(dtl, index) in candidateList" :key="index" :class="getRowColor(dtl)" >
                          <td class="submit-hot memberid" @click="onClickMember(dtl.memberId)">{{ dtl.memberId }} </td>

                          

                          <td>{{ dtl.memberName }}</td>
                        
                          <td>{{ dtl.age }}</td>
                          <td>{{ dtl.sexId }}</td>
                          <td>{{ dtl.mobileNumber }}</td>
                          
                          
                          <td>{{ dtl.religionName }}</td>
                          <td>{{ dtl.civilStatusName }}</td>
                          <td>{{ dtl.applicantStatusName }}</td>
                          <td>{{ dtl.memberStatusName }}</td>
                          <td class="p-1">
                            <div class="act-btns gap" sm-1 mb-0 >
                                <button v-show="dtl.screeningFlag==1 && dtl.docTypeCount!==0 && !isClosed"  type="button" class="justify-center w-100 dark-light  " title="Id's" @click="onAddDoc(dtl, index)">
                                  <i class="fa  fa-lg fa-id-card-o"></i>
                                </button>


                                <button v-show="!isClosed && dtl.applicantStatusId===1 && dtl.screeningFlag===0 " type="button" class=" justify-center success w-100" title="Process" @click="onProcessMember(dtl, index)">
                                  <i class="fa fa-lg fa-gear"></i>
                                </button>
                                
                                <button v-show="dtl.screeningFlag===1 && dtl.docTypeCount===0 && !isClosed && dtl.applicantStatusId!==3" type="button" class="justify-center w-100 info " title="Hire" @click="onHireMember(dtl, index)">
                                  <i class="fa fa-lg fa-check-square-o"></i>
                                </button>

                
                            </div>
                          </td>
                          <td class="p-1">
                            <div class="others-btns gap" sm-1 mb-0 >

                                <button  type="button" class=" primary-light " title="Progress" @click="onShowProgress(dtl, index)">
                                  <i class="fa  fa-lg fa-line-chart "></i>
                                </button>
                                
                                <button type="button" :class="logButtonClass" class=" btn-log "  title="Logs" @click="onViewLog(index)">
                                  <i class="fa fa-lg  fa-clock-o"></i>
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

      </sym-tab>


      </sym-tabs>  
       </div> 
       </div>

      </sym-form>
      

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
    <form id="ars0050A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
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
    <form id="ars0050B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
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
    <form id="ars0050C" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
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
    <form id="ars0050D" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
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
        <sym-combo ref="myCombo"   v-show="isCandidateInProcessVisible" v-model="candidate.applicantScreeningId" caption="Hiring Process" align="bottom" display-field="applicantScreeningName" :datasource="applicantScreenings"  @changed="onApplicantScreeningIdChanged(candidate.applicantScreeningId)"></sym-combo>
        <sym-combo v-show="isCandidateInProcessVisible" v-model="candidate.screeningStatusId" caption="Status" align="bottom" display-field="screeningStatusName" :datasource="screeningStatus" ></sym-combo>

        <sym-int v-model="candidate.memberId" caption="Member ID" align="bottom"></sym-int>
        <sym-text v-model="candidate.memberName" caption="Member Name" align="bottom"></sym-text>
        <sym-memo v-model="candidate.remarks" caption="Remarks" align="bottom"></sym-memo>
        <div class="app-grid-column-2 gap">
        <sym-date v-show="isCandidateHireVisible" v-model="request.deploymentDate" caption="Target Deployment Date" align="bottom" ></sym-date>
        <sym-date v-show="isCandidateHireVisible" v-model="candidate.hiredDate" caption="Effective Hired Date" align="bottom" @changing="onHiredDateChanging"></sym-date>
      </div>
       
        <div class="doc-editor-boxes"  v-if="!isCandidateHireVisible && isUploadRequired" >
              <sym-combo v-model="candidate.requestDocTypeId" caption="Doc Type" align="bottom" display-field="docTypeName" :datasource="tradeTestDocType" @changing="onRequestDocTypeIdChanging"  ></sym-combo>
              
              <div class="doctype-fields" >
                <div class="upload-files">
                  <sym-tag class="upload-text">{{ fileName }}</sym-tag>
                  <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
                </div>
              </div>
        </div>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="danger justify-between border-main" @click="onRemoveCandidate()"><i class="fa fa-close mr-2"></i>Failed</button>
        
        <button type="button" class="info justify-between border-main" v-if="isSubmiBtnDisable" @click="onSubmitCandidate()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isCandidateEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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
            <div class="doc-editor-boxes">
             
              <sym-combo v-model="doc.docTypeId" caption="Doc Type" align="bottom" display-field="docTypeName" :datasource="docType" @changing="onDocTypeIdChanging" @changed="onDocTypeIdChanged(doc.docTypeId)"></sym-combo>
              <div class="doctype-fields" v-show="doc.docTypeId != 0">
              
              <div class="MobileContainer">
                <span class="MobileHeader">Reference</span>
                <input type="text" v-model="doc.docTypeReference" @input="validateInput" :maxlength="doc.docTypeLength" placeholder="Enter reference" @changing="onDocTypeReferenceChanging"> 
              </div>
              <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            
            </div>
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
<!-- Progress -->

<!-- Candidate -->
<sym-modal
  id="progress-editor"
  v-model="isProgressEditorVisible"
  size="lg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorCandidateTitle"
  @show="onShowProgressEditor($event)"
  @hide="onHideProgressEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0050X" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="vaccine-editor-boxes">

    <table class="table-scroll light striped-even mb-0 "> 
      <thead>
        <tr class="align-top">
          <th class="w-25">Screening</th>          
          <th class="w-45 text-center">File Name</th>          
          <th class="w-10 text-center">Upload</th> 
          <th class="w-10 text-center">Date</th>
          <th class="w-10 text-center">Status</th>
       
        </tr>
      </thead>
      <tbody class="white">
        <tr class="align-top" v-for="(dtl, index) in applicantScreeningProgress" :key="index">

          <td >{{ dtl.applicantScreeningName }}</td>
          <td class="text-center">{{ dtl.requestDocTypeFileName }}</td>
          <td>
          <div v-if="dtl.fileUrl">
                           <button class="button info sm-4 ml-2" @click="openDocPreview(dtl.fileUrl, dtl.requestDocTypeFileName)"><i class="fa fa-image mr-2"></i><span class="button-caption ml-2">Preview</span>  </button>
                        <!-- </div> -->


          </div>
          </td>


          
          <td class="text-center">{{ core.toDateFormat(dtl.screeningDate, true, 'MM/dd/yyyy' ) }}</td>
          <td class="text-center">{{ dtl.screeningStatusName }}</td>
        </tr>
      </tbody>
    </table>


      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="warning justify-between border-main mr-0" @click="isProgressEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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


     <!-- Image  -->
  <sym-modal
        id="doc-editor"
        v-model="isDocPreviewVisible"
        size="lg"
        :header="true"
        :customBody="true"
        :footer="false"
        :keyboard="false"
        :dismissible="false"
        :closeOnBackButton="false"
        :title="editorDocPreviewTitle"
        headerClass="app-form-header"
        dismissButtonClass="danger"
      >
        <!-- <div class="board p-1 mb-0 w-100" @click.self="closeDocPreview">
          <form id="ars0050Z" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
            <div class="detail-editor-boxes"> -->


  <div class="board p-1 mb-0 w-100" @click.self="closeDocPreview">
    
    <form id="ars0050Z" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
            <div class="preview-file">
              <!-- <iframe :src="previewFileUrl" width="100%" height="600px"></iframe> -->
<!-- <iframe :src="previewFileUrl" width="100%" height="600px"></iframe>

<div class="image-container">
  <img :src="previewFileUrl" :style="{ transform: 'scale(' + imageZoom + ')' }" />
</div> -->


<div class="preview-file">
  <template v-if="isPDF">
    <iframe :src="previewFileUrl" width="100%" height="600px"></iframe>
  </template>

  <template v-else>
    <div class="image-container">
      <img :src="previewFileUrl" :style="{ transform: 'scale(' + imageZoom + ')' }" />
    </div>
  </template>

  <!-- <template v-else>
    <p>Unsupported file type: {{ fileExtension }}</p>
  </template> -->
</div>



            </div>

            <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 > 
              <div v-if="previewFileName">
                <a  class="justify-between border-main button success sm-4" :href="previewFileUrl"  :download="previewFileName">
                  <i class="fa fa-download fa-lg mr-2"></i> <span class="button-caption ml-2">Download</span>
                </a>
              </div>      
              
              <button  type="button" class="warning justify-between border-main mr-0" @click="isDocPreviewVisible = false" >
                <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
            </div>
          </form>
        </div>
      </sym-modal>
 

</section>  
</template>

<script>

import { DateTime } from "../../js/core";

import {
  get,
  upload, 
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
import SymInteger from '../../comp/SymInteger.vue';
import SymMemo from '../../comp/SymMemo.vue';

export default {
  components: { SymInteger, SymMemo },
  extends: PageMaintenance,
  name: 'ars0050',

  data () {
    return {
    
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
      requestDocTypeName: '',
       requestDocTypeId: 0,
      requestDocTypeFileName: "",
      
      requestDocTypeGUID: "",
      fileURL: "",
      lockId: '',
    },

    candidateIndex: -1,
      isAddingCandidate: false,
      isCandidateEditorVisible: false,
   
      isCandidateInProcessVisible: false,
      isCandidateHireVisible: false,


      newCandidate : {
      applicantlId: 0,
      memberRequestId: 0,
      lockId: '',
    },

    newCandidateIndex: -1,
      isAddingNewCandidate: false,
      isNewCandidateEditorVisible: false,

      isNewCandidateInProcessVisible: false,
      isNewCandidateHireVisible: false,

      request: {  
        memberRequestId: 0,
        memberRequestName: '',
        name3: '',
        clientContractId: 0,
        clientContractName: '',
        clientPosition: '',
        memberRequestPositionId: 0,
        memberRequestPositionName: '',
        jobCode: '',
        jobDescription: '',
        memberTypeId: '',
        memberTypeName:'',
        payoutTypeId: '',
        payoutTypeName: '',
        vacancyCount: '',
        deploymentDate: null,
        regionId: '',
        regionName: '',
        provinceId: '',
        provinceName: '', 
        municipalityId: '',
        municipalityName: '',      
        positionKeyword: '',
        remarks: '',
        skillNameKeyword: '',
        memberRequestStatusId: 0,
        educationLevelId: 0,
        age: 0,
        clientPayGroupId: 0,
        clientPayGroupName: '',
        workingDays: '',
        totalHired: 0,
        totalVacancy: 0,
        totalApplicant: 0,
        memberRequestRemarks:'',
        memberRequestPerks:'',
        barangayName: '',
        memberTypeId: 0, 
        lockId: ''
        
      },

      provinces: [],
      municipalities: [],
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
      activeTabIndex: 1,

      rateAmount: 0,
      educationLevelVisible: false,
      employmentTypeVisible: false,
      qualificationTypeVisible: false,
      medicalResultTypeVisible: false,
      nciiVisible: false,
      licenseVisible: false,
      complianceVisible: false,
      religionVisible: false,
      civilStatusVisible: false,
      sexVisible: false,

      memberName: '',

      filterParam: {
        filter:'',
        skillNameKeyword:'',
        positionKeyword:'',
      },
      recordIndex: -1,
      isAdding: false,

      logs: [],
      isLogVisible: false,

      payOutDailyTotalRate : 0,
      payOutMonthlyTotalRate : 0,
      billingDailyTotalRate : 0,
      billingMonthlyTotalRate : 0,
      isSubmiBtnDisable: true,

      docs: [],

      tradeTestDocType: [],
      applicantProgress: [],
      applicantScreeningProgress: [],

      doc: {
        docTypeDetailId: 0,
        memberId: 0,
        docTypeId: 0,
        docTypeName: "",
        docTypeReference: "",
        docTypeFileName: "",
        docTypeGUID: "",
        fileURL: "",
        lockId: "",
      },

      docIndex: -1,
      isAddingDoc: false,
      isDocEditorVisible: false,

      selectedFileList: [],
      documentNames: [],        
      docsUploadResult: null,
      catalogItems: [],

      fileName: "",
      isUploadRequired: false,
      pathFileName: "",
      guidReference: "",
      uploadPhotoTemp: false,
      pooling: [],
      currentPage: 1,
      totalPages: 0,
      memberCount: 0,
      rowPerPageId: 0,
      isProgressEditorVisible: false,

            
      isDocPreviewVisible: false,
      previewFileUrl: '', 
      previewFileName: '',

      fullImageFileNames: [],
      fullscreenElement: null,
      imageZoom: .75,
    };
  },

  computed: {

    isPDF() {
    const cleanUrl = (this.previewFileName || '').trim().toLowerCase();
    const result = cleanUrl.endsWith('.pdf') || /\.pdf(\w*)?$/.test(cleanUrl);
    return result;
  },

    editorDocPreviewTitle() {
      return this.previewFileName ;
    },
 
    isClosed () {
      return this.request.totalHired === this.request.vacancyCount;
    },

    isHired () {
      return this.candidateList.applicantStatusId === 3;
    },

    isCancelled () {
      return this.request.memberRequestStatusId === 3;
    },

    isActivated () {
      return this.request.memberRequestStatusId === 1;
    },

    isNotActive () {
      return this.request.memberRequestStatusId === 2;
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

    editorCandidateTitle () {
      if (this.isAddingCandidate) {
        return 'Add Candidate Detail';
      }
      return 'Candidate Progress';
    },

    editorDocTitle() {
      if (this.isAddingDoc) {
        return 'Add Doc Type Detail';
      }
      return 'Doc Type Detail';
    },

  },

  methods: {

    async fileExists(url) {
      try {
        const response = await fetch(url, { method: 'HEAD' });
        return response.ok;
      } catch (error) {
        this.advice.fault("Unsupported File Type.");
        return false;
      }
    },

    async openDocPreview(fileUrl, fileName) {
      const isSupported = /\.(pdf|jpg|jpeg|png)$/i.test(fileUrl);
      if (!isSupported) {
        this.advice.fault("Unsupported File Type.");
        return;
      }

    const exists = await this.fileExists(fileUrl);
    if (!exists) {
      this.advice.fault("File does not exists.");

      return;
    }

    this.previewFileUrl = fileUrl +  '#toolbar=0&navpanes=0&scrollbar=0';
    this.previewFileName = fileName;
    this.isDocPreviewVisible = true;    
  },

    closeDocPreview() {
      this.isDocPreviewVisible = false;
      this.previewFileUrl = '';
      this.previewFileName = '';
    },


    onHideProgressEditor () {
      const me = this;
      me.setActiveModel();
    },



    onShowProgressEditor () {
      const me = this;

      if (me.isCandidateInProcessVisible) {
        me.setRequiredMode(
        'applicantScreeningId',
        'screeningStatusId',
      );

      me.setOptionalMode(
        'hiredDate',
        'deploymentDate',
      );

    } else {
      me.setOptionalMode(
        'applicantScreeningId',
        'screeningStatusId',
      );

      me.setRequiredMode(
        'hiredDate',
        'deploymentDate',
      );


    }

      me.setDisplayMode(
        'memberId',
        'memberName',
      );

      setTimeout(() => {
        this.setFocus('applicantScreeningId');
      }, 200);
    },




  onShowProgress(dtl, index){
    this.applicantScreeningProgress = this.applicantProgress;
    this.applicantScreeningProgress = this.applicantScreeningProgress.filter(item => item.memberId === dtl.memberId);
      this.isProgressEditorVisible = true;
  },

    refreshTotal () {
      const me = this;
      
      me.payOutDailyTotalRate = 0;
      me.payOutMonthlyTotalRate = 0;
      me.billingDailyTotalRate = 0;
      me.billingMonthlyTotalRate = 0;

      me.payOuts.forEach( dtl => {

        me.payOutDailyTotalRate = me.payOutDailyTotalRate + dtl.dailyRate;
        me.payOutMonthlyTotalRate = me.payOutMonthlyTotalRate + dtl.monthlyRate;
      });

      me.billings.forEach( dtl => {

        me.billingDailyTotalRate = me.billingDailyTotalRate + dtl.dailyRate;
        me.billingMonthlyTotalRate = me.billingMonthlyTotalRate + dtl.monthlyRate;
      });

        
    },


    onClickMember(memberId) {
      const me = this;

      let route = {
        name: "hrs0010",
        query: {
          memberId: memberId,
          activeTabIndex: 6 
        },
      };
      me.go(route);
    },
    onClickApplicant(applicantId) {
      const me = this;

      let route = {
        name: "hrs0030",
        query: {
          applicantId: applicantId,        
          activeTabIndex: 1 
        },
      };
      me.go(route);
    },


 
    onHiredDateChanging (e) {
        e.message = 'Entry rejected.';
        e.callback = this.onHiredDateCallback;
        },

        onHiredDateCallback (e) {
        const me = this;
        if (e.proposedValue <= this.request.deploymentDate) {
          e.message = 'Hired Date cannot be less than deployment date <b>[' + this.request.deploymentDate + ']</b>.';
          me.isSubmiBtnDisable = false;
          return false;
          
        }
        else
         if (e.proposedValue >= this.request.deploymentDate) {
   
          me.isSubmiBtnDisable = true;
           return true;
        }
      },
 
    
    getTargetPath() {
      const me = this,
        q = {};

      if (me.request.memberRequestId) {
        q.memberRequestId = me.request.memberRequestId;
      }
        q.page = me.currentPage;

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
      if ("page" in q && me.core.isInteger(q.page)) {
        me.currentPage = parseInt(q.page);
      }
     },
    getRowColor (dtl) {
      if (dtl.applicantStatusId== 2) { return 'danger-light';}
      if (dtl.applicantStatusId== 3) { return 'info-light';}
      
    },

    onSubmitCandidate() {
      const me = this,
      
      d = me.candidate;
      let message = "";
      if (this.candidate.applicantScreeningId === 3 && this.candidate.screeningStatusId === 2) {
        me.isUploadRequired = true; 
      }
      
      if (!me.isValid("ars0050X")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

       if (me.isUploadRequired) {
              if (!me.fileName || me.fileName.trim() === "") {
                me.advice.fault('Upload required. Please select a file.', { duration: 5 });
                return;
              }
      }  

      me.isCandidateEditorVisible = false;
    
      if (me.isCandidateHireVisible) {
      message ='You are about to hire'
      } else {
        message ='You are about to update the screening status for'
      }

  
      me.dialog.confirm(message + ' <b>' + d.memberName + ' [' + d.memberId+ ']</b>. Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {

              me.isAdding = false;
              
              let dtl = {};
    
              dtl = me.candidateList[me.candidateIndex];
        
              dtl.applicantDetailId = d.applicantDetailId;
              dtl.memberId = d.memberId;
              dtl.memberRequestId = me.request.memberRequestId;
              dtl.applicantScreeningId = d.applicantScreeningId;
              dtl.screeningStatusId = d.screeningStatusId;
              dtl.applicantStatusId = d.applicantStatusId;
              dtl.hiredDate = null;
              dtl.deploymentDate = null;
              dtl.requestDocTypeId = d.requestDocTypeId;
              dtl.requestDocTypeFileName = me.fileName || d.requestDocTypeFileName;
              d.requestDocTypeFileName = dtl.requestDocTypeFileName;
              dtl.requestDocTypeGUID = me.guidReference || d.requestDocTypeGUID ;
              d.requestDocTypeGUID = dtl.requestDocTypeGUID;

              me.isCandidateEditorVisible = false;
          
              me.onSubmit();
              me.clearCandidatesPad();

            }
                return;
          }
        );
      },


      onApplicantScreeningIdChanged (newValue) {
      const me = this;

      let o = me.applicantScreenings.find( o => o.applicantScreeningId === newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
           
          me.isUploadRequired = true
   
        }
        else {
          me.isUploadRequired = false

        }

      }
    },
      
    onRegionIdChanged() {
      const me = this,
        wait = me.wait();

        let o = me.region.find( o => o.regionId == me.request.regionId);
     
        if (o) {
        me.provinces = [];
        me.municipalities = [];
        }

      me.getProvinces().then(
        (data) => {
          me.stopWait(wait);
          me.provinces = data;

          if (me.provinces.length) {
            me.provinces.unshift({
              provinceId: 0,
              provinceName: "--- Select Province ---",
            });

            me.setFocus("provinceId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
      
    },

    onProvinceIdChanged() {
      const me = this,
        wait = me.wait();
       let o = me.provinces.find( o => o.provinceId == me.request.provinceId);
       
       if(o){
        me.municipalities = [];
       }  
      

      me.getMunicipalities().then(
        (data) => {
          me.stopWait(wait);
          me.municipalities = data;

          if (me.municipalities.length) {
            me.municipalities.unshift({
              municipalityId: 0,
              municipalityName: "--- Select Municipality ---",
            });
            me.setFocus("municipalityId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },



    onHideCandidateEditor () {
      const me = this;
      me.setActiveModel();
    },


    onShowCandidateEditor () {
      const me = this;

      me.setActiveModel('candidate');

      if (me.isCandidateInProcessVisible) {
        me.setRequiredMode(
        'applicantScreeningId',
        'screeningStatusId',
      );

      me.setOptionalMode(
        'hiredDate',
        'deploymentDate',
      );

    } else {
      me.setOptionalMode(
        'applicantScreeningId',
        'screeningStatusId',
      );

      me.setRequiredMode(
        'hiredDate',
        'deploymentDate',
      );


    }

      me.setDisplayMode(
        'memberId',
        'memberName',
      );

      setTimeout(() => {
        this.setFocus('applicantScreeningId');
      }, 200);
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


    onRefresh () {
      this.loadData();
    },
   

  toggleEducationLevelVisibility() {
    this.educationLevelVisible = !this.educationLevelVisible;
  },

  toggleEmploymentTypeVisibility() {
    this.employmentTypeVisible = !this.employmentTypeVisible;
  },

  toggleQualificationTypeVisibility() {
    this.qualificationTypeVisible = !this.qualificationTypeVisible;
  },

  toggleMedicalResultTypeVisibility() {
    this.medicalResultTypeVisible = !this.medicalResultTypeVisible;
  },

  toggleNCIIVisibility() {
    this.nciiVisible = !this.nciiVisible;
  },

  toggleLicenseVisibility() {
    this.licenseVisible = !this.licenseVisible;
  },

  toggleComplianceVisibility() {
    this.complianceVisible = !this.complianceVisible;
  },

  toggleReligionVisibility() {
    this.religionVisible = !this.religionVisible;
  },

  toggleCivilStatusVisibility() {
    this.civilStatusVisible = !this.civilStatusVisible;
  },

  toggleSexVisibility() {
    this.sexVisible = !this.sexVisible;
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


    onActiveTabIndexChanged () {
      this.reload();
    },
 
    onHireMember(dtl, index) {
      
        const d = this.candidate;
        this.candidateIndex = index;
         this.isSubmiBtnDisable = true;
        this.isCandidateInProcessVisible = false,
        this.isCandidateHireVisible = true,
        this.isUploadRequired = false,

        dtl = this.core.convertDates(dtl);

        if (dtl.docTypeCount > 0) {
            this.advice.fault('<b>' + dtl.docTypeName +'</b> is required.', { duration: 5 })
            return;
        }  


        if (dtl.deploymentDate === null) {
          dtl.deploymentDate = this.core.emptyDateTime();
          dtl.deploymentDate  = this.request.deploymentDate;
        }

        if (dtl.hiredDate === null) {

        dtl.hiredDate = this.core.emptyDateTime();
        dtl.hiredDate = this.request.deploymentDate; 
        dtl.remarks  = '';
        }
        d.applicantDetailId = dtl.applicantDetailId;
        d.memberId = dtl.memberId;
        d.applicantScreeningId = dtl.applicantScreeningId;
        d.applicantStatusId = 3; 'Hired'
        d.screeningStatusId = dtl.screeningStatusId;
        d.remarks = dtl.remarks;
        d.memberName = dtl.memberName;      
        d.hiredDate = dtl.hiredDate;
        d.deploymentDate = dtl.deploymentDate;
        d.lockId = dtl.lockId;

        this.isCandidateEditorVisible = true;

      },


      onCreateMemberRecord(dtl, index) {
        const d = this.newCandidate;
        this.newCandidateIndex = index;
        d.applicantId = dtl.applicantId;
        d.memberRequestId = dtl.memberRequestId;
       
        let route = {
        name: "hrs0030",
        query: {
          applicantId: d.applicantId,
          memberRequestId: d.memberRequestId,
          memberTypeId: this.request.memberTypeId

        },
        };

    
      this.go(route);
      },


      onProcessMember(dtl, index) {
        const d = this.candidate;
        this.candidateIndex = index;

        this.isCandidateInProcessVisible = true;
        this.isCandidateHireVisible = false;
        this.fileName = '';
        this.isUploadRequired = false

        dtl = this.core.convertDates(dtl);
        this.fileName = dtl.requestDocTypeFileName;
        
        if (dtl.hiredDate === null) {
          dtl.hiredDate = this.core.emptyDateTime();
        }

        if (dtl.deploymentDate === null) {
          dtl.deploymentDate = this.core.emptyDateTime();
        }

        d.applicantDetailId = dtl.applicantDetailId;
        d.memberId = dtl.memberId;
        d.requestDocTypeId = dtl.requestDocTypeId
        if (dtl.screeningStatusId==2) {
          d.applicantScreeningId = '';
          d.screeningStatusId = 1;
          d.requestDocTypeGUID = '';
          d.requestDocTypeFileName = '';
          d.requestDocTypeId = '';
          this.fileName = '';
          dtl.remarks = '';
        }
          else {
            d.applicantScreeningId = dtl.applicantScreeningId;
            d.screeningStatusId = dtl.screeningStatusId;
            d.remarks = dtl.remarks;
 
        } 
        this.getApplicantScreening(d.applicantDetailId).then(
        (data) => {
          this.applicantScreenings = '';
          this.applicantScreenings = [...data];
          this.refresh();  
          },
          (fault) => {
            this.showFault(fault);
          }
      )

      d.applicantStatusId = dtl.applicantStatusId;
        d.remarks = dtl.remarks;
        d.memberName = dtl.memberName;      
        d.hiredDate = dtl.hiredDate;
        d.deploymentDate = dtl.deploymentDate;
        d.lockId = dtl.lockId;

        this.isCandidateEditorVisible = true;
      },


    onAddMember(dtl, index) {
      const me = this;
      
      const isMemberExists = this.candidateList.some(member => member.memberId === dtl.memberId);
      
      if (isMemberExists) {
      this.advice.warning('This member is already in the candidate list', { duration: 5 });
      return;
    }  


    me.dialog.confirm('Add <b>' + dtl.memberName + ' [' + dtl.memberId+ '] </b>' +  'in candidate list to process for hiring? Please note that no movement for 30 days,member will up for grabs for other recruiters. ' , { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
        if (reply === 'yes') {

          me.isAdding = true;

  
          dtl.applicantDetailId = 0;
          dtl.memberRequestId = this.request.memberRequestId;
          dtl.applicantScreeningId = 0 ;
          dtl.applicantScreeningName = "XX";
          dtl.applicantStatusId = 1;
          dtl.screeningStatusId = 1;
          dtl.applicantStatusName = "1";
          dtl.remarks = '';
          dtl.hiredDate = null;
          dtl.deploymentDate = null;
          dtl.lockId = '';

          me.candidate.applicantDetailId = -1;
          me.candidate.memberId = dtl.memberId;
          me.candidate.memberRequestId = dtl.memberRequestId;
          me.candidate.applicantScreeningId = 0;
          me.candidate.screeningStatusId = 1;
          me.candidate.applicantStatusId = 1;
          me.candidate.remarks = '';
          me.candidate.hiredDate = null;
          me.candidate.deploymentDate = dtl.null;
          me.candidate.lockId = '';

          dtl.memberRequestId = this.request.memberRequestId;
          dtl.applicantScreeningId = 0 ;
          dtl.applicantScreeningName = "XX";
          dtl.applicantStatusId = 1;
          dtl.screeningStatusId = 1;
          dtl.applicantStatusName = "1";
          dtl.remarks = "";
          dtl.hiredDate = null;
          dtl.deploymentDate = null;
          dtl.lockId = '';
          dtl.docTypeName = '';
          dtl.docTypeId = 0,
          dtl.docTypeReference = "";
          dtl.docTypeFileName = "";
          dtl.docTypeGUID = "";
          dtl.fileURL = "";  
          this.candidateList.push(dtl);


        if (me.isAdding) {

        me.onSubmit();
      } else {
      dtl = me.candidateList[me.recordIndex];

      dtl.applicantDetailId = d.applicantDetailId;
      dtl.memberId = d.memberId;
      me.onSubmit();
    }


      this.memberList.splice(index, 1);
      this.advice.success(`${dtl.memberName} has been added to the candidate list.`, { duration: 5 });


          }
          return;
        }
      );

      },



      onRemoveMember(index) { 
      this.memberList.splice(index, 1);
    },


    onRemoveCandidate() {
      const me = this,
      d = me.candidate;
      d.applicantStatusId = 2
      let message ="";
      message ='You are about to fail '
      me.dialog.confirm(message + ' <b>' + d.memberName + ' [' + d.memberId+ ']</b>. Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {

            me.isAdding = false;

            let dtl = {};

        dtl = me.candidateList[me.candidateIndex];
  
        dtl.applicantDetailId = d.applicantDetailId;
        dtl.memberId = d.memberId;
        dtl.memberRequestId = me.request.memberRequestId;
        dtl.applicantScreeningId = d.applicantScreeningId;
        dtl.screeningStatusId = d.screeningStatusId;
        dtl.applicantStatusId = d.applicantStatusId;
        dtl.hiredDate = null;
        dtl.deploymentDate = null;
        dtl.name3 = this.request.name3;

        me.isCandidateEditorVisible = false;
        me.onSubmit();

          }
          return;
        }
      );

      },



    onResetFilter() {
      const me = this;
      me.educations = [];
      me.typeQualifications = [];
      me.medicalResults = [];
      me.religions = [];
      me.civilStatus = [];
      me.sexes = [];
      me.nciis = [];
      me.compliances = [];
      me.licenses = [];
      me.memberList= [];
      me.request.regionId = '';
      me.request.provinceId = '';
      me.request.municipalityId = '';
      me.memberName='';
      me.request.positionKeyword='';
      me.request.skillNameKeyword='';
      me.request.age=0;
      me.request.educationLevelId='';
    },


    onPooling() {
      const me = this;

      let filter = 'MemberName is not NULL AND MemberStatusId = 1  ';
  
     

      let ageFilter = '';

      if (me.request.age) {
        ageFilter = ' AND Age <=' + me.request.age
      }

      let educationLevelFilter = '';

      if (me.request.educationLevelId) {
        educationLevelFilter = ' AND EducationLevelSortSeq <=' + me.request.educationLevelId
      }

      let regionFilter = '';

      if (me.request.regionId) {
        regionFilter = ' AND regionId =' + me.request.regionId
      }

      let provinceFilter = '';

      if (me.request.provinceId) {
        provinceFilter = ' AND provinceId =' + me.request.provinceId
      }

      let municipalitiesFilter = '';

      if (me.request.municipalityId) {
        municipalitiesFilter = ' AND municipalityId =' + me.request.municipalityId
      }
      
      let skillNameKeywordFilter = '0';

      if (me.request.skillNameKeyword) {
        skillNameKeywordFilter = me.request.skillNameKeyword
      }

      let positionKeywordFilter = '0';

      if (me.request.positionKeyword) {
        positionKeywordFilter = me.request.positionKeyword
      }
        
      let qualificationTypeFilter = '';
      if (me.typeQualifications && me.typeQualifications.length > 0) {
      
        me.typeQualifications.forEach((typeQualification) => {
          qualificationTypeFilter =  typeQualification.typeQualificationDetailId + "," + qualificationTypeFilter
        });
        
        qualificationTypeFilter = ' AND TypeQualificationDetailId in(' + qualificationTypeFilter.slice(0, -1) + ')'
      }

      let medicalResultTypeFilter = '';
      if (me.medicalResults && me.medicalResults.length > 0) {
      
        me.medicalResults.forEach((medicalResult) => {
          medicalResultTypeFilter =  medicalResult.medicalResultTypeId + "," + medicalResultTypeFilter
        });
        
        medicalResultTypeFilter = ' AND MedicalResultTypeId in(' + medicalResultTypeFilter.slice(0, -1) + ')'
      }

      let religionFilter = '';
      if (me.religions && me.religions.length > 0) {
      
        me.religions.forEach((religion) => {
          religionFilter =  religion.religionId + "," + religionFilter
        });
        
        religionFilter = ' AND ReligionId in(' + religionFilter.slice(0, -1) + ')'
      }

      let civilStatusFilter = '';
      if (me.civilStatus && me.civilStatus.length > 0) {
      
        me.civilStatus.forEach((civilStatus) => {
          civilStatusFilter =  civilStatus.civilStatusId + "," + civilStatusFilter
        });
        
        civilStatusFilter = ' AND CivilStatusId in(' + civilStatusFilter.slice(0, -1) + ')'
      }

      let sexFilter = '';
      if (me.sexes && me.sexes.length > 0) {
      
        me.sexes.forEach((sex) => {
          sexFilter =  "'" + sex.sexId + "'," + sexFilter
        });
        
        sexFilter = ' AND SexId in(' + sexFilter.slice(0, -1) + ')'
      }
 
      let nciiFilter = '';
      if (me.nciis && me.nciis.length > 0) {
      
        me.nciis.forEach((ncii) => {
          nciiFilter =  ncii.nciiQualificationTitleId + "," + nciiFilter
        });
        
        nciiFilter = ' AND NCIIQualificationTitleId in(' + nciiFilter.slice(0, -1) + ')'
      }
 
      let complianceFilter = '';
      if (me.compliances && me.compliances.length > 0) {
      
        me.compliances.forEach((compliance) => {
          complianceFilter =  compliance.complianceTrainingId + "," + complianceFilter
        });
        
        complianceFilter = ' AND ComplianceTrainingId in(' + complianceFilter.slice(0, -1) + ')'
      }

      let licenseFilter = '';
      if (me.licenses && me.licenses.length > 0) {
      
        me.licenses.forEach((license) => {
          licenseFilter =  license.licenseProfessionId + "," + licenseFilter
        });
        
        licenseFilter = ' AND LicenseProfessionId in(' + licenseFilter.slice(0, -1) + ')'
      }

      filter = filter + ageFilter + educationLevelFilter + qualificationTypeFilter  + medicalResultTypeFilter + religionFilter + civilStatusFilter + sexFilter + nciiFilter + complianceFilter + licenseFilter + regionFilter + provinceFilter + municipalitiesFilter
      let filterName = '';
      if (me.memberName) {
        filterName = " AND MemberName like '%" + me.memberName + "%'"
      }

      me.filterParam = {}; 
      me.filterParam.filter = filter + filterName;
      me.filterParam.positionKeyword = positionKeywordFilter;
      me.filterParam.skillNameKeyword = skillNameKeywordFilter;
      me.getMembers().then(
        (data) => {
          
          me.memberList = [];
          me.memberList = data;
           
          if (!me.memberList.length > 0) {
            me.advice.fault('No record found. Clear the filter and try again.', { duration: 5 })
            return;
          }  
        },
        (fault) => {
          me.showFault(fault);
        }
      );

      me.currentPage = 1;  
      me.memberCount = 0;
    },

  

    onClickPooling() {
      let route = {
        name: "hrs0020",
        query: {
          memberRequestId: this.request.memberRequestId,
        },
      };
      this.go(route);
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

    clearCandidatesPad () {
      const d = this.candidate;

      d.requestDocTypeId = 0;
      d.requestDocTypeFileName = "";
      d.requestDocTypeGUID = "";
      d.fileURL = "";
    
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

    getRegionName() {
      const me = this,
        wait = me.wait();
        
        let o = me.region.find( o => o.regionId == me.request.regionId);
       
        if (o) {

          if (o.rateAmount) {
            me.rateAmount = o.rateAmount;
          } 
 
      }
        
      me.provinces = [];
      me.municipalities = [];

      me.getProvinces().then(
        (data) => {
          me.stopWait(wait);
          me.provinces = data;

          if (me.provinces.length) {
            me.provinces.unshift({
              provinceId: 0,
              provinceName: "--- Select Province ---",
            });

            me.setFocus("provinceId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onProvinceIdChanged() {
      const me = this,
        wait = me.wait();

      me.municipalities = [];

      me.getMunicipalities().then(
        (data) => {
          me.stopWait(wait);
          me.municipalities = data;

          if (me.municipalities.length) {
            me.municipalities.unshift({
              municipalityId: 0,
              municipalityName: "--- Select Municipality ---",
            });
            me.setFocus("municipalityId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
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

    onMemberRequestPositionIdChanging (e) {
      e.message = "Member Request Position ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.memberRequestPositionIdCallback;
    },

    memberRequestPositionIdCallback (e) {
      const me = this;
      let filter = "MemberRequestPositionId='" + e.proposedValue + "'";
      return getList('dbo.ArsMemberRequestPosition', 'MemberRequestPositionId, MemberRequestPositionName', '', filter).then(
        position => {
          if (position && position.length) {
            me.request.memberRequestPositionName = position[0].memberRequestPositionName;
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
      
      this.focusNext();

    },

    onClientContractIdChanging (e) {
      e.message = "Client Contract ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientContractIdCallback;
    },

    clientContractIdCallback (e) {
      const me = this;
      let filter = "ClientContractId='" + e.proposedValue + "'";
      return getList('dbo.ArsClientContract', 'ClientContractId, ClientContractName', '', filter).then(
        contract => {
          if (contract && contract.length) {
            me.request.clientContractName = contract[0].clientContractName;
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

    onClientContractIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.request;

      item.clientContractId = data.clientContractId;
      item.clientContractName = data.clientContractName;
      
      this.focusNext();

    },


    onClientPayGroupIdChanging (e) {
      e.message = "Client Pay Group ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      const me = this;
      let filter = "ClientPayGroupId='" + e.proposedValue + "'";
      return getList('dbo.ArsClientPayGroup', 'ClientPayGroupId, ClientPayGroupName', '', filter).then(
        payGroup => {
          if (payGroup && payGroup.length) {
            me.request.clientPayGroupName = payGroup[0].clientPayGroupName;
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
      
      this.focusNext();

    },


    loadData () {
      const
        me = this,
        wait = me.wait();
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
            me.screeningStatus = data.screeningStatus; 
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
      
      me.docs = info.docs;  
      me.provinces = info.province;
      me.municipalities = info.municipality;
      me.typeQualificationList = info.memberTypeQualifications;
      me.medicalResults = info.medicalResults;
      me.billings = info.billings;
      me.payOuts = info.payOuts;
      me.screenings = info.screenings;
      me.updateTypeQualificationList = true;

      me.candidateList = info.applicants ;
      
      me.newCandidateList = info.newApplicants;
      me.docType = info.docType;
      me.tradeTestDocType = info.tradeTestDocType;

      me.applicantProgress = info.applicantProgress;
      
      me.payOuts.forEach((payOut) => {    
        payOut.monthlyRate = Math.round((payOut.dailyRate * me.request.workingDays / 12) * 100) / 100;
      });

      me.billings.forEach((billing) => {    
        billing.monthlyRate = Math.round((billing.dailyRate * me.request.workingDays / 12) * 100) / 100;
      });

      me.provinces = info.province;
      me.municipalities = info.municipality;
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
      me.oldCandidateList = JSON.stringify(me.candidateList);
      me.oldNewCandidateList = JSON.stringify(me.newCandidateList);
    },

    // API calls

    onPageChange(page) {
      this.currentPage = page;
      this.reload();
    },

    getMemberRequestApplicantDocType(memberId) {
      return get("api/member-request-applicant-docs/" + memberId );

    },



    getApplicantScreening(applicantDetailId) {
      return get("api/member-request-new-applicant-screenings/" + applicantDetailId + "/" + this.request.memberRequestId);

    },


    getApiPayloadFilter () {
      const
        me = this,
        filterParam = {};

      Object.assign(filterParam, me.filterParam);
      return filterParam;
    },


    getMembers () {
      const
        payload = this.getApiPayloadFilter(),
        body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/request-pooling-members', options);
    },


    getMemberTypeQualifications() {
      const me = this;
      return get("api/request-type-qualifications/" + me.request.memberTypeId);
    },
 
    getProvinces() {
      return get("api/request-pooling-provinces/" + this.request.regionId);
    },

    getMunicipalities() {
      const me = this; 
      return get("api/request-pooling-municipalities/" + me.request.regionId + "/" + me.request.provinceId);
    },

    getRequest () {
      return get('api/member-request-pooling/' + this.request.memberRequestId);
    },

    getReferences () {
      const me = this;

      if (me.type.length) {
        return Promise.resolve(true);
      }
      return get('api/references/ars0050');
    },


    getChangeLog() {
      const
        detail = this.candidate;

      return get("api/member-request-new-applicants/" + detail.applicantDetailId + "/log");
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);
        return ajax('api/member-request-new-applicants/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);
      return ajax('api/member-request-applicants/' + this.candidate.applicantDetailId  + '/' + currentUserId, options);
    },


    createRecordDocType () {
      const
        payload = this.getApiPayloadDocType(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);
        return ajax('api/member-request-applicant-docs/' + currentUserId + '/' + this.request.memberRequestId, options);
    },

    modifyRecordDocType () {
      const
        payload = this.getApiPayloadDocType(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);
  
      return ajax('api/member-request-applicant-docs/' + this.candidate.memberId  + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        detail = this.candidate,
        options = this.core.getAjaxOptions('DELETE');
      return ajax('api/member-request-applicants/' + detail.applicantDetailId + this.getDeleteQuery(detail.lockId), options);
    },


    getApiPayloadDocType () {
      const
        me = this,
        detail = {};
      me.docs.memberId = me.docs[0].memberId
        me.docs.docTypeDetailId = me.docs[0].docTypeDetailId

        me.docs.docTypeFileName = me.docs[0].docTypeFileName
        me.docs.docTypeGUID = me.docs[0].docTypeGUID
        me.docs.docTypeId = me.docs[0].docTypeId
        me.docs.docTypeReference = me.docs[0].docTypeReference
        me.docs.fileURL = me.docs[0].fileURL
        me.docs.lockId = me.docs[0].lockId
        
        Object.assign(detail, me.docs);
     
      return detail;
    },


    getApiPayload () {
      const
        me = this,
        detail = {};
     
        me.candidate.memberRequestId = me.request.memberRequestId;
        me.candidate.name3 = me.request.name3; 
        Object.assign(detail, me.candidate);
      
      return detail;
    },
    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('request', 'educations', 'religions', 'civilStatus', 'sexes', 'employmentTypes', 'typeQualifications', 'nciis', 'ncii', 'licenses', 'license','cdas','cda','compliances','compliance', 'medicalResults', 'billings', 'billing', 'payOuts', 'payOut','screenings','candidateList', 'newCandidateList');
      dc.keyField = 'memberRequestId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.isCancelling = false;
      this.memberList = [];
      this.candidateList = [];
      this.newCandidateList = [];

      this.refreshOldRefs();
      setTimeout(() => {
        this.disableElement(
          'btn-add'
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

    onMemberRequestIdSearchFill(e) {
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

    

    onDocTypeIdChanging(e) {
      e.message = 'Entry rejected.';
      e.callback = this.docTypeIdCallback.bind(this); 
    },

    onRequestDocTypeIdChanging(e) {
      e.message = 'Entry rejected.';
      e.callback = this.requestDocTypeIdCallback.bind(this); 
    },

    requestDocTypeIdCallback(e) {
      
      const me = this;

      if (!Array.isArray(me.docs)) {
        console.warn('docs is undefined or not an array');
        return true;
      }

      const index = me.docs.findIndex(obj => obj.requestDocTypeId === e.proposedValue);
      
      if (index > -1) {
        const requestDocTypeName = me.docs[index].docTypeName;
        e.message = 'Doc Type <b>' + requestDocTypeName + '</b> is already in the list.';
        return false;
      }

      return true;
    },

    onRequestDocTypeIdChanged(newValue) {
      const me = this;
     

      let selectedDocType = me.docType.find(o => o.docTypeId === newValue);

      if (selectedDocType) {
          if (selectedDocType.uploadRequiredFlag) {
            this.isUploadRequired = true
    
          }

        }else {
          me.isUploadRequired = false; 
      }
     },

    onDocTypeReferenceChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.docTypeReferenceCallback;
    },

    docTypeReferenceCallback (e) {
      const me = this;
      let docTypeReference = e.proposedValue;
      let filter = "docTypeReference='" + docTypeReference + "'";
      return getCount('dbo.HrsMemberDocType', filter).then(
        count => {
          if (count > 0) {
          e.message = '<b>' + docTypeReference + '</b> already exists.';
          return false;
        } else {
            let index = me.docs.findIndex(obj => obj.docTypeReference === docTypeReference);

            if (index > -1) {
              e.message = 'Reference <b>' + docTypeReference + '</b> is already in the list.'
              return false;
            }

      return true;
        }
        },
        () => {
          return true;
        }
      );


    },

    onDocTypeIdChanged(newValue) {
      const me = this;
 
      let selectedDocType = me.docType.find(o => o.docTypeId === newValue);
        if (selectedDocType) {
 
            me.doc.docTypeLength = (selectedDocType.docTypeLength !== undefined && selectedDocType.docTypeLength !== null) 
                ? selectedDocType.docTypeLength 
                : 12;

           
            if (me.doc.docTypeLength === null) {
                me.doc.docTypeLength = 12;
            }



            me.isUploadRequired = selectedDocType.uploadRequiredFlag;

            me.doc.docTypeReference = ''; 
            this.validateInput(); 
    
        } else {
   
            me.isUploadRequired = false; 
        }
    },

    validateInput() {

      const regex = /^[0-9]*$/;


      if (!regex.test(this.doc.docTypeReference)) {
        
        this.doc.docTypeReference = this.doc.docTypeReference.slice(0, -1);
      }


      if (this.doc.docTypeReference.length > this.doc.docTypeLength) {
        this.doc.docTypeReference = this.doc.docTypeReference.slice(0, this.doc.docTypeLength); // Trim to max length if exceeded
      }
    },

    onSelectDocuments() {
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

    onSelectedChanged (fileList) {

      this.selectedFileList = fileList;

      if (fileList.length) {
        const uploader = this.$refs.uploader;
        if (uploader) {
          uploader.invokeUpload();
        }
      }

    },
    
    onSelectFile () {
    const uploader = this.$refs.uploader;
    if (uploader) {
      uploader.invokeClick();
    }
    },

    onAddDoc(dtl,index) {  
      const me = this;
      me.clearDocPad();
        const d = this.candidate;
        this.candidateIndex = index;


        d.memberId = dtl.memberId;



      this.getMemberRequestApplicantDocType(dtl.memberId).then(
        (data) => {
          me.docs = data.member;
          me.docType = data.docType;
          me.refresh();
        },
        (fault) => {
          me.showFault(fault);
        }
      );

      me.isDocEditorVisible = true;
      me.isAddingDoc = true;
    },


    onSubmitDoc() {
  
      const me = this,
        d = me.doc;
      me.isUploadRequired = true;

      if (!d.docTypeId) {
        me.isDocEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
          me.advice.fault('Upload required. Please select a file.', { duration: 5 });
          return;
        }
      }

      if (!me.isValid("hrs0010D")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 });
        return;
      }

      if (this.doc.docTypeReference.length < this.doc.docTypeLength) {
        this.advice.fault('Reference length is not valid.', { duration: 5 });
        return;
      }


      let docTypeReference = d.docTypeReference;
      let filter = "docTypeReference='" + docTypeReference + "' AND docTypeId='" + d.docTypeId +"'";

      getCount('dbo.HrsMemberDocType', filter).then(count => {
        if (count > 0) {
          this.advice.fault( '<b>' + docTypeReference + '</b> already exists.');
          return;
        }
       
        let dtl = {};

        if (me.isAddingDoc) {
          Object.assign(dtl, d);
         
          me.docs.push(dtl);

          me.docType.forEach((docType) => {
            if (docType.docTypeId == dtl.docTypeId) {
              dtl.docTypeName = docType.docTypeName;
            }
          });
          dtl.memberId = this.candidate.memberId;

          dtl.docTypeFileName = me.fileName;
          dtl.docTypeGUID = me.guidReference;
          
          me.clearDocPad();
          me.advice.info("Doc Type Name '" + dtl.docTypeName + "' added to list.", { duration: 5 });
          me.setFocus("DocTypeId");
          me.onSubmitDocType();
          me.isDocEditorVisible = false;

        } else {
          dtl = me.docs[me.docIndex];

          me.docType.forEach((doc) => {
            if (doc.docTypeId == d.docTypeId) {
              dtl.docTypeName = doc.docTypeName;
            }
          });

          dtl.docTypeDetailId = d.docTypeDetailId;
          dtl.docTypeId = d.docTypeId;
          dtl.docTypeReference = d.docTypeReference;
          dtl.docTypeFileName = me.fileName || d.docTypeFileName;
          dtl.docTypeGUID = me.guidReference || d.docTypeGUID;

          me.isDocEditorVisible = false;
        }
      });

            
    },

    clearDocPad() {
      const d = this.doc;

      d.docTypeDetailId = 0;
      d.docTypeId = 0;
      d.docTypeReference = "";
      d.docTypeFileName = "";
      d.docTypeGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },


    onShowDocEditor() {
      const me = this;

      me.setActiveModel("doc");

      me.setRequiredMode(
        'docTypeId',
        'docTypeReference'
      );

      setTimeout(() => {
        this.setFocus("docTypeId");
      }, 200);
    },

    onHideDocEditor() {
      const me = this;

      me.isAddingDoc = false;
      me.setActiveModel();
    },

    onEditDoc(dtl, index) {
      const d = this.doc;
      this.docIndex = index;
       this.docType.forEach((doctype) => {
          if (doctype.docTypeId == dtl.docTypeId) {
            dtl.docTypeLength = doctype.docTypeLength;
          }
        });
      d.docTypeDetailId = dtl.docTypeDetailId;
      d.docTypeId = dtl.docTypeId;
      d.docTypeLength = dtl.docTypeLength;
      d.docTypeReference = dtl.docTypeReference;
      d.docTypeFileName = dtl.docTypeFileName;
      d.docTypeGUID = dtl.docTypeGUID;
      this.fileName = dtl.docTypeFileName

      this.isDocEditorVisible = true;
      me.clearDocPad();
    },

    loadFile (fileName, GUID) {
      const
        me = this;
        me.getFileName(fileName, GUID).then(
        info => {
          me.pathFileName = info

          return;
        }
      );

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

    getFileName (fileName, GUID) {
      return get('api/member/download-file/' + this.candidate.memberId + '/' + fileName + '/' + GUID);
    },

    guid () {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
      (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
    },

    // API calls
    uploadDocuments (files) {
      let q = this.guid();

      return upload('api/member/' + this.candidate.memberId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
    },
    
    
    onSubmitDocType (nextRoute) {
      const me = this;
     
      let
        promise,
        message,
        wait = me.wait();

      if (me.isAddingDoc) {
        promise = me.createRecordDocType();
        
      } else {
        promise = me.modifyRecordDocType();
      }

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (typeof success === 'number' && success > 0) {
              me.request.memberRequestId = success; 
            }

            if (!me.isAdding) {
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
          this.loadData()
          // me.onReset();
        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

    },



    onSubmit (nextRoute) {
      const me = this;
      let
        promise,
        message,
        wait = me.wait();

      if (me.isAdding) {
        promise = me.createRecord();
        
      } else {
        promise = me.modifyRecord();
      }

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (typeof success === 'number' && success > 0) {
              me.request.memberRequestId = success; 
            }

            if (!me.isAdding) {

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
          this.loadData()
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
        );

        me.setDisplayMode(
          'memberRequestName',
          'clientContractId',
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
          'memberRequestPositionName',
          'clientPayGroupId',
          'clientPayGroupName',
          'workingDays',
        );

        me.setFocus('memberRequestName');
      }, 100);

    },


    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();
        me.setDisplayMode(
          'clientContractName',
          'memberRequestPositionName',
          'clientPayGroupName',
          'memberRequestName',
          'clientContractId',
          'clientPosition',
          'memberRequestPositionId',
          'jobCode',
          'jobDescription',
          'memberTypeId',
          'payoutTypeId',
          'vacancyCount',
          'deploymentDate',
          'clientPayGroupId',
          'workingDays',
          'memberRequestRemarks',
          'memberRequestPerks'
        );

        me.setFocus('memberRequestName');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }
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
    me.newCandidateList = [];
    me.payTrx = [];   
    me.applicantScreenings = []; 
    me.screeningStatus = []; 
    me.billings = [];
    me.payOuts = [];
    me.today = me.sym.dateInfo.serverDate;
    me.docType = [];
  },

  mounted () {
      const me = this;
      me.currentPage = 1;
  },

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
  grid-template-columns: 1fr 1fr 1fr;
  gap: 1rem;
  width: 100%;

}
.sub-container-2{
  display: grid;
  grid-template-rows: .5fr;
  gap: .5rem;

}
.box-1{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.box-3{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
  gap: .5rem;
}
.sub-container-3{
  display: grid;
  grid-template-rows: 1fr 1fr ;
}
.container-1{
  display: flex;
  flex-direction: column;
}
.container-2{
  display: flex;
  flex-direction: column;
   gap: .5rem;
}
.container-3{
  display: flex;
    flex-direction: column;
    gap: .5rem;
}
.container-4{
  display: flex;
    flex-direction: column;
    gap: .5rem;
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
  overflow-x: hidden;
  max-height: 50vh;
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
 justify-content:center;
 align-items: center;
}
.others-btns{
display: flex;
 justify-content: center;
}
.btnsss{
  display: flex;
}
.select-btns{
width: 100%;

display: grid;
grid-template-columns: .4fr .4fr 3fr;

}
.applicant-btn{
display: flex;
justify-content: space-between;
width: 100%;
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

.internal-pos-field{
  display: grid;
  grid-template-columns: 3fr 5fr;
  gap: .5rem;
  width: 100%;
}
.payout-field{
  display: grid;
  grid-template-columns: 3fr 3fr 2fr;
  gap: .5rem;
  width: 100%;
}


.Header {
  width: 100%;
  border: 0;
  padding: 5px;
  text-align: center;
  font-weight: bold;
  color: white;
  
  text-transform: uppercase;
}
.box-fields{
  display: flex;
  flex-direction: column;
  gap: .5rem;
}
.box-info-main{
display: flex ;
flex-direction: column;
gap: .5rem;
}
.box-info{
display: flex ;
flex-direction: column;
gap: .5rem;
}


.box-container{
  display: grid;
  grid-template-columns: 1fr 15fr ;
  gap: .5rem;
}
.box-column{
  display: grid;
  grid-template-columns: 2fr 2fr 2fr;
  gap: 1rem;
  margin-bottom: .5rem;
}
.box-row{
  display: grid;
  grid-template-columns: 1.2fr .3fr 1.3fr 1.2fr 1.2fr ;
  gap: .5rem;
}

.box-remarks{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr ;
  gap: .5rem;
}
.box-main{
  margin-top: 10px;
}
.plus-btn{
  padding: 0;
}
.btn{
  height:30px;
 
}
.location-fields{
  display: grid;
  grid-template-columns: 1fr 1fr  1fr;
  gap: .5rem;
}
.location-two{
  display: grid;
  grid-template-columns: 2fr 1fr ;
  gap: .5rem;
}

sym-memo >>> 

.candidate-id{
  width: 5%;
}
.candidate-name{
  width: 15%;
}
.candidate-birth{
  width: 5%;
}
.candidate-age{
  width: 3%;
}
.candidate-sex{
  width: 3%;
}
.candidate-mobile{
  width:6%;
}
.candidate-status{
  width:5%;
}
.candidate-religion{
  width:6%;
}
.candidate-civil{
  width:6%;
}
.candidate-member-stat{
  width:5%;
}
.candidate-action{

  
  width: 2%;
}
.candidate-others{

  
  width: 4%;
}
.religion-scroller{
  max-height: 20vh;
  overflow: auto;
}
@media(max-width: 530px){
  .action-buttons {
  display: flex;
  flex-direction: row;
 justify-content: center;

}
  
.fixed-header {

  width: 200vw;

}
.table-scroller{
 
    overflow: auto;
}
.id{
  width: 10%;
}
.name{
    width: 60%;
}
.sort{
    width: 15%;
}
.action{
    width: 20%;
}
.buttons{
  display: flex;
  width: 100%;
}
.record-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 2fr 7fr 2fr;
  gap: .5rem;
  width: 150vw;
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
 
width: 100%;
}
}

.main-container {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  margin-top: 0.5rem;
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

.table-scroll-wrapper {
  overflow-x: auto;
}
.pooling-fixed-header{
  width: 120vw;
}

.table-scroll {
  white-space: nowrap;
  width: -moz-available;
  width: -webkit-fill-available;
}
.pooling-scroll-wrapper {
  overflow-x: auto;
  max-height: 50vh;
}
.pooling-scroll {
  white-space: nowrap;
  width: -moz-available;
  width: -webkit-fill-available;
}
.pooling-scroll thead  {
  position: sticky;
  top: 0;
  z-index: 10;
  background-color: #FFFFE0;
  width: 8rem;
 
}
.pooling-thead th:nth-child(1) {
  position: sticky;
  left: 0;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 1rem;
}

.pooling-thead th:nth-child(2) {
  position: sticky;
  left: 130px;
  z-index: 10;
  background-color: #FFFFE0;
  border: 2px solid red;
  width: 10px;
}
.pooling-thead  th:nth-child(3) {
  position: sticky;
  left: 265px;
  z-index: 10;
  background-color: #FFFFE0;
  border-right: 2px solid #005fa3;
  width: 60px;
}

.pooling-tbody td:nth-child(1) {
  position: sticky;
  left: 0;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 5;
  width: 1rem;
  text-align: left;
}
.pooling-tbody td:nth-child(2) {
  position: sticky;
  left: 130px;
  background-color: #FFFFE0;
  border-right: 2px solid grey;
  z-index: 5;
  width: 10px;
  text-align: left;
}
.pooling-tbody td:nth-child(3) {
  position: sticky;
  left: 260px;
  background-color: #FFFFE0;
  border-right: 2px solid #ddd;
  z-index: 5;
  width: 40px;
  text-align: left;
}
.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
.box-table tr{
  border-style: hidden;

}
.box-description{
  text-align: right;
}
.memberid:hover{
  color: rgb(230, 54, 54);
  transition: .1s;
  text-decoration: underline;
}
.id{
  width: 2px;
  text-wrap: wrap;
}
.name{
  width: 25px;
  text-wrap: wrap;
}
.date{
  width: 2px;
  text-wrap: wrap;
}
.age{
  width: 2%;
  text-wrap: wrap;
}
.sex{
  width: 2%;
  text-wrap: wrap;
}
.mobile{
  width: 5%;
  text-wrap: wrap;
}
.region{
  width: 15%;
  text-wrap: wrap;
}
.province{
  width: 15%;
  text-wrap: wrap;
}
.municipality{
  width: 15%;
  text-wrap: wrap;
}
.status{
  width: 5%;
  text-wrap: wrap;
}
.action{
  width: 5%;
  text-wrap: wrap;
}
.MobileContainer {
  display: flex;
  flex-direction: column;
  margin-bottom: 0.5rem;
}
.MobileHeader {
  border: 1px solid rgb(206, 203, 203);
  background-color: rgb(245, 243, 243);
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  width: 100%;
  padding: 0.5rem;
}
.doc-editor-boxes{
  display: flex;
  flex-direction: column; 
  gap: .5rem;
}
.upload-files{
  display: flex;
  flex-direction: row;

}
.upload-text{


  width: 100%;
  margin-left: 0;
  margin-bottom: 0;
}
 
.image-container {
  overflow: auto;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 600px;
} 

.preview-file {
  display: flex;
  flex-direction: column;
}
</style>