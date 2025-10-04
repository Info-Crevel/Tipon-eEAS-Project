<template>

<main role="app-wrapper" class="main-wrapper">

  <input type="checkbox" id="toggle" @change="tsMenuIcon+=1" ref="menu-toggle">


    <nav id="navbar" class=" collapse-flex show justify-evenly text-white">
      
      <div class="nav-menu  ">
           <div class="test">
              <div class="p-1 mb-1 mt-0 ">
            
              <button type="button" :class="logButtonClass" class="  warning border-main px-3 burger" @click="closeMenu"> 
                  <span class="bold">X</span> 
                </button>
            </div>
            </div>
            
          <ul class="nav-menu-content">
         
            <div class="header-content">

            
              <div class="img">
                <img :src="dom.getImageSource(sym.userInfo.photo)" alt="..." class="photo d-block mx-auto my-2 fw-28 circle border-main square-100" v-if="sym.userInfo.photo">
                <img src="../public/img/anonymous.png" width="110rem" height="110rem" v-if="!sym.userInfo.photo" class="photo d-block mx-auto my-2 fw-28 circle border-main square-100 " />
                <div class="user">
                  <span class="dropitem-text pt-1 text-narrow bold">{{ sym.userInfo.userName }}</span>
                  
                </div>
                
              </div>
            </div>
            <div class="footer-content">
              <div class="sidebar-footer " :key="tsUser">
                <button type="button" class="act-btns curved sm-1" tabindex="-1" @click.prevent="showPasswordRequestDialog">Change Password?</button>
                
                <button type="button" class="act-btns curved sm-1" tabindex="-1" @click.prevent="showOrgDialog" v-show="orgs.length !== 1">Change Cooperative?</button>
                
                <button type="button" class="act-btns curved sm-1" @click="signout"><i class="fa fa-lg fa-sign-out mr-2"></i>Logout</button>
              </div>
            </div>
            <div class="body-content">

              <div class="search-container">
                <input v-model="searchName" type="text" placeholder="Search Form Name" class="input-text" list="pageNames"  @change="onNameSelected" />
                <datalist id="pageNames"><option v-for="item in pages" :key="item.pageId" :value="item.pageName" @input="e => pageName = e.toUpperCase()" class="dropdown"></option></datalist> 
              </div>
              <div class="main-container-scroller">
                <div class="main-container-wrappaer">

                
                <div class="main-container" @click="toggleSection('isExpanded1')">
                  <div class="header mb-2">
                    <span class="menu-item  app-sub-content-font">FILE SYSTEM MAINTENANCE</span>
             
                      <span class="fa fa-play " :class="{ 'rotated' : isExpanded1 }" ></span >
              
                  </div>
                  
                
                  <div class="expanded" v-show="isExpanded1" >
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0020') && canAccessPage('dbs0020')" to="dbs0020" title="Platforms" @click.native="handleSymLinkClick">Business Platform Master</sym-link></li>
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0080') && canAccessPage('dbs0080')" to="dbs0080" title="Orgs" @click.native="handleSymLinkClick">Organization Platform Mapping</sym-link></li>
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0030') && canAccessPage('dbs0030')" to="dbs0030" title="Industries" @click.native="handleSymLinkClick">Industry Setup</sym-link></li>
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0580') && canAccessPage('dbs0580')" to="dbs0580" title="Request Types" @click.native="handleSymLinkClick">Request Type Setup</sym-link></li>
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0590') && canAccessPage('dbs0590')" to="dbs0590" title="Hiring Process Screen" @click.native="handleSymLinkClick">Hiring Process Screen</sym-link></li>
                    <li class="pl-4"><sym-link  v-show="sym.hasPageAccess('dbs0900') && canAccessPage('dbs0900')" to="dbs0900" title="Member Control" @click.native="handleSymLinkClick">Member Control</sym-link></li>
                  </div>

                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded1')" >
                        <span class="app-sub-content-font">Chart Of Accounts</span>
                        <span class="fa fa-play" :class="{ 'rotated': isSubExpanded1 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded1">
                      <li><sym-link v-show="sym.hasPageAccess('gls1000') && canAccessPage('gls1000')" to="gls1000" title="Accounts Structure" @click.native="handleSymLinkClick">Chart of Accounts Structure</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('gls0100') && canAccessPage('gls0100')" to="gls0100" title="Transaction Templates" @click.native="handleSymLinkClick">Template Setup</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('fin0100') && canAccessPage('fin0100')" to="fin0100" title="Chart of Accounts" @click.native="handleSymLinkClick">Chart of Accounts</sym-link></li>
                    </div>
                  </div>

                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded2')" >
                      <span class=" app-sub-content-font ">Address</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded2 }"></span >
                      
                    </div>  
                    <div class="expand" v-show="isSubExpanded2">
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0050') && canAccessPage('dbs0050')" to="dbs0050" title="Regions" @click.native="handleSymLinkClick">Region Master</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0060') && canAccessPage('dbs0060')" to="dbs0060" title="Provinces / Districts" @click.native="handleSymLinkClick">Province Definition</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0070') && canAccessPage('dbs0070')" to="dbs0070" title="Municipalities / Cities" @click.native="handleSymLinkClick">Municipality Setup</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content" v-show="isExpanded1">
                    <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded3')">
                      <span class=" app-sub-content-font ">Member Records</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded3 }"></span >
                    </div>
                    <div class="expand" v-show="isSubExpanded3">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0140') && canAccessPage('dbs0140')" to="dbs0140" title="Source of Application" @click.native="handleSymLinkClick">Source of Application</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0400') && canAccessPage('dbs0400')" to="dbs0400" title="CDA Membership Types" @click.native="handleSymLinkClick">CDA Membership Type</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0560') && canAccessPage('dbs0560')" to="dbs0560" title="Member Suffixes" @click.native="handleSymLinkClick">Member Suffix Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0010') && canAccessPage('dbs0010')" to="dbs0010" title="Religions" @click.native="handleSymLinkClick">Religion Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0040') && canAccessPage('dbs0040')" to="dbs0040" title="Relations" @click.native="handleSymLinkClick">Relation Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0100') && canAccessPage('dbs0100')" to="dbs0100" title="Disabilities" @click.native="handleSymLinkClick">Disability Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0120') && canAccessPage('dbs0120')" to="dbs0120" title="Member Type Qualifications" @click.native="handleSymLinkClick">Member Type Qualification Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0090') && canAccessPage('dbs0090')" to="dbs0090" title="Education Levels" @click.native="handleSymLinkClick">Education Level Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0150') && canAccessPage('dbs0150')" to="dbs0150" title="License Profession" @click.native="handleSymLinkClick">License Profession</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0210') && canAccessPage('dbs0210')" to="dbs0210" title="NCII Qualification Titles" @click.native="handleSymLinkClick">NCII Qualification Title</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0220') && canAccessPage('dbs0220')" to="dbs0220" title="Training Institutions" @click.native="handleSymLinkClick">Training Institution</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0230') && canAccessPage('dbs0230')" to="dbs0230" title="Assessment Centers" @click.native="handleSymLinkClick">Assessment Center</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0240') && canAccessPage('dbs0240')" to="dbs0240" title="Compliance Trainings" @click.native="handleSymLinkClick">Compliance Training</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0270') && canAccessPage('dbs0270')" to="dbs0270" title="School Definition" @click.native="handleSymLinkClick">School Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0280') && canAccessPage('dbs0280')" to="dbs0280" title="Course Definition" @click.native="handleSymLinkClick">Course Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0360') && canAccessPage('dbs0360')" to="dbs0360" title="Affiliation Definition" @click.native="handleSymLinkClick">Affiliation Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0370') && canAccessPage('dbs0370')" to="dbs0370" title="CDA Training" @click.native="handleSymLinkClick">CDA Training</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0410') && canAccessPage('dbs0410')" to="dbs0410" title="Languages" @click.native="handleSymLinkClick">Language Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0420') && canAccessPage('dbs0420')" to="dbs0420" title="RNR Recordings" @click.native="handleSymLinkClick">RNR Recording Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0430') && canAccessPage('dbs0430')" to="dbs0430" title="Revenue Qualification" @click.native="handleSymLinkClick">Revenue Qualification Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0440') && canAccessPage('dbs0440')" to="dbs0440" title="Non-Revenue Qualification" @click.native="handleSymLinkClick">Non-Revenue Qualification Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0450') && canAccessPage('dbs0450')" to="dbs0450" title="Vaccine Types" @click.native="handleSymLinkClick">Vaccine Type Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0460') && canAccessPage('dbs0460')" to="dbs0460" title="Medical Result Type Setup" @click.native="handleSymLinkClick">Medical Result Type Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0160') && canAccessPage('dbs0160')" to="dbs0160" title="Applicant Position" @click.native="handleSymLinkClick">Applicant Position</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded4')" >
                      <span class=" app-sub-content-font ">Internal Job Position Naming</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded4 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded4">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0480') && canAccessPage('dbs0480')" to="dbs0480" title="MRF Internal Position Setup" @click.native="handleSymLinkClick">MRF Internal Position Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0500') && canAccessPage('dbs0500')" to="dbs0500" title="Skills" @click.native="handleSymLinkClick">Skill Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0130') && canAccessPage('dbs0130')" to="dbs0130" title="Skill Set" @click.native="handleSymLinkClick">Skill Set Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0110') && canAccessPage('dbs0110')" to="dbs0110" title="Document Types" @click.native="handleSymLinkClick">Document Type Setup</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded5')" >
                      <span class=" app-sub-content-font ">Ratesheet Parameters</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded5 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded5">
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0340') && canAccessPage('dbs0340')" to="dbs0340" title="Allowances" @click.native="handleSymLinkClick">Allowance Setup</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0350') && canAccessPage('dbs0350')" to="dbs0350" title="Deminimis" @click.native="handleSymLinkClick">Deminimis Definition</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0130') && canAccessPage('dbs0130')" to="dbs0130" title="Skill Set" @click.native="handleSymLinkClick">Skill Set Setup</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0110') && canAccessPage('dbs0110')" to="dbs0110" title="Document Types" @click.native="handleSymLinkClick">Document Type Setup</sym-link></li>
                      <li><sym-link  v-show="sym.hasPageAccess('dbs0490') && canAccessPage('dbs0490')" to="dbs0490" title="Payroll Transactions" @click.native="handleSymLinkClick">Payroll Transaction Setup</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded6')" >
                      <span class=" app-sub-content-font ">Work Day And Holiday Setup</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded6 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded6">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0380') && canAccessPage('dbs0380')" to="dbs0380" title="Day Types" @click.native="handleSymLinkClick">Day Type Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0390') && canAccessPage('dbs0390')" to="dbs0390" title="Holidays" @click.native="handleSymLinkClick">Holiday Setup</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content"v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded7')" >
                      <span class=" app-sub-content-font ">Government Mandates, Contribution And Taxes</span>
                      <span class="fa fa-play " :class="{ 'rotated': isSubExpanded7 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded7">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0510') && canAccessPage('dbs0510')" to="dbs0510" title="Withholding Tax Setup" @click.native="handleSymLinkClick">Withholding Tax Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0520') && canAccessPage('dbs0520')" to="dbs0520" title="SSS Contribution" @click.native="handleSymLinkClick">SSS Contribution Schedule</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0530') && canAccessPage('dbs0530')" to="dbs0530" title="Pag-IBIG Contribution" @click.native="handleSymLinkClick">Pag-IBIG Contribution Table</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0540') && canAccessPage('dbs0540')" to="dbs0540" title="PhilHealth Contribution" @click.native="handleSymLinkClick">PhilHealth Contribution Table</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0570') && canAccessPage('dbs0570')" to="dbs0570" title="Region Minimum Wages" @click.native="handleSymLinkClick">Region Minimum Wage Setup</sym-link></li>
                    </div>
                  </div>
                  
                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded8')" >
                      <span class=" app-sub-content-font ">Saving Loans Benefits and Leaves</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded8 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded8">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0190') && canAccessPage('dbs0190')" to="dbs0190" title="In-House Benefits" @click.native="handleSymLinkClick">In-House Benefits</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0200') && canAccessPage('dbs0200')" to="dbs0200" title="Statutory Benefits" @click.native="handleSymLinkClick">Statutory Benefits</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0250') && canAccessPage('dbs0250')" to="dbs0250" title="Insurance Coverage" @click.native="handleSymLinkClick">Insurance Coverage</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0260') && canAccessPage('dbs0260')" to="dbs0260" title="Savings and Loans" @click.native="handleSymLinkClick">Savings and Loans</sym-link></li>
                    </div>
                  </div>

                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header pl-5 mb-2" @click.stop="toggleSection('isSubExpanded9')" >
                      <span class=" app-sub-content-font ">Contract Management</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded9 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded9">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0170') && canAccessPage('dbs0170')" to="dbs0170" title="Contract Type" @click.native="handleSymLinkClick">Contract Type</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0180') && canAccessPage('dbs0180')" to="dbs0180" title="Charging Consideration" @click.native="handleSymLinkClick"> Consideration</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0470') && canAccessPage('dbs0470')" to="dbs0470" title="Engagement Type Setup" @click.native="handleSymLinkClick">Engagement Type Setup</sym-link></li>
                    </div>
                  </div>

                  <div class="sub-content" v-show="isExpanded1">
                      <div class="sub-header  pl-5 mb-2"  @click.stop="toggleSection('isSubExpanded10')" >
                      <span class=" app-sub-content-font ">Others</span>
                      <span class="fa fa-play" :class="{ 'rotated': isSubExpanded10 }"></span >
                    </div>  
                    <div class="expand" v-show="isSubExpanded10">
                      <li><sym-link v-show="sym.hasPageAccess('dbs0290') && canAccessPage('dbs0290')" to="dbs0290" title="Bank Definition" @click="handleSymLinkClick">Bank Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0300') && canAccessPage('dbs0300')" to="dbs0300" title="Bank Locations" @click.native="handleSymLinkClick">Bank Location Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0310') && canAccessPage('dbs0310')" to="dbs0310" title="Member Transaction Types" @click.native="handleSymLinkClick">Member Transaction Type Setup</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0320') && canAccessPage('dbs0320')" to="dbs0320" title="Expense Charging" @click.native="handleSymLinkClick">Expense Charging Definition</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0330') && canAccessPage('dbs0330')" to="dbs0330" title="Inventory Release Charging" @click.native="handleSymLinkClick">Inventory Release Charging</sym-link></li>
                      <li><sym-link v-show="sym.hasPageAccess('dbs0920') && canAccessPage('dbs0920')" to="dbs0920" title="Cluster Definition" @click.native="handleSymLinkClick">Cluster Definition</sym-link></li>
                    </div>
                  
                  </div>  

                </div>
                

                <div class="main-container" @click="toggleSection('isExpanded2')">

                  <div class= " header mb-2">
                    <span class= "  menu-item  app-sub-content-font">CLIENT MANAGEMENT</span>
                    <span class="fa fa-play"  :class="{ 'rotated': isExpanded2 }"></span >
                  </div>

                  <div class="  pl-4" v-show="isExpanded2">
                    <li><sym-link  v-show="sym.hasPageAccess('ars0010') && canAccessPage('ars0010')" to="ars0010" title="Client Profile" @click.native="handleSymLinkClick">Client Profile</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0020') && canAccessPage('ars0020')" to="ars0020" title="Member Request Form " @click.native="handleSymLinkClick">Client Contract </sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0040') && canAccessPage('ars0040')" to="ars0040" title="Client Pay Group Setup" @click.native="handleSymLinkClick">Client Pay Group Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0030') && canAccessPage('ars0030')" to="ars0030" title="Member Request Form " @click.native="handleSymLinkClick">Member Request Form </sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0050') && canAccessPage('ars0050')" to="ars0050" title="Member Request Pooling" @click.native="handleSymLinkClick">Member Request Pooling</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0060') && canAccessPage('ars0060')" to="ars0060" title="Member Request Monitoring" @click.native="handleSymLinkClick">Member Request Monitoring</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0070') && canAccessPage('ars0070')" to="ars0070" title="Paygroup Member List" @click.native="handleSymLinkClick">Paygroup Member List</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0080') && canAccessPage('ars0080')" to="ars0080" title="Member Deduction" @click.native="handleSymLinkClick">Member Deduction</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0120') && canAccessPage('ars0120')" to="ars0120" title="MRF Transfer" @click.native="handleSymLinkClick">MRF Transfer</sym-link></li>
                  </div>
                </div>  

                <div class="main-container" @click="toggleSection('isExpanded3')">

                  <div class= " header mb-2">
                    <span class="  menu-item  app-sub-content-font">HUMAN RESOURCES</span>
                    <span class="fa fa-play" :class="{ 'rotated': isExpanded3 }"></span >
                  </div>

                  <div class="pl-4" v-show="isExpanded3">
                    
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0010') && canAccessPage('hrs0010')" to="hrs0010" title="Member Profile" @click.native="handleSymLinkClick">Member Profile</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0030') && canAccessPage('hrs0030')" to="hrs0030" title="Applicant Profile" @click.native="handleSymLinkClick">Applicant Profile</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0040') && canAccessPage('hrs0040')" to="hrs0040" title="Recruiter Setup" @click.native="handleSymLinkClick">Recruiter Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0050') && canAccessPage('hrs0050')" to="hrs0050" title="Member Employment Status Action" @click.native="handleSymLinkClick">Member Employment Status Action</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0060') && canAccessPage('hrs0060')" to="hrs0060" title="Member Employment Status Update" @click.native="handleSymLinkClick">Member Employment Status Update</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0070') && canAccessPage('hrs0070')" to="hrs0070" title="Member Engagement Monitoring" @click.native="handleSymLinkClick">Member Engagement Monitoring</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0080') && canAccessPage('hrs0080')" to="hrs0080" title="Member Government ID" @click.native="handleSymLinkClick">Member Government ID Monitoring</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('hrs0090') && canAccessPage('hrs0090')" to="hrs0090" title="Member Request Candidate List" @click.native="handleSymLinkClick">Member Request Candidate List</sym-link></li>
                  </div>
                </div>
                
                <div class="main-container" @click="toggleSection('isExpanded4')">

                  <div class= " header mb-2">
                    <span class=" menu-item  app-sub-content-font">TIMEKEEPING</span>
                    <span class="fa fa-play" :class="{ 'rotated': isExpanded4 }"></span >
                  </div>

                  <div class="pl-4" v-show="isExpanded4">
                    
                    <li><sym-link  v-show="sym.hasPageAccess('pay0010') && canAccessPage('pay0010')" to="pay0010" title="Policy" @click.native="handleSymLinkClick" >Policy</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('pay0020') && canAccessPage('pay0020')" to="pay0020" title="Schedule" @click.native="handleSymLinkClick">Schedule</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('pay0030') && canAccessPage('pay0030')" to="pay0030" title="Daily Time Record" @click.native="handleSymLinkClick">Daily Time Record</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('pay0040') && canAccessPage('pay0040')" to="pay0040" title="Daily Time Record Generation" @click.native="handleSymLinkClick">Daily Time Record Generation</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0090') && canAccessPage('ars0090')" to="ars0090" title="Member Pay Out" @click.native="handleSymLinkClick">Member Pay Out</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0100') && canAccessPage('ars0100')" to="ars0100" title="Client Billing" @click.native="handleSymLinkClick">Client Billing</sym-link></li>
                  </div>
                </div>


                <div class="main-container" @click="toggleSection('isExpanded5')">

                  <div class= " header mb-2">
                    <span class="  menu-item  app-sub-content-font">ACCOUNTING AND FINANCE MANAGEMENT</span>
                    <span class="fa fa-play " :class="{ 'rotated': isExpanded5 }"></span >
                  </div>
               

                  
                  <div class="pl-4 " v-show="isExpanded5">
                    
                    <li><sym-link  v-show="sym.hasPageAccess('pay0040') && canAccessPage('pay0040')" to="pay0040" title="Billing Invoice" @click.native="handleSymLinkClick">Billing Invoice</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('pay0040') && canAccessPage('pay0040')" to="pay0040" title="Receivables" @click.native="handleSymLinkClick">Receivables</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('fin0010') && canAccessPage('fin0010')" to="fin0010" title="Cash Receipt" @click.native="handleSymLinkClick">Cash Receipt</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('fin0020') && canAccessPage('fin0020')" to="fin0020" title="ATC Definition" @click.native="handleSymLinkClick">ATC Definition</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('fin0030') && canAccessPage('fin0030')" to="fin0030" title="Bank Setup" @click.native="handleSymLinkClick">Bank Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('fin0040') && canAccessPage('fin0040')" to="fin0040" title="Bank Setup" @click.native="handleSymLinkClick">Journal Entry</sym-link></li>

                    <li><sym-link  v-show="sym.hasPageAccess('pay0040') && canAccessPage('pay0040')" to="pay0040" title="Cash Receipt Entry" @click.native="handleSymLinkClick">Credit Memo</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0080') && canAccessPage('ars0080')" to="ars0080" title="Deduction Entry" @click.native="handleSymLinkClick">Deduction Entry</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0010') && canAccessPage('aps0010')" to="aps0010" title="Payee Type Setup" @click.native="handleSymLinkClick">Payee Type Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0020') && canAccessPage('aps0020')" to="aps0020" title="Payable Account Setup" @click.native="handleSymLinkClick">Payable Account Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0030') && canAccessPage('aps0030')" to="aps0030" title="Payable Tax Setup" @click.native="handleSymLinkClick">Payable Tax Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0040') && canAccessPage('aps0040')" to="aps0040" title="Payee Definition" @click.native="handleSymLinkClick">Payee Definition</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0100') && canAccessPage('aps0100')" to="aps0100" title="Payables" @click.native="handleSymLinkClick">Payables</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0500') && canAccessPage('aps0500')" to="aps0500" title="Payable Reviewer" @click.native="handleSymLinkClick">Payable Reviewer</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0510') && canAccessPage('aps0510')" to="aps0510" title="Payable Request Type" @click.native="handleSymLinkClick">Payable Request Type</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0520') && canAccessPage('aps0520')" to="aps0520" title="Payable Request Type Particulars" @click.native="handleSymLinkClick">Payable Request Type Particulars</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0530') && canAccessPage('aps0530')" to="aps0530" title="Payout Account Mapping" @click.native="handleSymLinkClick">Payout Account Mapping</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0540') && canAccessPage('aps0540')" to="aps0540" title="Request Doc Type Setup" @click.native="handleSymLinkClick">Request Doc Type Setup</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0550') && canAccessPage('aps0550')" to="aps0550" title="Payable Approver" @click.native="handleSymLinkClick">Payable Approver</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0560') && canAccessPage('aps0560')" to="aps0560" title="Payable Request Transaction Type" @click.native="handleSymLinkClick">Payable Request Transaction Type</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0570') && canAccessPage('aps0570')" to="aps0570" title="Payable Request Transaction Mapping" @click.native="handleSymLinkClick">Payable Request Transaction Mapping</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('ars0500') && canAccessPage('ars0500')" to="ars0500" title="Receivables" @click.native="handleSymLinkClick">Receivables</sym-link></li>
                    <li><sym-link  v-show="sym.hasPageAccess('aps0600') && canAccessPage('aps0600')" to="aps0600" title="Disbursment Request Form" @click.native="handleSymLinkClick">Disbursment Request Form</sym-link></li>
                  </div>
            
                </div>  
                </div>
              </div>
            </div>

            
          </ul>

          
          
            



      </div>

        <button
          type="button"
          class="burger   "
          @click="openMenu"
          v-show="sym.userInfo.isAuthenticated"
        >
          <i class="fa fa-lg fa-fw" :class="menuIcon"></i>
        </button>
     
        <div class="org-header  bold">
                <span >{{this.sym.userInfo.userOrgShortName }}</span>
        </div>

      <div class="nav-group gap">

        <div class="nav-group" v-show="sym.userInfo.isAuthenticated">
        <a href="/" class="home-leftXXX button app-orange outline border-0"><i class="fa fa-home fa-lg mr-1"></i> Home</a>
        </div>

        <div class="nav-group">
          <sym-link class="logon-link mx-1 button app-orange outline border-0" to='/logon' v-show="!sym.userInfo.isAuthenticated"><i class="fa fa-lg fa-sign-in pr-2"></i>Login</sym-link>
        </div>
      </div>

      

    </nav>

    <!-- Banner -->

    <section id="main-header" class="banner app-blue-light pos-relative shadow p-1">

    </section>

    <div id="main-content" class="light">
      <transition name="fade" mode="out-in" appear>
        <router-view>
        </router-view>
      </transition>
    </div>

  <!-- end content -->


  <div class="d-flex justify-end light">
    <sym-go-top id="gotop" buttonClass="go-top app-blue circle shadow border-white op-70">
    </sym-go-top>
  </div>

  <footer id="app-footer" class="app-blue-dark p-2">
    <p class="text-light mb-0 sm-1 text-center">© <span>{{ currentYear }}</span> TIPON. All Rights Reserved.</p>
  </footer>

  <auto-logout @timeout="onAutoLogout"></auto-logout>

  <sym-modal
    v-model="isAutoLogoutModalVisible"
    size="md"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="true"
    :dismissible="true"
    title="Session Expired"
    v-if="isAutoLogoutModalVisible"
    headerClass="info"
    dismissButtonClass="warning darken-3"
  >
    <div class="p-2">
      <div class="board box-border text-center beige mb-0">
        <h3 class="text-charcoal mb-3">You've Been<br>Logged Out</h3>
        <p class="lg-1">You were gone for too long</p>
        <button type="button" class="text-center w-100 info lg-3 p-1" @click="goLogonPage" autofocus>Log In</button>
      </div>
    </div>

  </sym-modal>

<!-- Change Password -->
<sym-modal
    v-model="isChangePasswordRequestVisible"
    size="sm"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isChangePasswordRequestVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <button class="close" @click="hideChangePasswordRequestDialog()"><i class="fa fa-times"></i></button>
      <div class="curved border-dark arcticblue curved p-3 text-centerXXX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-key fa-lg mr-2"></i>Change Password</p>
        </div>

        <div class="whitesmoke border-main p-2 curved">
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="oldPassword" caption="Old Password" captionClass="bold"  :modeControl="false"></sym-text>
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="newPassword" caption="New Password" captionClass="bold"  :modeControl="false" @changing="onNewPasswordChanging"></sym-text>
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="confirmPassword" caption="Confirm Password" captionClass="bold" :modeControl="false" @changing="onNewPasswordChanging"></sym-text>
          <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button type="submit" class="info" :disabled="!oldPassword || !newPassword || !confirmPassword" @click="onResetPassword()"><i class="fa fa-play fa-lg mr-2"></i> Set New Password</button>
          </div>

        </div>
      </div>
    </div>
</sym-modal>

<!-- Change Org -->
<sym-modal
    v-model="isChangeOrgVisible"
    size="md"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isChangeOrgVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <button class="close" @click="hideChangeOrgDialog()"><i class="fa fa-times"></i></button>
      <div class="curved border-dark arcticblue curved p-3 text-centerXXX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-users fa-lg mr-2"></i>Change Multipurpose Cooperative</p>
        </div>

        <div class="whitesmoke border-main p-2 curved">
         
          <sym-combo v-model="orgId" caption="Name" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
           
         <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button type="submit" class="info" @click="onSubmitOrg()"><i class="fa fa-play fa-lg mr-2"></i> Submit</button>
          </div>

     

        </div>

       
      </div>
    </div>
</sym-modal>
</main>


</template>

<script>

import
  AutoLogout
from './comp/AutoLogout.vue';

import {
  logoff
} from './js/dbSys';

import {
  get,
  ajax
} from './js/http';

import
  PageBase
from './page/PageBase.vue';


export default {
  extends: PageBase,
  components: {
    'auto-logout': AutoLogout
  },

  provide () {
    return {
      theme: this.core.config.theme
    };
  },

  data () {
    return {
      ts: 0,
      tsUser: 0,
      tsMenuIcon: 0,
      dateInfo: this.sym.dateInfo,
      userCode: '',
      currentYear: 2022,
      drilldownIconclass: 'fa-caret-down',
      matchMediaQuery: '(max-width: 1000px)',
      isAutoLogoutModalVisible: false,
      isChangePasswordRequestVisible: false,
      oldPassword: '',
      accessId: '',
      phoneNumberEndDigits: '',
      newPassword: '',
      confirmPassword: '',
      isChangeOrgVisible: false,
      isExpanded1: false,
      isExpanded2: false,
      isExpanded3: false,
      isExpanded4: false,
      isExpanded5: false,
      isExpanded6: false,
      isSubExpanded1: false,
      isSubExpanded2: false,
      isSubExpanded3: false,
      isSubExpanded4: false,
      isSubExpanded5: false,
      isSubExpanded6: false,
      isSubExpanded7: false,
      isSubExpanded8: false,
      isSubExpanded9: false,
      isSubExpanded10: false,
      orgId: 0,
      orgShortName: '',
      orgs: [],
      pages: [],
      searchName:'',
    };
  },
  computed: {
    menuIcon() {
      return this.isMenuOpen ? "fa-close" : "fa-bars";
    },
    

   
  },
  methods: {

    onNameSelected() {

      const selected = this.pages.find(
        item => item.pageName.toLowerCase() === this.searchName.toLowerCase()
      );
      if (selected) {
        this.selectedPageId = selected.pageId;

      const me = this;

      let route = {
        name: selected.pageId,
      
      };
      me.go(route);
        me.searchName = '';
        me.closeMenu();


      } else {
        this.selectedPageId = null; 
      }
    },
 
    canAccessPage (pageId) {
      if (!pageId) {
        return false;
      }
      return true;
    },
    handleSymLinkClick() {

    this.isExpanded1 = false;
    this.isExpanded2 = false;
    this.isExpanded3 = false;
    this.isExpanded4 = false;
    this.isExpanded5 = false;
    this.isSubExpanded1 = false,
    this.isSubExpanded2 = false,
    this.isSubExpanded3 = false,
    this.isSubExpanded4 = false,
    this.isSubExpanded5 = false,
    this.isSubExpanded6 = false,
    this.isSubExpanded7 = false,
    this.isSubExpanded8 = false,
    this.isSubExpanded9 = false,
    this.isSubExpanded10 = false,

    this.closeMenu();
  },


    onSubmitOrg() {
      const me = this;
      
      me.sym.userInfo.userOrgId = me.orgId;
      let o = me.orgs.find( o => o.orgId == me.orgId);
      if (o) {
        me.sym.userInfo.userOrgShortName = o.orgShortName
      }

      me.updateUserOrg()
        .then(() => {
          
          me.isChangeOrgVisible = false;
          me.closeMenu();
          me.isExpanded1 = false;
          me.isExpanded2 = false;
          me.isExpanded3 = false;
          me.isExpanded4 = false;
          me.isExpanded5 = false;
          me.isSubExpanded1 = false;
          me.isSubExpanded2 = false;
          me.isSubExpanded3 = false;
          me.isSubExpanded4 = false;
          me.isSubExpanded5 = false;
          me.isSubExpanded6 = false;
          me.isSubExpanded7 = false;
          me.isSubExpanded8 = false;
          me.isSubExpanded9 = false;
          me.isSubExpanded10 = false;
          me.refresh();    
      })
     

    },


    onNewPasswordChanging(e) {
      e.callback = this.newPasswordCallback;
    },

    newPasswordCallback(e) {
    
      e.message ="";
      if (e.proposedValue < 8) {
      e.message = "Password must be at least 8 characters long.";
      return false;
      }

      if (!/[A-Z]/.test(e.proposedValue)) {
        e.message = "Password must contain at least one uppercase letter.";
        return false;
      }

      if (!/\d/.test(e.proposedValue)) {
        e.message = "Password must contain at least one number.";
        return false;
      }

    return true;

    },

    isValidAccessPassword () {
      if (this.accessId < 0) {
        return Promise.resolve(null);
      }
      return get('api/passwords/validate?accessId=' + this.accessId + '&password=' + this.oldPassword);
    },

    updateUserOrg () {
         const
         me = this,
        options = this.core.getAjaxOptions('PUT');
        return ajax('api/user-org/'+ me.sym.userInfo.token + '/'+ me.orgId, options);
    },

    onResetPassword () {
      const me = this;

      me.isValidAccessPassword().then(

      result => {

        if (result===0) {
          me.advice.fault("Old Password does not match.", { duration: 5 });
          return;
        }



      if (me.oldPassword == me.newPassword) {
        me.advice.fault("You cannot use your old password as the new one!");
        me.setFocus('newPassword');
        return;
      }

      if (me.newPassword !== me.confirmPassword) {
        me.advice.fault("New and confirm password does not match!");
        me.setFocus('newPassword');
        return;
    }

      if (!me.newPassword) {
        me.advice.fault("New Password", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('newPassword');
        return;
      }

      let wait = me.wait();

      me.resetNewPassword().then(
        success => {
          me.stopWait(wait);
          if (success) {
            me.dialog.success("Password changed.<br><br>You may try logging on now to check.", { size: 'sm'}).then(
              () => {
                me.isChangePasswordRequestVisible = false;
                me.signout()
                me.$nextTick( () => {
                  setTimeout(() => {
                    me.setFocus('logonId');
                  }, 200);
                });
              }
            );
          } else {
            me.dialog.fault("Attempt to set new password failed.<br><br>Make sure you have correct credentials.", { size: 'sm'});
          }

        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
      }
    );



    },

    resetNewPassword () {
      const
        me = this;

      let payload = {
        accessId: me.accessId,
        oldPassword: me.oldPassword,
        newPassword: me.newPassword
      };

      const
        body = JSON.stringify(payload),
        options = me.core.getAjaxOptions('PUT', body);
      return ajax('api/passwords/reset-default', options);
    },

    hideChangePasswordRequestDialog () {
      const me = this;
      me.isChangePasswordRequestVisible = false;
    },

    hideChangeOrgDialog () {
      const me = this;
      me.isChangeOrgVisible = false;
    },


    getInputKeyType () {
      return this.showPassword ? 'text' : 'password';
    },

    showPasswordRequestDialog () {
      const me = this;
      me.accessId = me.sym.userInfo.logOnId;
      me.oldPassword = '';
      me.newPassword = '';
      me.confirmPassword = '';
      me.isChangePasswordRequestVisible = true;
    },

    showOrgDialog () {
      const me = this;
      me.orgs = me.sym.userInfo.orgs;
      me.orgId = me.sym.userInfo.userOrgId;
      me.isChangeOrgVisible = true;
    },


    signout () {
      const
        me = this,
        wait = me.dialog.wait(),
        userName = me.sym.userInfo.userName;
      me.closeMenu();
      this.isExpanded1 = false;
      this.isExpanded2 = false;
      this.isExpanded3 = false;
      this.isExpanded4 = false;
      this.isExpanded5 = false;
      this.isSubExpanded1 = false,
      this.isSubExpanded2 = false,
      this.isSubExpanded3 = false,
      this.isSubExpanded4 = false,
      this.isSubExpanded5 = false,
      this.isSubExpanded6 = false,
      this.isSubExpanded7 = false,
      this.isSubExpanded8 = false,
      this.isSubExpanded9 = false,
      this.isSubExpanded10 = false,
          
      logoff().then(
        () => {
          me.dialog.stop(wait);
          me.sym.userInfo.update();
          me.core.navigate('home', true);
          me.advice.success(userName, { title: 'Logout complete', enclosed: true, alignment: 'bottom-right', duration: 4, dismissible: true });
          me.sym.userInfo.userOrgId = 0;
          me.sym.userInfo.userOrgShortName = '';
          me.userCode = '';
          me.refreshUserTag();
          window.scrollToTop();
          me.eventBus.$emit('user:logoff');
          me.tsUser = Date.now();
        },
        () => {
          me.dialog.stop(wait);
        }
      );
    },

    openPortal () {
      this.core.navigate('/menu');
    },

    onUserInfoDropDown (show) {
      if (show) {
        this.tsUser = Date.now();
      }
    },

    refreshUserTag () {
      this.ts +=1;
    },

    onResize () {
      this.ts = this.ts + 1;
    },

    getDrilldownIcon (direction) {
      if (direction) {
        if (direction === 'R') { return 'fa-chevron-right'; }
        if (direction === 'L') { return 'fa-chevron-left'; }
      }

      const mql = window.matchMedia(this.matchMediaQuery);
      if (mql.matches) {
        return 'fa-chevron-right';
      } else {
        return 'fa-chevron-down';
      }
    },

    getToggleIconClass () {
      let el = document.getElementById("toggle");
      if (el && el.checked  ) {
        return 'fa-close';
      }
      return 'fa-bars';
    },

    refreshMenu () {
      const mql = window.matchMedia(this.matchMediaQuery);
      if (mql.matches) {
        this.ts +=1;
      }
    },

    closeMenu () {
      const
        me = this,
        mql = window.matchMedia(me.matchMediaQuery);
        me.isExpanded1 = false;
        me.isExpanded2 = false;
        me.isExpanded3 = false;
        me.isExpanded4 = false;
        me  .isExpanded5 = false;
      if (mql.matches) {
        setTimeout(() => {
          me.$refs['menu-toggle' ].checked = false;
          me.tsMenuIcon += 1;
        }, 30);
       
      } else {
        let menus = document.querySelectorAll('.nav-menu ul ul');
        menus.forEach(el => {
          el.classList.add('d-none');
          setTimeout(() => {
            el.classList.remove('d-none');
          }, 100);
          
        });
      }
      
          
    },

    addNavLinkListener () {
      const navLinks = document.querySelectorAll(".nav-link");
      navLinks.forEach(el => el.addEventListener("click", this.closeMenu));
    },

    removeNavLinkListener () {
      const navLinks = document.querySelectorAll(".nav-link");
      navLinks.forEach(el => el.removeEventListener("click", this.closeMenu));
    },

    onAutoLogout () {
      const me = this;
      me.signout();
      me.isAutoLogoutModalVisible = true;
    },

    goLogonPage () {
      const me = this;

      me.core.navigate('logon');
      me.isAutoLogoutModalVisible = false;
    },

    openMenu() {
      document.querySelector(".nav-menu").classList.add("open");


      this.pages = this.sym.userInfo.pages;
      this.orgs = this.sym.userInfo.orgs;
      this.refresh();
 
    },

    closeMenu() {
      document.querySelector(".nav-menu").classList.remove("open");
    },

    toggleSection(key) {
    this[key] = !this[key];
  }
  },

  created () {
    const me = this; 

    me.dom.on(window, 'resize', me.onResize);
    me.accessId = me.sym.userInfo.logOnId;
    me.eventBus.$on('user:logon', () => {
    me.userCode = me.sym.userInfo.userCode;
    me.refreshUserTag();
    });
    me.userOrgShortName = me.sym.userInfo.userOrgShortName;
    me.pages = me.sym.userInfo.pages;


  },

  updated () {
    this.removeNavLinkListener();
    this.addNavLinkListener();
    

  },

  beforeDestroy () {

    this.dom.off(window, 'resize', this.onResize);
    this.removeNavLinkListener();
  },

  mounted () {
    const me = this;

    me.currentYear = me.core.getCurrentYear();
    me.userCode = me.sym.userInfo.userCode;
    me.refresh();

    if (me.sym.userInfo.isAuthenticated) {
      me.eventBus.$emit('user:logon');
     
    }

    me.$nextTick( () => {
      me.addNavLinkListener();
    });
    const navLinks = document.querySelectorAll(".nav-item");
    navLinks.forEach((el) => el.addEventListener("click", me.closeMenu));
  },
  beforeDestroy() {
    const navLinks = document.querySelectorAll(".nav-item");
    navLinks.forEach((el) => el.removeEventListener("click", this.closeMenu));
  },
  
};



</script>

<style scoped>

button.btn-go-top {
  background: transparent;
}

.banner {
  padding: .5rem;

  display: grid;
  grid-template-columns: 1fr 280px;
  gap: 1rem;
}

.banner-info {
  display: grid;
  grid-template-columns: 400px 1fr;
}

.logo-lgu {
  width: 100px;
  margin-right: .75rem;
  opacity: .85;
  display: block;
}

.phils {
  font-size: 1.0625rem;
  line-height: 1.6;
  border-bottom: 1px solid #36454f;
}

.lgu {
  font-size: 2.125rem;
  color: black;
  opacity: 0.75;
  margin-top: .25rem;
  line-height: 1.2;
}

.motto {
  font-size: 1.25rem;
  margin-top: -.25rem;
}

.motto-init {
  font-size: 1.25rem;
}

.motto.narrow {
  display: none;
}

#gotop {
  padding-top: .625rem;
  padding-bottom: .625rem;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}

.fade-enter, .fade-leave-to {
  opacity: 0;
}

#navbar {
  position: sticky;
  background: var(--app-blue);

  z-index: 1000;
  /* top: 0; */

  padding: .25rem;
  height: 45px;
}

#navbar >>> .photo {
  object-fit: cover;

}



.burger {
  display: none;
  align-items: center;
  
}

.logon-link {
  color: whitesmoke;
  text-decoration: none;
}
.org-header{

  padding: 10px;
  text-transform: uppercase;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;


}

.main-container,
.sub-content {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  width: 100%;
  transition: background-color 0.3s ease;
  text-wrap: wrap;
}
.main-container{
  overflow: auto;
}

.small-screen {
  display: none;
}

input.submenus,
input#toggle {
  position: absolute;
  display: none;
}

.nav-menu ul {
  margin: 0;
  padding: 0;
  list-style: none;
  white-space: nowrap;
  text-align: left;
}


.menu-holder {
  display: flex;
  align-items: center;
  position: sticky;
}

.main-container-scroller{
  overflow-y: auto;
  /* border: 1px solid white; */
}
.main-container-wrappaer{
  height: auto;
  max-height: 70vh;
}
.search-container{
  display: flex;
  justify-content: center;
  align-items: center;
  /* width: 50%; */
  padding: 10px;
}
.nav-menu {
    position: fixed;
    top: 0;
    left: -50%;
    width: 25%;
    height: 100%;
  
    background:rgba(17, 66, 121, .9) ;
    color: white;
    overflow-y: auto;
    transition: left 0.3s ease;
    box-shadow: -3px 0 10px rgba(0, 0, 0, 0.3);
    z-index: 1000;
  }
.act-btns{
  display: block;
  width: 100%;
  padding: .5rem .75rem;
  clear: both;
  text-align: inherit;
  white-space: nowrap;
  border: 0;
  color: inherit;
  background: inherit;
  border-radius: 5px;
}
.act-btns:hover{
  background-color: #e79b44;
}
.sidebar-footer{
  display: flex;
  /* flex-direction: column; */
  /* justify-content: center;
  align-content: center; */
  /* height: 50%; */
 
}
  .nav-menu.open {
    left: 0; 
  }

.nav-menu-content {
  width: 100%;
  padding: 0;
  margin: 0;
  
  color: #ecf0f1;
  font-family: 'Segoe UI', sans-serif;
  transition: all 0.3s ease;
  overflow-y: hidden;
  max-height: 90%;
  display: flex;
  flex-direction: column;
}
.nav-menu-content .footer-content{
  justify-content:flex-end;
}
.header {
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  font-weight: bold;
  background-color: #173c69; 
  box-shadow: 0 2px 4px rgb(22, 22, 22);
}
.sub-header {
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;

  background-color: #173c69; 
  box-shadow: 0 2px 4px rgb(22, 22, 22);
}
.fa.rotated {
  transform: rotate(90deg);
  transition: transform 0.3s ease;
}
.test{
  display: flex;
  justify-content: flex-end;
  padding: 10px;
}
.expand {
  list-style: none;
  margin: 0;
  padding-left: 40px;
  transition: 0.5s ease;
}
.expand li {
  margin: 5px 0;
}
 li:focus{
  background-color: red;
}
.expand sym-link {
  display: block;
  color: #bdc3c7;
  padding: 5px 0;
  transition: color 0.2s ease;

}
.expand sym-link:hover {
  color: #1abc9c;
}
.active-header {
  background-color: #1abc9c !important;
}
.active-link {
  color: #1abc9c !important;
  font-weight: bold;
}
.user{
  display: flex;
  justify-content: center;
  align-items: center;
 
}
.user span{
  color: white;
  font-size: 17px;

}
  .nav-item {
    width: 100%;
    border-top: 1px solid gray;
    border-bottom: 1px solid gray;
  }

  .link-icon {
    margin-right: 0.5rem;
  }

  .burger {
    display: flex;
    background-color: inherit;
    border: 0;
  }
  .burger:hover{
    background-color: #e79b44;
  }

  .close-btn {
    border-bottom: 1px solid gray;
  }
.nav-menu li {
  display: block;
  position: relative;
}



.nav-menu ul ul ul {left:100%; top:auto; margin-top:-51px; border-radius:5px;}

.nav-menu ul li.left > ul {left:auto; right:0; top:41px;}

.nav-menu ul li.left ul li > ul {left:auto; right:100%; top:auto; margin-top:-51px;}

.nav-menu > ul {margin:0 auto;}
.nav-menu > ul > li {float:left; position:relative;}

.nav-menu a {
  display: block;
  color: whitesmoke !important;
  text-decoration: none;
  padding: 0 20px;
  position: relative;
  z-index: 10;
  line-height: 2.25rem;
  font-size: 17px;
}

.nav-menu > ul > li > label {
  display:block;
}

.nav-menu > ul > li > a > label {
  margin-bottom: 0;
}

.nav-menu ul > li > a > label:hover {
  cursor: pointer;
}

.nav-menu ul ul > li > label {
  display:block;
  position:relative;
  margin-top:-25px;
  width:100%;
  height:25px;
  z-index:100;

  -webkit-transition: 0s .3s;
  -o-transition: 0s .3s;
  -moz-transition: 0s .3s;
  transition: 0s .3s;
}

.nav-menu ul li.back {display:none;}

.nav-menu li:hover > label {width:0;}

.nav-menu li.left ul a {text-align:right; padding:0 20px;}

.nav-menu li:hover > a,
.nav-menu ul li:hover > a {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

.nav-menu ul li:hover > a.hassub {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

.nav-menu li.left ul li:hover > a.hassub {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

/* .nav-menu > ul {*display:inline} */

.nav-menu ul ul,
.nav-menu ul ul ul,
.nav-menu ul li.left > ul,
.nav-menu ul li.left ul li > ul {left:-9999px; right:auto; opacity:0;}

.nav-menu ul li:hover > ul {left:0; opacity:1;}
.nav-menu ul ul li:hover > ul {left:100%; opacity:1;}
.nav-menu ul li.left:hover > ul {left:auto; right:0; opacity:1;}
.nav-menu ul li.left ul li:hover > ul {left:auto; right:100%; opacity:1;}

.drill-icon {
  margin-top: .5rem;
}

.nav-menu li.menu-close {
  display: none;
}

.home a {
  padding-left: 12px;
}
.sub-header {
  cursor: pointer;
  padding: 8px;

}


.name{
  color: white;
}

.main-wrapper {
  display: flex;
  flex-flow: column;
  height: 100%;
}

#main-headerXXX {
  height: 116px;

}

#main-content {
  flex: 1 1 auto;

}

#app-footer {
  height: 34px;
}

@media (max-width: 3840px), (max-height: 2160px) {
  .main-container-wrappaer {
    max-width: 70vh;
  }
}
@media (max-width: 2560px), (max-height: 1440px) {
  .main-container-wrappaer {
    max-height: 100vh;
  }
}
@media (max-width: 1920px), (max-height: 1080px) {
  .main-container-wrappaer {
    max-width: 70vh;
  }
}
@media (max-width: 1280px), (max-height: 720px) {
  .main-container-wrappaer {
    
    max-height: 60vh;
  }
}
@media(max-width: 900px){
  .nav-menu ul {
  margin: 0;
  padding: 0;
  list-style: none;
  white-space: nowrap;
  text-align: left;
}
.nav-menu {
    position: fixed;
    top: 0;
    left: -100%; 
    width: 100%;
    height: 100%;
   
    background:rgba(17, 66, 121, .9) ;
    color: white;
    overflow-y: auto;
    transition: left 0.3s ease;
    box-shadow: -3px 0 10px rgba(0, 0, 0, 0.3);
    z-index: 1000;
}

.nav-menu.open {
    left: 0; 
}

.nav-menu li {
  display: block;
  position: relative;
}

.nav-menu a {
  display: block;
  color: whitesmoke !important;
  text-decoration: none;
  padding: 0 20px;
  position: relative;
  z-index: 10;
  line-height: 2.25rem;
  font-size: 17px;
}

.nav-menu li.menu-close {
  display: none;
}

.nav-menu-content {
  width: 100%;
  padding: 0;
  margin: 0;
  
  color: #ecf0f1;
  font-family: 'Segoe UI', sans-serif;
  transition: all 0.3s ease;
  overflow-y: hidden;
  height: 100%;
}


.main-container-scroller{
  overflow-y: auto;
 
}
.main-container-wrappaer{
  height: 70vh;
}
.test{
  display: flex;
  justify-content: flex-end;
  padding: 10px;
}

.expand {
  list-style: none;
  margin: 0;
  padding-left: 40px;
  transition: 0.5s ease;
}
.expand li {
  margin: 5px 0;
}
 li:focus{
  background-color: red;
}
.expand sym-link {
  display: block;
  color: #bdc3c7;
  padding: 5px 0;
  transition: color 0.2s ease;

}
.expand sym-link:hover {
  color: #1abc9c;
}
.active-header {
  background-color: #1abc9c !important;
}
.active-link {
  color: #1abc9c !important;
  font-weight: bold;
}

.nav-item {
    width: 100%;
    border-top: 1px solid gray;
    border-bottom: 1px solid gray;
  }

  .link-icon {
    margin-right: 0.5rem;
  }

  .burger {
    display: flex;
    background-color: inherit;
    border: 0;
  }
  .burger:hover{
    background-color: #e79b44;
  }

  .close-btn {
    border-bottom: 1px solid gray;
  }
  .sidebar-footer{
  display: flex;
  justify-content: space-between;
  }
  .org-header{
  /* margin-right: 150px; */
  padding: 10px;
  text-transform: uppercase;
  display: flex;

  text-align: center;
  width: 100%;

  /* border: 1px solid white; */
}
}
@media(max-width: 400px){
  .nav-menu ul {
  margin: 0;
  padding: 0;
  list-style: none;
  white-space: nowrap;
  text-align: left;
}
.nav-menu {
    position: fixed;
    top: 0;
    left: 100%; 
    width: 100%;
    height: 100%;
   
    background:rgba(17, 66, 121, .9) ;
    color: white;
    overflow-y: auto;
    transition: left 0.3s ease;
    box-shadow: -3px 0 10px rgba(0, 0, 0, 0.3);
    z-index: 1000;
}

.nav-menu.open {
    left: 0; 
}

.nav-menu li {
  display: block;
  position: relative;
}

.nav-menu a {
  display: block;
  color: whitesmoke !important;
  text-decoration: none;
  padding: 0 20px;
  position: relative;
  z-index: 10;
  line-height: 2.25rem;
  font-size: 17px;
}

.nav-menu li.menu-close {
  display: none;
}

.nav-menu-content {
  width: 100%;
  padding: 0;
  margin: 0;
  
  color: #ecf0f1;
  font-family: 'Segoe UI', sans-serif;
  transition: all 0.3s ease;
  overflow-y: hidden;
  height: 100%;
}
.main-container-scroller{
  overflow-y: auto;
 
}
.main-container-wrappaer{
  height: 57vh;
}
.test{
  display: flex;
  justify-content: flex-end;
  padding: 10px;
}

.expand {
  list-style: none;
  margin: 0;
  padding-left: 40px;
  transition: 0.5s ease;
}
.expand li {
  margin: 5px 0;
}
 li:focus{
  background-color: red;
}
.expand sym-link {
  display: block;
  color: #bdc3c7;
  padding: 5px 0;
  transition: color 0.2s ease;

}
.expand sym-link:hover {
  color: #1abc9c;
}
.active-header {
  background-color: #1abc9c !important;
}
.active-link {
  color: #1abc9c !important;
  font-weight: bold;
}

.nav-item {
    width: 100%;
    border-top: 1px solid gray;
    border-bottom: 1px solid gray;
  }

  .link-icon {
    margin-right: 0.5rem;
  }

  .burger {
    display: flex;
    background-color: inherit;
    border: 0;
  }
  .burger:hover{
    background-color: #e79b44;
  }

  .close-btn {
    border-bottom: 1px solid gray;
  }
  .sidebar-footer{
  display: flex;
  justify-content: space-between;
  }
  .org-header{
  /* margin-right: 150px; */
  padding: 10px;
  text-transform: uppercase;
  display: flex;

  text-align: center;
  width: 100%;

  /* border: 1px solid white; */
}
}
 
</style>
