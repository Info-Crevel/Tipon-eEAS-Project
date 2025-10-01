/* jshint esversion: 6 */

import {
  setup
} from './app';

import
  * as dom
from './dom';

import {
  config
} from './core';

import {
  getLookup
} from './dbSys';

import
  Lookup
from './../comp/SymLookup.vue';

import
  Vue
from 'vue';

let cache = {};

function destroy (instance) {
  dom.off(document, 'httpfault', onHttpFault);
  dom.removeElement(instance.$el);
  instance.$destroy();
}

function init (options, resolve = null) {
  const Component = Vue.extend(Lookup);
  let instance = new Component({
    propsData: {
      lookupId: options.lookupId,
      triggerControl: options.triggerControl,
      params: options.params,
      scheme: options.scheme,
      title: options.title,
      // size : options.size,
      lookupSize: options.lookupSize,
      querySize: options.querySize,
      dismissible: options.dismissible,
      buttonScheme: options.buttonScheme,
      footer: true,
      searchValue: options.searchValue,
      callback (result) {
        destroy(instance);
        if (resolve) {
          resolve(result);
        }
      }
    }
  });

  instance.$mount();
  document.body.appendChild(instance.$el);
  instance.show = true;
  return instance;
}

function onHttpFault (e) {
  if (e.fault.status === 404 && e.url && e.url.startsWith('api/lookups')) {
    e.throwFault = true;
  }
}

function loadLookup (lookupId) {
  if (lookupId in cache) {
    return Promise.resolve(cache[lookupId]);
  } else {
    let p = getLookup(lookupId);
    p.then(
      params => {
        cache[lookupId] = params;
      }
    )
    return p;
  }
}

export default {
  show (lookupId, options = {}) {
    dom.cursorWait();
    dom.on(document, 'httpfault', onHttpFault);

    // return getLookup(lookupId).then(
    return loadLookup(lookupId).then(
      params => {
        dom.cursorDefault();
        options.lookupId = lookupId;
        options.title = setup.appName;
        // options.size = options.size || params.size || 'rg';
        options.lookupSize = options.lookupSize || params.lookupsize || 'rg';
        options.querySize = options.querySize || params.querysize || 'rg';
        options.scheme = 'info';
        options.buttonScheme = config.buttonScheme;
        if (options.filter) {
          params.filter = options.filter;
        }
        if (options.sort) {
          params.sort = options.sort;
        }
        params.hasQuery = ('query' in params && Array.isArray(params.query));
        params.hasCombo = ('combo' in params && Array.isArray(params.query));

        options.params = params;

        return new Promise((resolve) => {
          init(options, resolve);
        });
      },
      fault => {
        dom.cursorDefault();
        return Promise.reject(fault);
      }
    );

  }

};