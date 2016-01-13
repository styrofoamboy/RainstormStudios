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
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RainstormStudios.Drawing
{
    [Author("Unfried, Michael")]
    public struct FeatheredRectangle : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private int
            _feather;
        private Rectangle
            _rect;
        private GraphicsPath
            _gpShape;
        private GraphicsPath
            _gpBorder;
        private int
            _border;
        //***************************************************************************
        // Public Fields
        // 
        public static readonly FeatheredRectangle
            Empty;
        #endregion

        #region Pubic Properties
        //***************************************************************************
        // Public Properties
        // 
        public int CornerFeather
        {
            get { return this._feather; }
            set { this._feather = value; }
        }
        public Point Location
        {
            get { return this._rect.Location; }
            set { this._rect.Location = value; }
        }
        public Size Size
        {
            get { return this._rect.Size; }
            set { this._rect.Size = value; }
        }
        public int Width
        {
            get { return this._rect.Width; }
            set { this._rect.Width = value; }
        }
        public int Height
        {
            get { return this._rect.Height; }
            set { this._rect.Height = value; }
        }
        public int X
        { get { return this._rect.X; } }
        public int Y
        { get { return this._rect.Y; } }
        public int Left
        { get { return this._rect.X; } }
        public int Right
        { get { return this._rect.Right; } }
        public int Top
        { get { return this._rect.Top; } }
        public int Bottom
        { get { return this._rect.Bottom; } }
        public bool IsEmpty
        { get { return this.RectEmpty(); } }
        public int BorderOffset
        {
            get { return this._border; }
            set { this._border = value; }
        }
        #endregion

        #region Class Constrcutors
        //***************************************************************************
        // Class Constructors
        // 
        public FeatheredRectangle(int x, int y, int width, int height)
            : this(new Point(x, y), new Size(width, height), 0)
        { }
        public FeatheredRectangle(int x, int y, int width, int height, int feather)
            : this(new Point(x, y), new Size(width, height), feather)
        { }
        public FeatheredRectangle(Point location, Size size)
            : this(location, size, 0)
        { }
        public FeatheredRectangle(Point location, Size size, int feather)
        {
            this._rect = new Rectangle(location, size);
            this._feather = feather;
            this._gpShape = new GraphicsPath(FillMode.Winding);
            this._gpBorder = new GraphicsPath(FillMode.Winding);
            this._border = 0;
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static FeatheredRectangle Ceiling(FeatheredRectangleF value)
        {
            return new FeatheredRectangle(Point.Ceiling(value.Location), Size.Ceiling(value.Size), (int)System.Math.Ceiling(value.CornerFeather));
        }
        public static FeatheredRectangle Truncate(FeatheredRectangleF value)
        {
            return new FeatheredRectangle(Point.Truncate(value.Location), Size.Truncate(value.Size), (int)System.Math.Truncate(value.CornerFeather));
        }
        public static FeatheredRectangle Round(FeatheredRectangleF value)
        {
            return new FeatheredRectangle(Point.Round(value.Location), Size.Round(value.Size), (int)System.Math.Round(value.CornerFeather));
        }
        public static FeatheredRectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return FeatheredRectangle.FromLTRB(left, top, right, bottom, 0);
        }
        public static FeatheredRectangle FromLTRB(int left, int top, int right, int bottom, int feather)
        {
            return new FeatheredRectangle(left, top, right - left, bottom - top, feather);
        }
        public static FeatheredRectangle Union(FeatheredRectangle a, FeatheredRectangle b)
        {
            Rectangle rslt = Rectangle.Union(a._rect, b._rect);
            FeatheredRectangle retVal = new FeatheredRectangle(rslt.Location, rslt.Size, System.Math.Max(a._feather, b._feather));
            retVal.BorderOffset = System.Math.Max(a._border, b._border);
            return retVal;
        }
        public static FeatheredRectangle Inflate(FeatheredRectangle rect, int x, int y)
        {
            return new FeatheredRectangle(rect.Location, new Size(rect.Width + x, rect.Height + y), rect.CornerFeather);
        }
        public static FeatheredRectangle Intersect(FeatheredRectangle a, FeatheredRectangle b)
        {
            Rectangle rslt = Rectangle.Intersect(a._rect, b._rect);
            if (!rslt.IsEmpty)
            {
                FeatheredRectangle retVal = new FeatheredRectangle(rslt.Location, rslt.Size, System.Math.Max(a._feather, b._feather));
                retVal.BorderOffset = System.Math.Max(a._border, b._border);
                return retVal;
            }
            else
                return FeatheredRectangle.Empty;
        }
        #endregion

        #region Public methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public bool Contains(Point value)
        {
            return _gpShape.IsVisible(value);
        }
        public bool Contains(Rectangle value)
        {
            if (_gpShape.IsVisible(value.Location) &&
                _gpShape.IsVisible(value.Left, value.Bottom) &&
                _gpShape.IsVisible(value.Right, value.Top) &&
                _gpShape.IsVisible(value.Right, value.Bottom))
                return true;
            else
                return false;
        }
        public bool Contains(int x, int y)
        {
            return _gpShape.IsVisible(x, y);
        }
        public Rectangle GetBounds()
        {
            return this._rect;
        }
        public override bool Equals(object obj)
        {
            if (this._rect.Equals(obj) && this._border == ((FeatheredRectangle)obj)._border && this._feather == ((FeatheredRectangle)obj)._feather)
                return true;
            else
                return false;
        }
        public void Offset(Point pos)
        {
            this.Offset(pos.X, pos.Y);
        }
        public void Offset(int x, int y)
        {
            this._rect.Offset(x, y);
        }
        public void Inflate(Size size)
        {
            this.Inflate(size.Width, size.Height);
        }
        public void Inflate(int x, int y)
        {
            this._rect.Inflate(x, y);
        }
        public System.Drawing.Drawing2D.GraphicsPath GetRegion()
        {
            CalcGp();
            return this._gpShape;
        }
        public System.Drawing.Drawing2D.GraphicsPath GetBorder()
        {
            CalcGp();
            return this._gpBorder;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private bool RectEmpty()
        {
            if (this._rect.IsEmpty && this._feather <= 0)
                return true;
            else
                return false;
        }
        private void CalcGp()
        {
            int iFeatherSq = this._feather * 2;
            int iBorderHalf = this._border / 2;
            if (iFeatherSq > 0)
            {
                Rectangle rUL = new Rectangle(iBorderHalf + this._rect.Left, iBorderHalf + this._rect.Top, iFeatherSq, iFeatherSq);
                Rectangle rBL = new Rectangle(iBorderHalf + this._rect.Left, ((this._rect.Height - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Top, iFeatherSq, iFeatherSq);
                Rectangle rUR = new Rectangle(((this._rect.Width - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Left, iBorderHalf + this._rect.Top, iFeatherSq, iFeatherSq);
                Rectangle rBR = new Rectangle(((this._rect.Width - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Left, ((this._rect.Height - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Top, iFeatherSq, iFeatherSq);

                this._gpShape.Reset();
                this._gpShape.AddPie(rUL, 180, 90);
                this._gpShape.AddPie(rBL, 90, 90);
                this._gpShape.AddPie(rUR, 270, 90);
                this._gpShape.AddPie(rBR, 0, 90);
                this._gpShape.AddRectangle(new Rectangle(this._rect.Left + this._feather + iBorderHalf, iBorderHalf + this._rect.Top, this._rect.Width - ((this._feather + iBorderHalf) * 2) - 1, this._feather));
                this._gpShape.AddRectangle(new Rectangle(this._rect.Left + this._feather + iBorderHalf, (this._rect.Height - (this._feather + iBorderHalf) - 1) + this._rect.Top, this._rect.Width - ((this._feather + iBorderHalf) * 2) - 1, this._feather));
                if ((this._rect.Height - ((this._feather + iBorderHalf) * 2) - 1) > 0)
                    this._gpShape.AddRectangle(new Rectangle(iBorderHalf + this._rect.Left, this._feather + iBorderHalf + this._rect.Top, this._rect.Width - iBorderHalf, this._rect.Height - ((this._feather + iBorderHalf) * 2) - 1));
                this._gpShape.CloseFigure();
                this._gpShape.Flatten();

                this._gpBorder.Reset();
                this._gpBorder.AddArc(rUL, 180, 90);
                this._gpBorder.AddArc(rUR, 270, 90);
                this._gpBorder.AddArc(rBR, 0, 90);
                this._gpBorder.AddArc(rBL, 90, 90);
                this._gpBorder.CloseFigure();

                rUL = Rectangle.Empty;
                rBL = Rectangle.Empty;
                rUR = Rectangle.Empty;
                rBR = Rectangle.Empty;
            }
            else
            {
                throw new Exception("Cannot render a FeatheredRectangle object with a feather of '0'.");
            }

        }
        #endregion

        #region Operator Overrides
        //***************************************************************************
        // Operator Overrides
        // 
        public static bool operator ==(FeatheredRectangle a, FeatheredRectangle b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(FeatheredRectangle a, FeatheredRectangle b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region Base-Class Overrides
        //***************************************************************************
        // Base-Class Overrides
        // 
        public override int GetHashCode()
        {
            return Convert.ToString(this._rect.Top + this._rect.Left + this._rect.Width + this._rect.Height + this._feather + this._border).GetHashCode();
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public struct FeatheredRectangleF : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private float
            _feather;
        private RectangleF
            _rect;
        private GraphicsPath
            _gpShape,
            _gpBorder;
        private float
            _border;
        //***************************************************************************
        // Public Fields
        // 
        public static readonly FeatheredRectangleF
            Empty;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public float CornerFeather
        {
            get { return this._feather; }
            set { this._feather = value; }
        }
        public PointF Location
        {
            get { return this._rect.Location; }
            set { this._rect.Location = value; }
        }
        public SizeF Size
        {
            get { return this._rect.Size; }
            set { this._rect.Size = value; }
        }
        public float Width
        {
            get { return this._rect.Width; }
            set { this._rect.Width = value; }
        }
        public float Height
        {
            get { return this._rect.Height; }
            set { this._rect.Height = value; }
        }
        public float X
        { get { return this._rect.X; } }
        public float Y
        { get { return this._rect.Y; } }
        public float Left
        { get { return this._rect.X; } }
        public float Right
        { get { return this._rect.Right; } }
        public float Top
        { get { return this._rect.Top; } }
        public float Bottom
        { get { return this._rect.Bottom; } }
        public bool IsEmpty
        { get { return this.RectEmpty(); } }
        public float BorderOffset
        {
            get { return this._border; }
            set { this._border = value; }
        }
        #endregion

        #region Class Constrcutors
        //***************************************************************************
        // Class Constructors
        // 
        public FeatheredRectangleF(float x, float y, float width, float height)
            : this(new PointF(x, y), new SizeF(width, height), 0)
        { }
        public FeatheredRectangleF(float x, float y, float width, float height, float feather)
            : this(new PointF(x, y), new SizeF(width, height), feather)
        { }
        public FeatheredRectangleF(PointF location, SizeF size)
            : this(location, size, 0)
        { }
        public FeatheredRectangleF(PointF location, SizeF size, float feather)
        {
            this._rect = new RectangleF(location, size);
            this._feather = feather;
            this._gpShape = new GraphicsPath(FillMode.Winding);
            this._gpBorder = new GraphicsPath(FillMode.Winding);
            this._border = 0.0f;
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static FeatheredRectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return FeatheredRectangleF.FromLTRB(left, top, right, bottom, 0);
        }
        public static FeatheredRectangleF FromLTRB(float left, float top, float right, float bottom, float feather)
        {
            return new FeatheredRectangleF(left, top, right - left, bottom - top, feather);
        }
        public static FeatheredRectangleF Union(FeatheredRectangleF a, FeatheredRectangleF b)
        {
            RectangleF rslt = RectangleF.Union(a._rect, b._rect);
            FeatheredRectangleF retVal = new FeatheredRectangleF(rslt.Location, rslt.Size, System.Math.Max(a._feather, b._feather));
            retVal.BorderOffset = System.Math.Max(a._border, b._border);
            return retVal;
        }
        public static FeatheredRectangleF Inflate(FeatheredRectangleF rect, float x, float y)
        {
            return new FeatheredRectangleF(rect.Location, new SizeF(rect.Width + x, rect.Height + y), rect.CornerFeather);
        }
        public static FeatheredRectangleF Intersect(FeatheredRectangleF a, FeatheredRectangleF b)
        {
            RectangleF rslt = RectangleF.Intersect(a._rect, b._rect);
            if (!rslt.IsEmpty)
            {
                FeatheredRectangleF retVal = new FeatheredRectangleF(rslt.Location, rslt.Size, System.Math.Max(a._feather, b._feather));
                retVal.BorderOffset = System.Math.Max(a._border, b._border);
                return retVal;
            }
            else
                return FeatheredRectangleF.Empty;
        }
        #endregion

        #region Public methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public bool Contains(PointF value)
        {
            return _gpShape.IsVisible(value);
        }
        public bool Contains(RectangleF value)
        {
            if (_gpShape.IsVisible(value.Location) &&
                _gpShape.IsVisible(value.Left, value.Bottom) &&
                _gpShape.IsVisible(value.Right, value.Top) &&
                _gpShape.IsVisible(value.Right, value.Bottom))
                return true;
            else
                return false;
        }
        public bool Contains(float x, float y)
        {
            return _gpShape.IsVisible(x, y);
        }
        public RectangleF GetBounds()
        {
            return this._rect;
        }
        public override bool Equals(object obj)
        {
            if (this._rect.Equals(obj) && this._border == ((FeatheredRectangleF)obj)._border && this._feather == ((FeatheredRectangleF)obj)._feather)
                return true;
            else
                return false;
        }
        public void Offset(PointF pos)
        {
            this.Offset(pos.X, pos.Y);
        }
        public void Offset(float x, float y)
        {
            this._rect.Offset(x, y);
        }
        public void Inflate(SizeF size)
        {
            this.Inflate(size.Width, size.Height);
        }
        public void Inflate(float x, float y)
        {
            this._rect.Inflate(x, y);
        }
        public System.Drawing.Drawing2D.GraphicsPath GetRegion()
        {
            CalcGp();
            return this._gpShape;
        }
        public System.Drawing.Drawing2D.GraphicsPath GetBorder()
        {
            CalcGp();
            return this._gpBorder;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private bool RectEmpty()
        {
            if (this._rect.IsEmpty && this._feather <= 0)
                return true;
            else
                return false;
        }
        private void CalcGp()
        {
            float iFeatherSq = this._feather * 2;
            float iBorderHalf = this._border / 2;

            RectangleF rUL = new RectangleF(iBorderHalf + this._rect.Left, iBorderHalf + this._rect.Top, iFeatherSq, iFeatherSq);
            RectangleF rBL = new RectangleF(iBorderHalf + this._rect.Left, ((this._rect.Height - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Top, iFeatherSq, iFeatherSq);
            RectangleF rUR = new RectangleF(((this._rect.Width - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Left, iBorderHalf + this._rect.Top, iFeatherSq, iFeatherSq);
            RectangleF rBR = new RectangleF(((this._rect.Width - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Left, ((this._rect.Height - 1) - ((iFeatherSq) + iBorderHalf)) + this._rect.Top, iFeatherSq, iFeatherSq);

            this._gpShape.Reset();
            this._gpShape.AddPie(rUL.X, rUL.Y, rUL.Width, rUL.Height, 180, 90);
            this._gpShape.AddPie(rBL.X, rBL.Y, rBL.Width, rBL.Height, 90, 90);
            this._gpShape.AddPie(rUR.X, rUR.Y, rUR.Width, rUR.Height, 270, 90);
            this._gpShape.AddPie(rBR.X, rBR.Y, rBR.Width, rBR.Height, 0, 90);
            this._gpShape.AddRectangle(new RectangleF(this._rect.Left + this._feather + iBorderHalf, iBorderHalf + this._rect.Top, this._rect.Width - ((this._feather + iBorderHalf) * 2) - 1, this._feather));
            this._gpShape.AddRectangle(new RectangleF(this._rect.Left + this._feather + iBorderHalf, (this._rect.Height - (this._feather + iBorderHalf) - 1) + this._rect.Top, this._rect.Width - ((this._feather + iBorderHalf) * 2) - 1, this._feather));
            if ((this._rect.Height - ((this._feather + iBorderHalf) * 2) - 1) > 0)
                this._gpShape.AddRectangle(new RectangleF(iBorderHalf + this._rect.Left, this._feather + iBorderHalf + this._rect.Top, this._rect.Width - iBorderHalf, this._rect.Height - ((this._feather + iBorderHalf) * 2) - 1));
            this._gpShape.CloseFigure();
            this._gpShape.Flatten();

            this._gpBorder.Reset();
            this._gpBorder.AddArc(rUL, 180, 90);
            this._gpBorder.AddArc(rUR, 270, 90);
            this._gpBorder.AddArc(rBR, 0, 90);
            this._gpBorder.AddArc(rBL, 90, 90);
            this._gpBorder.CloseFigure();

            rUL = RectangleF.Empty;
            rBL = RectangleF.Empty;
            rUR = RectangleF.Empty;
            rBR = RectangleF.Empty;
        }
        #endregion

        #region Operator Overrides
        //***************************************************************************
        // Operator Overrides
        // 
        public static bool operator ==(FeatheredRectangleF a, FeatheredRectangleF b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(FeatheredRectangleF a, FeatheredRectangleF b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region Base-Class Overrides
        //***************************************************************************
        // Base-Class Overrides
        // 
        public override int GetHashCode()
        {
            return Convert.ToString(this._rect.Top + this._rect.Left + this._rect.Width + this._rect.Height + this._feather + this._border).GetHashCode();
        }
        #endregion
    }
}
