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
using System.Web;
using System.Web.UI;

namespace RainstormStudios.Web
{
    /// <summary>
    /// Provides extension methods for working with strongly-typed values in a .NET <see cref="T:System.Web.Statebag"/> viewstate object.
    /// </summary>
    public static class WebHelper
    {
        #region Declarations
        //***************************************************************************
        // Delegate Definitions
        // 
        public delegate bool
            TryParseHandler<T>(string value, out T result);
        #endregion

        #region Public Methods
        //***************************************************************************
        // Extension Methods
        // 
        public static int GetInteger(this StateBag viewState, string valueName)
        { return WebHelper.GetViewStateIntValue(viewState, valueName); }
        public static int GetInteger(this StateBag viewState, string valueName, int defaultValue)
        { return WebHelper.GetViewStateIntValue(viewState, valueName, defaultValue); }
        public static int GetInteger(this StateBag viewState, string valueName, int defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateIntValue(viewState, valueName, defaultValue, throwExceptionIfNull); }

        public static float GetFloat(this StateBag viewState, string valueName)
        { return WebHelper.GetViewStateFloatValue(viewState, valueName); }
        public static float GetFloat(this StateBag viewState, string valueName, float defaultValue)
        { return WebHelper.GetViewStateFloatValue(viewState, valueName, defaultValue); }
        public static float GetFloat(this StateBag viewState, string valueName, float defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateFloatValue(viewState, valueName, defaultValue, throwExceptionIfNull); }

        public static double GetDouble(this StateBag viewState, string valueName)
        { return WebHelper.GetViewStateDoubleValue(viewState, valueName); }
        public static double GetDouble(this StateBag viewState, string valueName, double defaultValue)
        { return WebHelper.GetViewStateDoubleValue(viewState, valueName, defaultValue); }
        public static double GetDouble(this StateBag viewState, string valueName, double defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateDoubleValue(viewState, valueName, defaultValue, throwExceptionIfNull); }

        public static bool GetBoolean(this StateBag viewState, string valueName)
        { return WebHelper.GetViewStateBoolValue(viewState, valueName); }
        public static bool GetBoolean(this StateBag viewState, string valueName, bool defaultValue)
        { return WebHelper.GetViewStateBoolValue(viewState, valueName, defaultValue); }
        public static bool GetBoolean(this StateBag viewState, string valueName, bool defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateBoolValue(viewState, valueName, defaultValue, throwExceptionIfNull); }

        public static DateTime GetDateTime(this StateBag viewState, string valueName)
        { return WebHelper.GetViewStateDateTimeValue(viewState, valueName); }
        public static DateTime GetDateTime(this StateBag viewState, string valueName, DateTime defaultValue)
        { return WebHelper.GetViewStateDateTimeValue(viewState, valueName, defaultValue); }
        public static DateTime GetDateTime(this StateBag viewState, string valueName, DateTime defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateDateTimeValue(viewState, valueName, defaultValue, throwExceptionIfNull); }

        public static T GetValue<T>(this StateBag viewState, string valueName, T defaultValue, bool throwExceptionIfNull, TryParseHandler<T> handler)
            where T : struct
        {
            return WebHelper.GetViewStateValue<T>(viewState, valueName, defaultValue, throwExceptionIfNull, handler);
        }

        /// <summary>
        /// Finds the master page instance of the specified type that this page inherits from.
        /// </summary>
        /// <typeparam name="T">The Type name of the MasterPage instance to find.</typeparam>
        /// <param name="currentPage"></param>
        /// <returns>An instance of type <see cref="T:MasterPage"/> if one is found.  Otherwise, null.</returns>
        public static T GetMasterPage<T>(this Page currentPage)
            where T : MasterPage
        { return WebHelper.FindMasterOfType<T>(currentPage); }

        //***************************************************************************
        // Static Methods
        // 
        public static int GetViewStateIntValue(StateBag viewState, string valueName)
        { return WebHelper.GetViewStateIntValue(viewState, valueName, 0); }
        public static int GetViewStateIntValue(StateBag viewState, string valueName, int defaultValue)
        { return WebHelper.GetViewStateIntValue(viewState, valueName, defaultValue, false); }
        public static int GetViewStateIntValue(StateBag viewState, string valueName, int defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateValue<int>(viewState, valueName, defaultValue, throwExceptionIfNull, int.TryParse); }

        public static float GetViewStateFloatValue(StateBag viewState, string valueName)
        { return WebHelper.GetViewStateFloatValue(viewState, valueName, float.NaN); }
        public static float GetViewStateFloatValue(StateBag viewState, string valueName, float defaultValue)
        { return WebHelper.GetViewStateFloatValue(viewState, valueName, defaultValue, false); }
        public static float GetViewStateFloatValue(StateBag viewState, string valueName, float defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateValue<float>(viewState, valueName, defaultValue, float.TryParse); }

        public static double GetViewStateDoubleValue(StateBag viewState, string valueName)
        { return WebHelper.GetViewStateDoubleValue(viewState, valueName, double.NaN); }
        public static double GetViewStateDoubleValue(StateBag viewState, string valueName, double defaultValue)
        { return WebHelper.GetViewStateDoubleValue(viewState, valueName, defaultValue, false); }
        public static double GetViewStateDoubleValue(StateBag viewState, string valueName, double defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateValue<double>(viewState, valueName, defaultValue, double.TryParse); }

        public static bool GetViewStateBoolValue(StateBag viewState, string valueName)
        { return WebHelper.GetViewStateBoolValue(viewState, valueName, false); }
        public static bool GetViewStateBoolValue(StateBag viewState, string valueName, bool defaultValue)
        { return WebHelper.GetViewStateBoolValue(viewState, valueName, defaultValue, false); }
        public static bool GetViewStateBoolValue(StateBag viewState, string valueName, bool defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateValue<bool>(viewState, valueName, defaultValue, throwExceptionIfNull, bool.TryParse); }

        public static DateTime GetViewStateDateTimeValue(StateBag viewState, string valueName)
        { return WebHelper.GetViewStateDateTimeValue(viewState, valueName, DateTime.MinValue); }
        public static DateTime GetViewStateDateTimeValue(StateBag viewState, string valueName, DateTime defaultValue)
        { return WebHelper.GetViewStateDateTimeValue(viewState, valueName, defaultValue, false); }
        public static DateTime GetViewStateDateTimeValue(StateBag viewState, string valueName, DateTime defaultValue, bool throwExceptionIfNull)
        { return WebHelper.GetViewStateValue<DateTime>(viewState, valueName, defaultValue, throwExceptionIfNull, DateTime.TryParse); }

        public static T GetViewStateValue<T>(StateBag viewState, string valueName)
        //where T : struct
        { return WebHelper.GetViewStateValue<T>(viewState, valueName, default(T), true, null); }
        public static T GetViewStateValue<T>(StateBag viewState, string valueName, TryParseHandler<T> handler)
        //where T : struct
        { return WebHelper.GetViewStateValue<T>(viewState, valueName, default(T), handler); }
        public static T GetViewStateValue<T>(StateBag viewState, string valueName, T defaultValue, TryParseHandler<T> handler)
        //where T : struct
        { return WebHelper.GetViewStateValue<T>(viewState, valueName, defaultValue, false, handler); }
        public static T GetViewStateValue<T>(StateBag viewState, string valueName, T defaultValue, bool throwExceptionIfNull, TryParseHandler<T> handler)
        //where T : struct
        {
            object vsVal = viewState[valueName];
            if (vsVal is T)
                return (T)vsVal;
            else if (handler == null)
            {
                if (throwExceptionIfNull)
                    throw new Exception("Specified view state value is not the correct data type, and no tryparse delegate was provided.");
                else
                    return defaultValue;
            }
            else
            {
                if (vsVal == null)
                    if (throwExceptionIfNull)
                        throw new Exception("There is no value in the viewstate for this name.");
                    else
                        return defaultValue;

                T tVal;
                if (handler(vsVal.ToString(), out tVal))
                    return tVal;
                else
                    if (throwExceptionIfNull)
                        throw new Exception("Value stored in viewstate could not be parsed with the provided handler method.");
                    else
                        return defaultValue;
            }
        }

        public static T FindMasterOfType<T>(Page currentPage)
            where T : MasterPage
        {
            MasterPage mPg = currentPage.Master;
            while (mPg != null)
            {
                if (mPg is T)
                    return (T)mPg;

                mPg = mPg.Master;
            }
            return null;
        }
        #endregion
    }
}
