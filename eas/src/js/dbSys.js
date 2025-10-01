/*jshint esversion: 6 */

import {
  get,
  ajax,
  encode
} from './http';

export function getAppConfig () {
  return get('api/config');
}

export function getSysInfo () {
  return get('api/sysinfo');
}

// export function getSysInfo () {
//   let o = {
//     siteName: 'Municipality of Atimonan',
//     siteShortName: 'Atimonan',
//     siteId: 2003,
//     sessionTimeout: 10,
//     activeTokenCount: 0,
//     currencyId: 'PHP',
//     pages: [],
//     productDescription: 'eATM 2.0',
//     productName: 'Symphony eATM 2.0',
//     productTitle: 'Symphony',
//   }

//   return Promise.resolve(o);
//   // return get('api/sysinfo');
// }

export function getReferenceDates () {
  return get('api/refdates');
}

// export function getUserInfo (token, accountId) {
//   return get('api/userinfo?token=' + token + '&accountId=' + accountId.toString());
// }

export function getUserInfo (token, userCode) {
  return get('api/userinfo?token=' + token + '&userCode=' + userCode);
}

export function getLookup (lookupId) {
  return get('api/lookups/' + lookupId);
}

export function getList (dataSource, fields, sort = '', filter = '') {
  let q = 'dataSource=' + dataSource + '&fields=' + fields;
  if (filter) {
    q = q + '&filter=' + encode(filter);
  }
  if (sort) {
    q = q + '&sort=' + sort;
  }
  return get('api/list?' + q);
}

export function getCount (dataSourceName, filter) {
  return get('api/count?dataSourceName=' + dataSourceName + '&filter=' + filter);
}

export function getSafeDeleteFlag (tableName, rowId) {
  return get('api/safedel/' + tableName + '/' + rowId.toString());
}

export function getLinkStatus (url, options = { mode: 'no-cors' }) {
  return fetch(url, options);     // returns an opaque Response object
}

export function logon (key) {
  const options = {
    method: 'POST',
    headers: {
      Authorization: 'Basic ' + key,
      Accept: 'application/json'
    },
    logonFlag: true
  };
  return ajax('api/logon', options);
}

export function logoff () {
  const options = {
    method: 'DELETE'
  };
  return ajax('api/logoff', options);
}

export function verifyRecaptcha (response) {
  const body = JSON.stringify(
    {
      response: response
    }
  );

  const options = {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: body
  };

  return ajax('api/recaptcha', options);
}