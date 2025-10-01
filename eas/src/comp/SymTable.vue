<template>

<table :class="tableClass">
  <caption v-if="caption" :class="captionClassList" >
    <div class="d-flex justify-between align-items-center">
      <span v-html="caption"></span>
      <slot name="caption-tool"/>
    </div>
  </caption>
  <slot name="header"/>
  <slot name="body"/>
  <slot name="footer"/>
</table>

</template>

<script>

import {
  config,
} from '../js/core';

export default {
  inject: {
    'injTheme': { from: 'theme' }
  },

  provide () {
    return {
      theme: this.theme,
      colorClass: this.colorClass
    };
  },

  props: {
    caption: String,
    captionClass: String,
    colorClass: String,
    themeId: {
      type: String,
      validator: (value) => {
        return config.themes.indexOf(value) !== -1;
      }
    }
  },

  computed: {
    theme () {
      return this.themeId ? this.core.getTheme(this.themeId) : this.injTheme;
    },

    tableClass () {
      return this.colorClass || this.theme.tableColor;
    },

    captionClassList () {
      let cls = (this.colorClass || this.theme.tableCaptionColor) + ' darken-1';
      return this.captionClass ? cls + ' ' + this.captionClass : cls;
    },

  }

}

</script>
