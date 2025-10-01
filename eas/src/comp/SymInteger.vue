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
        @blur="onBlur($event)"
        @input="onInput($event)"
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
      if (me.value === 0 && me.isBlankWhenZero) {
        return '';
      }
      if (me.hasFocus) {
        return me.value;
      }
      return me.formatValue(me.value);
    },

    isBlankWhenZero () {
      return this.blankZero || this.isEntityId
    }

  },

  methods: {
    onKeyDown (e, value) {
      const me = this;
      let targetValue = e.target.value;

      e.colName = me.fieldId;

      // if (e.key === 'Enter' && !targetValue && !value && me.isAutoId) {
      if (e.key === 'Enter' && !targetValue && !value && me.isAutoId && !me.isLookupQuery) {
        e.target.value = -1;
        targetValue = -1;
        me.onChange(e.target.value, value)
        return;
      }

      me.$emit('keydown', e, targetValue, value);
      if (e.defaultPrevented) { return; }
      me.handleKeyDown(e, targetValue, value);
      if (e.defaultPrevented) { return; }
      if (me.core.intKeys.includes(e.key)) { return; }
      e.preventDefault();
    },

    onChange (newValue, oldValue) {
      const me = this;
      let newVal = newValue.trim().replace(/,/g,'');
      if (!me.core.isInteger(newVal)) {
        me.advice.fault(newValue, 'Entry is not an integer.');
        me.restoreOldValue(oldValue);
        return;
      }
      newVal = me.toInternalValue(newValue);
      me.handleChange(newVal, oldValue);
      if (me.isEmpty() && oldValue === 0 && me.isBlankWhenZero) {
        me.refresh();
      }
    },

    onInput (e) {
      this.$emit('input', e);
    },

    isEmpty () {
      return this.value ? false : true;
    },

    formatValue (value) {
      return (this.isEntityId || this.noFormat) ? value : this.core.toIntegerFormat(value, this.isBlankWhenZero);
    },

    toInternalValue (value) {
      return value / 1;
    },

    restoreOldValue (oldValue) {
      const box = this.$refs.inputbox;
      if (box) {
        // if (oldValue || (!oldValue && !this.isBlankWhenZero)) {
        //   box.value = oldValue;
        // }
        if (oldValue || (!oldValue && !this.isBlankWhenZero)) {
          box.value = oldValue;
        } else {
          box.value = '';
        }
      }
      this.setInputFocus();
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

  },

  created () {
    if (!('spellcheck' in this.$attrs)) {
      this.$attrs.spellcheck = this.core.config.spellCheck.toString();
    }
  }

};

</script>
