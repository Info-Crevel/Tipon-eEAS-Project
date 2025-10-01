<script>

import
  PageBase
from './PageBase.vue';

export default {
  extends: PageBase,
  name: 'PageMaintenance',

  data () {
    return {
      isFilled: false,

      // 05 Feb 2025 - EMT
      noAddFlag: false,
      noEditFlag: false,
      noDeleteFlag: false
    }
  },

  // 05 Feb 2025 - EMT
  computed: {
    canAdd () { return !this.noAddFlag; },
    canEdit () { return !this.noEditFlag; },
    canDelete () { return !this.noDeleteFlag; }
  },

  methods: {
    hasChanges () {
      return false;
    },

    // onEscape () {
    //   const me = this;

    //   if (!me.$data[me.dataConfig.models[0]][me.dataConfig.keyField]) {
    //     return;
    //   }

    //   // if (document.querySelector('.modal-dialog')) {
    //   //   return;
    //   // }

    //   if (me.isModalShown()) {
    //     return;
    //   }

    //   me._onClear(true);
    // },

    // _onLoad () {
    //   this.onLoad();
    // },

    // 05 Feb 2025 - EMT
    _onLoad () {
      const me = this;
      me.onLoad();
      let info = me.sym.getPageInfo(me.$options.name);
      me.noAddFlag = info.xa;
      me.noEditFlag = info.xe;
      me.noDeleteFlag = info.xd;
    },

    onLoad () {
      // empty by design; intended for override
    },

    onNew () {
      const
        me = this,
        dc = me.dataConfig,
        data = me.$data;

      let keyValue = data[dc.models[0]][dc.keyField];
      if (dc.keyField && dc.autoAssignKey && me.core.isInteger(keyValue) && !keyValue) {
        data[dc.models[0]][dc.keyField] = -1;
        me.loadData();
        me.replaceUrl();
      }
    },

    // 05 Feb 2025 - EMT (changed to computed property)
    // canDelete () {
    //   return true;
    // },

    // facade
    onClear (e) {
      this._onClear(e);
    },

    _onClear (e) {
      const
        me = this,
        activeElement = document.activeElement;

      if (me.hasChanges()) {
        // me.dialog.confirmCancel('Save changes before clearing the form?', { scheme: 'warning', icon: 'warning'}).then(
        me.dialog.confirmCancel('Save changes before clearing the form?', { scheme: 'warning', icon: 'warning', size: 'md'}).then(
          reply => {
            switch (reply) {
              case 'no':
                me.onReset();
                return;

              case 'cancel':
                if (activeElement) {
                  activeElement.focus();
                  me.dom.scrollIntoView(activeElement);
                } else {
                  me.$el.focus();
                }
                return;

              default:
                me.onSubmit();
                return;
            }
          }
        );
      } else {
        if (e instanceof MouseEvent) {
          me.onReset();
          return;
        }

        // me.dialog.confirm('Clear the form?').then(
        me.dialog.confirm('Clear the form?', { size: 'md'}).then(
          reply => {
            if (reply === 'yes') {
              me.onReset();
              return;
            }
            if (activeElement) {
              activeElement.focus();
              me.dom.scrollIntoView(activeElement);
            } else {
              me.$el.focus();
            }
          }
        );
      }
    },

    // facade
    isNew () {
      return this._isNew();
    },

    _isNew () {
      const
        me = this,
        dc = me.dataConfig,
        data = me.$data;

      if (dc.models.length && dc.keyField) {
        let keyValue = data[dc.models[0]][dc.keyField];
        // if (me.core.isInteger(keyValue) && keyValue < 0) {
        //   return true;
        // }
        if (me.core.isInteger(keyValue) && keyValue < 0 && dc.autoAssignKey) {
          return true;
        }

        if ('lockId' in data[dc.models[0]] && !data[dc.models[0]]['lockId']) {
          return true;
        }
      }
      return false;
    },

    // facade
    onReset (initialize) {
      this._onReset(initialize);
    },

    _onReset (initialize) {
      const
        me = this,
        dataConfig = me.dataConfig,
        core = me.core,
        data = me.$data,
        bf = me.boundFields;

// me.setActiveModel();
      me.onResetBefore();

      Object.keys(bf).forEach(key => {
        if (bf[key].control) {
          bf[key].control.clearFaultTag();
        }
      });

      if (dataConfig.models.length) {
        if (!me.isNew()) {
          me.setCopyData();
        }
        let model;

        dataConfig.models.forEach( model => {
          if (model in data) {
            if (core.isObject(data[model])) {
              core.resetObject(data[model]);
            }
            if (core.isArray(data[model])) {
              data[model] = [];
            }
          }
        });

        if (initialize) {
          me.syncValues(me.params, me.query);
        }

        me.replaceUrl();

        for (let index = 0; index < dataConfig.models.length; index++) {
          model = dataConfig.models[index];
          if (core.isObject(data[model])) {
            me.setDisplayMode(data[model]);
          }
        }

        if (me.dataConfig.keyField) {
          me.setRequiredMode(dataConfig.keyField);
        }
        me.goTop();

        me.isFilled = false;

        setTimeout(() => {
          me.enableElement('btn-new');
          // 05 Feb 2025 - EMT
          if (me.noAddFlag) {
            me.disableElement('btn-new');
            me.disableElement('btn-copy');
          }
          me.disableElement(
            'btn-save',
            'btn-delete',
            'btn-clear',
            'btn-log',
            'btn-copy',
          );
        }, 100);

        me.onResetAfter();

        me.refresh();
      }

    },

    setDefaultControlStates () {
      const
        me = this,
        models = me.dataConfig.models;

      // fields

      if (models.length && models[0] in me.$data && me.core.isObject(me.$data[models[0]])) {
        me.setOptionalMode(me.$data[models[0]]);
      }
      if (me.dataConfig.keyField) {
        me.setDisplayMode(me.dataConfig.keyField);
      }

      // buttons

      me.disableElement(
        'btn-new',
        'btn-log',
        'btn-copy'
      );

      me.enableElement(
        'btn-save',
        'btn-clear',
        'btn-copy'
      );

      if (!me.isNew()) {
        me.enableElement(
          'btn-log',
        );

        // 05 Feb 2025 - EMT
        if (me.noEditFlag) {
          me.disableElement(
            'btn-save',
            'btn-add',
            'btn-dtl-edit',
            'btn-dtl-delete'    
          );
          me.dialog.warning('Note: You are NOT allowed to edit documents.', { duration: 5, size: 'sm' });
        }

        // if (me.canDelete()) {
        //   me.enableElement(
        //     'btn-delete',
        //   );
        // }

        // 05 Feb 2025 - EMT (canDelete is now a property)
        if (me.canDelete) {
          me.enableElement(
            'btn-delete',
          );
        }

      }

    },

    // 05 Feb 2025 - EMT
    setupGridControls () {
      const me = this;

      setTimeout(() => {
        me.disableElement(
          'btn-add'
        );

        if (me.canAdd) { me.enableElement('btn-add'); }
      }, 100);

      me.setupDetailControls();
    },

    // 05 Feb 2025 - EMT
    setupDetailControls () {
      const me = this;

      setTimeout(() => {
        me.disableElement(
          'btn-dtl-edit',
          'btn-dtl-delete'
        );

        if (me.canEdit) { me.enableElement('btn-dtl-edit'); }
        if (me.canDelete) { me.enableElement('btn-dtl-delete'); }
      }, 100);
    },

    // 05 Feb 2025 - EMT
    scrollToBottom (listId) {
      const me = this;

      if (!listId) { return; }
      // let el = me.$refs.list;
      let el = me.$refs[listId];
      if (el) {
        me.dom.scrollToBottom(el);
      }
    },

    // 05 Feb 2025 - EMT
    getDeleteQuery (lockId) {
      return '?currentUserId=' + this.sym.userInfo.userId + '&lockId=' + lockId;
    },

    goTop () {
      const me = this;
      if (me.dataConfig.keyField) {
        me.goTopField(me.dataConfig.keyField);
      }
    }

  },

  mounted () {
    const
      me = this,
      dataConfig = me.dataConfig,
      data = me.$data;

    dataConfig.models = [];
    dataConfig.keyField = '';

    me._onLoad();
    me.onReset(true);

    if (me.core.isObject(data[dataConfig.models[0]]) && dataConfig.keyField) {
      //
      // auto-load record
      //
      if (data[dataConfig.models[0]][dataConfig.keyField]) {
        me.loadData();
      }
    }
  },

  beforeRouteLeave (to, from, next) {
    if (to.name === 'fault') {
      next();
      return;
    }

    if (this.hasChanges()) {
      this.dialog.confirmCancel('You have unsaved changes to this record.<br>Save before leaving the page?', { scheme: 'warning', icon: 'warning', size: 'md' })
        .then( reply => {
          if (reply === 'yes') {
            this.onSubmit(to);
            next(false);
          } else if (reply === 'cancel') {
            next(false);
          } else {
            next();
          }
        });
      return;
    }

    next();
  }

}

</script>