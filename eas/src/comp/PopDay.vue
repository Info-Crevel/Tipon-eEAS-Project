<template>

<table class="no-border plain">
  <thead>
    <tr>
      <th colspan="1" class="p-0">
        <button type="button" :class="navButtonClass" @click="goPrevious()"><i :class="getIconClass(iconLeft)"></i></button>
      </th>
      <th colspan="5" class="px-1 py-0">
        <button type="button" :class="navButtonClass" @click="changeView()">{{ yearMonthText }}</button>
      </th>
      <th colspan="1" class="p-0">
        <button type="button" :class="navButtonClass" @click="goNext()"><i :class="getIconClass(iconRight)"></i></button>
      </th>
    </tr>

    <tr class="lightyellow text-narrow py-0">
      <td v-for="(day, index) in weekDays" :key="index" class="sm-1 px-0 pr-2 text-right">
        <small>{{ dayNames[day] }}</small>
      </td>
    </tr>
  </thead>

  <tbody>
    <tr v-for="(row, i) in monthDayRows" :key="i">
      <td v-for="(date, j) in row" :key="j" class="p-0">
        <button
          :class="getButtonClass(date)"
          :disabled="date.disabled"
          type="button"
          data-action="select"
          @click="select(date)">
          <span :class="{'text-muted': (month !== date.month) || date.disabled }"><small data-action="select">{{ date.day }}</small></span>
        </button>
      </td>
    </tr>
  </tbody>

</table>

</template>

<script>

import {
  DateTime,
} from '../js/core';

export default {
  props: {
    year: Number,
    month: Number,
    date: DateTime,
    today: DateTime,
    range: Object,
    iconLeft: { type: String, default: 'chevron-left' },
    iconRight: { type: String, default: 'chevron-right' }
  },

  data () {
    return {
      dayNames: []
    };
  },

  computed: {
    yearMonthText () {
      return this.core.isDefined(this.month) ? `${this.core.getMonthName(this.month)} ${this.year}` : this.year;
    },

    navButtonClass () {
      return 'info-light w-100 text-center sm-2 static';
    },

    weekDays () {
      let days = [];
      let firstDay = this.core.config.startOfWeek;
      while (days.length < 7) {
        days.push(firstDay++)
        if (firstDay > 6) {
          firstDay = 0
        }
      }
      return days
    },

    monthDayRows () {
      let
        rows = [],
        lastDayPrevMonth = DateTime.endOfMonth(this.year, this.month - 1).day,
        startIndex = new DateTime(this.year, this.month, 1).dayOfWeek,
        days = DateTime.daysInMonth(this.year, this.month),
        weekOffset = 0,
        config = this.core.config;

      if (config.startOfWeek > startIndex) {
        weekOffset = 7 - config.startOfWeek;
      } else {
        weekOffset = 0 - config.startOfWeek;
      }

      for (let i = 0; i < 6; i++) {
        rows.push([]);
        for (let j = 0 - weekOffset; j < 7 - weekOffset; j++) {
          let index = i * 7 + j;
          let o = { year: this.year, disabled: false };

          if (index < startIndex) {
            o.day = lastDayPrevMonth - startIndex + index + 1;
            if (this.month > 0) {
              o.month = this.month - 1;
            } else {
              o.month = 11;
              o.year--;
            }
          } else if (index < startIndex + days) {
            o.day = index - startIndex + 1;
            o.month = this.month;
          } else {
            o.day = index - startIndex - days + 1;
            if (this.month < 11) {
              o.month = this.month + 1;
            } else {
              o.month = 0;
              o.year++;
            }
          }

          // process date range
          if (this.range) {
            let
              d = new DateTime(o.year, o.month, o.day),
              lessMin = this.core.isDefined(this.range.minDate) && (d.compare(this.range.minDate) === -1),
              overMax = this.core.isDefined(this.range.maxDate) && (d.compare(this.range.maxDate) === 1);

            o.disabled = lessMin || overMax;
          }
          rows[i].push(o);
        }
      }
      return rows;
    }

  },

  methods: {
    getButtonClass (d) {
      let
        cd = this.date,   // current selected
        td = this.today,  // today
        backColor = 'light',
        dateClass = 'border-0 w-100 text-right py-0 pr-2 mb-1 static';

      if (d.year === cd.year && d.month === cd.month && d.day === cd.day) {
        backColor = 'info';
      } else if (d.year === td.year && d.month === td.month && d.day === td.day) {
        backColor = 'info-light';
      }
      return dateClass + ' ' + backColor;
    },

    getIconClass (icon) {
      return 'fa fa-' + icon;
    },

    goPrevious () {
      let month = this.month;
      let year = this.year;
      if (this.month > 1) {
        month--;
      } else {
        month = 12;
        year--;
        this.$emit('year-change', year);
      }
      this.$emit('month-change', month);
    },

    goNext () {
      let month = this.month;
      let year = this.year;
      if (this.month < 12) {
        month++;
      } else {
        month = 1;
        year++;
        this.$emit('year-change', year);
      }
      this.$emit('month-change', month);
    },

    changeView () {
      this.$emit('view-change', 'm')
    },

    select (date) {
      this.$emit('date-change', date)
    }

  },

  created () {
    this.dayNames = this.core.getDayNames();
  }

}

</script>
