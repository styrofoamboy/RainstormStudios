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
using RainstormStudios.Collections;

namespace RainstormStudios
{
    /// <summary>
    /// Provides a set of static methods for working with hexidecimal values.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class Hex
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        // I know this looks "primitive", but it greatly simplifies the math going
        //   between Base-10 and Base-16, since each character's index in the array
        //   is, effectively, it's Base-10 'value'.
        private static readonly char[]
            hexChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Converts the specified <see cref="T:System.Object"/> value into its binary equivalent. If the object's type is unsupported, an exception of type <see cref="T:System.ArgumentException"/> will be thrown.
        /// </summary>
        /// <param name="val">The value to be converted.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the binary representation of the specified value.</returns>
        public static byte[] GetBinary(object val)
        { return Hex.GetBinary(val, false); }
        /// <summary>
        /// Converts the specified <see cref="T:System.Object"/> value into its binary equivalent. If the object's type is unsupported, an exception of type <see cref="T:System.ArgumentException"/> will be thrown.
        /// </summary>
        /// <param name="val">The value to be converted.</param>
        /// <param name="littleEndian">A value of type <see cref="T:System.Boolean"/> which indicates whether the returned value should be in 'Little Endian' format. This format results in the most significant byte first and is common place when reading or writing to binary file formats.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the binary representation of the specified value.</returns>
        public static byte[] GetBinary(object val, bool littleEndian)
        {
            switch (val.GetType().Name.ToLower())
            {
                case "int16":
                    return Hex.GetBinary((short)val, littleEndian);
                case "int32":
                    return Hex.GetBinary((int)val, littleEndian);
                case "int64":
                    return Hex.GetBinary((long)val, littleEndian);
                case "double":
                    return Hex.GetBinary((long)System.Math.Round((double)val));
                case "float":
                    return Hex.GetBinary((int)System.Math.Round((float)val));
                case "decimal":
                    return Hex.GetBinary((short)System.Math.Round((decimal)val));
                default:
                    throw new ArgumentException(val.GetType().Name + " object is not supported by this class.", "val");
            }
        }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int16"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int16"/> value to be converted.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(short val)
        { return Hex.GetBinary(val, false); }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int16"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int16"/> value to be converted.</param>
        /// <param name="littleEndian">A value of type <see cref="T:System.Boolean"/> which indicates whether the returned value should be in 'Little Endian' format. This format results in the most significant byte first and is common place when reading or writing to binary file formats.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(short val, bool littleEndian)
        {
            byte[] bin = Hex.GetBytes(Hex.ToHex(val).PadLeft(4, '0'));
            if (littleEndian)
                Array.Reverse(bin);
            return bin;
        }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int32"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int32"/> value to be converted.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(int val)
        { return Hex.GetBinary(val, false); }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int32"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int32"/> value to be converted.</param>
        /// <param name="littleEndian">A value of type <see cref="T:System.Boolean"/> which indicates whether the returned value should be in 'Little Endian' format. This format results in the most significant byte first and is common place when reading or writing to binary file formats.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(int val, bool littleEndian)
        {
            byte[] bin = Hex.GetBytes(Hex.ToHex(val).PadLeft(8, '0'));
            if (littleEndian)
                Array.Reverse(bin);
            return bin;
        }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int64"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int64"/> value to be converted.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(long val)
        { return Hex.GetBinary(val, false); }
        /// <summary>
        /// Converts the specified <see cref="T:System.Int64"/> value into its binary equivalent.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int64"/> value to be converted.</param>
        /// <param name="littleEndian">A value of type <see cref="T:System.Boolean"/> which indicates whether the returned value should be in 'Little Endian' format. This format results in the most significant byte first and is common place when reading or writing to binary file formats.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values containing the specified value's binary representation.</returns>
        public static byte[] GetBinary(long val, bool littleEndian)
        {
            byte[] bin = Hex.GetBytes(Hex.ToHex(val).PadLeft(16, '0'));
            if (littleEndian)
                Array.Reverse(bin);
            return bin;
        }
        /// <summary>
        /// Converts the provided <see cref="T:System.Byte"/> array of binary data into its equivalent <see cref="T:System.Int32"/> value.
        /// </summary>
        /// <param name="val">An array of <see cref="T:System.Byte"/> values to be converted.</param>
        /// <returns>An object of type <see cref="T:System.Int32"/> equivalent to the values from the provided <see cref="T:System.Byte"/> array.</returns>
        public static int GetInteger(byte[] val)
        { return Hex.GetInteger(val, false); }
        /// <summary>
        /// Converts the provided <see cref="T:System.Byte"/> array of binary data into its equivalent <see cref="T:System.Int32"/> value.
        /// </summary>
        /// <param name="val">An array of <see cref="T:System.Byte"/> values to be converted.</param>
        /// <param name="littleEndian">A <see cref="T:System.Boolean"/> value indicating if the provided binary data is in 'Little Endian' format. Binary data in this format presents the most significant byte first and must therefore be reversed before being parsed.</param>
        /// <returns>An object of type <see cref="T:System.Int32"/> equivalent to the values from the provided <see cref="T:System.Byte"/> array.</returns>
        public static int GetInteger(byte[] val, bool littleEndian)
        {
            if (littleEndian)
                Array.Reverse(val);
            return Convert.ToInt32(Hex.ToHex(val), 16);
        }
        /// <summary>
        /// Converts the provided <see cref="T:System.Byte"/> array of binary data into its equivalent <see cref="T:System.Int64"/> value.
        /// </summary>
        /// <param name="val">An array of <see cref="T:System.Byte"/> values to be converted.</param>
        /// <returns>An object of type <see cref="T:System.Int64"/> equivalent to the values from the provided <see cref="T:System.Byte"/> array.</returns>
        public static long GetLong(byte[] val)
        { return Hex.GetLong(val, false); }
        /// <summary>
        /// Converts the provided <see cref="T:System.Byte"/> array of binary data into its equivalent <see cref="T:System.Int64"/> value.
        /// </summary>
        /// <param name="val">An array of <see cref="T:System.Byte"/> values to be converted.</param>
        /// <param name="littleEndian">A <see cref="T:System.Boolean"/> value indicating if the provided binary data is in 'Little Endian' format. Binary data in this format presents the most significant byte first and must therefore be reversed before being parsed.</param>
        /// <returns>An object of type <see cref="T:System.Int64"/> equivalent to the values from the provided <see cref="T:System.Byte"/> array.</returns>
        public static long GetLong(byte[] val, bool littleEndian)
        {
            if (littleEndian)
                Array.Reverse(val);
            return Convert.ToInt64(Hex.ToHex(val), 16);
        }
        /// <summary>
        /// Determines if the provided <see cref="T:System.Char"/> value is a valid hexidecimal digit.
        /// </summary>
        /// <param name="c">The <see cref="T:System.Char"/> value to be evaluated.</param>
        /// <returns>A <see cref="T:System.Boolean"/> value indicating 'true' if the specified character is a valid hexidecimal digit. Otherwise, 'false'.</returns>
        public static bool IsHexDigit(char c)
        {
            c = char.ToUpper(c);
            int numChar = Convert.ToInt32(c);
            int numA = Convert.ToInt32('A');
            int num0 = Convert.ToInt32('0');

            if (numChar >= numA && numChar < (numA + 6))
                return true;
            else if (numA >= num0 && numChar < (num0 + 10))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Converts the provided <see cref="T:System.Byte"/> value into its equivalent hexidecimal string.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Byte"/> value to be converted.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the equivalent hexidecimal string.</returns>
        public static string ToHex(byte val)
        { return val.ToString("x2"); }
        /// <summary>
        /// Converts the provided <see cref="T:System.Int32"/> value into its equivalent hexidecimal string.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int32"/> value to be converted.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the equivalent hexidecimal string.</returns>
        public static string ToHex(int val)
        { return Hex.ToHex((long)val); }
        /// <summary>
        /// Converts the provided <see cref="T:System.Int64"/> value into its equivalent hexidecimal string.
        /// </summary>
        /// <param name="val">A <see cref="T:System.Int64"/> value to be converted.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the equivalent hexidecimal string.</returns>
        public static string ToHex(long val)
        {
            if (val < 255)
                return Hex.ToHex(Convert.ToByte(val));

            // Process the integer value into a base-16 number and create
            //   a hexadecimal string to represent the number.
            long rem = 0, num = val;
            string digits = "";

            do
            {
                rem = num % 16;
                if (rem == 0 && num == 0)
                    break;

                digits = hexChar[rem] + digits;
                num = (long)System.Math.Floor((double)(num / 16));
            } while (num > 0);
            return digits;
        }
        /// <summary>
        /// Converts an array of <see cref="T:System.Byte"/> values into their corresponding hexidecimal values.
        /// </summary>
        /// <param name="bytes">An array of <see cref="T:System.Byte"/> values to be converted.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the hexidecimal equivalent of the supplied <see cref="T:System.Byte"/> values.</returns>
        public static string ToHex(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
                hexString += Hex.ToHex(bytes[i]);
            return hexString;
        }
        /// <summary>
        /// Converts the supplied hexadecimal string into bytes based on each pair of character values.
        /// </summary>
        /// <param name="hexString">A string value containing the hexadecimal value to convert.</param>
        /// <returns>An array of type <see cref="T:System.Byte"/>.</returns>
        public static byte[] GetBytes(string hexString)
        {
            int ignore;
            return Hex.GetBytes(hexString, out ignore);
        }
        /// <summary>
        /// Converts the supplied hexadecimal string into <see cref="T:System.Byte"/> values based on each pair of character values.
        /// </summary>
        /// <param name="hexString">A <see cref="T:System.String"/> value containing the hexadecimal value to convert.</param>
        /// <param name="ignored">A <see cref="T:System.Int32"/> value indicating the number of characters in the original hexadecimal value which were ignored.</param>
        /// <returns>An array of type <see cref="T:System.Byte"/>.</returns>
        public static byte[] GetBytes(string hexString, out int ignored)
        {
            int discard = 0;
            StringBuilder sb = new StringBuilder();

            // Remove all non A-F, 0-9 characters, and any whitespace.
            hexString = hexString.Replace(" ", "");
            for (int i = 0; i < hexString.Length; i++)
            {
                char c = hexString[i];
                if (Hex.IsHexDigit(c))
                    sb.Append(c);
                else
                    discard += 1;
            }
            ignored = discard;

            string newString = sb.ToString();

            // Make sure the length of the string we're parsing is a
            //   multiple of 2. If it's not, add a leading '0'.
            if (newString.Length % 2 != 0)
                newString = newString.PadLeft((int)System.Math.Ceiling((double)(newString.Length / 2)), '0');

            return Array.ConvertAll<string, byte>(newString.BreakApart(2), new Converter<string, byte>(Hex.ToByte));
        }
        /// <summary>
        /// Converts a one or two-digit hexadecimal value into its corresponding <see cref="T:System.Byte"/> value.
        /// </summary>
        /// <param name="hexVal">A <see cref="T:System.String"/> value containing the hexadecimal digits to convert.</param>
        /// <returns>The hexadecimal value's corresponding <see cref="T:System.Byte"/> value.</returns>
        public static byte ToByte(string hexVal)
        {
            if (hexVal.Length > 2 || hexVal.Length <= 0)
                throw new ArgumentException("Hexidecimal value must be 1 or 2 characters in length.");

            return Byte.Parse(hexVal, System.Globalization.NumberStyles.HexNumber);
        }
        /// <summary>
        /// Converts the supplied hexadecimal string into an <see cref="T:System.Int32"/> value.
        /// </summary>
        /// <param name="hexValue">A <see cref="T:System.String"/> value containing the hexadecimal value to convert.</param>
        /// <returns>A <see cref="T:System.Int32"/> value equivalent to the provided hexidecimal value.</returns>
        public static Int32 ToInt32(string hexValue)
        {
            return Hex.GetInteger(Hex.GetBytes(hexValue));
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> value representing the web-compatible color equivalent to the supplied <see cref="T:System.Drawing.Color"/> object.
        /// </summary>
        /// <param name="clr">A <see cref="T:System.Drawing.Color"/> object containing the color value to be represented.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the supplied color represented by either color's name or a hexadeicmal RGB value equivalant if the color is not named.</returns>
        public static string GetWebColor(System.Drawing.Color clr)
        {
            if (clr.IsNamedColor)
                return clr.Name;
            else
                return "#" + Hex.ToHex(clr.R) + Hex.ToHex(clr.G) + Hex.ToHex(clr.B);
        }
        /// <summary>
        /// Converts the specified web color (named or hex value) into a <see cref="System.Drawing.Color"/> value.
        /// </summary>
        /// <param name="webColorString"></param>
        /// <returns>A value of type <see cref="T:System.Drawing.Color"/> matching the provided web color.</returns>
        public static System.Drawing.Color GetSystemColor(string webColorString)
        {
            // First, try and determine if the user passed the name of a known color.
            StringCollection strCol = new StringCollection(Enum.GetNames(typeof(System.Drawing.KnownColor)));
            // If so, create a new Color object using the known color name.
            if (strCol.Contains(webColorString))
                return System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)Enum.Parse(typeof(System.Drawing.KnownColor), webColorString));

            // If that failed, assume it's a RGB hex value. First we have to make sure it's 6 digits long.
            string parseClr = webColorString.ToLower().TrimStart('#');
            if (parseClr.Length != 6)
                throw new ArgumentException("Value must be a known color name or a valid hexadecimal RGB web color value.", "webColorString");
            // Break the string value up and pass it to the Hex.GetSystemColor(string, string, string) method.
            return Hex.GetSystemColor(parseClr.Substring(0, 2), parseClr.Substring(2, 2), parseClr.Substring(4, 2));
        }
        /// <summary>
        /// Gets a valid <see cref="T:System.Drawing.Color"/> value from the provided hexidecimal RGB values.
        /// </summary>
        /// <param name="hRed">A <see cref="T:System.String"/> value containing a one or two-digit hexidecimal value for the red color component.</param>
        /// <param name="hGreen">A <see cref="T:System.String"/> value containing a one or two-digit hexidecimal value for the green color component.</param>
        /// <param name="hBlue">A <see cref="T:System.String"/> value containing a one or two-digit hexidecimal value for the blue color component.</param>
        /// <returns>A value of type <see cref="T:System.Drawing.Color"/> matching the provided hexidecimal values.</returns>
        public static System.Drawing.Color GetSystemColor(string hRed, string hGreen, string hBlue)
        {
            byte r = Hex.ToByte(hRed),
                g = Hex.ToByte(hGreen),
                b = Hex.ToByte(hBlue);
            return Hex.GetSystemColor(r, g, b);
        }
        /// <summary>
        /// Gets a valid <see cref="T:System.Drawing.Color"/> value from the provided hexidecimal RGB values.
        /// </summary>
        /// <param name="hRed">A <see cref="T:System.Byte"/> value containing the 0-255 value for the red color component.</param>
        /// <param name="hGreen">A <see cref="T:System.Byte"/> value containing the 0-255 value for the green color component.</param>
        /// <param name="hBlue">A <see cref="T:System.Byte"/> value containing the 0-255 value for the blue color component.</param>
        /// <returns>A value of type <see cref="T:System.Drawing.Color"/> matching the provided hexidecimal values.</returns>
        public static System.Drawing.Color GetSystemColor(byte r, byte g, byte b)
        {
            return System.Drawing.Color.FromArgb(Convert.ToInt32(r),
                                                    Convert.ToInt32(g),
                                                    Convert.ToInt32(b));
        }
        #endregion
    }
}
