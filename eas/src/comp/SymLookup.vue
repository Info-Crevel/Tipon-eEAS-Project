<template>

<sym-modal
  :id="formId"
  ref="modal"
  v-model="show"
  :scheme="scheme"
  :size="formSize"
  :title="formTitle"
  :header="!!title"
  :backdrop="closeOnBackdropClick"
  :type="type"
  :dismissible="dismissible"
  dismiss-button-class="danger"
  bodyClass="gainsboro p-3"
  :transitionDuration="transitionDuration"
  :footer="footer"
  :keyboard="keyboard"
  @hide="onHide">

  <div v-if="!list.length && params.hasQuery">
    <div class="query" ref="query">
      <div v-for="(field, index) in params.query" :key="index" class="query-box shadow-light">
        <sym-text v-if="field.datatype === 'S' && !field.comboId" :isLookupQuery="true" v-model="query[field.column]" :colName="field.column" align="bottom" :caption="field.caption" :ref="'q' + index"></sym-text>
        <sym-int v-if="field.datatype === 'I' && !field.comboId" :isLookupQuery="true" v-model="query[field.column]" :colName="field.column" align="bottom" :caption="field.caption" :ref="'q' + index"></sym-int>
        <sym-date v-if="field.datatype === 'D' && !field.comboId" :isLookupQuery="true" v-model="query[field.column]" :colName="field.column" align="bottom" :caption="field.caption" :ref="'q' + index"></sym-date>
        <sym-check v-if="field.datatype === 'B' && !field.comboId" :isLookupQuery="true" v-model="query[field.column]" :colName="field.column" align="bottom" :caption="field.caption" :ref="'q' + index"></sym-check>
        <sym-combo v-if="field.comboId" v-model="query[field.column]" :colName="field.column" align="bottom" :caption="field.caption" :ref="'q' + index" :display-field="getComboDisplayField(field.comboId)" :datasource="comboData[field.comboId]" :key="tsCombo"></sym-combo>
      </div>
    </div>
  </div>

  <div v-if="!list.length && params.hasQuery" slot="footer">
    <div class="text-right">
      <!-- <div class="buttons" :info-light="isMono" :outline="isMono" border-main fw-28 mb-0 shadow-soft> -->
      <div class="buttons" :info-light="isMono" :outline="isMono" border-main sm-1 mb-0 shadow-soft>
        <button :class="searchButtonClass" class="justify-between" @click="onSearch">
          <i class="fa fa-play fa-lg mr-2"></i><span>Search</span>
        </button>
        <button :class="clearButtonClass" class="justify-between" @click="onClear" v-if="params.query.length > 1">
          <i class="fa fa-undo fa-lg mr-2"></i><span>Clear</span>
        </button>
        <button :class="closeButtonClass" class="justify-between" @click="toggle(false)">
          <i class="fa fa-times fa-lg mr-2"></i><span>Close</span>
        </button>
      </div>
    </div>
  </div>

  <div v-show="list.length" class="fixed-header">
    <sym-table class="lookup bill hover ellipsis shadow" colorClass="darkslategray">
      <sym-table-header slot="header">
        <tr class="align-top">
          <!-- <th v-for="(field, index) in params.lookup" :key="index" :class="getColumnClass(field)" :title="field.caption">{{ field.caption }}</th> -->
          <th v-for="(field, index) in params.lookup" :key="index" :class="getHeaderColumnClass(field)" :title="field.caption">{{ field.caption }}</th>
          <th class="text-center"></th>
        </tr>
      </sym-table-header>

      <sym-table-body slot="body">
        <tr v-for="(item, index) in list" :key="index" class="align-top" :class="getRowClass(item)" @click="onSelectItem(item)" ref="item">
          <!-- <td v-for="(field, colIndex) in params.lookup" :key="colIndex" :class="getColumnClass(field)" :title="getCellTip(item, field)">{{ getCellContent(item, field) }}</td> -->
          <td v-for="(field, colIndex) in params.lookup" :key="colIndex" :class="getDetailColumnClass(field)" :title="getCellTip(item, field)">{{ getCellContent(item, field) }}</td>
          <!-- removed select button - 01 Oct 2022 - EMT -->
          <!-- <td class="text-center p-1"><button type="button" class="success border-main w-100 text-center mb-0 shadow-soft" @click.stop="onSelectItem(item)"><i class="fa fa-check-circle fa-lg"></i></button></td> -->
        </tr>
      </sym-table-body>
    </sym-table>
  </div>

  <div v-show="list.length" slot="footer">
    <div class="buttons d-flex justify-between" :info-light="isMono" :outline="isMono" border-main mb-0 shadow sm-1>
      <button :class="searchButtonClass" class="justify-between" @click="onSearchAgain" v-if="params.hasQuery">
        <i class="fa fa-play fa-lg fa-rotate-180 mr-2"></i><span>Search Again</span>
      </button>
      <!-- <span class="border-0" v-if="!params.hasQuery"></span> -->
      <span class="border-0" v-if="!params.hasQuery" disabled></span>
      <button :class="closeButtonClass" class="justify-between" @click="toggle(false)">
        <i class="fa fa-times fa-lg mr-2"></i><span>Close</span>
      </button>
    </div>
  </div>

</sym-modal>

</template>

<script>

import {
  getList
} from '../js/dbSys';

export default {
  props: {
    lookupId: String,
    triggerControl: { type: Object },
    params: { type: Object, required: true },
    scheme: String,
    title: String,
    dismissible: { type: Boolean, default: true },
    buttonScheme: { type: String, default: 'color' },   // color, mono
    type: { type: Number, default: 4 },
    // size: { type: String, default: '' },
    lookupSize: { type: String, default: 'rg' },
    querySize: { type: String, default: 'rg' },
    footer: { type: Boolean, default: true },
    searchValue: undefined,
    keyboard: { type: Boolean, default: true },
    transitionDuration: { type: Number, default: 100 },
    callback: { type: Function, required: true }
  },

  data () {
    return {
      show: false,
      query: {},
      list: [],
      comboData: {},
      tsCombo: 0
    };
  },

  computed: {
    formTitle () {
      if (this.list.length) {
        return '<i class="fa fa-file-text-o mr-2"></i>' + (this.params.lookuptitle ? this.params.lookuptitle : this.title);
      } else {
        return '<i class="fa fa-search mr-2"></i>' + (this.params.querytitle ? this.params.querytitle : this.title);
      }
    },

    closeOnBackdropClick () {
      return this.core.isDefined(this.backdrop) ? Boolean(this.backdrop) : true;
    },

    searchButtonClass () {
      return this.isMono ? false : 'info-light';
    },

    clearButtonClass () {
      return this.isMono ? false : 'primary-light';
    },

    closeButtonClass () {
      return this.isMono ? false : 'danger-light';
    },

    isMono () {
      return (this.buttonScheme === 'mono');
    },

    formId () {
      if (this.lookupId) {
        return this.lookupId.toLowerCase();
      }
      return 'lookup';
    },

    formSize () {
      if (this.list.length) {
        return this.lookupSize;
      }
      return this.querySize;
    }

  },

  methods: {
    toggle (show) {
      this.$refs.modal.toggle(show);
    },

    onHide () {
      this.callback(this.result);
    },

    getHeaderColumnClass (field) {
      return this.getColumnClass(field);
    },

    getDetailColumnClass (field) {
      let cls = this.getColumnClass(field);
      if (field.datatype === 'B') {
        return cls + ' bold';
      }
      return cls;
    },

    getColumnClass (field) {
      let
        tags = this.tags,
        cls = 'w-' + String(field.width);

      if (field.align) {
        switch (field.align) {
          case 'R':
            return cls + ' text-right';

          case 'C':
            return cls + ' text-center';

          default:
            return cls + ' text-left';
        }
      }

      switch (field.datatype) {
        case tags.DataTypeId.Integer:
        case tags.DataTypeId.Decimal:
          return cls + ' text-right';

        case tags.DataTypeId.Boolean:
          return cls + ' text-center';

        default:
          return cls;
      }
    },

    getRowClass (item) {
      const me = this;
      if (me.params.key && me.searchValue && item[me.params.key] === me.searchValue) {
        return 'darktan';
      }
      return false;
    },

    // getCellContent (listItem, field) {
    //   return listItem[field.column];
    // },

    getCellContent (listItem, field) {
      let value = listItem[field.column];
      if (field.datatype === 'B' && typeof value === 'boolean') {
        if (value) {
          return '√';
          // return '#';
        } else {
          return '';
        }
      }
      if (field.datatype === 'F' && typeof value === 'number') {
        return this.core.toFinancialFormat(value);
      }
      if (field.datatype === 'D') {
        if ('format' in field && field.format) {
          return this.core.toDateFormat(value, true, field.format);
        }
        return this.core.toDateFormat(value);
      }

      return value;
    },

    getCellTip (listItem, field) {
      if (this.tooltipFields.findIndex( fld => fld === field.column) > -1) {
        return listItem[field.column];
      }
      return '';
    },

    onSearch () {
      const
        me = this,
        p = me.params;

      let
        promise,
        filter = p.filter,
        queryFilter = '';

      if (me.params.hasQuery) {
        queryFilter = me.getQueryFilter();
        if (me.params.criteriarequired && !queryFilter)  {
          me.advice.fault('Search criteria required.');
          me.goTop();
          return;
        }
      }

      const e = new CustomEvent('searchfill');

      e.sort = p.sort;
      e.filter = queryFilter;
      me.dom.cursorWait();

      if (me.triggerControl) {
        e.fillPromise = null;
        e.datasource = p.datasource;
        e.fields = me.fields;
        me.triggerControl.$emit('searchfill', e);
        if (e.fillPromise) {
          promise = e.fillPromise;
        }
      }

      if (e.filter) {
        filter = filter > '' ? filter + ' AND (' + e.filter + ')' : e.filter;
      }

      if (!promise) {
        promise = getList(p.datasource, me.fields, e.sort, filter);
      }

      promise.then(
        list => {
          me.dom.cursorDefault();
          if (list) {
            me.list = list;

            setTimeout(() => {
              me.scrollCurrentItemIntoView()
            }, 100);
          }
          if (!list || !list.length) {
            me.advice.warning('No matches found.');

            // 11 Apr 2023 - EMT
            if (!me.params.hasQuery) {
              me.toggle(false);
              return;
            }
            me.goTop();
          }
        },
        () => {
          me.dom.cursorDefault();
        }
      );
    },

    scrollCurrentItemIntoView () {
      const me = this;
      let index;

      if (!me.searchValue) {
        return;
      }

      index = me.list.findIndex(obj => obj[me.params.key] === me.searchValue);
      if (index > -1) {
        let el = me.$refs.item[index];
        me.dom.scrollIntoView(el);
      }
    },

    getQueryFilter () {
      const
        me = this,
        tags = me.tags;

      let
        filter = '',
        queryField,
        fieldVal,
        datatype,
        expr;

      for (const propName in me.query) {
        fieldVal = me.query[propName];
        if (fieldVal) {
          queryField = me.core.getArrayItem(me.params.query, 'column', propName);

          if (queryField && 'datatype' in queryField) {
            datatype = queryField.datatype;

            switch (datatype) {
              case tags.DataTypeId.Integer:
              case tags.DataTypeId.Decimal:
                expr = propName + '=' + String(fieldVal);
                break;

              case tags.DataTypeId.Date:
                expr = propName + "='" + fieldVal.toDateFormatISO() + "'";
                break;

              case tags.DataTypeId.Boolean:
                expr = propName + "=1";
                break;

              default:
                fieldVal = fieldVal.replace(/'/g,"''")  // sanitize input
                if (fieldVal.startsWith('!')) {
                  expr = propName + " LIKE '%" + fieldVal.substring(1) + "%'";
                } else {
                  expr = propName + " LIKE '" + fieldVal + "%'";
                }
                break;
            }

            filter = filter > '' ? filter + ' AND (' + expr + ')' : expr;
          }
        }
      }
      return filter;
    },

    onClear () {
      this.resetQueryFields();
      this.goTop();
    },

    resetQueryFields () {
      const
        me = this,
        tags = me.tags;

      let fieldVal;

      me.params.query.forEach( field => {
        switch (field.datatype) {
          case tags.DataTypeId.Integer:
          case tags.DataTypeId.Decimal:
            fieldVal = 0;
            break;

          case tags.DataTypeId.Boolean:
            fieldVal = false;
            break;

          case tags.DataTypeId.Date:
          case tags.DataTypeId.Time:
            fieldVal = null;
            break;

          default:
            fieldVal = '';
            break;
        }

        me.$set(me.query, field.column, fieldVal);
      });
    },

    goTop () {
      setTimeout(() => {
        const refs = this.$refs;
        if (refs.q0 && refs.q0.length) {
          refs.q0[0].setInputFocus();
        }
      }, 100);
    },

    onSearchAgain () {
      const me = this;

      me.list = [];
      me.onClear();
      me.setQueryBoxWidths();
    },

    onSelectItem (item) {
      const me = this;

      if (me.params.multipleselect) {
        return;
      }

      let selected = {};
      Object.assign(selected, item);

      me.result = [];
      me.result.push(selected);
      me.show = false;
    },

    getComboDisplayField (comboId) {
      let field = this.params.combo.find( obj => obj.column === comboId);
      return field.display;
    },

    getQueryDataType (comboId) {
      let field = this.params.query.find( obj => obj.comboId === comboId);
      if (field) {
        return field.datatype;
      }
      return 'S';
    },

    setQueryBoxWidths () {
      const
        me = this,
        refs = me.$refs;

      //
      // build query column widths for CSS
      //
      let queryColWidths = '';

      me.params.query.forEach( field => {
        queryColWidths = queryColWidths + field.width.toString() + 'fr ';
      });

      setTimeout(() => {
        if (queryColWidths && refs.query) {
          refs.query.style.gridTemplateColumns = queryColWidths;
        }
      }, 20);
    }

  },

  created () {
    const me = this;

    me.fields = '';
    me.result = null;
    me.tooltipFields = [];

    if (me.params.hasCombo) {
      me.params.combo.forEach( combo => {
        me.comboData[combo.key] = [];
      });
    }

    me.params.lookup.forEach( field => {
      if (field.tip) {
        me.tooltipFields.push(field.column);
      }
    });

  },

  mounted () {
    const me = this;

    if (me.params.hasCombo) {
      let fields = '';
      me.params.combo.forEach( combo => {
        fields = combo.key;
        if (combo.key !== combo.display) {
          fields = fields + ',' + combo.display
        }

        getList(combo.datasource, fields, combo.sort, combo.filter).then(
          data => {
            let
              dataObj = {},
              datatype = me.getQueryDataType(combo.column);

            switch (datatype) {
              case me.tags.DataTypeId.Integer:
                dataObj[combo.key] = 0;
                break;

              default:
                dataObj[combo.key] = '';
            }

            dataObj[combo.display] = '';
            data.unshift(dataObj);

            me.comboData[combo.key] = data;
            me.tsCombo += 1;
          }
        );
      });
    }

    let isKeyFieldIncluded = false;

    //
    // build column/field list
    //
    me.fields = '';
    me.params.lookup.forEach( field => {
      me.fields = me.fields + ((me.fields ? ',' : '') + field.column);
      if (field.column === me.params.key) {
        isKeyFieldIncluded = true;
      }
    });

    if (!isKeyFieldIncluded) {
      me.fields = me.params.key + ',' + me.fields;
    }

    //
    // add extra columns if specified
    //
    if (me.params.extra) {
      const
        colList = me.fields.split(','),
        extraList = me.params.extra.split(',').map( item => { return item.trim()}),
        combined = colList.concat(extraList);

      //
      // remove possible duplicate column name(s)
      //
      me.fields = combined.filter( (item, index) => { return combined.indexOf(item) === index});
    }

    if (me.params.hasQuery) {
      me.resetQueryFields()
      me.setQueryBoxWidths();
      me.goTop();
    } else {
      me.onSearch();
    }

  }

}

</script>

<style scoped>

.query {
  display: grid;
  grid-auto-flow: column;
  gap: .5rem;
}

.lookup {
  margin-bottom: 0;
}

.lookup.hover tbody tr:hover {
  background: rgba(46, 139, 87, .8);   /* seagreen */
  color: white;
  cursor: pointer;
}

.query-box >>> .form-control {
  margin-bottom: 0;
}

.fixed-header {
  overflow-y: auto;
  max-height: 70vh;
}

.fixed-header >>> th {
  position: sticky;
  top: 0;
  background: rgb(211, 211, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
}

/* .fixed-header >>> td {
  padding-top: .375rem;
  padding-bottom: .375rem;
} */

</style>