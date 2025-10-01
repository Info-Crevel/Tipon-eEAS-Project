<template>

<div class="form-control border-main curved" ref="ctrl">
  <p class="caption px-2 light darken-1 text-center curved-top mb-0" v-if="caption" ref="cap">{{ caption }}</p>
  <div class="pos-relative">
    <img :src="src" class="preview" :class="imageClass" ref="image" :key="ts">
    <input
      type="file"
      class="input-file"
      title="Click to select image"
      :accept="accept"
      @change="onChange($event)"
      @focus="onFocus()"
      @blur="onBlur()"
      @keyup.esc.stop
      @keydown.enter.prevent
      @keydown.delete="onClear"
      ref="inputbox"
    >
  </div>
  <div class="text-right whitesmoke darken-1 curved-bottom">
    <div class="buttons py-1 pr-1" mb-0>
      <button class="info-light border-main py-1 mr-1 sm-1 " @mousedown.prevent tabindex="-1" @click="onSelect" ref="selectbutton"><i class="fa fa-search fa-fw"></i></button>
      <button class="danger-light border-main py-1 sm-1" @mousedown.prevent tabindex="-1" @click="onClear" ref="clearbutton"><i class="fa fa-close fa-fw"></i></button>
    </div>
  </div>
</div>

</template>

<script>

export default {
  model: {
    prop: 'value',
    event: 'input'
  },

  inject: {
    'injTheme': { from: 'theme' }
  },

  props: {
    colName: String,
    value: { type: String, default: '' },
    accept: { type: String, default: '.jpg' },
    caption: { type: String },
    imageClass: { type: String },
    themeId: {
      type: String,
      validator: (value) => {
        return this.core.config.themes.indexOf(value) !== -1;
      }
    }
  },

  data () {
    return {
      ts: 0,
      label: '',
      imageSource: ''
    };
  },

  computed: {
    src () {
      return this.imageSource ? this.dom.getImageSource(this.imageSource) : false;
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
        if (host) {
          return host + '-' + fieldId;
        }
      }
      return fieldId;
    },

  },

  methods: {
    onChange (e) {
      if (!e.target.files.length) {
        return;
      }

      const
        me = this,
        fileName = e.target.files[0].name;

      if (!me.hasExtension(fileName.toLowerCase(), me.validExtensions)) {
        let mssg = 'File rejected. (' + fileName + ')<hr>Valid file type: ' + me.validExtensionsText;
        me.advice.fault(mssg, 6);
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        me.imageSource = reader.result;
        me.$emit('input', me.imageSource)
        me.clearFaultTag();
        me.refresh();
      };
      reader.readAsDataURL(e.target.files[0]);
    },

    hasExtension (fileName, extensions) {
      for (let i = 0; i < extensions.length; i++) {
        if (fileName.toLowerCase().endsWith(extensions[i])) {
          return true;
        }
      }
      return false;
    },

    onSelect () {
      const box = this.$refs.inputbox;
      if (box) {
        box.focus();
        box.click();
      }
    },

    onClear () {
      const box = this.$refs.inputbox;
      if (box) {
        box.value = null;
        box.focus();
        this.imageSource = null;
        this.$emit('input', this.imageSource)
        this.refresh();
      }
    },

    refresh () {
      setTimeout(() => {
        this.ts++;
      }, 50);
    },

    getInputBox () {
      return this.$refs.inputbox;
    },

    onFocus () {
      const
        me = this,
        t = me.theme,
        r = me.$refs;

      r.ctrl.classList.add(t.outlineColor);
      r.cap.classList.add(t.inputFocusColor);
      me.hasFocus = true;
    },

    onBlur (e) {
      const
        me = this,
        t = me.theme,
        r = me.$refs;

      if (r.ctrl) {
        r.ctrl.classList.remove(t.outlineColor);
      }
      if (r.cap) {
        r.cap.classList.remove(t.inputFocusColor);
      }
      me.hasFocus = false;
      me.$emit('lostfocus', e);
    },

    applyFieldMode () {
      const
        me = this,
        fm = me.meta,
        dom = me.dom,
        refs = me.$refs;

      if (fm) {
        if (fm.mode === 'D') {
          dom.disable(refs.inputbox);
          if (refs.selectbutton) {
            dom.disable(refs.selectbutton);
          }
          if (refs.clearbutton) {
            dom.disable(refs.clearbutton);
          }
        } else {
          dom.enable(refs.inputbox);
          if (refs.selectbutton) {
            dom.enable(refs.selectbutton);
          }
          if (refs.clearbutton) {
            dom.enable(refs.clearbutton);
          }
        }
      }
    },

    setFaultTag () {
      this.dom.addAttribute(this.$refs.cap, 'fault');
    },

    clearFaultTag () {
      this.dom.removeAttribute(this.$refs.cap, 'fault');
    }

  },

  created () {
    this.label = this.caption;
    this.validExtensions = [];
  },

  beforeUpdate () {
    init(this);
  },

  mounted () {
    this.validExtensions = this.accept.split(",").map(item => item.toLowerCase().trim());
    this.validExtensionsText = this.validExtensions.join(", ");
    init(this);
  },

  watch: {
    value (newVal) {
      this.imageSource = newVal;
      this.refresh();
    }
  }

}

function init (me) {
  if (!me) { return; }
  const fm = me.meta;
  if (fm) {
    fm.control = me;
    me.core.sessionMgr.currentPage.enlistField(me.fieldId, fm);
    me.$nextTick( () => {
      me.applyFieldMode();
    });
  }

}

</script>

<style scoped>

.caption {
  padding-top: .375rem;
  padding-bottom: .375rem;
}

.input-file {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  border: none;
}

.input-file:not([disabled]) {
  cursor: pointer;
  border: 1px solid gray;
}

.input-file[disabled] {
  visibility: hidden;
}

.preview {
  display: block;
  background: dimgray;
  width: 100%;
  object-fit: cover;
}

</style>