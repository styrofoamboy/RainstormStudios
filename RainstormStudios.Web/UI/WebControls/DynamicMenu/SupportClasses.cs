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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.DynamicMenu
{
    public delegate void DynamicMenuEventHandler(object sender, DynamicMenuEventArgs e);
    public delegate void DynamicMenuItemRenderingEventHandler(object sender, DynamicMenuItemRenderingEventArgs e);
    public class DynamicMenuEventArgs : System.EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public readonly DynamicMenuItem
            Item;
        public readonly DynamicMenuItemControlType
            ControlType;
        public readonly CommandEventArgs
            CommandArgs;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuEventArgs(DynamicMenuItem item, DynamicMenuItemControlType type)
        {
            this.Item = item;
            this.ControlType = type;
        }
        public DynamicMenuEventArgs(DynamicMenuItem item, CommandEventArgs args, DynamicMenuItemControlType type)
            : this(item, type)
        {
            this.CommandArgs = args;
        }
        #endregion
    }
    public class DynamicMenuItemRenderingEventArgs : DynamicMenuEventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public bool
            Cancel;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuItemRenderingEventArgs(DynamicMenuItem item, DynamicMenuItemControlType type)
            : base(item, type)
        { }
        public DynamicMenuItemRenderingEventArgs(DynamicMenuItem item, CommandEventArgs args, DynamicMenuItemControlType type)
            : base(item, args, type)
        { }
        #endregion
    }
    abstract class DynamicMenuChildControl : System.Web.UI.WebControls.CompositeControl, ICallbackEventHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected Style
            _itemStyle,
            _aciveItemStyle,
            _hoverItemStyle;
        protected DynamicMenuItem
            _menuItem;
        protected bool
            _selectorMode;
        //HiddenField
        //    _hdnActiveMenuItem;
        //***************************************************************************
        // Public Events
        // 
        public event DynamicMenuEventHandler
            MenuItemClicked;
        public event DynamicMenuItemRenderingEventHandler
            MenuItemRendering;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool SelectorMode
        {
            get { return this._selectorMode; }
            set { this._selectorMode = value; }
        }
        public object CurrentActiveMenuItemProviderKey
        {
            get { return this.ViewState["CurrentActiveMenuItemProviderKey"]; }
            set { this.ViewState["CurrentActiveMenuItemProviderKey"] = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected string JavascriptKeyName
        { get { return "jsDynaMenuInit_" + this.ClientID; } }
        protected string HiddenFieldName
        { get { return "hdnDynamicMenuActiveMenuItem"; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected DynamicMenuChildControl(DynamicMenuItem item, Style itemStyle, Style activeStyle, Style hoverStyle)
        {
            this._menuItem = item;
            this._itemStyle = itemStyle;
            this._aciveItemStyle = activeStyle;
            this._hoverItemStyle = hoverStyle;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string GetCallbackResult()
        {
            //return this._hdnActiveMenuItem != null ? this._hdnActiveMenuItem.Value : "N/A";
            return this.CurrentActiveMenuItemProviderKey != null ? this.CurrentActiveMenuItemProviderKey.ToString() : "N/A";
        }
        public void RaiseCallbackEvent(string eventArg)
        {
            //if (this._hdnActiveMenuItem == null)
            //    return;

            //this._hdnActiveMenuItem.Value = eventArg;

            this.CurrentActiveMenuItemProviderKey = eventArg;
            this.Page.ClientScript.RegisterHiddenField(HiddenFieldName, eventArg);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            ClientScriptManager cm = this.Page.ClientScript;
            string cbRef = cm.GetCallbackEventReference(this, "arg", "ReceiveServerData", "");
            string cbScript = "function updateActiveMenuItem(arg, context) { " + cbRef + "; }";
            cm.RegisterClientScriptBlock(typeof(RainstormStudios.Web.UI.WebControls.DynamicMenu.DynamicMenuChildControl), "UpdateActiveMenuItem", cbScript, true);

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.EnsureChildControls();
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            //this._hdnActiveMenuItem = new HiddenField();
            //this._hdnActiveMenuItem.ID = "hdnDynamicMenuActiveMenuItem";
            //this.Controls.Add(this._hdnActiveMenuItem);
        }
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            if (source is DynamicMenuChildControl)
            {
                DynamicMenuChildControl item = (source as DynamicMenuChildControl);
                this.OnMenuItemClicked(new DynamicMenuEventArgs(item._menuItem, DynamicMenuItemControlType.Command));
                base.OnBubbleEvent(source, args);
                return true;
            }
            return false;
        }
        protected virtual bool CheckRender()
        {
            Type myType = this.GetType();
            DynamicMenuItemControlType ctrlType = DynamicMenuItemControlType.Command;
            if (!Enum.TryParse<DynamicMenuItemControlType>(myType.Name, out ctrlType))
                ctrlType = DynamicMenuItemControlType.Command;

            DynamicMenuItemRenderingEventArgs eArgs = new DynamicMenuItemRenderingEventArgs(this._menuItem, ctrlType);
            this.OnMenuItemRendering(eArgs);
            return !eArgs.Cancel;
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnMenuItemClicked(DynamicMenuEventArgs e)
        {
            if (this.MenuItemClicked != null)
                this.MenuItemClicked.Invoke(this, e);
        }
        protected virtual void OnMenuItemRendering(DynamicMenuItemRenderingEventArgs e)
        {
            if (this.MenuItemRendering != null)
                this.MenuItemRendering.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void ctrl_MenuItemClicked(object sender, DynamicMenuEventArgs e)
        { this.OnMenuItemClicked(e); }
        protected void ctrl_MenuItemRendering(object sender, DynamicMenuItemRenderingEventArgs e)
        { this.OnMenuItemRendering(e); }
        #endregion
    }
}
