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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using RainstormStudios.Drawing;

namespace RainstormStudios.Controls
{
    [Author("Unfried, Michael")]
    public sealed class DropDownButton : RainstormStudios.Controls.AdvancedButton
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ContextMenuStrip
            _mnu;
        Color
            _triClr = Color.Black;
        GraphicsPath
            _triGp;
        ToolStrip
            _fakeMnu;
        ToolStripItemCollection
            _items;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [Category("Design"), Description("Specifies the color of the drop down indicator triangle drawn on the control."), Browsable(true)]
        public Color TriangleColor
        {
            get { return this._triClr; }
            set { this._triClr = value; }
        }
        [Category("Design"), Description("The collection of items to be listed in the control's drop down menu."), Browsable(true)]
        public ToolStripItemCollection Items
        {
            get { return this._items; }
        }
        [Category("Design"), Description("Specifies the background color of the control's drop down menu."), Browsable(true)]
        public Color MenuColor
        {
            get { return this._mnu.BackColor; }
            set { this._mnu.BackColor = value; }
        }
        [Category("Design"), Description("Specifies the height of the control's drop down menu."), Browsable(true)]
        public int MenuHeight
        {
            get { return this._mnu.Height; }
            set { this._mnu.Height = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DropDownButton()
            : base()
        {
            this._fakeMnu = new ToolStrip();
            ToolStripMenuItem initItem = new ToolStripMenuItem("Item 01");
            this._items = new ToolStripItemCollection(this._fakeMnu, new ToolStripItem[] { initItem });
            this._mnu = new ContextMenuStrip();
            this._mnu.ImageList = this.ImageList;
            this._triGp = GetTriangle();
            this.ForegroundReservedSpace = this._triGp.GetBounds().Width;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            if (this._mnu != null)
                this._mnu.Dispose();

            // Make sure you let the base class dispose, too.
            base.Dispose();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private GraphicsPath GetTriangle()
        {
            GraphicsPath rg = new GraphicsPath(FillMode.Winding);
            int r = this.Width - this.CornerFeather,
                h = ((this.Height - (this.CornerFeather * 2)) / 2) + this.CornerFeather;
            rg.AddLines(new Point[]{
                    new Point(r - 14, h - 5), 
                    new Point(r, h - 5),
                    new Point(r - 7, h + 5),
                    new Point(r - 14, h - 5) });
            return rg;
        }
        #endregion

        #region Event Handlers
        //***************************************************************************
        // Event Overrides
        // 
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            using (SolidBrush brush = new SolidBrush(this._triClr))
                pevent.Graphics.FillPath(brush, this._triGp);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this._mnu.Items.Clear();
            for (int i = 0; i < this._items.Count; i++)
                this._mnu.Items.Add(this._items[i]);
            this._mnu.Show(this, new Point(0, this.Height));
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this._triGp = this.GetTriangle();
        }
        #endregion
    }
}