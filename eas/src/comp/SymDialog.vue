<template>

<sym-modal
  ref="modal"
  v-model="show"
  :scheme="scheme"
  :size="size"
  :title="title"
  :header="!!title"
  :backdrop="closeOnBackdropClick"
  :type="type"
  :dismissible="false"
  :transitionDuration="transitionDuration"
  :footer="footer"
  :keyboard="keyboard"
  @hide="onHide">

  <div class="media mt-content" v-if="type !== 3">
    <span v-if="icon" class="mt-icon pt-0"><i :class="iconClass"></i></span>
    <div class="media-body mt-icon">
      <div v-if="isHtml" class="pt-1 lg-2 mb-2" v-html="message"></div>
      <div v-else class="pt-1 lg-2 mb-2">{{ message }}</div>
    </div>
  </div>

  <div class="d-flex justify-center py-2" v-if="type === 3">
    <span class="px-4" v-if="icon"><i :class="iconClass"></i></span>
  </div>

  <div class="text-right" slot="footer">
    <div class="buttons" :info-light="isMono" :outline="isMono" border-main fw-24 mb-0 shadow-soft>
      <button :class="okButtonClass" @click="toggle(false, type === 2 ? 'yes' : 'ok')" ref="okbtn">
        <span>{{ okText || "OK" }}</span>
      </button>
      <button :class="rejectButtonClass" @click="toggle(false, 'no')" v-if="type === 2" ref="rejectbtn">
        <span>{{ rejectText || "No" }}</span>
      </button>
      <button :class="cancelButtonClass" @click="toggle(false, 'cancel')" v-if="(type === 2) && cancelButton" ref="cancelbtn">
        <span>{{ cancelText || "Cancel" }}</span>
      </button>
    </div>
  </div>

</sym-modal>

</template>

<script>

export default {
  props: {
    message: String,
    scheme: String,
    title: { type: String, default: 'Symphony' },
    isHtml: { type: Boolean, default: false },
    icon: String,
    backdrop: null,
    okCaption: String,
    rejectCaption: String,
    cancelCaption: String,
    cancelButton: { type: Boolean, default: false },
    buttonScheme: { type: String, default: 'color' },   // color, mono
    defaultReply: { type: String, default: 'no' },
    type: { type: Number, default: 1 },
    size: { type: String, default: '' },
    footer: { type: Boolean, default: true },
    keyboard: { type: Boolean, default: true },
    transitionDuration: { type: Number, default: 200 },
    callback: { type: Function, required: true }
  },

  data () {
    return {
      show: false,
      reply: ''
    };
  },

  computed: {
    closeOnBackdropClick () {
      return this.core.isDefined(this.backdrop) ? Boolean(this.backdrop) : true;
    },

    iconClass () {
      let cls = 'fa fa-' + this.icon + ' text-' + `${this.scheme}`;
      if (this.icon !== 'none' && this.type !== 3) {
        cls = cls + ' mr-3';
      }
      return cls + ' ' + (this.type === 3 ? 'fa-4x fa-spin' : 'fa-3x');
    },

    okText () {
      return this.okCaption || (this.type === 1 ? 'OK' : 'Yes');
    },

    rejectText () {
      return this.rejectCaption || 'No';
    },

    cancelText () {
      return this.cancelCaption || 'Cancel';
    },

    okButtonClass () {
      return this.isMono ? false : 'text-center info-light';
    },

    rejectButtonClass () {
      return this.isMono ? false : 'text-center primary-light';
    },

    cancelButtonClass () {
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
    }

  },

  mounted () {
    const me = this;

    me.reply = 'ok';
    if (me.type === me.tags.NotificationType.Confirmation) {
      me.reply = me.cancelButton ? 'cancel' : 'no';
    }

    setTimeout(() => {
      let btn;
      switch (me.defaultReply) {
        case 'no':
          btn = me.$refs.rejectbtn;
          if (!btn) {
            btn = me.$refs.okbtn;
          }
          break;

        case 'cancel':
          btn = me.$refs.cancelbtn;
          break;

        default:
          btn = me.$refs.okbtn;
          break;
      }
      if (btn) { btn.focus(); }
    }, 110);

  }

}

</script>

<style scoped>

.mt-content {
  margin-top: -.5rem;
}

</style>
