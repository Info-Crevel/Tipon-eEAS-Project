<template>
<div>
</div>
</template>

<script>

import {
  appConfig
} from '../js/session';

export default {
  data () {
    return {
      events: [
        'click',
        'scroll',
        'keypress'
      ],

      logoutInterval: null,
      timeoutSeconds: 0,
      inactiveSeconds: 0
    }
  },

  methods: {
    enable () {
      const me = this;

      me.resetTimer();

      me.events.forEach( event => {
        me.dom.on(window, event, me.resetDebounced);
      });
    },

    disable () {
      const me = this;

      clearInterval(me.logoutInterval);

      me.events.forEach( event => {
        me.dom.off(window, event, me.resetDebounced);
      });
    },

    resetTimer () {
      const me = this;

      me.logoutInterval = setInterval(() => {
        me.incrementCounter();
      }, 1000);
    },

    resetCounter () {
      this.inactiveSeconds = 0;
    },

    incrementCounter () {
      const me = this;

      me.inactiveSeconds = me.inactiveSeconds + 1;
// console.log(me.inactiveSeconds, me.timeoutSeconds);
      if (me.inactiveSeconds >= me.timeoutSeconds) {
        me.$emit('timeout');
        me.resetCounter();
        me.disable();
      }
    }

  },

  created () {
    const me = this;

    me.resetDebounced = null;

    me.eventBus.$on('user:logon', () => {
      if (appConfig.autoLogoutMinutes) {
        me.enable();
      }
    });

    me.eventBus.$on('user:logoff', () => {
      me.disable();
    });

  },

  beforeDestroy () {
    this.disable();
  },

  mounted () {
    const me = this;

    clearInterval(me.logoutInterval);
    me.resetDebounced = me.core.debounce(me.resetCounter, 250);

    me.timeoutSeconds = appConfig.autoLogoutMinutes * 60;
// me.timeoutSeconds = 5   // for testing;

  }
}

</script>

<style scoped>

</style>