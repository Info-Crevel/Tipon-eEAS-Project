<template>
  <div>
  </div>
</template>

<script>

import {
  config,
  DateTime
} from '../js/core';

export default {
  inject: {
    'injHostPage': { from: 'hostPage'},
    'injTheme': { from: 'theme' }
  },

  props: {
    colName: String,
    caption: String,
    captionWidth: { type: Number, default: 16 },
    captionClass: { type: String, default: undefined },
    bullet: String,
    inputWidth: Number,
    icon: String,
    required: Boolean,
    align: { type: String, default: 'right' },
    captionAlign: { type: String, default: 'left' },
    textAlign: { type: String, default: 'left' },
    lookupId: { type: String,  default: '' },
    inputBoxClass: { type: String, default: undefined },
    modeControl: { type: Boolean, default: true },
    isLookupQuery: { type: Boolean, default: false},
    themeId: {
      type: String,
      validator: (value) => {
        return config.themes.indexOf(value) !== -1;
      }
    },
  },

  data () {
    return {
      ts: 0,
      id: '',
      label: '',
      maximumLength: 0,
      mask: '',
      isEntityId: false,
      noFormat: false,
      hasFocus: false,
      isEnterPressed: false,
      isAutoId: false
    };
  },

  computed: {
    hostPage () {
      return this.injHostPage;
    },

    theme () {
      return this.themeId ? this.core.getTheme(this.themeId) : this.injTheme;
    },

    fieldId () {
      let exp = this.$vnode.data.model ? this.$vnode.data.model.expression : '';
      return this.colName || exp.split('.')[exp.split('.').length - 1];
    },

    meta () {
      return this.core.getMetaItem(this.core.sessionMgr.currentPageMeta, this.metaId);
    },

    metaId () {
      let
        host,
        fieldId = this.fieldId,
        exp = this.$vnode.data.model ? this.$vnode.data.model.expression : '';

      if (exp) {
        if (exp.indexOf('.') > -1) {
          host = exp.split('.')[0];
        }
        if (exp.indexOf('[') > -1) {
          host = exp.split('[')[0];
        }
        if (host) {
          return host + '-' + fieldId;
        }
      }
      return fieldId;
    },

    isPlain () { return 'plain' in this.$attrs; },

    isAlignBottom () { return this.align === 'bottom' ? true : false; },
    hasCaption () { return (this.label || this.bullet) ? true : false; },

    controlClass () {
      const me = this;
      let cls;
      if (me.isAlignBottom) {
        cls = (me.inputWidth ? 'fw-' + me.inputWidth : '') + (me.hasCaption ? ' d-block' : ' d-flex');
      } else {
        cls = 'd-inline-flex' + (me.inputWidth ? '' : ' w-100');
      }
      return 'form-control ' + cls.trim();
    },

    labelClass () {
      const me = this;
      if (me.hasCaption) {
        let cls = me.isAlignBottom ? 'top-tag w-100' : 'left-tag' + (me.bullet ? '' : ' fw-' + me.captionWidth);
        if (me.captionAlign === 'right' || me.captionAlign === 'center') {
          cls = cls + ' text-' + me.captionAlign;
        }
        return 'input-tag ' + (me.isPlain ? 'plain ': '') + cls + (me.captionClass ? ' ' + me.captionClass : '');
      }
      return 'd-none';
    },

    inputAlign () {
      return this.isEntityId ? 'left' : this.textAlign;
    },

    bulletClass () { return this.bullet ? 'fa fa-fw fa-lg fa-' + this.bullet : ''; },

    minValMessage () { return config.msgMinValue; },
    maxValMessage () { return config.msgMaxValue; },
    maxLen () { return this.maximumLength ? this.maximumLength : false; }
  },

  methods: {
    async handleChange (newValue, oldValue, displayText = '') {
      const me = this;
      if (!me.isValid(newValue)) {
        me.restoreOldValue(oldValue);
        return;
      }
      let e;

      await me.emitChangingEvent(newValue, displayText)
        .then( (event) => {
          e = event;
          if (e.defaultPrevented && !(me.isAutoId && newValue === -1)) {
            // me.advice.fault(e.message || me.formatValue(e.proposedValue), { title: e.title, duration: e.messageDuration || 4 });
            if (!e.suppressMessage) {
              me.advice.fault(e.message || me.formatValue(e.proposedValue), { title: e.title, duration: e.messageDuration || 4 });
            }
            me.restoreOldValue(oldValue);
          } else {
            me.emitChangedEvent(e.proposedValue, me.fieldId);
            me.copyValue = e.proposedValue;
            me.clearFaultTag();
            let page = me.core.sessionMgr.currentPage;
            if (page) {
              page.removeException(me.fieldId);
            }
            if (me.isEnterPressed) {
              me.focusNext();
            }
          }
        })

    },

    getMaskedValue ( value ) {
      if (this.core.isString(value)) {
        let val = value;
        if (this.mask === '!') { val = value.toUpperCase(); }
        if (this.mask === '_') { val = value.toLowerCase(); }
        return val.trim();
      }
      return value;
    },

    isValid (newValue) {
      const me = this;
      let
        code = '',
        message = '',
        title = '',
        isRequired = false,
        enclosed = false;

      isRequired = (me.required || me.getMeta('mode') === 'R') && (!newValue || (newValue instanceof DateTime && newValue.isEmpty()));
      if (isRequired) {
        code = 'req';
        if (me.label) {
          message = me.label;
          title = config.msgEntryRequired;
          enclosed = true;
        } else {
          message = config.msgEntryRequired;
        }
        me.handleRejected(newValue, code, message, title, enclosed);
        return false;
      }

      // if (me.core.isString(newValue)) {
      if (me.core.isString(newValue) && !me.isLookupQuery) {
        code = 'mnl';
        let minLen = me.getMeta('minLength') || me.getDef('minLength');
        // if (minLen && newValue.length < minLen) {
        if (newValue && minLen && newValue.length < minLen) {
          title = config.msgEntryRejected;
          if (!newValue) {
            title = config.msgEntryRequired;
          }
          message = config.msgMinLength + me.core.toIntegerFormat(minLen);
          me.handleRejected(newValue, code, message, title, enclosed);
          return false;
        }
      }

      let
        minVal = me.getMinValue(),
        maxVal = me.getMaxValue();

      // if (minVal && newValue < minVal) {
      if (minVal && newValue && (newValue < minVal)) {
        code = 'min';
        title = config.msgEntryRejected;
        if (!newValue) {
          title = config.msgEntryRequired;
        }
        message = me.minValMessage + me.formatValue(minVal);
        me.handleRejected(newValue, code, message, title, enclosed);
        return false;
      }
      if (maxVal && newValue > maxVal) {
        code = 'max';
        title = config.msgEntryRejected;
        message = me.maxValMessage + me.formatValue(maxVal);
        me.handleRejected(newValue, code, message, title, enclosed);
        return false;
      }
      return true;
    },

    handleRejected (newValue, code, message, title, enclosed = false) {
      const
        e = this.emitValueRejectedEvent(newValue, code, message, title, enclosed),
        options = e.title ? { title: e.title, enclosed: e.enclosed } : {};

      this.advice.fault(e.message, options);
    },

    handleKeyDown (e, targetValue, value) {
      const me = this;
      if (me.lookupId && e.key === 'F4') {
        me.emitSearchEvent(me.lookupId);
        return;
      }
      if (e.key === 'F9') {
        if (me.hostPage && 'copyData' in me.hostPage) {
          if (me.fieldId in me.hostPage['copyData']) {
            let value = me.hostPage['copyData'][me.fieldId];
            me.onCopyValue(value);
            return;
          }
        }
      }
      // me.isEnterPressed = e.key === 'Enter' && !e.tabOnly;
      me.isEnterPressed = (e.key === 'Enter' && !e.tabOnly) || e.key === 'F9';
      if (me.isEnterPressed || (e.key === 'Tab' && !e.shiftKey)) {
        let internal = me.toInternalValue(targetValue);
        if (me.isEqual(internal, value)) {
          if (me.isEmpty()) {
            if (!me.isValid(internal)) {
              e.preventDefault();
              me.restoreOldValue(value);
              return;
            }
          }
          if (me.isEnterPressed) {
            e.preventDefault();
            e.stopPropagation();
            me.focusNext();
          }
        }
      }
    },

    setInputFocus () {
      const box = this.getInputBox();
      if (box) {
        return new Promise((resolve) => {
          setTimeout( () => {
            box.focus();
            this.selectInputBoxContent();
            resolve(true);
          }, 100);
        });
      } else {
        return Promise.resolve(false);
      }
    },

    restoreOldValue (oldValue) {
      const box = this.$refs.inputbox;
      if (box) {
        box.value = oldValue;
      }
      this.setInputFocus();
    },

    setInputValue (value, eventName = 'change') {
      if (!eventName) { return; }
      const
        me = this,
        el = me.$refs.inputbox;

      if (!el) { return; }
      el.value = value;
      const e = new CustomEvent(eventName);
      el.dispatchEvent(e);
      me.$nextTick( () => {
        me.selectInputBoxContent();
      });
    },

    setFaultTag () {
      if (this.$refs.cap) {
        this.dom.addAttribute(this.$refs.cap, 'fault');
      }
    },

    clearFaultTag () {
      if (this.$refs.cap) {
        this.dom.removeAttribute(this.$refs.cap, 'fault');
      }
    },

    buttonClick (lookupId) {
      if (!lookupId) { return; }
      this.setInputFocus().then(
        focused => {
          if (focused) {
            this.emitSearchEvent(lookupId);
          }
        }
      );
    },

    documentFocusIn (e) {
      const me = this;
      if (me.$refs.inputbox === e.target) {
        me.$nextTick( () => {
          me.selectInputBoxContent();
        });
      }
    },

    focusNext () {
      this.core.focusNext();
    },

    selectInputBoxContent () {
      const box = this.getInputBox();
      if (box && this.core.isFunction(box.select)) {
        box.select();
      }
    },

    getInputBox () {
      const inputBox = this.$refs.inputbox;
      if (Array.isArray(inputBox)) {
        return inputBox[0];
      }
      return inputBox;
    },

    getDef (id) {
      if (this.fieldId) {
        let f = this.core.getFieldDef(this.fieldId);
        return (f && id in f) ? f[id] : null;
      }
      return null;
    },

    getMeta (id) {
      const fm = this.meta;
      if (fm && id in fm) {
        return fm[id];
      }
      return null;
    },

    getMinValue () {
      return this.getMeta('minValue') || this.getDef('minValue');
    },

    getMaxValue () {
      return this.getMeta('maxValue') || this.getDef('maxValue');
    },

    formatValue (value) {
      return value.toString();
    },

    toInternalValue (value) {
      return value;
    },

    isEmpty () {
      return true;
    },

    isEqual (newValue, oldValue) {
      if (newValue === oldValue) { return true; }
      if (!newValue && (oldValue === null)) { return true; }
      return false;
    },

    async emitChangingEvent (proposedValue, displayText = '') {
      let
        me = this,
        e = new CustomEvent('changing', { cancelable: true, bubbles: false });

      e.proposedValue = proposedValue;
      e.title = 'Entry rejected.';
      e.message = '';
      e.messageDuration = 4;
      e.suppressMessage = false;
      if (displayText) {
        e.displayText = displayText;
      }
      me.$emit('changing', e);
      if (me.core.isFunction(e.callback)) {
        let result = await e.callback(e);
        if (!result) {
          e.preventDefault();
        }
      }
      return e;
    },

    emitChangedEvent (newValue, colName) {
      this.$emit('changed', newValue, colName);
    },

    emitValueRejectedEvent (proposedValue, rejectCode, message, title = '', enclosed = false) {
      let e = new CustomEvent('rejected', { bubbles: false });
      e.proposedValue = proposedValue;
      e.rejectCode = rejectCode;
      e.message = message;
      e.title = title;
      e.enclosed = enclosed;
      this.$emit('rejected', e);
      return e;
    },

    emitSearchEvent (lookupId) {
      const
        me = this,
        props = { cancelable: true, bubbles: false },
        box = me.getInputBox();

      const e = new CustomEvent('search', props);
      e.lookupId = lookupId;
      e.filter = '';
      e.sort = '';
      e.size = '';
      e.dismissible = true;

      if (box) {
        if ('value' in box && box.value) {
          e.searchValue = box.value;
        } else {
          if (me.copyValue) {
            e.searchValue = me.copyValue;
          }
        }
      }

      me.$emit('search', e);
      if (e.defaultPrevented) {
        return;
      }

      let options = {
        triggerControl: me,
        filter: e.filter,
        sort: e.sort,
        size: e.size,
        dismissible: e.dismissible,
        searchValue: e.searchValue
      };

      me.lookup.show(e.lookupId, options).then(
        result => {
          me.$emit('searchresult', result);
        },
        fault => {
          let message;
          if (fault.status === 404) {
            message = "Lookup (" + e.lookupId + ") not found."
          } else {
            message = String(fault.status) +  "Problem loading lookup (" + e.lookupId + ")."
          }
          me.advice.fault(message);
        }
      );

    },

    onFocus () {
      const
        me = this,
        t = me.theme,
        r = me.$refs;

      if (me.isPlain) {
        // if (r.inpcont) {
        //   r.inpcont.classList.add(t.outlineColor);
        // }
        // if (r.inputbox) {
        //   r.inputbox.classList.add(t.outlineColor);
        // }
        if (r.inpcont && r.inpcont.classList) {
          r.inpcont.classList.add(t.outlineColor);
        }
        if (r.inputbox && r.inputbox.classList) {
          r.inputbox.classList.add(t.outlineColor);
        }
      } else {
        if (r.ctrl) {
          r.ctrl.classList.add(t.outlineColor);
        }
        if (r.cap) {
          r.cap.classList.add(t.inputFocusColor);
        }
      }
      me.hasFocus = true;
    },

    onBlur (e) {
      const
        me = this,
        t = me.theme,
        r = me.$refs;

      if (me.isPlain) {
        // if (r.inpcont) {
        //   r.inpcont.classList.remove(t.outlineColor);
        // }
        // if (r.inputbox) {
        //   r.inputbox.classList.remove(t.outlineColor);
        // }
        if (r.inpcont && r.inpcont.classList) {
          r.inpcont.classList.remove(t.outlineColor);
        }
        if (r.inputbox && r.inputbox.classList) {
          r.inputbox.classList.remove(t.outlineColor);
        }
      } else {
        if (r.ctrl) {
          r.ctrl.classList.remove(t.outlineColor);
        }
        if (r.cap) {
          r.cap.classList.remove(t.inputFocusColor);
        }
      }
      me.hasFocus = false;

      me.$emit('lostfocus', e);

    },

    applyFieldMode () {
      const
        me = this,
        fm = me.meta,
        dom = me.dom;

      // if (fm) {
      if (fm && me.modeControl) {
        if (fm.mode === 'D') {
          dom.disable(me.$refs.inputbox);
          if (me.$refs.lkpbutton) {
            dom.disable(me.$refs.lkpbutton.$el);
          }
        } else {
          dom.enable(me.$refs.inputbox);
          if (me.$refs.lkpbutton) {
            dom.enable(me.$refs.lkpbutton.$el);
          }
        }
      }
    },

    onCopyValue () {
      // empty by design; implemented by 'subclasses'
    }

  },

  created () {
    const me = this;

    me.dom.on(document, 'focusin', me.documentFocusIn);

    me.id = 's-' + me.core.hex8();
    me.label = !me.bullet ? me.caption : '';
    if (me.fieldId) {
      const f = me.core.getFieldDef(me.fieldId);
      if (f) {
        if ('caption' in f && f.caption && !me.label && !me.bullet) {
          me.label = f.caption;
        }
        if (!('maxlength' in me.$attrs) && 'maxLength' in f) {
          me.maximumLength = f.maxLength;
        }
        if ('entityId' in f && me.core.isBoolean(f.entityId)) {
          me.isEntityId = f.entityId;
        }
        if ('noFormat' in f && me.core.isBoolean(f.noFormat)) {
          me.noFormat = f.noFormat;
        }
        if ('mask' in f && me.core.isString(f.mask)) {
          me.mask = f.mask;
        }
      }
    }

    if (!me.captionWidth) {
      me.label = '';
    }

    this.copyValue = undefined;
  },

  beforeUpdate () {
    init(this);
  },

  mounted () {
    init(this);

    setTimeout(() => {
      const page = this.core.sessionMgr.currentPage;
      if (page && page.dataConfig && page.dataConfig.keyField === this.fieldId && page.dataConfig.autoAssignKey) {
        this.isAutoId = true;
      }
    }, 100);
  },

  beforeDestroy () {
    this.dom.off(document, 'focusin', this.documentFocusIn);
  },

  watch: {
    caption (val) {
      this.label = val;
    }
  }

}

function init (me) {
  if (!me) { return; }
  const fm = me.meta;
  if (fm) {
    me.$nextTick( () => {
      fm.control = me;
      me.core.sessionMgr.currentPage.enlistField(me.fieldId, fm);
      me.applyFieldMode();
    });
  }
}

</script>
