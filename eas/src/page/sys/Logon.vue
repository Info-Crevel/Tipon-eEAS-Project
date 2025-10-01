<template>

<div class="d-flex">
  <div class="left" ref="left">
  </div>
  
  <div class="right d-flex" ref="right">
    <!-- <sym-form class="box-entry curved-2" bodyClass="aliceblue curved p-3"> -->

    <sym-form class="box-entry curved-2" bodyClass="arcticblue curved p-3">
      <p class="serif bold mb-1 display-6 text-center app-text-orange text-shadow">TEAM 1.1</p>
      <!-- <p class="serifXXX bold mb-3 display-8 text-center text-charcoal">{{ sym.sysInfo.siteName }}</p> -->
      <p class=" bold mb-3 display-9 text-center frs-text-blue">{{ sym.sysInfo.siteName }}</p>
      <div class="grid rows-2 mb-1">
        <sym-text v-model="logonId" bullet="user-o" :cue="app.setup.logonIdCue" v-focus="true" @keydown="onKeyDown" @rejected="logonIdRejected"></sym-text>
        <div class="password-cont">
          <sym-text v-model="password" bullet="key" :cue="app.setup.passwordCue" :type="getInputKeyType()" @keydown="onKeyDown" @rejected="passwordRejected"></sym-text>
          <sym-check v-model="showPassword" class="show-password text-info -mt-1" plain caption="Show Password" :captionWidth="32" :inputLeft="true"></sym-check>
          <button type="button" class="beige text-info border-light outline p-0 px-2 sm-1" tabindex="-1" @click.prevent="showPasswordRequestDialog">Forgot Password?</button>

        </div>

        <!-- <div class="password-cont">
          <button type="button" class="beige text-info border-light outline p-0 px-2 sm-1" tabindex="-1" @click.prevent="showPasswordRequestDialog">Forgot Password?</button>

        </div> -->

      </div>
      <div class="buttons float-right sm-1 mt-1X" shadow mb-0 curved-1>
        <button type="submit" class="info" @click.prevent="authenticate"><i class="fa fa-play mr-2"></i>Submit</button>
        <button type="button" class="warning" @click="goBack"><i class="fa fa-arrow-left mr-2"></i>Back</button>
      </div>
    </sym-form>

    <sym-modal
    v-model="isPasswordRequestVisible"
    size="sm"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isPasswordRequestVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <button class="close" @click="hidePasswordRequestDialog()"><i class="fa fa-times"></i></button>
      <div class="curved border-dark arcticblue p-3 text-centerXXX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-key fa-lg mr-2"></i>Password Reset</p>
        </div>

        <div class="whitesmoke border-main p-2 curved mb-4">
          <p class="lg-1"><span class="text-crimson bold">Step 1</span>. Complete this form to receive a password reset PIN via SMS.</p>

          <sym-text plain align="bottom" v-model="accessId" :caption="app.setup.logonIdCue" captionClass="bold" :disabled="!isStepOne" :modeControl="false"></sym-text>
          <sym-text plain align="bottom" v-model="phoneNumberEndDigits" caption="Last 2 digits of registered phone number" captionClass="bold" :disabled="!isStepOne" :modeControl="false"></sym-text>

          <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button class="info text-center mb-1" @click="onRequestPasswordReset()" :disabled="!isStepOne"><i class="fa fa-send-o fa-lg mr-2"></i> Send Request</button>
          </div>
        </div>

        <div class="whitesmoke border-main p-2 curved">
          <p class="lg-1"><span class="text-crimson bold">Step 2</span>.Enter reset PIN sent to your phone via SMS and your new password</p>

          <sym-text plain align="bottom" v-model="passwordSecurityCode" caption="Password Reset PIN" captionClass="bold" :disabled="isStepOne" :modeControl="false"></sym-text>
  
  
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="newPassword" caption="New Password" captionClass="bold" :disabled="isStepOne" :modeControl="false" @changing="onNewPasswordChanging"></sym-text>
  
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="confirmPassword" caption="Confirm Password" captionClass="bold" :disabled="isStepOne" :modeControl="false" @changing="onNewPasswordChanging"></sym-text>



          <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button class="success text-center mb-1" @click="onSetNewPassword()" :disabled="isStepOne"><i class="fa fa-play fa-lg mr-2"></i> Set New Password</button>
          </div>
        </div>

        <button class="primary darken-2 text-center w-100 shadow-dark lg-1 mt-4" @click="hidePasswordRequestDialog()"><i class="fa fa-times fa-lg mr-2"></i>Close</button>

      </div>
    </div>
  </sym-modal>

<!-- Change Password 1st time login -->
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
      <div class="curved border-dark arcticblue curved p-3 text-centerX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-key fa-lg mr-2"></i>Change Password</p>
        </div>

        <div class="whitesmoke border-main p-2 curved">
          <p class="lg-1"><span class="text-crimson bold">Please update your password to complete your first-time login.</span></p>
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="oldPassword" caption="Old Password" captionClass="bold"  :modeControl="false"></sym-text>
    
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="newPassword" caption="New Password" captionClass="bold"  :modeControl="false" @changing="onNewPasswordChanging"></sym-text>
  
          <sym-text plain align="bottom" :cue="app.setup.passwordCue" :type="getInputKeyType()" v-model="confirmPassword" caption="Confirm Password" captionClass="bold" :modeControl="false" @changing="onNewPasswordChanging"></sym-text>

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
    v-model="isSelectOrgVisible"
    size="md"
    :header="false"
    :customBody="true"
    :footer="false"
    :backdrop="false"
    v-if="isSelectOrgVisible"
  >
    <div class="pos-relative1 board p-1 mb-0">
      <div class="curved border-dark arcticblue curved p-3 text-centerX">
        <div class="text-center mb-2">
          <p class="display-9 bold"><i class="fa fa-users fa-lg mr-2"></i>Select Multipurpose Cooperative</p>
        </div>

         <div class="whitesmoke border-main p-2 curved">
          <!-- <p class="lg-1"><span class="text-crimson bold">Please update your password to complete your first-time login.</span></p> -->

          <sym-combo v-model="orgId" caption="Name" align="bottom" display-field="orgName" :datasource="orgs"></sym-combo>
          
          <div class="buttons mt-3" justify lg-1 shadow-soft>
            <button type="submit" class="info" @click="onSubmitOrg()"><i class="fa fa-play fa-lg mr-2"></i> Submit</button>
          </div>
        </div> 


      </div>
    </div>
  </sym-modal>



  </div>
</div>

</template>

<script>

import {
  getCount,
  logon,
  logoff
} from '../../js/dbSys';

import {
  ajax
} from '../../js/http';

import {
  appConfig
} from '../../js/session';

import
  PageBase
from '../../page/PageBase.vue';

export default {
  extends: PageBase,
  name: 'logon',

  data () {
    return {
      logonId: '',
      password: '',
      isLogonInProgress: false,
      showPassword: false,

      code: 0,
      countDown: 30,

      isPasswordRequestVisible: false,

      accessId: '',
      phoneNumberEndDigits: '',
      newPassword: '',
      confirmPassword: '',
      
      passwordSecurityCode: '',
      passwordStep: 1,

      isChangePasswordRequestVisible: false,
      isChangePasswordRequestVisibleA: false,
      oldPassword: '',
      webChangePassword: false,

      orgs: [],
      platforms: [],
      isSelectOrgVisible: false,

      orgId: 0,
      orgShortName: "",
    };
  },

  computed: {



    // isDisabled () {
    //   if ( this.countDown === 30 || this.countDown === 0 ) {
    //     return false;
    //   } else {
    //     return true;
    //   }
    // }

    isDisabled () {
      return this.countDown > this.buttonEnabledSeconds;
    },

    isStepOne () {
      return this.passwordStep === 1;
    },

  },

  methods: {

    onSubmitOrg () {
      const me = this;

      if (!me.orgId) {
        me.advice.fault("Cooperative is required.", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('orgId');
        return;
      }

      me.sym.userInfo.userOrgId = me.orgId;
      // me.pages = me.sym.userInfo.pages;
      let o = me.orgs.find( o => o.orgId == me.orgId);
      if (o) {
        me.sym.userInfo.userOrgShortName = o.orgShortName
      }

      me.updateUserOrg()
      .then(() => {
        me.goTargetPage(me.sym.userInfo.pagePath);
        me.removeLoginStat();
        me.eventBus.$emit('user:logon');
      })

      me.isSelectOrgVisible = false

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


    refreshUserTag () {
      this.ts +=1;
    },

    isValidAccessPhone() {    
      const me = this;

      let filter = "LogOnId='" + me.accessId + "' AND " + "RIGHT(PhoneNumber, 2)=" + me.phoneNumberEndDigits,
      message;  
      message = 'Last two digits <b>' + me.phoneNumberEndDigits + '</b> does not match with Login ID ' + me.accessId + '.';

      return getCount('dbo.SecUser', filter).then(
      result => {
        if (result===0) {
          me.advice.success(message, { duration: 5 });    
          return false;
        }
        else {
          return true;   
        }   
      }
    );
    },
    
    showPasswordRequestDialog () {
      const me = this;

      me.accessId = '';
      me.phoneNumberEndDigits = '';
      me.passwordSecurityCode = '';
      me.oldPassword = '';
      me.newPassword = '';
      me.confirmPassword = '';
      me.passwordStep = 1;

      me.isPasswordRequestVisible = true;

      me.$nextTick( () => {
        setTimeout(() => {
          me.setFocus('accessId');
        }, 200);
      });

    },

    hidePasswordRequestDialog () {
      const me = this;

      me.isPasswordRequestVisible = false;
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

    hideChangePasswordRequestDialog () {
      const me = this;

      me.isChangePasswordRequestVisible = false;
      me.signout();
      // me.goBack()
    },

    getTargetPath () {
      return {
        name: this.$options.name,
        query: {
          logonId: this.logonId,
          accessKey: this.password
        }
      };
    },

    syncValues (p, q) {
      const me = this;
      if ('logonId' in q) { me.logonId = q.logonId; }
      if ('accessKey' in q) { me.password = q.accessKey; }
    },

    authenticate () {
      const me = this;
      if (me.isLogonInProgress) { return; }
      if (!me.logonId) {
        me.advice.fault("Login ID", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('logonId');
        return;
      }
      if (!me.password.trim()) {
        me.advice.fault("Password", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('password');
        return;
      }

      if (me.isSuspended()) {
        me.showSuspendedMessage();
        me.setFocus('password');
        return;
      }

      const wait = me.wait();
      me.isLogonInProgress = true;
     
      logon(me.getKey()).then(
        info => {
   
          me.stopWait(wait);
          me.isLogonInProgress = false;
   
          me.sym.userInfo.update(info);
          me.pages = me.sym.userInfo.pages;
           
          me.orgs = me.sym.userInfo.orgs;
          me.isChangePasswordRequestVisible = me.sym.userInfo.webChangePassword
           //  JENON

         

          me.removeLoginStat();

          if (me.isChangePasswordRequestVisible) {
            return; 
          }
           if (me.orgs.length > 0) {   
            if (me.orgs.length === 1) {

          me.orgId = me.orgs[0].orgId;  

          me.sym.userInfo.userOrgId = me.orgId;

          let o = me.orgs.find(o => o.orgId === me.orgId);
          if (o) {
            me.sym.userInfo.userOrgShortName = o.orgShortName;
           
              me.goTargetPage(me.sym.userInfo.pagePath);
                  me.removeLoginStat();
                  me.eventBus.$emit('user:logon');

            }
          } else {
          me.isSelectOrgVisible = true;            
          }
          }
          else { 
            me.advice.fault('Access to cooperative is required.', { title: 'Logon failed.' });
            me.signout();
            return;  
          }
          me.eventBus.$emit('user:logon');

        },
        fault => {
          me.stopWait(wait);
          if (fault.status === 401) {
            me.logFailedAttempt();
            me.advice.fault('Enter your correct credentials.', { title: 'Logon failed.' });
            me.setFocus('logonId');
          } else {
            me.core.showHttpFault(fault);
          }
          me.isLogonInProgress = false;
        }
      );
     
    },
    

    goTargetPage (defaultPagePath) {
      const me = this;

      let nextPath = '/';
      if ('redirect' in me.query && me.query.redirect.startsWith('/')) {
        // nextPath = me.query.redirect;
        if (me.query.redirect === '/back') {
          nextPath = '';
          me.goBack();
        } else {
          nextPath = me.query.redirect;
        }
      } else {
        if (defaultPagePath) {
          nextPath = '/' + defaultPagePath;
        } else {
          nextPath = '';
          me.goBack();
        }
      }
      if (nextPath) {
        me.go(nextPath, true);
      }
    },
    // onSubmitOrg() {
    //   const me = this;
  
    // },
    getKey () {
      return window.btoa(this.logonId + ':' + this.password);
    },

    onKeyDown (e, targetValue) {
      const me = this;
      if (e.key === 'Escape') {
        if (targetValue) {
          e.target.value = "";
        } else {
          me.goTargetPage();
        }
      }
    },

    logonIdRejected (e) {
      e.title = e.message;
      e.message = 'Login ID';
      e.enclosed = true;
    },

    passwordRejected (e) {
      e.title = e.message;
      e.message = 'Password';
      e.enclosed = true;
    },

    onResize () {
      const
        me = this,
        left = me.$refs.left,
        right = me.$refs.right,
        mql = window.matchMedia('(max-width: 800px)');

      if (mql.matches) {
        right.style.backgroundImage = me.imageUrl;
        left.style.backgroundImage = 'none';
      } else {
        left.style.backgroundImage = me.imageUrl;
        right.style.backgroundImage = 'none';
      }
    },

    getInputKeyType () {
      return this.showPassword ? 'text' : 'password';
    },

    onRequestPasswordReset () {
      const me = this;
     
      // if (!me.sym.userInfo.changePasswordFlag) {
      //   me.advice.fault("Login ID <b>" +  me.accessId + "</b> is not allowed to change password.");
      //   me.setFocus('accessId');
      //   return;
      // }

      if (!me.accessId) {
        me.advice.fault(me.app.setup.logonIdCue, { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('accessId');
        return;
      }
      if (!me.phoneNumberEndDigits) {
        me.advice.fault("Last 2 digits of phone number", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('phoneNumberEndDigits');
        return;
      }


      let wait = me.wait();
      let filter = "ChangePasswordFlag = 1 AND LogOnId='" + me.accessId + "' AND " + "RIGHT(PhoneNumber, 2)=" + me.phoneNumberEndDigits,
      message;  
      
      message = 'Login ID <b>' +  me.accessId + '</b> is not allowed to change password or Last two digits <b>' + me.phoneNumberEndDigits + '</b> does not match with Login ID ' + me.accessId + '.';

      return getCount('dbo.SecUser', filter).then(
      result => {
        if (result===0) {
          me.advice.fault(message, { duration: 5 });    
          me.stopWait(wait);

          return ;
        }
        else {
   
      me.requestPasswordReset().then(
        info => {
          me.stopWait(wait);
          me.oldPassword = info.password;
          if (me.oldPassword) {
            me.passwordStep = 2;
          }          
          me.dialog.success("Request sent.<br><br>Please check your phone for the Password Reset PIN.", { size: 'sm'}).then(
            () => {
              me.$nextTick( () => {
                setTimeout(() => {
                  me.setFocus('passwordSecurityCode');
                }, 200);
              });
            }
          );
        },
        fault  => {
          me.stopWait(wait);
          me.oldPassword = '';
          me.showFault(fault);
        }
      );

        }   
      }
    );

    },

    updateUserOrg () {
         const
         me = this,
        options = this.core.getAjaxOptions('PUT');
        return ajax('api/user-org/'+ me.sym.userInfo.token + '/'+ me.orgId, options);
    },

    requestPasswordReset () {
      // const
      //   me = this,
      //   options = me.core.getAjaxOptions('POST');    
      //    return ajax('api/passwords/request?accessId=' + me.accessId + '&phoneNumberEndDigits=' + me.phoneNumberEndDigits, options);

         const
         me = this,
        // payload = this.getApiPayload(),
        // body = JSON.stringify(payload),
        options = this.core.getAjaxOptions('PUT');

        return ajax('api/passwords/request?accessId=' + me.accessId + '&phoneNumberEndDigits=' + me.phoneNumberEndDigits, options);



    },

    setNewPassword () {
      const
        me = this;
        // toEmail = me.logonId,
        // accountName = me.userInfoHold.userName,
        // verifyUrl = window.location.origin + '/verifier?email=' + this.logonId + '&code=' + this.userInfoHold.accountCode;

      let payload = {
        accessId: me.accessId,
        phoneNumberEndDigits: me.phoneNumberEndDigits,
        oldPassword: me.oldPassword,
        newPassword: me.newPassword,
        securityCode: me.passwordSecurityCode
      };

      const
        body = JSON.stringify(payload),
        options = me.core.getAjaxOptions('PUT', body);

      return ajax('api/passwords/reset', options);
    },
 
    resetNewPassword () {
      const
        me = this;
       
      let payload = {
        accessId: me.logonId,
        oldPassword: me.password,
        newPassword: me.newPassword
      };

      const
        body = JSON.stringify(payload),
        options = me.core.getAjaxOptions('PUT', body);
      return ajax('api/passwords/reset-default', options);
    },

    onResetPassword () {
      const me = this;
     
      // if (!me.validatePassword(me.newPassword)) {
      //   me.setFocus('newPassword');
      //   return;
      // }

      if (!me.oldPassword) {
        me.advice.fault("Old Password", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('oldPassword');
        return;
      }


      if (me.password !== me.oldPassword) {
        me.advice.fault("Old password dooes not match!");
        me.setFocus('oldPassword');
        return;
      }

      if (me.password == me.newPassword) {
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

    },



    onSetNewPassword () {
      const me = this;

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
      if (!me.passwordSecurityCode) {
        me.advice.fault("Password Reset PIN", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('passwordSecurityCode');
        return;
      }

      let wait = me.wait();

      me.setNewPassword().then(
        success => {
          me.stopWait(wait);
          if (success) {
            me.dialog.success("Password changed.<br><br>You may try logging on now to check.", { size: 'sm'}).then(
              () => {
                me.hidePasswordRequestDialog();
                me.$nextTick( () => {
                  setTimeout(() => {
                    me.setFocus('logonId');
                  }, 200);
                });
              }
            );
          } else {
            me.dialog.fault("Attempt to set new password failed.<br><br>Make sure you have the right PIN.", { size: 'sm'});
          }

        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );

    },

    logFailedAttempt () {
      const
        me = this,
        stat = me.loginStat,
        today = new Date();

      stat.attempts +=1;
      stat.lastAttempt = today.valueOf();
      me.setLocalItem('login', stat);

      if (stat.attempts >= me.maxAttempts) {
        me.showSuspendedMessage();
      }
    },

    removeLoginStat () {
      this.removeLocalItem('login');
    },

    isSuspended () {
      const
        me = this,
        stat = me.loginStat,
        today = new Date();

      if (stat.attempts < me.maxAttempts || !stat.lastAttempt) {
        return false;
      }

      let
        last = new Date(stat.lastAttempt),
        diff = today - last,
        minutes = Math.floor(diff / 60000);

      let flag = (minutes < me.suspendMinutes);
      if (!flag) {
        stat.attempts = 0;
        me.setLocalItem('login', stat);
      }
      return flag;
    },

    showSuspendedMessage () {
      return this.dialog.fault("Too many failed login attempts,<br>Login is suspended for " + this.suspendMinutes + " minute(s).", { size: 'md' })
    }

  },

  created () {
    this.ipAddress = '';
    this.maxAttempts = appConfig.maxLoginAttempts || 3;
    this.suspendMinutes = appConfig.suspendLoginMinutes || 5;
    this.loginStat = {
      attempts: 0,
      lastAttempt: null
    }
    this.imageUrl = '';
    this.dom.on(window, 'resize', this.onResize);
    this.sym.userInfo.userOrgId = this.orgId;
    this.sym.userInfo.userOrgShortName = this.orgShortName;
    // console.log(sym.userInfo.orgs);
    // console.log(sym.userInfo.platforms);
  },

  beforeDestroy () {
    this.dom.showMenu();
    this.app.showHeaderFooters();
    this.dom.off(window, 'resize', this.onResize);
  },

  mounted () {
    const
      me = this,
      meta = me.meta,
      // image = 'logon-' + this.core.getRandomInteger(12).toString();
      image = 'logon-' + this.core.getRandomInteger(15).toString();

    me.dom.hideMenu();
    me.app.hideHeaderFooters();
    // me.imageUrl = `url(${require(`../../img/bgd/${image}.jpg`)})`;
    // public folder
    me.imageUrl = `url(img/bgd/${image}.jpg)`;

    meta.logonId.mode = 'R';
    meta.password.mode = 'R';

    let stat = me.getLocalItem('login');
    if (stat) {
      Object.assign(me.loginStat, stat);
    }

    // me.core.isReachable(appConfig.ipInfoUrl).then(
    //   isOnline => {
    //     if (isOnline) {
    //       get(appConfig.ipInfoUrl).then(
    //         info => {
    //           me.ipAddress = info.ip;
    //         },
    //         () => {
    //         }
    //       );
    //     }
    //   }
    // );

    me.syncValues(me.params, me.query);
    me.onResize();
  }

};

</script>

<style scoped>

.left {
  width: 35%;
}

.right {
  width: 65%;
  /* background-color: #36454f; */
  background-color: rgb(15, 34, 83);
}

.left,
.right {
  background-position: center;
  background-size: cover;
  height: 100vh;
}

.box-entry {
  margin: auto;
  margin-top: 4%;
  max-width: 90%;
  min-width: 65%;
  border: solid 4px #2d2d2d;
}

.show-password >>> .form-control {
  margin-bottom: 0;
  margin-left: .875rem;
}

@media (max-width: 800px) {
  .left {
    display: none;
    width: 0;
  }
  .right {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .box-entry {
    min-width: 96%;
  }
}

@media (max-width: 320px) {
  .box-entry {
    min-width: 98%;
  }
}

</style>