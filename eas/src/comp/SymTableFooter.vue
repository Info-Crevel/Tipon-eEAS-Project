<template>
<thead :class="footerClass">
  <slot></slot>
</thead>
</template>

<script>

export default {
  inject: {
    'theme': { from: 'theme'},
    'injColorClass': { from: 'colorClass' }
  },

  props: {
    colorClass: String
  },

  computed: {
    footerClass () {
      return this.injColorClass || this.colorClass || this.theme.tableCaptionColor;
    }
  }

}

</script>
