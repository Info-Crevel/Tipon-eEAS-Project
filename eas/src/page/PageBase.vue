<template>
  <div>
  </div>
</template>

<script>

import {
  DateTime,
  localDb,
  sessionDb
} from '../js/core';

import {
  getCount
} from '../js/dbSys';

export default {
  name: 'PageBase',

  provide () {
    return {
      theme: this.core.config.theme,
      hostPage: this
    };
  },

  data () {
    return {
      ts: 0,    // timestamp
      boundFields: {},
      exceptions: [],
      dataCopy: {}
    };
  },

  computed: {
    params () { return this.$route.params; },
    query () { return this.$route.query; },
    basePath () { return this.sym.getPath(this.$options.name); },
    pageName () { return this.sym.getPageName(this.$options.name); },
    isMono () { return this.core.config.buttonScheme === 'mono'; },

    submitButtonClass () {
      return this.isMono ? false : 'info-light';
    },

    clearButtonClass () {
      return this.isMono ? false : 'info-light';
    },

    deleteButtonClass () {
      return this.isMono ? false : 'danger-light';
    },

    backButtonClass () {
      return this.isMono ? false : 'primary-light';
    },

    logButtonClass () {
      return this.isMono ? false : 'info-light';
    },

    copyButtonClass () {
      return this.isMono ? false : 'secondary-light';
    },

    cancelButtonClass () {
      return this.isMono ? false : 'info-light';
    },

    activeButtonClass () {
      return this.isMono ? false : 'info-light';
    },

  },

  methods: {
    go (route, replaceUrlFlag = false) {
      this.core.navigate(route, replaceUrlFlag);
    },

    goHome () {
      this.go('/');
    },

    goBack () {
      this.$router.go(-1);
    },

    goNotFound () {
      this.go({ name: 'not-found' });
    },

    loadData () {
      // empty by design
    },

    syncValues () {
      // empty by design
    },

    replaceUrl (onComplete) {
      this.$router.replace(this.getTargetPath(), onComplete, () => {});
    },

    getTargetPath () {
      return { path: this.basePath };
    },

    reload () {
      // navigating to a new route triggers 'beforeRouteUpdate' of page component
      // this.$router.push(this.getTargetPath());
      this.$router.push(this.getTargetPath(), () => {}, () => {});
    },

    refresh () {
      this.ts +=1;
    },

    setFocus (colName) {
      const fm = this.getMeta(colName);
      if (fm && fm.control && this.core.isFunction(fm.control.setInputFocus)) {
        fm.control.setInputFocus();
      }
    },

    focusNext () {
      this.core.focusNext();
    },

    enlistField (colName, meta) {
      const
        ctrl = meta.control,
        inputBox = ctrl.getInputBox(),
        fields = this.boundFields,
        fldDef = this.core.getFieldDef(colName);

      let element = null;
      if (inputBox) {
        element = inputBox.form;
      }
      fields[colName] = meta;
      fields[colName].caption = ctrl.label;
      if (element && element.id) {
        fields[colName].formId = element.id;
      }
      if (!('minLength' in meta)) {
        if (fldDef && 'minLength' in fldDef) {
          meta.minLength = fldDef.minLength;
        }
      }
    },

    isValid (formId, customExceptions = []) {
      const me = this;
      let
        meta,
        value,
        focusField;

      me.exceptions = me.exceptions.filter(getExceptionMismatch, formId).concat(customExceptions);

      for (let colName in me.boundFields) {
        meta = me.boundFields[colName];
        meta.control.clearFaultTag();
        if (formId === meta.formId || !formId) {
          value = meta.control.value;
          if (value instanceof me.core.DateTime) {
            if (value.isEmpty() && meta.mode === 'R') {
              me.addException(formId, colName, meta.caption, "Required");
              if (!focusField) {
                focusField = colName;
              }
            }
          } else {
            if (!value) {
              if (typeof value === 'string' || meta.datatype === 'S') {
                if ('minLength' in meta && me.core.isNumber(meta.minLength) && meta.minLength > 0 ) {
                  me.addException(formId, colName, meta.caption, "Minimum length is " + meta.minLength);
                  if (!focusField) {
                    focusField = colName;
                  }
                } else {
                  if (meta.mode === 'R') {
                    me.addException(formId, colName, meta.caption, "Required");
                    if (!focusField) {
                      focusField = colName;
                    }
                  }
                }
              } else {
                if (meta.mode === 'R') {
                  me.addException(formId, colName, meta.caption, "Required");
                  if (!focusField) {
                    focusField = colName;
                  }
                }
              }
            }
          }
        }
      }
      if (focusField) {
        me.setFocus(focusField);
      }
      return me.exceptions.length === 0 ? true : false;
    },

    // 01 Mar 2025 - EMT (re-purposed method)
    // isActionAllowed (...actionName) {
    //   return this.sym.isActionAllowed(...actionName);
    // },

    // 01 Mar 2025 - EMT (re-purposed method, now using Action ID of SecAction)
    isActionAllowed (actionId) {
      const me = this;

      let o = me.sym.userInfo.restrictions.find(obj => obj.actionId === actionId);
      if (!o) {
        // 14 Mar 2025 - EMT (bug fix)
        // me.advice.fault('Undefined action identifier (' + actionId + ')');
        // return Promise.resolve('cancel');
        return Promise.resolve('yes');
      }

      if (!o.password) {
        return Promise.resolve('yes');
      }

      let
        message = 'The requested action is restricted.<br><b>' + o.actionName + '</b><br>Enter the password to continue.' ,
        options = { title: 'Restricted Action', scheme: 'warning' };

      return me.security.pass(message, o.password, options);
    },

    formExceptions (formId) {
      return this.exceptions.filter(getException, formId);
    },

    addException (formId, colName, caption, message, customExceptions) {
      const
        me = this,
        fm = me.getMeta(colName),
        exception = { formId: formId, colName: colName, caption: caption, message: message };

      if (customExceptions) {
        customExceptions.push(exception);
        return;
      }
      me.exceptions.push(exception);
      if (fm && fm.control) {
        fm.control.setFaultTag();
      }
    },

    removeException (colName) {
      const
        me = this,
        ex = me.core.getArrayItem(me.exceptions, 'colName', colName);

      if (!ex) { return; }
      me.core.deleteArrayItem(me.exceptions, ex);
    },

    showFault (fault) {
      this.dialog.fault(this.core.getFaultText(fault));
    },

    wait () {
      return this.dialog.wait();
    },

    stopWait (modalInstance) {
      this.dialog.stop(modalInstance);
    },

    // getMeta (colName) {
    //   let id = colName;
    //   if (this.dataConfig.models.length) {
    //     id = this.dataConfig.models[0] + '-' + colName;
    //   }
    //   return this.core.getMetaItem(this.meta, id);
    // },

    getMeta (colName) {
      let id = colName;
      if (this.dataConfig.models.length) {
        let host = this.dataConfig.models[0];
        if (this.dataConfig.activeModel) {
          host = this.dataConfig.activeModel;
        }
        id = host + '-' + colName;
      }
      return this.core.getMetaItem(this.meta, id);
    },

    setRequiredMode (...columnNames) {
      const me = this;

      if (me.core.isArray(columnNames)) {
        if (me.core.isObject(columnNames[0])) {
          const colNames = [];
          Object.keys(columnNames[0]).forEach( key => {
            colNames.push(key);
          });
          setFieldMode.call(me, 'R', ...colNames);
        } else {
          setFieldMode.call(me, 'R', ...columnNames);
        }
      }
    },

    setOptionalMode (...columnNames) {
      const me = this;

      if (me.core.isArray(columnNames)) {
        if (me.core.isObject(columnNames[0])) {
          const colNames = [];
          Object.keys(columnNames[0]).forEach( key => {
            colNames.push(key);
          });
          setFieldMode.call(me, 'O', ...colNames);
        } else {
          setFieldMode.call(me, 'O', ...columnNames);
        }
      }
    },

    setDisplayMode (...columnNames) {
      const me = this;

      if (me.core.isArray(columnNames)) {
        if (me.core.isObject(columnNames[0])) {
          const colNames = [];
          Object.keys(columnNames[0]).forEach( key => {
            colNames.push(key);
          });
          setFieldMode.call(me, 'D', ...colNames);
        } else {
          setFieldMode.call(me, 'D', ...columnNames);
        }
      }
    },

    setActiveModel (modelId) {
      if (modelId) {
        this.dataConfig.activeModel = modelId;
      } else {
        this.dataConfig.activeModel = '';
      }
    },

    setFaultTag (...refs) {
      const me = this;
      let obj = null;
      refs.forEach( ref => {
        if (me.core.isString(ref)) {
          obj = me.$refs[ref];
          if (obj) {
            if (me.core.isFunction(obj.setFaultTag)) {
              obj.setFaultTag();
            } else {
              me.dom.addAttribute(obj, 'fault');
            }
          }
        }
      });
    },

    clearFaultTag (...refs) {
      const me = this;
      let obj = null;

      refs.forEach( ref => {
        if (me.core.isString(ref)) {
          obj = me.$refs[ref];
          if (obj) {
            if (me.core.isFunction(obj.clearFaultTag)) {
              obj.clearFaultTag();
            } else {
              me.dom.removeAttribute(obj, 'fault');
            }
          }
        }
      });
    },

    initPage () {
      const me = this;
      if (me.$options.name === 'PageBase') {
        return;
      }

      me.core.sessionMgr.setCurrentPage(me);

      let
        fldDef,
        metaId,
        sysData = ['ts','boundFields','config'];

      for (let key in me.$data) {
        if (!sysData.includes(key) && !Array.isArray(me.$data[key])) {
          if (me.core.isObject(me.$data[key]) && !(me.$data[key] instanceof DateTime)) {
            for (let propName in me.$data[key]) {
              metaId = key + '-' + propName;
              me.meta[metaId] = {};
              fldDef = me.core.getFieldDef(propName);
              if (fldDef && 'datatype' in fldDef) {
                me.meta[metaId].datatype = fldDef.datatype;
              }
            }
          } else {
            me.meta[key] = {};
            fldDef = me.core.getFieldDef(key);
            if (fldDef && 'datatype' in fldDef) {
              me.meta[key].datatype = fldDef.datatype;
            }
          }
        }
      }
    },

    isValidEntity (dataSourceName, fieldId, fieldValue, detectInteger = true) {
      let filter;

      if (detectInteger && this.core.isInteger(fieldValue)) {
        filter = fieldId + '=' + fieldValue.toString();
      } else {
        filter = fieldId + "='" + fieldValue + "'" ;
      }

      return getCount(dataSourceName, filter).then(
        count => {
          return count > 0;
        },
        () => {
          return false;
        }
      );

    },

    goTopField (fieldId) {
      setTimeout(() => {
        this.setFocus(fieldId);
      }, 200);
    },

    enableElement (...className) {
      this.dom.toggleAttribute('disabled', false, ...className);
    },

    disableElement (...className) {
      this.dom.toggleAttribute('disabled', true, ...className);
    },

    isModalShown () {
      return document.querySelector('.modal-dialog') ? true : false;
    },

    setCopyData () {
      const me = this;

      if (me.dataConfig.models.length) {
        let
          copy = {},
          host = me.dataConfig.models[0];

        Object.assign(copy, me[host]);
        me.copyData = copy;
      }
    },

    getBooleanIconClass (value) {
      return value ? 'fa fa-check' : 'fa';
    },

    // data storage

    getLocalItem (key) {
      return localDb.get(key);
    },

    setLocalItem (key, info) {
      localDb.set(key, info);
    },

    removeLocalItem (key) {
      localDb.remove(key);
    },

    getSessionItem (key) {
      return sessionDb.get(key);
    },

    setSessionItem (key, info) {
      sessionDb.set(key, info);
    },

    removeSessionItem (key) {
      sessionDb.remove(key);
    },

    // internal methods

    _onLoad () {
      // empty by design
    },

    // facade methods

    onReset () {
      // empty by design
    },

    onLoad () {
      // empty by design
    },

    onSubmit () {
      // empty by design
    },

    onDelete () {
      // empty by design
    },

    onViewLog () {
      // empty by design
    },

    onClear () {
      // empty by design
    },

    onResetBefore () {
      // empty by design; called by _onReset() via method override
    },

    onResetAfter () {
      // empty by design; called by _onReset() via method override
    },

    onButtonMouseDown (e) {
      e.preventDefault();
    },

    onCopy () {
      // empty by design
    },

  },

  beforeMount () {
    this.initPage();
  },

  created () {
    this.meta = {};
    this.dataConfig = {
      models: [],
      keyField: '',
      autoAssignKey: false,
      activeModel: ''
    };
  },

  destroyed () {
    const me = this;

    me.core.sessionMgr.setCurrentPage(null);

    let m;
    for (let colName in me.meta) {
      m = me.meta[colName];
      if (m.control) {
        m.control = null;
      }
    }

    me.meta = {};
    me.boundFields = {};
  }

};

function getException (item) {
  return item.formId === this;
}

function getExceptionMismatch (item) {
  return item.formId !== this;
}

function setFieldMode (mode, ...columnNames) {
  const meta = this.meta;
  let
    host,
    id;

  if (this.dataConfig.models.length) {
    host = this.dataConfig.models[0];
    if (this.dataConfig.activeModel) {
      host = this.dataConfig.activeModel;
    }
  }

  columnNames.forEach( columnName => {
    id = host ? host + '-' + columnName : columnName;
    if (id in meta) {
      meta[id].mode = mode;
      if (meta[id].control) {
        meta[id].control.applyFieldMode();
      }
    }
  });
}

</script>
