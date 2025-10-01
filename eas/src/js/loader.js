/* jshint esversion: 6 */

import {
  removeElement
} from './dom';

import
  ProgressBar
from './../comp/SymProgressBar.vue';

import
  Vue
from 'vue';

let
  value = 0,
  intervalId = 0,
  instance = null;

function start () {
  if (intervalId) {
    clearInterval(intervalId);
  }

  if (instance) {
    destroy(instance);
  }

  const Component = Vue.extend(ProgressBar);

  instance = new Component({
    propsData: {
      themeId: 'warning'
    }
  });

  instance.$mount();
  document.body.insertBefore(instance.$el, document.body.firstChild);
  instance.$el.classList.add('progress-overlay', 'sm-3');

  intervalId = setInterval( () => {
    if (instance) {
      value += 20;
      instance.loaderValue = Math.min(value, 100);
      if (value > 200) {
        clearInterval(intervalId);
      }
    }
  }, 100);
}

function done () {
  reset();
  if (instance) {
    instance.loaderValue = 100;
    setTimeout( () => {
      destroy(instance);
    }, 800);
  }
}

function reset () {
  if (intervalId) {
    clearInterval(intervalId);
  }
  value = 0;
}

function destroy (instance) {
  removeElement(instance.$el);
  instance.$destroy();
}

export default {
  start,
  done
};
