<template>
<div class="['drop-zone border-secondary curved', sizeClass]" :disabled="isUploading">
  <input type="file" class="input-file" title="" :multiple="multiple" :accept="accept" @change="onChange($event)" ref="input">
  <div class="text-center" v-show="isDropZoneVisible">
    <div class="button w-100 text-center d-inline-flex align-items-center p-2" :class="inputClass">
      <i class="fa" :class="iconClass"></i>
      <p class="text-left lg-2 mb-0 ml-1 mr-4" v-html="instructionsText"></p>
    </div>
  </div>

  <div class="box-border beige mb-2 mt-3" v-show="fileCount">
    <div class="pos-relative mb-2 mt-1">
      <!-- <button type="button" class="w-100 lg-2 text-center" :class="uploadButtonClass" @click="onUpload" :disabled="!fileCount" ref="upload"><i class="fa fa-upload fa-lg mr-2" :class="uploadIconClass"></i>{{ uploadButtonCaption }}</button> -->
      <button type="button" class="w-100 lg-2 text-center" :class="uploadButtonClass" @click="onUpload" ref="upload"><i class="fa fa-upload fa-lg mr-2" :class="uploadIconClass"></i>{{ uploadButtonCaption }}</button>
    </div>
    <div class="d-flex justify-between align-items-center pos-relative mb-2">
      <span class="lg-2 pl-2"><span class="bold display-8">{{ fileCount }}</span> {{ getSelectedText(fileCount) }}</span>
      <button type="button" class="fw-26" :class="resetButtonClass" @click="onReset"><i class="fa fa-undo mr-2"></i>Reset</button>
    </div>
  </div>

  <ul class="mb-2" v-show="fileCount">
    <li class="list-item text-dark" v-for="(file, index) in fileList" :key="index">
      <div class="d-flex justify-between align-items-center lg-1">
        <span>{{ file.name }} ● {{ core.toPrettyFileSize(file.size) }}</span>
        <button type="button" title="Remove file" class="dark outline shadow-light sm-3 mb-0 ml-2" @click="onRemoveFile(index)" v-if="multiple"><i class="fa fa-times"></i></button>
      </div>
    </li>
  </ul>
</div>
</template>

<script>

const
  instructionsSingle = 'Drag file here or click to browse',
  instructionsMultiple = 'Drag file(s) here<br>or click to browse';

export default {
  props: {
    accept: { type: String, default: '.pdf' },
    multiple: { type: Boolean, default: false },
    instructions: { type: String, default: ''},
    inputClass: { type: String, default: 'info shadow'},
    uploadButtonText: '',
    uploadButtonClass: '',
    resetButtonClass: '',
    iconClass: { type: String, default: 'fa-plus-circle fa-fw fa-3x'},
    blinkUploadIcon: { type: Boolean, default: false },
    size: String,     // sm, md, rg, lg, xl
  },

  data () {
    return {
      fileList: [],
      isUploading: false,
      validExtensions: []
    }
  },

  computed: {

    sizeClass () {
      return {
        [`upload-${this.size}`]: Boolean(this.size)
      };
    },


    fileCount () {
      return this.fileList.length;
    },

    instructionsText () {
      return this.instructions ? this.instructions : this.multiple ? instructionsMultiple : instructionsSingle
    },

    uploadIconClass () {
      return this.blinkUploadIcon ? 'fa-blink' : false
    },

    uploadButtonCaption () {
      return this.uploadButtonText ? this.uploadButtonText : 'Upload Now';
    },

    isDropZoneVisible () {
      return (this.multiple || (!this.multiple && !this.fileCount));
    }

  },

  methods: {
    onChange (e) {
      if (!e.target.files.length) {
        return;
      }

      const
        me = this,
        candidateList = Array.from(e.target.files);

      let newList = [];

      e.target.value = "";

      if (me.fileList.length) {
        let
          found = false,
          candidate = null;

        for (let c = 0; c < candidateList.length; c++) {

          candidate = candidateList[c];
          found = false;

          for (let f = 0; f < me.fileList.length; f++) {
            if (me.isSameFile(candidate, me.fileList[f])) {
              found = true;
              break;
            }
          }
          if (found) {
            let mssg = 'File omitted; already in the list.<hr>' + candidate.name;
            me.advice.fault(mssg, 8);
          } else {
            newList.push(candidate);
          }
        }

      } else {
        newList = candidateList;
      }

      // check for correct extension/file type
      // loop in reverse, so splice works

      for (let i = newList.length - 1; i >= 0; i--) {
        if (!me.isValidType(newList[i].type, me.validExtensions)) {
          let mssg = 'File rejected. (' + newList[i].name + ')<hr>Valid file type: ' + me.validExtensionsText;
          me.advice.fault(mssg, 8);
          newList.splice(i, 1);
        }
      }

      if (!newList.length) {
        return;
      }

      // const evt = new CustomEvent('select', { cancelable: false, bubbles: false });
      // evt.files = newList;

      // me.$emit('select', evt);

      // if (Array.isArray(evt.files) && evt.files.length) {
      //   me.fileList = me.fileList.concat(evt.files);
      // }

      me.emitSelectEvent(newList).then(
        e => {
          if (e.defaultPrevented) {
            return;
          }

          if (Array.isArray(e.files) && e.files.length) {
            // me.fileList = me.fileList.concat(e.files);
            if (me.multiple) {
              me.fileList = me.fileList.concat(e.files);
            } else {
              me.fileList = e.files;
            }
          }

          me.emitSelectedChanged();
        }
      );

    },

    async emitSelectEvent (newList) {
      const e = new CustomEvent('select', { cancelable: true, bubbles: false });
      e.files = newList;

      this.$emit('select', e);

      if (this.core.isFunction(e.callback)) {
        let result = await e.callback(e);
        if (!result) {
          e.preventDefault();
        }
      }
      return e;
    },

    onUpload () {
      const
        me = this,
        e = new CustomEvent('upload', { cancelable: false, bubbles: false });

      me.isUploading = true;

      e.fileList = me.fileList;
      e.reset = () => {
        me.fileList = [];
        me.isUploading = false;
      };

      e.showUploadResponse = me.showUploadResponse;

      this.$emit('upload', e);
    },

    onReset () {
      this.fileList = [];
      this.emitSelectedChanged();
    },

    isSameFile (file1, file2 ) {
      return file1.name === file2.name &&
             file1.lastModified === file2.lastModified &&
             file1.size === file2.size &&
             file1.type === file2.type
    },

    isValidType (type, extensions) {
      for (let i = 0; i < extensions.length; i++) {
        switch (extensions[i]) {
          case '.jpg':
            if (type === 'image/jpeg') { return true; }
            break;

          case '.png':
            if (type === 'image/png') { return true; }
            break;

          case '.gif':
            if (type === 'image/gif') { return true; }
            break;

          case '.pdf':
            if (type === 'application/pdf') { return true; }
            break;

          case '.tif', '.tiff':
            if (type === 'image/tiff') { return true; }
            break;

          case '.txt':
            if (type === 'text/plain') { return true; }
            break;

          case '.xml':
            if (type === 'text/xml') { return true; }
            break;

          default:
            break;
        }
      }
      return false;
    },

    showUploadResponse (info) {
      if (!info || (!info.createdCount && !info.failedCount)) {
        return Promise.resolve();
      }

      let html = '';
      if (info.createdCount) {
        html = info.createdCount.toString() + ' file(s) uploaded<hr>';
      } else {
        html = info.failedCount.toString() + ' file(s) rejected<hr>';
      }
      info.details.forEach( detail => {
        if (info.createdCount) {
          if (detail.statusCode === 201) {
            html = html + detail.fileName + '<br>';
          }
        } else {
          if (detail.statusCode === 415) {
            html = html + detail.fileName + '<br>';
          }
        }
      })

      if (info.createdCount) {
        return this.dialog.success(html, { size: this.size});
      } else {
        return this.dialog.fault(html, { size: this.size});
      }
    },

    getSelectedText (fileCount) {
      return fileCount === 1 ? 'file selected' : 'files selected'
    },

    emitSelectedChanged () {
      this.$emit('selectedchanged', this.fileList);
    },

    onRemoveFile (index) {
      this.fileList.splice(index, 1);
      this.emitSelectedChanged();
    },

    invokeClick () {
      this.$refs.input.click();
    },

    invokeUpload () {
      this.$refs.upload.click();
    }

  },

  mounted () {
    this.validExtensions = this.accept.split(",").map(item => item.toLowerCase().trim());
    this.validExtensionsText = this.validExtensions.join(", ");
  }

}

</script>

<style scoped>

.input-file {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
}

.drop-zone {
  position: relative;
}

.drop-zone ul {
  list-style-type: none;
}
/* jyl */
.upload-sm {
  max-width: 300px; 
}

.upload-md {
  max-width: 500px;
}

.upload-lg {
  max-width: 700px;
}

.upload-xl {
  max-width: 900px;
}
/* jyl */
@media (max-width: 576px) {
  .drop-zone {
    padding: .25rem !important;
  }
  
  .drop-zone .list-item {
    padding-left: .5rem;
    padding-right: .5rem;
  }
}

</style>