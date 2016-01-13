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
    [Author("Unfried, Michael")]
    public struct EventBoolean
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private bool
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
        public bool Value
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
        public EventBoolean(bool initVal)
            : this(initVal, null)
        { }
        public EventBoolean(bool initVal, EventHandler del)
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
        public static EventBoolean Parse(string s)
        {
            try
            { return new EventBoolean(bool.Parse(s)); }
            catch
            { throw; }
        }
        public static bool TryParse(string s, out EventBoolean result)
        {
            bool b;
            bool retVal = bool.TryParse(s, out b);
            result = new EventBoolean(b);
            return retVal;
        }
        #endregion

        #region Operators
        //***************************************************************************
        // Implicit Conversion
        // 
        public static implicit operator System.Boolean(RainstormStudios.EventValueTypes.EventBoolean b)
        {
            bool newBool = b.Value;
            return newBool;
        }
        //public static implicit operator RainstormStudios.EventValueTypes.EventBoolean(System.Boolean b)
        //{
        //    EventBoolean newBool = new EventBoolean();
        //    newBool._value = b;
        //    return newBool;
        //}
        #endregion
    }
}
