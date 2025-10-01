<template>

<div :style="style">
  <sym-alert :class="adviceClass" :type="type" :title="title" :duration="duration" :icon="icon" :dismissible="dismissible" @dismissed="onDismissed">
    <div v-if="isHtml" v-html="message"></div>
    <div v-else>{{ message }}</div>
  </sym-alert>
</div>

</template>

<script>

const transitionDuration = 150;

export default {
  props: {
    message: String,
    type: String,
    title: String,
    isHtml: { type: Boolean, default: false },
    duration: { type: Number, default: 2 },
    dismissible: { type: Boolean, default: false },
    alignment: { type: String, default: 'top-right' },
    icon: String,
    transition: { type: String, default: 'fade' },
    callback: { type: Function, required: true },
    queue: { type: Array, required: true },
    offsetY: { type: Number, default: 2 },
    offsetX: { type: Number, default: 2 },
    offset: { type: Number, default: 2 },
    width: { type: Number, default: 0 }
  },

  data () {
    return {
      height: 0,
      top: 0,
      horizontal: this.alignment.includes('-left') ? 'left' : 'right',
      vertical: this.alignment.includes('top-') ? 'top' : 'bottom'
    };
  },

  computed: {
    style () {
      let
        queue = this.queue,
        index = queue.indexOf(this);

      return {
        position: 'fixed',
        [this.vertical]: `${this.getQueueHeight(queue, index)}px`,
        width: this.width > 0 ? `${this.width}px`: 'auto',
        transition: `all ${transitionDuration / 1000}s ease-in-out`,
        zIndex: 2000
      };
    },

    adviceClass () {
      return 'shadow-dark mb-1 ' + this.type;
    }
  },

  methods: {
    getQueueHeight (queue, lastIndex = queue.length) {
      let height = this.offsetY;
      for (let i = 0; i < lastIndex; i++) {
        height += queue[i].height + this.offset;
      }
      return height;
    },

    onDismissed () {
      setTimeout(this.callback, transitionDuration);
    }
  },

  created () {
    this.top = this.getQueueHeight(this.queue);
  },

  mounted () {
    let
      me = this,
      el = me.$el;

    el.style[me.vertical] = me.top + 'px';
    el.style[me.horizontal] = `-${me.width}px`;

    me.$nextTick( () => {
      me.height = el.offsetHeight - me.offset;
      el.style[me.horizontal] = `${me.offsetX}px`;
    });
  }

};

</script>
