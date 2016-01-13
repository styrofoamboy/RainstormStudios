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
using System.Configuration;
using System.Configuration.Provider;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios.Providers;
using RainstormStudios.Web.UI.WebControls.DynamicMenu;

namespace RainstormStudios.Web.UI.WebControls
{
    public class DynamicMenuControl : System.Web.UI.WebControls.CompositeControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _menuName,
            _providerName;
        private DynamicMenuRenderType
            _renderMode = DynamicMenuRenderType.HoverVertical;
        private Style
            _menuStyle,
            _menuItemStyle,
            _menuItemHoverStyle,
            _activeMenuItemStyle;
        private bool
            _autoSetActive = true,
            _designMode = false;
        private object
            _activeItemKey = null;
        private DynamicMenuProvider
            _provider;
        private DynamicMenuItemCollection
            _items;
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
        public string MenuName
        {
            get { return this._menuName; }
            set { this._menuName = value; }
        }
        public string ProviderName
        {
            get { return this._providerName; }
            set { this._providerName = value; }
        }
        [Browsable(false)]
        public DynamicMenuItemCollection MenuItems
        {
            get { return this._items; }
        }
        [Browsable(false)]
        public object ActiveItemKey
        {
            get { return this._activeItemKey; }
            set { this._activeItemKey = value; }
        }
        public bool AutoSetActiveItemOnClick
        {
            get { return this._autoSetActive; }
            set { this._autoSetActive = value; }
        }
        public DynamicMenuRenderType RenderMode
        {
            get { return this._renderMode; }
            set { this._renderMode = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style MenuStyle
        {
            get { return this._menuStyle; }
            set { this._menuStyle = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style MenuItemStyle
        {
            get { return this._menuItemStyle; }
            set { this._menuItemStyle = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style MenuItemHoverStyle
        {
            get { return this._menuItemHoverStyle; }
            set { this._menuItemHoverStyle = value; }
        }
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style ActiveMenuItemStyle
        {
            get { return this._activeMenuItemStyle; }
            set { this._activeMenuItemStyle = value; }
        }
        [Browsable(true)]
        public bool DesignMode
        {
            get { return this._designMode; }
            set { this._designMode = value; }
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
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Load provider.
            if (!string.IsNullOrEmpty(this._providerName))
                this._provider = DynamicMenuProviderManager.Providers[this._providerName];
            else
                this._provider = DynamicMenuProviderManager.Provider;

            if (this._provider == null)
                throw new Exception("Specified dynamic menu provider name not found or no default provider set.");
        }
        protected override void OnPreRender(EventArgs e)
        {
            // Emit CSS reference.
            Control lnkCheck = this.Page.Header.FindControl("DynamicMenuCss");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "DynamicMenuCss";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.DynamicMenuControl), "RainstormStudios.Web.UI.WebControls.style.dynamicMenu.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }
            this.Page.ClientScript.RegisterClientScriptResource(typeof(RainstormStudios.Web.UI.WebControls.DynamicMenuControl), "RainstormStudios.Web.UI.WebControls.scripts.dynamicMenu.js");

            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();
            writer.BeginRender();
            try
            {
                if (string.IsNullOrEmpty(this.CssClass))
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "iconMenu");
                else
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                for (int i = 0; i < this.Controls.Count; i++)
                    this.Controls[i].RenderControl(writer);

                writer.RenderEndTag();
            }
            finally
            {
                writer.EndRender();
            }

            // These are not the droids you're looking for.
            //base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            if (string.IsNullOrEmpty(this.MenuName))
                throw new Exception("You must specify the name of the menu to build.");
            this.LoadMenu(this.MenuName);

            if (this._items == null)
                throw new Exception("MenuItemCollection has not been initialized!");
            DynamicMenuChildControl ctrl = null;
            switch (this.RenderMode)
            {
                case DynamicMenuRenderType.HoverHorizontal:
                    throw new NotImplementedException();

                case DynamicMenuRenderType.HoverVertical:
                    throw new NotImplementedException();

                case DynamicMenuRenderType.Icon:
                    for (int i = 0; i < this._items.Count; i++)
                    {
                        ctrl = new DynamicMenuIconGroup(this._items[i], this._menuItemStyle, this._activeMenuItemStyle, this._menuItemHoverStyle);
                        if (this._designMode)
                            ctrl.SelectorMode = true;
                        ctrl.MenuItemClicked += new DynamicMenuEventHandler(this.childCtrl_OnClick);
                        ctrl.MenuItemRendering += new DynamicMenuItemRenderingEventHandler(this.childCtrl_OnRendering);
                        this.Controls.Add(ctrl);
                    }
                    break;

                case DynamicMenuRenderType.Popout:
                    {
                        ctrl = new DynamicMenuPopoutGroup(this._items, this._menuItemStyle, this._activeMenuItemStyle, this._menuItemHoverStyle);
                        if (this._designMode)
                            ctrl.SelectorMode = true;
                        ctrl.MenuItemClicked += new DynamicMenuEventHandler(this.childCtrl_OnClick);
                        ctrl.MenuItemRendering += new DynamicMenuItemRenderingEventHandler(this.childCtrl_OnRendering);
                        this.Controls.Add(ctrl);
                    }
                    break;

                default:
                    throw new Exception("Unrecognized DynamicMenuRenderType value.");
            }
            base.CreateChildControls();
        }
        private void LoadMenu(string menuName)
        {
            if (this._items == null)
                this._items = new DynamicMenuItemCollection();
            else
                this._items.Clear();
            this._items.AddRange(this._provider.GetMenuItems(menuName));
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected void OnMenuItemClicked(DynamicMenuEventArgs e)
        {
            if (this.MenuItemClicked != null)
                this.MenuItemClicked.Invoke(this, e);
        }
        protected void OnMenuItemRendering(DynamicMenuItemRenderingEventArgs e)
        {
            if (this.MenuItemRendering != null)
                this.MenuItemRendering(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void childCtrl_OnClick(object sender, DynamicMenuEventArgs e)
        {
            this.OnMenuItemClicked(e);
        }
        protected void childCtrl_OnRendering(object sender, DynamicMenuItemRenderingEventArgs e)
        {
            this.OnMenuItemRendering(e);
        }
        #endregion
    }
}
