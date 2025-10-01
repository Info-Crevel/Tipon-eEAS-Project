<template>

<main id="home" class="main-container text-charcoal">

    <!-- <a href="/" title="Home">
          <img class="logo-lgu" src="img/TIPONbg.png"  alt="">
        </a> -->
  
  <!-- <section class="banner-container">
    <sym-slide-show class="banner-slider charcoal" :interval="8" :images="banners" :smartPause="false">
    </sym-slide-show>
  </section> -->

  <!-- <section class="section-wrapper lgu-orange-light">
    <p class="section-title serif text-center lgu-text-blue bold mb-3">Latest News</p>

    <div class="news-links mb-4">
      <div class="news-item light border-main curved shadow-light" v-for="(n, index) in news" :key="index" @click="onClickNews(n)">
        <img :src="getNewsCoverPhotoUrl(n)" class="w-100 d-block" loading="lazy">

        <div class="news-info">
          <p class="news-title bold mb-2">{{ n.articleTitle }}</p>
          <p class="mb-0">{{ core.toDateDisplayFormat(n.articleDate)}}</p>
        </div>
      </div>
    </div>

    <div class="text-center">
      <sym-link to="/news-archive" class="lg-2 button info text-center shadow-dark"><i class="fa fa-newspaper-o fa-lg mr-3"></i> Show All News</sym-link>
    </div>

  </section> -->

  <!-- <section class="section-wrapper lgu-blue-light">
    <div class="d-flex justify-center mb-3">
      <img src="img/lgu/atimonan-muna.png" alt="Atimonan Muna logo" class="logo-atimonan-muna">
    </div>

    <div class="discovery-wrapper lg-1">
      <p class="section-title serif text-center lgu-text-blue bold mb-3">Discover Atimonan</p>

      <div class="discovery-copy">
        <p>Atimonan is a progressive municipality, geared towards becoming the prime agro-industrial destination in Quezon Province.
          It is also the center for Nature-Based Adventure Tourism in Quezon. Its natural wonders, unexplored caves, pristine waters
          and century-old trees are a source of pride among Atimonanins.
        </p>

        <p>The municipality continues to be awarded and recognized in various fields, such as Good Financial Housekeeping, business
          friendliness, social services, health, nutrition, sanitation, employment service, cooperative development, disaster resilience,
          sports, environmental protection, tourism and agriculture.
        </p>

        <p>Get ready to be delighted by what our vibrant community has to offer.</p>

        <p class="lg-3 mb-4">See, explore, and discover Atimonan <b>now</b>!</p>
      </div>

      <div class="discovery-links">
        <div class="discovery-item light border-lightgray curved text-center shadow-light" v-for="(d, index) in discoverLinks" :key="index" @click="onClickCard(d.linkPath)">
          <img :src="getDiscoverPhotoUrl(d)" class="w-100 d-block" loading="lazy">
          <p class="lg-2 bold mb-0 p-1">{{ d.title }}</p>
        </div>
      </div>

    </div>

  </section> -->

  <!-- <section class="section-wrapper lgu-orange">
    <p class="section-title serif text-center text-light text-shadow bold curved mb-3">Announcements</p>

    <div class="announcement-container">
      <sym-slide-show
        class="announcement-slider lgu-vert-gradient"
        imageClass="slide-contain-fit"
        :interval="8"
        :images="announcements"
        :smartPause="false"
      >
      </sym-slide-show>

    </div>

  </section> -->

  <!-- <section class="offices-wrapper gainsboro" v-if="tags.length">
    <p class="section-title serif text-center lgu-text-blue bold mb-4">Offices / Departments</p>

    <div class="tags lg-1">
      <sym-link :to="getOfficePage(t.code)" :title="getOfficeTitle(t.code)" v-for="(t, index) in tags" :key="index" class="button border-main sm-1 light shadow">
        <span>{{ t.tag }}</span>
      </sym-link>
    </div>

  </section> -->

  <!-- <section class="gov-links-wrapper text-center lgu-blue-light">
    <p class="section-title serif lgu-text-blue bold mb-4">Government Links</p>

    <div class="gov-links">
      <a :href="o.link" target="_blank" class="gov-link curved" v-for="(o, index) in govOffices" :key="index">
        <img class="d-inline-block" :src="getOfficeLogoUrl(o.icon)" :width="o.iconWidth">
        <p class="sm-2 mb-0">{{ o.name }}</p>
      </a>
    </div>

  </section> -->

</main>

</template>

<script>

// import {
//   get
// } from '../../js/http';

import
  PageBase
from '../PageBase.vue';

export default {
  extends: PageBase,
  name: 'home',

  data () {
    return {
      // offices: [],
      // tags: [],
      // govOffices: [],

      // banners: [],
      // discoverLinks: [],
      // news: [],
      // announcements: []
    };
  },

  methods: {
    // loadData () {
    //   const
    //     me = this,
    //     wait = me.wait();

    //   me.getBanners()
    //     .then( banners => {
    //       banners.forEach(banner => {
    //         banner.imagePath = 'lgu/banner/' + banner.imagePath;
    //       });

    //       me.banners = banners;
    //       // return me.getArticles();
    //       return Promise.resolve([]);
    //     })

    //     .then( articles => {
    //       articles.forEach(article => {
    //         article.coverImage = 'lgu/news/' + article.imagePath;
    //       });

    //       me.news = articles;
    //       return me.getDiscoverLinks();
    //     })

    //     .then( links => {
    //       me.discoverLinks = links;
    //       return me.getAnnouncements();
    //     })

    //     .then( announcements => {
    //       announcements.forEach(announcement => {
    //         announcement.imagePath = 'lgu/announcement/' + announcement.imagePath;
    //       });

    //       me.announcements = announcements;
    //       return me.getOffices();
    //     })

    //     .then( offices => {
    //       me.offices = offices;
    //       return me.getTags();
    //     })

    //     .then( tags => {
    //       me.tags = tags;
    //       return me.getGovOffices();
    //     })

    //     .then( govOffices => {
    //       me.stopWait(wait);
    //       me.govOffices = govOffices;
    //     })

    //     .catch( fault => {
    //       me.stopWait(wait);
    //       me.showFault(fault);
    //     });

    // },

    // API calls

    // getBanners () {
    //   return get('api/banners');
    // },

    // getDiscoverLinks () {
    //   return get('api/discover-links');
    // },

    // getArticles () {
    //   return get('api/articles?page=1&ipp=3');
    // },

    // getAnnouncements () {
    //   return get('api/announcements');
    // },

    // getGovOffices () {
    //   return get('api/gov-offices');
    // },

    // getOffices () {
    //   return get('api/offices');
    // },

    // getTags () {
    //   return get('api/offices-tags');
    // },

    // event handlers

    // onClickNews (news) {
    //   let route = {
    //     name: 'local-news',
    //     params: {
    //       articleId: news.articleId
    //     },
    //     query: {
    //       page: this.currentPage
    //     }
    //   };
    //   this.go(route);
    // },

    // onClickCard (page) {
    //   this.go(page);
    // },

    // helpers

    // getNewsCoverPhotoUrl (news) {
    //   return 'img/lgu/news/' + news.coverPhoto;
    // },

    // getDiscoverPhotoUrl (link) {
    //   return 'img/lgu/discover/' + link.imagePath;
    // },

    // getOfficeLogoUrl (fileName) {
    //   return 'img/logo/gov/' + fileName;
    // },

    // getOfficePage (officeCode) {
    //   let office = this.offices.find(o => o.code === officeCode);
    //   if (office) {
    //     return '/' + office.page;
    //   }
    //   return '/' + officeCode;
    // },

    // getOfficeTitle (officeCode) {
    //   let office = this.offices.find(o => o.code === officeCode);
    //   return office ? office.name : '';
    // }

  },

  mounted () {
    // this.loadData();
  }

};

</script>

<style scoped>

.main-container {
  /* background: url('/img/bgd/worn-dots.png') repeat;
  background: url('/img/bgd/backdrop.jpg') no-repeat center center fixed; */
  background-image: url('/img/TEAM2.png');
  height: 100%;
  background-repeat: no-repeat;
  background-size:contain;
  background-position: center; 
 
 }

.banner-container {
  display: flex;
  justify-content: center;
  padding: 1.3875rem 0;
}

.banner-slider {
  width: 82%;
  border: 1px solid #bbb;
}

.news-links,
.discovery-links {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.5rem;
  padding: 0 3rem;
}

.news-item,
.discovery-item {
  position: relative;
  transition: all 0.3s ease-in-out;
  width: 31%;
}

.news-item:hover,
.discovery-item:hover {
  cursor: pointer;
  box-shadow: 2px 2px 16px 8px rgba(48, 48, 48, .3);
}

.news-title {
  font-size: 1.125rem;
  line-height: 1.3;
}

.news-info {
  padding: .75rem;
}

.announcement-container {
  display: flex;
  justify-content: center;
  margin: 1.3875rem 0;
}

.announcement-slider {
  width: 70%;
  border: 1px solid #bbb;
}

.section-wrapper {
  padding: 2rem 1.5rem 3rem;
}

.section-title {
  font-size: 1.75rem;
}

.offices-wrapper {
  padding: 3rem 1.5rem;
}

.tags {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: .625rem;
}

.logo-atimonan-muna {
  width: 18%;
}

.discovery-copy {
  padding: 0 3rem;
  text-align: center;
}

.gov-links-wrapper {
  padding: 3rem 1rem;
}

.gov-links {
  display: flex;
  flex-wrap: wrap;
  padding: 0 1rem;
  justify-content: center;
  gap: .75rem;
}

.gov-link {
  text-decoration: none;
  padding: .375rem .5rem;
  width: 6rem;
  min-width: 6rem;
  border: 1px solid lightgray;
  box-shadow: 3px 3px 3px 0px rgba(128, 128, 128, .1);
  background: white;
}

.gov-link:hover {
  border-color: var(--lgu-orange);
}

.gov-link:focus {
  outline-color: var(--lgu-orange);
}

@media (max-width: 1100px) {
  .logo-atimonan-muna {
    width: 27%;
  }
}

@media (max-width: 1024px) {
  .logo-atimonan-muna {
    width: 22%;
  }
}

@media (max-width: 900px) {
  .news-links,
  .discovery-links {
    gap: 1rem;
    padding: 0 1rem;
  }

  .news-item,
  .discovery-item {
    width: 40%;
  }
}

@media (max-width: 800px) {
  .banner-slider {
    width: 100%;
    border: none;
  }

  .announcement-slider {
    width: 100%;
  }

  .news-item,
  .discovery-item {
    width: 48%;
  }
}

@media (max-width: 640px) {
  .news-item,
  .discovery-item {
    width: 88%;
  }
}

@media (max-width: 480px) {
  .banner-container {
    margin: 1rem 0;
  }

  .section-wrapper {
    padding: 1rem .75rem 2rem;
  }

  .section-title {
    font-size: 1.5rem;
  }

  .logo-atimonan-muna {
    width: 44%;
  }

  .discovery-wrapper p {
    line-height: 1.3;
  }

  .discovery-copy {
    padding: 0 1rem;
  }

  .news-links,
  .discovery-links {
    gap: 1rem;
    padding: 0 .5rem;
  }

  .news-item,
  .discovery-item {
    width: 100%;
    padding: 0;
  }

  .tags {
    gap: .375rem;
  }

  .gov-links-wrapper {
    padding: 1.5rem 0;
  }
}

@media (max-width: 320px) {
  .section-wrapper {
    padding: 1rem .5rem 1.5rem;
  }

  .section-title {
    font-size: 1.375rem;
  }

  .logo-atimonan-muna {
    width: 56%;
  }

  .news-links,
  .discovery-links {
    padding: 0;
  }

  .discovery-wrapper p {
    line-height: 1.2;
  }

  .discovery-copy {
    padding: 0;
  }

  .offices-wrapper {
    padding: 2rem 0;
  }

  .gov-links {
    padding: 0 .5rem;
    gap: .25rem;
  }
}

</style>
