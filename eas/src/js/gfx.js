/* jslint esversion: 6  */

import
  * as d3
from 'd3';

export const
  barChartColors = ['#deebf7', 'steelblue', 'white', 'black', 'peru'],
  lineChartColors = ['#deebf7', 'black', 'steelblue', 'khaki', 'lightgreen', 'plum'],
  barChartPadding = { top: 16, right: 20, bottom: 10, left: 10 },
  lineChartPadding = { top: 16, right: 20, bottom: 30, left: 40 },

  // https://bl.ocks.org/mbostock/5577023
  chartColors = {
    'blues': ["#f7fbff","#deebf7","#c6dbef","#9ecae1","#6baed6","#4292c6","#2171b5","#08519c","#08306b"],
    'greens': ["#f7fcf5","#e5f5e0","#c7e9c0","#a1d99b","#74c476","#41ab5d","#238b45","#006d2c","#00441b"],
    'greys': ["#ffffff","#f0f0f0","#d9d9d9","#bdbdbd","#969696","#737373","#525252","#252525","#000000"],
    'oranges': ["#fff5eb","#fee6ce","#fdd0a2","#fdae6b","#fd8d3c","#f16913","#d94801","#a63603","#7f2704"],
    'purples': ["#fcfbfd","#efedf5","#dadaeb","#bcbddc","#9e9ac8","#807dba","#6a51a3","#54278f","#3f007d"],
    'reds': ["#fff5f0","#fee0d2","#fcbba1","#fc9272","#fb6a4a","#ef3b2c","#cb181d","#a50f15","#67000d"],
    'pastel1':  ["#fbb4ae","#b3cde3","#ccebc5","#decbe4","#fed9a6","#ffffcc","#e5d8bd","#fddaec","#f2f2f2"],
    'spectral': ["#9e0142","#d53e4f","#f46d43","#fdae61","#fee08b","#ffffbf","#e6f598","#abdda4","#66c2a5","#3288bd","#5e4fa2"]
  };

export function drawPieChart(data, elementId, width, height, colors, donutWidth) {
  let
    svg,
    arc,
    pie,
    w = width,
    h = height,
    r = Math.min(w, h) / 2,
    id = elementId + '_svg',
    dw = donutWidth || r,
    d = data;

  // remove existing chart
  d3.select('#' + id).remove();

  svg = d3.select('#' + elementId)
    .append('svg')
    .attr('id', id)
    .attr('width', w)
    .attr('height', h)
    .append('g')
    .attr('transform', 'translate(' + (w / 2) +  ',' + (h / 2) + ')');

  arc = d3.arc()
    .innerRadius(r - dw)
    .outerRadius(r);

  pie = d3.pie()
    .value(function (d) { return d; })
    .sort(null);

  svg.selectAll('path')
    .data(pie(d))
    .enter()
    .append('path')
    .attr('d', arc)
    .attr('fill', function(d, i) { return colors[i]; });
}

export function drawBarChartHorizontal(data, elementId, width, barHeight, colors, padding, maxScale, rulerTicks, labelWidth, labelYOffset, valuePrefix, valueSuffix, gap = 2) {

  let
    x,
    svg,
    bar,
    barH = barHeight,
    id = elementId + '_svg',
    chartColors = colors || barChartColors,
    h = (barH + gap) * data.length,
    pad = padding || barChartPadding,
    labelW = labelWidth || 100,
    w = width - labelW - pad.left - pad.right,
    lblYOffset = labelYOffset || '0.5em',
    valPrefix = valuePrefix || '',
    valSuffix = valueSuffix || '';

  // remove existing chart
  d3.select('#' + id).remove();

  x = d3.scaleLinear()
    .range([0, Math.max(w, 0)])
    .domain([0, maxScale || d3.max(data, function (d) { return d.value; })]);

  svg = d3.select('#' + elementId)
    .append('svg')
    .attr('id', id)
    .attr('style', 'background: ' + chartColors[0] + '; display: block;')
    .attr('width', width)
    .attr('height', pad.top + h + pad.bottom + 14);

  // lines
  svg.selectAll('line')
    .data(x.ticks(rulerTicks || d3.max(data, function (d) { return d.value; })))
    .enter().append('line')
    .attr('style', 'stroke: ' + chartColors[4] + ';')
    .attr('x1', function (d) { return x(d) + pad.left + labelW; })
    .attr('x2', function (d) { return x(d) + pad.left + labelW; })
    .attr('y1', pad.top)
    .attr('y2', pad.top + ((barH + gap) * data.length));

  // ruler
  svg.selectAll('text')
    .data(x.ticks(rulerTicks || d3.max(data, function (d) { return d.value; })))
    .enter().append('text')
    .attr('style', 'fill: ' + chartColors[3] + ';')
    .attr('x', function (d) { return x(d) + pad.left + labelW; })
    .attr('y', pad.top + ((barH + gap) * data.length) + 18)
    .attr('dy', -6)
    .attr('text-anchor', 'middle')
    .attr('font-size', 11)
    .text(String);

  bar = svg.selectAll('g')
    .data(data)
    .enter().append('g')
    .attr('transform', function (d, i) { return 'translate(0,' + i * (barH + gap) + ')'; });

  bar.append('rect')
    .attr('style', 'fill: ' + chartColors[1])
    .attr('x', pad.left + labelW)
    .attr('y', pad.top)
    .attr('width', function (d) { return x(d.value); })
    .attr('height', barH - 1);

  // value
  bar.append('text')
    .attr('style', 'fill: ' + chartColors[2] + '; text-anchor: end;')
    .attr('x', function (d) { return x(d.value) + pad.left + labelW - 6; })
    .attr('y', pad.top + (barH / 2))
    .attr('dy', lblYOffset)
    .text(function (d) { return valPrefix + d.value + valSuffix; });

  // label
  bar.append('text')
    .attr('style', 'fill: ' + chartColors[3] + '; text-anchor: start;')
    .attr('x', pad.left)
    .attr('y', pad.top + (barH / 2))
    .attr('dy', lblYOffset)
    .text(function (d) { return d.key; });

  // border
  svg.append('rect')
    .attr('x', 1)
    .attr('y', 1)
    .attr('width', Math.max(pad.left + labelW + w + pad.right - 2, 0))
    .attr('height', pad.top + h + pad.bottom + 12)
    .style('stroke', chartColors[5])
    .style('fill', 'none')
    .style('stroke-width', 1);
}

export function drawLineChart(data, elementId, width, height, colors, padding, xAxisStyle, yAxisText) {
  let
    x,
    y,
    svg,
    g,
    line,
    line2,
    line3,
    line4,
    id = elementId + "_svg",
    chartColors = colors || lineChartColors,
    w = width - padding.left - padding.right,
    h = height - padding.top - padding.bottom,
    yAxis,
    xAS = xAxisStyle || 1,
    yAT = yAxisText || '',
    yMax;

  if (data.length > 0) {
    if ('value2' in data[0]) {
      line2 = d3.line()
        .x(function (d) { return x(d.key); })
        .y(function (d) { return y(d.value2); });
    }

    if ('value3' in data[0]) {
      line3 = d3.line()
        .x(function (d) { return x(d.key); })
        .y(function (d) { return y(d.value3); });
    }

    if ('value4' in data[0]) {
      line4 = d3.line()
        .x(function (d) { return x(d.key); })
        .y(function (d) { return y(d.value4); });
    }
  }

  // remove existing chart
  d3.select('#' + id).remove();

  x = d3.scaleTime()
    .rangeRound([0, w])
    .domain(d3.extent(data, function (d) { return d.key; }));

  yMax = d3.max(data, function (d) { return d.value; });

  if (line2) { yMax = Math.max(yMax, d3.max(data, function (d) { return d.value2; })); }
  if (line3) { yMax = Math.max(yMax, d3.max(data, function (d) { return d.value3; })); }
  if (line4) { yMax = Math.max(yMax, d3.max(data, function (d) { return d.value4; })); }

  y = d3.scaleLinear()
    .rangeRound([h, 0])
    .domain([0, yMax]);

  line = d3.line()
    .x(function(d) { return x(d.key); })
    .y(function(d) { return y(d.value); });

  svg = d3.select('#' + elementId)
    .append('svg')
    .attr('id', id)
    .attr('style', 'background: ' + chartColors[0] + '; display: block;')   // 'display: block' rids of extra space below element
    .attr('width', padding.left + w + padding.right)
    .attr('height', padding.top + h + padding.bottom);

  // add the X gridlines
  svg.append('g')
    .attr('style', 'stroke-opacity: 0.1; shape-rendering: crispEdges;')
    .attr('transform', 'translate(' + padding.left + ',' + (padding.top + h) + ')')
    .call(makeXGridLines(x)
      .tickSize(h * -1)
      .tickFormat('')
    );

  // add the Y gridlines
  svg.append("g")
    .attr('style', 'stroke-opacity: 0.1; shape-rendering: crispEdges;')
    .attr('transform', 'translate(' + padding.left + ',' + padding.top + ')')
    .call(makeYGridLines(y)
      .tickSize(w * -1)
      .tickFormat('')
    );

  g = svg.append('g')
    .attr('transform', 'translate(' + padding.left + "," + padding.top + ')');

  if (xAS === 1) {
    g.append('g')
      .attr('transform', 'translate(0,' + h + ')')
      .call(d3.axisBottom(x));
  } else {
    g.append('g')
      .attr('transform', 'translate(0,' + h + ')')
      .call(d3.axisBottom(x)
        .tickFormat(function (date) {
          return d3.timeFormat('%b %d')(date);
        }
      )
    );
  }

  yAxis = g.append("g")
    .call(d3.axisLeft(y));

  if (yAT) {
    yAxis.append("text")
      .attr("fill", chartColors[2])
      .attr("transform", "rotate(-90)")
      .attr("y", 6)
      .attr("dy", ".71rem")
      .attr("text-anchor", "end")
      .text(yAT);
  }

  drawLine(g, data, line4, chartColors[5]);
  drawLine(g, data, line3, chartColors[4]);
  drawLine(g, data, line2, chartColors[3]);
  drawLine(g, data, line, chartColors[2], 1.5);
}

function drawLine (g, data, line, color, width = 1.2) {

  if (!line) { return; }

  g.append("path")
    .datum(data)
    .attr("fill", "none")
    .attr("stroke", color)
    .attr("stroke-linejoin", "round")
    .attr("stroke-linecap", "round")
    .attr("stroke-width", width)
    .attr("d", line);
}

function makeXGridLines (x) {
  return d3.axisBottom(x);
}

function makeYGridLines (y) {
  return d3.axisLeft(y);
}
