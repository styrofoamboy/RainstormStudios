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
using System.Collections.Generic;
using System.Text;
using RainstormStudios.Collections;

namespace RainstormStudios
{
    public class rsDateTime
    {
        #region Fields
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// Defines the default format for the 'Now' property.
        /// </summary>
        public static string
            CurrentTimeFormat = "HH:mm:ss";
        /// <summary>
        /// Defines the default format for the 'TimeStamp' property.
        /// </summary>
        public static string
            TimeStampFormat = "yyyyMMddHHmmss";
        #endregion

        #region Public Properties
        //***************************************************************************
        // Static Properties
        // 
        /// <summary>
        /// Returns the current time in HH:mm:ss format.
        /// </summary>
        public static string Now
        { get { return DateTime.Now.ToString(rsDateTime.CurrentTimeFormat); } }
        /// <summary>
        /// Returns the current date/time in a format suitable for use as a timestamp (yyyyMMddHHmmss).
        /// </summary>
        public static string TimeStamp
        { get { return DateTime.Now.ToString(rsDateTime.TimeStampFormat); } }
        #endregion

        #region Public Static Methods
        //***************************************************************************
        // Public Static Methods
        // 
        /// <summary>
        /// Appends the current date/time (in 'TimeStamp' format) to the given file name.
        /// </summary>
        /// <param name="FileName">A string value containing the file name to append a time stamp to.</param>
        /// <returns></returns>
        public static string AppendTimeStampToFileName(string FileName)
        {
            try
            {
                FileInfo fi = new FileInfo(FileName);
                return fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + "_" + rsDateTime.TimeStamp + fi.Extension;
            }
            catch (Exception ex)
            { throw new Exception("Unable to append timestamp to file name.", ex); }
        }
        /// <summary>
        /// Creates a DateTime object from a TimeSpan object, using today as the DateTime's base value.
        /// </summary>
        /// <param name="TimeOfDay">A TimeSpan object containing the time to populate into the new DateTime object.</param>
        /// <returns>A DateTime object set with the provided TimeSpan's time and today's date.</returns>
        public static DateTime GetDateTimeFromTimeSpan(TimeSpan TimeOfDay)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, TimeOfDay.Hours, TimeOfDay.Minutes, TimeOfDay.Seconds);
        }
        public static byte[] GetBinary()
        { return rsDateTime.GetBinary(DateTime.Now); }
        public static byte[] GetBinary(DateTime value)
        {
            ByteCollection bytes = new ByteCollection();
            bytes.AddRange(Hex.GetBytes(value.ToString("yyyy")));
            bytes.AddRange(Hex.GetBytes(value.ToString("MM")));
            bytes.AddRange(Hex.GetBytes(value.ToString("dd")));
            bytes.AddRange(Hex.GetBytes(value.ToString("HH")));
            bytes.AddRange(Hex.GetBytes(value.ToString("mm")));
            bytes.AddRange(Hex.GetBytes(value.ToString("ss")));
            return bytes.ToArray();
        }
        #endregion
    }
}
