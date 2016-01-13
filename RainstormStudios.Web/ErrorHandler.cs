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
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace RainstormStudios.Web
{
    [Author("Unfried, Michael")]
    public interface IMasterPageErrorHandler
    {
        void ShowSystemError(string msg, bool showAdditionalInfo = true);
    }
    [Author("Unfried, Michael")]
    public class ErrorHandler
    {
        #region Declaration
        //***************************************************************************
        // Private Fields
        // 
        protected static ErrorHandlerConfiguration
            _config;
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static ErrorHandler()
        {
            Initialize();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public static void ShowErrorMessage<T>(Page pg, string message, Exception ex = null, bool showSupportMsg = false) where T : MasterPage, IMasterPageErrorHandler
        {
            T rootMaster = WebHelper.FindMasterOfType<T>(pg);

            if (rootMaster != null)
                rootMaster.ShowSystemError(message, showSupportMsg);
            else
                throw new Exception("Unable to display system message: " + message, ex);
        }
        public static void SendServerErrMsg(string codeSection, Exception exSvr, HttpContext context, SeverityLevel lvl = SeverityLevel.Warning)
        {
            if (!_config.EnableEmailAlerts)
                return;

            if (context != null && context.Request != null && context.Request.Url != null && context.Request.Url.IsLoopback && !_config.SendAlertsInDebugMode)
                return;

            if (context!=null && context.IsDebuggingEnabled && !_config.SendAlertsInDebugMode)
                return;

            // Attempt to get the currently logged in user.
            System.Web.Security.MembershipUser curUsr = null;
            try
            { curUsr = System.Web.Security.Membership.GetUser(); }
            catch { } // If it fails, don't worry about it.

            // Set the message's subject line.
            string subject = _config.ApplicationName + " Exception - " + codeSection;

            // Build the page body in a StringBuilder object.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (context != null && context.Request != null)
                sb.AppendLine(string.Format("<h2>{0}</h2>", context.Request.ApplicationPath));
            sb.AppendLine("<p>The following exception has occured:</p>");
            sb.AppendLine("<ul>");
            Exception innerEx = exSvr;
            while (innerEx != null)
            {
                sb.AppendLine(string.Format("<li>{0}</li>", innerEx.Message));
                innerEx = innerEx.InnerException;
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("<p/>");
            if (exSvr.TargetSite != null)
                sb.AppendLine(string.Format("<p>Target Site: {0} {1} (in {2})</p>", exSvr.TargetSite.MemberType, exSvr.TargetSite.Name, exSvr.TargetSite.DeclaringType));
            else
                sb.AppendLine("<p>Target Site: N/A</p>");
            sb.AppendLine("<p/>");
            sb.AppendLine("<hr />");
            if (context != null && context.Request != null)
            {
                HttpRequest req = context.Request;
                sb.AppendLine("<p>");
                sb.AppendLine(string.Format("<div>Request URL: {0}</div>", req.Url));
                sb.AppendLine(string.Format("<div>Request Path: {0}</div>", req.FilePath));
                sb.AppendLine(string.Format("<div>User: {0}</div>", (curUsr != null) ? curUsr.UserName : "Unknown"));
                sb.AppendLine(string.Format("<div>User Host Address: {0}</div>", req.UserHostAddress));
                // We're going to try a reverse DNS search on the IP address.  Failover,
                //   just uses the UserHostName value from the HttpContext object.
                string hostName = null;
                try { hostName = System.Net.Dns.GetHostEntry(req.UserHostAddress).HostName; }
                catch { hostName = null; }
                if (hostName == null) hostName = req.UserHostName;
                sb.AppendLine(string.Format("<div>User Host Name: {0}</div>", hostName));
                sb.AppendLine(string.Format("<div>User Agent: {0}</div>", req.UserAgent));
                sb.AppendLine(string.Format("<div>Server Name: {0}</div>", (context.Server != null ? context.Server.MachineName : "Unknown")));
                sb.AppendLine(string.Format("<div>Request Identity: {0}</div>", (req.LogonUserIdentity != null ? req.LogonUserIdentity.Name : "Unknown")));

                sb.AppendLine("</p>");
                sb.AppendLine("<hr />");
            }
            sb.AppendLine("<p>Stack trace follows:</p>");
            Exception ex = exSvr;
            while (ex != null)
            {
                sb.AppendLine("<p>");
                sb.AppendLine(string.Format("<b>&gt;&gt;&nbsp;{0}:</b> {1} in {2}<br/>", ex.GetType().Name, HttpUtility.HtmlEncode(ex.Message), HttpUtility.HtmlEncode(ex.Source)));
                sb.AppendLine(HttpUtility.HtmlEncode(ex.StackTrace));
                sb.AppendLine("</p>");
                ex = ex.InnerException;
            }
            if (context.Trace.IsEnabled)
            {
                sb.AppendLine("<hr />");
                sb.AppendLine("<p>Web Trace:</p>");
                sb.AppendLine(string.Format("<p>{0}</p>", HttpUtility.HtmlEncode(context.Trace.ToString())));
            }

            EmailHelper.SendAdminEmailNotification(subject.Trim(), sb.ToString(), true);
        }
        public static void OnError(string errMsg, Exception ex, HttpContext context, Page pg = null, SeverityLevel lvl = SeverityLevel.Warning, string moduleName = null)
        {
            if (_config.WriteErrorsToEventLog)
                RainstormStudios.Web.LogManager.WriteToLog(ex, context, lvl, moduleName);

            if (_config.EnableEmailAlerts)
                ErrorHandler.SendServerErrMsg(errMsg, ex, context, lvl);

            if (pg != null)
            {
                // In order to let the end user specify the Type value that we're going to pass to the generic method,
                //   we have to do a little reflection to pull everything together.
                // 
                // ErrorHandler.ShowErrorMessage<userMasterPageTypeHere>(pg, errMsg, ex);

                // First, we're going to find the "ShowErrorMessage" method.  This is the generic method above.
                System.Reflection.MethodInfo method = typeof(ErrorHandler).GetMethod("ShowErrorMessage", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                // Now, we need to find the Type that the user specified in the .config file.
                Type modalMasterType = System.Reflection.Assembly.GetCallingAssembly().GetType(_config.ErrorModalMasterClassName, false);
                if (modalMasterType == null)
                    // If we couldn't find a type with the specified name, let them know that something's wrong.
                    throw new ConfigurationErrorsException("Unable to locate specified 'errorModalMasterClassName' withing application assembly.");

                // Now, we're going to make our method reference from above include the generic Type parameter.
                System.Reflection.MethodInfo generic = method.MakeGenericMethod(modalMasterType);

                // Finally, we're ready to call the method.
                generic.Invoke(null, new object[] { pg, errMsg, ex, true });
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private static void Initialize()
        {
            _config = GetConfig();
        }
        private static ErrorHandlerConfiguration GetConfig()
        {
            ErrorHandlerConfiguration config = ErrorHandlerConfiguration.config;
            if (config == null)
                throw new ConfigurationErrorsException("You must specify an 'ErrorHandlerConfiguration' section in your web.config to use this class.");
            else
                return config;
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class ErrorHandlerConfiguration : System.Configuration.ConfigurationSection
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly ErrorHandlerConfiguration config =
            (ConfigurationManager.GetSection("ErrorHandler") as ErrorHandlerConfiguration);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("appName", IsRequired = false)]
        [SettingsDescription("Specifies the application name that will appear on the error logs.")]
        //[StringValidator(MinLength = 1)]
        public string ApplicationName
        {
            get
            {
                string appNm = (string)base["appName"];
                if (string.IsNullOrWhiteSpace(appNm))
                    return System.Web.Hosting.HostingEnvironment.SiteName;
                else
                    return appNm;
            }
            set { base["appName"] = value; }
        }
        [ConfigurationProperty("writeErrorsToEventLog", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("Specifies 'true' if error messages should be written to the event log using the 'ITCWebToolkit.Web.LogManager' class.  Otherwise, false.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool WriteErrorsToEventLog
        {
            get { return (bool)base["writeErrorsToEventLog"]; }
            set { base["writeErrorsToEventLog"] = value; }
        }
        [ConfigurationProperty("enableEmailAlerts", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("Indicates 'true' if email alerts should be automatically sent to the specified email addresses.  Otherwise, false.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool EnableEmailAlerts
        {
            get { return (bool)base["enableEmailAlerts"]; }
            set { base["enableEmailAlerts"] = value; }
        }
        [ConfigurationProperty("sendAlertsInDebugMode", DefaultValue = "false", IsRequired = false)]
        [SettingsDescription("Indicates 'true' if email alerts should be sent when the application's 'Debug' flag is enabled.  Otherwise, false.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool SendAlertsInDebugMode
        {
            get { return (bool)base["sendAlertsInDebugMode"]; }
            set { base["sendAlertsInDebugMode"] = value; }
        }
        [ConfigurationProperty("sendAlertsFromLocalhost", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("Indicates 'true' if email alerts should be sent when the current execution context is 'localhost'.  Otherwise, false.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool SendAlertsFromLocalhost
        {
            get { return (bool)base["sendAlertsFromLocalhost"]; }
            set { base["sendAlertsFromLocalhost"] = value; }
        }
        [ConfigurationProperty("recipients", IsDefaultCollection = true, IsRequired = false)]
        [SettingsDescription("Specifies a list of users who will receive email alerts about exceptions in this application.")]
        public ErrorEmailContactElementCollection AlertRecipients
        {
            get { return (ErrorEmailContactElementCollection)base["recipients"]; }
            set { base["recipients"] = value; }
        }
        [ConfigurationProperty("errorModalMasterClassName", IsRequired = false)]
        [SettingsDescription("Specifies the class name of the master page which inherits the IMasterPageErrorHandler interface and will be responsible for showing error messages to the user.")]
        //[SubclassTypeValidator(typeof(IMasterPageErrorHandler))]
        public string ErrorModalMasterClassName
        {
            get { return (string)base["errorModalMasterClassName"]; }
            set { base["errorModalMasterClassName"] = value; }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class ErrorEmailContactElement : EmailContactElement
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("severityLevel", DefaultValue = SeverityLevel.Warning, IsRequired = false)]
        [SettingsDescription("Specifies the lowest level of severity which should result in an alert being sent to this address.")]
        //[SubclassTypeValidator(typeof(SeverityLevel))]
        public SeverityLevel SeverityLevel
        {
            get { return (SeverityLevel)base["severityLevel"]; }
            set { base["severityLevel"] = value; }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    [ConfigurationCollection(typeof(ErrorEmailContactElement))]
    public class ErrorEmailContactElementCollection : System.Configuration.ConfigurationElementCollection
    {
        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErrorEmailContactElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErrorEmailContactElement)element).EmailAddress;
        }
        #endregion
    }
}
