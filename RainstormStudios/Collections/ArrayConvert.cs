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
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace RainstormStudios.Collections
{
    /// <summary>
    /// Provides static methods for working with arrays.
    /// </summary>
    [Author("Unfried, Michael")]
    public abstract class ArrayConvert
    {
        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Concatenates the result of each of the Array element's "ToString()" method into a single string value.
        /// </summary>
        /// <param name="value">An Array of values.</param>
        /// <returns>A string value.</returns>
        public static string ConcatArray(Array value)
        { return ArrayConvert.ConcatArray(value, string.Empty); }
        /// <summary>
        /// Concatenates the result of each of the Array element's "ToString()" method, seperated by the designating string, into a single string value.
        /// </summary>
        /// <param name="value">An Array of values.</param>
        /// <param name="sep">A string value to place in between the string representation of each of the Array's elements.</param>
        /// <returns>A string value.</returns>
        public static string ConcatArray(Array value, string sep)
        {
            string retVal = "";
            for (int i = 0; i < value.Length; i++)
                retVal += value.GetValue(i).ToString() + sep;
            return (
                (string.IsNullOrEmpty(sep))
                    ? retVal
                    : retVal.Substring(0, retVal.Length - sep.Length));
        }
        /// <summary>
        /// Converts a string value into an array of byte values, using each character's byte value as an element in the resulting Array.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>An Array of byte values.</returns>
        public static byte[] ToBytes(string value)
        { return Array.ConvertAll(value.ToCharArray(), new Converter<char, byte>(Convert.ToByte)); }
        /// <summary>
        /// Converts an array of System.Byte values into their corresponding character values and concatentes them into a string.
        /// </summary>
        /// <param name="value">An Array of byte values to be converted.</param>
        /// <returns>A string value.</returns>
        public static string ToString(byte[] value)
        { return ArrayConvert.ConcatArray(Array.ConvertAll(value, new Converter<byte, char>(Convert.ToChar))); }
        /// <summary>
        /// Sorts the elements of an array using a basic 'BubbleSort' algorythm. This method defaults sort direction to ascending.
        /// </summary>
        /// <param name="values">The array of values to be sorted.  This array must be passed with the 'ref' argument prefix and the contents will be directly modified.</param>
        /// <returns>A System.Boolean value indicating true if the array was successfully sorted. Otherwise, false.</returns>
        public static bool BubbleSort(ref Array values)
        { return ArrayConvert.BubbleSort(ref values, SortDirection.Ascending); }
        /// <summary>
        /// Sorts the elements of an array using a basic 'BubbleSort' algorythm in the specified sort direction.
        /// </summary>
        /// <param name="values">The array of values to be sorted.  This array must be passed with the 'ref' argument prefix and the contents will be directly modified.</param>
        /// <param name="dir">A value of type RainstormStudios.SortDirection indicating in which direction the values should be sorted.</param>
        /// <returns>A System.Boolean value indicating true if the array was successfully sorted. Otherwise, false.</returns>
        public static bool BubbleSort(ref Array values, SortDirection dir)
        {
            try
            {
                Array.Sort(values);
                if (dir == SortDirection.Descending)
                    Array.Reverse(values);
            }
            catch(Exception ex)
            { throw new Exception("Unable to sort array.", ex); }
            finally
            { }
            return true;
        }
        /// <summary>
        /// Returns the element with the smallest value from an Array object, or null if the values could not be compared.
        /// </summary>
        /// <param name="values">An Array of values.</param>
        /// <returns>An object value.</returns>
        public static object MinValue(Array values)
        {
            if (BubbleSort(ref values))
                return values.GetValue(0);
            else
                return null;
        }
        /// <summary>
        /// Returns the element with the largest value from an Array object, or null if the values could not be compared.
        /// </summary>
        /// <param name="values">An Array of values.</param>
        /// <returns>An object value.</returns>
        public static object MaxValue(Array values)
        {
            if (BubbleSort(ref values))
                return values.GetValue(values.Length - 1);
            else
                return null;
        }
        /// <summary>
        /// Converts an Array object into an Array of type object[].  Useful for methods which accept only Array's whose elements are type 'object'.
        /// </summary>
        /// <param name="values">An Array of values.</param>
        /// <returns>An Array of type 'object[]'.</returns>
        public static object[] GetObjectArray(Array values)
        {
            object[] retVal = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
                retVal[i] = values.GetValue(i);
            return retVal;
        }
        #endregion
    }
}
