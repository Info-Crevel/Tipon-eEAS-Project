<template>

 <div class="menu container curved-1 border-light">
  <div class="info text-center p-2 serif lg-3 curved-top-2">
    Main Menu
  </div>

  <div class="menu-content info-light" ref="menu-content">

    <sym-card id="site-explorer" class="beige shadow-light border-main">
      <div slot="header">
        <span class="menu-item bold">Site Explorer</span>
      </div>

      <ul class="menu-item serif">
        <li><sym-link :class="listItemClass('rpt0010')" to="rpt0010" title="Site Reports">Reports</sym-link></li>
        <li class="list-item-separator"></li>
        <li><sym-link :class="listItemClass('sts1010')" to="sts1010" title="Daily Occupancy">Occupancy</sym-link></li>
        <li><sym-link :class="listItemClass('sts1020')" to="sts1020" title="Site Trends">Trends</sym-link></li>
      </ul>
    </sym-card>

    <sym-card id="room-type-stats" class="beige shadow border-main">
      <div slot="header">
        <span class="menu-item bold">Room Type Statistics</span>
      </div>

      <ul class="menu-item serif">
        <li><sym-link :class="listItemClass('sts1030')" to="sts1030" title="Room Type Occupancy">Occupancy</sym-link></li>
        <li><sym-link :class="listItemClass('sts1040')" to="sts1040" title="Room Type Trends">Trends</sym-link></li>
      </ul>
    </sym-card>

  </div>

</div>

</template>

<script>

export default {

  methods: {
    listItemClass ( pageId ) {
      return {
        'list-item': true,
        'disabled': !(this.sym.hasPageAccess(pageId) && this.canAccessPage(pageId))
      }
    },

    initMenuBlocks () {
      let count = 0;

      count += this.initMenuBlock('site-explorer');
      count += this.initMenuBlock('room-type-stats');

      if (count === 1) {
        this.$refs['menu-content'].classList.add('single-column');
      }
    },

    initMenuBlock ( menuId ) {
      const
        el = document.getElementById(menuId),
        selectors = '#' + menuId + ' ul .list-item';

      let items = document.querySelectorAll(selectors);
      if (items.length > 0) {
        for (let item of items) {
          if (!item.classList.contains('disabled')) {
            return 1;
          }
        }
        if (el.classList) {
          el.classList.add('collapse');
        }
      }
      return 0;
    },

    onKeyDown (e) {
      if (e.key === 'Escape') {
        this.$router.back();
      }
    },

    canAccessPage (pageId) {
      if (!pageId) {
        return false;
      }
      return true;
    }

  },

  mounted () {
    this.initMenuBlocks();
    this.dom.on(window, 'keydown', this.onKeyDown);
  },

  updated () {
    this.initMenuBlocks();
  },

  beforeDestroy () {
    this.dom.off(window, 'keydown', this.onKeyDown);
  }

}

</script>

<style>

.menu.container {
  padding: 0;
}

.menu-content {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  column-gap: 1.5rem;
  row-gap: 1rem;
  padding: 1.5rem 10rem;
}

.menu-item {
  font-size: 1.125rem;
}

ul {
  list-style-type: none;
  margin-bottom: .5rem;
}

ul a {
  color: inherit;
  padding-top: .5rem;
  padding-bottom: .5rem;
  text-decoration: none;
}

ul a.disabled {
  opacity: 0.4;
}

ul a:not(.disabled):hover {
  background: gainsboro;
}

.single-column {
  display: flex;
  justify-content: center;
}

.single-column section {
  min-width: 30rem;
  width: 50%;
}

@media (max-width: 1260px) {
  .menu-content:not(.single-column) {
    padding-left: 6.75rem;
    padding-right: 6.75rem;
  }
}

@media (max-width: 1100px) {
  .menu-content:not(.single-column) {
    padding-left: 4.5rem;
    padding-right: 4.5rem;
  }
}

@media (max-width: 1060px) {
  .menu-content:not(.single-column) {
    padding-left: 1.5rem;
    padding-right: 1.5rem;
  }
}

@media (max-width: 740px) {
  .menu-content {
    padding: 1.5rem;
    grid-template-columns: 1fr;
  }

  .single-column section {
    width: 100%;
    min-width: initial;
  }
}

</style>