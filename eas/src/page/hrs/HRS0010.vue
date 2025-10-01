// Member Profile

<!-- <meta http-equiv="Content-Security-Policy" content="upgrade-insecure-requests"></meta> -->

<template>
  <section class="container " :key="ts">
    <sym-form
      id="hrs0010"
      :caption="pageName"
        cardClass="curved-0"
      headerClass="app-form-header"
      footerClass="border-top-main app-form-footer darken-2 py-0"
      bodyClass="app-form-body pb-3 "
    >


    <!-- footer -->
      <div slot="footer" class="action-buttons p-1">

        <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="submitButtonClass" class="justify-between btn-save"  @click="onSubmit" > <i class="fa fa-save fa-lg"></i><span>Save</span> </button>
          <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear" > <i class="fa fa-undo fa-lg"></i><span>Clear</span> </button>
          <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(1)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
          <button type="button" :class="deleteButtonClass" class="justify-between btn-delete"  @click="goBack()" > <i class="fa fa-arrow-left mr-2"></i><span>Back</span> </button>
        </div>
      </div>


      <div class="header-containerXX">
        <div class="memberid-info fw-110">
          <sym-int v-model="member.memberId" :caption-width="35" caption="Member ID" lookupId="HrsMember" @lostfocus="onMemberIdLostFocus" @changing="onMemberIdChanging" @changed="onMemberIdChanged" @searchresult="onMemberIdSearchResult" ></sym-int>
          <div class="buttons d-inline">
            <button class="btn-new warning-light border-main justify-between fw-28 shadow-light" :tabindex="-1" @mousedown.prevent @click="onNew" > <i class="fa fa-file-o"></i>New </button>
          </div>
          <sym-tag class="text-center border-light lg-2   curved-1 -10 shadow-light status" style="text-transform:uppercase" v-if="member.memberId > 0" :class="getMemberStatusColorClass(member.memberStatusId)">{{ member.memberStatusName }}</sym-tag>
          </div>


      </div>

      <!-- Personal Information Section -->
      <sym-tabs id="member-info-tabs" v-model="activeTabIndex" @changed="onActiveTabIndexChanged">
        <sym-tab title="Personal Info" icon="user-o">
          <div class="main-container">
            <div class="personal gap ">
              <div class="container-1">
                  <div class="app-box-style">
<<<<<<< HEAD
                    <div class="Header app-form-header curved-1">PROFILE profiles</div>
=======
                    <div class="Header app-form-header curved-1">PROFILE</div>
>>>>>>> Updateheader
                    <div class="main">
                      <div class="test">
                        <div class="box-profile gap">

                       
                            <div class="box-img">
                              <div class="pos-relative fw-40 curved border-lightgray shadow-light  "  >
                                <img :src="getImageUrl()" alt="..." class="photo d-block mx-auto fw-30 border-main ">
                                <input type="file" class="input-file " title="" accept=".jpg, .png" ref="inputfile" @input="onUploadPhoto" :disabled="member.memberId === 0 ">
                                <button type="button"  class="info w-100 lg-1 text-center upload-btn" :disabled="member.memberId === 0">Upload Photo</button>
                              </div>
                            </div>
                            <div class="box-profile-content">                           
                                <div class="box-employee gap">
                                  <sym-text v-model="member.memberEmployeeId" align="bottom" :caption-width="35" caption="EmployeeId"> </sym-text>
                                  <sym-text v-model="member.memberLastName" align="bottom" :caption-width="35" :disabled="isDisabled" caption="Last Name" @changing="onMemberLastNameChanging"> </sym-text>
                                  <sym-text v-model="member.memberFirstName" align="bottom" :caption-width="35" caption="First Name" @changing="onMemberFirstNameChanging"></sym-text>
                                  <sym-text v-model="member.memberMiddleName" align="bottom" :caption-width="35" caption="Middle Name" @changing="onMemberMiddleNameChanging"></sym-text>
                                  <sym-combo v-model="member.memberSuffixId" align="bottom" :caption-width="46" caption="Suffix" display-field="memberSuffixName" :datasource="suffix"></sym-combo>
                                </div>
                                <div class="box-third-column">
                                <sym-combo v-model="member.sexId" align="bottom" :caption-width="46" caption="Sex" display-field="sexName" :datasource="sex"></sym-combo>
                                <sym-date v-model="member.birthDate" align="bottom" :caption-width="35" caption="Birth Date"  @changing="onBirthDateChanging" @changed="onBirthDateChanged"></sym-date>
                                <sym-int v-model="member.age" align="bottom" :caption-width="35" caption="Age" class="text-left"></sym-int>
                                <sym-text v-model="member.birthProvinceId" align="bottom" :caption-width="34" caption="Province ID" display-field="provinceName" :datasource="birthProvinces" lookupId="HrsProvince" @changing="onBirthProvinceIdChanging" @changed="onBirthProvinceIdChanged" @searchresult="onBirthProvinceIdSearchResult"></sym-text>
                                <sym-text v-model="member.birthProvinceName" :caption-width="10" caption="Birth Place Province " align="bottom"></sym-text>
                                <sym-combo v-model="member.birthMunicipalityId" align="bottom" :caption-width="43" caption=" Birth Place Municipality"  display-field="municipalityName" :datasource="birthMunicipalities"></sym-combo>
                                  
                              </div>
                              <div class="box-forth-column">
                                <div class="MobileContainer">
                                  <span class="MobileHeader">Height (cm)</span>
                                  <input class="MobilePhone"  v-model="member.height" @input="validateHeight"  type="mobile" :disabled="member.memberId ===0"/>
                                </div>
                                <div class="MobileContainer">
                                  <span class="MobileHeader">Weight (kg)</span>
                                  <input class="MobilePhone"  v-model="member.weight" @input="validateWeight"  type="mobile" :disabled="member.memberId ===0" />
                                </div>
                                <sym-combo v-model="member.bloodTypeId" align="bottom" :caption-width="46" caption="Blood Type" display-field="bloodTypeName" :datasource="bloodType" ></sym-combo>
                                <sym-combo v-model="member.religionId" align="bottom" :caption-width="46" caption="Religion" display-field="religionName" :datasource="religion" ></sym-combo>
                                <sym-combo v-model="member.civilStatusId" align="bottom" :caption-width="46" caption="Civil Status" display-field="civilStatusName" :datasource="civilStatus" ></sym-combo>
                              </div>
                            </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  
                  
              </div>
                <div class="container-2">
                  <div class="box-form">
                    <div class="app-box-style ">
                      <div class="Header app-form-header curved-1">ADDRESS</div>
                        <div class="box-loc-column">
                          <sym-text v-model="member.provinceId" align="bottom" :caption-width="34" caption="Province ID" display-field="provinceName" :datasource="provinces" lookupId="HrsProvince" @changing="onProvinceIdChanging" @changed="onProvinceIdChanged" @searchresult="onProvinceIdSearchResult"></sym-text>
                          <sym-text v-model="member.provinceName"  align="bottom" :caption-width="34" caption="Province Name"></sym-text>
                          <sym-combo v-model="member.municipalityId" align="bottom" :caption-width="45" caption="Municipality" display-field="municipalityName" :datasource="municipalities" @changed="onMunicipalityIdChanged()" ></sym-combo>
                          <sym-combo v-model="member.barangayId" align="bottom" :caption-width="45" caption="Barangay" display-field="barangayName" :datasource="barangays" @changed="onBarangayIdChanged(member.barangayId)"></sym-combo>
                        </div> 
                        <div class="box-brgy">
                          <div class="MobileContainer">
                            <span class="MobileHeader">Postal code</span>
                            <input class="MobilePhone"  v-model="member.postalCode" @input="validatePostalCode" type="mobile"  :disabled="true"   />
                          </div>
                          <sym-text  v-model="member.address1" align="bottom" :caption-width="35" caption="Address (House/Lot No, Building/Subdivision/Village Name, Street)" ></sym-text>
                        </div>

                    </div>
                    <div class="app-box-style">
                          <div class="Header app-form-header curved-1">ALTERNATIVE ADDRESS</div>
                            <div class="box-loc-column">
                              <sym-text v-model="member.alternateProvinceId" align="bottom" :caption-width="34" caption="Province ID" display-field="provinceName" :datasource="alternateProvinces" lookupId="HrsProvince" @changing="onAlternateProvinceIdChanging" @changed="onAlternateProvinceIdChanged" @searchresult="onAlternateProvinceIdSearchResult"></sym-text>
                              <sym-text v-model="member.alternateProvinceName"  align="bottom" :caption-width="34" caption="Province Name"></sym-text>
                              <sym-combo v-model="member.alternateMunicipalityId" align="bottom" :caption-width="45" caption="Municipality" display-field="municipalityName" :datasource="alternateMunicipalities" @changed="onAlternateMunicipalityIdChanged()" ></sym-combo>
                              <sym-combo v-model="member.alternateBarangayId" align="bottom" :caption-width="45" caption="Barangay" display-field="barangayName" :datasource="alternateBarangays" @changed="onAlternateBarangayIdChanged(member.alternateBarangayId)"></sym-combo>
                            </div>
                            <div class="box-brgy">
                              
                              <div class="MobileContainer">
                                <span class="MobileHeader">Postal code</span>
                                <input class="MobilePhone"  v-model="member.alternatePostalCode" @input="validateAlternatePostalCode" type="mobile"  :disabled="true"   />
                              </div>
                              <sym-text  v-model="member.address2" align="bottom" :caption-width="35" caption="Address (House/Lot No, Building/Subdivision/Village Name, Street)" ></sym-text>
                            </div>

                    </div>
                  </div>     
                </div>
                  <div class="container-3">

                      <div class="app-box-style ">
                          <div class="Header app-form-header">CONTACT DETAILS</div>
                          <div class="contact">
                            <div class=" gap box-fix-column">
                              <div class="MobileContainer">
                                <span class="MobileHeader">Phone Number</span>
                                <input class="MobilePhone"  v-model="member.phoneNumber" @input="validatePhoneNumber"  type="mobile"  :disabled="member.memberId ===0" />
                              </div>
                              
                              <div class="MobileContainer">
                              <span class="MobileHeader">Mobile Number</span>
                              <input class="MobilePhone"  v-model="member.mobileNumber" @input="validateMobileNumber" placeholder="(09**)*******" type="mobile"   :style="{
                                borderColor: isValidMobileNumber == 'valid' ? 'green' : isValidMobileNumber == 'invalid' ? '' : '', }" :disabled="member.memberId ===0"/>
                              </div>
                              <sym-text v-model="member.email" align="bottom" :caption-width="35" caption="Email" @changing="onEmailChanging"></sym-text>
                              <sym-text v-model="member.facebook" align="bottom" :caption-width="35" caption="Facebook" ></sym-text>
                              <sym-text v-model="member.instagram" align="bottom" :caption-width="35"  caption="Instagram" ></sym-text>
                            </div>

                              
                        </div>
                      </div>
                      <div class="app-box-style">        
                            <!--Medical -->
                            <div>
                            <div class="Header app-form-header curved-1">MEDICAL RESULT</div>

                            <div class="medical-scroller">
                              <table class="light striped-even  mb-0 medical-tb">
                                <thead>
                                  <tr>
                                    <th class="w-40">Medical Result</th>
                                    <th class="w-30">File Name</th>
                                    <th class="w-10 text-center">Action</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr v-for="(dtl, index) in medicals" :key="index">
                                    <td>{{ dtl.medicalResultTypeName }}</td>
                                    <td>{{ dtl.medicalResultTypeFileName}}</td>
                                                  
                                    <td class="p-1">
                                      <div class="medical-btns gap" sm-1 mb-0>
                                        <div v-if="dtl.medicalResultTypeFileName">
                                        <a class="button success sm-4 " :href="dtl.fileUrl" download  title="Download" ><i class="fa fa-download"></i ></a> 
                                        </div>
                                        <button type="button" title="Edit" class="justify-between info  btn-dtl-edit" @click="onEditMedical(dtl, index)" > <i class="fa fa-edit fa-lg"></i> </button>
                                        <button type="button"   class="warning btn-dtl-delete "  title="Delete"  @click="onDeleteMedical(index)" > <i class="fa fa-times fa-lg"></i> </button>
                                      </div>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>


                            <!--Medical -->

                            </div>
                      
                      <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                        <div></div>
                        <div
                          class="buttons justify-center w-100" :info-light="isMono" :outline="isMono"  border-main fw-35 mb-0 sm-1 shadow-light >
                          <button type="button" class="justify-between btn-add" @click="onAddMedical" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                          <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(15)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
                        </div>
                      </div>
                    </div>


                    </div>
                  </div>
            </div>
        </div>
      </sym-tab>

        <!-- MEMBER INFORMATION -->
        <sym-tab title="Member Info" icon="vcard-o" class="scroller-tabs">
          <div class="main-container ">
            <div class="personal gap">
                <div class="container-1">
                  <div class="app-box-style ">
                    <div class="Header app-form-header curved-1">OTHER INFORMATION</div>
                    <div class="main">
                      <div class="test">
                        <div class="box-info-column">
                          <sym-combo v-model="member.cdaMemberTypeId" align="bottom" :caption-width="90" caption="CDA Membership Type"  display-field="cdaMemberTypeName" :datasource="cdaMemberTypes" @changed="cdaMemberTypeIdChanged()"></sym-combo>
                          <sym-dec v-model="member.cdaMemberTypeAmount" align="bottom" :caption-width="50" caption="Amount" :blankZero="false" ></sym-dec>
                          <sym-combo v-model="member.memberTypeId" align="bottom" :caption-width="43" caption="Type"  display-field="memberTypeName" :datasource="memberType" @changed="onMemberTypeIdChanged"></sym-combo>
                          <sym-combo v-model="member.typeQualificationDetailId" align="bottom" :caption-width="43" caption="Qualification" display-field="typeQualificationName" :datasource="memberTypeQualification" ></sym-combo>
                          <sym-int  v-model="member.expectedSalary" align="bottom" :caption-width="50" caption="Expected Salary" ></sym-int>
                          <sym-date v-model="member.initialHiredDate" align="bottom" :caption-width="35" caption="Initial Hired Date" @changing ="onInitialHiredDateChanging" ></sym-date>
                        </div>
                        <div class="box-check-column">
                          
                           
                              <sym-check v-model="member.abroadFlag" :caption-width="80" caption="Willngness to go abroad"></sym-check>
                              <sym-check  v-model="member.relocateFlag" :caption-width="80" caption="Are you willing to relocate?"></sym-check>
               
                          <sym-check v-model="member.weekendHolidayFlag" :caption-width="80" caption="Available working on weekend or holiday?"></sym-check>
                        </div> 


                      </div>
                    </div>
                  </div>
                </div>
                <div class="container-2">




                    <!-- RNR -->
                  <div class="app-box-style" v-show="member.memberTypeId ==2">
                          <div class="Header app-form-header curved-1">RNR</div>

                          <div class="table-scroller">
                          <div class="fixed-header">
                            <table class="light striped-even mb-0 ">
                            <thead>
                              <tr>
                                <th class="w-50">Rnr Recording</th>
                                <th class="w-20 ">Rnr Number</th>
                                <th class="W-2  ">Action</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr v-for="(dtl, index) in rnrRecordings" :key="index">
                                <td>{{ dtl.rnrRecordingName }}</td>
                                <td>{{ dtl.rnrNumber }}</td>
                                <td class="p-1">
                                  <div class="act-btns" sm-1 mb-0 >
                                    <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteRnrRecording(index)">
                                      <i class="fa fa-times fa-lg"></i>
                                    </button>
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                          </div>
                        </div>
                          <div class="action-buttons light border-main p-1 border-top-0 mb-2">
                            <div></div>

                            <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
                              <button type="button" class="justify-between btn-add" @click="onAddRnrRecording"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                              <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(3)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
                            </div>
                          </div>
                  </div>
                  <div class="box-form">

                  
                    <div class="app-box-style">
                      <div class="Header app-form-header curved-1">PAYMENT INFORMATION</div>
                        <div class="box-bank">
                          <div class="MobileContainer">
                            <span class="MobileHeader">GCASH Verified Number</span>
                            <input class="MobilePhone"  v-model="member.gCashNumber" @input="validateGcashNumber"  type="mobile"  :disabled="member.memberId ===0"/>
                          </div>
                          <sym-combo v-model="member.bankId" align="bottom" :caption-width="70" caption="Bank" display-field="bankName" :datasource="banks" ></sym-combo>
                          <div class="MobileContainer">
                              <span class="MobileHeader">Bank Account Number</span>
                              <input class="MobilePhone"  v-model="member.bankAccountNumber" @input="validateBankNumber"  type="mobile" :disabled="member.memberId ===0"/>
                          </div>
                        </div>
                    </div>

                      <!-- Vaccine -->
                    <div class="app-box-style">
                            <div class="Header app-form-header curved-1">VACCINE</div>

                            <div class="table-scroller">
                            <div class="fixed-header">
                              <table class="light striped-even mb-0 ">
                              <thead>
                                <tr>
                                  <th class="w-20">Vaccine Type</th>
                                  <th class="w-30 ">Vaccine Name</th>
                                  <th class="w-20">Vaccine Date</th>
                                  <th class="W-5">Action</th>
                                </tr>
                              </thead>
                              <tbody>
                                <tr v-for="(dtl, index) in vaccines" :key="index">
                                  <td>{{ dtl.vaccineTypeName }}</td>
                                  <td>{{ dtl.vaccineName }}</td>
                                  <td>{{ dtl.vaccineDate }}</td>
                                  <td class="p-1">
                                    <div class="act-btns" sm-1 mb-0 >
                                      <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditVaccine(dtl, index)"><i class="fa fa-edit fa-lg"></i><span>Edit</span></button>
                                      <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteVaccine(index)"><i class="fa fa-times fa-lg"></i></button>
                                    </div>
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                            </div>
                          </div>
                            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
                              <div></div>

                              <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
                                <button type="button" class="justify-between btn-add" @click="onAddVaccine"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(4)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
                              </div>
                            </div>
                    </div>
                  </div>

                </div>
                <div class="container-3">
                  <div class="d-inline-block border-soft w-100 mb-3 app-box-style">
                    <div class="Header app-form-header curved-1">LANGUAGE</div>

 <div class="language-info" v-show="member.memberId !== 0">
    <div class="language-selector">

      <sym-text
        placeholder="Type to search/add language..."
        v-model="languageInput"
        align="bottom"
        :caption-width="35"
        caption="Language"
        list="language-suggestions"
      ></sym-text>

      <datalist id="language-suggestions">
        <option
          v-for="lang in filteredLanguages"
          :key="lang.languageId" 
          :value="lang.languageName"
        />
      </datalist>

      <button
        class="btn btn-primary mb-3"
        @click="addLanguage"
        :disabled="!languageInput.trim()"
      >
        Add
      </button>

      <div class="selected-languages">
        <span
          class="badge badge-secondary mr-2 mb-2"
          v-for="languageId in memberLanguages"
          :key="languageId.languageId" 
        >
          {{ getLanguageName(languageId.languageId) }}
          <i
            class="fa fa-times ml-1 cursor-pointer"
            @click="removeLanguageById(languageId.languageId)"
          ></i>
        </span>
      </div>

    </div>
  </div>


                      </div>  



                </div>
              </div>
          
          </div>
        </sym-tab>

        <!-- Kin -->
        <sym-tab title="Kin" icon="users" class="scroller-tabs">
          <div>
            <div class="text-center border-light curved p-1 slategray mt-2">
              <span class="serif lg-3">Kins</span>
            </div>
            <div class="kin-scroller">
              <table class="light striped-even mb-0 kin-tb">
                <thead>
                  <tr>
                    <th class="w-20">Last Name</th>
                    <th class="w-20">First Name</th>
                    <th class="w-20">Middle Name</th>
                    <th class="w-10">Suffix</th>

                    <th class="w-8">Relation</th>
                    <th class="w-12">Emergency Contact</th>
                    <th class="w-8">Phone</th>
                    <th class="w-20">Email</th>
                    <th class="w-13">Occupation</th>
                    <th class="w-10">File Name</th>
                    <th class="w-20">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in kins" :key="index">

                    <td>{{ dtl.kinLastName }}</td>
                    <td>{{ dtl.kinFirstName }}</td>
                    <td>{{ dtl.kinMiddleName }}</td>
                    <td>{{ dtl.kinSuffixName }}</td>

                    <td>{{ dtl.relationName }}</td>
              
                <td data-label="Emergency" class="text-center">
            <i :class="getBooleanIconClass(dtl.emergencyContactFlag)"></i>
          </td>
                    <td>{{ dtl.kinPhoneNumber }}</td>
                    <td>{{ dtl.kinEmail }}</td>
                    <td>{{ dtl.kinOccupation }}</td>
                    <td>{{ dtl.kinFileName }}</td>

                    <td class="p-1">
                      <div class="kin-btns" sm-1 mb-0>
                        <div v-if="dtl.kinFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>

                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditKin(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteKin(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddKin" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(6)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>
        </sym-tab>

        <!-- Education & Qualification -->
        <sym-tab title="Educations & Qualifications" icon="mortar-board" class="scroller-tabs">
          <div>
            <div class="text-left border-light curved p-2 slategray mt-2">
              <span class="serif lg-3">Educations</span>
            </div>

            <div class="education-scroller">
              <table class="light striped-even mb-0 edu-tb">
                <thead>
                  <tr>
                    <th class="w-10">Education Name</th>
                    <th class="w-15">School Name</th>
                    <th class="w-15">Course Name</th>
                    <th class="w-3">Start Year</th>
                    <th class="w-3">End Year</th>
                    <th class="w-5">File Name</th>
                    <th class="w-10">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in educations" :key="index">

                    <td>{{ dtl.educationLevelName }}</td>
                    <td>{{ dtl.schoolName }}</td>
                    <td>{{ dtl.courseName }}</td>
                    <td>{{ dtl.eduStartYear }}</td>
                    <td>{{ dtl.eduEndYear }}</td>
                    <td>{{ dtl.eduFileName }}</td>

                    <td class="p-1">
                      <div class="edu-btns" sm-1 mb-0>
                        <div v-if="dtl.eduFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>

                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditEducation(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteEducation(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddEducation" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(8)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

            <!--License Profession  -->

          <div>
            <div class="text-left border-light curved p-2 slategray mt-2">
              <span class="serif lg-3">License Profession</span>
            </div>

            <div class="license-scroller">
              <table class="light striped-even mb-0 license-tb">
                <thead>
                  <tr>
                    <th class="w-30">License Name</th>
                    <th class="w-5">PRC ID</th>
                    <th class="w-5">File Name</th>
                    <th class="w-8">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in licenses" :key="index">

                    <td>{{ dtl.licenseProfessionName }}</td>
                    <td>{{ dtl.prcIdNo }}</td>
                    <td>{{ dtl.licenseFileName }}</td>

                    <td class="p-1">
                      <div class="license-btns" sm-1 mb-0>
                        <div v-if="dtl.licenseFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>
                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditLicense(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteLicense(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddLicense" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(9)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

            <!-- NCII   -->
            <div>
            <div class="text-left border-light curved p-2 slategray mt-2">
              <span class="serif lg-3">NCII Qualification Title</span>
            </div>

            <div class="ncii-scroller">
              <table class="light striped-even mb-0 ncii-tb">
                <thead>
                  <tr>
                    <th class="w-20">NCII Title</th>
                    <th class="w-10">Certificate Number</th>
                    <th class="w-10">Issuance Date</th>
                    <th class="w-10">Validity Date</th>
                    <th class="w-20">Training Institution</th>
                    <th class="w-20">Assessment Center</th>
                    <th class="w-10">File Name</th>
                    <th class="w-15">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in nciis" :key="index">

                    <td>{{ dtl.nciiQualificationTitleName }}</td>
                    <td>{{ dtl.certificateNumber }}</td>
                    <td>{{ dtl.issuanceDate }}</td>
                    <td>{{ dtl.validityDate }}</td>
                    <td>{{ dtl.trainingInstitutionName }}</td>
                    <td>{{ dtl.assessmentCenterName }}</td>

                    <td>{{ dtl.nciiFileName }}</td>

                    <td class="p-1">
                      <div class="ncii-btns" sm-1 mb-0>
                        <div v-if="dtl.nciiFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>
                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditNCII(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteNCII(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddNCII" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(11)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

            <!-- Compliance  -->
            <div>
            <div class="text-left border-light curved p-2 slategray mt-2">
              <span class="serif lg-3">Compliance Training</span>
            </div>

            <div class="license-scroller">
              <table class="light striped-even mb-0 kin-tb">
                <thead>
                  <tr>
                    <th class="w-20">Compliance Name</th>
                    <th class="w-10">Certificate Number</th>
                    <th class="w-10">Issuance Date</th>
                    <th class="w-10">Validity Date</th>
                    <th class="w-20">Training Institution</th>
                    <th class="w-20">Assessment Center</th>
                    <th class="w-10">File Name</th>
                    <th class="w-15">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in compliances" :key="index">

                    <td>{{ dtl.complianceTrainingName }}</td>
                    <td>{{ dtl.certificateNumber }}</td>
                    <td>{{ dtl.issuanceDate }}</td>
                    <td>{{ dtl.validityDate }}</td>
                    <td>{{ dtl.trainingInstitutionName }}</td>
                    <td>{{ dtl.assessmentCenterName }}</td>
                    <td>{{ dtl.complianceFileName }}</td>

                    <td class="p-1">
                      <div class="compliance-btns" sm-1 mb-0>
                        <div v-if="dtl.complianceFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>
                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditCompliance(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteCompliance(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddCompliance" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(12)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>



          <!-- CDA Training  -->
            <div>
            <div class="text-left border-light curved p-2 slategray mt-2">
              <span class="serif lg-3 ">CDA Training</span>
            </div>

            <div class="cda-scroller">
              <table class="light striped-even mb-0 cda-tb">
                <thead>
                  <tr>
                    <th class="w-20">CDA Training Name</th>
                    <th class="w-10">Certificate Number</th>
                    <th class="w-5">Training Date</th>
                    <th class="w-5">File Name</th>
                    <th class="w-7">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in cdaTrainings" :key="index">

                    <td>{{ dtl.cdaTrainingName }}</td>
                    <td>{{ dtl.certificateNumber }}</td>
                    <td>{{ dtl.trainingDate }}</td>
                    <td>{{ dtl.cdaFileName }}</td>

                    <td class="p-1">
                      <div class="cda-btns" sm-1 mb-0>
                        <div v-if="dtl.cdaFileName">
                          <a class="button success sm-4 " :href="dtl.fileUrl" download><i class="fa fa-download"></i ><span  class="button-caption ml-2" >Download</span></a>
                        </div>
                         <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditCDATraining(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button"  class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteCDATraining(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddCDATraining" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(10)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>



        </sym-tab>

        <!-- Work Experience -->
        <sym-tab title="Work Experience" icon="suitcase" class="scroller-tabs">
          <div>
            <div class="text-left border-light curved p-1 slategray mt-2">
              <span class="serif lg-3 ">Work</span>
            </div>
            <div class="work-scroller">
              <table class="light striped-even mb-0 work-tb">
                <thead>
                  <tr>
                    <th class="w-30">Position</th>
                    <th class="w-30">Company</th>
                    <th class="w-15">Municipality</th>
                    <th class="w-10">Contact Number</th>
                    <th class="w-10">Start Date</th>
                    <th class="w-10">End Date</th>
                    <th class="w-30">Reason For Leaving</th>
                    <th class="w-10">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in works" :key="index">

                    <td>{{ dtl.workPosition }}</td>
                    <td>{{ dtl.companyName }}</td>
                    <td>{{ dtl.municipalityName }}</td>
                    <td>{{ dtl.companyPhoneNumber }}</td>
                    <td>{{ dtl.workStartDate }}</td>
                    <td>{{ dtl.workEndDate }}</td>
                    <td>{{ dtl.reasonForLeaving }}</td>

                    <td class="p-1">
                      <div class="work-btns" sm-1 mb-0>
                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditWork(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button" class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteWork(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddWork" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(13)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

          <!-- Affiliation -->
          <div>
            <div class="text-left border-light curved p-1 slategray mt-2">
              <span class="serif lg-3">Affiliation</span>
            </div>
            <div class="affiliation-scroller">
              <table class="light striped-even mb-0 affiliation-tb">
                <thead>
                  <tr>
                    <th class="w-20">Affiliation</th>
                    <th class="w-5">Membership Date</th>
                    <th class="w-15">Position</th>
                    <th class="w-15">Description</th>
                    <th class="w-5">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in affiliations" :key="index">

                    <td>{{ dtl.affiliationName }}</td>
                    <td>{{ dtl.affiliationDate }}</td>
                    <td>{{ dtl.affiliationPosition }}</td>
                    <td>{{ dtl.affiliationDescription }}</td>

                    <td class="p-1">
                      <div class="affiliation-btns " sm-1 mb-0>
                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditAffiliation(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button" class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteAffiliation(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddAffiliation" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(14)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>


        </sym-tab>

        <!-- Skill Set -->
        <sym-tab title="Skill Set" icon="gavel" class="scroller-tabs">
          <div>
            <div class="text-left border-light curved p-1 slategray mt-2">
              <span class="serif lg-3">Skill Set</span>
            </div>
            <div class="skill-scroller">
              <table class="light striped-even mb-0 skill-tb">
                <thead>
                  <tr>
                    <th class="w-20">Skill Name</th>
                    <th class="w-10">Remarks</th>
                    <th class="w-5">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in skillSets" :key="index">
                    <td>{{ dtl.skillName }}</td>
                    <td>{{ dtl.remarks }}</td>

                    <td class="p-1">
                      <div class="skill-btns" sm-1 mb-0>
                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditSkillSet(dtl, index)"><i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteSkillSet(index)"><i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="command-buttons light border-main p-1 border-top-0 mb-2" >
              <div></div>

              <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
                <button type="button" class="justify-between btn-add" @click="onAddSkillSet"><i class="fa fa-plus-circle fa-lg"></i><span>Add</span> </button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(5)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>

        </sym-tab>

        <!-- Doc -->
        <sym-tab title="Documents" icon="book" class="scroller-tabs">
          <div>
            <div class="text-left border-light curved p-1 slategray mt-2">
              <span class="serif lg-3">Documents</span>
            </div>
            <div class="doc-scroller">
              <table class="light striped-even mb-0 doc-tb">
                <thead>
                  <tr>
                    <th class="w-20">Doc Type</th>
                    <th class="w-10">Expiration Date</th>
                    <th class="w-10">Reference</th>
                    <th class="w-5">File Name</th>
                    <th class="w-10">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in docs" :key="index">

                    <td>{{ dtl.docTypeName }}</td>
                    <td>{{ dtl.expirationDate }}</td>
                    <td>{{ dtl.docTypeReference }}</td>
                    <td>{{ dtl.docTypeFileName }}</td>

                    <td class="p-1">
                      <div class="docs-btns" sm-1 mb-0>
                        <button class="button info sm-4 ml-2" @click="openDocPreview(dtl.fileUrl, dtl.docTypeFileName)"><i class="fa fa-image mr-2"></i><span class="button-caption ml-2">Preview</span>  </button>
                        <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDoc(dtl, index)" > <i class="fa fa-edit fa-lg"></i><span>Edit</span> </button>
                        <button type="button" class="danger-light btn-dtl-delete" title="Delete"  @click="onDeleteDoc(index)" > <i class="fa fa-times fa-lg"></i> </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>

              <div
                class="buttons justify-center" :info-light="isMono" :outline="isMono"  border-main fw-30 mb-0 sm-1 shadow-light >
                <button type="button" class="justify-between btn-add" @click="onAddDoc" > <i class="fa fa-plus-circle fa-lg"></i><span>Add</span></button>
                <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog(7)" > <i class="fa fa-database fa-lg"></i><span>Log</span> </button>
              </div>
            </div>
          </div>
        </sym-tab>

        <!-- Pooling -->
        <sym-tab title="Pooling History" icon="history" class="scroller-tabs">
          <div>
            <div class="text-left border-light curved p-1 slategray mt-2">
              <span class="serif lg-3">Member Request Form</span>
            </div>
            <div class="doc-scroller">
              <table class="table-scroll light striped-even mb-0 doc-tb">
                <thead>
                  <tr>
                    <th class="w-10">MRF #</th>
                    <th class="w-25">Org</th>
                    <th class="w-25">Platform</th>
                    <th class="w-10">Applicant Status</th>
                    <th class="w-10">Hired Date</th>
                    <th class="w-10">Employment Status</th>
                    <th class="w-10">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(dtl, index) in pooling" :key="index">

                    <td class="memberid" @click="onClickMRF(dtl.memberRequestId)">{{ dtl.memberRequestId }}</td>
                    <td>{{ dtl.orgName }}</td>
                    <td>{{ dtl.platformName }}</td>
                    <td>{{ dtl.applicantStatusName }}</td>
                    <td>{{ dtl.hiredDate }}</td>
                    <td>{{ dtl.employmentStatusName }}</td>

                    <td class="p-1">
                      <div class="log-btn " sm-1 mb-0>
                          <button type="button" :class="logButtonClass" class="justify-between btn-log w-40"   @click="onViewPoolingLog(index)">
                                <i class="fa fa-database fa-lg"></i><span>Log</span> 
                              </button>

                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="command-buttons light border-main p-1 border-top-0 mb-2">
              <div></div>
            </div>
          </div>
        </sym-tab>



      </sym-tabs>

    </sym-form>
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
        <div class="board p-1 mb-0 w-100" @click.self="closeDocPreview">
          <form id="hrs0010E" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
            <div class="modal-content detail-editor-boxes">
              <div class="modal-content detail-editor-boxes">
                <template v-if="isPDF">
                  <iframe :src="previewFileUrl" width="100%" height="600px"></iframe>
                </template>

                <template v-else>
                  <div class="image-container">
                    <img :src="previewFileUrl" :style="{ transform: 'scale(' + imageZoom + ')' }" />
                  </div>
                </template>
              </div>
            </div>

            <div class="buttons w-100 d-flex gap justify-end mt-3 mb-2 fw-26 shadow-soft mb-0"> 
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
<!-- Upload File -->
  <sym-upload
      ref="uploader"
      class="d-none  border-main curved-1 p-1 shadow-light mb-3 "
      inputClass=" shadow"
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
      size="sm"
    >
  </sym-upload>

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
        title="Member Profile Change Log"
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

<!-- MODALS -->

<!-- Rnr -->
<sym-modal
  id="rnr-editor"
  v-model="isRnrRecordingEditorVisible"
  size="md"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorRnrTitle"
  @show="onShowRnrRecordingEditor($event)"
  @hide="onHideRnrRecordingEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="hrs0010M" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="rnr-editor-boxes">
        <sym-combo v-model="rnrRecording.rnrRecordingId" caption="Rnr Recording" align="bottom" display-field="rnrRecordingName" :datasource="recordings" @changing="onRnrRecordingIdChanging"></sym-combo>
        <sym-combo v-model="rnrRecording.rnrNumber" caption="Rnr Number" align="bottom" display-field="rnrNumber" :datasource="rnrNumbers" @changing="onRnrNumberIdChanging"></sym-combo>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitRnrRecording()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isRnrRecordingEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>
  </div>

  </sym-modal>



<!-- Vaccine -->
  <sym-modal
  id="vaccine-editor"
  v-model="isVaccineEditorVisible"
  size="rg"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :title="editorVaccineTitle"
  @show="onShowVaccineEditor($event)"
  @hide="onHideVaccineEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

  <div class="board p-1 mb-0 w-100">
    <form id="hrs0010A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
      <div class="vaccine-editor-boxes">
        <sym-combo v-model="vaccine.vaccineTypeId" caption="Type" align="bottom" display-field="vaccineTypeName" :datasource="vaccineType" ></sym-combo>
        <sym-text v-model="vaccine.vaccineName" caption="Name" align="bottom"></sym-text>
        <sym-date v-model="vaccine.vaccineDate" caption="Date" align="bottom"></sym-date>
      </div>

      <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
        <button type="button" class="info justify-between border-main" @click="onSubmitVaccine()"><i class="fa fa-check mr-2"></i>Submit</button>
        <button type="button" class="warning justify-between border-main mr-0" @click="isVaccineEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
      </div>

    </form>
  </div>

  </sym-modal>

  <!-- Medical -->
  <sym-modal
      id="medical-editor"
      v-model="isMedicalEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorMedicalTitle"
      @show="onShowMedicalEditor($event)"
      @hide="onHideMedicalEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010L" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="medical.medicalResultTypeId" caption="Medical Result Type" align="bottom" display-field="medicalResultTypeName" :datasource="medicalResultType" @changed="onMedicalResultTypeIdChanged(medical.medicalResultTypeId)" @changing="onMedicalResultTypeIdChanging" ></sym-combo>
          <div class="medical-fields" v-show="medical.medicalResultTypeId != 0">
             <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()">
            <i class="fa fa-upload mr-2"></i> Upload
            </button>
             </div>
             </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >



              <button type="button"  class="info justify-between border-main" @click="onSubmitMedical()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isMedicalEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>


  <!-- Kin -->
  <sym-modal
      id="kin-editor"
      v-model="isKinEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorKinTitle"
      @show="onShowKinEditor($event)"
      @hide="onHideKinEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010C" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="kin-editor-boxes">
            <sym-text v-model="kin.kinLastName" caption="Last Name" align="bottom" ></sym-text>
            <sym-text v-model="kin.kinFirstName" caption="First Name" align="bottom" ></sym-text>
            <sym-text v-model="kin.kinMiddleName" caption="Middle Name" align="bottom" ></sym-text>
            <sym-combo v-model="kin.kinSuffixId" caption="Suffix" align="bottom" display-field="kinSuffixName" :datasource="kinSuffix"></sym-combo>


            <sym-combo v-model="kin.relationId" caption="Relation" align="bottom" display-field="relationName" :datasource="relation" @changed="onRelationIdChanged(kin.relationId)"></sym-combo>
            <sym-check v-model="kin.emergencyContactFlag" caption="Emergency Contact" align="bottom" :caption-width="0" caption-align="center" check-align="center"></sym-check>
            <div class="MobileContainer">
                  <span class="MobileHeader">Contact Number</span>
                  <input class="MobilePhone"  v-model="kin.kinPhoneNumber" @input="validateKinContactNumber" type="mobile"   :style="{
                    borderColor: isValidKinContactNumber == 'valid' ? 'green' : isValidKinContactNumber == 'invalid' ? '' : '', }" />
            </div>
            <sym-text v-model="kin.kinEmail" caption="Email" align="bottom" @changing="onEmailChanging"></sym-text>
            <sym-text v-model="kin.kinOccupation" caption="Occupation" align="bottom" ></sym-text>
          
          </div>
            <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>
          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >

            <button type="button"  class="info justify-between border-main" @click="onSubmitKin()"> <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isKinEditorVisible = false" ><i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- Education -->
  <sym-modal
      id="education-editor"
      v-model="isEducationEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorEducationTitle"
      @show="onShowEducationEditor($event)"
      @hide="onHideEducationEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010E" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="education.educationLevelId" caption="Education" align="bottom" display-field="educationLevelName" :datasource="educationLevel" @changed="onEducationLevelIdChanged(education.educationLevelId)"></sym-combo>
             <div class="educYears" v-show="education.educationLevelId != 0">
            <sym-text v-model="education.schoolName" caption="School" align="bottom"></sym-text>
            <sym-text v-model="education.courseName" caption="Course" align="bottom"></sym-text>
            <div class="dates">
            <sym-int v-model="education.eduStartYear" caption="Start" align="bottom" @changing="onEduStartYearChanging"></sym-int>
            <sym-int v-model="education.eduEndYear" caption="End" align="bottom" @changing="onEduEndYearChanging"></sym-int>
            </div>
            <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>
            </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitEducation()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isEducationEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- License -->
  <sym-modal
      id="license-editor"
      v-model="isLicenseEditorVisible"
      size="md"
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
        <form id="hrs0010F" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="license-editor-boxes">
            <sym-combo v-model="license.licenseProfessionId" caption="License" align="bottom" display-field="licenseProfessionName" :datasource="licenseProfession" @changed="onLicenseProfessionIdChanged(license.licenseProfessionId)"></sym-combo>
            <div class="license-fields" v-show="license.licenseProfessionId != 0">


            <sym-text v-model="license.prcIdNo" caption="PRC No" align="bottom"></sym-text>
           <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>
            </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitLicense()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isLicenseEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- NCII -->
  <sym-modal
      id="ncii-editor"
      v-model="isNCIIEditorVisible"
      size="md"
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
        <form id="hrs0010H" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="ncii.nciiQualificationTitleId" caption="NCII Title" align="bottom" display-field="nciiQualificationTitleName" :datasource="nciiTitle" @changed="onNCIIQualificationTitleIdChanged(ncii.nciiQualificationTitleId)"></sym-combo>
            <div class="ncii-fields" v-show="ncii.nciiQualificationTitleId != 0">

            <sym-text v-model="ncii.certificateNumber" caption="Certificate Number" align="bottom"></sym-text>
            <div class="dates">
            <sym-date v-model="ncii.issuanceDate" caption="Issuance Date" align="bottom" @changing ="onNciiIssuanceDateChanging"></sym-date>
            <sym-date v-model="ncii.validityDate" caption="Validity Date" align="bottom" @changing ="onNciiValidityDateChanging" ></sym-date>

            </div>
            <div class="d-grid gap">
              <input v-model="searchTraining" type="text" placeholder="Search Training Institution" class="input-text" list="trainingInstitutionName" @change="onTrainingInstitutionSelected" />
              <datalist id="trainingInstitutionName"><option v-for="item in trainingInstitution" :key="item.trainingInstitutionId" :value="item.trainingInstitutionName" ></option></datalist>
              <sym-text v-model="ncii.trainingInstitutionName" caption="Training Institution" align="bottom"></sym-text>
            </div>
            <div class="d-grid gap">
              <input v-model="searchAssessment" type="text" placeholder="Search Assessment Center" class="input-text" list="assessmentCenterName" @change="onAssementCenterSelected" />
              <datalist id="assessmentCenterName"><option v-for="item in assessmentCenter" :key="item.assessmentCenterId" :value="item.assessmentCenterName" ></option></datalist>
              <sym-text v-model="ncii.assessmentCenterName" caption="Assessment Center" align="bottom"></sym-text>
            </div>
            <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>

            </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitNCII()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isNCIIEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- Compliance -->
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
        <form id="hrs0010I" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="compliance.complianceTrainingId" caption="Compliance Name" align="bottom" display-field="complianceTrainingName" :datasource="complianceTraining" @changed="onComplianceTrainingIdChanged(compliance.complianceTrainingId)"></sym-combo>
            <div class="complaince-fie" v-show="compliance.complianceTrainingId != 0">
            <sym-text v-model="compliance.certificateNumber" caption="Certificate Number" align="bottom"></sym-text>
            <div class="dates">
            <sym-date v-model="compliance.issuanceDate" caption="Issuance Date" align="bottom" @changing ="onComplianceIssuanceDateChanging"></sym-date>
            <sym-date v-model="compliance.validityDate" caption="Validity Date" align="bottom" @changing ="onComplianceValidityDateChanging"></sym-date>

            </div>

            <sym-combo v-model="compliance.trainingInstitutionId" caption="Training Institution" align="bottom" display-field="trainingInstitutionName" :datasource="trainingInstitution"></sym-combo>

            <sym-combo v-model="compliance.assessmentCenterId" caption="Assessment Center" align="bottom" display-field="assessmentCenterName" :datasource="assessmentCenter"></sym-combo>
            <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>
            </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitCompliance()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isComplianceEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- CDA Training -->
  <sym-modal
      id="cdaTraining-editor"
      v-model="isCDATrainingEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorCDATrainingTitle"
      @show="onShowCDATrainingEditor($event)"
      @hide="onHideCDATrainingEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010G" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="cdaTraining.cdaTrainingId" caption="CDA Training Name" align="bottom" display-field="cdaTrainingName" :datasource="cdaTrainingType" @changed="onCDATrainingIdChanged(cdaTraining.cdaTrainingId)"></sym-combo>
            <div class="cda-fields" v-show="cdaTraining.cdaTrainingId != 0">
            <sym-text v-model="cdaTraining.certificateNumber" caption="Certificate Number" align="bottom"></sym-text>
            <sym-date v-model="cdaTraining.trainingDate" caption="Training Date" align="bottom" @changing ="onCDATrainingDateChanging" ></sym-date>

            <div class="upload-files">
            <sym-tag class="upload-text">{{ fileName }}</sym-tag>
            <button type="button" class="info justify-between border-main" @click="onSelectFile()"> <i class="fa fa-upload mr-2"></i> Upload </button>
            </div>
            </div>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitCDATraining()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isCDATrainingEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- Work -->
  <sym-modal
      id="work-editor"
      v-model="isWorkEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorWorkTitle"
      @show="onShowWorkEditor($event)"
      @hide="onHideWorkEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010J" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-text v-model="work.workPosition" caption="Position" align="bottom"></sym-text>
            <sym-text v-model="work.companyName" caption="Company Name" align="bottom"></sym-text>
            <sym-text v-model="work.municipalityId" caption="Municipality" align="bottom" lookupId="DbsMunicipality" @changing="onMunicipalityIdChanging" @searchresult="onMunicipalityIdSearchResult"></sym-text>
            <sym-text v-model="work.municipalityName" caption="Municipality Name" align="bottom"></sym-text>
            <div class="MobileContainer">
                  <span class="MobileHeader">Contact Number</span>
                  <input class="MobilePhone"  v-model="work.companyPhoneNumber" @input="validateWorkContactNumber" type="mobile"   :style="{
                    borderColor: isValidWorkContactNumber == 'valid' ? 'green' : isValidWorkContactNumber == 'invalid' ? '' : '', }" />
            </div>
            <div class="dates">
            <sym-date v-model="work.workStartDate" caption="Start Date" align="bottom" @changing="onWorkStartDateChanging"></sym-date>
            <sym-date v-model="work.workEndDate" caption="End Date" align="bottom" @changing="onWorkEndDateChanging"></sym-date>
            </div>

            <sym-text v-model="work.reasonForLeaving" caption="Reason for Leaving" align="bottom"></sym-text>
          </div>

          <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitWork()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isWorkEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

    <!-- Affiliation -->
  <sym-modal
      id="affiliation-editor"
      v-model="isAffiliationEditorVisible"
      size="md"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorAffiliationTitle"
      @show="onShowAffiliationEditor($event)"
      @hide="onHideAffiliationEditor($event)"
      headerClass="app-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form id="hrs0010K" class="curved-bottom border-dark marianblue p-3" @submit.prevent >
          <div class="detail-editor-boxes">
            <sym-combo v-model="affiliation.affiliationId" caption="Affiliation" align="bottom" display-field="affiliationName" :datasource="affiliationName"></sym-combo>
            <div class="affiliation-fields" v-show="affiliation.affiliationId != 0">
            <sym-date v-model="affiliation.affiliationDate" caption="Membership Date" align="bottom" @changing="onAffiliationDateChanging"></sym-date>
            <sym-text v-model="affiliation.affiliationPosition" caption="Position" align="bottom"></sym-text>
            <sym-text v-model="affiliation.affiliationDescription" caption="Description" align="bottom"></sym-text>
            </div>
            </div>

            <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0 >
              <button type="button"  class="info justify-between border-main" @click="onSubmitAffiliation()">
              <i class="fa fa-check mr-2"></i>Submit</button>
            <button  type="button" class="warning justify-between border-main mr-0" @click="isAffiliationEditorVisible = false" >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close </button>
          </div>
        </form>
      </div>
  </sym-modal>

  <!-- Skill Set  -->
  <sym-modal
    id="skillSet-editor"
    v-model="isSkillSetEditorVisible"
    size="rg"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="false"
    :dismissible="false"
    :closeOnBackButton="false"
    :title="editorSkillSetTitle"
    @show="onShowSkillSetEditor($event)"
    @hide="onHideSkillSetEditor($event)"
    headerClass="app-form-header"
    dismissButtonClass="danger"
  >

    <div class="board p-1 mb-0 w-100">
      <form id="hrs0010B" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
        <div class="skill-editor-boxes">
          <sym-integer v-model="skillSet.skillDetailId" caption="Skill ID" align="bottom" lookupId="DbsSkillSetDetail" @changing="onSkillDetailIdChanging" @searchresult="onSkillDetailIdSearchResult"></sym-integer>
          <sym-text v-model="skillSet.skillName" caption="Skill Name" align="bottom"></sym-text>
          <sym-text v-model="skillSet.remarks" caption="Remarks" align="bottom"></sym-text>
        </div>

        <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
          <button type="button" class="info justify-between border-main" @click="onSubmitSkillSet(skillSet.skillDetailId)"><i class="fa fa-check mr-2"></i>Submit</button>
          <button type="button" class="warning justify-between border-main mr-0" @click="isSkillSetEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
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
              <div class="doc-container">
                <sym-combo v-model="doc.docTypeId" caption="Doc Type" align="bottom" display-field="docTypeName" :datasource="docType" @changing="onDocTypeIdChanging" @changed="onDocTypeIdChanged(doc.docTypeId)"></sym-combo>
              </div>
              
              <div class="doctype-fields" v-show="doc.docTypeId != 0">
                <div class="exp" v-if = "isUploadRequired">
                  <sym-date v-model="doc.expirationDate" caption="Expiration Date" align="bottom"></sym-date>
                </div>
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

</section>
</template>

<script>

import { DateTime } from "../../js/core";

import {
  get,
  upload, //PHOTO
  ajax,
} from "../../js/http";

import {
  getCount,
  getList,
  getSafeDeleteFlag,
} from "../../js/dbSys";

import PageMaintenance from "../PageMaintenance.vue";
import SymImageSelect from "../../comp/SymImageSelect.vue";
import SymInteger from '../../comp/SymInteger.vue';

export default {
  components: { SymImageSelect, SymInteger },
  extends: PageMaintenance,
  name: "hrs0010",

  data() {
    return {

      fieldEnable: true,
      member: {
        memberId: 0,
        memberBadgeId: "",
        memberEmployeeId: "",
        memberLastName: "",
        memberFirstName: "",
        memberMiddleName: "",
        memberSuffixId: 0,
        memberName: "",
        memberTypeId: 0,
        typeQualificationDetailId: 0,
        memberStatusId: 0,
        memberStatusName: "",
        birthDate: null,
        age: 0,
        birthRegionId: "",
        birthProvinceId: "",
        birthProvinceName: "",
        birthMunicipalityId: "",

        sexId: "",
        bloodTypeId: 0,
        height: "",
        weight: "",
        civilStatusId: 0,
        religionId: 0,
        address1: "",
        address2: "",
        postalCode: "",
        phoneNumber: "",
        mobileNumber: "",
        email: "",
        regionId: 0,
        provinceId: "",
        provinceName: "",
        municipalityId: "",
        barangayId: "",

        alternateProvinceId: "",
        alternateProvinceName: "",
        alternateMunicipalityId: "",
        alternateBarangayId: "",
        alternatePostalCode: "",

        facebook: "",
        instagram: "",
        alternateAddress1: "",
        alternateAddress2: "",
        cdaMemberTypeId: 0,
        cdaMemberTypeAmount: 0,
        abroadFlag: false,
        relocateFlag: false,
        weeklidayFlag: false,
        expectedSalary: 0,
        initialHiredDate: null,
        gCashNumber: "",
        bankId: 0,
        bankAccountNumber: "",
        recruiterId: 0,
        recruiterName: "",
        photo: null, //PHOTO
        memberCode: "", //PHOTO
        imageUrl: "", //PHOTO
        photoFileName: "",
        photoGUID: "",

        lockId: "",
      },

      provinces: [],
      municipalities: [],
      barangays: [],

      alternateProvinces: [],
      alternateMunicipalities: [],
      alternateBarangays: [],

      birthProvinces: [],
      birthMunicipalities: [],
      memberLanguages: {
        languageId: 0,
        languageName: '',
        lockId: "",
      },

      memberDisabilities: [],
      memberRecordings: [],

      oldMember: [],


      vaccines: [],

      vaccine: {
        vaccineDetailId: 0,
        vaccineTypeId: 0,
        vaccineTypeName: 0,
        vaccineName: "",
        vaccineDate: null,
      },

      vaccineIndex: -1,
      isAddingVaccine: false,
      isVaccineEditorVisible: false,

      skillSets: [],

      skillSet: {
        skillSetDetailId: 0,
        memberId: 0,
        skillDetailId: 0,
        skillName: "",
        remarks: "",
      },

      skillSetIndex: -1,
      isAddingSkillSet: false,
      isSkillSetEditorVisible: false,

      kins: [],

      kin: {
        kinDetailId: 0,
        memberId: 0,
        kinLastName: "",
        kinFirstName: "",
        kinMiddleName: "",
        kinSuffixId: 0,
        kinSuffixName: "",
        emergencyContactFlag: false,
        kinPhoneNumber: "",
        relationId: 0,
        relationName: "",
        kinOccupation: "",
        kinEmail: "",
        kinFileName: "",
        kinGUID: "",
        fileURL: "",
        lockId: "",
      },

      kinIndex: -1,
      isAddingKin: false,
      isKinEditorVisible: false,

      docs: [],

      doc: {
        docTypeDetailId: 0,
        memberId: 0,
        docTypeId: 0,
        docTypeName: "",
        expirationDate: null,
        docTypeReference: "",
        docTypeFileName: "",
        docTypeGUID: "",
        expirationFlag: false,
        fileURL: "",
        lockId: "",
      },

      docIndex: -1,
      isAddingDoc: false,
      isDocEditorVisible: false,

      educations: [],

      education: {
        eduDetailId: 0,
        memberId: 0,
        educationLevelId: 0,
        schoolName: "",
        courseName: "",
        eduStartYear: 0,
        eduEndYear: 0,
        eduFileName: "",
        eduGUID: "",
        fileURL: "",
        lockId: "",
      },

      educationIndex: -1,
      isAddingEducation: false,
      isEducationEditorVisible: false,

      licenses: [],

      license: {
        licenseProfessionDetailId: 0,
        memberId: 0,
        prcIdNo: "",
        licenseProfessionId: 0,
        licenseProfessionName: "",
        licenseFileName: "",
        licenseGUID: "",
        fileURL: "",
        lockId: "",
      },

      licenseIndex: -1,
      isAddingLicense: false,
      isLicenseEditorVisible: false,


      cdaTrainings: [],

      cdaTraining: {
        cdaTrainingDetailId: 0,
        memberId: 0,
        certificateNumber: "",
        cdaTrainingId: 0,
        cdaTrainingName: "",
        trainingDate: null,
        cdaFileName: "",
        cdaGUID: "",
        fileURL: "",
        lockId: "",
      },

      cdaTrainingIndex: -1,
      isAddingCDATraining: false,
      isCDATrainingEditorVisible: false,


      nciis: [],

      ncii: {
        nciiQualificationTitleDetailId: 0,
        memberId: 0,
        certificateNumber: "",
        nciiQualificationTitleId: 0,
        nciiQualificationTitleName: "",
        issuanceDate: null,
        validityDate: null,

        trainingInstitutionId: 0,
        trainingInstitutionName: "",

        assessmentCenterId: 0,
        assessmentCenterName: "",

        nciiFileName: "",
        nciiGUID: "",
        fileURL: "",
        lockId: "",
      },

      nciiIndex: -1,
      isAddingNCII: false,
      isNCIIEditorVisible: false,

      compliances: [],

      compliance: {
        complianceTrainingDetailId: 0,
        memberId: 0,
        certificateNumber: "",
        complianceTrainingId: 0,
        complianceTrainingName: "",
        issuanceDate: null,
        validityDate: null,

        trainingInstitutionId: 0,
        trainingInstitutionName: "",

        assessmentCenterId: 0,
        assessmentCenterName: "",

        complianceFileName: "",
        complianceGUID: "",
        fileURL: "",
        lockId: "",
      },

      complianceIndex: -1,
      isAddingCompliance: false,
      isComplianceEditorVisible: false,

      works: [],

      work: {
        workDetailId: 0,
        memberId: 0,
        workPosition: "",
        companyName: "",
        municipalityId: "",
        municipalityName: "",
        companyPhoneNumber: "",
        workStartDate: null,
        workEndDate: null,
        reasonForLeaving: "",

        workFileName: "",
        workGUID: "",
        fileURL: "",
        lockId: "",
      },

      workIndex: -1,
      isAddingWork: false,
      isWorkEditorVisible: false,


      affiliations: [],

      affiliation: {
        affiliationDetailId: 0,
        memberId: 0,
        affiliationId: 0,
        affiliationName: "",
        affiliationDate: null,
        affiliationPosition: "",
        affiliationDescription: "",
        affiliationFileName: "",
        affiliationGUID: "",
        fileURL: "",
        lockId: "",

        isPoolingFlag: false,

      },
      affiliationIndex: -1,
      isAddingAffiliation: false,
      isAffiliationEditorVisible: false,

      medicals: [],

      medical: {
        medicalResultTypeDetailId: 0,
        memberId: 0, 
        remarks: "",
        medicalResultTypeCode: "",
        medicalResultTypeId: 0,
        medicalResultTypeName: "",
        medicalResultTypeFileName: "",
        medicalResultTypeGUID: "",
        fileURL: "",
        lockId: "",
      },

      medicalIndex: -1,
      isAddingMedical: false,
      isMedicalEditorVisible: false,


      rnrRecordings: [],

      rnrRecording: {
        rnrRecordingDetailId: 0,
        rnrRecordingId: 0,
        rnrRecordingName: "",
        rnrNumberId : 0,
        rnrNumber : 0,
        lockId: "",
      },

      rnrRecordingIndex: -1,
      isAddingRnrRecording: false,
      isRnrRecordingEditorVisible: false,

      logs: [],
      isLogVisible: false,

      tsPhoto: 0, //PHOTO
      max: 4,
      isDisabled: true,
      activeTabIndex: 0,

      selectedFileList: [],
      documentNames: [],            // uploaded files
      docsUploadResult: null,
      catalogItems: [],

      fileName: "",
      isUploadRequired: false,
      pathFileName: "",
      guidReference: "",
      uploadPhotoTemp: false,
      pooling: [],
      trainingInstitution:[],
      assessmentCenter:[],
      searchName:'',
      searchTraining:'',
      searchAssessment: '',
      isDocPreviewVisible: false,
      previewFileUrl: '', 
      previewFileName: '',

      fullImageFileNames: [],
      fullscreenElement: null,
      imageZoom: .75,

      languageInput: '',
    };

  },

  mounted() {
    const me = this;
    me.isPoolingFlag = false;
    me.syncValues(me.params, me.query);
    me.replaceUrl();
  },

  methods: {

 addLanguage() {


    const input = this.languageInput.toLowerCase().trim();

    const lang = this.languages.find(
      l => l.languageName.toLowerCase() === input
    );
    if (lang && !this.memberLanguages.includes(lang.languageId)) {
      this.memberLanguages.push(lang.languageId); 
      this.memberLanguages.push(lang.languageName); 
    }

    this.languageInput = '';
  },

  removeLanguageById(id) {
    const index = this.memberLanguages.indexOf(id);
    if (index !== -1) {
      this.memberLanguages.splice(index, 1);
    }
  },

  getLanguageName(id) {
    const lang = this.languages.find(l => l.languageId === id);
    return lang ? lang.languageName : '(Unknown)';
  },

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

    onTrainingInstitutionSelected() {
      const selected = this.trainingInstitution.find(
        item => item.trainingInstitutionName === this.searchTraining
      );

      if (selected) {
        this.ncii.trainingInstitutionName = selected.trainingInstitutionName;
        this.ncii.trainingInstitutionId = selected.trainingInstitutionId;
      } else {
        this.ncii.trainingInstitutionName = this.searchTraining;
        this.ncii.trainingInstitutionId = null;
      }
    },

     onAssementCenterSelected() {
      const selected = this.assessmentCenter.find(
        item => item.assessmentCenterName === this.searchAssessment
      );

      if (selected) {
        this.ncii.assessmentCenterName = selected.assessmentCenterName;
        this.ncii.assessmentCenterId = selected.assessmentCenterId;
      } else {
        this.ncii.assessmentCenterName = this.searchAssessment;
        this.ncii.assessmentCenterId = null;
      }
    },
  


    cdaMemberTypeIdChanged () {
      const me = this;

      let o = me.cdaMemberTypes.find( o => o.cdaMemberTypeId == me.member.cdaMemberTypeId);
      if (o) {

        if (o.cdaMemberTypeAmount) {
          me.member.cdaMemberTypeAmount = o.cdaMemberTypeAmount
        }

      }
    },


    getTargetPath() {
      const me = this,
        q = {};

      if (me.member.memberId) {
        q.memberId = me.member.memberId;
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
      
      if ("memberId" in q && me.core.isInteger(q.memberId)) {
        me.member.memberId = parseInt(q.memberId);
        me.isPoolingFlag = true
     
      }

      if ("activeTabIndex" in q && me.core.isInteger(q.activeTabIndex)) {
        me.activeTabIndex = parseInt(q.activeTabIndex);
     
     
      }

     },
 
    onRnrNumberIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.rnrNumberIdCallback;
    },

    rnrNumberIdCallback(e) {
      const me = this;
      let total = 0
      me.rnrRecordings.forEach(dtl => {

      if (typeof Number(dtl.rnrNumber) === 'number' && !isNaN(dtl.rnrNumber)) {

        total += Number(dtl.rnrNumber);

      }
      });


      if (total + Number(e.proposedValue) > 1) {
          e.message = 'Rnr Recording Total cannot be greater than 1.';
          return false;
      }

      return true;
      },



    onRnrRecordingIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.rnrRecordingIdCallback;
    },

    rnrRecordingIdCallback(e) {
      const me = this;

      let index = me.rnrRecordings.findIndex(obj => obj.rnrRecordingId === e.proposedValue);
      if (index > -1) {

        const rnrRecordingName = me.rnrRecordings[index].rnrRecordingName;
        e.message = 'Rnr Recording <b>' + rnrRecordingName + '</b> is already in the list.';
        return false;
      }

      return true;
    },


    onShowRnrRecordingEditor () {
      const me = this;

      me.setActiveModel('rnrRecording');

      me.setRequiredMode(
        'rnrRecordingId',
        'rnrNumberId',
      );

      setTimeout(() => {
        this.setFocus('rnrRecordingId');
      }, 200);
    },

    onHideRnrRecordingEditor () {
      const me = this;

      me.isAddingRnrRecording = false;
      me.setActiveModel();
    },

    onEditRnrRecording (dtl, index) {

      const d = this.rnrRecording;
      this.rnrRecordingIndex = index;

      d.rnrRecordingDetailId = dtl.rnrRecordingDetailId;
      d.rnrRecordingId = dtl.rnrRecordingId;
      d.rnrNumberId = dtl.rnrNumberId;
      this.isRnrRecordingEditorVisible = true;
    },

    onAddRnrRecording () {
      const me = this;

      me.clearRnrRecordingPad();

      me.rnrRecording.rnrRecordingDetailId = -1

      me.isRnrRecordingEditorVisible = true;
      me.isAddingRnrRecording = true;

    },
    onSubmitRnrRecording() {
      const me = this,
        d = me.rnrRecording;

      if (!d.rnrRecordingId) {
        me.isRnrRecordingEditorVisible = false;
        return;
      }

      if (!me.isValid("hrs0010M")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }


      let dtl = {};

      if (me.isAddingRnrRecording) {

        Object.assign(dtl, d);

        me.recordings.forEach((recording) => {
          if (recording.rnrRecordingId == dtl.rnrRecordingId) {
            dtl.rnrRecordingName = recording.rnrRecordingName;
          }
        });

        me.rnrNumbers.forEach((rnrNumber) => {
          if (rnrNumber.rnrNumberId == dtl.rnrNumberId) {
            dtl.rnrNumber = rnrNumber.rnrNumber;
          }
        });

        me.rnrRecordings.push(dtl);

        me.clearRnrRecordingPad();

        me.advice.info("Rnr Recording '" + dtl.rnrRecordingName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("rnrRecordingId");

      } else {

        dtl = me.rnrRecordings[me.rnrRecordingIndex];

        me.recordings.forEach((dtl) => {
          if (dtl.rnrRecordingId == d.rnrRecordingId) {
            d.rnrRecordingName = dtl.rnrRecordingName;
          }
        });

        me.rnrNumbers.forEach((dtl) => {
          if (dtl.rnrNumberId == d.rnrNumberId) {
            d.rnrNumber = dtl.rnrNumber;
          }
        });


        dtl.rnrRecordingDetailId = d.rnrRecordingDetailId;
        dtl.rnrRecordingId = d.rnrRecordingId;
        dtl.rnrRecordingName = d.rnrRecordingName;
        dtl.rnrNumberId = d.rnrNumberId;
        dtl.rnrNumber = d.rnrNumber;
        me.isRnrRecordingEditorVisible = false;

      }
    },


    onDeleteRnrRecording(index) {
      const me = this;

      me.dialog.confirm( "Rnr Recording <b>" + me.rnrRecordings[index].rnrRecordingName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.rnrRecordings.splice(index, 1);
          }
          return;
        });
    },


    clearRnrRecordingPad () {
      const d = this.rnrRecording;

      d.rnrRecordingDetailId = 0;
      d.rnrRecordingId = 0;
      d.rnrRecordingName = '';
      d.rnrNumberId = 0;
      d.rnrNumber = 0;

    },

    // STOP NA

    onAffiliationDateChanging (e) {
    e.message = 'Entry rejected.';
    e.callback = this.onAffiliationDateCallback;
    },

    onAffiliationDateCallback (e) {
      const me = this;

      if (e.proposedValue <= this.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Membership Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      return true;
    },



    onWorkStartDateChanging (e) {
    e.message = 'Entry rejected.';
    e.callback = this.onWorkStartDateCallback;
    },

    onWorkStartDateCallback (e) {
      const me = this;

      if (e.proposedValue <= this.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Invalid Start Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (me.work.workEndDate && e.proposedValue >= me.work.workEndDate) {
        e.message = 'Start Date must be earlier than End Date.';
        return false;
      }

      return true;
    },

    onWorkEndDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onWorkEndDateCallback;
    },

    onWorkEndDateCallback (e) {
      const me = this;

      if (e.proposedValue <= me.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Start Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (e.proposedValue <= me.work.workStartDate) {
        e.message = 'End Date must be later than Start Date.';
        return false;
      }

      return true;
    },

    onCDATrainingDateChanging (e) {
    e.message = 'Entry rejected.';
    e.callback = this.onCDATrainingDateCallback;
    },

    onCDATrainingDateCallback (e) {
      const me = this;

      if (e.proposedValue <= this.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Invalid Training Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      return true;
    },


    onComplianceIssuanceDateChanging (e) {
    e.message = 'Entry rejected.';
    e.callback = this.onComplianceIssuanceDateCallback;
    },

    onComplianceIssuanceDateCallback (e) {
      const me = this;

      if (e.proposedValue <= this.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Invalid Issuance Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (me.compliance.validityDate && e.proposedValue >= me.compliance.validityDate) {
        e.message = 'Issuance Date must be earlier than Validity Date.';
        return false;
      }

      return true;
    },

    onComplianceValidityDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onComplianceValidityDateCallback;
    },

    onComplianceValidityDateCallback (e) {
      const me = this;

      if (e.proposedValue <= me.member.birthDate || e.proposedValue > me.today) {
        e.message = 'Invalid Validity Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (e.proposedValue <= me.compliance.issuanceDate) {
        e.message = 'Validity Date must be later than Issuance Date.';
        return false;
      }

      return true;
    },





      onNciiIssuanceDateChanging (e) {
        e.message = 'Entry rejected.';
        e.callback = this.onNciiIssuanceDateCallback;
        },

      onNciiIssuanceDateCallback (e) {
        const me = this;

        if (e.proposedValue <= this.member.birthDate || e.proposedValue > me.today) {
          e.message = 'Invalid Issuance Date <b>[' + e.proposedValue + ']</b>.';
          return false;
        }
        if (me.ncii.validityDate && e.proposedValue >= me.ncii.validityDate) {
          e.message = 'Issuance Date must be earlier than Validity Date.';
          return false;
        }

        return true;
      },

    onInitialHiredDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onInitialHiredDateCallback;
    },

    onInitialHiredDateCallback (e) {
      const me = this;

      if (e.proposedValue > me.today) {
        e.message = 'Invalid Initial Hired Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      return true;
    },

    onNciiValidityDateChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.onNciiValidityDateCallback;
    },

    onNciiValidityDateCallback (e) {
      const me = this;

      if (e.proposedValue <= me.member.birthDate ) {
        e.message = 'Invalid Validity Date <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (e.proposedValue <= me.ncii.issuanceDate) {
        e.message = 'Validity Date must be later than Issuance Date.';
        return false;
      }

      return true;
    },
    onEduStartYearChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.eduStartYearCallback;
    },

    eduStartYearCallback (e) {
      const me = this;
      const today = new Date();

      if (e.proposedValue <= me.member.birthDate.year || e.proposedValue > today.getFullYear()) {
        e.message = 'Invalid Start Year <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (me.education.eduEndYear && e.proposedValue > me.education.eduEndYear) {
        e.message = 'Start Year must be less than End Year.';
        return false;
      }

      return true;
    },

    onEduEndYearChanging (e) {
      e.message = 'Entry rejected.';
      e.callback = this.eduEndYearCallback;
    },

    eduEndYearCallback (e) {
      const me = this;
      const today = new Date();

      if (e.proposedValue <= me.member.birthDate.year || e.proposedValue > today.getFullYear()) {
        e.message = 'Invalid End Year <b>[' + e.proposedValue + ']</b>.';
        return false;
      }

      if (e.proposedValue <= me.education.eduStartYear) {
        e.message = 'End Year must be greater than Start Year.';
        return false;
      }

      return true;
    },

    onTrainingYearChanging (e) {

        if (e.proposedValue instanceof DateTime) {
        let me = this,
          p = e.proposedValue;
          let message ="Invalid Tranining year.";

        if (p.year <= me.member.birthDate.year ) {
          e.preventDefault();
          me.clearCDATrainingDate();
          me.advice.fault(message, { duration: 5 });
          return;
        }

      }
    },


    onBarangayIdChanged (newValue) {
      const me = this;
      let o = me.barangays.find( o => o.barangayId == newValue);
      if (o) {
        this.member.postalCode =  o.postalCode
      }
    },

    onAlternateBarangayIdChanged (newValue) {
      const me = this;
      let o = me.alternateBarangays.find( o => o.alternateBarangayId == newValue);
      if (o) {
        this.member.alternatePostalCode =  o.postalCode
      }
    },


    onMedicalResultTypeIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.medicalResultTypeIdCallback;
    },

    medicalResultTypeIdCallback(e) {
      const me = this;
      let index = me.medicals.findIndex(obj => obj.medicalResultTypeId == e.proposedValue);
      if (index > -1) {

        const medicalResultTypeName = me.medicals[index].medicalResultTypeName;
        e.message = 'Medical Result Type <b>' + medicalResultTypeName + '</b> is already in the list.';
        return false;
      }

      return true;
    },

    onMedicalResultTypeIdChanged (newValue) {
      const me = this;

      let o = me.medicalResultType.find( o => o.medicalResultTypeId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

      }
    },
    onMedicalResultTypeCodeChanged (newValue) {
      const me = this;
      let o = me.medicalResultType.find( o => o.medicalResultTypeCode == newValue);
      if (o) {
        me.medical.medicalResultTypeName = o.medicalResultTypeName
        me.medical.medicalResultTypeId = o.medicalResultTypeId
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false      
        }

      }
    },
    onSubmitMedical() {
      const me = this,
        d = me.medical;

      if (!d.medicalResultTypeId) {
        me.isMedicalEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }
  
      if (!me.isValid("hrs0010L")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingMedical) {
        Object.assign(dtl, d);

        me.medicals.push(dtl);

        me.medicalResultType.forEach((medicalResultType) => {
          if (medicalResultType.medicalResultTypeId == dtl.medicalResultTypeId) {
            dtl.medicalResultTypeName = medicalResultType.medicalResultTypeName;
          }
        });
        
        dtl.medicalResultTypeFileName = this.fileName;
        dtl.medicalResultTypeGUID = this.guidReference;

        me.clearMedicalPad();

        me.advice.info("Medical Result Type '" + dtl.medicalResultTypeName + "' added to list.", {
          duration: 5,
        });
        
        me.setFocus("medicalResultTypeId");
        
      } else {
         
        dtl = me.medicals[me.medicalIndex];
          me.medicalResultType.forEach((dtl) => {
          
          if (dtl.medicalResultTypeId == d.medicalResultTypeId) {
            d.medicalResultTypeName = dtl.medicalResultTypeName;
          }
        });
          
        dtl.medicalResultTypeDetailId = d.medicalResultTypeDetailId;
        dtl.medicalResultTypeName = d.medicalResultTypeName;
        dtl.medicalResultTypeId = d.medicalResultTypeId;
        dtl.remarks = d.remarks;

        if (this.fileName) {
        dtl.medicalResultTypeFileName = this.fileName;
        } else {
        dtl.medicalResultTypeFileName = d.medicalResultTypeFileName;    
        }
        
        if (this.guidReference) {
        dtl.medicalResultTypeGUID = this.guidReference;
        } else {
        dtl.medicalResultTypeGUID = d.medicalResultTypeGUID;    
        }

        me.isMedicalEditorVisible = false;
        
      }
    },

    onShowMedicalEditor() {
      const me = this;

      me.setActiveModel("medical");

      me.setRequiredMode(
        'medicalResultTypeId',
        'remarks'
      );
      me.setDisplayMode(
        'medicalResultTypeName',
      );

      setTimeout(() => {
        this.setFocus("medicalResultTypeId");
      }, 200);

    },

    onHideMedicalEditor() {
      const me = this;

      me.isAddingMedical = false;
      me.setActiveModel();
    },

    onEditMedical(dtl, index) {
      const d = this.medical;
      this.medicalIndex = index;
    
      d.medicalResultTypeDetailId = dtl.medicalResultTypeDetailId;
      d.medicalResultTypeId = dtl.medicalResultTypeId;
      d.remarks = dtl.remarks;
      d.medicalResultTypeFileName = dtl.medicalResultTypeFileName;
      this.fileName = dtl.medicalResultTypeFileName
      this.onMedicalResultTypeIdChanged(d.medicalResultTypeId);
      
      this.isMedicalEditorVisible = true;
    },

    onAddMedical() {
      const me = this;

      me.clearMedicalPad();
      me.medical.medicalResultTypeDetailId = -1;

      me.isMedicalEditorVisible = true;
      me.isAddingMedical = true;
    },

    onDeleteMedical(index) {
      const me = this;

      me.dialog.confirm( "Medical Result Type <b>" + me.medicals[index].medicalResultTypeName+ "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.medicals.splice(index, 1);
          }
          return;
        });
    },

    onUploadPhoto () {
      const me = this;

      if (!me.$refs.inputfile.files.length) {
        return;
      }

      switch (me.$refs.inputfile.files[0].type) {
        case 'image/jpeg':
        case 'image/png':
          break;

        default:
          me.advice.fault('Selected file rejected.<hr>Valid file type: .jpg, .png', { duration: 4 });
          return;
      }

      let wait = me.wait();
      me.uploadPhotoTemp = false
      me.uploadPhoto()
        .then( result => {
          if (result) {
            me.advice.success('Image uploaded.', { duration: 4 });
            me.uploadPhotoTemp = true
          }
          me.stopWait(wait);
        })
         .catch ( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        });

    },


    uploadPhoto () {
      const
        me = this,
        width = 200,
        quality = 75,
        files = me.$refs.inputfile.files,
        guid = this.guid();
        this.member.photoFileName = files[0].name;
        this.member.photoGUID = guid;

        return upload('api/images/' + guid + '?width=' + width.toString() + '&quality=' + quality.toString(), files);
    },

    getImageUrl () {
      if (this.uploadPhotoTemp) {
        let files = this.$refs.inputfile.files;
        let fileName = files[0].name;
        let fileExtension = fileName.split('.').pop().toLowerCase();
        return 'temp/' + (this.member.photoGUID + '.' + fileExtension || 'anonymous.png') + '?ts=' + this.tsPhoto.toString();
      } else
      {
        return 'upload/' + (this.member.imageUrl || 'anonymous.png') + '?ts=' + this.tsPhoto.toString();
      }
    },

    onRecruiterIdChanging (e) {
      e.message = "Recruiter ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.recruiterIdCallback;
    },

    recruiterIdCallback (e) {
      const me = this;
      let filter = "RecruiterId='" + e.proposedValue + "'";
      return getList('dbo.HrsRecruiter', 'RecruiterId, RecruiterName', '', filter).then(
        recruiter => {
          if (recruiter && recruiter.length) {
            me.member.recruiterName = recruiter[0].recruiterName;
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

    onRecruiterIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.member;

      item.recruiterId = data.recruiterId;
      item.recruiterName = data.recruiterName;
      this.focusNext();

    },

    onSubmitAffiliation() {
      const me = this,
        d = me.affiliation;

      if (!d.affiliationId) {
        me.isAffiliationEditorVisible = false;
        return;
      }
      if (!me.isValid("hrs0010K")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingAffiliation) {
        Object.assign(dtl, d);

        me.affiliations.push(dtl);

        me.affiliationName.forEach((affiliationName) => {
          if (affiliationName.affiliationId == dtl.affiliationId) {
            dtl.affiliationName = affiliationName.affiliationName;
          }
        });

        dtl.affiliationFileName = this.fileName;
        dtl.affiliationGUID = this.guidReference;

        me.clearAffiliationPad();

        me.advice.info("Affiliation Name '" + dtl.affiliationName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("affiliationId");

      } else {
        dtl = me.affiliations[me.affiliationIndex];

        me.affiliationName.forEach((dtl) => {
          if (dtl.affiliationId == d.affiliationId) {
            d.affiliationName = dtl.affiliationName;
          }
        });

        dtl.affiliationDetailId = d.affiliationDetailId;
        dtl.affiliationId = d.affiliationId;
        dtl.affiliationName = d.affiliationName;
        dtl.affiliationDate = d.affiliationDate;
        dtl.affiliationPosition = d.affiliationPosition;
        dtl.affiliationDescription = d.affiliationDescription;

        if (this.fileName) {
        dtl.affiliationFileName = this.fileName;
        } else {
        dtl.affiliationFileName = d.affiliationFileName;
        }

        if (this.guidReference) {
        dtl.affiliationGUID = this.guidReference;
        } else {
        dtl.affiliationGUID = d.affiliationGUID;
        }

        me.isAffiliationEditorVisible = false;

      }
    },

    onShowAffiliationEditor() {
      const me = this;

      me.setActiveModel("affiliation");

      me.setRequiredMode(
        'affiliationId',
        'affiliationName',
        'affiliationDate',
        'affiliationPosition',
        'affiliationDescription'
      );

      setTimeout(() => {
        this.setFocus("affiliationId");
      }, 200);
    },

    onHideAffiliationEditor() {
      const me = this;

      me.isAddingAffiliation = false;
      me.setActiveModel();
    },

    onEditAffiliation(dtl, index) {
      const d = this.affiliation;
      this.affiliationIndex = index;

      dtl = this.core.convertDates(dtl);
      if (dtl.affiliationDate === null) {
        dtl.affiliationDate = this.core.emptyDateTime();
      }

      d.affiliationDetailId = dtl.AffiliationDetailId;
      d.affiliationId = dtl.affiliationId;
      d.affiliationDate = dtl.affiliationDate;
      d.affiliationPosition = dtl.affiliationPosition;
      d.affiliationDescription = dtl.affiliationDescription;
      d.affiliationFileName = dtl.affiliationFileName;

      this.fileName = dtl.affiliationFileName

      this.isAffiliationEditorVisible = true;
    },

    onAddAffiliation() {
      const me = this;

      me.clearAffiliationPad();
      me.affiliation.affiliationDetailId = -1;
      me.affiliation.affiliationDate = me.today;//me.core.emptyDateTime();

      me.isAffiliationEditorVisible = true;
      me.isAddingAffiliation = true;
    },

    onDeleteAffiliation(index) {
      const me = this;

      me.dialog.confirm( "Affiliation <b>" + me.affiliations[index].affiliationName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.affiliations.splice(index, 1);
          }
          return;
        });
    },


    onMunicipalityIdChanging (e) {
      e.message = "Municipality ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.municipalityIdCallback;
    },

    municipalityIdCallback (e) {
      const me = this;
      let filter = "MunicipalityId=" + e.proposedValue;
      return getList('dbo.DbsMunicipality', 'MunicipalityId, MunicipalityName', '', filter).then(
        municipality => {
          if (municipality && municipality.length) {


            me.work.municipalityName = municipality[0].municipalityName;
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

    onMunicipalityIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        work = this.work;


        work.municipalityId = data.municipalityId;
        work.municipalityName = data.municipalityName;

        this.focusNext();

    },

    onSubmitWork() {
      const me = this,
        d = me.work;

      if (!d.workPosition) {
        me.isWorkEditorVisible = false;
        return;
      }

      if (!me.isValid("hrs0010J")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingWork) {
        Object.assign(dtl, d);

        me.works.push(dtl);

        dtl.workileName = this.fileName;
        dtl.workGUID = this.guidReference;

        me.clearWorkPad();

        me.advice.info("Position '" + dtl.workPosition + "' added to list.", {
          duration: 5,
        });

        me.setFocus("workPosition");

      } else {
        dtl = me.works[me.workIndex];

        dtl.workDetailId = d.workDetailId;

        dtl.workPosition = d.workPosition;
        dtl.companyName = d.companyName;
        dtl.municipalityId = d.municipalityId;
        dtl.municipalityName = d.municipalityName;

        dtl.companyPhoneNumber = d.companyPhoneNumber;
        dtl.workStartDate = d.workStartDate;
        dtl.workEndDate = d.workEndDate;
        dtl.reasonForLeaving = d.reasonForLeaving;

        if (this.fileName) {
        dtl.workFileName = this.fileName;
        } else {
        dtl.workFileName = d.workFileName;
        }

        if (this.guidReference) {
        dtl.workGUID = this.guidReference;
        } else {
        dtl.workGUID = d.workGUID;
        }

        me.isWorkEditorVisible = false;

      }
    },

    onShowWorkEditor() {
      const me = this;

      me.setActiveModel("work");

      me.setRequiredMode(
        'workPosition',
        'companyName',
        'municipalityId',
        'companyPhoneNumber',
        'workStartDate',
        'workEndDate',
        'reasonForLeaving'
      );

      me.setDisplayMode(
        'municipalityName',
      );

      setTimeout(() => {
        this.setFocus("workPosition");
      }, 200);
    },

    onHideWorkEditor() {
      const me = this;

      me.isAddingWork = false;
      me.setActiveModel();
    },

    onEditWork(dtl, index) {
      const d = this.work;
      this.workIndex = index;

      dtl = this.core.convertDates(dtl);

      if (dtl.workStartDate === null) {
        dtl.workStartDate = this.core.emptyDateTime();
      }

      if (dtl.workEndDate === null) {
        dtl.workEndDate = this.core.emptyDateTime();
      }

      d.workDetailId = dtl.workDetailId;
      d.workPosition = dtl.workPosition;
      d.companyName = dtl.companyName;
      d.municipalityId = dtl.municipalityId;
      d.municipalityName = dtl.municipalityName;
      d.companyPhoneNumber = dtl.companyPhoneNumber;

      d.workStartDate = dtl.workStartDate;
      d.workEndDate = dtl.workEndDate;

      d.reasonForLeaving = dtl.reasonForLeaving;

      d.workFileName = dtl.workFileName;
      this.fileName = dtl.workFileName

      this.isWorkEditorVisible = true;
    },

    onAddWork() {
      const me = this;

      me.clearWorkPad();
      me.work.workDetailId = -1;

      me.work.workStartDate = me.today;
      me.work.workEndDate = me.today;

      me.isWorkEditorVisible = true;
      me.isAddingWork= true;
    },

    onDeleteWork(index) {
      const me = this;

      me.dialog.confirm( "Position <b>" + me.works[index].workPosition + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.works.splice(index, 1);
          }
          return;
        });
    },

    onComplianceTrainingIdChanged (newValue) {
      const me = this;

      let o = me.complianceTraining.find( o => o.complianceTrainingId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

      }
    },

    onSubmitCompliance() {
      const me = this,
        d = me.compliance;

      if (!d.complianceTrainingId) {
        me.isComplianceEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010I")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingCompliance) {
        Object.assign(dtl, d);

        me.compliances.push(dtl);

        me.complianceTraining.forEach((complianceTraining) => {
          if (complianceTraining.complianceTrainingId == dtl.complianceTrainingId) {
            dtl.complianceTrainingName = complianceTraining.complianceTrainingName;
          }
        });

        me.trainingInstitution.forEach((trainingInstitution) => {
          if (trainingInstitution.trainingInstitutionId == dtl.trainingInstitutionId) {
            dtl.trainingInstitutionName = trainingInstitution.trainingInstitutionName;
          }
        });

        me.assessmentCenter.forEach((assessmentCenter) => {
          if (assessmentCenter.assessmentCenterId == dtl.assessmentCenterId) {
            dtl.assessmentCenterName = assessmentCenter.assessmentCenterName;
          }
        });

        dtl.complianceFileName = this.fileName;
        dtl.complianceGUID = this.guidReference;

        me.clearCompliancePad();

        me.advice.info("Compliance Name '" + dtl.complianceTrainingName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("complianceTrainingId");

      } else {
        dtl = me.compliances[me.complianceIndex];

        me.complianceTraining.forEach((complaince) => {
          if (complaince.complianceTrainingId == d.complianceTrainingId) {
            dtl.complianceTrainingName = complaince.complianceTrainingName;
          }
        });

        dtl.complianceTrainingDetailId = d.complianceTrainingDetailId;
        dtl.complianceTrainingId = d.complianceTrainingId;

        dtl.certificateNumber = d.certificateNumber;
        dtl.issuanceDate = d.issuanceDate;
        dtl.validityDate = d.validityDate;

        dtl.trainingInstitutionId = d.trainingInstitutionId;
        dtl.assessmentCenterId = d.assessmentCenterId;

        if (this.fileName) {
        dtl.complianceFileName = this.fileName;
        } else {
        dtl.complianceFileName = d.complianceFileName;
        }

        if (this.guidReference) {
        dtl.complianceGUID = this.guidReference;
        } else {
        dtl.complianceGUID = d.complianceGUID;
        }

        me.isComplianceEditorVisible = false;

      }
    },

    onShowComplianceEditor() {
      const me = this;

      me.setActiveModel("compliance");

      me.setRequiredMode(
        'complianceTrainingId',
        'certificateNumber',
        'issuanceDate',
        'validityDate',
        'trainingInstitutionId',
        'assessmentCenterId',
      );

      setTimeout(() => {
        this.setFocus("complianceTrainingId");
      }, 200);
    },

    onHideComplianceEditor() {
      const me = this;

      me.isAddingCompliance = false;
      me.setActiveModel();
    },

    onEditCompliance(dtl, index) {
      const d = this.compliance;
      this.complianceIndex = index;

      dtl = this.core.convertDates(dtl);

      if (dtl.issuanceDate === null) {
        dtl.issuanceDate = this.core.emptyDateTime();
      }

      if (dtl.validityDate === null) {
        dtl.validityDate = this.core.emptyDateTime();
      }

      d.complianceTrainingDetailId = dtl.complianceTrainingDetailId;
      d.complianceTrainingId = dtl.complianceTrainingId;
      d.certificateNumber = dtl.certificateNumber;
      d.issuanceDate = dtl.issuanceDate;
      d.validityDate = dtl.validityDate;
      d.trainingInstitutionId = dtl.trainingInstitutionId;
      d.assessmentCenterId = dtl.assessmentCenterId;

      d.complianceFileName = dtl.complianceFileName;
      this.fileName = dtl.complianceFileName
      this.onComplianceTrainingIdChanged(d.complianceTrainingId);

      this.isComplianceEditorVisible = true;
    },

    onAddCompliance() {
      const me = this;

      me.clearCompliancePad();
      me.compliance.complianceTrainingDetailId = -1;

      me.compliance.issuanceDate = me.today;
      me.compliance.validityDate = me.today;

      me.isComplianceEditorVisible = true;
      me.isAddingCompliance = true;
    },

    onDeleteCompliance(index) {
      const me = this;

      me.dialog.confirm( "Compliance Title <b>" + me.compliances[index].complianceTrainingName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.compliances.splice(index, 1);
          }
          return;
        });
    },


    onNCIIQualificationTitleIdChanged (newValue) {
      const me = this;

      let o = me.nciiTitle.find( o => o.nciiQualificationTitleId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

      }
    },

    onSubmitNCII() {
      const me = this,
        d = me.ncii;

      if (!d.nciiQualificationTitleId) {
        me.isNCIIEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010H")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingNCII) {
        Object.assign(dtl, d);

        me.nciis.push(dtl);

        me.nciiTitle.forEach((nciiTitle) => {
          if (nciiTitle.nciiQualificationTitleId == dtl.nciiQualificationTitleId) {
            dtl.nciiQualificationTitleName = nciiTitle.nciiQualificationTitleName;
          }
        });

        me.trainingInstitution.forEach((trainingInstitution) => {
          if (trainingInstitution.trainingInstitutionId == dtl.trainingInstitutionId) {
            dtl.trainingInstitutionName = trainingInstitution.trainingInstitutionName;
          }
        });

        me.assessmentCenter.forEach((assessmentCenter) => {
          if (assessmentCenter.assessmentCenterId == dtl.assessmentCenterId) {
            dtl.assessmentCenterName = assessmentCenter.assessmentCenterName;
          }
        });

        dtl.nciiFileName = this.fileName;
        dtl.nciiGUID = this.guidReference;

        me.clearNCIIPad();

        me.advice.info("NCII Title '" + dtl.nciiQualificationTitleName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("nciiQualificationTitleId");

      } else {
        dtl = me.nciis[me.nciiIndex];

        me.nciiTitle.forEach((ncii) => {
          if (ncii.nciiQualificationTitleId == d.nciiQualificationTitleId) {
            dtl.nciiQualificationTitleName = ncii.nciiQualificationTitleName;
          }
        });

        dtl.nciiQualificationTitleDetailId = d.nciiQualificationTitleDetailId;
        dtl.nciiQualificationTitleId = d.nciiQualificationTitleId;

        dtl.certificateNumber = d.certificateNumber;
        dtl.issuanceDate = d.issuanceDate;
        dtl.validityDate = d.validityDate;

        dtl.trainingInstitutionId = d.trainingInstitutionId;
        dtl.trainingInstitutionName = d.trainingInstitutionName;
        dtl.assessmentCenterId = d.assessmentCenterId;
        dtl.assessmentCenterName = d.assessmentCenterName;

        if (this.fileName) {
        dtl.nciiFileName = this.fileName;
        } else {
        dtl.nciiFileName = d.nciiFileName;
        }
      
        if (this.guidReference) {
       
        dtl.nciiGUID = this.guidReference;
        } else {
       
        dtl.nciiGUID = d.nciiGUID;
        }

        me.isNCIIEditorVisible = false;

      }
    },

    onShowNCIIEditor() {
      const me = this;

      me.setActiveModel("ncii");

      me.setRequiredMode(
        'nciiQualificationTitleId',
        'certificateNumber',
        'issuanceDate',
        'validityDate',
        'trainingInstitutionId',
        'assessmentCenterId',
      );
      me.setDisplayMode(
        'trainingInstitutionName',
        'assessmentCenterName',
      );
      setTimeout(() => {
        this.setFocus("nciiQualificationTitleId");
      }, 200);
    },

    onHideNCIIEditor() {
      const me = this;

      me.isAddingNCII = false;
      me.setActiveModel();
    },

    onEditNCII(dtl, index) {
      const d = this.ncii;
      this.nciiIndex = index;

      dtl = this.core.convertDates(dtl);

      if (dtl.issuanceDate === null) {
        dtl.issuanceDate = this.core.emptyDateTime();
      }

      if (dtl.validityDate === null) {
        dtl.validityDate = this.core.emptyDateTime();
      }


      d.nciiQualificationTitleDetailId = dtl.nciiQualificationTitleDetailId;
      d.nciiQualificationTitleId = dtl.nciiQualificationTitleId;
      d.certificateNumber = dtl.certificateNumber;
      d.issuanceDate = dtl.issuanceDate;
      d.validityDate = dtl.validityDate;
      d.trainingInstitutionId = dtl.trainingInstitutionId;
      d.trainingInstitutionName = dtl.trainingInstitutionName;
      d.assessmentCenterId = dtl.assessmentCenterId;
      d.assessmentCenterName = dtl.assessmentCenterName;
      d.nciiFileName = dtl.nciiFileName;
      d.nciiGUID = dtl.nciiGUID;
      this.fileName = dtl.nciiFileName
      this.onNCIIQualificationTitleIdChanged(d.nciiQualificationTitleId);

      this.isNCIIEditorVisible = true;
    },

    onAddNCII() {
      const me = this;

      me.clearNCIIPad();
      me.ncii.nciiQualificationTitleDetailId = -1;
      me.ncii.issuanceDate = me.today; 
      me.ncii.validityDate = me.today;
      me.isNCIIEditorVisible = true;
      me.isAddingNCII = true;
      me.trainingInstitution = this.trainingInstitution;
      me.assessmentCenter = this.assessmentCenter;
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


    onCDATrainingIdChanged (newValue) {
      const me = this;

      let o = me.cdaTrainingType.find( o => o.cdaTrainingId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

      }
    },

    onSubmitCDATraining() {
      const me = this,
        d = me.cdaTraining;

      if (!d.cdaTrainingId) {
        me.isCDATrainingEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010G")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingCDATraining) {
        Object.assign(dtl, d);

        me.cdaTrainings.push(dtl);

        me.cdaTrainingType.forEach((cdaTrainingType) => {
          if (cdaTrainingType.cdaTrainingId == dtl.cdaTrainingId) {
            dtl.cdaTrainingName = cdaTrainingType.cdaTrainingName;
          }
        });

        dtl.cdaFileName = this.fileName;
        dtl.cdaGUID = this.guidReference;

        me.clearCDATrainingPad();

        me.advice.info("Training Name '" + dtl.cdaTrainingName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("cdaTrainingId");

      } else {
        dtl = me.cdaTrainings[me.cdaTrainingIndex];

        me.cdaTrainingType.forEach((cda) => {
          if (cda.cdaTrainingId == d.cdaTrainingId) {
            dtl.cdaTrainingName = cda.cdaTrainingName;
          }
        });

        dtl.cdaTrainingDetailId = d.cdaTrainingDetailId;
        dtl.cdaTrainingId = d.cdaTrainingId;

        dtl.certificateNumber = d.certificateNumber;
        dtl.trainingDate = d.trainingDate;

        if (this.fileName) {
        dtl.cdaFileName = this.fileName;
        } else {
        dtl.cdaFileName = d.cdaFileName;
        }

        if (this.guidReference) {
        dtl.cdaGUID = this.guidReference;
        } else {
        dtl.cdaGUID = d.cdaGUID;
        }

        me.isCDATrainingEditorVisible = false;

      }
    },

    onShowCDATrainingEditor() {
      const me = this;

      me.setActiveModel("cdaTraining");

      me.setRequiredMode(
        'cdaTrainingId',
        'certificateNumber',
        'trainingDate',
      );

      setTimeout(() => {
        this.setFocus("cdaTrainingId");
      }, 200);
    },

    onHideCDATrainingEditor() {
      const me = this;

      me.isAddingCDATraining = false;
      me.setActiveModel();
    },

    onEditCDATraining(dtl, index) {
      const d = this.cdaTraining;
      this.cdaTrainingIndex = index;

      dtl = this.core.convertDates(dtl);

      if (dtl.trainingDate === null) {
        dtl.trainingDate = this.core.emptyDateTime();
      }

      d.cdaTrainingDetailId = dtl.cdaTrainingDetailId;
      d.cdaTrainingId = dtl.cdaTrainingId;
      d.certificateNumber = dtl.certificateNumber;
      d.trainingDate = dtl.trainingDate;

      d.cdaFileName = dtl.cdaFileName;
      this.fileName = dtl.cdaFileName
      this.onCDATrainingIdChanged(d.cdaTrainingId);

      this.isCDATrainingEditorVisible = true;
    },

    onAddCDATraining() {
      const me = this;

      me.clearCDATrainingPad();
      me.cdaTraining.cdaTrainingDetailId = -1;
      me.cdaTraining.trainingDate = me.today;

      me.isCDATrainingEditorVisible = true;
      me.isAddingCDATraining = true;
    },

    onDeleteCDATraining(index) {
      const me = this;

      me.dialog.confirm( "CDA Training <b>" + me.cdaTrainings[index].cdaTrainingName+ "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.cdaTrainings.splice(index, 1);
          }
          return;
        });
    },

    onLicenseProfessionIdChanged (newValue) {
      const me = this;

      let o = me.licenseProfession.find( o => o.licenseProfessionId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

      }
    },

    onSubmitLicense() {
      const me = this,
        d = me.license;

      if (!d.licenseProfessionId) {
        me.isLicenseEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010F")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingLicense) {
        Object.assign(dtl, d);

        me.licenses.push(dtl);

        me.licenseProfession.forEach((licenseProfession) => {
          if (licenseProfession.licenseProfessionId == dtl.licenseProfessionId) {
            dtl.licenseProfessionName = licenseProfession.licenseProfessionName;
          }
        });

        dtl.licenseFileName = this.fileName;
        dtl.licenseGUID = this.guidReference;

        me.clearLicensePad();

        me.advice.info("License Name '" + dtl.licenseProfessionName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("licenseProfessionId");

      } else {
        dtl = me.licenses[me.licenseIndex];

        me.licenseProfession.forEach((license) => {
          if (license.licenseProfessionId == d.licenseProfessionId) {
            dtl.licenseProfessionName = license.licenseProfessionName;
          }
        });

        dtl.licenseProfessionDetailId = d.licenseProfessionDetailId;
        dtl.licenseProfessionId = d.licenseProfessionId;
        dtl.prcIdNo = d.prcIdNo;

        if (this.fileName) {
        dtl.licenseFileName = this.fileName;
        } else {
        dtl.licenseFileName = d.licenseFileName;
        }

        if (this.guidReference) {
        dtl.licenseGUID = this.guidReference;
        } else {
        dtl.licenseGUID = d.licenseGUID;
        }

        me.isLicenseEditorVisible = false;

      }
    },

    onShowLicenseEditor() {
      const me = this;

      me.setActiveModel("license");

      me.setRequiredMode(
        'licenseProfessionId',
        'prcIdNo'
      );

      setTimeout(() => {
        this.setFocus("licenseProfessionId");
      }, 200);
    },

    onHideLicenseEditor() {
      const me = this;

      me.isAddingLicense = false;
      me.setActiveModel();
    },

    onEditLicense(dtl, index) {
      const d = this.license;
      this.licenseIndex = index;

      d.licenseProfessionDetailId = dtl.licenseProfessionDetailId;
      d.licenseProfessionId = dtl.licenseProfessionId;
      d.prcIdNo = dtl.prcIdNo;
      d.licenseFileName = dtl.licenseFileName;

      this.fileName = dtl.licenseFileName
      this.onLicenseProfessionIdChanged(d.licenseProfessionId);

      this.isLicenseEditorVisible = true;
    },

    onAddLicense() {
      const me = this;

      me.clearLicensePad();
      me.license.licenseProfessionDetailId = -1;

      me.isLicenseEditorVisible = true;
      me.isAddingLicense = true;
    },

    onDeleteLicense(index) {
      const me = this;

      me.dialog.confirm( "License <b>" + me.licenses[index].licenseProfessionName+ "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.licenses.splice(index, 1);
          }
          return;
        });
    },


    onEducationLevelIdChanged (newValue) {
      const me = this;

      let o = me.educationLevel.find( o => o.educationLevelId == newValue);
      if (o) {
        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

        if (o.courseNameRequiredFlag) {
          me.education.courseName = '';
          me.setRequiredMode(
          'courseName'
          )
        }
        else {
          me.setDisplayMode(
          'courseName'
          )
        }

      }
    },

    onSubmitEducation() {
      const me = this,
        d = me.education;

      if (!d.educationLevelId) {
        me.isEducationEditorVisible = false;
        return;
      }

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010E")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingEducation) {
        Object.assign(dtl, d);

        me.educations.push(dtl);

        me.educationLevel.forEach((educationLevel) => {
          if (educationLevel.educationLevelId == dtl.educationLevelId) {
            dtl.educationLevelName = educationLevel.educationLevelName;
          }
        });

        dtl.eduFileName = this.fileName;
        dtl.eduGUID = this.guidReference;

        me.clearEducationPad();
        me.advice.info("Education Name '" + dtl.educationLevelName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("educationLevelId");

      } else {
        dtl = me.educations[me.educationIndex];

        me.educationLevel.forEach((dtl) => {
          if (dtl.educationLevelId == d.educationLevelId) {
            d.educationLevelName = dtl.educationLevelName;
          }
        });

        dtl.eduDetailId = d.eduDetailId;
        dtl.educationLevelId = d.educationLevelId;
        dtl.educationLevelName = d.educationLevelName;
        dtl.schoolName = d.schoolName;
        dtl.courseName = d.courseName;
        dtl.eduStartYear = d.eduStartYear;
        dtl.eduEndYear = d.eduEndYear;
        if (this.fileName) {
        dtl.eduFileName = this.fileName;
        } else {
        dtl.eduFileName = d.eduFileName;
        }

        if (this.guidReference) {
        dtl.eduGUID = this.guidReference;
        } else {
        dtl.eduGUID = d.eduGUID;
        }

        me.isEducationEditorVisible = false;

      }

      let birthday = new Date(this.member.birthDate);


      let year = birthday.getFullYear();

      let hiredDate = initialHiredDate.getFullYear();


      if (this.education.enedustartYear < year) {
  
      } else {
       
      }


    },

    onShowEducationEditor() {
      const me = this;

      me.setActiveModel("education");

      me.setRequiredMode(
        'educationLevelId',
        'schoolName',
        'eduStartYear',
        'eduEndYear'
      );

      setTimeout(() => {
        this.setFocus("educationLevelId");
      }, 200);
    },

    onHideEducationEditor() {
      const me = this;

      me.isAddingEducation = false;
      me.setActiveModel();
    },

    onEditEducation(dtl, index) {
      const d = this.education;
      this.educationIndex = index;

      d.eduDetailId = dtl.eduDetailId;
      d.educationLevelId = dtl.educationLevelId;
      d.schoolName = dtl.schoolName;
      d.courseName = dtl.courseName;

      d.eduStartYear = dtl.eduStartYear;
      d.eduEndYear = dtl.eduEndYear;
      d.eduFileName = dtl.eduFileName;

      this.fileName = dtl.eduFileName
      this.onEducationLevelIdChanged(d.educationLevelId);

      this.isEducationEditorVisible = true;
    },

    onAddEducation() {
      const me = this;

      me.clearEducationPad();
      me.education.eduDetailId = -1;

      me.isEducationEditorVisible = true;
      me.isAddingEducation = true;
    },

    onDeleteEducation(index) {
      const me = this;

      me.dialog.confirm( "Education <b>" + me.educations[index].educationLevelName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.educations.splice(index, 1);
          }
          return;
        });
    },


    onDocTypeIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.docTypeIdCallback;
    },

    docTypeIdCallback(e) {
      const me = this;
        
      let index = me.docs.findIndex(obj => obj.docTypeId === e.proposedValue);
      if (index > -1) {

        const docTypeName = me.docs[index].docTypeName;
        e.message = 'Doc Type <b>' + docTypeName + '</b> is already in the list.';
        return false;
      }

      return true;
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
          // return true;
        }
        },
        () => {
          return true;
        }
      );


    },
   validateInput() {
    const regex = /^[0-9]*$/;
   if (!regex.test(this.doc.docTypeReference)) {
   
        this.doc.docTypeReference = this.doc.docTypeReference.slice(0, -1);
    }

    if (this.doc.docTypeReference.length > this.doc.docTypeLength) {
        this.doc.docTypeReference = this.doc.docTypeReference.slice(0, this.doc.docTypeLength);
    }

    
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
          
          me.isUploadRequired = selectedDocType.expirationFlag; 
          
        me.doc.docTypeReference = ''; 
          
          this.validateInput(); 
        
      } else {
          me.isUploadRequired = false; 
      }
  },
            

    onSubmitDoc() {

      const me = this, d = me.doc;

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
        this.advice.fault('Entry is not valid.', { duration: 5 });
        return;
      }


      let docTypeReference = d.docTypeReference;
      let filter = "docTypeReference='" + docTypeReference + "' AND docTypeId=" + d.docTypeId  + " AND docTypeDetailId!=" + d.docTypeDetailId; 

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

          dtl.docTypeFileName = me.fileName;
          dtl.docTypeGUID = me.guidReference;

          me.clearDocPad();
          me.advice.info("Doc Type Name '" + dtl.docTypeName + "' added to list.", { duration: 5 });
          me.setFocus("DocTypeId");
        } else {
          dtl = me.docs[me.docIndex];

          me.docType.forEach((doc) => {
            if (doc.docTypeId == d.docTypeId) {
              dtl.docTypeName = doc.docTypeName;
            }
          });

          dtl.docTypeDetailId = d.docTypeDetailId;
          dtl.docTypeId = d.docTypeId;
          dtl.expirationDate = d.expirationDate;
          dtl.docTypeReference = d.docTypeReference;
          dtl.docTypeFileName = me.fileName || d.docTypeFileName;
          dtl.docTypeGUID = me.guidReference || d.docTypeGUID;

          me.isDocEditorVisible = false;
        }
      });

            
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

      dtl = this.core.convertDates(dtl);
      
       this.docType.forEach((doctype) => {
          if (doctype.docTypeId == dtl.docTypeId) {
            dtl.docTypeLength = doctype.docTypeLength;
        
          }
        });

        if (dtl.expirationDate === null) {
        dtl.expirationDate = this.core.emptyDateTime();
      }
      d.docTypeDetailId = dtl.docTypeDetailId;
      d.docTypeId = dtl.docTypeId;
      d.expirationDate = dtl.expirationDate;
      d.docTypeLength = dtl.docTypeLength;
      d.docTypeReference = dtl.docTypeReference;
      d.docTypeFileName = dtl.docTypeFileName;
      d.docTypeGUID = dtl.docTypeGUID;
      this.fileName = dtl.docTypeFileName

      this.isDocEditorVisible = true;
    },

    onAddDoc() {
      const me = this;

      me.clearDocPad();
      me.doc.docTypeDetailId = -1;

      me.isDocEditorVisible = true;
      me.isAddingDoc = true;
    },

    onDeleteDoc(index) {
      const me = this;

      me.dialog.confirm( "Doc <b>" + me.docs[index].docTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.docs.splice(index, 1);
          }
          return;
        });
    },

    onRelationIdChanged (newValue) {
      const me = this;

      let o = me.relation.find( o => o.relationId == newValue);
      if (o) {

        if (o.uploadRequiredFlag) {
          me.isUploadRequired = true
        }
        else {
          me.isUploadRequired = false
        }

        if (o.occupationRequiredFlag) {
          me.setRequiredMode(
          'kinOccupation'
          )
        }
        else {
          me.setOptionalMode(
          'kinOccupation'
          )
        }

        if (o.emailRequiredFlag) {
          me.setRequiredMode(
          'kinEmail'
          )
        }
        else {
          me.setOptionalMode(
          'kinEmail'
          )
        }

      }
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

    getFileName (fileName, GUID) {
      return get('api/member/download-file/' + this.member.memberId + '/' + fileName + '/' + GUID);
    },

    guid () {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
      (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
    },

    // API calls
    uploadDocuments (files) {
      let q = this.guid();

      return upload('api/member/' + this.member.memberId + '/' + this.sym.userInfo.userId + '/' + this.guidReference + '/files?' + q, files);
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

    onSelectedChanged (fileList) {

      this.selectedFileList = fileList;

      if (fileList.length) {
        const uploader = this.$refs.uploader;
        if (uploader) {
          uploader.invokeUpload();
        }
      }

    },

    onSelectDocuments () {

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

    onKinNameChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.kinNameCallback;
    },

    kinNameCallback (e) {
      const me = this;

      let index = me.kins.findIndex(obj => obj.kinName === e.proposedValue);
      if (index > -1) {
        e.message = 'Kin Name <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }

      return true;
    },


    onSubmitKin() {
      const me = this,
        d = me.kin;

      if (!d.kinLastName) {
        me.isKinEditorVisible = false;
        return;
      }

      if (!this.core.isValidEmail(d.kinEmail) && !d.kinEmail) {
        me.advice.fault( '(' + d.kinEmail + ") is not a valid email address.", { duration: 5 });
        return;
      }
      

      if (me.isUploadRequired) {
        if (!me.fileName || me.fileName.trim() === "") {
        me.advice.fault('Upload required. Please select a file.', { duration: 5 });
        return;
        }
      }

      if (!me.isValid("hrs0010C")) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      let dtl = {};

      if (me.isAddingKin) {
        Object.assign(dtl, d);

        me.kins.push(dtl);

        me.relation.forEach((relation) => {
          if (relation.relationId == dtl.relationId) {
            dtl.relationName = relation.relationName;
          }
        });

        me.kinSuffix.forEach((kinSuffix) => {
          if (kinSuffix.kinSuffixId == dtl.kinSuffixId) {
            dtl.kinSuffixName = kinSuffix.kinSuffixName;
          }
        });

     
        dtl.kinFileName = this.fileName;
        dtl.kinGUID = this.guidReference;

        me.clearKinPad();
        me.advice.info("Kin Name '" + dtl.kinLastName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("kinName");

      } else {
        dtl = me.kins[me.kinIndex];

        me.relation.forEach((dtl) => {
          if (dtl.relationId == d.relationId) {
            d.relationName = dtl.relationName;
          }
        });

        me.kinSuffix.forEach((dtl) => {
          if (dtl.kinSuffixId == d.kinSuffixId) {
            d.kinSuffixName = dtl.kinSuffixName;
          }
        });

     
     
        dtl.kinDetailId = d.kinDetailId;
        dtl.kinLastName = d.kinLastName;
        dtl.kinFirstName = d.kinFirstName;
        dtl.kinMiddleName = d.kinMiddleName;
        dtl.kinSuffixId = d.kinSuffixId;
        dtl.emergencyContactFlag = d.emergencyContactFlag;
        dtl.relationId = d.relationId;
        dtl.relationName = d.relationName;
        dtl.kinSuffixName = d.kinSuffixName;
        dtl.kinPhoneNumber = d.kinPhoneNumber;
        dtl.kinOccupation = d.kinOccupation;
        dtl.kinEmail = d.kinEmail;

        if (this.fileName) {
        dtl.kinFileName = this.fileName;
        } else {
        dtl.kinFileName = d.kinFileName;
        }

        if (this.guidReference) {
        dtl.kinGUID = this.guidReference;
        } else {
        dtl.kinGUID = d.kinGUID;
        }

        me.isKinEditorVisible = false;

      }
    },

    onShowKinEditor() {
      const me = this;

      me.setActiveModel("kin");

      me.setRequiredMode(
        'kinLastName',
        'kinFirstName',
        'relationId',
      );

      setTimeout(() => {
        this.setFocus("kinLastName");
      }, 200);
    },

    onHideKinEditor() {
      const me = this;

      me.isAddingKin = false;
      me.setActiveModel();
    },

    onEditKin(dtl, index) {
      const d = this.kin;
      this.kinIndex = index;

      d.kinDetailId = dtl.kinDetailId;
      d.kinLastName = dtl.kinLastName;
      d.kinFirstName = dtl.kinFirstName;
      d.kinMiddleName = dtl.kinMiddleName;
      d.kinSuffixId = dtl.kinSuffixId;
      d.emergencyContactFlag = dtl.emergencyContactFlag;
      d.relationId = dtl.relationId;
      d.kinPhoneNumber = dtl.kinPhoneNumber;
      d.kinOccupation = dtl.kinOccupation;
      d.kinEmail = dtl.kinEmail;
      d.kinFileName = dtl.kinFileName;

      this.fileName = dtl.kinFileName
      this.onRelationIdChanged(d.relationId);

      this.isKinEditorVisible = true;
    },

    onAddKin() {
      const me = this;

      me.clearKinPad();
      me.kin.kinDetailId = -1;

      me.isKinEditorVisible = true;
      me.isAddingKin = true;
    },

    onDeleteKin(index) {
      const me = this;

      me.dialog.confirm( "Kin <b>" + me.kins[index].kinLastName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.kins.splice(index, 1);
          }
          return;
        });
    },

    onMemberLastNameChanging (e) {
      e.callback = this.memberLastNameCallback;
    },

    memberLastNameCallback (e) {
      const me = this;
      if (e.proposedValue && me.member.memberFirstName && me.member.birthDate) {
      let filter = "MemberLastName='" + e.proposedValue + "' AND MemberFirstName='" + me.member.memberFirstName + "' AND MemberMiddleName='" + me.member.memberMiddleName + "' AND BirthDate='" + this.member.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsMember', 'MemberLastName, MemberFirstName, MemberMiddleName, BirthDate, MemberId', '', filter).then(
        member => {
    
          if (member && member.length) {

          me.dialog.confirm('Already exist. Load existing record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.member.memberId = member[0].memberId;
                me.onMemberIdChanged()
                
              return true
              }
              else {
               me.activeTabIndex = 6; 
              }
              return false;
            }
          );

            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },

    onMemberFirstNameChanging (e) {
      e.callback = this.memberFirstNameCallback;
    },

    memberFirstNameCallback (e) {
      const me = this;
      if (e.proposedValue && me.member.memberLastName && me.member.birthDate) {
      let filter = "MemberFirstName='" + e.proposedValue + "' AND MemberLastName='" + me.member.memberLastName + "' AND MemberMiddleName='" + me.member.memberMiddleName + "' AND BirthDate='" + this.member.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsMember', 'MemberLastName, MemberFirstName, MemberMiddleName, BirthDate, MemberId', '', filter).then(
        member => {
          if (member && member.length) {

          me.dialog.confirm('Already exist. Load existing record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.member.memberId = member[0].memberId;
                me.onMemberIdChanged()
              
              return true
              }
              else {
               me.activeTabIndex = 6; 
              }
              return false;
            }
          );

            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },

    onMemberMiddleNameChanging (e) {
      e.callback = this.memberMiddleNameCallback;
    },

    memberMiddleNameCallback (e) {
      const me = this;
      if (me.member.memberFirstName && me.member.memberLastName && me.member.birthDate) {
      let filter = "MemberMiddleName='" + e.proposedValue + "' AND MemberLastName='" + me.member.memberLastName + "' AND MemberFirstName='" + me.member.memberFirstName + "' AND BirthDate='" + this.member.birthDate.toDateFormat() + "'" ;
      return getList('dbo.HrsMember', 'MemberLastName, MemberFirstName, MemberMiddleName, BirthDate, MemberId', '', filter).then(
        member => {
          if (member && member.length) {

          me.dialog.confirm('Already exist. Load existing record?', { scheme: 'warning', icon: 'warning', size: "md" }).then(
            reply => {
              if (reply === 'yes') {
                me.member.memberId = member[0].memberId;
                me.onMemberIdChanged()
             
              return true
              }
              else {
               me.activeTabIndex = 6; 
              }
              return false;
            }
          );

            return false;
          }
          return true;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
      }
      return true;
    },


    onShowSkillSetEditor() {
      const me = this;

      me.setActiveModel("skillSet");

      me.setRequiredMode(
        'skillDetailId',
        'remarks'
      );

      me.setDisplayMode(
        'skillName'
      );

      setTimeout(() => {
        this.setFocus("skillDetailId");
      }, 200);
    },

    onHideSkillSetEditor() {
      const me = this;

      me.isAddingSkillSet = false;
      me.setActiveModel();
    },

    onEditSkillSet(dtl, index) {
      const d = this.skillSet;

      this.skillSetIndex = index;

      d.skillSetDetailId = dtl.skillSetDetailId;
      d.skillDetailId = dtl.skillDetailId;
      d.skillName = dtl.skillName;
      d.remarks = dtl.remarks;

      this.isSkillSetEditorVisible = true;
    },

    onAddSkillSet() {
      const me = this;

      me.clearSkillSetPad();
      me.skillSet.skillSetDetailId = -1;

      me.isSkillSetEditorVisible = true;
      me.isAddingSkillSet = true;
    },

    onDeleteSkillSet(index) {
      const me = this;

      me.dialog
        .confirm(
          "Skill Name <b>" +
            me.skillSets[index].skillName +
            "</b> will be removed from the list.<br><br>Continue?",
          { size: "rg", scheme: "warning" }
        )
        .then((reply) => {
          if (reply === "yes") {
            me.skillSets.splice(index, 1);
          }
          return;
        });
    },

    onSubmitSkillSet() {
      const me = this,
        d = me.skillSet;

      if (!d.skillSetDetailId) {
        me.isSkillSetEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingSkillSet) {
        Object.assign(dtl, d);
        me.skillSets.push(dtl);

        me.clearSkillSetPad();
        me.advice.info("Skill Name '" + dtl.skillName + "' added to list.", {
          duration: 5,
        });
        me.setFocus("skillSetDetailId");

      } else {
        dtl = me.skillSets[me.skillSetIndex];

        dtl.skillSetDetailId = d.skillSetDetailId;
        dtl.skillDetailId = d.skillDetailId;
        dtl.remarks = d.remarks;
        me.isSkillSetEditorVisible = false;

      }
    },

  onSkillDetailIdChanging (e) {
      e.message = "Skill ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.skillDetailIdCallback;
    },

    skillDetailIdCallback (e) {
      const me = this;
      let filter = "SkillDetailId=" + e.proposedValue;
      return getList('dbo.QDbsSkillSetDetail', 'SkillDetailId, SkillDetailName', '', filter).then(
        skill => {
          if (skill && skill.length) {

            let index = me.skillSets.findIndex(obj => obj.skillDetailId === e.proposedValue);
            if (index > -1) {
              e.message = 'Skill ID <b>' + e.proposedValue + '</b> is already in the list.'
              return false;
            }


            me.skillSet.skillName = skill[0].skillDetailName;
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

    onSkillDetailIdSearchResult (result) {
      if (!result) { return; }
      const
        data = result[0],
        skillSet = this.skillSet;
        let index = this.skillSets.findIndex(obj => obj.skillDetailId === data.skillDetailId);
        if (index > -1) {
          this.advice.fault('Skill ID <b>' + data.skillDetailId + '</b> is already in the list.', { duration: 2 } )
          return false;
        }
        skillSet.skillDetailId = data.skillDetailId;
        skillSet.skillName = data.skillDetailName;

        this.focusNext();

    },

  onShowVaccineEditor () {
      const me = this;

      me.setActiveModel('vaccine');

      me.setRequiredMode(
        'vaccineTypeId',
        'vaccineName',
        'vaccineDate'
      );

      setTimeout(() => {
        this.setFocus('vaccineTypeId');
      }, 200);
    },

    onHideVaccineEditor () {
      const me = this;

      me.isAddingVaccine = false;
      me.setActiveModel();
    },

    onEditVaccine (dtl, index) {

      const d = this.vaccine;
      this.vaccineIndex = index;

      dtl = this.core.convertDates(dtl);
      if (dtl.vaccineDate === null) {
        dtl.vaccineDate = this.core.emptyDateTime();
      }

      d.vaccineDetailId = dtl.vaccineDetailId;
      d.vaccineTypeId = dtl.vaccineTypeId;
      d.vaccineName = dtl.vaccineName;      d.vaccineDate = dtl.vaccineDate;

      this.isVaccineEditorVisible = true;
    },

    onAddVaccine () {
      const me = this;

      me.clearVaccinePad();

      me.vaccine.vaccineDetailId = -1
      me.vaccine.vaccineDate = me.today;
      me.isVaccineEditorVisible = true;
      me.isAddingVaccine = true;

    },

    onVaccineReligion (index) {
      const me = this;

      me.dialog.confirm('Vaccine <b>' + me.vaccines[index].religionName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.vaccines.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitVaccine() {
      const me = this,
        d = me.vaccine;

      if (!d.vaccineTypeId) {
        me.isVaccineEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAddingVaccine) {

        Object.assign(dtl, d);

        me.vaccineType.forEach((vaccineType) => {
          if (vaccineType.vaccineTypeId == dtl.vaccineTypeId) {
            dtl.vaccineTypeName = vaccineType.vaccineTypeName;
          }
        });

        me.vaccines.push(dtl);

        me.clearVaccinePad();

        me.advice.info("Vaccine '" + dtl.vaccineTypeName + "' added to list.", {
          duration: 5,
        });

        me.setFocus("vaccineTypeId");

      } else {

        dtl = me.vaccines[me.vaccineIndex];

        me.vaccineType.forEach((dtl) => {
          if (dtl.vaccineTypeId == d.vaccineTypeId) {
            d.vaccineTypeName = dtl.vaccineTypeName;
          }
        });

        dtl.vaccineDetailId = d.vaccineDetailId;
        dtl.vaccineTypeId = d.vaccineTypeId;
        dtl.vaccineTypeName = d.vaccineTypeName;
        dtl.vaccineName = d.vaccineName;
        dtl.vaccineDate = d.vaccineDate;
        me.isVaccineEditorVisible = false;

      }
    },

    onDeleteVaccine(index) {
      const me = this;

      me.dialog.confirm( "Vaccine Type <b>" + me.vaccines[index].vaccineTypeName + "</b> will be removed from the list.<br><br>Continue?", { size: "rg", scheme: "warning" })
        .then((reply) => {
          if (reply === "yes") {
            me.vaccines.splice(index, 1);
          }
          return;
        });
    },

    clearVaccinePad () {
      const d = this.vaccine;

      d.vaccineDetailId = 0;
      d.vaccineTypeId = 0;
      d.vaccineTypeName = '';
      d.vaccineName = '';
      d.vaccineDate = null;

    },

    clearSkillSetPad() {
      const d = this.skillSet;

      d.skillSetDetailId = 0;
      d.skillDetailId = 0;
      d.skillName = "";
      d.remarks = "";
    },

    clearKinPad() {
      const d = this.kin;

      d.kinDetailId = 0;
      d.kinLastName = "";
      d.kinFirstName = "";
      d.kinMiddleName = "";
      d.kinSuffixId = "";
      d.emergencyContactFlag = false;
      d.relationId = 0;
      d.kinPhoneNumber = "";
      d.kinOccupation = "";
      d.kinEmail = "";
      d.kinFileName = "";
      d.kinGUID = "";

      this.fileName = "";
      this.guidReference = "";
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

    clearEducationPad() {
      const d = this.education;

      d.eduDetailId = 0;
      d.educationLevelId = 0;
      d.schoolName = "";
      d.courseName = "";
      d.eduStartYear = 0;
      d.eduEndYear = 0;
      d.eduFileName = "";
      d.eduGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },

    clearLicensePad() {
      const d = this.license;

      d.licenseProfessionDetailId = 0;
      d.licenseProfessionId = 0;
      d.prcIdNo = "";
      d.licenseFileName = "";
      d.licenseGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },

    clearCDATrainingPad() {
      const d = this.cdaTraining;

      d.cdaTrainingDetailId = 0;
      d.cdaTrainingId = 0;
      d.certificateNumber = "";
      d.trainingDate = null;
      d.cdaFileName = "";
      d.cdaGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },
    clearCDATrainingDate(){
      const d = this.cdatraining;
      d.trainingDate = null;
    },

    clearNCIIPad() {
      const d = this.ncii;

      d.nciiQualificationTitleDetailId = 0;
      d.nciiQualificationTitleId = 0;
      d.certificateNumber = "";
      d.issuanceDate = null;
      d.validityDate = null;
      d.trainingInstitutionId = 0;
      d.trainingInstitutionName = "";
      d.assessmentCenterId = 0;
      d.assessmentCenterName = "";
      d.nciiFileName = "";
      d.nciiGUID = "";
      this.searchTraining = '';
      this.searchAssessment = '';
      this.fileName = "";
      this.guidReference = "";
    },
    clearNCIIIssedDate() {
      const d = this.ncii;
      d.issuanceDate = null;
    },
    clearNCIIValidityDate(){
      const d = this.ncii;
      d.validity = null;
    },


    clearCompliancePad() {
      const d = this.compliance;

      d.complianceTrainingDetailId = 0;
      d.complianceTrainingId = 0;
      d.certificateNumber = "";
      d.issuanceDate = null;
      d.validityDate = null;
      d.trainingInstitutionId = 0;
      d.assessmentCenterId = 0;
      d.complianceFileName = "";
      d.complianceGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },
    clearComplieanceIssuedDate(){
      const d = this.complaince;

      d.issuanceDate = null;

    },
      clearComplieanceValidityDate(){
      const d = this.complaince;

      d.validityDate = null;

    },

    clearWorkPad() {
      const d = this.work;

      d.workDetailId = 0;
      d.workPosition = "";
      d.companyName = "";
      d.municipalityId = "";
      d.municipalityName = "";
      d.companyPhoneNumber = "";
      d.workStartDate = null;
      d.workEndDate = null;
      d.reasonForLeaving = "";

      d.workFileName = "";
      d.workGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },
    clearWorkStardate() {
      const d = this.work;
      d.workStartDate = null;
    },
    clearWorkEnddate() {
      const d = this.work;
      d.workStartDate = null;
    },

    clearAffiliationPad() {
      const d = this.affiliation;

      d.affiliationDetailId = 0;
      d.affiliationId = 0;
      d.affiliationDate = null;
      d.affiliationPosition = "";
      d.affiliationDescription = "";
      d.affiliationFileName = "";
      d.affiliationGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },

    clearAffiliationMembeshipDate() {
      const d = this.affiliation;
      d.affiliationDate = null;
    },

    clearMedicalPad() {
      const d = this.medical;

      d.medicalResultTypeDetailId = 0;
      d.medicalResultTypeId = 0;
      d.remarks = "";
      d.medicalResultTypeFileName = "";
      d.medicalResultTypeGUID = "";

      this.fileName = "";
      this.guidReference = "";
    },
      clearCdaTrainingPad() {
      const d = this.cdaTraining;
        d.trainingDate = "";
    },


    getMemberStatusColorClass() {
      if (this.member.memberStatusId === 1) {
        return "success";
      } // Active
      if (this.member.memberStatusId === 2) {
        return "primary";
      } // InActive
      if (this.member.memberStatusId === 3) {
        return "danger";
      } // Suspended
      if (this.member.memberStatusId === 4) {
        return "warning";
      } // Delisted
      return false;
    },

    getRemarksForDisability(disability) {
      const disabilityInMember = this.memberDisabilities.find(obj => obj.disabilityId === disability.disabilityId);
      return disabilityInMember ? disabilityInMember.remarks : ''; // Return remarks or default
    },

    onEmailChanging(e) {
      if (!e.proposedValue) {
        return;
      }

      if (!this.core.isValidEmail(e.proposedValue)) {
        e.message = "(" + e.proposedValue + ") is not a valid email address.";
        e.preventDefault();
        return;
      }
    },

    validatePostalCode() {
      if (!this.member.postalCode) {
        this.result = null;
        return;
      }

      if (this.member.postalCode.length > 4) {
        this.member.postalCode = this.member.postalCode.slice(0, 4);
      }

      this.member.postalCode = this.member.postalCode.replace(/\D/g, "");
    },

    validateAlternatePostalCode() {
      if (!this.member.alternatePostalCode) {
        this.result = null;
        return;
      }

      if (this.member.alternatePostalCode.length > 4) {
        this.member.alternatePostalCode = this.member.alternatePostalCode.slice(0, 4);
      }

      this.member.alternatePostalCode = this.member.alternatePostalCode.replace(/\D/g, "");
    },

    validatePhoneNumber() {
      if (!this.member.phoneNumber) {
        this.result = null;
        return;
      }

      if (this.member.phoneNumber.length > 11) {
        this.member.phoneNumber = this.member.phoneNumber.slice(0, 11);
      }

      this.member.phoneNumber = this.member.phoneNumber.replace(/\D/g, "");
    },


    validateMobileNumber() {
      if (!this.member.mobileNumber) {
        this.result = null;
        return;
      }

      if (this.member.mobileNumber.length > 11) {
        this.member.mobileNumber = this.member.mobileNumber.slice(0, 13);
      }

      this.member.mobileNumber = this.member.mobileNumber.replace(/\D/g, "");

      if (this.member.mobileNumber.length > 4) {
        this.member.mobileNumber= `(${this.member.mobileNumber.slice(0, 4)})${this.member.mobileNumber.slice(4)}`;
      } else {
        this.member.mobileNumber = `(${this.member.mobileNumber.slice(0, 4)}`;
      }
    },

    validateHeight() {
      if (!this.member.height) {
        this.result = null;
        return;
      }

      if (this.member.height.length > 3) {
        this.member.height = this.member.height.slice(0, 3);
      }

      this.member.height = this.member.height.replace(/\D/g, "");
    },
    validateWeight() {
      if (!this.member.weight) {
        this.result = null;
        return;
      }

      if (this.member.weight.length > 3) {
        this.member.weight = this.member.height.slice(0, 3);
      }

      this.member.weight = this.member.weight.replace(/\D/g, "");
    },

    validateGcashNumber() {
      if (!this.member.gCashNumber) {
        this.result = null;
        return;
      }

      if (this.member.gCashNumber.length > 11) {
        this.member.gCashNumber = this.member.gCashNumber.slice(0, 11);
      }

      this.member.gCashNumber = this.member.gCashNumber.replace(/\D/g, "");
    },

    validateBankNumber() {
      if (!this.member.bankAccountNumber) {
        this.result = null;
        return;
      }

      if (this.member.bankAccountNumber.length > 15) {
        this.member.bankAccountNumber = this.member.bankAccountNumber.slice(0, 15);
      }

      this.member.bankAccountNumber = this.member.bankAccountNumber.replace(/\D/g, "");
    },
     validateKinContactNumber() {
      if (!this.kin.kinPhoneNumber) {
        this.result = null;
        return;
      }

      if (this.kin.kinPhoneNumber.length > 11) {
        this.kin.kinPhoneNumber = this.kin.kinPhoneNumber.slice(0, 11);
      }

      this.kin.kinPhoneNumber = this.kin.kinPhoneNumber.replace(/\D/g, "");
    },
    validateWorkContactNumber() {
      if (!this.work.companyPhoneNumber) {
        this.result = null;
        return;
      }

      if (this.work.companyPhoneNumber.length > 11) {
        this.work.companyPhoneNumber = this.work.companyPhoneNumber.slice(0, 11);
      }

      this.work.companyPhoneNumber = this.work.companyPhoneNumber.replace(/\D/g, "");
    },


    onProvinceIdChanging (e) {
      e.message = "Province ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.provinceIdCallback;
    },

    provinceIdCallback (e) {
      const me = this;
      let filter = "ProvinceId='" + e.proposedValue + "'";
      return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter).then(
        province => {
          if (province && province.length) {
            me.member.provinceName = province[0].provinceName;
            me.member.regionId = province[0].regionId;
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

    onProvinceIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.member;

      item.provinceId = data.provinceId;
      item.provinceName = data.provinceName;
      item.regionId = data.regionId;

      this.onProvinceIdChanged()

      this.focusNext();

    },

    onBirthProvinceIdChanging (e) {
      e.message = "Province ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.birthProvinceIdCallback;
    },

    birthProvinceIdCallback (e) {
      const me = this;
      let filter = "ProvinceId='" + e.proposedValue + "'";
      return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter).then(
        province => {
          if (province && province.length) {
            me.member.birthProvinceName = province[0].provinceName;
            me.member.birthRegionId = province[0].regionId;
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

    onBirthProvinceIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.member;

      item.birthProvinceId = data.provinceId;
      item.birthProvinceName = data.provinceName;
      item.birthRegionId = data.regionId;

      this.onBirthProvinceIdChanged()

      this.focusNext();

    },

    onAlternateProvinceIdChanging (e) {
      e.message = "Province ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.alternateProvinceIdCallback;
    },

    alternateProvinceIdCallback (e) {
      const me = this;
      let filter = "ProvinceId='" + e.proposedValue + "'";
      return getList('dbo.DbsProvince', 'ProvinceId, ProvinceName, RegionId', '', filter).then(
        province => {
          if (province && province.length) {
            me.member.alternateProvinceName = province[0].provinceName;
            me.member.alternateRegionId = province[0].regionId;
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

    onAlternateProvinceIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        item = this.member;

      item.alternateProvinceId = data.provinceId;
      item.alternateProvinceName = data.provinceName;
      item.alternateRegionId = data.regionId;

      this.onAlternateProvinceIdChanged()

      this.focusNext();

    },

    onToggleDisabilitySelection (disability) {
      const
        me = this,
        index = me.memberDisabilities.findIndex(obj => obj.disabilityId === disability.disabilityId);

        if (this.isDisabilitySelected(disability)) {
          disability.remarks = '';
        }

      if (index > -1) {
        me.memberDisabilities.splice(index, 1);

      } else {

        me.memberDisabilities.push({
          memberId: me.member.memberId,
          disabilityId: disability.disabilityId,
          remarks: disability.remarks
        });

        me.setFocus("remarks");
      }
    },

    isLanguageSelected (language) {

      return this.memberLanguages.findIndex(obj => obj.languageId === language.languageId) > -1;
    },

    onToggleLanguageSelection (language) {
      const
        me = this,
        index = me.memberLanguages.findIndex(obj => obj.languageId === language.languageId);

      if (index > -1) {
        me.memberLanguages.splice(index, 1);
      } else {
        me.memberLanguages.push({
          memberId: me.member.memberId,
          languageId: language.languageId
        });
      }
    },


    onMemberBadgeIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.memberBadgeIdCallback;
    },

    memberBadgeIdCallback (e) {

      let memberBadgeId = e.proposedValue;
      let filter = "MemberBadgeId='" + memberBadgeId + "'";
      return getCount('dbo.HrsMember', filter).then(
        count => {
          if (count > 0) {
          e.message = '<b>' + memberBadgeId + '</b> already exists.';
          return false;
        } else {
          return true;
        }
        },
        () => {
          return true;
        }
      );


    },

    onActiveTabIndexChanged () {
      this.reload();
    },

    onBirthProvinceIdChanged() {
      const me = this,
        wait = me.wait();
        me.birthMunicipalities = [];

        me.getBirthMunicipalities().then(
        (data) => {
          me.stopWait(wait);
          me.birthMunicipalities = data;

          if (me.birthMunicipalities.length) {
            me.birthMunicipalities.unshift({
              birthMunicipalityId: 0,
              municipalityName: "--- Select Municipality ---",
            });
            me.setFocus("birthMunicipalityId");
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
      me.barangays = [];

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

    onMunicipalityIdChanged() {
      const me = this,
        wait = me.wait();

      me.getBarangays().then(
        (data) => {
          me.stopWait(wait);
          me.barangays = data;

          if (me.barangays.length) {
            me.barangays.unshift({
              barangayId: 0,
              barangayName: "--- Select Barangay ---",
            });
            me.setFocus("barangayId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onAlternateProvinceIdChanged() {
      const me = this,
        wait = me.wait();

      me.alternateMunicipalities = [];
      me.alternateBarangays = [];

      me.getAlternateMunicipalities().then(
        (data) => {
          me.stopWait(wait);
          me.alternateMunicipalities = data;
        
          if (me.alternateMunicipalities.length) {
            me.alternateMunicipalities.unshift({
              alternateMunicipalityId: 0,
              municipalityName: "--- Select Municipality ---",
            });
            me.setFocus("alternateMunicipalityId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onAlternateMunicipalityIdChanged() {
      const me = this,
        wait = me.wait();

      me.getAlternateBarangays().then(
        (data) => {
          me.stopWait(wait);
          me.alternateBarangays = data;
       
          if (me.alternateBarangays.length) {
            me.alternateBarangays.unshift({
              alternateBarangayId: 0,
              barangayName: "--- Select Barangay ---",
            });
            me.setFocus("alternateBarangayId");
          }
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },


    onBirthDateChanging(e) {
      if (e.proposedValue instanceof DateTime) {
        let me = this,
          p = e.proposedValue;
          let message ="Minimum Age is '<b>" + me.memberValidation[0].minimumAge + "</b>'";
   
        if (p > me.memberValidation[0].minimumBirthDate) {
          e.preventDefault();
          me.advice.fault(message, { duration: 5 });
          return;
        }
  
        

        if (me.member.memberLastName && me.member.memberFirstName && e.proposedValue) {
        
        let filter = "MemberLastName='" + me.member.memberLastName + "' AND MemberFirstName='" + me.member.memberFirstName + "' AND MemberMiddleName='" + me.member.memberMiddleName + "' AND BirthDate='" + e.proposedValue.toDateFormat() + "'" ;

        return getList('dbo.HrsMember', 'MemberLastName, MemberFirstName, MemberMiddleName, BirthDate, MemberId', '', filter).then(
          member => {
           
            if (member && member.length) {
          
              me.dialog.confirm('Already exist. Load existing record?',  { scheme: 'warning', icon: 'warning', size: "md" }).then(
              reply => {
                if (reply === 'yes') {
                
                  me.member.memberId = member[0].memberId;
                  me.onMemberIdChanged()
                  
                  return true;
                }
                else {
                  me.activeTabIndex = 6;
                }
                

                return false;
              }
            );
             
              return false;
            }
            return true;
          },
          fault => {
            me.showFault(fault);
            return false;
          }
        );
      }

      
      }
      return true;

    },

   onBirthDateChanged() {

    const birthDate = new Date(this.member.birthDate.year, this.member.birthDate.month - 1, this.member.birthDate.day);
    const today = new Date();

    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDifference = today.getMonth() - birthDate.getMonth();

    if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

      this.member.age = age;
    },

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.memberType = data.memberType;
            me.memberStatus = data.memberStatus;
            me.sex = data.sex;
            me.civilStatus = data.civilStatus;
            me.bloodType = data.bloodType;
            me.religion = data.religion;
            me.region = data.region;
            me.relation = data.relation;
            me.birthRegion = data.birthRegion;
            me.cdaMemberTypes = data.cdaMemberTypes;
            me.employmentStatus = data.employmentStatus;
            me.banks = data.banks;
            me.languages = data.languages;
            me.disabilities = data.disabilities;
            me.recordings = data.recordings;
            me.vaccineType = data.vaccineType;
            me.docType = data.docType;
             console.log("doctype",data.docType)
            me.educationLevel = data.educationLevel;
            me.licenseProfession = data.licenseProfession;
            me.cdaTrainingType = data.cdaTrainingType;
            me.nciiTitle = data.nciiTitle;
           
            me.trainingInstitution = data.trainingInstitution;
            me.assessmentCenter = data.assessmentCenter;
            me.complianceTraining = data.complianceTraining;
            me.affiliationName = data.affiliationName;
            me.sources = data.sources;
            me.medicalResultType = data.medicalResultType;
            me.memberTypeQualification = data.memberTypeQualification;
            me.memberValidation = data.memberValidation;
            me.employmentType = data.employmentType;
            me.rnrNumbers = data.rnrNumbers;
            me.suffix = data.suffix;
            me.kinSuffix = data.kinSuffix;
          }
          if (me.member.memberId < 0) {
            return Promise.resolve(null);
          }
          return me.getMember();
        })
        .then((member) => {
          me.stopWait(wait);
          if (member && member.member.length) {
            me.setModels(member);
          } else {
            if (me.member.memberId > -1) {
              let message =
                "Member ID '<b>" + me.member.memberId + "</b>' not found.";
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
          me.setupControls();
          me.isFilled = true;
        })
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    },

    onViewPoolingLog(index) {
      const me = this,
        wait = me.wait(),
        dtl = me.pooling[index];
        me.getPoolingLog(dtl.applicantDetailId).then(
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
        member = info.member[0];

      me.member = me.core.convertDates(member);
    
      me.provinces = info.province;
      me.municipalities = info.municipality;
      me.barangays = info.barangay;
      me.birthProvinces = info.birthProvince;
      me.birthMunicipalities = info.birthMunicipality;
      me.memberLanguages = info.languages;
      me.memberDisabilities = info.disabilities;
      me.rnrRecordings = info.recordings;

      me.vaccines = info.vaccines;
      me.skillSets = info.skillSets;
      me.kins = info.kins;
 
      me.docs = info.docs;
      me.educations = info.educations;
      me.licenses = info.licenses;
      me.cdaTrainings = info.cdaTrainings;
      me.nciis = info.nciis;
      me.compliances = info.compliances;
      me.works = info.works;
      me.affiliations = info.affiliations;
      me.medicals = info.medicals;

      me.alternateProvinces = info.alternateProvince;
      me.alternateMunicipalities = info.alternateMunicipality;
      me.alternateBarangays = info.alternateBarangay;
      me.pooling = info.pooling;
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldMember = JSON.stringify(me.member);
      me.oldMemberLanguages = JSON.stringify(me.memberLanguages);
      me.oldMemberDisabilities = JSON.stringify(me.memberDisabilities);
      me.oldVaccines = JSON.stringify(me.vaccines);
      me.oldSkillSets = JSON.stringify(me.skillSets);
      me.oldKins = JSON.stringify(me.kins);
      me.oldDocs = JSON.stringify(me.docs);
      me.oldEducations = JSON.stringify(me.educations);
      me.oldLicenses = JSON.stringify(me.licenses);
      me.oldCdaTrainings = JSON.stringify(me.cdaTrainings);
      me.oldNciis = JSON.stringify(me.nciis);
      me.oldCompliances = JSON.stringify(me.compliances);
      me.oldWorks = JSON.stringify(me.works);
      me.oldAffiliations = JSON.stringify(me.affiliations);
      me.oldMedicals = JSON.stringify(me.medicals);
      me.oldRnrRecordings = JSON.stringify(me.rnrRecordings);
    
    },

    getProvinces() {
      return get("api/member-provinces");
    },

    getMunicipalities() {
      const me = this;
      return get("api/member-municipalities/" + me.member.provinceId );
    },

    getBarangays() {
      const me = this;
      return get("api/member-barangays/" + me.member.municipalityId);
    },

    getBirthProvinces() {
      return get("api/member-birth-provinces" );
    },

    getBirthMunicipalities() {
      const me = this;

      return get("api/member-birth-municipalities/" + me.member.birthProvinceId );
    },


    getAlternateProvinces() {
      return get("api/member-alternate-provinces");
    },

    getAlternateMunicipalities() {
      const me = this;
      return get("api/member-alternate-municipalities/" + me.member.alternateProvinceId );
    },

    getAlternateBarangays() {
      const me = this;
      return get("api/member-alternate-barangays/" + me.member.alternateMunicipalityId);
    },

    getMember() {
      if (this.member.memberId < 0) {
        return Promise.resolve(null);
      }

      return get("api/members/" + this.member.memberId);
    },

    getReferences() {
      const me = this;
      if (me.memberType.length) {
        return Promise.resolve(true);
      }

      return get("api/references/hrs0010");
    },

    getChangeLog(log) {
      return get("api/member/" + log + "/" + this.member.memberId + "/log");
    },

    getPoolingLog(applicantDetailId) {

      return get("api/member-pooling/" + applicantDetailId + "/log");
    },

    getMemberTypeQualifications() {
      const me = this;
      return get("api/member-type-qualifications/" + me.member.memberTypeId);
    },


    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/members/" + currentUserId, options);
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
        return ajax("api/members/" + this.member.memberId + "/" + currentUserId,options);
    },

    deleteRecord() {
      const
        member = this.member,
        options = this.core.getAjaxOptions("DELETE");

      return ajax("api/members/" + member.memberId + this.getDeleteQuery(member.lockId), options);
    },

    getApiPayload() {
      const me = this,
        member = {};
     
      Object.assign(member, me.member);
    
      member.languages = me.memberLanguages;
      member.disabilities = me.memberDisabilities;
      member.vaccines = me.vaccines;
      member.skillSets = me.skillSets;
      member.kins = me.kins;
      member.docs = me.docs;
      member.educations = me.educations;
      member.licenses = me.licenses;
      member.cdaTrainings = me.cdaTrainings;
      member.nciis = me.nciis;
      member.compliances = me.compliances;
      member.works = me.works;
      member.affiliations = me.affiliations;
      member.medicals = me.medicals;
   
      if (me.member.memberTypeId ==1) {
        member.rnrRecordings = [];
      } else {
        member.rnrRecordings = me.rnrRecordings;
      }
      return member;
    },

    // event handlers

    onLoad() {
      const me = this,
        dc = me.dataConfig;

      dc.models.push(
        "member",
        "languages",
        "disabilities",
        "rnrRecordings",
        "vaccines",
        "skillSets",
        "kins",
        "docs",
        "educations",
        "licenses",
        "cdaTrainings",
        "nciis",
        "compliances",
        "works",
        "affiliations",
        "rnrRecording",
        "vaccine",
        "skillSet",
        "kin",
        "doc",
        "education",
        "license",
        "cdaTraining",
        "ncii",
        "compliance",
        "work",
        "affiliation",

      );
      dc.keyField = "memberId";
      dc.autoAssignKey = true;
    },


    onResetAfter() {
      (this.memberLanguages = []),
      (this.memberDisabilities = []),
      (this.vaccines = []),
      (this.skillSets = []),
      (this.kins = []),
      (this.docs = []),
      (this.educations = []),
      (this.licenses = []),
      (this.cdaTrainings = []),
      (this.nciis = []),
      (this.compliances = []),
      (this.works = []),
      (this.affiliations = []),
      (this.medicals = []),
      (this.rnrRecordings = []),

      this.refreshOldRefs();

      this.uploadPhotoTemp = false

      this.activeTabIndex = 0;

      setTimeout(() => {
        this.disableElement("btn-add");
      }, 100);
    },

    onMemberIdChanging(e) {
      e.callback = this.memberIdCallback;
    },

    memberIdCallback(e) {
      e.message = "Member ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity("dbo.HrsMember", "memberId", e.proposedValue ).then((result) => {
        return result;
      });
    },

    onMemberIdChanged() {
      const me = this;
      me.loadData();
      me.replaceUrl();
    },


    onMemberIdLostFocus() {
      const me = this;

      if (!me.member.memberId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.member.memberId = data.memberId;
      me.loadData();
      me.replaceUrl();

    },

    onMemberTypeIdChanged() {
      const me = this;

      me.getMemberTypeQualifications().then(
        (data) => {

          me.member.typeQualificationDetailId = 0;
          me.memberTypeQualification = []
          me.memberTypeQualification = data;
          me.setFocus("typeQualificationDetailId");
        },
        (fault) => {
          // me.stopWait(wait);
          me.showFault(fault);
        }
      );

      if (!me.member.memberTypeId ==2) {
        me.rnrRecordings = [];
        me.recordings = [];
      }

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
    
      if (me.member.memberTypeId == 2) {
        let total = 0;
        me.rnrRecordings.forEach(dtl => {
          if (typeof Number(dtl.rnrNumber) === 'number' && !isNaN(dtl.rnrNumber)) {
            total += Number(dtl.rnrNumber);
          }
        });

        if (total !== 1) {
          me.advice.fault(
            "Rnr Recording must be equal to 1 before saving.",
            { duration: 5 }
          );
          return;
        }
      }

      if (!me.hasChanges()) {
       
        if (me.isPoolingFlag){
            
          me.goBack();    
        }
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
              me.member.memberId = success;
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
            if (me.isPoolingFlag){
              me.goBack();
              } else {
              me.advice.success(message, { duration: 5 });
            }
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

      getSafeDeleteFlag("HrsMember", me.member.memberId)
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

    // helpers

    setupControls() {
      const me = this;

      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "memberBadgeId",
          "memberLastName",
          "memberFirstName",
          "memberTypeId",
          "typeQualificationDetailId",
          "memberStatusId",
          "birthDate",
          "initialHiredDate",
          "sexId",
          "civilStatusId",
          "religionId",
          "address1",
          "regionId",
          "provinceId",
          "municipalityId",
          "barangayId",
          "cdaMemberTypeId",
          "cdaMemberTypeAmount",
          "employmentStatusId",
          "employmentTypeId",
          "phoneNumber",
          "initialHiredDate"
          // "email", 
        );

        if (me.member.initialHiredDate) {
                  me.setDisplayMode(
          "initialHiredDate"
        );
          
        }

        me.setDisplayMode(
          "age",
          "birthProvinceName",
          "provinceName",
          "recruiterName",
          "alternateProvinceName",
          "cdaMemberTypeAmount",
          "postalCode"
        );

        me.setFocus("memberLastName");
      }, 100);
    },
    hasChanges() {
      const me = this;
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.member) !== me.oldMember) {return true;}


       if (JSON.stringify(me.memberLanguages) !== me.oldMemberLanguages) { return true; }
     
      if (JSON.stringify(me.rnrRecordings) !== me.oldRnrRecordings) { return true; } 
      if (JSON.stringify(me.vaccines) !== me.oldVaccines) { return true; }
      if (JSON.stringify(me.skillSets) !== me.oldSkillSets) { return true; }
      if (JSON.stringify(me.kins) !== me.oldKins) { return true; }
      if (JSON.stringify(me.docs) !== me.oldDocs) { return true; }
     
      if (JSON.stringify(me.educations) !== me.oldEducations) { return true; }
       if (JSON.stringify(me.licenses) !== me.oldLicenses) { return true; }
      if (JSON.stringify(me.cdaTrainings) !== me.oldCdaTrainings) { return true; }
      if (JSON.stringify(me.nciis) !== me.oldNciis) { return true; }
      if (JSON.stringify(me.compliances) !== me.oldCompliances) { return true; }
      if (JSON.stringify(me.works) !== me.oldWorks) { return true; }
      if (JSON.stringify(me.affiliations) !== me.oldAffiliations) { return true; }
      if (JSON.stringify(me.medicals) !== me.oldMedicals) { return true; }
      return false;

    },
    
    onClickMRF(memberRequestId) {
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


  },

  created () {
    const me = this;

    me.oldMember = "";
    me.oldMemberLanguages = '';
    me.oldMemberDisabilities = '';
    me.oldRnrRecordings = '';

    me.oldVaccines = '';
    me.oldSkillSets = '';
    me.oldKins = '';
    me.oldDocs = '';
    me.oldEducations = '';
    me.oldLicenses = '';
    me.oldCdaTrainings = '';
    me.oldNciis = '';
    me.oldCompliances = '';
    me.oldWorks = '';
    me.oldAffiliations = '';
    me.oldMedicals = '';
    me.memberType = []; 
    me.memberStatus = []; 
    me.sex = []; 
    me.civilStatus = []; 
    me.bloodType = []; 
    me.religion = []; 
    me.region = []; 
    me.birthRegion = []; 
    me.relation = []; 
    me.kinSuffix = []; 
 
    me.cdaMemberTypes = [];
    me.employmentStatus = [];
    me.banks = [];
    me.languages = [];
    me.disabilities = [];
    me.recordings = [];
    me.vaccineType = [];
    me.docType = [];
    me.educationLevel = [];
    me.licenseProfession = [];
    me.cdaTrainingType = [];
    me.nciiTitle = [];
    me.trainingInstitution = [];
    me.assessmentCenter = [];
    me.complianceTraining = [];
    me.affiliationName = [];
    me.sources = [];
    me.medicalResultType = [];
    me.employmentType = [];
    me.rnrNumbers = [];
    me.suffix = [];
    me.memberTypeQualification = []; 
    me.memberValidation = []; 

    me.today = me.sym.dateInfo.serverDate;
  },

  computed: {

 filteredLanguages() {
    const input = this.languageInput.toLowerCase().trim();
    console.log('LANGUAGE',input)
    return this.languages.filter(lang =>
      lang.languageName.toLowerCase().includes(input) &&
      !this.memberLanguages.includes(lang.languageId)
    );
  },

     isPDF() {
    const cleanUrl = (this.previewFileName || '').trim().toLowerCase();
    const result = cleanUrl.endsWith('.pdf') || /\.pdf(\w*)?$/.test(cleanUrl);
 
    return result;
  },

    editorDocPreviewTitle() {
      return this.previewFileName ;
    },

    isValidMobileNumber() {

    let value = "";

      if (this.member.mobileNumber) {

      if (this.member.mobileNumber.length == 0) {
        value = "default";
        return value;
      }

      }

      const mobileNumberRegex = /^09\d{09}$/;
      const isValid = mobileNumberRegex.test(this.member.mobileNumber);

      value = isValid ? "valid" : "invalid";
      return value;
    },

    isValidGcashVerifyNumber() {

      let value = "";

      if (this.member.gCashNumber) {

        if (this.member.gCashNumber.length == 0 ) {
        value = "default";
        return value;
      }

      return value;
    }

      const mobileNumberRegex = /^09\d{09}$/;
      const isValid = mobileNumberRegex.test(this.member.gCashNumber);

      value = isValid ? "valid" : "invalid";
      return value;
    },
    isValidKinContactNumber() {

    let value = "";

      if (this.kin.kinPhoneNumber) {

      if (this.kin.kinPhoneNumber.length === 0 && this.kin.kinPhoneNumber.length < 11) {
        value = "";
        return value;
      }

      }
      const mobileNumberRegex = /^09\d{09}$/;
      const isValid = mobileNumberRegex.test(this.kin.kinPhoneNumber);

      value = isValid ? "valid" : "invalid";
      return value;
    },
    isValidWorkContactNumber() {

    let value = "";

      if (this.work.companyPhoneNumber) {

      if (this.work.companyPhoneNumber.length === 0 && this.work.companyPhoneNumber.length < 11) {
        value = "";
        return value;
      }

      }
      const mobileNumberRegex = /^09\d{09}$/;
      const isValid = mobileNumberRegex.test(this.work.companyPhoneNumber);

      value = isValid ? "valid" : "invalid";
      return value;
    },

    editorVaccineTitle () {
      if (this.isAddingVaccine) {
        return 'Add Vaccine Detail';
      }
      return 'Edit Vaccine Detail';
    },

    editorRnrTitle () {
      if (this.isAddingVaccine) {
        return 'Add RNR Detail';
      }
      return 'Edit RNR Detail';
    },

    editorSkillSetTitle() {
      if (this.isAddingSkillSet) {
        return 'Add Skill Set Detail';
      }
      return 'Skill Set Detail';
    },

    editorKinTitle() {
      if (this.isAddingKin) {
        return 'Add Kin Detail';
      }
      return 'Kin Detail';
    },

    editorDocTitle() {
      if (this.isAddingDoc) {
        return 'Add Doc Type Detail';
      }
      return 'Doc Type Detail';
    },

    editorEducationTitle() {
      if (this.isAddingEducation) {
        return 'Add Education Detail';
      }
      return 'Education Detail';
    },

    editorLicenseTitle() {
      if (this.isAddingLicense) {
        return 'Add License Detail';
      }
      return 'License Detail';
    },

    editorCDATrainingTitle() {
      if (this.isAddingCDATraining) {
        return 'Add CDA Training Detail';
      }
      return 'CDA Training Detail';
    },

    editorNCIITitle() {
      if (this.isAddingNCII) {
        return 'Add NCII Title Detail';
      }
      return 'NCII Title Detail';
    },

    editorComplianceTitle() {
      if (this.isAddingCompliance) {
        return 'Add Compliance Training Detail';
      }
      return 'Compliance Training Detail';
    },

    editorWorkTitle() {
      if (this.isAddingWork) {
        return 'Add Work Experience Detail';
      }
      return 'Work Experience Detail';
    },

    editorAffiliationTitle() {
      if (this.isAddingAffiliation) {
        return 'Add Affiliation Detail';
      }
      return 'Affiliation Detail';
    },

    editorMedicalTitle() {
      if (this.isAddingMedical) {
        return 'Add Medical Result Type Detail';
      }
      return 'Medical Result Type Detail';
    },

  },
};
</script>

<style scoped>
input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
}
.action-buttons{
display: flex;
justify-content: center;
}


.command-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.buttons {
  flex-wrap: nowrap;
}
.header-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.memberid-info {
  display: flex;
  flex-direction: row;
  gap: 1rem;
  width: 100%;
}

.main-container {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  margin-top: 0.5rem;
}
.container-1 {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex-wrap: wrap;
}
.container-2 {
  display: flex;
  flex-direction: column;
  gap: 0.5em;
}
.container-3 {
  display: flex;
  flex-direction: column;
  gap: 0.5em;
}
.container-4 {
margin-top: 1rem;
}

.photo {
  object-fit: cover;
  height: 12rem !important;
  max-height: 12rem !important;
  width: 100% !important;
  max-width: 100% !important;
}

.personal {
  display: flex;
  flex-direction: column;
  justify-content: center;
  width: 100%;
  padding: 0;
  margin: 0;
}

.Header {
  width: 100%;
  border: 0;
  padding: 10px;
  text-align: center;
  font-weight: bold;
  margin-bottom: 0.5rem;

  color: white;
}
.input-file {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
}
.upload-btn{
  border-top-left-radius: 0px;
  border-top-right-radius: 0px;
  width: 100%;
}
.img {
  border: 1px solid black;
  height: 23%;
}
.btn-uploadPhoto {
  width: 100%;

  border-bottom-left-radius: 5px;
  border-bottom-right-radius: 5px;
  border-top-left-radius: 0;
  border-top-right-radius: 0;
}
.tb {
  width: 100%;
  margin-left: 3px;
}
.work-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.status{
  width: 20%;
}
.scroller {
  overflow: hidden;
}
.doc-tb {
  width: 100%;
}

.physic {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.contact {
  display: flex;
  flex-direction: column;
  gap: 0;
  padding: 0px;
}
.soc {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.identify {
  display: flex;
  flex-direction: column;
  gap: 0reemm;
  margin-top: 0;
  padding: 0px;
}
.religion {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-top: 0;
  padding: 0px;
}
.tab1 {
  display: flex;
  flex-direction: column;
  width: 150%;
}
.scroller-tabs {
  padding-top: 10px;
}

.status {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.btns {
  display: flex;

  justify-content: flex-end;
  gap: 0.5rem;
}

.parent-container {
  display: grid;
  grid-template-columns: 7fr 1fr 1fr;
  gap: 0.5rem;
}
.pwd-container {
  display: grid;
  grid-template-columns: 7fr 1fr 1fr;
  gap: 0.5rem;
}
.medical-scroller{
  overflow: auto;
}
.medical-btns {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
.kin-scroller {
  overflow: auto;
}
.kin-tb {
  width: 150vw;
}
.kin-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.kin-name {
  width: 81%;
}
.kin-relation {
  width: 11%;
}
.kin-contact {
  width: 8%;
}
.kin-occupation {
  width: 15%;
}
.kin-name {
  width: 15%;
}
.kin-email {
  width: 15%;
}
.kin-file {
  width: 15%;
}
.kin-action {
  width: 6%;
}
.education-scroller {
  overflow: auto;
}
.edu-tb {
  width: 100vw;
}
.edu-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.educ-name {
  width: 12%;
}
.educ-school {
  width: 17%;
}
.educ-course {
  width: 30%;
}
.educ-start {
  width: 4%;
}
.educ-end {
  width: 4%;
}
.educ-file {
  width: 10%;
}
.educ-action {
  width: 10%;
}
.license-scroller{
  overflow: auto;
}
.linces-tb{
  width: 100vw;
}
.license-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.license-name {
  width: 51%;
}
.license-prcid {
  width: 51%;
}
.license-file {
  width: 51%;
}
.license-action {
  width: 20%;
}
.ncii-scroller{
  overflow: auto;
}
.ncii-tb {
  width: 110vw;
}
.ncii-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.ncii-cert {
  width: 15%;
}
.ncii-quali {
  width: 51%;
}
.ncii-issuance {
  width: 15%;
}
.ncii-validity {
  width: 15%;
}
.ncii-training {
  width: 51%;
}
.ncii-assesment {
  width: 51%;
}
.ncii-file {
  width: 25%;
}
.ncii-action {
  width: 25%;
}
.compliance-scroller{
  overflow: auto;
}
.compliance-tb {
  width: 115vw;
}
.compliance-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.compli-cert {
  width: 25%;
}
.compli-compli {
  width: 51%;
}
.compli-issuance {
  width: 15%;
}
.compli-validity {
  width: 15%;
}
.compli-training {
  width: 51%;
}
.compli-assessment {
  width: 51%;
}
.compli-file {
  width: 25%;
}
.compli-action {
  width: 20%;
}
.cda-scroller{
 overflow: auto;
}
.cda-tb{
  width: 100vw;
}
.cda-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.affiliation-scroller{
 overflow: auto;
}
.affiliation-tb{
  width: 100vw;
}
.affiliation-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.work-scroller {
  overflow: auto;
}
.work-tb {
  width: 120vw;
}
.work-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.work-title {
  width: 31%;
}
.work-company {
  width: 51%;
}
.work-address {
  width: 51%;
}
.work-phone {
  width: 20%;
}
.work-start {
  width: 20%;
}
.work-end {
  width: 20%;
}
.work-action {
  width: 20%;
}

.cert-name {
  width: 51%;
}
.cert-rating {
  width: 15%;
}
.cert-issuedBy {
  width: 51%;
}
.cert-issuedDate {
  width: 15%;
}
.cert-action {
  width: 15%;
}
.elig-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.elig-name {
  width: 81%;
}
.elig-year {
  width: 15%;
}
.elig-action {
  width: 11%;
}
.licen-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.licen-title {
  width: 51%;
}
.licen-expiry {
  width: 10%;
}
.licen-action {
  width: 7%;
}
.skill-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.skill-name {
  width: 51%;
}
.skill-remarks {
  width: 40%;
}
.skill-action {
  width: 2%;
}
.disability-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.docs-btns {
  display: flex;
  flex-direction: row;
  justify-content: space-evenly;
}
.log-btn{
  display: flex;
  justify-content: center;
 
}
.kin-upload {
  display: grid;
  grid-template-columns: 50fr 5fr;
  gap: 0.5rem;
}
#hrs0010 >>> ul.tabs {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 1rem;
}
.detail-editor-boxes {
 display: flex;
 flex-direction: column;
 gap: .5rem;
}
.kin-editor-boxes {
 display: flex;
 flex-direction: column;
 gap: .5rem;
}
.educ-editor-boxes {
 display: flex;
 flex-direction: column;
 gap: .5rem;
}
.license-editor-boxes {
  display: flex;
  flex-direction: column;
}
.skill-editor-boxes {
  display: grid;
  grid-template-columns: 3fr 6fr 3fr;
  gap: 0.5rem;
}
.vaccine-editor-boxes {
  display: grid;
  grid-template-columns: 3fr 6fr 3fr;
  gap: 0.5rem;
}
.rnr-editor-boxes{
  display: grid;
  grid-template-columns: 5fr  3fr;
  gap: 0.5rem;
}
.kins-btns {
  display: flex;
  flex-direction: column;
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
.MobilePhone {
  border-top-right-radius: 0;
  border-bottom-right-radius: 4px;
}
.act-btns {
  display: flex;
  justify-content: space-evenly;
}
.remarks-container{
  padding: 0;
}
.remarks{
  border-radius: 0;
  border: 1px solid rgb(154, 150, 150);
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
.province-field{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr ;
  width: 100%;
  margin: 0;
  gap: .5rem;
}
.dates{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;
}
.box-img{
  height: 100%;
}
.box-third-column{
  display: grid;
  grid-template-columns: .4fr .4fr .3fr .4fr 1fr 1fr;
  gap: .5rem;
}
.box-forth-column{
  display: grid;
  grid-template-columns: .5fr .5fr .5fr 1fr 1fr;
  gap: .5rem;
}
.box-loc-column{
  display: grid;
  grid-template-columns: .5fr 1fr 1fr 1fr ;
  gap: .5rem;
}
.box-address-column{
  display: grid;
  grid-template-columns: 1fr 1fr ;
  gap: .5rem;
}
.box-fix-column{
  display: grid;
  grid-template-columns: .5fr .5fr 1fr 1fr 1fr;
  gap: .5rem;  
}
.box-qual-column{
  display: grid;
  grid-template-columns: .8fr .8fr 1fr;
  gap: .5rem;  
}
.box-employee{
  display: grid;
  grid-template-columns: .6fr 2fr 2fr 2fr .6fr;
  gap: .5rem;  
}
.box-info-column{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr;
  gap: .5rem;  
}
.box-check-column{
  display: grid;
  grid-template-columns: .5fr .2fr .5fr 1fr  ;
  gap: .5rem;  
}
.box-check{
  display: flex;
  flex-direction: column;
  /* gap: .5rem;   */
}
.box-bank{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr ;
  gap: .5rem;  
}
.box-form{
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: .5rem;  
}
.box-brgy{
  display: grid;
  grid-template-columns: .5fr 2.5fr ;
  gap: .5rem;  
}
.box-profile{
  display: grid;
  grid-template-columns: .2fr 3fr ;
}
.memberid:hover{
  color: rgb(230, 54, 54);
  transition: .1s;
  text-decoration: underline;
  cursor: pointer;
}
.table-scroll tbody tr:hover {
  background-color: lightblue;
  box-shadow: 2px 2px 16px 8px rgb(235, 233, 233);
  cursor: pointer;
  transition: all 0.3s ease-in-out;
}
.language-info{
  max-height: 50vh;
  overflow: auto;
}
</style>
