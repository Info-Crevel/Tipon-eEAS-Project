// Client Profile

<template>
  <section class="container p-0 w-80" :key="ts">
    <sym-form
      id="ars0010"
      :caption="pageName"
      cardClass="curved-0"
      headerClass="frs-form-header"
      footerClass="border-top-main frs-form-footer darken-2 py-0"
      bodyClass="frs-form-body pb-3"
    >
      <!-- footer -->
      <div slot="footer" class="action-buttons p-1">
        <div
          class="buttons justify-center"
          :info-light="isMono"
          :outline="isMono"
          border-main
          fw-23
          mb-0
          sm-1
          shadow-light
        >
          <button
            type="button"
            :class="submitButtonClass"
            class="justify-between btn-save"
            @click="onSubmit"
          >
            <i class="fa fa-save fa-lg"></i><span>Save</span>
          </button>
          <button
            type="button"
            :class="clearButtonClass"
            class="justify-between btn-clear"
            @click="onClear"
          >
            <i class="fa fa-undo fa-lg"></i><span>Clear</span>
          </button>
          <button
            type="button"
            :class="deleteButtonClass"
            class="justify-between btn-delete"
            @click="onDelete"
          >
            <i class="fa fa-times-circle fa-lg"></i><span>Delete</span>
          </button>
        </div>

        <div class="buttons justify-end" :info-light="isMono" :outline="isMono" border-main fw-23 mb-0 sm-1 shadow-light>
          <button type="button" :class="logButtonClass" class="justify-between btn-log" @click="onViewLog">
            <i class="fa fa-database fa-lg"></i><span>Log</span>
          </button>
        </div> 

      </div>

      <div>
        <div class="clientid-info fw-110">
          <sym-int
            v-model="client.clientId"
            :caption-width="46"
            caption="Client ID"
            lookupId="ArsClient"
            @lostfocus="onClientIdLostFocus"
            @changing="onClientIdChanging"
            @changed="onClientIdChanged"
            @searchresult="onClientIdSearchResult"
          ></sym-int>
          <div class="buttons d-inline">
            <button
              class="btn-new warning-light border-main justify-between fw-28 shadow-light mb-2"
              :tabindex="-1"
              @mousedown.prevent
              @click="onNew"
            >
              <i class="fa fa-file-o"></i>New
            </button>
          </div>
        </div>

            <div class="box-field">
              <div class="app-box-style">
                <div
                  class="text-center border-light curved p-1 app-form-header mb-2"
                >
                  <span class="serif lg-3"> INFORMATION</span>
                </div>
                <div class="box-1">
                <sym-text
                  v-model="client.clientName"
                  :caption-width="46"
                  align="bottom"
                  caption="Name"
                  @changing="onClientNameChanging"
                ></sym-text>
                <sym-text
                  v-model="client.address1"
                  :caption-width="46"
                  align="bottom"
                  caption="BILLING ADDRESS"
                ></sym-text>
                <sym-text
                  v-model="client.address2"
                  :caption-width="46"
                  align="bottom"
                  caption="MAIN OFFICE ADDRESS "
                ></sym-text>
                  <div class="MobileContainer">
                    <span class="MobileHeader">Postal Code</span>
                    <input class="MobilePhone"  v-model="client.postalCode" @input="validatePostalCode"  type="mobile"  :disabled="client.clientId === 0"/>
                  </div>
                </div>
                
                <div class="box-2">
                
                  <div class="MobileContainer">
                    <span class="MobileHeader">TIN</span>
                    <input class="MobilePhone"  v-model="client.taxIdNumber" @input="validateTaxNumber"  type="mobile"  :disabled="client.clientId ===0"/>
                  </div>
                  <div class="MobileContainer">
                    <span class="MobileHeader">Mobile Number</span>
                    <input class="MobilePhone"  v-model="client.mobileNumber" @input="validateMobileNumber" placeholder="(09**)*******" type="mobile"   :disabled="client.clientId === 0" />
                  </div>
                  <div class="MobileContainer">
                    <span class="MobileHeader">Phone Number</span>
                    <input class="MobilePhone"  v-model="client.phoneNumber" @input="validatePhoneNumber" type="mobile" :disabled="client.clientId ===0" />
                  </div>
                  <div class="MobileContainer">
                    <span class="MobileHeader">Fax Number </span>
                    <input class="MobilePhone"  v-model="client.faxNumber" @input="validateFaxNumber" type="mobile" :disabled="client.clientId ===0" />
                  </div>

                  <sym-text
                    v-model="client.email"
                    :caption-width="46"
                    align="bottom"
                    caption="Email"
                    @changing="onEmailChanging"
                  ></sym-text>
                </div>
             
              </div>
            
        
              <div class=" app-box-style">
                      <div
                        class="text-center border-light curved p-1 app-form-header mb-2"
                      >
                        <span class="serif lg-3">ACCOUNT</span>
                      </div>

                      <div class="box-3">
                        <sym-combo
                        v-model="client.creditStatusId"
                        :caption-width="46"
                        align="bottom"
                        caption="Credit Status"
                        display-field="creditStatusName"
                        :datasource="creditStatus"
                        ></sym-combo>
                        <sym-int
                          v-model="client.creditLimit"
                          :caption-width="46"
                          align="bottom"
                          caption="Credit Limit"
                        ></sym-int>
                        
                        <sym-int
                          v-model="client.dueDays"
                          :caption-width="46"
                          align="bottom"
                          caption="Due Days"
                        ></sym-int>
                        <sym-memo
                        v-model="client.remarks"
                        :caption-width="46"
                        caption="Remarks"
                      ></sym-memo>
                      </div>
                      
                      

                      <div class="box-4">
                        <sym-check
                          v-model="client.noStatementFlag"
                          :caption-width="46"
                          caption="No Statement Flag"
                        ></sym-check>
                        <sym-check
                          v-model="client.purgeFlag"
                          :caption-width="46"
                          caption="Purge Flag"
                        ></sym-check>
                        
                    </div>
              </div>
            
                  <!--CONTACT PERSON-->
              <div class="app-box-style">
                    <div 
                      class="text-center border-light curved p-1 app-form-header mb-2"
                      >
                        <span class="serif lg-3">CONTACT PERSON</span>
                      </div>

                      <div class="scroller">
                            <table class="light striped-even mb-0 tb">
                              <thead>
                                <tr>
                                  <th class="w-50">Name</th>
                                  <th class="w-10">Mobile Number</th>
                                  <th class="w-15">Email</th>
                                  <th class="w-15">Position</th>
                                  <th class="w-10">Action</th>
                                </tr>
                              </thead>
                              <tbody>
                                <tr v-for="(dtl, index) in contacts" :key="index">
                                  <td>{{ dtl.contactPerson }}</td>
                                  <td>{{ dtl.contactMobileNumber }}</td>
                                  <td>{{ dtl.contactEmail }}</td>
                                  <td>{{ dtl.contactPosition }}</td>
                                  <td class="p-1">
                                    <div class="d-flex justify-center gap" sm-1 mb-0>
                                      <button
                                        type="button"
                                        class="justify-between info fw-22 btn-dtl-edit"
                                        @click="onEditContactPerson(dtl, index)"
                                      >
                                        <i class="fa fa-edit fa-lg"></i><span>Edit</span>
                                      </button>
                                      <button
                                        type="button"
                                        class="warning btn-dtl-delete"
                                        title="Delete"
                                        @click="onDeleteContactPerson(index)"
                                      >
                                        <i class="fa fa-times fa-lg"></i>
                                      </button>
                                    </div>
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                          </div>
                        
                          <div
                            class="command-buttons light border-main p-1 border-top-0 mb-2"
                          >
                            <div></div>

                            <div
                              class="buttons justify-center"
                              :info-light="isMono"
                              :outline="isMono"
                              border-main
                              fw-35
                              mb-0
                              sm-1
                              shadow-light
                            >
                              <button
                                type="button"
                                class="justify-between btn-add"
                                @click="onAddContactPerson"
                              >
                                <i class="fa fa-plus-circle fa-lg"></i><span>Add</span>
                              </button>
                            </div>
                          </div>
              </div>
            </div>
          
        </div>

    <br>
      


    </sym-form>

    <!-- LOGS -->
    <!-- <sym-modal
      v-model="isLogVisible"
      size="xl"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="true"
      :dismissible="true"
      :closeOnBackButton="false"
      title="Client Profile Change Log"
      headerClass="frs-form-header"
      dismissButtonClass="danger"
    >
      <div class="fixed-header">
        <table id="logs" class="striped-odd">
          <thead>
            <tr>
              <th class="w-7">Action</th>
              <th class="w-15">Column</th>
              <th class="w-18">Old Value</th>
              <th class="w-18">New Value</th>
              <th class="w-15">Log Date/Time</th>
              <th class="w-10">User</th>
              <th class="w-22">Reference</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="(log, index) in logs" :key="index">
              <td>{{ log.logDescription }}</td>
              <td>{{ log.columnCaption }}</td>
              <td>{{ log.oldValue }}</td>
              <td>{{ log.newValue }}</td>
              <td>
                {{
                  core.toDateFormat(log.logDateTime, true, "MM/dd/yyyy h:mm tt")
                }}
              </td>
              <td>{{ log.logUserInfo }}</td>
              <td>{{ log.logReference }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </sym-modal> -->

    <!-- Kin -->
    <sym-modal
      id="contactperson-editor"
      v-model="isContactPersonEditorVisible"
      size="rg"
      :header="true"
      :customBody="true"
      :footer="false"
      :keyboard="false"
      :dismissible="false"
      :closeOnBackButton="false"
      :title="editorContactPersonTitle"
      @show="onShowContactPersonEditor($event)"
      @hide="onHideContactPersonEditor($event)"
      headerClass="frs-form-header"
      dismissButtonClass="danger"
    >
      <div class="board p-1 mb-0 w-100">
        <form
          id="ars0010A"
          class="curved-bottom border-dark marianblue p-3"
          @submit.prevent
        >
          <div class="contact-editor-boxes gap">
            <sym-text
              v-model="contact.contactPerson"
              caption="Name"
              align="bottom"
              @changing="onContactPersonChanging"
            ></sym-text>
            <div class="MobileContainer">
                <span class="MobileHeader">Mobile Number</span>
                <input class="MobilePhone"  v-model="contact.contactMobileNumber" @input="validateContactMobileNumber" placeholder="(09**)*******" type="mobile"   :style="{
                borderColor: isValidMobileNumber == 'valid' ? 'green' : isValidMobileNumber == 'invalid' ? '' : '', }" />
            </div>
            <sym-text
              v-model="contact.contactEmail"
              caption="Email Address"
              align="bottom"
              
            ></sym-text>
            <sym-text
              v-model="contact.contactPosition"
              caption="Position"
              align="bottom"
            ></sym-text>
          </div>

          <div
            class="buttons w-100 justify-end mt-3 mb-2"
            fw-26
            shadow-soft
            mb-0
          >
            <button
              type="button"
              class="info justify-between border-main"
              @click="onSubmitContactPerson()"
            >
              <i class="fa fa-check mr-2"></i>Submit
            </button>
            <button
              type="button"
              class="warning justify-between border-main mr-0"
              @click="isContactPersonEditorVisible = false"
            >
              <i class="fa fa-times-circle fa-lg mr-2"></i>Close
            </button>
          </div>
        </form>
      </div>
    </sym-modal>

  </section>
</template>

<script>
import { get, ajax } from "../../js/http";

import {
  getList,
  getSafeDeleteFlag,
} from "../../js/dbSys";

import PageMaintenance from "../PageMaintenance.vue";
import { disable } from "../../js/dom";

export default {
  extends: PageMaintenance,
  name: "ars0010",

  data() {
    return {
      client: {
        clientId: 0,
        clientName: "",
        clientTypeId: 0,
        creditStatusId: 0,
        address1: "",
        address2: "",
        postalCode: "",
        taxIdNumber: "",
        phoneNumber: "",
        mobileNumber: "",
        faxNumber: "",
        email: "",
        contactPerson: "",
        remarks: "",
        creditLimit: 0,
        dueDays: 0,
        accountId: "",
        accountName: "",
        noStatementFlag: false,
        purgeFlag: false,
        LockId: "",
      },

      contacts: [],

      contact: {
        contactDetailId: 0,
        clientId: 0,
        contactPerson: "",
        contactMobileNumber:"",
        contactEmail:"",
        contactPosition: "",
        lockId: "",
      },

      contactPersonIndex: -1,
      isContactPersonEditorVisible: false,
    };
  },

  methods: {


      onEmailChanging(e) {
      if (!e.proposedValue) {
        return;
      }

      if (!this.core.isValidEmail(e.proposedValue)) {
        e.message = "(" + e.proposedValue + ") is not a valid email address.";
        e.preventDefault();
        return;
      }
    },

    validatePhoneNumber() {
      if (!this.client.phoneNumber) {
        this.result = null;
        return;
      }

      if (this.client.phoneNumber.length > 11) {
        this.client.phoneNumber = this.client.phoneNumber.slice(0, 11);
      }

      this.client.phoneNumber = this.client.phoneNumber.replace(/\D/g, "");
    },

    validateMobileNumber() {
      if (!this.client.mobileNumber) {
        this.result = null;
        return;
      }

      if (this.client.mobileNumber.length > 11) {
        this.client.mobileNumber = this.client.mobileNumber.slice(0, 11);
      }

      this.client.mobileNumber = this.client.mobileNumber.replace(/\D/g, "");
    },

    validateFaxNumber() {
      if (!this.client.faxNumber) {
        this.result = null;
        return;
      }

      if (this.client.faxNumber.length > 25) {
        this.client.faxNumber = this.client.faxNumber.slice(0, 25);
      }

      this.client.faxNumber = this.client.faxNumber.replace(/\D/g, "");
    },

    validateContactMobileNumber() {
      if (!this.contact.contactMobileNumber) {
        this.result = null;
        return;
      }

      if (this.contact.contactMobileNumber.length > 11) {
        this.contact.contactMobileNumber = this.contact.contactMobileNumber.slice(0, 11);
      }

      this.contact.contactMobileNumber = this.contact.contactMobileNumber.replace(/\D/g, "");
    },

    validateTaxNumber() {
      if (!this.client.taxIdNumber) {
        this.result = null;
        return;
      }

      if (this.client.taxIdNumber.length > 12) {
        this.client.taxIdNumber = this.client.taxIdNumber.slice(0, 12);
      }

      this.client.taxIdNumber = this.client.taxIdNumber.replace(/\D/g, "");
    },

    validatePostalCode() {
      if (!this.client.postalCode) {
        this.result = null;
        return;
      }

      if (this.client.postalCode.length > 4) {
        this.client.postalCode = this.client.postalCode.slice(0, 4);
      }

      this.client.postalCode = this.client.postalCode.replace(/\D/g, "");
    },

    getTargetPath() {
      const q = {},
        me = this;

      if (me.client.clientId) {
        q.clientId = me.client.clientId;
      }

      return {
        name: me.$options.name,
        query: q,
      };
    },

    syncValues(p, q) {
      const me = this;

      if ("clientId" in q && me.core.isInteger(q.clientId)) {
        me.client.clientId = parseInt(q.clientId);
      }
    },

    loadData() {
      const me = this,
        wait = me.wait();

      me.getReferences()
        .then((data) => {
          if (!me.core.isBoolean(data)) {
            me.clientType = data.clientType;
            me.creditStatus = data.creditStatus;
            }
          if (me.client.clientId < 0) {
            return Promise.resolve(null);
          }
          return me.getClient();
        })
        .then((client) => {
          me.stopWait(wait);
           if (client && client.clients.length) {
            me.setModels(client);
          } else {
            if (me.client.clientId > -1) {
              let message =
                "Client ID '<b>" + me.client.clientId + "</b>' not found.";
                
              me.advice.fault(message, { duration: 5 });
              me.onReset();
              return;
            }

            me.advice.warning("You are adding a new document.", {
              duration: 5,
            });
          }

          me.setupControls();
          me.isFilled = true;
        })
        .catch((fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        });
    },
    setModels(info) {
      const me = this,
      client = info.clients[0];
      me.client = client;
 
      me.contacts = info.contacts;
      me.refreshOldRefs();
    },

    refreshOldRefs() {
      const me = this;
      me.oldClient = JSON.stringify(me.client);
      me.oldContacts= JSON.stringify(me.contacts);
    },

    // API calls

    getClient() {
      if (this.client.clientId < 0) {
        return Promise.resolve(null);
      }

      return get("api/clients/" + this.client.clientId);
    },

    getReferences() {
      const me = this;
      if (me.clientType.length) {
        return Promise.resolve(true);
      }
      return get("api/references/ars0010");
    },

    createRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("POST", body);
      return ajax("api/clients/" + currentUserId, options);
     
    },

    modifyRecord() {
      const payload = this.getApiPayload(),
        body = JSON.stringify(payload),
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("PUT", body);
      return ajax(
        "api/clients/" + this.client.clientId + "/" + currentUserId,
        options
      );
    },

    deleteRecord() {
      const client = this.client,
        currentUserId = this.sym.userInfo.userId,
        options = this.core.getAjaxOptions("DELETE");
      return ajax(
        "api/clients/" +
          client.clientId +
          "/" +
          client.lockId +
          "/" +
          currentUserId,
        options
      );
    },

    onContactPersonChanging (e) {
      e.message = 'Entry rejected.'
      e.callback = this.contactPersonCallback;
    },

    contactPersonCallback (e) {
      const me = this;

      let index = me.contacts.findIndex(obj => obj.contactPerson === e.proposedValue);
      if (index > -1) {
        e.message = 'Contact Person <b>' + e.proposedValue + '</b> is already in the list.'
        return false;
      }
      return true;
    },

    onSubmitContactPerson() {
      const me = this,
        d = me.contact;

      if (!d.contactPerson) {
        me.isContactPersonEditorVisible = false;
        return;
      }

      let dtl = {};

      if (me.isAdding) {
        Object.assign(dtl, d);
        me.contacts.push(dtl);
        console.log('PG',me.contacts)
        me.clearContactPersonPad();
        me.advice.info("Contact Person '" + dtl.contactPerson + "' added to list.", {
          duration: 5,
        });
        me.setFocus("contactPerson");
      } else {
        dtl = me.contacts[me.contactPersonIndex];
        dtl.contactDetailId = d.contactDetailId;
        dtl.contactPerson = d.contactPerson;
        dtl.contactPosition = d.contactPosition;

        me.isContactPersonEditorVisible = false;
      }
    },

    onShowContactPersonEditor() {
      const me = this;

      me.setActiveModel("contact");

      me.setRequiredMode(
        'contactPerson',
        'contactPosition'
      );

      setTimeout(() => {
        this.setFocus("contactPerson");
      }, 200);
    },

    onHideContactPersonEditor() {
      const me = this;

      me.isAdding = false;
      me.setActiveModel();
    },

    onEditContactPerson(dtl, index) {
      const d = this.contact;
      this.contactPersonIndex = index;

      d.contactDetailId = dtl.contactDetailId;
      d.contactPerson = dtl.contactPerson;
      d.contactMobileNumber = dtl.contactMobileNumber;
      d.contactEmail = dtl.contactEmail;
      d.contactPosition = dtl.contactPosition;

      this.isContactPersonEditorVisible = true;
    },

    onAddContactPerson() {
      const me = this;

      me.clearContactPersonPad();
      me.contact.contactDetailId = -1;

      me.isContactPersonEditorVisible = true;
      me.isAdding = true;
    },

    onDeleteContactPerson(index) {
      const me = this;

      me.dialog
        .confirm(
          "Contact Person <b>" +
            me.contacts[index].contactPerson +
            "</b> will be removed from the list.<br><br>Continue?",
          { size: "rg", scheme: "warning" }
        )
        .then((reply) => {
          if (reply === "yes") {
            me.contacts.splice(index, 1);
          }
          return;
        });
    },

    getApiPayload() {
      const me = this,
        client = {};

      Object.assign(client, me.client);
      client.contacts = me.contacts;
      return client;
    },

    // event handlers

    onLoad() {
      const me = this,  
        dc = me.dataConfig;

      dc.models.push(
        "client",
        "contacts"
      );
      dc.keyField = "clientId";
      dc.autoAssignKey = true;
    },

    onResetAfter() {
      (this.contacts = []),
      this.refreshOldRefs();

      setTimeout(() => {
        this.disableElement("btn-add");
      }, 100);
    },

    onClientIdChanging(e) {
 
      e.callback = this.clientIdCallback;
    },

    clientIdCallback(e) {
      e.message = "Client ID '<b>" + e.proposedValue + "</b>' not found.";
      return this.isValidEntity(
        "dbo.ArsClient",
        "clientId",
        e.proposedValue
      ).then((result) => {
        return result;
      });
    },

    onClientIdChanged() {
      const me = this;

      me.loadData();
      me.replaceUrl();
    },

    onClientIdLostFocus() {
      const me = this;
      if (!me.client.clientId && !me.isModalShown()) {
        me.goTop();
      }
    },

    onClientIdSearchResult(result) {
      if (!result) {
        return;
      }

      const me = this,
        data = result[0];

      me.client.clientId = data.clientId;
      me.replaceUrl();
      me.loadData();
    },

    onClientNameChanging(e) {
      e.callback = this.clientNameCallback;
    },

    clientNameCallback(e) {
      e.message = "Client '<b>" + e.proposedValue + "</b>' already defined.";
      return true;
    },

    onAccountIdChanging (e) {
      e.message = "Account ID '<b>" + e.proposedValue + "</b>' not found / acceptable.";

      e.callback = this.accountIdCallback;
    },

    accountIdCallback (e) { 
      const me = this;
      let filter = "AccountId='" + e.proposedValue + "' AND HeaderFlag = 0";
      return getList('dbo.FinAccount', 'AccountId, AccountName', '', filter).then(
        acct => {
          if (acct && acct.length) {
            me.client.accountName = acct[0].accountName;
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

    onAccountIdSearchResult (result) {
      if (!result) { return; }

      const
        data = result[0],
        client = this.client;

      client.accountId = data.accountId;
      client.accountName = data.accountName;

      this.focusNext();

    },

    onSubmit(nextRoute) {
      const me = this;
      if (!me.isValid(me.$options.name)) {
        me.advice.fault(
          "Fill in the required fields (marked in red) before saving.",
          { duration: 5 }
        );
        return;
      }


      if (this.client.taxIdNumber < 12) {
        let message ="Invalid TIn Number";   
        me.advice.fault(message, { duration: 5 });
        return;
      }
      if (!me.hasChanges()) {
        me.advice.success("Document updated.", { duration: 5 });
        me.onReset();
        return;
      }
      let promise,
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
            if (isNew && typeof success === "number" && success > 0) {
              me.client.clientId = success;
            }
            if (isNew) {
              message = "New document created.";
            } else {
              message = "Document updated.";
            }
            me.setCopyData();
            if (nextRoute && !(nextRoute instanceof MouseEvent)) {
              me.dialog.success(message, { size: "md" }).then(() => {
                me.refreshOldRefs();
                me.go(nextRoute);
                return;
              });
            } else {
              me.advice.success(message, { duration: 5 });
            }
          }

          me.onReset();
        },
        (fault) => {
          me.stopWait(wait);
          me.showFault(fault);
        }
      );
    },

    onDelete() {
      const me = this;

      getSafeDeleteFlag("ArsClient", me.client.clientId)
        .then((safe) => {
          if (safe) {
            return me.dialog.confirm(
              "Document will be deleted.<br><br>Continue?",
              { scheme: "warning", icon: "warning", size: "md" }
            );
          } else {
            me.advice.fault(
              "Delete attempt failed.<hr>Cannot delete document at this time.",
              { duration: 5 }
            );
            return "";
          }
        })
        .then((reply) => {
          if (reply === "yes") {
            return me.deleteRecord();
          }
          return false;
        })
        .then((success) => {
          if (success) {
            me.advice.success("Document deleted.", { duration: 4 });
            me.onReset();
          }
        })
        .catch((fault) => {
          me.showFault(fault);
        });
    },
    // helpers

    setupControls() {
      const me = this;

      setTimeout(() => {
        me.enableElement("btn-add");

        me.setDefaultControlStates();

        me.setRequiredMode(
          "clientName",
          "creditStatusId",
        );

        me.setDisplayMode(
          'accountName'      
        ); 
       
        me.setFocus("clientName");
      }, 100);
    },

    hasChanges() {
      const me = this;
      
      if (JSON.stringify(me.client) !== me.oldClient) {
        return true;
      }
      if (JSON.stringify(me.contacts) !== me.oldContacts) {
        return true;
      }
      return false;
    },

    clearContactPersonPad() {
      const d = this.contact;

      d.contactDetailId = 0;
      d.contactPerson = "";
      d.contactPosition = "";
      d.contactMobileNumber = "";
      d.contactEmail = "";
    },
  },

  created() {
    const me = this;

    me.oldClient = "";
    me.oldContacts = "";

    me.creditStatus = [];
    me.clientType = [];
  },

  computed: {

    editorContactPersonTitle() {
      if (this.isAdding) {
        return "Add Contact Person Detail";
      }
      return "Edit Contact Person Detail";
    },

    isValidMobileNumber() {

      let value = "";

        if (this.client.mobileNumber) {

        if (this.client.mobileNumber.length == 0) {
          value = "default";
          return value;
        }

        }

        const mobileNumberRegex = /^09\d{09}$/;
        const isValid = mobileNumberRegex.test(this.client.mobileNumber);

        value = isValid ? "valid" : "invalid";
        return value;
    },

  }
};

</script>

<style scoped>

  .clientid-info {
    display: flex;
    flex-direction: row;
    gap: 2rem;
  }
  .action-buttons {
    display: flex;
    flex-direction: row;
    justify-content: center;
  }

  #logs tr {
    vertical-align: top;
  }

  #logs td {
    font-size: 0.875rem;
    padding: 0.375rem;
  }  
  .box-field {
    display: grid;
    grid-template-rows: .5fr .5fr .5fr ;
    gap: .5rem;
  }
  .box-1{
    display: grid;
    grid-template-columns: 1fr 1fr 1fr .3fr;
    gap: .5rem;
  }
  .box-2{
    display: grid;
    grid-template-columns: .5fr .4fr .4fr .5fr 1fr;
    gap: .5rem;
  }
   .box-3{
    display: grid;
    grid-template-columns: .5fr .5fr .5fr 2fr ;
    gap: .5rem;
  }
   .box-4{
    display: grid;
    grid-template-columns: .5fr .5fr .5fr .5fr 1fr;
    gap: .5rem;
  }
  .container-2 {
    display: flex;
    flex-direction: column;
    width: 50%;

    border: 1px solid rgba(190, 190, 190, 0.918);
    background-color: rgba(219, 215, 215, 0.473);
    padding: 10px;
    height: 50%;
    border-radius: 10px;
  }
  .flag {
    display: grid;
    gap: .2rem;
  }
  .credit {
    display: grid;
    grid-template-columns: 1fr 1fr .5fr;
    gap: .5rem;
  }
.contact-editor-boxes{
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr;
}
.MobileContainer {
  display: flex;
  flex-direction: column;
  margin-bottom: 0.5rem;
}
.MobileHeader {
  border: 1px solid rgb(206, 203, 203);
  background-color: rgb(245, 243, 243);
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  width: 100%;
  padding: 0.5rem;
}
.MobilePhone {
  border-top-right-radius: 0;
  border-bottom-right-radius: 4px;
}
</style>
