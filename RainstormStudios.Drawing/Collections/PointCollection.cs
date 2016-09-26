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
using System.Text;
using System.Drawing;

namespace RainstormStudios.Collections
{
    public class PointCollection : ObjectCollectionBase<Point>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PointCollection()
            : base()
        { }
        public PointCollection(Point[] values)
            : this(values, new string[0])
        { }
        public PointCollection(Point[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Point value)
        { return base.Add(value, ""); }
        public void Add(Point value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Point value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, Point value, string key)
        { base.Insert(index, value, key); }
        public new Point[] ToArray()
        { return ToArray(0, this.List.Count); }
        public new Point[] ToArray(int offset, int length)
        { return base.ToArray(offset, length); }
        #endregion
    }
}
