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

namespace RainstormStudios.Web
{
    static class WebUtil
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        [Obsolete("Do not reference this variable directly.  Use the 'PseudoPage' property.")]
        private static System.Web.UI.Page
            _cachedPage;
        #endregion

        #region Properties
        //***************************************************************************
        // Private Properties
        // 
        private static System.Web.UI.Page PseudoPage
        {
            get
            {
#pragma warning disable 612, 618
                if (_cachedPage == null)
                    _cachedPage = new System.Web.UI.Page();
                return _cachedPage;
#pragma warning restore 612, 618
            }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Static Methods
        // 
        public static string GetWebResourceUrl(Type type, string resource)
        {
            return PseudoPage.ClientScript.GetWebResourceUrl(type, resource);
        }
        public static bool IsAbsoluteUrl(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("URL was not well formed.", "url");

            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
        public static string MakeAbsoluteUrl(string relativeUrl)
        { return MakeAbsoluteUrl(relativeUrl, System.Web.HttpContext.Current); }
        public static string MakeAbsoluteUrl(string relativeUrl, System.Web.HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Cannot use NULL context.", "context");

            if (WebUtil.IsAbsoluteUrl(relativeUrl))
                return relativeUrl;

            // First, make sure we don't have an 'app-relative' path (starts with "~/")
            string fullPath = relativeUrl;
            if (!System.Web.VirtualPathUtility.IsAbsolute(relativeUrl))
                fullPath = System.Web.VirtualPathUtility.ToAbsolute(relativeUrl);

            // Now we're ready to build out the full URL;
            // TODO:: "Server Name" is probably wrong, here.  Need to test.
            return string.Format("http://{0}/{1}", context.Request.Url.Host, fullPath.TrimStart('/'));
        }
        public static string UrlEncode(string value)
        { return PseudoPage.Server.UrlEncode(value); }
        public static string UrlDecode(string value)
        { return PseudoPage.Server.UrlDecode(value); }
        public static string ResolveUrl(string url)
        { return PseudoPage.ResolveUrl(url); }
        public static string MapPath(string url)
        { return PseudoPage.Server.MapPath(url); }
        #endregion
    }
}
