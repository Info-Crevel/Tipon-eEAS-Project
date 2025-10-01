// Skill Set Setup

<template>
<section class="container p-0" :key="ts">
<sym-form id="dbs0130" :caption="pageName" headerClass="app-form-header" footerClass="border-top-main app-form-footer py-0" bodyClass="app-form-body pb-3">

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

  <div class="d-flex justify-between align-items-end fw-96">
    <sym-int v-model="skillSet.skillSetId" :caption-width="35" :input-width="20" caption="Skill Set ID" lookupId="DbsSkillSet" @lostfocus="onSkillSetIdLostFocus" @changing="onSkillSetIdChanging" @changed="onSkillSetIdChanged" @searchresult="onSkillSetIdSearchResult"></sym-int>
    <div class="buttons d-inline">
      <button class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2" :tabindex="-1" @mousedown.prevent @click="onNew"><i class="fa fa-file-o"></i>New</button>
    </div>
  </div>

  <sym-text v-model="skillSet.skillSetName" :caption-width="35" :input-width="100" caption="Skill Set Name"></sym-text>
  <sym-int v-model="skillSet.sortSeq" :caption-width="35" :input-width="20" caption="Sort Seq"></sym-int>

  <div class="text-center border-light curved p-1 slategray mt-2">
    <span class="serif lg-3">{{ detailTag }}</span>
  </div>

  <div class="d-flex justify-centerXXX fixed-header" ref="list">
    <table class="striped-even mb-0 w-50">
      <thead>
        <tr>
          <th class="w-58">Skill Name</th>
          <th class="w-16 text-right">Sort Seq</th>
          <th class="w-26">Action</th>
        </tr>
      </thead>
      <tbody class="white">
        <tr v-for="(dtl, index) in skillSetDetails" :key="index">
          <td>{{ dtl.skillName }}</td>
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
  size="rg"
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
  <form id="dbs0130A" class="curved-bottom border-dark marianblue p-3" @submit.prevent>
    <div class="detail-editor-boxes">
      <sym-int v-model="detail.skillId" caption="Skill ID" align="bottom" lookupId="DbsSkill" @changing="onSkillIdChanging" @searchresult="onSkillIdSearchResult"></sym-int>
      <sym-text v-model="detail.skillName" caption="Skill Name" align="bottom"></sym-text>
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
  getList,
  getSafeDeleteFlag
} from '../../js/dbSys';

import
  PageMaintenance
from '../PageMaintenance.vue';

export default {
  extends: PageMaintenance,
  name: 'dbs0130',

  data () {
    return {
      skillSet: {
        skillSetId: 0,
        skillSetName: '',
        sortSeq: 0,
        lockId: ''
      },

      skillSetDetails: [],

      isDetailEditorVisible: false,

      detail: {
        skillDetailId: 0,
        skillId: 0,
        skillName: '',
        sortSeq: 0,
        lockId: ''
      },

      detailIndex: -1,
      isAdding: false

    };
  },

  computed: {
    detailTag () {
      return 'Skills: ' + this.skillSetDetails.length;
    },

    editorTitle () {
      if (this.isAdding) {
        return 'Add Skill Detail';
      }
      return 'Edit Skill Detail';
    }
  },

  methods: {

    loadData () {
      const
        me = this,
        wait = me.wait();

      me.getReferences()
        .then( data => {
          if (!me.core.isBoolean(data)) {
            me.skills = data.skills;
          }
           if (me.skillSet.skillSetId < 0) {
            return Promise.resolve(null);
          }
          return me.getSkillSet();
        })
        .then( info => {
          me.stopWait(wait);
          if (info && info.skillSet.length) {
            me.setModels(info);
          } else {
            if (me.skillSet.skillSetId > -1) {
              let message = "Skill Set ID '<b>" + me.skillSet.skillSetId + "</b>' not found.";
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }

            if (me.canAdd) {
              me.advice.warning('You are adding a new document.', { duration: 5 });
            } else {
              let mssg = 'You are not allowed to create new documents.';
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
      const me = this;

      me.skillSet = info.skillSet[0];
      me.skillSetDetails = info.skillSetDetails;

      me.refreshOldRefs();
    },

    refreshOldRefs () {
      const me = this;

      me.oldSkillSet = JSON.stringify(me.skillSet);
      me.oldSkillSetDetails = JSON.stringify(me.skillSetDetails);
    },

    // API calls

    getSkillSet () {
      return get('api/skill-sets/' + this.skillSet.skillSetId);
    },

    getReferences () {
      const me = this;

      if (me.skills.length) {

        return Promise.resolve(true);
      }
      return get('api/references/dbs0130');
    },


    createRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('POST', body);

      return ajax('api/skill-sets/' + currentUserId, options);
    },

    modifyRecord () {
      const
        payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions('PUT', body);

      return ajax('api/skill-sets/' + this.skillSet.skillSetId + '/' + currentUserId, options);
    },

    deleteRecord () {
      const
        skillSet = this.skillSet,
        options = this.core.getAjaxOptions('DELETE');

      return ajax('api/skill-sets/' + skillSet.skillSetId + this.getDeleteQuery(skillSet.lockId), options);
    },

    getApiPayload () {
      const
        me = this,
        skillSet = {};

      Object.assign(skillSet, me.skillSet);
      skillSet.details = me.skillSetDetails;

      return skillSet;
    },

    // event handlers

    onLoad () {
      const
        me = this,
        dc = me.dataConfig;

      dc.models.push('skillSet', 'skillSetDetails', 'detail');
      dc.keyField = 'skillSetId';
      dc.autoAssignKey = true;
    },

    onResetAfter () {
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement(
          'btn-add'
        );
      }, 50);
    },

    onSkillSetIdChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.skillSetIdCallback;
    },

    skillSetIdCallback (e) {
      e.message = "Skill Set ID '<b>" + e.proposedValue + "</b>' not found.";

      return this.isValidEntity('dbo.DbsSkillSet', 'skillSetId', e.proposedValue).then(
        result => {
          return result;
        }
      );
    },

    onSkillSetIdChanged () {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onSkillSetIdLostFocus () {
      const me = this;

      if (!me.skillSet.skillSetId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onSkillSetIdSearchResult (result) {
      if (!result) { return; }

      const
        me = this,
        data = result[0];

      me.skillSet.skillSetId = data.skillSetId;
      me.replaceUrl();
      me.loadData();
    },

    onSkillIdChanging (e) {
      e.message = "Skill ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";
      e.callback = this.skillIdCallback;
    },

    skillIdCallback (e) {
      const me = this;
      let filter = "SkillId=" + e.proposedValue;
      return getList('dbo.DbsSkill', 'SkillId, SkillName', '', filter).then(
        skill => {
          if (skill && skill.length) {
            // 05 Feb 2025 - EMT (no duplicates)
            let index = me.skillSetDetails.findIndex(obj => obj.skillId === e.proposedValue);
            if (index > -1) {
              e.message = 'Skill <b>' + me.skillSetDetails[index].skillName + '</b> is already in the list.'
              return false;
            }
            me.detail.skillName = skill[0].skillName;

            setTimeout(() => {
              me.setFocus('sortSeq');
            }, 50);

            return true;
          }
          return false;
        },
        fault => {
          me.showFault(fault);
          return false;
        }
      );
    },

    onSkillIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        detail = this.detail;

      // 05 Feb 2025 - EMT (no duplicates)
      let index = this.skillSetDetails.findIndex(obj => obj.skillId === data.skillId);
      if (index > -1) {
        let message = 'Skill <b>' + this.skillSetDetails[index].skillName + '</b> is already in the list.'
        this.advice.fault(message, { duration: 5 })
        return;
      }

      detail.skillId = data.skillId;
      detail.skillName = data.skillName;

      this.focusNext();
    },

    onSubmit (nextRoute) {
      const me = this;

      if (!me.isValid(me.$options.name)) {
        me.advice.fault('Fill in the required fields (marked in red) before saving.', { duration: 5 })
        return;
      }

      if (!me.skillSetDetails.length) {
        me.advice.fault('Fill in the Details before saving.', { duration: 5 })
        return;
      }

      if (!me.hasChanges()) {
        me.advice.success('Document updated.', { duration: 5 });
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
            // set auto-generated ID returned by API so F9 works
            if (isNew && typeof success === 'number' && success > 0) {
              me.skillSet.skillSetId = success;
            }

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

      getSafeDeleteFlag('DbsSkillSet', me.skillSet.skillSetId)
        .then( safe => {
          if (safe) {
            return me.dialog.confirm('Document will be deleted.<br><br>Continue?', { scheme: 'warning', icon: 'warning', size: "md" } );
          } else {
            me.advice.fault("Delete attempt failed.<hr>Cannot delete document at this time.", { duration: 5});
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

      me.setActiveModel('detail');

      me.setRequiredMode(
        'skillId',
        'sortSeq'
      );

      me.setDisplayMode(
        'skillName'
      );

      setTimeout(() => {
        this.setFocus('skillId');
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

      d.skillDetailId = dtl.skillDetailId;
      d.skillId = dtl.skillId;
      d.skillName = dtl.skillName;
      d.sortSeq = dtl.sortSeq;

      this.isDetailEditorVisible = true;
    },

    onAddDetail () {
      const me = this;

      me.clearDetailPad();
      me.detail.skillDetailId = -1;

      me.isDetailEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteDetail (index) {
      const me = this;

      me.dialog.confirm('Skill <b>' + me.skillSetDetails[index].skillName + '</b> will be removed from the list.<br><br>Continue?', { size: 'md', scheme: 'warning' }).then(
        reply => {
          if (reply === 'yes') {
            me.skillSetDetails.splice(index, 1);
          }
          return;
        }
      );
    },

    onSubmitDetail () {
      const
        me = this,
        d = me.detail;

      if (!d.skillId) {
        me.isDetailEditorVisible = false;
        return;
      }
      if (!d.sortSeq) {
        me.advice.fault("Sort Seq", { title: me.core.config.msgEntryRequired, enclosed: true });
        me.setFocus('sortSeq');
        return;
      }

      let dtl = {};

      if (me.isAdding) {
        Object.assign(dtl, d);

        dtl.skillId = d.skillId;
        me.skillSetDetails.push(dtl);

        me.clearDetailPad();
        me.detail.skillDetailId = -1;
        me.advice.info("Skill '" + dtl.skillName + "' added to list.", { duration: 5 });
        // me.setFocus('skillId');
        setTimeout(() => {
          me.scrollToBottom('list');
          me.setFocus('skillId');
        }, 100);
      } else {
        dtl = me.skillSetDetails[me.detailIndex];

        dtl.skillDetailId = d.skillDetailId;
        dtl.skillId = d.skillId;
        dtl.skillName = d.skillName;
        dtl.sortSeq = d.sortSeq;

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

        me.setRequiredMode(
          'skillSetName',
          'sortSeq'
        );

        me.setFocus('skillSetName');
      }, 100);

    },

    hasChanges () {
      const me = this;

      // 05 Feb 2025 - EMT
      if (!me.isNew() && me.noEditFlag) {
        return false;
      }

      if (JSON.stringify(me.skillSet) !== me.oldSkillSet) { return true; }
      if (JSON.stringify(me.skillSetDetails) !== me.oldSkillSetDetails) { return true; }
      return false;
    },

    clearDetailPad () {
      const d = this.detail;

      d.skillDetailId = 0;
      d.skillId = 0;
      d.skillName = '';
      d.sortSeq = 0;
    }

  },

  created () {
    const me = this;

    me.oldSkillSet = '';
    me.oldSkillSetDetails = '';
    me.skills = [];     
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
  grid-template-columns: 1fr 3fr 1fr;
  gap: .5rem;
}

.fixed-header {

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
    width: 60%;
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

  #dbs0130 >>> .card-body {
    padding: .5rem;
  }
}

</style>