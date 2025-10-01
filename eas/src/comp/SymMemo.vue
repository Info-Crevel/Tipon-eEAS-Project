<template>

<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap">{{ label }}</label>
    <!-- 05 Feb 2025 - EMT (added container for textarea, just like in SymText)     -->
    <div :class="inputContainerClass" ref="inpcont">
      <textarea
        :id="id"
        ref="inputbox"
        :class="inputClass"
        :value="text"
        :placeholder="placeholder"
        :required="required ? true : false"
        :maxlength="maxLen"
        v-bind="$attrs"
        @change="onChange($event.target.value, value)"
        @keydown="onKeyDown($event, value)"
        @focus="onGotFocus"
        @blur="onLostFocus"
      >
      </textarea>
    </div>
  </div>
</div>

</template>

<script>

import
  InputBase
from './InputBase.vue';

export default {
  extends: InputBase,

  model: {
    prop: 'value',
    event: 'changed'
  },

  props: {
    value: { type: String, default: '' },
    inputHeight: { type: Number, default: 2 },
    align: { type: String, default: 'bottom' }
  },

  computed: {
    text () {
      return this.value ? this.value.trim() : '';
    },

    inputClass () {
      const me = this;
      let cls = '';
      if (me.hasCaption) {
        if (me.isPlain) {
          cls = 'curved';
        } else {
          if (me.isAlignBottom) {
            cls = 'curved-top-0';
          } else {
            cls = 'curved-0 curved-right';
          }
        }
      }
      cls = cls + ' tah-' + me.inputHeight;
      // 05 Feb 2025 - EMT (added setting of specified width as in SymText, vis InputBase)
      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : '');
      if (me.mask) {
        if (me.mask === '!') { cls = cls + ' ucase'; }
        if (me.mask === '_') { cls = cls + ' lcase'; }
      }
      return cls;
    }
  },

  methods: {
    onKeyDown (e, value) {
      const
        me = this,
        targetValue = e.target.value;

      e.tabOnly = true;
      e.colName = me.fieldId;
      me.$emit('keydown', e, targetValue, value);
      if (e.defaultPrevented) { return; }
      me.handleKeyDown(e, targetValue, value);
    },

    onChange (newValue, oldValue) {
      this.handleChange(this.getMaskedValue(newValue), oldValue);
    },

    selectInputBoxContent () {
      // set caret position to 0 instead
      this.$refs.inputbox.setSelectionRange(0, 0);
    },

    onGotFocus () {
      this.onFocus();
      this.scrollToTop();
    },

    onLostFocus () {
      this.onBlur();
      this.scrollToTop();
    },

    isEmpty () {
      return this.value ? false : true;
    },

    refresh () {
      this.ts +=1;
    },

    scrollToTop () {
      const memo = this.$refs.inputbox;

      if (memo) {
        memo.scrollLeft = 0;
        memo.scrollTop = 0;
      }
    },

    onCopyValue (value) {
      const
        me = this,
        e = new Event('change');

      let el = me.$refs.inputbox;
      el.value = value;
      el.dispatchEvent(e);
      me.focusNext();
    }

  },

  created () {
    if (!('spellcheck' in this.$attrs)) {
      this.$attrs.spellcheck = this.core.config.spellCheck.toString();
    }
  }

}

</script>
