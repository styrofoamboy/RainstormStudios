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


Type.registerNamespace("RainstormStudios.Web.UI.WebControls.DynamicForms");

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl = function (element) {
    RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.initializeBase(this, [element]);

    this._onClientVisChanged = null;
    this._onClientAnsChanged = null;
    this._onClientFocus = null;
    this._onClientBlur = null;
    this._answerControlID = null;
}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.prototype = {
    initialize: function () {
        RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.callBaseMethod(this, 'initialize');

        this._onfocusHandler = Function.createDelegate(this, this._onFocus);
        this._onblurHandler = Function.createDelegate(this, this._onBlur);

        $addHandlers(this.get_element(),
        {
            'visibilityChanged': this._onVisibilityChanged,
            'answerChanged': this._onAnswerChanged,
            'focus': this._onFocus,
            'blur': this._onBlur
        },
        this);

        if (this._answerControlID != null && this._answerControlID.length > 0) {
            $addHandlers(this.get_element(this._answerControlID),
            {
                'selectedValueChanged': this._onAnswerChanged,
                'textChanged': this._onAnswerChanged
            },
            this);
        }
    },

    dispose: function () {
        $clearHandlers(this.get_element());
        $clearHandlers(this.get_element(this._answerControlID));
        RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.callBaseMethod(this, 'dispose');
    },

    _onFocus: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            eval(this.get_onClientFocus);
        }
    },

    _onBlur: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            eval(this.get_onClientBlur);
        }
    },

    _onVisibilityChanged: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            eval(this.get_onClientVisibilityChanged);
        }
    },

    _onAnswerChanged: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            eval(this.get_onClientAnswerChanged);
        }
    },

    get_onClientVisibilityChanged: function () {
        return this._onClientVisChanged;
    },

    set_onClientVisibilityChanged: function () {
        if (this._onClientVisChanged !== value) {
            this._onClientVisChanged = value;
            this.raisePropertyChanged("onClientVisibilityChanged");
        }
    },

    get_onClientAnswerChanged: function () {
        return this._onClientAnsChanged;
    },

    set_onClientAnswerChanged: function () {
        if (this._onClientAnsChanged !== value) {
            this._onClientAnsChanged = value;
            this.raisePropertyChanged("onClientAnswerChanged");
        }
    },

    get_onClientFocus: function () {
        return this._onClientFocus;
    },

    set_onClientFocus: function () {
        if (this._onClientFocus !== value) {
            this._onClientFocus = value;
            this.raisePropertyChanged("onClientFocus");
        }
    },

    get_onClientBlur: function () {
        return this._onClientBlur;
    },

    set_onClientBlur: function () {
        if (this._onClientBlur !== value) {
            this._onClientBlur = value;
            this.raisePropertyChanged("onClientBlur");
        }
    },

    get_AnswerControlID: function () {
        return this._answerControlID;
    },

    set_AnswerControlID: function () {
        if (this._answerControlID !== value) {
            this._answerControlID = value;
            this.raisePropertyChanged("answerControlID");
        }
    }
}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.descriptor = {
    properties: [{ name: 'onClientAnswerChanged', type: String },
                 { name: 'onClientVisibilityChanged', type: String },
                 { name: 'onClientFocus', type: String },
                 { name: 'onClientBlur', type: String },
                 { name: 'answerControlID', type: String}]
}

RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl.registerClass('RainstormStudios.Web.UI.WebControls.DynamicForms.FormElementControl', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
