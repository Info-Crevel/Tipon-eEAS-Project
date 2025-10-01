/* jshint esversion: 6 */

import {
  setup
} from './app';

import {
  getUserInfo
} from './dbSys';

import {
  config,
  sessionDb,
  DateTime,
  isDefined,
  isPageId,
  toDate,
  padLeft,
  setPageTitle
} from './core';

import {
  on,
  hideSpinner
} from './dom';

import {
  getSysInfo,
  getReferenceDates
} from './dbSys';

import {
  start
} from './session';

let
  dateRefreshCounter = 0,
  startSeconds = 0,
  secondsElapsed = 0;

const
  alerts = [],
  smallDeviceMaxWidth = 480,

  tags = {
    newLine: String.fromCharCode(13)
  },

  defaultDates = {
    today: DateTime.today(),
    yesterday: DateTime.today().addDays(-1),
  },

  dateInfo = {
    systemDate: defaultDates.today,
    auditDate: defaultDates.today,
    serverDate: defaultDates.today,
    serverTime: '00:00:00',
    systemDateDisplay: '',
    auditDateDisplay: '',
    serverDateDisplay: '',
    systemDateTimeDisplay: '',
    serverDateTimeDisplay: '',

    refreshDisplays () {
      const me = this;
      me.systemDateDisplay = me.systemDate.toDateDisplayFormat();
      me.auditDateDisplay = me.auditDate.toDateDisplayFormat();
      me.serverDateDisplay = me.serverDate.toDateDisplayFormat();
      me.systemDateTimeDisplay = me.buildDateTime(me.systemDate).toDateTimeDisplayFormat();
      me.serverDateTimeDisplay = me.buildDateTime(me.serverDate).toDateTimeDisplayFormat();
    },

    buildDateTime (dateTime) {
      return new DateTime(dateTime.year, dateTime.month, dateTime.day, parseInt(this.serverTime.substr(0,2)), parseInt(this.serverTime.substr(3,2)), 0);
    }
  },

  sysInfo = {
    siteId: 101,
    siteName: 'Symphony Manila',
    siteShortName: 'Symphony Manila',
    sessionTimeout: 180,    // minutes
    currencyId: 'PHP',
    productTitle: 'Symphony',
    productDescription: '',
    productName: '',
    yearId: 2022
  },

  userInfo = {
    userId: 0,
    userName: '',
    userShortName: '',
    photo: null,
    isSysAdmin: false,
    workgroupName: '',
    pageId: '',
    pageName: '',
    pagePath: '',
    pages: [],
    actions: [],
    restrictions: [],
    token: '',
    userCode: '',
    modules: [],
    moduleId: '',
    // careCenterId: 0,
    // careCenterName: '',
    // accountId: 0,
    // accountCode: '',
    // accountTypeId: 0,
    // accountStatusId: 0,
    imageUrl: '',

    get isAuthenticated () {
      return this.token !== '';
    },

    get reset () {
      return function () {
        this.userId = 0;
        this.userName = '';
        this.userShortName = '';
        this.photo = null;
        this.pageId = '';
        this.pageName = '';
        this.pagePath = '';
        this.isSysAdmin = false;
        this.workgroupName = '';
        this.pages = [];
        this.actions = [];
        this.restrictions = [];
        this.token = '';
        this.userCode = '';
        this.modules = [];
        this.moduleId = '';
        // this.careCenterId = 0;
        // this.careCenterName = '';
        // this.accountId = 0;
        // this.accountCode = '';
        // this.accountTypeId = 0;
        // this.accountStatusId = 0;
        this.imageUrl = '';
      };
    },

    get hasPageAccess () {
      return function (pageIdPath) {
        return this.isSysAdmin ||
              this.pages.findIndex(obj => obj.pageId === pageIdPath) > -1 ||
              this.pages.findIndex(obj => obj.pagePath === pageIdPath) > -1;
      };
    },

    get hasModuleAccess () {
      return function (moduleId) {
        return this.isSysAdmin ||
              this.modules.findIndex(obj => obj.moduleId === moduleId) > -1;
      };
    },

    get restore () {
      return function () {
        const ta = sessionDb.get(config.userInfoTag);
        if (ta) {
          return getUserInfo(ta.token, ta.key).then(
            info => {
              if (info) {
                userInfo.update(info);
                return true;
              }
              return false;
            },
            () => {
              return false;
            }
          );
        } else {
          // return Promise.resolve(true);
          return Promise.resolve(false);
        }
      };
    },

    get update () {
      return function (info) {
        if (!isDefined(info)) {
          this.reset();
          sessionDb.remove(config.userInfoTag);
        } else {
          Object.assign(this, info);
          sessionDb.set(config.userInfoTag, {
            token: info.token,
            // key: info.accountId
            key: info.userCode
          });
        }
      };
    }

  };

export function init (vue) {
  return getSysInfo()
    .then( info => {
      if (!info) {
        hideSpinner();
        return;
      }
      Object.assign(sysInfo, info);
      on(window, 'resize', onResize);
      onResize();
      // setPageTitle(setup.appName + ' - ' + info.siteName);
      setPageTitle('eEAS - TIPON');
      // hideSpinner();
      return refreshReferenceDates();
    })
    .then( () => {
      return start();
    })
    .then( () => {
      dateInfo.refreshDisplays();
      if (setup.dateRefreshInterval > 0) {
        setInterval(dateIntervalHandler, 1000);
      }
      hideSpinner();
      vue.$mount('#app');
    })
    .catch( () => {
      hideSpinner();
    }
  );
}

export function getPath (pageId) {
  if (!isPageId(pageId)) { return pageId; }
  let index = sysInfo.pages.findIndex(obj => obj.pageId === pageId);
  return index < 0 ? pageId : sysInfo.pages[index].pagePath;
}

export function getPageName (pageId) {
  if (!isPageId(pageId)) { return pageId; }
  let index = sysInfo.pages.findIndex(obj => obj.pageId === pageId);
  return index < 0 ? pageId : sysInfo.pages[index].pageName;
}

// 05 Feb 2025 - EMT
export function getPageInfo (pageId) {
  let p = {};
  p.pageId = pageId;
  p.pagePath = '';
  p.xa = false;
  p.xe = false;
  p.xd = false;

  if (!isPageId(pageId)) { return p; }
  let index = userInfo.pages.findIndex(obj => obj.pageId === pageId);
  return index < 0 ? p : userInfo.pages[index];
}

export function isActionAllowed (...actionNames) {
  if (!userInfo.actions.length) {
    return true;
  }

  for (const actionName of actionNames) {
    if (!userInfo.actions.includes(actionName)) {
      return false;
    }
  }
  return true;
}

function refreshReferenceDates () {
  return getReferenceDates()
    .then( info => {
      Object.assign(dateInfo, info, {
        systemDate: toDate(info.systemDate, true),
        serverDate: toDate(info.serverDate, true),
        auditDate: toDate(info.auditDate, true),
        serverTime: info.serverTime.substr(0,8)
        }
      );
      startSeconds = parseInt(info.serverTime.substr(6,2));
      secondsElapsed = 0;
    })
    .catch( () => {
    });
}

function dateIntervalHandler () {
  secondsElapsed += 1;

  if (startSeconds + secondsElapsed >= 60) {
    dateRefreshCounter += 1;
    if (dateRefreshCounter >= setup.dateRefreshInterval) {
      dateRefreshCounter = 0;
      refreshReferenceDates()
        .then( () => {
          dateInfo.refreshDisplays();
        })
        .catch( () => {
        });
    } else {
      let
        today = defaultDates.today,
        hour = parseInt(dateInfo.serverTime.substr(0,2)),
        minute = parseInt(dateInfo.serverTime.substr(3,2));

      if (minute === 59) {
        minute = 0;
        hour = (hour === 23) ? 0 : hour + 1;
      } else {
        minute += 1;
      }

      if (hour + minute === 0) {
        defaultDates.yesterday = today;
        today = today.addDays(1);
        dateInfo.serverDate = today;
        defaultDates.today = dateInfo.serverDate;
      }

      hour = padLeft(hour, 2, '0');
      minute = padLeft(minute, 2, '0');

      dateInfo.serverTime = hour + ':' + minute + ':00';
      dateInfo.refreshDisplays();
      secondsElapsed = 0;
      startSeconds = 0;
    }
  }
}

function onResize () {
  config.adviceWidth = window.innerWidth <= smallDeviceMaxWidth ? 240 : 290;
}

function hasPageAccess (pageIdPath) {
  return userInfo.hasPageAccess(pageIdPath);
}

function hasModuleAccess (moduleId) {
  return userInfo.hasModuleAccess(moduleId);
}

function hasMenuSystem () {
  return setup.isMenuEnabled && userInfo.isAuthenticated && (userInfo.pages.length || userInfo.isSysAdmin);
}

export {
  alerts,
  tags,
  sysInfo,
  userInfo,
  dateInfo,
  defaultDates,
  hasPageAccess,
  hasModuleAccess,
  hasMenuSystem
};
