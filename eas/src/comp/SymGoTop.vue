<template>

<transition :name="transition">
  <!-- <button type="button" @click="goTop" :class="buttonClass" title="Go to top" v-if="isVisible"> -->
  <button type="button" @click="goTop" :class="buttonClass" title="Go to top" v-show="isVisible">
    <i v-if="icon" :class="iconClass"></i><span v-if="caption">{{ caption }}</span>
  </button>
</transition>

</template>

<script>

export default {
  props: {
    caption: String,
    buttonClass: { type: String, default: '' },
    icon: { stype: String, default: 'arrow-up' },
    transition: { type: String, default: 'fader' },
    offset: { type: Number, default: 100 },
    offsetBottom: { type: Number, default: 0 },
    isTool: { type: Boolean, default: false },
    scrollFunction: {
      type: Function,
      default: function () {}
    }
  },

  computed: {
    iconClass () {
      return this.icon ? 'fa ' + (this.isTool ? 'fa-lg ' : 'fa-2x ') + 'fa-' + this.icon : '';
      // return this.icon ? 'fa ' + (this.isTool ? 'fa-lg ' : 'fa-2x mx-1 ') + 'fa-' + this.icon : '';
    },

    isVisible () {
      return this.isTool || (this.visible && this.dom.hasScrollbar());
    }
  },

  data () {
    return {
      visible: false
    };
  },

  methods: {
    handleScroll () {
      const
        me = this,
        pastTopOffset = window.pageYOffset > parseInt(me.offset),
        pastBottomOffset = window.innerHeight + window.pageYOffset >= document.body.offsetHeight - parseInt(me.offsetBottom);

      me.visible = parseInt(me.offsetBottom) > 0 ? pastTopOffset && !pastBottomOffset : pastTopOffset;
      me.scrollFunction(me);
    },

    goTop () {
      window.scrollToTop();
      this.$emit('scrolled');
    }

  },

  mounted () {
    window.scrollToTop();
    this.dom.on(window, 'scroll', this.handleScroll);
  },

  destroyed () {
    this.dom.off(window, 'scroll', this.handleScroll);
  }

}

</script>

<style scoped>

.fader-enter-active, .fader-leave-active {
  transition: opacity .75s ease-in-out;
}

.fader-enter, .fader-leave-to {
  opacity: 0;
}

</style>
