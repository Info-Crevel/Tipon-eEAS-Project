<template>
  <figure class="video-container">
    <div class="video-wrapper" :style="wrapperStyle">
      <video
        v-bind="videoAttrs"
        crossorigin="anonymous"
        :poster="poster"
        disablePictureInPicture
        ref="video"
      >
        <source :src="videoUrl" type="video/mp4" @error="onVideoSourceError($event)">
        <track :label="subtitleLabel" kind="subtitles" srclang="en" :src="subtitleUrl" default v-if="subtitleUrl">
        Your browser does not support HTML5 videos
      </video>

      <img :src="overlayImage" class="overlay play-overlay collapse" ref="play" @click="onPlay()" v-show="overlayColor !== 'none' && !errorFlag">
      <!-- <img src="../img/bgd/video-unavailable.png" class="overlay unavailable-overlay" ref="unavailable" v-show="errorFlag"> -->
      <!-- public folder -->
      <img src="img/bgd/video-unavailable.png" class="overlay unavailable-overlay" ref="unavailable" v-show="errorFlag">
    </div>
  </figure>
</template>

<script>

const SHOW = 'show';
const OVERLAY_COLORS= ['black','beige','blue','green','orange','pink','red','violet'];

export default {
  props: {
    width: { type: String, default: '80%' },
    videoUrl: { type: String, required: true },
    posterUrl: { type: String },
    subtitleUrl: { type: String },
    subtitleLabel: { type: String, default: 'English' },
    controls: { type: Boolean, default: true },
    autoPlay: { type: Boolean, default: false },
    loop: { type: Boolean, default: false },
    muted: { type: Boolean, default: false },
    noFullScreen: { type: Boolean, default: false },
    smartPause: { type: Boolean, default: true },
    overlayColor: { type: String, default: 'black' }
  },

  data () {
    return {
      video: null,
      observer: null,
      overlayClassList: null,
      errorFlag: false
    };
  },

  computed: {
    videoAttrs () {
      return {
        'autoplay': this.autoPlay,
        'controls': this.controls,
        'loop': this.loop,
        'muted': this.autoPlay ? true: this.muted,
        'controlslist': this.noFullScreen ? 'nofullscreen npdownload' : 'nodownload'
      };
    },

    wrapperStyle () {
      return 'width: ' + this.width;
    },

    overlayImage () {
      if (this.overlayColor === 'none'){
        return '';
      }
      let color = 'black';
      if (OVERLAY_COLORS.includes(this.overlayColor)) {
        color = this.overlayColor;
      }
      // return `${require(`../img/bgd/play-overlay-${color}.png`)}`;
      // public folder
      return `img/bgd/play-overlay-${color}.png`;
    },

    poster () {
      return this.errorFlag ? '' : this.posterUrl;
    }

  },

  methods: {
    onPlay() {
      if (this.video) {
        this.video.play();
      }
    },

    showOverlay () {
      if (this.overlayClassList) {
        this.overlayClassList.add(SHOW);
      }
    },

    hideOverlay () {
      if (this.overlayClassList) {
        this.overlayClassList.remove(SHOW);
      }
    },

    onVideoSourceError () {
      this.errorFlag = true;
    }

  },

  mounted () {
    const me = this;

    me.video = me.$refs.video;
    me.overlayClassList = me.$refs.play.classList;

    if (me.video) {
      if (me.smartPause) {
        me.observer = new IntersectionObserver(entries => {
          entries.forEach(entry => {
            if (entry.intersectionRatio !== 1 && !me.video.paused) {
              me.video.pause();
            }
          });
        }, {threshold: 1});
        me.observer.observe(me.video);
      }

      me.video.onloadedmetadata = () => {
        me.$el.style.display = "flex";
      }

      if (me.overlayClassList && me.overlayColor !== 'none') {
        me.showOverlay();

        me.video.onpause = () => {
          me.$nextTick( () => {
            me.showOverlay();
          });
        }

        me.video.onplay = () => {
          me.hideOverlay();
          me.video.focus();
        }

        me.video.onseeking = () => {
          me.hideOverlay();
        }

        me.video.onended = () => {
          me.showOverlay();
        }
      }
    }
  }

}

</script>

<style scoped>

.video-container {
  display: flex;
}

.video-wrapper {
  position: relative;
  display: inline-flex;
}

.video-wrapper video {
  width: 100%;
}

.overlay {
  position: absolute;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.play-overlay {
  z-index: 1;
  opacity: 0.5;
}

.play-overlay:hover {
  cursor: pointer;
  opacity: 0.6;
}

.unavailable-overlay {
  z-index: 2;
}

</style>