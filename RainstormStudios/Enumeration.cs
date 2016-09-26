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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios
{
    /// <summary>
    /// Used as a special-case base class for creating classes that imitate enumerations, in situations where enumerations are not sufficient.
    /// </summary>
    /// <remarks>
    /// The use of this class has been depreciated, in favor of using actual enumerations with a <see cref="T:System.ComponentModel.Description"/>
    /// or <see cref="T:LocalizableDescriptionAttribute"/> attached to the values.
    /// </remarks>
    [Obsolete("The use of this class has been depreciated, in favor of using actual enumerations with LocalizableDescription attributes on the values.")]
    public abstract class Enumeration : IComparable
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private readonly int
            _value;
        private readonly string
            _displayName;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int Value
        {
            get { return this._value; }
        }
        public string DisplayName
        {
            get { return this._displayName; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        protected Enumeration()
        { }
        protected Enumeration(int value, string displayName)
        {
            this._value = value;
            this._displayName = displayName;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public int CompareTo(object other)
        {
            return Value.CompareTo(((Enumeration)other).Value);
        }
        //***************************************************************************
        // Public Overrides
        public override string ToString()
        {
            return DisplayName;
        }
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = this._value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }
        //***************************************************************************
        // Static Methods
        // 
        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                    yield return locatedValue;
            }
        }
        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = System.Math.Abs(firstValue.Value - secondValue.Value);
            return absoluteDifference;
        }
        public static T FromValue<T>(int value) where T : Enumeration, new()
        {
            var matchingItem = parse<T, int>(value, "value", item => item.Value == value);
            return matchingItem;
        }
        public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
        {
            var matchingItem = parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
            return matchingItem;
        }
        private static T parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new ApplicationException(string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T)));

            return matchingItem;
        }
        #endregion
    }
}
