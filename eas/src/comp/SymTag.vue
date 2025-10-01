<template>

<span :class="tagClass">
  <slot></slot>
</span>

</template>

<script>

export default {
  props: {
    width: { type: Number, default: 22 }
  },

  computed: {
    tagClass () {
      return 'info-tag fw-' + this.width;
    }
  }

}

</script>