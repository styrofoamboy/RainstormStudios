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

namespace RainstormStudios.IO
{
    public class CircularBuffer<T>
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private T[]
            _buffer;
        private int
            _start,
            _end;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int Capacity
        { get { return this._buffer.Length; } }
        public bool IsEmpty
        { get { return this._end == this._start; } }
        public bool IsFull
        { get { return (this._end + 1) % this._buffer.Length == this._start; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public CircularBuffer()
            : this(capacity: 10)
        { }
        public CircularBuffer(int capacity)
        {
            this._buffer = new T[capacity + 1];
            this._start = 0;
            this._end = 0;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public void Write(T value)
        {
            this._buffer[this._end] = value;
            this._end = (this._end + 1) % this._buffer.Length;
            if (this._end == this._start)
                this._start = (this._start + 1) % this._buffer.Length;
        }
        public T Read()
        {
            var result = this._buffer[this._start];
            this._start = (this._start + 1) % this._buffer.Length;
            return result;
        }
        #endregion
    }
}
