<template>

<router-link :to="path" :event="clickEvent" @click.native="click" v-bind="$attrs" :key="ts" ref="link">
  <slot>{{ linkName }}</slot>
</router-link>

</template>

<script>

export default {
  props: {
    to: String
  },

  data () {
    return {
      disabled: false,
      ts: 0
    };
  },

  computed: {
    path () { return this.to ? this.sym.getPath(this.to) : ''; },
    linkName () { return this.to ? this.sym.getPageName(this.to) : ''; },
    clickEvent () { return this.disabled ? null : 'click' }
  },

  mounted () {
    const
      me = this,
      anchor = me.$router.currentRoute.hash;

    me.$nextTick( () => {
      if (anchor && document.querySelector(anchor)) {
        location.href = anchor;
      }
    });
  },

  methods: {
    click () {
      const me = this;

      if (me.dom.hasClass(me.$el, 'disabled') || me.dom.hasAttribute(me.$el, 'disabled')) {
        return;
      }

      me.$emit('click');
      if (me.core.isSmallDevice()) {
        me.ts++;
      }

      const href = me.$router.currentRoute.hash;
      if (href) {
        me.$nextTick( () => {
          const el = document.querySelector(href);
          if (el) {
            me.dom.scrollIntoView(el);
          }
        });
      }
    }
  }

};

</script>
