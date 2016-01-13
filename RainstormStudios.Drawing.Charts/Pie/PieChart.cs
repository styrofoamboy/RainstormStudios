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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Drawing.Charts.Pie
{
    [Author("Unfried, Michael")]
    public class PieChart:ChartBase<PieChartSlice>
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        //private string
        //    _text;
        //private System.Windows.Forms.TextImageRelation
        //    _textRelation;
        //private System.Drawing.ContentAlignment
        //    _textAlign;
        //private Point
        //    _textPos;
        private int
            _tiltAngle;
        private bool
            _divLines;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool SliceDivideLines
        { get { return this._divLines; } set { this._divLines = value; } }
        public int AngleOfTilt
        { get { return this._tiltAngle; } set { this._tiltAngle = value; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PieChart()
            :base()
        {
            //this._text = "";
            //this._textAlign = ContentAlignment.TopLeft;
            //this._textPos = Point.Empty;
            this._maxPercent = 100;
            this._use3d = false;
            this._bdrColor = Color.Black;
            this._bdrWidth = 1;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Draw(Graphics g)
        { this.Draw(g, Rectangle.Round(g.ClipBounds)); }
        public override void Draw(Graphics g, Rectangle bounds)
        {
            // Save the graphics state before we start changing settings.
            GraphicsState gState = g.Save();

            // If we're set to use multisampling, we have to make sure the
            //   graphics device is set to draw that way.
            if (this._useMsmp)
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.SmoothingMode = (this._useMsmpHq) ? SmoothingMode.HighQuality : SmoothingMode.AntiAlias;
                g.InterpolationMode = (this._useMsmpHq) ? InterpolationMode.HighQualityBicubic : InterpolationMode.Bilinear;
                g.CompositingQuality = (this._useMsmpHq) ? CompositingQuality.HighQuality : CompositingQuality.HighSpeed;
            }
            else
                g.CompositingMode = CompositingMode.SourceCopy;

            // If the caller doesn't explicitly specify the bounds, we will sometimes get the
            //   Min/Max values for a signed int (the graphics object's "Clipping Region" is,
            //   essentially, unbounded), which means Top/Left will be -2147483647, which will 
            //   throw an exception when call "DrawSlice".
            int bTop = System.Math.Max(bounds.Top, 0),
                bLeft = System.Math.Max(bounds.Left, 0);

            // Detemine the center of the pie chart.
            int xCen = (bounds.Width / 2) + System.Math.Max(bLeft, 0),
                yCen = (bounds.Height / 2) + System.Math.Max(bTop, 0);

            Rectangle drawArea = new Rectangle(
                (bLeft + 2) + (this._bdrWidth / 2),
                (bTop + 2) + (this._bdrWidth / 2),
                bounds.Width - (4 + this._bdrWidth),
                bounds.Height - (4 + this._bdrWidth));

            for (int i = 0; i < this._elements.Count; i++)
                this.Elements[i].DrawElement(g, drawArea);

            using (Pen p = new Pen(this._bdrColor))
            {
                p.Width = (float)this._bdrWidth;
                g.DrawEllipse(p, drawArea);

                //if (this._divLines)
                //    foreach (PieChartSlice slice in this.Slices)
                //        g.DrawLine(p, new Point((bounds.Width / 2) + bounds.Left, (bounds.Height / 2) + bounds.Top),
                //            Point.Truncate(CircleF.PointAtAngle(
                //            new CircleF((bounds.Width / 2) + bounds.Left, (bounds.Height / 2) + bounds.Top,
                //            Math.Min(bounds.Width, bounds.Height)), (slice.Size * 360) / this._maxPrcnt)));
            }

            // Don't forget to restore the graphics device.
            g.Restore(gState);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override ICollection<PieChartSlice> CreateElementsCollection()
        {
            return new PieChartSliceCollection(this);
        }
        #endregion
    }
}
