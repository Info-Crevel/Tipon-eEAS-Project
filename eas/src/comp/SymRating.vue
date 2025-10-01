<template>
<div>
  <i class="text-shadow star" :title="index" :class="getIconClass(index)" v-for="index in starCount" :key="index" @click="onClick(index)"></i>
</div>
</template>

<script>

export default {
  props: {
    value: {
      type: Number,
      default: 3,
      required: true
    },
    starCount: { type: Number, default: 5 },
    iconClass: { type: String, default: 'text-goldenrod' },
    iconName: { type: String, default: 'star' } // FontAwesome font name
  },

  methods: {
    onClick (index) {
      if (index > 1 && index === this.value) {
        index = index - 1;
      }
      this.$emit('input', index);
    },

    getIconClass (index) {
      let cls = 'fa fa-lg fa-' + this.iconName;
      if (index < this.starCount) {
        cls = cls + ' mr-2';
      }
      if (index <= this.value) {
        cls = cls + ' ' + this.iconClass;
      }
      return cls;
    }
  }

}

</script>

<style scoped>

.star:hover {
  cursor: pointer;
}

</style>