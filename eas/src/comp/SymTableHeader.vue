<template>
<thead :class="headerClass">
  <slot></slot>
</thead>
</template>

<script>

export default {
  inject: [
    'theme'
  ],

  props: {
    colorClass: String
  },

  computed: {
    headerClass () {
      return this.colorClass || this.theme.tableHeaderColor;
    }
  }

}

</script>
