/* jshint esversion: 6 */

import {
  setup
} from './app';

import {
  config,
  isDefined,
  isString
} from './core';

import {
  removeElement
} from './dom';

import {
  NotificationType
} from './tags';

import
  Dialog
from './../comp/SymDialog.vue';

import
  Vue
from 'vue';

function destroy (instance) {
  removeElement(instance.$el);
  instance.$destroy();
}

function init (message, options, resolve = null) {
  const Component = Vue.extend(Dialog);
  let instance = new Component({
    propsData: {
      message,
      scheme: options.scheme,
      title: options.title,
      isHtml: options.isHtml,
      icon: options.icon,
      type: options.type,
      size : options.size,
      okCaption: options.okCaption,
      rejectCaption: options.rejectCaption,
      cancelCaption: options.cancelCaption,
      cancelButton: options.cancelButton,
      buttonScheme: options.buttonScheme,
      defaultReply: options.defaultReply,
      footer: options.footer,
      keyboard: options.keyboard,
      backdrop: options.backdrop,
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

function notify (message, options) {

  if (isString(options)) {
    options = { title: options };
  }

  if (message) {
    if (!isDefined(options.isHtml) && (message.includes('</')) || message.includes('<br>') || message.includes('<hr>')) {
      options.isHtml = true;
    }
  }

  if (options.type === NotificationType.Wait) {
    return init(message, options);
  }

  return new Promise((resolve) => {
    init(message, options, resolve);
  });

}

function setDefaults (options, type, scheme, icon) {
  options.type = type;
  options.scheme = scheme;
  options.icon = icon;
  if (!options.size) {
    options.size = 'rg';
  }
  options.buttonScheme = config.buttonScheme;
  if (!options.title) {
    options.title = setup.appName;
  }
  if (type === NotificationType.Confirmation) {
    if (!('defaultReply' in options)) {
      options.defaultReply = 'ok';
    }
  }
}

export default {

  info (message, options = {}) {
    setDefaults(options, NotificationType.Advice, 'info', options.icon || 'info-circle');
    return notify(message, options);
  },

  success (message, options = {}) {
    setDefaults(options, NotificationType.Advice, 'success', options.icon || 'check-circle');
    return notify(message, options);
  },

  warning (message, options = {}) {
    setDefaults(options, NotificationType.Advice, 'warning', options.icon || 'warning');
    return notify(message, options);
  },

  fault (message, options = {}) {
    setDefaults(options, NotificationType.Advice, 'danger', options.icon || 'times-circle');
    return notify(message, options);
  },

  confirm (message, options = {}) {
    setDefaults(options, NotificationType.Confirmation, options.scheme || 'info', options.icon || 'exclamation-circle');
    return notify(message, options);
  },

  confirmCancel (message, options = {}) {
    if (!('cancelButton' in options)) {
      options.cancelButton = true;
    }
    setDefaults(options, NotificationType.Confirmation, options.scheme || 'info', options.icon || 'exclamation-circle');
    return notify(message, options);
  },

  wait (scheme, icon) {
    const options = {};
    setDefaults(options, NotificationType.Wait, scheme || config.loaderScheme, icon || config.loaderIcon);
    options.footer = false;
    options.keyboard = false;
    options.backdrop = false;
    options.title = '';
    options.size = 'sm';
    return notify('', options);
  },

  stop (modalInstance) {
    if (modalInstance && ('show' in modalInstance) ) {
      modalInstance.show = false;
    }
  }

};
