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


Type.registerNamespace("RainstormStudios.Web.UI.WebControls");

RainstormStudios.Web.UI.WebControls.DynamicFormControl = function (element) {
    RainstormStudios.Web.UI.WebControls.DynamicFormControl.initializeBase(this, [element]);

    this._onClientElementVisibilityChanged = null;
    this._onClientElementAnswerChanged = null;
    this._onClientElementRendering = null;
    this._onClientElementRendered = null;
}

RainstormStudios.Web.UI.WebControls.DynamicFormControl.prototype = {
    initialize: function () {
        RainstormStudios.Web.UI.WebControls.DynamicFormControl.callBaseMethod(this, 'initialize');

    },

    dispose: function () {
        $clearHandlers(this.get_element());
        RainstormStudios.Web.UI.WebControls.DynamicFormControl.callBaseMethod(this, 'dispose');
    },

    get_onClientElementVisibilityChanged: function () {
        return this._onClientElementVisibilityChanged;
    },

    set_onClientElementVisibilityChanged: function () {
        if (this._onClientElementVisibilityChanged !== value) {
            this._onClientElementVisibilityChanged = value;
            this.raisePropertyChanged("onClientElementVisibilityChanged");
        }
    },

    get_onClientElementAnswerChanged: function () {
        return this._onClientElementAnswerChanged;
    },

    set_onClientElementAnswerChanged: function () {
        if (this._onClientElementAnswerChanged !== value) {
            this._onClientElementAnswerChanged = value;
            this.raisePropertyChanged("onClientElementAnswerChanged");
        }
    },

    get_onClientElementRendering: function () {
        return this._onClientElementRendering;
    },

    set_onClientElementRendering: function () {
        if (this._onClientElementRendering !== value) {
            this._onClientElementRendering = value;
            this.raisePropertyChanged("onClientElementRendering");
        }
    },

    get_onClientElementRendered: function () {
        return this._onClientElementRendered;
    },

    set_onClientElementRendered: function () {
        if (this._onClientElementRendered !== value) {
            this._onClientElementRendered = value;
            this.raisePropertyChanged("onClientElementRendered");
        }
    }
}

RainstormStudios.Web.UI.WebControls.DynamicFormControl.descriptor = {
    properties: [{ name: "onClientElementAnswerChanged", type: String },
                 { name: "onClientElementVisibilityChanged", type: String },
                 { name: "onClientElementRendering", type: String },
                 { name: "onClientElementRendered", type: String}]
}

RainstormStudios.Web.UI.WebControls.DynamicFormControl.registerClass('RainstormStudios.Web.UI.WebControls.DynamicFormControl', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();