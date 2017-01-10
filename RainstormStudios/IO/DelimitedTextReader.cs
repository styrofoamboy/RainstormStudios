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
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.IO
{
    public class DelimitedTextReader : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
            _rowDelim,
            _colDelim,
            _txtQual,
            _hdrQual;
        private Stream
            _strm;
        private StreamReader
            _sr;
        private Encoding
            _enc;
        private bool
            _disposed = false;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public long StreamPosition
        { get { return this._strm.Position; } }
        public long StreamLength
        { get { return this._strm.Length; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private DelimitedTextReader(Stream stream, Encoding enc)
        {
            this._strm = stream;
            this._enc = enc;
            this._sr = new StreamReader(this._strm, this._enc);
        }
        public DelimitedTextReader(string fileName, string colDelimiter, Encoding encoding)
            : this(fileName, "\r\n", colDelimiter, string.Empty, encoding)
        { }
        public DelimitedTextReader(string fileName, string colDelimiter, string textQualifier)
            : this(fileName, "\r\n", colDelimiter, textQualifier, Encoding.Default)
        { }
        public DelimitedTextReader(string fileName, string rowDelimiter, string colDelimiter, string textQualifier, Encoding encoding)
            : this(new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read), rowDelimiter, colDelimiter, textQualifier, encoding)
        { }
        public DelimitedTextReader(Stream stream, string colDelimiter, Encoding encoding)
            : this(stream, "\r\n", colDelimiter, string.Empty, encoding)
        { }
        public DelimitedTextReader(Stream stream, string colDelimiter, string textQualifier)
            : this(stream, "\r\n", colDelimiter, textQualifier, Encoding.Default)
        { }
        public DelimitedTextReader(Stream stream, string rowDelimiter, string colDelimiter, string textQualifier, Encoding encoding)
            : this(stream, encoding)
        {
            this._rowDelim = rowDelimiter;
            this._colDelim = colDelimiter;
            this._txtQual = textQualifier;
        }
        //***************************************************************************
        // Deconstructor
        // 
        ~DelimitedTextReader()
        {
            this.Dispose(false);
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public string ReadField()
        {
            bool eof;
            return this.ReadField(out eof);
        }
        public string[] ReadRow()
        {
            List<string> col = new List<string>();
            while (!this._sr.EndOfStream && this._sr.BaseStream.Position < this._sr.BaseStream.Length - this._rowDelim.Length)
            {
                bool endOfRecord;
                col.Add(this.ReadField(out endOfRecord));
                if (endOfRecord)
                    break;
            }
            return col.ToArray();
        }
        public DataTable ReadFile()
        {
            return null;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private string ReadField(out bool endOfRecord)
        {
            bool inQual = false;
            string bfr = "";

            endOfRecord = false;
            while (!this._sr.EndOfStream)
            {
                bfr += char.ConvertFromUtf32(this._sr.Read());

                if (this._txtQual != "<none>" && bfr.Length == this._txtQual.Length && bfr == this._txtQual)
                {
                    inQual = true;
                    bfr = "";
                }
                else if (inQual)
                {
                    if (bfr.Substring(bfr.Length - this._txtQual.Length) == this._txtQual)
                    {
                        inQual = false;
                        bfr = bfr.Substring(0, bfr.Length - this._txtQual.Length);
                    }
                }
                else if (!inQual)
                {
                    if (bfr.Length > this._colDelim.Length && bfr.Substring(bfr.Length - this._colDelim.Length) == this._colDelim)
                    {
                        bfr = bfr.Substring(0, bfr.Length - this._colDelim.Length);
                        break;
                    }
                    else if ((this._rowDelim == "\r\n" || this._rowDelim == "\n\r") && (bfr.EndsWith("\r") || bfr.EndsWith("\n") || bfr.EndsWith("\n\r")))
                    {
                        bfr = bfr.TrimEnd('\r', '\n');
                        endOfRecord = true;
                        break;
                    }
                    else if (bfr.Length > this._rowDelim.Length && bfr.Substring(bfr.Length - this._rowDelim.Length) == this._rowDelim)
                    {
                        bfr = bfr.Substring(0, bfr.Length - this._rowDelim.Length);
                        endOfRecord = true;
                        break;
                    }
                }
            }
            return bfr;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._sr != null)
                {
                    this._sr.DiscardBufferedData();
                    this._sr.Close();
                    this._sr.Dispose();
                }
                if (this._strm != null)
                {
                    this._strm.Close();
                    this._strm.Dispose();
                }
            }
            this._disposed = true;
        }
        #endregion
    }
}
