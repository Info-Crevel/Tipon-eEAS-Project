<template>

<button class="input-button light" type="button" tabindex="-1" @click="$emit('click')" @mousedown="onMouseDown($event)"><i :class="iconClass"></i>
</button>

</template>

<script>

export default {

  props: {
    icon: String
  },

  computed: {
    iconClass () { return 'fa fa-fw fa-' + this.icon; }
  },

  methods: {
    onMouseDown (e) {
      // prevent button focus
      e.preventDefault();
    }
  }

};

</script>
