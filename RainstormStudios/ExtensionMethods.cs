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

namespace RainstormStudios
{
    [Author("Michael Unfried")]
    public static class ValueTypeExtensions
    {
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true if any elements of the supplied array match this value.
        /// </summary>
        /// <typeparam name="T">A type which is struct that inherits the <see cref="T:System.IComparable"/> interface.</typeparam>
        /// <param name="matches">An array of values who's elements are of type 'T' to compare against this value.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if this value match any elements in the array. Otherwise, false.</returns>
        public static bool MatchesAny<T>(this T value, params T[] matches) where T : struct, IComparable<T>
        {
            for (int i = 0; i < matches.Length; i++)
                // Stucts do not implicitly implement comparison operators, so we'll
                //   use the generic EqualityComparer class to initiate the "default"
                //   comparison between the two values.
                if (EqualityComparer<T>.Default.Equals(value, matches[i]))
                    return true;

            // If no match was found, return false.
            return false;
        }
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true of all elements of the supplied array match this value.
        /// </summary>
        /// <typeparam name="T">A type which is struct that inherits the <see cref="T:System.IComparable"/> interface.</typeparam>
        /// <param name="matches">An array of values who's elements are of type 'T' to compare against this value.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if this value match any elements in the array. Otherwise, false.</returns>
        public static bool MatchesAll<T>(this T value, params T[] matches) where T : struct,IComparable<T>
        {
            for (int i = 0; i < matches.Length; i++)
                // Structs do not implicitly implement comparison operators, so we'll
                //   use the generic EqualityComparer class to initiate the "default"
                //   comparison betwenn the two values.
                if (!EqualityComparer<T>.Default.Equals(value, matches[i]))
                    return false;

            // If none of the values failed to match, return true.
            return true;
        }
        /// <summary>
        /// Converts a <see cref="T:System.Byte[]"/> array into a hexidecimal string.
        /// </summary>
        /// <returns>A value of type <see cref="T:System.String"/> containing a hexidecimal representation of the supplied <see cref="T:System.Byte[]"/> array.</returns>
        public static string GetHexidecimal(this byte[] binary)
        {
            return Hex.ToHex(binary);
        }
        /// <summary>
        /// Returns the mask bit value of an Enumeration item as a <see cref="T:System.String"/>.
        /// </summary>
        /// <returns>A value of type <see cref="T:System.String"/> containing the <see cref="T:System.Enum"/> item's mask bit value as a string.</returns>
        public static string GetEnumValueAsString(Enum value)
        {
            if (!value.GetType().IsSubclassOf(typeof(UInt64)))
                throw new ArgumentException("Specified enumeration does not inherit from System.UInt64");

            UInt64 mask = (UInt64)Convert.ChangeType(value, typeof(UInt64));
            StringBuilder sbTxt = new StringBuilder();
            for (int bit = 0; bit < 64; ++bit)
            {
                if (mask == 0) break;

                if ((mask & 1) != 0)
                {
                    UInt64 bvalue = 1U << bit;
                    string member = Enum.GetName(value.GetType(), bvalue);
                    if (string.IsNullOrEmpty(member)) member = bvalue.ToString();
                    if (sbTxt.Length > 0)
                        sbTxt.Append(",");
                    sbTxt.Append(member);
                }
                mask >>= 1;
            }

            if (sbTxt.Length < 1)
                sbTxt.Append(value);

            return sbTxt.ToString();
        }
    }
    [Author("Michael Unfried")]
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a boolean value indicating true if the current <see cref="T:System.String"/> is equal to any of the elements in the "vals" array argument. Otherwise, false.
        /// </summary>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool MatchesAny(this string str, params string[] vals)
        { return StringExtensions.MatchesAny(str, false, vals); }
        /// <summary>
        /// Returns a boolean value indicating true if the current <see cref="T:System.String"/> is equal to any of the elements in the "vals" array argument. Otherwise, false.
        /// </summary>
        /// <param name="matchCase">A <see cref="T:System.Boolean"/> value indicating true if the comparison should be case sensitive.  Otherwise, false.</param>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool MatchesAny(this string str, bool matchCase, params string[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                if (!matchCase && str.ToLower() == vals[i].ToLower())
                    return true;
                else if (matchCase && str == vals[i])
                    return true;
            return false;
        }
        /// <summary>
        /// Returns a boolean value indicating true if the current <see cref="T:System.String"/> contains any of the the elements in the "vals" array argument. Otherwise, false.
        /// </summary>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool ContainsAny(this string str, params string[] vals)
        { return StringExtensions.ContainsAny(str, false, vals); }
        /// <summary>
        /// Returns a boolean value indicating true if the current <see cref="T:System.String"/> contains any of the the elements in the "vals" array argument. Otherwise, false.
        /// </summary>
        /// <param name="matchCase">A <see cref="T:System.Boolean"/> value indicating true if the comparison should be case sensitive.  Otherwise, false.</param>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool ContainsAny(this string str, bool matchCase, params string[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                if (!matchCase && str.ToLower().Contains(vals[i].ToLower()))
                    return true;
                else if (matchCase && str.Contains(vals[i]))
                    return true;
            return false;
        }
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true of the current <see cref="T:System.String"/> starts with any of the elements in the <paramref name="vals"/> array argument. Otherwise, false.
        /// </summary>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool StartsWithAny(this string str, params string[] vals)
        { return StringExtensions.StartsWithAny(str, true, vals); }
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true of the current <see cref="T:System.String"/> starts with any of the elements in the <paramref name="vals"/> array argument. Otherwise, false.
        /// </summary>
        /// <param name="matchCase">A <see cref="T:System.Boolean"/> value indicating true if the comparison should be case sensitive.  Otherwise, false.</param>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool StartsWithAny(this string str, bool matchCase, params string[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                if (!matchCase && str.ToLower().StartsWith(vals[i].ToLower()))
                    return true;
                else if (matchCase && str.StartsWith(vals[i]))
                    return true;
            return false;
        }
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true of the current <see cref="T:System.String"/> ends with any of the elements in the <paramref name="vals"/> array argument. Otherwise, false.
        /// </summary>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool EndsWithAny(this string str, params string[] vals)
        { return StringExtensions.EndsWithAny(str, true, vals); }
        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> value indicating true of the current <see cref="T:System.String"/> ends with any of the elements in the <paramref name="vals"/> array argument. Otherwise, false.
        /// </summary>
        /// <param name="matchCase">A <see cref="T:System.Boolean"/> value indicating true if the comparison should be case sensitive.  Otherwise, false.</param>
        /// <param name="vals">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns>A value of type <see cref="T:System.Boolean"/> indicating true if any matches were found. Otherwise, false.</returns>
        public static bool EndsWithAny(this string str, bool matchCase, params string[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                if (!matchCase && str.ToLower().EndsWith(vals[i].ToLower()))
                    return true;
                else if (matchCase && str.EndsWith(vals[i]))
                    return true;
            return false;
        }
        /// <summary>
        /// Breaks this string apart into an array of string, each of the length specified.
        /// </summary>
        /// <param name="size">A value of type <see cref="T:System.Int32"/> indicating the size of each element in the resulting array.</param>
        /// <param name="keepRemaining">A <see cref="T:System.Boolean"/> value indicating true if "remainder" characters should be included in the output array.</param>
        /// <returns>A <see cref="T:System.String[]"/> array containing the piece of the original <see cref="T:System.String"/> broken into chunks of the specified size.</returns>
        public static string[] BreakApart(this string value, int size, bool keepRemaining = false)
        {
            List<string> retVal = new List<string>();

            for (int i = 0; i < value.Length; i += size)
            {
                if (i + size < value.Length)
                    retVal.Add(value.Substring(i, size));
                else if (keepRemaining)
                    retVal.Add(value.Substring(i));
            }
            return retVal.ToArray();
        }
        /// <summary>
        /// Removes the specified characters from this <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="chars">A <see cref="T:System.Char[]"/> array containing the characters to be removed from the <see cref="T:System.String"/>.</param>
        /// <returns>A value of type <see cref="T:System.String"/> containing the original <see cref="T:System.String"/> with the specified characters removed.</returns>
        public static string RemoveChars(this string value, params char[] chars)
        {
            string retVal = value;
            for (int i = 0; i < chars.Length; i++)
                retVal = retVal.Replace(chars[i].ToString(), "");
            return retVal;
        }
        /// <summary>
        /// Parses this string and removes all characters not provided in the <see cref="T:System.Char"/> array.
        /// </summary>
        /// <param name="chars">A <see cref="T:System.Char[]"/> array containing the characters to be kept from the original <see cref="T:System.String"/> value.</param>
        /// <returns>A value of type <see cref="T:System.String"/> containing the original <see cref="T:System.String"/> with all characters not included in the method call parameter removed.</returns>
        public static string KeepOnlyChars(this string value, params char[] chars)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            char[] valChars = value.ToCharArray();
            for (int i = 0; i < valChars.Length; i++)
                if (chars.Contains(valChars[i]))
                    sb.Append(valChars[i]);
            return sb.ToString();
        }
        /// <summary>
        /// Returns a copy of this string with the order of the characters reversed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReverseString(this string value)
        {
            return new string(value.ToCharArray().Reverse().ToArray());
        }
    }
    [Author("Michael Unfried")]
    public static class IntegerExtensions
    {
        /// <summary>
        /// Returns this integer value as a roman numeral.  Values over 3,999 will not be displayed "correctly" with the "dash" above the number indicating multples of 10.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Integer"/> value to display as a roman numeral.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the roman numeral representation of the <see cref="T:System.Integer"/> value.</returns>
        public static string ToRomanNumeral(this int value)
        { return ToRomanNumeral(value, false, false); }
        public static string ToRomanNumeral(this int value, bool unicode, bool lowercase)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Please use a positive integer greater than zero.");

            StringBuilder sb = new StringBuilder();
            int remain = value;
            while (remain > 0)
            {
                if (unicode && remain >= 10000) { sb.Append("\u2182"); remain -= 10000; }
                else if (unicode && remain >= 9000) { sb.Append("\u2180\u2182"); remain -= 9000; }
                else if (unicode && remain >= 5000) { sb.Append("\u2181"); remain -= 5000; }
                else if (unicode && remain >= 4000) { sb.Append("\u2180\u2181"); remain -= 4000; }
                else if (remain >= 1000) { sb.Append(unicode ? lowercase ? "\u217f" : "\u216f" : lowercase ? "m" : "M"); remain -= 1000; }
                else if (remain >= 900) { sb.Append(unicode ? lowercase ? "\u217d\u217f" : "\u216d\u216f" : lowercase ? "cm" : "CM"); remain -= 900; }
                else if (remain >= 500) { sb.Append(unicode ? lowercase ? "\u217e" : "\u216e" : lowercase ? "d" : "D"); remain -= 500; }
                else if (remain >= 400) { sb.Append(unicode ? lowercase ? "\u217d\u217e" : "\u216d\u216e" : lowercase ? "cd" : "CD"); remain -= 400; }
                else if (remain >= 100) { sb.Append(unicode ? lowercase ? "\u217d" : "\u216d" : lowercase ? "c" : "C"); remain -= 100; }
                else if (remain >= 90) { sb.Append(unicode ? lowercase ? "\u2179" : "\u2169" : lowercase ? "xc" : "XC"); remain -= 90; }
                else if (remain >= 50) { sb.Append(unicode ? lowercase ? "\u217c" : "\u216c" : lowercase ? "l" : "L"); remain -= 50; }
                else if (remain >= 40) { sb.Append(unicode ? lowercase ? "\u2179\u217c" : "\u2169\u216c" : lowercase ? "xl" : "XL"); remain -= 40; }
                else if (unicode && remain == 12) { sb.Append(lowercase ? "\u217b" : "\u216b"); remain -= 12; } // XII
                else if (unicode && remain == 11) { sb.Append(lowercase ? "\u217a" : "\u216a"); remain -= 11; } // XI
                else if (remain >= 10) { sb.Append(unicode ? lowercase ? "\u2179" : "\u2169" : lowercase ? "x" : "X"); remain -= 10; }
                else if (remain >= 9) { sb.Append(unicode ? lowercase ? "\u2178" : "\u2168" : lowercase ? "ix" : "IX"); remain -= 9; }
                else if (unicode && remain == 8) { sb.Append(lowercase ? "\u2177" : "\u2167"); remain -= 8; } // VIII
                else if (unicode && remain == 7) { sb.Append(lowercase ? "\u2176" : "\u2166"); remain -= 7; } // VII
                else if (unicode && remain == 6) { sb.Append(lowercase ? "\u2175" : "\u2165"); remain -= 6; } // VI
                else if (remain >= 5) { sb.Append(unicode ? lowercase ? "\u2174" : "\u2164" : lowercase ? "v" : "V"); remain -= 5; }
                else if (remain >= 4) { sb.Append(unicode ? lowercase ? "\u2173" : "\u2163" : lowercase ? "iv" : "IV"); remain -= 4; }
                else if (unicode && remain == 3) { sb.Append(lowercase ? "\u2172" : "\u2162"); remain -= 3; } // III
                else if (unicode && remain == 2) { sb.Append(lowercase ? "\u2171" : "\u2161"); remain -= 2; } // II
                else if (remain >= 1) { sb.Append(unicode ? lowercase ? "\u2170" : "\u2160" : lowercase ? "i" : "I"); remain -= 1; }
                else throw new Exception("Unexpected error."); // <<-- shouldn't be possble to get here, but it ensures that we will never have an infinite loop (in case the computer is on crack that day).
            }

            return sb.ToString();
        }
        public static string ToWrittenNumber(this int value)
        {
            string words = "";
            double intPart;
            double decPart = 0;
            if (value == 0)
                return "zero";
            try
            {
                string[] splitter = value.ToString().Split('.');
                intPart = double.Parse(splitter[0]);
                decPart = double.Parse(splitter[1]);
            }
            catch
            {
                intPart = value;
            }

            words = NumWords(intPart);

            if (decPart > 0)
            {
                if (words != "")
                    words += " and ";
                int counter = decPart.ToString().Length;
                switch (counter)
                {
                    case 1: words += NumWords(decPart) + " tenths"; break;
                    case 2: words += NumWords(decPart) + " hundredths"; break;
                    case 3: words += NumWords(decPart) + " thousandths"; break;
                    case 4: words += NumWords(decPart) + " ten-thousandths"; break;
                    case 5: words += NumWords(decPart) + " hundred-thousandths"; break;
                    case 6: words += NumWords(decPart) + " millionths"; break;
                    case 7: words += NumWords(decPart) + " ten-millionths"; break;
                }
            }
            return words;
        }
        static String NumWords(double n) //converts double to words
        {
            string[] numbersArr = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensArr = new string[] { "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninty" };
            string[] suffixesArr = new string[] { "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion", "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septdecillion", "Octodecillion", "Novemdecillion", "Vigintillion" };
            string words = "";

            bool tens = false;

            if (n < 0)
            {
                words += "negative ";
                n *= -1;
            }

            int power = (suffixesArr.Length + 1) * 3;

            while (power > 3)
            {
                double pow = System.Math.Pow(10, power);
                if (n >= pow)
                {
                    if (n % pow > 0)
                    {
                        words += NumWords(System.Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1] + ", ";
                    }
                    else if (n % pow == 0)
                    {
                        words += NumWords(System.Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1];
                    }
                    n %= pow;
                }
                power -= 3;
            }
            if (n >= 1000)
            {
                if (n % 1000 > 0) words += NumWords(System.Math.Floor(n / 1000)) + " thousand, ";
                else words += NumWords(System.Math.Floor(n / 1000)) + " thousand";
                n %= 1000;
            }
            if (0 <= n && n <= 999)
            {
                if ((int)n / 100 > 0)
                {
                    words += NumWords(System.Math.Floor(n / 100)) + " hundred";
                    n %= 100;
                }
                if ((int)n / 10 > 1)
                {
                    if (words != "")
                        words += " ";
                    words += tensArr[(int)n / 10 - 2];
                    tens = true;
                    n %= 10;
                }

                if (n < 20 && n > 0)
                {
                    if (words != "" && tens == false)
                        words += " ";
                    words += (tens ? "-" + numbersArr[(int)n - 1] : numbersArr[(int)n - 1]);
                    n -= System.Math.Floor(n);
                }
            }

            return words;
        }
    }
    [Author("Michael Unfried")]
    public static class DateTimeExtensions
    {
        public static byte[] GetBinary(this DateTime value)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Hex.GetBytes(value.ToString("yyyy")));
            bytes.AddRange(Hex.GetBytes(value.ToString("MM")));
            bytes.AddRange(Hex.GetBytes(value.ToString("dd")));
            bytes.AddRange(Hex.GetBytes(value.ToString("HH")));
            bytes.AddRange(Hex.GetBytes(value.ToString("mm")));
            bytes.AddRange(Hex.GetBytes(value.ToString("ss")));

            return bytes.ToArray();
        }
        /// <summary>
        /// Gets a <see cref="T:System.String"/> value that can be used as a unique timestamp.
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTime"/> value used to generate the timestamp.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the timestamp.</returns>
        public static string GetTimeStamp(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// Gets a <see cref="T:System.DateTime"/> object from this TimeSpan value, using DateTime.Now for the date component.
        /// </summary>
        /// <param name="value">The <see cref="T:System.TimeSpan"/> instance to be converted to a DateTime instance.</param>
        /// <returns>A <see cref="T:System.DateTime"/> instance containing DateTime.Now as the date component, and the <see cref="T:System.TimeSpan"/> value for the time component.</returns>
        public static DateTime GetDateTime(this TimeSpan value)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, value.Hours, value.Minutes, value.Seconds);
        }
        /// <summary>
        /// Works exactly as the DateTime.ToString(string format) method, but interprets a single additional value of '%s' to add the suffix for the day of the month. (ie: st, nd, rd, th)
        /// </summary>
        /// <param name="value">The <see cref="T:System.DateTime"/> to display.</param>
        /// <param name="format">A <see cref="T:System.String"/> value containing the formating string used to display the date.</param>
        /// <returns>A <see cref="T:System.String"/> value containing the DateTime object in the format defined by the <paramref name="format"/> parameter.</returns>
        public static string ToStringEx(this DateTime value, string format)
        {
            string fmt = format.Replace("%s", string.Format("'{0}'", value.Day.GetNumericSuffix()));
            return value.ToString(fmt);
        }
        /// <summary>
        /// Returns the short date of the this <see cref="T:System.Nullable"/> <see cref="T:System.DateTime"/> object, if it has a value.  Otherwise, the <see cref="T:System.String"/> value "N/A".
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToShortDateString(this DateTime? value)
        { return value.ToShortDateString("N/A"); }
        /// <summary>
        /// Returns the short date of the this <see cref="T:System.Nullable"/> <see cref="T:System.DateTime"/> object, if it has a value.  Otherwise, the <see cref="T:System.String"/> value specified by the parameter <param name="nullString"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToShortDateString(this DateTime? value, string nullString)
        {
            return (value != null && value.HasValue) ? value.Value.ToShortDateString() : nullString;
        }
        /// <summary>
        /// Returns the numeric suffix (st, nd, rd, th) to display numbers as 1st, 2nd, 3rd, 4th, 5th, 6th, etc.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNumericSuffix(this int value)
        {
            string day = value.ToString();

            if (day.EndsWith("1") && !(day.Length == 2 && day.StartsWith("1")))
                return "st";

            else if (day.EndsWith("2"))
                return "nd";

            else if (day.EndsWith("3") && !(day.Length == 2 && day.StartsWith("1")))
                return "rd";

            else
                return "th";
        }
    }
    [Author("Michael Unfried")]
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Parses the <see cref="T:System.Exception"/> object's stack trace and returns only the line containing the name of the function where the error occured.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> value containing the parsed error stack trace.</returns>
        public static string GetErrMsg(this Exception value)
        {
            string exStr = value.ToString();
            while (exStr.IndexOf(" at ") > -1)
                exStr = exStr.Substring(exStr.IndexOf(" at ") + 4);
            return " at " + value;
        }
        /// <summary>
        /// Parses an <see cref="T:System.Exception"/> object's stack trace and returns the line number on which the error occured.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> value containing the line number where the exception occured.</returns>
        public static string GetErrMsgLine(this Exception value)
        {
            string errMsg = value.GetErrMsg();
            return errMsg.Substring(errMsg.LastIndexOf(':') + 1);
        }
    }
    [Author("Unfried, Michael")]
    public static class EnumExtensions
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static readonly Dictionary<Type, Dictionary<object, string>>
            enumToStringDictionary;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static EnumExtensions()
        {
            // Occording the to the .NET spec, private, static, parameterless contructors
            //   should get called once, the first time one of the class's members is
            //   called.

            // Using this dictionary object allows us to "cache" all the previous enum
            //   description text lookups, which lowers the amount of reflection
            //   (and possibly globalization) that has to occur.
            enumToStringDictionary = new Dictionary<Type, Dictionary<object, string>>();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Extension Methods
        // 
        public static string GetDescriptionString(this Enum value)
        {
            // Get the Enum's Type.
            Type enumType = value.GetType();

            // Lookup that type in the Dictionary.
            string dVal = EnumExtensions.enumToStringDictionary[enumType][value];

            if (string.IsNullOrEmpty(dVal))
                // We've never encountered this enum before, so add it to the type collection.
                dVal = ParseEnum(enumType, value);

            // Return the descriptive value.
            return dVal;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        static string ParseEnum(Type enumType, Enum returnVal)
        {
            // Create a new values/string dictionary to store the new enum.
            Dictionary<object, string> enumValues = new Dictionary<object, string>();

            // This will keep track of the requested string, so that we don't have to look it up again.
            string finalStrVal = null;

            // Get all the Enum values.
            Array values = Enum.GetValues(enumType);

            // Iterate through all the Enum values.
            foreach (object val in values)
            {
                // Grab the FieldInfo object for this Enum value.
                System.Reflection.FieldInfo fieldInfo = enumType.GetField(val.ToString());

                // Check to see if this field (enum value) has a "Description" attribute.
                System.ComponentModel.DescriptionAttribute attr = (Attribute.GetCustomAttribute(fieldInfo, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute);

                string valDesc = null;
                if (attr != null)
                {
                    // If we found a description attribute, store it's value in the dictionary.
                    if (attr is LocalizableDescriptionAttribute)
                        // We'll also watch for use of the LocalizableDescriptionAttribute class.
                        valDesc = ((LocalizableDescriptionAttribute)attr).Description;
                    else
                        valDesc = attr.Description;
                }
                else
                {
                    // If we didn't find the description attribute, just store the enum value's "ToString" value.
                    valDesc = val.ToString();
                }
                // Add the new value/string to the dictionary.
                enumValues.Add(val, valDesc);

                // If this value is the one originally requested, remember what the descriptive string was.
                if (val == returnVal)
                    finalStrVal = valDesc;
            }
            // Add the values dictionary to our "master" Enum Types dictionary.
            EnumExtensions.enumToStringDictionary.Add(enumType, enumValues);

            // Return the requested string value.
            return finalStrVal;
        }
        #endregion
    }
}