<template>

<div :key="ts" :colName="fieldId">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap"><i :class="bulletClass"></i>{{ label }}</label>
    <div :class="radioContainerClass" ref="inpcont" v-bind="$attrs">
      <div class="radio"
        v-for="(option, index) in datasource"
        :key="optionValue(option)">
        <input
          type="radio"
          :id="id + '-' + index"
          :name="id"
          v-model="selected"
          :value="optionValue(option)"
          ref="inputbox"
          @change="onChange($event.target.value, value, displayText(option))"
          @keydown="onKeyDown($event, selected)"
          @focus="onFocus"
          @blur="onBlur"
        >
        <!-- <label class="label-radio" :for="id + '-' + index">{{ displayText(option) }}</label> -->
        <label class="label-radio my-auto" :for="id + '-' + index">{{ displayText(option) }}</label>

      </div>
      <slot></slot>
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
    value: { type: [String, Number], default: undefined },
    displayField: { type: String, required: true },
    datasource: { type: Array, required: true }
  },

  data () {
    return {
      selected: this.value
    };
  },

  computed: {
    radioContainerClass () {
      let
        me = this,
        cls = me.isPlain ? 'curved' : (me.isAlignBottom ? 'curved-bottom' : 'curved-right');

      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : '');
      return 'radios justify-between ' + (me.inputBoxClass ? cls + ' ' + me.inputBoxClass : cls);
    }

  },

  methods: {
    onKeyDown (e, value) {
      const me = this;
      e.colName = me.fieldId;
      me.$emit('keydown', e, value, value);
      if (e.defaultPrevented) { return; }
      me.handleKeyDown(e, value, value);
    },

    onChange (newValue, oldValue, displayText) {
      this.handleChange(this.toInternalValue(newValue), oldValue, displayText);
    },

    restoreOldValue (oldValue) {
      this.selected = oldValue;
      this.setInputFocus();
      this.refresh();
    },

    isEmpty () {
      return this.selected ? false : true;
    },

    optionValue (option) {
      return option[this.fieldId];
    },

    displayText (option) {
      return option[this.displayField];
    },

    toInternalValue (value) {
      if (this.meta && this.meta.datatype === 'S') {
        return value;
      }
      if (this.core.isInteger(value)) {
        return value / 1;
      }
      return value;
    },

    refresh () {
      this.ts +=1;
    },

    // onCopyValue (value) {
    //   const
    //     me = this,
    //     e = new Event('change');

    //   let el = me.$refs.inputbox;
    //   el.value = value;
    //   el.dispatchEvent(e);
    //   me.focusNext();
    // }

  },

  watch: {
    value (val) {
      this.selected = val;
    }
  }

}

</script>

<style>

.label-radio:hover {
  cursor: pointer;
}

</style>