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
using System.Drawing;
using System.Linq;
using System.Text;

namespace RainstormStudios.Drawing.Charts
{
    public abstract class ChartBase<TChartElement> : IChart<TChartElement>
        where TChartElement : IChartElement, new()
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected ICollection<TChartElement>
            _elements;
        protected bool
            _use3d,
            _useMsmp,
            _useMsmpHq;
        protected int
            _maxPercent,
            _bdrWidth;
        protected Color
            _bdrColor;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int MaxValue
        {
            get { return this._maxPercent; }
            set { this._maxPercent = value; }
        }
        public bool UseMultisampling
        {
            get { return this._useMsmp; }
            set { this._useMsmp = value; }
        }
        public bool HighQualityMultisampling
        {
            get { return this._useMsmpHq; }
            set { this._useMsmpHq = value; }
        }
        public bool Use3DEffect
        {
            get { return this._use3d; }
            set { this._use3d = value; }
        }
        public int BorderWidth
        {
            get { return this._bdrWidth; }
            set { this._bdrWidth = value; }
        }
        public Color BorderColor
        {
            get { return this._bdrColor; }
            set { this._bdrColor = value; }
        }
        public TChartElement[] Elements
        { get { return this._elements.ToArray(); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected ChartBase()
        {
            this._elements = this.CreateElementsCollection();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public Bitmap GetImage(Size size)
        { return this.GetImage(size, Color.Empty); }
        public Bitmap GetImage(Size size, Color bgColor)
        { return this.GetImage(size.Width, size.Height, bgColor); }
        public Bitmap GetImage(int width, int height)
        { return this.GetImage(width, height, Color.Empty); }
        public Bitmap GetImage(int width, int height, Color bgColor)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics gBmp = Graphics.FromImage(bmp))
            {
                if (bgColor != Color.Empty)
                    gBmp.Clear(bgColor);
                this.Draw(gBmp, new Rectangle(0, 0, width, height));
            }
            return bmp;
        }
        public void AddElement(TChartElement element)
        { this._elements.Add(element); }
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract void Draw(Graphics g, Rectangle bounds);
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected abstract ICollection<TChartElement> CreateElementsCollection();
        #endregion
    }
}
