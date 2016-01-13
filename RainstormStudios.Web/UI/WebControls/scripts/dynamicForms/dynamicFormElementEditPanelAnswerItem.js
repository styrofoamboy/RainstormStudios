//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
/// <reference name="MicrosoftAjax.js"/>


Type.registerNamespace("RainstormStudios.Web.UI.WebControls.DynamicForm");
RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem = function (element) {
    RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.initializeBase(this, [element]);

    this._onClientActionButtonClick = null;
    this._onClientActionButtonClick = null;
    this._actionBtnID = null;
    this._removeBtnID = null;
}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.prototype = {
    initialize: function () {
        RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.callBaseMethod(this, 'initialize');

        this._onActionBtnClickHandler = Function.createDelegate(this, this._onActionButtonClick);
        this._onRemoveBtnClickHandler = Function.createDelegate(this, this._onRemoveButtonClick);

        if (this._actionBtnID != null && this._actionBtnID.length > 0) {
            $addHandlers(this.get_element(this._actionButnID), {
                'click': this._onActionButtonClick
            }, this);
        }
        if (this._removeBtnID != null && this._removeBtnID.length > 0) {
            $addHandlers(this.get_element(this._removeBtnID), {
                'click': this._onRemoveButtonClick
            }, this);
        }
    },

    dispose: function () {
        if (this._actionBtnID != null && this._actionBtnID.length > 0) {
            $clearHandlers(this.get_element(this._actionBtnID));
        }
        if (this._removeBtnID != null && this._removeBtnID.length > 0) {
            $clearHandlers(this.get_element(this._removeBtnID));
        }
        RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.callBaseMethod(this, 'dispose');
    },

    _onActionButtonClick: function (e) {
        if (this.get_element() && !this.get_element().disabled && this.get_element(this._actionBtnID) && !this.get_element(this._actionBtnID).disabled) {
            eval(this._onClientActionButtonClick);
        }
    },

    _onRemoveButtonClick: function (e) {
        if (this.get_element() && !this.get_element().disabled && this.get_element(this._removeBtnID) && !this.get_element(this._removeBtnID).disabled) {
            eval(this._onClientActionButtonClick);
        }
    },

    get_onClientActionButtonClick: function () {
        return this.onClientActionButtonClick;
    },

    set_onClientActionButtonClick: function () {
        if (this._onClientActionButtonClick !== value) {
            this._onClientActionButtonClick = value;
            this.raisePropertyChanged("onClientActionButtonClick");
        }
    },

    get_onClientRemoveButtonClick: function () {
        return this._onClientRemoveButtonClick;
    },

    set_onClientRemoveButtonClick: function () {
        if (this._onClientRemoveButtonClick !== value) {
            this._onClientRemoveButtonClick = value;
            this.raisePropertyChanged("onClientRemoveButtonClick");
        }
    },

    get_actionBtnID: function () {
        return this._actionBtnID;
    },

    set_actionBtnID: function () {
        if (this._actionBtnID !== value) {
            this._actionBtnID = value;
            this.raisePropertyChanged("actionBtnID");
        }
    },

    get_removeBtnID: function () {
        return this._removeBtnID;
    },

    set_removeBtnID: function () {
        if (this._removeBtnID !== value) {
            this._removeBtnID = value;
            this.raisePropertyChanged("removeBtnID");
        }
    }

}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.descriptor = {
    properties: [{ name: 'onClientActionButtonClick', type: String },
                 { name: 'onClientRemoveButtonClick', type: String },
                 { name: 'actionBtnID', type: String },
                 { name: 'removeBtnID', type: String }]
}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementEditPanelAnswerItem.registerClass('RainstormStudios.Web.UI.WebControls.DyanicForms.FormElementEditPanelAnswerItem', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();