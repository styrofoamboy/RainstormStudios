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
using RainstormStudios;

namespace RainstormStudios.Drawing
{
    public struct Circle : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        public static readonly Circle
            Empty;
        public Point
            Center;
        public int
            Radius;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsEmpty
        { get { return this.Equals(CircleF.Empty) || this.Center == Point.Empty || this.Radius == 0; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Circle(Point center, int radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        public Circle(int xCenter, int yCenter, int radius)
        {
            this.Center = new Point(xCenter, yCenter);
            this.Radius = radius;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return MemberwiseClone();
        }
        public void Clear()
        {
            this.Center = Point.Empty;
            this.Radius = 0;
        }
        public Point GetPointAtAngle(float degree)
        { return Point.Truncate(CircleF.PointAtAngle(CircleF.FromCircle(this), degree)); }
        public int AngleOfPoint(int x, int y)
        { return this.AngleOfPoint(new Point(x, y)); }
        public int AngleOfPoint(Point point)
        { return (int)System.Math.Truncate(CircleF.AngleOfPoint(CircleF.FromCircle(this), new PointF((float)point.X, (float)point.Y))); }
        public int AreaOfIntersect(Circle cir)
        { return (int)System.Math.Truncate(CircleF.AreaOfIntersect(CircleF.FromCircle(this), CircleF.FromCircle(cir))); }
        public GraphicsPath GetShape()
        { return this.GetShape(30, PathPointType.Line); }
        public GraphicsPath GetShape(int pointCount, PathPointType pointType)
        {
            List<Point> pnts = new List<Point>(pointCount);
            List<byte> pntTypes = new List<byte>(pointCount);
            for (int i = 0; i < pointCount; i++)
            {
                double a = (double)(i + 1) / ((double)pointCount / 2) * System.Math.PI;
                int x = (int)System.Math.Truncate(this.Center.X + this.Radius * System.Math.Sin(a));
                int y = (int)System.Math.Truncate(this.Center.Y + this.Radius * System.Math.Cos(a));
                pnts.Add(new Point(x, y));
                pntTypes.Add((byte)pointType);
            }
            pnts.Add(new Point(pnts[0].X, pnts[0].Y));
            pntTypes.Add((byte)pointType);
            return new GraphicsPath(pnts.ToArray(), pntTypes.ToArray());
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static Circle Truncate(CircleF val)
        { return new Circle((int)System.Math.Truncate(val.Center.X), (int)System.Math.Truncate(val.Center.Y), (int)System.Math.Truncate(val.Radius)); }
        public static Circle Round(CircleF val)
        { return new Circle((int)System.Math.Round(val.Center.X), (int)System.Math.Round(val.Center.Y), (int)System.Math.Round(val.Radius)); }
        public static Circle Floor(CircleF val)
        { return new Circle((int)System.Math.Floor(val.Center.X), (int)System.Math.Floor(val.Center.Y), (int)System.Math.Floor(val.Radius)); }
        public static Circle Ceiling(CircleF val)
        { return new Circle((int)System.Math.Ceiling(val.Center.X), (int)System.Math.Ceiling(val.Center.Y), (int)System.Math.Ceiling(val.Radius)); }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public static bool operator ==(Circle val1, Circle val2)
        {
            // If both values are empty, they match.
            if (val1.IsEmpty && val2.IsEmpty)
                return true;

            // If either value is empty at this point, they can't match.
            else if (val1.IsEmpty || val2.IsEmpty)
                return false;

            // Otherwise, compare the center/radius values to see if they match.
            else
                return (val1.Center.X == val2.Center.X && val1.Center.Y == val2.Center.Y && val1.Radius == val2.Radius);
        }
        public static bool operator !=(Circle val1, Circle val2)
        {
            return !(val1 == val2);
        }
        #endregion
    }
    public struct CircleF : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        public static readonly CircleF
            Empty;
        public PointF
            Center;
        public float
            Radius;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsEmpty
        { get { return this.Equals(CircleF.Empty) || this.Center == PointF.Empty || float.IsNaN(this.Radius); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CircleF(PointF center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        public CircleF(float xCenter, float yCenter, float radius)
        {
            this.Center = new PointF(xCenter, yCenter);
            this.Radius = radius;
        }
        public CircleF(PointF center, PointF point)
        {
            this.Center = center;
            this.Radius = LineF.FindDistance(center, point);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object Clone()
        {
            return MemberwiseClone();
        }
        public void Clear()
        {
            this.Center = PointF.Empty;
            this.Radius = float.NaN;
        }
        public float AngleOfPoint(float x, float y)
        { return this.AngleOfPoint(new PointF(x, y)); }
        public float AngleOfPoint(PointF point)
        { return CircleF.AngleOfPoint(this, point); }
        public float AreaOfIntersect(CircleF circle)
        { return CircleF.AreaOfIntersect(this, circle); }
        public GraphicsPath GetShape()
        { return this.GetShape(30, PathPointType.Line); }
        public GraphicsPath GetShape(int pointCount, PathPointType pointType)
        {
            List<PointF> pnts = new List<PointF>(pointCount);
            List<byte> pntTypes = new List<byte>(pointCount);
            for (int i = 0; i < pointCount; i++)
            {
                double a = (double)(i + 1) / ((double)pointCount / 2) * System.Math.PI;
                double x = this.Center.X + this.Radius * System.Math.Sin(a);
                double y = this.Center.Y + this.Radius * System.Math.Cos(a);
                pnts.Add(new PointF((float)x, (float)y));
                pntTypes.Add((byte)pointType);
            }
            pnts.Add(new PointF(pnts[0].X, pnts[0].Y));
            pntTypes.Add((byte)pointType);
            return new GraphicsPath(pnts.ToArray(), pntTypes.ToArray());
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static PointF PointAtAngle(CircleF cir, float degree)
        {
            if (degree > 360 || degree < 0)
                throw new ArgumentOutOfRangeException("degree", degree, "Valid values for 'degree' are 0 through 360 in floating point increments");

            // For the Law of Cosines to work, we have to convert degrees
            //   into radians.
            double t = degree / 180 * System.Math.PI;
            PointF retVal = new PointF(
                (float)(cir.Center.X + cir.Radius + System.Math.Sin(t)),
                (float)(cir.Center.Y + cir.Radius + System.Math.Cos(t)));

            return retVal;

            //for (float t = 0; t < degree; t+=0.1)
            //{
            //    double a = t * 180 / Math.PI;
            //    retVal.X = (float)(cir.Center.X + cir.Radius + Math.Sin(a));
            //    retVal.Y = (flaot)(cir.Center.Y + cir.Radius + Math.Cos(a));
            //}
        }
        public static float AngleOfPoint(CircleF cir, float x, float y)
        {
            return AngleOfPoint(cir, new PointF(x, y));
        }
        public static float AngleOfPoint(CircleF cir, PointF point)
        {
            return LineF.FindAngle(cir.Center, new PointF(cir.Center.X + cir.Radius, cir.Center.Y), cir.Center, point);
        }
        public static float AreaOfIntersect(CircleF val1, CircleF val2)
        {
            // First, we find the distance between the two circle's centers.
            double c = LineF.FindDistance(val1.Center, val2.Center);
            // Then, we use the Law of Cosines to find the angle of the points
            //   at which the two cirlces touch from the circles' centers.
            //  cos(theta) = (r2^2 + c^2 - r1^2) / (2 * r1 * c)
            double theta = System.Math.Sin((System.Math.Pow(val2.Radius, 2) + System.Math.Pow(c, 2) - System.Math.Pow(val1.Radius, 2)) / (2 * val2.Radius * c));

            // Now we find the distance from the midpoint of the chord (the line
            //   segment drawn between the two intersection points) to the center
            //   of either circle.  This value will always be the same no matter
            //   which circle you calculate to.
            double d = val1.Radius * System.Math.Cos(theta / 2);

            // Now, the each 'segment' can be seen as two triangles back-to-back
            //   with an arched cap.  This arched cap is the area of intersect.
            //  (r1^2 * arccos(d / r1)) - (d * sqrt(r1^2 - d^2))
            double area = (System.Math.Pow(val1.Radius, 2) * System.Math.Acos(d / val1.Radius)) - (d * System.Math.Sqrt(System.Math.Pow(val1.Radius, 2) - System.Math.Pow(d, 2)));

            // Since the area of intersect is two of these arched caps back-to-back,
            //   the total area of intersect is twice "area".
            return (float)(area * 2);
        }
        public static void DrawShape(CircleF cir, Graphics g, Pen p)
        {
            using (GraphicsPath path = cir.GetShape(30, PathPointType.Line))
                g.DrawPath(p, path);
        }
        public static void FillShape(CircleF cir, Graphics g, Brush b)
        {
            using (GraphicsPath path = cir.GetShape(60, PathPointType.Bezier))
                g.FillPath(b, path);
        }
        //***************************************************************************
        // Internal Static Methods
        // 
        internal static CircleF FromCircle(Circle cir)
        { return new CircleF((float)cir.Center.X, (float)cir.Center.Y, (float)cir.Radius); }
        #endregion

        #region Operator Overloads
        //***************************************************************************
        // Operator Overloads
        // 
        public static bool operator ==(CircleF val1, CircleF val2)
        {
            // If both values are empty, they match.
            if (val1.IsEmpty && val2.IsEmpty)
                return true;

            // If either value is empty at this point, they can't match.
            else if (val1.IsEmpty || val2.IsEmpty)
                return false;

            // Otherwise, compare the center/radius values to see if they match.
            else
                return (val1.Center.X == val2.Center.X && val1.Center.Y == val2.Center.Y && val1.Radius == val2.Radius);
        }
        public static bool operator !=(CircleF val1, CircleF val2)
        {
            return !(val1 == val2);
        }
        #endregion
    }
}
