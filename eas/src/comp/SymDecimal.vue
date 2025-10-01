<template>

<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap"><i :class="bulletClass"></i>{{ label }}</label>
    <div :class="inputContainerClass" ref="inpcont">
      <input
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
        @focus="onFocus"
        @blur="onBlur"
      >
      <sym-input-button :class="buttonClass" v-if="lookupIcon && lookupIcon !== 'none'" :icon="lookupIcon" @click="buttonClick(lookupId)" ref="lkpbutton"></sym-input-button>
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
    value: { type: Number, default: 0 },
    textAlign: { type: String, default: 'right'},
    blankZero: { type: Boolean, default: true }
  },

  computed: {
    text () {
      const me = this;
      if (me.value === 0 && me.blankZero) {
        return '';
      }
      if (me.hasFocus) {
        return me.value;
      }
      return me.formatValue(me.value);
    }
  },

  methods: {
    onKeyDown (e, value) {
      const
        me = this,
        targetValue = e.target.value;

      e.colName = me.fieldId;
      me.$emit('keydown', e, targetValue, value);
      if (e.defaultPrevented) { return; }
      me.handleKeyDown(e, targetValue, value);
      if (e.defaultPrevented) { return; }
      if (me.core.decKeys.includes(e.key)) { return; }
      e.preventDefault();
    },

    onChange (newValue, oldValue) {
      const me = this;
      let newVal = newValue.trim();
      if (!me.core.isDecimal(newVal)) {
        me.advice.fault(newValue, 'Entry is not a decimal value.');
        me.restoreOldValue(oldValue);
        return;
      }
      newVal = me.toInternalValue(newValue);
      me.handleChange(newVal, oldValue);
      if (me.isEmpty() && oldValue === 0 && me.blankZero) {
        me.refresh();
      }
    },

    isEmpty () {
      return this.value ? false : true;
    },

    formatValue (value) {
      let v = value === null ? 0 : value;
      return this.core.toDecimalFormat(v, this.app.setup.decimalDigits, this.blankZero);
    },

    toInternalValue (value) {
      return value / 1;
    },

    refresh () {
      this.ts +=1;
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

  }

};

</script>
