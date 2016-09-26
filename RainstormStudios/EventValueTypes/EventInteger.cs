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

namespace RainstormStudios.EventValueTypes
{
    /// <summary>
    /// Provides a container for a basic System.Boolean Type which will trigger an event when the value is changed.
    /// </summary>
    public struct EventInteger
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private int
            _value;
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler ValueChanged;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                if (this.ValueChanged != null)
                    this.ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public EventInteger(int initVal)
            : this(initVal, null)
        { }
        public EventInteger(int initVal, EventHandler del)
        {
            this._value = initVal;
            this.ValueChanged = del;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override string ToString()
        {
            return this.Value.ToString();
        }
        //***************************************************************************
        // Static Methods
        // 
        public static EventInteger Parse(string s)
        { return EventInteger.Parse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture.NumberFormat); }
        public static EventInteger Parse(string s, System.Globalization.NumberStyles style)
        { return EventInteger.Parse(s, style, System.Globalization.CultureInfo.CurrentCulture.NumberFormat); }
        public static EventInteger Parse(string s, IFormatProvider provider)
        { return EventInteger.Parse(s, System.Globalization.NumberStyles.Integer, provider); }
        public static EventInteger Parse(string s, System.Globalization.NumberStyles style, IFormatProvider provider)
        {
            try
            { return new EventInteger(int.Parse(s, style, provider)); }
            catch
            { throw; }
        }
        public static bool TryParse(string s, out EventInteger result)
        { return EventInteger.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out result); }
        public static bool TryParse(string s,System.Globalization.NumberStyles style,IFormatProvider provider , out EventInteger result)
        {
            int i;
            bool retVal = int.TryParse(s, style, provider, out i);
            result = new EventInteger(i);
            return retVal;
        }
        #endregion

        #region Operators
        //***************************************************************************
        // Implicit Conversion
        // 
        public static implicit operator System.Int32(RainstormStudios.EventValueTypes.EventInteger b)
        {
            int newInt = b._value;
            return newInt;
        }
        //public static implicit operator RainstormStudios.EventValueTypes.EventInteger(System.Int32 b)
        //{
        //    EventInteger newInt = new EventInteger();
        //    newInt._value = b;
        //    return newInt;
        //}
        #endregion
    }
}
