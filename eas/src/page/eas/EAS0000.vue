// Portal (Menu)

<template>

 <div class="menu container curved-1 border-light">
  <div class="info text-center p-2 serif lg-3 curved-top-2">
    TEAM Menu
  </div>

  <div class="menu-content info-light" ref="menu-content">


    <sym-card id="database" class="beige shadow border-main">
      <div slot="header">
        <span class="menu-item bold serif lg-3">FILE SYSTEM MAINTENANCE</span>
      </div>

      <ul class="menu-item serifXXX">
        <li><sym-link :class="listItemClass('dbs0020')" to="dbs0020" title="Platforms">Business Platform Master</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0080')" to="dbs0080" title="Orgs">Organization Platform Mapping</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0030')" to="dbs0030" title="Industries">Industry Setup</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0580')" to="dbs0580" title="Request Types">Request Type Setup</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0900')" to="dbs0900" title="Member Control">Member Control</sym-link></li>
        <li class="list-item-separator"></li>
        <sym-card id="database" class="beige shadow border-main"> <!--chart of accounts-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">CHART OF ACCOUNTS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="  ('gls1000')" to="gls1000" title="Accounts Structure">Chart of Accounts Structure</sym-link></li>
            <li><sym-link :class="listItemClass('gls0100')" to="gls0100" title="Transaction Templates">Template Setup</sym-link></li>
            <li><sym-link :class="listItemClass('gls0010')" to="gls0010" title="COA">Chart of Accounts</sym-link></li>
            
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--address-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">ADDRESS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0050')" to="dbs0050" title="Regions">Region Master</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0060')" to="dbs0060" title="Provinces / Districts">Province Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0070')" to="dbs0070" title="Municipalities / Cities">Municipality Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Member Records-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">MEMBER RECORDS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0140')" to="dbs0140" title="Source of Application">Source of Application</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0400')" to="dbs0400" title="CDA Membership Types">CDA Membership Type</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0560')" to="dbs0560" title="Member Suffixes">Member Suffix Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0010')" to="dbs0010" title="Religions">Religion Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0040')" to="dbs0040" title="Relations">Relation Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0100')" to="dbs0100" title="Disabilities">Disability Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0120')" to="dbs0120" title="Member Type Qualifications">Member Type Qualification Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0090')" to="dbs0090" title="Education Levels">Education Level Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0150')" to="dbs0150" title="License Profession">License Profession</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0210')" to="dbs0210" title="NCII Qualification Titles">NCII Qualification Title</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0220')" to="dbs0220" title="Training Institutions">Training Institution</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0230')" to="dbs0230" title="Assessment Centers">Assessment Center</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0240')" to="dbs0240" title="Compliance Trainings">Compliance Training</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0270')" to="dbs0270" title="School Definition">School Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0280')" to="dbs0280" title="Course Definition">Course Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0360')" to="dbs0360" title="Affiliation Definition">Affiliation Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0370')" to="dbs0370" title="CDA Training">CDA Training</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0410')" to="dbs0410" title="Languages">Language Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0420')" to="dbs0420" title="RNR Recordings">RNR Recording Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0430')" to="dbs0430" title="Revenue Qualification">Revenue Qualification Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0440')" to="dbs0440" title="Non-Revenue Qualification">Non-Revenue Qualification Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0450')" to="dbs0450" title="Vaccine Types">Vaccine Type Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0460')" to="dbs0460" title="Medical Result Type Setup">Medical Result Type Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0160')" to="dbs0160" title="Applicant Position">Applicant Position</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Internal Job Position Naming-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">INTERNAL JOB POSITION NAMING</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0480')" to="dbs0480" title="MRF Internal Position Setup">MRF Internal Position Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0500')" to="dbs0500" title="Skills">Skill Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0130')" to="dbs0130" title="Skill Set">Skill Set Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0110')" to="dbs0110" title="Document Types">Document Type Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Ratesheet Parameters-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">RATESHEET PARAMETERS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0340')" to="dbs0340" title="Allowances">Allowance Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0350')" to="dbs0350" title="Deminimis">Deminimis Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0130')" to="dbs0130" title="Skill Set">Skill Set Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0110')" to="dbs0110" title="Document Types">Document Type Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0490')" to="dbs0490" title="Payroll Transactions">Payroll Transaction Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Work Day and holiday Setup-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">WORK DAY AND HOLIDAY SETUP</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0380')" to="dbs0380" title="Day Types">Day Type Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0390')" to="dbs0390" title="Holidays">Holiday Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Goverment Mandates, Contribution and Taxes-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">GOVERNMENT MANDATES, CONTRIBUTION AND TAXES</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0510')" to="dbs0510" title="Withholding Tax Setup">Withholding Tax Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0520')" to="dbs0520" title="SSS Contribution">SSS Contribution Schedule</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0530')" to="dbs0530" title="Pag-IBIG Contribution">Pag-IBIG Contribution Table</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0540')" to="dbs0540" title="PhilHealth Contribution">PhilHealth Contribution Table</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0570')" to="dbs0570" title="Region Minimum Wages">Region Minimum Wage Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Savings, Loans, Benefits and Leaves-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">SAVINGS, LOANS, BENEFITS AND LEAVES</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0190')" to="dbs0190" title="In-House Benefits">In-House Benefits</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0200')" to="dbs0200" title="Statutory Benefits">Statutory Benefits</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0250')" to="dbs0250" title="Insurance Coverage">Insurance Coverage</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0260')" to="dbs0260" title="Savings and Loans">Savings and Loans</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Contract Management-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">CONTRACT MANAGEMENT</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0170')" to="dbs0170" title="Contract Type">Contract Type</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0180')" to="dbs0180" title="Charging Consideration"> Consideration</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0470')" to="dbs0470" title="Engagement Type Setup">Engagement Type Setup</sym-link></li>
          </ul>
        </sym-card>
        <sym-card id="database" class="beige shadow border-main"> <!--Others-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">OTHERS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0290')" to="dbs0290" title="Bank Definition">Bank Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0300')" to="dbs0300" title="Bank Locations">Bank Location Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0310')" to="dbs0310" title="Member Transaction Types">Member Transaction Type Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0320')" to="dbs0320" title="Expense Charging">Expense Charging Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0330')" to="dbs0330" title="Inventory Release Charging">Inventory Release Charging</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0910')" to="dbs0910" title="Work Schedules">Work Schedules</sym-link></li>
          </ul>
        </sym-card>

      


        


        
        

        
        


        
        
        <!-- <li><sym-link :class="listItemClass('dbs0550')" to="dbs0550" title="Payroll Variables">Payroll Variable Definition</sym-link></li> -->
        
        
        


        

        <!-- <li class="list-item-separator"></li> -->


        <!-- <li><sym-link :class="listItemClass('dbs0110')" to="dbs0110" title="Collecting Officer Setup">Collecting Officers</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0120')" to="dbs0120" title="Disbursing Officer Setup">Disbursing Officers</sym-link></li>
        <li class="list-item-separator"></li>

        <li><sym-link :class="listItemClass('dbs0130')" to="dbs0130" title="Payor Definition">Payors</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0140')" to="dbs0140" title="Payee Definition">Payees</sym-link></li>
        <li class="list-item-separator"></li>

        <li><sym-link :class="listItemClass('dbs0150')" to="dbs0150" title="Signatory Setup">Signatory Setup</sym-link></li>
        <li><sym-link :class="listItemClass('dbs0200')" to="dbs0200" title="Transaction Item">Transaction Item Definition</sym-link></li> -->

      </ul>
    </sym-card>
    <div class="box-container">

      <sym-card id="cash" class="beige shadow border-main">
        <div slot="header">
          <!-- <span class="menu-item bold serif lg-3">Cash / Treasury</span> -->
          <span class="menu-item bold serif lg-3">CLIENT MANAGEMENT</span>
        </div>

        <ul class="menu-item serifXXX">
          <li><sym-link :class="listItemClass('ars0010')" to="ars0010" title="Client Profile">Client Profile</sym-link></li>
          <li><sym-link :class="listItemClass('ars0020')" to="ars0020" title="Member Request Form ">Client Contract </sym-link></li>
          <li><sym-link :class="listItemClass('ars0040')" to="ars0040" title="Client Pay Group Setup">Client Pay Group Setup</sym-link></li>
          <li><sym-link :class="listItemClass('ars0030')" to="ars0030" title="Member Request Form ">Member Request Form </sym-link></li>
          <li><sym-link :class="listItemClass('ars0050')" to="ars0050" title="Member Request Pooling">Member Request Pooling</sym-link></li>
          <li><sym-link :class="listItemClass('ars0060')" to="ars0060" title="Member Request Monitoring">Member Request Monitoring</sym-link></li>
          <li><sym-link :class="listItemClass('ars0070')" to="ars0070" title="Paygroup Member List">Paygroup Member List</sym-link></li>
          <li><sym-link :class="listItemClass('ars0080')" to="ars0080" title="Member Deduction">Member Deduction</sym-link></li>
          <li><sym-link :class="listItemClass('ars0090')" to="ars0090" title="Member Pay Out">Member Pay Out</sym-link></li>
          <li><sym-link :class="listItemClass('ars0100')" to="ars0100" title="Client Billing">Client Billing</sym-link></li>
          <!-- <li><sym-link :class="listItemClass('ars0110')" to="ars0110" title="Member Allowance">Member Allowance</sym-link></li> -->


          <!-- <li><sym-link :class="listItemClass('csh0010')" to="csh0010" title="Receipts">Collection Posting</sym-link></li> -->
          <!-- <li><sym-link :class="listItemClass('csh0020')" to="csh0020" title="Disbursements">Disbursement Posting</sym-link></li>
          <li class="list-item-separator"></li>

          <li><sym-link :class="listItemClass('csh0030')" to="csh0030" title="Liquidation">Liquidation Entry</sym-link></li>
          <li><sym-link :class="listItemClass('csh0040')" to="csh0040" title="Deposit Entry">Deposit Entry</sym-link></li>
          <li class="list-item-separator"></li>

          <li><sym-link :class="listItemClass('csh0110')" to="csh0110" title="ADA Number">ADA Number Assignment</sym-link></li>
          <li><sym-link :class="listItemClass('csh0120')" to="csh0120" title="RCI Number">RCI Number Assignment</sym-link></li>
          <li><sym-link :class="listItemClass('csh0130')" to="csh0130" title="EMDS Number">EMDS Number Assignment</sym-link></li> -->

        </ul>
      </sym-card>

      <sym-card id="budget" class="beige shadow-light border-main">
        <div slot="header">
          <!-- <span class="menu-item bold serif lg-3">Budget</span> -->
          <span class="menu-item bold serif lg-3">HUMAN RESOURCES</span>
        </div>

        <ul class="menu-item serifXXX">

          <li><sym-link :class="listItemClass('hrs0010')" to="hrs0010" title="Member Profile">Member Profile</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0030')" to="hrs0030" title="Applicant Profile">Applicant Profile</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0040')" to="hrs0040" title="Recruiter Setup">Recruiter Setup</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0050')" to="hrs0050" title="Member Employment Status Action">Member Employment Status Action</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0060')" to="hrs0060" title="Member Employment Status Update">Member Employment Status Update</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0070')" to="hrs0070" title="Member Engagement Monitoring">Member Engagement Monitoring</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0080')" to="hrs0080" title="Member Government ID">Member Government ID Monitoring</sym-link></li>
          <li><sym-link :class="listItemClass('hrs0090')" to="hrs0090" title="Member Request Candidate List">Member Request Candidate List</sym-link></li>

        </ul>
      </sym-card>


      <sym-card id="timekeeping" class="beige shadow border-main">
        <div slot="header">
          <span class="menu-item bold serif lg-3">Timekeeping</span>
        </div>

        <ul class="menu-item serifXXX">
  
          <li><sym-link :class="listItemClass('pay0010')" to="pay0010" title="Policy">Policy</sym-link></li>
    <li><sym-link :class="listItemClass('pay0020')" to="pay0020" title="Schedule">Schedule</sym-link></li>
    <li><sym-link :class="listItemClass('pay0030')" to="pay0030" title="Daily Time Record">Daily Time Record</sym-link></li>
    <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Daily Time Record Generation">Daily Time Record Generation</sym-link></li>
    <!-- <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Process Payout">Process Payout</sym-link></li>
    <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Process Billing">Process Billing</sym-link></li> -->

        </ul>
      </sym-card>

      <sym-card id="accounting" class="beige shadow border-main">
        <div slot="header">
          <span class="menu-item bold serif lg-3">ACCOUNTING ANG FINANCE MANAGEMENT</span>
        </div>

        <ul class="menu-item serifXXX">
    
        <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Billing Invoice">Billing Invoice</sym-link></li>
        <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Receivables">Receivables</sym-link></li>
        <li><sym-link :class="listItemClass('fin0010')" to="fin0010" title="Cash Receipt">Cash Receipt</sym-link></li>
        <li><sym-link :class="listItemClass('pay0040')" to="pay0040" title="Cash Receipt Entry">Credit Memo</sym-link></li>

        <li><sym-link :class="listItemClass('ars0080')" to="ars0080" title="Deduction Entry">Deduction Entry</sym-link></li>
  
    

        </ul>
      </sym-card>
      <!-- <sym-time><span>time</span></sym-time> -->
    </div>
  </div>

</div>

</template>

<script>

export default {

  methods: {
    listItemClass ( pageId ) {
      return {
        'list-item': true,
        'disabled': !(this.sym.hasPageAccess(pageId) && this.canAccessPage(pageId))
      }
    },

    initMenuBlocks () {
      let count = 0;

      count += this.initMenuBlock('budget');
      count += this.initMenuBlock('accounting');
      count += this.initMenuBlock('cash');
      count += this.initMenuBlock('database');

      if (count === 1) {
        this.$refs['menu-content'].classList.add('single-column');
      }
    },

    initMenuBlock ( menuId ) {
      const
        el = document.getElementById(menuId),
        selectors = '#' + menuId + ' ul .list-item';

      let items = document.querySelectorAll(selectors);
      if (items.length > 0) {
        for (let item of items) {
          if (!item.classList.contains('disabled')) {
            return 1;
          }
        }
        if (el.classList) {
          el.classList.add('collapse');
        }
      }
      return 0;
    },

    onKeyDown (e) {
      if (e.key === 'Escape') {
        this.$router.back();
      }
    },

    canAccessPage (pageId) {
      if (!pageId) {
        return false;
      }
      return true;
    }

  },

  mounted () {
    this.initMenuBlocks();
    this.dom.on(window, 'keydown', this.onKeyDown);
  },

  updated () {
    this.initMenuBlocks();
  },

  beforeDestroy () {
    this.dom.off(window, 'keydown', this.onKeyDown);
  }

}

</script>

<style>

.menu.container {
  padding: 0;
}

.menu-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  column-gap: 1.5rem;
  row-gap: 1rem;
  padding: 1.5rem 10rem;
}

.box-containe {
  display: grid;
  grid-template-columns: repeat(2, 2 fr);
  column-gap: 1.5rem;
  row-gap: 1rem;
  padding: 1.5rem 10rem;
}

.menu-item {
  font-size: 1.125rem;
}

ul {
  list-style-type: none;
  margin-bottom: .5rem;
}

ul a {
  color: inherit;
  padding-top: .5rem;
  padding-bottom: .5rem;
  text-decoration: none;
}

ul a.disabled {
  opacity: 0.4;
}

ul a:not(.disabled):hover {
  /* background: gainsboro; */
  color: white;
  background-color: var(--app-orange);
}

.single-column {
  display: flex;
  justify-content: center;
}

.single-column section {
  min-width: 30rem;
  width: 50%;
}

@media (max-width: 1260px) {
  .menu-content:not(.single-column) {
    padding-left: 6.75rem;
    padding-right: 6.75rem;
  }
}

@media (max-width: 1100px) {
  .menu-content:not(.single-column) {
    padding-left: 4.5rem;
    padding-right: 4.5rem;
  }
}

@media (max-width: 1060px) {
  .menu-content:not(.single-column) {
    padding-left: 1.5rem;
    padding-right: 1.5rem;
  }
}

@media (max-width: 740px) {
  .menu-content {
    padding: 1.5rem;
    grid-template-columns: 1fr;
  }

  .single-column section {
    width: 100%;
    min-width: initial;
  }
}

</style>