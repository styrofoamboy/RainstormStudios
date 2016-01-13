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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    [Author("Unfried, Michael"), DefaultProperty("Text"), ParseChildren(false), ToolboxData("<{0}:Button runat=\"server\"></{0}:Button>")]
    public class Button : System.Web.UI.WebControls.Button
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        //public bool Toggled
        //{
        //    get
        //    {
        //        object vsVal = this.ViewState["ButtonToggled"];
        //        bool bVsVal;
        //        if (vsVal == null || !bool.TryParse(vsVal.ToString(), out bVsVal))
        //            return false;
        //        else
        //            return bVsVal;
        //    }
        //    set { this.ViewState["ButtonToggled"] = value; }
        //}
        public ButtonColorTheme ColorTheme
        {
            get
            {
                object vsVal = this.ViewState["ColorTheme"];
                ButtonColorTheme theme;
                if (vsVal == null || !Enum.TryParse(vsVal.ToString(), out theme))
                    return ButtonColorTheme.Blue;
                else
                    return theme;
            }
            set { this.ViewState["ColorTheme"] = value; }
        }
        public ButtonStyleTheme StyleTheme
        {
            get
            {
                object vsVal = this.ViewState["StyleTheme"];
                ButtonStyleTheme theme;
                if (vsVal == null || !Enum.TryParse(vsVal.ToString(), out theme))
                    return ButtonStyleTheme.Standard;
                else
                    return theme;
            }
            set { this.ViewState["StyleTheme"] = value; }
        }
        public Unit TextVerticalOffset
        {
            get
            {
                object vsVal = this.ViewState["TextVertOffset"];
                if (vsVal == null)
                    return Unit.Empty;
                else
                    try { return Unit.Parse(vsVal.ToString()); }
                    catch { return Unit.Empty; }
            }
            set { this.ViewState["TextVertOffset"] = value; }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Protected Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            Control lnkCheck = this.Page.Header.FindControl("rsButtonCss");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "rsButtonCss";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.Button), "RainstormStudios.Web.UI.WebControls.style.button.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.BeginRender();
            try
            {
                //bool toggled = this.Toggled;

                // Write the anchor start.  This will make these images be an actionable object.
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Page.ClientScript.GetPostBackClientHyperlink(this, "", true));
                if (this.StyleTheme != ButtonStyleTheme.Custom)
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "rsButton " + this.StyleTheme.ToString().ToLower() + " " + this.ColorTheme.ToString().ToLower() + (this.Enabled ? " active" : " disabled"));
                else if (!string.IsNullOrEmpty(this.CssClass))
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
                if (Enabled)
                    writer.AddAttribute("Enabled","false");
                writer.RenderBeginTag(HtmlTextWriterTag.A);

                // Write the "outer" span.  This will be the left edge of the button.
                if (this.ControlStyleCreated)
                {
                    if (this.ControlStyle.Font.Italic)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "oblique");
                    if (this.ControlStyle.Font.Size != FontUnit.Empty)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, this.ControlStyle.Font.Size.ToString());
                    if (this.ControlStyle.Font.Names.Length > 0)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, string.Join(", ", this.ControlStyle.Font.Names));
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "lOuter");
                writer.RenderBeginTag("span");

                // Write the "inner-outer" span.  This will be the right edge of the button.
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "rOuter");
                writer.RenderBeginTag("span");

                // Write the "inner" span.  This will be the center of the button and contain the text.
                if (this.Width != null && this.Width != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "inner");
                writer.RenderBeginTag("span");

                if (this.ControlStyleCreated)
                {
                    if (this.ControlStyle.Font.Bold)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    else if (this.ControlStyle.ForeColor != System.Drawing.Color.Transparent)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Color, Hex.GetWebColor(this.ControlStyle.ForeColor));
                    if (this.ControlStyle.Font.Strikeout)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "strikeout");
                    if (this.ControlStyle.Font.Underline)
                        writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "underline");
                }
                if (this.TextVerticalOffset != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, this.TextVerticalOffset.ToString());
                writer.RenderBeginTag("span");

                writer.WriteEncodedText(this.Text);


                writer.RenderEndTag(); // SPAN_text
                writer.RenderEndTag(); // SPAN_inner
                writer.RenderEndTag(); // SPAN_inner-outer
                writer.RenderEndTag(); // SPAN_outer
                writer.RenderEndTag(); // A
            }
            finally
            {
                // Make sure we "end" the rendering, no matter what.
                writer.EndRender();
            }

            // These are not the droids you're looking for...
            //base.Render(writer);
        }
        #endregion
    }
    public enum ButtonStyleTheme : uint
    {
        Custom = 0,
        Standard,
        Classic,
        Fancy,
        Plastic
    }
    public enum ButtonColorTheme : uint
    {
        Blue = 0,
        Gray,
        Green,
        Pink,
        Purple,
        Red,
        Yellow
    }
}
