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
using System.Text;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public class FixedWidthStreamWriter
    {
        #region Sub-Classes
        //***************************************************************************
        // Nested Classes
        // 
        [Author("Unfried, Michael")]
        public class FixedWidthColumn
        {
            #region Declarations
            //=============================================================
            // Private Fields
            // 
            private int
                _w;
            private TextAlignment
                _align;
            private char
                _blank;
            private string
                _name;
            #endregion

            #region Properties
            //=============================================================
            // Public Properties
            // 
            public string ColumnName
            { get { return this._name; } set { this._name = value; } }
            public int Width
            { get { return this._w; } set { this._w = value; } }
            public TextAlignment Alignment
            { get { return this._align; } set { this._align = value; } }
            public char BlankCharacter
            { get { return this._blank; } set { this._blank = value; } }
            #endregion

            #region Class Constructors
            //=============================================================
            // Class Constructors
            // 
            public FixedWidthColumn(int width)
                : this("", width)
            { }
            public FixedWidthColumn(string name, int width)
                : this(name, width, TextAlignment.Left, ' ')
            { }
            public FixedWidthColumn(int width, TextAlignment align)
                : this("", width, align, ' ')
            { }
            public FixedWidthColumn(string name, int width, TextAlignment align)
                : this(name, width, align, ' ')
            { }
            public FixedWidthColumn(string name, int width, TextAlignment align, char blankChar)
            {
                this._name = name;
                this._w = width;
                this._align = align;
                this._blank = blankChar;
            }
            #endregion
        }
        [Author("Unfried, Michael")]
        public class FixedWidthColumnCollection : ObjectCollectionBase
        {
            #region Properties
            //=============================================================
            // Public Properties
            // 
            public new FixedWidthColumn this[int index]
            {
                get { return (FixedWidthColumn)base[index]; }
                set { base[index] = value; }
            }
            public new FixedWidthColumn this[string key]
            {
                get { return (FixedWidthColumn)base[key]; }
                set { base[key] = value; }
            }
            #endregion

            #region Public Methods
            //=============================================================
            // Public Methods
            // 
            #endregion
        }
        #endregion
    }
}
