<template>
  <figure class="youtube-container">
    <div class="youtube-wrapper" :style="wrapperStyle" ref="wrapper">
      <iframe
        :id="playerId"
        class="youtube"
        v-bind="videoAttrs"
        :src="videoPath"
        :width="videoWidth"
        :height="videoHeight"
        frameborder="0"
        allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
      >
      </iframe>
    </div>
  </figure>
</template>

<script>

// const videoUrl = "http://www.youtube.com/embed/";
const videoUrl = "https://www.youtube.com/embed/";

export default {
  props: {
    playerId: '',
    videoId: { type: String },
    width: { type: String, default: '80%' },
    videoWidth: { type: String, default: "320" },
    videoHeight: { type: String, default: "180" },
    controls: { type: Boolean, default: true },
    autoPlay: { type: Boolean, default: false },
    loop: { type: Boolean, default: false },
    muted: { type: Boolean, default: false },
    noFullScreen: { type: Boolean, default: false },
    enableApi: { type: Boolean, default: false },
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
        q = 'origin=' + window.location.origin + '&modestbranding=1&rel=-1',
        url = videoUrl + this.videoId;

      if (this.noFullScreen) {
        q = q + '&fs=0';
      }

      if (this.autoPlay) {
        if (q) {
          q = q + '&';
        }
        q = q + 'autoplay=1&mute=1';
      } else {
        if (this.muted) {
          if (q) {
            q = q + '&';
          }
          q = 'mute=1';
        }
      }
      if (!this.controls) {
        if (q) {
          q = q + '&';
        }
        q = q + 'controls=0';
      }
      if (this.loop) {
        if (q) {
          q = q + '&';
        }
        q = q + 'loop=1'
      }
      if (this.enableApi) {
        if (q) {
          q = q + '&';
        }
        q = q + 'enablejsapi=1'
      }
      if (q) {
        url = url + '?' + q;
      }
      return url;
    },

    videoAttrs () {
      return {
        // 'allow': 'fullscreen',
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

.youtube-container {
  display: flex;
}

.youtube-wrapper {
  position: relative;
  height: 0;
  width: 100%;
}

.youtube-wrapper .youtube {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

</style>