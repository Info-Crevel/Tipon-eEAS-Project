<template>
<tbody :class="bodyClass">
  <slot></slot>
</tbody>
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
    bodyClass () {
      return this.core.config.tableBodyColor || this.injColorClass || this.colorClass || this.theme.tableColor;
    }
  }

}

</script>
