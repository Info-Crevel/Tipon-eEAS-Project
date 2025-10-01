<template>
<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap"><i :class="bulletClass"></i>{{ label }}</label>
    <div :class="selectContainerClass" ref="inpcont">
      <!-- <select
        :id="id"
        ref="inputbox"
        :class="inputClass"
        v-model="selected"
        @input="onChange($event.target.value, value)"
        @keydown="onKeyDown($event, value)"
        @focus="onFocus"
        @blur="onBlur"
      >
        <option
          v-for="option in datasource"
          :value="optionValue(option)"
          :key="optionValue(option)">{{ displayText(option) }}</option>
      </select> -->
      <select
        :id="id"
        ref="inputbox"
        :class="inputClass"
        v-model="selected"
        @change="onChange($event.target.value, value)"
        @keydown="onKeyDown($event, value)"
        @focus="onFocus"
        @blur="onBlur"
      >
        <option
          v-for="option in datasource"
          :value="optionValue(option)"
          :key="optionValue(option)">{{ displayText(option) }}</option>
      </select>
      <button type="button" :class="buttonClass" tabindex=-1><i class="fa fa-fw fa-chevron-down"></i></button>

      
    </div>

  </div>
</div>
</template>

<script>

import
  InputCore
from './InputCore.vue';

export default {
  extends: InputCore,

  model: {
    prop: 'value',
    event: 'changed'
  },

  props: {
    value: undefined,
    displayField: { type: String, required: true },
    datasource: { type: Array, required: true },
    sourceField: { type: String }
  },

  data () {
    return {
      selected: this.value
    };
  },

  computed: {
    selectContainerClass () {
      // return this.inputWidth ? 'px-0 fw-' + this.inputWidth : 'px-0 w-100';
      // 05 Feb 2025 - EMT
      return this.inputWidth ? 'px-0 fw-' + this.inputWidth : 'px-0 flex-fill';
    },

    buttonClass () {
      return this.theme.buttonColor + ' select-button curved-left-0' + (this.align === 'bottom' && !this.isPlain ? ' curved-top-right-0' : '');
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
            cls = 'curved-right';
          }
        }
      }
      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : ' flex-fill');
      return cls + (me.inputAlign === 'left' ? '' : ' text-' + me.inputAlign) + (me.inputBoxClass ? ' ' + me.inputBoxClass : '');
    },

  },

  methods: {
    onKeyDown (e, value) {
      // const EMT
      //   me = this,
      //   targetValue = e.target.value;

      // e.colName = me.fieldId;
      // me.$emit('keydown', e, targetValue, value);
      // if (e.defaultPrevented) { return; }
      // me.handleKeyDown(e, targetValue, value);
      
      //JYL To enable delete button
      const me = this;
      const targetValue = e.target.value;

      e.colName = me.fieldId;
      me.$emit('keydown', e, targetValue, value);
      if (e.defaultPrevented) return;

      const isDeleteKey = e.key === 'Delete' || e.keyCode === 46;
      const isBackspaceKey = e.key === 'Backspace' || e.keyCode === 8;

      if ((isDeleteKey || isBackspaceKey) && !me.isEmpty()) {
        const oldValue = me.selected;
        me.selected = '';
        me.onChange('', oldValue);
        return;
      }

      me.handleKeyDown(e, targetValue, value);

    },

    onChange (newValue, oldValue) {
      const
        me = this,
        item = me.core.getArrayItem(me.datasource, me.fieldId, newValue);

      me.handleChange(me.toInternalValue(newValue), oldValue, item ? item[me.displayField] : '');
    },

    isEmpty () {
      return this.selected ? false : true;
    },

    optionValue (option) {
      const field = this.sourceField || this.fieldId;
      return option[field];
    },

    displayText (option) {
      return option[this.displayField];
    },

    toInternalValue (value) {
      if (this.meta && this.meta.datatype === 'S') {
        return value;
      }
      // if (this.core.isString(value) && value) {
      //   return value;   // 09 Nov 2024 - EMT
      // }
      if (this.core.isString(value) && !value) {
        // return value;
        return 0;     // 19 Apr 2023 - EMT
      }
      if (this.core.isInteger(value)) {
        return value / 1;
      }
      return value;
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
    },

    restoreOldValue (oldValue) {
      const box = this.$refs.inputbox;
      if (box) {
        box.value = oldValue;
        this.selected = box.value;
      }
      this.setInputFocus();
    }

  },

  watch: {
    value (val) {
      this.selected = val;
    }
  }

};

</script>
