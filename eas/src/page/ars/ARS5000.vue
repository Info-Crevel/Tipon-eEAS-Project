// Sourcing And Hiring Performance

<template>
<section class="container p-0" :key="ts">
<sym-form id="ars5000" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <!-- footer -->
  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between" @click="onRefresh">
        <i class="fa fa-refresh fa-lg"></i><span>Refresh</span>
      </button>
    </div>
  </div>

      <div class="d-flex justify-between pb-3">
          <span class="display-8 text-charcoal bold serif">Total MRF</span>
          <span class="display-8 text-charcoal bold mr-2">{{ core.toDecimalFormat(orgMrf.totalMrf,0) }}</span>
        </div>


          <div class="media w-100">
            <div class="mr-3XX ml-3 mr-4" id="mrf-chart"></div>
            <div class="media-body">
              <ul class="stats-right w-100">
                <li class="list-item"><span class="steelblue legend-dot mr-2"></span><span class="lg-2">Open</span><span class="float-right lg-3">{{ core.toDecimalFormat(orgMrf.totalOpenMrf, 0) }}</span></li>
                <li class="list-item"><span class="warning lighten-5 legend-dot mr-2"></span><span class="lg-2">Closed</span><span class="float-right lg-3">{{ core.toDecimalFormat(orgMrf.totalClosedMrf,0) }}</span></li>
                <li class="list-item"><span class="success lighten-5 legend-dot mr-2"></span><span class="lg-2">Accomplishment</span><span class="float-right lg-3">{{ core.toDecimalFormat(orgMrf.totalClosedMrfPercentage,2) }} %</span></li>
              </ul>


              
            </div>
          </div>


    <div class="d-flex justify-between pb-3">
          <span class="display-8 text-charcoal bold serif">Total Vacancy</span>
          <span class="display-8 text-charcoal bold mr-2">{{ core.toDecimalFormat(orgMrf.totalVacancy,0 ) }}</span>
        </div>


                <div class="media w-100">
            <div class="mr-3XX ml-3 mr-4" id="vacancy-chart"></div>
            <div class="media-body">
              <ul class="stats-right w-100">
                <li class="list-item"><span class="steelblue legend-dot mr-2"></span><span class="lg-2">Vacancy</span><span class="float-right lg-3">{{ core.toDecimalFormat(orgMrf.remainingVacancy,0) }}</span></li>
                <li class="list-item"><span class="warning lighten-5 legend-dot mr-2"></span><span class="lg-2">Hired</span><span class="float-right lg-3">{{ core.toDecimalFormat(orgMrf.totalHired,0) }}</span></li>
              </ul>
            </div>
          </div>



        </sym-form>
</section>
</template>

<script>

import {
  get
} from '../../js/http';

import {
  chartColors,
  drawPieChart
} from '../../js/gfx';


import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'ars5000',

  data () {
    return {
      orgMrf: [],
    };


    
  },

  methods: {

    onLoad () {
      const
        me = this;
        me.refresh();
    },

    drawMrfChart () {
      let
        me = this,
        colors = [],
        pieData = [me.orgMrf.totalOpenMrf, me.orgMrf.totalClosedMrf];

      colors[0] = chartColors.blues[5];
      colors[1] = chartColors.oranges[3];

      drawPieChart(pieData, 'mrf-chart', 100, 100, colors, 50);
    },

    drawVacancyChart () {
      let
        me = this,
        colors = [],
        pieData = [me.orgMrf.remainingVacancy, me.orgMrf.totalHired];

      colors[0] = chartColors.blues[5];
      colors[1] = chartColors.oranges[3];

      drawPieChart(pieData, 'vacancy-chart', 100, 100, colors, 50);
    },


    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getOrgMrf()
        .then( list => {
          me.stopWait(wait);
          me.orgMrf = list[0];
          me.drawMrfChart();
          me.drawVacancyChart();
          me.isFilled = true;
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })

    },

    refreshOldRefs () {
      const me = this;
      me.oldDetail = JSON.stringify(me.detail);
    },

    // API calls

    getOrgMrf () {
      return get('api/sourcing-hiring-dashboard/' + this.sym.userInfo.userOrgId);
    },


    onRefresh () {
      this.loadData();
    },


  },

  created () {
    const me = this;
    me.oldDetail = JSON.stringify(me.detail)
  },

  mounted () {
    const me = this;
    me.loadData();
  }

}

</script>

<style scoped>

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
  overflow: auto;
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
  flex-wrap: nowrap;
}

@media (max-width: 1200px) {
  .fixed-header table {
    width: 85%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 95%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #ars5000 >>> .card-body {
    padding: .25rem;
  }
}

</style>