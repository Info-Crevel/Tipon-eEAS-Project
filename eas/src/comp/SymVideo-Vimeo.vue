<template>
  <figure class="vimeo-container">
    <div class="vimeo-wrapper" :style="wrapperStyle" ref="wrapper">
      <iframe
        class="vimeo"
        v-bind="videoAttrs"
        :src="videoPath"
        :width="videoWidth"
        :height="videoHeight"
        frameborder="0"
      >
      </iframe>
    </div>
  </figure>
</template>

<script>

const videoUrl = "https://player.vimeo.com/video/";

export default {
  props: {
    videoId: { type: String },
    width: { type: String, default: '80%' },
    videoWidth: { type: String, default: "320" },
    videoHeight: { type: String, default: "180" },
    controls: { type: Boolean, default: true },
    autoPlay: { type: Boolean, default: false },
    loop: { type: Boolean, default: false },
    muted: { type: Boolean, default: false },
    noFullScreen: { type: Boolean, default: false },
    autoSize: { type: Boolean, default: true }
  },

  data () {
    return {
      clientWidth: 0,
      clientHeight: 0
    };
  },

  computed: {
    videoPath () {
      let
        q = 'dnt=1&sidedock=0&title=0&byline=0',
        url = videoUrl + this.videoId;

      if (this.autoPlay) {
        q = q + '&autoplay=1&muted=1';
      } else {
        if (this.muted) {
          q = '&muted=1';
        }
      }
      if (this.loop) {
        q = q + '&loop=1'
      }
      if (!this.controls) {
        q = q + '&controls=0'
      }
      return url + '?' + q;
    },

    videoAttrs () {
      return {
        'allow': this.noFullScreen ? 'autoplay' : 'autoplay; fullscreen',
        'allowfullscreen': !this.noFullScreen,
        'webkitallowfullscreen': !this.noFullScreen,
        'mozallowfullscreen': !this.noFullScreen
      };
    },

    wrapperStyle () {
      const me = this;

      if (!me.autoSize) {
        return false;
      }

      if (me.width.includes('%') && me.clientWidth > 0) {
        let
          percentage = Number(me.width.replace('%', '')) / 100,
          actualWidth = Math.round(percentage * me.clientWidth),
          padding = Math.round(actualWidth * me.aspectRatio).toString();

        return 'width: ' + actualWidth.toString() + 'px; padding-bottom: ' + padding + 'px;';
      }
      return 'width: ' + me.width + ';';
    },

    aspectRatio () {
      return (this.videoHeight / this.videoWidth);
    }

  },

  methods: {
    setClientWidth () {
      this.clientWidth = this.$el.clientWidth;
    },

    adjustSize (maxSize) {
      if (!maxSize) {
        return;
      }

      let
        me = this,
        wrapper = me.$refs.wrapper,
        width = me.clientWidth,
        height = me.clientHeight;

      if (width > maxSize.width || !width) {
        width = maxSize.width;
        height = Math.round(width * me.aspectRatio);
      }

      if (height > maxSize.height || !height) {
        height = maxSize.height;
        width = Math.round(height / me.aspectRatio);
      }

      if (width < maxSize.width && height < maxSize.height) {
        let h = Math.round(maxSize.width * me.aspectRatio);
        if (h < maxSize.height) {
          width = maxSize.width;
          height = h;
        }
      }

      if (height < maxSize.height && width < maxSize.width) {
        let w = Math.round(maxSize.height / me.aspectRatio);
        if (w <= maxSize.width) {
          height = maxSize.height;
          width = w;
        }
      }

      me.clientWidth = width;
      me.clientHeight = height;

      wrapper.style.width = width + 'px';
      wrapper.style['padding-bottom'] = height + 'px';
    }

  },

  mounted () {
    const me = this;
    if (me.autoSize) {
      me.setClientWidth();
      me.dom.on(window, 'resize', me.setClientWidth);
    }
  },

  beforeDestroy () {
    if (this.autoSize) {
      this.dom.off(window, 'resize', this.setClientWidth);
    }
  }

}

</script>

<style scoped>

.vimeo-container {
  display: flex;
}

.vimeo-wrapper {
  position: relative;
  height: 0;
  width: 100%;
}

.vimeo-wrapper .vimeo {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

</style>