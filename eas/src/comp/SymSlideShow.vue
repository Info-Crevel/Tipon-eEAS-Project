<template>

<section class="slide-container" ref="slider">
  <img class="slide-placeholder w-100 d-block m-0" :src="getImagePath(images[0])">

  <a :href="getLinkPath(src)" v-for="(src, index) in images" :key="index" target="_blank" @mouseenter.prevent="onMouseEnter" @mouseleave.prevent="onMouseLeave">
    <img class="slide d-block m-0" :src="getImagePath(src)" :class="imageClass" @load="imageLoaded" @click="onClickImage">
  </a>

  <p class="mb-0 image-caption" :class="captionClass" ref="caption" v-if="caption"> {{ caption }}</p>

  <div v-if="navigation">
    <label class="slide-prev text-shadow" @mousedown.prevent @click.prevent="moveSlide(-1)" @blur="playSlideShow()">&#10094;</label>
    <label class="slide-next text-shadow" @mousedown.prevent @click.prevent="moveSlide(1)" @blur="playSlideShow()">&#10095;</label>
  </div>

</section>

</template>

<script>

export default {
  props: {
    images: { type: Array, required: true },
    display: { type: String, default: 'inline-block' },
    interval: { type: Number, default: 5 },
    captionClass: { type: String, default: '' },
    imageClass: { type: String, default: '' },
    showCaption: { type: Boolean, default: true },
    smartPause: { type: Boolean, default: true },
    hoverPause: { type: Boolean, default: true },
    navigation: { type: Boolean, default: true },
    imageClick: { type: Boolean, default: false }
  },

  data () {
    return {
      caption: '',
      slider: null,
      slides: [],
      slideCount: 0,
      // imagesLoaded: 0,
      duration: 5,
      currentIndex: 0,
      nextSlide: null,
      prevSlide: null,
      intervalId: 0,
      timeoutId: 0,
      observer: null,
      isStopped: false,
      isMounted: false,
      firstImagePath: ''
    };
  },

  methods: {
    getImagePath (src) {
      if (typeof src === 'object') {
        // return `${require(`../img/${src.path}`)}`;
        return `img/${src.imagePath}`;   // public folder
      } else {
        // return `${require(`../img/${src}`)}`;
        return `img/${src}`;        // public folder
      }
    },

    getLinkPath (src) {
      return src.linkPath ? src.linkPath : false;
    },

    setNextSlide () {
      const me = this;

      me.nextSlide = me.slides[me.currentIndex];
      if (me.nextSlide) {
        me.nextSlide.classList.add('next');
      }

      if (me.showCaption && typeof me.images[me.currentIndex] === 'object' && 'caption' in me.images[me.currentIndex])  {
        me.caption = me.images[me.currentIndex].caption;
      } else {
        me.caption = '';
      }

    },

    imageLoaded (e) {
      if (!e.path) { return; }
      const me = this;
      let o = e.path[0];
      if (o && 'src' in o) {
        let src = o.src;
        if (src.endsWith(me.firstImagePath)) {
          me.slides = me.slider.querySelectorAll('.slide');
          me.setNextSlide();
          me.playSlideShow();
        }
      }

    },

    playSlideShow () {
      const me = this;

      me.isStopped = false;
      me.intervalId = setInterval(() => {
        // me.showSlide()
        me.showSlide(1);
      }, (me.duration * 1000));
    },

    stopSlideShow () {
      const me = this;

      if (me.intervalId) {
        clearInterval(me.intervalId);
      }

      if (me.timeoutId) {
        clearTimeout(me.timeoutId);
      }
      me.intervalId = 0;
      me.isStopped = true;
    },

    showSlide (step) {
      const me = this;

      if (step === 0) {
        return;
      }

      if (me.prevSlide) {
        me.prevSlide.classList.remove('prev', 'fade-out');
      }

      if (me.nextSlide) {
        me.nextSlide.classList.remove('next');
      }
      me.prevSlide = me.nextSlide;

      if (me.prevSlide) {
        me.prevSlide.classList.add('prev');
      }

      if (step > 0) {
        me.currentIndex++;
        if (me.currentIndex >= me.slideCount) {
          me.currentIndex = 0;
        }
      } else {
        if (me.currentIndex === 0) {
          me.currentIndex = me.slideCount - 1;
        } else {
          me.currentIndex--;
        }
      }

      me.setNextSlide();
      me.prevSlide.classList.add('fade-out');
    },

    moveSlide (step) {
      const me = this;

      if (me.slideCount === 1) {
        return;
      }

      if (step === 0) {
        return;
      }

      me.stopSlideShow();

      if (me.timeoutId) {
        clearTimeout(me.timeoutId);
      }

      me.showSlide(step);

      me.timeoutId = setTimeout(() => {
        if (me.isStopped) {
          me.playSlideShow();
        }
      }, me.duration * 1000);
    },

    onMouseEnter () {
      if (this.hoverPause) {
        this.stopSlideShow();
      }
    },

    onMouseLeave () {
      if (this.hoverPause) {
        this.playSlideShow();
      }
    },

    onClickImage (e) {
      if (!this.imageClick) { return; }
      this.$emit('clickimage', e);
    }

  },

  mounted () {
    const me = this;

    me.duration = me.interval < 2 ? 2 : me.interval;
    me.slider = me.$refs.slider;
    me.slides = me.slider.querySelectorAll('.slide');
    me.slideCount = me.images.length;
    if (me.slideCount) {
      me.firstImagePath = me.getImagePath(me.images[0]);
    }

    if (me.smartPause) {
      me.observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
          if (entry.intersectionRatio !== 1 && !me.isStopped) {
            me.stopSlideShow();
          } else if (me.isStopped) {
            me.playSlideShow();
          }
        });
      }, {threshold: 1});

      me.observer.observe(me.slider);
    }

  },

  watch: {
    images (val) {
      this.firstImagePath = this.getImagePath(val[0]);
      this.slideCount = val.length;
    }
  }

}

</script>

<style scoped>

.image-caption {
  z-index: 10;
}

.slide-placeholder {
  opacity: 0;
  object-fit: cover;
}

.slide-container {
  position: relative;
}

.slide-container .slide {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  opacity: 0;
  z-index: -1;
}

.slide-container .slide.next {
  opacity: 1;
  z-index: 1;
}

.slide-container .slide.prev {
  opacity: 1;
  z-index: 2;
}

.slide-container .slide.fade-out {
  opacity: 0;
  transition: visibility 0s .6s, opacity .6s linear;
  visibility: hidden;
}

.slide-prev,
.slide-next {
  cursor: pointer;
  position: absolute;
  top: 50%;
  width: auto;
  margin-top: -2rem;
  padding: .5rem .75rem;
  color: white;
  font-weight: bold;
  font-size: 1.5rem;
  line-height: 1;
  transition: 0.6s ease;
  border-radius: 0 3px 3px 0;
  user-select: none;
  z-index: 20;
  background-color: rgba(96, 96, 96, .2);
}

.slide-next {
  right: 0;
  border-radius: 3px 0 0 3px;
}

.slide-prev:focus {
  background-color: rgba(255, 0, 0, .4);
}

.slide-contain-fit {
  height: 100%;
  object-fit: contain;
  object-position: center;
}

@media (hover: hover) {
  .slide-prev:hover,
  .slide-next:hover {
    background-color: rgba(48, 48, 48, .7);
  }
}

@media (max-width: 480px) {
  .slide-prev,
  .slide-next {
    margin-top: -1.25rem;
    padding: .5rem;
    font-size: 1.25rem;
  }
}

</style>