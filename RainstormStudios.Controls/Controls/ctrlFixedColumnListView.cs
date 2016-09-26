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
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace RainstormStudios.Controls
{
    [System.Drawing.ToolboxBitmap(typeof(ListView))]
    public class FixedColumnListView : ListView
    {
        #region Sub-Classes
        //***************************************************************************
        // Sub-Classes
        // 
        public class FixedColumnHeader : ColumnHeader
        {
            #region Declarations
            //-----------------------------------------------------------------------
            // Global Variables
            // 
            int _size;
            SizeType _type;
            #endregion

            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public new int Width
            { get { return this._size; } set { this._size = value; } }
            public SizeType SizeType
            { get { return this._type; } set { this._type = value; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public FixedColumnHeader()
                : this(60, SizeType.Absolute)
            { }
            public FixedColumnHeader(int imageIndex)
                : this()
            {
                base.ImageIndex = imageIndex;
            }
            public FixedColumnHeader(string imageKey)
                : this()
            {
                base.ImageKey = imageKey;
            }
            public FixedColumnHeader(int width, SizeType sizeMode)
            {
                this._type = sizeMode;
                this._size = width;
            }
            public FixedColumnHeader(string colName, string headerText, int width, SizeType sizeMode)
                : this(width, sizeMode)
            {
                this.Name = colName;
                this.Text = headerText;
            }
            #endregion

            #region Public Methods
            //-----------------------------------------------------------------------
            // Public Methods
            // 
            public ColumnHeader GetColumnHeader()
            {
                using (ColumnHeader ch = new ColumnHeader())
                {
                    ch.Name = this.Name;
                    ch.Tag = this.Tag;
                    ch.Text = this.Text;
                    ch.TextAlign = this.TextAlign;
                    ch.Width = this.Width;
                    ch.ImageIndex = this.ImageIndex;
                    ch.ImageKey = this.ImageKey;
                    ch.DisplayIndex = this.DisplayIndex;
                    return (ch.Clone() as ColumnHeader);
                }
            }
            #endregion
        }
        public class FixedColumnHeaderCollection : CollectionBase
        {
            #region Declarations
            //-----------------------------------------------------------------------
            // Global Variables
            // 
            //ColumnHeaderCollection _hCol;
            ListView _owner;
            //-----------------------------------------------------------------------
            // Delegates
            // 
            public delegate void ColumnChangedEventHandler(object sender, ColumnsChangedEventArgs e);
            //-----------------------------------------------------------------------
            // Public Events
            // 
            public event ColumnChangedEventHandler ColumnAdded;
            public event ColumnChangedEventHandler ColumnRemoved;
            public event EventHandler ColumnsCleared;
            #endregion

            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public FixedColumnHeader this[int index]
            { get { return (FixedColumnHeader)List[index]; } set { List[index] = value; } }
            public FixedColumnHeader this[string key]
            {
                get
                {
                    foreach (object fch in List)
                        if (((FixedColumnHeader)fch).Name == key)
                            return (FixedColumnHeader)fch;
                    return new FixedColumnHeader();
                }
                set
                {
                    for (int i = 0; i < List.Count; i++)
                        if (((FixedColumnHeader)List[i]).Name == key)
                        { List[i] = value; break; }
                }
            }
            public ListView ListView
            { get { return this._owner; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public FixedColumnHeaderCollection(ListView owner)
            {
                this._owner = owner;
                //this._hCol = new ColumnHeaderCollection(this._owner);
            }
            #endregion

            #region Public Methods
            //-----------------------------------------------------------------------
            // Public Methods
            // 
            public int Add(FixedColumnHeader value)
            {
                //value.ListView = this._owner;
                List.Add(value);
                //this._hCol.Add(value.GetColumnHeader());
                ColumnAddedEvent(value);
                return List.Count - 1;
            }
            public FixedColumnHeader Add(string text)
            { return Add(text, 60, SizeType.Absolute, HorizontalAlignment.Left); }
            public FixedColumnHeader Add(string text, int width)
            { return Add(text, width, SizeType.Absolute, HorizontalAlignment.Left); }
            public FixedColumnHeader Add(string text, int width, SizeType sizeMode)
            { return Add(text, width, sizeMode, HorizontalAlignment.Left); }
            public FixedColumnHeader Add(string text, int width, HorizontalAlignment align)
            { return Add(text, width, SizeType.Absolute, align); }
            public FixedColumnHeader Add(string text, int width, SizeType sizeMode, HorizontalAlignment align)
            {
                FixedColumnHeader ch = new FixedColumnHeader();
                ch.Text = text;
                ch.Width = width;
                ch.SizeType = sizeMode;
                ch.TextAlign = align;
                Add(ch);
                return ch;
            }
            public FixedColumnHeader Add(string key, string text)
            { return Add(key, text, 60, SizeType.Absolute); }
            public FixedColumnHeader Add(string key, string text, int width)
            { return Add(key, text, width, SizeType.Absolute, HorizontalAlignment.Left); }
            public FixedColumnHeader Add(string key, string text, int width, SizeType sizeMode)
            { return Add(key, text, width, sizeMode, HorizontalAlignment.Left); }
            public FixedColumnHeader Add(string key, string text, int width, SizeType sizeMode, HorizontalAlignment align)
            {
                FixedColumnHeader ch = new FixedColumnHeader();
                ch.Text = text;
                ch.Width = width;
                ch.SizeType = sizeMode;
                ch.TextAlign = align;
                Add(ch);
                return ch;
            }
            public void Remove(FixedColumnHeader value)
            {
                List.Remove(value);
                //this._hCol.RemoveAt(value.Index);
                ColumnRemovedEvent(value);
            }
            public new void RemoveAt(int index)
            {
                ColumnRemovedEvent((FixedColumnHeader)List[index]);
                List.RemoveAt(index);
            }
            public new void Clear()
            {
                List.Clear();
                ColumnsClearedEvent();
            }
            public int IndexOf(FixedColumnHeader value)
            {
                return InnerList.IndexOf(value);
            }
            public bool Contains(FixedColumnHeader value)
            {
                return InnerList.Contains(value);
            }
            #endregion

            #region Event Triggers
            //-----------------------------------------------------------------------
            // Event Triggers
            // 
            private void ColumnAddedEvent(FixedColumnHeader col)
            {
                if (this.ColumnAdded != null)
                    this.ColumnAdded.Invoke(this, new ColumnsChangedEventArgs(col));
            }
            private void ColumnRemovedEvent(FixedColumnHeader col)
            {
                if (this.ColumnRemoved != null)
                    this.ColumnRemoved.Invoke(this, new ColumnsChangedEventArgs(col));
            }
            private void ColumnsClearedEvent()
            {
                if (this.ColumnsCleared != null)
                    this.ColumnsCleared.Invoke(this, EventArgs.Empty);
            }
            #endregion
        }
        public class ColumnsChangedEventArgs : EventArgs
        {
            #region Declarations
            //-----------------------------------------------------------------------
            // Global Variables
            // 
            FixedColumnHeader _ch;
            #endregion

            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public FixedColumnHeader ColumnHeader
            { get { return this._ch; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public ColumnsChangedEventArgs(FixedColumnHeader column)
            {
                this._ch = column;
            }
            #endregion
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        bool _colResize;
        FixedColumnHeaderCollection _cols;
        //Int32Collection _colWeight;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        [Browsable(true), TypeConverter(typeof(RainstormStudios.Controls.FixedColumnListView.FixedColumnHeader))]
        public new FixedColumnHeaderCollection Columns
        { get { return this._cols; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public FixedColumnListView()
        {
            //this._colWeight = new Int32Collection();
            this._cols = new FixedColumnHeaderCollection(this);
            this._cols.ColumnAdded += new FixedColumnHeaderCollection.ColumnChangedEventHandler(this.columns_onColumnAdded);
            this._cols.ColumnRemoved += new FixedColumnHeaderCollection.ColumnChangedEventHandler(this.columns_onColumnRemoved);
            this._cols.ColumnsCleared += new EventHandler(this.columns_onClear);
            this._colResize = false;
            this.AllowColumnReorder = false;
            this.View = View.Details;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        //public ColumnHeader AddColumn(string colName, string colText, int size)
        //{
        //    this._colWeight.Add(size);
        //    ColumnHeader newCol = new ColumnHeader();
        //    newCol.Name = colName;
        //    newCol.Text = colText;
        //    this.Columns.Add(newCol);
        //    return newCol;
        //}
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected void SetColWidths()
        {
            this._colResize = true;
            FixedColumnHeaderCollection shr = new FixedColumnHeaderCollection(this);
            int defWidth = 0;
            for(int i=0;i<this.Columns.Count;i++)
                switch (this.Columns[i].SizeType)
                {
                    case SizeType.Absolute:
                        this.Columns[i].Width = this.Columns[i].Width;
                        defWidth += this.Columns[i].Width;
                        break;
                    case SizeType.Percent:
                        this.Columns[i].Width = ((this.ClientRectangle.Width - 1) * this.Columns[i].Width) / 100;
                        defWidth += this.Columns[i].Width;
                        break;
                    case SizeType.AutoSize:
                        shr.Add(this.Columns[i]);
                        break;
                }
            foreach (FixedColumnHeader colHeader in shr)
                colHeader.Width = ((this.ClientRectangle.Width - 1) - defWidth) / shr.Count;

            //for (int i = 0; i < this.Columns.Count; i++)
            //    if (this._colWeight.Count > i)
            //        this.Columns[i].Width = ((this.ClientRectangle.Width - 1) * this._colWeight[i]) / 100;
            //    //else
            //    //    this.Columns[i].Width = 60;
            this._colResize = false;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Handlers
        // 
        private void columns_onColumnAdded(object sender, ColumnsChangedEventArgs e)
        {
            base.Columns.Add(e.ColumnHeader);
        }
        private void columns_onColumnRemoved(object sender, ColumnsChangedEventArgs e)
        {
            base.Columns.Remove(e.ColumnHeader);
        }
        private void columns_onClear(object sender, EventArgs e)
        {
            base.Columns.Clear();
        }
        #endregion

        #region Override Methods
        //***************************************************************************
        // Overrides Methods
        // 
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.SetColWidths();
        }
        protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
        {
            base.OnColumnWidthChanged(e);
            if (!this._colResize)
                this.SetColWidths();
        }
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            this.SetColWidths();
        }
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            base.OnDrawColumnHeader(e);
        }
        #endregion
    }
}
