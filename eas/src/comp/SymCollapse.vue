<script>

const COLLAPSE = 'collapse';
const COLLAPSING = 'collapsing';
const SHOW = 'show';

export default {
  render (h) {
    return h(this.tag, {}, this.$slots.default)
  },

  props: {
    tag: { type: String, default: 'div' },
    value: { type: Boolean, default: false },
    transitionDuration: { type: Number, default: 150 },
    autoScroll: { type: Boolean, default: true }
  },

  data () {
    return {
      timeoutId: 0
    };
  },

  watch: {
    value (show) {
      this.refresh(show);
    }
  },

  mounted () {
    this.$el.classList.add(COLLAPSE);
    this.refresh(this.value);
  },

  methods: {
    refresh (show) {
      const
        me = this,
        el = me.$el,
        cls = el.classList;

      clearTimeout(me.timeoutId);

      if (show) {
        me.$emit('show');
        cls.remove(COLLAPSE);
        el.style.height = 'auto';
        let height = window.getComputedStyle(el).height;
        el.style.height = null;
        cls.add(COLLAPSING);
        el.offsetHeight; // force repaint
        el.style.height = height;
        me.timeoutId = setTimeout( () => {
          cls.remove(COLLAPSING);
          cls.add(COLLAPSE, SHOW);
          el.style.height = null;
          me.timeoutId = 0;
          me.$emit('shown');
          if (me.autoScroll) {
            me.dom.scrollIntoView(el);
          }
        }, me.transitionDuration);
      } else {
        me.$emit('hide');
        el.style.height = window.getComputedStyle(el).height;
        cls.remove(COLLAPSE, SHOW);
        el.offsetHeight;
        el.style.height = null;
        cls.add(COLLAPSING);
        me.timeoutId = setTimeout( () => {
          cls.add(COLLAPSE);
          cls.remove(COLLAPSING);
          el.style.height = null;
          me.timeoutId = 0;
          me.$emit('hidden');
        }, me.transitionDuration)
      }
    }

  }
}

</script>
