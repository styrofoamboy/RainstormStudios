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
    [ToolboxData("<{0}:PlusOneControl runat=\"server\"></{0}:PlusOneControl>"), DefaultProperty("ItemCount"), ParseChildren(true)]
    public class PlusOneControl : System.Web.UI.WebControls.CompositeControl, INamingContainer, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private PlusOneControlItemCollection
            _items;
        private ITemplate
            _template,
            _separator,
            _header,
            _footer;
        //***************************************************************************
        // Public Events
        // 
        public event PlusOneItemCommandEventHandler
            ItemCommand;
        public event PlusOneItemEventHandler
            ItemRemoved;
        public event PlusOneItemEventHandler
            ItemAdded;
        public event PlusOneItemEventHandler
            ItemCreated;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [TemplateContainer(typeof(PlusOneControl))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ItemTemplate
        {
            get { return this._template; }
            set { this._template = value; }
        }

        [TemplateContainer(typeof(PlusOneControl))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate SeparatorTemplate
        {
            get { return this._separator; }
            set { this._separator = value; }
        }

        [TemplateContainer(typeof(PlusOneControl))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate
        {
            get { return this._header; }
            set { this._header = value; }
        }

        [TemplateContainer(typeof(PlusOneControl))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FooterTemplate
        {
            get { return this._footer; }
            set { this._footer = value; }
        }

        public int ItemCount
        {
            get { return this.ViewState.GetInteger("ItemCount", 0); }
            set { this.ViewState["ItemCount"] = value; }
        }

        /// <summary>
        /// The collection of <see cref="T:PlusOneControlItem"/> controls that are part of the control.
        /// </summary>
        [Browsable(false)]
        public PlusOneControlItemCollection Items
        {
            get
            {
                this.EnsureChildControls();
                return this._items;
            }
        }

        /// <summary>
        /// A <see cref="T:System.Boolean"/> value indicating 'true' if each item should automatically build a 'Remove Item' button.  Otherwise, false.
        /// </summary>
        [Description("A boolean value indicating 'true' if each item should automatically build a 'Remove Item' button.  Otherwise, false.")
        , Browsable(true)
        , DefaultValue(false)]
        public bool AutoRemoveControl
        {
            get { return this.ViewState.GetBoolean("autoRemCtrl", false); }
            set { this.ViewState["autoRemCtrl"] = value; }
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
        public PlusOneControl()
            : base()
        {
            this._items = new PlusOneControlItemCollection(this);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void CreateNew()
        {
            this.ItemCount++;

            for (int i = 0; i < this.Controls.Count; i++)
            {
                PlusOneControlItem ctrl = (this.Controls[i] as PlusOneControlItem);
                if (ctrl == null)
                    continue;

                // This works off the premise that we'll never have more than one
                //   PlusOneControlItem set to Visible=false.
                if (ctrl.Visible == false)
                {
                    ctrl.Visible = true;
                    if (this._separator != null)
                    {
                        // We don't want to create the seperator until we're actually
                        //   ready to show the "exta" item.  This *does* mean that
                        //   controls within the separator template cannot participate
                        //   in the view state.
                        Panel panSep = new Panel();
                        this._separator.InstantiateIn(panSep);
                        for (int t = 0; t < panSep.Controls.Count; t++)
                            this.Controls.AddAt(i + t, panSep.Controls[t]);
                    }
                    this._items.Add(ctrl);
                    this.OnItemAdded(new PlusOneItemEventArgs(ctrl));
                    break;
                }
            }
        }
        public void RemoveItemAt(int idx)
        {
            if (idx < 0 || idx > this._items.Count)
                throw new ArgumentOutOfRangeException("idx");

            PlusOneControlItem item = this._items[idx];
            this.Controls.Remove(item);
            this._items.RemoveAt(idx);
            this.ItemCount--;
            this.OnItemRemoved(new PlusOneItemEventArgs(item));
        }
        public void RemoveItem(PlusOneControlItem item)
        {
            int idx = this.Controls.IndexOf(item);
            if (idx == -1)
                throw new ArgumentOutOfRangeException("item", "Specified item was not found in the control.");
            else
                this.RemoveItemAt(idx);
        }
        public void RaisePostDataChangedEvent()
        {
        }
        public void RaisePostBackEvent(string val)
        {
        }
        public bool LoadPostData(string val, System.Collections.Specialized.NameValueCollection data)
        {
            return true;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            this.EnsureChildControls();

            base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            // Build the header.
            if (this._header != null)
                // We're just going to render it directly into the panel, since we
                //   don't want to "mess up" and formating the user might try and
                //   do here.
                this._header.InstantiateIn(this);

            // Build the items.
            for (int i = 0; i < this.ItemCount; i++)
            {
                if (i > 0 && this._separator != null)
                    this._separator.InstantiateIn(this);

                PlusOneControlItem item = new PlusOneControlItem(this, this.AutoRemoveControl, null, this._items.Count);
                if (this._template != null)
                    this._template.InstantiateIn(item);

                item.AutoRemoveButtonClicked += new CommandEventHandler(this.item_OnAutoRemoveButtonClicked);

                this._items.Add(item);
                this.Controls.Add(item);
                this.OnItemCreated(new PlusOneItemEventArgs(item));
            }

            // Create an "extra" hidden control item.
            PlusOneControlItem itemHdn = new PlusOneControlItem(this, this.AutoRemoveControl, null, this._items.Count);
            itemHdn.Visible = false;
            if (this._template != null)
                this._template.InstantiateIn(itemHdn);
            this.Controls.Add(itemHdn);

            // Build the footer.
            if (this._footer != null)
                // Same as the header, just render it exactly as specified.
                this._footer.InstantiateIn(this);

            base.CreateChildControls();
        }
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            if (args is CommandEventArgs)
            {
                CommandEventArgs cmdArgs = (CommandEventArgs)args;
                if (cmdArgs.CommandName == "RemoveItem")
                {
                    int idx;
                    if (int.TryParse(cmdArgs.CommandArgument.ToString(), out idx))
                        this.RemoveItemAt(idx);
                }
                else
                {
                    this.OnItemCommand(new PlusOneItemCommandEventArgs(this, source, cmdArgs));
                }
                return base.OnBubbleEvent(source, args);
            }
            return false;
        }
        //***************************************************************************
        // Event Triggers
        // 
        protected virtual void OnItemCommand(PlusOneItemCommandEventArgs e)
        {
            if (this.ItemCommand != null)
                this.ItemCommand.Invoke(this, e);
        }
        protected virtual void OnItemRemoved(PlusOneItemEventArgs e)
        {
            if (this.ItemRemoved != null)
                this.ItemRemoved.Invoke(this, e);
        }
        protected virtual void OnItemAdded(PlusOneItemEventArgs e)
        {
            if (this.ItemAdded != null)
                this.ItemAdded.Invoke(this, e);
        }
        protected virtual void OnItemCreated(PlusOneItemEventArgs e)
        {
            if (this.ItemCreated != null)
                this.ItemCreated.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        protected void item_OnAutoRemoveButtonClicked(object sender, CommandEventArgs e)
        {
            int idx;
            if (!int.TryParse(e.CommandArgument.ToString(), out idx))
                throw new Exception("Unable to determine item index from event args.");
            this.RemoveItemAt(idx);
        }
        #endregion
    }
    public sealed class PlusOneControlItem : System.Web.UI.WebControls.CompositeControl, IPostBackDataHandler
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private PlusOneControl
            _owner;
        private bool
            _autoRemBtn;
        private string
            _autoRemBtnImgUrl;
        private ImageButton
            _imgBtnRemove;
        //***************************************************************************
        // Public Fields
        // 
        public int
            Index { get; private set; }
        //***************************************************************************
        // Public Events
        // 
        public event CommandEventHandler
            AutoRemoveButtonClicked;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public new PlusOneControl Parent
        { get { return this._owner; } }
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        { get { return HtmlTextWriterTag.Span; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal PlusOneControlItem(PlusOneControl parent, bool autoRemBtn, string autoRemBtnImgUrl, int idx)
            : base()
        {
            this._owner = parent;
            this._autoRemBtn = autoRemBtn;
            this._autoRemBtnImgUrl = autoRemBtnImgUrl;
            this.Index = idx;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void RaisePostDataChangedEvent()
        {
        }
        public bool LoadPostData(string val, System.Collections.Specialized.NameValueCollection data)
        {
            return true;
        }
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
                // We use "RenderContents" here because it keeps the PlusOneControlItem
                //   itself from rendering any encapsulating markup.
                this.RenderContents(writer);
            }
            finally
            { writer.EndRender(); }

            //base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            if (this._autoRemBtn)
            {
                ImageButton remBtn = new ImageButton();
                this.Controls.Add(remBtn);
                this._imgBtnRemove = remBtn;
                remBtn.ID = "remItem";
                remBtn.CommandName = "RemoveItem";
                remBtn.CommandArgument = this.Index.ToString();
                remBtn.Command += new CommandEventHandler(this.autoRemBtn_OnCommand);
                remBtn.ImageAlign = ImageAlign.Middle;
                remBtn.ImageUrl = string.IsNullOrEmpty(this._autoRemBtnImgUrl)
                                    ? WebUtil.GetWebResourceUrl(this.GetType(), "RainstormStudios.Web.UI.WebControls.images.plusOneControl.remBtn.png")
                                    : this._autoRemBtnImgUrl;
                remBtn.ToolTip = "Remove";
            }

            base.CreateChildControls();
        }
        //***************************************************************************
        // Event Triggers
        // 
        private void OnAutoRemoveButtonClicked(CommandEventArgs e)
        {
            if (this.AutoRemoveButtonClicked != null)
                this.AutoRemoveButtonClicked.Invoke(this, e);
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void autoRemBtn_OnCommand(object sender, CommandEventArgs e)
        {
            this.OnAutoRemoveButtonClicked(e);
        }
        #endregion
    }
    public sealed class PlusOneControlItemCollection : System.Collections.CollectionBase, ICollection<PlusOneControlItem>
    {
        #region Declaration
        //***************************************************************************
        // Private Fields
        // 
        private PlusOneControl
            _owner;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public PlusOneControlItem this[int idx]
        {
            get { return (PlusOneControlItem)base.InnerList[idx]; }
            set { this.InnerList[idx] = value; }
        }
        public bool IsReadOnly
        { get { return false; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PlusOneControlItemCollection(PlusOneControl parent)
        {
            this._owner = parent;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Add(PlusOneControlItem item)
        { this.InnerList.Add(item); }
        public void AddRange(System.Collections.ICollection items)
        { base.InnerList.AddRange(items); }
        public bool Remove(PlusOneControlItem item)
        {
            int idx = -1;
            try
            { idx = base.InnerList.IndexOf(item); }
            catch (ArgumentOutOfRangeException)
            { return false; }

            if (idx < 0)
                return false;

            base.InnerList.RemoveAt(idx);
            return true;
        }
        public bool Contains(PlusOneControlItem item)
        { return base.InnerList.Contains(item); }
        public void CopyTo(PlusOneControlItem[] arr, int idx)
        { base.InnerList.CopyTo(arr, idx); }
        public new System.Collections.Generic.IEnumerator<PlusOneControlItem> GetEnumerator()
        { return new PlusOneControlItemEnumerator(this); }
        #endregion
    }
    public sealed class PlusOneControlItemEnumerator : System.Collections.Generic.IEnumerator<PlusOneControlItem>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        PlusOneControlItemCollection
            _collection;
        int
            _curIdx;
        PlusOneControlItem
            _curVal;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public PlusOneControlItem Current
        { get { return this._curVal; } }
        object System.Collections.IEnumerator.Current
        { get { return this.Current; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal PlusOneControlItemEnumerator(PlusOneControlItemCollection collection)
        {
            this._collection = collection;
            this._curIdx = -1;
            this._curVal = null;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public bool MoveNext()
        {
            if (++this._curIdx >= this._collection.Count)
                return false;

            else
                this._curVal = this._collection[this._curIdx];

            return true;
        }
        public void Reset()
        { this._curIdx = -1; }
        void IDisposable.Dispose()
        { }
        #endregion
    }
    public delegate void PlusOneItemCommandEventHandler(object sender, PlusOneItemCommandEventArgs e);
    public class PlusOneItemCommandEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private PlusOneControl
            _mSrc;
        private object
            _cSrc;
        private CommandEventArgs
            _args;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public PlusOneControl
            PlusOneControlSource { get { return this._mSrc; } }
        public object
            ControlSource { get { return this._cSrc; } }
        public CommandEventArgs
            Args { get { return this._args; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PlusOneItemCommandEventArgs(PlusOneControl mSrc, object cSrc, CommandEventArgs args)
        {
            this._mSrc = mSrc;
            this._cSrc = cSrc;
            this._args = args;
        }
        #endregion
    }
    public delegate void PlusOneItemEventHandler(object sender, PlusOneItemEventArgs e);
    public class PlusOneItemEventArgs : EventArgs
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public PlusOneControlItem
            Item { get; private set; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PlusOneItemEventArgs(PlusOneControlItem item)
        {
            this.Item = item;
        }
        #endregion
    }
}
