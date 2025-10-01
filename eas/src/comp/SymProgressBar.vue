<template>

<div class="progress" :key="ts">
  <div class="progress-bar" :class="barClass" :style="barStyle">
    <span v-show="showText" class="my-auto">{{ percentage }}%</span>
  </div>
</div>

</template>

<script>

import {
  config,
} from '../js/core';

export default {
  props: {
    value: {
      type: Number,
      default: 0
    },
    maxValue: {
      type: Number,
      default: 100
    },
    showText: {
      type: Boolean,
      default: false
    },
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
      loaderValue: 0
    }
  },

  computed: {
    theme () {
      return this.themeId ? this.core.getTheme(this.themeId) : this.core.config.theme;
    },

    percentage () {
      const p = (((this.value + this.loaderValue)/ this.maxValue) * 100).toFixed(2);
      return Math.min(p, 100);
    },

    barStyle () {
      let s = {
        'width': this.percentage + '%'
      }
      return s;
    },

    barClass () {
      return this.theme.baseColor;
    }

  },

  methods: {
    refresh () {
      this.ts +=1;
    }
  }

}

</script>
