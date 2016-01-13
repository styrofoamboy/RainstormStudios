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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Web
{
    /// <summary>
    /// Provides a simple method for adding parameters to a querystring before reidrecting the user to a different page, and for
    /// redirecting users back to a calling back, in the event that a "ReturnUrl" parameter exists in the query string.
    /// </summary>
    public sealed class PageRedirect
    {
        #region Nested Classes
        //***************************************************************************
        // Nested Classes
        // 
        public enum Mode
        {
            Redirect,
            ReturnToCallingPage
        }
        #endregion

        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private Mode
            _curMode;
        private NameValueCollection
            _qsVals;
        private string
            _dest;
        private HttpContext
            _context;
        private bool
            _includeQs;
        List<string>
            _includeQsNames;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        /// <summary>
        /// Indicates the current redirection mode.
        /// </summary>
        public Mode CurrentMode
        { get { return this._curMode; } }
        /// <summary>
        /// A <see cref="T:System.String"/> value indicating the current redirect target.
        /// </summary>
        public string Destination
        {
            get { return this._dest; }
            set { this._dest = value; }
        }
        /// <summary>
        /// A <see cref="T:System.Collections.Specialized.NameValueCollection"/> object containing the QueryString parameters included in the request and redirect.
        /// </summary>
        public NameValueCollection QueryStringValues
        { get { return this._qsVals; } }
        /// <summary>
        /// A <see cref="T:System.Collections.Generic.List"/> object of type <see cref="T:System.String"/> containing the names of the QueryString parameters included in the current request.
        /// </summary>
        public List<string> ReturnedValues
        { get { return this._includeQsNames; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private PageRedirect()
        {
            this._qsVals = new NameValueCollection();
            this._includeQsNames = new List<string>();
        }
        /// <summary>
        /// This is for an initial redirect to a different page.
        /// </summary>
        /// <param name="destination">A <see cref="T:System.String"/> value indicating the target page to redirect to.</param>
        /// <param name="context">The current ASP.NET execution context.</param>
        public PageRedirect(string destination, HttpContext context)
            : this()
        {
            this._curMode = Mode.Redirect;
            this._dest = destination;
            this._context = context;
        }
        /// <summary>
        /// This is for sending the request back to the calling page.
        /// </summary>
        /// <param name="context">The current ASP.NET execution context.</param>
        /// <param name="includeQueryString">A <see cref="T:System.Boolean"/> value indicating true of the current QueryString parameters should be returned to the calling page. Otherwise, false.</param>
        public PageRedirect(HttpContext context, bool includeQueryString)
            : this()
        {
            this._curMode = Mode.ReturnToCallingPage;
            this._context = context;
            this._includeQs = includeQueryString;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        /// <summary>
        /// Adds a new QueryString parameter to pass to the redirected page.
        /// </summary>
        /// <param name="name">The name of the QueryString parameter.</param>
        /// <param name="value">The value of the QueryString parameter.</param>
        public void AddParameter(string name, object value)
        {
            if (value == null)
                throw new ArgumentNullException("Cannot add null values to the querystring.");

            this._qsVals.Add(name, value.ToString());
        }
        /// <summary>
        /// Performs a redirect to the specified page.  The page specified here overwrites any previously specified redirect target.
        /// </summary>
        /// <param name="destination">A <see cref="T:System.String"/> value indicating the page to redirect to.</param>
        public void Go(string destination)
        {
            this._dest = destination;
            this.Go();
        }
        /// <summary>
        /// Performs a redirect to the previously specified page target, including the stored QueryString parameters.
        /// </summary>
        public void Go()
        {
            if (this._curMode == Mode.Redirect)
            {
                PageRedirect.RedirectToPage(this._dest, this._context, this._qsVals);
            }
            else if (this._curMode == Mode.ReturnToCallingPage)
            {
                if (string.IsNullOrEmpty(this._dest))
                {
                    if (this._includeQsNames.Count > 0)
                        PageRedirect.SendBackToCallingPage(this._context, this._qsVals, this._includeQsNames.ToArray());
                    else
                        PageRedirect.SendBackToCallingPage(this._context, this._qsVals, this._includeQs);
                }
                else
                {
                    if (this._includeQsNames.Count > 0)
                        PageRedirect.SendBackToCallingPage(this._dest, this._context, this._qsVals, this._includeQsNames.ToArray());
                    else
                        PageRedirect.SendBackToCallingPage(this._dest, this._context, this._qsVals, this._includeQs);
                }
            }
            else
            {
                // Only way this can happen is if somebody messes with the code and
                //   doesn't update this method.  I like to be thorough...
                throw new InvalidOperationException("Redirect mode is not valid.");
            }
        }
        //***************************************************************************
        // Static Methods
        // 
        public static void RedirectToPage(string destination, HttpContext context)
        { PageRedirect.RedirectToPage(destination, context, null); }
        public static void RedirectToPage(string destinationPg, HttpContext context, NameValueCollection qsVals)
        {
            if (context == null)
                throw new ArgumentNullException("context", "You must pass the current HttpContext object for this method to function.");
            else if (context.Response == null || context.Request == null)
                throw new Exception("Specified HttpContext must contain both a valid request and response object.");

            string redirectUrl = PageRedirect.GetUrlPefix(destinationPg);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (qsVals != null)
            {
                for (int i = 0; i < qsVals.Count; i++)
                    sb.AppendFormat("&{0}={1}", context.Server.UrlEncode(qsVals.Keys[i]), context.Server.UrlEncode(qsVals[i]));
            }

            redirectUrl += "?ReturnUrl=" + PageRedirect.GetCurrentPageEncoded(context) + sb.ToString();
            context.Response.Redirect(redirectUrl, true);
        }
        public static void SendBackToCallingPage(HttpContext context)
        { PageRedirect.SendBackToCallingPage(context, true); }
        public static void SendBackToCallingPage(HttpContext context, bool includeQueryString)
        { PageRedirect.SendBackToCallingPage(context, null, includeQueryString); }
        public static void SendBackToCallingPage(HttpContext context, NameValueCollection additionalParams, bool includeQueryString)
        { PageRedirect.SendBackToCallingPage(context, null, PageRedirect.GetQueryStringParamKeys(context.Request)); }
        public static void SendBackToCallingPage(HttpContext context, params string[] includedParams)
        { PageRedirect.SendBackToCallingPage(context, null, includedParams); }
        public static void SendBackToCallingPage(HttpContext context, NameValueCollection additionalParams, params string[] includedParams)
        {
            if (string.IsNullOrEmpty(context.Request.QueryString["ReturnURL"]))
                throw new ArgumentException("To use this method, the requesting querystring must contain a value called 'ReturnURL'.", "context");

            PageRedirect.SendBackToCallingPage(context.Request.QueryString["ReturnURL"], context, additionalParams, includedParams);
        }
        public static void SendBackToCallingPage(string destinationPg, HttpContext context, bool includeQueryString)
        { PageRedirect.SendBackToCallingPage(destinationPg, context, null, includeQueryString); }
        public static void SendBackToCallingPage(string destinationPg, HttpContext context, NameValueCollection additionalParams, bool includeQueryString)
        { PageRedirect.SendBackToCallingPage(destinationPg, context, additionalParams, PageRedirect.GetQueryStringParamKeys(context.Request)); }
        public static void SendBackToCallingPage(string destinationPg, HttpContext context, params string[] includedParams)
        { PageRedirect.SendBackToCallingPage(destinationPg, context, null, includedParams); }
        public static void SendBackToCallingPage(string destinationPg, HttpContext context, NameValueCollection additionalParams, params string[] includedParams)
        {
            if (context == null)
                throw new ArgumentNullException("context", "You must pass the current HttpContext object for this method to function.");
            else if (context.Response == null || context.Request == null)
                throw new Exception("Specified HttpContext must contain both a valid request and response object.");
            if (string.IsNullOrEmpty(destinationPg))
                throw new ArgumentNullException("desinationPg", "You must specify the destination page you want to redirect to.");

            string responseUrl = PageRedirect.GetUrlPefix(destinationPg);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (includedParams.Length > 0)
            {
                // We're using a delegate to define an 'anonymous' method, since the Array.ConvertAll
                //   requires that we create a 'Converter' delegate pointed to a method, but this
                //   saves the overhead of having a 'real' method call for one line of code.
                string[] lVals = Array.ConvertAll<string, string>(includedParams, new Converter<string, string>(delegate(string val) { return val.ToLower(); }));
                HttpRequest req = context.Request;

                for (int i = 0; i < req.QueryString.Count; i++)
                    if (lVals.Contains(req.QueryString.GetKey(i).ToLower()))
                        sb.AppendFormat("&{0}={1}", context.Server.UrlEncode(req.QueryString.GetKey(i)), context.Server.UrlEncode(req.QueryString.Get(i)));
            }
            if (additionalParams != null)
            {
                for (int i = 0; i < additionalParams.Count; i++)
                    sb.AppendFormat("&{0}={1}", context.Server.UrlEncode(additionalParams.GetKey(i)), context.Server.UrlEncode(additionalParams[i]));
            }

            string qs = sb.ToString();
            if (!string.IsNullOrEmpty(qs))
                responseUrl += "?" + qs.TrimStart('&');

            context.Response.Redirect(responseUrl);
        }
        public static void ClearQueryString(HttpContext context)
        { PageRedirect.ClearQueryString(context, string.Empty); }
        public static void ClearQueryString(HttpContext context, string queryStringValues)
        {
            NameValueCollection nvCol = new NameValueCollection();
            string[] pairs = queryStringValues.Split('|', ',', ' ', ';', '&');
            for (int i = 0; i < pairs.Length; i++)
            {
                string[] val = pairs[i].Split('=');
                if (val.Length < 2)
                    throw new ArgumentException("Specified query string values is not in an allowable format.");
                nvCol.Add(val[0], val[1]);
            }
            PageRedirect.ClearQueryString(context, nvCol);
        }
        public static void ClearQueryString(HttpContext context, NameValueCollection newParams)
        {
            if (context == null)
                throw new ArgumentNullException("context", "You must pass the current HttpContext object for this method to function.");
            else if (context.Response == null || context.Request == null)
                throw new Exception("Specified HttpContext must contain both a valid request and response object.");

            string responseUrl = PageRedirect.GetCurrentPage(context);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (newParams != null)
            {
                for (int i = 0; i < newParams.Count; i++)
                    sb.AppendFormat("&{0}={1}", context.Server.UrlEncode(newParams.Keys[i]), context.Server.UrlEncode(newParams[i]));
                responseUrl += "?" + sb.ToString().TrimStart('&');
            }

            context.Response.Redirect(responseUrl);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Static Methods
        // 
        private static string[] GetQueryStringParamKeys(HttpRequest req)
        {
            List<string> qsKeys = new List<string>();
            for (int i = 0; i < req.QueryString.Keys.Count; i++)
                if (req.QueryString.GetKey(i).ToLower() != "returnurl")
                    qsKeys.Add(req.QueryString.GetKey(i));
            return qsKeys.ToArray();
        }
        private static string GetUrlPefix(string url)
        {
            string responseUrl = url;
            //if (VirtualPathUtility.IsAbsolute(url))
            //    responseUrl += "~/" + url.TrimStart('/');
            if (VirtualPathUtility.IsAppRelative(url))
                responseUrl = VirtualPathUtility.ToAbsolute(responseUrl);
            return responseUrl.Replace(HttpContext.Current.Request.FilePath, "");
        }
        private static string GetCurrentPage(HttpContext context)
        {
            string returnUrl = "~" + context.Request.FilePath.ToLower();
            if (System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath != "/")
                returnUrl = returnUrl.Replace(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.ToLower(), "");
            return returnUrl;
        }
        private static string GetCurrentPageEncoded(HttpContext context)
        {
            return context.Server.UrlEncode(PageRedirect.GetCurrentPage(context));
        }
        #endregion
    }
}
