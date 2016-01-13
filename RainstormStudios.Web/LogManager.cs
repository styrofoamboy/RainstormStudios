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

namespace RainstormStudios.Web
{
    [Author("Unfried, Michael")]
    public class LogManager
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        static LogManagerConfiguration
            _config;
        static System.Diagnostics.EventLog
            _eventLog;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        static LogManager()
        {
            // The private, static constructor will be called the first time someone uses the class.
            Initialize();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public static void WriteToLog(Exception ex, System.Web.HttpContext context, SeverityLevel lvl = SeverityLevel.Warning, string moduleName = null)
        {
            Exception curEx = ex;
            System.Text.StringBuilder sbEx = new System.Text.StringBuilder();
            while (curEx != null)
            {
                sbEx.AppendFormat(": {0}", curEx.Message);
                curEx = curEx.InnerException;
            }
            string exMsg = sbEx.ToString().Replace('\r', ' ').Replace('\n', ' ').TrimEnd(':').Trim();
            if (string.IsNullOrEmpty(exMsg))
                exMsg = ": Unknown Error";

            string msgTxt = null;
            if (context != null && context.Request != null)
                msgTxt = string.Format("{0}{1} - {2}", !string.IsNullOrEmpty(context.Request.UserHostAddress) ? context.Request.UserHostAddress : "Unknown Host", exMsg, context.Request.FilePath);
            else
                msgTxt = string.Format("{0}{1} - {2}", "0.0.0.0", exMsg, "<Unknown>");

            try
            {
                string modNm = (string.IsNullOrEmpty(moduleName) ? (context.Request != null ? context.Request.FilePath : "Uknown") : moduleName);
                if (_config.WriteToTextLog)
                {
                    LogMessage msg = new LogMessage(lvl, msgTxt, modNm);
                    Logger.Instance.WriteToLog(msg);
                }
                else if (_config.WriteToSystemLog)
                {
                    _eventLog.WriteEntry(msgTxt, GetLogType(lvl), 0, 0, System.Text.Encoding.UTF8.GetBytes(ex.ToString()));
                }
            }
            catch (Exception)
            {
                // I cannot allow this to throw its own errors.
                // TODO:: Modify this catch handler so that it at least attempts to write an error at this level to the system event log.
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

            if (_config.WriteToTextLog)
            {
                string lcPath = _config.LogFilePath;
                if (!System.IO.Path.IsPathRooted(lcPath))
                    throw new ConfigurationErrorsException("Please specify the full drive path for the log files to be written to.");

                Logger.Instance.LogFilesDirectory = _config.LogFilePath;
                Logger.Instance.LogFilesExtension = _config.LogFileExtension;
                Logger.Instance.MaxFileSize = _config.MaxFileSize;
                Logger.Instance.RollOverOnNewDate = _config.RollOverOnNewDate;
            }
            else if (_config.WriteToSystemLog)
            {
                _eventLog = new System.Diagnostics.EventLog(_config.SystemLogName);
            }
        }
        private static LogManagerConfiguration GetConfig()
        {
            LogManagerConfiguration config = LogManagerConfiguration.config;
            if (config == null)
                throw new ConfigurationErrorsException("You must supply a 'LogManager' configuration section in the web config.");
            else
                return config;
        }
        private static System.Diagnostics.EventLogEntryType GetLogType(SeverityLevel lvl)
        {
            switch (lvl)
            {
                case SeverityLevel.Debug:
                case SeverityLevel.Information:
                    return System.Diagnostics.EventLogEntryType.Information;
                case SeverityLevel.Warning:
                    return System.Diagnostics.EventLogEntryType.Warning;
                default:
                    return System.Diagnostics.EventLogEntryType.Error;
            }
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public class LogManagerConfiguration : System.Configuration.ConfigurationSection
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly LogManagerConfiguration config =
            (ConfigurationManager.GetSection("LogManager") as LogManagerConfiguration);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("writeToTextLog", DefaultValue = true, IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if errors should be written to a text-based log file.  Otherwise, 'false'.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool WriteToTextLog
        {
            get { return (bool)base["writeToTextLog"]; }
            set { base["writeToTextLog"] = value; }
        }
        [ConfigurationProperty("logPath", DefaultValue = "~/logs", IsRequired = false)]
        [SettingsDescription("Specifies the path where log files should be written.  Default value is '~/logs'.  The IIS worker process user should have permissions to write to this folder.")]
        //[StringValidator(InvalidCharacters = string.Concat(System.IO.Path.GetInvalidPathChars()))]
        public string LogFilePath
        {
            get { return (string)base["logPath"]; }
            set { base["logPath"] = value; }
        }
        [ConfigurationProperty("logFileExt", DefaultValue = ".log", IsRequired = false)]
        [SettingsDescription("Specifies the extension to be given to log files.  Default value is '.log'.")]
        //[StringValidator(InvalidCharacters = "/\\", MinLength = 1)]
        public string LogFileExtension
        {
            get { return (string)base["logFileExt"]; }
            set { base["logFileExt"] = value; }
        }
        [ConfigurationProperty("rollOverOnDate", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("Indicates a valid of 'true' of the LogManager should automatically start a new file for each day.  Otherwise, 'false'.")]
        //[RegexStringValidator("^((true)|(false))$")]
        public bool RollOverOnNewDate
        {
            get { return (bool)base["rollOverOnDate"]; }
            set { base["rollOverOnDate"] = value; }
        }
        [ConfigurationProperty("maxFileSize", DefaultValue = "500000", IsRequired = false)]
        [SettingsDescription("Specifies an integer value indicating the maximum size that a log file should be allowed to reach (in bytes) before starting a new log file.")]
        [IntegerValidator(ExcludeRange = false, MinValue = 1)]
        public int MaxFileSize
        {
            get { return (int)base["maxFileSize"]; }
            set { base["maxFileSize"] = value; }
        }
        [ConfigurationProperty("writeToSystemLog", DefaultValue = false, IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if errors should be writen to the Windows event log on the web server.  Otherwise, 'false'.")]
        //[SubclassTypeValidator(typeof(Boolean))]
        public bool WriteToSystemLog
        {
            get { return (bool)base["writeToSystemLog"]; }
            set { base["writeToSystemLog"] = value; }
        }
        [ConfigurationProperty("systemLogName", DefaultValue = "Application", IsRequired = false)]
        [SettingsDescription("A string value indicating the name of the Windows event log to write to when system event logging is enabled.")]
        //[StringValidator(MinLength = 1)]
        public string SystemLogName
        {
            get { return (string)base["systemLogName"]; }
            set { base["systemLogName"] = value; }
        }
        #endregion
    }

}
