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
using System.Web;
using System.Web.UI;

// NOTE:  To use the ChevronHeader control, an HttpHandler must be referenced in the application's web.config file.
// 
// <add path="chevron.axd" verb="*" type="RainstormStudios.Web.HttpHandlers.ChevronImageHandler, RainstormStudios.Web"/>
// 

namespace RainstormStudios.Web.UI.WebControls
{
    [ToolboxData("<{0}:ChevronHeader runat=\"server\"></{0}:ChevronHeader>"), DefaultProperty("Items"), ParseChildren(true), PersistChildren(false), AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ChevronHeader : System.Web.UI.Control
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private ChevronItemCollection
            _itemsCol;
        private int
            _fontPadding,
            _totalWidth = -1,
            _itemWidth = -1,
            _itemHeight = -1,
            _itemSpc = -1,
            _itemAlpha = 255,
            _compAlpha = 255,
            _actvAlpha = 255,
            _brdrAlpha = 180,
            _bdrWidth = 1,
            _accentBaseOpac = 25,
            _txtShadowOpac = 90;
        //private bool
        //    _bevel = false;
        private Color
            _backClr = Color.Empty,
            _bckClr1 = Color.Empty,
            _bckClr2 = Color.Empty,
            _actClr1 = Color.Empty,
            _actClr2 = Color.Empty,
            _cmpClr1 = Color.Empty,
            _cmpClr2 = Color.Empty,
            _brdrClr = Color.Empty,
            _foreClr = Color.Empty;
        private System.Web.UI.WebControls.FontUnit
            _fontSz = new System.Web.UI.WebControls.FontUnit("10pt");
        private bool
            _fontBold = false,
            _fontItalic = false,
            _allowWrap = true,
            _txtShadow = false;
        private string
            _fontName = "Arial",
            _shapeFileUrl = string.Empty,
            _shapeName = string.Empty;
        private ChevronAccent
            _accentMode = ChevronAccent.None;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ChevronItemCollection Items
        {
            get { return this._itemsCol; }
        }
        public int TotalWidthPixels
        {
            get { return this._totalWidth; }
            set { this._totalWidth = value; }
        }
        public int ItemWidthPixels
        {
            get { return this._itemWidth; }
            set { this._itemWidth = value; }
        }
        public int ItemHeightPixels
        {
            get { return this._itemHeight; }
            set { this._itemHeight = value; }
        }
        public int ItemSpacing
        {
            get { return (this._itemSpc >= 0 ? this._itemSpc : 2); }
            set { this._itemSpc = value; }
        }
        [Browsable(false)]
        public int ActiveItemIndex
        {
            get
            {
                object vsVal = this.ViewState["ItemAlpha"];
                int iVsVal;
                if (vsVal == null || !int.TryParse(vsVal.ToString(), out iVsVal))
                    return -1;
                else
                    return iVsVal;
            }
            set { this.ViewState["ItemAlpha"] = value; }
        }
        public int ItemAlpha
        {
            get { return this._itemAlpha; }
            set { this._itemAlpha = System.Math.Max(0, System.Math.Min(255, value)); }
        }
        public int CompletedItemAlpha
        {
            get { return this._compAlpha; }
            set { this._compAlpha = System.Math.Max(0, System.Math.Min(255, value)); }
        }
        public int ActiveItemAlpha
        {
            get { return this._actvAlpha; }
            set { this._actvAlpha = System.Math.Max(0, System.Math.Min(255, value)); }
        }
        public int BorderAlpha
        {
            get { return this._brdrAlpha; }
            set { this._brdrAlpha = System.Math.Max(0, System.Math.Min(255, value)); }
        }
        public int BorderWidth
        {
            get { return this._bdrWidth; }
            set { this._bdrWidth = System.Math.Max(0, value); }
        }
        public Color BackColor
        {
            get { return this._backClr; }
            set { this._backClr = value; }
        }
        public Color ItemBackColor1
        {
            get { return this._bckClr1; }
            set { this._bckClr1 = value; }
        }
        public Color ItemBackColor2
        {
            get { return this._bckClr2; }
            set { this._bckClr2 = value; }
        }
        public Color ActiveItemBackColor1
        {
            get { return this._actClr1; }
            set { this._actClr1 = value; }
        }
        public Color ActiveItemBackColor2
        {
            get { return this._actClr2; }
            set { this._actClr2 = value; }
        }
        public Color CompleteItemBackColor1
        {
            get { return this._cmpClr1; }
            set { this._cmpClr1 = value; }
        }
        public Color CompleteItemBackColor2
        {
            get { return this._cmpClr2; }
            set { this._cmpClr2 = value; }
        }
        public int FontPadding
        {
            get { return this._fontPadding; }
            set { this._fontPadding = value; }
        }
        public ChevronAccent ChevronAccent
        {
            get { return this._accentMode; }
            set { this._accentMode = value; }
        }
        //public bool Bevel
        //{
        // Beveling is currently disabled in the image creator due to the introduction of custom shapes.
        //    get { return this._bevel; }
        //    set { this._bevel = value; }
        //}
        public Color ForeColor
        {
            get { return this._foreClr; }
            set { this._foreClr = value; }
        }
        public Color BorderColor
        {
            get { return this._brdrClr; }
            set { this._brdrClr = value; }
        }
        public System.Web.UI.WebControls.FontUnit FontSize
        {
            get { return this._fontSz; }
            set { this._fontSz = value; }
        }
        public bool FontBold
        {
            get { return this._fontBold; }
            set { this._fontBold = value; }
        }
        public bool FontItalic
        {
            get { return this._fontItalic; }
            set { this._fontItalic = value; }
        }
        public string FontName
        {
            get { return this._fontName; }
            set { this._fontName = value; }
        }
        public bool TextWrap
        {
            get { return this._allowWrap; }
            set { this._allowWrap = value; }
        }
        public bool TextShadow
        {
            get { return this._txtShadow; }
            set { this._txtShadow = value; }
        }
        public int TextShadowAlpha
        {
            get { return this._txtShadowOpac; }
            set { this._txtShadowOpac = value; }
        }
        public int AccentBaseOpacityPercentage
        {
            get { return this._accentBaseOpac; }
            set
            {
                if (value < 1 || value > 100)
                    throw new ArgumentOutOfRangeException("Accent Base Opacity Percentage requires a number between 1 and 100.");
                this._accentBaseOpac = value;
            }
        }
        public string CustomShapeFileUrl
        {
            get { return this._shapeFileUrl; }
            set { this._shapeFileUrl = value; }
        }
        public string CustomShapeName
        {
            get { return this._shapeName; }
            set { this._shapeName = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronHeader()
            : base()
        {
            this._itemsCol = new ChevronItemCollection(this);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Override Methods
        // 
        public void IncrementActiveItem()
        {
            if (this._itemsCol.Count > this.ActiveItemIndex + 1)
            {
                this._itemsCol[this.ActiveItemIndex].IsActive = false;
                this.ActiveItemIndex++;
            }
            this._itemsCol[this.ActiveItemIndex].IsActive = true;
        }
        public void DecrementActiveItem()
        {
            if (this.ActiveItemIndex > 0)
            {
                this._itemsCol[this.ActiveItemIndex].IsActive = false;
                this.ActiveItemIndex--;
            }
            else
                this.ActiveItemIndex = 0;
            this._itemsCol[this.ActiveItemIndex].IsActive = true;
        }
        public void SetActiveItem(int idx)
        {
            if (idx < 0 || idx > this._itemsCol.Count)
                throw new ArgumentOutOfRangeException("Specified index was outside the bounds of the item collection.");

            this._itemsCol[this.ActiveItemIndex].IsActive = false;

            this._itemsCol[idx].IsActive = true;
            this.ActiveItemIndex = idx;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Override Methods
        // 
        protected override void Render(HtmlTextWriter writer)
        {
            writer.BeginRender();
            try
            {
                // Get the filename for the currently executing page.
                string curPgNm = System.IO.Path.GetFileName(this.Context.Request.PhysicalPath).Trim().ToLower();

                StringBuilder sb = new StringBuilder("chevron.axd");

                sb.AppendFormat("?h={0}&spc={1}", this._itemHeight, this._itemSpc);
                if (this._itemWidth != -1)
                    sb.AppendFormat("&itmw={0}", this._itemWidth);
                else
                    sb.AppendFormat("&w={0}", this._totalWidth);
                sb.AppendFormat("&fnt={0}&fsz={1}&tpd={2}", this.FontName, this.FontSize.Unit.Value, this.FontPadding);
                sb.AppendFormat("&bld={0}&itl={1}&twp={2}", (this.FontBold ? 1 : 0), (this.FontItalic ? 1 : 0), (this.TextWrap ? 1 : 0));
                sb.AppendFormat("&tClr={0}&bClr={1}&fClr={2}", Hex.GetWebColor(this.ItemBackColor1).TrimStart('#'), Hex.GetWebColor(this.ItemBackColor2).TrimStart('#'), Hex.GetWebColor(this.ForeColor).TrimStart('#'));
                sb.AppendFormat("&bdrClr={0}&atClr={1}&abClr={2}", Hex.GetWebColor(this.BorderColor).TrimStart('#'), Hex.GetWebColor(this.ActiveItemBackColor1).TrimStart('#'), Hex.GetWebColor(this.ActiveItemBackColor2).TrimStart('#'));
                sb.AppendFormat("&ctClr={0}&cbClr={1}", Hex.GetWebColor(this.CompleteItemBackColor1).TrimStart('#'), Hex.GetWebColor(this.CompleteItemBackColor2).TrimStart('#'));
                sb.AppendFormat("&acMd={0}&acOp={2}&bckClr={1}", (int)this._accentMode, Hex.GetWebColor(this.BackColor).TrimStart('#'), this._accentBaseOpac);
                //sb.AppendFormat("&bvl={2}", this.Bevel);
                sb.AppendFormat("&bdrWdt={0}", this.BorderWidth);
                sb.AppendFormat("&bckTrns={0}&cmpTrns={1}&actTrns={2}&bdrTrns={3}", this.ItemAlpha, this.CompletedItemAlpha, this.ActiveItemAlpha, this.BorderAlpha);
                sb.AppendFormat("&tsOn={0}", (this.TextShadow ? 1 : 0));
                if (this.TextShadow)
                    sb.AppendFormat("&tsOp={0}", this.TextShadowAlpha);

                if (!string.IsNullOrEmpty(this.CustomShapeFileUrl))
                    sb.AppendFormat("&shpFl={0}", this.CustomShapeFileUrl);
                if (!string.IsNullOrEmpty(this.CustomShapeName))
                    sb.AppendFormat("&shpNm={0}", this.CustomShapeName);

                int actIdx = -1;
                List<string> parts = new List<string>();
                List<string> imgs = new List<string>();
                if (this._itemsCol.Count > 0)
                {
                    for (int i = 0; i < this._itemsCol.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(this._itemsCol[i].AssocPageName) && this._itemsCol[i].AssocPageName.Split('|').Any(s => s.Trim().ToLower() == curPgNm))
                            this._itemsCol[i].IsActive = true;

                        parts.Add(Context.Server.UrlEncode(this._itemsCol[i].Text));
                        if (this._itemsCol[i].IsActive)
                            actIdx = i;
                        if (this._itemsCol[i].ImageSrc != null)
                        {
                            string imgUrl = this._itemsCol[i].ImageSrc.OriginalString;
                            if (!this._itemsCol[i].ImageSrc.IsAbsoluteUri && !VirtualPathUtility.IsAbsolute(imgUrl))
                                imgUrl = VirtualPathUtility.ToAbsolute(imgUrl);
                            imgs.Add(Context.Server.UrlEncode(imgUrl));
                        }
                        else
                            imgs.Add(Context.Server.UrlEncode(" "));
                    }
                    if (actIdx == -1)
                    {
                        actIdx = 0;
                        this._itemsCol[0].IsActive = true;
                    }
                }
                sb.AppendFormat("&act={0}", actIdx);
                sb.AppendFormat("&pcs={0}", string.Join("|", parts.ToArray()));
                sb.AppendFormat("&img={0}", string.Join("|", imgs.ToArray()));

                string url = sb.ToString();
                if (url.Length > 2000)
                    throw new Exception("Chevron handler URL exceeds 2000 characters.");

                writer.AddAttribute(HtmlTextWriterAttribute.Src, url);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
            finally
            { writer.EndRender(); }
        }
        #endregion
    }
    public class ChevronItem
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _itemName,
            _assocPg;
        private bool
            _activeItem;
        private ChevronItemType
            _itemType;
        private Uri
            _imgUrl;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public string Text
        {
            get { return this._itemName; }
            set { this._itemName = value; }
        }
        public bool IsActive
        {
            get { return this._activeItem; }
            set { this._activeItem = value; }
        }
        public ChevronItemType ItemType
        {
            get { return this._itemType; }
            set { this._itemType = value; }
        }
        public Uri ImageSrc
        {
            get { return this._imgUrl; }
            set { this._imgUrl = value; }
        }
        public string AssocPageName
        {
            get { return this._assocPg; }
            set { this._assocPg = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronItem()
        { }
        public ChevronItem(string text, bool isAct, ChevronItemType type)
            : this()
        {
            this._itemName = text;
            this._activeItem = isAct;
            this._itemType = type;
        }
        public ChevronItem(string text, bool isAct, ChevronItemType type, Uri imgSrc)
            : this(text, isAct, type)
        {
            this._imgUrl = imgSrc;
        }
        public ChevronItem(string text, ChevronItemType type, string assocPgNm)
            : this(text, false, type)
        {
            this._assocPg = assocPgNm;
        }
        #endregion
    }
    public class ChevronItemCollection : System.Collections.ICollection
    {
        private System.Collections.ArrayList
            _items;
        private ChevronHeader
            _parent;

        public virtual int Count
        { get { return this._items.Count; } }
        public virtual ChevronItem this[int index]
        { get { return (ChevronItem)this._items[index]; } }
        public virtual bool IsReadOnly
        { get { return false; } }
        public virtual bool IsSynchronized
        { get { return false; } }
        public virtual object SyncRoot
        { get { return this; } }
        public virtual ChevronHeader Parent
        {
            get { return this._parent; }
            internal set { this._parent = value; }
        }

        internal ChevronItemCollection(ChevronHeader parent)
        {
            this._items = new System.Collections.ArrayList();
            this._parent = parent;
        }

        public virtual void CopyTo(Array array, int index)
        { _items.CopyTo(array, index); }

        public virtual System.Collections.IEnumerator GetEnumerator()
        { return _items.GetEnumerator(); }

        public virtual void Add(ChevronItem item)
        {
            if (item.IsActive)
                for (int i = 0; i < this.Count; i++)
                    if (this[i].IsActive)
                        this[i].IsActive = false;

            int myIdx = this._items.Add(item);
            if (item.IsActive)
                this._parent.ActiveItemIndex = myIdx;
        }
    }
    public enum ChevronItemType
    {
        Item = 0,
        AlternatingItem
    }
    public class ChevronItemControlDesigner : System.Web.UI.Design.ControlDesigner
    {
        private System.ComponentModel.Design.DesignerActionListCollection
            _actionLists = null;

        public override bool AllowResize
        { get { return false; } }
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (this._actionLists == null)
                {
                    this._actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this._actionLists.AddRange(base.ActionLists);

                    this._actionLists.Add(new ActionList(this));
                }
                return this._actionLists;
            }
        }

        public class ActionList : System.ComponentModel.Design.DesignerActionList
        {
            private ChevronItemControlDesigner
                _parent;
            private System.ComponentModel.Design.DesignerActionItemCollection
                _items;

            public ActionList(ChevronItemControlDesigner parent)
                : base(parent.Component)
            {
                this._parent = parent;
            }

            public override System.ComponentModel.Design.DesignerActionItemCollection GetSortedActionItems()
            {
                if (this._items == null)
                {
                    this._items = new System.ComponentModel.Design.DesignerActionItemCollection();
                    this._items.Add(new System.ComponentModel.Design.DesignerActionMethodItem(this, "AddChevronItem", "Add Chevron Item", true));
                }
                return this._items;
            }

            private void AddChevronItem()
            {
            }
        }
    }
    public enum ChevronAccent : uint
    {
        None = 0,
        Shadow = 1,
        Reflection = 2
    }
}
