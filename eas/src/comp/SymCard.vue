<template>

<section class="card">
  <div :class="headerClassList" v-if="hasHeader">
    <slot name="header">
    </slot>
  </div>

  <slot name="subheader"></slot>
  <slot name="img-top" v-if="hasTopImage"></slot>

  <div :id="bodyId" :class="bodyClassList" v-if="hasBody">
    <div class="card-title" v-if="hasTitle">
      <slot name="title"></slot>
    </div>
    <div class="card-subtitle" v-if="hasSubTitle">
      <slot name="subtitle"></slot>
    </div>
    <slot>
    </slot>
  </div>

  <slot name="img-bottom" v-if="hasBottomImage"></slot>
  <slot name="subfooter"></slot>

  <div :class="footerClassList" v-if="hasFooter">
    <slot name="footer">
    </slot>
  </div>

</section>

</template>

<script>

export default {
  props: {
    headerClass: String,
    bodyClass: String,
    footerClass: String,
    bodyId: String
  },

  computed: {
    hasHeader () { return 'header' in this.$slots; },
    hasTitle () { return 'title' in this.$slots; },
    hasSubTitle () { return 'subtitle' in this.$slots; },
    hasBody () { return 'default' in this.$slots; },
    hasFooter () { return 'footer' in this.$slots; },
    hasTopImage () { return 'img-top' in this.$slots; },
    hasBottomImage () { return 'img-bottom' in this.$slots; },

    headerClassList () { return 'card-header' + (this.headerClass ? ' ' + this.headerClass : '') },
    bodyClassList () { return 'card-body' + (this.bodyClass ? ' ' + this.bodyClass : '') },
    footerClassList () { return 'card-footer' + (this.footerClass ? ' ' + this.footerClass : '') }
  },

  mounted () {
    const me = this;
    if (me.hasTopImage) {
      let el = me.$slots["img-top"][0].elm;
      if (me.hasHeader) {
        el.classList.add('card-img', 'curved-0');
      } else {
        el.classList.add('card-img-top');
      }
    }
    if (me.hasBottomImage) {
      let el = me.$slots["img-bottom"][0].elm;
      el.classList.add('card-img-bottom');
      if (me.hasFooter) {
        el.classList.add('curved-0');
      }
    }
  }

}
</script>
