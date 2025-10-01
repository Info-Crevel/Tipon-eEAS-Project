<template>

<table role="grid" class="no-border">
  <thead>
    <tr>
      <th class="p-0 pr-1">
        <button type="button" :class="navButtonClass" @click="goPrevious()"><i :class="getIconClass(iconLeft)"></i></button>
      </th>
      <th class="p-0" colspan="3">
        <button type="button" :class="navButtonClass">{{ yearText }}</button>
      </th>
      <th class="p-0 pl-1">
        <button type="button" :class="navButtonClass" @click="goNext()"><i :class="getIconClass(iconRight)"></i></button>
      </th>
    </tr>
  </thead>

  <tbody>
    <tr v-for="(row, i) in rows" :key="i">
      <td v-for="year in row" :key="year" class="w-20 px-0 py-1">
        <button
          :class="getButtonClass(year)"
          @click="changeView(year)">
          <span class="sm-2">{{ year }}</span>
        </button>
      </td>
    </tr>
  </tbody>

</table>

</template>

<script>

export default {

  props: {
    year: { type: Number, default: new Date().getFullYear() },
    iconLeft: { type: String, default: 'chevron-left' },
    iconRight: { type: String, default: 'chevron-right' }
  },

  computed: {
    yearText () {
      let start = this.year - this.year % 20;
      return `${start} ~ ${start + 19}`
    },

    navButtonClass () {
      return 'info-light w-100 text-center sm-2 static';
    },

    rows () {
      let rows = [];
      let yearsStart = this.year - this.year % 20
      for (let i = 0; i < 4; i++) {
        rows.push([]);
        for (let j = 0; j < 5; j++) {
          rows[i].push(yearsStart + i * 5 + j);
        }
      }
      return rows;
    }
  },

  methods: {
    getButtonClass (year) {
      const yearClass = 'border-0 w-100 text-center static';
      return yearClass + ' ' + (year === this.year ? 'info' : 'light');
    },

    getIconClass (icon) {
      return 'fa fa-' + icon;
    },

    goPrevious () {
      this.$emit('year-change', this.year - 20);
    },

    goNext () {
      this.$emit('year-change', this.year + 20);
    },

    changeView (year) {
      this.$emit('year-change', year);
      this.$emit('view-change', 'm');
    }
  }
}

</script>
