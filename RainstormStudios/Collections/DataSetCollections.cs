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
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Collections
{
    [Author("Unfried, Michael")]
    public class DataSetCollection : ObjectCollectionBase<DataSet>
    {
        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DataSetCollection()
            : base()
        { }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        //
        public string Add(DataSet value)
        { return base.Add(value, ""); }
        public void Add(DataSet value, string key)
        { base.Add(value, key); }
        public string Insert(int index, DataSet value)
        { return base.Insert(index, value, ""); }
        public void Insert(int index, DataSet value, string key)
        { base.Insert(index, value, key); }
        public new DataSet[] ToArray()
        { return this.ToArray(0, this.List.Count); }
        public new DataSet[] ToArray(int offset, int length)
        {
            //return Array.ConvertAll<object, DataSet>(base.ToArray(offset, length), new Converter<object, DataSet>(this.CastValue));
            return base.ToArray(offset, length);
        }
        #endregion
    }
}
