<template>

<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" v-if="!inputLeft" :class="labelClass" @click="setInputFocus" ref="cap">{{ label }}</label>
    <div :class="inputContainerClass" ref="inpcont">
      <!-- <input
        :id="id"
        ref="inputbox"
        type="checkbox"
        class="fw-5"
        v-model="checked"
        @input="onChange($event.target.checked, checked)"
        @keydown="onKeyDown($event, checked)"
        @focus="onFocus()"
        @blur="onBlur()"
      > -->
      <input
        :id="id"
        ref="inputbox"
        type="checkbox"
        :class="checkboxClass"
        v-model="checked"
        @input="onChange($event.target.checked, checked)"
        @keydown="onKeyDown($event, checked)"
        @focus="onFocus()"
        @blur="onBlur()"
      >
      <label :for="id" v-if="checkLabel && isAlignBottom" class="pl-2 mb-0">{{ checkLabel }}</label>
    </div>
    <label :for="id" v-if="inputLeft" :class="labelClass" @click="setInputFocus" ref="cap">{{ label }}</label>
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
    value: { type: Boolean, default: false },
    captionWidth: { type: Number, default: 12 },
    inputWidth: { type: Number, default: 10 },
    checkLabel: { type: String, default: undefined },
    checkAlign: { type: String, default: ''},
    inputLeft: { type: Boolean, default: false }
  },

  data () {
    return {
      checked: this.value
    };
  },

  computed: {
    controlClass () {
      const bottom = this.isAlignBottom;

      return {
        'form-control': true,
        'd-block': bottom,
        'd-inline-flex': !bottom,
        // [`fw-${this.captionWidth}`] : bottom && this.captionWidth
        [`fw-${this.captionWidth}`] : bottom && this.captionWidth && !this.isLookupQuery
      }
    },

    labelClass () {
      let
        cls = '',
        me = this;

      if (me.isAlignBottom) {
        cls = ' top-tag w-100'
      } else {
        if (!me.isPlain) {
          cls = cls + (me.inputLeft ? ' right-tag' : ' left-tag');
        }
        cls = cls + (me.captionWidth ? ' fw-' + me.captionWidth : '');
      }
      if (me.captionAlign === 'right' || me.captionAlign === 'center') {
        cls = cls + ' text-' + me.captionAlign;
      }
      return 'input-tag' + (me.isPlain ? ' plain' : '') + cls + (me.captionClass ? ' ' + me.captionClass : '');
    },

    inputContainerClass () {
      let
        me = this,
        cls;

      if (me.isPlain) {
        cls = 'curved'
      } else {
        // cls = me.isAlignBottom ? 'curved-bottom' : (me.inputLeft ? 'curved-left' : 'curved-right') + ' light fw-10';
        cls = me.isAlignBottom ? 'curved-bottom py-2 light' : (me.inputLeft ? 'curved-left' : 'curved-right') + ' light fw-10';
      }
      if (me.checkAlign === 'center' || me.checkAlign === 'end') {
        cls = cls + (me.isAlignBottom ? ' justify-' : ' align-items-') + me.checkAlign;
      } else {
        cls = cls + (me.checkAlign === 'start' ? ' align-items-start': ' align-items-center');
      }
      return 'd-flex box-border mb-0 ' + (me.isPlain ? 'border-0 ' : '') + (me.inputBoxClass ? cls + ' ' + me.inputBoxClass : cls);
    },

    checkboxClass () {
      if (this.isLookupQuery) {
        return false;
      }
      return 'fw-5';
    }
  },

  methods: {
    onKeyDown (e, value) {
      const
        me = this,
        targetValue = e.target.checked;

      e.colName = me.fieldId;
      me.$emit('keydown', e, targetValue, value);
      if (e.defaultPrevented) { return; }
      me.handleKeyDown(e, targetValue, value);
    },

    onChange (newValue, oldValue) {
      this.handleChange(newValue, oldValue);
    },

    restoreOldValue (oldValue) {
      let box = this.$refs.inputbox;
      if (box) {
        box.checked = oldValue;
      }
      this.setInputFocus();
    },

    formatValue () {
      return '';
    },

    isEmpty () {
      return !this.value;
    },

    onCopyValue (value) {
      const
        me = this,
        e = new Event('input');

      let el = me.$refs.inputbox;
      el.checked = value;
      el.dispatchEvent(e);
      me.focusNext();
    }

  },

  // 06 Feb 2025 - EMT
  created () {
    const me = this;
    me.label = !me.bullet ? me.caption : '';
  },

  watch: {
    value (val) {
      this.checked = val;
    }
  }

}

</script>

<style scoped>

.form-control:hover > label {
  cursor: pointer;
}

</style>
