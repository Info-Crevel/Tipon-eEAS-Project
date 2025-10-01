<template>

<transition :name="transition" ref="alert">
  <div v-if="isVisible" role="alert" :class="alertClass">
    <div class="media">
      <span v-if="iconName" class="mr-2 pt-0"><i :class="iconClass"></i></span>
      <div class="media-body">
        <button type="button" class="close mb-0 pb-2" aria-label="Close" v-if="dismissible" @click="closeAlert">
          <span aria-hidden='true'>&times;</span>
        </button>
        <h6 class="mb-1" v-if="title">{{ title }}</h6>
        <slot></slot>
      </div>
    </div>
  </div>
</transition>

</template>

<script>

export default {
  props: {
    type: { type: String, default: 'info' },
    title: String,
    duration: { type: Number, default: 0 },
    dismissible: { type: Boolean, default: false },
    icon: String,
    transition: { type: String, default: 'fade' },
    visible: { type: Boolean, default: true }
  },

  data () {
    return {
      timeout: 0,
      isVisible: this.visible,
      autoIcon: ''
    };
  },

  computed: {
    alertClass () {
      return {
        'alert': true,
        'alert-dismissible': this.dismissible
      }
    },

    iconClass () {
      return 'fa fa-lg fa-' + this.iconName;
    },

    iconName () {
      return this.autoIcon === 'none' ? undefined : this.autoIcon;
    }

  },

  methods: {
    closeAlert () {
      clearTimeout(this.timeout);
      this.$emit('dismissed');
      this.isVisible = false;
    }
  },

  mounted () {
    if (this.duration) {
      this.timeout = setTimeout(this.closeAlert, this.duration * 1000);
    }
    let icon = this.icon;
    if (!this.icon) {
      const
        hasClass = this.dom.hasClass,
        el = this.$el;

      icon = 'info-circle';
      if (hasClass(el, 'success') || hasClass(el, 'success-light')) { icon = 'check-circle'; }
      if (hasClass(el, 'warning') || hasClass(el, 'warning-light')) { icon = 'warning'; }
      if (hasClass(el, 'danger') || hasClass(el, 'danger-light')) { icon = 'times-circle'; }
    }
    this.autoIcon = icon;

  },

  destroyed () {
    clearTimeout(this.timeout);
  }

};

</script>
