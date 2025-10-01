/* jslint esversion: 6 */

import './css/font-awesome.css';
import './css/sym.css';
import './css/app.css';

import
  * as app
from './js/app';

import
  * as sym
from './js/sym';

import
  * as core
from './js/core';

import
  * as dom
from './js/dom';

import
  * as tags
from './js/tags';

import
  advice
from './js/advice';

import
  dialog
from './js/dialog';

import
  lookup
from './js/lookup';

import
  loader
from './js/loader';

import
  security
from './js/security';

import Vue from 'vue';
import Router from 'vue-router';

const App = () => import(/* webpackChunkName: "main-app" */ './App.vue');

import Link from './comp/SymLink.vue';
import GoTop from './comp/SymGoTop.vue';

const p = Vue.prototype;
p.eventBus = new Vue();
p.app = app;
p.sym = sym;
p.core = core;
p.dom = dom;
p.tags = tags;
p.advice = advice;
p.dialog = dialog;
p.lookup = lookup;
p.loader = loader;
p.security = security;

dom.showSpinner();

const focusSelectors = ['input','textarea','select'];

const Alert = () => import(/* webpackChunkName: "controls" */ './comp/SymAlert.vue');
const Modal = () => import(/* webpackChunkName: "controls" */ './comp/SymModal.vue');
const InputButton = () => import(/* webpackChunkName: "controls" */ './comp/SymInputButton.vue');
const SymText = () => import(/* webpackChunkName: "controls" */ './comp/SymText.vue');
const SymTime = () => import(/* webpackChunkName: "controls" */ './comp/SymTime.vue');
const SymDate = () => import(/* webpackChunkName: "controls" */ './comp/SymDate.vue');
const SymInteger = () => import(/* webpackChunkName: "controls" */ './comp/SymInteger.vue');
const SymDecimal = () => import(/* webpackChunkName: "controls" */ './comp/SymDecimal.vue');
const SymDropdown = () => import(/* webpackChunkName: "controls" */ './comp/SymDropdown.vue');
const SymCheck = () => import(/* webpackChunkName: "controls" */ './comp/SymCheck.vue');
const SymCombo = () => import(/* webpackChunkName: "controls" */ './comp/SymCombo.vue');
const SymMemo = () => import(/* webpackChunkName: "controls" */ './comp/SymMemo.vue');
const SymCard = () => import(/* webpackChunkName: "controls" */ './comp/SymCard.vue');
const SymCollapse = () => import(/* webpackChunkName: "controls" */ './comp/SymCollapse.vue');
const SymExceptions = () => import(/* webpackChunkName: "controls" */ './comp/SymExceptions.vue');
const SymForm = () => import(/* webpackChunkName: "controls" */ './comp/SymForm.vue');
const SymImageSelect = () => import(/* webpackChunkName: "controls" */ './comp/SymImageSelect.vue');
const SymInput = () => import(/* webpackChunkName: "controls" */ './comp/SymInput.vue');
const SymMonth = () => import(/* webpackChunkName: "controls" */ './comp/SymMonth.vue');
const SymPager = () => import(/* webpackChunkName: "controls" */ './comp/SymPager.vue');
const SymProgressBar = () => import(/* webpackChunkName: "controls" */ './comp/SymProgressBar.vue');
const SymRadio = () => import(/* webpackChunkName: "controls" */ './comp/SymRadio.vue');
const SymRating = () => import(/* webpackChunkName: "controls" */ './comp/SymRating.vue');
const SymSlideShow = () => import(/* webpackChunkName: "controls" */ './comp/SymSlideShow.vue');
const SymTab = () => import(/* webpackChunkName: "controls" */ './comp/SymTab.vue');
const SymTabs = () => import(/* webpackChunkName: "controls" */ './comp/SymTabs.vue');
const SymTag = () => import(/* webpackChunkName: "controls" */ './comp/SymTag.vue');
const SymTable = () => import(/* webpackChunkName: "controls" */ './comp/SymTable.vue');
const SymTableHeader = () => import(/* webpackChunkName: "controls" */ './comp/SymTableHeader.vue');
const SymTableBody = () => import(/* webpackChunkName: "controls" */ './comp/SymTableBody.vue');
const SymTableFooter = () => import(/* webpackChunkName: "controls" */ './comp/SymTableFooter.vue');
const SymUpload = () => import(/* webpackChunkName: "controls" */ './comp/SymUpload.vue');
const SymVideo = () => import(/* webpackChunkName: "media" */ './comp/SymVideo.vue');
const SymDailyMotion = () => import(/* webpackChunkName: "media" */ './comp/SymVideo-DailyMotion.vue');
const SymVimeo = () => import(/* webpackChunkName: "media" */ './comp/SymVideo-Vimeo.vue');
const SymYouTube = () => import(/* webpackChunkName: "media" */ './comp/SymVideo-YouTube.vue');

const HttpCodePage = () => import(/* webpackChunkName: "sys-pages" */ './comp/SymHttpCode.vue');
const Home = () => import(/* webpackChunkName: "sys-pages" */ './page/sys/Home.vue');
const Logon = () => import(/* webpackChunkName: "sys-pages" */ './page/sys/Logon.vue');
// const Menu = () => import(/* webpackChunkName: "sys-pages" */ './page/sys/Menu.vue');

Vue.use(Router);

const routes = [];
let router;

sym.userInfo.restore().then(
  () => {
    registerRoutes();
    app.registerRoutes(addRoute);
    registerExceptionRoutes();

    router = new Router({
      routes: routes,
      mode: 'history',
      scrollBehavior: core.scrollBehavior
    });

    registerComponents();
    registerDirectives();
    registerGuards();

    Vue.config.productionTip = false;

    core.init(router);

    let vue = new Vue({
      router: router,
      render: h => h(App)
    });

    sym.init(vue);
  }
);

function registerComponents () {
  rc('alert', Alert);
  rc('modal', Modal);
  rc('link', Link);
  rc('go-top', GoTop);
  rc('input-button', InputButton);
  rc('text', SymText);
  rc('time', SymTime);
  rc('date', SymDate);
  rc('int', SymInteger);
  rc('dec', SymDecimal);
  rc('dropdown', SymDropdown);
  rc('check', SymCheck);
  rc('combo', SymCombo);
  rc('memo', SymMemo);
  rc('card', SymCard);
  rc('collapse', SymCollapse);
  rc('exceptions', SymExceptions);
  rc('form', SymForm);
  rc('img-select', SymImageSelect);
  rc('input', SymInput);
  rc('month', SymMonth);
  rc('pager', SymPager);
  rc('progress-bar', SymProgressBar);
  rc('radio', SymRadio);
  rc('rating', SymRating);
  rc('slide-show', SymSlideShow);
  rc('tab', SymTab);
  rc('tabs', SymTabs);
  rc('tag', SymTag);
  rc('table', SymTable);
  rc('table-header', SymTableHeader);
  rc('table-body', SymTableBody);
  rc('table-footer', SymTableFooter);
  rc('upload', SymUpload);
  rc('video', SymVideo);
  rc('vimeo', SymVimeo);
  rc('youtube', SymYouTube);
  rc('dailymotion', SymDailyMotion);
  rc('http-code', HttpCodePage);
}

function rc (tag, comp) {
  Vue.component('sym-' + tag, comp);
}

function registerDirectives () {
  Vue.directive('focus', {
    inserted (el, binding) {
      if ( !binding.value ) return;
      if ( focusSelectors.some(selector => el.matches(selector)) ) {
        el.focus();
        return;
      }
      let child = el.querySelector(focusSelectors.join(','));
      if ( child ) { child.focus(); }
    }
  });
}

function addRoute (path, comp, name, isPublic) {
  let route = {
    path: path,
    component: comp,
    name: name ? name : undefined,
    meta: { locked: isPublic ? false : true }
  };

  routes.push(route);
  return route;
}

function registerGuards () {

  router.beforeEach( (to, from, next) => {
    if (to.meta.locked && !sym.userInfo.isAuthenticated) {
      next({ name: 'logon', query: { redirect: to.fullPath }});
      return;
    }
    if (to.meta.locked && to.name !== 'menu') {

      let isAccessible = sym.hasPageAccess(to.name.replace('-base', ''));
      if (!isAccessible) {
        advice.fault(to.path.substring(1), { title: 'You have no access to requested resource.', duration: 5, dismissible: true });
        if (from.path && from.name !== 'logon') {
          next(false);
        } else {
          // next({ name: 'home' });
          next({ path: '/menu' });  // 01 Mar 2025 - EMT (bug fix)
        }
      } else {
        // next();
        // 01 Mar 2025 - EMT (security)
        if (to.path === from.path) {
          next();
          return;
        }
        isPageAllowed(to.name).then(
          reply => {
            if (reply === 'yes') {
              next();
              return;
            }

            if (reply === 'no' && from.name === 'logon') {
              next({ path: '/menu' });
              return;
            }

            next(false);
          }
        );
      }
    } else {
      next();
    }
  });

  router.beforeResolve( (to, from, next) => {
    if (to.name) {
      loader.start();
    }
    next();
  });

  router.afterEach( () => {
    loader.done();
  });
}

function registerRoutes () {
  addRoute('/', Home, 'home', true);
  addRoute('/logon', Logon, 'logon', true);
  // addRoute('/menu', Menu, 'menu');
}

function registerExceptionRoutes () {
  addRoute('/exception/:httpCode', HttpCodePage, 'fault', true);

  routes.push(
    {
    path: '*',
    redirect: {
      name: 'fault',
      params: { httpCode: 404 },
      query: { statusText: 'Resource not found', pathName: window.location.href }
    },
    name: 'not-found',
    meta: { locked: false }
    }
  );
}

function isPageAllowed (pageId) {

  let o = sym.userInfo.pages.find(obj => obj.pageId === pageId);

  if (!o) {
    advice.fault('Undefined page identifier (' + pageId + ')', { title: 'Security Note'});
    return Promise.resolve('cancel');
  }

  if (!o.password) {
    return Promise.resolve('yes');
  }

  let
    message = '<b>' + o.pageName + '</b><br><br>The requested page is restricted.<br>Enter the password to continue.' ,
    options = { title: 'Restricted Page', scheme: 'warning' };

  return security.pass(message, o.password, options);

}