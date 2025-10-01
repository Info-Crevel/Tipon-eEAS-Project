/* jslint esversion: 6 */

const
  setup = {
    appName: 'eEAS',
    logonIdCue: 'Login ID',
    passwordCue: 'Password',
    isMenuEnabled: true,
    locale: 'en-US',
    // dateRefreshInterval: 720,     // in minutes; 0 to turn off
    dateRefreshInterval: 0,          // in minutes; 0 to turn off
    dateFormat: 'MM/dd/yyyy',     // dd/MM/yyyy, yyyy/MM/dd
    dateDisplayFormat: 'dd MMM yyyy',
    // dateTemplate: '  /  /    ',
    dateTemplate: 'mm/dd/yyyy',
    monthFormat: 'MM/yyyy',
    monthTemplate: 'mm/yyyy',
    dateSeparator: '/',
    // timeFormat: 'HH:mm',
    // timeDisplayFormat: 'h:mm tt',
 // timeFormat: 'HH:mm',
    timeFormat: 'HHmm',           // 16 May 2025 - EMT
    // timeDisplayFormat: 'h:mm tt',
    timeDisplayFormat: 'HHmm',    // 16 May 2025 - EMT
    timeSeparator: ':',
    decimalDigits: 2,
    emptyOption: 'All',
    anonymousPhoto: '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'
  };

function registerRoutes (addRoute) {

  // const BGS0010 = () => import(/* webpackChunkName: "bgs0010" */ '../page/bgs/BGS0010.vue');
  // const BGS0020 = () => import(/* webpackChunkName: "bgs0020" */ '../page/bgs/BGS0020.vue');
  // const BGS0030 = () => import(/* webpackChunkName: "bgs0030" */ '../page/bgs/BGS0030.vue');
  // const BGS0040 = () => import(/* webpackChunkName: "bgs0040" */ '../page/bgs/BGS0040.vue');
  // const BGS0100 = () => import(/* webpackChunkName: "bgs0100" */ '../page/bgs/BGS0100.vue');
  // const BGS0110 = () => import(/* webpackChunkName: "bgs0110" */ '../page/bgs/BGS0110.vue');
  // const BGS0120 = () => import(/* webpackChunkName: "bgs0120" */ '../page/bgs/BGS0120.vue');
  // const BGS0510 = () => import(/* webpackChunkName: "bgs0510" */ '../page/bgs/BGS0510.vue');
  // const BGS0520 = () => import(/* webpackChunkName: "bgs0520" */ '../page/bgs/BGS0520.vue');
  // const BGS0530 = () => import(/* webpackChunkName: "bgs0530" */ '../page/bgs/BGS0530.vue');
  // const BGS0540 = () => import(/* webpackChunkName: "bgs0540" */ '../page/bgs/BGS0540.vue');
  // const BGS0550 = () => import(/* webpackChunkName: "bgs0550" */ '../page/bgs/BGS0550.vue');
  // const BGS0560 = () => import(/* webpackChunkName: "bgs0560" */ '../page/bgs/BGS0560.vue');
  // const BGS0570 = () => import(/* webpackChunkName: "bgs0570" */ '../page/bgs/BGS0570.vue');
  // const BGS1000 = () => import(/* webpackChunkName: "bgs1000" */ '../page/bgs/BGS1000.vue');

  // const CSH0010 = () => import(/* webpackChunkName: "csh0010" */ '../page/csh/CSH0010.vue');
  // const CSH0020 = () => import(/* webpackChunkName: "csh0020" */ '../page/csh/CSH0020.vue');
  // const CSH0030 = () => import(/* webpackChunkName: "csh0030" */ '../page/csh/CSH0030.vue');
  // const CSH0040 = () => import(/* webpackChunkName: "csh0040" */ '../page/csh/CSH0040.vue');
  // const CSH0110 = () => import(/* webpackChunkName: "csh0110" */ '../page/csh/CSH0110.vue');
  // const CSH0120 = () => import(/* webpackChunkName: "csh0120" */ '../page/csh/CSH0120.vue');
  // const CSH0130 = () => import(/* webpackChunkName: "csh0130" */ '../page/csh/CSH0130.vue');
  const APS0010 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0010.vue');
  const APS0020 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0020.vue');
  const APS0030 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0030.vue');
  const APS0040 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0040.vue');
  const APS0100 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0100.vue');
  const APS0500 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0500.vue');
  const APS0510 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0510.vue');
  const APS0520 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0520.vue');
  const APS0530 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0530.vue');
  const APS0540 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0540.vue');
  const APS0550 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0550.vue');
  const APS0560 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0560.vue');
  const APS0570 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0570.vue');
  const APS0600 = () => import(/* webpackChunkName: "dbs0010" */ '../page/aps/APS0600.vue');

  const ARS0010 = () => import(/* webpackChunkName: "dbs0010" */ '../page/ars/ARS0010.vue');
  const ARS0020 = () => import(/* webpackChunkName: "dbs0020" */ '../page/ars/ARS0020.vue');
  const ARS0030 = () => import(/* webpackChunkName: "dbs0030" */ '../page/ars/ARS0030.vue');
  const ARS0040 = () => import(/* webpackChunkName: "dbs0040" */ '../page/ars/ARS0040.vue');
  const ARS0050 = () => import(/* webpackChunkName: "dbs0050" */ '../page/ars/ARS0050.vue');
  const ARS0060 = () => import(/* webpackChunkName: "dbs0060" */ '../page/ars/ARS0060.vue');
  const ARS0070 = () => import(/* webpackChunkName: "ars0070" */ '../page/ars/ARS0070.vue');
  const ARS0080 = () => import(/* webpackChunkName: "ars0080" */ '../page/ars/ARS0080.vue');
  const ARS0090 = () => import(/* webpackChunkName: "ars0090" */ '../page/ars/ARS0090.vue');
  const ARS0100 = () => import(/* webpackChunkName: "ars0100" */ '../page/ars/ARS0100.vue');
  const ARS0110 = () => import(/* webpackChunkName: "ars0110" */ '../page/ars/ARS0110.vue');
  const ARS0120 = () => import(/* webpackChunkName: "ars0110" */ '../page/ars/ARS0120.vue');
  const ARS0130 = () => import(/* webpackChunkName: "ars0500" */ '../page/ars/ARS0130.vue');
  const ARS0140 = () => import(/* webpackChunkName: "ars0500" */ '../page/ars/ARS0140.vue');
  const ARS0150 = () => import(/* webpackChunkName: "ars0500" */ '../page/ars/ARS0150.vue');
  const ARS0500 = () => import(/* webpackChunkName: "ars0500" */ '../page/ars/ARS0500.vue');
  const ARS5000 = () => import(/* webpackChunkName: "ARS5000" */ '../page/ars/ARS5000.vue');
  

  const DBS0010 = () => import(/* webpackChunkName: "dbs0010" */ '../page/dbs/DBS0010.vue');
  const DBS0020 = () => import(/* webpackChunkName: "dbs0020" */ '../page/dbs/DBS0020.vue');
  const DBS0030 = () => import(/* webpackChunkName: "dbs0030" */ '../page/dbs/DBS0030.vue');
  const DBS0040 = () => import(/* webpackChunkName: "dbs0040" */ '../page/dbs/DBS0040.vue');
  const DBS0050 = () => import(/* webpackChunkName: "dbs0050" */ '../page/dbs/DBS0050.vue');
  const DBS0060 = () => import(/* webpackChunkName: "dbs0060" */ '../page/dbs/DBS0060.vue');
  const DBS0070 = () => import(/* webpackChunkName: "dbs0070" */ '../page/dbs/DBS0070.vue');
  const DBS0080 = () => import(/* webpackChunkName: "dbs0070" */ '../page/dbs/DBS0080.vue');
  const DBS0090 = () => import(/* webpackChunkName: "dbs0090" */ '../page/dbs/DBS0090.vue');
  const DBS0100 = () => import(/* webpackChunkName: "dbs0100" */ '../page/dbs/DBS0100.vue');
  const DBS0110 = () => import(/* webpackChunkName: "dbs0110" */ '../page/dbs/DBS0110.vue');
  const DBS0120 = () => import(/* webpackChunkName: "dbs0120" */ '../page/dbs/DBS0120.vue');
  const DBS0130 = () => import(/* webpackChunkName: "dbs0130" */ '../page/dbs/DBS0130.vue');
  const DBS0140 = () => import(/* webpackChunkName: "dbs0140" */ '../page/dbs/DBS0140.vue');

  const DBS0150 = () => import(/* webpackChunkName: "dbs0150" */ '../page/dbs/DBS0150.vue');
  const DBS0160 = () => import(/* webpackChunkName: "dbs0160" */ '../page/dbs/DBS0160.vue');
  const DBS0170 = () => import(/* webpackChunkName: "dbs0170" */ '../page/dbs/DBS0170.vue');
  const DBS0180 = () => import(/* webpackChunkName: "dbs0180" */ '../page/dbs/DBS0180.vue');
  const DBS0190 = () => import(/* webpackChunkName: "dbs0190" */ '../page/dbs/DBS0190.vue');
  const DBS0200 = () => import(/* webpackChunkName: "dbs0200" */ '../page/dbs/DBS0200.vue');
  const DBS0210 = () => import(/* webpackChunkName: "dbs0210" */ '../page/dbs/DBS0210.vue');
  const DBS0220 = () => import(/* webpackChunkName: "dbs0220" */ '../page/dbs/DBS0220.vue');
  const DBS0230 = () => import(/* webpackChunkName: "dbs0230" */ '../page/dbs/DBS0230.vue');
  const DBS0240 = () => import(/* webpackChunkName: "dbs0240" */ '../page/dbs/DBS0240.vue');
  const DBS0250 = () => import(/* webpackChunkName: "dbs0250" */ '../page/dbs/DBS0250.vue');
  const DBS0260 = () => import(/* webpackChunkName: "dbs0260" */ '../page/dbs/DBS0260.vue');
  const DBS0270 = () => import(/* webpackChunkName: "dbs0270" */ '../page/dbs/DBS0270.vue');
  const DBS0280 = () => import(/* webpackChunkName: "dbs0280" */ '../page/dbs/DBS0280.vue');
  const DBS0290 = () => import(/* webpackChunkName: "dbs0290" */ '../page/dbs/DBS0290.vue');
  const DBS0300 = () => import(/* webpackChunkName: "dbs0300" */ '../page/dbs/DBS0300.vue');
  const DBS0310 = () => import(/* webpackChunkName: "dbs0310" */ '../page/dbs/DBS0310.vue');
  const DBS0320 = () => import(/* webpackChunkName: "dbs0320" */ '../page/dbs/DBS0320.vue');
  const DBS0330 = () => import(/* webpackChunkName: "dbs0330" */ '../page/dbs/DBS0330.vue');
  const DBS0340 = () => import(/* webpackChunkName: "dbs0340" */ '../page/dbs/DBS0340.vue');
  const DBS0350 = () => import(/* webpackChunkName: "dbs0350" */ '../page/dbs/DBS0350.vue');
  const DBS0360 = () => import(/* webpackChunkName: "dbs0360" */ '../page/dbs/DBS0360.vue');
  const DBS0370 = () => import(/* webpackChunkName: "dbs0370" */ '../page/dbs/DBS0370.vue');
  const DBS0380 = () => import(/* webpackChunkName: "dbs0380" */ '../page/dbs/DBS0380.vue');
  const DBS0390 = () => import(/* webpackChunkName: "dbs0390" */ '../page/dbs/DBS0390.vue');

  const DBS0400 = () => import(/* webpackChunkName: "dbs0400" */ '../page/dbs/DBS0400.vue');
  const DBS0410 = () => import(/* webpackChunkName: "dbs0410" */ '../page/dbs/DBS0410.vue');
  const DBS0420 = () => import(/* webpackChunkName: "dbs0420" */ '../page/dbs/DBS0420.vue');
  const DBS0430 = () => import(/* webpackChunkName: "dbs0430" */ '../page/dbs/DBS0430.vue');
  const DBS0440 = () => import(/* webpackChunkName: "dbs0440" */ '../page/dbs/DBS0440.vue');
  const DBS0450 = () => import(/* webpackChunkName: "dbs0450" */ '../page/dbs/DBS0450.vue');
  const DBS0460 = () => import(/* webpackChunkName: "dbs0460" */ '../page/dbs/DBS0460.vue');
  const DBS0470 = () => import(/* webpackChunkName: "dbs0460" */ '../page/dbs/DBS0470.vue');
  const DBS0480 = () => import(/* webpackChunkName: "dbs0480" */ '../page/dbs/DBS0480.vue');
  const DBS0490 = () => import(/* webpackChunkName: "dbs0490" */ '../page/dbs/DBS0490.vue');


  const DBS0500 = () => import(/* webpackChunkName: "dbs0500" */ '../page/dbs/DBS0500.vue');
  const DBS0510 = () => import(/* webpackChunkName: "dbs0510" */ '../page/dbs/DBS0510.vue');
  const DBS0520 = () => import(/* webpackChunkName: "dbs0520" */ '../page/dbs/DBS0520.vue');
  const DBS0530 = () => import(/* webpackChunkName: "dbs0530" */ '../page/dbs/DBS0530.vue');
  const DBS0540 = () => import(/* webpackChunkName: "dbs0540" */ '../page/dbs/DBS0540.vue');
  const DBS0550 = () => import(/* webpackChunkName: "dbs0550" */ '../page/dbs/DBS0550.vue');
  const DBS0560 = () => import(/* webpackChunkName: "dbs0560" */ '../page/dbs/DBS0560.vue');
  const DBS0570 = () => import(/* webpackChunkName: "dbs0570" */ '../page/dbs/DBS0570.vue');
  const DBS0580 = () => import(/* webpackChunkName: "dbs0580" */ '../page/dbs/DBS0580.vue');
  const DBS0590 = () => import(/* webpackChunkName: "dbs0910" */ '../page/dbs/DBS0590.vue');

  const DBS0900 = () => import(/* webpackChunkName: "dbs0900" */ '../page/dbs/DBS0900.vue');
  const DBS0910 = () => import(/* webpackChunkName: "dbs0910" */ '../page/dbs/DBS0910.vue');
  const DBS0920 = () => import(/* webpackChunkName: "dbs0910" */ '../page/dbs/DBS0920.vue');
  

  const HRS0010 = () => import(/* webpackChunkName: "hrs0010" */ '../page/hrs/HRS0010.vue');
  const HRS0030 = () => import(/* webpackChunkName: "hrs0030" */ '../page/hrs/HRS0030.vue');
  const HRS0040 = () => import(/* webpackChunkName: "hrs0040" */ '../page/hrs/HRS0040.vue');
  const HRS0050 = () => import(/* webpackChunkName: "hrs0050" */ '../page/hrs/HRS0050.vue');
  const HRS0060 = () => import(/* webpackChunkName: "hrs0060" */ '../page/hrs/HRS0060.vue');
  const HRS0070 = () => import(/* webpackChunkName: "hrs0070" */ '../page/hrs/HRS0070.vue');
  const HRS0080 = () => import(/* webpackChunkName: "hrs0080" */ '../page/hrs/HRS0080.vue');
  const HRS0090 = () => import(/* webpackChunkName: "hrs0090" */ '../page/hrs/HRS0090.vue');
  

  // const DBS0100 = () => import(/* webpackChunkName: "dbs0100" */ '../page/dbs/DBS0100.vue');
  // const DBS0110 = () => import(/* webpackChunkName: "dbs0110" */ '../page/dbs/DBS0110.vue');
  // const DBS0120 = () => import(/* webpackChunkName: "dbs0120" */ '../page/dbs/DBS0120.vue');
  // const DBS0130 = () => import(/* webpackChunkName: "dbs0130" */ '../page/dbs/DBS0130.vue');
  // const DBS0140 = () => import(/* webpackChunkName: "dbs0140" */ '../page/dbs/DBS0140.vue');
  // const DBS0150 = () => import(/* webpackChunkName: "dbs0150" */ '../page/dbs/DBS0150.vue');
  // const DBS0200 = () => import(/* webpackChunkName: "dbs0200" */ '../page/dbs/DBS0200.vue');

  // const GLS0010 = () => import(/* webpackChunkName: "gls0010" */ '../page/gls/GLS0010.vue');
  // const GLS0020 = () => import(/* webpackChunkName: "gls0020" */ '../page/gls/GLS0020.vue');
  // const GLS0030 = () => import(/* webpackChunkName: "gls0030" */ '../page/gls/GLS0030.vue');
  // const GLS0040 = () => import(/* webpackChunkName: "gls0040" */ '../page/gls/GLS0040.vue');
  // const GLS0050 = () => import(/* webpackChunkName: "gls0050" */ '../page/gls/GLS0050.vue');
  // const GLS0060 = () => import(/* webpackChunkName: "gls0060" */ '../page/gls/GLS0060.vue');
  // const GLS0070 = () => import(/* webpackChunkName: "gls0070" */ '../page/gls/GLS0070.vue');
  const GLS0100 = () => import(/* webpackChunkName: "gls0100" */ '../page/gls/GLS0100.vue');
  // const GLS0200 = () => import(/* webpackChunkName: "gls0200" */ '../page/gls/GLS0200.vue');
  const GLS1000 = () => import(/* webpackChunkName: "gls1000" */ '../page/gls/GLS1000.vue');

  const EAS0000 = () => import(/* webpackChunkName: "eas0000" */ '../page/eas/EAS0000.vue');
  addRoute('/menu', EAS0000, 'eas0000', true);

  const PAY0010 = () => import(/* webpackChunkName: "pay0010" */ '../page/pay/PAY0010.vue');
  const PAY0020 = () => import(/* webpackChunkName: "pay0020" */ '../page/pay/PAY0020.vue');
  const PAY0030 = () => import(/* webpackChunkName: "pay0030" */ '../page/pay/PAY0030.vue');
  const PAY0040 = () => import(/* webpackChunkName: "pay0040" */ '../page/pay/PAY0040.vue');

  const FIN0010 = () => import(/* webpackChunkName: "fin0010" */ '../page/fin/FIN0010.vue');
  const FIN0020 = () => import(/* webpackChunkName: "fin0020" */ '../page/fin/FIN0020.vue');
  const FIN0030 = () => import(/* webpackChunkName: "fin0030" */ '../page/fin/FIN0030.vue');
  const FIN0040 = () => import(/* webpackChunkName: "fin0030" */ '../page/fin/FIN0040.vue');
  const FIN0100 = () => import(/* webpackChunkName: "fin0100" */ '../page/fin/FIN0100.vue');


  // addRoute('/appropriation-entry', BGS0010, 'bgs0010');
  // addRoute('/appropriation-details', BGS0020, 'bgs0020');

  // addRoute('/obligation-entry', BGS0030, 'bgs0030');
  // addRoute('/allotment-transfer', BGS0040, 'bgs0040');

  // addRoute('/activity-definition', BGS0100, 'bgs0100');
  // addRoute('/allotment-entry', BGS0110, 'bgs0110');
  // addRoute('/allotment-details', BGS0120, 'bgs0120');

  // addRoute('/budget-execution-fund-status', BGS0510, 'bgs0510');
  // addRoute('/budget-execution-summary-current', BGS0520, 'bgs0520');
  // addRoute('/budget-execution-summary-continuing', BGS0530, 'bgs0530');
  // addRoute('/budget-execution-detail-current', BGS0540, 'bgs0540');
  // addRoute('/budget-execution-detail-current-saa', BGS0550, 'bgs0550');
  // addRoute('/budget-execution-detail-continuing', BGS0560, 'bgs0560');
  // addRoute('/budget-execution-detail-continuing-saa', BGS0570, 'bgs0570');
  // addRoute('/activities-structure', BGS1000, 'bgs1000');

  // addRoute('/collection-posting', CSH0010, 'csh0010');
  // addRoute('/disbursement-posting', CSH0020, 'csh0020');
  // addRoute('/cash-advance-liquidation-entry', CSH0030, 'csh0030');
  // addRoute('/deposit-entry', CSH0040, 'csh0040');
  // addRoute('/ada-number-assignment', CSH0110, 'csh0110');
  // addRoute('/rci-number-assignment', CSH0120, 'csh0120');
  // addRoute('/emds-number-assignment', CSH0130, 'csh0130');
  addRoute('/payee-type-setup', APS0010, 'aps0010');
  addRoute('/payable-account-setup', APS0020, 'aps0020');
  addRoute('/payable-tax-setup', APS0030, 'aps0030');
  addRoute('/payee-definition', APS0040, 'aps0040');
  addRoute('/payables', APS0100, 'aps0100')
  addRoute('/payable-reviewer', APS0500, 'aps0500')
  addRoute('/payable-request-type', APS0510, 'aps0510')
  addRoute('/payable-request-type-particulars', APS0520, 'aps0520')
  addRoute('/payout-account-mapping', APS0530, 'aps0530')
  addRoute('/request-doc-type-setup', APS0540, 'aps0540')
  addRoute('/payable-approver', APS0550, 'aps0550')
  addRoute('/payable-request-transaction-type', APS0560, 'aps0560')
  addRoute('/payable-request-transaction-mapping', APS0570, 'aps0570')
  addRoute('/disbursment-request-form', APS0600, 'aps0600')



  addRoute('/client-profile', ARS0010, 'ars0010');
  addRoute('/client-contract-', ARS0020, 'ars0020');
  addRoute('/member-request-form-', ARS0030, 'ars0030');
  addRoute('/client-pay-group-setup', ARS0040, 'ars0040');
  addRoute('/member-request-pooling', ARS0050, 'ars0050');
  addRoute('/member-request-monitoring', ARS0060, 'ars0060');
  addRoute('/paygroup-member-list', ARS0070, 'ars0070');
  addRoute('/member-deduction-entry', ARS0080, 'ars0080');
  addRoute('/member-pay-out', ARS0090, 'ars0090');
  addRoute('/client-billing', ARS0100, 'ars0100');
  addRoute('/member-allowance', ARS0110, 'ars0110');
  addRoute('/mrf-transfer', ARS0120, 'ars0120');
  addRoute('/billing-rate-sheet', ARS0130, 'ars0130');
  addRoute('/member-deminimis', ARS0140, 'ars0140');
  addRoute('/pay-out-sheet', ARS0150, 'ars0150');
  addRoute('/receivables', ARS0500, 'ars0500');
  addRoute('/sourcing-and-hiring-performance', ARS5000, 'ars5000');

  addRoute('/religion-setup', DBS0010, 'dbs0010');
  addRoute('/business-platform-master', DBS0020, 'dbs0020');
  addRoute('/industry-setup', DBS0030, 'dbs0030');
  addRoute('/relation-definition', DBS0040, 'dbs0040');
  addRoute('/region-master', DBS0050, 'dbs0050');
  addRoute('/province-definition', DBS0060, 'dbs0060');
  addRoute('/municipality-setup', DBS0070, 'dbs0070');

  addRoute('/organization-platform-setup', DBS0080, 'dbs0080');
  addRoute('/education-level-setup', DBS0090, 'dbs0090');
  addRoute('/disability-setup', DBS0100, 'dbs0100');
  addRoute('/doc-type-setup', DBS0110, 'dbs0110');
  addRoute('/member-type-qualification-setup', DBS0120, 'dbs0120');
  addRoute('/skill-set-setup', DBS0130, 'dbs0130');

  addRoute('/source-of-application', DBS0140, 'dbs0140');
  addRoute('/license-profession', DBS0150, 'dbs0150');
  addRoute('/applicant-position', DBS0160, 'dbs0160');
  addRoute('/contract-type', DBS0170, 'dbs0170');
  addRoute('/charging-consideration', DBS0180, 'dbs0180');
  addRoute('/in-house-benefits', DBS0190, 'dbs0190');
  addRoute('/statutory-benefits', DBS0200, 'dbs0200');

  addRoute('/ncii-qualification-title', DBS0210, 'dbs0210');
  addRoute('/training-institution', DBS0220, 'dbs0220');
  addRoute('/assessment-center', DBS0230, 'dbs0230');
  addRoute('/compliance-training', DBS0240, 'dbs0240');
  addRoute('/insurance-coverage', DBS0250, 'dbs0250');
  addRoute('/savings-and-loans', DBS0260, 'dbs0260');

  addRoute('/school-definition', DBS0270, 'dbs0270');
  addRoute('/course-definition', DBS0280, 'dbs0280');
  addRoute('/bank-definition', DBS0290, 'dbs0290');
  addRoute('/bank-location-setup', DBS0300, 'dbs0300');
  addRoute('/member-transaction-type-setup', DBS0310, 'dbs0310');
  addRoute('/expense-charging-definition', DBS0320, 'dbs0320');
  addRoute('/inventory-release-charging', DBS0330, 'dbs0330');
  addRoute('/allowance-setup', DBS0340, 'dbs0340');
  addRoute('/deminimis-definition', DBS0350, 'dbs0350');
  addRoute('/affiliation-definition', DBS0360, 'dbs0360');
  addRoute('/cda-training', DBS0370, 'dbs0370');
  addRoute('/day-type-setup', DBS0380, 'dbs0380');
  addRoute('/holiday-setup', DBS0390, 'dbs0390');

  addRoute('/cda-membership-type', DBS0400, 'dbs0400');
  addRoute('/language-definition', DBS0410, 'dbs0410');
  addRoute('/rnr-recording-setup', DBS0420, 'dbs0420');
  addRoute('/revenue-qualification-setup', DBS0430, 'dbs0430');
  addRoute('/non-revenue-qualification-setup', DBS0440, 'dbs0440');
  addRoute('/vaccine-type-definition', DBS0450, 'dbs0450');
  addRoute('/medical-result-type-setup', DBS0460, 'dbs0460');
  addRoute('/engagement-type-setup',DBS0470 , 'dbs0470');
  addRoute('/mrf-internal-position-setup',DBS0480 , 'dbs0480');
  addRoute('/payroll-transaction-setup',DBS0490 , 'dbs0490');
  addRoute('/payroll-variable-definition',DBS0550 , 'dbs0550');
  addRoute('/member-suffix-definition',DBS0560 , 'dbs0560');
  addRoute('/region-minimum-wage-setup',DBS0570 , 'dbs0570');
  addRoute('/request-type-setup', DBS0580, 'dbs0580');
  addRoute('/hiring-process-screen',DBS0590 , 'dbs0590');


  addRoute('/skill-definition', DBS0500, 'dbs0500');
  addRoute('/withholding-tax-setup', DBS0510, 'dbs0510');
  addRoute('/sss-contribution-schedule', DBS0520, 'dbs0520');
  addRoute('/pag-ibig-contribution-table', DBS0530, 'dbs0530');
  addRoute('/philhealth-contribution-table', DBS0540, 'dbs0540');

  addRoute('/member-control', DBS0900, 'dbs0900');
  addRoute('/work-schedule-setup', DBS0910, 'dbs0910');
  addRoute('/cluster-definition', DBS0920, 'dbs0920');

  addRoute('/member-profile', HRS0010, 'hrs0010');
  addRoute('/applicant-profile', HRS0030, 'hrs0030');
  addRoute('/recruiter-setup', HRS0040, 'hrs0040');
  addRoute('/member-employment-status-action', HRS0050, 'hrs0050');
  addRoute('/member-employment-status-update', HRS0060, 'hrs0060');
  addRoute('/member-engagement-monitoring', HRS0070, 'hrs0070');
  addRoute('/member-incomplete-identification', HRS0080, 'hrs0080');
  addRoute('/member-request-candidate-list', HRS0090, 'hrs0090');

  // addRoute('/cost-center-setup', DBS0100, 'dbs0100');
  // addRoute('/collecting-officer-setup', DBS0110, 'dbs0110');
  // addRoute('/disbursing-officer-setup', DBS0120, 'dbs0120');
  // addRoute('/payor-definition', DBS0130, 'dbs0130');
  // addRoute('/payee-definition', DBS0140, 'dbs0140');
  // addRoute('/signatory-setup', DBS0150, 'dbs0150');
  // addRoute('/transaction-item-definition', DBS0200, 'dbs0200');

  // addRoute('/chart-of-accounts', GLS0010, 'gls0010');
  // addRoute('/general-journal-entry', GLS0020, 'gls0020');
  // addRoute('/cash-receipts-journal-entry', GLS0030, 'gls0030');
  // addRoute('/cash-disbursements-journal-entry', GLS0040, 'gls0040');
  // addRoute('/check-disbursements-journal-entry', GLS0050, 'gls0050');
  // addRoute('/ada-disbursements-journal-entry', GLS0060, 'gls0060');
  // addRoute('/emds-disbursements-journal-entry', GLS0070, 'gls0070');
  addRoute('/template-setup', GLS0100, 'gls0100');
  // addRoute('/order-of-payment-entry', GLS0200, 'gls0200');

  addRoute('/chart-of-accounts-structure', GLS1000, 'gls1000');

  addRoute('/timekeeping-policy-setup', PAY0010, 'pay0010');
  addRoute('/timekeeping-schedule', PAY0020, 'pay0020');
  addRoute('/daily-time-record', PAY0030, 'pay0030');
  addRoute('/daily-time-record-generation', PAY0040, 'pay0040');


  addRoute('/cash-receipt', FIN0010, 'fin0010');
  addRoute('/atc-definition', FIN0020, 'fin0020');
  addRoute('/bank-setup', FIN0030, 'fin0030');
  addRoute('/journal-entry', FIN0040, 'fin0040');
  addRoute('/chart-of-accounts', FIN0100, 'fin0100');
}

function hideHeader () {
  setDisplay('main-header');
}

function showHeader () {
  setDisplay('main-header', 'grid');
}

function hideMainFooter () {
  setDisplay('main-footer');
}

function showMainFooter () {
  setDisplay('main-footer', 'grid');
}

function hideAppFooter () {
  setDisplay('app-footer');
}

function showAppFooter () {
  setDisplay('app-footer', 'block');
}

function hideHeaderFooters () {
  hideHeader();
  hideMainFooter();
  hideAppFooter();
}

function showHeaderFooters () {
  showHeader();
  showMainFooter();
  showAppFooter();
}

function setDisplay (elementId, value) {
  let el = document.getElementById(elementId);
  if (el) {
    el.style.display = value ? value: 'none';
  }
}

export {
  setup,
  registerRoutes,
  hideHeader,
  showHeader,
  hideMainFooter,
  showMainFooter,
  hideAppFooter,
  showAppFooter,
  hideHeaderFooters,
  showHeaderFooters
};
