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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: System.Web.UI.WebResource("RainstormStudios.Web.UI.WebControls.style.modal.css", "text/css", PerformSubstitution = true)]

namespace RainstormStudios.Web.UI.WebControls
{
    [DefaultProperty("HeaderText"), ParseChildren(true), ToolboxData("<{0}:ModalPopup runat=\"server\"></{0}:ModalPopup>")]
    public class ModalPopup : System.Web.UI.WebControls.CompositeControl, INamingContainer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ITemplate
            _hdrTemplate,
            _cntTemplate,
            _cmdTemplate;
        private string
            _hdrText;
        private AjaxControlToolkit.ModalPopupExtender
            _extender;
        //***************************************************************************
        // Public Events
        // 
        public event ModalItemCommandEventHandler
            ModalItemCommand;
        public event ModalItemCommandEventHandler
            ModalOkClicked;
        public event ModalItemCommandEventHandler
            ModalCancelClicked;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// A template to be used for the header of the template. This is also the template's drag handle.
        /// </summary>
        [TemplateContainer(typeof(ModalPopup))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate
        {
            get { return this._hdrTemplate; }
            set { this._hdrTemplate = value; }
        }
        /// <summary>
        /// A template to be used for the content of the modal.
        /// </summary>
        [TemplateContainer(typeof(ModalPopup))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ContentTemplate
        {
            get { return this._cntTemplate; }
            set { this._cntTemplate = value; }
        }
        /// <summary>
        /// A template to be used for the lower section of the modal containing the command controls.
        /// </summary>
        [TemplateContainer(typeof(ModalPopup))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CommandTemplate
        {
            get { return this._cmdTemplate; }
            set { this._cmdTemplate = value; }
        }
        /// <summary>
        /// A value from the <see cref="T:RainstormStudios.Web.UI.WebControls.ModalButtonConfig"/> enum specifying which 'default' button config to use in the absense of a <paramref name="CommandTemplate"/>. The default value is 'OK'.
        /// </summary>
        public ModalButtonConfig ButtonConfig
        {
            get
            {
                object vsVal = this.ViewState["ModalButtonConfig"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return ModalButtonConfig.OK;
                else
                    return (ModalButtonConfig)iVsVal;
            }
            set { this.ViewState["ModalButtonConfig"] = (int)value; }
        }
        /// <summary>
        /// Sets the text to be drawn in the modal's header, in the absense of a <paramref name="HeaderTemplate"/>.
        /// </summary>
        public string HeaderText
        {
            get { return this._hdrText; }
            set { this._hdrText = value; }
        }
        /// <summary>
        /// Defines the ID of the control in the <paramref name="CommandTemplate"/> to be treated as the 'OK' button.
        /// </summary>
        public string OkTemplateControlID
        {
            get { return (string)this.ViewState["OkTemplateControlID"]; }
            set { this.ViewState["OkTemplateControlID"] = value; }
        }
        /// <summary>
        /// Defines the ID of the control in the <paramref name="CommandTemplate"/> to be treated as the 'Cancel' button.
        /// </summary>
        public string CancelTemplateControlID
        {
            get { return (string)this.ViewState["CancelTemplateControlID"]; }
            set { this.ViewState["CancelTemplateControlID"] = value; }
        }
        /// <summary>
        /// Specifies how the modal will be repositioned durring window scroll/resize events. Default value is <see cref="T:AjaxControlToolkit.ModalPopupRepositionMode"/> 'None'.
        /// </summary>
        public AjaxControlToolkit.ModalPopupRepositionMode RepositionMode
        {
            get
            {
                object vsVal = this.ViewState["ModalPopupRepositionMode"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return AjaxControlToolkit.ModalPopupRepositionMode.None;
                else
                    return (AjaxControlToolkit.ModalPopupRepositionMode)iVsVal;
            }
            set { this.ViewState["ModalPopupRepositionMode"] = (int)value; }
        }
        /// <summary>
        /// A JavaScript code snippet to be executed when the 'OK' button is clicked.
        /// </summary>
        public string OnOkScript
        {
            get { return (string)this.ViewState["OnOkScript"]; }
            set { this.ViewState["OnOkScript"] = value; }
        }
        /// <summary>
        /// A JavaScript code snippet to be executed when the 'Cancel' button is clicked.
        /// </summary>
        public string OnCancelScript
        {
            get { return (string)this.ViewState["OnCancelScript"]; }
            set { this.ViewState["OnCancelScript"] = value; }
        }
        public AjaxControlToolkit.Animation OnShowing
        {
            get { return (AjaxControlToolkit.Animation)this.ViewState["OnShowing"]; }
            set { this.ViewState["OnShowing"] = value; }
        }
        public AjaxControlToolkit.Animation OnShown
        {
            get { return (AjaxControlToolkit.Animation)this.ViewState["OnShown"]; }
            set { this.ViewState["OnShown"] = value; }
        }
        public AjaxControlToolkit.Animation OnHiding
        {
            get { return (AjaxControlToolkit.Animation)this.ViewState["OnHiding"]; }
            set { this.ViewState["OnHiding"] = value; }
        }
        public AjaxControlToolkit.Animation OnHidden
        {
            get { return (AjaxControlToolkit.Animation)this.ViewState["OnHidden"]; }
            set { this.ViewState["OnHidden"] = value; }
        }
        /// <summary>
        /// A <see cref="T:System.Boolean"/> value indicating whether or not the 'default' OK button will cause a server postback. Default is 'true', unless <paramref name="ButtonConfig"/> value is set to <see cref="T:ModalButtonConfig"/> 'OK'.
        /// </summary>
        public bool DefaultOkCausesPostback
        {
            get
            {
                object vsVal = this.ViewState["DefaultOkCausesPostback"];
                bool bVsVal;
                // For the "OK" button config, odds are we're not doing anything when the user clicks "OK"
                //   so we'll default that one to false.
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return (this.ButtonConfig == ModalButtonConfig.OK) ? false : true;
                else
                    return bVsVal;
            }
            set { this.ViewState["DefaultOkCausesPostback"] = value; }
        }
        /// <summary>
        /// A <see cref="T:System.Boolean"/> value whether or not the 'default' Cancel button will cause a server postback. Default is 'false'.
        /// </summary>
        public bool DefaultCancelCausesPostback
        {
            get
            {
                object vsVal = this.ViewState["DefaultCancelCausesPostback"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return false;
                else
                    return bVsVal;
            }
            set { this.ViewState["DefaultCancelCausesPostback"] = value; }
        }
        /// <summary>
        /// Defines the ID of the control which will cause this modal to appear when clicked. Default is NULL, which causes a 'fake' link button to be constructed and relies on the calling page to manually Show/Hide the modal.
        /// </summary>
        public string TargetControlID
        {
            get { return (string)this.ViewState["TargetControlID"]; }
            set { this.ViewState["TargetControlID"] = value; }
        }
        /// <summary>
        /// A <see cref="T:System.Boolean"/> value indicating whether or not the modal can be moved around when the user clicks the modal's header. Default is 'true'.
        /// </summary>
        public bool AllowModalDrag
        {
            get
            {
                object vsVal = this.ViewState["AllowModalDrag"];
                bool bVsVal;
                if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
                    return true;
                else
                    return bVsVal;
            }
            set { this.ViewState["AllowModalDrag"] = value; }
        }
        /// <summary>
        /// A <see cref="T:System.String"/> value indicating the Validation group to which the default 'OK' button will be attached.
        /// </summary>
        public string DefaultOkValidationGroup
        {
            get { return (string)this.ViewState["DefaultOkValidationGroup"]; }
            set { this.ViewState["DefaultOkValidationGroup"] = value; }
        }
        /// <summary>
        /// A <see cref="T:System.String"/> value indicating the Validation group to which the default 'Cancel' button will be attached.
        /// </summary>
        public string DefaultCancelValidationGroup
        {
            get { return (string)this.ViewState["DefaultCancelValidationGroup"]; }
            set { this.ViewState["DefaultCancelValidationGroup"] = value; }
        }
        /// <summary>
        /// Gets or sets the command argument value assigned to the default 'OK' button.
        /// </summary>
        public string DefaultOkCommandArgument
        {
            get { return (string)this.ViewState["DefaultOkCommandArgument"]; }
            set { this.ViewState["DefaultOkCommandArgument"] = value; }
        }
        /// <summary>
        /// Gets or sets the command argument value assigned to the default 'Cancel' button.
        /// </summary>
        public string DefaultCancelCommandArgument
        {
            get { return (string)this.ViewState["DefaultCancelCommandArgument"]; }
            set { this.ViewState["DefaultCancelCommandArgument"] = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        { get { return HtmlTextWriterTag.Div; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ModalPopup()
            : base()
        {
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Causes the modal to appear in the browser window.
        /// </summary>
        public void Show()
        {
            this.EnsureChildControls();
            this._extender.Show();
        }
        /// <summary>
        /// Causes the modal to be hidden in the browser window.
        /// </summary>
        public void Hide()
        {
            this.EnsureChildControls();
            this._extender.Hide();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            Control lnkCheck = this.Page.Header.FindControl("ModalPopupCss");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "ModalPopupCss";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.ModalPopup), "RainstormStudios.Web.UI.WebControls.style.modal.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            Panel panModal = new Panel();
            panModal.ID = "panModalPopup";
            panModal.Width = this.ControlStyle.Width;
            panModal.Height = this.ControlStyle.Height;
            panModal.CssClass = "modalPopup";
            panModal.Style.Add(HtmlTextWriterStyle.Display, "none");

            Panel panModalHandle = new Panel();
            panModalHandle.ID = "panModalHandle";
            panModalHandle.CssClass = "modalHandle";
            if (this.HeaderTemplate != null)
                this.HeaderTemplate.InstantiateIn(panModalHandle);
            else
            {
                LiteralControl hdr = new LiteralControl();
                hdr.Text = this.HeaderText;
                panModalHandle.Controls.Add(hdr);
            }
            panModal.Controls.Add(panModalHandle);

            Panel panModalContent = new Panel();
            panModalContent.ID = "panModalContent";
            panModalContent.CssClass = "modalContent";
            if (this.ContentTemplate != null)
                this.ContentTemplate.InstantiateIn(panModalContent);
            panModal.Controls.Add(panModalContent);

            Panel panModalCommand = new Panel();
            panModalCommand.ID = "panModalCommand";
            panModalCommand.CssClass = "modalCommand";
            if (this.CommandTemplate != null)
            {
                this.CommandTemplate.InstantiateIn(panModalCommand);
                this.ButtonConfig = ModalButtonConfig.Custom;
            }
            else
            {
                // If the user didn't specify a command template, we'll build a generic one, based on the
                //   selected "ModalButtonConfig" value.

                if (this.ButtonConfig == ModalButtonConfig.OkCancel || this.ButtonConfig == ModalButtonConfig.ConfirmCancel || this.ButtonConfig == ModalButtonConfig.SubmitCancel || this.ButtonConfig == ModalButtonConfig.SaveCancel)
                {
                    LinkButton lnkCancel = new LinkButton();
                    lnkCancel.ID = "lnkModalCancel";
                    lnkCancel.CommandName = "Cancel";
                    lnkCancel.Text = "Cancel";
                    if (!this.DefaultCancelCausesPostback)
                        this.CancelTemplateControlID = lnkCancel.ID;
                    else
                        lnkCancel.Command += new CommandEventHandler(this.lnkCancel_OnCommand);
                    if (!string.IsNullOrEmpty(this.DefaultCancelValidationGroup))
                        lnkCancel.ValidationGroup = this.DefaultCancelValidationGroup;
                    if (!string.IsNullOrEmpty(this.DefaultCancelCommandArgument))
                        lnkCancel.CommandArgument = this.DefaultCancelCommandArgument;
                    panModalCommand.Controls.Add(lnkCancel);
                }
                if (this.ButtonConfig == ModalButtonConfig.OK || this.ButtonConfig == ModalButtonConfig.OkCancel || this.ButtonConfig == ModalButtonConfig.ConfirmCancel || this.ButtonConfig == ModalButtonConfig.SubmitCancel || this.ButtonConfig == ModalButtonConfig.SaveCancel)
                {
                    LinkButton lnkOK = new LinkButton();
                    if (this.ButtonConfig == ModalButtonConfig.ConfirmCancel)
                    {
                        lnkOK.ID = "lnkModalConfirm";
                        lnkOK.CommandName = "Confirm";
                        lnkOK.Text = "Confirm";
                    }
                    else if (this.ButtonConfig == ModalButtonConfig.SubmitCancel)
                    {
                        lnkOK.ID = "lnkModalSubmit";
                        lnkOK.CommandName = "Submit";
                        lnkOK.Text = "Submit";
                    }
                    else if (this.ButtonConfig == ModalButtonConfig.SaveCancel)
                    {
                        lnkOK.ID = "lnkModalSave";
                        lnkOK.CommandName = "Save";
                        lnkOK.Text = "Save";
                    }
                    else
                    {
                        lnkOK.ID = "lnkModalOK";
                        lnkOK.CommandName = "OK";
                        lnkOK.Text = "OK";
                    }
                    if (!this.DefaultOkCausesPostback)
                        this.OkTemplateControlID = lnkOK.ID;
                    else
                        lnkOK.Command += new CommandEventHandler(this.lnkOk_OnCommand);
                    if (!string.IsNullOrEmpty(this.DefaultOkValidationGroup))
                        lnkOK.ValidationGroup = this.DefaultOkValidationGroup;
                    if (!string.IsNullOrEmpty(this.DefaultOkCommandArgument))
                        lnkOK.CommandArgument = this.DefaultOkCommandArgument;
                    panModalCommand.Controls.Add(lnkOK);

                }
                if (this.ButtonConfig == ModalButtonConfig.YesNo)
                {
                    LinkButton lnkNo = new LinkButton();
                    lnkNo.ID = "lnkModalNo";
                    lnkNo.CommandName = "No";
                    lnkNo.Text = "No";
                    if (!this.DefaultCancelCausesPostback)
                        this.CancelTemplateControlID = lnkNo.ID;
                    else
                        lnkNo.Command += new CommandEventHandler(this.lnkCancel_OnCommand);
                    if (!string.IsNullOrEmpty(this.DefaultCancelValidationGroup))
                        lnkNo.ValidationGroup = this.DefaultCancelValidationGroup;
                    if (!string.IsNullOrEmpty(this.DefaultCancelCommandArgument))
                        lnkNo.CommandArgument = this.DefaultCancelCommandArgument;
                    panModalCommand.Controls.Add(lnkNo);

                    LinkButton lnkYes = new LinkButton();
                    lnkYes.ID = "lnkModalYes";
                    lnkYes.CommandName = "Yes";
                    lnkYes.Text = "Yes";
                    if (!this.DefaultOkCausesPostback)
                        this.OkTemplateControlID = lnkYes.ID;
                    else
                        lnkYes.Command += new CommandEventHandler(this.lnkOk_OnCommand);
                    if (!string.IsNullOrEmpty(this.DefaultOkValidationGroup))
                        lnkYes.ValidationGroup = this.DefaultOkValidationGroup;
                    if (!string.IsNullOrEmpty(this.DefaultOkCommandArgument))
                        lnkYes.CommandArgument = this.DefaultOkCommandArgument;
                    panModalCommand.Controls.Add(lnkYes);
                }
            }
            panModal.Controls.Add(panModalCommand);

            this.Controls.Add(panModal);

            LinkButton lnkFake = new LinkButton();
            if (string.IsNullOrEmpty(this.TargetControlID))
            {
                lnkFake.ID = "lnkFakeTarget";
                lnkFake.Text = string.Empty;
                lnkFake.Style.Add(HtmlTextWriterStyle.Display, "none");

                this.Controls.Add(lnkFake);
            }

            AjaxControlToolkit.ModalPopupExtender popupExtender = new AjaxControlToolkit.ModalPopupExtender();
            if (!string.IsNullOrEmpty(this.TargetControlID))
                popupExtender.TargetControlID = this.TargetControlID;
            else
                popupExtender.TargetControlID = lnkFake.ID;
            popupExtender.PopupControlID = panModal.ID;
            if (this.AllowModalDrag)
                popupExtender.PopupDragHandleControlID = panModalHandle.ID;
            if (!string.IsNullOrEmpty(this.OkTemplateControlID))
                popupExtender.OkControlID = this.OkTemplateControlID;
            if (!string.IsNullOrEmpty(this.CancelTemplateControlID))
                popupExtender.CancelControlID = this.CancelTemplateControlID;
            if (!string.IsNullOrEmpty(this.OnOkScript))
                popupExtender.OnOkScript = this.OnOkScript;
            if (!string.IsNullOrEmpty(this.OnCancelScript))
                popupExtender.OnCancelScript = this.OnCancelScript;
            popupExtender.OnShowing = this.OnShowing;
            popupExtender.OnShown = this.OnShown;
            popupExtender.OnHiding = this.OnHiding;
            popupExtender.OnHidden = this.OnHidden;
            popupExtender.RepositionMode = this.RepositionMode;

            this._extender = popupExtender;

            this.Controls.Add(popupExtender);
        }
        protected override void OnDataBinding(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnDataBinding(e);
        }
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            if (args is CommandEventArgs)
            {
                this.OnModalItemCommand(new ModalItemCommandEventArgs(this, source, (CommandEventArgs)args));
                base.OnBubbleEvent(source, args);
                return true;
            }
            return false;
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnModalItemCommand(ModalItemCommandEventArgs e)
        {
            if (this.ModalItemCommand != null)
                this.ModalItemCommand.Invoke(this, e);
        }
        protected void OnModalOkClicked(ModalItemCommandEventArgs e)
        {
            if (this.ModalOkClicked != null)
                this.ModalOkClicked.Invoke(this, e);
        }
        protected void OnModalCancelClicked(ModalItemCommandEventArgs e)
        {
            if (this.ModalCancelClicked != null)
                this.ModalCancelClicked.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void lnkOk_OnCommand(object sender, CommandEventArgs e)
        {
            this.OnModalOkClicked(new ModalItemCommandEventArgs(this, sender, e));
        }
        protected void lnkCancel_OnCommand(object sender, CommandEventArgs e)
        {
            this.OnModalCancelClicked(new ModalItemCommandEventArgs(this, sender, e));
        }
        #endregion
    }
    public enum ModalButtonConfig : uint
    {
        ConfirmCancel,
        Custom,
        OK,
        OkCancel,
        SaveCancel,
        SubmitCancel,
        YesNo
    }
    public delegate void ModalItemCommandEventHandler(object sender, ModalItemCommandEventArgs e);
    public class ModalItemCommandEventArgs : EventArgs
    {
        private ModalPopup
            _mSrc;
        private object
            _cSrc;
        private CommandEventArgs
            _args;

        public ModalPopup
            ModalSource { get { return this._mSrc; } }
        public object
            ControlSource { get { return this._cSrc; } }
        public CommandEventArgs
            Args { get { return this._args; } }

        public ModalItemCommandEventArgs(ModalPopup mSrc, object cSrc, CommandEventArgs args)
        {
            this._mSrc = mSrc;
            this._cSrc = cSrc;
            this._args = args;
        }
    }
}
