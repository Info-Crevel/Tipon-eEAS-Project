<template>

<div
  :style="pickerStyle"
  data-role="date-picker"
  @click="onPickerClick"
  class='box-border light p-1'
  @keydown="onKeyDown"
>
  <pop-day
    v-show="view === 'd'"
    :month="month"
    :year="year"
    :date="dateValue"
    :today="today"
    :range="range"
    :icon-left="iconLeft"
    :icon-right="iconRight"
    @month-change="onMonthChange"
    @year-change="onYearChange"
    @date-change="onDateChange"
    @view-change="onViewChange"
  />
  <pop-month
    v-show="view === 'm'"
    :month="month"
    :year="year"
    :icon-left="iconLeft"
    :icon-right="iconRight"
    @month-change="onMonthChange"
    @year-change="onYearChange"
    @view-change="onViewChange"
    ref="month"
  />
  <pop-year
    v-show="view === 'y'"
    :year="year"
    :icon-left="iconLeft"
    :icon-right="iconRight"
    @year-change="onYearChange"
    @view-change="onViewChange"
  />
  <div class="buttons d-block text-center mb-2" fw-12>
    <button type="button" class='success-light sm-3 static' data-action="select" v-if="todayButton" @click="selectToday()">Today</button>
    <button type="button" class='info-light sm-3 static' data-action="select" v-if="clearButton" @click="selectClear()">Clear</button>
    <button type="button" class='danger-light sm-3 ml-2 static' @click="closePicker()" ref="close">Close</button>
  </div>

</div>

</template>

<script>

import {
  setup
} from '../js/app';

import {
  DateTime,
  daysInMonth
} from '../js/core';

const Day = () => import(/* webpackChunkName: "controls" */ './PopDay.vue');
const Month = () => import(/* webpackChunkName: "controls" */ './PopMonth.vue');
const Year = () => import(/* webpackChunkName: "controls" */ './PopYear.vue');

export default {
  components: {
    'pop-day': Day,
    'pop-month': Month,
    'pop-year': Year
  },

  props: {
    value: null,
    width: { type: Number, default: 256 },
    minDate: null,
    maxDate: null,
    format: { type: String, default: setup.dateFormat },
    initialView: { type: String, default: 'd' },
    iconLeft: { type: String, default: 'chevron-left' },
    iconRight: { type: String, default: 'chevron-right' },
    closeOnSelect: { type: Boolean, default: true },
    todayButton: { type: Boolean, default: false },
    clearButton: { type: Boolean, default: false },
    dateControl: { type: Object, default: null },
    defaultDay: { type: Number, default: 0 }
  },

  data () {
    return {
      today: DateTime.today(),
      month: 0,
      year: 0,
      view: 'd'
    };
  },

  computed: {
    dateValue () {
      if (this.core.isDate(this.value)) {
        return this.core.toDate(this.value);
      }
      if (this.minDate && this.minDate.compare(this.today) === 1) {
        return this.minDate;
      }
      if (this.maxDate && this.today.compare(this.maxDate) === 1) {
        return this.maxDate;
      }

      return this.today;
    },

    pickerStyle () {
      return { width: this.width + 'px'};
    },

    range () {
      let range = {};
      if (this.minDate) {
        range.minDate = this.minDate;
      }
      if (this.maxDate) {
        range.maxDate = this.maxDate;
      }
      return range;
    }

  },

  methods: {
    setMonthYearFromValue (val, oldVal) {
      if (val && this.defaultDay) {
        let
          m = parseInt(val.substr(0, 2)),
          y = parseInt(val.substr(3, 4));
          d = this.defaultDay

        if (this.defaultDay > 30) {
          d = daysInMonth(y, m);
        }
        val = m + '/' + d + '/' + y;
      }

      let d = new DateTime(val);
      if (d.isEmpty()) {
        return;
      }

      const
        me = this,
        range = me.range,
        isDef = me.core.isDefined;

      if (range) {
        let
          lessMin = isDef(range.minDate) && (d.compare(range.minDate) === -1),
          overMax = isDef(range.maxDate) && (d.compare(range.maxDate) === 1);

        if (lessMin || overMax) {
          me.$emit('input', oldVal || '');
          return;
        }
      }

      me.month = d.month;
      me.year = d.year;
    },

    onMonthChange (month) {
      this.month = month;

      if (this.defaultDay) {
        this.onDateChange({
          year: this.year,
          month: this.month,
          day: this.defaultDay > 30 ? daysInMonth(this.year, this.month) : this.defaultDay
        })
      }
    },

    onYearChange (year) {
      this.year = year;
      this.month = 1;
    },

    onDateChange (d) {
      let date = null;
      if (d) {
        date = new DateTime(d.year, d.month, d.day);
      }
      if (date && !date.isEmpty()) {
        this.$emit('input', date.toDateFormat());
      } else {
        this.$emit('input', '');
      }
    },

    onViewChange (view) {
      this.view = view;
    },

    selectToday () {
      const
        me = this,
        today = me.today;

      me.view = 'd'
      me.onDateChange({
        year: today.year,
        month: today.month,
        day: today.day
      })
    },

    selectClear () {
      const
        me = this,
        today = me.today;

      me.month = today.month;
      me.year = today.year;
      me.view = me.initialView;
      me.onDateChange();
    },

    selectClose () {
      this.dateControl.isPickerVisible = false;
    },

    onPickerClick (event) {
      if (event.target.getAttribute('data-action') !== 'select' || !this.closeOnSelect) {
        event.stopPropagation();
      }
    },

    onKeyDown (e) {
      if (e.code === "Escape") {
        this.closePicker();
      }
    },

    closePicker () {
      if (this.dateControl) {
        this.dateControl.isPickerVisible = false;
        this.dateControl.setInputFocus();
      }
    }

  },

  mounted () {
    const me = this;
    if (me.value) {
      me.setMonthYearFromValue(me.value)
    } else {
      me.month = me.today.month;
      me.year = me.today.year;
      me.view = me.initialView;
    }
  },

  watch: {
    value (val, oldVal) {
      this.setMonthYearFromValue(val, oldVal);
    },

    view () {
      this.$refs.close.focus();
    }
  }

}

</script>
