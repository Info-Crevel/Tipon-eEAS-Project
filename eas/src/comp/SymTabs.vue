<template>

<div>
  <ul class="tabs">
    <template v-for="(tab, index) in tabList">
      <li :key=index>
        <a href="#" :class="getTabClass(tab)" @click.prevent="select(tabs.indexOf(tab))">
          <i v-if="tab.icon" :class="tab.iconClass" ></i>{{ tab.title }}
        </a>
      </li>
    </template>
  </ul>
  <div class="tab-content">
    <slot/>
  </div>
</div>

</template>

<script>

const BEFORE_CHANGE_EVENT = 'before-change';

export default {
  props: {
    value: { type: Number, validator: v => v >= 0 },
    transition: { type: Number, default: 150 }
  },

  data () {
    return {
      tabs: [],
      activeIndex: 0 // no need for v-model
    };
  },

  computed: {
    tabList () {
      let tabs = [];
      this.tabs.forEach(tab => {
        tabs.push(tab);
      })
      return tabs;
    }

  },

  watch: {
    value: {
      immediate: true,
      handler (value) {
        if (this.core.isNumber(value)) {
          this.activeIndex = value;
          this.selectCurrent();
        }
      }
    },

    tabs (tabs) {
      tabs.forEach((tab, index) => {
        tab.transition = this.transitionDuration;
        if (index === this.activeIndex) {
          tab.show();
        }
      })
      this.selectCurrent();
    }
  },

  methods: {
    getTabClass (tab) {
      let defaultClass = {
        active: tab.active,
        disabled: tab.disabled
      };

      return Object.assign(defaultClass, tab['tabClass']);
    },

    selectCurrent () {
      let found = false;
      this.tabs.forEach((tab, index) => {
        if (index === this.activeIndex) {
          found = !tab.active;
          tab.active = true;
        } else {
          tab.active = false;
        }
      });

      if (found) {
        this.$emit('changed', this.activeIndex);
      }
    },

    selectValidate (index) {
      if (this.core.isFunction(this.$listeners[BEFORE_CHANGE_EVENT])) {
        this.$emit(BEFORE_CHANGE_EVENT, this.activeIndex, index, (result) => {
          if (!this.core.isDefined(result)) {
            this.$select(index);
          }
        });
      } else {
        this.$select(index);
      }
    },

    select (index) {
      if (!this.tabs[index].disabled && index !== this.activeIndex) {
        this.selectValidate(index);
      }
    },

    $select (index) {
      if (this.core.isNumber(this.value)) {
        this.$emit('input', index);
      } else {
        this.activeIndex = index;
        this.selectCurrent();
      }
    }
  }

}

</script>
