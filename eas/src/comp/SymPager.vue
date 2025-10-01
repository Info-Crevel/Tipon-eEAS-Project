<template>
<nav v-show="!(hideSinglePage && totalPages === 1)" :class="navClass" :key="ts">
  <div class="buttons" fw-10>
    <button v-if="boundaryLinks" :class="buttonClass(-1)" :disabled="isDisabled(-1)" @click.prevent="onPageChange(1)" aria-label="First page"><i class="fa fa-step-backward"></i></button>
    <button v-if="totalPages > maxSize" :class="buttonClass(-2)" :disabled="isDisabled(-2)" @click.prevent="toPage(1)" aria-label="Previous group"><i class="fa fa-ellipsis-h"></i></button>
    <button v-if="directionLinks && totalPages > maxSize" :class="buttonClass(-3)" :disabled="isDisabled(-3)" @click.prevent="onPageChange(value-1)" aria-label="Previous page"><i class="fa fa-lg fa-caret-left"></i></button>
    <button v-for="item in pageArray" :key="item" :class="buttonClass(item)" @click.prevent="onPageChange(item + 1)">{{ item + 1 }}</button>
    <button v-if="directionLinks && totalPages > maxSize" :class="buttonClass(-4)" :disabled="isDisabled(-4)" @click.prevent="onPageChange(value+1)" aria-label="Next page"><i class="fa fa-lg fa-caret-right"></i></button>
    <button v-if="totalPages > maxSize" :class="buttonClass(-5)" :disabled="isDisabled(-5)" @click.prevent="toPage(0)" aria-label="Next group" class="fa fa-ellipsis-h"></button>
    <button v-if="boundaryLinks" :class="buttonClass(-6)" :disabled="isDisabled(-6)" @click.prevent="onPageChange(totalPages)"><i class="fa fa-step-forward"></i></button>
  </div>
</nav>
</template>

<script>

import {
  config,
} from '../js/core';

export default {
  inject: {
   'injTheme': { from: 'theme' }
  },

  props: {
    value: {
      type: Number,
      required: true,
      validator: v => v >= 0
    },
    totalPages: {
      type: Number,
      required: true,
      validator: v => v >= 0
    },
    maxSize: {
      type: Number,
      default: 5,
      validator: v => v >= 0
    },
    directionLinks: {
      type: Boolean,
      default: true
    },
    boundaryLinks: {
      type: Boolean,
      default: false
    },
    hideSinglePage: {
      type: Boolean,
      default: true
    },
    roundButtons: {
      type: Boolean,
      default: false
    },
    align: String,
    disabled: Boolean,
    themeId: {
      type: String,
      validator: (value) => {
        return config.themes.indexOf(value) !== -1;
      }
    }
  },

  data () {
    return {
      ts: 0,
      rangeStart: 0
    }
  },

  computed: {
    theme () {
      return this.themeId ? this.core.getTheme(this.themeId) : this.injTheme;
    },

    navClass () {
      return {
        [`text-${this.align}`]: Boolean(this.align)
      };
    },

    pageArray () {
      const me = this;
      return me.getArrayRange(me.totalPages).slice(me.rangeStart, me.rangeStart + me.maxSize);
    }

  },

  methods: {
    buttonClass (item) {
      const me = this;
      let cls = me.roundButtons ? 'circle ' : '';

      if (item < 0) {
        if (me.isDisabled(item)) {
          return cls + 'gainsboro text-muted border-main disabled';
        }
      }

      if (me.value === item + 1) {
        return cls + me.theme.baseColor;
      } else {
        return cls + me.theme.buttonColor;
      }
    },

    isDisabled (item) {
      const me = this;
      let disabled = false;

      if (item === -1) {
        disabled = me.value <= 1;
      }
      if (item === -2) {
        disabled = me.rangeStart === 0;
      }
      if (item === -3) {
        disabled = me.value === 1;
      }
      if (item === -4) {
        disabled = me.value === me.totalPages;
      }
      if (item === -5) {
        disabled = me.rangeStart >= me.totalPages - me.maxSize;
      }
      if (item === -6) {
        disabled = me.value >= me.totalPages;
      }
      return disabled;
    },

    computeRangeStart () {
      const
        me = this,
        oldStart = me.rangeStart;

      if (me.value > me.rangeStart + me.maxSize) {
        if (me.value > me.totalPages - me.maxSize) {
          me.rangeStart = me.totalPages - me.maxSize;
        } else {
          me.rangeStart = me.value - 1;
        }
      } else if (me.value < me.rangeStart + 1) {
        if (me.value > me.maxSize) {
          me.rangeStart = me.value - me.maxSize;
        } else {
          me.rangeStart = 0;
        }
      }
      if (oldStart !== me.rangeStart) {
        me.eventBus.$emit('pager:rangeChanged', me.rangeStart);
      }
    },

    toPage (pre) {
      if (this.disabled) {
        return;
      }
      const
        me = this,
        oldStart = me.rangeStart,
        start = pre ? me.rangeStart - me.maxSize : me.rangeStart + me.maxSize;

      if (start < 0) {
        me.rangeStart = 0;
      } else if (start > me.totalPages - me.maxSize) {
        me.rangeStart = me.totalPages - me.maxSize;
      } else {
        me.rangeStart = start;
      }
      if (oldStart !== me.rangeStart) {
        me.eventBus.$emit('pager:rangeChanged', me.rangeStart);
      }
    },

    refresh () {
      this.ts +=1;
    },

    onPageChange (page) {
      if (!this.disabled && page > 0 && page <= this.totalPages && page !== this.value) {
        this.$emit('input', page);
        this.$emit('change', page);
      }
    },

    getArrayRange (end, start = 0, step = 1) {
      const a = [];
      for (let i = start; i < end; i += step) {
        a.push(i);
      }
      return a;
    }
  },

  created () {
    this.$watch( vm => [vm.value, vm.maxSize, vm.totalPages].join(), this.computeRangeStart, {
      immediate: true
    });

    this.eventBus.$on('pager:rangeChanged', (val) => {
      this.rangeStart = val;
    });
  },

  watch: {
    rangeStart (val, oldVal) {
      if (val !== oldVal) {
        this.refresh();
      }
    }
  }

}

</script>
