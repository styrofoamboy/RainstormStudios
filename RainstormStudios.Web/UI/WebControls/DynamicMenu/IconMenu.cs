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

namespace RainstormStudios.Web.UI.WebControls.DynamicMenu
{
    class DynamicMenuIconGroup : DynamicMenuChildControl
    {
        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        { get { return HtmlTextWriterTag.Fieldset; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuIconGroup(DynamicMenuItem item, Style itemStyle, Style activeStyle, Style hoverStyle)
            : base(item, itemStyle, activeStyle, hoverStyle)
        { }
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

            writer.AddAttribute(HtmlTextWriterAttribute.Class, string.IsNullOrEmpty(this.CssClass)
                                                                ? "iconGroup"
                                                                : this.CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);

            writer.RenderBeginTag(HtmlTextWriterTag.Legend);
            writer.WriteEncodedText(this._menuItem.Text);
            writer.RenderEndTag(); // Legend

            foreach (Control c in this.Controls)
            {
                if (!(c is DynamicMenuChildControl))
                    continue;

                c.RenderControl(writer);
            }

            writer.RenderEndTag(); // Fieldset
        }
        protected override void CreateChildControls()
        {
            DynamicMenuItem item = this._menuItem;
            for (int i = 0; i < item.MenuItems.Count; i++)
            {
                DynamicMenuIcon ctrl = new DynamicMenuIcon(item.MenuItems[i], this._itemStyle, this._aciveItemStyle, this._hoverItemStyle);
                ctrl.SelectorMode = this._selectorMode;
                ctrl.MenuItemClicked += new DynamicMenuEventHandler(ctrl_MenuItemClicked);
                ctrl.MenuItemRendering += new DynamicMenuItemRenderingEventHandler(ctrl_MenuItemRendering);
                this.Controls.Add(ctrl);
            }
            base.CreateChildControls();
        }
        #endregion
    }
    class DynamicMenuIcon : DynamicMenuChildControl
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        Control
            _lnkBtn;
        #endregion

        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Span;
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DynamicMenuIcon(DynamicMenuItem item, Style itemStyle, Style activeItemStyle, Style hoverItemStyle)
            : base(item, itemStyle, activeItemStyle, hoverItemStyle)
        { }
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

            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            this._lnkBtn.RenderControl(writer);

            writer.RenderEndTag();
        }
        protected override void CreateChildControls()
        {
            DynamicMenuItem item = this._menuItem;

            Control lnk = null;
            string ctrlID = "lnkMenuItem_" + item.MenuItemProviderKey.ToString();
            string cmdName = (this.SelectorMode) ? "Select" : item.CommandName;
            if (string.IsNullOrEmpty(cmdName) || !string.IsNullOrEmpty(item.Target))
            {
                HyperLink lnkHref = new HyperLink();
                lnkHref.ID = ctrlID;
                lnkHref.NavigateUrl = item.NavigationUrl;
                if (!string.IsNullOrEmpty(item.Target))
                    lnkHref.Target = item.Target;
                lnk = lnkHref;
            }
            else if (!string.IsNullOrEmpty(item.CommandName))
            {
                if (!string.IsNullOrEmpty(item.Target))
                    throw new Exception("Cannot specify a DynamicMenuItem.Target value when using a command name or argument.");

                LinkButton lnkBtn = new LinkButton();
                lnkBtn.ID = ctrlID;
                lnkBtn.CommandName = item.CommandName;
                if (item.MenuItemProviderKey != null)
                    lnkBtn.CommandArgument = item.MenuItemProviderKey.ToString();
                lnkBtn.Command += new CommandEventHandler(lnkBtn_Command);
                lnk = lnkBtn;
            }

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                Image img = new Image();
                img.ID = "imgIcon";
                img.ImageUrl = item.ImageUrl;
                img.GenerateEmptyAlternateText = true;
                lnk.Controls.Add(img);
            }

            LiteralControl txt = new LiteralControl();
            txt.Text = "<br/>" + item.Text;
            lnk.Controls.Add(txt);

            this._lnkBtn = lnk;
            this.Controls.Add(lnk);

            base.CreateChildControls();
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void lnkBtn_Command(object sender, CommandEventArgs e)
        {
            this.OnMenuItemClicked(new DynamicMenuEventArgs(this._menuItem, e, DynamicMenuItemControlType.DynamicMenuIcon));
        }
        #endregion
    }
}
