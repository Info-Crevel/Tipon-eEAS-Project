<template>
<form :id="id" @submit.prevent autocomplete="off">
  <sym-card :class="cardClassList" :headerClass="headerClassList" :bodyClass="bodyClassList" :footerClass="footerClass" bodyId="body">
    <div v-if="caption" slot="header" class="d-flex justify-between align-items-center" @click.ctrl.shift.exact="showPageId">
      <span v-html="caption"></span>
      <slot name="caption-tool" />
    </div>
    <div slot="img-top" class="form-toolbar">
      <slot name="toolbar" />
    </div>
    <slot></slot>
    <div slot="footer" v-if="hasFooter">
      <slot name="footer" />
    </div>
  </sym-card>
</form>

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
      theme: this.theme
    };
  },

  props: {
    id: { type: String },
    caption: String,
    cardClass: String,
    headerClass: String,
    bodyClass: String,
    footerClass: String,
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

    cardClassList () {
      return 'border-main mb-0' + (this.cardClass ? ' ' + this.cardClass : '');
    },

    headerClassList () {
      if (this.caption) {
        let cls = 'lg-2 form-caption ' + this.theme.formCaptionColor;
        return this.headerClass ? cls + ' ' + this.headerClass : cls;
      } else {
        return 'p-0';
      }
    },

    bodyClassList () {
      return 'white' + (this.bodyClass ? ' ' + this.bodyClass : '');
    },

    formCaptionColor () {
      return this.themeId ? 'caption-' + this.themeId : this.core.config.formCaptionColor;
    },

    hasFooter () { return 'footer' in this.$slots; }

  },

  methods: {
    showPageId () {
      this.advice.info('Page ID: ' + this.id, { alignment: 'top-left'});
    }

  }

}

</script>
