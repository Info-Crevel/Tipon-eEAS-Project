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
      <sym-input-button :class="buttonClass" v-if="lookupIcon && lookupIcon !== 'none'" :icon="lookupIcon" @click="onPickerButtonClick" ref="lkpbutton"></sym-input-button>
    </div>
  </div>

  <sym-date-picker
    id="picker"
    ref="picker"
    v-show="isPickerVisible"
    v-model="text"
    class="shadow-light mt-0"
    initialView="d"
    :dateControl = this
    :minDate="minDate"
    :maxDate="maxDate"
    :today-button="todayButton"
    :clear-button="clearButton"
    :close-on-select="closeOnSelect"
    @input="onInput"
  >
  </sym-date-picker>

</div>

</template>

<script>

import {
  DateTime
} from '../js/core';

import
  InputBase
from './InputBase.vue';

const DatePicker = () => import(/* webpackChunkName: "controls" */ './SymDatePicker.vue');

export default {
  extends: InputBase,
  components: {
    'sym-date-picker': DatePicker
  },

  model: {
    prop: 'value',
    event: 'changed'
  },

  props: {
    value: DateTime,
    closeOnSelect: { type: Boolean, default: true },
    todayButton: { type: Boolean, default: true },
    clearButton: { type: Boolean, default: true }
  },

  data () {
    return {
      isPickerVisible: false,
      minDate: null,
      maxDate: null
    };
  },

  computed: {
    text: {
      get () {
        return this.value && !this.value.isEmpty() ? this.value.toDateFormat() : null;
      },
      set (newValue) {
        this.onChange(newValue, this.value);
      }
    },

    placeholder () {
      return this.cue || this.app.setup.dateTemplate || this.app.setup.dateFormat.toLowerCase();
    },

    maxLen () { return 10; },
    minValMessage () { return 'Earliest date allowed is '; },
    maxValMessage () { return 'Date cannot be later than '; },

    inputClass () {
      const me = this;
      let cls = '';
      if (me.hasCaption) {
        if (me.isPlain) {
          cls = 'curved-left';
        } else {
          cls = me.isAlignBottom ? 'curved-top-0 curved-right-0' : 'curved-0';
        }
        cls = cls + ' border-right-0'
      } else {
        cls = cls + 'curved-right-0 border-right-0'
      }
      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : ' flex-fill');
      if (me.mask) {
        if (me.mask === '!') { cls = cls + ' ucase'; }
        if (me.mask === '_') { cls = cls + ' lcase'; }
      }
      return cls + (me.inputAlign === 'left' ? '' : ' text-' + me.inputAlign) + (me.inputBoxClass ? ' ' + me.inputBoxClass : '');
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
      if (e.key === 'F4') {
        me.isPickerVisible = true;
        me.setInputFocus();
        return;
      }
      me.handleKeyDown(e, targetValue, value);
      if (e.defaultPrevented) { return; }

      if (e.key === 'Escape') {
        me.isPickerVisible = false;
        return;
      }
      if (me.isPickerVisible) {
        if (me.core.dateKeys.includes(e.key)) { return; }
      }
      if (me.core.dateKeys.includes(e.key)) {
        me.isPickerVisible = false;
        return;
      }
      e.preventDefault();
    },

    handleKeyDown (e, targetValue, oldValue) {
      const me = this;
      me.isEnterPressed = e.key === 'Enter' && !e.tabOnly;
      if (me.isEnterPressed || (e.key === 'Tab' && !e.shiftKey)) {
        if (targetValue && !me.core.isDate(targetValue.trim())) {
          me.advice.fault(targetValue, 'Date not valid.');
          e.preventDefault();
          me.restoreOldValue(oldValue);
          return;
        }
        let
          noChange = false,
          internal = me.toInternalValue(targetValue);

        if (oldValue === null) {
          noChange = internal.isEmpty();
          // internal = null;
        } else {
          noChange = internal.equals(oldValue);
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
        } else {
          if (internal.isEmpty()) {
            if (!me.isValid(internal)) {
              e.preventDefault();
              me.restoreOldValue(oldValue);
              return;
            }
          }
        }
      }

      if (e.key === '+' && !e.ctrlKey) {
        // add 1 day
        e.preventDefault();
        if (this.value) {
          const newValue = this.value.addDays(1).toDateFormat();
          this.setInputValue(newValue);
          this.isPickerVisible = false;
        }
      }

      if (e.key === '-' && !e.ctrlKey) {
        // subtract 1 day
        e.preventDefault();
        if (this.value) {
          const newValue = this.value.addDays(-1).toDateFormat();
          this.setInputValue(newValue);
          this.isPickerVisible = false;
        }
      }
    },

    onPickerButtonClick () {
      const me = this;
      me.$refs.picker.$data.view = 'd';
      me.isPickerVisible = !me.isPickerVisible;
    },

    onChange (newValue, oldValue) {
      const me = this;
      let newVal = newValue.trim();
      if (me.core.isDate(newVal) || !newVal) {
        me.handleChange(me.core.toDate(newVal), oldValue);
      } else {
        if (newValue) {
          me.advice.fault(newValue, 'Date not valid.');
        }
        me.restoreOldValue(oldValue);
      }
    },

    isEmpty () {
      return this.value ? this.value.isEmpty() : true;
    },

    formatValue (value) {
      return value.toDateFormat();
    },

    toInternalValue (value) {
      return this.core.toDate(value);
    },

    refresh () {
      this.ts +=1;
    },

    restoreOldValue (oldValue) {
      const box = this.$refs.inputbox;
      if (oldValue) {
        box.value = oldValue.toDateFormat();
      } else {
        box.value = null;
      }
      this.setInputFocus();
    },

    onDocumentClick (e) {
      let target = e.target;
      if (this.$refs.lkpbutton && this.$refs.lkpbutton.$el.contains(target)) {
        return;
      }
      let el = this.$refs.picker.$el;
      if (el !== target && !el.contains(target)) {
        this.isPickerVisible = false;
      }
    },

    onInput () {
      if (this.closeOnSelect) {
        this.isPickerVisible = false;
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

  watch: {
    isPickerVisible (flag) {
      if (flag) {
        this.minDate = this.getMinValue();
        this.maxDate = this.getMaxValue();
      }
    }
  },

  created () {
    if (!this.icon) {
      this.lookupIcon = 'search';
    }
    this.dom.on(document, 'click', this.onDocumentClick);
  },

  beforeDestroy () {
    this.dom.off(document, 'click', this.onDocumentClick);
  }

};

</script>

<style scoped>

#picker {
  margin-top: .25rem;
  position: absolute;
  z-index: 9;
}

</style>
