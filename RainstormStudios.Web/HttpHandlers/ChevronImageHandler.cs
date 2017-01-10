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
using System.Xml.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using RainstormStudios.Web.UI.WebControls;

// NOTE:  To use the ChevronHeader control, this handler must be referenced in the application's web.config file.
// 
// <add path="chevron.axd" verb="*" type="RainstormStudios.Web.HttpHandlers.ChevronImageHandler, RainstormStudios.Web"/>
// 

namespace RainstormStudios.Web.HttpHandlers
{
    public class ChevronImageHandler : IHttpHandler
    {
        #region Nested Classes
        //***************************************************************************
        // Nested Classes
        // 
        class ChevronShapeFile : IDisposable
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            DateTime
                _lastParse;
            ChevronShapeCollection
                _shapes;
            System.IO.Stream
                _fileStream;
            string
                _srcUrl;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShapeCollection Shapes
            {
                get
                {
                    if (this._shapes == null || DateTime.Now.AddMinutes(-5) > this._lastParse)
                        this.ParseFile();
                    return this._shapes;
                }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronShapeFile(string fileUrl)
            {
                this._srcUrl = fileUrl;
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(fileUrl);
                req.Method = "GET";
                req.Accept = "text/xml, */*";
                req.SendChunked = false;
                //req.ProtocolVersion = System.Net.HttpVersion.Version10;
                req.KeepAlive = false;
                req.ContentLength = 0;
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; GTB7.4; SLCC2; .NET CLR 2.0.50727; Media Center PC 6.0; .NET4.0C; InfoPath.3; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0E)";
                req.Host = "RainstormStudios.net";

                System.Net.WebResponse response = null;
                System.IO.Stream strm = null;

                try
                { response = req.GetResponse(); }
                catch (Exception ex)
                { throw new ChevronShapeFileLoadException("Unable to get response from shape file URL: " + fileUrl, ex); }
                try
                { strm = response.GetResponseStream(); }
                catch (Exception ex)
                { throw new ChevronShapeFileLoadException("Error gettting response stream from shape file URL: " + fileUrl, ex); }
                this.LoadStream(strm);
            }
            public ChevronShapeFile(System.IO.Stream shapeFileStream)
            {
                this.LoadStream(shapeFileStream);
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Dispose()
            {
                if (this._fileStream != null)
                    this._fileStream.Dispose();
            }
            public void ParseFile()
            {
                try
                {
                    this._lastParse = DateTime.Now;
                    if (this._shapes != null)
                        this._shapes.Clear();
                    this._shapes = new ChevronShapeCollection(this);
                    // Reset the file stream to the starting position.
                    this._fileStream.Position = 0;
                    XDocument xml = XDocument.Load(this._fileStream);

                    var shapes = (from c in xml.Root.Descendants("shape")
                                  select c);

                    foreach (XElement shapeRecord in shapes)
                    {
                        ChevronShape shape = null;

                        // Read the name and offset from the XML node.
                        string
                            shapeName = (string)shapeRecord.Attribute("name"),
                            shapeOffset = (string)shapeRecord.Attribute("offsetAdjustment");

                        // Get the textBounds element, if there is one.
                        XElement xTxtBounds = shapeRecord.Element("textBounds");
                        if (xTxtBounds != null)
                        {
                            string
                                x = xTxtBounds.Attribute("x").Value,
                                y = xTxtBounds.Attribute("y").Value,
                                w = xTxtBounds.Attribute("w").Value,
                                h = xTxtBounds.Attribute("h").Value;
                            shape = new ChevronShape(shapeName, shapeOffset, x, y, w, h);
                        }
                        else
                        {
                            // If we didn't find a textBounds node, just create the shape
                            //   without specific text bounds.
                            shape = new ChevronShape(shapeName, shapeOffset);
                        }

                        // Now, we're ready to read the points, lines and variables.
                        foreach (XElement pointRecord in shapeRecord.Element("points").Elements("point"))
                        {
                            string
                                ptName = pointRecord.Attribute("name").Value,
                                ptX = pointRecord.Attribute("x").Value,
                                ptY = pointRecord.Attribute("y").Value;

                            shape.AddPoint(ptName, ptX, ptY);
                        }
                        foreach (XElement lineRecord in shapeRecord.Element("lines").Elements("line"))
                        {
                            string
                                startPointNm = lineRecord.Attribute("StartPoint").Value,
                                endPointNm = lineRecord.Attribute("EndPoint").Value;

                            bool isBezier = false;
                            XAttribute bezierAttr = lineRecord.Attribute("BezierLine");
                            string ctrlPt1 = string.Empty, ctrlPt2 = string.Empty;
                            if (bezierAttr != null)
                                if (bool.TryParse(bezierAttr.Value, out isBezier) && isBezier)
                                {
                                    ctrlPt1 = lineRecord.Attribute("CtrlPoint1").Value;
                                    ctrlPt2 = lineRecord.Attribute("CtrlPoint2").Value;
                                }

                            if (!isBezier)
                                shape.AddLine(startPointNm, endPointNm);
                            else
                                shape.AddBezier(startPointNm, endPointNm, ctrlPt1, ctrlPt2);
                        }
                        foreach (XElement varRecord in shapeRecord.Element("variables").Elements("variable"))
                        {
                            string
                                varName = varRecord.Attribute("name").Value,
                                varExpr = varRecord.Attribute("value").Value;
                            shape.AddVariable(varName, varExpr);
                        }

                        // Last, we just want to load the "startShape" and "endShape", if there is one
                        XElement startShapeRec = shapeRecord.Element("startShape");
                        if (startShapeRec != null)
                        {
                            XAttribute xOffset = startShapeRec.Attribute("offsetAdjustment");
                            ChevronPreShape startShp = new ChevronPreShape(shape, startShapeRec.Attribute("width").Value, xOffset != null ? xOffset.Value : string.Empty);

                            foreach (XElement pointRecord in startShapeRec.Element("points").Elements("point"))
                            {
                                string
                                    ptName = pointRecord.Attribute("name").Value,
                                    ptX = pointRecord.Attribute("x").Value,
                                    ptY = pointRecord.Attribute("y").Value;

                                startShp.AddPoint(ptName, ptX, ptY);
                            }
                            foreach (XElement lineRecord in startShapeRec.Elements("lines").Elements("line"))
                            {
                                string
                                    startPointNm = lineRecord.Attribute("StartPoint").Value,
                                    endPointNm = lineRecord.Attribute("EndPoint").Value;

                                bool isBezier = false;
                                XAttribute bezierAttr = lineRecord.Attribute("BezierLine");
                                string ctrlPt1 = string.Empty, ctrlPt2 = string.Empty;
                                if (bezierAttr != null)
                                    if (bool.TryParse(bezierAttr.Value, out isBezier) && isBezier)
                                    {
                                        ctrlPt1 = lineRecord.Attribute("CtrlPoint1").Value;
                                        ctrlPt2 = lineRecord.Attribute("CtrlPoint2").Value;
                                    }

                                if (!isBezier)
                                    startShp.AddLine(startPointNm, endPointNm);
                                else
                                    startShp.AddBezier(startPointNm, endPointNm, ctrlPt1, ctrlPt2);
                            }
                            shape.StartShape = startShp;
                        }
                        XElement endShapeRec = shapeRecord.Element("endShape");
                        if (endShapeRec != null)
                        {
                            XAttribute xOffset = startShapeRec.Attribute("offsetAdjustment");
                            ChevronPreShape endShp = new ChevronPreShape(shape, endShapeRec.Attribute("width").Value, xOffset != null ? xOffset.Value : string.Empty);

                            foreach (XElement pointRecord in endShapeRec.Element("points").Elements("point"))
                            {
                                string
                                    ptName = pointRecord.Attribute("name").Value,
                                    ptX = pointRecord.Attribute("x").Value,
                                    ptY = pointRecord.Attribute("y").Value;

                                endShp.AddPoint(ptName, ptX, ptY);
                            }
                            foreach (XElement lineRecord in endShapeRec.Elements("lines").Elements("line"))
                            {
                                string
                                    startPointNm = lineRecord.Attribute("StartPoint").Value,
                                    endPointNm = lineRecord.Attribute("EndPoint").Value;

                                bool isBezier = false;
                                XAttribute bezierAttr = lineRecord.Attribute("BezierLine");
                                string ctrlPt1 = string.Empty, ctrlPt2 = string.Empty;
                                if (bezierAttr != null)
                                    if (bool.TryParse(bezierAttr.Value, out isBezier) && isBezier)
                                    {
                                        ctrlPt1 = lineRecord.Attribute("CtrlPoint1").Value;
                                        ctrlPt2 = lineRecord.Attribute("CtrlPoint2").Value;
                                    }

                                if (!isBezier)
                                    endShp.AddLine(startPointNm, endPointNm);
                                else
                                    endShp.AddBezier(startPointNm, endPointNm, ctrlPt1, ctrlPt2);
                            }
                            shape.EndShape = endShp;
                        }

                        this.Shapes.Add(shape);
                    }
                }
                catch (Exception ex)
                { throw new ChevronShapeFileParseException("There was an error while parsing the chevron shape file: " + ex.Message, ex); }
            }
            #endregion

            #region Private Methods
            //***********************************************************************
            // Private Methods
            // 
            private void LoadStream(string shapeFileContent)
            {
                System.IO.MemoryStream strm = new System.IO.MemoryStream();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(shapeFileContent);
                strm.Write(buffer, 0, buffer.Length);
                this.LoadStream(strm);
            }
            private void LoadStream(System.IO.Stream shapeFileStream)
            {
                this._fileStream = shapeFileStream;
            }
            #endregion
        }
        abstract class ChevronShapeBase
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            protected ChevronPointCollection
                _points;
            protected ChevronLineCollection
                _lines;
            protected string
                _offsetExpr;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronPointCollection Points
            { get { return this._points; } }
            public ChevronLineCollection Lines
            { get { return this._lines; } }
            abstract public ChevronVariableCollection Variables { get; }
            public string OffsetExpression
            { get { return this._offsetExpr; } }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            protected ChevronShapeBase()
            {
                // Prep the collections to hold the shape data.
                this._points = new ChevronPointCollection(this);
                this._lines = new ChevronLineCollection(this);

                // We're setting these collection objects to return null if the
                //   specified key is not, but an out-of-range index will still
                //   throw an exception.
                this._points.ReturnNullForKeyNotFound = true;
                this._lines.ReturnNullForKeyNotFound = true;

            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public Point[] GetCoords(Rectangle bounds, int itemSpacing)
            {
                List<Point> pnts = new List<Point>();
                for (int i = 0; i < this._points.Count; i++)
                    pnts.Add(this._points[i].GetCoordinate(bounds, itemSpacing));
                return pnts.ToArray();
            }
            public GraphicsPath GetPath(Rectangle bounds, int itemSpacing)
            { return GetPath(bounds, itemSpacing, FillMode.Winding); }
            public GraphicsPath GetPath(Rectangle bounds, int itemSpacing, FillMode fillMode)
            {
                GraphicsPath path = new GraphicsPath(fillMode);

                if (this._lines.Count > 0)
                    for (int i = 0; i < this._lines.Count; i++)
                        if (!this._lines[i].IsBezier)
                            path.AddLine(this._lines[i].StartPoint.GetCoordinate(bounds, itemSpacing), this._lines[i].EndPoint.GetCoordinate(bounds, itemSpacing));
                        else
                            path.AddBezier(this._lines[i].StartPoint.GetCoordinate(bounds, itemSpacing), this._lines[i].CtrlPt1.GetCoordinate(bounds, itemSpacing), this._lines[i].CtrlPt2.GetCoordinate(bounds, itemSpacing), this._lines[i].EndPoint.GetCoordinate(bounds, itemSpacing));

                else
                    path.AddPolygon(this.GetCoords(bounds, itemSpacing));

                return path;
            }
            public NCalc.Expression GetExpressionParser(Rectangle bounds, int itemSpacing, string expr)
            {
                NCalc.Expression e = new NCalc.Expression(expr);

                e.Parameters["T"] = bounds.Top;
                e.Parameters["L"] = bounds.Left;
                e.Parameters["B"] = bounds.Bottom;
                e.Parameters["R"] = bounds.Right;
                e.Parameters["W"] = bounds.Width;
                e.Parameters["H"] = bounds.Height;
                e.Parameters["S"] = itemSpacing;

                try
                {
                    for (int i = 0; i < this.Variables.Count; i++)
                    {
                        ChevronVariable variable = this.Variables[i];
                        e.Parameters[variable.Name] = new NCalc.Expression(variable.Expression);
                    }
                }
                catch (Exception ex)
                { throw new Exception("Unable to add variables to the expression evaluation: " + ex.Message, ex); }

                // I'm excluding the references to other points for now.  This poses a requirement of "pre-parsing" the
                //   expression to determine what other points are needed and poses too large of a performance risk
                //   caused by iterative calls to this method.
                //try
                //{
                //    for (int i = 0; i < shape.Points.Count; i++)
                //    {
                //        ChevronPoint p = shape.Points[i];
                //        if (!string.IsNullOrEmpty(p.Name) && p.Name != this.Name)
                //        {
                //            e.Parameters["#" + p.Name] = (p.Coordinate != Point.Empty ? p.Coordinate : p.GetCoordinate(bounds));
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{ throw new Exception("An error occured while loading values of other points: " + ex.Message, ex); }

                return e;
            }
            public int GetExpressionResult(string expr, Rectangle bounds, int itemSpacing)
            {
                NCalc.Expression e = this.GetExpressionParser(bounds, itemSpacing, expr);

                object eVal = e.Evaluate();
                float iVal;
                if (eVal == null)
                    throw new Exception("Expression evaluated to a NULL value.");
                else if (!float.TryParse(eVal.ToString(), out iVal))
                    throw new Exception("Expression evaluated to a non-integer value.");
                else
                    return (int)System.Math.Round(iVal);
            }
            public void AddPoint(string name, string xExpr, string yExpr)
            { this.Points.Add(new ChevronPoint(name, xExpr, yExpr, this)); }
            public void AddLine(string startPointName, string endPointName)
            {
                ChevronPoint sPoint = this.Points[startPointName];
                if (sPoint == null)
                    throw new ArgumentOutOfRangeException("startPoint", "Specified start point name was not found in the Points collection.");

                ChevronPoint ePoint = this.Points[endPointName];
                if (ePoint == null)
                    throw new ArgumentOutOfRangeException("endPointName", "Specified end point name was not found in the Points collection.");

                this.Lines.Add(new ChevronLine(this, sPoint, ePoint));
            }
            public void AddBezier(string startPointName, string endPointName, string ctrlPt1Name, string ctrlPt2Name)
            {
                ChevronPoint sPoint = this.Points[startPointName];
                if (sPoint == null)
                    throw new ArgumentOutOfRangeException("startPoint", "Specified start point name was not found in the Points collection.");

                ChevronPoint ePoint = this.Points[endPointName];
                if (ePoint == null)
                    throw new ArgumentOutOfRangeException("endPointName", "Specified end point name was not found in the Points collection.");

                ChevronPoint ctrlPt1 = this.Points[ctrlPt1Name];
                if (ctrlPt1 == null)
                    throw new ArgumentOutOfRangeException("ctrlPt1Name", "Specified control point name was not found in the Points collection.");

                ChevronPoint ctrlPt2 = this.Points[ctrlPt2Name];
                if (ctrlPt2 == null)
                    throw new ArgumentOutOfRangeException("ctrlPt2Name", "Specified control point name was not found in the Points collection.");

                this.Lines.Add(new ChevronLine(this, sPoint, ePoint, ctrlPt1, ctrlPt2));
            }
            public int GetOffset(Rectangle bounds, int itemSpacing)
            {
                return !string.IsNullOrEmpty(this.OffsetExpression)
                        ? this.GetExpressionResult(this.OffsetExpression, bounds, itemSpacing)
                        : 0;
            }
            #endregion
        }
        class ChevronShape : ChevronShapeBase
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            string
                _shapeNm,
                _txtBndsX,
                _txtBndsY,
                _txtBndsW,
                _txtBndsH;
            ChevronVariableCollection
                _variables;
            ChevronShapeCollection
                _owner;
            ChevronPreShape
                _startShape,
                _endShape;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public string Name
            { get { return this._shapeNm; } }
            public override ChevronVariableCollection Variables
            { get { return this._variables; } }
            public ChevronShapeCollection Owner
            {
                get { return this._owner; }
                internal set { this._owner = value; }
            }
            public ChevronShapeFile Parent
            { get { return this._owner.Parent; } }
            public ChevronPreShape StartShape
            {
                get { return this._startShape; }
                internal set { this._startShape = value; }
            }
            public ChevronPreShape EndShape
            {
                get { return this._endShape; }
                internal set { this._endShape = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronShape()
                : base()
            {
                // Prep the collections to hold the shape data.
                this._points = new ChevronPointCollection(this);
                this._lines = new ChevronLineCollection(this);
                this._variables = new ChevronVariableCollection(this);

                // We're setting these collection objects to return null if the
                //   specified key is not, but an out-of-range index will still
                //   throw an exception.
                this._points.ReturnNullForKeyNotFound = true;
                this._lines.ReturnNullForKeyNotFound = true;
                this._variables.ReturnNullForKeyNotFound = true;
            }
            public ChevronShape(string name)
                : this()
            {
                this._shapeNm = name;
            }
            public ChevronShape(string name, string offsetExpr)
                : this(name)
            {
                this._offsetExpr = offsetExpr;
            }
            public ChevronShape(string name, string offsetExpr, string txtBndXExpr, string txtBndYExpr, string txtBndWExpr, string txtBndHExpr)
                : this(name, offsetExpr)
            {
                this._txtBndsX = txtBndXExpr;
                this._txtBndsY = txtBndYExpr;
                this._txtBndsW = txtBndWExpr;
                this._txtBndsH = txtBndHExpr;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public Rectangle GetTextBounds(Rectangle bounds)
            { return this.GetTextBounds(bounds, this._txtBndsX, this._txtBndsY, this._txtBndsW, this._txtBndsH); }
            public void AddVariable(string variableName, string expr)
            { this.Variables.Add(new ChevronVariable(variableName, expr, this)); }
            #endregion

            #region Private Methods
            //***********************************************************************
            // Private Methods
            // 
            internal void SetTextBoundsExpressions(string xExpr, string yExpr, string wExpr, string hExpr)
            {
                this._txtBndsX = xExpr;
                this._txtBndsY = yExpr;
                this._txtBndsW = wExpr;
                this._txtBndsH = hExpr;
            }
            private Rectangle GetTextBounds(Rectangle bounds, string xExpr, string yExpr, string wExpr, string hExpr)
            {
                int x = (int)System.Math.Round(Convert.ToDouble(GetExpressionParser(bounds, 0, xExpr).Evaluate()));
                int y = (int)System.Math.Round(Convert.ToDouble(GetExpressionParser(bounds, 0, yExpr).Evaluate()));
                int w = (int)System.Math.Round(Convert.ToDouble(GetExpressionParser(bounds, 0, wExpr).Evaluate()));
                int h = (int)System.Math.Round(Convert.ToDouble(GetExpressionParser(bounds, 0, hExpr).Evaluate()));

                return new Rectangle(x, y, w, h);
            }
            #endregion
        }
        class ChevronShapeCollection : Collections.ObjectCollectionBase<ChevronShape>
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            ChevronShapeFile
                _owner;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShapeFile Parent
            { get { return this._owner; } }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronShapeCollection(ChevronShapeFile parent)
                : base()
            {
                this._owner = parent;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Add(ChevronShape value)
            {
                if (base._keys.Contains(value.Name))
                    throw new Exception(string.Format("Chevron shape '{0}' has already been defined.  Please ensure that there are no duplicate shape names in XML file.", value.Name));

                value.Owner = this;
                base.Add(value, value.Name);
            }
            #endregion
        }
        class ChevronPreShape : ChevronShapeBase
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            private string
                _width,
                _offset;
            private ChevronShape
                _owner;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public string WidthExpression
            { get { return this._width; } }
            public override ChevronVariableCollection Variables
            {
                get { return this.Parent.Variables; }
            }
            public ChevronShape Parent
            { get { return this._owner; } }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            private ChevronPreShape()
                : base()
            {
                // Prep the collections to hold the shape data.
                this._points = new ChevronPointCollection(this);
                this._lines = new ChevronLineCollection(this);

                // We're setting these collection objects to return null if the
                //   specified key is not, but an out-of-range index will still
                //   throw an exception.
                this._points.ReturnNullForKeyNotFound = true;
                this._lines.ReturnNullForKeyNotFound = true;
            }
            public ChevronPreShape(ChevronShape parentShape, string widthExpression, string offsetExpression)
                : this()
            {
                this._owner = parentShape;
                this._width = widthExpression;
                this._offset = offsetExpression;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public Rectangle GetBounds(int height)
            {
                return new Rectangle(new Point(0, 0), new Size(this.GetWidthExpressionResult(this._width, height), height));
            }
            public NCalc.Expression GetWidthExpressionParser(string expr, int height)
            {
                NCalc.Expression e = new NCalc.Expression(expr);

                e.Parameters["H"] = height;

                try
                {
                    for (int i = 0; i < this.Variables.Count; i++)
                    {
                        ChevronVariable variable = this.Variables[i];
                        e.Parameters[variable.Name] = new NCalc.Expression(variable.Expression);
                    }
                }
                catch (Exception ex)
                { throw new Exception("Unable to add variables to the expression evaluation: " + ex.Message, ex); }

                return e;
            }
            public int GetWidthExpressionResult(string expr, int height)
            {
                NCalc.Expression e = this.GetWidthExpressionParser(expr, height);

                object eVal = e.Evaluate();
                float iVal;
                if (eVal == null)
                    throw new Exception("Expression evaluated to a NULL value.");
                else if (!float.TryParse(eVal.ToString(), out iVal))
                    throw new Exception("Expression evaluated to a non-integer value.");
                else
                    return (int)System.Math.Round(iVal);
            }
            #endregion
        }
        class ChevronPoint
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            string
                _pointNm,
                _xVal,
                _yVal;
            ChevronShapeBase
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properites
            // 
            public string Name
            { get { return this._pointNm; } }
            public string XExpression
            { get { return this._xVal; } }
            public string YExpression
            { get { return this._yVal; } }
            public ChevronShapeBase Owner
            {
                get { return this._parent; }
                internal set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronPoint(string name, string xExpression, string yExpression, ChevronShapeBase owner)
            {
                this._pointNm = name;
                this._parent = owner;
                this._xVal = xExpression;
                this._yVal = yExpression;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public Point GetCoordinate(Rectangle bounds, int itemSpacing)
            {
                //int x = GetExpressionResult(this._xVal, bounds);
                //int y = GetExpressionResult(this._yVal, bounds);
                int x = this._parent.GetExpressionResult(this._xVal, bounds, itemSpacing);
                int y = this._parent.GetExpressionResult(this._yVal, bounds, itemSpacing);

                return new Point(x, y);
            }
            #endregion

            #region Private Methods
            //***********************************************************************
            // Private Methods
            // 
            #endregion
        }
        class ChevronPointCollection : Collections.ObjectCollectionBase<ChevronPoint>
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            ChevronShapeBase
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShapeBase Owner
            {
                get { return this._parent; }
                internal set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronPointCollection(ChevronShapeBase owner)
                : base()
            {
                this._parent = owner;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Add(ChevronPoint value)
            { base.Add(value, value.Name); }
            #endregion
        }
        class ChevronLine
        {
            #region Declaration
            //***********************************************************************
            // Private Fields
            // 
            ChevronShapeBase
                _owner;
            ChevronPoint
                _start,
                _end,
                _ctrl1,
                _ctrl2;
            bool
                _bezier;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShapeBase Parent
            { get { return this._owner; } }
            public ChevronPoint StartPoint
            { get { return this._start; } }
            public ChevronPoint EndPoint
            { get { return this._end; } }
            public ChevronPoint CtrlPt1
            { get { return this._ctrl1; } }
            public ChevronPoint CtrlPt2
            { get { return this._ctrl2; } }
            public bool IsBezier
            { get { return this._bezier; } }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronLine(ChevronShapeBase parent)
            {
                this._owner = parent;
            }
            public ChevronLine(ChevronShapeBase parent, string startPointName, string endPointName)
                : this(parent)
            {
                this._start = parent.Points[startPointName];
                this._end = parent.Points[endPointName];
            }
            public ChevronLine(ChevronShapeBase parent, string startPointName, string endPointName, string ctrlPt1Name, string ctrlPt2Name)
                : this(parent, startPointName, endPointName)
            {
                this._bezier = true;
                this._ctrl1 = parent.Points[ctrlPt1Name];
                this._ctrl2 = parent.Points[ctrlPt2Name];
            }
            public ChevronLine(ChevronShapeBase parent, int startPointIdx, int endPointIdx)
                : this(parent)
            {
                this._start = parent.Points[startPointIdx];
                this._end = parent.Points[endPointIdx];
            }
            public ChevronLine(ChevronShapeBase parent, int startPointIdx, int endPointIdx, int ctrlPt1Idx, int ctrlPt2Idx)
                : this(parent, startPointIdx, endPointIdx)
            {
                this._bezier = true;
                this._ctrl1 = parent.Points[ctrlPt1Idx];
                this._ctrl2 = parent.Points[ctrlPt2Idx];
            }
            public ChevronLine(ChevronShapeBase parent, ChevronPoint startPoint, ChevronPoint endPoint)
                : this(parent)
            {
                this._start = startPoint;
                this._end = endPoint;
            }
            public ChevronLine(ChevronShapeBase parent, ChevronPoint startPoint, ChevronPoint endPoint, ChevronPoint ctrlPt1, ChevronPoint ctrlPt2)
                : this(parent, startPoint, endPoint)
            {
                this._bezier = true;
                this._ctrl1 = ctrlPt1;
                this._ctrl2 = ctrlPt2;
            }
            #endregion
        }
        class ChevronLineCollection : Collections.ObjectCollectionBase<ChevronLine>
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            ChevronShapeBase
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShapeBase Owner
            {
                get { return this._parent; }
                internal set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronLineCollection(ChevronShapeBase owner)
                : base()
            {
                this._parent = owner;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Add(ChevronLine value)
            {

                base.Add(value, string.Empty);
            }
            #endregion
        }
        class ChevronVariable
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            string
                _varNm,
                _varExpr;
            ChevronShape
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public string Name
            { get { return this._varNm; } }
            public string Expression
            { get { return this._varExpr; } }
            public ChevronShape Owner
            {
                get { return this._parent; }
                internal set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronVariable(string name, string expression, ChevronShape owner)
            {
                this._varNm = name;
                this._varExpr = expression;
                this._parent = owner;
            }
            #endregion
        }
        class ChevronVariableCollection : Collections.ObjectCollectionBase<ChevronVariable>
        {
            #region Declarations
            //***********************************************************************
            // Private Fields
            // 
            ChevronShape
                _parent;
            #endregion

            #region Properties
            //***********************************************************************
            // Public Properties
            // 
            public ChevronShape Owner
            {
                get { return this._parent; }
                internal set { this._parent = value; }
            }
            #endregion

            #region Class Constructors
            //***********************************************************************
            // Class Constructors
            // 
            public ChevronVariableCollection(ChevronShape owner)
                : base()
            {
                this._parent = owner;
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void Add(ChevronVariable value)
            {
                if (base._keys.Contains(value.Name))
                    throw new Exception(string.Format("Variable name '{0}' is specified twice.  Please provide each variable with a unique name.", value.Name));

                value.Owner = this._parent;
                base.Add(value, value.Name);
            }
            #endregion
        }
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public bool IsReusable
        { get { return false; } }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;
            //DateTime dtStart = DateTime.Now;
            int imgPadding = 0;

            #region Read Values From QueryString
            string
                sH = req.QueryString["h"],
                sTW = req.QueryString["w"],
                sItmW = req.QueryString["itmw"],
                sParts = req.QueryString["pcs"],
                sImgParts = req.QueryString["img"],
                sAct = req.QueryString["act"],
                sSpc = req.QueryString["spc"],
                sTClr = req.QueryString["tClr"],
                sBClr = req.QueryString["bClr"],
                sFClr = req.QueryString["fClr"],
                sATClr = req.QueryString["atClr"],
                sABClr = req.QueryString["abClr"],
                sCTClr = req.QueryString["ctClr"],
                sCBClr = req.QueryString["cbClr"],
                sFBld = req.QueryString["bld"],
                sFItl = req.QueryString["itl"],
                sFont = req.QueryString["fnt"],
                sFSz = req.QueryString["fsz"],
                sTxtPd = req.QueryString["tpd"],
                sTxtWrp = req.QueryString["twp"],
                sBorderClr = req.QueryString["bdrClr"],
                sAccMd = req.QueryString["acMd"],
                sAccOp = req.QueryString["acOp"],
                sBackClr = req.QueryString["bckClr"],
                sBackTrns = req.QueryString["bckTrns"],
                sCompTrns = req.QueryString["cmpTrns"],
                sActvTrns = req.QueryString["actTrns"],
                sBrdrTrns = req.QueryString["bdrTrns"],
                sBrdrWidth = req.QueryString["bdrWdt"],
                stxtShdwOn = req.QueryString["tsOn"],
                stxtShdwOpac = req.QueryString["tsOp"],
                //sBvl = req.QueryString["bvl"],
                sfUrl = req.QueryString["shpFl"],
                shpNm = req.QueryString["shpNm"];
            if (string.IsNullOrEmpty(sParts))
                return;
            string[]
                arParts = sParts.Split(new char[] { '|' }, StringSplitOptions.None),
                imgParts = sImgParts.Split(new char[] { '|' }, StringSplitOptions.None);
            #endregion

            #region Process QS Values
            int iTW, iItmW, iH;
            bool hasTotalW = false, hasItemW = false;
            if (int.TryParse(sTW, out iTW))
                hasTotalW = true;
            if (int.TryParse(sItmW, out iItmW))
                hasItemW = true;
            if (!int.TryParse(sH, out iH))
                throw new ChevronQueryStringParseException(sH, "Height", "integer");

            if (!hasTotalW && !hasItemW)
                throw new ArgumentException("You must specify either the total width, or the item width.");

            int iSpc;
            if (string.IsNullOrEmpty(sSpc))
                sSpc = "2";
            if (!int.TryParse(sSpc, out iSpc))
                throw new ChevronQueryStringParseException(sSpc, "ItemSpacing", "integer");

            float ifontSz;
            if (string.IsNullOrEmpty(sFSz) || sFSz == "0")
                sFSz = "10.0";
            if (!float.TryParse(sFSz, out ifontSz))
                throw new ChevronQueryStringParseException(sFSz, "FontSize", "floating point number");

            bool bFontBold;
            if (string.IsNullOrEmpty(sFBld) || sFBld == "0")
                sFBld = "false";
            else
                sFBld = "true";
            if (!bool.TryParse(sFBld, out bFontBold))
                throw new ChevronQueryStringParseException(sFBld, "FontBold", "boolean");

            bool bFontItalic;
            if (string.IsNullOrEmpty(sFItl) || sFItl == "0")
                sFItl = "false";
            else
                sFItl = "true";
            if (!bool.TryParse(sFItl, out bFontItalic))
                throw new ChevronQueryStringParseException(sFItl, "FontItalic", "boolean");

            int iTextPadding;
            if (string.IsNullOrEmpty(sTxtPd))
                sTxtPd = "0";
            if (!int.TryParse(sTxtPd, out iTextPadding))
                throw new ChevronQueryStringParseException(sTxtPd, "TextPadding", "integer");

            bool bTextWrap;
            if (string.IsNullOrEmpty(sTxtWrp) || sTxtWrp == "0")
                sTxtWrp = "false";
            else
                sTxtWrp = "true";
            if (!bool.TryParse(sTxtWrp, out bTextWrap))
                throw new ChevronQueryStringParseException(sTxtWrp, "TextWrap", "boolean");

            bool bTxtShadow;
            if (string.IsNullOrEmpty(stxtShdwOn) || stxtShdwOn == "0")
                stxtShdwOn = "false";
            else
                stxtShdwOn = "true";
            if (!bool.TryParse(stxtShdwOn, out bTxtShadow))
                throw new ChevronQueryStringParseException(stxtShdwOn, "TextShadow", "boolean");

            int iTxtShadowOpac;
            if (string.IsNullOrEmpty(stxtShdwOpac))
                stxtShdwOpac = "90";
            if (!int.TryParse(stxtShdwOpac, out iTxtShadowOpac))
                throw new ChevronQueryStringParseException(stxtShdwOpac, "TextShadowOpacity", "integer");

            int iActiveStep;
            if (!int.TryParse(sAct, out iActiveStep))
                throw new ChevronQueryStringParseException(sAct, "ActiveStepIndex", "integer");
            iActiveStep = System.Math.Max(0, iActiveStep);

            Color foreClr = Hex.GetSystemColor(sFClr);
            Color backClr1 = Hex.GetSystemColor(sTClr);
            if (string.IsNullOrEmpty(sBClr))
                sBClr = sTClr;
            Color backClr2 = Hex.GetSystemColor(sBClr);
            if (string.IsNullOrEmpty(sATClr))
                sATClr = sTClr;
            Color actBackClr1 = Hex.GetSystemColor(sATClr);
            if (string.IsNullOrEmpty(sABClr))
                sABClr = sBClr;
            Color actBackClr2 = Hex.GetSystemColor(sABClr);
            Color bdrColor = Color.Transparent;
            if (!string.IsNullOrEmpty(sBorderClr))
                bdrColor = Hex.GetSystemColor(sBorderClr);
            if (string.IsNullOrEmpty(sBackClr))
                sBackClr = "#ffffff";
            Color backClr = Hex.GetSystemColor(sBackClr);
            Color compClr1 = Hex.GetSystemColor(sCTClr);
            if (string.IsNullOrEmpty(sCTClr))
                compClr1 = backClr1;
            Color compClr2 = Hex.GetSystemColor(sCBClr);
            if (string.IsNullOrEmpty(sCBClr))
                if (!string.IsNullOrEmpty(sCTClr))
                    compClr2 = compClr1;
                else
                    compClr2 = backClr2;

            int iBackTrans, iCompTrans, iActvTrans, iBrdrTrns, iBrdrWidth;
            if (!int.TryParse(sBackTrns, out iBackTrans))
                iBackTrans = 255;
            if (!int.TryParse(sCompTrns, out iCompTrans))
                iCompTrans = 255;
            if (!int.TryParse(sActvTrns, out iActvTrans))
                iActvTrans = 255;
            if (!int.TryParse(sBrdrTrns, out iBrdrTrns))
                iBrdrTrns = 180;
            if (!int.TryParse(sBrdrWidth, out iBrdrWidth))
                iBrdrWidth = 1;

            //bool reflectImg;
            //if (!bool.TryParse(sReflect, out reflectImg))
            //    reflectImg = false;
            int iAccMode;
            if (!int.TryParse(sAccMd, out iAccMode))
                throw new ChevronQueryStringParseException(sAccMd, "AccentMode", "ChevronAccent");
            ChevronAccent accentMode = (ChevronAccent)iAccMode;

            int iAccOpacity;
            if (!int.TryParse(sAccOp, out iAccOpacity))
                throw new ChevronQueryStringParseException(sAccOp, "AccentBaseOpacity", "integer");

            //bool bevelImg;
            //if (!bool.TryParse(sBvl, out bevelImg))
            //    bevelImg = false;

            int iTotalWidth, iPartWidth;
            //int iPartHeight = (accentMode != ChevronAccent.None) ? (int)(iH / 2) : iH;
            int iPartHeight = iH;
            int iTotalHeight = (accentMode != ChevronAccent.None) ? (iH * 2) : iH;
            if (iItmW > 0)
            {
                iTotalWidth = (int)((iItmW * arParts.Length) + (iSpc * (arParts.Length - 1)));
                if (accentMode != ChevronAccent.None)
                    iTotalWidth += (iH / 2);
                iPartWidth = iItmW;
            }
            else
            {
                iTotalWidth = (accentMode != ChevronAccent.None) ? (int)(iTW + (iPartHeight / 2)) : iTW;
                iPartWidth = (iTotalWidth / arParts.Length) - (iSpc * (arParts.Length - 1));
            }

            FontStyle fontStyle = FontStyle.Regular;
            if (bFontBold)
                fontStyle |= FontStyle.Bold;
            if (bFontItalic)
                fontStyle |= FontStyle.Italic;
            #endregion

            #region Load Any Item Images
            Collections.BitmapCollection colImgParts = new Collections.BitmapCollection();
            for (int i = 0; i < imgParts.Length; i++)
            {
                // Make sure we don't waste time loading the same image more than once.
                if (string.IsNullOrEmpty(imgParts[i].Trim()) || colImgParts.ContainsKey(imgParts[i].Trim()))
                    continue;

                byte[] imgBuffer = null;
                // First, we're going to try and map the image path to the local server, to
                //   see if we can just read it as a file stream.
                try
                {
                    // Ask the server to map the path for us.
                    string lcPath = context.Server.MapPath(imgParts[i].Trim());

                    // Yay!  It's local and we've got a physical disk path.
                    using (System.IO.FileStream fs = new System.IO.FileStream(lcPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        imgBuffer = new byte[fs.Length];
                        fs.Read(imgBuffer, 0, imgBuffer.Length);
                    }
                }
                catch
                {
                    // Nope :(  Gotta create a WebRequest to go get the image.
                    try
                    {
                        string fullImgUrl = WebUtil.MakeAbsoluteUrl(imgParts[i]);
                        System.Net.HttpWebRequest imgReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(fullImgUrl);
                        imgReq.Method = "GET";
                        imgReq.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                        imgReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; GTB7.4; SLCC2; .NET CLR 2.0.50727; Media Center PC 6.0; .NET4.0C; InfoPath.3; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0E)";
                        imgReq.Host = context.Request.Url.Host;
                        System.Net.HttpWebResponse imgResp = (System.Net.HttpWebResponse)imgReq.GetResponse();
                        using (System.IO.Stream strmResp = imgResp.GetResponseStream())
                        {
                            imgBuffer = new byte[strmResp.Length];
                            strmResp.Read(imgBuffer, 0, imgBuffer.Length);
                        }
                    }
                    catch (Exception ex2)
                    { throw new ChevronImageLoadException("Unable to load item image from specified URI.", ex2); }
                }

                // If we've got data in our image buffer, try and load it as a Bitmap object.
                if (imgBuffer != null && imgBuffer.Length > 0)
                {
                    try
                    {
                        using (System.IO.MemoryStream strm = new System.IO.MemoryStream(imgBuffer))
                        {
                            Bitmap bmpImg = (Bitmap)Bitmap.FromStream(strm, true, true);
                            colImgParts.Add(bmpImg, imgParts[i].Trim());
                        }
                    }
                    catch (Exception ex)
                    { throw new ChevronImageLoadException("Specified URI did not point to a valid image.", ex); }
                }
            }
            #endregion

            #region Draw the ChevronHeader image
            using (Font txtFont = new Font(sFont, ifontSz, fontStyle))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                if (!bTextWrap)
                    format.FormatFlags = StringFormatFlags.NoWrap;

                using (Bitmap bmp = new Bitmap(iTotalWidth, iTotalHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (Graphics gBmp = Graphics.FromImage(bmp))
                {
                    gBmp.Clear(backClr);

                    gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    gBmp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gBmp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gBmp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gBmp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    ChevronShapeFile shapeFile = null;
                    GraphicsPath path = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(sfUrl))
                        {
                            if (!WebUtil.IsAbsoluteUrl(sfUrl))
                                sfUrl = WebUtil.MakeAbsoluteUrl(sfUrl, context);
                            shapeFile = new ChevronShapeFile(sfUrl);
                        }
                        else
                        {
                            // If the user didn't specify a custom shape file, we're
                            //   going to load the 'default' one from the DLL's
                            //   resources collection.
                            string xmlFileText = global::RainstormStudios.Web.Properties.Resources.DefaultChevron;
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlFileText);
                            System.IO.MemoryStream strm = new System.IO.MemoryStream();
                            strm.Write(buffer, 0, buffer.Length);
                            shapeFile = new ChevronShapeFile(strm);
                        }

                        ChevronShape chevronShape = null;
                        try
                        {
                            if (!string.IsNullOrEmpty(shpNm))
                                chevronShape = shapeFile.Shapes[shpNm];
                            else
                                chevronShape = shapeFile.Shapes[0];
                        }
                        catch (Exception ex)
                        { throw new Exception("Unable to load specified shape: " + ex.Message, ex); }

                        #region Draw the shape's "pre" shape, if defined
                        // Each shape has the ability to draw an "initial" shape before rendering the chevron items.
                        //   This allows for creating more complex chevron header designs.
                        int preShapeOffset = 0;
                        if (chevronShape.StartShape != null)
                        {
                            Rectangle preShapeBounds = chevronShape.StartShape.GetBounds(iPartHeight);
                            GraphicsPath prePath = null;
                            try
                            {
                                prePath = chevronShape.StartShape.GetPath(preShapeBounds, iSpc);
                                using (Pen preShpPen = new Pen(bdrColor))
                                    gBmp.DrawPath(preShpPen, prePath);
                                preShapeOffset = preShapeBounds.Width + chevronShape.StartShape.GetOffset(preShapeBounds, iSpc);
                            }
                            finally
                            {
                                if (prePath != null)
                                    prePath.Dispose();
                            }
                        }
                        #endregion

                        #region Draw Each Chevron Item
                        // The item bounds are always the same, since each individual
                        //   chevron item gets "built" in its own bitmap.
                        Rectangle itemBounds = new Rectangle(new Point(0, 0), new Size(iPartWidth, iPartHeight));

                        // Define the area for the text to be drawn into.
                        // Since we're drawing each shape into its own bitmap, the
                        //   dimensions for this never really change.
                        Rectangle shapeTxtBounds = chevronShape.GetTextBounds(itemBounds);
                        PointF txtBoundsLoc = new Point(shapeTxtBounds.Left + iTextPadding, shapeTxtBounds.Top + iTextPadding);
                        SizeF txtBoundsSize = new SizeF(shapeTxtBounds.Width - (iTextPadding * 2), shapeTxtBounds.Height - (iTextPadding * 2));
                        RectangleF textBounds = new RectangleF(txtBoundsLoc, txtBoundsSize);

                        // Define the "clear" color.
                        Color clrColor = Color.FromArgb(0, Color.Transparent);

                        // Since the chevron is rendered in its own bitmap, and the
                        //   offset doesn't count for the shapes themselves, the
                        //   path data will never change between items.
                        path = chevronShape.GetPath(itemBounds, iSpc);

                        // Get the shape file's offset adjustment.  This allows shape
                        //   designers to create shapes that "interlock".
                        int customShapeOffset = chevronShape.GetOffset(itemBounds, iSpc);

                        // Loop through each item.
                        for (int i = 0; i < arParts.Length; i++)
                        {
                            // Determine the colors we're going to use to draw this part.
                            Color myBgClr1 = Color.Empty,
                                myBgClr2 = Color.Empty;
                            int foreItemAlpha = 255;
                            if (i == iActiveStep)
                            { myBgClr1 = actBackClr1; myBgClr2 = actBackClr2; foreItemAlpha = iActvTrans; }
                            else if (i < iActiveStep)
                            { myBgClr1 = compClr1; myBgClr2 = compClr2; foreItemAlpha = iCompTrans; }
                            else
                            { myBgClr1 = backClr1; myBgClr2 = backClr2; foreItemAlpha = iBackTrans; }

                            Bitmap bmpItemImg = null;
                            if (!string.IsNullOrEmpty(imgParts[i]) && colImgParts.ContainsKey(imgParts[i]))
                                bmpItemImg = colImgParts[imgParts[i]];

                            RectangleF itemTxtBounds = textBounds;
                            if (bmpItemImg != null)
                                itemTxtBounds = new RectangleF(new PointF(textBounds.Left + bmpItemImg.Width + imgPadding, textBounds.Top), new SizeF(textBounds.Width - (bmpItemImg.Width + 4), textBounds.Height));

                            // Create this chevron item's bitmap that will be used
                            //   to "build" the item before rending it to the main
                            //   image.
                            using (Bitmap bmpItem = new Bitmap(iPartWidth + 1, iPartHeight + 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                            {
                                using (Graphics gBmpItem = Graphics.FromImage(bmpItem))
                                {
                                    gBmpItem.Clear(clrColor);

                                    gBmpItem.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                                    gBmpItem.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                    gBmpItem.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    gBmpItem.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    gBmpItem.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                                    // Create the gradient brush.
                                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                                                                                new Point(itemBounds.Left, itemBounds.Top),
                                                                                new Point(itemBounds.Left, itemBounds.Bottom),
                                                                                myBgClr1, myBgClr2))
                                        // ...and fill the shape with our gradient.
                                        gBmpItem.FillPath(bgBrush, path);

                                    if (bTxtShadow)
                                    {
                                        // Draw the text shadow.
                                        using (Bitmap bmpTxtShadow = new Bitmap(bmpItem.Width, bmpItem.Height))
                                        {
                                            // We're going to do the shadow on its own bitmap, then composite it
                                            //   onto the item bitmap.  This is because we're going to use a
                                            //   gaussian blur filter, and we don't want to blur the entire
                                            //   item bitmap.
                                            using (Graphics gBmpTxtShadow = Graphics.FromImage(bmpTxtShadow))
                                            {
                                                //gBmpTxtShadow.Clear(Color.White);
                                                gBmpTxtShadow.CompositingMode = CompositingMode.SourceOver;
                                                gBmpTxtShadow.CompositingQuality = CompositingQuality.HighQuality;

                                                using (SolidBrush txtBrush = new SolidBrush(Color.FromArgb(25, Color.Black)))
                                                    gBmpTxtShadow.DrawString(HttpContext.Current.Server.UrlDecode(arParts[i]), txtFont, txtBrush, itemTxtBounds, format);
                                            }

                                            // Get the "blurred" text.
                                            //using (Bitmap bmpTxtShadowBlured = Drawing.BitmapFilter.GausianBlur(bmpTxtShadow))
                                            //{
                                            //    // Now, we're going to "prep" this by manually setting the pixel alpha before we
                                            //    //   composite it to the main bitmap item.
                                            //    //var bitsImg = bmpTxtShadowBlured.LockBits(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                            //    //unsafe
                                            //    //{
                                            //    //    for (int y = 0; y < bmpTxtShadowBlured.Height; y++)
                                            //    //    {
                                            //    //        // Grab a single 'row' of pixels.
                                            //    //        byte* ptrItem = (byte*)bitsImg.Scan0 + y * bitsImg.Stride;
                                            //    //        for (int x = 0; x < bmpTxtShadowBlured.Width; x++)
                                            //    //        {
                                            //    //            // The 4th byte (current pixel + 3) of data is our alpha transparency.
                                            //    //            int pointAlpha = (int)ptrItem[x * 4 + 3];
                                            //    //            if (pointAlpha == 0)
                                            //    //                // If this is already a transparent pixel, there's nothing to
                                            //    //                //   do here.
                                            //    //                continue;
                                            //    //            int newAlpha = (pointAlpha * iTxtShadowOpac) / 255;
                                            //    //            ptrItem[x * 4 + 3] = (byte)Math.Max(0, newAlpha);
                                            //    //        }
                                            //    //    }
                                            //    //}
                                            //    //bmpTxtShadowBlured.UnlockBits(bitsImg);

                                            //    // We're done, draw it to the bitmap item, offset by 2 pixels.
                                            //}
                                            gBmpItem.DrawImageUnscaled(bmpTxtShadow, new Point(0, 1));
                                        }
                                    }

                                    // And then draw the text, using the foreground color.
                                    using (SolidBrush txtBrush = new SolidBrush(foreClr))
                                        gBmpItem.DrawString(HttpContext.Current.Server.UrlDecode(arParts[i]), txtFont, txtBrush, itemTxtBounds, format);

                                    // If there's an image, draw that also.
                                    if (bmpItemImg != null)
                                    {
                                        // First, we need to measure the length of the text.
                                        SizeF txtPartSz = gBmpItem.MeasureString(arParts[i], txtFont, (int)itemTxtBounds.Width, format);
                                        // Now, we're going to find the left edge of the text;
                                        int iTextStart = (int)System.Math.Truncate(((itemTxtBounds.Width / 2) - (txtPartSz.Width / 2)) + itemTxtBounds.Left);
                                        int iBmpStart = iTextStart - bmpItemImg.Width - imgPadding;
                                        // Now, we know exactly where to draw our image.
                                        gBmpItem.DrawImageUnscaled(bmpItemImg, new Point(iBmpStart, ((int)textBounds.Height / 2) - (bmpItemImg.Height / 2)));
                                    }

                                    // Beveling is disabled for the moment, while I work out how to do that
                                    //   with the custom shapes.
                                    // Draw in the beveling/border
                                    //if (bevelImg)
                                    //{
                                    //    using (Pen shadowPen = new Pen(Color.FromArgb(180, Color.Black)))
                                    //        gBmpItem.DrawLines(shadowPen, new Point[] { LB, RB, RC, RT });
                                    //    using (Pen highlightPen = new Pen(Color.FromArgb(180, Color.White)))
                                    //        gBmpItem.DrawLines(highlightPen, new Point[] { LB, LC, LT, RT });
                                    //}
                                    //else 
                                    if (bdrColor != Color.Transparent)
                                    {
                                        using (SolidBrush bdrBrush = new SolidBrush(Color.FromArgb(iBrdrTrns, bdrColor)))
                                        using (Pen bdrPen = new Pen(bdrBrush, iBrdrWidth))
                                            gBmpItem.DrawPath(bdrPen, path);
                                    }
                                }

                                if (foreItemAlpha < 255)
                                {
                                    // If this item is suppposed to be transparent, we need to do it *after* the item is composited
                                    //   with full opacity.  This way, we're applying the alpha evenly across the whole item at
                                    //   once, instead of compositing each layer seperately, which would give us a different
                                    //   blending effect.
                                    Rectangle itemRect = new Rectangle(0, 0, bmpItem.Width, bmpItem.Height);
                                    var bitsImg = bmpItem.LockBits(itemRect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                    unsafe
                                    {
                                        for (int y = 0; y < bmpItem.Height; y++)
                                        {
                                            byte* ptrItem = (byte*)bitsImg.Scan0 + y * bitsImg.Stride;
                                            for (int x = 0; x < bmpItem.Width; x++)
                                            {
                                                int pointAlpha = (int)ptrItem[x * 4 + 3];
                                                int newAlpha = (pointAlpha * foreItemAlpha) / 255;
                                                ptrItem[x * 4 + 3] = (byte)System.Math.Max(0, newAlpha);
                                            }
                                        }
                                    }
                                    bmpItem.UnlockBits(bitsImg);
                                }

                                // We draw each chevron item to its *own* bitmap first,
                                //   so the offset values don't come into play until
                                //   we draw *that* bitmap back to the main one.
                                int itemOffsetX = (i * ((iPartWidth + iSpc) + customShapeOffset)) + preShapeOffset;
                                Point itemOffsetPoint = new Point(itemOffsetX, 0);

                                // Draw the image item to the main Bitmap.
                                gBmp.DrawImageUnscaled(bmpItem, itemOffsetPoint);
                            }
                        }
                        #endregion

                        #region Draw the shape's "end" shape, if defined
                        // Each shape has the ability to specify an "end" shape that is rendered only once, after all
                        //   other chevron items have been rendered.  This allows for created more complex chevron
                        //   header designs.
                        if (chevronShape.EndShape != null)
                        {
                            int endShapeOffsetX = (arParts.Length * ((iPartWidth + iSpc) + customShapeOffset)) + preShapeOffset;
                            Rectangle endShapeBounds = chevronShape.EndShape.GetBounds(iPartHeight);
                            GraphicsPath endPath = null;
                            try
                            {
                                endPath = chevronShape.EndShape.GetPath(endShapeBounds, iSpc);
                                using (Pen preShpPen = new Pen(bdrColor))
                                    gBmp.DrawPath(preShpPen, endPath);
                            }
                            finally
                            {
                                if (endPath != null)
                                    endPath.Dispose();
                            }
                        }
                        #endregion

                        #region Draw any accent pieces (Reflection, etc)
                        // If an accent is turned on, we're going to draw the accent.
                        if (accentMode != ChevronAccent.None)
                        {
                            // Get "skew" points
                            Point[] refDestPoints = {
                                                        new Point(0, iPartHeight),       // Upper-left point
                                                        new Point(iTotalWidth, iPartHeight),      // Upper-right point
                                                        new Point(iTotalHeight - iPartHeight, iTotalHeight)    // Lower-left point
                                                    };
                            // And the determine the "fade" rectangle.
                            Rectangle fadeRect = new Rectangle(0, iPartHeight + 1, iTotalWidth, iTotalHeight - iPartHeight - 1);

                            // The first thing we're going to do is create a "mask". This is basically, just like the reflection, only
                            //   it will be grayscale color, for the sole purpose of determining alpha in the final image.
                            // Using this method gives us the ability to "tweak" the final mask more-so than just a fade from top to
                            //   bottom.  We will make the edges between items fade out slightly, also, to acheive a much higher
                            //   quality effect.
                            Rectangle maskRect = new Rectangle(0, 0, iTotalWidth, iPartHeight);
                            using (Bitmap bmpMask = new Bitmap(iTotalWidth, iPartHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                            {
                                // Do a "Clear" and make sure the mask is fully black before we start.
                                using (Graphics gBmpMask = Graphics.FromImage(bmpMask))
                                    gBmpMask.Clear(Color.Black);

                                // The first step is to draw the "skewed" reflection. This will help us determine the mask shape.
                                //   If the accent mode is reflection, this is what we'll end up painting back to the main image.
                                using (Bitmap bmpRef = new Bitmap(iTotalWidth, iPartHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                                {
                                    using (Graphics gBmpRef = Graphics.FromImage(bmpRef))
                                        gBmpRef.DrawImageUnscaledAndClipped(bmp, new Rectangle(0, 0, iTotalWidth, iPartHeight));
                                    bmpRef.RotateFlip(RotateFlipType.RotateNoneFlipY);

                                    // Now we've got our skewed image, time to determine
                                    //   the alpha mask.
                                    {
                                        var bitsMask = bmpMask.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                                        var bitsInput = bmpRef.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                        unsafe
                                        {
                                            // Yes, we are about to modify the image data at a binary
                                            //   level.  This way is unsafe (not managed) but it
                                            //   is also crazy fast, since we're editing the values
                                            //   directly in memory.
                                            for (int y = 0; y < maskRect.Height; y++)
                                            {
                                                // Capture a single "scanline" of pixels from the bitmaps.
                                                byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;    // <-- this is technically the part that makes this operation "unsafe" since we're creating direct memory pointer arrays.
                                                byte* ptrInput = (byte*)bitsInput.Scan0 + y * bitsInput.Stride;

                                                // We want to lower the alpha level from 255 to 0 over the course
                                                //   of the height of the accent.
                                                var alphaScan = ((255 * y) / maskRect.Height);

                                                // Loop through the pixels on this scan row and build the mask.
                                                for (int x = 0; x < maskRect.Width; x++)
                                                {
                                                    // Determine the alpha level from the
                                                    //   skewed image.
                                                    var alpha = (int)ptrInput[x * 4 + 3];  // Bit 4 (idx + 3) of the source image is the "alpha" (RGBA format).

                                                    if (alpha == 0)
                                                        // If this spot is already transparent,
                                                        //   there's nothing to be done here.
                                                        continue;

                                                    // Lower the value of the mask level by our
                                                    //   determined height value.
                                                    alpha -= alphaScan;

                                                    // TODO:: My math is wrong and creating "artifacts" at the left edge
                                                    //   of the accent.
                                                    // Now, we're going to fade the "edges" of our accent slightly to
                                                    //   simulate "light leak".  The fade will increase towards the bottom
                                                    //   of the accent.
                                                    //if (x < 20)
                                                    //    alpha -= (x - (maskRect.Height - y));
                                                    //else if (x > (maskRect.Width - 2))
                                                    //    alpha -= ((maskRect.Width - x) - (maskRect.Height - y));

                                                    // Make sure our alpha value doesn't go over 255, or under 0;
                                                    alpha = System.Math.Min(alpha, 255);
                                                    alpha = System.Math.Max(alpha, 0);

                                                    // Now we're going to write our alpha value as a grayscale point in
                                                    //   the mask image.
                                                    ptrMask[x * 4] =
                                                        ptrMask[x * 4 + 1] =
                                                        ptrMask[x * 4 + 2] = (byte)(alpha * ((float)iAccOpacity / 100.0f));
                                                }
                                            }
                                        }
                                        bmpMask.UnlockBits(bitsMask);
                                        bmpRef.UnlockBits(bitsInput);
                                    }

                                    // Now, we have a gray scale mask.  Build up the
                                    //   actual effect image.
                                    if (accentMode == ChevronAccent.Reflection)
                                    {
                                        // We've already created the initial reflection
                                        //   image, so now we just have to apply the mask
                                        //   to it's alpha values.
                                        {
                                            var bitsMask = bmpMask.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                                            var bitsRef = bmpRef.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                            unsafe
                                            {
                                                for (int y = 0; y < maskRect.Height; y++)
                                                {
                                                    byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
                                                    byte* ptrRef = (byte*)bitsRef.Scan0 + y * bitsRef.Stride;
                                                    for (int x = 0; x < maskRect.Width; x++)
                                                    {
                                                        // Remember, we're only setting the alpha channel, and it is a direct copy
                                                        //   of the byte value stored in the corresponding mask pixel
                                                        ptrRef[x * 4 + 3] = ptrMask[x * 4];
                                                    }
                                                }
                                            }
                                            bmpMask.UnlockBits(bitsMask);
                                            bmpRef.UnlockBits(bitsRef);
                                        }

                                        // We're done.  Draw the reflection back to
                                        //   the main Bitmap.
                                        gBmp.DrawImage(bmpRef, refDestPoints);
                                    }
                                    else if (accentMode == ChevronAccent.Shadow)
                                    {
                                        // The shadow one is going to be easy, since it will just be a solid color with the mask applied to it.
                                        using (Bitmap bmpShdw = new Bitmap(iTotalWidth, iPartHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                                        {
                                            using (Graphics gBmpShdw = Graphics.FromImage(bmpShdw))
                                                // First, we're just going to fill the
                                                //   entire image with our shadow color.
                                                gBmpShdw.Clear(Color.Gray);

                                            // Now comes more of that fun binary pixel
                                            //   editing.
                                            {
                                                var bitsMask = bmpMask.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                                                var bitsShdw = bmpShdw.LockBits(maskRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                                unsafe
                                                {
                                                    for (int y = 0; y < maskRect.Height; y++)
                                                    {
                                                        byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
                                                        byte* ptrShdw = (byte*)bitsShdw.Scan0 + y * bitsShdw.Stride;
                                                        for (int x = 0; x < maskRect.Width; x++)
                                                        {
                                                            // Now, all we have to do is copy the alpha level
                                                            //   from the mask, just like we did with the reflection.
                                                            ptrShdw[x * 4 + 3] = ptrMask[x * 4];
                                                        }
                                                    }
                                                }
                                                bmpMask.UnlockBits(bitsMask);
                                                bmpShdw.UnlockBits(bitsShdw);
                                            }

                                            // All done.  Just draw the shadow back to the main Bitmap.
                                            gBmp.DrawImage(bmpShdw, refDestPoints);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    finally
                    {
                        if (shapeFile != null)
                            shapeFile.Dispose();
                        if (path != null)
                            path.Dispose();
                    }

                    //DateTime dtEnd = DateTime.Now;
                    //TimeSpan tsDiff = dtEnd.Subtract(dtStart);
                    // 07-03-13 Performance test:    80ms 100ms 110ms  90ms
                    // 07-24-13 Custom Shape Mod:   230ms 237ms 235ms 233ms
                    // 12-10-13 Bezier Shape Mod:   318ms 139ms 157ms 334ms 152ms (initial page request takes much longer than post back)
                    // 12-19-13 Item Image Mod:     243ms 206ms (using 'arrow' shape. times are likely to be longer using a bezier shape)

                    #region Output the final image to the response stream
                    // Convert the image to a PNG, and return the bytes into the response output stream.
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "image/png";
                        byte[] imgBts = ms.ToArray();
                        context.Response.OutputStream.Write(imgBts, 0, imgBts.Length);
                    }
                    #endregion
                }
            }
            #endregion
        }
        #endregion
    }
    public class ChevronQueryStringParseException : Exception
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public string
            Value { get; private set; }
        public string
            ValueName { get; private set; }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronQueryStringParseException(string value, string valueName, string valueType)
            : base(string.Format("Invalid Chevron image url parse failure: {0} value of \"{2}\" is not a valid {1}.", valueName, valueType, value))
        {
            this.Value = value;
            this.ValueName = valueName;
        }
        #endregion
    }
    public class ChevronImageLoadException : Exception
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronImageLoadException()
            : base()
        { }
        public ChevronImageLoadException(string message)
            : base(message)
        { }
        public ChevronImageLoadException(string message, Exception innerException)
            : base(message, innerException)
        { }
        #endregion
    }
    public class ChevronShapeFileParseException : Exception
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronShapeFileParseException(string message)
            : base(message)
        { }
        public ChevronShapeFileParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
        #endregion
    }
    public class ChevronShapeFileLoadException : Exception
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ChevronShapeFileLoadException(string message)
            : base(message)
        { }
        public ChevronShapeFileLoadException(string message, Exception innerException)
            : base(message, innerException)
        { }
        #endregion
    }
}
