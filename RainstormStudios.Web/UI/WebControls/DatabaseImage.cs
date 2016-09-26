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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    [ToolboxData("<{0}:ChevronHeader runat=\"server\"></{0}:ChevronHeader>"), ParseChildren(false), AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DatabaseImage : System.Web.UI.Control
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string ProviderName
        {
            get { return (string)this.ViewState["ProviderName"]; }
            set { this.ViewState["ProviderName"] = value; }
        }
        public object ImageProviderKey
        {
            get { return this.ViewState["providerKey"]; }
            set { this.ViewState["providerKey"] = value; }
        }
        public Unit Width
        {
            get
            {
                object vsVal = this.ViewState["Width"];
                if (vsVal == null || !(vsVal is Unit))
                    return Unit.Empty;
                else
                    return (Unit)vsVal;
            }
            set { this.ViewState["Width"] = value; }
        }
        public Unit Height
        {
            get
            {
                object vsVal = this.ViewState["Height"];
                if (vsVal == null || !(vsVal is Unit))
                    return Unit.Empty;
                else
                    return (Unit)vsVal;
            }
            set { this.ViewState["Height"] = value; }
        }
        public ImageAlign Align
        {
            get { return this.ViewState.GetValue<ImageAlign>("Align", ImageAlign.NotSet, false, Enum.TryParse<ImageAlign>); }
            set { this.ViewState["Align"] = value; }
        }
        public string HandlerValue
        {
            get
            {
                string vsVal = (string)this.ViewState["HandlerValue"];
                if (string.IsNullOrEmpty(vsVal))
                    return "dbstrm.axd";
                else
                    return vsVal;
            }
            set { this.ViewState["HandlerValue"] = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            writer.BeginRender();
            try
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                if (this.Width != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
                if (this.Height != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Src, this.GetWebHandlerReference());
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                base.Render(writer);
            }
            finally
            { writer.EndRender(); }
        }
        private string GetWebHandlerReference()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}?k={1}"
                , this.HandlerValue
                , HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.ImageProviderKey.ToString()))));

            if (!string.IsNullOrEmpty(this.ProviderName))
                sb.AppendFormat("&p={1}"
                    , HttpContext.Current.Server.UrlEncode(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.ProviderName))));

            return sb.ToString();
        }
        #endregion
    }
}
