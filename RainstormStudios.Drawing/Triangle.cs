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
using RainstormStudios;
using System.Drawing.Drawing2D;

namespace RainstormStudios.Drawing
{
    public struct Vector 
    {
        public Point
            Origin;
        public float
            Angle;

        public Vector(Point org, float ang)
        {
            this.Origin = org;
            this.Angle = ang;
        }
    }
    public struct Triangle : ICloneable
    {
        #region Fields
        //***************************************************************************
        // Private Fields
        // 
        private TriangleF
            _srcTri;
        //***************************************************************************
        // Public Fields
        // 
        public static readonly Triangle
            Empty;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public Point PointA
        { get { return Point.Round(_srcTri.PointA); } set { _srcTri.PointA = new PointF((float)value.X, (float)value.Y); } }
        public Point PointB
        { get { return Point.Round(_srcTri.PointB); } set { _srcTri.PointB = new PointF((float)value.X, (float)value.Y); } }
        public Point PointC
        { get { return Point.Round(_srcTri.PointC); } set { _srcTri.PointC = new PointF((float)value.X, (float)value.Y); } }
        public bool IsValid
        { get { return !_srcTri.PointA.IsEmpty && !_srcTri.PointB.IsEmpty && !_srcTri.PointC.IsEmpty; } }
        public int Area
        { get { return (int)_srcTri.Area; } }
        public int Perimeter
        { get { return (int)_srcTri.Perimiter; } }
        public int SemiPerimeter
        { get { return Perimeter / 2; } }
        public int RadiusInscribed
        { get { return (int)_srcTri.RadiusInscribed; } }
        public int RadiusCircumscribed
        { get { return (int)_srcTri.RadiusCircumscribed; } }
        public Point AngleBisectorIntersection
        { get { return Point.Round(_srcTri.AngleBisectorIntersection); } }
        public Point MedianIntersection
        { get { return Point.Round(_srcTri.MedianIntersection); } }
        public Point PerpendicularBisectorIntersection
        { get { return Point.Round(_srcTri.PerpendicularBisectorIntersection); } }
        public Point AngleHeightIntersection
        { get { return Point.Round(_srcTri.AngleHeightIntersection); } }
        public Line Line_a
        { get { return Line.Round(_srcTri.Line_a); } }
        public Line Line_b
        { get { return Line.Round(_srcTri.Line_b); } }
        public Line Line_c
        { get { return Line.Round(_srcTri.Line_c); } }
        public float Slope_a
        { get { return _srcTri.Slope_a; } }
        public float Slope_b
        { get { return _srcTri.Slope_b; } }
        public float Slope_c
        { get { return _srcTri.Slope_c; } }
        public int Length_a
        { get { return (int)_srcTri.Length_a; } }
        public int Length_b
        { get { return (int)_srcTri.Length_b; } }
        public int Length_c
        { get { return (int)_srcTri.Length_c; } }
        public Point MidPoint_a
        { get { return Point.Round(_srcTri.MidPoint_a); } }
        public Point MidPoint_b
        { get { return Point.Round(_srcTri.MidPoint_b); } }
        public Point MidPoint_c
        { get { return Point.Round(_srcTri.MidPoint_c); } }
        public int MedianLength_a
        { get { return (int)_srcTri.MedianLength_a; } }
        public int MedianLength_b
        { get { return (int)_srcTri.MedianLength_b; } }
        public int MedianLength_c
        { get { return (int)_srcTri.MedianLength_c; } }
        public int BisectorLength_A
        { get { return (int)_srcTri.BisectorLength_A; } }
        public int BisectorLength_B
        { get { return (int)_srcTri.BisectorLength_B; } }
        public int BisectorLength_C
        { get { return (int)_srcTri.BisectorLength_C; } }
        public Point HeightIntersect_a
        { get { return Point.Round(_srcTri.HeightIntersect_a); } }
        public Point HeightIntersect_b
        { get { return Point.Round(_srcTri.HeightIntersect_b); } }
        public Point HeightIntersect_c
        { get { return Point.Round(_srcTri.HeightIntersect_c); } }
        public Point PerpendicularBisectorIntersect_a
        { get { return Point.Round(_srcTri.PerpendicularBisectorIntersect_a); } }
        public Point PerpendicularBisectorIntersect_b
        { get { return Point.Round(_srcTri.PerpendicularBisectorIntersect_b); } }
        public Point PerpendicularBisectorIntersect_c
        { get { return Point.Round(_srcTri.PerpendicularBisectorIntersect_c); } }
        public Point AngleBisectorIntersect_a
        { get { return Point.Round(_srcTri.AngleBisectorIntersect_a); } }
        public Point AngleBisectorIntersect_b
        { get { return Point.Round(_srcTri.AngleBisectorIntersect_b); } }
        public Point AngleBisectorIntersect_c
        { get { return Point.Round(_srcTri.AngleBisectorIntersect_c); } }
        public int Height_A
        { get { return (int)_srcTri.Height_A; } }
        public int Height_B
        { get { return (int)_srcTri.Height_B; } }
        public int Height_C
        { get { return (int)_srcTri.Height_C; } }
        public float Theta_A
        { get { return _srcTri.Theta_A; } }
        public float Theta_B
        { get { return _srcTri.Theta_B; } }
        public float Theta_C
        { get { return _srcTri.Theta_C; } }
        public int Angle_A
        { get { return (int)_srcTri.Angle_A; } }
        public int Angle_B
        { get { return (int)_srcTri.Angle_B; } }
        public int Angle_C
        { get { return (int)_srcTri.Angle_C; } }
        public int AngleBisectorLength_A
        { get { return (int)_srcTri.AngleBisectorLength_A; } }
        public int AngleBisectorLength_B
        { get { return (int)_srcTri.AngleBisectorLength_B; } }
        public int AngleBisectorLength_C
        { get { return (int)_srcTri.AngleBisectorLength_C; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Triangle(Rectangle rect)
        {
            this._srcTri = new TriangleF(new RectangleF(
                new PointF((float)rect.Location.X, (float)rect.Location.Y),
                new SizeF((float)rect.Size.Width, (float)rect.Size.Height)));
        }
        public Triangle(Point p1, Point p2, Point p3)
        {
            this._srcTri = new TriangleF(
                new PointF((float)p1.X, (float)p1.Y),
                new PointF((float)p2.X, (float)p2.Y),
                new PointF((float)p3.X, (float)p3.Y));
        }
        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
            : this(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3))
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        { return this.MemberwiseClone(); }
        public void Clear()
        { this._srcTri = TriangleF.Empty; }
        public Rectangle GetBounds()
        { return Rectangle.Round(this._srcTri.GetBounds()); }
        public Rectangle GetCircumBounds()
        { return Rectangle.Round(this._srcTri.GetCircumBounds()); }
        public bool Contains(Point p)
        { return this._srcTri.Contains(new PointF((float)p.X, (float)p.Y)); }
        public void Offset(Point val)
        { this.Offset(val.X, val.Y); }
        public void Offset(int x, int y)
        { this._srcTri.Offset((float)x, (float)y); }
        public void Scale(Size val)
        { this.Scale(val.Width, val.Height); }
        public void Scale(int width, int height)
        { this._srcTri.Scale((float)width, (float)height); }
        public void RotateAtMedianCenter(int degrees)
        { this._srcTri.RotateAtMedianCenter((float)degrees); }
        public void RotateAtMassCenter(int degrees)
        { this._srcTri.RotateAtMassCenter((float)degrees); }
        public Point[] GetLines()
        { return new Point[] { PointA, PointB, PointC, PointA }; }
        public GraphicsPath GetShape()
        { return new GraphicsPath(this.GetLines(), new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line }); }
        public void DrawShape(Graphics g, Pen p)
        { TriangleF.DrawShape(this._srcTri, g, p); }
        public void FillShape(Graphics g, Brush b)
        { TriangleF.FillShape(this._srcTri, g, b); }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static Triangle Round(TriangleF value)
        { return new Triangle(Point.Round(value.PointA), Point.Round(value.PointB), Point.Round(value.PointC)); }
        public static Triangle Truncate(TriangleF value)
        { return new Triangle(Point.Truncate(value.PointA), Point.Truncate(value.PointB), Point.Truncate(value.PointC)); }
        public static Triangle Ceiling(TriangleF value)
        { return new Triangle(Point.Ceiling(value.PointA), Point.Ceiling(value.PointB), Point.Ceiling(value.PointC)); }
        #endregion

        #region Old Code
        /*
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly Triangle
            Empty;
        public Point
            PointA, PointB, PointC;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsValid
        { get { return !PointA.IsEmpty && !PointB.IsEmpty && !PointC.IsEmpty; } }
        public double Slope_a
        { get { return (PointC.Y - PointB.Y) / (PointC.X - PointB.X); } }
        public double Slope_b
        { get { return (PointC.Y - PointA.Y) / (PointC.X - PointA.X); } }
        public double Slope_c
        { get { return (PointB.Y - PointA.Y) / (PointB.X - PointA.X); } }
        public double Theta_A
        { get { return Math.Atan((Slope_c - Slope_b) / (1 + (Slope_b + Slope_c))); } }
        public double Theta_B
        { get { return Math.Atan((Slope_c - Slope_a) / (1 + (Slope_a + Slope_c))); } }
        public double Theta_C
        { get { return Math.Atan((Slope_b - Slope_a) / (1 + (Slope_a + Slope_b))); } }
        public double Angle_A
        { get { return Theta_A * (180 / Math.PI); } }
        public double Angle_B
        { get { return Theta_B * (180 / Math.PI); } }
        public double Angle_C
        { get { return Theta_C * (180 / Math.PI); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Triangle(Rectangle rect)
        {
            this.PointA = new Point((rect.Width / 2) + rect.X, rect.Y);
            this.PointB = new Point(rect.Right, rect.Bottom);
            this.PointC = new Point(rect.Left, rect.Bottom);
        }
        public Triangle(Point p1, Point p2, Point p3)
        {
            this.PointA = p1;
            this.PointB = p2;
            this.PointC = p3;
        }
        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
            : this(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3))
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public void Clear()
        {
            PointA = Point.Empty;
            PointB = Point.Empty;
            PointC = Point.Empty;
        }
        public Rectangle GetBounds()
        {
            Point loc = new Point(
                    Math.Min(PointA.X, Math.Min(PointB.X, PointC.X)),
                    Math.Min(PointA.Y, Math.Min(PointB.Y, PointC.Y)));
            Size size = new Size(
                    Math.Max(PointA.X, Math.Max(PointB.X, PointC.X)) - loc.X,
                    Math.Max(PointA.Y, Math.Max(PointB.Y, PointC.Y)) - loc.Y);
            return new Rectangle(loc, size);
        }
        public Rectangle GetCircumBounds()
        {
        }
        public Point[] GetLines()
        {
            return new Point[] { PointA, PointB, PointC, PointA };
        }
        public GraphicsPath GetPath()
        {
            GraphicsPath retVal = new GraphicsPath(FillMode.Winding);
            retVal.AddLines(new Point[] { this.PointA, this.PointB, this.PointC, this.PointA });
            return retVal;
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static bool Intersects(Triangle val1, Triangle val2)
        {
            return false;
        }
        public static Triangle Round(TriangleF val)
        {
            return new Triangle(
                new Point((int)Math.Round(val.PointA.X), (int)Math.Round(val.PointA.Y)),
                new Point((int)Math.Round(val.PointB.X), (int)Math.Round(val.PointB.Y)),
                new Point((int)Math.Round(val.PointC.X), (int)Math.Round(val.PointC.Y)));
        }
        public static Triangle Truncate(TriangleF val)
        {
            return new Triangle(
                new Point((int)Math.Truncate(val.PointA.X), (int)Math.Truncate(val.PointA.Y)),
                new Point((int)Math.Truncate(val.PointB.X), (int)Math.Truncate(val.PointB.Y)),
                new Point((int)Math.Truncate(val.PointC.X), (int)Math.Truncate(val.PointC.Y)));
        }
        public static Triangle Ceiling(TriangleF val)
        {
            return new Triangle(
                new Point((int)Math.Ceiling(val.PointA.X), (int)Math.Ceiling(val.PointA.Y)),
                new Point((int)Math.Ceiling(val.PointB.X), (int)Math.Ceiling(val.PointB.Y)),
                new Point((int)Math.Ceiling(val.PointC.X), (int)Math.Ceiling(val.PointC.Y)));
        }
        public static Triangle Floor(TriangleF val)
        {
            return new Triangle(
                new Point((int)Math.Floor(val.PointA.X), (int)Math.Floor(val.PointA.Y)),
                new Point((int)Math.Floor(val.PointB.X), (int)Math.Floor(val.PointB.Y)),
                new Point((int)Math.Floor(val.PointC.X), (int)Math.Floor(val.PointC.Y)));
        }
        #endregion
         */
        #endregion
    }
    public struct TriangleF : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly TriangleF
            Empty;
        public PointF
            PointA, PointB, PointC;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsValid
        { get { return !PointA.IsEmpty && !PointB.IsEmpty && !PointC.IsEmpty; } }
        public float Area
        { get { return (float)System.Math.Sqrt(Semiperimiter * (Semiperimiter - Length_a) * (Semiperimiter - Length_b) * (Semiperimiter - Length_c)); } }
        public float Perimiter
        { get { return LineF.FindDistance(this.PointA, this.PointB) + LineF.FindDistance(this.PointB, this.PointC) + LineF.FindDistance(this.PointC, this.PointA); } }
        public float Semiperimiter
        { get { return Perimiter / 2; } }
        public float RadiusInscribed
        { get { return (2 * Area) / Perimiter; } }
        public double RadiusCircumscribed
        { get { return (Length_a * Length_b * Length_c) / (4 * Area); } }
        public PointF AngleBisectorIntersection
        { get { return LineF.FindIntersect(new LineF(this.PointA, AngleBisectorIntersect_a), new LineF(this.PointB, AngleBisectorIntersect_b)); } }
        public PointF MedianIntersection
        { get { return LineF.FindIntersect(new LineF(this.PointA, MidPoint_a), new LineF(this.PointB, MidPoint_b)); } }
        public PointF PerpendicularBisectorIntersection
        { get { return LineF.FindIntersect(new LineF(this.MidPoint_a, this.PerpendicularBisectorIntersect_a), new LineF(this.MidPoint_b, this.PerpendicularBisectorIntersect_b)); } }
        public PointF AngleHeightIntersection
        { get { return LineF.FindIntersect(new LineF(this.PointA, this.AngleBisectorIntersect_a), new LineF(this.PointC, this.AngleBisectorIntersect_c)); } }
        public LineF Line_a
        { get { return new LineF(this.PointB, this.PointC); } }
        public LineF Line_b
        { get { return new LineF(this.PointA, this.PointC); } }
        public LineF Line_c
        { get { return new LineF(this.PointA, this.PointB); } }
        public float Slope_a
        { get { return (PointB.Y - PointC.Y) / (PointB.X - PointC.X); } }
        public float Slope_b
        { get { return (PointC.Y - PointA.Y) / (PointC.X - PointA.X); } }
        public float Slope_c
        { get { return (PointA.Y - PointB.Y) / (PointA.X - PointB.X); } }
        public float Length_a
        { get { return new LineF(this.PointB, this.PointC).Length; } }
        public float Length_b
        { get { return new LineF(this.PointA, this.PointC).Length; } }
        public float Length_c
        { get { return new LineF(this.PointA, this.PointB).Length; } }
        public PointF MidPoint_a
        { get { return LineF.FindMidPoint(new LineF(this.PointB, this.PointC)); } }
        public PointF MidPoint_b
        { get { return LineF.FindMidPoint(new LineF(this.PointA, this.PointC)); } }
        public PointF MidPoint_c
        { get { return LineF.FindMidPoint(new LineF(this.PointA, this.PointB)); } }
        public float MedianLength_a
        { get { return (float)System.Math.Sqrt(((2 * System.Math.Pow(Length_b, 2)) + (2 * System.Math.Pow(Length_c, 2)) - System.Math.Pow(Length_a, 2)) / 2); } }
        public float MedianLength_b
        { get { return (float)System.Math.Sqrt(((2 * System.Math.Pow(Length_b, 2)) + (2 * System.Math.Pow(Length_c, 2)) - System.Math.Pow(Length_b, 2)) / 2); } }
        public float MedianLength_c
        { get { return (float)System.Math.Sqrt(((2 * System.Math.Pow(Length_a, 2)) + (2 * System.Math.Pow(Length_b, 2)) - System.Math.Pow(Length_c, 2)) / 2); } }
        public float BisectorLength_A
        { get { return (float)System.Math.Sqrt((Length_b * Length_c) * ((1 - System.Math.Pow(Length_a, 2)) / System.Math.Pow(Length_b + Length_c, 2))); } }
        public float BisectorLength_B
        { get { return (float)System.Math.Sqrt((Length_a * Length_c) * ((1 - System.Math.Pow(Length_b, 2)) / System.Math.Pow(Length_a + Length_c, 2))); } }
        public float BisectorLength_C
        { get { return (float)System.Math.Sqrt((Length_a * Length_b) * ((1 - System.Math.Pow(Length_c, 2)) / System.Math.Pow(Length_a + Length_b, 2))); } }
        public PointF HeightIntersect_a
        { get { return LineF.FindIntersect(new LineF(this.PointB, this.PointC), LineF.FindPerpendicular(this.PointA, new LineF(this.PointB, this.PointC))); } }
        public PointF HeightIntersect_b
        { get { return LineF.FindIntersect(new LineF(this.PointA, this.PointC), LineF.FindPerpendicular(this.PointB, new LineF(this.PointA, this.PointC))); } }
        public PointF HeightIntersect_c
        { get { return LineF.FindIntersect(new LineF(this.PointA, this.PointB), LineF.FindPerpendicular(this.PointC, new LineF(this.PointA, this.PointB))); } }
        public PointF PerpendicularBisectorIntersect_a
        {
            get
            {
                PointF ib = LineF.FindIntersect(Line_b, LineF.FindPerpendicular(MidPoint_a, Line_a));
                PointF ic = LineF.FindIntersect(Line_c, LineF.FindPerpendicular(MidPoint_a, Line_a));
                return (LineF.FindDistance(MidPoint_a, ib) < LineF.FindDistance(MidPoint_a, ic)) ? ib : ic;
            }
        }
        public PointF PerpendicularBisectorIntersect_b
        {
            get
            {
                PointF ia = LineF.FindIntersect(Line_a, LineF.FindPerpendicular(MidPoint_b, Line_b));
                PointF ic = LineF.FindIntersect(Line_c, LineF.FindPerpendicular(MidPoint_b, Line_b));
                return (LineF.FindDistance(MidPoint_b, ia) < LineF.FindDistance(MidPoint_b, ic)) ? ia : ic;
            }
        }
        public PointF PerpendicularBisectorIntersect_c
        {
            get
            {
                PointF ia = LineF.FindIntersect(Line_a, LineF.FindPerpendicular(MidPoint_c, Line_c));
                PointF ib = LineF.FindIntersect(Line_b, LineF.FindPerpendicular(MidPoint_c, Line_c));
                return (LineF.FindDistance(MidPoint_c, ia) < LineF.FindDistance(MidPoint_c, ib)) ? ia : ib;
            }
        }
        public PointF AngleBisectorIntersect_a
        {
            get
            {
                using (System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix())
                {
                    PointF[] p = new PointF[] { new PointF(this.PointA.X, this.PointA.Y), new PointF(this.PointB.X, this.PointB.Y) };
                    mat.RotateAt(-(float)this.Angle_A / 2, this.PointA);
                    mat.TransformPoints(p);
                    return LineF.FindIntersect(new LineF(p[0], p[1]), this.Line_a);
                }
            }
        }
        public PointF AngleBisectorIntersect_b
        {
            get
            {
                using (System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix())
                {
                    PointF[] p = new PointF[] { new PointF(this.PointB.X, this.PointB.Y), new PointF(this.PointC.X, this.PointC.Y) };
                    mat.RotateAt(-(float)Angle_B / 2, this.PointB);
                    mat.TransformPoints(p);
                    return LineF.FindIntersect(new LineF(p[0], p[1]), this.Line_b);
                }
            }
        }
        public PointF AngleBisectorIntersect_c
        {
            get
            {
                using (System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix())
                {
                    PointF[] p = new PointF[] { new PointF(this.PointC.X, this.PointC.Y), new PointF(this.PointA.X, this.PointA.Y) };
                    mat.RotateAt(-(float)Angle_C / 2, this.PointC);
                    mat.TransformPoints(p);
                    return LineF.FindIntersect(new LineF(p[0], p[1]), this.Line_c);
                }
            }
        }
        public float Height_A
        { get { return (2 * Area) / Length_a; } }
        public float Height_B
        { get { return (2 * Area) / Length_b; } }
        public float Height_C
        { get { return (2 * Area) / Length_c; } }
        public float Theta_A
        { get { return (float)System.Math.Atan(RadiusInscribed / (Semiperimiter - Length_a)) * 2; } }
        public float Theta_B
        { get { return (float)System.Math.Atan(RadiusInscribed / (Semiperimiter - Length_b)) * 2; } }
        public float Theta_C
        { get { return (float)System.Math.Atan(RadiusInscribed / (Semiperimiter - Length_c)) * 2; } }
        public float Angle_A
        { get { return (float)(Theta_A * (180 / System.Math.PI)); } }
        public float Angle_B
        { get { return (float)(Theta_B * (180 / System.Math.PI)); } }
        public float Angle_C
        { get { return (float)(Theta_C * (180 / System.Math.PI)); } }
        public float AngleBisectorLength_A
        { get { return (float)System.Math.Sqrt((Length_b * Length_c) * ((1 - System.Math.Pow(Length_a, 2)) / System.Math.Pow(Length_b + Length_c, 2))); } }
        public float AngleBisectorLength_B
        { get { return (float)System.Math.Sqrt((Length_a * Length_c) * ((1 - System.Math.Pow(Length_b, 2)) / System.Math.Pow(Length_a + Length_c, 2))); } }
        public float AngleBisectorLength_C
        { get { return (float)System.Math.Sqrt((Length_a * Length_b) * ((1 - System.Math.Pow(Length_c, 2)) / System.Math.Pow(Length_a + Length_b, 2))); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private TriangleF(Triangle tri)
        {
            this.PointA = new PointF((float)tri.PointA.X, (float)tri.PointA.Y);
            this.PointB = new PointF((float)tri.PointB.X, (float)tri.PointB.Y);
            this.PointC = new PointF((float)tri.PointC.X, (float)tri.PointC.Y);
        }
        public TriangleF(RectangleF rect)
        {
            this.PointA = new PointF((rect.Width / 2) + rect.X, rect.Y);
            this.PointB = new PointF(rect.Right, rect.Bottom);
            this.PointC = new PointF(rect.Left, rect.Bottom);
        }
        public TriangleF(PointF p1, PointF p2, PointF p3)
        {
            this.PointA = p1;
            this.PointB = p2;
            this.PointC = p3;
        }
        public TriangleF(float x1, float y1, float x2, float y2, float x3, float y3)
            : this(new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3))
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public void Clear()
        {
            PointA = PointF.Empty;
            PointB = PointF.Empty;
            PointC = PointF.Empty;
        }
        public RectangleF GetBounds()
        {
            PointF loc = new PointF(
                    System.Math.Min(PointA.X, System.Math.Min(PointB.X, PointC.X)),
                    System.Math.Min(PointA.Y, System.Math.Min(PointB.Y, PointC.Y)));
            SizeF size = new SizeF(
                    System.Math.Max(PointA.X, System.Math.Max(PointB.X, PointC.X)) - loc.X,
                    System.Math.Max(PointA.Y, System.Math.Max(PointB.Y, PointC.Y)) - loc.Y);
            return new RectangleF(loc, size);
        }
        /// <summary>Gets the rectangular bounds of this triangle object's circumscribed circle.</summary>
        /// <returns>An object of type System.Drawing.RectangleF.</returns>
        public RectangleF GetCircumBounds()
        {
            PointF loc = new PointF(
                this.PerpendicularBisectorIntersection.X - ((float)this.RadiusCircumscribed + 1),
                this.PerpendicularBisectorIntersection.Y - ((float)this.RadiusCircumscribed + 1));
            SizeF size = new SizeF(((float)this.RadiusCircumscribed + 1) * 2, ((float)this.RadiusCircumscribed + 1) * 2);
            return new RectangleF(loc, size);
        }
        /// <summary>Determines whether the specified point exists within this triangle's sides.</summary>
        /// <param name="p">An object of type System.Drawing.Point to test.</param>
        /// <returns>A bool value indicating true if the specified point is within the triangle.  Otherwise, false.</returns>
        public bool Contains(PointF p)
        {
            LineF ac = new LineF(this.PointA, this.PointC);
            LineF ab = new LineF(this.PointA, this.PointB);
            LineF cb = new LineF(this.PointB, this.PointC);

            float da = LineF.FindDistance(this.PointA, p);
            float db = LineF.FindDistance(this.PointB, p);
            float dc = LineF.FindDistance(this.PointC, p);

            float dai = LineF.FindDistance(this.PointA, LineF.FindIntersect(cb, new LineF(this.PointA, p)));
            float dbi = LineF.FindDistance(this.PointB, LineF.FindIntersect(ac, new LineF(this.PointB, p)));
            float dci = LineF.FindDistance(this.PointC, LineF.FindIntersect(ab, new LineF(this.PointC, p)));

            return (da < dai) && (db < dbi) && (dc < dci);
        }
        /// <summary>Translates the triangle's points by the specified x/y values.</summary>
        /// <param name="val">An object of type System.Drawing.Point whose x/y values contain the number of units to translate along the x/y axis.</param>
        public void Offset(PointF val)
        { this.Offset(val.X, val.Y); }
        /// <summary>Translates the triangle's points by the specified x/y values.</summary>
        /// <param name="x">The number of units to translate on the x-axis.</param>
        /// <param name="y">The number of units to translate on the y-axis.</param>
        public void Offset(float x, float y)
        {
            this.PointA.X += x;
            this.PointA.Y += y;
            this.PointB.X += x;
            this.PointB.Y += y;
            this.PointC.X += x;
            this.PointC.Y += y;
        }
        /// <summary>Expands the triangle by the specified length and width, keepings its center position constant.</summary>
        /// <param name="val">A size parameter which specifies the length and width amounts to increase this triangle's size.</param>
        public void Scale(SizeF val)
        {
            this.Scale(val.Width, val.Height);
        }
        /// <summary>Expands the triangle by the specified length and width, keepings its center position constant.</summary>
        /// <param name="width">The number of units to add to the width.</param>
        /// <param name="height">The number of units to add to the height.</param>
        public void Scale(float width, float height)
        {
            using (System.Drawing.Drawing2D.Matrix trans = new System.Drawing.Drawing2D.Matrix())
            {
                trans.Scale(width, height);
                trans.TransformPoints(new PointF[] { this.PointA, this.PointB, this.PointC });
            }
        }
        /// <summary>Rotates the triangle around the point of intersection of its medians.</summary>
        /// <param name="degrees">The number of degrees to rotate.  Positive values result in a clockwise rotation.</param>
        public void RotateAtMedianCenter(float degrees)
        { this = TriangleF.Rotate(this, degrees, MedianIntersection); }
        /// <summary>Rotates the triangle around the center of its angle bisectors.</summary>
        /// <param name="degrees">The number of degrees to rotate.  Positive values result in a clockwise rotation.</param>
        public void RotateAtMassCenter(float degrees)
        { this = TriangleF.Rotate(this, degrees, AngleBisectorIntersection); }
        /// <summary>Rotates the triangle around the point of intersection of its perpendicular bisectors.</summary>
        /// <param name="degrees">The number of degrees to rotate.  Positive values result in a clockwise rotation.</param>
        public void RotateAtCircumCenter(float degrees)
        { this = TriangleF.Rotate(this, degrees, this.PerpendicularBisectorIntersection); }
        /// <summary>Creates an array of points representing this triangle object.</summary>
        /// <returns>An object of type PointF Array.</returns>
        public PointF[] GetLines()
        { return new PointF[] { PointA, PointB, PointC, PointA }; }
        /// <summary>Converts the triangle into a GraphicsPath object.</summary>
        /// <returns>An object of type GraphicsPath.</returns>
        public GraphicsPath GetShape()
        { return new GraphicsPath(this.GetLines(), new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line }); }
        public void DrawShape(Graphics g, Pen p)
        {
            TriangleF.DrawShape(this, g, p);
        }
        public void FillShape(Graphics g, Brush b)
        {
            TriangleF.FillShape(this, g, b);
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static TriangleF Rotate(TriangleF tri, float degrees, PointF center)
        {
            PointF[] p = new PointF[] { new PointF(tri.PointA.X, tri.PointA.Y), new PointF(tri.PointB.X, tri.PointB.Y), new PointF(tri.PointC.X, tri.PointC.Y) };
            using (System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix())
            {
                mat.RotateAt(degrees, center);
                mat.TransformPoints(p);
            }
            return new TriangleF(p[0], p[1], p[2]);
        }
        public static void DrawShape(TriangleF tri, Graphics g, Pen p)
        {
            using (GraphicsPath path = tri.GetShape())
                g.DrawPath(p, path);
        }
        public static void FillShape(TriangleF tri, Graphics g, Brush b)
        {
            using (GraphicsPath path = tri.GetShape())
                g.FillPath(b, path);
        }
        #endregion
    }
}
