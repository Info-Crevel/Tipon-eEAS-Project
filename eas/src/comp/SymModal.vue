<template>

<div class="modal" tabindex="-1" :class="{ fade : transitionDuration > 0 }" @click.self="backdropClicked">
  <div class="modal-dialog shadow-dark" :class="sizeClass" ref="dialog">
    <div :class="contentClass">
      <div class="card-header d-flex justify-between p-1 align-items-center" :class="headerClass" v-if="header">
        <slot name="header">
          <div class="lg-1 pl-1" v-html="title"></div>
          <button type="button" :class="closeButtonClass" class="py-0" @click="toggle(false)" v-if="dismissible">
            <span aria-hidden="true" v-html="dismissButtonText" class="lg-2 bold"></span>
          </button>
        </slot>
      </div>
      <div id="body" class="card-body" :class="defaultBodyClass" v-if="!customBody">
        <slot>
        </slot>
      </div>
      <div id="custom-body" v-if="customBody">
        <slot>
        </slot>
      </div>
      <footer class="card-footer" :class="defaultFooterClass" v-if="footer">
        <slot name="footer">
        </slot>
      </footer>
    </div>
  </div>
  <div class="modal-backdrop" ref="backdrop" :class="{ fade : transitionDuration > 0 }">
  </div>
</div>

</template>

<script>

const MODAL_BACKDROP = 'modal-backdrop';
const MODAL_OPEN = 'modal-open';
const SHOW = 'show';

const getOpenModalCount = () => document.querySelectorAll(`.${MODAL_BACKDROP}`).length;

export default {
  // inject: {
  //   'injHostPage': { from: 'hostPage'}
  // },

  // model: {
  //   prop: 'value'
  // },

  props: {
    title: String,
    size: String,     // sm, md, rg, lg, xl
    scheme: { type: String, default: 'info' },  // info, warning, success, danger, primary
    type: { type: Number, default: 1 },
    value: { type: Boolean, default: false },
    backdrop: { type: Boolean, default: true },
    header: { type: Boolean, default: true },
    footer: { type: Boolean, default: true },
    closeOnBackButton: { type: Boolean, default: true },
    appendToBody: { type: Boolean, default: false },
    headerClass: { type: String},
    bodyClass: { type: String, default: 'white'},
    customBody: { type: Boolean, default: false },
    customBodyClass: { type: String, default: 'white' },
    keyboard: { type: Boolean, default: true },
    dismissible: { type: Boolean, default: true },
    // dismissButtonText: { type: String, default: '&#x2716'},
    dismissButtonText: { type: String, default: 'X'},
    dismissButtonClass: { type: String },
    transitionDuration: { type: Number, default: 0 },
    zOffset: { type: Number, default: 20 }
  },

  provide () {
    return {
      theme: this.core.config.theme,
      // hostPage: this.hostPage
      hostPage: null
    };
  },

  data () {
    return {
      timeoutId: 0
    };
  },

  computed: {
    // hostPage () {
    //   return this.injHostPage;
    // },

    sizeClass () {
      return {
        [`modal-${this.size}`]: Boolean(this.size)
      };
    },

    closeButtonClass () {
      return (this.dismissButtonClass ? this.dismissButtonClass + ' ' : '') + 'shadow-soft ' + `${this.scheme}`;
    },

    defaultBodyClass () {
      if (this.type === this.tags.NotificationType.Wait) {
        return 'curved dark border-' + `${this.scheme}` + ' p-1';
      }
      return this.type === this.tags.NotificationType.Lookup ? this.bodyClass : this.bodyClass + ' pb-1';
    },

    defaultFooterClass () {
      let cls = '';
      if (this.scheme === 'info') {
        cls = 'whitesmoke darken-1';
        } else {
        cls = `${this.scheme}-light brighten-4`;
      }
      // return 'border-top-main p-2 ' + cls;
      return 'border-top-main p-1 ' + cls;
    },

    contentClass () {
      return 'card h-auto mb-0 ' + (this.customBody ? this.customBodyClass : `${this.scheme}-light`);
    }

  },

  methods: {
    onKeyDown (e) {
      if (this.keyboard && this.value && e.key === 'Escape') {
        this.toggle(false);
      }
    },

    async toggle (show) {
      await this.emitClosingEvent().then(
        e => {
          if (e.defaultPrevented) {
            return;
          }
          this.$emit('input', show);
          this.$toggle(show);
        }
      );
    },

    async emitClosingEvent () {
      const e = new CustomEvent('closing', { cancelable: true, bubbles: false });
      this.$emit('closing', e);
      if (this.core.isFunction(e.callback)) {
        let result = await e.callback(e);
        if (!result) {
          e.preventDefault();
        }
      }
      return e;
    },

    $toggle (show) {
      const
        me = this,
        modal = me.$el,
        backdrop = me.$refs.backdrop,
        dom = me.dom;

      clearTimeout(me.timeoutId);
      if (show) {
        let openModals = getOpenModalCount();
        document.body.appendChild(backdrop);
        if (me.appendToBody) {
          document.body.appendChild(modal);
        }
        modal.style.display = "flex";
        modal.scrollTop = 0;
        backdrop.offsetHeight;  // force repaint
        me.toggleBodyOverflow(false);
        backdrop.classList.add(SHOW);
        modal.classList.add(SHOW);
        if (openModals > 0) {
          const
            modalBaseZ = parseInt(window.getComputedStyle(modal).zIndex) || 1050,       // 1050 is default modal z-Index
            backdropBaseZ = parseInt(window.getComputedStyle(backdrop).zIndex) || 1040, // 1040 is default backdrop z-Index
            offset = openModals * me.zOffset;

          modal.style.zIndex = `${modalBaseZ + offset}`;
          backdrop.style.zIndex = `${backdropBaseZ + offset}`;
        }
        me.timeoutId = setTimeout( () => {
          me.$emit('show');
          me.timeoutId = 0;
        }, me.transitionDuration);
      } else {
        if (backdrop) {
          backdrop.classList.remove(SHOW);
        }
        modal.classList.remove(SHOW);
        me.$emit('hide');
        me.timeoutId = setTimeout( () => {

          modal.style.display = 'none';
          dom.removeElement(backdrop);
          if (me.appendToBody) {
            dom.removeElement(modal);
          }
          if (getOpenModalCount() === 0) {
            me.toggleBodyOverflow(true);
          }
          // me.$emit('hide');
          me.timeoutId = 0;
          // restore z-index for nested modals
          modal.style.zIndex = '';
          if (backdrop) {
            backdrop.style.zIndex = '';
          }
        }, me.transitionDuration);
      }
    },

    backdropClicked () {
      if (this.backdrop) {
        this.toggle(false);
      }
    },

    toggleBodyOverflow (enable) {
      const
        body = document.body,
        dom = this.dom;

      if (enable) {
        body.classList.remove(MODAL_OPEN);
        body.style.paddingRight = null;
      } else {
        const
          withFloatingScrollbar = dom.isIE10() || dom.isIE11(),
          documentHasScrollbar = dom.hasScrollbar(document.documentElement) || dom.hasScrollbar(document.body);

        if (documentHasScrollbar && !withFloatingScrollbar) {
          body.style.paddingRight = `${dom.getScrollbarWidth()}px`;
        }
        body.classList.add(MODAL_OPEN);
      }
    }

  },

  watch: {
    value (v) {
      this.$toggle(v);
    }
  },

  created () {
    const me = this;
    me.activeElement = document.activeElement;

    if (me.type === me.tags.NotificationType.Wait) {
      return;
    }

    // if (me.core.config.closeModalOnBackButton) {
    if (me.core.config.closeModalOnBackButton && me.closeOnBackButton) {
      const unregisterRouterGuard = me.core.router.beforeEach((to, from, next) => {
        switch (to.name) {
          case 'home':
            next();
            break;

          case 'logon':
            me.toggle(false);
            next();
            break;

          default:
            me.toggle(false);
            next(false);
            break;
        }
      });

      me.$once('hook:destroyed', () => {
        unregisterRouterGuard();
      });
    }

    me.core.sessionMgr.setCurrentModal(me);

  },

  mounted () {
    this.dom.removeElement(this.$refs.backdrop);
    this.dom.on(window, 'keydown', this.onKeyDown);

    if (this.value) {
      this.$toggle(true);
    }
  },

  beforeDestroy () {
    const
      me = this,
      dom = me.dom;

    clearTimeout(me.timeoutId);
    dom.removeElement(me.$refs.backdrop);
    dom.removeElement(me.$el);

    if (getOpenModalCount() === 0) {
      me.toggleBodyOverflow(true);
    }
    dom.off(window, 'keydown', this.onKeyDown);

    if (me.activeElement) {
      me.activeElement.focus();
    }

    me.core.sessionMgr.setCurrentModal(null);

  },

}

</script>
