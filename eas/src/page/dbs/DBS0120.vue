// Member Type Qualification Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0120" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

  <div slot="footer" class="action-buttons p-1">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
      <button type="button" :class="submitButtonClass" class="justify-between btn-save" @click="onSubmit">
        <i class="fa fa-save fa-lg"></i><span>Save</span>
      </button>
      <button type="button" :class="clearButtonClass" class="justify-between btn-clear" @click="onClear">
        <i class="fa fa-undo fa-lg"></i><span>Clear</span>
      </button>
      <button type="button" :class="deleteButtonClass" class="justify-between btn-delete" @click="onDelete">
        <i class="fa fa-times-circle fa-lg"></i><span>Delete</span>
      </button>
    </div>

  </div>

  <sym-int v-model="memberType.memberTypeId" :caption-width="45" :input-width="20" caption="Member Type ID" lookupId="HrsMemberType" @lostfocus="onMemberTypeIdLostFocus" @changing="onMemberTypeIdChanging" @changed="onMemberTypeIdChanged" @searchresult="onMemberTypeIdSearchResult"></sym-int>
  <sym-text v-model="memberType.memberTypeName" :caption-width="45" :input-width="60" caption="Member Type Name"></sym-text>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 w-50">
      <thead>
        <tr>
          <th class="w-58">Qualification</th>
          <th class="w-16 text-right">Sort Seq</th>
          <th class="w-26">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in memberTypeDetails" :key="index">
          <td>{{ dtl.typeQualificationName }}</td>
          <td class="text-right">{{ dtl.sortSeq }}</td>
          <td class="p-1">
            <div class="buttons" sm-1 mb-0>
              <button type="button" class="justify-between info-light fw-22 btn-dtl-edit" @click="onEditDetail(dtl, index)">
                <i class="fa fa-edit fa-lg"></i><span>Edit</span>
              </button>
              <button type="button" class="danger-light btn-dtl-delete" title="Delete" @click="onDeleteDetail(index)">
                <i class="fa fa-times fa-lg"></i>
              </button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <div class="command-buttons light border-main p-1 border-top-0 mb-2">
    <div></div>

    <div class="buttons justify-center" :info-light="isMono" :outline="isMono" border-main fw-30 mb-0 sm-1 shadow-light>
      <button type="button" class="justify-between btn-add" @click="onAddDetail">
        <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
      </button>
    </div>
  </div>

</sym-form>

<sym-modal
  id="detail-editor"
  v-model="isDetailEditorVisible"
  size="md"
  :header="true"
  :customBody="true"
  :footer="false"
  :keyboard="false"
  :dismissible="false"
  :closeOnBackButton="false"
  :backdrop="false"
  :title="editorTitle"
  @show="onShowDetailEditor($event)"
  @hide="onHideDetailEditor($event)"
  headerClass="app-form-header"
  dismissButtonClass="danger"
>

<div class="board p-1 mb-0 w-100">
  <form id="dbs0120A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-text v-model="detail.typeQualificationName" caption="Qualification" align="bottom"></sym-text>
      <sym-int v-model="detail.sortSeq" caption="Sort Seq" align="bottom" captionAlign="right"></sym-int>
    </div>

    <div class="buttons w-100 justify-end mt-3 mb-2" fw-26 shadow-soft mb-0>
      <button type="button" class="info justify-between border-main" @click="onSubmitDetail()"><i class="fa fa-check mr-2"></i>Submit</button>
      <button type="button" class="warning justify-between border-main mr-0" @click="isDetailEditorVisible=false"><i class="fa fa-times-circle fa-lg mr-2"></i>Close</button>
    </div>

  </form>
</div>

</sym-modal>

</section>
</template>

<script>

import {
  get,
  ajax
} from '../../js/http';

import {
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0120',

  data () {
    return {
      memberType: {
        memberTypeId: 0,
        memberTypeName: '',
        lockId: ''
      },

      memberTypeDetails: [],

      isDetailEditorVisible: false,

      detail: {
        typeQualificationDetailId: 0,
        memberTypeId: 0,
        typeQualificationName: '',
        sortSeq: 0,
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    detailTag () {
      return 'Qualifications: ' + this.memberTypeDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Qualification';
      }
      return 'Edit Qualification';
    }

  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getMemberType()
        .then( info => {
          me.stopWait(wait);
          if (info && info.memberType.length) {
            me.setModels(info);
          } else {
            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'Member Type <b>' + me.memberType.memberTypeId + '</b> not found.';
              me.advice.fault(mssg, { duration: 5 });
              me.onReset();
              return;
            }
          }
          me.setupControls();
          me.isFilled = true;
        })
        .catch( fault => {
          me.stopWait(wait);
          me.showFault(fault);
        })
    },

    setModels (info) {
      const
        me = this,
        memberType = info.memberType[0];

      me.memberType = memberType;
      me.memberTypeDetails = info.memberTypeDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldMemberType = JSON.stringify(me.memberType);
      me.oldMemberTypeDetails = JSON.stringify(me.memberTypeDetails);
    },

    // API calls

    getMemberType () {
      return get('api/member-types/' + this.memberType.memberTypeId);
    },

    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/member-types/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/member-types/' + this.memberType.memberTypeId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        memberType = this.memberType,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/member-types/' + memberType.memberTypeId + this.getDeleteQuery(memberType.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        memberType = {};

      Object.assign(memberType, me.memberType);
      memberType.details = me.memberTypeDetails;

      return memberType;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('memberType', 'memberTypeDetails', 'detail');
      dc.keyField = 'memberTypeId';
      dc.autoAssignKey = false;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 100);
    },

    onMemberTypeIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.memberTypeIdCallback;
    },

    memberTypeIdCallback (e) {
      e.message = "Member Type ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.HrsMemberType', 'memberTypeId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onMemberTypeIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onMemberTypeIdLostFocus () {
      const me = this;

      if (!me.memberType.memberTypeId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onMemberTypeIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.memberType.memberTypeId = data.memberTypeId;
      me.replaceUrl();
      me.loadData();
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 } )
        return;
      }

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 3 });
        me.onReset();
        return;
      }

      let
        promise,
        message,
        wait = me.wait(),
        isNew = me.isNew();

      if (isNew) {
        promise = me.createRecord();
      } else {
        promise = me.modifyRecord();
      }

      promise.then(
        (success) => {
          me.stopWait(wait);
          if (success) {
            if (isNew) {
              message = 'New document created.'
            } else {
              message = 'Document updated.'
            }
            me.setCopyData();

            if (nextRoute && !(nextRoute instanceof MouseEvent)) {
              me.dialog.success(message, { size: 'md' }).then(
                () => {
                  me.refreshOldRefs();
                  me.go(nextRoute);
                  return;
                }
              );
            } else {
              me.advice.success(message, { duration: 5 });
            }

          }

          me.onReset();
        },
        fault => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      )

    },

    onDelete () {
      const me = this;

      getSafeDeleteFlag('HrsMemberType', me.memberType.memberTypeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Document will be deleted.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" } );
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            return me.deleteRecord();
          }
          return false;
        })
        .then( success => {
          if (success) {
            me.advice.success('Document deleted.', { duration: 4 });
            me.onReset();
          }
        })
        .catch( fault => {
          me.showFault(fault);
        });
    },

    onShowDetailEditor () {
      const me = this;

      let target = 'typeQualificationName';

      me.setActiveModel('detail');

      me.setRequiredMode(
        'typeQualificationName',
        'sortSeq'
      );

      if (!me.isAdding) {
        if (me.detail.lockId) {
          target = 'sortSeq';
          me.setDisplayMode(
            'typeQualificationName'
          );
        }
      }

      setTimeout(() => {
        this.setFocus(target);
      }, 200);
    },

    onHideDetailEditor () {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
    },

    onEditDetail (dtl, index) {
      // hold current property values of selected detail for editing; passed index too.
      const d = this.detail;
      this.detailIndex = index;

      d.typeQualificationDetailId = dtl.typeQualificationDetailId;
      d.typeQualificationName = dtl.typeQualificationName;
      d.sortSeq = dtl.sortSeq;
      d.lockId = dtl.lockId;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.typeQualificationDetailId = -1;
      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      if (!me.memberTypeDetails[index].lockId) {
        me.memberTypeDetails.splice(index, 1);
        return;
      }

      getSafeDeleteFlag('HrsMemberTypeQualification', me.memberTypeDetails[index].memberTypeId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Qualification <b>' + me.memberTypeDetails[index].typeQualificationName + '</b> will be removed from the list.<br><br>Continue?', { size: 'rg', scheme: 'warning' });
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot remove document at this time.", { duration: 5} );
            return '';
          }
        })
        .then( reply => {
          if (reply === 'yes') {
            me.memberTypeDetails.splice(index, 1);
          }
          return;
        })
        .catch( fault => {
          me.showFault(fault);
        });

    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      if (!d.typeQualificationName) {
        me.isDetailEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAdding) {
        Object.assign(dtl, d);

        dtl.memberTypeId = me.memberType.memberTypeId
        me.memberTypeDetails.push(dtl);

        me.clearDetailPad();
        me.detail.typeQualificationDetailId = -1;
        me.advice.info("Qualification '" + dtl.typeQualificationName + "' added to list.", { duration: 5 });
        // me.setFocus('typeQualificationName');
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('typeQualificationName');
        }, 100);
      } else {
        dtl = me.memberTypeDetails[me.detailIndex];

        // dtl.platformId = d.platformId;
        dtl.typeQualificationName = d.typeQualificationName;
        dtl.memberTypeId = me.memberType.memberTypeId;
        dtl.sortSeq = d.sortSeq;
        dtl.lockId = d.lockId;

        me.isDetailEditorVisible = false;
      }

    },

    // helpers

    setupControls () {
      const me = this;

      setTimeout(() => {
        me.enableElement(
          'btn-add'
        );

        me.setDefaultControlStates();

        me.setDisplayMode(
          'memberTypeName'
        );

        me.setFocus('memberTypeName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.memberType) !== me.oldMemberType) { return true; }
      if (JSON.stringify(me.memberTypeDetails) !== me.oldMemberTypeDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.typeQualificationDetailId = 0;
      d.typeQualificationName = '';
      d.sortSeq = 0;
      d.lockId = '';
    }

  },

  created () {
    const me = this;

    me.oldMemberType = '';
    me.oldMemberTypeDetails = '';
  }

}

</script>

<style scoped>

.action-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.command-buttons {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 1fr 1fr 1fr;
  gap: .75rem;
}

.detail-editor-boxes {
  display: grid;
  grid-auto-flow: column;
  grid-template-columns: 3fr 1fr;
  gap: .5rem;
}

.fixed-header {
  /* overflow-y: auto; */
  overflow: auto;
  max-height: 50vh;
}

.fixed-header th {
  position: sticky;
  top: 0;
  background: rgb(221, 221, 176);
  box-shadow: inset 1px 1px silver, 1px 0 silver;
}

.fixed-header thead {
  z-index: 1;
  position: relative;
}

.buttons {
  flex-wrap: nowrap;
}

@media (max-width: 1200px) {
  .fixed-header table {
    width: 65%;
  }
}

@media (max-width: 900px) {
  .fixed-header table {
    width: 75%;
  }
}

@media (max-width: 800px) {
  .fixed-header table {
    width: 85%;
  }
}

@media (max-width: 700px) {
  .fixed-header table {
    width: 100%;
  }

  .container {
    padding: 0;
    margin: 0;
    max-width: 100%;
  }

  #dbs0120 >>> .card-body {
    padding: .5rem;
  }
}

</style>