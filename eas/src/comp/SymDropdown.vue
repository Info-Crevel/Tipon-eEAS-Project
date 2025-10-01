<template>

<div :class="classList" @keydown.esc="close()">
  <slot name="toggle"></slot>
  <slot></slot>
</div>

</template>

<script>

export default {
  props: {
    value: {
      type: Boolean,
      default: false
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },

  data () {
    return {
      show: false,
      bodyEl: null,
      triggerEl: null
    };
  },

  computed: {
    classList () {
      return {
        show: this.show
      }
    }
  },

  methods: {
    toggle (show) {
      const me = this;
      if (me.disabled) {
        return;
      }
      if (me.core.isBoolean(show)) {
        me.show = show;
      } else {
        me.show = !me.show
      }
      me.$emit('input', me.show);
      if (me.show) {
        me.dom.scrollIntoView(me.bodyEl);
        if (me.ignoreCloseElements.length) {
          setTimeout( () => {
            me.ignoreCloseElements[0].focus();
          }, 600);
        }
      }
    },

    windowClicked (e) {
      const
        me = this,
        target = e.target,
        ignoreElements = me.ignoreCloseElements;

      if (me.show && target) {
        let targetInIgnoreElements = false;
        if (ignoreElements) {
          for (let i = 0, l = ignoreElements.length; i < l; i++) {
            targetInIgnoreElements = ignoreElements[i].contains(target);
            if (targetInIgnoreElements) {
              break;
            }
          }
        }

        let targetInBody = me.bodyEl ? me.bodyEl.contains(target) : false;
        let targetInTrigger = me.triggerEl.contains(target);
        if ((!targetInTrigger || targetInBody) && !targetInIgnoreElements) {
          me.toggle(false);
        }

      }

    },

    close () {
      this.toggle(false);
    }
  },

  watch: {
    value (v) {
      this.toggle(v);
    }
  },

  created () {
    this.ignoreCloseElements = [];
  },

  mounted () {
    const
      me = this,
      trigger = me.$el.querySelector('.drop-toggle') || me.$el.firstChild;

    me.triggerEl = trigger && trigger !== me.$refs.dropdown ? trigger : null;
    me.bodyEl = me.$el.querySelector('.dropbox');

    if (me.bodyEl) {
      let elements = me.bodyEl.querySelectorAll('input[data-ignore-close="true"]');
      elements.forEach(element => {
        me.ignoreCloseElements.push(element);
      });
    }

    if (me.triggerEl && !me.triggerEl.hasAttribute('disabled')) {
      me.dom.on(me.triggerEl, 'click', me.toggle);
    }
    me.dom.on(window, 'click', me.windowClicked);
    if (me.value) {
      me.toggle(true);
    }
  },

  beforeDestroy () {
    const me = this;
    if (me.triggerEl) {
      me.dom.off(me.triggerEl, 'click', me.toggle);
    }
    me.dom.off(window, 'click', me.windowClicked);
  }

}

</script>