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
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RainstormStudios.Drawing
{
    [Author("Unfried, Michael")]
    public class Polygon : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        PolygonF
            _ply;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public Point Centroid
        { get { return Point.Truncate(this._ply.Centroid); } }
        public float Area
        { get { return this._ply.Area; } }
        public bool CloseShape
        {
            get { return this._ply.CloseShape; }
            set { this._ply.CloseShape = value; }
        }
        public bool IsEmpty
        { get { return this._ply.IsEmpty; } }
        public bool PreCalcBoundingBox
        {
            get { return this._ply.PreCalcBoundBox; }
            set { this._ply.PreCalcBoundBox = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Polygon()
        {
            this._ply = new PolygonF();
        }
        public Polygon(params Point[] points)
        {
            this._ply = new PolygonF(points.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public virtual object Clone()
        { return this.MemberwiseClone(); }
        public void Clear()
        { this._ply.Clear(); }
        public void AddPoint(Point val)
        { this._ply.AddPoint(new PointF((float)val.X, (float)val.Y)); }
        public void AddRange(IEnumerable<Point> val)
        { this._ply.AddRange(val.Select(p => new PointF((float)p.X, (float)p.Y))); }
        public void InsertPoint(int idx, Point val)
        { this._ply.InsertPoint(idx, new PointF((float)val.X, (float)val.Y)); }
        public void InsertRange(int idx, IEnumerable<Point> val)
        { this._ply.InsertRange(idx, val.Select(p => new PointF((float)p.X, (float)p.Y))); }
        public void RemovePointAt(int idx)
        { this._ply.RemovePointAt(idx); }
        public void RemovePointRange(int idx, int count)
        { this._ply.RemovePointRange(idx, count); }
        public Point GetPoint(int idx)
        { return Point.Truncate(this._ply.GetPoint(idx)); }
        public Point[] GetPoints()
        { return this._ply.GetPoints().Select(p => Point.Truncate(p)).ToArray(); }
        public void SetPoint(int idx, Point val)
        { this._ply.SetPoint(idx, new PointF((float)val.X, (float)val.Y)); }
        public bool Contains(Point p)
        { return this._ply.Contains(new PointF((float)p.X, (float)p.Y)); }
        public void Offset(int x, int y)
        { this._ply.Offset((float)x, (float)y); }
        public void Scale(int width, int height)
        { this._ply.Scale((float)width, (float)height); }
        public void RotateAtCentroid(float degrees)
        { this._ply.RotateAtCentroid(degrees); }
        public GraphicsPath GetShape()
        {
            Point[] p = this.GetPoints();
            byte[] bt = new byte[p.Length];
            bt[0] = (byte)PathPointType.Start;
            for (int i = 1; i < p.Length; i++)
                bt[i] = (byte)PathPointType.Line;
            return new GraphicsPath(p, bt);
        }
        public Rectangle GetBounds()
        { return Rectangle.Truncate(this._ply.GetBounds()); }
        //***************************************************************************
        // Static Methods
        // 
        public static Polygon Round(PolygonF ply)
        { return new Polygon(ply.GetPoints().Select(p => Point.Round(p)).ToArray()); }
        public static Polygon Truncate(PolygonF ply)
        { return new Polygon(ply.GetPoints().Select(p => Point.Truncate(p)).ToArray()); }
        public static Polygon Ceiling(PolygonF ply)
        { return new Polygon(ply.GetPoints().Select(p => Point.Ceiling(p)).ToArray()); }
        public static Point GetCentroid(Point[] points)
        { return Point.Truncate(PolygonF.GetCentroid(points.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray())); }
        public static Polygon Rotate(Polygon ply, float degrees, Point center)
        { return Polygon.Truncate(PolygonF.Rotate(ply._ply, degrees, new PointF((float)center.X, (float)center.Y))); }
        public static Polygon Scale(Polygon ply, int width, int height)
        { return Polygon.Truncate(PolygonF.Scale(ply._ply, (float)width, (float)height)); }
        public static Polygon Offset(Polygon ply, int x, int y)
        { return Polygon.Truncate(PolygonF.Offset(ply._ply, (float)x, (float)y)); }
        public static void DrawShape(Polygon ply, Graphics g, Pen p)
        {
            using (GraphicsPath path = ply.GetShape())
                g.DrawPath(p, path);
        }
        public static void FillShape(Polygon ply, Graphics g, Brush b)
        {
            using (GraphicsPath path = ply.GetShape())
                g.FillPath(b, path);
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Polygon val1, Polygon val2)
        { return (val1._ply == val2._ply); }
        public static bool operator !=(Polygon val1, Polygon val2)
        { return !(val1 == val2); }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class PolygonF : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        List<PointF>
            _pnts;
        protected bool
            _isDirty = false;
        protected PointF
            _centroid;
        protected float
            _area;
        protected bool
            _closeShape;
        protected bool
            _preCalcBBox;
        float
            _minX, _maxX,
            _minY, _maxY;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public PointF Centroid
        {
            get
            {
                this.EnsureValues();
                return this._centroid;
            }
        }
        public float Area
        {
            get
            {
                this.EnsureValues();
                return this._area;
            }
        }
        public bool CloseShape
        {
            get { return this._closeShape; }
            set
            {
                if (this._closeShape != value)
                {
                    this._closeShape = value;
                    this._isDirty = true;
                }
            }
        }
        public bool IsEmpty
        { get { return this._pnts.Count == 0; } }
        public bool PreCalcBoundBox
        {
            get { return this._preCalcBBox; }
            set { this._preCalcBBox = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PolygonF()
        {
            this._pnts = new List<PointF>();
            this._closeShape = false;
            this._area = 0.0f;
            this._preCalcBBox = true;
        }
        protected PolygonF(Polygon ply)
            : this()
        {
            PolygonF polyf = PolygonF.FromPolygon(ply);
            this.AddRange(polyf._pnts.ToArray());
        }
        public PolygonF(params PointF[] points)
            : this()
        {
            this.AddRange(points);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        public virtual void Clear()
        {
            this._pnts.Clear();
            this._centroid = PointF.Empty;
            this._area = float.NaN;
            this._isDirty = true;
        }
        public virtual void AddPoint(PointF val)
        {
            this._pnts.Add(val);
            if (this._preCalcBBox)
                this.UpdateMinMax(val);
            this._isDirty = true;
        }
        public virtual void AddRange(IEnumerable<PointF> val)
        {
            this._pnts.AddRange(val);
            if (this._preCalcBBox)
                this.UpdateMinMax(val);
            this._isDirty = true;
        }
        public virtual void InsertPoint(int idx, PointF val)
        {
            this.CheckRange(idx, "idx");
            this._pnts.Insert(idx, val);
            if (this._preCalcBBox)
                this.UpdateMinMax(val);
            this._isDirty = true;
        }
        public virtual void InsertRange(int idx, IEnumerable<PointF> val)
        {
            this.CheckRange(idx, "idx");
            this._pnts.InsertRange(idx, val);
            if (this._preCalcBBox)
                this.UpdateMinMax(val);
            this._isDirty = true;
        }
        public virtual void RemovePointAt(int idx)
        {
            this.CheckRange(idx, "idx");
            this._pnts.RemoveAt(idx);
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            this._isDirty = true;
        }
        public virtual void RemovePointRange(int idx, int count)
        {
            this.CheckRange(idx, "idx");
            this.CheckRange(idx + count, "count");
            this._pnts.RemoveRange(idx, count);
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            this._isDirty = true;
        }
        public PointF GetPoint(int idx)
        {
            this.CheckRange(idx, "idx");
            return this._pnts[idx];
        }
        public PointF[] GetPoints()
        { return this._pnts.ToArray(); }
        public void SetPoint(int idx, PointF val)
        {
            this.CheckRange(idx, "idx");
            this._pnts[idx] = val;
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            this._isDirty = true;
        }
        public virtual bool Contains(PointF p)
        {
            if (this._pnts.Count < 3)
                return false;

            // First, we're going to do the quick min/max bounding box check.  If
            //   it's outside this area, then it *cannot* be within the polygon.
            if (p.X < this._minX || p.X > this._maxX || p.Y < this._minY || p.Y > this._maxY)
                return false;

            // If the point is at least within our bounding box, we're going to have
            //   to do a more granular test.
            PointF p1, p2;
            bool inside = false;
            PointF[] poly = this._pnts.ToArray();

            PointF oldPoint = new PointF(poly[poly.Length - 1].X, poly[poly.Length - 1].Y);
            for (int i = 0; i < poly.Length; i++)
            {
                PointF newPoint = new PointF(poly[i].X, poly[i].Y);
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X) &&
                    ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }
        public virtual void Offset(float x, float y)
        {
            PolygonF offsetPly = PolygonF.Offset(this, x, y);
            this.Clear();
            this._pnts.AddRange(offsetPly._pnts.ToArray());
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            offsetPly.Clear();
        }
        public virtual void Scale(float width, float height)
        {
            PolygonF scalePly = PolygonF.Scale(this, width, height);
            this.Clear();
            this._pnts.AddRange(scalePly._pnts.ToArray());
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            scalePly.Clear();
        }
        public virtual void RotateAtCentroid(float degrees)
        {
            PolygonF rotatePly = PolygonF.Rotate(this, degrees, this.Centroid);
            this.Clear();
            this._pnts.AddRange(rotatePly._pnts.ToArray());
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            rotatePly.Clear();
        }
        public virtual GraphicsPath GetShape()
        {
            PointF[] p = null;
            int arrCount = this._pnts.Count;
            if (arrCount < 1)
                return null;

            if (this._closeShape && (this._pnts[0].X != this._pnts[arrCount - 1].X || this._pnts[0].Y != this._pnts[arrCount - 1].Y))
                p = new PointF[arrCount + 1];
            else
                p = new PointF[arrCount];
            this._pnts.CopyTo(p);
            if (p.Length > this._pnts.Count)
                p[p.Length - 1] = this._pnts[0];

            byte[] bt = new byte[p.Length];
            bt[0] = (byte)PathPointType.Start;
            for (int i = 1; i < p.Length; i++)
                bt[i] = (byte)PathPointType.Line;
            return new GraphicsPath(p, bt);
        }
        public virtual RectangleF GetBounds()
        {
            if (this._preCalcBBox)
                this.UpdateMinMax(this._pnts);
            using (GraphicsPath path = this.GetShape())
            {
                if (path == null)
                    return RectangleF.Empty;
                else
                    return path.GetBounds();
            }
        }
        public virtual float GetAngleAtPoint(int idx)
        {
            this.CheckRange(idx, "idx");

            PointF p1 = idx > 0 ? this._pnts[idx - 1] : this._pnts[this._pnts.Count - 1];
            PointF p2 = this._pnts[idx];
            PointF p3 = idx < this._pnts.Count - 1 ? this._pnts[idx + 1] : this._pnts[0];

            TriangleF tri = new TriangleF(p1, p2, p3);
            return tri.Angle_B;
        }
        //***************************************************************************
        // Static Methods
        // 
        public static PolygonF FromPolygon(Polygon poly)
        {
            return new PolygonF(poly.GetPoints().Select(p => new PointF(p.X, p.Y)).ToArray());
        }
        public static PointF GetCentroid(PointF[] points)
        {
            float areaDummy;
            return PolygonF.GetCentroid(points, out areaDummy);
        }
        public static PolygonF Rotate(PolygonF ply, float degress, PointF center)
        {
            PointF[] p = new PointF[ply._pnts.Count];
            ply._pnts.CopyTo(p);
            using (Matrix mat = new Matrix())
            {
                mat.RotateAt(degress, center);
                mat.TransformPoints(p);
            }
            return new PolygonF(p);
        }
        public static PolygonF Scale(PolygonF ply, float width, float height)
        {
            PointF[] p = new PointF[ply._pnts.Count];
            ply._pnts.CopyTo(p);
            using (Matrix mat = new Matrix())
            {
                mat.Scale(width, height);
                mat.TransformPoints(p);
            }
            return new PolygonF(p);
        }
        public static PolygonF Offset(PolygonF ply, float x, float y)
        {
            PointF[] p = new PointF[ply._pnts.Count];
            ply._pnts.CopyTo(p);
            for (int i = 0; i < p.Length; i++)
            {
                p[i].X += x;
                p[i].Y += y;
            }
            return new PolygonF(p);
        }
        public static void DrawShape(PolygonF ply, Graphics g, Pen p)
        {
            using (GraphicsPath path = ply.GetShape())
                g.DrawPath(p, path);
        }
        public static void FillShape(PolygonF ply, Graphics g, Brush b)
        {
            using (GraphicsPath path = ply.GetShape())
                g.FillPath(b, path);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual void CheckRange(int idx, string arg)
        {
            if (idx < 0 || idx > this._pnts.Count - 1)
                throw new ArgumentOutOfRangeException(arg);
        }
        protected virtual void EnsureValues()
        {
            if (this._isDirty)
            {
                // If points have been altered, we need to recalc a few things.
                if (this._pnts.Count < 1)
                {
                    this._centroid = PointF.Empty;
                    this._area = 0.0f;
                    return;
                }

                // Get the polygon's centroid.
                this._centroid = GetCentroid(this._pnts.ToArray(), out this._area);

                this._isDirty = false;
            }
        }
        protected virtual void UpdateMinMax(IEnumerable<PointF> pnts)
        {
            foreach (var p in pnts)
                this.UpdateMinMax(p);
        }
        protected virtual void UpdateMinMax(PointF p)
        {
            this._minX = System.Math.Min(this._minX, p.X);
            this._maxX = System.Math.Max(this._maxX, p.X);
            this._minY = System.Math.Min(this._minY, p.Y);
            this._maxY = System.Math.Max(this._maxY, p.Y);
        }
        //***************************************************************************
        // Static Methods
        // 
        protected static PointF GetCentroid(PointF[] points, out float area)
        {
            // I hope you like Trig...
            // http://en.wikipedia.org/wiki/Centroid#Centroid_of_polygon
            // 
            // cX = 1/6A * Sum( (x[i] + x[i+1]) * ( (x[i] * y[i+1]) - (x[i+1] * y[i]) ) )
            // cY = 1/6A * Sum( (y[i] + y[i+1]) * ( (x[i] * y[i+1]) - (x[i+1] * y[i]) ) )

            float accumArea = 0.0f;
            float cX = 0.0f;
            float cY = 0.0f;
            float tmp = 0.0f;
            int k;
            int lastIdx = points.Length - 1;

            // This part represents the sum "E" and also sets up the area calc.
            for (int i = 0; i <= lastIdx; i++)
            {
                k = (i + 1) % (lastIdx + 1);  // The '%' makes k 'wrap' around at the end of the array.
                tmp = points[i].X * points[k].Y -
                      points[k].X * points[i].Y;

                accumArea += tmp;

                cX += (points[i].X + points[k].X) * tmp;
                cY += (points[i].Y + points[k].Y) * tmp;
            }

            // Actual area is half of the accumlation
            area = accumArea * 0.5f;

            // c = 1/6A * Sum(n), where A = area
            cX *= 1.0f / (6.0f * area);
            cY *= 1.0f / (6.0f * area);

            // Return the centroid.
            return new PointF(cX, cY);
        }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(PolygonF val1, PolygonF val2)
        {
            // If both values are NULL, then they (technically) are equal.
            if (object.ReferenceEquals(val1, null) && object.ReferenceEquals(val2, null))
                return true;

            // If either value is NULL at this point, then they can't match.
            else if (object.ReferenceEquals(val1, null) || object.ReferenceEquals(val2, null))
                return false;

            // If both values are empty, they match.
            else if (val1.IsEmpty && val2.IsEmpty)
                return true;

            // If either value is empty at this point, they can't match.
            else if (val1.IsEmpty || val2.IsEmpty)
                return false;

            // If they have a different number of points, they don't match.
            else if (val1._pnts.Count != val2._pnts.Count)
                return false;

            // Finally, we have to compare all the points, at least until we find a pair that don't match.
            else
                for (int i = 0; i < val1._pnts.Count; i++)
                    if (val1._pnts[i].X != val2._pnts[i].X || val1._pnts[i].Y != val2._pnts[i].Y)
                        return false;

            // If we made it here, they polygons match.
            return true;
        }
        public static bool operator !=(PolygonF val1, PolygonF val2)
        {
            return !(val1 == val2);
        }
        #endregion
    }
}
