// Client Pay Group Setup 

<template>
  <section class="container p-0" :key="ts">
    <sym-form id="ars0040" :caption="pageName" cardClass="curved-0" headerClass="app-form-header" footerClass="border-top-main app-form-footer darken-2 py-0" bodyClass="app-form-body pb-3 ">

    <!-- footer -->
    <div slot="footer" class="action-buttons p-1">
      <div class="buttons justify-start" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 sm-1 shadow-light>


        <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onSetCanceled" v-if="canSetCanceled && !isCancelled">
          <i class="fa fa-thumbs-o-down fa-lg"></i><span>Cancel</span>
        </button>

      </div>
    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit" > <i class="fa fa-save fa-lg"></i><span>Save</span> </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear" > <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="onDelete" v-show="this.memberRequestList.memberRequestId != 0"> <i class="fa fa-times-circle fa-lg"></i><span>Delete</span> </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
      
    </div>

  </div>

  <div class="header-containerX">
     <div class="mrf-id">
      <sym-int v-model="payGroup.clientPayGroupId" :caption-width="20" caption="Pay ID" lookupId="ArsClientPayGroup" @lostfocus="onClientPayGroupIdLostFocus" @changing="onClientPayGroupIdChanging" @changed="onClientPayGroupIdChanged" @searchfill="onClientPayGroupIdSearchFill" @searchresult="onClientPayGroupIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
   
    </div>
      <sym-tag class="danger text-center border-light lg-2 ml-3" :width="38" v-if="isCancelled">Cancelled</sym-tag>
      <sym-tag class="success text-center border-light lg-2 ml-3" :width="38" v-if="isActivated">Active</sym-tag>
      <sym-tag class="warning text-center border-light lg-2 ml-3" :width="38" v-if="isNotActive">In-Active</sym-tag>
      <sym-tag class="info text-center border-light lg-2 ml-3" :width="38" v-if="isClosed">Closed</sym-tag> 

      



    </div>

    <sym-tabs id="request-info-tabs" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">
     
        <sym-tab title="Pay Group Information" icon="sticky-note-o ">
          <div class="body-container mt-2">
            <div class="container-1">
            
              <div class="mri-info"> 
                <div class="container-3">

                  <sym-text v-model="payGroup.clientPayGroupName" align="bottom" :caption-width="10" caption="Pay Group Name" ></sym-text>          

                  <div class="contract-fields">
                    <sym-int v-model="payGroup.clientContractId" align="bottom" :caption-width="34" caption="Contract ID" display-field="clientContractName" :datasource="contract" lookupId="ArsClientContract" @changing="onClientContractIdChanging"  @searchfill="onClientContractIdSearchFill" @searchresult="onClientContractIdSearchResult"></sym-int> 
                    <sym-text v-model="payGroup.clientContractName" align="bottom" :caption-width="10" caption="Client Contract Name" ></sym-text>          
                    <sym-text v-model="payGroup.clientName" align="bottom" :caption-width="34" caption="Client Name"></sym-text>      
                    <sym-text v-model="payGroup.clusterName" align="bottom" :caption-width="34" caption="Cluster Name"></sym-text>                
                  </div>

                  <div class="paygroup-fields">
                    <sym-int v-model="payGroup.timekeepingId" align="bottom" :caption-width="24" caption="Timekeeping ID" display-field="timekeepingDescription" :datasource="contract" lookupId="PayTimekeeping" @changing="onTimekeepingIdChanging" @searchresult="onTimekeepingIdSearchResult"></sym-int> 
                    <sym-text v-model="payGroup.timekeepingDescription" align="bottom" :caption-width="10" caption="Timekeeping Description" ></sym-text>          
                  </div>

                </div>
                <div class="other-info-fields">
                 
                    <sym-text v-model="payGroup.name3" align="bottom" :caption-width="34" caption="Department Name"></sym-text> 
                    <sym-text v-model="payGroup.name2" align="bottom" :caption-width="34" caption="Name 2"></sym-text> 
                    
                    <sym-text v-model="payGroup.name4" align="bottom" :caption-width="34" caption="Name 4"></sym-text> 
                    <sym-text v-model="payGroup.name5" align="bottom" :caption-width="34" caption="Name 5"></sym-text> 
                    <sym-text v-model="payGroup.name6" align="bottom" :caption-width="34" caption="Name 6"></sym-text> 
                    <sym-text v-model="payGroup.name7" align="bottom" :caption-width="34" caption="Name 7"></sym-text> 
                  </div>
              <div class="computation-fields ">
              <sym-combo v-model="payGroup.payOutComputationId" align="bottom" :caption-width="46" caption="PayOut Computation" display-field="payOutComputationName" :datasource="payOutComputation" ></sym-combo>
              <sym-combo v-model="payGroup.billingComputationId" align="bottom" :caption-width="46" caption="Billing Computation" display-field="billingComputationName" :datasource="billingComputation" @changed="onBillingComputationIdChanged"></sym-combo>
              <sym-combo v-model="payGroup.payFreqId" align="bottom" :caption-width="46" caption="Pay Frequency" display-field="payFreqName" :datasource="payFreq" ></sym-combo>
              <sym-date v-model="payGroup.startCutOffDate" align="bottom" :caption-width="10" caption="Start Cut-Off" ></sym-date>          
              <sym-date v-model="payGroup.endCutOffDate" align="bottom" :caption-width="10" caption="End Cut-Off" ></sym-date>
              <sym-date v-model="payGroup.billingDate" align="bottom" :caption-width="10" caption="Billing" ></sym-date>
              <sym-date v-model="payGroup.payOutDate" align="bottom" :caption-width="10" caption="PayOut" ></sym-date>          
              
            </div>
    
            <div class="location-fields">
              <sym-combo v-model="payGroup.regionId" align="bottom" :caption-width="46" caption="Region" display-field="regionName" :datasource="region" @changed="onRegionIdChanged()"></sym-combo>

              <sym-combo v-model="payGroup.provinceId" align="bottom" :caption-width="43" caption="Province" display-field="provinceName" :datasource="provinces" @changed="onProvinceIdChanged()" ></sym-combo>
              <sym-combo v-model="payGroup.municipalityId" align="bottom" :caption-width="43" caption="Municipality"  display-field="municipalityName" :datasource="municipalities" ></sym-combo>

            </div>
            <div class="box-column">
            </div>
   
                  
              </div>
            </div>

           
       
     



          </div>  

        </sym-tab>
          <sym-tab title="Pay Out Rate Sheet Information" icon="sticky-note-o ">
              <div class="payout-container gap">
                <div class="d-flex justify-end d-inline ml-auto">
                <button class="btn-copy border-main justify-between fw-60 primary mb-2 mt-2" type="button" :class="copyButtonClass" v-if="payGroup.clientPayGroupId > 0" @click="onClickPayOutRateSheet()">
                  <i class="fa fa-table fa-lg"></i><span>PayOut Rate Sheet</span>
                </button>
              </div>
                <div class="app-box-style">
                    <div class="Header app-form-header curved-1">Pay Out Rate Sheet</div>
          
                      <div class="table-scroller">
                        <div class="pay-out-scroller">
                            <table class="light striped-even mb-0 ">
                              <thead>
                                <tr>
                                  <th class="w-7">Trx Name</th>
                                  <th class="w-15 text-right">Formula</th>
                                  <th class="w-10 text-right">Fixed Amount (X)</th>
                                  <th class="w-7 text-center">Basic (A)</th>
                                  <th class="w-7 text-center">Deminimis (B)</th>
                                  <th class="w-7 text-center">Allowance (C)</th>
                                  <!-- <th class="w-7 text-center">Overtime (D)</th>
                                  <th class="w-7 text-center">Night Diff (E)</th>
                                  <th class="w-7 text-center">Holiday (F)</th>
                                  <th class="w-7 text-center">RestDay (G)</th> -->
                                  <th class="w-7">Action</th>
                
                                </tr>
                              </thead>
                            <tbody>
                          <tr v-for="(dtl, index) in payOuts" :key="index">
                
                            
                <td>{{ dtl.payTrxName }}</td>
                <td class="text-right">{{ dtl.payOutSheetFormula }}</td>
                <td class="text-right">{{ dtl.fixedAmount }}</td>
                <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.basicFlag"/></td>
                <td ><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.deminimisFlag"/></td>
                <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.allowanceFlag"/></td>
                <!-- <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.overtimeFlag"/></td>
                <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.nightDifferentialFlag"/></td>
                <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.holidayFlag"/></td>
                <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.restDayFlag"/></td> -->
            
                <td class="p-1">
                  <div class="act-btns" sm-1 mb-0 >
                    <button type="button" class="justify-between info fw-22" @click="onEditPayOutAccess(dtl, index)">
                      <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                    </button>
                    <button type="button" class="warning" title="Delete" @click="onDeletePayOut(index)">
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
<!-- 
            <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
              <button type="button" class="justify-between btn-add" @click="onAddPayOut"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
            </div> -->


            <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
              <button type="button" class="justify-between btn-copy" @click="onCopyBillingRateSheet"><i class="fa fa-copy fa-lg"></i><span>Copy Billing</span></button>
            </div>

          </div>
                
        </div>  
        
        <div class="container-2">
            <div class="day-container">
              
            <div class="day-type-info">
            <div class="Header app-form-header curved-1">Pay Out Day Type Sheet</div>
            
            <div class="table-scroller">
            <div class="fixed-header">
              <table class="light striped-even mb-0 ">
              <thead>
                <tr>
                  <th class="w-50">Day Type Name</th>
                  <th class="w-7 text-right">Percentage</th>
                  <th class="w-10">Action</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(dtl, index) in payOutDayTypes" :key="index">
                  <td>{{ dtl.dayTypeName }}</td>
                  <td class="text-right">{{ dtl.premiumPercentage }}</td>
              
                  <td class="p-1">
                    <div class="act-btns" sm-1 mb-0 >
                      <button type="button" class="justify-between info fw-22" @click="onEditPayOutDayType(dtl, index)">
                        <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                      </button>
                      <button type="button" class="warning" title="Delete" @click="onDeletePayOutDayType(index)">
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
                <button type="button" class="justify-between btn-add" @click="onAddPayOutDayType"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
              </div>


            <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
              <button type="button" class="justify-between btn-copy" @click="onCopyBilling"><i class="fa fa-copy fa-lg"></i><span>Copy Billing</span></button>
            </div>

            </div>
                </div>
              </div>
    
                    
      
                </div>  


              </div>  
        </sym-tab>
        <sym-tab title="Billing Rate Sheet Information" icon="sticky-note-o ">
     
              <div class="d-flex justify-end d-inline ml-auto">
                <button class="btn-copy border-main justify-between fw-60 primary mb-2 mt-2" type="button" :class="copyButtonClass" v-if="payGroup.clientPayGroupId > 0" @click="onClickRateSheet()">
                  <i class="fa fa-table fa-lg"></i><span>Billing Rate Sheet</span>
                </button>
              </div>
            <div class="app-box-style mb-2">
              <div class="Header app-form-header curved-1 mt-2">Billing Rate Sheet</div>
              <table class="light striped-even mb-0 ">
                <thead>
                  <tr>
                    <th class="w-7">Trx Name</th>
                    <th class="w-15 text-right">Formula</th>
                    <!-- <th class="w-10 text-right">Fixed Amount</th> -->
                     <th class="w-10 text-right">Admin Fee</th>
                     <th class="text-center w-13">Shouldered by Coop?</th>
                    <!-- <th class="w-13">Charging Consideration</th> -->
                    <th class="w-7 text-center">Basic (A)</th>
                    <th class="w-7 text-center">Deminimis (B)</th>
                    <th class="w-7 text-center">Allowance (C)</th>
                    <!-- <th class="w-7 text-center">Overtime (D)</th>
                    <th class="w-7 text-center">Night Diff (E)</th>
                    <th class="w-7 text-center">Holiday (F)</th>
                    <th class="w-7 text-center">RestDay (G)</th> -->
                    <th class="w-10">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in billings" :key="index">
                    <td>{{ dtl.payTrxName }}</td>
                    <td  class="text-right">{{ dtl.billingSheetFormula }}</td>
                    <!-- <td class="text-right">{{ dtl.fixedAmount }}</td>
                    <td>{{ dtl.chargingConsiderationName }}</td> -->
                    <td class="text-right">{{ core.toDecimalFormat(dtl.adminFee) }}</td>
                    <!-- <td class="text-right">{{ dtl.orgFlag }}</td> -->
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.orgFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.basicFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.deminimisFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.allowanceFlag"/></td>
                    <!-- <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.overtimeFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.nightDifferentialFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.holidayFlag"/></td>
                    <td><input type="checkbox" :disabled="true" class="d-flex my-2" v-model="dtl.restDayFlag"/></td> -->
                
                    <td class="p-1">
                      <div class="act-btns" sm-1 mb-0 >
                        <button type="button" class="justify-between info fw-22" @click="onEditBillingAccess(dtl, index)">
                          <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                        </button>
                        <button type="button" class="warning" title="Delete" @click="onDeleteBilling(index)">
                          <i class="fa fa-times fa-lg"></i>
                        </button>
                      </div>
                    </td>

                  </tr>
                </tbody>
              </table>
              <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                <div></div>

                <!-- <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                  <button type="button" class="justify-between btn-add" @click="onAddBilling"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                </div> -->

                  <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
                    <button type="button" class="justify-between btn-copy" @click="onCopyPayOutRateSheet"><i class="fa fa-copy fa-lg"></i><span>Copy Payout</span></button>
                  </div>
              </div>  
           </div> 
          
          <div class="container-6">

            <div class="container-2">
              <div class="day-container">
              <div class="app-box-style">
              <div class="Header app-form-header curved-1">Billing Day Type Sheet</div>
              
              <div class="table-scroller">
           
                <table class="light striped-even mb-0 ">
                <thead>
                  <tr>
                    <th class="w-15">Day Type Name</th>
                    <th class="w-5 text-right">Admin Fee</th>
                    <th class="w-6 text-right">Percentage</th>
                    <th class="w-7">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in dayTypes" :key="index">
                    <td>{{ dtl.dayTypeName }}</td>
                    <td class="text-right">{{ dtl.adminFee }}</td>
                    <td class="text-right">{{ dtl.premiumPercentage }}</td>
                        
                    <td class="p-1">
                      <div class="act-btns" sm-1 mb-0 >
                        <button type="button" class="justify-between info fw-22" @click="onEditDayType(dtl, index)">
                          <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                        </button>
                        <button type="button" class="warning" title="Delete" @click="onDeleteDayType(index)">
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
                  <button type="button" class="justify-between btn-add" @click="onAddDayType"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                </div>

            <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-35 mb-0 sm-1 shadow-light>
              <button type="button" class="justify-between btn-copy" @click="onCopyPayOut"><i class="fa fa-copy fa-lg"></i><span>Copy Payout</span></button>
            </div>


              </div>
                  </div>
              </div>
            </div>

            
        
            <div class="app-box-style" v-show="payGroup.billingComputationId !=1">
          
          <div class="Header app-form-header curved-1">Overtime Billing Rate Sheet</div>
              <sym-table class="table-select striped-odd bill hover mb-0" colorClass="light">
                <sym-table-header slot="header">
                  <tr class="align-top">
                    <th class="w-10 text-center"><i class="fa fa-check"></i></th>
                    <th class="w-90">Trx Name</th>
                  </tr>
                  
                </sym-table-header>
                  <sym-table-body slot="body" v-show="payGroup.clientPayGroupId !=0">
                    <tr v-for="(payTrx, index) in payTrxList" :key="index" class="align-top" @click="onTogglePayTrxCodeSelection(payTrx)">
                      <td class="w-10 text-center"><input type="checkbox" :checked="isPayTrxCodeSelected(payTrx)"></td>
                      <td class="w-90">{{ payTrx.payTrxName }}</td>
                    </tr>
                  </sym-table-body>
                </sym-table>
            </div> 
            
      
          </div>
  
     
        </sym-tab> 
<sym-tab title="Holiday Setup" icon="sticky-note-o ">
              <div class="payout-container gap">
                
                <div class="payOut-info">
              
                      <div class="table-scroller">
        <div class="container-2">
            <div class="day-container">
            <div class="day-type-info">
            <div class="Header app-form-header curved-1">Holiday Setup</div>
            
            <div class="table-scroller">
            <div class="fixed-header">
              <table class="light striped-even mb-0 ">
              <thead>
                <tr>
                  <th class="w-50">Holiday</th>
                  <th class="w-10">Date</th>
                  <th class="w-30">Day Type Name</th>
                  <th class="w-10">Action</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(dtl, index) in holidays" :key="index">
                  <td>{{ dtl.holidayName }}</td>
                  <td >{{ dtl.holidayDate }}</td>
                  <td >{{ dtl.dayTypeName }}</td>
              
                  <td class="p-1">
                    <div class="act-btns" sm-1 mb-0 >
                      <button type="button" class="justify-between info fw-22" @click="onEditHoliday(dtl, index)">
                        <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                      </button>
                      <button type="button" class="warning" title="Delete" @click="onDeleteHoliday(index)">
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
                <button type="button" class="justify-between btn-add" @click="onAddHoliday"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
              </div>

            </div>
                </div>
              </div>
    
                    
      
                </div>  



        </div>



        

                
        </div>  
        


              </div>  
        </sym-tab>
<sym-tab title="MRF" icon="clipboard">
<div class="mrf-info">

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">Total Count: {{ payGroup.mrfCount }}</span>
  </div>
 

                <button type="button" class="success mb-3 w-100 text-center lg-2 p-1 shadow-soft" @click="tableToExcel('table', 'Lorem Table')"><i class="fa fa-file-excel-o mr-2"></i>Export To Excel </button>
  
      <div class="table-scroll-wrapper">
        <div class="fixed-header">
        <table class="table-scroll light striped-even mb-0" ref="table" id="loremTable" summary="lorem ipsum sit amet" rules="groups" frame="hsides" border="2">
    
      <thead>
        <tr class="align-top">
          <th class="w-6">MRF #</th>
          <th class="w-20">MRF Name</th>
          <th class="w-8">Status</th>
          <th class="w-10">Platform</th>
          <th class="w-12">Contract Name</th>
          <th class="w-10">Position</th>
          <th class="w-8">Emp Type</th>
          <th class="w-6">Duration</th>
          <th class="w-6">Vac</th>
          <th class="w-6">Tot Hired</th>
          <th class="w-6">Tot Vac</th>
          <th class="w-10">Deployment date</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr class="align-top" v-for="(dtl, index) in memberRequestList" :key="index" @click="onClickMemberRequest(dtl.memberRequestId)">
          <td >{{ dtl.memberRequestId }}</td>
          <td>{{ dtl.memberRequestName }}</td>
          <td>{{ dtl.memberRequestStatusName }}</td>
          <td>{{ dtl.platformName }}</td>
          <td>{{ dtl.clientContractName }}</td>
          <td>{{ dtl.memberRequestPositionName }}</td>
          <td>{{ dtl.employmentTypeName }}</td>
          <td>{{ dtl.endDate }}</td>
          <td>{{ dtl.vacancyCount }}</td>
          <td>{{ dtl.totalHired }}</td>
          <td>{{ dtl.totalVacancy }}</td>
          <td>{{ dtl.deploymentDate }}</td>
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
<sym-modal
  id="payOut-editor"
  v-model="isPayOutEditorVisible"
  size="lg"
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
    <form id="ars0040A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="payout-scroller">
      <div class="payOut-editor-boxes">
        <sym-combo v-model="payOut.payTrxCode" caption="Trx Name" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onPayTrxCodeChanging"></sym-combo>
        <sym-text v-model="payOut.payOutSheetFormula" caption="Formula" align="bottom" @changing="onPayOutSheetFormulaChanging($event,payOut)"></sym-text>
        <sym-decimal v-model="payOut.fixedAmount"  caption="Fixed Amount" align="bottom" ></sym-decimal>
        <sym-check v-model="payOut.basicFlag" :caption-width="30" caption="Basic" align="bottom" ></sym-check>
        <sym-check v-model="payOut.deminimisFlag" :caption-width="30"  caption="Deminimis" align="bottom" ></sym-check>
        <sym-check v-model="payOut.allowanceFlag" :caption-width="30"  caption="Allowance" align="bottom" ></sym-check>
        <!-- <sym-check v-model="payOut.overtimeFlag" :caption-width="30" caption="Overtime" align="bottom" ></sym-check>
        <sym-check v-model="payOut.nightDifferentialFlag" :caption-width="30" caption="Night Diff" align="bottom" ></sym-check>
        <sym-check v-model="payOut.holidayFlag" :caption-width="30" caption="Holiday" align="bottom" ></sym-check>
        <sym-check v-model="payOut.restDayFlag" :caption-width="30" caption="RestDay" align="bottom" ></sym-check> -->

      </div>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitPayOut()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isPayOutEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>


      <!-- billing -->
      <sym-modal
  id="billing-editor"
  v-model="isBillingEditorVisible"
  size="lg"
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
    <form id="ars0040B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="billing-scroller">
        <div class="billing-editor-boxes">
        <sym-combo v-model="billing.payTrxCode" caption="Trx Name" align="bottom" display-field="payTrxName" :datasource="payTrx" @changing="onBillingPayTrxCodeChanging"></sym-combo>
        <sym-combo v-model="billing.chargingConsiderationId" caption="Charging Consideration" align="bottom" display-field="chargingConsiderationName" :datasource="chargingConsideration"></sym-combo>
        <sym-text v-model="billing.billingSheetFormula" caption="Formula" align="bottom" @changing="onBillingSheetFormulaChanging($event,billing)"></sym-text>
        <sym-decimal v-model="billing.fixedAmount" caption="Fixed Amount" align="bottom" ></sym-decimal>
        <sym-check v-model="billing.basicFlag" :caption-width="30" caption="Basic" align="bottom" ></sym-check>
        <sym-check v-model="billing.deminimisFlag" :caption-width="30" caption="Deminimis" align="bottom" ></sym-check>
        <sym-check v-model="billing.allowanceFlag" :caption-width="30" caption="Allowance" align="bottom" ></sym-check>
        <sym-check v-model="billing.overtimeFlag" :caption-width="30" caption="Overtime" align="bottom" ></sym-check>
        <sym-check v-model="billing.nightDifferentialFlag" :caption-width="30" caption="Night Diff" align="bottom" ></sym-check>
        <sym-check v-model="billing.holidayFlag" :caption-width="30" caption="Holiday" align="bottom" ></sym-check>
        <sym-check v-model="billing.restDayFlag" :caption-width="30" caption="RestDay" align="bottom" ></sym-check>

      </div>
      </div>
   

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitBilling()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isBillingEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

</sym-modal>
      <sym-modal
  id="dayType-editor"
  v-model="isDayTypeEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorDayTypeTitle"
  @show="onShowDayTypeEditor($event)"
  @hide="onHideDayTypeEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0040C" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="day-editor-boxes">
        <sym-integer v-model="dayType.dayTypeId" caption="Day Type ID" align="bottom" lookupId="DbsDayType" @changing="onDayTypeIdChanging" @searchresult="onDayTypeIdSearchResult"></sym-integer>
        <sym-text v-model="dayType.dayTypeName" caption="Day Type Name" align="bottom" ></sym-text>
        <sym-decimal v-model="dayType.adminFee" caption="Admin Fee" align="bottom" ></sym-decimal>
        <sym-decimal v-model="dayType.premiumPercentage" caption="Percentage" align="bottom" ></sym-decimal>
  
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitDayType()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isDayTypeEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>
      <sym-modal
  id="payOutDayType-editor"
  v-model="isPayOutDayTypeEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorPayOutDayTypeTitle"
  @show="onShowPayOutDayTypeEditor($event)"
  @hide="onHidePayOutDayTypeEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0040D" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="day-editor-boxes">
        <sym-integer v-model="payOutDayType.dayTypeId" caption="Day Type ID" align="bottom" lookupId="DbsDayType" @changing="onPayOutDayTypeIdChanging" @searchresult="onPayOutDayTypeIdSearchResult"></sym-integer>
        <sym-text v-model="payOutDayType.dayTypeName" caption="Day Type Name" align="bottom" ></sym-text>
        <sym-decimal v-model="payOutDayType.premiumPercentage" caption="Percentage" align="bottom" ></sym-decimal>
  
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitPayOutDayType()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isPayOutDayTypeEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>


  <!-- Holiday -->
   <sym-modal
  id="holiday-editor"
  v-model="isHolidayEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorHolidayTitle"
  @show="onShowHolidayEditor($event)"
  @hide="onHideHolidayEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="ars0040E" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="day-editor-boxes">
        <sym-integer v-model="holiday.holidayId" caption="Holiday ID" align="bottom" lookupId="DbsHoliday" @changing="onHolidayIdChanging" @searchresult="onHolidayIdSearchResult"></sym-integer>
        <sym-text v-model="holiday.holidayName" caption="Holiday Name" align="bottom" ></sym-text>

        <sym-integer v-model="holiday.dayTypeId" caption="Day Type ID" align="bottom" lookupId="DbsDayType" @changing="onHolidayDayTypeIdChanging" @searchresult="onHolidayDayTypeIdSearchResult"></sym-integer>
        <sym-text v-model="holiday.dayTypeName" caption="Day Type Name" align="bottom" ></sym-text>

        <sym-date v-model="holiday.holidayDate" caption="Holiday Date" align="bottom" ></sym-date>
  
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitHoliday()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isHolidayEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div> 

    </form>  
  </div>  

  </sym-modal>


</section>  
</template>


<script>
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
import SymDecimal from '../../comp/SymDecimal.vue';

export default {
  components: { SymInteger, SymMemo, SymDecimal },
  extends: PageMaintenance,
  name: 'ars0040',

  data () {
    return {
    
      payGroup: {  
        clientPayGroupId: 0,
        clientPayGroupName: '',
        clientContractId: 0,
        clientContractName: "",
        clusterName: "",
        payOutComputationId: 0,
        billingComputationId: 0,
        adminFee: 0,
        
        payFreqId: 0,
        startCutOffDate: null,
        endCutOffDate: null,
        payOutDate: null,
        billingDate: null,
        payOutRemitFlag: false,
        clientPayGroupStatusId: 0,
        regionId:'',
        provinceId: '',
        municipalityId: '',
        rateAmount:0,
        deminimisAmount:0,
        allowanceAmount:0,
        minimumWageFlag: false,
        name2:'',
        name3:'',
        name4:'',
        name5:'',
        name6:'',
        name7:'',
        timekeepingId:0,
        timekeepingDescription:'',
        clientName:'',
        lockId: ''
      },

      payOuts: [],

      payOut: {
        clientPayOutDetailId: 0,
        clientPayGroupId: 0,
        payTrxCode: '',
        payTrxName: '',
        payOutSheetFormula: '',
        fixedAmount: 0,
        basicFlag: false,
        deminimisFlag: false,
        allowanceFlag: false,
        overtimeFlag: false,
        nightDifferentialFlag: false,
        holidayFlag: false,
        restDayFlag: false,
        lockId: ''
       
      },

      payOutIndex: -1,
      isAddingPayOut: false,
      isPayOutEditorVisible: false,

      billings: [],

      billing: {
        clientBillingDetailId: 0,
        clientPayGroupId: 0,
        payTrxCode: '',
        payTrxName: '',
        chargingConsiderationId: 0,
        chargingConsiderationName: '',
        billingSheetFormula: '',
        fixedAmount: 0,
        basicFlag: false,
        deminimisFlag: false,
        allowanceFlag: false,
        overtimeFlag: false,
        nightDifferentialFlag: false,
        holidayFlag: false,
        restDayFlag: false,
        lockId: ''
       
      },

      overtimes: [],

      billingIndex: -1,
      isAddingBilling: false,
      isBillingEditorVisible: false,


      dayTypes: [],

      dayType: {
        clientDayTypeDetailId: 0,
        clientPayGroupId: 0,
        dayTypeId: 0,
        dayTypeName: '',
        premiumPercentage: 0,
        adminFee: 0,
        orgFlag: false,
        lockId: ''
       
      },

      dayTypeIndex: -1,
      isAddingDayType: false,
      isDayTypeEditorVisible: false,
      


      payOutDayTypes: [],

      payOutDayType: {
        clientPayOutDayTypeDetailId: 0,
        clientPayGroupId: 0,
        dayTypeId: 0,
        dayTypeName: '',
        premiumPercentage: 0,
        lockId: ''
       
      },

      payOutDayTypeIndex: -1,
      isAddingPayOutDayType: false,
      isPayOutDayTypeEditorVisible: false,


      holidays: [],

      holiday: {
        clientPayOutHolidayDetailId: 0,
        clientPayGroupId: 0,
        dayTypeId: 0,
        dayTypeName: '',
        holidayId: 0,
        holidayName: 0,
        holidayDate: null,
        lockId: ''
       
      },

      holidayIndex: -1,
      isAddingHoliday: false,
      isHolidayEditorVisible: false,

      activeTabIndex: 0,

      provinces: [],
      municipalities: [],

           uri :'data:application/vnd.ms-excel;base64,',
        template:'<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64: function(s){ return window.btoa(unescape(encodeURIComponent(s))) },
        format: function(s, c) { return s.replace(/{(\w+)}/g, function(m, p) { return c[p]; }) },

      refBillingComputationId:0,  
      passwordFlag: false,    
    };
  },

  computed: {

    isHideOvertime(){
      return this.billingComputationId===1;
    },

    canSetCanceled () {
      return this.payGroup.clientPayGroupStatusId === 1 && this.payGroup.mrfCount === 0;
    },

    isCancelled () {
      return this.payGroup.clientPayGroupStatusId === 3;
    },

    isActivated () {
      return this.payGroup.clientPayGroupStatusId === 1;
    },

    isNotActive () {
      return this.payGroup.clientPayGroupStatusId === 2;
    },

    isClosed () {
      return this.payGroup.clientPayGroupStatusId === 4;
    },

    editorPayOutTitle () {
      if (this.isAddingPayOut) {
        return 'Add Pay Out Detail';
      }
      return 'Edit Pay Out Detail';
    },

    editorBillingTitle () {
      if (this.isAddingBilling) {
        return 'Add Billing Detail';
      }
      return 'Edit Billing Detail';
    },

    editorDayTypeTitle () {
      if (this.isAddingDayType) {
        return 'Add Day Type Detail';
      }
      return 'Edit Day Type Detail';
    },


    editorPayOutDayTypeTitle () {
      if (this.isAddingPayOutDayType) {
        return 'Add Pay Out Day Type Detail';
      }
      return 'Edit Pay Out Day Type Detail';
    },

    editorHolidayTitle () {
      if (this.isAddingHoliday) {
        return 'Add Holiday Detail';
      }
      return 'Edit Holiday Detail';
    },

  },
  
  methods: {

    onEditBillingAccess(dtl, index) {

      const me = this;
      if (!me.passwordFlag) {    
      let promise = me.isActionAllowed('EDIT-BIL-SHEET');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.passwordFlag =  true;
            me.onEditBilling(dtl, index);
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );
} else {
  me.onEditBilling(dtl, index);
  
}
    },


    onEditPayOutAccess(dtl, index) {

      const me = this;
      if (!me.passwordFlag) {    
      let promise = me.isActionAllowed('EDIT-PAY-SHEET');
      
      promise.then(
        reply => {
          if (reply === 'yes') {
            me.passwordFlag =  true;
            me.onEditPayOut(dtl, index);
            return;
          }
        },
        fault => {
          me.showFault(fault);
        }
      );
} else {
  me.onEditPayOut(dtl, index);
  
}
    },



  onBillingSheetFormulaChanging(e, billing) {
    e.callback = (event) => {
      billing.basicFlag = event.proposedValue.includes('A');
      billing.deminimisFlag = event.proposedValue.includes('B');
      billing.allowanceFlag = event.proposedValue.includes('C');
      billing.overtimeFlag = event.proposedValue.includes('D');
      billing.nightDifferentialFlag = event.proposedValue.includes('E');
      billing.holidayFlag = event.proposedValue.includes('F');
      billing.restDayFlag = event.proposedValue.includes('G');
      return true; 
    };
  },

    onPayOutSheetFormulaChanging(e, payOut) {
    e.callback = (event) => {
      payOut.basicFlag = event.proposedValue.includes('A');
      payOut.deminimisFlag = event.proposedValue.includes('B');
      payOut.allowanceFlag = event.proposedValue.includes('C');
      payOut.overtimeFlag = event.proposedValue.includes('D');
      payOut.nightDifferentialFlag = event.proposedValue.includes('E');
      payOut.holidayFlag = event.proposedValue.includes('F');
      payOut.restDayFlag = event.proposedValue.includes('G');
      return true; 
    };
  },

 onCopyBillingRateSheet() {
   
  this.payOuts = this.billings.map(({ billingSheetFormula,...rest }) => ({
    ...rest,                        
    payOutSheetFormula: billingSheetFormula      

  }));

    },

    onCopyPayOutRateSheet() {
   
  this.billings = this.payOuts.map(({ payOutSheetFormula,...rest }) => ({
    ...rest,                        
    billingSheetFormula: payOutSheetFormula  

  }));

    },



    onBillingComputationIdChanged (newValue) {
      const me = this;
    
      let o = me.billingComputation.find( o => o.billingComputationId == newValue);
      if (o) {
        me.refBillingComputationId = o.billingComputationId;
         me.overtimes = [];
      }
    },


    onClickRateSheet(){
      const me = this;

      let route = {
        name: "ars0130",
        query: {
          clientPayGroupId: me.payGroup.clientPayGroupId,
        },
      };
      me.go(route);

    },


    onClickPayOutRateSheet(){
      const me = this;

      let route = {
        name: "ars0150",
        query: {
           clientPayGroupId: me.payGroup.clientPayGroupId,
        },
      };
      me.go(route);

    },

  onHolidayIdChanging (e) {
      e.message = "Holiday ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.holidayIdCallback;
    },

    holidayIdCallback (e) {
      const me = this;
      let filter = "HolidayId=" + e.proposedValue;
      return getList('dbo.QDbsHoliday', 'HolidayId, HolidayName, HolidayDate, DayTypeId, DayTypeName', '', filter).then(
        info => {
          if (info && info.length) {
               
            let index = me.holidays.findIndex(obj => obj.holidayId === e.proposedValue);
            if (index > -1) {
              e.message = 'Holiday ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }
            
            me.holiday.holidayName = info[0].holidayName;
            me.holiday.holidayDate = this.core.convertDates(info[0]).holidayDate;
            me.holiday.dayTypeId = info[0].dayTypeId;
            me.holiday.dayTypeName = info[0].dayTypeName;
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

    onHolidayIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        holiday = this.holiday;
        let index = this.holidays.findIndex(obj => obj.holidayId === data.holidayId);
        if (index > -1) {
          this.advice.fault('Holiday ID <b>' + data.holidayId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        holiday.holidayId = data.holidayId;
        holiday.holidayName = data.holidayName;
        holiday.holidayDate = this.core.convertDates(data).holidayDate;
        holiday.dayTypeId = data.dayTypeId;
        holiday.dayTypeName = data.dayTypeName;
        this.focusNext();

    },


    onHolidayDayTypeIdChanging (e) {
      e.message = "Holiday Day Type ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.holidayDayTypeIdCallback;
    },

    holidayDayTypeIdCallback (e) {
      const me = this;
      let filter = "DayTypeId=" + e.proposedValue;
      return getList('dbo.DbsDayType', 'DayTypeId, DayTypeName, PremiumPercentage', '', filter).then(
        info => {
          if (info && info.length) {
               
            let index = me.holiday.findIndex(obj => obj.dayTypeId === e.proposedValue);
            
            me.holiday.dayTypeName = info[0].dayTypeName;
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

    onHolidayDayTypeIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        holiday = this.holiday;
        let index = this.holidays.findIndex(obj => obj.dayTypeId === data.dayTypeId);
        holiday.dayTypeId = data.dayTypeId;
        holiday.dayTypeName = data.dayTypeName;
 
        this.focusNext();

    },
 
    onShowHolidayEditor () {
      const me = this;
      
      me.setActiveModel('holiday');

      me.setRequiredMode(
        'dayTypeId',
        'holidayId',
        'holidayDate',
      );

      me.setDisplayMode(
        'dayTypeName',
        'holidayName',
      );

      setTimeout(() => {
        this.setFocus('holidayId');
      }, 200);
    },

    onHideHolidayEditor () {
      const me = this;

      me.isAddingHoliday = false;
      me.setActiveModel();
    },

    onEditHoliday (dtl, index) {

      const d = this.holiday;
      this.holidayIndex = index;

     dtl = this.core.convertDates(dtl);
      if (dtl.holidayDate === null) {
        dtl.holidayDate = this.core.emptyDateTime();
      }


      d.clientHolidayDetailId = dtl.clientHolidayDetailId;
      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeName = dtl.dayTypeName;
      d.holidayId = dtl.holidayId;
      d.holidayName = dtl.holidayName;
      d.holidayDate = dtl.holidayDate;
      
      this.isHolidayEditorVisible = true;
    },

    onAddHoliday () {
      const me = this;
      
      me.clearHolidayPad();
      
      me.holiday.clientHolidayDetailId = -1 
      me.holiday.holidayDate = me.today;

      me.isHolidayEditorVisible = true;
      me.isAddingHoliday = true;
        
    },

    onSubmitHoliday() {
      const me = this,
        d = me.holiday;

      if (!d.holidayId) {
        me.isHolidayEditorVisible = false;
        return;
      }

      if (!me.isValid("ars0040E")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingHoliday) {
 
        Object.assign(dtl, d);
      
        me.holidays.push(dtl);

        me.clearHolidayPad();
        
        me.advice.info("Holiday Day Type '" + dtl.holidayId + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("holidayId");
        
      } else {

        dtl = me.holidays[me.holidayIndex];
        dtl.clientHolidayDetailId = d.clientHolidayDetailId;
        dtl.holidayId = d.holidayId;
        dtl.holidayName = d.holidayName;
        dtl.dayTypeId = d.dayTypeId;
        dtl.dayTypeName = d.dayTypeName;
        dtl.holidayDate = d.holidayDate;
   
        me.isHolidayEditorVisible = false;

      }
    },

    onDeleteHoliday(index) {
      const me = this;

      me.dialog.confirm( "Holiday <b>" + me.holidays[index].holidayName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.holidays.splice(index, 1);
          }
          return;
        });
    },

    clearHolidayPad () {
      const d = this.holiday;

      d.clientHolidayDetailId = 0;
       d.holidayId = 0;
      d.holidayName = '';
      d.dayTypeId = 0;
      d.dayTypeName = '';
      d.holidayDate = null;
    
    },

// STOP NA



    onCopyBilling() {
      this.payOutDayTypes = this.dayTypes.map(dayType => ({
        ...dayType, 
      }));
  
      this.refreshTotal();
    },

    onCopyPayOut() {
      this.dayTypes = this.payOutDayTypes.map(dayType => ({
        ...dayType, 
      }));
  
      this.refreshTotal();
    },




    tableToExcel(table, name){
        
        if (!table.nodeType) table = this.$refs.table
        var ctx = {worksheet: name || 'Worksheet', table: table.innerHTML}
        window.location.href = this.uri + this.base64(this.format(this.template, ctx))
      },
    onTimekeepingIdChanging (e) {
      e.message = "Timekeeping ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.timekeepingIdCallback;
    },

    timekeepingIdCallback (e) {
      const me = this;
      let filter = "TimekeepingId='" + e.proposedValue + "'";
      return getList('dbo.PayTimekeeping', 'TimekeepingId, TimekeepingDescription', '', filter).then(
        timekeeping => {
          if (timekeeping && timekeeping.length) {
            me.payGroup.timekeepingId = timekeeping[0].timekeepingId;
            me.payGroup.timekeepingDescription = timekeeping[0].timekeepingDescription;
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

    onTimekeepingIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.payGroup;

      item.timekeepingId = data.timekeepingId;
      item.timekeepingDescription = data.timekeepingDescription;
      
      this.focusNext();

    },



    getTargetPath() {
      const me = this,
        q = {};

      if (me.payGroup.clientPayGroupId) {
        q.clientPayGroupId = me.payGroup.clientPayGroupId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;
      
      if ("clientPayGroupId" in q && me.core.isInteger(q.clientPayGroupId)) {
        me.payGroup.clientPayGroupId = parseInt(q.clientPayGroupId);
      }

     },



    onClickMemberRequest(memberRequestId) {
      const me = this;

      let route = {
        name: "ars0050",
        query: {
          memberRequestId: memberRequestId,
          activeTabIndex: 1 
        },
      };
      me.go(route);
    },


    onSetCanceled () {
      const me = this;
      
      
      let promise = me.isActionAllowed('CAN-STATUS-PAY');
      
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


    addPercentageSymbol() {
        this.payGroup.adminFee = this.payGroup.adminFeeFormat;

      let cleanedValue = this.payGroup.adminFeeFormat.replace(/[^1-9.]/g, '').slice(0, 2);
      if (cleanedValue) {
        this.payGroup.adminFeeFormat = `${cleanedValue}%`;
        
      } else {

        this.payGroup.adminFee = 0;
        this.payGroup.adminFeeFormat = 0;
      }
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


    onRegionIdChanged() {
      const me = this,
        wait = me.wait();

        let o = me.region.find( o => o.regionId == me.payGroup.regionId);
      
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

   
    onPayOutDayTypeIdChanging (e) {
      e.message = "Pay Out Day Type ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.payOutDayTypeIdCallback;
    },

    payOutDayTypeIdCallback (e) {
      const me = this;
      let filter = "DayTypeId=" + e.proposedValue;
      return getList('dbo.DbsDayType', 'DayTypeId, DayTypeName, PremiumPercentage', '', filter).then(
        info => {
          if (info && info.length) {
               
            let index = me.payOutDayTypes.findIndex(obj => obj.dayTypeId === e.proposedValue);
            if (index > -1) {
              e.message = 'Day Type ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }
            
            me.payOutDayType.dayTypeName = info[0].dayTypeName;
            me.payOutDayType.premiumPercentage = info[0].premiumPercentage;
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

    onPayOutDayTypeIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        payOutDayType = this.payOutDayType;
        let index = this.payOutDayTypes.findIndex(obj => obj.dayTypeId === data.dayTypeId);
        if (index > -1) {
          this.advice.fault('Pay Out Day Type ID <b>' + data.dayTypeId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        payOutDayType.dayTypeId = data.dayTypeId;
        payOutDayType.dayTypeName = data.dayTypeName;
        payOutDayType.premiumPercentage = data.premiumPercentage;
 
        this.focusNext();

    },
 
    onShowPayOutDayTypeEditor () {
      const me = this;
      
      me.setActiveModel('payOutDayType');

      me.setRequiredMode(
        'dayTypeId',
        'premiumPercentage'
      );

      me.setDisplayMode(
        'dayTypeName'
      );

      setTimeout(() => {
        this.setFocus('dayTypeId');
      }, 200);
    },

    onHidePayOutDayTypeEditor () {
      const me = this;

      me.isAddingPayOutDayType = false;
      me.setActiveModel();
    },

    onEditPayOutDayType (dtl, index) {

      const d = this.payOutDayType;
      this.payOutDayTypeIndex = index;

      d.clientPayOutDayTypeDetailId = dtl.clientPayOutDayTypeDetailId;
      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeName = dtl.dayTypeName;

      d.premiumPercentage = dtl.premiumPercentage;
      
      this.isPayOutDayTypeEditorVisible = true;

    },

    onAddPayOutDayType () {
      const me = this;
      
      me.clearPayOutDayTypePad();
      
      me.payOutDayType.clientPayOutDayTypeDetailId = -1 

      me.isPayOutDayTypeEditorVisible = true;
      me.isAddingPayOutDayType = true;
        
    },

    onSubmitPayOutDayType() {
      const me = this,
        d = me.payOutDayType;

      if (!d.dayTypeId) {
        me.isPayOutDayTypeEditorVisible = false;
        return;
      }

      if (!me.isValid("ars0040D")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingPayOutDayType) {
 
        Object.assign(dtl, d);
      
        me.payOutDayTypes.push(dtl);

        me.clearPayOutDayTypePad();
        
        me.advice.info("Pay Out Day Type '" + dtl.dayTypeId + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("dayTypeId");
        
      } else {

        dtl = me.payOutDayTypes[me.payOutDayTypeIndex];
        dtl.clientPayOutDayTypeDetailId = d.clientPayOutDayTypeDetailId;
        dtl.dayTypeId = d.dayTypeId;
        dtl.dayTypeName = d.dayTypeName;
        dtl.premiumPercentage = d.premiumPercentage;
   
        me.isPayOutDayTypeEditorVisible = false;

      }
    },

    onDeletePayOutDayType(index) {
      const me = this;

      me.dialog.confirm( "Pay Out Day Type <b>" + me.payOutDayTypes[index].dayTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.payOutDayTypes.splice(index, 1);
          }
          return;
        });
    },

    clearPayOutDayTypePad () {
      const d = this.payOutDayType;

      d.clientPayOutDayTypeDetailId = 0;
      d.dayTypeId = 0;
      d.dayTypeName = '';
      d.premiumPercentage = 0;
    
    },
    
    onDayTypeIdChanging (e) {
      e.message = "Day Type ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.dayTypeIdCallback;
    },

    dayTypeIdCallback (e) {
      const me = this;
      let filter = "DayTypeId=" + e.proposedValue;
      return getList('dbo.DbsDayType', 'DayTypeId, DayTypeName, PremiumPercentage', '', filter).then(
        info => {
          if (info && info.length) {
               
            let index = me.dayTypes.findIndex(obj => obj.dayTypeId === e.proposedValue);
            if (index > -1) {
              e.message = 'Day Type ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }
            
            me.dayType.dayTypeName = dayType[0].dayTypeName;
            me.dayType.premiumPercentage = dayType[0].premiumPercentage;
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

    onDayTypeIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        dayType = this.dayType;
        let index = this.dayTypes.findIndex(obj => obj.dayTypeId === data.dayTypeId);
        if (index > -1) {
          this.advice.fault('Day Type ID <b>' + data.dayTypeId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        dayType.dayTypeId = data.dayTypeId;
        dayType.dayTypeName = data.dayTypeName;
        dayType.premiumPercentage = data.premiumPercentage;
 
        this.focusNext();

    },
 
    onShowDayTypeEditor () {
      const me = this;
      
      me.setActiveModel('dayType');

      me.setRequiredMode(
        'dayTypeId',
        'premiumPercentage'
      );

      me.setDisplayMode(
        'dayTypeName'
      );

      setTimeout(() => {
        this.setFocus('dayTypeId');
      }, 200);
    },

    onHideDayTypeEditor () {
      const me = this;

      me.isAddingDayType = false;
      me.setActiveModel();
    },

    onEditDayType (dtl, index) {

      const d = this.dayType;
      this.dayTypeIndex = index;

      d.clientDayTypeDetailId = dtl.clientDayTypeDetailId;
      d.dayTypeId = dtl.dayTypeId;
      d.dayTypeName = dtl.dayTypeName;

      d.premiumPercentage = dtl.premiumPercentage;
      d.adminFee = dtl.adminFee;
      
      this.isDayTypeEditorVisible = true;
    },

    onAddDayType () {
      const me = this;
      
      me.clearDayTypePad();
      
      me.dayType.clientDayTypeDetailId = -1 

      me.isDayTypeEditorVisible = true;
      me.isAddingDayType = true;
        
    },

    onSubmitDayType() {
      const me = this,
        d = me.dayType;

      if (!d.dayTypeId) {
        me.isDayTypeEditorVisible = false;
        return;
      }

      if (!me.isValid("ars0040C")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingDayType) {
 
        Object.assign(dtl, d);
        me.dayTypes.push(dtl);

        me.clearDayTypePad();
        
        me.advice.info("Day Type '" + dtl.dayTypeId + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("dayTypeId");
        
      } else {

        dtl = me.dayTypes[me.dayTypeIndex];
        dtl.clientDayTypeDetailId = d.clientDayTypeDetailId;
        dtl.dayTypeId = d.dayTypeId;
        dtl.dayTypeName = d.dayTypeName;
        dtl.premiumPercentage = d.premiumPercentage;
        dtl.adminFee = d.adminFee;
   
        me.isDayTypeEditorVisible = false;

      }
    },

    onDeleteDayType(index) {
      const me = this;

      me.dialog.confirm( "Day Type <b>" + me.dayTypes[index].dayTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.dayTypes.splice(index, 1);
          }
          return;
        });
    },

    clearDayTypePad () {
      const d = this.dayType;

      d.clientDayTypeDetailId = 0;
      d.dayTypeId = 0;
      d.dayTypeName = '';

      d.premiumPercentage = 0;
      d.adminFee = 0;
    
    },

    isPayTrxCodeSelected (payTrx) { 
      return this.overtimes.findIndex(obj => obj.payTrxCode === payTrx.payTrxCode) > -1;
    },
  
    onTogglePayTrxCodeSelection (payTrx) {
      const
        me = this,
        index = me.overtimes.findIndex(obj => obj.payTrxCode === payTrx.payTrxCode);

      if (index > -1) {
        me.overtimes.splice(index, 1);
      } else {
        me.overtimes.push({
          clientPayGroupId: me.payGroup.clientPayGroupId,
          payTrxCode: payTrx.payTrxCode
        });
      }
    },

    onShowBillingEditor () {
      const me = this;
      
      me.setActiveModel('billing');

      me.setRequiredMode(
        'payTrxCode',
        'chargingConsiderationId'
      );

      me.setDisplayMode(
        'basicFlag',
        'deminimisFlag',
        'allowanceFlag',
        'overtimeFlag',
        'nightDifferentialFlag',
        'holidayFlag',
        'restDayFlag'
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

      d.clientBillingDetailId = dtl.clientBillingDetailId;
      
      let route = {
        name: "ars0130",
        query: {
          clientBillingDetailId: d.clientBillingDetailId,
          clientPayGroupId: this.payGroup.clientPayGroupId,
        },
      };
      this.go(route);

      // d.payTrxCode = dtl.payTrxCode;
      // d.chargingConsiderationId = dtl.chargingConsiderationId;
      // d.billingSheetFormula = dtl.billingSheetFormula;
      // d.fixedAmount = dtl.fixedAmount;
      // d.basicFlag = dtl.basicFlag;
      // d.deminimisFlag = dtl.deminimisFlag;
      // d.allowanceFlag = dtl.allowanceFlag;
      // d.overtimeFlag = dtl.overtimeFlag;
      // d.nightDifferentialFlag = dtl.nightDifferentialFlag;
      // d.holidayFlag = dtl.holidayFlag;
      // d.restDayFlag = dtl.restDayFlag;
      
      // this.isBillingEditorVisible = true;
    },

    onAddBilling () {
      const me = this;
      
      me.clearBillingPad();
      
      me.billing.clientBillingDetailId = -1 

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

      if (!me.isValid("ars0040B")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
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
      
        me.chargingConsideration.forEach((chargingConsideration) => {
          if (chargingConsideration.chargingConsiderationId == dtl.chargingConsiderationId) {
            dtl.chargingConsiderationName = chargingConsideration.chargingConsiderationName;
          }
        });

        me.billings.push(dtl);

        me.clearBillingPad();
        
        me.advice.info("Pay Trx '" + dtl.payTrxCode + "' added to list.", {
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

        me.chargingConsideration.forEach((dtl) => {
          if (dtl.chargingConsiderationId == d.chargingConsiderationId) {
            d.chargingConsiderationName = dtl.chargingConsiderationName;
          }
        });

        dtl.clientBillingDetailId = d.clientBillingDetailId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.chargingConsiderationId = d.chargingConsiderationId;
        dtl.chargingConsiderationName = d.chargingConsiderationName;

        dtl.billingSheetFormula = d.billingSheetFormula;
        dtl.fixedAmount = d.fixedAmount;
        dtl.basicFlag = d.basicFlag;
        dtl.deminimisFlag = d.deminimisFlag;
        dtl.allowanceFlag = d.allowanceFlag;
        dtl.overtimeFlag = d.overtimeFlag;
        dtl.nightDifferentialFlag = d.nightDifferentialFlag;
        dtl.holidayFlag = d.holidayFlag;
        dtl.restDayFlag = d.restDayFlag;
   
        me.isBillingEditorVisible = false;

      }
    },

    onDeleteBilling(index) {
      const me = this;

      me.dialog.confirm( "Pay Trx <b>" + me.billings[index].payTrxCode + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.billings.splice(index, 1);
          }
          return;
        });
    },

    clearBillingPad () {
      const d = this.billing;

      d.clientBillingDetailId = 0;
      d.payTrxCode = '';
      d.chargingConsiderationId = 0;

      d.billingSheetFormula = '';
      d.fixedAmount = 0;
      d.basicFlag = false;
      d.deminimisFlag = false;
      d.allowanceFlag = false;
      d.overtimeFlag = false;
      d.nightDifferentialFlag = false;
      d.holidayFlag = false;
      d.restDayFlag = false;
    
    },

onBillingPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.billingPayTrxCodeCallback;
    },

    billingPayTrxCodeCallback(e) {
      const me = this;

      let index = me.billings.findIndex(obj => obj.payTrxCode === e.proposedValue);
      if (index > -1) {

        const payTrxName = me.billings[index].payTrxName;
        e.message = 'Pay Trx Name <b>' + payTrxName + '</b> is already in the list.';
        return false;
      }

      return true;
    },
 
   
onPayTrxCodeChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.payTrxCodeCallback;
    },

    payTrxCodeCallback(e) {
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

      me.setDisplayMode(
        'basicFlag',
        'deminimisFlag',
        'allowanceFlag',
        'overtimeFlag',
       'nightDifferentialFlag',
       'holidayFlag',
       'restDayFlag'
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

      d.clientPayOutDetailId = dtl.clientPayOutDetailId;
      // d.payTrxCode = dtl.payTrxCode;
      // d.payOutSheetFormula = dtl.payOutSheetFormula;
      // d.fixedAmount = dtl.fixedAmount;
      // d.basicFlag = dtl.basicFlag;
      // d.deminimisFlag = dtl.deminimisFlag;
      // d.allowanceFlag = dtl.allowanceFlag;
      // d.overtimeFlag = dtl.overtimeFlag;
      // d.nightDifferentialFlag = dtl.nightDifferentialFlag;
      // d.holidayFlag = dtl.holidayFlag;
      // d.restDayFlag = dtl.restDayFlag;
      
      // this.isPayOutEditorVisible = true;

       let route = {
        name: "ars0150",
        query: {
          clientPayOutDetailId: d.clientPayOutDetailId,
          clientPayGroupId: this.payGroup.clientPayGroupId,
        },
      };
      this.go(route);

    },

    onAddPayOut () {
      const me = this;
      
      me.clearPayOutPad();
      
      me.payOut.clientPayOutDetailId = -1 

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

      if (!me.isValid("ars0040A")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
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
      
        me.payOuts.push(dtl);

        me.clearPayOutPad();
        
        me.advice.info("Pay Trx '" + dtl.payTrxCode + "' added to list.", {
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

        dtl.clientPayOutDetailId = d.clientPayOutDetailId;
        dtl.payTrxCode = d.payTrxCode;
        dtl.payTrxName = d.payTrxName;
        dtl.payOutSheetFormula = d.payOutSheetFormula;
        dtl.fixedAmount = d.fixedAmount;
        dtl.basicFlag = d.basicFlag;
        dtl.deminimisFlag = d.deminimisFlag;
        dtl.allowanceFlag = d.allowanceFlag;
        dtl.overtimeFlag = d.overtimeFlag;
        dtl.nightDifferentialFlag = d.nightDifferentialFlag;
        dtl.holidayFlag = d.holidayFlag;
        dtl.restDayFlag = d.restDayFlag;
   
        me.isPayOutEditorVisible = false;

      }
    },

    onDeletePayOut(index) {
      const me = this;

      me.dialog.confirm( "Pay Trx <b>" + me.payOuts[index].payTrxCode + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.payOuts.splice(index, 1);
          }
          return;
        });
    },

    clearPayOutPad () {
      const d = this.payOut;

      d.clientPayOutDetailId = 0;
      d.payTrxCode = '';
      d.payOutSheetFormula = '';
      d.fixedAmount = 0;
      d.basicFlag = false;
      d.deminimisFlag = false;
      d.allowanceFlag = false;
      d.overtimeFlag = false;
      d.nightDifferentialFlag = false;
      d.holidayFlag = false;
      d.restDayFlag = false;
    
    },
    onActiveTabIndexChanged () {
      this.reload();
    },
 

    onCancel () {
      const me = this;

      me.dialog.confirm('Ready to cancel <b>'+ me.payGroup.clientPayGroupName +'</b> Pay Group.<br>Once cancelled, Pay Group cannot be modified.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.payGroup.clientPayGroupStatusId = 3;
    
            me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onActivate () {
      const me = this;

      me.dialog.confirm('Ready to activate <b>'+ me.payGroup.clientPayGroupName +'</b> Pay Group.<br>Once activated, Pay Group cannot be canceled.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.payGroup.clientPayGroupStatusId = 1;
            // me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onClose () {
      const me = this;

      me.dialog.confirm('Ready to close <b>'+ me.payGroup.clientPayGroupName +'</b> Pay Group.<br>Once closed, Pay Group cannot be activate/canceled.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
        reply => {
          if (reply === 'yes') {
            me.payGroup.clientPayGroupStatusId = 4;
            // me.isCancelling = true;
            me.onSubmit();
          }
          return;
        }
      );
    },

    onClientContractIdChanging (e) {
      e.message = "Client Contract ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.clientContractIdCallback;
    },

    clientContractIdCallback (e) {
      const me = this;
      let filter = "ClientContractId=" + e.proposedValue + " AND OrgId=" + me.sym.userInfo.userOrgId;
      return getList('dbo.QArsClientContract', 'ClientContractId, ClientContractName, ClientName, ClusterName', '', filter).then(
        contract => {
          if (contract && contract.length) {
            me.payGroup.clientContractName = contract[0].clientContractName;
            me.payGroup.clientName = contract[0].clientName;
            me.payGroup.clusterName = contract[0].clusterName;
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

   onClientContractIdSearchFill (e) {
      e.filter = "OrgId = " + this.sym.userInfo.userOrgId  
    },

    onClientContractIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.payGroup;

      item.clientContractId = data.clientContractId;
      item.clientContractName = data.clientContractName;
      item.clientName = data.clientName;
      item.clusterName = data.clusterName;
      
      this.focusNext();

    },


    loadData () {
      const
        me = this,
        wait = me.wait();
    
      me.getReferences()
        .then(data => {
     
          if (!me.core.isBoolean(data)) {
            me.contract = data.contract;
            me.payOutComputation = data.payOutComputation;
            me.billingComputation = data.billingComputation;
            me.payFreq = data.payFreq;
            me.payTrx = data.payTrx;
            me.payOutSheet = data.payOutSheet;
            me.chargingConsideration = data.chargingConsideration;
            me.billingSheet = data.billingSheet;
            me.overtimeSheet = data.overtimeSheet;
            me.payTrxList = data.payTrx;
            me.dayTypeSheet = data.dayTypeSheet;
            me.region = data.region;
            me.holidayList = data.holidays;

          }
          if (me.payGroup.clientPayGroupId< 0) {
            return Promise.resolve(null);
          }
          return me.getClientPayGroup();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.payGroup.length) {
            me.setModels(info);
          } else {
            if (me.payGroup.clientPayGroupId > -1) {
              let message = "Client Pay Group ID '<b>" + me.payGroup.clientPayGroupId + "</b>' not found.";
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

            me.payOuts = me.payOutSheet;
            me.overtimes = me.overtimeSheet;
            me.dayTypes = me.dayTypeSheet;
            me.holidays = me.holidayList;
            me.payOutDayTypes = me.dayTypeSheet;


          }
          if (me.isCancelled || me.isClosed)  {
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

      me.payGroup = me.core.convertDates(info.payGroup[0]);

      me.payOuts = info.payOuts;
      me.billings = info.billings;
      me.overtimes = info.overtimes;
      me.dayTypes = info.dayTypes;
      me.payOutDayTypes = info.payOutDayTypes;
      me.memberRequestList = info.mrfs;
      me.provinces = info.province;
      me.municipalities = info.municipality;
      me.holidays = info.holidays;
      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldPayGroup = JSON.stringify(me.payGroup);
      me.oldPayOuts = JSON.stringify(me.payOuts);
      me.oldOvertimes = JSON.stringify(me.overtimes);
      me.oldDayTypes = JSON.stringify(me.dayTypes);
      me.oldPayOutDayTypes = JSON.stringify(me.payOutDayTypes);
      me.oldHolidays = JSON.stringify(me.holidays);
    },

    // API calls

    getProvinces() {
      return get("api/client-pay-group-provinces/" + this.payGroup.regionId);
    },

    getMunicipalities() {
      const me = this; 
      return get("api/client-pay-group-municipalities/" + me.payGroup.regionId + "/" + me.payGroup.provinceId);
    },

    getClientPayGroup () {
      return get('api/client-pay-groups/' + this.payGroup.clientPayGroupId);
    },

    getReferences () {
      const me = this;

      if (me.contract.length) {
        return Promise.resolve(true);
      }
      return get('api/references/ars0040');
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/client-pay-groups/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);
       
      return ajax('api/client-pay-groups/' + this.payGroup.clientPayGroupId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        request = this.request,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/client-pay-groups/' + this.payGroup.clientPayGroupId + '/' + request.lockId, options);
    },

    getApiPayload () {
      const
        me = this,
        payGroup = {};

      Object.assign(payGroup, me.payGroup);
      payGroup.payOuts = me.payOuts;
      payGroup.overtimes = me.overtimes;
      payGroup.dayTypes = me.dayTypes;
      payGroup.payOutDayTypes = me.payOutDayTypes;
      payGroup.holidays = me.holidays;
      return payGroup;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('payGroup','payOuts','payOut','overtimes','dayTypes','dayType','payOutDayTypes','payOutDayType','holidays','holiday');
      dc.keyField = 'clientPayGroupId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.isCancelling = false;
      this.memberRequestList = [];
      this.refreshOldRefs();
      
      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 50);
    },

    onClientPayGroupIdChanging (e) {
      e.callback = this.clientPayGroupIdCallback;
    },

    clientPayGroupIdCallback (e) {
      e.message = "Client Pay Group ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.ArsClientPayGroup', 'clientPayGroupId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onClientPayGroupIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onClientPayGroupIdLostFocus () {
      const me = this;

      if (!me.payGroup.clientPayGroupId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onClientPayGroupIdSearchFill (e) {
      e.filter = "OrgId = " + this.sym.userInfo.userOrgId  
    },


    onClientPayGroupIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.payGroup.clientPayGroupId = data.clientPayGroupId;
      me.replaceUrl();
      me.loadData();
    },


    onSubmit (nextRoute) {
      const me = this;

      if (me.payGroup.clientPayGroupId===3) {
        me.onReset();
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
              me.payGroup.clientPayGroupId = success; 
            }

            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'

              if (me.isCancelling) {
                message = "Pay Group # '" + me.payGroup.clientPayGroupId + "' cancelled."
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

      getSafeDeleteFlag('ArsClientPayGroup', me.payGroup.clientPayGroupId)
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
          'clientContractId',
          'clientContractName',
          'clientPayGroupName',
          'payOutComputationId',
          'billingComputationId',
          'adminFee',
          'payFreqId',
          'startCutOffDate',
          'endCutOffDate',
          'payOutDate',
          'regionId',
          'deminimisAmount',
          'allowanceAmount',
          'minimumWageFlag',
          'billingDate',
        );


        me.setFocus('clientPayGroupName');
      }, 100);

    },


    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();

        me.setRequiredMode(
          'clientContractId',
          'clientPayGroupName',
          'payOutComputationId',
          'billingComputationId',
          'adminFee',
          'payFreqId',
          'startCutOffDate',
          'endCutOffDate',
          'payOutDate',
          'regionId',
          'billingDate',
        );

        me.setDisplayMode(
          'clientContractName',
          'clientName',
          'clusterName',
          'timekeepingDescription'

        );

        me.setFocus('clientPayGroupName');
      }, 100);

    },

    hasChanges () {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.payGroup) !== me.oldPayGroup) { return true; }
      if (JSON.stringify(me.payOuts) !== me.oldPayOuts) { return true; }
      if (JSON.stringify(me.overtimes) !== me.oldOvertimes) { return true; }
      if (JSON.stringify(me.dayTypes) !== me.oldDayTypes) { return true; }
      if (JSON.stringify(me.payOutDayTypes) !== me.oldPayOutDayTypes) { return true; }
      if (JSON.stringify(me.holidays) !== me.oldHolidays) { return true; }

      return false;
    },

  },

  created () {
    const me = this;

    me.isCancelling = false;


    me.oldPayGroup = '';
    me.oldPayOuts = '';
    me.oldOvertimes = '';
    me.oldDayTypes = '';
    me.oldPayOutDayTypes = '';
    me.oldHolidays = '';

    me.contract = []; 
    me.payOutComputation = [];     
    me.billingComputation = [];
    me.payFreq = []; 
    me.payOutSheet = []; 
    me.billingSheet = []; 
    me.overtimeSheet = []; 
    me.dayTypeSheet = []; 
    me.payOutDayTypeSheet = []; 
    me.region = []; 

    me.payTrx = []; 
    me.chargingConsideration = []; 
    me.payTrxList = []; 
    me.memberRequestList = [];
    me.holidayList= [];
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
  grid-template-columns: 2fr 2fr 2fr;
  gap: 1rem;

}
.block-container{
  gap: .5rem;
  display: flex;
  flex-direction: column;
}
.container-1{
  display: flex;
  flex-direction: column;
}
.container-2{

  display: flex;
  flex-direction: row;
  gap: .5rem;
}
.container-3{

display: grid;
grid-template-columns: .6fr 1.5fr 1fr ;
gap: .5rem;
}
.container-4{
display: grid;
grid-template-columns: 1fr 1fr;
gap: .5rem;
}
.container-5{
display: grid;
grid-template-columns: 1fr 1fr;
gap: .5rem;
}
.container-6{
display: grid;
grid-template-columns: 1fr 1fr;
margin-bottom: 5px;
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
.table-scroller{

    overflow: auto;
    height: auto;
    max-height: 50vh;
}
.fixed-header {
  overflow: hidden;
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
width: 10vw;
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
  grid-template-columns:  .6fr 1fr 1.2fr 1.2fr;
  gap: .5rem;
}
.paygroup-fields{
  display: grid;
  grid-template-columns:  .4fr 1fr;
  gap: .5rem;
}
.other-info-fields{
  display: grid;
  grid-template-columns: repeat(6,1fr);
  gap: .5rem;
}
.client-fields{
  display: grid;
  grid-template-columns:  1fr 2fr;
  gap: .5rem;
}
.computation-fields{
  display: grid;
  grid-template-columns:  1fr 1fr .8fr 1fr .8fr .8fr .8fr .8fr .8fr .8fr;
  gap: .5rem;
}
.location-fields{
  display: grid;
  grid-template-columns:  1fr 1fr 1fr;
  gap: .5rem;
}
.fee-fields{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;

}
.startDate-fields{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;

}
.payout-fields{
  display: grid;
  grid-template-columns: 1fr 1fr ;
  gap: .5rem;

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
.payOut-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  margin-top: 10px;
}
.billing-info{
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
.day-type-info{
  display: flex;
  flex-direction: column;
  justify-content: center;
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
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  height: 50vh;
  overflow: auto;
}
.civil-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  height: 35vh;
}
.sex-info{
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(190, 190, 190, 0.918);
  background-color: rgba(219, 215, 215, 0.473);
  padding: 10px;
  border-radius: 10px;
  height: 20vh;
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
  grid-template-columns: 2fr 1fr ;
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
.candidate-action{
  width: 6%;
}
.payout-scroller{
  overflow: auto;
}
.day-editor-boxes{

  display: grid;
  gap: .5rem;
  grid-template-columns: 1fr 3fr 1fr ;
}
.payOut-editor-boxes{
  width: 80vw;
  display: grid;
  gap: .5rem;
  grid-template-columns: .7fr 1fr .5fr .5fr .5fr .5fr .5fr .5fr  .5fr .5fr;
}
.billing-editor-boxes{
  width: 95vw;
  display: grid;
  gap: .5rem;
  grid-template-columns: .7fr 1.3fr 1.5fr .6fr .5fr .5fr .5fr .5fr .5fr .5fr .5fr;
}
.billing-scroll{
  overflow: auto;
}

table.billing-table{
  min-width: 100%;
  width:150%;
}
.box-column{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.MobileContainer {
  display: flex;
  flex-direction: column;
  margin-bottom: 0.5rem;
}
.MobileHeader{ 
  border: 1px solid rgb(206, 203, 203);
  background-color: rgb(245, 243, 243);
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  width: 100%;
  padding: 0.5rem;
}
.MobilePhone {
  border-top-right-radius: 0;
  border-bottom-right-radius: 4px;
}
.table-scroll-wrapper {
  overflow-x: auto;

  
}

.table-scroll {
  white-space: nowrap;
  width: -moz-available;
  width: -webkit-fill-available;
}

.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
.payout-container{
  display: flex;
  flex-direction: column;
}
.day-table-scroller{
  overflow: auto;
  max-height: 100vh;
  height: 30vh
}
.billing-scroller{
  overflow: auto;
}
thead{
  position: sticky;
  top: 0;
  z-index: 20;

}
</style>