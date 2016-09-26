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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RainstormStudios.Data.Linq
{
    /// <summary>
    /// When an instance of this class is attached to a Linq2Sql data context's "Log" parameter, it outputs the generated T-SQL commands.
    /// </summary>
    public class LinqDebugWriter : System.IO.TextWriter
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        static LinqDebugWriterConfiguration
            _config;
        static DateTime
            _lastWriteTime = DateTime.MinValue;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
        //***************************************************************************
        // Static Properties
        // 
        public bool LogToFile
        {
            get { return _config.LogToFile; }
        }
        public bool LogToConsole
        {
            get { return _config.LogToConsole; }
        }
        public string LogPath
        {
            get { return _config.LogFilePath; }
        }
        public string[] ExcludedServers
        {
            get { return _config.GetExcludedServerList(); }
        }
        public bool AppendOutput
        {
            get { return _config.AppendOutput; }
        }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public LinqDebugWriter()
            : base()
        {
            if (_config == null)
                _config = this.GetConfig();
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public override void Write(char[] buffer, int index, int count)
        {
            this.Write(new String(buffer, index, count));
        }
        public override void Write(string value)
        {
#if DEBUG
            if (HttpContext.Current != null)
            {
                if (!HttpContext.Current.IsDebuggingEnabled && _config.OnlyInDebugMode)
                    return;

                string[] exclServers = this.ExcludedServers;
                if (HttpContext.Current != null && HttpContext.Current.Server != null && exclServers.Length > 0)
                {
                    // This is just an extra check to make sure we're not on a production server.
                    bool onExcludedServer = false;
                    for (int i = 0; i < exclServers.Length; i++)
                        if (HttpContext.Current.Server.MachineName.StartsWith(exclServers[i]))
                            onExcludedServer = true;
                    if (onExcludedServer)
                        return;
                }
            }

            if (_config.LogToFile)
            {
                string fn = System.Web.Hosting.HostingEnvironment.MapPath(LogPath);
                using (System.IO.FileStream fs = new System.IO.FileStream(fn, (_config.AppendOutput ? System.IO.FileMode.Append : System.IO.FileMode.Create), System.IO.FileAccess.Write))
                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(fs))
                {
                    if (DateTime.Now > _lastWriteTime.AddMinutes(_config.TimeBetweenDateStamps))
                        sr.WriteLine("--== {0} ==--", DateTime.Now.ToString("MM-dd-yyyy  HH:mm:ss"));
                    else if (DateTime.Now > _lastWriteTime.AddSeconds(_config.TimeBetweenTimestamps))
                        sr.WriteLine("--== {0} ==--", DateTime.Now.ToString("HH:mm:ss"));
                    sr.WriteLine(value);
                }
            }
            if (_config.LogToConsole)
            {
                if (DateTime.Now > _lastWriteTime.AddMinutes(_config.TimeBetweenDateStamps))
                    System.Diagnostics.Debug.WriteLine("--== {0} ==--", DateTime.Now.ToString("MM-dd-yyyy  HH:mm:ss"));
                else if (DateTime.Now > _lastWriteTime.AddSeconds(_config.TimeBetweenTimestamps))
                    System.Diagnostics.Debug.WriteLine("--== {0} ==--", DateTime.Now.ToString("HH:mm:ss"));
                System.Diagnostics.Debug.WriteLine(value);
            }
#endif
            _lastWriteTime = DateTime.Now;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private LinqDebugWriterConfiguration GetConfig()
        {
            LinqDebugWriterConfiguration config = LinqDebugWriterConfiguration.Config;
            if (config == null)
                throw new ConfigurationErrorsException("You must specify a LinqDebugWriter section in the application's .config file.");
            else
                return config;
        }
        #endregion
    }
    public class LinqDebugWriterConfiguration : System.Configuration.ConfigurationSection
    {
        #region Declarations
        //***************************************************************************
        // Public Fields
        // 
        public static readonly LinqDebugWriterConfiguration
            Config = (ConfigurationManager.GetSection("LinqDebugWriter") as LinqDebugWriterConfiguration);
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("logToFile", DefaultValue = "false", IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' of output should be sent to a log file.  Otherwise, 'false'.")]
        public bool LogToFile
        {
            get { return (bool)base["logToFile"]; }
            set { base["logToFile"] = value; }
        }
        [ConfigurationProperty("logToConsole", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if output should be sent to the debug console.  Otherwise, 'false'.")]
        public bool LogToConsole
        {
            get { return (bool)base["logToConsole"]; }
            set { base["logToConsole"] = value; }
        }
        [ConfigurationProperty("logFilePath", DefaultValue = "~/lingDebug.txt", IsRequired = false)]
        [SettingsDescription("A string value containing the name of the file to write to.")]
        public string LogFilePath
        {
            get { return (string)base["logFilePath"]; }
            set { base["logFilePath"] = value; }
        }
        [ConfigurationProperty("appendOutput")]
        [SettingsDescription("A boolean value indicating 'true' if new output should be appended to the file.  Otherwise, 'false'.")]
        public bool AppendOutput
        {
            get { return (bool)base["appendOutput"]; }
            set { base["appendOutput"] = value; }
        }
        [ConfigurationProperty("onlyInDebugMode", DefaultValue = "true", IsRequired = false)]
        [SettingsDescription("A boolean value indicating 'true' if the Linq2Sql debug output should only happen when the application is in debug mode.  Otherwise, 'false'.")]
        public bool OnlyInDebugMode
        {
            get { return (bool)base["onlyInDebugMode"]; }
            set { base["onlyInDebugMode"] = value; }
        }
        [ConfigurationProperty("excludedServers", IsDefaultCollection = true)]
        [SettingsDescription("Specifies a list of servers on which the debug writer will not be allowed to function.")]
        public ServerElementCollection ExcludedServers
        {
            get { return (ServerElementCollection)base["excludedServers"]; }
            set { base["excludedServers"] = value; }
        }
        [ConfigurationProperty("timeBetweenTimeStamps", DefaultValue = 5, IsRequired = false)]
        [SettingsDescription("An integer value indicating the number of seconds between timestamps in the output.  Default is '5'.")]
        [IntegerValidator()]
        public int TimeBetweenTimestamps
        {
            get { return (int)base["timeBetweenTimeStamps"]; }
            set { base["timeBetweenTimeStampes"] = value; }
        }
        [ConfigurationProperty("timeBetweenDateStamps", DefaultValue = 30, IsRequired = false)]
        [SettingsDescription("An integer value indicating the number of minutes between full date/time stamps in the output.  Default is '30'.")]
        [IntegerValidator()]
        public int TimeBetweenDateStamps
        {
            get { return (int)base["timeBetweenDateStamps"]; }
            set { base["timeBetweenDateStamps"] = value; }
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public string[] GetExcludedServerList()
        {
            if (Config != null)
            {
                List<string> lst = new List<string>();
                foreach (ServerElement el in Config.ExcludedServers)
                    lst.Add(el.ServerName);
                return lst.ToArray();
            }
            else
                return new string[0];
        }
        #endregion
    }
    public class ServerElement : System.Configuration.ConfigurationElement
    {
        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [ConfigurationProperty("serverName", IsRequired = true, IsKey = true)]
        [SettingsDescription("A string value indicating the server name.")]
        public string ServerName
        {
            get { return (string)base["serverName"]; }
            set { base["serverName"] = value; }
        }
        #endregion
    }
    [ConfigurationCollection(typeof(ServerElement))]
    public class ServerElementCollection : System.Configuration.ConfigurationElementCollection
    {
        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerElement)element).ServerName;
        }
        #endregion
    }
}
