/* jshint esversion: 6 */

import {
  setup
} from './app';

import {
  config
} from './core';

import {
  removeElement
} from './dom';

import
  Password
from './../comp/SymPassword.vue';

import
  Vue
from 'vue';

function destroy (instance) {
  removeElement(instance.$el);
  instance.$destroy();
}

function init (message, password, options, resolve = null) {
  const Component = Vue.extend(Password);
  let instance = new Component({
    propsData: {
      message,
      correctPassword: password,
      scheme: options.scheme,
      title: options.title,
      size : options.size,
      buttonScheme: options.buttonScheme,
      transitionDuration: options.transitionDuration,
      callback (reply) {
        destroy(instance);
        if (resolve) {
          resolve(reply);
        }
      }
    }
  });

  instance.$mount();
  document.body.appendChild(instance.$el);
  instance.show = true;
  return instance;
}

export default {

  pass (message, correctPassword, options = {}) {
    if (!options.size) {
      options.size = 'md';
    }
    if (!options.buttonScheme) {
      options.buttonScheme = config.buttonScheme;
    }
    if (!options.title) {
      options.title = setup.appName;
    }

    return new Promise((resolve) => {
      init(message, correctPassword, options, resolve);
    });

    // setDefaults(options, NotificationType.Advice, 'info', options.icon || 'info-circle');
    // return notify(message, options);
  }

}