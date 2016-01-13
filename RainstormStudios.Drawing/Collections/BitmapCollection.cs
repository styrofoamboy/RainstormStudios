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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Collections
{
    [Author("Unfried, Michael")]
    public class BitmapCollection : ObjectCollectionBase<Bitmap>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public BitmapCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string Add(Bitmap value)
        { return base.Add(value, ""); }
        public void Add(Bitmap value, string key)
        { base.Add(value, key); }
        public string Insert(int index, Bitmap value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, Bitmap value, string key)
        { base.Insert(index, value, key); }
        public void Remove(Bitmap value)
        { base.RemoveAt(this.IndexOf(value)); }
        public new Bitmap[] ToArray()
        { return this.ToArray(0, this.List.Count); }
        public new Bitmap[] ToArray(int offset, int length)
        { return base.ToArray(offset, length); }
        #endregion
    }
}
