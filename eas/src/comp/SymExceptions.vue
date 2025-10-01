<template>

<div :class="exceptionClass" v-if="exceptions.length">
  <p fault><i class="fa fa-lg fa-exclamation-circle mr-2"></i><b>{{ message }}</b></p>
  <ul>
    <li class="ml-4" v-for="ex in exceptions" :key="ex.colName">
      <b>{{ ex.caption }}</b> : {{ ex.message }}
    </li>
  </ul>
</div>

</template>

<script>

export default {
  props: {
    exceptions: { type: Array, required: true },
    message: { type: String, default: 'Form exception(s):' },
    scheme: { type: String, default: 'danger-light' }
  },

  computed: {
    exceptionClass () {
      return 'box-border p-3 mb-3 ' + (this.scheme ? this.scheme : 'danger-light');
    }
  }
}

</script>
