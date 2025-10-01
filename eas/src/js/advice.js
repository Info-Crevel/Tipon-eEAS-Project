/* jshint esversion: 6 */

import {
  removeElement
} from './dom';

import {
  isDefined,
  isString,
  isNumber,
  isFunction,
  isPromiseSupported,
  deleteArrayItem
} from './core';

import
  Notification
from './../comp/SymNotification.vue';

import
  Vue
from 'vue';

const alertAlignments = {
  topLeft: 'top-left',
  topRight: 'top-right',
  bottomLeft: 'bottom-left',
  bottomRight: 'bottom-right'
};

const queues = {
  [alertAlignments.topLeft]: [],
  [alertAlignments.topRight]: [],
  [alertAlignments.bottomLeft]: [],
  [alertAlignments.bottomRight]: []
};

function init (message, options, callback, resolve = null, reject = null) {

  const queue = queues[options.alignment];
  if (!isDefined(queue)) { return; }

  const Component = Vue.extend(Notification);
  let instance = new Component({
    propsData: {
      message,
      type: options.type,
      title: options.title,
      isHtml: options.isHtml,
      duration: options.duration,
      dismissible: options.dismissible,
      alignment: options.alignment,
      icon: options.icon,
      queue,
      offsetX: options.offsetX,
      offsetY: options.offsetY,
      offset: options.offset,
      width: options.width,
      callback (msg) {
        destroy(queue, instance);
        if (isFunction(callback)) {
          callback(msg);
        } else if (resolve && reject) {
          resolve(msg);
        }
      }
    }
  });

  instance.$mount();
  document.body.appendChild(instance.$el);
  queue.push(instance);
}

function notify (message, type, options, callback) {

  if (isString(options)) {
    options = { title: options };
  }

  if (isNumber(options)) {
    options = { duration: options };
  }

  options.type = type;

  if ('title' in options && message) {
    if (options.enclosed) {
      message = `<hr>(${message})`;
    } else {
      message = `<hr>${message}`;
    }
  }

  if (message) {
    if (!isDefined(options.isHtml) && (message.includes('</')) || message.includes('<br>') || message.includes('<hr>')) {
      options.isHtml = true;
    }
  }

  if (!isDefined(options.alignment)) {
    options.alignment = alertAlignments.topRight;
  }

  if (options.dismissible === undefined) {
    options.dismissible = false;
  }

  if (!isString(options.icon)) {
    switch (type) {
      case 'info': options.icon = 'info-circle'; break;
      case 'success': options.icon = 'check-circle'; break;
      case 'warning': options.icon = 'warning'; break;
      case 'danger': options.icon = 'times-circle'; break;
    }
  }

  if (isPromiseSupported()) {
    return new Promise((resolve, reject) => {
      init(message, options, callback, resolve, reject);
    });
  } else {
    init(message, options, callback);
  }
}

function destroy (queue, instance) {
  removeElement(instance.$el);
  instance.$destroy();
  deleteArrayItem(queue, instance);
}

export default {

  info (message, options = {}, callback = null) {
    return notify(message, 'info', options, callback);
  },

  success (message, options = {}, callback = null) {
    return notify(message, 'success', options, callback);
  },

  warning (message, options = {}, callback = null) {
    return notify(message, 'warning', options, callback);
  },

  fault (message, options = {}, callback = null) {
    return notify(message, 'danger', options, callback);
  }

};
