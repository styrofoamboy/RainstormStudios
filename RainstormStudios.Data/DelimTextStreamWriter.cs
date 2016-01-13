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
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Data
{
    [Author("Unfried, Michael")]
    public class DelimTextStreamWriter : IDisposable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        protected Stream
            _strm;
        protected bool
            _trimVals,
            _lineStart;
        protected string
            _delim;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public Stream BaseStream
        {
            get { return this._strm; }
        }
        public bool TrimValues
        {
            get { return this._trimVals; }
            set { this._trimVals = value; }
        }
        public string ValueDelimiter
        {
            get { return this._delim; }
            set { this._delim = value; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public DelimTextStreamWriter(string fileName)
            : this(fileName, FileMode.Create)
        { }
        public DelimTextStreamWriter(string fileName, FileMode mode)
            : this(new FileStream(fileName, mode, FileAccess.Write))
        { }
        public DelimTextStreamWriter(Stream s)
        {
            if (!s.CanWrite)
                throw new ArgumentException("You must supply a writable stream.", "s");
            this._strm = s;
            this._lineStart = true;
        }
        ~DelimTextStreamWriter()
        {
            this.Dispose();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Dispose()
        {
            if (this._strm != null)
            {
                this._strm.Flush();
                this._strm.Close();
                this._strm.Dispose();
            }
            this._delim = string.Empty;
        }
        public virtual void Write(string value)
        {
            this.Write(value);
        }
        public virtual void Write(object value)
        {
            // If this isn't the first field in the row, then we need to
            //   write the text delimiter first.
            if (!this._lineStart)
            {
                byte[] delimBuffer = Array.ConvertAll<char, byte>(this._delim.ToCharArray(), new Converter<char, byte>(Convert.ToByte));
                this._strm.Write(delimBuffer, 0, delimBuffer.Length);
            }
            // Load the output buffer with the current field data.
            byte[] buffer = Array.ConvertAll<char, byte>(value.ToString().ToCharArray(), new Converter<char, byte>(Convert.ToByte));
            this._strm.Write(buffer, 0, buffer.Length);

            // Set the marker indicating we're no longer at the beginning of a line.
            this._lineStart = false;
        }
        public virtual void WriteLine()
        {
            this.WriteLine(new string[0]);
        }
        public virtual void WriteLine(DataRow dr)
        {
            this.WriteLine(dr.ItemArray);
        }
        public virtual void WriteLine(params string[] values)
        {
            this.WriteLine(values);
        }
        public virtual void WriteLine(Array values)
        {
            // Loop through the output data.
            for (int i = 0; i < values.Length; i++)
                this.Write(values.GetValue(i));

            // Don't forget the CR/LF ;)
            byte[] crlf = Array.ConvertAll<char, byte>(Environment.NewLine.ToCharArray(), new Converter<char, byte>(Convert.ToByte));
            this._strm.Write(crlf, 0, crlf.Length);

            // Reset the marker that we're at the beginning of a new line.
            this._lineStart = true;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        #endregion
    }
}
