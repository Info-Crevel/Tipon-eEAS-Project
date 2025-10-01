<template>

<main role="app-wrapper" class="main-wrapper">

  <input type="checkbox" id="toggle" @change="tsMenuIcon+=1" ref="menu-toggle">

  <!-- <div class="content"> -->

    <nav id="navbar" class="nav-bar collapse-flex show justify-evenly">
        <!-- <nav id="navbar" class="nav-bar"> -->

      <div class="nav-group">
        <a href="/" class="home-leftXXX button app-orange outline border-0"><i class="fa fa-home fa-lg mr-1"></i> Home</a>
      </div>

        <sym-dropdown class="dropdown" @input="onUserInfoDropDown">
          <!-- <button type="button" class="drop-toggle app-orange outline border-0" slot="toggle" v-show="sym.userInfo.isAuthenticated"><i class="fa fa-lg fa-user-circle"></i><span class="pl-2 wide-screen">{{ sym.userInfo.userShortName }}</span></button> -->
          <div class="dropbox shadow-dark beige text-center align-right" :key="tsUser">

        <sym-card id="database" class="beige shadow border-main"> <!--Others-->
          <div slot="header">
            <span class="menu-item bold serif lg-3 mt-10">OTHERS</span>
          </div>
          <ul class="menu-item serifXXX">
            <li><sym-link :class="listItemClass('dbs0290')" to="dbs0290" title="Bank Definition">Bank Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0300')" to="dbs0300" title="Bank Locations">Bank Location Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0310')" to="dbs0310" title="Member Transaction Types">Member Transaction Type Setup</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0320')" to="dbs0320" title="Expense Charging">Expense Charging Definition</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0330')" to="dbs0330" title="Inventory Release Charging">Inventory Release Charging</sym-link></li>
            <li><sym-link :class="listItemClass('dbs0910')" to="dbs0910" title="Work Schedules">Work Schedules</sym-link></li>
          </ul>
        </sym-card>
            
          </div>
        </sym-dropdown>



      <div class="org-header text-center">
        <h5 >{{this.sym.userInfo.userOrgShortName }}</h5>
      </div>
      <!-- <div id="app-menu" class="menu-holder">
        <input class="submenus" id="submenu1" type="checkbox">
        <input class="submenus" id="submenu2" type="checkbox">
        <input class="submenus" id="submenu3" type="checkbox">
        <input class="submenus" id="submenu4" type="checkbox">
        <input class="submenus" id="submenu5" type="checkbox">
        <input class="submenus" id="submenu6" type="checkbox">
        <input class="submenus" id="submenu7" type="checkbox">
        <input class="submenus" id="submenu8" type="checkbox">
        <input class="submenus" id="submenu9" type="checkbox">
        <input class="submenus" id="submenu10" type="checkbox">
        <input class="submenus" id="submenu11" type="checkbox">

        <input class="submenus" id="sub-gov" type="checkbox">
        <input class="submenus" id="sub-gov-exec" type="checkbox">
        <input class="submenus" id="sub-gov-leg" type="checkbox">

        <input class="submenus" id="sub-disc" type="checkbox">
        <input class="submenus" id="sub-about" type="checkbox">

        <div id="navmenu" class="nav-menu" :key="ts" @transitionend="refreshMenu">

          <ul class="nav-menu-content">
            <li class="menu-close">
              <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
            </li>

            <li class="home">
              <a href="/"><i class="fa fa-home fa-lg mr-1"></i> Home</a>
            </li>

            <li>
              <a class="hassub" @mousedown.prevent>
                <label for="sub-gov" class="d-flex justify-between w-100" aria-haspopup="true" aria-controls="a1">
                  <span>Government</span>
                  <i class="fa ml-2 drill-icon" :class="getDrilldownIcon()"></i>
                </label>
              </a>

              <ul id="a1">
                <li class="menu-close">
                  <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
                </li>

                <li class="back">
                  <span>Government</span>
                  <label for="sub-gov">
                    <i class="fa fa-caret-left fa-2x"></i>
                  </label>
                </li>

                <li class="nav-link"><sym-link to="/offices">Offices / Departments</sym-link></li>

                <li>
                  <a class="hassub" @mousedown.prevent>
                    <label for="sub-gov-exec" class="d-flex justify-between w-100" aria-haspopup="true" aria-controls="a1-1">
                      <span>Executive</span>
                      <i class="fa ml-2 drill-icon" :class="getDrilldownIcon('R')"></i>
                    </label>
                  </a>

                  <ul id="a1-1">
                    <li class="menu-close">
                      <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
                    </li>

                    <li class="back">
                      <span>Executive</span>
                      <label for="sub-gov-exec">
                        <i class="fa fa-caret-left fa-2x"></i>
                      </label>
                    </li>

                    <li class="nav-link"><sym-link to="/profile-mayor">Mayor's Profile</sym-link></li>
                    <li class="nav-link"><sym-link to="/executive-orders">Executive Orders</sym-link></li>

                  </ul>

                </li>

                <li>
                  <a class="hassub" @mousedown.prevent>
                    <label for="sub-gov-leg" class="d-flex justify-between w-100" aria-haspopup="true" aria-controls="a1-2">
                      <span>Legislative</span>
                      <i class="fa ml-2 drill-icon" :class="getDrilldownIcon('R')"></i>
                    </label>
                  </a>

                  <ul id="a1-2">
                    <li class="menu-close">
                      <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
                    </li>

                    <li class="back">
                      <span>Legislative</span>
                      <label for="sub-gov-leg">
                        <i class="fa fa-caret-left fa-2x"></i>
                      </label>
                    </li>

                    <li class="nav-link"><sym-link to="/committees">Committees</sym-link></li>
                    <li class="nav-link"><sym-link to="/ordinances">Ordinances</sym-link></li>

                  </ul>

                </li>

                <li class="nav-link"><sym-link to="/officials">Officials</sym-link></li>

              </ul>
            </li>

            <li>
              <a class="hassub" @mousedown.prevent>
                <label for="sub-disc" class="d-flex justify-between w-100" aria-haspopup="true" aria-controls="a2">
                  <span>Discover Atimonan</span>
                  <i class="fa ml-2 drill-icon" :class="getDrilldownIcon()"></i>
                </label>
              </a>

              <ul id="a2">
                <li class="menu-close">
                  <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
                </li>

                <li class="back">
                  <span>Discover Atimonan</span>
                  <label for="sub-disc">
                    <i class="fa fa-caret-left fa-2x"></i>
                  </label>
                </li>

                <li class="nav-link"><sym-link to="/directions">Getting To Atimonan</sym-link></li>
                <li class="nav-link"><sym-link to="/heritage-sites">Heritage Sites</sym-link></li>
                <li class="nav-link"><sym-link to="/eco-tourism-sites">Eco-Tourism Sites</sym-link></li>
                <li class="nav-link"><sym-link to="/events">Events / Festivals</sym-link></li>
                <li class="nav-link"><sym-link to="/dances">Original Dances</sym-link></li>
                <li class="nav-link"><sym-link to="/products">Local Products</sym-link></li>

              </ul>
            </li>

            <li>
              <a class="hassub" @mousedown.prevent>
                <label for="sub-about" class="d-flex justify-between w-100" aria-haspopup="true" aria-controls="a3">
                  <span>About Us</span>
                  <i class="fa ml-2 drill-icon" :class="getDrilldownIcon()"></i>
                </label>
              </a>

              <ul id="a3">
                <li class="menu-close">
                  <a><label for="toggle" class="d-flex align-items-center justify-end" @click="tsMenuIcon+=1"><i class="fa fa-close mr-2 display-7 -mt-1"></i><span class="my-auto mb-0">Close</span></label></a>
                </li>

                <li class="back">
                  <span>About Us</span>
                  <label for="sub-about">
                    <i class="fa fa-caret-left fa-2x"></i>
                  </label>
                </li>

                <li class="nav-link"><sym-link to="/profile-lgu">Profile</sym-link></li>
                <li class="nav-link"><sym-link to="/history">History</sym-link></li>
                <li class="nav-link"><sym-link to="/vmseal">Vision / Mission / Seal</sym-link></li>
                <li class="nav-link"><sym-link to="/geo-features">Geographical Features</sym-link></li>
                <li class="nav-link"><sym-link to="/mayors">Mayors</sym-link></li>
                <li class="nav-link"><sym-link to="/barangays">Barangays</sym-link></li>

              </ul>
            </li>

            <li class="nav-link"><sym-link to="/news-archive">News</sym-link></li>

          </ul>

        </div> -->

      <!-- end menu-holder -->
      <!-- </div> -->

      <div class="nav-group">

        <button type="button" class="app-orange outline border-0" @click="openPortal" v-show="sym.userInfo.isAuthenticated" title="eEAS Menu"><i class="fa fa-lg fa-street-view"></i><span class="pl-1 wide-screen">Portal</span></button>

        <sym-dropdown class="dropdown" @input="onUserInfoDropDown">
          <button type="button" class="drop-toggle app-orange outline border-0" slot="toggle" v-show="sym.userInfo.isAuthenticated"><i class="fa fa-lg fa-user-circle"></i><span class="pl-2 wide-screen">{{ sym.userInfo.userShortName }}</span></button>
          <div class="dropbox shadow-dark beige text-center align-right" :key="tsUser">
            <span class="dropitem-text pt-1 text-narrow">{{ sym.userInfo.userName }}</span>
            <img :src="dom.getImageSource(sym.userInfo.photo)" alt="..." class="photo d-block mx-auto my-2 fw-28 circle border-main square-100" v-if="sym.userInfo.photo">
            <hr class="my-1">
            <button type="button" class="beige text-info border-light outline p-0 px-2 sm-1" tabindex="-1" @click.prevent="showPasswordRequestDialog">Change Password?</button>
            <hr class="my-1">
            <button type="button" class="beige text-info border-light outline p-0 px-2 sm-1" tabindex="-1" @click.prevent="showOrgDialog" >Change Cooperative?</button>
            <hr class="my-1">
            <button type="button" class="dropitem curved sm-1" @click="signout"><i class="fa fa-lg fa-sign-out mr-2"></i>Logout</button>
          </div>
        </sym-dropdown>

        <!-- <sym-link class="logon-link mx-1" to='/logon' v-show="!sym.userInfo.isAuthenticated"><i class="fa fa-lg fa-sign-in pr-2"></i>Login</sym-link> -->
        <sym-link class="logon-link mx-1 button app-orange outline border-0" to='/logon' v-show="!sym.userInfo.isAuthenticated"><i class="fa fa-lg fa-sign-in pr-2"></i>Login</sym-link>
        <sym-go-top transition="" :isTool="true" buttonClass="btn-go-top border-0 h-100"></sym-go-top>
        <!-- 04 Feb 2025 - EMT (removed) -->
        <!-- <button id="app-menu-trigger" type="button" class="burger app-orange outline border-0 pl-2 h-100 ml-1"><label for="toggle" class="mb-0 d-flex" @click="tsMenuIcon+=1"><i class="fa fa-fw mr-1 display-7" :class="getToggleIconClass()" :key="tsMenuIcon"></i><span class="my-auto">Menu</span></label></button> -->

      </div>

    </nav>

    <!-- Banner -->

    <section id="main-header" class="banner app-blue-light pos-relative shadow p-1">
      <!-- <div class="media">
        <a href="/" title="Home">
          <img class="logo-lgu" src="img/logo/lgu.png" width="100" alt="Official Seal of Atimonan, Quezon">
        </a>
        <div class="media-body banner-info">
          <div class="text-charcoal">
            <p class="phils bold mb-0">Republic of the Philippines</p>
            <p class="lgu bold mb-0">ATIMONAN <small>Quezon</small></p>
            <p class="motto wide bold mb-0"><small><span class="motto-init">T</span>ulong-tulong sa asenso, <span class="motto-init">M</span>amamayan ang panalo!</small></p>
            <p class="motto narrow bold mb-0"><small><span class="motto-init">T</span>ulong-tulong sa asenso, <span class="motto-init"><br>M</span>amamayan ang panalo!</small></p>
          </div>
        </div>
      </div> -->

      <!-- <div class="banner-right"> -->
        <!-- Philippine Standard Time -->
      <!-- </div> -->

    </section>

    <div id="main-content" class="light">
      <transition name="fade" mode="out-in" appear>
        <router-view>
        </router-view>
      </transition>
    </div>

  <!-- end content -->
  <!-- </div> -->

  <!-- <section id="main-footer" class="lgu-footer-links lgu-orange justify-center text-shadow">
    <div>
      <h5>LGU Atimonan</h5>
      <p>Atimonan Municipal Building</p>
      <p>Quezon St. corner Rizal St.</p>
      <p>Brgy. Zone III Poblacion</p>
      <p>Atimonan, Quezon</p>
      <br>
      <p><i class="fa fa-phone fa-fw mr-1"></i> <span class="lg-2">(042) 316-5385</span></p>
      <p><i class="fa fa-envelope-o fa-fw mr-1"></i>  <a href="mailto:lguatimonan2016@gmail.com">lguatimonan2016@gmail.com</a> </p>
      <p><i class="fa fa-facebook-official fa-fw mr-1"></i>  <a href="https://facebook.com/PRIOAtimonan" target="_blank">PRIO Atimonan</a> </p>
    </div>

    <div class="lgu-hotlines">

      <div>
        <h5>Hotlines</h5>
        <p>Coast Guard</p>
        <p class="lg-2 mb-2">0999 451-9595</p>
        <p>Philippine National Police</p>
        <p class="lg-2 mb-2">(042) 717-1153</p>
        <p>Bureau of Fire Protection</p>
        <p class="lg-2">0933 437-7327</p>
      </div>

      <div>
        <p class="lgu-hotline-spacer-mps"></p>
        <p>Atimonan MPS (PNP)</p>
        <p class="lg-2 mb-0">Hotline: 166</p>
        <p class="lg-2 mb-2">0909 931-0955</p>
        <p>MDRRMO Atimonan</p>
        <p class="lg-2 mb-0">(042) 316-6525</p>
        <p class="lg-2">0908 983-2111</p>
      </div>

      <div>
        <p class="lgu-hotline-spacer-bfp"></p>
        <p>Atimonan BFP</p>
        <p class="lg-2 mb-0">Hotline: 165</p>
        <p class="lg-2 mb-0">(042) 785-6155</p>
        <p class="lg-2">0933 437-7327</p>
      </div>
    </div>

    <div>
      <h5>Local Governance</h5>
      <p class="nav-link"><sym-link to="/news-archive">Discover Atimonan</sym-link></p>
      <p class="nav-link"><sym-link to="/offices">Offices / Services</sym-link></p>
      <p class="nav-link"><sym-link to="/news-archive">Transparency</sym-link></p>
      <p class="nav-link"><sym-link to="/news-archive">News</sym-link></p>
    </div>

  </section> -->

  <!-- <sym-go-top id="gotop" buttonClass="go-top lgu-blue justify-center circle float-right shadow border-white op-80">
  </sym-go-top> -->

  <div class="d-flex justify-end light">
    <sym-go-top id="gotop" buttonClass="go-top app-blue circle shadow border-white op-70">
    </sym-go-top>
  </div>

  <footer id="app-footer" class="app-blue-dark p-2">
    <p class="text-light mb-0 sm-1 text-center">© <span>{{ currentYear }}</span> TIPON. All Rights Reserved.</p>
  </footer>

  <auto-logout @timeout="onAutoLogout"></auto-logout>

  <sym-modal
    v-model="isAutoLogoutModalVisible"
    size="md"
    :header="true"
    :customBody="true"
    :footer="false"
    :keyboard="true"
    :dismissible="true"
    title="Session Expired"
    v-if="isAutoLogoutModalVisible"
    headerClass="info"
    dismissButtonClass="warning darken-3"
  >
    <div class="p-2">
      <div class="board box-border text-center beige mb-0">
        <h3 class="text-charcoal mb-3">You've Been<br>Logged Out</h3>
        <p class="lg-1">You were gone for too long</p>
        <button type="button" class="text-center w-100 info lg-3 p-1" @click="goLogonPage" autofocus>Log In</button>
      </div>
    </div>

  </sym-modal>

<!-- Change Password -->
<sym-modal
    v-model="isChangePasswordRequestVisible"
    size="sm"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isChangePasswordRequestVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <button class="close" @click="hideChangePasswordRequestDialog()"><i class="fa fa-times"></i></button>
      <div class="curved border-dark arcticblue curved p-3 text-centerXXX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-key fa-lg mr-2"></i>Change Password</p>
        </div>

        <div class="whitesmoke border-main p-2 curved">
          <!-- <p class="lg-1"><span class="text-crimson bold"></span></p> -->
          <!-- <sym-text plain align="bottom" v-model="accessId" :caption="app.setup.logonIdCue" captionClass="bold" :modeControl="false"></sym-text> -->

          <!-- <sym-text plain align="bottom" v-model="accessId" caption="Login ID" captionClass="bold" :modeControl="false"></sym-text> -->


          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="oldPassword" caption="Old Password" captionClass="bold"  :modeControl="false"></sym-text>

          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="newPassword" caption="New Password" captionClass="bold"  :modeControl="false" @changing="onNewPasswordChanging"></sym-text>

          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="confirmPassword" caption="Confirm Password" captionClass="bold" :modeControl="false" @changing="onNewPasswordChanging"></sym-text>

          <!-- <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button class="success text-center mb-1" :disabled="!oldPassword || !newPassword || !confirmPassword" @click="onResetPassword()"><i class="fa fa-play fa-lg mr-2"></i> Set New Password</button>
          </div> -->

          <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button type="submit" class="info" :disabled="!oldPassword || !newPassword || !confirmPassword" @click="onResetPassword()"><i class="fa fa-play fa-lg mr-2"></i> Set New Password</button>
          </div>




        </div>

        <!-- <button class="primary darken-2 text-center w-100 shadow-dark lg-1 mt-4" @click="hideChangePasswordRequestDialog()"><i class="fa fa-times fa-lg mr-2"></i>Close</button> -->

      </div>
    </div>
</sym-modal>
<!-- Change Org -->
<sym-modal
    v-model="isChangeOrgVisible"
    size="md"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isChangeOrgVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <button class="close" @click="hideChangeOrgDialog()"><i class="fa fa-times"></i></button>
      <div class="curved border-dark arcticblue curved p-3 text-centerXXX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-users fa-lg mr-2"></i>Change Multipurpose Cooperative</p>
        </div>

        <div class="whitesmoke border-main p-2 curved">
         
          <sym-combo v-model="orgId" caption="Name" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
           
         <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button type="submit" class="info" @click="onSubmitOrg()"><i class="fa fa-play fa-lg mr-2"></i> Submit</button>
          </div>

     

        </div>

        <!-- <button class="primary darken-2 text-center w-100 shadow-dark lg-1 mt-4" @click="hideChangePasswordRequestDialog()"><i class="fa fa-times fa-lg mr-2"></i>Close</button> -->

      </div>
    </div>
</sym-modal>
</main>


</template>

<script>

import
  AutoLogout
from './comp/AutoLogout.vue';

import {
  logoff
} from './js/dbSys';

import {
  get,
  ajax
} from './js/http';

import
  PageBase
from './page/PageBase.vue';


export default {
  extends: PageBase,
  components: {
    'auto-logout': AutoLogout
  },

  provide () {
    return {
      theme: this.core.config.theme
    };
  },

  data () {
    return {
      ts: 0,
      tsUser: 0,
      tsMenuIcon: 0,
      dateInfo: this.sym.dateInfo,
      userCode: '',
      currentYear: 2022,
      drilldownIconclass: 'fa-caret-down',
      // matchMediaQuery: '(max-width: 716px)'
      matchMediaQuery: '(max-width: 1000px)',
      isAutoLogoutModalVisible: false,
      isChangePasswordRequestVisible: false,
      oldPassword: '',
      accessId: '',
      phoneNumberEndDigits: '',
      newPassword: '',
      confirmPassword: '',
      isChangeOrgVisible: false,
      
      orgId: 0,
      orgShortName: '',
      // userOrgShortName:'',
      orgs: [],
    };
  },

  methods: {

    canAccessPage (pageId) {
      if (!pageId) {
        return false;
      }
      return true;
    },

    listItemClass ( pageId ) {
      return {
        'list-item': true,
        'disabled': !(this.sym.hasPageAccess(pageId) && this.canAccessPage(pageId))
      }
    },

    onSubmitOrg() {
      const me = this;
      
      // me.sym.userInfo.userOrgId = me.orgId;
      // me.sym.userInfo.userOrgShortName = me.orgShortName;
      // me.userOrgShortName = me.orgShortName.toUpperCase();
      me.sym.userInfo.userOrgId = me.orgId;
      let o = me.orgs.find( o => o.orgId == me.orgId);
      if (o) {
        me.sym.userInfo.userOrgShortName = o.orgShortName
      }

      me.updateUserOrg()
      .then(() => {
        me.isChangeOrgVisible = false;
      })


    },


    onNewPasswordChanging(e) {
      e.callback = this.newPasswordCallback;
    },

    newPasswordCallback(e) {
      // const me = this;

      e.message ="";
      if (e.proposedValue < 8) {
      e.message = "Password must be at least 8 characters long.";
      return false;
      }

      if (!/[A-Z]/.test(e.proposedValue)) {
        e.message = "Password must contain at least one uppercase letter.";
        return false;
      }

      if (!/\d/.test(e.proposedValue)) {
        e.message = "Password must contain at least one number.";
        return false;
      }

    return true;

    },

    isValidAccessPassword () {
      if (this.accessId < 0) {
        return Promise.resolve(null);
      }
      return get('api/passwords/validate?accessId=' + this.accessId + '&password=' + this.oldPassword);
    },

    updateUserOrg () {
         const
         me = this,
        options = this.core.getAjaxOptions('PUT');
        return ajax('api/user-org/'+ me.sym.userInfo.token + '/'+ me.orgId, options);
    },

    onResetPassword () {
      const me = this;

      me.isValidAccessPassword().then(

      result => {

        if (result===0) {
          me.advice.fault("Old Password does not match.", { duration: 5 });
          return;
        }


      // if (!me.oldPassword) {
      //   me.advice.fault("Old Password", { title: me.core.config.msgEntryRequired, enclosed: true });
      //   // me.setFocus('oldPassword');
      //   // return;
      // }


      // if (me.password !== me.oldPassword) {
      //   me.advice.fault("Old password dooes not match!");
      //   me.setFocus('oldPassword');
      //   return;
      // }

      if (me.oldPassword == me.newPassword) {
        me.advice.fault("You cannot use your old password as the new one!");
        me.setFocus('newPassword');
        return;
      }

      if (me.newPassword !== me.confirmPassword) {
        me.advice.fault("New and confirm password does not match!");
        me.setFocus('newPassword');
        return;
    }

      if (!me.newPassword) {
        me.advice.fault("New Password", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('newPassword');
        return;
      }

      let wait = me.wait();

      me.resetNewPassword().then(
        success => {
          me.stopWait(wait);
          if (success) {
            me.dialog.success("Password changed.<br><br>You may try logging on now to check.", { size: 'sm'}).then(
              () => {
                me.isChangePasswordRequestVisible = false;
                me.signout()
                me.$nextTick( () => {
                  setTimeout(() => {
                    me.setFocus('logonId');
                  }, 200);
                });
              }
            );
          } else {
            me.dialog.fault("Attempt to set new password failed.<br><br>Make sure you have correct credentials.", { size: 'sm'});
          }

        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
      }
    );



    },

    resetNewPassword () {
      const
        me = this;

      let payload = {
        accessId: me.accessId,
        oldPassword: me.oldPassword,
        newPassword: me.newPassword
      };

      const
        body = JSON.stringify(payload),
        options = me.core.getAjaxOptions('PUT', body);
      return ajax('api/passwords/reset-default', options);
    },

    hideChangePasswordRequestDialog () {
      const me = this;
      me.isChangePasswordRequestVisible = false;
    },

    hideChangeOrgDialog () {
      const me = this;
      me.isChangeOrgVisible = false;
    },


    getInputKeyType () {
      return this.showPassword ? 'text' : 'password';
    },

    showPasswordRequestDialog () {
      const me = this;
      me.accessId = me.sym.userInfo.logOnId;
      me.oldPassword = '';
      me.newPassword = '';
      me.confirmPassword = '';
      me.isChangePasswordRequestVisible = true;
    },

    showOrgDialog () {
      const me = this;
      me.orgs = me.sym.userInfo.orgs;
      me.orgId = me.sym.userInfo.userOrgId;
      me.isChangeOrgVisible = true;
    },


    signout () {
      const
        me = this,
        wait = me.dialog.wait(),
        userName = me.sym.userInfo.userName;

      logoff().then(
        () => {
          me.dialog.stop(wait);
          me.sym.userInfo.update();
          me.core.navigate('home', true);
          me.advice.success(userName, { title: 'Logout complete', enclosed: true, alignment: 'bottom-right', duration: 4, dismissible: true });
          // me.sym.userInfo.update();
          // me.accountTypeId = 0;

          me.sym.userInfo.userOrgId = 0;
          me.sym.userInfo.userOrgShortName = '';
          me.userCode = '';
          me.refreshUserTag();
          window.scrollToTop();
          me.eventBus.$emit('user:logoff');
          me.tsUser = Date.now();
        },
        () => {
          me.dialog.stop(wait);
        }
      );
    },

    openPortal () {
      this.core.navigate('/menu');
    },

    onUserInfoDropDown (show) {
      if (show) {
        this.tsUser = Date.now();
      }
    },

    refreshUserTag () {
      this.ts +=1;
    },

    onResize () {
      this.ts = this.ts + 1;
    },

    getDrilldownIcon (direction) {
      if (direction) {
        if (direction === 'R') { return 'fa-chevron-right'; }
        if (direction === 'L') { return 'fa-chevron-left'; }
      }

      const mql = window.matchMedia(this.matchMediaQuery);
      if (mql.matches) {
        return 'fa-chevron-right';
      } else {
        return 'fa-chevron-down';
      }
    },

    getToggleIconClass () {
      let el = document.getElementById("toggle");
      if (el && el.checked  ) {
        return 'fa-close';
      }
      return 'fa-bars';
    },

    refreshMenu () {
      const mql = window.matchMedia(this.matchMediaQuery);
      if (mql.matches) {
        this.ts +=1;
      }
    },

    closeMenu () {
      const
        me = this,
        mql = window.matchMedia(me.matchMediaQuery);

      if (mql.matches) {
        setTimeout(() => {
          me.$refs['menu-toggle' ].checked = false;
          me.tsMenuIcon += 1;
        }, 30);

      } else {
        let menus = document.querySelectorAll('.nav-menu ul ul');
        menus.forEach(el => {
          el.classList.add('d-none');
          setTimeout(() => {
            el.classList.remove('d-none');
          }, 100);
        });
      }

    },

    addNavLinkListener () {
      const navLinks = document.querySelectorAll(".nav-link");
      navLinks.forEach(el => el.addEventListener("click", this.closeMenu));
    },

    removeNavLinkListener () {
      const navLinks = document.querySelectorAll(".nav-link");
      navLinks.forEach(el => el.removeEventListener("click", this.closeMenu));
    },

    onAutoLogout () {
      const me = this;
      me.signout();
      me.isAutoLogoutModalVisible = true;
    },

    goLogonPage () {
      const me = this;

      me.core.navigate('logon');
      me.isAutoLogoutModalVisible = false;
    }



  },

  created () {
    const me = this;
    // me.orgs = me.sym.userInfo.orgs;
    me.dom.on(window, 'resize', me.onResize);
    // me.eventBus.$on('user:changed', () => {
    //   me.accountTypeId = me.sym.userInfo.accountTypeId;
    //   me.refreshUserTag();
    // });
    me.accessId = me.sym.userInfo.logOnId;

    me.eventBus.$on('user:logon', () => {
      me.userCode = me.sym.userInfo.userCode;
      me.refreshUserTag();
    });
  },

  updated () {
    this.removeNavLinkListener();
    this.addNavLinkListener();
  },

  beforeDestroy () {
    // this.dom.showMenu();
    this.dom.off(window, 'resize', this.onResize);
    this.removeNavLinkListener();
  },

  mounted () {
    const me = this;

    me.currentYear = me.core.getCurrentYear();
    me.userOrgShortName = me.sym.userInfo.userOrgShortName;
    // me.accountTypeId = me.sym.userInfo.accountTypeId;
    me.userCode = me.sym.userInfo.userCode;
    me.refreshUserTag();

    if (me.sym.userInfo.isAuthenticated) {
      me.eventBus.$emit('user:logon');
    }

    me.$nextTick( () => {
      me.addNavLinkListener();
    });

  }

};

</script>

<style>

/* .nav-bar {
  height: 45px;
} */

</style>

<style scoped>

button.btn-go-top {
  background: transparent;
}

.banner {
  padding: .5rem;

  display: grid;
  grid-template-columns: 1fr 280px;
  gap: 1rem;
}

.banner-info {
  display: grid;
  grid-template-columns: 400px 1fr;
}

.logo-lgu {
  width: 100px;
  margin-right: .75rem;
  opacity: .85;
  display: block;
}

.phils {
  font-size: 1.0625rem;
  line-height: 1.6;
  border-bottom: 1px solid #36454f;
}

.lgu {
  font-size: 2.125rem;
  color: black;
  opacity: 0.75;
  margin-top: .25rem;
  line-height: 1.2;
}

.motto {
  font-size: 1.25rem;
  margin-top: -.25rem;
}

.motto-init {
  font-size: 1.25rem;
}

.motto.narrow {
  display: none;
}

#gotop {
  padding-top: .625rem;
  padding-bottom: .625rem;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}

.fade-enter, .fade-leave-to {
  opacity: 0;
}

#navbar {
  position: sticky;
  background: var(--app-blue);

  z-index: 1000;
  top: 0;

  padding: .25rem;
  height: 45px;
}

#navbar >>> .photo {
  object-fit: cover;
  /* height: 7.125rem !important;
  max-height: 7.125rem !important; */
}

/* .home-left {
  font-size: 1.0625rem;
  text-decoration: none;
  display: none;
} */

.burger {
  display: none;
  align-items: center;
}

.logon-link {
  color: whitesmoke;
  text-decoration: none;
}
.org-header{
  margin-right: 150px;
  padding: 10px;
  text-transform: uppercase;
  /* border: 1px solid white; */
}
/* .nav-bar{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
} */
@media (max-width: 834px) {
  .banner-info {
    grid-template-columns: 390px 1fr;
  }
}

@media (max-width: 768px) {
  .banner-right {
    display: none;
  }

  .logo-lgu {
    width: 86px;
    margin-right: .625rem;
  }

  .phils {
    font-size: 1.0625rem;
    line-height: 1.4;
  }

  .lgu {
    font-size: 2rem;
    margin-top: .0625rem;
    line-height: 1.1;
  }

  .motto {
    font-size: 1.25rem;
    line-height: 1.1;
  }

  .motto-init {
    font-size: 1.25rem;
  }
}

@media (max-width: 500px) {
  .logo-lgu {
    width: 70px;
    margin-right: .5rem;
  }

  .phils {
    display: none;
  }

  .banner-info {
    grid-template-columns: 210px 1fr;
  }

  .lgu {
    font-size: 1.625rem;
    margin-top: -.125rem;
    line-height: 1.25;
  }

  .motto.wide {
    display: none;
  }

  .motto.narrow {
    display: block;
    line-height: .9;
  }
}

@media (max-width: 320px) {
  .btn-go-top {
    display: none;
  }
}

.small-screen {
  display: none;
}

input.submenus,
input#toggle {
  position: absolute;
  display: none;
}

.nav-menu ul {
  margin: 0;
  padding: 0;
  list-style: none;
  white-space: nowrap;
  text-align: left;
}

/* #navbar {
  position: sticky;
  background: var(--app-blue);

  z-index: 1000;
  top: 0;
} */

.menu-holder {
  display: flex;
  align-items: center;
  position: sticky;
}

.nav-menu {
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  -o-user-select: none;
  user-select: none;
  -ms-touch-action: none;
  touch-action: none;
}

.nav-menu li {
  display: block;
  position: relative;
}

.nav-menu ul ul {
  position: absolute;
  left: -9999px;
  padding: 10px 0;
  z-index: 0;
  border-radius: 0 0 5px 5px;
  border: 1px solid var(--app-blue-dark);

  -webkit-transition: opacity .2s ease-in-out;
  -o-transition: opacity .2s ease-in-outs;
  -moz-transition: opacity .2s ease-in-out;
  transition: opacity .2s ease-in-out;

}

.nav-menu ul ul ul {left:100%; top:auto; margin-top:-51px; border-radius:5px;}

.nav-menu ul li.left > ul {left:auto; right:0; top:41px;}

.nav-menu ul li.left ul li > ul {left:auto; right:100%; top:auto; margin-top:-51px;}

.nav-menu > ul {margin:0 auto;}
.nav-menu > ul > li {float:left; position:relative;}

.nav-menu ul {
  background: var(--app-blue);
}

.nav-menu a {
  display: block;
  color: whitesmoke !important;
  text-decoration: none;
  padding: 0 20px;
  position: relative;
  z-index: 10;
  line-height: 2.25rem;
  font-size: 17px;
}

.nav-menu > ul > li > label {
  display:block;
}

.nav-menu > ul > li > a > label {
  margin-bottom: 0;
}

.nav-menu ul > li > a > label:hover {
  cursor: pointer;
}

.nav-menu ul ul > li > label {
  display:block;
  position:relative;
  margin-top:-25px;
  width:100%;
  height:25px;
  z-index:100;

  -webkit-transition: 0s .3s;
  -o-transition: 0s .3s;
  -moz-transition: 0s .3s;
  transition: 0s .3s;
}

.nav-menu ul li.back {display:none;}

.nav-menu li:hover > label {width:0;}

.nav-menu li.left ul a {text-align:right; padding:0 20px;}

.nav-menu li:hover > a,
.nav-menu ul li:hover > a {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

.nav-menu ul li:hover > a.hassub {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

.nav-menu li.left ul li:hover > a.hassub {
  color: whitesmoke;
  background-color: var(--app-orange);
  text-decoration: none;
}

.nav-menu > ul {*display:inline;}

.nav-menu ul ul,
.nav-menu ul ul ul,
.nav-menu ul li.left > ul,
.nav-menu ul li.left ul li > ul {left:-9999px; right:auto; opacity:0;}

.nav-menu ul li:hover > ul {left:0; opacity:1;}
.nav-menu ul ul li:hover > ul {left:100%; opacity:1;}
.nav-menu ul li.left:hover > ul {left:auto; right:0; opacity:1;}
.nav-menu ul li.left ul li:hover > ul {left:auto; right:100%; opacity:1;}

.drill-icon {
  margin-top: .5rem;
}

.nav-menu li.menu-close {
  display: none;
}

.home a {
  padding-left: 12px;
}

/* for tablets & smartphones */

/* @media only screen and (max-width: 716px) { */
@media only screen and (max-width: 1000px) {
  .burger {
    display: flex;
  }

  .small-screen {
    display: initial;
  }

  .wide-screen {
    display: none;
  }

  /* .home-left {
    display: flex;
  } */

  .nav-menu li.menu-close {
    display: block;
  }

  .menu-holder {
    position: fixed;
    left: -320px;
    width: 320px;
    top: 0;
    bottom: 0;
    height: auto;
    overflow-x: hidden;
    background: var(--app-blue-dark);
    z-index: 1010;

    -webkit-transition: left .3s;
    -o-transition: left .3s;
    -moz-transition: left .3s;
    transition: left .3s;
  }

  .nav-menu {
    width: 300px;
    text-align: center;
    position: absolute;
    z-index: 100;
    background: var(--app-blue);
    left: 0;
    top: 0;

    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    -o-user-select: none;
    user-select: none;
    -ms-touch-action: none;
    touch-action: none;

    -webkit-transition: .3s;
    -o-transition: .3s;
    -moz-transition: .3s;
    transition: .3s;
  }

  .nav-menu ul {
    margin: 0;
    padding: 0;
    width: 300px;
    list-style: none;
    text-align: left;
    border-radius: 0;
  }

  .nav-menu li {
    display: block;
    position: static;
    width: 300px;
    height: 45px;
    border-top: 1px solid var(--secondary);
    border-bottom: 1px solid #666;
    background: var(--app-blue);
  }

  .nav-menu ul li.back {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-right: 14px;
  }

  .nav-menu ul ul {
    position: absolute;
    left: 320px;
    top: -5000px;
    padding: 0;
    z-index: 0;
    opacity: 1;
    background: var(--app-blue);

    -webkit-transition: 0s .3s;
    -o-transition: 0s .3s;
    -moz-transition: 0s .3s;
    transition: 0s .3s;

  }

  .nav-menu ul ul ul {left:640px; top:-5000px; margin-top:0; opacity:1;}

  .nav-menu ul li.left > ul {left:280px; right:auto; top:-5000px;}
  .nav-menu ul li.left ul li > ul {left:280px; right:auto; top:-5000px; margin-top:0;}

  .nav-menu > ul {margin:0;}
  .nav-menu > ul > li {float:left; position:static;}

  .nav-menu a {
    display: block;
    font-family: 'Heebo', Arial, Helvetica, sans-serif;
    line-height: 3.125rem;
    color: whitesmoke;
  }

  .nav-menu > ul > li > a {line-height:45px;}

  .nav-menu a.hassub,
  .nav-menu li.menu-close a {
    padding: 0;
  }

  .nav-menu a.hassub label,
  .nav-menu li.menu-close a label {
    padding: 0 20px;
  }

  .nav-menu li.left ul a {
    text-align: left;
    padding: 0 20px;
  }

  .nav-menu ul li a:hover,
  .nav-menu ul li a.hassub:hover {
    color: whitesmoke;
    text-decoration: none;
  }

  .nav-menu ul li a.hassub,
  .nav-menu li.left ul li a.hassub,
  .nav-menu li ul li a.hassub {background:none;}

  .nav-menu ul li.back label {
    width: 46px;
    min-width: 46px;
    height: 40px;
    margin-top: 4px;
    border:1px solid whitesmoke;
    border-radius: 2px;
    float: right;
    z-index: 100;
    cursor: pointer;

    margin-right: 6px;
    padding: .125rem .875rem;
    color: whitesmoke;
    background: var(--app-orange-dark);
  }

  .nav-menu ul li.back span {
    font: normal 18px/18px 'Heebo Bold', arial, sans-serif;
    color: whitesmoke;
    padding: 0 12px;
    white-space: normal;
    width: 100%;
  }

  .nav-menu ul ul,
  .nav-menu ul ul ul,
  .nav-menu ul li.left > ul,
  .nav-menu ul li.left ul li > ul {left:320px; top:-5000px; right:auto; opacity:1;}


  .nav-menu ul li:hover > ul,
  .nav-menu ul ul li:hover > ul,
  .nav-menu ul li.left:hover > ul,
  .nav-menu ul li.left ul li:hover > ul {left:320px; right:auto; opacity:1;}

  .nav-menu ul li:hover > a.hassub,
  .nav-menu li.left ul li:hover > a.hassub {background:none; color:#333; text-decoration:none;}

  .nav-menu ul li a.hassub:hover,
  .nav-menu ul ul li a.hassub:hover,
  .nav-menu li.left ul li a.hassub:hover {
    color: whitesmoke;
    background: var(--app-orange);
    text-decoration: none;
  }

  #sub-gov:checked ~ .nav-menu #a1,
  #sub-gov-exec:checked ~ .nav-menu #a1-1,
  #sub-gov-leg:checked ~ .nav-menu #a1-2,
  #sub-disc:checked ~ .nav-menu #a2,
  #sub-about:checked ~ .nav-menu #a3,

  #submenu1:checked ~ .nav-menu #p1,
  #submenu2:checked ~ .nav-menu #p2,
  #submenu3:checked ~ .nav-menu #p3,
  #submenu4:checked ~ .nav-menu #p4,
  #submenu5:checked ~ .nav-menu #p5,
  #submenu6:checked ~ .nav-menu #p6,
  #submenu7:checked ~ .nav-menu #p7,
  #submenu8:checked ~ .nav-menu #p8,
  #submenu9:checked ~ .nav-menu #p9,
  #submenu10:checked ~ .nav-menu #p10,
  #submenu11:checked ~ .nav-menu #p11 {
    top:0;
    -webkit-transition: 0s;
    -o-transition: 0s;
    -moz-transition: 0s;
    transition: 0s;
  }


  #sub-gov:checked ~ .nav-menu,
  #sub-disc:checked ~ .nav-menu,
  #sub-about:checked ~ .nav-menu,

  #submenu1:checked ~ .nav-menu,
  #submenu2:checked ~ .nav-menu,
  #submenu7:checked ~ .nav-menu,
  #submenu8:checked ~ .nav-menu {
    left: -320px;
  }

  #sub-gov-exec:checked ~ .nav-menu,
  #sub-gov-leg:checked ~ .nav-menu,

  #submenu3:checked ~ .nav-menu,
  #submenu5:checked ~ .nav-menu,
  #submenu6:checked ~ .nav-menu,
  #submenu9:checked ~ .nav-menu,
  #submenu11:checked ~ .nav-menu {
    left: -640px;
  }

  #submenu4:checked ~ .nav-menu,
  #submenu10:checked ~ .nav-menu {
    left: -980px;
  }


  #toggle:checked ~ .content {left: 320px;}

  #toggle:checked ~ .content .menu-holder {
    left:0;
    box-shadow: 2px 4px 8px 4px rgba(64, 64, 64, .54);
  }

  .drill-icon {
    margin-top: 1rem;
  }
}

.main-wrapper {
  display: flex;
  flex-flow: column;
  height: 100%;
}

#main-headerXXX {
  height: 116px;
  /* flex: 0 1 auto;   */
}

#main-content {
  flex: 1 1 auto;
  /* flex-grow: 1;   */
}

#app-footer {
  height: 34px;
}

</style>
