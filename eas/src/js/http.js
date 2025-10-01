/* jslint esversion: 6  */

import {
  userInfo
} from './sym';

import {
  showHttpFault
} from './core';

import
  loader
from './loader';

export function get (url) {
  const options = {
    headers: { Accept: 'application/json' }
  };
  return ajax(url, options);
}

export function ajax (url, options, includeToken = true) {
  if (includeToken && userInfo.token) {
    if (!('headers' in options)) {
      options.headers = {};
    }
    options.headers.Token = userInfo.token;
  }

  const request = new Request(url, options);
  loader.start();

  return fetch(request).then(handleHttpFault).then(
    response => {
      loader.done();
      if (options && options.headers && 'Accept' in options.headers && options.headers.Accept.indexOf('/json') > -1) {
        return response.json();
      } else {
        return response.text();
      }
    },
    fault => {
// console.log(fault);
      loader.done();
      if (fault instanceof TypeError) {
        showHttpFault(new HttpFault(fault.message, 503, 'Service Unavailable', request.url));
      } else {
        if ((fault.status === 401 || fault instanceof HttpFault) && !options.logonFlag) {
          const e = new Event('httpfault', { cancelable: true });
          e.fault = fault;
          e.url = url;
          e.throwFault = false;
          document.dispatchEvent(e);
          if (!e.throwFault) {
            if (!e.defaultPrevented) {
              showHttpFault(fault);
            }
            return;
          }
        }
        throw fault;
      }
    }
  );
}

export function upload (url, files) {
  const
    formData = new FormData(),
    options = {
      method: 'POST',
      body: formData,
      headers: { Accept: 'application/json' }
    };

  for (let i = 0; i < files.length; i++) {
    formData.append(files[i].name, files[i]);
  }

  return ajax(url, options);
}

export function encode (text) {
  // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/encodeURIComponent
  let s = encodeURIComponent(text).replace(/[!'()*]/g, function (c) {
    return '%' + c.charCodeAt(0).toString(16).toUpperCase();
  });
  return s;
}

function handleHttpFault (response) {
  if ( [401,404].includes(response.status)) {
    let statusText = response.status === 401 ? 'Unauthorized' : 'Not Found';
    throw new HttpFault(statusText, response.status, statusText, response.url);
  }
  if (!response.ok) {
    return response.text()
      .then( text => {
        let
          o = '',
          message = '';

        if (text && !text.startsWith('<')) {
          o = JSON.parse(text);
          message = typeof o === 'object' && 'message' in o ? o.message : o;
        } else {
          message = response.statusText;
        }
        throw new HttpFault(message, response.status, response.statusText, response.url);
      });
  }
  return response;
}

export function HttpFault (message, status, statusText, url) {
  this.message = message;
  this.status = status || 200;
  this.statusText = statusText || 'OK';
  this.url = url;
}

HttpFault.prototype = new Error();
