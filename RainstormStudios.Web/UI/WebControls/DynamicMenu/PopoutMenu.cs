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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls.DynamicMenu
{
    class DynamicMenuPopoutGroup : DynamicMenuChildControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        DynamicMenuItemCollection
            _menuItems;
        bool
            _isSubMenu,
            _expanded;
        Collections.StringCollection
            _assocDivs;
        #endregion

        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
        internal DynamicMenuItemCollection MenuItems
        { get { return this._menuItems; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuPopoutGroup(DynamicMenuItemCollection items, Style itemStyle, Style activeStyle, Style hoverStyle)
            : base(null, itemStyle, activeStyle, hoverStyle)
        {
            this._assocDivs = new Collections.StringCollection();
            this._menuItems = items;
            //this._menuItems.Updated += new Collections.CollectionEventHandler(menuItems_Updated);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (!this.CheckRender())
                return;

            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, string.IsNullOrEmpty(this.CssClass) ? (this._isSubMenu ? "popoutSubMenu" : "popoutMenu") : this.CssClass);
            if (this._isSubMenu && !this._expanded)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            StringBuilder jsInit = new StringBuilder();
            for (int i = 0; i < this.Controls.Count; i++)
            {
                DynamicMenuPopoutItem ctrl = (this.Controls[i] as DynamicMenuPopoutItem);
                if (ctrl != null)
                {
                    if (ctrl.MenuItem.MenuItems.Count > 0)
                        // Setup the javascript event handler for the menu items that have popout menus.
                        jsInit.AppendLine(string.Format("dynamicMenuControl_SetupPopupMenus('{0}','{1}', '{2}');", ctrl.ClientID, this._assocDivs[ctrl.MenuItem.MenuItemProviderKey.ToString()], ctrl.MenuItem.MenuItemProviderKey));

                    // I have modified the javascript to use jQuery to 'capture' any navigation menu items and read their child anchor's href.
                    //else if (!string.IsNullOrEmpty(ctrl.MenuItem.NavigationUrl))
                    //    // Setup javascript event handlers for the menu items that are just navigation links so that the containing 'span' will also trigger navigation.
                    //    jsInit.AppendLine(string.Format("dynamicMenuControl_SetupPopupMenuNav('{0}', '{1}');", ctrl.ClientID, ctrl.MenuItem.NavigationUrl));

                    ctrl.RenderControl(writer);
                }
            }
            this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), this.JavascriptKeyName, jsInit.ToString(), true);

            writer.RenderEndTag();

            for (int i = 0; i < this.Controls.Count; i++)
                if (this.Controls[i] is DynamicMenuPopoutGroup)
                {
                    this.Controls[i].RenderControl(writer);
                }
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            for (int i = 0; i < this._menuItems.Count; i++)
            {
                DynamicMenuItem item = this._menuItems[i];

                DynamicMenuPopoutItem ctrl = new DynamicMenuPopoutItem(item, this._itemStyle, this._aciveItemStyle, this._hoverItemStyle);
                ctrl.SelectorMode = this._selectorMode;
                ctrl.ParentGroup = this;
                ctrl.MenuItemClicked += new DynamicMenuEventHandler(ctrl_MenuItemClicked);
                ctrl.MenuItemRendering += new DynamicMenuItemRenderingEventHandler(ctrl_MenuItemRendering);
                this.Controls.Add(ctrl);

                if (item.MenuItems.Count > 0)
                {
                    DynamicMenuPopoutGroup ctrlGrp = new DynamicMenuPopoutGroup(item.MenuItems, this._itemStyle, this._aciveItemStyle, this._hoverItemStyle);
                    ctrlGrp.SelectorMode = this._selectorMode;
                    ctrlGrp._isSubMenu = true;
                    ctrlGrp.MenuItemClicked += new DynamicMenuEventHandler(ctrl_MenuItemClicked);
                    ctrlGrp.MenuItemRendering += new DynamicMenuItemRenderingEventHandler(ctrl_MenuItemRendering);
                    this.Controls.Add(ctrlGrp);
                    this._assocDivs.Add(ctrlGrp.ClientID, item.MenuItemProviderKey.ToString());
                }

                this.Controls.Add(ctrl);
            }
        }
        internal void ShowSubMenu(object menuItemProviderKey)
        {
            string clientID = this._assocDivs[menuItemProviderKey.ToString()];
            DynamicMenuPopoutGroup subMenu = null;
            for (int i = 0; i < this.Controls.Count; i++)
                if (this.Controls[i].ClientID == clientID)
                { subMenu = (this.Controls[i] as DynamicMenuPopoutGroup); break; }
            if (subMenu != null)
                subMenu._expanded = true;
        }
        internal void HideSubMenu(object menuItemProviderKey)
        {
            string clientID = this._assocDivs[menuItemProviderKey.ToString()];
            DynamicMenuPopoutGroup subMenu = null;
            for (int i = 0; i < this.Controls.Count; i++)
                if (this.Controls[i].ClientID == clientID)
                { subMenu = (this.Controls[i] as DynamicMenuPopoutGroup); break; }
            if (subMenu != null)
                subMenu._expanded = false;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void menuItems_Updated(object sender, RainstormStudios.Collections.CollectionEventArgs e)
        {
            //this.Controls.Clear();
            //this.CreateChildControls();
        }
        #endregion
    }
    class DynamicMenuPopoutItem : DynamicMenuChildControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        DynamicMenuPopoutGroup
            _owner;
        Control
            _btn;
        bool
            _autoActivate = true;
        #endregion

        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        internal DynamicMenuPopoutGroup ParentGroup
        {
            get { return this._owner; }
            set { this._owner = value; }
        }
        internal DynamicMenuItem MenuItem
        { get { return this._menuItem; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuPopoutItem(DynamicMenuItem item, Style itemStyle, Style activeStyle, Style hoverStyle)
            : base(item, itemStyle, activeStyle, hoverStyle)
        { }
        public DynamicMenuPopoutItem(DynamicMenuItem item, Style itemStyle, Style activeStyle, Style hoverStyle, bool autoActivate)
            : this(item, itemStyle, activeStyle, hoverStyle)
        {
            this._autoActivate = autoActivate;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (!this.CheckRender())
                return;

            //Style myStyle = this._menuItem.Activated
            //                ? this._aciveItemStyle
            //                : this._itemStyle;
            //if (myStyle != null)
            //    writer.EnterStyle(myStyle);
            //else
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "menuItem");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);

            this._btn.ApplyStyleSheetSkin(this.Page);
            this._btn.RenderControl(writer);

            //if (myStyle != null)
            //    writer.ExitStyle(myStyle);
            //else
                writer.RenderEndTag();
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            DynamicMenuItem item = this._menuItem;

            Control lnkItem = null;
            string lnkID = "lnkMenuItem_" + item.MenuItemProviderKey.ToString();

            string cmdName = (this._selectorMode && item.MenuItems.Count == 0) ? "Select" : item.CommandName;

            if (!this._selectorMode && (item.MenuItems.Count > 0 || string.IsNullOrEmpty(cmdName)))
            {
                if (item.MenuItems.Count > 0 && (!string.IsNullOrEmpty(item.NavigationUrl) || !string.IsNullOrEmpty(cmdName)))
                    throw new Exception("You cannot specify a navigation URL or command name for a popout menu item that has children.");

                HyperLink lnk = new HyperLink();
                if (item.MenuItems.Count == 0)
                {
                    if (!string.IsNullOrEmpty(item.Target))
                        lnk.Target = item.Target;
                    lnk.NavigateUrl = item.NavigationUrl;
                }
                if (string.IsNullOrEmpty(item.ImageUrl))
                    lnk.Text = item.Text;
                lnkItem = lnk;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.Target) && !this._selectorMode)
                    throw new Exception("You cannot specify a target and a command name for a menu item.");

                LinkButton lnk = new LinkButton();
                lnk.CommandName = item.CommandName;
                lnk.CommandArgument = item.MenuItemProviderKey.ToString();
                lnk.Command += new CommandEventHandler(lnk_Command);
                if (string.IsNullOrEmpty(item.ImageUrl))
                    lnk.Text = item.Text;
                lnkItem = lnk;
            }

            lnkItem.ID = lnkID;
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                Image img = new Image();
                img.ID = "menuItemImg_" + item.MenuItemProviderKey.ToString();
                img.ImageUrl = item.ImageUrl;
                img.ImageAlign = ImageAlign.Middle;
                lnkItem.Controls.Add(img);
                LiteralControl txt = new LiteralControl(item.Text);
                lnkItem.Controls.Add(txt);
            }

            this.Controls.Add(lnkItem);
            this._btn = lnkItem;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void lnk_Command(object sender, CommandEventArgs e)
        {
            if (this._menuItem.MenuItems.Count > 0 && this._autoActivate)
            {
                DynamicMenuItem item = this._menuItem;
                item.Activated = !item.Activated;
                this.ParentGroup.MenuItems[item.MenuItemProviderKey.ToString()] = item;
                this.ParentGroup.ShowSubMenu(item.MenuItemProviderKey);
            }

            else
                this.OnMenuItemClicked(new DynamicMenuEventArgs(this._menuItem, e, DynamicMenuItemControlType.DynamicMenuPopoutItem));
        }
        #endregion
    }
}
