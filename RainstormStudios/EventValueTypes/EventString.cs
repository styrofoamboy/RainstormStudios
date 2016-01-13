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
    public struct EventString
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private string
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
        public string Value
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
        public EventString(string initVal)
            : this(initVal, null)
        { }
        public EventString(string initVal, EventHandler del)
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
        public object Clone()
        {
            return this.Value.Clone();
        }
        public int CompareTo(object obj)
        { return this.CompareTo(obj.ToString()); }
        public int CompareTo(string strB)
        {
            return this.Value.CompareTo(strB);
        }
        public bool Contains(string value)
        { return this.Value.Contains(value); }
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        { this.Value.CopyTo(sourceIndex, destination, destinationIndex, count); }
        public bool EndsWith(string value)
        { return this.EndsWith(value, true); }
        public bool EndsWith(string value, bool ignoreCase)
        { return this.EndsWith(value, ignoreCase, null); }
        public bool EndsWith(string value, bool ignoreCase, System.Globalization.CultureInfo culture)
        { return this.Value.EndsWith(value, ignoreCase, culture); }
        public CharEnumerator GetEnumerator()
        { return this.Value.GetEnumerator(); }
        public int IndexOf(char value)
        { return this.IndexOf(value, 0, this.Value.Length); }
        public int IndexOf(char value, int startIndex)
        { return this.IndexOf(value, startIndex, this.Value.Length - startIndex); }
        public int IndexOf(char value, int startIndex, int count)
        { return this.Value.IndexOf(value, startIndex, count); }
        public int IndexOf(string value)
        { return this.IndexOf(value, 0, this.Value.Length); }
        public int IndexOf(string value, int startIndex)
        { return this.IndexOf(value, startIndex, this.Value.Length - startIndex); }
        public int IndexOf(string value, int startIndex, int count)
        { return this.IndexOf(value, startIndex, count, StringComparison.Ordinal); }
        public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
        { return this.Value.IndexOf(value, startIndex, count, comparisonType); }
        public int IndexOfAny(params char[] anyOf)
        { return this.IndexOfAny(anyOf, 0, this.Value.Length); }
        public int IndexOfAny(char[] anyOf, int startIndex)
        { return this.IndexOfAny(anyOf, startIndex, this.Value.Length - startIndex); }
        public int IndexOfAny(char[] anyOf, int startIndex, int count)
        { return this.Value.IndexOfAny(anyOf, startIndex, count); }
        public string Insert(int index, string s)
        { return this.Value.Insert(index, s); }
        public bool IsNormalized()
        { return this.IsNormalized(NormalizationForm.FormC); }
        public bool IsNormalized(NormalizationForm normalizationForm)
        { return this.Value.IsNormalized(normalizationForm); }
        public int LastIndexOf(char value)
        { return this.LastIndexOf(value, 0, this.Value.Length); }
        public int LastIndexOf(char value, int startIndex)
        { return this.LastIndexOf(value, startIndex, this.Value.Length - startIndex); }
        public int LastIndexOf(char value, int startIndex, int count)
        { return this.Value.LastIndexOf(value, startIndex, count); }
        public int LastIndexOf(string value)
        { return this.LastIndexOf(value, 0, this.Value.Length); }
        public int LastIndexOf(string value, int startIndex)
        { return this.LastIndexOf(value, startIndex, this.Value.Length - startIndex); }
        public int LastIndexOf(string value, int startIndex, int count)
        { return this.LastIndexOf(value, startIndex, count, StringComparison.Ordinal); }
        public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
        { return this.Value.LastIndexOf(value, startIndex, count, comparisonType); }
        public int LastIndexOfAny(params char[] anyOf)
        { return this.LastIndexOfAny(anyOf, 0, this.Value.Length); }
        public int LastIndexOfAny(char[] anyOf, int startIndex)
        { return this.LastIndexOfAny(anyOf, startIndex, this.Value.Length - startIndex); }
        public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
        { return this.Value.LastIndexOfAny(anyOf, startIndex, count); }
        public string Normalize()
        { return this.Normalize(NormalizationForm.FormC); }
        public string Normalize(NormalizationForm normalizationForm)
        { return this.Value.Normalize(normalizationForm); }
        public string PadLeft(int totalWidth)
        { return this.PadLeft(totalWidth, ' '); }
        public string PadLeft(int totalWidth, char paddingChar)
        { return this.Value.PadLeft(totalWidth, paddingChar); }
        public string PadRight(int totalWidth)
        { return this.PadRight(totalWidth, ' '); }
        public string PadRight(int totalWidth, char paddingChar)
        { return this.Value.PadRight(totalWidth, paddingChar); }
        public string Remove(int startIndex)
        { return this.Remove(startIndex, this.Value.Length - startIndex); }
        public string Remove(int startIndex, int count)
        { return this.Value.Remove(startIndex, count); }
        public string Replace(char oldChar, char newChar)
        { return this.Value.Replace(oldChar, newChar); }
        public string Replace(string oldValue, string newValue)
        { return this.Value.Replace(oldValue, newValue); }
        public string[] Split(params char[] separator)
        { return this.Value.Split(separator); }
        public string[] Split(char[] separator, int count)
        { return this.Split(separator, count, StringSplitOptions.None); }
        public string[] Split(char[] separator, StringSplitOptions options)
        { return this.Value.Split(separator, options); }
        public string[] Split(string[] separator, StringSplitOptions options)
        { return this.Value.Split(separator, options); }
        public string[] Split(char[] separator, int count, StringSplitOptions options)
        { return this.Value.Split(separator, count, options); }
        public string[] Split(string[] separator, int count, StringSplitOptions options)
        { return this.Value.Split(separator, count, options); }
        public bool StartsWith(string value)
        { return this.Value.StartsWith(value); }
        public bool StartsWith(string value, StringComparison comparisonType)
        { return this.Value.StartsWith(value, comparisonType); }
        public bool StartsWith(string value, bool ignoreCase, System.Globalization.CultureInfo culture)
        { return this.Value.StartsWith(value, ignoreCase, culture); }
        public string SubString(int startIndex)
        { return this.SubString(startIndex, this.Value.Length - startIndex); }
        public string SubString(int startIndex, int length)
        { return this.Value.Substring(startIndex, length); }
        public char[] ToCharArray()
        { return this.ToCharArray(0, this.Value.Length); }
        public char[] ToCharArray(int startIndex, int length)
        { return this.Value.ToCharArray(startIndex, length); }
        public string ToLower()
        { return this.ToLower(System.Globalization.CultureInfo.CurrentCulture); }
        public string ToLower(System.Globalization.CultureInfo culture)
        { return this.Value.ToLower(culture); }
        public string ToLowerInvariant()
        { return this.Value.ToLowerInvariant(); }
        public string ToUpper()
        { return this.ToUpper(System.Globalization.CultureInfo.CurrentCulture); }
        public string ToUpper(System.Globalization.CultureInfo culture)
        { return this.Value.ToUpper(culture); }
        public string ToUpperInvariant()
        { return this.Value.ToUpperInvariant(); }
        public string Trim()
        { return this.Value.Trim(); }
        public string Trim(params char[] trimChars)
        { return this.Value.Trim(trimChars); }
        public string TrimEnd(params char[] trimChars)
        { return this.Value.TrimEnd(trimChars); }
        public string TrimStart(params char[] trimChars)
        { return this.Value.TrimStart(trimChars); }
        #endregion

        #region Operators
        //***************************************************************************
        // Implicit Conversion
        // 
        public static implicit operator System.String(RainstormStudios.EventValueTypes.EventString s)
        {
            string newStr = s._value;
            return newStr;
        }
        //public static implicit operator RainstormStudios.EventValueTypes.EventString(System.String s)
        //{
        //    EventString newStr = new EventString();
        //    newStr._value = s;
        //    return newStr;
        //}
        #endregion
    }
}
