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

        /// <summary>
        /// Adds a link to the specified CSS file to this <typeparamref name="System.Web.Page"/> instance's header, using the specified key to prevent duplicate links.
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="cssUrl">A <typeparamref name="System.String"/> value containing the app-relative path to the CSS file you want to reference.</param>
        /// <returns>A <typeparamref name="System.Web.UI.Control"/> object containing either the existing reference (if found) or the added reference.</returns>
        public static System.Web.UI.Control AddCssReference(this Page pg, string cssUrl)
        { return pg.AddCssReference(cssUrl, string.Empty); }
        /// <summary>
        /// Adds a link to the specified CSS file to this <typeparamref name="System.Web.Page"/> instance's header, using the specified key to prevent duplicate links.
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="cssUrl">A <typeparamref name="System.String"/> value containing the app-relative path to the CSS file you want to reference.</param>
        /// <param name="keyName">A <typeparamref name="System.String"/> value to be used as a 'key' to prevent the same CSS file from being referenced multiple times.</param>
        /// <returns>A <typeparamref name="System.Web.UI.Control"/> object containing either the existing reference (if found) or the added reference.</returns>
        public static System.Web.UI.Control AddCssReference(this Page pg, string cssUrl, string keyName)
        {
            string fullCssUrl = pg.ResolveUrl(cssUrl);

            if (string.IsNullOrEmpty(keyName))
                keyName = fullCssUrl;

            Control lnkCheck = pg.Header.FindControl(keyName);
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = keyName;
                link.Attributes.Add("href", fullCssUrl);
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                pg.Header.Controls.Add(link);
                lnkCheck = link;
            }
            return lnkCheck;
        }

        /// <summary>
        /// Adds a <typeparamref name="System.ListItem"/> as the first item in this <typeparamref name="System.Web.UI.WebControls.ListControl"/>'s "Items" collection, with the text value of "- Select -".
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="defaultValue">A <typeparamref name="System.String"/> value to assign to the new item.  Default is "-1".</param>
        public static void AddSelectItem(this System.Web.UI.WebControls.ListControl ctrl, string defaultValue = "-1")
        {
            ctrl.Items.Insert(0, new System.Web.UI.WebControls.ListItem("- Select -", "-1"));
        }

        /// <summary>
        /// Adds a script tag to this page, with the "src" attribute pointed to the named javascript file.
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="scriptUrl">A <typeparamref name="System.String"/> value indicating the app-relative path to the Javascript file you want to include on the page.</param>
        public static void AddJavascriptInclude(this Page pg, string scriptUrl)
        { pg.AddJavascriptInclude(scriptUrl, string.Empty); }
        /// <summary>
        /// Adds a script tag to this page, with the "src" attribute pointed to the named javascript file.
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="scriptUrl">A <typeparamref name="System.String"/> value indicating the app-relative path to the Javascript file you want to include on the page.</param>
        /// <param name="keyName">A <typeparamref name="System.String"/> value to be used as a 'key' to prevent the same Javascript file from being referenced multiple times.</param>
        public static void AddJavascriptInclude(this Page pg, string scriptUrl, string keyName)
        {
            string fullScriptUrl = pg.ResolveUrl(scriptUrl);

            if (string.IsNullOrEmpty(keyName))
                keyName = fullScriptUrl;

            pg.ClientScript.RegisterClientScriptInclude(pg.GetType(), keyName, fullScriptUrl);
        }

        public static void AddPageLoadCommand(this Page pg, string jsCommand)
        { WebHelper.AddPageLoadCommand(pg, jsCommand, string.Empty); }
        public static void AddPageLoadCommand(this Page pg, string jsCommand, string keyName)
        {
            string js = string.Format("Sys.Application.add_load(function () {{ {0}; }});", jsCommand.TrimEnd(';'));
            pg.ClientScript.RegisterStartupScript(pg.GetType(), (string.IsNullOrEmpty(keyName) ? jsCommand : keyName), js, true);
        }

        /// <summary>
        /// Searches the specified parent control for a child control of the specified type, and throws an exception if not found.
        /// </summary>
        /// <typeparam name="T">The Type of the control to search for.</typeparam>
        /// <param name="pg"></param>
        /// <param name="parent">The parent control to search in.</param>
        /// <param name="childName">The ID of the child control to search for.</param>
        /// <returns>A <typeparamref name="System.Web.UI.WebControls.WebControl"/> of the type specified that is a child of the specified parent.</returns>
        public static T FindElementControl<T>(this Page pg, System.Web.UI.WebControls.WebControl parent, string childName)
            where T : System.Web.UI.WebControls.WebControl
        {
            T ctrl = (parent.FindControl(childName) as T);
            if (ctrl == null)
                throw new Exception(string.Format("Unable to locate control '{0}' of type '{1}' on page.", childName, typeof(T).Name));
            else
                return ctrl;
        }

        /// <summary>
        /// Returns the <typeparamref name="System.Web.UI.Control"/> object that triggered the current page postback, or NULL if no postback is in progress.
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>
        public static Control GetPostbackSource(this Page pg)
        {
            if (!pg.IsPostBack)
                return null;

            Control submitControl = null;

            // If the postback was done via "DoPostback" then the postback control ID is stored in "__EVENTTARGET".
            string ctrlName = pg.Request.Form["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctrlName))
            {
                submitControl = pg.FindControl(ctrlName);
            }
            else
            {
                // If __EVENTTARGET is null, then the control is a Button or ImageButton, so we'll grab it from
                //   the Form collection.
                Control foundCtrl = null;
                foreach (string ctrl in pg.Request.Form)
                {
                    if (ctrl.EndsWith(".x") || ctrl.EndsWith(".y"))
                    {
                        // The ImageButton control uses an additional 'quasi-property'
                        //   in their ID which indicates the X/Y coords of the img.
                        foundCtrl = pg.FindControl(ctrl.Substring(0, ctrlName.Length - 2));
                    }
                    else
                    {
                        foundCtrl = pg.FindControl(ctrl);
                    }

                    if (!(foundCtrl is System.Web.UI.WebControls.IButtonControl))
                        continue;

                    // The "Form" collection only contains the ID for the single control which triggered the postback.
                    submitControl = foundCtrl;
                    break;
                }
            }
            return submitControl;
        }

        /// <summary>
        /// Uses the <typeparamref name="System.Web.Script.Serialization.JavaScriptSerializer"/> class to convert this object into a JSON data object that can be passed and used by client side scripts.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ToJSON(this object src)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(src);
        }

        /// <summary>
        /// NOTE: This will fail if any public members of the specified type are not standard .NET types.
        /// </summary>
        /// <typeparam name="T">The type to convert the serialized JSON data into.</typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T FromJSON<T>(this string src)
        {
            // TODO:: This will fail if any properties within the containing Type are
            //   not standard system data types.  I need to capture this exception,
            //   and display a message indicating exactly what needs to be done to
            //   make this class work, or provide an overload with reflection
            //   that can make this work with non-system types.
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<T>(src);
        }

        public static System.Web.UI.Control FindParent(this System.Web.UI.Control ctrl, string parentID)
        { return FindParent<System.Web.UI.WebControls.WebControl>(ctrl, parentID); }
        public static System.Web.UI.Control FindParent<T>(this System.Web.UI.Control ctrl)
            where T : System.Web.UI.Control
        { return FindParent<T>(ctrl, null); }
        public static T FindParent<T>(this System.Web.UI.Control ctrl, string parentID)
            where T : System.Web.UI.Control
        {
            Control parent = ctrl.Parent;
            while (parent != null)
            {
                if ((parent is T) && ((parent.ID == parentID) || string.IsNullOrEmpty(parentID)))
                    return (T)parent;

                parent = parent.Parent;
            }
            return null;
        }

        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Encodes the specified <typeparamref name="System.String"/> into a format that is acceptable for use in a URL or query string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        { return WebUtil.UrlEncode(value); }
        /// <summary>
        /// Decodes the specified <typeparamref name="System.String"/> from a URL-acceptable format into human-readable text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlDecode(string value)
        { return WebUtil.UrlDecode(value); }

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
