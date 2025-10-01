<template>

<table role="grid" class="no-border">
  <thead>
    <tr>
      <th class="p-0 pr-1">
        <button type="button" :class="navButtonClass" @click="goPrevious()"><i :class="getIconClass(iconLeft)"></i></button>
      </th>
      <th class="p-0">
        <button type="button" :class="navButtonClass" @click="changeView()">{{ year }}</button>
      </th>
      <th class="p-0 pl-1">
        <button type="button" :class="navButtonClass" @click="goNext()"><i :class="getIconClass(iconRight)"></i></button>
      </th>
    </tr>
  </thead>

  <tbody>
    <tr v-for="(row, i) in rows" :key="i">
      <td v-for="(month, j) in row" :key="j" class="px-0 py-1 text-narrow">
        <button
          :class="getButtonClass(month)"
          type="button"
          @click="changeView((i * 3) + j + 1)">
          <span class="sm-2">{{ core.getMonthName(month) }}</span>
        </button>
      </td>
    </tr>
  </tbody>

</table>

</template>

<script>

export default {

  props: {
    month: Number,
    year: Number,
    iconLeft: { type: String, default: 'chevron-left' },
    iconRight: { type: String, default: 'chevron-right' }
  },

  data () {
    return {
      rows: []
    };
  },

  computed: {
    navButtonClass () {
      return 'info-light w-100 text-center sm-2 static';
    }
  },

  methods: {
    getButtonClass (month) {
      const monthClass = 'border-0 w-100 text-center static';
      return monthClass + ' ' + (month === this.month ? 'info' : 'light');
    },

    getIconClass (icon) {
      return 'fa fa-' + icon;
    },

    goPrevious () {
      this.$emit('year-change', this.year - 1);
    },

    goNext () {
      this.$emit('year-change', this.year + 1);
    },

    changeView (monthIndex) {
      if (this.core.isDefined(monthIndex)) {
        this.$emit('month-change', monthIndex);
        this.$emit('view-change', 'd');
      } else {
        this.$emit('view-change', 'y');
      }
    }

  },

  mounted () {
    for (let i = 0; i < 4; i++) {
      this.rows.push([]);
      for (let j = 0; j < 3; j++) {
        this.rows[i].push(i * 3 + j + 1);
      }
    }
  }

}

</script>
