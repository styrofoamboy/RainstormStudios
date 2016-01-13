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

namespace RainstormStudios.Web.UI.WebControls
{
    [DefaultProperty("Items"), ParseChildren(ChildrenAsProperties = true), ToolboxData("<{0}:SlideShow runat=\"server\"></{0}:SlideShow>")]
    public class SlideShow : System.Web.UI.Control, System.Web.UI.INamingContainer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        SlideShowItemCollection
            _items = new SlideShowItemCollection();
        Unit
            _width,
            _height;
        int
            _delay = 4000,
            _startFrm = 0,
            _zIdx = 1,
            _fadeDur = -1;
        string
            _imgFldr,
            _imgFldrMask = "*";
        bool
            _imgFldrSubfldr = false;
        FadeSpeed
            _fdSpd = FadeSpeed.Fast;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public SlideShowItemCollection Items
        { get { return this._items; } }
        public Unit Width
        {
            get { return this._width; }
            set { this._width = value; }
        }
        public Unit Height
        {
            get { return this._height; }
            set { this._height = value; }
        }
        public int FrameDelay
        {
            get { return this._delay; }
            set { this._delay = value; }
        }
        public int StartFrame
        {
            get { return this._startFrm; }
            set { this._startFrm = value; }
        }
        public int zIndex
        {
            get { return this._zIdx; }
            set { this._zIdx = value; }
        }
        public string LoadImagesFrom
        {
            get { return this._imgFldr; }
            set { this._imgFldr = value; }
        }
        public string LoadImagesMask
        {
            get { return this._imgFldrMask; }
            set { this._imgFldrMask = value; }
        }
        public bool LoadImagesInSubfolders
        {
            get { return this._imgFldrSubfldr; }
            set { this._imgFldrSubfldr = value; }
        }
        public int FadeDuration
        {
            get { return this._fadeDur; }
            set { this._fadeDur = value; }
        }
        public FadeSpeed FadeSpeed
        {
            get { return this._fdSpd; }
            set { this._fdSpd = value; }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            Control lnkCheck = this.Page.Header.FindControl("SlideShowCSS");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "SlideShowCSS";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.style.slideShow.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            string scriptPathRef = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.scripts.slideShow.js");
            this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "SlideShowScript", scriptPathRef);

            StringBuilder sbInitScript = new StringBuilder("$(document).ready(function () { initSlideshow(");
            sbInitScript.AppendFormat("'{0}', {1}, {2}", this.ID, this.FrameDelay, this.StartFrame);
            if (this._fadeDur > -1)
                sbInitScript.AppendFormat(", {0}", this._fadeDur);
            else
                sbInitScript.Append(", 'fast'");
            sbInitScript.Append("); });");
            this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), this.ClientID + "_ScriptInit", sbInitScript.ToString(), true);

            base.OnPreRender(e);
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.BeginRender();
            try
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "slide-images");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                for (int i = 0; i < this._items.Count; i++)
                    this.CreateListItem(writer, this._items[i].ImageUrl, this._items[i].AltText);

                if (!string.IsNullOrEmpty(this._imgFldr))
                {
                    string vPath = System.Web.VirtualPathUtility.ToAbsolute(this._imgFldr);
                    string physPath = this.Context.Server.MapPath(vPath);
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(physPath);
                    if (di.Exists)
                    {
                        System.IO.FileInfo[] fiImgs = di.GetFiles(this._imgFldrMask, (this.LoadImagesInSubfolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly));
                        for (int i = 0; i < fiImgs.Length; i++)
                        {
                            //int folderStart = fiImgs[i].FullName.LastIndexOf(vPath.Replace('/', '\\'));
                            //string fiVirtualPath = fiImgs[i].FullName.Substring(folderStart);
                            string fiVirtualPath = System.IO.Path.Combine(vPath, fiImgs[i].Name);
                            string fullImgPath = System.Web.VirtualPathUtility.ToAbsolute(System.Web.VirtualPathUtility.ToAppRelative(fiVirtualPath));
                            this.CreateListItem(writer, fullImgPath, "");
                        }
                    }
                }

                writer.RenderEndTag(); // UL
            }
            finally
            {
                writer.EndRender();
            }

            // These are not the droids you're looking for.
            //base.Render(writer);
        }
        private void CreateListItem(System.Web.UI.HtmlTextWriter writer, string imgUrl, string altTxt)
        {
            writer.RenderBeginTag("li");

            writer.AddAttribute(HtmlTextWriterAttribute.Src, System.Web.VirtualPathUtility.ToAbsolute(imgUrl));
            writer.AddAttribute(HtmlTextWriterAttribute.Alt, altTxt);
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }
        #endregion
    }
    [PersistChildren(true)]
    public class SlideShowItem : System.Web.UI.INamingContainer
    {
        public string
            ImageUrl { get; set; }
        public string
            AltText { get; set; }

        public SlideShowItem()
        { }
        public SlideShowItem(string imgUrl)
            : this()
        {
            this.ImageUrl = imgUrl;
        }
        public SlideShowItem(string imgUrl, string altText)
            : this(imgUrl)
        {
            this.AltText = altText;
        }
    }
    [PersistChildren(true)]
    public class SlideShowItemCollection : RainstormStudios.Collections.ObjectCollectionBase<SlideShowItem>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public SlideShowItemCollection()
            : base()
        { }
        public SlideShowItemCollection(SlideShowItem[] items)
            : base(items, (items != null) ? items.Select(i => i.ImageUrl).ToArray() : new string[0])
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(string imgUrl)
        {
            this.Add(new SlideShowItem(imgUrl));
        }
        public void Add(SlideShowItem item)
        {
            if (base.ContainsKey(item.ImageUrl))
                throw new Exception("You cannot add the same slide show item more than once.");

            base.Add(item, item.ImageUrl);
        }
        public new void Sort(Collections.SortDirection dir = Collections.SortDirection.Ascending)
        {
            base.Sort("ImageUrl", dir);
        }
        #endregion
    }
    public enum FadeSpeed
    {
        Fast = 0,
        Slow
    }
}
