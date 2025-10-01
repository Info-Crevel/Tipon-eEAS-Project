<template>

<div class="tab-pane" role="tabpanel">
  <slot></slot>
</div>

</template>

<script>

const ACTIVE = 'active';

export default {
  props: {
    title: { type: String, default: 'Tab' },
    icon: String,
    disabled: { type: Boolean, default: false },
    'tab-class': { type: Object, default: () => { return {} } },
    group: String
  },

  data () {
    return {
      active: true,
      transitionDuration: 150
    };
  },

  computed: {
    iconClass () {
      return 'fa fa-lg fa-' + this.icon + ' mr-1';
    },
  },

  watch: {
    active (isActive) {
      const
        me = this,
        el = me.$el;

      if (isActive) {
        setTimeout( () => {
          el.classList.add(ACTIVE);
          el.offsetHeight;
        }, me.transitionDuration);
      } else {
        setTimeout( () => {
          el.classList.remove(ACTIVE);
        }, me.transitionDuration);
      }
    }

  },

  created () {
    try {
      this.$parent.tabs.push(this);
    } catch (e) {
      throw new Error('<tab> parent must be <tabs>.');
    }
  },

  beforeDestroy () {
    let tabs = this.$parent && this.$parent.tabs;
    this.core.deleteArrayItem(tabs, this);
  },

  methods: {
    show () {
      this.$nextTick( () => {
        this.$el.classList.add(ACTIVE);
      })
    }
  }

}

</script>
