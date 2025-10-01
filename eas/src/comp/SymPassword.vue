<!-- 01 Mar 2025 - EMT -->
<template>

<sym-modal
  ref="modal"
  v-model="show"
  :scheme="scheme"
  :size="size"
  :title="title"
  :header="!!title"
  :backdrop="false"
  :dismissible="false"
  :transitionDuration="transitionDuration"
  :footer="true"
  :keyboard="false"
  bodyClass="whitesmoke"
  @hide="onHide">

  <div class="box-border mb-2">
    <div class="p-3 lg-2 text-center shadow-dark charcoal" v-html="message"></div>
  </div>

  <sym-text v-model="password" class="mb-1X" caption="Password" :captionWidth="28" type="password" @changed="onPasswordChanged" ref="pwd"></sym-text>

  <div class="text-right" slot="footer">
    <div class="buttons" :info-light="isMono" :outline="isMono" border-main fw-26 mb-0 shadow-soft>
      <button type="submit" :class="submitButtonClass" @click.prevent="onSubmit"><i class="fa fa-play mr-2"></i>Submit</button>
      <button type="button" :class="closeButtonClass" @click="onClose">Close</button>
    </div>
  </div>

</sym-modal>

</template>

<script>

export default {
  props: {
    message: { type: String, required: true },
    scheme: String,
    title: { type: String, default: 'Symphony' },
    buttonScheme: { type: String, default: 'color' },   // color, mono
    size: { type: String, default: '' },
    transitionDuration: { type: Number, default: 200 },
    callback: { type: Function, required: true },
    correctPassword: { type: String, required: true }
  },

  data () {
    return {
      show: false,
      reply: '',
      password: '',
      passControl: null,
      isSubmitting: false
    };
  },

  computed: {
    submitButtonClass () {
      return this.isMono ? false : 'text-center info-light';
    },

    closeButtonClass () {
      return this.isMono ? false : 'text-center info-light';
    },

    isMono () {
      return (this.buttonScheme === 'mono');
    }

  },

  methods: {
    toggle (show, reply) {
      this.reply = reply;
      this.$refs.modal.toggle(show);
    },

    onHide () {
      this.callback(this.reply);
    },

    onSubmit () {
      const me = this;

      if (me.isSubmitting) { return; }
      me.isSubmitting = true;

      if (!me.password.trim()) {
        me.advice.fault("Password", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus();
        me.isSubmitting = false;
        return;
      }

      if (me.password !== me.correctPassword) {
        me.advice.fault("Entry rejected.");
        me.setFocus();
        me.isSubmitting = false;
        return;
      }

      me.isSubmitting = false;
      this.toggle(false, 'yes');
    },

    onClose () {
      this.toggle(false, 'no');
    },

    onPasswordChanged () {
      const me = this;

      if (me.password) {
        me.onSubmit();
      }
    },

    setFocus () {
      const me = this;
      if (!me.passControl) { return; }
      setTimeout( () => {
        me.passControl.focus();
      }, 50);
    },

  },

  mounted () {
    const me = this;
    me.reply = 'no';

    me.$nextTick( () => {
      setTimeout( () => {
        me.passControl = me.$refs.pwd.$refs.inputbox;
        me.setFocus();
      }, 100);
    });

  }

}

</script>

<style>

</style>