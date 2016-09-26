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
using System.IO;
using System.Collections;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios.IO
{
    public class FixedWidthStreamWriter : IDisposable
    {
        #region Sub-Classes
        //***************************************************************************
        // Sub-Classes
        // 
        public enum StringAlignment
        {
            Left = 0,
            Center,
            Right
        }
        public class FixedWidthColumn
        {
            #region Declarations
            //-----------------------------------------------------------------------
            // Private Fields
            // 
            int _w;
            StringAlignment _align;
            char _blank;
            string _name;
            #endregion

            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public string ColumnName
            { get { return this._name; } set { this._name = value; } }
            public int Width
            { get { return this._w; } set { this._w = value; } }
            public StringAlignment Alignment
            { get { return this._align; } set { this._align = value; } }
            public char BlankCharacter
            { get { return this._blank; } set { this._blank = value; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public FixedWidthColumn(int width)
                : this("", width)
            { }
            public FixedWidthColumn(string name, int width)
                : this(width, StringAlignment.Left)
            { }
            public FixedWidthColumn(int width, StringAlignment align)
                : this("", width, align)
            { }
            public FixedWidthColumn(string name, int width, StringAlignment align)
                : this(width, align, ' ')
            { }
            public FixedWidthColumn(int width, StringAlignment align, char blankChar)
                : this("", width, align, blankChar)
            { }
            public FixedWidthColumn(string name, int width, StringAlignment align, char blankChar)
            {
                this._name = name;
                this._w = width;
                this._align = align;
                this._blank = blankChar;
            }
            #endregion
        }
        public class FixedWidthColumnCollection : CollectionBase
        {
            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public FixedWidthColumn this[int index]
            { get { return (FixedWidthColumn)List[index]; } set { List[index] = value; } }
            public FixedWidthColumn this[string name]
            { get { return this.FindColumn(name); } set { this.List[List.IndexOf(value)] = value; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public FixedWidthColumnCollection()
            { }
            #endregion

            #region Public Methods
            //-----------------------------------------------------------------------
            // Public Methods
            // 
            public void Add(string name, int width)
            {
                this.Add(new FixedWidthColumn(name, width));
            }
            public void Add(int width, StringAlignment align)
            {
                this.Add(new FixedWidthColumn(width, align));
            }
            public void Add(string name, int width, StringAlignment align)
            {
                this.Add(new FixedWidthColumn(name, width, align));
            }
            public void Add(FixedWidthColumn value)
            {
                if (string.IsNullOrEmpty(value.ColumnName))
                    value.ColumnName = "Column" + Convert.ToString(List.Count + 1).PadLeft(4, '0');
                if (this.IndexOf(value.ColumnName) > -1)
                    throw new ArgumentException("A column with the specified name already exists.", "value.ColumnName");
                List.Add(value);
            }
            public void Remove(FixedWidthColumn value)
            {
                List.Remove(value);
            }
            public bool Contains(FixedWidthColumn value)
            {
                return InnerList.Contains(value);
            }
            public FixedWidthColumn FindColumn(string name)
            {
                foreach (object col in this.List)
                    if (((FixedWidthColumn)col).ColumnName == name)
                        return (FixedWidthColumn)col;
                return null;
            }
            public int IndexOf(string name)
            {
                for (int i = 0; i < List.Count; i++)
                    if (this[i].ColumnName == name)
                        return i;
                return -1;
            }
            #endregion
        }
        public class FixedWidthRow
        {
            #region Declarations
            //-----------------------------------------------------------------------
            // Global Variables
            // 
            FixedWidthColumnCollection _cols;
            StringCollection _vals;
            #endregion

            #region Public Properties
            //-----------------------------------------------------------------------
            // Public Properties
            // 
            public string this[int index]
            { get { return this._vals[index]; } set { this._vals[index] = value; } }
            public string this[string name]
            { get { return this._vals[this._cols.IndexOf(name)]; } set { this._vals[this._cols.IndexOf(name)] = value; } }
            public StringCollection FieldValues
            { get { return this._vals; } }
            #endregion

            #region Class Constructors
            //-----------------------------------------------------------------------
            // Class Constructors
            // 
            public FixedWidthRow()
            {
                this._vals = new StringCollection();
            }
            public FixedWidthRow(params string[] values)
                : this()
            {
                foreach (string val in values)
                    this._vals.Add(val);
            }
            #endregion

            #region Public Methods
            //***********************************************************************
            // Public Methods
            // 
            public void AddField(string value)
            {
                this._vals.Add(value);
            }
            public void InsertField(int ordinal, string value)
            {
                this._vals.Insert(ordinal, value);
            }
            public void RemoveField(int ordinal)
            {
                this._vals.RemoveAt(ordinal);
            }
            public void RemoveField(string value)
            {
                this._vals.Remove(value);
            }
            #endregion
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Global Variables
        // 
        bool _wCols = false;
        FileStream _fs;
        string _fn;
        FixedWidthColumnCollection _cols;
        #endregion

        #region Public Properties
        //***************************************************************************
        // Public Properties
        // 
        public string FileName
        { get { return this.FileName; } }
        public FileStream Stream
        { get { return this._fs; } }
        public long StreamPosition
        { get { return this._fs.Position; } }
        public FixedWidthColumnCollection Columns
        { get { return this._cols; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private FixedWidthStreamWriter()
        {
            this._cols = new FixedWidthColumnCollection();
        }
        public FixedWidthStreamWriter(string filepath, FileMode mode, FileAccess access)
            : this(new FileStream(filepath, mode, access))
        { }
        public FixedWidthStreamWriter(FileStream stream)
            : this()
        {
            this._fs = stream;
        }
        public FixedWidthStreamWriter(FileStream stream, bool writeColHeaders)
            : this(stream)
        {
            this._wCols = writeColHeaders;
        }
        ~FixedWidthStreamWriter()
        {
            if (this._fs != null)
                this._fs.Dispose();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string ParseField(FixedWidthColumn col, string value)
        {
            string fVal = (value.Length <= col.Width) ? value : value.Substring(0, col.Width);
            switch (col.Alignment)
            {
                case StringAlignment.Left:
                    fVal = fVal.PadRight(col.Width, col.BlankCharacter);
                    break;
                case StringAlignment.Center:
                    fVal = fVal.PadLeft((int)System.Math.Floor((double)(col.Width / 2)), col.BlankCharacter).PadRight((int)System.Math.Ceiling((double)(col.Width / 2)), col.BlankCharacter);
                    break;
                case StringAlignment.Right:
                    fVal = fVal.PadLeft(col.Width, col.BlankCharacter);
                    break;
            }
            return fVal;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this._fn = string.Empty;
            this._fs.Dispose();
        }
        public void WriteRow(params string[] values)
        {
            this.WriteRow(new FixedWidthRow(values));
        }
        public void WriteRow(FixedWidthRow row)
        {
            if (this._fs.Position == 0 && this._wCols)
            {
                string colHeader = "";
                foreach (FixedWidthColumn col in this._cols)
                    colHeader += ParseField(col, col.ColumnName);
                byte[] hdr = ArrayConvert.ToBytes(colHeader + "\r\n");
                this._fs.Write(hdr, 0, hdr.Length);
            }

            string wVal = "";
            for (int i = 0; i < row.FieldValues.Count; i++)
            {
                if (i < this._cols.Count)
                    wVal += ParseField(this._cols[i], row[i]);
                else
                    wVal += row[i];
            }
            byte[] b = ArrayConvert.ToBytes(wVal + "\r\n");
            this._fs.Write(b, 0, b.Length);
        }
        #endregion
    }
}
