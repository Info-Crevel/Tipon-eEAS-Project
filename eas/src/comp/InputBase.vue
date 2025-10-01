<script>

import
  InputCore
from './InputCore.vue';

export default {
  extends: InputCore,

  props: {
    cue: { type: String, default: null }
  },

  data () {
    return {
      lookupIcon: undefined
    };
  },

  computed: {
    hasIcon () {
      return this.lookupIcon && this.lookupIcon !== 'none' ? true : false;
    },

    inputContainerClass () {
      return 'd-flex px-0 ' + (this.isAlignBottom ? 'w-100' : 'flex-fill');
    },

    inputClass () {
      const me = this;
      let cls = '';
      if (me.hasCaption) {
        if (me.isPlain) {
          if (me.lookupId) {
            cls = "curved-left border-right-0"
          } else {
            cls = 'curved';
          }
        } else {
          if (me.isAlignBottom) {
            cls = me.hasIcon ? 'curved-top-0 curved-bottom-right-0 border-right-0' : 'curved-top-0';
          } else {
            cls = me.hasIcon ? 'curved-0 border-right-0' : 'curved-right';
          }
        }
      } else {
        cls = me.hasIcon ? 'curved-right-0 border-right-0' : 'curved';
      }
      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : ' flex-fill');
      if (me.mask) {
        if (me.mask === '!') { cls = cls + ' ucase'; }
        if (me.mask === '_') { cls = cls + ' lcase'; }
      }
      return cls + (me.inputAlign === 'left' ? '' : ' text-' + me.inputAlign) + (me.inputBoxClass ? ' ' + me.inputBoxClass : '');
    },

    buttonClass () {
      if (this.isPlain) { return this.theme.buttonColor; }
      return this.theme.buttonColor + (this.align === 'bottom' ? ' curved-top-right-0' : '');
    },

    placeholder () { return this.cue; }
  },

  created () {
    const me = this;
    me.lookupIcon = me.icon;
    if (me.lookupId && !me.lookupIcon) {
      me.lookupIcon = 'search';
    }
  }

};

</script>
