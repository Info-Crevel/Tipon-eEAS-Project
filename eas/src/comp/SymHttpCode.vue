<template>

<div class="container">
  <div class="board dark text-center border-main p-1 shadow">

    <div class="d-flex justify-center mb-2 mt-1">
      <span class='display-9 ml-2'>{{ app.setup.appName }} - {{ sym.sysInfo.siteShortName }}</span>
    </div>

    <div class="box-border light curved-top-0 pb-4 mb-0">
      <span class='display-4 d-block'>{{ this.$route.params.httpCode }}</span>
      <span class="display-8 d-block">{{ statusText }}</span>
      <p class="display-10 pt-1">{{ this.failedPath }}</p>
      <!-- <sym-link to="/" class='button warning justify-center fw-26 mt-2 shadow-light' @click='goHome'><i class='fa fa-lg fa-home mr-2'></i>Home</sym-link> -->
      <sym-link to="/" class='button warning justify-center fw-26 mt-2 shadow-light'><i class='fa fa-lg fa-home mr-2'></i>Home</sym-link>
    </div>
  </div>
</div>

</template>

<script>

export default {
  computed: {
    statusText () {
      return this.$route.params.httpCode === 404 ? 'Resource not found' : this.$route.query.statusText;
    },

    failedPath () {
      let
        path = this.$route.query.pathName,
        from = this.$route.redirectedFrom;

      if (from && !path.endsWith(from)) {
        path = path + from.substring(1);
      }
      return path;
    }
  },

  methods: {
    // goHome () {
    //   window.location = "/";
    // },

    goBack () {
      this.$router.go(-1);
    }
  },

  mounted () {
    this.dom.hideMenu();
  },

  beforeDestroy () {
    this.dom.showMenu();
  }

};

</script>
