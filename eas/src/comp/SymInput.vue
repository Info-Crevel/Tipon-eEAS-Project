<template>
<div :key="ts">
  <div :class="controlClass" ref="ctrl">
    <label :for="id" :class="labelClass" @click="setInputFocus" ref="cap"><i :class="bulletClass"></i>{{ label }}</label>
    <div :class="inputContainerClass" ref="inpcont" v-bind="$attrs">
      <slot></slot>
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

  computed: {
    inputContainerClass () {
      let
        me = this,
        cls = me.isPlain ? 'curved' : (me.isAlignBottom ? 'curved-bottom' : 'curved-right');

      cls = cls + (me.inputWidth ? ' fw-' + me.inputWidth : '');
      return 'control-set justify-between pr-3 ' + (me.inputBoxClass ? cls + ' ' + me.inputBoxClass : cls);
    }

  }

}

</script>
