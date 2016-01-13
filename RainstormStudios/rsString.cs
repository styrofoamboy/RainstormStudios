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
using RainstormStudios.Data;
using RainstormStudios.Collections;

namespace RainstormStudios
{
    /// <summary>
    /// Provides static methods for parsing different types of string data.  This class is sealed.
    /// </summary>
    [Author("Unfried, Michael")]
    public sealed class rsString
    {
        #region Global Variables
        //***************************************************************************
        // Public Fields
        // 
        public const char CharTab = '\t';
        public const char CharLineFeed = '\n';
        public const char CharCarriageReturn = '\r';
        public const char CharQuote = '\"';
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        public static string DeCommentQuery(string[] QueryLines)
        { return rsString.DeCommentQuery(QueryLines, true); }
        /// <summary>
        /// Parses through the given string array, removing all comments and blank space, optionally leaving line breaks.
        /// </summary>
        /// <param name="QueryLines">A string array containing lines of SQL query language.</param>
        /// <param name="singleLine">A bool value indicating whether all line breaks should be removed from the query.</param>
        /// <returns>A string value containing the given query in a single line with all comments removed.</returns>
        public static string DeCommentQuery(string[] QueryLines, bool singleLine)
        {
            if (QueryLines == null || QueryLines.Length < 1)
                return "";

            StringBuilder sbQry = new StringBuilder();
            //string queryString = "";
            try
            {
                foreach (string line in QueryLines)
                    if (line.IndexOf("--") > 0)
                        sbQry.Append((singleLine) ? line.Substring(0, line.IndexOf("--") - 1).Trim() + " " : line.Substring(0, line.IndexOf("--") - 1) + "\r\n");
                        //queryString += ((singleLine) ? line.Substring(0, line.IndexOf("--") - 1).Trim() + " " : line.Substring(0, line.IndexOf("--") - 1) + "\r\n");
                    else if (line.IndexOf("--") < 0)
                        sbQry.Append((singleLine) ? line.Trim() + " " : line + "\r\n");
                        //queryString += ((singleLine) ? line.Trim() + " " : line + "\r\n");

                //while (queryString.Contains("/*"))
                //{
                //    int endBlock = queryString.IndexOf("*/", queryString.IndexOf("/*"));
                //    queryString = queryString.Substring(0, queryString.IndexOf("/*") - 1).Trim() + " " + Convert.ToString((endBlock > 0 && endBlock + 2 < queryString.Length) ? queryString.Substring(endBlock + 2) : "").Trim();
                //}
                while (sbQry.ToString().Contains("/*"))
                {
                    int startBlock = sbQry.ToString().IndexOf("/*");
                    int endBlock = sbQry.ToString().IndexOf("*/", startBlock);
                    sbQry.Remove(startBlock, endBlock);
                }
            }
            catch
            { throw; }
            //return queryString.Trim();
            return sbQry.ToString().Trim();
        }
        /// <summary>
        /// Searches the given connection string an attempts to remove the "Provider=" keyword from it.
        /// </summary>
        /// <param name="ConnectionString">An ADO connection string.</param>
        /// <returns>A string value containing the given connection string without the "Provder=" keyword.</returns>
        public static string RemoveProviderKeyword(string ConnectionString)
        {
            StringBuilder sbNewConnStr = new StringBuilder(ConnectionString.Length);
            //string newConnStr = "";
            try
            {
                string[] keywords = ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string keyword in keywords)
                    if (keyword.ToLower().IndexOf("provider=") < 0)
                        sbNewConnStr.AppendFormat("{0};", keyword.Trim());
                        //newConnStr += keyword.Trim() + ";";
            }
            catch
            { throw; }
            // We don't want the last ";", so we drop the last character on the string.
            //return newConnStr.Substring(0, newConnStr.Length - 1);
            return sbNewConnStr.ToString().TrimEnd(';');
        }
        /// <summary>
        /// Parses a given QueryString to determine the base table name specified in the query.
        /// </summary>
        /// <param name="QueryString">The query to parse.</param>
        /// <returns>A string value containing the base table name from the query with all leading/trailing spaces removed.</returns>
        public static string[] ParseBaseTableName(string QueryString)
        {
            return ParseBaseTableName(QueryString, false);
        }
        /// <summary>
        /// Parses a given QueryString to determine the base table name specified in the query.
        /// </summary>
        /// <param name="QueryString">The query to parse.</param>
        /// <param name="RemoveQualifier">If set to true, all table qualifiers are removed from the returned value.</param>
        /// <returns>A string value containing the base table name from the query with all leading/trailing spaces removed.</returns>
        public static string[] ParseBaseTableName(string QueryString, bool RemoveQualifier)
        {
            StringCollection retVals = new StringCollection();
            if (QueryString.ToUpper().IndexOf("FROM") > -1 && (QueryString.Length - (QueryString.ToUpper().IndexOf("FROM") + 5) > 0))
            {
                // Find the start and end positions of the table names.
                int tblStart = QueryString.ToUpper().IndexOf("FROM") + 5;
                int tblEnd = -1;
                if (QueryString.ToUpper().IndexOf("WHERE") > -1)
                    tblEnd = QueryString.ToUpper().IndexOf("WHERE", tblStart);
                else if (QueryString.ToUpper().IndexOf("HAVING") > -1)
                    tblEnd = QueryString.ToUpper().IndexOf("HAVING", tblStart);
                else
                    tblEnd = QueryString.Length;

                // Get a string of just the table names.
                string tableNames = QueryString.Substring(tblStart, tblEnd - tblStart);

                // Split the tableNames string by coma's.
                string[] tblNames = tableNames.Split(',');

                foreach (string tbl in tblNames)
                {
                    string tblName = tbl;

                    // Strip any alias names
                    if (tblName.IndexOf(' ') > -1)
                        tblName = tblName.Substring(0, tblName.IndexOf(' '));

                    // Remove qualifiers if required.
                    if (RemoveQualifier && tblName.IndexOf('.') > -1)
                        tblName = tblName.Substring(tblName.LastIndexOf('.') + 1);

                    // Add each table name to the collection, stripping off
                    //   any leading/trailing commas & spaces.
                    retVals.Add(tblName.Trim(',', ' '));
                }
            }

            #region Depreciated Code
            // This method did not accomodate queries pulling from multiple tables.
            // 
            //if (QueryString.ToUpper().IndexOf("FROM") > -1 && (QueryString.Length - (QueryString.ToUpper().IndexOf("FROM") + 5) > 0))
            //{
            //    baseTableName = QueryString.Substring(QueryString.ToUpper().IndexOf("FROM") + 5, QueryString.Length - (QueryString.ToUpper().IndexOf("FROM") + 5)).Trim();
            //    if (baseTableName.IndexOf(' ', 0) > -1 && baseTableName.IndexOf(' ', 0) < baseTableName.Length)
            //        baseTableName = baseTableName.Substring(0, baseTableName.IndexOf(' ', 0));
            //    if (RemoveQualifier == true)
            //    {
            //        while (baseTableName.IndexOf('.') > 0) baseTableName = baseTableName.Substring(baseTableName.IndexOf('.') + 1, baseTableName.Length - (baseTableName.IndexOf('.') + 1));
            //    }
            //}
            #endregion
            return retVals.ToArray();
        }
        /// <summary>
        /// Attempts to determine the proper value of the RainstormStudios.AdoProviderType enumeration based on the given connection string.
        /// </summary>
        /// <param name="ConnString">The connnectino string to parse.</param>
        /// <returns>A value from the RainstormStudios.AdoProviderType enumeration.</returns>
        public static AdoProviderType ParseProviderType(string ConnString)
        {
            if (ConnString.ToLower().IndexOf("ole") > -1)
                return AdoProviderType.OleProvider;
            else if (ConnString.ToLower().IndexOf("sql") > -1)
                return AdoProviderType.SqlProvider;
            else if (ConnString.ToLower().IndexOf("db2") > -1)
                return AdoProviderType.DB2Provider;
            else
                return AdoProviderType.OdbcProvider;
        }
        /// <summary>
        /// Parses the given Exception object's stack trace and returns only the line containing the name of the function where the error occured.
        /// </summary>
        /// <param name="value">An Exception object containing the error stack trace to parse.</param>
        /// <returns>A string value containing the parsed error stack trace.</returns>
        public static string GetErrMsg(Exception value)
        {
            return GetErrMsg(value.ToString());
        }
        /// <summary>
        /// Parses the given stack trace and returns only the line containg the name of the function where the error occured.
        /// </summary>
        /// <param name="value">A string value containing an error stack trace.</param>
        /// <returns>A string value containing the parsed error stack trace.</returns>
        public static string GetErrMsg(string value)
        {
            while (value.IndexOf(" at ") > -1)
                value = value.Substring(value.IndexOf(" at ") + 4);
            return " at " + value;
        }
        /// <summary>
        /// Parses an Exception object's stack trace and returns the line number on which the error occured.
        /// </summary>
        /// <param name="value">The Exception object to parse.</param>
        /// <returns>A String value containing the line number where the exception occured.</returns>
        public static string GetErrMsgLine(Exception value)
        {
            return (value != null) ? GetErrMsgLine(value.ToString()) : "Null Exception Passed";
        }
        /// <summary>
        /// Parses the given stack trace and returns the line number on which the error occured.
        /// </summary>
        /// <param name="value">A String value containing a stack trace to parse.</param>
        /// <returns>A String value containing the line number where the exception occured.</returns>
        public static string GetErrMsgLine(string value)
        {
            string errMsg = GetErrMsg(value);
            return errMsg.Substring(errMsg.LastIndexOf(':') + 1);
        }
        public static string WebEscape(string value)
        { return rsString.WebEscape(value, false); }
        public static string WebEscape(string value, bool noSpace)
        {
            string retval = value
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace(">", "&gt;")
                .Replace("<", "&lt;");
            if (noSpace)
                retval = retval.Replace(" ", "&nbsp;");
            return retval;
        }
        public static string WebUnescape(string value)
        {
            return value
                .Replace("&quot;", "\"")
                .Replace("&nbsp;", " ")
                .Replace("&gt;", ">")
                .Replace("&lt;", "<")
                .Replace("&amp;", "&");
        }
        public static string[] BreakAppart(string value, int size)
        { return rsString.BreakAppart(value, size, true); }
        public static string[] BreakAppart(string value, int size, bool keepOfflen)
        {
            StringCollection retVal = new StringCollection();
            for (int i = 0; i < value.Length; i += size)
            {
                if (i + size < value.Length)
                    retVal.Add(value.Substring(i, size), i.ToString().PadLeft(5, '0'));
                else if (keepOfflen)
                    retVal.Add(value.Substring(i), i.ToString().PadLeft(5, '0'));
            }
            return retVal.ToArray();
        }
        public static string RemoveChars(string value, params char[] chars)
        {
            string retVal = value;
            foreach (char chr in chars)
                retVal = retVal.Replace(chr.ToString(), "");
            return retVal;
        }
        public static string GetEnumValueToString(Enum value)
        {
            if (!value.GetType().IsSubclassOf(typeof(UInt64)))
                throw new ArgumentException("Specified enumeration does not inherit from System.UInt64");

            UInt64 mask = (UInt64)Convert.ChangeType(value, typeof(UInt64));
            //string txt = "";
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
                    //if (txt.Length > 0) txt += ",";
                    sbTxt.Append(member);
                    //txt += member;
                }
                mask >>= 1;
            }
            //if (string.IsNullOrEmpty(txt)) txt = value.ToString();
            if (sbTxt.Length < 1)
                sbTxt.Append(value);
            //return txt;
            return sbTxt.ToString();
        }
        /// <summary>
        /// Returns a boolean value indicating true if the argument "val" is equal to any of strings in the "args" array argument. Otherwise, false.
        /// </summary>
        /// <param name="val">A <see cref="T:System.String" /> value to compare.</param>
        /// <param name="args">An array of type <see cref="T:System.String[]" /> containing a series of <see cref="T:System.String"/> values to evaluate.</param>
        /// <returns></returns>
        public static bool EqualToAny(string val, params string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                if (val == args[i])
                    return true;
            return false;
        }
        #endregion
    }
}
