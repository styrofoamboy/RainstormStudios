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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using RainstormStudios;

namespace RainstormStudios.Drawing
{
    /// <summary>
    /// A simple struct for storing a single, two-point line and also provides a number of static methods for performing geometric equations on lines.
    /// </summary>
    [Author("Unfried, Michael")]
    public struct Line : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly Line
            Empty;
        public Point
            PointA, PointB;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool Valid
        { get { return (!PointA.IsEmpty && !PointB.IsEmpty); } }
        public float Slope
        { get { return FindSlope(this); } }
        public int Length
        { get { return FindDistance(this); } }
        public Point MidPoint
        { get { return Line.FindMidPoint(this); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public Line(Point a, Point b)
        {
            this.PointA = a;
            this.PointB = b;
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
            PointA = Point.Empty;
            PointB = Point.Empty;
        }
        public void Offset(Point value)
        {
            Offset(value.X, value.Y);
        }
        public void Offset(int x, int y)
        {
            PointA.X += x;
            PointA.Y += y;
            PointB.X += x;
            PointB.Y += y;
        }
        public void Rotate(float degree)
        {
            this = Line.Rotate(this, degree);
        }
        public Rectangle GetBounds()
        {
            int left = System.Math.Min(PointA.X, PointB.X);
            int right = System.Math.Max(PointA.X, PointB.X);
            int top = System.Math.Min(PointA.Y, PointB.Y);
            int bottom = System.Math.Max(PointA.Y, PointB.Y);

            return Rectangle.FromLTRB(left, top, right, bottom);
        }
        public void Transform(Matrix mat)
        {
            this = Line.Transform(this, mat);
        }
        public double GetTheta(Line val)
        {
            return Line.FindAngle(this, val);
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static bool IsEmpty(Line value)
        {
            try
            {
                Point x = value.PointA;
                Point y = value.PointB;
                return false;
            }
            catch
            {
                return true;
            }
        }
        public static float FindSlope(Line val)
        {
            return FindSlope(val.PointA, val.PointB);
        }
        public static float FindSlope(Point val1, Point val2)
        {
            return LineF.FindSlope(new PointF((float)val1.X, (float)val1.Y), new PointF((float)val2.X, (float)val2.Y));
        }
        public static int FindAngle(Line line1, Line line2)
        {
            return (int)FindAngle(line1.PointA, line1.PointB, line2.PointA, line2.PointB);
        }
        public static int FindAngle(Point line1a, Point line1b, Point line2a, Point line2b)
        {
            return (int)FindAngle(
                FindSlope(line1a, line1b),
                FindSlope(line2a, line2b));
        }
        /// <summary>Returns the acute angle of where two lines intersect.  The obtuse angle can be determined by "180 - result".</summary>
        /// <param name="slope1">The slope of line1.</param>
        /// <param name="slope2">The slope of line2.</param>
        /// <returns></returns>
        public static double FindAngle(float slope1, float slope2)
        {
            return FindAngleInRadians(slope1, slope2) * (180 / System.Math.PI);
        }
        /// <summary>Returns the acute angle of where two lines intersect.  1 radian = (180/PI).</summary>
        /// <param name="slope1">The slope of line1.</param>
        /// <param name="slope2">The slope of line2.</param>
        /// <returns></returns>
        public static double FindAngleInRadians(float slope1, float slope2)
        {
            return System.Math.Atan((slope2 - slope1) / (1 + (slope1 * slope2)));
        }
        public static int FindDistance(Line val)
        {
            return FindDistance(val.PointA, val.PointB);
        }
        public static int FindDistance(Point p1, Point p2)
        {
            return (int)LineF.FindDistance(new LineF(
                new PointF((float)p1.X, (float)p1.Y),
                new PointF((float)p2.X, (float)p2.Y)));
        }
        public static Line Rotate(Line val, float degree)
        {
            LineF r = LineF.Rotate(new LineF(
                new PointF(val.PointA.X, val.PointA.Y),
                new PointF(val.PointB.X, val.PointB.Y)), degree);
            return new Line(Point.Round(r.PointA), Point.Round(r.PointB));
        }
        public static Line Transform(Line val, Matrix mat)
        {
            LineF r = LineF.Transform(new LineF(
                new PointF(val.PointA.X, val.PointA.Y),
                new PointF(val.PointB.X, val.PointB.Y)), mat);
            return new Line(Point.Round(r.PointA), Point.Round(r.PointB));
        }
        public static Line Round(LineF value)
        {
            return new Line(
                new Point(
                    (int)System.Math.Round(value.PointA.X),
                    (int)System.Math.Round(value.PointA.Y)),
                new Point(
                    (int)System.Math.Round(value.PointB.X),
                    (int)System.Math.Round(value.PointB.Y)));
        }
        public static Line Truncate(LineF value)
        {
            return new Line(
                new Point(
                    (int)System.Math.Truncate(value.PointA.X),
                    (int)System.Math.Truncate(value.PointA.Y)),
                new Point(
                    (int)System.Math.Truncate(value.PointB.X),
                    (int)System.Math.Truncate(value.PointB.Y)));
        }
        public static Line Ceiling(LineF value)
        {
            return new Line(
                new Point(
                    (int)System.Math.Ceiling(value.PointA.X),
                    (int)System.Math.Ceiling(value.PointA.Y)),
                new Point(
                    (int)System.Math.Ceiling(value.PointB.X),
                    (int)System.Math.Ceiling(value.PointB.Y)));
        }
        public static Line Floor(LineF value)
        {
            return new Line(
                new Point(
                    (int)System.Math.Floor(value.PointA.X),
                    (int)System.Math.Floor(value.PointA.Y)),
                new Point(
                    (int)System.Math.Floor(value.PointB.X),
                    (int)System.Math.Floor(value.PointB.Y)));
        }
        public static Point FindMidPoint(Line value)
        {
            return Point.Round(LineF.FindMidPoint(new LineF(
                                                new PointF((float)value.PointA.X, (float)value.PointA.Y),
                                                new PointF((float)value.PointB.X, (float)value.PointB.Y))));
        }
        #endregion
    }
    /// <summary>
    /// A simple struct for storing a single, two-point line with floating decimal coordinate values and provides a number of static methods for perfoming geometric equations on lines.
    /// </summary>
    [Author("Unfried, Michael")]
    public struct LineF : ICloneable
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly LineF
            Empty;
        public PointF
            PointA, PointB;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool Valid
        { get { return (!PointA.IsEmpty && !PointB.IsEmpty); } }
        public float Slope
        { get { return FindSlope(this); } }
        public float Length
        { get { return FindDistance(this); } }
        public PointF MidPoint
        { get { return LineF.FindMidPoint(this); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public LineF(PointF a, PointF b)
        {
            this.PointA = a;
            this.PointB = b;
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
            this.PointA = Point.Empty;
            this.PointB = Point.Empty;
        }
        public void Offset(PointF value)
        {
            Offset(value.X, value.Y);
        }
        public void Offset(float x, float y)
        {
            this.PointA.X += x;
            this.PointA.Y += y;
            this.PointB.X += x;
            this.PointB.Y += y;
        }
        public void Rotate(float degree)
        {
            LineF r = Rotate(this, degree);
            this.PointA = r.PointA;
            this.PointB = r.PointB;
        }
        public RectangleF GetBounds()
        {
            float left = System.Math.Min(PointA.X, PointB.X);
            float right = System.Math.Max(PointA.X, PointB.X);
            float top = System.Math.Min(PointA.Y, PointB.Y);
            float bottom = System.Math.Max(PointA.Y, PointB.Y);

            return RectangleF.FromLTRB(left, top, right, bottom);
        }
        public void Transform(Matrix mat)
        {
            this = Transform(this, mat);
        }
        public float GetTheta(LineF val)
        {
            return (float)LineF.FindAngle(this, val);
        }
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>Creates a line segment from the given point and slope, using the given x offset as the location of the other end of the segment.</summary>
        /// <param name="p">The coordinate of the start of the segment.</param>
        /// <param name="m">The slope of the line.</param>
        /// <param name="x">The x-coordinate of the end of the segment.  The y-coordinate will be calculated using y=mx.</param>
        /// <returns>A <see cref="T:AllOneSystem.Drawing.LineF"/> value of the new line segment.</returns>
        public static LineF FromSlope(PointF p, float m, float x)
        {
            //LineF nl = new LineF(new PointF(p.X, 0), new PointF(p.X + 10, m * (p.X + 10)));
            float yi = LineF.FindYIntercept(p, m).Y;
            return new LineF(p, new PointF(p.X + x, (m * (p.X + 10)) + yi));
        }
        /// <summary>Constructs a line segment from the linear equation "Ax + By + C = 0".</summary>
        /// <param name="alpha"></param>
        /// <param name="x"></param>
        /// <param name="omega"></param>
        /// <param name="y"></param>
        /// <param name="yintercept"></param>
        /// <returns></returns>
        public static LineF FromLinearEquation(float alpha, float x, float omega, float y, float yintercept)
        {
            return new LineF(new PointF(x, y), new PointF(x + 10, ((alpha / omega) * (x + 10)) + yintercept));
        }
        public static LineF FindPerpendicular(PointF p, LineF l)
        {
            //return LineF.FromSlope(p, -l.Slope, 50);
            float yi = LineF.FindYIntercept(p, (-(l.PointB.X - l.PointA.X)) / (l.PointB.Y - l.PointA.Y)).Y;
            return new LineF(p, new PointF(p.X + 20, ((p.X + 20) * ((-(l.PointB.X - l.PointA.X)) / (l.PointB.Y - l.PointA.Y))) + yi));
        }
        public static float FindSlope(LineF val)
        {
            return FindSlope(val.PointA, val.PointB);
        }
        public static float FindSlope(PointF val1, PointF val2)
        {
            return (val2.Y - val1.Y) / (val2.X - val1.X);
        }
        public static float FindAngle(LineF line1, LineF line2)
        {
            return FindAngle(line1.PointA, line1.PointB, line2.PointA, line2.PointB);
        }
        public static float FindAngle(PointF line1a, PointF line1b, PointF line2a, PointF line2b)
        {
            return (float)Line.FindAngle(
                FindSlope(line1a, line1b),
                FindSlope(line2a, line2b));
        }
        public static float FindDistance(LineF val)
        {
            return FindDistance(val.PointA, val.PointB);
        }
        public static float FindDistance(PointF p1, PointF p2)
        {
            return (float)System.Math.Sqrt(System.Math.Pow((p2.X - p1.X), 2) + System.Math.Pow((p2.Y - p1.Y), 2));
        }
        public static LineF Rotate(LineF val, float degree)
        {
            PointF[] p = new PointF[] { new PointF(val.PointA.X, val.PointA.Y), new PointF(val.PointB.X, val.PointB.Y) };
            using (Matrix trans = new Matrix())
            {
                RectangleF b = val.GetBounds();
                trans.RotateAt(degree, new PointF(b.Right - (b.Width / 2), b.Bottom - (b.Height / 2)));
                trans.TransformPoints(p);
            }
            return new LineF(p[0], p[1]);
        }
        public static LineF Transform(LineF val, Matrix mat)
        {
            PointF[] p = new PointF[] { val.PointA, val.PointB };
            mat.TransformPoints(p);
            return new LineF(p[0], p[1]);
        }
        public static PointF FindMidPoint(PointF a, PointF b)
        {
            return FindMidPoint(new LineF(a, b));
        }
        public static PointF FindMidPoint(LineF value)
        {
            //double x = ((50 * value.PointA.X) + (100 * value.PointB.X)) / 150;
            //double y = ((50 * value.PointA.Y) + (100 * value.PointB.Y)) / 150;
            double x, y;
            if(double.IsInfinity(value.Slope))
            {
                x = value.PointA.X;
                y = System.Math.Min(value.PointA.Y, value.PointB.Y) + (System.Math.Abs(value.PointB.Y - value.PointA.Y) / 2);
            }
            else if (double.IsNaN(value.Slope))
            {
                x = System.Math.Min(value.PointA.X, value.PointB.X) + (System.Math.Abs(value.PointB.X - value.PointA.X) / 2);
                y = value.PointA.Y;
            }
            else
            {
                x = (System.Math.Abs(value.PointB.X - value.PointA.X) / 2);
                y = (value.Slope * x) + ((value.PointA.X > value.PointB.X) ? value.PointB.Y : value.PointA.Y);
            }
            return new PointF((float)x + (float)System.Math.Min(value.PointA.X, value.PointB.X), (float)y);
        }
        /// <summary>Determines the coordinate of intersection of two lines.  If the given line segments do not actually intersect, this will return the point where they would intersect on an infinite plane.</summary>
        /// <param name="val1">An initialized <see cref="T:RainstormStudios.Drawing.LineF"/> value representing the first line.</param>
        /// <param name="val2">An initialized <see cref="T:RainstormStudios.Drawing.LineF"/> value representing the second line.</param>
        /// <returns></returns>
        public static PointF FindIntersect(LineF val1, LineF val2)
        {
            // First, we need the y-intercepts for each line.
            float yi1 = LineF.FindYIntercept(val1).Y;
            float yi2 = LineF.FindYIntercept(val2).Y;

            // Now that we have the slope and y-intercept of each line, the equations
            //   to find the intercept are easy.
            double ix = ((-yi1) + yi2) / (val1.Slope - val2.Slope);
            double iy = (((-val1.Slope) * yi2) + (val2.Slope * yi1)) / (val1.Slope - val2.Slope);

            // And then, we just return our new point, but inverting the y-coordinate,
            //   due to the method used to determine the y-intercept of our lines.
            return new PointF((float)ix, (float)(-iy));
        }
        public static PointF FindYIntercept(LineF value)
        {
            // First, we have to determine which point on the line is closest to the axis.
            // This will be the only point we use for calculations.
            PointF p = ((value.PointA.X < value.PointB.X) ? value.PointA : value.PointB);
            return FindYIntercept(p, value.Slope);
        }
        public static PointF FindYIntercept(PointF p, float slope)
        {
            // Now, using that point, we determine the y-intercept by using the
            //   point as the origin of the line's own personal coordinate system.
            // Using this method, we can use the y=mx+b equation.  For this scope, 'm'
            //   equals the slope of the line.  'x' represents the distance of the point
            //   from the 'real' y-axis.  Using the line's 'local' coordinate system,
            //   this means the 'x' in our equation is equal to the negative distance
            //   from the point's 'real' x-coordinate to the 'real' origin.
            // The result is that we get the number of units we need to move up or down
            //   the y-axis in the 'real' coordinate plane.
            float yi = p.Y - (p.X * slope);

            // Now, the y-intercept is (0,yi)
            return new PointF(0, yi);
        }
        #endregion
    }
}
