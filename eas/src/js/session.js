/* jslint esversion: 6  */

import {
  getAppConfig
} from './dbSys';

let appConfig = {};

// export function start () {
//   return Promise.resolve();
// }

export function start () {
  return getAppConfig()
    .then( config => {
      Object.assign(appConfig, config);
      return Promise.resolve();
    })
    .catch( () => {
    });
}

export {
  appConfig
};
