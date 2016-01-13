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
    public struct PieChartSlice : IChartElement<PieChart>
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        internal PieChart
            _owner;
        internal int
            _position;
        public int
            _value;
        //***************************************************************************
        // Public Fields
        // 
        public string
            Text;
        public Color
            Color;
        public Color
            LabelColor;
        public bool
            ShowPercent;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public PieChart Owner
        { get { return this._owner; } }
        public int Position
        { get { return this._position; } }
        public int Value
        {
            get { return this._value; }
            set { this._value = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        internal PieChartSlice(PieChart owner)
            : this(0, Color.Empty, "", false)
        {
            this._owner = owner;
        }
        public PieChartSlice(int size)
            : this(size, Color.Empty)
        { }
        public PieChartSlice(int size, Color color)
            : this(size, color, "")
        { }
        public PieChartSlice(int size, Color color, string text)
            : this(size, color, text, false)
        { }
        public PieChartSlice(int size, Color color, string text, bool showPercent)
        {
            this._value = size;
            this.Color = color;
            this.Text = text;
            this.LabelColor = Color.Black;
            this.ShowPercent = showPercent;
            this._owner = null;
            this._position = -1;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void DrawElement(Graphics g)
        { this.DrawElement(g, Rectangle.Round(g.ClipBounds)); }
        public void DrawElement(Graphics g, Rectangle bounds)
        {
            // Check to make sure this slice has a
            //   PieChart owner before attempting to draw.
            if (this.Owner == null)
                throw new Exception("PieChartSlice cannot 'self-draw' without attached owner.");

            using (Bitmap bmpDraw = new Bitmap(bounds.Width, bounds.Height, g))
            using (Graphics gBmp = Graphics.FromImage(bmpDraw))
            {
                // Get the center of the drawing area.
                int xCen = (bounds.Width / 2) + bounds.Left,
                    yCen = (bounds.Height / 2) + bounds.Top;

                // Get the value for the start of this slice by adding
                //   the value of all the previous slices together.
                int offset = 0;
                for (int i = 0; i < this._position; i++)
                    offset += this._owner.Elements[i].Value;

                int startAngle = ((offset * 360) / this._owner.MaxValue) - 90;
                int endAngle = (((offset + this.Value) * 360) / this._owner.MaxValue) - 90;

                //gBmp.Clear(Color.Transparent);
                GraphicsUnit gUnit = g.PageUnit;
                using (SolidBrush b = new SolidBrush((this.Color == Color.Empty) ? Color.Gray : this.Color))
                    gBmp.FillPie(b, Rectangle.Round(bmpDraw.GetBounds(ref gUnit)), startAngle, endAngle - startAngle);
                g.DrawImageUnscaled(bmpDraw, bounds);

                // Draw slice percent.
                if (this.ShowPercent)
                    using (SolidBrush txtBrush = new SolidBrush(this.LabelColor))
                    {
                        //Font pcntFont = new Font(FontFamily.GenericSansSerif, 8.0);
                        gBmp.DrawString(Convert.ToString((this.Value * 100) / this._owner.MaxValue) + "%", SystemFonts.SmallCaptionFont, txtBrush, (bounds.Width / 2) + bounds.Left, (bounds.Height / 2) + bounds.Top);
                    }
            }


            //// Get the points around the outside edge of the pie chart
            ////   for drawing the curved edge of the slice.
            //PointFCollection points = new PointFCollection();
            //points.Add(new PointF((float)xCen, (float)yCen));
            //for (int t = offset; t < (offset + this.Size); t++)
            //{
            //    double a = Math.PI * t / this._owner.TotalValue;
            //    double x = xCen + (bounds.Width / 2) * Math.Sin(a);
            //    double y = yCen + (bounds.Height / 2) * Math.Cos(a);
            //    points.Add(new PointF((float)x, (float)y), t.ToString());
            //}
            //points.Add(new PointF((float)xCen, (float)yCen));
            //byte[] pVals = new byte[points.Count];
            //pVals[0] = (byte)PathPointType.Line;
            //pVals[1] = (byte)PathPointType.Line;
            //for (int i = 2; i < pVals.Length - 2; i++)
            //    pVals[i] = (byte)PathPointType.Bezier;
            //pVals[pVals.Length - 1] = (byte)PathPointType.Line;
            //pVals[pVals.Length - 2] = (byte)PathPointType.Line;

            //using (GraphicsPath slice = new GraphicsPath(points.ToArray(), pVals, FillMode.Winding))
            //using (SolidBrush b = new SolidBrush(this.Color))
            //    g.FillPath(b, slice);
        }
        #endregion
    }
}
