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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    [DefaultProperty("FrameTitle"), ParseChildren(true), ToolboxData("<{0}:CustomFrame runat=\"server\"></{0}:CustomFrame>")]
    public class CustomFrame : System.Web.UI.WebControls.Panel
    {
        #region Nested Classes
        //***************************************************************************
        // Nested Classes
        // 
        public enum OverflowType
        {
            NotSet = -1,
            Auto = 0,
            Hidden = 1,
            Scroll,
            Visible,
            Inherit
        }
        public enum FrameTheme : int
        {
            Blue = 1,
            Green = 2
        }
        internal enum PieceName : int
        {
            TL = 0, T, TR,
            L, Ls, C, R, Rs,
            BL, B, BR
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _title = string.Empty,
            _titleImg = string.Empty;
        private Unit
            _padding = Unit.Empty,
            _paddingt = Unit.Empty,
            _paddingb = Unit.Empty,
            _paddingl = Unit.Empty,
            _paddingr = Unit.Empty,
            _minw = Unit.Empty,
            _maxw = Unit.Empty,
            _minh = Unit.Empty,
            _maxh = Unit.Empty,
            _sideW = Unit.Empty,
            _hdrH = Unit.Empty,
            _ftrH = Unit.Empty;
        private Style
            _titleStyle = null;
        private OverflowType
            _ovrFlw = OverflowType.NotSet,
            _xOvrFlw = OverflowType.NotSet,
            _yOvrFlw = OverflowType.NotSet;
        private FrameTheme
            _theme = FrameTheme.Blue;
        private string
            _imgTL,
            _imgT,
            _imgTR,
            _imgL,
            _imgLs,
            _imgC,
            _imgR,
            _imgRs,
            _imgBL,
            _imgB,
            _imgBR,
            _imgRscNm,
            _frmCssClsNm;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string FrameClassName
        {
            get { return this._frmCssClsNm; }
            set { this._frmCssClsNm = value; }
        }
        public string FrameTitle
        {
            get { return this._title; }
            set { this._title = value; }
        }
        public string FrameTitleImage
        {
            get { return this._titleImg; }
            set { this._titleImg = value; }
        }
        public Style TitleStyle
        {
            get { return this._titleStyle; }
        }
        public Color TitleForeColor
        {
            get { return this._titleStyle.ForeColor; }
            set { this._titleStyle.ForeColor = value; }
        }
        public string TitleFontNames
        {
            get { return string.Join(",", this._titleStyle.Font.Names); }
            set { this._titleStyle.Font.Names = value.Split(','); }
        }
        public bool TitleFontBold
        {
            get { return this._titleStyle.Font.Bold; }
            set { this._titleStyle.Font.Bold = value; }
        }
        public bool TitleFontItalic
        {
            get { return this._titleStyle.Font.Italic; }
            set { this._titleStyle.Font.Italic = value; }
        }
        public FontUnit TitleFontSize
        {
            get { return this._titleStyle.Font.Size; }
            set { this._titleStyle.Font.Size = value; }
        }
        public bool TitleFontOverline
        {
            get { return this._titleStyle.Font.Overline; }
            set { this._titleStyle.Font.Overline = value; }
        }
        public bool TitleFontStrikeOut
        {
            get { return this._titleStyle.Font.Strikeout; }
            set { this._titleStyle.Font.Strikeout = value; }
        }
        public bool TitleFontUnderline
        {
            get { return this._titleStyle.Font.Underline; }
            set { this._titleStyle.Font.Underline = value; }
        }
        public FrameTheme ColorThemeTheme
        {
            get { return this._theme; }
            set { this._theme = value; }
        }
        public Unit Padding
        {
            get { return this._padding; }
            set
            {
                this._padding = value;
                this._paddingb = Unit.Empty;
                this._paddingl = Unit.Empty;
                this._paddingr = Unit.Empty;
                this._paddingt = Unit.Empty;
            }
        }
        public Unit PaddingLeft
        {
            get { return this._paddingl; }
            set
            {
                this._paddingl = value;
                this.ClearPaddingAll();
            }
        }
        public Unit PaddingRight
        {
            get { return this._paddingr; }
            set
            {
                this._paddingr = value;
                this.ClearPaddingAll();
            }
        }
        public Unit PaddingTop
        {
            get
            {
                return (this._paddingt == Unit.Empty)
                          ? new Unit("4px")
                          : this._paddingt;
            }
            set
            {
                this._paddingt = value;
                this.ClearPaddingAll();
            }
        }
        public Unit PaddingBottom
        {
            get { return this._paddingb; }
            set
            {
                this._paddingb = value;
                this.ClearPaddingAll();
            }
        }
        public Unit MinimumWidth
        {
            get { return this._minw; }
            set { this._minw = value; }
        }
        public Unit MaximumWidth
        {
            get { return this._maxw; }
            set { this._maxw = value; }
        }
        public OverflowType Overflow
        {
            get { return this._ovrFlw; }
            set
            {
                this._ovrFlw = value;
                this._xOvrFlw = OverflowType.NotSet;
                this._yOvrFlw = OverflowType.NotSet;
            }
        }
        public OverflowType OverflowX
        {
            get { return this._xOvrFlw; }
            set
            {
                this._xOvrFlw = value;
                if (this._yOvrFlw == OverflowType.NotSet)
                    this._yOvrFlw = this._ovrFlw;
                this._ovrFlw = OverflowType.NotSet;
            }
        }
        public OverflowType OverflowY
        {
            get { return this._yOvrFlw; }
            set
            {
                this._yOvrFlw = value;
                if (this._xOvrFlw == OverflowType.NotSet)
                    this._xOvrFlw = this._ovrFlw;
                this._ovrFlw = OverflowType.NotSet;
            }
        }
        public Unit FrameSideWidth
        {
            get
            {
                return (this._sideW == Unit.Empty)
                          ? new Unit("15px")
                          : this._sideW;
            }
            set
            {
                if (value.Type != UnitType.Pixel)
                    throw new Exception("Frame side width must be designated in absolute pixels.");
                this._sideW = value;
            }
        }
        public Unit FrameHeaderHeight
        {
            get
            {
                return (this._hdrH == Unit.Empty)
                        ? new Unit("31px")
                        : this._hdrH;
            }
            set
            {
                if (value.Type != UnitType.Pixel)
                    throw new Exception("Frame header height must be designated in aboslute pixels.");
                this._hdrH = value;
            }
        }
        public Unit FrameFooterHeight
        {
            get
            {
                return (this._ftrH == Unit.Empty)
                        ? new Unit("13px")
                        : this._ftrH;
            }
            set
            {
                if (value.Type != UnitType.Pixel)
                    throw new Exception("Frame footer height must be designated in absolute pixels.");
                this._ftrH = value;
            }
        }
        public Unit MinimumHeight
        {
            get
            {
                return (this._minh == Unit.Empty)
                        ? new Unit("155px")
                        : this._minh;
            }
            set { this._minh = value; }
        }
        public Unit MaximumHeight
        {
            get { return this._maxh; }
            set { this._maxh = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CustomFrame()
        {
            this._titleStyle = new Style();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Base Overrides
        // 
        protected override void OnPreRender(EventArgs e)
        {
            Control lnkCheck = this.Page.Header.FindControl("CustomFrameCss");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "CustomFrameCss";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.CustomFrame), "RainstormStudios.Web.UI.WebControls.style.customFrame.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            base.OnPreRender(e);
        }
        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.BeginRender();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            if (this.MinimumWidth != Unit.Empty)
                writer.AddStyleAttribute("min-width", this.MinimumWidth.ToString());
            if (this.MaximumWidth != Unit.Empty)
                writer.AddStyleAttribute("max-width", this.MaximumWidth.ToString());
            writer.AddStyleAttribute("min-height", this.MinimumHeight.ToString());
            if (this.MaximumHeight != Unit.Empty)
                writer.AddStyleAttribute("max-height", this.MaximumHeight.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if (this.Width != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            if (this.Height != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
            writer.AddStyleAttribute("empty-cells", "show");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "rsFrameStyle1 " + this.ColorThemeTheme.ToString().ToLower());
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.Write(writer.NewLine);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "top");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "l");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "c");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            if (!string.IsNullOrEmpty(this._titleImg))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, this.ResolveUrl(this._titleImg));
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
            }
            this._titleStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(this.FrameTitle);
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.AddAttribute(CssClass, "r");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.RenderEndTag();  // tr
            writer.Write(writer.NewLine);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "middle");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "l");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "s");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.RenderEndTag();

            writer.RenderEndTag(); // td

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "c");
            writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(writer.NewLine);
            if (this._ovrFlw != OverflowType.NotSet)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, this._ovrFlw.ToString());
            else
            {
                if (this._xOvrFlw != OverflowType.NotSet)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, this._xOvrFlw.ToString());
                if (this._yOvrFlw != OverflowType.NotSet)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, this._yOvrFlw.ToString());
            }
            //writer.AddStyleAttribute("min-height", "121px");
            if (this.MinimumHeight != Unit.Empty && this.MinimumHeight.Type != UnitType.Percentage && this.FrameHeaderHeight != Unit.Empty && this.FrameFooterHeight != Unit.Empty)
                writer.AddStyleAttribute("min-height", new Unit(this.MinimumHeight.Value - (this.FrameHeaderHeight.Value + this.FrameFooterHeight.Value), this.MinimumHeight.Type).ToString());
            int iTst, iCnt = 1;
            if (this.Width.Type != UnitType.Percentage)
            {
                while (int.TryParse(this.Width.ToString().Substring(0, iCnt), out iTst))
                    iCnt++;
                string divWidth = Convert.ToString(this.Width.Value - (this.FrameSideWidth.Value * 2)) + ((iCnt < this.Width.ToString().Length) ? this.Width.ToString().Substring(iCnt - 1).Trim() : "");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, divWidth);
            }
            if (this.Height != Unit.Empty && this.Height.Type != UnitType.Percentage)
            {
                iCnt = 1;
                while (int.TryParse(this.Height.ToString().Substring(0, iCnt), out iTst))
                    iCnt++;
                string divHeight = Convert.ToString(this.Height.Value - (this.FrameHeaderHeight.Value + this.FrameFooterHeight.Value)) + ((iCnt < this.Height.ToString().Length) ? this.Height.ToString().Substring(iCnt - 1).Trim() : "");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, divHeight);
            }
            if (this.Padding != Unit.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, this.Padding.ToString());
            else
            {
                if (this.PaddingBottom != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, this.PaddingBottom.ToString());
                if (this.PaddingLeft != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, this.PaddingLeft.ToString());
                if (this.PaddingRight != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, this.PaddingRight.ToString());
                if (this.PaddingTop != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, this.PaddingTop.ToString());
            }
            if (this.ForeColor != Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Color, RainstormStudios.Hex.GetWebColor(this.ForeColor));
            if (this.BackColor != Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, RainstormStudios.Hex.GetWebColor(this.BackColor));
            if (!string.IsNullOrEmpty(this.CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
        }
        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.RenderEndTag(); // DIV-Content
            writer.RenderEndTag(); // TD

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "r");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "s");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.RenderEndTag();

            writer.RenderEndTag(); // td

            writer.RenderEndTag();  // tr
            writer.Write(writer.NewLine);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "bottom");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "l");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "c");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "r");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.RenderEndTag();  // tr
            writer.Write(writer.NewLine);
            writer.RenderEndTag();  // table
            writer.RenderEndTag();  // div
            writer.EndRender();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private void ClearPaddingAll()
        {
            if (this._padding != Unit.Empty)
            {
                if (this._paddingb == Unit.Empty)
                    this._paddingb = this._padding;
                if (this._paddingl == Unit.Empty)
                    this._paddingl = this._padding;
                if (this._paddingr == Unit.Empty)
                    this._paddingr = this._padding;
                if (this._paddingt == Unit.Empty)
                    this._paddingt = this._padding;
                this._padding = Unit.Empty;
            }
        }
        #endregion
    }
}
