/* jslint esversion: 6 */

import Chart from 'chart.js/auto';

Chart.defaults.font.family = 'Roboto Condensed';
Chart.defaults.font.size = 15;

export function getChartInstances () {
  return Chart.instances;
}

export function drawBarChart (elementId, data = {}, options = {}) {
  return drawChart('bar', elementId, data, options);
}

export function drawLineChart (elementId, data = {}, options = {}) {
  return drawChart('line', elementId, data, options);
}

function drawChart (chartType, elementId, data = {}, options = {}) {
  const
    config = {
      type: chartType,
      data: data,
      options: options
    },

    el = document.getElementById(elementId);

  if (!el) {
    return;
  }

  return new Chart(el,  config);
}
