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
        v-bind="$attrs"
        :maxlength="maxLen"
        @change="onChange($event.target.value, value)"
        @keydown="onKeyDown($event, value)"
        @focus="onFocus"
        @blur="onBlur"
      >
      <sym-input-button :class="buttonClass" v-if="lookupIcon && lookupIcon !== 'none'" :icon="lookupIcon" @click="onPickerButtonClick" ref="button"></sym-input-button>
    </div>
  </div>
</div>

</template>

<script>

import {
  TimeSpan
} from '../js/core';

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
    value: TimeSpan,
    isoFormat: { type: Boolean, default: false }
  },

  computed: {
    text: {
      get () {
        return this.value && !this.value.isEmpty() ? this.value.toTimeDisplayFormat() : null;
      },
      set (newValue) {
        this.onChange(newValue, this.value);
      }
    },

    placeholder () {
      return this.cue || this.app.setup.timeFormat;
    },

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

    handleKeyDown (e, targetValue, oldValue) {
      const me = this;
      me.isEnterPressed = e.key === 'Enter' && !e.tabOnly;
      if (me.isEnterPressed || (e.key === 'Tab' && !e.shiftKey)) {
        let
          noChange = false,
          internal = me.toInternalValue(targetValue);

        if (oldValue === null) {
          noChange = (oldValue === internal);
        } else {
          if (internal) {
            noChange = internal.equals(oldValue)
          }
        }

        if (noChange) {
          if (me.isEmpty()) {
            if (!me.isValid(internal)) {
              e.preventDefault();
              me.restoreOldValue(oldValue);
              return;
            }
          }
          if (this.isEnterPressed) {
            e.preventDefault();
            this.focusNext();
          }
        }

      }
    },

    onChange (newValue, oldValue) {
      const me = this;
      let
        newVal = newValue.trim(),
        timeInfo = me.core.TimeSpan.parse(newVal, me.isoFormat);

      if (!timeInfo) {
        if (newVal) {
          me.advice.fault(newValue, 'Time not valid.');
          me.restoreOldValue(oldValue);
        } else {
          me.handleChange(null, oldValue);
        }
      } else {
        // if (timeInfo.hour==0 && timeInfo.minute==0) {                
       //    me.handleChange(null, oldValue);
        // } else
        // {


        me.handleChange(new me.core.TimeSpan(timeInfo.hour, timeInfo.minute, 0), oldValue);
        // }  
        // me.handleChange(new me.core.TimeSpan(timeInfo.hour, timeInfo.minute, 0), oldValue);
      }
    },

    isEmpty () {
      return this.value ? this.value.isEmpty() : true;
    },

    formatValue (value) {
      return value.toTimeDisplayFormat();
    },

    toInternalValue (value) {
      // return this.core.toTime(value);
      return this.core.toTime(value, this.isoFormat);   // 16 May 2025 - EMT
    },

    refresh () {
      this.ts +=1;
    },

    restoreOldValue (oldValue) {
      const box = this.$refs.inputbox;
      if (oldValue) {
        box.value = oldValue.toTimeDisplayFormat();
      } else {
        box.value = null;
      }
      this.setInputFocus();
    },

  },

  created () {
    if (!('spellcheck' in this.$attrs)) {
      this.$attrs.spellcheck = this.core.config.spellCheck.toString();
    }
  }

}

</script>
