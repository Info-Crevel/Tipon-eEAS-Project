<template>

<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap"><i :class="bulletClass"></i>{{ label }}</label>
    <div :class="inputContainerClass" ref="inpcont">
      <input
        :id="id"
        :type="type"
        :pattern="inputPattern"
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
    value: { type: String, default: '' },
    type: { type: String, default: 'text' },
    pattern: { type: String, default: '' }
  },

  computed: {
    text () {
      return this.value ? this.value.trim() : '';
    },

    inputPattern () {
      return this.pattern ? this.pattern : false;
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
    },

    onChange (newValue, oldValue) {
      this.handleChange(this.getMaskedValue(newValue), oldValue);
    },

    onInput (e) {
      this.$emit('input', e);
    },

    isEmpty () {
      return this.value ? false : true;
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
