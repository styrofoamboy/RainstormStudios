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
    [Author("Unfried, Michael")]
    public class PointFCollection : ObjectCollectionBase<PointF>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public PointFCollection()
            : base()
        { }
        public PointFCollection(PointF[] values)
            : this(values, new string[0])
        { }
        public PointFCollection(PointF[] values, string[] keys)
            : base(values, keys)
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(PointF value)
        { return base.Add(value, ""); }
        public void Add(PointF value, string key)
        { base.Add(value, key); }
        public string Insert(int index, PointF value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, PointF value, string key)
        { base.Insert(index, value, key); }
        public new PointF[] ToArray()
        { return this.ToArray(0, this.List.Count); }
        public new PointF[] ToArray(int offset, int length)
        { return base.ToArray(offset, length); }
        #endregion
    }
}
